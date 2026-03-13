using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Drawing.Printing;
using RunMain;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormConfigEtc.
	/// </summary>
	public class FormConfigEtc : System.Windows.Forms.Form
	{
		private System.Windows.Forms.CheckBox checkBoxAiCalcSameFormat;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxMatchAlarmReturnGabAndHiHiLoLoDO;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownScanStart;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBoxContinueScriptOnError;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.CheckBox checkBoxTagInfoString;
		private System.Windows.Forms.CheckBox checkBoxFitWindowSize;
		private System.Windows.Forms.CheckBox checkBoxLeftMouseResponse;
		private System.Windows.Forms.CheckBox checkBoxRightMouseResponse;
		private System.Windows.Forms.CheckBox checkBoxGraphicMenuButton;
		private System.Windows.Forms.NumericUpDown numericUpDownMaxModule;
		private System.Windows.Forms.CheckBox checkBoxScanPause;
		private System.Windows.Forms.CheckBox checkBoxExitWithPlcScan;
		private System.Windows.Forms.CheckBox checkBoxEndPrompt;
		private System.Windows.Forms.CheckBox checkBoxPlcScanWriteTest;
		private System.Windows.Forms.CheckBox checkBoxWriteAiSubWhenStart;
		private System.Windows.Forms.CheckBox checkBoxWriteDiSubWhenStart;
		private System.Windows.Forms.NumericUpDown numericUpDownForLoopTimeout;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBoxPrinter;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownMaxBasicScreen;
        private CheckBox checkBoxUseNotifyIcon;
        private CheckBox checkBoxUseTestModeMessage;
        private TabPage tabPage3;
        private CheckBox checkBoxUseAnalogModule;
        private TextBox textBoxAnalogModule;
        private Label label8;
        private CheckBox checkBoxUseStringModule;
        private TextBox textBoxStringModule;
        private Label label7;
        private CheckBox checkBoxUseDigitalModule;
        private TextBox textBoxDigitalModule;
        private Label label6;
        private CheckBox checkBoxUseDeviceQuality;
        private Button buttonAnalogModule;
        private Button buttonStringModule;
        private Button buttonDigitalModule;
        private TabPage tabPage4;
        private CheckBox checkBoxCloseOpcServerWhenLocalMainExit;
        private CheckBox checkBoxStartOpcServerWhileLocalMainRunning;
        private TabPage tabPage5;
        private GroupBox groupBox5;
        private GroupBox groupBox4;
        private CheckBox checkBoxOpcServerUseRootItem;
        private NumericUpDown numericUpDownOpcServerSubscriptionRefreshCount;
        private Label label10;
        private TextBox textBoxOpcServerTagGroupNameToUse;
        private Label label9;
        private NumericUpDown numericUpDownReadInterval;
        private Label label11;
        private TabPage tabPage6;
        private GroupBox groupBox7;
        private GroupBox groupBox6;
        private CheckBox checkBoxRightMouseOnObject;
        private CheckBox checkBoxRightMouseGraphicContextMenu;
        private TextBox textBoxOpcServerGroupSerapatorChar;
        private Label label12;
        private TabPage tabPage7;
        private CheckBox checkBoxOnOffListAddOneSecondToOperationTime;
        private CheckBox checkBoxDisplayMouseZoneWhenSameTagSelected;
        private CheckBox checkBoxUseProtectMenu;
        private TabPage tabPage8;
        private LocalMain.FontsAndColors.UserControlFontsAndColors userControlFontsAndColors1;
        private CheckBox checkBoxUseAlarmServer;
        private TabPage tabPage9;
        private Label label13;
        private NumericUpDown numericUpDownPreviewOutputResult;
        private GroupBox groupBox8;
        private CheckBox checkBoxUsePreviewOutputResult;
        private CheckBox checkBoxRestoreLocationSizeOnStartup;
        private CheckBox checkBoxUseOpcServerItemSecurity;
        private CheckBox checkBoxRemoveFlashingWhenActivatingMdiModule;
        private CheckBox checkBoxTagValueShareBySharedMemory;
        private CheckBox checkBoxUseAutomaticFileRecovery;
        private CheckBox checkBoxScriptErrorMessageTagNotFound;
        private CheckBox checkBoxShowScriptErrorMessage;
        private CheckBox checkBoxScriptErrorMessageElse;
        private NumericUpDown numericUpDownMaxReport;
        private Label label14;
        private CheckBox checkBoxAlwaysOpenNewReport;
        private Button buttonCheckMonitor;
        private ComboBox comboBoxStartMonitorIndex;
        private Label label15;
        private CheckBox checkBoxEnableBatchProcessing;
        private NumericUpDown numericUpDownBatchSize;
        private GroupBox groupBox9;
        private ListBox m_list;
        private Label label16;
        private CheckBox checkBoxExitWithUaClient;
        private GroupBox groupBox10;
        private CheckBox checkBoxEnableMaintimerProfile;
        private NumericUpDown numThresholdDelay;
        private Label label18;
        private Label label17;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

		public FormConfigEtc()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigEtc));
            this.checkBoxAiCalcSameFormat = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxMatchAlarmReturnGabAndHiHiLoLoDO = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownScanStart = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxContinueScriptOnError = new System.Windows.Forms.CheckBox();
            this.checkBoxTagInfoString = new System.Windows.Forms.CheckBox();
            this.checkBoxFitWindowSize = new System.Windows.Forms.CheckBox();
            this.checkBoxLeftMouseResponse = new System.Windows.Forms.CheckBox();
            this.checkBoxRightMouseResponse = new System.Windows.Forms.CheckBox();
            this.checkBoxGraphicMenuButton = new System.Windows.Forms.CheckBox();
            this.numericUpDownMaxModule = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxScanPause = new System.Windows.Forms.CheckBox();
            this.checkBoxExitWithPlcScan = new System.Windows.Forms.CheckBox();
            this.checkBoxEndPrompt = new System.Windows.Forms.CheckBox();
            this.checkBoxPlcScanWriteTest = new System.Windows.Forms.CheckBox();
            this.checkBoxWriteAiSubWhenStart = new System.Windows.Forms.CheckBox();
            this.checkBoxWriteDiSubWhenStart = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownForLoopTimeout = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numThresholdDelay = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.checkBoxEnableMaintimerProfile = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableBatchProcessing = new System.Windows.Forms.CheckBox();
            this.checkBoxUseAlarmServer = new System.Windows.Forms.CheckBox();
            this.checkBoxUseTestModeMessage = new System.Windows.Forms.CheckBox();
            this.checkBoxUseNotifyIcon = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownMaxReport = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDownMaxBasicScreen = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownBatchSize = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxScriptErrorMessageElse = new System.Windows.Forms.CheckBox();
            this.checkBoxScriptErrorMessageTagNotFound = new System.Windows.Forms.CheckBox();
            this.checkBoxShowScriptErrorMessage = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxPrinter = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.buttonStringModule = new System.Windows.Forms.Button();
            this.buttonDigitalModule = new System.Windows.Forms.Button();
            this.buttonAnalogModule = new System.Windows.Forms.Button();
            this.checkBoxUseStringModule = new System.Windows.Forms.CheckBox();
            this.textBoxStringModule = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxUseDigitalModule = new System.Windows.Forms.CheckBox();
            this.textBoxDigitalModule = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxUseAnalogModule = new System.Windows.Forms.CheckBox();
            this.textBoxAnalogModule = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.checkBoxExitWithUaClient = new System.Windows.Forms.CheckBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseOpcServerItemSecurity = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseDeviceQuality = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxOpcServerGroupSerapatorChar = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownReadInterval = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownOpcServerSubscriptionRefreshCount = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxOpcServerTagGroupNameToUse = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxOpcServerUseRootItem = new System.Windows.Forms.CheckBox();
            this.checkBoxCloseOpcServerWhenLocalMainExit = new System.Windows.Forms.CheckBox();
            this.checkBoxStartOpcServerWhileLocalMainRunning = new System.Windows.Forms.CheckBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.checkBoxAlwaysOpenNewReport = new System.Windows.Forms.CheckBox();
            this.checkBoxRemoveFlashingWhenActivatingMdiModule = new System.Windows.Forms.CheckBox();
            this.checkBoxUseProtectMenu = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayMouseZoneWhenSameTagSelected = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkBoxRightMouseOnObject = new System.Windows.Forms.CheckBox();
            this.checkBoxRightMouseGraphicContextMenu = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.checkBoxOnOffListAddOneSecondToOperationTime = new System.Windows.Forms.CheckBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.m_list = new System.Windows.Forms.ListBox();
            this.buttonCheckMonitor = new System.Windows.Forms.Button();
            this.comboBoxStartMonitorIndex = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.checkBoxUseAutomaticFileRecovery = new System.Windows.Forms.CheckBox();
            this.checkBoxTagValueShareBySharedMemory = new System.Windows.Forms.CheckBox();
            this.checkBoxRestoreLocationSizeOnStartup = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxUsePreviewOutputResult = new System.Windows.Forms.CheckBox();
            this.numericUpDownPreviewOutputResult = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.userControlFontsAndColors1 = new LocalMain.FontsAndColors.UserControlFontsAndColors();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxModule)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownForLoopTimeout)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdDelay)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxBasicScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBatchSize)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReadInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOpcServerSubscriptionRefreshCount)).BeginInit();
            this.tabPage6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPreviewOutputResult)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxAiCalcSameFormat
            // 
            resources.ApplyResources(this.checkBoxAiCalcSameFormat, "checkBoxAiCalcSameFormat");
            this.checkBoxAiCalcSameFormat.Name = "checkBoxAiCalcSameFormat";
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
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxMatchAlarmReturnGabAndHiHiLoLoDO
            // 
            resources.ApplyResources(this.checkBoxMatchAlarmReturnGabAndHiHiLoLoDO, "checkBoxMatchAlarmReturnGabAndHiHiLoLoDO");
            this.checkBoxMatchAlarmReturnGabAndHiHiLoLoDO.Name = "checkBoxMatchAlarmReturnGabAndHiHiLoLoDO";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDownScanStart
            // 
            resources.ApplyResources(this.numericUpDownScanStart, "numericUpDownScanStart");
            this.numericUpDownScanStart.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDownScanStart.Name = "numericUpDownScanStart";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // checkBoxContinueScriptOnError
            // 
            resources.ApplyResources(this.checkBoxContinueScriptOnError, "checkBoxContinueScriptOnError");
            this.checkBoxContinueScriptOnError.Name = "checkBoxContinueScriptOnError";
            // 
            // checkBoxTagInfoString
            // 
            resources.ApplyResources(this.checkBoxTagInfoString, "checkBoxTagInfoString");
            this.checkBoxTagInfoString.Name = "checkBoxTagInfoString";
            // 
            // checkBoxFitWindowSize
            // 
            resources.ApplyResources(this.checkBoxFitWindowSize, "checkBoxFitWindowSize");
            this.checkBoxFitWindowSize.Name = "checkBoxFitWindowSize";
            this.checkBoxFitWindowSize.CheckedChanged += new System.EventHandler(this.checkBoxFitWindowSize_CheckedChanged);
            // 
            // checkBoxLeftMouseResponse
            // 
            resources.ApplyResources(this.checkBoxLeftMouseResponse, "checkBoxLeftMouseResponse");
            this.checkBoxLeftMouseResponse.Name = "checkBoxLeftMouseResponse";
            // 
            // checkBoxRightMouseResponse
            // 
            resources.ApplyResources(this.checkBoxRightMouseResponse, "checkBoxRightMouseResponse");
            this.checkBoxRightMouseResponse.Name = "checkBoxRightMouseResponse";
            this.checkBoxRightMouseResponse.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBoxGraphicMenuButton
            // 
            resources.ApplyResources(this.checkBoxGraphicMenuButton, "checkBoxGraphicMenuButton");
            this.checkBoxGraphicMenuButton.Name = "checkBoxGraphicMenuButton";
            // 
            // numericUpDownMaxModule
            // 
            resources.ApplyResources(this.numericUpDownMaxModule, "numericUpDownMaxModule");
            this.numericUpDownMaxModule.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMaxModule.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxModule.Name = "numericUpDownMaxModule";
            this.numericUpDownMaxModule.Value = new decimal(new int[] {
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
            // checkBoxScanPause
            // 
            resources.ApplyResources(this.checkBoxScanPause, "checkBoxScanPause");
            this.checkBoxScanPause.Name = "checkBoxScanPause";
            // 
            // checkBoxExitWithPlcScan
            // 
            resources.ApplyResources(this.checkBoxExitWithPlcScan, "checkBoxExitWithPlcScan");
            this.checkBoxExitWithPlcScan.Name = "checkBoxExitWithPlcScan";
            // 
            // checkBoxEndPrompt
            // 
            resources.ApplyResources(this.checkBoxEndPrompt, "checkBoxEndPrompt");
            this.checkBoxEndPrompt.Name = "checkBoxEndPrompt";
            // 
            // checkBoxPlcScanWriteTest
            // 
            resources.ApplyResources(this.checkBoxPlcScanWriteTest, "checkBoxPlcScanWriteTest");
            this.checkBoxPlcScanWriteTest.Name = "checkBoxPlcScanWriteTest";
            // 
            // checkBoxWriteAiSubWhenStart
            // 
            resources.ApplyResources(this.checkBoxWriteAiSubWhenStart, "checkBoxWriteAiSubWhenStart");
            this.checkBoxWriteAiSubWhenStart.Name = "checkBoxWriteAiSubWhenStart";
            // 
            // checkBoxWriteDiSubWhenStart
            // 
            resources.ApplyResources(this.checkBoxWriteDiSubWhenStart, "checkBoxWriteDiSubWhenStart");
            this.checkBoxWriteDiSubWhenStart.Name = "checkBoxWriteDiSubWhenStart";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDownForLoopTimeout);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownForLoopTimeout
            // 
            resources.ApplyResources(this.numericUpDownForLoopTimeout, "numericUpDownForLoopTimeout");
            this.numericUpDownForLoopTimeout.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericUpDownForLoopTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownForLoopTimeout.Name = "numericUpDownForLoopTimeout";
            this.numericUpDownForLoopTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage9);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.numThresholdDelay);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.checkBoxEnableMaintimerProfile);
            this.tabPage1.Controls.Add(this.checkBoxEnableBatchProcessing);
            this.tabPage1.Controls.Add(this.checkBoxUseAlarmServer);
            this.tabPage1.Controls.Add(this.checkBoxUseTestModeMessage);
            this.tabPage1.Controls.Add(this.checkBoxUseNotifyIcon);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.checkBoxEndPrompt);
            this.tabPage1.Controls.Add(this.checkBoxWriteDiSubWhenStart);
            this.tabPage1.Controls.Add(this.checkBoxMatchAlarmReturnGabAndHiHiLoLoDO);
            this.tabPage1.Controls.Add(this.numericUpDownBatchSize);
            this.tabPage1.Controls.Add(this.numericUpDownScanStart);
            this.tabPage1.Controls.Add(this.checkBoxAiCalcSameFormat);
            this.tabPage1.Controls.Add(this.checkBoxWriteAiSubWhenStart);
            this.tabPage1.Controls.Add(this.checkBoxPlcScanWriteTest);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.checkBoxFitWindowSize);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.checkBoxScanPause);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // numThresholdDelay
            // 
            resources.ApplyResources(this.numThresholdDelay, "numThresholdDelay");
            this.numThresholdDelay.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numThresholdDelay.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numThresholdDelay.Name = "numThresholdDelay";
            this.numThresholdDelay.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // checkBoxEnableMaintimerProfile
            // 
            resources.ApplyResources(this.checkBoxEnableMaintimerProfile, "checkBoxEnableMaintimerProfile");
            this.checkBoxEnableMaintimerProfile.Name = "checkBoxEnableMaintimerProfile";
            this.checkBoxEnableMaintimerProfile.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableBatchProcessing
            // 
            resources.ApplyResources(this.checkBoxEnableBatchProcessing, "checkBoxEnableBatchProcessing");
            this.checkBoxEnableBatchProcessing.Name = "checkBoxEnableBatchProcessing";
            this.checkBoxEnableBatchProcessing.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseAlarmServer
            // 
            resources.ApplyResources(this.checkBoxUseAlarmServer, "checkBoxUseAlarmServer");
            this.checkBoxUseAlarmServer.Name = "checkBoxUseAlarmServer";
            // 
            // checkBoxUseTestModeMessage
            // 
            resources.ApplyResources(this.checkBoxUseTestModeMessage, "checkBoxUseTestModeMessage");
            this.checkBoxUseTestModeMessage.Name = "checkBoxUseTestModeMessage";
            // 
            // checkBoxUseNotifyIcon
            // 
            resources.ApplyResources(this.checkBoxUseNotifyIcon, "checkBoxUseNotifyIcon");
            this.checkBoxUseNotifyIcon.Name = "checkBoxUseNotifyIcon";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.numericUpDownMaxReport);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.numericUpDownMaxBasicScreen);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.numericUpDownMaxModule);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // numericUpDownMaxReport
            // 
            resources.ApplyResources(this.numericUpDownMaxReport, "numericUpDownMaxReport");
            this.numericUpDownMaxReport.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMaxReport.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxReport.Name = "numericUpDownMaxReport";
            this.numericUpDownMaxReport.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // numericUpDownMaxBasicScreen
            // 
            resources.ApplyResources(this.numericUpDownMaxBasicScreen, "numericUpDownMaxBasicScreen");
            this.numericUpDownMaxBasicScreen.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownMaxBasicScreen.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxBasicScreen.Name = "numericUpDownMaxBasicScreen";
            this.numericUpDownMaxBasicScreen.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownBatchSize
            // 
            resources.ApplyResources(this.numericUpDownBatchSize, "numericUpDownBatchSize");
            this.numericUpDownBatchSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownBatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownBatchSize.Name = "numericUpDownBatchSize";
            this.numericUpDownBatchSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.checkBoxScriptErrorMessageElse);
            this.tabPage2.Controls.Add(this.checkBoxScriptErrorMessageTagNotFound);
            this.tabPage2.Controls.Add(this.checkBoxShowScriptErrorMessage);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.checkBoxContinueScriptOnError);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxScriptErrorMessageElse
            // 
            resources.ApplyResources(this.checkBoxScriptErrorMessageElse, "checkBoxScriptErrorMessageElse");
            this.checkBoxScriptErrorMessageElse.Name = "checkBoxScriptErrorMessageElse";
            this.checkBoxScriptErrorMessageElse.UseVisualStyleBackColor = true;
            // 
            // checkBoxScriptErrorMessageTagNotFound
            // 
            resources.ApplyResources(this.checkBoxScriptErrorMessageTagNotFound, "checkBoxScriptErrorMessageTagNotFound");
            this.checkBoxScriptErrorMessageTagNotFound.Name = "checkBoxScriptErrorMessageTagNotFound";
            this.checkBoxScriptErrorMessageTagNotFound.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowScriptErrorMessage
            // 
            resources.ApplyResources(this.checkBoxShowScriptErrorMessage, "checkBoxShowScriptErrorMessage");
            this.checkBoxShowScriptErrorMessage.Name = "checkBoxShowScriptErrorMessage";
            this.checkBoxShowScriptErrorMessage.UseVisualStyleBackColor = true;
            this.checkBoxShowScriptErrorMessage.CheckedChanged += new System.EventHandler(this.checkBoxShowScriptErrorMessage_CheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.comboBoxPrinter);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboBoxPrinter
            // 
            resources.ApplyResources(this.comboBoxPrinter, "comboBoxPrinter");
            this.comboBoxPrinter.Name = "comboBoxPrinter";
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Controls.Add(this.buttonStringModule);
            this.tabPage3.Controls.Add(this.buttonDigitalModule);
            this.tabPage3.Controls.Add(this.buttonAnalogModule);
            this.tabPage3.Controls.Add(this.checkBoxUseStringModule);
            this.tabPage3.Controls.Add(this.textBoxStringModule);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.checkBoxUseDigitalModule);
            this.tabPage3.Controls.Add(this.textBoxDigitalModule);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.checkBoxUseAnalogModule);
            this.tabPage3.Controls.Add(this.textBoxAnalogModule);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // buttonStringModule
            // 
            resources.ApplyResources(this.buttonStringModule, "buttonStringModule");
            this.buttonStringModule.Name = "buttonStringModule";
            this.buttonStringModule.UseVisualStyleBackColor = true;
            this.buttonStringModule.Click += new System.EventHandler(this.buttonStringModule_Click);
            // 
            // buttonDigitalModule
            // 
            resources.ApplyResources(this.buttonDigitalModule, "buttonDigitalModule");
            this.buttonDigitalModule.Name = "buttonDigitalModule";
            this.buttonDigitalModule.UseVisualStyleBackColor = true;
            this.buttonDigitalModule.Click += new System.EventHandler(this.buttonDigitalModule_Click);
            // 
            // buttonAnalogModule
            // 
            resources.ApplyResources(this.buttonAnalogModule, "buttonAnalogModule");
            this.buttonAnalogModule.Name = "buttonAnalogModule";
            this.buttonAnalogModule.UseVisualStyleBackColor = true;
            this.buttonAnalogModule.Click += new System.EventHandler(this.buttonAnalogModule_Click);
            // 
            // checkBoxUseStringModule
            // 
            resources.ApplyResources(this.checkBoxUseStringModule, "checkBoxUseStringModule");
            this.checkBoxUseStringModule.Name = "checkBoxUseStringModule";
            this.checkBoxUseStringModule.UseVisualStyleBackColor = true;
            this.checkBoxUseStringModule.CheckedChanged += new System.EventHandler(this.checkBoxUseStringModule_CheckedChanged);
            // 
            // textBoxStringModule
            // 
            resources.ApplyResources(this.textBoxStringModule, "textBoxStringModule");
            this.textBoxStringModule.Name = "textBoxStringModule";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // checkBoxUseDigitalModule
            // 
            resources.ApplyResources(this.checkBoxUseDigitalModule, "checkBoxUseDigitalModule");
            this.checkBoxUseDigitalModule.Name = "checkBoxUseDigitalModule";
            this.checkBoxUseDigitalModule.UseVisualStyleBackColor = true;
            this.checkBoxUseDigitalModule.CheckedChanged += new System.EventHandler(this.checkBoxUseDigitalModule_CheckedChanged);
            // 
            // textBoxDigitalModule
            // 
            resources.ApplyResources(this.textBoxDigitalModule, "textBoxDigitalModule");
            this.textBoxDigitalModule.Name = "textBoxDigitalModule";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // checkBoxUseAnalogModule
            // 
            resources.ApplyResources(this.checkBoxUseAnalogModule, "checkBoxUseAnalogModule");
            this.checkBoxUseAnalogModule.Name = "checkBoxUseAnalogModule";
            this.checkBoxUseAnalogModule.UseVisualStyleBackColor = true;
            this.checkBoxUseAnalogModule.CheckedChanged += new System.EventHandler(this.checkBoxUseAnalogModule_CheckedChanged);
            // 
            // textBoxAnalogModule
            // 
            resources.ApplyResources(this.textBoxAnalogModule, "textBoxAnalogModule");
            this.textBoxAnalogModule.Name = "textBoxAnalogModule";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // tabPage4
            // 
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Controls.Add(this.checkBoxExitWithUaClient);
            this.tabPage4.Controls.Add(this.checkBoxExitWithPlcScan);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // checkBoxExitWithUaClient
            // 
            resources.ApplyResources(this.checkBoxExitWithUaClient, "checkBoxExitWithUaClient");
            this.checkBoxExitWithUaClient.Name = "checkBoxExitWithUaClient";
            // 
            // tabPage5
            // 
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Controls.Add(this.groupBox10);
            this.tabPage5.Controls.Add(this.groupBox5);
            this.tabPage5.Controls.Add(this.groupBox4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Controls.Add(this.checkBoxUseOpcServerItemSecurity);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // checkBoxUseOpcServerItemSecurity
            // 
            resources.ApplyResources(this.checkBoxUseOpcServerItemSecurity, "checkBoxUseOpcServerItemSecurity");
            this.checkBoxUseOpcServerItemSecurity.Name = "checkBoxUseOpcServerItemSecurity";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.checkBoxUseDeviceQuality);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBoxUseDeviceQuality
            // 
            resources.ApplyResources(this.checkBoxUseDeviceQuality, "checkBoxUseDeviceQuality");
            this.checkBoxUseDeviceQuality.Name = "checkBoxUseDeviceQuality";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.textBoxOpcServerGroupSerapatorChar);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.numericUpDownReadInterval);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.numericUpDownOpcServerSubscriptionRefreshCount);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.textBoxOpcServerTagGroupNameToUse);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.checkBoxOpcServerUseRootItem);
            this.groupBox4.Controls.Add(this.checkBoxCloseOpcServerWhenLocalMainExit);
            this.groupBox4.Controls.Add(this.checkBoxStartOpcServerWhileLocalMainRunning);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // textBoxOpcServerGroupSerapatorChar
            // 
            resources.ApplyResources(this.textBoxOpcServerGroupSerapatorChar, "textBoxOpcServerGroupSerapatorChar");
            this.textBoxOpcServerGroupSerapatorChar.Name = "textBoxOpcServerGroupSerapatorChar";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // numericUpDownReadInterval
            // 
            resources.ApplyResources(this.numericUpDownReadInterval, "numericUpDownReadInterval");
            this.numericUpDownReadInterval.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownReadInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownReadInterval.Name = "numericUpDownReadInterval";
            this.numericUpDownReadInterval.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // numericUpDownOpcServerSubscriptionRefreshCount
            // 
            resources.ApplyResources(this.numericUpDownOpcServerSubscriptionRefreshCount, "numericUpDownOpcServerSubscriptionRefreshCount");
            this.numericUpDownOpcServerSubscriptionRefreshCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownOpcServerSubscriptionRefreshCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownOpcServerSubscriptionRefreshCount.Name = "numericUpDownOpcServerSubscriptionRefreshCount";
            this.numericUpDownOpcServerSubscriptionRefreshCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // textBoxOpcServerTagGroupNameToUse
            // 
            resources.ApplyResources(this.textBoxOpcServerTagGroupNameToUse, "textBoxOpcServerTagGroupNameToUse");
            this.textBoxOpcServerTagGroupNameToUse.Name = "textBoxOpcServerTagGroupNameToUse";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // checkBoxOpcServerUseRootItem
            // 
            resources.ApplyResources(this.checkBoxOpcServerUseRootItem, "checkBoxOpcServerUseRootItem");
            this.checkBoxOpcServerUseRootItem.Name = "checkBoxOpcServerUseRootItem";
            // 
            // checkBoxCloseOpcServerWhenLocalMainExit
            // 
            resources.ApplyResources(this.checkBoxCloseOpcServerWhenLocalMainExit, "checkBoxCloseOpcServerWhenLocalMainExit");
            this.checkBoxCloseOpcServerWhenLocalMainExit.Name = "checkBoxCloseOpcServerWhenLocalMainExit";
            // 
            // checkBoxStartOpcServerWhileLocalMainRunning
            // 
            resources.ApplyResources(this.checkBoxStartOpcServerWhileLocalMainRunning, "checkBoxStartOpcServerWhileLocalMainRunning");
            this.checkBoxStartOpcServerWhileLocalMainRunning.Name = "checkBoxStartOpcServerWhileLocalMainRunning";
            // 
            // tabPage6
            // 
            resources.ApplyResources(this.tabPage6, "tabPage6");
            this.tabPage6.Controls.Add(this.checkBoxAlwaysOpenNewReport);
            this.tabPage6.Controls.Add(this.checkBoxRemoveFlashingWhenActivatingMdiModule);
            this.tabPage6.Controls.Add(this.checkBoxUseProtectMenu);
            this.tabPage6.Controls.Add(this.checkBoxDisplayMouseZoneWhenSameTagSelected);
            this.tabPage6.Controls.Add(this.groupBox7);
            this.tabPage6.Controls.Add(this.groupBox6);
            this.tabPage6.Controls.Add(this.checkBoxTagInfoString);
            this.tabPage6.Controls.Add(this.checkBoxGraphicMenuButton);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // checkBoxAlwaysOpenNewReport
            // 
            resources.ApplyResources(this.checkBoxAlwaysOpenNewReport, "checkBoxAlwaysOpenNewReport");
            this.checkBoxAlwaysOpenNewReport.Name = "checkBoxAlwaysOpenNewReport";
            this.checkBoxAlwaysOpenNewReport.UseVisualStyleBackColor = true;
            // 
            // checkBoxRemoveFlashingWhenActivatingMdiModule
            // 
            resources.ApplyResources(this.checkBoxRemoveFlashingWhenActivatingMdiModule, "checkBoxRemoveFlashingWhenActivatingMdiModule");
            this.checkBoxRemoveFlashingWhenActivatingMdiModule.Name = "checkBoxRemoveFlashingWhenActivatingMdiModule";
            // 
            // checkBoxUseProtectMenu
            // 
            resources.ApplyResources(this.checkBoxUseProtectMenu, "checkBoxUseProtectMenu");
            this.checkBoxUseProtectMenu.Name = "checkBoxUseProtectMenu";
            // 
            // checkBoxDisplayMouseZoneWhenSameTagSelected
            // 
            resources.ApplyResources(this.checkBoxDisplayMouseZoneWhenSameTagSelected, "checkBoxDisplayMouseZoneWhenSameTagSelected");
            this.checkBoxDisplayMouseZoneWhenSameTagSelected.Name = "checkBoxDisplayMouseZoneWhenSameTagSelected";
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.checkBoxRightMouseOnObject);
            this.groupBox7.Controls.Add(this.checkBoxRightMouseGraphicContextMenu);
            this.groupBox7.Controls.Add(this.checkBoxRightMouseResponse);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // checkBoxRightMouseOnObject
            // 
            resources.ApplyResources(this.checkBoxRightMouseOnObject, "checkBoxRightMouseOnObject");
            this.checkBoxRightMouseOnObject.Name = "checkBoxRightMouseOnObject";
            // 
            // checkBoxRightMouseGraphicContextMenu
            // 
            resources.ApplyResources(this.checkBoxRightMouseGraphicContextMenu, "checkBoxRightMouseGraphicContextMenu");
            this.checkBoxRightMouseGraphicContextMenu.Name = "checkBoxRightMouseGraphicContextMenu";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.checkBoxLeftMouseResponse);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // tabPage7
            // 
            resources.ApplyResources(this.tabPage7, "tabPage7");
            this.tabPage7.Controls.Add(this.checkBoxOnOffListAddOneSecondToOperationTime);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // checkBoxOnOffListAddOneSecondToOperationTime
            // 
            resources.ApplyResources(this.checkBoxOnOffListAddOneSecondToOperationTime, "checkBoxOnOffListAddOneSecondToOperationTime");
            this.checkBoxOnOffListAddOneSecondToOperationTime.Name = "checkBoxOnOffListAddOneSecondToOperationTime";
            // 
            // tabPage8
            // 
            resources.ApplyResources(this.tabPage8, "tabPage8");
            this.tabPage8.Controls.Add(this.userControlFontsAndColors1);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // tabPage9
            // 
            resources.ApplyResources(this.tabPage9, "tabPage9");
            this.tabPage9.Controls.Add(this.label16);
            this.tabPage9.Controls.Add(this.groupBox9);
            this.tabPage9.Controls.Add(this.buttonCheckMonitor);
            this.tabPage9.Controls.Add(this.comboBoxStartMonitorIndex);
            this.tabPage9.Controls.Add(this.label15);
            this.tabPage9.Controls.Add(this.checkBoxUseAutomaticFileRecovery);
            this.tabPage9.Controls.Add(this.checkBoxTagValueShareBySharedMemory);
            this.tabPage9.Controls.Add(this.checkBoxRestoreLocationSizeOnStartup);
            this.tabPage9.Controls.Add(this.groupBox8);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // groupBox9
            // 
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.m_list);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // m_list
            // 
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.Name = "m_list";
            // 
            // buttonCheckMonitor
            // 
            resources.ApplyResources(this.buttonCheckMonitor, "buttonCheckMonitor");
            this.buttonCheckMonitor.Name = "buttonCheckMonitor";
            this.buttonCheckMonitor.UseVisualStyleBackColor = true;
            this.buttonCheckMonitor.Click += new System.EventHandler(this.buttonCheckMonitor_Click);
            // 
            // comboBoxStartMonitorIndex
            // 
            resources.ApplyResources(this.comboBoxStartMonitorIndex, "comboBoxStartMonitorIndex");
            this.comboBoxStartMonitorIndex.FormattingEnabled = true;
            this.comboBoxStartMonitorIndex.Name = "comboBoxStartMonitorIndex";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // checkBoxUseAutomaticFileRecovery
            // 
            resources.ApplyResources(this.checkBoxUseAutomaticFileRecovery, "checkBoxUseAutomaticFileRecovery");
            this.checkBoxUseAutomaticFileRecovery.Name = "checkBoxUseAutomaticFileRecovery";
            // 
            // checkBoxTagValueShareBySharedMemory
            // 
            resources.ApplyResources(this.checkBoxTagValueShareBySharedMemory, "checkBoxTagValueShareBySharedMemory");
            this.checkBoxTagValueShareBySharedMemory.Name = "checkBoxTagValueShareBySharedMemory";
            // 
            // checkBoxRestoreLocationSizeOnStartup
            // 
            resources.ApplyResources(this.checkBoxRestoreLocationSizeOnStartup, "checkBoxRestoreLocationSizeOnStartup");
            this.checkBoxRestoreLocationSizeOnStartup.Name = "checkBoxRestoreLocationSizeOnStartup";
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.checkBoxUsePreviewOutputResult);
            this.groupBox8.Controls.Add(this.numericUpDownPreviewOutputResult);
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // checkBoxUsePreviewOutputResult
            // 
            resources.ApplyResources(this.checkBoxUsePreviewOutputResult, "checkBoxUsePreviewOutputResult");
            this.checkBoxUsePreviewOutputResult.Name = "checkBoxUsePreviewOutputResult";
            this.checkBoxUsePreviewOutputResult.UseVisualStyleBackColor = true;
            this.checkBoxUsePreviewOutputResult.CheckedChanged += new System.EventHandler(this.checkBoxUsePreviewOutputResult_CheckedChanged);
            // 
            // numericUpDownPreviewOutputResult
            // 
            resources.ApplyResources(this.numericUpDownPreviewOutputResult, "numericUpDownPreviewOutputResult");
            this.numericUpDownPreviewOutputResult.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownPreviewOutputResult.Name = "numericUpDownPreviewOutputResult";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // userControlFontsAndColors1
            // 
            resources.ApplyResources(this.userControlFontsAndColors1, "userControlFontsAndColors1");
            this.userControlFontsAndColors1.Name = "userControlFontsAndColors1";
            // 
            // FormConfigEtc
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigEtc";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigEtc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxModule)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownForLoopTimeout)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdDelay)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxBasicScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBatchSize)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReadInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOpcServerSubscriptionRefreshCount)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPreviewOutputResult)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        //20241010 PSU
        public class MonitorItem
        {
            public int Index { get; private set; }
            public string Text { get; private set; }

            public MonitorItem(int index, string text)
            {
                this.Index = index;
                this.Text = text;
            }

            public override string ToString()
            {
                return this.Text;
            }
        }
        
        private void FormConfigEtc_Load(object sender, System.EventArgs e)
		{
			this.checkBoxAiCalcSameFormat.Checked = ConfigRunMain.bAiCalcSameFormat;
			this.checkBoxMatchAlarmReturnGabAndHiHiLoLoDO.Checked = ConfigRunMain.bMatchAlarmReturnGabAndHiHiLoLoDO;
			this.numericUpDownScanStart.Value = ConfigRunMain.nScanStartTime;
			this.checkBoxContinueScriptOnError.Checked = ConfigRunMain.bRunScriptIfError;

			this.checkBoxTagInfoString.Checked = ConfigViewMain.bDisplayToolTip;
			this.checkBoxFitWindowSize.Checked = ConfigViewMain.bFitToWindow;
			this.checkBoxLeftMouseResponse.Checked = ConfigViewMain.bResponseMouseLeftOnGraphic;
			this.checkBoxRightMouseResponse.Checked = ConfigViewMain.bResponseMouseRightOnGraphic;
            this.checkBoxRightMouseGraphicContextMenu.Checked = ConfigViewMain.bResponseMouseRightGraphicContextMenu;
            this.checkBoxRightMouseOnObject.Checked = ConfigViewMain.bResponseMouseRightOnObject;

			this.checkBoxGraphicMenuButton.Checked = ConfigViewMain.bUseMenuButtonOnGraphic;
			this.numericUpDownMaxModule.Value = ConfigViewMain.nMdiCountOnGraphic;
			this.numericUpDownMaxBasicScreen.Value = ConfigViewMain.nMdiCountOnBasicScreen;
            this.numericUpDownMaxReport.Value = ConfigViewMain.nMdiCountOnReport;
			this.checkBoxScanPause.Checked = SharedLocalMain.bScanPauseFlag;
			this.numericUpDownScanStart.Value = ConfigRunMain.nScanStartTime;
			this.checkBoxExitWithPlcScan.Checked = ConfigRunMain.bEndWithPlcScan;
			this.checkBoxEndPrompt.Checked = ConfigRunMain.bEndPrompt;
            this.checkBoxExitWithUaClient.Checked = ConfigRunMain.bEndWithOpcUaClient; //260225 PSU 추가
            this.checkBoxPlcScanWriteTest.Checked = TotalConfig.AutoBaseIniGetPlcScanWriteTest();
			this.checkBoxWriteDiSubWhenStart.Checked = ConfigRunMain.bWriteDiSubTagWhenStart;
			this.checkBoxWriteAiSubWhenStart.Checked = ConfigRunMain.bWriteAiSubTagWhenStart;
			this.numericUpDownForLoopTimeout.Value = ConfigViewMain.nForLoopTimeout;

            this.checkBoxUseNotifyIcon.Checked = ConfigViewMain.bUseNotifyIcon;
            this.checkBoxUseTestModeMessage.Checked = ConfigViewMain.bUseTestModeMessage;

            this.checkBoxUseAnalogModule.Checked = ConfigViewMain.bUserControlBoxUseAnalogModule;
            this.checkBoxUseDigitalModule.Checked = ConfigViewMain.bUserControlBoxUseDigitalModule;
            this.checkBoxUseStringModule.Checked = ConfigViewMain.bUserControlBoxUseStringModule;
            this.textBoxAnalogModule.Text = ConfigViewMain.sUserControlBoxAnalogModule;
            this.textBoxDigitalModule.Text = ConfigViewMain.sUserControlBoxDigitalModule;
            this.textBoxStringModule.Text = ConfigViewMain.sUserControlBoxStringModule;

            this.checkBoxUseDeviceQuality.Checked = ConfigViewMain.bUseDeviceQuality;

            this.checkBoxStartOpcServerWhileLocalMainRunning.Checked = ConfigViewMain.bStartOpcServerWhileLocalMainRunning;
            this.checkBoxCloseOpcServerWhenLocalMainExit.Checked = ConfigViewMain.bCloseOpcServerWhenLocalMainExit;

            this.checkBoxOpcServerUseRootItem.Checked = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcRootUse", 1) == 1;
            this.textBoxOpcServerTagGroupNameToUse.Text = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcUseGroupName", "");
            Tools.SetNumericUpDownValue(this.numericUpDownOpcServerSubscriptionRefreshCount, TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcRefresh", 1));
            Tools.SetNumericUpDownValue(this.numericUpDownReadInterval, TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcInterval", 100));
            this.textBoxOpcServerGroupSerapatorChar.Text = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcGroupSeparatorChar", "/");
            this.checkBoxUseOpcServerItemSecurity.Checked = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerItemSecurity", false);

            this.checkBoxOnOffListAddOneSecondToOperationTime.Checked = ConfigViewMain.bOnOffListAddOneSecondToOperationTime;

            this.checkBoxDisplayMouseZoneWhenSameTagSelected.Checked = ConfigViewMain.bDisplayMouseZoneWhenSameTagSelected;
            this.checkBoxUseProtectMenu.Checked = ConfigViewMain.bUseProtectMenu;

            this.checkBoxUseAlarmServer.Checked = ConfigViewMain.bUseAlarmServer;

            this.checkBoxUsePreviewOutputResult.Checked = ConfigRunMain.bPreviewOutputResult;
            Tools.SetNumericUpDownValue(this.numericUpDownPreviewOutputResult, ConfigRunMain.nPreviewOutputResultSeconds);

            this.checkBoxRestoreLocationSizeOnStartup.Checked = ConfigViewMain.bRestoreLocationSizeOnStartup;

            this.checkBoxRemoveFlashingWhenActivatingMdiModule.Checked = ConfigViewMain.bRemoveFlashingWhenActivatingMdiModule;
            this.checkBoxAlwaysOpenNewReport.Checked = ConfigViewMain.bAlwaysOpenNewReport; // 무조건 리포트 모듈 새로 열기 24-07-01 hsjeong

            this.checkBoxTagValueShareBySharedMemory.Checked = ConfigRunMain.bShareTagValueBySharedMemory;
            this.checkBoxUseAutomaticFileRecovery.Checked = ConfigViewMain.bUseAutomaticFileRecovery;

            this.checkBoxShowScriptErrorMessage.Checked = ConfigViewMain.bScriptErrorMessageShow;
            this.checkBoxScriptErrorMessageTagNotFound.Checked = ConfigViewMain.bScriptErrorMessageTagNotFound;
            this.checkBoxScriptErrorMessageElse.Checked = ConfigViewMain.bScriptErrorMessageElse;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ETC))
				this.buttonOK.Enabled = false;

            userControlFontsAndColors1.Set(ConfigRunMain.fac);

			foreach(String printer in PrinterSettings.InstalledPrinters) 
			{
				comboBoxPrinter.Items.Add(printer);

				if(ConfigViewMain.PrinterOnScriptPrintModule == printer) 
				{
					comboBoxPrinter.SelectedIndex = comboBoxPrinter.Items.Count-1;	
				}
			}

			if(comboBoxPrinter.Items.Count > 0) 
			{
				if(comboBoxPrinter.SelectedIndex == -1)
					comboBoxPrinter.SelectedIndex = 0;
			}

            this.numericUpDownBatchSize.Value = ConfigViewMain.nEventTimerBatchSize > 1 ? ConfigViewMain.nEventTimerBatchSize : 1;
            this.checkBoxEnableBatchProcessing.Checked = ConfigViewMain.bUseEventTimerBatching;

            this.numThresholdDelay.Value =  ConfigRunMain.nMainTimerProfileThresholdMs;
            this.checkBoxEnableMaintimerProfile.Checked = ConfigRunMain.bMainTimerProfile;

            //monitorIndex 추가 20241010 PSU
            PopulateMonitorComboBox();
            LoadSavedMonitorIndex();
        
            EnableDisableAnalogControlBox();
            EnableDisableDigitalControlBox();
            EnableDisableStringControlBox();
            EnableDisableMouseRightResponse();

            EnablePreviewOutputResult();

            EnableDisableScriptErrorMessageItems();

            LoadListViewLanguageSelection(); // LangTool 기능 추가 20251119 PSU
        }

        private void LoadListViewLanguageSelection()
        {
            string[] types = Enum.GetNames(typeof(EnumLanguage));

            for (int i = 0; i < types.Length; i++)
            {
                m_list.Items.Add(types[i]);
            }

            EnumLanguage elang = LanguageTool.GetLanguage();
            m_list.SelectedItem = elang.ToString();
        }

        private void PopulateMonitorComboBox()
        {
            comboBoxStartMonitorIndex.Items.Clear();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen screen = Screen.AllScreens[i];
                string itemText = string.Format(" {0} ({1}x{2})", i, screen.Bounds.Width, screen.Bounds.Height);
                if (screen.Primary)
                {
                    itemText += " - Main";
                }
                comboBoxStartMonitorIndex.Items.Add(new MonitorItem(i, itemText));
            }

            comboBoxStartMonitorIndex.DisplayMember = "Text";
            comboBoxStartMonitorIndex.ValueMember = "Index";

            if (comboBoxStartMonitorIndex.Items.Count > 0)
            {
                comboBoxStartMonitorIndex.SelectedIndex = 0;
            }
        }
        private void LoadSavedMonitorIndex()
        {
            int savedIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StartMonitorIndex", 0);
            for (int i = 0; i < comboBoxStartMonitorIndex.Items.Count; i++)
            {
                if (((MonitorItem)comboBoxStartMonitorIndex.Items[i]).Index == savedIndex)
                {
                    comboBoxStartMonitorIndex.SelectedIndex = i;
                    break;
                }
            }
        }


		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ConfigRunMain.bAiCalcSameFormat = this.checkBoxAiCalcSameFormat.Checked;	
			ConfigRunMain.bMatchAlarmReturnGabAndHiHiLoLoDO = this.checkBoxMatchAlarmReturnGabAndHiHiLoLoDO.Checked;
			ConfigRunMain.nScanStartTime = ConvertTool.ToInt32(this.numericUpDownScanStart.Value);
			ConfigRunMain.bRunScriptIfError = this.checkBoxContinueScriptOnError.Checked;

			ConfigViewMain.bDisplayToolTip = this.checkBoxTagInfoString.Checked;
			ConfigViewMain.bFitToWindow = this.checkBoxFitWindowSize.Checked;
			ConfigViewMain.bResponseMouseLeftOnGraphic = this.checkBoxLeftMouseResponse.Checked;
			ConfigViewMain.bResponseMouseRightOnGraphic = this.checkBoxRightMouseResponse.Checked;
            ConfigViewMain.bResponseMouseRightGraphicContextMenu = this.checkBoxRightMouseGraphicContextMenu.Checked;
            ConfigViewMain.bResponseMouseRightOnObject = this.checkBoxRightMouseOnObject.Checked;
			ConfigViewMain.bUseMenuButtonOnGraphic = this.checkBoxGraphicMenuButton.Checked;
			ConfigViewMain.nMdiCountOnGraphic = ConvertTool.ToInt32(this.numericUpDownMaxModule.Value);
			ConfigViewMain.nMdiCountOnBasicScreen = ConvertTool.ToInt32(this.numericUpDownMaxBasicScreen.Value);
            ConfigViewMain.nMdiCountOnReport = ConvertTool.ToInt32(this.numericUpDownMaxReport.Value);
			SharedLocalMain.bScanPauseFlag = this.checkBoxScanPause.Checked;
			ConfigRunMain.nScanStartTime = ConvertTool.ToInt32(this.numericUpDownScanStart.Value);
			ConfigRunMain.bEndWithPlcScan = this.checkBoxExitWithPlcScan.Checked;
			ConfigRunMain.bEndPrompt = this.checkBoxEndPrompt.Checked;
            ConfigRunMain.bEndWithOpcUaClient = this.checkBoxExitWithUaClient.Checked;
            TotalConfig.AutoBaseIniSetPlcScanWriteTest(this.checkBoxPlcScanWriteTest.Checked);
			ConfigRunMain.bWriteDiSubTagWhenStart = this.checkBoxWriteDiSubWhenStart.Checked;
			ConfigRunMain.bWriteAiSubTagWhenStart = this.checkBoxWriteAiSubWhenStart.Checked;
			ConfigViewMain.nForLoopTimeout = ConvertTool.ToInt32(this.numericUpDownForLoopTimeout.Value);

			ConfigViewMain.PrinterOnScriptPrintModule = comboBoxPrinter.Text;

            ConfigViewMain.bUseNotifyIcon = this.checkBoxUseNotifyIcon.Checked;
            ConfigViewMain.bUseTestModeMessage = this.checkBoxUseTestModeMessage.Checked;

            ConfigViewMain.bUserControlBoxUseAnalogModule = this.checkBoxUseAnalogModule.Checked;
            ConfigViewMain.bUserControlBoxUseDigitalModule = this.checkBoxUseDigitalModule.Checked;
            ConfigViewMain.bUserControlBoxUseStringModule = this.checkBoxUseStringModule.Checked;
            ConfigViewMain.sUserControlBoxAnalogModule = this.textBoxAnalogModule.Text;
            ConfigViewMain.sUserControlBoxDigitalModule = this.textBoxDigitalModule.Text;
            ConfigViewMain.sUserControlBoxStringModule = this.textBoxStringModule.Text;

            ConfigViewMain.bUseDeviceQuality = this.checkBoxUseDeviceQuality.Checked;

            ConfigViewMain.bStartOpcServerWhileLocalMainRunning = this.checkBoxStartOpcServerWhileLocalMainRunning.Checked;
            ConfigViewMain.bCloseOpcServerWhenLocalMainExit = this.checkBoxCloseOpcServerWhenLocalMainExit.Checked;

            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcRootUse", this.checkBoxOpcServerUseRootItem.Checked ? 1 : 0);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcUseGroupName", this.textBoxOpcServerTagGroupNameToUse.Text);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcRefresh", ConvertTool.ToInt32(this.numericUpDownOpcServerSubscriptionRefreshCount.Value));
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcInterval", ConvertTool.ToInt32(this.numericUpDownReadInterval.Value));
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcGroupSeparatorChar", this.textBoxOpcServerGroupSerapatorChar.Text);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerItemSecurity", this.checkBoxUseOpcServerItemSecurity.Checked);

            ConfigViewMain.bOnOffListAddOneSecondToOperationTime = this.checkBoxOnOffListAddOneSecondToOperationTime.Checked;

            ConfigViewMain.bDisplayMouseZoneWhenSameTagSelected = this.checkBoxDisplayMouseZoneWhenSameTagSelected.Checked;
            ConfigViewMain.bUseProtectMenu = this.checkBoxUseProtectMenu.Checked;

            ConfigRunMain.bPreviewOutputResult = this.checkBoxUsePreviewOutputResult.Checked;
            ConfigRunMain.nPreviewOutputResultSeconds = ConvertTool.ToInt32(this.numericUpDownPreviewOutputResult.Value);

            ConfigViewMain.bRestoreLocationSizeOnStartup = this.checkBoxRestoreLocationSizeOnStartup.Checked;

            ConfigViewMain.bRemoveFlashingWhenActivatingMdiModule = this.checkBoxRemoveFlashingWhenActivatingMdiModule.Checked;
            ConfigViewMain.bAlwaysOpenNewReport = this.checkBoxAlwaysOpenNewReport.Checked; //리포트 모듈을 새로 열기 24-07-01 hsjeong

            ConfigRunMain.bShareTagValueBySharedMemory = this.checkBoxTagValueShareBySharedMemory.Checked;
            ConfigViewMain.bUseAutomaticFileRecovery = this.checkBoxUseAutomaticFileRecovery.Checked;

            ConfigViewMain.bScriptErrorMessageShow = this.checkBoxShowScriptErrorMessage.Checked;
            ConfigViewMain.bScriptErrorMessageTagNotFound = this.checkBoxScriptErrorMessageTagNotFound.Checked;
            ConfigViewMain.bScriptErrorMessageElse = this.checkBoxScriptErrorMessageElse.Checked;

            if (ConfigViewMain.bUseAlarmServer != this.checkBoxUseAlarmServer.Checked)
            {
                LocalServiceMain.UnInit();
                ConfigViewMain.bUseAlarmServer = this.checkBoxUseAlarmServer.Checked;
                LocalServiceMain.Init();
            }

            //20241010 PSU
            if (comboBoxStartMonitorIndex.SelectedItem != null)
            {
                int selectedMonitorIndex = ((MonitorItem)comboBoxStartMonitorIndex.SelectedItem).Index;
                TotalConfig.SaveRegAutoBaseConfig("LocalMain", "Form", "StartMonitorIndex", selectedMonitorIndex);
            }

            ConfigViewMain.bUseEventTimerBatching = this.checkBoxEnableBatchProcessing.Checked;
            ConfigViewMain.nEventTimerBatchSize = ConvertTool.ToInt32(this.numericUpDownBatchSize.Value);

            ConfigRunMain.bMainTimerProfile = this.checkBoxEnableMaintimerProfile.Checked; //260226 PSU
            ConfigRunMain.nMainTimerProfileThresholdMs = ConvertTool.ToInt32(this.numThresholdDelay.Value); //260226 PSU

            userControlFontsAndColors1.Get(ConfigRunMain.fac);
            ConfigRunMain.fac.SaveConfig();

			ConfigRunMain.Save();
			ConfigViewMain.Save();

            LanguageTool.SetLanguage((string)m_list.SelectedItem); //LangTool 설정 저장 20251119 PSU

            FormLocalMain.formMain.InitNotifyIcon();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void label2_Click(object sender, System.EventArgs e)
		{
		
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
		
		}

        void EnableDisableMouseRightResponse()
        {
            bool flag = this.checkBoxRightMouseResponse.Checked;

            this.checkBoxRightMouseGraphicContextMenu.Enabled = flag;
            this.checkBoxRightMouseOnObject.Enabled = flag;
        }

		private void checkBox4_CheckedChanged(object sender, System.EventArgs e)
		{
            EnableDisableMouseRightResponse();
		}

		private void checkBoxFitWindowSize_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

        void EnableDisableAnalogControlBox()
        {
            this.textBoxAnalogModule.Enabled = this.checkBoxUseAnalogModule.Checked;
        }

        void EnableDisableDigitalControlBox()
        {
            this.textBoxDigitalModule.Enabled = this.checkBoxUseDigitalModule.Checked;
        }

        void EnableDisableStringControlBox()
        {
            this.textBoxStringModule.Enabled = this.checkBoxUseStringModule.Checked;
        }

        private void checkBoxUseAnalogModule_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableAnalogControlBox();
        }

        private void checkBoxUseDigitalModule_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableDigitalControlBox();
        }

        private void checkBoxUseStringModule_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableStringControlBox();
        }

        void PublicButtonModuleClick(TextBox textbox)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Module files (*.modx)|*.modx";

            dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string my_dir = TotalConfig.sDirWorkProject + "\\graphic";
                if (String.Compare(my_dir, 0, dialog.FileName, 0, my_dir.Length, true) != 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("프로젝트 폴더에 있는 모듈 파일만 사용할 수 있습니다.", "폴더 오류");
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("只可使用工程项目文件夹中的模块文件。", "文件夹错误");
                    else
                        MessageBox.Show("Use module file on project directory.", "Directory Error");

                    return;
                }

                textbox.Text = dialog.FileName.Substring(my_dir.Length + 1);
            }
        }

        private void buttonAnalogModule_Click(object sender, EventArgs e)
        {
            PublicButtonModuleClick(this.textBoxAnalogModule);
        }

        private void buttonDigitalModule_Click(object sender, EventArgs e)
        {
            PublicButtonModuleClick(this.textBoxDigitalModule);
        }

        private void buttonStringModule_Click(object sender, EventArgs e)
        {
            PublicButtonModuleClick(this.textBoxStringModule);
        }

        void EnablePreviewOutputResult()
        {
            this.numericUpDownPreviewOutputResult.Enabled = this.checkBoxUsePreviewOutputResult.Checked;
        }

        private void checkBoxUsePreviewOutputResult_CheckedChanged(object sender, EventArgs e)
        {
            EnablePreviewOutputResult();
        }

        void EnableDisableScriptErrorMessageItems()
        {
            bool flag = checkBoxShowScriptErrorMessage.Checked;

            this.checkBoxScriptErrorMessageTagNotFound.Enabled = flag;
            this.checkBoxScriptErrorMessageElse.Enabled = flag;
        }

        private void checkBoxShowScriptErrorMessage_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableScriptErrorMessageItems();
        }

        private void buttonCheckMonitor_Click(object sender, EventArgs e)
        {
            MonitorIndexDisplay.ShowMonitorIndexes(); 
        }

	}
}
