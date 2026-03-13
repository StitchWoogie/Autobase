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
	/// Summary description for ViewDigitalInputData.
	/// </summary>
	public class ViewDigitalInputData : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ContextMenu contextMenuDiData;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.IContainer components;		

		public string[] sTimeBuf;
		public string[] sButtonBuf;
		
		TagDiClass			di;
		public ArrayList	array;
		ArrayList			trendTagArr;

		public struct DI_DATA_VALUE
		{
			public bool		    bExist;
			public byte			val;
			public long		    ontime;
			public long		    oncount;
		};

		public struct DATA_DI_TAG
		{
			public bool		flag;
			public string	tag;			
			public Color	color;
			public  DI_DATA_VALUE[] val;
		};
		public DATA_DI_TAG		data_di_tag;

		public int xnum = 80;
		public int ynum = 29;
		public long	allcount;
		public long alltime;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour1;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour8;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour24;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour48;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour72;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour720;
		private System.Windows.Forms.MenuItem menuItem_data_same_min;
		private System.Windows.Forms.MenuItem menuItem_data_same_hour;
		private System.Windows.Forms.MenuItem menuItem_data_same_day;
		private System.Windows.Forms.MenuItem menuItem_data_same_week;
		private System.Windows.Forms.MenuItem menuItem_data_same_month;

		long				old_save_sec;

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem_di_curr_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;

		BasicScreen.kdymain.ButtonCheck2 hansolButton = new BasicScreen.kdymain.ButtonCheck2();

        
		public ViewDigitalInputData(Form parent, eDataTime data, ArrayList arr, Color backColor, int showMode)
            
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
			work.dataTime = data;

			sTimeBuf = new string[5];
			sButtonBuf = new string[2];
			if(Tools.IsLangKorean()) 
			{
				sTimeBuf[0] = "분";
				sTimeBuf[1] = "시간";
				sTimeBuf[2] = "일";
				sTimeBuf[3] = "주";
				sTimeBuf[4] = "월";
				sButtonBuf[0] = "보기변경[F4]";
				sButtonBuf[1] = "시간변경[F12]";
			}
			else if(Tools.IsLangJapanese()) 
			{
				sTimeBuf[0] = "分";
				sTimeBuf[1] = "時間";
				sTimeBuf[2] = "日";
				sTimeBuf[3] = "週";
				sTimeBuf[4] = "月";
				sButtonBuf[0] = "データ[F4]";
				sButtonBuf[1] = "時間変更[F12]";
			}
			else if(Tools.IsLangChinese()) 
			{
				sTimeBuf[0] = "分";
				sTimeBuf[1] = "时间";
				sTimeBuf[2] = "日";
				sTimeBuf[3] = "周";
				sTimeBuf[4] = "月";
				sButtonBuf[0] = "资料[F4]";
				sButtonBuf[1] = "更改时间[F12]";
			}
			else 
			{
				sTimeBuf[0] = "Min";
				sTimeBuf[1] = "Hour";
				sTimeBuf[2] = "Day";
				sTimeBuf[3] = "Week";
				sTimeBuf[4] = "Month";
				sButtonBuf[0] = "Show[F4]";
				sButtonBuf[1] = "Time[F12]";
			}

			work.dataDispType = (showMode >= 0 && showMode <= 2) ? (eDataDispType)showMode : eDataDispType.DECIMAL;

			work.backColor = backColor;
			work.eTagType = EnumTagType.DI;
			work.tagList = TagLib.GetTagList(work.eTagType);
			if(work.tagList == null) work.TagHap = 0;
			else				work.TagHap = work.tagList.Length;			
			
			DetailWindowSetSize(xnum, ynum);
			DiDataInitValueSetting();
			trendTagArr = arr;
			DiDataValueSetting();
			di = TagLib.GetStructDI(work.tagList[work.pos]);
			DiDataSetTitle();

            DiTrendRemainDataSave(); // 시작할 때도 남아있는 트랜드를 저장해 준다. 2010-12-21
		}

		public void DiDataInitValueSetting()
		{
            work.dt = DateTimeServer.Now;
			work.bDataReadFlag = false;
			getCurrentSec();
			work.Ix = 0;
			work.Iy = 0;
			work.mainTrendPos = 0;			// 표시할 시작 트랜드 single = 항상 0, multi 0 ~
			array = new ArrayList();
			work.trend_hap = 60;			// 최대 60개 자료 분 : 60, 시간 : 24 ...
		}
		
		public void DiDataValueSetting()
		{
			int 					i, pos;
			multiTrendTagStruct		multi = new multiTrendTagStruct();
			
			array.Clear();						// 어레이를 클리어, 			
			work.trend_tag_hap = trendTagArr.Count;
			
			for(i = 0; i < work.trend_tag_hap; i++) 
			{
				data_di_tag = new DATA_DI_TAG();
				multi = (multiTrendTagStruct)trendTagArr[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, multi.tag);
				if(pos <= 0) pos = 0;
				if(i == 0) work.pos = pos;				
				data_di_tag.tag = multi.tag;
				data_di_tag.color = multi.color;
				data_di_tag.flag = true;
				data_di_tag.val = new DI_DATA_VALUE[work.trend_hap];		// 최대 60개 분별 자료보기....
				array.Add(data_di_tag);
			}
		}
		
		void getCurrentSec()
		{
            DateTime dt = DateTimeServer.Now;
			old_save_sec = dt.Minute*60 + dt.Second;
		}

		async Task DiMultiDataValueRead()
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
				data_di_tag = (DATA_DI_TAG)array[i];
				await DataSetToBuf(data_di_tag, hap);
			}
			work.bDataReadFlag = true;
		}

		async Task DataSetToBuf(DATA_DI_TAG trend_one, int hap)
		{
			DataGate	gate = new DataGate();
			DataSet		ds;

			switch(work.dataTime)
			{
				case eDataTime.MIN : ds = await gate.GetDataDi(trend_one.tag, EnumDataType.CountOntime | EnumDataType.MOMENT, EnumDataTime.Minute, work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, hap, 1); break;
				case eDataTime.DAY : ds = await gate.GetDataDi(trend_one.tag, EnumDataType.CountOntime, EnumDataTime.Day, work.dt.Year, work.dt.Month, 1, 0, 0, hap, 1); break;
				case eDataTime.WEEK : ds = await gate.GetDataDi(trend_one.tag, EnumDataType.CountOntime, EnumDataTime.Week, work.dt.Year, 1, 1, 0, 0, hap, 1); break;
				case eDataTime.MONTH : ds = await gate.GetDataDi(trend_one.tag, EnumDataType.CountOntime, EnumDataTime.Month, work.dt.Year, 1, 1, 0, 0, hap, 1); break;
				default : ds = await gate.GetDataDi(trend_one.tag, EnumDataType.CountOntime, EnumDataTime.Hour, work.dt.Year, work.dt.Month, work.dt.Day, 0, 0, hap, 1); break;// eDataTime.HOUR :
			}			

			DataRow row;
			for(int i = 0; i < hap; i++) 
			{
				row = ds.Tables[0].Rows[i];
				trend_one.val[i].bExist = (ConvertTool.ToInt16(row[0].ToString()) == 1);
				if(work.dataTime == eDataTime.MIN)
				{
					trend_one.val[i].val = ConvertTool.ToByte(row["MOMENT"].ToString());	
					trend_one.val[i].ontime = ConvertTool.ToInt32(row["ONTIME"].ToString());	
					trend_one.val[i].oncount = ConvertTool.ToInt32(row["COUNT"].ToString());
				}
				else 
				{
					trend_one.val[i].val = 1;	
					trend_one.val[i].ontime = ConvertTool.ToInt32(row["ONTIME"].ToString());	
					trend_one.val[i].oncount = ConvertTool.ToInt32(row["COUNT"].ToString());
				}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputData));
            this.contextMenuDiData = new System.Windows.Forms.ContextMenu();
            this.menuItem_DiTrend_hour1 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour8 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour24 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour48 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour72 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour720 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_data_same_min = new System.Windows.Forms.MenuItem();
            this.menuItem_data_same_hour = new System.Windows.Forms.MenuItem();
            this.menuItem_data_same_day = new System.Windows.Forms.MenuItem();
            this.menuItem_data_same_week = new System.Windows.Forms.MenuItem();
            this.menuItem_data_same_month = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_di_curr_change = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // contextMenuDiData
            // 
            this.contextMenuDiData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_DiTrend_hour1,
            this.menuItem_DiTrend_hour8,
            this.menuItem_DiTrend_hour24,
            this.menuItem_DiTrend_hour48,
            this.menuItem_DiTrend_hour72,
            this.menuItem_DiTrend_hour720,
            this.menuItem2,
            this.menuItem_data_same_min,
            this.menuItem_data_same_hour,
            this.menuItem_data_same_day,
            this.menuItem_data_same_week,
            this.menuItem_data_same_month,
            this.menuItem1,
            this.menuItem_di_curr_change,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem4,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuDiData, "contextMenuDiData");
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
            // menuItem_data_same_min
            // 
            resources.ApplyResources(this.menuItem_data_same_min, "menuItem_data_same_min");
            this.menuItem_data_same_min.Index = 7;
            this.menuItem_data_same_min.Click += new System.EventHandler(this.menuItem_data_same_min_Click);
            // 
            // menuItem_data_same_hour
            // 
            resources.ApplyResources(this.menuItem_data_same_hour, "menuItem_data_same_hour");
            this.menuItem_data_same_hour.Index = 8;
            this.menuItem_data_same_hour.Click += new System.EventHandler(this.menuItem_data_same_hour_Click);
            // 
            // menuItem_data_same_day
            // 
            resources.ApplyResources(this.menuItem_data_same_day, "menuItem_data_same_day");
            this.menuItem_data_same_day.Index = 9;
            this.menuItem_data_same_day.Click += new System.EventHandler(this.menuItem_data_same_day_Click);
            // 
            // menuItem_data_same_week
            // 
            resources.ApplyResources(this.menuItem_data_same_week, "menuItem_data_same_week");
            this.menuItem_data_same_week.Index = 10;
            this.menuItem_data_same_week.Click += new System.EventHandler(this.menuItem_data_same_week_Click);
            // 
            // menuItem_data_same_month
            // 
            resources.ApplyResources(this.menuItem_data_same_month, "menuItem_data_same_month");
            this.menuItem_data_same_month.Index = 11;
            this.menuItem_data_same_month.Click += new System.EventHandler(this.menuItem_data_same_month_Click);
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
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 16;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 17;
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
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ViewDigitalInputData
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
            this.Name = "ViewDigitalInputData";
            this.Load += new System.EventHandler(this.ViewDigitalInputData_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputData_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewDigitalInputData_Paint);
            this.Closed += new System.EventHandler(this.ViewDigitalInputData_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputData_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewDigitalInputData_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion

		void DiDataSetTitle()
		{
			ViewDigitalInputDataMain	form = ViewDigitalInputDataMain.formThis;

			if(work.trend_tag_hap > 1) 
			{
				if(Tools.IsLangKorean()) form.Text = "디지털 멀티 자료보기";
				else if(Tools.IsLangChinese()) form.Text = "查看数字多资料";
				else form.Text = "Digital Multi Data View";
				return;
			}			
			
			if(Tools.IsLangKorean()) form.Text = string.Format("디지털 자료보기 - {0}", di.name);
			else if(Tools.IsLangChinese()) form.Text = string.Format("查看数字资料 - {0}", di.name);
			else form.Text = string.Format("Digital Data View - {0}", di.name);
		}

		//Bitmap bitmapClient = null;

		private void ViewDigitalInputData_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
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

			DiCountFirstScreen(g);
            //DiDiDataValueDraw(g);

            //if (_dataReady)
            //{
                DiDiDataValueDraw(g);
            //}
            //else
            //{
           //     g.DrawString("Loading...", SystemFonts.DefaultFont, Brushes.Gray, 10, 10);
           // }

            //OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

		void DiCountFirstScreen(Graphics g)
		{
			int						i, Ix, Iy, fontX, fontY;
			string					buf;
			StringFormat			format = new StringFormat();
			Ix = work.Ix;
			Iy = work.Iy;
			fontX = work.fontX;
			fontY = work.fontY;

			DrawClass.gcls(g, 0, 0, work.width, work.height, SharedData.colorTotal.BACK);
			DrawClass.PopBox2(g, Ix+fontX, Iy+(int)(fontY*0.75), Ix+fontX*79, Iy+(int)(fontY*28.5), work.grayColor);

			format.Alignment = StringAlignment.Center;
			buf = String.Format("{0}/{1}", work.pos+1, work.TagHap);
			DrawClass.WinDrawText(g, Ix+fontX*15,  Iy+fontY+1, fontX*13, fontY, buf, Color.Black, work.grayColor, work.f, format);
			if(di.bFileSave == 1) DrawClass.PushBox2(g, Ix+fontX*47, Iy+(int)(fontY*1.3), Ix+fontX*48, Iy+(int)(fontY*1.8), work.grayColor);
			DrawClass.PushBox2(g, Ix+fontX*28, Iy+fontY, Ix+fontX*46, Iy+fontY*2, Color.Blue);
			if(Tools.IsLangKorean()) 
				buf = String.Format("{0}별 자료보기", sTimeBuf[(int)work.dataTime]);
			else if(Tools.IsLangJapanese()) 
				buf = String.Format("{0}別 データ表示", sTimeBuf[(int)work.dataTime]);
			else if(Tools.IsLangChinese()) 
				buf = String.Format("{0} 查看资料", sTimeBuf[(int)work.dataTime]);
			else
				buf = String.Format("{0} Data", sTimeBuf[(int)work.dataTime]);

			DrawClass.WinDrawText(g, Ix+fontX*28, Iy+fontY+1, fontX*18, fontY, buf, Color.White, Color.Blue, work.f, format);	
			
			if(Tools.IsLangKorean()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*38, "태그:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*5.5), fontX*67, "설명:", di.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangJapanese()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*38, "タグ:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*5.5), fontX*67, "説明:", di.description, Color.Black, work.grayColor);
			}
			else if(Tools.IsLangChinese()) 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*38, "标记:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*5.5), fontX*67, "描述:", di.description, Color.Black, work.grayColor);
			}
			else 
			{
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*4), fontX*38, "Tag:", di.tag, Color.Black, work.grayColor);
				TagDescEtcDraw(g, Ix+(int)(fontX*1.5), Iy+(int)(fontY*5.5), fontX*67, "Desc:", di.description, Color.Black, work.grayColor);
			}

			DrawClass.PushBox2(g, Ix+fontX*22, Iy+(int)(fontY*2.5), Ix+fontX*48, Iy+(int)(fontY*3.5), work.grayColor);
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
			DrawClass.GrayDrawText(g, Ix+fontX*23, Iy+(int)(fontY*2.5)+1, fontX*24, fontY, buf, work.grayColor, work.f, format);

			for(i = 0; i < 2; i++) 
				hansolButton.ButtonCheckDraw2(g, Ix+fontX*(i*18+5), Iy+(int)(fontY*26.75),	Ix+fontX*(i*18+19), work.Iy+(int)(work.fontY*28.25), work.f, sButtonBuf[i], StringAlignment.Center);
			
		}

		void DiDiDataValueDraw(Graphics g)
		{
			//if(work.bDataReadFlag == false) //timer로 이동.
			//{
			//	Cursor = Cursors.WaitCursor;
			//	DiMultiDataValueRead();
			//}
			switch(work.dataTime) 
			{
				case eDataTime.MIN : MinDataScreenDraw(g); break;
				case eDataTime.HOUR : HourDataScreenDraw(g); break;
				case eDataTime.DAY : DayDataScreenDraw(g); break;
				case eDataTime.WEEK : WeekDataScreenDraw(g); break;
				case eDataTime.MONTH : MonthDataScreenDraw(g); break;
			}

			DiDataTotalValueScreenDraw(g);
			if(Cursor != Cursors.Arrow) Cursor = Cursors.Arrow;
			work.bChangeMinFlag = true;
			Cursor = Cursors.Arrow;		
		}

		void MakeTimebuf(ref string buf, long lTime)
		{
			if(Tools.IsLangKorean()) 
			{
				if(lTime < 60L) buf = String.Format("({0}초)", lTime);
				else if(lTime < 3600L) buf = String.Format("({0}분{1}초)", lTime/60L, lTime % 60L);				
				else if(lTime < 86400L) buf = String.Format("({0}시간 {1}분 {2}초)", lTime/3600L, lTime/60L%60L, lTime%60L);
				else buf = String.Format("({0}일 {1}시간 {2}분 {3}초)", lTime/86400L, lTime/3600L%60L, lTime/60L%60L, lTime%60L);
			}
			else if(Tools.IsLangJapanese()) 
			{
				if(lTime < 60L) buf = String.Format("({0}秒)", lTime);
				else if(lTime < 3600L) buf = String.Format("({0}分{1}秒)", lTime/60L, lTime % 60L);				
				else if(lTime < 86400L) buf = String.Format("({0}時間 {1}分 {2}秒)", lTime/3600L, lTime/60L%60L, lTime%60L);
				else buf = String.Format("({0}日 {1}時間 {2}分 {3}秒)", lTime/86400L, lTime/3600L%60L, lTime/60L%60L, lTime%60L);
			}
			else if(Tools.IsLangChinese()) 
			{
				if(lTime < 60L) buf = String.Format("({0}秒)", lTime);
				else if(lTime < 3600L) buf = String.Format("({0}分{1}秒)", lTime/60L, lTime % 60L);				
				else if(lTime < 86400L) buf = String.Format("({0}时间 {1}分 {2}秒)", lTime/3600L, lTime/60L%60L, lTime%60L);
				else buf = String.Format("({0}日 {1}时间 {2}分 {3}秒)", lTime/86400L, lTime/3600L%60L, lTime/60L%60L, lTime%60L);
			}
			else 
			{
				if(lTime < 60L) buf = String.Format("({0}S)", lTime);
				else if(lTime < 3600L) buf = String.Format("({0}:{1})", lTime/60L, lTime % 60L);				
				else if(lTime < 86400L) buf = String.Format("({0}:{1}:{2})", lTime/3600L, lTime/60L%60L, lTime%60L);
				else buf = String.Format("({0}Day {1}:{2}:{3})", lTime/86400L, lTime/3600L%60L, lTime/60L%60L, lTime%60L);
			}		
		}

		void DiDataTotalValueScreenDraw(Graphics g)
		{
			int 					y, Ix = work.Ix, Iy = work.Iy;
			string[]				title = new string[5];
			string					buf, onBuf;
			StringFormat			format = new StringFormat();

			if(Tools.IsLangKorean()) 
			{
				title[0] = "시간당";
				title[1] = "일간";
				title[2] = "월간";
				title[3] = "연간";
				title[4] =  "연간";
			}
			else if(Tools.IsLangJapanese()) 
			{
				title[0] = "時間";
				title[1] = "日間";
				title[2] = "月間";
				title[3] = "年間";
				title[4] =  "年間";
			}
			else if(Tools.IsLangChinese()) 
			{
				title[0] = "一小时";
				title[1] = "一天";
				title[2] = "一个月";
				title[3] = "一年";
				title[4] =  "一年";
			}
			else 
			{
				title[0] = "Hour";
				title[1] = "Day";
				title[2] = "Month";
				title[3] = "Year";
				title[4] =  "Year";
			}
			format.Alignment = StringAlignment.Center;
			if(di.desON.Length > 0) onBuf = String.Format("{0}", di.desON);
			else					onBuf = String.Format("ON");
			y = Iy+work.fontY;
			DrawClass.PushBox2(g, Ix+work.fontX*50, y, Ix+work.fontX*78, y+work.fontY*4, work.grayColor);

			y += work.fontY/2;
			if(Tools.IsLangKorean())       
				buf = String.Format("{0} {1}된 횟수:{2}회", title[(int)work.dataTime], onBuf, allcount);
			else if(Tools.IsLangJapanese())       
				buf = String.Format("{0} {1} 回数:{2}回", title[(int)work.dataTime], onBuf, allcount);
			else if(Tools.IsLangChinese())       
				buf = String.Format("{0}的 {1} 次数:{2}回", title[(int)work.dataTime], onBuf, allcount);
			else
				buf = String.Format("{0} {1} Count:{2}", title[(int)work.dataTime], onBuf, allcount);

			DrawClass.GrayDrawText(g, Ix+work.fontX*50, y, work.fontX*28, work.fontY, buf, work.grayColor, work.f, format);	
			
			y += (int)(work.fontY*1.5);    

			if(Tools.IsLangKorean())
				buf = String.Format("{0} {1}된 시간:{2}초", title[(int)work.dataTime], onBuf, alltime);
			else if(Tools.IsLangJapanese())
				buf = String.Format("{0} {1} 時間:{2}秒", title[(int)work.dataTime], onBuf, alltime);
			else if(Tools.IsLangChinese())
				buf = String.Format("{0} {1} 时间:{2}秒", title[(int)work.dataTime], onBuf, alltime);
			else
				buf = String.Format("{0} {1} Time:{2}S", title[(int)work.dataTime], onBuf, alltime);

			DrawClass.GrayDrawText(g, Ix+work.fontX*50, y, work.fontX*28, work.fontY, buf, work.grayColor, work.f, format);
			y += work.fontY;
			MakeTimebuf(ref buf, alltime);
			DrawClass.GrayDrawText(g, Ix+work.fontX*50, y, work.fontX*28, work.fontY, buf, work.grayColor, work.f, format);	
		}

		void MinDataScreenDraw(Graphics g)
		{
			int 					i, y, size, Ix, Iy;			
			string 					buf;
			StringFormat			format = new StringFormat();
			Color					iBcolor;

			format.Alignment = StringAlignment.Center;
			Ix = work.Ix;
			Iy = work.Iy;
			y = Iy+(int)(work.fontY*7.5);
			size = (int)(work.fontY*1.15);

			DiDataStringDraw(g, y, size, work.fontX*18, work.fontX*4, 4, 15, 60);
            iBcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.White : Color.Black;

			for(i = 0; i < 4; i++) 
			{
				if(Tools.IsLangKorean())
				{
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*8)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "상태", Color.Green, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*12.5)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "횟수", Color.Cyan, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*17)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "시간", Color.Magenta, iBcolor, work.f, format);	
				}
				else if(Tools.IsLangJapanese())
				{
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*8)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "状態", Color.Green, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*12.5)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "回数", Color.Cyan, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*17)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "時間", Color.Magenta, iBcolor, work.f, format);	
				}
				else if(Tools.IsLangChinese())
				{
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*8)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "状态", Color.Green, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*12.5)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "次数", Color.Cyan, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*17)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "时间", Color.Magenta, iBcolor, work.f, format);	
				}
				else 
				{
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*8)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "Status", Color.Green, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*12.5)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "Count", Color.Cyan, iBcolor, work.f, format);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*17)+i*work.fontX*18, y, (int)(work.fontX*6), work.fontY, "Time", Color.Magenta, iBcolor, work.f, format);	
				}
			}

			allcount = 0L;
			alltime = 0L;
			y += (int)(size*1.15)+1;			
			data_di_tag = (DATA_DI_TAG)array[0];
			for(i = 0; i < 60; i++) 
			{
				if(data_di_tag.val[i].bExist == true) 
				{
					allcount += data_di_tag.val[i].oncount;
					alltime += data_di_tag.val[i].ontime;

					if(data_di_tag.val[i].val == 1) 
					{
						if(di.desON.Length <= 0) DrawClass.WinDrawText(g, Ix+(int)(work.fontX*8)+(i/15)*work.fontX*18, y+(i%15)*size, (int)(work.fontX*4.5), work.fontY, "ON", Color.Red, iBcolor, work.f, format);
						else					 DrawClass.WinDrawText(g, Ix+(int)(work.fontX*8)+(i/15)*work.fontX*18, y+(i%15)*size, (int)(work.fontX*4.5), work.fontY, di.desON, Color.Red, iBcolor, work.f, format);
					}
					
					else 
					{
						if(di.desOFF.Length <= 0) DrawClass.WinDrawText(g, Ix+(int)(work.fontX*9)+(i/15)*work.fontX*18, y+(i%15)*size, (int)(work.fontX*4.5), work.fontY, "OFF", Color.Blue, iBcolor, work.f, format);
						else					  DrawClass.WinDrawText(g, Ix+(int)(work.fontX*9)+(i/15)*work.fontX*18, y+(i%15)*size, (int)(work.fontX*4.5), work.fontY, di.desOFF, Color.Blue, iBcolor, work.f, format);
					}
	
					buf = String.Format("{0,2}", data_di_tag.val[i].oncount);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*12.5)+(i/15)*work.fontX*18, y+(i%15)*size, (int)(work.fontX*4.5), work.fontY, buf, Color.Cyan, iBcolor, work.f, format);
					if(Tools.IsLangKorean())
						buf = String.Format("{0, 2}초", data_di_tag.val[i].ontime);
					else if(Tools.IsLangJapanese())
						buf = String.Format("{0, 2}秒", data_di_tag.val[i].ontime);
					else if(Tools.IsLangChinese())
						buf = String.Format("{0, 2}秒", data_di_tag.val[i].ontime);
					else
						buf = String.Format("{0, 2}", data_di_tag.val[i].ontime);
					DrawClass.WinDrawText(g, Ix+(int)(work.fontX*17)+(i/15)*work.fontX*18, y+(i%15)*size, (int)(work.fontX*5), work.fontY, buf, Color.Magenta, iBcolor, work.f, format);
				}
				else
					DrawClass.WinDrawText(g, Ix+work.fontX*8+(i/15)*work.fontX*18, y+(i%15)*size-3, work.fontX*14, work.fontY, "..", work.grayColor, iBcolor, work.f, format);
			}
		}

		void HourDataScreenDraw(Graphics g)
		{
			int i;

			allcount = 0L;
			alltime = 0L;
			for(i = 0; i < 24; i++) 
			{
				//if(work.bDataReadFlag == false) DiCalcOneHourData(i);
				allcount += data_di_tag.val[i].oncount;
				alltime += data_di_tag.val[i].ontime;
			}
			DiDataStringDraw(g, work.Iy+(int)(work.fontY*7.5), (int)(work.fontY*1.45), work.fontX*36, work.fontX*13, 2, 12, 24);
		}

		void DayDataScreenDraw(Graphics g)
		{
			int 		i, limit;
			
			allcount = 0L;
			alltime = 0L;
			limit = TimeUtil.getmonthlimit(work.dt.Year, work.dt.Month);
			for(i = 0; i < limit; i++) 
			{
				//if(work.bDataReadFlag == false) DiCalcOneDayData(i);
				if(data_di_tag.val[i].bExist == true) 
				{
					allcount += data_di_tag.val[i].oncount;
					alltime += data_di_tag.val[i].ontime;
				}
			}
			DiDataStringDraw(g, work.Iy+(int)(work.fontY*8), (int)(work.fontY*1.5), work.fontX*24, work.fontX*4, 3, 11, limit);
		}

		int getmonth(int year, int allday)
		{
			int i, j, hap = allday;

			if(allday > 366) return 12;          // month error

			for(i = 1; i <= 12; i++) 
			{
				j = TimeUtil.getmonthlimit(year, i);
				if(j >= hap) return i;
				hap -= j;
			}
			return 12;
		}

		int getday(int year, int allday)
		{
			int i, j, hap = allday;

			if(allday > 366) return 31;          // month error

			for(i = 1, j = 0; i <= 12; i++) 
			{
				j = TimeUtil.getmonthlimit(year, i);
				if(j >= hap) return hap;
				hap -= j;
			}
			return 31;
		}

		void WeekDataScreenDraw(Graphics g)
		{
			int	 				i, y, size,	yo, ju;
			int 				emonth, eday, day;
			string				buf;
			Color				iTcolor, iBcolor;
			StringFormat		format = new StringFormat();			

			yo = TimeUtil.GetWeekDay(work.dt.Year, 1, 1);
			i = TimeUtil.getmonthlimit(work.dt.Year, 2);
			if(i == 28) ju = (365+12-yo)/7;
			else        ju = (366+12-yo)/7;

			allcount = 0L;
			alltime = 0L;
			for(i = 0; i < 56; i++) 
			{
				if(data_di_tag.val[i].bExist == true) 
				{
					allcount += data_di_tag.val[i].oncount;
					alltime += data_di_tag.val[i].ontime;				
				}
			}

			y = work.Iy+(int)(work.fontY*7.5);
			size = (int)(work.fontY*1.25);
			DiDataStringDraw(g, y, size, work.fontX*18, work.fontX*7, 4, 14, ju);

            iBcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.White : Color.Black;
            iTcolor = (ConfigViewMain.bBasciScreenDataViewWhiteBackground == true) ? Color.Black : Color.White;
			y += (int)(size*1.15)+1;
			format.Alignment = StringAlignment.Center;
			for(i = 0, day = 7-yo; i < 56; i++) 
			{
				if(i >= ju) break;
				if(i != 0) day += 7;
				emonth = getmonth(work.dt.Year, day);
				eday = getday(work.dt.Year, day);
				buf = String.Format("-{0,2:d2}/{1,2:d2}", emonth, eday);
				DrawClass.WinDrawText(g, work.Ix+(int)(work.fontX*3)+(i/14)*work.fontX*18, y+(i%14)*size, work.fontX*9, work.fontY, buf, iTcolor, iBcolor, work.f, format);
			}
		}

		void MonthDataScreenDraw(Graphics g)
		{
			int 	i;

			for(i = 0; i < 12; i++) 
			{
				//if(work.bDataReadFlag == false) DiCalcOneMonthData(i);
				allcount += data_di_tag.val[i].oncount;
				alltime += data_di_tag.val[i].ontime;				
			}
			DiDataStringDraw(g, work.Iy+(int)(work.fontY*8), (int)(work.fontY*1.4), work.fontX*72, work.fontX*28, 1, 12, 12);
		}

		void DiDataStringDraw(Graphics g, int sy, int size, int width, int width2, int num, int line, int limit)
		{
			int	 					i, x, y;
			int	 					Ix, fontX;
			string 					buf;
			Color					iBcolor, iTcolor;
			StringFormat			format = new StringFormat();			
			
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
				DrawClass.WinDrawText(g, x+i*width, y, width2, work.fontY, sTimeBuf[(int)work.dataTime], Color.Green, iBcolor, work.f, format);//DT_CENTER);

				DrawClass.gline(g, x+width2+i*width, y-(int)(size*0.2), x+width2+i*width, y+size*(line+1), Color.DarkGray);
				if(i != 0) DrawClass.gline(g, x+i*width, y-(int)(size*0.2), x+i*width, y+size*(line+1), Color.DarkGray);

				if(work.dataTime == eDataTime.MIN) continue;

				if(Tools.IsLangKorean())
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("횟수");
					else								buf = String.Format("{0}횟수", di.desON);
				}
				else if(Tools.IsLangJapanese())
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("回数");
					else								buf = String.Format("{0}回数", di.desON);
				}
				else if(Tools.IsLangChinese())
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("次数");
					else								buf = String.Format("{0}次数", di.desON);
				}
				else 
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("Count");
					else								buf = String.Format("{0} Count", di.desON);
				}
				DrawClass.WinDrawText(g, x+width2+i*width, y, (width-width2)/2, work.fontY, buf, Color.Green, iBcolor, work.f, format);
				if(Tools.IsLangKorean()) 
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("시간");
					else								buf = String.Format("{0}시간", di.desON);
				}
				else if(Tools.IsLangJapanese()) 
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("時間");
					else								buf = String.Format("{0}時間", di.desON);
				}
				else if(Tools.IsLangChinese()) 
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("时间");
					else								buf = String.Format("{0}时间", di.desON);
				}
				else 
				{
					if(work.dataTime == eDataTime.WEEK) buf = String.Format("Time");
					else								buf = String.Format("{0} Time", di.desON);
				}
				DrawClass.WinDrawText(g, x+width2+(width-width2)/2+i*width, y, (width-width2)/2, work.fontY, buf, Color.Green, iBcolor, work.f, format);
			}
			for(i = 2; i <= line; i++)	DrawClass.gline(g, x, y+size*i, Ix+fontX*76-1, y+size*i, Color.DarkGray);

			data_di_tag = (DATA_DI_TAG)array[0];

			y += size+(int)(size*0.15)+1;
			for(i = 0; i < limit; i++) 
			{
				switch(work.dataTime) 
				{
					case eDataTime.MIN : buf = String.Format("{0,2:d2}", i);	break;
					case eDataTime.HOUR : buf = String.Format("{0,2:d2}:00-{1,2:d2}:59", i, i);break;					
					case eDataTime.DAY : buf = String.Format("{0,2:d2}", i+1);	break;
					default : buf = String.Format("{0,2:d2}{1}", i+1, sTimeBuf[(int)work.dataTime]); break;
				}		
				if(work.dataTime != eDataTime.WEEK) DrawClass.WinDrawText(g, x+(i/line)*width, y+(i%line)*size, width2, work.fontY, buf, iTcolor, iBcolor, work.f, format);	
				if(work.dataTime == eDataTime.MIN) continue;

				if(data_di_tag.val[i].bExist == true) 
				{
					buf = String.Format("{0}", data_di_tag.val[i].oncount);
					DrawClass.WinDrawText(g, x+width2+(i/line)*width, y+(i%line)*size, (width-width2)/2, work.fontY, buf, Color.Cyan, iBcolor, work.f, format);
	
					if(work.dataTime == eDataTime.WEEK) 
					{
						if(Tools.IsLangKorean())
							buf = String.Format("{0}초", data_di_tag.val[i].ontime);
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}秒", data_di_tag.val[i].ontime);
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}秒", data_di_tag.val[i].ontime);
						else
							buf = String.Format("{0}S", data_di_tag.val[i].ontime);
					}
					else if(data_di_tag.val[i].ontime < 60L) 
					{
						if(Tools.IsLangKorean())
							buf = String.Format( "{0}초", data_di_tag.val[i].ontime);
						else if(Tools.IsLangJapanese())
							buf = String.Format( "{0}秒", data_di_tag.val[i].ontime);
						else if(Tools.IsLangChinese())
							buf = String.Format( "{0}秒", data_di_tag.val[i].ontime);
						else
							buf = String.Format( "{0}S", data_di_tag.val[i].ontime);
					}
					else if(data_di_tag.val[i].ontime < 3600L) 
					{
						if(Tools.IsLangKorean())
							buf = String.Format("{0}분{1,2:d2}초", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
						else if(Tools.IsLangJapanese())
							buf = String.Format("{0}分{1,2:d2}秒", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
						else if(Tools.IsLangChinese())
							buf = String.Format("{0}分{1,2:d2}秒", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
						else
							buf = String.Format("{0}:{1,2:d2}", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
					}
					else 
					{
						if(work.dataTime == eDataTime.HOUR)	
						{
							if(Tools.IsLangKorean())
								buf = String.Format("{0}분{1,2:d2}초", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
							else if(Tools.IsLangJapanese())
								buf = String.Format("{0}分{1,2:d2}秒", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
							else if(Tools.IsLangChinese())
								buf = String.Format("{0}分{1,2:d2}秒", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
							else
								buf = String.Format("{0}:{1,2:d2}", data_di_tag.val[i].ontime/60L, data_di_tag.val[i].ontime % 60L);
						}
						else if(work.dataTime == eDataTime.DAY)	
						{
							if(Tools.IsLangKorean())
								buf = String.Format("{0}시{1,2:d2}분{2,2:d2}", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
							else if(Tools.IsLangJapanese())
								buf = String.Format("{0}時{1,2:d2}分{2,2:d2}", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
							else if(Tools.IsLangChinese())
								buf = String.Format("{0}时{1,2:d2}分{2,2:d2}", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
							else
								buf = String.Format("{0}:{1,2:d2}:{2,2:d2}", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
						}
						else 
						{
							if(Tools.IsLangKorean())
								buf = String.Format("{0}시간 {1,2:d2}분 {2,2:d2}초", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
							else if(Tools.IsLangJapanese())
								buf = String.Format("{0}時間 {1,2:d2}分 {2,2:d2}秒", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
							else if(Tools.IsLangChinese())
								buf = String.Format("{0}时间 {1,2:d2}分 {2,2:d2}秒", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
							else
								buf = String.Format("{0} : {1,2:d2} : {2,2:d2}", data_di_tag.val[i].ontime/3600L, (data_di_tag.val[i].ontime/60L)%60L, data_di_tag.val[i].ontime%60L);
						}
					}
					DrawClass.WinDrawText(g, x+width2+(width-width2)/2+(i/line)*width, y+(i%line)*size, (width-width2)/2, work.fontY, buf, Color.Magenta, iBcolor, work.f, format);
				}
				else
					DrawClass.WinDrawText(g, x+width2+(i/line)*width, y+(i%line)*size, width-width2, work.fontY, "..", work.grayColor, iBcolor, work.f, format);
			}
		}

		void DiTrendRemainDataSave()
		{
			TagDiClass					tag;
			DATA_DI_TAG					iTrend_tag;
			for(int pos, i = 0; i < array.Count; i++) 
			{
				iTrend_tag = (DATA_DI_TAG)array[i];
				pos = TagLib.GetTagPosOnlyList(work.tagList, iTrend_tag.tag);
				if(pos < 0) continue;
				tag = TagLib.GetStructDI(work.tagList[pos]);
				if(tag == null) continue;
				SharedViewMain.SaveTrendRemainDI(tag);	// 남아있는 자료저장 자료간격이 2분 이상일 때...
			}
		}
		
		public void changeDiDataTime(eDataTime dataTime)
		{
			if(work.dataTime == dataTime) return;
			work.bDataReadFlag = false;
			work.dataTime = dataTime;
			this.Invalidate();
		}

		
		private void menuItem_data_same_min_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(eDataTime.MIN);
		}

		private void menuItem_data_same_hour_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(eDataTime.HOUR);		
		}

		private void menuItem_data_same_day_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(eDataTime.DAY);		
		}

		private void menuItem_data_same_week_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(eDataTime.WEEK);		
		}

		private void menuItem_data_same_month_Click(object sender, System.EventArgs e)
		{
			changeDiDataTime(eDataTime.MONTH);		
		}

		void DiDataButtonLeftMouseCheck(MouseEventArgs e)
		{
			int		i;

			for(i = 0; i < 2; i++)
				if(hansolButton.ButtonCheckDown2(this, e, work.Ix+work.fontX*(i*18+5), work.Iy+(int)(work.fontY*26.75),	work.Ix+work.fontX*(i*18+19), work.Iy+(int)(work.fontY*28.25), work.f, sButtonBuf[i], i+1)) return;
			
		}

		private void ViewDigitalInputData_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    this.contextMenuDiData.Show(control, pos);
                    return;
                }
            }

			if(e.Button == MouseButtons.Left) 
			{
				DiDataButtonLeftMouseCheck(e);
				return;
			}
		}

		private void ViewDigitalInputData_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hansolButton.ButtonCheckMove2(this, e);		
		}

		private void ViewDigitalInputData_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int			id;
			
			id = hansolButton.ButtonCheckUp2(this);
			if(id == -1) return;

			switch(id) 
			{
				case 1 : callShowTimeChangeFunc(); return;
				case 2 : callTimeSettingDialog(); return;			
			}
		
		}


		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewDigitalInputData_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged += new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
			SharedViewMain.EventListHourChanged += new SharedViewMain.OnEventHourChanged(OnHourChanged);
		}

		private void ViewDigitalInputData_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListMinuteChanged -= new SharedViewMain.OnEventMinuteChanged(OnMinuteChanged);
			SharedViewMain.EventListHourChanged -= new SharedViewMain.OnEventHourChanged(OnHourChanged);			
		}		

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
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
			DiTrendRemainDataSave();	// 남아있는 자료저장 자료간격이 2분 이상일 때...
			work.bDataReadFlag = false;
			this.Invalidate();
            await Task.CompletedTask;
        }

		private void OnHourChanged()
		{
			if(work.dataTime == eDataTime.MIN) return;		// 분 자료보기

			work.bDataReadFlag = false;
			this.Invalidate();			
		}

		void DiOneDataViewTagChangeSetting()
		{
			if(trendTagArr == null || trendTagArr.Count <= 0) return;

			multiTrendTagStruct		trendTag = (multiTrendTagStruct)trendTagArr[0];
			di = TagLib.GetStructDI(work.tagList[work.pos]);

			trendTag.tag = di.name;
			trendTagArr[0] = trendTag;
			DiDataValueSetting();
			this.Invalidate();
		}

		private async void timer1_Tick(object sender, System.EventArgs e)
		{
			if(work.bTagChanged && work.trend_tag_hap == 1) 
			{
				DiOneDataViewTagChangeSetting();
				DiDataSetTitle();
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
			if (_isLoading) return;  // 중복 방지
			_isLoading = true;

			try
			{
				await DiMultiDataValueRead();  // async 데이터 읽기
				this.Invalidate();             // 읽기 끝나면 다시 그리기
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

        private void menuItem3_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }

		
	}
}
