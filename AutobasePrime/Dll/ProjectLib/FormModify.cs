using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.IO;
using AutoLibLocal;

namespace ProjectSelect
{
	/// <summary>
	/// Summary description for FormModify.
	/// </summary>
	public class FormModify : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TextBox textBoxDirectory;
		private System.Windows.Forms.Button buttonDirectory; 
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public int nSelectedIndex;
		public ListView listParent;
        private GroupBox groupBox3;
        private RadioButton radioButtonTarget1;
        private RadioButton radioButtonTarget0;
        Lib libProject = null;
        public PROJECT_STRUCT EditingProject;

        public FormModify(Lib lib)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            libProject = lib;
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

        public void Set(PROJECT_STRUCT project)
        {
            this.radioButtonTarget0.Checked = (project.nPlatform == 0);
            this.radioButtonTarget1.Checked = (project.nPlatform == 1);
        }

        public void Get(PROJECT_STRUCT project)
        {
            if (this.radioButtonTarget0.Checked) project.nPlatform = 0;
            else if (this.radioButtonTarget1.Checked) project.nPlatform = 1;
            else project.nPlatform = 0;
        }

        

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModify));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonDirectory = new System.Windows.Forms.Button();
            this.textBoxDirectory = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonTarget1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTarget0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxTitle);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.AccessibleDescription = null;
            this.textBoxTitle.AccessibleName = null;
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackgroundImage = null;
            this.textBoxTitle.Font = null;
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.TextChanged += new System.EventHandler(this.textBoxTitle_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.buttonDirectory);
            this.groupBox2.Controls.Add(this.textBoxDirectory);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonDirectory
            // 
            this.buttonDirectory.AccessibleDescription = null;
            this.buttonDirectory.AccessibleName = null;
            resources.ApplyResources(this.buttonDirectory, "buttonDirectory");
            this.buttonDirectory.BackgroundImage = null;
            this.buttonDirectory.Font = null;
            this.buttonDirectory.Name = "buttonDirectory";
            this.buttonDirectory.Click += new System.EventHandler(this.buttonDirectory_Click);
            // 
            // textBoxDirectory
            // 
            this.textBoxDirectory.AccessibleDescription = null;
            this.textBoxDirectory.AccessibleName = null;
            resources.ApplyResources(this.textBoxDirectory, "textBoxDirectory");
            this.textBoxDirectory.BackgroundImage = null;
            this.textBoxDirectory.Font = null;
            this.textBoxDirectory.Name = "textBoxDirectory";
            this.textBoxDirectory.TextChanged += new System.EventHandler(this.textBoxDirectory_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonTarget1);
            this.groupBox3.Controls.Add(this.radioButtonTarget0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonTarget1
            // 
            this.radioButtonTarget1.AccessibleDescription = null;
            this.radioButtonTarget1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonTarget1, "radioButtonTarget1");
            this.radioButtonTarget1.BackgroundImage = null;
            this.radioButtonTarget1.Font = null;
            this.radioButtonTarget1.Name = "radioButtonTarget1";
            this.radioButtonTarget1.TabStop = true;
            this.radioButtonTarget1.UseVisualStyleBackColor = true;
            // 
            // radioButtonTarget0
            // 
            this.radioButtonTarget0.AccessibleDescription = null;
            this.radioButtonTarget0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonTarget0, "radioButtonTarget0");
            this.radioButtonTarget0.BackgroundImage = null;
            this.radioButtonTarget0.Checked = true;
            this.radioButtonTarget0.Font = null;
            this.radioButtonTarget0.Name = "radioButtonTarget0";
            this.radioButtonTarget0.TabStop = true;
            this.radioButtonTarget0.UseVisualStyleBackColor = true;
            // 
            // FormModify
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModify";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormModify_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        /*
		public static bool Run(Lib lib, string title, PROJECT_STRUCT project, ListView list, int index)
		{
			FormModify dialog = new FormModify(lib);

			dialog.Text = title;
			dialog.textBoxTitle.Text = project.title;
			dialog.textBoxDirectory.Text = project.directory;
			dialog.listParent = list;
			dialog.nSelectedIndex = index;

			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				project.title = dialog.textBoxTitle.Text;
				project.directory = dialog.textBoxDirectory.Text;

				return true;
			}

			return false;
		}*/

		bool CheckFilenameValid(string filename, out char code)
		{
			code = ' ';
			int size = filename.Length;
			int i;

			char[] invalid = Path.GetInvalidPathChars();

			for(i = 0; i < size; i++) 
			{
				code = filename[i];

				for(int j = 0; j < invalid.Length; j++) 
				{
					if(code == invalid[j])	return false;
				}

				if(code == '/')		return false;
				if(code == '*')		return false;
				if(code == '?')		return false;
				if(code == '"')		return false;
				if(code == '<')		return false;
				if(code == '>')		return false;
				if(code == '|')		return false;
			}

			return true;
		}

		bool CheckUsedTitleDir(string title, string dir)
		{
			ListViewItem lvi;

			for(int i = 0; i < listParent.Items.Count; i++) 
			{
				lvi = listParent.Items[i];

                PROJECT_STRUCT p = lvi.Tag as PROJECT_STRUCT;
                if (p != null && Object.ReferenceEquals(p, this.EditingProject))
                    continue;   // 자신의 항목은 비교할 필요가 없다.

                if (String.Compare(lvi.SubItems[0].Text, title, true) == 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						MessageBox.Show("같은 제목의 프로젝트가 이미 존재합니다.", "제목중복");
					}
					else if(Tools.IsLangChinese()) 
					{
						MessageBox.Show("同名的项目已经存在。", "标题已存在");
					}
					else 
					{
						MessageBox.Show("Same title is already used.", "Title error");
					}
					return false;
				}
				else if(String.Compare(lvi.SubItems[1].Text, dir, true) == 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						MessageBox.Show("지정한 폴더는 이미 다른 프로젝트에서 사용중입니다.", "폴더");
					}
					else if(Tools.IsLangChinese()) 
					{
						MessageBox.Show("您指定的文件夹在别的项目中正在使用。", "文件夹");
					}
					else 
					{
						MessageBox.Show("Same directory is already used by another project.", "Directory error");
					}
					return false;
				}
			}

			return true;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxTitle, TextBoxLimit.MAX_ProjectTitle)) return;
            
            this.textBoxTitle.Text = this.textBoxTitle.Text.Trim();
			this.textBoxDirectory.Text = this.textBoxDirectory.Text.Trim();

			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("작업의 제목이 필요합니다.", "제목없음");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入标题。", "输入错误");
				}
				else 
				{
					MessageBox.Show("Need project title.", "No title !!!");
				}
				this.textBoxTitle.Select();
				return;
			}

			if(this.textBoxDirectory.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("작업할 폴더명을 입력해야 합니다.", "폴더 불분명");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入文件夹名。", "输入错误");
				}
				else 
				{
					MessageBox.Show("Need project directory.", "No Directory!!!");
				}
				this.textBoxDirectory.Select();
				return;
			}

			char code;
			if(!CheckFilenameValid(this.textBoxDirectory.Text, out code)) 
			{
				string msg;
				if(Tools.IsLangKorean()) 
				{
					msg = String.Format("{0}[{1:X02}] 는 폴더명에 적당하지 않습니다.\n입력예) D:\\TEST", code, (int)code);
					MessageBox.Show(msg, "폴더명 오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					msg = String.Format("{0}[{1:X02}] 不适合于文件夹名。\n榜样) D:\\TEST", code, (int)code);
					MessageBox.Show(msg, "文件夹错误");
				}
				else 
				{
					msg = String.Format("{0}[{1:X02}] is invalid character.\nExample) D:\\TEST", code, (int)code);
					MessageBox.Show(msg, "Directory name error");
				}
				return;
			}

			// GetFullPath를 하면 사용자가 입력한 Path를 적당한 path로 만들어준다.
			try 
			{
				this.textBoxDirectory.Text = Path.GetFullPath(this.textBoxDirectory.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(this.textBoxDirectory.Text+"\nMessage="+exception.Message, "Error");
				return;
			}

			char[] trim = {'\\'};
			this.textBoxDirectory.Text = this.textBoxDirectory.Text.TrimEnd(trim);

			if(!CheckUsedTitleDir(this.textBoxTitle.Text, this.textBoxDirectory.Text))	return;

            if (!libProject.CheckExistProjectDirectory(this.textBoxDirectory.Text)) 
			{
				this.textBoxDirectory.Select();
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonDirectory_Click(object sender, System.EventArgs e)
		{
			string title;
						
			if(Tools.IsLangKorean()) 
			{
				title = "작업 폴더 선택";
			}
			else if(Tools.IsLangJapanese()) 
			{
				title = "プロジェクト フォルダ選択";
			}
			else if(Tools.IsLangChinese()) 
			{
				title = "选择项目文件夹";
			}
            else if (Tools.IsLangVietnamese())
            {
                title = "Chọn cặp dự án";
            }
			else 
			{
				title = "Select project folder";
			}

			FormDirectorySelect dialog = new FormDirectorySelect();

			dialog.FileName = this.textBoxDirectory.Text;
			dialog.Text = title;
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxDirectory.Text = dialog.FileName;
			}

            bAutoMakeFolderName = false;    // 폴더를 사용자가 선택하고 나면 자동 폴더 이름 만들기는 하지 않는다.
		}

        string sFirstFolder;
        public bool bModifyDialog = false;
        bool bAutoMakeFolderName = true;    // 자동으로 폴더명을 만든다. 2011-12-13 추가

        private void FormModify_Load(object sender, EventArgs e)
        {
            
            sFirstFolder = this.textBoxDirectory.Text;

            if (!bModifyDialog)
            {
                // 추가일때만 제목으로 포커스 이동
                this.textBoxTitle.Focus();
                this.textBoxTitle.Select();
            }

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.groupBox3.Visible = false;
                this.textBoxDirectory.ReadOnly = true;
            }
            else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                this.groupBox3.Visible = false;
            }
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            if (bModifyDialog) return;
            if (!bAutoMakeFolderName) return;

            string title = this.textBoxTitle.Text.Trim();

            if(title.Length > 0)
                this.textBoxDirectory.Text = sFirstFolder + "\\" + this.textBoxTitle.Text.Trim();
            else
                this.textBoxDirectory.Text = sFirstFolder;
        }

        private void textBoxDirectory_TextChanged(object sender, EventArgs e)
        {

        }
	}
}
