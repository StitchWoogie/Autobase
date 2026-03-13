namespace OPCUA.Client.UI
{
    partial class FormAddServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddServer));
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbServers = new System.Windows.Forms.ComboBox();
            this.txtDiscoveryUrl = new System.Windows.Forms.TextBox();
            this.btnFindServers = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetEndpoints = new System.Windows.Forms.Button();
            this.cmbEndpoints = new System.Windows.Forms.ComboBox();
            this.buttonTestCon = new System.Windows.Forms.Button();
            this.txtAccessName = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpAuth = new System.Windows.Forms.GroupBox();
            this.lblAuthMode = new System.Windows.Forms.Label();
            this.cmbAuthMode = new System.Windows.Forms.ComboBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtAuthUser = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtAuthPassword = new System.Windows.Forms.TextBox();
            this.lblCertThumb = new System.Windows.Forms.Label();
            this.txtCertThumbprint = new System.Windows.Forms.TextBox();
            this.btnBrowseCert = new System.Windows.Forms.Button();
            this.lblCertFile = new System.Windows.Forms.Label();
            this.txtCertFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowseCertFile = new System.Windows.Forms.Button();
            this.lblCertPassword = new System.Windows.Forms.Label();
            this.txtCertPassword = new System.Windows.Forms.TextBox();
            this.chkSaveCredentials = new System.Windows.Forms.CheckBox();
            this.btnShowPassword = new System.Windows.Forms.Button();
            this.grpAuth.SuspendLayout();
            this.SuspendLayout();
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Servers :";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Endpoints : ";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "AccessName :";
            //
            // cmbServers
            //
            this.cmbServers.FormattingEnabled = true;
            this.cmbServers.Location = new System.Drawing.Point(126, 64);
            this.cmbServers.Name = "cmbServers";
            this.cmbServers.Size = new System.Drawing.Size(294, 20);
            this.cmbServers.TabIndex = 1;
            //
            // txtDiscoveryUrl
            //
            this.txtDiscoveryUrl.Location = new System.Drawing.Point(126, 18);
            this.txtDiscoveryUrl.Name = "txtDiscoveryUrl";
            this.txtDiscoveryUrl.Size = new System.Drawing.Size(294, 21);
            this.txtDiscoveryUrl.TabIndex = 2;
            this.txtDiscoveryUrl.Text = "opc.tcp://127.0.0.1:62541";
            //
            // btnFindServers
            //
            this.btnFindServers.Location = new System.Drawing.Point(435, 18);
            this.btnFindServers.Name = "btnFindServers";
            this.btnFindServers.Size = new System.Drawing.Size(137, 23);
            this.btnFindServers.TabIndex = 3;
            this.btnFindServers.Text = "Find Servers";
            this.btnFindServers.UseVisualStyleBackColor = true;
            this.btnFindServers.Click += new System.EventHandler(this.btnFindServers_Click);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Discovery URL :";
            //
            // btnGetEndpoints
            //
            this.btnGetEndpoints.Location = new System.Drawing.Point(435, 62);
            this.btnGetEndpoints.Name = "btnGetEndpoints";
            this.btnGetEndpoints.Size = new System.Drawing.Size(137, 23);
            this.btnGetEndpoints.TabIndex = 3;
            this.btnGetEndpoints.Text = "Get EndPoints";
            this.btnGetEndpoints.UseVisualStyleBackColor = true;
            this.btnGetEndpoints.Click += new System.EventHandler(this.btnGetEndpoints_Click);
            //
            // cmbEndpoints
            //
            this.cmbEndpoints.DropDownWidth = 600;
            this.cmbEndpoints.FormattingEnabled = true;
            this.cmbEndpoints.Location = new System.Drawing.Point(126, 109);
            this.cmbEndpoints.Name = "cmbEndpoints";
            this.cmbEndpoints.Size = new System.Drawing.Size(294, 20);
            this.cmbEndpoints.TabIndex = 1;
            //
            // buttonTestCon
            //
            this.buttonTestCon.Location = new System.Drawing.Point(435, 107);
            this.buttonTestCon.Name = "buttonTestCon";
            this.buttonTestCon.Size = new System.Drawing.Size(137, 23);
            this.buttonTestCon.TabIndex = 3;
            this.buttonTestCon.Text = "Test connection";
            this.buttonTestCon.UseVisualStyleBackColor = true;
            this.buttonTestCon.Click += new System.EventHandler(this.btnTestConnection_Click);
            //
            // txtAccessName
            //
            this.txtAccessName.Location = new System.Drawing.Point(126, 154);
            this.txtAccessName.Name = "txtAccessName";
            this.txtAccessName.Size = new System.Drawing.Size(294, 21);
            this.txtAccessName.TabIndex = 2;
            //
            // grpAuth
            //
            this.grpAuth.Controls.Add(this.txtCertPassword);
            this.grpAuth.Controls.Add(this.lblCertPassword);
            this.grpAuth.Controls.Add(this.btnBrowseCertFile);
            this.grpAuth.Controls.Add(this.txtCertFilePath);
            this.grpAuth.Controls.Add(this.lblCertFile);
            this.grpAuth.Controls.Add(this.btnBrowseCert);
            this.grpAuth.Controls.Add(this.txtCertThumbprint);
            this.grpAuth.Controls.Add(this.lblCertThumb);
            this.grpAuth.Controls.Add(this.btnShowPassword);
            this.grpAuth.Controls.Add(this.txtAuthPassword);
            this.grpAuth.Controls.Add(this.lblPassword);
            this.grpAuth.Controls.Add(this.chkSaveCredentials);
            this.grpAuth.Controls.Add(this.txtAuthUser);
            this.grpAuth.Controls.Add(this.lblUserName);
            this.grpAuth.Controls.Add(this.cmbAuthMode);
            this.grpAuth.Controls.Add(this.lblAuthMode);
            this.grpAuth.Location = new System.Drawing.Point(12, 190);
            this.grpAuth.Name = "grpAuth";
            this.grpAuth.Size = new System.Drawing.Size(560, 160);
            this.grpAuth.TabIndex = 5;
            this.grpAuth.TabStop = false;
            this.grpAuth.Text = "Authentication";
            //
            // lblAuthMode
            //
            this.lblAuthMode.AutoSize = true;
            this.lblAuthMode.Location = new System.Drawing.Point(10, 22);
            this.lblAuthMode.Name = "lblAuthMode";
            this.lblAuthMode.Size = new System.Drawing.Size(42, 12);
            this.lblAuthMode.TabIndex = 0;
            this.lblAuthMode.Text = "Mode :";
            //
            // cmbAuthMode
            //
            this.cmbAuthMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAuthMode.FormattingEnabled = true;
            this.cmbAuthMode.Items.AddRange(new object[] {
            "Anonymous",
            "Username / Password",
            "Certificate"});
            this.cmbAuthMode.Location = new System.Drawing.Point(114, 19);
            this.cmbAuthMode.Name = "cmbAuthMode";
            this.cmbAuthMode.Size = new System.Drawing.Size(200, 20);
            this.cmbAuthMode.TabIndex = 1;
            this.cmbAuthMode.SelectedIndexChanged += new System.EventHandler(this.cmbAuthMode_SelectedIndexChanged);
            //
            // lblUserName
            //
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(10, 52);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(73, 12);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "Username :";
            this.lblUserName.Visible = false;
            //
            // txtAuthUser
            //
            this.txtAuthUser.Location = new System.Drawing.Point(114, 49);
            this.txtAuthUser.Name = "txtAuthUser";
            this.txtAuthUser.Size = new System.Drawing.Size(200, 21);
            this.txtAuthUser.TabIndex = 2;
            this.txtAuthUser.Visible = false;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(10, 82);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(69, 12);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password :";
            this.lblPassword.Visible = false;
            //
            // txtAuthPassword
            //
            this.txtAuthPassword.Location = new System.Drawing.Point(114, 79);
            this.txtAuthPassword.Name = "txtAuthPassword";
            this.txtAuthPassword.PasswordChar = '*';
            this.txtAuthPassword.Size = new System.Drawing.Size(200, 21);
            this.txtAuthPassword.TabIndex = 3;
            this.txtAuthPassword.Visible = false;
            //
            // chkSaveCredentials
            //
            this.chkSaveCredentials.AutoSize = true;
            this.chkSaveCredentials.Location = new System.Drawing.Point(320, 51);
            this.chkSaveCredentials.Name = "chkSaveCredentials";
            this.chkSaveCredentials.Size = new System.Drawing.Size(118, 16);
            this.chkSaveCredentials.TabIndex = 9;
            this.chkSaveCredentials.Text = "Save Credentials";
            this.chkSaveCredentials.UseVisualStyleBackColor = true;
            this.chkSaveCredentials.Visible = false;
            //
            // btnShowPassword
            //
            this.btnShowPassword.Location = new System.Drawing.Point(320, 78);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(55, 23);
            this.btnShowPassword.TabIndex = 10;
            this.btnShowPassword.Text = "Show";
            this.btnShowPassword.UseVisualStyleBackColor = true;
            this.btnShowPassword.Visible = false;
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            //
            // lblCertThumb
            //
            this.lblCertThumb.AutoSize = true;
            this.lblCertThumb.Location = new System.Drawing.Point(10, 52);
            this.lblCertThumb.Name = "lblCertThumb";
            this.lblCertThumb.Size = new System.Drawing.Size(82, 12);
            this.lblCertThumb.TabIndex = 0;
            this.lblCertThumb.Text = "Thumbprint :";
            this.lblCertThumb.Visible = false;
            //
            // txtCertThumbprint
            //
            this.txtCertThumbprint.Location = new System.Drawing.Point(114, 49);
            this.txtCertThumbprint.Name = "txtCertThumbprint";
            this.txtCertThumbprint.ReadOnly = true;
            this.txtCertThumbprint.Size = new System.Drawing.Size(310, 21);
            this.txtCertThumbprint.TabIndex = 4;
            this.txtCertThumbprint.Visible = false;
            //
            // btnBrowseCert
            //
            this.btnBrowseCert.Location = new System.Drawing.Point(435, 48);
            this.btnBrowseCert.Name = "btnBrowseCert";
            this.btnBrowseCert.Size = new System.Drawing.Size(110, 23);
            this.btnBrowseCert.TabIndex = 5;
            this.btnBrowseCert.Text = "Browse...";
            this.btnBrowseCert.UseVisualStyleBackColor = true;
            this.btnBrowseCert.Visible = false;
            this.btnBrowseCert.Click += new System.EventHandler(this.btnBrowseCert_Click);
            //
            // lblCertFile
            //
            this.lblCertFile.AutoSize = true;
            this.lblCertFile.Location = new System.Drawing.Point(10, 82);
            this.lblCertFile.Name = "lblCertFile";
            this.lblCertFile.Size = new System.Drawing.Size(90, 12);
            this.lblCertFile.TabIndex = 0;
            this.lblCertFile.Text = "Cert File (.pfx) :";
            this.lblCertFile.Visible = false;
            //
            // txtCertFilePath
            //
            this.txtCertFilePath.Location = new System.Drawing.Point(114, 79);
            this.txtCertFilePath.Name = "txtCertFilePath";
            this.txtCertFilePath.ReadOnly = true;
            this.txtCertFilePath.Size = new System.Drawing.Size(310, 21);
            this.txtCertFilePath.TabIndex = 6;
            this.txtCertFilePath.Visible = false;
            //
            // btnBrowseCertFile
            //
            this.btnBrowseCertFile.Location = new System.Drawing.Point(435, 78);
            this.btnBrowseCertFile.Name = "btnBrowseCertFile";
            this.btnBrowseCertFile.Size = new System.Drawing.Size(110, 23);
            this.btnBrowseCertFile.TabIndex = 7;
            this.btnBrowseCertFile.Text = "Browse...";
            this.btnBrowseCertFile.UseVisualStyleBackColor = true;
            this.btnBrowseCertFile.Visible = false;
            this.btnBrowseCertFile.Click += new System.EventHandler(this.btnBrowseCertFile_Click);
            //
            // lblCertPassword
            //
            this.lblCertPassword.AutoSize = true;
            this.lblCertPassword.Location = new System.Drawing.Point(10, 112);
            this.lblCertPassword.Name = "lblCertPassword";
            this.lblCertPassword.Size = new System.Drawing.Size(90, 12);
            this.lblCertPassword.TabIndex = 0;
            this.lblCertPassword.Text = "Cert Password :";
            this.lblCertPassword.Visible = false;
            //
            // txtCertPassword
            //
            this.txtCertPassword.Location = new System.Drawing.Point(114, 109);
            this.txtCertPassword.Name = "txtCertPassword";
            this.txtCertPassword.PasswordChar = '*';
            this.txtCertPassword.Size = new System.Drawing.Size(200, 21);
            this.txtCertPassword.TabIndex = 8;
            this.txtCertPassword.Visible = false;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(152, 365);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(87, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(282, 365);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // FormAddServer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 405);
            this.Controls.Add(this.grpAuth);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.buttonTestCon);
            this.Controls.Add(this.btnGetEndpoints);
            this.Controls.Add(this.btnFindServers);
            this.Controls.Add(this.txtAccessName);
            this.Controls.Add(this.txtDiscoveryUrl);
            this.Controls.Add(this.cmbEndpoints);
            this.Controls.Add(this.cmbServers);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAddServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add OPCUA Server";
            this.Load += new System.EventHandler(this.FromAddServer_Load);
            this.grpAuth.ResumeLayout(false);
            this.grpAuth.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbServers;
        private System.Windows.Forms.TextBox txtDiscoveryUrl;
        private System.Windows.Forms.Button btnFindServers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetEndpoints;
        private System.Windows.Forms.ComboBox cmbEndpoints;
        private System.Windows.Forms.Button buttonTestCon;
        private System.Windows.Forms.TextBox txtAccessName;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpAuth;
        private System.Windows.Forms.Label lblAuthMode;
        private System.Windows.Forms.ComboBox cmbAuthMode;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtAuthUser;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtAuthPassword;
        private System.Windows.Forms.Label lblCertThumb;
        private System.Windows.Forms.TextBox txtCertThumbprint;
        private System.Windows.Forms.Button btnBrowseCert;
        private System.Windows.Forms.Label lblCertFile;
        private System.Windows.Forms.TextBox txtCertFilePath;
        private System.Windows.Forms.Button btnBrowseCertFile;
        private System.Windows.Forms.Label lblCertPassword;
        private System.Windows.Forms.TextBox txtCertPassword;
        private System.Windows.Forms.CheckBox chkSaveCredentials;
        private System.Windows.Forms.Button btnShowPassword;
    }
}
