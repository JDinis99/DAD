using GigaStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Threading;
using System.Threading.Tasks;

namespace GigaClient
{
    class Program
    {
        private static readonly string UNKNOWN_COMMAND = "Unknown command.";

        private static readonly string HELP_STRING =
            "---------------------------------------\n" +
            "> read partition_id object_id server_id\n" +
            "Reads the object identified by the (partition_id, object_id) pair and outputs (onscreen and eventually to a log) the corresponding value.\n" +
            "It should return the string 'N/A' if the object is not present in the storage system. If the server that the clientis currently attached to does not hold the requested object, the client will use the 'server_id' parameter to try to find the object at the 'server_id' server.\n" +
            "If the client does not need to change server to obtain the requested object or if the 'server_id' parameter is '-1', the parameter should be ignored.\n" +
            "\n" +
            "> write partition_id object_id value\n" +
            "Writes the object identified by the (partition_id, object_id) pair and assigns it the quotes delimited string 'value' (e.g. \"a possible stored value\").\n" +
            "\n" +
            "> listServer server_id\n" +
            "Lists all objects stored on the server identified by 'server_id' and whether the server is the master replica for that object or not.\n" +
            "\n" +
            "> listGlobal\n" +
            "Lists the partition and object identifiers of all objects stored on the system.\n" +
            "\n" +
            "> wait x\n" +
            "Delays the execution of the next command for 'x' milliseconds.\n" +
            "\n" +
            "> begin-repeat x\n" +
            "Repeats 'x' number of times the commands following this command and before the next 'end-repeat'.\n" +
            "It is not possible to have another 'begin-repeat' command before this loop is closed by a 'end-repeat' command.\n" +
            "Within the repeat cycle any occurrence of the string '$i' should be replaced by the number of the iteration of that cycle being performed, from '0' to 'x−1'.\n" +
            "\n" +
            "> end-repeat\n" +
            "Closes a repeat loop.\n" +
            "---------------------------------------";


        static async Task Main(string[] args)
        {
            /* add shutdown hook in order to properly close the client */
            //Console.CancelKeyPress += new ConsoleCancelEventHandler(ShutdownHook);

            /* receive and print arguments */
            Console.WriteLine($"Received {args.Length} arguments:");
            for (int i = 0; i < args.Length; i++)
                Console.WriteLine($"  arg[{i}] = {args[i]}");

            /* check arguments amount */
            if (!(args.Length == 2 || args.Length == 3))
            {
                Console.WriteLine("Invalid amount of arguments.\n" +
                    "Usage: dotnet run serversCount isAdvanced [filename]");
                return;
            }

            /* validate arguments */
            if (!Int32.TryParse(args[0], out int serversCount) || serversCount <= 0)
            {
                Console.WriteLine("'serversCount' must be a positive value of type Int32.");
                return;
            }
            if (!Boolean.TryParse(args[1], out bool isAdvanced))
            {
                Console.WriteLine("'isAdvanced' must be a value of type Boolean.");
                return;
            }
            string filename = null;
            if (args.Length == 3)
            {
                // FIXME validate filename
                filename = args[2];
            }

            /* create a connection to the gRPC service */
            var random = new Random();
            var serverId = random.Next(serversCount) + 1;
            var frontend = new Frontend(serverId, serversCount, isAdvanced);

            /* read input from file */
            if (filename != null)
            {
                Console.WriteLine("Type a command ('help' for available commands).");
                try
                {
                    using var reader = new StreamReader(filename);
                    string fileline = null;
                    do
                    {
                        fileline = reader.ReadLine();
                        Console.WriteLine($"> ${fileline}");
                        if (fileline != null && fileline != "")
                            await ExecInputAsync(frontend, fileline, false, reader);

                    } while (fileline != null);

                } 
                catch (IOException e)
                {
                    Console.WriteLine($"IOException: {e.Message}");
                }
            }

            /* read input */
            Console.WriteLine("Type a command ('help' for available commands).");
            string line;
            do
            {
                Console.Write("> ");
                line = Console.ReadLine();
                if (line != null && line != "")
                    await ExecInputAsync(frontend, line, false);

            } while (true);

        } // Main


