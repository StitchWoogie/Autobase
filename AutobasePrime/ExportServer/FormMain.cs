using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using NetTools;
using AutoLib;
using AutoLibLocal;

namespace ExportServer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

        public static bool bCloseByCallEditor = false;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuNotify;
        private ToolStripMenuItem showToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItemExit;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem listToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem hideToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem configToolStripMenuItem;
        public static FormMain formMain = null;

		public FormMain()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuNotify.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuNotify;
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            // 
            // contextMenuNotify
            // 
            this.contextMenuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItemExit});
            this.contextMenuNotify.Name = "contextMenuNotify";
            resources.ApplyResources(this.contextMenuNotify, "contextMenuNotify");
            this.contextMenuNotify.Opened += new System.EventHandler(this.contextMenuNotify_Opened);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            resources.ApplyResources(this.showToolStripMenuItem, "showToolStripMenuItem");
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            resources.ApplyResources(this.toolStripMenuItemExit, "toolStripMenuItemExit");
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.toolStripMenuItemExit_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.toolStripSeparator2,
            this.configToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            resources.ApplyResources(this.listToolStripMenuItem, "listToolStripMenuItem");
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            resources.ApplyResources(this.hideToolStripMenuItem, "hideToolStripMenuItem");
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            this.configToolStripMenuItem.Click += new System.EventHandler(this.configToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Closed += new System.EventHandler(this.FormMain_Closed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.contextMenuNotify.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
				AutoLibLocal.LanguageTool.ChangeUICulture();

				bool createdNew=false; 
				Mutex gM1 = new Mutex(true,"AutoBaseExportServerMutex", out createdNew);
	                                         
				if (createdNew) 
				{
                    Application.AddMessageFilter(new MessageFilter());
					Application.Run(new FormMain());
				}
				else 
				{
					Process p = Tools.GetPreviousProcess();
					if(p != null && p.MainWindowHandle != IntPtr.Zero) 
					{
						Win32Function.SetForegroundWindow(p.MainWindowHandle);
					}
				}
#if USE_EXCEPTION_REPORT				
			}
			catch (Exception exception)
			{
				ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);

				dialog.ShowDialog();
			}
#endif
		}

		private void FormMain_Closed(object sender, System.EventArgs e)
		{
			foreach(Form child in this.MdiChildren) 
			{
				child.Close();
			}

			ServerList.UnInitAll();
			AutoLibLocal.WatchDogInfo.UnInit();
		}

		private void FormMain_Load(object sender, System.EventArgs e)
		{
            ExportServerConfig.Load();

            if (ExportServerConfig.bHideOnStartUp)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
            }

			DriverList.Load();
			ServerList.Load();
			ServerList.InitAll();
			AutoLibLocal.WatchDogInfo.Init();

            //20241010
            TerminalClass.Init();
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			AutoLibLocal.WatchDogInfo.SetTimer(AutoLibLocal.EnumWatchDogInfo.WDI_ExportServer, 0);
		}

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            this.Visible = true;
            this.ShowInTaskbar = true;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
            this.ShowInTaskbar = this.Visible;
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            bExit = true;
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bExit = true;
            Close();
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form exist;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                exist = this.MdiChildren[i];

                if (exist.Name == "FormList")
                {

                    exist.Activate();	// ?? form.Focus() 도 확실하지 않음

                    if (exist.WindowState == FormWindowState.Minimized)
                        exist.WindowState = FormWindowState.Normal;
                    return;
                }
            }

            FormList form = new FormList();

            form.MdiParent = this;
            form.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();

            dialog.ProgramIcon = this.Icon;
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            //this.ShowInTaskbar = false; //2024-12-13 PSU Win10/11 에서 리스트창이 사라지지 않아서 제거
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfig form = new FormConfig();
            form.StartPosition = FormStartPosition.CenterParent;

            form.ShowDialog(this);
        }

        private void contextMenuNotify_Opened(object sender, EventArgs e)
        {
            showToolStripMenuItem.Checked = this.Visible;
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
	}

    /// <summary>
    /// MainForm에 override 할때는 잘되지 않고 메시지 필터를 Application.AddMessageFilter(new MessageFilter()); 와 같이 등록해서 사용하면 잘 된다.
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
                        FormMain.bCloseByCallEditor = true;
                        FormMain.formMain.Close();
                    }
                    break;
            }

            return false;

        }

    }
}
