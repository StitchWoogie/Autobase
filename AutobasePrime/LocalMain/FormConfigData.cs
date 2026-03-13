using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;
using System.IO;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormConfigData.
	/// </summary>
	public class FormConfigData : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxSaveDataAll;
		private System.Windows.Forms.TextBox textBoxBasicDirectory;
		private System.Windows.Forms.Button buttonBasicDirectory;
		private System.Windows.Forms.Button buttonLogDirectory;
		private System.Windows.Forms.TextBox textBoxLogDirectory;
		private System.Windows.Forms.CheckBox checkBoxUseAutoDelete;
		private AutoLibLocal.MyNumericUpDown numericUpDownLimitHour;
        private AutoLibLocal.MyNumericUpDown numericUpDownLimitMinute;
        private AutoLibLocal.MyNumericUpDown numericUpDownSavingCountAtOnce;
        private AutoLibLocal.MyNumericUpDown numericUpDownSaveDelay;
		private System.Windows.Forms.CheckBox checkBoxSaveAtTestMode;
		private System.Windows.Forms.CheckBox checkBoxSaveAtZeroValue;
		private System.Windows.Forms.Button buttonDirSecondary;
		private System.Windows.Forms.TextBox textBoxDirSecondary;
		private System.Windows.Forms.Button buttonDirPrimary;
		private System.Windows.Forms.TextBox textBoxDirPrimary;
		private System.Windows.Forms.CheckBox checkBoxUseDuplexData;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxBackupDirectory;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private RadioButton radioButtonDataDeletingOption1;
        private RadioButton radioButtonDataDeletingOption0;
        private GroupBox groupBox8;
        private Label label10;
        private AutoLibLocal.MyNumericUpDown numericUpDownDiskUsedSizeStart;
        private Label label13;
        private Label label14;
        private AutoLibLocal.MyNumericUpDown numericUpDownDiskUsedSizeAlarm;
        private Label label15;
        private GroupBox groupBox7;
        private ProgressBar progressBarDiskUsed;
        private Label labelDiskUsed;
        private CheckBox checkBoxDiscardNotSaveDataOnProgramExit;
        private CheckBox checkBoxDisplayMessageDataSkip;
        private CheckBox checkBoxSkipDataWhenSavingDelayed;
        private TabPage tabPage3;
        private Button buttonBackupFolder;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigData()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigData));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxSaveDataAll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonLogDirectory = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLogDirectory = new System.Windows.Forms.TextBox();
            this.buttonBasicDirectory = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxBasicDirectory = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownLimitMinute = new AutoLibLocal.MyNumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownLimitHour = new AutoLibLocal.MyNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxBackupDirectory = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxUseAutoDelete = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownSavingCountAtOnce = new AutoLibLocal.MyNumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownSaveDelay = new AutoLibLocal.MyNumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxSkipDataWhenSavingDelayed = new System.Windows.Forms.CheckBox();
            this.checkBoxDiscardNotSaveDataOnProgramExit = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveAtZeroValue = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveAtTestMode = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseDuplexData = new System.Windows.Forms.CheckBox();
            this.buttonDirSecondary = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxDirSecondary = new System.Windows.Forms.TextBox();
            this.buttonDirPrimary = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxDirPrimary = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxDisplayMessageDataSkip = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonBackupFolder = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.labelDiskUsed = new System.Windows.Forms.Label();
            this.progressBarDiskUsed = new System.Windows.Forms.ProgressBar();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownDiskUsedSizeStart = new AutoLibLocal.MyNumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDownDiskUsedSizeAlarm = new AutoLibLocal.MyNumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.radioButtonDataDeletingOption0 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataDeletingOption1 = new System.Windows.Forms.RadioButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitHour)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSavingCountAtOnce)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveDelay)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiskUsedSizeStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiskUsedSizeAlarm)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.tabPage3.SuspendLayout();
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
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // checkBoxSaveDataAll
            // 
            resources.ApplyResources(this.checkBoxSaveDataAll, "checkBoxSaveDataAll");
            this.checkBoxSaveDataAll.Name = "checkBoxSaveDataAll";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.buttonLogDirectory);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxLogDirectory);
            this.groupBox1.Controls.Add(this.buttonBasicDirectory);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxBasicDirectory);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonLogDirectory
            // 
            resources.ApplyResources(this.buttonLogDirectory, "buttonLogDirectory");
            this.buttonLogDirectory.Name = "buttonLogDirectory";
            this.buttonLogDirectory.Click += new System.EventHandler(this.buttonLogDirectory_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxLogDirectory
            // 
            resources.ApplyResources(this.textBoxLogDirectory, "textBoxLogDirectory");
            this.textBoxLogDirectory.Name = "textBoxLogDirectory";
            // 
            // buttonBasicDirectory
            // 
            resources.ApplyResources(this.buttonBasicDirectory, "buttonBasicDirectory");
            this.buttonBasicDirectory.Name = "buttonBasicDirectory";
            this.buttonBasicDirectory.Click += new System.EventHandler(this.buttonBasicDirectory_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxBasicDirectory
            // 
            resources.ApplyResources(this.textBoxBasicDirectory, "textBoxBasicDirectory");
            this.textBoxBasicDirectory.Name = "textBoxBasicDirectory";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericUpDownLimitMinute);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numericUpDownLimitHour);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownLimitMinute
            // 
            resources.ApplyResources(this.numericUpDownLimitMinute, "numericUpDownLimitMinute");
            this.numericUpDownLimitMinute.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownLimitMinute.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLimitMinute.Name = "numericUpDownLimitMinute";
            this.numericUpDownLimitMinute.SampleProperty = 0;
            this.numericUpDownLimitMinute.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownLimitHour
            // 
            resources.ApplyResources(this.numericUpDownLimitHour, "numericUpDownLimitHour");
            this.numericUpDownLimitHour.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownLimitHour.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLimitHour.Name = "numericUpDownLimitHour";
            this.numericUpDownLimitHour.SampleProperty = 0;
            this.numericUpDownLimitHour.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxBackupDirectory
            // 
            resources.ApplyResources(this.textBoxBackupDirectory, "textBoxBackupDirectory");
            this.textBoxBackupDirectory.Name = "textBoxBackupDirectory";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // checkBoxUseAutoDelete
            // 
            resources.ApplyResources(this.checkBoxUseAutoDelete, "checkBoxUseAutoDelete");
            this.checkBoxUseAutoDelete.Name = "checkBoxUseAutoDelete";
            this.checkBoxUseAutoDelete.CheckedChanged += new System.EventHandler(this.checkBoxUseAutoDelete_CheckedChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.numericUpDownSavingCountAtOnce);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericUpDownSavingCountAtOnce
            // 
            resources.ApplyResources(this.numericUpDownSavingCountAtOnce, "numericUpDownSavingCountAtOnce");
            this.numericUpDownSavingCountAtOnce.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownSavingCountAtOnce.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownSavingCountAtOnce.Name = "numericUpDownSavingCountAtOnce";
            this.numericUpDownSavingCountAtOnce.SampleProperty = 0;
            this.numericUpDownSavingCountAtOnce.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.numericUpDownSaveDelay);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // numericUpDownSaveDelay
            // 
            resources.ApplyResources(this.numericUpDownSaveDelay, "numericUpDownSaveDelay");
            this.numericUpDownSaveDelay.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownSaveDelay.Name = "numericUpDownSaveDelay";
            this.numericUpDownSaveDelay.SampleProperty = 0;
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.checkBoxSkipDataWhenSavingDelayed);
            this.groupBox5.Controls.Add(this.checkBoxDiscardNotSaveDataOnProgramExit);
            this.groupBox5.Controls.Add(this.checkBoxSaveAtZeroValue);
            this.groupBox5.Controls.Add(this.checkBoxSaveAtTestMode);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBoxSkipDataWhenSavingDelayed
            // 
            resources.ApplyResources(this.checkBoxSkipDataWhenSavingDelayed, "checkBoxSkipDataWhenSavingDelayed");
            this.checkBoxSkipDataWhenSavingDelayed.Name = "checkBoxSkipDataWhenSavingDelayed";
            // 
            // checkBoxDiscardNotSaveDataOnProgramExit
            // 
            resources.ApplyResources(this.checkBoxDiscardNotSaveDataOnProgramExit, "checkBoxDiscardNotSaveDataOnProgramExit");
            this.checkBoxDiscardNotSaveDataOnProgramExit.Name = "checkBoxDiscardNotSaveDataOnProgramExit";
            // 
            // checkBoxSaveAtZeroValue
            // 
            resources.ApplyResources(this.checkBoxSaveAtZeroValue, "checkBoxSaveAtZeroValue");
            this.checkBoxSaveAtZeroValue.Name = "checkBoxSaveAtZeroValue";
            // 
            // checkBoxSaveAtTestMode
            // 
            resources.ApplyResources(this.checkBoxSaveAtTestMode, "checkBoxSaveAtTestMode");
            this.checkBoxSaveAtTestMode.Name = "checkBoxSaveAtTestMode";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.checkBoxUseDuplexData);
            this.groupBox6.Controls.Add(this.buttonDirSecondary);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.textBoxDirSecondary);
            this.groupBox6.Controls.Add(this.buttonDirPrimary);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.textBoxDirPrimary);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // checkBoxUseDuplexData
            // 
            resources.ApplyResources(this.checkBoxUseDuplexData, "checkBoxUseDuplexData");
            this.checkBoxUseDuplexData.Name = "checkBoxUseDuplexData";
            // 
            // buttonDirSecondary
            // 
            resources.ApplyResources(this.buttonDirSecondary, "buttonDirSecondary");
            this.buttonDirSecondary.Name = "buttonDirSecondary";
            this.buttonDirSecondary.Click += new System.EventHandler(this.buttonDirSecondary_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // textBoxDirSecondary
            // 
            resources.ApplyResources(this.textBoxDirSecondary, "textBoxDirSecondary");
            this.textBoxDirSecondary.Name = "textBoxDirSecondary";
            // 
            // buttonDirPrimary
            // 
            resources.ApplyResources(this.buttonDirPrimary, "buttonDirPrimary");
            this.buttonDirPrimary.Name = "buttonDirPrimary";
            this.buttonDirPrimary.Click += new System.EventHandler(this.buttonDirPrimary_Click);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // textBoxDirPrimary
            // 
            resources.ApplyResources(this.textBoxDirPrimary, "textBoxDirPrimary");
            this.textBoxDirPrimary.Name = "textBoxDirPrimary";
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.checkBoxDisplayMessageDataSkip);
            this.tabPage1.Controls.Add(this.checkBoxSaveDataAll);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisplayMessageDataSkip
            // 
            resources.ApplyResources(this.checkBoxDisplayMessageDataSkip, "checkBoxDisplayMessageDataSkip");
            this.checkBoxDisplayMessageDataSkip.Name = "checkBoxDisplayMessageDataSkip";
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.buttonBackupFolder);
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.textBoxBackupDirectory);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.checkBoxUseAutoDelete);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonBackupFolder
            // 
            resources.ApplyResources(this.buttonBackupFolder, "buttonBackupFolder");
            this.buttonBackupFolder.Name = "buttonBackupFolder";
            this.buttonBackupFolder.Click += new System.EventHandler(this.buttonBackupFolder_Click);
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.labelDiskUsed);
            this.groupBox8.Controls.Add(this.progressBarDiskUsed);
            this.groupBox8.Controls.Add(this.label10);
            this.groupBox8.Controls.Add(this.numericUpDownDiskUsedSizeStart);
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.numericUpDownDiskUsedSizeAlarm);
            this.groupBox8.Controls.Add(this.label15);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // labelDiskUsed
            // 
            resources.ApplyResources(this.labelDiskUsed, "labelDiskUsed");
            this.labelDiskUsed.Name = "labelDiskUsed";
            // 
            // progressBarDiskUsed
            // 
            resources.ApplyResources(this.progressBarDiskUsed, "progressBarDiskUsed");
            this.progressBarDiskUsed.Name = "progressBarDiskUsed";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // numericUpDownDiskUsedSizeStart
            // 
            resources.ApplyResources(this.numericUpDownDiskUsedSizeStart, "numericUpDownDiskUsedSizeStart");
            this.numericUpDownDiskUsedSizeStart.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDiskUsedSizeStart.Name = "numericUpDownDiskUsedSizeStart";
            this.numericUpDownDiskUsedSizeStart.SampleProperty = 0;
            this.numericUpDownDiskUsedSizeStart.Value = new decimal(new int[] {
            95,
            0,
            0,
            0});
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // numericUpDownDiskUsedSizeAlarm
            // 
            resources.ApplyResources(this.numericUpDownDiskUsedSizeAlarm, "numericUpDownDiskUsedSizeAlarm");
            this.numericUpDownDiskUsedSizeAlarm.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDiskUsedSizeAlarm.Name = "numericUpDownDiskUsedSizeAlarm";
            this.numericUpDownDiskUsedSizeAlarm.SampleProperty = 0;
            this.numericUpDownDiskUsedSizeAlarm.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.radioButtonDataDeletingOption0);
            this.groupBox7.Controls.Add(this.radioButtonDataDeletingOption1);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // radioButtonDataDeletingOption0
            // 
            resources.ApplyResources(this.radioButtonDataDeletingOption0, "radioButtonDataDeletingOption0");
            this.radioButtonDataDeletingOption0.Name = "radioButtonDataDeletingOption0";
            this.radioButtonDataDeletingOption0.TabStop = true;
            this.radioButtonDataDeletingOption0.UseVisualStyleBackColor = true;
            this.radioButtonDataDeletingOption0.CheckedChanged += new System.EventHandler(this.radioButtonDataDeletingOption0_CheckedChanged);
            // 
            // radioButtonDataDeletingOption1
            // 
            resources.ApplyResources(this.radioButtonDataDeletingOption1, "radioButtonDataDeletingOption1");
            this.radioButtonDataDeletingOption1.Name = "radioButtonDataDeletingOption1";
            this.radioButtonDataDeletingOption1.TabStop = true;
            this.radioButtonDataDeletingOption1.UseVisualStyleBackColor = true;
            this.radioButtonDataDeletingOption1.CheckedChanged += new System.EventHandler(this.radioButtonDataDeletingOption1_CheckedChanged);
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // FormConfigData
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigData";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigData_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLimitHour)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSavingCountAtOnce)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveDelay)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiskUsedSizeStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiskUsedSizeAlarm)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigData_Load(object sender, System.EventArgs e)
		{
			this.checkBoxSaveDataAll.Checked = ConfigData.bDataSaveAll;
			this.textBoxBasicDirectory.Text = ConfigData.sDirData;
			this.textBoxLogDirectory.Text = ConfigData.sDirDataLog;
			this.checkBoxUseAutoDelete.Checked = ConfigData.bDataUseAutoDelete;
			this.numericUpDownLimitHour.Value = ConfigData.nMonthDataSave;
			this.numericUpDownLimitMinute.Value = ConfigData.nMonthDataSaveOfMin;

			this.numericUpDownSaveDelay.Value = ConfigData.nDataSaveDelay;
			this.checkBoxSaveAtTestMode.Checked = ConfigData.bDataSaveAtTestMode;
			this.checkBoxSaveAtZeroValue.Checked = ConfigData.bDataSaveAtZeroValue;
			this.checkBoxUseDuplexData.Checked = ConfigData.duplexDir.bUse == 1;
			this.textBoxDirPrimary.Text = ConfigData.duplexDir.sPrimary;
			this.textBoxDirSecondary.Text = ConfigData.duplexDir.sSecondary;
			this.textBoxBackupDirectory.Text = ConfigData.sBackupDirectory;

            this.radioButtonDataDeletingOption0.Checked = (ConfigData.nAutoDeleteMethod == 0);
            this.radioButtonDataDeletingOption1.Checked = (ConfigData.nAutoDeleteMethod == 1);
            Tools.SetNumericUpDownValue(this.numericUpDownDiskUsedSizeAlarm, ConfigData.nAutoDeleteUsedSizeAlarm);
            Tools.SetNumericUpDownValue(this.numericUpDownDiskUsedSizeStart, ConfigData.nAutoDeleteUsedSizeStart);

            this.checkBoxDiscardNotSaveDataOnProgramExit.Checked = ConfigData.bDiscardNotSavedDataOnProgramExit;

            Tools.SetNumericUpDownValue(this.numericUpDownSavingCountAtOnce, ConfigData.nSaveCountAtOnce);
            this.checkBoxDisplayMessageDataSkip.Checked = ConfigData.bDisplaySaveSkipMessage;
            //this.checkBoxUseThreadWhenSavingData.Checked = ConfigData.bUseThreadWhenSavingData;
            this.checkBoxSkipDataWhenSavingDelayed.Checked = ConfigData.bSkipDataWhenSavingDelayed;

            SetDiskUsed();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_DATA))
				this.buttonOK.Enabled = false;

            EnableDisableAutoDelete();

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.checkBoxDiscardNotSaveDataOnProgramExit.Visible = false;
                this.checkBoxDisplayMessageDataSkip.Visible = false;
                this.checkBoxSkipDataWhenSavingDelayed.Visible = false;

                this.checkBoxSaveAtTestMode.Visible = false;    // 테스트모드일 때도 자료저장

                this.tabControl1.TabPages.Remove(tabPage3);
                this.tabControl1.TabPages.Remove(tabPage2);

                this.groupBox3.Visible = false; // 한번에 저장할 개수

                this.textBoxBasicDirectory.ReadOnly = true;
                this.textBoxLogDirectory.ReadOnly = true;

                this.textBoxBackupDirectory.ReadOnly = true;

                this.label6.Visible = false;
                this.numericUpDownLimitMinute.Visible = false;
                this.label5.Visible = false;
            }
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			// 백업 폴더를 만들 수 있는가를 검사한다.
			if(!Directory.Exists(this.textBoxBackupDirectory.Text)) 
			{
				Directory.CreateDirectory(this.textBoxBackupDirectory.Text);	
			}

			if(!Directory.Exists(this.textBoxBackupDirectory.Text)) 
			{
				MessageBox.Show("Can't make the data backup directory.", "Directory error");
				return;
			}

			ConfigData.bDataSaveAll = this.checkBoxSaveDataAll.Checked;
			ConfigData.sDirData = this.textBoxBasicDirectory.Text;
			ConfigData.sDirDataLog = this.textBoxLogDirectory.Text;
			ConfigData.bDataUseAutoDelete = this.checkBoxUseAutoDelete.Checked;
			ConfigData.nMonthDataSave = ConvertTool.ToInt32(this.numericUpDownLimitHour.Value);
			ConfigData.nMonthDataSaveOfMin = ConvertTool.ToInt32(this.numericUpDownLimitMinute.Value);
			ConfigData.nDataSaveDelay = ConvertTool.ToInt32(this.numericUpDownSaveDelay.Value);
			ConfigData.bDataSaveAtTestMode = this.checkBoxSaveAtTestMode.Checked;
			ConfigData.bDataSaveAtZeroValue = this.checkBoxSaveAtZeroValue.Checked;
			ConfigData.duplexDir.bUse = this.checkBoxUseDuplexData.Checked ? (sbyte)1 : (sbyte)0;
			ConfigData.duplexDir.sPrimary = this.textBoxDirPrimary.Text;
			ConfigData.duplexDir.sSecondary = this.textBoxDirSecondary.Text;
			ConfigData.sBackupDirectory = this.textBoxBackupDirectory.Text;

            if (this.radioButtonDataDeletingOption0.Checked) ConfigData.nAutoDeleteMethod = 0;
            else if (this.radioButtonDataDeletingOption1.Checked) ConfigData.nAutoDeleteMethod = 1;
            else ConfigData.nAutoDeleteMethod = 0;

            ConfigData.nAutoDeleteUsedSizeAlarm = ConvertTool.ToInt32(this.numericUpDownDiskUsedSizeAlarm.Value);
            ConfigData.nAutoDeleteUsedSizeStart = ConvertTool.ToInt32(this.numericUpDownDiskUsedSizeStart.Value);

            ConfigData.bDiscardNotSavedDataOnProgramExit = this.checkBoxDiscardNotSaveDataOnProgramExit.Checked;

            ConfigData.nSaveCountAtOnce = ConvertTool.ToInt32(this.numericUpDownSavingCountAtOnce.Value);
            ConfigData.bDisplaySaveSkipMessage = this.checkBoxDisplayMessageDataSkip.Checked;
            //ConfigData.bUseThreadWhenSavingData = this.checkBoxUseThreadWhenSavingData.Checked;
            ConfigData.bSkipDataWhenSavingDelayed = this.checkBoxSkipDataWhenSavingDelayed.Checked;

			ConfigData.SaveConfig();
            //CheckEngineMinuteChanged.UnInitThread();
            //CheckEngineMinuteChanged.InitThread();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonBasicDirectory_Click(object sender, System.EventArgs e)
		{
			FormDirectorySelect dialog = new FormDirectorySelect();
			dialog.FileName = this.textBoxBasicDirectory.Text;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxBasicDirectory.Text = dialog.FileName;
			}
		}

		private void buttonLogDirectory_Click(object sender, System.EventArgs e)
		{
			FormDirectorySelect dialog = new FormDirectorySelect();
			dialog.FileName = this.textBoxLogDirectory.Text;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxLogDirectory.Text = dialog.FileName;
			}
		}

		private void buttonDirPrimary_Click(object sender, System.EventArgs e)
		{
			FormDirectorySelect dialog = new FormDirectorySelect();
			dialog.FileName = this.textBoxDirPrimary.Text;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxDirPrimary.Text = dialog.FileName;
			}
		}

		private void buttonDirSecondary_Click(object sender, System.EventArgs e)
		{
			FormDirectorySelect dialog = new FormDirectorySelect();
			dialog.FileName = this.textBoxDirSecondary.Text;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxDirSecondary.Text = dialog.FileName;
			}
		}

        void EnableDisableAutoDelete()
        {
            bool flag = this.checkBoxUseAutoDelete.Checked;

            this.radioButtonDataDeletingOption0.Enabled = flag;
            this.radioButtonDataDeletingOption1.Enabled = flag;
            this.textBoxBackupDirectory.Enabled = flag;

            bool flag_monthlimit = false;
            bool flag_usedlimit = false;

            if (flag)
            {
                if (this.radioButtonDataDeletingOption1.Checked) flag_usedlimit = true;
                else flag_monthlimit = true;
            }

            this.numericUpDownLimitHour.Enabled = flag_monthlimit;
            this.numericUpDownLimitMinute.Enabled = flag_monthlimit;

            this.numericUpDownDiskUsedSizeAlarm.Enabled = flag_usedlimit;
            this.numericUpDownDiskUsedSizeStart.Enabled = flag_usedlimit;

        }

		private void checkBoxUseAutoDelete_CheckedChanged(object sender, System.EventArgs e)
		{
            EnableDisableAutoDelete();
		}

        private void radioButtonDataDeletingOption0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAutoDelete();
        }

        private void radioButtonDataDeletingOption1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAutoDelete();
        }

        public static double GetDiskUsedPercent(string basic_directory)
        {
            string root = Path.GetPathRoot(basic_directory);

            ulong FreeBytesAvailable;
            ulong TotalNumberOfBytes;
            ulong TotalNumberOfFreeBytes;
            if (!Win32Function.GetDiskFreeSpaceEx(root, out FreeBytesAvailable, out TotalNumberOfBytes, out TotalNumberOfFreeBytes)) {
                return 0;
            }

            double percent = (double)(TotalNumberOfBytes-FreeBytesAvailable)/(TotalNumberOfBytes);
            percent *= 100;

            return percent;
        }

        void SetDiskUsed()
        {
            double percent = GetDiskUsedPercent(this.textBoxBasicDirectory.Text);

            this.progressBarDiskUsed.Value = (int)percent;

            if(Tools.IsLangKorean())
                this.labelDiskUsed.Text = String.Format("{0:F1}% 사용 중", percent);
            else
                this.labelDiskUsed.Text = String.Format("{0:F1}% Used", percent);
        }

        private void buttonBackupFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.SelectedPath = this.textBoxBackupDirectory.Text;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxBackupDirectory.Text = dialog.SelectedPath;
            }
        }

	}

	public class ConfigData 
	{
		public static string	sDirData;			// 데이터 파일이 저장된 폴더를 말한다.
		public static string	sDirDataLog;		// 데이터 파일이 저장된 폴더를 말한다.
		public static DUPLEX_DIR_STRUCT duplexDir = new DUPLEX_DIR_STRUCT();

		public static int	nMonthDataSave; 		// 데이터 파일을 저장하는 기한수를 말한다.
		public static int	nMonthDataSaveOfMin; 	// 데이터 파일을 저장하는 기한수를 말한다. Trend
		public static int	nDataSaveDelay; 		// 지정된 분 후부터 데이터를 저장한다.
		public static bool	bDataSaveAll;			// 모든 자료를 저장한다.
		public static bool	bDataSaveAtTestMode;	// 테스트 모드일때도 자료를 저장한다.
		public static bool	bDataSaveAtZeroValue;	// 아날로그 값이 0일때도 자료를 저장한다.
		public static readonly int	nDataSaveTrendRemain = 10;	// 분자료 저장시 몇분단위로 묶어서 저장할까?

		public static bool	bDataUseAutoDelete;		// 데이터를 자동으로 삭제한다.
        public static int   nAutoDeleteMethod;
        public static int nAutoDeleteUsedSizeAlarm; 
        public static int nAutoDeleteUsedSizeStart;
		public static string sBackupDirectory;		// 자동삭제시 백업 폴더

        public static bool bDiscardNotSavedDataOnProgramExit;   // 프로그램 종료시 저장하지 못한 데이터 버림

        public static bool bDisplaySaveSkipMessage;
        public static int nSaveCountAtOnce = 1;
        //public static bool bUseThreadWhenSavingData;            // 데이터 저장시 스레드 사용
        public static bool bSkipDataWhenSavingDelayed;

		static ConfigData()
		{
			LoadConfig();
		}

		static void LoadConfig()
		{
			string work_dir = TotalConfig.sDirWorkProject;
			sDirData = TotalConfig.GetProjectDataDirectory(work_dir);
			sDirDataLog = TotalConfig.GetProjectDataLogDirectory(work_dir);
			duplexDir.bUse = (sbyte)TotalConfig.LoadRegAutoBaseProjectConfig(work_dir, "DuplexDirUse", 0);
			duplexDir.sPrimary = TotalConfig.LoadRegAutoBaseProjectConfig(work_dir, "DuplexDirPrimary", "C:\\PRIMARY");
			duplexDir.sSecondary = TotalConfig.LoadRegAutoBaseProjectConfig(work_dir, "DuplexDirSecondary", "C:\\SECONDARY");
			nMonthDataSave = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nMonthDataSave", 12);
			nMonthDataSaveOfMin = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nMonthDataSaveOfMin", 1);

			nDataSaveDelay = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nDataSaveDelay", 0);
			bDataSaveAll = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bDataSaveAll", true);
			bDataSaveAtTestMode = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bDataSaveAtTestMode", true);
			bDataSaveAtZeroValue = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bDataSaveAtZeroValue", true);
            //nDataSaveTrendRemain = 10;// TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nDataSaveTrendRemain", 10);
            			
			// 자동삭제 기능은 일단은 끄는 것이 좋겠다. (8.5환경에서 꺼놓았을 수 있으므로)
			bDataUseAutoDelete = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bDataUseAutoDelete", false);
            nAutoDeleteMethod = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nAutoDeleteMethod", 0);
            nAutoDeleteUsedSizeAlarm = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nAutoDeleteUsedSizeAlarm", 90);
            nAutoDeleteUsedSizeStart = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nAutoDeleteUsedSizeStart", 95);
            sBackupDirectory = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "sBackupDirectory", TotalConfig.MakePathByWindowsDisk(TotalConfig.sOemRootFolder + "\\Backup Data"));

            bDiscardNotSavedDataOnProgramExit = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bDiscardNotSavedDataOnProgramExit", false);

            bDisplaySaveSkipMessage = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bDisplaySaveSkipMessage", false);
            nSaveCountAtOnce = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "nDBSaveCountAtOnce", 100); //251028 PSU 레지스트리명 변경 및 최소 100개 처리.
            //bUseThreadWhenSavingData = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bUseThreadWhenSavingData", false);
            bSkipDataWhenSavingDelayed = TotalConfig.LoadRegAutoBaseConfig("ConfigData", null, "bSkipDataWhenSavingDelayed", false);

            if (nSaveCountAtOnce < 100) nSaveCountAtOnce = 100;   //251028 PSU 레지스트리명 변경 및 최소 100개 처리.
        }

		public static void SaveConfig()
		{
			string work_dir = TotalConfig.sDirWorkProject;
			TotalConfig.SetProjectDataDirectory(work_dir, sDirData);
			TotalConfig.SetProjectDataLogDirectory(work_dir, sDirDataLog);
			TotalConfig.SaveRegAutoBaseProjectConfig(work_dir, "DuplexDirUse", duplexDir.bUse);
			TotalConfig.SaveRegAutoBaseProjectConfig(work_dir, "DuplexDirPrimary", duplexDir.sPrimary);
			TotalConfig.SaveRegAutoBaseProjectConfig(work_dir, "DuplexDirSecondary", duplexDir.sSecondary);
			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nMonthDataSave", nMonthDataSave);
			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nMonthDataSaveOfMin", nMonthDataSaveOfMin);

			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nDataSaveDelay", nDataSaveDelay);
			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bDataSaveAll", bDataSaveAll);
			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bDataSaveAtTestMode", bDataSaveAtTestMode);
			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bDataSaveAtZeroValue", bDataSaveAtZeroValue);
			//TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nDataSaveTrendRemain", nDataSaveTrendRemain);

			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bDataUseAutoDelete", bDataUseAutoDelete);
            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nAutoDeleteMethod", nAutoDeleteMethod);
            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nAutoDeleteUsedSizeAlarm", nAutoDeleteUsedSizeAlarm);
            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nAutoDeleteUsedSizeStart", nAutoDeleteUsedSizeStart);
			TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "sBackupDirectory", sBackupDirectory);

            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bDiscardNotSavedDataOnProgramExit", bDiscardNotSavedDataOnProgramExit);

            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bDisplaySaveSkipMessage", bDisplaySaveSkipMessage);
            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "nDBSaveCountAtOnce", nSaveCountAtOnce);  //251028 PSU 레지스트리명 변경 및 최소 100개 처리.
            //TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bUseThreadWhenSavingData", bUseThreadWhenSavingData);
            TotalConfig.SaveRegAutoBaseConfig("ConfigData", null, "bSkipDataWhenSavingDelayed", bSkipDataWhenSavingDelayed);
		}
	}

	public class DUPLEX_DIR_STRUCT
	{
		public sbyte	bUse;
		public string	sPrimary;
		public string 	sSecondary;
	} 
}
