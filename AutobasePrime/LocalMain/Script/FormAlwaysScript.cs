using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormAlwaysScript.
	/// </summary>
	public class FormAlwaysScript : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listViewScript;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3; 
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItemRunStop;
		private System.Windows.Forms.MenuItem menuItemDetail;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemClose;
		public static CatWindowRing ringScript = new CatWindowRing();

		public FormAlwaysScript()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        public static void SetTitle()
        {
            Form form = ringScript.GetFirstHWND();

            if (form == null) return;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlwaysScript));

            string text = resources.GetString("$this.Text");

            if (CheckEngineAlwaysScript.bActiveScript == false)
            {
                if (Tools.IsLangKorean())
                {
                    text += " - 전체 스크립트 정지 중";
                }
                else
                {
                    text += " - All Scripts Stopped.";
                }
            }

            form.Text = text;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlwaysScript));
            this.listViewScript = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemRunStop = new System.Windows.Forms.MenuItem();
            this.menuItemDetail = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemClose = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.SuspendLayout();
            // 
            // listViewScript
            // 
            this.listViewScript.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewScript.ContextMenu = this.contextMenu1;
            resources.ApplyResources(this.listViewScript, "listViewScript");
            this.listViewScript.FullRowSelect = true;
            this.listViewScript.HideSelection = false;
            this.listViewScript.MultiSelect = false;
            this.listViewScript.Name = "listViewScript";
            this.listViewScript.UseCompatibleStateImageBehavior = false;
            this.listViewScript.View = System.Windows.Forms.View.Details;
            this.listViewScript.SelectedIndexChanged += new System.EventHandler(this.listViewScript_SelectedIndexChanged);
            this.listViewScript.DoubleClick += new System.EventHandler(this.listViewScript_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemRunStop,
            this.menuItemDetail,
            this.menuItem3,
            this.menuItemClose});
            // 
            // menuItemRunStop
            // 
            this.menuItemRunStop.Index = 0;
            resources.ApplyResources(this.menuItemRunStop, "menuItemRunStop");
            this.menuItemRunStop.Click += new System.EventHandler(this.menuItemRunStop_Click);
            // 
            // menuItemDetail
            // 
            this.menuItemDetail.Index = 1;
            resources.ApplyResources(this.menuItemDetail, "menuItemDetail");
            this.menuItemDetail.Click += new System.EventHandler(this.menuItemDetail_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuItemClose
            // 
            this.menuItemClose.Index = 3;
            resources.ApplyResources(this.menuItemClose, "menuItemClose");
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
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
            // FormAlwaysScript
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.listViewScript);
            this.Controls.Add(this.toolBar1);
            this.KeyPreview = true;
            this.Name = "FormAlwaysScript";
            this.Load += new System.EventHandler(this.FormAlwaysScript_Load);
            this.Closed += new System.EventHandler(this.FormAlwaysScript_Closed);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAlwaysScript_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAlwaysScript_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		Color GetStatusColor(PCL_STRUCT pcl)
		{
			Color color;

			if(pcl.pcl.IsError())
				color = Color.Red;
			else 
			{
				color = pcl.flag ? Color.Black : Color.DarkGray;
			}

			return color;
		}
		
		private void FormAlwaysScript_Load(object sender, System.EventArgs e)
		{
			PCL_STRUCT pcl;
			ListViewItem item;
			for(int i = 0; i < CheckEngineAlwaysScript.blockPCL.Count; i++) 
			{
				pcl = (PCL_STRUCT)CheckEngineAlwaysScript.blockPCL[i];
				item = new ListViewItem(pcl.filename);
				item.SubItems.Add(pcl.pcl.Description);
				item.SubItems.Add(pcl.pcl.GetScanTime().ToString());
				item.SubItems.Add(MakeStatusString(pcl));
				item.ForeColor = GetStatusColor(pcl);
				this.listViewScript.Items.Add(item);
			}

			if(this.listViewScript.Items.Count > 0) 
			{
				this.listViewScript.Items[0].Selected = true;
			}

			ringScript.push(this);

			TotalConfig.AutoBaseMainListCtrlConfigLoad(this.listViewScript, "LocalMainViewScriptFrame");

            SharedViewMain.EventListUserChanged += new SharedViewMain.DelegatePublic(OnUserChanged);
		}

        private void FormAlwaysScript_FormClosed(object sender, FormClosedEventArgs e)
        {
            SharedViewMain.EventListUserChanged -= new SharedViewMain.DelegatePublic(OnUserChanged);
        }

        void OnUserChanged()
        {
            this.toolBarButton2.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCRIPT);
        }

		private void listViewScript_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			/*
			if(this.listViewScript.SelectedItems.Count == 0)	return;

			int index = this.listViewScript.SelectedItems[0].Index;
			PCL_STRUCT pcl;

			pcl = (PCL_STRUCT)CheckEngineAlwaysScript.blockPCL[index];

			this.checkBoxRun.Checked = pcl.flag;
			*/
		}

		private void checkBoxRun_CheckedChanged(object sender, System.EventArgs e)
		{
			/*
			if(this.listViewScript.SelectedItems.Count == 0)	return;

			ListViewItem item = this.listViewScript.SelectedItems[0];
			int index = item.Index;
			PCL_STRUCT pcl;
			
			pcl = (PCL_STRUCT)CheckEngineAlwaysScript.blockPCL[index];

			pcl.flag = this.checkBoxRun.Checked;
			item.ForeColor = GetStatusColor(pcl);
			item.SubItems[3].Text = MakeStatusString(pcl);
			*/
		}

		string MakeStatusString(PCL_STRUCT pcl)
		{
			string msg;

			if(pcl.pcl.IsError()) 
			{
				msg = pcl.pcl.GetError();
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					if(pcl.flag)	msg = "실행중";
					else			msg = "정지중";
				}
				else if(Tools.IsLangJapanese()) 
				{
					if(pcl.flag)	msg = "実行中";
					else			msg = "停止中";
				}
				else if(Tools.IsLangChinese()) 
				{
					if(pcl.flag)	msg = "正在运行";
					else			msg = "停止";
				}
				else 
				{
					if(pcl.flag)	msg = "Running";
					else			msg = "Stopped";
				}

                if (pcl.pcl.bUseThread)
                    msg += "(Thread)";
			}

			return msg;
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			PCL_STRUCT pcl;
			ListViewItem item;
			for(int i = 0; i < CheckEngineAlwaysScript.blockPCL.Count; i++) 
			{
				pcl = (PCL_STRUCT)CheckEngineAlwaysScript.blockPCL[i];
				item = this.listViewScript.Items[i];

				if(item.ForeColor != GetStatusColor(pcl)) 
				{
					item.ForeColor = GetStatusColor(pcl);
					item.SubItems[3].Text = MakeStatusString(pcl);			
				}
			}
		}

		static FormAlwaysScriptDetail formScriptDetail = null;

		void ViewDetail()
		{
			if(this.listViewScript.SelectedItems.Count == 0)	return;

			ListViewItem item = this.listViewScript.SelectedItems[0];
			int index = item.Index;
			PCL_STRUCT pcl;
			
			pcl = (PCL_STRUCT)CheckEngineAlwaysScript.blockPCL[index];

			if(formScriptDetail != null) 
				formScriptDetail.Close();
			
			formScriptDetail = new FormAlwaysScriptDetail(pcl.pcl, pcl.filename);
			
			formScriptDetail.Owner = TotalConfig.formMain;
			formScriptDetail.Show();
		}

		private void listViewScript_DoubleClick(object sender, System.EventArgs e)
		{
			ViewDetail();
		}

		private void FormAlwaysScript_Closed(object sender, System.EventArgs e)
		{
			ringScript.pop(this);
			TotalConfig.AutoBaseMainListCtrlConfigSave(this.listViewScript, "LocalMainViewScriptFrame");
		}

		void RunStop()
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCRIPT))
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("스크립트를 실행/정지할 권한이 없습니다.", "권한없음");
                else
                    MessageBox.Show("You do not have permission to RUN/STOP the script.", "No permission");
                return;
            }

			if(this.listViewScript.SelectedItems.Count == 0)	return;

			ListViewItem item = this.listViewScript.SelectedItems[0];
			int index = item.Index;
			PCL_STRUCT pcl;
			
			pcl = (PCL_STRUCT)CheckEngineAlwaysScript.blockPCL[index];

			pcl.flag = !pcl.flag;
			item.ForeColor = GetStatusColor(pcl);
			item.SubItems[3].Text = MakeStatusString(pcl);
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == this.toolBarButton1) 
			{
				this.Close();
			}
            else if (e.Button == this.toolBarButton2) 
			{
				RunStop();				
			}
            else if (e.Button == this.toolBarButton3) 
			{
				ViewDetail();
			}
		}

		private void menuItemRunStop_Click(object sender, System.EventArgs e)
		{
			RunStop();
		}

		private void menuItemDetail_Click(object sender, System.EventArgs e)
		{
			ViewDetail();
		}

		private void FormAlwaysScript_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)		this.Close();
			else if(e.KeyCode == Keys.F3)		RunStop();
			else if(e.KeyCode == Keys.Enter)	ViewDetail();
			else{}
		}

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
	}
}
