using GigaStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using static GigaStore.Propagate;
using System.Threading;

namespace GigaStore
{
    public class GigaStorage
    {
        private static readonly GigaStorage _instance = new GigaStorage();
        private MultiKeyDictionary<int, int, string> _gigaObjects;
        private MultiKeyDictionary<int, int, Semaphore> _semObjects;
        private int _serverId;
        private int _numberOfServers;
        private GrpcChannel[] _chanels;
        private PropagateClient[] _clients;
        private AutoResetEvent[] _handles;
        private bool _inited = false;

        // Lists masters for all partitions
        private int[] _master;
        // Lists status of all servers
        private bool[] _down;
        // Lists partitions of current server
        private List<int> _partitions;


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
            _partitions = new List<int>();
            string url;
            for (int i = 1; i <= _numberOfServers; i++)
            {
                if (i != _serverId)
                {
                    url = "https://localhost:500" + i;

                    Console.WriteLine("I: " + i + " URL: " + url);

                    _chanels[i] = GrpcChannel.ForAddress(url);
                    _clients[i] = new PropagateClient(_chanels[i]);
                }
                _master[i] = i;
                _down[i] = false;
            }

            int interval = _numberOfServers / 2;
            int partition_id;
            _partitions.Add(_serverId);
            Console.WriteLine("Partition ID: " + _serverId);

            for (int i = 1, y = 0; i <= interval; i++, y++)
            {
                partition_id = _serverId + i;

                if (partition_id > _numberOfServers)
                {
                    partition_id = partition_id - _numberOfServers;
                }
                Console.WriteLine("Partition ID: " + partition_id);
                _partitions.Add(partition_id);
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



        public async Task ThreadLockAsync (int t, int serverID, LockRequest lockRequest)
        {
            Console.WriteLine("thread: " + t);
            await _clients[serverID].LockServersAsync(lockRequest);
            _handles[t].Set();
        }

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

            for (int i = 1; i < _partitions.Count; i++)
            {
                propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
                server_id = _partitions[i];
                Console.WriteLine("ServerID: " + server_id);
                try
                {
                    _clients[server_id].PropagateServersAdvanced(propagateRequest);
                }
                catch
                {
                    Console.WriteLine("Server: " + server_id + " is down");
                    ChangeMasterRequest(server_id, server_id + 1);
                    for (int x = 1; x <= _numberOfServers; x++)
                    {
                        if (x == _serverId || x == server_id || _down[x])
                        {
                            continue;
                        }
                        ChangeMasterNotificationRequest(server_id, x, server_id + 1);
                    }
                    
                }
                Console.WriteLine("Value Propagated");

            }
        }

        public async void ChangeMasterRequest(int old_server_id, int server_id)
        {
            // Se nao responde e pq foi abaixo
            masterUpdate(old_server_id, server_id);
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

            }
        }

        public void ChangeMasterNotificationRequest (int down_server_id, int server_id, int new_server)
        {
            try
            {
                Console.WriteLine("Notifying server: " + server_id + " about down server " + down_server_id);
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

        public void ChangeMaster(int server_id)
        {
            Init();
            masterUpdate(server_id, _serverId);

            Console.WriteLine("IM NOW MASTER OF PARTITION " + server_id);
        }

        public void ChangeMasterNotification(int old_server, int new_server)
        {
            Init();
            // If already knew then ignore
            if (_down[old_server])
            {
                return;
            }

            masterUpdate(old_server, new_server);

            // 
            var tmp = new List<PropagateClient>(_clients);
            tmp.RemoveAt(old_server);
            _clients = tmp.ToArray();
            Console.WriteLine("IM NOTIFIED THAT SERVER " + old_server + " IS DOWN");
        }

        public bool isMaster(int partition_id)
        {
            Init();
            return _master[partition_id] == _serverId;
        }

        // Set new master for all partitions ruled by old server
        public void masterUpdate (int old_server, int new_server)
        {
            _down[old_server] = true;

            for (int i = 1; i < _master.Length; i++)
            {
                if (_master[i] == old_server)
                {
                    _master[i] = new_server;
                }
            }
        }

    }

}
      
