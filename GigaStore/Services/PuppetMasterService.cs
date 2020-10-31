using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        public override Task<StatusReply> Status(StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new StatusReply { Ack = "Server up and running " });
        }

        public override Task<CrashReply> CrashServer(CrashRequest request, ServerCallContext context)
        {
            try
            {
                Process.GetCurrentProcess().Kill();
                return Task.FromResult(new CrashReply { Ack = "Success" });
            }
            catch (Win32Exception e)
            {
                _logger.LogError(e.Message);
                return Task.FromResult(new CrashReply { Ack = "Unsuccess" });
            }
        }
    }
}