using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using GigaStore;
using Grpc.Net.Client;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection.Metadata;
using System.IO;

namespace PuppetMaster
{
    public partial class PuppetMaster : Form
    {
        private int _no_servers = 5;
        private Boolean _initedServers = false;
        private Dictionary<string, GrpcChannel> _channels = new Dictionary<string, GrpcChannel>(); 
        private Dictionary<string, GigaStore.PuppetMaster.PuppetMasterClient> _puppetServerClients = new Dictionary<string, GigaStore.PuppetMaster.PuppetMasterClient>();

        public delegate void ReplicationFactorDelegate(String factor);
        public delegate void CreateServerDelegate(String[] args);
        public delegate void PartitionDelegate(String[] args);
        public delegate void CreateClientDelegate(String[] args);
        public delegate void StatusDelegate();
        public delegate void CrashServerDelegate(String serverId);
        public delegate void FreezeServerDelegate(String serverId);
        public delegate void UnfreezeServerDelegate(String serverId);
        public delegate void RunScriptDelegate(string filename);
        private delegate void SafeCallDelegate(string text);

        public PuppetMaster()
        {
            InitializeComponent();
        }

        public void ReplicationFactor(String factor)
        {
            Boolean success = Int32.TryParse(factor, out int rep_factor);
            if (!success || rep_factor <= 0)
            {
                WriteToLogger("Replication factor must be a positive integer greater than 0");
                WriteToLogger(Environment.NewLine);
                return;
            }

            WriteToLogger("Configuring system to replicate partitions on " + rep_factor + " servers...");
            WriteToLogger(Environment.NewLine);
            foreach (string id in _puppetServerClients.Keys)
            {
                _puppetServerClients[id].ReplicationFactor(new ReplicationFactorRequest { Factor = rep_factor });
            }
        }

        public void CreateServer(String[] args)
        {
            string server_id = args[0];
            string server_url = args[1];
            Boolean success = Int32.TryParse(args[2], out int min_delay);
            if (!success || min_delay < 0)
            {
                WriteToLogger("Min delay must be a positive integer.");
                WriteToLogger(Environment.NewLine);
                return;
            }
            success = Int32.TryParse(args[3], out int max_delay);
            if (!success || max_delay < 0)
            {
                WriteToLogger("Max delay must be a positive integer.");
                WriteToLogger(Environment.NewLine);
                return;
            }

            if (!_channels.ContainsKey(server_id))
            {
                WriteToLogger("Creating new server with id " + server_id + " on URL " + server_url + " with delay between " + min_delay + "ms and " + max_delay + "ms...");
                WriteToLogger(Environment.NewLine);
                Process newServer = new Process();
                newServer.StartInfo.FileName = ".\\..\\..\\..\\..\\GigaStore\\bin\\Debug\\netcoreapp3.1\\GigaStore.exe";
                newServer.StartInfo.Arguments = server_id + " " + server_url + " " + min_delay + " " + max_delay + " " + _no_servers;
                newServer.Start();
                Thread.Sleep(500); // give a little for server to init
                lock (this)
                {
                    _channels.Add(server_id, GrpcChannel.ForAddress(server_url));
                    _puppetServerClients.Add(server_id, new GigaStore.PuppetMaster.PuppetMasterClient(_channels[server_id]));
                }
                WriteToLogger(Environment.NewLine);
            }
            else
            {
                WriteToLogger("Server with id " + server_id + " already exists.");
                WriteToLogger(Environment.NewLine);
            }
        }

        public void Partition(String[] args)
        {
            Boolean success = Int32.TryParse(args[0], out int rep_factor);
            if (!success || rep_factor <= 0 || rep_factor != (args.Length - 2))
            {
                WriteToLogger("Replication factor must be a positive integer greater than 0 and it must match the count of server ids given.");
                WriteToLogger(Environment.NewLine);
                return;
            }
            string partition_name = args[1];
            string list_servers = "";
            for (int i = 2; i < args.Length; i++)
            {
                if (!_channels.ContainsKey(args[i]))
                {
                    WriteToLogger("Server with id " + args[i] + " doesn't exist. Partition not created.");
                    WriteToLogger(Environment.NewLine);
                    return;
                }
                list_servers += args[i] + " ";
            }
            // Make sure all servers are inited
            InitAllServers();
            WriteToLogger("Storing " + rep_factor + " replicas of partition " + partition_name + " on servers " + list_servers + "...");
            WriteToLogger(Environment.NewLine);
            for (int i = 2; i < args.Length; i++)
            {
                _puppetServerClients[args[i]].Partition(new PartitionRequest { Factor = rep_factor, Name = partition_name });
            }
        }

