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
	/// Summary description for FormConfigModel.
	/// </summary>
	public class FormConfigModel : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonCopy;
		private System.Windows.Forms.Button buttonPaste;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete; 
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public FormConfigModel()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigModel));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
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
            // buttonCopy
            // 
            this.buttonCopy.AccessibleDescription = null;
            this.buttonCopy.AccessibleName = null;
            resources.ApplyResources(this.buttonCopy, "buttonCopy");
            this.buttonCopy.BackgroundImage = null;
            this.buttonCopy.Font = null;
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.AccessibleDescription = null;
            this.buttonPaste.AccessibleName = null;
            resources.ApplyResources(this.buttonPaste, "buttonPaste");
            this.buttonPaste.BackgroundImage = null;
            this.buttonPaste.Font = null;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
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
            // FormConfigModel
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonPaste);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigModel";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigModel_Load);
            this.ResumeLayout(false);

		}
		#endregion

		public int bConfigOrSelect = 0;
		ArrayList blockTemp = new ArrayList();
		public string m_SelectTitle;
		public bool bChangeFlag;
		
        //20241010 PSU Form owner 추가
		public static bool ScheduleModelSelect(Form owner, out string title)
		{
			title = "";
			FormConfigModel dialog = new FormConfigModel();
			bool retn = false;

			dialog.bConfigOrSelect = 1;

			if(dialog.ShowDialog(owner) == DialogResult.OK) 
			{
				title = dialog.m_SelectTitle;
				retn = true;
			}

			return retn;
		}

		SCHEDULE_MODEL_STRUCT modelClipboard = null;
		bool bCopyFlag;

		private void FormConfigModel_Load(object sender, System.EventArgs e)
		{
			bChangeFlag = false;

			bCopyFlag = false;

            blockTemp = ScheduleLib.ModelLoad();

			if(bConfigOrSelect == 0) 
			{
				if(Tools.IsLangKorean())
					this.Text = "일일 운전모델 설정";
				else if(Tools.IsLangJapanese())
					this.Text = "日モデルの設定";
				else if(Tools.IsLangChinese())
                    this.Text = "设置一日的操作模型";
                else if (Tools.IsLangVietnamese())
                    this.Text = "Cấu hình kiểu ngày";
				else
					this.Text = "Day model config";
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					this.Text = "일일 운전모델 선택";
					this.buttonOK.Text = "선택";
				}
				else if(Tools.IsLangJapanese())
				 {
					 this.Text = "日モデルの選択";
					 this.buttonOK.Text = "選択";
				 }
				else if(Tools.IsLangChinese())
				{
                    this.Text = "选择一日的操作模型";
					this.buttonOK.Text = "选择";
				}
                else if (Tools.IsLangVietnamese())
                {
                    this.Text = "Chọn kiểu ngày";
                    this.buttonOK.Text = "Chọn";
                }
				else 
				{
					this.Text = "Select day model";
					this.buttonOK.Text = "Select";
				}
			}

			TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list, "DllDialogConfigModel");

			//m_list.SetImageList(&image, LVSIL_SMALL);

			SCHEDULE_MODEL_STRUCT model;
			int l;
			ListViewItem item;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				model = (SCHEDULE_MODEL_STRUCT)blockTemp[l];
				item = new ListViewItem(model.title, 25);
				item.SubItems.Add(model.description);
				m_list.Items.Add(item);
			}

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCHEDULE_SETUP))
                this.buttonOK.Enabled = false;
		}

			

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			FormConfigModelOne dialog = new FormConfigModelOne();

			if(Tools.IsLangKorean())
				dialog.Text = "운전 모델 추가";
			else if(Tools.IsLangJapanese())
				dialog.Text = "モデル追加";
			else if(Tools.IsLangChinese())
				dialog.Text = "添加模型";
            else if (Tools.IsLangVietnamese())
                dialog.Text = "Thêm kiểu";
			else
				dialog.Text = "Add Model";

            dialog.Set(blockTemp, -1);
	
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				int pos = blockTemp.Count;

				SCHEDULE_MODEL_STRUCT model;
				model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(dialog.model);
				blockTemp.Add(model);
				ListViewItem item = m_list.Items.Insert(pos, model.title, 25);
				item.SubItems.Add(model.description);
				bChangeFlag = true;

				//CheckSameName();
			}
		}

		static int ListCtrlGetSelect(ListView list)
		{
			if(list.SelectedItems.Count == 0)	return -1;

			return list.SelectedItems[0].Index;
		}

		public static int ListCtrlGetDeleteItem(ListView list)
		{
			int retn = ListCtrlGetSelect(list);

			if(retn == -1) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("항목을 하나 선택한 후 삭제할 수 있습니다.", "선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else
					MessageBox.Show("Select one item to delete.", "Selection error");
				return -1;
			}

			return retn;
		}

		public static int ListCtrlGetModifyItem(ListView list)
		{
			int retn = ListCtrlGetSelect(list);

			if(retn == -1) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("항목을 하나 선택한 후 수정할 수 있습니다.", "선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else
					MessageBox.Show("Select one item to modify.", "Selection error");

				return -1;
			}

			return retn;
		}

		public static int ListCtrlGetSelectItem(ListView list)
		{
			int retn = ListCtrlGetSelect(list);

			if(retn == -1) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("항목을 하나 선택하세요.", "선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选1个项。", "选择错误");
				else
					MessageBox.Show("Select one item.", "Selection error");

				return -1;
			}

			return retn;
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			int retn = ListCtrlGetDeleteItem(m_list);
			if(retn == -1)	return;

			m_list.Items.RemoveAt(retn);
			blockTemp.RemoveAt(retn);
			bChangeFlag = true;	
		}

		void Modify()
		{
			int retn = ListCtrlGetModifyItem(m_list);
			if(retn == -1)	return;

			FormConfigModelOne dialog = new FormConfigModelOne();
			SCHEDULE_MODEL_STRUCT model;

			model = (SCHEDULE_MODEL_STRUCT)blockTemp[retn];
			dialog.model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(model);
			if(Tools.IsLangKorean())
				dialog.Text = "운전 모델 수정";
			else if(Tools.IsLangJapanese())
				dialog.Text = "モデル修正";
			else
				dialog.Text = "Modify Model";

            dialog.Set(blockTemp, retn);

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(dialog.model);
				blockTemp[retn] = model;
				ListViewItem item = m_list.Items[retn];
				item.SubItems[0].Text = model.title;
				item.SubItems[1].Text = model.description;
				bChangeFlag = true;
				//CheckSameName();
			}			
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			if(bConfigOrSelect == 0)
				Modify();
			else
				OK();	
		}

        /*
		bool CheckSameName() 
		{
			SCHEDULE_MODEL_STRUCT model1, model2;
			int l, m;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				model1 = (SCHEDULE_MODEL_STRUCT)blockTemp[l];
				for(m = l+1; m < blockTemp.Count; m++) 
				{
					model2 = (SCHEDULE_MODEL_STRUCT)blockTemp[m];
					if(model1.title == model2.title) 
					{
						MessageBox.Show("Same title exist.\nChange the title.", model1.title);
						return true;
					}
				}
			}

			return false;
		}*/

		void OK()
		{
			//if(CheckSameName())	return;

			if(bConfigOrSelect == 1) 
			{
				int retn = ListCtrlGetSelectItem(m_list);
				if(retn == -1)	return;

				SCHEDULE_MODEL_STRUCT model;

				model = (SCHEDULE_MODEL_STRUCT)blockTemp[retn];

				m_SelectTitle = model.title;
			}

			TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DllDialogConfigModel");

            ScheduleLib.ModelSave(blockTemp);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			OK();
		}

		private void buttonCopy_Click(object sender, System.EventArgs e)
		{
			int retn = ListCtrlGetSelect(m_list);	
			if(retn == -1)	return;

			SCHEDULE_MODEL_STRUCT model;

			model = (SCHEDULE_MODEL_STRUCT)blockTemp[retn];

			bCopyFlag = true;
			modelClipboard = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(model);

			this.buttonPaste.Enabled = true;
		}

		private void buttonPaste_Click(object sender, System.EventArgs e)
		{
			if(bCopyFlag == false)	return;

			SCHEDULE_MODEL_STRUCT model;
			model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(modelClipboard);

			if(Tools.IsLangKorean()) 
				model.title += "-복사본";
			else if(Tools.IsLangJapanese()) 
				model.title += "-コピー";
			else
				model.title += "-Copy";

            //복사 붙여넣기시 이름 중복이 되므로 중복방지용 추가
            int count = 0;
            for (int i = 0; i < blockTemp.Count; i++)
            {
                SCHEDULE_MODEL_STRUCT sc = new SCHEDULE_MODEL_STRUCT();
                sc = (SCHEDULE_MODEL_STRUCT)blockTemp[i];
                if (model.title == sc.title)
                {
                    count++;
                    model.title += string.Format("{0}", count);
                    i = -1;
                }

            }
            // hsjeong 25-03-12

			int pos = blockTemp.Count;

			blockTemp.Add(model);
			ListViewItem item = m_list.Items.Insert(pos, model.title, 25);
			item.SubItems.Add(model.description);

			bChangeFlag = true;	
		}
	}
}

