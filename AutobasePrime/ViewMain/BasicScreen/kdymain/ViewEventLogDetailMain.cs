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
using System.Drawing.Printing;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewEventLogDetailMain.
	/// </summary>
	public class ViewEventLogDetailMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu contextMenuEventLogDetail;
		private System.Windows.Forms.MenuItem menuItem3;

		//BasicScreen.kdymain.ViewEventLogDetail child;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.MenuItem menuItem_PringLogData;
		private System.Windows.Forms.MenuItem menuItem2;

		DataSet					ds;
		int[]					headWidth = new int[6];
		ControlListView			list = new ControlListView();
		//int			TagHap, currPos;
		string					logFileName;
		int						nPrintListPage, nPrintListPos;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;

        // 필터링 변수
        private LogLevel? filterLevel = null;
        private MenuItem menuItem_FilterByLevel;
        private MenuItem menuItem_FilterByCategory;
        private MenuItem menuItem__ViewDetail;
        private Label labelFilter;
        private ComboBox comboBoxLevel;
        private ComboBox comboBoxCategory;
        private Panel panel3;
        private Panel panel4;
        private int? filterCategory = null;

        public ViewEventLogDetailMain(string log, int pos)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.panel2.Controls.Add(list);
			basicElementSetting();

			logFileName = log;
			if(log.Length >= 8) 
			{
				//if(Tools.IsLangKorean())
				//	this.Text = String.Format("{0}{1}{2}{3}년 {4}{5}월 {6}{7}일 로그 내용보기", log[0], log[1], log[2], log[3], log[4], log[5], log[6], log[7]);
				//else if(Tools.IsLangJapanese())
				//	this.Text = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 ログの詳細表示", log[0], log[1], log[2], log[3], log[4], log[5], log[6], log[7]);
				//else if(Tools.IsLangChinese())
				//	this.Text = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 查看日志内容", log[0], log[1], log[2], log[3], log[4], log[5], log[6], log[7]);
				//else
				//	this.Text = String.Format("{0}{1}{2}{3}/{4}{5}/{6}{7} LOG Data View", log[0], log[1], log[2], log[3], log[4], log[5], log[6], log[7]);
				this.Text = $"{log} LogView";
			}

			DataGate gate = new DataGate();
			this.Load += async (sender, e) => { 
				
				ds = await gate.GetLogFile(log);
                fillListDataAll();

            } ;
			if(ds == null) list.listHap = 0;
			else		   list.listHap = ds.Tables[0].Rows.Count;
			list.currPos = pos;
			if(list.currPos < 0 || list.currPos >= list.listHap) list.currPos = list.listHap-1;

            // 컬럼 너비 설정 (순서, 시간, 레벨, 카테고리, 사용자, 메시지)
            headWidth[0] = 50;   // 순서
            headWidth[1] = 150;  // 시간
            headWidth[2] = 80;   // 레벨
            headWidth[3] = 100;  // 카테고리
            headWidth[4] = 80;   // 사용자
            headWidth[5] = 400;  // 메시지
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewEventLogDetailMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.contextMenuEventLogDetail = new System.Windows.Forms.ContextMenu();
            this.menuItem_PringLogData = new System.Windows.Forms.MenuItem();
            this.menuItem_FilterByLevel = new System.Windows.Forms.MenuItem();
            this.menuItem_FilterByCategory = new System.Windows.Forms.MenuItem();
            this.menuItem__ViewDetail = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBoxLevel = new System.Windows.Forms.ComboBox();
            this.labelFilter = new System.Windows.Forms.Label();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2});
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.Divider = false;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            resources.ApplyResources(this.toolBarButton1, "toolBarButton1");
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            resources.ApplyResources(this.toolBarButton2, "toolBarButton2");
            // 
            // contextMenuEventLogDetail
            // 
            this.contextMenuEventLogDetail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_PringLogData,
            this.menuItem_FilterByLevel,
            this.menuItem_FilterByCategory,
            this.menuItem__ViewDetail,
            this.menuItem2,
            this.menuItem3});
            // 
            // menuItem_PringLogData
            // 
            this.menuItem_PringLogData.Index = 0;
            resources.ApplyResources(this.menuItem_PringLogData, "menuItem_PringLogData");
            this.menuItem_PringLogData.Click += new System.EventHandler(this.menuItem_PringLogData_Click);
            // 
            // menuItem_FilterByLevel
            // 
            this.menuItem_FilterByLevel.Index = 1;
            resources.ApplyResources(this.menuItem_FilterByLevel, "menuItem_FilterByLevel");
            this.menuItem_FilterByLevel.Click += new System.EventHandler(this.menuItem_FilterByLevel_Click);
            // 
            // menuItem_FilterByCategory
            // 
            this.menuItem_FilterByCategory.Index = 2;
            resources.ApplyResources(this.menuItem_FilterByCategory, "menuItem_FilterByCategory");
            this.menuItem_FilterByCategory.Click += new System.EventHandler(this.menuItem_FilterByCategory_Click);
            // 
            // menuItem__ViewDetail
            // 
            this.menuItem__ViewDetail.Index = 3;
            resources.ApplyResources(this.menuItem__ViewDetail, "menuItem__ViewDetail");
            this.menuItem__ViewDetail.Click += new System.EventHandler(this.menuItem__ViewDetail_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 4;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 5;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel4);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.Controls.Add(this.comboBoxLevel);
            this.panel3.Controls.Add(this.labelFilter);
            this.panel3.Controls.Add(this.comboBoxCategory);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // comboBoxLevel
            // 
            this.comboBoxLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLevel.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxLevel, "comboBoxLevel");
            this.comboBoxLevel.Name = "comboBoxLevel";
            this.comboBoxLevel.SelectedIndexChanged += new System.EventHandler(this.comboBoxLevel_SelectedIndexChanged);
            // 
            // labelFilter
            // 
            resources.ApplyResources(this.labelFilter, "labelFilter");
            this.labelFilter.Name = "labelFilter";
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCategory.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCategory, "comboBoxCategory");
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.SelectedIndexChanged += new System.EventHandler(this.comboBoxCategory_SelectedIndexChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.toolBar1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // ViewEventLogDetailMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "ViewEventLogDetailMain";
            this.Closed += new System.EventHandler(this.ViewEventLogDetailMain_Closed);
            this.Load += new System.EventHandler(this.ViewEventLogDetailMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewEventLogDetailMain_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewEventLogDetailMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		

		private void OnMainFontChanged()
		{
			list.font = ConfigViewMain.fontMain;
			list.fontChanged();
			//this.listView1.Font = ConfigViewMain.fontMain;
			//this.listView1.Refresh();		// font 가 바뀌면 새로 그려야 한다
		}

		void OnColorChanged()
		{
			this.BackColor = SharedData.colorTotal.BACK;				// 기본화면의 배경색상, list visable =
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용
			list.dataAreaInvalidate();
			//this.listView1.BackColor = SharedData.colorTotal.BACK;
		}

		void OnUserChanged()
		{
			//setDetailButtonEnableDisable();
		}

		void onListMouse_DoubleClicked()
		{
            // 더블클릭 시 상세 정보 표시
            ShowLogDetail();
        }

		void onSelectedIndexChanged()
		{
			//setDetailButtonEnableDisable();
		}
		
		void oneLineDraw(Graphics g, DataRow row, int i, int x, int y, int xGap)
		{
            Color color = SharedData.colorTotal.TEXT;
            ControlListViewHeader head;

            // 순서
            head = (ControlListViewHeader)list.header[0];
            DrawClass.WinDrawText(g, x + xGap, y, head.width - xGap * 2, (int)list.fontY,
                i.ToString(), color, list.backColor, list.font, head.format);
            x += head.width;

            // 시간
            head = (ControlListViewHeader)list.header[1];
            DateTime datetime = (DateTime)row["log_datetime"];
            //string data = datetime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            string data = datetime.ToString("yyyy-MM-dd HH:mm:ss"); //이미 localtime.
            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY,
                data, color, list.backColor, list.font, head.format);
            x += head.width;

            // 레벨
            head = (ControlListViewHeader)list.header[2];
            string levelStr = row["Level"].ToString();
            Color levelColor = GetLevelColor(levelStr);
            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY,
                levelStr, levelColor, list.backColor, list.font, head.format);
            x += head.width;

            // 카테고리
            head = (ControlListViewHeader)list.header[3];
            string categoryStr = row["Category"].ToString();
            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY,
                categoryStr, color, list.backColor, list.font, head.format);
            x += head.width;

            // 사용자
            head = (ControlListViewHeader)list.header[4];
            string username = row["Username"].ToString();
            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY,
                username, color, list.backColor, list.font, head.format);
            x += head.width;

            // 메시지
            head = (ControlListViewHeader)list.header[5];
            string message = row["Message"].ToString();
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY,
                message, color, list.backColor, list.font, format);
        }


        Color GetLevelColor(string level)
        {
            switch (level)
            {
                case "DEBUG":
                    return Color.Gray;
                case "INFO":
                    return SharedData.colorTotal.TEXT;
                case "WARNING":
                    return Color.Orange;
                case "ERROR":
                    return Color.Red;
                case "CRITICAL":
                    return Color.DarkRed;
                case "FATAL":
                    return Color.Purple;
                default:
                    return SharedData.colorTotal.TEXT;
            }
        }

        void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);

			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;			
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
            DataRowView rowView;

            endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;

            // filteredDataView 사용하도록 변경
            DataView dataView = filteredDataView ?? ds.Tables[0].DefaultView;

            for (pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;

                //  DataView에서 가져오기
                if (pos >= dataView.Count) continue;
                rowView = dataView[pos];
                if (rowView == null) continue;

                x = list.startX;
				oneLineDraw(g, rowView.Row, pos +1, x, y, xGap);
			}
		}

		void onPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();
		
		private void ViewEventLogDetailMain_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged += new SharedViewMain.DelegatePublic(OnColorChanged);
		
			//TotalConfig.AutoBaseListCtrlConfigLoad(this.listView1, "BasicScreen", "ViewEventLogDetail");
			//fillListView(currPos);
			list.doubleClick += new ControlListView.OnEventDoubleClick(onListMouse_DoubleClicked);
			list.selectedIndexChanged += new ControlListView.OnEventSelectedIndexChanged(onSelectedIndexChanged);
			list.paintMessage += new ControlListView.OnEventPaintMessage(onPaintMessage);
			            			
			TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "ViewEventLogDetail");
			
			list.bOwnerDraw = true;
			listHeaderFill();
			list.Show();
			fillListDataAll();

            // 필터 콤보박스 초기화
            InitializeFilterComboBoxes();

            this.panel1.Height = this.toolBar1.Height;			// 초기크기를 설정
			ringForm.push(this);
		}

        void InitializeFilterComboBoxes()
        {
            // 레벨 필터
            comboBoxLevel.Items.Add("전체");
            comboBoxLevel.Items.Add("DEBUG");
            comboBoxLevel.Items.Add("INFO");
            comboBoxLevel.Items.Add("WARNING");
            comboBoxLevel.Items.Add("ERROR");
            comboBoxLevel.Items.Add("CRITICAL");
            comboBoxLevel.Items.Add("FATAL");
            comboBoxLevel.SelectedIndex = 0;

            // 카테고리 필터
            comboBoxCategory.Items.Add("전체");
            comboBoxCategory.Items.Add("시스템");
            comboBoxCategory.Items.Add("보안");
            comboBoxCategory.Items.Add("데이터");
            comboBoxCategory.Items.Add("태그");
            comboBoxCategory.Items.Add("통신");
            comboBoxCategory.Items.Add("경보");
            comboBoxCategory.SelectedIndex = 0;
        }


        void listHeaderFill()
		{
			ControlListViewHeader	head;
			string[] text = new string[6];

            if (Tools.IsLangKorean())
            {
                text[0] = "순서";
                text[1] = "시간";
                text[2] = "레벨";
                text[3] = "카테고리";
                text[4] = "사용자";
                text[5] = "메시지";
            }
            else if (Tools.IsLangJapanese())
            {
                text[0] = "番号";
                text[1] = "時刻";
                text[2] = "レベル";
                text[3] = "カテゴリ";
                text[4] = "ユーザー";
                text[5] = "メッセージ";
            }
            else if (Tools.IsLangChinese())
            {
                text[0] = "顺序";
                text[1] = "时间";
                text[2] = "级别";
                text[3] = "类别";
                text[4] = "用户";
                text[5] = "消息";
            }
            else
            {
                text[0] = "No";
                text[1] = "Time";
                text[2] = "Level";
                text[3] = "Category";
                text[4] = "User";
                text[5] = "Message";
            }

            for (int i = 0; i < 6; i++)
            {
                head = new ControlListViewHeader();
                head.width = headWidth[i];
                head.text = text[i];
                head.format = new StringFormat();
                head.format.Alignment = StringAlignment.Center;
                list.header.Add(head);
            }
        }

		void basicElementSetting()
		{
			list.font = ConfigViewMain.fontMain;
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {

            }
            else
            {
                list.contextMenu = this.contextMenuEventLogDetail;
            }
		}

		void fillListDataAll()
		{
            if (ds == null)
            {
                list.listHap = 0;
                filteredDataView = null;
            }
            else
            {
                //  초기에는 필터 없이 전체 표시
                filteredDataView = ds.Tables[0].DefaultView;
                filteredDataView.RowFilter = "";
                list.listHap = filteredDataView.Count;
            }
			list.listItemChanged();
			//setDetailButtonEnableDisable();
		}

        private DataView filteredDataView;

        void ApplyFilter()
        {
            if (ds == null || ds.Tables.Count == 0) return;

            DataTable sourceTable = ds.Tables[0];
            filteredDataView = sourceTable.DefaultView;

            string filter = "";

            // 레벨 필터
            if (filterLevel.HasValue)
            {
                filter = $"LevelValue = {(int)filterLevel.Value}";
            }

            // 카테고리 필터
            if (filterCategory.HasValue)
            {
                if (!string.IsNullOrEmpty(filter)) filter += " AND ";

                int categoryMin = filterCategory.Value;
                int categoryMax = categoryMin + 100;
                filter += $"CategoryValue >= {categoryMin} AND CategoryValue < {categoryMax}";
            }

            filteredDataView.RowFilter = filter;
            list.listHap = filteredDataView.Count;
            list.listItemChanged();
        }


        async void ShowLogDetail()
        {
            if (list.currPos < 0 || list.currPos >= list.listHap) return;
            if (ds == null || ds.Tables[0].Rows.Count <= list.currPos) return;

            DataRow row = ds.Tables[0].Rows[list.currPos];
            long logId = Convert.ToInt64(row["Id"]);

            DataGate gate = new DataGate();
            var logDetail = await gate.GetLogDetail(logId);

            if (logDetail != null)
            {
                string detailMessage = $"ID: {logDetail.Id}\n" +
                    $"시간: {logDetail.LogDateTime:yyyy-MM-dd HH:mm:ss}\n" +
                    $"레벨: {logDetail.Level}\n" +
                    $"카테고리: {logDetail.Category}\n" +
                    $"사용자: {logDetail.Username}\n" +
                    $"IP: {logDetail.IpAddress}\n" +
                    $"컴퓨터: {logDetail.MachineName}\n" +
                    $"메시지: {logDetail.Message}\n";

                if (!string.IsNullOrEmpty(logDetail.Detail))
                {
                    detailMessage += $"\n상세정보:\n{logDetail.Detail}";
                }

                MessageBox.Show(detailMessage, "로그 상세 정보",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // OK 클릭 후 자동으로 클립보드에 복사
                Clipboard.SetText(detailMessage);
            }
        }


        void currentListPrint()
		{			
			PrinterSettings s = new PrinterSettings();
			PrintDialog dialog = new PrintDialog();
			dialog.PrinterSettings = s;
			PrintDocument pd = new PrintDocument();
			pd.PrintPage += new PrintPageEventHandler(drawPrintPage);
			dialog.Document = pd;
			if(dialog.ShowDialog(this) != DialogResult.OK) return;
			nPrintListPos = 0;			// 0 번 list 부터 인쇄
			nPrintListPage = 1;			// 1 페이지 부터
			dialog.Document.Print();
		}

		void drawPrintPageHeader(PrintPageEventArgs ev, ref int y)
		{
			string		buf = "";
			int			x = 10;
			
			try 
			{
				if(logFileName.Length >= 8) 
				{
                    //if(Tools.IsLangKorean())
                    //	buf = String.Format("{0}{1}{2}{3}년 {4}{5}월 {6}{7}일 로그 자료", logFileName[0], logFileName[1], logFileName[2], logFileName[3], logFileName[4], logFileName[5], logFileName[6], logFileName[7]);
                    //else if(Tools.IsLangJapanese())
                    //	buf = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 ログデータ", logFileName[0], logFileName[1], logFileName[2], logFileName[3], logFileName[4], logFileName[5], logFileName[6], logFileName[7]);
                    //else if(Tools.IsLangChinese())
                    //	buf = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 日志", logFileName[0], logFileName[1], logFileName[2], logFileName[3], logFileName[4], logFileName[5], logFileName[6], logFileName[7]);
                    //else
                    //	buf = String.Format("{0}{1}{2}{3}/{4}{5}/{6}{7} Log Data", logFileName[0], logFileName[1], logFileName[2], logFileName[3], logFileName[4], logFileName[5], logFileName[6], logFileName[7]);

                    if (Tools.IsLangKorean())
                        buf = $"{logFileName} 로그 자료";
                    else if (Tools.IsLangJapanese())
                        buf = $"{logFileName} ログデータ";
                    else if (Tools.IsLangChinese())
                        buf = $"{logFileName} 日志";
                    else
                        buf = $"{logFileName} Log Data";
                }
				buf += "   ( Page " + nPrintListPage.ToString() + ")";
			}
			catch {}

            SafeException.SafeDrawString(ev.Graphics, buf, list.font, Brushes.Black, x, y);
			y += list.font.Height + 6;
			Rectangle				r;
			ControlListViewHeader	head;

			for(int i = 0; i < 3; i++) 
			{
				head = (ControlListViewHeader)list.header[i];
				r = new Rectangle(x, y, head.width, list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, head.text, list.font, Brushes.Black, r);
				x += head.width;
			}
			y += list.font.Height;
		}
		

		void drawPrintPage(object sender, PrintPageEventArgs ev)
		{
			if(list.listHap <= nPrintListPos) return;

			int						x, y = 40;
			DataRow					row;
			ControlListViewHeader	head;
			Rectangle				r;
			//string					data;
			
			drawPrintPageHeader(ev, ref y);                         // 순서, 사용자, 전화번호..
            for (int i = nPrintListPos; i < list.listHap; i++)
            {
                row = ds.Tables[0].Rows[i];
                if (row == null) continue;
                x = 10;

                // 순서
                head = (ControlListViewHeader)list.header[0];
                r = new Rectangle(x, y, head.width, list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, string.Format("{0}", i + 1),
                    list.font, Brushes.Black, r);
                x += head.width;

                // 시간
                head = (ControlListViewHeader)list.header[1];
                r = new Rectangle(x, y, head.width, list.font.Height);
                DateTime datetime = (DateTime)row["log_datetime"];
                SafeException.SafeDrawString(ev.Graphics, datetime.ToString("yyyy-MM-dd HH:mm:ss"), 
                    list.font, Brushes.Black, r);  //datetime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                x += head.width;

                // 레벨
                head = (ControlListViewHeader)list.header[2];
                r = new Rectangle(x, y, head.width, list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, row["Level"].ToString(),
                    list.font, Brushes.Black, r);
                x += head.width;

                // 카테고리
                head = (ControlListViewHeader)list.header[3];
                r = new Rectangle(x, y, head.width, list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, row["Category"].ToString(),
                    list.font, Brushes.Black, r);
                x += head.width;

                // 사용자
                head = (ControlListViewHeader)list.header[4];
                r = new Rectangle(x, y, head.width, list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, row["Username"].ToString(),
                    list.font, Brushes.Black, r);
                x += head.width;

                // 메시지
                head = (ControlListViewHeader)list.header[5];
                r = new Rectangle(x, y, head.width, list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, row["Message"].ToString(),
                    list.font, Brushes.Black, r);

                y += list.font.Height;
                if (y >= ev.Graphics.VisibleClipBounds.Height - list.font.Height)
                {
                    nPrintListPos = i + 1;
                    nPrintListPage++;
                    ev.HasMorePages = true;
                    return;
                }
            }
        }

		private void menuItem_PringLogData_Click(object sender, System.EventArgs e)
		{
			currentListPrint();
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) Close();
			if(e.Button == toolBarButton2) currentListPrint();
		}

		private void ViewEventLogDetailMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
                case Keys.Enter: ShowLogDetail(); return;
            }
			//if(child.mainArrowKeyOperation(e.KeyCode)) return;		
		}

		private void ViewEventLogDetailMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
			//this.listView1.Height = this.ClientSize.Height-this.toolBar1.Height;
		}

		//protected override void OnPaintBackground(PaintEventArgs pevent) 
		//{ 
		//	//preventing drawing background by overriding parent OnPaintBackground method 
		//	//with empty method. 
		//}

		private void ViewEventLogDetailMain_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			
			//TotalConfig.AutoBaseListCtrlConfigSave(this.listView1, "BasicScreen", "ViewEventLogDetail");
			list.doubleClick -= new ControlListView.OnEventDoubleClick(onListMouse_DoubleClicked);
			list.selectedIndexChanged -= new ControlListView.OnEventSelectedIndexChanged(onSelectedIndexChanged);
			list.paintMessage -= new ControlListView.OnEventPaintMessage(onPaintMessage);

			ControlListViewHeader head;
			bool					bChange = false;
			for(int i = 0; i < list.header.Count; i++) 
			{
				head = (ControlListViewHeader)list.header[i];
				if(headWidth[i] == head.width) continue;
				headWidth[i] = head.width;
				bChange = true;
			}
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "ViewEventLogDetail");

			ringForm.pop(this);
		}

        private void menuItem_FilterByLevel_Click(object sender, EventArgs e)
        {
            // 레벨 필터 콤보박스에 포커스
            comboBoxLevel.Focus();
            comboBoxLevel.DroppedDown = true;
        }

        private void menuItem_FilterByCategory_Click(object sender, EventArgs e)
        {
            // 카테고리 필터 콤보박스에 포커스
            comboBoxCategory.Focus();
            comboBoxCategory.DroppedDown = true;
        }

        private void menuItem__ViewDetail_Click(object sender, EventArgs e)
        {
            ShowLogDetail();
        }

        private void comboBoxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxLevel.SelectedIndex)
            {
                case 0: filterLevel = null; break;
                case 1: filterLevel = LogLevel.DEBUG; break;
                case 2: filterLevel = LogLevel.INFO; break;
                case 3: filterLevel = LogLevel.WARNING; break;
                case 4: filterLevel = LogLevel.ERROR; break;
                case 5: filterLevel = LogLevel.CRITICAL; break;
                case 6: filterLevel = LogLevel.FATAL; break;
            }
            ApplyFilter();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxCategory.SelectedIndex)
            {
                case 0: filterCategory = null; break;
                case 1: filterCategory = LogCategory.SYSTEM; break;
                case 2: filterCategory = LogCategory.SECURITY; break;
                case 3: filterCategory = LogCategory.DATA; break;
                case 4: filterCategory = LogCategory.TAG; break;
                case 5: filterCategory = LogCategory.COMMUNICATION; break;
                case 7: filterCategory = LogCategory.ALARM; break;
            }
            ApplyFilter();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }		

		

		

		
	}
}
