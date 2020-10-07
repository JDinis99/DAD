using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("Client contacted server " + request.ServerId + " and reached " + _gigaStorage.GetServerId());

            string value = _gigaStorage.Read(request.PartitionId, request.ObjectId);
            return Task.FromResult(new ReadReply
            {
                Value = value
            });
        }

        public override async Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            // TODO verificar o server
            await _gigaStorage.WriteAsync(request.PartitionId, request.ObjectId, request.Value);
            return new WriteReply
            {
                // Empty message as ack
            };

        }
    }
}
