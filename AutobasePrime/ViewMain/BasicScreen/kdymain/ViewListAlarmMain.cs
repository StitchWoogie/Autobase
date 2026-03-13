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
using System.Threading;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewListAlarmMain.
	/// </summary>
	public class ViewListAlarmMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu contextMenuListAlarm;
		private System.Windows.Forms.MenuItem menuItem_view_list_alarm;
		private System.Windows.Forms.MenuItem menuItem3;

		//BasicScreen.kdymain.ViewListAlarm child;
		DataSet					ds;
		int[]					headWidth = new int[3];
		ControlListView			list = new ControlListView();
		//int			TagHap, currPos;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem_DeleteAlarmFile;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;

		public ViewListAlarmMain(int pos)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			//work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			//AdInitDisplayCurrentColumnWidthAlarm();
			//child = new ViewListAlarm(work.xNum, pos);
			//this.Controls.Add(child);			
			this.panel2.Controls.Add(list);
			basicElementSetting();
            this.Load += async (sender, e) => await GetLogListAndHap();
			list.currPos = pos;
			if(list.currPos < 0 || list.currPos >= list.listHap) list.currPos = list.listHap-1;
			
			headWidth[0] = 80;
			headWidth[1] = 180;
			headWidth[2] = 130;
		}

		async Task GetLogListAndHap()
		{
			try
			{
				DataGate gate = new DataGate();
				ds = await gate.GetAlarmLists();

				if (ds == null) list.listHap = 0;
				else list.listHap = ds.Tables[0].Rows.Count;

                this.Refresh(); //260102 PSU 추가. 데이터 로드 지연 시 다시 paint
            }
			catch (Exception ex)
			{
				list.listHap = 0;
				MessageDisplay.Show(ex.Message, "Error");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewListAlarmMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.contextMenuListAlarm = new System.Windows.Forms.ContextMenu();
            this.menuItem_view_list_alarm = new System.Windows.Forms.MenuItem();
            this.menuItem_DeleteAlarmFile = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3});
            resources.ApplyResources(this.toolBar1, "toolBar1");
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
            // toolBarButton3
            // 
            this.toolBarButton3.Name = "toolBarButton3";
            resources.ApplyResources(this.toolBarButton3, "toolBarButton3");
            // 
            // contextMenuListAlarm
            // 
            this.contextMenuListAlarm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_view_list_alarm,
            this.menuItem_DeleteAlarmFile,
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem_view_list_alarm
            // 
            this.menuItem_view_list_alarm.Index = 0;
            resources.ApplyResources(this.menuItem_view_list_alarm, "menuItem_view_list_alarm");
            this.menuItem_view_list_alarm.Click += new System.EventHandler(this.menuItem_view_list_alarm_Click);
            // 
            // menuItem_DeleteAlarmFile
            // 
            this.menuItem_DeleteAlarmFile.Index = 1;
            resources.ApplyResources(this.menuItem_DeleteAlarmFile, "menuItem_DeleteAlarmFile");
            this.menuItem_DeleteAlarmFile.Click += new System.EventHandler(this.menuItem_DeleteAlarmFile_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolBar1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // ViewListAlarmMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "ViewListAlarmMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ViewListAlarmMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewListAlarmMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewListAlarmMain_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewListAlarmMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		int getSelectedPos()
		{
			return list.currPos;
			//if(this.listView1.SelectedItems == null || this.listView1.SelectedItems.Count <= 0) return -1;
			//return this.listView1.SelectedIndices[0];
		}

		void setDetailButtonEnableDisable()		// 상세 버턴을 Enable/Disable
		{
			if(getSelectedPos() == -1) 
			{
				toolBarButton2.Enabled = false;
				toolBarButton3.Enabled = false;
				menuItem_view_list_alarm.Enabled = false;
				menuItem_DeleteAlarmFile.Enabled = false;
			}
			else 
			{
				toolBarButton2.Enabled = true;
				menuItem_view_list_alarm.Enabled = true;
                toolBarButton3.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_DELETE_ALARM_FILE);//log file delete
                menuItem_DeleteAlarmFile.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_DELETE_ALARM_FILE);//log file delete
			}
		}


		private void OnMainFontChanged()
		{
			list.font = ConfigViewMain.fontMain;
			list.fontChanged();
			//this.listView1.Font = ConfigViewMain.fontMain;
			//this.listView1.Refresh();		// font 가 바뀌면 새로 그려야 한다
		}

		void OnColorChanged()
		{
			this.BackColor = SharedData.colorTotal.BACK;			// 기본화면의 배경색상, list visable =
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용
			list.dataAreaInvalidate();
			//this.listView1.BackColor = SharedData.colorTotal.BACK;
			//for(int i = 0; i < TagHap; i++) 
			//{
			//	ModifyOneListItem(i);
			//}
		}

		void OnUserChanged()
		{
			setDetailButtonEnableDisable();
		}

        /// <summary>
        /// 9.5.1 까지 getLogListAndHap(); fillListDataAll(); 두줄로 경보가 발생하면 무조건 다시 읽어 왔는데 이것은 속도가 너무 느리므로 
        /// 9.5.2 부터 해당날짜의 아이템만 바꿔주는 방식으로 바꾸었다.
        /// </summary>
        /// <param name="alarm"></param>
        async Task OnNewAlarm(ALARM_FILE_STRUCT alarm)
        {
            if (ds != null)
            {
                DataRow row;
                int year, month, day;
                string data, imsi;
                int count;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    row = ds.Tables[0].Rows[i];

                    data = row[0].ToString();

                    imsi = data.Substring(0, 4);
                    year = ConvertTool.ToInt32(imsi);

                    imsi = data.Substring(4, 2);
                    month = ConvertTool.ToInt32(imsi);

                    imsi = data.Substring(6, 2);
                    day = ConvertTool.ToInt32(imsi);

                    if (year == alarm.t.wYear && month == alarm.t.wMonth && day == alarm.t.wDay)
                    {
                        imsi = row[1].ToString();
                        count = ConvertTool.ToInt32(imsi);
                        count++;
                        row[1] = count.ToString();
                        list.listItemChanged();
                        return;
                    }
                }
            }

            // 해당되는 날짜의 경보가 없다. 새로운 날짜의 경보
            await GetLogListAndHap();
            fillListDataAll();
        }

		void OnListMouse_DoubleClicked()
		{
			CallListAlarmDetailWindows();
		}

		void OnSelectedIndexChanged()
		{
			setDetailButtonEnableDisable();
		}
		
		void OneLineDraw(Graphics g, DataRow row, int i, int x, int y, int xGap)
		{
			Color					color;
			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];			
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			
			color = SharedData.colorTotal.TEXT;			
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, i.ToString(), color, list.backColor, list.font, head.format);

			string data = String.Format("{0}", row[0].ToString());
			if(data.Length < 8) return;

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			//if(Tools.IsLangKorean())
			//	data = String.Format("{0}{1}{2}{3}년 {4}{5}월 {6}{7}일 경보", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);				
			//else if(Tools.IsLangJapanese())
   //             data = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 警報", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);				
			//else if(Tools.IsLangChinese())
			//	data = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 警报", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);				
			//else
			//	data = String.Format("{0}{1}{2}{3}/{4}{5}/{6}{7} Alarm Data", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data, color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, row[1].ToString(), color, list.backColor, list.font, head.format);
			
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);

			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;			
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
			DataRow					row;

			endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;

				row = ds.Tables[0].Rows[pos];
				if(row == null) continue;
				x = list.startX;
				OneLineDraw(g, row, pos+1, x, y, xGap);
			}
		}

		void OnPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();
		
		private void ViewListAlarmMain_Load(object sender, System.EventArgs e)
		{
			//SharedViewMain.EventListTagChanged += new SharedViewMain.OnEventTagChanged(OnEventTagChanged);
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged += new SharedViewMain.DelegatePublic(OnColorChanged);
			//SharedViewMain.EventListTagPropertyChanged += new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
			//SharedViewMain.EventListTagListChanged += new SharedViewMain.DelegatePublic(OnTagListChanged);
			SharedViewMain.EventListUserChanged += new SharedViewMain.DelegatePublic(OnUserChanged);
			SharedViewMain.EventListNewAlarm += new SharedViewMain.DelegateNewAlarm(OnNewAlarm);

			//TotalConfig.AutoBaseListCtrlConfigLoad(this.listView1, "BasicScreen", "ViewListAlarm");
			//fillListView(currPos);
			list.doubleClick += new ControlListView.OnEventDoubleClick(OnListMouse_DoubleClicked);
			list.selectedIndexChanged += new ControlListView.OnEventSelectedIndexChanged(OnSelectedIndexChanged);
			list.paintMessage += new ControlListView.OnEventPaintMessage(OnPaintMessage);
			            			
			TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "ViewListAlarm");
			
			list.bOwnerDraw = true;
			listHeaderFill();
			list.Show();
			fillListDataAll();
			this.panel1.Height = this.toolBar1.Height;			// 초기크기를 설정
			ringForm.push(this);

            if (TotalConfig.eOemType == EnumOemType.KobasAI)
            {
                this.Text = "이상 탐지";
            }
		}

		void listHeaderFill()
		{
			ControlListViewHeader	head;
			string[]				text = new string[3];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "순서";
				text[1] = "경보날짜";
				text[2] = "경보개수";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "No";
                text[1] = "警報日付";
                text[2] = "警報の数";
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "顺序";
				text[1] = "警报日期";
				text[2] = "警报数";
			}
            else if (Tools.IsLangVietnamese())
            {
                text[0] = "Số";
                text[1] = "Ngày báo động";
                text[2] = "Số báo động";
            }
			else 
			{
				text[0] = "No";
				text[1] = "Alarm Date";
				text[2] = "Alarm Count";
			}			

			for(int i = 0; i < 3; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();
				//if(i < 3) head.format.Alignment = StringAlignment.Near;
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
                list.contextMenu = this.contextMenuListAlarm;
            }
		}

		void fillListDataAll()
		{
			if(ds == null) list.listHap = 0;
			else list.listHap = ds.Tables[0].Rows.Count;
			list.listItemChanged();
			setDetailButtonEnableDisable();
		}

		/*void fillListView(int pos)
		{
			this.listView1.BackColor = SharedData.colorTotal.BACK;
			this.listView1.Font = ConfigViewMain.fontMain;
			this.listView1.Items.Clear();
			for(int i = 0; i < TagHap; i++) AddNewItem(i);
			setListItemPos(pos);
		}

		void setListItemPos(int pos)
		{
			if(pos < 0) return;
			if(this.listView1.Items.Count > pos) 
			{
				this.listView1.Items[pos].Selected = true;	// 지정한 (첫번째 = 기본 ) 항목을 선택
				this.listView1.Items[pos].Focused = true;
				this.listView1.Items[pos].EnsureVisible();
				setDetailButtonEnableDisable();
			}
		}

		void setItemPosNoReArrange()
		{
			if(this.listView1.Items.Count <= 0) return;

			ListViewItem			item;
			for(int i = 0; i < listView1.Items.Count; i++)
			{
				item = this.listView1.Items[i];
				item.Text = string.Format("{0}", i+1);
			}
		}*/

		void CallListAlarmDetailWindows()
		{
			if(list.listHap <= 0 || list.currPos < 0 || list.listHap <= list.currPos) return;
			
			DataRow				row = ds.Tables[0].Rows[list.currPos];

            ViewListAlarmDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewListAlarmDetailMain(row[0].ToString(), -1, 0, 1, 1, 1, DateTimeServer.Now, DateTimeServer.Now, ""), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void ViewAlarmList(int year, int month, int day)
		{
            ViewListAlarmDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewListAlarmDetailMain("", -1, 1, year, month, day, DateTimeServer.Now, DateTimeServer.Now, ""), ConfigViewMain.nMdiCountOnBasicScreen);			
		}

		private SemaphoreSlim _deleteAlarmLock = new SemaphoreSlim(1, 1);
		async Task SelectedListAlarmFileDelete()
		{
			if (!await _deleteAlarmLock.WaitAsync(0))
				return;
			try
			{
				if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_DELETE_ALARM_FILE)) return;

				if (list.currPos >= list.listHap || list.currPos < 0) return;
				DataRow row = ds.Tables[0].Rows[list.currPos];
				if (row == null) return;

				string filename, caption;
				filename = string.Format("{0}\\alarm\\{1}", TotalConfig.GetProjectDataDirectory(), row[0].ToString());
				if (Tools.IsLangKorean()) caption = "이 파일을 삭제할까요?";
				else if (Tools.IsLangJapanese()) caption = "このファイルを削除しますか。";
				else if (Tools.IsLangChinese()) caption = "要删除此文件吗？";
				else if (Tools.IsLangVietnamese()) caption = "Bạn có muốn xóa file Báo cáo đã chọn?";
				else caption = "Are you sure you want to delete the selected Alarm File?";

				if (MessageBox.Show(filename, caption, MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					try
					{
						if (System.IO.File.Exists(filename))
						{
							System.IO.File.Delete(filename);
							await GetLogListAndHap();
							//this.listView1.Items.RemoveAt(pos);
							if (list.currPos >= list.listHap) list.currPos = list.listHap - 1;
							fillListDataAll();
							//setItemPosNoReArrange();						// 순서번호를 다시정렬
							//setListItemPos(pos);
						}
					}
					catch { }
				}
			}
			finally
			{
				_deleteAlarmLock.Release();
			}
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			setDetailButtonEnableDisable();
		}

		private void menuItem_view_list_alarm_Click(object sender, System.EventArgs e)
		{
			CallListAlarmDetailWindows();
		}

		private async void menuItem_DeleteAlarmFile_Click(object sender, System.EventArgs e)
		{
			await SelectedListAlarmFileDelete();
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			CallListAlarmDetailWindows();
		}


		/*private void setAlarmMainSizeChange()
		{
			GetMainYnumSize(true);
			if(work.bDisplayFitWindowSize == true) 
			{
				getMatchFontSize(getTotalColumnWidthHap()/2, work.width);
				GetMainYnumSize(true);
			}
			
			child.Left = 0;
			child.Top = (int)(work.fontY*1.5);
			child.Width = work.width;
			child.Height = work.height-(toolBar1.Height+(int)(work.fontY*1.5));

			this.toolBar1.Left = 0;
			this.toolBar1.Top = work.height-this.toolBar1.Height;
			this.toolBar1.Width = work.width;
		}

		private void ViewListAlarmMain_SizeChanged(object sender, System.EventArgs e)
		{
			setAlarmMainSizeChange();
			this.Invalidate();		
		}

		private void ViewListAlarmMain_Resize(object sender, System.EventArgs e)
		{
			ViewListAlarmMain_SizeChanged(sender, e);		
		}*/

		private async void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) this.Close();
			if(e.Button == toolBarButton2) CallListAlarmDetailWindows();
			if(e.Button == toolBarButton3) await SelectedListAlarmFileDelete();
		}

		/*private void ViewListAlarmMain_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			setAlarmMainSizeChange();
			work.Ix = this.child.AutoScrollPosition.X;

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.

			Graphics gScreen = e.Graphics;
			Bitmap bitmap = new Bitmap(ClientRectangle.Width, (int)(work.fontY*1.5), gScreen);
			Graphics g = Graphics.FromImage(bitmap);
			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;

			ListAlarmDescriptionDraw(g, 0);
			if(work.bMouseCapture) AdDisplayCaptureLineDraw(g);

			gScreen.DrawImageUnscaled(bitmap, 0, 0);
		}

		private void ViewListAlarmMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left) return;

			if(e.Y < ((int)(work.fontY*1.5))) 
			{
				if(work.bMouseCapture) 
				{					
					return;
				}
				if(!checkPointDisplayReSize(e.X)) 
				{				
					return;
				}
		
				Cursor = Cursors.SizeWE;
				work.bMouseCapture = true;

				AdDisplayCaptureLineInvalidate();
				//Graphics g = CreateGraphics();
				//AdDisplayCaptureLineDraw(g);
				return;
			}
		}

		private void ViewListAlarmMain_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			AdWmMouseMove(e.X, e.Y);		
		}

		private void ViewListAlarmMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left) return;
			
			if(AdWmLButtonUp(e.X)) 
			{
				child.Invalidate();
				this.child.Focus();
			}
		}

		private void toolBar1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{	// 사이즈 조절 마우스 위치에서 마우스가 바뀌면 커서를 기본으로 바꾼다.
			if(Cursor.Current != Cursors.Arrow)	Cursor = Cursors.Arrow;		
		}*/

		private async void ViewListAlarmMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
				case Keys.Enter : CallListAlarmDetailWindows(); return;
				case Keys.Delete : await SelectedListAlarmFileDelete();; return;
			}
			//if(child.mainArrowKeyOperation(e.KeyCode)) return;		
		}

		private void ViewListAlarmMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
			//this.listView1.Height = this.ClientSize.Height-this.toolBar1.Height;
		}


		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewListAlarmMain_Closed(object sender, System.EventArgs e)
		{
			//SharedViewMain.EventListTagChanged -= new SharedViewMain.OnEventTagChanged(OnEventTagChanged);
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			//SharedViewMain.EventListTagPropertyChanged -= new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
			//SharedViewMain.EventListTagListChanged -= new SharedViewMain.DelegatePublic(OnTagListChanged);
			SharedViewMain.EventListUserChanged -= new SharedViewMain.DelegatePublic(OnUserChanged);
			SharedViewMain.EventListNewAlarm -= new SharedViewMain.DelegateNewAlarm(OnNewAlarm);

			//TotalConfig.AutoBaseListCtrlConfigSave(this.listView1, "BasicScreen", "ViewListAlarm");
			list.doubleClick -= new ControlListView.OnEventDoubleClick(OnListMouse_DoubleClicked);
			list.selectedIndexChanged -= new ControlListView.OnEventSelectedIndexChanged(OnSelectedIndexChanged);
			list.paintMessage -= new ControlListView.OnEventPaintMessage(OnPaintMessage);

			ControlListViewHeader head;
			bool					bChange = false;
			for(int i = 0; i < list.header.Count; i++) 
			{
				head = (ControlListViewHeader)list.header[i];
				if(headWidth[i] == head.width) continue;
				headWidth[i] = head.width;
				bChange = true;
			}
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "ViewListAlarm");

			ringForm.pop(this);
		}

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
		

		

		
	}
}
