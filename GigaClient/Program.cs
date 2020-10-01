using GigaStore;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace GigaClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // TODO tirar isto de hard-coded. Por num json file ou algo de genero
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Giga.GigaClient(channel);

            var writeRequest = new WriteRequest { PartitionId = 1, ObjectId = 1, Value = "Praise the Sun" };
            await client.WriteAsync(writeRequest);
            Console.WriteLine("Value Stored");

            var readRequest = new ReadRequest { PartitionId = 1, ObjectId = 1, ServerId = 1 };
            var readReply = await client.ReadAsync(readRequest);
            Console.WriteLine( "value: " + readReply.Value);

            Console.ReadLine();
        }
    }
}
