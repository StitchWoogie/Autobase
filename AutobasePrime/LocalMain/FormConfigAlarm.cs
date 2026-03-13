using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;
using GraphicModule;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormConfigAlarm.
	/// </summary>
	public class FormConfigAlarm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxProtectAll;
		private System.Windows.Forms.RadioButton radioButtonSoundType0;
		private System.Windows.Forms.RadioButton radioButtonSoundType1;
		private System.Windows.Forms.TextBox textBoxWaveFile;
		private System.Windows.Forms.Button buttonWaveFile;
		private System.Windows.Forms.CheckBox checkBoxGraphicDisplay;
		private System.Windows.Forms.CheckBox checkBoxAutoDisplayEventWindow;
		private System.Windows.Forms.CheckBox checkBoxSortingEventWindow;
		private System.Windows.Forms.CheckBox checkBoxAlarmScreen;
		private System.Windows.Forms.NumericUpDown numericUpDownScreenTime;
		private System.Windows.Forms.NumericUpDown numericUpDownWaitTimeHandOperation;
		private System.Windows.Forms.TextBox textBoxDigitalOut;
		private System.Windows.Forms.Button buttonDigitalOut;
		private System.Windows.Forms.RadioButton radioButtonFilter0;
		private System.Windows.Forms.RadioButton radioButtonFilter1;
		private System.Windows.Forms.CheckBox checkBoxFilterBit0;
		private System.Windows.Forms.CheckBox checkBoxFilterBit1;
		private System.Windows.Forms.CheckBox checkBoxFilterBit2;
		private System.Windows.Forms.CheckBox checkBoxFilterBit3;
		private System.Windows.Forms.CheckBox checkBoxFilterBit4;
		private System.Windows.Forms.CheckBox checkBoxFilterBit5;
		private System.Windows.Forms.CheckBox checkBoxFilterBit6;
		private System.Windows.Forms.CheckBox checkBoxFilterBit7;
		private System.Windows.Forms.CheckBox checkBoxFilterBit8;
		private System.Windows.Forms.CheckBox checkBoxFilterBit9;
		private System.Windows.Forms.CheckBox checkBoxFilterBit10;
		private System.Windows.Forms.RadioButton radioButtonFilter4;
		private System.Windows.Forms.RadioButton radioButtonFilter3;
		private System.Windows.Forms.RadioButton radioButtonFilter2;
		private System.Windows.Forms.GroupBox groupBox7;
        private CheckBox checkBoxSaveAsCsvFormat;
        private CheckBox checkBoxSoundFlag;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBoxCsvItem;
        private CheckBox checkBoxCsvSavePort;
        private CheckBox checkBoxCsvSavePriority;
        private CheckBox checkBoxCsvSaveAlarmType;
        private CheckBox checkBoxCsvSaveMsg;
        private CheckBox checkBoxCsvSaveDescription;
        private CheckBox checkBoxCsvSaveTag;
        private CheckBox checkBoxCsvSaveMilliSecond;
        private CheckBox checkBoxCsvSaveType;
        private CheckBox checkBoxCsvSaveAddress;
        private CheckBox checkBoxCsvSaveStation;
        private Button buttonCsvTargetFolder;
        private TextBox textBoxCsvTargetFolder;
        private CheckBox checkBoxSpecifyTargetFolder;
        private GroupBox groupBoxCsvTargetFolder;
        private CheckBox checkBoxEnableContextMenuOnAlarmEvent;
        private TabPage tabPage3;
        private CheckBox checkBoxMailActive;
        private TextBox textBoxMailServer;
        private Label label6;
        private Button buttonMailTest;
        private TextBox textBoxMailFrom;
        private Label label10;
        private TextBox textBoxMailTo;
        private Label label9;
        private CheckBox checkBoxMailSSL;
        private TextBox textBoxMailPassword;
        private Label label8;
        private TextBox textBoxMailUsername;
        private Label label7;
        private GroupBox groupBoxMail;
        private RadioButton radioButtonFilter5;
        private Label label12;
        private NumericUpDown numericUpDownMailSendingInternal;
        private Label label11;
        private TabPage tabPage4;
        private CheckBox checkBoxSaveRemoteControlResult;
        private GroupBox groupBox8;
        private CheckBox checkBoxFilterBit12;
        private CheckBox checkBoxShowUserManualControl;
        private TextBox textBoxMailPort;
        private Label labelPort;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigAlarm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigAlarm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxWaveFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonSoundType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSoundType0 = new System.Windows.Forms.RadioButton();
            this.checkBoxSoundFlag = new System.Windows.Forms.CheckBox();
            this.buttonWaveFile = new System.Windows.Forms.Button();
            this.checkBoxProtectAll = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableContextMenuOnAlarmEvent = new System.Windows.Forms.CheckBox();
            this.checkBoxSortingEventWindow = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoDisplayEventWindow = new System.Windows.Forms.CheckBox();
            this.checkBoxGraphicDisplay = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveAsCsvFormat = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownScreenTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxAlarmScreen = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownWaitTimeHandOperation = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxDigitalOut = new System.Windows.Forms.TextBox();
            this.buttonDigitalOut = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radioButtonFilter5 = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkBoxFilterBit12 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit5 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit2 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit9 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit6 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit1 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit0 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit3 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit4 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit10 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit7 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterBit8 = new System.Windows.Forms.CheckBox();
            this.radioButtonFilter4 = new System.Windows.Forms.RadioButton();
            this.radioButtonFilter3 = new System.Windows.Forms.RadioButton();
            this.radioButtonFilter2 = new System.Windows.Forms.RadioButton();
            this.radioButtonFilter1 = new System.Windows.Forms.RadioButton();
            this.radioButtonFilter0 = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBoxCsvTargetFolder = new System.Windows.Forms.GroupBox();
            this.checkBoxSpecifyTargetFolder = new System.Windows.Forms.CheckBox();
            this.buttonCsvTargetFolder = new System.Windows.Forms.Button();
            this.textBoxCsvTargetFolder = new System.Windows.Forms.TextBox();
            this.groupBoxCsvItem = new System.Windows.Forms.GroupBox();
            this.checkBoxCsvSaveType = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveAddress = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveStation = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSavePort = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSavePriority = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveAlarmType = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveMsg = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveDescription = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveTag = new System.Windows.Forms.CheckBox();
            this.checkBoxCsvSaveMilliSecond = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBoxMail = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownMailSendingInternal = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonMailTest = new System.Windows.Forms.Button();
            this.textBoxMailServer = new System.Windows.Forms.TextBox();
            this.textBoxMailFrom = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxMailUsername = new System.Windows.Forms.TextBox();
            this.textBoxMailTo = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxMailPassword = new System.Windows.Forms.TextBox();
            this.checkBoxMailSSL = new System.Windows.Forms.CheckBox();
            this.checkBoxMailActive = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxShowUserManualControl = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveRemoteControlResult = new System.Windows.Forms.CheckBox();
            this.textBoxMailPort = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenTime)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWaitTimeHandOperation)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBoxCsvTargetFolder.SuspendLayout();
            this.groupBoxCsvItem.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBoxMail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMailSendingInternal)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox8.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxWaveFile);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButtonSoundType1);
            this.groupBox1.Controls.Add(this.radioButtonSoundType0);
            this.groupBox1.Controls.Add(this.checkBoxSoundFlag);
            this.groupBox1.Controls.Add(this.buttonWaveFile);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxWaveFile
            // 
            resources.ApplyResources(this.textBoxWaveFile, "textBoxWaveFile");
            this.textBoxWaveFile.Name = "textBoxWaveFile";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButtonSoundType1
            // 
            resources.ApplyResources(this.radioButtonSoundType1, "radioButtonSoundType1");
            this.radioButtonSoundType1.Name = "radioButtonSoundType1";
            this.radioButtonSoundType1.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButtonSoundType0
            // 
            resources.ApplyResources(this.radioButtonSoundType0, "radioButtonSoundType0");
            this.radioButtonSoundType0.Name = "radioButtonSoundType0";
            // 
            // checkBoxSoundFlag
            // 
            resources.ApplyResources(this.checkBoxSoundFlag, "checkBoxSoundFlag");
            this.checkBoxSoundFlag.Name = "checkBoxSoundFlag";
            // 
            // buttonWaveFile
            // 
            resources.ApplyResources(this.buttonWaveFile, "buttonWaveFile");
            this.buttonWaveFile.Name = "buttonWaveFile";
            this.buttonWaveFile.Click += new System.EventHandler(this.buttonWaveFile_Click);
            // 
            // checkBoxProtectAll
            // 
            resources.ApplyResources(this.checkBoxProtectAll, "checkBoxProtectAll");
            this.checkBoxProtectAll.Name = "checkBoxProtectAll";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxEnableContextMenuOnAlarmEvent);
            this.groupBox2.Controls.Add(this.checkBoxSortingEventWindow);
            this.groupBox2.Controls.Add(this.checkBoxAutoDisplayEventWindow);
            this.groupBox2.Controls.Add(this.checkBoxGraphicDisplay);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxEnableContextMenuOnAlarmEvent
            // 
            resources.ApplyResources(this.checkBoxEnableContextMenuOnAlarmEvent, "checkBoxEnableContextMenuOnAlarmEvent");
            this.checkBoxEnableContextMenuOnAlarmEvent.Name = "checkBoxEnableContextMenuOnAlarmEvent";
            // 
            // checkBoxSortingEventWindow
            // 
            resources.ApplyResources(this.checkBoxSortingEventWindow, "checkBoxSortingEventWindow");
            this.checkBoxSortingEventWindow.Name = "checkBoxSortingEventWindow";
            // 
            // checkBoxAutoDisplayEventWindow
            // 
            resources.ApplyResources(this.checkBoxAutoDisplayEventWindow, "checkBoxAutoDisplayEventWindow");
            this.checkBoxAutoDisplayEventWindow.Name = "checkBoxAutoDisplayEventWindow";
            // 
            // checkBoxGraphicDisplay
            // 
            resources.ApplyResources(this.checkBoxGraphicDisplay, "checkBoxGraphicDisplay");
            this.checkBoxGraphicDisplay.Name = "checkBoxGraphicDisplay";
            // 
            // checkBoxSaveAsCsvFormat
            // 
            resources.ApplyResources(this.checkBoxSaveAsCsvFormat, "checkBoxSaveAsCsvFormat");
            this.checkBoxSaveAsCsvFormat.Name = "checkBoxSaveAsCsvFormat";
            this.checkBoxSaveAsCsvFormat.CheckedChanged += new System.EventHandler(this.checkBoxSaveAsCsvFormat_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownScreenTime);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.checkBoxAlarmScreen);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownScreenTime
            // 
            resources.ApplyResources(this.numericUpDownScreenTime, "numericUpDownScreenTime");
            this.numericUpDownScreenTime.Name = "numericUpDownScreenTime";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // checkBoxAlarmScreen
            // 
            resources.ApplyResources(this.checkBoxAlarmScreen, "checkBoxAlarmScreen");
            this.checkBoxAlarmScreen.Name = "checkBoxAlarmScreen";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.numericUpDownWaitTimeHandOperation);
            this.groupBox4.Controls.Add(this.label5);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownWaitTimeHandOperation
            // 
            resources.ApplyResources(this.numericUpDownWaitTimeHandOperation, "numericUpDownWaitTimeHandOperation");
            this.numericUpDownWaitTimeHandOperation.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownWaitTimeHandOperation.Name = "numericUpDownWaitTimeHandOperation";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxDigitalOut);
            this.groupBox5.Controls.Add(this.buttonDigitalOut);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // textBoxDigitalOut
            // 
            resources.ApplyResources(this.textBoxDigitalOut, "textBoxDigitalOut");
            this.textBoxDigitalOut.Name = "textBoxDigitalOut";
            // 
            // buttonDigitalOut
            // 
            resources.ApplyResources(this.buttonDigitalOut, "buttonDigitalOut");
            this.buttonDigitalOut.Name = "buttonDigitalOut";
            this.buttonDigitalOut.Click += new System.EventHandler(this.buttonDigitalOut_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radioButtonFilter5);
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Controls.Add(this.radioButtonFilter4);
            this.groupBox6.Controls.Add(this.radioButtonFilter3);
            this.groupBox6.Controls.Add(this.radioButtonFilter2);
            this.groupBox6.Controls.Add(this.radioButtonFilter1);
            this.groupBox6.Controls.Add(this.radioButtonFilter0);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // radioButtonFilter5
            // 
            resources.ApplyResources(this.radioButtonFilter5, "radioButtonFilter5");
            this.radioButtonFilter5.Name = "radioButtonFilter5";
            this.radioButtonFilter5.CheckedChanged += new System.EventHandler(this.radioButtonFilter5_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.checkBoxFilterBit12);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit5);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit2);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit9);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit6);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit1);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit0);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit3);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit4);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit10);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit7);
            this.groupBox7.Controls.Add(this.checkBoxFilterBit8);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // checkBoxFilterBit12
            // 
            resources.ApplyResources(this.checkBoxFilterBit12, "checkBoxFilterBit12");
            this.checkBoxFilterBit12.Name = "checkBoxFilterBit12";
            this.checkBoxFilterBit12.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit12_CheckedChanged);
            // 
            // checkBoxFilterBit5
            // 
            resources.ApplyResources(this.checkBoxFilterBit5, "checkBoxFilterBit5");
            this.checkBoxFilterBit5.Name = "checkBoxFilterBit5";
            this.checkBoxFilterBit5.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit5_CheckedChanged);
            // 
            // checkBoxFilterBit2
            // 
            resources.ApplyResources(this.checkBoxFilterBit2, "checkBoxFilterBit2");
            this.checkBoxFilterBit2.Name = "checkBoxFilterBit2";
            this.checkBoxFilterBit2.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit2_CheckedChanged);
            // 
            // checkBoxFilterBit9
            // 
            resources.ApplyResources(this.checkBoxFilterBit9, "checkBoxFilterBit9");
            this.checkBoxFilterBit9.Name = "checkBoxFilterBit9";
            this.checkBoxFilterBit9.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit9_CheckedChanged);
            // 
            // checkBoxFilterBit6
            // 
            resources.ApplyResources(this.checkBoxFilterBit6, "checkBoxFilterBit6");
            this.checkBoxFilterBit6.Name = "checkBoxFilterBit6";
            this.checkBoxFilterBit6.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit6_CheckedChanged);
            // 
            // checkBoxFilterBit1
            // 
            resources.ApplyResources(this.checkBoxFilterBit1, "checkBoxFilterBit1");
            this.checkBoxFilterBit1.Name = "checkBoxFilterBit1";
            this.checkBoxFilterBit1.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit1_CheckedChanged);
            // 
            // checkBoxFilterBit0
            // 
            resources.ApplyResources(this.checkBoxFilterBit0, "checkBoxFilterBit0");
            this.checkBoxFilterBit0.Name = "checkBoxFilterBit0";
            this.checkBoxFilterBit0.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit0_CheckedChanged);
            // 
            // checkBoxFilterBit3
            // 
            resources.ApplyResources(this.checkBoxFilterBit3, "checkBoxFilterBit3");
            this.checkBoxFilterBit3.Name = "checkBoxFilterBit3";
            this.checkBoxFilterBit3.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit3_CheckedChanged);
            // 
            // checkBoxFilterBit4
            // 
            resources.ApplyResources(this.checkBoxFilterBit4, "checkBoxFilterBit4");
            this.checkBoxFilterBit4.Name = "checkBoxFilterBit4";
            this.checkBoxFilterBit4.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit4_CheckedChanged);
            // 
            // checkBoxFilterBit10
            // 
            resources.ApplyResources(this.checkBoxFilterBit10, "checkBoxFilterBit10");
            this.checkBoxFilterBit10.Name = "checkBoxFilterBit10";
            this.checkBoxFilterBit10.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit10_CheckedChanged);
            // 
            // checkBoxFilterBit7
            // 
            resources.ApplyResources(this.checkBoxFilterBit7, "checkBoxFilterBit7");
            this.checkBoxFilterBit7.Name = "checkBoxFilterBit7";
            this.checkBoxFilterBit7.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit7_CheckedChanged);
            // 
            // checkBoxFilterBit8
            // 
            resources.ApplyResources(this.checkBoxFilterBit8, "checkBoxFilterBit8");
            this.checkBoxFilterBit8.Name = "checkBoxFilterBit8";
            this.checkBoxFilterBit8.CheckedChanged += new System.EventHandler(this.checkBoxFilterBit8_CheckedChanged);
            // 
            // radioButtonFilter4
            // 
            resources.ApplyResources(this.radioButtonFilter4, "radioButtonFilter4");
            this.radioButtonFilter4.Name = "radioButtonFilter4";
            this.radioButtonFilter4.CheckedChanged += new System.EventHandler(this.radioButtonFilter4_CheckedChanged_1);
            // 
            // radioButtonFilter3
            // 
            resources.ApplyResources(this.radioButtonFilter3, "radioButtonFilter3");
            this.radioButtonFilter3.Name = "radioButtonFilter3";
            this.radioButtonFilter3.CheckedChanged += new System.EventHandler(this.radioButtonFilter3_CheckedChanged_1);
            // 
            // radioButtonFilter2
            // 
            resources.ApplyResources(this.radioButtonFilter2, "radioButtonFilter2");
            this.radioButtonFilter2.Name = "radioButtonFilter2";
            this.radioButtonFilter2.CheckedChanged += new System.EventHandler(this.radioButtonFilter2_CheckedChanged);
            // 
            // radioButtonFilter1
            // 
            resources.ApplyResources(this.radioButtonFilter1, "radioButtonFilter1");
            this.radioButtonFilter1.Name = "radioButtonFilter1";
            this.radioButtonFilter1.CheckedChanged += new System.EventHandler(this.radioButtonFilter1_CheckedChanged);
            // 
            // radioButtonFilter0
            // 
            resources.ApplyResources(this.radioButtonFilter0, "radioButtonFilter0");
            this.radioButtonFilter0.Name = "radioButtonFilter0";
            this.radioButtonFilter0.CheckedChanged += new System.EventHandler(this.radioButtonFilter0_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxProtectAll);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBoxCsvTargetFolder);
            this.tabPage2.Controls.Add(this.groupBoxCsvItem);
            this.tabPage2.Controls.Add(this.checkBoxSaveAsCsvFormat);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            // groupBoxCsvItem
            // 
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveType);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveAddress);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveStation);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSavePort);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSavePriority);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveAlarmType);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveMsg);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveDescription);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveTag);
            this.groupBoxCsvItem.Controls.Add(this.checkBoxCsvSaveMilliSecond);
            resources.ApplyResources(this.groupBoxCsvItem, "groupBoxCsvItem");
            this.groupBoxCsvItem.Name = "groupBoxCsvItem";
            this.groupBoxCsvItem.TabStop = false;
            // 
            // checkBoxCsvSaveType
            // 
            resources.ApplyResources(this.checkBoxCsvSaveType, "checkBoxCsvSaveType");
            this.checkBoxCsvSaveType.Name = "checkBoxCsvSaveType";
            this.checkBoxCsvSaveType.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveAddress
            // 
            resources.ApplyResources(this.checkBoxCsvSaveAddress, "checkBoxCsvSaveAddress");
            this.checkBoxCsvSaveAddress.Name = "checkBoxCsvSaveAddress";
            this.checkBoxCsvSaveAddress.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveStation
            // 
            resources.ApplyResources(this.checkBoxCsvSaveStation, "checkBoxCsvSaveStation");
            this.checkBoxCsvSaveStation.Name = "checkBoxCsvSaveStation";
            this.checkBoxCsvSaveStation.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSavePort
            // 
            resources.ApplyResources(this.checkBoxCsvSavePort, "checkBoxCsvSavePort");
            this.checkBoxCsvSavePort.Name = "checkBoxCsvSavePort";
            this.checkBoxCsvSavePort.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSavePriority
            // 
            resources.ApplyResources(this.checkBoxCsvSavePriority, "checkBoxCsvSavePriority");
            this.checkBoxCsvSavePriority.Name = "checkBoxCsvSavePriority";
            this.checkBoxCsvSavePriority.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveAlarmType
            // 
            resources.ApplyResources(this.checkBoxCsvSaveAlarmType, "checkBoxCsvSaveAlarmType");
            this.checkBoxCsvSaveAlarmType.Name = "checkBoxCsvSaveAlarmType";
            this.checkBoxCsvSaveAlarmType.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveMsg
            // 
            resources.ApplyResources(this.checkBoxCsvSaveMsg, "checkBoxCsvSaveMsg");
            this.checkBoxCsvSaveMsg.Name = "checkBoxCsvSaveMsg";
            this.checkBoxCsvSaveMsg.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveDescription
            // 
            resources.ApplyResources(this.checkBoxCsvSaveDescription, "checkBoxCsvSaveDescription");
            this.checkBoxCsvSaveDescription.Name = "checkBoxCsvSaveDescription";
            this.checkBoxCsvSaveDescription.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveTag
            // 
            resources.ApplyResources(this.checkBoxCsvSaveTag, "checkBoxCsvSaveTag");
            this.checkBoxCsvSaveTag.Name = "checkBoxCsvSaveTag";
            this.checkBoxCsvSaveTag.UseVisualStyleBackColor = true;
            // 
            // checkBoxCsvSaveMilliSecond
            // 
            resources.ApplyResources(this.checkBoxCsvSaveMilliSecond, "checkBoxCsvSaveMilliSecond");
            this.checkBoxCsvSaveMilliSecond.Name = "checkBoxCsvSaveMilliSecond";
            this.checkBoxCsvSaveMilliSecond.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBoxMail);
            this.tabPage3.Controls.Add(this.checkBoxMailActive);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBoxMail
            // 
            this.groupBoxMail.Controls.Add(this.textBoxMailPort);
            this.groupBoxMail.Controls.Add(this.labelPort);
            this.groupBoxMail.Controls.Add(this.label12);
            this.groupBoxMail.Controls.Add(this.numericUpDownMailSendingInternal);
            this.groupBoxMail.Controls.Add(this.label11);
            this.groupBoxMail.Controls.Add(this.label6);
            this.groupBoxMail.Controls.Add(this.buttonMailTest);
            this.groupBoxMail.Controls.Add(this.textBoxMailServer);
            this.groupBoxMail.Controls.Add(this.textBoxMailFrom);
            this.groupBoxMail.Controls.Add(this.label7);
            this.groupBoxMail.Controls.Add(this.label10);
            this.groupBoxMail.Controls.Add(this.textBoxMailUsername);
            this.groupBoxMail.Controls.Add(this.textBoxMailTo);
            this.groupBoxMail.Controls.Add(this.label8);
            this.groupBoxMail.Controls.Add(this.label9);
            this.groupBoxMail.Controls.Add(this.textBoxMailPassword);
            this.groupBoxMail.Controls.Add(this.checkBoxMailSSL);
            resources.ApplyResources(this.groupBoxMail, "groupBoxMail");
            this.groupBoxMail.Name = "groupBoxMail";
            this.groupBoxMail.TabStop = false;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // numericUpDownMailSendingInternal
            // 
            resources.ApplyResources(this.numericUpDownMailSendingInternal, "numericUpDownMailSendingInternal");
            this.numericUpDownMailSendingInternal.Name = "numericUpDownMailSendingInternal";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // buttonMailTest
            // 
            resources.ApplyResources(this.buttonMailTest, "buttonMailTest");
            this.buttonMailTest.Name = "buttonMailTest";
            this.buttonMailTest.UseVisualStyleBackColor = true;
            this.buttonMailTest.Click += new System.EventHandler(this.buttonMailTest_Click);
            // 
            // textBoxMailServer
            // 
            resources.ApplyResources(this.textBoxMailServer, "textBoxMailServer");
            this.textBoxMailServer.Name = "textBoxMailServer";
            // 
            // textBoxMailFrom
            // 
            resources.ApplyResources(this.textBoxMailFrom, "textBoxMailFrom");
            this.textBoxMailFrom.Name = "textBoxMailFrom";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // textBoxMailUsername
            // 
            resources.ApplyResources(this.textBoxMailUsername, "textBoxMailUsername");
            this.textBoxMailUsername.Name = "textBoxMailUsername";
            // 
            // textBoxMailTo
            // 
            resources.ApplyResources(this.textBoxMailTo, "textBoxMailTo");
            this.textBoxMailTo.Name = "textBoxMailTo";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // textBoxMailPassword
            // 
            resources.ApplyResources(this.textBoxMailPassword, "textBoxMailPassword");
            this.textBoxMailPassword.Name = "textBoxMailPassword";
            this.textBoxMailPassword.TextChanged += new System.EventHandler(this.textBoxMailPassword_TextChanged);
            // 
            // checkBoxMailSSL
            // 
            resources.ApplyResources(this.checkBoxMailSSL, "checkBoxMailSSL");
            this.checkBoxMailSSL.Name = "checkBoxMailSSL";
            this.checkBoxMailSSL.UseVisualStyleBackColor = true;
            // 
            // checkBoxMailActive
            // 
            resources.ApplyResources(this.checkBoxMailActive, "checkBoxMailActive");
            this.checkBoxMailActive.Name = "checkBoxMailActive";
            this.checkBoxMailActive.CheckedChanged += new System.EventHandler(this.checkBoxMailActive_CheckedChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox8);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.checkBoxShowUserManualControl);
            this.groupBox8.Controls.Add(this.checkBoxSaveRemoteControlResult);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // checkBoxShowUserManualControl
            // 
            resources.ApplyResources(this.checkBoxShowUserManualControl, "checkBoxShowUserManualControl");
            this.checkBoxShowUserManualControl.Name = "checkBoxShowUserManualControl";
            this.checkBoxShowUserManualControl.UseVisualStyleBackColor = true;
            // 
            // checkBoxSaveRemoteControlResult
            // 
            resources.ApplyResources(this.checkBoxSaveRemoteControlResult, "checkBoxSaveRemoteControlResult");
            this.checkBoxSaveRemoteControlResult.Name = "checkBoxSaveRemoteControlResult";
            // 
            // textBoxMailPort
            // 
            resources.ApplyResources(this.textBoxMailPort, "textBoxMailPort");
            this.textBoxMailPort.Name = "textBoxMailPort";
            // 
            // labelPort
            // 
            resources.ApplyResources(this.labelPort, "labelPort");
            this.labelPort.Name = "labelPort";
            // 
            // FormConfigAlarm
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
            this.Name = "FormConfigAlarm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigAlarm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScreenTime)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWaitTimeHandOperation)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBoxCsvTargetFolder.ResumeLayout(false);
            this.groupBoxCsvTargetFolder.PerformLayout();
            this.groupBoxCsvItem.ResumeLayout(false);
            this.groupBoxCsvItem.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBoxMail.ResumeLayout(false);
            this.groupBoxMail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMailSendingInternal)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void radioButton2_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		uint[] tempFlag = new uint[6];

        bool bPageLoading = false;

		private void FormConfigAlarm_Load(object sender, System.EventArgs e)
		{
            bPageLoading = true;

			this.checkBoxProtectAll.Checked = ConfigAlarm.bAlarmProtectAll;
			this.checkBoxSoundFlag.Checked = ConfigAlarm.bAlarmSoundFlag;
			this.radioButtonSoundType0.Checked = ConfigAlarm.cAlarmSoundType == 0;
			this.radioButtonSoundType1.Checked = ConfigAlarm.cAlarmSoundType == 1;
			this.textBoxWaveFile.Text = ConfigAlarm.sAlarmWaveFile;
			this.checkBoxAlarmScreen.Checked = ConfigAlarm.bAlarmScreenFlag;
			this.numericUpDownScreenTime.Value = ConfigAlarm.nAlarmScreenTime;
			this.checkBoxAutoDisplayEventWindow.Checked = ConfigAlarm.bAlarmAutoMakeConfirmBox;
			tempFlag[0] = ConfigAlarm.dwAlarmFilterEvent;
			tempFlag[1] = ConfigAlarm.dwAlarmFilterLinePrinter;
			tempFlag[2] = ConfigAlarm.dwAlarmFilterSound;
			tempFlag[3] = ConfigAlarm.dwAlarmFilterFile;
			tempFlag[4] = ConfigAlarm.dwAlarmFilterSmsManager;
            tempFlag[5] = ConfigAlarm.dwAlarmFilterMail;
			this.checkBoxSortingEventWindow.Checked = ConfigAlarm.bAlarmConfirmSorting;
			this.checkBoxGraphicDisplay.Checked = ConfigAlarm.bDisplayGraphicFileOnAlarm;
            Tools.SetNumericUpDownValue(this.numericUpDownWaitTimeHandOperation, ConfigAlarm.nWaitAlarmAfterHandOper);
			this.textBoxDigitalOut.Text = ConfigAlarm.sAlarmDigitalOut;
            this.checkBoxEnableContextMenuOnAlarmEvent.Checked = ConfigAlarm.bEnableContextMenuOnAlarmEvent;

            this.checkBoxSaveAsCsvFormat.Checked = ConfigAlarm.bAlarmAlsoSaveAsCsvFormat;

            this.checkBoxCsvSaveAddress.Checked = ConfigAlarm.bCsvSaveAddress;
            this.checkBoxCsvSaveAlarmType.Checked = ConfigAlarm.bCsvSaveAlarmType;
            this.checkBoxCsvSaveDescription.Checked = ConfigAlarm.bCsvSaveDescription;
            this.checkBoxCsvSaveMilliSecond.Checked = ConfigAlarm.bCsvSaveMillisecond;
            this.checkBoxCsvSaveMsg.Checked = ConfigAlarm.bCsvSaveMsg;
            this.checkBoxCsvSavePort.Checked = ConfigAlarm.bCsvSavePort;
            this.checkBoxCsvSavePriority.Checked = ConfigAlarm.bCsvSavePriority;
            this.checkBoxCsvSaveStation.Checked = ConfigAlarm.bCsvSaveStation;
            this.checkBoxCsvSaveTag.Checked = ConfigAlarm.bCsvSaveTag;
            this.checkBoxCsvSaveType.Checked = ConfigAlarm.bCsvSaveAlarmSubType;

            this.checkBoxSpecifyTargetFolder.Checked = ConfigAlarm.bCsvSpecifyTargetFolder;
            this.textBoxCsvTargetFolder.Text = ConfigAlarm.sCsvTargetFolder;

			this.radioButtonFilter0.Checked = true;

            this.checkBoxMailActive.Checked = ConfigAlarm.bMailActive;
            this.textBoxMailServer.Text = ConfigAlarm.sMailServer;
            this.textBoxMailUsername.Text = ConfigAlarm.sMailUsername;
            this.textBoxMailPassword.Text = ConfigAlarm.sMailPassword;
            this.textBoxMailFrom.Text = ConfigAlarm.sMailFrom;
            this.textBoxMailTo.Text = ConfigAlarm.sMailTo;
            this.checkBoxMailSSL.Checked = ConfigAlarm.bMailSSL;
            Tools.SetNumericUpDownValue(this.numericUpDownMailSendingInternal, ConfigAlarm.nMailSendingInverval);
            this.textBoxMailPort.Text = Convert.ToString(ConfigAlarm.nMailPort); //port 인자 추가 20240509 PSU

            this.checkBoxSaveRemoteControlResult.Checked = ConfigAlarm.bSaveRemoteControlResult;
            this.checkBoxShowUserManualControl.Checked = ConfigViewMain.bShowUserManualControl;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ALARM))
				this.buttonOK.Enabled = false;

            EnableCsvItem();
            EnableMail();

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.radioButtonFilter1.Visible = false;
                this.radioButtonFilter2.Visible = false;
                this.radioButtonFilter4.Visible = false;
                // 위치가 구멍나서 이동함
                this.radioButtonFilter3.Location = radioButtonFilter1.Location;
                this.radioButtonFilter5.Location = radioButtonFilter2.Location;

                this.tabControl1.TabPages.Remove(tabPage4);     // 기타 탭

                this.textBoxDigitalOut.ReadOnly = true;
                this.textBoxWaveFile.ReadOnly = true;

                this.checkBoxFilterBit7.Visible = false;
                this.checkBoxFilterBit8.Visible = false;
                this.checkBoxFilterBit9.Visible = false;
                this.checkBoxFilterBit10.Visible = false;

                this.groupBox4.Visible = false;
                this.groupBox5.Location = this.groupBox4.Location;

                if (Tools.IsLangKorean())
                {
                    this.radioButtonSoundType1.Text = "사용자 정의 소리";
                }
            }

            bPageLoading = false;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxMailFrom, 30)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxMailPassword, 60)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxMailServer, 30)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxMailTo, 30)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxMailUsername, 30)) return;

			ConfigAlarm.bAlarmProtectAll = this.checkBoxProtectAll.Checked;
			ConfigAlarm.bAlarmSoundFlag = this.checkBoxSoundFlag.Checked;

			if(this.radioButtonSoundType0.Checked)		ConfigAlarm.cAlarmSoundType = 0;
			else if(this.radioButtonSoundType1.Checked) ConfigAlarm.cAlarmSoundType = 1;
			else										ConfigAlarm.cAlarmSoundType = 0;

			ConfigAlarm.sAlarmWaveFile = this.textBoxWaveFile.Text;
			ConfigAlarm.bAlarmScreenFlag = this.checkBoxAlarmScreen.Checked;
			ConfigAlarm.nAlarmScreenTime = ConvertTool.ToInt32(this.numericUpDownScreenTime.Value);
			ConfigAlarm.bAlarmAutoMakeConfirmBox = this.checkBoxAutoDisplayEventWindow.Checked;
			ConfigAlarm.dwAlarmFilterEvent = tempFlag[0];
			ConfigAlarm.dwAlarmFilterLinePrinter = tempFlag[1];
			ConfigAlarm.dwAlarmFilterSound = tempFlag[2];
			ConfigAlarm.dwAlarmFilterFile = tempFlag[3];
			ConfigAlarm.dwAlarmFilterSmsManager = tempFlag[4];
            ConfigAlarm.dwAlarmFilterMail = tempFlag[5];
			ConfigAlarm.bAlarmConfirmSorting = this.checkBoxSortingEventWindow.Checked;
			ConfigAlarm.bDisplayGraphicFileOnAlarm = this.checkBoxGraphicDisplay.Checked;
			ConfigAlarm.nWaitAlarmAfterHandOper = ConvertTool.ToInt32(this.numericUpDownWaitTimeHandOperation.Value);
			ConfigAlarm.sAlarmDigitalOut = this.textBoxDigitalOut.Text;
            ConfigAlarm.bEnableContextMenuOnAlarmEvent = this.checkBoxEnableContextMenuOnAlarmEvent.Checked;

            ConfigAlarm.bAlarmAlsoSaveAsCsvFormat = this.checkBoxSaveAsCsvFormat.Checked;

            ConfigAlarm.bCsvSaveAddress = this.checkBoxCsvSaveAddress.Checked;
            ConfigAlarm.bCsvSaveAlarmType = this.checkBoxCsvSaveAlarmType.Checked;
            ConfigAlarm.bCsvSaveDescription = this.checkBoxCsvSaveDescription.Checked;
            ConfigAlarm.bCsvSaveMillisecond = this.checkBoxCsvSaveMilliSecond.Checked;
            ConfigAlarm.bCsvSaveMsg = this.checkBoxCsvSaveMsg.Checked;
            ConfigAlarm.bCsvSavePort = this.checkBoxCsvSavePort.Checked;
            ConfigAlarm.bCsvSavePriority = this.checkBoxCsvSavePriority.Checked;
            ConfigAlarm.bCsvSaveStation = this.checkBoxCsvSaveStation.Checked;
            ConfigAlarm.bCsvSaveTag = this.checkBoxCsvSaveTag.Checked;
            ConfigAlarm.bCsvSaveAlarmSubType = this.checkBoxCsvSaveType.Checked;
            ConfigAlarm.bCsvSpecifyTargetFolder = this.checkBoxSpecifyTargetFolder.Checked;
            ConfigAlarm.sCsvTargetFolder = this.textBoxCsvTargetFolder.Text;

            ConfigAlarm.bMailActive = this.checkBoxMailActive.Checked;
            ConfigAlarm.sMailServer = this.textBoxMailServer.Text;
            ConfigAlarm.sMailUsername = this.textBoxMailUsername.Text;
            //ConfigAlarm.sMailPassword = this.textBoxMailPassword.Text;
            ConfigAlarm.sMailFrom = this.textBoxMailFrom.Text;
            ConfigAlarm.sMailTo = this.textBoxMailTo.Text;
            ConfigAlarm.bMailSSL = this.checkBoxMailSSL.Checked;
            string org_password, hash_password;
            GetPassword(out org_password, out hash_password);
            ConfigAlarm.sMailPassword = hash_password;
            ConfigAlarm.nMailSendingInverval = ConvertTool.ToInt32(this.numericUpDownMailSendingInternal.Value);
            ConfigAlarm.nMailPort = ConvertTool.ToInt32(this.textBoxMailPort.Text); //port 인자 추가 20240509 PSU

            ConfigAlarm.bSaveRemoteControlResult = this.checkBoxSaveRemoteControlResult.Checked;
            ConfigViewMain.bShowUserManualControl = this.checkBoxShowUserManualControl.Checked;

			ConfigAlarm.SaveConfig();

            MessageDisplay.nScreenLifeTime = ConfigAlarm.nAlarmScreenTime; // 경보 메시지가 떠 있는 시간을 설정한다

			DialogResult = DialogResult.OK;
			Close();
		}

		void DisplayCheckBit(int pos)
		{
			this.checkBoxFilterBit0.Checked = (tempFlag[pos] & Tools.DWORD_MASK[0]) > 0;
			this.checkBoxFilterBit1.Checked = (tempFlag[pos] & Tools.DWORD_MASK[1]) > 0;
			this.checkBoxFilterBit2.Checked = (tempFlag[pos] & Tools.DWORD_MASK[2]) > 0;
			this.checkBoxFilterBit3.Checked = (tempFlag[pos] & Tools.DWORD_MASK[3]) > 0;
			this.checkBoxFilterBit4.Checked = (tempFlag[pos] & Tools.DWORD_MASK[4]) > 0;
			this.checkBoxFilterBit5.Checked = (tempFlag[pos] & Tools.DWORD_MASK[5]) > 0;
			this.checkBoxFilterBit6.Checked = (tempFlag[pos] & Tools.DWORD_MASK[6]) > 0;
			this.checkBoxFilterBit7.Checked = (tempFlag[pos] & Tools.DWORD_MASK[7]) > 0;
			this.checkBoxFilterBit8.Checked = (tempFlag[pos] & Tools.DWORD_MASK[8]) > 0;
			this.checkBoxFilterBit9.Checked = (tempFlag[pos] & Tools.DWORD_MASK[9]) > 0;
			this.checkBoxFilterBit10.Checked = (tempFlag[pos] & Tools.DWORD_MASK[10]) > 0;
            this.checkBoxFilterBit12.Checked = (tempFlag[pos] & Tools.DWORD_MASK[12]) > 0;
		}

		private void radioButtonFilter0_CheckedChanged(object sender, System.EventArgs e)
		{
			DisplayCheckBit(0);
		}

		private void radioButtonFilter1_CheckedChanged(object sender, System.EventArgs e)
		{
			DisplayCheckBit(1);
		}

		private void radioButtonFilter2_CheckedChanged(object sender, System.EventArgs e)
		{
			DisplayCheckBit(2);
		}

		private void radioButtonFilter3_CheckedChanged_1(object sender, System.EventArgs e)
		{
			DisplayCheckBit(3);
		}

		private void radioButtonFilter4_CheckedChanged_1(object sender, System.EventArgs e)
		{
			DisplayCheckBit(4);
		}

        private void radioButtonFilter5_CheckedChanged(object sender, EventArgs e)
        {
            DisplayCheckBit(5);
        }

		void SaveBit(int bit_pos, CheckBox check)
		{
			int type;
			if(this.radioButtonFilter0.Checked)			type = 0;
			else if(this.radioButtonFilter1.Checked)	type = 1;
			else if(this.radioButtonFilter2.Checked)	type = 2;
			else if(this.radioButtonFilter3.Checked)	type = 3;
			else if(this.radioButtonFilter4.Checked)	type = 4;
            else if (this.radioButtonFilter5.Checked) type = 5;
			else										type = 0;

			if(check.Checked)	tempFlag[type] |= Tools.DWORD_MASK[bit_pos];
			else				tempFlag[type] &= 0xFFFFFFFF-Tools.DWORD_MASK[bit_pos];
		}

		private void checkBoxFilterBit0_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(0, (CheckBox)sender);
		}

		private void checkBoxFilterBit1_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(1, (CheckBox)sender);
		}

		private void checkBoxFilterBit2_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(2, (CheckBox)sender);
		}

		private void checkBoxFilterBit3_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(3, (CheckBox)sender);
		}

		private void checkBoxFilterBit4_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(4, (CheckBox)sender);
		}

		private void checkBoxFilterBit5_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(5, (CheckBox)sender);
		}

		private void checkBoxFilterBit6_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(6, (CheckBox)sender);
		}

		private void checkBoxFilterBit7_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(7, (CheckBox)sender);
		}

		private void checkBoxFilterBit8_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(8, (CheckBox)sender);
		}

		private void checkBoxFilterBit9_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(9, (CheckBox)sender);
		}

		private void checkBoxFilterBit10_CheckedChanged(object sender, System.EventArgs e)
		{
			SaveBit(10, (CheckBox)sender);
		}

        // Bit 11은 노멀이다.
        private void checkBoxFilterBit12_CheckedChanged(object sender, EventArgs e)
        {
            SaveBit(12, (CheckBox)sender);
        }

		private void buttonWaveFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Wave Files (*.wav)|*.wav";

			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				this.textBoxWaveFile.Text = dialog.FileName;
			}
		}

		private void buttonDigitalOut_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(DialogTag.SelectTag.SelectDo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxDigitalOut.Text = tag;
			}
		}

        void EnableCsvItem()
        {
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.textBoxCsvTargetFolder.ReadOnly = true;
            }
            else
            {
                this.textBoxCsvTargetFolder.Enabled = this.checkBoxSpecifyTargetFolder.Checked;
            }

            this.buttonCsvTargetFolder.Enabled = this.checkBoxSpecifyTargetFolder.Checked;

            this.groupBoxCsvItem.Enabled = this.checkBoxSaveAsCsvFormat.Checked;
            this.groupBoxCsvTargetFolder.Enabled = this.checkBoxSaveAsCsvFormat.Checked;
        }

        private void checkBoxSaveAsCsvFormat_CheckedChanged(object sender, EventArgs e)
        {
            EnableCsvItem();
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

        private void checkBoxSpecifyTargetFolder_CheckedChanged(object sender, EventArgs e)
        {
            EnableCsvItem();
        }

        void EnableMail()
        {
            this.groupBoxMail.Enabled = this.checkBoxMailActive.Checked;
        }

        private void checkBoxMailActive_CheckedChanged(object sender, EventArgs e)
        {
            EnableMail();
        }

        bool CheckMailInvalidate()
        {
            if (!this.checkBoxMailActive.Checked) return true;   // Mail을 사용하지 않으면 체크할 필요가 없다.

            if (this.textBoxMailServer.Text.Length == 0)
            {
                MessageBox.Show("Input the Mail Server", "Input error");
                return false;
            }

            if (this.textBoxMailFrom.Text.Length == 0)
            {
                MessageBox.Show("Input the Mail From", "Input error");
                return false;
            }

            if (this.textBoxMailTo.Text.Length == 0)
            {
                MessageBox.Show("Input the Mail To", "Input error");
                return false;
            }

            return true;
        }

        void GetPassword(out string org_password, out string hash_password)
        {
            string seed = System.Environment.MachineName + System.Environment.UserName;

            org_password = this.textBoxMailPassword.Text;
            hash_password = org_password;

            if (bModifiedPassword)
            {
                hash_password = WebTools.StringHash.Encode(org_password, seed);
            }
            else
            {
                org_password = WebTools.StringHash.Decode(hash_password, seed);
            }
        }

        private void buttonMailTest_Click(object sender, EventArgs e)
        {
            string err_msg;

            if(!CheckMailInvalidate())  return;

            string server = this.textBoxMailServer.Text;
            string username = this.textBoxMailUsername.Text;
            string mail_to = this.textBoxMailTo.Text;
            string mail_from = this.textBoxMailFrom.Text;
            bool ssl = this.checkBoxMailSSL.Checked;
            int port = ConvertTool.ToInt32(this.textBoxMailPort.Text);

            string text = String.Format("Test Mail\nTest Time={0}", DateTime.Now);

            string password;
            string hash_password;

            GetPassword(out password, out hash_password);

            if (ScriptFunctionMail.SendMail(server, username, password, ssl, mail_to, mail_from, "Test Mail", text, false, "", port, out err_msg))
            {
                MessageBox.Show("Mail test succeeded.", "O.K");
            }
            else
            {
                MessageBox.Show(err_msg, "Mail test failed");
            }
        }

        bool bModifiedPassword = false;

        private void textBoxMailPassword_TextChanged(object sender, EventArgs e)
        {
            if (bPageLoading) return;

            bModifiedPassword = true;
        }

        

        
		
	}

	
}
