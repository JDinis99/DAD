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
            /* receive and print arguments */
            Console.WriteLine($"Received {args.Length} arguments:");
            for (int i = 0; i < args.Length; i++)
                Console.WriteLine($"  arg[{i}] = {args[i]}");

            /* check arguments amount */
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid amount of arguments.\n" + "Usage: dotnet run serverId nservers");
                return;
            }

            /* validate arguments */
            if (!Int32.TryParse(args[0], out int serverId) || serverId <= 0)
            {
                Console.WriteLine("'serverId' must be a positive value of type Int32.");
                return;
            }
            if (!Int32.TryParse(args[1], out int nservers) || nservers <= 0)
            {
                Console.WriteLine("'n_servers' must be a positive value of type Int32.");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // FIXME variables already parsed and assigned
                    var serverId = Int32.Parse(args[0]);
                    var nservers = Int32.Parse(args[1]);

                    string url = "https://localhost:500" + serverId;
                    GigaStorage giga = GigaStorage.GetGigaStorage();
                    webBuilder.UseUrls(url);

                    giga.SetServerId(serverId);
                    giga.SetNumberOfServers(nservers);

                    webBuilder.UseStartup<Startup>();
                });
    }
}
