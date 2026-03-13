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
	/// Summary description for ViewEventLogMain.
	/// </summary>
	public class ViewEventLogMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu contextMenuEventLog;
		private System.Windows.Forms.MenuItem menuItem_view_event_log;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;

		//BasicScreen.kdymain.ViewEventLog child;
		DataSet					ds;
		int[]					headWidth = new int[3];
		ControlListView			list = new ControlListView();
		//int			TagHap, currPos;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.MenuItem menuItem_DeleteLogFile;


		public ViewEventLogMain(int pos)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			//work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			//AdInitDisplayCurrentColumnWidthLog();
			//child = new ViewEventLog(work.xNum, pos);
			//this.Controls.Add(child);
			this.panel2.Controls.Add(list);
			BasicElementSetting();

			this.Load += async(sender,e) => await GetLogListAndHap();
			list.currPos = pos;
			if(list.currPos < 0 || list.currPos >= list.listHap) list.currPos = list.listHap-1;

			headWidth[0] = 80;
			headWidth[1] = 250;
			headWidth[2] = 250;
		}

		async Task GetLogListAndHap()
		{
			try
			{
				DataGate gate = new DataGate();
				ds = await gate.GetLogLists();

				if (ds == null) list.listHap = 0;
				else list.listHap = ds.Tables[0].Rows.Count;

                this.Refresh(); //251030 PSU 추가. 데이터 로드 지연 시 다시 paint
            }
			catch(Exception ex)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewEventLogMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.contextMenuEventLog = new System.Windows.Forms.ContextMenu();
            this.menuItem_view_event_log = new System.Windows.Forms.MenuItem();
            this.menuItem_DeleteLogFile = new System.Windows.Forms.MenuItem();
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
            // contextMenuEventLog
            // 
            this.contextMenuEventLog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_view_event_log,
            this.menuItem_DeleteLogFile,
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem_view_event_log
            // 
            this.menuItem_view_event_log.Index = 0;
            resources.ApplyResources(this.menuItem_view_event_log, "menuItem_view_event_log");
            this.menuItem_view_event_log.Click += new System.EventHandler(this.menuItem_view_event_log_Click);
            // 
            // menuItem_DeleteLogFile
            // 
            this.menuItem_DeleteLogFile.Index = 1;
            resources.ApplyResources(this.menuItem_DeleteLogFile, "menuItem_DeleteLogFile");
            this.menuItem_DeleteLogFile.Click += new System.EventHandler(this.menuItem_DeleteLogFile_Click);
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
            // ViewEventLogMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "ViewEventLogMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Closed += new System.EventHandler(this.ViewEventLogMain_Closed);
            this.Load += new System.EventHandler(this.ViewEventLogMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewEventLogMain_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewEventLogMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		
		int GetSelectedPos()
		{
			return list.currPos;
			//if(this.listView1.SelectedItems == null || this.listView1.SelectedItems.Count <= 0) return -1;
			//return this.listView1.SelectedIndices[0];
		}

		void SetDetailButtonEnableDisable()		// 상세 버턴을 Enable/Disable
		{
			if(GetSelectedPos() == -1) 
			{
				toolBarButton2.Enabled = false;
				toolBarButton3.Enabled = false;
				menuItem_view_event_log.Enabled = false;
				menuItem_DeleteLogFile.Enabled = false;
			}
			else 
			{
				toolBarButton2.Enabled = true;
				menuItem_view_event_log.Enabled = true;
                toolBarButton3.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_DELETE_LOG_FILE);//log file delete
                menuItem_DeleteLogFile.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_DELETE_LOG_FILE);//log file delete
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
			this.BackColor = SharedData.colorTotal.BACK;				// 기본화면의 배경색상, list visable =
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
			SetDetailButtonEnableDisable();
		}

		void OnListMouse_DoubleClicked()
		{
			CallEventLogDetailWindows();
		}

		void OnSelectedIndexChanged()
		{
			SetDetailButtonEnableDisable();
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
			//	data = String.Format("{0}{1}{2}{3}년 {4}{5}월 {6}{7}일 로그", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
			//else if(Tools.IsLangJapanese())
			//	data = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 ログ", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
			//else if(Tools.IsLangChinese())
			//	data = String.Format("{0}{1}{2}{3}年 {4}{5}月 {6}{7}日 日志", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
			//else
			//	data = String.Format("{0}{1}{2}{3}/{4}{5}/{6}{7} LOG Data", data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]);
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
		
		private void ViewEventLogMain_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged += new SharedViewMain.DelegatePublic(OnColorChanged);
			SharedViewMain.EventListUserChanged += new SharedViewMain.DelegatePublic(OnUserChanged);

			//TotalConfig.AutoBaseListCtrlConfigLoad(this.listView1, "BasicScreen", "ViewEventLog");
			//fillListView(currPos);
			list.doubleClick += new ControlListView.OnEventDoubleClick(OnListMouse_DoubleClicked);
			list.selectedIndexChanged += new ControlListView.OnEventSelectedIndexChanged(OnSelectedIndexChanged);
			list.paintMessage += new ControlListView.OnEventPaintMessage(OnPaintMessage);
			            			
			TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "ViewEventLog");
			
			list.bOwnerDraw = true;
			ListHeaderFill();
			list.Show();
			FillListDataAll();
			this.panel1.Height = this.toolBar1.Height;			// 초기크기를 설정
			ringForm.push(this);
		}

		void ListHeaderFill()
		{
			ControlListViewHeader	head;
			string[]				text = new string[3];

            if (Tools.IsLangKorean())
            {
                text[0] = "순서";
                text[1] = "로그날짜";
                text[2] = "로그개수";
            }
            else if (Tools.IsLangJapanese())
            {
                text[0] = "No";
                text[1] = "ログ日付";
                text[2] = "ログ件数";   // 파일명 → 로그개수				
            }
            else if (Tools.IsLangChinese())
            {
                text[0] = "顺序";
                text[1] = "日志日期";
                text[2] = "日志数量";   // 文件名 → 日志数量				
            }
            else if (Tools.IsLangVietnamese())
            {
                text[0] = "Số";
                text[1] = "Ngày đăng nhập";
                text[2] = "Số lượng bản ghi";  // Tên tệp → 로그개수
            }
            else
            {
                text[0] = "No";
                text[1] = "Log Date";
                text[2] = "Log Count";   // Filename → Log Count
            }

            for (int i = 0; i < 3; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();
				head.format.Alignment = StringAlignment.Center;
				list.header.Add(head);
			}
		}

		void BasicElementSetting()
		{
			list.font = ConfigViewMain.fontMain;
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;	// 유효하지 않은태그일 경우의 색상으로 사용

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.toolBar1.Buttons.Remove(toolBarButton3);   // delete tool bar
            }
            else
            {
                list.contextMenu = this.contextMenuEventLog;
            }
		}

		void FillListDataAll()
		{
			if(ds == null) list.listHap = 0;
			else list.listHap = ds.Tables[0].Rows.Count;
			list.listItemChanged();
			SetDetailButtonEnableDisable();
		}


		void CallEventLogDetailWindows()
		{
			if(list.listHap <= 0 || list.currPos < 0 || list.listHap <= list.currPos) return;
			
			DataRow				row = ds.Tables[0].Rows[list.currPos];			

			ViewEventLogDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewEventLogDetailMain(row[0].ToString(), -1), ConfigViewMain.nMdiCountOnBasicScreen);
		}

        private readonly SemaphoreSlim _deleteLogLock = new SemaphoreSlim(1, 1);

        async Task SelectedEventLogFileDelete()
		{
            if (!await _deleteLogLock.WaitAsync(0))
                return;

			try
			{
				if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_DELETE_LOG_FILE)) return;

				if (list.currPos >= list.listHap || list.currPos < 0) return;
				DataRow row = ds.Tables[0].Rows[list.currPos];
				if (row == null) return;

				string filename, caption;
				filename = string.Format("{0}\\log\\{1}", TotalConfig.GetProjectDataDirectory(), row[0].ToString());

				if (Tools.IsLangKorean()) caption = "이 파일을 삭제할까요?";
				else if (Tools.IsLangJapanese()) caption = "このファイルを削除しますか。";
				else if (Tools.IsLangChinese()) caption = "要删除此文件吗？";
				else caption = "Are you sure you want to delete the selected Log File?";

				if (MessageBox.Show(filename, caption, MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					try
					{
						if (System.IO.File.Exists(filename))
						{
							System.IO.File.Delete(filename);
							await GetLogListAndHap();
							//getLogListAndHap();
							if (list.currPos >= list.listHap) list.currPos = list.listHap - 1;
							FillListDataAll();
							//this.listView1.Items.RemoveAt(pos);
							//setItemPosNoReArrange();						// 순서번호를 다시정렬
							//setListItemPos(pos);
						}
					}
					catch { }
				}
			}
			finally
			{
				_deleteLogLock.Release();
			}
        }
		
		

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetDetailButtonEnableDisable();
		}

		private void menuItem_view_event_log_Click(object sender, System.EventArgs e)
		{
			CallEventLogDetailWindows();
		}

		private async void menuItem_DeleteLogFile_Click(object sender, System.EventArgs e)
		{
			await SelectedEventLogFileDelete();
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			CallEventLogDetailWindows();
		} 


		private async void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) 
			{
				this.Close();
				return;
			}
			if(e.Button == toolBarButton2)
			{
				CallEventLogDetailWindows();
				return;
			}
			if(e.Button == toolBarButton3)
			{
				await SelectedEventLogFileDelete();
				return;
			}
		}

		
		private async void ViewEventLogMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
				case Keys.Enter : CallEventLogDetailWindows(); return;
				case Keys.Delete : await SelectedEventLogFileDelete(); return;
			}
			//if(child.mainArrowKeyOperation(e.KeyCode)) return;		
		}

		private void ViewEventLogMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
			//this.listView1.Height = this.ClientSize.Height-this.toolBar1.Height;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewEventLogMain_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			SharedViewMain.EventListUserChanged -= new SharedViewMain.DelegatePublic(OnUserChanged);

			//TotalConfig.AutoBaseListCtrlConfigSave(this.listView1, "BasicScreen", "ViewEventLog");
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
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "ViewEventLog");
			ringForm.pop(this);
		}

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		

		

		
		
		
	}
}
