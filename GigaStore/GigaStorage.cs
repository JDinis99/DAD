using GigaStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using static GigaStore.Propagate;

namespace GigaStore
{
    public class GigaStorage
    {
        private static readonly GigaStorage _instance = new GigaStorage();
        private MultiKeyDictionary<int, int, string> gigaObjects;
        private int _serverId;
        private int _numberOfServers;
        private GrpcChannel[] _chanels;
        private PropagateClient[] _clients;


        private GigaStorage()
        {
            gigaObjects = new MultiKeyDictionary<int, int, string>();
        }

        public static GigaStorage GetGigaStorage()
        {
            return _instance;
        }

        public void Init ()
        // Starts the GRPC clients with the other servers
        {
            // +1 So the server_id and respective index in the array match
            _chanels = new GrpcChannel[_numberOfServers + 1];
            _clients = new PropagateClient[_numberOfServers + 1];
            var url = "";
            for (int i = 1; i <= _numberOfServers; i++)
            {
                if (i != _serverId)
                {
                    url = "https://localhost:500" + i;

                    Console.WriteLine("I: " + i + " URL: " + url);

                    _chanels[i] = GrpcChannel.ForAddress(url);
                    _clients[i] = new PropagateClient(_chanels[i]);
                }
            }
        }

        public async Task WriteAsync(int partition_id, int object_id, string value)
        {
            Init();
            gigaObjects.Add(partition_id, object_id, value);
            var propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
            int interval =_numberOfServers / 2;
            Console.WriteLine("Interval: " + interval);

            for (int i = 1; i <=interval; i++)
            {
                propagateRequest = new PropagateRequest { PartitionId = partition_id, ObjectId = object_id, Value = value };
                var serverID = _serverId + i;
                if (serverID > _numberOfServers)
                {
                    serverID = serverID - _numberOfServers;
                }
                Console.WriteLine("ServerID: " + serverID);
                await _clients[serverID].PropagateServersAsync(propagateRequest);
                Console.WriteLine("Value Propagated");

            }
        }

        public void Store(int partition_id, int object_id, string value)
        {
            gigaObjects.Add(partition_id, object_id, value);
        }

        public string Read(int partition_id, int object_id)
        {
            string value;
            try
            {
                value = gigaObjects[partition_id][object_id];
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
    }

}
      
