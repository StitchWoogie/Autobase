using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using PublicStudioLocalMain.Schedule;
using NetTools.OldDefine;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigModelItem.
	/// </summary>
	public class FormConfigModelItem : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.GroupBox groupBoxSpecifiedTime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ComboBox m_comboHour;
		private System.Windows.Forms.ComboBox m_comboMinute;
		public System.Windows.Forms.TextBox textBoxScript;
		private System.Windows.Forms.Label label3;
        private ColumnHeader columnHeader3;
        private GroupBox groupBox2;
        private RadioButton radioButtonTimeType2;
        private RadioButton radioButtonTimeType1;
        private RadioButton radioButtonTimeType0;
        private Label label4;
        private Label label5;
        private NumericUpDown numericUpDownSunAfterBefore;
        private GroupBox groupBoxSunControl;
        private Label label6;
        private Button buttonLocation;
        private TextBox textBoxLocation;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigModelItem()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigModelItem));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.groupBoxSpecifiedTime = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_comboMinute = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_comboHour = new System.Windows.Forms.ComboBox();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.textBoxScript = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonTimeType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeType0 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownSunAfterBefore = new System.Windows.Forms.NumericUpDown();
            this.groupBoxSunControl = new System.Windows.Forms.GroupBox();
            this.buttonLocation = new System.Windows.Forms.Button();
            this.textBoxLocation = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxSpecifiedTime.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSunAfterBefore)).BeginInit();
            this.groupBoxSunControl.SuspendLayout();
            this.SuspendLayout();
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
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonModify
            // 
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // groupBoxSpecifiedTime
            // 
            this.groupBoxSpecifiedTime.Controls.Add(this.label2);
            this.groupBoxSpecifiedTime.Controls.Add(this.m_comboMinute);
            this.groupBoxSpecifiedTime.Controls.Add(this.label1);
            this.groupBoxSpecifiedTime.Controls.Add(this.m_comboHour);
            resources.ApplyResources(this.groupBoxSpecifiedTime, "groupBoxSpecifiedTime");
            this.groupBoxSpecifiedTime.Name = "groupBoxSpecifiedTime";
            this.groupBoxSpecifiedTime.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // m_comboMinute
            // 
            this.m_comboMinute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboMinute, "m_comboMinute");
            this.m_comboMinute.Name = "m_comboMinute";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_comboHour
            // 
            this.m_comboHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboHour, "m_comboHour");
            this.m_comboHour.Name = "m_comboHour";
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            resources.ApplyResources(this.m_list, "m_list");
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
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // textBoxScript
            // 
            this.textBoxScript.AcceptsReturn = true;
            this.textBoxScript.AcceptsTab = true;
            resources.ApplyResources(this.textBoxScript, "textBoxScript");
            this.textBoxScript.Name = "textBoxScript";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonTimeType2);
            this.groupBox2.Controls.Add(this.radioButtonTimeType1);
            this.groupBox2.Controls.Add(this.radioButtonTimeType0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonTimeType2
            // 
            resources.ApplyResources(this.radioButtonTimeType2, "radioButtonTimeType2");
            this.radioButtonTimeType2.Name = "radioButtonTimeType2";
            this.radioButtonTimeType2.TabStop = true;
            this.radioButtonTimeType2.UseVisualStyleBackColor = true;
            this.radioButtonTimeType2.CheckedChanged += new System.EventHandler(this.radioButtonTimeType2_CheckedChanged);
            // 
            // radioButtonTimeType1
            // 
            resources.ApplyResources(this.radioButtonTimeType1, "radioButtonTimeType1");
            this.radioButtonTimeType1.Name = "radioButtonTimeType1";
            this.radioButtonTimeType1.TabStop = true;
            this.radioButtonTimeType1.UseVisualStyleBackColor = true;
            this.radioButtonTimeType1.CheckedChanged += new System.EventHandler(this.radioButtonTimeType1_CheckedChanged);
            // 
            // radioButtonTimeType0
            // 
            resources.ApplyResources(this.radioButtonTimeType0, "radioButtonTimeType0");
            this.radioButtonTimeType0.Checked = true;
            this.radioButtonTimeType0.Name = "radioButtonTimeType0";
            this.radioButtonTimeType0.TabStop = true;
            this.radioButtonTimeType0.UseVisualStyleBackColor = true;
            this.radioButtonTimeType0.CheckedChanged += new System.EventHandler(this.radioButtonTimeType0_CheckedChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownSunAfterBefore
            // 
            resources.ApplyResources(this.numericUpDownSunAfterBefore, "numericUpDownSunAfterBefore");
            this.numericUpDownSunAfterBefore.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numericUpDownSunAfterBefore.Minimum = new decimal(new int[] {
            1440,
            0,
            0,
            -2147483648});
            this.numericUpDownSunAfterBefore.Name = "numericUpDownSunAfterBefore";
            this.numericUpDownSunAfterBefore.ValueChanged += new System.EventHandler(this.numericUpDownSunAfterBefore_ValueChanged);
            // 
            // groupBoxSunControl
            // 
            this.groupBoxSunControl.Controls.Add(this.buttonLocation);
            this.groupBoxSunControl.Controls.Add(this.textBoxLocation);
            this.groupBoxSunControl.Controls.Add(this.label6);
            this.groupBoxSunControl.Controls.Add(this.label5);
            this.groupBoxSunControl.Controls.Add(this.numericUpDownSunAfterBefore);
            this.groupBoxSunControl.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBoxSunControl, "groupBoxSunControl");
            this.groupBoxSunControl.Name = "groupBoxSunControl";
            this.groupBoxSunControl.TabStop = false;
            // 
            // buttonLocation
            // 
            resources.ApplyResources(this.buttonLocation, "buttonLocation");
            this.buttonLocation.Name = "buttonLocation";
            this.buttonLocation.Click += new System.EventHandler(this.buttonLocation_Click);
            // 
            // textBoxLocation
            // 
            resources.ApplyResources(this.textBoxLocation, "textBoxLocation");
            this.textBoxLocation.Name = "textBoxLocation";
            this.textBoxLocation.ReadOnly = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // FormConfigModelItem
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxSunControl);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxScript);
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.groupBoxSpecifiedTime);
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigModelItem";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigModelItem_Load);
            this.groupBoxSpecifiedTime.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSunAfterBefore)).EndInit();
            this.groupBoxSunControl.ResumeLayout(false);
            this.groupBoxSunControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public ArrayList blockTemp = new ArrayList();

		private void FormConfigModelItem_Load(object sender, System.EventArgs e)
		{
			string buf;
			int i;

			for(i = 0; i < 24; i++) 
			{
				buf = String.Format("{0:00}", i);
				m_comboHour.Items.Add(buf);
			}

			for(i = 0; i < 60; i++) 
			{
				buf = String.Format("{0:00}", i);
				m_comboMinute.Items.Add(buf);
			}

			TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list, "DllDialogConfigModelItem");

			SCHEDULE_TAG_VALUE_STRUCT tagValue;
			int l;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				tagValue = (SCHEDULE_TAG_VALUE_STRUCT)blockTemp[l];
				ListViewItem item = new ListViewItem("");
				item.SubItems.Add("");
                item.SubItems.Add("");
				m_list.Items.Add(item);
				ChangeOneItem(item, tagValue.tag, tagValue.val);
			}
	
			this.m_comboHour.SelectedIndex = this.m_nHour;
			this.m_comboMinute.SelectedIndex = this.m_nMinute;

            EnableTimeType();

            RecalcSunContolTime();

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                groupBox2.Visible = false;
                groupBoxSunControl.Visible = false;
                groupBoxSpecifiedTime.Left = groupBox2.Left;

                m_list.Top -= 88;
                buttonAdd.Top -= 88;
                buttonModify.Top -= 88;
                buttonDelete.Top -= 88;

                this.label3.Visible = false;
                this.textBoxScript.Visible = false;
                this.Height -= (144+88);

                this.radioButtonTimeType0.Checked = true;
            }
		}

        //SCHEDULE_MODEL_ITEM_STRUCT tempItem;

        public void SetStruct(SCHEDULE_MODEL_ITEM_STRUCT item)
        {
            this.radioButtonTimeType0.Checked = (item.nTimeType == 0);
            this.radioButtonTimeType1.Checked = (item.nTimeType == 1);
            this.radioButtonTimeType2.Checked = (item.nTimeType == 2);

            this.numericUpDownSunAfterBefore.Value = item.nSunBeforeAfterMinutes;
            this.textBoxLocation.Text = item.sLocation;
        }

        public void GetStruct(SCHEDULE_MODEL_ITEM_STRUCT item)
        {
            item.nTimeType = GetRadioTimeType();
            item.nSunBeforeAfterMinutes = ConvertTool.ToInt32(this.numericUpDownSunAfterBefore.Value);
            item.sLocation = this.textBoxLocation.Text;
        }

        int GetRadioTimeType()
        {
            int type;

            if (this.radioButtonTimeType0.Checked) type = 0;
            else if (this.radioButtonTimeType1.Checked) type = 1;
            else if (this.radioButtonTimeType2.Checked) type = 2;
            else type = 0;

            return type;
        }

        void EnableTimeType()
        {
            int type = GetRadioTimeType();
            bool flag_sun_control = false;
            bool flag_specifid_control = false;

            if (type == 1 || type == 2)
            {
                flag_sun_control = true;
            }
            else
            {
                flag_specifid_control = true;
            }

            this.groupBoxSpecifiedTime.Enabled = flag_specifid_control;
            this.groupBoxSunControl.Enabled = flag_sun_control;
        }

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			SCHEDULE_TAG_VALUE_STRUCT tagValue;
			int i;
			ListViewItem item;

			blockTemp.Clear();

			for(i = 0; i < m_list.Items.Count; i++) 
			{
				tagValue = new SCHEDULE_TAG_VALUE_STRUCT();
				item = m_list.Items[i];

				tagValue.tag = item.SubItems[0].Text;
				tagValue.val = item.SubItems[2].Text;
				blockTemp.Add(tagValue);
			}

			this.m_nHour = ConvertTool.ToInt32(this.m_comboHour.Text);
			this.m_nMinute = ConvertTool.ToInt32(this.m_comboMinute.Text);

			TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DllDialogConfigModelItem");

			DialogResult = DialogResult.OK;
			Close();
		}

		public int m_nHour;
		public int m_nMinute;

        //private void buttonAdd_Click(object sender, System.EventArgs e)
        //{
        //    string tag;
        //    string des;

        //    if(SelectTag.SelectAll(out tag, out des) != DialogResult.OK)	return;

        //    int i;
        //    ListViewItem item;

        //    for(i = 0; i < m_list.Items.Count; i++) 
        //    {
        //        item = m_list.Items[i];
        //        if(tag == item.SubItems[0].Text) 
        //        {
        //            item.Selected = true;
        //            return;
        //        }
        //    }

        //    item = new ListViewItem(tag);
        //    item.SubItems.Add(des);
        //    item.SubItems.Add("0");
        //    m_list.Items.Add(item);
        //    item.Selected = true;
        //}

        //일일운전모델 태그다중선택 추가 20241010
        static bool bTagMultiSelection = true;

        void AddOneTag(string tag, string des)
        {
            ListViewItem item;
            for (int i = 0; i < m_list.Items.Count; i++)
            {
                item = m_list.Items[i];
                if (String.Compare(item.Text, tag, true) == 0)
                {
                    item.Selected = true;
                    return;	// already registerd
                }
            }

            item = new ListViewItem(tag);
            item.SubItems.Add(des);
            item.SubItems.Add("0");
            m_list.Items.Add(item);
            item.Selected = true;
        }


        private void buttonAdd_Click(object sender, System.EventArgs e)
        {
            FormSelectTag dialog = new FormSelectTag();
            dialog.bUseTagAI = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagST = true;
            dialog.bUseTagAO = true;
            dialog.bUseTagDO = true;
            dialog.bUseTagGDO = true;
            dialog.bUseMultiSelection = true;
            dialog.checkBoxMultiSelect.Checked = bTagMultiSelection;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.checkBoxMultiSelect.Checked == false || dialog.m_list.SelectedItems.Count <= 1)
                {
                    AddOneTag(dialog.sTag, dialog.sDes);
                }
                else
                {
                    ListViewItem lvi;
                    string tag;
                    int[] tag_pos = new int[1];
                    TagPublicClass tp;

                    for (int i = 0; i < dialog.m_list.SelectedItems.Count; i++)
                    {
                        lvi = dialog.m_list.SelectedItems[i];

                        tag = dialog.GetFullTagName(lvi.SubItems[0].Text);

                        tp = TagLib.GetStructPublic(tag, ref tag_pos);

                        AddOneTag(tag, tp.description);
                    }
                }

                bTagMultiSelection = dialog.checkBoxMultiSelect.Checked;
            }
        }

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			int retn = FormConfigModel.ListCtrlGetDeleteItem(m_list);
			if(retn == -1)	return;

			m_list.Items.RemoveAt(retn);
		}

		void ChangeOneItem(ListViewItem item, string tag, string val)
		{
            TagPublicClass tp;
            int[] tag_pos = new int[1];

            tp = TagLib.GetStructPublic(tag, ref tag_pos);

			item.SubItems[0].Text = tag;
			item.SubItems[1].Text = tp.description;
            item.SubItems[2].Text = val;
		}

		void Modify()
		{
			int retn = FormConfigModel.ListCtrlGetModifyItem(m_list);
			if(retn == -1)	return;

			FormModelChangeValue dialog = new FormModelChangeValue();

			ListViewItem item = m_list.Items[retn];

            dialog.textBoxTag.Text = item.SubItems[0].Text;
            dialog.textBoxDescription.Text = item.SubItems[1].Text;
            dialog.textBoxValue.Text = item.SubItems[2].Text;
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				item.SubItems[0].Text = dialog.textBoxTag.Text;
				item.SubItems[2].Text = dialog.textBoxValue.Text;
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

        // 일출/일몰 일때는 제어 시간을 계산해서 디스프레이 해준다.
        void RecalcSunContolTime()
        {
            int time_type = GetRadioTimeType();

            if (time_type == 0) return;

            double longitude, latitude;

            LocationLib.CalcLocationInfomationByTitle(LocationLib.arrayLocation, this.textBoxLocation.Text, out latitude, out longitude);
            DateTime t = DateTime.Now;
            SYSTEMTIME tRise = new SYSTEMTIME(), tSet = new SYSTEMTIME();
            SunRiseSet.GetTime(t.Year, t.Month, t.Day, longitude, latitude, 0, tRise, tSet);

            if (time_type == 1)
            {
                t = new DateTime(t.Year, t.Month, t.Day, tRise.wHour, tRise.wMinute, 0);
            }
            else if (time_type == 2)
            {
                t = new DateTime(t.Year, t.Month, t.Day, tSet.wHour, tSet.wMinute, 0);
            }

            t = t.AddMinutes(ConvertTool.ToInt32(this.numericUpDownSunAfterBefore.Value));

            t = t.ToLocalTime();
            this.m_comboHour.Text = t.Hour.ToString("00");
            this.m_comboMinute.Text = t.Minute.ToString("00");
        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            FormConfigLocation dialog = new FormConfigLocation();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxLocation.Text = dialog.sSelectedCity;

                RecalcSunContolTime();
            }
        }

        private void radioButtonTimeType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeType();
            RecalcSunContolTime();
        }

        private void radioButtonTimeType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeType();
            RecalcSunContolTime();
        }

        private void radioButtonTimeType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeType();
            RecalcSunContolTime();
        }

        private void numericUpDownSunAfterBefore_ValueChanged(object sender, EventArgs e)
        {
            RecalcSunContolTime();
        }
	}
}

