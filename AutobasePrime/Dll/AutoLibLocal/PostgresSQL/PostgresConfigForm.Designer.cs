namespace AutoLibLocal.PostgresSQL
{
    partial class PostgresConfigForm
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
            this.lblHost = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.lblTimezone = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.cmbTimezone = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblPasswordConfirm = new System.Windows.Forms.Label();
            this.txtPasswordConfirm = new System.Windows.Forms.TextBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            // 이중화 컨트롤
            this.grpReplication = new System.Windows.Forms.GroupBox();
            this.chkUseReplication = new System.Windows.Forms.CheckBox();
            this.lblStandbyHost = new System.Windows.Forms.Label();
            this.txtStandbyHost = new System.Windows.Forms.TextBox();
            this.lblStandbyPort = new System.Windows.Forms.Label();
            this.txtStandbyPort = new System.Windows.Forms.TextBox();
            this.lblStandbyUsername = new System.Windows.Forms.Label();
            this.txtStandbyUsername = new System.Windows.Forms.TextBox();
            this.lblStandbyPassword = new System.Windows.Forms.Label();
            this.txtStandbyPassword = new System.Windows.Forms.TextBox();
            this.chkReadWriteSplit = new System.Windows.Forms.CheckBox();
            this.chkAutoFailover = new System.Windows.Forms.CheckBox();
            this.btnTestStandby = new System.Windows.Forms.Button();
            this.lblStandbyStatus = new System.Windows.Forms.Label();
            this.grpReplication.SuspendLayout();
            this.SuspendLayout();
            //
            // lblHost
            //
            this.lblHost.Location = new System.Drawing.Point(11, 16);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(80, 18);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Host";
            //
            // lblPort
            //
            this.lblPort.Location = new System.Drawing.Point(11, 44);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(80, 18);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port";
            //
            // lblUsername
            //
            this.lblUsername.Location = new System.Drawing.Point(11, 72);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(80, 18);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "User";
            //
            // lblPassword
            //
            this.lblPassword.Location = new System.Drawing.Point(11, 100);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(80, 18);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password";
            //
            // lblDatabase
            //
            this.lblDatabase.Location = new System.Drawing.Point(11, 158);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(104, 18);
            this.lblDatabase.TabIndex = 8;
            this.lblDatabase.Text = "Database";
            //
            // lblTimezone
            //
            this.lblTimezone.Location = new System.Drawing.Point(10, 188);
            this.lblTimezone.Name = "lblTimezone";
            this.lblTimezone.Size = new System.Drawing.Size(80, 18);
            this.lblTimezone.TabIndex = 10;
            this.lblTimezone.Text = "TimeZone";
            //
            // txtHost
            //
            this.txtHost.Location = new System.Drawing.Point(145, 11);
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(250, 21);
            this.txtHost.TabIndex = 1;
            //
            // txtPort
            //
            this.txtPort.Location = new System.Drawing.Point(145, 41);
            this.txtPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 21);
            this.txtPort.TabIndex = 2;
            //
            // txtUsername
            //
            this.txtUsername.Location = new System.Drawing.Point(145, 69);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(250, 21);
            this.txtUsername.TabIndex = 3;
            //
            // txtPassword
            //
            this.txtPassword.Location = new System.Drawing.Point(145, 97);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(250, 21);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // txtDatabase
            //
            this.txtDatabase.Location = new System.Drawing.Point(145, 155);
            this.txtDatabase.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(250, 21);
            this.txtDatabase.TabIndex = 6;
            //
            // cmbTimezone
            //
            this.cmbTimezone.FormattingEnabled = true;
            this.cmbTimezone.Items.AddRange(new object[] {
            "Asia/Seoul",
            "UTC",
            "America/New_York",
            "America/Los_Angeles",
            "Europe/London",
            "Europe/Paris",
            "Asia/Tokyo",
            "Asia/Shanghai"});
            this.cmbTimezone.Location = new System.Drawing.Point(145, 185);
            this.cmbTimezone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbTimezone.Name = "cmbTimezone";
            this.cmbTimezone.Size = new System.Drawing.Size(250, 20);
            this.cmbTimezone.TabIndex = 7;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(233, 480);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 24);
            this.btnSave.TabIndex = 20;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(323, 480);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 24);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            // btnTest
            //
            this.btnTest.Location = new System.Drawing.Point(12, 218);
            this.btnTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(127, 24);
            this.btnTest.TabIndex = 8;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            //
            // lblPasswordConfirm
            //
            this.lblPasswordConfirm.Location = new System.Drawing.Point(11, 128);
            this.lblPasswordConfirm.Name = "lblPasswordConfirm";
            this.lblPasswordConfirm.Size = new System.Drawing.Size(128, 18);
            this.lblPasswordConfirm.TabIndex = 6;
            this.lblPasswordConfirm.Text = "Confirm Password";
            //
            // txtPasswordConfirm
            //
            this.txtPasswordConfirm.Location = new System.Drawing.Point(145, 125);
            this.txtPasswordConfirm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPasswordConfirm.Name = "txtPasswordConfirm";
            this.txtPasswordConfirm.Size = new System.Drawing.Size(250, 21);
            this.txtPasswordConfirm.TabIndex = 5;
            this.txtPasswordConfirm.UseSystemPasswordChar = true;
            //
            // buttonExport
            //
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExport.Location = new System.Drawing.Point(13, 480);
            this.buttonExport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(80, 24);
            this.buttonExport.TabIndex = 13;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Visible = false;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            //
            // buttonImport
            //
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonImport.Location = new System.Drawing.Point(99, 480);
            this.buttonImport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(80, 24);
            this.buttonImport.TabIndex = 13;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Visible = false;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(145, 218);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(250, 40);
            this.lblStatus.TabIndex = 10;
            //
            // grpReplication
            //
            this.grpReplication.Controls.Add(this.chkUseReplication);
            this.grpReplication.Controls.Add(this.lblStandbyHost);
            this.grpReplication.Controls.Add(this.txtStandbyHost);
            this.grpReplication.Controls.Add(this.lblStandbyPort);
            this.grpReplication.Controls.Add(this.txtStandbyPort);
            this.grpReplication.Controls.Add(this.lblStandbyUsername);
            this.grpReplication.Controls.Add(this.txtStandbyUsername);
            this.grpReplication.Controls.Add(this.lblStandbyPassword);
            this.grpReplication.Controls.Add(this.txtStandbyPassword);
            this.grpReplication.Controls.Add(this.chkReadWriteSplit);
            this.grpReplication.Controls.Add(this.chkAutoFailover);
            this.grpReplication.Controls.Add(this.btnTestStandby);
            this.grpReplication.Controls.Add(this.lblStandbyStatus);
            this.grpReplication.Location = new System.Drawing.Point(11, 260);
            this.grpReplication.Name = "grpReplication";
            this.grpReplication.Size = new System.Drawing.Size(393, 210);
            this.grpReplication.TabIndex = 14;
            this.grpReplication.TabStop = false;
            this.grpReplication.Text = "Replication (Standby)";
            //
            // chkUseReplication
            //
            this.chkUseReplication.Location = new System.Drawing.Point(10, 20);
            this.chkUseReplication.Name = "chkUseReplication";
            this.chkUseReplication.Size = new System.Drawing.Size(200, 18);
            this.chkUseReplication.TabIndex = 0;
            this.chkUseReplication.Text = "Enable Replication";
            this.chkUseReplication.UseVisualStyleBackColor = true;
            this.chkUseReplication.CheckedChanged += new System.EventHandler(this.ChkUseReplication_CheckedChanged);
            //
            // lblStandbyHost
            //
            this.lblStandbyHost.Location = new System.Drawing.Point(10, 46);
            this.lblStandbyHost.Name = "lblStandbyHost";
            this.lblStandbyHost.Size = new System.Drawing.Size(120, 18);
            this.lblStandbyHost.TabIndex = 1;
            this.lblStandbyHost.Text = "Standby Host";
            //
            // txtStandbyHost
            //
            this.txtStandbyHost.Enabled = false;
            this.txtStandbyHost.Location = new System.Drawing.Point(134, 43);
            this.txtStandbyHost.Name = "txtStandbyHost";
            this.txtStandbyHost.Size = new System.Drawing.Size(250, 21);
            this.txtStandbyHost.TabIndex = 2;
            //
            // lblStandbyPort
            //
            this.lblStandbyPort.Location = new System.Drawing.Point(10, 72);
            this.lblStandbyPort.Name = "lblStandbyPort";
            this.lblStandbyPort.Size = new System.Drawing.Size(120, 18);
            this.lblStandbyPort.TabIndex = 3;
            this.lblStandbyPort.Text = "Standby Port";
            //
            // txtStandbyPort
            //
            this.txtStandbyPort.Enabled = false;
            this.txtStandbyPort.Location = new System.Drawing.Point(134, 69);
            this.txtStandbyPort.Name = "txtStandbyPort";
            this.txtStandbyPort.Size = new System.Drawing.Size(100, 21);
            this.txtStandbyPort.TabIndex = 4;
            this.txtStandbyPort.Text = "5432";
            //
            // lblStandbyUsername
            //
            this.lblStandbyUsername.Location = new System.Drawing.Point(10, 98);
            this.lblStandbyUsername.Name = "lblStandbyUsername";
            this.lblStandbyUsername.Size = new System.Drawing.Size(120, 18);
            this.lblStandbyUsername.TabIndex = 5;
            this.lblStandbyUsername.Text = "Standby User";
            //
            // txtStandbyUsername
            //
            this.txtStandbyUsername.Enabled = false;
            this.txtStandbyUsername.Location = new System.Drawing.Point(134, 95);
            this.txtStandbyUsername.Name = "txtStandbyUsername";
            this.txtStandbyUsername.Size = new System.Drawing.Size(250, 21);
            this.txtStandbyUsername.TabIndex = 6;
            //
            // lblStandbyPassword
            //
            this.lblStandbyPassword.Location = new System.Drawing.Point(10, 124);
            this.lblStandbyPassword.Name = "lblStandbyPassword";
            this.lblStandbyPassword.Size = new System.Drawing.Size(120, 18);
            this.lblStandbyPassword.TabIndex = 7;
            this.lblStandbyPassword.Text = "Standby Password";
            //
            // txtStandbyPassword
            //
            this.txtStandbyPassword.Enabled = false;
            this.txtStandbyPassword.Location = new System.Drawing.Point(134, 121);
            this.txtStandbyPassword.Name = "txtStandbyPassword";
            this.txtStandbyPassword.Size = new System.Drawing.Size(250, 21);
            this.txtStandbyPassword.TabIndex = 8;
            this.txtStandbyPassword.UseSystemPasswordChar = true;
            //
            // chkReadWriteSplit
            //
            this.chkReadWriteSplit.Enabled = false;
            this.chkReadWriteSplit.Location = new System.Drawing.Point(10, 150);
            this.chkReadWriteSplit.Name = "chkReadWriteSplit";
            this.chkReadWriteSplit.Size = new System.Drawing.Size(170, 18);
            this.chkReadWriteSplit.TabIndex = 9;
            this.chkReadWriteSplit.Text = "Read/Write Split";
            this.chkReadWriteSplit.UseVisualStyleBackColor = true;
            //
            // chkAutoFailover
            //
            this.chkAutoFailover.Checked = true;
            this.chkAutoFailover.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoFailover.Enabled = false;
            this.chkAutoFailover.Location = new System.Drawing.Point(200, 150);
            this.chkAutoFailover.Name = "chkAutoFailover";
            this.chkAutoFailover.Size = new System.Drawing.Size(170, 18);
            this.chkAutoFailover.TabIndex = 10;
            this.chkAutoFailover.Text = "Auto Failover";
            this.chkAutoFailover.UseVisualStyleBackColor = true;
            //
            // btnTestStandby
            //
            this.btnTestStandby.Enabled = false;
            this.btnTestStandby.Location = new System.Drawing.Point(10, 178);
            this.btnTestStandby.Name = "btnTestStandby";
            this.btnTestStandby.Size = new System.Drawing.Size(140, 24);
            this.btnTestStandby.TabIndex = 11;
            this.btnTestStandby.Text = "Test Standby";
            this.btnTestStandby.UseVisualStyleBackColor = true;
            this.btnTestStandby.Click += new System.EventHandler(this.BtnTestStandby_Click);
            //
            // lblStandbyStatus
            //
            this.lblStandbyStatus.Location = new System.Drawing.Point(156, 178);
            this.lblStandbyStatus.Name = "lblStandbyStatus";
            this.lblStandbyStatus.Size = new System.Drawing.Size(228, 24);
            this.lblStandbyStatus.TabIndex = 12;
            //
            // PostgresConfigForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 518);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPasswordConfirm);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPasswordConfirm);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTimezone);
            this.Controls.Add(this.cmbTimezone);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.grpReplication);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PostgresConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PostgreSQL Database Config";
            this.grpReplication.ResumeLayout(false);
            this.grpReplication.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPasswordConfirm;
        private System.Windows.Forms.TextBox txtPasswordConfirm;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label lblStatus;

        // 이중화 컨트롤
        private System.Windows.Forms.GroupBox grpReplication;
        private System.Windows.Forms.CheckBox chkUseReplication;
        private System.Windows.Forms.Label lblStandbyHost;
        private System.Windows.Forms.TextBox txtStandbyHost;
        private System.Windows.Forms.Label lblStandbyPort;
        private System.Windows.Forms.TextBox txtStandbyPort;
        private System.Windows.Forms.Label lblStandbyUsername;
        private System.Windows.Forms.TextBox txtStandbyUsername;
        private System.Windows.Forms.Label lblStandbyPassword;
        private System.Windows.Forms.TextBox txtStandbyPassword;
        private System.Windows.Forms.CheckBox chkReadWriteSplit;
        private System.Windows.Forms.CheckBox chkAutoFailover;
        private System.Windows.Forms.Button btnTestStandby;
        private System.Windows.Forms.Label lblStandbyStatus;
    }
}
