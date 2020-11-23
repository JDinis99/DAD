using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GigaClient.Services
{
    public class PuppetMasterClientService : PuppetMasterClient.PuppetMasterClientBase
    {
        private readonly ILogger<PuppetMasterClientService> _logger;

        public PuppetMasterClientService(ILogger<PuppetMasterClientService> logger)
        {
            _logger = logger;
        }

        public override Task<ClientStatusReply> ClientStatus(ClientStatusRequest request, ServerCallContext context)
        {
            Console.WriteLine("STATUS: Client up and running.");
            return Task.FromResult(new ClientStatusReply { Ack = "Success" });
        }
    }
}