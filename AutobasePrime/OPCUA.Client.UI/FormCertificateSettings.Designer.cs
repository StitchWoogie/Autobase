namespace OPCUA.Client.UI
{
    partial class FormCertificateSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCertificateSettings));
            this.buttonClose = new System.Windows.Forms.Button();
            this.richTextBoxClientCert = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_list_certs = new System.Windows.Forms.ListView();
            this.buttonImport = new System.Windows.Forms.Button();
            this.labelCertStatusText = new System.Windows.Forms.Label();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonTrustReject = new System.Windows.Forms.Button();
            this.labelCertStatus = new System.Windows.Forms.Label();
            this.buttonBrowsePrivateKey = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.textPrivateKeyPath = new System.Windows.Forms.TextBox();
            this.textDerPath = new System.Windows.Forms.TextBox();
            this.buttonBrowseDer = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonRegisterCert = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonPwToggle = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(678, 4);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(99, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // richTextBoxClientCert
            // 
            this.richTextBoxClientCert.Location = new System.Drawing.Point(5, 27);
            this.richTextBoxClientCert.Name = "richTextBoxClientCert";
            this.richTextBoxClientCert.Size = new System.Drawing.Size(395, 178);
            this.richTextBoxClientCert.TabIndex = 5;
            this.richTextBoxClientCert.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Certificate List";
            // 
            // m_list_certs
            // 
            this.m_list_certs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_list_certs.HideSelection = false;
            this.m_list_certs.Location = new System.Drawing.Point(0, 0);
            this.m_list_certs.Name = "m_list_certs";
            this.m_list_certs.Size = new System.Drawing.Size(657, 184);
            this.m_list_certs.TabIndex = 4;
            this.m_list_certs.UseCompatibleStateImageBehavior = false;
            this.m_list_certs.View = System.Windows.Forms.View.Details;
            this.m_list_certs.SelectedIndexChanged += new System.EventHandler(this.m_list_certs_SelectedIndexChanged);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(10, 131);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(99, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // labelCertStatusText
            // 
            this.labelCertStatusText.AutoSize = true;
            this.labelCertStatusText.Location = new System.Drawing.Point(172, 9);
            this.labelCertStatusText.Name = "labelCertStatusText";
            this.labelCertStatusText.Size = new System.Drawing.Size(33, 12);
            this.labelCertStatusText.TabIndex = 6;
            this.labelCertStatusText.Text = "Valid";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(10, 86);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(99, 23);
            this.buttonRemove.TabIndex = 1;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(10, 176);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(99, 23);
            this.buttonExport.TabIndex = 1;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonTrustReject
            // 
            this.buttonTrustReject.Location = new System.Drawing.Point(10, 41);
            this.buttonTrustReject.Name = "buttonTrustReject";
            this.buttonTrustReject.Size = new System.Drawing.Size(99, 23);
            this.buttonTrustReject.TabIndex = 1;
            this.buttonTrustReject.Text = "Trust / Reject";
            this.buttonTrustReject.UseVisualStyleBackColor = true;
            this.buttonTrustReject.Click += new System.EventHandler(this.buttonTrustReject_Click);
            // 
            // labelCertStatus
            // 
            this.labelCertStatus.AutoSize = true;
            this.labelCertStatus.Location = new System.Drawing.Point(12, 9);
            this.labelCertStatus.Name = "labelCertStatus";
            this.labelCertStatus.Size = new System.Drawing.Size(144, 12);
            this.labelCertStatus.TabIndex = 6;
            this.labelCertStatus.Text = "Client Certificate Status :";
            // 
            // buttonBrowsePrivateKey
            // 
            this.buttonBrowsePrivateKey.Location = new System.Drawing.Point(729, 93);
            this.buttonBrowsePrivateKey.Name = "buttonBrowsePrivateKey";
            this.buttonBrowsePrivateKey.Size = new System.Drawing.Size(25, 23);
            this.buttonBrowsePrivateKey.TabIndex = 7;
            this.buttonBrowsePrivateKey.Text = "...";
            this.buttonBrowsePrivateKey.UseVisualStyleBackColor = true;
            this.buttonBrowsePrivateKey.Click += new System.EventHandler(this.buttonBrowsePrivateKey_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonRegisterCert);
            this.panel1.Controls.Add(this.textPassword);
            this.panel1.Controls.Add(this.textPrivateKeyPath);
            this.panel1.Controls.Add(this.textDerPath);
            this.panel1.Controls.Add(this.buttonBrowseDer);
            this.panel1.Controls.Add(this.buttonPwToggle);
            this.panel1.Controls.Add(this.buttonBrowsePrivateKey);
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Controls.Add(this.labelCertStatus);
            this.panel1.Controls.Add(this.richTextBoxClientCert);
            this.panel1.Controls.Add(this.labelCertStatusText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(784, 215);
            this.panel1.TabIndex = 8;
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(505, 126);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(206, 21);
            this.textPassword.TabIndex = 11;
            this.textPassword.UseSystemPasswordChar = true;
            // 
            // textPrivateKeyPath
            // 
            this.textPrivateKeyPath.Location = new System.Drawing.Point(505, 95);
            this.textPrivateKeyPath.Name = "textPrivateKeyPath";
            this.textPrivateKeyPath.Size = new System.Drawing.Size(206, 21);
            this.textPrivateKeyPath.TabIndex = 11;
            // 
            // textDerPath
            // 
            this.textDerPath.Location = new System.Drawing.Point(505, 64);
            this.textDerPath.Name = "textDerPath";
            this.textDerPath.Size = new System.Drawing.Size(206, 21);
            this.textDerPath.TabIndex = 11;
            // 
            // buttonBrowseDer
            // 
            this.buttonBrowseDer.Location = new System.Drawing.Point(727, 62);
            this.buttonBrowseDer.Name = "buttonBrowseDer";
            this.buttonBrowseDer.Size = new System.Drawing.Size(27, 23);
            this.buttonBrowseDer.TabIndex = 7;
            this.buttonBrowseDer.Text = "...";
            this.buttonBrowseDer.UseVisualStyleBackColor = true;
            this.buttonBrowseDer.Click += new System.EventHandler(this.buttonBrowseDer_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 215);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(784, 226);
            this.panel2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.m_list_certs);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(5, 35);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(657, 184);
            this.panel3.TabIndex = 5;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(5, 5);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(657, 30);
            this.panel5.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.buttonImport);
            this.panel4.Controls.Add(this.buttonTrustReject);
            this.panel4.Controls.Add(this.buttonRemove);
            this.panel4.Controls.Add(this.buttonExport);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(662, 5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(115, 214);
            this.panel4.TabIndex = 6;
            // 
            // buttonRegisterCert
            // 
            this.buttonRegisterCert.Location = new System.Drawing.Point(573, 164);
            this.buttonRegisterCert.Name = "buttonRegisterCert";
            this.buttonRegisterCert.Size = new System.Drawing.Size(138, 23);
            this.buttonRegisterCert.TabIndex = 12;
            this.buttonRegisterCert.Text = "Register Certificate";
            this.buttonRegisterCert.UseVisualStyleBackColor = true;
            this.buttonRegisterCert.Click += new System.EventHandler(this.buttonRegisterCert_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(417, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Cert. Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(417, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "Key Path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(416, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "Password";
            // 
            // buttonPwToggle
            // 
            this.buttonPwToggle.Location = new System.Drawing.Point(717, 126);
            this.buttonPwToggle.Name = "buttonPwToggle";
            this.buttonPwToggle.Size = new System.Drawing.Size(52, 23);
            this.buttonPwToggle.TabIndex = 7;
            this.buttonPwToggle.Text = "Show";
            this.buttonPwToggle.UseVisualStyleBackColor = true;
            this.buttonPwToggle.MouseCaptureChanged += new System.EventHandler(this.buttonPwToggle_MouseCaptureChanged);
            // 
            // FormCertificateSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCertificateSettings";
            this.Text = "Certificate Settings";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.RichTextBox richTextBoxClientCert;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView m_list_certs;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label labelCertStatusText;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonTrustReject;
        private System.Windows.Forms.Label labelCertStatus;
        private System.Windows.Forms.Button buttonBrowsePrivateKey;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button buttonBrowseDer;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.TextBox textPrivateKeyPath;
        private System.Windows.Forms.TextBox textDerPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRegisterCert;
        private System.Windows.Forms.Button buttonPwToggle;
    }
}