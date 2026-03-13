using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using AutoLibLocal;
using ReportModule;
using Studio.Script;
using System.Collections.Generic;
using NetTools;
using Studio.Solution;

namespace Studio
{
	/// <summary>
	/// Summary description for FormSolution.
	/// </summary>
	public class FormSolution : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView treeViewSolution;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemRefresh;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemRename;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemNewFolder;
        private MenuItem menuItem2;
        private MenuItem menuItemAddScriptProject;
        private MenuItem menuItemNewScriptProject;
        private MenuItem menuItemExistingScriptProject;
        private MenuItem menuItemAddScriptItem;
        private MenuItem menuItemNewScriptClass;
        private MenuItem menuItemExistingScriptClass;
        private MenuItem menuItemScriptProjectProperties;
		private System.ComponentModel.IContainer components;

		public FormSolution()
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSolution));
            this.treeViewSolution = new System.Windows.Forms.TreeView();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemOpen = new System.Windows.Forms.MenuItem();
            this.menuItemRefresh = new System.Windows.Forms.MenuItem();
            this.menuItemRename = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItemNewFolder = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemAddScriptProject = new System.Windows.Forms.MenuItem();
            this.menuItemNewScriptProject = new System.Windows.Forms.MenuItem();
            this.menuItemExistingScriptProject = new System.Windows.Forms.MenuItem();
            this.menuItemAddScriptItem = new System.Windows.Forms.MenuItem();
            this.menuItemNewScriptClass = new System.Windows.Forms.MenuItem();
            this.menuItemExistingScriptClass = new System.Windows.Forms.MenuItem();
            this.menuItemScriptProjectProperties = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeViewSolution
            // 
            this.treeViewSolution.ContextMenu = this.contextMenu1;
            resources.ApplyResources(this.treeViewSolution, "treeViewSolution");
            this.treeViewSolution.HideSelection = false;
            this.treeViewSolution.ImageList = this.imageList1;
            this.treeViewSolution.ItemHeight = 16;
            this.treeViewSolution.LabelEdit = true;
            this.treeViewSolution.Name = "treeViewSolution";
            this.treeViewSolution.Sorted = true;
            this.treeViewSolution.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewSolution_AfterLabelEdit);
            this.treeViewSolution.DoubleClick += new System.EventHandler(this.treeViewSolution_DoubleClick);
            this.treeViewSolution.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewSolution_BeforeLabelEdit);
            this.treeViewSolution.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeViewSolution_KeyDown);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemOpen,
            this.menuItemRefresh,
            this.menuItemRename,
            this.menuItemDelete,
            this.menuItemNewFolder,
            this.menuItem2,
            this.menuItemAddScriptProject,
            this.menuItemAddScriptItem,
            this.menuItemScriptProjectProperties,
            this.menuItem3,
            this.menuItem1});
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // menuItemOpen
            // 
            this.menuItemOpen.Index = 0;
            resources.ApplyResources(this.menuItemOpen, "menuItemOpen");
            this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
            // 
            // menuItemRefresh
            // 
            this.menuItemRefresh.Index = 1;
            resources.ApplyResources(this.menuItemRefresh, "menuItemRefresh");
            this.menuItemRefresh.Click += new System.EventHandler(this.menuItemRefresh_Click);
            // 
            // menuItemRename
            // 
            this.menuItemRename.Index = 2;
            resources.ApplyResources(this.menuItemRename, "menuItemRename");
            this.menuItemRename.Click += new System.EventHandler(this.menuItemRename_Click);
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Index = 3;
            resources.ApplyResources(this.menuItemDelete, "menuItemDelete");
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // menuItemNewFolder
            // 
            this.menuItemNewFolder.Index = 4;
            resources.ApplyResources(this.menuItemNewFolder, "menuItemNewFolder");
            this.menuItemNewFolder.Click += new System.EventHandler(this.menuItemNewFolder_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 5;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItemAddScriptProject
            // 
            this.menuItemAddScriptProject.Index = 6;
            this.menuItemAddScriptProject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNewScriptProject,
            this.menuItemExistingScriptProject});
            resources.ApplyResources(this.menuItemAddScriptProject, "menuItemAddScriptProject");
            // 
            // menuItemNewScriptProject
            // 
            this.menuItemNewScriptProject.Index = 0;
            resources.ApplyResources(this.menuItemNewScriptProject, "menuItemNewScriptProject");
            this.menuItemNewScriptProject.Click += new System.EventHandler(this.menuItemNewScriptProject_Click);
            // 
            // menuItemExistingScriptProject
            // 
            this.menuItemExistingScriptProject.Index = 1;
            resources.ApplyResources(this.menuItemExistingScriptProject, "menuItemExistingScriptProject");
            this.menuItemExistingScriptProject.Click += new System.EventHandler(this.menuItemExistingScriptProject_Click);
            // 
            // menuItemAddScriptItem
            // 
            this.menuItemAddScriptItem.Index = 7;
            this.menuItemAddScriptItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNewScriptClass,
            this.menuItemExistingScriptClass});
            resources.ApplyResources(this.menuItemAddScriptItem, "menuItemAddScriptItem");
            // 
            // menuItemNewScriptClass
            // 
            this.menuItemNewScriptClass.Index = 0;
            resources.ApplyResources(this.menuItemNewScriptClass, "menuItemNewScriptClass");
            this.menuItemNewScriptClass.Click += new System.EventHandler(this.menuItemNewScriptClass_Click);
            // 
            // menuItemExistingScriptClass
            // 
            this.menuItemExistingScriptClass.Index = 1;
            resources.ApplyResources(this.menuItemExistingScriptClass, "menuItemExistingScriptClass");
            this.menuItemExistingScriptClass.Click += new System.EventHandler(this.menuItemExistingScriptClass_Click);
            // 
            // menuItemScriptProjectProperties
            // 
            this.menuItemScriptProjectProperties.Index = 8;
            resources.ApplyResources(this.menuItemScriptProjectProperties, "menuItemScriptProjectProperties");
            this.menuItemScriptProjectProperties.Click += new System.EventHandler(this.menuItemScriptProjectProperties_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 9;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 10;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(233)))), ((int)(((byte)(216)))));
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "project.png");
            this.imageList1.Images.SetKeyName(7, "cs.png");
            this.imageList1.Images.SetKeyName(8, "Z_IoT_Edit.ico");
            // 
            // FormSolution
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.treeViewSolution);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSolution";
            this.Load += new System.EventHandler(this.FormSolution_Load);
            this.ResumeLayout(false);

		}
		#endregion

		void AddGraphics(string path, TreeNode root)
		{
			if(!Directory.Exists(path))	return;

			DirectoryInfo info = new DirectoryInfo(path);

			foreach(DirectoryInfo di in info.GetDirectories()) 
			{
				TreeNode node = new TreeNode(di.Name);
				node.ImageIndex = 0;
				node.SelectedImageIndex = 0;
				node.Tag = "DIR";
				root.Nodes.Add(node);
				AddGraphics(path+"\\"+di.Name, node);
			}

			foreach(FileInfo fi in info.GetFiles("*.mod?")) 
			{
				TreeNode node = new TreeNode(fi.Name);
                if (TotalConfig.eOemType == EnumOemType.ZIoT)
                {
                    node.ImageIndex = 8;
                    node.SelectedImageIndex = 8;
                }
                else
                {
                    if (String.Compare(".mod", Path.GetExtension(fi.Name), true) == 0)
                    {
                        node.ImageIndex = 2;
                        node.SelectedImageIndex = 2;
                    }
                    else
                    {
                        node.ImageIndex = 3;
                        node.SelectedImageIndex = 3;
                    }
                }
				root.Nodes.Add(node);
			}

            foreach (FileInfo fi in info.GetFiles("*.rmod"))
            {
                TreeNode node = new TreeNode(fi.Name);

                node.ImageIndex = 3;
                node.SelectedImageIndex = 3;
                
                root.Nodes.Add(node);
            }
		}

		void AddReports(string path, TreeNode root)
		{
			if(!Directory.Exists(path))	return;

			DirectoryInfo info = new DirectoryInfo(path);

			foreach(DirectoryInfo di in info.GetDirectories()) 
			{
				TreeNode node = new TreeNode(di.Name);
				node.ImageIndex = 0;
				node.SelectedImageIndex = 0;
				node.Tag = "DIR";
				root.Nodes.Add(node);
				AddReports(path+"\\"+di.Name, node);
			}

			foreach(FileInfo fi in info.GetFiles("*.rpt?")) 
			{
				TreeNode node = new TreeNode(fi.Name);
				if(String.Compare(".rpt", Path.GetExtension(fi.Name), true) == 0) 
				{
					node.ImageIndex = 4;
					node.SelectedImageIndex = 4;
				}
				else 
				{
					node.ImageIndex = 5;
					node.SelectedImageIndex = 5;
				}
				root.Nodes.Add(node);
			}
		}

        

        void FillTree()
		{
			this.treeViewSolution.Nodes.Clear();
			TreeNode node = treeViewSolution.Nodes.Add("Graphic");
			node.ImageIndex = 0;
			node.SelectedImageIndex = 0;
			node.Tag = "GRAPHIC";
			AddGraphics(TotalConfig.sDirWorkProject+"\\Graphic", node);

			node = treeViewSolution.Nodes.Add("Report");
			node.ImageIndex = 0;
			node.SelectedImageIndex = 0;
			node.Tag = "REPORT";
			AddReports(TotalConfig.sDirWorkProject+"\\Report", node);

            if (NextVersion.bScript11)
            {
                node = treeViewSolution.Nodes.Add("Script");
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                node.Tag = "ScriptSolution";
                AddScriptSolution(node);
            }
		}

		private void FormSolution_Load(object sender, System.EventArgs e)
		{
			FillTree();
		}

		void OpenFile()
		{
			TreeNode node = this.treeViewSolution.SelectedNode;

            if (node == null) return;       // 마우스를 마구 클릭하면 선택된 것이 취소될 수도 있다.

			string ext = Path.GetExtension(node.Text);
			if(String.Compare(ext, ".mod", true) == 0 ||
				String.Compare(ext, ".modx", true) == 0) 
			{
				string file = TotalConfig.sDirWorkProject+"\\"+node.FullPath;
				SharedStudio.formMain.OpenModule(file);
			}

			if(String.Compare(ext, ".rpt", true) == 0 ||
				String.Compare(ext, ".rptx", true) == 0) 
			{
				string file = TotalConfig.sDirWorkProject+"\\"+node.FullPath;
				SharedStudio.formMain.OpenReport(file);
			}

            if (String.Compare(ext, ".cs", true) == 0)
            {
                string dir;
                ScriptProject project = GetProjectClassFromSelectedNode(out dir);

                if (project != null)
                {
                    string file_path;
                    
                    if(dir.Length == 0)
                        file_path = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), node.Text);
                    else
                        file_path = String.Format("{0}\\{1}\\{2}", Path.GetDirectoryName(project.sProjectFilename), dir, node.Text);

                    SharedStudio.formMain.OpenScript(file_path);
                }
            }
		}

		private void treeViewSolution_DoubleClick(object sender, System.EventArgs e)
		{
			OpenFile();
		}

		void RecurseSeekFile(TreeNodeCollection nodes, string name)
		{
			TreeNode node;
			for(int i = 0; i < nodes.Count; i++) 
			{
				node = nodes[i];
				if(String.Compare(node.FullPath, name, true) == 0) 
				{
					node.TreeView.SelectedNode = node;
					return;
				}
				RecurseSeekFile(node.Nodes, name);
			}
		}

        public void MdiActivate(string filepath)
		{
			string name;

			if(Path.IsPathRooted(filepath))	// fullpath
			{
				if(String.Compare(filepath, 0, TotalConfig.sDirWorkProject, 0, TotalConfig.sDirWorkProject.Length, true) == 0) 
				{
					name = filepath.Substring(TotalConfig.sDirWorkProject.Length+1);
				}
				else	// 작업폴더가 아닌 다른 폴더의 파일이다.
				{
					return;
				}
			}
			else 
			{
				name = filepath;
				string ext = Path.GetExtension(filepath);
				if(String.Compare(ext, ".mod", true) == 0 ||
					String.Compare(ext, ".modx", true) == 0) 
				{
					name = "Graphic\\"+filepath;
				}
				else if(String.Compare(ext, ".rpt", true) == 0 || 
					String.Compare(ext, ".rptx", true) == 0) 
				{
					name = "Report\\"+filepath;
				}
				else 
				{
					name = filepath;
				}
			}

			RecurseSeekFile(this.treeViewSolution.Nodes, name);
		}

		public void ReLoad()
		{
			FillTree();

			Form child = SharedStudio.formMain.ActiveMdiChild;
			string argument = "";
			if(child != null) 
			{
				if(child.Name == "FormEditGraphicFrame")
				{
					FormEditGraphicFrame form = (FormEditGraphicFrame)child;

					argument = form.formChild.workThis.filename;
				}
				else if(child.Name == "FormReportMainEdit")
				{
					FormReportMainEdit form = (FormReportMainEdit)child;

					argument = form.formChild.sFilename;
				}
                else if (child.GetType() == typeof(Studio.Script.FormNewScriptEditor))
                {
                    FormNewScriptEditor form = (FormNewScriptEditor)child;

                    argument = form.formChild.panelEditor.sFilename;
                }
				else {}
			}

			if(argument.Length > 0) 
			{
				MdiActivate(argument);
			}
		}

		private void menuItemRefresh_Click(object sender, System.EventArgs e)
		{
			ReLoad();
		}

		private void treeViewSolution_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter) 
			{
				OpenFile();
			}
			else if(e.KeyCode == Keys.Delete)
			{
				Delete();
			}
		}

		private void menuItemOpen_Click(object sender, System.EventArgs e)
		{
			OpenFile();		
		}

		private void contextMenu1_Popup(object sender, System.EventArgs e)
		{
			TreeNode node = this.treeViewSolution.SelectedNode;

			bool open_flag = false;
            bool flag_script_solution = false;
            bool flag_script_project = false;

			if(node != null) 
			{
				string ext = Path.GetExtension(node.Text);

				if(String.Compare(ext, ".mod", true) == 0 ||
					String.Compare(ext, ".modx", true) == 0) 
				{
					open_flag = true;
				}
				else if(String.Compare(ext, ".rpt", true) == 0 ||
					String.Compare(ext, ".rptx", true) == 0) 
				{
					open_flag = true;
				}
                else if (String.Compare(ext, ".cs", true) == 0)
                {
                    open_flag = true;
                }
				else 
				{
					open_flag = false;
				}

                if ((string)node.Tag == "ScriptSolution")
                    flag_script_solution = true;

                if ((string)node.Tag == "ScriptProject")
                    flag_script_project = true;    
			}

			this.menuItemOpen.Enabled = open_flag;
			this.menuItemRename.Enabled = this.IsAbleRename(node);
			this.menuItemDelete.Enabled = this.IsAbleDelete(node);
			this.menuItemNewFolder.Enabled = this.IsAbleNewFolder(node);

            this.menuItemAddScriptProject.Visible = flag_script_solution;
            this.menuItemAddScriptItem.Visible = IsAbleAddScriptItem(node);
            this.menuItemScriptProjectProperties.Visible = flag_script_project;
		}

        bool IsAbleAddScriptItem(TreeNode node)
        {
            if (node == null) return false;

            if ((string)node.Tag == "ScriptProject")
                return true;

            if ((string)node.Tag == "DIR")
            {
                string dir;
                if (GetProjectClassFromSelectedNode(out dir) != null)
                {
                    return true;
                }
            }

            return false;
        }

		bool IsAbleRename(TreeNode node)
		{
			if(node == null)	return false;
			string ext = Path.GetExtension(node.Text);

			bool flag = true;
			if(String.Compare(ext, ".mod", true) == 0 ||
				String.Compare(ext, ".modx", true) == 0) 
			{
				flag = true;
			}
			else if(String.Compare(ext, ".rpt", true) == 0 ||
				String.Compare(ext, ".rptx", true) == 0) 
			{
				flag = true;
			}
            else if (String.Compare(ext, ".cs", true) == 0)
            {
                flag = true;
            }
			else if(node.Tag != null && (string)node.Tag == "DIR") 
			{
				flag = true;
			}
			else 
			{
				flag = false;
			}

			return flag;
		}

		private void menuItemRename_Click(object sender, System.EventArgs e)
		{
			TreeNode node = this.treeViewSolution.SelectedNode;

			if(!IsAbleRename(node))	return;

			node.BeginEdit();
		}

		private void treeViewSolution_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			TreeNode node = this.treeViewSolution.SelectedNode;

			if(!IsAbleRename(node))
				e.CancelEdit = true;
		}

		// 확장자는 항상 같게 일치시켜야 한다. 확장자가 바뀌면 안된다.
		private void treeViewSolution_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			TreeNode node = e.Node;
			
			// 실제 여기는 들어오지 않지만 만약을 위해서
		
			if(!IsAbleRename(node)) 
			{
				e.CancelEdit = true;
				return;
			}

			if(e.Label == null)	return;

			string text = e.Label.Trim();

			if(text.Length == 0)
			{
				e.CancelEdit = true;
				return;
			}

			string new_path;
			string old_path;

			// 폴더 이름 바꾸기
			if(node.Tag != null && (string)node.Tag == "DIR") 
			{
				new_path = TotalConfig.sDirWorkProject+"\\"+node.Parent.FullPath+"\\"+text;
				old_path = TotalConfig.sDirWorkProject+"\\"+node.FullPath;

				if(Directory.Exists(new_path)) 
				{
					if(NetTools.Tools.IsLangKorean()) 
					{
						MessageBox.Show("같은 폴더 이름이 이미 존재합니다.", "이름 중복");
					}
					if(NetTools.Tools.IsLangChinese()) 
					{
						MessageBox.Show("同样的文件夹名已存在。", "名冗余错误"); 
					}
					else 
					{
						MessageBox.Show("Same directory already exists.", "Name already exist");
					}
					e.CancelEdit = true;
					return;
				}

                try
                {
                    Directory.Move(old_path, new_path);
                }
                catch (Exception exception)
                {
                    string msg = String.Format("Can't rename the folder.\nError={0}", exception.Message);
                    MessageBox.Show(msg, "Folder rename error");
                    e.CancelEdit = true;
                    return;
                }

				return;
			}

			string ext = Path.GetExtension(node.Text);

			string new_file = Path.GetFileNameWithoutExtension(text)+ext;
			string old_file = node.Text;

			// 같은 이름이다.
			if(String.Compare(new_file, node.Text, true) == 0) 
			{
				e.CancelEdit = true;
				return;
			}

			new_path = TotalConfig.sDirWorkProject+"\\"+node.Parent.FullPath+"\\"+new_file;

			if(File.Exists(new_path)) 
			{
				if(NetTools.Tools.IsLangKorean()) 
				{
					MessageBox.Show("같은 파일 이름이 이미 존재합니다.", "이름 중복");
				}
				else if(NetTools.Tools.IsLangChinese()) 
				{
					MessageBox.Show("同样的文件名已存在。", "名冗余错误");
				}
				else 
				{
					MessageBox.Show("Same name already exists.", "Name already exist");
				}
				e.CancelEdit = true;
				return;
			}

			old_path = TotalConfig.sDirWorkProject+"\\"+node.FullPath;

			File.Move(old_path, new_path);
			
			// 부가 파일 변경
			if(String.Compare(ext, ".mod", true) == 0) 
			{
				RenameModuleControlFile(old_path, new_path, ".ctl");
			}
			else if(String.Compare(ext, ".modx", true) == 0) 
			{
				RenameModuleControlFile(old_path, new_path, ".ctlx");
			}
			else 
			{

			}
			
			e.CancelEdit = true;
			node.Text = new_file;

			// 열려있는 MDI창의 파일명도 바꾸어 준다.
			Form child;
			
			for(int i = 0; i < SharedStudio.formMain.MdiChildren.Length; i++) 
			{
				child = SharedStudio.formMain.MdiChildren[i];

				string argument = "";

				if(child != null) 
				{
					if(child.Name == "FormEditGraphicFrame")
					{
						FormEditGraphicFrame form = (FormEditGraphicFrame)child;

						argument = form.formChild.workThis.filename;

						if(String.Compare(argument, old_path, true) == 0) 
						{
							form.formChild.workThis.filename = new_path;
							form.formChild.SetTitle();
							break;
						}
					}
					else if(child.Name == "FormReportMainEdit")
					{
						FormReportMainEdit form = (FormReportMainEdit)child;

						argument = form.formChild.sFilename;

						if(String.Compare(argument, old_path, true) == 0) 
						{
							form.formChild.sFilename = new_path;
							form.formChild.SetTitle();
							break;
						}
					}
					else {}
				}
			}
		}

		void RenameOneControlFile(string script_dir, string sub_dir, string old_name, string new_name, string ext)
		{
			string old_path;
			string new_path;

			if(sub_dir.Length == 0) 
			{
				old_path = TotalConfig.sDirWorkProject+"\\control\\"+script_dir+"\\"+old_name+ext;
				new_path = TotalConfig.sDirWorkProject+"\\control\\"+script_dir+"\\"+new_name+ext;
			}
			else 
			{
				old_path = TotalConfig.sDirWorkProject+"\\control\\"+script_dir+"\\"+sub_dir+"\\"+old_name+ext;
				new_path = TotalConfig.sDirWorkProject+"\\control\\"+script_dir+"\\"+sub_dir+"\\"+new_name+ext;
			}

			if(!File.Exists(old_path))	return;	// 파일이 존재하지 않는 경우는 이름을 바꿀 필요가 없다.
			if(File.Exists(new_path)) 
			{
				File.Delete(new_path);	// 지난파일 삭제
			}

			File.Move(old_path, new_path);
		}

		void RenameModuleControlFile(string old_path, string new_path, string ext)
		{
			string sub_dir = Path.GetDirectoryName(old_path);

			if(TotalConfig.sDirWorkProject.Length+8 == sub_dir.Length)	// 그래픽 하위 폴더가 없다.
				sub_dir = "";
			else
				sub_dir = sub_dir.Substring(TotalConfig.sDirWorkProject.Length+9);	// 9=\GRAPHIC\

			string old_name = Path.GetFileNameWithoutExtension(old_path);
			string new_name = Path.GetFileNameWithoutExtension(new_path);

			RenameOneControlFile("ModAct", sub_dir, old_name, new_name, ext);
			RenameOneControlFile("ModAlway", sub_dir, old_name, new_name, ext);
			RenameOneControlFile("ModEnd", sub_dir, old_name, new_name, ext);
			RenameOneControlFile("ModNoAct", sub_dir, old_name, new_name, ext);
			RenameOneControlFile("ModStart", sub_dir, old_name, new_name, ext);
		}

		bool IsAbleDelete(TreeNode node)
		{
			if(node == null)	return false;

			string ext = Path.GetExtension(node.Text);

			bool flag = true;
			if(String.Compare(ext, ".mod", true) == 0 ||
				String.Compare(ext, ".modx", true) == 0) 
			{
				flag = true;
			}
			else if(String.Compare(ext, ".rpt", true) == 0 ||
				String.Compare(ext, ".rptx", true) == 0) 
			{
				flag = true;
			}
            else if (String.Compare(ext, ".cs", true) == 0)
            {
                flag = true;
            }
			else if(node.Tag != null && (string)node.Tag == "DIR") 
			{
				flag = true;
			}
            else if (node.Tag != null && (string)node.Tag == "ScriptDir")
            {
                flag = true;
            }
            else if (node.Tag != null && (string)node.Tag == "ScriptProject")
            {
                flag = true;
            }
			else 
			{
				flag = false;
			}

			return flag;
		}

		void Delete()
		{
			TreeNode node = this.treeViewSolution.SelectedNode;

			if(!IsAbleDelete(node))	return;

            string path;

            if (node.Tag != null && (string)node.Tag == "ScriptCs")
            {
                string dir;
                ScriptProject project = GetProjectClassFromSelectedNode(out dir);

                string name;

                if (dir.Length == 0)
                {
                    path = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), node.Text);
                    name = String.Format("{0}", node.Text);
                }
                else
                {
                    path = String.Format("{0}\\{1}\\{2}", Path.GetDirectoryName(project.sProjectFilename), dir, node.Text);
                    name = String.Format("{0}\\{1}", dir, node.Text);
                }

                for (int i = 0; i < project.arraySource.Count; i++)
                {
                    if (String.Compare(project.arraySource[i], name, true) == 0)
                    {
                        project.arraySource.RemoveAt(i);
                        project.SaveScriptProject();
                        break;
                    }
                }

                FileTool.FileDeleteSafety(path);
                node.Remove();

                return;
            }

            if (node.Tag != null && (string)node.Tag == "ScriptDir")
            {
                string dir;
                ScriptProject project = GetProjectClassFromSelectedNode(out dir);

                path = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), dir);

                FileTool.FolderDeleteSafety(path);
                node.Remove();

                return;
            }

			path = TotalConfig.sDirWorkProject+"\\"+node.FullPath;

			// 폴더인 경우
			if(node.Tag != null && (string)node.Tag == "DIR") 
			{
				try 
				{
					Directory.Delete(path);
				}
				catch 
				{
					if(NetTools.Tools.IsLangKorean()) 
						MessageBox.Show("폴더를 삭제할 수 없습니다.\n비어 있지 않거나 삭제할 수 없는 폴더입니다.", "삭제오류");
					else if(NetTools.Tools.IsLangChinese()) 
						MessageBox.Show("不能删除文件夹。\n 不空着或不能删除的文件夹。", "删除错误");
					else
						MessageBox.Show("Can't remove folder.\nFolder not empty or protected folder.", "Delete error");

					return;
				}

				node.Remove();
				return;
			}

            // 스크립트 프로젝트인 경우
            if (node.Tag != null && (string)node.Tag == "ScriptProject")
            {
                for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
                {
                    if (node.Text == Path.GetFileNameWithoutExtension(ScriptSolution.arrayProjects[i].sProjectFilename))
                    {
                        node.Remove();
                        ScriptSolution.arrayProjects.RemoveAt(i);
                        ScriptSolution.SaveScriptSolution();
                        return;
                    }
                }

                return;
            }

			string ext = Path.GetExtension(node.Text);

			DialogResult result;

			if(NetTools.Tools.IsLangKorean()) 
			{
				string msg = String.Format("삭제한 파일은 {0} 에 보관됩니다.\n{1} 파일을 삭제할까요?", BackUp.GetBackupDirectory(path), node.Text);
				result = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo);
			}
			else if(NetTools.Tools.IsLangChinese()) 
			{
				string msg = String.Format("将删除的文件保存为{0}。\n是否删除{1}文件？", BackUp.GetBackupDirectory(path), node.Text);
				result = MessageBox.Show(msg, "删除确认", MessageBoxButtons.YesNo);
			}
			else 
			{
				string msg = String.Format("Are you sure you want to delete the {1} file?\nBackup directory={0}", BackUp.GetBackupDirectory(path), node.Text);
				result = MessageBox.Show(msg, "File Delete", MessageBoxButtons.YesNo);
			}

			if(result != DialogResult.Yes)	return;

			BackUp.BackUpFile(path);
			File.Delete(path);
			
			// 부가 파일 변경
			if(String.Compare(ext, ".mod", true) == 0) 
			{
				DeleteModuleControlFile(path, ".ctl");
			}
			else if(String.Compare(ext, ".modx", true) == 0) 
			{
				DeleteModuleControlFile(path, ".ctlx");
			}
			else 
			{

			}

			node.Remove();

			// 열려있는 MDI창의 파일도 닫아준다.
			Form child;
			
			for(int i = 0; i < SharedStudio.formMain.MdiChildren.Length; i++) 
			{
				child = SharedStudio.formMain.MdiChildren[i];

				string argument = "";

				if(child != null) 
				{
					if(child.Name == "FormEditGraphicFrame")
					{
						FormEditGraphicFrame form = (FormEditGraphicFrame)child;

						argument = form.formChild.workThis.filename;

						if(String.Compare(argument, path, true) == 0) 
						{
							form.Close();
							break;
						}
					}
					else if(child.Name == "FormReportMainEdit")
					{
						FormReportMainEdit form = (FormReportMainEdit)child;

						argument = form.formChild.sFilename;

						if(String.Compare(argument, path, true) == 0) 
						{
							form.Close();
							break;
						}
					}
                    else if (child.GetType() == typeof(FormNewScriptEditor))
                    {
                        FormNewScriptEditor form = (FormNewScriptEditor)child;

                        argument = form.formChild.panelEditor.sFilename;

                        if (String.Compare(argument, path, true) == 0)
                        {
                            form.Close();
                            break;
                        }
                    }
					else {}
				}
			}

            // 현재 활성화 되어있지 않는 모듈을 삭제하고 나면 선택이 다른 위치로 가면서 삭제한 후 활성화 되어 있는 모듈이 솔루션에 나타나지 않는 문제점 해결
            if (SharedStudio.formMain.MdiChildren.Length > 0)
            {
                Form c = SharedStudio.formMain.ActiveMdiChild;

                if (c.GetType() == typeof(FormEditGraphicFrame))
                {
                    FormEditGraphicFrame f = (FormEditGraphicFrame)c;

                    f.lpfnCallOnMdiActivated(f.formChild, f.formChild.workThis.filename, f.formChild.workThis.obj.groupRoot);
                }
                else if (c.GetType() == typeof(ReportModule.FormReportMainEdit))
                {
                    FormReportMainEdit f = (FormReportMainEdit)c;

                    f.lpfnCallOnMdiActivated(f.formChild, f.formChild.sFilename, null);
                }
            }
		}

		private void menuItemDelete_Click(object sender, System.EventArgs e)
		{
			Delete();
		}

		void DeleteOneControlFile(string script_dir, string sub_dir, string name, string ext)
		{
			string path;
			
			if(sub_dir.Length == 0)
				path = TotalConfig.sDirWorkProject+"\\control\\"+script_dir+"\\"+name+ext;
			else
				path = TotalConfig.sDirWorkProject+"\\control\\"+script_dir+"\\"+sub_dir+"\\"+name+ext;

			if(!File.Exists(path))	return;	// 파일이 존재하지 않는 경우는 삭제를 할 필요가 없다.

			BackUp.BackUpFile(path);

			File.Delete(path);
		}

		void DeleteModuleControlFile(string path, string ext)
		{
			string sub_dir = Path.GetDirectoryName(path); 

			if(TotalConfig.sDirWorkProject.Length+8 == sub_dir.Length)	// 그래픽 하위 폴더가 없다.
				sub_dir = "";
			else
				sub_dir = sub_dir.Substring(TotalConfig.sDirWorkProject.Length+9);	// 9=\GRAPHIC\

			string name = Path.GetFileNameWithoutExtension(path);

			DeleteOneControlFile("ModAct", sub_dir, name, ext);
			DeleteOneControlFile("ModAlway", sub_dir, name, ext);
			DeleteOneControlFile("ModEnd", sub_dir, name, ext);
			DeleteOneControlFile("ModNoAct", sub_dir, name, ext);
			DeleteOneControlFile("ModStart", sub_dir, name, ext);
		}

		private void menuItemNewFolder_Click(object sender, System.EventArgs e)
		{
			TreeNode node = this.treeViewSolution.SelectedNode;
			
			if(!IsAbleNewFolder(node))	return;

			string name = "";

			int count = 1;

			while(true) 
			{
				if(NetTools.Tools.IsLangKorean()) 
				{
					if(count == 1)	name = "새 폴더";
					else			name = String.Format("새 폴더 ({0})", count);
				}
				else if(NetTools.Tools.IsLangJapanese()) 
				{
					if(count == 1)	name = "新しいフォルダ";
					else			name = String.Format("新しいフォルダ ({0})", count);
				}
				else if(NetTools.Tools.IsLangChinese()) 
				{
					if(count == 1)	name = "新建文件夹";
					else			name = String.Format("新建文件夹 ({0})", count);
				}
				else 
				{
					if(count == 1)	name = "New Folder";
					else			name = String.Format("New Folder ({0})", count);
				}

				string path;

                string dir;
                ScriptProject project = GetProjectClassFromSelectedNode(out dir);

                // script project 폴더이거나 script Project 폴더안의 폴더이다.
                if(project != null) {
                    if(dir.Length == 0)
                        path = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), name);
                    else
                        path = String.Format("{0}\\{1}\\{2}", Path.GetDirectoryName(project.sProjectFilename), dir, name);
                }
                else
                    path = TotalConfig.sDirWorkProject+"\\"+node.FullPath+"\\"+name;

				if(!Directory.Exists(path)) 
				{
					Directory.CreateDirectory(path);
					break;
				}

				count++;
			}

			TreeNode child = new TreeNode(name);
			child.ImageIndex = 0;
			child.SelectedImageIndex = 0;
			child.Tag = "DIR";
			node.Nodes.Add(child);
			this.treeViewSolution.SelectedNode = child;
		}

		bool IsAbleNewFolder(TreeNode node)
		{
			if(node == null)	return false;

            if ((string)node.Tag == "ScriptSolution") return false;

			if(node.ImageIndex == 0)	return true;
			else						return false;
		}

        

        TreeNode AddOneNodeScriptFile(TreeNode root, string name)
        {
            TreeNode node = new TreeNode(name);

            node.ImageIndex = 7;
            node.SelectedImageIndex = 7;
            node.Tag = "ScriptCs";

            root.Nodes.Add(node);

            return node;
        }

        // 폴더만 추가한다.
        void RecurseAddTreeScripts(ScriptProject project, string root_path, string path, TreeNode root)
        {
            if (!Directory.Exists(path)) return;

            DirectoryInfo info = new DirectoryInfo(path);

            foreach (DirectoryInfo di in info.GetDirectories())
            {
                TreeNode node = new TreeNode(di.Name);
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                node.Tag = "ScriptDir";
                root.Nodes.Add(node);
                RecurseAddTreeScripts(project, root_path, path + "\\" + di.Name, node);
            }

            string path_dir = path.Substring(root_path.Length);
            if (path_dir.Length > 0 && path_dir[0] == '\\')
            {
                path_dir = path_dir.Substring(1);
            }

            for (int i = 0; i < project.arraySource.Count; i++)
            {
                string dir = Path.GetDirectoryName(project.arraySource[i]);
                string name = Path.GetFileName(project.arraySource[i]);

                if (path_dir == dir)
                {
                    AddOneNodeScriptFile(root, name);
                }
            }

            /*
            foreach (FileInfo fi in info.GetFiles("*.cs"))
            {
                TreeNode node = new TreeNode(fi.Name);

                node.ImageIndex = 5;
                node.SelectedImageIndex = 5;

                root.Nodes.Add(node);
            }*/
        }

        TreeNode FillTreeOneProject(TreeNode root, ScriptProject project)
        {
            TreeNode node = root.Nodes.Add(Path.GetFileNameWithoutExtension(project.sProjectFilename));
            node.ImageIndex = 6;
            node.SelectedImageIndex = 6;
            node.Tag = "ScriptProject";

            string root_path = Path.GetDirectoryName(project.sProjectFilename);

            RecurseAddTreeScripts(project, root_path, root_path, node);

            return node;
        }

        void AddScriptSolution(TreeNode root)
        {
            ScriptSolution.LoadScriptSolution();

            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                FillTreeOneProject(root, ScriptSolution.arrayProjects[i]);
            }
        }

        /*
        ScriptProject LoadOneProject(string project_file)
        {
            ScriptProject project = new ScriptProject();
            project.sProjectFilename = project_file;

            TextReader reader = new StreamReader(project_file);

            string one_line;

            CommaTextReader comma = new CommaTextReader();
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
            }

            reader.Close();

            return project;
        }*/

        void AddOneProject(string project_file)
        {
            TreeNode node = this.treeViewSolution.SelectedNode;

            ScriptProject project = new ScriptProject();

            project.sProjectFilename = project_file;
            project.LoadScriptProject();
            ScriptSolution.arrayProjects.Add(project);

            TreeNode child_node = FillTreeOneProject(node, project);

            this.treeViewSolution.SelectedNode = child_node;

            ScriptSolution.SaveScriptSolution();
        }

        private void menuItemNewScriptProject_Click(object sender, EventArgs e)
        {
            FormNewScriptProject dialog = new FormNewScriptProject();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                AddOneProject(dialog.sSelectedProject);
            }
        }

        bool bFlagInitialDirectory = false;

        private void menuItemExistingScriptProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (!bFlagInitialDirectory)
            {
                dialog.InitialDirectory = String.Format("{0}\\Script", TotalConfig.sDirWorkProject);
                bFlagInitialDirectory = true;
            }

            dialog.Filter = "Script Project Files *.ScriptProject|*.ScriptProject";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
                {
                    if (String.Compare(ScriptSolution.arrayProjects[i].sProjectFilename, dialog.FileName, true) == 0)
                    {
                        MessageBox.Show("Same project is already included in the Solution.", "Project already exists.");
                        return;
                    }
                }

                AddOneProject(dialog.FileName);
            }
        }

        void MakeSampleFile(string filename)
        {
            TextWriter writer = new StreamWriter(filename);
            writer.Write(FormNewScriptEditor.MakeSampleCode(filename));
            writer.Close();
        }

        ScriptProject GetProjectClassFromSelectedNode(out string dir)
        {
            TreeNode node = this.treeViewSolution.SelectedNode;

            dir = "";
            string project_name = "";

            while (true)
            {
                if (node == null) return null;

                if ((string)node.Tag == "ScriptProject")
                {
                    project_name = node.Text;
                    break;
                }
                else if ((string)node.Tag == "ScriptDir")
                {
                    if (dir.Length == 0)
                        dir = node.Text;
                    else
                        dir = node.Text + "\\" + dir;
                }

                node = node.Parent;
            }

            return ScriptSolution.SeekProjectByName(project_name);
        }

        private void menuItemNewScriptClass_Click(object sender, EventArgs e)
        {
            string dir;

            ScriptProject project = GetProjectClassFromSelectedNode(out dir);

            if (project == null) return;

            string insert_folder;

            if (dir.Length == 0)
                insert_folder = String.Format("{0}", Path.GetDirectoryName(project.sProjectFilename));
            else
                insert_folder = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), dir);

            FormSelectNewClass dialog = new FormSelectNewClass(insert_folder);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string source_file;
                
                if(dir.Length == 0)
                    source_file = Path.GetFileName(dialog.sFileName);
                else
                    source_file = String.Format("{0}\\{1}", dir, Path.GetFileName(dialog.sFileName));

                TreeNode node = this.treeViewSolution.SelectedNode;

                this.treeViewSolution.SelectedNode = AddOneNodeScriptFile(node, Path.GetFileName(dialog.sFileName));

                project.arraySource.Add(source_file);
                project.SaveScriptProject();
                MakeSampleFile(dialog.sFileName);

                SharedStudio.formMain.OpenScript(dialog.sFileName);
            }
        }

        private void menuItemExistingScriptClass_Click(object sender, EventArgs e)
        {
            string dir;

            ScriptProject project = GetProjectClassFromSelectedNode(out dir);

            if (project == null) return;

            string insert_folder;
            
            if(dir.Length == 0)
                insert_folder = String.Format("{0}", Path.GetDirectoryName(project.sProjectFilename));
            else
                insert_folder = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), dir);

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Source Files (*.cs)|*.cs";
            dialog.InitialDirectory = insert_folder;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string source_file;
                string target_path;

                // 같은 폴더에 있는 파일은 그냥 삽입 하면 된다.
                if (String.Compare(insert_folder, Path.GetDirectoryName(dialog.FileName), true) == 0)
                {
                    source_file = Path.GetFileName(dialog.FileName);
                    target_path = dialog.FileName;
                }
                else
                {
                    source_file = Path.GetFileName(dialog.FileName);
                    target_path = String.Format("{0}\\{1}", insert_folder, source_file);

                    if (File.Exists(target_path))
                    {
                        MessageBox.Show("Same filename already exists.", source_file);
                        return;
                    }

                    File.Copy(dialog.FileName, target_path);
                }

                if (dir.Length == 0)
                    source_file = Path.GetFileName(dialog.FileName);
                else
                    source_file = String.Format("{0}\\{1}", dir, Path.GetFileName(dialog.FileName));

                TreeNode node = this.treeViewSolution.SelectedNode;

                this.treeViewSolution.SelectedNode = AddOneNodeScriptFile(node, Path.GetFileName(dialog.FileName));

                project.arraySource.Add(source_file);
                project.SaveScriptProject();

                SharedStudio.formMain.OpenScript(target_path);
            }
        }

        private void menuItemScriptProjectProperties_Click(object sender, EventArgs e)
        {
            string dir;

            ScriptProject project = GetProjectClassFromSelectedNode(out dir);

            if (project == null) return;

            FormConfigScriptProjectProperties dialog = new FormConfigScriptProjectProperties();

            dialog.Set(project);

            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(project);
                project.SaveScriptProject();
            }
        }
	}
}
