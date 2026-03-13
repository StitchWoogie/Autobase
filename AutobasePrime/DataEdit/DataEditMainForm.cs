using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using AutoLibLocal;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using NetTools;
using System.IO;
using AutoLib;

namespace DataEdit
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class DataEditMainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu DataEditMenu;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItemFileAI;
		private System.Windows.Forms.MenuItem menuItemFileDi;
		private System.Windows.Forms.MenuItem menuItemFileExit;
		private System.Windows.Forms.MenuItem menuItem_ConfigMain;
		private System.Windows.Forms.MenuItem menuItem_Config;
		private System.Windows.Forms.MenuItem menuItem_FileMain;
		private System.Windows.Forms.MenuItem menuItem_WindowCascade;
		private System.Windows.Forms.MenuItem menuItem_WindowTile;
		private System.Windows.Forms.MenuItem menuItem_WindowTileVert;
		private System.Windows.Forms.MenuItem menuItem_WindowArrange;
		private System.Windows.Forms.MenuItem menuItem_WindowClose;
		private System.Windows.Forms.MenuItem menuItem_WindowCloseAll;
		private System.Windows.Forms.MenuItem menuItem_WindowMain;
		private System.Windows.Forms.MenuItem menuItem_HelpMain;
        private System.Windows.Forms.MenuItem menuItem_HelpDataEdit;
        private IContainer components;

		public DataEditMainForm()
		{
			//
			// Required for Windows Form Designer support
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataEditMainForm));
            this.DataEditMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem_FileMain = new System.Windows.Forms.MenuItem();
            this.menuItemFileAI = new System.Windows.Forms.MenuItem();
            this.menuItemFileDi = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemFileExit = new System.Windows.Forms.MenuItem();
            this.menuItem_ConfigMain = new System.Windows.Forms.MenuItem();
            this.menuItem_Config = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowMain = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowCascade = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowTile = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowTileVert = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowArrange = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowClose = new System.Windows.Forms.MenuItem();
            this.menuItem_WindowCloseAll = new System.Windows.Forms.MenuItem();
            this.menuItem_HelpMain = new System.Windows.Forms.MenuItem();
            this.menuItem_HelpDataEdit = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // DataEditMenu
            // 
            this.DataEditMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_FileMain,
            this.menuItem_ConfigMain,
            this.menuItem_WindowMain,
            this.menuItem_HelpMain});
            // 
            // menuItem_FileMain
            // 
            this.menuItem_FileMain.Index = 0;
            this.menuItem_FileMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFileAI,
            this.menuItemFileDi,
            this.menuItem4,
            this.menuItemFileExit});
            resources.ApplyResources(this.menuItem_FileMain, "menuItem_FileMain");
            // 
            // menuItemFileAI
            // 
            this.menuItemFileAI.Index = 0;
            resources.ApplyResources(this.menuItemFileAI, "menuItemFileAI");
            this.menuItemFileAI.Click += new System.EventHandler(this.menuItemFileAI_Click);
            // 
            // menuItemFileDi
            // 
            this.menuItemFileDi.Index = 1;
            resources.ApplyResources(this.menuItemFileDi, "menuItemFileDi");
            this.menuItemFileDi.Click += new System.EventHandler(this.menuItemFileDi_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // menuItemFileExit
            // 
            this.menuItemFileExit.Index = 3;
            resources.ApplyResources(this.menuItemFileExit, "menuItemFileExit");
            this.menuItemFileExit.Click += new System.EventHandler(this.menuItemFileExit_Click);
            // 
            // menuItem_ConfigMain
            // 
            this.menuItem_ConfigMain.Index = 1;
            this.menuItem_ConfigMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_Config});
            resources.ApplyResources(this.menuItem_ConfigMain, "menuItem_ConfigMain");
            // 
            // menuItem_Config
            // 
            this.menuItem_Config.Index = 0;
            resources.ApplyResources(this.menuItem_Config, "menuItem_Config");
            this.menuItem_Config.Click += new System.EventHandler(this.menuItem_Config_Click);
            // 
            // menuItem_WindowMain
            // 
            this.menuItem_WindowMain.Index = 2;
            this.menuItem_WindowMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_WindowCascade,
            this.menuItem_WindowTile,
            this.menuItem_WindowTileVert,
            this.menuItem_WindowArrange,
            this.menuItem_WindowClose,
            this.menuItem_WindowCloseAll});
            resources.ApplyResources(this.menuItem_WindowMain, "menuItem_WindowMain");
            // 
            // menuItem_WindowCascade
            // 
            this.menuItem_WindowCascade.Index = 0;
            resources.ApplyResources(this.menuItem_WindowCascade, "menuItem_WindowCascade");
            this.menuItem_WindowCascade.Click += new System.EventHandler(this.menuItem_WindowCascade_Click);
            // 
            // menuItem_WindowTile
            // 
            this.menuItem_WindowTile.Index = 1;
            resources.ApplyResources(this.menuItem_WindowTile, "menuItem_WindowTile");
            this.menuItem_WindowTile.Click += new System.EventHandler(this.menuItem_WindowTile_Click);
            // 
            // menuItem_WindowTileVert
            // 
            this.menuItem_WindowTileVert.Index = 2;
            resources.ApplyResources(this.menuItem_WindowTileVert, "menuItem_WindowTileVert");
            this.menuItem_WindowTileVert.Click += new System.EventHandler(this.menuItem_WindowTileVert_Click);
            // 
            // menuItem_WindowArrange
            // 
            this.menuItem_WindowArrange.Index = 3;
            resources.ApplyResources(this.menuItem_WindowArrange, "menuItem_WindowArrange");
            this.menuItem_WindowArrange.Click += new System.EventHandler(this.menuItem_WindowArrange_Click);
            // 
            // menuItem_WindowClose
            // 
            this.menuItem_WindowClose.Index = 4;
            resources.ApplyResources(this.menuItem_WindowClose, "menuItem_WindowClose");
            this.menuItem_WindowClose.Click += new System.EventHandler(this.menuItem_WindowClose_Click);
            // 
            // menuItem_WindowCloseAll
            // 
            this.menuItem_WindowCloseAll.Index = 5;
            resources.ApplyResources(this.menuItem_WindowCloseAll, "menuItem_WindowCloseAll");
            this.menuItem_WindowCloseAll.Click += new System.EventHandler(this.menuItem_WindowCloseAll_Click);
            // 
            // menuItem_HelpMain
            // 
            this.menuItem_HelpMain.Index = 3;
            this.menuItem_HelpMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_HelpDataEdit});
            resources.ApplyResources(this.menuItem_HelpMain, "menuItem_HelpMain");
            // 
            // menuItem_HelpDataEdit
            // 
            this.menuItem_HelpDataEdit.Index = 0;
            resources.ApplyResources(this.menuItem_HelpDataEdit, "menuItem_HelpDataEdit");
            this.menuItem_HelpDataEdit.Click += new System.EventHandler(this.menuItem_HelpDataEdit_Click);
            // 
            // DataEditMainForm
            // 
            resources.ApplyResources(this, "$this");
            this.IsMdiContainer = true;
            this.Menu = this.DataEditMenu;
            this.Name = "DataEditMainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
