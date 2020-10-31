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
            this.status_button = new System.Windows.Forms.Button();
            this.client_button = new System.Windows.Forms.Button();
            this.partition_button = new System.Windows.Forms.Button();
            this.server_button = new System.Windows.Forms.Button();
            this.replication_button = new System.Windows.Forms.Button();
            this.button_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // crash_button
            // 
            this.crash_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.crash_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.crash_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crash_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.crash_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.crash_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.crash_button.Location = new System.Drawing.Point(829, 3);
            this.crash_button.Name = "crash_button";
            this.crash_button.Size = new System.Drawing.Size(117, 97);
            this.crash_button.TabIndex = 0;
            this.crash_button.Text = "Crash";
            this.crash_button.UseVisualStyleBackColor = false;
            this.crash_button.Click += new System.EventHandler(this.BtCrashClick);
            // 
            // logger
            // 
            this.logger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.logger.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.logger.Location = new System.Drawing.Point(15, 12);
            this.logger.Multiline = true;
            this.logger.Name = "logger";
            this.logger.Size = new System.Drawing.Size(584, 333);
            this.logger.TabIndex = 1;
            // 
            // unfreeze_button
            // 
            this.unfreeze_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.unfreeze_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.unfreeze_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unfreeze_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.unfreeze_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.unfreeze_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.unfreeze_button.Location = new System.Drawing.Point(711, 3);
            this.unfreeze_button.Name = "unfreeze_button";
            this.unfreeze_button.Size = new System.Drawing.Size(112, 97);
            this.unfreeze_button.TabIndex = 0;
            this.unfreeze_button.Text = "Unfreeze";
            this.unfreeze_button.UseVisualStyleBackColor = false;
            this.unfreeze_button.Click += new System.EventHandler(this.BtUnfreezeClick);
            // 
            // freeze_button
            // 
            this.freeze_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.freeze_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.freeze_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.freeze_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.freeze_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.freeze_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.freeze_button.Location = new System.Drawing.Point(593, 3);
            this.freeze_button.Name = "freeze_button";
            this.freeze_button.Size = new System.Drawing.Size(112, 97);
            this.freeze_button.TabIndex = 0;
            this.freeze_button.Text = "Freeze";
            this.freeze_button.UseVisualStyleBackColor = false;
            this.freeze_button.Click += new System.EventHandler(this.BtFreezeClick);
            // 
            // button_panel
            // 
            this.button_panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_panel.BackColor = System.Drawing.Color.GhostWhite;
            this.button_panel.ColumnCount = 8;
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.button_panel.Controls.Add(this.status_button, 4, 0);
            this.button_panel.Controls.Add(this.client_button, 3, 0);
            this.button_panel.Controls.Add(this.partition_button, 2, 0);
            this.button_panel.Controls.Add(this.server_button, 1, 0);
            this.button_panel.Controls.Add(this.replication_button, 0, 0);
            this.button_panel.Controls.Add(this.crash_button, 7, 0);
            this.button_panel.Controls.Add(this.freeze_button, 5, 0);
            this.button_panel.Controls.Add(this.unfreeze_button, 6, 0);
            this.button_panel.Location = new System.Drawing.Point(12, 375);
            this.button_panel.Name = "button_panel";
            this.button_panel.RowCount = 1;
            this.button_panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.button_panel.Size = new System.Drawing.Size(949, 103);
            this.button_panel.TabIndex = 2;
            // 
            // status_button
            // 
            this.status_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.status_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.status_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.status_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.status_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.status_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.status_button.Location = new System.Drawing.Point(475, 3);
            this.status_button.Name = "status_button";
            this.status_button.Size = new System.Drawing.Size(112, 97);
            this.status_button.TabIndex = 0;
            this.status_button.Text = "Status";
            this.status_button.UseVisualStyleBackColor = false;
            this.status_button.Click += new System.EventHandler(this.BtStatusClick);
            // 
            // client_button
            // 
            this.client_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.client_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.client_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.client_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.client_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.client_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.client_button.Location = new System.Drawing.Point(357, 3);
            this.client_button.Name = "client_button";
            this.client_button.Size = new System.Drawing.Size(112, 97);
            this.client_button.TabIndex = 0;
            this.client_button.Text = "New Client";
            this.client_button.UseVisualStyleBackColor = false;
            this.client_button.Click += new System.EventHandler(this.BtClientClick);
            // 
            // partition_button
            // 
            this.partition_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.partition_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.partition_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.partition_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.partition_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.partition_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.partition_button.Location = new System.Drawing.Point(239, 3);
            this.partition_button.Name = "partition_button";
            this.partition_button.Size = new System.Drawing.Size(112, 97);
            this.partition_button.TabIndex = 0;
            this.partition_button.Text = "Partition";
            this.partition_button.UseVisualStyleBackColor = false;
            this.partition_button.Click += new System.EventHandler(this.BtPartitionClick);
            // 
            // server_button
            // 
            this.server_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.server_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.server_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.server_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.server_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.server_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.server_button.Location = new System.Drawing.Point(121, 3);
            this.server_button.Name = "server_button";
            this.server_button.Size = new System.Drawing.Size(112, 97);
            this.server_button.TabIndex = 0;
            this.server_button.Text = "New Server";
            this.server_button.UseVisualStyleBackColor = false;
            this.server_button.Click += new System.EventHandler(this.BtServerClick);
            // 
            // replication_button
            // 
            this.replication_button.BackColor = System.Drawing.Color.DarkSlateGray;
            this.replication_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.replication_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replication_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replication_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.replication_button.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.replication_button.Location = new System.Drawing.Point(3, 3);
            this.replication_button.Name = "replication_button";
            this.replication_button.Size = new System.Drawing.Size(112, 97);
            this.replication_button.TabIndex = 0;
            this.replication_button.Text = "Rep. Factor";
            this.replication_button.UseVisualStyleBackColor = false;
            this.replication_button.Click += new System.EventHandler(this.BtReplicationClick);
            // 
            // PuppetMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(973, 500);
            this.Controls.Add(this.button_panel);
            this.Controls.Add(this.logger);
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
    }
}

