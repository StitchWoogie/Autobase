using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using System.Data;
using AutoLibLocal;
using System.Drawing.Drawing2D; 

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputDetail.
	/// </summary>
	public class ViewAnalogInputDetail : AnalogDigitalCommonDrawClass
	{
		private System.ComponentModel.IContainer components;

		private System.Windows.Forms.Timer timer1;
		
		struct DETAIL_VALUE
		{
			public bool		flag;
			public double	val;
		};
		public int xnum = 90;
		public int ynum = 28;

		DETAIL_VALUE[] trendValue;
		//TagListStruct[]		tagList;
		//TagAiClass			ai;
		//long				remain_mili_sec, old_mili_sec;
		private System.Windows.Forms.MenuItem menuItem_trend_hour1;
		private System.Windows.Forms.MenuItem menuItem_trend_hour8;
		private System.Windows.Forms.MenuItem menuItem_trend_hour24;
		private System.Windows.Forms.MenuItem menuItem_trend_hour48;
		private System.Windows.Forms.MenuItem menuItem_trend_hour72;
		private System.Windows.Forms.MenuItem menuItem_trend_hour720;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem_detail_setting;
        private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MenuItem menuItem_data_hour;
		private System.Windows.Forms.MenuItem menuItem_data_day;
		private System.Windows.Forms.MenuItem menuItem_data_week;
		private System.Windows.Forms.MenuItem menuItem_data_month;
		private System.Windows.Forms.MenuItem menuItem_data_min;
		private System.Windows.Forms.MenuItem menuItem_ai_setting_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
        private System.Windows.Forms.MenuItem menuItem_HandInput;
        private MenuItem menuItem_close;
		private System.Windows.Forms.ContextMenu contextMenuAiDetail;
		
		
		public ViewAnalogInputDetail(Form parent, string tag)
            
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
			work.eTagType = EnumTagType.AI;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else					 work.TagHap = work.tagList.Length;
			work.pos = TagLib.GetTagPosOnlyList(work.tagList, tag);
			if(work.pos <= 0) work.pos = 0;

			AiDetailInitValueSetting();
			DetailWindowSetSize(xnum, ynum);
			
			AiDetailSetTitle();
			TagAiClass			ai;
			ai = TagLib.GetStructAI(work.tagList[work.pos]);
			work.iOldValue = ai.curr;
			work.bTagChanged = false;			// 태그가 바뀌었는지 확인하기 위해...
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputDetail));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuAiDetail = new System.Windows.Forms.ContextMenu();
            this.menuItem_trend_hour1 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour8 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour24 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour48 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour72 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour720 = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_data_min = new System.Windows.Forms.MenuItem();
            this.menuItem_data_hour = new System.Windows.Forms.MenuItem();
            this.menuItem_data_day = new System.Windows.Forms.MenuItem();
            this.menuItem_data_week = new System.Windows.Forms.MenuItem();
            this.menuItem_data_month = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_ai_setting_change = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem_detail_setting = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem_close = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuAiDetail
            // 
            this.contextMenuAiDetail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_trend_hour1,
            this.menuItem_trend_hour8,
            this.menuItem_trend_hour24,
            this.menuItem_trend_hour48,
            this.menuItem_trend_hour72,
            this.menuItem_trend_hour720,
            this.menuItem1,
            this.menuItem_data_min,
            this.menuItem_data_hour,
            this.menuItem_data_day,
            this.menuItem_data_week,
            this.menuItem_data_month,
            this.menuItem2,
            this.menuItem_ai_setting_change,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem_detail_setting,
            this.menuItem4,
            this.menuItem_close});
            resources.ApplyResources(this.contextMenuAiDetail, "contextMenuAiDetail");
            // 
            // menuItem_trend_hour1
            // 
            resources.ApplyResources(this.menuItem_trend_hour1, "menuItem_trend_hour1");
            this.menuItem_trend_hour1.Index = 0;
            this.menuItem_trend_hour1.Click += new System.EventHandler(this.menuItem_trend_hour1_Click);
            // 
            // menuItem_trend_hour8
            // 
            resources.ApplyResources(this.menuItem_trend_hour8, "menuItem_trend_hour8");
            this.menuItem_trend_hour8.Index = 1;
            this.menuItem_trend_hour8.Click += new System.EventHandler(this.menuItem_trend_hour8_Click);
            // 
            // menuItem_trend_hour24
            // 
            resources.ApplyResources(this.menuItem_trend_hour24, "menuItem_trend_hour24");
            this.menuItem_trend_hour24.Index = 2;
            this.menuItem_trend_hour24.Click += new System.EventHandler(this.menuItem_trend_hour24_Click);
            // 
            // menuItem_trend_hour48
            // 
            resources.ApplyResources(this.menuItem_trend_hour48, "menuItem_trend_hour48");
            this.menuItem_trend_hour48.Index = 3;
            this.menuItem_trend_hour48.Click += new System.EventHandler(this.menuItem_trend_hour48_Click);
            // 
            // menuItem_trend_hour72
            // 
            resources.ApplyResources(this.menuItem_trend_hour72, "menuItem_trend_hour72");
            this.menuItem_trend_hour72.Index = 4;
            this.menuItem_trend_hour72.Click += new System.EventHandler(this.menuItem_trend_hour72_Click);
            // 
            // menuItem_trend_hour720
            // 
            resources.ApplyResources(this.menuItem_trend_hour720, "menuItem_trend_hour720");
            this.menuItem_trend_hour720.Index = 5;
            this.menuItem_trend_hour720.Click += new System.EventHandler(this.menuItem_trend_hour720_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 6;
            // 
            // menuItem_data_min
            // 
            resources.ApplyResources(this.menuItem_data_min, "menuItem_data_min");
            this.menuItem_data_min.Index = 7;
            this.menuItem_data_min.Click += new System.EventHandler(this.menuItem_data_min_Click);
            // 
            // menuItem_data_hour
            // 
            resources.ApplyResources(this.menuItem_data_hour, "menuItem_data_hour");
            this.menuItem_data_hour.Index = 8;
            this.menuItem_data_hour.Click += new System.EventHandler(this.menuItem_data_hour_Click);
            // 
            // menuItem_data_day
            // 
            resources.ApplyResources(this.menuItem_data_day, "menuItem_data_day");
            this.menuItem_data_day.Index = 9;
            this.menuItem_data_day.Click += new System.EventHandler(this.menuItem_data_day_Click);
            // 
            // menuItem_data_week
            // 
            resources.ApplyResources(this.menuItem_data_week, "menuItem_data_week");
            this.menuItem_data_week.Index = 10;
            this.menuItem_data_week.Click += new System.EventHandler(this.menuItem_data_week_Click);
            // 
            // menuItem_data_month
            // 
            resources.ApplyResources(this.menuItem_data_month, "menuItem_data_month");
            this.menuItem_data_month.Index = 11;
            this.menuItem_data_month.Click += new System.EventHandler(this.menuItem_data_month_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 12;
            // 
            // menuItem_ai_setting_change
            // 
            resources.ApplyResources(this.menuItem_ai_setting_change, "menuItem_ai_setting_change");
            this.menuItem_ai_setting_change.Index = 13;
            this.menuItem_ai_setting_change.Click += new System.EventHandler(this.menuItem_ai_setting_change_Click);
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
            // menuItem_detail_setting
            // 
            resources.ApplyResources(this.menuItem_detail_setting, "menuItem_detail_setting");
            this.menuItem_detail_setting.Index = 16;
            this.menuItem_detail_setting.Click += new System.EventHandler(this.menuItem_detail_setting_Click);
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 17;
            // 
            // menuItem_close
            // 
            resources.ApplyResources(this.menuItem_close, "menuItem_close");
            this.menuItem_close.Index = 18;
            this.menuItem_close.Click += new System.EventHandler(this.menuItem_close_Click);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // ViewAnalogInputDetail
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "ViewAnalogInputDetail";
            this.Load += new System.EventHandler(this.ViewAnalogInputDetail_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewAnalogInputDetail_Paint);
            this.Closed += new System.EventHandler(this.ViewAnalogInputDetail_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputDetail_MouseDown);
            this.ResumeLayout(false);

		}
		#endregion

		void AiDetailSetTitle()
		{
			ViewAnalogInputDetailMain	form = ViewAnalogInputDetailMain.formThis;
			TagAiClass			ai;
			ai = TagLib.GetStructAI(work.tagList[work.pos]);

			if(Tools.IsLangKorean()) form.Text = string.Format("아날로그 입력 상세- {0}", ai.name);
			else if(Tools.IsLangJapanese()) form.Text = string.Format("アナログ入力の詳細表示- {0}", ai.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("详细查看模拟输入- {0}", ai.name);
			else form.Text = string.Format("Analog Input Detail - {0}", ai.name);
		}
				
		private void AiDetailTrendMemoryAllocation()
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
				trendValue[i].val = 0.0;
			}
		}

		private void AiDetailInitValueSetting()
		{
			work.spos = 0;
			work.Ix = 0;
			work.Iy = 0;
			work.trend_min = 1;
			work.trend_width = 1;		// 0.5 초			
			AiDetailTrendMemoryAllocation();
		}

		//Bitmap bitmapClient = null;

		private void ViewAnalogInputDetail_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics					gScreen = e.Graphics;
			TagAiClass					ai;			

			DetailWindowSetSize(xnum, ynum);
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.
			ai = TagLib.GetStructAI(work.tagList[work.pos]);

			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);

			//Graphics g = Graphics.FromImage(bitmapClient);

            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;
			
			AdDetailFirstScreen(g);
			AnalogDetailAllValueDraw(g, ai);
			AnalogDetailGraphDraw(g);
			AnalogDetailCurrentValueTrend(g, ai);
			AnalogDetailTrendDraw(g, ai);

			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}		

		private void AnalogDetailAllValueDraw(Graphics g, TagAiClass ai)
		{
			int 						y, Ix, Iy, size, fontX, fontY;
			string 						buf;
			StringFormat				format = new StringFormat();
			string						undifinedBuf;
			
			if(Tools.IsLangKorean()) undifinedBuf = "설정없음";
			else if(Tools.IsLangJapanese()) undifinedBuf = "設定ない"; 
			else if(Tools.IsLangChinese()) undifinedBuf = "未设置"; 
			else					 undifinedBuf = "UnDefined";
			
			fontX = (int)work.fontX;
			fontY = (int)work.fontY;
			size = (int)(fontY*1.2);
			Ix = work.Ix;
			Iy = work.Iy;
			
			DrawClass.PopBox2(g, Ix+(int)(fontX*1.0), Iy+(int)(fontY*21), Ix+fontX*79, Iy+(int)(fontY*26.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.3), Iy+(int)(fontY*21.1), Ix+(int)(fontX*78.8), Iy+(int)(fontY*26.4), work.grayColor);

			y = Iy+(int)(fontY*21.5);
			format.Alignment = StringAlignment.Far;
			if(Tools.IsLangKorean())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*12, fontY, "최대값:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*12, fontY, "최소값:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*12, fontY, "계기최대값:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*12, fontY, "계기최소값:", Color.Black, work.grayColor, work.f, format);
			}
			else if(Tools.IsLangJapanese())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*12, fontY, "最大値:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*12, fontY, "最小値:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*12, fontY, "PLC-最大値:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*12, fontY, "PLC-最小値:", Color.Black, work.grayColor, work.f, format);
			}
			else if(Tools.IsLangChinese())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*12, fontY, "最大值:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*12, fontY, "最小值:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*12, fontY, "PLC-最大值:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*12, fontY, "PLC-最小值:", Color.Black, work.grayColor, work.f, format);
			}
			else 
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*12, fontY, "Max:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*12, fontY, "Min:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*12, fontY, "Eng Max:", Color.Black, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*12, fontY, "Eng Min:", Color.Black, work.grayColor, work.f, format);
			}
			format.Alignment = StringAlignment.Near;
			buf = String.Format("{0}", TagUtil.AiValueToStringOnlyPoint(ai, ai.fFull));
			DrawClass.WinDrawText(g, Ix+fontX*13, y, fontX*14, fontY, buf, Color.Black, work.grayColor, work.f, format);
			buf = String.Format("{0}", TagUtil.AiValueToStringOnlyPoint(ai, ai.fBase));
			DrawClass.WinDrawText(g, Ix+fontX*13, y+size, fontX*14, fontY, buf, Color.Black, work.grayColor, work.f, format);
			buf = String.Format("{0,7:f2}", ai.fPlcFull);
			DrawClass.WinDrawText(g, Ix+fontX*13, y+size*2, fontX*14, fontY, buf, Color.Black, work.grayColor, work.f, format);
			buf = String.Format("{0,7:f2}", ai.fPlcBase);
			DrawClass.WinDrawText(g, Ix+fontX*13, y+size*3, fontX*14, fontY, buf, Color.Black, work.grayColor, work.f, format);
	
			format.Alignment = StringAlignment.Far;
			DrawClass.WinDrawText(g, Ix+fontX*27, y, fontX*10, fontY, "Hi-Hi:", Color.DarkRed, work.grayColor, work.f, format);	
			DrawClass.WinDrawText(g, Ix+fontX*27, y+size, fontX*10, fontY, "High:", Color.Red, work.grayColor, work.f, format);	
			DrawClass.WinDrawText(g, Ix+fontX*27, y+size*2, fontX*10, fontY, "Low:", Color.Blue, work.grayColor, work.f, format);
			DrawClass.WinDrawText(g, Ix+fontX*27, y+size*3, fontX*10, fontY, "Lo-Lo:", Color.DarkBlue, work.grayColor, work.f, format);

			format.Alignment = StringAlignment.Near;
			buf = String.Format("{0}", TagUtil.AiValueToStringOnlyPoint(ai, ai.hihi));
			DrawClass.WinDrawText(g, Ix+fontX*37, y, fontX*15, fontY, buf, Color.DarkRed, work.grayColor, work.f, format);			
			buf = String.Format("{0}", TagUtil.AiValueToStringOnlyPoint(ai, ai.high));			
			DrawClass.WinDrawText(g, Ix+fontX*37, y+size, fontX*15, fontY, buf, Color.Red, work.grayColor, work.f, format);			
			buf = String.Format("{0}", TagUtil.AiValueToStringOnlyPoint(ai, ai.low));
			DrawClass.WinDrawText(g, Ix+fontX*37, y+size*2, fontX*15, fontY, buf, Color.Blue, work.grayColor, work.f, format);						
			buf = String.Format("{0}", TagUtil.AiValueToStringOnlyPoint(ai, ai.lolo));			
			DrawClass.WinDrawText(g, Ix+fontX*37, y+size*3, fontX*15, fontY, buf, Color.DarkBlue, work.grayColor, work.f, format);
	
			format.Alignment = StringAlignment.Far;
			DrawClass.WinDrawText(g, Ix+fontX*52, y, fontX*10, fontY, "HiHI DO:", Color.DarkOrange, work.grayColor, work.f, format);
			DrawClass.WinDrawText(g, Ix+fontX*52, y+size, fontX*10, fontY, "LoLo DO:", Color.DarkOrange, work.grayColor, work.f, format);
			DrawClass.WinDrawText(g, Ix+fontX*52, y+size*2, fontX*10, fontY, "AO Tag:", Color.DarkOrange, work.grayColor, work.f, format);			
			DrawClass.WinDrawText(g, Ix+fontX*52, y+size*3, fontX*10, fontY, "AO SV:", Color.DarkOrange, work.grayColor, work.f, format);
			
			format.Alignment = StringAlignment.Near;
			string			str;

			str = ai.sSubOutDigitalHiHi;
			if(str != null) str.Trim();
			if(str == null || str.Length <= 0) DrawClass.WinDrawText(g, Ix+fontX*62, y, fontX*16, fontY, undifinedBuf, Color.DarkOrange, work.grayColor, work.f, format);
			else							   DrawClass.WinDrawText(g, Ix+fontX*62, y, fontX*16, fontY, ai.sSubOutDigitalHiHi, Color.DarkOrange, work.grayColor, work.f, format);
			str = ai.sSubOutDigitalLoLo;
			if(str != null) str.Trim();
			if(str == null || str.Length <= 0) DrawClass.WinDrawText(g, Ix+fontX*62, y+size, fontX*16, fontY, undifinedBuf, Color.DarkOrange, work.grayColor, work.f, format);
			else							   DrawClass.WinDrawText(g, Ix+fontX*62, y+size, fontX*16, fontY, ai.sSubOutDigitalLoLo, Color.DarkOrange, work.grayColor, work.f, format);
			str = ai.sSubOutAnalog;
			if(str != null) str.Trim();
			if(str == null || str.Length <= 0) DrawClass.WinDrawText(g, Ix+fontX*62, y+size*2, fontX*16, fontY, undifinedBuf, Color.DarkOrange, work.grayColor, work.f, format);
			else							   DrawClass.WinDrawText(g, Ix+fontX*62, y+size*2, fontX*16, fontY, ai.sSubOutAnalog, Color.DarkOrange, work.grayColor, work.f, format);			
			str = ai.sSubOutAnalogSP;
			if(str != null) str.Trim();
			if(str == null || str.Length <= 0) DrawClass.WinDrawText(g, Ix+fontX*62, y+size*3, fontX*16, fontY, undifinedBuf, Color.DarkOrange, work.grayColor, work.f, format);
			else							   DrawClass.WinDrawText(g, Ix+fontX*62, y+size*3, fontX*16, fontY, ai.sSubOutAnalogSP, Color.DarkOrange, work.grayColor, work.f, format);
						
			format.Alignment = StringAlignment.Center;
			y = Iy+(int)(fontY*21.9);
			switch(ai.cTagLinkType)
			{	
				case 0 : 			
					y = Iy+(int)(fontY*20.9);
					DrawClass.PopBox2(g, Ix+(int)(fontX*79.5), y-(int)(fontY*0.5), Ix+(int)(fontX*88.5), y+(int)(fontY*5.5), work.grayColor);
					DrawClass.GrayDrawText(g, Ix+fontX*80, y, fontX*8, fontY, "Port:", work.grayColor, work.f, format);
					buf = String.Format("{0,3:d3}", ai.port);
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+fontY, fontX*8, fontY, buf, work.grayColor, work.f, format);
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+fontY*3, fontX*8, fontY, "Address:", work.grayColor, work.f, format);
					buf = String.Format("{0,4:d4}", ai.address);
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+fontY*4, fontX*8, fontY, buf, work.grayColor, work.f, format);				
					break;
				case 1 :			
					y = Iy+(int)(fontY*16.9);
					DrawClass.PopBox2(g, Ix+(int)(fontX*79.5), y-(int)(fontY*0.5), Ix+(int)(fontX*88.5), y+(int)(fontY*9.5), work.grayColor);
					format.Alignment = StringAlignment.Far;
					DrawClass.GrayDrawText(g, Ix+fontX*80, y, fontX*8, fontY, "Service:", work.grayColor, work.f, format);
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+size*3, fontX*8, fontY, "Topic:", work.grayColor, work.f, format);	
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+size*6, fontX*8, fontY, "Item:", work.grayColor, work.f, format);

					format.Alignment = StringAlignment.Near;
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+size, fontX*8, fontY, ai.sDdeService, work.grayColor, work.f, format);	
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+size*4, fontX*8, fontY, ai.sDdeTopic, work.grayColor, work.f, format);	
					DrawClass.GrayDrawText(g, Ix+fontX*80, y+size*7, fontX*8, fontY, ai.sDdeItem, work.grayColor, work.f, format);	
					break;				
				case 2 :
					DrawClass.PopBox2(g, Ix+(int)(fontX*79.5), y, Ix+(int)(fontX*88.5), y+fontY*3, work.grayColor);

					if(Tools.IsLangKorean())
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "메 모 리", work.grayColor, work.f, format);
					else if(Tools.IsLangJapanese())
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "メモリー", work.grayColor, work.f, format);
					else if(Tools.IsLangChinese())
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "内存", work.grayColor, work.f, format);
					else
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "Memory", work.grayColor, work.f, format);
					break;
				case 3 :			
					DrawClass.PopBox2(g, Ix+(int)(fontX*79.5), y, Ix+(int)(fontX*88.5), y+fontY*3, work.grayColor);

					if(Tools.IsLangKorean())
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "간접태그", work.grayColor, work.f, format);
					else if(Tools.IsLangJapanese())
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "間接タグ", work.grayColor, work.f, format);
					else if(Tools.IsLangChinese())
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "间接标记", work.grayColor, work.f, format);
					else
						DrawClass.GrayDrawText(g, Ix+(int)(fontX*79.8), y+fontY, (int)(fontX*8.5), fontY, "Indirect", work.grayColor, work.f, format);
					break;
			}
		}

		private void AnalogDetailGraphDraw(Graphics g)
		{
			int 						i, Ix, Iy, fontX, fontY;
			String 						buf;
			StringFormat				format = new StringFormat();
			TagAiClass					ai;
			ai = TagLib.GetStructAI(work.tagList[work.pos]);

			Ix = work.Ix;
			Iy = work.Iy;
			fontX = (int)work.fontX;
			fontY = (int)work.fontY;

			DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*20.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), Ix+fontX*19, Iy+(int)(fontY*19), work.grayColor);
			AiUpLinePosDraw(g, fontX, fontY, Ix+fontX*13, Iy+(int)(fontY*4.5), fontY*14, ai.view_full, ai.view_base, ai.sDisplayFormat, ai.fDisplayFormat, ai.cDisplayFormat);
			DrawClass.PushBox2(g, Ix+(int)(fontX*19.9), Iy+(int)(fontY*4.4), Ix+(int)(fontX*76.1), Iy+(int)(fontY*18.6), Color.White);

			buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, Ix+fontX*2,  Iy+(int)(fontY*19.5), fontX*16, fontY, buf, Color.Black, work.grayColor, work.f, format);

			if(Tools.IsLangKorean())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "태그:", ai.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "설명:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "단위:", ai.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "タグ:", ai.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "説明:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "単位:", ai.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "标记:", ai.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "描述:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "单位:", ai.unit, Color.Black, work.grayColor);
			}
			else 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "Tag:", ai.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "Desc:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "Unit:", ai.unit, Color.Black, work.grayColor);
			}			
			format.Alignment = StringAlignment.Far;
			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*10, fontY, "현재값:",  Color.Black, work.f, format);
			else if(Tools.IsLangJapanese())
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*10, fontY, "現在値:",  Color.Black, work.f, format);
			else if(Tools.IsLangChinese())
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*10, fontY, "现在值:",  Color.Black, work.f, format);
			else
				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+fontY+1, fontX*10, fontY, "Value:",  Color.Black, work.f, format);
			DrawClass.PushBox2(g, Ix+(int)(fontX*61.5), Iy+fontY, Ix+(int)(fontX*78.5), Iy+fontY*2, Color.Blue);
			DownLinePosDraw(g, fontX, fontY, Ix+fontX*20, Iy+(int)(fontY*18.8), fontX*56, 24);

			format.Alignment = StringAlignment.Center;
			for(i = 0; i < 7; i++) 
			{
				if(Tools.IsLangKorean())
					buf = String.Format("{0}초", -(work.trend_min*60)/6*i);
				else if(Tools.IsLangJapanese())
					buf = String.Format("{0}秒", -(work.trend_min*60)/6*i);
				else if(Tools.IsLangChinese())
					buf = String.Format("{0}秒", -(work.trend_min*60)/6*i);
				else
					buf = String.Format("{0}", -(work.trend_min*60)/6*i);
				DrawClass.GrayDrawText(g, Ix+fontX*16+i*fontX*28/3, Iy+(int)(fontY*19.5), fontX*8, fontY, buf, work.grayColor, work.f, format);
			}

			if((ai.view_full - ai.view_base) == 0.0 || (ai.fFull - ai.fBase) == 0.0) return;    // avoid divide by zero

			AiDetailGraphStatusLineDraw(g, ai.hihi, SharedData.colorTotal.HIHI, ai);
			AiDetailGraphStatusLineDraw(g, ai.high, SharedData.colorTotal.HIGH, ai);
			AiDetailGraphStatusLineDraw(g, ai.low, SharedData.colorTotal.LOW, ai);
			AiDetailGraphStatusLineDraw(g, ai.lolo, SharedData.colorTotal.LOLO, ai);
		}

		private void AiDetailGraphStatusLineDraw(Graphics g, double val, Color color, TagAiClass ai)
		{
			double		f;
            double full, _base, imsi;
			
			full = (float)TagUtil.GetDisplayValue(ai, ai.view_full);
			_base = (float)TagUtil.GetDisplayValue(ai, ai.view_base);
			imsi = (float)TagUtil.GetDisplayValue(ai, val);

			//full = ai.fFull;
			//_base = ai.fBase;
			//imsi = val;

			if(imsi <= full && imsi >= _base) 
			{
                f = (float)(((TagUtil.GetDisplayValue(ai, val) - ai.view_base) / (ai.view_full - ai.view_base)) * work.fontY * 14);
				//f = (float)(val - _base) / (full-_base) * work.fontY*14;
				DrawClass.gline(g, work.Ix+(int)work.fontX*20, work.Iy+(int)(work.fontY*18.5-f), work.Ix+(int)work.fontX*76+1, work.Iy+(int)(work.fontY*18.5-f), color);
			}
		}

		private void AnalogDetailCurrentValueTrend(Graphics g, TagAiClass ai)
		{
			int 						Ix, Iy, fontX, fontY;
			String 						buf;
			StringFormat				format = new StringFormat();
			
			
			fontX = (int)work.fontX;
			fontY = (int)work.fontY;
			Ix = work.Ix;
			Iy = work.Iy;

			DrawClass.PushBox(g, Ix+fontX*14, Iy+(int)(fontY*4.5)-2, Ix+fontX*17, Iy+(int)(fontY*18.5)+1, work.grayColor);
			AnalogCurrentValueDrawBarUp(g, Ix+fontX*14+3, Ix+fontX*17-3, Iy+(int)(fontY*18.5), fontY*14, ai);

			buf = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);
			DrawClass.gcls(g, Ix+fontX*62, Iy+fontY+1, Ix+fontX*78, Iy+fontY*2-1, Color.Blue);
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, Ix+fontX*62, Iy+fontY+1, fontX*16, fontY, buf, Color.White, Color.Blue, work.f, format);
		}

		private void AnalogDetailTrendDraw(Graphics g, TagAiClass ai)
		{
			int 						x;
			double 						full;
			Pen							pen = new Pen(Color.Black);
			Color						color = Color.Black;
			
			x = work.Ix+(int)work.fontX*20;
			full = ai.view_full - ai.view_base;

			TrendDrawLine(g, ai, x, work.Iy+(int)(work.fontY*18.5), full, ai.view_base, (int)work.fontX*56+1, (int)work.fontY*14, work.trend_hap);
		}

		private void AnalogDetailTrendValueSetting(TagAiClass ai)
		{
			int 						i;
			
			for(i = work.trend_hap-2; i >= 0; i--) trendValue[i+1].val = trendValue[i].val;
			if(work.trend_pos < work.trend_hap) 
			{
				trendValue[work.trend_pos].flag = true;
				work.trend_pos++;
			}
			trendValue[0].val = ai.curr;			
		}

		private void TrendDrawLine(Graphics g, TagAiClass ai, int x1, int startY, double full, double viewBase, int width, int height, int count)
		{
			int 					endY,	i;
			double					xunit, v1, v2, ratio;
			
			endY = startY-height;
			xunit = (double)width/(double)(count-1);   //x pixel per 1 min

			if(full == 0.0) ratio = 0.0F;
			else            ratio = (double)height/(double)full;

			for(i = 1; i < count; i++)
			{
				if(trendValue[i-1].flag == false || trendValue[i].flag == false) continue;

                v1 = startY - (TagUtil.GetDisplayValue(ai, trendValue[i - 1].val) - viewBase) * ratio;
				//v1 = (double)startY - (double)(trendValue[i-1].val-viewBase)*ratio;
				if(v1 > startY)    v1 = (double)startY;
				else if(v1 < endY) v1 = (double)endY;

                v2 = startY - (TagUtil.GetDisplayValue(ai, trendValue[i].val) - viewBase) * ratio;
				//v2 = (double)startY - (double)(trendValue[i].val-viewBase)*ratio;
				if(v2 > startY)    v2 = (double)startY;
				else if(v2 < endY) v2 = (double)endY;

				DrawClass.gline(g, x1+(int)(xunit*(double)(i-1)), (int)v1, x1+(int)(xunit*(double)(i-1)+xunit), (int)v2, Color.Black);
			}
		}

		public void AnalogCurrentValueDrawBarUp(Graphics g, int x1, int x2, int y, int height, TagAiClass ai)
		{
			int 					j;
			double 					full, imsi;
			Color					color = new Color();
			

			full = ai.view_full - ai.view_base;
			if(full == 0.0) return;    // avoid divide by zero
			
			imsi = TagUtil.GetDisplayValue(ai, ai.curr) - ai.view_base;
			//imsi = ai.curr - ai.view_base;
			j = (int)((imsi / full) * (float)height);

			if(ai.curr > ai.hihi) color = SharedData.colorTotal.HIHI;
			else if(ai.curr > ai.high) color = SharedData.colorTotal.HIGH;
			else if(ai.curr > ai.low)  color = SharedData.colorTotal.LOW;
			else    	       		  color = SharedData.colorTotal.LOLO;
			
			if(j <= 1) j = 0;
			else if(j > height) j = height;
			DrawClass.gcls(g, x1, y-j, x2, y, color);
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            DateTime dt = DateTimeServer.Now;
			long						miliSec, hap;
			TagAiClass					ai;
			ai = TagLib.GetStructAI(work.tagList[work.pos]);

			if(work.bTagChanged) 
			{
				AiDetailTrendMemoryAllocation();
				AiDetailSetTitle();
				work.remain_mili_sec = work.trend_width*500;	// 화면을 그릴시간이다.
				work.bTagChanged = false;				
			}
			ai.NeedDataCurr = true;

			miliSec = dt.Second*1000+dt.Millisecond;
			hap = (miliSec >= work.old_mili_sec) ? miliSec-work.old_mili_sec : 60000+miliSec-work.old_mili_sec;
			work.old_mili_sec = miliSec;
			work.remain_mili_sec += hap;
			if(work.remain_mili_sec < work.trend_width*500) return;		
			

			Rectangle		rect;
			double			oldVal = work.iOldValue;

			while(work.remain_mili_sec >= work.trend_width*500)
			{			
				AnalogDetailTrendValueSetting(ai);
				if(ai.curr != work.iOldValue) work.iOldValue = ai.curr;
				work.remain_mili_sec -= work.trend_width*500;
			}
			
			if(ai.curr != oldVal) 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*14, work.Iy+(int)(work.fontY*4.5)-2, work.fontX*3, work.fontY*15+1);
				this.Invalidate(rect);
				rect = new Rectangle(work.Ix+work.fontX*62, work.Iy+work.fontY+1, work.fontX*16, work.fontY);
				this.Invalidate(rect);
			}			
			rect = new Rectangle(work.Ix+(int)work.fontX*20, work.Iy+(int)(work.fontY*4.5), work.fontX*56+1, work.fontY*14+1);
			this.Invalidate(rect);
		}
		
				
		private void ViewAnalogInputDetail_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenuAiDetail.Show(control, pos);
                    return;
                }
            }
		}
		
		public void CallAiDetailSettingDialog()
		{
			int			oldMin, oldPeriod;
			ViewAnalogInputDetailDlgSetting dlg = new ViewAnalogInputDetailDlgSetting();
			TagAiClass					ai;
			
			ai = TagLib.GetStructAI(work.tagList[work.pos]);

			oldMin = work.trend_min;
			oldPeriod = work.trend_width;
			dlg.numericUpDown_total_time.Value = work.trend_min;
			dlg.comboBox_data_read_period.SelectedIndex = (work.trend_width-1);

			dlg.SetValue(ai);
            dlg.StartPosition = FormStartPosition.CenterParent;

			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{
				
				work.trend_min = (int)dlg.numericUpDown_total_time.Value;
				work.trend_width = (int)dlg.comboBox_data_read_period.SelectedIndex+1;

				if(oldMin != work.trend_min || oldPeriod != work.trend_width)
				{
					AiDetailTrendMemoryAllocation();
				}

				dlg.GetValue(ai);

                ai.bTagChangeFlag = true;   // 경보레벨을 바꾸면 경보가 발생할 수 있도록 수정. 2013-6-18

				this.Invalidate();
			}			
		}

		
		private void menuItem_detail_setting_Click(object sender, System.EventArgs e)
		{
			CallAiDetailSettingDialog();		
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewAnalogInputDetail_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			timer1.Enabled = false;
		}

		private void ViewAnalogInputDetail_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
		}

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
		}

		private void menuItem_ai_setting_change_Click(object sender, System.EventArgs e)
		{
			CallAiSettingDialog();
		}

		private void menuItem_TagProperityModify_Click(object sender, System.EventArgs e)
		{
			CallTagPropertyWindows();
		}

		private void menuItem_HandInput_Click(object sender, System.EventArgs e)
		{
			callAiHandInputDialog();
		}



        public void menuItem_trend_hour1_Click(object sender, System.EventArgs e)
        {
            callAiTrendWindow(1);
        }

        public void menuItem_trend_hour8_Click(object sender, System.EventArgs e)
        {
            callAiTrendWindow(8);
        }

        public void menuItem_trend_hour24_Click(object sender, System.EventArgs e)
        {
            callAiTrendWindow(24);
        }

        public void menuItem_trend_hour48_Click(object sender, System.EventArgs e)
        {
            callAiTrendWindow(48);
        }

        public void menuItem_trend_hour72_Click(object sender, System.EventArgs e)
        {
            callAiTrendWindow(72);
        }

        public void menuItem_trend_hour720_Click(object sender, System.EventArgs e)
        {
            callAiTrendWindow(720);
        }

        public void menuItem_data_min_Click(object sender, System.EventArgs e)
        {
            CallAiDataWindow(eDataTime.MIN);
        }

        public void menuItem_data_hour_Click(object sender, System.EventArgs e)
        {
            CallAiDataWindow(eDataTime.HOUR);
        }

        public void menuItem_data_day_Click(object sender, System.EventArgs e)
        {
            CallAiDataWindow(eDataTime.DAY);
        }

        public void menuItem_data_week_Click(object sender, System.EventArgs e)
        {
            CallAiDataWindow(eDataTime.WEEK);
        }

        public void menuItem_data_month_Click(object sender, System.EventArgs e)
        {
            CallAiDataWindow(eDataTime.MONTH);
        }

        private void menuItem_close_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }
		
	}
}
