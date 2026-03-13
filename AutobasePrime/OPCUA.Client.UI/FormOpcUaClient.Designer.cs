namespace OPCUA.Client.UI
{
    partial class FormOpcUaClient
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOpcUaClient));
            this.gridRealtime = new System.Windows.Forms.DataGridView();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCertificateSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCreateCert = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.startMinimizedToTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.treeSources = new System.Windows.Forms.TreeView();
            this.listLog = new System.Windows.Forms.ListView();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitBottom = new System.Windows.Forms.SplitContainer();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridRealtime)).BeginInit();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitBottom)).BeginInit();
            this.splitBottom.Panel1.SuspendLayout();
            this.splitBottom.Panel2.SuspendLayout();
            this.splitBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridRealtime
            // 
            this.gridRealtime.AllowUserToAddRows = false;
            this.gridRealtime.AllowUserToDeleteRows = false;
            this.gridRealtime.AllowUserToResizeRows = false;
            this.gridRealtime.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridRealtime.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridRealtime.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRealtime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRealtime.GridColor = System.Drawing.SystemColors.Window;
            this.gridRealtime.Location = new System.Drawing.Point(0, 0);
            this.gridRealtime.Name = "gridRealtime";
            this.gridRealtime.RowHeadersVisible = false;
            this.gridRealtime.RowHeadersWidth = 62;
            this.gridRealtime.RowTemplate.Height = 23;
            this.gridRealtime.Size = new System.Drawing.Size(641, 417);
            this.gridRealtime.TabIndex = 4;
            this.gridRealtime.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridRealtime_CellMouseDown);
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.serverToolStripMenuItem,
            this.configToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menu.Size = new System.Drawing.Size(965, 24);
            this.menu.TabIndex = 6;
            this.menu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClose});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // menuClose
            // 
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(103, 22);
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aToolStripMenuItem,
            this.deleteServerToolStripMenuItem,
            this.modifyServerToolStripMenuItem,
            this.toolStripSeparator1,
            this.connectToolStripMenuItem});
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(52, 22);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.aToolStripMenuItem.Text = "Add Server";
            this.aToolStripMenuItem.Click += new System.EventHandler(this.aToolStripMenuItem_Click);
            // 
            // deleteServerToolStripMenuItem
            // 
            this.deleteServerToolStripMenuItem.Name = "deleteServerToolStripMenuItem";
            this.deleteServerToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.deleteServerToolStripMenuItem.Text = "Delete Server";
            // 
            // modifyServerToolStripMenuItem
            // 
            this.modifyServerToolStripMenuItem.Name = "modifyServerToolStripMenuItem";
            this.modifyServerToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.modifyServerToolStripMenuItem.Text = "Modify Server";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(146, 6);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionOptionToolStripMenuItem,
            this.menuCertificateSettings,
            this.menuCreateCert,
            this.toolStripSeparator2,
            this.startMinimizedToTrayToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(55, 22);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // connectionOptionToolStripMenuItem
            // 
            this.connectionOptionToolStripMenuItem.Name = "connectionOptionToolStripMenuItem";
            this.connectionOptionToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.connectionOptionToolStripMenuItem.Text = "Connection Options";
            this.connectionOptionToolStripMenuItem.Click += new System.EventHandler(this.connectionOptionToolStripMenuItem_Click);
            // 
            // menuCertificateSettings
            // 
            this.menuCertificateSettings.Name = "menuCertificateSettings";
            this.menuCertificateSettings.Size = new System.Drawing.Size(201, 22);
            this.menuCertificateSettings.Text = "Certificate Settings";
            this.menuCertificateSettings.Click += new System.EventHandler(this.menuCertificateSettings_Click);
            // 
            // menuCreateCert
            // 
            this.menuCreateCert.Name = "menuCreateCert";
            this.menuCreateCert.Size = new System.Drawing.Size(201, 22);
            this.menuCreateCert.Text = "Create Certificate";
            this.menuCreateCert.Click += new System.EventHandler(this.menuCreateCert_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(198, 6);
            // 
            // startMinimizedToTrayToolStripMenuItem
            // 
            this.startMinimizedToTrayToolStripMenuItem.Name = "startMinimizedToTrayToolStripMenuItem";
            this.startMinimizedToTrayToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.startMinimizedToTrayToolStripMenuItem.Text = "Start Minimized to Tray";
            this.startMinimizedToTrayToolStripMenuItem.Click += new System.EventHandler(this.startMinimizedToTrayToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(58, 22);
            this.toolStripMenuItem1.Text = "Refresh";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // treeSources
            // 
            this.treeSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeSources.HideSelection = false;
            this.treeSources.Location = new System.Drawing.Point(0, 0);
            this.treeSources.Name = "treeSources";
            this.treeSources.Size = new System.Drawing.Size(320, 417);
            this.treeSources.TabIndex = 7;
            this.treeSources.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSources_AfterSelect);
            this.treeSources.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeSources_NodeMouseClick);
            // 
            // listLog
            // 
            this.listLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLog.FullRowSelect = true;
            this.listLog.HideSelection = false;
            this.listLog.Location = new System.Drawing.Point(0, 0);
            this.listLog.Name = "listLog";
            this.listLog.Size = new System.Drawing.Size(965, 170);
            this.listLog.TabIndex = 8;
            this.listLog.UseCompatibleStateImageBehavior = false;
            this.listLog.View = System.Windows.Forms.View.Details;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.treeSources);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.gridRealtime);
            this.splitMain.Size = new System.Drawing.Size(965, 417);
            this.splitMain.SplitterDistance = 320;
            this.splitMain.TabIndex = 9;
            // 
            // splitBottom
            // 
            this.splitBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBottom.Location = new System.Drawing.Point(0, 24);
            this.splitBottom.Name = "splitBottom";
            this.splitBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitBottom.Panel1
            // 
            this.splitBottom.Panel1.Controls.Add(this.splitMain);
            // 
            // splitBottom.Panel2
            // 
            this.splitBottom.Panel2.Controls.Add(this.listLog);
            this.splitBottom.Size = new System.Drawing.Size(965, 591);
            this.splitBottom.SplitterDistance = 417;
            this.splitBottom.TabIndex = 10;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Autobase OPCUA Client";
            this.notifyIcon1.Visible = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormOpcUaClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 615);
            this.Controls.Add(this.splitBottom);
            this.Controls.Add(this.menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.MinimumSize = new System.Drawing.Size(598, 394);
            this.Name = "FormOpcUaClient";
            this.Text = "Autobase OPCUA Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOpcUaClient_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOpcUaClient_FormClosed);
            this.Load += new System.EventHandler(this.FormOpcUaClient_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridRealtime)).EndInit();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitBottom.Panel1.ResumeLayout(false);
            this.splitBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBottom)).EndInit();
            this.splitBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView gridRealtime;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modifyServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuCertificateSettings;
        private System.Windows.Forms.TreeView treeSources;
        private System.Windows.Forms.ListView listLog;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitBottom;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.ToolStripMenuItem menuCreateCert;
        private System.Windows.Forms.ToolStripMenuItem startMinimizedToTrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer timer1;
    }
}