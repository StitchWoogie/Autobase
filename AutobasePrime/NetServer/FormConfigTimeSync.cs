using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetCommon;
using NetTools;

namespace NetServer
{
	/// <summary>
	/// Summary description for FormConfigTimeSync.
	/// </summary>
	public class FormConfigTimeSync : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonSetNow;
		private System.Windows.Forms.CheckBox checkBoxActive;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numericUpDownHour;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownMinute;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonCycle0;
		private System.Windows.Forms.RadioButton radioButtonCycle1;
		private System.Windows.Forms.RadioButton radioButtonCycle2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigTimeSync()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigTimeSync));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSetNow = new System.Windows.Forms.Button();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownMinute = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownHour = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonCycle2 = new System.Windows.Forms.RadioButton();
            this.radioButtonCycle1 = new System.Windows.Forms.RadioButton();
            this.radioButtonCycle0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHour)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            // 
            // buttonSetNow
            // 
            this.buttonSetNow.AccessibleDescription = null;
            this.buttonSetNow.AccessibleName = null;
            resources.ApplyResources(this.buttonSetNow, "buttonSetNow");
            this.buttonSetNow.BackgroundImage = null;
            this.buttonSetNow.Font = null;
            this.buttonSetNow.Name = "buttonSetNow";
            this.buttonSetNow.Click += new System.EventHandler(this.buttonSetNow_Click);
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.AccessibleDescription = null;
            this.checkBoxActive.AccessibleName = null;
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.BackgroundImage = null;
            this.checkBoxActive.Font = null;
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.numericUpDownMinute);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownHour);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownMinute
            // 
            this.numericUpDownMinute.AccessibleDescription = null;
            this.numericUpDownMinute.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownMinute, "numericUpDownMinute");
            this.numericUpDownMinute.Font = null;
            this.numericUpDownMinute.Name = "numericUpDownMinute";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownHour
            // 
            this.numericUpDownHour.AccessibleDescription = null;
            this.numericUpDownHour.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownHour, "numericUpDownHour");
            this.numericUpDownHour.Font = null;
            this.numericUpDownHour.Name = "numericUpDownHour";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButtonCycle2);
            this.groupBox2.Controls.Add(this.radioButtonCycle1);
            this.groupBox2.Controls.Add(this.radioButtonCycle0);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonCycle2
            // 
            this.radioButtonCycle2.AccessibleDescription = null;
            this.radioButtonCycle2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonCycle2, "radioButtonCycle2");
            this.radioButtonCycle2.BackgroundImage = null;
            this.radioButtonCycle2.Font = null;
            this.radioButtonCycle2.Name = "radioButtonCycle2";
            this.radioButtonCycle2.CheckedChanged += new System.EventHandler(this.radioButtonCycle2_CheckedChanged);
            // 
            // radioButtonCycle1
            // 
            this.radioButtonCycle1.AccessibleDescription = null;
            this.radioButtonCycle1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonCycle1, "radioButtonCycle1");
            this.radioButtonCycle1.BackgroundImage = null;
            this.radioButtonCycle1.Font = null;
            this.radioButtonCycle1.Name = "radioButtonCycle1";
            this.radioButtonCycle1.CheckedChanged += new System.EventHandler(this.radioButtonCycle1_CheckedChanged);
            // 
            // radioButtonCycle0
            // 
            this.radioButtonCycle0.AccessibleDescription = null;
            this.radioButtonCycle0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonCycle0, "radioButtonCycle0");
            this.radioButtonCycle0.BackgroundImage = null;
            this.radioButtonCycle0.Font = null;
            this.radioButtonCycle0.Name = "radioButtonCycle0";
            this.radioButtonCycle0.CheckedChanged += new System.EventHandler(this.radioButtonCycle0_CheckedChanged);
            // 
            // FormConfigTimeSync
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonSetNow);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxActive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigTimeSync";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigTimeSync_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHour)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigTimeSync_Load(object sender, System.EventArgs e)
		{
			this.checkBoxActive.Checked = configStruct.bTimeSyncActive == 1;

			this.numericUpDownHour.Value = configStruct.nTimeSyncHour;
			this.numericUpDownMinute.Value = configStruct.nTimeSyncMin;

			this.radioButtonCycle0.Checked = (configStruct.nTimeSyncWhen == 0);
			this.radioButtonCycle1.Checked = (configStruct.nTimeSyncWhen == 1);
			this.radioButtonCycle2.Checked = (configStruct.nTimeSyncWhen == 2);

			EnableDisable();
		}

		int GetRadioWhen()
		{
			int nTimeSyncWhen;

			if(this.radioButtonCycle0.Checked)		nTimeSyncWhen = 0;
			else if(this.radioButtonCycle1.Checked)	nTimeSyncWhen = 1;
			else if(this.radioButtonCycle2.Checked)	nTimeSyncWhen = 2;
			else									nTimeSyncWhen = 0;

			return nTimeSyncWhen;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			configStruct.bTimeSyncActive = this.checkBoxActive.Checked ? (sbyte)1 : (sbyte)0;

			configStruct.nTimeSyncHour = ConvertTool.ToInt32(this.numericUpDownHour.Value);
			configStruct.nTimeSyncMin = ConvertTool.ToInt32(this.numericUpDownMinute.Value);

			configStruct.nTimeSyncWhen = GetRadioWhen();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonSetNow_Click(object sender, System.EventArgs e)
		{
			SendTimeSyncSignal();
		}

		private void radioButtonCycle0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonCycle1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonCycle2_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		void EnableDisable()
		{
			bool active = this.checkBoxActive.Checked;
			bool flag_hour = false, flag_min = false;
			int nWhen = GetRadioWhen();

			if(active) 
			{
				if(nWhen == 0) 
				{
					flag_hour = true;
					flag_min  = true;
				}
				else 
				{
					flag_min = true;
				}
			}

			this.radioButtonCycle0.Enabled = active;
			this.radioButtonCycle1.Enabled = active;
			this.radioButtonCycle2.Enabled = active;

			this.numericUpDownHour.Enabled = flag_hour;
			this.numericUpDownMinute.Enabled = flag_min;
		}

		private void checkBoxActive_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		static void SendTimeSyncSignal()
		{
			string buf;
			DateTime t = DateTime.Now;

			buf = String.Format("Year={0},Mon={1},Day={2},Hour={3},Min={4},Sec={5},", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);

			NetWorkProtocolSend send = new NetWorkProtocolSend();

			ushort trans = NetWorkProtocolSend.GetTransaction();

			send.MakeBlock(EnumNetworkCommand.TIME_SYNC, trans, buf);

			NetLib.SendCodeToAllExceptMe(EnumNetworkCommand.TIME_SYNC, trans, send.bufSend, send.nBufCount);
		}

		static bool bTimeSyncRunFlag = false;
		static int old_hour = 0;
		static int old_min = 0;
		static int old_day;

		public static void CheckNetworkTimeSync()
		{
			if(configStruct.bTimeSyncActive == 0)	return;

			DateTime t = DateTime.Now;

			if(configStruct.nTimeSyncWhen == 1) 
			{	// 매시

				if(bTimeSyncRunFlag == true) 
				{
					if(t.Hour != old_hour) 
					{
						bTimeSyncRunFlag = false;
						old_hour = t.Hour;
					}
				}

				if(bTimeSyncRunFlag	== false) 
				{
					if(t.Minute >= configStruct.nTimeSyncMin) 
					{
						SendTimeSyncSignal();			
						bTimeSyncRunFlag = true;
						old_hour = t.Hour;
					}
				}
			}
			else if(configStruct.nTimeSyncWhen == 2) 
			{	// 분마다

				if(configStruct.nTimeSyncMin != 0) 
				{	// devide by zero
					if(bTimeSyncRunFlag == true) 
					{
						if(t.Minute != old_min) 
						{
							bTimeSyncRunFlag = false;
							old_min = t.Minute;
						}
					}

					if(bTimeSyncRunFlag	== false) 
					{
						if((t.Minute%configStruct.nTimeSyncMin) == 0) 
						{
							SendTimeSyncSignal();			
							bTimeSyncRunFlag = true;
							old_min = t.Minute;
						}
					}	
				}
			}
			else 
			{
				if(bTimeSyncRunFlag == true) 
				{
					if(t.Day != old_day) 
					{
						bTimeSyncRunFlag = false;
						old_day = t.Day;
					}
				}

				if(bTimeSyncRunFlag	== false) 
				{
					if(t.Hour >= configStruct.nTimeSyncHour &&
						t.Minute >= configStruct.nTimeSyncMin) 
					{
						SendTimeSyncSignal();			
						bTimeSyncRunFlag = true;
						old_day = t.Day;
					}
				}
			}
		}
	}
}
