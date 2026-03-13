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
using System.Threading.Tasks;
using System.Security.Claims;

namespace BasicScreen.kdymain 
{
	/// <summary>
	/// Summary description for ViewListAlarmDetailMain.
	/// 
	/// 20251016 PSU - 경보를 DB저장/로드로 변경하고, row[0] 에 id가 추가되어 index 1씩 추가.
	///							 file명이 없고 2025-10-16처럼 날짜에 "-"이 추가되어 수정.
	/// </summary>
	public class ViewListAlarmDetailMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu contextMenuListAlarmDetail;
		private System.Windows.Forms.MenuItem menuItem3;

		//BasicScreen.kdymain.ViewListAlarmDetail child;
		DataSet					ds;
		int[]					headWidth = new int[7];
		ControlListView			list = new ControlListView();
		ArrayList				alarmList = new ArrayList();
		//int			TagHap, currPos;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.MenuItem menuItem_Priority;
		private System.Windows.Forms.MenuItem menuItem_print;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.FontDialog fontDialog1;

		string	alarmFileName;
		bool[]	bPriority = new bool[1000];
		
		int		nPrintListPage, nPrintListPos;
		int	year, month, day;
		private System.Windows.Forms.Panel panel1;
        private ToolBarButton toolBarButton4;
		private System.Windows.Forms.Panel panel2;

        public ViewListAlarmDetailMain(string alarm, int pos, int call_method, int al_year, int al_month, int al_day, DateTime call2_from, DateTime call2_To, string call2_filter)
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
			
			alarmFileName = alarm;
            if (call_method == 1)	// ViewAlarmList함수에서 호출한 경우이다. 
            {
                year = al_year;
                month = al_month;
                day = al_day;
            }
            else if (call_method == 2)	// DialogAlaFileSearch에서 콜
            {

            }
            else
            {
                try
                {
                    year = ConvertTool.ToUInt16(alarmFileName.Substring(0, 4));
                    month = ConvertTool.ToUInt16(alarmFileName.Substring(5, 2));
                    day = ConvertTool.ToUInt16(alarmFileName.Substring(8, 2));
                }
                catch
                {
                    year = 1;
                    month = 1;
                    day = 1;
                }

                if (alarm.Length >= 8)
                {
					//if (Tools.IsLangKorean())
					//    this.Text = String.Format("{0}{1}{2}{3}년 {4}{5}월 {6}{7}일 경보내용 보기", alarm[0], alarm[1], alarm[2], alarm[3], alarm[4], alarm[5], alarm[6], alarm[7]);
					//else if (Tools.IsLangJapanese())
					//    this.Text = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 警報の詳細表示", alarm[0], alarm[1], alarm[2], alarm[3], alarm[4], alarm[5], alarm[6], alarm[7]);
					//else if (Tools.IsLangChinese())
					//    this.Text = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 查看警报内容", alarm[0], alarm[1], alarm[2], alarm[3], alarm[4], alarm[5], alarm[6], alarm[7]);
					//else
					//    this.Text = String.Format("{0}{1}{2}{3}/{4}{5}/{6}{7} Alarm Data View", alarm[0], alarm[1], alarm[2], alarm[3], alarm[4], alarm[5], alarm[6], alarm[7]);

					if (Tools.IsLangKorean())
						this.Text = $"{alarm} 경보내용 보기";
					else if (Tools.IsLangJapanese())
						this.Text = $"{alarm} 警報の詳細表示";
					else if (Tools.IsLangChinese())
						this.Text = $"{alarm} 查看警报内容";
					else
						this.Text = $"{alarm} Alarm Data View";
                }
            }

			for(int i = 0; i < 1000; i ++) bPriority[i] = true;		// 모든 우선순위

