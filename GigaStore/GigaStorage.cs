using GigaStore.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using static GigaStore.Propagate;
using System.Threading;
using Grpc.Core;
using System.Drawing.Printing;
using GigaStore.Services;
using System.Diagnostics;

namespace GigaStore
{
    public class GigaStorage
    {
        private static readonly GigaStorage _instance = new GigaStorage();
        private MultiKeyDictionary<string, int, string> _gigaObjects;
        private MultiKeyDictionary<string, int, Semaphore> _semObjects;

        public string ServerId { get; set; }
        public int ServersCount { get; set; }
        public bool IsAdvanced { get; set; }
        public int MinDelay { get; set; }
        public int MaxDelay { get; set; }

        private Dictionary<string, GrpcChannel> _chanels;
        private Dictionary<string, PropagateClient> _clients;

        private int replicationFactor;
        private AutoResetEvent[] _handles;
        private bool _inited = false;
        private bool _frozen = false;
        private int _replicationFactor;

        // Lists masters for all partitions. First string is the partition and second is the serverId of its master
        private Dictionary<string, string> _master;
        // Lists status of all servers
        private Dictionary<string, bool> _down;
        // Lists servers current server talks with. First entry for partition and second for servers that sahre said partition
        private Dictionary<string, List<string>> _servers;


        private GigaStorage()
        {
            _gigaObjects = new MultiKeyDictionary<string, int, string>();
            _semObjects = new MultiKeyDictionary<string, int, Semaphore>();
            _chanels = new Dictionary<string, GrpcChannel>();
            _clients = new Dictionary<string, PropagateClient>();
            _master = new Dictionary<string, string>();
            _down = new Dictionary<string, bool>();
            _servers = new Dictionary<string, List<string>>();
        }

        // Singleton Design Patter "constructor"
        public static GigaStorage GetGigaStorage()
        {
            return _instance;
        }

        // Starts the GRPC clients with the other servers and sets up all axiliary lists
        public void Init (List<string> servers, List<string> urls)
        {
            // If it has already been initiated then ignore
            if (_inited)
            {
                return;
            }

            for (int i = 0; i <= ServersCount; i++)
            {
                if (servers[i] != ServerId)
                {
                    _chanels.Add(servers[i], GrpcChannel.ForAddress(urls[i]));
                    _clients.Add(servers[i], new PropagateClient(_chanels[servers[i]]));
                }
                _down.Add(servers[i], false);
            }
        }


        public void MakePartition(string partition, List<string> servers, string master)
        {
            _servers.Add(partition, servers);
            _master.Add(partition, master);
        }

        /*
         * Base Version
         * 
         */

        // Base version Write
        public void Write(string partition_id, int object_id, string value)
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

            // Stores the value and a semaphore for this object
            _gigaObjects.Add(partition_id, object_id, value);
            _semObjects.Add(partition_id, object_id, new Semaphore(1, 1));


            // Stores the value on all servers that share this partition
            propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
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
        public void Lock(string partition, int object_id)
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
        public void Store(string partition, int object_id, string value)
        {
            _gigaObjects.Add(partition, object_id, value);
            _semObjects[partition][object_id].Release();

            Console.WriteLine("UNLOCKED partition: " + partition+ " object: " + object_id);
        }

        // Base Version Read
        public string Read(string partition, int objectId)
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
        public void WriteAdvanced(string partition, int object_id, string value)
        {
            PropagateRequest propagateRequest;
            string server;

            _gigaObjects.Add(partition, object_id, value);

            // Propagate to all servers without waiting for response
            for (int i = 0; i < _servers[partition].Count; i++)
            {
                propagateRequest = new PropagateRequest { PartitionId = partition, ObjectId = object_id, Value = value };
                server = _servers[partition][i];
                Console.WriteLine("ServerID: " + server);
                try
                {
                    _clients[server].PropagateServersAdvanced(propagateRequest);
                }
                catch
                {
                    // If fails then the server is down
                    DeadServerReport(server);
                }
            }
            Console.WriteLine("Value Propagated");
        }

        // Advanced Version Read
        public string ReadAdvanced(string partition_id, int object_id)
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
        public void StoreAdvanced(string partition_id, int object_id, string value)
        {
            _gigaObjects.Add(partition_id, object_id, value);
        }

