using GigaStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using static GigaStore.Propagate;
using System.Threading;
using Grpc.Core;

namespace GigaStore
{
    public class GigaStorage
    {
        private static readonly GigaStorage _instance = new GigaStorage();
        private MultiKeyDictionary<int, int, string> _gigaObjects;
        private MultiKeyDictionary<int, int, Semaphore> _semObjects;
        private int _serverId;
        private int _numberOfServers;
        private int _aliveServers;
        private GrpcChannel[] _chanels;
        private PropagateClient[] _clients;
        private AutoResetEvent[] _handles;
        private bool _inited = false;

        // Lists masters for all partitions
        private int[] _master;
        // Lists status of all servers
        private bool[] _down;
        // Lists servers current server talks with. First entry for partition and second for servers that sahre said partition
        private List<List<int>> _servers;


        private GigaStorage()
        {
            _gigaObjects = new MultiKeyDictionary<int, int, string>();
            _semObjects = new MultiKeyDictionary<int, int, Semaphore>();
        }

        public static GigaStorage GetGigaStorage()
        {
            return _instance;
        }

        public void Init ()
        // Starts the GRPC clients with the other servers and sets up all axiliary lists
        {
            // If it has already been iniated then ignore
            if (_inited)
            {
                return;
            }
            // +1 So the server_id and respective index in the array match
            _chanels = new GrpcChannel[_numberOfServers + 1];
            _clients = new PropagateClient[_numberOfServers + 1];
            _master = new int[_numberOfServers + 1];
            _down = new bool[_numberOfServers + 1];
            _servers = new List<List<int>>();
            _aliveServers = _numberOfServers;
            string url;
            _servers.Add(new List<int>());

            // Connects to all other servers
            for (int i = 1; i <= _numberOfServers; i++)
            {
                if (i != _serverId)
                {
                    url = "https://localhost:500" + i;

                    Console.WriteLine("I: " + i + " URL: " + url);

                    _chanels[i] = GrpcChannel.ForAddress(url);
                    _clients[i] = new PropagateClient(_chanels[i]);
                }
                _servers.Add(new List<int>());
                _master[i] = i;
                _down[i] = false;
            }

            // Ads servers with same partition to _servers list
            int interval = _numberOfServers / 2;
            int server_id;

            for (int i = 1, y = 0; i <= interval; i++, y++)
            {
                server_id = _serverId + i;

                if (server_id > _numberOfServers)
                {
                    server_id = server_id - _numberOfServers;
                }
                Console.WriteLine("Server ID: " + server_id);
                _servers[_serverId].Add(server_id);
            }

            _inited = true;
        }

        public void Write(int partition_id, int object_id, string value)
        {
            Init();
            PropagateRequest propagateRequest;
            var lockRequest = new LockRequest { PartitionId = partition_id, ObjectId = object_id }; ;
            int interval = _numberOfServers / 2;
            Console.WriteLine("Interval: " + interval);
            int server_id;
            _handles = new AutoResetEvent[interval];
            Thread[] threads = new Thread[interval];

            for (int i = 1, y = 0; i <= interval; i++, y++)
            {
                server_id = _serverId + i;

                if (server_id > _numberOfServers)
                {
                    server_id = server_id - _numberOfServers;
                }

                _handles[y] = new AutoResetEvent(false);
                int t = y;
                int s = server_id;
                var lockRequestTmp = lockRequest;

                threads[y] = new Thread(async () => await ThreadLockAsync(t, s, lockRequestTmp));
            }

            for (int x = 0; x < threads.Length; x++)
            {
                Console.WriteLine("Starting thread: " + x);
                threads[x].Start();
            }

            Console.WriteLine("Awating lock handles");
            WaitHandle.WaitAll(_handles);
            Console.WriteLine("Done lock handles");

            _gigaObjects.Add(partition_id, object_id, value);
            _semObjects.Add(partition_id, object_id, new Semaphore(1, 1));

            _handles = new AutoResetEvent[interval];
            threads = new Thread[interval];

            for (int i = 1, y = 0; i <= interval; i++, y++)
            {
                propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
                server_id = _serverId + i;
                if (server_id > _numberOfServers)
                {
                    server_id = server_id - _numberOfServers;
                }
                Console.WriteLine("ServerID: " + server_id);
                _handles[y] = new AutoResetEvent(false);
                int t = y;
                int s = server_id;
                var propagateRequestTmp = propagateRequest;

                threads[y] = new Thread(async () => await ThreadPropagateAsync(t, s, propagateRequestTmp));
                Console.WriteLine("Value Propagated");

            }

            for (int x = 0; x < threads.Length; x++)
            {
                Console.WriteLine("Starting thread: " + x);
                threads[x].Start();
            }

            Console.WriteLine("Awating propagate handles");
            WaitHandle.WaitAll(_handles);
            Console.WriteLine("Done propagate handles");
        }


