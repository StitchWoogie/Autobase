using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using AutoLib;
using NetTools;
using System.Data;
using AutoLibLocal;
using System.Drawing.Drawing2D;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogOutputDetail.
	/// </summary>
	public class ViewAnalogOutputDetail : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ContextMenu contextMenuAoDetail;
		private System.Windows.Forms.MenuItem menuItem_aoValueSetting;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		struct DETAIL_VALUE
		{
			public bool		flag;
			public double	val;
		};
		public int xnum = 80;
		public int ynum = 28;

		DETAIL_VALUE[] trendValue;

		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.Label label1;


		public ViewAnalogOutputDetail(Form parent, string tag)
            
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

			work.eTagType = EnumTagType.AO;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;
			work.pos = TagLib.GetTagPosOnlyList(work.tagList, tag);
			if(work.pos <= 0) work.pos = 0;

			AoDetailInitValueSetting();
			DetailWindowSetSize(xnum, ynum);
			TagAoClass ao = TagLib.GetStructAO(work.tagList[work.pos]);
			work.iOldValue = ao.curr;
			AoDetailSetTitle();
		}

		private void AoDetailTrendMemoryAllocation()
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


		private void AoDetailInitValueSetting()
		{
			work.spos = 0;
			work.Ix = 0;
			work.Iy = 0;
			work.trend_min = 1;
			work.trend_width = 1;		// 0.5 초			
			AoDetailTrendMemoryAllocation();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogOutputDetail));
            this.contextMenuAoDetail = new System.Windows.Forms.ContextMenu();
            this.menuItem_aoValueSetting = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // contextMenuAoDetail
            // 
            this.contextMenuAoDetail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_aoValueSetting,
            this.menuItem_TagProperityModify,
            this.menuItem1,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuAoDetail, "contextMenuAoDetail");
            // 
            // menuItem_aoValueSetting
            // 
            resources.ApplyResources(this.menuItem_aoValueSetting, "menuItem_aoValueSetting");
            this.menuItem_aoValueSetting.Index = 0;
            this.menuItem_aoValueSetting.Click += new System.EventHandler(this.menuItem_aoValueSetting_Click);
            // 
            // menuItem_TagProperityModify
            // 
            resources.ApplyResources(this.menuItem_TagProperityModify, "menuItem_TagProperityModify");
            this.menuItem_TagProperityModify.Index = 1;
            this.menuItem_TagProperityModify.Click += new System.EventHandler(this.menuItem_TagProperityModify_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 2;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 3;
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // ViewAnalogOutputDetail
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
            this.Name = "ViewAnalogOutputDetail";
            this.Load += new System.EventHandler(this.ViewAnalogOutputDetail_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewAnalogOutputDetail_Paint);
            this.Closed += new System.EventHandler(this.ViewAnalogOutputDetail_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogOutputDetail_MouseDown);
            this.ResumeLayout(false);

		}
		#endregion


		void AoDetailSetTitle()
		{
			ViewAnalogOutputDetailMain	form = ViewAnalogOutputDetailMain.formThis;
			TagAoClass ao = TagLib.GetStructAO(work.tagList[work.pos]);

			if(Tools.IsLangKorean()) form.Text = string.Format("아날로그 출력 상세- {0}", ao.name);
			else if(Tools.IsLangJapanese()) form.Text = string.Format("アナログ出力の詳細表示- {0}", ao.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("详细查看模拟输出- {0}", ao.name);
			else form.Text = string.Format("Analog Output Detail - {0}", ao.name);
		}

		//Bitmap bitmapClient = null;

		private void ViewAnalogOutputDetail_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics	gScreen = e.Graphics;

			DetailWindowSetSize(xnum, ynum);
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.

			TagAoClass ao = TagLib.GetStructAO(work.tagList[work.pos]);

			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);
			//Graphics g = Graphics.FromImage(bitmapClient);
            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);

			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;
			
			AdDetailFirstScreen(g);	
			AnalogOutDetailAllValueDraw(g, ao);
			AnalogOutDetailGraphDraw(g, ao);
			AnalogOutDetailCurrentValueTrend(g, ao);
			AnalogOutDetailTrendDraw(g, ao);

			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		void AnalogOutDetailAllValueDraw(Graphics g, TagAoClass	ao)
		{
			int 						y, Ix, Iy, fontX, fontY, size;
			string 						buf;
			StringFormat				format = new StringFormat();
			
			size = (int)(work.fontY*1.2);
			Ix = work.Ix;
			Iy = work.Iy+(int)(work.fontY*1.5);
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.PopBox2(g, Ix+(int)(fontX*1.0), Iy+(int)(fontY*19.5), Ix+fontX*79, Iy+fontY*25, work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.3), Iy+(int)(fontY*19.6), Ix+(int)(fontX*78.8), Iy+(int)(fontY*24.9), work.grayColor);
			y = Iy+fontY*20;

			format.Alignment = StringAlignment.Far;
			if(Tools.IsLangKorean())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "최대값 :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "최소값 :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*20, fontY, "PLC-최대값:", Color.DarkBlue, work.grayColor, work.f, format);// DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*20, fontY, "PLC-최소값:", Color.DarkBlue, work.grayColor, work.f, format);
			}
			else if(Tools.IsLangJapanese())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "最大値 :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "最小値 :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*20, fontY, "PLC-最大値:", Color.DarkBlue, work.grayColor, work.f, format);// DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*20, fontY, "PLC-最小値:", Color.DarkBlue, work.grayColor, work.f, format);
			}
			else if(Tools.IsLangChinese())
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "最大值 :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "最小值 :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*20, fontY, "PLC-最大值:", Color.DarkBlue, work.grayColor, work.f, format);// DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*20, fontY, "PLC-最小值:", Color.DarkBlue, work.grayColor, work.f, format);
			}
			else 
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "Max :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);	
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "Min :", Color.Black, work.grayColor, work.f, format);//DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*2, fontX*20, fontY, "Eng-Max:", Color.DarkBlue, work.grayColor, work.f, format);// DT_RIGHT);
				DrawClass.WinDrawText(g, Ix+fontX, y+size*3, fontX*20, fontY, "Eng-Min:", Color.DarkBlue, work.grayColor, work.f, format);
			}
			format.Alignment = StringAlignment.Near;
			buf = String.Format("{0,7:f2}", ao.fFull);
			DrawClass.WinDrawText(g, Ix+fontX*21, y, fontX*20, fontY, buf, Color.Black, work.grayColor, work.f, format);
			buf = String.Format("{0,7:f2}", ao.fBase);
			DrawClass.WinDrawText(g, Ix+fontX*21, y+size, fontX*20, fontY, buf, Color.Black, work.grayColor, work.f, format);
			buf = String.Format("{0,7:f2}", ao.plc_full);
			DrawClass.WinDrawText(g, Ix+fontX*21, y+size*2, fontX*20, fontY, buf, Color.DarkBlue, work.grayColor, work.f, format);
			buf = String.Format("{0,7:f2}", ao.plc_base);
			DrawClass.WinDrawText(g, Ix+fontX*21, y+size*3, fontX*20, fontY, buf, Color.DarkBlue, work.grayColor, work.f, format);
						
			format.Alignment = StringAlignment.Far;
			switch(ao.cTagLinkType)
			{
				case 0 :
					DrawClass.WinDrawText(g, Ix+fontX*41, y, fontX*15, fontY, "Port :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*15, fontY, "Station :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size*2, fontX*15, fontY, "Device :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size*3, fontX*15, fontY, "Address :", Color.DarkGreen, work.grayColor, work.f, format);

					format.Alignment = StringAlignment.Near;
					buf = String.Format("{0,3:d3}", ao.port);				
					DrawClass.WinDrawText(g, Ix+fontX*56, y, fontX*22, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					buf = String.Format("{0,3:d3}", ao.station);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size, fontX*22, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size*2, fontX*22, fontY, ao.sExtraAddr, Color.DarkGreen, work.grayColor, work.f, format);
					buf = String.Format("{0, 4:X04}", ao.address);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size*3, fontX*22, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);				
					break;
				case 1 :				
					size = (int)(work.fontY*1.5);
					DrawClass.WinDrawText(g, Ix+fontX*41, y, fontX*15, fontY, "Service :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*15, fontY, "Topic :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size*2, fontX*15, fontY, "Item :", Color.DarkGreen, work.grayColor, work.f, format);
			
					format.Alignment = StringAlignment.Near;
					DrawClass.WinDrawText(g, Ix+fontX*56, y, fontX*22, fontY, ao.sDdeService, Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size, fontX*22, fontY, ao.sDdeTopic, Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size*2, fontX*22, fontY, ao.sDdeItem, Color.DarkGreen, work.grayColor, work.f, format);			
					break;
				case 3 :
					format.Alignment = StringAlignment.Center;
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*30, fontY, "간   접   태   그", Color.DarkGreen, work.grayColor, work.f, format);
					break;
			}
		}

		void AnalogOutDetailGraphDraw(Graphics g, TagAoClass ao)
		{
			int 					i;
			int						Ix, Iy, fontX, fontY;;
			string 					buf;
			StringFormat			format = new StringFormat();
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*20.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), Ix+fontX*19, Iy+(int)(fontY*19), work.grayColor);
			AiUpLinePosDraw(g, fontX, (int)(fontY), Ix+fontX*13, Iy+(int)(fontY*4.5), fontY*14, ao.fFull, ao.fBase, "", 10.2f, 'F');
			DrawClass.PushBox2(g, Ix+(int)(fontX*19.9), Iy+(int)(fontY*4.4), Ix+(int)(fontX*76.1), Iy+(int)(fontY*18.6), Color.White);

			format.Alignment = StringAlignment.Center;
			buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
			DrawClass.WinDrawText(g, Ix+fontX*2,  Iy+(int)(fontY*19.5), fontX*16, fontY, buf, Color.Black, work.grayColor, work.f, format);//DT_CENTER);

			if(Tools.IsLangKorean())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "태그:", ao.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "설명:", ao.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "단위:", ao.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "タグ:", ao.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "説明:", ao.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "単位:", ao.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "标记:", ao.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "描述:", ao.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "单位:", ao.unit, Color.Black, work.grayColor);
			}
			else
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "Tag:", ao.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*50, "Desc:", ao.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*2.5), fontX*10, "Unit:", ao.unit, Color.Black, work.grayColor);
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
					buf = String.Format("{0}S", -(work.trend_min*60)/6*i);
				DrawClass.GrayDrawText(g, Ix+fontX*17+i*fontX*28/3, Iy+(int)(fontY*19.5), fontX*6, fontY, buf, work.grayColor, work.f, format);
			}
		}

		void AnalogOutCurrentValueDrawBarUp(Graphics g, int x1, int x2, int y, int height, TagAoClass ao)
		{
			int 						j;
			double 						full, imsi;
			
			full = ao.fFull - ao.fBase;
			if(full == 0.0) return;    // avoid divide by zero

			imsi = ao.curr - ao.fBase;
			j = (int)((imsi / full) * height);

			if(j <= 0) j = 0;
			else if(j > height) j = height;
			DrawClass.gcls(g, x1, y-j, x2, y, Color.Blue);
		}

		void AnalogOutDetailCurrentValueTrend(Graphics g, TagAoClass ao)
		{
			string 					buf;
			StringFormat			format = new StringFormat();
			
			DrawClass.PushBox(g, work.Ix+work.fontX*14, work.Iy+(int)(work.fontY*4.5)-2, work.Ix+work.fontX*17, work.Iy+(int)(work.fontY*18.5)+1, work.grayColor);
			AnalogOutCurrentValueDrawBarUp(g, work.Ix+work.fontX*14+3, work.Ix+work.fontX*17-3, work.Iy+(int)(work.fontY*18.5), work.fontY*14, ao);

			//AiValueToStringOnlyPoint(buf, ai, ai->curr);
			buf = String.Format("{0,7:f2}", ao.curr);
			DrawClass.gcls(g, work.Ix+work.fontX*62, work.Iy+work.fontY+1, work.Ix+work.fontX*78, work.Iy+work.fontY*2-1, Color.Blue);
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, work.Ix+work.fontX*62, work.Iy+work.fontY+1, work.fontX*16, work.fontY, buf, Color.White, Color.Blue, work.f, format);						
		}

		private void AnalogOutDetailTrendDraw(Graphics g, TagAoClass ao)
		{
			int 						x;
			double 						full;
			Pen							pen = new Pen(Color.Black);
			Color						color = Color.Black;			
			
			x = work.Ix+(int)work.fontX*20;

			full = ao.fFull - ao.fBase;
			TrendDrawLine(g, x, work.Iy+(int)(work.fontY*18.5), full, ao.fBase, (int)work.fontX*56+1, (int)work.fontY*14, work.trend_hap);			
		}

		private void AnalogOutDetailTrendValueSetting(TagAoClass ao)
		{
			int 						i;
			
			for(i = work.trend_hap-2; i >= 0; i--) trendValue[i+1].val = trendValue[i].val;
			if(work.trend_pos < work.trend_hap) 
			{
				trendValue[work.trend_pos].flag = true;
				work.trend_pos++;
			}
			trendValue[0].val = ao.curr;			
		}

		private void TrendDrawLine(Graphics g, int x1, int startY, double full, double viewBase, int width, int height, int count)
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

				//v1 = (float)startY - (float)(GetDisplayValue(ai, val[i-1].value)-base)*ratio;
				v1 = (double)startY - (double)(trendValue[i-1].val-viewBase)*ratio;
				if(v1 > startY)    v1 = (double)startY;
				else if(v1 < endY) v1 = (double)endY;

				//v2 = (float)startY - (float)(GetDisplayValue(ai, val[i].value)-base)*ratio;
				v2 = (double)startY - (double)(trendValue[i].val-viewBase)*ratio;
				if(v2 > startY)    v2 = (double)startY;
				else if(v2 < endY) v2 = (double)endY;

				DrawClass.gline(g, x1+(int)(xunit*(double)(i-1)), (int)v1, x1+(int)(xunit*(double)(i-1)+xunit), (int)v2, Color.Black);
			}
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            DateTime dt = DateTimeServer.Now;
			long		miliSec, hap;
			TagAoClass	ao = TagLib.GetStructAO(work.tagList[work.pos]);

			if(work.bTagChanged) 
			{
				AoDetailTrendMemoryAllocation();
				AoDetailSetTitle();
				work.remain_mili_sec = work.trend_width*500;	// 화면을 그릴시간이다.
				work.bTagChanged = false;				
			}
			ao.NeedDataCurr = true;

			miliSec = dt.Second*1000+dt.Millisecond;
			hap = (miliSec >= work.old_mili_sec) ? miliSec-work.old_mili_sec : 60000+miliSec-work.old_mili_sec;
			work.old_mili_sec = miliSec;
			work.remain_mili_sec += hap;
			if(work.remain_mili_sec < work.trend_width*500) return;

			Rectangle		rect;
			double			oldVal = work.iOldValue;
			
			while(work.remain_mili_sec >= work.trend_width*500)
			{			
				AnalogOutDetailTrendValueSetting(ao);
				if(ao.curr != work.iOldValue) work.iOldValue = ao.curr;
				work.remain_mili_sec -= work.trend_width*500;					
			}
			if(ao.curr != oldVal) 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*14, work.Iy+(int)(work.fontY*4.5)-2, work.fontX*3, work.fontY*15+1);
				this.Invalidate(rect);
				rect = new Rectangle(work.Ix+work.fontX*62, work.Iy+work.fontY+1, work.fontX*16, work.fontY);
				this.Invalidate(rect);
			}
			rect = new Rectangle(work.Ix+(int)work.fontX*20, work.Iy+(int)(work.fontY*4.5), work.fontX*56+1, work.fontY*14+1);
			this.Invalidate(rect);
		}

		private void ViewAnalogOutputDetail_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenuAoDetail.Show(control, pos);
                    return;
                }
            }
		}
		
		
		private void menuItem_aoValueSetting_Click(object sender, System.EventArgs e)
		{
			CallAnalogOutputDialog();
		} 


		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}
		
		private void ViewAnalogOutputDetail_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			//SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			//SharedViewMain.EventListTagPropertyChanged -= new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
		}

		private void ViewAnalogOutputDetail_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			//SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			//SharedViewMain.EventListTagPropertyChanged -= new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
		}

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;			
			this.Invalidate();
		}

		//void OnColorChanged()
		//{
		//	this.Invalidate();
		//}

		/*void OnTagPropertyChanged(TagPublicClass tp)
		{
			if(tp.enumTagType != EnumTagType.AO) return;
			TagAoClass ao = TagLib.GetStructAO(work.tagList[work.pos].tag, ref work.tagList[work.pos].tag_pos);			
			if(ao == null || ao != (TagAoClass)tp) return;
			//AoTrendDataValueSetting();
			this.Invalidate();
		}*/

		private void menuItem_TagProperityModify_Click(object sender, System.EventArgs e)
		{
			CallTagPropertyWindows();
		}

        private void menuItem3_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }
		

	}
}
