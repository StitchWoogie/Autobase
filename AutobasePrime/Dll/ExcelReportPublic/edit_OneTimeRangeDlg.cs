using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_OneTimeRangeDlg.
	/// </summary>
	public class edit_OneTimeRangeDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.VScrollBar vScrollBar_TimeStartSub;
		private System.Windows.Forms.VScrollBar vScrollBar_TimeStartMain;
		private System.Windows.Forms.TextBox textBox_TimeStartRangeDisplay;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioButton_TimeWeek;
		private System.Windows.Forms.RadioButton radioButton_TimeMonth;
		private System.Windows.Forms.RadioButton radioButton_TimeDay;
		private System.Windows.Forms.RadioButton radioButton_TimeHour;
		private System.Windows.Forms.RadioButton radioButton_TimeMin;
		private System.Windows.Forms.Button button_OK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BasicRptTool.eTimeRangeType timeRange;
		public int nUpTimeRange;
		public int nCurrTimeRange;
		string[] timeRangeBuf = new string[5];
		string[] timePlusMinusBuf = new string[3];
		int[] nStartScrollPos = { 0, 0 };
		
		public edit_OneTimeRangeDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			if(Tools.IsLangKorean()) 
			{
				timePlusMinusBuf[0] = "이전";
				timePlusMinusBuf[1] = "지정";
				timePlusMinusBuf[2] = "다음+";

				timeRangeBuf[0] = "분 ";
				timeRangeBuf[1] = "시 ";
				timeRangeBuf[2] = "일 ";
				timeRangeBuf[3] = "월 ";
				//timeRangeBuf[4] = "주 ";
				timeRangeBuf[4] = "년 ";
			}
			else 
			{
				timePlusMinusBuf[0] = "Prev";
				timePlusMinusBuf[1] = "Curr";
				timePlusMinusBuf[2] = "Next+";

				timeRangeBuf[0] = "Min";
				timeRangeBuf[1] = "Hour";
				timeRangeBuf[2] = "Day";
				timeRangeBuf[3] = "Month";
				//timeRangeBuf[4] = "Week";
				timeRangeBuf[4] = "Year";
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_OneTimeRangeDlg));
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vScrollBar_TimeStartSub = new System.Windows.Forms.VScrollBar();
            this.vScrollBar_TimeStartMain = new System.Windows.Forms.VScrollBar();
            this.textBox_TimeStartRangeDisplay = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_TimeWeek = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeMonth = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeDay = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeHour = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeMin = new System.Windows.Forms.RadioButton();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vScrollBar_TimeStartSub);
            this.groupBox1.Controls.Add(this.vScrollBar_TimeStartMain);
            this.groupBox1.Controls.Add(this.textBox_TimeStartRangeDisplay);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // vScrollBar_TimeStartSub
            // 
            resources.ApplyResources(this.vScrollBar_TimeStartSub, "vScrollBar_TimeStartSub");
            this.vScrollBar_TimeStartSub.Maximum = 99999;
            this.vScrollBar_TimeStartSub.Minimum = -99999;
            this.vScrollBar_TimeStartSub.Name = "vScrollBar_TimeStartSub";
            this.vScrollBar_TimeStartSub.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_TimeStartSub_Scroll);
            // 
            // vScrollBar_TimeStartMain
            // 
            resources.ApplyResources(this.vScrollBar_TimeStartMain, "vScrollBar_TimeStartMain");
            this.vScrollBar_TimeStartMain.Maximum = 99999;
            this.vScrollBar_TimeStartMain.Minimum = -99999;
            this.vScrollBar_TimeStartMain.Name = "vScrollBar_TimeStartMain";
            this.vScrollBar_TimeStartMain.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_TimeStartMain_Scroll);
            // 
            // textBox_TimeStartRangeDisplay
            // 
            resources.ApplyResources(this.textBox_TimeStartRangeDisplay, "textBox_TimeStartRangeDisplay");
            this.textBox_TimeStartRangeDisplay.Name = "textBox_TimeStartRangeDisplay";
            this.textBox_TimeStartRangeDisplay.ReadOnly = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton_TimeWeek);
            this.groupBox4.Controls.Add(this.radioButton_TimeMonth);
            this.groupBox4.Controls.Add(this.radioButton_TimeDay);
            this.groupBox4.Controls.Add(this.radioButton_TimeHour);
            this.groupBox4.Controls.Add(this.radioButton_TimeMin);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // radioButton_TimeWeek
            // 
            resources.ApplyResources(this.radioButton_TimeWeek, "radioButton_TimeWeek");
            this.radioButton_TimeWeek.Name = "radioButton_TimeWeek";
            this.radioButton_TimeWeek.CheckedChanged += new System.EventHandler(this.radioButton_TimeWeek_CheckedChanged);
            // 
            // radioButton_TimeMonth
            // 
            resources.ApplyResources(this.radioButton_TimeMonth, "radioButton_TimeMonth");
            this.radioButton_TimeMonth.Name = "radioButton_TimeMonth";
            this.radioButton_TimeMonth.CheckedChanged += new System.EventHandler(this.radioButton_TimeMonth_CheckedChanged);
            // 
            // radioButton_TimeDay
            // 
            resources.ApplyResources(this.radioButton_TimeDay, "radioButton_TimeDay");
            this.radioButton_TimeDay.Name = "radioButton_TimeDay";
            this.radioButton_TimeDay.CheckedChanged += new System.EventHandler(this.radioButton_TimeDay_CheckedChanged);
            // 
            // radioButton_TimeHour
            // 
            resources.ApplyResources(this.radioButton_TimeHour, "radioButton_TimeHour");
            this.radioButton_TimeHour.Name = "radioButton_TimeHour";
            this.radioButton_TimeHour.CheckedChanged += new System.EventHandler(this.radioButton_TimeHour_CheckedChanged);
            // 
            // radioButton_TimeMin
            // 
            resources.ApplyResources(this.radioButton_TimeMin, "radioButton_TimeMin");
            this.radioButton_TimeMin.Name = "radioButton_TimeMin";
            this.radioButton_TimeMin.CheckedChanged += new System.EventHandler(this.radioButton_TimeMin_CheckedChanged);
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            // 
            // edit_OneTimeRangeDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_OneTimeRangeDlg";
            this.Load += new System.EventHandler(this.edit_OneTimeRangeDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


		public void setCurrentTimeRange()
		{
			switch(timeRange) 
			{
				case BasicRptTool.eTimeRangeType.MIN : radioButton_TimeMin.Checked = true; break;
				case BasicRptTool.eTimeRangeType.DAY : radioButton_TimeDay.Checked = true; break;				
				case BasicRptTool.eTimeRangeType.MONTH : radioButton_TimeMonth.Checked = true; break;
				case BasicRptTool.eTimeRangeType.WEEK : radioButton_TimeWeek.Checked = true; break;
				default :this.radioButton_TimeHour.Checked = true; break;
			}
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange, nCurrTimeRange);			
		}

		int getTimeRangeMaxValue()
		{
			switch(timeRange) 
			{
				case BasicRptTool.eTimeRangeType.MIN : return 59;
				case BasicRptTool.eTimeRangeType.DAY : return 31;
				case BasicRptTool.eTimeRangeType.WEEK : return 54;
				case BasicRptTool.eTimeRangeType.MONTH : return 12;
				default :return 23;
			}
		}

		int getTimeRangeMinValue()
		{
			switch(timeRange) 
			{
				case BasicRptTool.eTimeRangeType.MIN : return 0;
				case BasicRptTool.eTimeRangeType.DAY : 
				case BasicRptTool.eTimeRangeType.WEEK : 
				case BasicRptTool.eTimeRangeType.MONTH : return 1;
				default :return 0;
			}			
		}

		void getCurrentTimeRange()
		{
			if(radioButton_TimeMin.Checked) timeRange = BasicRptTool.eTimeRangeType.MIN;
			else if(radioButton_TimeDay.Checked) timeRange = BasicRptTool.eTimeRangeType.DAY;
			else if(radioButton_TimeWeek.Checked) timeRange = BasicRptTool.eTimeRangeType.WEEK;
			else if(radioButton_TimeMonth.Checked) timeRange = BasicRptTool.eTimeRangeType.MONTH;
			else timeRange = BasicRptTool.eTimeRangeType.HOUR;
			nCurrTimeRange = getTimeRangeMinValue();
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange, nCurrTimeRange);				
			checkAndSetMinMaxValue();
		}

		public string makeCurrentTimeRangeBuf(int upRange, int currRange)
		{
			string	buf;
			int		buf_pos, rangeBufPos;

			if(upRange < 0) buf_pos = 0;
			else if(upRange > 0) buf_pos = 2;
			else buf_pos = 1;

			switch(timeRange) 
			{
				case BasicRptTool.eTimeRangeType.MIN : rangeBufPos = 0;	break;
				case BasicRptTool.eTimeRangeType.DAY : rangeBufPos = 2;	break;
					//case BasicRptTool.eTimeRangeType.WEEK : rangeBufPos = 3; break;
				case BasicRptTool.eTimeRangeType.MONTH : rangeBufPos = 3; break;					
				default :rangeBufPos = 1; break;
			}
			buf = timePlusMinusBuf[buf_pos % 3];
			if((buf_pos % 3) != 1) buf += upRange.ToString();
			buf += timeRangeBuf[(rangeBufPos + 1) % 5];
			buf += String.Format("{0,2:d02}", currRange);
			buf += timeRangeBuf[rangeBufPos % 5];
			return buf;
		}

		void checkAndSetMinMaxValue()
		{
			if(nCurrTimeRange < getTimeRangeMinValue()) nCurrTimeRange = getTimeRangeMinValue();
			if(nCurrTimeRange > getTimeRangeMaxValue()) nCurrTimeRange = getTimeRangeMaxValue();			
		}
		
		private void edit_OneTimeRangeDlg_Load(object sender, System.EventArgs e)
		{
			vScrollBar_TimeStartMain.Value = nStartScrollPos[0];
			vScrollBar_TimeStartSub.Value = nStartScrollPos[1];
			checkAndSetMinMaxValue();
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange, nCurrTimeRange);		// 먼저 Time Range 를 Setting 한 후에 하기 위해..		
		}

		private void radioButton_TimeMin_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();
		}

		private void radioButton_TimeHour_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();
		}

		private void radioButton_TimeDay_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();
		}

		private void radioButton_TimeMonth_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();
		}

		private void radioButton_TimeWeek_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();
		}

		private void vScrollBar_TimeStartMain_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(vScrollBar_TimeStartMain.Value == nStartScrollPos[0]) return;

			if(vScrollBar_TimeStartMain.Value < nStartScrollPos[0]) nUpTimeRange++;			
			else nUpTimeRange--;
				
			nStartScrollPos[0] = vScrollBar_TimeStartMain.Value;
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange, nCurrTimeRange);			
		}

		
		private void vScrollBar_TimeStartSub_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(vScrollBar_TimeStartSub.Value == nStartScrollPos[1]) return;

			if(vScrollBar_TimeStartSub.Value <= nStartScrollPos[1]) 
			{
				nCurrTimeRange++; // 위쪽 버튼이 - 값이 나온다
				if(nCurrTimeRange > getTimeRangeMaxValue()) 
				{
					nUpTimeRange++;
					nCurrTimeRange = getTimeRangeMinValue();
				}
			}
			else 
			{
				nCurrTimeRange--;
				if(nCurrTimeRange < getTimeRangeMinValue()) 
				{
					nUpTimeRange--;
					nCurrTimeRange = getTimeRangeMaxValue();
				}
			}			

			nStartScrollPos[1] = vScrollBar_TimeStartSub.Value;
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange, nCurrTimeRange);			
		}
	}
}
