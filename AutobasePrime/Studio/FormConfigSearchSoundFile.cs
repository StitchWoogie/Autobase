using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using AutoLibLocal;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for FormConfigSearchSoundFile.
	/// </summary>
	public class FormConfigSearchSoundFile : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonPlay;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigSearchSoundFile()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigSearchSoundFile));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.m_list);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.m_list.Font = null;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
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
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            // 
            // buttonAdd
            // 
            this.buttonAdd.AccessibleDescription = null;
            this.buttonAdd.AccessibleName = null;
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.BackgroundImage = null;
            this.buttonAdd.Font = null;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.AccessibleDescription = null;
            this.buttonDelete.AccessibleName = null;
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.BackgroundImage = null;
            this.buttonDelete.Font = null;
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.AccessibleDescription = null;
            this.buttonPlay.AccessibleName = null;
            resources.ApplyResources(this.buttonPlay, "buttonPlay");
            this.buttonPlay.BackgroundImage = null;
            this.buttonPlay.Font = null;
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // FormConfigSearchSoundFile
            // 
            this.AcceptButton = this.buttonPlay;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigSearchSoundFile";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigSearchSoundFile_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		void ChangeItem(ListViewItem lvi, FileInfo fi)
		{
			lvi.SubItems[0].Text = fi.Name;
			lvi.SubItems[1].Text = fi.Length.ToString();
		}

		private void FormConfigSearchSoundFile_Load(object sender, System.EventArgs e)
		{
			string path = String.Format("{0}\\sound", TotalConfig.sDirWorkProject);
			
			Directory.CreateDirectory(path);

			DirectoryInfo info = new DirectoryInfo(path);

			foreach(FileInfo fi in info.GetFiles("*.wav")) 
			{
				ListViewItem lvi = new ListViewItem("");
				lvi.SubItems.Add("");
				ChangeItem(lvi, fi);
				this.m_list.Items.Add(lvi);
			}
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Wave files(*.wav)|*.wav";
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				string source, target;
				string name = Path.GetFileName(dialog.FileName);

				source = dialog.FileName;
				target = String.Format("{0}\\Sound\\{1}", TotalConfig.sDirWorkProject, name);

				try 
				{
					File.Copy(source, target, true);
				}
				catch 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("파일을 복사할 수 없습니다.", target);
					else if(Tools.IsLangChinese())
						MessageBox.Show("文件不能复制.", target);
					else
						MessageBox.Show("Can't copy the file.", target);
					return;
				}
				
				FileInfo fi = new FileInfo(target);
				ListViewItem lvi = new ListViewItem("");
				lvi.SubItems.Add("");
				ChangeItem(lvi, fi);
				this.m_list.Items.Add(lvi);
				lvi.Selected = true;
				lvi.EnsureVisible();
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("삭제할 목록을 선택하세요.", "오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请选择要删除的项。", "删除错误");
				}
				else 
				{
					MessageBox.Show("Select the item to delete.", "Error");
				}
				return;
			}

			ListViewItem lvi = m_list.SelectedItems[0];
			string filename = String.Format("{0}\\Sound\\{1}", TotalConfig.sDirWorkProject, lvi.SubItems[0].Text);

			if(Tools.IsLangKorean()) 
			{
				if(MessageBox.Show(filename, "선택한 파일을 삭제할까요?", MessageBoxButtons.YesNo)
					!= DialogResult.Yes)	return;
			}
			else if(Tools.IsLangChinese()) 
			{
				if(MessageBox.Show(filename, "选择的文件要删除吗？", MessageBoxButtons.YesNo)
					!= DialogResult.Yes)	return;
			}
			else 
			{
				if(MessageBox.Show(filename, "Delete selected files?", MessageBoxButtons.YesNo)
					!= DialogResult.Yes)	return;
			}

			File.Delete(filename);
			m_list.Items.RemoveAt(lvi.Index);
		}

		void Play()
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("재생할 목록을 선택하세요.", "오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请选择要播放的目录。", "选择错误");
				}
				else 
				{
					MessageBox.Show("Select the item to play.", "Error");
				}
				return;
			}

			ListViewItem lvi = m_list.SelectedItems[0];
			string filename = String.Format("{0}\\Sound\\{1}", TotalConfig.sDirWorkProject, lvi.SubItems[0].Text);

			bool retn = Win32Function.PlaySound(filename, IntPtr.Zero, EnumPlaySound.SND_ASYNC);	// ASYNC

            // Windows XP에서는 되고 Windows 7에서는 띵소리만 나는 경우가 있었는데 파일이 약간 이상한 것 같다. 2013

		}

		private void buttonPlay_Click(object sender, System.EventArgs e)
		{
			Play();	
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Play();
		}
	}
}