        // Method to facilitate ThreadLock
        public async Task ThreadLockAsync (int t, int serverID, LockRequest lockRequest)
        {
            Console.WriteLine("thread: " + t);
            await _clients[serverID].LockServersAsync(lockRequest);
            _handles[t].Set();
        }

        // Method to facilitate ThreadPropagate
        public async Task ThreadPropagateAsync(int t, int serverID, PropagateRequest propagateRequest)
        {
            Console.WriteLine("thread: " + t);
            await _clients[serverID].PropagateServersAsync(propagateRequest);
            _handles[t].Set();
        }

        public void Lock(int partition_id, int object_id)
        {
            Console.WriteLine("LOCKING partition: " + partition_id + " object: " + object_id);

            try
            {
                _semObjects[partition_id][object_id].WaitOne();
            }
            catch (KeyNotFoundException)
            {
                _semObjects.Add(partition_id, object_id, new Semaphore(1,1));
                _semObjects[partition_id][object_id].WaitOne();

            }
            Console.WriteLine("LOCKED partition: " + partition_id + " object: " + object_id);
        }

        public void Store(int partition_id, int object_id, string value)
        {
            _gigaObjects.Add(partition_id, object_id, value);
            _semObjects[partition_id][object_id].Release();

            Console.WriteLine("UNLOCKED partition: " + partition_id + " object: " + object_id);
        }


        public string Read(int partition_id, int object_id)
        {
            string value;
            try
            {
                Console.WriteLine("LOCKED READ partition: " + partition_id + " object: " + object_id);
                _semObjects[partition_id][object_id].WaitOne();
                value = _gigaObjects[partition_id][object_id];
                _semObjects[partition_id][object_id].Release();
                Console.WriteLine("UNLOCKED READ partition: " + partition_id + " object: " + object_id);
            }
            catch (KeyNotFoundException)
            {
                value = "N/A";
            }
            return value;
        }

        public void SetServerId(int serverId)
        {
            _serverId = serverId;
        }

        public int GetServerId()
        {
            return _serverId;
        }

        public void SetNumberOfServers(int numberOfServers)
        {
            _numberOfServers = numberOfServers;
        }

        public void WriteAdvanced(int partition_id, int object_id, string value)
        {
            Init();
            PropagateRequest propagateRequest;
            int server_id;

            _gigaObjects.Add(partition_id, object_id, value);

            for (int i = 0; i < _servers[partition_id].Count; i++)
            {
                propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
                server_id = _servers[partition_id][i];
                Console.WriteLine("ServerID: " + server_id);
                try
                {
                    _clients[server_id].PropagateServersAdvanced(propagateRequest);
                }
                catch
                {
                    ChangeMasterRequest(server_id, server_id + 1);
                }

            }
            Console.WriteLine("Value Propagated");
        }

        // Notifies new master and all other servers that a server is down
        public async void ChangeMasterRequest(int old_server_id, int server_id)
        {

            Console.WriteLine("Server: " + old_server_id + " is down");
            // If here for the first time
            

            try
            {
                Console.WriteLine("changing master from: " + old_server_id + " to " + server_id);

                ChangeRequest changeRequest = new ChangeRequest { ServerId = old_server_id };
                await _clients[server_id].ChangeMasterAsync(changeRequest);


            }
            catch
            {
                // Keep trying until it works
                ChangeMasterRequest(old_server_id, server_id + 1);
                ChangeMasterRequest(server_id, server_id + 1);
            }
            if (!_down[old_server_id])
            {
                await masterUpdateAsync(old_server_id, server_id);
                for (int x = 1; x <= _numberOfServers; x++)
                {
                    if (x == _serverId || x == server_id || _down[x])
                    {
                        continue;
                    }
                    ChangeMasterNotificationRequest(old_server_id, x, server_id);
                }
            }
        }

