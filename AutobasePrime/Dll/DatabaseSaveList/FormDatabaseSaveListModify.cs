using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DatabaseConnection;
using AutoLib;
using AutoLibLocal;
using DialogTag;
using NetTools;

namespace DatabaseSaveList
{
	/// <summary>
	/// Summary description for FormDatabaseSaveListModify.
	/// </summary>
	public class FormDatabaseSaveListModify : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxTable;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxDsn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxField;
		private System.Windows.Forms.NumericUpDown m_Day;
		private System.Windows.Forms.NumericUpDown m_Hour;
		private System.Windows.Forms.NumericUpDown m_Minute;
		private System.Windows.Forms.NumericUpDown m_Second;
		private System.Windows.Forms.NumericUpDown m_Millisecond;
		private System.ComponentModel.IContainer components;

//		DatabaseSaveListClass classSaveList;
		ConnectionStringList dsnList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox comboBoxDataType;
		SaveList listTemp;
		private System.Windows.Forms.CheckBox checkBoxDuplex;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBoxSaveMinute;
		private System.Windows.Forms.CheckBox checkBoxSaveHour;
		private System.Windows.Forms.CheckBox checkBoxSaveDay;
		private System.Windows.Forms.CheckBox checkBoxSaveMonth;
		private System.Windows.Forms.CheckBox checkBoxSaveYear;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBoxSaveMinute;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textBoxSaveHour;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBoxSaveDay;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textBoxSaveMonth;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textBoxSaveYear;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TextBox textBoxColumnMilli;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textBoxColumnDate;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonDateType0;
		private System.Windows.Forms.RadioButton radioButtonDateType1;
		private System.Windows.Forms.CheckBox checkBoxSaveAsNextTime;

		static int nTabPos;
        private CheckBox checkBoxEnable;
        private Button buttonRunTag;
        private TextBox textBoxRunTag;
        private CheckBox checkBoxUseRunTag;
		ArrayList tempMember;
                
