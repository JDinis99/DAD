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
        // TODO definir instancias
        private int instance;
        private bool _virginService = true;

        public GigaService(ILogger<GigaService> logger)
        {
            _logger = logger;
            _gigaStorage = new GigaStorage();
        }

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("VIRGIN SERVICE: " + _virginService);
            // TODO verificar instancia
            string value = _gigaStorage.read(request.PartitionId, request.ObjectId);
            return Task.FromResult(new ReadReply
            {
                Value = value
            });
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            _virginService = false;
            Console.WriteLine("VIRGIN SERVICE: " + _virginService);
            // TODO verificar instancia
            _gigaStorage.write(request.PartitionId, request.ObjectId, request.Value);
            return Task.FromResult(new WriteReply
            {
                // Empty message as ack
            });

        }
    }
}
