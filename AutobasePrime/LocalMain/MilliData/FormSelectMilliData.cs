using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.IO;
using AutoLibLocal;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormSelectMilliData.
	/// </summary>
	public class FormSelectMilliData : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.ListBox m_ListGroup;
		private System.Windows.Forms.ListBox m_ListItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormSelectMilliData()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectMilliData));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.m_ListGroup = new System.Windows.Forms.ListBox();
            this.m_ListItem = new System.Windows.Forms.ListBox();
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
            // m_ListGroup
            // 
            this.m_ListGroup.AccessibleDescription = null;
            this.m_ListGroup.AccessibleName = null;
            resources.ApplyResources(this.m_ListGroup, "m_ListGroup");
            this.m_ListGroup.BackgroundImage = null;
            this.m_ListGroup.Font = null;
            this.m_ListGroup.Name = "m_ListGroup";
            this.m_ListGroup.SelectedIndexChanged += new System.EventHandler(this.m_ListGroup_SelectedIndexChanged);
            // 
            // m_ListItem
            // 
            this.m_ListItem.AccessibleDescription = null;
            this.m_ListItem.AccessibleName = null;
            resources.ApplyResources(this.m_ListItem, "m_ListItem");
            this.m_ListItem.BackgroundImage = null;
            this.m_ListItem.Font = null;
            this.m_ListItem.Name = "m_ListItem";
            this.m_ListItem.DoubleClick += new System.EventHandler(this.m_ListItem_DoubleClick);
            // 
            // FormSelectMilliData
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.m_ListItem);
            this.Controls.Add(this.m_ListGroup);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectMilliData";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormSelectMilliData_Load);
            this.ResumeLayout(false);

		}
		#endregion

		public string sDirectory;

		public static bool SelectMilliData(out string filename)
		{
			filename = "";
			FormSelectMilliData dialog = new FormSelectMilliData();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) 
			{
				filename = dialog.sDirectory;
				return true;
			}

			return false;
		}

		static int nTempPosGroup;
		static int nTempPosItem;

		private void FormSelectMilliData_Load(object sender, System.EventArgs e)
		{
			FillGroupList();

			if(nTempPosGroup < m_ListGroup.Items.Count)
				m_ListGroup.SelectedIndex = nTempPosGroup;

			FillMemberList();

			if(nTempPosItem < m_ListItem.Items.Count)
				m_ListItem.SelectedIndex = nTempPosItem;

			m_ListItem.TopIndex = nTempPosItem-5;
		}

		void FillGroupList()
		{
			string path;

			path = String.Format("{0}\\MiliData", TotalConfig.GetProjectDataDirectory());

			if(!Directory.Exists(path))	return;

			DirectoryInfo info = new DirectoryInfo(path);

			foreach(DirectoryInfo di in info.GetDirectories()) 
			{
				m_ListGroup.Items.Add(di.Name);
			}
		}

		void FillMemberList()
		{
			m_ListItem.Items.Clear();

			int retn = m_ListGroup.SelectedIndex;

			if(retn == -1)	return;

			string str;

			str = (string)m_ListGroup.SelectedItem;

			string path;
			string buf;

			path = String.Format("{0}\\MiliData\\{1}", TotalConfig.GetProjectDataDirectory(), str);

			DirectoryInfo info = new DirectoryInfo(path);

            string only_name;

			foreach(FileInfo fi in info.GetFiles("*.mdb")) 
			{
                only_name = Path.GetFileNameWithoutExtension(fi.Name);

                if (only_name.Length < 15) continue;

				buf = "";
				buf += fi.Name[0];
				buf += fi.Name[1];
				buf += fi.Name[2];
				buf += fi.Name[3];
				buf += '-';
				buf += fi.Name[4];
				buf += fi.Name[5];
				buf += '-';
				buf += fi.Name[6];
				buf += fi.Name[7];
				buf += ' ';
				buf += fi.Name[9];
				buf += fi.Name[10];
				buf += ':';
				buf += fi.Name[11];
				buf += fi.Name[12];
				buf += ':';
				buf += fi.Name[13];
				buf += fi.Name[14];

                if(only_name.Length > 15)
                    buf += " "+only_name.Substring(15);

				m_ListItem.Items.Add(buf);
			}
		}

		private void m_ListGroup_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FillMemberList();		
		}

		void OK()
		{
			int retn1 = m_ListGroup.SelectedIndex;
			int retn2 = m_ListItem.SelectedIndex;

			if(retn1 == -1 || retn2 == -1) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("보고 싶은 날짜를 선택하세요.", "선택 오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请选择要查看的日期。", "选择错误");
				}
				else 
				{
					MessageBox.Show("Select date to show.", "Selection error");
				}
				return;
			}

			string str1, str2;
			string buf;

			str1 = (string)m_ListGroup.SelectedItem;
			str2 = (string)m_ListItem.SelectedItem;

			nTempPosGroup = m_ListGroup.SelectedIndex;
			nTempPosItem = retn2;

			buf = "";
			buf += str2[0];
			buf += str2[1];
			buf += str2[2];
			buf += str2[3];
			buf += str2[5];
			buf += str2[6];
			buf += str2[8];
			buf += str2[9];
			buf += '_';
			buf += str2[11];
			buf += str2[12];
			buf += str2[14];
			buf += str2[15];
			buf += str2[17];
			buf += str2[18];

            if (str2.Length >= 21)
            {
                buf += str2.Substring(20);
            }

			sDirectory = String.Format("{0}\\MiliData\\{1}\\{2}.mdb", TotalConfig.GetProjectDataDirectory(), str1, buf);	

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			OK();
		}

		private void m_ListItem_DoubleClick(object sender, System.EventArgs e)
		{
			OK();
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			int retn1 = m_ListGroup.SelectedIndex;
			int retn2 = m_ListItem.SelectedIndex;

			if(retn1 == -1 || retn2 == -1) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("삭제하고 싶은 날짜를 선택하세요.", "삭제 오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的日期。", "选择错误");
				else 
				{
					MessageBox.Show("Select date to delete.", "Delete error");
				}
				return;
			}

			string str1, str2;

			str1 = (string)m_ListGroup.SelectedItem;
			str2 = (string)m_ListItem.SelectedItem;

			string filename;

			ChangeListItemToFileName(out filename, str1, str2);

			string msg;
			if(Tools.IsLangKorean())		msg = "이 파일을 삭제할까요?";
            else if (Tools.IsLangJapanese()) msg = "このファイルを削除しますか。";
			else if(Tools.IsLangChinese())	msg = "要删除此文件吗？";
			else							msg = "Delete this file?"; 

			if(MessageBox.Show(filename, msg, MessageBoxButtons.YesNo) == DialogResult.Yes) 
			{
				try {
					File.Delete(filename); 
					m_ListItem.Items.RemoveAt(retn2);	
					m_ListItem.SelectedIndex = retn2;
				}
				catch 
				{
					if(Tools.IsLangKorean()) 
					{
						MessageBox.Show("파일을 삭제할 수 없습니다.\n파일이 사용중이거나 읽기전용입니다.", filename);
					}
					else 
					{
						MessageBox.Show("Cannot delete file.\nMaybe file is readonly or used.", filename);
					}
				}
			}	
		}

		void ChangeListItemToFileName(out string target, string str1, string str2)
		{
			string buf;

			buf = "";
			buf += str2[0];
			buf += str2[1];
			buf += str2[2];
			buf += str2[3];
			buf += str2[5];
			buf += str2[6];
			buf += str2[8];
			buf += str2[9];
			buf += '_';
			buf += str2[11];
			buf += str2[12];
			buf += str2[14];
			buf += str2[15];
			buf += str2[17];
			buf += str2[18];
			
			target = String.Format("{0}\\MiliData\\{1}\\{2}.mdb", TotalConfig.GetProjectDataDirectory(), str1, buf);
		}
	}
}

