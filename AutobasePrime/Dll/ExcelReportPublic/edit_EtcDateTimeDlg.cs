using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_EtcDateTimeDlg.
	/// </summary>
	public class edit_EtcDateTimeDlg : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ComboBox comboBox_Data_Type;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Button button_TimeRange;
		private System.Windows.Forms.TextBox textBox_TimeRangeDisplay;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.CheckBox checkBox_Seletced_DataTime;
		public bool bUseRange;
		public bool bOkFlag = false;
		public BasicRptTool.eTimeRangeType timeRange;
		public int[] nUpTimeRange = new int[2];
		public int[] nCurrTimeRange = new int[2];
		public bool bMultiRow;

		public edit_EtcDateTimeDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_EtcDateTimeDlg));
            this.comboBox_Data_Type = new System.Windows.Forms.ComboBox();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button_TimeRange = new System.Windows.Forms.Button();
            this.textBox_TimeRangeDisplay = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_Seletced_DataTime = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_Data_Type
            // 
            resources.ApplyResources(this.comboBox_Data_Type, "comboBox_Data_Type");
            this.comboBox_Data_Type.Name = "comboBox_Data_Type";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox_Data_Type);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button_TimeRange);
            this.groupBox6.Controls.Add(this.textBox_TimeRangeDisplay);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.checkBox_Seletced_DataTime);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // button_TimeRange
            // 
            resources.ApplyResources(this.button_TimeRange, "button_TimeRange");
            this.button_TimeRange.Name = "button_TimeRange";
            this.button_TimeRange.Click += new System.EventHandler(this.button_TimeRange_Click);
            // 
            // textBox_TimeRangeDisplay
            // 
            resources.ApplyResources(this.textBox_TimeRangeDisplay, "textBox_TimeRangeDisplay");
            this.textBox_TimeRangeDisplay.Name = "textBox_TimeRangeDisplay";
            this.textBox_TimeRangeDisplay.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // checkBox_Seletced_DataTime
            // 
            resources.ApplyResources(this.checkBox_Seletced_DataTime, "checkBox_Seletced_DataTime");
            this.checkBox_Seletced_DataTime.Name = "checkBox_Seletced_DataTime";
            // 
            // edit_EtcDateTimeDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_EtcDateTimeDlg";
            this.Load += new System.EventHandler(this.edit_EtcDateTimeDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(this.comboBox_Data_Type.Text.Length <= 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("명렁인식 문장을 설정해야 합니다.", "설정오류");
				else
					MessageBox.Show("Please Select Command Name.", "Select Error");
				return;
			}
			bOkFlag = true;
			Close();
		}

		private void setTimeRangeBuf()
		{
			string		buf;
			edit_TimeRangeDlg dlg = new edit_TimeRangeDlg();

			dlg.timeRange = timeRange;
			buf = dlg.makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			if(bMultiRow) 
			{
				buf += " ~ ";
				buf += dlg.makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
			}
			textBox_TimeRangeDisplay.Text = buf;
		}

		private void edit_EtcDateTimeDlg_Load(object sender, System.EventArgs e)
		{			
			if(bUseRange == false) this.button_TimeRange.Enabled = false;
			else setTimeRangeBuf();
		}

		private void button_TimeRange_Click(object sender, System.EventArgs e)
		{
			edit_TimeRangeDlg dlg = new edit_TimeRangeDlg();

			dlg.timeRange = timeRange;
			dlg.setCurrentTimeRange();
			dlg.nUpTimeRange[0] = nUpTimeRange[0];
			dlg.nCurrTimeRange[0] = nCurrTimeRange[0];
			if(bMultiRow) 
			{
				dlg.nUpTimeRange[1] = nUpTimeRange[1];
				dlg.nCurrTimeRange[1] = nCurrTimeRange[1];
			}
			else 
			{
				dlg.nUpTimeRange[1] = nUpTimeRange[0];
				dlg.nCurrTimeRange[1] = nCurrTimeRange[0];
			}				
			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{
				timeRange = dlg.timeRange;
				nUpTimeRange[0] = dlg.nUpTimeRange[0];
				nCurrTimeRange[0] = dlg.nCurrTimeRange[0];
				nUpTimeRange[1] = dlg.nUpTimeRange[1];
				nCurrTimeRange[1] = dlg.nCurrTimeRange[1];
				setTimeRangeBuf();
			}
		}


	}
}
