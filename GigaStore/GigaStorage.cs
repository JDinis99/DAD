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
        private bool[] _master;


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
        // Starts the GRPC clients with the other servers
        {
            if (_inited)
            {
                return;
            }
            // +1 So the server_id and respective index in the array match
            _chanels = new GrpcChannel[_numberOfServers + 1];
            _clients = new PropagateClient[_numberOfServers + 1];
            _master = new bool[_numberOfServers + 1];
            var url = "";
            for (int i = 1; i <= _numberOfServers; i++)
            {
                if (i != _serverId)
                {
                    url = "https://localhost:500" + i;

                    Console.WriteLine("I: " + i + " URL: " + url);

                    _chanels[i] = GrpcChannel.ForAddress(url);
                    _clients[i] = new PropagateClient(_chanels[i]);
                    _master[i] = false;
                }
                else
                {
                    _master[i] = true;
                }
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
            int serverID;
            _handles = new AutoResetEvent[interval];
            Thread[] threads = new Thread[interval];

            for (int i = 1, y = 0; i <= interval; i++, y++)
            {
                serverID = _serverId + i;

                if (serverID > _numberOfServers)
                {
                    serverID = serverID - _numberOfServers;
                }

                _handles[y] = new AutoResetEvent(false);
                int t = y;
                int s = serverID;
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
                serverID = _serverId + i;
                if (serverID > _numberOfServers)
                {
                    serverID = serverID - _numberOfServers;
                }
                Console.WriteLine("ServerID: " + serverID);
                _handles[y] = new AutoResetEvent(false);
                int t = y;
                int s = serverID;
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
            int interval = _numberOfServers / 2;
            int serverID;

            _gigaObjects.Add(partition_id, object_id, value);

            for (int i = 1; i <= interval; i++)
            {
                propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
                serverID = _serverId + i;
                if (serverID > _numberOfServers)
                {
                    serverID = serverID - _numberOfServers;
                }
                Console.WriteLine("ServerID: " + serverID);
                try
                {
                _clients[serverID].PropagateServersAdvanced(propagateRequest);
                }
                catch
                {
                    Console.WriteLine("Deu merda");
                }
                Console.WriteLine("Value Propagated");

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
            _master[server_id] = true;
            var tmp = new List<PropagateClient>(_clients);
            tmp.RemoveAt(server_id);
            _clients= tmp.ToArray();
            _numberOfServers--;
        }

        public void ChangeMasterNotification(int old_server, int new_server)
        {
            Init();
            var tmp = new List<PropagateClient>(_clients);
            tmp.RemoveAt(old_server);
            _clients = tmp.ToArray();
            _numberOfServers--;
        }

    }

}
      
