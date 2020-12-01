namespace PuppetMaster
{
    partial class PuppetMaster
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PuppetMaster));
            this.crash_button = new System.Windows.Forms.Button();
            this.logger = new System.Windows.Forms.TextBox();
            this.unfreeze_button = new System.Windows.Forms.Button();
            this.freeze_button = new System.Windows.Forms.Button();
            this.button_panel = new System.Windows.Forms.TableLayoutPanel();
            this.script_button = new System.Windows.Forms.Button();
            this.replication_button = new System.Windows.Forms.Button();
            this.status_button = new System.Windows.Forms.Button();
            this.server_button = new System.Windows.Forms.Button();
            this.partition_button = new System.Windows.Forms.Button();
            this.client_button = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.button_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // crash_button
            // 
            this.crash_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.crash_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.crash_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crash_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.crash_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.crash_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.crash_button.Image = ((System.Drawing.Image)(resources.GetObject("crash_button.Image")));
            this.crash_button.Location = new System.Drawing.Point(147, 384);
            this.crash_button.Margin = new System.Windows.Forms.Padding(0);
            this.crash_button.Name = "crash_button";
            this.crash_button.Size = new System.Drawing.Size(147, 193);
            this.crash_button.TabIndex = 0;
            this.crash_button.Text = "Crash";
            this.crash_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.crash_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.crash_button.UseVisualStyleBackColor = false;
            this.crash_button.Click += new System.EventHandler(this.BtCrashClick);
            // 
            // logger
            // 
            this.logger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.logger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.logger.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logger.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.logger.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.logger.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.logger.Location = new System.Drawing.Point(12, 113);
            this.logger.Multiline = true;
            this.logger.Name = "logger";
            this.logger.ReadOnly = true;
            this.logger.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logger.Size = new System.Drawing.Size(609, 450);
            this.logger.TabIndex = 1;
            // 
            // unfreeze_button
            // 
            this.unfreeze_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.unfreeze_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.unfreeze_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unfreeze_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.unfreeze_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.unfreeze_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.unfreeze_button.Image = ((System.Drawing.Image)(resources.GetObject("unfreeze_button.Image")));
            this.unfreeze_button.Location = new System.Drawing.Point(0, 384);
            this.unfreeze_button.Margin = new System.Windows.Forms.Padding(0);
            this.unfreeze_button.Name = "unfreeze_button";
            this.unfreeze_button.Size = new System.Drawing.Size(147, 193);
            this.unfreeze_button.TabIndex = 0;
            this.unfreeze_button.Text = "Unfreeze";
            this.unfreeze_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.unfreeze_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.unfreeze_button.UseVisualStyleBackColor = false;
            this.unfreeze_button.Click += new System.EventHandler(this.BtUnfreezeClick);
            // 
            // freeze_button
            // 
            this.freeze_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.freeze_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.freeze_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.freeze_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.freeze_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.freeze_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.freeze_button.Image = ((System.Drawing.Image)(resources.GetObject("freeze_button.Image")));
            this.freeze_button.Location = new System.Drawing.Point(294, 192);
            this.freeze_button.Margin = new System.Windows.Forms.Padding(0);
            this.freeze_button.Name = "freeze_button";
            this.freeze_button.Size = new System.Drawing.Size(149, 192);
            this.freeze_button.TabIndex = 0;
            this.freeze_button.Text = "Freeze";
            this.freeze_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.freeze_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.freeze_button.UseVisualStyleBackColor = false;
            this.freeze_button.Click += new System.EventHandler(this.BtFreezeClick);
            // 
            // button_panel
            // 
            this.button_panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_panel.BackColor = System.Drawing.Color.GhostWhite;
            this.button_panel.ColumnCount = 3;
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.button_panel.Controls.Add(this.script_button, 2, 2);
            this.button_panel.Controls.Add(this.crash_button, 1, 2);
            this.button_panel.Controls.Add(this.replication_button, 0, 0);
            this.button_panel.Controls.Add(this.unfreeze_button, 0, 2);
            this.button_panel.Controls.Add(this.status_button, 1, 1);
            this.button_panel.Controls.Add(this.freeze_button, 2, 1);
            this.button_panel.Controls.Add(this.server_button, 1, 0);
            this.button_panel.Controls.Add(this.partition_button, 2, 0);
            this.button_panel.Controls.Add(this.client_button, 0, 1);
            this.button_panel.Location = new System.Drawing.Point(636, 0);
            this.button_panel.Name = "button_panel";
            this.button_panel.RowCount = 3;
            this.button_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.button_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.button_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.button_panel.Size = new System.Drawing.Size(443, 577);
            this.button_panel.TabIndex = 2;
            // 
            // script_button
            // 
            this.script_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.script_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.script_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.script_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.script_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.script_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.script_button.Image = ((System.Drawing.Image)(resources.GetObject("script_button.Image")));
            this.script_button.Location = new System.Drawing.Point(294, 384);
            this.script_button.Margin = new System.Windows.Forms.Padding(0);
            this.script_button.Name = "script_button";
            this.script_button.Size = new System.Drawing.Size(149, 193);
            this.script_button.TabIndex = 0;
            this.script_button.Text = "Run Script";
            this.script_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.script_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.script_button.UseVisualStyleBackColor = false;
            this.script_button.Click += new System.EventHandler(this.BtScriptClick);
            // 
            // replication_button
            // 
            this.replication_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.replication_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.replication_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replication_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replication_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.replication_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.replication_button.Image = ((System.Drawing.Image)(resources.GetObject("replication_button.Image")));
            this.replication_button.Location = new System.Drawing.Point(0, 0);
            this.replication_button.Margin = new System.Windows.Forms.Padding(0);
            this.replication_button.Name = "replication_button";
            this.replication_button.Size = new System.Drawing.Size(147, 192);
            this.replication_button.TabIndex = 0;
            this.replication_button.Text = "Rep. Factor";
            this.replication_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.replication_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.replication_button.UseVisualStyleBackColor = false;
            this.replication_button.Click += new System.EventHandler(this.BtReplicationClick);
            // 
            // status_button
            // 
            this.status_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.status_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.status_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.status_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.status_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.status_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.status_button.Image = ((System.Drawing.Image)(resources.GetObject("status_button.Image")));
            this.status_button.Location = new System.Drawing.Point(147, 192);
            this.status_button.Margin = new System.Windows.Forms.Padding(0);
            this.status_button.Name = "status_button";
            this.status_button.Size = new System.Drawing.Size(147, 192);
            this.status_button.TabIndex = 0;
            this.status_button.Text = "Status";
            this.status_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.status_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.status_button.UseVisualStyleBackColor = false;
            this.status_button.Click += new System.EventHandler(this.BtStatusClick);
            // 
            // server_button
            // 
            this.server_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.server_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.server_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.server_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.server_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.server_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.server_button.Image = ((System.Drawing.Image)(resources.GetObject("server_button.Image")));
            this.server_button.Location = new System.Drawing.Point(147, 0);
            this.server_button.Margin = new System.Windows.Forms.Padding(0);
            this.server_button.Name = "server_button";
            this.server_button.Size = new System.Drawing.Size(147, 192);
            this.server_button.TabIndex = 0;
            this.server_button.Text = "New Server";
            this.server_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.server_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.server_button.UseVisualStyleBackColor = false;
            this.server_button.Click += new System.EventHandler(this.BtServerClick);
            // 
            // partition_button
            // 
            this.partition_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.partition_button.Cursor = System.Windows.Forms.Cursors.Default;
            this.partition_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.partition_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.partition_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.partition_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.partition_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.partition_button.Image = ((System.Drawing.Image)(resources.GetObject("partition_button.Image")));
            this.partition_button.Location = new System.Drawing.Point(294, 0);
            this.partition_button.Margin = new System.Windows.Forms.Padding(0);
            this.partition_button.Name = "partition_button";
            this.partition_button.Size = new System.Drawing.Size(149, 192);
            this.partition_button.TabIndex = 0;
            this.partition_button.Text = "Partition";
            this.partition_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.partition_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.partition_button.UseVisualStyleBackColor = false;
            this.partition_button.Click += new System.EventHandler(this.BtPartitionClick);
            // 
            // client_button
            // 
            this.client_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(167)))), ((int)(((byte)(146)))));
            this.client_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.client_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.client_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.client_button.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.client_button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.client_button.Image = ((System.Drawing.Image)(resources.GetObject("client_button.Image")));
            this.client_button.Location = new System.Drawing.Point(0, 192);
            this.client_button.Margin = new System.Windows.Forms.Padding(0);
            this.client_button.Name = "client_button";
            this.client_button.Size = new System.Drawing.Size(147, 192);
            this.client_button.TabIndex = 0;
            this.client_button.Text = "New Client";
            this.client_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.client_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.client_button.UseVisualStyleBackColor = false;
            this.client_button.Click += new System.EventHandler(this.BtClientClick);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.title.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.title.Font = new System.Drawing.Font("Segoe UI Semibold", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(64)))), ((int)(((byte)(72)))));
            this.title.Location = new System.Drawing.Point(128, 22);
            this.title.Margin = new System.Windows.Forms.Padding(0);
            this.title.Name = "title";
            this.title.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.title.Size = new System.Drawing.Size(379, 75);
            this.title.TabIndex = 3;
            this.title.Text = "PuppetMaster";
            // 
            // PuppetMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(217)))), ((int)(((byte)(202)))));
            this.ClientSize = new System.Drawing.Size(1079, 575);
            this.Controls.Add(this.title);
            this.Controls.Add(this.button_panel);
            this.Controls.Add(this.logger);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PuppetMaster";
            this.Text = "PuppetMaster";
            this.button_panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button crash_button;
        private System.Windows.Forms.TextBox logger;
        private System.Windows.Forms.Button unfreeze_button;
        private System.Windows.Forms.Button freeze_button;
        private System.Windows.Forms.TableLayoutPanel button_panel;
        private System.Windows.Forms.Button status_button;
        private System.Windows.Forms.Button client_button;
        private System.Windows.Forms.Button partition_button;
        private System.Windows.Forms.Button server_button;
        private System.Windows.Forms.Button replication_button;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Button script_button;
    }
}

