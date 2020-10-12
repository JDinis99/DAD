using GigaStore;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace GigaClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /* add shutdown hook in order to close the client */
            // Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutdownHook);

            /* receive and print arguments */
            Console.WriteLine($"Received {args.Length} arguments.");
            for (int i = 0; i < args.Length; i++)
                Console.WriteLine($"arg[{i}] = {args[i]}");

            /* check arguments amount */
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid amount of arguments.");
                Console.WriteLine("Usage: dotnet run [n_servers]");
                return;
            }

            /* validate arguments */
            if (!Int32.TryParse(args[0], out int n_servers) || n_servers <= 0)
            {
                Console.WriteLine("'n_servers' must be a positive value of type Int32.");
                return;
            }

            /* create a connection to the gRPC service */
            var random = new Random();
            var instance = random.Next(n_servers);
            // TODO (?) remove hard-coded, use json file
            using var channel = GrpcChannel.ForAddress("https://localhost:500" + (instance + 1));
            var client = new Giga.GigaClient(channel);
            Console.WriteLine($"Connected to server {instance + 1}.");
            // TODO find how to close channel/client and do it

            /* read input */
            Console.WriteLine("Type a command ('help' for available commands).");
            do
            {
                // FIXME (?) try catch not working outside of ExecInputAsync
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line != null && line != "")
                    await ExecInputAsync(client, n_servers, line, false);

            } while (true);

        }


        private static void ShutdownHook(object sender, EventArgs args)
        {
            // TODO close connection
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task ExecInputAsync(Giga.GigaClient client, int n_servers, string line, bool repeat)
        {
            try
            {
                string[] words = line.Split(' ');

                if (String.Equals(words[0], "read") && words.Length == 4)
                {
                    int partition_id = Int32.Parse(words[1]);
                    int object_id = Int32.Parse(words[2]);
                    int server_id = Int32.Parse(words[3]);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = partition_id,
                        ObjectId = object_id,
                        ServerId = server_id
                    };
                    var readReply = await client.ReadAsync(readRequest);
                    Console.WriteLine(readReply.Value);

                    if (String.Equals(readReply.Value, "N/A") && server_id != -1)
                    {
                        Console.Write($"Connecting to server {server_id}... ");
                        using var channel = GrpcChannel.ForAddress("https://localhost:500" + server_id);
                        client = new Giga.GigaClient(channel);
                        Console.WriteLine("Connected.");

                        readReply = await client.ReadAsync(readRequest);
                        Console.WriteLine(readReply.Value);
                    }

                }
                else if (String.Equals(words[0], "write") && words.Length > 3)
                {
                    int partition_id = Int32.Parse(words[1]);
                    int object_id = Int32.Parse(words[2]);

                    // parse 'value' from input
                    string value;
                    try
                    {
                        int firstQuote = line.IndexOf('"');
                        int secondQuote = line.IndexOf('"', firstQuote + 1);
                        int valueLength = secondQuote + 1 - firstQuote; // include quotes
                        value = line.Substring(firstQuote, valueLength);
                        if (line.Substring(secondQuote + 1).Trim() != "")
                            throw new InvalidOperationException("Unknown command.");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new InvalidOperationException("Unknown command.");
                    }

                    var writeRequest = new WriteRequest
                    {
                        PartitionId = partition_id,
                        ObjectId = object_id,
                        Value = value
                    };
                    var writeReply = await client.WriteAsync(writeRequest);

                    var master_id = writeReply.MasterId;
                    if (master_id != -1)
                    {
                        Console.Write($"Connecting to master server (id: {master_id}) for partition {partition_id}... ");
                        using var channel = GrpcChannel.ForAddress("https://localhost:500" + master_id);
                        client = new Giga.GigaClient(channel);
                        Console.WriteLine("Connected.");

                        writeReply = await client.WriteAsync(writeRequest);
                    }

                    Console.WriteLine("Write operation was sucessfull.");

                }
                else if (String.Equals(words[0], "listServer") && words.Length == 2)
                {
                    int server_id = Int32.Parse(words[1]);

                    // FIXME if same server as current, dont start new connection
                    Console.Write($"Connecting to server {server_id}... ");
                    using var channel = GrpcChannel.ForAddress("https://localhost:500" + server_id);
                    client = new Giga.GigaClient(channel);
                    Console.WriteLine("Connected.");

                    var listServerRequest = new ListServerRequest();
                    var listServerReply = await client.ListServerAsync(listServerRequest);

                    // TODO (?) write message for empty results
                    Console.WriteLine("PartitionId | ObjectId | Value | In Master?");
                    foreach (var o in listServerReply.Objects)
                    {
                        string inMaster = o.InMaster ? "Yes" : "No";
                        // FIXME insert tabs
                        Console.WriteLine($"{o.PartitionId} | {o.ObjectId} | {o.Value} | {inMaster}");
                    }

                }
                else if (String.Equals(words[0], "listGlobal") && words.Length == 1)
                {
                    var listServerRequest = new ListServerRequest();
                    for (int server_id = 1; server_id <= n_servers; server_id++)
                    {
                        Console.Write($"Connecting to server {server_id}... ");
                        using var channel = GrpcChannel.ForAddress("https://localhost:500" + server_id);
                        client = new Giga.GigaClient(channel);
                        Console.WriteLine("Connected.");

                        var listServerReply = await client.ListServerAsync(listServerRequest);

                        // TODO (?) write message for empty results
                        Console.WriteLine($"[SERVER {server_id}]");
                        foreach (var o in listServerReply.Objects)
                        {
                            Console.WriteLine($"  PartitionId: {o.PartitionId}, ObjectId: {o.ObjectId}");
                        }
                    }

                }
                else if (String.Equals(words[0], "wait") && words.Length == 2)
                {
                    int milliseconds = Int32.Parse(words[1]);
                    Thread.Sleep(milliseconds);

                }
                else if (String.Equals(words[0], "begin-repeat") && words.Length == 2)
                {
                    if (repeat)
                        throw new InvalidOperationException("It is not possible to have another begin-repeat command before this loop is closed by a end-repeat command.");

                    int iterations = Int32.Parse(words[1]);
                    if (iterations < 0)
                        throw new ArgumentOutOfRangeException("The number of iterations must be non-negative.");

                    var commands = new List<string>();
                    string command;
                    do
                    {
                        Console.Write(">> ");
                        command = Console.ReadLine();
                        if (command != "" && command != null)
                            commands.Add(command);
                    }
                    while (!String.Equals(command, "end-repeat"));

                    for (int i = 1; i <= iterations; i++)
                    {
                        foreach (var c in commands)
                        {
                            command = c.Replace("$i", i.ToString());
                            await ExecInputAsync(client, n_servers, command, true);
                        }
                    }

                }
                else if (String.Equals(words[0], "end-repeat") && words.Length == 1)
                {
                    if (!repeat)
                        throw new InvalidOperationException("There isn't a repeat loop to close.");

                }
                else if (String.Equals(words[0], "help") && words.Length == 1)
                {
                    // TODO help text
                    Console.Write("  .d88b.   .d88b.  d88888b \n" +
                                  " .8P  Y8. .8P  Y8. 88'     \n" +
                                  " 88    88 88    88 88ooo   \n" +
                                  " 88    88 88    88 88~~~   \n" +
                                  " `8b  d8' `8b  d8' 88      \n" +
                                  "  `Y88P'   `Y88P'  YP      \n");
                }
                else
                {
                    throw new InvalidOperationException("Unknown command.");
                }

                // FIXME (?) atm the client always reconnects to the server in which he started

            }
            catch (FormatException e)
            {
                Console.WriteLine($"FormatException: {e.Message}");
            }
            catch (OverflowException e)
            {
                Console.WriteLine($"OverflowException: {e.Message}");
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"ArgumentOutOfRangeException: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"InvalidOperationException: {e.Message}");
            }
        }
    }
}
