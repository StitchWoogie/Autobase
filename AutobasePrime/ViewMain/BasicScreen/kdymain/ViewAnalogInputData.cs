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
using System.Runtime.Remoting.Channels;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputData.
	/// </summary>
	public class ViewAnalogInputData : AnalogDigitalCommonDrawClass
	{
		private System.ComponentModel.IContainer components;		

		public int xnum = 80;
		public int ynum = 29;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem_setDlg_open;
		private System.Windows.Forms.ContextMenu contextMenu_AiData;
		private System.Windows.Forms.MenuItem menuItem_data_min_same;
		private System.Windows.Forms.MenuItem menuItem_data_hour_same;
		private System.Windows.Forms.MenuItem menuItem_data_day_same;
		private System.Windows.Forms.MenuItem menuItem_data_week_same;
		private System.Windows.Forms.MenuItem menuItem_data_month_same;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem_trend_hour720;
		private System.Windows.Forms.MenuItem menuItem_trend_hour1;
		private System.Windows.Forms.MenuItem menuItem_trend_hour8;
		private System.Windows.Forms.MenuItem menuItem_trend_hour24;
		private System.Windows.Forms.MenuItem menuItem_trend_hour48;
		private System.Windows.Forms.MenuItem menuItem_trend_hour72;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem_close;
		private System.Windows.Forms.Label label1;
        		
		public string[]				sDataBuf;
		public string[]				sTimeBuf;
		public string[]				sButtonBuf;		
		TagAiClass					ai;
		public ArrayList			array;
		ArrayList					trendTagArr;

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
			public string	tag;
			public double	full;
			public double	view_full;
			public double	view_base;
			public double	hihi;
			public double	high;
			public double	low;
			public double	lolo;
			public eTrendEdgeType edge;
			public Color	color;
			public  AI_TREND_VALUE[] val;
			public float	fDisplayFormat;
			public char		cDisplayFormat;
			public string	sDisplayFormat;
		};

		public TREND_TAG	trend_tag;
		long				old_save_sec;

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuItem menuItem_ai_setting_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;

		BasicScreen.kdymain.ButtonCheck2 hansolButton = new BasicScreen.kdymain.ButtonCheck2();
		
		public ViewAnalogInputData(Form parent, eDataTime data, ArrayList arr, Color backColor, int showMode)
            
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

			sDataBuf = new string[4];
			sTimeBuf = new string[5];
			sButtonBuf = new string[5];
			if(Tools.IsLangKorean()) 
			{
				sDataBuf[0] = "평균"; 
				sDataBuf[1] = "최소";
				sDataBuf[2] = "최대";
				sDataBuf[3] = "적산";
				sTimeBuf[0] = "분";
				sTimeBuf[1] = "시간";
				sTimeBuf[2] = "일";
				sTimeBuf[3] = "주";
				sTimeBuf[4] = "월";
				sButtonBuf[0] = "설정[F3]";
				sButtonBuf[1] = "보기변경[F4]";
				sButtonBuf[2] = "표시변경[F11]";
				sButtonBuf[3] = "시간변경[F12]";
				sButtonBuf[4] = "기준태그[F7]";
			}
			else if(Tools.IsLangJapanese()) 
			{
				sDataBuf[0] = "平均"; 
				sDataBuf[1] = "最小";
				sDataBuf[2] = "最大";
                sDataBuf[3] = "累計";
				sTimeBuf[0] = "分";
				sTimeBuf[1] = "時間";
				sTimeBuf[2] = "日";
				sTimeBuf[3] = "週";
				sTimeBuf[4] = "月";
				sButtonBuf[0] = "設定[F3]";
				sButtonBuf[1] = "データ[F4]";
				sButtonBuf[2] = "グラフ[F11]";
				sButtonBuf[3] = "時間変更[F12]";
				sButtonBuf[4] = "基準タグ[F7]";
			}
			else if(Tools.IsLangChinese()) 
			{
				sDataBuf[0] = "平均"; 
				sDataBuf[1] = "最小";
				sDataBuf[2] = "最大";
				sDataBuf[3] = "累计";
				sTimeBuf[0] = "分";
				sTimeBuf[1] = "时间";
				sTimeBuf[2] = "日";
				sTimeBuf[3] = "周";
				sTimeBuf[4] = "月";
				sButtonBuf[0] = "设置[F3]";
				sButtonBuf[1] = "资料[F4]";
				sButtonBuf[2] = "图表[F11]";
				sButtonBuf[3] = "更改时间[F12]";
				sButtonBuf[4] = "标准标记[F7]";
			}
			else 
			{
				sDataBuf[0] = "Average";
				sDataBuf[1] = "Min";
				sDataBuf[2] = "Max";
				sDataBuf[3] = "Sum";
				sTimeBuf[0] = "Min";
				sTimeBuf[1] = "Hour";
				sTimeBuf[2] = "Day";
				sTimeBuf[3] = "Week";
				sTimeBuf[4] = "Month";
				sButtonBuf[0] = "Set[F3]";
				sButtonBuf[1] = "Data[F4]";
				sButtonBuf[2] = "Display[F11]";
				sButtonBuf[3] = "Time[F12]";
				sButtonBuf[4] = "Main Tag[F7]";
			}

			work.dataTime = data;
			work.dataSort = eDataSort.AVERAGE;
			work.dataDispType = (showMode >= 0 && showMode <= 2) ? (eDataDispType)showMode : eDataDispType.DECIMAL;
			
			if(arr.Count > 1) xnum = 91;
			work.backColor = backColor;
			work.eTagType = EnumTagType.AI;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;

			DetailWindowSetSize(xnum, ynum);
			AiDataInitValueSetting();
			trendTagArr = arr;
			AiDataValueSetting();
			AiDataSetTitle();

           SaveTrendRemainAI(); // 시작할 때도 남아있는 트랜드를 저장해 준다. 2010-12-21
		}

		public void AiDataInitValueSetting()
		{
            work.dt = DateTimeServer.Now;
			work.bDataReadFlag = false;
			getCurrentSec();
			work.Ix = 0;
			work.Iy = 0;
			work.dataSort = eDataSort.AVERAGE;
			work.mainTrendPos = 0;			// 표시할 시작 트랜드 single = 항상 0, multi 0 ~
			work.bGuideLine = false;
			work.bGuideAlarm = true;
			work.bCurrPosLine = false;
            work.dt = DateTimeServer.Now;
			array = new ArrayList();
			work.trend_hap = 60;			// 최대 60개 자료 분 : 60, 시간 : 24 ...			
		}
		
		public void AiDataValueSetting()
		{
			int 					i, pos;
			multiTrendTagStruct		multi = new multiTrendTagStruct();
			
			array.Clear();						// 어레이를 클리어, 
			trend_tag = new TREND_TAG();
			//work.trend_tag_hap = trendTagArr.Count;// 2005-3-16 삭제
			
			for(i = 0; i < trendTagArr.Count; i++)	// 2005-3-16 수정
			{
				multi = (multiTrendTagStruct)trendTagArr[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, multi.tag);
				if(pos <= 0) pos = 0;

				if(i == 0) work.pos = pos;
				trend_tag.tag = multi.tag;
				trend_tag.color = multi.color;
				trend_tag.edge = multi.edge;
				trend_tag.flag = true;
				ai = TagLib.GetStructAI(work.tagList[pos]);
				trend_tag.full = ai.view_full-ai.view_base;
				trend_tag.view_full = ai.view_full;
				trend_tag.view_base = ai.view_base;
				trend_tag.hihi = ai.hihi;
				trend_tag.high = ai.high;
				trend_tag.low = ai.low;
				trend_tag.lolo = ai.lolo;				
				trend_tag.val = new AI_TREND_VALUE[work.trend_hap];		// 최대 60개 분별 자료보기....
				trend_tag.sDisplayFormat = ai.sDisplayFormat;
				trend_tag.fDisplayFormat = ai.fDisplayFormat;
				trend_tag.cDisplayFormat = ai.cDisplayFormat;
				array.Add(trend_tag);
			}
			work.trend_tag_hap = array.Count;			// 2005-3-16 추가
		}

		void getCurrentSec()
		{
            DateTime dt = DateTimeServer.Now;
			old_save_sec = dt.Minute*60 + dt.Second;
		}

		async Task AiMultiDataValueRead()
		{
			int 					i, hap;
			
			getCurrentSec();
			switch(work.dataTime)
			{
				case eDataTime.MIN : hap =  60; break;
				case eDataTime.DAY : hap = TimeUtil.getmonthlimit(work.dt.Year, work.dt.Month); break;
				case eDataTime.WEEK : hap = TimeUtil.GetWeekCount(work.dt.Year); break;
				case eDataTime.MONTH : hap = 12; break;
				default : hap = 24; break;// eDataTime.HOUR :
			}
			
			for(i = 0; i < work.trend_tag_hap; i++) 
			{
				trend_tag = (TREND_TAG)array[i];
				await DataSetToBuf(trend_tag, hap);
			}
			work.bDataReadFlag = true;
		}

		async Task DataSetToBuf(TREND_TAG trend_one, int hap)
		{
			DataGate	gate = new DataGate();
			DataSet		ds;

			switch(work.dataTime)
			{
				case eDataTime.MIN : ds = await gate.GetDataAi(trend_one.tag, EnumDataType.AveMinMaxSum, EnumDataTime.Minute, work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, hap, 1); break;
				case eDataTime.DAY : ds = await gate.GetDataAi(trend_one.tag, EnumDataType.AveMinMaxSum, EnumDataTime.Day, work.dt.Year, work.dt.Month, 1, 0, 0, hap, 1); break;
				case eDataTime.WEEK : ds = await gate.GetDataAi(trend_one.tag, EnumDataType.AveMinMaxSum, EnumDataTime.Week, work.dt.Year, 1, 1, 0, 0, hap, 1); break;
				case eDataTime.MONTH : ds = await gate.GetDataAi(trend_one.tag, EnumDataType.AveMinMaxSum, EnumDataTime.Month, work.dt.Year, 1, 1, 0, 0, hap, 1); break;
				default : ds = await gate.GetDataAi(trend_one.tag, EnumDataType.AveMinMaxSum, EnumDataTime.Hour, work.dt.Year, work.dt.Month, work.dt.Day, 0, 0, hap, 1); break;// eDataTime.HOUR :
			}

			DataRow row;
			for(int i = 0; i < hap; i++) 
			{
				row = ds.Tables[0].Rows[i];
				trend_one.val[i].bExist = (ConvertTool.ToInt16(row[0].ToString()) == 1);
				trend_one.val[i].max = ConvertTool.ToSingle(row["MAX"].ToString());	
				trend_one.val[i].min = ConvertTool.ToSingle(row["MIN"].ToString());	
				trend_one.val[i].sum = ConvertTool.ToSingle(row["SUM"].ToString());
				trend_one.val[i].average = ConvertTool.ToSingle(row["AVE"].ToString());
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputData));
            this.contextMenu_AiData = new System.Windows.Forms.ContextMenu();
            this.menuItem_trend_hour1 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour8 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour24 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour48 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour72 = new System.Windows.Forms.MenuItem();
            this.menuItem_trend_hour720 = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_data_min_same = new System.Windows.Forms.MenuItem();
            this.menuItem_data_hour_same = new System.Windows.Forms.MenuItem();
            this.menuItem_data_day_same = new System.Windows.Forms.MenuItem();
            this.menuItem_data_week_same = new System.Windows.Forms.MenuItem();
            this.menuItem_data_month_same = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem_ai_setting_change = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem_setDlg_open = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_close = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // contextMenu_AiData
            // 
            this.contextMenu_AiData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_trend_hour1,
            this.menuItem_trend_hour8,
            this.menuItem_trend_hour24,
            this.menuItem_trend_hour48,
            this.menuItem_trend_hour72,
            this.menuItem_trend_hour720,
            this.menuItem1,
            this.menuItem_data_min_same,
            this.menuItem_data_hour_same,
            this.menuItem_data_day_same,
            this.menuItem_data_week_same,
            this.menuItem_data_month_same,
            this.menuItem6,
            this.menuItem_ai_setting_change,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem_setDlg_open,
            this.menuItem2,
            this.menuItem_close});
            // 
            // menuItem_trend_hour1
            // 
            this.menuItem_trend_hour1.Index = 0;
            resources.ApplyResources(this.menuItem_trend_hour1, "menuItem_trend_hour1");
            this.menuItem_trend_hour1.Click += new System.EventHandler(this.menuItem_trend_hour1_Click);
            // 
            // menuItem_trend_hour8
            // 
            this.menuItem_trend_hour8.Index = 1;
            resources.ApplyResources(this.menuItem_trend_hour8, "menuItem_trend_hour8");
            this.menuItem_trend_hour8.Click += new System.EventHandler(this.menuItem_trend_hour8_Click);
            // 
            // menuItem_trend_hour24
            // 
            this.menuItem_trend_hour24.Index = 2;
            resources.ApplyResources(this.menuItem_trend_hour24, "menuItem_trend_hour24");
            this.menuItem_trend_hour24.Click += new System.EventHandler(this.menuItem_trend_hour24_Click);
            // 
            // menuItem_trend_hour48
            // 
            this.menuItem_trend_hour48.Index = 3;
            resources.ApplyResources(this.menuItem_trend_hour48, "menuItem_trend_hour48");
            this.menuItem_trend_hour48.Click += new System.EventHandler(this.menuItem_trend_hour48_Click);
            // 
            // menuItem_trend_hour72
            // 
            this.menuItem_trend_hour72.Index = 4;
            resources.ApplyResources(this.menuItem_trend_hour72, "menuItem_trend_hour72");
            this.menuItem_trend_hour72.Click += new System.EventHandler(this.menuItem_trend_hour72_Click);
            // 
            // menuItem_trend_hour720
            // 
            this.menuItem_trend_hour720.Index = 5;
            resources.ApplyResources(this.menuItem_trend_hour720, "menuItem_trend_hour720");
            this.menuItem_trend_hour720.Click += new System.EventHandler(this.menuItem_trend_hour720_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 6;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItem_data_min_same
            // 
            this.menuItem_data_min_same.Index = 7;
            resources.ApplyResources(this.menuItem_data_min_same, "menuItem_data_min_same");
            this.menuItem_data_min_same.Click += new System.EventHandler(this.menuItem_data_min_same_Click);
            // 
            // menuItem_data_hour_same
            // 
            this.menuItem_data_hour_same.Index = 8;
            resources.ApplyResources(this.menuItem_data_hour_same, "menuItem_data_hour_same");
            this.menuItem_data_hour_same.Click += new System.EventHandler(this.menuItem_data_hour_same_Click);
            // 
            // menuItem_data_day_same
            // 
            this.menuItem_data_day_same.Index = 9;
            resources.ApplyResources(this.menuItem_data_day_same, "menuItem_data_day_same");
            this.menuItem_data_day_same.Click += new System.EventHandler(this.menuItem_data_day_same_Click);
            // 
            // menuItem_data_week_same
            // 
            this.menuItem_data_week_same.Index = 10;
            resources.ApplyResources(this.menuItem_data_week_same, "menuItem_data_week_same");
            this.menuItem_data_week_same.Click += new System.EventHandler(this.menuItem_data_week_same_Click);
            // 
            // menuItem_data_month_same
            // 
            this.menuItem_data_month_same.Index = 11;
            resources.ApplyResources(this.menuItem_data_month_same, "menuItem_data_month_same");
            this.menuItem_data_month_same.Click += new System.EventHandler(this.menuItem_data_month_same_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 12;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // menuItem_ai_setting_change
            // 
            this.menuItem_ai_setting_change.Index = 13;
            resources.ApplyResources(this.menuItem_ai_setting_change, "menuItem_ai_setting_change");
            this.menuItem_ai_setting_change.Click += new System.EventHandler(this.menuItem_ai_setting_change_Click);
            // 
            // menuItem_TagProperityModify
            // 
            this.menuItem_TagProperityModify.Index = 14;
            resources.ApplyResources(this.menuItem_TagProperityModify, "menuItem_TagProperityModify");
            this.menuItem_TagProperityModify.Click += new System.EventHandler(this.menuItem_TagProperityModify_Click);
            // 
            // menuItem_HandInput
            // 
            this.menuItem_HandInput.Index = 15;
            resources.ApplyResources(this.menuItem_HandInput, "menuItem_HandInput");
            this.menuItem_HandInput.Click += new System.EventHandler(this.menuItem_HandInput_Click);
            // 
            // menuItem_setDlg_open
            // 
            this.menuItem_setDlg_open.Index = 16;
            resources.ApplyResources(this.menuItem_setDlg_open, "menuItem_setDlg_open");
            this.menuItem_setDlg_open.Click += new System.EventHandler(this.menuItem_setDlg_open_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 17;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem_close
            // 
            this.menuItem_close.Index = 18;
            resources.ApplyResources(this.menuItem_close, "menuItem_close");
            this.menuItem_close.Click += new System.EventHandler(this.menuItem_close_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ViewAnalogInputData
            // 
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "ViewAnalogInputData";
            this.Load += new System.EventHandler(this.ViewAnalogInputData_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputData_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewAnalogInputData_Paint);
            this.Closed += new System.EventHandler(this.ViewAnalogInputData_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputData_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewAnalogInputData_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion

		
		void AiDataSetTitle()
		{
			ViewAnalogInputDataMain	form = ViewAnalogInputDataMain.formThis;

			if(work.trend_tag_hap > 1) 
			{
				if(Tools.IsLangKorean()) form.Text = "아날로그 멀티 자료보기";
				else form.Text = "Analog Multi Data View";
				return;
			}			
			
			if(Tools.IsLangKorean()) form.Text = string.Format("아날로그 자료보기 - {0}", ai.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("查看模拟资料 - {0}", ai.name);
			else form.Text = string.Format("Analog Data View - {0}", ai.name);
		}

		//Bitmap bitmapClient = null;
		
		private void ViewAnalogInputData_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics gScreen = e.Graphics;
						
			DetailWindowSetSize(xnum, ynum);
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.

			/*
			if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
				bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);

			Graphics g = Graphics.FromImage(bitmapClient);
			*/

            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);


			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;
			
			AnalogCountFirstScreen(g);			
			AiTotalSumDataDraw(g);
            AiCurrentReadDataDraw(g);

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

		public void AnalogCountFirstScreen(Graphics g)
		{
			int						i, Ix, Iy, fontX, fontY;
			String					buf;
			StringFormat			format = new StringFormat();
			
			//trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			//if(pos <= 0) pos = 0;
			//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
			if(getTrendTagAndAiClass(work.mainTrendPos) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.gcls(g, 0, 0, work.width, work.height, SharedData.colorTotal.BACK);
			if(work.trend_tag_hap > 1) DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+(int)(fontX*90.5), Iy+(int)(fontY*28.5), work.grayColor);
			else 
			{
				DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*28.5), work.grayColor);
				buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
				format.Alignment = StringAlignment.Center;
				DrawClass.WinDrawText(g, Ix+fontX*15,  Iy+fontY+1, fontX*13, fontY, buf, Color.Black, work.grayColor, work.f, format);
			}

			if(ai.bFileSave != 0) DrawClass.PushBox2(g, Ix+fontX*47, Iy+(int)(fontY*1.3), Ix+fontX*48, Iy+(int)(fontY*1.8), work.grayColor);

			DrawClass.PushBox2(g, Ix+fontX*28, Iy+fontY, Ix+fontX*46, Iy+fontY*2, Color.Blue);			
			if(Tools.IsLangKorean())
				buf = String.Format("{0}별 {1}자료", sTimeBuf[(int)work.dataTime], sDataBuf[(int)work.dataSort]);
			else if(Tools.IsLangJapanese())
				buf = String.Format("{0}別 {1}データ", sTimeBuf[(int)work.dataTime], sDataBuf[(int)work.dataSort]);
			else if(Tools.IsLangChinese())
				buf = String.Format("{0} {1}资料", sTimeBuf[(int)work.dataTime], sDataBuf[(int)work.dataSort]);
			else
				buf = String.Format("{0} {1} Data", sTimeBuf[(int)work.dataTime], sDataBuf[(int)work.dataSort]);
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, Ix+fontX*28, Iy+fontY+1, fontX*18, fontY, buf, Color.White, Color.Blue, work.f, format);

			if(Tools.IsLangKorean()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*4), fontX*39, "태그:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*5.5), fontX*50, "설명:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*5.5), fontX*10, "단위:", ai.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*4), fontX*39, "タグ:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*5.5), fontX*50, "説明:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*5.5), fontX*10, "単位:", ai.unit, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*4), fontX*39, "标记:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*5.5), fontX*50, "描述:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*5.5), fontX*10, "单位:", ai.unit, Color.Black, work.grayColor);
			}
			else 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*4), fontX*39, "Tag:", ai.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*0.5), Iy+(int)(fontY*5.5), fontX*50, "Des:", ai.description, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*60), Iy+(int)(fontY*5.5), fontX*10, "Unit:", ai.unit, Color.Black, work.grayColor);
			}
	
			DrawClass.PushBox2(g, Ix+fontX*3, Iy+(int)(fontY*2.5), Ix+fontX*24, Iy+(int)(fontY*3.5), work.grayColor);
			switch(work.dataTime) 
			{
				case eDataTime.MIN :
					if(Tools.IsLangKorean()) 
						buf = String.Format("{0,4:d4}년{1,2:d2}월{2,2:d2}일{3,2:d2}시", work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour);
					else if(Tools.IsLangJapanese()) 
						buf = String.Format("{0,4:d4}年{1,2:d2}月{2,2:d2}日{3,2:d2}時", work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour);
					else if(Tools.IsLangChinese()) 
						buf = String.Format("{0,4:d4}年{1,2:d2}月{2,2:d2}日{3,2:d2}时", work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour);
					else
						buf = String.Format("{0,4:d4}/{1,2:d2}/{2,2:d2} {3,2:d2}", work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour);
					break;					
				case eDataTime.HOUR :
					if(Tools.IsLangKorean()) 
						buf = String.Format("{0,4:d4}년{1,2:d2}월{2,2:d2}일", work.dt.Year, work.dt.Month, work.dt.Day);
					else if(Tools.IsLangJapanese()) 
						buf = String.Format("{0,4:d4}年{1,2:d2}月{2,2:d2}日", work.dt.Year, work.dt.Month, work.dt.Day);
					else if(Tools.IsLangChinese()) 
						buf = String.Format("{0,4:d4}年{1,2:d2}月{2,2:d2}日", work.dt.Year, work.dt.Month, work.dt.Day);
					else
						buf = String.Format("{0,4:d4}/{1,2:d2}/{2,2:d2}", work.dt.Year, work.dt.Month, work.dt.Day);
					break;
				case eDataTime.DAY :
					if(Tools.IsLangKorean())
						buf = String.Format("{0,4:d4}년{1,2:d2}월", work.dt.Year, work.dt.Month);
					else if(Tools.IsLangJapanese())
						buf = String.Format("{0,4:d4}年{1,2:d2}月", work.dt.Year, work.dt.Month);
					else if(Tools.IsLangChinese())
						buf = String.Format("{0,4:d4}年{1,2:d2}月", work.dt.Year, work.dt.Month);
					else
						buf = String.Format("{0,4:d4}/{1,2:d2}", work.dt.Year, work.dt.Month);
					break;
				default:
					if(Tools.IsLangKorean())
						buf = String.Format("{0,4:d4}년", work.dt.Year);
					else if(Tools.IsLangJapanese())
						buf = String.Format("{0,4:d4}年", work.dt.Year);
					else if(Tools.IsLangChinese())
						buf = String.Format("{0,4:d4}年", work.dt.Year);
					else
						buf = String.Format("{0,4:d4}", work.dt.Year);
					break;
			}
			DrawClass.GrayDrawText(g, Ix+fontX*2, Iy+(int)(fontY*2.5)+1, fontX*21, fontY, buf, work.grayColor, work.f, format);

            for (i = 0; i < 4; i++)
            {
                if (TotalConfig.eOemType == EnumOemType.SBAS && i == 0) continue;   // SBAS는 설정버튼을 그리지 않는다.

                hansolButton.ButtonCheckDraw2(g, Ix + fontX * (i * 18 + 5), Iy + (int)(fontY * 26.75), Ix + fontX * (i * 18 + 19), work.Iy + (int)(work.fontY * 28.25), work.f, sButtonBuf[i], StringAlignment.Center);
            }

			if(work.trend_tag_hap <= 1) return;
			multiDataTagElementDraw(g);
		}

		private void multiDataTagElementDraw(Graphics g)
		{
			int						i, count;
			string					buf;
			
			count = array.Count;
			if(count > 10) count = 10;

			hansolButton.ButtonCheckDraw2(g, work.Ix+work.fontX*77, work.Iy+(int)(work.fontY*7.5), work.Ix+work.fontX*90, work.Iy+(int)(work.fontY*8.7), work.f, sButtonBuf[4], StringAlignment.Center);
			
			for(i = 0; i < count; i++) 
			{
				//trend_tag = (TREND_TAG)array[(work.mainTrendPos+i)%array.Count];
				//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
				//if(pos <= 0) pos = 0;
				//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
				if(getTrendTagAndAiClass(work.mainTrendPos+i) == false) continue;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치

				DrawClass.PushBox2(g, work.Ix+work.fontX*77, work.Iy+(int)(work.fontY*(9.2+i*1.7)), work.Ix+(int)(work.fontX*77.8), work.Iy+(int)(work.fontY*(10.3+i*1.7)), trend_tag.color);
				if(trend_tag.flag) buf = String.Format("{0} {1}", "v", ai.tag);
				else			   buf = String.Format("{0}", ai.tag);
				hansolButton.ButtonCheckDraw2(g, work.Ix+work.fontX*78, work.Iy+(int)(work.fontY*(9+i*1.7)), work.Ix+work.fontX*90, work.Iy+(int)(work.fontY*(10.5+i*1.7)), work.f, buf, StringAlignment.Near);
			}			
		}


		public void AiTotalSumDataDraw(Graphics g)
		{
			int						Ix, Iy, fontX, fontY;
			String					buf;
			StringFormat			format = new StringFormat();
			
			//trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			//if(pos <= 0) pos = 0;
			//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
			if(getTrendTagAndAiClass(work.mainTrendPos) == false) return;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;			

			DrawClass.PushBox2(g, Ix+fontX*50, Iy+fontY, Ix+(int)(fontX*78.5), Iy+fontY*3, work.grayColor);
			if(Tools.IsLangKorean())
				buf = String.Format("전체 적산값 :{0,10:f2}", ai.fSumTotal);
			else if(Tools.IsLangJapanese())
                buf = String.Format("全体累計値 :{0,10:f2}", ai.fSumTotal);
			else if(Tools.IsLangChinese())
				buf = String.Format("全体累计值 :{0,10:f2}", ai.fSumTotal);
			else
				buf = String.Format("Total Sum :{0,10:f2}", ai.fSumTotal);
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, Ix+fontX*50, Iy+fontY+1, (int)(fontX*28.5), fontY, buf, Color.Black, work.grayColor, work.f, format);
			
			buf = string.Format("({0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02} ~ )", ai.tSumTotal.Year, ai.tSumTotal.Month, ai.tSumTotal.Day, ai.tSumTotal.Hour, ai.tSumTotal.Minute);
			DrawClass.GrayDrawText(g, Ix+fontX*50, Iy+fontY*2, (int)(fontX*28.5), fontY, buf, work.grayColor, work.f, format);

			DrawClass.PushBox2(g, Ix+fontX*50, Iy+(int)(fontY*3.2), Ix+(int)(fontX*78.5), Iy+(int)(fontY*5.2), work.grayColor);
			if(Tools.IsLangKorean())
				buf = String.Format("부분 적산값 :{0,10:f2}", ai.fSumPart);
			else if(Tools.IsLangJapanese())
                buf = String.Format("部分累計値 :{0,10:f2}", ai.fSumPart);
			else if(Tools.IsLangChinese())
				buf = String.Format("部分累计值 :{0,10:f2}", ai.fSumPart);
			else
				buf = String.Format("Period Sum :{0,10:f2}", ai.fSumPart);
			DrawClass.WinDrawText(g, Ix+fontX*50, Iy+(int)(fontY*3.2)+1, (int)(fontX*28.5), fontY, buf, Color.Black, work.grayColor, work.f, format);
			
			buf = string.Format("({0,4:d04}/{1,2:d02}/{2,2:d02} {3,2:d02}:{4,2:d02} ~ )", ai.tSumPart.Year, ai.tSumPart.Month, ai.tSumPart.Day, ai.tSumPart.Hour, ai.tSumPart.Minute);			
			DrawClass.GrayDrawText(g, Ix+fontX*50, Iy+(int)(fontY*4.2), (int)(fontX*28.5), fontY, buf, work.grayColor, work.f, format);
		}

		public void AiCurrentReadDataDraw(Graphics g)
		{						
			//if(work.bDataReadFlag == false) //timer로 이동
			//{
			//	Cursor = Cursors.WaitCursor;
			//	AiMultiDataValueRead();
			//}
			switch(work.dataTime)
			{
				case eDataTime.MIN : MinDataScreenDraw(g); break;
				case eDataTime.HOUR : HourDataScreenDraw(g); break;
				case eDataTime.DAY : DayDataScreenDraw(g); break;
				case eDataTime.WEEK : WeekDataScreenDraw(g); break;
				case eDataTime.MONTH : MonthDataScreenDraw(g); break;
			}
			if(Cursor != Cursors.Arrow) Cursor = Cursors.Arrow;
			work.bChangeMinFlag = true;			
		}

		public void MinDataScreenDraw(Graphics g)
		{
			if(work.dataDispType != eDataDispType.DECIMAL) 
			{
				ReadDataShowToGraph(g, 60, 5, 0);
				return;
			}
			ReadDataShowToString(g, work.Iy+(int)(work.fontY*7.5), (int)(work.fontY*1.15), work.fontX*18, work.fontX*4, 4, 15, 60);
		}

		private void HourDataScreenDraw(Graphics g)
		{
			if(work.dataDispType != eDataDispType.DECIMAL) 
			{
				ReadDataShowToGraph(g, 24, 1, 0);
				return;
			}
			ReadDataShowToString(g, (int)(work.Iy+work.fontY*7.5), (int)(work.fontY*1.45), work.fontX*36, work.fontX*13, 2, 12, 24);
		}

		void DayDataScreenDraw(Graphics g)
		{
			if(work.dataDispType != eDataDispType.DECIMAL) 
			{
				ReadDataShowToGraph(g, TimeUtil.getmonthlimit(work.dt.Year, work.dt.Month), 2, 1);
				return;
			}
			ReadDataShowToString(g, work.Iy+(int)(work.fontY*8), (int)(work.fontY*1.5), work.fontX*24, work.fontX*6, 3, 11, TimeUtil.getmonthlimit(work.dt.Year, work.dt.Month));
		}

		private void getAllDayToMonthDay(int year, int allday, ref int mon, ref int day)
		{
			int						i, j, hap = allday;			
			
			for(i = 1; i <= 12; i++) 
			{
				j = TimeUtil.getmonthlimit(year, i);
				if(j >= hap) 
				{
					mon = i;
					day = hap;
					return;
				}
				hap -= j;
			}
			mon = 12;
			day = 31;          // end month 
		}

		private void WeekDataScreenDraw(Graphics g)
		{
			int	 					i, y, size,	ju, yo;
			int 					emonth = 12, eday = 31, day;
			String 					buf;
			Color					iTcolor, iBcolor;
			StringFormat			format = new StringFormat();

			ju = TimeUtil.GetWeekCount(work.dt.Year);
			yo = TimeUtil.GetWeekDay(work.dt.Year, 1, 1);
			
			if(work.dataDispType != eDataDispType.DECIMAL) 
			{
				ReadDataShowToGraph(g, ju, 5, 1);
				return;
			}

			y = work.Iy+(int)(work.fontY*7.5);
			size = (int)(work.fontY*1.25);
			ReadDataShowToString(g, y, size, work.fontX*18, work.fontX*7, 4, 14, ju);

            iBcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.White : Color.Black;
            iTcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.Black : Color.White;
			y += (int)(size*1.15)+1;
			format.Alignment = StringAlignment.Center;
			for(i = 0, day = 7-yo; i < ju; i++) 
			{
				getAllDayToMonthDay(work.dt.Year, day, ref emonth, ref eday);
				buf = String.Format("-{0,2:d02}/{1,2:d02}", emonth, eday);
				DrawClass.WinDrawText(g, work.Ix+(int)(work.fontX*3)+(i/14)*work.fontX*18, y+(i%14)*size, work.fontX*9, work.fontY, buf, iTcolor, iBcolor, work.f, format);
				day += 7;
			}
		}

		private void MonthDataScreenDraw(Graphics g)
		{
			if(work.dataDispType != eDataDispType.DECIMAL) 
			{
				ReadDataShowToGraph(g, 12, 1, 1);
				return;
			}
			ReadDataShowToString(g, work.Iy+(int)(work.fontY*8), (int)(work.fontY*1.4),	work.fontX*72, work.fontX*28, 1, 12, 12);
		}

		
		private void ReadDataShowToGraph(Graphics g, int limit, int num, int start)
		{
			int	 							i, x;
			int								Ix, Iy, fontX, fontY;
			int								pos, pos2, width, height;
			String						  	buf;
			//double							f1, f2;
			double							curr = 0.0, oldVal = 0.0, total = 0.0;
			Color							color;
			StringFormat					format = new StringFormat();
			
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;
			width = fontX*60;
			height = fontY*17;
			x = Ix+fontX*16;
			
			trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];

			DrawClass.PushBox2(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*7), Ix+fontX*15, Iy+(int)(fontY*25), work.grayColor);
			AiUpLinePosDraw(g, fontX, fontY, Ix+fontX*13, Iy+(int)(fontY*7.5), fontY*17, trend_tag.view_full, trend_tag.view_base, trend_tag.sDisplayFormat, trend_tag.fDisplayFormat, trend_tag.cDisplayFormat);
			DrawClass.PushBox2(g, Ix+(int)(fontX*15.9), Iy+(int)(fontY*7.5), Ix+fontX*76, Iy+(int)(fontY*24.5)-1, work.backColor);

			if(work.bGuideLine) 
			{
				for(i = 1; i < 20; i++)	DrawClass.gline(g, x, Iy+(int)(fontY*7.5)+i*height/20, Ix+fontX*76+1, Iy+(int)(fontY*7.5)+i*height/20, Color.DarkGray);
				for(i = 0; i < limit; i++) 
				{
					if(limit > 40 && (i-1) % 5 != 4) continue;
					DrawClass.gline(g, x+(int)((float)(i+0.5)*width/limit), Iy+(int)(fontY*7.5), x+(int)((float)(i+0.5)*width/limit), Iy+(int)(fontY*24.5), Color.DarkGray);
				}
			}
			if(work.bGuideAlarm) 
			{
				trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
                double max = TagUtil.GetDisplayValue(ai, trend_tag.view_full);
                double min = TagUtil.GetDisplayValue(ai, trend_tag.view_base);
                AiTrendGraphStatusLineDraw(g, ai, work.Iy + (int)(work.fontY * 24.5), max, min, work.fontY * 17);
			}

			DrawClass.gline(g, x+width/(limit*2), Iy+(int)(fontY*24.7), x+width-width/(limit*2), Iy+(int)(fontY*24.7), Color.DarkGray);
			format.Alignment = StringAlignment.Center;
			for(i = 0; i < limit; i++) 
			{
				if(i == 0 || (i+start) % num == 0 || ((i+start) % num != 0 && i == limit-1)) 
				{
					buf = String.Format("{0}", i+start);
					DrawClass.GrayDrawText(g, (int)(x+(float)(i+0.5)*width/limit)-fontX*2, Iy+(int)(fontY*25.3), fontX*4, fontY, buf, work.grayColor, work.f, format);
					DrawClass.gline(g, x+(int)((i+0.5)*width/limit), Iy+(int)(fontY*24.7), x+(int)((i+0.5)*width/limit),  Iy+(int)(fontY*25.2), Color.DarkGray);
				}
				else
					DrawClass.gline(g, x+(int)((float)(i+0.5)*width/limit), Iy+(int)(fontY*24.7), x+(int)((i+0.5)*width/limit),  Iy+(int)(fontY*25), Color.DarkGray);
			}

			for(pos = 0, pos2 = 0; pos < work.trend_tag_hap; pos++) 
			{
				trend_tag = (TREND_TAG)array[pos];
				if(trend_tag.flag == false) continue;
				for(i = 0; i < limit; i++) 
				{
					switch(work.dataDispType) 
					{
						case eDataDispType.BAR: // rect bar graph
							if(trend_tag.full == 0.0) break;
							if(trend_tag.val[i].bExist == false) break;
							getCurrentDataAndSumTotal(i, i, ref curr, ref total);
							if(curr < trend_tag.view_base) break;
							if(work.trend_tag_hap > 1) color = trend_tag.color;
							else if(work.dataSort == eDataSort.SUM) color = trend_tag.color;
							else if(curr > trend_tag.hihi) color = SharedData.colorTotal.HIHI;
							else if(curr > trend_tag.high) color = SharedData.colorTotal.HIGH;
							else if(curr > trend_tag.low)  color = SharedData.colorTotal.LOW;
							else              	           color = SharedData.colorTotal.LOLO;
							if(curr > trend_tag.view_full) curr = trend_tag.view_full;
							curr -= trend_tag.view_base;
							DrawClass.gcls(g, x+(int)((float)(i+0.25+pos2*0.2)*width/limit), Iy+(int)(fontY*24.5)-(int)(curr*(float)height/trend_tag.full),	x+(int)((float)(i+0.75+pos2*0.2)*width/limit), Iy+(int)(fontY*24.5), color);
							break;
						case eDataDispType.LINE:
							if(i == 0) break;
							if(trend_tag.full == 0.0) break;

                            if (trend_tag.val[i - 1].bExist == false && trend_tag.val[i].bExist == false)
                                break;
                            
							getCurrentDataAndSumTotal(i, i, ref curr, ref total);
							if(trend_tag.val[i].bExist == false || curr < trend_tag.view_base) curr = trend_tag.view_base;							
							if(curr > trend_tag.view_full) curr = trend_tag.view_full;
							curr -= trend_tag.view_base;

							getCurrentDataAndSumTotal(i-1, i-1, ref oldVal, ref total);
							if(trend_tag.val[i-1].bExist == false || oldVal < trend_tag.view_base) oldVal = trend_tag.view_base;
							if(oldVal > trend_tag.view_full) oldVal = trend_tag.view_full;
							oldVal -= trend_tag.view_base;

							DrawClass.gline(g, x+(int)((float)(i-0.5)*width/limit), Iy+(int)(fontY*24.5)-(int)(oldVal*(float)height/trend_tag.full), x+(int)((float)(i+0.5)*width/limit), Iy+(int)(fontY*24.5)-(int)(curr*(float)height/trend_tag.full), trend_tag.color);
							DrawTrendEdgePointDisplay(g, fontX, x+(int)((float)(i-0.5)*width/limit), Iy+(int)(fontY*24.5)-(int)(oldVal*(float)height/trend_tag.full), trend_tag.edge, trend_tag.color);
							DrawTrendEdgePointDisplay(g, fontX, x+(int)((float)(i+0.5)*width/limit), Iy+(int)(fontY*24.5)-(int)(curr*(float)height/trend_tag.full), trend_tag.edge, trend_tag.color);
							break;
					}
				}
				pos2++;
			}
		}


		private void getCurrentDataAndSumTotal(int i, int count, ref double curr, ref double total)
		{
			switch(work.dataSort) 
			{
				case eDataSort.MAX : 
					curr = trend_tag.val[i].max; 
					total = (total < curr) ? curr : total; 
					break;							
				case eDataSort.MIN : 
					curr = trend_tag.val[i].min;
					if(count == 0) 
					{ 
						total = curr; 
						break;
					}
					total = (total > curr) ? curr : total; 
					break;
				case eDataSort.SUM : 
					curr = trend_tag.val[i].sum; 
					total += curr;
					break;
				default :
					curr = trend_tag.val[i].average; 
					total += curr;
					break;
			}			
		}


		private void ReadDataShowToString(Graphics g, int sy, int size, int width, int width2, int num, int line, int limit)
		{
			int				 	i, x, y, count;
			int	 				Ix, fontX;
			double 				total = 0.0F, imsi = 0.0F;
			Color				iBcolor, iTcolor;
			String 				buf;
			StringFormat		format = new StringFormat();

			trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			
			Ix = work.Ix;
			fontX = work.fontX;
			x = Ix+fontX*4;
			y = sy;
            iBcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.White : Color.Black;
            iTcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.Black : Color.White;
			DrawClass.PushBox(g, x, y-(int)(size*0.2), Ix+fontX*76, y+(int)(size*0.97), iBcolor);
			DrawClass.PushBox(g, x, y+(int)(size*1.05), Ix+fontX*76, y+size*(line+1)+1, iBcolor);

			format.Alignment = StringAlignment.Center;
			for(i = 0; i < num; i++) 
			{
				buf = String.Format("{0}", sTimeBuf[(int)work.dataTime]);
				DrawClass.WinDrawText(g, x+i*width, y, width2, work.fontY, buf, Color.Green, iBcolor, work.f, format);
		
				buf = String.Format("{0}", sDataBuf[(int)work.dataSort]);
				DrawClass.WinDrawText(g, x+i*width+width2, y, width-width2, work.fontY, buf, Color.Green, iBcolor, work.f, format);
		
				DrawClass.gline(g, x+width2+i*width, y-(int)(size*0.2), x+width2+i*width, y+size*(line+1), Color.DarkGray);
				if(i >= 1) DrawClass.gline(g, x+i*width, y-(int)(size*0.2), x+i*width, y+size*(line+1), Color.DarkGray);
			}
			for(i = 2; i <= line; i++)
				DrawClass.gline(g, x, y+size*i, Ix+fontX*76-1, y+size*i, Color.DarkGray);

			y += size+(int)(size*0.15)+1;
			for(i = 0, count = 0; i < limit; i++) 
			{
				switch(work.dataTime) 
				{
					case eDataTime.MIN : buf = String.Format("{0,2:d02}", i); break;
					case eDataTime.HOUR : buf = String.Format("{0,2}:00-{1,2}:59", i, i); break;
					default: 
						buf = String.Format("{0,2}{1}", i+1, sTimeBuf[(int)work.dataTime]);
						break;
				}		
				if(work.dataTime != eDataTime.WEEK)//3)
					DrawClass.WinDrawText(g, x+(i/line)*width, y+(i%line)*size, width2, work.fontY, buf, iTcolor, iBcolor, work.f, format);

				if(trend_tag.val[i].bExist) 
				{ 
					getCurrentDataAndSumTotal(i, count, ref imsi, ref total);
					/*switch(work.dataSort) 
					{
						case eDataSort.MAX : 
							imsi = trend_tag.val[i].max; 
							total = (total < imsi) ? imsi : total; 
							break;							
						case eDataSort.MIN : 
							imsi = trend_tag.val[i].min;
							if(count == 0) 
							{ 
								total = imsi; 
								break;
							}
							total = (total > imsi) ? imsi : total; 
							break;
						case eDataSort.SUM : 
							imsi = trend_tag.val[i].sum; 
							total += imsi;
							break;
						default :
							imsi = trend_tag.val[i].average; 
							total += imsi;
							break;
					}*/
					count ++;
					//buf = String.Format("{0,3:f2}", imsi);
					buf = TagUtil.AiValueToStringOnlyPoint(trend_tag.sDisplayFormat, trend_tag.fDisplayFormat, trend_tag.cDisplayFormat, imsi);
					DrawClass.WinDrawText(g, x+(i/line)*width+width2, y+(i%line)*size, width-width2, work.fontY, buf, Color.Red, iBcolor, work.f, format);
				}
				else
					DrawClass.WinDrawText(g, x+(i/line)*width+width2, y+(i%line)*size, width-width2, work.fontY, "..", work.grayColor, iBcolor, work.f, format);
			}
			if(work.dataSort == eDataSort.AVERAGE && count != 0) total = total / (float)count;
			
			CurrentDataHapDraw(g, total, trend_tag.sDisplayFormat, trend_tag.fDisplayFormat, trend_tag.cDisplayFormat);
		}


		private void CurrentDataHapDraw(Graphics g, double total, string sFormat, float fFormat, char cFormat)
		{
			int					x, y;
			String				buf;
			StringFormat		format = new StringFormat();

			x = work.Ix+(int)(work.fontX*26);
			y = work.Iy+(int)(work.fontY*2.5);

			DrawClass.PushBox2(g, x, y, x+work.fontX*22, y+work.fontY, work.grayColor);
			//buf = String.Format("{0} {1}:{2,3:f2}", sTimeBuf[(int)work.dataTime], sDataBuf[(int)work.dataSort], total);	
			buf = String.Format("{0} {1}:{2}", sTimeBuf[(int)work.dataTime], sDataBuf[(int)work.dataSort], TagUtil.AiValueToStringOnlyPoint(sFormat, fFormat, cFormat, total));	
			format.Alignment = StringAlignment.Center;
			DrawClass.GrayDrawText(g, x, y+1, work.fontX*22, work.fontY, buf, work.grayColor, work.f, format);
		}

		public string callShowDataSortFunc()
		{
			string			buf;
			int				pos;
			
			if(work.dataSort == eDataSort.SUM) work.dataSort = eDataSort.AVERAGE;
			else work.dataSort++;
			pos = (int)work.dataSort;
			buf = String.Format("{0}[F2]", sDataBuf[pos%4]);			
			this.Invalidate();
			return buf;
		}

		public void changeDataTime(eDataTime dataTime)
		{
			if(work.dataTime == dataTime) return;
			work.bDataReadFlag = false;
			work.dataTime = dataTime;
			this.Invalidate();
		}

		private void menuItem_data_min_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(eDataTime.MIN);
		}

		private void menuItem_data_hour_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(eDataTime.HOUR);
		}

		private void menuItem_data_day_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(eDataTime.DAY);
		}

		private void menuItem_data_week_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(eDataTime.WEEK);		
		}

		private void menuItem_data_month_same_Click(object sender, System.EventArgs e)
		{
			changeDataTime(eDataTime.MONTH);
		}

		private void menuItem_setDlg_open_Click(object sender, System.EventArgs e)
		{
			callSettingDialog();
		}

		public void callSettingDialog()
		{
			ViewAnalogInputDataDlgSetting dlg = new ViewAnalogInputDataDlgSetting(this);

			eDataDispType oldDispType = work.dataDispType;
			dlg.comboBox_show_method.SelectedIndex = (int)work.dataDispType;
			trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
			dlg.comboBox_dot_type.SelectedIndex = (int)trend_tag.edge;

			int	pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
			TagAiClass	tag = TagLib.GetStructAI(work.tagList[pos]);
            //int[] pos = new int[1];
            //TagAiClass tag = TagLib.GetStructAI(trend_tag.tag, ref pos);

			if(tag == null) return;
			try 
			{
				dlg.numericUpDown_view_full.Value = (Decimal)trend_tag.view_full;
				dlg.numericUpDown_view_base.Value = (Decimal)trend_tag.view_base;
				dlg.numericUpDown_sum_total.Value = (Decimal)tag.fSumTotal;
				dlg.numericUpDown_sum_part.Value = (Decimal)tag.fSumPart;
				dlg.tSumTotal = tag.tSumTotal;
				dlg.tSumPart = tag.tSumPart;
			}
			catch
			{
			}
			dlg.checkBox_alarm_line.Checked = work.bGuideAlarm;
			dlg.checkBox_guide_line.Checked = work.bGuideLine;
            dlg.checkBoxWhiteBackground.Checked = ConfigViewMain.bBasciScreenDataViewWhiteBackground;
            dlg.StartPosition = FormStartPosition.CenterParent;
						
			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{
				work.dataDispType = (eDataDispType)dlg.comboBox_show_method.SelectedIndex;
				work.bGuideAlarm = dlg.checkBox_alarm_line.Checked;
				work.bGuideLine = dlg.checkBox_guide_line.Checked;

                ConfigViewMain.bBasciScreenDataViewWhiteBackground = dlg.checkBoxWhiteBackground.Checked;

				trend_tag = (TREND_TAG)array[work.mainTrendPos%work.trend_tag_hap];
				trend_tag.view_full = (double)dlg.numericUpDown_view_full.Value;
				trend_tag.view_base = (double)dlg.numericUpDown_view_base.Value;
				trend_tag.full = trend_tag.view_full-trend_tag.view_base;
				trend_tag.edge = (eTrendEdgeType)dlg.comboBox_dot_type.SelectedIndex;
				tag.fSumTotal = (double)dlg.numericUpDown_sum_total.Value;
				tag.fSumPart = (double)dlg.numericUpDown_sum_part.Value;
				tag.tSumTotal = dlg.tSumTotal;
				tag.tSumPart = dlg.tSumPart;
				array[work.mainTrendPos%work.trend_tag_hap] = trend_tag;

                
				this.Invalidate();

                ConfigViewMain.Save();
			}
		}

		public void callShowMethodFunc()
		{
			if(work.dataDispType == eDataDispType.LINE) work.dataDispType = eDataDispType.DECIMAL;
			else work.dataDispType++;			
			this.Invalidate();
		}

		public void AiMultiDataMainTagChange()
		{
			work.mainTrendPos++;
			work.mainTrendPos %= work.trend_tag_hap; 
			this.Invalidate();
		}

		void AiMultiDataTagEnable(int id)
		{
			int 				pos, count;
			
			count = array.Count;
			if(count <= 1) return;
			if(count > 10) count = 10;
			pos = id-11;
			if(pos < 0 || pos >= count) return;
			trend_tag = (TREND_TAG)array[(work.mainTrendPos+pos)%array.Count];
			trend_tag.flag = (trend_tag.flag) ? false : true;
			array[(work.mainTrendPos+pos)%array.Count] = trend_tag;
			this.Invalidate();
		}

		void AiDataButtonLeftMouseCheck(MouseEventArgs e)
		{
			int 				i, count;
			string				buf;

            for (i = 0; i < 4; i++)
            {
                if (TotalConfig.eOemType == EnumOemType.SBAS && i == 0) continue;   // SBAS는 설정버튼을 그리지 않는다.

                if (hansolButton.ButtonCheckDown2(this, e, work.Ix + work.fontX * (i * 18 + 5), work.Iy + (int)(work.fontY * 26.75), work.Ix + work.fontX * (i * 18 + 19), work.Iy + (int)(work.fontY * 28.25), work.f, sButtonBuf[i], i + 1)) return;
            }
			
			count = array.Count;
			if(count <= 1) return;

			if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*77, work.Iy+(int)(work.fontY*7.5), work.Ix+work.fontX*90, work.Iy+(int)(work.fontY*8.7), work.f, sButtonBuf[4], 10)) return;
			if(count > 10) count = 10;
			for(i = 0; i < count; i++) 
			{
				//trend_tag = (TREND_TAG)array[(work.mainTrendPos+i)%array.Count];
				//pos = TagLib.GetTagPosOnlyList(work.tagList, trend_tag.tag);
				//if(pos <= 0) pos = 0;
				//ai = TagLib.GetStructAI(work.tagList[pos].tag, ref work.tagList[pos].tag_pos);
				if(getTrendTagAndAiClass(work.mainTrendPos+i) == false) continue;		// 2005-3-16 수정, 태그가 없거나 잘못된 위치
				if(trend_tag.flag) buf = String.Format("{0} {1}", "v", ai.tag);
				else			   buf = String.Format("{0}", ai.tag);					
				if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*78, work.Iy+(int)(work.fontY*(9+i*1.7)), work.Ix+work.fontX*90, work.Iy+(int)(work.fontY*(10.5+i*1.7)), work.f, buf, 11+i)) return;
			}			
		}

		private void ViewAnalogInputData_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenu_AiData.Show(control, pos);
                    return;
                }
            }

			if(e.Button == MouseButtons.Left) 
			{
				AiDataButtonLeftMouseCheck(e);
				return;
			}
		
		}

		private void ViewAnalogInputData_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hansolButton.ButtonCheckMove2(this, e);
		}

		private void ViewAnalogInputData_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int			id;
			
			id = hansolButton.ButtonCheckUp2(this);
			if(id == -1) return;

			switch(id) 
			{
				case 1 : callSettingDialog(); return;
				case 2 : callShowTimeChangeFunc(); return;
				case 3 : callShowMethodFunc(); return;
				case 4 : callTimeSettingDialog(); return;
				case 10 : AiMultiDataMainTagChange(); return;
				default : 
					if(id <= 10 || id > 20) return;
					AiMultiDataTagEnable(id); return;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewAnalogInputData_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged += new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
			SharedViewMain.EventListHourChanged += new SharedViewMain.OnEventHourChanged(OnHourChanged);
			SharedViewMain.EventListTagPropertyChanged += new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
		}
		
		private void ViewAnalogInputData_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged -= new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
			SharedViewMain.EventListHourChanged -= new SharedViewMain.OnEventHourChanged(OnHourChanged);
			SharedViewMain.EventListTagPropertyChanged -= new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
		}

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
		}

		/*void readCurrentAllTagName(ref string[] tag)		// 나중에 추가할 코드
		{
			for(int i = 0; i < array.Count; i++) 
			{
				trend_tag = (TREND_TAG)array[i];
				tag[i] = trend_tag.tag;
			}
		}

		void DiTrendRemainDataSave(string[] tag)			// 나중에 추가할 코드
		{			
		}*/

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
			if(work.dataTime != eDataTime.MIN) return;		// 분자료보기가 아니다

            DateTime currTime = DateTimeServer.Now, trendTime;

			try 
			{
				currTime = currTime.AddMinutes(-1);			// 저장자료는 1 분전 이므로
				if(currTime.Second > 0) currTime = currTime.AddSeconds(-currTime.Second);// 초 이하는 무시
				trendTime = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
				if(currTime < trendTime) return;
				trendTime = trendTime.AddMinutes(work.trend_hap);
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
				trend_tag.full = tag.view_full-tag.view_base;
				trend_tag.view_full = tag.view_full;
				trend_tag.view_base = tag.view_base;
				array[i] = iTrend_tag;
			}
			this.Invalidate();
		}

		private void OnHourChanged()
		{
			if(work.dataTime == eDataTime.MIN) return;		// 분 자료보기

			work.bDataReadFlag = false;
			this.Invalidate();			
		}

		void AiOneDataViewTagChangeSetting()
		{
			if(trendTagArr == null || trendTagArr.Count <= 0) return;

			multiTrendTagStruct		trendTag = (multiTrendTagStruct)trendTagArr[0];
			TagAiClass tag = TagLib.GetStructAI(work.tagList[work.pos]);

			trendTag.tag = tag.name;
			trendTagArr[0] = trendTag;
			AiDataValueSetting();
			this.Invalidate();
		}

		private async void timer1_Tick(object sender, System.EventArgs e)
		{
			if(work.bTagChanged && work.trend_tag_hap == 1) 
			{
				AiOneDataViewTagChangeSetting();
				AiDataSetTitle();
				work.bDataReadFlag = false;						// 데이터를 다시 읽어라
				work.bTagChanged = false;          
            }

            if (work.bDataReadFlag == false)  //250924 PSU paint 에서 분리, ReadFlag false 일 때 다시 읽기.
				await RefreshDataAsync();

            /*DateTime	dt = DateTimeServer.Now;			// 현재는 필요없는 코드 2003-7-10
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
            if (_isLoading) return;  // 중복 방지
            _isLoading = true;

            try
            {
                await AiMultiDataValueRead();  // async 데이터 읽기
                this.Invalidate();             // 읽기 끝나면 다시 그리기
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

        private void menuItem_close_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }
		
	}
}
