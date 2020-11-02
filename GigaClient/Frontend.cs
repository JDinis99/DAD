using GigaStore;
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
        // we assume sequential server ids i.e. from 1 to _nservers (inclusive)
        private int _serversCount;

        public int ServerId { get; private set; } = -1;
        private GrpcChannel _channel = null;
        private Giga.GigaClient _client;

        public Frontend(int serverId, int serversCount)
        {
            _serversCount = serversCount;
            // TODO check if positive serverId (or in between 1 and _nservers)
            EstablishChannel(serverId);
        }


        private void EstablishChannel(int serverId)
        {
            if (serverId != ServerId)
            {
                Console.Write($"Establishing channel with server {serverId}... ");
                if (_channel != null) _channel.ShutdownAsync().Wait();
                // TODO(?) remove hard-coded, use json file or something
                _channel = GrpcChannel.ForAddress("https://localhost:500" + serverId);
                _client = new Giga.GigaClient(_channel);
                ServerId = serverId;
                Console.WriteLine("Established.");
            }
        }

        public async Task<ReadReply> ReadAsync(ReadRequest request)
        {
            var reply = new ReadReply(); // empty reply;
            try
            {
                reply = await _client.ReadAsync(request);

                if (String.Equals(reply.Value, "N/A") && request.ServerId != -1)
                {
                    EstablishChannel(request.ServerId);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = -1
                    };
                    reply = await this.ReadAsync(readRequest);
                }
            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkStatusRequest);

                if (request.ServerId != -1)
                {
                    EstablishChannel(request.ServerId);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = request.PartitionId,
                        ObjectId = request.ObjectId,
                        ServerId = -1
                    };
                    reply = await this.ReadAsync(readRequest);
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
                reply = await _client.WriteAsync(request);

                var masterId = reply.MasterId;
                if (masterId != -1)
                {
                    Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {partitionId}.");
                    EstablishChannel(masterId);
                    reply = await this.WriteAsync(request);
                }

            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkStatusRequest);

                var getMasterReply = await this.GetMasterAsync(new GetMasterRequest
                {
                    PartitionId = partitionId
                });
                var masterId = getMasterReply.MasterId;
                Console.WriteLine($"Establish a channel with the master server (id: {masterId}) of partition {partitionId}.");
                EstablishChannel(masterId);
                reply = await this.WriteAsync(request);

            }

            return reply;
        }

        public async Task<ListServerReply> ListServerAsync(ListServerRequest request, int serverId)
        {
            ListServerReply reply;
            try
            {
                EstablishChannel(serverId);
                reply = await _client.ListServerAsync(request);
            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkStatusRequest);

                reply = new ListServerReply(); // empty reply;
            }

            return reply;
        }

        public async Task<ListServerReply[]> ListGlobalAsync(ListServerRequest request)
        {
            ListServerReply[] reply = new ListServerReply[_serversCount + 1]; // skip index 0
            for (int id = 1; id <= _serversCount; id++)
            {
                try
                {
                    EstablishChannel(id);
                    reply[id] = await _client.ListServerAsync(request);
                }
                catch (Grpc.Core.RpcException e)
                {
                    Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                    var checkStatusRequest = new CheckStatusRequest
                    {
                        ServerId = this.ServerId
                    };
                    await this.CheckStatusAsync(checkStatusRequest);

                    reply[id] = new ListServerReply(); // empty reply
                }
            }

            return reply;
        }

        public async Task<CheckStatusReply> CheckStatusAsync(CheckStatusRequest request)
        {
            CheckStatusReply reply = new CheckStatusReply(); // empty reply
            try
            {
                // FIXME check server_id > server_count
                // connect to next server_id and check the status of the previous one
                var newServerId = this.ServerId - 1;
                if (newServerId == 0) newServerId = _serversCount;
                EstablishChannel(newServerId);
                await _client.CheckStatusAsync(request);

            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkStatusRequest);

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
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkStatusRequest = new CheckStatusRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkStatusRequest);

                reply = await this.GetMasterAsync(request);
            }

            return reply;
        }

    } // class

} // namespace