        public void CreateClient(String[] args)
        {

        }

        public void Status() {
            WriteToLogger("Asking nodes to print their statuses...");
            WriteToLogger(Environment.NewLine);
            foreach (string id in _puppetServerClients.Keys)
            {
                try
                {
                    var reply = _puppetServerClients[id].Status(new StatusRequest { });
                    if (reply.Ack.Equals("Success"))
                    {
                        WriteToLogger("Server with id " + id + " is up and running.");
                        WriteToLogger(Environment.NewLine);
                    }
                }
                catch (Exception)
                {
                    WriteToLogger("Server with id " + id + " couldn't be reached.");
                    WriteToLogger(Environment.NewLine);
                }
            }
            WriteToLogger(Environment.NewLine);
        }

        public void FreezeServer(String serverId)
        {
            try
            {
                var reply = _puppetServerClients[serverId].FreezeServer(new FreezeRequest { });
                WriteToLogger("Freezing server with id " + serverId + "...");
                WriteToLogger(Environment.NewLine);
            }
            catch (Exception)
            {
                WriteToLogger("Server with id " + serverId + " couldn't be reached or does not exist.");
                WriteToLogger(Environment.NewLine);
            }
            WriteToLogger(Environment.NewLine);
        }

        public void UnfreezeServer(String serverId)
        {
            try
            {
                var reply = _puppetServerClients[serverId].UnfreezeServer(new UnfreezeRequest { });
                WriteToLogger("Unfreezing server with id " + serverId + "...");
                WriteToLogger(Environment.NewLine);
            }
            catch (Exception)
            {
                WriteToLogger("Server with id " + serverId + " couldn't be reached or does not exist.");
                WriteToLogger(Environment.NewLine);
            }
            WriteToLogger(Environment.NewLine);
        }

        public void CrashServer(String serverId)
        {
            try
            {
                var reply = _puppetServerClients[serverId].CrashServer(new CrashRequest { });
                _channels.Remove(serverId);
                _puppetServerClients.Remove(serverId);
                if (reply.Ack.Equals("Unsuccess"))
                {
                    WriteToLogger("Server with id " + serverId + " couldn't crash");
                    WriteToLogger(Environment.NewLine);
                }
            }
            catch (Exception)
            {
                _channels.Remove(serverId);
                _puppetServerClients.Remove(serverId);
                WriteToLogger("Server with id " + serverId + " couldn't be reached - either it successfully crashed or it doesn't exist.");
                WriteToLogger(Environment.NewLine);
            }
            WriteToLogger(Environment.NewLine);
        }

