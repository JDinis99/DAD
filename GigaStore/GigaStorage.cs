using GigaStore.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using static GigaStore.Propagate;
using System.Threading;
using System.Diagnostics;
using Grpc.Core;

namespace GigaStore
{
    public class GigaStorage
    {
        private static readonly GigaStorage _instance = new GigaStorage();
        private MultiKeyDictionary<string, string, string> _gigaObjects;
        private MultiKeyDictionary<string, string, Semaphore> _semObjects;
        private MultiKeyDictionary<string, string, int> _objectVersion;

        public string ServerId { get; set; }
        public bool IsAdvanced { get; set; }
        public int MinDelay { get; set; }
        public int MaxDelay { get; set; }

        private Dictionary<string, GrpcChannel> _channels;
        private Dictionary<string, PropagateClient> _clients;
        private Dictionary<string, Semaphore> _semPartitions;

        private AutoResetEvent[] _handles;
        private bool _inited = false;
        //private bool _frozen = false;
        private Semaphore _frozen = new Semaphore(1, 1);
        private int _replicationFactor;

        // Lists masters for all partitions. First string is the partition and second is the serverId of its master
        private Dictionary<string, string> _master;
        // Lists status of all servers
        private Dictionary<string, bool> _down;
        // Lists servers current server talks with. First entry for partition and second for servers that sahre said partition
        private Dictionary<string, List<string>> _servers;

        private const int DEADLINE_GRPC = 30;

        private GigaStorage()
        {
            
        }

        // Singleton Design Patter "constructor"
        public static GigaStorage GetGigaStorage()
        {
            return _instance;
        }