			this.Load += async (s, e) =>
            {
  
      
            DataGate gate = new DataGate();
            if (call_method == 1)
            {
                DateTime tFrom;
                DateTime tTo;

                if (year == 0)
                {
                    tFrom = TimeUtil.MakeDateTime(1, 1, 1, 0, 0, 0);
                    tTo = TimeUtil.MakeDateTime(9999, 12, 31, 23, 59, 59);
                }
                else if (month == 0)
                {
                    tFrom = TimeUtil.MakeDateTime(year, 1, 1, 0, 0, 0);
                    tTo = TimeUtil.MakeDateTime(year, 12, 31, 23, 59, 59);
                }
                else if (day == 0)
                {
                    tFrom = TimeUtil.MakeDateTime(year, month, 1, 0, 0, 0);
                    tTo = TimeUtil.MakeDateTime(year, month, 31, 23, 59, 59);
                }
                else
                {
                    tFrom = TimeUtil.MakeDateTime(year, month, day, 0, 0, 0);
                    tTo = TimeUtil.MakeDateTime(year, month, day, 23, 59, 59);
                }

                ds = await gate.GetAlarmFileByScript(tFrom, tTo, "", false);
            }
            else if (call_method == 2)
            {
                ds = await gate.GetAlarmFileByScript(call2_from, call2_To, call2_filter, false);
                //getValidPriority();
                //fillListDataAll();                
            }
            else
                ds = await gate.GetAlarmFile(alarm);

                getValidPriority();

                listHeaderFill();
                list.Show();
                fillListDataAll();
                this.panel1.Height = this.toolBar1.Height;          // 초기크기를 설정
                ringForm.push(this);

            };

