using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using DialogTag;

namespace Studio
{
	/// <summary>
	/// Summary description for FormConfigMilliDataAdd.
	/// </summary>
	public class FormConfigMilliDataAdd : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.Button buttonOK; 
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownDataUnit;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonAddMember;
		private System.Windows.Forms.Button buttonDeleteMember;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonCondition0;
		private System.Windows.Forms.RadioButton radioButtonCondition1;
        private System.Windows.Forms.RadioButton radioButtonCondition2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonSizeCut0;
		private System.Windows.Forms.RadioButton radioButtonSizeCut1;
		private System.Windows.Forms.RadioButton radioButtonSizeCut2;
		private System.Windows.Forms.RadioButton radioButtonSizeCut3;
		private System.Windows.Forms.RadioButton radioButtonSizeCut4;
		private System.Windows.Forms.RadioButton radioButtonSizeCut5;
		private System.Windows.Forms.RadioButton radioButtonSizeCut6;
		private System.Windows.Forms.TextBox textBoxSizeCut;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxCheckDI;
		private System.Windows.Forms.Button buttonCheckDI;
		private System.Windows.Forms.ColumnHeader columnHeader1;
        private GroupBox groupBox4;
        private CheckBox checkBoxUseAutoDelete;
        private Label label6;
        private NumericUpDown numericUpDownDaysOfAutoDelete;
        private GroupBox groupBox5;
        private RadioButton radioButtonSaveType2;
        private RadioButton radioButtonSaveType1;
        private RadioButton radioButtonSaveType0;
        private CheckBox checkBoxMatchToDateTime;
        private GroupBox groupBoxCsvTargetFolder;
        private CheckBox checkBoxSpecifyTargetFolder;
        private Button buttonCsvTargetFolder;
        private TextBox textBoxCsvTargetFolder;
        private CheckBox checkBoxUseCsvDateFolder;
        private GroupBox groupBox6;
        private CheckBox checkBoxUseFilenameAddition;
        private Button buttonUseFilenameAdditionTag;
        private TextBox textBoxUseFilenameAdditionTag;
        private Button buttonDown;
        private Button buttonUp;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigMilliDataAdd()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigMilliDataAdd));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownDataUnit = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.buttonDeleteMember = new System.Windows.Forms.Button();
            this.buttonAddMember = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonCondition2 = new System.Windows.Forms.RadioButton();
            this.radioButtonCondition1 = new System.Windows.Forms.RadioButton();
            this.radioButtonCondition0 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxSizeCut = new System.Windows.Forms.TextBox();
            this.radioButtonSizeCut6 = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeCut5 = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeCut4 = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeCut3 = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeCut2 = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeCut1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeCut0 = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxCheckDI = new System.Windows.Forms.TextBox();
            this.buttonCheckDI = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownDaysOfAutoDelete = new System.Windows.Forms.NumericUpDown();
            this.checkBoxUseAutoDelete = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButtonSaveType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonSaveType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSaveType0 = new System.Windows.Forms.RadioButton();
            this.checkBoxMatchToDateTime = new System.Windows.Forms.CheckBox();
            this.groupBoxCsvTargetFolder = new System.Windows.Forms.GroupBox();
            this.checkBoxSpecifyTargetFolder = new System.Windows.Forms.CheckBox();
            this.buttonCsvTargetFolder = new System.Windows.Forms.Button();
            this.textBoxCsvTargetFolder = new System.Windows.Forms.TextBox();
            this.checkBoxUseCsvDateFolder = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseFilenameAddition = new System.Windows.Forms.CheckBox();
            this.buttonUseFilenameAdditionTag = new System.Windows.Forms.Button();
            this.textBoxUseFilenameAdditionTag = new System.Windows.Forms.TextBox();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataUnit)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDaysOfAutoDelete)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBoxCsvTargetFolder.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxTitle
            // 
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.Name = "textBoxTitle";
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
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownDataUnit
            // 
            resources.ApplyResources(this.numericUpDownDataUnit, "numericUpDownDataUnit");
            this.numericUpDownDataUnit.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownDataUnit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDataUnit.Name = "numericUpDownDataUnit";
            this.numericUpDownDataUnit.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonDown);
            this.groupBox1.Controls.Add(this.buttonUp);
            this.groupBox1.Controls.Add(this.m_list);
            this.groupBox1.Controls.Add(this.buttonDeleteMember);
            this.groupBox1.Controls.Add(this.buttonAddMember);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // buttonDeleteMember
            // 
            resources.ApplyResources(this.buttonDeleteMember, "buttonDeleteMember");
            this.buttonDeleteMember.Name = "buttonDeleteMember";
            this.buttonDeleteMember.Click += new System.EventHandler(this.buttonDeleteMember_Click);
            // 
            // buttonAddMember
            // 
            resources.ApplyResources(this.buttonAddMember, "buttonAddMember");
            this.buttonAddMember.Name = "buttonAddMember";
            this.buttonAddMember.Click += new System.EventHandler(this.buttonAddMember_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonCondition2);
            this.groupBox2.Controls.Add(this.radioButtonCondition1);
            this.groupBox2.Controls.Add(this.radioButtonCondition0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonCondition2
            // 
            resources.ApplyResources(this.radioButtonCondition2, "radioButtonCondition2");
            this.radioButtonCondition2.Name = "radioButtonCondition2";
            this.radioButtonCondition2.CheckedChanged += new System.EventHandler(this.radioButtonCondition2_CheckedChanged);
            // 
            // radioButtonCondition1
            // 
            resources.ApplyResources(this.radioButtonCondition1, "radioButtonCondition1");
            this.radioButtonCondition1.Name = "radioButtonCondition1";
            this.radioButtonCondition1.CheckedChanged += new System.EventHandler(this.radioButtonCondition1_CheckedChanged);
            // 
            // radioButtonCondition0
            // 
            resources.ApplyResources(this.radioButtonCondition0, "radioButtonCondition0");
            this.radioButtonCondition0.Name = "radioButtonCondition0";
            this.radioButtonCondition0.CheckedChanged += new System.EventHandler(this.radioButtonCondition0_CheckedChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxSizeCut);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut6);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut5);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut4);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut3);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut2);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut1);
            this.groupBox3.Controls.Add(this.radioButtonSizeCut0);
            this.groupBox3.Controls.Add(this.label5);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // textBoxSizeCut
            // 
            resources.ApplyResources(this.textBoxSizeCut, "textBoxSizeCut");
            this.textBoxSizeCut.Name = "textBoxSizeCut";
            // 
            // radioButtonSizeCut6
            // 
            resources.ApplyResources(this.radioButtonSizeCut6, "radioButtonSizeCut6");
            this.radioButtonSizeCut6.Name = "radioButtonSizeCut6";
            // 
            // radioButtonSizeCut5
            // 
            resources.ApplyResources(this.radioButtonSizeCut5, "radioButtonSizeCut5");
            this.radioButtonSizeCut5.Name = "radioButtonSizeCut5";
            this.radioButtonSizeCut5.CheckedChanged += new System.EventHandler(this.radioButtonSizeCut5_CheckedChanged);
            // 
            // radioButtonSizeCut4
            // 
            resources.ApplyResources(this.radioButtonSizeCut4, "radioButtonSizeCut4");
            this.radioButtonSizeCut4.Name = "radioButtonSizeCut4";
            this.radioButtonSizeCut4.CheckedChanged += new System.EventHandler(this.radioButtonSizeCut4_CheckedChanged);
            // 
            // radioButtonSizeCut3
            // 
            resources.ApplyResources(this.radioButtonSizeCut3, "radioButtonSizeCut3");
            this.radioButtonSizeCut3.Name = "radioButtonSizeCut3";
            // 
            // radioButtonSizeCut2
            // 
            resources.ApplyResources(this.radioButtonSizeCut2, "radioButtonSizeCut2");
            this.radioButtonSizeCut2.Name = "radioButtonSizeCut2";
            // 
            // radioButtonSizeCut1
            // 
            resources.ApplyResources(this.radioButtonSizeCut1, "radioButtonSizeCut1");
            this.radioButtonSizeCut1.Name = "radioButtonSizeCut1";
            // 
            // radioButtonSizeCut0
            // 
            resources.ApplyResources(this.radioButtonSizeCut0, "radioButtonSizeCut0");
            this.radioButtonSizeCut0.Name = "radioButtonSizeCut0";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxCheckDI
            // 
            resources.ApplyResources(this.textBoxCheckDI, "textBoxCheckDI");
            this.textBoxCheckDI.Name = "textBoxCheckDI";
            // 
            // buttonCheckDI
            // 
            resources.ApplyResources(this.buttonCheckDI, "buttonCheckDI");
            this.buttonCheckDI.Name = "buttonCheckDI";
            this.buttonCheckDI.Click += new System.EventHandler(this.buttonCheckDI_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.numericUpDownDaysOfAutoDelete);
            this.groupBox4.Controls.Add(this.checkBoxUseAutoDelete);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // numericUpDownDaysOfAutoDelete
            // 
            resources.ApplyResources(this.numericUpDownDaysOfAutoDelete, "numericUpDownDaysOfAutoDelete");
            this.numericUpDownDaysOfAutoDelete.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownDaysOfAutoDelete.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDaysOfAutoDelete.Name = "numericUpDownDaysOfAutoDelete";
            this.numericUpDownDaysOfAutoDelete.Value = new decimal(new int[] {
            365,
            0,
            0,
            0});
            // 
            // checkBoxUseAutoDelete
            // 
            resources.ApplyResources(this.checkBoxUseAutoDelete, "checkBoxUseAutoDelete");
            this.checkBoxUseAutoDelete.Name = "checkBoxUseAutoDelete";
            this.checkBoxUseAutoDelete.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButtonSaveType2);
            this.groupBox5.Controls.Add(this.radioButtonSaveType1);
            this.groupBox5.Controls.Add(this.radioButtonSaveType0);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // radioButtonSaveType2
            // 
            resources.ApplyResources(this.radioButtonSaveType2, "radioButtonSaveType2");
            this.radioButtonSaveType2.Name = "radioButtonSaveType2";
            // 
            // radioButtonSaveType1
            // 
            resources.ApplyResources(this.radioButtonSaveType1, "radioButtonSaveType1");
            this.radioButtonSaveType1.Name = "radioButtonSaveType1";
            // 
            // radioButtonSaveType0
            // 
            resources.ApplyResources(this.radioButtonSaveType0, "radioButtonSaveType0");
            this.radioButtonSaveType0.Name = "radioButtonSaveType0";
            // 
            // checkBoxMatchToDateTime
            // 
            resources.ApplyResources(this.checkBoxMatchToDateTime, "checkBoxMatchToDateTime");
            this.checkBoxMatchToDateTime.Name = "checkBoxMatchToDateTime";
            this.checkBoxMatchToDateTime.UseVisualStyleBackColor = true;
            // 
            // groupBoxCsvTargetFolder
            // 
            this.groupBoxCsvTargetFolder.Controls.Add(this.checkBoxSpecifyTargetFolder);
            this.groupBoxCsvTargetFolder.Controls.Add(this.buttonCsvTargetFolder);
            this.groupBoxCsvTargetFolder.Controls.Add(this.textBoxCsvTargetFolder);
            resources.ApplyResources(this.groupBoxCsvTargetFolder, "groupBoxCsvTargetFolder");
            this.groupBoxCsvTargetFolder.Name = "groupBoxCsvTargetFolder";
            this.groupBoxCsvTargetFolder.TabStop = false;
            // 
            // checkBoxSpecifyTargetFolder
            // 
            resources.ApplyResources(this.checkBoxSpecifyTargetFolder, "checkBoxSpecifyTargetFolder");
            this.checkBoxSpecifyTargetFolder.Name = "checkBoxSpecifyTargetFolder";
            this.checkBoxSpecifyTargetFolder.UseVisualStyleBackColor = true;
            this.checkBoxSpecifyTargetFolder.CheckedChanged += new System.EventHandler(this.checkBoxSpecifyTargetFolder_CheckedChanged);
            // 
            // buttonCsvTargetFolder
            // 
            resources.ApplyResources(this.buttonCsvTargetFolder, "buttonCsvTargetFolder");
            this.buttonCsvTargetFolder.Name = "buttonCsvTargetFolder";
            this.buttonCsvTargetFolder.UseVisualStyleBackColor = true;
            this.buttonCsvTargetFolder.Click += new System.EventHandler(this.buttonCsvTargetFolder_Click);
            // 
            // textBoxCsvTargetFolder
            // 
            resources.ApplyResources(this.textBoxCsvTargetFolder, "textBoxCsvTargetFolder");
            this.textBoxCsvTargetFolder.Name = "textBoxCsvTargetFolder";
            // 
            // checkBoxUseCsvDateFolder
            // 
            resources.ApplyResources(this.checkBoxUseCsvDateFolder, "checkBoxUseCsvDateFolder");
            this.checkBoxUseCsvDateFolder.Name = "checkBoxUseCsvDateFolder";
            this.checkBoxUseCsvDateFolder.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBoxUseFilenameAddition);
            this.groupBox6.Controls.Add(this.buttonUseFilenameAdditionTag);
            this.groupBox6.Controls.Add(this.textBoxUseFilenameAdditionTag);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // checkBoxUseFilenameAddition
            // 
            resources.ApplyResources(this.checkBoxUseFilenameAddition, "checkBoxUseFilenameAddition");
            this.checkBoxUseFilenameAddition.Name = "checkBoxUseFilenameAddition";
            this.checkBoxUseFilenameAddition.UseVisualStyleBackColor = true;
            this.checkBoxUseFilenameAddition.CheckedChanged += new System.EventHandler(this.checkBoxUseFilenameAddition_CheckedChanged);
            // 
            // buttonUseFilenameAdditionTag
            // 
            resources.ApplyResources(this.buttonUseFilenameAdditionTag, "buttonUseFilenameAdditionTag");
            this.buttonUseFilenameAdditionTag.Name = "buttonUseFilenameAdditionTag";
            this.buttonUseFilenameAdditionTag.UseVisualStyleBackColor = true;
            this.buttonUseFilenameAdditionTag.Click += new System.EventHandler(this.buttonUseFilenameAdditionTag_Click);
            // 
            // textBoxUseFilenameAdditionTag
            // 
            resources.ApplyResources(this.textBoxUseFilenameAdditionTag, "textBoxUseFilenameAdditionTag");
            this.textBoxUseFilenameAdditionTag.Name = "textBoxUseFilenameAdditionTag";
            // 
            // buttonUp
            // 
            resources.ApplyResources(this.buttonUp, "buttonUp");
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            resources.ApplyResources(this.buttonDown, "buttonDown");
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // FormConfigMilliDataAdd
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.checkBoxUseCsvDateFolder);
            this.Controls.Add(this.groupBoxCsvTargetFolder);
            this.Controls.Add(this.checkBoxMatchToDateTime);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.buttonCheckDI);
            this.Controls.Add(this.textBoxCheckDI);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownDataUnit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigMilliDataAdd";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigMilliDataAdd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataUnit)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDaysOfAutoDelete)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBoxCsvTargetFolder.ResumeLayout(false);
            this.groupBoxCsvTargetFolder.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		ArrayList blockTag = new ArrayList();

        //private void buttonAddMember_Click(object sender, System.EventArgs e)
        //{
        //    string tag;
        //    string des;
        //    int  i;

        //    if(SelectTag.SelectAiDiSt(out tag, out des) == DialogResult.OK) 
        //    {
        //        ListViewItem lvi;
        //        for(i = 0; i < m_list.Items.Count; i++) 
        //        {
        //            lvi = m_list.Items[i];
        //            if(lvi.SubItems[0].Text == tag) 
        //            {
        //                lvi.Selected = true;
        //                return;
        //            }
        //        }

        //        lvi = new ListViewItem(tag);
        //        m_list.Items.Add(lvi);
        //        lvi.Selected = true;
        //    }
        //}
        
        //미세자료 태그 다중선택 기능 추가 20241010 PSU
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
            m_list.Items.Add(item);
            item.Selected = true;
        }

        private void buttonAddMember_Click(object sender, System.EventArgs e)
        {
            FormSelectTag dialog = new FormSelectTag();
            dialog.bUseTagAI = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagST = true;
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

		int m_nCondition = 1;
		int m_nCutMethod = 5;
        int m_nSaveType = 0;

		private void FormConfigMilliDataAdd_Load(object sender, System.EventArgs e)
		{
			this.radioButtonCondition0.Checked = (m_nCondition == 0);
			this.radioButtonCondition1.Checked = (m_nCondition == 1);
			this.radioButtonCondition2.Checked = (m_nCondition == 2);

			this.radioButtonSizeCut0.Checked = (m_nCutMethod == 0);
			this.radioButtonSizeCut1.Checked = (m_nCutMethod == 1);
			this.radioButtonSizeCut2.Checked = (m_nCutMethod == 2);
			this.radioButtonSizeCut3.Checked = (m_nCutMethod == 3);
			this.radioButtonSizeCut4.Checked = (m_nCutMethod == 4);
			this.radioButtonSizeCut5.Checked = (m_nCutMethod == 5);
			this.radioButtonSizeCut6.Checked = (m_nCutMethod == 6);

            this.radioButtonSaveType0.Checked = (m_nSaveType == 0);
            this.radioButtonSaveType1.Checked = (m_nSaveType == 1);
            this.radioButtonSaveType2.Checked = (m_nSaveType == 2);

			EnableDisable();
            EnableUseFilenameAddition();
		}

		void FillListBox()
		{
			m_list.Items.Clear();

			MILLI_DATA_TAG tag;
			int l;
			ListViewItem lvi;

			for(l = 0; l < blockTag.Count; l++) 
			{
				tag = (MILLI_DATA_TAG)blockTag[l];
				lvi = new ListViewItem(tag.tag);
				m_list.Items.Add(lvi);
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(Tools.IsLangKorean())   
				{
					MessageBox.Show("제목을 입력해야 합니다.", "제목");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入标题。", "输入错误");
				}
				else 
				{
					MessageBox.Show("You must input Title.", "Title Error");
				}
				this.textBoxTitle.Select();
				return;
			}

			if(m_list.Items.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("저장할 태그를 하나 이상 추가하여야 합니다.", "태그 없음");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请添加一个以上的标记。", "没有标记");
				}
				else 
				{
					MessageBox.Show("Register tag to save.", "Tag not registered.");
				}
				return;
			}

            //20250619 PSU 존재하지 않는 태그 삭제 추가.
            for (int i = m_list.Items.Count - 1; i >= 0; i--) // 역순으로 순회 (삭제를 위해)
            {
                MILLI_DATA_TAG tag = new MILLI_DATA_TAG();
                tag.tag = m_list.Items[i].Text.Trim(); // i번째 항목 사용

                if (!TagLib.GetTagTypeAndPos(tag.tag, ref tag.tag_type, ref tag.tag_pos))
                {
                    DialogResult result;

                    if (Tools.IsLangKorean())
                    {
                        result = MessageBox.Show(
                            $"{tag.tag}\n존재하지 않는 태그입니다.\n목록에서 삭제하시겠습니까?",
                            "태그 없음",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    }
                    else
                    {
                        result = MessageBox.Show(
                            $"{tag.tag}\nThis tag does not exist.\nDo you want to remove it from the list?",
                            "Tag Not Found",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    }

                    if (result == DialogResult.Yes)
                    {
                        m_list.Items.RemoveAt(i);
                    }
                }
            }

            m_nCondition = GetRadioCondition();

			if(m_nCondition == 1) 
			{
				if(this.textBoxCheckDI.Text.Length == 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						MessageBox.Show("DI 태그를 입력하여야 합니다.", "태그 추가");
					}
					else if(Tools.IsLangChinese()) 
					{
						MessageBox.Show("请输入[DI标记]。", "输入错误");
					}
					else 
					{
						MessageBox.Show("Must Input DI Tag.", "DI Tag.");
					}
					this.textBoxCheckDI.Select();
					return;
				}
			}

			DialogResult = DialogResult.OK;
			Close();

			
		}

		private void buttonDeleteMember_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0)
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("삭제하고 싶은 항목을 선택한 후 다시 하세요.", "삭제 오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else 
				{
					MessageBox.Show("Select tag to delete.", "Delete Error");
				}
				return;
			}

			int retn = m_list.SelectedItems[0].Index;

			m_list.Items.RemoveAt(retn);
		}

		private void buttonCheckDI_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;

			if(SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxCheckDI.Text = tag;
			}		
		}

		int GetRadioCondition()
		{
			int val;

			if(this.radioButtonCondition0.Checked)		val = 0;
			else if(this.radioButtonCondition1.Checked)	val = 1;
			else if(this.radioButtonCondition2.Checked)	val = 2;
			else										val = 0;
			return val;
		}

        int GetRadioSaveType()
        {
            int val;

            if (this.radioButtonSaveType0.Checked) val = 0;
            else if (this.radioButtonSaveType1.Checked) val = 1;
            else if (this.radioButtonSaveType2.Checked) val = 2;
            else val = 0;
            return val;
        }

		int GetRadioCutMethod()
		{
			int val;

			if(this.radioButtonSizeCut0.Checked)		val = 0;
			else if(this.radioButtonSizeCut1.Checked)	val = 1;
			else if(this.radioButtonSizeCut2.Checked)	val = 2;
			else if(this.radioButtonSizeCut3.Checked)	val = 3;
			else if(this.radioButtonSizeCut4.Checked)	val = 4;
			else if(this.radioButtonSizeCut5.Checked)	val = 5;
			else if(this.radioButtonSizeCut6.Checked)	val = 6;
			else										val = 0;

			return val;
		}

		void EnableDisable()
		{
			int condition = GetRadioCondition();

			bool tag_flag = false;
			bool cut_flag = false;

			if(condition == 1) 
			{
				tag_flag = true;
				cut_flag = true;
			}
			else if(condition == 2) 
			{
				tag_flag = false;
				cut_flag = true;
			}
			else 
			{
				tag_flag = false;
				cut_flag = false;
			}

			this.textBoxCheckDI.Enabled = tag_flag;
			this.buttonCheckDI.Enabled = tag_flag;

			this.textBoxSizeCut.Enabled = cut_flag;
			this.radioButtonSizeCut0.Enabled = cut_flag;
			this.radioButtonSizeCut1.Enabled = cut_flag;
			this.radioButtonSizeCut2.Enabled = cut_flag;
			this.radioButtonSizeCut3.Enabled = cut_flag;
			this.radioButtonSizeCut4.Enabled = cut_flag;
			this.radioButtonSizeCut5.Enabled = cut_flag;
			this.radioButtonSizeCut6.Enabled = cut_flag;
		}

		private void radioButtonCondition0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();	
		}

		private void radioButtonCondition1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();	
		}

		private void radioButtonCondition2_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();	
		}

		public void GetStruct(ref MILLI_DATA_STRUCT item)
		{
			item.nCondition = GetRadioCondition();
			item.nCutMethod = GetRadioCutMethod();
            item.nSaveFileType = GetRadioSaveType();
			item.nGab = ConvertTool.ToInt32(this.numericUpDownDataUnit.Value);
			item.nSizeCut = ConvertTool.ToInt32(this.textBoxSizeCut.Text);
			item.tagCheckDI = this.textBoxCheckDI.Text;
			item.title = this.textBoxTitle.Text;
			//item.bTimeSave = this.checkBoxDateTimeColumn.Checked ? (sbyte)1 : (sbyte)0;
            item.bDateTimeMatch = this.checkBoxMatchToDateTime.Checked;

            item.bAutoDelete = this.checkBoxUseAutoDelete.Checked;
            item.nDaysOfAutoDelete = ConvertTool.ToInt32(this.numericUpDownDaysOfAutoDelete.Value);

            item.bUseTargetFolder = this.checkBoxSpecifyTargetFolder.Checked;
            item.sTargetFolder = this.textBoxCsvTargetFolder.Text;

            item.bUseCsvDateFolder = this.checkBoxUseCsvDateFolder.Checked;

            item.bUseFilenameAddition = this.checkBoxUseFilenameAddition.Checked;
            item.sUseFilenameAdditionTag = this.textBoxUseFilenameAdditionTag.Text;

			MILLI_DATA_TAG tag;
			int i;
			ListViewItem lvi;

			item.blockTag = new ArrayList();

			for(i = 0; i < m_list.Items.Count; i++) 
			{
				lvi = m_list.Items[i];
				tag = new MILLI_DATA_TAG();
				tag.tag = lvi.SubItems[0].Text;
				item.blockTag.Add(tag);
			}
		}

		public void SetStruct(MILLI_DATA_STRUCT item)
		{
			this.m_nCondition = item.nCondition;
			this.m_nCutMethod = item.nCutMethod;
            this.m_nSaveType = item.nSaveFileType;

			this.numericUpDownDataUnit.Value = item.nGab;
			this.textBoxSizeCut.Text = item.nSizeCut.ToString();
			this.textBoxCheckDI.Text = item.tagCheckDI;
			this.textBoxTitle.Text = item.title;
			//this.checkBoxDateTimeColumn.Checked = (item.bTimeSave == 1);
            this.checkBoxMatchToDateTime.Checked = item.bDateTimeMatch;

            this.checkBoxUseAutoDelete.Checked = item.bAutoDelete;
            Tools.SetNumericUpDownValue(this.numericUpDownDaysOfAutoDelete, item.nDaysOfAutoDelete);

			this.blockTag = (ArrayList)Tools.CopyObject(item.blockTag);

            this.checkBoxSpecifyTargetFolder.Checked = item.bUseTargetFolder;
            this.textBoxCsvTargetFolder.Text = item.sTargetFolder;

            this.checkBoxUseCsvDateFolder.Checked = item.bUseCsvDateFolder;

            this.checkBoxUseFilenameAddition.Checked = item.bUseFilenameAddition;
            this.textBoxUseFilenameAdditionTag.Text = item.sUseFilenameAdditionTag;

            FillListBox();

            EnableDisableTargetFolder();
            EnableUseFilenameAddition();
		}

		private void radioButtonSizeCut5_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		private void radioButtonSizeCut4_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

        private void buttonCsvTargetFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.SelectedPath = this.textBoxCsvTargetFolder.Text;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxCsvTargetFolder.Text = dialog.SelectedPath;
            }
        }

        void EnableDisableTargetFolder()
        {
            bool flag = this.checkBoxSpecifyTargetFolder.Checked;

            this.textBoxCsvTargetFolder.Enabled = flag;
            this.buttonCsvTargetFolder.Enabled = flag;
        }

        private void checkBoxSpecifyTargetFolder_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableTargetFolder();
        }

        void EnableUseFilenameAddition()
        {
            bool flag = this.checkBoxUseFilenameAddition.Checked;

            this.textBoxUseFilenameAdditionTag.Enabled = flag;
            this.buttonUseFilenameAdditionTag.Enabled = flag;
        }

        private void checkBoxUseFilenameAddition_CheckedChanged(object sender, EventArgs e)
        {
            EnableUseFilenameAddition();
        }

        private void buttonUseFilenameAdditionTag_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (SelectTag.SelectSt(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxUseFilenameAdditionTag.Text = tag;
            }		
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0 || m_list.SelectedItems[0].Index == 0)
                return;

            int selectedIndex = m_list.SelectedItems[0].Index;
            ListViewItem item = m_list.SelectedItems[0];
            m_list.Items.RemoveAt(selectedIndex);
            m_list.Items.Insert(selectedIndex - 1, item);
            m_list.Items[selectedIndex - 1].Selected = true;
            m_list.Select();

            EnsureVisible(selectedIndex - 1);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0 || m_list.SelectedItems[0].Index == m_list.Items.Count - 1)
                return;

            int selectedIndex = m_list.SelectedItems[0].Index;
            ListViewItem item = m_list.SelectedItems[0];
            m_list.Items.RemoveAt(selectedIndex);
            m_list.Items.Insert(selectedIndex + 1, item);
            m_list.Items[selectedIndex + 1].Selected = true;
            m_list.Select();

            EnsureVisible(selectedIndex + 1);
        }

        private void EnsureVisible(int index)
        {
            if (index >= 0 && index < m_list.Items.Count)
            {
                m_list.EnsureVisible(index);
            }
        }
	}
}



