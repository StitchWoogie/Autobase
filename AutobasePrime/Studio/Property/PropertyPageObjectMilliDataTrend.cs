using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using DatabaseConnection;
using AutoLibLocal;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectDatabaseTrend.
	/// </summary>
	public class PropertyPageObjectMilliDataTrend : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonDsn;
		private System.Windows.Forms.RadioButton radioButtonDataCycle0;
		private System.Windows.Forms.RadioButton radioButtonDataCycle1;
		private System.Windows.Forms.RadioButton radioButtonDataCycle2;
		private System.Windows.Forms.RadioButton radioButtonDataCycle3;
		private System.Windows.Forms.RadioButton radioButtonDataCycle4;
		private System.Windows.Forms.RadioButton radioButtonDataCycle5;
		private System.Windows.Forms.NumericUpDown numericUpDownradioButtonDataCycleUnit;
        private System.Windows.Forms.NumericUpDown numericUpDownShowUnit;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownTimeDevide;
		private System.Windows.Forms.ComboBox comboBoxDsn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private System.Windows.Forms.RadioButton radioButtonDataCycle6;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown numericUpDownBasicSpaceLeft;
		private System.Windows.Forms.NumericUpDown numericUpDownBasicSpaceRight;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.NumericUpDown numericUpDownGuideDevide;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
        private CheckBox checkBoxAutoUpdate;
        private GroupBox groupBox4;
        private NumericUpDown numericUpDownLogarithmicScaleBase;
        private Label label2;
        private CheckBox checkBoxLogarithmicScaleUse;
        private CheckBox checkBoxDontUseConfigurationDialog;
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

		ConnectionStringList listDsn = new ConnectionStringList();
		
		public PropertyPageObjectMilliDataTrend()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectMilliDataTrend));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxDsn = new System.Windows.Forms.ComboBox();
            this.buttonDsn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonDataCycle6 = new System.Windows.Forms.RadioButton();
            this.numericUpDownradioButtonDataCycleUnit = new System.Windows.Forms.NumericUpDown();
            this.radioButtonDataCycle5 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle4 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle3 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDataCycle0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownShowUnit = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownTimeDevide = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownBasicSpaceLeft = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownBasicSpaceRight = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericUpDownGuideDevide = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.checkBoxAutoUpdate = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numericUpDownLogarithmicScaleBase = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxLogarithmicScaleUse = new System.Windows.Forms.CheckBox();
            this.checkBoxDontUseConfigurationDialog = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownToolbarTextSize = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownToolbarButtonSize = new System.Windows.Forms.NumericUpDown();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.radioButtonToolbarBtnColor0 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarBtnColor1 = new System.Windows.Forms.RadioButton();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.radioButtonToolbarPos3 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarPos2 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarPos1 = new System.Windows.Forms.RadioButton();
            this.radioButtonToolbarPos0 = new System.Windows.Forms.RadioButton();
            this.checkBoxHideDataRange = new System.Windows.Forms.CheckBox();
            this.checkBoxUseToolBar = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownradioButtonDataCycleUnit)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeDevide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBasicSpaceLeft)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBasicSpaceRight)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideDevide)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLogarithmicScaleBase)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarTextSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarButtonSize)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.comboBoxDsn);
            this.groupBox1.Controls.Add(this.buttonDsn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // comboBoxDsn
            // 
            resources.ApplyResources(this.comboBoxDsn, "comboBoxDsn");
            this.comboBoxDsn.Name = "comboBoxDsn";
            this.comboBoxDsn.SelectedIndexChanged += new System.EventHandler(this.comboBoxDsn_SelectedIndexChanged);
            // 
            // buttonDsn
            // 
            resources.ApplyResources(this.buttonDsn, "buttonDsn");
            this.buttonDsn.Name = "buttonDsn";
            this.buttonDsn.Click += new System.EventHandler(this.buttonDsn_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.radioButtonDataCycle6);
            this.groupBox2.Controls.Add(this.numericUpDownradioButtonDataCycleUnit);
            this.groupBox2.Controls.Add(this.radioButtonDataCycle5);
            this.groupBox2.Controls.Add(this.radioButtonDataCycle4);
            this.groupBox2.Controls.Add(this.radioButtonDataCycle3);
            this.groupBox2.Controls.Add(this.radioButtonDataCycle2);
            this.groupBox2.Controls.Add(this.radioButtonDataCycle1);
            this.groupBox2.Controls.Add(this.radioButtonDataCycle0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonDataCycle6
            // 
            resources.ApplyResources(this.radioButtonDataCycle6, "radioButtonDataCycle6");
            this.radioButtonDataCycle6.Name = "radioButtonDataCycle6";
            // 
            // numericUpDownradioButtonDataCycleUnit
            // 
            resources.ApplyResources(this.numericUpDownradioButtonDataCycleUnit, "numericUpDownradioButtonDataCycleUnit");
            this.numericUpDownradioButtonDataCycleUnit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownradioButtonDataCycleUnit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownradioButtonDataCycleUnit.Name = "numericUpDownradioButtonDataCycleUnit";
            this.numericUpDownradioButtonDataCycleUnit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonDataCycle5
            // 
            resources.ApplyResources(this.radioButtonDataCycle5, "radioButtonDataCycle5");
            this.radioButtonDataCycle5.Name = "radioButtonDataCycle5";
            // 
            // radioButtonDataCycle4
            // 
            resources.ApplyResources(this.radioButtonDataCycle4, "radioButtonDataCycle4");
            this.radioButtonDataCycle4.Name = "radioButtonDataCycle4";
            // 
            // radioButtonDataCycle3
            // 
            resources.ApplyResources(this.radioButtonDataCycle3, "radioButtonDataCycle3");
            this.radioButtonDataCycle3.Name = "radioButtonDataCycle3";
            this.radioButtonDataCycle3.CheckedChanged += new System.EventHandler(this.radioButtonDataCycle3_CheckedChanged);
            // 
            // radioButtonDataCycle2
            // 
            resources.ApplyResources(this.radioButtonDataCycle2, "radioButtonDataCycle2");
            this.radioButtonDataCycle2.Name = "radioButtonDataCycle2";
            // 
            // radioButtonDataCycle1
            // 
            resources.ApplyResources(this.radioButtonDataCycle1, "radioButtonDataCycle1");
            this.radioButtonDataCycle1.Name = "radioButtonDataCycle1";
            // 
            // radioButtonDataCycle0
            // 
            resources.ApplyResources(this.radioButtonDataCycle0, "radioButtonDataCycle0");
            this.radioButtonDataCycle0.Name = "radioButtonDataCycle0";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownShowUnit);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownShowUnit
            // 
            resources.ApplyResources(this.numericUpDownShowUnit, "numericUpDownShowUnit");
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
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericUpDownTimeDevide
            // 
            resources.ApplyResources(this.numericUpDownTimeDevide, "numericUpDownTimeDevide");
            this.numericUpDownTimeDevide.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTimeDevide.Name = "numericUpDownTimeDevide";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // numericUpDownBasicSpaceLeft
            // 
            resources.ApplyResources(this.numericUpDownBasicSpaceLeft, "numericUpDownBasicSpaceLeft");
            this.numericUpDownBasicSpaceLeft.Name = "numericUpDownBasicSpaceLeft";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.numericUpDownBasicSpaceRight);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.numericUpDownBasicSpaceLeft);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // numericUpDownBasicSpaceRight
            // 
            resources.ApplyResources(this.numericUpDownBasicSpaceRight, "numericUpDownBasicSpaceRight");
            this.numericUpDownBasicSpaceRight.Name = "numericUpDownBasicSpaceRight";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.numericUpDownGuideDevide);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.numericUpDownTimeDevide);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // numericUpDownGuideDevide
            // 
            resources.ApplyResources(this.numericUpDownGuideDevide, "numericUpDownGuideDevide");
            this.numericUpDownGuideDevide.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownGuideDevide.Name = "numericUpDownGuideDevide";
            this.numericUpDownGuideDevide.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // checkBoxAutoUpdate
            // 
            resources.ApplyResources(this.checkBoxAutoUpdate, "checkBoxAutoUpdate");
            this.checkBoxAutoUpdate.Name = "checkBoxAutoUpdate";
            this.checkBoxAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.numericUpDownLogarithmicScaleBase);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.checkBoxLogarithmicScaleUse);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // numericUpDownLogarithmicScaleBase
            // 
            resources.ApplyResources(this.numericUpDownLogarithmicScaleBase, "numericUpDownLogarithmicScaleBase");
            this.numericUpDownLogarithmicScaleBase.DecimalPlaces = 1;
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
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // checkBoxLogarithmicScaleUse
            // 
            resources.ApplyResources(this.checkBoxLogarithmicScaleUse, "checkBoxLogarithmicScaleUse");
            this.checkBoxLogarithmicScaleUse.Name = "checkBoxLogarithmicScaleUse";
            this.checkBoxLogarithmicScaleUse.UseVisualStyleBackColor = true;
            this.checkBoxLogarithmicScaleUse.CheckedChanged += new System.EventHandler(this.checkBoxLogarithmicScaleUse_CheckedChanged);
            // 
            // checkBoxDontUseConfigurationDialog
            // 
            resources.ApplyResources(this.checkBoxDontUseConfigurationDialog, "checkBoxDontUseConfigurationDialog");
            this.checkBoxDontUseConfigurationDialog.Name = "checkBoxDontUseConfigurationDialog";
            this.checkBoxDontUseConfigurationDialog.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.label8);
            this.groupBox9.Controls.Add(this.label5);
            this.groupBox9.Controls.Add(this.numericUpDownToolbarTextSize);
            this.groupBox9.Controls.Add(this.numericUpDownToolbarButtonSize);
            this.groupBox9.Controls.Add(this.groupBox8);
            this.groupBox9.Controls.Add(this.groupBox10);
            this.groupBox9.Controls.Add(this.checkBoxHideDataRange);
            this.groupBox9.Controls.Add(this.checkBoxUseToolBar);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownToolbarTextSize
            // 
            resources.ApplyResources(this.numericUpDownToolbarTextSize, "numericUpDownToolbarTextSize");
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
            resources.ApplyResources(this.numericUpDownToolbarButtonSize, "numericUpDownToolbarButtonSize");
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
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.radioButtonToolbarBtnColor0);
            this.groupBox8.Controls.Add(this.radioButtonToolbarBtnColor1);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // radioButtonToolbarBtnColor0
            // 
            resources.ApplyResources(this.radioButtonToolbarBtnColor0, "radioButtonToolbarBtnColor0");
            this.radioButtonToolbarBtnColor0.Name = "radioButtonToolbarBtnColor0";
            this.radioButtonToolbarBtnColor0.TabStop = true;
            this.radioButtonToolbarBtnColor0.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarBtnColor1
            // 
            resources.ApplyResources(this.radioButtonToolbarBtnColor1, "radioButtonToolbarBtnColor1");
            this.radioButtonToolbarBtnColor1.Name = "radioButtonToolbarBtnColor1";
            this.radioButtonToolbarBtnColor1.TabStop = true;
            this.radioButtonToolbarBtnColor1.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos3);
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos2);
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos1);
            this.groupBox10.Controls.Add(this.radioButtonToolbarPos0);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // radioButtonToolbarPos3
            // 
            resources.ApplyResources(this.radioButtonToolbarPos3, "radioButtonToolbarPos3");
            this.radioButtonToolbarPos3.Name = "radioButtonToolbarPos3";
            this.radioButtonToolbarPos3.TabStop = true;
            this.radioButtonToolbarPos3.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarPos2
            // 
            resources.ApplyResources(this.radioButtonToolbarPos2, "radioButtonToolbarPos2");
            this.radioButtonToolbarPos2.Name = "radioButtonToolbarPos2";
            this.radioButtonToolbarPos2.TabStop = true;
            this.radioButtonToolbarPos2.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarPos1
            // 
            resources.ApplyResources(this.radioButtonToolbarPos1, "radioButtonToolbarPos1");
            this.radioButtonToolbarPos1.Name = "radioButtonToolbarPos1";
            this.radioButtonToolbarPos1.TabStop = true;
            this.radioButtonToolbarPos1.UseVisualStyleBackColor = true;
            // 
            // radioButtonToolbarPos0
            // 
            resources.ApplyResources(this.radioButtonToolbarPos0, "radioButtonToolbarPos0");
            this.radioButtonToolbarPos0.Name = "radioButtonToolbarPos0";
            this.radioButtonToolbarPos0.TabStop = true;
            this.radioButtonToolbarPos0.UseVisualStyleBackColor = true;
            // 
            // checkBoxHideDataRange
            // 
            resources.ApplyResources(this.checkBoxHideDataRange, "checkBoxHideDataRange");
            this.checkBoxHideDataRange.Name = "checkBoxHideDataRange";
            this.checkBoxHideDataRange.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseToolBar
            // 
            resources.ApplyResources(this.checkBoxUseToolBar, "checkBoxUseToolBar");
            this.checkBoxUseToolBar.Name = "checkBoxUseToolBar";
            this.checkBoxUseToolBar.UseVisualStyleBackColor = true;
            // 
            // PropertyPageObjectMilliDataTrend
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.checkBoxDontUseConfigurationDialog);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxAutoUpdate);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectMilliDataTrend";
            this.Load += new System.EventHandler(this.PropertyPageObjectDatabaseTrend_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownradioButtonDataCycleUnit)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeDevide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBasicSpaceLeft)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBasicSpaceRight)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideDevide)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLogarithmicScaleBase)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarTextSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToolbarButtonSize)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonDsn_Click(object sender, System.EventArgs e)
		{
            if (FormConfigMilliData.ConfigMilliData() == true)
            {
                FillCombo();
            }
		}

        void FillCombo()
        {
            ArrayList block = new ArrayList();
            MilliData.LoadMilliData(block);
            MILLI_DATA_STRUCT item;

            this.comboBoxDsn.Items.Clear();

            for (int i = 0; i < block.Count; i++)
            {
                item = (MILLI_DATA_STRUCT)block[i];
                this.comboBoxDsn.Items.Add(item.title);
            }
        }

		private void PropertyPageObjectDatabaseTrend_Load(object sender, System.EventArgs e)
		{
            FillCombo();

            EnableLogarithmicScale();
		}

		private void radioButtonDataCycle3_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		private void label4_Click(object sender, System.EventArgs e)
		{
		
		}

		public ObjectArgsMilliDataTrend ObjectArgs 
		{
			set 
			{
				this.radioButtonDataCycle0.Checked = (value.wTimeSelectOption == 0);
				this.radioButtonDataCycle1.Checked = (value.wTimeSelectOption == 1);
				this.radioButtonDataCycle2.Checked = (value.wTimeSelectOption == 2);
				this.radioButtonDataCycle3.Checked = (value.wTimeSelectOption == 3);
				this.radioButtonDataCycle4.Checked = (value.wTimeSelectOption == 4);
				this.radioButtonDataCycle5.Checked = (value.wTimeSelectOption == 5);
				this.radioButtonDataCycle6.Checked = (value.wTimeSelectOption == 6);

				this.numericUpDownShowUnit.Value = value.wShowUnit;
				this.numericUpDownTimeDevide.Value = value.wTimeDevide;
				this.numericUpDownGuideDevide.Value = value.wLevelDevide;
				this.numericUpDownradioButtonDataCycleUnit.Value = value.nDataCycle;

				this.comboBoxDsn.Text = value.sDsn;
				//this.textBoxTable.Text = value.sTable;
				//this.textBoxTimeColumn.Text = value.sColumnTime;
				
				//this.textBoxMilliSecColumn.Text = value.sColumnMilli;

				//this.radioButtonDateTimeType0.Checked = (value.nDateColumnType == 0);
				//this.radioButtonDateTimeType1.Checked = (value.nDateColumnType == 1);

				this.numericUpDownBasicSpaceLeft.Value = value.nBasicSpaceLeft;
				this.numericUpDownBasicSpaceRight.Value = value.nBasicSpaceRight;
                this.checkBoxAutoUpdate.Checked = value.bAutoUpdate;

                this.checkBoxLogarithmicScaleUse.Checked = value.logarithmicScale.bUse;
                this.numericUpDownLogarithmicScaleBase.Value = Convert.ToDecimal(value.logarithmicScale.fBase);

                this.checkBoxDontUseConfigurationDialog.Checked = value.bDontUseConfigDialog;

                this.checkBoxUseToolBar.Checked = value.bUseToolBar;  //20250306 PSU 추가.
                this.radioButtonToolbarPos0.Checked = (value.nToolBarPos == 0);
                this.radioButtonToolbarPos1.Checked = (value.nToolBarPos == 1);
                this.radioButtonToolbarPos2.Checked = (value.nToolBarPos == 2);
                this.radioButtonToolbarPos3.Checked = (value.nToolBarPos == 3);
                this.radioButtonToolbarBtnColor0.Checked = (value.nTooolBarBtnColor == 0);
                this.radioButtonToolbarBtnColor1.Checked = (value.nTooolBarBtnColor == 1);
                this.checkBoxHideDataRange.Checked = value.bHideLabelDataRange;

                this.checkBoxHideDataRange.Checked = value.bHideLabelDataRange;

                if (value.nToolBarButtonSize < 16) this.numericUpDownToolbarButtonSize.Value = 16; //20250317 PSU 추가.
                else this.numericUpDownToolbarButtonSize.Value = value.nToolBarButtonSize;

                this.numericUpDownToolbarTextSize.Value = value.nToolBarTextSize;

			}
			get 
			{
                ObjectArgsMilliDataTrend args = new ObjectArgsMilliDataTrend();

				if(this.radioButtonDataCycle0.Checked)	args.wTimeSelectOption = 0;
				else if(this.radioButtonDataCycle1.Checked)	args.wTimeSelectOption = 1;
				else if(this.radioButtonDataCycle2.Checked)	args.wTimeSelectOption = 2;
				else if(this.radioButtonDataCycle3.Checked)	args.wTimeSelectOption = 3;
				else if(this.radioButtonDataCycle4.Checked)	args.wTimeSelectOption = 4;
				else if(this.radioButtonDataCycle5.Checked)	args.wTimeSelectOption = 5;
				else if(this.radioButtonDataCycle6.Checked)	args.wTimeSelectOption = 6;
				else										args.wTimeSelectOption = 2;

				args.wShowUnit = ConvertTool.ToInt32(this.numericUpDownShowUnit.Value);
				args.wTimeDevide = ConvertTool.ToInt32(this.numericUpDownTimeDevide.Value);
				args.wLevelDevide = ConvertTool.ToInt32(this.numericUpDownGuideDevide.Value);
				args.nDataCycle = ConvertTool.ToInt32(this.numericUpDownradioButtonDataCycleUnit.Value);

				args.sDsn = this.comboBoxDsn.Text;
				//args.sTable = this.textBoxTable.Text;
				//args.sColumnTime = this.textBoxTimeColumn.Text;
				//args.sColumnMilli = this.textBoxMilliSecColumn.Text;

				//if(this.radioButtonDateTimeType0.Checked)		args.nDateColumnType = 0;
				//else if(this.radioButtonDateTimeType1.Checked)	args.nDateColumnType = 1;
				//else											args.nDateColumnType = 0;

				args.nBasicSpaceLeft = ConvertTool.ToInt32(this.numericUpDownBasicSpaceLeft.Value);
				args.nBasicSpaceRight = ConvertTool.ToInt32(this.numericUpDownBasicSpaceRight.Value);
                args.bAutoUpdate = this.checkBoxAutoUpdate.Checked;

                args.logarithmicScale.bUse = this.checkBoxLogarithmicScaleUse.Checked;
                args.logarithmicScale.fBase = Convert.ToDouble(this.numericUpDownLogarithmicScaleBase.Value);

                args.bDontUseConfigDialog = this.checkBoxDontUseConfigurationDialog.Checked;

                args.bUseToolBar = this.checkBoxUseToolBar.Checked;   //20250306 PSU 추가
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

        private void comboBoxDsn_SelectedIndexChanged(object sender, EventArgs e)
        {

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
