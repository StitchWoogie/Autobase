namespace OPCUA.Client.UI
{
    partial class FormCreateCert
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
            this.buttonGenerateKey = new System.Windows.Forms.Button();
            this.labelCN = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numYears = new System.Windows.Forms.NumericUpDown();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxDNS = new System.Windows.Forms.TextBox();
            this.textBoxURI = new System.Windows.Forms.TextBox();
            this.textBoxOrg = new System.Windows.Forms.TextBox();
            this.textBoxCN = new System.Windows.Forms.TextBox();
            this.checkBoxEncrypt = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.labelYears = new System.Windows.Forms.Label();
            this.labelValidFor = new System.Windows.Forms.Label();
            this.labelDNS = new System.Windows.Forms.Label();
            this.labelURI = new System.Windows.Forms.Label();
            this.labelOrg = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonGoDir = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYears)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonGenerateKey
            // 
            this.buttonGenerateKey.Location = new System.Drawing.Point(404, 33);
            this.buttonGenerateKey.Name = "buttonGenerateKey";
            this.buttonGenerateKey.Size = new System.Drawing.Size(100, 23);
            this.buttonGenerateKey.TabIndex = 0;
            this.buttonGenerateKey.Text = "Generate";
            this.buttonGenerateKey.UseVisualStyleBackColor = true;
            this.buttonGenerateKey.Click += new System.EventHandler(this.buttonGenerateKey_Click);
            // 
            // labelCN
            // 
            this.labelCN.AutoSize = true;
            this.labelCN.Location = new System.Drawing.Point(6, 32);
            this.labelCN.Name = "labelCN";
            this.labelCN.Size = new System.Drawing.Size(103, 12);
            this.labelCN.TabIndex = 1;
            this.labelCN.Text = "Common Name :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numYears);
            this.groupBox1.Controls.Add(this.textBoxPassword);
            this.groupBox1.Controls.Add(this.textBoxDNS);
            this.groupBox1.Controls.Add(this.textBoxURI);
            this.groupBox1.Controls.Add(this.textBoxOrg);
            this.groupBox1.Controls.Add(this.textBoxCN);
            this.groupBox1.Controls.Add(this.checkBoxEncrypt);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.labelYears);
            this.groupBox1.Controls.Add(this.labelValidFor);
            this.groupBox1.Controls.Add(this.labelDNS);
            this.groupBox1.Controls.Add(this.labelURI);
            this.groupBox1.Controls.Add(this.labelOrg);
            this.groupBox1.Controls.Add(this.labelCN);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 223);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Certificate Setting";
            // 
            // numYears
            // 
            this.numYears.Location = new System.Drawing.Point(115, 158);
            this.numYears.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numYears.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numYears.Name = "numYears";
            this.numYears.Size = new System.Drawing.Size(78, 21);
            this.numYears.TabIndex = 4;
            this.numYears.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(115, 191);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(180, 21);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // textBoxDNS
            // 
            this.textBoxDNS.Location = new System.Drawing.Point(115, 125);
            this.textBoxDNS.Name = "textBoxDNS";
            this.textBoxDNS.Size = new System.Drawing.Size(228, 21);
            this.textBoxDNS.TabIndex = 3;
            this.textBoxDNS.Text = "autobase";
            // 
            // textBoxURI
            // 
            this.textBoxURI.Location = new System.Drawing.Point(115, 93);
            this.textBoxURI.Name = "textBoxURI";
            this.textBoxURI.ReadOnly = true;
            this.textBoxURI.Size = new System.Drawing.Size(228, 21);
            this.textBoxURI.TabIndex = 3;
            this.textBoxURI.Text = "urn:autobase.opcua:client";
            // 
            // textBoxOrg
            // 
            this.textBoxOrg.Location = new System.Drawing.Point(115, 61);
            this.textBoxOrg.Name = "textBoxOrg";
            this.textBoxOrg.Size = new System.Drawing.Size(228, 21);
            this.textBoxOrg.TabIndex = 3;
            this.textBoxOrg.Text = "autobase";
            // 
            // textBoxCN
            // 
            this.textBoxCN.Location = new System.Drawing.Point(115, 29);
            this.textBoxCN.Name = "textBoxCN";
            this.textBoxCN.Size = new System.Drawing.Size(228, 21);
            this.textBoxCN.TabIndex = 3;
            this.textBoxCN.Text = "AutobaseOPCUAClient";
            // 
            // checkBoxEncrypt
            // 
            this.checkBoxEncrypt.AutoSize = true;
            this.checkBoxEncrypt.Location = new System.Drawing.Point(304, 195);
            this.checkBoxEncrypt.Name = "checkBoxEncrypt";
            this.checkBoxEncrypt.Size = new System.Drawing.Size(67, 16);
            this.checkBoxEncrypt.TabIndex = 2;
            this.checkBoxEncrypt.Text = "Encrypt";
            this.checkBoxEncrypt.UseVisualStyleBackColor = true;
            this.checkBoxEncrypt.CheckedChanged += new System.EventHandler(this.checkBoxEncrypt_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Password :";
            // 
            // labelYears
            // 
            this.labelYears.AutoSize = true;
            this.labelYears.Location = new System.Drawing.Point(199, 160);
            this.labelYears.Name = "labelYears";
            this.labelYears.Size = new System.Drawing.Size(38, 12);
            this.labelYears.TabIndex = 1;
            this.labelYears.Text = "Years";
            // 
            // labelValidFor
            // 
            this.labelValidFor.AutoSize = true;
            this.labelValidFor.Location = new System.Drawing.Point(6, 160);
            this.labelValidFor.Name = "labelValidFor";
            this.labelValidFor.Size = new System.Drawing.Size(63, 12);
            this.labelValidFor.TabIndex = 1;
            this.labelValidFor.Text = "Valid for : ";
            // 
            // labelDNS
            // 
            this.labelDNS.AutoSize = true;
            this.labelDNS.Location = new System.Drawing.Point(6, 128);
            this.labelDNS.Name = "labelDNS";
            this.labelDNS.Size = new System.Drawing.Size(38, 12);
            this.labelDNS.TabIndex = 1;
            this.labelDNS.Text = "DNS :";
            // 
            // labelURI
            // 
            this.labelURI.AutoSize = true;
            this.labelURI.Location = new System.Drawing.Point(6, 96);
            this.labelURI.Name = "labelURI";
            this.labelURI.Size = new System.Drawing.Size(32, 12);
            this.labelURI.TabIndex = 1;
            this.labelURI.Text = "URI :";
            // 
            // labelOrg
            // 
            this.labelOrg.AutoSize = true;
            this.labelOrg.Location = new System.Drawing.Point(6, 64);
            this.labelOrg.Name = "labelOrg";
            this.labelOrg.Size = new System.Drawing.Size(88, 12);
            this.labelOrg.TabIndex = 1;
            this.labelOrg.Text = "Organization : ";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(404, 212);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(100, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonGoDir
            // 
            this.buttonGoDir.Location = new System.Drawing.Point(404, 76);
            this.buttonGoDir.Name = "buttonGoDir";
            this.buttonGoDir.Size = new System.Drawing.Size(100, 23);
            this.buttonGoDir.TabIndex = 0;
            this.buttonGoDir.Text = "Go to Directory";
            this.buttonGoDir.UseVisualStyleBackColor = true;
            this.buttonGoDir.Click += new System.EventHandler(this.buttonGoDir_Click);
            // 
            // FormCreateCert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 251);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonGoDir);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonGenerateKey);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormCreateCert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Certificate";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numYears)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonGenerateKey;
        private System.Windows.Forms.Label labelCN;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelOrg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelYears;
        private System.Windows.Forms.Label labelValidFor;
        private System.Windows.Forms.Label labelDNS;
        private System.Windows.Forms.Label labelURI;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonGoDir;
        private System.Windows.Forms.CheckBox checkBoxEncrypt;
        private System.Windows.Forms.NumericUpDown numYears;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxDNS;
        private System.Windows.Forms.TextBox textBoxURI;
        private System.Windows.Forms.TextBox textBoxOrg;
        private System.Windows.Forms.TextBox textBoxCN;
    }
}