		public FormDatabaseSaveListModify(SaveList list, ConnectionStringList dsn_list)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			//sharedRunMain = shared_run_main;
			listTemp = list;
			dsnList = dsn_list;
			tempMember = (ArrayList)NetTools.Tools.CopyObject(list.arrayMember);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDatabaseSaveListModify));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxSaveYear = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxSaveMonth = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxSaveDay = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxSaveHour = new System.Windows.Forms.TextBox();
            this.checkBoxSaveYear = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveMonth = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveDay = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveHour = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveMinute = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxSaveMinute = new System.Windows.Forms.TextBox();
            this.checkBoxDuplex = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveAsNextTime = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_Millisecond = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.m_Second = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.m_Minute = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.m_Hour = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.m_Day = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxDsn = new System.Windows.Forms.ComboBox();
            this.textBoxTable = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonDateType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDateType0 = new System.Windows.Forms.RadioButton();
            this.textBoxColumnMilli = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxColumnDate = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.comboBoxDataType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxField = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.buttonRunTag = new System.Windows.Forms.Button();
            this.textBoxRunTag = new System.Windows.Forms.TextBox();
            this.checkBoxUseRunTag = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_Millisecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Second)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Minute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Day)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = null;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.AccessibleDescription = null;
            this.tabPage1.AccessibleName = null;
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.BackgroundImage = null;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.checkBoxDuplex);
            this.tabPage1.Controls.Add(this.checkBoxSaveAsNextTime);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.comboBoxDsn);
            this.tabPage1.Controls.Add(this.textBoxTable);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.textBoxTitle);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.textBoxSaveYear);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.textBoxSaveMonth);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBoxSaveDay);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.textBoxSaveHour);
            this.groupBox2.Controls.Add(this.checkBoxSaveYear);
            this.groupBox2.Controls.Add(this.checkBoxSaveMonth);
            this.groupBox2.Controls.Add(this.checkBoxSaveDay);
            this.groupBox2.Controls.Add(this.checkBoxSaveHour);
            this.groupBox2.Controls.Add(this.checkBoxSaveMinute);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.textBoxSaveMinute);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label15
            // 
            this.label15.AccessibleDescription = null;
            this.label15.AccessibleName = null;
            resources.ApplyResources(this.label15, "label15");
            this.label15.Font = null;
            this.label15.Name = "label15";
            // 
            // textBoxSaveYear
            // 
            this.textBoxSaveYear.AccessibleDescription = null;
            this.textBoxSaveYear.AccessibleName = null;
            resources.ApplyResources(this.textBoxSaveYear, "textBoxSaveYear");
            this.textBoxSaveYear.BackgroundImage = null;
            this.textBoxSaveYear.Font = null;
            this.textBoxSaveYear.Name = "textBoxSaveYear";
            // 
            // label14
            // 
            this.label14.AccessibleDescription = null;
            this.label14.AccessibleName = null;
            resources.ApplyResources(this.label14, "label14");
            this.label14.Font = null;
            this.label14.Name = "label14";
            // 
            // textBoxSaveMonth
            // 
            this.textBoxSaveMonth.AccessibleDescription = null;
            this.textBoxSaveMonth.AccessibleName = null;
            resources.ApplyResources(this.textBoxSaveMonth, "textBoxSaveMonth");
            this.textBoxSaveMonth.BackgroundImage = null;
            this.textBoxSaveMonth.Font = null;
            this.textBoxSaveMonth.Name = "textBoxSaveMonth";
            // 
            // label13
            // 
            this.label13.AccessibleDescription = null;
            this.label13.AccessibleName = null;
            resources.ApplyResources(this.label13, "label13");
            this.label13.Font = null;
            this.label13.Name = "label13";
            // 
            // textBoxSaveDay
            // 
            this.textBoxSaveDay.AccessibleDescription = null;
            this.textBoxSaveDay.AccessibleName = null;
            resources.ApplyResources(this.textBoxSaveDay, "textBoxSaveDay");
            this.textBoxSaveDay.BackgroundImage = null;
            this.textBoxSaveDay.Font = null;
            this.textBoxSaveDay.Name = "textBoxSaveDay";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // textBoxSaveHour
            // 
            this.textBoxSaveHour.AccessibleDescription = null;
            this.textBoxSaveHour.AccessibleName = null;
            resources.ApplyResources(this.textBoxSaveHour, "textBoxSaveHour");
            this.textBoxSaveHour.BackgroundImage = null;
            this.textBoxSaveHour.Font = null;
            this.textBoxSaveHour.Name = "textBoxSaveHour";
            // 
            // checkBoxSaveYear
            // 
            this.checkBoxSaveYear.AccessibleDescription = null;
            this.checkBoxSaveYear.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveYear, "checkBoxSaveYear");
            this.checkBoxSaveYear.BackgroundImage = null;
            this.checkBoxSaveYear.Font = null;
            this.checkBoxSaveYear.Name = "checkBoxSaveYear";
            this.checkBoxSaveYear.CheckedChanged += new System.EventHandler(this.checkBoxSaveYear_CheckedChanged);
            // 
            // checkBoxSaveMonth
            // 
            this.checkBoxSaveMonth.AccessibleDescription = null;
            this.checkBoxSaveMonth.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveMonth, "checkBoxSaveMonth");
            this.checkBoxSaveMonth.BackgroundImage = null;
            this.checkBoxSaveMonth.Font = null;
            this.checkBoxSaveMonth.Name = "checkBoxSaveMonth";
            this.checkBoxSaveMonth.CheckedChanged += new System.EventHandler(this.checkBoxSaveMonth_CheckedChanged);
            // 
            // checkBoxSaveDay
            // 
            this.checkBoxSaveDay.AccessibleDescription = null;
            this.checkBoxSaveDay.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveDay, "checkBoxSaveDay");
            this.checkBoxSaveDay.BackgroundImage = null;
            this.checkBoxSaveDay.Font = null;
            this.checkBoxSaveDay.Name = "checkBoxSaveDay";
            this.checkBoxSaveDay.CheckedChanged += new System.EventHandler(this.checkBoxSaveDay_CheckedChanged);
            // 
            // checkBoxSaveHour
            // 
            this.checkBoxSaveHour.AccessibleDescription = null;
            this.checkBoxSaveHour.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveHour, "checkBoxSaveHour");
            this.checkBoxSaveHour.BackgroundImage = null;
            this.checkBoxSaveHour.Font = null;
            this.checkBoxSaveHour.Name = "checkBoxSaveHour";
            this.checkBoxSaveHour.CheckedChanged += new System.EventHandler(this.checkBoxSaveHour_CheckedChanged);
            // 
            // checkBoxSaveMinute
            // 
            this.checkBoxSaveMinute.AccessibleDescription = null;
            this.checkBoxSaveMinute.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveMinute, "checkBoxSaveMinute");
            this.checkBoxSaveMinute.BackgroundImage = null;
            this.checkBoxSaveMinute.Font = null;
            this.checkBoxSaveMinute.Name = "checkBoxSaveMinute";
            this.checkBoxSaveMinute.CheckedChanged += new System.EventHandler(this.checkBoxSaveMinute_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // textBoxSaveMinute
            // 
            this.textBoxSaveMinute.AccessibleDescription = null;
            this.textBoxSaveMinute.AccessibleName = null;
            resources.ApplyResources(this.textBoxSaveMinute, "textBoxSaveMinute");
            this.textBoxSaveMinute.BackgroundImage = null;
            this.textBoxSaveMinute.Font = null;
            this.textBoxSaveMinute.Name = "textBoxSaveMinute";
            // 
            // checkBoxDuplex
            // 
            this.checkBoxDuplex.AccessibleDescription = null;
            this.checkBoxDuplex.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDuplex, "checkBoxDuplex");
            this.checkBoxDuplex.BackgroundImage = null;
            this.checkBoxDuplex.Font = null;
            this.checkBoxDuplex.Name = "checkBoxDuplex";
            // 
            // checkBoxSaveAsNextTime
            // 
            this.checkBoxSaveAsNextTime.AccessibleDescription = null;
            this.checkBoxSaveAsNextTime.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveAsNextTime, "checkBoxSaveAsNextTime");
            this.checkBoxSaveAsNextTime.BackgroundImage = null;
            this.checkBoxSaveAsNextTime.Font = null;
            this.checkBoxSaveAsNextTime.Name = "checkBoxSaveAsNextTime";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.m_Millisecond);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.m_Second);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.m_Minute);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.m_Hour);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.m_Day);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // m_Millisecond
            // 
            this.m_Millisecond.AccessibleDescription = null;
            this.m_Millisecond.AccessibleName = null;
            resources.ApplyResources(this.m_Millisecond, "m_Millisecond");
            this.m_Millisecond.Font = null;
            this.m_Millisecond.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.m_Millisecond.Name = "m_Millisecond";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // m_Second
            // 
            this.m_Second.AccessibleDescription = null;
            this.m_Second.AccessibleName = null;
            resources.ApplyResources(this.m_Second, "m_Second");
            this.m_Second.Font = null;
            this.m_Second.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.m_Second.Name = "m_Second";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // m_Minute
            // 
            this.m_Minute.AccessibleDescription = null;
            this.m_Minute.AccessibleName = null;
            resources.ApplyResources(this.m_Minute, "m_Minute");
            this.m_Minute.Font = null;
            this.m_Minute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.m_Minute.Name = "m_Minute";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // m_Hour
            // 
            this.m_Hour.AccessibleDescription = null;
            this.m_Hour.AccessibleName = null;
            resources.ApplyResources(this.m_Hour, "m_Hour");
            this.m_Hour.Font = null;
            this.m_Hour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.m_Hour.Name = "m_Hour";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // m_Day
            // 
            this.m_Day.AccessibleDescription = null;
            this.m_Day.AccessibleName = null;
            resources.ApplyResources(this.m_Day, "m_Day");
            this.m_Day.Font = null;
            this.m_Day.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.m_Day.Name = "m_Day";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // comboBoxDsn
            // 
            this.comboBoxDsn.AccessibleDescription = null;
            this.comboBoxDsn.AccessibleName = null;
            resources.ApplyResources(this.comboBoxDsn, "comboBoxDsn");
            this.comboBoxDsn.BackgroundImage = null;
            this.comboBoxDsn.Font = null;
            this.comboBoxDsn.Name = "comboBoxDsn";
            // 
            // textBoxTable
            // 
            this.textBoxTable.AccessibleDescription = null;
            this.textBoxTable.AccessibleName = null;
            resources.ApplyResources(this.textBoxTable, "textBoxTable");
            this.textBoxTable.BackgroundImage = null;
            this.textBoxTable.Font = null;
            this.textBoxTable.Name = "textBoxTable";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // tabPage3
            // 
            this.tabPage3.AccessibleDescription = null;
            this.tabPage3.AccessibleName = null;
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.BackgroundImage = null;
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.textBoxColumnMilli);
            this.tabPage3.Controls.Add(this.label16);
            this.tabPage3.Controls.Add(this.textBoxColumnDate);
            this.tabPage3.Controls.Add(this.label17);
            this.tabPage3.Font = null;
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonDateType1);
            this.groupBox3.Controls.Add(this.radioButtonDateType0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonDateType1
            // 
            this.radioButtonDateType1.AccessibleDescription = null;
            this.radioButtonDateType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDateType1, "radioButtonDateType1");
            this.radioButtonDateType1.BackgroundImage = null;
            this.radioButtonDateType1.Font = null;
            this.radioButtonDateType1.Name = "radioButtonDateType1";
            // 
            // radioButtonDateType0
            // 
            this.radioButtonDateType0.AccessibleDescription = null;
            this.radioButtonDateType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDateType0, "radioButtonDateType0");
            this.radioButtonDateType0.BackgroundImage = null;
            this.radioButtonDateType0.Font = null;
            this.radioButtonDateType0.Name = "radioButtonDateType0";
            // 
            // textBoxColumnMilli
            // 
            this.textBoxColumnMilli.AccessibleDescription = null;
            this.textBoxColumnMilli.AccessibleName = null;
            resources.ApplyResources(this.textBoxColumnMilli, "textBoxColumnMilli");
            this.textBoxColumnMilli.BackgroundImage = null;
            this.textBoxColumnMilli.Font = null;
            this.textBoxColumnMilli.Name = "textBoxColumnMilli";
            // 
            // label16
            // 
            this.label16.AccessibleDescription = null;
            this.label16.AccessibleName = null;
            resources.ApplyResources(this.label16, "label16");
            this.label16.Font = null;
            this.label16.Name = "label16";
            // 
            // textBoxColumnDate
            // 
            this.textBoxColumnDate.AccessibleDescription = null;
            this.textBoxColumnDate.AccessibleName = null;
            resources.ApplyResources(this.textBoxColumnDate, "textBoxColumnDate");
            this.textBoxColumnDate.BackgroundImage = null;
            this.textBoxColumnDate.Font = null;
            this.textBoxColumnDate.Name = "textBoxColumnDate";
            // 
            // label17
            // 
            this.label17.AccessibleDescription = null;
            this.label17.AccessibleName = null;
            resources.ApplyResources(this.label17, "label17");
            this.label17.Font = null;
            this.label17.Name = "label17";
            // 
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.comboBoxDataType);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.textBoxField);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.buttonDelete);
            this.tabPage2.Controls.Add(this.buttonAdd);
            this.tabPage2.Controls.Add(this.m_list);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // comboBoxDataType
            // 
            this.comboBoxDataType.AccessibleDescription = null;
            this.comboBoxDataType.AccessibleName = null;
            resources.ApplyResources(this.comboBoxDataType, "comboBoxDataType");
            this.comboBoxDataType.BackgroundImage = null;
            this.comboBoxDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataType.Font = null;
            this.comboBoxDataType.Name = "comboBoxDataType";
            this.comboBoxDataType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataType_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // textBoxField
            // 
            this.textBoxField.AccessibleDescription = null;
            this.textBoxField.AccessibleName = null;
            resources.ApplyResources(this.textBoxField, "textBoxField");
            this.textBoxField.BackgroundImage = null;
            this.textBoxField.Font = null;
            this.textBoxField.Name = "textBoxField";
            this.textBoxField.TextChanged += new System.EventHandler(this.textBoxField_TextChanged);
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
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
            this.m_list.SmallImageList = this.imageList1;
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
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
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
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
            // checkBoxEnable
            // 
            this.checkBoxEnable.AccessibleDescription = null;
            this.checkBoxEnable.AccessibleName = null;
            resources.ApplyResources(this.checkBoxEnable, "checkBoxEnable");
            this.checkBoxEnable.BackgroundImage = null;
            this.checkBoxEnable.Font = null;
            this.checkBoxEnable.Name = "checkBoxEnable";
            this.checkBoxEnable.UseVisualStyleBackColor = true;
            this.checkBoxEnable.CheckedChanged += new System.EventHandler(this.checkBoxEnable_CheckedChanged);
            // 
            // buttonRunTag
            // 
            this.buttonRunTag.AccessibleDescription = null;
            this.buttonRunTag.AccessibleName = null;
            resources.ApplyResources(this.buttonRunTag, "buttonRunTag");
            this.buttonRunTag.BackgroundImage = null;
            this.buttonRunTag.Font = null;
            this.buttonRunTag.Name = "buttonRunTag";
            this.buttonRunTag.Click += new System.EventHandler(this.buttonRunTag_Click);
            // 
            // textBoxRunTag
            // 
            this.textBoxRunTag.AccessibleDescription = null;
            this.textBoxRunTag.AccessibleName = null;
            resources.ApplyResources(this.textBoxRunTag, "textBoxRunTag");
            this.textBoxRunTag.BackgroundImage = null;
            this.textBoxRunTag.Font = null;
            this.textBoxRunTag.Name = "textBoxRunTag";
            // 
            // checkBoxUseRunTag
            // 
            this.checkBoxUseRunTag.AccessibleDescription = null;
            this.checkBoxUseRunTag.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseRunTag, "checkBoxUseRunTag");
            this.checkBoxUseRunTag.BackgroundImage = null;
            this.checkBoxUseRunTag.Font = null;
            this.checkBoxUseRunTag.Name = "checkBoxUseRunTag";
            this.checkBoxUseRunTag.UseVisualStyleBackColor = true;
            this.checkBoxUseRunTag.CheckedChanged += new System.EventHandler(this.checkBoxUseRunTag_CheckedChanged);
            // 
            // FormDatabaseSaveListModify
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonRunTag);
            this.Controls.Add(this.textBoxRunTag);
            this.Controls.Add(this.checkBoxUseRunTag);
            this.Controls.Add(this.checkBoxEnable);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDatabaseSaveListModify";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDatabaseSaveListModify_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_Millisecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Second)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Minute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_Day)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(textBoxTitle.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean()) 
					MessageBox.Show("제목을 입력해야 합니다.", "설정오류");
				else if(Tools.IsLangChinese()) 
					MessageBox.Show("请输入标题。", "输入错误");
				else
					MessageBox.Show("You must input the title.", "Title error");

				return;
			}
			if(textBoxTable.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("테이블명을 입력해야 합니다.", "설정오류");
				if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入表名。", "输入错误");
				else
					MessageBox.Show("You must input the Table name.", "Table name error");

				return;
			}
			if(m_Day.Value == 0 && m_Hour.Value == 0 && m_Minute.Value == 0 && m_Second.Value == 0 && m_Millisecond.Value == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("저장주기가 설정되지 않았습니다.", "설정오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("没设置保存周期。", "输入错误");
				else
					MessageBox.Show("You must input the Data cycle.", "Data cycle error");
				return;
			}

			if(checkBoxSaveMinute.Checked && textBoxSaveMinute.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("분자료 테이블 명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入按1分钟单位资料的表名。", "输入错误");
				else
					MessageBox.Show("You must input the Table name of minute data.", "Input error");

				textBoxSaveMinute.Select();
				return;
			}
			if(checkBoxSaveHour.Checked && textBoxSaveHour.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("시간자료 테이블 명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入按1个小时单位资料的表名。", "输入错误");
				else
					MessageBox.Show("You must input the table name of Hour data.", "Input error");

				textBoxSaveHour.Select();
				return;
			}
			if(checkBoxSaveDay.Checked && textBoxSaveDay.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean()) 
					MessageBox.Show("일자료 테이블 명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese()) 
					MessageBox.Show("请输入按1天单位资料的表名。", "输入错误");
				else
					MessageBox.Show("You must input the table name of Day data.", "Input error");

				textBoxSaveDay.Select();
				return;
			}
			if(checkBoxSaveMonth.Checked && textBoxSaveMonth.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("월자료 테이블 명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入按1个月单位资料的表名。", "输入错误");
				else
					MessageBox.Show("You must input the table name of Month data.", "Input error");

				textBoxSaveMonth.Select();
				return;
			}
			if(checkBoxSaveYear.Checked && textBoxSaveYear.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("년자료 테이블 명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入按1年单位资料的表名。", "输入错误");
				else
					MessageBox.Show("You must input the table name of Month data.", "Input error");

				textBoxSaveYear.Select();
				return;
			}
			if(textBoxColumnDate.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("날짜/시간 컬럼명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入日期/时间列名。", "输入错误");
				else
					MessageBox.Show("You must input the DateTime column.", "Input error");

				textBoxColumnDate.Select();
				return;
			}
			if(textBoxColumnMilli.Text.Length == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("밀리초 컬럼명을 입력해야 합니다.", "입력오류");
				else if(NetTools.Tools.IsLangChinese())
					MessageBox.Show("请输入毫秒列名。", "输入错误");
				else
					MessageBox.Show("You must input the millisecond column.", "Input error");

				textBoxColumnMilli.Select();
				return;
			}

            if (this.checkBoxEnable.Checked && this.checkBoxUseRunTag.Checked && this.textBoxRunTag.Text.Trim().Length == 0)
            {
                MessageBox.Show("Input the Run Tag.", "Run Tag needed");

                textBoxRunTag.Select();
                return;
            }

			ListViewItem item;
			string field;

			for(int i = 0; i < m_list.Items.Count; i++) 
			{
				item = m_list.Items[i];
				field = item.SubItems[1].Text;
				for(int j = i+1; j < m_list.Items.Count; j++) 
				{
					item = m_list.Items[j];
					if(String.Compare(field, item.SubItems[1].Text, true) == 0) 
					{
						if(NetTools.Tools.IsLangKorean())
							MessageBox.Show("같은 이름의 컬럼이 존재합니다.", "Column="+field);
						else if(NetTools.Tools.IsLangChinese())
							MessageBox.Show("存在着同样的列名。", "Column="+field);
						else
							MessageBox.Show("Same column name is already exist.", "Column="+field);

						tabControl1.SelectedIndex = 1;
						item.Selected = true;
						item.EnsureVisible();
						return;
					}
				}
			}

            listTemp.bActive = this.checkBoxEnable.Checked;
			listTemp.title = textBoxTitle.Text;
			listTemp.con_dsn = comboBoxDsn.Text;
			listTemp.table = textBoxTable.Text;
			listTemp.cycle = ConvertTool.ToInt32(m_Day.Value)*1000*60*60*24+
							ConvertTool.ToInt32(m_Hour.Value)*1000*60*60+
							ConvertTool.ToInt32(m_Minute.Value)*1000*60+
							ConvertTool.ToInt32(m_Second.Value)*1000+
							ConvertTool.ToInt32(m_Millisecond.Value);

			
			/*
			DatabaseMember member;

			listTemp.arrayMember.Clear();
			for(int i = 0; i < m_list.Items.Count; i++) 
			{
				member = new DatabaseMember();
				item = m_list.Items[i];
				member.tag = item.SubItems[0].Text;
				member.field = item.SubItems[1].Text;
				member.eSaveType = (EnumSaveType)Enum.Parse(typeof(EnumSaveType), item.SubItems[2].Text);
				listTemp.arrayMember.Add(member);
			}
			*/
			listTemp.arrayMember = tempMember;

			listTemp.bSaveAsNextTime = this.checkBoxSaveAsNextTime.Checked;
			listTemp.bRelationDuplex = checkBoxDuplex.Checked;

			listTemp.bTableSaveMinute = checkBoxSaveMinute.Checked;
			listTemp.bTableSaveHour = checkBoxSaveHour.Checked;
			listTemp.bTableSaveDay = checkBoxSaveDay.Checked;
			listTemp.bTableSaveMonth = checkBoxSaveMonth.Checked;
			listTemp.bTableSaveYear = checkBoxSaveYear.Checked;

			listTemp.sTableSaveMinute = textBoxSaveMinute.Text;
			listTemp.sTableSaveHour = textBoxSaveHour.Text;
			listTemp.sTableSaveDay = textBoxSaveDay.Text;
			listTemp.sTableSaveMonth = textBoxSaveMonth.Text;
			listTemp.sTableSaveYear = textBoxSaveYear.Text;

			nTabPos = tabControl1.SelectedIndex;

			listTemp.sColumnDate = textBoxColumnDate.Text;
			listTemp.sColumnMilli = textBoxColumnMilli.Text;
			
			if(radioButtonDateType0.Checked)		listTemp.nDateType = 0;
			else if(radioButtonDateType1.Checked)	listTemp.nDateType = 1;
			else									listTemp.nDateType = 0;

            listTemp.bUseItemRunTag = this.checkBoxUseRunTag.Checked;
            listTemp.sItemRunTag = this.textBoxRunTag.Text;

			DialogResult = DialogResult.OK;
			Close();
		}

		int GetImageIndex(DatabaseMember member)
		{
			TagPublicClass tp;
			int index;

			tp = TagLib.GetStructPublic(member.tag, ref member.tag_pos);
			if(tp.enumTagType == EnumTagType.AI) 
				index = 0;
			else if(tp.enumTagType == EnumTagType.AO) 
				index = 1;
			else if(tp.enumTagType == EnumTagType.DI) 
				index = 2;
			else if(tp.enumTagType == EnumTagType.DO) 
				index = 3;
			else if(tp.enumTagType == EnumTagType.ST) 
				index = 4;
			else
				index = 5;

			return index;
		}

		int GetImageIndex(string tag)
		{
			TagPublicClass tp;
			int index;
			int[] tag_pos = new int[1];

			tp = TagLib.GetStructPublic(tag, ref tag_pos);
			if(tp.enumTagType == EnumTagType.AI) 
				index = 0;
			else if(tp.enumTagType == EnumTagType.AO) 
				index = 1;
			else if(tp.enumTagType == EnumTagType.DI) 
				index = 2;
			else if(tp.enumTagType == EnumTagType.DO) 
				index = 3;
			else if(tp.enumTagType == EnumTagType.ST) 
				index = 4;
			else
				index = 5;

			return index;
		}

		string GetSaveTypeString(EnumSaveType type)
		{
			if(NetTools.Tools.IsLangKorean()) 
			{
				if(type == EnumSaveType.Ave)			return "평균값";
				else if(type == EnumSaveType.Max)		return "최대값";
				else if(type == EnumSaveType.Min)		return "최소값";
				else if(type == EnumSaveType.Moment)	return "순시값";
				else if(type == EnumSaveType.OffTime)	return "OFF시간";
				else if(type == EnumSaveType.OnTime)	return "ON시간";
				else if(type == EnumSaveType.Sum)		return "적산값";
				else 									return "새로운형식";
			}
			else if(NetTools.Tools.IsLangJapanese()) 
			{
				if(type == EnumSaveType.Ave)			return "平均値";
				else if(type == EnumSaveType.Max)		return "最大値";
				else if(type == EnumSaveType.Min)		return "最小値";
				else if(type == EnumSaveType.Moment)	return "瞬間値";
				else if(type == EnumSaveType.OffTime)	return "OFF時間";
				else if(type == EnumSaveType.OnTime)	return "ON時間";
                else if (type == EnumSaveType.Sum) return "累計値";
				else 									return "NewType";
			}
			else if(NetTools.Tools.IsLangChinese()) 
			{
				if(type == EnumSaveType.Ave)			return "平均值";
				else if(type == EnumSaveType.Max)		return "最大值";
				else if(type == EnumSaveType.Min)		return "最小值";
                else if (type == EnumSaveType.Moment)   return "瞬时值";
				else if(type == EnumSaveType.OffTime)	return "OFF时间";
				else if(type == EnumSaveType.OnTime)	return "ON时间";
				else if(type == EnumSaveType.Sum)		return "累计值";
				else 									return "NewType";
			}
			else
			{
				if(type == EnumSaveType.Ave)			return "Ave";
				else if(type == EnumSaveType.Max)		return "Max";
				else if(type == EnumSaveType.Min)		return "Min";
				else if(type == EnumSaveType.Moment)	return "Moment";
				else if(type == EnumSaveType.OffTime)	return "OffTime";
				else if(type == EnumSaveType.OnTime)	return "OnTime";
				else if(type == EnumSaveType.Sum)		return "Sum";
				else 									return "NewType";
			}
		}

		private void FormDatabaseSaveListModify_Load(object sender, System.EventArgs e)
		{
            this.checkBoxEnable.Checked = listTemp.bActive;

			textBoxTitle.Text = listTemp.title;
			textBoxTable.Text = listTemp.table;

			int cycle = listTemp.cycle;

			m_Day.Value = cycle/(1000*60*60*24);
			cycle %= (1000*60*60*24);
			m_Hour.Value = cycle/(1000*60*60);
			cycle %= (1000*60*60);
			m_Minute.Value = cycle/(1000*60);
			cycle %= (1000*60);
			m_Second.Value = cycle/(1000);
			cycle %= (1000);
			m_Millisecond.Value = cycle;

			ConnectionString conn;
			for(int i = 0; i < dsnList.arrayConnectionString.Count; i++) 
			{
				conn = (ConnectionString)dsnList.arrayConnectionString[i];
				comboBoxDsn.Items.Add(conn.title);
			}

			comboBoxDsn.Text = listTemp.con_dsn;

			DatabaseMember member;

			ListViewItem item;
			
			for(int i = 0; i < listTemp.arrayMember.Count; i++) 
			{
				member = (DatabaseMember)listTemp.arrayMember[i];

				item = new ListViewItem();

				item.Text = member.tag;
				item.SubItems.Add("");
				item.SubItems.Add("");
				m_list.Items.Add(item);
				ChangeMemberListItem(item, member);
			}

			EnumSaveType[] types = (EnumSaveType[])Enum.GetValues(typeof(EnumSaveType));
			for(int i = 0; i < types.Length; i++) 
			{
				comboBoxDataType.Items.Add(GetSaveTypeString(types[i]));
			}

			checkBoxSaveAsNextTime.Checked = listTemp.bSaveAsNextTime;
			checkBoxDuplex.Checked = listTemp.bRelationDuplex;

			checkBoxSaveMinute.Checked = listTemp.bTableSaveMinute;
			checkBoxSaveHour.Checked = listTemp.bTableSaveHour;
			checkBoxSaveDay.Checked = listTemp.bTableSaveDay;
			checkBoxSaveMonth.Checked = listTemp.bTableSaveMonth;
			checkBoxSaveYear.Checked = listTemp.bTableSaveYear;

			textBoxSaveMinute.Text = listTemp.sTableSaveMinute;
			textBoxSaveHour.Text = listTemp.sTableSaveHour;
			textBoxSaveDay.Text = listTemp.sTableSaveDay;
			textBoxSaveMonth.Text = listTemp.sTableSaveMonth;
			textBoxSaveYear.Text = listTemp.sTableSaveYear;

			tabControl1.SelectedIndex = nTabPos;

			textBoxSaveMinute.Enabled = checkBoxSaveMinute.Checked;
			textBoxSaveHour.Enabled = checkBoxSaveHour.Checked;
			textBoxSaveDay.Enabled = checkBoxSaveDay.Checked;
			textBoxSaveMonth.Enabled = checkBoxSaveMonth.Checked;
			textBoxSaveYear.Enabled = checkBoxSaveYear.Checked;

			textBoxColumnDate.Text = listTemp.sColumnDate;
			textBoxColumnMilli.Text = listTemp.sColumnMilli;
			radioButtonDateType0.Checked = (listTemp.nDateType == 0);
			radioButtonDateType1.Checked = (listTemp.nDateType == 1);

            this.checkBoxUseRunTag.Checked = listTemp.bUseItemRunTag;
            this.textBoxRunTag.Text = listTemp.sItemRunTag;

            EnableTotal();
		}

		void ChangeMemberListItem(ListViewItem lvi, DatabaseMember member)
		{
			lvi.ImageIndex = GetImageIndex(member.tag);

			lvi.SubItems[0].Text = member.tag;
			lvi.SubItems[1].Text = member.field;
			lvi.SubItems[2].Text = GetSaveTypeString(member.eSaveType);
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			SelectTag dialog = new SelectTag();

			dialog.bUseTagAI = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagST = true;

			if(dialog.Run(this) == DialogResult.OK) 
			{
				DatabaseMember member = new DatabaseMember();

				ListViewItem item = new ListViewItem();

				member.tag = dialog.sTag;
				member.field = dialog.sTag;
				member.eSaveType = EnumSaveType.Moment;

				tempMember.Add(member);

				item.SubItems.Add("");
				item.SubItems.Add("");
				ChangeMemberListItem(item, member);
				item.Selected = true;
				item.EnsureVisible();
				m_list.Items.Add(item);
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("삭제하고 싶은 항목을 선택한 후 다시 하세요.", "선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else
					MessageBox.Show("Select item to delete.", "Delete error");

				return;
			}

			int index = m_list.SelectedItems[0].Index;

			m_list.Items.RemoveAt(index);
			tempMember.RemoveAt(index);
		}

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				return;
			}

			//ListViewItem item = m_list.SelectedItems[0];
			//textBoxField.Text = item.SubItems[1].Text;
			//comboBoxDataType.Text = item.SubItems[2].Text;
			int index = m_list.SelectedItems[0].Index;
			DatabaseMember member = (DatabaseMember)tempMember[index];

			textBoxField.Text = member.field;
			comboBoxDataType.Text = GetSaveTypeString(member.eSaveType);
		}

		private void textBoxField_TextChanged(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0)
			{
				return;
			}

			int index = m_list.SelectedItems[0].Index;
			DatabaseMember member = (DatabaseMember)tempMember[index];

			ListViewItem item = m_list.SelectedItems[0];
			member.field = textBoxField.Text;
			ChangeMemberListItem(item, member);
		}

		private void comboBoxDataType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0)
			{
				return;
			}

			int index = m_list.SelectedItems[0].Index;
			DatabaseMember member = (DatabaseMember)tempMember[index];

			ListViewItem item = m_list.SelectedItems[0];
			member.eSaveType = (EnumSaveType)comboBoxDataType.SelectedIndex;
			ChangeMemberListItem(item, member);
		}

		private void checkBoxSaveMinute_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxSaveMinute.Enabled = checkBoxSaveMinute.Checked;
		}

		private void checkBoxSaveHour_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxSaveHour.Enabled = checkBoxSaveHour.Checked;
		}

		private void checkBoxSaveDay_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxSaveDay.Enabled = checkBoxSaveDay.Checked;
		}

		private void checkBoxSaveMonth_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxSaveMonth.Enabled = checkBoxSaveMonth.Checked;
		}

		private void checkBoxSaveYear_CheckedChanged(object sender, System.EventArgs e)
		{
			textBoxSaveYear.Enabled = checkBoxSaveYear.Checked;
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_list.SmallImageList = imageList1;
		}

        void EnableDisableRunTag()
        {
            bool flag;

            if (this.checkBoxEnable.Checked)
            {
                flag = this.checkBoxUseRunTag.Checked;
            }
            else
            {
                flag = false;
            }

            this.textBoxRunTag.Enabled = flag;
            this.buttonRunTag.Enabled = flag;
        }

        void EnableTotal()
        {
            bool flag = this.checkBoxEnable.Checked;

            this.tabControl1.Enabled = flag;

            this.checkBoxUseRunTag.Enabled = flag;

            EnableDisableRunTag();
        }

        private void checkBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            EnableTotal();
        }

        private void checkBoxUseRunTag_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableRunTag();
        }

        private void buttonRunTag_Click(object sender, EventArgs e)
        {
            string tag, des;

            if (DialogTag.SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxRunTag.Text = tag;
            }
        }
	}
}
