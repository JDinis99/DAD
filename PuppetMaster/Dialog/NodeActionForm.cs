using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PuppetMaster.Dialog
{
    public partial class NodeActionForm : Form
    {
        private string _type;
        public string item1;
        public string item2;
        public string item3;
        public string item4;
        public NodeActionForm(string type)
        {
            InitializeComponent();
            _type = type;
            ConfigureForm(type);
        }

        private void ConfigureForm(string type)
        {
            if (type.Equals("replication")) {
                this.Text = "Replication Factor";
                label1.Text = "Replication Factor";
                label2.Visible = false;
                textBox2.Visible = false;
                label3.Visible = false;
                textBox3.Visible = false;
                label4.Visible = false;
                textBox4.Visible = false;
                error_label1.Text = "Please fill with number of servers";
            }

            else if (type.Equals("server"))
            {
                this.Text = "New Server";
                label1.Text = "Server ID";
                label2.Text = "Server URL";
                label3.Text = "Min delay";
                label4.Text = "Max delay";
                error_label1.Text = "Please fill with server ID";
                error_label2.Text = "Please fill with server URL";
                error_label3.Text = "Please fill with minimum delay";
                error_label4.Text = "Please fill with maximum delay";
            }

            else if (type.Equals("partition"))
            {
                this.Text = "Partition Configuration";
                label1.Text = "Number of replicas";
                label2.Text = "Partition name";
                label3.Text = "List of server Ids";
                label4.Visible = false;
                textBox4.Visible = false;
                error_label1.Text = "Please fill with number of replicas";
                error_label2.Text = "Please fill with partition name";
                error_label3.Text = "Please fill with servers separated by space";
            }

            else if (type.Equals("client"))
            {
                this.Text = "New Client";
                label1.Text = "Client username";
                label2.Text = "Client URL";
                label3.Text = "Script file";
                label4.Visible = false;
                textBox4.Visible = false;
                error_label1.Text = "Please fill with client username";
                error_label2.Text = "Please fill with client URL";
                error_label3.Text = "Please fill with script filename";
            }
        }

        private void action_button_Click(object sender, EventArgs e)
        {
            var misconfigured = false;

            if (label1.Visible)
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    misconfigured = true;
                    error_label1.Visible = true;
                }
                else
                {
                    if (error_label1.Visible == true) error_label1.Visible = false;
                }
            }

            if (label2.Visible)
            {
                if (String.IsNullOrEmpty(textBox2.Text))
                {
                    misconfigured = true;
                    error_label2.Visible = true;
                }
                else
                {
                    if (error_label2.Visible == true) error_label2.Visible = false;
                }
            }

            if (label3.Visible)
            {
                if (String.IsNullOrEmpty(textBox3.Text))
                {
                    misconfigured = true;
                    error_label3.Visible = true;
                }
                else
                {
                    if (error_label3.Visible == true) error_label3.Visible = false;
                }
            }

            if (label4.Visible)
            {
                if (String.IsNullOrEmpty(textBox4.Text))
                {
                    misconfigured = true;
                    error_label4.Visible = true;
                }
                else
                {
                    if (error_label4.Visible == true) error_label4.Visible = false;
                }
            }

            if (!misconfigured)
            {
                if (textBox1.Visible) item1 = textBox1.Text;
                if (textBox2.Visible) item2 = textBox2.Text;
                if (textBox3.Visible) item3 = textBox3.Text;
                if (textBox4.Visible) item4 = textBox4.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
