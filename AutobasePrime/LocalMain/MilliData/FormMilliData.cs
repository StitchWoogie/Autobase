using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using AutoLibLocal;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormMilliData.
	/// </summary>
	/// 
	
	public class FormMilliData : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormMilliData()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMilliData));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.toolBarButton1.Tag = "Close";
            resources.ApplyResources(this.toolBarButton1, "toolBarButton1");
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Tag = "Load";
            resources.ApplyResources(this.toolBarButton2, "toolBarButton2");
            // 
            // toolBarButton3
            // 
            this.toolBarButton3.Name = "toolBarButton3";
            this.toolBarButton3.Tag = "Config";
            resources.ApplyResources(this.toolBarButton3, "toolBarButton3");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // FormMilliData
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolBar1);
            this.KeyPreview = true;
            this.Name = "FormMilliData";
            this.Closed += new System.EventHandler(this.FormMilliData_Closed);
            this.Load += new System.EventHandler(this.FormMilliData_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMilliData_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;

		public static CatWindowRing ringForm = new CatWindowRing();

		MilliDataWnd wndChild = new MilliDataWnd();

		private void FormMilliData_Load(object sender, System.EventArgs e)
		{
			wndChild.TopLevel = false;
			wndChild.Parent = this;
			wndChild.Dock = DockStyle.Fill;
			
			this.panel1.Controls.Add(wndChild);

			wndChild.Show();
						
			ringForm.push(this);
		}

		private void FormMilliData_Closed(object sender, System.EventArgs e)
		{
			ringForm.pop(this);
		}

		void CommandMilliDataLoad()
		{
			string filename;

			if(FormSelectMilliData.SelectMilliData(out filename)) 
			{
				//wndChild.SetFileNameLocal(filename);
			}
		}

		void CommandMilliDataOption()
		{
            FormSelectMilliDataOption dialog = new FormSelectMilliDataOption();

            dialog.m_nTimeType = wndChild.GetTimeType();
            dialog.m_nBackColor = MilliDataWnd.nMilliDataBackColor;
            dialog.checkBoxDisplayTagDescription.Checked = MilliDataWnd.bMilliDataTagDescription;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                wndChild.SetTimeType(dialog.m_nTimeType);
                MilliDataWnd.nMilliDataBackColor = dialog.m_nBackColor;
                MilliDataWnd.bMilliDataTagDescription = dialog.checkBoxDisplayTagDescription.Checked;
                TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMilliDataBackColor", MilliDataWnd.nMilliDataBackColor);
                TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bMilliDataTagDescription", MilliDataWnd.bMilliDataTagDescription);
                wndChild.bBitmapFill = false;	// 상단에 있는 전체그래프롤 무효화 시켜야 배경색상이 적용된다.
            }
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == this.toolBarButton1)	
			{
				this.Close();
			}
			else if(e.Button == this.toolBarButton2) 
			{
				CommandMilliDataLoad();
			}
			else if(e.Button == this.toolBarButton3) 
			{
				CommandMilliDataOption();
			}

		}

		private void FormMilliData_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)	this.Close();
			else if(e.KeyCode == Keys.F3)	CommandMilliDataOption();
			else if(e.KeyCode == Keys.F4)	CommandMilliDataLoad();
			else {}
		}

	}
}

