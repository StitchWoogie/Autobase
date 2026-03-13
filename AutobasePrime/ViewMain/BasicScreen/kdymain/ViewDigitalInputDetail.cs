using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using NetTools;
using DialogControl;
using System.Data;
using System.Drawing.Drawing2D;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewDigitalInputDetail.
	/// </summary>
	public class ViewDigitalInputDetail : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ContextMenu contextMenuDiDetail;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour1;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour8;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour24;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour48;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour72;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour720;
		private System.Windows.Forms.MenuItem menuItem_DiData_min;
		private System.Windows.Forms.MenuItem menuItem_DiData_hour;
		private System.Windows.Forms.MenuItem menuItem_DiData_day;
		private System.Windows.Forms.MenuItem menuItem_DiData_week;
		private System.Windows.Forms.MenuItem menuItem_DiData_month;

		struct DETAIL_VALUE
		{
			public bool		flag;
			public bool		val;
		};	

		DETAIL_VALUE[]		trendValue;
		public int			xnum = 80;
		public int			ynum = 27;

		bool				bFlag;		
		public string[]		sButtonBuf;
		private System.Windows.Forms.MenuItem menuItem_di_curr_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;
		private System.Windows.Forms.MenuItem menuItem_ViewSetting;
		private System.Windows.Forms.MenuItem menuItem4;
		
		BasicScreen.kdymain.ButtonCheck2 hansolButton = new BasicScreen.kdymain.ButtonCheck2();


		public ViewDigitalInputDetail(Form parent, string tag)
            
		{
            parentForm = parent;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.label1.Left = -100;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;

			sButtonBuf = new string[1];
			if(Tools.IsLangKorean()) sButtonBuf[0] = "출력[F3]";
			else if(Tools.IsLangJapanese()) sButtonBuf[0] = "出力[F3]";
			else if(Tools.IsLangChinese()) sButtonBuf[0] = "输出[F3]";
			else					 sButtonBuf[0] = "Out[F3]";
			work.eTagType = EnumTagType.DI;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;
			work.pos = TagLib.GetTagPosOnlyList(work.tagList, tag);
			if(work.pos <= 0) work.pos = 0;

			DiDetailInitValueSetting();
			DetailWindowSetSize(xnum, ynum);

			TagDiClass di = TagLib.GetStructDI(work.tagList[work.pos]);
			work.iOldValue = (double)di.curr;
			DiDetailSetTitle();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputDetail));
            this.contextMenuDiDetail = new System.Windows.Forms.ContextMenu();
            this.menuItem_DiTrend_hour1 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour8 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour24 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour48 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour72 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour720 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_min = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_hour = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_day = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_week = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_month = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_di_curr_change = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem_ViewSetting = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // contextMenuDiDetail
            // 
            this.contextMenuDiDetail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_DiTrend_hour1,
            this.menuItem_DiTrend_hour8,
            this.menuItem_DiTrend_hour24,
            this.menuItem_DiTrend_hour48,
            this.menuItem_DiTrend_hour72,
            this.menuItem_DiTrend_hour720,
            this.menuItem2,
            this.menuItem_DiData_min,
            this.menuItem_DiData_hour,
            this.menuItem_DiData_day,
            this.menuItem_DiData_week,
            this.menuItem_DiData_month,
            this.menuItem1,
            this.menuItem_di_curr_change,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem_ViewSetting,
            this.menuItem4,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuDiDetail, "contextMenuDiDetail");
            // 
            // menuItem_DiTrend_hour1
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour1, "menuItem_DiTrend_hour1");
            this.menuItem_DiTrend_hour1.Index = 0;
            this.menuItem_DiTrend_hour1.Click += new System.EventHandler(this.menuItem_DiTrend_hour1_Click);
            // 
            // menuItem_DiTrend_hour8
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour8, "menuItem_DiTrend_hour8");
            this.menuItem_DiTrend_hour8.Index = 1;
            this.menuItem_DiTrend_hour8.Click += new System.EventHandler(this.menuItem_DiTrend_hour8_Click);
            // 
            // menuItem_DiTrend_hour24
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour24, "menuItem_DiTrend_hour24");
            this.menuItem_DiTrend_hour24.Index = 2;
            this.menuItem_DiTrend_hour24.Click += new System.EventHandler(this.menuItem_DiTrend_hour24_Click);
            // 
            // menuItem_DiTrend_hour48
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour48, "menuItem_DiTrend_hour48");
            this.menuItem_DiTrend_hour48.Index = 3;
            this.menuItem_DiTrend_hour48.Click += new System.EventHandler(this.menuItem_DiTrend_hour48_Click);
            // 
            // menuItem_DiTrend_hour72
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour72, "menuItem_DiTrend_hour72");
            this.menuItem_DiTrend_hour72.Index = 4;
            this.menuItem_DiTrend_hour72.Click += new System.EventHandler(this.menuItem_DiTrend_hour72_Click);
            // 
            // menuItem_DiTrend_hour720
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour720, "menuItem_DiTrend_hour720");
            this.menuItem_DiTrend_hour720.Index = 5;
            this.menuItem_DiTrend_hour720.Click += new System.EventHandler(this.menuItem_DiTrend_hour720_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 6;
            // 
            // menuItem_DiData_min
            // 
            resources.ApplyResources(this.menuItem_DiData_min, "menuItem_DiData_min");
            this.menuItem_DiData_min.Index = 7;
            this.menuItem_DiData_min.Click += new System.EventHandler(this.menuItem_DiData_min_Click);
            // 
            // menuItem_DiData_hour
            // 
            resources.ApplyResources(this.menuItem_DiData_hour, "menuItem_DiData_hour");
            this.menuItem_DiData_hour.Index = 8;
            this.menuItem_DiData_hour.Click += new System.EventHandler(this.menuItem_DiData_hour_Click);
            // 
            // menuItem_DiData_day
            // 
            resources.ApplyResources(this.menuItem_DiData_day, "menuItem_DiData_day");
            this.menuItem_DiData_day.Index = 9;
            this.menuItem_DiData_day.Click += new System.EventHandler(this.menuItem_DiData_day_Click);
            // 
            // menuItem_DiData_week
            // 
            resources.ApplyResources(this.menuItem_DiData_week, "menuItem_DiData_week");
            this.menuItem_DiData_week.Index = 10;
            this.menuItem_DiData_week.Click += new System.EventHandler(this.menuItem_DiData_week_Click);
            // 
            // menuItem_DiData_month
            // 
            resources.ApplyResources(this.menuItem_DiData_month, "menuItem_DiData_month");
            this.menuItem_DiData_month.Index = 11;
            this.menuItem_DiData_month.Click += new System.EventHandler(this.menuItem_DiData_month_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 12;
            // 
            // menuItem_di_curr_change
            // 
            resources.ApplyResources(this.menuItem_di_curr_change, "menuItem_di_curr_change");
            this.menuItem_di_curr_change.Index = 13;
            this.menuItem_di_curr_change.Click += new System.EventHandler(this.menuItem_di_curr_change_Click);
            // 
            // menuItem_TagProperityModify
            // 
            resources.ApplyResources(this.menuItem_TagProperityModify, "menuItem_TagProperityModify");
            this.menuItem_TagProperityModify.Index = 14;
            this.menuItem_TagProperityModify.Click += new System.EventHandler(this.menuItem_TagProperityModify_Click);
            // 
            // menuItem_HandInput
            // 
            resources.ApplyResources(this.menuItem_HandInput, "menuItem_HandInput");
            this.menuItem_HandInput.Index = 15;
            this.menuItem_HandInput.Click += new System.EventHandler(this.menuItem_HandInput_Click);
            // 
            // menuItem_ViewSetting
            // 
            resources.ApplyResources(this.menuItem_ViewSetting, "menuItem_ViewSetting");
            this.menuItem_ViewSetting.Index = 16;
            this.menuItem_ViewSetting.Click += new System.EventHandler(this.menuItem_ViewSetting_Click);
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 17;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 18;
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ViewDigitalInputDetail
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.Name = "ViewDigitalInputDetail";
            this.Load += new System.EventHandler(this.ViewDigitalInputDetail_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputDetail_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewDigitalInputDetail_Paint);
            this.Closed += new System.EventHandler(this.ViewDigitalInputDetail_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputDetail_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputDetail_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion
	
		private void DiDetailTrendMemoryAllocation()
		{
			int		i;

            work.dt = DateTimeServer.Now;
			work.remain_mili_sec = 0;
			work.old_mili_sec = work.dt.Second*1000+work.dt.Millisecond;
			work.trend_hap = work.trend_min*120/work.trend_width + 1;
			work.trend_pos = 0;
			trendValue = new DETAIL_VALUE[work.trend_hap];
			for(i = 0; i < work.trend_hap; i++) 
			{
				trendValue[i].flag = false;
				trendValue[i].val = false;
			}
		}


		private void DiDetailInitValueSetting()
		{
			work.spos = 0;//work.nSendTagPos;
			work.Ix = 0;
			work.Iy = 0;
			work.trend_min = 1;
			work.trend_width = 1;		// 0.5 초
			DiDetailTrendMemoryAllocation();			
		}

		void DiDetailSetTitle()
		{
			ViewDigitalInputDetailMain	form = ViewDigitalInputDetailMain.formThis;
			TagDiClass di = TagLib.GetStructDI(work.tagList[work.pos]);

			if(Tools.IsLangKorean()) form.Text = string.Format("디지털 입력 상세- {0}", di.name);
			else if(Tools.IsLangJapanese()) form.Text = string.Format("デジタル入力の詳細表示- {0}", di.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("详细查看数字输入- {0}", di.name);
			else form.Text = string.Format("Digital Input Detail - {0}", di.name);
		}

		//Bitmap bitmapClient = null;

		private void ViewDigitalInputDetail_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics	gScreen = e.Graphics;

			DetailWindowSetSize(xnum, ynum);
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.
			TagDiClass di = TagLib.GetStructDI(work.tagList[work.pos]);

			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);

			//Graphics g = Graphics.FromImage(bitmapClient);
            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;
			
			AdDetailFirstScreen(g);
			DiDetailAllValueDraw(g, di);
			DigitalDetailGraphDraw(g, di);
			DigitalDetailCurrentValueDraw(g, di);
			DiDetailTrendDraw(g, di);

			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		void DiDetailAllValueDraw(Graphics g, TagDiClass di)
		{
			int 				 		y, Ix, Iy, fontX, fontY, size;
			string 				 		buf;
			StringFormat				format = new StringFormat();
			string						undefinedBuf;
			
			if(Tools.IsLangKorean()) undefinedBuf = "설정없음";
			else if(Tools.IsLangJapanese()) undefinedBuf = "設定ない";
			else if(Tools.IsLangChinese()) undefinedBuf = "未设置";
			else					 undefinedBuf = "UnDefined";

			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;
			size = (int)(fontY*1.3);

			DrawClass.PopBox2(g, Ix+(int)(fontX*1.0), Iy+(int)(fontY*21), Ix+fontX*79, Iy+(int)(fontY*25.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.3), Iy+(int)(fontY*21.1), Ix+(int)(fontX*78.8), Iy+(int)(fontY*25.4), work.grayColor);
			
			y = Iy+(int)(fontY*21.5);
			format.Alignment = StringAlignment.Far;

			if(Tools.IsLangKorean())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*11, fontY, "On 설명:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*11, fontY, "Off 설명:", Color.Blue, work.grayColor, work.f, format);//DT_RIGHT);	
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*11, fontY, "경보설정:", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
			}
			else if(Tools.IsLangJapanese())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*11, fontY, "On 説明:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*11, fontY, "Off 説明:", Color.Blue, work.grayColor, work.f, format);//DT_RIGHT);	
                DrawClass.WinDrawText(g, Ix + fontX, y + size * 2, fontX * 11, fontY, "警報設定:", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
			}
			else if(Tools.IsLangChinese())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*11, fontY, "ON 描述:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*11, fontY, "OFF 描述:", Color.Blue, work.grayColor, work.f, format);//DT_RIGHT);	
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*11, fontY, "设置警报:", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
			}
			else 
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*11, fontY, "On Desc:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*11, fontY, "Off Sesc:", Color.Blue, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*11, fontY, "Set Alarm:", Color.Black, work.grayColor, work.f, format);
			}

			switch(di.cAlarmType)
			{
				case 0: 
					if(Tools.IsLangKorean()) buf = String.Format("ON 일때");
					else					 buf = String.Format("ON");
					break;
				case 1: 
					if(Tools.IsLangKorean()) buf = String.Format("OFF 일때");
					else					 buf = String.Format("OFF");
					break;
				case 2: buf = String.Format("OFF-ON-OFF");  	break;
				case 3: buf = String.Format("ON-OFF-ON"); 		break;  
				case 4:
				case 5: buf = String.Format("ON, OFF"); 		break;
				default:
					if(Tools.IsLangKorean()) buf = String.Format("새 경보");
					else					 buf = String.Format("New Alarm");
					break;
			}			
			format.Alignment = StringAlignment.Near;
			DrawClass.WinDrawText(g, Ix+fontX*12, y+size*2, fontX*15, fontY, buf, Color.Black, work.grayColor, work.f, format);
			buf = String.Format("{0}", di.desON);
			DrawClass.WinDrawText(g, Ix+fontX*12, y, fontX*15, fontY, buf, Color.Red, work.grayColor, work.f, format);	
			buf = String.Format("{0}", di.desOFF);
			DrawClass.WinDrawText(g, Ix+fontX*12, y+size, fontX*15, fontY, buf, Color.Blue, work.grayColor, work.f, format);

			format.Alignment = StringAlignment.Far;
			if(Tools.IsLangKorean())
				DrawClass.WinDrawText(g, Ix+fontX*27, y, fontX*12, fontY, "OUT(출력):", Color.DarkMagenta, work.grayColor, work.f, format);
			else if(Tools.IsLangJapanese())
				DrawClass.WinDrawText(g, Ix+fontX*27, y, fontX*12, fontY, "OUT(出力):", Color.DarkMagenta, work.grayColor, work.f, format);
			else if(Tools.IsLangChinese())
				DrawClass.WinDrawText(g, Ix+fontX*27, y, fontX*12, fontY, "OUT(输出):", Color.DarkMagenta, work.grayColor, work.f, format);
			else
				DrawClass.WinDrawText(g, Ix+fontX*27, y, fontX*12, fontY, "Output:", Color.DarkMagenta, work.grayColor, work.f, format);
			DrawClass.WinDrawText(g, Ix+fontX*27, y+size, fontX*12, fontY, "D/O-ON:", Color.DarkBlue, work.grayColor, work.f, format);
			DrawClass.WinDrawText(g, Ix+fontX*27, y+size*2, fontX*12, fontY, "D/O-OFF:", Color.DarkBlue, work.grayColor, work.f, format);
			
			format.Alignment = StringAlignment.Near;
			if(di.nSubOutDigital1[0] != TagLib.TAG_NOT_FOUND)	DrawClass.WinDrawText(g, Ix+fontX*39, y, fontX*20, fontY, di.sSubOutDigital1, Color.DarkMagenta, work.grayColor, work.f, format);
			else				        DrawClass.WinDrawText(g, Ix+fontX*39, y, fontX*20, fontY, undefinedBuf, Color.DarkMagenta, work.grayColor, work.f, format);
			if(di.nSubOutDigital1[0] != TagLib.TAG_NOT_FOUND) DrawClass.WinDrawText(g, Ix+fontX*39, y+size, fontX*20, fontY, di.sSubOutDigitalOnTag, Color.DarkBlue, work.grayColor, work.f, format);//DT_LEFT);			
			else						DrawClass.WinDrawText(g, Ix+fontX*39, y+size, fontX*20, fontY, undefinedBuf, Color.DarkBlue, work.grayColor, work.f, format);				
			if(di.nSubOutDigitalOffTag[0] != TagLib.TAG_NOT_FOUND) DrawClass.WinDrawText(g, Ix+fontX*39, y+size*2, fontX*20, fontY, di.sSubOutDigitalOffTag, Color.DarkBlue, work.grayColor, work.f, format);			
			else							 DrawClass.WinDrawText(g, Ix+fontX*39, y+size*2, fontX*20, fontY, undefinedBuf, Color.DarkBlue, work.grayColor, work.f, format);

			format.Alignment = StringAlignment.Far;
			switch(di.cTagLinkType)
			{
				case 0 :
					DrawClass.WinDrawText(g, Ix+fontX*60, y, fontX*9, fontY, "Port :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*60, y+size, fontX*9, fontY, "Address :", Color.DarkGreen, work.grayColor, work.f, format);

					format.Alignment = StringAlignment.Near;
					buf = String.Format("{0,3:d3}", di.port);
					DrawClass.WinDrawText(g, Ix+fontX*69, y, fontX*10, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					buf = String.Format("{0}.{1:X}", di.address_word, di.address_bit);
					DrawClass.WinDrawText(g, Ix+fontX*69, y+size, fontX*10, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					break;
				case 1 :			
					DrawClass.WinDrawText(g, Ix+fontX*60, y, fontX*9, fontY, "Service:", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*60, y+size, fontX*9, fontY, "Topic:", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*60, y+size*2, fontX*9, fontY, "Item :", Color.DarkGreen, work.grayColor, work.f, format);

					format.Alignment = StringAlignment.Near;
					buf = String.Format("{0}", di.sDdeService);			
					DrawClass.WinDrawText(g, Ix+fontX*69, y, fontX*10, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);			
					buf = String.Format("{0}", di.sDdeTopic);	
					DrawClass.WinDrawText(g, Ix+fontX*69, y+size, fontX*10, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					buf = String.Format("{0}", di.sDdeItem);	
					DrawClass.WinDrawText(g, Ix+fontX*69, y+size*2, fontX*10, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					break;
				case 2 :
					if(Tools.IsLangKorean())
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "메모리 태그", Color.DarkGreen, work.grayColor, work.f, format);
					else if(Tools.IsLangJapanese())
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "メモリー タグ", Color.DarkGreen, work.grayColor, work.f, format);
					else if(Tools.IsLangChinese())
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "内存 标记", Color.DarkGreen, work.grayColor, work.f, format);
					else
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "Memory Tag", Color.DarkGreen, work.grayColor, work.f, format);
					break;
				case 3 :
					if(Tools.IsLangKorean())
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "간 접 태 그", Color.DarkGreen, work.grayColor, work.f, format);
					else if(Tools.IsLangJapanese())
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "間接タグ", Color.DarkGreen, work.grayColor, work.f, format);
					else if(Tools.IsLangChinese())
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "间接 标记", Color.DarkGreen, work.grayColor, work.f, format);
					else
						DrawClass.WinDrawText(g, Ix+fontX*61, y+size, fontX*17, fontY, "Indirect Tag", Color.DarkGreen, work.grayColor, work.f, format);
					break;				
			}
		}


		void DigitalDetailGraphDraw(Graphics g, TagDiClass di)
		{
			int 						Ix, Iy, fontX, fontY;
			string 						buf;
			StringFormat				format = new StringFormat();
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*20.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), Ix+fontX*19, Iy+(int)(fontY*19), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*19.9), Iy+(int)(fontY*4.4), Ix+(int)(fontX*76.1), Iy+(int)(fontY*18.6), Color.White);

			format.Alignment = StringAlignment.Center;
			buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
			DrawClass.WinDrawText(g, Ix+fontX*2,  Iy+(int)(fontY*19.5), fontX*16, fontY, buf, Color.Black, work.grayColor, work.f, format);	

			if(Tools.IsLangKorean()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "태그:", di.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "설명:", di.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "タグ:", di.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "説明:", di.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "标记:", di.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "描述:", di.description, Color.Black, work.grayColor);
			}
			else
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "Tag:", di.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "Desc:", di.description, Color.Black, work.grayColor);
			}
			
			format.Alignment = StringAlignment.Far;

			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*12, fontY, "현재상태 :",  Color.Black, work.f, format);
			else if(Tools.IsLangJapanese()) 
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*12, fontY, "現在値 :",  Color.Black, work.f, format);
			else if(Tools.IsLangChinese()) 
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*12, fontY, "现在值 :",  Color.Black, work.f, format);
			else
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*12, fontY, "Value :",  Color.Black, work.f, format);

			DrawClass.PushBox2(g, Ix+(int)(fontX*63.5), Iy+fontY, Ix+(int)(fontX*77), Iy+fontY*2, Color.Blue);
			
			DrawClass.PushBox2(g, Ix+fontX*14, Iy+(int)(fontY*5), Ix+fontX*16, Iy+(int)(fontY*5.25), work.grayColor);
			DrawClass.PushBox2(g, Ix+fontX*14, Iy+(int)(fontY*17.75), Ix+fontX*16, Iy+(int)(fontY*18), work.grayColor);
			DrawClass.PushBox2(g, Ix+fontX*16, Iy+(int)(fontY*5), Ix+(int)(fontX*16.5), Iy+(int)(fontY*18), work.grayColor);

			format.Alignment = StringAlignment.Center;
			if(di.desON.Length > 0) buf = String.Format("{0}", di.desON);
			else                 buf = String.Format("ON");
			DrawClass.WinDrawText(g, Ix+fontX*9, Iy+(int)(fontY*5.5), fontX*7, fontY, buf, Color.Red, work.grayColor, work.f, format);
			
			if(di.desOFF.Length > 0) buf = String.Format("{0}", di.desOFF);
			else					buf = String.Format("OFF");						
			DrawClass.WinDrawText(g, Ix+fontX*9, Iy+(int)(fontY*16.5), fontX*7, fontY, buf, Color.Blue, work.grayColor, work.f, format);
	
			hansolButton.ButtonCheckDraw2(g, Ix+fontX*7, Iy+(int)(fontY*10.5), Ix+(int)(fontX*15.5), Iy+(int)(fontY*13.5), work.f, sButtonBuf[0], StringAlignment.Center);
			DownLinePosDraw(g, fontX, fontY, Ix+fontX*20, Iy+(int)(fontY*18.8), fontX*56, 24);
			for(int i = 0; i < 7; i++) 
			{
				if(Tools.IsLangKorean())
					buf = String.Format("{0}초", -(work.trend_min*60)/6*i);
				else if(Tools.IsLangJapanese())
					buf = String.Format("{0}秒", -(work.trend_min*60)/6*i);
				else if(Tools.IsLangChinese())
					buf = String.Format("{0}秒", -(work.trend_min*60)/6*i);
				else
					buf = String.Format("{0}S", -(work.trend_min*60)/6*i);
				DrawClass.GrayDrawText(g, Ix+fontX*17+i*fontX*28/3, Iy+(int)(fontY*19.5), fontX*6, fontY, buf, work.grayColor, work.f, format);
			}
		}

		void DigitalDetailCurrentValueDraw(Graphics g, TagDiClass di)
		{ 
			int							Ix, Iy, fontX, fontY;
			string						buf;
			StringFormat				format = new StringFormat();
			Brush						br;
		
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.gcls(g, Ix+fontX*4, Iy+(int)(fontY*5), Ix+fontX*8, Iy+(int)(fontY*7.5), work.grayColor);
			DrawClass.gcls(g, Ix+fontX*4, Iy+(int)(fontY*15.5), Ix+fontX*8, Iy+(int)(fontY*18.5), work.grayColor);

			DrawClass.gcls(g, work.Ix+work.fontX*64, work.Iy+work.fontY, work.Ix+work.fontX*76, work.Iy+work.fontY*2-1, Color.Blue);
			DrawClass.PushBox2(g, Ix+(int)(fontX*5.5), Iy+(int)(fontY*5.1), Ix+(int)(fontX*6.5), Iy+(int)(fontY*17.5), Color.DarkGray);

			format.Alignment = StringAlignment.Center;
			if(di.curr == 1) 
			{		
				if(di.desON.Length > 0) buf = String.Format("{0,4:S4}", di.desON);
				else                    buf = String.Format("ON");
				DrawClass.WinDrawText(g, Ix+fontX*64, Iy+fontY, fontX*12, fontY, buf, Color.Red, Color.Blue, work.f, format);
				br = new SolidBrush(Color.Red);
				g.FillEllipse(br, Ix+fontX*4, Iy+(int)(fontY*5), fontX*4, fontY*2);
			}
			else 
			{
				
				if(di.desOFF.Length > 0) buf = String.Format("{0,4:S4}", di.desOFF);
				else                     buf = String.Format("OFF");
				DrawClass.WinDrawText(g, Ix+fontX*64, Iy+fontY, fontX*12, fontY, buf, Color.White, Color.Blue, work.f, format);
				br = new SolidBrush(Color.Blue);
				g.FillEllipse(br, Ix+fontX*4, Iy+(int)(fontY*16), fontX*4, fontY*2);
			}			
		}

		void DiDetailTrendDraw(Graphics g, TagDiClass di)
		{
			int 							x;
			Color							iColor;
			Brush							br;			
			
			x = work.Ix+work.fontX*20;			
			if(di.curr == 1) 
			{
				iColor = (bFlag) ? Color.Red : Color.DarkGray;
				br = new SolidBrush(iColor);
				g.FillEllipse(br, work.Ix+work.fontX*4, work.Iy+(int)(work.fontY*5), work.fontX*4, work.fontY*2);
			}
			else 
			{
				iColor = (bFlag) ? Color.Blue : Color.DarkGray;
				br = new SolidBrush(iColor);
				g.FillEllipse(br, work.Ix+work.fontX*4, work.Iy+(int)(work.fontY*16), work.fontX*4, work.fontY*2);
			}
			DigitalTrendDrawLine(g, x, work.Iy+(int)(work.fontY*5), work.Iy+(int)(work.fontY*18), work.fontX*56+1, work.trend_hap);
			
		}


		private void DiDetailTrendValuseSetting(TagDiClass di)
		{
			int 							i;			
			
			for(i = work.trend_hap-2; i >= 0; i--) 
			{
				trendValue[i+1].flag = trendValue[i].flag;
				trendValue[i+1].val = trendValue[i].val;
			}
			trendValue[0].flag = true;
			trendValue[0].val = (di.curr == 1) ? true : false;			
		}

		
		void DigitalTrendDrawLine(Graphics g, int startX, int startY, int endY, int width, int count)
		{
			int 		i, sx, ex, midY;
			int			hap = count-1;
			
			//bool		flag = false;

			if(count < 2) return;
			//count--;

			midY = (startY+endY)/2;			

			for(i = 1; i < count; i++)
			{
				if(trendValue[i-1].flag == false || trendValue[i].flag == false) continue;

				if(i == 1) sx = startX;
				else	   sx = startX+(int)(((double)width/hap)*(i-1));
				if(i == count-1) ex = startX+width;
				else			 ex = startX+(int)(((double)width/hap)*i);
				
				if(trendValue[i].val == true)
					DrawClass.gline(g, sx, startY, ex, startY, Color.Red);
				else
					DrawClass.gline(g, sx, endY, ex, endY, Color.Blue);

				if(trendValue[i-1].val != trendValue[i].val) 
				{
					if(trendValue[i-1].val == true) 
					{
						DrawClass.gline(g, sx, startY, sx, midY, Color.Red);//DARK_CYAN);
						DrawClass.gline(g, sx, midY, sx, endY, Color.Blue);//DARK_YELLOW);
					}
					else 
					{
						DrawClass.gline(g, sx, endY, sx, midY, Color.Blue);//DARK_YELLOW);
						DrawClass.gline(g, sx, midY, sx, startY, Color.Red);//DARK_CYAN);
					}
				}
			}
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            DateTime dt = DateTimeServer.Now;
			long		miliSec, hap;			

			if(work.bTagChanged) 
			{
				DiDetailTrendMemoryAllocation();
				DiDetailSetTitle();
				work.remain_mili_sec = work.trend_width*500;	// 화면을 그릴시간이다.
				work.bTagChanged = false;				
			}
			TagDiClass di = TagLib.GetStructDI(work.tagList[work.pos]);
			di.NeedDataCurr = true;

			miliSec = dt.Second*1000+dt.Millisecond;
			hap = (miliSec >= work.old_mili_sec) ? miliSec-work.old_mili_sec : 60000+miliSec-work.old_mili_sec;
			work.old_mili_sec = miliSec;
			work.remain_mili_sec += hap;
			if(work.remain_mili_sec < work.trend_width*500) return;

			Rectangle		rect;
			double			oldVal = work.iOldValue;			
			bFlag = (bFlag == true) ? false : true;

			while(work.remain_mili_sec >= work.trend_width*500)
			{			
				DiDetailTrendValuseSetting(di);
				if(di.curr != (int)work.iOldValue) work.iOldValue = (double)di.curr;
				work.remain_mili_sec -= work.trend_width*500;					
			}

			if(di.curr == 1) 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*4, work.Iy+(int)(work.fontY*5), work.fontX*4, work.fontY*2);
				this.Invalidate(rect);
			}
			else 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*4, work.Iy+(int)(work.fontY*16), work.fontX*4, work.fontY*2);
				this.Invalidate(rect);				
			}


			if((double)di.curr != oldVal) 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*4, work.Iy+(int)(work.fontY*5), work.fontX*4, work.fontY*3);
				this.Invalidate(rect);
				rect = new Rectangle(work.Ix+(int)work.fontX*4, work.Iy+(int)(work.fontY*15.5), work.fontX*4, work.fontY*3);
				this.Invalidate(rect);
				rect = new Rectangle(work.Ix+work.fontX*64, work.Iy+work.fontY, work.fontX*12, work.fontY);
				this.Invalidate(rect);
			}
			rect = new Rectangle(work.Ix+(int)(work.fontX*20), work.Iy+(int)(work.fontY*4.5), work.fontX*56+1, work.fontY*14+1);
			this.Invalidate(rect);
		}

		public void CallDiDetailSettingDialog()
		{
			int			oldMin, oldPeriod;
			ViewDigitalInputDetailDlgSetting dlg = new ViewDigitalInputDetailDlgSetting();

			oldMin = work.trend_min;
			oldPeriod = work.trend_width;
			dlg.numericUpDown_total_time.Value = work.trend_min;
			dlg.comboBox_data_read_period.SelectedIndex = (work.trend_width-1);

			if(Tools.IsLangKorean()) dlg.Text = "디지털 상세보기 설정";
            else if (Tools.IsLangJapanese()) dlg.Text = "表示設定";
			else if(Tools.IsLangChinese()) dlg.Text = "设置详细查看数字输入";
			else dlg.Text = "Digital Input Detail Settings";

            dlg.StartPosition = FormStartPosition.CenterParent;
			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{				
				work.trend_min = (int)dlg.numericUpDown_total_time.Value;
				work.trend_width = (int)dlg.comboBox_data_read_period.SelectedIndex+1;

				if(oldMin != work.trend_min || oldPeriod != work.trend_width)
				{
					DiDetailTrendMemoryAllocation();
					this.Invalidate();
				}
			}			
		}		

		void DiDetailButtonLeftMouseCheck(MouseEventArgs e)
		{
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*7, work.Iy+(int)(work.fontY*10.5), work.Ix+(int)(work.fontX*15.5), work.Iy+(int)(work.fontY*13.5), work.f, sButtonBuf[0], 1)) return;
		}

		private void ViewDigitalInputDetail_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {

            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    Point pos = new Point(e.X, e.Y);
                    Control control = new Control(this, "Text");
                    this.contextMenuDiDetail.Show(control, pos);
                    return;
                }
            }

			if(e.Button == MouseButtons.Left) 
			{
				DiDetailButtonLeftMouseCheck(e);
				return;
			}
		}

		private void ViewDigitalInputDetail_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hansolButton.ButtonCheckMove2(this, e);		
		}

		private void ViewDigitalInputDetail_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int			id;

			id = hansolButton.ButtonCheckUp2(this);
			if(id == -1) return;

			switch(id) 
			{
				case 1 : CallDigitalOutputDialog(); return;
			}		
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewDigitalInputDetail_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
		}		

		private void ViewDigitalInputDetail_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
		}

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
		}

		private void menuItem_di_curr_change_Click(object sender, System.EventArgs e)
		{
			CallDigitalOutputDialog();
		}

		private void menuItem_TagProperityModify_Click(object sender, System.EventArgs e)
		{
			CallTagPropertyWindows();
		}

		private void menuItem_HandInput_Click(object sender, System.EventArgs e)
		{
			callDiHandInputDialog();
		}

		private void menuItem_ViewSetting_Click(object sender, System.EventArgs e)
		{
			CallDiDetailSettingDialog();
		}


        public void menuItem_DiTrend_hour1_Click(object sender, System.EventArgs e)
        {
            callDiTrendWindow(1);
        }

        public void menuItem_DiTrend_hour8_Click(object sender, System.EventArgs e)
        {
            callDiTrendWindow(8);
        }

        public void menuItem_DiTrend_hour24_Click(object sender, System.EventArgs e)
        {
            callDiTrendWindow(24);
        }

        public void menuItem_DiTrend_hour48_Click(object sender, System.EventArgs e)
        {
            callDiTrendWindow(48);
        }

        public void menuItem_DiTrend_hour72_Click(object sender, System.EventArgs e)
        {
            callDiTrendWindow(72);
        }

        public void menuItem_DiTrend_hour720_Click(object sender, System.EventArgs e)
        {
            callDiTrendWindow(720);
        }

        public void menuItem_DiData_min_Click(object sender, System.EventArgs e)
        {
            CallDiDataWindow(eDataTime.MIN);
        }

        public void menuItem_DiData_hour_Click(object sender, System.EventArgs e)
        {
            CallDiDataWindow(eDataTime.HOUR);
        }

        public void menuItem_DiData_day_Click(object sender, System.EventArgs e)
        {
            CallDiDataWindow(eDataTime.DAY);
        }

        public void menuItem_DiData_week_Click(object sender, System.EventArgs e)
        {
            CallDiDataWindow(eDataTime.WEEK);
        }

        public void menuItem_DiData_month_Click(object sender, System.EventArgs e)
        {
            CallDiDataWindow(eDataTime.MONTH);
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }
	}
}
