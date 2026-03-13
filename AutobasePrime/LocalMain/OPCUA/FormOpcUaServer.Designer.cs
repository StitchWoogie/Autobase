namespace LocalMain.OPCUA
{
    partial class FormOpcUaServer
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
            if (disposing)
            {
                OpcUaServerLogBridge.Detach(AddLog);
                components?.Dispose();
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSetUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCertificateSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCreateCert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUserSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.labelPort = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.labelLog = new System.Windows.Forms.Label();
            this.checkBoxSha256 = new System.Windows.Forms.CheckBox();
            this.comboBoxSecurity = new System.Windows.Forms.ComboBox();
            this.checkBoxNone = new System.Windows.Forms.CheckBox();
            this.groupBoxSecurity = new System.Windows.Forms.GroupBox();
            this.groupBoxUser = new System.Windows.Forms.GroupBox();
            this.chkUsernameToken = new System.Windows.Forms.CheckBox();
            this.chkCertificateToken = new System.Windows.Forms.CheckBox();
            this.chkAnonymousToken = new System.Windows.Forms.CheckBox();
            this.groupBoxServer = new System.Windows.Forms.GroupBox();
            this.buttonReloadIP = new System.Windows.Forms.Button();
            this.comboBoxIP = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelIp = new System.Windows.Forms.Label();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.chkServerEnable = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.groupBoxSecurity.SuspendLayout();
            this.groupBoxUser.SuspendLayout();
            this.groupBoxServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(17, 22);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(53, 12);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "lblStatus";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(68, 113);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 36);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(163, 113);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 36);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverToolStripMenuItem,
            this.configToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(684, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSetUpToolStripMenuItem,
            this.menuCertificateSettings,
            this.menuCreateCert,
            this.menuUserSettings});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // autoSetUpToolStripMenuItem
            // 
            this.autoSetUpToolStripMenuItem.Name = "autoSetUpToolStripMenuItem";
            this.autoSetUpToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.autoSetUpToolStripMenuItem.Text = "Auto Settings";
            this.autoSetUpToolStripMenuItem.Click += new System.EventHandler(this.autoSetUpToolStripMenuItem_Click);
            // 
            // menuCertificateSettings
            // 
            this.menuCertificateSettings.Name = "menuCertificateSettings";
            this.menuCertificateSettings.Size = new System.Drawing.Size(175, 22);
            this.menuCertificateSettings.Text = "Certificate Settings";
            this.menuCertificateSettings.Click += new System.EventHandler(this.setupPKIToolStripMenuItem_Click);
            // 
            // menuCreateCert
            // 
            this.menuCreateCert.Name = "menuCreateCert";
            this.menuCreateCert.Size = new System.Drawing.Size(175, 22);
            this.menuCreateCert.Text = "Create Certificate";
            this.menuCreateCert.Click += new System.EventHandler(this.menuCreateCert_Click);
            // 
            // menuUserSettings
            // 
            this.menuUserSettings.Name = "menuUserSettings";
            this.menuUserSettings.Size = new System.Drawing.Size(175, 22);
            this.menuUserSettings.Text = "User Settings";
            this.menuUserSettings.Click += new System.EventHandler(this.menuUserSettings_Click);
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(17, 53);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(27, 12);
            this.labelPort.TabIndex = 5;
            this.labelPort.Text = "Port";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(85, 47);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(80, 21);
            this.numericUpDownPort.TabIndex = 6;
            this.numericUpDownPort.Value = new decimal(new int[] {
            43344,
            0,
            0,
            0});
            // 
            // labelLog
            // 
            this.labelLog.AutoSize = true;
            this.labelLog.Location = new System.Drawing.Point(10, 241);
            this.labelLog.Name = "labelLog";
            this.labelLog.Size = new System.Drawing.Size(26, 12);
            this.labelLog.TabIndex = 8;
            this.labelLog.Text = "Log";
            // 
            // checkBoxSha256
            // 
            this.checkBoxSha256.AutoSize = true;
            this.checkBoxSha256.Location = new System.Drawing.Point(18, 67);
            this.checkBoxSha256.Name = "checkBoxSha256";
            this.checkBoxSha256.Size = new System.Drawing.Size(114, 16);
            this.checkBoxSha256.TabIndex = 10;
            this.checkBoxSha256.Text = "Basic256Sha256";
            this.checkBoxSha256.UseVisualStyleBackColor = true;
            this.checkBoxSha256.CheckedChanged += new System.EventHandler(this.checkBoxSha256_CheckedChanged);
            // 
            // comboBoxSecurity
            // 
            this.comboBoxSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSecurity.FormattingEnabled = true;
            this.comboBoxSecurity.Items.AddRange(new object[] {
            "Sign And Encrypt"});
            this.comboBoxSecurity.Location = new System.Drawing.Point(18, 97);
            this.comboBoxSecurity.Name = "comboBoxSecurity";
            this.comboBoxSecurity.Size = new System.Drawing.Size(141, 20);
            this.comboBoxSecurity.TabIndex = 11;
            // 
            // checkBoxNone
            // 
            this.checkBoxNone.AutoSize = true;
            this.checkBoxNone.Location = new System.Drawing.Point(18, 34);
            this.checkBoxNone.Name = "checkBoxNone";
            this.checkBoxNone.Size = new System.Drawing.Size(54, 16);
            this.checkBoxNone.TabIndex = 10;
            this.checkBoxNone.Text = "None";
            this.checkBoxNone.UseVisualStyleBackColor = true;
            this.checkBoxNone.CheckedChanged += new System.EventHandler(this.checkBoxSha256_CheckedChanged);
            // 
            // groupBoxSecurity
            // 
            this.groupBoxSecurity.Controls.Add(this.groupBoxUser);
            this.groupBoxSecurity.Controls.Add(this.comboBoxSecurity);
            this.groupBoxSecurity.Controls.Add(this.checkBoxNone);
            this.groupBoxSecurity.Controls.Add(this.checkBoxSha256);
            this.groupBoxSecurity.Location = new System.Drawing.Point(326, 59);
            this.groupBoxSecurity.Name = "groupBoxSecurity";
            this.groupBoxSecurity.Size = new System.Drawing.Size(334, 163);
            this.groupBoxSecurity.TabIndex = 12;
            this.groupBoxSecurity.TabStop = false;
            this.groupBoxSecurity.Text = "Security Policy";
            // 
            // groupBoxUser
            // 
            this.groupBoxUser.Controls.Add(this.chkUsernameToken);
            this.groupBoxUser.Controls.Add(this.chkCertificateToken);
            this.groupBoxUser.Controls.Add(this.chkAnonymousToken);
            this.groupBoxUser.Location = new System.Drawing.Point(183, 20);
            this.groupBoxUser.Name = "groupBoxUser";
            this.groupBoxUser.Size = new System.Drawing.Size(141, 132);
            this.groupBoxUser.TabIndex = 12;
            this.groupBoxUser.TabStop = false;
            this.groupBoxUser.Text = "User Token";
            // 
            // chkUsernameToken
            // 
            this.chkUsernameToken.AutoSize = true;
            this.chkUsernameToken.Location = new System.Drawing.Point(7, 97);
            this.chkUsernameToken.Name = "chkUsernameToken";
            this.chkUsernameToken.Size = new System.Drawing.Size(82, 16);
            this.chkUsernameToken.TabIndex = 0;
            this.chkUsernameToken.Text = "Username";
            this.chkUsernameToken.UseVisualStyleBackColor = true;
            // 
            // chkCertificateToken
            // 
            this.chkCertificateToken.AutoSize = true;
            this.chkCertificateToken.Location = new System.Drawing.Point(6, 65);
            this.chkCertificateToken.Name = "chkCertificateToken";
            this.chkCertificateToken.Size = new System.Drawing.Size(80, 16);
            this.chkCertificateToken.TabIndex = 0;
            this.chkCertificateToken.Text = "Certificate";
            this.chkCertificateToken.UseVisualStyleBackColor = true;
            // 
            // chkAnonymousToken
            // 
            this.chkAnonymousToken.AutoSize = true;
            this.chkAnonymousToken.Location = new System.Drawing.Point(7, 32);
            this.chkAnonymousToken.Name = "chkAnonymousToken";
            this.chkAnonymousToken.Size = new System.Drawing.Size(92, 16);
            this.chkAnonymousToken.TabIndex = 0;
            this.chkAnonymousToken.Text = "Anonymous";
            this.chkAnonymousToken.UseVisualStyleBackColor = true;
            // 
            // groupBoxServer
            // 
            this.groupBoxServer.Controls.Add(this.buttonReloadIP);
            this.groupBoxServer.Controls.Add(this.comboBoxIP);
            this.groupBoxServer.Controls.Add(this.numericUpDownPort);
            this.groupBoxServer.Controls.Add(this.label1);
            this.groupBoxServer.Controls.Add(this.labelIp);
            this.groupBoxServer.Controls.Add(this.labelPort);
            this.groupBoxServer.Controls.Add(this.btnStop);
            this.groupBoxServer.Controls.Add(this.btnStart);
            this.groupBoxServer.Controls.Add(this.lblStatus);
            this.groupBoxServer.Location = new System.Drawing.Point(12, 59);
            this.groupBoxServer.Name = "groupBoxServer";
            this.groupBoxServer.Size = new System.Drawing.Size(308, 163);
            this.groupBoxServer.TabIndex = 13;
            this.groupBoxServer.TabStop = false;
            this.groupBoxServer.Text = "Server";
            // 
            // buttonReloadIP
            // 
            this.buttonReloadIP.Location = new System.Drawing.Point(229, 79);
            this.buttonReloadIP.Name = "buttonReloadIP";
            this.buttonReloadIP.Size = new System.Drawing.Size(54, 23);
            this.buttonReloadIP.TabIndex = 8;
            this.buttonReloadIP.Text = "Reload";
            this.buttonReloadIP.UseVisualStyleBackColor = true;
            this.buttonReloadIP.Click += new System.EventHandler(this.buttonReloadIP_Click);
            // 
            // comboBoxIP
            // 
            this.comboBoxIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIP.FormattingEnabled = true;
            this.comboBoxIP.Location = new System.Drawing.Point(85, 81);
            this.comboBoxIP.Name = "comboBoxIP";
            this.comboBoxIP.Size = new System.Drawing.Size(128, 20);
            this.comboBoxIP.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Set IP";
            // 
            // labelIp
            // 
            this.labelIp.AutoSize = true;
            this.labelIp.Location = new System.Drawing.Point(83, 22);
            this.labelIp.Name = "labelIp";
            this.labelIp.Size = new System.Drawing.Size(53, 12);
            this.labelIp.TabIndex = 5;
            this.labelIp.Text = "127.0.0.1";
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Location = new System.Drawing.Point(12, 272);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.Size = new System.Drawing.Size(638, 138);
            this.richTextBoxLog.TabIndex = 15;
            this.richTextBoxLog.Text = "";
            // 
            // chkServerEnable
            // 
            this.chkServerEnable.AutoSize = true;
            this.chkServerEnable.Location = new System.Drawing.Point(12, 37);
            this.chkServerEnable.Name = "chkServerEnable";
            this.chkServerEnable.Size = new System.Drawing.Size(132, 16);
            this.chkServerEnable.TabIndex = 16;
            this.chkServerEnable.Text = "Use OPCUA Server";
            this.chkServerEnable.UseVisualStyleBackColor = true;
            this.chkServerEnable.CheckedChanged += new System.EventHandler(this.chkServerEnable_CheckedChanged);
            // 
            // FormOpcUaServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 441);
            this.Controls.Add(this.chkServerEnable);
            this.Controls.Add(this.richTextBoxLog);
            this.Controls.Add(this.groupBoxServer);
            this.Controls.Add(this.groupBoxSecurity);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormOpcUaServer";
            this.Text = "Autobase OPCUA Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOpcUaServer_FormClosed);
            this.Load += new System.EventHandler(this.FormOpcUaServer_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.groupBoxSecurity.ResumeLayout(false);
            this.groupBoxSecurity.PerformLayout();
            this.groupBoxUser.ResumeLayout(false);
            this.groupBoxUser.PerformLayout();
            this.groupBoxServer.ResumeLayout(false);
            this.groupBoxServer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSetUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuCreateCert;
        private System.Windows.Forms.ToolStripMenuItem menuCertificateSettings;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.CheckBox checkBoxSha256;
        private System.Windows.Forms.ComboBox comboBoxSecurity;
        private System.Windows.Forms.CheckBox checkBoxNone;
        private System.Windows.Forms.GroupBox groupBoxSecurity;
        private System.Windows.Forms.GroupBox groupBoxServer;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Label labelIp;
        private System.Windows.Forms.ComboBox comboBoxIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonReloadIP;
        private System.Windows.Forms.CheckBox chkServerEnable;
        private System.Windows.Forms.ToolStripMenuItem menuUserSettings;
        private System.Windows.Forms.GroupBox groupBoxUser;
        private System.Windows.Forms.CheckBox chkUsernameToken;
        private System.Windows.Forms.CheckBox chkCertificateToken;
        private System.Windows.Forms.CheckBox chkAnonymousToken;
    }
}