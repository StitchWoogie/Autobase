using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using NetTools.OldDefine;
using System.IO;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for FormNavigator.
	/// </summary>
	public class FormNavigator : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable. 
		/// </summary>
		private System.ComponentModel.Container components = null;
		ObjectRoot objectGraphic = null;
		string sFileName;
		bool bMouseCapture = false;
		int  nStartX, nStartY, nOldX, nOldY;
		public RECT rZone = new RECT();

		public FormNavigator(string filename)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			//objectGraphic = new ObjectRoot(sharedData);
			sFileName = filename;
			SharedData.formNavigator = this;

			if(ConfigViewMain.bNaviSaveFlag) 
			{
				int x1, y1, x2, y2;

				GraphicTool.GetMultiScreenSize(out x1, out y1, out x2, out y2);

				int startx = ConfigViewMain.nNaviSaveX;
				int starty = ConfigViewMain.nNaviSaveY;

				if(startx < x1)		startx = x1;
				if(starty < y1)		starty = y1;
				if(startx >= x2)	startx = x2-50;
				if(starty >= y2)	starty = y2-50;

				this.StartPosition = FormStartPosition.Manual;
				this.Left = startx;
				this.Top = starty;
				this.Width = ConfigViewMain.nNaviSaveWidth;
				this.Height = ConfigViewMain.nNaviSaveHeight;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNavigator));
            this.SuspendLayout();
            // 
            // FormNavigator
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "FormNavigator";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormNavigator_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormNavigator_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormNavigator_Paint);
            this.SizeChanged += new System.EventHandler(this.FormNavigator_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormNavigator_MouseDown);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormNavigator_Closing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormNavigator_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormNavigator_Load(object sender, System.EventArgs e)
		{
			//Graphics g = CreateGraphics();
			//objectGraphic.LoadByScreen(this, g, 0, sFileName);
			//objectGraphic.SetScreenSize(g, ClientSize.Width, ClientSize.Height);
			//objectGraphic.SetObjectOpticMethod(2);

			//sharedData.onEventTagChanged += new SharedData.OnEventTagChanged(FormGraphic_EventTag);
			//SetGraphicWindowTitle();

			//sharedData.arrayFormGraphic.Add(this);
			LoadNewModule();

            //20250314 PSU 추가, 깜박임 방지.
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
		}

		public int LoadNewModule()
		{
			if(objectGraphic != null) 
			{
				objectGraphic = null;
			}

			Form child = TotalConfig.formMain.ActiveMdiChild;

			if(child == null)	return 0;

			if(child.Name != "FormGraphicFrame")	// 그래픽 윈도우가 아니다.
				return 0;

			FormGraphicChild childGraphic = ((FormGraphicFrame)child).formChild;

            sFileName = childGraphic.sFileName;
			string ext = Path.GetExtension(sFileName);

			SetNavigatorWindowText(sFileName);
			
			if(String.Compare(ext, ".MOD", true) == 0) {}
			else if(String.Compare(ext, ".MODX", true) == 0) {}
			else
			{
				return 0;
			}

			objectGraphic = new ObjectRoot();
			objectGraphic.Load(this, sFileName);
			objectGraphic.SetScreenSize(ClientSize.Width, ClientSize.Height);
			objectGraphic.SetObjectOpticMethod(2);

			int x1=0, y1=0, x2=0, y2=0;
			childGraphic.GetNavigatorSize(ref x1, ref y1, ref x2, ref y2);
			this.UpdateNavigatorRectZone(x1, y1, x2, y2);

			return 1;
		}

		private void FormNavigator_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(this.WindowState == FormWindowState.Minimized)	return;

			Graphics g = e.Graphics;

			if(objectGraphic != null) {
				Point point = new Point(0, 0);
				objectGraphic.Display(g, this.ClientRectangle, e.ClipRectangle, point);

				int x1, y1, x2, y2;

				if(bMouseCapture) {
					x1 = nStartX;
					y1 = nStartY;
					x2 = nOldX;
					y2 = nOldY;
				}
				else {
					int modx, mody;

					objectGraphic.GetModuleSize(out modx, out mody);

					x1 = rZone.left*(ClientRectangle.Width)/modx;
					y1 = rZone.top*(ClientRectangle.Height)/mody;
					x2 = rZone.right*(ClientRectangle.Width)/modx;
					y2 = rZone.bottom*(ClientRectangle.Height)/mody;
				}

				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

				x2 += 1;
				y2 += 1;

				Pen hPenBlack = new Pen(Color.Black, 1);
				Pen hPenWhite = new Pen(Color.White, 1);

				g.DrawLine(hPenWhite, x1, y1, x2, y1);
				g.DrawLine(hPenWhite, x2, y1, x2, y2);
				g.DrawLine(hPenWhite, x2, y2, x1, y2);
				g.DrawLine(hPenWhite, x1, y2, x1, y1);

				g.DrawLine(hPenBlack, x1-1, y1-1, x2+1, y1-1);
				g.DrawLine(hPenBlack, x2+1, y1-1, x2+1, y2+1);
				g.DrawLine(hPenBlack, x2+1, y2+1, x1-1, y2+1);
				g.DrawLine(hPenBlack, x1-1, y2+1, x1-1, y1-1);

				g.DrawLine(hPenBlack, x1+1, y1+1, x2-1, y1+1);
				g.DrawLine(hPenBlack, x2-1, y1+1, x2-1, y2-1);
				g.DrawLine(hPenBlack, x2-1, y2-1, x1+1, y2-1);
				g.DrawLine(hPenBlack, x1+1, y2-1, x1+1, y1+1);
			}
			else {
				DrawClass.gcls(g, ClientRectangle, Color.White);
			}

			//timerGraphic.Enabled = false;
			
			//timerGraphic.Enabled = true;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		} 

		private void FormNavigator_SizeChanged(object sender, System.EventArgs e)
		{
			if(objectGraphic != null)
				objectGraphic.SetScreenSize(ClientSize.Width, ClientSize.Height);
			Invalidate();
		}

		private void FormNavigator_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)	return;

			if(bMouseCapture)	return;

			this.Capture = true;
			bMouseCapture = true;
			nStartX = nOldX = e.X;
			nStartY = nOldY = e.Y;

			this.Invalidate();
		}

		private void FormNavigator_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(!bMouseCapture)	return;

			nOldX = e.X;
			nOldY = e.Y;

			Invalidate();
		}

		private void FormNavigator_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(!bMouseCapture)	return;

			bMouseCapture = false;
			this.Capture = false;

			Form child = TotalConfig.formMain.ActiveMdiChild;

			if(child == null)	return;

			if(child.Name != "FormGraphicFrame")	// 그래픽 윈도우가 아니다.
				return;

			FormGraphicChild childGraphic = ((FormGraphicFrame)child).formChild;

			if(objectGraphic == null)	return;

			int x, y;

			objectGraphic.GetModuleSize(out x, out y);

			RECT r = new RECT();
			Rectangle rect = ClientRectangle;
	
			r.left = nStartX*x/rect.Right;
			r.top  = nStartY*y/rect.Bottom;
			r.right = nOldX*x/rect.Right;
			r.bottom = nOldY*y/rect.Bottom;

			if(r.left > r.right)	Tools.Temp(ref r.left, ref r.right);
			if(r.top  > r.bottom)	Tools.Temp(ref r.top,  ref r.bottom);

			childGraphic.NavigatorChangePosSize(r);
		}

		void SaveCoordinate()
		{
			ConfigViewMain.bNaviSaveFlag = true;
			ConfigViewMain.nNaviSaveX = this.Left;
			ConfigViewMain.nNaviSaveY = this.Top;
			ConfigViewMain.nNaviSaveWidth = this.Width;
			ConfigViewMain.nNaviSaveHeight = this.Height;

			ConfigViewMain.Save();
		}

		private void FormNavigator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveCoordinate();
			SharedData.formNavigator = null;
		}

		void SetNavigatorWindowText(string filename)
		{
			string str;

			if(Tools.IsLangKorean()) 
				str = String.Format("항해지도({0})", filename);
			else if(Tools.IsLangJapanese()) 
				str = String.Format("ナビゲーター({0})", filename);
			else if(Tools.IsLangChinese()) 
				str = String.Format("航海地图({0})", filename);
			else
				str = String.Format("Navigation({0})", filename);

			this.Text = str;
		}

		public void UpdateNavigatorRectZone(int x1, int y1, int x2, int y2)
		{
			Rectangle rect = ClientRectangle;
			
			rZone.left = x1;
			rZone.top = y1;
			rZone.right = x2;
			rZone.bottom = y2;

			Invalidate();
		}
	}

	public class ViewNavigator
	{
		public ViewNavigator()
		{
		}

		public static void NavigatorChangeModule()
		{
			if(SharedData.formNavigator == null)	return;

			FormNavigator form = (FormNavigator)SharedData.formNavigator;

			form.LoadNewModule();

			form.Invalidate();
		}

		public static void UpdateNavigatorRectZone(int x1, int y1, int x2, int y2)
		{
			if(SharedData.formNavigator == null)	return;

			FormNavigator form = (FormNavigator)SharedData.formNavigator;

			form.UpdateNavigatorRectZone(x1, y1, x2, y2);
		}
	}
}

