using System;
using System.Drawing;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLibLocal.DemandNew;

namespace Studio
{
	public class PropertyPageObjectDemandChart : Form
	{
		private TabControl tabControl;
		private TabPage tabGeneral;
		private TabPage tabStyle;
		private TabPage tabTrend;
		private TabPage tabColors;

		// General
		private Label lblBlockId;
		private ComboBox comboBlockId;
		private Button btnConfig;
		private Label lblPercentY;
		private NumericUpDown numPercentY;
		private Label lblTargetThick;
		private NumericUpDown numTargetThick;
		private Label lblStatusPos;
		private ComboBox comboStatusPos;

		// Style
		private Label lblTheme;
		private ComboBox comboTheme;
		private Label lblKpiDensity;
		private ComboBox comboKpiDensity;
		private Label lblCornerRadius;
		private NumericUpDown numCornerRadius;
		private Label lblGridIntensity;
		private NumericUpDown numGridIntensity;
		private Label lblKpiOpacity;
		private NumericUpDown numKpiOpacity;
		private CheckBox chkGridLabels;

		// Trend
		private Label lblTrendThick;
		private NumericUpDown numTrendThick;
		private Label lblForecastThick;
		private NumericUpDown numForecastThick;
		private CheckBox chkAreaFill;
		private CheckBox chkForecastBand;
		private CheckBox chkPeakLine;
		private CheckBox chkStepLines;
		private CheckBox chkTimeZoneTarget;
		private CheckBox chkDetailedAxis;

		// Colors
		private Label lblSectionTrend;
		private Label lblClrPrediction;
		private Button btnPrediction;
		private Label lblClrTarget;
		private Button btnTarget;
		private Label lblClrExcess;
		private Button btnExcess;
		private Label lblClrAreaFill;
		private Button btnAreaFillColor;
		private Label lblClrForecastBand;
		private Button btnForecastBandColor;
		private Label lblClrPeakLine;
		private Button btnPeakLineColor;
		private Label lblClrStatusBack;
		private Button btnStatusBack;
		private Label lblClrStatusFill;
		private Button btnStatusFill;
		private Label lblClrStatusValue;
		private Button btnStatusValue;
		private Label lblClrStatusTitle;
		private Button btnStatusTitle;
		private Label lblClrTargetValue;
		private Button btnTargetValue;
		private Label lblClrPreValue;
		private Button btnPreValue;
		private Label lblClrExValue;
		private Button btnExValue;
		private Label lblClrCardBack;
		private Button btnCardBack;
		private Label lblClrCardBorder;
		private Button btnCardBorder;
		private bool _isBindingObjectArgs;
		private bool _isRevertingTheme;
		private int _lastThemeIndex;

		public PropertyPageObjectDemandChart()
		{
			InitializeComponent();
			_lastThemeIndex = Math.Max(0, comboTheme.SelectedIndex);
			//ApplyModernStyle();
		}