        // Starts the GRPC clients with the other servers and sets up all axiliary lists
        public void Init (List<string> servers, List<string> urls)
        {
            Console.WriteLine("INIT");
            // If it has already been initiated then ignore
            if (_inited)
            {
                return;
            }
            _inited = true;

            // Set up lists
            _gigaObjects = new MultiKeyDictionary<string, string, string>();
            _semObjects = new MultiKeyDictionary<string, string, Semaphore>();
            _semPartitions = new Dictionary<string, Semaphore>();
            _channels = new Dictionary<string, GrpcChannel>();
            _clients = new Dictionary<string, PropagateClient>();
            _master = new Dictionary<string, string>();
            _down = new Dictionary<string, bool>();
            _servers = new Dictionary<string, List<string>>();
            _objectVersion = new MultiKeyDictionary<string, string, int>();

            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i] != ServerId)
                {
                    _channels.Add(servers[i], GrpcChannel.ForAddress(urls[i]));
                    _clients.Add(servers[i], new PropagateClient(_channels[servers[i]]));
                    _down.Add(servers[i], false);
                    Console.WriteLine("server: " + servers[i]);
                }
            }
        }


        public void MakePartition(string partition, List<string> servers, string master)
        {
            // Delete old partition if existed
            try
            {
                _servers.Remove(partition);
                Console.WriteLine("REMOVED Partition: " + partition + " master: " + master);
            }
            catch
            {
                // If not ignored
            }

            Console.WriteLine("New Partition: " + partition + " master: " + master);
            _master.Add(partition, master);
            _semPartitions.Add(partition, new Semaphore(1, 1));
            List<String> tmp = new List<string>();
            foreach(String server in servers)
            {
                if (ServerId != server && server != "")
                {
                    tmp.Add(server);
                    Console.WriteLine("adding server: <" + server + ">");
                }
            }
            _servers.Add(partition, tmp);
        }

        /*
         * Base Version
         * 
         */

        // Base version Write
        public void Write(string partition_id, string object_id, string value)
        {
            // Locks object on all servers that share this partition
            PropagateRequest propagateRequest;
            var lockRequest = new LockRequest { PartitionId = partition_id, ObjectId = object_id }; ;
            _handles = new AutoResetEvent[_servers[partition_id].Count];
            Thread[] threads = new Thread[_servers[partition_id].Count];

            Console.WriteLine("Count " + _servers[partition_id].Count);

            for (int i = 0; i < _servers[partition_id].Count ; i++)
            {
                _handles[i] = new AutoResetEvent(false);
                int t = i;
                string s = _servers[partition_id][i];
                var lockRequestTmp = lockRequest;

                threads[i] = new Thread(async () => await ThreadLockAsync(t, s, lockRequestTmp));
            }

            for (int x = 0; x < threads.Length; x++)
            {
                Console.WriteLine("Starting thread: " + x);
                threads[x].Start();
            }

            if (threads.Length != 0)
            {
                // Waits for all locks
                Console.WriteLine("Awating lock handles");
                WaitHandle.WaitAll(_handles);
                Console.WriteLine("Done lock handles");
            }

            // Get previous version of object
            int currentVersion = getVersion(partition_id, object_id);

            // Stores the value and a semaphore for this object
            _gigaObjects.Add(partition_id, object_id, value);
            _semObjects.Add(partition_id, object_id, new Semaphore(1, 1));
            _objectVersion.Add(partition_id, object_id, currentVersion + 1);


            // Stores the value on all servers that share this partition
            propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value, Version = currentVersion+1};
            _handles = new AutoResetEvent[_servers[partition_id].Count];
            threads = new Thread[_servers[partition_id].Count];

            for (int i = 0; i < _servers[partition_id].Count; i++)
            {

                Console.WriteLine("ServerID: " + i);
                _handles[i] = new AutoResetEvent(false);
                int t = i;
                string s = _servers[partition_id][i];
                var propagateRequestTmp = propagateRequest;

                threads[i] = new Thread(async () => await ThreadPropagateAsync(t, s, propagateRequestTmp));
                Console.WriteLine("Value Propagated");

            }

            for (int x = 0; x < threads.Length; x++)
            {
                Console.WriteLine("Starting thread: " + x);
                threads[x].Start();
            }

            if (threads.Length != 0)
            {
                // Waits for all propagations
                Console.WriteLine("Awating propagate handles");
                WaitHandle.WaitAll(_handles);
                Console.WriteLine("Done propagate handles");
            }
        }


        // Method to facilitate Lock Requests to other servers with threads
        public async Task ThreadLockAsync (int t, string server, LockRequest lockRequest)
        {
            Console.WriteLine("thread: " + t);
            try
            {
                await _clients[server].LockServersAsync(lockRequest);
            }
            catch
            {
                // If fails then the server is down
                DeadServerReport(server);
            }
            _handles[t].Set();
        }

        // Method to facilitate Propagate Requests to other servers with threads
        public async Task ThreadPropagateAsync(int t, string server, PropagateRequest propagateRequest)
        {
            try
            {
                await _clients[server].PropagateServersAsync(propagateRequest);
            }
            catch
            {
                // If fails then the server is down
                DeadServerReport(server);
            }
            _handles[t].Set();
        }

        // Locks an object
        public void Lock(string partition, string object_id)
        {
            Console.WriteLine("LOCKING partition: " + partition + " object: " + object_id);

            try
            {
                _semObjects[partition][object_id].WaitOne();
            }
            catch (KeyNotFoundException)
            {
                // If there is no semaphore (in case of first write of this object) create one
                _semObjects.Add(partition, object_id, new Semaphore(1,1));
                _semObjects[partition][object_id].WaitOne();


            }
            Console.WriteLine("LOCKED partition: " + partition+ " object: " + object_id);
        }

        // Stores an object from propagation and releases the lock
        public void Store(string partition, string object_id, string value, int version)
        {
            int currentVersion = getVersion(partition, object_id);
            if (version > currentVersion)
            {
                _gigaObjects.Add(partition, object_id, value);
                _semObjects[partition][object_id].Release();
            }
            _objectVersion[partition][object_id] = version;

            Console.WriteLine("UNLOCKED partition: " + partition+ " object: " + object_id);
        }

        // Base Version Read
        public string Read(string partition, string objectId)
        {
            string value;
            try
            {
                _semObjects[partition][objectId].WaitOne();
                Console.WriteLine($"[READ] Locked (partition {partition}, object {objectId}).");
                value = _gigaObjects[partition][objectId];
                _semObjects[partition][objectId].Release();
                Console.WriteLine($"[READ] Unlocked (partition {partition}, object {objectId}).");
            }
            catch (KeyNotFoundException)
            {
                value = "N/A";
            }
            return value;
        }

        // Base Version List
        public List<Object> ListServer()
        {
            var objects = new List<Object>();
            foreach (var partitionId in _gigaObjects.Keys)
            {
                foreach (var objectId in _gigaObjects[partitionId].Keys)
                {
                    try
                    {
                        _semObjects[partitionId][objectId].WaitOne();
                        Console.WriteLine($"[LIST] Locked (partition {partitionId}, object {objectId}).");
                        var value = _gigaObjects[partitionId][objectId];
                        _semObjects[partitionId][objectId].Release();
                        Console.WriteLine($"[LIST] Unlocked (partition {partitionId}, object {objectId}).");

                        var obj = new Object(partitionId, objectId, value);
                        objects.Add(obj);
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine($"KeyNotFoundException: {e.Message}");
                    }
                }
            }
            return objects;
        }


        /*
         * Advanced Version
         * 
         */

        // Advanced Version Write
        public void WriteAdvanced(string partition, string object_id, string value)
        {
            string server;

            _gigaObjects.Add(partition, object_id, value);

            int currentVersion = getVersion(partition, object_id);
            _objectVersion.Add(partition, object_id, currentVersion + 1);

            PropagateRequest propagateRequest = new PropagateRequest { PartitionId = partition, ObjectId = object_id, Value = value, Version = currentVersion+1};
            // Propagate to all servers without waiting for response
            for (int i = 0; i < _servers[partition].Count; i++)
            {
                server = _servers[partition][i];
                string s = server;
                Console.WriteLine("ServerID: " + server);
                new Thread(() => ThreadWriteAdvanced(s, propagateRequest)).Start();
            }
            Console.WriteLine("Value Propagated");
        }

        public void ThreadWriteAdvanced(String server, PropagateRequest request)
        {
           Console.WriteLine("ServerID THREAD: " + server);
            try
            {
                _clients[server].PropagateServersAdvanced(request);
            }
            catch
            {
                // If fails then the server is down
                DeadServerReport(server);
            }
        }

        // Advanced Version Read
        public string ReadAdvanced(string partition_id, string object_id)
        {
            string value;
            try
            {
                value = _gigaObjects[partition_id][object_id];
            }
            catch (KeyNotFoundException)
            {
                value = "N/A";
            }
            return value;
        }

        // Advanced Version List
        public List<Object> ListServerAdvanced()
        {
            var objects = new List<Object>();
            foreach (var partitionId in _gigaObjects.Keys)
            {
                foreach (var objectId in _gigaObjects[partitionId].Keys)
                {
                    var value = _gigaObjects[partitionId][objectId];
                    var obj = new Object(partitionId, objectId, value);
                    objects.Add(obj);
                }
            }
            return objects;
        }

        // Stores value without blocking a semaphore
        public void StoreAdvanced(string partition_id, string object_id, string value, int version)
        {
            Console.WriteLine(partition_id + object_id + value + version);
            _semPartitions[partition_id].WaitOne();
            int currentVersion = getVersion(partition_id, object_id);
            if (version > currentVersion)
            {
                _gigaObjects.Add(partition_id, object_id, value);
                _objectVersion.Add(partition_id, object_id, version);
            }
            _semPartitions[partition_id].Release();
        }

        /*
         * Auxiliary
         * 
         */

        // Starts all process related to finding out a server is down
        public void DeadServerReport(string down_server)
        {
            if (_down[down_server])
            {
                return;
            }
            _down[down_server] = true;
            Console.WriteLine("DEAD Server: " + down_server);
            bool isMaster = false;
            // Needs to find a new master for all partitions, this server is master off
            foreach (KeyValuePair<string, string> master in _master)
            {
                if (master.Value == down_server)
                {
                    isMaster = true;
                    Console.WriteLine("DEAD Server: " + down_server + " Partition: " + master.Key);
                    ChangeMasterRequest(down_server, master.Key, 0);
                }
            }
            // If server wasnt a master
            if (!isMaster)
            {
                _ = MasterUpdateAsync(down_server, "-1", "-1");
                // Notify all other servers that old_server_id is down
                foreach (KeyValuePair<string, PropagateClient> server in _clients)
                {
                    // Ignore current server and down servers
                    if (server.Key == ServerId || server.Key == down_server || _down[server.Key])
                    {
                        continue;
                    }
                    ChangeMasterNotificationRequest(server.Key, down_server, "-1", "-1");
                }
            }
        }

        // Notifies new master and all other servers that down_server_id is down and that new_server_id is the new master
        public async void ChangeMasterRequest(string down_server, string partition, int iteration)
        {
            string new_server;

            // Iteration starts at 0
            if (iteration >= _servers[partition].Count )
            {
                Console.WriteLine("No more servers to propagate partition " + partition + " :(");
                return;
            }
            new_server = _servers[partition][iteration];
            if (new_server == down_server)
            {
                ChangeMasterRequest(down_server, partition, iteration + 1);
                return;
            }

            try
            {
                Console.WriteLine("changing master from: " + down_server + " to " + new_server);

                ChangeRequest changeRequest = new ChangeRequest { ServerId = down_server, PartitionId = partition};
                await _clients[new_server].ChangeMasterAsync(changeRequest, new CallOptions().WithDeadline(DateTime.UtcNow.AddSeconds(DEADLINE_GRPC)) );
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    // Dont report as dead
                }
                else
                {
                    // If it failed, the new server is also down
                    DeadServerReport(new_server);
                }
                // Keep trying until it works with another server
                ChangeMasterRequest(down_server, partition, iteration + 1);
                
                return;
            }

            // If we have succed in changing master we notify other servers
            
            await MasterUpdateAsync(down_server, new_server, partition);

            // Notify all other servers that old_server_id is down
            foreach(KeyValuePair<string, PropagateClient> server in _clients )
            {
                // Ignore current server, new master and down servers
                if (server.Key == ServerId || server.Key == new_server|| _down[server.Key])
                {
                    continue;
                }
                ChangeMasterNotificationRequest(server.Key, down_server, new_server, partition);
            }

        }

        // Notifies server_id that down_server_id is down and that the new master of the partions of down_server_id is new_server
        public void ChangeMasterNotificationRequest (string server, string down_server, string new_server, string partition)
        {
            Console.WriteLine("Notifying server: " + server + " about down server " + down_server + " with new " + new_server + " for partition " + partition);
            ChangeNotificationRequest changeNotificationRequest = new ChangeNotificationRequest { ServerId = down_server, NewServerId = new_server, PartitionId = partition };
            string s = server;
            new Thread(() => ThreadNotification(s, changeNotificationRequest)).Start();

        }

        public void ThreadNotification (string server, ChangeNotificationRequest request)
        {
            try
            {
                _clients[server].ChangeMasterNotificationAsync(request);
            }
            catch
            {
                // If fails then the server is down
                DeadServerReport(ServerId);

            }
        }
    
        // Changes Master of partition with current server and downs old server
        public async Task ChangeMasterAsync(string server, string partition)
        {
            await MasterUpdateAsync(server, ServerId, partition);

            Console.WriteLine("IM NOW MASTER OF PARTITION " + partition + " that used to belong to " + server);
        }

        // Changes Master of partition with new server and downs old server
        public async Task ChangeMasterNotificationAsync(string old_server, string new_server, string partition)
        {
            Console.WriteLine("IM NOTIFIED THAT SERVER " + old_server + " IS DOWN for partition: " + partition);
            await MasterUpdateAsync(old_server, new_server, partition);
        }

        // Marks old server as down, sets new master for partition, removes old server from server list and verifies if another server needs to be contacted
        // Recieves new_server as "-1" if old server wasnt a master
        public async Task MasterUpdateAsync (string old_server, string new_server, string partition)
        {
            Console.WriteLine("Donwing server: " + old_server + " for: " + new_server + " on partition: " + partition);
            _down[old_server] = true;

            // For every partition remove old server from propate list
            foreach (KeyValuePair<string, List<string>> server in _servers)
            {
                try
                {
                    server.Value.Remove(old_server);
                }
                catch
                {
                    // Ignore
                }
            }

            if (new_server != "-1")
            {
                _master[partition] = new_server;
            }

            // Only propagate if in advanced mode
            if (IsAdvanced)
            {
                // Check for all partitions current server is master off
                foreach (KeyValuePair<String, String> partitions in _master )
                {
                    if (partitions.Value != ServerId)
                    {
                        continue;
                    }
                    // If not enough servers propagate partition with more servers
                    
                    foreach(KeyValuePair<string, PropagateClient> server in _clients )
                    {
                        if (_servers[partitions.Key].Count < _replicationFactor - 1)
                        {

                            Console.WriteLine(" --------------------- NEED MORE REPLICAS --------------------- ");
                            Console.WriteLine("Count: " + _servers[partitions.Key].Count + " _replicationFactor: " + (_replicationFactor - 1));
                            // Ignore current server, down servers and servers this partition already propagates to
                            if (server.Key == ServerId || _down[server.Key] || _servers[partitions.Key].Contains(server.Key))
                            {
                                continue;
                            }
                            try
                            {
                                // TODO Some sleep to ensure its there is something to write??
                                Console.WriteLine("Asking server: " + server.Key + " to replicate partition: " + partitions.Key);
                                var replicateRequest = _clients[server.Key].ReplicatePartition();
                                foreach (KeyValuePair<string, string> entry in _gigaObjects[partitions.Key])
                                {
                                    await replicateRequest.RequestStream.WriteAsync(new ReplicateRequest
                                    {
                                        PartitionId = partitions.Key,
                                        ObjectId = entry.Key,
                                        Value = entry.Value,
                                        Version = _objectVersion[partitions.Key][entry.Key]
                                    });

                                }
                                await replicateRequest.RequestStream.CompleteAsync();
                                Console.WriteLine("Adding server: " + server.Key + " to partition: " + partitions.Key);
                                _servers[partitions.Key].Add(server.Key);
                                // Notify all other servers about change in replication
                                NotifyPropagator(server.Key, partitions.Key);
                                break;
                            }
                            catch (KeyNotFoundException)
                            {
                                _servers[partitions.Key].Add(server.Key);
                                // Ignore. Nothing to propagate yet
                            }
                            catch
                            {
                                // If it fails server is down
                                DeadServerReport(server.Key);
                            }
                        }
                    }

                }
            }
        }

        //Notifies all other servers that a new servers belongs to partition
        public void NotifyPropagator (string new_server, string partition)
        {
            foreach(KeyValuePair<string, PropagateClient> server in _clients)
            {
                // Ignore current server, new server and downed servers
                if (server.Key == ServerId || server.Key ==  new_server || _down[server.Key])
                {
                    continue;
                }
                Console.WriteLine("Notifying server: " + server.Key + " that server: " + new_server + " now belongs to partition: " + partition);
                NewPropagatorRequest newPropagatorRequest = new NewPropagatorRequest { ServerId = new_server, PartitionId = partition };
                server.Value.NewPropagator(newPropagatorRequest);
            }
        }

        // Stores a new server beloging to partition
        public void NewPropagator(string new_server, string partition)
        {
            Console.WriteLine("Notified that server: " + new_server + " now belongs to partition: " + partition);
            _servers[partition].Add(new_server);
        }

        // Gets Partition
        public Dictionary<string, string> GetPartition (string partition)
        {
            return _gigaObjects[partition];
        } 

        // Checks status of server_id in case a client suspects is down
        public async Task CheckStatusAsync (string server)
        {
            try
            {
                DEBUG($"[CHECK] SERVER {server}");
                if (!_down[server])
                {
                    DEBUG("[CHECK] NOT DOWN");
                    CheckServersRequest checkServersRequest = new CheckServersRequest { };
                    await _clients[server].CheckStatusServersAsync(checkServersRequest);
                }
                DEBUG("[CHECK] SUCCESS");
            }
            catch
            {
                DEBUG("[CHECK] EXCEPTION");
                // If server is down
                DeadServerReport(server);
                DEBUG("[CHECK] REPORTED");
            }
        }

        /*
        // Responds if the current server is the master of partition_id
        public bool IsMaster(string partition_id) {
            return _master[partition_id] == this.ServerId;
        }
        */

        public string GetMaster(string partitionId)
        {
            string masterId;
            try
            {
                masterId = _master[partitionId];
                return masterId;
            }
            catch
            {
                masterId = "";
                return masterId;
            }
        }


        public void ChangeReplicationFactor(int factor)
        {
            _replicationFactor = factor;
        }

        public void Delay()
        {
            Random r = new Random();
            int rInt = r.Next(this.MinDelay, this.MaxDelay);
            Thread.Sleep(rInt);
        }

        public void PrintStatus()
        {
            Console.WriteLine("Server up and running");
        }

        public bool Crash()
        {
            try
            {
                Process.GetCurrentProcess().Kill();
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Semaphore GetFrozenSemaphore()
        {
            return _frozen;
        }

        public void FreezeServer()
        {
            Console.WriteLine("Freezing server");
            _frozen.WaitOne();
        }

        public void UnfreezeServer()
        {
            Console.WriteLine("Unfreezing server");
            _frozen.Release();
        }

        public void DEBUG(string message) {
            // TODO dont forget to delete this function afterwards
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DEBUG: " + message);
            Console.ResetColor();
        }

        public int getVersion (string partition_id, string object_id)
        {
            int currentVersion = 0;
            try
            {
                currentVersion = _objectVersion[partition_id][object_id];
            }
            catch
            {
                // Ignore, first version
            }
            return currentVersion;
        }

        public void WaitUnfreeze()
        {
            _frozen.WaitOne();
            _frozen.Release();
        }

    }
    public class Object
    {
        public string PartitionId { get; }
        public string ObjectId { get; }
        public string Value { get; }

        public Object(string partitionId, string objectId, string value)
        {
            PartitionId = partitionId;
            ObjectId = objectId;
            Value = value;
        }
    }
}