        private static async Task ExecInputAsync(Frontend frontend, string line, bool repeat, StreamReader reader = null)
        {
            try
            {
                string[] words = line.Split(' ');

                if (String.Equals(words[0], "read") && words.Length == 4)
                {
                    int partitionId = Int32.Parse(words[1]);
                    int objectId = Int32.Parse(words[2]);
                    int serverId = Int32.Parse(words[3]);

                    var readRequest = new ReadRequest
                    {
                        PartitionId = partitionId,
                        ObjectId = objectId,
                        ServerId = serverId
                    };

                    var readReply = await frontend.ReadAsync(readRequest);
                    Console.WriteLine(readReply.Value);

                }
                else if (String.Equals(words[0], "write") && words.Length > 3)
                {
                    int partitionId = Int32.Parse(words[1]);
                    int objectId = Int32.Parse(words[2]);

                    // parse 'value' from input
                    string value;
                    try
                    {
                        int firstQuote = line.IndexOf('"');
                        int secondQuote = line.IndexOf('"', firstQuote + 1);
                        int valueLength = secondQuote + 1 - firstQuote; // include quotes
                        value = line.Substring(firstQuote, valueLength);
                        if (line.Substring(secondQuote + 1).Trim() != "")
                            throw new InvalidOperationException(UNKNOWN_COMMAND);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new InvalidOperationException(UNKNOWN_COMMAND);
                    }

                    var writeRequest = new WriteRequest
                    {
                        PartitionId = partitionId,
                        ObjectId = objectId,
                        Value = value
                    };

                    var writeReply = await frontend.WriteAsync(writeRequest);
                    Console.WriteLine("Write operation was sucessfull.");

                }
                else if (String.Equals(words[0], "listServer") && words.Length == 2)
                {
                    int serverId = Int32.Parse(words[1]);

                    var listServerRequest = new ListServerRequest();
                    var listServerReply = await frontend.ListServerAsync(listServerRequest, serverId);

                    // TODO write message for empty results
                    Console.WriteLine("PartitionId | ObjectId | Value | In Master?");
                    foreach (var o in listServerReply.Objects)
                    {
                        string inMaster = o.InMaster ? "Yes" : "No";
                        // TODO insert tabs
                        Console.WriteLine($"{o.PartitionId} | {o.ObjectId} | {o.Value} | {inMaster}");
                    }

                }
                else if (String.Equals(words[0], "listGlobal") && words.Length == 1)
                {
                    var listGloabalRequest = new ListServerRequest();
                    var listGlobalReply = await frontend.ListGlobalAsync(listGloabalRequest);

                    for (var serverId = 1; serverId < listGlobalReply.Length; serverId++)
                    {
                        // TODO write message for empty results
                        Console.WriteLine($"[SERVER {serverId}]");
                        foreach (var obj in listGlobalReply[serverId].Objects)
                        {
                            Console.WriteLine($"  PartitionId: {obj.PartitionId}, ObjectId: {obj.ObjectId}");
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
                        if (reader == null)
                        {
                            command = Console.ReadLine();
                        }
                        else
                        {
                            command = reader.ReadLine();
                            Console.WriteLine(command);
                        }

                        if (command != "" && command != null)
                            commands.Add(command);
                    }
                    while (!String.Equals(command, "end-repeat"));

                    for (int i = 1; i <= iterations; i++)
                    {
                        foreach (var c in commands)
                        {
                            command = c.Replace("$i", i.ToString());
                            await ExecInputAsync(frontend, command, true, reader);
                        }
                    }

                }
                else if (String.Equals(words[0], "end-repeat") && words.Length == 1)
                {
                    if (!repeat)
                        throw new InvalidOperationException("There isn't a repeat loop to close.");

                    // FIXME throw error, should never enter here

                }
                else if (String.Equals(words[0], "help") && words.Length == 1)
                {
                    Console.WriteLine(HELP_STRING);
                }
                else
                {
                    throw new InvalidOperationException(UNKNOWN_COMMAND);
                }

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
            catch (IOException e)
            {
                Console.WriteLine($"IOException: {e.Message}");
            }

        } // ExecInputAsync


        private static void ShutdownHook(object sender, EventArgs args)
        {
            // FIXME ShutdownHook not working properly
            // TODO close connection
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    } // class

} // namespace
