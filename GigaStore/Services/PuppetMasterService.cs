using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GigaStore.Services
{
    public class PuppetMasterService : PuppetMaster.PuppetMasterBase
    {
        private readonly ILogger<PuppetMasterService> _logger;
        private GigaStorage _gigaStorage;

        public PuppetMasterService(ILogger<PuppetMasterService> logger)
        {
            _logger = logger;
            _gigaStorage = GigaStorage.GetGigaStorage();
        }

        public override Task<ReplicationFactorReply> ReplicationFactor(ReplicationFactorRequest request, ServerCallContext context)
        {
            Console.WriteLine("A MUDAR O REPLICATION PARA VALOR: " + request.Factor);
            _gigaStorage.ChangeReplicationFactor(request.Factor);
            return Task.FromResult(new ReplicationFactorReply { Ack = "Success" });
        }

        public override Task<PartitionReply> Partition(PartitionRequest request, ServerCallContext context)
        {
            String[] ids_as_list = request.Ids.Split(" ");
            List<string> ids = new List<string>();
            for (int i=0; i<ids_as_list.Length; i++)
            {
                ids.Add(ids_as_list[i]);
            }
            _gigaStorage.MakePartition(request.Name, ids, ids_as_list[0]);
            return Task.FromResult(new PartitionReply { Ack = "Success" });
        }

        public override Task<StatusReply> Status(StatusRequest request, ServerCallContext context)
        {
            Console.WriteLine("Server up and running ");
            return Task.FromResult(new StatusReply { Ack = "Success" });
        }

        public override Task<CrashReply> CrashServer(CrashRequest request, ServerCallContext context)
        {
            try
            {
                var success = _gigaStorage.Crash();
                if (success)
                {
                    return Task.FromResult(new CrashReply { Ack = "Success" });
                }
                return Task.FromResult(new CrashReply { Ack = "Unsuccess" }); //couldn't crash if it reaches here
            }
            catch (Win32Exception e)
            {
                _logger.LogError(e.Message);
                return Task.FromResult(new CrashReply { Ack = "Unsuccess" });
            }
        }

        public override Task<FreezeReply> FreezeServer(FreezeRequest request, ServerCallContext context)
        {
            _gigaStorage.FreezeServer();
            return Task.FromResult(new FreezeReply { Ack = "Success" });
        }

        public override Task<UnfreezeReply> UnfreezeServer(UnfreezeRequest request, ServerCallContext context)
        {
            _gigaStorage.UnfreezeServer();
            return Task.FromResult(new UnfreezeReply { Ack = "Success" });
        }

        public override Task<InitServerReply> InitServer(InitServerRequest request, ServerCallContext context)
        {
            List<String> ids = new List<String>();
            List<String> urls = new List<String>();
            String[] ids_string_as_list = request.Ids.Split(" ");
            String[] urls_string_as_list = request.Urls.Split(" ");
            for (int i = 0; i < ids_string_as_list.Length; i++)
            {
                ids.Add(ids_string_as_list[i]);
                urls.Add(urls_string_as_list[i]);
            }
            _gigaStorage.Init(ids, urls);
            return Task.FromResult(new InitServerReply { Ack = "Success" });
        }

    }
}