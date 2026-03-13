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
using System.Threading.Tasks;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewDigitalInputTrend.
	/// </summary>
	public class ViewDigitalInputTrend : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenu contextMenuDiTrend;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.ComponentModel.IContainer components;		
		private System.Windows.Forms.MenuItem menuItem_DiTrend_same_hour1;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_same_hour8;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_same_hour24;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_same_hour48;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_same_hour72;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_same_hour720;

		//TagListStruct[] tagList;
		public int xnum = 77;
		public int ynum = 28;

		TagDiClass			di;
		public ArrayList	array;
		ArrayList			trendTagArr;

		private System.Windows.Forms.MenuItem menuItem_DiData_min;
		private System.Windows.Forms.MenuItem menuItem_DiData_hour;
		private System.Windows.Forms.MenuItem menuItem_DiData_day;
		private System.Windows.Forms.MenuItem menuItem_DiData_week;
		private System.Windows.Forms.MenuItem menuItem_DiData_month;

		public struct DI_TREND_VALUE
		{
			public bool		    bExist;
			public byte			val;
		};

		public struct TREND_DI_TAG
		{
			public bool		flag;
			public string	tag;			
			public Color	color;
			public  DI_TREND_VALUE[] val;
		};
		public TREND_DI_TAG		trend_di_tag;

		public string[]		sButtonBuf;
		private System.Windows.Forms.Timer timer1;
		long				old_save_sec;
		private System.Windows.Forms.MenuItem menuItem_di_trend_setting;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem_di_curr_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;

		BasicScreen.kdymain.ButtonCheck2 hansolButton = new BasicScreen.kdymain.ButtonCheck2();


		public ViewDigitalInputTrend(Form parent, int hour, ArrayList arr, Color backColor)
            
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

			sButtonBuf = new string[4];
			if(Tools.IsLangKorean()) 
			{
				sButtonBuf[0] = "1월1일12시00분~";
				sButtonBuf[1] = "시간변경[F12]";
				sButtonBuf[2] = "자료간격 : 01분";
				sButtonBuf[3] = "1월1일13시00분";
			}
			else if(Tools.IsLangJapanese()) 
			{
				sButtonBuf[0] = "1月1日12時00分~";
				sButtonBuf[1] = "時間変更[F12]";
				sButtonBuf[2] = "データ間隔 : 01分";
				sButtonBuf[3] = "1月1日13時00分";
			}
			else if(Tools.IsLangChinese()) 
			{
				sButtonBuf[0] = "1月1日12时00分~";
				sButtonBuf[1] = "更改时间[F12]";
				sButtonBuf[2] = "资料间隔 : 01分";
				sButtonBuf[3] = "1月1日13时00分";
			}
			else 
			{
				sButtonBuf[0] = "1/1 12:00 ~";
				sButtonBuf[1] = "Time[F12]";
				sButtonBuf[2] = "Period : 01Min";
				sButtonBuf[3] = "1/1 13:00";
			}

			if(arr.Count > 1) 
			{
				xnum = 91;
				ynum = 33;				
			}
			work.backColor = backColor;
			work.trend_hour = hour;
			if(work.trend_hour <= 0) work.trend_hour = 1;
			
			work.eTagType = EnumTagType.DI;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;			

			DetailWindowSetSize(xnum, ynum);
			DiTrendInitValueSetting();
			trendTagArr = arr;
			DiTrendDataValueSetting();
			di = TagLib.GetStructDI(work.tagList[work.pos]);
			DiTrendSetTitle();

            DiTrendRemainDataSave(); // 시작할 때도 남아있는 트랜드를 저장해 준다. 2010-12-21
		}

		public void DiTrendInitValueSetting()
		{			
			work.bDataReadFlag = false;
			work.mainTrendPos = 0;	// 표시할 시작 트랜드 single = 항상 0, multi 0 ~
			work.bGuideLine = false;
			work.bGuideAlarm = true;
			work.bCurrPosLine = true;
			work.Ix = 0;
			work.Iy = 0;
			work.dataSort = eDataSort.AVERAGE;			
			array = new ArrayList();			
			setInitTrendWidth();
			setInitTrendDataHapAndPos();			
			getCurrentSec();
		}

		async Task DiMultiTrendValueRead()
		{
			int 					i;
			
			getCurrentSec();
			for(i = 0; i < work.trend_tag_hap; i++) 
			{
				trend_di_tag = (TREND_DI_TAG)array[i];
				await DataSetToBuf(trend_di_tag);				
			}
			work.bDataReadFlag = true;
		}

		async Task DataSetToBuf(TREND_DI_TAG trend_one)
		{
			DataGate gate = new DataGate();

			DataSet ds = await gate.GetDataDi(trend_one.tag, EnumDataType.MOMENT, EnumDataTime.Minute, work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, work.trend_hap, work.trend_width);

			DataRow row;
			for(int i = 0; i < work.trend_hap; i++) 
			{
				row = ds.Tables[0].Rows[i];
				trend_one.val[i].bExist = (ConvertTool.ToInt16(row[0].ToString()) == 1);
				trend_one.val[i].val = ConvertTool.ToByte(row[1].ToString());
			}
		}

		void getCurrentSec()
		{
            DateTime dt = DateTimeServer.Now;
			old_save_sec = dt.Minute*60 + dt.Second;
		}

		public void DiTrendDataValueSetting()
		{
			int 					i;
			multiTrendTagStruct		multi = new multiTrendTagStruct();
			
			array.Clear();						// 어레이를 클리어, 
			trend_di_tag = new TREND_DI_TAG();
			work.trend_tag_hap = trendTagArr.Count;
			

			for(i = 0; i < work.trend_tag_hap; i++) 
			{
				multi = (multiTrendTagStruct)trendTagArr[i];
				if(i == 0) 
				{
					work.pos = TagLib.GetTagPosOnlyList(work.tagList, multi.tag);
					if(work.pos <= 0) work.pos = 0;
				}
				trend_di_tag.tag = multi.tag;
				trend_di_tag.color = multi.color;
				trend_di_tag.flag = true;
				trend_di_tag.val = new DI_TREND_VALUE[work.trend_hap];
				array.Add(trend_di_tag);
			}
		}

		public void DiMultiTrendDataValueReSetting()
		{
			int 					i, count;
			
			setInitTrendDataHapAndPos();
			work.bDataReadFlag = false;			
			getCurrentSec();
			trend_di_tag = new TREND_DI_TAG();
			count = array.Count;			
			for(i = 0; i < count; i++) 
			{
				trend_di_tag = (TREND_DI_TAG)array[i];
				trend_di_tag.val = new DI_TREND_VALUE[work.trend_hap];
				array[i] = trend_di_tag;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputTrend));
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuDiTrend = new System.Windows.Forms.ContextMenu();
            this.menuItem_DiTrend_same_hour1 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_same_hour8 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_same_hour24 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_same_hour48 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_same_hour72 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_same_hour720 = new System.Windows.Forms.MenuItem();
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
            this.menuItem_di_trend_setting = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // contextMenuDiTrend
            // 
            this.contextMenuDiTrend.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_DiTrend_same_hour1,
            this.menuItem_DiTrend_same_hour8,
            this.menuItem_DiTrend_same_hour24,
            this.menuItem_DiTrend_same_hour48,
            this.menuItem_DiTrend_same_hour72,
            this.menuItem_DiTrend_same_hour720,
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
            this.menuItem_di_trend_setting,
            this.menuItem5,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuDiTrend, "contextMenuDiTrend");
            // 
            // menuItem_DiTrend_same_hour1
            // 
            resources.ApplyResources(this.menuItem_DiTrend_same_hour1, "menuItem_DiTrend_same_hour1");
            this.menuItem_DiTrend_same_hour1.Index = 0;
            this.menuItem_DiTrend_same_hour1.Click += new System.EventHandler(this.menuItem_DiTrend_same_hour1_Click);
            // 
            // menuItem_DiTrend_same_hour8
            // 
            resources.ApplyResources(this.menuItem_DiTrend_same_hour8, "menuItem_DiTrend_same_hour8");
            this.menuItem_DiTrend_same_hour8.Index = 1;
            this.menuItem_DiTrend_same_hour8.Click += new System.EventHandler(this.menuItem_DiTrend_same_hour8_Click);
            // 
            // menuItem_DiTrend_same_hour24
            // 
            resources.ApplyResources(this.menuItem_DiTrend_same_hour24, "menuItem_DiTrend_same_hour24");
            this.menuItem_DiTrend_same_hour24.Index = 2;
            this.menuItem_DiTrend_same_hour24.Click += new System.EventHandler(this.menuItem_DiTrend_same_hour24_Click);
            // 
            // menuItem_DiTrend_same_hour48
            // 
            resources.ApplyResources(this.menuItem_DiTrend_same_hour48, "menuItem_DiTrend_same_hour48");
            this.menuItem_DiTrend_same_hour48.Index = 3;
            this.menuItem_DiTrend_same_hour48.Click += new System.EventHandler(this.menuItem_DiTrend_same_hour48_Click);
            // 
            // menuItem_DiTrend_same_hour72
            // 
            resources.ApplyResources(this.menuItem_DiTrend_same_hour72, "menuItem_DiTrend_same_hour72");
            this.menuItem_DiTrend_same_hour72.Index = 4;
            this.menuItem_DiTrend_same_hour72.Click += new System.EventHandler(this.menuItem_DiTrend_same_hour72_Click);
            // 
            // menuItem_DiTrend_same_hour720
            // 
            resources.ApplyResources(this.menuItem_DiTrend_same_hour720, "menuItem_DiTrend_same_hour720");
            this.menuItem_DiTrend_same_hour720.Index = 5;
            this.menuItem_DiTrend_same_hour720.Click += new System.EventHandler(this.menuItem_DiTrend_same_hour720_Click);
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
            // menuItem_di_trend_setting
            // 
            resources.ApplyResources(this.menuItem_di_trend_setting, "menuItem_di_trend_setting");
            this.menuItem_di_trend_setting.Index = 16;
            this.menuItem_di_trend_setting.Click += new System.EventHandler(this.menuItem_di_trend_setting_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 17;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 18;
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ViewDigitalInputTrend
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
            this.Name = "ViewDigitalInputTrend";
            this.Load += new System.EventHandler(this.ViewDigitalInputTrend_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputTrend_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewDigitalInputTrend_Paint);
            this.Closed += new System.EventHandler(this.ViewDigitalInputTrend_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputTrend_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputTrend_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion

		
		void DiTrendSetTitle()
		{
			ViewDigitalInputTrendMain	form = ViewDigitalInputTrendMain.formThis;

			if(work.trend_tag_hap > 1) 
			{
				if(Tools.IsLangKorean()) form.Text = "디지털 멀티 경향진단";
				else if(Tools.IsLangJapanese()) form.Text = "デジタル MULTI 傾向";
				else if(Tools.IsLangChinese()) form.Text = "诊断数字多倾向";
				else form.Text = "Digital Multi Historical Trend";
				return;
			}			
			
			if(Tools.IsLangKorean()) form.Text = string.Format("디지털 경향진단 - {0}", di.name);
			else if(Tools.IsLangJapanese()) form.Text = string.Format("デジタル傾向 - {0}", di.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("诊断数字倾向 - {0}", di.name);
			else form.Text = string.Format("Digital Historical Trend - {0}", di.name);
		}

		//Bitmap bitmapClient = null;

		private void ViewDigitalInputTrend_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics gScreen = e.Graphics;
			
			DetailWindowSetSize(xnum, ynum);
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.

			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);

			//Graphics g = Graphics.FromImage(bitmapClient);
            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;

			DiTrendFirstScreen(g);
			DiTrendGraphDrawCommon(g);
			ReadDiTrendData(g);
			DiTrendCurrentPosDraw(g, -1);
			DiTrendCurrentValueDraw(g, -1);
			TrendStartEndTimeDraw(g, hansolButton, ref sButtonBuf[0], ref sButtonBuf[3]);

			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		void DiTrendFirstScreen(Graphics g)
		{
			string						buf;
			int 			 			Ix, Iy, fontX, fontY;
			StringFormat				format = new StringFormat();
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.gcls(g, 0, 0, work.width, work.height, SharedData.colorTotal.BACK);

			DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*27.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*6), Ix+fontX*15, Iy+(int)(fontY*24), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*15.9), Iy+(int)(fontY*6.4), Ix+(int)(fontX*76.1), Iy+(int)(fontY*23.5), work.backColor);

			format.Alignment = StringAlignment.Center;
			buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
			DrawClass.WinDrawText(g, Ix+fontX*58,  Iy+fontY+1, fontX*18, fontY, buf, Color.Black, work.grayColor, work.f, format);//DT_CENTER);
			
			if(di.bFileSave != 0) DrawClass.PushBox2(g, Ix+fontX*17, Iy+(int)(fontY*1.3), Ix+fontX*18, Iy+(int)(fontY*1.8), work.grayColor);

			DrawClass.PushBox2(g, Ix+fontX*21, Iy+fontY, Ix+fontX*55, Iy+fontY*2, Color.Blue);

			if(Tools.IsLangKorean())
			{
				if(work.trend_hour == 720) buf = String.Format("디지털 30일 경향진단 자료보기");
				else					   buf = String.Format("디지털 {0}시간 경향진단 자료보기", work.trend_hour);
			}
			else if(Tools.IsLangJapanese())
			{
				if(work.trend_hour == 720) buf = String.Format("デジタル 30日間 傾向データ表示");
				else					   buf = String.Format("デジタル {0}時間 傾向データ表示", work.trend_hour);
			}
			else if(Tools.IsLangChinese())
			{
				if(work.trend_hour == 720) buf = String.Format("查看30天的数字倾向资料");
				else					   buf = String.Format("查看{0}小时的数字倾向资料", work.trend_hour);
			}
			else 
			{
				if(work.trend_hour == 720) buf = String.Format("Digital 30 Day Historical Trend");
				else					   buf = String.Format("Digital {0} Hour Historical Trend", work.trend_hour);
			}
			DrawClass.WinDrawText(g, Ix+fontX*21, Iy+fontY+1, fontX*34, fontY, buf, Color.White, Color.Blue, work.f, format);
			if(Tools.IsLangKorean())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "태그:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*67, "설명:", di.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "タグ:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*67, "説明:", di.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese())
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "标记:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*67, "描述:", di.description, Color.Black, work.grayColor);
			}
			else 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "Tag:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*67, "Desc:", di.description, Color.Black, work.grayColor);
			}
			
			if(work.bCurrPosLine == true) 
			{
				format.Alignment = StringAlignment.Far;
				if(Tools.IsLangKorean())
					DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+(int)(fontY*2.5)+1, fontX*12, fontY, "현재 값 :",  Color.Black, work.f, format);
				else if(Tools.IsLangJapanese())
					DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+(int)(fontY*2.5)+1, fontX*12, fontY, "現在値 :",  Color.Black, work.f, format);
				else if(Tools.IsLangChinese())
					DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+(int)(fontY*2.5)+1, fontX*12, fontY, "现在值 :",  Color.Black, work.f, format);
				else
					DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+(int)(fontY*2.5)+1, fontX*12, fontY, "Value :",  Color.Black, work.f, format);

				DrawClass.PushBox2(g, Ix+(int)(fontX*63.5), Iy+(int)(fontY*2.5), Ix+(int)(fontX*77), Iy+(int)(fontY*3.5), Color.Blue);
			}			
		}

		void DiTrendGraphDrawCommon(Graphics g)
		{
			int 						i, imsi, size;
			int 						Ix, Iy, fontX, fontY;
			string 						buf;
			DateTime					dt;
			StringFormat				format = new StringFormat();
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;
			size = work.fontY*17;

			if(work.bGuideLine) 
			{
				for(i = 1; i < 20; i++) DrawClass.gline(g, Ix+fontX*16, Iy+(int)(fontY*6.5)+i*size/20, Ix+(int)(fontX*76.1), Iy+(int)(fontY*6.5)+i*size/20, Color.DarkGray);
				for(i = 1; i < 24; i++) DrawClass.gline(g, Ix+fontX*16+(int)(i*fontX*2.5), Iy+(int)(fontY*6.4), Ix+fontX*16+(int)((long)i*fontX*2.5), Iy+(int)(fontY*23.6), Color.DarkGray);
			}

			imsi = (work.trend_hour == 1 || work.trend_hour == 720) ? 6 : 4;
			DownLinePosDraw(g, fontX, fontY, Ix+fontX*16, Iy+(int)(fontY*23.8), fontX*60, imsi*4);

			format.Alignment = StringAlignment.Center;
			if(imsi == 4) imsi = 8;
			dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
			for(i = 0; i <= imsi; i++) 
			{
				switch(work.trend_hour) 
				{
					case 1 : 
						if(Tools.IsLangKorean()) buf = String.Format("{0}분", i*10);
						else if(Tools.IsLangJapanese()) buf = String.Format("{0}分", i*10);
						else if(Tools.IsLangChinese()) buf = String.Format("{0}分", i*10);
						else buf = String.Format("{0}", i*10);
						break;
					case 8 : 
						if(Tools.IsLangKorean()) buf = String.Format("{0}시", (work.dt.Hour+i)%24);
						else if(Tools.IsLangJapanese()) buf = String.Format("{0}時", (work.dt.Hour+i)%24);
						else if(Tools.IsLangChinese()) buf = String.Format("{0}时", (work.dt.Hour+i)%24);
						else buf = String.Format("{0}", (work.dt.Hour+i)%24);
						break;
					case 24: 
						if(Tools.IsLangKorean()) buf = String.Format("{0}시", (work.dt.Hour+i*3)%24);
						else if(Tools.IsLangJapanese()) buf = String.Format("{0}時", (work.dt.Hour+i*3)%24);
						else if(Tools.IsLangChinese()) buf = String.Format("{0}时", (work.dt.Hour+i*3)%24);
						else buf = String.Format("{0}", (work.dt.Hour+i*3)%24);
						break;
					case 48: 
						if(Tools.IsLangKorean()) buf = String.Format("{0}시", (work.dt.Hour+i*6)%24);
						else if(Tools.IsLangJapanese()) buf = String.Format("{0}時", (work.dt.Hour+i*6)%24);
						else if(Tools.IsLangChinese()) buf = String.Format("{0}时", (work.dt.Hour+i*6)%24);
						else buf = String.Format("{0}", (work.dt.Hour+i*6)%24);
						break;
					case 72: 
						if(Tools.IsLangKorean()) buf = String.Format("{0}시", (work.dt.Hour+i*9)%24);
						else if(Tools.IsLangJapanese()) buf = String.Format("{0}時", (work.dt.Hour+i*9)%24);
						else if(Tools.IsLangChinese()) buf = String.Format("{0}时", (work.dt.Hour+i*9)%24);
						else buf = String.Format("{0}", (work.dt.Hour+i*9)%24);
						break;
					default:
						if(i > 0) dt = dt.AddDays(5);
						if(Tools.IsLangKorean()) buf = String.Format("{0}일", dt.Day);
						else if(Tools.IsLangJapanese()) buf = String.Format("{0}日", dt.Day);
						else if(Tools.IsLangChinese()) buf = String.Format("{0}日", dt.Day);
						else buf = String.Format("{0}", dt.Day);
						break;
				}
				DrawClass.GrayDrawText(g, Ix+fontX*13+i*fontX*60/imsi, Iy+(int)(fontY*24.4), fontX*6, fontY, buf, work.grayColor, work.f, format);
			}

			DrawClass.PushBox2(g, Ix+fontX*8, Iy+(int)(fontY*6.5), Ix+fontX*10, Iy+(int)(fontY*6.75), work.grayColor);
			DrawClass.PushBox2(g, Ix+fontX*8, Iy+(int)(fontY*23.25), Ix+fontX*10, Iy+(int)(fontY*23.5), work.grayColor);
			DrawClass.PushBox2(g, Ix+fontX*10, Iy+(int)(fontY*6.5), Ix+(int)(fontX*10.5), Iy+(int)(fontY*23.5), work.grayColor);

			if(di.desON.Length > 0) buf = String.Format("{0,5:s5}", di.desON);
			else                    buf = String.Format("ON");
			DrawClass.WinDrawText(g, Ix+fontX, Iy+(int)(fontY*7), fontX*9, fontY, buf, Color.Red, work.grayColor, work.f, format);//DT_CENTER);
			if(di.desOFF.Length > 0) buf = String.Format("{0,5:s5}", di.desOFF);
			else                     buf = String.Format("OFF");
			DrawClass.WinDrawText(g, Ix+fontX, Iy+(int)(fontY*22), fontX*9, fontY, buf, Color.Blue, work.grayColor, work.f, format);//DT_CENTER);

            hansolButton.ButtonCheckDraw2(g, work.Ix + (int)(work.fontX * 24.5), work.Iy + (int)(work.fontY * 25.75), work.Ix + (int)(work.fontX * 38.5), work.Iy + (int)(work.fontY * 27.25), work.f, sButtonBuf[1], StringAlignment.Center);

            if (TotalConfig.eOemType != EnumOemType.SBAS)
            {
                if (Tools.IsLangKorean())
                    sButtonBuf[2] = String.Format("자료간격:{0:00}분", work.trend_width);
                else if (Tools.IsLangJapanese())
                    sButtonBuf[2] = String.Format("データ間隔:{0:00}分", work.trend_width);
                else if (Tools.IsLangChinese())
                    sButtonBuf[2] = String.Format("资料间隔:{0:00}分", work.trend_width);
                else
                    sButtonBuf[2] = String.Format("Period:{0:00}Min", work.trend_width);
                hansolButton.ButtonCheckDraw2(g, work.Ix + (int)(work.fontX * 40.5), work.Iy + (int)(work.fontY * 25.75), work.Ix + (int)(work.fontX * 55.5), work.Iy + (int)(work.fontY * 27.25), work.f, sButtonBuf[2], StringAlignment.Center);
            }
		}

		
		void ReadDiTrendData(Graphics g)
		{
			int 						i;
			Rectangle					rect;

			//if(work.bDataReadFlag == false) 
			//{
			//	Cursor = Cursors.WaitCursor;
			//	DiMultiTrendValueRead();
			//}
			g.ResetClip();
			rect = new Rectangle(work.Ix+work.fontX*16, work.Iy+(int)(work.fontY*6.5), work.fontX*60, work.fontY*17);
			g.IntersectClip(rect);

			trend_di_tag = (TREND_DI_TAG)array[0];
			for(i = 0; i < work.trend_hap; i++) 
			{
				TrendDiLineDrawOne(g, i, work.trend_hap, trend_di_tag.val[i]);
			}			
			if(Cursor != Cursors.Arrow) Cursor = Cursors.Arrow;

			g.ResetClip();
			rect.X = 0;
			rect.Y = 0;
			rect.Width = work.width;
			rect.Height = work.height;
			g.IntersectClip(rect);
		}		

		void TrendDiLineDrawOne(Graphics g, int pos, int count, DI_TREND_VALUE val)
		{
			int 		x, y, height, width;

			if(val.bExist == false) return;

			x = work.Ix+work.fontX*16;
			y = work.Iy+(int)(work.fontY*23.5);
			height = work.fontY*17;
			width = work.fontX*60/count + 1;

			x += (int)((long) work.fontX*60*pos/count);
			if(val.val == 1) DrawClass.gcls(g, x, y-height, x+width, y-height/2, SharedData.colorTotal.ON);
			else		     DrawClass.gcls(g, x, y-height/2, x+width, y, SharedData.colorTotal.OFF);
		}


		
		void DiTrendCurrentPosDraw(Graphics g, int oldPos)
		{
			int					x, width;
			Rectangle			rect;

			if(work.bCurrPosLine == false) return;
			if(work.trend_hap <= 1) return;

			width = (int)(work.fontX*60);			
			if(oldPos >= 0) 
			{
				x = work.Ix+work.fontX*16+oldPos*width/work.trend_hap + width/(work.trend_hap*2);
				rect = new Rectangle(x, work.Iy+(int)(work.fontY*6), 1, work.fontY*18);
				this.Invalidate(rect);
			}

			x = work.Ix+work.fontX*16+work.trend_pos*width/work.trend_hap + width/(work.trend_hap*2);
			DrawClass.gline(g, x, work.Iy+(int)(work.fontY*6), x, work.Iy+(int)(work.fontY*23.6), Color.Black);			
		}


		void DiTrendCurrentValueDraw(Graphics g, int oldPos)
		{
			int  						x, y;
			string 						buf;
			DateTime					dt;
			DI_TREND_VALUE				val;
			StringFormat				format = new StringFormat();
			
			if(work.bCurrPosLine == false) return;

			trend_di_tag = (TREND_DI_TAG)array[0];			
		
			if(oldPos >= 0) 
			{
				x = getCurrentValueDrawPos(oldPos);
				DrawClass.gcls(g, x, work.Iy+(int)(work.fontY*5.3), x+work.fontX*21, work.Iy+(int)(work.fontY*6.3)-1, work.grayColor);
			}
			x = getCurrentValueDrawPos(work.trend_pos);
			DrawClass.gcls(g, x, work.Iy+(int)(work.fontY*5.3), x+work.fontX*21, work.Iy+(int)(work.fontY*6.3)-1, work.grayColor);
			dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
			dt = dt.AddMinutes(work.trend_pos*work.trend_width);

			if(Tools.IsLangKorean())
				buf = String.Format("{0}월{1}일{2,2:d2}시{3,2:d2}분", dt.Month, dt.Day, dt.Hour, dt.Minute);
			else if(Tools.IsLangJapanese())
				buf = String.Format("{0}月{1}日{2,2:d2}時{3,2:d2}分", dt.Month, dt.Day, dt.Hour, dt.Minute);
			else if(Tools.IsLangChinese())
				buf = String.Format("{0}月{1}日{2,2:d2}时{3,2:d2}分", dt.Month, dt.Day, dt.Hour, dt.Minute);
			else
				buf = String.Format("{0}/{1} {2,2:d2}:{3,2:d2}", dt.Month, dt.Day, dt.Hour, dt.Minute);
			format.Alignment = StringAlignment.Center;
			DrawClass.GrayDrawText(g, x, work.Iy+(int)(work.fontY*5.3), work.fontX*21, work.fontY, buf, work.grayColor, work.f, format);
	
			val = trend_di_tag.val[work.trend_pos%work.trend_hap];
			
			y = work.Iy+(int)(work.fontY*2.5);
			DrawClass.gcls(g, work.Ix+(int)(work.fontX*64), y, work.Ix+(int)(work.fontX*76.5), y+work.fontY-1, Color.Blue);

			if(val.bExist == false) 
			{
				if(Tools.IsLangKorean())
					DrawClass.WinDrawText(g, work.Ix+work.fontX*64, y, (int)(work.fontX*12.5), work.fontY, "자료없음", Color.White, Color.Blue, work.f, format);
				else if(Tools.IsLangJapanese())
					DrawClass.WinDrawText(g, work.Ix+work.fontX*64, y, (int)(work.fontX*12.5), work.fontY, "データない", Color.White, Color.Blue, work.f, format);
				else if(Tools.IsLangChinese())
					DrawClass.WinDrawText(g, work.Ix+work.fontX*64, y, (int)(work.fontX*12.5), work.fontY, "没有资料", Color.White, Color.Blue, work.f, format);
				else
					DrawClass.WinDrawText(g, work.Ix+work.fontX*64, y, (int)(work.fontX*12.5), work.fontY, "Not Exist", Color.White, Color.Blue, work.f, format);
			} 
			else if(val.val == 0) 
			{
				if(di.desOFF.Length > 0) buf = String.Format("{0}", di.desOFF);
				else                     buf = String.Format("OFF");
				DrawClass.WinDrawText(g, work.Ix+work.fontX*64, y, (int)(work.fontX*12.5), work.fontY, buf, Color.White, Color.Blue, work.f, format);
			}
			else 
			{
				if(di.desON.Length > 0) buf = String.Format("{0}", di.desON);
				else                    buf = String.Format("ON");
				DrawClass.WinDrawText(g, work.Ix+work.fontX*64, y, (int)(work.fontX*12.5), work.fontY, buf, Color.Red, Color.Blue, work.f, format);//DT_CENTER);
			}
			
		}

		private bool mouseLeftButtonCheck(int x, int y)
		{
			int			i, pos;
			
			if(x < work.Ix+work.fontX*16 || x >= work.Ix+work.fontX*76 || y > work.Iy+(int)(work.fontY*24.5) || y < work.Iy+(int)(work.fontY*5.5)) return false;
			
			i = work.Ix+work.fontX*16;
			pos = (x-i)*work.trend_hap/(work.fontX*60);
			if(pos != work.trend_pos && (pos >= 0 && pos < work.trend_hap)) 
			{
				Graphics g = CreateGraphics();

				i = work.trend_pos;
				work.trend_pos = pos;
				DiTrendCurrentPosDraw(g, i);
				DiTrendCurrentValueDraw(g, i);
			}
			return true;
		}		


		void changeDiDataTime(int hour)
		{
			if(work.trend_hour == hour) return;			

			work.bDataReadFlag = false;
			work.trend_hour = hour;
			setInitTrendWidth();
			DiMultiTrendDataValueReSetting();			
			this.Invalidate();
		}

		private void menuItem_DiTrend_same_hour1_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(1);
		}

		private void menuItem_DiTrend_same_hour8_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(8);		
		}

		private void menuItem_DiTrend_same_hour24_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(24);		
		}

		private void menuItem_DiTrend_same_hour48_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(48);		
		}

		private void menuItem_DiTrend_same_hour72_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(72);		
		}

		private void menuItem_DiTrend_same_hour720_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(720);		
		}

		public void callDiDataMinusFunc()
		{
			int			oldPos;
			Graphics	g = CreateGraphics();

			if(work.trend_pos <= 0) return;
			oldPos = work.trend_pos;
			work.trend_pos--;
			DiTrendCurrentPosDraw(g, oldPos);
			DiTrendCurrentValueDraw(g, oldPos);
		
		}

		public void callDiDataPlusFunc()
		{
			int			oldPos;
			Graphics	g = CreateGraphics();

			if(work.trend_pos >= (work.trend_hap-1)) return;
			oldPos = work.trend_pos;
			work.trend_pos++;
			DiTrendCurrentPosDraw(g, oldPos);
			DiTrendCurrentValueDraw(g, oldPos);		
		}

		public void callDiTrendSettingDialog()
		{
			int				i;
			DateTime		dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);

			ViewDigitalInputTrendDlgSetting dlg = new ViewDigitalInputTrendDlgSetting();

			dlg.checkBox_guide_line.Checked = work.bGuideLine;
			dlg.checkBox_curr_line.Checked = work.bCurrPosLine;			
			dlg.comboBox_data_read_period.SelectedIndex = getTrendWidthPos();
			
			dlg.numericUpDown_year.Value = work.dt.Year;
			dlg.numericUpDown_month.Value = work.dt.Month;
			dlg.numericUpDown_day.Value = work.dt.Day;
			dlg.numericUpDown_hour.Value = work.dt.Hour;
            dlg.StartPosition = FormStartPosition.CenterParent;
						
			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{
				work.bGuideLine = dlg.checkBox_guide_line.Checked;
				work.bCurrPosLine = dlg.checkBox_curr_line.Checked;				

				i = getPosToTrendWidth(dlg.comboBox_data_read_period.SelectedIndex);
				if(work.trend_hour > 72 && i < 10) i = 10;

				work.dt = new DateTime((int)dlg.numericUpDown_year.Value, (int)dlg.numericUpDown_month.Value, (int)dlg.numericUpDown_day.Value, (int)dlg.numericUpDown_hour.Value, 0, 0);
				if(dt != work.dt) work.bDataReadFlag = false;

				if(i != work.trend_width) 
				{
					work.trend_width = i;
					DiMultiTrendDataValueReSetting();
				}
				this.Invalidate();
			}			
		}

		void DiTrendButtonLeftMouseCheck(MouseEventArgs e)
		{
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*1.5), work.Iy+(int)(work.fontY*25.75), work.Ix+work.fontX*20, work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[0], 1)) return;
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*24.5), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*38.5), work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[1], 2)) return;
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*40.5), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*55.5), work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[2], 3)) return;
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*61.5), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*78.5), work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[3], 4)) return;		
		}

		private void ViewDigitalInputTrend_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenuDiTrend.Show(control, pos);
                    return;
                }
            }

			if(e.Button == MouseButtons.Left) 
			{
				if(mouseLeftButtonCheck(e.X, e.Y)) return;
				DiTrendButtonLeftMouseCheck(e);
			}
		}

		void DiTrendRemainDataSave()
		{
			TagDiClass					tag;
			TREND_DI_TAG				iTrend_tag;
			for(int pos, i = 0; i < array.Count; i++) 
			{
				iTrend_tag = (TREND_DI_TAG)array[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, iTrend_tag.tag);
				if(pos < 0) continue;
				tag = TagLib.GetStructDI(work.tagList[pos]);
				if(tag == null) continue;
				SharedViewMain.SaveTrendRemainDI(tag);	// 남아있는 자료저장 자료간격이 2분 이상일 때...
			}
		}


		private void ViewDigitalInputTrend_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{		
			hansolButton.ButtonCheckMove2(this, e);
		}

		private void ViewDigitalInputTrend_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int			id;
			
			id = hansolButton.ButtonCheckUp2(this);
			if(id == -1) return;

			switch(id) 
			{
				case 1 : callDataHourMinusFunc(); return;
				case 2 : callTimeSettingDialog(); return;
				case 3 :
                    if (TotalConfig.eOemType != EnumOemType.SBAS)
                    {
                        callDiTrendSettingDialog();
                    }
                        return;
				case 4 : callDataHourPlusFunc(); return;			
			}
		
		}

		private void menuItem_di_trend_setting_Click(object sender, System.EventArgs e)
		{
			callDiTrendSettingDialog();
		} 

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewDigitalInputTrend_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged += new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
		}

		private void ViewDigitalInputTrend_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged -= new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
		}
		
		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
		}

		private async Task OnMinuteChanged()
		{
            DateTime currTime = DateTimeServer.Now, trendTime;
			
			try 
			{
				currTime = currTime.AddMinutes(-1);			// 저장자료는 1 분전 이므로
				if(currTime.Second > 0) currTime = currTime.AddSeconds(-currTime.Second);// 초 이하는 무시				
				trendTime = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
				if(currTime < trendTime) return;
				trendTime = trendTime.AddMinutes(work.trend_hap * work.trend_width);
				if(currTime.Millisecond > 0) currTime = currTime.AddSeconds(-1);// milli 초이하는 무시, milli 초는 -가 않되므로
				if(currTime > trendTime) return;
			}
			catch {}

			//string[]		tag = new string(array.Count);	// 나중에 추가할 코드
			//readCurrentAllTagName(ref tag);
			DiTrendRemainDataSave();	// 남아있는 자료저장 자료간격이 2분 이상일 때...
			work.bDataReadFlag = false;
			this.Invalidate();
            await Task.CompletedTask;
        }

		void DiOneTrendViewTagChangeSetting()
		{
			if(trendTagArr == null || trendTagArr.Count <= 0) return;

			multiTrendTagStruct		trendTag = (multiTrendTagStruct)trendTagArr[0];
			di = TagLib.GetStructDI(work.tagList[work.pos]);

			trendTag.tag = di.name;
			trendTagArr[0] = trendTag;
			DiTrendDataValueSetting();
			this.Invalidate();
		}

		private async void timer1_Tick(object sender, System.EventArgs e)
		{
			if(work.bTagChanged && work.trend_tag_hap == 1) 
			{
				DiOneTrendViewTagChangeSetting();
				DiTrendSetTitle();
				work.bDataReadFlag = false;						// 데이터를 다시 읽어라
				work.bTagChanged = false;

            }

            if (work.bDataReadFlag == false)  //250924 PSU paint 에서 분리, ReadFlag false 일 때 다시 읽기.
                await RefreshDataAsync();
            /*DateTime	dt = DateTimeServer.Now;
            long		lSec, gap;

            lSec = dt.Minute*60 + dt.Second;
            gap = (lSec >= old_save_sec) ? lSec-old_save_sec : 3600+lSec-old_save_sec;
            if(gap < 120) return;
            old_save_sec = lSec;
						
            work.bDataReadFlag = false;
            this.Invalidate();*/
        }

        private bool _isLoading = false;
		private async Task RefreshDataAsync()
		{
			if (_isLoading) return;
			_isLoading = true;

			try
			{
				await DiMultiTrendValueRead();
				this.Invalidate();  // 준비 끝나면 Paint 다시 호출
			}
			finally
			{
				_isLoading = false;
			}
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

        private void menuItem_DiData_min_Click(object sender, EventArgs e)
        {
            CallDiDataWindow(eDataTime.MIN);
        }

        private void menuItem_DiData_hour_Click(object sender, EventArgs e)
        {
            CallDiDataWindow(eDataTime.HOUR);
        }

        private void menuItem_DiData_day_Click(object sender, EventArgs e)
        {
            CallDiDataWindow(eDataTime.DAY);
        }

        private void menuItem_DiData_week_Click(object sender, EventArgs e)
        {
            CallDiDataWindow(eDataTime.WEEK);
        }

        private void menuItem_DiData_month_Click(object sender, EventArgs e)
        {
            CallDiDataWindow(eDataTime.MONTH);
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }
		


	}
}
