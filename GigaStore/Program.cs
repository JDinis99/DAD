using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
            if (args.Length != 6)
            {
                Console.WriteLine("Invalid amount of arguments.\n" + "Usage: dotnet run serverId url minDelay maxDelay serversCount isAdvanced");
                return;
            }

            /* validate arguments */
            if (!Int32.TryParse(args[2], out int minDelay) || minDelay < 0)
            {
                Console.WriteLine("'minDelay' must be a positive value of type Int32.");
                return;
            }
            if (!Int32.TryParse(args[3], out int maxDelay) || maxDelay < 0)
            {
                Console.WriteLine("'maxDelay' must be a positive value of type Int32.");
                return;
            }
            if (!Int32.TryParse(args[4], out int nservers) || nservers <= 0)
            {
                Console.WriteLine("'serversCount' must be a positive value of type Int32.");
                return;
            }
            if (!Boolean.TryParse(args[5], out bool isAdvanced))
            {
                Console.WriteLine("'isAdvanced' must be a value of type Boolean.");
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
                    var serverId = args[0];
                    var url = args[1];
                    var minDelay = Int32.Parse(args[2]);
                    var maxDelay = Int32.Parse(args[3]);
                    var serversCount = Int32.Parse(args[4]);
                    var isAdvanced = Boolean.Parse(args[5]);

                    GigaStorage giga = GigaStorage.GetGigaStorage();
                    webBuilder.UseUrls(url);

                    giga.MinDelay = minDelay;
                    giga.MaxDelay = maxDelay;
                    giga.ServerId = serverId;
                    giga.IsAdvanced = isAdvanced;

                    webBuilder.UseStartup<Startup>();
                });
    }
}
