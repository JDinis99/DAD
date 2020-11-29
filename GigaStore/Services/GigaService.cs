using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
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


        /* ====================================================================== */
        /* ====[                        Base Version                        ]==== */
        /* ====================================================================== */

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            WaitUnfreeze();
            var partitionId = request.PartitionId;
            var value = _gigaStorage.Read(partitionId, request.ObjectId);

            var masterId = _gigaStorage.GetMaster(partitionId);
            var currentServerId = _gigaStorage.ServerId;

            // Debug
            if (currentServerId != masterId && value != "N/A")
                Console.WriteLine($"This server (id: {currentServerId}) is not the master server for partition {partitionId}, but it replicates that partition (object value: {value}).");
            else if (currentServerId != masterId && value == "N/A")
                Console.WriteLine($"This server (id: {currentServerId}) is not the master server for partition {partitionId} and it does not replicate that partition.");
            else if (currentServerId == masterId)
                Console.WriteLine($"This server (id: {currentServerId}) is the master server for partition {partitionId} (object value: {value}).");
            else
                // This should never happen
                Console.WriteLine("[READ] WTF Just Happened?!"); 


            return Task.FromResult(new ReadReply
            {
                Value = value,
                MasterId = masterId
            });
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            WaitUnfreeze();
            var partitionId = request.PartitionId;

            var masterId = _gigaStorage.GetMaster(partitionId);
            var currentServerId = _gigaStorage.ServerId;
            if (currentServerId != masterId)
            {
                Console.WriteLine($"This server (id: {currentServerId}) is not the master server for partition {partitionId}.");
                return Task.FromResult(new WriteReply
                {
                    MasterId = masterId
                });
            }

            _gigaStorage.Write(request.PartitionId, request.ObjectId, request.Value);

            // The current server is already the master for this partition
            return Task.FromResult(new WriteReply
            {
                MasterId = currentServerId
            });
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            var currentServerId = _gigaStorage.ServerId;

            var reply = new ListServerReply();
            var objects = _gigaStorage.ListServer();
            foreach (var o in objects)
            {
                var masterId = _gigaStorage.GetMaster(o.PartitionId);
                var inMaster = (currentServerId == masterId);

                var obj = new ListServerReply.Types.Object
                {
                    PartitionId = o.PartitionId,
                    ObjectId = o.ObjectId,
                    Value = o.Value,
                    InMaster = inMaster
                };
                reply.Objects.Add(obj);
            }
            return Task.FromResult(reply);
        }


        /* ====================================================================== */
        /* ====[                      Advanced Version                      ]==== */
        /* ====================================================================== */

        public override Task<ReadReply> ReadAdvanced(ReadRequest request, ServerCallContext context)
        {
            WaitUnfreeze();
            var partitionId = request.PartitionId;
            var value = _gigaStorage.ReadAdvanced(partitionId, request.ObjectId);

            var masterId = _gigaStorage.GetMaster(partitionId);
            var currentServerId = _gigaStorage.ServerId;

            // Debug
            if (currentServerId != masterId && value != "N/A")
                Console.WriteLine($"This server (id: {currentServerId}) is not the master server for partition {partitionId}, but it replicates that partition (object value: {value}).");
            else if (currentServerId != masterId && value == "N/A")
                Console.WriteLine($"This server (id: {currentServerId}) is not the master server for partition {partitionId} and it does not replicate that partition.");
            else if (currentServerId == masterId)
                Console.WriteLine($"This server (id: {currentServerId}) is the master server for partition {partitionId} (object value: {value}).");
            else
                // This should never happen
                Console.WriteLine("[READ] WTF Just Happened?!");


            return Task.FromResult(new ReadReply {
                Value = value,
                MasterId = masterId
            });
        }

        public override Task<WriteReply> WriteAdvanced(WriteRequest request, ServerCallContext context)
        {
            WaitUnfreeze();
            var partitionId = request.PartitionId;

            var masterId = _gigaStorage.GetMaster(partitionId);
            var currentServerId = _gigaStorage.ServerId;
            if (currentServerId != masterId) {
                Console.WriteLine($"This server (id: {currentServerId}) is not the master server for partition {partitionId}.");
                return Task.FromResult(new WriteReply {
                    MasterId = masterId
                });
            }

            _gigaStorage.WriteAdvanced(request.PartitionId, request.ObjectId, request.Value);

            // The current server is already the master for this partition
            return Task.FromResult(new WriteReply {
                MasterId = currentServerId
            });
        }

        public override Task<ListServerReply> ListServerAdvanced(ListServerRequest request, ServerCallContext context)
        {
            var currentServerId = _gigaStorage.ServerId;

            var reply = new ListServerReply();
            var objects = _gigaStorage.ListServerAdvanced();
            foreach (var o in objects) {
                var masterId = _gigaStorage.GetMaster(o.PartitionId);
                var inMaster = (currentServerId == masterId);

                var obj = new ListServerReply.Types.Object {
                    PartitionId = o.PartitionId,
                    ObjectId = o.ObjectId,
                    Value = o.Value,
                    InMaster = inMaster
                };
                reply.Objects.Add(obj);
            }
            return Task.FromResult(reply);
        }


        /* ====================================================================== */
        /* ====[                         Auxilliary                         ]==== */
        /* ====================================================================== */

        public override async Task<CheckStatusReply> CheckStatus(CheckStatusRequest request, ServerCallContext context)
        {
            // Add requested server ids to a Set, to remove all duplicates
            var ids = new HashSet<string>(request.ServerId.Count);
            foreach (var serverId in request.ServerId)
            {
                ids.Add(serverId);
            }

            // Check the status of each server id
            foreach (var serverId in ids)
            {
                Console.WriteLine($"Checking status of server {serverId}...");
                await _gigaStorage.CheckStatusAsync(serverId);
            }

            return await Task.FromResult(new CheckStatusReply
            {
                // Empty message as ack
            });
        }

        public override async Task<GetMasterReply> GetMaster(GetMasterRequest request, ServerCallContext context)
        {
            var masterId = _gigaStorage.GetMaster(request.PartitionId);
            return await Task.FromResult(new GetMasterReply
            {
                MasterId = masterId
            });
        }

        public void WaitUnfreeze()
        {
            Semaphore sem = _gigaStorage.GetFrozenSemaphore();
            sem.WaitOne();
            sem.Release();
        }

    }
}
