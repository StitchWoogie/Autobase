using System;
using System.Drawing;
using System.Windows.Forms;

namespace LocalMain.DemandNew
{
	partial class FormDemandNewSettings
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
				components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.tabControl = new TabControl();
			this.tabGeneral = new TabPage();
			this.tabContract = new TabPage();
			this.tabMeter = new TabPage();
			this.tabLoads = new TabPage();
			this.tabPolicy = new TabPage();
			this.tabForecast = new TabPage();
			this.tabStorage = new TabPage();
			this.panelBtn = new Panel();

			// 일반 탭 컨트롤
			this.lblBlockId = new Label();
			this.cboBlockId = new ComboBox();
			this.lblTitle = new Label();
			this.txtTitle = new TextBox();
			this.lblMode = new Label();
			this.rdoShadow = new RadioButton();
			this.rdoActive = new RadioButton();
			this.lblInterval = new Label();
			this.cboInterval = new ComboBox();
			this.chkClockAligned = new CheckBox();

			// 계약 탭 컨트롤
			this.lblContractKW = new Label();
			this.numContractKW = new NumericUpDown();
			this.lblSafetyFactor = new Label();
			this.numSafetyFactor = new NumericUpDown();
			this.lblTimeZoneTargets = new Label();
			this.gridTimeZone = new DataGridView();

			// 계측 탭 컨트롤
			this.lblMeterTag = new Label();
			this.txtMeterTag = new TextBox();
			this.btnMeterTag = new Button();
			this.lblMeterType = new Label();
			this.cboMeterType = new ComboBox();
			this.lblPulseRatio = new Label();
			this.numPulseRatio = new NumericUpDown();
			this.chkAutoReset = new CheckBox();
			this.chkUseTargetTag = new CheckBox();
			this.txtTargetTag = new TextBox();
			this.btnTargetTag = new Button();
			this.chkUseEOI = new CheckBox();
			this.txtEOITag = new TextBox();
			this.btnEOITag = new Button();
			this.lblPredictionDisplay = new Label();
			this.txtPredictionTag = new TextBox();
			this.btnPredictionTag = new Button();
			this.lblCommStatusTag = new Label();
			this.txtCommStatusTag = new TextBox();
			this.btnCommStatusTag = new Button();

			// 부하 탭 컨트롤
			this.lvLoads = new ListView();
			this.btnAddLoad = new Button();
			this.btnEditLoad = new Button();
			this.btnDeleteLoad = new Button();

			// 정책 탭 컨트롤
			this.lblShedMargin = new Label();
			this.numShedMargin = new NumericUpDown();
			this.lblRestoreMargin = new Label();
			this.numRestoreMargin = new NumericUpDown();
			this.lblProtectionTime = new Label();
			this.numProtectionTime = new NumericUpDown();
			this.chkMultiStep = new CheckBox();
			this.lblStepCount = new Label();
			this.cboStepCount = new ComboBox();
			this.pnlStepDetail = new Panel();
			this.grpStep1 = new GroupBox();
			this.grpStep2 = new GroupBox();
			this.grpStep3 = new GroupBox();
			this.lblStepReduction1 = new Label();
			this.numStepReductionKW1 = new NumericUpDown();
			this.lblStepLoads1 = new Label();
			this.clbStepLoads1 = new CheckedListBox();
			this.lblStepReduction2 = new Label();
			this.numStepReductionKW2 = new NumericUpDown();
			this.lblStepLoads2 = new Label();
			this.clbStepLoads2 = new CheckedListBox();
			this.lblStepReduction3 = new Label();
			this.numStepReductionKW3 = new NumericUpDown();
			this.lblStepLoads3 = new Label();
			this.clbStepLoads3 = new CheckedListBox();

			// 예측 탭 컨트롤
			this.lblTrendWindow = new Label();
			this.trkTrendWindow = new TrackBar();
			this.lblTrendValue = new Label();
			this.lblEwmaAlpha = new Label();
			this.trkEwmaAlpha = new TrackBar();
			this.lblAlphaValue = new Label();
			this.lblForecastDesc = new Label();

			// 저장 탭 컨트롤
			this.chkDbSave = new CheckBox();
			this.lblDsn = new Label();
			this.cboDsn = new ComboBox();
			this.chkFailover = new CheckBox();
			this.lblCsvExport = new Label();
			this.lblFrom = new Label();
			this.dtpFrom = new DateTimePicker();
			this.lblTo = new Label();
			this.dtpTo = new DateTimePicker();
			this.lblExportPath = new Label();
			this.txtExportPath = new TextBox();
			this.btnExportInterval = new Button();
			this.btnExportEvent = new Button();
			this.btnExportPeak = new Button();
			this.btnRecoverBackup = new Button();

			// 하단 버튼
			this.btnOK = new Button();
			this.btnCancel = new Button();
			this.btnApply = new Button();

