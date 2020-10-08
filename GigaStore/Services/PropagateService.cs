using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GigaStore.Services
{
    public class PropagateService : Propagate.PropagateBase
    {

        private readonly ILogger<PropagateService> _logger;
        private GigaStorage _gigaStorage;

        public PropagateService(ILogger<PropagateService> logger)
        {
            _logger = logger;
            _gigaStorage = GigaStorage.GetGigaStorage();
        }
        
        public override  Task<PropagateReply> PropagateServers(PropagateRequest request, ServerCallContext context)
        {
            Console.WriteLine("Propagated to Server: " + _gigaStorage.GetServerId());

            _gigaStorage.Store(request.PartitionId, request.ObjectId, request.Value);
            return Task.FromResult(new PropagateReply
            {
                // Empty message as ack
            });

        }
        
        public override Task<LockReply> LockServers(LockRequest request, ServerCallContext context)
        {
            _gigaStorage.Lock(request.PartitionId, request.ObjectId);
            return Task.FromResult(new LockReply
            {
                // Empty message as ack
            });
        }
        
    }
}


