﻿using GigaStore.Domain;
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
        private MultiKeyDictionary<int, int, string> _gigaObjects;
        private MultiKeyDictionary<int, int, Semaphore> _semObjects;
        private int _serverId;
        private int _numberOfServers;
        private int _aliveServers;
        private GrpcChannel[] _chanels;
        private PropagateClient[] _clients;
        private AutoResetEvent[] _handles;
        private bool _inited = false;
        private bool _frozen = false;
        private int _minDelay;
        private int _maxDelay;
        private int _replicationFactor;

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

        // Singleton Design Patter "constructor"
        public static GigaStorage GetGigaStorage()
        {
            return _instance;
        }

        public void Init ()
        // Starts the GRPC clients with the other servers and sets up all axiliary lists
        {
            // If it has already been initiated then ignore
            if (_inited)
            {
                return;
            }
            // +1 So the server_id and respective index in the array match
            _chanels = new GrpcChannel[_numberOfServers + 1];
            _clients = new PropagateClient[_numberOfServers + 1];
            // _master: we assume that the number of partitions is the same as the number of servers
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

            Thread t = new Thread(async () => await ThreadPing());
            //t.Start();
            _inited = true;
        }

        /*
         * Base Version
         * 
         */

        // Base version Write
        public void Write(int partition_id, int object_id, string value)
        {
            Init();

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
                int s = _servers[partition_id][i];
                var lockRequestTmp = lockRequest;

                threads[i] = new Thread(async () => await ThreadLockAsync(t, s, lockRequestTmp));
            }

            for (int x = 0; x < threads.Length; x++)
            {
                Console.WriteLine("Starting thread: " + x);
                threads[x].Start();
            }

            // Waits for all locks
            Console.WriteLine("Awating lock handles");
            WaitHandle.WaitAll(_handles);
            Console.WriteLine("Done lock handles");

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
                int s = _servers[partition_id][i];
                var propagateRequestTmp = propagateRequest;

                threads[i] = new Thread(async () => await ThreadPropagateAsync(t, s, propagateRequestTmp));
                Console.WriteLine("Value Propagated");

            }

            for (int x = 0; x < threads.Length; x++)
            {
                Console.WriteLine("Starting thread: " + x);
                threads[x].Start();
            }

            // Waits for all propagations
            Console.WriteLine("Awating propagate handles");
            WaitHandle.WaitAll(_handles);
            Console.WriteLine("Done propagate handles");
        }


        // Method to facilitate Lock Requests to other servers with threads
        public async Task ThreadLockAsync (int t, int serverID, LockRequest lockRequest)
        {
            Console.WriteLine("thread: " + t);
            try
            {
                await _clients[serverID].LockServersAsync(lockRequest);
            }
            catch
            {
                // If fails then the server is down
                ChangeMasterRequest(serverID, serverID + 1);
            }
            _handles[t].Set();
        }

        // Method to facilitate Propagate Requests to other servers with threads
        public async Task ThreadPropagateAsync(int t, int serverID, PropagateRequest propagateRequest)
        {
            Console.WriteLine("thread: " + t);
            try
            {
                await _clients[serverID].PropagateServersAsync(propagateRequest);
            }
            catch
            {
                // If fails then the server is down
                ChangeMasterRequest(serverID, serverID + 1);
            }
            _handles[t].Set();
        }

        // Locks an object
        public void Lock(int partition_id, int object_id)
        {
            Console.WriteLine("LOCKING partition: " + partition_id + " object: " + object_id);

            try
            {
                _semObjects[partition_id][object_id].WaitOne();
            }
            catch (KeyNotFoundException)
            {
                // If there is no semaphore (in case of first write of this object) create one
                _semObjects.Add(partition_id, object_id, new Semaphore(1,1));
                _semObjects[partition_id][object_id].WaitOne();

            }
            Console.WriteLine("LOCKED partition: " + partition_id + " object: " + object_id);
        }

        // Stores an object from propagation and releases the lock
        public void Store(int partition_id, int object_id, string value)
        {
            Init();
            _gigaObjects.Add(partition_id, object_id, value);
            _semObjects[partition_id][object_id].Release();

            Console.WriteLine("UNLOCKED partition: " + partition_id + " object: " + object_id);
        }


        // Base Version Read
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

        public List<Object> ListServer()
        {
            var objects = new List<Object>();
            foreach (int part in _gigaObjects.Keys)
            {
                foreach (var obj in _gigaObjects[part].Keys)
                {
                    Console.WriteLine("LOCKED READ partition: " + part + " object: " + obj);
                    _semObjects[part][obj].WaitOne();
                    string val = _gigaObjects[part][obj];
                    _semObjects[part][obj].Release();
                    Console.WriteLine("UNLOCKED READ partition: " + part + " object: " + obj);
                    var o = new Object(part, obj, val);
                    objects.Add(o);
                }
            }
            return objects;
        }


        /*
         * Advanced Version
         * 
         */


        // Advanced Version Write
        public void WriteAdvanced(int partition_id, int object_id, string value)
        {
            Init();
            PropagateRequest propagateRequest;
            int server_id;

            _gigaObjects.Add(partition_id, object_id, value);

            // Propagate to all servers without waiting for response
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
                    // If fails then the server is down
                    ChangeMasterRequest(server_id, server_id + 1);
                }

            }
            Console.WriteLine("Value Propagated");
        }

        // Advanced Version Read
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

        // Stores value without blocking a semaphore
        public void StoreAdvanced(int partition_id, int object_id, string value)
        {
            Init();
            _gigaObjects.Add(partition_id, object_id, value);
        }

        /*
         * Auxiliary
         * 
         */

        // Notifies new master and all other servers that down_server_id is down and that new_server_id is the new master
        public async void ChangeMasterRequest(int down_server_id, int new_server_id)
        {

            Console.WriteLine("DownServer: " + down_server_id + " NewServer: " + new_server_id);
            
            if (new_server_id > _numberOfServers)
            {
                new_server_id = new_server_id - _numberOfServers;
            }

            Console.WriteLine("Server: " + down_server_id + " is down");
            try
            {
                Console.WriteLine("changing master from: " + down_server_id + " to " + new_server_id);

                ChangeRequest changeRequest = new ChangeRequest { ServerId = down_server_id };
                await _clients[new_server_id].ChangeMasterAsync(changeRequest);


            }
            catch
            {
                // Keep trying until it works with another server
                ChangeMasterRequest(down_server_id, new_server_id + 1);
                // If it failed, the new server is also down
                ChangeMasterRequest(new_server_id, new_server_id + 1);
            }

            // If is the first time current server is notified about down server
            if (!_down[down_server_id])
            {
                await masterUpdateAsync(down_server_id, new_server_id);
                // Notify all other servers that old_server_id is down
                for (int x = 1; x <= _numberOfServers; x++)
                {
                    // Ignore current server, new master and down servers
                    if (x == _serverId || x == new_server_id || _down[x])
                    {
                        continue;
                    }
                    ChangeMasterNotificationRequest(down_server_id, x, new_server_id);
                }
            }
        }

        public async void ChangeMasterNoNoto(int down_server_id, int new_server_id)
        {
            if (new_server_id > _numberOfServers)
            {
                new_server_id = new_server_id - _numberOfServers;
            }

            Console.WriteLine("Server: " + down_server_id + " is down");
            try
            {
                Console.WriteLine("changing master from: " + down_server_id + " to " + new_server_id);

                ChangeRequest changeRequest = new ChangeRequest { ServerId = down_server_id };
                await _clients[new_server_id].ChangeMasterAsync(changeRequest);


            }
            catch
            {
                // Keep trying until it works with another server
                ChangeMasterNoNoto(down_server_id, new_server_id + 1);
                // If it failed, the new server is also down
                ChangeMasterNoNoto(new_server_id, new_server_id + 1);
            }

            if (!_down[down_server_id])
            {
                await masterUpdateAsync(down_server_id, new_server_id);
            }
        }

            public async void ChangeMasterNoNotification(int down_server_id, int new_server_id)
        {
            if (new_server_id > _numberOfServers)
            {
                new_server_id = new_server_id - _numberOfServers;
            }

            Console.WriteLine("Server: " + down_server_id + " is down");
            try
            {
                Console.WriteLine("changing master from: " + down_server_id + " to " + new_server_id);

                ChangeRequest changeRequest = new ChangeRequest { ServerId = down_server_id };
                await _clients[new_server_id].ChangeMasterAsync(changeRequest);


            }
            catch
            {
                // Keep trying until it works with another server
                ChangeMasterRequest(down_server_id, new_server_id + 1);
                // If it failed, the new server is also down
                ChangeMasterRequest(new_server_id, new_server_id + 1);
            }

            // If is the first time current server is notified about down server
            if (!_down[down_server_id])
            {
                await masterUpdateAsync(down_server_id, new_server_id);
                // Notify all other servers that old_server_id is down
                for (int x = 1; x <= _numberOfServers; x++)
                {
                    // Ignore current server, new master and down servers
                    if (x == _serverId || x == new_server_id || _down[x])
                    {
                        continue;
                    }
                    ChangeMasterNotificationRequest(down_server_id, x, new_server_id);
                }
            }
        }

        // Notifies server_id that down_server_id is down and that the new master of the partions of down_server_id is new_server
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
                // If fails then the server is down
                ChangeMasterRequest(server_id, server_id + 1);

            }
        }
    
        // Changes Master of all partitions of server_id with current server
        public async Task ChangeMasterAsync(int server_id)
        {
            Init();
            await masterUpdateAsync(server_id, _serverId);

            Console.WriteLine("IM NOW MASTER OF PARTITION " + server_id);
        }

        // Chasges Master of all partitions of old_server with new_server
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

        // Responds is current server is master of partition_id
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
                    // Ignore
                }
            }


            // If not enough servers share the master partitions request more servers to share it
            for (int i = 1; i < _master.Length; i++)
            {
                if (_master[i] == _serverId)
                {

                    Console.WriteLine("MASTER OF PARTITION: " + i);
                    // If not enough servers propagate
                    if (_servers[i].Count < _replicationFactor)
                    {
                        for (int x = i + 1; x != i;)
                        {
                            if (x > _numberOfServers)
                            {
                                x -= _numberOfServers;
                            }

                            Console.WriteLine("Partition: " + i + " Server ID: " + x);
                            if (_down[x] || _servers[i].Contains(x) || x == _serverId)
                            {
                                x++;
                                continue;
                            }
                            try
                            {
                                Console.WriteLine("Asking server: " + x + " to replicate partition: " + i);
                                var replicateRequest = _clients[x].ReplicatePartition();
                                foreach (KeyValuePair<int, string> entry in _gigaObjects[i])
                                {
                                    await replicateRequest.RequestStream.WriteAsync(new ReplicateRequest
                                    {
                                        PartitionId = i,
                                        ObjectId = entry.Key,
                                        Value = entry.Value
                                    });

                                }
                                await replicateRequest.RequestStream.CompleteAsync();
                                x++;
                                break;
                            }
                            catch
                            {
                                x++;
                                // skip this one
                            }

                        }

                    }
                
                }

            }

        }

        // Gets Partition
        public Dictionary<int, string> getPartition (int parttion_id)
        {
            return _gigaObjects[parttion_id];
        } 

        // TODO Test this
        // Checks status of server_id in case a client suspects is down
        public async Task CheckStatusAsync (int server_id)
        {
            try
            {
                if (!_down[server_id])
                {
                    CheckServersRequest checkServersRequest = new CheckServersRequest { };
                    await _clients[server_id].CheckStatusServersAsync(checkServersRequest);
                }
            }
            catch
            {
                // If server is down
                ChangeMasterRequest(server_id, server_id + 1);
            }
        }


        // Ping all other servers at 5sec intervals
        public Task ThreadPing()
        {
            PingRequest pingRequest = new PingRequest { };
            while (true)
            {
                for (int i = 1; i <= _numberOfServers; i++)
                {
                    if (_down[i] || i == _serverId)
                    {
                        continue;
                    }
                    try
                    {
                        Console.WriteLine("Pinging server: " + i);
                        _clients[i].Ping(pingRequest);
                    }
                    catch
                    {
                        // If fails then the server is down
                        Console.WriteLine("Server is down: " + i);
                        ChangeMasterNoNoto(i, i + 1);
                    }
                }
                Console.WriteLine("SLEEPING");
                Thread.Sleep(5000);
            }
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

        public int getMaster(int partition_id)
        {
            return _master[partition_id];
        }


        public void ChangeReplicationFactor(int factor)
        {
            _replicationFactor = factor;
        }

        public void SetMinDelay(int delay)
        {
            _minDelay = delay;
        }

        public void SetMaxDelay(int delay)
        {
            _maxDelay = delay;
        }

        public void Delay()
        {
            Random r = new Random();
            int rInt = r.Next(_minDelay, _maxDelay);
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
        public int PartitionId { get; }
        public int ObjectId { get; }
        public string Value { get; }

        public Object(int partitionId, int objectId, string value)
        {
            PartitionId = partitionId;
            ObjectId = objectId;
            Value = value;
        }
    }

}
      
