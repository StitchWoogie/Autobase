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
using System.Threading.Tasks;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputTrend.
	/// </summary>
	public class ViewAnalogInputTrend : AnalogDigitalCommonDrawClass
	{
		private System.ComponentModel.IContainer components;
		
		//SharedData sharedData;
		public int xnum = 80;
		public int ynum = 29;
		
		private System.Windows.Forms.ContextMenu contextMenuAiTrend;
		private System.Windows.Forms.MenuItem menuItem_trend_hour1_same;
		private System.Windows.Forms.MenuItem menuItem_trend_hour8_same;
		private System.Windows.Forms.MenuItem menuItem_trend_hour24_same;
		private System.Windows.Forms.MenuItem menuItem_trend_hour48_same;
		private System.Windows.Forms.MenuItem menuItem_trend_hour72_same;
		private System.Windows.Forms.MenuItem menuItem_trend_hour720_same;
		private System.Windows.Forms.MenuItem menuItem_data_min;
		private System.Windows.Forms.MenuItem menuItem_data_hour;
		private System.Windows.Forms.MenuItem menuItem_data_day;
		private System.Windows.Forms.MenuItem menuItem_data_week;
		private System.Windows.Forms.MenuItem menuItem_data_month;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem_close;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.Label label1;		

		TagAiClass					ai;
		public ArrayList			array;

		public string[] sDataBuf; //= { "평균", "최소", "최대", "적산", "전체" };
		public string[] sButtonBuf;// = { "1월1일12시00분~", "시간변경[F12]", "자료간격:01분", "1월1일13시00분" };
		
		public struct AI_TREND_VALUE
		{
			public bool		    bExist;
			public float		sum;
			public float 		average;
			public float 		max;
			public float 		min;
		};

		public struct TREND_TAG
		{
			public bool		flag;
			public string 	tag;
			public double	full;		// view 최대 - view 최소
			public double	view_full;
			public double	view_base;
			public eTrendEdgeType	edge;
			public Color	color;
			public  AI_TREND_VALUE[] val;
			public float	fDisplayFormat;
			public char		cDisplayFormat;
			public string	sDisplayFormat;
		};
		public TREND_TAG	trend_tag;
		private System.Windows.Forms.Timer timer1;

		long				old_save_sec;
		private System.Windows.Forms.MenuItem menuItem_trend_setting_change;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem_ai_setting_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;

		
		BasicScreen.kdymain.ButtonCheck2 hansolButton = new BasicScreen.kdymain.ButtonCheck2();

		ArrayList		trendTagArr;

		
		public ViewAnalogInputTrend(Form parent, int hour, ArrayList arr, Color backColor, int nTagNameDispSize)
            
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
			work.nMultiTrendShowTagSize = nTagNameDispSize;

			sDataBuf = new string[5];
			sButtonBuf = new string[4];
			if(Tools.IsLangKorean())
			{
				sDataBuf[0] = "평균";
				sDataBuf[1] = "최소";
				sDataBuf[2] = "최대";
				sDataBuf[3] = "적산";
				sDataBuf[4] = "전체";
				sButtonBuf[0] = "1월1일12시00분~";
				sButtonBuf[1] = "시간변경[F12]";
				sButtonBuf[2] = "자료간격:01분";
				sButtonBuf[3] = "1월1일13시00분";
			}
			else if(Tools.IsLangJapanese())
			{
				sDataBuf[0] = "平均";
				sDataBuf[1] = "最小";
				sDataBuf[2] = "最大";
                sDataBuf[3] = "累計";
				sDataBuf[4] = "全体";
				sButtonBuf[0] = "1月1日12時00分~";
				sButtonBuf[1] = "時間変更[F12]";
				sButtonBuf[2] = "データ間隔:01分";
				sButtonBuf[3] = "1月1日13時00分"; 
			}
			else if(Tools.IsLangChinese())
			{
				sDataBuf[0] = "平均";
				sDataBuf[1] = "最小";
				sDataBuf[2] = "最大";
				sDataBuf[3] = "累计";
				sDataBuf[4] = "全体";
				sButtonBuf[0] = "1月1日12时00分~";
				sButtonBuf[1] = "更改时间[F12]";
				sButtonBuf[2] = "资料间隔:01分";
				sButtonBuf[3] = "1月1日13时00分"; 
			}
			else 
			{
				sDataBuf[0] = "Average";
				sDataBuf[1] = "Min";
				sDataBuf[2] = "Max";
				sDataBuf[3] = "Sum";
				sDataBuf[4] = "Total";
				sButtonBuf[0] = "1/1 12:00 ~";
				sButtonBuf[1] = "Time[F12]";
				sButtonBuf[2] = "Period:01Min";
				sButtonBuf[3] = "1/1 13:00";
			}


			checkMultiTrendTagNameShowSize();

			if(arr.Count > 1) 
			{
				xnum = 91+work.nMultiTrendShowTagSize-14;				
				ynum = 33;				
			}
			work.backColor = backColor;

			work.trend_hour = hour;
			if(work.trend_hour <= 0) work.trend_hour = 1;

			work.eTagType = EnumTagType.AI;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;			
			DetailWindowSetSize(xnum, ynum);
			AiTrendInitValueSetting();
			trendTagArr = arr;
			AiTrendDataValueSetting();
			AiTrendSetTitle();

            SaveTrendRemainAI(); // 시작할 때도 남아있는 트랜드를 저장해 준다.
		}

		void getCurrentSec()
		{
            DateTime dt = DateTimeServer.Now;
			old_save_sec = dt.Minute*60 + dt.Second;
		}

		public void AiTrendInitValueSetting()
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

		private void checkMultiTrendTagNameShowSize()
		{
			if(work.nMultiTrendShowTagSize < 14) work.nMultiTrendShowTagSize = 14;
			if(work.nMultiTrendShowTagSize > 40) work.nMultiTrendShowTagSize = 40;		
		}

		public void AiTrendDataValueSetting()
		{
			int 					i, pos, count;
			multiTrendTagStruct		multi = new multiTrendTagStruct();
			
			array.Clear();						// 어레이를 클리어, 
			trend_tag = new TREND_TAG();
			//work.trend_tag_hap = trendTagArr.Count;	// 2005-3-15 delete

			for(i = 0, count = 0; i < trendTagArr.Count; i++) 
			{
				multi = (multiTrendTagStruct)trendTagArr[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, multi.tag);
				if(pos < 0) continue;//pos = 0; -1 = 태그가 없거나 아날로그 태그가 아니다 2005-3-15 수정
				if(count == 0) work.pos = pos;
				count++;
				trend_tag.tag = multi.tag;
				trend_tag.color = multi.color;
				trend_tag.edge = multi.edge;
				trend_tag.flag = true;
				ai = TagLib.GetStructAI(work.tagList[pos]);
				trend_tag.full = ai.view_full-ai.view_base;
				trend_tag.view_full = ai.view_full;
				trend_tag.view_base = ai.view_base;
				trend_tag.val = new AI_TREND_VALUE[work.trend_hap];
				trend_tag.sDisplayFormat = ai.sDisplayFormat;
				trend_tag.fDisplayFormat = ai.fDisplayFormat;
				trend_tag.cDisplayFormat = ai.cDisplayFormat;

				array.Add(trend_tag);
			}
			work.trend_tag_hap = array.Count;		// 2005-3-15 add
		}

		async Task AiMultiTrendValueRead()
		{
			int 					i;
			
			getCurrentSec();
			for(i = 0; i < work.trend_tag_hap; i++) 
			{
				trend_tag = (TREND_TAG)array[i];
				await DataSetToBuf(trend_tag);				
			}
			work.bDataReadFlag = true;
		}

		async Task DataSetToBuf(TREND_TAG trend_one)
		{
			DataGate gate = new DataGate();

			DataSet ds = await gate.GetDataAi(trend_one.tag, EnumDataType.AveMinMaxSum, EnumDataTime.Minute, work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, work.trend_hap, work.trend_width);

			DataRow row;
			for(int i = 0; i < work.trend_hap; i++) 
			{
				row = ds.Tables[0].Rows[i];
				trend_one.val[i].bExist = (ConvertTool.ToInt16(row[0].ToString()) == 1);
				trend_one.val[i].max = ConvertTool.ToSingle(row["MAX"].ToString());	
				trend_one.val[i].min = ConvertTool.ToSingle(row["MIN"].ToString());	
				trend_one.val[i].sum = ConvertTool.ToSingle(row["SUM"].ToString());
				trend_one.val[i].average = ConvertTool.ToSingle(row["AVE"].ToString());
			}
		}

		public void AiMultiTrendDataValueReSetting()
		{
			int 					i, count;			
			
			setInitTrendDataHapAndPos();
			getCurrentSec();
			work.bDataReadFlag = false;
			trend_tag = new TREND_TAG();
			count = array.Count;			
			for(i = 0; i < count; i++) 
			{
				trend_tag = (TREND_TAG)array[i];
				trend_tag.val = new AI_TREND_VALUE[work.trend_hap];
				array[i] = trend_tag;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputTrend));
            this.contextMenuAiTrend = new System.Windows.Forms.ContextMenu();
            this.menuItem_trend_hour1_same = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour8_same = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour24_same = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour48_same = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour72_same = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour720_same = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem_data_min = new System.Windows.Forms.MenuItem();
            this.menuItem_data_hour = new System.Windows.Forms.MenuItem();
            this.menuItem_data_day = new System.Windows.Forms.MenuItem();
            this.menuItem_data_week = new System.Windows.Forms.MenuItem();
            this.menuItem_data_month = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_ai_setting_change = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_setting_change = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem_close = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // contextMenuAiTrend
            // 
            this.contextMenuAiTrend.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_trend_hour1_same,
            this.menuItem_trend_hour8_same,
            this.menuItem_trend_hour24_same,
            this.menuItem_trend_hour48_same,
            this.menuItem_trend_hour72_same,
            this.menuItem_trend_hour720_same,
            this.menuItem6,
            this.menuItem_data_min,
            this.menuItem_data_hour,
            this.menuItem_data_day,
            this.menuItem_data_week,
            this.menuItem_data_month,
            this.menuItem1,
            this.menuItem_ai_setting_change,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem_trend_setting_change,
            this.menuItem3,
            this.menuItem_close});
            resources.ApplyResources(this.contextMenuAiTrend, "contextMenuAiTrend");
            // 
            // menuItem_trend_hour1_same
            // 
            resources.ApplyResources(this.menuItem_trend_hour1_same, "menuItem_trend_hour1_same");
            this.menuItem_trend_hour1_same.Index = 0;
            this.menuItem_trend_hour1_same.Click += new System.EventHandler(this.menuItem_trend_hour1_same_Click);
            // 
            // menuItem_trend_hour8_same
            // 
            resources.ApplyResources(this.menuItem_trend_hour8_same, "menuItem_trend_hour8_same");
            this.menuItem_trend_hour8_same.Index = 1;
            this.menuItem_trend_hour8_same.Click += new System.EventHandler(this.menuItem_trend_hour8_same_Click);
            // 
            // menuItem_trend_hour24_same
            // 
            resources.ApplyResources(this.menuItem_trend_hour24_same, "menuItem_trend_hour24_same");
            this.menuItem_trend_hour24_same.Index = 2;
            this.menuItem_trend_hour24_same.Click += new System.EventHandler(this.menuItem_trend_hour24_same_Click);
            // 
            // menuItem_trend_hour48_same
            // 
            resources.ApplyResources(this.menuItem_trend_hour48_same, "menuItem_trend_hour48_same");
            this.menuItem_trend_hour48_same.Index = 3;
            this.menuItem_trend_hour48_same.Click += new System.EventHandler(this.menuItem_trend_hour48_same_Click);
            // 
            // menuItem_trend_hour72_same
            // 
            resources.ApplyResources(this.menuItem_trend_hour72_same, "menuItem_trend_hour72_same");
            this.menuItem_trend_hour72_same.Index = 4;
            this.menuItem_trend_hour72_same.Click += new System.EventHandler(this.menuItem_trend_hour72_same_Click);
            // 
            // menuItem_trend_hour720_same
            // 
            resources.ApplyResources(this.menuItem_trend_hour720_same, "menuItem_trend_hour720_same");
            this.menuItem_trend_hour720_same.Index = 5;
            this.menuItem_trend_hour720_same.Click += new System.EventHandler(this.menuItem_trend_hour720_same_Click);
            // 
            // menuItem6
            // 
            resources.ApplyResources(this.menuItem6, "menuItem6");
            this.menuItem6.Index = 6;
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
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 12;
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
            // menuItem_trend_setting_change
            // 
            resources.ApplyResources(this.menuItem_trend_setting_change, "menuItem_trend_setting_change");
            this.menuItem_trend_setting_change.Index = 16;
            this.menuItem_trend_setting_change.Click += new System.EventHandler(this.menuItem_trend_setting_change_Click);
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 17;
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
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ViewAnalogInputTrend
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
            this.Name = "ViewAnalogInputTrend";
            this.Load += new System.EventHandler(this.ViewAnalogInputTrend_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputTrend_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewAnalogInputTrend_Paint);
            this.Closed += new System.EventHandler(this.ViewAnalogInputTrend_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputTrend_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputTrend_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion
				

		void AiTrendSetTitle()
		{
			ViewAnalogInputTrendMain	form = ViewAnalogInputTrendMain.formThis;

			if(work.trend_tag_hap > 1) 
			{
				if(Tools.IsLangKorean()) form.Text = "아날로그 멀티 경향진단";
				else if(Tools.IsLangJapanese()) form.Text = "アナログ Multi トレンド";
				else if(Tools.IsLangChinese()) form.Text = "诊断模拟多倾向";
				else form.Text = "Analog Multi Historical Trend";
				return;
			}

			if(Tools.IsLangKorean())	    form.Text = string.Format("아날로그 경향진단 - {0}", ai.name);
			else if(Tools.IsLangJapanese()) form.Text = string.Format("アナログ Multi トレンド - {0}", ai.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("诊断模拟倾向 - {0}", ai.name);
			else form.Text = string.Format("Analog Historical Trend - {0}", ai.name);
		}

		private void ViewAnalogInputTrend_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
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
					
			AnalogTrendFirstScreen(g);
			if(array.Count > 1)	AnalogMultiTrendValueDraw(g);
			TrendGraphDrawCommon(g);
			ReadAnalogTrendData(g);
			TrendCurrentPosDraw(g);
			AnalogTrendCurrentValueDraw(g);
			TrendStartEndTimeDraw(g, hansolButton, ref sButtonBuf[0], ref sButtonBuf[3]);

			//gScreen.DrawImageUnscaled(bitmapClient, 0, 0);
			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		bool getTrendTagAndAiClass(int pos)		// 2005-3-16 추가
		{
			int			i;

			if(work.trend_tag_hap <= 0) return false;
			trend_tag = (TREND_TAG)array[pos%work.trend_tag_hap];
			i = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			if(i < 0) return false;
			ai = TagLib.GetStructAI(work.tagList[i]);
			if(ai == null) return false;
			return true;
		}

		int getMainTrendTagPos()				// 2005-3-16 추가
		{
			if(work.trend_tag_hap <= 0) return 0;

			int		pos;
			TREND_TAG iTrend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			pos = TagLib.GetTagPosOnlyList(work.tagList, iTrend_tag.tag);
			if(pos <= 0) pos = 0;
			return pos;
		}

		public void AnalogTrendFirstScreen(Graphics g)
		{
			int							pos, Ix, Iy, fontX, fontY, width;
			String 						buf;
			StringFormat				format = new StringFormat();
						
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			//trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			//if(pos <= 0) pos = 0;
			//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
			if(getTrendTagAndAiClass(work.mainTrendPos) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치

			DrawClass.gcls(g, 0, 0, work.width, work.height, SharedData.colorTotal.BACK);

			if(array.Count <= 1) 
			{ // single trend
				DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*27.5), work.grayColor);
				buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
				format.Alignment = StringAlignment.Center;
				DrawClass.WinDrawText(g, Ix+fontX*58,  Iy+fontY+1, fontX*18, fontY, buf, Color.Black, work.grayColor, work.f, format);
			}
			else // multi trend
			{
				width = work.nMultiTrendShowTagSize-14;
				DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+(int)(fontX*(90.5+width)), Iy+(int)(fontY*32.25), work.grayColor);
			}			
			
			if(ai.bFileSave == 1) DrawClass.PushBox2(g, Ix+fontX*17, Iy+(int)(fontY*1.3), Ix+fontX*18, Iy+(int)(fontY*1.8), work.grayColor);
			DrawClass.PushBox2(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*6), Ix+fontX*15, Iy+(int)(fontY*24), work.grayColor);
			AiUpLinePosDraw(g, fontX, fontY, Ix+fontX*13, Iy+(int)(fontY*6.5), fontY*17, trend_tag.view_full, trend_tag.view_base, trend_tag.sDisplayFormat, trend_tag.fDisplayFormat, trend_tag.cDisplayFormat);
			DrawClass.PushBox2(g, Ix+(int)(fontX*15.9), Iy+(int)(fontY*6.4), Ix+(int)(fontX*76.1), Iy+(int)(fontY*23.5), work.backColor);

			DrawClass.PushBox2(g, Ix+fontX*20, Iy+fontY, Ix+fontX*56, Iy+fontY*2, Color.Blue);
			if(Tools.IsLangKorean())
			{
				if(work.trend_hour > 72) buf = String.Format("아날로그 {0}일 경향진단 {1}자료", work.trend_hour/24, sDataBuf[(int)(work.dataSort)%5]);
				else				     buf = String.Format("아날로그 {0}시간 경향진단 {1}자료", work.trend_hour, sDataBuf[(int)(work.dataSort)%5]);
			}
			else if(Tools.IsLangJapanese())
			{
				if(work.trend_hour > 72) buf = String.Format("アナログ {0}日 傾向 {1}データ", work.trend_hour/24, sDataBuf[(int)(work.dataSort)%5]);
				else				     buf = String.Format("アナログ {0}時間 傾向 {1}データ", work.trend_hour, sDataBuf[(int)(work.dataSort)%5]);
			}
			else if(Tools.IsLangChinese())
			{
				if(work.trend_hour > 72) buf = String.Format("诊断{0}天的模拟倾向 {1}资料", work.trend_hour/24, sDataBuf[(int)(work.dataSort)%5]);
				else				     buf = String.Format("诊断{0}小时的模拟倾向 {1}资料", work.trend_hour, sDataBuf[(int)(work.dataSort)%5]);
			}
			else 
			{
				if(work.trend_hour > 72) buf = String.Format("Analog {0}Day Historical {1} Data Trend", work.trend_hour/24, sDataBuf[(int)(work.dataSort)%5]);
				else				     buf = String.Format("Analog {0}Hour Historical {1} Data Trend", work.trend_hour, sDataBuf[(int)(work.dataSort)%5]);
			}
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, Ix+fontX*20, Iy+fontY+1, fontX*36, fontY, buf, Color.White, Color.Blue, work.f, format);
			if(Tools.IsLangKorean())
			{	
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "태그:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*50, "설명:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*4), fontX*10, "단위:", ai.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese())
			{	
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "タグ:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*50, "説明:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*4), fontX*10, "単位:", ai.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese())
			{	
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "标记:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*50, "描述:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*4), fontX*10, "单位:", ai.unit, Color.Black, work.grayColor);
			}
			else 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*2.5), fontX*42, "Tag:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*50, "Desc:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*4), fontX*10, "Unit:", ai.unit, Color.Black, work.grayColor);
			}

			if(work.bCurrPosLine == true) 
			{
				format.Alignment = StringAlignment.Far;
				if(work.dataSort == eDataSort.ALL_DATA) pos = 0;
				else									pos = (int)work.dataSort;
				if(Tools.IsLangKorean())
					buf = String.Format("{0}값:", sDataBuf[pos]);
				else if(Tools.IsLangJapanese())
					buf = String.Format("{0}値:", sDataBuf[pos]);
				else if(Tools.IsLangChinese())
					buf = String.Format("{0}值:", sDataBuf[pos]);
				else
					buf = String.Format("{0} Data:", sDataBuf[pos]);

				DrawClass.GrayDrawText(g, Ix+fontX*51, Iy+(int)(fontY*2.5)+1, fontX*10, fontY, buf,  Color.Black, work.f, format);
				DrawClass.PushBox2(g, Ix+(int)(fontX*61.5), Iy+(int)(fontY*2.5), Ix+(int)(fontX*78.5), Iy+(int)(fontY*3.5), work.grayColor);
			}
			
		}

		public void TrendGraphDrawCommon(Graphics g)
		{
			int 				i, x, size, imsi;
			int					Ix, Iy, fontX, fontY;
			String				buf;
			DateTime			dt;
			StringFormat		format = new StringFormat();

			x = work.fontX*12;
			size = work.fontY*17;
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			if(work.bGuideLine) 
			{
				for(i = 1; i < 20; i++) DrawClass.gline(g, Ix+fontX*16, Iy+(int)(fontY*6.5)+i*size/20, Ix+(int)(fontX*76.1), Iy+(int)(fontY*6.5)+i*size/20, Color.DarkGray);
				for(i = 1; i < 24; i++)	DrawClass.gline(g, Ix+fontX*16+(int)(i*fontX*2.5), Iy+(int)(fontY*6.4), Ix+fontX*16+(int)(i*fontX*2.5), Iy+(int)(fontY*23.6), Color.DarkGray);
			}

			if(getTrendTagAndAiClass(work.mainTrendPos) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
			//trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];

            if (work.bGuideAlarm) AiTrendGraphStatusLineDraw(g, ai, work.Iy + (int)(work.fontY * 23.5), trend_tag.view_full, trend_tag.view_base, work.fontY * 17.1);
            //if (work.bGuideAlarm) AiTrendGraphStatusLineDraw(g, ai, work.Iy + (int)(work.fontY * 17.1), trend_tag.view_full, trend_tag.view_base, work.fontY * 17.1);

			imsi = (work.trend_hour == 1 || work.trend_hour == 720) ? 6 : 4;
			DownLinePosDraw(g, fontX, fontY, Ix+fontX*16, Iy+(int)(fontY*23.8), fontX*60, imsi*4);

			if(imsi == 4) imsi = 8;
			switch(work.trend_hour)		// add 2005-6-27, 기타 시간(2,.., 5,.., 168 등의 시간) 표시를 위해
			{
				case 1 :
				case 8 :
				case 24 :
				case 48 :
				case 72 :
				case 720 : break;
				default : imsi = 1; break;// 시작과 끝에만 표시한다.
			}


			dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
			for(i = 0; i <= imsi; i++) 
			{
				switch(work.trend_hour) 
				{
					case 1 : 
						if(Tools.IsLangKorean())
							buf = String.Format("{0}분", i*10);
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}分", i*10);
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}分", i*10);
						else
							buf = String.Format("{0}", i*10);
						break;
					case 8 : 
						if(Tools.IsLangKorean())
							buf = String.Format("{0}시", (work.dt.Hour+i)%24);
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}時", (work.dt.Hour+i)%24);
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}时", (work.dt.Hour+i)%24);
						else
							buf = String.Format("{0}", (work.dt.Hour+i)%24);
						break;
					case 24: 
						if(Tools.IsLangKorean())
							buf = String.Format("{0}시", (work.dt.Hour+i*3)%24); 
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}時", (work.dt.Hour+i*3)%24); 
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}时", (work.dt.Hour+i*3)%24); 
						else
							buf = String.Format("{0}", (work.dt.Hour+i*3)%24);
						break;
					case 48: 
						if(Tools.IsLangKorean())
							buf = String.Format("{0}시", (work.dt.Hour+i*6)%24); 
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}時", (work.dt.Hour+i*6)%24); 
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}时", (work.dt.Hour+i*6)%24); 
						else
							buf = String.Format("{0}", (work.dt.Hour+i*6)%24); 
						break;
					case 72: 
						if(Tools.IsLangKorean())
							buf = String.Format("{0}시", (work.dt.Hour+i*9)%24); 
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}時", (work.dt.Hour+i*9)%24); 
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}时", (work.dt.Hour+i*9)%24); 
						else
							buf = String.Format("{0}", (work.dt.Hour+i*9)%24); 
						break;
					case 720 :							// modify default -> 720					
						if(i > 0) dt = dt.AddDays(5);
						if(Tools.IsLangKorean())
							buf = String.Format("{0}일", dt.Day);
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}日", dt.Day);
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}日", dt.Day);
						else
							buf = String.Format("{0}", dt.Day);
						break;
					default:							// add 2005-6-27, 기타 시간(2,.., 5,.., 168 등의 시간) 표시를 위해
						if(i > 0) dt = dt.AddHours(work.trend_hour);
						if(Tools.IsLangKorean()) 
						{
							if(work.trend_hour > 24) buf = String.Format("{0}일", dt.Day);
							else					 buf = String.Format("{0}시", dt.Hour);
						}
						else if(Tools.IsLangJapanese()) 
						{
							if(work.trend_hour > 24) buf = String.Format("{0}日", dt.Day);
							else					 buf = String.Format("{0}時", dt.Hour);
						}
						else if(Tools.IsLangChinese()) 
						{
							if(work.trend_hour > 24) buf = String.Format("{0}日", dt.Day);
							else					 buf = String.Format("{0}时", dt.Hour);
						}
						else 
						{
							if(work.trend_hour > 24) buf = String.Format("{0}", dt.Day);
							else					 buf = String.Format("{0}", dt.Hour);
						}
						break;
				}
				format.Alignment = StringAlignment.Center;
				DrawClass.GrayDrawText(g, Ix+fontX*13+i*fontX*60/imsi, Iy+(int)(fontY*24.4), fontX*6, fontY, buf, work.grayColor, work.f, format);
				
			}			
			hansolButton.ButtonCheckDraw2(g, Ix+(int)(fontX*24.5), Iy+(int)(fontY*25.75), Ix+(int)(fontX*38.5), Iy+(int)(fontY*27.25), work.f, sButtonBuf[1], StringAlignment.Center);

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
                hansolButton.ButtonCheckDraw2(g, Ix + (int)(fontX * 40.5), Iy + (int)(fontY * 25.75), Ix + (int)(fontX * 55.5), Iy + (int)(fontY * 27.25), work.f, sButtonBuf[2], StringAlignment.Center);
            }

			if(array.Count <= 1) return;
			TrendTagElementDraw(g);
		}

		private void TrendTagElementDraw(Graphics g)
		{
			int 						i, count, width;
			string						buf;
			
			count = array.Count;		
			if(count > 12) count = 12;
			width = work.nMultiTrendShowTagSize-2;
			for(i = 0; i < count; i++) 
			{
				if(getTrendTagAndAiClass(work.mainTrendPos+i) == false) continue;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
				//trend_tag = (TREND_TAG)array[(work.mainTrendPos+i)%array.Count];
				//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
				//if(pos <= 0) pos = 0;
				//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
				if(i == 0) 
				{
					DrawClass.PushBox2(g, work.Ix+work.fontX*64, work.Iy+(int)(work.fontY*1.2), work.Ix+(int)(work.fontX*64.8), work.Iy+work.fontY*2, trend_tag.color);
					if(Tools.IsLangKorean())
					{
						if(trend_tag.flag) buf = String.Format("{0} 기준태그[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("기준태그[F7]:{0}", ai.tag);
					}
					else if(Tools.IsLangJapanese())
					{
						if(trend_tag.flag) buf = String.Format("{0} 基準タグ[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("基準タグ[F7]:{0}", ai.tag);
					}
					else if(Tools.IsLangChinese())
					{
						if(trend_tag.flag) buf = String.Format("{0} 标准标记[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("标准标记[F7]:{0}", ai.tag);
					}
					else 
					{
						if(trend_tag.flag) buf = String.Format("{0} Main Tag[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("Main Tag[F7]:{0}", ai.tag);
					}
					hansolButton.ButtonCheckDraw2(g, work.Ix+work.fontX*65, work.Iy+work.fontY, work.Ix+work.fontX*(78+width), work.Iy+(int)(work.fontY*2.2), work.f, buf, StringAlignment.Near);
				}
				else 
				{
					DrawClass.PushBox2(g, work.Ix+work.fontX*77, work.Iy+(int)(work.fontY*(4.2+i*1.7)), work.Ix+(int)(work.fontX*77.8), work.Iy+(int)(work.fontY*(5.3+i*1.7)), trend_tag.color);
					if(trend_tag.flag) buf = String.Format("{0} {1}", "v", ai.tag);
					else			   buf = String.Format("{0}", ai.tag);					
					hansolButton.ButtonCheckDraw2(g, work.Ix+work.fontX*78, work.Iy+(int)(work.fontY*(4+i*1.7)), work.Ix+work.fontX*(78+width), work.Iy+(int)(work.fontY*(5.5+i*1.7)), work.f, buf, StringAlignment.Near);
				}				
			}
		}	
		

		public void ReadAnalogTrendData(Graphics g)
		{
			int 						i, k, count;
			bool						bMode;
			Rectangle					rect;
									
			//if(work.bDataReadFlag == false) 
			//{
			//	Cursor = Cursors.WaitCursor;
			//	AiMultiTrendValueRead();
			//}
			g.ResetClip();
			rect = new Rectangle(work.Ix+work.fontX*16, work.Iy+(int)(work.fontY*6.5), work.fontX*60, work.fontY*17);
			g.IntersectClip(rect);
			
			count = array.Count;
			bMode = (count > 1) ? true : false;
			for(k = 0; k < count; k++) 
			{
				trend_tag = (TREND_TAG)array[k];
				for(i = 0; i < work.trend_hap; i++) 
				{
					if(i != 0 && trend_tag.flag == true)
						TrendAnalogLineDrawOne(g, bMode, i-1, work.trend_hap-1, trend_tag.view_full, trend_tag.view_base, ref trend_tag.val[i-1], ref trend_tag.val[i], trend_tag.color, trend_tag.edge);
				}
			}
			if(Cursor != Cursors.Arrow) Cursor = Cursors.Arrow;
			
			g.ResetClip();
			rect.X = 0;
			rect.Y = 0;
			rect.Width = work.width;
			rect.Height = work.height;
			g.IntersectClip(rect);
		}
		
		public void TrendAnalogLineDrawOne(Graphics g, bool bDrawMode, int pos, int hap, double view_full, double view_base, ref AI_TREND_VALUE val1, ref AI_TREND_VALUE val2, Color color, eTrendEdgeType edge)
		{
			int					width, height, x1, y, y1, y2, flag;
			long				a, b;
			Color				icolor;
			double				ratio;			
			
			if(TotalConfig.eOemType == EnumOemType.UYeG_GS)
                if (val1.bExist == false || val2.bExist == false) return;
            else
                if(val1.bExist == false && val2.bExist == false) return;

			if(hap <= 0) return;

			width = work.fontX*60;
			height = work.fontY*17;
			x1 = work.fontX*16;
			y = (int)(work.fontY*23.5);
			a = (long)width * pos;
			b = a + (long)width;
			if(view_full-view_base == 0.0) return;
			ratio = height/(view_full-view_base);

			if(work.dataSort == eDataSort.MAX || work.dataSort == eDataSort.ALL_DATA)
			{
				flag = 0;
				y1 = y-(int)((GetDisplayValue(val1.max)-view_base)*ratio);
				flag = CheckTrendLineYlimit(ref y, ref y1, height);
				y2 = y-(int)((GetDisplayValue(val2.max)-view_base)*ratio);
				flag += CheckTrendLineYlimit(ref y, ref y2, height);
				icolor = (bDrawMode == false) ? Color.Red : color;
				if(flag < 2) 
				{ // RED
					DrawClass.gline(g, work.Ix+x1+(int)(a/hap), work.Iy+y1, work.Ix+x1+(int)(b/hap), work.Iy+y2, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(a/hap), work.Iy+y1, edge, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(b/hap), work.Iy+y2, edge, icolor);
				}
			}
			if(work.dataSort == eDataSort.MIN || work.dataSort == eDataSort.ALL_DATA)
			{
				flag = 0;
				y1 = y-(int)((GetDisplayValue(val1.min)-view_base)*ratio);
				flag = CheckTrendLineYlimit(ref y, ref y1, height);
				y2 = y-(int)((GetDisplayValue(val2.min)-view_base)*ratio);
				flag += CheckTrendLineYlimit(ref y, ref y2, height);
				icolor = (bDrawMode == false) ? Color.Green : color;
				if(flag < 2) 
				{// GREEN
					DrawClass.gline(g, work.Ix+x1+(int)(a/hap), work.Iy+y1, work.Ix+x1+(int)(b/hap), work.Iy+y2, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(a/hap), work.Iy+y1, edge, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(b/hap), work.Iy+y2, edge, icolor);
				}
			}
			if(work.dataSort == eDataSort.AVERAGE || work.dataSort == eDataSort.ALL_DATA)
			{
				flag = 0;
				y1 = y-(int)((GetDisplayValue(val1.average)-view_base)*ratio);
				flag = CheckTrendLineYlimit(ref y, ref y1, height);
				y2 = y-(int)((GetDisplayValue(val2.average)-view_base)*ratio);
				flag += CheckTrendLineYlimit(ref y, ref y2, height);
				icolor = (bDrawMode == false) ? Color.Blue : color;
				if(flag < 2) // BLUE
				{
					DrawClass.gline(g, work.Ix+x1+(int)(a/hap), work.Iy+y1, work.Ix+x1+(int)(b/hap), work.Iy+y2, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(a/hap), work.Iy+y1, edge, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(b/hap), work.Iy+y2, edge, icolor);
				}
			}
			if(work.dataSort == eDataSort.SUM || work.dataSort == eDataSort.ALL_DATA)
			{
				flag = 0;
				y1 = y-(int)((GetDisplayValue(val1.sum)-view_base)*ratio);
				flag = CheckTrendLineYlimit(ref y, ref y1, height);
				y2 = y-(int)((GetDisplayValue(val2.sum)-view_base)*ratio);
				flag += CheckTrendLineYlimit(ref y, ref y2, height);
				icolor = (bDrawMode == false) ? Color.Black : color;
				if(flag < 2) // BLACK
				{
					DrawClass.gline(g, work.Ix+x1+(int)(a/hap), work.Iy+y1, work.Ix+x1+(int)(b/hap), work.Iy+y2, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(a/hap), work.Iy+y1, edge, icolor);
					DrawTrendEdgePointDisplay(g, work.fontX, work.Ix+x1+(int)(b/hap), work.Iy+y2, edge, icolor);
				}
			}
		}

		private int CheckTrendLineYlimit(ref int y, ref int y1, int height)
		{
			int		retn = 0;

			if(y1 > y) 
			{
				y1 = y;
				retn++;
			}
			if(y1 < y-height) 
			{
				y1 = y-height;
				retn++;
			}
			return retn;
		}		

		public void AnalogTrendCurrentValueDraw(Graphics g)
		{
			int  				i, x, y, count, width;
			float				imsi;
			String 				buf;
			StringFormat		format = new StringFormat();
			DateTime			dt;
			AI_TREND_VALUE		val;
			
			if(work.bCurrPosLine == false) return;

			//trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			//if(pos <= 0) pos = 0;
			//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
			if(getTrendTagAndAiClass(work.mainTrendPos) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치

			x = getCurrentValueDrawPos(work.trend_pos);
			DrawClass.gcls(g, x, work.Iy+(int)(work.fontY*5.3), x+work.fontX*20, work.Iy+(int)(work.fontY*6.3)-1, work.grayColor);
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
	
			y = work.Iy+(int)(work.fontY*2.5);
			val = trend_tag.val[work.trend_pos%work.trend_hap];
			DrawClass.gcls(g, work.Ix+work.fontX*62, y+1, work.Ix+work.fontX*78, y+work.fontY-1, work.grayColor);
			
			if(val.bExist) 
			{
				switch(work.dataSort) 
				{
					case eDataSort.MAX : imsi = val.max; break;
					case eDataSort.MIN : imsi = val.min; break;
					case eDataSort.SUM : imsi = val.sum; break;
					default :			 imsi = val.average; break;
				}

				buf = TagUtil.AiValueToStringOnlyPoint(ai, imsi);
				format.Alignment = StringAlignment.Center;
				DrawClass.WinDrawText(g, work.Ix+work.fontX*62, y+1, work.fontX*16, work.fontY, buf, Color.Blue, work.grayColor, work.f, format);
			}
			else 
			{
				if(Tools.IsLangKorean())
					DrawClass.WinDrawText(g, work.Ix+work.fontX*62, y+1, work.fontX*16, work.fontY, "자료없음", Color.Blue, work.grayColor, work.f, format);
				else if(Tools.IsLangJapanese())
					DrawClass.WinDrawText(g, work.Ix+work.fontX*62, y+1, work.fontX*16, work.fontY, "データない", Color.Blue, work.grayColor, work.f, format);
				else if(Tools.IsLangChinese())
					DrawClass.WinDrawText(g, work.Ix+work.fontX*62, y+1, work.fontX*16, work.fontY, "没有资料", Color.Blue, work.grayColor, work.f, format);
				else
					DrawClass.WinDrawText(g, work.Ix+work.fontX*62, y+1, work.fontX*16, work.fontY, "Not Exist", Color.Blue, work.grayColor, work.f, format);
			}
			if(array.Count <= 1) return;

			width = work.nMultiTrendShowTagSize-14;
			count = array.Count;
			if(count > (width + 75) / (15 + width)) count = (width + 75) / (15 + width);
			width = work.fontX*(15+width);
			//if(count > 5) count = 5;
			//width = work.fontX*15;
			x = work.Ix+(int)(work.fontX*1.25) + work.fontX*13;
			y = work.Iy+(int)(work.fontY*27.75);
			for(i = 0; i < count; i++) 
			{
				//trend_tag = (TREND_TAG)array[(work.mainTrendPos+i)%array.Count];
				//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
				//if(pos <= 0) pos = 0;
				//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
				if(getTrendTagAndAiClass(work.mainTrendPos+i) == false) continue;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
				val = trend_tag.val[work.trend_pos%work.trend_hap];

				DrawClass.gcls(g, x+i*width+(int)(work.fontX*0.5), y+work.fontY*3+1, x+(i+1)*width-(int)(work.fontX*0.5), y+work.fontY*4-1, Color.Blue);
				if(val.bExist)
				{
					switch(work.dataSort) 
					{
						case eDataSort.MAX : imsi = val.max; break;
						case eDataSort.MIN : imsi = val.min; break;
						case eDataSort.SUM : imsi = val.sum; break;
						default :			 imsi = val.average; break;
					}
					buf = TagUtil.AiValueToStringOnlyPoint(ai, imsi);
					if(buf.Length >= 12) buf = String.Format("{0,10:f0}", imsi);
					DrawClass.WinDrawText(g, x+i*width, y+work.fontY*3+1, width-work.fontX, work.fontY, buf, Color.White, Color.Blue, work.f, format);//DT_CENTER);
				}
				else 
				{
					if(Tools.IsLangKorean())
						DrawClass.WinDrawText(g, x+i*width, y+work.fontY*3+1, width-work.fontX, work.fontY, "자료없음", Color.White, Color.Blue, work.f, format);
					else if(Tools.IsLangJapanese())
						DrawClass.WinDrawText(g, x+i*width, y+work.fontY*3+1, width-work.fontX, work.fontY, "データない", Color.White, Color.Blue, work.f, format);
					else if(Tools.IsLangChinese())
						DrawClass.WinDrawText(g, x+i*width, y+work.fontY*3+1, width-work.fontX, work.fontY, "没有资料", Color.White, Color.Blue, work.f, format);
					else
						DrawClass.WinDrawText(g, x+i*width, y+work.fontY*3+1, width-work.fontX, work.fontY, "Not Exist", Color.White, Color.Blue, work.f, format);
				}
			}
		}

		public void AnalogTrendCurrentValueInvalidate(int oldPos)
		{
			int  				i, x, y, count, width;
			Rectangle			rect;
			
			if(work.bCurrPosLine == false || oldPos < 0) return;

			//trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			//if(pos <= 0) pos = 0;
			//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
			if(getTrendTagAndAiClass(work.mainTrendPos) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치

			x = getCurrentValueDrawPos(oldPos);
			rect = new Rectangle(x, work.Iy+(int)(work.fontY*5.3), work.fontX*21, work.fontY);
			this.Invalidate(rect);
			
			x = getCurrentValueDrawPos(work.trend_pos);
			rect = new Rectangle(x, work.Iy+(int)(work.fontY*5.3), work.fontX*21, work.fontY);
			this.Invalidate(rect);
			
			y = work.Iy+(int)(work.fontY*2.5);
			rect = new Rectangle(work.Ix+work.fontX*62, y, work.fontX*16, work.fontY);
			this.Invalidate(rect);
			if(array.Count <= 1) return;

			count = array.Count;
			if(count > 5) count = 5;
			width = work.fontX*15;
			x = work.Ix+(int)(work.fontX*1.25) + work.fontX*13;
			y = work.Iy+(int)(work.fontY*27.75);
			for(i = 0; i < count; i++) 
			{
				rect = new Rectangle(x+i*width, y+work.fontY*3, width, work.fontY);
				this.Invalidate(rect);
				
			}
		}

		public void AnalogMultiTrendValueDraw(Graphics g)
		{
			int								i, y, Ix, Iy, fontX, fontY, width, count;
			String							buf;
			StringFormat					format = new StringFormat();
			
			Ix = work.Ix+(int)(work.fontX*1.25);
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;			

			y = Iy+(int)(fontY*27.75);
			width = work.nMultiTrendShowTagSize-14;
			DrawClass.PopBox2(g, Ix+(int)(fontX*0.5), y-(int)(fontY*0.25), Ix+(int)(fontX*(88.5+width)), y+(int)(fontY*4.25), work.grayColor);
			width = fontX*10;
	
			for(i = 0; i < 3; i++) 			
				DrawClass.PopBox2(g, Ix+(int)(fontX*1.5), y+i*(int)(fontY), Ix+(int)(fontX*1.5)+width, y+(i+1)*fontY, work.grayColor);			
			DrawClass.PopBox2(g, Ix+(int)(fontX*1.5), y+i*(int)(fontY), Ix+(int)(fontX*1.5)+width, y+(i+1)*fontY, Color.Blue);

			format.Alignment = StringAlignment.Center;
			DrawClass.GrayDrawText(g, Ix+(int)(fontX*1.5), y+1, width, fontY, "TAG", work.grayColor, work.f, format);
			DrawClass.GrayDrawText(g, Ix+(int)(fontX*1.5), y+fontY+1, width, fontY, "Full", work.grayColor, work.f, format);
			DrawClass.GrayDrawText(g, Ix+(int)(fontX*1.5), y+fontY*2+1, width, fontY, "Base", work.grayColor, work.f, format);	
			if(Tools.IsLangKorean())
				DrawClass.WinDrawText(g, Ix+(int)(fontX*1.5), y+fontY*3+1, width, fontY, "현재값", Color.White, Color.Blue, work.f, format);
			else if(Tools.IsLangJapanese())
				DrawClass.WinDrawText(g, Ix+(int)(fontX*1.5), y+fontY*3+1, width, fontY, "現在値", Color.White, Color.Blue, work.f, format);
			else if(Tools.IsLangChinese())
				DrawClass.WinDrawText(g, Ix+(int)(fontX*1.5), y+fontY*3+1, width, fontY, "现在值", Color.White, Color.Blue, work.f, format);
			else
				DrawClass.WinDrawText(g, Ix+(int)(fontX*1.5), y+fontY*3+1, width, fontY, "Value", Color.White, Color.Blue, work.f, format);
			Ix += fontX*13;
			width = work.nMultiTrendShowTagSize-14;
			count = array.Count;
			if(count > (width + 75) / (15 + width)) count = (width + 75) / (15 + width);
			width = fontX*(15+width);

			for(i = 0; i < count; i++) 
			{
				//trend_tag = (TREND_TAG)array[(work.mainTrendPos+i)%array.Count];
				//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
				//if(pos <= 0) pos = 0;
				//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
				if(getTrendTagAndAiClass(work.mainTrendPos+i) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치

				format.Alignment = StringAlignment.Near;
				DrawClass.PushBox2(g, Ix+i*width, y, Ix+(i+1)*width-(int)(fontX*0.5), y+fontY, work.grayColor);
				DrawClass.GrayDrawText(g, Ix+i*width, y+1, width-(int)(fontX), fontY, ai.tag, work.grayColor, work.f, format);

				format.Alignment = StringAlignment.Center;
				DrawClass.PushBox2(g, Ix+i*width, y+fontY, Ix+(i+1)*width-(int)(fontX*0.5), y+fontY*2, work.grayColor);
				buf = String.Format("{0, 7:f1}", trend_tag.view_full);
				DrawClass.GrayDrawText(g, Ix+i*width, y+fontY+1, width-(int)(fontX), fontY, buf, work.grayColor, work.f, format);
				DrawClass.PushBox2(g, Ix+i*width, y+fontY*2, Ix+(i+1)*width-(int)(fontX*0.5), y+fontY*3, work.grayColor);
				buf = String.Format("{0, 7:f1}", trend_tag.view_base);//ai->file.base);
				DrawClass.GrayDrawText(g, Ix+i*width, y+fontY*2+1, width-(int)(fontX), fontY, buf, work.grayColor, work.f, format);
				DrawClass.PushBox2(g, Ix+i*width, y+fontY*3, Ix+(i+1)*width-(int)(fontX*0.5), y+fontY*4, Color.Blue);
			}
		}	

		public void changeDataTime(int hour)
		{
			if(work.trend_hour == hour) return;			

			work.bDataReadFlag = false;
			work.trend_hour = hour;
			setInitTrendWidth();
			AiMultiTrendDataValueReSetting();			
			this.Invalidate();
		}

		public void menuItem_trend_hour1_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(1);
		}

		public void menuItem_trend_hour8_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(8);		
		}

		public void menuItem_trend_hour24_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(24);		
		}

		public void menuItem_trend_hour48_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(48);		
		}

		public void menuItem_trend_hour72_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(72);		
		}

		public void menuItem_trend_hour720_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(720);
		}

        

		public void callSettingDialog()
		{
			int				i;
			DateTime		dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
			
			ViewAnalogInputTrendDialogSettings dlg = new ViewAnalogInputTrendDialogSettings(this);
			trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			
			dlg.comboBox_dot_type.SelectedIndex = (int)trend_tag.edge;
			dlg.checkBox_alarm_line.Checked = work.bGuideAlarm;
			dlg.checkBox_guide_line.Checked = work.bGuideLine;
			dlg.checkBox_curr_line.Checked = work.bCurrPosLine;			
			dlg.comboBox_data_read_period.SelectedIndex = getTrendWidthPos();
			try 
			{
				dlg.numericUpDown_view_full.Value = (Decimal)trend_tag.view_full;
				dlg.numericUpDown_view_base.Value = (Decimal)trend_tag.view_base;
			}
			catch
			{
			}

			dlg.numericUpDown_year.Value = work.dt.Year;
			dlg.numericUpDown_month.Value = work.dt.Month;
			dlg.numericUpDown_day.Value = work.dt.Day;
			dlg.numericUpDown_hour.Value = work.dt.Hour;

            dlg.Set(bUseSamePeriod);

            dlg.StartPosition = FormStartPosition.CenterParent;
						
			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{
                dlg.Get(out bUseSamePeriod);

				work.bGuideAlarm = dlg.checkBox_alarm_line.Checked;
				work.bGuideLine = dlg.checkBox_guide_line.Checked;
				work.bCurrPosLine = dlg.checkBox_curr_line.Checked;

				trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
				trend_tag.edge = (eTrendEdgeType)dlg.comboBox_dot_type.SelectedIndex;
				trend_tag.view_full = (double)dlg.numericUpDown_view_full.Value;
				trend_tag.view_base = (double)dlg.numericUpDown_view_base.Value;
				trend_tag.full = trend_tag.view_full-trend_tag.view_base;
				array[work.mainTrendPos%work.trend_tag_hap] = trend_tag;
				i = getPosToTrendWidth(dlg.comboBox_data_read_period.SelectedIndex);

                if (!bUseSamePeriod)
                {
                    if (work.trend_hour > 72 && i < 10) i = 10;
                }
                else
                {
                    nUseSamePeriod = i;
                }

				work.dt = new DateTime((int)dlg.numericUpDown_year.Value, (int)dlg.numericUpDown_month.Value, (int)dlg.numericUpDown_day.Value, (int)dlg.numericUpDown_hour.Value, 0, 0);
				if(dt != work.dt) work.bDataReadFlag = false;
				if(i != work.trend_width) 
				{
					work.trend_width = i;
					AiMultiTrendDataValueReSetting();
				}

				this.Invalidate();
			}			
		}

		
		public void callDataMinusFunc()
		{
			int			oldPos;
			
			if(work.trend_pos <= 0) return;
			oldPos = work.trend_pos;
			work.trend_pos--;
			TrendCurrentPosInvalidate(oldPos);
			AnalogTrendCurrentValueInvalidate(oldPos);		
		}

		public void callDataPlusFunc()
		{
			int			oldPos;
			
			if(work.trend_pos >= (work.trend_hap-1)) return;
			oldPos = work.trend_pos;
			work.trend_pos++;
			TrendCurrentPosInvalidate(oldPos);
			AnalogTrendCurrentValueInvalidate(oldPos);		
		}
	
		public String callShowDataSortFunc()
		{
			String			buf;
			int				pos;
			
			if(work.dataSort == eDataSort.ALL_DATA) work.dataSort = eDataSort.AVERAGE;
			else work.dataSort++;
			pos = (int)work.dataSort;			
			buf = String.Format("{0}[F2]", sDataBuf[pos%5]);
			this.Invalidate();
			return buf;
		}

		public bool mouseLeftButtonCheck(int x, int y)
		{
			int			i, pos, width;
			
			if(x < work.Ix+(int)(work.fontX*15.5) || x >= work.Ix+(int)(work.fontX*76.5) || y > work.Iy+(int)(work.fontY*24.5) || y < work.Iy+(int)(work.fontY*5.5)) return false;
			
			width = work.fontX*60;
			i = work.Ix+(int)(work.fontX*16);
			if(x <= work.Ix+work.fontX*16) pos = 0;
			else if(x >= work.Ix+work.fontX*76) pos = work.trend_hap-1;
			else pos = (x-i)*(work.trend_hap)/width;
			if(pos != work.trend_pos && (pos >= 0 && pos < work.trend_hap)) 
			{				
				i = work.trend_pos;
				work.trend_pos = pos;
				TrendCurrentPosInvalidate(i);
				AnalogTrendCurrentValueInvalidate(i);
			}
			return true;
		}

		public void AiMultiTrendMainTagChange()
		{
			work.mainTrendPos++;
			work.mainTrendPos %= work.trend_tag_hap; 
			this.Invalidate();
			return;
		}

		void AiMultiTrendTagEnable(int id)
		{
			int 				pos, count;
			
			count = array.Count;
			if(count <= 1) return;
			if(count > 12) count = 12;
			pos = id-10;
			if(pos <= 0 || pos > count) return;
			trend_tag = (TREND_TAG)array[(work.mainTrendPos+pos)%array.Count];
			trend_tag.flag = (trend_tag.flag) ? false : true;
			array[(work.mainTrendPos+pos)%array.Count] = trend_tag;
			this.Invalidate();
		}
		
		void AiTrendButtonLeftMouseCheck(MouseEventArgs e)
		{
			int 				i, count, width;
			string				buf;

			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*1.5), work.Iy+(int)(work.fontY*25.75), work.Ix+work.fontX*20, work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[0], 1)) return;
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*24.5), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*38.5), work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[1], 2)) return;
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*40.5), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*55.5), work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[2], 3)) return;
			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+(int)(work.fontX*60.0), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*78.5), work.Iy+(int)(work.fontY*27.25), work.f, sButtonBuf[3], 4)) return;

			count = array.Count;
			if(count <= 1) return;

			width = work.nMultiTrendShowTagSize-2;
			if(count > 12) count = 12;
			for(i = 0; i < count; i++) 
			{
				//trend_tag = (TREND_TAG)array[(work.mainTrendPos+i)%array.Count];
				//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
				//if(pos <= 0) pos = 0;
				//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
				if(getTrendTagAndAiClass(work.mainTrendPos+i) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
				if(i == 0) 
				{
					if(Tools.IsLangKorean())
					{
						if(trend_tag.flag) buf = String.Format("{0} 기준태그[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("기준태그[F7]:{0}", ai.tag);
					}
					else if(Tools.IsLangJapanese())
					{
						if(trend_tag.flag) buf = String.Format("{0} 基準タグ[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("基準タグ[F7]:{0}", ai.tag);
					}
					else if(Tools.IsLangChinese())
					{
						if(trend_tag.flag) buf = String.Format("{0} 标准标记[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("标准标记[F7]:{0}", ai.tag);
					}
					else 
					{
						if(trend_tag.flag) buf = String.Format("{0} Main Tag[F7]:{1}", "v", ai.tag);
						else			   buf = String.Format("Main Tag[F7]:{0}", ai.tag);
					}
					if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*65, work.Iy+work.fontY, work.Ix+work.fontX*(78+width), work.Iy+(int)(work.fontY*2.2), work.f, buf, 10+i)) return;
				}
				else 
				{
					if(trend_tag.flag) buf = String.Format("{0} {1}", "v", ai.tag);
					else			   buf = String.Format("{0}", ai.tag);					
					if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*78, work.Iy+(int)(work.fontY*(4+i*1.7)), work.Ix+work.fontX*(78+width), work.Iy+(int)(work.fontY*(5.5+i*1.7)), work.f, buf, 10+i)) return;
				}				
			}			
		}

		private void ViewAnalogInputTrend_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenuAiTrend.Show(control, pos);
                    return;
                }
            }

			if(e.Button == MouseButtons.Left) 
			{				
				if(mouseLeftButtonCheck(e.X, e.Y)) return;
				AiTrendButtonLeftMouseCheck(e);
			}
		}

		private void ViewAnalogInputTrend_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hansolButton.ButtonCheckMove2(this, e);
		}

		private void ViewAnalogInputTrend_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
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
                        callSettingDialog();
                    }
                        return;
				case 4 : callDataHourPlusFunc(); return;
				case 10 : AiMultiTrendMainTagChange(); return;					
				default : 
					if(id <= 10 || id > 22) return;
					AiMultiTrendTagEnable(id); return;
			}
		}

		private void menuItem_trend_setting_change_Click(object sender, System.EventArgs e)
		{
			callSettingDialog();		
		} 

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewAnalogInputTrend_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged += new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
			SharedViewMain.EventListTagPropertyChanged += new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
		}
		
		private void ViewAnalogInputTrend_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged -= new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
			SharedViewMain.EventListTagPropertyChanged -= new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
		}

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
		}

		void SaveTrendRemainAI()
		{
			TagAiClass					tag;
			TREND_TAG					iTrend_tag;
			for(int pos, i = 0; i < array.Count; i++) 
			{
				iTrend_tag = (TREND_TAG)array[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, iTrend_tag.tag);
				if(pos < 0) continue;
				tag = TagLib.GetStructAI(work.tagList[pos]);
				if(tag == null) continue;
				SharedViewMain.SaveTrendRemainAI(tag);	// 남아있는 자료저장 자료간격이 2분 이상일 때...
			}
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
			SaveTrendRemainAI();							// 남아있는 자료저장 자료간격이 2분 이상일 때...
			work.bDataReadFlag = false;
			this.Invalidate();
            await Task.CompletedTask;
        }		

		void OnTagPropertyChanged(TagPublicClass tp)
		{
			if(tp.enumTagType != EnumTagType.AI) return;
			
			TagAiClass					tag;
			TREND_TAG					iTrend_tag;

			for(int pos, i = 0; i < array.Count; i++) 
			{
				iTrend_tag = (TREND_TAG)array[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, iTrend_tag.tag);
				if(pos < 0) continue;
				tag = TagLib.GetStructAI(work.tagList[pos]);
				if(tag == null || tag != (TagAiClass)tp) continue;
				iTrend_tag.full = tag.view_full-tag.view_base;
				iTrend_tag.view_full = tag.view_full;
				iTrend_tag.view_base = tag.view_base;
				array[i] = iTrend_tag;
			}
			this.Invalidate();
		}

		void AiOneTrendViewTagChangeSetting()
		{
			if(trendTagArr == null || trendTagArr.Count <= 0) return;

			multiTrendTagStruct		trendTag = (multiTrendTagStruct)trendTagArr[0];
			ai = TagLib.GetStructAI(work.tagList[work.pos]);

			trendTag.tag = ai.name;
			trendTagArr[0] = trendTag;
			AiTrendDataValueSetting();
			this.Invalidate();
		}

		
		private async void timer1_Tick(object sender, System.EventArgs e)
		{
			if(work.bTagChanged && work.trend_tag_hap == 1) 
			{
				AiOneTrendViewTagChangeSetting();
				AiTrendSetTitle();
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
            this.Invalidate();		*/
        }

        private bool _isLoading = false;

        private async Task RefreshDataAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
              	await AiMultiTrendValueRead();
                this.Invalidate();  // 준비 끝나면 Paint 다시 호출
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void menuItem_ai_setting_change_Click(object sender, System.EventArgs e)
		{
			work.pos = getMainTrendTagPos();		// 2005-3-16 추가
			CallAiSettingDialog();
		}

		private void menuItem_TagProperityModify_Click(object sender, System.EventArgs e)
		{
			work.pos = getMainTrendTagPos();		// 2005-3-16 추가
			CallTagPropertyWindows();
		}

		private void menuItem_HandInput_Click(object sender, System.EventArgs e)
		{
			work.pos = getMainTrendTagPos();		// 2005-3-16 추가
			callAiHandInputDialog();
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
