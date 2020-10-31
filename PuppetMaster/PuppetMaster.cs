using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using GigaStore;
using Grpc.Net.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class PuppetMaster : Form
    {
        private int no_servers = 5;
        private GrpcChannel[] _channels; 
        private GigaStore.PuppetMaster.PuppetMasterClient[] _puppetServerClients;
        public PuppetMaster()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _channels = new GrpcChannel[no_servers + 1];
            _puppetServerClients = new GigaStore.PuppetMaster.PuppetMasterClient[no_servers + 1];
            for (int i = 1; i<=no_servers; i++)
            {
                _channels[i] = GrpcChannel.ForAddress("https://localhost:500" + i);
                _puppetServerClients[i] = new GigaStore.PuppetMaster.PuppetMasterClient(_channels[i]);
            }
        }

        private void BtReplicationClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("replication");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string rep_factor = form.item1;
                logger.AppendText("Configuring system to replicate partitions on " + rep_factor + " servers...");
                logger.AppendText(Environment.NewLine);
            }
        }

        private void BtServerClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("server");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string server_id = form.item1;
                string server_url = form.item2;
                string min_delay = form.item3;
                string max_delay = form.item4;
                logger.AppendText("Creating new server with id " + server_id + " on URL " + server_url + " with delay between " + min_delay + "ms and " + max_delay + "ms...");
                logger.AppendText(Environment.NewLine);
            }
        }

        private void BtPartitionClick(object sender, EventArgs e)
        {
            var form = new Dialog.NodeActionForm("partition");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string rep_count = form.item1;
                string partition_name = form.item2;
                string list_servers = form.item3;
                logger.AppendText("Storing " + rep_count + " replicas of partition " + partition_name + " on servers " + list_servers + "...");
                logger.AppendText(Environment.NewLine);
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
                logger.AppendText("Creating new client with username " + username + " on URL " + client_url + " to run script " + script_file + "...");
                logger.AppendText(Environment.NewLine);
            }
        }

        private void BtStatusClick(object sender, EventArgs e)
        {
            var reply = _puppetServerClients[1].Status(new StatusRequest { });
            logger.AppendText("Asking nodes to print their statuses...");
            logger.AppendText(Environment.NewLine);
            // ONLY FOR DEMO - NEED TO FIX
            logger.AppendText(Environment.NewLine);
            logger.AppendText(reply.Ack);
        }

        private void BtFreezeClick(object sender, EventArgs e)
        {
            var form = new Dialog.ServerDebugForm("Freeze");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string serverId = form.serverId;
                logger.AppendText("Freezing server with id " + serverId + "...");
                logger.AppendText(Environment.NewLine);
            }
        }


        private void BtUnfreezeClick(object sender, EventArgs e)
        {
            var form = new Dialog.ServerDebugForm("Unfreeze");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string serverId = form.serverId;
                logger.AppendText("Unfreezing server with id " + serverId + "...");
                logger.AppendText(Environment.NewLine);
            }
        }

        private void BtCrashClick(object sender, EventArgs e)
        {
            var form = new Dialog.ServerDebugForm("Crash");
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                string serverId = form.serverId;
                logger.AppendText("Crashing server with id " + serverId + "...");
                logger.AppendText(Environment.NewLine);
                var reply = _puppetServerClients[Int32.Parse(serverId)].CrashServer(new CrashRequest { });
                if (reply.Ack.Equals("Success"))
                {
                    logger.AppendText("Server with id " + serverId + " successfully crashed");
                    logger.AppendText(Environment.NewLine);
                }
                else if (reply.Ack.Equals("Unsuccess"))
                {
                    logger.AppendText("Server with id " + serverId + " couldn't crash");
                    logger.AppendText(Environment.NewLine);
                }
            }
        }
    }
}