        /*
         * Auxiliary
         * 
         */

        // Starts all process related to finding out a server is down
        public void DeadServerReport(string down_server)
        {
            Console.WriteLine("Down Server: " + down_server);
            // Needs to find a new master for all partitions, this server is master off
            foreach (KeyValuePair<string, string> master in _master)
            {
                if (master.Value == down_server)
                {
                    ChangeMasterRequest(down_server, master.Key, 0);
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

            try
            {
                Console.WriteLine("changing master from: " + down_server + " to " + new_server);

                ChangeRequest changeRequest = new ChangeRequest { ServerId = down_server};
                await _clients[new_server].ChangeMasterAsync(changeRequest);
            }
            catch
            {
                // Keep trying until it works with another server
                ChangeMasterRequest(down_server, partition, iteration + 1);
                // If it failed, the new server is also down
                DeadServerReport(new_server);
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
            try
            {
                Console.WriteLine("Notifying server: " + server + " about down server " + down_server+ " with new " + new_server + " for partition " + partition);
                ChangeNotificationRequest changeNotificationRequest = new ChangeNotificationRequest { ServerId = down_server, NewServerId = new_server, PartitionId = partition };
                _clients[server].ChangeMasterNotificationAsync(changeNotificationRequest);
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
            // If already knew then ignore
            if (_down[old_server])
            {
                return;
            }

            await MasterUpdateAsync(old_server, new_server, partition);
            Console.WriteLine("IM NOTIFIED THAT SERVER " + old_server + " IS DOWN for partition: " + partition);

        }

        // Responds is current server is master of partition_id
        public bool IsMaster(string partition_id)
        {
            return _master[partition_id] == ServerId;
        }

        // Marks old server as down, sets new master for partition, removes old server from server list and verifies if another server needs to be contacted
        public async Task MasterUpdateAsync (string old_server, string new_server, string partition)
        {
            Console.WriteLine("Donwing server: " + old_server + " for: " + new_server + " on partition: " + partition);
            _down[old_server] = true;

            _master[partition] = new_server;

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


            // Only propagate if in advanced mode
            if (IsAdvanced)
            {
                // If not enough servers propagate partition with more servers
                if (_servers[partition].Count < replicationFactor)
                {
                    foreach(KeyValuePair<string, PropagateClient> server in _clients )
                    {
                        // Ignore current server, down servers and servers this partition already propagates to
                        if (server.Key == ServerId || _down[server.Key] || _servers[partition].Contains(server.Key))
                        {
                            continue;
                        }
                        try
                        {
                            Console.WriteLine("Asking server: " + server.Key + " to replicate partition: " + partition);
                            var replicateRequest = _clients[server.Key].ReplicatePartition();
                            foreach (KeyValuePair<int, string> entry in _gigaObjects[partition])
                            {
                                await replicateRequest.RequestStream.WriteAsync(new ReplicateRequest
                                {
                                    PartitionId = partition,
                                    ObjectId = entry.Key,
                                    Value = entry.Value
                                });

                            }
                            await replicateRequest.RequestStream.CompleteAsync();
                            break;
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

        // Gets Partition
        public Dictionary<int, string> GetPartition (string partition)
        {
            return _gigaObjects[partition];
        } 

        // TODO Test this
        // Checks status of server_id in case a client suspects is down
        public async Task CheckStatusAsync (string server)
        {
            try
            {
                if (!_down[server])
                {
                    CheckServersRequest checkServersRequest = new CheckServersRequest { };
                    await _clients[server].CheckStatusServersAsync(checkServersRequest);
                }
            }
            catch
            {
                // If server is down
                DeadServerReport(server);
            }
        }



        public string GetMaster(string partition)
        {
            return _master[partition];
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

        public Boolean IsFrozen()
        {
            return _frozen;
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

        public void FreezeServer()
        {
            Console.WriteLine("Freezing server");
            _frozen = true;
        }

        public void UnfreezeServer()
        {
            Console.WriteLine("Unfreezing server");
            _frozen = false;
        }

    }
    public class Object
    {
        public string PartitionId { get; }
        public int ObjectId { get; }
        public string Value { get; }

        public Object(string partitionId, int objectId, string value)
        {
            PartitionId = partitionId;
            ObjectId = objectId;
            Value = value;
        }
    }
}
