using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using DatabaseConnection;
using NetTools;
using Studio.Script;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectDatabaseTrend.
	/// </summary>
	public class PropertyPageObjectMultiTrend : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownShowUnit;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownTimeDevide;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.NumericUpDown numericUpDownGuideDevide;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonDataCycle3;
		private System.Windows.Forms.RadioButton radioButtonDataCycle2;
		private System.Windows.Forms.RadioButton radioButtonDataCycle1;
		private System.Windows.Forms.RadioButton radioButtonDataCycle0;
        private Button buttonEventCellClick;
		private System.Windows.Forms.Label label12;
        private CheckBox checkBoxUseMouseButtonAsZoom;
        private CheckBox checkBoxDontUseConfigurationDialog;
        private CheckBox checkBoxZoomWithYAxis;
        private Label label1;
        private NumericUpDown numericUpDownDataCycleUnit;
        private GroupBox groupBox2;
        private Label label2;
        private CheckBox checkBoxLogarithmicScaleUse;
        private NumericUpDown numericUpDownLogarithmicScaleBase;
        private CheckBox checkBoxDontUseConfigurationAutoRange;
        private GroupBox groupBox9;
        private GroupBox groupBox8;
        private RadioButton radioButtonToolbarBtnColor0;
        private RadioButton radioButtonToolbarBtnColor1;
        private GroupBox groupBox10;
        private RadioButton radioButtonToolbarPos3;
        private RadioButton radioButtonToolbarPos2;
        private RadioButton radioButtonToolbarPos1;
        private RadioButton radioButtonToolbarPos0;
        private CheckBox checkBoxHideDataRange;
        private CheckBox checkBoxUseToolBar;
        private Label label8;
        private Label label5;
        private NumericUpDown numericUpDownToolbarTextSize;
        private NumericUpDown numericUpDownToolbarButtonSize;

        ScriptClass scriptEventAfterSettings;

		public PropertyPageObjectMultiTrend()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectMultiTrend));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownShowUnit = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownTimeDevide = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericUpDownGuideDevide = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownDataCycleUnit = new System.Windows.Forms.NumericUpDown();
            this.radioButtonDataCycle3 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle0 = new System.Windows.Forms.RadioButton();
            this.buttonEventCellClick = new System.Windows.Forms.Button();
            this.checkBoxUseMouseButtonAsZoom = new System.Windows.Forms.CheckBox();
            this.checkBoxDontUseConfigurationDialog = new System.Windows.Forms.CheckBox();
            this.checkBoxZoomWithYAxis = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownLogarithmicScaleBase = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxLogarithmicScaleUse = new System.Windows.Forms.CheckBox();
            this.checkBoxDontUseConfigurationAutoRange = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.radioButtonToolbarBtnColor0 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarBtnColor1 = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.radioButtonToolbarPos3 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarPos2 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarPos1 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarPos0 = new System.Windows.Forms.RadioButton();
            this.checkBoxHideDataRange = new System.Windows.Forms.CheckBox();
            this.numericUpDownToolbarTextSize = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownToolbarButtonSize = new System.Windows.Forms.NumericUpDown();
            this.checkBoxUseToolBar = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeDevide)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideDevide)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataCycleUnit)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLogarithmicScaleBase)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarTextSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarButtonSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownShowUnit);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownShowUnit
            // 
            this.numericUpDownShowUnit.AccessibleDescription = null;
            this.numericUpDownShowUnit.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownShowUnit, "numericUpDownShowUnit");
            this.numericUpDownShowUnit.Font = null;
            this.numericUpDownShowUnit.Maximum = new decimal(new int[] {
            44640,
            0,
            0,
            0});
            this.numericUpDownShowUnit.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownShowUnit.Name = "numericUpDownShowUnit";
            this.numericUpDownShowUnit.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // numericUpDownTimeDevide
            // 
            this.numericUpDownTimeDevide.AccessibleDescription = null;
            this.numericUpDownTimeDevide.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTimeDevide, "numericUpDownTimeDevide");
            this.numericUpDownTimeDevide.Font = null;
            this.numericUpDownTimeDevide.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTimeDevide.Name = "numericUpDownTimeDevide";
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.numericUpDownGuideDevide);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.numericUpDownTimeDevide);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // numericUpDownGuideDevide
            // 
            this.numericUpDownGuideDevide.AccessibleDescription = null;
            this.numericUpDownGuideDevide.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownGuideDevide, "numericUpDownGuideDevide");
            this.numericUpDownGuideDevide.Font = null;
            this.numericUpDownGuideDevide.Name = "numericUpDownGuideDevide";
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownDataCycleUnit);
            this.groupBox1.Controls.Add(this.radioButtonDataCycle3);
            this.groupBox1.Controls.Add(this.radioButtonDataCycle2);
            this.groupBox1.Controls.Add(this.radioButtonDataCycle1);
            this.groupBox1.Controls.Add(this.radioButtonDataCycle0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownDataCycleUnit
            // 
            this.numericUpDownDataCycleUnit.AccessibleDescription = null;
            this.numericUpDownDataCycleUnit.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDataCycleUnit, "numericUpDownDataCycleUnit");
            this.numericUpDownDataCycleUnit.Font = null;
            this.numericUpDownDataCycleUnit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDataCycleUnit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDataCycleUnit.Name = "numericUpDownDataCycleUnit";
            this.numericUpDownDataCycleUnit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonDataCycle3
            // 
            this.radioButtonDataCycle3.AccessibleDescription = null;
            this.radioButtonDataCycle3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDataCycle3, "radioButtonDataCycle3");
            this.radioButtonDataCycle3.BackgroundImage = null;
            this.radioButtonDataCycle3.Font = null;
            this.radioButtonDataCycle3.Name = "radioButtonDataCycle3";
            // 
            // radioButtonDataCycle2
            // 
            this.radioButtonDataCycle2.AccessibleDescription = null;
            this.radioButtonDataCycle2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDataCycle2, "radioButtonDataCycle2");
            this.radioButtonDataCycle2.BackgroundImage = null;
            this.radioButtonDataCycle2.Font = null;
            this.radioButtonDataCycle2.Name = "radioButtonDataCycle2";
            // 
            // radioButtonDataCycle1
            // 
            this.radioButtonDataCycle1.AccessibleDescription = null;
            this.radioButtonDataCycle1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDataCycle1, "radioButtonDataCycle1");
            this.radioButtonDataCycle1.BackgroundImage = null;
            this.radioButtonDataCycle1.Font = null;
            this.radioButtonDataCycle1.Name = "radioButtonDataCycle1";
            this.radioButtonDataCycle1.CheckedChanged += new System.EventHandler(this.radioButtonDataCycle1_CheckedChanged);
            // 
            // radioButtonDataCycle0
            // 
            this.radioButtonDataCycle0.AccessibleDescription = null;
            this.radioButtonDataCycle0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDataCycle0, "radioButtonDataCycle0");
            this.radioButtonDataCycle0.BackgroundImage = null;
            this.radioButtonDataCycle0.Font = null;
            this.radioButtonDataCycle0.Name = "radioButtonDataCycle0";
            // 
            // buttonEventCellClick
            // 
            this.buttonEventCellClick.AccessibleDescription = null;
            this.buttonEventCellClick.AccessibleName = null;
            resources.ApplyResources(this.buttonEventCellClick, "buttonEventCellClick");
            this.buttonEventCellClick.BackgroundImage = null;
            this.buttonEventCellClick.Font = null;
            this.buttonEventCellClick.Name = "buttonEventCellClick";
            this.buttonEventCellClick.UseVisualStyleBackColor = true;
            this.buttonEventCellClick.Click += new System.EventHandler(this.buttonEventCellClick_Click);
            // 
            // checkBoxUseMouseButtonAsZoom
            // 
            this.checkBoxUseMouseButtonAsZoom.AccessibleDescription = null;
            this.checkBoxUseMouseButtonAsZoom.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseMouseButtonAsZoom, "checkBoxUseMouseButtonAsZoom");
            this.checkBoxUseMouseButtonAsZoom.BackgroundImage = null;
            this.checkBoxUseMouseButtonAsZoom.Font = null;
            this.checkBoxUseMouseButtonAsZoom.Name = "checkBoxUseMouseButtonAsZoom";
            this.checkBoxUseMouseButtonAsZoom.UseVisualStyleBackColor = true;
            this.checkBoxUseMouseButtonAsZoom.CheckedChanged += new System.EventHandler(this.checkBoxUseMouseButtonAsZoom_CheckedChanged);
            // 
            // checkBoxDontUseConfigurationDialog
            // 
            this.checkBoxDontUseConfigurationDialog.AccessibleDescription = null;
            this.checkBoxDontUseConfigurationDialog.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDontUseConfigurationDialog, "checkBoxDontUseConfigurationDialog");
            this.checkBoxDontUseConfigurationDialog.BackgroundImage = null;
            this.checkBoxDontUseConfigurationDialog.Font = null;
            this.checkBoxDontUseConfigurationDialog.Name = "checkBoxDontUseConfigurationDialog";
            this.checkBoxDontUseConfigurationDialog.UseVisualStyleBackColor = true;
            // 
            // checkBoxZoomWithYAxis
            // 
            this.checkBoxZoomWithYAxis.AccessibleDescription = null;
            this.checkBoxZoomWithYAxis.AccessibleName = null;
            resources.ApplyResources(this.checkBoxZoomWithYAxis, "checkBoxZoomWithYAxis");
            this.checkBoxZoomWithYAxis.BackgroundImage = null;
            this.checkBoxZoomWithYAxis.Font = null;
            this.checkBoxZoomWithYAxis.Name = "checkBoxZoomWithYAxis";
            this.checkBoxZoomWithYAxis.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.numericUpDownLogarithmicScaleBase);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.checkBoxLogarithmicScaleUse);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // numericUpDownLogarithmicScaleBase
            // 
            this.numericUpDownLogarithmicScaleBase.AccessibleDescription = null;
            this.numericUpDownLogarithmicScaleBase.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownLogarithmicScaleBase, "numericUpDownLogarithmicScaleBase");
            this.numericUpDownLogarithmicScaleBase.DecimalPlaces = 1;
            this.numericUpDownLogarithmicScaleBase.Font = null;
            this.numericUpDownLogarithmicScaleBase.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLogarithmicScaleBase.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownLogarithmicScaleBase.Name = "numericUpDownLogarithmicScaleBase";
            this.numericUpDownLogarithmicScaleBase.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // checkBoxLogarithmicScaleUse
            // 
            this.checkBoxLogarithmicScaleUse.AccessibleDescription = null;
            this.checkBoxLogarithmicScaleUse.AccessibleName = null;
            resources.ApplyResources(this.checkBoxLogarithmicScaleUse, "checkBoxLogarithmicScaleUse");
            this.checkBoxLogarithmicScaleUse.BackgroundImage = null;
            this.checkBoxLogarithmicScaleUse.Font = null;
            this.checkBoxLogarithmicScaleUse.Name = "checkBoxLogarithmicScaleUse";
            this.checkBoxLogarithmicScaleUse.UseVisualStyleBackColor = true;
            this.checkBoxLogarithmicScaleUse.CheckedChanged += new System.EventHandler(this.checkBoxLogarithmicScaleUse_CheckedChanged);
            // 
            // checkBoxDontUseConfigurationAutoRange
            // 
            this.checkBoxDontUseConfigurationAutoRange.AccessibleDescription = null;
            this.checkBoxDontUseConfigurationAutoRange.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDontUseConfigurationAutoRange, "checkBoxDontUseConfigurationAutoRange");
            this.checkBoxDontUseConfigurationAutoRange.BackgroundImage = null;
            this.checkBoxDontUseConfigurationAutoRange.Font = null;
            this.checkBoxDontUseConfigurationAutoRange.Name = "checkBoxDontUseConfigurationAutoRange";
            this.checkBoxDontUseConfigurationAutoRange.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.AccessibleDescription = null;
            this.groupBox9.AccessibleName = null;
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.BackgroundImage = null;
            this.groupBox9.Controls.Add(this.groupBox8);
            this.groupBox9.Controls.Add(this.label8);
            this.groupBox9.Controls.Add(this.label5);
            this.groupBox9.Controls.Add(this.groupBox10);
            this.groupBox9.Controls.Add(this.checkBoxHideDataRange);
            this.groupBox9.Controls.Add(this.numericUpDownToolbarTextSize);
            this.groupBox9.Controls.Add(this.numericUpDownToolbarButtonSize);
            this.groupBox9.Controls.Add(this.checkBoxUseToolBar);
            this.groupBox9.Font = null;
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // groupBox8
            // 
            this.groupBox8.AccessibleDescription = null;
            this.groupBox8.AccessibleName = null;
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.BackgroundImage = null;
            this.groupBox8.Controls.Add(this.radioButtonToolbarBtnColor0);
            this.groupBox8.Controls.Add(this.radioButtonToolbarBtnColor1);
            this.groupBox8.Font = null;
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // radioButtonToolbarBtnColor0
            // 
            this.radioButtonToolbarBtnColor0.AccessibleDescription = null;
            this.radioButtonToolbarBtnColor0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonToolbarBtnColor0, "radioButtonToolbarBtnColor0");
            this.radioButtonToolbarBtnColor0.BackgroundImage = null;
            this.radioButtonToolbarBtnColor0.Font = null;
            this.radioButtonToolbarBtnColor0.Name = "radioButtonToolbarBtnColor0";
            this.radioButtonToolbarBtnColor0.TabStop = true;
            this.radioButtonToolbarBtnColor0.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarBtnColor1
            // 
            this.radioButtonToolbarBtnColor1.AccessibleDescription = null;
            this.radioButtonToolbarBtnColor1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonToolbarBtnColor1, "radioButtonToolbarBtnColor1");
            this.radioButtonToolbarBtnColor1.BackgroundImage = null;
            this.radioButtonToolbarBtnColor1.Font = null;
            this.radioButtonToolbarBtnColor1.Name = "radioButtonToolbarBtnColor1";
            this.radioButtonToolbarBtnColor1.TabStop = true;
            this.radioButtonToolbarBtnColor1.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // groupBox10
            // 
            this.groupBox10.AccessibleDescription = null;
            this.groupBox10.AccessibleName = null;
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.BackgroundImage = null;
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos3);
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos2);
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos1);
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos0);
            this.groupBox10.Font = null;
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // radioButtonToolbarPos3
            // 
            this.radioButtonToolbarPos3.AccessibleDescription = null;
            this.radioButtonToolbarPos3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonToolbarPos3, "radioButtonToolbarPos3");
            this.radioButtonToolbarPos3.BackgroundImage = null;
            this.radioButtonToolbarPos3.Font = null;
            this.radioButtonToolbarPos3.Name = "radioButtonToolbarPos3";
            this.radioButtonToolbarPos3.TabStop = true;
            this.radioButtonToolbarPos3.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarPos2
            // 
            this.radioButtonToolbarPos2.AccessibleDescription = null;
            this.radioButtonToolbarPos2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonToolbarPos2, "radioButtonToolbarPos2");
            this.radioButtonToolbarPos2.BackgroundImage = null;
            this.radioButtonToolbarPos2.Font = null;
            this.radioButtonToolbarPos2.Name = "radioButtonToolbarPos2";
            this.radioButtonToolbarPos2.TabStop = true;
            this.radioButtonToolbarPos2.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarPos1
            // 
            this.radioButtonToolbarPos1.AccessibleDescription = null;
            this.radioButtonToolbarPos1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonToolbarPos1, "radioButtonToolbarPos1");
            this.radioButtonToolbarPos1.BackgroundImage = null;
            this.radioButtonToolbarPos1.Font = null;
            this.radioButtonToolbarPos1.Name = "radioButtonToolbarPos1";
            this.radioButtonToolbarPos1.TabStop = true;
            this.radioButtonToolbarPos1.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarPos0
            // 
            this.radioButtonToolbarPos0.AccessibleDescription = null;
            this.radioButtonToolbarPos0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonToolbarPos0, "radioButtonToolbarPos0");
            this.radioButtonToolbarPos0.BackgroundImage = null;
            this.radioButtonToolbarPos0.Font = null;
            this.radioButtonToolbarPos0.Name = "radioButtonToolbarPos0";
            this.radioButtonToolbarPos0.TabStop = true;
            this.radioButtonToolbarPos0.UseVisualStyleBackColor = true;
            // 
            // checkBoxHideDataRange
            // 
            this.checkBoxHideDataRange.AccessibleDescription = null;
            this.checkBoxHideDataRange.AccessibleName = null;
            resources.ApplyResources(this.checkBoxHideDataRange, "checkBoxHideDataRange");
            this.checkBoxHideDataRange.BackgroundImage = null;
            this.checkBoxHideDataRange.Font = null;
            this.checkBoxHideDataRange.Name = "checkBoxHideDataRange";
            this.checkBoxHideDataRange.UseVisualStyleBackColor = true;
            // 
            // numericUpDownToolbarTextSize
            // 
            this.numericUpDownToolbarTextSize.AccessibleDescription = null;
            this.numericUpDownToolbarTextSize.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownToolbarTextSize, "numericUpDownToolbarTextSize");
            this.numericUpDownToolbarTextSize.Font = null;
            this.numericUpDownToolbarTextSize.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownToolbarTextSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToolbarTextSize.Name = "numericUpDownToolbarTextSize";
            this.numericUpDownToolbarTextSize.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // numericUpDownToolbarButtonSize
            // 
            this.numericUpDownToolbarButtonSize.AccessibleDescription = null;
            this.numericUpDownToolbarButtonSize.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownToolbarButtonSize, "numericUpDownToolbarButtonSize");
            this.numericUpDownToolbarButtonSize.Font = null;
            this.numericUpDownToolbarButtonSize.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownToolbarButtonSize.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numericUpDownToolbarButtonSize.Name = "numericUpDownToolbarButtonSize";
            this.numericUpDownToolbarButtonSize.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // checkBoxUseToolBar
            // 
            this.checkBoxUseToolBar.AccessibleDescription = null;
            this.checkBoxUseToolBar.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseToolBar, "checkBoxUseToolBar");
            this.checkBoxUseToolBar.BackgroundImage = null;
            this.checkBoxUseToolBar.Font = null;
            this.checkBoxUseToolBar.Name = "checkBoxUseToolBar";
            this.checkBoxUseToolBar.UseVisualStyleBackColor = true;
            // 
            // PropertyPageObjectMultiTrend
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.checkBoxDontUseConfigurationAutoRange);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxZoomWithYAxis);
            this.Controls.Add(this.checkBoxDontUseConfigurationDialog);
            this.Controls.Add(this.checkBoxUseMouseButtonAsZoom);
            this.Controls.Add(this.buttonEventCellClick);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Icon = null;
            this.Name = "PropertyPageObjectMultiTrend";
            this.Load += new System.EventHandler(this.PropertyPageObjectDatabaseTrend_Load);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeDevide)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideDevide)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataCycleUnit)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLogarithmicScaleBase)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarTextSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarButtonSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void PropertyPageObjectDatabaseTrend_Load(object sender, System.EventArgs e)
		{
			//listDsn.ConnectionStringLoad();
			//DbTool.FillComboBox(this.comboBoxDsn, listDsn); 

            EnableLogarithmicScale();
		}

		private void radioButtonDataCycle1_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		public ObjectArgsMultiTrend ObjectArgs 
		{
			set 
			{
				this.radioButtonDataCycle0.Checked = (value.wTimeSelectOption == 0);
				this.radioButtonDataCycle1.Checked = (value.wTimeSelectOption == 1);
				this.radioButtonDataCycle2.Checked = (value.wTimeSelectOption == 2);
				this.radioButtonDataCycle3.Checked = (value.wTimeSelectOption == 3);
				

				this.numericUpDownShowUnit.Value = value.wShowUnit;
				//this.numericUpDownDataTime.Value = value.nDataTime;
				this.numericUpDownTimeDevide.Value = value.wTimeDevide;
				this.numericUpDownGuideDevide.Value = value.wLevelDevide;
                this.numericUpDownDataCycleUnit.Value = value.nDataCycle;

                scriptEventAfterSettings = value.scriptEventAfterSettings;

                this.checkBoxUseMouseButtonAsZoom.Checked = value.bUseMouseButtonAsZoom;
                this.checkBoxZoomWithYAxis.Checked = value.bUseMouseButtonAsZoomWithY;
                this.checkBoxDontUseConfigurationDialog.Checked = value.bDontUseConfigDialog;
                this.checkBoxDontUseConfigurationAutoRange.Checked = value.bDontUseConfigAutoRange;

				//this.numericUpDownradioButtonDataCycleUnit.Value = value.nDataCycle;

				//this.comboBoxDsn.Text = value.sDsn;
				//this.textBoxTable.Text = value.sTable;
				//this.textBoxTimeColumn.Text = value.sColumnTime;
				
				//this.textBoxMilliSecColumn.Text = value.sColumnMilli;

				//this.radioButtonDateTimeType0.Checked = (value.nDateColumnType == 0);
				//this.radioButtonDateTimeType1.Checked = (value.nDateColumnType == 1);

				//this.numericUpDownBasicSpaceLeft.Value = value.nBasicSpaceLeft;
				//this.numericUpDownBasicSpaceRight.Value = value.nBasicSpaceRight;

				//this.checkBoxRightToLeft.Checked = (value.bTimeDirToLeft == 1);
				//this.checkBoxDisplayRealTime.Checked = (value.bDisplayByTime == 1);

                this.checkBoxLogarithmicScaleUse.Checked = value.logarithmicScale.bUse;
                this.numericUpDownLogarithmicScaleBase.Value = Convert.ToDecimal(value.logarithmicScale.fBase);

                this.checkBoxUseToolBar.Checked = value.bUseToolBar; //20250306 PSU 추가.
                this.radioButtonToolbarPos0.Checked = (value.nToolBarPos == 0);
                this.radioButtonToolbarPos1.Checked = (value.nToolBarPos == 1);
                this.radioButtonToolbarPos2.Checked = (value.nToolBarPos == 2);
                this.radioButtonToolbarPos3.Checked = (value.nToolBarPos == 3);
                this.radioButtonToolbarBtnColor0.Checked = (value.nTooolBarBtnColor == 0);
                this.radioButtonToolbarBtnColor1.Checked = (value.nTooolBarBtnColor == 1);
                this.checkBoxHideDataRange.Checked = value.bHideLabelDataRange;

                if (value.nToolBarButtonSize < 16) this.numericUpDownToolbarButtonSize.Value = 16; //20250317 PSU 추가.
                else this.numericUpDownToolbarButtonSize.Value = value.nToolBarButtonSize;

                this.numericUpDownToolbarTextSize.Value = value.nToolBarTextSize;

                EnableDisableZoomWithYAxis();
			}
			get 
			{
				ObjectArgsMultiTrend args = new ObjectArgsMultiTrend();

				if(this.radioButtonDataCycle0.Checked)	args.wTimeSelectOption = 0;
				else if(this.radioButtonDataCycle1.Checked)	args.wTimeSelectOption = 1;
				else if(this.radioButtonDataCycle2.Checked)	args.wTimeSelectOption = 2;
				else if(this.radioButtonDataCycle3.Checked)	args.wTimeSelectOption = 3;
				else										args.wTimeSelectOption = 0;

				args.wShowUnit = ConvertTool.ToInt32(this.numericUpDownShowUnit.Value);
				//args.nDataTime = ConvertTool.ToInt32(this.numericUpDownDataTime.Value);
				args.wTimeDevide = ConvertTool.ToInt32(this.numericUpDownTimeDevide.Value);
				args.wLevelDevide = ConvertTool.ToInt32(this.numericUpDownGuideDevide.Value);
                args.nDataCycle = ConvertTool.ToInt32(this.numericUpDownDataCycleUnit.Value);

                args.scriptEventAfterSettings = scriptEventAfterSettings;

                args.bUseMouseButtonAsZoom= this.checkBoxUseMouseButtonAsZoom.Checked;
                args.bUseMouseButtonAsZoomWithY = this.checkBoxZoomWithYAxis.Checked;
                args.bDontUseConfigDialog = this.checkBoxDontUseConfigurationDialog.Checked;
                args.bDontUseConfigAutoRange = this.checkBoxDontUseConfigurationAutoRange.Checked;

				//args.bDisplayByTime = this.checkBoxDisplayRealTime.Checked ? (sbyte)1 : (sbyte)0;
				//args.bTimeDirToLeft = this.checkBoxRightToLeft.Checked ? (sbyte)1 : (sbyte)0;

				
				//args.nDataCycle = ConvertTool.ToInt32(this.numericUpDownradioButtonDataCycleUnit.Value);

				//args.sDsn = this.comboBoxDsn.Text;
				//args.sTable = this.textBoxTable.Text;
				//args.sColumnTime = this.textBoxTimeColumn.Text;
				//args.sColumnMilli = this.textBoxMilliSecColumn.Text;

				//if(this.radioButtonDateTimeType0.Checked)		args.nDateColumnType = 0;
				//else if(this.radioButtonDateTimeType1.Checked)	args.nDateColumnType = 1;
				//else											args.nDateColumnType = 0;

				//args.nBasicSpaceLeft = ConvertTool.ToInt32(this.numericUpDownBasicSpaceLeft.Value);
				//args.nBasicSpaceRight = ConvertTool.ToInt32(this.numericUpDownBasicSpaceRight.Value);

                args.logarithmicScale.bUse = this.checkBoxLogarithmicScaleUse.Checked;
                args.logarithmicScale.fBase = Convert.ToDouble(this.numericUpDownLogarithmicScaleBase.Value);

                args.bUseToolBar = this.checkBoxUseToolBar.Checked;  //20250306 PSU 추가
                if (this.radioButtonToolbarPos0.Checked) args.nToolBarPos = 0;
                else if (this.radioButtonToolbarPos1.Checked) args.nToolBarPos = 1;
                else if (this.radioButtonToolbarPos2.Checked) args.nToolBarPos = 2;
                else if (this.radioButtonToolbarPos3.Checked) args.nToolBarPos = 3;
                if (this.radioButtonToolbarBtnColor0.Checked) args.nTooolBarBtnColor = 0;
                else if (this.radioButtonToolbarBtnColor1.Checked) args.nTooolBarBtnColor = 1;
                args.bHideLabelDataRange = this.checkBoxHideDataRange.Checked;

                args.nToolBarButtonSize = ConvertTool.ToInt32(this.numericUpDownToolbarButtonSize.Value);
                args.nToolBarTextSize = ConvertTool.ToInt32(this.numericUpDownToolbarTextSize.Value);

				return args;
			}
		}

        private void buttonEventCellClick_Click(object sender, EventArgs e)
        {
            FormScriptEditor dialog = new FormScriptEditor();

            if (NetTools.Tools.IsLangKorean())
                dialog.SetScript("'설정창 확인 버튼 누른 후' 스크립트 편집", scriptEventAfterSettings);
            else
                dialog.SetScript("EventAfterSettings Script", scriptEventAfterSettings);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptEventAfterSettings = dialog.GetScript();
            }
        }

        void EnableDisableZoomWithYAxis()
        {
            this.checkBoxZoomWithYAxis.Enabled = this.checkBoxUseMouseButtonAsZoom.Checked;
        }

        private void checkBoxUseMouseButtonAsZoom_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableZoomWithYAxis();
        }

        void EnableLogarithmicScale()
        {
            bool flag = this.checkBoxLogarithmicScaleUse.Checked;

            this.numericUpDownLogarithmicScaleBase.Enabled = flag;
        }

        private void checkBoxLogarithmicScaleUse_CheckedChanged(object sender, EventArgs e)
        {
            EnableLogarithmicScale();
        }
	}
}
