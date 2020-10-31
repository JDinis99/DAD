using System.Runtime.CompilerServices;

namespace PuppetMaster.Dialog
{
    partial class ServerDebugForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
#region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.id_text_box = new System.Windows.Forms.TextBox();
            this.server_id_label = new System.Windows.Forms.Label();
            this.action_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.error_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // id_text_box
            // 
            this.id_text_box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.id_text_box.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.id_text_box.Location = new System.Drawing.Point(225, 66);
            this.id_text_box.Name = "id_text_box";
            this.id_text_box.Size = new System.Drawing.Size(248, 52);
            this.id_text_box.TabIndex = 0;
            // 
            // server_id_label
            // 
            this.server_id_label.AutoSize = true;
            this.server_id_label.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.server_id_label.Location = new System.Drawing.Point(53, 69);
            this.server_id_label.Name = "server_id_label";
            this.server_id_label.Size = new System.Drawing.Size(163, 46);
            this.server_id_label.TabIndex = 1;
            this.server_id_label.Text = "Server ID:";
            // 
            // action_button
            // 
            this.action_button.Location = new System.Drawing.Point(184, 196);
            this.action_button.Name = "action_button";
            this.action_button.Size = new System.Drawing.Size(136, 59);
            this.action_button.TabIndex = 2;
            this.action_button.Text = "button1";
            this.action_button.UseVisualStyleBackColor = true;
            this.action_button.Click += new System.EventHandler(this.ActionButtonClick);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(351, 196);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(134, 59);
            this.cancel_button.TabIndex = 3;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // error_label
            // 
            this.error_label.AutoSize = true;
            this.error_label.ForeColor = System.Drawing.Color.Crimson;
            this.error_label.Location = new System.Drawing.Point(225, 121);
            this.error_label.Name = "error_label";
            this.error_label.Size = new System.Drawing.Size(193, 20);
            this.error_label.TabIndex = 4;
            this.error_label.Text = "Please fill this with server ID";
            this.error_label.Visible = false;
            // 
            // ServerDebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_button;
            this.ClientSize = new System.Drawing.Size(524, 291);
            this.Controls.Add(this.error_label);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.action_button);
            this.Controls.Add(this.server_id_label);
            this.Controls.Add(this.id_text_box);
            this.Name = "ServerDebugForm";
            this.Text = "ServerDebugForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox id_text_box;
        private System.Windows.Forms.Label server_id_label;
        private System.Windows.Forms.Button action_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.Label error_label;
    }
}