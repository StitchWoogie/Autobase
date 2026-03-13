using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for ReportMainDataSortSelectDlg.
	/// </summary>
	public class ReportMainDataSortSelectDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button_OK;
		public System.Windows.Forms.ListBox m_list;
		public System.Windows.Forms.RadioButton radioButton_DB_Data;
		public System.Windows.Forms.RadioButton radioButton_Basic_Analog;
		public System.Windows.Forms.RadioButton radioButton_Basic_Digital;
		public System.Windows.Forms.RadioButton radioButton_Etc;
		private System.Windows.Forms.Button button_CANCEL;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int				nMainDataNum;
		public int				nSubDataNum;
		public ArrayList		memberArr;
		public bool				bOkFlag = false;

		public ReportMainDataSortSelectDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportMainDataSortSelectDlg));
            this.m_list = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_Etc = new System.Windows.Forms.RadioButton();
            this.radioButton_Basic_Digital = new System.Windows.Forms.RadioButton();
            this.radioButton_Basic_Analog = new System.Windows.Forms.RadioButton();
            this.radioButton_DB_Data = new System.Windows.Forms.RadioButton();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_list
            // 
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.Name = "m_list";
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_Etc);
            this.groupBox1.Controls.Add(this.radioButton_Basic_Digital);
            this.groupBox1.Controls.Add(this.radioButton_Basic_Analog);
            this.groupBox1.Controls.Add(this.radioButton_DB_Data);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButton_Etc
            // 
            resources.ApplyResources(this.radioButton_Etc, "radioButton_Etc");
            this.radioButton_Etc.Name = "radioButton_Etc";
            this.radioButton_Etc.CheckedChanged += new System.EventHandler(this.radioButton_Etc_CheckedChanged);
            // 
            // radioButton_Basic_Digital
            // 
            resources.ApplyResources(this.radioButton_Basic_Digital, "radioButton_Basic_Digital");
            this.radioButton_Basic_Digital.Name = "radioButton_Basic_Digital";
            this.radioButton_Basic_Digital.CheckedChanged += new System.EventHandler(this.radioButton_Basic_Digital_CheckedChanged);
            // 
            // radioButton_Basic_Analog
            // 
            resources.ApplyResources(this.radioButton_Basic_Analog, "radioButton_Basic_Analog");
            this.radioButton_Basic_Analog.Name = "radioButton_Basic_Analog";
            this.radioButton_Basic_Analog.CheckedChanged += new System.EventHandler(this.radioButton_Basic_Analog_CheckedChanged);
            // 
            // radioButton_DB_Data
            // 
            resources.ApplyResources(this.radioButton_DB_Data, "radioButton_DB_Data");
            this.radioButton_DB_Data.Name = "radioButton_DB_Data";
            this.radioButton_DB_Data.CheckedChanged += new System.EventHandler(this.radioButton_DB_Data_CheckedChanged);
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // ReportMainDataSortSelectDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportMainDataSortSelectDlg";
            this.Load += new System.EventHandler(this.ReportMainDataSortSelectDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		bool checkCurrentMainDataIsExist(string sMainBuf, int num)
		{
			int					i;
			
			if(num < 0 || num > 3) return false;
			for(i = 0; i < BasicRptTool.data[num].dataTypeMaxCount; i++) 
			{
				if(sMainBuf == BasicRptTool.data[nMainDataNum].sMainDataTypeBuf[i]) return true;				
			}
			return false;			
		}

		void setCurrentMainItemFill(bool bStart)
		{
			int					i;

			if(!bStart) 
			{
				if(radioButton_Etc.Checked) nMainDataNum = 3;
				else if(radioButton_Basic_Digital.Checked) nMainDataNum = 2;
				else if(radioButton_Basic_Analog.Checked) nMainDataNum = 1;
				else nMainDataNum = 0;
			}

			if(memberArr == null) return;

			m_list.Items.Clear();
			for(i = 0; i < BasicRptTool.data[nMainDataNum % 4].dataTypeMaxCount; i++) 
			{
				m_list.Items.Add(BasicRptTool.data[nMainDataNum].sMainDataTypeBuf[i]);
			}
		}
	
		
		private void ReportMainDataSortSelectDlg_Load(object sender, System.EventArgs e)
		{
			if(nMainDataNum > 3 && nMainDataNum < 0) nMainDataNum = 0;			
			setCurrentMainItemFill(true);
			switch(nMainDataNum) 
			{
				case 1 : this.radioButton_Basic_Analog.Checked = true; break;
				case 2 : this.radioButton_Basic_Digital.Checked = true; break;
				case 3 : this.radioButton_Etc.Checked = true; break;
				default: this.radioButton_DB_Data.Checked = true; break;
			}
			m_list.SelectedItem = BasicRptTool.data[nMainDataNum % 4].sMainDataTypeBuf[nSubDataNum % BasicRptTool.data[nMainDataNum % 4].dataTypeMaxCount];
		}

		private void radioButton_DB_Data_CheckedChanged(object sender, System.EventArgs e)
		{
			setCurrentMainItemFill(false);
		}

		private void radioButton_Basic_Analog_CheckedChanged(object sender, System.EventArgs e)
		{
			setCurrentMainItemFill(false);
		}

		private void radioButton_Basic_Digital_CheckedChanged(object sender, System.EventArgs e)
		{
			setCurrentMainItemFill(false);
		}

		private void radioButton_Etc_CheckedChanged(object sender, System.EventArgs e)
		{
			setCurrentMainItemFill(false);
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count > 0) 
			{
				bOkFlag = true;
				Close();
			}
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count <= 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("선택된 자료요소가 없습니다.", "선택오류");
				else
					MessageBox.Show("Not Exist Selected Data Type.", "Select error");
				return;
			}
			bOkFlag = true;
			Close();
		}
	}
}