        public void ChangeMasterNotificationRequest (int down_server_id, int server_id, int new_server)
        {
            try
            {
                Console.WriteLine("Notifying server: " + server_id + " about down server " + down_server_id + " with new " + new_server);
                ChangeNotificationRequest changeNotificationRequest = new ChangeNotificationRequest { ServerId = down_server_id, NewServerId = new_server };
                _clients[server_id].ChangeMasterNotificationAsync(changeNotificationRequest);
            }
            catch
            {
                // Se nao responde e pq foi abaixo
                ChangeMasterRequest(server_id, server_id + 1);

            }
        }
    

        public string ReadAdvanced(int partition_id, int object_id)
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

        public void StoreAdvanced(int partition_id, int object_id, string value)
        {
            _gigaObjects.Add(partition_id, object_id, value);
        }

        public async Task ChangeMasterAsync(int server_id)
        {
            Init();
            await masterUpdateAsync(server_id, _serverId);

            Console.WriteLine("IM NOW MASTER OF PARTITION " + server_id);
        }

        public async Task ChangeMasterNotificationAsync(int old_server, int new_server)
        {
            Init();
            // If already knew then ignore
            if (_down[old_server])
            {
                return;
            }

            await masterUpdateAsync(old_server, new_server);

            Console.WriteLine("IM NOTIFIED THAT SERVER " + old_server + " IS DOWN");
        }

        public bool isMaster(int partition_id)
        {
            Init();
            return _master[partition_id] == _serverId;
        }

        // Marks old server as down, sets new master for all partitions ruled by old server, removes old server from server list and verifies if another server needs to be contacted
        public async Task masterUpdateAsync (int old_server, int new_server)
        {
            // If already new ignore
            if (_down[old_server])
            {
                return;
            }

            Console.WriteLine("Donwing server: " + old_server + " for: " + new_server);
            _down[old_server] = true;
            _aliveServers--;

            for (int i = 1; i < _master.Length; i++)
            {
                if (_master[i] == old_server)
                {
                    _master[i] = new_server;
                }
            }

            for (int i = 1; i < _servers.Count; i++)
            {
                try
                {
                    _servers[i].Remove(old_server);
                }
                catch
                {
                    // ignore
                }
            }


            // If not enough servers share the master partitions request more servers to share it
            
            for (int i = 1; i < _master.Length; i++)
            {
                if (_master[i] == _serverId)
                {
                    // If not enough servers propagate
                    if (_servers[i].Count < _aliveServers / 2)
                    {
                        ReplicateNewRequest replicateNewRequest = new ReplicateNewRequest { PartitionId = i, ServerId = _serverId};
                        int server_id;
                        for (int x = i + 1; x != i; x++)
                        {
                            server_id = x;
                            if (server_id > _numberOfServers)
                            {
                                server_id = server_id - _numberOfServers;
                            }

                            if (_down[server_id] || _servers[i].Contains(server_id))
                            {
                                continue;
                            }
                            try
                            {
                                Console.WriteLine("Asking server: " + server_id + " to replicate partition: " + i);
                                await _clients[server_id].ReplicateNewAsync(replicateNewRequest);
                                break;
                            }
                            catch
                            {
                                // skip this one
                            }

                        }

                    }
                
                }

            }

        }

        public Dictionary<int, string> getPartition (int parttion_id)
        {
            return _gigaObjects[parttion_id];
        } 
        public async Task ReplicateNewAsync (int partition_id, int server_id)
        {
            
            ReplicateRequest replicateRequest = new ReplicateRequest { PartitionId = partition_id };
            var objects = _clients[server_id].ReplicatePartition(replicateRequest);

            await foreach (var giga in objects.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine("REPLICATE PARTITIOM P: " + giga.PartitionId + " ID: " + giga.ObjectId + " Value: " + giga.Value);
                _gigaObjects.Add(giga.PartitionId, giga.ObjectId, giga.Value);
            }
            
        }

        // TODO Test this
        public async Task CheckStatusAsync (int server_id)
        {
            try
            {
                CheckServersRequest checkServersRequest = new CheckServersRequest { };
                await _clients[server_id].CheckStatusServersAsync(checkServersRequest);
            }
            catch
            {
                // If server is down
                ChangeMasterRequest(server_id, _serverId + 1);
            }
        }

    }

}
      
