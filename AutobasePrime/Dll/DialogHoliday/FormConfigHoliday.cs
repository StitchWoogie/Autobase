using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace DialogHoliday
{
	/// <summary>
	/// Summary description for FormConfigHoliday.
	/// </summary>
	public class FormConfigHoliday : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel; 
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigHoliday()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigHoliday));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
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
            // FormConfigHoliday
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
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigHoliday";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigHoliday_Load);
            this.ResumeLayout(false);

		}
		#endregion

		public ArrayList blockTemp;

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			HOLIDAY_LIST holiday = new HOLIDAY_LIST();

			string title;
			if(Tools.IsLangKorean())	title = "항목 추가";
			else if(Tools.IsLangJapanese())	title = "項目追加";
			else if(Tools.IsLangChinese())	title = "添加项";
            else if (Tools.IsLangVietnamese()) title = "Thêm mục";
			else						title = "Item Add";

			if(ConfigHolidayAdd(this, title, holiday, blockTemp)) 
			{
				blockTemp.Add(holiday);
				ListViewItem item = new ListViewItem("");
				item.SubItems.Add("");
				m_list.Items.Add(item);
				ChangeOneItem(item, holiday);
			}	
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0) 
			{
			
				if(Tools.IsLangKorean()) 
					MessageBox.Show("항목을 하나 선택한 후 삭제할 수 있습니다.", "선택 오류");
				else if(Tools.IsLangChinese()) 
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else
					MessageBox.Show("Select one item to delete.", "Selection error");

				return;
			}

			int i = this.m_list.SelectedItems[0].Index;
			m_list.Items.RemoveAt(i);
			blockTemp.RemoveAt(i);	
		}

		void Modify()
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("항목을 하나 선택한 후 수정할 수 있습니다.", "수정 오류");
				else if(Tools.IsLangChinese()) 
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else
					MessageBox.Show("Select one item to modify.", "Selection error");

				return;
			}

			int i = this.m_list.SelectedItems[0].Index;

			HOLIDAY_LIST holiday;

			holiday = (HOLIDAY_LIST)blockTemp[i];

			string title;
			if(Tools.IsLangKorean())	title = "항목 수정";
			else if(Tools.IsLangJapanese())	title = "項目修正";
			else if(Tools.IsLangChinese())	title = "修改项";
			else						title = "Item Modity";

            if (ConfigHolidayModify(this, title, holiday, blockTemp, i)) 
			{
				blockTemp[i] = holiday;
				ChangeOneItem(m_list.Items[i], holiday);
			}		
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		void ChangeOneItem(ListViewItem item, HOLIDAY_LIST lst)
		{
			string buf;

			Holiday.MakeString(out buf, lst);
			item.SubItems[0].Text = buf;
			item.SubItems[1].Text = lst.title;
		}

		private void FormConfigHoliday_Load(object sender, System.EventArgs e)
		{
			TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list, "DialogConfigHoliday");

			FillListBox();	
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

		void FillListBox()
		{
			int l;
			HOLIDAY_LIST holiday;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				holiday = (HOLIDAY_LIST)blockTemp[l];
				ListViewItem item = new ListViewItem("");
				item.SubItems.Add("");
				m_list.Items.Add(item);
				ChangeOneItem(item, holiday);
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DialogConfigHoliday");	

			DialogResult = DialogResult.OK;
			Close();
		}	

        //20241010 PSU Form owner 추가
		static bool ConfigHolidayAdd(Form owner, string dlg_title, HOLIDAY_LIST holiday, ArrayList array)
		{
			FormConfigHolidayAdd dialog = new FormConfigHolidayAdd();
			dialog.Text = dlg_title;

            dialog.Set(array, -1);

			if(dialog.ShowDialog(owner) == DialogResult.OK) 
			{
				holiday.title = dialog.textBoxTitle.Text;

				holiday.type = (sbyte)dialog.m_type;

				if(dialog.m_sYear == "매")
					holiday.year = 0;
				else
					holiday.year = ConvertTool.ToInt16(dialog.m_sYear);

				holiday.month = (sbyte)dialog.m_nMon;

				if(dialog.m_nDay >= 0 && dialog.m_nDay <= 30)
					holiday.day = (sbyte)(dialog.m_nDay+1);
				else
					holiday.day = 32;

				holiday.bSunOrMoon = dialog.m_solar_lunar;

				holiday.week = dialog.m_nWeek;
				holiday.weekday = dialog.m_nWeekDay;

				return true;
			}

			return false;
		}

        //20241010 PSU Form owner 추가
		public static bool ConfigHolidayModify(Form owner, string dlg_title, HOLIDAY_LIST holiday, ArrayList array, int index)
		{
			FormConfigHolidayAdd dialog = new FormConfigHolidayAdd();
			string buf;
	
			dialog.Text = dlg_title;
			dialog.textBoxTitle.Text = holiday.title;
			dialog.m_type = holiday.type;

			string sEvery;
			//string sLast;

			if(Tools.IsLangKorean()) 
			{
				sEvery = "매";
			}
			else if(Tools.IsLangJapanese()) 
			{
				sEvery = "每";
			}
			else if(Tools.IsLangChinese()) 
			{
				sEvery = "按";
			}
			else 
			{
				sEvery = "Every";
			}

			if(holiday.year == 0) 
			{
				dialog.m_sYear = sEvery;
			}
			else 
			{
				buf = String.Format("{0}", holiday.year);
				dialog.m_sYear = buf;
			}

			dialog.m_nMon = holiday.month;
			if(holiday.day == 32)
				dialog.m_nDay = 31;
			else
				dialog.m_nDay = (sbyte)(holiday.day-1);
			dialog.m_solar_lunar = holiday.bSunOrMoon;
			dialog.m_nWeek = holiday.week;
			dialog.m_nWeekDay = holiday.weekday;

            dialog.Set(array, index);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(owner) == DialogResult.OK) 
			{
				holiday.title = dialog.textBoxTitle.Text;

				holiday.type = dialog.m_type;

				if(dialog.m_sYear == sEvery)
					holiday.year = 0;
				else
					holiday.year = ConvertTool.ToInt16(dialog.m_sYear);

				holiday.month = dialog.m_nMon;

				if(dialog.m_nDay >= 0 && dialog.m_nDay <= 30)
					holiday.day = (sbyte)(dialog.m_nDay+1);
				else
					holiday.day = 32;

				holiday.bSunOrMoon = dialog.m_solar_lunar;

				holiday.week = dialog.m_nWeek;
				holiday.weekday = dialog.m_nWeekDay;

				return true;
			}

			return false;
		}
	}
}

