using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LinePrinter
{
	/// <summary>
	/// Summary description for FormConfigDisplayFormat.
	/// </summary>
	public class FormConfigDisplayFormat : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonDisplayType0;
		private System.Windows.Forms.RadioButton radioButtonDisplayType1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonFormatDate;
		private System.Windows.Forms.Button buttonFormatDate2;
		private System.Windows.Forms.Button buttonFormatTime;
		private System.Windows.Forms.Button buttonFormatTag;
		private System.Windows.Forms.Button buttonFormatDesc;
		private System.Windows.Forms.Button buttonFormatMsg;
		private System.Windows.Forms.Button buttonFormatMsg40;
		private System.Windows.Forms.TextBox textBoxEdit;
		private System.Windows.Forms.Button buttonDefault;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigDisplayFormat()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigDisplayFormat));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisplayType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplayType0 = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxEdit = new System.Windows.Forms.TextBox();
            this.buttonFormatMsg40 = new System.Windows.Forms.Button();
            this.buttonFormatMsg = new System.Windows.Forms.Button();
            this.buttonFormatDesc = new System.Windows.Forms.Button();
            this.buttonFormatTag = new System.Windows.Forms.Button();
            this.buttonFormatTime = new System.Windows.Forms.Button();
            this.buttonFormatDate2 = new System.Windows.Forms.Button();
            this.buttonFormatDate = new System.Windows.Forms.Button();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonDisplayType1);
            this.groupBox1.Controls.Add(this.radioButtonDisplayType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonDisplayType1
            // 
            this.radioButtonDisplayType1.AccessibleDescription = null;
            this.radioButtonDisplayType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDisplayType1, "radioButtonDisplayType1");
            this.radioButtonDisplayType1.BackgroundImage = null;
            this.radioButtonDisplayType1.Font = null;
            this.radioButtonDisplayType1.Name = "radioButtonDisplayType1";
            this.radioButtonDisplayType1.CheckedChanged += new System.EventHandler(this.radioButtonDisplayType1_CheckedChanged);
            // 
            // radioButtonDisplayType0
            // 
            this.radioButtonDisplayType0.AccessibleDescription = null;
            this.radioButtonDisplayType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDisplayType0, "radioButtonDisplayType0");
            this.radioButtonDisplayType0.BackgroundImage = null;
            this.radioButtonDisplayType0.Font = null;
            this.radioButtonDisplayType0.Name = "radioButtonDisplayType0";
            this.radioButtonDisplayType0.CheckedChanged += new System.EventHandler(this.radioButtonDisplayType0_CheckedChanged);
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
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.textBoxEdit);
            this.groupBox2.Controls.Add(this.buttonFormatMsg40);
            this.groupBox2.Controls.Add(this.buttonFormatMsg);
            this.groupBox2.Controls.Add(this.buttonFormatDesc);
            this.groupBox2.Controls.Add(this.buttonFormatTag);
            this.groupBox2.Controls.Add(this.buttonFormatTime);
            this.groupBox2.Controls.Add(this.buttonFormatDate2);
            this.groupBox2.Controls.Add(this.buttonFormatDate);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxEdit
            // 
            this.textBoxEdit.AccessibleDescription = null;
            this.textBoxEdit.AccessibleName = null;
            resources.ApplyResources(this.textBoxEdit, "textBoxEdit");
            this.textBoxEdit.BackgroundImage = null;
            this.textBoxEdit.Font = null;
            this.textBoxEdit.Name = "textBoxEdit";
            // 
            // buttonFormatMsg40
            // 
            this.buttonFormatMsg40.AccessibleDescription = null;
            this.buttonFormatMsg40.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatMsg40, "buttonFormatMsg40");
            this.buttonFormatMsg40.BackgroundImage = null;
            this.buttonFormatMsg40.Font = null;
            this.buttonFormatMsg40.Name = "buttonFormatMsg40";
            this.buttonFormatMsg40.Click += new System.EventHandler(this.buttonFormatMsg40_Click);
            // 
            // buttonFormatMsg
            // 
            this.buttonFormatMsg.AccessibleDescription = null;
            this.buttonFormatMsg.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatMsg, "buttonFormatMsg");
            this.buttonFormatMsg.BackgroundImage = null;
            this.buttonFormatMsg.Font = null;
            this.buttonFormatMsg.Name = "buttonFormatMsg";
            this.buttonFormatMsg.Click += new System.EventHandler(this.buttonFormatMsg_Click);
            // 
            // buttonFormatDesc
            // 
            this.buttonFormatDesc.AccessibleDescription = null;
            this.buttonFormatDesc.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatDesc, "buttonFormatDesc");
            this.buttonFormatDesc.BackgroundImage = null;
            this.buttonFormatDesc.Font = null;
            this.buttonFormatDesc.Name = "buttonFormatDesc";
            this.buttonFormatDesc.Click += new System.EventHandler(this.buttonFormatDesc_Click);
            // 
            // buttonFormatTag
            // 
            this.buttonFormatTag.AccessibleDescription = null;
            this.buttonFormatTag.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatTag, "buttonFormatTag");
            this.buttonFormatTag.BackgroundImage = null;
            this.buttonFormatTag.Font = null;
            this.buttonFormatTag.Name = "buttonFormatTag";
            this.buttonFormatTag.Click += new System.EventHandler(this.buttonFormatTag_Click);
            // 
            // buttonFormatTime
            // 
            this.buttonFormatTime.AccessibleDescription = null;
            this.buttonFormatTime.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatTime, "buttonFormatTime");
            this.buttonFormatTime.BackgroundImage = null;
            this.buttonFormatTime.Font = null;
            this.buttonFormatTime.Name = "buttonFormatTime";
            this.buttonFormatTime.Click += new System.EventHandler(this.buttonFormatTime_Click);
            // 
            // buttonFormatDate2
            // 
            this.buttonFormatDate2.AccessibleDescription = null;
            this.buttonFormatDate2.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatDate2, "buttonFormatDate2");
            this.buttonFormatDate2.BackgroundImage = null;
            this.buttonFormatDate2.Font = null;
            this.buttonFormatDate2.Name = "buttonFormatDate2";
            this.buttonFormatDate2.Click += new System.EventHandler(this.buttonFormatDate2_Click);
            // 
            // buttonFormatDate
            // 
            this.buttonFormatDate.AccessibleDescription = null;
            this.buttonFormatDate.AccessibleName = null;
            resources.ApplyResources(this.buttonFormatDate, "buttonFormatDate");
            this.buttonFormatDate.BackgroundImage = null;
            this.buttonFormatDate.Font = null;
            this.buttonFormatDate.Name = "buttonFormatDate";
            this.buttonFormatDate.Click += new System.EventHandler(this.buttonFormatDate_Click);
            // 
            // buttonDefault
            // 
            this.buttonDefault.AccessibleDescription = null;
            this.buttonDefault.AccessibleName = null;
            resources.ApplyResources(this.buttonDefault, "buttonDefault");
            this.buttonDefault.BackgroundImage = null;
            this.buttonDefault.Font = null;
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // FormConfigDisplayFormat
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigDisplayFormat";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigDisplayFormat_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		int GetRadioType()
		{
			int type = 0;
			if(this.radioButtonDisplayType0.Checked)		type = 0;
			else if(this.radioButtonDisplayType1.Checked)	type = 1;

			return type;
		}

		void EnableDisable()
		{
			bool flag = (GetRadioType() == 1);
			this.buttonFormatDate.Enabled = flag;
			this.buttonFormatDate2.Enabled = flag;
			this.buttonFormatTime.Enabled = flag;
			this.buttonFormatTag.Enabled = flag;
			this.buttonFormatDesc.Enabled = flag;
			this.buttonFormatMsg.Enabled = flag;
			this.buttonFormatMsg40.Enabled = flag;
			this.textBoxEdit.Enabled = flag;
		}

		private void FormConfigDisplayFormat_Load(object sender, System.EventArgs e)
		{
			this.radioButtonDisplayType0.Checked = (Config.space_type == 0);
			this.radioButtonDisplayType1.Checked = (Config.space_type == 1);
			this.textBoxEdit.Text = Config.str;

			EnableDisable();
		}

		private void radioButtonDisplayType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonDisplayType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void buttonFormatDate_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Date]";
		}

		private void buttonFormatDate2_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Date/]";
		}

		private void buttonFormatTime_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Time]";
		}

		private void buttonFormatTag_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Tag]";
		}

		private void buttonFormatDesc_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Desc]";
		}

		private void buttonFormatMsg_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Msg]";
		}

		private void buttonFormatMsg40_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text += "&[Msg40]";
		}

		private void buttonDefault_Click(object sender, System.EventArgs e)
		{
			this.textBoxEdit.Text = "&[Date] &[Time] &[Tag] &[Desc] &[Msg]";
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Config.space_type = this.GetRadioType();
			Config.str = this.textBoxEdit.Text;

			Config.Save();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

