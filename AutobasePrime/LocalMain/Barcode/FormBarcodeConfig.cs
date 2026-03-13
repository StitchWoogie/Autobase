using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using DialogTag;
using NetTools;

namespace LocalMain.Barcode
{
	/// <summary>
	/// Barcode / QR Code Configuration Form
	/// - Scanner configuration (Keyboard Wedge, Serial Port)
	/// - Generator configuration (QR, DataMatrix, Code128, Code39, EAN13)
	/// - Scan log viewer
	/// - Test scan and barcode generation
	/// </summary>
	public class FormBarcodeConfig : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components = null;

		#region Controls

		// Main layout
		private TabControl tabMain;

		// ── Tab 1: Scanner Settings ──
		private TabPage tabScanners;
		private SplitContainer splitScanners;
		private ListView listViewScanners;
		private ColumnHeader colScanName;
		private ColumnHeader colScanType;
		private ColumnHeader colScanEnabled;
		private ToolStrip toolStripScanners;
		private ToolStripButton btnAddScanner;
		private ToolStripButton btnRemoveScanner;

		private Panel panelScannerDetail;
		private GroupBox grpScannerBasic;
		private Label lblScannerName;
		private TextBox txtScannerName;
		private Label lblInputType;
		private ComboBox comboInputType;
		private CheckBox chkScannerEnabled;

		private GroupBox grpKeyboardWedge;
		private Label lblPrefix;
		private TextBox txtPrefix;
		private Label lblSuffix;
		private TextBox txtSuffix;
		private Label lblKeyTimeout;
		private NumericUpDown numKeyTimeout;
		private Label lblMinLength;
		private NumericUpDown numMinLength;

		private GroupBox grpSerialPort;
		private Label lblPortName;
		private ComboBox comboPortName;
		private Label lblBaudRate;
		private ComboBox comboBaudRate;
		private Label lblDataBits;
		private ComboBox comboDataBits;
		private Label lblParity;
		private ComboBox comboParity;
		private Label lblStopBits;
		private ComboBox comboStopBits;
		private Label lblTerminator;
		private TextBox txtTerminator;

		private GroupBox grpResultTag;
		private Label lblResultTag;
		private TextBox txtResultTag;
		private Button btnResultTagSelect;

		private GroupBox grpValidation;
		private CheckBox chkValidationEnabled;
		private Label lblValMinLen;
		private NumericUpDown numValMinLen;
		private Label lblValMaxLen;
		private NumericUpDown numValMaxLen;
		private Label lblRequiredPrefix;
		private TextBox txtRequiredPrefix;
		private Label lblRegex;
		private TextBox txtRegex;

		// ── Tab 2: Generator Settings ──
		private TabPage tabGenerators;
		private SplitContainer splitGenerators;
		private ListView listViewGenerators;
		private ColumnHeader colGenName;
		private ColumnHeader colGenFormat;
		private ToolStrip toolStripGenerators;
		private ToolStripButton btnAddGenerator;
		private ToolStripButton btnRemoveGenerator;

		private Panel panelGeneratorDetail;
		private GroupBox grpGenBasic;
		private Label lblGenName;
		private TextBox txtGenName;
		private Label lblGenFormat;
		private ComboBox comboGenFormat;
		private Label lblGenText;
		private TextBox txtGenText;
		private Label lblGenTemplate;
		private TextBox txtGenTemplate;
		private Button btnGenTemplateTag;
		private CheckBox chkAutoUpdate;

		private GroupBox grpGenSize;
		private Label lblGenWidth;
		private NumericUpDown numGenWidth;
		private Label lblGenHeight;
		private NumericUpDown numGenHeight;
		private Label lblEcLevel;
		private ComboBox comboEcLevel;

		private GroupBox grpGenPreview;
		private PictureBox picBarcode;
		private Button btnGeneratePreview;
		private Button btnExportImage;

		// ── Tab 3: Scan Log ──
		private TabPage tabScanLog;
		private Panel panelLogFilter;
		private Label lblLogDate;
		private DateTimePicker dtpLogDate;
		private Button btnLogSearch;
		private DataGridView gridScanLog;
		private DataGridViewTextBoxColumn colLogTime;
		private DataGridViewTextBoxColumn colLogOperator;
		private DataGridViewTextBoxColumn colLogBarcode;
		private DataGridViewTextBoxColumn colLogResult;
		private DataGridViewTextBoxColumn colLogScanner;
		private DataGridViewTextBoxColumn colLogTag;
		private DataGridViewTextBoxColumn colLogValue;

		// ── Tab 4: Test ──
		private TabPage tabTest;
		private GroupBox grpTestScan;
		private Label lblTestInput;
		private TextBox txtTestInput;
		private Button btnTestValidate;
		private Label lblTestResult;
		private TextBox txtTestResult;
		private Label lblTestParse;
		private TextBox txtTestParse;

		private GroupBox grpTestGenerate;
		private Label lblTestGenText;
		private TextBox txtTestGenText;
		private Label lblTestGenFormat;
		private ComboBox comboTestGenFormat;
		private Button btnTestGenerate;
		private PictureBox picTestBarcode;

		// Bottom
		private Panel panelBottom;
		private CheckBox chkEnabled;
		private CheckBox chkEnableLog;
		private Button btnApply;
		private Button btnClose;

		#endregion

