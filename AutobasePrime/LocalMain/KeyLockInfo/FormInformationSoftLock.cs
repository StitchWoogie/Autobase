using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal.KeyLock;
using NetTools.OldDefine;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormInformationSoftLock.
	/// </summary>
	public class FormInformationSoftLock : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxFrom;
		private System.Windows.Forms.TextBox textBoxTo;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBoxRemain;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.TextBox textBoxAgency;
		private System.Windows.Forms.TextBox textBoxAgencyManager;
		private System.Windows.Forms.TextBox textBoxSite;
		private System.Windows.Forms.TextBox textBoxSystem;
		private System.Windows.Forms.TextBox textBoxSiteManager;
		private System.Windows.Forms.TextBox textBoxTelephone;
		private System.Windows.Forms.TextBox textBoxSerialNumber;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormInformationSoftLock()
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
				if(components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInformationSoftLock));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxRemain = new System.Windows.Forms.TextBox();
            this.textBoxTo = new System.Windows.Forms.TextBox();
            this.textBoxFrom = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxAgency = new System.Windows.Forms.TextBox();
            this.textBoxAgencyManager = new System.Windows.Forms.TextBox();
            this.textBoxSite = new System.Windows.Forms.TextBox();
            this.textBoxSystem = new System.Windows.Forms.TextBox();
            this.textBoxSiteManager = new System.Windows.Forms.TextBox();
            this.textBoxTelephone = new System.Windows.Forms.TextBox();
            this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxRemain);
            this.groupBox1.Controls.Add(this.textBoxTo);
            this.groupBox1.Controls.Add(this.textBoxFrom);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxRemain
            // 
            this.textBoxRemain.AccessibleDescription = null;
            this.textBoxRemain.AccessibleName = null;
            resources.ApplyResources(this.textBoxRemain, "textBoxRemain");
            this.textBoxRemain.BackgroundImage = null;
            this.textBoxRemain.Font = null;
            this.textBoxRemain.Name = "textBoxRemain";
            this.textBoxRemain.ReadOnly = true;
            // 
            // textBoxTo
            // 
            this.textBoxTo.AccessibleDescription = null;
            this.textBoxTo.AccessibleName = null;
            resources.ApplyResources(this.textBoxTo, "textBoxTo");
            this.textBoxTo.BackgroundImage = null;
            this.textBoxTo.Font = null;
            this.textBoxTo.Name = "textBoxTo";
            this.textBoxTo.ReadOnly = true;
            // 
            // textBoxFrom
            // 
            this.textBoxFrom.AccessibleDescription = null;
            this.textBoxFrom.AccessibleName = null;
            resources.ApplyResources(this.textBoxFrom, "textBoxFrom");
            this.textBoxFrom.BackgroundImage = null;
            this.textBoxFrom.Font = null;
            this.textBoxFrom.Name = "textBoxFrom";
            this.textBoxFrom.ReadOnly = true;
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            // 
            // textBoxAgency
            // 
            this.textBoxAgency.AccessibleDescription = null;
            this.textBoxAgency.AccessibleName = null;
            resources.ApplyResources(this.textBoxAgency, "textBoxAgency");
            this.textBoxAgency.BackgroundImage = null;
            this.textBoxAgency.Font = null;
            this.textBoxAgency.Name = "textBoxAgency";
            this.textBoxAgency.ReadOnly = true;
            // 
            // textBoxAgencyManager
            // 
            this.textBoxAgencyManager.AccessibleDescription = null;
            this.textBoxAgencyManager.AccessibleName = null;
            resources.ApplyResources(this.textBoxAgencyManager, "textBoxAgencyManager");
            this.textBoxAgencyManager.BackgroundImage = null;
            this.textBoxAgencyManager.Font = null;
            this.textBoxAgencyManager.Name = "textBoxAgencyManager";
            this.textBoxAgencyManager.ReadOnly = true;
            // 
            // textBoxSite
            // 
            this.textBoxSite.AccessibleDescription = null;
            this.textBoxSite.AccessibleName = null;
            resources.ApplyResources(this.textBoxSite, "textBoxSite");
            this.textBoxSite.BackgroundImage = null;
            this.textBoxSite.Font = null;
            this.textBoxSite.Name = "textBoxSite";
            this.textBoxSite.ReadOnly = true;
            // 
            // textBoxSystem
            // 
            this.textBoxSystem.AccessibleDescription = null;
            this.textBoxSystem.AccessibleName = null;
            resources.ApplyResources(this.textBoxSystem, "textBoxSystem");
            this.textBoxSystem.BackgroundImage = null;
            this.textBoxSystem.Font = null;
            this.textBoxSystem.Name = "textBoxSystem";
            this.textBoxSystem.ReadOnly = true;
            // 
            // textBoxSiteManager
            // 
            this.textBoxSiteManager.AccessibleDescription = null;
            this.textBoxSiteManager.AccessibleName = null;
            resources.ApplyResources(this.textBoxSiteManager, "textBoxSiteManager");
            this.textBoxSiteManager.BackgroundImage = null;
            this.textBoxSiteManager.Font = null;
            this.textBoxSiteManager.Name = "textBoxSiteManager";
            this.textBoxSiteManager.ReadOnly = true;
            // 
            // textBoxTelephone
            // 
            this.textBoxTelephone.AccessibleDescription = null;
            this.textBoxTelephone.AccessibleName = null;
            resources.ApplyResources(this.textBoxTelephone, "textBoxTelephone");
            this.textBoxTelephone.BackgroundImage = null;
            this.textBoxTelephone.Font = null;
            this.textBoxTelephone.Name = "textBoxTelephone";
            this.textBoxTelephone.ReadOnly = true;
            // 
            // textBoxSerialNumber
            // 
            this.textBoxSerialNumber.AccessibleDescription = null;
            this.textBoxSerialNumber.AccessibleName = null;
            resources.ApplyResources(this.textBoxSerialNumber, "textBoxSerialNumber");
            this.textBoxSerialNumber.BackgroundImage = null;
            this.textBoxSerialNumber.Font = null;
            this.textBoxSerialNumber.Name = "textBoxSerialNumber";
            this.textBoxSerialNumber.ReadOnly = true;
            // 
            // FormInformationSoftLock
            // 
            this.AcceptButton = this.buttonClose;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.textBoxSerialNumber);
            this.Controls.Add(this.textBoxTelephone);
            this.Controls.Add(this.textBoxSiteManager);
            this.Controls.Add(this.textBoxSystem);
            this.Controls.Add(this.textBoxSite);
            this.Controls.Add(this.textBoxAgencyManager);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAgency);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInformationSoftLock";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormInformationSoftLock_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FormInformationSoftLock_Load(object sender, System.EventArgs e)
		{
			SYSTEMTIME tFr = new SYSTEMTIME(), tTo = new SYSTEMTIME();

			this.textBoxAgency.Text = KeyLock.sSoftLockManagementCompany;
			this.textBoxAgencyManager.Text = KeyLock.sSoftLockManagement;
			this.textBoxSite.Text = KeyLock.sSoftLockInstallPlace;
			this.textBoxSiteManager.Text = KeyLock.sSoftLockSystemUser;
			this.textBoxSystem.Text = KeyLock.sSoftLockSystem;
			this.textBoxTelephone.Text = KeyLock.sSoftLockSystemTelephone;
			this.textBoxSerialNumber.Text = KeyLock.sSerialNumber;

			DateTime t;

			t = new DateTime(KeyLock.tSoftLockFrom.wYear, KeyLock.tSoftLockFrom.wMonth, KeyLock.tSoftLockFrom.wDay);
			this.textBoxFrom.Text = t.ToString("d");
			t = new DateTime(KeyLock.tSoftLockTo.wYear, KeyLock.tSoftLockTo.wMonth, KeyLock.tSoftLockTo.wDay, 23, 59,59);
			this.textBoxTo.Text = t.ToString("d");

			DateTime today = DateTime.Now;

			TimeSpan span = t-today;

			if(Tools.IsLangKorean()) 
			{
				this.textBoxRemain.Text = String.Format("사용 허가 기간이 {0:F1}일 남았습니다.", span.TotalDays);
			}
			else 
			{
				this.textBoxRemain.Text = String.Format("{0:F1} Days remain.", span.TotalDays);
			}
		}

		public static void DisplayLockInfoOnStartUp()
		{
            if (SharedLocalMain.bTestMode == true) return;
			if(KeyLock.bExistLocalKey == false)		return;
			if(!KeyLock.IsSoftLock()) return;

			FormInformationSoftLock dialog = new FormInformationSoftLock();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(Form.ActiveForm);
		}
	}

}


