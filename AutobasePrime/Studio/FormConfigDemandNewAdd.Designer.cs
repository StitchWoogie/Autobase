using System;
using System.Drawing;
using System.Windows.Forms;

namespace Studio
{
	partial class FormConfigDemandNewAdd
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
            this.grpGeneral = new System.Windows.Forms.Label();
            this.grpContract = new System.Windows.Forms.Label();
            this.grpMetering = new System.Windows.Forms.Label();
            this.grpPolicy = new System.Windows.Forms.Label();
            this.grpForecast = new System.Windows.Forms.Label();
            this.grpStorage = new System.Windows.Forms.Label();
            this.grpDR = new System.Windows.Forms.Label();
            this.lblBlockId = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.lblInterval = new System.Windows.Forms.Label();
            this.lblContractKW = new System.Windows.Forms.Label();
            this.lblSafetyFactor = new System.Windows.Forms.Label();
            this.lblMeterTag = new System.Windows.Forms.Label();
            this.lblMeterType = new System.Windows.Forms.Label();
            this.lblPulseRatio = new System.Windows.Forms.Label();
            this.lblCommStatusTag = new System.Windows.Forms.Label();
            this.lblShedMargin = new System.Windows.Forms.Label();
            this.lblRestoreMargin = new System.Windows.Forms.Label();
            this.lblProtectionTime = new System.Windows.Forms.Label();
            this.lblTrendWindow = new System.Windows.Forms.Label();
            this.lblEwmaAlpha = new System.Windows.Forms.Label();
            this.lblDsn = new System.Windows.Forms.Label();
            this.lblDrTargetKW = new System.Windows.Forms.Label();
            this.lblDrStart = new System.Windows.Forms.Label();
            this.lblDrEnd = new System.Windows.Forms.Label();
            this.txtBlockId = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.cmbInterval = new System.Windows.Forms.ComboBox();
            this.chkClockAligned = new System.Windows.Forms.CheckBox();
            this.numContractKW = new System.Windows.Forms.NumericUpDown();
            this.numSafetyFactor = new System.Windows.Forms.NumericUpDown();
            this.txtMeterTag = new System.Windows.Forms.TextBox();
            this.btnMeterTag = new System.Windows.Forms.Button();
            this.cmbMeterType = new System.Windows.Forms.ComboBox();
            this.numPulseRatio = new System.Windows.Forms.NumericUpDown();
            this.txtCommStatusTag = new System.Windows.Forms.TextBox();
            this.btnCommStatusTag = new System.Windows.Forms.Button();
            this.numShedMargin = new System.Windows.Forms.NumericUpDown();
            this.numRestoreMargin = new System.Windows.Forms.NumericUpDown();
            this.numProtectionTime = new System.Windows.Forms.NumericUpDown();
            this.numTrendWindow = new System.Windows.Forms.NumericUpDown();
            this.numEwmaAlpha = new System.Windows.Forms.NumericUpDown();
            this.chkDbSave = new System.Windows.Forms.CheckBox();
            this.txtDbDsn = new System.Windows.Forms.TextBox();
            this.chkFailover = new System.Windows.Forms.CheckBox();
            this.chkDrEnabled = new System.Windows.Forms.CheckBox();
            this.numDrTargetKW = new System.Windows.Forms.NumericUpDown();
            this.dtpDrStart = new System.Windows.Forms.DateTimePicker();
            this.dtpDrEnd = new System.Windows.Forms.DateTimePicker();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numContractKW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSafetyFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShedMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRestoreMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProtectionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrendWindow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEwmaAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrTargetKW)).BeginInit();
            this.SuspendLayout();
            // 
            // grpGeneral
            // 
            this.grpGeneral.AutoSize = true;
            this.grpGeneral.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpGeneral.Location = new System.Drawing.Point(12, 12);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(72, 12);
            this.grpGeneral.TabIndex = 0;
            this.grpGeneral.Text = "■ General";
            // 
            // grpContract
            // 
            this.grpContract.AutoSize = true;
            this.grpContract.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpContract.Location = new System.Drawing.Point(12, 182);
            this.grpContract.Name = "grpContract";
            this.grpContract.Size = new System.Drawing.Size(125, 12);
            this.grpContract.TabIndex = 10;
            this.grpContract.Text = "■ Contract/Target";
            // 
            // grpMetering
            // 
            this.grpMetering.AutoSize = true;
            this.grpMetering.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpMetering.Location = new System.Drawing.Point(12, 280);
            this.grpMetering.Name = "grpMetering";
            this.grpMetering.Size = new System.Drawing.Size(78, 12);
            this.grpMetering.TabIndex = 15;
            this.grpMetering.Text = "■ Metering";
            // 
            // grpPolicy
            // 
            this.grpPolicy.AutoSize = true;
            this.grpPolicy.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpPolicy.Location = new System.Drawing.Point(360, 12);
            this.grpPolicy.Name = "grpPolicy";
            this.grpPolicy.Size = new System.Drawing.Size(62, 12);
            this.grpPolicy.TabIndex = 26;
            this.grpPolicy.Text = "■ Policy";
            // 
            // grpForecast
            // 
            this.grpForecast.AutoSize = true;
            this.grpForecast.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpForecast.Location = new System.Drawing.Point(360, 123);
            this.grpForecast.Name = "grpForecast";
            this.grpForecast.Size = new System.Drawing.Size(78, 12);
            this.grpForecast.TabIndex = 33;
            this.grpForecast.Text = "■ Forecast";
            // 
            // grpStorage
            // 
            this.grpStorage.AutoSize = true;
            this.grpStorage.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpStorage.Location = new System.Drawing.Point(360, 206);
            this.grpStorage.Name = "grpStorage";
            this.grpStorage.Size = new System.Drawing.Size(71, 12);
            this.grpStorage.TabIndex = 38;
            this.grpStorage.Text = "■ Storage";
            // 
            // grpDR
            // 
            this.grpDR.AutoSize = true;
            this.grpDR.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grpDR.Location = new System.Drawing.Point(360, 286);
            this.grpDR.Name = "grpDR";
            this.grpDR.Size = new System.Drawing.Size(144, 12);
            this.grpDR.TabIndex = 43;
            this.grpDR.Text = "■ Demand Response";
            // 
            // lblBlockId
            // 
            this.lblBlockId.AutoSize = true;
            this.lblBlockId.Location = new System.Drawing.Point(12, 37);
            this.lblBlockId.Name = "lblBlockId";
            this.lblBlockId.Size = new System.Drawing.Size(55, 12);
            this.lblBlockId.TabIndex = 1;
            this.lblBlockId.Text = "Block ID:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 65);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(33, 12);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Title:";
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(12, 93);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(41, 12);
            this.lblMode.TabIndex = 5;
            this.lblMode.Text = "Mode:";
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(12, 121);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(80, 12);
            this.lblInterval.TabIndex = 7;
            this.lblInterval.Text = "Interval(min):";
            // 
            // lblContractKW
            // 
            this.lblContractKW.AutoSize = true;
            this.lblContractKW.Location = new System.Drawing.Point(12, 207);
            this.lblContractKW.Name = "lblContractKW";
            this.lblContractKW.Size = new System.Drawing.Size(82, 12);
            this.lblContractKW.TabIndex = 11;
            this.lblContractKW.Text = "Contract(kW):";
            // 
            // lblSafetyFactor
            // 
            this.lblSafetyFactor.AutoSize = true;
            this.lblSafetyFactor.Location = new System.Drawing.Point(12, 235);
            this.lblSafetyFactor.Name = "lblSafetyFactor";
            this.lblSafetyFactor.Size = new System.Drawing.Size(83, 12);
            this.lblSafetyFactor.TabIndex = 13;
            this.lblSafetyFactor.Text = "Safety Factor:";
            // 
            // lblMeterTag
            // 
            this.lblMeterTag.AutoSize = true;
            this.lblMeterTag.Location = new System.Drawing.Point(12, 305);
            this.lblMeterTag.Name = "lblMeterTag";
            this.lblMeterTag.Size = new System.Drawing.Size(67, 12);
            this.lblMeterTag.TabIndex = 16;
            this.lblMeterTag.Text = "Meter Tag:";
            // 
            // lblMeterType
            // 
            this.lblMeterType.AutoSize = true;
            this.lblMeterType.Location = new System.Drawing.Point(12, 333);
            this.lblMeterType.Name = "lblMeterType";
            this.lblMeterType.Size = new System.Drawing.Size(74, 12);
            this.lblMeterType.TabIndex = 19;
            this.lblMeterType.Text = "Meter Type:";
            // 
            // lblPulseRatio
            // 
            this.lblPulseRatio.AutoSize = true;
            this.lblPulseRatio.Location = new System.Drawing.Point(12, 361);
            this.lblPulseRatio.Name = "lblPulseRatio";
            this.lblPulseRatio.Size = new System.Drawing.Size(73, 12);
            this.lblPulseRatio.TabIndex = 21;
            this.lblPulseRatio.Text = "Pulse Ratio:";
            // 
            // lblCommStatusTag
            // 
            this.lblCommStatusTag.AutoSize = true;
            this.lblCommStatusTag.Location = new System.Drawing.Point(12, 389);
            this.lblCommStatusTag.Name = "lblCommStatusTag";
            this.lblCommStatusTag.Size = new System.Drawing.Size(112, 12);
            this.lblCommStatusTag.TabIndex = 23;
            this.lblCommStatusTag.Text = "Comm Status Tag:";
            // 
            // lblShedMargin
            // 
            this.lblShedMargin.AutoSize = true;
            this.lblShedMargin.Location = new System.Drawing.Point(360, 37);
            this.lblShedMargin.Name = "lblShedMargin";
            this.lblShedMargin.Size = new System.Drawing.Size(107, 12);
            this.lblShedMargin.TabIndex = 27;
            this.lblShedMargin.Text = "Shed Margin(kW):";
            // 
            // lblRestoreMargin
            // 
            this.lblRestoreMargin.AutoSize = true;
            this.lblRestoreMargin.Location = new System.Drawing.Point(360, 65);
            this.lblRestoreMargin.Name = "lblRestoreMargin";
            this.lblRestoreMargin.Size = new System.Drawing.Size(121, 12);
            this.lblRestoreMargin.TabIndex = 29;
            this.lblRestoreMargin.Text = "Restore Margin(kW):";
            // 
            // lblProtectionTime
            // 
            this.lblProtectionTime.AutoSize = true;
            this.lblProtectionTime.Location = new System.Drawing.Point(360, 93);
            this.lblProtectionTime.Name = "lblProtectionTime";
            this.lblProtectionTime.Size = new System.Drawing.Size(98, 12);
            this.lblProtectionTime.TabIndex = 31;
            this.lblProtectionTime.Text = "Protect Time(s):";
            // 
            // lblTrendWindow
            // 
            this.lblTrendWindow.AutoSize = true;
            this.lblTrendWindow.Location = new System.Drawing.Point(360, 148);
            this.lblTrendWindow.Name = "lblTrendWindow";
            this.lblTrendWindow.Size = new System.Drawing.Size(107, 12);
            this.lblTrendWindow.TabIndex = 34;
            this.lblTrendWindow.Text = "Trend Window(s):";
            // 
            // lblEwmaAlpha
            // 
            this.lblEwmaAlpha.AutoSize = true;
            this.lblEwmaAlpha.Location = new System.Drawing.Point(360, 176);
            this.lblEwmaAlpha.Name = "lblEwmaAlpha";
            this.lblEwmaAlpha.Size = new System.Drawing.Size(82, 12);
            this.lblEwmaAlpha.TabIndex = 36;
            this.lblEwmaAlpha.Text = "EWMA Alpha:";
            // 
            // lblDsn
            // 
            this.lblDsn.AutoSize = true;
            this.lblDsn.Location = new System.Drawing.Point(360, 233);
            this.lblDsn.Name = "lblDsn";
            this.lblDsn.Size = new System.Drawing.Size(34, 12);
            this.lblDsn.TabIndex = 40;
            this.lblDsn.Text = "DSN:";
            // 
            // lblDrTargetKW
            // 
            this.lblDrTargetKW.AutoSize = true;
            this.lblDrTargetKW.Location = new System.Drawing.Point(360, 335);
            this.lblDrTargetKW.Name = "lblDrTargetKW";
            this.lblDrTargetKW.Size = new System.Drawing.Size(91, 12);
            this.lblDrTargetKW.TabIndex = 45;
            this.lblDrTargetKW.Text = "DR Target(kW):";
            // 
            // lblDrStart
            // 
            this.lblDrStart.AutoSize = true;
            this.lblDrStart.Location = new System.Drawing.Point(360, 363);
            this.lblDrStart.Name = "lblDrStart";
            this.lblDrStart.Size = new System.Drawing.Size(54, 12);
            this.lblDrStart.TabIndex = 47;
            this.lblDrStart.Text = "DR Start:";
            // 
            // lblDrEnd
            // 
            this.lblDrEnd.AutoSize = true;
            this.lblDrEnd.Location = new System.Drawing.Point(360, 391);
            this.lblDrEnd.Name = "lblDrEnd";
            this.lblDrEnd.Size = new System.Drawing.Size(51, 12);
            this.lblDrEnd.TabIndex = 49;
            this.lblDrEnd.Text = "DR End:";
            // 
            // txtBlockId
            // 
            this.txtBlockId.Location = new System.Drawing.Point(140, 34);
            this.txtBlockId.Name = "txtBlockId";
            this.txtBlockId.Size = new System.Drawing.Size(200, 21);
            this.txtBlockId.TabIndex = 2;
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(140, 62);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(200, 21);
            this.txtTitle.TabIndex = 4;
            // 
            // cmbMode
            // 
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.Items.AddRange(new object[] {
            "Shadow",
            "Active"});
            this.cmbMode.Location = new System.Drawing.Point(140, 90);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(200, 20);
            this.cmbMode.TabIndex = 6;
            // 
            // cmbInterval
            // 
            this.cmbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterval.Items.AddRange(new object[] {
            "5",
            "10",
            "15",
            "30",
            "60"});
            this.cmbInterval.Location = new System.Drawing.Point(140, 118);
            this.cmbInterval.Name = "cmbInterval";
            this.cmbInterval.Size = new System.Drawing.Size(200, 20);
            this.cmbInterval.TabIndex = 8;
            // 
            // chkClockAligned
            // 
            this.chkClockAligned.AutoSize = true;
            this.chkClockAligned.Location = new System.Drawing.Point(140, 146);
            this.chkClockAligned.Name = "chkClockAligned";
            this.chkClockAligned.Size = new System.Drawing.Size(102, 16);
            this.chkClockAligned.TabIndex = 9;
            this.chkClockAligned.Text = "Clock Aligned";
            // 
            // numContractKW
            // 
            this.numContractKW.Location = new System.Drawing.Point(140, 204);
            this.numContractKW.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numContractKW.Name = "numContractKW";
            this.numContractKW.Size = new System.Drawing.Size(200, 21);
            this.numContractKW.TabIndex = 12;
            // 
            // numSafetyFactor
            // 
            this.numSafetyFactor.DecimalPlaces = 2;
            this.numSafetyFactor.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numSafetyFactor.Location = new System.Drawing.Point(140, 232);
            this.numSafetyFactor.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            131072});
            this.numSafetyFactor.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            this.numSafetyFactor.Name = "numSafetyFactor";
            this.numSafetyFactor.Size = new System.Drawing.Size(200, 21);
            this.numSafetyFactor.TabIndex = 14;
            this.numSafetyFactor.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // txtMeterTag
            // 
            this.txtMeterTag.Location = new System.Drawing.Point(140, 302);
            this.txtMeterTag.Name = "txtMeterTag";
            this.txtMeterTag.Size = new System.Drawing.Size(170, 21);
            this.txtMeterTag.TabIndex = 17;
            // 
            // btnMeterTag
            // 
            this.btnMeterTag.Location = new System.Drawing.Point(314, 302);
            this.btnMeterTag.Name = "btnMeterTag";
            this.btnMeterTag.Size = new System.Drawing.Size(26, 22);
            this.btnMeterTag.TabIndex = 18;
            this.btnMeterTag.Text = "...";
            this.btnMeterTag.Click += new System.EventHandler(this.btnMeterTag_Click);
            // 
            // cmbMeterType
            // 
            this.cmbMeterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMeterType.Items.AddRange(new object[] {
            "DirectKW",
            "DeltaKWH",
            "Pulse"});
            this.cmbMeterType.Location = new System.Drawing.Point(140, 330);
            this.cmbMeterType.Name = "cmbMeterType";
            this.cmbMeterType.Size = new System.Drawing.Size(200, 20);
            this.cmbMeterType.TabIndex = 20;
            // 
            // numPulseRatio
            // 
            this.numPulseRatio.DecimalPlaces = 3;
            this.numPulseRatio.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numPulseRatio.Location = new System.Drawing.Point(140, 358);
            this.numPulseRatio.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPulseRatio.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numPulseRatio.Name = "numPulseRatio";
            this.numPulseRatio.Size = new System.Drawing.Size(200, 21);
            this.numPulseRatio.TabIndex = 22;
            this.numPulseRatio.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // txtCommStatusTag
            // 
            this.txtCommStatusTag.Location = new System.Drawing.Point(140, 386);
            this.txtCommStatusTag.Name = "txtCommStatusTag";
            this.txtCommStatusTag.Size = new System.Drawing.Size(170, 21);
            this.txtCommStatusTag.TabIndex = 24;
            // 
            // btnCommStatusTag
            // 
            this.btnCommStatusTag.Location = new System.Drawing.Point(314, 386);
            this.btnCommStatusTag.Name = "btnCommStatusTag";
            this.btnCommStatusTag.Size = new System.Drawing.Size(26, 22);
            this.btnCommStatusTag.TabIndex = 25;
            this.btnCommStatusTag.Text = "...";
            this.btnCommStatusTag.Click += new System.EventHandler(this.btnCommStatusTag_Click);
            // 
            // numShedMargin
            // 
            this.numShedMargin.DecimalPlaces = 1;
            this.numShedMargin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numShedMargin.Location = new System.Drawing.Point(488, 34);
            this.numShedMargin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numShedMargin.Name = "numShedMargin";
            this.numShedMargin.Size = new System.Drawing.Size(200, 21);
            this.numShedMargin.TabIndex = 28;
            // 
            // numRestoreMargin
            // 
            this.numRestoreMargin.DecimalPlaces = 1;
            this.numRestoreMargin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numRestoreMargin.Location = new System.Drawing.Point(488, 62);
            this.numRestoreMargin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRestoreMargin.Name = "numRestoreMargin";
            this.numRestoreMargin.Size = new System.Drawing.Size(200, 21);
            this.numRestoreMargin.TabIndex = 30;
            // 
            // numProtectionTime
            // 
            this.numProtectionTime.Location = new System.Drawing.Point(488, 90);
            this.numProtectionTime.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numProtectionTime.Name = "numProtectionTime";
            this.numProtectionTime.Size = new System.Drawing.Size(200, 21);
            this.numProtectionTime.TabIndex = 32;
            // 
            // numTrendWindow
            // 
            this.numTrendWindow.Location = new System.Drawing.Point(488, 145);
            this.numTrendWindow.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numTrendWindow.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numTrendWindow.Name = "numTrendWindow";
            this.numTrendWindow.Size = new System.Drawing.Size(200, 21);
            this.numTrendWindow.TabIndex = 35;
            this.numTrendWindow.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // numEwmaAlpha
            // 
            this.numEwmaAlpha.DecimalPlaces = 2;
            this.numEwmaAlpha.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numEwmaAlpha.Location = new System.Drawing.Point(488, 173);
            this.numEwmaAlpha.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            131072});
            this.numEwmaAlpha.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numEwmaAlpha.Name = "numEwmaAlpha";
            this.numEwmaAlpha.Size = new System.Drawing.Size(200, 21);
            this.numEwmaAlpha.TabIndex = 37;
            this.numEwmaAlpha.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // chkDbSave
            // 
            this.chkDbSave.AutoSize = true;
            this.chkDbSave.Location = new System.Drawing.Point(488, 206);
            this.chkDbSave.Name = "chkDbSave";
            this.chkDbSave.Size = new System.Drawing.Size(122, 16);
            this.chkDbSave.TabIndex = 39;
            this.chkDbSave.Text = "DB Save Enabled";
            // 
            // txtDbDsn
            // 
            this.txtDbDsn.Location = new System.Drawing.Point(488, 230);
            this.txtDbDsn.Name = "txtDbDsn";
            this.txtDbDsn.Size = new System.Drawing.Size(200, 21);
            this.txtDbDsn.TabIndex = 41;
            // 
            // chkFailover
            // 
            this.chkFailover.AutoSize = true;
            this.chkFailover.Location = new System.Drawing.Point(488, 258);
            this.chkFailover.Name = "chkFailover";
            this.chkFailover.Size = new System.Drawing.Size(118, 16);
            this.chkFailover.TabIndex = 42;
            this.chkFailover.Text = "Failover Enabled";
            // 
            // chkDrEnabled
            // 
            this.chkDrEnabled.AutoSize = true;
            this.chkDrEnabled.Location = new System.Drawing.Point(488, 308);
            this.chkDrEnabled.Name = "chkDrEnabled";
            this.chkDrEnabled.Size = new System.Drawing.Size(90, 16);
            this.chkDrEnabled.TabIndex = 44;
            this.chkDrEnabled.Text = "DR Enabled";
            // 
            // numDrTargetKW
            // 
            this.numDrTargetKW.Location = new System.Drawing.Point(488, 332);
            this.numDrTargetKW.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numDrTargetKW.Name = "numDrTargetKW";
            this.numDrTargetKW.Size = new System.Drawing.Size(200, 21);
            this.numDrTargetKW.TabIndex = 46;
            // 
            // dtpDrStart
            // 
            this.dtpDrStart.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpDrStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDrStart.Location = new System.Drawing.Point(488, 360);
            this.dtpDrStart.Name = "dtpDrStart";
            this.dtpDrStart.Size = new System.Drawing.Size(200, 21);
            this.dtpDrStart.TabIndex = 48;
            // 
            // dtpDrEnd
            // 
            this.dtpDrEnd.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpDrEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDrEnd.Location = new System.Drawing.Point(488, 388);
            this.dtpDrEnd.Name = "dtpDrEnd";
            this.dtpDrEnd.Size = new System.Drawing.Size(200, 21);
            this.dtpDrEnd.TabIndex = 50;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(704, 12);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(68, 28);
            this.buttonOK.TabIndex = 51;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(704, 53);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(68, 28);
            this.buttonCancel.TabIndex = 52;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormConfigDemandNewAdd
            // 
            this.ClientSize = new System.Drawing.Size(784, 429);
            this.Controls.Add(this.grpGeneral);
            this.Controls.Add(this.lblBlockId);
            this.Controls.Add(this.txtBlockId);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.cmbMode);
            this.Controls.Add(this.lblInterval);
            this.Controls.Add(this.cmbInterval);
            this.Controls.Add(this.chkClockAligned);
            this.Controls.Add(this.grpContract);
            this.Controls.Add(this.lblContractKW);
            this.Controls.Add(this.numContractKW);
            this.Controls.Add(this.lblSafetyFactor);
            this.Controls.Add(this.numSafetyFactor);
            this.Controls.Add(this.grpMetering);
            this.Controls.Add(this.lblMeterTag);
            this.Controls.Add(this.txtMeterTag);
            this.Controls.Add(this.btnMeterTag);
            this.Controls.Add(this.lblMeterType);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.lblPulseRatio);
            this.Controls.Add(this.numPulseRatio);
            this.Controls.Add(this.lblCommStatusTag);
            this.Controls.Add(this.txtCommStatusTag);
            this.Controls.Add(this.btnCommStatusTag);
            this.Controls.Add(this.grpPolicy);
            this.Controls.Add(this.lblShedMargin);
            this.Controls.Add(this.numShedMargin);
            this.Controls.Add(this.lblRestoreMargin);
            this.Controls.Add(this.numRestoreMargin);
            this.Controls.Add(this.lblProtectionTime);
            this.Controls.Add(this.numProtectionTime);
            this.Controls.Add(this.grpForecast);
            this.Controls.Add(this.lblTrendWindow);
            this.Controls.Add(this.numTrendWindow);
            this.Controls.Add(this.lblEwmaAlpha);
            this.Controls.Add(this.numEwmaAlpha);
            this.Controls.Add(this.grpStorage);
            this.Controls.Add(this.chkDbSave);
            this.Controls.Add(this.lblDsn);
            this.Controls.Add(this.txtDbDsn);
            this.Controls.Add(this.chkFailover);
            this.Controls.Add(this.grpDR);
            this.Controls.Add(this.chkDrEnabled);
            this.Controls.Add(this.lblDrTargetKW);
            this.Controls.Add(this.numDrTargetKW);
            this.Controls.Add(this.lblDrStart);
            this.Controls.Add(this.dtpDrStart);
            this.Controls.Add(this.lblDrEnd);
            this.Controls.Add(this.dtpDrEnd);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigDemandNewAdd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Demand Block Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numContractKW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSafetyFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPulseRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShedMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRestoreMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProtectionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrendWindow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEwmaAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrTargetKW)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		// 그룹 라벨
		private Label grpGeneral;
		private Label grpContract;
		private Label grpMetering;
		private Label grpPolicy;
		private Label grpForecast;
		private Label grpStorage;
		private Label grpDR;

		// 필드 라벨
		private Label lblBlockId;
		private Label lblTitle;
		private Label lblMode;
		private Label lblInterval;
		private Label lblContractKW;
		private Label lblSafetyFactor;
		private Label lblMeterTag;
		private Label lblMeterType;
		private Label lblPulseRatio;
		private Label lblCommStatusTag;
		private Label lblShedMargin;
		private Label lblRestoreMargin;
		private Label lblProtectionTime;
		private Label lblTrendWindow;
		private Label lblEwmaAlpha;
		private Label lblDsn;
		private Label lblDrTargetKW;
		private Label lblDrStart;
		private Label lblDrEnd;

		// 일반
		private TextBox txtBlockId;
		private TextBox txtTitle;
		private ComboBox cmbMode;
		private ComboBox cmbInterval;
		private CheckBox chkClockAligned;

		// 계약
		private NumericUpDown numContractKW;
		private NumericUpDown numSafetyFactor;

		// 계측
		private TextBox txtMeterTag;
		private Button btnMeterTag;
		private ComboBox cmbMeterType;
		private NumericUpDown numPulseRatio;
		private TextBox txtCommStatusTag;
		private Button btnCommStatusTag;

		// 정책
		private NumericUpDown numShedMargin;
		private NumericUpDown numRestoreMargin;
		private NumericUpDown numProtectionTime;

		// 예측
		private NumericUpDown numTrendWindow;
		private NumericUpDown numEwmaAlpha;

		// 저장
		private CheckBox chkDbSave;
		private TextBox txtDbDsn;
		private CheckBox chkFailover;

		// DR
		private CheckBox chkDrEnabled;
		private NumericUpDown numDrTargetKW;
		private DateTimePicker dtpDrStart;
		private DateTimePicker dtpDrEnd;

		// 버튼
		private Button buttonOK;
		private Button buttonCancel;
	}
}
