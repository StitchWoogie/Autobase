using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_EtcAlarmDataAddFieldDlg.
	/// </summary>
	public class edit_EtcAlarmDataAddFieldDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public bool bOkFlag = false;
		private System.Windows.Forms.RadioButton radioButton_OrderNo;
		private System.Windows.Forms.RadioButton radioButton_Priority;
		private System.Windows.Forms.RadioButton radioButton_Content;
		private System.Windows.Forms.RadioButton radioButton_Description;
		private System.Windows.Forms.RadioButton radioButton_tagName;
		private System.Windows.Forms.RadioButton radioButton_DateTime;
		public int	nColumnPos;

		public edit_EtcAlarmDataAddFieldDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_EtcAlarmDataAddFieldDlg));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_OrderNo = new System.Windows.Forms.RadioButton();
            this.radioButton_Priority = new System.Windows.Forms.RadioButton();
            this.radioButton_Content = new System.Windows.Forms.RadioButton();
            this.radioButton_Description = new System.Windows.Forms.RadioButton();
            this.radioButton_tagName = new System.Windows.Forms.RadioButton();
            this.radioButton_DateTime = new System.Windows.Forms.RadioButton();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_OrderNo);
            this.groupBox1.Controls.Add(this.radioButton_Priority);
            this.groupBox1.Controls.Add(this.radioButton_Content);
            this.groupBox1.Controls.Add(this.radioButton_Description);
            this.groupBox1.Controls.Add(this.radioButton_tagName);
            this.groupBox1.Controls.Add(this.radioButton_DateTime);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButton_OrderNo
            // 
            resources.ApplyResources(this.radioButton_OrderNo, "radioButton_OrderNo");
            this.radioButton_OrderNo.Name = "radioButton_OrderNo";
            // 
            // radioButton_Priority
            // 
            resources.ApplyResources(this.radioButton_Priority, "radioButton_Priority");
            this.radioButton_Priority.Name = "radioButton_Priority";
            // 
            // radioButton_Content
            // 
            resources.ApplyResources(this.radioButton_Content, "radioButton_Content");
            this.radioButton_Content.Name = "radioButton_Content";
            // 
            // radioButton_Description
            // 
            resources.ApplyResources(this.radioButton_Description, "radioButton_Description");
            this.radioButton_Description.Name = "radioButton_Description";
            // 
            // radioButton_tagName
            // 
            resources.ApplyResources(this.radioButton_tagName, "radioButton_tagName");
            this.radioButton_tagName.Name = "radioButton_tagName";
            // 
            // radioButton_DateTime
            // 
            resources.ApplyResources(this.radioButton_DateTime, "radioButton_DateTime");
            this.radioButton_DateTime.Name = "radioButton_DateTime";
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // edit_EtcAlarmDataAddFieldDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_EtcAlarmDataAddFieldDlg";
            this.Load += new System.EventHandler(this.edit_EtcAlarmDataAddFieldDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(this.radioButton_DateTime.Checked) this.nColumnPos = 0;
			else if(this.radioButton_tagName.Checked) this.nColumnPos = 1;
			else if(this.radioButton_Description.Checked) this.nColumnPos = 2;
			else if(this.radioButton_Content.Checked) this.nColumnPos = 3;
			else if(this.radioButton_Priority.Checked) this.nColumnPos = 4;
			else if(this.radioButton_OrderNo.Checked) this.nColumnPos = 5;
			else 			
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("표시할 경보 컬럼이름을 입력하여야 합니다.", "입력오류");
				else
					MessageBox.Show("Please Input Alarm Column Name.", "Input error");
				return;				
			}
			bOkFlag = true;
			Close();
		}

		private void edit_EtcAlarmDataAddFieldDlg_Load(object sender, System.EventArgs e)
		{
			if(nColumnPos == 1) radioButton_tagName.Checked = true;
			if(nColumnPos == 2) radioButton_Description.Checked = true;
			if(nColumnPos == 3) radioButton_Content.Checked = true;
			if(nColumnPos == 4) radioButton_Priority.Checked = true;
			if(nColumnPos == 5) radioButton_OrderNo.Checked = true;
			else				radioButton_DateTime.Checked = true;		
		}
	}
}
