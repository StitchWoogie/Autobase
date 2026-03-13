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
            this.btnSave.Location = new System.Drawing.Point(233, 260);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 24);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(323, 260);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 24);
            this.btnCancel.TabIndex = 10;
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
            this.buttonExport.Location = new System.Drawing.Point(13, 260);
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
            this.buttonImport.Location = new System.Drawing.Point(99, 260);
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
            // PostgresConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 299);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPasswordConfirm;
        private System.Windows.Forms.TextBox txtPasswordConfirm;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label lblStatus;
    }
}