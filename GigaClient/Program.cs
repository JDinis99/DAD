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

            var channel4 = GrpcChannel.ForAddress("https://localhost:5004");
            var client4 = new Giga.GigaClient(channel4);

            var channel5 = GrpcChannel.ForAddress("https://localhost:5005");
            var client5 = new Giga.GigaClient(channel5);


            var writeRequest1 = new WriteRequest { PartitionId = 1, ObjectId = 1, Value = "Praise the Sun" };
            var writeRequest2 = new WriteRequest { PartitionId = 2, ObjectId = 1, Value = "Nuclear Launch codes" };
            var writeRequest3 = new WriteRequest { PartitionId = 3, ObjectId = 1, Value = "Very confidential information" };
            var writeRequest4 = new WriteRequest { PartitionId = 4, ObjectId = 1, Value = "Homework folder" };
            var writeRequest5 = new WriteRequest { PartitionId = 5, ObjectId = 1, Value = "Hot Milfs in your area ad" };

            await client1.WriteAsync(writeRequest1);
            Console.WriteLine("Value Stored 1");

            await client2.WriteAsync(writeRequest2);
            Console.WriteLine("Value Stored 2");

            await client3.WriteAsync(writeRequest3);
            Console.WriteLine("Value Stored 3");

            await client4.WriteAsync(writeRequest4);
            Console.WriteLine("Value Stored 4");

            await client5.WriteAsync(writeRequest5);
            Console.WriteLine("Value Stored 5");

            Console.WriteLine("5 seconds to crash");
            System.Threading.Thread.Sleep(5000);





            
            await client1.WriteAsync(writeRequest1);
            Console.WriteLine("Value Stored 1");

            /*
            await client2.WriteAsync(writeRequest2);
            Console.WriteLine("Value Stored 2");
            */

            await client3.WriteAsync(writeRequest3);
            Console.WriteLine("Value Stored 3");

            await client4.WriteAsync(writeRequest4);
            Console.WriteLine("Value Stored 4");

            await client5.WriteAsync(writeRequest5);
            Console.WriteLine("Value Stored 5");

            Console.WriteLine("10 seconds to propagate");
            System.Threading.Thread.Sleep(10000);

            Console.WriteLine("");
            var readRequest1 = new ReadRequest { PartitionId = 1, ObjectId = 1, ServerId = 1 };
            var readRequest2 = new ReadRequest { PartitionId = 2, ObjectId = 1, ServerId = 1 };
            var readRequest3 = new ReadRequest { PartitionId = 3, ObjectId = 1, ServerId = 1 };
            var readRequest4 = new ReadRequest { PartitionId = 4, ObjectId = 1, ServerId = 1 };
            var readRequest5 = new ReadRequest { PartitionId = 5, ObjectId = 1, ServerId = 1 };


            Console.WriteLine("Value From Server 1");

            var readReply11 = await client1.ReadAsync(readRequest1);
            Console.WriteLine("value 1: " + readReply11.Value);

            var readReply12 = await client1.ReadAsync(readRequest2);
            Console.WriteLine("value 2: " + readReply12.Value);

            var readReply13 = await client1.ReadAsync(readRequest3);
            Console.WriteLine("value 3: " + readReply13.Value);

            var readReply14 = await client1.ReadAsync(readRequest4);
            Console.WriteLine("value 4: " + readReply14.Value);

            var readReply15 = await client1.ReadAsync(readRequest5);
            Console.WriteLine("value 5: " + readReply15.Value);

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("");



            /*
            Console.WriteLine("Value From Server 2");

            var readReply21 = await client2.ReadAsync(readRequest1);
            Console.WriteLine("value 1: " + readReply21.Value);

            var readReply22 = await client2.ReadAsync(readRequest2);
            Console.WriteLine("value 2: " + readReply22.Value);

            var readReply23 = await client2.ReadAsync(readRequest3);
            Console.WriteLine("value 3: " + readReply23.Value);

            var readReply24 = await client2.ReadAsync(readRequest4);
            Console.WriteLine("value 4: " + readReply24.Value);

            var readReply25 = await client2.ReadAsync(readRequest5);
            Console.WriteLine("value 5: " + readReply25.Value);

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("");
            */

            Console.WriteLine("Value From Server 3");

            var readReply31 = await client3.ReadAsync(readRequest1);
            Console.WriteLine("value 1: " + readReply31.Value);

            var readReply32 = await client3.ReadAsync(readRequest2);
            Console.WriteLine("value 2: " + readReply32.Value);

            var readReply33 = await client3.ReadAsync(readRequest3);
            Console.WriteLine("value 3: " + readReply33.Value);

            var readReply34 = await client3.ReadAsync(readRequest4);
            Console.WriteLine("value 4: " + readReply34.Value);

            var readReply35 = await client3.ReadAsync(readRequest5);
            Console.WriteLine("value 5: " + readReply35.Value);

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("");


            Console.WriteLine("Value From Server 4");

            var readReply41 = await client4.ReadAsync(readRequest1);
            Console.WriteLine("value 1: " + readReply41.Value);

            var readReply42 = await client4.ReadAsync(readRequest2);
            Console.WriteLine("value 2: " + readReply42.Value);

            var readReply43 = await client4.ReadAsync(readRequest3);
            Console.WriteLine("value 3: " + readReply43.Value);

            var readReply44 = await client4.ReadAsync(readRequest4);
            Console.WriteLine("value 4: " + readReply44.Value);

            var readReply45 = await client4.ReadAsync(readRequest5);
            Console.WriteLine("value 5: " + readReply45.Value);

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("");



            Console.WriteLine("Value From Server 5");

            var readReply51 = await client5.ReadAsync(readRequest1);
            Console.WriteLine("value 1: " + readReply51.Value);

            var readReply52 = await client5.ReadAsync(readRequest2);
            Console.WriteLine("value 2: " + readReply52.Value);

            var readReply53 = await client5.ReadAsync(readRequest3);
            Console.WriteLine("value 3: " + readReply53.Value);

            var readReply54 = await client5.ReadAsync(readRequest4);
            Console.WriteLine("value 4: " + readReply54.Value);

            var readReply55 = await client5.ReadAsync(readRequest5);
            Console.WriteLine("value 5: " + readReply55.Value);

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("");

            Console.ReadLine();
        }
    }
}