        private void BtReplicationClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("replication");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                ReplicationFactorDelegate del = new ReplicationFactorDelegate(ReplicationFactor);
                var workTask = Task.Run(() => del.Invoke(form.item1));
            }
        }

        private void BtServerClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("server");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                String[] args = new String[4];
                args[0] = form.item1;
                args[1] = form.item2;
                args[2] = form.item3;
                args[3] = form.item4;
                CreateServerDelegate del = new CreateServerDelegate(CreateServer);
                var workTask = Task.Run(() => del.Invoke(args));
            }
        }

        private void BtPartitionClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("partition");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                int server_count = Int32.Parse(form.item1);
                String[] args = new String[2+server_count];
                args[0] = form.item1;
                args[1] = form.item2;
                // Send ids as separate list item to be processed the same way as if it were a script
                String ids = form.item3;
                String[] ids_list = ids.Split(" ");
                for (int i=0; i < server_count; i++)
                    args[i+2] = ids_list[i];
                WriteToLogger(String.Join(",", args));
                PartitionDelegate del = new PartitionDelegate(Partition);
                var workTask = Task.Run(() => del.Invoke(args));
            }
        }

        private void BtClientClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("client");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string username = form.item1;
                string client_url = form.item2;
                string script_file = form.item3;
                WriteToLogger("Creating new client with username " + username + " on URL " + client_url + " to run script " + script_file + "...");
                WriteToLogger(Environment.NewLine);
            }
        }

        private void BtStatusClick(object sender, EventArgs e)
        {
            StatusDelegate del = new StatusDelegate(Status);
            var workTask = Task.Run(() => del.Invoke());
        }

        private void BtFreezeClick(object sender, EventArgs e)
        {
            var form = new Dialog.ServerDebugForm("Freeze");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                FreezeServerDelegate del = new FreezeServerDelegate(FreezeServer);
                var workTask = Task.Run(() => del.Invoke(form.serverId));
            }
        }


        private void BtUnfreezeClick(object sender, EventArgs e)
        {
            var form = new Dialog.ServerDebugForm("Unfreeze");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                UnfreezeServerDelegate del = new UnfreezeServerDelegate(UnfreezeServer);
                var workTask = Task.Run(() => del.Invoke(form.serverId));
            }
        }

        private void BtCrashClick(object sender, EventArgs e)
        {
            var form = new Dialog.ServerDebugForm("Crash");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                CrashServerDelegate del = new CrashServerDelegate(CrashServer);
                var workTask = Task.Run(() => del.Invoke(form.serverId));
            }
        }

        private void BtScriptClick(object sender, EventArgs e)
        {
            OpenFileDialog file_form = new OpenFileDialog();
            file_form.DefaultExt = "txt";
            var result = file_form.ShowDialog();
            if (result == DialogResult.OK)
            {
                RunScriptDelegate del = new RunScriptDelegate(RunScript);
                var workTask = Task.Run(() => del.Invoke(file_form.FileName));
            }
        }

        private void RunScript(String filename)
        {
            WriteToLogger("Executing script " + filename + Environment.NewLine);
            StreamReader script = File.OpenText(filename);

            String line = script.ReadLine();
            while (line != null)
            {
                RunCommand(line);
                line = script.ReadLine();
            }
            script.Close();
        }

        private void RunCommand(String command)
        {
            String[] split_command = command.Split(" ");
            String keyword = split_command[0];
            String[] args = new String[0];
            if (split_command.Length > 1) { args = split_command[1..]; }

            switch (keyword)
            {
                case "ReplicationFactor":
                    ReplicationFactorDelegate rf_del = new ReplicationFactorDelegate(ReplicationFactor);
                    var rf_workTask = Task.Run(() => rf_del.Invoke(args[0]));
                    break;

                case "Server":
                    CreateServerDelegate cs_del = new CreateServerDelegate(CreateServer);
                    var cs_workTask = Task.Run(() => cs_del.Invoke(args));
                    break;

                case "Partition":
                    PartitionDelegate p_del = new PartitionDelegate(Partition);
                    var p_workTask = Task.Run(() => p_del.Invoke(args));
                    break;

                case "Client":
                    CreateClientDelegate cc_del = new CreateClientDelegate(CreateClient);
                    var cc_workTask = Task.Run(() => cc_del.Invoke(args));
                    break;

                case "Status":
                    StatusDelegate s_del = new StatusDelegate(Status);
                    var s_workTask = Task.Run(() => s_del.Invoke());
                    break;

                case "Freeze":
                    FreezeServerDelegate fs_del = new FreezeServerDelegate(FreezeServer);
                    var fs_workTask = Task.Run(() => fs_del.Invoke(args[0]));
                    break;

                case "Unfreeze":
                    UnfreezeServerDelegate us_del = new UnfreezeServerDelegate(UnfreezeServer);
                    var us_workTask = Task.Run(() => us_del.Invoke(args[0]));
                    break;

                case "Crash":
                    CrashServerDelegate cr_del = new CrashServerDelegate(CrashServer);
                    var cr_workTask = Task.Run(() => cr_del.Invoke(args[0]));
                    break;
            }
        }

        private void WriteToLogger(string text)
        {
            if (logger.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteToLogger);
                logger.Invoke(d, new object[] { text });
            }
            else
            {
                logger.AppendText(text);
            }
        }

        // Run init on all servers to start grpc channels between them
        private void InitAllServers()
        {
            if (!_initedServers)
            {
                foreach (String id in _puppetServerClients.Keys)
                {
                    _puppetServerClients[id].InitServer(new InitServerRequest { });
                }
                _initedServers = true;
            }
        }
    }
}
