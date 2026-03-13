using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_TimeRangeDlg.
	/// </summary>
	public class edit_TimeRangeDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.RadioButton radioButton_TimeDay;
		private System.Windows.Forms.RadioButton radioButton_TimeHour;
		private System.Windows.Forms.RadioButton radioButton_TimeMin;
		private System.Windows.Forms.RadioButton radioButton_TimeMonth;
		private System.Windows.Forms.TextBox textBox_TimeStartRangeDisplay;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.VScrollBar vScrollBar_TimeStartSub;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.VScrollBar vScrollBar_TimeEndSub;
		private System.Windows.Forms.VScrollBar vScrollBar_TimeEndMain;
		private System.Windows.Forms.TextBox textBox_TimeEndRangeDisplay;
		private System.Windows.Forms.Label label2;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public BasicRptTool.eTimeRangeType timeRange;
		public int[] nUpTimeRange = new int[2];
		public int[] nCurrTimeRange = new int[2];
		string[] timeRangeBuf = new string[5];
		private System.Windows.Forms.VScrollBar vScrollBar_TimeStartMain;
		string[] timePlusMinusBuf = new string[3];
		int[] nStartScrollPos = { 0, 0 };
		private System.Windows.Forms.RadioButton radioButton_TimeWeek;
		int[] nEndScrollPos = { 0, 0 };

		public edit_TimeRangeDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_TimeRangeDlg));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton_TimeWeek = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeMonth = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeDay = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeHour = new System.Windows.Forms.RadioButton();
            this.radioButton_TimeMin = new System.Windows.Forms.RadioButton();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.textBox_TimeStartRangeDisplay = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.vScrollBar_TimeEndSub = new System.Windows.Forms.VScrollBar();
            this.vScrollBar_TimeEndMain = new System.Windows.Forms.VScrollBar();
            this.textBox_TimeEndRangeDisplay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.vScrollBar_TimeStartSub = new System.Windows.Forms.VScrollBar();
            this.vScrollBar_TimeStartMain = new System.Windows.Forms.VScrollBar();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            // 
            // textBox_TimeStartRangeDisplay
            // 
            resources.ApplyResources(this.textBox_TimeStartRangeDisplay, "textBox_TimeStartRangeDisplay");
            this.textBox_TimeStartRangeDisplay.Name = "textBox_TimeStartRangeDisplay";
            this.textBox_TimeStartRangeDisplay.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.vScrollBar_TimeEndSub);
            this.groupBox1.Controls.Add(this.vScrollBar_TimeEndMain);
            this.groupBox1.Controls.Add(this.textBox_TimeEndRangeDisplay);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.vScrollBar_TimeStartSub);
            this.groupBox1.Controls.Add(this.vScrollBar_TimeStartMain);
            this.groupBox1.Controls.Add(this.textBox_TimeStartRangeDisplay);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // vScrollBar_TimeEndSub
            // 
            resources.ApplyResources(this.vScrollBar_TimeEndSub, "vScrollBar_TimeEndSub");
            this.vScrollBar_TimeEndSub.Maximum = 99999;
            this.vScrollBar_TimeEndSub.Minimum = -99999;
            this.vScrollBar_TimeEndSub.Name = "vScrollBar_TimeEndSub";
            this.vScrollBar_TimeEndSub.ValueChanged += new System.EventHandler(this.vScrollBar_TimeEndSub_ValueChanged);
            // 
            // vScrollBar_TimeEndMain
            // 
            resources.ApplyResources(this.vScrollBar_TimeEndMain, "vScrollBar_TimeEndMain");
            this.vScrollBar_TimeEndMain.Maximum = 99999;
            this.vScrollBar_TimeEndMain.Minimum = -99999;
            this.vScrollBar_TimeEndMain.Name = "vScrollBar_TimeEndMain";
            this.vScrollBar_TimeEndMain.ValueChanged += new System.EventHandler(this.vScrollBar_TimeEndMain_ValueChanged);
            this.vScrollBar_TimeEndMain.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_TimeEndMain_Scroll);
            // 
            // textBox_TimeEndRangeDisplay
            // 
            resources.ApplyResources(this.textBox_TimeEndRangeDisplay, "textBox_TimeEndRangeDisplay");
            this.textBox_TimeEndRangeDisplay.Name = "textBox_TimeEndRangeDisplay";
            this.textBox_TimeEndRangeDisplay.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // vScrollBar_TimeStartSub
            // 
            resources.ApplyResources(this.vScrollBar_TimeStartSub, "vScrollBar_TimeStartSub");
            this.vScrollBar_TimeStartSub.Maximum = 99999;
            this.vScrollBar_TimeStartSub.Minimum = -99999;
            this.vScrollBar_TimeStartSub.Name = "vScrollBar_TimeStartSub";
            this.vScrollBar_TimeStartSub.ValueChanged += new System.EventHandler(this.vScrollBar_TimeStartSub_ValueChanged);
            // 
            // vScrollBar_TimeStartMain
            // 
            resources.ApplyResources(this.vScrollBar_TimeStartMain, "vScrollBar_TimeStartMain");
            this.vScrollBar_TimeStartMain.Maximum = 99999;
            this.vScrollBar_TimeStartMain.Minimum = -99999;
            this.vScrollBar_TimeStartMain.Name = "vScrollBar_TimeStartMain";
            this.vScrollBar_TimeStartMain.ValueChanged += new System.EventHandler(this.vScrollBar_TimeStartMain_ValueChanged);
            // 
            // edit_TimeRangeDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_TimeRangeDlg";
            this.Load += new System.EventHandler(this.edit_TimeRangeDlg_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
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
			nCurrTimeRange[0] = getTimeRangeMinValue();
			nCurrTimeRange[1] = getTimeRangeMinValue();
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
			
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
			if(nCurrTimeRange[0] > getTimeRangeMaxValue()) nCurrTimeRange[0] = getTimeRangeMaxValue();
			if(nCurrTimeRange[0] < getTimeRangeMinValue()) nCurrTimeRange[0] = getTimeRangeMinValue();
			
			if(nCurrTimeRange[1] > getTimeRangeMaxValue()) nCurrTimeRange[1] = getTimeRangeMaxValue();
			if(nCurrTimeRange[1] < getTimeRangeMinValue()) nCurrTimeRange[1] = getTimeRangeMinValue();			
		}

		private void edit_TimeRangeDlg_Load(object sender, System.EventArgs e)
		{
			vScrollBar_TimeStartMain.Value = nStartScrollPos[0];
			vScrollBar_TimeStartSub.Value = nStartScrollPos[1];
			vScrollBar_TimeEndMain.Value = nEndScrollPos[0];
			vScrollBar_TimeEndSub.Value = nEndScrollPos[1];
			checkAndSetMinMaxValue();
			
			//setCurrentTimeRange();			
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);		// 먼저 Time Range 를 Setting 한 후에 하기 위해..
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);			
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

		private void radioButton_TimeWeek_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();
		}

		private void radioButton_TimeMonth_CheckedChanged(object sender, System.EventArgs e)
		{
			getCurrentTimeRange();		
		}


		void checkUpTimeAbdSetUpScroll()
		{
			if(nUpTimeRange[0] > nUpTimeRange[1]) nUpTimeRange[1] = nUpTimeRange[0];
			else if(nUpTimeRange[0] == nUpTimeRange[1] && nCurrTimeRange[0] > nCurrTimeRange[1]) nCurrTimeRange[1] = nCurrTimeRange[0];
		}

		void checkDownTimeAbdSetUpScroll()
		{
			if(nUpTimeRange[0] > nUpTimeRange[1]) nUpTimeRange[0] = nUpTimeRange[1];
			else if(nUpTimeRange[0] == nUpTimeRange[1] && nCurrTimeRange[0] > nCurrTimeRange[1]) nCurrTimeRange[0] = nCurrTimeRange[1];
		}

		private void vScrollBar_TimeStartMain_ValueChanged(object sender, System.EventArgs e)
		{
			if(vScrollBar_TimeStartMain.Value < nStartScrollPos[0]) // 위쪽 버튼이 - 값이 나온다
			{
				nUpTimeRange[0]++;	
				checkUpTimeAbdSetUpScroll();
			}
			else nUpTimeRange[0]--;
			nStartScrollPos[0] = vScrollBar_TimeStartMain.Value;
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
		}

		private void vScrollBar_TimeStartSub_ValueChanged(object sender, System.EventArgs e)
		{
			if(vScrollBar_TimeStartSub.Value < nStartScrollPos[1]) // 위쪽 버튼이 - 값이 나온다
			{
				nCurrTimeRange[0]++;
				if(nCurrTimeRange[0] > getTimeRangeMaxValue()) 
				{
					nUpTimeRange[0]++;
					nCurrTimeRange[0] = getTimeRangeMinValue();
				}
				checkUpTimeAbdSetUpScroll();
			}
			else 
			{
				nCurrTimeRange[0]--;
				if(nCurrTimeRange[0] < getTimeRangeMinValue()) 
				{
					nUpTimeRange[0]--;
					nCurrTimeRange[0] = getTimeRangeMaxValue();
				}
			}
			nStartScrollPos[1] = vScrollBar_TimeStartSub.Value;
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
		}

		private void vScrollBar_TimeEndMain_ValueChanged(object sender, System.EventArgs e)
		{
			if(vScrollBar_TimeEndMain.Value < nEndScrollPos[0]) nUpTimeRange[1]++;	// 위쪽 버튼이 - 값이 나온다
			else 
			{
				nUpTimeRange[1]--;
				checkDownTimeAbdSetUpScroll();
			}
			nEndScrollPos[0] = vScrollBar_TimeEndMain.Value;
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
		}

		private void vScrollBar_TimeEndSub_ValueChanged(object sender, System.EventArgs e)
		{
			if(vScrollBar_TimeEndSub.Value < nEndScrollPos[1]) // 위쪽 버튼이 - 값이 나온다
			{
				nCurrTimeRange[1]++;
				if(nCurrTimeRange[1] > getTimeRangeMaxValue()) 
				{
					nUpTimeRange[1]++;
					nCurrTimeRange[1] = getTimeRangeMinValue();
				}
			}
			else 
			{
				nCurrTimeRange[1]--;
				if(nCurrTimeRange[1] < getTimeRangeMinValue()) 
				{
					nUpTimeRange[1]--;
					nCurrTimeRange[1] = getTimeRangeMaxValue();
				}
				checkDownTimeAbdSetUpScroll();
			}
			nEndScrollPos[1] = vScrollBar_TimeEndSub.Value;
			textBox_TimeStartRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[0], nCurrTimeRange[0]);
			textBox_TimeEndRangeDisplay.Text = makeCurrentTimeRangeBuf(nUpTimeRange[1], nCurrTimeRange[1]);
		}

		private void vScrollBar_TimeEndMain_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
		
		}

		
		

		

		

		
	}
}
