using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using DialogHoliday;
using AutoLibLocal;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigSchedule.
	/// </summary>
	public class FormConfigSchedule : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigSchedule()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigSchedule));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_list = new System.Windows.Forms.ListBox();
            this.buttonScheduleModify = new System.Windows.Forms.Button();
            this.buttonScheduleDelete = new System.Windows.Forms.Button();
            this.buttonScheduleAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.m_listModel = new System.Windows.Forms.ListBox();
            this.buttonModelDelete = new System.Windows.Forms.Button();
            this.buttonModelAdd = new System.Windows.Forms.Button();
            this.buttonSpecial = new System.Windows.Forms.Button();
            this.buttonHodiday = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.m_list);
            this.groupBox1.Controls.Add(this.buttonScheduleModify);
            this.groupBox1.Controls.Add(this.buttonScheduleDelete);
            this.groupBox1.Controls.Add(this.buttonScheduleAdd);
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
            this.m_list.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_list.Font = null;
            this.m_list.Name = "m_list";
            this.m_list.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.m_list_DrawItem);
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // buttonScheduleModify
            // 
            this.buttonScheduleModify.AccessibleDescription = null;
            this.buttonScheduleModify.AccessibleName = null;
            resources.ApplyResources(this.buttonScheduleModify, "buttonScheduleModify");
            this.buttonScheduleModify.BackgroundImage = null;
            this.buttonScheduleModify.Font = null;
            this.buttonScheduleModify.Name = "buttonScheduleModify";
            this.buttonScheduleModify.Click += new System.EventHandler(this.buttonScheduleModify_Click);
            // 
            // buttonScheduleDelete
            // 
            this.buttonScheduleDelete.AccessibleDescription = null;
            this.buttonScheduleDelete.AccessibleName = null;
            resources.ApplyResources(this.buttonScheduleDelete, "buttonScheduleDelete");
            this.buttonScheduleDelete.BackgroundImage = null;
            this.buttonScheduleDelete.Font = null;
            this.buttonScheduleDelete.Name = "buttonScheduleDelete";
            this.buttonScheduleDelete.Click += new System.EventHandler(this.buttonScheduleDelete_Click);
            // 
            // buttonScheduleAdd
            // 
            this.buttonScheduleAdd.AccessibleDescription = null;
            this.buttonScheduleAdd.AccessibleName = null;
            resources.ApplyResources(this.buttonScheduleAdd, "buttonScheduleAdd");
            this.buttonScheduleAdd.BackgroundImage = null;
            this.buttonScheduleAdd.Font = null;
            this.buttonScheduleAdd.Name = "buttonScheduleAdd";
            this.buttonScheduleAdd.Click += new System.EventHandler(this.buttonScheduleAdd_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.m_listModel);
            this.groupBox2.Controls.Add(this.buttonModelDelete);
            this.groupBox2.Controls.Add(this.buttonModelAdd);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // m_listModel
            // 
            this.m_listModel.AccessibleDescription = null;
            this.m_listModel.AccessibleName = null;
            resources.ApplyResources(this.m_listModel, "m_listModel");
            this.m_listModel.BackgroundImage = null;
            this.m_listModel.Font = null;
            this.m_listModel.Name = "m_listModel";
            // 
            // buttonModelDelete
            // 
            this.buttonModelDelete.AccessibleDescription = null;
            this.buttonModelDelete.AccessibleName = null;
            resources.ApplyResources(this.buttonModelDelete, "buttonModelDelete");
            this.buttonModelDelete.BackgroundImage = null;
            this.buttonModelDelete.Font = null;
            this.buttonModelDelete.Name = "buttonModelDelete";
            this.buttonModelDelete.Click += new System.EventHandler(this.buttonModelDelete_Click);
            // 
            // buttonModelAdd
            // 
            this.buttonModelAdd.AccessibleDescription = null;
            this.buttonModelAdd.AccessibleName = null;
            resources.ApplyResources(this.buttonModelAdd, "buttonModelAdd");
            this.buttonModelAdd.BackgroundImage = null;
            this.buttonModelAdd.Font = null;
            this.buttonModelAdd.Name = "buttonModelAdd";
            this.buttonModelAdd.Click += new System.EventHandler(this.buttonModelAdd_Click);
            // 
            // buttonSpecial
            // 
            this.buttonSpecial.AccessibleDescription = null;
            this.buttonSpecial.AccessibleName = null;
            resources.ApplyResources(this.buttonSpecial, "buttonSpecial");
            this.buttonSpecial.BackgroundImage = null;
            this.buttonSpecial.Font = null;
            this.buttonSpecial.Name = "buttonSpecial";
            this.buttonSpecial.Click += new System.EventHandler(this.buttonSpecial_Click);
            // 
            // buttonHodiday
            // 
            this.buttonHodiday.AccessibleDescription = null;
            this.buttonHodiday.AccessibleName = null;
            resources.ApplyResources(this.buttonHodiday, "buttonHodiday");
            this.buttonHodiday.BackgroundImage = null;
            this.buttonHodiday.Font = null;
            this.buttonHodiday.Name = "buttonHodiday";
            this.buttonHodiday.Click += new System.EventHandler(this.buttonHodiday_Click);
            // 
            // FormConfigSchedule
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonSpecial);
            this.Controls.Add(this.buttonHodiday);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigSchedule";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigSchedule_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		ArrayList blockTemp;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonScheduleAdd;
		private System.Windows.Forms.Button buttonScheduleDelete;
		private System.Windows.Forms.Button buttonScheduleModify;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonModelDelete;
		private System.Windows.Forms.Button buttonModelAdd;
		private System.Windows.Forms.Button buttonSpecial;
		private System.Windows.Forms.Button buttonHodiday;
		private System.Windows.Forms.ListBox m_list;
		private System.Windows.Forms.ListBox m_listModel;
		int nOldListPos = 0;
		bool bInitial = false;

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ListToBlock();

            ScheduleLib.ScheduleSaveFixed(blockTemp);
            
			DialogResult = DialogResult.OK;
			Close();
		}

		void FillListBoxModel()
		{
			int retn = m_list.SelectedIndex;

			if(retn == -1)	return;

			m_listModel.Items.Clear();

			SCHEDULE_STRUCT sc;
			NAME_STRUCT name;
			int l;

			sc = (SCHEDULE_STRUCT)blockTemp[retn];
			for(l = 0; l < sc.blockName.Count; l++) 
			{
				name = (NAME_STRUCT)sc.blockName[l];
				m_listModel.Items.Add(name.title);
			}
		}

		void FillListBox()
		{
			m_list.Items.Clear();

			int l;
			string imsi;
			SCHEDULE_STRUCT sc;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				sc = (SCHEDULE_STRUCT)blockTemp[l];
				imsi = String.Format("{0}", sc.title);
				m_list.Items.Add(imsi);
			}
		}

		private void FormConfigSchedule_Load(object sender, System.EventArgs e)
		{
			blockTemp = ScheduleLib.ScheduleLoadFixed();

			FillListBox();	
			if(m_list.Items.Count > 0)
				m_list.SelectedIndex = 0;
			FillListBoxModel();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCHEDULE_SETUP))
                this.buttonOK.Enabled = false;

			this.bInitial = true;
		}

		private void buttonScheduleAdd_Click(object sender, System.EventArgs e)
		{
			FormConfigScheduleAdd dialog = new FormConfigScheduleAdd();

			if(Tools.IsLangKorean())
				dialog.Text = "고정 스케쥴 모델 추가";
			else if(Tools.IsLangJapanese())
				dialog.Text = "固定スケジュールのモデル追加";
			else if(Tools.IsLangChinese())
				dialog.Text = "添加固定计划表模型";
            else if (Tools.IsLangVietnamese())
                dialog.Text = "Thêm kiểu lịch cố định";
			else
				dialog.Text = "Add Fixed Model Schedule";

            dialog.Set(blockTemp, -1);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				SCHEDULE_STRUCT sc;
				int l;
		
				for(l = 0; l < blockTemp.Count; l++) 
				{
					sc = (SCHEDULE_STRUCT)blockTemp[l];	
					if(String.Compare(sc.title, dialog.textBoxTitle.Text,true) == 0) 
					{
						m_list.SelectedIndex = l;
						return;
					}
				}

				sc = new SCHEDULE_STRUCT();
				sc.title = dialog.textBoxTitle.Text;
				sc.color = dialog.buttonColor.BackColor;
				sc.blockName = new ArrayList();
				blockTemp.Add(sc);
				FillListBox();
				m_list.SelectedIndex = blockTemp.Count-1;
			}	
		}

		void ListToBlock() 
		{
			int retn = m_list.SelectedIndex;

			if(retn == -1)	return;

			if(nOldListPos >= (int)blockTemp.Count)	return;

			SCHEDULE_STRUCT sc;
			NAME_STRUCT name;
			int l;

			sc = (SCHEDULE_STRUCT)blockTemp[nOldListPos];
			sc.blockName.Clear();

			for(l = 0; l < (int)m_listModel.Items.Count; l++) 
			{
				name = new NAME_STRUCT();
				name.title = (string)m_listModel.Items[l];
				sc.blockName.Add(name);
			}	
		}

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(!this.bInitial)	return;	// Form Loading 중이다.

			ListToBlock();
			m_list.Invalidate();
			FillListBoxModel();
			nOldListPos = m_list.SelectedIndex;	
		}

		private void buttonScheduleDelete_Click(object sender, System.EventArgs e)
		{
			int retn = m_list.SelectedIndex;
			SCHEDULE_STRUCT sc;

			if(retn == -1) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("삭제하고 싶은 항목을 선택한 후\n삭제할 수 있습니다.", "삭제 오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else 
				{
					MessageBox.Show("Select item to delete.", "Delete error");
				}
				return;
			}

			sc = (SCHEDULE_STRUCT)blockTemp[retn];

			if(Tools.IsLangKorean()) 
			{
				if(MessageBox.Show("항목을 삭제할까요?", sc.title, MessageBoxButtons.YesNo) != DialogResult.Yes)	return;
			}
			else 
			{
				if(MessageBox.Show("Delete this item?", sc.title, MessageBoxButtons.YesNo) != DialogResult.Yes)	return;
			}

			blockTemp.RemoveAt(retn);
			FillListBox();

			if(retn < m_list.Items.Count)
				m_list.SelectedIndex = retn;
		}

		private void m_list_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			if(e.Index == -1)	return;
			
			SCHEDULE_STRUCT sc;
			Color color;
			Color tcolor;
			Color bcolor;

			if(m_list.SelectedIndex == e.Index)	
			{
				color = Color.FromArgb(0, 0, 0x80);

				tcolor = Color.White;
				bcolor = color;
			}
			else 
			{
				color = Color.White;
				tcolor = Color.Black;
				bcolor = color;
			}

			sc = (SCHEDULE_STRUCT)blockTemp[e.Index];
			DrawClass.gcls(e.Graphics, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right-1, e.Bounds.Bottom-1, color);
			DrawClass.PushBox2(e.Graphics, e.Bounds.Left+1, e.Bounds.Top+1, e.Bounds.Left+10, e.Bounds.Bottom-2, sc.color);

            SafeException.SafeDrawString(e.Graphics, sc.title, this.m_list.Font, new SolidBrush(tcolor), e.Bounds.Left + 20, e.Bounds.Top);
		}

		void Modify()
		{
			int retn = m_list.SelectedIndex;
			SCHEDULE_STRUCT sc;

			if(retn == -1) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("수정하고 싶은 항목을 선택한 후\n수정할 수 있습니다.", "수정 오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
                else if (Tools.IsLangVietnamese())
                    MessageBox.Show("Lựa chọn mục sửa đổi.", "Lỗi lựa chọn");
				else 
				{
					MessageBox.Show("Select item to modify.", "Selection error");
				}
				return;
			}

			sc = (SCHEDULE_STRUCT)blockTemp[retn];

			FormConfigScheduleAdd dialog = new FormConfigScheduleAdd();

			if(Tools.IsLangKorean())
				dialog.Text = "고정 스케쥴 모델 수정";
			else if(Tools.IsLangJapanese())
				dialog.Text = "固定スケジュールのモデル修正";
			else if(Tools.IsLangChinese())
				dialog.Text = "修改固定计划表模型";
			else
				dialog.Text = "Modify Fixed Model Schedule";

			dialog.textBoxTitle.Text = sc.title;
			dialog.buttonColor.BackColor = sc.color;

            dialog.Set(blockTemp, retn);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				sc.title = dialog.textBoxTitle.Text;
				sc.color = dialog.buttonColor.BackColor;
				m_list.Invalidate();
			}	
		}

		private void buttonScheduleModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void buttonModelAdd_Click(object sender, System.EventArgs e)
		{
			string model;
			string seek;
			int i;

			if(FormConfigModel.ScheduleModelSelect(this, out model)) 
			{
				for(i = 0; i < m_listModel.Items.Count; i++) 
				{
					seek = (string)m_listModel.Items[i];
					if(seek == model) 
					{
						if(Tools.IsLangKorean()) 
						{
							MessageBox.Show("같은 모델명이 이미 추가되어 있습니다.", model);
						}
						else if(Tools.IsLangKorean()) 
						{
							MessageBox.Show("同样的模型名已添加了。", model);
						}
						else 
						{
							MessageBox.Show("Same model already registerd.", model);
						}
						m_listModel.SelectedIndex = i;
						return;
					}
				}

				m_listModel.Items.Add(model);
				m_listModel.SelectedItem = model;
			}
		}

		private void buttonModelDelete_Click(object sender, System.EventArgs e)
		{
			int retn = m_listModel.SelectedIndex;

			if(retn == -1)	return;

			m_listModel.Items.RemoveAt(retn);	
		}

		private void buttonHodiday_Click(object sender, System.EventArgs e)
		{
			if(Holiday.ConfigHoliday(this, ref Holiday.arrayHoliday)) 
			{
				
			}
		}

		private void buttonSpecial_Click(object sender, System.EventArgs e)
		{
			if(Holiday.ConfigSpecialDay(this, ref Holiday.arraySpecialDay)) 
			{
				
			}
		}

	}
}

