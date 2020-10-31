using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PuppetMaster.Dialog
{
    public partial class ServerDebugForm : Form
    {
        public string serverId;
        static public string errorMsg = "Please fill this with server ID";
        public ServerDebugForm(string type)
        {
            InitializeComponent();
            this.Text = type + " server";
            action_button.Text = type;
        }

        private void ActionButtonClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(id_text_box.Text))
            {
                this.serverId = id_text_box.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                error_label.Visible = true;
            }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
