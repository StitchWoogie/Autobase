using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using ProjectLib;
using System.Diagnostics;
using System.Threading;

namespace ProjectSelect
{
	/// <summary>
	/// Summary description for FormSelectProject.
	/// </summary>
	public class FormSelectProject : System.Windows.Forms.Form 
	{
		private System.Windows.Forms.Button buttonRun;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.CheckBox checkBoxTestMode;
		private System.Windows.Forms.CheckBox checkBoxEnableEdit;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.MainMenu mainMenu1; 
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemFileBackup;
        private System.Windows.Forms.MenuItem menuItemFileRestore;
        private MenuItem menuItem2;
        private MenuItem menuItemHelpAbout;
        private IContainer components;
        private ColumnHeader columnHeader4;
        private MenuItem menuItem3;
        private MenuItem menuItemImportFromSmartDevice;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        public bool bCallByStudio = false;

		public FormSelectProject()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("Z_IoT_Studio.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.OPEN_SCADA)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("OemOpenScadaProjectManager.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectProject));
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBoxTestMode = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableEdit = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemFileBackup = new System.Windows.Forms.MenuItem();
            this.menuItemFileRestore = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemImportFromSmartDevice = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemHelpAbout = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRun
            // 
            resources.ApplyResources(this.buttonRun, "buttonRun");
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonEdit
            // 
            resources.ApplyResources(this.buttonEdit, "buttonEdit");
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonNew
            // 
            resources.ApplyResources(this.buttonNew, "buttonNew");
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonModify
            // 
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader3});
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.m_list_ColumnClick);
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // checkBoxTestMode
            // 
            resources.ApplyResources(this.checkBoxTestMode, "checkBoxTestMode");
            this.checkBoxTestMode.Name = "checkBoxTestMode";
            // 
            // checkBoxEnableEdit
            // 
            resources.ApplyResources(this.checkBoxEnableEdit, "checkBoxEnableEdit");
            this.checkBoxEnableEdit.Name = "checkBoxEnableEdit";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFileBackup,
            this.menuItemFileRestore,
            this.menuItem3,
            this.menuItemImportFromSmartDevice});
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItemFileBackup
            // 
            this.menuItemFileBackup.Index = 0;
            resources.ApplyResources(this.menuItemFileBackup, "menuItemFileBackup");
            this.menuItemFileBackup.Click += new System.EventHandler(this.menuItemFileBackup_Click);
            // 
            // menuItemFileRestore
            // 
            this.menuItemFileRestore.Index = 1;
            resources.ApplyResources(this.menuItemFileRestore, "menuItemFileRestore");
            this.menuItemFileRestore.Click += new System.EventHandler(this.menuItemFileRestore_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuItemImportFromSmartDevice
            // 
            this.menuItemImportFromSmartDevice.Index = 3;
            resources.ApplyResources(this.menuItemImportFromSmartDevice, "menuItemImportFromSmartDevice");
            this.menuItemImportFromSmartDevice.Click += new System.EventHandler(this.menuItemImportFromSmartDevice_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemHelpAbout});
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItemHelpAbout
            // 
            this.menuItemHelpAbout.Index = 0;
            resources.ApplyResources(this.menuItemHelpAbout, "menuItemHelpAbout");
            this.menuItemHelpAbout.Click += new System.EventHandler(this.menuItemHelpAbout_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_list);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.buttonModify);
            this.panel2.Controls.Add(this.buttonDelete);
            this.panel2.Controls.Add(this.buttonNew);
            this.panel2.Controls.Add(this.buttonClose);
            this.panel2.Controls.Add(this.buttonEdit);
            this.panel2.Controls.Add(this.buttonRun);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkBoxTestMode);
            this.panel3.Controls.Add(this.checkBoxEnableEdit);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // FormSelectProject
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "FormSelectProject";
            this.Load += new System.EventHandler(this.FormSelectProject_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


		void ChangeOneItem(ListViewItem lvi, PROJECT_STRUCT lst)
		{
			lvi.SubItems[0].Text = lst.title;
			lvi.SubItems[1].Text = lst.directory;
            lvi.SubItems[2].Text = (lst.nPlatform == 1) ? "CE" : "Win";
			lvi.SubItems[3].Text = lst.used_date;
		}

        Lib libProject = new Lib();

		void FillListBox()
		{
            m_list.Items.Clear();

            string curDir = TotalConfig.sDirWorkProject;
            ListViewItem selectItem = null;

            foreach (PROJECT_STRUCT project in libProject.projectBlock)
            {
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");

                ChangeOneItem(lvi, project);

                lvi.Tag = project;

                m_list.Items.Add(lvi);

                if (String.Compare(project.directory, curDir, true) == 0)
                    selectItem = lvi;
            }

            if (selectItem != null)
            {
                selectItem.Selected = true;
                selectItem.EnsureVisible();
            }
        }

		private void FormSelectProject_Load(object sender, System.EventArgs e)
		{
			Icon icon = new Icon(this.Icon, 64, 64);
			this.pictureBox1.Image = icon.ToBitmap();
			
			string buf;
			string path;

            libProject.LoadProjectListFile();
			
			// TODO: Add extra initialization here
			buf = TotalConfig.AutoBaseIniGetOemProgramName();

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                path = "KD POWER";
            }
            else if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                path = String.Format("{0} 작업선택 1.0", buf);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    path = String.Format("{0} 작업 선택", buf);
                }
                else if (Tools.IsLangJapanese())
                {
                    path = String.Format("{0} プロジェクト選択", buf);
                }
                else if (Tools.IsLangChinese())
                {
                    path = String.Format("选择 {0} 项目", buf);
                }
                else if (Tools.IsLangVietnamese())
                {
                    path = String.Format("Quản lý dự án {0}", buf);
                }
                else
                {
                    path = String.Format("{0} Project Manager", buf);
                }
            }
			
			this.Text = path;

			TotalConfig.AutoBaseListCtrlConfigLoad(this.m_list, "ProjectSelect", "SelectProject");

			FillListBox();
			
			this.checkBoxTestMode.Checked = TotalConfig.GetAutoBaseTestMode();
			this.checkBoxEnableEdit.Checked = TotalConfig.GetAutoBaseEditEnable();

            if (bCallByStudio)
            {
                this.buttonClose.Visible = false;

                if (Tools.IsLangKorean())
                {
                    this.buttonRun.Text = "확인";
                    this.buttonEdit.Text = "취소";
                }
                else
                {
                    this.buttonRun.Text = "OK";
                    this.buttonEdit.Text = "Cancel";
                }

                this.AcceptButton = this.buttonRun;
                this.CancelButton = this.buttonEdit;
            }

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                this.menuItem2.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.buttonEdit.Visible = false;
                this.checkBoxEnableEdit.Visible = false;
                this.checkBoxTestMode.Visible = false;

                this.menuItem3.Visible = false;
                this.menuItemImportFromSmartDevice.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                this.menuItem3.Visible = false;
                this.menuItemImportFromSmartDevice.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                this.menuItem3.Visible = false;
                this.menuItemImportFromSmartDevice.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.KobasAI)
            {
                this.menuItem3.Visible = false;
                this.menuItemImportFromSmartDevice.Visible = false;
            }

		}

		//int GetSelectedPosition()
		//{
		//	if(m_list.SelectedItems.Count == 0)	return -1;
		//	return m_list.SelectedItems[0].Index;
		//}

        PROJECT_STRUCT GetSelectedProject()
        {
            if (m_list.SelectedItems.Count == 0)
                return null;

            return m_list.SelectedItems[0].Tag as PROJECT_STRUCT;
        }

        private void buttonDelete_Click(object sender, System.EventArgs e)
		{
            PROJECT_STRUCT project = GetSelectedProject();
            if (project == null)
            {
                
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("항목을 하나 선택한 후 삭제할 수 있습니다.", "선택 오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else 
				{
					MessageBox.Show("Select one item to delete.", "Selection error");
				}
				return;
			}

            int index = libProject.projectBlock.IndexOf(project);
            if (index < 0) return;

            if (Tools.IsLangKorean()) 
			{
				if(MessageBox.Show("작업을 삭제 할까요?\n작업을 삭제하면 작업 이름만 삭제 되므로\n기존 작업을 다음에 다시 설정할 수 있습니다.", project.title, MessageBoxButtons.YesNo)
					!= DialogResult.Yes)	return;
			}
			else if(Tools.IsLangChinese()) 
			{
				if(MessageBox.Show("要删除吗?\n如果要删除,就只删除项目名.\n您可以下一次再设置已有的项目.", project.title, MessageBoxButtons.YesNo)
					!= DialogResult.Yes)	return;
			}
			else 
			{
				if(MessageBox.Show("Delete Selected project?\nDelete project is only remove title from list.", project.title, MessageBoxButtons.YesNo)
					!= DialogResult.Yes)	return;
			}


            libProject.projectBlock.RemoveAt(index);
            m_list.Items.Remove(m_list.SelectedItems[0]);
        }

		void Modify()
		{
            PROJECT_STRUCT project = GetSelectedProject();
            if (project == null)
            {
                if (Tools.IsLangKorean()) 
				{
					MessageBox.Show("왼쪽에 있는 작업중에 하나를 선택한 후\n속성 수정 버튼을 다시 눌러주세요.", "속성 수정 오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请选择要修改的项。", "选择错误");
				}
				else 
				{
					MessageBox.Show("After select one project\nclick modify button.", "modify error");
				}
				return;
			}

            //project = (PROJECT_STRUCT)libProject.projectBlock[index];
            int index = libProject.projectBlock.IndexOf(project);

            string title;
			if(Tools.IsLangKorean()) 
				title = "작업 속성 수정";
			else if(Tools.IsLangJapanese()) 
				title = "プロジェクト プロパティ修正";
			else if(Tools.IsLangChinese()) 
				title = "修改项目属性";
			else 
				title = "Project modify";

            FormModify dialog = new FormModify(libProject);

            dialog.bModifyDialog = true;
            dialog.EditingProject = project;
            dialog.Text = title;
            dialog.textBoxTitle.Text = project.title;
            dialog.textBoxDirectory.Text = project.directory;
            dialog.listParent = m_list;
            dialog.nSelectedIndex = index;
            dialog.Set(project);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(project);
                project.title = dialog.textBoxTitle.Text;
                project.directory = dialog.textBoxDirectory.Text;

                ChangeOneItem(m_list.SelectedItems[0], project);
            }
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void buttonNew_Click(object sender, System.EventArgs e)
		{
			PROJECT_STRUCT project;

			project = new PROJECT_STRUCT();

			project.title = "";

			project.directory = TotalConfig.MakePathByWindowsDisk(TotalConfig.sOemRootFolder+"\\Project");

			string title;
			if(Tools.IsLangKorean()) 
				title = "새 작업 만들기";
			else if(Tools.IsLangJapanese()) 
				title = "プロジェクト新規作成";
			else if(Tools.IsLangChinese()) 
				title = "新项目";
            else if (Tools.IsLangVietnamese())
                title = "Dự án mới";
			else 
				title = "New Project";

            FormModify dialog = new FormModify(libProject);

            
            dialog.Text = title;
            dialog.textBoxTitle.Text = project.title;
            dialog.textBoxDirectory.Text = project.directory;
            dialog.EditingProject = null;
            dialog.listParent = m_list;
            dialog.nSelectedIndex = -1;
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
                dialog.Get(project);
                project.title = dialog.textBoxTitle.Text;
                project.directory = dialog.textBoxDirectory.Text;

                libProject.projectBlock.Add(project);
				ListViewItem lvi = new ListViewItem("");
				lvi.SubItems.Add("");
				lvi.SubItems.Add("");
                lvi.SubItems.Add("");
				this.ChangeOneItem(lvi, project);
                lvi.Tag = project;
                m_list.Items.Add(lvi);
				lvi.Selected = true;
				lvi.EnsureVisible();
			}	
		}

		void SaveProjectDirectory(string title, string directory)
		{
            if (!bCallByStudio) // 스튜디오에서 호출한 경우는 모든 프로젝트를 닫은 후 환경을 저장한다.
            {
                TotalConfig.SaveRegWorkProject(directory);
                //TotalConfig.SaveRegAutoBaseConfig("Work Project", null, "Directory", directory);
            }

			DateTime t = DateTime.Now;
			
			string ini_file = directory+"\\Config\\ProjectConfig.inix";
			string used_date = String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
			Profile.WritePrivateProfileStringW("Date Information", "Last Used", used_date, ini_file);
			Profile.WritePrivateProfileStringW("Information", "Title", title, ini_file);

            /* 이 부분은 Vista에서 일반실행에서 엑세스 오류가 발생한다. DLL을 모두 8.6으로 바꾸고 이부분을 뺏다. 2007-4-5
			// 8.2.0 ~ 8.2.? 버전에 개발된 프로토콜이 작업 폴더를 autobase.ini에서 읽어오기 때문에 autobase.ini파일도 수정해 주어야 한다.
			// 작업 폴더를 인식하지 못할때는 다시 컴파일해서 공급해야 할 것이다.
			string ini_path = String.Format("{0}\\autobase.ini", Tools.GetWindowsRootDirectory());
			Profile.WritePrivateProfileString("Work Project", "Directory", directory, ini_path);
             */
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			int index;	

            PROJECT_STRUCT project = GetSelectedProject();
            if (project == null)
            {
				SaveProjectDirectory(project.title, project.directory);
			}

			TotalConfig.SetAutoBaseEditEnable(this.checkBoxEnableEdit.Checked);
			TotalConfig.SetAutoBaseTestMode(this.checkBoxTestMode.Checked);

			TotalConfig.AutoBaseListCtrlConfigSave(this.m_list, "ProjectSelect", "SelectProject");

            libProject.SaveProjectListFile();

			Close();
		}

        public string sSelectedProject;

		bool RunOrEdit()
		{
			int index;
            PROJECT_STRUCT project = GetSelectedProject();
            if (project == null)
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("왼쪽에 있는 작업중에 하나를 선택한 후\n실행/편집 버튼을 다시 눌러주세요.", "실행/편집 오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("先选择项目项中的一个，\n然后请再按{运行/编辑}按钮。", "运行/编辑 错误");
				}
				else 
				{
					MessageBox.Show("Open after select one project.", "open error");
				}
				return false;
			}

            if (!libProject.CheckExistProjectDirectory(project.directory))
                return false;

            libProject.CloseAllAutoBaseProgram();

			SaveProjectDirectory(project.title, project.directory);

			TotalConfig.SetAutoBaseEditEnable(this.checkBoxEnableEdit.Checked);
			TotalConfig.SetAutoBaseTestMode(this.checkBoxTestMode.Checked);

			TotalConfig.AutoBaseListCtrlConfigSave(this.m_list, "ProjectSelect", "SelectProject");

            libProject.SaveProjectListFile();

            sSelectedProject = project.directory;

            //Close();

			return true;
		}

        public static Process SeekProcess(string processname)
        {
            Process[] p;

            p = Process.GetProcessesByName(processname);

            if (p.Length > 0)
            {
                return p[0];
            }

            p = Process.GetProcessesByName(processname + ".vshost");
            if (p.Length > 0)
            {
                return p[0];
            }

            return null;
        }

        public static void ShutDownProcess(string processname)
        {
            Process p = SeekProcess(processname);
            if (p == null) return;
            p.Kill();
        }

        bool CloseAnotherProgram(string processname, bool shutdown)
        {
            Process p = SeekProcess(processname);

            if (p != null)
            {
                if (shutdown)
                {
                    ShutDownProcess(processname);
                    return true;
                }

                Win32Function.PostMessage(p.MainWindowHandle, 0x111, (IntPtr)(int)EnumIdmPublic.IDM_PUBLIC_DESTROY_WINDOW, (IntPtr)12345678);

                TimeOutClass timeout = new TimeOutClass();
                while (true)
                {
                    if (timeout.IsTimeOut(10))
                    {
                        string msg;

                        if (Tools.IsLangKorean())
                            msg = String.Format("{0} 프로그램을 종료할 수 없습니다.\n강제로 종료할까요?", processname);
                        else
                            msg = "Can't close the " + processname + " Program.\nShut down the program?";

                        if (MessageBox.Show(msg, "error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ShutDownProcess(processname);
                            return true;
                        }
                        return false;
                    }

                    Thread.Sleep(10);
                    p = SeekProcess(processname);
                    if (p == null) break;
                }
            }

            return true;
        }

        void Run()
        {
            if (!RunOrEdit()) return;

            if (this.bCallByStudio)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                Process p = SeekProcess("LocalMain");

                if (p != null)
                {
                    if (Tools.IsLangKorean())
                    {
                        if (MessageBox.Show(this, "감시 프로그램이 현재 실행 중입니다.\n감시 프로그램을 재시작 할까요?", "실행 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;
                    }
                    else
                    {
                        if (MessageBox.Show(this, "LocalMain program is running.\nRestart LocalMain program?", "Restart", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;
                    }
                }

                if (!CloseAnotherProgram("LocalMain", false)) return;
            }

            Close();

            string path = String.Format("{0}\\LocalMain.exe", Application.StartupPath);

            System.Diagnostics.Process.Start(path);
        }

		private void buttonRun_Click(object sender, System.EventArgs e)
		{
            Run();
		}

		private void buttonEdit_Click(object sender, System.EventArgs e)
		{
            if (this.bCallByStudio)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

			if(!RunOrEdit())	return;

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                Process p = SeekProcess("Studio");

                if (p != null)
                {
                    if (Tools.IsLangKorean())
                    {
                        if (MessageBox.Show(this, "스튜디오 프로그램이 현재 실행 중입니다.\n스튜디오 프로그램을 재시작 할까요?", "실행 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;
                    }
                    else
                    {
                        if (MessageBox.Show(this, "Studio program is running.\nRestart Studio program?", "Restart", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;
                    }
                }

                if (!CloseAnotherProgram("Studio", true)) return;
            }

            Close();

			string path = String.Format("{0}\\Studio.exe", Application.StartupPath);

			System.Diagnostics.Process.Start(path);
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
            if (this.bCallByStudio)
                Run();
            else
			    Modify();
		}

		private void menuItemFileBackup_Click(object sender, System.EventArgs e)
		{
            PROJECT_STRUCT project = GetSelectedProject();

            if (project == null)
            {
                if (Tools.IsLangKorean()) 
				{
					MessageBox.Show("백업할 작업을 선택하세요.", "백업 오류");
				}
				else 
				{
					MessageBox.Show("Backup after select one project.", "Backup error");
				}
				return;
			}

			FormBackUp dialog = new FormBackUp();

			dialog.sProjectTitle = project.title;
			dialog.sProjectDir = project.directory;
            dialog.nPlatform = project.nPlatform;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
		}

		private void menuItemFileRestore_Click(object sender, System.EventArgs e)
		{
			FormRestore dialog = new FormRestore();

			dialog.listParent = this.m_list;
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				if(dialog.bAddFlag) 
				{
                    libProject.projectBlock.Add(dialog.projectAdd);
					ListViewItem lvi = new ListViewItem("");
					lvi.SubItems.Add("");
					lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
					this.ChangeOneItem(lvi, dialog.projectAdd);
                    lvi.Tag = dialog.projectAdd;
                    m_list.Items.Add(lvi);
					lvi.Selected = true;
					lvi.EnsureVisible();
				}

				m_list.Select();
			}
		}

        private void menuItemHelpAbout_Click(object sender, EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();

            dialog.ProgramIcon = this.Icon;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                dialog.sFixTextProgram = "SBAS 작업선택";

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void m_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void menuItemImportFromSmartDevice_Click(object sender, EventArgs e)
        {
            FormImportFromSmartDevice dialog = new FormImportFromSmartDevice();

            dialog.listParent = this.m_list;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.bAddFlag)
                {
                    libProject.projectBlock.Add(dialog.projectAdd);
                    ListViewItem lvi = new ListViewItem("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    this.ChangeOneItem(lvi, dialog.projectAdd);
                    lvi.Tag = dialog.projectAdd;
                    m_list.Items.Add(lvi);
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                }

                m_list.Select();
            }
        }

        private int _sortColumn = -1;
        private SortOrder _sortOrder = SortOrder.None;
        private void m_list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 같은 컬럼 클릭 → 정렬 방향 토글
            if (e.Column == _sortColumn)
            {
                _sortOrder = (_sortOrder == SortOrder.Ascending)
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _sortColumn = e.Column;
                _sortOrder = SortOrder.Ascending;
            }

            m_list.ListViewItemSorter = new ListViewItemComparer(_sortColumn, _sortOrder);
            m_list.Sort();
        }

        public class ListViewItemComparer : IComparer
        {
            private int _col;
            private SortOrder _order;

            public ListViewItemComparer(int column, SortOrder order)
            {
                _col = column;
                _order = order;
            }

            public int Compare(object x, object y)
            {
                ListViewItem itemX = (ListViewItem)x;
                ListViewItem itemY = (ListViewItem)y;

                string sx = itemX.SubItems[_col].Text;
                string sy = itemY.SubItems[_col].Text;

                int result = String.Compare(sx, sy, StringComparison.CurrentCultureIgnoreCase);

                if (_order == SortOrder.Descending)
                    result = -result;

                return result;
            }
        }
    }

    public class PROJECT_STRUCT
	{
		public string title;
		public string directory;
		public string used_date;
        public int nPlatform;
	}
}


