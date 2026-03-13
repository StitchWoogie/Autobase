using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NetClient
{
	/// <summary>
	/// Summary description for FormConfigEtc.
	/// </summary>
	public class FormConfigEtc : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxExchangeTagCurrent;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxExchangeAlarmStatus;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigEtc()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigEtc));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxExchangeTagCurrent = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxExchangeAlarmStatus = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
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
            // checkBoxExchangeTagCurrent
            // 
            this.checkBoxExchangeTagCurrent.AccessibleDescription = null;
            this.checkBoxExchangeTagCurrent.AccessibleName = null;
            resources.ApplyResources(this.checkBoxExchangeTagCurrent, "checkBoxExchangeTagCurrent");
            this.checkBoxExchangeTagCurrent.BackgroundImage = null;
            this.checkBoxExchangeTagCurrent.Font = null;
            this.checkBoxExchangeTagCurrent.Name = "checkBoxExchangeTagCurrent";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.checkBoxExchangeAlarmStatus);
            this.groupBox1.Controls.Add(this.checkBoxExchangeTagCurrent);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxExchangeAlarmStatus
            // 
            this.checkBoxExchangeAlarmStatus.AccessibleDescription = null;
            this.checkBoxExchangeAlarmStatus.AccessibleName = null;
            resources.ApplyResources(this.checkBoxExchangeAlarmStatus, "checkBoxExchangeAlarmStatus");
            this.checkBoxExchangeAlarmStatus.BackgroundImage = null;
            this.checkBoxExchangeAlarmStatus.Font = null;
            this.checkBoxExchangeAlarmStatus.Name = "checkBoxExchangeAlarmStatus";
            // 
            // FormConfigEtc
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigEtc";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigEtc_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigEtc_Load(object sender, System.EventArgs e)
		{
			this.checkBoxExchangeTagCurrent.Checked = (configStruct.bCommunicationValue == 1);
			this.checkBoxExchangeAlarmStatus.Checked = (configStruct.bExchangeAlarmStatus == 1);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			configStruct.bCommunicationValue = this.checkBoxExchangeTagCurrent.Checked ? (sbyte)1 : (sbyte)0;
			AutoLibLocal.SystemStatusMemory.SetDI(AutoLibLocal.SSMDI.CommunicationValue, configStruct.bCommunicationValue);
			configStruct.bExchangeAlarmStatus = this.checkBoxExchangeAlarmStatus.Checked ? (sbyte)1 : (sbyte)0;
			AutoLibLocal.SystemStatusMemory.SetDI(AutoLibLocal.SSMDI.bExchangeAlarmStatus, configStruct.bExchangeAlarmStatus);
			
			DialogResult = DialogResult.OK;

			Close();
		}
	}
}
