using GigaStore;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GigaClient
{
    /**
     * Encapsulates gRPC channel and client for remote service. 
     * All remote calls from the client should use this object.
     */
    class Frontend
    {
        private readonly Dictionary<string, string> _servers;
        private readonly int _serversCount;
        private readonly bool _isAdvanced;

        public string ServerId { get; private set; } = null;
        private GrpcChannel _channel = null;
        private Giga.GigaClient _client;

        public Frontend(int serversCount, bool isAdvanced, Dictionary<string, string> servers)
        {
            // TODO validate input arguments
            _serversCount = serversCount;
            _isAdvanced = isAdvanced;
            _servers = servers;

            var random = new Random();
            var index = random.Next(_serversCount);
            var serverId = _servers.Keys.ElementAt(index);
            EstablishChannel(serverId);
        }

        public void EstablishChannel(string serverId)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            if (serverId != this.ServerId)
            {
                try {
                    Console.Write($"Establishing channel with server {serverId}... ");
                    if (_channel != null) _channel.ShutdownAsync().Wait();
                    _channel = GrpcChannel.ForAddress(_servers[serverId]);
                    _client = new Giga.GigaClient(_channel);
                    this.ServerId = serverId;
                    Console.WriteLine("Established.");
                }
                catch (Exception e) {
                    Console.WriteLine("Failed.");
                    Console.ResetColor();
                    throw e; // rethrow exception
                }
            }

            Console.ResetColor();
        }


        /* ====================================================================== */
        /* ====[                        Remote Calls                        ]==== */
        /* ====================================================================== */

        public async Task<ReadReply> ReadAsync(ReadRequest request)
        {
            var reply = new ReadReply(); // empty reply;
            try
            {
                reply = await ClientReadAsync(request);

                if (String.Equals(reply.Value, "N/A") && request.ServerId != "-1")
                {
                    EstablishChannel(request.ServerId);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = "-1"
                    };
                    reply = await ReadAsync(readRequest); // recursion
                }
                else if (String.Equals(reply.Value, "N/A") && request.ServerId == "-1" && reply.MasterId != this.ServerId && reply.MasterId != "") 
                {
                    // if reply.Value == "N/A" then ask master, because the master of a partition always contains that partition
                    // if the partition master doesnt contain the request object, its because it doesnt exist
                    Console.WriteLine($"Establish a channel with the master server (id: {reply.MasterId}) of partition {request.PartitionId}.");
                    EstablishChannel(reply.MasterId);

                    var readRequest = new ReadRequest {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = "-1"
                    };
                    reply = await ReadAsync(readRequest); // recursion
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                await CheckCurrentServerStatus();

                if (request.ServerId != "-1")
                {
                    EstablishChannel(request.ServerId);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = "-1"
                    };
                    reply = await ReadAsync(readRequest); // recursion
                }
                else if (request.ServerId == "-1")
                {
                    var getMasterRequest = new GetMasterRequest { PartitionId = request.PartitionId };
                    var getMasterReply = await GetMasterAsync(getMasterRequest);
                    var masterId = getMasterReply.MasterId;
                    // if the current server is not the master
                    if (masterId != this.ServerId && masterId != "") {
                        Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {request.PartitionId}.");
                        EstablishChannel(masterId);
                    }
                    reply = await ReadAsync(request); // recursion
                }
            }

            return reply;
        }

        public async Task<WriteReply> WriteAsync(WriteRequest request)
        {
            WriteReply reply;
            var partitionId = request.PartitionId;
            try
            {
                reply = await ClientWriteAsync(request);

                var masterId = reply.MasterId;
                // if the current server is not the master
                if (masterId != this.ServerId && masterId != "")
                {
                    Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {partitionId}.");
                    EstablishChannel(masterId);
                    reply = await WriteAsync(request); // recursion
                }

            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                await CheckCurrentServerStatus();

                var getMasterRequest = new GetMasterRequest { PartitionId = partitionId };
                var getMasterReply = await GetMasterAsync(getMasterRequest);
                var masterId = getMasterReply.MasterId;
                // if the current server is not the master
                if (masterId != this.ServerId && masterId != "") 
                {
                    Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {partitionId}.");
                    EstablishChannel(masterId);
                }
                reply = await WriteAsync(request); // recursion

            }

            return reply;
        }

        public async Task<ListServerReply> ListServerAsync(ListServerRequest request, string serverId)
        {
            var previousId = this.ServerId;

            ListServerReply reply;
            try
            {
                EstablishChannel(serverId);
                reply = await ClientListServerAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                await CheckCurrentServerStatus();

                reply = new ListServerReply(); // empty reply;
            }

            Console.WriteLine($"Establish a channel with the server that preceded the ListServer operation (id: {previousId}).");
            EstablishChannel(previousId);
            return reply;
        }

        public async Task<Dictionary<string, ListServerReply>> ListGlobalAsync(ListServerRequest request)
        {
            var previousId = this.ServerId;

            var reply = new Dictionary<string, ListServerReply>(_serversCount);
            foreach (var id in _servers.Keys)
            {
                try
                {
                    EstablishChannel(id);
                    reply[id] = await ClientListServerAsync(request);
                }
                catch (RpcException e)
                {
                    Console.WriteLine($"RpcException: {e.StatusCode}");
                    await CheckCurrentServerStatus();

                    reply[id] = new ListServerReply(); // empty reply
                }
            }

            Console.WriteLine($"Establish a channel with the server that preceded the ListGlobal operation (id: {previousId}).");
            EstablishChannel(previousId);
            return reply;
        }


        /* ====================================================================== */
        /* ====[                   Auxiliary Remote Calls                   ]==== */
        /* ====================================================================== */

        public async Task<CheckStatusReply> CheckStatusAsync(CheckStatusRequest request)
        {
            CheckStatusReply reply = new CheckStatusReply(); // empty reply
            try
            {
                Console.WriteLine($"Checking status of server {this.ServerId}.");
                // Connect to random serverId (hopefully one that isnt down) and check the status of the suspicious ones
                var random = new Random();
                var index = random.Next(_serversCount);
                var newServerId = _servers.Keys.ElementAt(index);
                Console.WriteLine($"Establish a channel with a random server (id: {newServerId}).");
                EstablishChannel(newServerId);
                await _client.CheckStatusAsync(request);

            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                // Exception -> the newServer is also suspicious
                request.ServerId.Add(this.ServerId);
                await CheckStatusAsync(request); // recursion

            }

            return reply;
        }

        public async Task<GetMasterReply> GetMasterAsync(GetMasterRequest request)
        {
            GetMasterReply reply;
            try
            {
                reply = await _client.GetMasterAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                await CheckCurrentServerStatus();
                reply = await GetMasterAsync(request); // recursion
            }

            if (reply.MasterId == "")
                Console.WriteLine($"The partition {request.PartitionId} does not have a master.");

            return reply;
        }

        private AsyncUnaryCall<ReadReply> ClientReadAsync(ReadRequest request) {
            if (_isAdvanced)
                return _client.ReadAdvancedAsync(request);
            return _client.ReadAsync(request);
        }

        private AsyncUnaryCall<WriteReply> ClientWriteAsync(WriteRequest request) {
            if (_isAdvanced)
                return _client.WriteAdvancedAsync(request);
            return _client.WriteAsync(request);
        }

        private AsyncUnaryCall<ListServerReply> ClientListServerAsync(ListServerRequest request) {
            if (_isAdvanced)
                return _client.ListServerAdvancedAsync(request);
            return _client.ListServerAsync(request);
        }

        private async Task<CheckStatusReply> CheckCurrentServerStatus() {
            var checkStatusRequest = new CheckStatusRequest();
            checkStatusRequest.ServerId.Add(this.ServerId);
            return await CheckStatusAsync(checkStatusRequest);
        }


        /* ====[ NOT USED ]====
        
        public void DEBUG(string message) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DEBUG: " + message);
            Console.ResetColor();
        }

        */

    } // class

} // namespace
