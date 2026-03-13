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
using DialogControl;
using AutoLibLocal;
using System.Drawing.Drawing2D;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewDigitalOutputDetail.
	/// </summary>
	public class ViewDigitalOutputDetail : AnalogDigitalCommonDrawClass
	{
		private System.ComponentModel.IContainer components;

		struct DETAIL_VALUE
		{
			public bool		flag;
			public bool		val;
		};

		DETAIL_VALUE[] trendValue;
		public int			xnum = 80;
		public int			ynum = 28;
		bool				bFlag;		
		public string[]		sButtonBuf;
		
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ContextMenu contextMenuDoDetail;
		private System.Windows.Forms.MenuItem menuItem_DoOutputValueChange;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		BasicScreen.kdymain.ButtonCheck2 hansolButton = new BasicScreen.kdymain.ButtonCheck2();


		public ViewDigitalOutputDetail(Form parent, string tag)
            
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
			if(Tools.IsLangKorean()) 
			{
				sButtonBuf[0] = "출력[F3]";
			}
			else if(Tools.IsLangJapanese()) 
			{
				sButtonBuf[0] = "出力[F3]";
			}
			else if(Tools.IsLangChinese()) 
			{
				sButtonBuf[0] = "输出[F3]";
			}
			else 
			{
				sButtonBuf[0] = "OUT[F3]";
			}
			work.eTagType = EnumTagType.DO;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;
			work.pos = TagLib.GetTagPosOnlyList(work.tagList, tag);
			if(work.pos <= 0) work.pos = 0;

			DoDetailInitValueSetting();
			DetailWindowSetSize(xnum, ynum);
			TagDoClass dout = TagLib.GetStructDO(work.tagList[work.pos]);
			work.iOldValue = (double)dout.curr;
			DoDetailSetTitle();
		}

		private void DoDetailTrendMemoryAllocation()
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


		private void DoDetailInitValueSetting()
		{
			work.spos = 0;
			work.Ix = 0;
			work.Iy = 0;
			work.trend_min = 1;
			work.trend_width = 1;		// 0.5 초			
			DoDetailTrendMemoryAllocation();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalOutputDetail));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuDoDetail = new System.Windows.Forms.ContextMenu();
            this.menuItem_DoOutputValueChange = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuDoDetail
            // 
            this.contextMenuDoDetail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_DoOutputValueChange,
            this.menuItem_TagProperityModify,
            this.menuItem1,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuDoDetail, "contextMenuDoDetail");
            // 
            // menuItem_DoOutputValueChange
            // 
            resources.ApplyResources(this.menuItem_DoOutputValueChange, "menuItem_DoOutputValueChange");
            this.menuItem_DoOutputValueChange.Index = 0;
            this.menuItem_DoOutputValueChange.Click += new System.EventHandler(this.menuItem_DoOutputValueChange_Click);
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // ViewDigitalOutputDetail
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
            this.Name = "ViewDigitalOutputDetail";
            this.Load += new System.EventHandler(this.ViewDigitalOutputDetail_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalOutputDetail_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewDigitalOutputDetail_Paint);
            this.Closed += new System.EventHandler(this.ViewDigitalOutputDetail_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalOutputDetail_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalOutputDetail_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion


		void DoDetailSetTitle()
		{
			ViewDigitalOutputDetailMain	form = ViewDigitalOutputDetailMain.formThis;
			TagDoClass dout = TagLib.GetStructDO(work.tagList[work.pos]);

			if(Tools.IsLangKorean()) form.Text = string.Format("디지털 출력 상세감시- {0}", dout.name);
			else if(Tools.IsLangJapanese()) form.Text = string.Format("デジタル出力の詳細表示- {0}", dout.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("详细查看数字输出- {0}", dout.name);
			else form.Text = string.Format("Digital Output Detail - {0}", dout.name);
		}

		//Bitmap bitmapClient = null;

		private void ViewDigitalOutputDetail_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics	gScreen = e.Graphics;

			DetailWindowSetSize(xnum, ynum);
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.
			TagDoClass dout = TagLib.GetStructDO(work.tagList[work.pos]);

			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);

			//Graphics g = Graphics.FromImage(bitmapClient);
            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;
			
			AdDetailFirstScreen(g);
			DoDetailAllValueDraw(g, dout);
			DoDetailGraphDraw(g, dout);
			DoDetailCurrentValueDraw(g, dout);
			DoDetailTrendDraw(g, dout);

			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		void DoDetailAllValueDraw(Graphics g, TagDoClass dout)
		{
			int 				 		y, Ix, Iy, fontX, fontY, size;
			string 				 		buf;
			StringFormat				format = new StringFormat();

			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;
			size = (int)(fontY*1.2);
			y = Iy+(int)(fontY*21.5);

			DrawClass.PopBox2(g, Ix+(int)(fontX*1.0), Iy+(int)(fontY*21), Ix+fontX*79, Iy+(int)(fontY*26.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.3), Iy+(int)(fontY*21.1), Ix+(int)(fontX*78.8), Iy+(int)(fontY*26.4), work.grayColor);
						
			format.Alignment = StringAlignment.Far;
			if(Tools.IsLangKorean()) 
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "On 설명:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "Off 설명:", Color.Blue, work.grayColor, work.f, format);
			}
			else if(Tools.IsLangJapanese()) 
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "On 説明:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "Off 説明:", Color.Blue, work.grayColor, work.f, format);
			}
			else if(Tools.IsLangChinese()) 
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "On 描述:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "Off 描述:", Color.Blue, work.grayColor, work.f, format);
			}
			else
			{
				DrawClass.WinDrawText(g, Ix+fontX, y, fontX*20, fontY, "On Desc:", Color.Red, work.grayColor, work.f, format);
				DrawClass.WinDrawText(g, Ix+fontX, y+size, fontX*20, fontY, "Off Desc:", Color.Blue, work.grayColor, work.f, format);
			}			
			format.Alignment = StringAlignment.Near;
			DrawClass.WinDrawText(g, Ix+fontX*21, y, fontX*20, fontY, dout.desON, Color.Red, work.grayColor, work.f, format);	
			DrawClass.WinDrawText(g, Ix+fontX*21, y+size, fontX*20, fontY, dout.desOFF, Color.Blue, work.grayColor, work.f, format);

			format.Alignment = StringAlignment.Far;
			switch(dout.cTagLinkType)
			{
				case 0 :			
					DrawClass.WinDrawText(g, Ix+fontX*41, y, fontX*15, fontY, "Port :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*15, fontY, "Station :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size*2, fontX*15, fontY, "Device :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size*3, fontX*15, fontY, "Address :", Color.DarkGreen, work.grayColor, work.f, format);

					format.Alignment = StringAlignment.Near;
					buf = String.Format("{0,3:d3}", dout.port);				
					DrawClass.WinDrawText(g, Ix+fontX*56, y, fontX*22, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					buf = String.Format("{0,3:d3}", dout.station);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size, fontX*22, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size*2, fontX*22, fontY, dout.sExtraAddr, Color.DarkGreen, work.grayColor, work.f, format);
					buf = String.Format("{0, 4:X04}", dout.address);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size*3, fontX*22, fontY, buf, Color.DarkGreen, work.grayColor, work.f, format);				
					break;
				case 1 :				
					size = (int)(work.fontY*1.5);
					DrawClass.WinDrawText(g, Ix+fontX*41, y, fontX*15, fontY, "Service :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*15, fontY, "Topic :", Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*41, y+size*2, fontX*15, fontY, "Item :", Color.DarkGreen, work.grayColor, work.f, format);
			
					format.Alignment = StringAlignment.Near;
					DrawClass.WinDrawText(g, Ix+fontX*56, y, fontX*22, fontY, dout.sDdeService, Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size, fontX*22, fontY, dout.sDdeTopic, Color.DarkGreen, work.grayColor, work.f, format);
					DrawClass.WinDrawText(g, Ix+fontX*56, y+size*2, fontX*22, fontY, dout.sDdeItem, Color.DarkGreen, work.grayColor, work.f, format);			
					break;

				case 3 :
					format.Alignment = StringAlignment.Center;
					if(Tools.IsLangKorean())
						DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*30, fontY, "간   접   태   그", Color.DarkGreen, work.grayColor, work.f, format);
					else if(Tools.IsLangJapanese())
						DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*30, fontY, "間接タグ", Color.DarkGreen, work.grayColor, work.f, format);
					else if(Tools.IsLangChinese())
						DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*30, fontY, "间接标记", Color.DarkGreen, work.grayColor, work.f, format);
					else
						DrawClass.WinDrawText(g, Ix+fontX*41, y+size, fontX*30, fontY, "Indirect Tag", Color.DarkGreen, work.grayColor, work.f, format);
					break;
			}

		}

		void DoDetailGraphDraw(Graphics g, TagDoClass dout)
		{
			int 						i, Ix, Iy, fontX, fontY;
			string						buf;
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
			DrawClass.WinDrawText(g, Ix+fontX*2,  Iy+(int)(fontY*19.5), fontX*16, fontY, buf, Color.Black, work.grayColor, work.f, format);//, DT_CENTER);	

			if(Tools.IsLangKorean())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "태그:", dout.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "설명:", dout.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "タグ:", dout.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "説明:", dout.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "标记:", dout.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "描述:", dout.description, Color.Black, work.grayColor);
			}
			else 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+fontY, fontX*42, "Tag:", dout.tag, Color.White, Color.Blue);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*67, "Desc:", dout.description, Color.Black, work.grayColor);
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
			if(dout.desON.Length > 0) buf = String.Format("{0}", dout.desON);
			else                      buf = String.Format("On");
			DrawClass.WinDrawText(g, Ix+fontX*9, Iy+(int)(fontY*5.5), fontX*7, fontY, buf, Color.Red, work.grayColor, work.f, format);//DT_CENTER);

			if(dout.desOFF.Length > 0) buf = String.Format("{0}", dout.desOFF);
			else                       buf = String.Format("Off");			
			DrawClass.WinDrawText(g, Ix+fontX*9, Iy+(int)(fontY*16.5), fontX*7, fontY, buf, Color.Blue, work.grayColor, work.f, format);
	
			hansolButton.ButtonCheckDraw2(g, Ix+fontX*7, Iy+(int)(fontY*10.5), Ix+(int)(fontX*15.5), Iy+(int)(fontY*13.5), work.f, sButtonBuf[0], StringAlignment.Center);
			DownLinePosDraw(g, fontX, fontY, Ix+fontX*20, Iy+(int)(fontY*18.8), fontX*56, 24);
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

		void DoDetailCurrentValueDraw(Graphics g, TagDoClass dout)
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
			if(dout.curr == 1) 
			{		
				if(dout.desON.Length > 0) buf = String.Format("{0,4:S4}", dout.desON);
				else                      buf = String.Format("ON");

				DrawClass.WinDrawText(g, Ix+fontX*64, Iy+fontY, fontX*12, fontY, buf, Color.Red, Color.Blue, work.f, format);
				br = new SolidBrush(Color.Red);
				g.FillEllipse(br, Ix+fontX*4, Iy+(int)(fontY*5), fontX*4, fontY*2);
			}
			else 
			{
				
				if(dout.desOFF.Length > 0) buf = String.Format("{0,4:S4}", dout.desOFF);
				else                       buf = String.Format("OFF");
				DrawClass.WinDrawText(g, Ix+fontX*64, Iy+fontY, fontX*12, fontY, buf, Color.White, Color.Blue, work.f, format);
				br = new SolidBrush(Color.Blue);
				g.FillEllipse(br, Ix+fontX*4, Iy+(int)(fontY*16), fontX*4, fontY*2);
			}
		}

		void DoDetailTrendDraw(Graphics g, TagDoClass dout)
		{
			int 							x;
			Color							iColor;
			Brush							br;			
			
			x = work.Ix+work.fontX*20;			
			if(dout.curr == 1) 
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

		private void DoDetailTrendValueSetting(TagDoClass dout)
		{
			int 							i;
			
			for(i = work.trend_hap-2; i >= 0; i--) 
			{
				trendValue[i+1].flag = trendValue[i].flag;
				trendValue[i+1].val = trendValue[i].val;
			}
			trendValue[0].flag = true;
			trendValue[0].val = (dout.curr == 1) ? true : false;			
		}

		
		void DigitalTrendDrawLine(Graphics g, int startX, int startY, int endY, int width, int count)
		{
			int 		i, sx, ex, midY;
			int			hap = count-1;
			
			if(count < 2) return;		
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
				DoDetailTrendMemoryAllocation();
				DoDetailSetTitle();
				work.remain_mili_sec = work.trend_width*500;	// 화면을 그릴시간이다.
				work.bTagChanged = false;				
			}

			TagDoClass dout = TagLib.GetStructDO(work.tagList[work.pos]);
			dout.NeedDataCurr = true;

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
				DoDetailTrendValueSetting(dout);
				if(dout.curr != (int)work.iOldValue) work.iOldValue = (double)dout.curr;
				work.remain_mili_sec -= work.trend_width*500;					
			}
			if(dout.curr == 1) 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*4, work.Iy+(int)(work.fontY*5), work.fontX*4, work.fontY*2);
				this.Invalidate(rect);
			}
			else 
			{
				rect = new Rectangle(work.Ix+(int)work.fontX*4, work.Iy+(int)(work.fontY*16), work.fontX*4, work.fontY*2);
				this.Invalidate(rect);				
			}

			if((double)dout.curr != oldVal) 
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

		private void menuItem_DoOutputValueChange_Click(object sender, System.EventArgs e)
		{
			CallDoDigitalOutputDialog();		
		}

		public void CallDoDigitalOutputDialog()
		{
			DialogControl.ControlBoxDigitalOutputGo dialog = new DialogControl.ControlBoxDigitalOutputGo();
			dialog.Go(this, work.tagList[work.pos].tag);
			//DialogControl.ControlBoxDigitalOutput dialog = new DialogControl.ControlBoxDigitalOutput(work.sharedData, work.tagList[work.pos].tag);
			//dialog.ShowDialog();
		}

		void DoDetailButtonLeftMouseCheck(MouseEventArgs e)
		{
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*7, work.Iy+(int)(work.fontY*10.5), work.Ix+(int)(work.fontX*15.5), work.Iy+(int)(work.fontY*13.5), work.f, sButtonBuf[0], 1)) return;
		}

		private void ViewDigitalOutputDetail_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenuDoDetail.Show(control, pos);
                    return;
                }
            }

			if(e.Button == MouseButtons.Left) 
			{
				DoDetailButtonLeftMouseCheck(e);
				return;
			}
		}

		private void ViewDigitalOutputDetail_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{		
			hansolButton.ButtonCheckMove2(this, e);
		}

		private void ViewDigitalOutputDetail_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int			id;

			id = hansolButton.ButtonCheckUp2(this);
			if(id == -1) return;

			switch(id) 
			{
				case 1 : CallDoDigitalOutputDialog(); return;
			}		
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewDigitalOutputDetail_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
		}
		
		private void ViewDigitalOutputDetail_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
		}

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;			
			this.Invalidate();
		}

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