			((System.ComponentModel.ISupportInitialize)(this.gridTimeZone)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkTrendWindow)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkEwmaAlpha)).BeginInit();
			this.SuspendLayout();

			// =====================================================
			// Tab 1: General
			// =====================================================
			//
			// lblBlockId
			//
			this.lblBlockId.Text = "Block ID:";
			this.lblBlockId.Location = new Point(20, 20);
			this.lblBlockId.AutoSize = true;
			//
			// cboBlockId
			//
			this.cboBlockId.Location = new Point(160, 20);
			this.cboBlockId.Width = 200;
			this.cboBlockId.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboBlockId.SelectedIndexChanged += new EventHandler(this.CboBlockId_SelectedIndexChanged);
			//
			// lblTitle
			//
			this.lblTitle.Text = "Title:";
			this.lblTitle.Location = new Point(20, 50);
			this.lblTitle.AutoSize = true;
			//
			// txtTitle
			//
			this.txtTitle.Location = new Point(160, 50);
			this.txtTitle.Width = 200;
			//
			// lblMode
			//
			this.lblMode.Text = "Mode:";
			this.lblMode.Location = new Point(20, 80);
			this.lblMode.AutoSize = true;
			//
			// rdoShadow
			//
			this.rdoShadow.Text = "Shadow";
			this.rdoShadow.Location = new Point(160, 80);
			this.rdoShadow.AutoSize = true;
			//
			// rdoActive
			//
			this.rdoActive.Text = "Active";
			this.rdoActive.Location = new Point(300, 80);
			this.rdoActive.AutoSize = true;
			//
			// lblInterval
			//
			this.lblInterval.Text = "Interval (min):";
			this.lblInterval.Location = new Point(20, 110);
			this.lblInterval.AutoSize = true;
			//
			// cboInterval
			//
			this.cboInterval.Location = new Point(160, 110);
			this.cboInterval.Width = 80;
			this.cboInterval.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboInterval.Items.AddRange(new object[] { 5, 10, 15, 30, 60 });
			//
			// chkClockAligned
			//
			this.chkClockAligned.Text = "Clock Aligned";
			this.chkClockAligned.Location = new Point(160, 140);
			this.chkClockAligned.AutoSize = true;
			//
			// tabGeneral
			//
			this.tabGeneral.Text = "General";
			this.tabGeneral.Controls.AddRange(new Control[] {
				this.lblBlockId, this.cboBlockId,
				this.lblTitle, this.txtTitle,
				this.lblMode, this.rdoShadow, this.rdoActive,
				this.lblInterval, this.cboInterval,
				this.chkClockAligned
			});

			// =====================================================
			// Tab 2: Contract/Tariff
			// =====================================================
			//
			// lblContractKW
			//
			this.lblContractKW.Text = "Contract KW:";
			this.lblContractKW.Location = new Point(20, 20);
			this.lblContractKW.AutoSize = true;
			//
			// numContractKW
			//
			this.numContractKW.Location = new Point(160, 20);
			this.numContractKW.Width = 120;
			this.numContractKW.Minimum = 0;
			this.numContractKW.Maximum = 999999;
			this.numContractKW.DecimalPlaces = 0;
			//
			// lblSafetyFactor
			//
			this.lblSafetyFactor.Text = "Safety Factor:";
			this.lblSafetyFactor.Location = new Point(20, 50);
			this.lblSafetyFactor.AutoSize = true;
			//
			// numSafetyFactor
			//
			this.numSafetyFactor.Location = new Point(160, 50);
			this.numSafetyFactor.Width = 80;
			this.numSafetyFactor.Minimum = 0.50M;
			this.numSafetyFactor.Maximum = 1.00M;
			this.numSafetyFactor.DecimalPlaces = 2;
			this.numSafetyFactor.Increment = 0.01M;
			//
			// lblTimeZoneTargets
			//
			this.lblTimeZoneTargets.Text = "TimeZone Targets:";
			this.lblTimeZoneTargets.Location = new Point(20, 90);
			this.lblTimeZoneTargets.AutoSize = true;
			//
			// gridTimeZone
			//
			this.gridTimeZone.Location = new Point(20, 118);
			this.gridTimeZone.Size = new Size(600, 200);
			this.gridTimeZone.AllowUserToAddRows = true;
			this.gridTimeZone.AllowUserToDeleteRows = true;
			this.gridTimeZone.EditMode = DataGridViewEditMode.EditOnEnter;
			this.gridTimeZone.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			this.gridTimeZone.DataError += new DataGridViewDataErrorEventHandler(this.gridTimeZone_DataError);
			//
			// gridTimeZone columns
			//
			var colZone = new DataGridViewComboBoxColumn();
			colZone.HeaderText = "Zone";
			colZone.Name = "Zone";
			colZone.Width = 100;
			colZone.Items.AddRange("OffPeak", "MidPeak", "OnPeak", "SuperPeak");
			this.gridTimeZone.Columns.Add(colZone);

			var colStart = new DataGridViewComboBoxColumn();
			colStart.HeaderText = "Start (HH:MM)";
			colStart.Name = "StartTime";
			colStart.Width = 110;
			colStart.Items.AddRange(new object[] {
				"00:00", "00:30", "01:00", "01:30", "02:00", "02:30",
				"03:00", "03:30", "04:00", "04:30", "05:00", "05:30",
				"06:00", "06:30", "07:00", "07:30", "08:00", "08:30",
				"09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
				"12:00", "12:30", "13:00", "13:30", "14:00", "14:30",
				"15:00", "15:30", "16:00", "16:30", "17:00", "17:30",
				"18:00", "18:30", "19:00", "19:30", "20:00", "20:30",
				"21:00", "21:30", "22:00", "22:30", "23:00", "23:30"
			});
			var colEnd = new DataGridViewComboBoxColumn();
			colEnd.HeaderText = "End (HH:MM)";
			colEnd.Name = "EndTime";
			colEnd.Width = 110;
			colEnd.Items.AddRange(new object[] {
				"00:00", "00:30", "01:00", "01:30", "02:00", "02:30",
				"03:00", "03:30", "04:00", "04:30", "05:00", "05:30",
				"06:00", "06:30", "07:00", "07:30", "08:00", "08:30",
				"09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
				"12:00", "12:30", "13:00", "13:30", "14:00", "14:30",
				"15:00", "15:30", "16:00", "16:30", "17:00", "17:30",
				"18:00", "18:30", "19:00", "19:30", "20:00", "20:30",
				"21:00", "21:30", "22:00", "22:30", "23:00", "23:30"
			});
			this.gridTimeZone.Columns.Add(colStart);
			this.gridTimeZone.Columns.Add(colEnd);
			this.gridTimeZone.Columns.Add("TargetKW", "Target KW");
			//
			// tabContract
			//
			this.tabContract.Text = "Contract/Tariff";
			this.tabContract.Controls.AddRange(new Control[] {
				this.lblContractKW, this.numContractKW,
				this.lblSafetyFactor, this.numSafetyFactor,
				this.lblTimeZoneTargets, this.gridTimeZone
			});

			// =====================================================
			// Tab 3: Metering
			// =====================================================
			//
			// lblMeterTag
			//
			this.lblMeterTag.Text = "Meter Tag:";
			this.lblMeterTag.Location = new Point(20, 20);
			this.lblMeterTag.AutoSize = true;
			//
			// txtMeterTag
			//
			this.txtMeterTag.Location = new Point(160, 20);
			this.txtMeterTag.Width = 200;
			//
			// btnMeterTag
			//
			this.btnMeterTag.Text = "...";
			this.btnMeterTag.Location = new Point(365, 20);
			this.btnMeterTag.Width = 30;
			this.btnMeterTag.Click += new EventHandler(this.btnMeterTag_Click);
			//
			// lblMeterType
			//
			this.lblMeterType.Text = "Meter Type:";
			this.lblMeterType.Location = new Point(20, 50);
			this.lblMeterType.AutoSize = true;
			//
			// cboMeterType
			//
			this.cboMeterType.Location = new Point(160, 50);
			this.cboMeterType.Width = 120;
			this.cboMeterType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboMeterType.Items.AddRange(new object[] { "Direct kW", "Delta kWh", "Pulse" });
			//
			// lblPulseRatio
			//
			this.lblPulseRatio.Text = "Pulse Ratio:";
			this.lblPulseRatio.Location = new Point(20, 80);
			this.lblPulseRatio.AutoSize = true;
			//
			// numPulseRatio
			//
			this.numPulseRatio.Location = new Point(160, 80);
			this.numPulseRatio.Width = 120;
			this.numPulseRatio.Minimum = 0.001M;
			this.numPulseRatio.Maximum = 99999;
			this.numPulseRatio.DecimalPlaces = 3;
			this.numPulseRatio.Value = 1;
			//
			// chkAutoReset
			//
			this.chkAutoReset.Text = "Auto Reset on Pulse Drop";
			this.chkAutoReset.Location = new Point(160, 110);
			this.chkAutoReset.AutoSize = true;
			//
			// chkUseTargetTag
			//
			this.chkUseTargetTag.Text = "Use Target Tag:";
			this.chkUseTargetTag.Location = new Point(20, 140);
			this.chkUseTargetTag.AutoSize = true;
			//
			// txtTargetTag
			//
			this.txtTargetTag.Location = new Point(160, 140);
			this.txtTargetTag.Width = 200;
			//
			// btnTargetTag
			//
			this.btnTargetTag.Text = "...";
			this.btnTargetTag.Location = new Point(365, 140);
			this.btnTargetTag.Width = 30;
			this.btnTargetTag.Click += new EventHandler(this.btnTargetTag_Click);
			//
			// chkUseEOI
			//
			this.chkUseEOI.Text = "Use EOI Tag:";
			this.chkUseEOI.Location = new Point(20, 170);
			this.chkUseEOI.AutoSize = true;
			//
			// txtEOITag
			//
			this.txtEOITag.Location = new Point(160, 170);
			this.txtEOITag.Width = 200;
			//
			// btnEOITag
			//
			this.btnEOITag.Text = "...";
			this.btnEOITag.Location = new Point(365, 170);
			this.btnEOITag.Width = 30;
			this.btnEOITag.Click += new EventHandler(this.btnEOITag_Click);
			//
			// lblPredictionDisplay
			//
			this.lblPredictionDisplay.Text = "Prediction Display:";
			this.lblPredictionDisplay.Location = new Point(20, 200);
			this.lblPredictionDisplay.AutoSize = true;
			//
			// txtPredictionTag
			//
			this.txtPredictionTag.Location = new Point(160, 200);
			this.txtPredictionTag.Width = 200;
			//
			// btnPredictionTag
			//
			this.btnPredictionTag.Text = "...";
			this.btnPredictionTag.Location = new Point(365, 200);
			this.btnPredictionTag.Width = 30;
			this.btnPredictionTag.Click += new EventHandler(this.btnPredictionTag_Click);
			//
			// lblCommStatusTag
			//
			this.lblCommStatusTag.Text = "Comm Status Tag:";
			this.lblCommStatusTag.Location = new Point(20, 230);
			this.lblCommStatusTag.AutoSize = true;
			//
			// txtCommStatusTag
			//
			this.txtCommStatusTag.Location = new Point(160, 230);
			this.txtCommStatusTag.Width = 200;
			//
			// btnCommStatusTag
			//
			this.btnCommStatusTag.Text = "...";
			this.btnCommStatusTag.Location = new Point(365, 230);
			this.btnCommStatusTag.Width = 30;
			this.btnCommStatusTag.Click += new EventHandler(this.btnCommStatusTag_Click);
			//
			// tabMeter
			//
			this.tabMeter.Text = "Metering";
			this.tabMeter.Controls.AddRange(new Control[] {
				this.lblMeterTag, this.txtMeterTag, this.btnMeterTag,
				this.lblMeterType, this.cboMeterType,
				this.lblPulseRatio, this.numPulseRatio,
				this.chkAutoReset,
				this.chkUseTargetTag, this.txtTargetTag, this.btnTargetTag,
				this.chkUseEOI, this.txtEOITag, this.btnEOITag,
				this.lblPredictionDisplay, this.txtPredictionTag, this.btnPredictionTag,
				this.lblCommStatusTag, this.txtCommStatusTag, this.btnCommStatusTag
			});

			// =====================================================
			// Tab 4: Loads
			// =====================================================
			//
			// lvLoads
			//
			this.lvLoads.View = View.Details;
			this.lvLoads.FullRowSelect = true;
			this.lvLoads.GridLines = true;
			this.lvLoads.Location = new Point(20, 20);
			this.lvLoads.Size = new Size(660, 300);
			this.lvLoads.Columns.Add("ID", 50);
			this.lvLoads.Columns.Add("Name", 90);
			this.lvLoads.Columns.Add("Priority", 60);
			this.lvLoads.Columns.Add("Group", 55);
			this.lvLoads.Columns.Add("Est.KW", 65);
			this.lvLoads.Columns.Add("Command Tag", 110);
			this.lvLoads.Columns.Add("Feedback Tag", 95);
			this.lvLoads.Columns.Add("MinOff(s)", 65);
			this.lvLoads.Columns.Add("MinOn(s)", 65);
			//
			// btnAddLoad
			//
			this.btnAddLoad.Text = "Add";
			this.btnAddLoad.Location = new Point(20, 330);
			this.btnAddLoad.Width = 80;
			this.btnAddLoad.Click += new EventHandler(this.BtnAddLoad_Click);
			//
			// btnEditLoad
			//
			this.btnEditLoad.Text = "Edit";
			this.btnEditLoad.Location = new Point(110, 330);
			this.btnEditLoad.Width = 80;
			this.btnEditLoad.Click += new EventHandler(this.BtnEditLoad_Click);
			//
			// btnDeleteLoad
			//
			this.btnDeleteLoad.Text = "Delete";
			this.btnDeleteLoad.Location = new Point(200, 330);
			this.btnDeleteLoad.Width = 80;
			this.btnDeleteLoad.Click += new EventHandler(this.BtnDeleteLoad_Click);
			//
			// tabLoads
			//
			this.tabLoads.Text = "Loads";
			this.tabLoads.Controls.AddRange(new Control[] {
				this.lvLoads, this.btnAddLoad, this.btnEditLoad, this.btnDeleteLoad
			});

			// =====================================================
			// Tab 5: Policy
			// =====================================================
			//
			// lblShedMargin
			//
			this.lblShedMargin.Text = "Shed Margin KW:";
			this.lblShedMargin.Location = new Point(20, 20);
			this.lblShedMargin.AutoSize = true;
			//
			// numShedMargin
			//
			this.numShedMargin.Location = new Point(200, 20);
			this.numShedMargin.Width = 100;
			this.numShedMargin.Minimum = 0;
			this.numShedMargin.Maximum = 9999;
			this.numShedMargin.DecimalPlaces = 1;
			//
			// lblRestoreMargin
			//
			this.lblRestoreMargin.Text = "Restore Margin KW:";
			this.lblRestoreMargin.Location = new Point(20, 50);
			this.lblRestoreMargin.AutoSize = true;
			//
			// numRestoreMargin
			//
			this.numRestoreMargin.Location = new Point(200, 50);
			this.numRestoreMargin.Width = 100;
			this.numRestoreMargin.Minimum = 0;
			this.numRestoreMargin.Maximum = 9999;
			this.numRestoreMargin.DecimalPlaces = 1;
			//
			// lblProtectionTime
			//
			this.lblProtectionTime.Text = "Protection Time (sec):";
			this.lblProtectionTime.Location = new Point(20, 80);
			this.lblProtectionTime.AutoSize = true;
			//
			// numProtectionTime
			//
			this.numProtectionTime.Location = new Point(200, 80);
			this.numProtectionTime.Width = 100;
			this.numProtectionTime.Minimum = 0;
			this.numProtectionTime.Maximum = 3600;
			this.numProtectionTime.Value = 60;
			//
			// chkMultiStep
			//
			this.chkMultiStep.Text = "Multi-Step Control";
			this.chkMultiStep.Location = new Point(20, 110);
			this.chkMultiStep.AutoSize = true;
			this.chkMultiStep.CheckedChanged += new EventHandler(this.chkMultiStep_CheckedChanged);
			//
			// lblStepCount
			//
			this.lblStepCount.Text = "Step Count:";
			this.lblStepCount.Location = new Point(20, 140);
			this.lblStepCount.AutoSize = true;
			//
			// cboStepCount
			//
			this.cboStepCount.Location = new Point(200, 140);
			this.cboStepCount.Width = 60;
			this.cboStepCount.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cboStepCount.Items.AddRange(new object[] { 1, 2, 3 });
			this.cboStepCount.SelectedIndexChanged += new EventHandler(this.cboStepCount_SelectedIndexChanged);
			//
			// pnlStepDetail
			//
			this.pnlStepDetail.Location = new Point(10, 180);
			this.pnlStepDetail.Size = new Size(660, 200);
			this.pnlStepDetail.Visible = false;
			//
			// Step 1
			//
			this.grpStep1.Text = "Step 1";
			this.grpStep1.Location = new Point(0, 0);
			this.grpStep1.Size = new Size(205, 195);
			this.lblStepReduction1.Text = "Reduction(kW):";
			this.lblStepReduction1.Location = new Point(8, 20);
			this.lblStepReduction1.AutoSize = true;
			this.numStepReductionKW1.Location = new Point(110, 18);
			this.numStepReductionKW1.Width = 85;
			this.numStepReductionKW1.Minimum = 0;
			this.numStepReductionKW1.Maximum = 99999;
			this.numStepReductionKW1.DecimalPlaces = 0;
			this.lblStepLoads1.Text = "Loads:";
			this.lblStepLoads1.Location = new Point(8, 52);
			this.lblStepLoads1.AutoSize = true;
			this.clbStepLoads1.Location = new Point(8, 76);
			this.clbStepLoads1.Size = new Size(187, 108);
			this.clbStepLoads1.CheckOnClick = true;
			this.grpStep1.Controls.AddRange(new Control[] { this.lblStepReduction1, this.numStepReductionKW1, this.lblStepLoads1, this.clbStepLoads1 });
			//
			// Step 2
			//
			this.grpStep2.Text = "Step 2";
			this.grpStep2.Location = new Point(215, 0);
			this.grpStep2.Size = new Size(205, 195);
			this.lblStepReduction2.Text = "Reduction(kW):";
			this.lblStepReduction2.Location = new Point(8, 20);
			this.lblStepReduction2.AutoSize = true;
			this.numStepReductionKW2.Location = new Point(110, 18);
			this.numStepReductionKW2.Width = 85;
			this.numStepReductionKW2.Minimum = 0;
			this.numStepReductionKW2.Maximum = 99999;
			this.numStepReductionKW2.DecimalPlaces = 0;
			this.lblStepLoads2.Text = "Loads:";
			this.lblStepLoads2.Location = new Point(8, 52);
			this.lblStepLoads2.AutoSize = true;
			this.clbStepLoads2.Location = new Point(8, 76);
			this.clbStepLoads2.Size = new Size(187, 108);
			this.clbStepLoads2.CheckOnClick = true;
			this.grpStep2.Controls.AddRange(new Control[] { this.lblStepReduction2, this.numStepReductionKW2, this.lblStepLoads2, this.clbStepLoads2 });
			//
			// Step 3
			//
			this.grpStep3.Text = "Step 3";
			this.grpStep3.Location = new Point(430, 0);
			this.grpStep3.Size = new Size(205, 195);
			this.lblStepReduction3.Text = "Reduction(kW):";
			this.lblStepReduction3.Location = new Point(8, 20);
			this.lblStepReduction3.AutoSize = true;
			this.numStepReductionKW3.Location = new Point(110, 18);
			this.numStepReductionKW3.Width = 85;
			this.numStepReductionKW3.Minimum = 0;
			this.numStepReductionKW3.Maximum = 99999;
			this.numStepReductionKW3.DecimalPlaces = 0;
			this.lblStepLoads3.Text = "Loads:";
			this.lblStepLoads3.Location = new Point(8, 52);
			this.lblStepLoads3.AutoSize = true;
			this.clbStepLoads3.Location = new Point(8, 76);
			this.clbStepLoads3.Size = new Size(187, 108);
			this.clbStepLoads3.CheckOnClick = true;
			this.grpStep3.Controls.AddRange(new Control[] { this.lblStepReduction3, this.numStepReductionKW3, this.lblStepLoads3, this.clbStepLoads3 });

			this.pnlStepDetail.Controls.AddRange(new Control[] { this.grpStep1, this.grpStep2, this.grpStep3 });
			//
			// tabPolicy
			//
			this.tabPolicy.Text = "Policy";
			this.tabPolicy.Controls.AddRange(new Control[] {
				this.lblShedMargin, this.numShedMargin,
				this.lblRestoreMargin, this.numRestoreMargin,
				this.lblProtectionTime, this.numProtectionTime,
				this.chkMultiStep,
				this.lblStepCount, this.cboStepCount,
				this.pnlStepDetail
			});

			// =====================================================
			// Tab 6: Forecast
			// =====================================================
			//
			// lblTrendWindow
			//
			this.lblTrendWindow.Text = "Trend Window (sec):";
			this.lblTrendWindow.Location = new Point(20, 20);
			this.lblTrendWindow.AutoSize = true;
			//
			// trkTrendWindow
			//
			this.trkTrendWindow.Location = new Point(20, 45);
			this.trkTrendWindow.Width = 400;
			this.trkTrendWindow.Minimum = 30;
			this.trkTrendWindow.Maximum = 300;
			this.trkTrendWindow.Value = 120;
			this.trkTrendWindow.TickFrequency = 30;
			this.trkTrendWindow.ValueChanged += new EventHandler(this.trkTrendWindow_ValueChanged);
			//
			// lblTrendValue
			//
			this.lblTrendValue.Text = "120";
			this.lblTrendValue.Location = new Point(440, 50);
			this.lblTrendValue.AutoSize = true;
			//
			// lblEwmaAlpha
			//
			this.lblEwmaAlpha.Text = "EWMA Alpha:";
			this.lblEwmaAlpha.Location = new Point(20, 105);
			this.lblEwmaAlpha.AutoSize = true;
			//
			// trkEwmaAlpha
			//
			this.trkEwmaAlpha.Location = new Point(20, 130);
			this.trkEwmaAlpha.Width = 400;
			this.trkEwmaAlpha.Minimum = 5;
			this.trkEwmaAlpha.Maximum = 50;
			this.trkEwmaAlpha.Value = 30;
			this.trkEwmaAlpha.TickFrequency = 5;
			this.trkEwmaAlpha.ValueChanged += new EventHandler(this.trkEwmaAlpha_ValueChanged);
			//
			// lblAlphaValue
			//
			this.lblAlphaValue.Text = "0.30";
			this.lblAlphaValue.Location = new Point(440, 135);
			this.lblAlphaValue.AutoSize = true;
			//
			// lblForecastDesc
			//
			this.lblForecastDesc.Location = new Point(20, 190);
			this.lblForecastDesc.Size = new Size(500, 100);
			this.lblForecastDesc.Text = "EWMA Alpha: Higher = more responsive to recent data (0.1=stable, 0.5=sensitive)\nTrend Window: Recent data range used for prediction (seconds)";
			//
			// tabForecast
			//
			this.tabForecast.Text = "Forecast";
			this.tabForecast.Controls.AddRange(new Control[] {
				this.lblTrendWindow, this.trkTrendWindow, this.lblTrendValue,
				this.lblEwmaAlpha, this.trkEwmaAlpha, this.lblAlphaValue,
				this.lblForecastDesc
			});

			// =====================================================
			// Tab 7: Storage/Export
			// =====================================================
			//
			// chkDbSave
			//
			this.chkDbSave.Text = "Enable DB Save";
			this.chkDbSave.Location = new Point(20, 20);
			this.chkDbSave.AutoSize = true;
			//
			// lblDsn
			//
			this.lblDsn.Text = "DSN:";
			this.lblDsn.Location = new Point(20, 50);
			this.lblDsn.AutoSize = true;
			//
			// cboDsn
			//
			this.cboDsn.Location = new Point(160, 50);
			this.cboDsn.Width = 200;
			this.cboDsn.DropDownStyle = ComboBoxStyle.DropDownList;
			//
			// chkFailover
			//
			this.chkFailover.Text = "Enable Failover (JSON backup)";
			this.chkFailover.Location = new Point(20, 80);
			this.chkFailover.AutoSize = true;
			//
			// lblCsvExport
			//
			this.lblCsvExport.Text = "── CSV Export ──";
			this.lblCsvExport.Location = new Point(20, 120);
			this.lblCsvExport.AutoSize = true;
			//
			// lblFrom
			//
			this.lblFrom.Text = "From:";
			this.lblFrom.Location = new Point(20, 145);
			this.lblFrom.AutoSize = true;
			//
			// dtpFrom
			//
			this.dtpFrom.Location = new Point(160, 145);
			this.dtpFrom.Width = 200;
			this.dtpFrom.Format = DateTimePickerFormat.Short;
			//
			// lblTo
			//
			this.lblTo.Text = "To:";
			this.lblTo.Location = new Point(20, 175);
			this.lblTo.AutoSize = true;
			//
			// dtpTo
			//
			this.dtpTo.Location = new Point(160, 175);
			this.dtpTo.Width = 200;
			this.dtpTo.Format = DateTimePickerFormat.Short;
			//
			// lblExportPath
			//
			this.lblExportPath.Text = "Export Path:";
			this.lblExportPath.Location = new Point(20, 205);
			this.lblExportPath.AutoSize = true;
			//
			// txtExportPath
			//
			this.txtExportPath.Location = new Point(160, 205);
			this.txtExportPath.Width = 300;
			//
			// btnExportInterval
			//
			this.btnExportInterval.Text = "Export Intervals";
			this.btnExportInterval.Location = new Point(20, 240);
			this.btnExportInterval.Width = 120;
			this.btnExportInterval.Click += new EventHandler(this.BtnExportInterval_Click);
			//
			// btnExportEvent
			//
			this.btnExportEvent.Text = "Export Events";
			this.btnExportEvent.Location = new Point(150, 240);
			this.btnExportEvent.Width = 120;
			this.btnExportEvent.Click += new EventHandler(this.BtnExportEvent_Click);
			//
			// btnExportPeak
			//
			this.btnExportPeak.Text = "Export Peaks";
			this.btnExportPeak.Location = new Point(280, 240);
			this.btnExportPeak.Width = 120;
			this.btnExportPeak.Click += new EventHandler(this.BtnExportPeak_Click);
			//
			// btnRecoverBackup
			//
			this.btnRecoverBackup.Text = "Recover from Backup";
			this.btnRecoverBackup.Location = new Point(20, 275);
			this.btnRecoverBackup.Width = 160;
			this.btnRecoverBackup.Click += new EventHandler(this.BtnRecoverBackup_Click);
			//
			// tabStorage
			//
			this.tabStorage.Text = "Storage/Export";
			this.tabStorage.Controls.AddRange(new Control[] {
				this.chkDbSave, this.lblDsn, this.cboDsn, this.chkFailover,
				this.lblCsvExport,
				this.lblFrom, this.dtpFrom,
				this.lblTo, this.dtpTo,
				this.lblExportPath, this.txtExportPath,
				this.btnExportInterval, this.btnExportEvent, this.btnExportPeak,
				this.btnRecoverBackup
			});

			// =====================================================
			// TabControl
			// =====================================================
			this.tabControl.Dock = DockStyle.Fill;
			this.tabControl.TabPages.Add(this.tabGeneral);
			this.tabControl.TabPages.Add(this.tabContract);
			this.tabControl.TabPages.Add(this.tabMeter);
			this.tabControl.TabPages.Add(this.tabLoads);
			this.tabControl.TabPages.Add(this.tabPolicy);
			this.tabControl.TabPages.Add(this.tabForecast);
			this.tabControl.TabPages.Add(this.tabStorage);
			this.tabControl.SelectedIndexChanged += new EventHandler(this.tabControl_SelectedIndexChanged);

			// =====================================================
			// Bottom Button Panel
			// =====================================================
			//
			// panelBtn
			//
			this.panelBtn.Dock = DockStyle.Bottom;
			this.panelBtn.Height = 40;
			//
			// btnOK
			//
			this.btnOK.Text = "OK";
			this.btnOK.Width = 80;
			this.btnOK.Location = new Point(430, 8);
			this.btnOK.Click += new EventHandler(this.BtnOK_Click);
			//
			// btnCancel
			//
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Width = 80;
			this.btnCancel.Location = new Point(520, 8);
			this.btnCancel.DialogResult = DialogResult.Cancel;
			//
			// btnApply
			//
			this.btnApply.Text = "Apply";
			this.btnApply.Width = 80;
			this.btnApply.Location = new Point(610, 8);
			this.btnApply.Click += new EventHandler(this.BtnApply_Click);

			this.panelBtn.Controls.AddRange(new Control[] { this.btnOK, this.btnCancel, this.btnApply });

			// =====================================================
			// FormDemandNewSettings
			// =====================================================
			this.Text = "Demand Control Settings (New)";
			this.Size = new Size(720, 560);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.AcceptButton = this.btnOK;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.panelBtn);

			((System.ComponentModel.ISupportInitialize)(this.gridTimeZone)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkTrendWindow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkEwmaAlpha)).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private TabControl tabControl;
		private TabPage tabGeneral;
		private TabPage tabContract;
		private TabPage tabMeter;
		private TabPage tabLoads;
		private TabPage tabPolicy;
		private TabPage tabForecast;
		private TabPage tabStorage;
		private Panel panelBtn;
		private Button btnOK;
		private Button btnCancel;
		private Button btnApply;

		// 일반 탭
		private Label lblBlockId;
		private ComboBox cboBlockId;
		private Label lblTitle;
		private TextBox txtTitle;
		private Label lblMode;
		private RadioButton rdoShadow;
		private RadioButton rdoActive;
		private Label lblInterval;
		private ComboBox cboInterval;
		private CheckBox chkClockAligned;

		// 계약/요금 탭
		private Label lblContractKW;
		private NumericUpDown numContractKW;
		private Label lblSafetyFactor;
		private NumericUpDown numSafetyFactor;
		private Label lblTimeZoneTargets;
		private DataGridView gridTimeZone;

		// 계측 탭
		private Label lblMeterTag;
		private TextBox txtMeterTag;
		private Button btnMeterTag;
		private Label lblMeterType;
		private ComboBox cboMeterType;
		private Label lblPulseRatio;
		private NumericUpDown numPulseRatio;
		private CheckBox chkAutoReset;
		private CheckBox chkUseTargetTag;
		private TextBox txtTargetTag;
		private Button btnTargetTag;
		private CheckBox chkUseEOI;
		private TextBox txtEOITag;
		private Button btnEOITag;
		private Label lblPredictionDisplay;
		private TextBox txtPredictionTag;
		private Button btnPredictionTag;
		private Label lblCommStatusTag;
		private TextBox txtCommStatusTag;
		private Button btnCommStatusTag;

		// 부하 탭
		private ListView lvLoads;
		private Button btnAddLoad;
		private Button btnEditLoad;
		private Button btnDeleteLoad;

		// 정책 탭
		private Label lblShedMargin;
		private NumericUpDown numShedMargin;
		private Label lblRestoreMargin;
		private NumericUpDown numRestoreMargin;
		private Label lblProtectionTime;
		private NumericUpDown numProtectionTime;
		private CheckBox chkMultiStep;
		private Label lblStepCount;
		private ComboBox cboStepCount;
		private Panel pnlStepDetail;
		private GroupBox grpStep1;
		private GroupBox grpStep2;
		private GroupBox grpStep3;
		private Label lblStepReduction1;
		private NumericUpDown numStepReductionKW1;
		private Label lblStepLoads1;
		private CheckedListBox clbStepLoads1;
		private Label lblStepReduction2;
		private NumericUpDown numStepReductionKW2;
		private Label lblStepLoads2;
		private CheckedListBox clbStepLoads2;
		private Label lblStepReduction3;
		private NumericUpDown numStepReductionKW3;
		private Label lblStepLoads3;
		private CheckedListBox clbStepLoads3;

		// 예측 탭
		private Label lblTrendWindow;
		private TrackBar trkTrendWindow;
		private Label lblTrendValue;
		private Label lblEwmaAlpha;
		private TrackBar trkEwmaAlpha;
		private Label lblAlphaValue;
		private Label lblForecastDesc;

		// 저장/내보내기 탭
		private CheckBox chkDbSave;
		private Label lblDsn;
		private ComboBox cboDsn;
		private CheckBox chkFailover;
		private Label lblCsvExport;
		private Label lblFrom;
		private DateTimePicker dtpFrom;
		private Label lblTo;
		private DateTimePicker dtpTo;
		private Label lblExportPath;
		private TextBox txtExportPath;
		private Button btnExportInterval;
		private Button btnExportEvent;
		private Button btnExportPeak;
		private Button btnRecoverBackup;
	}
}
