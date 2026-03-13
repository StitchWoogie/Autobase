using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DatabaseConnection;
using DatabaseSaveList;
using NetTools;

namespace DialogAddition
{
	/// <summary>
	/// Summary description for FormAdditionModify.
	/// </summary>
	public class FormAdditionModify : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.ComboBox comboBoxSaveList;
		public System.Windows.Forms.ComboBox comboBoxTag;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		DatabaseSaveListClass classSaveList;

		public FormAdditionModify(DatabaseSaveListClass class_save_list)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			classSaveList = class_save_list;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdditionModify));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxSaveList = new System.Windows.Forms.ComboBox();
            this.comboBoxTag = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comboBoxSaveList
            // 
            this.comboBoxSaveList.AccessibleDescription = null;
            this.comboBoxSaveList.AccessibleName = null;
            resources.ApplyResources(this.comboBoxSaveList, "comboBoxSaveList");
            this.comboBoxSaveList.BackgroundImage = null;
            this.comboBoxSaveList.Font = null;
            this.comboBoxSaveList.Name = "comboBoxSaveList";
            this.comboBoxSaveList.SelectedIndexChanged += new System.EventHandler(this.comboBoxSaveList_SelectedIndexChanged);
            // 
            // comboBoxTag
            // 
            this.comboBoxTag.AccessibleDescription = null;
            this.comboBoxTag.AccessibleName = null;
            resources.ApplyResources(this.comboBoxTag, "comboBoxTag");
            this.comboBoxTag.BackgroundImage = null;
            this.comboBoxTag.Font = null;
            this.comboBoxTag.Name = "comboBoxTag";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // FormAdditionModify
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.comboBoxTag);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxSaveList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAdditionModify";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormAdditionModify_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormAdditionModify_Load(object sender, System.EventArgs e)
		{
			SaveList list;
			for(int i = 0; i < classSaveList.arrayDatabaseSaveList.Count; i++) {
				list = (SaveList)classSaveList.arrayDatabaseSaveList[i];
				comboBoxSaveList.Items.Add(list.title);
			}

			FillComboBoxTag();
		}

		void FillComboBoxTag()
		{
			string text = comboBoxSaveList.Text;

			comboBoxTag.Items.Clear();

			SaveList list;
			DatabaseMember member;
			for(int i = 0; i < classSaveList.arrayDatabaseSaveList.Count; i++) 
			{
				list = (SaveList)classSaveList.arrayDatabaseSaveList[i];
				if(text == list.title) 
				{
					for(int j = 0; j < list.arrayMember.Count; j++) 
					{
						member = (DatabaseMember)list.arrayMember[j];
						comboBoxTag.Items.Add(member.field);
					}
				}
			}
		}

		private void comboBoxSaveList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FillComboBoxTag();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(comboBoxTag.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("태그 필드를 입력해야 합니다.");
				else
					MessageBox.Show("You must input Tag field.");

				return;
			}
			if(this.comboBoxSaveList.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("저장목록을 입력해야 합니다.");
				else
					MessageBox.Show("You must input Save List.");

				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
