using GigaStore;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
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

        public Frontend(string serverId, int serversCount, bool isAdvanced)
        {
            // FIXME !!!!!!! remove hardcode
            _servers = new Dictionary<string, string>(serversCount);
            for (int i = 1; i <= serversCount; i++)
            {
                string id = i.ToString();
                _servers[id] = $"https://localhost:500{id}";
            }

            _serversCount = serversCount;
            EstablishChannel(serverId);
            _isAdvanced = isAdvanced;
        }


        private void EstablishChannel(string serverId)
        {
            if (serverId != this.ServerId)
            {
                Console.Write($"Establishing channel with server {serverId}... ");
                if (_channel != null) _channel.ShutdownAsync().Wait();
                _channel = GrpcChannel.ForAddress(_servers[serverId]);
                _client = new Giga.GigaClient(_channel);
                this.ServerId = serverId;
                Console.WriteLine("Established.");
            }
        }

        private AsyncUnaryCall<ReadReply> ClientReadAsync(ReadRequest request)
        {
            if (_isAdvanced)
                return _client.ReadAdvancedAsync(request);
            return _client.ReadAsync(request);
        }

        private AsyncUnaryCall<WriteReply> ClientWriteAsync(WriteRequest request)
        {
            if (_isAdvanced)
                return _client.WriteAdvancedAsync(request);
            return _client.WriteAsync(request);
        }

        private AsyncUnaryCall<ListServerReply> ClientListServerAsync(ListServerRequest request)
        {
            if (_isAdvanced)
                return _client.ListServerAdvancedAsync(request);
            return _client.ListServerAsync(request);
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

                if (String.Equals(reply.Value, "N/A") && request.ServerId != null)
                {
                    EstablishChannel(request.ServerId);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = null
                    };
                    reply = await ReadAsync(readRequest); // recursion
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await CheckStatusAsync(checkStatusRequest);

                if (request.ServerId != null)
                {
                    EstablishChannel(request.ServerId);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = null
                    };
                    reply = await ReadAsync(readRequest); // recursion
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
                if (masterId != null)
                {
                    Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {partitionId}.");
                    EstablishChannel(masterId);
                    reply = await WriteAsync(request); // recursion
                }

            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await CheckStatusAsync(checkStatusRequest);

                var getMasterReply = await GetMasterAsync(new GetMasterRequest
                {
                    PartitionId = partitionId
                });
                var masterId = getMasterReply.MasterId;
                Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {partitionId}.");
                EstablishChannel(masterId);
                reply = await WriteAsync(request); // recursion

            }

            return reply;
        }

        public async Task<ListServerReply> ListServerAsync(ListServerRequest request, string serverId)
        {
            ListServerReply reply;
            try
            {
                EstablishChannel(serverId);
                reply = await ClientListServerAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await CheckStatusAsync(checkStatusRequest);

                reply = new ListServerReply(); // empty reply;
            }

            return reply;
        }

        public async Task<Dictionary<string, ListServerReply>> ListGlobalAsync(ListServerRequest request)
        {
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
                    var checkStatusRequest = new CheckStatusRequest
                    {
                        ServerId = this.ServerId
                    };
                    await CheckStatusAsync(checkStatusRequest);

                    reply[id] = new ListServerReply(); // empty reply
                }
            }

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
                // connect to next server_id and check the status of the previous one
                //
                // FIXME !!!!!!! remove Parse and ToString, handle it properly
                //
                var newServerId = Int32.Parse(this.ServerId) - 1;
                if (newServerId == 0) newServerId = _serversCount;
                EstablishChannel(newServerId.ToString());
                await _client.CheckStatusAsync(request);

            }
            catch (RpcException e)
            {
                Console.WriteLine($"RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await CheckStatusAsync(checkStatusRequest); // recursion

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
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await CheckStatusAsync(checkStatusRequest);

                reply = await GetMasterAsync(request); // recursion
            }

            return reply;
        }

    } // class

} // namespace
