using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLib;
using AutoLibLocal;
using DialogHoliday;
using PublicStudioLocalMain.Schedule;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigScheduleAdditional.
	/// </summary>
	public class FormConfigScheduleAdditional : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable. 
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigScheduleAdditional()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScheduleAdditional));
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_list.Font = null;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
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
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
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
            // buttonModify
            // 
            this.buttonModify.AccessibleDescription = null;
            this.buttonModify.AccessibleName = null;
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.BackgroundImage = null;
            this.buttonModify.Font = null;
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // FormConfigScheduleAdditional
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.m_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScheduleAdditional";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigScheduleAdditional_Load);
            this.ResumeLayout(false);

		}
		#endregion

		
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ListView m_list;

		public ArrayList blockTemp;

		private void FormConfigScheduleAdditional_Load(object sender, System.EventArgs e)
		{
			TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list, "DialogConfigScheduleAdditional");

            blockTemp = ScheduleLib.ScheduleLoadAdditional();

			SCHEDULE_ADDITIONAL add;
			int l;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				add = (SCHEDULE_ADDITIONAL)blockTemp[l];
				ListViewItem item = new ListViewItem("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				m_list.Items.Add(item);
				ChangeItem(item, add);
			}

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCHEDULE_SETUP))
                this.buttonOK.Enabled = false;
		}

		void ChangeItem(ListViewItem item, SCHEDULE_ADDITIONAL add)
		{
			string buf;

			ScheduleLib.MakeStringScheduleAdditional(out buf, add);

			item.SubItems[0].Text = buf;
			item.SubItems[1].Text = add.title;
			item.SubItems[2].Text = add.model;
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			FormConfigScheduleAdditionalAdd dialog = new FormConfigScheduleAdditionalAdd();

			if(Tools.IsLangKorean()) 
				dialog.Text = "추가 스케쥴 추가";
			else if(Tools.IsLangJapanese()) 
				dialog.Text = "追加スケジュールの追加";
			else if(Tools.IsLangChinese()) 
				dialog.Text = "添加添加计划表";
			else
				dialog.Text = "Additional Schedule Add";

            dialog.Set(blockTemp, -1);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				blockTemp.Add(dialog.additional);
				ListViewItem item = new ListViewItem("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				m_list.Items.Add(item);
				ChangeItem(item, dialog.additional);
			}	
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0) 
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

			int i = m_list.SelectedItems[0].Index;

			m_list.Items.RemoveAt(i);
			blockTemp.RemoveAt(i);	
		}

		void Modify()
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("항목을 하나 선택한 후 수정할 수 있습니다.", "수정 오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else 
				{
					MessageBox.Show("Select one item to modify.", "Selection error");
				}
				return;
			}
			
			int i = m_list.SelectedItems[0].Index;

			FormConfigScheduleAdditionalAdd dialog = new FormConfigScheduleAdditionalAdd();
            
			dialog.additional = (SCHEDULE_ADDITIONAL)Tools.CopyObject(blockTemp[i]);
			if(Tools.IsLangKorean()) 
			{
				dialog.Text = "추가 스케쥴 수정";
			}
			else if(Tools.IsLangJapanese()) 
				dialog.Text = "追加スケジュールの修正";
			else if(Tools.IsLangChinese()) 
				dialog.Text = "修改添加计划表";
			else
				dialog.Text = "Additional Schedule Modify";

            dialog.Set(blockTemp, i);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				blockTemp[i] = (SCHEDULE_ADDITIONAL)Tools.CopyObject(dialog.additional);
				ChangeItem(m_list.Items[i], dialog.additional);
			}		
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DialogConfigScheduleAdditional");

            ScheduleLib.ScheduleSaveAdditional(blockTemp);
	
			DialogResult = DialogResult.OK;
			Close();
		}

	}
}

