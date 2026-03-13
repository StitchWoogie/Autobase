using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Security.Cryptography;
using AutoLibLocal;
using NetTools;

namespace SmsServiceServer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormConfigService : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxUsername;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxPassword2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxTcpPort;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxServerIP;
		private System.Windows.Forms.TextBox textBoxSiteNamePrimary;
		private System.Windows.Forms.Button buttonTestPrimary;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBoxSiteNameSecondary;
		private System.Windows.Forms.Button buttonTestSecondary;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown numericUpDownServerConnectionTimeout;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigService()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigService));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxSiteNamePrimary = new System.Windows.Forms.TextBox();
            this.buttonTestPrimary = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPassword2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxSiteNameSecondary = new System.Windows.Forms.TextBox();
            this.buttonTestSecondary = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownServerConnectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxServerIP = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxTcpPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownServerConnectionTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxSiteNamePrimary
            // 
            this.textBoxSiteNamePrimary.AccessibleDescription = null;
            this.textBoxSiteNamePrimary.AccessibleName = null;
            resources.ApplyResources(this.textBoxSiteNamePrimary, "textBoxSiteNamePrimary");
            this.textBoxSiteNamePrimary.BackgroundImage = null;
            this.textBoxSiteNamePrimary.Font = null;
            this.textBoxSiteNamePrimary.Name = "textBoxSiteNamePrimary";
            // 
            // buttonTestPrimary
            // 
            this.buttonTestPrimary.AccessibleDescription = null;
            this.buttonTestPrimary.AccessibleName = null;
            resources.ApplyResources(this.buttonTestPrimary, "buttonTestPrimary");
            this.buttonTestPrimary.BackgroundImage = null;
            this.buttonTestPrimary.Font = null;
            this.buttonTestPrimary.Name = "buttonTestPrimary";
            this.buttonTestPrimary.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // textBoxPassword2
            // 
            this.textBoxPassword2.AccessibleDescription = null;
            this.textBoxPassword2.AccessibleName = null;
            resources.ApplyResources(this.textBoxPassword2, "textBoxPassword2");
            this.textBoxPassword2.BackgroundImage = null;
            this.textBoxPassword2.Font = null;
            this.textBoxPassword2.Name = "textBoxPassword2";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.AccessibleDescription = null;
            this.textBoxPassword.AccessibleName = null;
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.BackgroundImage = null;
            this.textBoxPassword.Font = null;
            this.textBoxPassword.Name = "textBoxPassword";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.AccessibleDescription = null;
            this.textBoxUsername.AccessibleName = null;
            resources.ApplyResources(this.textBoxUsername, "textBoxUsername");
            this.textBoxUsername.BackgroundImage = null;
            this.textBoxUsername.Font = null;
            this.textBoxUsername.Name = "textBoxUsername";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = null;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.AccessibleDescription = null;
            this.tabPage1.AccessibleName = null;
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.BackgroundImage = null;
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.textBoxSiteNameSecondary);
            this.tabPage1.Controls.Add(this.buttonTestSecondary);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.textBoxPassword);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.textBoxUsername);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBoxSiteNamePrimary);
            this.tabPage1.Controls.Add(this.buttonTestPrimary);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.textBoxPassword2);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // textBoxSiteNameSecondary
            // 
            this.textBoxSiteNameSecondary.AccessibleDescription = null;
            this.textBoxSiteNameSecondary.AccessibleName = null;
            resources.ApplyResources(this.textBoxSiteNameSecondary, "textBoxSiteNameSecondary");
            this.textBoxSiteNameSecondary.BackgroundImage = null;
            this.textBoxSiteNameSecondary.Font = null;
            this.textBoxSiteNameSecondary.Name = "textBoxSiteNameSecondary";
            // 
            // buttonTestSecondary
            // 
            this.buttonTestSecondary.AccessibleDescription = null;
            this.buttonTestSecondary.AccessibleName = null;
            resources.ApplyResources(this.buttonTestSecondary, "buttonTestSecondary");
            this.buttonTestSecondary.BackgroundImage = null;
            this.buttonTestSecondary.Font = null;
            this.buttonTestSecondary.Name = "buttonTestSecondary";
            this.buttonTestSecondary.Click += new System.EventHandler(this.buttonTestSecondary_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.numericUpDownServerConnectionTimeout);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.textBoxServerIP);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.textBoxTcpPort);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // numericUpDownServerConnectionTimeout
            // 
            this.numericUpDownServerConnectionTimeout.AccessibleDescription = null;
            this.numericUpDownServerConnectionTimeout.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownServerConnectionTimeout, "numericUpDownServerConnectionTimeout");
            this.numericUpDownServerConnectionTimeout.Font = null;
            this.numericUpDownServerConnectionTimeout.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownServerConnectionTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownServerConnectionTimeout.Name = "numericUpDownServerConnectionTimeout";
            this.numericUpDownServerConnectionTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // textBoxServerIP
            // 
            this.textBoxServerIP.AccessibleDescription = null;
            this.textBoxServerIP.AccessibleName = null;
            resources.ApplyResources(this.textBoxServerIP, "textBoxServerIP");
            this.textBoxServerIP.BackgroundImage = null;
            this.textBoxServerIP.Font = null;
            this.textBoxServerIP.Name = "textBoxServerIP";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // textBoxTcpPort
            // 
            this.textBoxTcpPort.AccessibleDescription = null;
            this.textBoxTcpPort.AccessibleName = null;
            resources.ApplyResources(this.textBoxTcpPort, "textBoxTcpPort");
            this.textBoxTcpPort.BackgroundImage = null;
            this.textBoxTcpPort.Font = null;
            this.textBoxTcpPort.Name = "textBoxTcpPort";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // FormConfigService
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigService";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownServerConnectionTimeout)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		void Test(string url)
		{
			biz.autobase.sms.www.ServiceSend2 send = new SmsServiceServer.biz.autobase.sms.www.ServiceSend2(url);

			if(this.textBoxPassword.Text != init_password) 
			{
				PassCode = hash(this.textBoxPassword.Text);
			}
				
			int retn;
			
			try 
			{
				retn = send.PingTest(this.textBoxUsername.Text, PassCode);

				if(retn == 99999) 
				{
					MessageBox.Show("Connection Test O.K", "Test Result");
				}
				else if(retn == 1) 
				{
					MessageBox.Show("Error:Username or password is invalid.", "Test Result");
				}
				else 
				{
					MessageBox.Show("Connection Test Failed", "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception exception)
			{
				string msg;
				msg = String.Format("Connection Test Failed\nMessage={0}", exception.Message);
				MessageBox.Show(msg, "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonTest_Click(object sender, System.EventArgs e)
		{
			Test(ShareServerMain.sSiteNamePrimary);
		}

		byte[] StringToBytes(string buf)
		{
			byte[] b = new byte[buf.Length*2];

			for(int i = 0; i < buf.Length; i++) 
			{
				b[i*2+0] = (byte)(buf[i]/256);
				b[i*2+1] = (byte)(buf[i]%256);
			}

			return b;
		}

		byte[] hash(string org)
		{
			byte[] b = StringToBytes(org);

			SHA1 sha = new SHA1CryptoServiceProvider();
			byte[] result = sha.ComputeHash(b);

			return result;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(this.textBoxUsername.Text.Length == 0) 
			{
				MessageBox.Show("You must input the username.", "Username error");
				return;
			}
			if(this.textBoxPassword.Text.Length == 0) 
			{
				MessageBox.Show("You must input the password.", "Password error");
				return;
			}
			if(this.textBoxPassword.Text != this.textBoxPassword2.Text) 
			{
				MessageBox.Show("Password mismatched.", "Password error");
				return;
			}

			TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "SiteNamePrimary", this.textBoxSiteNamePrimary.Text);
			TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "SiteNameSecondary", this.textBoxSiteNameSecondary.Text);
			TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "Username", this.textBoxUsername.Text);

			if(this.textBoxPassword.Text != init_password) 
			{
				byte[] result = hash(this.textBoxPassword.Text);
				TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "PassCode", result);
			}

			TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "LocalServerPort", ConvertTool.ToInt32(this.textBoxTcpPort.Text));
			TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "ServerIP", this.textBoxServerIP.Text);
			TotalConfig.SaveRegAutoBaseConfig("SMSService", "Config", "ServerConnectTimeOut", ConvertTool.ToInt32(this.numericUpDownServerConnectionTimeout.Value));

			ShareServerMain.UnInit();
			ShareServerMain.LoadConfig();
			ShareServerMain.Init();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		string init_password = "_P_a_S_W_O_r_D_m_u_s_";
		byte[] PassCode = null;

		private void Form1_Load(object sender, System.EventArgs e)
		{
			PassCode = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "PassCode", PassCode);
			this.textBoxServerIP.Text = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "ServerIP", "127.0.0.1");
			this.numericUpDownServerConnectionTimeout.Value = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "ServerConnectTimeOut", 10);

			this.textBoxPassword.Text  = init_password;
			this.textBoxPassword2.Text = init_password;
			this.textBoxSiteNamePrimary.Text  = ShareServerMain.sSiteNamePrimary;
			this.textBoxSiteNameSecondary.Text  = ShareServerMain.sSiteNameSecondary;
			this.textBoxUsername.Text  = ShareServerMain.sUserName;
			this.textBoxTcpPort.Text   = ShareServerMain.nPortTcp.ToString();
		}

		private void buttonTestSecondary_Click(object sender, System.EventArgs e)
		{
			Test(ShareServerMain.sSiteNameSecondary);
		}
	}
}
