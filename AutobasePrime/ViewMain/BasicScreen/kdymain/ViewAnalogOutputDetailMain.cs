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

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogOutputDetailMain.
	/// </summary>
	public class ViewAnalogOutputDetailMain : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		static public ViewAnalogOutputDetailMain formThis;
		BasicScreen.kdymain.ViewAnalogOutputDetail child;

		public ViewAnalogOutputDetailMain(string tag)
            
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			formThis = this;
			child = new ViewAnalogOutputDetail(this, tag);
			child.TopLevel = false;
			this.Controls.Add(child);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogOutputDetailMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
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
            this.toolBarButton2});
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
            // ViewAnalogOutputDetailMain
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.toolBar1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "ViewAnalogOutputDetailMain";
            this.Load += new System.EventHandler(this.ViewAnalogOutputDetailMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewAnalogOutputDetailMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewAnalogOutputDetailMain_Closed);
            this.Resize += new System.EventHandler(this.ViewAnalogOutputDetailMain_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewAnalogOutputDetailMain_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();

		private void ViewAnalogOutputDetailMain_Load(object sender, System.EventArgs e)
		{
			child.Show();
			DetailMainWindowSetSize();
			this.child.Focus();
			ringForm.push(this);
		}

		private void DetailMainWindowSetSize()
		{
			getCurrentFontSize();
			getClientSize();

			child.Left = 0;
			child.Top = 0;
			child.Width = work.width;
			child.Height = work.height-toolBar1.Height;

			this.toolBar1.Left = 0;
			this.toolBar1.Top = work.height-this.toolBar1.Height;
			this.toolBar1.Width = work.width;
		}

		private void ViewAnalogOutputDetailMain_SizeChanged(object sender, System.EventArgs e)
		{
			DetailMainWindowSetSize();
			this.Invalidate();
		}

		private void ViewAnalogOutputDetailMain_Resize(object sender, System.EventArgs e)
		{
			ViewAnalogOutputDetailMain_SizeChanged(sender, e);		
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) 
			{
				this.Close();
				return;
			}
			if(e.Button == toolBarButton2) 
			{
				child.CallAnalogOutputDialog();
				return;
			}
		}

		private void ViewAnalogOutputDetailMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
				case Keys.F3 : child.CallAnalogOutputDialog(); return;
			}
			if(child.detailArrowKeyOperation(e.KeyCode)) return;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}
		
		private void ViewAnalogOutputDetailMain_Closed(object sender, System.EventArgs e)
		{
			child.Close();
			ringForm.pop(this);
		} 


	}
}
