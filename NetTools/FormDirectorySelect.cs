using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace NetTools
{
	/// <summary>
	/// Summary description for FormDirectorySelect.
	/// </summary>
	public class FormDirectorySelect : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBoxPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxDevice;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TreeView m_tree;
        private Button buttonFolder;
        string sFileName;

		public string FileName 
		{
			get 
			{
				return sFileName;
			}
			set 
			{
				sFileName = value;
			}
		}

		public FormDirectorySelect()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDirectorySelect));
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.m_tree = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxPath
            // 
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // m_tree
            // 
            this.m_tree.HideSelection = false;
            resources.ApplyResources(this.m_tree, "m_tree");
            this.m_tree.ItemHeight = 14;
            this.m_tree.Name = "m_tree";
            this.m_tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.m_tree_AfterSelect);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxDevice, "comboBoxDevice");
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevice_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // buttonFolder
            // 
            resources.ApplyResources(this.buttonFolder, "buttonFolder");
            this.buttonFolder.Name = "buttonFolder";
            this.buttonFolder.UseVisualStyleBackColor = true;
            this.buttonFolder.Click += new System.EventHandler(this.buttonFolder_Click);
            // 
            // FormDirectorySelect
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonFolder);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxDevice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_tree);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDirectorySelect";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDirectorySelect_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		bool bWaitFlag = true;

		private void FormDirectorySelect_Load(object sender, System.EventArgs e)
		{
			string drive;
			string target;

			sFileName = sFileName.Trim();

			if(sFileName.Length == 0) 
			{
				sFileName = "C:\\";
			}

			try 
			{
				drive = Path.GetPathRoot(sFileName);
			}
			catch 
			{
				drive = "";
			}

			if(drive.Length == 0)	// 이상한 폴더를 주었을 경우
			{
				sFileName = "C:\\";
			}

			this.textBoxPath.Text = sFileName;
			target = sFileName;

			target.ToUpper();

			drive = Path.GetPathRoot(target);

			

			drive.ToUpper();

			string[] drives = Directory.GetLogicalDrives();

			for(int i = 0; i < drives.Length; i++) 
			{
				this.comboBoxDevice.Items.Add(drives[i]);
				/*
				type = GetDriveType(name);
				if(type == DRIVE_UNKNOWN)		continue;
				if(type == DRIVE_NO_ROOT_DIR)	continue;

				if(type == DRIVE_REMOTE) 
				{
					name.Format("%c: Network drive", i);
					m_combo.AddString(name);
				}
				else 
				{
					string volume;
					string name_buffer;
					DWORD lSerialNumber, lComponentLength, lFileSystemFlags;

					if(GetVolumeInformation(name, volume, sizeof(volume), 
						&lSerialNumber, &lComponentLength, &lFileSystemFlags,
						name_buffer, sizeof(name_buffer))) 
					{
						name.Format("%c: %s", i, volume);
						m_combo.AddString(name);
					}
					else 
					{
						name.Format("%c:", i);
						m_combo.AddString(name);
					}
				}
				*/

				//if(String.Compare(drives[i], drive, true) == 0) 
				//	this.comboBoxDevice.SelectedIndex = comboBoxDevice.Items.Count-1;
			}

            this.m_tree.TreeViewNodeSorter = new TreeNodeComparer();

            ReloadTree(target);

            // 최초 1회 정렬
            this.m_tree.Sort();

            this.comboBoxDevice.Text = drive[0]+":\\";

			bWaitFlag = false;
		}

        class TreeNodeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode a = (TreeNode)x;
                TreeNode b = (TreeNode)y;

                return String.Compare(a.Text, b.Text, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
		{
			sFileName = this.textBoxPath.Text;
			DialogResult = DialogResult.OK;
			Close();
		}

		void ReloadTree(string target) 
		{
			m_tree.Nodes.Clear();

			string imsi;

			imsi = String.Format("{0}:", target[0]);

			TreeNode hRoot = new TreeNode(imsi);

			m_tree.Nodes.Add(hRoot);

			RecurseTree(hRoot, imsi+"\\", target);

			m_tree.ExpandAll();

            //if(m_tree.Nodes.Count == 1) 
            //{
            //	m_tree.SelectedNode = hRoot;
            //}      
        }

		void RecurseTree(TreeNode hParent, string curr, string target)
		{
			try 
			{
				DirectoryInfo info;	
				info = new DirectoryInfo(curr);
				foreach(DirectoryInfo di in info.GetDirectories("*.*"))
				{
					TreeNode hTree = new TreeNode(di.Name);
					hParent.Nodes.Add(hTree);
		
					if(String.Compare(di.FullName, 0, target, 0, di.FullName.Length, true) == 0) 
					{
						RecurseTree(hTree, di.FullName, target);
					}

					if(String.Compare(di.FullName, target, true) == 0) 
					{
						m_tree.SelectedNode = hTree;
					}
				}
			}
			catch 
			{

			}
		}

		private void comboBoxDevice_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(bWaitFlag)	return;

			string title;

			title = this.comboBoxDevice.Text;

			string imsi;

			imsi = String.Format("{0}:", title[0]);
			ReloadTree(imsi);
			this.textBoxPath.Text = imsi;	
		}

		private void m_tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode hTree = m_tree.SelectedNode;

			string path= hTree.FullPath;

			if(hTree.Nodes.Count == 0) 
			{
				RecurseTree(hTree, path, path);
			}

			this.textBoxPath.Text = path;
		}

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select project folder";
                dlg.SelectedPath = textBoxPath.Text;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string path = dlg.SelectedPath;
                    textBoxPath.Text = path;

                    // TreeView 재갱신
                    ReloadTree(path);

                    // 드라이브 콤보도 맞춰줌
                    string drive = Path.GetPathRoot(path);
                    if (!string.IsNullOrEmpty(drive))
                        comboBoxDevice.Text = drive;
                }
            }
        }
    }
}

