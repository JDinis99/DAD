using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GigaStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string url = "https://localhost:500" + args[0];
                    GigaStorage giga = GigaStorage.GetGigaStorage();
                    webBuilder.UseUrls(url);

                    giga.SetServerId(Int32.Parse(args[0]));
                    giga.SetNumberOfServers(Int32.Parse(args[1]));

                    webBuilder.UseStartup<Startup>();
                });
    }
}
