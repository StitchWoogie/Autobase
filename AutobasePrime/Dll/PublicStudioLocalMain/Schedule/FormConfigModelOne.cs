using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using PublicStudioLocalMain.Schedule;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigModelOne.
	/// </summary>
	public class FormConfigModelOne : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public FormConfigModelOne()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigModelOne));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
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
            // textBoxTitle
            // 
            this.textBoxTitle.AccessibleDescription = null;
            this.textBoxTitle.AccessibleName = null;
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackgroundImage = null;
            this.textBoxTitle.Font = null;
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AccessibleDescription = null;
            this.textBoxDescription.AccessibleName = null;
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.BackgroundImage = null;
            this.textBoxDescription.Font = null;
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
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
            this.m_list.SmallImageList = this.imageList1;
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
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
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
            // FormConfigModelOne
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
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigModelOne";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigModelOne_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public SCHEDULE_MODEL_STRUCT model = new SCHEDULE_MODEL_STRUCT();

		private void FormConfigModelOne_Load(object sender, System.EventArgs e)
		{
			TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list, "DllDialogConfigModelOne");

			SCHEDULE_MODEL_ITEM_STRUCT item;
			int l;

			this.textBoxTitle.Text = model.title;
			this.textBoxDescription.Text = model.description;

			for(l = 0; l < model.blockItem.Count; l++) 
			{
				item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[l];
				ListViewItem lvi = new ListViewItem("");
				lvi.SubItems.Add("");
				ChangeOneItem(lvi, item);
				this.m_list.Items.Add(lvi);
			}
		}

        ArrayList arraySchedule = null;
        int indexSchedule = -1;

        public void Set(ArrayList array_schedule, int index)
        {
            arraySchedule = array_schedule;
            indexSchedule = index;
        }

		private void buttonOK_Click(object sender, System.EventArgs e) 
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxTitle, TextBoxLimit.MAX_ModelName )) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxDescription, TextBoxLimit.MAX_ModelDescription)) return;

            this.textBoxTitle.Text = this.textBoxTitle.Text.Trim(); //모델이름 공백방지를 위하여 추가/ hsjeong 25-03-11

			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("제목을 입력해야 합니다.", "입력 오류");
				else if(Tools.IsLangChinese()) 
					MessageBox.Show("请输入标题。", "输入错误");
				else
					MessageBox.Show("You must input the Title", "Input Error");
				return;
			}

            SCHEDULE_MODEL_STRUCT sms;

            for (int i = 0; i < arraySchedule.Count; i++)
            {
                if (i == indexSchedule) continue;

                sms = (SCHEDULE_MODEL_STRUCT)arraySchedule[i];
                if (String.Compare(sms.title, this.textBoxTitle.Text, true) == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("같은 모델명이 이미 추가되어 있습니다.", "모델명 중복");
                    else
                        MessageBox.Show("Same title already exists.", "Title exists");

                    return;
                }
            }

			model.title = this.textBoxTitle.Text;
			model.description = this.textBoxDescription.Text;

			TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DllDialogConfigModelOne");
	
			DialogResult = DialogResult.OK;
			Close();
		}

		void ChangeOneItem(ListViewItem item, SCHEDULE_MODEL_ITEM_STRUCT model_item)
		{
			string buf;

			item.ImageIndex = model_item.hour;
            if (model_item.nTimeType == 1)
            {
                item.SubItems[0].Text = String.Format("Sunrise:{0}", model_item.nSunBeforeAfterMinutes);
            }
            else if (model_item.nTimeType == 2)
            {
                item.SubItems[0].Text = String.Format("Sunset:{0}", model_item.nSunBeforeAfterMinutes);
            }
            else
            {
                item.SubItems[0].Text = String.Format("{0:00}:{1:00}", model_item.hour, model_item.minute);
            }

			ScheduleLib.MakeViewString(model_item, out buf);
			item.SubItems[1].Text = buf;
		}

        private void buttonAdd_Click(object sender, System.EventArgs e)
        {
            FormConfigModelItem dialog = new FormConfigModelItem();

            if (Tools.IsLangKorean())
                dialog.Text = "모델 아이템 추가";
            else if (Tools.IsLangJapanese())
                dialog.Text = "モデル アイテムの追加";
            else if (Tools.IsLangChinese())
                dialog.Text = "添加模型项";
            else if (Tools.IsLangVietnamese())
                dialog.Text = "Thêm kiểu mục";
            else
                dialog.Text = "Add Model Item";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                int pos = model.blockItem.Count;
                SCHEDULE_MODEL_ITEM_STRUCT item;

                item = new SCHEDULE_MODEL_ITEM_STRUCT();

                dialog.GetStruct(item);

                item.hour = dialog.m_nHour;
                item.minute = dialog.m_nMinute;

                item.script = dialog.textBoxScript.Text;

                item.blockTag = (ArrayList)Tools.CopyObject(dialog.blockTemp);

                model.blockItem.Add(item);
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("");
                m_list.Items.Add(lvi);
                ChangeOneItem(lvi, item);
            }
        }

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			int retn = FormConfigModel.ListCtrlGetDeleteItem(m_list);
			if(retn == -1)	return;

			m_list.Items.RemoveAt(retn);
			model.blockItem.RemoveAt(retn);	
		}

		void Modify()
		{
			int retn = FormConfigModel.ListCtrlGetModifyItem(m_list);
			if(retn == -1)	return;

			FormConfigModelItem dialog = new FormConfigModelItem();
			SCHEDULE_MODEL_ITEM_STRUCT item;

			item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[retn];

			dialog.m_nHour = item.hour;
			dialog.m_nMinute = item.minute;
			dialog.textBoxScript.Text = item.script;

			dialog.blockTemp = (ArrayList)Tools.CopyObject(item.blockTag);

			if(Tools.IsLangKorean())
				dialog.Text = "모델 아이템 수정";
			else if(Tools.IsLangJapanese())
				dialog.Text = "モデル アイテムの修正";
			else if(Tools.IsLangChinese())
				dialog.Text = "修改模型项";
			else
				dialog.Text = "Modify Model Item";

            dialog.SetStruct(item);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
                dialog.GetStruct(item);
				item.hour = dialog.m_nHour;
				item.minute = dialog.m_nMinute;
				item.script = dialog.textBoxScript.Text;
				item.blockTag = (ArrayList)Tools.CopyObject(dialog.blockTemp);

				ChangeOneItem(m_list.Items[retn], item);
			}				
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

	}
}