		public FormBarcodeConfig()
		{
			InitializeComponent();
			ApplyLanguage();
			InitializeForm();
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabScanners = new System.Windows.Forms.TabPage();
            this.splitScanners = new System.Windows.Forms.SplitContainer();
            this.listViewScanners = new System.Windows.Forms.ListView();
            this.colScanName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScanType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colScanEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripScanners = new System.Windows.Forms.ToolStrip();
            this.btnAddScanner = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveScanner = new System.Windows.Forms.ToolStripButton();
            this.panelScannerDetail = new System.Windows.Forms.Panel();
            this.grpValidation = new System.Windows.Forms.GroupBox();
            this.txtRegex = new System.Windows.Forms.TextBox();
            this.lblRegex = new System.Windows.Forms.Label();
            this.txtRequiredPrefix = new System.Windows.Forms.TextBox();
            this.lblRequiredPrefix = new System.Windows.Forms.Label();
            this.numValMaxLen = new System.Windows.Forms.NumericUpDown();
            this.lblValMaxLen = new System.Windows.Forms.Label();
            this.numValMinLen = new System.Windows.Forms.NumericUpDown();
            this.lblValMinLen = new System.Windows.Forms.Label();
            this.chkValidationEnabled = new System.Windows.Forms.CheckBox();
            this.grpResultTag = new System.Windows.Forms.GroupBox();
            this.txtResultTag = new System.Windows.Forms.TextBox();
            this.btnResultTagSelect = new System.Windows.Forms.Button();
            this.lblResultTag = new System.Windows.Forms.Label();
            this.grpSerialPort = new System.Windows.Forms.GroupBox();
            this.txtTerminator = new System.Windows.Forms.TextBox();
            this.lblTerminator = new System.Windows.Forms.Label();
            this.comboStopBits = new System.Windows.Forms.ComboBox();
            this.lblStopBits = new System.Windows.Forms.Label();
            this.comboParity = new System.Windows.Forms.ComboBox();
            this.lblParity = new System.Windows.Forms.Label();
            this.comboDataBits = new System.Windows.Forms.ComboBox();
            this.lblDataBits = new System.Windows.Forms.Label();
            this.comboBaudRate = new System.Windows.Forms.ComboBox();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.comboPortName = new System.Windows.Forms.ComboBox();
            this.lblPortName = new System.Windows.Forms.Label();
            this.grpKeyboardWedge = new System.Windows.Forms.GroupBox();
            this.numMinLength = new System.Windows.Forms.NumericUpDown();
            this.lblMinLength = new System.Windows.Forms.Label();
            this.numKeyTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblKeyTimeout = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.lblSuffix = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.grpScannerBasic = new System.Windows.Forms.GroupBox();
            this.chkScannerEnabled = new System.Windows.Forms.CheckBox();
            this.comboInputType = new System.Windows.Forms.ComboBox();
            this.lblInputType = new System.Windows.Forms.Label();
            this.txtScannerName = new System.Windows.Forms.TextBox();
            this.lblScannerName = new System.Windows.Forms.Label();
            this.tabGenerators = new System.Windows.Forms.TabPage();
            this.splitGenerators = new System.Windows.Forms.SplitContainer();
            this.listViewGenerators = new System.Windows.Forms.ListView();
            this.colGenName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colGenFormat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripGenerators = new System.Windows.Forms.ToolStrip();
            this.btnAddGenerator = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveGenerator = new System.Windows.Forms.ToolStripButton();
            this.panelGeneratorDetail = new System.Windows.Forms.Panel();
            this.grpGenPreview = new System.Windows.Forms.GroupBox();
            this.btnExportImage = new System.Windows.Forms.Button();
            this.btnGeneratePreview = new System.Windows.Forms.Button();
            this.picBarcode = new System.Windows.Forms.PictureBox();
            this.grpGenSize = new System.Windows.Forms.GroupBox();
            this.comboEcLevel = new System.Windows.Forms.ComboBox();
            this.lblEcLevel = new System.Windows.Forms.Label();
            this.numGenHeight = new System.Windows.Forms.NumericUpDown();
            this.lblGenHeight = new System.Windows.Forms.Label();
            this.numGenWidth = new System.Windows.Forms.NumericUpDown();
            this.lblGenWidth = new System.Windows.Forms.Label();
            this.grpGenBasic = new System.Windows.Forms.GroupBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.txtGenTemplate = new System.Windows.Forms.TextBox();
            this.btnGenTemplateTag = new System.Windows.Forms.Button();
            this.lblGenTemplate = new System.Windows.Forms.Label();
            this.txtGenText = new System.Windows.Forms.TextBox();
            this.lblGenText = new System.Windows.Forms.Label();
            this.comboGenFormat = new System.Windows.Forms.ComboBox();
            this.lblGenFormat = new System.Windows.Forms.Label();
            this.txtGenName = new System.Windows.Forms.TextBox();
            this.lblGenName = new System.Windows.Forms.Label();
            this.tabScanLog = new System.Windows.Forms.TabPage();
            this.gridScanLog = new System.Windows.Forms.DataGridView();
            this.colLogTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogOperator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogScanner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelLogFilter = new System.Windows.Forms.Panel();
            this.btnLogSearch = new System.Windows.Forms.Button();
            this.dtpLogDate = new System.Windows.Forms.DateTimePicker();
            this.lblLogDate = new System.Windows.Forms.Label();
            this.tabTest = new System.Windows.Forms.TabPage();
            this.grpTestGenerate = new System.Windows.Forms.GroupBox();
            this.picTestBarcode = new System.Windows.Forms.PictureBox();
            this.btnTestGenerate = new System.Windows.Forms.Button();
            this.comboTestGenFormat = new System.Windows.Forms.ComboBox();
            this.lblTestGenFormat = new System.Windows.Forms.Label();
            this.txtTestGenText = new System.Windows.Forms.TextBox();
            this.lblTestGenText = new System.Windows.Forms.Label();
            this.grpTestScan = new System.Windows.Forms.GroupBox();
            this.txtTestParse = new System.Windows.Forms.TextBox();
            this.lblTestParse = new System.Windows.Forms.Label();
            this.txtTestResult = new System.Windows.Forms.TextBox();
            this.lblTestResult = new System.Windows.Forms.Label();
            this.btnTestValidate = new System.Windows.Forms.Button();
            this.txtTestInput = new System.Windows.Forms.TextBox();
            this.lblTestInput = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.chkEnableLog = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabMain.SuspendLayout();
            this.tabScanners.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitScanners)).BeginInit();
            this.splitScanners.Panel1.SuspendLayout();
            this.splitScanners.Panel2.SuspendLayout();
            this.splitScanners.SuspendLayout();
            this.toolStripScanners.SuspendLayout();
            this.panelScannerDetail.SuspendLayout();
            this.grpValidation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValMaxLen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numValMinLen)).BeginInit();
            this.grpResultTag.SuspendLayout();
            this.grpSerialPort.SuspendLayout();
            this.grpKeyboardWedge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyTimeout)).BeginInit();
            this.grpScannerBasic.SuspendLayout();
            this.tabGenerators.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGenerators)).BeginInit();
            this.splitGenerators.Panel1.SuspendLayout();
            this.splitGenerators.Panel2.SuspendLayout();
            this.splitGenerators.SuspendLayout();
            this.toolStripGenerators.SuspendLayout();
            this.panelGeneratorDetail.SuspendLayout();
            this.grpGenPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).BeginInit();
            this.grpGenSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGenHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGenWidth)).BeginInit();
            this.grpGenBasic.SuspendLayout();
            this.tabScanLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridScanLog)).BeginInit();
            this.panelLogFilter.SuspendLayout();
            this.tabTest.SuspendLayout();
            this.grpTestGenerate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTestBarcode)).BeginInit();
            this.grpTestScan.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabScanners);
            this.tabMain.Controls.Add(this.tabGenerators);
            this.tabMain.Controls.Add(this.tabScanLog);
            this.tabMain.Controls.Add(this.tabTest);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(853, 481);
            this.tabMain.TabIndex = 0;
            // 
            // tabScanners
            // 
            this.tabScanners.Controls.Add(this.splitScanners);
            this.tabScanners.Location = new System.Drawing.Point(4, 22);
            this.tabScanners.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabScanners.Name = "tabScanners";
            this.tabScanners.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabScanners.Size = new System.Drawing.Size(845, 455);
            this.tabScanners.TabIndex = 0;
            this.tabScanners.Text = "Scanners";
            this.tabScanners.UseVisualStyleBackColor = true;
            // 
            // splitScanners
            // 
            this.splitScanners.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitScanners.Location = new System.Drawing.Point(4, 3);
            this.splitScanners.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitScanners.Name = "splitScanners";
            // 
            // splitScanners.Panel1
            // 
            this.splitScanners.Panel1.Controls.Add(this.listViewScanners);
            this.splitScanners.Panel1.Controls.Add(this.toolStripScanners);
            // 
            // splitScanners.Panel2
            // 
            this.splitScanners.Panel2.AutoScroll = true;
            this.splitScanners.Panel2.Controls.Add(this.panelScannerDetail);
            this.splitScanners.Size = new System.Drawing.Size(837, 449);
            this.splitScanners.SplitterDistance = 190;
            this.splitScanners.SplitterWidth = 5;
            this.splitScanners.TabIndex = 0;
            // 
            // listViewScanners
            // 
            this.listViewScanners.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colScanName,
            this.colScanType,
            this.colScanEnabled});
            this.listViewScanners.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewScanners.FullRowSelect = true;
            this.listViewScanners.GridLines = true;
            this.listViewScanners.HideSelection = false;
            this.listViewScanners.Location = new System.Drawing.Point(0, 25);
            this.listViewScanners.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listViewScanners.MultiSelect = false;
            this.listViewScanners.Name = "listViewScanners";
            this.listViewScanners.Size = new System.Drawing.Size(190, 424);
            this.listViewScanners.TabIndex = 0;
            this.listViewScanners.UseCompatibleStateImageBehavior = false;
            this.listViewScanners.View = System.Windows.Forms.View.Details;
            // 
            // colScanName
            // 
            this.colScanName.Text = "Name";
            this.colScanName.Width = 100;
            // 
            // colScanType
            // 
            this.colScanType.Text = "Type";
            this.colScanType.Width = 80;
            // 
            // colScanEnabled
            // 
            this.colScanEnabled.Text = "On";
            this.colScanEnabled.Width = 40;
            // 
            // toolStripScanners
            // 
            this.toolStripScanners.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddScanner,
            this.btnRemoveScanner});
            this.toolStripScanners.Location = new System.Drawing.Point(0, 0);
            this.toolStripScanners.Name = "toolStripScanners";
            this.toolStripScanners.Size = new System.Drawing.Size(190, 25);
            this.toolStripScanners.TabIndex = 1;
            // 
            // btnAddScanner
            // 
            this.btnAddScanner.Name = "btnAddScanner";
            this.btnAddScanner.Size = new System.Drawing.Size(33, 22);
            this.btnAddScanner.Text = "Add";
            // 
            // btnRemoveScanner
            // 
            this.btnRemoveScanner.Name = "btnRemoveScanner";
            this.btnRemoveScanner.Size = new System.Drawing.Size(54, 22);
            this.btnRemoveScanner.Text = "Remove";
            // 
            // panelScannerDetail
            // 
            this.panelScannerDetail.AutoSize = true;
            this.panelScannerDetail.Controls.Add(this.grpValidation);
            this.panelScannerDetail.Controls.Add(this.grpResultTag);
            this.panelScannerDetail.Controls.Add(this.grpSerialPort);
            this.panelScannerDetail.Controls.Add(this.grpKeyboardWedge);
            this.panelScannerDetail.Controls.Add(this.grpScannerBasic);
            this.panelScannerDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelScannerDetail.Location = new System.Drawing.Point(0, 0);
            this.panelScannerDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelScannerDetail.Name = "panelScannerDetail";
            this.panelScannerDetail.Size = new System.Drawing.Size(625, 587);
            this.panelScannerDetail.TabIndex = 0;
            // 
            // grpValidation
            // 
            this.grpValidation.Controls.Add(this.txtRegex);
            this.grpValidation.Controls.Add(this.lblRegex);
            this.grpValidation.Controls.Add(this.txtRequiredPrefix);
            this.grpValidation.Controls.Add(this.lblRequiredPrefix);
            this.grpValidation.Controls.Add(this.numValMaxLen);
            this.grpValidation.Controls.Add(this.lblValMaxLen);
            this.grpValidation.Controls.Add(this.numValMinLen);
            this.grpValidation.Controls.Add(this.lblValMinLen);
            this.grpValidation.Controls.Add(this.chkValidationEnabled);
            this.grpValidation.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpValidation.Location = new System.Drawing.Point(0, 435);
            this.grpValidation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpValidation.Name = "grpValidation";
            this.grpValidation.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpValidation.Size = new System.Drawing.Size(625, 152);
            this.grpValidation.TabIndex = 4;
            this.grpValidation.TabStop = false;
            this.grpValidation.Text = "Validation";
            // 
            // txtRegex
            // 
            this.txtRegex.Location = new System.Drawing.Point(117, 114);
            this.txtRegex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtRegex.Name = "txtRegex";
            this.txtRegex.Size = new System.Drawing.Size(349, 21);
            this.txtRegex.TabIndex = 8;
            // 
            // lblRegex
            // 
            this.lblRegex.Location = new System.Drawing.Point(13, 116);
            this.lblRegex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRegex.Name = "lblRegex";
            this.lblRegex.Size = new System.Drawing.Size(93, 18);
            this.lblRegex.TabIndex = 9;
            this.lblRegex.Text = "Regex";
            // 
            // txtRequiredPrefix
            // 
            this.txtRequiredPrefix.Location = new System.Drawing.Point(117, 90);
            this.txtRequiredPrefix.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtRequiredPrefix.Name = "txtRequiredPrefix";
            this.txtRequiredPrefix.Size = new System.Drawing.Size(186, 21);
            this.txtRequiredPrefix.TabIndex = 6;
            // 
            // lblRequiredPrefix
            // 
            this.lblRequiredPrefix.Location = new System.Drawing.Point(13, 92);
            this.lblRequiredPrefix.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRequiredPrefix.Name = "lblRequiredPrefix";
            this.lblRequiredPrefix.Size = new System.Drawing.Size(93, 18);
            this.lblRequiredPrefix.TabIndex = 10;
            this.lblRequiredPrefix.Text = "Req. Prefix";
            // 
            // numValMaxLen
            // 
            this.numValMaxLen.Location = new System.Drawing.Point(117, 66);
            this.numValMaxLen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numValMaxLen.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numValMaxLen.Name = "numValMaxLen";
            this.numValMaxLen.Size = new System.Drawing.Size(117, 21);
            this.numValMaxLen.TabIndex = 4;
            // 
            // lblValMaxLen
            // 
            this.lblValMaxLen.Location = new System.Drawing.Point(13, 68);
            this.lblValMaxLen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValMaxLen.Name = "lblValMaxLen";
            this.lblValMaxLen.Size = new System.Drawing.Size(93, 18);
            this.lblValMaxLen.TabIndex = 11;
            this.lblValMaxLen.Text = "Max Length";
            // 
            // numValMinLen
            // 
            this.numValMinLen.Location = new System.Drawing.Point(117, 42);
            this.numValMinLen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numValMinLen.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numValMinLen.Name = "numValMinLen";
            this.numValMinLen.Size = new System.Drawing.Size(117, 21);
            this.numValMinLen.TabIndex = 2;
            // 
            // lblValMinLen
            // 
            this.lblValMinLen.Location = new System.Drawing.Point(13, 44);
            this.lblValMinLen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblValMinLen.Name = "lblValMinLen";
            this.lblValMinLen.Size = new System.Drawing.Size(93, 18);
            this.lblValMinLen.TabIndex = 12;
            this.lblValMinLen.Text = "Min Length";
            // 
            // chkValidationEnabled
            // 
            this.chkValidationEnabled.Location = new System.Drawing.Point(13, 20);
            this.chkValidationEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkValidationEnabled.Name = "chkValidationEnabled";
            this.chkValidationEnabled.Size = new System.Drawing.Size(233, 18);
            this.chkValidationEnabled.TabIndex = 0;
            this.chkValidationEnabled.Text = "Enable Validation";
            // 
            // grpResultTag
            // 
            this.grpResultTag.Controls.Add(this.btnResultTagSelect);
            this.grpResultTag.Controls.Add(this.txtResultTag);
            this.grpResultTag.Controls.Add(this.lblResultTag);
            this.grpResultTag.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpResultTag.Location = new System.Drawing.Point(0, 384);
            this.grpResultTag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpResultTag.Name = "grpResultTag";
            this.grpResultTag.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpResultTag.Size = new System.Drawing.Size(625, 51);
            this.grpResultTag.TabIndex = 3;
            this.grpResultTag.TabStop = false;
            this.grpResultTag.Text = "Result Tag";
            // 
            // txtResultTag
            // 
            this.txtResultTag.Location = new System.Drawing.Point(117, 20);
            this.txtResultTag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtResultTag.Name = "txtResultTag";
            this.txtResultTag.Size = new System.Drawing.Size(291, 21);
            this.txtResultTag.TabIndex = 1;
            //
            // btnResultTagSelect
            //
            this.btnResultTagSelect.Location = new System.Drawing.Point(412, 19);
            this.btnResultTagSelect.Name = "btnResultTagSelect";
            this.btnResultTagSelect.Size = new System.Drawing.Size(50, 23);
            this.btnResultTagSelect.TabIndex = 3;
            this.btnResultTagSelect.Text = "...";
            //
            // lblResultTag
            // 
            this.lblResultTag.Location = new System.Drawing.Point(13, 22);
            this.lblResultTag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultTag.Name = "lblResultTag";
            this.lblResultTag.Size = new System.Drawing.Size(93, 18);
            this.lblResultTag.TabIndex = 2;
            this.lblResultTag.Text = "Tag Name";
            // 
            // grpSerialPort
            // 
            this.grpSerialPort.Controls.Add(this.txtTerminator);
            this.grpSerialPort.Controls.Add(this.lblTerminator);
            this.grpSerialPort.Controls.Add(this.comboStopBits);
            this.grpSerialPort.Controls.Add(this.lblStopBits);
            this.grpSerialPort.Controls.Add(this.comboParity);
            this.grpSerialPort.Controls.Add(this.lblParity);
            this.grpSerialPort.Controls.Add(this.comboDataBits);
            this.grpSerialPort.Controls.Add(this.lblDataBits);
            this.grpSerialPort.Controls.Add(this.comboBaudRate);
            this.grpSerialPort.Controls.Add(this.lblBaudRate);
            this.grpSerialPort.Controls.Add(this.comboPortName);
            this.grpSerialPort.Controls.Add(this.lblPortName);
            this.grpSerialPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSerialPort.Location = new System.Drawing.Point(0, 212);
            this.grpSerialPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpSerialPort.Name = "grpSerialPort";
            this.grpSerialPort.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpSerialPort.Size = new System.Drawing.Size(625, 172);
            this.grpSerialPort.TabIndex = 2;
            this.grpSerialPort.TabStop = false;
            this.grpSerialPort.Text = "Serial Port";
            // 
            // txtTerminator
            // 
            this.txtTerminator.Location = new System.Drawing.Point(117, 140);
            this.txtTerminator.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTerminator.Name = "txtTerminator";
            this.txtTerminator.Size = new System.Drawing.Size(139, 21);
            this.txtTerminator.TabIndex = 11;
            this.txtTerminator.Text = "\\r\\n";
            // 
            // lblTerminator
            // 
            this.lblTerminator.Location = new System.Drawing.Point(13, 142);
            this.lblTerminator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTerminator.Name = "lblTerminator";
            this.lblTerminator.Size = new System.Drawing.Size(93, 18);
            this.lblTerminator.TabIndex = 12;
            this.lblTerminator.Text = "Terminator";
            // 
            // comboStopBits
            // 
            this.comboStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStopBits.Location = new System.Drawing.Point(117, 116);
            this.comboStopBits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboStopBits.Name = "comboStopBits";
            this.comboStopBits.Size = new System.Drawing.Size(139, 20);
            this.comboStopBits.TabIndex = 9;
            // 
            // lblStopBits
            // 
            this.lblStopBits.Location = new System.Drawing.Point(13, 118);
            this.lblStopBits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(93, 18);
            this.lblStopBits.TabIndex = 13;
            this.lblStopBits.Text = "Stop Bits";
            // 
            // comboParity
            // 
            this.comboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboParity.Location = new System.Drawing.Point(117, 92);
            this.comboParity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboParity.Name = "comboParity";
            this.comboParity.Size = new System.Drawing.Size(139, 20);
            this.comboParity.TabIndex = 7;
            // 
            // lblParity
            // 
            this.lblParity.Location = new System.Drawing.Point(13, 94);
            this.lblParity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(93, 18);
            this.lblParity.TabIndex = 14;
            this.lblParity.Text = "Parity";
            // 
            // comboDataBits
            // 
            this.comboDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDataBits.Location = new System.Drawing.Point(117, 68);
            this.comboDataBits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboDataBits.Name = "comboDataBits";
            this.comboDataBits.Size = new System.Drawing.Size(139, 20);
            this.comboDataBits.TabIndex = 5;
            // 
            // lblDataBits
            // 
            this.lblDataBits.Location = new System.Drawing.Point(13, 70);
            this.lblDataBits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(93, 18);
            this.lblDataBits.TabIndex = 15;
            this.lblDataBits.Text = "Data Bits";
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBaudRate.Location = new System.Drawing.Point(117, 44);
            this.comboBaudRate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(139, 20);
            this.comboBaudRate.TabIndex = 3;
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.Location = new System.Drawing.Point(13, 46);
            this.lblBaudRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(93, 18);
            this.lblBaudRate.TabIndex = 16;
            this.lblBaudRate.Text = "Baud Rate";
            // 
            // comboPortName
            // 
            this.comboPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPortName.Location = new System.Drawing.Point(117, 20);
            this.comboPortName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboPortName.Name = "comboPortName";
            this.comboPortName.Size = new System.Drawing.Size(139, 20);
            this.comboPortName.TabIndex = 1;
            // 
            // lblPortName
            // 
            this.lblPortName.Location = new System.Drawing.Point(13, 22);
            this.lblPortName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPortName.Name = "lblPortName";
            this.lblPortName.Size = new System.Drawing.Size(93, 18);
            this.lblPortName.TabIndex = 17;
            this.lblPortName.Text = "Port";
            // 
            // grpKeyboardWedge
            // 
            this.grpKeyboardWedge.Controls.Add(this.numMinLength);
            this.grpKeyboardWedge.Controls.Add(this.lblMinLength);
            this.grpKeyboardWedge.Controls.Add(this.numKeyTimeout);
            this.grpKeyboardWedge.Controls.Add(this.lblKeyTimeout);
            this.grpKeyboardWedge.Controls.Add(this.txtSuffix);
            this.grpKeyboardWedge.Controls.Add(this.lblSuffix);
            this.grpKeyboardWedge.Controls.Add(this.txtPrefix);
            this.grpKeyboardWedge.Controls.Add(this.lblPrefix);
            this.grpKeyboardWedge.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpKeyboardWedge.Location = new System.Drawing.Point(0, 92);
            this.grpKeyboardWedge.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpKeyboardWedge.Name = "grpKeyboardWedge";
            this.grpKeyboardWedge.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpKeyboardWedge.Size = new System.Drawing.Size(625, 120);
            this.grpKeyboardWedge.TabIndex = 1;
            this.grpKeyboardWedge.TabStop = false;
            this.grpKeyboardWedge.Text = "Keyboard Wedge";
            // 
            // numMinLength
            // 
            this.numMinLength.Location = new System.Drawing.Point(117, 92);
            this.numMinLength.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numMinLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinLength.Name = "numMinLength";
            this.numMinLength.Size = new System.Drawing.Size(117, 21);
            this.numMinLength.TabIndex = 7;
            this.numMinLength.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblMinLength
            // 
            this.lblMinLength.Location = new System.Drawing.Point(13, 94);
            this.lblMinLength.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMinLength.Name = "lblMinLength";
            this.lblMinLength.Size = new System.Drawing.Size(93, 18);
            this.lblMinLength.TabIndex = 8;
            this.lblMinLength.Text = "Min Length";
            // 
            // numKeyTimeout
            // 
            this.numKeyTimeout.Location = new System.Drawing.Point(117, 68);
            this.numKeyTimeout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numKeyTimeout.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numKeyTimeout.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numKeyTimeout.Name = "numKeyTimeout";
            this.numKeyTimeout.Size = new System.Drawing.Size(117, 21);
            this.numKeyTimeout.TabIndex = 5;
            this.numKeyTimeout.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblKeyTimeout
            // 
            this.lblKeyTimeout.Location = new System.Drawing.Point(13, 70);
            this.lblKeyTimeout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblKeyTimeout.Name = "lblKeyTimeout";
            this.lblKeyTimeout.Size = new System.Drawing.Size(93, 18);
            this.lblKeyTimeout.TabIndex = 9;
            this.lblKeyTimeout.Text = "Timeout(ms)";
            // 
            // txtSuffix
            // 
            this.txtSuffix.Location = new System.Drawing.Point(117, 44);
            this.txtSuffix.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(186, 21);
            this.txtSuffix.TabIndex = 3;
            // 
            // lblSuffix
            // 
            this.lblSuffix.Location = new System.Drawing.Point(13, 46);
            this.lblSuffix.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.Size = new System.Drawing.Size(93, 18);
            this.lblSuffix.TabIndex = 10;
            this.lblSuffix.Text = "Suffix";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(117, 20);
            this.txtPrefix.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(186, 21);
            this.txtPrefix.TabIndex = 1;
            // 
            // lblPrefix
            // 
            this.lblPrefix.Location = new System.Drawing.Point(13, 22);
            this.lblPrefix.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(93, 18);
            this.lblPrefix.TabIndex = 11;
            this.lblPrefix.Text = "Prefix";
            // 
            // grpScannerBasic
            // 
            this.grpScannerBasic.Controls.Add(this.chkScannerEnabled);
            this.grpScannerBasic.Controls.Add(this.comboInputType);
            this.grpScannerBasic.Controls.Add(this.lblInputType);
            this.grpScannerBasic.Controls.Add(this.txtScannerName);
            this.grpScannerBasic.Controls.Add(this.lblScannerName);
            this.grpScannerBasic.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpScannerBasic.Location = new System.Drawing.Point(0, 0);
            this.grpScannerBasic.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpScannerBasic.Name = "grpScannerBasic";
            this.grpScannerBasic.Padding = new System.Windows.Forms.Padding(9, 7, 9, 7);
            this.grpScannerBasic.Size = new System.Drawing.Size(625, 92);
            this.grpScannerBasic.TabIndex = 0;
            this.grpScannerBasic.TabStop = false;
            this.grpScannerBasic.Text = "Basic";
            // 
            // chkScannerEnabled
            // 
            this.chkScannerEnabled.Location = new System.Drawing.Point(117, 70);
            this.chkScannerEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkScannerEnabled.Name = "chkScannerEnabled";
            this.chkScannerEnabled.Size = new System.Drawing.Size(140, 18);
            this.chkScannerEnabled.TabIndex = 4;
            this.chkScannerEnabled.Text = "Enabled";
            // 
            // comboInputType
            // 
            this.comboInputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInputType.Location = new System.Drawing.Point(117, 46);
            this.comboInputType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboInputType.Name = "comboInputType";
            this.comboInputType.Size = new System.Drawing.Size(233, 20);
            this.comboInputType.TabIndex = 3;
            // 
            // lblInputType
            // 
            this.lblInputType.Location = new System.Drawing.Point(13, 48);
            this.lblInputType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInputType.Name = "lblInputType";
            this.lblInputType.Size = new System.Drawing.Size(93, 18);
            this.lblInputType.TabIndex = 5;
            this.lblInputType.Text = "Input Type";
            // 
            // txtScannerName
            // 
            this.txtScannerName.Location = new System.Drawing.Point(117, 20);
            this.txtScannerName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtScannerName.Name = "txtScannerName";
            this.txtScannerName.Size = new System.Drawing.Size(233, 21);
            this.txtScannerName.TabIndex = 1;
            // 
            // lblScannerName
            // 
            this.lblScannerName.Location = new System.Drawing.Point(13, 22);
            this.lblScannerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScannerName.Name = "lblScannerName";
            this.lblScannerName.Size = new System.Drawing.Size(93, 18);
            this.lblScannerName.TabIndex = 6;
            this.lblScannerName.Text = "Name";
            // 
            // tabGenerators
            // 
            this.tabGenerators.Controls.Add(this.splitGenerators);
            this.tabGenerators.Location = new System.Drawing.Point(4, 22);
            this.tabGenerators.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabGenerators.Name = "tabGenerators";
            this.tabGenerators.Size = new System.Drawing.Size(845, 455);
            this.tabGenerators.TabIndex = 1;
            this.tabGenerators.Text = "Generators";
            this.tabGenerators.UseVisualStyleBackColor = true;
            // 
            // splitGenerators
            // 
            this.splitGenerators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGenerators.Location = new System.Drawing.Point(0, 0);
            this.splitGenerators.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitGenerators.Name = "splitGenerators";
            // 
            // splitGenerators.Panel1
            // 
            this.splitGenerators.Panel1.Controls.Add(this.listViewGenerators);
            this.splitGenerators.Panel1.Controls.Add(this.toolStripGenerators);
            // 
            // splitGenerators.Panel2
            // 
            this.splitGenerators.Panel2.AutoScroll = true;
            this.splitGenerators.Panel2.Controls.Add(this.panelGeneratorDetail);
            this.splitGenerators.Size = new System.Drawing.Size(845, 455);
            this.splitGenerators.SplitterDistance = 164;
            this.splitGenerators.SplitterWidth = 5;
            this.splitGenerators.TabIndex = 0;
            // 
            // listViewGenerators
            // 
            this.listViewGenerators.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colGenName,
            this.colGenFormat});
            this.listViewGenerators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewGenerators.FullRowSelect = true;
            this.listViewGenerators.GridLines = true;
            this.listViewGenerators.HideSelection = false;
            this.listViewGenerators.Location = new System.Drawing.Point(0, 25);
            this.listViewGenerators.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listViewGenerators.MultiSelect = false;
            this.listViewGenerators.Name = "listViewGenerators";
            this.listViewGenerators.Size = new System.Drawing.Size(164, 430);
            this.listViewGenerators.TabIndex = 0;
            this.listViewGenerators.UseCompatibleStateImageBehavior = false;
            this.listViewGenerators.View = System.Windows.Forms.View.Details;
            // 
            // colGenName
            // 
            this.colGenName.Text = "Name";
            this.colGenName.Width = 100;
            // 
            // colGenFormat
            // 
            this.colGenFormat.Text = "Format";
            this.colGenFormat.Width = 80;
            // 
            // toolStripGenerators
            // 
            this.toolStripGenerators.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddGenerator,
            this.btnRemoveGenerator});
            this.toolStripGenerators.Location = new System.Drawing.Point(0, 0);
            this.toolStripGenerators.Name = "toolStripGenerators";
            this.toolStripGenerators.Size = new System.Drawing.Size(164, 25);
            this.toolStripGenerators.TabIndex = 1;
            // 
            // btnAddGenerator
            // 
            this.btnAddGenerator.Name = "btnAddGenerator";
            this.btnAddGenerator.Size = new System.Drawing.Size(33, 22);
            this.btnAddGenerator.Text = "Add";
            // 
            // btnRemoveGenerator
            // 
            this.btnRemoveGenerator.Name = "btnRemoveGenerator";
            this.btnRemoveGenerator.Size = new System.Drawing.Size(54, 22);
            this.btnRemoveGenerator.Text = "Remove";
            // 
            // panelGeneratorDetail
            // 
            this.panelGeneratorDetail.AutoSize = true;
            this.panelGeneratorDetail.Controls.Add(this.grpGenPreview);
            this.panelGeneratorDetail.Controls.Add(this.grpGenSize);
            this.panelGeneratorDetail.Controls.Add(this.grpGenBasic);
            this.panelGeneratorDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelGeneratorDetail.Location = new System.Drawing.Point(0, 0);
            this.panelGeneratorDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelGeneratorDetail.Name = "panelGeneratorDetail";
            this.panelGeneratorDetail.Size = new System.Drawing.Size(659, 507);
            this.panelGeneratorDetail.TabIndex = 0;
            // 
            // grpGenPreview
            // 
            this.grpGenPreview.Controls.Add(this.btnExportImage);
            this.grpGenPreview.Controls.Add(this.btnGeneratePreview);
            this.grpGenPreview.Controls.Add(this.picBarcode);
            this.grpGenPreview.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpGenPreview.Location = new System.Drawing.Point(0, 249);
            this.grpGenPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGenPreview.Name = "grpGenPreview";
            this.grpGenPreview.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGenPreview.Size = new System.Drawing.Size(659, 258);
            this.grpGenPreview.TabIndex = 2;
            this.grpGenPreview.TabStop = false;
            this.grpGenPreview.Text = "Preview";
            // 
            // btnExportImage
            // 
            this.btnExportImage.Location = new System.Drawing.Point(292, 54);
            this.btnExportImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExportImage.Name = "btnExportImage";
            this.btnExportImage.Size = new System.Drawing.Size(117, 26);
            this.btnExportImage.TabIndex = 2;
            this.btnExportImage.Text = "Export Image";
            // 
            // btnGeneratePreview
            // 
            this.btnGeneratePreview.Location = new System.Drawing.Point(292, 22);
            this.btnGeneratePreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGeneratePreview.Name = "btnGeneratePreview";
            this.btnGeneratePreview.Size = new System.Drawing.Size(117, 26);
            this.btnGeneratePreview.TabIndex = 1;
            this.btnGeneratePreview.Text = "Generate";
            // 
            // picBarcode
            // 
            this.picBarcode.BackColor = System.Drawing.Color.White;
            this.picBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBarcode.Location = new System.Drawing.Point(13, 22);
            this.picBarcode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.picBarcode.Name = "picBarcode";
            this.picBarcode.Size = new System.Drawing.Size(150, 150);
            this.picBarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBarcode.TabIndex = 0;
            this.picBarcode.TabStop = false;
            // 
            // grpGenSize
            // 
            this.grpGenSize.Controls.Add(this.comboEcLevel);
            this.grpGenSize.Controls.Add(this.lblEcLevel);
            this.grpGenSize.Controls.Add(this.numGenHeight);
            this.grpGenSize.Controls.Add(this.lblGenHeight);
            this.grpGenSize.Controls.Add(this.numGenWidth);
            this.grpGenSize.Controls.Add(this.lblGenWidth);
            this.grpGenSize.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpGenSize.Location = new System.Drawing.Point(0, 152);
            this.grpGenSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGenSize.Name = "grpGenSize";
            this.grpGenSize.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGenSize.Size = new System.Drawing.Size(659, 97);
            this.grpGenSize.TabIndex = 1;
            this.grpGenSize.TabStop = false;
            this.grpGenSize.Text = "Size / EC Level";
            // 
            // comboEcLevel
            // 
            this.comboEcLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEcLevel.Location = new System.Drawing.Point(117, 68);
            this.comboEcLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboEcLevel.Name = "comboEcLevel";
            this.comboEcLevel.Size = new System.Drawing.Size(139, 20);
            this.comboEcLevel.TabIndex = 5;
            // 
            // lblEcLevel
            // 
            this.lblEcLevel.Location = new System.Drawing.Point(13, 70);
            this.lblEcLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEcLevel.Name = "lblEcLevel";
            this.lblEcLevel.Size = new System.Drawing.Size(93, 18);
            this.lblEcLevel.TabIndex = 6;
            this.lblEcLevel.Text = "EC Level";
            // 
            // numGenHeight
            // 
            this.numGenHeight.Location = new System.Drawing.Point(117, 44);
            this.numGenHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numGenHeight.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numGenHeight.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numGenHeight.Name = "numGenHeight";
            this.numGenHeight.Size = new System.Drawing.Size(117, 21);
            this.numGenHeight.TabIndex = 3;
            this.numGenHeight.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // lblGenHeight
            // 
            this.lblGenHeight.Location = new System.Drawing.Point(13, 46);
            this.lblGenHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenHeight.Name = "lblGenHeight";
            this.lblGenHeight.Size = new System.Drawing.Size(93, 18);
            this.lblGenHeight.TabIndex = 7;
            this.lblGenHeight.Text = "Height";
            // 
            // numGenWidth
            // 
            this.numGenWidth.Location = new System.Drawing.Point(117, 20);
            this.numGenWidth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numGenWidth.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numGenWidth.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numGenWidth.Name = "numGenWidth";
            this.numGenWidth.Size = new System.Drawing.Size(117, 21);
            this.numGenWidth.TabIndex = 1;
            this.numGenWidth.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // lblGenWidth
            // 
            this.lblGenWidth.Location = new System.Drawing.Point(13, 22);
            this.lblGenWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenWidth.Name = "lblGenWidth";
            this.lblGenWidth.Size = new System.Drawing.Size(93, 18);
            this.lblGenWidth.TabIndex = 8;
            this.lblGenWidth.Text = "Width";
            // 
            // grpGenBasic
            // 
            this.grpGenBasic.Controls.Add(this.chkAutoUpdate);
            this.grpGenBasic.Controls.Add(this.btnGenTemplateTag);
            this.grpGenBasic.Controls.Add(this.txtGenTemplate);
            this.grpGenBasic.Controls.Add(this.lblGenTemplate);
            this.grpGenBasic.Controls.Add(this.txtGenText);
            this.grpGenBasic.Controls.Add(this.lblGenText);
            this.grpGenBasic.Controls.Add(this.comboGenFormat);
            this.grpGenBasic.Controls.Add(this.lblGenFormat);
            this.grpGenBasic.Controls.Add(this.txtGenName);
            this.grpGenBasic.Controls.Add(this.lblGenName);
            this.grpGenBasic.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpGenBasic.Location = new System.Drawing.Point(0, 0);
            this.grpGenBasic.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGenBasic.Name = "grpGenBasic";
            this.grpGenBasic.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGenBasic.Size = new System.Drawing.Size(659, 152);
            this.grpGenBasic.TabIndex = 0;
            this.grpGenBasic.TabStop = false;
            this.grpGenBasic.Text = "Basic";
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.Location = new System.Drawing.Point(117, 118);
            this.chkAutoUpdate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(233, 18);
            this.chkAutoUpdate.TabIndex = 8;
            this.chkAutoUpdate.Text = "Auto Update on Tag Change";
            // 
            // txtGenTemplate
            // 
            this.txtGenTemplate.Location = new System.Drawing.Point(117, 92);
            this.txtGenTemplate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtGenTemplate.Name = "txtGenTemplate";
            this.txtGenTemplate.Size = new System.Drawing.Size(352, 21);
            this.txtGenTemplate.TabIndex = 7;
            //
            // btnGenTemplateTag
            //
            this.btnGenTemplateTag.Location = new System.Drawing.Point(475, 91);
            this.btnGenTemplateTag.Name = "btnGenTemplateTag";
            this.btnGenTemplateTag.Size = new System.Drawing.Size(50, 23);
            this.btnGenTemplateTag.TabIndex = 11;
            this.btnGenTemplateTag.Text = "...";
            //
            // lblGenTemplate
            // 
            this.lblGenTemplate.Location = new System.Drawing.Point(13, 94);
            this.lblGenTemplate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenTemplate.Name = "lblGenTemplate";
            this.lblGenTemplate.Size = new System.Drawing.Size(93, 18);
            this.lblGenTemplate.TabIndex = 9;
            this.lblGenTemplate.Text = "Template";
            // 
            // txtGenText
            // 
            this.txtGenText.Location = new System.Drawing.Point(117, 68);
            this.txtGenText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtGenText.Name = "txtGenText";
            this.txtGenText.Size = new System.Drawing.Size(408, 21);
            this.txtGenText.TabIndex = 5;
            // 
            // lblGenText
            // 
            this.lblGenText.Location = new System.Drawing.Point(13, 70);
            this.lblGenText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenText.Name = "lblGenText";
            this.lblGenText.Size = new System.Drawing.Size(93, 18);
            this.lblGenText.TabIndex = 10;
            this.lblGenText.Text = "Text";
            // 
            // comboGenFormat
            // 
            this.comboGenFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGenFormat.Location = new System.Drawing.Point(117, 44);
            this.comboGenFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboGenFormat.Name = "comboGenFormat";
            this.comboGenFormat.Size = new System.Drawing.Size(233, 20);
            this.comboGenFormat.TabIndex = 3;
            // 
            // lblGenFormat
            // 
            this.lblGenFormat.Location = new System.Drawing.Point(13, 46);
            this.lblGenFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenFormat.Name = "lblGenFormat";
            this.lblGenFormat.Size = new System.Drawing.Size(93, 18);
            this.lblGenFormat.TabIndex = 11;
            this.lblGenFormat.Text = "Format";
            // 
            // txtGenName
            // 
            this.txtGenName.Location = new System.Drawing.Point(117, 20);
            this.txtGenName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtGenName.Name = "txtGenName";
            this.txtGenName.Size = new System.Drawing.Size(233, 21);
            this.txtGenName.TabIndex = 1;
            // 
            // lblGenName
            // 
            this.lblGenName.Location = new System.Drawing.Point(13, 22);
            this.lblGenName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenName.Name = "lblGenName";
            this.lblGenName.Size = new System.Drawing.Size(93, 18);
            this.lblGenName.TabIndex = 12;
            this.lblGenName.Text = "Name";
            // 
            // tabScanLog
            // 
            this.tabScanLog.Controls.Add(this.gridScanLog);
            this.tabScanLog.Controls.Add(this.panelLogFilter);
            this.tabScanLog.Location = new System.Drawing.Point(4, 22);
            this.tabScanLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabScanLog.Name = "tabScanLog";
            this.tabScanLog.Size = new System.Drawing.Size(845, 455);
            this.tabScanLog.TabIndex = 2;
            this.tabScanLog.Text = "Scan Log";
            this.tabScanLog.UseVisualStyleBackColor = true;
            // 
            // gridScanLog
            // 
            this.gridScanLog.AllowUserToAddRows = false;
            this.gridScanLog.AllowUserToDeleteRows = false;
            this.gridScanLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLogTime,
            this.colLogOperator,
            this.colLogBarcode,
            this.colLogResult,
            this.colLogScanner,
            this.colLogTag,
            this.colLogValue});
            this.gridScanLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridScanLog.Location = new System.Drawing.Point(0, 31);
            this.gridScanLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridScanLog.Name = "gridScanLog";
            this.gridScanLog.ReadOnly = true;
            this.gridScanLog.RowHeadersVisible = false;
            this.gridScanLog.Size = new System.Drawing.Size(845, 424);
            this.gridScanLog.TabIndex = 1;
            // 
            // colLogTime
            // 
            this.colLogTime.HeaderText = "Time";
            this.colLogTime.Name = "colLogTime";
            this.colLogTime.ReadOnly = true;
            this.colLogTime.Width = 140;
            // 
            // colLogOperator
            // 
            this.colLogOperator.HeaderText = "Operator";
            this.colLogOperator.Name = "colLogOperator";
            this.colLogOperator.ReadOnly = true;
            this.colLogOperator.Width = 80;
            // 
            // colLogBarcode
            // 
            this.colLogBarcode.HeaderText = "Barcode";
            this.colLogBarcode.Name = "colLogBarcode";
            this.colLogBarcode.ReadOnly = true;
            this.colLogBarcode.Width = 200;
            // 
            // colLogResult
            // 
            this.colLogResult.HeaderText = "Result";
            this.colLogResult.Name = "colLogResult";
            this.colLogResult.ReadOnly = true;
            this.colLogResult.Width = 80;
            // 
            // colLogScanner
            // 
            this.colLogScanner.HeaderText = "Scanner";
            this.colLogScanner.Name = "colLogScanner";
            this.colLogScanner.ReadOnly = true;
            // 
            // colLogTag
            // 
            this.colLogTag.HeaderText = "Tag";
            this.colLogTag.Name = "colLogTag";
            this.colLogTag.ReadOnly = true;
            this.colLogTag.Width = 120;
            // 
            // colLogValue
            // 
            this.colLogValue.HeaderText = "Value";
            this.colLogValue.Name = "colLogValue";
            this.colLogValue.ReadOnly = true;
            this.colLogValue.Width = 120;
            // 
            // panelLogFilter
            // 
            this.panelLogFilter.Controls.Add(this.btnLogSearch);
            this.panelLogFilter.Controls.Add(this.dtpLogDate);
            this.panelLogFilter.Controls.Add(this.lblLogDate);
            this.panelLogFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogFilter.Location = new System.Drawing.Point(0, 0);
            this.panelLogFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelLogFilter.Name = "panelLogFilter";
            this.panelLogFilter.Size = new System.Drawing.Size(845, 31);
            this.panelLogFilter.TabIndex = 0;
            // 
            // btnLogSearch
            // 
            this.btnLogSearch.Location = new System.Drawing.Point(210, 5);
            this.btnLogSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnLogSearch.Name = "btnLogSearch";
            this.btnLogSearch.Size = new System.Drawing.Size(82, 21);
            this.btnLogSearch.TabIndex = 2;
            this.btnLogSearch.Text = "Search";
            // 
            // dtpLogDate
            // 
            this.dtpLogDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpLogDate.Location = new System.Drawing.Point(61, 6);
            this.dtpLogDate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dtpLogDate.Name = "dtpLogDate";
            this.dtpLogDate.Size = new System.Drawing.Size(139, 21);
            this.dtpLogDate.TabIndex = 1;
            // 
            // lblLogDate
            // 
            this.lblLogDate.Location = new System.Drawing.Point(9, 7);
            this.lblLogDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogDate.Name = "lblLogDate";
            this.lblLogDate.Size = new System.Drawing.Size(47, 18);
            this.lblLogDate.TabIndex = 3;
            this.lblLogDate.Text = "Date";
            // 
            // tabTest
            // 
            this.tabTest.Controls.Add(this.grpTestGenerate);
            this.tabTest.Controls.Add(this.grpTestScan);
            this.tabTest.Location = new System.Drawing.Point(4, 22);
            this.tabTest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabTest.Name = "tabTest";
            this.tabTest.Size = new System.Drawing.Size(845, 455);
            this.tabTest.TabIndex = 3;
            this.tabTest.Text = "Test";
            this.tabTest.UseVisualStyleBackColor = true;
            // 
            // grpTestGenerate
            // 
            this.grpTestGenerate.Controls.Add(this.picTestBarcode);
            this.grpTestGenerate.Controls.Add(this.btnTestGenerate);
            this.grpTestGenerate.Controls.Add(this.comboTestGenFormat);
            this.grpTestGenerate.Controls.Add(this.lblTestGenFormat);
            this.grpTestGenerate.Controls.Add(this.txtTestGenText);
            this.grpTestGenerate.Controls.Add(this.lblTestGenText);
            this.grpTestGenerate.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTestGenerate.Location = new System.Drawing.Point(0, 148);
            this.grpTestGenerate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpTestGenerate.Name = "grpTestGenerate";
            this.grpTestGenerate.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpTestGenerate.Size = new System.Drawing.Size(845, 277);
            this.grpTestGenerate.TabIndex = 1;
            this.grpTestGenerate.TabStop = false;
            this.grpTestGenerate.Text = "Test Generate";
            // 
            // picTestBarcode
            // 
            this.picTestBarcode.BackColor = System.Drawing.Color.White;
            this.picTestBarcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTestBarcode.Location = new System.Drawing.Point(117, 102);
            this.picTestBarcode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.picTestBarcode.Name = "picTestBarcode";
            this.picTestBarcode.Size = new System.Drawing.Size(150, 150);
            this.picTestBarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picTestBarcode.TabIndex = 5;
            this.picTestBarcode.TabStop = false;
            // 
            // btnTestGenerate
            // 
            this.btnTestGenerate.Location = new System.Drawing.Point(117, 72);
            this.btnTestGenerate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnTestGenerate.Name = "btnTestGenerate";
            this.btnTestGenerate.Size = new System.Drawing.Size(117, 23);
            this.btnTestGenerate.TabIndex = 4;
            this.btnTestGenerate.Text = "Generate";
            // 
            // comboTestGenFormat
            // 
            this.comboTestGenFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTestGenFormat.Location = new System.Drawing.Point(117, 46);
            this.comboTestGenFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboTestGenFormat.Name = "comboTestGenFormat";
            this.comboTestGenFormat.Size = new System.Drawing.Size(186, 20);
            this.comboTestGenFormat.TabIndex = 3;
            // 
            // lblTestGenFormat
            // 
            this.lblTestGenFormat.Location = new System.Drawing.Point(13, 48);
            this.lblTestGenFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTestGenFormat.Name = "lblTestGenFormat";
            this.lblTestGenFormat.Size = new System.Drawing.Size(93, 18);
            this.lblTestGenFormat.TabIndex = 6;
            this.lblTestGenFormat.Text = "Format";
            // 
            // txtTestGenText
            // 
            this.txtTestGenText.Location = new System.Drawing.Point(117, 20);
            this.txtTestGenText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTestGenText.Name = "txtTestGenText";
            this.txtTestGenText.Size = new System.Drawing.Size(408, 21);
            this.txtTestGenText.TabIndex = 1;
            // 
            // lblTestGenText
            // 
            this.lblTestGenText.Location = new System.Drawing.Point(13, 22);
            this.lblTestGenText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTestGenText.Name = "lblTestGenText";
            this.lblTestGenText.Size = new System.Drawing.Size(93, 18);
            this.lblTestGenText.TabIndex = 7;
            this.lblTestGenText.Text = "Text";
            // 
            // grpTestScan
            // 
            this.grpTestScan.Controls.Add(this.txtTestParse);
            this.grpTestScan.Controls.Add(this.lblTestParse);
            this.grpTestScan.Controls.Add(this.txtTestResult);
            this.grpTestScan.Controls.Add(this.lblTestResult);
            this.grpTestScan.Controls.Add(this.btnTestValidate);
            this.grpTestScan.Controls.Add(this.txtTestInput);
            this.grpTestScan.Controls.Add(this.lblTestInput);
            this.grpTestScan.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTestScan.Location = new System.Drawing.Point(0, 0);
            this.grpTestScan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpTestScan.Name = "grpTestScan";
            this.grpTestScan.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpTestScan.Size = new System.Drawing.Size(845, 148);
            this.grpTestScan.TabIndex = 0;
            this.grpTestScan.TabStop = false;
            this.grpTestScan.Text = "Test Scan / Parse";
            // 
            // txtTestParse
            // 
            this.txtTestParse.Location = new System.Drawing.Point(117, 74);
            this.txtTestParse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTestParse.Multiline = true;
            this.txtTestParse.Name = "txtTestParse";
            this.txtTestParse.ReadOnly = true;
            this.txtTestParse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestParse.Size = new System.Drawing.Size(466, 56);
            this.txtTestParse.TabIndex = 6;
            // 
            // lblTestParse
            // 
            this.lblTestParse.Location = new System.Drawing.Point(13, 76);
            this.lblTestParse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTestParse.Name = "lblTestParse";
            this.lblTestParse.Size = new System.Drawing.Size(93, 18);
            this.lblTestParse.TabIndex = 7;
            this.lblTestParse.Text = "Parsed";
            // 
            // txtTestResult
            // 
            this.txtTestResult.Location = new System.Drawing.Point(117, 48);
            this.txtTestResult.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTestResult.Name = "txtTestResult";
            this.txtTestResult.ReadOnly = true;
            this.txtTestResult.Size = new System.Drawing.Size(466, 21);
            this.txtTestResult.TabIndex = 4;
            // 
            // lblTestResult
            // 
            this.lblTestResult.Location = new System.Drawing.Point(13, 50);
            this.lblTestResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTestResult.Name = "lblTestResult";
            this.lblTestResult.Size = new System.Drawing.Size(93, 18);
            this.lblTestResult.TabIndex = 8;
            this.lblTestResult.Text = "Result";
            // 
            // btnTestValidate
            // 
            this.btnTestValidate.Location = new System.Drawing.Point(595, 18);
            this.btnTestValidate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnTestValidate.Name = "btnTestValidate";
            this.btnTestValidate.Size = new System.Drawing.Size(117, 23);
            this.btnTestValidate.TabIndex = 2;
            this.btnTestValidate.Text = "Validate/Parse";
            // 
            // txtTestInput
            // 
            this.txtTestInput.Location = new System.Drawing.Point(117, 20);
            this.txtTestInput.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTestInput.Name = "txtTestInput";
            this.txtTestInput.Size = new System.Drawing.Size(466, 21);
            this.txtTestInput.TabIndex = 1;
            // 
            // lblTestInput
            // 
            this.lblTestInput.Location = new System.Drawing.Point(13, 22);
            this.lblTestInput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTestInput.Name = "lblTestInput";
            this.lblTestInput.Size = new System.Drawing.Size(93, 18);
            this.lblTestInput.TabIndex = 9;
            this.lblTestInput.Text = "Barcode";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.chkEnabled);
            this.panelBottom.Controls.Add(this.chkEnableLog);
            this.panelBottom.Controls.Add(this.btnApply);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 481);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(853, 37);
            this.panelBottom.TabIndex = 1;
            // 
            // chkEnabled
            // 
            this.chkEnabled.Location = new System.Drawing.Point(14, 9);
            this.chkEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(187, 18);
            this.chkEnabled.TabIndex = 0;
            this.chkEnabled.Text = "Enable Barcode System";
            // 
            // chkEnableLog
            // 
            this.chkEnableLog.Location = new System.Drawing.Point(210, 9);
            this.chkEnableLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkEnableLog.Name = "chkEnableLog";
            this.chkEnableLog.Size = new System.Drawing.Size(140, 18);
            this.chkEnableLog.TabIndex = 1;
            this.chkEnableLog.Text = "Enable Scan Log";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(639, 7);
            this.btnApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(93, 24);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(744, 7);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(93, 24);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            // 
            // FormBarcodeConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 518);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.panelBottom);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(814, 465);
            this.Name = "FormBarcodeConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Barcode / QR Code Configuration";
            this.tabMain.ResumeLayout(false);
            this.tabScanners.ResumeLayout(false);
            this.splitScanners.Panel1.ResumeLayout(false);
            this.splitScanners.Panel1.PerformLayout();
            this.splitScanners.Panel2.ResumeLayout(false);
            this.splitScanners.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitScanners)).EndInit();
            this.splitScanners.ResumeLayout(false);
            this.toolStripScanners.ResumeLayout(false);
            this.toolStripScanners.PerformLayout();
            this.panelScannerDetail.ResumeLayout(false);
            this.grpValidation.ResumeLayout(false);
            this.grpValidation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValMaxLen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numValMinLen)).EndInit();
            this.grpResultTag.ResumeLayout(false);
            this.grpResultTag.PerformLayout();
            this.grpSerialPort.ResumeLayout(false);
            this.grpSerialPort.PerformLayout();
            this.grpKeyboardWedge.ResumeLayout(false);
            this.grpKeyboardWedge.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyTimeout)).EndInit();
            this.grpScannerBasic.ResumeLayout(false);
            this.grpScannerBasic.PerformLayout();
            this.tabGenerators.ResumeLayout(false);
            this.splitGenerators.Panel1.ResumeLayout(false);
            this.splitGenerators.Panel1.PerformLayout();
            this.splitGenerators.Panel2.ResumeLayout(false);
            this.splitGenerators.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGenerators)).EndInit();
            this.splitGenerators.ResumeLayout(false);
            this.toolStripGenerators.ResumeLayout(false);
            this.toolStripGenerators.PerformLayout();
            this.panelGeneratorDetail.ResumeLayout(false);
            this.grpGenPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).EndInit();
            this.grpGenSize.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numGenHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGenWidth)).EndInit();
            this.grpGenBasic.ResumeLayout(false);
            this.grpGenBasic.PerformLayout();
            this.tabScanLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridScanLog)).EndInit();
            this.panelLogFilter.ResumeLayout(false);
            this.tabTest.ResumeLayout(false);
            this.grpTestGenerate.ResumeLayout(false);
            this.grpTestGenerate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTestBarcode)).EndInit();
            this.grpTestScan.ResumeLayout(false);
            this.grpTestScan.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		#region Multilingual

		/// <summary>
		/// 다국어 텍스트 적용 (한글/영어)
		/// </summary>
		private void ApplyLanguage()
		{
			bool kor = Tools.IsLangKorean();

			// Form title
			this.Text = kor ? "바코드 / QR 코드 설정" : "Barcode / QR Code Configuration";

			// ── Tab titles ──
			tabScanners.Text = kor ? "스캐너" : "Scanners";
			tabGenerators.Text = kor ? "생성기" : "Generators";
			tabScanLog.Text = kor ? "스캔 로그" : "Scan Log";
			tabTest.Text = kor ? "테스트" : "Test";

			// ── Tab 1: Scanner ──
			grpScannerBasic.Text = kor ? "기본" : "Basic";
			lblScannerName.Text = kor ? "이름" : "Name";
			lblInputType.Text = kor ? "입력 방식" : "Input Type";
			chkScannerEnabled.Text = kor ? "사용" : "Enabled";

			grpKeyboardWedge.Text = kor ? "키보드 웨지" : "Keyboard Wedge";
			lblPrefix.Text = kor ? "접두사" : "Prefix";
			lblSuffix.Text = kor ? "접미사" : "Suffix";
			lblKeyTimeout.Text = kor ? "타임아웃(ms)" : "Timeout(ms)";
			lblMinLength.Text = kor ? "최소 길이" : "Min Length";

			grpSerialPort.Text = kor ? "시리얼 포트" : "Serial Port";
			lblPortName.Text = kor ? "포트" : "Port";
			lblBaudRate.Text = kor ? "통신속도" : "Baud Rate";
			lblDataBits.Text = kor ? "데이터 비트" : "Data Bits";
			lblParity.Text = kor ? "패리티" : "Parity";
			lblStopBits.Text = kor ? "정지 비트" : "Stop Bits";
			lblTerminator.Text = kor ? "종료 문자" : "Terminator";

			grpResultTag.Text = kor ? "결과 태그" : "Result Tag";
			lblResultTag.Text = kor ? "태그 이름" : "Tag Name";

			grpValidation.Text = kor ? "유효성 검사" : "Validation";
			chkValidationEnabled.Text = kor ? "유효성 검사 사용" : "Enable Validation";
			lblValMinLen.Text = kor ? "최소 길이" : "Min Length";
			lblValMaxLen.Text = kor ? "최대 길이" : "Max Length";
			lblRequiredPrefix.Text = kor ? "필수 접두사" : "Req. Prefix";
			lblRegex.Text = kor ? "정규식" : "Regex";

			// Scanner list columns
			colScanName.Text = kor ? "이름" : "Name";
			colScanType.Text = kor ? "타입" : "Type";
			colScanEnabled.Text = kor ? "사용" : "On";

			// Scanner toolbar
			btnAddScanner.Text = kor ? "추가" : "Add";
			btnRemoveScanner.Text = kor ? "삭제" : "Remove";

			// ── Tab 2: Generator ──
			grpGenBasic.Text = kor ? "기본" : "Basic";
			lblGenName.Text = kor ? "이름" : "Name";
			lblGenFormat.Text = kor ? "포맷" : "Format";
			lblGenText.Text = kor ? "텍스트" : "Text";
			lblGenTemplate.Text = kor ? "템플릿" : "Template";
			chkAutoUpdate.Text = kor ? "태그 변경 시 자동 업데이트" : "Auto Update on Tag Change";

			grpGenSize.Text = kor ? "크기 / 오류 정정" : "Size / EC Level";
			lblGenWidth.Text = kor ? "너비" : "Width";
			lblGenHeight.Text = kor ? "높이" : "Height";
			lblEcLevel.Text = kor ? "오류 정정" : "EC Level";

			grpGenPreview.Text = kor ? "미리보기" : "Preview";
			btnGeneratePreview.Text = kor ? "생성" : "Generate";
			btnExportImage.Text = kor ? "이미지 내보내기" : "Export Image";

			// Generator list columns
			colGenName.Text = kor ? "이름" : "Name";
			colGenFormat.Text = kor ? "포맷" : "Format";

			// Generator toolbar
			btnAddGenerator.Text = kor ? "추가" : "Add";
			btnRemoveGenerator.Text = kor ? "삭제" : "Remove";

			// ── Tab 3: Scan Log ──
			lblLogDate.Text = kor ? "날짜" : "Date";
			btnLogSearch.Text = kor ? "검색" : "Search";

			colLogTime.HeaderText = kor ? "시간" : "Time";
			colLogOperator.HeaderText = kor ? "작업자" : "Operator";
			colLogBarcode.HeaderText = kor ? "바코드" : "Barcode";
			colLogResult.HeaderText = kor ? "결과" : "Result";
			colLogScanner.HeaderText = kor ? "스캐너" : "Scanner";
			colLogTag.HeaderText = kor ? "태그" : "Tag";
			colLogValue.HeaderText = kor ? "값" : "Value";

			// ── Tab 4: Test ──
			grpTestScan.Text = kor ? "스캔 / 파싱 테스트" : "Test Scan / Parse";
			lblTestInput.Text = kor ? "바코드" : "Barcode";
			btnTestValidate.Text = kor ? "검증/파싱" : "Validate/Parse";
			lblTestResult.Text = kor ? "결과" : "Result";
			lblTestParse.Text = kor ? "파싱 결과" : "Parsed";

			grpTestGenerate.Text = kor ? "생성 테스트" : "Test Generate";
			lblTestGenText.Text = kor ? "텍스트" : "Text";
			lblTestGenFormat.Text = kor ? "포맷" : "Format";
			btnTestGenerate.Text = kor ? "생성" : "Generate";

			// ── Bottom ──
			chkEnabled.Text = kor ? "바코드 시스템 사용" : "Enable Barcode System";
			chkEnableLog.Text = kor ? "스캔 로그 사용" : "Enable Scan Log";
			btnApply.Text = kor ? "적용" : "Apply";
			btnClose.Text = kor ? "닫기" : "Close";
		}

		#endregion

		#region Form Logic (separated from InitializeComponent)

		/// <summary>
		/// InitializeComponent 이후의 폼 초기화 로직.
		/// 콤보박스 데이터 채움, 이벤트 핸들러 연결, 설정 로드.
		/// </summary>
		private void InitializeForm()
		{
			// ComboBox 데이터 채움
			comboInputType.Items.AddRange(new object[] { "Keyboard Wedge", "Serial Port" });
			comboInputType.SelectedIndex = 0;

			string[] ports = SerialPortScanner.GetAvailablePorts();
			comboPortName.Items.AddRange(ports.Length > 0 ? ports : new object[] { "COM1" });
			if (comboPortName.Items.Count > 0) comboPortName.SelectedIndex = 0;

			comboBaudRate.Items.AddRange(new object[] { "2400", "4800", "9600", "19200", "38400", "57600", "115200" });
			comboBaudRate.SelectedIndex = 2; // 9600

			comboDataBits.Items.AddRange(new object[] { "7", "8" });
			comboDataBits.SelectedIndex = 1;

			comboParity.Items.AddRange(new object[] { "None", "Odd", "Even" });
			comboParity.SelectedIndex = 0;

			comboStopBits.Items.AddRange(new object[] { "1", "1.5", "2" });
			comboStopBits.SelectedIndex = 0;

			comboGenFormat.Items.AddRange(new object[] { "QR Code", "DataMatrix", "Code128", "Code39", "EAN-13", "EAN-8", "UPC-A", "ITF-14", "PDF417" });
			comboGenFormat.SelectedIndex = 0;

			comboEcLevel.Items.AddRange(new object[] { "L (7%)", "M (15%)", "Q (25%)", "H (30%)" });
			comboEcLevel.SelectedIndex = 2;

			comboTestGenFormat.Items.AddRange(new object[] { "QR Code", "DataMatrix", "Code128", "Code39", "EAN-13", "EAN-8", "UPC-A", "ITF-14", "PDF417" });
			comboTestGenFormat.SelectedIndex = 0;

			// 이벤트 핸들러 연결
			this.btnAddScanner.Click += BtnAddScanner_Click;
			this.btnRemoveScanner.Click += BtnRemoveScanner_Click;
			this.listViewScanners.SelectedIndexChanged += ListViewScanners_SelectedIndexChanged;
			this.comboInputType.SelectedIndexChanged += ComboInputType_SelectedIndexChanged;

			this.btnAddGenerator.Click += BtnAddGenerator_Click;
			this.btnRemoveGenerator.Click += BtnRemoveGenerator_Click;
			this.listViewGenerators.SelectedIndexChanged += ListViewGenerators_SelectedIndexChanged;

			this.btnGeneratePreview.Click += BtnGeneratePreview_Click;
			this.btnExportImage.Click += BtnExportImage_Click;
			this.btnLogSearch.Click += BtnLogSearch_Click;
			this.btnTestValidate.Click += BtnTestValidate_Click;
			this.btnTestGenerate.Click += BtnTestGenerate_Click;

			this.btnResultTagSelect.Click += BtnResultTagSelect_Click;
			this.btnGenTemplateTag.Click += BtnGenTemplateTag_Click;

			this.btnApply.Click += BtnApply_Click;
			this.btnClose.Click += BtnClose_Click;

			// 설정 로드
			LoadConfigToUI();

			// 초기 상태: 시리얼 그룹 숨김
			UpdateScannerGroupVisibility();
		}

		private void LoadConfigToUI()
		{
			var config = BarcodeManager.LoadConfig();

			chkEnabled.Checked = config.Enabled;
			chkEnableLog.Checked = config.EnableScanLog;

			// 스캐너 목록
			listViewScanners.Items.Clear();
			foreach (var sc in config.Scanners)
			{
				var lvi = new ListViewItem(new[] {
					sc.Name, sc.InputType.ToString(), sc.Enabled ? "Y" : "N" });
				lvi.Tag = sc;
				listViewScanners.Items.Add(lvi);
			}

			// 생성기 목록
			listViewGenerators.Items.Clear();
			foreach (var gc in config.Generators)
			{
				var lvi = new ListViewItem(new[] { gc.Name, gc.Format.ToString() });
				lvi.Tag = gc;
				listViewGenerators.Items.Add(lvi);
			}
		}

		private BarcodeConfig CollectConfigFromUI()
		{
			var config = new BarcodeConfig
			{
				Enabled = chkEnabled.Checked,
				EnableScanLog = chkEnableLog.Checked,
			};

			foreach (ListViewItem lvi in listViewScanners.Items)
			{
				if (lvi.Tag is ScannerConfig sc)
					config.Scanners.Add(sc);
			}

			foreach (ListViewItem lvi in listViewGenerators.Items)
			{
				if (lvi.Tag is GeneratorConfig gc)
					config.Generators.Add(gc);
			}

			return config;
		}

		private void UpdateScannerGroupVisibility()
		{
			bool isSerial = comboInputType.SelectedIndex == 1;
			grpKeyboardWedge.Visible = !isSerial;
			grpSerialPort.Visible = isSerial;
		}

		#endregion

		#region Scanner Event Handlers

		private void BtnAddScanner_Click(object sender, EventArgs e)
		{
			var sc = new ScannerConfig
			{
				Name = $"Scanner{listViewScanners.Items.Count + 1}",
				Enabled = true,
			};
			var lvi = new ListViewItem(new[] {
				sc.Name, sc.InputType.ToString(), sc.Enabled ? "Y" : "N" });
			lvi.Tag = sc;
			listViewScanners.Items.Add(lvi);
			lvi.Selected = true;
		}

		private void BtnRemoveScanner_Click(object sender, EventArgs e)
		{
			if (listViewScanners.SelectedItems.Count > 0)
				listViewScanners.Items.Remove(listViewScanners.SelectedItems[0]);
		}

		private void ListViewScanners_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewScanners.SelectedItems.Count == 0) return;

			var sc = listViewScanners.SelectedItems[0].Tag as ScannerConfig;
			if (sc == null) return;

			txtScannerName.Text = sc.Name;
			comboInputType.SelectedIndex = (int)sc.InputType;
			chkScannerEnabled.Checked = sc.Enabled;

			txtPrefix.Text = sc.Prefix;
			txtSuffix.Text = sc.Suffix;
			numKeyTimeout.Value = Math.Max(numKeyTimeout.Minimum, Math.Min(numKeyTimeout.Maximum, sc.KeyTimeoutMs));
			numMinLength.Value = Math.Max(numMinLength.Minimum, Math.Min(numMinLength.Maximum, sc.MinLength));

			// 시리얼 포트
			SelectComboItem(comboPortName, sc.PortName);
			SelectComboItem(comboBaudRate, sc.BaudRate.ToString());
			SelectComboItem(comboDataBits, sc.DataBits.ToString());
			SelectComboItem(comboParity, sc.Parity);
			SelectComboItem(comboStopBits, sc.StopBits.ToString("0.##"));
			txtTerminator.Text = sc.SerialTerminator.Replace("\r", "\\r").Replace("\n", "\\n");

			txtResultTag.Text = sc.ResultTagName;

			// Validation
			chkValidationEnabled.Checked = sc.Validation.Enabled;
			numValMinLen.Value = sc.Validation.MinLength;
			numValMaxLen.Value = sc.Validation.MaxLength;
			txtRequiredPrefix.Text = sc.Validation.RequiredPrefix;
			txtRegex.Text = sc.Validation.RegexPattern;

			UpdateScannerGroupVisibility();
		}

		private void ComboInputType_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateScannerGroupVisibility();
			SaveCurrentScannerToItem();
		}

		private void SaveCurrentScannerToItem()
		{
			if (listViewScanners.SelectedItems.Count == 0) return;
			var lvi = listViewScanners.SelectedItems[0];

			var sc = new ScannerConfig
			{
				Name = txtScannerName.Text,
				InputType = (ScannerInputType)comboInputType.SelectedIndex,
				Enabled = chkScannerEnabled.Checked,
				Prefix = txtPrefix.Text,
				Suffix = txtSuffix.Text,
				KeyTimeoutMs = (int)numKeyTimeout.Value,
				MinLength = (int)numMinLength.Value,
				PortName = comboPortName.Text,
				BaudRate = int.TryParse(comboBaudRate.Text, out int br) ? br : 9600,
				DataBits = int.TryParse(comboDataBits.Text, out int db) ? db : 8,
				Parity = comboParity.Text,
				StopBits = float.TryParse(comboStopBits.Text, out float sb) ? sb : 1,
				SerialTerminator = txtTerminator.Text.Replace("\\r", "\r").Replace("\\n", "\n"),
				ResultTagName = txtResultTag.Text,
				Validation = new ValidationConfig
				{
					Enabled = chkValidationEnabled.Checked,
					MinLength = (int)numValMinLen.Value,
					MaxLength = (int)numValMaxLen.Value,
					RequiredPrefix = txtRequiredPrefix.Text,
					RegexPattern = txtRegex.Text,
				}
			};

			lvi.Tag = sc;
			lvi.SubItems[0].Text = sc.Name;
			lvi.SubItems[1].Text = sc.InputType.ToString();
			lvi.SubItems[2].Text = sc.Enabled ? "Y" : "N";
		}

		#endregion

		#region Generator Event Handlers

		private void BtnAddGenerator_Click(object sender, EventArgs e)
		{
			var gc = new GeneratorConfig
			{
				Name = $"Gen{listViewGenerators.Items.Count + 1}",
			};
			var lvi = new ListViewItem(new[] { gc.Name, gc.Format.ToString() });
			lvi.Tag = gc;
			listViewGenerators.Items.Add(lvi);
			lvi.Selected = true;
		}

		private void BtnRemoveGenerator_Click(object sender, EventArgs e)
		{
			if (listViewGenerators.SelectedItems.Count > 0)
				listViewGenerators.Items.Remove(listViewGenerators.SelectedItems[0]);
		}

		private void ListViewGenerators_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewGenerators.SelectedItems.Count == 0) return;

			var gc = listViewGenerators.SelectedItems[0].Tag as GeneratorConfig;
			if (gc == null) return;

			txtGenName.Text = gc.Name;
			comboGenFormat.SelectedIndex = (int)gc.Format;
			txtGenText.Text = gc.TextSource;
			txtGenTemplate.Text = gc.TagBindingTemplate;
			chkAutoUpdate.Checked = gc.AutoUpdate;
			numGenWidth.Value = Math.Max(numGenWidth.Minimum, Math.Min(numGenWidth.Maximum, gc.Width));
			numGenHeight.Value = Math.Max(numGenHeight.Minimum, Math.Min(numGenHeight.Maximum, gc.Height));
			comboEcLevel.SelectedIndex = (int)gc.ErrorCorrectionLevel;
		}

		private void BtnGeneratePreview_Click(object sender, EventArgs e)
		{
			try
			{
				string text = !string.IsNullOrEmpty(txtGenTemplate.Text)
					? BarcodeGenerator.ResolveTemplate(txtGenTemplate.Text)
					: txtGenText.Text;

				if (string.IsNullOrEmpty(text))
				{
					string msg = Tools.IsLangKorean() ? "텍스트 또는 템플릿을 입력하세요." : "Please enter text or template.";
					MessageBox.Show(msg, "Barcode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				var bmp = BarcodeManager.GenerateBarcode(
					text,
					(BarcodeFormat)comboGenFormat.SelectedIndex,
					(int)numGenWidth.Value,
					(int)numGenHeight.Value,
					(QRErrorCorrectionLevel)comboEcLevel.SelectedIndex);

				if (bmp != null)
				{
					picBarcode.Image?.Dispose();
					picBarcode.Image = bmp;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnExportImage_Click(object sender, EventArgs e)
		{
			if (picBarcode.Image == null)
			{
				string msg = Tools.IsLangKorean() ? "먼저 바코드를 생성하세요." : "Generate a barcode first.";
				MessageBox.Show(msg, "Barcode", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			using (var sfd = new SaveFileDialog())
			{
				sfd.Filter = "PNG Image|*.png|Bitmap Image|*.bmp";
				sfd.DefaultExt = "png";
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					var format = sfd.FilterIndex == 2
						? System.Drawing.Imaging.ImageFormat.Bmp
						: System.Drawing.Imaging.ImageFormat.Png;
					picBarcode.Image.Save(sfd.FileName, format);
				}
			}
		}

		#endregion

		#region Scan Log

		private void BtnLogSearch_Click(object sender, EventArgs e)
		{
			gridScanLog.Rows.Clear();
			string logDir = System.IO.Path.Combine(Application.StartupPath, "BarcodeLog");
			var logger = new ScanEventLogger(logDir);
			var records = logger.ReadLogs(dtpLogDate.Value.Date);

			foreach (var r in records)
			{
				gridScanLog.Rows.Add(
					r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
					r.Operator,
					r.BarcodeValue,
					r.ValidationResult.ToString(),
					r.ScannerId,
					r.ResultTagName,
					r.ResultTagValue);
			}
		}

		#endregion

		#region Test

		private void BtnTestValidate_Click(object sender, EventArgs e)
		{
			string input = txtTestInput.Text;
			if (string.IsNullOrEmpty(input))
			{
				txtTestResult.Text = Tools.IsLangKorean() ? "입력이 비어 있습니다" : "Empty input";
				return;
			}

			// Validation test
			var valConfig = new ValidationConfig
			{
				Enabled = chkValidationEnabled.Checked,
				MinLength = (int)numValMinLen.Value,
				MaxLength = (int)numValMaxLen.Value,
				RequiredPrefix = txtRequiredPrefix.Text,
				RegexPattern = txtRegex.Text,
			};
			var result = BarcodeValidator.Validate(input, valConfig);
			txtTestResult.Text = result.ToString();

			// Parse test
			var format = QRDataParser.DetectFormat(input);
			var sb = new System.Text.StringBuilder();
			sb.AppendLine($"Format: {format}");

			if (format == QRDataFormat.KeyValue)
			{
				var kvp = QRDataParser.ParseKeyValue(input);
				foreach (var kv in kvp)
					sb.AppendLine($"  {kv.Key} = {kv.Value}");
			}
			else if (format == QRDataFormat.JSON)
			{
				var kvp = QRDataParser.ParseSimpleJson(input);
				foreach (var kv in kvp)
					sb.AppendLine($"  {kv.Key} = {kv.Value}");
			}

			txtTestParse.Text = sb.ToString();
		}

		private void BtnTestGenerate_Click(object sender, EventArgs e)
		{
			string text = txtTestGenText.Text;
			if (string.IsNullOrEmpty(text))
			{
				string msg = Tools.IsLangKorean() ? "텍스트를 입력하세요." : "Please enter text.";
				MessageBox.Show(msg, "Test", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var bmp = BarcodeManager.GenerateBarcode(
				text,
				(BarcodeFormat)comboTestGenFormat.SelectedIndex);

			if (bmp != null)
			{
				// ZXing이 반환하는 Indexed PixelFormat 비트맵은 PictureBox에서 오류 발생
				// 32bppArgb로 복사하여 사용
				var safeBmp = new System.Drawing.Bitmap(bmp.Width, bmp.Height,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				using (var g = System.Drawing.Graphics.FromImage(safeBmp))
				{
					g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
				}
				bmp.Dispose();

				picTestBarcode.Image?.Dispose();
				picTestBarcode.Image = safeBmp;
			}
		}

		#endregion

		#region Bottom Buttons

		private void BtnApply_Click(object sender, EventArgs e)
		{
			// 현재 선택된 스캐너의 편집 내용을 저장
			SaveCurrentScannerToItem();

			var config = CollectConfigFromUI();
			BarcodeManager.ApplyConfig(config);
			string msg = Tools.IsLangKorean() ? "설정이 적용 및 저장되었습니다." : "Configuration applied and saved.";
			MessageBox.Show(msg, "Barcode", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void BtnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void BtnResultTagSelect_Click(object sender, EventArgs e)
		{
			string tag, des;
			if (SelectTag.SelectAll(this, out tag, out des) == DialogResult.OK)
			{
				txtResultTag.Text = tag;
			}
		}

		private void BtnGenTemplateTag_Click(object sender, EventArgs e)
		{
			string tag, des;
			if (SelectTag.SelectAll(this, out tag, out des) == DialogResult.OK)
			{
				// 커서 위치에 {Tag.xxx} 형태로 삽입
				string insert = "{Tag." + tag + "}";
				int pos = txtGenTemplate.SelectionStart;
				txtGenTemplate.Text = txtGenTemplate.Text.Insert(pos, insert);
				txtGenTemplate.SelectionStart = pos + insert.Length;
				txtGenTemplate.Focus();
			}
		}

		#endregion

		#region Utility

		private static void SelectComboItem(ComboBox combo, string value)
		{
			for (int i = 0; i < combo.Items.Count; i++)
			{
				if (combo.Items[i].ToString() == value)
				{
					combo.SelectedIndex = i;
					return;
				}
			}
			if (combo.Items.Count > 0)
				combo.SelectedIndex = 0;
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
