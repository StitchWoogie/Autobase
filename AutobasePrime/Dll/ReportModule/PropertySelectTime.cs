using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertySelectTime.
	/// </summary>
	public class PropertySelectTime : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonTimeType0;
		private System.Windows.Forms.RadioButton radioButtonTimeType1;
		private System.Windows.Forms.RadioButton radioButtonTimeType2;
		private System.Windows.Forms.RadioButton radioButtonTimeType3;
		private System.Windows.Forms.RadioButton radioButtonTimeType4;
		private System.Windows.Forms.RadioButton radioButtonTimeType5;
		private System.Windows.Forms.TextBox textBoxTimeFrom;
		private System.Windows.Forms.VScrollBar vScrollBarFromBig;
		private System.Windows.Forms.VScrollBar vScrollBarFromSmall;
		private System.Windows.Forms.VScrollBar vScrollBarToSmall;
		private System.Windows.Forms.VScrollBar vScrollBarToBig;
		private System.Windows.Forms.TextBox textBoxTimeTo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxToDataTime;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertySelectTime()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertySelectTime));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTimeType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType0 = new System.Windows.Forms.RadioButton();
            this.textBoxTimeFrom = new System.Windows.Forms.TextBox();
            this.vScrollBarFromBig = new System.Windows.Forms.VScrollBar();
            this.vScrollBarFromSmall = new System.Windows.Forms.VScrollBar();
            this.vScrollBarToSmall = new System.Windows.Forms.VScrollBar();
            this.vScrollBarToBig = new System.Windows.Forms.VScrollBar();
            this.textBoxTimeTo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxToDataTime = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTimeType5);
            this.groupBox1.Controls.Add(this.radioButtonTimeType4);
            this.groupBox1.Controls.Add(this.radioButtonTimeType3);
            this.groupBox1.Controls.Add(this.radioButtonTimeType2);
            this.groupBox1.Controls.Add(this.radioButtonTimeType1);
            this.groupBox1.Controls.Add(this.radioButtonTimeType0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonTimeType5
            // 
            resources.ApplyResources(this.radioButtonTimeType5, "radioButtonTimeType5");
            this.radioButtonTimeType5.Name = "radioButtonTimeType5";
            this.radioButtonTimeType5.CheckedChanged += new System.EventHandler(this.radioButtonTimeType5_CheckedChanged);
            // 
            // radioButtonTimeType4
            // 
            resources.ApplyResources(this.radioButtonTimeType4, "radioButtonTimeType4");
            this.radioButtonTimeType4.Name = "radioButtonTimeType4";
            this.radioButtonTimeType4.CheckedChanged += new System.EventHandler(this.radioButtonTimeType4_CheckedChanged);
            // 
            // radioButtonTimeType3
            // 
            resources.ApplyResources(this.radioButtonTimeType3, "radioButtonTimeType3");
            this.radioButtonTimeType3.Name = "radioButtonTimeType3";
            this.radioButtonTimeType3.CheckedChanged += new System.EventHandler(this.radioButtonTimeType3_CheckedChanged);
            // 
            // radioButtonTimeType2
            // 
            resources.ApplyResources(this.radioButtonTimeType2, "radioButtonTimeType2");
            this.radioButtonTimeType2.Name = "radioButtonTimeType2";
            this.radioButtonTimeType2.CheckedChanged += new System.EventHandler(this.radioButtonTimeType2_CheckedChanged);
            // 
            // radioButtonTimeType1
            // 
            resources.ApplyResources(this.radioButtonTimeType1, "radioButtonTimeType1");
            this.radioButtonTimeType1.Name = "radioButtonTimeType1";
            this.radioButtonTimeType1.CheckedChanged += new System.EventHandler(this.radioButtonTimeType1_CheckedChanged);
            // 
            // radioButtonTimeType0
            // 
            resources.ApplyResources(this.radioButtonTimeType0, "radioButtonTimeType0");
            this.radioButtonTimeType0.Name = "radioButtonTimeType0";
            this.radioButtonTimeType0.CheckedChanged += new System.EventHandler(this.radioButtonTimeType0_CheckedChanged);
            // 
            // textBoxTimeFrom
            // 
            resources.ApplyResources(this.textBoxTimeFrom, "textBoxTimeFrom");
            this.textBoxTimeFrom.Name = "textBoxTimeFrom";
            this.textBoxTimeFrom.ReadOnly = true;
            // 
            // vScrollBarFromBig
            // 
            resources.ApplyResources(this.vScrollBarFromBig, "vScrollBarFromBig");
            this.vScrollBarFromBig.Name = "vScrollBarFromBig";
            this.vScrollBarFromBig.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarFromBig_Scroll);
            // 
            // vScrollBarFromSmall
            // 
            resources.ApplyResources(this.vScrollBarFromSmall, "vScrollBarFromSmall");
            this.vScrollBarFromSmall.Name = "vScrollBarFromSmall";
            this.vScrollBarFromSmall.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarFromSmall_Scroll);
            // 
            // vScrollBarToSmall
            // 
            resources.ApplyResources(this.vScrollBarToSmall, "vScrollBarToSmall");
            this.vScrollBarToSmall.Name = "vScrollBarToSmall";
            this.vScrollBarToSmall.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarToSmall_Scroll);
            // 
            // vScrollBarToBig
            // 
            resources.ApplyResources(this.vScrollBarToBig, "vScrollBarToBig");
            this.vScrollBarToBig.Name = "vScrollBarToBig";
            this.vScrollBarToBig.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarToBig_Scroll);
            // 
            // textBoxTimeTo
            // 
            resources.ApplyResources(this.textBoxTimeTo, "textBoxTimeTo");
            this.textBoxTimeTo.Name = "textBoxTimeTo";
            this.textBoxTimeTo.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxToDataTime
            // 
            resources.ApplyResources(this.checkBoxToDataTime, "checkBoxToDataTime");
            this.checkBoxToDataTime.Name = "checkBoxToDataTime";
            // 
            // PropertySelectTime
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxToDataTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.vScrollBarToSmall);
            this.Controls.Add(this.vScrollBarToBig);
            this.Controls.Add(this.textBoxTimeTo);
            this.Controls.Add(this.textBoxTimeFrom);
            this.Controls.Add(this.vScrollBarFromSmall);
            this.Controls.Add(this.vScrollBarFromBig);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertySelectTime";
            this.Load += new System.EventHandler(this.PropertySelectTime_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		int nFrom;
		int nTo;
		int nShiftFrom;
		int nShiftTo;

		void UpdateToEditBox()
		{
			string buf;
			MakeTimeText(out buf, nTo, nShiftTo);
			this.textBoxTimeTo.Text = buf;
		}

		void UpdateFromEditBox()
		{
			string buf;
			MakeTimeText(out buf, nFrom, nShiftFrom);
			this.textBoxTimeFrom.Text = buf;
		}

		int GetZoneType()
		{
			int zone;

			if(this.radioButtonTimeType0.Checked)		zone = 0;
			else if(this.radioButtonTimeType1.Checked)	zone = 1;
			else if(this.radioButtonTimeType2.Checked)	zone = 2;
			else if(this.radioButtonTimeType3.Checked)	zone = 3;
			else if(this.radioButtonTimeType4.Checked)	zone = 4;
			else if(this.radioButtonTimeType5.Checked)	zone = 5;
			else										zone = 0;

			return zone;
		}

		void MakeTimeText(out string buf, int time, int shift)
		{
			int zone = GetZoneType();

			MakeCellTimeToText(out buf, time, shift, zone);
		}

		

		void MakeCellTimeToText(out string buf, int time, int shift, int zone)
		{
			string[] sWeekDay;
            			
			if(Tools.IsLangKorean()) 
			{
				sWeekDay = new string[7] { "일", "월", "화", "수", "목", "금", "토" };
                
				switch(zone) 
				{
					case 0:
						if(shift < 0)		buf = String.Format("이전{0}시 {1:00}분", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}시 {1:00}분", shift, time);
						else 				buf = String.Format("지정시 {0:00}분", time);
						break;
					case 1:
						if(shift < 0)		buf = String.Format("이전{0}일 {1:00}시", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}일 {1:00}시", shift, time);
						else 				buf = String.Format("지정일 {0:00}시", time);
						break;
					case 2:
						if(shift < 0)		buf = String.Format("이전{0}달 {1:00}일", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}달 {1:00}일", shift, time);
						else 				buf = String.Format("지정달 {0:00}일", time);
						break;
					case 3:
						if(shift < 0)		buf = String.Format("이전{0}주 {1}요일", shift, sWeekDay[time]);
						else if(shift > 0)	buf = String.Format("다음+{0}주 {1}요일", shift, sWeekDay[time]);
						else 				buf = String.Format("지정주 {0}요일", sWeekDay[time]);
						break;
					case 4:
						if(shift < 0)		buf = String.Format("이전{0}년 {1:00}월", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}년 {1:00}월", shift, time);
						else 				buf = String.Format("지정년 {0:00}월", time);
						break;
					default:
						buf = String.Format("새로운 시간범위");
						break;
				}
			}
			else 
			{
				sWeekDay = new string[7] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

				switch(zone) 
				{
					case 0:
						if(shift < 0)		buf = String.Format("Prev {0}H {1:00}M", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}H {1:00}M", shift, time);
						else 				buf = String.Format("Set Hour {0:00}M", time);
						break;
					case 1:
						if(shift < 0)		buf = String.Format("Prev {0}D {1:00}H", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}D {1:00}H", shift, time);
						else 				buf = String.Format("Set Day {0:00}H", time);
						break;
					case 2:
						if(shift < 0)		buf = String.Format("Prev {0}M {1:00}D", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}M {1:00}D", shift, time);
						else 				buf = String.Format("Set Month {0:00}D", time);
						break;
					case 3:
						if(shift < 0)		buf = String.Format("Prev {0}W {1}Day", shift, sWeekDay[time]);
						else if(shift > 0)	buf = String.Format("Next +{0}W {1}Day", shift, sWeekDay[time]);
						else 				buf = String.Format("Set Week {0}Day", sWeekDay[time]);
						break;
					case 4:
						if(shift < 0)		buf = String.Format("Prev {0}Y {1:00}M", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}Y {1:00}M", shift, time);
						else 				buf = String.Format("Set Year {0:00}M", time);
						break;
					default:
						buf = String.Format("Unknown time zone");
						break;
				}
			}
		}

		public delegate void OnEventTimeTypeChanged(int type);
		public event OnEventTimeTypeChanged EventListTimeTypeChanged;

		void EventGoTimeTypeChanged()
		{
			if(EventListTimeTypeChanged == null)	return;

			int type = GetZoneType();

			Delegate[] DelegateList = EventListTimeTypeChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((OnEventTimeTypeChanged) DelegateList[i])(type);
			}
		}

		private void radioButtonTimeType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EventGoTimeTypeChanged();

			if(bInitializing)	return;	// 초기화 진행중에는 return
			nFrom = 0;
			nTo = 0;
			UpdateFromEditBox();
			UpdateToEditBox();	
		} 

		private void radioButtonTimeType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EventGoTimeTypeChanged();

			if(bInitializing)	return;	// 초기화 진행중에는 return
			nFrom = 0;
			nTo = 0;
			UpdateFromEditBox();
			UpdateToEditBox();	
		}

		private void radioButtonTimeType2_CheckedChanged(object sender, System.EventArgs e)
		{
			EventGoTimeTypeChanged();

			if(bInitializing)	return;	// 초기화 진행중에는 return
			nFrom = 1;
			nTo = 1;
			UpdateFromEditBox();
			UpdateToEditBox();	
		}

		private void radioButtonTimeType3_CheckedChanged(object sender, System.EventArgs e)
		{
			EventGoTimeTypeChanged();

			if(bInitializing)	return;	// 초기화 진행중에는 return
			nFrom = 0;
			nTo = 0;
			UpdateFromEditBox();
			UpdateToEditBox();	
		}

		private void radioButtonTimeType4_CheckedChanged(object sender, System.EventArgs e)
		{
			EventGoTimeTypeChanged();

			if(bInitializing)	return;	// 초기화 진행중에는 return
			nFrom = 1;
			nTo = 1;
			UpdateFromEditBox();
			UpdateToEditBox();	
		}

		private void radioButtonTimeType5_CheckedChanged(object sender, System.EventArgs e)
		{
			EventGoTimeTypeChanged();

			if(bInitializing)	return;	// 초기화 진행중에는 return
		}

        const int MAX_SHIFT_FROM = 1000;
        const int MIN_SHIFT_FROM = -1000;

		private void vScrollBarFromBig_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			switch(e.Type) 
			{
				case ScrollEventType.SmallDecrement:
                    if (nShiftFrom >= MAX_SHIFT_FROM) return;
					nShiftFrom++;

					if(nShiftFrom > nShiftTo ||
						(nShiftFrom == nShiftTo && nFrom > nTo)) 
					{
						nTo = nFrom;
						nShiftTo = nShiftFrom;
					}
					break;
				case ScrollEventType.SmallIncrement:
                    if (nShiftFrom <= MIN_SHIFT_FROM) return;
					nShiftFrom--;
					break;
			}
			UpdateFromEditBox();
			UpdateToEditBox();
		}

		private void vScrollBarFromSmall_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			switch(e.Type) 
			{
				case ScrollEventType.SmallDecrement: 
					if(nFrom >= GetLimitMax()) 
					{
						nShiftFrom++;
						nFrom = GetLimitMin();
					}
					else 
					{
						nFrom++;
					}
					if(nShiftFrom > nShiftTo ||
						(nShiftFrom == nShiftTo && nFrom > nTo)) 
					{
						nTo = nFrom;
						nShiftTo = nShiftFrom;
					}
					break;
				case ScrollEventType.SmallIncrement:
					if(nFrom <= GetLimitMin()) 
					{
						nShiftFrom--;
						nFrom = GetLimitMax();
					}
					else 
					{
						nFrom--;
					}
					break;
			}
			UpdateFromEditBox();
			UpdateToEditBox();
		}

		private void vScrollBarToBig_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			switch(e.Type) 
			{
				case ScrollEventType.SmallDecrement:
					if(nShiftTo >= MAX_SHIFT_FROM)	return;
					nShiftTo++;
					break;
				case ScrollEventType.SmallIncrement:
                    if (nShiftTo <= MIN_SHIFT_FROM) return;
					nShiftTo--;

					if(nShiftTo < nShiftFrom ||
						(nShiftTo == nShiftFrom && nTo < nFrom)) 
					{
						nFrom = nTo;
						nShiftFrom = nShiftTo;
					}
					//if(nTo < nFrom)	nFrom = nTo;
					break;
			}
			UpdateFromEditBox();
			UpdateToEditBox();
		}

		private void vScrollBarToSmall_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			switch(e.Type) 
			{
				case ScrollEventType.SmallDecrement: 
					if(nTo >= GetLimitMax()) 
					{
						nTo = GetLimitMin();
						nShiftTo++;
					}
					else 
					{
						nTo++;
					}
					break;
				case ScrollEventType.SmallIncrement:
					if(nTo <= GetLimitMin()) 
					{
						nTo = GetLimitMax();
						nShiftTo--;
					}
					else 
					{
						nTo--;
					}
					if(nShiftTo < nShiftFrom ||
						(nShiftTo == nShiftFrom && nTo < nFrom)) 
					{
						nFrom = nTo;
						nShiftFrom = nShiftTo;
					}
					//if(nTo < nFrom)	nFrom = nTo;
					break;
			}
			UpdateFromEditBox();
			UpdateToEditBox();
		}

		int GetLimitMax() 
		{
			int zone = GetZoneType();

			switch(zone) 
			{
				case 0:		return 59;
				case 1:		return 23;
				case 2:		return 31;
				case 3:		return 6;
				case 4:		return 12;
				default:	return 10;
			}
		}

		int GetLimitMin() 
		{
			int zone = GetZoneType();

			switch(zone) 
			{
				case 0:		return 0;
				case 1:		return 0;
				case 2:		return 1;
				case 3:		return 0;
				case 4:		return 1;
				default:	return 0;
			}
		}

		bool bInitializing;

		public void SetTime(CELL_TIME obj)
		{
			bInitializing = true;	// 초기화 진행중

			//dialog.bSyncFlag = ON;
			this.nFrom = obj.from;
			this.nTo = obj.to;
			this.nShiftFrom = obj.shift_from;
			this.nShiftTo = obj.shift_to;

			if(String.Compare(obj.zone, "Min", true) == 0)			this.radioButtonTimeType0.Checked = true;
			else if(String.Compare(obj.zone, "Hour", true) == 0)	this.radioButtonTimeType1.Checked = true;
			else if(String.Compare(obj.zone, "Day", true) == 0)		this.radioButtonTimeType2.Checked = true;
			else if(String.Compare(obj.zone, "Week", true) == 0)	this.radioButtonTimeType3.Checked = true;
			else if(String.Compare(obj.zone, "Mon", true) == 0)		this.radioButtonTimeType4.Checked = true;
			else													this.radioButtonTimeType5.Checked = true;
		
			this.checkBoxToDataTime.Checked = (obj.bToDataTime == 1);

			UpdateFromEditBox();
			UpdateToEditBox();

			bInitializing = false;	// 초기화 종료
		}

		public void GetTime(CELL_TIME obj)
		{
			obj.from = this.nFrom;
			obj.to = this.nTo;
			obj.shift_from = this.nShiftFrom;
			obj.shift_to = this.nShiftTo;

			int zone = GetZoneType();

			if(zone == 0)		obj.zone = "Min";
			else if(zone == 1)	obj.zone = "Hour";
			else if(zone == 2)	obj.zone = "Day";
			else if(zone == 3)	obj.zone = "Week";
			else if(zone == 4)	obj.zone = "Mon";
			else				obj.zone = "None";

			obj.bToDataTime = this.checkBoxToDataTime.Checked ? (sbyte)1:(sbyte)0;
		}

		private void PropertySelectTime_Load(object sender, System.EventArgs e)
		{
		
		}

		
		public bool UseToDataTime 
		{
			set 
			{
				this.checkBoxToDataTime.Enabled = value;
			}
		}

	}
}

