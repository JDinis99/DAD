using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GigaStore.Services
{
    public class GigaService : Giga.GigaBase
    {

        private readonly ILogger<GigaService> _logger;
        private GigaStorage _gigaStorage;

        public GigaService(ILogger<GigaService> logger)
        {
            _logger = logger;
            _gigaStorage = GigaStorage.GetGigaStorage();
        }

        /*
         * Base Version
         * 
         */

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            string value = _gigaStorage.Read(request.PartitionId, request.ObjectId);
            return Task.FromResult(new ReadReply
            {
                Value = value
            });
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            var partitionId = request.PartitionId;
            if (!_gigaStorage.isMaster(partitionId))
            {
                Console.WriteLine("This is not the master server for this partition.");
                return Task.FromResult(new WriteReply
                {
                    MasterId = _gigaStorage.getMaster(partitionId)
                });
            }

            _gigaStorage.Write(request.PartitionId, request.ObjectId, request.Value);

            return Task.FromResult(new WriteReply
            {
                // The current server is already the master for this partition
                MasterId = -1
            });
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            var reply = new ListServerReply();

            var objects = _gigaStorage.ListServer();
            foreach (var obj in objects)
            {
                ListServerReply.Types.Object o = new ListServerReply.Types.Object
                {
                    PartitionId = obj.PartitionId,
                    ObjectId = obj.ObjectId,
                    Value = obj.Value,
                    InMaster = _gigaStorage.isMaster(obj.PartitionId)
                };
                reply.Objects.Add(o);
            }
            return Task.FromResult(reply);
        }

        /*
         * Advanced Version
         * 
         */

        public override Task<ReadReply> ReadAdvanced(ReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("Advanced read");

            string value = _gigaStorage.ReadAdvanced(request.PartitionId, request.ObjectId);
            return Task.FromResult(new ReadReply
            {
                Value = value
            });
        }

        public override Task<WriteReply> WriteAdvanced(WriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Advanced write");
            if (!_gigaStorage.isMaster(request.PartitionId))
            {
                // TODO lancar uma execao
                Console.WriteLine("PARTICAO ERRADA");
            }
            // TODO verificar o server
            _gigaStorage.WriteAdvanced(request.PartitionId, request.ObjectId, request.Value);
            return Task.FromResult(new WriteReply
            {
                // Empty message as ack
            });

        }

        public override async Task<CheckReply> CheckStatus(CheckRequest request, ServerCallContext context)
        {
            Console.WriteLine("Checking status of server " + request.ServerId);
            await _gigaStorage.CheckStatusAsync(request.ServerId);
            return await Task.FromResult(new CheckReply
            {
                // Empty message as ack
            });
        }

    }
}
