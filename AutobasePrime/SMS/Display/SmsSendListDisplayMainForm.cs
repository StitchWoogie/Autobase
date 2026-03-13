using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SMS.SmsFunc;

namespace SMS.Display
{
	/// <summary>
	/// Summary description for SmsSendListDisplayMainForm.
	/// </summary>
	public class SmsSendListDisplayMainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;

		SmsSendListDisplayForm child;

		public SmsSendListDisplayMainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			child = new SmsSendListDisplayForm();
			this.Controls.Add(child);
			Font	f = new Font("±¼¸²", 10);
			this.toolBar1.Font = f;
			//this.toolBar1.Font = SmsBasic.smsConfig.listFont;			
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmsSendListDisplayMainForm));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4});
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            this.toolBar1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolBar1_KeyDown);
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
            // toolBarButton4
            // 
            this.toolBarButton4.Name = "toolBarButton4";
            resources.ApplyResources(this.toolBarButton4, "toolBarButton4");
            // 
            // SmsSendListDisplayMainForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.toolBar1);
            this.KeyPreview = true;
            this.Name = "SmsSendListDisplayMainForm";
            this.Load += new System.EventHandler(this.SmsSendListDisplayMainForm_Load);
            this.SizeChanged += new System.EventHandler(this.SmsSendListDisplayMainForm_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void SmsSendListDisplayMainForm_Load(object sender, System.EventArgs e)
		{
			child.Show();
			setListMainSizeChange();
			this.child.Focus();
		}

		private void setListMainSizeChange()
		{			
			child.Left = 0;
			child.Top = 0;
			child.Width = ClientSize.Width;
			child.Height = ClientSize.Height-this.toolBar1.Height;
		}

		private void SmsSendListDisplayMainForm_SizeChanged(object sender, System.EventArgs e)
		{
			setListMainSizeChange();
			this.Invalidate();
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
				child.deleteSeelctedSendData();
				return;
			}
			if(e.Button == toolBarButton3) 
			{
				child.currentListPrint();
				return;
			}
			if(e.Button == toolBarButton4) 
			{
				child.setFont();
				return;
			}
		}

		
		private void toolBar1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			child.sendListViewKeyProc(e);
		}
	}
}
