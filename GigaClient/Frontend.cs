using GigaStore;
using Grpc.Net.Client;
using System;
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
        private int _nservers;

        public int ServerId { get; private set; } = -1;
        private GrpcChannel _channel = null;
        private Giga.GigaClient _client;

        public Frontend(int serverId, int nservers)
        {
            _nservers = nservers;
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
            var reply = await _client.ReadAsync(request);

            if (String.Equals(reply.Value, "N/A") && request.ServerId != -1)
            {
                EstablishChannel(request.ServerId);
                reply = await _client.ReadAsync(request);
            }

            return reply;
        }

        public async Task<WriteReply> WriteAsync(WriteRequest request)
        {
            var reply = await _client.WriteAsync(request);

            if (reply.MasterId != -1)
            {
                Console.WriteLine($"Establish a channel with the master server (id: {reply.MasterId}) of partition {request.PartitionId}.");
                EstablishChannel(reply.MasterId);
                reply = await _client.WriteAsync(request);
            }

            return reply;
        }

        public async Task<ListServerReply> ListServerAsync(ListServerRequest request, int serverId)
        {
            /*
            ListServerReply reply;
            try
            {
                EstablishChannel(serverId);
                reply = await _client.ListServerAsync(request);
            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkRequest = new CheckRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkRequest);

                reply = new ListServerReply(); // empty reply
            }
            */

            EstablishChannel(serverId);
            var reply = await _client.ListServerAsync(request);

            return reply;
        }

        public async Task<ListServerReply[]> ListGlobalAsync(ListServerRequest request)
        {
            ListServerReply[] reply = new ListServerReply[_nservers + 1]; // skip index 0
            for (int id = 1; id <= _nservers; id++)
            {
                try
                {
                    EstablishChannel(id);
                    reply[id] = await _client.ListServerAsync(request);
                }
                catch (Grpc.Core.RpcException e)
                {
                    Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                    var checkRequest = new CheckRequest
                    {
                        ServerId = this.ServerId
                    };
                    await this.CheckStatusAsync(checkRequest);

                    reply[id] = new ListServerReply(); // empty reply
                }
            }

            return reply;
        }

        public async Task<CheckReply> CheckStatusAsync(CheckRequest request)
        {
            var reply = new CheckReply();
            try
            {
                // connect to next server_id and check the status of the previous one
                var newServerId = this.ServerId - 1;
                if (newServerId == 0) newServerId = _nservers;
                EstablishChannel(newServerId);
                reply = await _client.CheckStatusAsync(request);

            }
            catch (Grpc.Core.RpcException e)
            {
                Console.WriteLine($"Grpc.Core.RpcException: {e.StatusCode}");
                var checkRequest = new CheckRequest
                {
                    ServerId = this.ServerId
                };
                await this.CheckStatusAsync(checkRequest);

            }

            return reply;
        }

    }
}