		private void InitializeComponent()
		{
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.lblBlockId = new System.Windows.Forms.Label();
            this.comboBlockId = new System.Windows.Forms.ComboBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.lblPercentY = new System.Windows.Forms.Label();
            this.numPercentY = new System.Windows.Forms.NumericUpDown();
            this.lblTargetThick = new System.Windows.Forms.Label();
            this.numTargetThick = new System.Windows.Forms.NumericUpDown();
            this.lblStatusPos = new System.Windows.Forms.Label();
            this.comboStatusPos = new System.Windows.Forms.ComboBox();
            this.tabStyle = new System.Windows.Forms.TabPage();
            this.lblTheme = new System.Windows.Forms.Label();
            this.comboTheme = new System.Windows.Forms.ComboBox();
            this.lblKpiDensity = new System.Windows.Forms.Label();
            this.comboKpiDensity = new System.Windows.Forms.ComboBox();
            this.lblCornerRadius = new System.Windows.Forms.Label();
            this.numCornerRadius = new System.Windows.Forms.NumericUpDown();
            this.lblGridIntensity = new System.Windows.Forms.Label();
            this.numGridIntensity = new System.Windows.Forms.NumericUpDown();
            this.lblKpiOpacity = new System.Windows.Forms.Label();
            this.numKpiOpacity = new System.Windows.Forms.NumericUpDown();
            this.chkGridLabels = new System.Windows.Forms.CheckBox();
            this.tabTrend = new System.Windows.Forms.TabPage();
            this.lblTrendThick = new System.Windows.Forms.Label();
            this.numTrendThick = new System.Windows.Forms.NumericUpDown();
            this.lblForecastThick = new System.Windows.Forms.Label();
            this.numForecastThick = new System.Windows.Forms.NumericUpDown();
            this.chkAreaFill = new System.Windows.Forms.CheckBox();
            this.chkForecastBand = new System.Windows.Forms.CheckBox();
            this.chkPeakLine = new System.Windows.Forms.CheckBox();
            this.chkStepLines = new System.Windows.Forms.CheckBox();
            this.chkTimeZoneTarget = new System.Windows.Forms.CheckBox();
            this.chkDetailedAxis = new System.Windows.Forms.CheckBox();
            this.tabColors = new System.Windows.Forms.TabPage();
            this.lblSectionTrend = new System.Windows.Forms.Label();
            this.lblClrPrediction = new System.Windows.Forms.Label();
            this.btnPrediction = new System.Windows.Forms.Button();
            this.lblClrTarget = new System.Windows.Forms.Label();
            this.btnTarget = new System.Windows.Forms.Button();
            this.lblClrExcess = new System.Windows.Forms.Label();
            this.btnExcess = new System.Windows.Forms.Button();
            this.lblClrAreaFill = new System.Windows.Forms.Label();
            this.btnAreaFillColor = new System.Windows.Forms.Button();
            this.lblClrForecastBand = new System.Windows.Forms.Label();
            this.btnForecastBandColor = new System.Windows.Forms.Button();
            this.lblClrPeakLine = new System.Windows.Forms.Label();
            this.btnPeakLineColor = new System.Windows.Forms.Button();
            this.lblClrStatusBack = new System.Windows.Forms.Label();
            this.btnStatusBack = new System.Windows.Forms.Button();
            this.lblClrStatusFill = new System.Windows.Forms.Label();
            this.btnStatusFill = new System.Windows.Forms.Button();
            this.lblClrStatusValue = new System.Windows.Forms.Label();
            this.btnStatusValue = new System.Windows.Forms.Button();
            this.lblClrStatusTitle = new System.Windows.Forms.Label();
            this.btnStatusTitle = new System.Windows.Forms.Button();
            this.lblClrTargetValue = new System.Windows.Forms.Label();
            this.btnTargetValue = new System.Windows.Forms.Button();
            this.lblClrPreValue = new System.Windows.Forms.Label();
            this.btnPreValue = new System.Windows.Forms.Button();
            this.lblClrExValue = new System.Windows.Forms.Label();
            this.btnExValue = new System.Windows.Forms.Button();
            this.lblClrCardBack = new System.Windows.Forms.Label();
            this.btnCardBack = new System.Windows.Forms.Button();
            this.lblClrCardBorder = new System.Windows.Forms.Label();
            this.btnCardBorder = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPercentY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetThick)).BeginInit();
            this.tabStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCornerRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGridIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKpiOpacity)).BeginInit();
            this.tabTrend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTrendThick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numForecastThick)).BeginInit();
            this.tabColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabStyle);
            this.tabControl.Controls.Add(this.tabTrend);
            this.tabControl.Controls.Add(this.tabColors);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(550, 450);
            this.tabControl.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.lblBlockId);
            this.tabGeneral.Controls.Add(this.comboBlockId);
            this.tabGeneral.Controls.Add(this.btnConfig);
            this.tabGeneral.Controls.Add(this.lblPercentY);
            this.tabGeneral.Controls.Add(this.numPercentY);
            this.tabGeneral.Controls.Add(this.lblTargetThick);
            this.tabGeneral.Controls.Add(this.numTargetThick);
            this.tabGeneral.Controls.Add(this.lblStatusPos);
            this.tabGeneral.Controls.Add(this.comboStatusPos);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(542, 424);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            // 
            // lblBlockId
            // 
            this.lblBlockId.Location = new System.Drawing.Point(16, 16);
            this.lblBlockId.Name = "lblBlockId";
            this.lblBlockId.Size = new System.Drawing.Size(120, 18);
            this.lblBlockId.TabIndex = 0;
            this.lblBlockId.Text = "Block ID";
            // 
            // comboBlockId
            // 
            this.comboBlockId.Location = new System.Drawing.Point(140, 14);
            this.comboBlockId.Name = "comboBlockId";
            this.comboBlockId.Size = new System.Drawing.Size(140, 20);
            this.comboBlockId.TabIndex = 1;
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(306, 10);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(80, 26);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "Config...";
            this.btnConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // lblPercentY
            // 
            this.lblPercentY.Location = new System.Drawing.Point(16, 50);
            this.lblPercentY.Name = "lblPercentY";
            this.lblPercentY.Size = new System.Drawing.Size(120, 18);
            this.lblPercentY.TabIndex = 3;
            this.lblPercentY.Text = "Y-Axis %";
            // 
            // numPercentY
            // 
            this.numPercentY.Location = new System.Drawing.Point(140, 48);
            this.numPercentY.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numPercentY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numPercentY.Name = "numPercentY";
            this.numPercentY.Size = new System.Drawing.Size(72, 21);
            this.numPercentY.TabIndex = 4;
            this.numPercentY.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // lblTargetThick
            // 
            this.lblTargetThick.Location = new System.Drawing.Point(16, 80);
            this.lblTargetThick.Name = "lblTargetThick";
            this.lblTargetThick.Size = new System.Drawing.Size(120, 18);
            this.lblTargetThick.TabIndex = 5;
            this.lblTargetThick.Text = "Target Thick";
            // 
            // numTargetThick
            // 
            this.numTargetThick.Location = new System.Drawing.Point(140, 78);
            this.numTargetThick.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numTargetThick.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTargetThick.Name = "numTargetThick";
            this.numTargetThick.Size = new System.Drawing.Size(72, 21);
            this.numTargetThick.TabIndex = 6;
            this.numTargetThick.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblStatusPos
            // 
            this.lblStatusPos.Location = new System.Drawing.Point(16, 110);
            this.lblStatusPos.Name = "lblStatusPos";
            this.lblStatusPos.Size = new System.Drawing.Size(120, 18);
            this.lblStatusPos.TabIndex = 7;
            this.lblStatusPos.Text = "Status Position";
            // 
            // comboStatusPos
            // 
            this.comboStatusPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStatusPos.Items.AddRange(new object[] {
            "Top",
            "Left",
            "Hidden"});
            this.comboStatusPos.Location = new System.Drawing.Point(140, 108);
            this.comboStatusPos.Name = "comboStatusPos";
            this.comboStatusPos.Size = new System.Drawing.Size(140, 20);
            this.comboStatusPos.TabIndex = 8;
            // 
            // tabStyle
            // 
            this.tabStyle.Controls.Add(this.lblTheme);
            this.tabStyle.Controls.Add(this.comboTheme);
            this.tabStyle.Controls.Add(this.lblKpiDensity);
            this.tabStyle.Controls.Add(this.comboKpiDensity);
            this.tabStyle.Controls.Add(this.lblCornerRadius);
            this.tabStyle.Controls.Add(this.numCornerRadius);
            this.tabStyle.Controls.Add(this.lblGridIntensity);
            this.tabStyle.Controls.Add(this.numGridIntensity);
            this.tabStyle.Controls.Add(this.lblKpiOpacity);
            this.tabStyle.Controls.Add(this.numKpiOpacity);
            this.tabStyle.Controls.Add(this.chkGridLabels);
            this.tabStyle.Location = new System.Drawing.Point(4, 22);
            this.tabStyle.Name = "tabStyle";
            this.tabStyle.Padding = new System.Windows.Forms.Padding(3);
            this.tabStyle.Size = new System.Drawing.Size(542, 424);
            this.tabStyle.TabIndex = 1;
            this.tabStyle.Text = "Style";
            // 
            // lblTheme
            // 
            this.lblTheme.Location = new System.Drawing.Point(16, 16);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(120, 18);
            this.lblTheme.TabIndex = 0;
            this.lblTheme.Text = "Theme";
            // 
            // comboTheme
            // 
            this.comboTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTheme.Items.AddRange(new object[] {
            "Light Minimal",
            "Dark Ops",
            "High Contrast"});
            this.comboTheme.Location = new System.Drawing.Point(140, 14);
            this.comboTheme.Name = "comboTheme";
            this.comboTheme.Size = new System.Drawing.Size(150, 20);
            this.comboTheme.TabIndex = 1;
            this.comboTheme.SelectedIndexChanged += new System.EventHandler(this.comboTheme_SelectedIndexChanged);
            // 
            // lblKpiDensity
            // 
            this.lblKpiDensity.Location = new System.Drawing.Point(16, 130);
            this.lblKpiDensity.Name = "lblKpiDensity";
            this.lblKpiDensity.Size = new System.Drawing.Size(120, 18);
            this.lblKpiDensity.TabIndex = 2;
            this.lblKpiDensity.Text = "KPI Density";
            // 
            // comboKpiDensity
            // 
            this.comboKpiDensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboKpiDensity.Items.AddRange(new object[] {
            "Compact",
            "Detailed"});
            this.comboKpiDensity.Location = new System.Drawing.Point(136, 128);
            this.comboKpiDensity.Name = "comboKpiDensity";
            this.comboKpiDensity.Size = new System.Drawing.Size(120, 20);
            this.comboKpiDensity.TabIndex = 3;
            // 
            // lblCornerRadius
            // 
            this.lblCornerRadius.Location = new System.Drawing.Point(16, 46);
            this.lblCornerRadius.Name = "lblCornerRadius";
            this.lblCornerRadius.Size = new System.Drawing.Size(120, 18);
            this.lblCornerRadius.TabIndex = 4;
            this.lblCornerRadius.Text = "Corner Radius";
            // 
            // numCornerRadius
            // 
            this.numCornerRadius.Location = new System.Drawing.Point(140, 44);
            this.numCornerRadius.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numCornerRadius.Name = "numCornerRadius";
            this.numCornerRadius.Size = new System.Drawing.Size(72, 21);
            this.numCornerRadius.TabIndex = 5;
            this.numCornerRadius.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // lblGridIntensity
            // 
            this.lblGridIntensity.Location = new System.Drawing.Point(16, 160);
            this.lblGridIntensity.Name = "lblGridIntensity";
            this.lblGridIntensity.Size = new System.Drawing.Size(120, 18);
            this.lblGridIntensity.TabIndex = 6;
            this.lblGridIntensity.Text = "Grid Intensity";
            // 
            // numGridIntensity
            // 
            this.numGridIntensity.Location = new System.Drawing.Point(136, 158);
            this.numGridIntensity.Name = "numGridIntensity";
            this.numGridIntensity.Size = new System.Drawing.Size(72, 21);
            this.numGridIntensity.TabIndex = 7;
            this.numGridIntensity.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            // 
            // lblKpiOpacity
            // 
            this.lblKpiOpacity.Location = new System.Drawing.Point(16, 76);
            this.lblKpiOpacity.Name = "lblKpiOpacity";
            this.lblKpiOpacity.Size = new System.Drawing.Size(120, 18);
            this.lblKpiOpacity.TabIndex = 8;
            this.lblKpiOpacity.Text = "KPI Opacity";
            // 
            // numKpiOpacity
            // 
            this.numKpiOpacity.Location = new System.Drawing.Point(140, 74);
            this.numKpiOpacity.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numKpiOpacity.Name = "numKpiOpacity";
            this.numKpiOpacity.Size = new System.Drawing.Size(72, 21);
            this.numKpiOpacity.TabIndex = 9;
            this.numKpiOpacity.Value = new decimal(new int[] {
            82,
            0,
            0,
            0});
            // 
            // chkGridLabels
            // 
            this.chkGridLabels.Location = new System.Drawing.Point(16, 190);
            this.chkGridLabels.Name = "chkGridLabels";
            this.chkGridLabels.Size = new System.Drawing.Size(140, 20);
            this.chkGridLabels.TabIndex = 10;
            this.chkGridLabels.Text = "Grid Labels";
            // 
            // tabTrend
            // 
            this.tabTrend.Controls.Add(this.lblTrendThick);
            this.tabTrend.Controls.Add(this.numTrendThick);
            this.tabTrend.Controls.Add(this.lblForecastThick);
            this.tabTrend.Controls.Add(this.numForecastThick);
            this.tabTrend.Controls.Add(this.chkAreaFill);
            this.tabTrend.Controls.Add(this.chkForecastBand);
            this.tabTrend.Controls.Add(this.chkPeakLine);
            this.tabTrend.Controls.Add(this.chkStepLines);
            this.tabTrend.Controls.Add(this.chkTimeZoneTarget);
            this.tabTrend.Controls.Add(this.chkDetailedAxis);
            this.tabTrend.Location = new System.Drawing.Point(4, 22);
            this.tabTrend.Name = "tabTrend";
            this.tabTrend.Padding = new System.Windows.Forms.Padding(3);
            this.tabTrend.Size = new System.Drawing.Size(542, 424);
            this.tabTrend.TabIndex = 2;
            this.tabTrend.Text = "Trend";
            // 
            // lblTrendThick
            // 
            this.lblTrendThick.Location = new System.Drawing.Point(16, 16);
            this.lblTrendThick.Name = "lblTrendThick";
            this.lblTrendThick.Size = new System.Drawing.Size(120, 18);
            this.lblTrendThick.TabIndex = 0;
            this.lblTrendThick.Text = "Trend Thick";
            // 
            // numTrendThick
            // 
            this.numTrendThick.Location = new System.Drawing.Point(140, 14);
            this.numTrendThick.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numTrendThick.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTrendThick.Name = "numTrendThick";
            this.numTrendThick.Size = new System.Drawing.Size(72, 21);
            this.numTrendThick.TabIndex = 1;
            this.numTrendThick.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblForecastThick
            // 
            this.lblForecastThick.Location = new System.Drawing.Point(260, 16);
            this.lblForecastThick.Name = "lblForecastThick";
            this.lblForecastThick.Size = new System.Drawing.Size(120, 18);
            this.lblForecastThick.TabIndex = 2;
            this.lblForecastThick.Text = "Forecast Thick";
            // 
            // numForecastThick
            // 
            this.numForecastThick.Location = new System.Drawing.Point(380, 14);
            this.numForecastThick.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numForecastThick.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numForecastThick.Name = "numForecastThick";
            this.numForecastThick.Size = new System.Drawing.Size(72, 21);
            this.numForecastThick.TabIndex = 3;
            this.numForecastThick.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkAreaFill
            // 
            this.chkAreaFill.Location = new System.Drawing.Point(16, 52);
            this.chkAreaFill.Name = "chkAreaFill";
            this.chkAreaFill.Size = new System.Drawing.Size(150, 20);
            this.chkAreaFill.TabIndex = 4;
            this.chkAreaFill.Text = "Area Fill";
            // 
            // chkForecastBand
            // 
            this.chkForecastBand.Location = new System.Drawing.Point(200, 52);
            this.chkForecastBand.Name = "chkForecastBand";
            this.chkForecastBand.Size = new System.Drawing.Size(150, 20);
            this.chkForecastBand.TabIndex = 5;
            this.chkForecastBand.Text = "Forecast Band";
            // 
            // chkPeakLine
            // 
            this.chkPeakLine.Location = new System.Drawing.Point(16, 80);
            this.chkPeakLine.Name = "chkPeakLine";
            this.chkPeakLine.Size = new System.Drawing.Size(150, 20);
            this.chkPeakLine.TabIndex = 6;
            this.chkPeakLine.Text = "Peak Line";
            // 
            // chkStepLines
            // 
            this.chkStepLines.Location = new System.Drawing.Point(200, 80);
            this.chkStepLines.Name = "chkStepLines";
            this.chkStepLines.Size = new System.Drawing.Size(150, 20);
            this.chkStepLines.TabIndex = 7;
            this.chkStepLines.Text = "Step Lines";
            // 
            // chkTimeZoneTarget
            // 
            this.chkTimeZoneTarget.Location = new System.Drawing.Point(16, 108);
            this.chkTimeZoneTarget.Name = "chkTimeZoneTarget";
            this.chkTimeZoneTarget.Size = new System.Drawing.Size(170, 20);
            this.chkTimeZoneTarget.TabIndex = 8;
            this.chkTimeZoneTarget.Text = "Timezone Target";
            // 
            // chkDetailedAxis
            // 
            this.chkDetailedAxis.Location = new System.Drawing.Point(200, 108);
            this.chkDetailedAxis.Name = "chkDetailedAxis";
            this.chkDetailedAxis.Size = new System.Drawing.Size(150, 20);
            this.chkDetailedAxis.TabIndex = 9;
            this.chkDetailedAxis.Text = "Detailed Axis";
            // 
            // tabColors
            // 
            this.tabColors.Controls.Add(this.lblSectionTrend);
            this.tabColors.Controls.Add(this.lblClrPrediction);
            this.tabColors.Controls.Add(this.btnPrediction);
            this.tabColors.Controls.Add(this.lblClrTarget);
            this.tabColors.Controls.Add(this.btnTarget);
            this.tabColors.Controls.Add(this.lblClrExcess);
            this.tabColors.Controls.Add(this.btnExcess);
            this.tabColors.Controls.Add(this.lblClrAreaFill);
            this.tabColors.Controls.Add(this.btnAreaFillColor);
            this.tabColors.Controls.Add(this.lblClrForecastBand);
            this.tabColors.Controls.Add(this.btnForecastBandColor);
            this.tabColors.Controls.Add(this.lblClrPeakLine);
            this.tabColors.Controls.Add(this.btnPeakLineColor);
            this.tabColors.Controls.Add(this.lblClrStatusBack);
            this.tabColors.Controls.Add(this.btnStatusBack);
            this.tabColors.Controls.Add(this.lblClrStatusFill);
            this.tabColors.Controls.Add(this.btnStatusFill);
            this.tabColors.Controls.Add(this.lblClrStatusValue);
            this.tabColors.Controls.Add(this.btnStatusValue);
            this.tabColors.Controls.Add(this.lblClrStatusTitle);
            this.tabColors.Controls.Add(this.btnStatusTitle);
            this.tabColors.Controls.Add(this.lblClrTargetValue);
            this.tabColors.Controls.Add(this.btnTargetValue);
            this.tabColors.Controls.Add(this.lblClrPreValue);
            this.tabColors.Controls.Add(this.btnPreValue);
            this.tabColors.Controls.Add(this.lblClrExValue);
            this.tabColors.Controls.Add(this.btnExValue);
            this.tabColors.Controls.Add(this.lblClrCardBack);
            this.tabColors.Controls.Add(this.btnCardBack);
            this.tabColors.Controls.Add(this.lblClrCardBorder);
            this.tabColors.Controls.Add(this.btnCardBorder);
            this.tabColors.Location = new System.Drawing.Point(4, 22);
            this.tabColors.Name = "tabColors";
            this.tabColors.Padding = new System.Windows.Forms.Padding(3);
            this.tabColors.Size = new System.Drawing.Size(542, 424);
            this.tabColors.TabIndex = 3;
            this.tabColors.Text = "Colors";
            // 
            // lblSectionTrend
            // 
            this.lblSectionTrend.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblSectionTrend.Location = new System.Drawing.Point(16, 16);
            this.lblSectionTrend.Name = "lblSectionTrend";
            this.lblSectionTrend.Size = new System.Drawing.Size(200, 18);
            this.lblSectionTrend.TabIndex = 0;
            this.lblSectionTrend.Text = "Trend";
            // 
            // lblClrPrediction
            // 
            this.lblClrPrediction.Location = new System.Drawing.Point(16, 40);
            this.lblClrPrediction.Name = "lblClrPrediction";
            this.lblClrPrediction.Size = new System.Drawing.Size(90, 18);
            this.lblClrPrediction.TabIndex = 1;
            this.lblClrPrediction.Text = "Prediction";
            // 
            // btnPrediction
            // 
            this.btnPrediction.Location = new System.Drawing.Point(108, 36);
            this.btnPrediction.Name = "btnPrediction";
            this.btnPrediction.Size = new System.Drawing.Size(36, 22);
            this.btnPrediction.TabIndex = 2;
            this.btnPrediction.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrTarget
            // 
            this.lblClrTarget.Location = new System.Drawing.Point(190, 40);
            this.lblClrTarget.Name = "lblClrTarget";
            this.lblClrTarget.Size = new System.Drawing.Size(90, 18);
            this.lblClrTarget.TabIndex = 3;
            this.lblClrTarget.Text = "Target";
            // 
            // btnTarget
            // 
            this.btnTarget.Location = new System.Drawing.Point(282, 36);
            this.btnTarget.Name = "btnTarget";
            this.btnTarget.Size = new System.Drawing.Size(36, 22);
            this.btnTarget.TabIndex = 4;
            this.btnTarget.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrExcess
            // 
            this.lblClrExcess.Location = new System.Drawing.Point(360, 40);
            this.lblClrExcess.Name = "lblClrExcess";
            this.lblClrExcess.Size = new System.Drawing.Size(90, 18);
            this.lblClrExcess.TabIndex = 5;
            this.lblClrExcess.Text = "Excess";
            // 
            // btnExcess
            // 
            this.btnExcess.Location = new System.Drawing.Point(452, 36);
            this.btnExcess.Name = "btnExcess";
            this.btnExcess.Size = new System.Drawing.Size(36, 22);
            this.btnExcess.TabIndex = 6;
            this.btnExcess.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrAreaFill
            // 
            this.lblClrAreaFill.Location = new System.Drawing.Point(16, 74);
            this.lblClrAreaFill.Name = "lblClrAreaFill";
            this.lblClrAreaFill.Size = new System.Drawing.Size(90, 18);
            this.lblClrAreaFill.TabIndex = 7;
            this.lblClrAreaFill.Text = "Area Fill";
            // 
            // btnAreaFillColor
            // 
            this.btnAreaFillColor.Location = new System.Drawing.Point(108, 70);
            this.btnAreaFillColor.Name = "btnAreaFillColor";
            this.btnAreaFillColor.Size = new System.Drawing.Size(36, 22);
            this.btnAreaFillColor.TabIndex = 8;
            this.btnAreaFillColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrForecastBand
            // 
            this.lblClrForecastBand.Location = new System.Drawing.Point(190, 74);
            this.lblClrForecastBand.Name = "lblClrForecastBand";
            this.lblClrForecastBand.Size = new System.Drawing.Size(90, 18);
            this.lblClrForecastBand.TabIndex = 9;
            this.lblClrForecastBand.Text = "Forecast Band";
            // 
            // btnForecastBandColor
            // 
            this.btnForecastBandColor.Location = new System.Drawing.Point(282, 70);
            this.btnForecastBandColor.Name = "btnForecastBandColor";
            this.btnForecastBandColor.Size = new System.Drawing.Size(36, 22);
            this.btnForecastBandColor.TabIndex = 10;
            this.btnForecastBandColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrPeakLine
            // 
            this.lblClrPeakLine.Location = new System.Drawing.Point(360, 74);
            this.lblClrPeakLine.Name = "lblClrPeakLine";
            this.lblClrPeakLine.Size = new System.Drawing.Size(90, 18);
            this.lblClrPeakLine.TabIndex = 11;
            this.lblClrPeakLine.Text = "Peak Line";
            // 
            // btnPeakLineColor
            // 
            this.btnPeakLineColor.Location = new System.Drawing.Point(452, 70);
            this.btnPeakLineColor.Name = "btnPeakLineColor";
            this.btnPeakLineColor.Size = new System.Drawing.Size(36, 22);
            this.btnPeakLineColor.TabIndex = 12;
            this.btnPeakLineColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrStatusBack
            // 
            this.lblClrStatusBack.Location = new System.Drawing.Point(16, 114);
            this.lblClrStatusBack.Name = "lblClrStatusBack";
            this.lblClrStatusBack.Size = new System.Drawing.Size(90, 18);
            this.lblClrStatusBack.TabIndex = 13;
            this.lblClrStatusBack.Text = "Status Back";
            // 
            // btnStatusBack
            // 
            this.btnStatusBack.Location = new System.Drawing.Point(108, 110);
            this.btnStatusBack.Name = "btnStatusBack";
            this.btnStatusBack.Size = new System.Drawing.Size(36, 22);
            this.btnStatusBack.TabIndex = 14;
            this.btnStatusBack.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrStatusFill
            // 
            this.lblClrStatusFill.Location = new System.Drawing.Point(190, 114);
            this.lblClrStatusFill.Name = "lblClrStatusFill";
            this.lblClrStatusFill.Size = new System.Drawing.Size(90, 18);
            this.lblClrStatusFill.TabIndex = 15;
            this.lblClrStatusFill.Text = "Status Fill";
            // 
            // btnStatusFill
            // 
            this.btnStatusFill.Location = new System.Drawing.Point(282, 110);
            this.btnStatusFill.Name = "btnStatusFill";
            this.btnStatusFill.Size = new System.Drawing.Size(36, 22);
            this.btnStatusFill.TabIndex = 16;
            this.btnStatusFill.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrStatusValue
            // 
            this.lblClrStatusValue.Location = new System.Drawing.Point(360, 114);
            this.lblClrStatusValue.Name = "lblClrStatusValue";
            this.lblClrStatusValue.Size = new System.Drawing.Size(90, 18);
            this.lblClrStatusValue.TabIndex = 17;
            this.lblClrStatusValue.Text = "Status Value";
            // 
            // btnStatusValue
            // 
            this.btnStatusValue.Location = new System.Drawing.Point(452, 110);
            this.btnStatusValue.Name = "btnStatusValue";
            this.btnStatusValue.Size = new System.Drawing.Size(36, 22);
            this.btnStatusValue.TabIndex = 18;
            this.btnStatusValue.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrStatusTitle
            // 
            this.lblClrStatusTitle.Location = new System.Drawing.Point(16, 148);
            this.lblClrStatusTitle.Name = "lblClrStatusTitle";
            this.lblClrStatusTitle.Size = new System.Drawing.Size(90, 18);
            this.lblClrStatusTitle.TabIndex = 19;
            this.lblClrStatusTitle.Text = "Status Title";
            // 
            // btnStatusTitle
            // 
            this.btnStatusTitle.Location = new System.Drawing.Point(108, 144);
            this.btnStatusTitle.Name = "btnStatusTitle";
            this.btnStatusTitle.Size = new System.Drawing.Size(36, 22);
            this.btnStatusTitle.TabIndex = 20;
            this.btnStatusTitle.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrTargetValue
            // 
            this.lblClrTargetValue.Location = new System.Drawing.Point(190, 148);
            this.lblClrTargetValue.Name = "lblClrTargetValue";
            this.lblClrTargetValue.Size = new System.Drawing.Size(90, 18);
            this.lblClrTargetValue.TabIndex = 21;
            this.lblClrTargetValue.Text = "Target Value";
            // 
            // btnTargetValue
            // 
            this.btnTargetValue.Location = new System.Drawing.Point(282, 144);
            this.btnTargetValue.Name = "btnTargetValue";
            this.btnTargetValue.Size = new System.Drawing.Size(36, 22);
            this.btnTargetValue.TabIndex = 22;
            this.btnTargetValue.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrPreValue
            // 
            this.lblClrPreValue.Location = new System.Drawing.Point(360, 148);
            this.lblClrPreValue.Name = "lblClrPreValue";
            this.lblClrPreValue.Size = new System.Drawing.Size(90, 18);
            this.lblClrPreValue.TabIndex = 23;
            this.lblClrPreValue.Text = "Predict Value";
            // 
            // btnPreValue
            // 
            this.btnPreValue.Location = new System.Drawing.Point(452, 144);
            this.btnPreValue.Name = "btnPreValue";
            this.btnPreValue.Size = new System.Drawing.Size(36, 22);
            this.btnPreValue.TabIndex = 24;
            this.btnPreValue.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrExValue
            // 
            this.lblClrExValue.Location = new System.Drawing.Point(16, 182);
            this.lblClrExValue.Name = "lblClrExValue";
            this.lblClrExValue.Size = new System.Drawing.Size(90, 18);
            this.lblClrExValue.TabIndex = 25;
            this.lblClrExValue.Text = "Excess Value";
            // 
            // btnExValue
            // 
            this.btnExValue.Location = new System.Drawing.Point(108, 178);
            this.btnExValue.Name = "btnExValue";
            this.btnExValue.Size = new System.Drawing.Size(36, 22);
            this.btnExValue.TabIndex = 26;
            this.btnExValue.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrCardBack
            // 
            this.lblClrCardBack.Location = new System.Drawing.Point(190, 182);
            this.lblClrCardBack.Name = "lblClrCardBack";
            this.lblClrCardBack.Size = new System.Drawing.Size(90, 18);
            this.lblClrCardBack.TabIndex = 27;
            this.lblClrCardBack.Text = "Card Back";
            // 
            // btnCardBack
            // 
            this.btnCardBack.Location = new System.Drawing.Point(282, 178);
            this.btnCardBack.Name = "btnCardBack";
            this.btnCardBack.Size = new System.Drawing.Size(36, 22);
            this.btnCardBack.TabIndex = 28;
            this.btnCardBack.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // lblClrCardBorder
            // 
            this.lblClrCardBorder.Location = new System.Drawing.Point(360, 182);
            this.lblClrCardBorder.Name = "lblClrCardBorder";
            this.lblClrCardBorder.Size = new System.Drawing.Size(90, 18);
            this.lblClrCardBorder.TabIndex = 29;
            this.lblClrCardBorder.Text = "Card Border";
            // 
            // btnCardBorder
            // 
            this.btnCardBorder.Location = new System.Drawing.Point(452, 178);
            this.btnCardBorder.Name = "btnCardBorder";
            this.btnCardBorder.Size = new System.Drawing.Size(36, 22);
            this.btnCardBorder.TabIndex = 30;
            this.btnCardBorder.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // PropertyPageObjectDemandChart
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(550, 450);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PropertyPageObjectDemandChart";
            this.Load += new System.EventHandler(this.PropertyPageObjectDemandChart_Load);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numPercentY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetThick)).EndInit();
            this.tabStyle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCornerRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGridIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKpiOpacity)).EndInit();
            this.tabTrend.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTrendThick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numForecastThick)).EndInit();
            this.tabColors.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		private void ApplyModernStyle()
		{
			DemandUIStyle.StyleForm(this);
			DemandUIStyle.StyleAllControls(this);
		}

		private void FillComboBox()
		{
			comboBlockId.Items.Clear();
			try
			{
				var configs = DemandNewConfigLoader.LoadAll();
				for (int i = 0; i < configs.Count; i++)
				{
					if (!string.IsNullOrEmpty(configs[i].BlockId))
						comboBlockId.Items.Add(configs[i].BlockId);
				}
			}
			catch { }
		}

		private void PropertyPageObjectDemandChart_Load(object sender, EventArgs e)
		{
			FillComboBox();
		}

		private void buttonConfig_Click(object sender, EventArgs e)
		{
			FormConfigDemandNew.FunctionBlockDemandNew(FindForm());
			FillComboBox();
		}

		private void buttonColor_Click(object sender, EventArgs e)
		{
			Button btn = sender as Button;
			if (btn == null) return;
			FormColorDialog dialog = new FormColorDialog();
			dialog.SetSelectedColor(btn.BackColor, true);
			dialog.StartPosition = FormStartPosition.CenterParent;
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				btn.BackColor = dialog.GetSelectedColor();
			}
		}

		private void comboTheme_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_isBindingObjectArgs || _isRevertingTheme)
			{
				_lastThemeIndex = Math.Max(0, comboTheme.SelectedIndex);
				return;
			}

			int newTheme = Math.Max(0, comboTheme.SelectedIndex);
			if (newTheme == _lastThemeIndex) return;

			string msg = Tools.IsLangKorean()
				? "테마를 변경하면 DemandChart의 스타일/트렌드/색상 속성이 프리셋으로 전체 변경됩니다.\r\n적용하시겠습니까?"
				: "Changing theme will apply preset values to all DemandChart style/trend/color properties.\r\nDo you want to apply it?";
			string title = Tools.IsLangKorean() ? "테마 적용" : "Apply Theme";

			if (MessageBox.Show(this, msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
			{
				_isRevertingTheme = true;
				comboTheme.SelectedIndex = _lastThemeIndex;
				_isRevertingTheme = false;
				return;
			}

			ApplyThemePreset(newTheme);
			_lastThemeIndex = newTheme;
		}

		private void ApplyThemePreset(int theme)
		{
			theme = Math.Max(0, Math.Min(2, theme));

			// General preset
			numPercentY.Value = 120;
			numTargetThick.Value = (theme == 2) ? 2 : 1;
			comboStatusPos.SelectedIndex = (theme == 1) ? 1 : 0;

			// Style preset
			comboKpiDensity.SelectedIndex = (theme == 1) ? 1 : 0;
			numCornerRadius.Value = (theme == 2) ? 2 : 8;
			numGridIntensity.Value = (theme == 2) ? 70 : ((theme == 1) ? 55 : 45);
			numKpiOpacity.Value = (theme == 1) ? 90 : 82;
			chkGridLabels.Checked = true;

			// Trend preset
			numTrendThick.Value = (theme == 2) ? 3 : 2;
			numForecastThick.Value = (theme == 2) ? 2 : 1;
			chkAreaFill.Checked = true;
			chkForecastBand.Checked = true;
			chkPeakLine.Checked = true;
			chkStepLines.Checked = true;
			chkTimeZoneTarget.Checked = true;
			chkDetailedAxis.Checked = (theme != 0);

			// Color preset
			if (theme == 0) // Light Minimal
			{
				btnPrediction.BackColor = Color.FromArgb(0, 122, 204);
				btnExcess.BackColor = Color.FromArgb(217, 72, 15);
				btnTarget.BackColor = Color.FromArgb(32, 167, 86);
				btnStatusBack.BackColor = Color.FromArgb(236, 241, 246);
				btnStatusFill.BackColor = Color.FromArgb(51, 78, 104);
				btnStatusTitle.BackColor = Color.FromArgb(36, 44, 58);
				btnStatusValue.BackColor = Color.FromArgb(18, 33, 48);
				btnTargetValue.BackColor = Color.FromArgb(32, 167, 86);
				btnPreValue.BackColor = Color.FromArgb(0, 122, 204);
				btnExValue.BackColor = Color.FromArgb(217, 72, 15);
				btnCardBack.BackColor = Color.FromArgb(245, 248, 252);
				btnCardBorder.BackColor = Color.FromArgb(206, 216, 227);
				btnAreaFillColor.BackColor = Color.FromArgb(48, 255, 141, 64);
				btnForecastBandColor.BackColor = Color.FromArgb(48, 30, 136, 229);
				btnPeakLineColor.BackColor = Color.FromArgb(255, 255, 152, 0);
			}
			else if (theme == 1) // Dark Ops
			{
				btnPrediction.BackColor = Color.FromArgb(88, 188, 255);
				btnExcess.BackColor = Color.FromArgb(255, 122, 122);
				btnTarget.BackColor = Color.FromArgb(98, 214, 149);
				btnStatusBack.BackColor = Color.FromArgb(44, 54, 66);
				btnStatusFill.BackColor = Color.FromArgb(70, 92, 120);
				btnStatusTitle.BackColor = Color.FromArgb(210, 220, 232);
				btnStatusValue.BackColor = Color.FromArgb(228, 236, 246);
				btnTargetValue.BackColor = Color.FromArgb(116, 230, 168);
				btnPreValue.BackColor = Color.FromArgb(129, 209, 255);
				btnExValue.BackColor = Color.FromArgb(255, 138, 138);
				btnCardBack.BackColor = Color.FromArgb(35, 45, 58);
				btnCardBorder.BackColor = Color.FromArgb(103, 121, 138);
				btnAreaFillColor.BackColor = Color.FromArgb(76, 65, 167, 255);
				btnForecastBandColor.BackColor = Color.FromArgb(82, 64, 174, 255);
				btnPeakLineColor.BackColor = Color.FromArgb(255, 255, 196, 96);
			}
			else // High Contrast
			{
				btnPrediction.BackColor = Color.FromArgb(0, 86, 179);
				btnExcess.BackColor = Color.FromArgb(190, 20, 20);
				btnTarget.BackColor = Color.FromArgb(0, 135, 62);
				btnStatusBack.BackColor = Color.FromArgb(255, 255, 255);
				btnStatusFill.BackColor = Color.FromArgb(10, 10, 10);
				btnStatusTitle.BackColor = Color.FromArgb(0, 0, 0);
				btnStatusValue.BackColor = Color.FromArgb(0, 0, 0);
				btnTargetValue.BackColor = Color.FromArgb(0, 120, 53);
				btnPreValue.BackColor = Color.FromArgb(0, 76, 158);
				btnExValue.BackColor = Color.FromArgb(170, 18, 18);
				btnCardBack.BackColor = Color.FromArgb(255, 255, 255);
				btnCardBorder.BackColor = Color.FromArgb(0, 0, 0);
				btnAreaFillColor.BackColor = Color.FromArgb(90, 0, 122, 204);
				btnForecastBandColor.BackColor = Color.FromArgb(110, 0, 0, 0);
				btnPeakLineColor.BackColor = Color.FromArgb(255, 175, 94, 0);
			}
		}

		public ObjectArgsDemandChart ObjectArgs
		{
			set
			{
				_isBindingObjectArgs = true;
				try
				{
					if (value == null) value = new ObjectArgsDemandChart();
					comboBlockId.Text = value.demand_block_id;
					numPercentY.Value = Math.Max(100, Math.Min(200, value.nPercentY));
					numTargetThick.Value = Math.Max(1, Math.Min(20, value.thick_target));
					comboStatusPos.SelectedIndex = Math.Max(0, Math.Min(2, value.nStatusBarPos));
					comboTheme.SelectedIndex = Math.Max(0, Math.Min(2, value.uiTheme));

					chkAreaFill.Checked = value.showAreaFill;
					chkPeakLine.Checked = value.showPeakLine;
					chkForecastBand.Checked = value.showForecastBand;
					chkGridLabels.Checked = value.showGridLabels;
					chkStepLines.Checked = value.showStepLines;
					chkTimeZoneTarget.Checked = value.showTimeZoneTarget;
					chkDetailedAxis.Checked = value.showDetailedAxis;
					numGridIntensity.Value = Math.Max(0, Math.Min(100, value.gridIntensity));
					numTrendThick.Value = Math.Max(1, Math.Min(5, value.lineThicknessTrend));
					numForecastThick.Value = Math.Max(1, Math.Min(5, value.lineThicknessForecast));
					comboKpiDensity.SelectedIndex = Math.Max(0, Math.Min(1, value.kpiDensity));
					numCornerRadius.Value = Math.Max(0, Math.Min(16, value.cornerRadius));
					numKpiOpacity.Value = Math.Max(30, Math.Min(100, value.kpiOpacity));

					btnPrediction.BackColor = value.lColorPrediction;
					btnExcess.BackColor = value.lColorExcess;
					btnTarget.BackColor = value.lColorTarget;
					btnStatusBack.BackColor = value.lColorStatusBack;
					btnStatusFill.BackColor = value.lColorStatusFill;
					btnStatusValue.BackColor = value.lColorStatusValue;
					btnStatusTitle.BackColor = value.lColorStatusTitle;
					btnTargetValue.BackColor = value.lColorTargetValue;
					btnPreValue.BackColor = value.lColorPreValue;
					btnExValue.BackColor = value.lColorExValue;
					btnCardBack.BackColor = value.lColorCardBack;
					btnCardBorder.BackColor = value.lColorCardBorder;
					btnAreaFillColor.BackColor = value.lColorAreaFill;
					btnForecastBandColor.BackColor = value.lColorForecastBand;
					btnPeakLineColor.BackColor = value.lColorPeakLine;
					_lastThemeIndex = Math.Max(0, comboTheme.SelectedIndex);
				}
				finally
				{
					_isBindingObjectArgs = false;
				}
			}
			get
			{
				ObjectArgsDemandChart args = new ObjectArgsDemandChart();
				args.demand_block_id = comboBlockId.Text;
				args.nPercentY = (int)numPercentY.Value;
				args.thick_target = (int)numTargetThick.Value;
				args.nStatusBarPos = Math.Max(0, comboStatusPos.SelectedIndex);
				args.uiTheme = Math.Max(0, comboTheme.SelectedIndex);

				args.showAreaFill = chkAreaFill.Checked;
				args.showPeakLine = chkPeakLine.Checked;
				args.showForecastBand = chkForecastBand.Checked;
				args.showGridLabels = chkGridLabels.Checked;
				args.showStepLines = chkStepLines.Checked;
				args.showTimeZoneTarget = chkTimeZoneTarget.Checked;
				args.showDetailedAxis = chkDetailedAxis.Checked;
				args.gridIntensity = (int)numGridIntensity.Value;
				args.lineThicknessTrend = (int)numTrendThick.Value;
				args.lineThicknessForecast = (int)numForecastThick.Value;
				args.kpiDensity = Math.Max(0, comboKpiDensity.SelectedIndex);
				args.cornerRadius = (int)numCornerRadius.Value;
				args.kpiOpacity = (int)numKpiOpacity.Value;

				args.lColorPrediction = btnPrediction.BackColor;
				args.lColorExcess = btnExcess.BackColor;
				args.lColorTarget = btnTarget.BackColor;
				args.lColorStatusBack = btnStatusBack.BackColor;
				args.lColorStatusFill = btnStatusFill.BackColor;
				args.lColorStatusValue = btnStatusValue.BackColor;
				args.lColorStatusTitle = btnStatusTitle.BackColor;
				args.lColorTargetValue = btnTargetValue.BackColor;
				args.lColorPreValue = btnPreValue.BackColor;
				args.lColorExValue = btnExValue.BackColor;
				args.lColorCardBack = btnCardBack.BackColor;
				args.lColorCardBorder = btnCardBorder.BackColor;
				args.lColorAreaFill = btnAreaFillColor.BackColor;
				args.lColorForecastBand = btnForecastBandColor.BackColor;
				args.lColorPeakLine = btnPeakLineColor.BackColor;
				return args;
			}
		}
	}
}
