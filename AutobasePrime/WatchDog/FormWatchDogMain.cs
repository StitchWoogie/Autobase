using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Globalization;
using NetTools;
using AutoLibLocal;

namespace WatchDog
{
	/// <summary>
	/// Summary description for FormWatchDogMain. 
	/// </summary>
	/// 
	public class FormWatchDogMain : System.Windows.Forms.Form 
	{
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.Timer timerMain;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItemProperties;
		private System.Windows.Forms.MenuItem menuItemRunProgram;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemViewLog;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.ContextMenu contextMenuNotify;
		private System.Windows.Forms.MenuItem menuItemNotityShow;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemNotifyExit;
		private System.Windows.Forms.MenuItem menuItemFileProperty;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemFile;
        private MenuItem menuItem2;
        private MenuItem menuItem4;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
		private System.Windows.Forms.MenuItem menuItemViewHide;

        public static bool bCloseByCallEditor = false;
        private MenuItem menuItem7;
        private MenuItem menuItemConfig;
        public static FormWatchDogMain formMain = null;

		public FormWatchDogMain()
		{
			//
			// Required for Windows Form Designer support
			//
            formMain = this;
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWatchDogMain));
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemRunProgram = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemProperties = new System.Windows.Forms.MenuItem();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemFileProperty = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemViewLog = new System.Windows.Forms.MenuItem();
            this.menuItemViewHide = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItemConfig = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenu();
            this.menuItemNotityShow = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemNotifyExit = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // m_list
            // 
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader1});
            this.m_list.ContextMenu = this.contextMenu1;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemRunProgram,
            this.menuItem1,
            this.menuItemProperties});
            // 
            // menuItemRunProgram
            // 
            this.menuItemRunProgram.Index = 0;
            resources.ApplyResources(this.menuItemRunProgram, "menuItemRunProgram");
            this.menuItemRunProgram.Click += new System.EventHandler(this.menuItemRunProgram_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItemProperties
            // 
            this.menuItemProperties.Index = 2;
            resources.ApplyResources(this.menuItemProperties, "menuItemProperties");
            this.menuItemProperties.Click += new System.EventHandler(this.menuItemProperties_Click);
            // 
            // timerMain
            // 
            this.timerMain.Interval = 200;
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItem3,
            this.menuItem2});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFileProperty,
            this.menuItem6,
            this.menuItemExit});
            resources.ApplyResources(this.menuItemFile, "menuItemFile");
            this.menuItemFile.Popup += new System.EventHandler(this.menuItemFile_Popup);
            // 
            // menuItemFileProperty
            // 
            this.menuItemFileProperty.Index = 0;
            resources.ApplyResources(this.menuItemFileProperty, "menuItemFileProperty");
            this.menuItemFileProperty.Click += new System.EventHandler(this.menuItemFileProperty_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 1;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 2;
            resources.ApplyResources(this.menuItemExit, "menuItemExit");
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemViewLog,
            this.menuItemViewHide,
            this.menuItem7,
            this.menuItemConfig});
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuItemViewLog
            // 
            this.menuItemViewLog.Index = 0;
            resources.ApplyResources(this.menuItemViewLog, "menuItemViewLog");
            this.menuItemViewLog.Click += new System.EventHandler(this.menuItemViewLog_Click);
            // 
            // menuItemViewHide
            // 
            this.menuItemViewHide.Index = 1;
            resources.ApplyResources(this.menuItemViewHide, "menuItemViewHide");
            this.menuItemViewHide.Click += new System.EventHandler(this.menuItemViewHide_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 2;
            resources.ApplyResources(this.menuItem7, "menuItem7");
            // 
            // menuItemConfig
            // 
            this.menuItemConfig.Index = 3;
            resources.ApplyResources(this.menuItemConfig, "menuItemConfig");
            this.menuItemConfig.Click += new System.EventHandler(this.menuItemConfig_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 2;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4});
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenu = this.contextMenuNotify;
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            // 
            // contextMenuNotify
            // 
            this.contextMenuNotify.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNotityShow,
            this.menuItem5,
            this.menuItemNotifyExit});
            this.contextMenuNotify.Popup += new System.EventHandler(this.contextMenuNotify_Popup);
            // 
            // menuItemNotityShow
            // 
            this.menuItemNotityShow.Index = 0;
            resources.ApplyResources(this.menuItemNotityShow, "menuItemNotityShow");
            this.menuItemNotityShow.Click += new System.EventHandler(this.menuItemNotityShow_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            resources.ApplyResources(this.menuItem5, "menuItem5");
            // 
            // menuItemNotifyExit
            // 
            this.menuItemNotifyExit.Index = 2;
            resources.ApplyResources(this.menuItemNotifyExit, "menuItemNotifyExit");
            this.menuItemNotifyExit.Click += new System.EventHandler(this.menuItemNotifyExit_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // FormWatchDogMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu;
            this.MinimizeBox = false;
            this.Name = "FormWatchDogMain";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Closed += new System.EventHandler(this.FormWatchDogMain_Closed);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormWatchDogMain_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
        {
#if USE_EXCEPTION_REPORT
			try 
			{
#endif
				LanguageTool.ChangeUICulture();
				WatchDogInfo.Init();

				bool createdNew=false; 
				Mutex gM1 = new Mutex(true,"AutoBaseWatchDogMutex", out createdNew);

				if (createdNew) 
				{
                    Application.AddMessageFilter(new MessageFilter());
					Application.Run(new FormWatchDogMain());
				}
				else 
				{
					Process p = Tools.GetPreviousProcess();
					if(p != null && p.MainWindowHandle != IntPtr.Zero) 
					{
						Win32Function.SetForegroundWindow(p.MainWindowHandle);
					}
				}

				WatchDogInfo.UnInit();

#if USE_EXCEPTION_REPORT
			}
			catch (Exception exception)
			{
				ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);

				dialog.ShowDialog();
			}
#endif
		}

		ArrayList arrayWatchDog = new ArrayList();

        // 실행파일이 존재하는 경우에만 워치독에서 감시한다.
        void AddWatchDogIfFileExists(string title, string process_name, string filename, int default_timer, EnumWatchDogInfo wdi_id)
        {
            string path = String.Format("{0}\\{1}", Application.StartupPath, filename);

            if (!File.Exists(path))
                return;

            WatchDogItem item;

            item = new WatchDogItem();
            
            item.sTitle = title;
            item.sProcessName = process_name;
            item.sFileName = filename;
            item.nTimer = default_timer;
            item.addr = wdi_id;
            arrayWatchDog.Add(item);
        }

		void LoadWatchDogList()
		{
			WatchDogItem item;

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "감시 프로그램";
			else if(Tools.IsLangChinese())
				item.sTitle = "监视程序";
			else
				item.sTitle = "Main Program";
			item.sProcessName = "LocalMain";
			item.sFileName = "LocalMain.exe";
			item.nTimer = 600;
			item.addr = EnumWatchDogInfo.WDI_LocalMain;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "통신 프로그램";
			else if(Tools.IsLangChinese())
				item.sTitle = "通讯程序";
			else
				item.sTitle = "PLC_SCAN Program";
			item.sProcessName = "Plc_Scan";
			item.sFileName = "Plc_Scan.exe";
			item.nTimer = 60;
			item.addr = EnumWatchDogInfo.WDI_PlcScan;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "네트워크 서버";
			else if(Tools.IsLangChinese())
				item.sTitle = "网络服务器";
			else
				item.sTitle = "Network Server";
			item.sProcessName = "NetServer";
			item.sFileName = "NetServer.exe";
			item.nTimer = 60;
			item.addr = EnumWatchDogInfo.WDI_NetServ;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "네트워크 클라이언트";
			else if(Tools.IsLangChinese())
				item.sTitle = "网络客户";
			else
				item.sTitle = "Network Client";
			item.sProcessName = "NetClient";
			item.sFileName = "NetClient.exe";
			item.nTimer = 60;
			item.addr = EnumWatchDogInfo.WDI_NetClnt;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "데이터 서버";
			else if(Tools.IsLangChinese())
				item.sTitle = "运行程序";
			else
				item.sTitle = "Data Server";
			item.sProcessName = "RunMain";
			item.sFileName = "RunMain.exe";
			item.nTimer = 120;
			item.addr = EnumWatchDogInfo.WDI_RunMain;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "라인 인쇄 관리기";
			else if(Tools.IsLangChinese())
				item.sTitle = "行式打印管理器";
			else
				item.sTitle = "Line Printer";
			item.sProcessName = "LinePrinter";
			item.sFileName = "LinePrinter.exe";
			item.nTimer = 60;
			item.addr = EnumWatchDogInfo.WDI_LinePrinter;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "문자메시지 관리기";
			else if(Tools.IsLangChinese())
				item.sTitle = "短信管理器";
			else
				item.sTitle = "SMS Server";
			item.sProcessName = "SMS";
			item.sFileName = "SMS.exe";
			item.nTimer = 120;
			item.addr = EnumWatchDogInfo.WDI_SMS;
			arrayWatchDog.Add(item);

			item = new WatchDogItem();
			if(Tools.IsLangKorean())
				item.sTitle = "익스포트 서버";
			else if(Tools.IsLangChinese())
				item.sTitle = "导出服务器";
			else
				item.sTitle = "Export Server";

			item.sProcessName = "ExportServer";
			item.sFileName = "ExportServer.exe";
			item.nTimer = 60;
			item.addr = EnumWatchDogInfo.WDI_ExportServer;
			arrayWatchDog.Add(item);

            item = new WatchDogItem();
            if (Tools.IsLangKorean())
                item.sTitle = "OPC 클라이언트";
            else
                item.sTitle = "OPC Client";

            item.sProcessName = "OpcClient";
            item.sFileName = "OpcClient.exe";
            item.nTimer = 60;
            item.addr = EnumWatchDogInfo.WDI_OpcClient;
            arrayWatchDog.Add(item);

            AddWatchDogIfFileExists("Data Sync", "DataSync", "DataSync.exe", 60, EnumWatchDogInfo.WDI_DataSync);

            //AddWatchDogIfFileExists("Autobase OPC UA Client", "OpcUAClient", "OpcUAClient\\OpcUAClient.exe", 60, EnumWatchDogInfo.WDI_OpcUAClient);

            //AddWatchDogIfFileExists("Autobase OPC UA Server", "OpcUAServer", "OpcUAServer\\OpcUAServer.exe", 60, EnumWatchDogInfo.WDI_OpcUAServer); //OPC UA 추가 hsjeong 24-11-27

            //AddWatchDogIfFileExists("REST API Client", "RESTAPIClient", "RESTAPIClient.exe", 60, EnumWatchDogInfo.WDI_RESTAPIClient); //RESTAPIClient 추가 250106 PSU

            AddWatchDogIfFileExists("Autobase OPCUA Client", "Autobase_OPCUA_Client", "Autobase_OPCUA_Client.exe", 60, EnumWatchDogInfo.WDI_OpcUAClient); //260226 PSU 추가.

            int i;
			string retn;

			for(i = 0; i < arrayWatchDog.Count; i++) 
			{
				item = (WatchDogItem)arrayWatchDog[i];
				retn = TotalConfig.LoadRegAutoBaseConfig("WatchDog", item.sProcessName, "Timer", item.nTimer.ToString());
				try 
				{
					item.nTimer = ConvertTool.ToInt32(retn);
				}
				catch 
				{

				}
				retn = TotalConfig.LoadRegAutoBaseConfig("WatchDog", item.sProcessName, "Active", "false");
				try 
				{
					item.bActive = ConvertTool.ToBoolean(retn);	
				}
				catch 
				{

				}
			}
		}

		void SaveWatchDogList()
		{
			WatchDogItem item;

			int i;

			for(i = 0; i < arrayWatchDog.Count; i++) 
			{
				item = (WatchDogItem)arrayWatchDog[i];
				TotalConfig.SaveRegAutoBaseConfig("WatchDog", item.sProcessName, "Timer", item.nTimer.ToString());
				TotalConfig.SaveRegAutoBaseConfig("WatchDog", item.sProcessName, "Active", item.bActive.ToString());
			}
		}

		private void FormMain_Load(object sender, System.EventArgs e)
		{
            WatchDogConfig.Load();

            if (WatchDogConfig.bHideOnStartUp)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
            }

			LoadWatchDogList();

			WatchDogItem itemw;
			ListViewItem iteml;

			int i;

			for(i = 0; i < arrayWatchDog.Count; i++) 
			{
				itemw = (WatchDogItem)arrayWatchDog[i];

				iteml = new ListViewItem(itemw.sTitle);
				iteml.SubItems.Add(itemw.nTimer.ToString()+" sec");
				iteml.SubItems.Add("");	// current time
				iteml.SubItems.Add("Not running");
				iteml.SubItems.Add(itemw.bActive.ToString());
				
				m_list.Items.Add(iteml);

				UpdateList(itemw, i);
			}

			timerMain.Enabled = true;

			string log_msg = String.Format("WatchDog Started (Version:{0})", Application.ProductVersion);
			WriteLog(log_msg);
		}

		void UpdateList(WatchDogItem itemw, int pos)
		{
			ListViewItem item;

			item = m_list.Items[pos];

			if(itemw.bRunning) 
			{
                //if(itemw.bActive)
                //    item.SubItems[2].Text = WatchDogInfo.GetTimer(itemw.addr).ToString();
                if (itemw.bActive)
                {
                    //if (itemw.addr == EnumWatchDogInfo.WDI_OpcUAClient || itemw.addr == EnumWatchDogInfo.WDI_OpcUAServer) //24-09-03 hsjeong OPC UA 위하여 추가 24-11-26 OPC UA Server 추가 hsjeong
                    //{
                    //    item.SubItems[2].Text = TotalConfig.LoadRegAutoBaseConfig("WatchDog", itemw.sProcessName, "val", 0).ToString();
                    //}

                    //else item.SubItems[2].Text = WatchDogInfo.GetTimer(itemw.addr).ToString();

                    item.SubItems[2].Text = WatchDogInfo.GetTimer(itemw.addr).ToString(); //20260226 PSU 수정
                }
				else
					item.SubItems[2].Text = "";

				item.SubItems[3].Text = itemw.dtStart.ToString();
			}
			else 
			{
				item.SubItems[2].Text = "";
				if(Tools.IsLangKorean()) 
				{
					item.SubItems[3].Text = "정지중";
				}
				else if(Tools.IsLangJapanese()) 
				{
					item.SubItems[3].Text = "停止中"; 
				}
				else if(Tools.IsLangChinese()) 
				{
					item.SubItems[3].Text = "停止";
				}
				else 
					item.SubItems[3].Text = "Not running";

			}

			if(Tools.IsLangKorean()) 
			{
				item.SubItems[1].Text = itemw.nTimer.ToString()+" 초";
				item.SubItems[4].Text = itemw.bActive ? "사용" : "미사용";
			}
			else if(Tools.IsLangJapanese()) 
			{
				item.SubItems[1].Text = itemw.nTimer.ToString()+" 秒";
				item.SubItems[4].Text = itemw.bActive ? "使用" : "未使用";
			}
			else if(Tools.IsLangChinese()) 
			{
				item.SubItems[1].Text = itemw.nTimer.ToString()+" 秒";
				item.SubItems[4].Text = itemw.bActive ? "使用" : "未使用";
			}
			else 
			{
				item.SubItems[1].Text = itemw.nTimer.ToString()+" sec";
				item.SubItems[4].Text = itemw.bActive.ToString();
			}
		}

		void RunProgram(WatchDogItem itemw)
		{
			string filename;

			filename = Application.StartupPath+"\\"+itemw.sFileName;

            if (File.Exists(filename))
            {
                if (String.Compare(itemw.sProcessName, "Plc_Scan", true) == 0)
                    Process.Start(filename, "Minimized=true");
                else 
                    //Process.Start(filename);
                {   try  // OPC UA Client 와 OPC UA Server가 64bit .exe 이므로 32bit PC 에서 Process.Start시 WatchDog 다운됨. 이를 방지하고자 추가. 24-11-27 hsjeong
                    {
                        Process.Start(filename);
                    }
                    catch// (Exception ex)
                    {
                        
                    }
                }
            }
		}

		void WriteLog(string msg)
		{
			DateTime t = DateTime.Now;
			string filename;

			filename = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

			filename += "\\WatchDog";

			if(!Directory.Exists(filename)) 
			{
				try 
				{
					Directory.CreateDirectory(filename);
				}
				catch 
				{

				}
			}

			filename += "\\WatchDog.log";

			TextWriter writer;
			try 
			{
				writer = File.AppendText(filename);
				writer.WriteLine("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}, {6}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, msg);
				writer.Close();
			}
			catch 
			{
			}
		}

		int nPosCheckItem = 0;

		private void timerMain_Tick(object sender, System.EventArgs e)
		{
			RefreshNotifyIcon();

			if(arrayWatchDog.Count == 0)	return;

			nPosCheckItem++;
			nPosCheckItem%=arrayWatchDog.Count;

			WatchDogItem itemw;

			itemw = (WatchDogItem)arrayWatchDog[nPosCheckItem];

            string process_name;

            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    process_name = p.ProcessName;	// 이름을 얻는 도중 핸들이 잘못되었습니다 라는 메시지가 나오는 경우가 있다. 2009-4-28
                }
                catch
                {
                    continue;
                }

                if (String.Compare(itemw.sProcessName, process_name, true) != 0) continue; 

				// 파일이 실행중일 때

				if(!itemw.bRunning) 
				{
                    try
                    {
                        itemw.dtStart = p.StartTime;        // 엑세스가 거부될때가 있다. 2012-8-3 전차장 컴퓨터에서 발생   "액세스가 거부되었습니다"
                    }
                    catch
                    {
                        itemw.dtStart = DateTime.Now;       
                    }

					itemw.bRunning = true;
					itemw.nOldSec = DateTime.Now.Second;

                    WatchDogInfo.SetTimer(itemw.addr, 0); //260226 PSU 수정.
                    //if (itemw.addr == EnumWatchDogInfo.WDI_OpcUAClient || itemw.addr == EnumWatchDogInfo.WDI_OpcUAServer) //24-09-03 hsjeong OPC UA 위하여 추가 24-11-26 OPC UA Server 추가 hsjeong
                    //{
                    //}
                    //else WatchDogInfo.SetTimer(itemw.addr, 0);

					UpdateList(itemw, nPosCheckItem);

					WriteLog(itemw.sTitle+" Checked Run");

					return;
				}
				else 
				{
					if(itemw.bActive) 
					{
						int sec = DateTime.Now.Second;
						int gab = 0;
						if(sec < itemw.nOldSec)		// 분이 지남
							gab = sec+60-itemw.nOldSec;
						else
							gab = sec-itemw.nOldSec;

						itemw.nOldSec = sec;

                        // OPC UA 24-09-02 추가 hsjeong
                        int val = 0;
                        //if (itemw.addr == EnumWatchDogInfo.WDI_OpcUAClient || itemw.addr == EnumWatchDogInfo.WDI_OpcUAServer) //24-09-03 hsjeong OPC UA 위하여 추가 24-11-26 OPC UA Server 추가 hsjeong
                        //{
                        //    val = TotalConfig.LoadRegAutoBaseConfig("WatchDog", itemw.sProcessName, "val", 0);
                        //}
                        //else val = WatchDogInfo.GetTimer(itemw.addr);

                        val = WatchDogInfo.GetTimer(itemw.addr); //20260226 PSU 수정
                        val += gab;

                        //if (itemw.addr == EnumWatchDogInfo.WDI_OpcUAClient || itemw.addr == EnumWatchDogInfo.WDI_OpcUAServer) //24-09-03 hsjeong OPC UA 위하여 추가 24-11-26 OPC UA Server 추가 hsjeong 
                        //{
                        //    TotalConfig.SaveRegAutoBaseConfig("WatchDog", itemw.sProcessName, "val", val);
                        //}
                        //else WatchDogInfo.SetTimer(itemw.addr, val);

                        WatchDogInfo.SetTimer(itemw.addr, val);// 20260226 PSU 수정

                        if (val >= itemw.nTimer)
                        {
                            p.Kill();

                            //if (itemw.addr == EnumWatchDogInfo.WDI_OpcUAClient || itemw.addr == EnumWatchDogInfo.WDI_OpcUAServer) //24-09-03 hsjeong OPC UA 위하여 추가 24-11-26 OPC UA Server 추가 hsjeong
                            //{
                            //    TotalConfig.SaveRegAutoBaseConfig("WatchDog", itemw.sProcessName, "val", 0);
                            //}

                            WriteLog(itemw.sTitle + " Kill Command by WatchDog");
                        }


                        //int val = WatchDogInfo.GetTimer(itemw.addr);
                        //val += gab;
                        //WatchDogInfo.SetTimer(itemw.addr, val);

                        //if(val >= itemw.nTimer) 
                        //{
                        //    p.Kill();
                        //    WriteLog(itemw.sTitle+" Kill Command by WatchDog");
                        //}

						UpdateList(itemw, nPosCheckItem);
					}

				}
				return;
			}

			if(itemw.bRunning) 
			{
				itemw.bRunning = false;
				UpdateList(itemw, nPosCheckItem);

				WriteLog(itemw.sTitle+" Checked Exit");
			}
			else 
			{
				if(itemw.bActive) 
				{
					RunProgram(itemw);
					WriteLog(itemw.sTitle+" Run Command by WatchDog");
				}
			}
		}

		void DialogProperty()
		{
			if(m_list.SelectedItems.Count == 0)	return;

			WatchDogItem itemw;
			ListViewItem iteml;

			iteml = m_list.SelectedItems[0];

			itemw = (WatchDogItem)arrayWatchDog[iteml.Index];

			FormModify dialog = new FormModify();

			dialog.checkBoxActive.Checked = itemw.bActive;
			dialog.numericUpDownTimer.Value = itemw.nTimer;
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				itemw.bActive = dialog.checkBoxActive.Checked;	
				itemw.nTimer = ConvertTool.ToInt32(dialog.numericUpDownTimer.Value);

				UpdateList(itemw, iteml.Index);

				SaveWatchDogList();
			}
		}

		private void menuItemProperties_Click(object sender, System.EventArgs e)
		{
			DialogProperty();
		}

		private void menuItemRunProgram_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0)	return;

			WatchDogItem itemw;
			ListViewItem iteml;

			iteml = m_list.SelectedItems[0];

			itemw = (WatchDogItem)arrayWatchDog[iteml.Index];

			RunProgram(itemw);
		}

		private void FormWatchDogMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			WatchDogItem itemw;
			bool active = false;

			for(int i = 0; i < arrayWatchDog.Count; i++) 
			{
				itemw = (WatchDogItem)arrayWatchDog[i];
				if(itemw.bActive)	active = true;
			}

            if (!bCloseByCallEditor)
            {

                if (active)
                {
                    if (Tools.IsLangKorean())
                    {
                        if (MessageBox.Show("워치독이 현재 프로그램 감시 중입니다.\n워치독을 종료하면 각종 프로그램을 감시할 수 없습니다.\n종료할까요?", "종료확인", MessageBoxButtons.YesNo)
                            != DialogResult.Yes) e.Cancel = true;
                    }
                    else if (Tools.IsLangJapanese())
                    {
                        if (MessageBox.Show("ワッチドッグ(WatchDog)が現在プログラムを監視しています。\nワッチドッグを終了したら各種プログラムの監視が出来ません。\n終了しますか。", "終了確認", MessageBoxButtons.YesNo)
                            != DialogResult.Yes) e.Cancel = true;
                    }
                    else if (Tools.IsLangChinese())
                    {
                        if (MessageBox.Show("监视器正在监控程序。\n如果关闭了监视器，就不能监视各种程序。\n要关闭吗?", "退出确认", MessageBoxButtons.YesNo)
                            != DialogResult.Yes) e.Cancel = true;
                    }
                    else
                    {
                        if (MessageBox.Show("WatchDog is watching program.\nExit this program?", "Program exit", MessageBoxButtons.YesNo)
                            != DialogResult.Yes) e.Cancel = true;
                    }
                }
            }
		}

		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			bExit = true;
			Close();
		}

		private void menuItemViewLog_Click(object sender, System.EventArgs e)
		{
			string filename;

			filename = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);
			filename += "\\WatchDog";
			filename += "\\WatchDog.log";

			Process.Start("notepad", filename);			
		}

		private void FormWatchDogMain_Closed(object sender, System.EventArgs e)
		{
			WriteLog("WatchDog Closed");
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			DialogProperty();		
		}

		private void menuItemNotifyExit_Click(object sender, System.EventArgs e)
		{
			bExit = true;
			Close();
		}

		private void menuItemNotityShow_Click(object sender, System.EventArgs e)
		{
            this.Visible = !this.Visible;
            this.ShowInTaskbar = this.Visible;
		}

		private void contextMenuNotify_Popup(object sender, System.EventArgs e)
		{
			menuItemNotityShow.Checked = this.Visible;
		}

		int  nNotifyOldSec;
		int  nNotifyIconView;

		Icon iconNotify1 = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WatchDog.Notify1.ico"));
		Icon iconNotify2 = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WatchDog.Notify2.ico"));
		Icon iconNotify3 = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WatchDog.Notify3.ico"));

		void RefreshNotifyIcon()
		{
			DateTime t = DateTime.Now;

			if(t.Second != nNotifyOldSec) 
			{
				nNotifyOldSec = t.Second;
				nNotifyIconView ++;
				nNotifyIconView %= 3;

				if(nNotifyIconView == 1)		notifyIcon1.Icon = iconNotify1;
				else if(nNotifyIconView == 2)	notifyIcon1.Icon = iconNotify2;
				else							notifyIcon1.Icon = iconNotify3;
			}
		}

		private void menuItemViewHide_Click(object sender, System.EventArgs e)
		{
			this.Visible = false;
            this.ShowInTaskbar = false;
		}

		private void notifyIcon1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)	return;

			this.Visible = true;
            this.ShowInTaskbar = true;
		}

		private void menuItemFileProperty_Click(object sender, System.EventArgs e)
		{
			DialogProperty();		
		}

		private void menuItemFile_Popup(object sender, System.EventArgs e)
		{
			this.menuItemFileProperty.Enabled = (m_list.SelectedItems.Count > 0);
		}

        private void menuItem4_Click(object sender, EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();

            dialog.ProgramIcon = this.Icon;
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
        }

        private void menuItemConfig_Click(object sender, EventArgs e)
        {
            FormConfig form = new FormConfig();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        bool systemShutDown = false;

        protected override void WndProc(ref Message m)
        {
            int WM_QUERYENDSESSION = 0x11;

            if (m.Msg == WM_QUERYENDSESSION)
            {
                systemShutDown = true;
            }

            base.WndProc(ref m);
        }

        bool bExit = false;

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bExit || systemShutDown)	// 종료를 눌렀을 때나 윈도우 메뉴의 다시 시작이나 종료일 때만 
            {

            }
            else if (bCloseByCallEditor)    // 편집기에서 종료했을 때
            {

            }
            else
            {
                this.Visible = false;
                e.Cancel = true;
            }
        }

        private void m_list_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
	}

    /// <summary>
    /// MainForm에 override 할때는 잘되지 않고 메시지 필터를 Application.AddMessageFilter(new MessageFilter()); 와 같이 등록에서 사용하면 잘 된다.
    /// </summary>
    public class MessageFilter : System.Windows.Forms.IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x111: // WM_COMMAND
                    if (m.WParam == new IntPtr((int)EnumIdmPublic.IDM_PUBLIC_DESTROY_WINDOW) && m.LParam == new IntPtr(12345678))
                    {
                        FormWatchDogMain.bCloseByCallEditor = true;
                        FormWatchDogMain.formMain.Close();
                    }
                    break;
            }

            return false;

        }

    }
}
