using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using DialogHoliday;
using PublicStudioLocalMain.Schedule;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormScheduleChild.
	/// </summary>
	public class FormScheduleChild : System.Windows.Forms.Form 
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormScheduleChild()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            //fontMain = AutoLibLocal.ConfigViewMain.MakeDefaultFont();
            fontMain = ConfigRunMain.facScheduleFont.GetFont();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduleChild));
            this.panel1 = new System.Windows.Forms.Panel();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.vScrollBar1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.AccessibleDescription = null;
            this.vScrollBar1.AccessibleName = null;
            resources.ApplyResources(this.vScrollBar1, "vScrollBar1");
            this.vScrollBar1.BackgroundImage = null;
            this.vScrollBar1.Font = null;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // panelMain
            // 
            this.panelMain.AccessibleDescription = null;
            this.panelMain.AccessibleName = null;
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackgroundImage = null;
            this.panelMain.Font = null;
            this.panelMain.Name = "panelMain";
            this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
            this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMain_MouseDown);
            this.panelMain.SizeChanged += new System.EventHandler(this.panelMain_SizeChanged);
            // 
            // FormScheduleChild
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.Name = "FormScheduleChild";
            this.Load += new System.EventHandler(this.FormScheduleChild_Load);
            this.SizeChanged += new System.EventHandler(this.FormScheduleChild_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormScheduleChild_MouseDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		SYSTEMTIME tCursor = new SYSTEMTIME();
		SYSTEMTIME tToday = new SYSTEMTIME();
		FormScheduleDay wndDay = null;
		int nHeight = 0;
		int nWidth = 0;
		int cyChar = 0;
		int cxChar = 0;
		//CScrollBar m_spin;
		Font fontSmall = null;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.Panel panelMain;

		Font fontMain = new Font("Gulim", 10);

		private void FormScheduleChild_Load(object sender, System.EventArgs e)
		{
			wndDay = null;
			tCursor.GetLocalTime();
			tToday.GetLocalTime();	

			cxChar = fontMain.Height/2;
			cyChar = fontMain.Height;

			//RECT r;

			//r.left = 4;
			//r.top = 4;
			//r.right = r.left+30;
			//r.bottom = r.top+cyChar*2-8+1;
			//m_spin.Create(WS_CHILD|WS_VISIBLE|SBS_VERT, r, this, 1);

			fontSmall = new Font("Gulim", (float)(10*0.8));

			UpdateLabel();
		}

		bool ScheduleGetDayColor(int year, int mon, int day, out Color color, int week, bool bHoliday, bool bSpecial)
		{
			if(Schedule.scheduleWeek[8].block_pos != -1) 
			{
				if(bSpecial) 
				{
					color = Schedule.scheduleWeek[8].color;
					return true;
				}
			}
			if(Schedule.scheduleWeek[7].block_pos != -1) 
			{
				if(bHoliday) 
				{
					color = Schedule.scheduleWeek[7].color;
					return true;
				}
			}
			if(Schedule.scheduleWeek[week].block_pos != -1) 
			{
				color = Schedule.scheduleWeek[week].color;
				return true;
			}

			color = Color.Black;

			return false;
		}

		private void FormScheduleChild_SizeChanged(object sender, System.EventArgs e)
		{
			
		}

		private void FormScheduleChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			
		}

		public void SetScheduleDayWnd(FormScheduleDay wnd) 
		{ 
			wndDay = wnd; 
			wndDay.ChangeDay(tCursor);
		}

		public void ReLoad() 
		{
			wndDay.ChangeDay(tCursor);
			this.panelMain.Invalidate();
		}

		private void panelMain_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			RECT rect = new RECT();

			rect.left = this.panelMain.ClientRectangle.Left;
			rect.top = this.panelMain.ClientRectangle.Top;
			rect.right = this.panelMain.ClientRectangle.Right;
			rect.bottom = this.panelMain.ClientRectangle.Bottom;

			DrawClass.gcls(g, rect.left, rect.top, rect.right, rect.bottom, Color.LightGray);

			int week;
			int y;
			int day;
			string[] sWeek;

			if(Tools.IsLangKorean()) 
			{
				sWeek = new string[7] { "일", "월", "화", "수", "목", "금", "토" };
			}
			else if(Tools.IsLangJapanese()) 
			{
				sWeek = new string[7] { "日", "月", "火", "水", "木", "金", "土" };
			}
			else if(Tools.IsLangChinese()) 
			{
				sWeek = new string[7] { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
			}
            else if (Tools.IsLangVietnamese())
            {
                sWeek = new string[7] { "CN", "T Hai", "T Ba", "T Tư", "T Năm", "T Sáu", "T Bảy" };
            }
			else 
			{
				sWeek = new string[7] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
			}
			RECT r = new RECT();
			string buf;
			int x1, y1, x2, y2;
			SCHEDULE_STRUCT sc;
			int l;
			string holiday_text;
			string special_text;
			bool bHoliday;
			Color color;
			bool bSpecial;

			StringFormat formatcenter = new StringFormat();
			formatcenter.Alignment = StringAlignment.Center;
			formatcenter.LineAlignment = StringAlignment.Center;

			y = 5;

			for(week = 0; week < 9; week++) 
			{
				if(week == 8) 
				{
					x1 = 5+7*(nWidth+5);
					y1 = y+nHeight+5;
					x2 = x1+nWidth;
					y2 = y1+nHeight;
				}
				else 
				{
					x1 = 5+week*(nWidth+5);
					y1 = y;
					x2 = x1+nWidth;
					y2 = y1+nHeight;
				}

				DrawClass.PopBox2(g, x1, y1, x2, y2, Color.FromArgb(0, 0x80, 0x80));

				if(Schedule.scheduleWeek[week].block_pos != -1) 
				{
					DrawClass.PushBox2(g, x2-10, y1+3, x2-3, y2-3, Schedule.scheduleWeek[week].color);
				}

				r.left = x1+4;
				r.top  = y1+4;
				r.right  = x2-3;
				r.bottom = y2-3;
		
				if(week == 0)	color = Color.FromArgb(255, 0, 0);
				else			color = Color.Black;

				

				if(week == 7) 
				{
					if(Tools.IsLangKorean()) 
						DrawClass.DrawText(g, "공휴일", fontMain, new SolidBrush(color), r, formatcenter);	
					else if(Tools.IsLangJapanese()) 
						DrawClass.DrawText(g, "公休日", fontMain, new SolidBrush(color), r, formatcenter);	
					else if(Tools.IsLangChinese()) 
						DrawClass.DrawText(g, "公休日", fontMain, new SolidBrush(color), r, formatcenter);
                    else if (Tools.IsLangVietnamese())
                        DrawClass.DrawText(g, "Ngày lễ", fontMain, new SolidBrush(color), r, formatcenter);	
					else 
						DrawClass.DrawText(g, "Holiday", fontMain, new SolidBrush(color), r, formatcenter);	
				}
				else if(week == 8) 
				{
					if(Tools.IsLangKorean()) 
						DrawClass.DrawText(g, "특정일", fontMain, new SolidBrush(color), r, formatcenter);	
					else if(Tools.IsLangJapanese()) 
						DrawClass.DrawText(g, "特定日", fontMain, new SolidBrush(color), r, formatcenter);	
					else if(Tools.IsLangChinese()) 
						DrawClass.DrawText(g, "特定日", fontMain, new SolidBrush(color), r, formatcenter);
                    else if (Tools.IsLangVietnamese())
                        DrawClass.DrawText(g, "Đặc biệt", fontMain, new SolidBrush(color), r, formatcenter);	
					else 
						DrawClass.DrawText(g, "Special", fontMain, new SolidBrush(color), r, formatcenter);	
				}
				else 
				{
					DrawClass.DrawText(g, sWeek[week], fontMain, new SolidBrush(color), r, formatcenter);	
				}
			}

			y += nHeight+5;

			week = TimeUtil.GetWeekDay(tCursor.wYear, tCursor.wMonth, 1);

			int month_limit = TimeUtil.getmonthlimit(tCursor.wYear, tCursor.wMonth);

			for(day = 1; day <= month_limit; day++) 
			{
				// x = 5+week*;
				x1 = 5+week*(nWidth+5);
				y1 = y;
				x2 = x1+nWidth;
				y2 = y+nHeight;

				if(tCursor.wYear == tToday.wYear && tCursor.wMonth == tToday.wMonth && day == tToday.wDay) 
				{
					DrawClass.gcls(g, x1-1, y1-1, x2+1, y2+1, Color.FromArgb(0, 0, 0x80));
				}
				if(tCursor.wDay == day) 
				{
					DrawClass.gcls(g, x1-2, y1-2, x2+2, y2+2, Color.FromArgb(0, 0, 255));
				}

				DrawClass.PopBox2(g, x1, y1, x2, y2, Color.LightGray);

				//int lyear, lmon, lday, leap;
				DATE_SOLAR_LUNAR date = new DATE_SOLAR_LUNAR();
				date.syear = tCursor.wYear;
				date.smon = (char)tCursor.wMonth;
				date.sday = day;
				SolarLunar.ConvertSolarToLunar(date);

				// lunar diaplay
				//dc.SetTextColor(DARK_GRAY_COLOR);
				buf = String.Format("{0}/{1}", date.lmon, date.lday);
				r.left = x1+1;
				r.top = y1+1;
				r.right = x2-1;
				r.bottom = y2-1;
				g.DrawString(buf, fontSmall, Brushes.DarkGray, r.left, r.top);
				//dc.SelectObject(old_font);
					
				r.left = x1+3;
				r.top = y1+3;
				r.right = x2-11;
				r.bottom = y2-3;

				bSpecial = Holiday.IsSpecialDay(tCursor.wYear, tCursor.wMonth, day, out special_text);
				bHoliday = Holiday.IsHoliday(tCursor.wYear, tCursor.wMonth, day, out holiday_text);

				if(bSpecial) 
				{
					//SetTextColor(hdc, RGB(255, 0, 0));
					color = Color.FromArgb(255, 0, 0);
					buf = String.Format("{0}({1})", day, special_text);
				}
				else 
				{
					if(bHoliday) 
					{
						color = Color.FromArgb(255, 0, 0);
						buf = String.Format("{0}({1})", day, holiday_text);
					}
					else 
					{
						if(week == 0)		color = Color.FromArgb(255, 0, 0);
						else				color = Color.Black;
						buf = String.Format("{0}", day);
					}
				}

				Color color_b;
				if(ScheduleGetDayColor(tCursor.wYear, tCursor.wMonth, day, out color_b, week, bHoliday, bSpecial)) 
					DrawClass.PushBox2(g, x2-10, y1+3, x2-3, y2-3, color_b);

				DrawClass.DrawText(g, buf, fontMain, new SolidBrush(color), r, formatcenter);

				if(week >= 6) 
				{
					y += nHeight+5;
					week=0;
				}
				else
					week++;
			}

			y = 5+(nHeight+5)*2;

			DrawClass.PopBox2(g,  5+7*(nWidth+5), y, rect.right-1, rect.bottom-1, Color.LightGray);
			DrawClass.PushBox2(g, 5+7*(nWidth+5)+2, y+cyChar+3, rect.right-1-2, rect.bottom-1-2, Color.LightGray);

			r.left = 5+7*(nWidth+5)+2;
			r.top = y+3;
			r.right = rect.right-3;
			r.bottom = r.top+cyChar-1;

			//dc.SetTextColor(DARK_COLOR);

			if(Tools.IsLangKorean()) 
				DrawClass.DrawText(g, "운전모드", fontMain, Brushes.Black, r, formatcenter);
			else if(Tools.IsLangJapanese()) 
				DrawClass.DrawText(g, "運転モード", fontMain, Brushes.Black, r, formatcenter);
			else if(Tools.IsLangChinese())
                DrawClass.DrawText(g, "操作方式", fontMain, Brushes.Black, r, formatcenter);
            else if (Tools.IsLangVietnamese())
                DrawClass.DrawText(g, "Chế độ hoạt động", fontMain, Brushes.Black, r, formatcenter);
			else 
				DrawClass.DrawText(g, "Operation Mode", fontMain, Brushes.Black, r, formatcenter);

			y = 5+(nHeight+5)*2+cyChar+4;

			for(l = 0; l < Schedule.blockScheduleFixed.Count; l++, y+=cyChar) 
			{
				sc = (SCHEDULE_STRUCT)Schedule.blockScheduleFixed[l];
				r.top = y;
				r.bottom = y+cyChar;
				DrawClass.PushBox2(g, r.left+2, r.top+1, r.left+2+cxChar*2, r.bottom-1, sc.color);
				DrawClass.DrawText(g, sc.title, fontMain, Brushes.Black, r, formatcenter);
			}

            if (CheckEngineSchedule.bActiveSchedule == false)
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;

                Font font = new Font("Arial", 20);

                if (Tools.IsLangKorean())
                    g.DrawString("전체 스케쥴 정지 중", font, Brushes.Red, ClientRectangle, format);
                else
                    g.DrawString("All Schedule Stopped.", font, Brushes.Red, ClientRectangle, format);
            }
		}

		private void panelMain_SizeChanged(object sender, System.EventArgs e)
		{
			nHeight = (panelMain.ClientRectangle.Height-7*5-5)/7;
			nWidth  = (panelMain.ClientRectangle.Width-9*5)/8;	
			panelMain.Invalidate();
		}

		private void panelMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)	return;

			RECT rect = new RECT();

			rect.left = this.panelMain.ClientRectangle.Left;
			rect.top = this.panelMain.ClientRectangle.Top;
			rect.right = this.panelMain.ClientRectangle.Right;
			rect.bottom = this.panelMain.ClientRectangle.Bottom;

			if(e.Y >= 5 && e.Y <= 5+nHeight+5) 
			{
                FormSchedule.ConfigScheduleWeek();
                //FormConfigScheduleWeek dialog = new FormConfigScheduleWeek();
                //if(dialog.ShowDialog() == DialogResult.OK)	this.panelMain.Invalidate();
                //return;
			}

			/*
			if(e.X >= rect.right-cxChar*20) 
			{
				FormConfigSchedule dialog = new FormConfigSchedule();
				if(dialog.ShowDialog() == DialogResult.OK)
					this.panelMain.Invalidate();
				return;
			}
			*/

			int week;
			int y;
			int day;
			int x1, y1, x2, y2;

			y = 5;
			y += nHeight+5;

			week = TimeUtil.GetWeekDay(tCursor.wYear, tCursor.wMonth, 1);

			int month_limit = TimeUtil.getmonthlimit(tCursor.wYear, tCursor.wMonth);

			for(day = 1; day <= month_limit; day++) 
			{
				x1 = 5+week*(nWidth+5);
				y1 = y;
				x2 = x1+nWidth;
				y2 = y+nHeight;

				if(e.X >= x1 && e.Y >= y1 && e.X <= x2 && e.Y <= y2) 
				{
					tCursor.wDay = (ushort)day;
					this.panelMain.Invalidate();
					wndDay.ChangeDay(tCursor);
					break;
				}

				if(week >= 6) 
				{
					y += nHeight+5;
					week=0;
				}
				else
					week++;
			}		
		}

		void UpdateLabel()
		{
			if(Tools.IsLangKorean()) 
			{
				this.label1.Text = String.Format("{0}년 {1}월", tCursor.wYear, tCursor.wMonth);
			}
			else if(Tools.IsLangJapanese()) 
			{
				this.label1.Text = String.Format("{0}年 {1}月", tCursor.wYear, tCursor.wMonth);
			}
			else if(Tools.IsLangChinese()) 
			{
				this.label1.Text = String.Format("{0}年 {1}月", tCursor.wYear, tCursor.wMonth);
			}
			else 
			{
				this.label1.Text = String.Format("{0} - {1}", tCursor.wYear, tCursor.wMonth);
			}
		}

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(e.Type == ScrollEventType.SmallDecrement) 
			{
				TimeUtil.PlusMonth(tCursor);
				if(tCursor.wDay > TimeUtil.getmonthlimit(tCursor.wYear, tCursor.wMonth)) 
				{
					tCursor.wDay = (ushort)TimeUtil.getmonthlimit(tCursor.wYear, tCursor.wMonth);
				}
				this.panelMain.Invalidate();
				wndDay.ChangeDay(tCursor);
				UpdateLabel();
			}
			else if(e.Type == ScrollEventType.SmallIncrement) 
			{
				TimeUtil.MinusMonth(tCursor);
				if(tCursor.wDay > TimeUtil.getmonthlimit(tCursor.wYear, tCursor.wMonth)) 
				{
					tCursor.wDay = (ushort)TimeUtil.getmonthlimit(tCursor.wYear, tCursor.wMonth);
				}
				this.panelMain.Invalidate();
				wndDay.ChangeDay(tCursor);	
				UpdateLabel();
			}
			else {}
		}
	}
}

/*
void CWndSchedule::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	// TODO: Add your message handler code here and/or call default
	switch(nSBCode) {
		case SB_LINEUP:
			PlusMonth(&tCursor);
			InvalidateRect(NULL);
			wndDay->ChangeDay(&tCursor);
			break;
		case SB_LINEDOWN:
			MinusMonth(&tCursor);
			InvalidateRect(NULL);
			wndDay->ChangeDay(&tCursor);
			break;
	}
	
	CWnd::OnVScroll(nSBCode, nPos, pScrollBar);
}


*/