            headWidth[0] = 80;
			headWidth[1] = 80;
			headWidth[2] = 100;
			headWidth[3] = 100;
			headWidth[4] = 120;
			headWidth[5] = 130;
			headWidth[6] = 200;
			list.currPos = pos;
			//getValidPriority();
		}

		void getValidPriority()
		{
			alarmList.Clear();
			if(ds == null) 
			{
				list.listHap = 0;
				return;
			}
			
			DataRow				row;
			int					nPriority = 0, alarm_type;
			DateTime			dt;
			AlarmListItemData	data;

			for(int i = 0; i < ds.Tables[0].Rows.Count; i++) 
			{
				row = ds.Tables[0].Rows[i];
				try 
				{
					nPriority = ConvertTool.ToInt16(row[6].ToString());
					if(nPriority >= 1000 || nPriority < 0) nPriority = 0;
					if(bPriority[nPriority % 1000] == false) continue;

					data = new AlarmListItemData();
                    data.pos_datarow = i;   // Row의 위치도 기억해 놓는다.
					data.text[0] = string.Format("{0}", alarmList.Count + 1);

					alarm_type = ConvertTool.ToInt16(row[5].ToString());
					data.color = SharedData.alarmClass.GetAlarmColor(alarm_type, nPriority);
					data.text[1] = String.Format("{0,03:D3}", nPriority);
					dt = ConvertTool.ToDateTime(row[1].ToString());
					data.text[2] = dt.ToShortDateString();
					data.text[3] = String.Format("{0}:{1,02:D2}:{2,02:D2}", dt.Hour, dt.Minute, dt.Second);
					data.text[4] = row[2].ToString();
					data.text[5] = row[3].ToString();
                    data.text[6] = row[4].ToString();
                    if ((int)row[5] == 7 && ConfigViewMain.bShowUserManualControl)
                    {
                        if (Tools.IsLangKorean()) 
                            data.text[6] += String.Format(" 변경자=''{0}''", row[11].ToString());
                        else 
                            data.text[6] += String.Format(" Changed by=''{0}''", row[11].ToString());
                    }

                    //if(alarm_type == 7)
                    //    data.text[6] += " 사용자="+row[10].ToString();

					alarmList.Add(data);
				}
				catch 
				{
					continue;
				}
			}
			list.listHap = alarmList.Count;
			if(list.currPos < 0 || list.currPos >= list.listHap) list.currPos = list.listHap-1;	

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewListAlarmDetailMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.contextMenuListAlarmDetail = new System.Windows.Forms.ContextMenu();
            this.menuItem_Priority = new System.Windows.Forms.MenuItem();
            this.menuItem_print = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.AccessibleDescription = null;
            this.toolBar1.AccessibleName = null;
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.BackgroundImage = null;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4});
            this.toolBar1.Font = null;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButton1
            // 
            resources.ApplyResources(this.toolBarButton1, "toolBarButton1");
            this.toolBarButton1.Name = "toolBarButton1";
            // 
            // toolBarButton2
            // 
            resources.ApplyResources(this.toolBarButton2, "toolBarButton2");
            this.toolBarButton2.Name = "toolBarButton2";
            // 
            // toolBarButton3
            // 
            resources.ApplyResources(this.toolBarButton3, "toolBarButton3");
            this.toolBarButton3.Name = "toolBarButton3";
            // 
            // toolBarButton4
            // 
            resources.ApplyResources(this.toolBarButton4, "toolBarButton4");
            this.toolBarButton4.Name = "toolBarButton4";
            // 
            // contextMenuListAlarmDetail
            // 
            this.contextMenuListAlarmDetail.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_Priority,
            this.menuItem_print,
            this.menuItem5,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuListAlarmDetail, "contextMenuListAlarmDetail");
            // 
            // menuItem_Priority
            // 
            resources.ApplyResources(this.menuItem_Priority, "menuItem_Priority");
            this.menuItem_Priority.Index = 0;
            this.menuItem_Priority.Click += new System.EventHandler(this.menuItem_Priority_Click);
            // 
            // menuItem_print
            // 
            resources.ApplyResources(this.menuItem_print, "menuItem_print");
            this.menuItem_print.Index = 1;
            this.menuItem_print.Click += new System.EventHandler(this.menuItem_print_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 2;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 3;
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.toolBar1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // ViewListAlarmDetailMain
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "ViewListAlarmDetailMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ViewListAlarmDetailMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewListAlarmDetailMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewListAlarmDetailMain_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewListAlarmDetailMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void OnMainFontChanged()
		{
			list.font = ConfigViewMain.fontMain;
			list.fontChanged();
			//list.font = ConfigViewMain.fontMain;
			//list.fontChanged();
		}

		void OnColorChanged()
		{
			this.BackColor = SharedData.colorTotal.BACK;				// 기본화면의 배경색상, list visable =
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용
			list.dataAreaInvalidate();
		}

		Task OnNewAlarm(ALARM_FILE_STRUCT alarm)
		{
			if(year != alarm.t.wYear || month != alarm.t.wMonth || day != alarm.t.wDay) return Task.CompletedTask;
			if(alarm.priority >= 1000 || alarm.priority < 0) return Task.CompletedTask;
			if(bPriority[alarm.priority % 1000] == false) return Task.CompletedTask;

			AlarmListItemData	data = new AlarmListItemData();
			data.color = SharedData.alarmClass.GetAlarmColor(alarm.alarm_type, alarm.priority);
			data.text[0] = String.Format("{0}", alarmList.Count + 1);
			data.text[1] = String.Format("{0,03:D3}", alarm.priority);
			DateTime dt = alarm.t.ToDateTime();
			data.text[2] = dt.ToShortDateString();
			data.text[3] = String.Format("{0}:{1,02:D2}:{2,02:D2}", dt.Hour, dt.Minute, dt.Second);
			data.text[4] = alarm.tag;
			data.text[5] = alarm.description;
			data.text[6] = alarm.msg;
			alarmList.Add(data);
			list.listHap = alarmList.Count;
			list.listItemChanged();
            //AddNewItem(alarm);

            return Task.CompletedTask;

        }

		void onListMouse_DoubleClicked()
		{
            if (list.listHap <= 0 || list.currPos < 0 || list.listHap <= list.currPos) return;

            AlarmListItemData data = (AlarmListItemData)alarmList[list.currPos];

            DataRow row = ds.Tables[0].Rows[data.pos_datarow];

            ViewListAlarmDetailItemDetail dialog = new ViewListAlarmDetailItemDetail();

            dialog.Set(ds, row);
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
		}

		void onSelectedIndexChanged()
		{
			if(list.listHap > 0) menuButtonPrintEnableDisable(true);
			else menuButtonPrintEnableDisable(false);
		}

		void oneLineDraw(Graphics g, int pos, int x, int y, int xGap)
		{
			AlarmListItemData		data = (AlarmListItemData)alarmList[pos];
			if(data == null) return;
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;

			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];
			string		buf = string.Format("{0}", pos+1);
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, buf.ToString(), data.color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data.text[1], data.color, list.backColor, list.font, head.format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data.text[2], data.color, list.backColor, list.font, head.format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[3];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data.text[3], data.color, list.backColor, list.font, head.format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[4];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data.text[4], data.color, list.backColor, list.font, format);

			x += head.width;
			head = (ControlListViewHeader)list.header[5];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data.text[5], data.color, list.backColor, list.font, format);

			x += head.width;
			head = (ControlListViewHeader)list.header[6];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data.text[6], data.color, list.backColor, list.font, format);
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, SharedData.alarmClass.colorAlarmBack);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, SharedData.alarmClass.colorAlarmBack);
			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;			
			
			endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;
				if(pos >= alarmList.Count) return;		// 저장한 리스트보다 크다
				
				x = list.startX;
				oneLineDraw(g, pos, x, y, xGap);
			}
		}

		void onPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}
		
		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();

		private void ViewListAlarmDetailMain_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged += new SharedViewMain.DelegatePublic(OnColorChanged);
			SharedViewMain.EventListNewAlarm += new SharedViewMain.DelegateNewAlarm(OnNewAlarm);
			
			list.doubleClick += new ControlListView.OnEventDoubleClick(onListMouse_DoubleClicked);
			list.selectedIndexChanged += new ControlListView.OnEventSelectedIndexChanged(onSelectedIndexChanged);
			list.paintMessage += new ControlListView.OnEventPaintMessage(onPaintMessage);
			            			
			TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "ViewListAlarmDetail");
			
			list.bOwnerDraw = true;

            list.colorCursor = Color.FromArgb(40, Color.DarkGray);
            list.colorCursorFocus = Color.FromArgb(40, Color.LightSkyBlue);
            list.colorCursorBorder = Color.FromArgb(128, Color.LightBlue);

			//listHeaderFill();
			//list.Show();
			//fillListDataAll();
			//this.panel1.Height = this.toolBar1.Height;			// 초기크기를 설정
			//ringForm.push(this);
		}

		void listHeaderFill()
		{
			ControlListViewHeader	head;
			string[]				text = new string[7];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "순서";
				text[1] = "우선순위";
				text[2] = "날짜";
				text[3] = "시간";
				text[4] = "태그";
				text[5] = "설명";				
				text[6] = "경보내용";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "番号";
				text[1] = "レベル";
				text[2] = "日付";
				text[3] = "時刻";
				text[4] = "タグ";
				text[5] = "説明";
                text[6] = "警報內容";
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "顺序";
				text[1] = "优先权";
				text[2] = "日期";
				text[3] = "时间";
				text[4] = "标记";
				text[5] = "标记描述";				
				text[6] = "警报内容";
			}
            else if (Tools.IsLangVietnamese())
            {
                text[0] = "Số";
                text[1] = "Mức";
                text[2] = "Ngày";
                text[3] = "Thời gian";
                text[4] = "Tag";
                text[5] = "Mô tả";
                text[6] = "Nội dung báo động";
            }
			else 
			{
				text[0] = "No";
				text[1] = "Priority";
				text[2] = "Date";
				text[3] = "Time";
				text[4] = "Tag";
				text[5] = "Description";
				text[6] = "Alarm Content";
			}			

			for(int i = 0; i < 7; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();
				//if(i == 0 || i > 3) head.format.Alignment = StringAlignment.Near;
				head.format.Alignment = StringAlignment.Center;
				head.color = SharedData.colorTotal.TEXT;// 기본으로 사용할 색상,
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
                list.contextMenu = this.contextMenuListAlarmDetail;
            }
		}

		/*void fitValidStartPos()
		{
			//if(list.bVscroll == false) return;
			if(list.listHap <= list.pageLineCount) return;
			if(list.currPos >= list.startPos && list.currPos < list.startPos + list.pageLineCount) return;

			if(list.pageLineCount <= 0) list.startPos = list.currPos;
			else list.startPos = list.currPos-list.pageLineCount+1;
		}*/

		void fillListDataAll()
		{
			list.listItemChanged();
			if(list.listHap > 0) menuButtonPrintEnableDisable(true);
			else menuButtonPrintEnableDisable(false);
		}

		void menuButtonPrintEnableDisable(bool flag)
		{
			this.toolBarButton3.Enabled = flag;
			this.menuItem_print.Enabled = flag;
		}

		int getSelectedPos()
		{
			return list.currPos;
			//if(this.listView1.SelectedItems == null || this.listView1.SelectedItems.Count <= 0) return 0;
			//return this.listView1.SelectedIndices[0];
		}


		void callPriorityFilteringDialog()
		{
			ViewListAlarmDetailDlgPriority dialog = new ViewListAlarmDetailDlgPriority(bPriority);
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK) 
			{
				this.bPriority = dialog.bPriority;
				getValidPriority();
				fillListDataAll();
				//fillListView(getSelectedPos());
			}
		}

		public void currentListPrint()
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
				if(alarmFileName.Length >= 8) 
				{
					if (Tools.IsLangKorean())
                        buf = $"{alarmFileName} 경보 자료";
                    else if (Tools.IsLangJapanese())
                        buf = $"{alarmFileName} 警報";
                    else if (Tools.IsLangChinese())
                        buf = $"{alarmFileName} 警报";
                    else
                        buf = $"{alarmFileName} Alarm Data";
                }
				buf += "   ( Page " + nPrintListPage.ToString() + ")";
			}
			catch {}

            SafeException.SafeDrawString(ev.Graphics, buf, list.font, Brushes.Black, x, y);
			y += list.font.Height + 6;
			Rectangle				r;
			ControlListViewHeader	head;

			for(int i = 0; i < 7; i++) 
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
			Rectangle				r;
			AlarmListItemData		data;
			ControlListViewHeader	head;

			drawPrintPageHeader(ev, ref y);
			for(int i = nPrintListPos; i < list.listHap; i++)
			{	
				data = (AlarmListItemData)alarmList[i];
				if(data == null) continue;				
				x = 10;
				for(int j = 0; j < 7; j++) 
				{
					head = (ControlListViewHeader)list.header[j];
					r = new Rectangle(x, y, head.width, list.font.Height);
                    SafeException.SafeDrawString(ev.Graphics, data.text[j], list.font, Brushes.Black, r);
					x += head.width;
				}
				y += list.font.Height;
				if(y >= ev.Graphics.VisibleClipBounds.Height-list.font.Height)
				{
					nPrintListPos = i+1;
					nPrintListPage ++;
					ev.HasMorePages = true;
					return;
				}
			}
		}
		
		private void menuItem_Priority_Click(object sender, System.EventArgs e)
		{
			callPriorityFilteringDialog();
		}

		private void menuItem_print_Click(object sender, System.EventArgs e)
		{
			currentListPrint();
		}
		
		private async void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) Close();
			if(e.Button == toolBarButton2) callPriorityFilteringDialog();
			if(e.Button == toolBarButton3) currentListPrint();
            if (e.Button == toolBarButton4)
            {
                ViewListAlarmDetailConfigSearch dialog = new ViewListAlarmDetailConfigSearch();
                dialog.StartPosition = FormStartPosition.CenterParent;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    DataGate gate = new DataGate();
                    ds = await gate.GetAlarmFileByScript(dialog.tFrom, dialog.tTo, dialog.sFilter, false);
                    getValidPriority();
                    fillListDataAll();
                }
            }
		}
		

		private void ViewListAlarmDetailMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
			}
		}

		private void ViewListAlarmDetailMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewListAlarmDetailMain_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			SharedViewMain.EventListNewAlarm -= new SharedViewMain.DelegateNewAlarm(OnNewAlarm);

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
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "ViewListAlarmDetail");

			ringForm.pop(this);
		}

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

	}


	class AlarmListItemData
	{
		public string[]					text = new string[7];
		public Color					color;
        public int pos_datarow;
	};

}
