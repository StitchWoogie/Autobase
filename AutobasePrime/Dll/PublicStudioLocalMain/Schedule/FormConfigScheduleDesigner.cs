using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using PublicStudioLocalMain.Schedule;
using NetTools.OldDefine;
using DialogTag;
using DialogHoliday;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigScheduleDesigner.
	/// </summary>
	public class FormConfigScheduleDesigner : System.Windows.Forms.Form
    {
        private IContainer components;

		public FormConfigScheduleDesigner()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScheduleDesigner));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxSpecial = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxHoliday = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxSat = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxFri = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxThu = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxWed = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxTue = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxMon = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSun = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonModelEdit = new System.Windows.Forms.Button();
            this.checkBoxModelSetView = new System.Windows.Forms.CheckBox();
            this.m_listModel = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonModelSetOff = new System.Windows.Forms.Button();
            this.buttonModelSetOn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.m_list_FixedSchedule = new System.Windows.Forms.ListBox();
            this.buttonScheduleModify = new System.Windows.Forms.Button();
            this.buttonScheduleDelete = new System.Windows.Forms.Button();
            this.buttonScheduleAdd = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.m_list_ScheduleAdditional = new System.Windows.Forms.ListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.buttonScheduleaAdditionalDelete = new System.Windows.Forms.Button();
            this.buttonScheduleAdditionalAdd = new System.Windows.Forms.Button();
            this.groupBoxScheduleSAdditional = new System.Windows.Forms.GroupBox();
            this.comboBoxScheduleAdditionalModel = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBoxScheduleAdditionalConfigDate = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.radioButtonScheduleAdditionalLunar = new System.Windows.Forms.RadioButton();
            this.radioButtonScheduleAdditionalSolar = new System.Windows.Forms.RadioButton();
            this.comboBoxScheduleAdditionalWeekday = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxScheduleAdditionalWeek = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBoxScheduleAdditionalDay = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxScheduleAdditionalMonth = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.comboBoxScheduleAdditionalYear = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.radioButtonScheduleAdditionalDateType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonScheduleAdditionalDateType0 = new System.Windows.Forms.RadioButton();
            this.textBoxScheduleAdditionalUserType = new System.Windows.Forms.TextBox();
            this.radioButtonScheduleAdditionalType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonScheduleAdditionalType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonScheduleAdditionalType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonScheduleAdditionalType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonScheduleAdditionalType0 = new System.Windows.Forms.RadioButton();
            this.label22 = new System.Windows.Forms.Label();
            this.buttonScheduleAdditionalApply = new System.Windows.Forms.Button();
            this.textBoxScheduleAdditionalTitle = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.radioButtonHoliday = new System.Windows.Forms.RadioButton();
            this.radioButtonSpecial = new System.Windows.Forms.RadioButton();
            this.buttonDeleteHolidaySpecial = new System.Windows.Forms.Button();
            this.m_list_HolidaySpecial = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.buttonAddHolidaySpecial = new System.Windows.Forms.Button();
            this.groupBoxHolidaySpecialDate = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButtonHolidaySpecialLunar = new System.Windows.Forms.RadioButton();
            this.radioButtonHolidaySpecialSolar = new System.Windows.Forms.RadioButton();
            this.comboBoxHolidaySpecialWeekday = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxHolidaySpecialWeek = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxHolidaySpecialDay = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxHolidaySpecialMonth = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxHolidaySpecialYear = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radioButtonHolidaySpecialType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonHolidaySpecialType0 = new System.Windows.Forms.RadioButton();
            this.buttonHolidaySpecialApply = new System.Windows.Forms.Button();
            this.textBoxHolidaySpecialTitle = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBoxScheduleSAdditional.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBoxScheduleAdditionalConfigDate.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBoxHolidaySpecialDate.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxSpecial);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.comboBoxHoliday);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.comboBoxSat);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.comboBoxFri);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.comboBoxThu);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.comboBoxWed);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.comboBoxTue);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.comboBoxMon);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.comboBoxSun);
            this.groupBox3.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // comboBoxSpecial
            // 
            this.comboBoxSpecial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSpecial, "comboBoxSpecial");
            this.comboBoxSpecial.Name = "comboBoxSpecial";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // comboBoxHoliday
            // 
            this.comboBoxHoliday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHoliday, "comboBoxHoliday");
            this.comboBoxHoliday.Name = "comboBoxHoliday";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // comboBoxSat
            // 
            this.comboBoxSat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSat, "comboBoxSat");
            this.comboBoxSat.Name = "comboBoxSat";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // comboBoxFri
            // 
            this.comboBoxFri.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxFri, "comboBoxFri");
            this.comboBoxFri.Name = "comboBoxFri";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comboBoxThu
            // 
            this.comboBoxThu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxThu, "comboBoxThu");
            this.comboBoxThu.Name = "comboBoxThu";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // comboBoxWed
            // 
            this.comboBoxWed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxWed, "comboBoxWed");
            this.comboBoxWed.Name = "comboBoxWed";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // comboBoxTue
            // 
            this.comboBoxTue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxTue, "comboBoxTue");
            this.comboBoxTue.Name = "comboBoxTue";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxMon
            // 
            this.comboBoxMon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxMon, "comboBoxMon");
            this.comboBoxMon.Name = "comboBoxMon";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBoxSun
            // 
            this.comboBoxSun.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSun, "comboBoxSun");
            this.comboBoxSun.Name = "comboBoxSun";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonModelEdit);
            this.groupBox1.Controls.Add(this.checkBoxModelSetView);
            this.groupBox1.Controls.Add(this.m_listModel);
            this.groupBox1.Controls.Add(this.buttonModelSetOff);
            this.groupBox1.Controls.Add(this.buttonModelSetOn);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonModelEdit
            // 
            resources.ApplyResources(this.buttonModelEdit, "buttonModelEdit");
            this.buttonModelEdit.Name = "buttonModelEdit";
            this.buttonModelEdit.UseVisualStyleBackColor = true;
            this.buttonModelEdit.Click += new System.EventHandler(this.buttonModelEdit_Click);
            // 
            // checkBoxModelSetView
            // 
            resources.ApplyResources(this.checkBoxModelSetView, "checkBoxModelSetView");
            this.checkBoxModelSetView.Name = "checkBoxModelSetView";
            this.checkBoxModelSetView.UseVisualStyleBackColor = true;
            this.checkBoxModelSetView.CheckedChanged += new System.EventHandler(this.checkBoxModelSetView_CheckedChanged);
            // 
            // m_listModel
            // 
            this.m_listModel.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_listModel.FullRowSelect = true;
            this.m_listModel.HideSelection = false;
            resources.ApplyResources(this.m_listModel, "m_listModel");
            this.m_listModel.Name = "m_listModel";
            this.m_listModel.SmallImageList = this.imageList1;
            this.m_listModel.UseCompatibleStateImageBehavior = false;
            this.m_listModel.View = System.Windows.Forms.View.Details;
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
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "redM.bmp");
            this.imageList1.Images.SetKeyName(1, "greenM.bmp");
            // 
            // buttonModelSetOff
            // 
            this.buttonModelSetOff.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonModelSetOff, "buttonModelSetOff");
            this.buttonModelSetOff.Name = "buttonModelSetOff";
            this.buttonModelSetOff.UseVisualStyleBackColor = false;
            this.buttonModelSetOff.Click += new System.EventHandler(this.buttonModelSetOff_Click);
            // 
            // buttonModelSetOn
            // 
            this.buttonModelSetOn.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonModelSetOn, "buttonModelSetOn");
            this.buttonModelSetOn.Name = "buttonModelSetOn";
            this.buttonModelSetOn.UseVisualStyleBackColor = false;
            this.buttonModelSetOn.Click += new System.EventHandler(this.buttonModelSetOn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.m_list_FixedSchedule);
            this.groupBox2.Controls.Add(this.buttonScheduleModify);
            this.groupBox2.Controls.Add(this.buttonScheduleDelete);
            this.groupBox2.Controls.Add(this.buttonScheduleAdd);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // m_list_FixedSchedule
            // 
            this.m_list_FixedSchedule.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.m_list_FixedSchedule, "m_list_FixedSchedule");
            this.m_list_FixedSchedule.Name = "m_list_FixedSchedule";
            this.m_list_FixedSchedule.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.m_list_FixedSchedule_DrawItem);
            this.m_list_FixedSchedule.SelectedIndexChanged += new System.EventHandler(this.m_list_FixedSchedule_SelectedIndexChanged);
            this.m_list_FixedSchedule.DoubleClick += new System.EventHandler(this.m_list_FixedSchedule_DoubleClick);
            // 
            // buttonScheduleModify
            // 
            this.buttonScheduleModify.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonScheduleModify, "buttonScheduleModify");
            this.buttonScheduleModify.Name = "buttonScheduleModify";
            this.buttonScheduleModify.UseVisualStyleBackColor = false;
            this.buttonScheduleModify.Click += new System.EventHandler(this.buttonScheduleModify_Click);
            // 
            // buttonScheduleDelete
            // 
            this.buttonScheduleDelete.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonScheduleDelete, "buttonScheduleDelete");
            this.buttonScheduleDelete.Name = "buttonScheduleDelete";
            this.buttonScheduleDelete.UseVisualStyleBackColor = false;
            this.buttonScheduleDelete.Click += new System.EventHandler(this.buttonScheduleDelete_Click);
            // 
            // buttonScheduleAdd
            // 
            this.buttonScheduleAdd.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonScheduleAdd, "buttonScheduleAdd");
            this.buttonScheduleAdd.Name = "buttonScheduleAdd";
            this.buttonScheduleAdd.UseVisualStyleBackColor = false;
            this.buttonScheduleAdd.Click += new System.EventHandler(this.buttonScheduleAdd_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.groupBoxScheduleSAdditional);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.m_list_ScheduleAdditional);
            this.groupBox8.Controls.Add(this.buttonScheduleaAdditionalDelete);
            this.groupBox8.Controls.Add(this.buttonScheduleAdditionalAdd);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // m_list_ScheduleAdditional
            // 
            this.m_list_ScheduleAdditional.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.m_list_ScheduleAdditional.FullRowSelect = true;
            this.m_list_ScheduleAdditional.HideSelection = false;
            resources.ApplyResources(this.m_list_ScheduleAdditional, "m_list_ScheduleAdditional");
            this.m_list_ScheduleAdditional.MultiSelect = false;
            this.m_list_ScheduleAdditional.Name = "m_list_ScheduleAdditional";
            this.m_list_ScheduleAdditional.UseCompatibleStateImageBehavior = false;
            this.m_list_ScheduleAdditional.View = System.Windows.Forms.View.Details;
            this.m_list_ScheduleAdditional.SelectedIndexChanged += new System.EventHandler(this.m_list_ScheduleAdditional_SelectedIndexChanged);
            // 
            // columnHeader9
            // 
            resources.ApplyResources(this.columnHeader9, "columnHeader9");
            // 
            // columnHeader10
            // 
            resources.ApplyResources(this.columnHeader10, "columnHeader10");
            // 
            // columnHeader11
            // 
            resources.ApplyResources(this.columnHeader11, "columnHeader11");
            // 
            // buttonScheduleaAdditionalDelete
            // 
            this.buttonScheduleaAdditionalDelete.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonScheduleaAdditionalDelete, "buttonScheduleaAdditionalDelete");
            this.buttonScheduleaAdditionalDelete.Name = "buttonScheduleaAdditionalDelete";
            this.buttonScheduleaAdditionalDelete.UseVisualStyleBackColor = false;
            this.buttonScheduleaAdditionalDelete.Click += new System.EventHandler(this.buttonScheduleaAdditionalDelete_Click);
            // 
            // buttonScheduleAdditionalAdd
            // 
            this.buttonScheduleAdditionalAdd.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonScheduleAdditionalAdd, "buttonScheduleAdditionalAdd");
            this.buttonScheduleAdditionalAdd.Name = "buttonScheduleAdditionalAdd";
            this.buttonScheduleAdditionalAdd.UseVisualStyleBackColor = false;
            this.buttonScheduleAdditionalAdd.Click += new System.EventHandler(this.buttonScheduleAdditionalAdd_Click);
            // 
            // groupBoxScheduleSAdditional
            // 
            this.groupBoxScheduleSAdditional.Controls.Add(this.comboBoxScheduleAdditionalModel);
            this.groupBoxScheduleSAdditional.Controls.Add(this.groupBox4);
            this.groupBoxScheduleSAdditional.Controls.Add(this.label22);
            this.groupBoxScheduleSAdditional.Controls.Add(this.buttonScheduleAdditionalApply);
            this.groupBoxScheduleSAdditional.Controls.Add(this.textBoxScheduleAdditionalTitle);
            this.groupBoxScheduleSAdditional.Controls.Add(this.label21);
            resources.ApplyResources(this.groupBoxScheduleSAdditional, "groupBoxScheduleSAdditional");
            this.groupBoxScheduleSAdditional.Name = "groupBoxScheduleSAdditional";
            this.groupBoxScheduleSAdditional.TabStop = false;
            // 
            // comboBoxScheduleAdditionalModel
            // 
            resources.ApplyResources(this.comboBoxScheduleAdditionalModel, "comboBoxScheduleAdditionalModel");
            this.comboBoxScheduleAdditionalModel.Name = "comboBoxScheduleAdditionalModel";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBoxScheduleAdditionalConfigDate);
            this.groupBox4.Controls.Add(this.textBoxScheduleAdditionalUserType);
            this.groupBox4.Controls.Add(this.radioButtonScheduleAdditionalType4);
            this.groupBox4.Controls.Add(this.radioButtonScheduleAdditionalType3);
            this.groupBox4.Controls.Add(this.radioButtonScheduleAdditionalType2);
            this.groupBox4.Controls.Add(this.radioButtonScheduleAdditionalType1);
            this.groupBox4.Controls.Add(this.radioButtonScheduleAdditionalType0);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // groupBoxScheduleAdditionalConfigDate
            // 
            this.groupBoxScheduleAdditionalConfigDate.Controls.Add(this.groupBox10);
            this.groupBoxScheduleAdditionalConfigDate.Controls.Add(this.groupBox11);
            resources.ApplyResources(this.groupBoxScheduleAdditionalConfigDate, "groupBoxScheduleAdditionalConfigDate");
            this.groupBoxScheduleAdditionalConfigDate.Name = "groupBoxScheduleAdditionalConfigDate";
            this.groupBoxScheduleAdditionalConfigDate.TabStop = false;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.radioButtonScheduleAdditionalLunar);
            this.groupBox10.Controls.Add(this.radioButtonScheduleAdditionalSolar);
            this.groupBox10.Controls.Add(this.comboBoxScheduleAdditionalWeekday);
            this.groupBox10.Controls.Add(this.label16);
            this.groupBox10.Controls.Add(this.comboBoxScheduleAdditionalWeek);
            this.groupBox10.Controls.Add(this.label17);
            this.groupBox10.Controls.Add(this.comboBoxScheduleAdditionalDay);
            this.groupBox10.Controls.Add(this.label18);
            this.groupBox10.Controls.Add(this.comboBoxScheduleAdditionalMonth);
            this.groupBox10.Controls.Add(this.label19);
            this.groupBox10.Controls.Add(this.comboBoxScheduleAdditionalYear);
            this.groupBox10.Controls.Add(this.label20);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // radioButtonScheduleAdditionalLunar
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalLunar, "radioButtonScheduleAdditionalLunar");
            this.radioButtonScheduleAdditionalLunar.Name = "radioButtonScheduleAdditionalLunar";
            // 
            // radioButtonScheduleAdditionalSolar
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalSolar, "radioButtonScheduleAdditionalSolar");
            this.radioButtonScheduleAdditionalSolar.Name = "radioButtonScheduleAdditionalSolar";
            // 
            // comboBoxScheduleAdditionalWeekday
            // 
            this.comboBoxScheduleAdditionalWeekday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxScheduleAdditionalWeekday, "comboBoxScheduleAdditionalWeekday");
            this.comboBoxScheduleAdditionalWeekday.Name = "comboBoxScheduleAdditionalWeekday";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // comboBoxScheduleAdditionalWeek
            // 
            this.comboBoxScheduleAdditionalWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxScheduleAdditionalWeek, "comboBoxScheduleAdditionalWeek");
            this.comboBoxScheduleAdditionalWeek.Name = "comboBoxScheduleAdditionalWeek";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // comboBoxScheduleAdditionalDay
            // 
            this.comboBoxScheduleAdditionalDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxScheduleAdditionalDay, "comboBoxScheduleAdditionalDay");
            this.comboBoxScheduleAdditionalDay.Name = "comboBoxScheduleAdditionalDay";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // comboBoxScheduleAdditionalMonth
            // 
            this.comboBoxScheduleAdditionalMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxScheduleAdditionalMonth, "comboBoxScheduleAdditionalMonth");
            this.comboBoxScheduleAdditionalMonth.Name = "comboBoxScheduleAdditionalMonth";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // comboBoxScheduleAdditionalYear
            // 
            this.comboBoxScheduleAdditionalYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxScheduleAdditionalYear, "comboBoxScheduleAdditionalYear");
            this.comboBoxScheduleAdditionalYear.Name = "comboBoxScheduleAdditionalYear";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.radioButtonScheduleAdditionalDateType1);
            this.groupBox11.Controls.Add(this.radioButtonScheduleAdditionalDateType0);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // radioButtonScheduleAdditionalDateType1
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalDateType1, "radioButtonScheduleAdditionalDateType1");
            this.radioButtonScheduleAdditionalDateType1.Name = "radioButtonScheduleAdditionalDateType1";
            this.radioButtonScheduleAdditionalDateType1.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalDateType1_CheckedChanged);
            // 
            // radioButtonScheduleAdditionalDateType0
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalDateType0, "radioButtonScheduleAdditionalDateType0");
            this.radioButtonScheduleAdditionalDateType0.Name = "radioButtonScheduleAdditionalDateType0";
            this.radioButtonScheduleAdditionalDateType0.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalDateType0_CheckedChanged);
            // 
            // textBoxScheduleAdditionalUserType
            // 
            resources.ApplyResources(this.textBoxScheduleAdditionalUserType, "textBoxScheduleAdditionalUserType");
            this.textBoxScheduleAdditionalUserType.Name = "textBoxScheduleAdditionalUserType";
            this.textBoxScheduleAdditionalUserType.ReadOnly = true;
            // 
            // radioButtonScheduleAdditionalType4
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalType4, "radioButtonScheduleAdditionalType4");
            this.radioButtonScheduleAdditionalType4.Name = "radioButtonScheduleAdditionalType4";
            this.radioButtonScheduleAdditionalType4.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalType4_CheckedChanged);
            // 
            // radioButtonScheduleAdditionalType3
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalType3, "radioButtonScheduleAdditionalType3");
            this.radioButtonScheduleAdditionalType3.Name = "radioButtonScheduleAdditionalType3";
            this.radioButtonScheduleAdditionalType3.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalType3_CheckedChanged);
            // 
            // radioButtonScheduleAdditionalType2
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalType2, "radioButtonScheduleAdditionalType2");
            this.radioButtonScheduleAdditionalType2.Name = "radioButtonScheduleAdditionalType2";
            this.radioButtonScheduleAdditionalType2.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalType2_CheckedChanged);
            // 
            // radioButtonScheduleAdditionalType1
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalType1, "radioButtonScheduleAdditionalType1");
            this.radioButtonScheduleAdditionalType1.Name = "radioButtonScheduleAdditionalType1";
            this.radioButtonScheduleAdditionalType1.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalType1_CheckedChanged);
            // 
            // radioButtonScheduleAdditionalType0
            // 
            resources.ApplyResources(this.radioButtonScheduleAdditionalType0, "radioButtonScheduleAdditionalType0");
            this.radioButtonScheduleAdditionalType0.Name = "radioButtonScheduleAdditionalType0";
            this.radioButtonScheduleAdditionalType0.CheckedChanged += new System.EventHandler(this.radioButtonScheduleAdditionalType0_CheckedChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // buttonScheduleAdditionalApply
            // 
            this.buttonScheduleAdditionalApply.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonScheduleAdditionalApply, "buttonScheduleAdditionalApply");
            this.buttonScheduleAdditionalApply.Name = "buttonScheduleAdditionalApply";
            this.buttonScheduleAdditionalApply.UseVisualStyleBackColor = false;
            this.buttonScheduleAdditionalApply.Click += new System.EventHandler(this.buttonScheduleAdditionalApply_Click);
            // 
            // textBoxScheduleAdditionalTitle
            // 
            resources.ApplyResources(this.textBoxScheduleAdditionalTitle, "textBoxScheduleAdditionalTitle");
            this.textBoxScheduleAdditionalTitle.Name = "textBoxScheduleAdditionalTitle";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.groupBoxHolidaySpecialDate);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.radioButtonHoliday);
            this.groupBox7.Controls.Add(this.radioButtonSpecial);
            this.groupBox7.Controls.Add(this.buttonDeleteHolidaySpecial);
            this.groupBox7.Controls.Add(this.m_list_HolidaySpecial);
            this.groupBox7.Controls.Add(this.buttonAddHolidaySpecial);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // radioButtonHoliday
            // 
            resources.ApplyResources(this.radioButtonHoliday, "radioButtonHoliday");
            this.radioButtonHoliday.Name = "radioButtonHoliday";
            this.radioButtonHoliday.TabStop = true;
            this.radioButtonHoliday.UseVisualStyleBackColor = true;
            this.radioButtonHoliday.CheckedChanged += new System.EventHandler(this.radioButtonHoliday_CheckedChanged);
            // 
            // radioButtonSpecial
            // 
            resources.ApplyResources(this.radioButtonSpecial, "radioButtonSpecial");
            this.radioButtonSpecial.Name = "radioButtonSpecial";
            this.radioButtonSpecial.TabStop = true;
            this.radioButtonSpecial.UseVisualStyleBackColor = true;
            this.radioButtonSpecial.CheckedChanged += new System.EventHandler(this.radioButtonSpecial_CheckedChanged);
            // 
            // buttonDeleteHolidaySpecial
            // 
            this.buttonDeleteHolidaySpecial.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonDeleteHolidaySpecial, "buttonDeleteHolidaySpecial");
            this.buttonDeleteHolidaySpecial.Name = "buttonDeleteHolidaySpecial";
            this.buttonDeleteHolidaySpecial.UseVisualStyleBackColor = false;
            this.buttonDeleteHolidaySpecial.Click += new System.EventHandler(this.buttonDeleteHolidaySpecial_Click);
            // 
            // m_list_HolidaySpecial
            // 
            this.m_list_HolidaySpecial.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.m_list_HolidaySpecial.FullRowSelect = true;
            this.m_list_HolidaySpecial.HideSelection = false;
            resources.ApplyResources(this.m_list_HolidaySpecial, "m_list_HolidaySpecial");
            this.m_list_HolidaySpecial.MultiSelect = false;
            this.m_list_HolidaySpecial.Name = "m_list_HolidaySpecial";
            this.m_list_HolidaySpecial.UseCompatibleStateImageBehavior = false;
            this.m_list_HolidaySpecial.View = System.Windows.Forms.View.Details;
            this.m_list_HolidaySpecial.SelectedIndexChanged += new System.EventHandler(this.m_list_HolidaySpecial_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // buttonAddHolidaySpecial
            // 
            this.buttonAddHolidaySpecial.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonAddHolidaySpecial, "buttonAddHolidaySpecial");
            this.buttonAddHolidaySpecial.Name = "buttonAddHolidaySpecial";
            this.buttonAddHolidaySpecial.UseVisualStyleBackColor = false;
            this.buttonAddHolidaySpecial.Click += new System.EventHandler(this.buttonAddHolidaySpecial_Click);
            // 
            // groupBoxHolidaySpecialDate
            // 
            this.groupBoxHolidaySpecialDate.Controls.Add(this.groupBox5);
            this.groupBoxHolidaySpecialDate.Controls.Add(this.groupBox6);
            this.groupBoxHolidaySpecialDate.Controls.Add(this.buttonHolidaySpecialApply);
            this.groupBoxHolidaySpecialDate.Controls.Add(this.textBoxHolidaySpecialTitle);
            this.groupBoxHolidaySpecialDate.Controls.Add(this.label15);
            resources.ApplyResources(this.groupBoxHolidaySpecialDate, "groupBoxHolidaySpecialDate");
            this.groupBoxHolidaySpecialDate.Name = "groupBoxHolidaySpecialDate";
            this.groupBoxHolidaySpecialDate.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButtonHolidaySpecialLunar);
            this.groupBox5.Controls.Add(this.radioButtonHolidaySpecialSolar);
            this.groupBox5.Controls.Add(this.comboBoxHolidaySpecialWeekday);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.comboBoxHolidaySpecialWeek);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.comboBoxHolidaySpecialDay);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.comboBoxHolidaySpecialMonth);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.comboBoxHolidaySpecialYear);
            this.groupBox5.Controls.Add(this.label14);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // radioButtonHolidaySpecialLunar
            // 
            resources.ApplyResources(this.radioButtonHolidaySpecialLunar, "radioButtonHolidaySpecialLunar");
            this.radioButtonHolidaySpecialLunar.Name = "radioButtonHolidaySpecialLunar";
            // 
            // radioButtonHolidaySpecialSolar
            // 
            resources.ApplyResources(this.radioButtonHolidaySpecialSolar, "radioButtonHolidaySpecialSolar");
            this.radioButtonHolidaySpecialSolar.Name = "radioButtonHolidaySpecialSolar";
            // 
            // comboBoxHolidaySpecialWeekday
            // 
            this.comboBoxHolidaySpecialWeekday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHolidaySpecialWeekday, "comboBoxHolidaySpecialWeekday");
            this.comboBoxHolidaySpecialWeekday.Name = "comboBoxHolidaySpecialWeekday";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // comboBoxHolidaySpecialWeek
            // 
            this.comboBoxHolidaySpecialWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHolidaySpecialWeek, "comboBoxHolidaySpecialWeek");
            this.comboBoxHolidaySpecialWeek.Name = "comboBoxHolidaySpecialWeek";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // comboBoxHolidaySpecialDay
            // 
            this.comboBoxHolidaySpecialDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHolidaySpecialDay, "comboBoxHolidaySpecialDay");
            this.comboBoxHolidaySpecialDay.Name = "comboBoxHolidaySpecialDay";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // comboBoxHolidaySpecialMonth
            // 
            this.comboBoxHolidaySpecialMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHolidaySpecialMonth, "comboBoxHolidaySpecialMonth");
            this.comboBoxHolidaySpecialMonth.Name = "comboBoxHolidaySpecialMonth";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // comboBoxHolidaySpecialYear
            // 
            this.comboBoxHolidaySpecialYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHolidaySpecialYear, "comboBoxHolidaySpecialYear");
            this.comboBoxHolidaySpecialYear.Name = "comboBoxHolidaySpecialYear";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radioButtonHolidaySpecialType1);
            this.groupBox6.Controls.Add(this.radioButtonHolidaySpecialType0);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // radioButtonHolidaySpecialType1
            // 
            resources.ApplyResources(this.radioButtonHolidaySpecialType1, "radioButtonHolidaySpecialType1");
            this.radioButtonHolidaySpecialType1.Name = "radioButtonHolidaySpecialType1";
            this.radioButtonHolidaySpecialType1.CheckedChanged += new System.EventHandler(this.radioButtonHolidaySpecialType1_CheckedChanged);
            // 
            // radioButtonHolidaySpecialType0
            // 
            resources.ApplyResources(this.radioButtonHolidaySpecialType0, "radioButtonHolidaySpecialType0");
            this.radioButtonHolidaySpecialType0.Name = "radioButtonHolidaySpecialType0";
            this.radioButtonHolidaySpecialType0.CheckedChanged += new System.EventHandler(this.radioButtonHolidaySpecialType0_CheckedChanged);
            // 
            // buttonHolidaySpecialApply
            // 
            this.buttonHolidaySpecialApply.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.buttonHolidaySpecialApply, "buttonHolidaySpecialApply");
            this.buttonHolidaySpecialApply.Name = "buttonHolidaySpecialApply";
            this.buttonHolidaySpecialApply.UseVisualStyleBackColor = false;
            this.buttonHolidaySpecialApply.Click += new System.EventHandler(this.buttonHolidaySpecialApply_Click);
            // 
            // textBoxHolidaySpecialTitle
            // 
            resources.ApplyResources(this.textBoxHolidaySpecialTitle, "textBoxHolidaySpecialTitle");
            this.textBoxHolidaySpecialTitle.Name = "textBoxHolidaySpecialTitle";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // FormConfigScheduleDesigner
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScheduleDesigner";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigScheduleDesigner_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBoxScheduleSAdditional.ResumeLayout(false);
            this.groupBoxScheduleSAdditional.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBoxScheduleAdditionalConfigDate.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBoxHolidaySpecialDate.ResumeLayout(false);
            this.groupBoxHolidaySpecialDate.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        
        private TabControl tabControl1;
        private TabPage tabPage1;
        private GroupBox groupBox3;
        private ComboBox comboBoxSpecial;
        private Label label9;
        private ComboBox comboBoxHoliday;
        private Label label8;
        private ComboBox comboBoxSat;
        private Label label7;
        private ComboBox comboBoxFri;
        private Label label6;
        private ComboBox comboBoxThu;
        private Label label5;
        private ComboBox comboBoxWed;
        private Label label4;
        private ComboBox comboBoxTue;
        private Label label3;
        private ComboBox comboBoxMon;
        private Label label2;
        private ComboBox comboBoxSun;
        private Label label1;
        private GroupBox groupBox1;
        private Button buttonModelEdit;
        private CheckBox checkBoxModelSetView;
        private ListView m_listModel;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private Button buttonModelSetOff;
        private Button buttonModelSetOn;
        private GroupBox groupBox2;
        private ListBox m_list_FixedSchedule;
        private Button buttonScheduleModify;
        private Button buttonScheduleDelete;
        private Button buttonScheduleAdd;
        private TabPage tabPage2;
        private GroupBox groupBox8;
        private ListView m_list_ScheduleAdditional;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader10;
        private ColumnHeader columnHeader11;
        private Button buttonScheduleaAdditionalDelete;
        private Button buttonScheduleAdditionalAdd;
        private GroupBox groupBoxScheduleSAdditional;
        private ComboBox comboBoxScheduleAdditionalModel;
        private GroupBox groupBox4;
        private GroupBox groupBoxScheduleAdditionalConfigDate;
        private GroupBox groupBox10;
        private RadioButton radioButtonScheduleAdditionalLunar;
        private RadioButton radioButtonScheduleAdditionalSolar;
        private ComboBox comboBoxScheduleAdditionalWeekday;
        private Label label16;
        private ComboBox comboBoxScheduleAdditionalWeek;
        private Label label17;
        private ComboBox comboBoxScheduleAdditionalDay;
        private Label label18;
        private ComboBox comboBoxScheduleAdditionalMonth;
        private Label label19;
        private ComboBox comboBoxScheduleAdditionalYear;
        private Label label20;
        private GroupBox groupBox11;
        private RadioButton radioButtonScheduleAdditionalDateType1;
        private RadioButton radioButtonScheduleAdditionalDateType0;
        private TextBox textBoxScheduleAdditionalUserType;
        private RadioButton radioButtonScheduleAdditionalType4;
        private RadioButton radioButtonScheduleAdditionalType3;
        private RadioButton radioButtonScheduleAdditionalType2;
        private RadioButton radioButtonScheduleAdditionalType1;
        private RadioButton radioButtonScheduleAdditionalType0;
        private Label label22;
        private Button buttonScheduleAdditionalApply;
        public TextBox textBoxScheduleAdditionalTitle;
        private Label label21;
        private TabPage tabPage3;
        private GroupBox groupBox7;
        private RadioButton radioButtonHoliday;
        private RadioButton radioButtonSpecial;
        private Button buttonDeleteHolidaySpecial;
        private ListView m_list_HolidaySpecial;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private Button buttonAddHolidaySpecial;
        private GroupBox groupBoxHolidaySpecialDate;
        private GroupBox groupBox5;
        private RadioButton radioButtonHolidaySpecialLunar;
        private RadioButton radioButtonHolidaySpecialSolar;
        private ComboBox comboBoxHolidaySpecialWeekday;
        private Label label10;
        private ComboBox comboBoxHolidaySpecialWeek;
        private Label label11;
        private ComboBox comboBoxHolidaySpecialDay;
        private Label label12;
        private ComboBox comboBoxHolidaySpecialMonth;
        private Label label13;
        private ComboBox comboBoxHolidaySpecialYear;
        private Label label14;
        private GroupBox groupBox6;
        private RadioButton radioButtonHolidaySpecialType1;
        private RadioButton radioButtonHolidaySpecialType0;
        private Button buttonHolidaySpecialApply;
        public TextBox textBoxHolidaySpecialTitle;
        private Label label15;
        private Button buttonCancel;
        private Button buttonOK;
        private ImageList imageList1;


        //ScheduleWeek
        int nOldListPos = 0;
        bool bInitial = false;
        bool bDeleteFixed = false;

        Color[] preparedColor = new Color[10] { Color.Red, Color.Blue, Color.Green, Color.Cyan, Color.Magenta, Color.Yellow, Color.Purple, Color.Lime, Color.Orange, Color.Violet };
        int nPreparedColor = 0;


        SCHEDULE_WEEK_STRUCT[] scheduleWeek;
        SCHEDULE_WEEK_STRUCT[] scheduleWeek_temp = new SCHEDULE_WEEK_STRUCT[9];
        ArrayList blockScheduleFixed;

        ArrayList blockTemp; //For FixedSchedule

        ArrayList blockModelTemp; // To Load ModelItem for FixedSchedule

        public ArrayList blockTemp_Additional; // For AdditionalSchedule

        ArrayList additional_arraySchedule = null;
        int additional_indexSchedule = -1;

        sbyte additional_type;
        sbyte additional_datetype;


        string additional_sYear = "매";
        sbyte additional_nMon;
        sbyte additional_nDay;
        sbyte additional_solar_lunar;
        sbyte additional_nWeek;
        sbyte additional_nWeekDay;

        //

        public ArrayList blockTemp_Holiday; // For Holiday

        public ArrayList blockTemp_Special; // For Special
        byte holidayspecial_mode = 99;
        sbyte holidayspecial_type;


        string holidayspecial_sYear = "매";
        sbyte holidayspecial_nMon;
        sbyte holidayspecial_nDay;
        sbyte holidayspecial_solar_lunar;
        sbyte holidayspecial_nWeek;
        sbyte holidayspecial_nWeekDay;

        ArrayList holidayspecial_arraySchedule = null;
        int holidayspecial_indexSchedule = -1;

        // Model
        bool bViewOnlySet = false;
        int image_index = 0;


        private void FormConfigScheduleDesigner_Load(object sender, EventArgs e)
        {
            //Fixed Schedule

            scheduleWeek = ScheduleLib.ScheduleLoadWeek();
            blockScheduleFixed = ScheduleLib.ScheduleLoadFixed();

            blockTemp = ScheduleLib.ScheduleLoadFixed();
            blockModelTemp = ScheduleLib.ModelLoad();

            FillComboBox2(this.comboBoxSun, scheduleWeek[0].title);
            FillComboBox2(this.comboBoxMon, scheduleWeek[1].title);
            FillComboBox2(this.comboBoxTue, scheduleWeek[2].title);
            FillComboBox2(this.comboBoxWed, scheduleWeek[3].title);
            FillComboBox2(this.comboBoxThu, scheduleWeek[4].title);
            FillComboBox2(this.comboBoxFri, scheduleWeek[5].title);
            FillComboBox2(this.comboBoxSat, scheduleWeek[6].title);
            FillComboBox2(this.comboBoxHoliday, scheduleWeek[7].title);
            FillComboBox2(this.comboBoxSpecial, scheduleWeek[8].title);

            for (int i = 0; i < 9; i++)
            {
                scheduleWeek_temp[i].title = string.Format("{0}", scheduleWeek[i].title);
            }



            FillListBox();
            if (m_list_FixedSchedule.Items.Count > 0)
                m_list_FixedSchedule.SelectedIndex = 0;
            FillListBoxModel();

            this.bInitial = true;
            this.bViewOnlySet = checkBoxModelSetView.Checked;

            // Fixed Schedule End

            DateTime t;
            string buf;
            int j;

            // Additional Schedule


            blockTemp_Additional = ScheduleLib.ScheduleLoadAdditional();

            SCHEDULE_ADDITIONAL additional = new SCHEDULE_ADDITIONAL();
            int l;

            for (l = 0; l < blockTemp_Additional.Count; l++)
            {
                additional = (SCHEDULE_ADDITIONAL)blockTemp_Additional[l];
                ListViewItem item = new ListViewItem("");
                item.SubItems.Add("");
                item.SubItems.Add("");

                m_list_ScheduleAdditional.Items.Add(item);

                ChangeItemScheduleAdditional(item, additional);
            }



            t = DateTimeServer.Now;

            if (Tools.IsLangKorean())
                this.comboBoxScheduleAdditionalYear.Items.Add("매");
            else if (Tools.IsLangJapanese())
                this.comboBoxScheduleAdditionalYear.Items.Add("每");
            else if (Tools.IsLangChinese())
                this.comboBoxScheduleAdditionalYear.Items.Add("按");
            else
                this.comboBoxScheduleAdditionalYear.Items.Add("Every");

            for (j = 0; j < 50; j++)
            {
                buf = String.Format("{0}", j + t.Year);
                this.comboBoxScheduleAdditionalYear.Items.Add(buf);
            }

            if (Tools.IsLangKorean())
                this.comboBoxScheduleAdditionalMonth.Items.Add("매");
            else if (Tools.IsLangJapanese())
                this.comboBoxScheduleAdditionalMonth.Items.Add("每");
            else if (Tools.IsLangChinese())
                this.comboBoxScheduleAdditionalMonth.Items.Add("按");
            else
                this.comboBoxScheduleAdditionalMonth.Items.Add("Every");

            for (j = 1; j <= 12; j++)
            {
                buf = String.Format("{0}", j);
                this.comboBoxScheduleAdditionalMonth.Items.Add(buf);
            }

            for (j = 1; j <= 31; j++)
            {
                buf = String.Format("{0}", j);
                this.comboBoxScheduleAdditionalDay.Items.Add(buf);
            }

            if (Tools.IsLangKorean())
                this.comboBoxScheduleAdditionalDay.Items.Add("말");
            else if (Tools.IsLangJapanese())
                this.comboBoxScheduleAdditionalDay.Items.Add("末");
            else if (Tools.IsLangChinese())
                this.comboBoxScheduleAdditionalDay.Items.Add("最后一");
            else
                this.comboBoxScheduleAdditionalDay.Items.Add("End");

            if (Tools.IsLangKorean())
            {
                this.comboBoxScheduleAdditionalWeek.Items.Add("매");
                this.comboBoxScheduleAdditionalWeek.Items.Add("첫째");
                this.comboBoxScheduleAdditionalWeek.Items.Add("둘째");
                this.comboBoxScheduleAdditionalWeek.Items.Add("셋째");
                this.comboBoxScheduleAdditionalWeek.Items.Add("넷째");
                this.comboBoxScheduleAdditionalWeek.Items.Add("다섯째");
                this.comboBoxScheduleAdditionalWeek.Items.Add("여섯째");
            }
            else if (Tools.IsLangChinese())
            {
                this.comboBoxScheduleAdditionalWeek.Items.Add("按");
                this.comboBoxScheduleAdditionalWeek.Items.Add("第一");
                this.comboBoxScheduleAdditionalWeek.Items.Add("第二");
                this.comboBoxScheduleAdditionalWeek.Items.Add("第三");
                this.comboBoxScheduleAdditionalWeek.Items.Add("第四");
                this.comboBoxScheduleAdditionalWeek.Items.Add("第五");
                this.comboBoxScheduleAdditionalWeek.Items.Add("第六");
            }
            else
            {
                this.comboBoxScheduleAdditionalWeek.Items.Add("Every");
                this.comboBoxScheduleAdditionalWeek.Items.Add("1st");
                this.comboBoxScheduleAdditionalWeek.Items.Add("2nd");
                this.comboBoxScheduleAdditionalWeek.Items.Add("3th");
                this.comboBoxScheduleAdditionalWeek.Items.Add("4th");
                this.comboBoxScheduleAdditionalWeek.Items.Add("5th");
                this.comboBoxScheduleAdditionalWeek.Items.Add("6th");
            }

            if (Tools.IsLangKorean())
            {
                this.comboBoxScheduleAdditionalWeekday.Items.Add("일");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("월");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("화");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("수");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("목");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("금");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("토");
            }
            else if (Tools.IsLangJapanese())
            {
                this.comboBoxScheduleAdditionalWeekday.Items.Add("日");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("月");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("火");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("水");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("木");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("金");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("土");
            }
            else if (Tools.IsLangChinese())
            {
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期天");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期一");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期二");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期三");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期四");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期五");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("星期六");
            }
            else
            {
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Sun");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Mon");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Tue");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Wed");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Thu");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Fri");
                this.comboBoxScheduleAdditionalWeekday.Items.Add("Sat");
            }

            FillModelBoxScheduleAdditional();




            // Additional Schedule End

            // Holiday/Special

            blockTemp_Holiday = (ArrayList)Tools.CopyObject(Holiday.arrayHoliday);
            blockTemp_Special = (ArrayList)Tools.CopyObject(Holiday.arraySpecialDay);



            t = DateTimeServer.Now;

            if (Tools.IsLangKorean())
                comboBoxHolidaySpecialYear.Items.Add("매");
            else if (Tools.IsLangJapanese())
                comboBoxHolidaySpecialYear.Items.Add("每");
            else if (Tools.IsLangChinese())
                comboBoxHolidaySpecialYear.Items.Add("按");
            else
                comboBoxHolidaySpecialYear.Items.Add("Every");

            for (j = 0; j < 50; j++)
            {
                buf = String.Format("{0}", j + t.Year);
                comboBoxHolidaySpecialYear.Items.Add(buf);
            }

            if (Tools.IsLangKorean())
                this.comboBoxHolidaySpecialMonth.Items.Add("매");
            else if (Tools.IsLangJapanese())
                this.comboBoxHolidaySpecialMonth.Items.Add("每");
            else if (Tools.IsLangChinese())
                this.comboBoxHolidaySpecialMonth.Items.Add("按");
            else
                this.comboBoxHolidaySpecialMonth.Items.Add("Every");

            for (j = 1; j <= 12; j++)
            {
                buf = String.Format("{0}", j);
                comboBoxHolidaySpecialMonth.Items.Add(buf);
            }

            for (j = 1; j <= 31; j++)
            {
                buf = String.Format("{0}", j);
                comboBoxHolidaySpecialDay.Items.Add(buf);
            }

            if (Tools.IsLangKorean())
                this.comboBoxHolidaySpecialDay.Items.Add("말");
            else if (Tools.IsLangJapanese())
                this.comboBoxHolidaySpecialDay.Items.Add("末");
            else if (Tools.IsLangChinese())
                this.comboBoxHolidaySpecialDay.Items.Add("最后一");
            else
                this.comboBoxHolidaySpecialDay.Items.Add("End");

            if (Tools.IsLangKorean())
            {
                this.comboBoxHolidaySpecialWeek.Items.Add("매");
                this.comboBoxHolidaySpecialWeek.Items.Add("첫째");
                this.comboBoxHolidaySpecialWeek.Items.Add("둘째");
                this.comboBoxHolidaySpecialWeek.Items.Add("셋째");
                this.comboBoxHolidaySpecialWeek.Items.Add("넷째");
                this.comboBoxHolidaySpecialWeek.Items.Add("다섯째");
                this.comboBoxHolidaySpecialWeek.Items.Add("여섯째");
            }
            else if (Tools.IsLangChinese())
            {
                this.comboBoxHolidaySpecialWeek.Items.Add("按");
                this.comboBoxHolidaySpecialWeek.Items.Add("第一");
                this.comboBoxHolidaySpecialWeek.Items.Add("第二");
                this.comboBoxHolidaySpecialWeek.Items.Add("第三");
                this.comboBoxHolidaySpecialWeek.Items.Add("第四");
                this.comboBoxHolidaySpecialWeek.Items.Add("第五");
                this.comboBoxHolidaySpecialWeek.Items.Add("第六");
            }
            else
            {
                this.comboBoxHolidaySpecialWeek.Items.Add("Every");
                this.comboBoxHolidaySpecialWeek.Items.Add("1st");
                this.comboBoxHolidaySpecialWeek.Items.Add("2nd");
                this.comboBoxHolidaySpecialWeek.Items.Add("3th");
                this.comboBoxHolidaySpecialWeek.Items.Add("4th");
                this.comboBoxHolidaySpecialWeek.Items.Add("5th");
                this.comboBoxHolidaySpecialWeek.Items.Add("6th");
            }

            if (Tools.IsLangKorean())
            {
                this.comboBoxHolidaySpecialWeekday.Items.Add("일");
                this.comboBoxHolidaySpecialWeekday.Items.Add("월");
                this.comboBoxHolidaySpecialWeekday.Items.Add("화");
                this.comboBoxHolidaySpecialWeekday.Items.Add("수");
                this.comboBoxHolidaySpecialWeekday.Items.Add("목");
                this.comboBoxHolidaySpecialWeekday.Items.Add("금");
                this.comboBoxHolidaySpecialWeekday.Items.Add("토");
            }
            else if (Tools.IsLangJapanese())
            {
                this.comboBoxHolidaySpecialWeekday.Items.Add("日");
                this.comboBoxHolidaySpecialWeekday.Items.Add("月");
                this.comboBoxHolidaySpecialWeekday.Items.Add("火");
                this.comboBoxHolidaySpecialWeekday.Items.Add("水");
                this.comboBoxHolidaySpecialWeekday.Items.Add("木");
                this.comboBoxHolidaySpecialWeekday.Items.Add("金");
                this.comboBoxHolidaySpecialWeekday.Items.Add("土");
            }
            else if (Tools.IsLangChinese())
            {
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期天");
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期一");
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期二");
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期三");
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期四");
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期五");
                this.comboBoxHolidaySpecialWeekday.Items.Add("星期六");
            }
            else
            {
                this.comboBoxHolidaySpecialWeekday.Items.Add("Sun");
                this.comboBoxHolidaySpecialWeekday.Items.Add("Mon");
                this.comboBoxHolidaySpecialWeekday.Items.Add("Tue");
                this.comboBoxHolidaySpecialWeekday.Items.Add("Wed");
                this.comboBoxHolidaySpecialWeekday.Items.Add("Thu");
                this.comboBoxHolidaySpecialWeekday.Items.Add("Fri");
                this.comboBoxHolidaySpecialWeekday.Items.Add("Sat");
            }


            //Holiday/Special End
            this.radioButtonHoliday.Checked = true;


            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCHEDULE_SETUP))
                this.buttonOK.Enabled = false;






        }

        void FillListBox()
        {

            m_list_FixedSchedule.Items.Clear();

            int l;
            string imsi;
            SCHEDULE_STRUCT sc;

            if (blockTemp.Count == 0)
            {
                groupBox3.Enabled = false;

            }
            else groupBox3.Enabled = true;

            for (l = 0; l < blockTemp.Count; l++)
            {
                sc = (SCHEDULE_STRUCT)blockTemp[l];
                imsi = String.Format("{0}", sc.title);
                m_list_FixedSchedule.Items.Add(imsi);
            }


        }

        void FillListBoxModel()
        {


            int retn = m_list_FixedSchedule.SelectedIndex;
            m_listModel.Items.Clear();

            if (retn == -1)
            {
                groupBox1.Enabled = false;
                return;
            }
            else groupBox1.Enabled = true;



            SCHEDULE_STRUCT sc;
            NAME_STRUCT name;
            //int l;

            bool check_flag = false;

            //sc = (SCHEDULE_STRUCT)blockTemp[retn];
            sc = (SCHEDULE_STRUCT)blockTemp[retn];

            //for (l = 0; l < sc.blockName.Count; l++)
            //{
            //    name = (NAME_STRUCT)sc.blockName[l];
            //    m_listModel.Items.Add(name.title);

            //    image_index = GetImageIndex(tp.tag, check_flag);

            //    ListViewItem item = listViewTag.Items.Add(tp.tag, image_index);
            //    item.SubItems.Add(tp.description);
            //    item.SubItems.Add(check_flag ? "1" : "0");
            //}

            SCHEDULE_MODEL_STRUCT model;
            int j;
            //ListViewItem item;

            blockModelTemp = ScheduleLib.ModelLoad();

            for (j = 0; j < blockModelTemp.Count; j++)
            {
                model = (SCHEDULE_MODEL_STRUCT)blockModelTemp[j];

                if (sc.blockName.Count > 0)
                {
                    for (int k = 0; k < sc.blockName.Count; k++)
                    {
                        name = (NAME_STRUCT)sc.blockName[k];
                        if (name.title == model.title)
                        {
                            check_flag = true;
                            break;
                        }
                        else check_flag = false;
                    }
                }

                if (check_flag) image_index = 1;
                else image_index = 0;

                if (bViewOnlySet && !check_flag) continue;

                ListViewItem item_model = m_listModel.Items.Add(model.title, image_index);
                item_model.SubItems.Add(model.description);
                item_model.SubItems.Add(check_flag ? "1" : "0");

            }


        }

        // 기존 고정 모델 추가시 사용 . 
        void FillComboBox(ComboBox combo, string text)
        {
            int l;
            SCHEDULE_STRUCT sc;

            if (blockScheduleFixed.Count > 0)
            {
                combo.Items.Add("");
            }

            for (l = 0; l < blockScheduleFixed.Count; l++)
            {
                sc = (SCHEDULE_STRUCT)blockScheduleFixed[l];
                combo.Items.Add(sc.title);
                if (sc.title == text)
                {
                    combo.SelectedIndex = combo.Items.Count - 1;
                }
            }
        }
        // 새로 임시 수정 중일 떄 쓰기 위하여 추가
        void ChangeComBox2(int pos)
        {
            SCHEDULE_STRUCT sc;
            sc = (SCHEDULE_STRUCT)blockTemp[pos];

            scheduleWeek_temp[0].title = this.comboBoxSun.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxSun.SelectedItem != null ? this.comboBoxSun.SelectedItem.ToString() : "");
            scheduleWeek_temp[1].title = this.comboBoxMon.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxMon.SelectedItem != null ? this.comboBoxMon.SelectedItem.ToString() : "");
            scheduleWeek_temp[2].title = this.comboBoxTue.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxTue.SelectedItem != null ? this.comboBoxTue.SelectedItem.ToString() : "");
            scheduleWeek_temp[3].title = this.comboBoxWed.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxWed.SelectedItem != null ? this.comboBoxWed.SelectedItem.ToString() : "");
            scheduleWeek_temp[4].title = this.comboBoxThu.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxThu.SelectedItem != null ? this.comboBoxThu.SelectedItem.ToString() : "");
            scheduleWeek_temp[5].title = this.comboBoxFri.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxFri.SelectedItem != null ? this.comboBoxFri.SelectedItem.ToString() : "");
            scheduleWeek_temp[6].title = this.comboBoxSat.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxSat.SelectedItem != null ? this.comboBoxSat.SelectedItem.ToString() : "");
            scheduleWeek_temp[7].title = this.comboBoxHoliday.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxHoliday.SelectedItem != null ? this.comboBoxHoliday.SelectedItem.ToString() : "");
            scheduleWeek_temp[8].title = this.comboBoxSpecial.SelectedIndex == pos + 1 ? sc.title : (this.comboBoxSpecial.SelectedItem != null ? this.comboBoxSpecial.SelectedItem.ToString() : "");

            FillComboBox2(this.comboBoxSun, scheduleWeek_temp[0].title);
            FillComboBox2(this.comboBoxMon, scheduleWeek_temp[1].title);
            FillComboBox2(this.comboBoxTue, scheduleWeek_temp[2].title);
            FillComboBox2(this.comboBoxWed, scheduleWeek_temp[3].title);
            FillComboBox2(this.comboBoxThu, scheduleWeek_temp[4].title);
            FillComboBox2(this.comboBoxFri, scheduleWeek_temp[5].title);
            FillComboBox2(this.comboBoxSat, scheduleWeek_temp[6].title);
            FillComboBox2(this.comboBoxHoliday, scheduleWeek_temp[7].title);
            FillComboBox2(this.comboBoxSpecial, scheduleWeek_temp[8].title);
        }

        void FillComboBox2()
        {

            scheduleWeek_temp[0].title = this.comboBoxSun.SelectedItem != null ? this.comboBoxSun.SelectedItem.ToString() : "";
            scheduleWeek_temp[1].title = this.comboBoxMon.SelectedItem != null ? this.comboBoxMon.SelectedItem.ToString() : "";
            scheduleWeek_temp[2].title = this.comboBoxTue.SelectedItem != null ? this.comboBoxTue.SelectedItem.ToString() : "";
            scheduleWeek_temp[3].title = this.comboBoxWed.SelectedItem != null ? this.comboBoxWed.SelectedItem.ToString() : "";
            scheduleWeek_temp[4].title = this.comboBoxThu.SelectedItem != null ? this.comboBoxThu.SelectedItem.ToString() : "";
            scheduleWeek_temp[5].title = this.comboBoxFri.SelectedItem != null ? this.comboBoxFri.SelectedItem.ToString() : "";
            scheduleWeek_temp[6].title = this.comboBoxSat.SelectedItem != null ? this.comboBoxSat.SelectedItem.ToString() : "";
            scheduleWeek_temp[7].title = this.comboBoxHoliday.SelectedItem != null ? this.comboBoxHoliday.SelectedItem.ToString() : "";
            scheduleWeek_temp[8].title = this.comboBoxSpecial.SelectedItem != null ? this.comboBoxSpecial.SelectedItem.ToString() : "";

            FillComboBox2(this.comboBoxSun, scheduleWeek_temp[0].title);
            FillComboBox2(this.comboBoxMon, scheduleWeek_temp[1].title);
            FillComboBox2(this.comboBoxTue, scheduleWeek_temp[2].title);
            FillComboBox2(this.comboBoxWed, scheduleWeek_temp[3].title);
            FillComboBox2(this.comboBoxThu, scheduleWeek_temp[4].title);
            FillComboBox2(this.comboBoxFri, scheduleWeek_temp[5].title);
            FillComboBox2(this.comboBoxSat, scheduleWeek_temp[6].title);
            FillComboBox2(this.comboBoxHoliday, scheduleWeek_temp[7].title);
            FillComboBox2(this.comboBoxSpecial, scheduleWeek_temp[8].title);
        }

        void FillComboBox2(ComboBox combo, string text)
        {
            int l;
            SCHEDULE_STRUCT sc;
            combo.Items.Clear();

            if (blockTemp.Count > 0)
            {
                combo.Items.Add("");
            }

            for (l = 0; l < blockTemp.Count; l++)
            {
                sc = (SCHEDULE_STRUCT)blockTemp[l];
                combo.Items.Add(sc.title);
                if (sc.title == text)
                {
                    combo.SelectedIndex = combo.Items.Count - 1;
                }
            }
        }



        private void buttonOK_Click(object sender, EventArgs e)
        {
            scheduleWeek[0].title = this.comboBoxSun.Text;
            scheduleWeek[1].title = this.comboBoxMon.Text;
            scheduleWeek[2].title = this.comboBoxTue.Text;
            scheduleWeek[3].title = this.comboBoxWed.Text;
            scheduleWeek[4].title = this.comboBoxThu.Text;
            scheduleWeek[5].title = this.comboBoxFri.Text;
            scheduleWeek[6].title = this.comboBoxSat.Text;
            scheduleWeek[7].title = this.comboBoxHoliday.Text;
            scheduleWeek[8].title = this.comboBoxSpecial.Text;

            ScheduleSaveWeek();
            ScheduleSaveFix();
            ScheduleSaveAdditional();
            ScheduleSaveHolidaySpecial();

            //FindFixedScheduleAtWeek();
            //FormSchedule.OnScheduleStructChanged();


            DialogResult = DialogResult.OK;
            Close();


        }

        void ScheduleSaveWeek()
        {
            string filename;
            TextWriter writer;
            int i;

            filename = String.Format("{0}\\SCHEDULE", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(filename);
            filename = String.Format("{0}\\SCHEDULE\\WEEK.LSTX", TotalConfig.sDirWorkProject);
            writer = new StreamWriter(filename);
            if (writer == null) return;

            for (i = 0; i < ScheduleLib.MAX_SCHEDULE_WEEK; i++)
            {
                writer.Write("{0},", i);
                writer.Write("{0},", scheduleWeek[i].title);
                writer.WriteLine();
            }
            writer.Close();
        }

        void ScheduleSaveFix()
        {
            ListToBlock();
            ScheduleLib.ScheduleSaveFixed(blockTemp);
        }

        void ScheduleSaveAdditional()
        {
            ScheduleLib.ScheduleSaveAdditional(blockTemp_Additional);
        }

        void ScheduleSaveHolidaySpecial()
        {
            ArrayList block = new ArrayList();

            block = (ArrayList)Tools.CopyObject(blockTemp_Holiday);
            Holiday.arrayHoliday = (ArrayList)Tools.CopyObject(block);
            HolidayFile.SaveHolidayList(Holiday.arrayHoliday);

            block = (ArrayList)Tools.CopyObject(blockTemp_Special);
            Holiday.arraySpecialDay = (ArrayList)Tools.CopyObject(block);
            HolidayFile.SaveSpecialDayList(Holiday.arraySpecialDay);
        }

        private void buttonScheduleAdd_Click(object sender, EventArgs e)
        {

            SCHEDULE_STRUCT sc;
            int l;
            string ScheduleTitle_temp = "";
            string ScheduleTitle_buf = "";

            if (Tools.IsLangKorean())
                ScheduleTitle_temp = "스케쥴";

            else
                ScheduleTitle_temp = "Schedule";

            int pos = blockTemp.Count;

            int count = 1;




            ScheduleTitle_buf = ScheduleTitle_temp + count.ToString();



            for (l = 0; l < blockTemp.Count; l++)
            {
                sc = (SCHEDULE_STRUCT)blockTemp[l];
                if (String.Compare(sc.title, ScheduleTitle_buf, true) == 0)
                {
                    count++;
                    ScheduleTitle_buf = ScheduleTitle_temp + count.ToString();
                    l = -1;
                }
            }



            sc = new SCHEDULE_STRUCT();
            sc.title = ScheduleTitle_buf;
            sc.color = preparedColor[nPreparedColor];
            nPreparedColor++;
            nPreparedColor %= preparedColor.Length;

            sc.blockName = new ArrayList();
            blockTemp.Add(sc);
            FillListBox();
            m_list_FixedSchedule.SelectedIndex = blockTemp.Count - 1;

            FillComboBox2();

        }

        private void m_list_FixedSchedule_DrawItem(object sender, DrawItemEventArgs e)
        {

            if (e.Index == -1) return;

            SCHEDULE_STRUCT sc;
            Color color;
            Color tcolor;
            Color bcolor;

            if (m_list_FixedSchedule.SelectedIndex == e.Index)
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
            DrawClass.gcls(e.Graphics, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom - 1, color);
            DrawClass.PushBox2(e.Graphics, e.Bounds.Left + 1, e.Bounds.Top + 1, e.Bounds.Left + 10, e.Bounds.Bottom - 2, sc.color);

            SafeException.SafeDrawString(e.Graphics, sc.title, this.m_list_FixedSchedule.Font, new SolidBrush(tcolor), e.Bounds.Left + 20, e.Bounds.Top);

        }

        private void m_list_FixedSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.bInitial) return;	// Form Loading 중이다.

            ListToBlock();
            m_list_FixedSchedule.Invalidate();
            FillListBoxModel();
            nOldListPos = m_list_FixedSchedule.SelectedIndex;
        }

        void ListToBlock()
        {
            int retn = m_list_FixedSchedule.SelectedIndex;

            if (retn == -1) return;

            if (nOldListPos >= (int)blockTemp.Count) return;

            SCHEDULE_STRUCT sc;
            NAME_STRUCT name;
            int l;

            if (!bDeleteFixed)
            {
                sc = (SCHEDULE_STRUCT)blockTemp[nOldListPos];
                sc.blockName.Clear();

                for (l = 0; l < (int)m_listModel.Items.Count; l++)
                {
                    name = new NAME_STRUCT();
                    // add when only set is 1
                    if (m_listModel.Items[l].SubItems[2].Text == "1")
                    {
                        name.title = (string)m_listModel.Items[l].Text;
                        sc.blockName.Add(name);
                    }
                }
            }
        }




        int GetImageIndex(string tag, bool flag)
        {


            int image_index = 0;

            if (flag) image_index++;

            return image_index;
        }

        private void checkBoxModelSetView_CheckedChanged(object sender, EventArgs e)
        {
            this.bViewOnlySet = checkBoxModelSetView.Checked;
            ListToBlock();
            m_list_FixedSchedule.Invalidate();
            FillListBoxModel();
        }

        private void buttonModelSetOn_Click(object sender, EventArgs e)
        {
            ListViewItem item;

            for (int i = 0; i < m_listModel.SelectedItems.Count; i++)
            {
                item = m_listModel.SelectedItems[i];
                item.SubItems[2].Text = "1";
                item.ImageIndex = 1;
            }
        }

        private void buttonModelSetOff_Click(object sender, EventArgs e)
        {
            ListViewItem item;
            for (int i = 0; i < m_listModel.SelectedItems.Count; i++)
            {
                item = m_listModel.SelectedItems[i];
                item.SubItems[2].Text = "0";
                item.ImageIndex = 0;
            }
        }

        private void buttonScheduleDelete_Click(object sender, EventArgs e)
        {

            int retn = m_list_FixedSchedule.SelectedIndex;
            SCHEDULE_STRUCT sc;

            if (retn == -1)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("삭제하고 싶은 항목을 선택한 후\n삭제할 수 있습니다.", "삭제 오류");
                }
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要删除的项。", "选择错误");
                else
                {
                    MessageBox.Show("Select item to delete.", "Delete error");
                }
                return;
            }

            sc = (SCHEDULE_STRUCT)blockTemp[retn];

            if (Tools.IsLangKorean())
            {
                if (MessageBox.Show("항목을 삭제할까요?", sc.title, MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }
            else
            {
                if (MessageBox.Show("Delete this item?", sc.title, MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }


            bDeleteFixed = true;
            //blockTemp.RemoveAt(retn);
            blockTemp.RemoveAt(retn);


            FillListBox();


            if (m_list_FixedSchedule.Items.Count > 0)
            {
                if (retn < m_list_FixedSchedule.Items.Count)
                {
                    m_list_FixedSchedule.ClearSelected();
                    m_list_FixedSchedule.SelectedIndex = retn;
                }
                else
                {
                    m_list_FixedSchedule.SelectedIndex = m_list_FixedSchedule.Items.Count - 1;
                }

            }


            FillListBoxModel();
            FillComboBox2();

            bDeleteFixed = false;
        }

        private void buttonScheduleModify_Click(object sender, EventArgs e)
        {
            Modify();
        }

        void Modify()
        {
            int retn = m_list_FixedSchedule.SelectedIndex;

            SCHEDULE_STRUCT sc;

            if (retn == -1)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("수정하고 싶은 항목을 선택한 후\n수정할 수 있습니다.", "수정 오류");
                }
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要修改的项。", "选择错误");
                else if (Tools.IsLangVietnamese())
                    MessageBox.Show("Lựa chọn mục sửa đổi.", "Lỗi lựa chọn");
                else
                {
                    MessageBox.Show("Select item to modify.", "Selection error");
                }
                return;
            }

            //sc = (SCHEDULE_STRUCT)blockTemp[retn];
            sc = (SCHEDULE_STRUCT)blockTemp[retn];

            FormConfigScheduleAdd dialog = new FormConfigScheduleAdd();

            if (Tools.IsLangKorean())
                dialog.Text = "고정 스케쥴 모델 수정";
            else if (Tools.IsLangJapanese())
                dialog.Text = "固定スケジュールのモデル修正";
            else if (Tools.IsLangChinese())
                dialog.Text = "修改固定计划表模型";
            else
                dialog.Text = "Modify Fixed Model Schedule";

            dialog.textBoxTitle.Text = sc.title;
            dialog.buttonColor.BackColor = sc.color;

            dialog.Set(blockTemp, retn);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                sc.title = dialog.textBoxTitle.Text;
                sc.color = dialog.buttonColor.BackColor;
                m_list_FixedSchedule.Invalidate();
                //ChangeComBox2(retn);
                ChangeComBox2(retn);

            }
        }



        void FillListBoxHolidaySpecial(byte mode)
        {
            m_list_HolidaySpecial.Items.Clear();

            int l;
            if (mode == 0)
            {
                HOLIDAY_LIST holiday;


                for (l = 0; l < blockTemp_Holiday.Count; l++)
                {
                    holiday = (HOLIDAY_LIST)blockTemp_Holiday[l];
                    ListViewItem item = new ListViewItem("");
                    item.SubItems.Add("");


                    m_list_HolidaySpecial.Items.Add(item);
                    ChangeOneItemHolidaySpecial(item, holiday);

                }

            }
            else if (mode == 1)
            {
                HOLIDAY_LIST special;

                for (l = 0; l < blockTemp_Special.Count; l++)
                {
                    special = (HOLIDAY_LIST)blockTemp_Special[l];
                    ListViewItem item = new ListViewItem("");
                    item.SubItems.Add("");


                    m_list_HolidaySpecial.Items.Add(item);
                    ChangeOneItemHolidaySpecial(item, special);

                }

            }
            else return;
        }

        void ChangeOneItemHolidaySpecial(ListViewItem item, HOLIDAY_LIST lst)
        {
            string buf;

            Holiday.MakeString(out buf, lst);
            item.SubItems[0].Text = buf;
            item.SubItems[1].Text = lst.title;
        }

        string sEvery;

        private void ConfigHolidayModify(string dlg_title, HOLIDAY_LIST holiday, ArrayList array, int index)
        {
            string buf;



            groupBoxHolidaySpecialDate.Text = dlg_title;
            textBoxHolidaySpecialTitle.Text = holiday.title;
            holidayspecial_type = holiday.type;


            //string sLast;

            if (Tools.IsLangKorean())
            {
                sEvery = "매";
            }
            else if (Tools.IsLangJapanese())
            {
                sEvery = "每";
            }
            else if (Tools.IsLangChinese())
            {
                sEvery = "按";
            }
            else
            {
                sEvery = "Every";
            }

            if (holiday.year == 0)
            {
                holidayspecial_sYear = sEvery;
            }
            else
            {
                buf = String.Format("{0}", holiday.year);
                holidayspecial_sYear = buf;
            }

            holidayspecial_nMon = holiday.month;
            if (holiday.day == 32)
                holidayspecial_nDay = 31;
            else
                holidayspecial_nDay = (sbyte)(holiday.day - 1);
            holidayspecial_solar_lunar = holiday.bSunOrMoon;
            holidayspecial_nWeek = holiday.week;
            holidayspecial_nWeekDay = holiday.weekday;

            Set_HolidaySpecial(array, index);


            //if (dialog.ShowDialog(owner) == DialogResult.OK)
            //{
            //    holiday.title = dialog.textBoxTitle.Text;

            //    holiday.type = dialog.m_type;

            //    if (dialog.m_sYear == sEvery)
            //        holiday.year = 0;
            //    else
            //        holiday.year = ConvertTool.ToInt16(dialog.m_sYear);

            //    holiday.month = dialog.m_nMon;

            //    if (dialog.m_nDay >= 0 && dialog.m_nDay <= 30)
            //        holiday.day = (sbyte)(dialog.m_nDay + 1);
            //    else
            //        holiday.day = 32;

            //    holiday.bSunOrMoon = dialog.m_solar_lunar;

            //    holiday.week = dialog.m_nWeek;
            //    holiday.weekday = dialog.m_nWeekDay;


            //}


        }

        private void Set_HolidaySpecial(ArrayList array_schedule, int index)
        {
            holidayspecial_arraySchedule = array_schedule;
            holidayspecial_indexSchedule = index;
        }

        private void Load_HolidaySpecial()
        {


            this.radioButtonHolidaySpecialSolar.Checked = (holidayspecial_solar_lunar == 0);
            this.radioButtonHolidaySpecialLunar.Checked = (holidayspecial_solar_lunar == 1);


            this.radioButtonHolidaySpecialType0.Checked = (holidayspecial_type == 0);
            this.radioButtonHolidaySpecialType1.Checked = (holidayspecial_type == 1);

            //comboBoxHolidaySpecialYear.Items.Clear();
            //comboBoxHolidaySpecialMonth.Items.Clear();
            //comboBoxHolidaySpecialDay.Items.Clear();
            //comboBoxHolidaySpecialWeek.Items.Clear();
            //comboBoxHolidaySpecialWeekday.Items.Clear();

            EnableDisableHolidaySpecial();



            this.comboBoxHolidaySpecialYear.Text = holidayspecial_sYear;
            this.comboBoxHolidaySpecialMonth.SelectedIndex = holidayspecial_nMon;
            this.comboBoxHolidaySpecialDay.SelectedIndex = holidayspecial_nDay;
            this.comboBoxHolidaySpecialWeek.SelectedIndex = holidayspecial_nWeek;
            this.comboBoxHolidaySpecialWeekday.SelectedIndex = holidayspecial_nWeekDay;
        }

        void EnableDisableHolidaySpecial()
        {
            int type = GetRadioTypeHolidaySpecial();

            bool flag_day = false;
            bool flag_week = false;

            if (type == 1)
            {
                flag_week = true;
            }
            else
            {
                flag_day = true;
            }

            comboBoxHolidaySpecialDay.Enabled = flag_day;
            radioButtonHolidaySpecialLunar.Enabled = flag_day;
            radioButtonHolidaySpecialSolar.Enabled = flag_day;

            comboBoxHolidaySpecialWeek.Enabled = flag_week;
            comboBoxHolidaySpecialWeekday.Enabled = flag_week;
        }

        sbyte GetRadioTypeHolidaySpecial()
        {
            sbyte type;

            if (radioButtonHolidaySpecialType0.Checked) type = 0;
            else if (radioButtonHolidaySpecialType1.Checked) type = 1;
            else type = 2;

            return type;
        }

        private void m_list_HolidaySpecial_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (m_list.SelectedItems.Count == 0)
            //{
            //    if (Tools.IsLangKorean())
            //        MessageBox.Show("항목을 하나 선택한 후 수정할 수 있습니다.", "수정 오류");
            //    else if (Tools.IsLangChinese())
            //        MessageBox.Show("请选择要修改的项。", "选择错误");
            //    else
            //        MessageBox.Show("Select one item to modify.", "Selection error");

            //    return;
            //}

            if (this.m_list_HolidaySpecial.SelectedItems.Count > 0)
            {
                int i = this.m_list_HolidaySpecial.SelectedItems[0].Index;

                if (holidayspecial_mode == 0)
                {
                    HOLIDAY_LIST holiday;

                    holiday = (HOLIDAY_LIST)blockTemp_Holiday[i];

                    string title;
                    if (Tools.IsLangKorean()) title = "항목 수정";
                    else if (Tools.IsLangJapanese()) title = "項目修正";
                    else if (Tools.IsLangChinese()) title = "修改项";
                    else title = "Item Modity";

                    groupBoxHolidaySpecialDate.Enabled = true;
                    ConfigHolidayModify(title, holiday, blockTemp_Holiday, i);
                    Load_HolidaySpecial();
                }
                else if (holidayspecial_mode == 1)
                {
                    HOLIDAY_LIST special;

                    special = (HOLIDAY_LIST)blockTemp_Special[i];

                    string title;
                    if (Tools.IsLangKorean()) title = "항목 수정";
                    else if (Tools.IsLangJapanese()) title = "項目修正";
                    else if (Tools.IsLangChinese()) title = "修改项";
                    else title = "Item Modity";

                    groupBoxHolidaySpecialDate.Enabled = true;
                    ConfigHolidayModify(title, special, blockTemp_Special, i);
                    Load_HolidaySpecial();
                }

            }
            else
            {
                textBoxHolidaySpecialTitle.Text = "";
                groupBoxHolidaySpecialDate.Enabled = false;
                return;
            }

        }

        private void buttonHolidaySpecialApply_Click(object sender, EventArgs e)
        {
            if (TextBoxTool.CheckTextBoxLimitOver(this.textBoxHolidaySpecialTitle, 30)) return;

            if (this.radioButtonHolidaySpecialSolar.Checked) this.holidayspecial_solar_lunar = 0;
            else if (this.radioButtonHolidaySpecialLunar.Checked) this.holidayspecial_solar_lunar = 1;
            else this.holidayspecial_solar_lunar = 0;

            holidayspecial_type = GetRadioTypeHolidaySpecial();

            this.textBoxHolidaySpecialTitle.Text = this.textBoxHolidaySpecialTitle.Text.Trim(); // 제목 공백 방지를 위하여 추가. hsejong 25-03-11

            if (this.textBoxHolidaySpecialTitle.Text.Length == 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("제목을 입력해야 합니다.", "입력오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请输入标题。", "输入错误");
                else
                    MessageBox.Show("You must input the Title.", "Error");
                return;
            }
            holidayspecial_sYear = this.comboBoxHolidaySpecialYear.Text;
            holidayspecial_nMon = (sbyte)this.comboBoxHolidaySpecialMonth.SelectedIndex;
            holidayspecial_nDay = (sbyte)this.comboBoxHolidaySpecialDay.SelectedIndex;
            holidayspecial_nWeek = (sbyte)this.comboBoxHolidaySpecialWeek.SelectedIndex;
            holidayspecial_nWeekDay = (sbyte)this.comboBoxHolidaySpecialWeekday.SelectedIndex;

            if (holidayspecial_sYear.Length == 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("년을 입력해야 합니다.", "입력오류");
                else
                    MessageBox.Show("You must select the Year.", "Error");

                return;
            }
            if (holidayspecial_nMon == -1)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("월을 입력해야 합니다.", "입력오류");
                else
                    MessageBox.Show("You must select the Month.", "Error");
                return;
            }

            if (holidayspecial_type == 0)
            {
                if (holidayspecial_nDay == -1)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("일을 입력해야 합니다.", "입력오류");
                    else
                        MessageBox.Show("You must select the Day.", "Error");
                    return;
                }
            }
            else
            {
                if (holidayspecial_nWeek == -1)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("주을 입력해야 합니다.", "입력오류");
                    else
                        MessageBox.Show("You must select the Week.", "Error");
                    return;
                }
                if (holidayspecial_nWeekDay == -1)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("요일을 입력해야 합니다.", "입력오류");
                    else
                        MessageBox.Show("You must select the WeekDay.", "Error");
                    return;
                }
            }

            if (holidayspecial_arraySchedule != null)
            {
                HOLIDAY_LIST hl;

                for (int i = 0; i < holidayspecial_arraySchedule.Count; i++)
                {
                    if (i == holidayspecial_indexSchedule) continue;

                    hl = (HOLIDAY_LIST)holidayspecial_arraySchedule[i];

                    if (String.Compare(hl.title, this.textBoxHolidaySpecialTitle.Text, true) == 0)
                    {
                        if (Tools.IsLangKorean())
                            MessageBox.Show("같은 제목의 항목이 이미 추가되어 있습니다.", "제목 중복");
                        else
                            MessageBox.Show("Same title already exists.", "Title exists");

                        return;
                    }

                    // 같은 날짜가 존재하는 체크해도 좋을 듯 하다. 나중에 지원
                }
            }
            HOLIDAY_LIST holiday = new HOLIDAY_LIST();



            if (this.m_list_HolidaySpecial.SelectedItems.Count > 0)
            {
                int j = this.m_list_HolidaySpecial.SelectedItems[0].Index;

                if (holidayspecial_mode == 0)
                {
                    holiday = (HOLIDAY_LIST)blockTemp_Holiday[j];
                }
                else if (holidayspecial_mode == 1)
                {
                    holiday = (HOLIDAY_LIST)blockTemp_Special[j];
                }

                holiday.title = textBoxHolidaySpecialTitle.Text;

                holiday.type = holidayspecial_type;

                if (holidayspecial_sYear == sEvery)
                    holiday.year = 0;
                else
                    holiday.year = ConvertTool.ToInt16(holidayspecial_sYear);

                holiday.month = holidayspecial_nMon;

                if (holidayspecial_nDay >= 0 && holidayspecial_nDay <= 30)
                    holiday.day = (sbyte)(holidayspecial_nDay + 1);
                else
                    holiday.day = 32;

                holiday.bSunOrMoon = holidayspecial_solar_lunar;

                holiday.week = holidayspecial_nWeek;
                holiday.weekday = holidayspecial_nWeekDay;

                FillListBoxHolidaySpecial(holidayspecial_mode);
                this.m_list_HolidaySpecial.Items[j].Selected = true;
            }
            else return;




        }

        private void radioButtonHolidaySpecialType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableHolidaySpecial();
        }

        private void radioButtonHolidaySpecialType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableHolidaySpecial();
        }

        private void radioButtonHoliday_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonHoliday.Checked) holidayspecial_mode = 0;
            else if (this.radioButtonSpecial.Checked) holidayspecial_mode = 1;



            if (holidayspecial_mode == 0)
            {
                FillListBoxHolidaySpecial(holidayspecial_mode);
                groupBoxHolidaySpecialDate.Enabled = false;
                this.textBoxHolidaySpecialTitle.Text = "";
                Set_HolidaySpecial(blockTemp_Holiday, -1);
            }
            //else if (holidayspecial_mode == 1)
            //{
            //    Set_HolidaySpecial(blockTemp_Special, -1);
            //}
            else return;

        }

        private void radioButtonSpecial_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonHoliday.Checked) holidayspecial_mode = 0;
            else if (this.radioButtonSpecial.Checked) holidayspecial_mode = 1;




            //if (holidayspecial_mode == 0)
            //{
            //    Set_HolidaySpecial(blockTemp_Holiday, -1);
            //}
            if (holidayspecial_mode == 1)
            {
                FillListBoxHolidaySpecial(holidayspecial_mode);
                groupBoxHolidaySpecialDate.Enabled = false;
                this.textBoxHolidaySpecialTitle.Text = "";
                Set_HolidaySpecial(blockTemp_Special, -1);
            }
            else return;



        }

        private void buttonDeleteHolidaySpecial_Click(object sender, EventArgs e)
        {

            if (m_list_HolidaySpecial.SelectedItems.Count == 0)
            {

                if (Tools.IsLangKorean())
                    MessageBox.Show("항목을 하나 선택한 후 삭제할 수 있습니다.", "선택 오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要删除的项。", "选择错误");
                else
                    MessageBox.Show("Select one item to delete.", "Selection error");

                return;
            }

            int i = this.m_list_HolidaySpecial.SelectedItems[0].Index;
            m_list_HolidaySpecial.Items.RemoveAt(i);
            if (holidayspecial_mode == 0)
            {

                blockTemp_Holiday.RemoveAt(i);
            }
            else if (holidayspecial_mode == 1)
            {
                blockTemp_Special.RemoveAt(i);
            }

            //FillListBoxHolidaySpecial(holidayspecial_mode);
        }

        private void buttonAddHolidaySpecial_Click(object sender, EventArgs e)
        {
            HOLIDAY_LIST holiday = new HOLIDAY_LIST();

            HOLIDAY_LIST sc;
            int l;
            string HolidayTitle_temp = "";
            string HolidayTitle_buf = "";
            if (holidayspecial_mode == 0)
            {
                if (Tools.IsLangKorean())
                    HolidayTitle_temp = "공휴일";

                else
                    HolidayTitle_temp = "Holiday";
            }
            else if (holidayspecial_mode == 1)
            {
                if (Tools.IsLangKorean())
                    HolidayTitle_temp = "특정일";

                else
                    HolidayTitle_temp = "Special";
            }
            else return;

            //int pos = blockTemp.Count;

            int count = 1;




            HolidayTitle_buf = HolidayTitle_temp + count.ToString();



            for (l = 0; l < holidayspecial_arraySchedule.Count; l++)
            {

                sc = (HOLIDAY_LIST)holidayspecial_arraySchedule[l];
                if (String.Compare(sc.title, HolidayTitle_buf, true) == 0)
                {
                    count++;
                    HolidayTitle_buf = HolidayTitle_temp + count.ToString();
                    l = -1;
                }
            }



            holiday.title = HolidayTitle_buf;

            holiday.type = 0;


            holiday.year = 0;


            holiday.month = 0;

            holiday.day = 1;

            holiday.bSunOrMoon = 0;

            holiday.week = 0;
            holiday.weekday = 0;

            if (holidayspecial_mode == 0)
            {
                blockTemp_Holiday.Add(holiday);
            }
            else if (holidayspecial_mode == 1)
            {
                blockTemp_Special.Add(holiday);
            }
            else return;

            ListViewItem item = new ListViewItem("");
            item.SubItems.Add("");
            m_list_HolidaySpecial.Items.Add(item);
            ChangeOneItemHolidaySpecial(item, holiday);
            m_list_HolidaySpecial.Items[m_list_HolidaySpecial.Items.Count - 1].Selected = true;
            m_list_HolidaySpecial.EnsureVisible(m_list_HolidaySpecial.Items.Count - 1);


        }

        // ScheduleAdditional
        private void Set_Additional(ArrayList array_schedule, int index)
        {
            additional_arraySchedule = array_schedule;
            additional_indexSchedule = index;
        }

        private void ChangeItemScheduleAdditional(ListViewItem item, SCHEDULE_ADDITIONAL add)
        {

            string buf;

            ScheduleLib.MakeStringScheduleAdditional(out buf, add);

            item.SubItems[0].Text = buf;
            item.SubItems[1].Text = add.title;
            item.SubItems[2].Text = add.model;

            bool bExist = false;

            for (int j = 0; j < blockModelTemp.Count; j++)
            {
                SCHEDULE_MODEL_STRUCT model = (SCHEDULE_MODEL_STRUCT)blockModelTemp[j];
                if (model.title == add.model)
                {
                    bExist = true;
                    break;
                }

            }

            if (!bExist) item.ForeColor = Color.Red;
            else item.ForeColor = Color.Black;




        }

        private void m_list_ScheduleAdditional_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (m_list_ScheduleAdditional.SelectedItems.Count == 0)
            {
                this.textBoxScheduleAdditionalTitle.Text = "";
                this.textBoxScheduleAdditionalUserType.Text = "";
                this.comboBoxScheduleAdditionalModel.Text = "";
                this.groupBoxScheduleSAdditional.Enabled = false;
                this.groupBoxScheduleAdditionalConfigDate.Visible = false;
                return;
            }

            int i = m_list_ScheduleAdditional.SelectedItems[0].Index;

            SCHEDULE_ADDITIONAL additional = new SCHEDULE_ADDITIONAL();
            additional = (SCHEDULE_ADDITIONAL)Tools.CopyObject(blockTemp_Additional[i]);

            if (Tools.IsLangKorean())
            {
                groupBoxScheduleSAdditional.Text = "추가 스케쥴 수정";
            }
            else if (Tools.IsLangJapanese())
                groupBoxScheduleSAdditional.Text = "追加スケジュールの修正";
            else if (Tools.IsLangChinese())
                groupBoxScheduleSAdditional.Text = "修改添加计划表";
            else
                groupBoxScheduleSAdditional.Text = "Additional Schedule Modify";

            Set_Additional(blockTemp_Additional, i);

            this.radioButtonScheduleAdditionalType0.Checked = (additional.type == 0);
            this.radioButtonScheduleAdditionalType1.Checked = (additional.type == 1);
            this.radioButtonScheduleAdditionalType2.Checked = (additional.type == 2);
            this.radioButtonScheduleAdditionalType3.Checked = (additional.type == 3);
            this.radioButtonScheduleAdditionalType4.Checked = (additional.type == 4);



            string buf;
            Holiday.MakeString(out buf, additional.user);
            this.textBoxScheduleAdditionalUserType.Text = buf;
            this.textBoxScheduleAdditionalTitle.Text = additional.title;

            this.comboBoxScheduleAdditionalModel.Text = additional.model;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.radioButtonScheduleAdditionalType4.Visible = false;
            }
            this.groupBoxScheduleSAdditional.Enabled = true;

            ConfigAdditionalDate(additional.user, null, -1);
            Load_Additional();
            EnableDisableAdditional();

        }

        void FillModelBoxScheduleAdditional()
        {
            int l;
            SCHEDULE_MODEL_STRUCT model;

            this.comboBoxScheduleAdditionalModel.Items.Clear();

            for (l = 0; l < blockModelTemp.Count; l++)
            {
                model = (SCHEDULE_MODEL_STRUCT)blockModelTemp[l];
                this.comboBoxScheduleAdditionalModel.Items.Add(model.title);
            }
        }

        void EnableDisableAdditional()
        {
            int type = GetRadioTypeAdditional();

            bool flag = false;
            if (type == 2) flag = true;
            else flag = false;



            //this.textBoxUserType.Enabled = flag;
            this.groupBoxScheduleAdditionalConfigDate.Enabled = flag;
            this.groupBoxScheduleAdditionalConfigDate.Visible = flag;
            if (flag)
            {


                type = GetRadioTypeAdditionalDateType();

                bool flag_day = false;
                bool flag_week = false;

                if (type == 1)
                {
                    flag_week = true;
                }
                else
                {
                    flag_day = true;
                }



                this.comboBoxScheduleAdditionalDay.Enabled = flag_day;
                this.radioButtonScheduleAdditionalLunar.Enabled = flag_day;
                this.radioButtonScheduleAdditionalSolar.Enabled = flag_day;

                this.comboBoxScheduleAdditionalWeek.Enabled = flag_week;
                this.comboBoxScheduleAdditionalWeekday.Enabled = flag_week;

            }

        }

        private int GetRadioTypeAdditional()
        {

            int type;
            if (this.radioButtonScheduleAdditionalType0.Checked) type = 0;
            else if (this.radioButtonScheduleAdditionalType1.Checked) type = 1;
            else if (this.radioButtonScheduleAdditionalType2.Checked) type = 2;
            else if (this.radioButtonScheduleAdditionalType3.Checked) type = 3;
            else type = 0;

            return type;
        }

        private void radioButtonScheduleAdditionalType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        private void radioButtonScheduleAdditionalType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        private void radioButtonScheduleAdditionalType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        private void radioButtonScheduleAdditionalType3_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        private void radioButtonScheduleAdditionalType4_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        private void ConfigAdditionalDate(HOLIDAY_LIST holiday, ArrayList array, int index)
        {
            string buf;



            additional_type = holiday.type;


            //string sLast;

            if (Tools.IsLangKorean())
            {
                sEvery = "매";
            }
            else if (Tools.IsLangJapanese())
            {
                sEvery = "每";
            }
            else if (Tools.IsLangChinese())
            {
                sEvery = "按";
            }
            else
            {
                sEvery = "Every";
            }

            if (holiday.year == 0)
            {
                additional_sYear = sEvery;
            }
            else
            {
                buf = String.Format("{0}", holiday.year);
                additional_sYear = buf;
            }

            additional_nMon = holiday.month;
            if (holiday.day == 32)
                additional_nDay = 31;
            else
                additional_nDay = (sbyte)(holiday.day - 1);
            additional_solar_lunar = holiday.bSunOrMoon;
            additional_nWeek = holiday.week;
            additional_nWeekDay = holiday.weekday;

            Set_Additional(array, index);


            //if (dialog.ShowDialog(owner) == DialogResult.OK)
            //{
            //    holiday.title = dialog.textBoxTitle.Text;

            //    holiday.type = dialog.m_type;

            //    if (dialog.m_sYear == sEvery)
            //        holiday.year = 0;
            //    else
            //        holiday.year = ConvertTool.ToInt16(dialog.m_sYear);

            //    holiday.month = dialog.m_nMon;

            //    if (dialog.m_nDay >= 0 && dialog.m_nDay <= 30)
            //        holiday.day = (sbyte)(dialog.m_nDay + 1);
            //    else
            //        holiday.day = 32;

            //    holiday.bSunOrMoon = dialog.m_solar_lunar;

            //    holiday.week = dialog.m_nWeek;
            //    holiday.weekday = dialog.m_nWeekDay;


            //}


        }

        private void Load_Additional()
        {


            this.radioButtonScheduleAdditionalSolar.Checked = (additional_solar_lunar == 0);
            this.radioButtonScheduleAdditionalLunar.Checked = (additional_solar_lunar == 1);


            this.radioButtonScheduleAdditionalDateType0.Checked = (additional_type == 0);
            this.radioButtonScheduleAdditionalDateType1.Checked = (additional_type == 1);

            //comboBoxHolidaySpecialYear.Items.Clear();
            //comboBoxHolidaySpecialMonth.Items.Clear();
            //comboBoxHolidaySpecialDay.Items.Clear();
            //comboBoxHolidaySpecialWeek.Items.Clear();
            //comboBoxHolidaySpecialWeekday.Items.Clear();

            EnableDisableAdditional();



            this.comboBoxScheduleAdditionalYear.Text = additional_sYear;
            this.comboBoxScheduleAdditionalMonth.SelectedIndex = additional_nMon;
            this.comboBoxScheduleAdditionalDay.SelectedIndex = additional_nDay;
            this.comboBoxScheduleAdditionalWeek.SelectedIndex = additional_nWeek;
            this.comboBoxScheduleAdditionalWeekday.SelectedIndex = additional_nWeekDay;
        }

        private void buttonScheduleAdditionalApply_Click(object sender, EventArgs e)
        {
            if (m_list_ScheduleAdditional.SelectedItems.Count == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("항목을 하나 선택한 후 수정할 수 있습니다.", "수정 오류");
                }
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要修改的项。", "选择错误");
                else
                {
                    MessageBox.Show("Select one item to modify.", "Selection error");
                }
                return;
            }



            if (TextBoxTool.CheckTextBoxLimitOver(textBoxScheduleAdditionalTitle, TextBoxLimit.MAX_ScheduleTitle)) return;

            this.textBoxScheduleAdditionalTitle.Text = this.textBoxScheduleAdditionalTitle.Text.Trim(); // 제목 공백 방지를 위하여 추가. hsejong 25-03-11

            if (this.textBoxScheduleAdditionalTitle.Text.Length == 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("제목을 입력해야 합니다..", "입력 오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请输入标题。", "输入错误");
                else
                    MessageBox.Show("You must input the Title.", "Input Error");
                return;
            }

            int i = m_list_ScheduleAdditional.SelectedItems[0].Index;
            Set_Additional(blockTemp_Additional, i);

            SCHEDULE_ADDITIONAL additional = (SCHEDULE_ADDITIONAL)Tools.CopyObject(blockTemp_Additional[i]);

            additional.model = this.comboBoxScheduleAdditionalModel.Text;



            additional.title = this.textBoxScheduleAdditionalTitle.Text;

            if (additional.model.Length == 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("모델을 선택해야 합니다.", "모델선택 오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选1个模型。", "选择错误");
                else
                    MessageBox.Show("You must select the Model.", "Selection Error");
                return;
            }

            SCHEDULE_ADDITIONAL sa;

            for (int j = 0; j < additional_arraySchedule.Count; j++)
            {
                if (j == additional_indexSchedule) continue;

                sa = (SCHEDULE_ADDITIONAL)additional_arraySchedule[j];
                if (String.Compare(sa.title, this.textBoxScheduleAdditionalTitle.Text, true) == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("같은 제목의 스케쥴이 이미 추가되어 있습니다.", "제목 중복");
                    else
                        MessageBox.Show("Same title already exists.", "Title exists");

                    return;
                }
            }

            additional.type = GetRadioTypeAdditional();


            string title;
            if (Tools.IsLangKorean()) title = "사용자 지정 스케쥴 수정";
            else if (Tools.IsLangChinese()) title = "修改自定义计划表";
            else title = "User Defined Schedule Modify";

            if (this.radioButtonScheduleAdditionalSolar.Checked) this.additional_solar_lunar = 0;
            else if (this.radioButtonScheduleAdditionalLunar.Checked) this.additional_solar_lunar = 1;
            else this.additional_solar_lunar = 0;


            additional_sYear = this.comboBoxScheduleAdditionalYear.Text;
            additional_nMon = (sbyte)this.comboBoxScheduleAdditionalMonth.SelectedIndex;
            additional_nDay = (sbyte)this.comboBoxScheduleAdditionalDay.SelectedIndex;
            additional_nWeek = (sbyte)this.comboBoxScheduleAdditionalWeek.SelectedIndex;
            additional_nWeekDay = (sbyte)this.comboBoxScheduleAdditionalWeekday.SelectedIndex;


            additional_datetype = GetRadioTypeAdditionalDateType();


            additional.user.title = title;


            additional.user.type = additional_datetype;

            if (additional_sYear == sEvery)
                additional.user.year = 0;
            else
                additional.user.year = ConvertTool.ToInt16(additional_sYear);

            additional.user.month = additional_nMon;

            if (additional_nDay >= 0 && additional_nDay <= 30)
                additional.user.day = (sbyte)(additional_nDay + 1);
            else
                additional.user.day = 32;

            additional.user.bSunOrMoon = additional_solar_lunar;

            additional.user.week = additional_nWeek;
            additional.user.weekday = additional_nWeekDay;
            additional.user.bSunOrMoon = additional_solar_lunar;



            Set_Additional(blockTemp_Additional, i);


            blockTemp_Additional[i] = (SCHEDULE_ADDITIONAL)Tools.CopyObject(additional);
            ChangeItemScheduleAdditional(m_list_ScheduleAdditional.Items[i], additional);

        }

        private void radioButtonScheduleAdditionalDateType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        private void radioButtonScheduleAdditionalDateType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAdditional();
        }

        sbyte GetRadioTypeAdditionalDateType()
        {
            sbyte type;

            if (this.radioButtonScheduleAdditionalDateType0.Checked) type = 0;
            else if (this.radioButtonScheduleAdditionalDateType1.Checked) type = 1;
            else type = 2;

            return type;
        }

        private void buttonScheduleAdditionalAdd_Click(object sender, EventArgs e)
        {
            Set_Additional(blockTemp_Additional, -1);

            SCHEDULE_ADDITIONAL additional = new SCHEDULE_ADDITIONAL();

            int l;
            string AdditionalTitle_temp = "";
            string AdditionalTitle_buf = "";

            if (Tools.IsLangKorean())
                AdditionalTitle_temp = "추가스케쥴";

            else
                AdditionalTitle_temp = "AdditionalSchedule";



            int count = 1;




            AdditionalTitle_buf = AdditionalTitle_temp + count.ToString();



            for (l = 0; l < additional_arraySchedule.Count; l++)
            {

                SCHEDULE_ADDITIONAL sc = (SCHEDULE_ADDITIONAL)additional_arraySchedule[l];
                if (String.Compare(sc.title, AdditionalTitle_buf, true) == 0)
                {
                    count++;
                    AdditionalTitle_buf = AdditionalTitle_temp + count.ToString();
                    l = -1;
                }
            }

            additional.title = AdditionalTitle_buf;

            string buf;
            if (Tools.IsLangKorean())
                buf = "모델을 선택하세요!";

            else
                buf = "Select a model item !";

            additional.model = buf;
            additional.type = 0;


            blockTemp_Additional.Add(additional);
            ListViewItem item = new ListViewItem("");
            item.SubItems.Add("");
            item.SubItems.Add("");
            m_list_ScheduleAdditional.Items.Add(item);
            ChangeItemScheduleAdditional(item, additional);
            m_list_ScheduleAdditional.Items[m_list_ScheduleAdditional.Items.Count - 1].Selected = true;
            m_list_ScheduleAdditional.EnsureVisible(m_list_ScheduleAdditional.Items.Count - 1);
        }

        private void buttonScheduleaAdditionalDelete_Click(object sender, EventArgs e)
        {

            if (m_list_ScheduleAdditional.SelectedItems.Count == 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("항목을 하나 선택한 후 삭제할 수 있습니다.", "선택 오류");
                }
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要删除的项。", "选择错误");
                else
                {
                    MessageBox.Show("Select one item to delete.", "Selection error");
                }
                return;
            }

            int i = m_list_ScheduleAdditional.SelectedItems[0].Index;

            m_list_ScheduleAdditional.Items.RemoveAt(i);
            blockTemp_Additional.RemoveAt(i);
        }

        private void buttonModelEdit_Click(object sender, EventArgs e)
        {

            FormConfigScheduleDesignerModelEditor dialog = new FormConfigScheduleDesignerModelEditor();



            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                blockModelTemp = ScheduleLib.ModelLoad();
                FillListBoxModel();
                FillModelBoxScheduleAdditional();
            }


        }

        private void m_list_FixedSchedule_DoubleClick(object sender, EventArgs e)
        {
            Modify();
        }

        

      

        

		

	}
}

