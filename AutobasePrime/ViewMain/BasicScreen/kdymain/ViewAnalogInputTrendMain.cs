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
	/// Summary description for ViewAnalogInputTrendMain.
	/// </summary>
	public class ViewAnalogInputTrendMain  : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ToolBarButton toolBarButton8;

		static public ViewAnalogInputTrendMain formThis;
		BasicScreen.kdymain.ViewAnalogInputTrend child;
        
		public ViewAnalogInputTrendMain(int hour, ArrayList arr, Color backColor, int nTagNameDispSize)
            
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			formThis = this;
			child = new ViewAnalogInputTrend(this, hour, arr, backColor, nTagNameDispSize);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputTrendMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton7 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton8 = new System.Windows.Forms.ToolBarButton();
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
            this.toolBarButton4,
            this.toolBarButton5,
            this.toolBarButton6,
            this.toolBarButton7,
            this.toolBarButton8});
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
            // toolBarButton5
            // 
            resources.ApplyResources(this.toolBarButton5, "toolBarButton5");
            this.toolBarButton5.Name = "toolBarButton5";
            // 
            // toolBarButton6
            // 
            resources.ApplyResources(this.toolBarButton6, "toolBarButton6");
            this.toolBarButton6.Name = "toolBarButton6";
            // 
            // toolBarButton7
            // 
            resources.ApplyResources(this.toolBarButton7, "toolBarButton7");
            this.toolBarButton7.Name = "toolBarButton7";
            // 
            // toolBarButton8
            // 
            resources.ApplyResources(this.toolBarButton8, "toolBarButton8");
            this.toolBarButton8.Name = "toolBarButton8";
            // 
            // ViewAnalogInputTrendMain
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.toolBar1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "ViewAnalogInputTrendMain";
            this.Load += new System.EventHandler(this.ViewAnalogInputTrendMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewAnalogInputTrendMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewAnalogInputTrendMain_Closed);
            this.Resize += new System.EventHandler(this.ViewAnalogInputTrendMain_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewAnalogInputTrendMain_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();

		private void ViewAnalogInputTrendMain_Load(object sender, System.EventArgs e)
		{
			child.Show();
			DetailMainWindowSetSize();
			this.child.Focus();
			ringForm.push(this);

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.toolBar1.Buttons.Remove(toolBarButton5);
                this.toolBar1.Buttons.Remove(toolBarButton6);

                
                this.toolBar1.Buttons.Remove(toolBarButton3);
                
            }
		}

		private void DetailMainWindowSetSize()
		{
			getCurrentFontSize();
			getClientSize();

			child.Left = 0;
			child.Top = 0;
			//child.Width = work.width;
			//child.Height = work.height-toolBar1.Height;

			child.Width = this.ClientRectangle.Width;
			child.Height = this.ClientRectangle.Height-toolBar1.Height;

			this.toolBar1.Left = 0;
			//this.toolBar1.Top = work.height-this.toolBar1.Height;
			//this.toolBar1.Width = work.width;

			this.toolBar1.Top = ClientRectangle.Height-this.toolBar1.Height;
			this.toolBar1.Width = ClientRectangle.Width;
		}

		private void ViewAnalogInputTrendMain_SizeChanged(object sender, System.EventArgs e)
		{
			DetailMainWindowSetSize();
			this.Invalidate();
		}

		private void ViewAnalogInputTrendMain_Resize(object sender, System.EventArgs e)
		{
			ViewAnalogInputTrendMain_SizeChanged(sender, e);		
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			String		buf;

			if(e.Button == toolBarButton1) 
			{
				this.Close();
				return;
			}
			if(e.Button == toolBarButton2) 
			{
				buf = child.callShowDataSortFunc();
				this.toolBarButton2.Text = buf;
				return;
			}
			if(e.Button == toolBarButton3) 
			{
				child.callSettingDialog();
				return;
			}
			if(e.Button == toolBarButton4) 
			{
				child.callTimeSettingDialog();
				return;
			}
			if(e.Button == toolBarButton5) 
			{
				child.callDataMinusFunc();
				return;
			}
			if(e.Button == toolBarButton6) 
			{
				child.callDataPlusFunc();
				return;
			}
			if(e.Button == toolBarButton7) 
			{
				child.callDataHourMinusFunc();
				return;
			}
			if(e.Button == toolBarButton8) 
			{
				child.callDataHourPlusFunc();
				return;
			}
		}

		private void ViewAnalogInputTrendMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			String		buf;

			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
				case Keys.F2 : 
					buf = child.callShowDataSortFunc();
					this.toolBarButton2.Text = buf;
					return;
				case Keys.F3 : child.callSettingDialog(); return;
				case Keys.F7 : child.AiMultiTrendMainTagChange(); return;
				case Keys.F12 : child.callTimeSettingDialog(); return;
				case Keys.Home : child.callDataMinusFunc(); return;
				case Keys.End : child.callDataPlusFunc(); return;
				case Keys.Subtract : child.callDataHourMinusFunc(); return;
				case Keys.Add : child.callDataHourPlusFunc(); return;
			}
			if(child.detailArrowKeyOperation(e.KeyCode)) return;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}
		
		private void ViewAnalogInputTrendMain_Closed(object sender, System.EventArgs e)
		{
			child.Close();
			ringForm.pop(this);
		} 
	}
}
