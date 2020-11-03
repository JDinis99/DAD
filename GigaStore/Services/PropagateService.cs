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
            Console.WriteLine("Propagated to Server: " + _gigaStorage.ServerId);

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

        public override Task<PropagateReply> PropagateServersAdvanced(PropagateRequest request, ServerCallContext context)
        {
            Console.WriteLine("Propagated Advanced to Server: " + _gigaStorage.ServerId);

            _gigaStorage.StoreAdvanced(request.PartitionId, request.ObjectId, request.Value);
            return Task.FromResult(new PropagateReply
            {
                // Empty message as ack
            });

        }

        public override async Task<ChangeReply> ChangeMaster(ChangeRequest request, ServerCallContext context)
        {
            Console.WriteLine("Change Master from server: " + request.ServerId + " for partition " + request.PartitionId);
            await _gigaStorage.ChangeMasterAsync(request.ServerId, request.PartitionId);
            return await Task.FromResult(new ChangeReply
            {
                // Empty message as ack
            });

        }

        public override async Task<ChangeReply> ChangeMasterNotification(ChangeNotificationRequest request, ServerCallContext context)
        {
            Console.WriteLine("Change Master Notification from server: " + request.ServerId + " to: " + request.NewServerId + " for partiton: " + request.PartitionId);
            await _gigaStorage.ChangeMasterNotificationAsync(request.ServerId, request.NewServerId, request.PartitionId);
            return await Task.FromResult(new ChangeReply
            {
                // Empty message as ack
            });

        }

        public override async Task<ReplicateReply> ReplicatePartition(IAsyncStreamReader<ReplicateRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine("Being Asked to replicate");
            while (await requestStream.MoveNext())
            {
                var replicateRequest = requestStream.Current;
                Console.WriteLine("Stroring Partition: " + replicateRequest.PartitionId + " Obejct " + replicateRequest.ObjectId + " Value " + replicateRequest.Value);
                _gigaStorage.StoreAdvanced(replicateRequest.PartitionId, replicateRequest.ObjectId, replicateRequest.Value);
            }
            return await Task.FromResult(new ReplicateReply
            {
                // Empty message as ack
            });
        }

        public override async Task<PingReply> Ping(PingRequest request, ServerCallContext context)
        {
            Console.WriteLine("Pinged");
            return await Task.FromResult(new PingReply
            {
                // Empty message as ack
            });

        }
    }
}