//#if USE_EXCEPTION_REPORT
//			try 
//			{
//#endif
				LanguageTool.ChangeUICulture();

				bool createdNew=false; 
				Mutex gM1 = new Mutex(true,"AutoBaseDataEditMutex", out createdNew);

				if (createdNew) 
				{
					Application.Run(new DataEditMainForm());
				}
				else 
				{
					Process p = Tools.GetPreviousProcess();
					if(p != null && p.MainWindowHandle != IntPtr.Zero) 
					{
						Win32Function.SetForegroundWindow(p.MainWindowHandle);
					}
				}		
//#if USE_EXCEPTION_REPORT				
//			}
//			catch (Exception exception)
//			{
//				ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);
//
//				dialog.ShowDialog();
//			}
//#endif
		}

		private void menuItemFileExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		void callAiWindow()
		{
			ViewAiTag		aiMain = new ViewAiTag(null);

			aiMain.MdiParent = this;
			aiMain.Show();
		}

		void callDiWindow()
		{
			ViewDiTag		diMain = new ViewDiTag(null);

			diMain.MdiParent = this;
			diMain.Show();
		}

		private void menuItemFileAI_Click(object sender, System.EventArgs e)
		{
			callAiWindow();
		}

		private void menuItemFileDi_Click(object sender, System.EventArgs e)
		{
			callDiWindow();
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			TerminalClass.Init();
			DataEditConfig.dTime = DateTime.Now;
			DataEditTools.loadDataEditConfigData();
			callDiWindow();
			callAiWindow();
		}

		
		private void menuItem_Config_Click(object sender, System.EventArgs e)
		{
			DataEditTools.callDataEditConfigDialog();
		}

		private void menuItem_WindowCascade_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade);
		}

		private void menuItem_WindowTile_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal);
		}

		private void menuItem_WindowTileVert_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi(System.Windows.Forms.MdiLayout.TileVertical);
		}

		private void menuItem_WindowArrange_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi(System.Windows.Forms.MdiLayout.ArrangeIcons);
		}

		private void menuItem_WindowClose_Click(object sender, System.EventArgs e)
		{
			if(this.ActiveMdiChild != null)
				this.ActiveMdiChild.Close();
		}

		private void menuItem_WindowCloseAll_Click(object sender, System.EventArgs e)
		{
			Form[] childForm = this.MdiChildren; 
			//Make sure to ask for saving the doc before exiting the app 

			for(int i=0; i < childForm.Length ; i++) 
				childForm[i].Close();
		}

		private void menuItem_HelpDataEdit_Click(object sender, System.EventArgs e)
		{
			DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();
			dialog.ProgramIcon = this.Icon;
			dialog.ShowDialog(this);
		}

		private void Form1_Closed(object sender, System.EventArgs e)
		{
			DataEditTools.saveDataEditConfigData();
		}
	}

	//static public DataEditConig editConfig;
	
	public class DataEditConfig
	{
		static public bool		bSaveWriteReason;		// 데이터 수정이유를 MDB 파일에 보관할 지의 여부
		static public DateTime	dTime;					// 자료읽기 기준시간
		static public int		nMaxTickCount = 5;		// 날짜/시간을 변경 후 데이터를 읽기 위한 시간 , 기본 = 5 * 100 mSec
		static public string	sDirData = TotalConfig.GetProjectDataDirectory();// 자료저장 폴더
		static public string	sSaveFilename = "modify_history.MDB"; // 자료저장 파일이름
		static public int		nTextSaveLoadCount = 24;//파일 저장/읽기 개수
		static public int		nTextSaveLoadType = 0;	//파일 저장/읽기 형식, 0 = 분자료, 1 = 시간자료
		static public string	sTextFilename = "c:\\catdata\\conv00.txt";//파일 저장/읽기 파일이름
		static public int		nBasicEditMode = 0;		// 기본 데이터 수정모드, 0= 분 자료수정, 1 = 시간 자료수정, 2 = 파일 저장/읽기 
		static public Font		fEditFont = new Font("굴림", 9.75F);// AI/DI 리스트의 글꼴
		//static public string	sProgramDir;			// 프로그램 실행 폴더
		//static public ConnectionString dsn = new ConnectionString();// 자료수정 이유 저장을 위한 데이터 베이스 원본이름
		
	}

}
