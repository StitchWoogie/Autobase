using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using System.IO;
using System.Threading.Tasks;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for FormGraphicFrame.
	/// </summary>
	public class FormGraphicFrame : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		private const int WM_ENTERSIZEMOVE = 0x0231;
		private const int WM_EXITSIZEMOVE = 0x0232;
		private bool _isUserResizing = false;

		public FormGraphicChild formChild;
		public static ArrayList arrayFormGraphFrame = new ArrayList();
		//static ArrayList registedGraphicModule = new ArrayList();
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButtonClose;
		private System.Windows.Forms.ToolBarButton toolBarButtonTop;
		private System.Windows.Forms.ToolBarButton toolBarButtonPrev;
		public bool bCloseCommand = false;

        /*
		class REGISTED_GRAPHIC_MODULE 
		{
            public FormGraphicFrame form;
			public int	 count;
			public string filename;
		}*/

		public FormGraphicFrame(string filename)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            formChild = new FormGraphicChild(filename);

            FormGraphicFrame.arrayFormGraphFrame.Add(this); // 모듈시작시 모듈종료시 스크립트가 child로 이동했으므로 리스트는 Load되기전에 등록해야한다. 
                                                            // 이전에는 Load에 있었음 2013-6-13 
                                                            // new formChild 위에 있으니 ModuleIsAlive 함수를 사용하면 formChild가 할당되지 않아서 다운된다. 그래서 밑에 두었다. 2013-7-8

			formChild.TopLevel = false;
			formChild.FormBorderStyle = FormBorderStyle.None;
			formChild.Dock = DockStyle.Fill;
			
			this.panel1.Controls.Add(formChild);

			formChild.Show();
		}

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
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
            if(!base.IsDisposed)
			    base.Dispose( disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphicFrame));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButtonClose = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonTop = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonPrev = new System.Windows.Forms.ToolBarButton();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButtonClose,
            this.toolBarButtonTop,
            this.toolBarButtonPrev});
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButtonClose
            // 
            this.toolBarButtonClose.Name = "toolBarButtonClose";
            resources.ApplyResources(this.toolBarButtonClose, "toolBarButtonClose");
            // 
            // toolBarButtonTop
            // 
            this.toolBarButtonTop.Name = "toolBarButtonTop";
            resources.ApplyResources(this.toolBarButtonTop, "toolBarButtonTop");
            // 
            // toolBarButtonPrev
            // 
            this.toolBarButtonPrev.Name = "toolBarButtonPrev";
            resources.ApplyResources(this.toolBarButtonPrev, "toolBarButtonPrev");
            // 
            // FormGraphicFrame
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolBar1);
            this.Name = "FormGraphicFrame";
            this.ShowInTaskbar = false;
            this.Activated += new System.EventHandler(this.FormGraphicFrame_Activated);
            this.Closed += new System.EventHandler(this.FormGraphicFrame_Closed);
            this.Deactivate += new System.EventHandler(this.FormGraphicFrame_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormGraphicFrame_FormClosed);
            this.Load += new System.EventHandler(this.FormGraphicFrame_Load);
            this.SizeChanged += new System.EventHandler(this.FormGraphicFrame_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        /*
		void PlayScriptWhenModStartEnd(string when)
		{
            ScriptClass control;

            if (String.Compare(when, "ModStart", true) == 0)
                control = formChild.objectGraphic.scriptModuleStart;
            else if (String.Compare(when, "ModEnd", true) == 0)
                control = formChild.objectGraphic.scriptModuleEnd;
            else
                control = null;

            if (control == null) return;

            control.Run(this.formChild, null);

			if(control.IsError()) 
			{
				string message;
				message = control.GetError();
				MessageBox.Show(message, when+" Script");
			}
		}*/

		private void FormGraphicFrame_Load(object sender, System.EventArgs e)
		{
			//FormGraphicFrame.arrayFormGraphFrame.Add(this);

			//RegisterGraphicModule(formChild.sFileName);

			if(ConfigViewMain.bUseMenuButtonOnGraphic == false) 
			{
				this.toolBar1.Visible = false;
			}

			if(formChild.objectGraphic.GetModuleWindowStyle() == 1)		// popup
			{
				this.toolBar1.Visible = false;
			}
		}

        /*
		void RegisterGraphicModule(string filename)
		{
			int l;
			REGISTED_GRAPHIC_MODULE module;
	
			for(l = 0; l < registedGraphicModule.Count; l++) 
			{
				module = (REGISTED_GRAPHIC_MODULE)registedGraphicModule[l];
				if(String.Compare(module.filename, filename, true) == 0)	
				{	// 같은 모듈이 이미 등록되었다.
					module.count++;
					return;
				}
			}

			// 모듈 시작 프로그램을 시작한다.
			PlayScriptWhenModStartEnd("ModStart");

			module = new REGISTED_GRAPHIC_MODULE();

			module.form = this;
			module.count = 0;
			module.filename = filename;

			registedGraphicModule.Add(module);
		}

		void UnRegisterGraphicModule(string filename)
		{
			int l;
			REGISTED_GRAPHIC_MODULE module;
	
			for(l = 0; l < registedGraphicModule.Count; l++) 
			{
				module = (REGISTED_GRAPHIC_MODULE)registedGraphicModule[l];
				if(String.Compare(module.filename, filename, true) == 0) 
				{
					if(module.count > 0) 
					{	// 열린 모듈이 또 있다.
						module.count--;
					}
					else 
					{
						registedGraphicModule.RemoveAt(l);		

						// 모듈 종료 시 프로그램을 시작한다.
						PlayScriptWhenModStartEnd("ModEnd");
					}
					return;
				}
			}
		}*/

		private void FormGraphicFrame_Closed(object sender, System.EventArgs e)
		{
			
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_ENTERSIZEMOVE)
			{
				_isUserResizing = true;
				formChild?.SuspendGraphicUpdate();
			}
			else if (m.Msg == WM_EXITSIZEMOVE)
			{
				_isUserResizing = false;
				// panel 크기를 최종 반영 후 한 번만 전체 갱신
				if (!this.IsDisposed)
				{
					this.panel1.Width = ClientRectangle.Width;
					this.panel1.Height = ClientRectangle.Height - this.toolBar1.Height;
				}
				formChild?.ResumeGraphicUpdate();
			}
			base.WndProc(ref m);
		}

		private void FormGraphicFrame_SizeChanged(object sender, System.EventArgs e)
		{
			if(this.IsDisposed)	return;	// Dispose된 후에도 Size메시지가 발생하는 것 같다. 그래서 추가

            // 이 부분이 없으면 그래픽 화면 열 때 중앙에 바가 나타나면서 깜박거리는 현상이 발생 2009.2.3
            this.panel1.Width = ClientRectangle.Width;
            // 이 부분이 없으니까 최소화에서 이전크기로 갈때 Panel 영역이 높이 부분이 작게 나온다.
            this.panel1.Height = ClientRectangle.Height - this.toolBar1.Height;

			if (!_isUserResizing)
				formChild.ScrollUpdate();
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(this.bCloseCommand) 
			{
				this.timer1.Enabled = false;
				Close();
				return;
			}
		}

		//static int lPosAlwaysModuleScript;

        /*
		public static void ModuleScriptAlwaysTimer()
		{
			if(registedGraphicModule.Count == 0)	return;	// 검사할 필요가 없다.

			lPosAlwaysModuleScript ++;
			if(lPosAlwaysModuleScript >= registedGraphicModule.Count) 
			{
				lPosAlwaysModuleScript = 0;
			}

			REGISTED_GRAPHIC_MODULE module;

			module = (REGISTED_GRAPHIC_MODULE)registedGraphicModule[lPosAlwaysModuleScript];

            ScriptClass script = module.form.formChild.objectGraphic.scriptModuleAlways;

            if (script == null) return;

            script.Run(module.form, null);

            if (script.IsError()) 
			{
				string message;
				string title;
                message = script.GetError();
				if(NetTools.Tools.IsLangKorean()) 
				{
					title = String.Format("{0}의 Module Script Always에서 오류", module.filename);
				} 
				else 
				{
					title = String.Format("Module Script Always Error at {0}", module.filename);
				}
				MessageDisplay.Show("{0} {1}", title, message);
				return;
			}
		}*/

		async Task ModuleScriptOnActivate(string filename)
		{
            ScriptClass script = formChild.objectGraphic.scriptModuleActive;

            if (script == null) return;

            await script.RunAsync(this, null);

            if (script.IsError())
            {
                string message;
                string title;
                message = script.GetError();
                if (NetTools.Tools.IsLangKorean())
                {
                    title = String.Format("{0}의 Module Script Activate에서 오류", filename);
                }
                else
                {
                    title = String.Format("Module Script Activate Error at {0}", filename);
                }
                MessageDisplay.Show("{0} {1}", title, message);
                return;
            }

            /*
			int l;
			REGISTED_GRAPHIC_MODULE module;
	
			for(l = 0; l < registedGraphicModule.Count; l++) 
			{
				module = (REGISTED_GRAPHIC_MODULE)registedGraphicModule[l];
				if(String.Compare(module.filename, filename, true) == 0)	
				{	
					goto play_go;
				}
			}

			return;

			play_go:

            ScriptClass script = module.form.formChild.objectGraphic.scriptModuleActive;

            if (script == null) return;

            script.Run(module.form, null);

            if (script.IsError()) 
			{
				string message;
				string title;
                message = script.GetError();
				if(NetTools.Tools.IsLangKorean()) 
				{
					title = String.Format("{0}의 Module Script Activate에서 오류", module.filename);
				}
				else 
				{
					title = String.Format("Module Script Activate Error at {0}", module.filename);
				}
				MessageDisplay.Show("{0} {1}", title, message);
				return;
			}*/
		}

		async Task ModuleScriptOnNonActivate(string filename)
		{
            ScriptClass script = formChild.objectGraphic.scriptModuleDeactive;

            if (script == null) return;

            await script.RunAsync(this, null);

            if (script.IsError())
            {
                string message;
                string title;
                message = script.GetError();
                if (NetTools.Tools.IsLangKorean())
                {
                    title = String.Format("{0}의 Module Script NonActivate에서 오류", filename);
                }
                else
                {
                    title = String.Format("Module Script NonActivate Error at {0}", filename);
                }
                MessageDisplay.Show("{0} {1}", title, message);
                return;
            }

            /*
			int l;
			REGISTED_GRAPHIC_MODULE module;
	
			for(l = 0; l < registedGraphicModule.Count; l++) 
			{
				module = (REGISTED_GRAPHIC_MODULE)registedGraphicModule[l];
				if(String.Compare(module.filename, filename, true) == 0)	
				{	
					goto play_go;
				}
			}

			return;

			play_go:

            ScriptClass script = module.form.formChild.objectGraphic.scriptModuleDeactive;

            if (script == null) return;

            script.Run(module.form, null);

            if (script.IsError()) 
			{
				string message;
				string title;
                message = script.GetError();
				if(NetTools.Tools.IsLangKorean()) 
				{
					title = String.Format("{0}의 Module Script NonActivate에서 오류", module.filename);
				} 
				else 
				{
					title = String.Format("Module Script NonActivate Error at {0}", module.filename);
				}
				MessageDisplay.Show("{0} {1}", title, message);
				return;
			}*/
		}

		private async void FormGraphicFrame_Activated(object sender, System.EventArgs e)
		{
			await ModuleScriptOnActivate(formChild.sFileName);
		}

		private async void FormGraphicFrame_Deactivate(object sender, System.EventArgs e)
		{
			await ModuleScriptOnNonActivate(formChild.sFileName);
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == this.toolBarButtonClose) 
			{
				this.Close();
			}
            else if (e.Button == this.toolBarButtonTop) 
			{
				string filename;
			
				if(SharedData.userInfo.sStartPage.Length == 0) 
				{
					filename = MakeFilePath.Graphic("StartUp.modx");

					if(!File.Exists(filename)) 
					{
						filename = MakeFilePath.Graphic("StartUp.mod");
					}
				}
				else 
				{
					filename = MakeFilePath.Graphic(SharedData.userInfo.sStartPage);
				}
			
				GraphicTool.RestoreGraphicWindow(filename, -1, 0, 0);
			}
			else if(e.Button == this.toolBarButtonPrev) 
			{
				GraphicTool.Pop();
			}
			else {}
		}

        private void FormGraphicFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            // UnRegisterGraphicModule(formChild.sFileName);

            arrayFormGraphFrame.Remove(this);
            formChild.Close();

            TotalConfig.formMain.Select();	// 이부분이 없으니까 PopUp Module닫을때 다른 프로그램으로 포커스가 간다.
        }
	}
}
