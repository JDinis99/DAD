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
            var channel1 = GrpcChannel.ForAddress("https://localhost:5001");
            var client1 = new Giga.GigaClient(channel1);

            var channel2 = GrpcChannel.ForAddress("https://localhost:5002");
            var client2 = new Giga.GigaClient(channel2);

            var channel3 = GrpcChannel.ForAddress("https://localhost:5003");
            var client3 = new Giga.GigaClient(channel3);



            var writeRequest1 = new WriteRequest { PartitionId = 1, ObjectId = 1, Value = "Praise the Sun" };
            var writeRequest12 = new WriteRequest { PartitionId = 1, ObjectId = 1, Value = "Praise the Sun 222222" };

            await client1.WriteAsync(writeRequest1);
            Console.WriteLine("Value Stored 1");

            await client1.WriteAsync(writeRequest12);
            Console.WriteLine("Value Stored 12");

            var readRequest1 = new ReadRequest { PartitionId = 1, ObjectId = 1, ServerId = 1 };
            var readReply1 = await client1.ReadAsync(readRequest1);
            Console.WriteLine("value 1 from server 1: " + readReply1.Value);

            System.Threading.Thread.Sleep(1000);
            
             var writeRequest2 = new WriteRequest { PartitionId = 2, ObjectId = 1, Value = "Nuclear Launch codes" };
             await client2.WriteAsync(writeRequest2);
             Console.WriteLine("Value Stored 2");


            var readRequest21 = new ReadRequest { PartitionId = 1, ObjectId = 1, ServerId = 2 };
            var readReply21 = await client2.ReadAsync(readRequest21);
            Console.WriteLine("value 1 from server 2: " + readReply21.Value);

            var readRequest22 = new ReadRequest { PartitionId = 2, ObjectId = 1, ServerId = 2 };
            var readReply22 = await client2.ReadAsync(readRequest22);
            Console.WriteLine("value 2 from server 2: " + readReply22.Value);

            System.Threading.Thread.Sleep(1000);
            
            var writeRequest3 = new WriteRequest { PartitionId = 3, ObjectId = 1, Value = "Very confidential information" };
            await client3.WriteAsync(writeRequest3);
            Console.WriteLine("Value Stored 3");

            var readRequest31 = new ReadRequest { PartitionId = 1, ObjectId = 1, ServerId = 3 };
            var readReply31 = await client3.ReadAsync(readRequest31);
            Console.WriteLine("value 1 from server 3: " + readReply31.Value);

            var readRequest32 = new ReadRequest { PartitionId = 2, ObjectId = 1, ServerId = 3 };
            var readReply32 = await client3.ReadAsync(readRequest32);
            Console.WriteLine("value 2 from server 3: " + readReply32.Value);

            var readRequest33 = new ReadRequest { PartitionId = 3, ObjectId = 1, ServerId = 3 };
            var readReply33 = await client3.ReadAsync(readRequest33);
            Console.WriteLine("value 1 from server 3: " + readReply33.Value);


            System.Threading.Thread.Sleep(1000);

            var readRequest11 = new ReadRequest { PartitionId = 1, ObjectId = 1, ServerId = 1 };
            var readReply11 = await client1.ReadAsync(readRequest11);
            Console.WriteLine("value 1 from server 1: " + readReply11.Value);

            var readRequest12 = new ReadRequest { PartitionId = 2, ObjectId = 1, ServerId = 1 };
            var readReply12 = await client1.ReadAsync(readRequest12);
            Console.WriteLine("value 2 from server 1: " + readReply12.Value);

            var readRequest13 = new ReadRequest { PartitionId = 3, ObjectId = 1, ServerId = 1 };
            var readReply13 = await client1.ReadAsync(readRequest13);
            Console.WriteLine("value 3 from server 1: " + readReply13.Value);
            
            Console.ReadLine();
        }
    }
}
