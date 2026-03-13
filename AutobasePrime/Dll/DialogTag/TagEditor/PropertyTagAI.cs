using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using System.IO;

namespace DialogTag.TagEditor
{
	/// <summary>
	/// Summary description for FormTagProperty.
	/// </summary>
	public class PropertyTagAI : System.Windows.Forms.Form
	{ 
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.GroupBox groupBox10;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.GroupBox groupBox14;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.GroupBox groupBox17;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.GroupBox groupBox15;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox16;
		private System.Windows.Forms.GroupBox groupBox18;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox19;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType0;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType1;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType2;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType3;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType4;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType5;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType6;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType7;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType8;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType9;
		private System.Windows.Forms.RadioButton radioButtonAiMemoryType10;
		private System.Windows.Forms.CheckBox checkBoxAiBcd;
        private System.Windows.Forms.ComboBox comboBoxAiAlarmCondition;
		private System.Windows.Forms.TextBox textBoxAiDisplayFormat;
		private System.Windows.Forms.Button buttonAiSubOutAO;
		private System.Windows.Forms.TextBox textBoxAiAddress;
		private System.Windows.Forms.TextBox textBoxAiPort;
		private System.Windows.Forms.Button buttonAiSubLoLoDO;
		private System.Windows.Forms.TextBox textBoxAiAlarmWaveFile;
		private System.Windows.Forms.TextBox textBoxAiLow;
		private System.Windows.Forms.Button buttonAiSubOutSV;
		private System.Windows.Forms.TextBox textBoxAiLoLo;
		private System.Windows.Forms.Button buttonAiSubHiHiDO;
		private System.Windows.Forms.TextBox textBoxAiSubOutSV;
		private System.Windows.Forms.TextBox textBoxAiSubOutAO;
		private System.Windows.Forms.TextBox textBoxAiSubLoLoDO;
		private System.Windows.Forms.TextBox textBoxAiSubHiHiDO;
		private System.Windows.Forms.TextBox textBoxAiHigh;
		private System.Windows.Forms.TextBox textBoxAiHiHi;
		private MyNumericUpDown numericUpDownAiAccumulation;
        private MyNumericUpDown numericUpDownAiConfirmCount;
		private System.Windows.Forms.RadioButton radioButtonAiFilter4;
		private System.Windows.Forms.RadioButton radioButtonAiFilter3;
		private System.Windows.Forms.RadioButton radioButtonAiFilter2;
		private System.Windows.Forms.RadioButton radioButtonAiFilter1;
		private System.Windows.Forms.RadioButton radioButtonAiFilter0;
		private MyNumericUpDown numericUpDownAiAlarmPriority;
		private MyNumericUpDown numericUpDownAiScanTime;
		private System.Windows.Forms.RadioButton radioButtonAiOverflowCut1;
		private System.Windows.Forms.RadioButton radioButtonAiOverflowCut0;
		private System.Windows.Forms.TextBox textBoxAiPlcBase;
		private System.Windows.Forms.TextBox textBoxAiPlcFull;
		private System.Windows.Forms.TextBox textBoxAiBase;
		private System.Windows.Forms.TextBox textBoxAiFull;
		private System.Windows.Forms.Button buttonAiAlarmWaveFile;
		private System.Windows.Forms.Button buttonAiAlarmGraphicFile;
		private System.Windows.Forms.TextBox textBoxAiAlarmGraphicFile;
		private MyNumericUpDown numericUpDownAiOnBigChangeSecond;
		private MyNumericUpDown numericUpDownAiOnBigChangePercent;
		private System.Windows.Forms.TextBox textBoxAiViewBase;
		private System.Windows.Forms.TextBox textBoxAiViewFull;
		private MyNumericUpDown numericUpDownAiChangeAlarm;

		MultiSelectRadioButton multiAiMemoryType = new MultiSelectRadioButton();
		MultiSelectCheckBox multiAiBcd = new MultiSelectCheckBox();
		MultiSelectTextBox multiAiPort = new MultiSelectTextBox();
		MultiSelectTextBox multiAiAddress = new MultiSelectTextBox();
        MultiSelectComboBox multiAiUnit = new MultiSelectComboBox();

		MultiSelectTextBox multiAiFormat = new MultiSelectTextBox();
		MultiSelectTextBox multiAiHiHi = new MultiSelectTextBox();
		MultiSelectTextBox multiAiHigh = new MultiSelectTextBox();
		MultiSelectTextBox multiAiLow = new MultiSelectTextBox();
		MultiSelectTextBox multiAiLoLo = new MultiSelectTextBox();
		MultiSelectComboBox multiAiAlarmCondition = new MultiSelectComboBox();

		MultiSelectTextBox multiAiSubHiHiDo = new MultiSelectTextBox();
		MultiSelectTextBox multiAiSubLoLoDo = new MultiSelectTextBox();
		MultiSelectTextBox multiAiSubOutAo = new MultiSelectTextBox();
		MultiSelectTextBox multiAiSubAoSv = new MultiSelectTextBox();

		MultiSelectTextBox multiAiFull = new MultiSelectTextBox();
		MultiSelectTextBox multiAiBase = new MultiSelectTextBox();
		MultiSelectTextBox multiAiPlcFull = new MultiSelectTextBox();
		MultiSelectTextBox multiAiPlcBase = new MultiSelectTextBox();
		MultiSelectTextBox multiAiViewFull = new MultiSelectTextBox();
		MultiSelectTextBox multiAiViewBase = new MultiSelectTextBox();

		MultiSelectTextBox multiAiAlarmWave = new MultiSelectTextBox();
		MultiSelectTextBox multiAiAlarmGraphic = new MultiSelectTextBox();

		MultiSelectCheckBox multiAiUsingAlarm = new MultiSelectCheckBox();
		MultiSelectCheckBox multiAiDataSave = new MultiSelectCheckBox();

        MultiSelectCheckBox multiAiDataAccumulate = new MultiSelectCheckBox(); //20250225 PSU 누적값

		MultiSelectNumericUpDown multiAiBigChangePercent = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiAiBigChangeSecond = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiAiAlarmPriority = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiAiAccumulation = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiAiConfirmCount = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiAiScanTime = new MultiSelectNumericUpDown();
		MultiSelectTextBox multiAiAlarmRuturnGab = new MultiSelectTextBox();
		MultiSelectNumericUpDown multiAiAlarmChange = new MultiSelectNumericUpDown();

		MultiSelectRadioButton multiAiFilter = new MultiSelectRadioButton();
		private System.Windows.Forms.TextBox textBoxAiAlarmReturnGab;
		MultiSelectRadioButton multiAiCut = new MultiSelectRadioButton();

		MultiSelectTextBox multiAiScrollUnit = new MultiSelectTextBox();

        MultiSelectComboBox multiAiMemorySubType = new MultiSelectComboBox();
        MultiSelectTextBox multiAiCalcScript = new MultiSelectTextBox();

		private System.Windows.Forms.CheckBox checkBoxAiUsingAlarm;
		private System.Windows.Forms.CheckBox checkBoxAiDataSave;
		private System.Windows.Forms.GroupBox groupBoxPlcScan;
		private System.Windows.Forms.GroupBox groupBoxMemoryType;
		private System.Windows.Forms.GroupBox groupBoxCalcFilter;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxAiScrollUnit;
        private Label label1;
        private ComboBox comboBoxMemorySubType;
        private RadioButton radioButtonAiFilter5;
        private GroupBox groupBox2;
        private TextBox textBoxCalcScript;
        private RadioButton radioButtonAiMemoryType12;
        private RadioButton radioButtonAiMemoryType11;
        private RadioButton radioButtonAiOverflowCut2;
        private ComboBox comboBoxAiUnit;
        private CheckBox checkBoxAiAccumulatedData;
        private GroupBox groupBox3;
		public int listViewOwnerPos = -1;

		public PropertyTagAI()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// 태그를 생성시킬때 0값이 항상들어가서 아래를 추가하였다.
			this.numericUpDownAiScanTime.Value = TagLib.DEFAULT_SCANTIME;

			multiAiMemoryType.Add(radioButtonAiMemoryType0, radioButtonAiMemoryType1, radioButtonAiMemoryType2, 
								  radioButtonAiMemoryType3, radioButtonAiMemoryType4, radioButtonAiMemoryType5, 
								  radioButtonAiMemoryType6, radioButtonAiMemoryType7, radioButtonAiMemoryType8, 
								  radioButtonAiMemoryType9, radioButtonAiMemoryType10, radioButtonAiMemoryType11,
                                  radioButtonAiMemoryType12);
			multiAiBcd.Add(this.checkBoxAiBcd);
			multiAiPort.Add(this.textBoxAiPort);
			multiAiAddress.Add(this.textBoxAiAddress);
			multiAiUnit.Add(this.comboBoxAiUnit);
			multiAiFormat.Add(this.textBoxAiDisplayFormat);
			multiAiFormat.nDecimalPointLow = 1;	// 소숫점 1자리만 사용
			multiAiHiHi.Add(this.textBoxAiHiHi);
			multiAiHigh.Add(this.textBoxAiHigh);
			multiAiLow.Add(this.textBoxAiLow);
			multiAiLoLo.Add(this.textBoxAiLoLo);
			multiAiAlarmCondition.Add(this.comboBoxAiAlarmCondition);

			multiAiSubHiHiDo.Add(this.textBoxAiSubHiHiDO);
			multiAiSubLoLoDo.Add(this.textBoxAiSubLoLoDO);
			multiAiSubOutAo.Add(this.textBoxAiSubOutAO);
			multiAiSubAoSv.Add(this.textBoxAiSubOutSV);

            // Full이 소수점 2째까지만 지원되었는데 자동으로 변경하였다. 2019-6-4 변경 Full/Base/PlcFull/PlcBase/ViewFull/ViewBase 같이 변경
			multiAiFull.Add(this.textBoxAiFull);
            multiAiFull.nDecimalPointLow = -1;
			multiAiBase.Add(this.textBoxAiBase);
            multiAiBase.nDecimalPointLow = -1;
			multiAiPlcFull.Add(this.textBoxAiPlcFull);
            multiAiPlcFull.nDecimalPointLow = -1;
			multiAiPlcBase.Add(this.textBoxAiPlcBase);
            multiAiPlcBase.nDecimalPointLow = -1;
			multiAiViewFull.Add(this.textBoxAiViewFull);
            multiAiViewFull.nDecimalPointLow = -1;
			multiAiViewBase.Add(this.textBoxAiViewBase);
            multiAiViewBase.nDecimalPointLow = -1;

			multiAiAlarmWave.Add(this.textBoxAiAlarmWaveFile);
			multiAiAlarmGraphic.Add(this.textBoxAiAlarmGraphicFile);

			multiAiBigChangePercent.Add(this.numericUpDownAiOnBigChangePercent);
			multiAiBigChangeSecond.Add(this.numericUpDownAiOnBigChangeSecond);
			multiAiAlarmPriority.Add(this.numericUpDownAiAlarmPriority);
			multiAiAccumulation.Add(this.numericUpDownAiAccumulation);
			multiAiConfirmCount.Add(this.numericUpDownAiConfirmCount);
			multiAiScanTime.Add(this.numericUpDownAiScanTime);
			multiAiAlarmRuturnGab.Add(this.textBoxAiAlarmReturnGab);
			multiAiAlarmChange.Add(this.numericUpDownAiChangeAlarm);

			multiAiFilter.Add(radioButtonAiFilter0, radioButtonAiFilter1, radioButtonAiFilter2, radioButtonAiFilter3, radioButtonAiFilter4, radioButtonAiFilter5);
			multiAiCut.Add(this.radioButtonAiOverflowCut0, radioButtonAiOverflowCut1, radioButtonAiOverflowCut2);

			multiAiUsingAlarm.Add(this.checkBoxAiUsingAlarm);
			multiAiDataSave.Add(this.checkBoxAiDataSave);

			multiAiScrollUnit.Add(this.textBoxAiScrollUnit);
            multiAiScrollUnit.nDecimalPointLow = -1;

            multiAiMemorySubType.Add(this.comboBoxMemorySubType);

            multiAiCalcScript.Add(this.textBoxCalcScript);

            multiAiDataAccumulate.Add(this.checkBoxAiAccumulatedData);  //20250225 PSU 누적값 추가.


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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagAI));
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxAiAlarmCondition = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.numericUpDownAiAccumulation = new AutoLibLocal.MyNumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.numericUpDownAiConfirmCount = new AutoLibLocal.MyNumericUpDown();
            this.radioButtonAiFilter4 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiFilter3 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiFilter2 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiFilter1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiFilter0 = new System.Windows.Forms.RadioButton();
            this.groupBoxCalcFilter = new System.Windows.Forms.GroupBox();
            this.radioButtonAiFilter5 = new System.Windows.Forms.RadioButton();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.numericUpDownAiAlarmPriority = new AutoLibLocal.MyNumericUpDown();
            this.textBoxAiDisplayFormat = new System.Windows.Forms.TextBox();
            this.radioButtonAiOverflowCut1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiOverflowCut0 = new System.Windows.Forms.RadioButton();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.radioButtonAiOverflowCut2 = new System.Windows.Forms.RadioButton();
            this.buttonAiSubOutAO = new System.Windows.Forms.Button();
            this.textBoxAiAddress = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.textBoxAiPort = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.groupBoxPlcScan = new System.Windows.Forms.GroupBox();
            this.textBoxAiPlcBase = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxAiPlcFull = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxAiBase = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxAiFull = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.buttonAiSubLoLoDO = new System.Windows.Forms.Button();
            this.buttonAiAlarmWaveFile = new System.Windows.Forms.Button();
            this.textBoxAiAlarmWaveFile = new System.Windows.Forms.TextBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.numericUpDownAiScanTime = new AutoLibLocal.MyNumericUpDown();
            this.textBoxAiLow = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonAiSubOutSV = new System.Windows.Forms.Button();
            this.label29 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxAiLoLo = new System.Windows.Forms.TextBox();
            this.buttonAiSubHiHiDO = new System.Windows.Forms.Button();
            this.buttonAiAlarmGraphicFile = new System.Windows.Forms.Button();
            this.textBoxAiAlarmGraphicFile = new System.Windows.Forms.TextBox();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.groupBoxMemoryType = new System.Windows.Forms.GroupBox();
            this.radioButtonAiMemoryType12 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType11 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxMemorySubType = new System.Windows.Forms.ComboBox();
            this.checkBoxAiBcd = new System.Windows.Forms.CheckBox();
            this.radioButtonAiMemoryType10 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType9 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType8 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType7 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType6 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAiMemoryType0 = new System.Windows.Forms.RadioButton();
            this.checkBoxAiDataSave = new System.Windows.Forms.CheckBox();
            this.checkBoxAiUsingAlarm = new System.Windows.Forms.CheckBox();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.textBoxAiSubOutSV = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxAiSubOutAO = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBoxAiSubLoLoDO = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBoxAiSubHiHiDO = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.textBoxAiHigh = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxAiHiHi = new System.Windows.Forms.TextBox();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.numericUpDownAiOnBigChangeSecond = new AutoLibLocal.MyNumericUpDown();
            this.label28 = new System.Windows.Forms.Label();
            this.numericUpDownAiOnBigChangePercent = new AutoLibLocal.MyNumericUpDown();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.numericUpDownAiChangeAlarm = new AutoLibLocal.MyNumericUpDown();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.textBoxAiAlarmReturnGab = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxAiViewBase = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxAiViewFull = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxAiScrollUnit = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxCalcScript = new System.Windows.Forms.TextBox();
            this.comboBoxAiUnit = new System.Windows.Forms.ComboBox();
            this.checkBoxAiAccumulatedData = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiAccumulation)).BeginInit();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiConfirmCount)).BeginInit();
            this.groupBoxCalcFilter.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiAlarmPriority)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.groupBoxPlcScan.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiScanTime)).BeginInit();
            this.groupBox16.SuspendLayout();
            this.groupBoxMemoryType.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox19.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiOnBigChangeSecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiOnBigChangePercent)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiChangeAlarm)).BeginInit();
            this.groupBox12.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // comboBoxAiAlarmCondition
            // 
            this.comboBoxAiAlarmCondition.AccessibleDescription = null;
            this.comboBoxAiAlarmCondition.AccessibleName = null;
            resources.ApplyResources(this.comboBoxAiAlarmCondition, "comboBoxAiAlarmCondition");
            this.comboBoxAiAlarmCondition.BackgroundImage = null;
            this.comboBoxAiAlarmCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAiAlarmCondition.Font = null;
            this.comboBoxAiAlarmCondition.Items.AddRange(new object[] {
            resources.GetString("comboBoxAiAlarmCondition.Items"),
            resources.GetString("comboBoxAiAlarmCondition.Items1"),
            resources.GetString("comboBoxAiAlarmCondition.Items2"),
            resources.GetString("comboBoxAiAlarmCondition.Items3"),
            resources.GetString("comboBoxAiAlarmCondition.Items4"),
            resources.GetString("comboBoxAiAlarmCondition.Items5"),
            resources.GetString("comboBoxAiAlarmCondition.Items6"),
            resources.GetString("comboBoxAiAlarmCondition.Items7"),
            resources.GetString("comboBoxAiAlarmCondition.Items8"),
            resources.GetString("comboBoxAiAlarmCondition.Items9")});
            this.comboBoxAiAlarmCondition.Name = "comboBoxAiAlarmCondition";
            // 
            // label17
            // 
            this.label17.AccessibleDescription = null;
            this.label17.AccessibleName = null;
            resources.ApplyResources(this.label17, "label17");
            this.label17.Font = null;
            this.label17.Name = "label17";
            // 
            // groupBox10
            // 
            this.groupBox10.AccessibleDescription = null;
            this.groupBox10.AccessibleName = null;
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.BackgroundImage = null;
            this.groupBox10.Controls.Add(this.label17);
            this.groupBox10.Controls.Add(this.numericUpDownAiAccumulation);
            this.groupBox10.Font = null;
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // numericUpDownAiAccumulation
            // 
            this.numericUpDownAiAccumulation.AccessibleDescription = null;
            this.numericUpDownAiAccumulation.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiAccumulation, "numericUpDownAiAccumulation");
            this.numericUpDownAiAccumulation.Font = null;
            this.numericUpDownAiAccumulation.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownAiAccumulation.Name = "numericUpDownAiAccumulation";
            this.numericUpDownAiAccumulation.SampleProperty = 0;
            // 
            // label26
            // 
            this.label26.AccessibleDescription = null;
            this.label26.AccessibleName = null;
            resources.ApplyResources(this.label26, "label26");
            this.label26.Font = null;
            this.label26.Name = "label26";
            // 
            // label18
            // 
            this.label18.AccessibleDescription = null;
            this.label18.AccessibleName = null;
            resources.ApplyResources(this.label18, "label18");
            this.label18.Font = null;
            this.label18.Name = "label18";
            // 
            // groupBox11
            // 
            this.groupBox11.AccessibleDescription = null;
            this.groupBox11.AccessibleName = null;
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.BackgroundImage = null;
            this.groupBox11.Controls.Add(this.label18);
            this.groupBox11.Controls.Add(this.numericUpDownAiConfirmCount);
            this.groupBox11.Font = null;
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // numericUpDownAiConfirmCount
            // 
            this.numericUpDownAiConfirmCount.AccessibleDescription = null;
            this.numericUpDownAiConfirmCount.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiConfirmCount, "numericUpDownAiConfirmCount");
            this.numericUpDownAiConfirmCount.Font = null;
            this.numericUpDownAiConfirmCount.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownAiConfirmCount.Name = "numericUpDownAiConfirmCount";
            this.numericUpDownAiConfirmCount.SampleProperty = 0;
            // 
            // radioButtonAiFilter4
            // 
            this.radioButtonAiFilter4.AccessibleDescription = null;
            this.radioButtonAiFilter4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiFilter4, "radioButtonAiFilter4");
            this.radioButtonAiFilter4.BackgroundImage = null;
            this.radioButtonAiFilter4.Font = null;
            this.radioButtonAiFilter4.Name = "radioButtonAiFilter4";
            this.radioButtonAiFilter4.CheckedChanged += new System.EventHandler(this.radioButtonAiFilter4_CheckedChanged);
            // 
            // radioButtonAiFilter3
            // 
            this.radioButtonAiFilter3.AccessibleDescription = null;
            this.radioButtonAiFilter3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiFilter3, "radioButtonAiFilter3");
            this.radioButtonAiFilter3.BackgroundImage = null;
            this.radioButtonAiFilter3.Font = null;
            this.radioButtonAiFilter3.Name = "radioButtonAiFilter3";
            this.radioButtonAiFilter3.CheckedChanged += new System.EventHandler(this.radioButtonAiFilter3_CheckedChanged);
            // 
            // radioButtonAiFilter2
            // 
            this.radioButtonAiFilter2.AccessibleDescription = null;
            this.radioButtonAiFilter2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiFilter2, "radioButtonAiFilter2");
            this.radioButtonAiFilter2.BackgroundImage = null;
            this.radioButtonAiFilter2.Font = null;
            this.radioButtonAiFilter2.Name = "radioButtonAiFilter2";
            this.radioButtonAiFilter2.CheckedChanged += new System.EventHandler(this.radioButtonAiFilter2_CheckedChanged);
            // 
            // radioButtonAiFilter1
            // 
            this.radioButtonAiFilter1.AccessibleDescription = null;
            this.radioButtonAiFilter1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiFilter1, "radioButtonAiFilter1");
            this.radioButtonAiFilter1.BackgroundImage = null;
            this.radioButtonAiFilter1.Font = null;
            this.radioButtonAiFilter1.Name = "radioButtonAiFilter1";
            this.radioButtonAiFilter1.CheckedChanged += new System.EventHandler(this.radioButtonAiFilter1_CheckedChanged);
            // 
            // radioButtonAiFilter0
            // 
            this.radioButtonAiFilter0.AccessibleDescription = null;
            this.radioButtonAiFilter0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiFilter0, "radioButtonAiFilter0");
            this.radioButtonAiFilter0.BackgroundImage = null;
            this.radioButtonAiFilter0.Font = null;
            this.radioButtonAiFilter0.Name = "radioButtonAiFilter0";
            this.radioButtonAiFilter0.CheckedChanged += new System.EventHandler(this.radioButtonAiFilter0_CheckedChanged);
            // 
            // groupBoxCalcFilter
            // 
            this.groupBoxCalcFilter.AccessibleDescription = null;
            this.groupBoxCalcFilter.AccessibleName = null;
            resources.ApplyResources(this.groupBoxCalcFilter, "groupBoxCalcFilter");
            this.groupBoxCalcFilter.BackgroundImage = null;
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAiFilter5);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAiFilter4);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAiFilter3);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAiFilter2);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAiFilter1);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAiFilter0);
            this.groupBoxCalcFilter.Font = null;
            this.groupBoxCalcFilter.Name = "groupBoxCalcFilter";
            this.groupBoxCalcFilter.TabStop = false;
            // 
            // radioButtonAiFilter5
            // 
            this.radioButtonAiFilter5.AccessibleDescription = null;
            this.radioButtonAiFilter5.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiFilter5, "radioButtonAiFilter5");
            this.radioButtonAiFilter5.BackgroundImage = null;
            this.radioButtonAiFilter5.Font = null;
            this.radioButtonAiFilter5.Name = "radioButtonAiFilter5";
            this.radioButtonAiFilter5.CheckedChanged += new System.EventHandler(this.radioButtonAiFilter5_CheckedChanged);
            // 
            // label27
            // 
            this.label27.AccessibleDescription = null;
            this.label27.AccessibleName = null;
            resources.ApplyResources(this.label27, "label27");
            this.label27.Font = null;
            this.label27.Name = "label27";
            // 
            // groupBox14
            // 
            this.groupBox14.AccessibleDescription = null;
            this.groupBox14.AccessibleName = null;
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.BackgroundImage = null;
            this.groupBox14.Controls.Add(this.numericUpDownAiAlarmPriority);
            this.groupBox14.Font = null;
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            // 
            // numericUpDownAiAlarmPriority
            // 
            this.numericUpDownAiAlarmPriority.AccessibleDescription = null;
            this.numericUpDownAiAlarmPriority.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiAlarmPriority, "numericUpDownAiAlarmPriority");
            this.numericUpDownAiAlarmPriority.Font = null;
            this.numericUpDownAiAlarmPriority.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownAiAlarmPriority.Name = "numericUpDownAiAlarmPriority";
            this.numericUpDownAiAlarmPriority.SampleProperty = 0;
            // 
            // textBoxAiDisplayFormat
            // 
            this.textBoxAiDisplayFormat.AccessibleDescription = null;
            this.textBoxAiDisplayFormat.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiDisplayFormat, "textBoxAiDisplayFormat");
            this.textBoxAiDisplayFormat.BackgroundImage = null;
            this.textBoxAiDisplayFormat.Font = null;
            this.textBoxAiDisplayFormat.Name = "textBoxAiDisplayFormat";
            // 
            // radioButtonAiOverflowCut1
            // 
            this.radioButtonAiOverflowCut1.AccessibleDescription = null;
            this.radioButtonAiOverflowCut1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiOverflowCut1, "radioButtonAiOverflowCut1");
            this.radioButtonAiOverflowCut1.BackgroundImage = null;
            this.radioButtonAiOverflowCut1.Font = null;
            this.radioButtonAiOverflowCut1.Name = "radioButtonAiOverflowCut1";
            // 
            // radioButtonAiOverflowCut0
            // 
            this.radioButtonAiOverflowCut0.AccessibleDescription = null;
            this.radioButtonAiOverflowCut0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiOverflowCut0, "radioButtonAiOverflowCut0");
            this.radioButtonAiOverflowCut0.BackgroundImage = null;
            this.radioButtonAiOverflowCut0.Font = null;
            this.radioButtonAiOverflowCut0.Name = "radioButtonAiOverflowCut0";
            // 
            // groupBox9
            // 
            this.groupBox9.AccessibleDescription = null;
            this.groupBox9.AccessibleName = null;
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.BackgroundImage = null;
            this.groupBox9.Controls.Add(this.radioButtonAiOverflowCut2);
            this.groupBox9.Controls.Add(this.radioButtonAiOverflowCut1);
            this.groupBox9.Controls.Add(this.radioButtonAiOverflowCut0);
            this.groupBox9.Font = null;
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // radioButtonAiOverflowCut2
            // 
            this.radioButtonAiOverflowCut2.AccessibleDescription = null;
            this.radioButtonAiOverflowCut2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiOverflowCut2, "radioButtonAiOverflowCut2");
            this.radioButtonAiOverflowCut2.BackgroundImage = null;
            this.radioButtonAiOverflowCut2.Font = null;
            this.radioButtonAiOverflowCut2.Name = "radioButtonAiOverflowCut2";
            // 
            // buttonAiSubOutAO
            // 
            this.buttonAiSubOutAO.AccessibleDescription = null;
            this.buttonAiSubOutAO.AccessibleName = null;
            resources.ApplyResources(this.buttonAiSubOutAO, "buttonAiSubOutAO");
            this.buttonAiSubOutAO.BackgroundImage = null;
            this.buttonAiSubOutAO.Font = null;
            this.buttonAiSubOutAO.Name = "buttonAiSubOutAO";
            this.buttonAiSubOutAO.Click += new System.EventHandler(this.buttonAiSubOutAO_Click);
            // 
            // textBoxAiAddress
            // 
            this.textBoxAiAddress.AccessibleDescription = null;
            this.textBoxAiAddress.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiAddress, "textBoxAiAddress");
            this.textBoxAiAddress.BackgroundImage = null;
            this.textBoxAiAddress.Font = null;
            this.textBoxAiAddress.Name = "textBoxAiAddress";
            // 
            // label33
            // 
            this.label33.AccessibleDescription = null;
            this.label33.AccessibleName = null;
            resources.ApplyResources(this.label33, "label33");
            this.label33.Font = null;
            this.label33.Name = "label33";
            // 
            // textBoxAiPort
            // 
            this.textBoxAiPort.AccessibleDescription = null;
            this.textBoxAiPort.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiPort, "textBoxAiPort");
            this.textBoxAiPort.BackgroundImage = null;
            this.textBoxAiPort.Font = null;
            this.textBoxAiPort.Name = "textBoxAiPort";
            // 
            // label35
            // 
            this.label35.AccessibleDescription = null;
            this.label35.AccessibleName = null;
            resources.ApplyResources(this.label35, "label35");
            this.label35.Font = null;
            this.label35.Name = "label35";
            // 
            // groupBoxPlcScan
            // 
            this.groupBoxPlcScan.AccessibleDescription = null;
            this.groupBoxPlcScan.AccessibleName = null;
            resources.ApplyResources(this.groupBoxPlcScan, "groupBoxPlcScan");
            this.groupBoxPlcScan.BackgroundImage = null;
            this.groupBoxPlcScan.Controls.Add(this.textBoxAiAddress);
            this.groupBoxPlcScan.Controls.Add(this.label33);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAiPort);
            this.groupBoxPlcScan.Controls.Add(this.label35);
            this.groupBoxPlcScan.Font = null;
            this.groupBoxPlcScan.Name = "groupBoxPlcScan";
            this.groupBoxPlcScan.TabStop = false;
            // 
            // textBoxAiPlcBase
            // 
            this.textBoxAiPlcBase.AccessibleDescription = null;
            this.textBoxAiPlcBase.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiPlcBase, "textBoxAiPlcBase");
            this.textBoxAiPlcBase.BackgroundImage = null;
            this.textBoxAiPlcBase.Font = null;
            this.textBoxAiPlcBase.Name = "textBoxAiPlcBase";
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // textBoxAiPlcFull
            // 
            this.textBoxAiPlcFull.AccessibleDescription = null;
            this.textBoxAiPlcFull.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiPlcFull, "textBoxAiPlcFull");
            this.textBoxAiPlcFull.BackgroundImage = null;
            this.textBoxAiPlcFull.Font = null;
            this.textBoxAiPlcFull.Name = "textBoxAiPlcFull";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // textBoxAiBase
            // 
            this.textBoxAiBase.AccessibleDescription = null;
            this.textBoxAiBase.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiBase, "textBoxAiBase");
            this.textBoxAiBase.BackgroundImage = null;
            this.textBoxAiBase.Font = null;
            this.textBoxAiBase.Name = "textBoxAiBase";
            // 
            // label15
            // 
            this.label15.AccessibleDescription = null;
            this.label15.AccessibleName = null;
            resources.ApplyResources(this.label15, "label15");
            this.label15.Font = null;
            this.label15.Name = "label15";
            // 
            // textBoxAiFull
            // 
            this.textBoxAiFull.AccessibleDescription = null;
            this.textBoxAiFull.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiFull, "textBoxAiFull");
            this.textBoxAiFull.BackgroundImage = null;
            this.textBoxAiFull.Font = null;
            this.textBoxAiFull.Name = "textBoxAiFull";
            // 
            // label16
            // 
            this.label16.AccessibleDescription = null;
            this.label16.AccessibleName = null;
            resources.ApplyResources(this.label16, "label16");
            this.label16.Font = null;
            this.label16.Name = "label16";
            // 
            // groupBox7
            // 
            this.groupBox7.AccessibleDescription = null;
            this.groupBox7.AccessibleName = null;
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.BackgroundImage = null;
            this.groupBox7.Controls.Add(this.textBoxAiPlcBase);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.textBoxAiPlcFull);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.textBoxAiBase);
            this.groupBox7.Controls.Add(this.label15);
            this.groupBox7.Controls.Add(this.textBoxAiFull);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Font = null;
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // buttonAiSubLoLoDO
            // 
            this.buttonAiSubLoLoDO.AccessibleDescription = null;
            this.buttonAiSubLoLoDO.AccessibleName = null;
            resources.ApplyResources(this.buttonAiSubLoLoDO, "buttonAiSubLoLoDO");
            this.buttonAiSubLoLoDO.BackgroundImage = null;
            this.buttonAiSubLoLoDO.Font = null;
            this.buttonAiSubLoLoDO.Name = "buttonAiSubLoLoDO";
            this.buttonAiSubLoLoDO.Click += new System.EventHandler(this.buttonAiSubLoLoDO_Click);
            // 
            // buttonAiAlarmWaveFile
            // 
            this.buttonAiAlarmWaveFile.AccessibleDescription = null;
            this.buttonAiAlarmWaveFile.AccessibleName = null;
            resources.ApplyResources(this.buttonAiAlarmWaveFile, "buttonAiAlarmWaveFile");
            this.buttonAiAlarmWaveFile.BackgroundImage = null;
            this.buttonAiAlarmWaveFile.Font = null;
            this.buttonAiAlarmWaveFile.Name = "buttonAiAlarmWaveFile";
            this.buttonAiAlarmWaveFile.Click += new System.EventHandler(this.buttonAiAlarmWaveFile_Click);
            // 
            // textBoxAiAlarmWaveFile
            // 
            this.textBoxAiAlarmWaveFile.AccessibleDescription = null;
            this.textBoxAiAlarmWaveFile.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiAlarmWaveFile, "textBoxAiAlarmWaveFile");
            this.textBoxAiAlarmWaveFile.BackgroundImage = null;
            this.textBoxAiAlarmWaveFile.Font = null;
            this.textBoxAiAlarmWaveFile.Name = "textBoxAiAlarmWaveFile";
            // 
            // groupBox17
            // 
            this.groupBox17.AccessibleDescription = null;
            this.groupBox17.AccessibleName = null;
            resources.ApplyResources(this.groupBox17, "groupBox17");
            this.groupBox17.BackgroundImage = null;
            this.groupBox17.Controls.Add(this.buttonAiAlarmWaveFile);
            this.groupBox17.Controls.Add(this.textBoxAiAlarmWaveFile);
            this.groupBox17.Font = null;
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.TabStop = false;
            // 
            // label21
            // 
            this.label21.AccessibleDescription = null;
            this.label21.AccessibleName = null;
            resources.ApplyResources(this.label21, "label21");
            this.label21.Font = null;
            this.label21.Name = "label21";
            // 
            // groupBox15
            // 
            this.groupBox15.AccessibleDescription = null;
            this.groupBox15.AccessibleName = null;
            resources.ApplyResources(this.groupBox15, "groupBox15");
            this.groupBox15.BackgroundImage = null;
            this.groupBox15.Controls.Add(this.label21);
            this.groupBox15.Controls.Add(this.numericUpDownAiScanTime);
            this.groupBox15.Font = null;
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.TabStop = false;
            // 
            // numericUpDownAiScanTime
            // 
            this.numericUpDownAiScanTime.AccessibleDescription = null;
            this.numericUpDownAiScanTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiScanTime, "numericUpDownAiScanTime");
            this.numericUpDownAiScanTime.Font = null;
            this.numericUpDownAiScanTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownAiScanTime.Name = "numericUpDownAiScanTime";
            this.numericUpDownAiScanTime.SampleProperty = 0;
            // 
            // textBoxAiLow
            // 
            this.textBoxAiLow.AccessibleDescription = null;
            this.textBoxAiLow.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiLow, "textBoxAiLow");
            this.textBoxAiLow.BackgroundImage = null;
            this.textBoxAiLow.Font = null;
            this.textBoxAiLow.Name = "textBoxAiLow";
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // buttonAiSubOutSV
            // 
            this.buttonAiSubOutSV.AccessibleDescription = null;
            this.buttonAiSubOutSV.AccessibleName = null;
            resources.ApplyResources(this.buttonAiSubOutSV, "buttonAiSubOutSV");
            this.buttonAiSubOutSV.BackgroundImage = null;
            this.buttonAiSubOutSV.Font = null;
            this.buttonAiSubOutSV.Name = "buttonAiSubOutSV";
            this.buttonAiSubOutSV.Click += new System.EventHandler(this.buttonAiSubOutSV_Click);
            // 
            // label29
            // 
            this.label29.AccessibleDescription = null;
            this.label29.AccessibleName = null;
            resources.ApplyResources(this.label29, "label29");
            this.label29.Font = null;
            this.label29.Name = "label29";
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // textBoxAiLoLo
            // 
            this.textBoxAiLoLo.AccessibleDescription = null;
            this.textBoxAiLoLo.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiLoLo, "textBoxAiLoLo");
            this.textBoxAiLoLo.BackgroundImage = null;
            this.textBoxAiLoLo.Font = null;
            this.textBoxAiLoLo.Name = "textBoxAiLoLo";
            // 
            // buttonAiSubHiHiDO
            // 
            this.buttonAiSubHiHiDO.AccessibleDescription = null;
            this.buttonAiSubHiHiDO.AccessibleName = null;
            resources.ApplyResources(this.buttonAiSubHiHiDO, "buttonAiSubHiHiDO");
            this.buttonAiSubHiHiDO.BackgroundImage = null;
            this.buttonAiSubHiHiDO.Font = null;
            this.buttonAiSubHiHiDO.Name = "buttonAiSubHiHiDO";
            this.buttonAiSubHiHiDO.Click += new System.EventHandler(this.buttonAiSubHiHiDO_Click);
            // 
            // buttonAiAlarmGraphicFile
            // 
            this.buttonAiAlarmGraphicFile.AccessibleDescription = null;
            this.buttonAiAlarmGraphicFile.AccessibleName = null;
            resources.ApplyResources(this.buttonAiAlarmGraphicFile, "buttonAiAlarmGraphicFile");
            this.buttonAiAlarmGraphicFile.BackgroundImage = null;
            this.buttonAiAlarmGraphicFile.Font = null;
            this.buttonAiAlarmGraphicFile.Name = "buttonAiAlarmGraphicFile";
            this.buttonAiAlarmGraphicFile.Click += new System.EventHandler(this.buttonAiAlarmGraphicFile_Click);
            // 
            // textBoxAiAlarmGraphicFile
            // 
            this.textBoxAiAlarmGraphicFile.AccessibleDescription = null;
            this.textBoxAiAlarmGraphicFile.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiAlarmGraphicFile, "textBoxAiAlarmGraphicFile");
            this.textBoxAiAlarmGraphicFile.BackgroundImage = null;
            this.textBoxAiAlarmGraphicFile.Font = null;
            this.textBoxAiAlarmGraphicFile.Name = "textBoxAiAlarmGraphicFile";
            // 
            // groupBox16
            // 
            this.groupBox16.AccessibleDescription = null;
            this.groupBox16.AccessibleName = null;
            resources.ApplyResources(this.groupBox16, "groupBox16");
            this.groupBox16.BackgroundImage = null;
            this.groupBox16.Controls.Add(this.buttonAiAlarmGraphicFile);
            this.groupBox16.Controls.Add(this.textBoxAiAlarmGraphicFile);
            this.groupBox16.Font = null;
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.TabStop = false;
            // 
            // groupBoxMemoryType
            // 
            this.groupBoxMemoryType.AccessibleDescription = null;
            this.groupBoxMemoryType.AccessibleName = null;
            resources.ApplyResources(this.groupBoxMemoryType, "groupBoxMemoryType");
            this.groupBoxMemoryType.BackgroundImage = null;
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType12);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType11);
            this.groupBoxMemoryType.Controls.Add(this.label1);
            this.groupBoxMemoryType.Controls.Add(this.comboBoxMemorySubType);
            this.groupBoxMemoryType.Controls.Add(this.checkBoxAiBcd);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType10);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType9);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType8);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType7);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType6);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType5);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType4);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType3);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType2);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType1);
            this.groupBoxMemoryType.Controls.Add(this.radioButtonAiMemoryType0);
            this.groupBoxMemoryType.Font = null;
            this.groupBoxMemoryType.Name = "groupBoxMemoryType";
            this.groupBoxMemoryType.TabStop = false;
            // 
            // radioButtonAiMemoryType12
            // 
            this.radioButtonAiMemoryType12.AccessibleDescription = null;
            this.radioButtonAiMemoryType12.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType12, "radioButtonAiMemoryType12");
            this.radioButtonAiMemoryType12.BackgroundImage = null;
            this.radioButtonAiMemoryType12.Font = null;
            this.radioButtonAiMemoryType12.Name = "radioButtonAiMemoryType12";
            this.radioButtonAiMemoryType12.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType12_CheckedChanged);
            // 
            // radioButtonAiMemoryType11
            // 
            this.radioButtonAiMemoryType11.AccessibleDescription = null;
            this.radioButtonAiMemoryType11.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType11, "radioButtonAiMemoryType11");
            this.radioButtonAiMemoryType11.BackgroundImage = null;
            this.radioButtonAiMemoryType11.Font = null;
            this.radioButtonAiMemoryType11.Name = "radioButtonAiMemoryType11";
            this.radioButtonAiMemoryType11.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType11_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comboBoxMemorySubType
            // 
            this.comboBoxMemorySubType.AccessibleDescription = null;
            this.comboBoxMemorySubType.AccessibleName = null;
            resources.ApplyResources(this.comboBoxMemorySubType, "comboBoxMemorySubType");
            this.comboBoxMemorySubType.BackgroundImage = null;
            this.comboBoxMemorySubType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMemorySubType.Font = null;
            this.comboBoxMemorySubType.Items.AddRange(new object[] {
            resources.GetString("comboBoxMemorySubType.Items"),
            resources.GetString("comboBoxMemorySubType.Items1"),
            resources.GetString("comboBoxMemorySubType.Items2"),
            resources.GetString("comboBoxMemorySubType.Items3"),
            resources.GetString("comboBoxMemorySubType.Items4"),
            resources.GetString("comboBoxMemorySubType.Items5"),
            resources.GetString("comboBoxMemorySubType.Items6"),
            resources.GetString("comboBoxMemorySubType.Items7"),
            resources.GetString("comboBoxMemorySubType.Items8"),
            resources.GetString("comboBoxMemorySubType.Items9")});
            this.comboBoxMemorySubType.Name = "comboBoxMemorySubType";
            // 
            // checkBoxAiBcd
            // 
            this.checkBoxAiBcd.AccessibleDescription = null;
            this.checkBoxAiBcd.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAiBcd, "checkBoxAiBcd");
            this.checkBoxAiBcd.BackgroundImage = null;
            this.checkBoxAiBcd.Font = null;
            this.checkBoxAiBcd.Name = "checkBoxAiBcd";
            // 
            // radioButtonAiMemoryType10
            // 
            this.radioButtonAiMemoryType10.AccessibleDescription = null;
            this.radioButtonAiMemoryType10.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType10, "radioButtonAiMemoryType10");
            this.radioButtonAiMemoryType10.BackgroundImage = null;
            this.radioButtonAiMemoryType10.Font = null;
            this.radioButtonAiMemoryType10.Name = "radioButtonAiMemoryType10";
            this.radioButtonAiMemoryType10.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType10_CheckedChanged);
            // 
            // radioButtonAiMemoryType9
            // 
            this.radioButtonAiMemoryType9.AccessibleDescription = null;
            this.radioButtonAiMemoryType9.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType9, "radioButtonAiMemoryType9");
            this.radioButtonAiMemoryType9.BackgroundImage = null;
            this.radioButtonAiMemoryType9.Font = null;
            this.radioButtonAiMemoryType9.Name = "radioButtonAiMemoryType9";
            this.radioButtonAiMemoryType9.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType9_CheckedChanged);
            // 
            // radioButtonAiMemoryType8
            // 
            this.radioButtonAiMemoryType8.AccessibleDescription = null;
            this.radioButtonAiMemoryType8.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType8, "radioButtonAiMemoryType8");
            this.radioButtonAiMemoryType8.BackgroundImage = null;
            this.radioButtonAiMemoryType8.Font = null;
            this.radioButtonAiMemoryType8.Name = "radioButtonAiMemoryType8";
            this.radioButtonAiMemoryType8.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType8_CheckedChanged);
            // 
            // radioButtonAiMemoryType7
            // 
            this.radioButtonAiMemoryType7.AccessibleDescription = null;
            this.radioButtonAiMemoryType7.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType7, "radioButtonAiMemoryType7");
            this.radioButtonAiMemoryType7.BackgroundImage = null;
            this.radioButtonAiMemoryType7.Font = null;
            this.radioButtonAiMemoryType7.Name = "radioButtonAiMemoryType7";
            this.radioButtonAiMemoryType7.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType7_CheckedChanged);
            // 
            // radioButtonAiMemoryType6
            // 
            this.radioButtonAiMemoryType6.AccessibleDescription = null;
            this.radioButtonAiMemoryType6.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType6, "radioButtonAiMemoryType6");
            this.radioButtonAiMemoryType6.BackgroundImage = null;
            this.radioButtonAiMemoryType6.Font = null;
            this.radioButtonAiMemoryType6.Name = "radioButtonAiMemoryType6";
            this.radioButtonAiMemoryType6.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType6_CheckedChanged);
            // 
            // radioButtonAiMemoryType5
            // 
            this.radioButtonAiMemoryType5.AccessibleDescription = null;
            this.radioButtonAiMemoryType5.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType5, "radioButtonAiMemoryType5");
            this.radioButtonAiMemoryType5.BackgroundImage = null;
            this.radioButtonAiMemoryType5.Font = null;
            this.radioButtonAiMemoryType5.Name = "radioButtonAiMemoryType5";
            this.radioButtonAiMemoryType5.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType5_CheckedChanged);
            // 
            // radioButtonAiMemoryType4
            // 
            this.radioButtonAiMemoryType4.AccessibleDescription = null;
            this.radioButtonAiMemoryType4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType4, "radioButtonAiMemoryType4");
            this.radioButtonAiMemoryType4.BackgroundImage = null;
            this.radioButtonAiMemoryType4.Font = null;
            this.radioButtonAiMemoryType4.Name = "radioButtonAiMemoryType4";
            this.radioButtonAiMemoryType4.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType4_CheckedChanged);
            // 
            // radioButtonAiMemoryType3
            // 
            this.radioButtonAiMemoryType3.AccessibleDescription = null;
            this.radioButtonAiMemoryType3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType3, "radioButtonAiMemoryType3");
            this.radioButtonAiMemoryType3.BackgroundImage = null;
            this.radioButtonAiMemoryType3.Font = null;
            this.radioButtonAiMemoryType3.Name = "radioButtonAiMemoryType3";
            this.radioButtonAiMemoryType3.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType3_CheckedChanged);
            // 
            // radioButtonAiMemoryType2
            // 
            this.radioButtonAiMemoryType2.AccessibleDescription = null;
            this.radioButtonAiMemoryType2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType2, "radioButtonAiMemoryType2");
            this.radioButtonAiMemoryType2.BackgroundImage = null;
            this.radioButtonAiMemoryType2.Font = null;
            this.radioButtonAiMemoryType2.Name = "radioButtonAiMemoryType2";
            this.radioButtonAiMemoryType2.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType2_CheckedChanged);
            // 
            // radioButtonAiMemoryType1
            // 
            this.radioButtonAiMemoryType1.AccessibleDescription = null;
            this.radioButtonAiMemoryType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType1, "radioButtonAiMemoryType1");
            this.radioButtonAiMemoryType1.BackgroundImage = null;
            this.radioButtonAiMemoryType1.Font = null;
            this.radioButtonAiMemoryType1.Name = "radioButtonAiMemoryType1";
            this.radioButtonAiMemoryType1.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType1_CheckedChanged);
            // 
            // radioButtonAiMemoryType0
            // 
            this.radioButtonAiMemoryType0.AccessibleDescription = null;
            this.radioButtonAiMemoryType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAiMemoryType0, "radioButtonAiMemoryType0");
            this.radioButtonAiMemoryType0.BackgroundImage = null;
            this.radioButtonAiMemoryType0.Font = null;
            this.radioButtonAiMemoryType0.Name = "radioButtonAiMemoryType0";
            this.radioButtonAiMemoryType0.CheckedChanged += new System.EventHandler(this.radioButtonAiMemoryType0_CheckedChanged);
            // 
            // checkBoxAiDataSave
            // 
            this.checkBoxAiDataSave.AccessibleDescription = null;
            this.checkBoxAiDataSave.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAiDataSave, "checkBoxAiDataSave");
            this.checkBoxAiDataSave.BackgroundImage = null;
            this.checkBoxAiDataSave.Font = null;
            this.checkBoxAiDataSave.Name = "checkBoxAiDataSave";
            this.checkBoxAiDataSave.CheckedChanged += new System.EventHandler(this.checkBoxAiDataSave_CheckedChanged);
            // 
            // checkBoxAiUsingAlarm
            // 
            this.checkBoxAiUsingAlarm.AccessibleDescription = null;
            this.checkBoxAiUsingAlarm.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAiUsingAlarm, "checkBoxAiUsingAlarm");
            this.checkBoxAiUsingAlarm.BackgroundImage = null;
            this.checkBoxAiUsingAlarm.Font = null;
            this.checkBoxAiUsingAlarm.Name = "checkBoxAiUsingAlarm";
            // 
            // groupBox18
            // 
            this.groupBox18.AccessibleDescription = null;
            this.groupBox18.AccessibleName = null;
            resources.ApplyResources(this.groupBox18, "groupBox18");
            this.groupBox18.BackgroundImage = null;
            this.groupBox18.Controls.Add(this.buttonAiSubOutSV);
            this.groupBox18.Controls.Add(this.buttonAiSubOutAO);
            this.groupBox18.Controls.Add(this.buttonAiSubLoLoDO);
            this.groupBox18.Controls.Add(this.textBoxAiSubOutSV);
            this.groupBox18.Controls.Add(this.label22);
            this.groupBox18.Controls.Add(this.textBoxAiSubOutAO);
            this.groupBox18.Controls.Add(this.label23);
            this.groupBox18.Controls.Add(this.textBoxAiSubLoLoDO);
            this.groupBox18.Controls.Add(this.label24);
            this.groupBox18.Controls.Add(this.textBoxAiSubHiHiDO);
            this.groupBox18.Controls.Add(this.label25);
            this.groupBox18.Controls.Add(this.buttonAiSubHiHiDO);
            this.groupBox18.Font = null;
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.TabStop = false;
            // 
            // textBoxAiSubOutSV
            // 
            this.textBoxAiSubOutSV.AccessibleDescription = null;
            this.textBoxAiSubOutSV.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiSubOutSV, "textBoxAiSubOutSV");
            this.textBoxAiSubOutSV.BackgroundImage = null;
            this.textBoxAiSubOutSV.Font = null;
            this.textBoxAiSubOutSV.Name = "textBoxAiSubOutSV";
            // 
            // label22
            // 
            this.label22.AccessibleDescription = null;
            this.label22.AccessibleName = null;
            resources.ApplyResources(this.label22, "label22");
            this.label22.Font = null;
            this.label22.Name = "label22";
            // 
            // textBoxAiSubOutAO
            // 
            this.textBoxAiSubOutAO.AccessibleDescription = null;
            this.textBoxAiSubOutAO.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiSubOutAO, "textBoxAiSubOutAO");
            this.textBoxAiSubOutAO.BackgroundImage = null;
            this.textBoxAiSubOutAO.Font = null;
            this.textBoxAiSubOutAO.Name = "textBoxAiSubOutAO";
            // 
            // label23
            // 
            this.label23.AccessibleDescription = null;
            this.label23.AccessibleName = null;
            resources.ApplyResources(this.label23, "label23");
            this.label23.Font = null;
            this.label23.Name = "label23";
            // 
            // textBoxAiSubLoLoDO
            // 
            this.textBoxAiSubLoLoDO.AccessibleDescription = null;
            this.textBoxAiSubLoLoDO.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiSubLoLoDO, "textBoxAiSubLoLoDO");
            this.textBoxAiSubLoLoDO.BackgroundImage = null;
            this.textBoxAiSubLoLoDO.Font = null;
            this.textBoxAiSubLoLoDO.Name = "textBoxAiSubLoLoDO";
            // 
            // label24
            // 
            this.label24.AccessibleDescription = null;
            this.label24.AccessibleName = null;
            resources.ApplyResources(this.label24, "label24");
            this.label24.Font = null;
            this.label24.Name = "label24";
            // 
            // textBoxAiSubHiHiDO
            // 
            this.textBoxAiSubHiHiDO.AccessibleDescription = null;
            this.textBoxAiSubHiHiDO.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiSubHiHiDO, "textBoxAiSubHiHiDO");
            this.textBoxAiSubHiHiDO.BackgroundImage = null;
            this.textBoxAiSubHiHiDO.Font = null;
            this.textBoxAiSubHiHiDO.Name = "textBoxAiSubHiHiDO";
            // 
            // label25
            // 
            this.label25.AccessibleDescription = null;
            this.label25.AccessibleName = null;
            resources.ApplyResources(this.label25, "label25");
            this.label25.Font = null;
            this.label25.Name = "label25";
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = null;
            this.groupBox5.AccessibleName = null;
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.BackgroundImage = null;
            this.groupBox5.Controls.Add(this.comboBoxAiAlarmCondition);
            this.groupBox5.Controls.Add(this.label30);
            this.groupBox5.Controls.Add(this.textBoxAiLoLo);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.textBoxAiLow);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.textBoxAiHigh);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.textBoxAiHiHi);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Font = null;
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label30
            // 
            this.label30.AccessibleDescription = null;
            this.label30.AccessibleName = null;
            resources.ApplyResources(this.label30, "label30");
            this.label30.Font = null;
            this.label30.Name = "label30";
            // 
            // textBoxAiHigh
            // 
            this.textBoxAiHigh.AccessibleDescription = null;
            this.textBoxAiHigh.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiHigh, "textBoxAiHigh");
            this.textBoxAiHigh.BackgroundImage = null;
            this.textBoxAiHigh.Font = null;
            this.textBoxAiHigh.Name = "textBoxAiHigh";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // textBoxAiHiHi
            // 
            this.textBoxAiHiHi.AccessibleDescription = null;
            this.textBoxAiHiHi.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiHiHi, "textBoxAiHiHi");
            this.textBoxAiHiHi.BackgroundImage = null;
            this.textBoxAiHiHi.Font = null;
            this.textBoxAiHiHi.Name = "textBoxAiHiHi";
            // 
            // groupBox19
            // 
            this.groupBox19.AccessibleDescription = null;
            this.groupBox19.AccessibleName = null;
            resources.ApplyResources(this.groupBox19, "groupBox19");
            this.groupBox19.BackgroundImage = null;
            this.groupBox19.Controls.Add(this.label29);
            this.groupBox19.Controls.Add(this.numericUpDownAiOnBigChangeSecond);
            this.groupBox19.Controls.Add(this.label28);
            this.groupBox19.Controls.Add(this.numericUpDownAiOnBigChangePercent);
            this.groupBox19.Font = null;
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.TabStop = false;
            // 
            // numericUpDownAiOnBigChangeSecond
            // 
            this.numericUpDownAiOnBigChangeSecond.AccessibleDescription = null;
            this.numericUpDownAiOnBigChangeSecond.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiOnBigChangeSecond, "numericUpDownAiOnBigChangeSecond");
            this.numericUpDownAiOnBigChangeSecond.Font = null;
            this.numericUpDownAiOnBigChangeSecond.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericUpDownAiOnBigChangeSecond.Name = "numericUpDownAiOnBigChangeSecond";
            this.numericUpDownAiOnBigChangeSecond.SampleProperty = 0;
            // 
            // label28
            // 
            this.label28.AccessibleDescription = null;
            this.label28.AccessibleName = null;
            resources.ApplyResources(this.label28, "label28");
            this.label28.Font = null;
            this.label28.Name = "label28";
            // 
            // numericUpDownAiOnBigChangePercent
            // 
            this.numericUpDownAiOnBigChangePercent.AccessibleDescription = null;
            this.numericUpDownAiOnBigChangePercent.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiOnBigChangePercent, "numericUpDownAiOnBigChangePercent");
            this.numericUpDownAiOnBigChangePercent.Font = null;
            this.numericUpDownAiOnBigChangePercent.Name = "numericUpDownAiOnBigChangePercent";
            this.numericUpDownAiOnBigChangePercent.SampleProperty = 0;
            // 
            // groupBox13
            // 
            this.groupBox13.AccessibleDescription = null;
            this.groupBox13.AccessibleName = null;
            resources.ApplyResources(this.groupBox13, "groupBox13");
            this.groupBox13.BackgroundImage = null;
            this.groupBox13.Controls.Add(this.label20);
            this.groupBox13.Controls.Add(this.numericUpDownAiChangeAlarm);
            this.groupBox13.Font = null;
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.TabStop = false;
            // 
            // label20
            // 
            this.label20.AccessibleDescription = null;
            this.label20.AccessibleName = null;
            resources.ApplyResources(this.label20, "label20");
            this.label20.Font = null;
            this.label20.Name = "label20";
            // 
            // numericUpDownAiChangeAlarm
            // 
            this.numericUpDownAiChangeAlarm.AccessibleDescription = null;
            this.numericUpDownAiChangeAlarm.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownAiChangeAlarm, "numericUpDownAiChangeAlarm");
            this.numericUpDownAiChangeAlarm.Font = null;
            this.numericUpDownAiChangeAlarm.Name = "numericUpDownAiChangeAlarm";
            this.numericUpDownAiChangeAlarm.SampleProperty = 0;
            // 
            // groupBox12
            // 
            this.groupBox12.AccessibleDescription = null;
            this.groupBox12.AccessibleName = null;
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.BackgroundImage = null;
            this.groupBox12.Controls.Add(this.textBoxAiAlarmReturnGab);
            this.groupBox12.Font = null;
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            // 
            // textBoxAiAlarmReturnGab
            // 
            this.textBoxAiAlarmReturnGab.AccessibleDescription = null;
            this.textBoxAiAlarmReturnGab.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiAlarmReturnGab, "textBoxAiAlarmReturnGab");
            this.textBoxAiAlarmReturnGab.BackgroundImage = null;
            this.textBoxAiAlarmReturnGab.Font = null;
            this.textBoxAiAlarmReturnGab.Name = "textBoxAiAlarmReturnGab";
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.textBoxAiViewBase);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.textBoxAiViewFull);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // textBoxAiViewBase
            // 
            this.textBoxAiViewBase.AccessibleDescription = null;
            this.textBoxAiViewBase.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiViewBase, "textBoxAiViewBase");
            this.textBoxAiViewBase.BackgroundImage = null;
            this.textBoxAiViewBase.Font = null;
            this.textBoxAiViewBase.Name = "textBoxAiViewBase";
            // 
            // label13
            // 
            this.label13.AccessibleDescription = null;
            this.label13.AccessibleName = null;
            resources.ApplyResources(this.label13, "label13");
            this.label13.Font = null;
            this.label13.Name = "label13";
            // 
            // textBoxAiViewFull
            // 
            this.textBoxAiViewFull.AccessibleDescription = null;
            this.textBoxAiViewFull.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiViewFull, "textBoxAiViewFull");
            this.textBoxAiViewFull.BackgroundImage = null;
            this.textBoxAiViewFull.Font = null;
            this.textBoxAiViewFull.Name = "textBoxAiViewFull";
            // 
            // label14
            // 
            this.label14.AccessibleDescription = null;
            this.label14.AccessibleName = null;
            resources.ApplyResources(this.label14, "label14");
            this.label14.Font = null;
            this.label14.Name = "label14";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxAiScrollUnit);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxAiScrollUnit
            // 
            this.textBoxAiScrollUnit.AccessibleDescription = null;
            this.textBoxAiScrollUnit.AccessibleName = null;
            resources.ApplyResources(this.textBoxAiScrollUnit, "textBoxAiScrollUnit");
            this.textBoxAiScrollUnit.BackgroundImage = null;
            this.textBoxAiScrollUnit.Font = null;
            this.textBoxAiScrollUnit.Name = "textBoxAiScrollUnit";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.textBoxCalcScript);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxCalcScript
            // 
            this.textBoxCalcScript.AccessibleDescription = null;
            this.textBoxCalcScript.AccessibleName = null;
            resources.ApplyResources(this.textBoxCalcScript, "textBoxCalcScript");
            this.textBoxCalcScript.BackgroundImage = null;
            this.textBoxCalcScript.Font = null;
            this.textBoxCalcScript.Name = "textBoxCalcScript";
            // 
            // comboBoxAiUnit
            // 
            this.comboBoxAiUnit.AccessibleDescription = null;
            this.comboBoxAiUnit.AccessibleName = null;
            resources.ApplyResources(this.comboBoxAiUnit, "comboBoxAiUnit");
            this.comboBoxAiUnit.BackgroundImage = null;
            this.comboBoxAiUnit.Font = null;
            this.comboBoxAiUnit.FormattingEnabled = true;
            this.comboBoxAiUnit.Name = "comboBoxAiUnit";
            // 
            // checkBoxAiAccumulatedData
            // 
            this.checkBoxAiAccumulatedData.AccessibleDescription = null;
            this.checkBoxAiAccumulatedData.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAiAccumulatedData, "checkBoxAiAccumulatedData");
            this.checkBoxAiAccumulatedData.BackgroundImage = null;
            this.checkBoxAiAccumulatedData.Font = null;
            this.checkBoxAiAccumulatedData.Name = "checkBoxAiAccumulatedData";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.groupBox14);
            this.groupBox3.Controls.Add(this.groupBox16);
            this.groupBox3.Controls.Add(this.groupBox19);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox13);
            this.groupBox3.Controls.Add(this.groupBox12);
            this.groupBox3.Controls.Add(this.checkBoxAiUsingAlarm);
            this.groupBox3.Controls.Add(this.groupBox17);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // PropertyTagAI
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.comboBoxAiUnit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.textBoxAiDisplayFormat);
            this.Controls.Add(this.groupBoxCalcFilter);
            this.Controls.Add(this.groupBoxMemoryType);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox18);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBoxPlcScan);
            this.Controls.Add(this.groupBox15);
            this.Controls.Add(this.checkBoxAiAccumulatedData);
            this.Controls.Add(this.checkBoxAiDataSave);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagAI";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PropertyTagAI_Load);
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiAccumulation)).EndInit();
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiConfirmCount)).EndInit();
            this.groupBoxCalcFilter.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiAlarmPriority)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBoxPlcScan.ResumeLayout(false);
            this.groupBoxPlcScan.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiScanTime)).EndInit();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBoxMemoryType.ResumeLayout(false);
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiOnBigChangeSecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiOnBigChangePercent)).EndInit();
            this.groupBox13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAiChangeAlarm)).EndInit();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void SetTagAI(TagAiClass ai)
		{
			multiAiMemoryType.Set(ai.fn);
			multiAiBcd.Set(ai.bBcdValue);
			multiAiPort.Set(ai.port);

			multiAiAddress.Set(ai.address);
			multiAiUnit.Set(ai.unit);
			multiAiFormat.Set(ai.sDisplayFormat);
			multiAiHiHi.Set(ai.hihi);
			multiAiHigh.Set(ai.high);
			multiAiLow.Set(ai.low);
			multiAiLoLo.Set(ai.lolo);

			multiAiAlarmCondition.Set(ai.cAlarmType);

			multiAiSubHiHiDo.Set(ai.sSubOutDigitalHiHi);
			multiAiSubLoLoDo.Set(ai.sSubOutDigitalLoLo);
			multiAiSubOutAo.Set(ai.sSubOutAnalog);
			multiAiSubAoSv.Set(ai.sSubOutAnalogSP);

			multiAiFull.Set(ai.fFull);
			multiAiBase.Set(ai.fBase);
			multiAiPlcFull.Set(ai.fPlcFull);
			multiAiPlcBase.Set(ai.fPlcBase);
			multiAiViewFull.Set(ai.view_full);
			multiAiViewBase.Set(ai.view_base);

			multiAiAlarmWave.Set(ai.sAlarmWaveFile);
			multiAiAlarmGraphic.Set(ai.sGraphicFile);

			multiAiBigChangePercent.Set(ai.cAlarmProtectOnBigChangePercent);
			multiAiBigChangeSecond.Set(ai.nAlarmProtectOnBigChangeSecond);
			multiAiAlarmPriority.Set(ai.wAlarmPriority);
			multiAiAccumulation.Set(ai.nCalcDelay);
			multiAiConfirmCount.Set(ai.cConfirmCount);
			multiAiScanTime.Set(ai.wScanTime);
			multiAiAlarmRuturnGab.Set(ai.fAlarmReturnGab);
			multiAiAlarmChange.Set(ai.wRateOfChangeLimit);

			multiAiFilter.Set(ai.nCalculateFilter);
			multiAiCut.Set(ai.bCutOverValue);

			multiAiUsingAlarm.Set(ai.alarm);
			multiAiDataSave.Set(ai.bFileSave);

			multiAiScrollUnit.Set(ai.fScrollUnit);
            multiAiMemorySubType.Set(ai.nMemoryTypeSub);
            multiAiCalcScript.Set(ai.sCalcScript);

            multiAiDataAccumulate.Set(ai.bAccumulatedValue);  //20250225 PSU


            if (checkBoxAiDataSave.Checked) checkBoxAiAccumulatedData.Enabled = true; //20250314 PSU 
            else checkBoxAiAccumulatedData.Enabled = false;
		}

		public void GetTagAI(TagAiClass ai)
		{
			multiAiMemoryType.Get(ref ai.fn);
			multiAiBcd.Get(ref ai.bBcdValue);
			multiAiPort.Get(ref ai.port);

			multiAiAddress.Get(ref ai.address);
			multiAiUnit.Get(ref ai.unit);

			multiAiFormat.Get(ref ai.sDisplayFormat);

			// 포맷에 오류가 있으면 자동으로 10.2
			if(!TagUtil.StringToDisplayFormat(ai.sDisplayFormat, out ai.fDisplayFormat, out ai.cDisplayFormat))
			{
				ai.sDisplayFormat = "10.2";
			}

			multiAiHiHi.Get(ref ai.hihi);
			multiAiHigh.Get(ref ai.high);
			multiAiLow.Get(ref ai.low);
			multiAiLoLo.Get(ref ai.lolo);
			multiAiAlarmCondition.Get(ref ai.cAlarmType);

			multiAiSubHiHiDo.Get(ref ai.sSubOutDigitalHiHi);
			multiAiSubLoLoDo.Get(ref ai.sSubOutDigitalLoLo);
			multiAiSubOutAo.Get(ref ai.sSubOutAnalog);
			multiAiSubAoSv.Get(ref ai.sSubOutAnalogSP);

            double old_full = ai.fFull;
            double old_base = ai.fBase;
            double old_viewfull = ai.view_full;
            double old_viewbase = ai.view_base;

			multiAiFull.Get(ref ai.fFull);
			multiAiBase.Get(ref ai.fBase);
			multiAiPlcFull.Get(ref ai.fPlcFull);
			multiAiPlcBase.Get(ref ai.fPlcBase);
			multiAiViewFull.Get(ref ai.view_full);
			multiAiViewBase.Get(ref ai.view_base);

            // Full/Base 와 view_full/view_base 가 같았을 경우 Full/Base만 바뀌면 따라간다.
            if (old_full == old_viewfull && old_base == old_viewbase)
            {
                // viewfull/viewbase가 변함이 없을 경우만 따라간다.
                if (old_viewfull == ai.view_full && old_viewbase == ai.view_base)
                {
                    ai.view_base = ai.fBase;
                    ai.view_full = ai.fFull;
                }
            }

			multiAiAlarmWave.Get(ref ai.sAlarmWaveFile);
			multiAiAlarmGraphic.Get(ref ai.sGraphicFile);

			multiAiBigChangePercent.Get(ref ai.cAlarmProtectOnBigChangePercent);
			multiAiBigChangeSecond.Get(ref ai.nAlarmProtectOnBigChangeSecond);
			multiAiAlarmPriority.Get(ref ai.wAlarmPriority);
			multiAiAccumulation.Get(ref ai.nCalcDelay);
			multiAiConfirmCount.Get(ref ai.cConfirmCount);
			multiAiScanTime.Get(ref ai.wScanTime);
			multiAiAlarmRuturnGab.Get(ref ai.fAlarmReturnGab);
			multiAiAlarmChange.Get(ref ai.wRateOfChangeLimit);

			multiAiFilter.Get(ref ai.nCalculateFilter);
			multiAiCut.Get(ref ai.bCutOverValue);

			multiAiUsingAlarm.Get(ref ai.alarm);
			multiAiDataSave.Get(ref ai.bFileSave);

			multiAiScrollUnit.Get(ref ai.fScrollUnit);
            multiAiMemorySubType.Get(ref ai.nMemoryTypeSub);
            multiAiCalcScript.Get(ref ai.sCalcScript);

            multiAiDataAccumulate.Get(ref ai.bAccumulatedValue);  //20250225 PSU 누적값
		}

		private void buttonAiSubHiHiDO_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;

			if(SelectTag.SelectDoGdo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxAiSubHiHiDO.Text = tag;
			}
		}

		private void buttonAiSubLoLoDO_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDoGdo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxAiSubLoLoDO.Text = tag;
			}
		}

		private void buttonAiSubOutAO_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxAiSubOutAO.Text = tag;
			}
		}

		private void buttonAiSubOutSV_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxAiSubOutSV.Text = tag;
			}
		}

		private void buttonAiAlarmWaveFile_Click(object sender, System.EventArgs e)
		{
			string filename;

			if(FormTagProperty.SelectAlarmWaveFile(out filename)) 
			{
				this.textBoxAiAlarmWaveFile.Text = filename;
			}
		}

		private void buttonAiAlarmGraphicFile_Click(object sender, System.EventArgs e)
		{
			string filename;

			if(FormTagProperty.SelectAlarmModuleFile(out filename)) 
			{
				this.textBoxAiAlarmGraphicFile.Text = filename;
			}	
		}

        int nTagLinkType = 0;

		public void EnableDisableConnectionType(int type)
		{
			bool flag_plcscan = true;
			bool flag_calcfilter = true;

			if(type == 0)
			{
				
			}
			else if(type == 1) 
			{
				flag_plcscan = false;
			}
			else if(type == 2) 
			{
				flag_plcscan = false;
				flag_calcfilter = true;
			}
			else if(type == 3) 
			{
				flag_plcscan = false;	
				flag_calcfilter = false;
			}
			else if(type == 4) 
			{
				flag_plcscan = false;
				flag_calcfilter = false;
			}
			else if(type == 5)  // OPC
			{
				flag_plcscan = false;
				flag_calcfilter = true;
			}
			
			this.groupBoxPlcScan.Enabled = flag_plcscan;
			this.groupBoxMemoryType.Enabled = flag_plcscan;
			this.groupBoxCalcFilter.Enabled = flag_calcfilter;

            if (flag_calcfilter)
            {
                if (type == 2)  // 메모리 태그일 떄
                {
                    this.radioButtonAiFilter0.Enabled = false;
                    this.radioButtonAiFilter1.Enabled = false;
                    this.radioButtonAiFilter3.Enabled = false;
                    this.radioButtonAiFilter4.Enabled = false;
                }
                else
                {
                    this.radioButtonAiFilter0.Enabled = true;
                    this.radioButtonAiFilter1.Enabled = true;
                    this.radioButtonAiFilter3.Enabled = true;
                    this.radioButtonAiFilter4.Enabled = true;
                }
            }

            nTagLinkType = type;

            EnableDisablePlcFullBase();
		}

        public static string[] sSampleUnit = { "℃", "Å", "℉", "Ø", "㎕", "㎖", "㎗", "ℓ", "㎘", "㏄", 
                                                 "㎣", "㎤", "㎥", "㎦", "㎙", "㎚", "㎛", "㎜", "㎝", "㎞", 
                                                 "㎟", "㎠", "㎡", "㎢", "㏊", "㎍", "㎎", "㎏", "㏏", "㎈", 
                                                 "㎉", "㏈", "㎧", "㎨", "㎰", "㎱", "㎲", "㎳", "㎴", "㎵", 
                                                 "㎶", "㎷", "㎸", "㎹", "㎀", "㎁", "㎂", "㎃", "㎄", "㎺", 
                                                 "㎻", "㎼", "㎽", "㎾", "㎿", "㎐", "㎑", "㎒", "㎓", "㎔",
                                               "Ω", "㏀", "㏁", "㎊", "㎋", "㎌", "㏖", "㏅", "㎭", "㎮", 
                                               "㎯", "㏛", "㎩", "㎪", "㎫", "㎬", "㏝", "㏐", "㏓", "㏃", 
                                               "㏉", "㏜", "㏆", "％" };

        private void PropertyTagAI_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < PropertyTagAI.sSampleUnit.Length; i++)
            {
                this.comboBoxAiUnit.Items.Add(PropertyTagAI.sSampleUnit[i]);
            }
        }

        void EnableDisableCalcScript()
        {
            this.textBoxCalcScript.Enabled = this.radioButtonAiFilter5.Checked;
        }

        private void radioButtonAiFilter0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
            EnableDisablePlcFullBase();
        }

        private void radioButtonAiFilter1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
            EnableDisablePlcFullBase();
        }

        private void radioButtonAiFilter2_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
            EnableDisablePlcFullBase();
        }

        private void radioButtonAiFilter3_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
            EnableDisablePlcFullBase();
        }

        private void radioButtonAiFilter4_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
            EnableDisablePlcFullBase();
        }

        private void radioButtonAiFilter5_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
            EnableDisablePlcFullBase();
        }

        void EnableDisablePlcFullBase()
        {
            bool flag = true;

            if (nTagLinkType == 2)
            {
                flag = false;
            }
            else if (nTagLinkType == 3)
            {
                flag = false;
            }
            else
            {
                if (this.radioButtonAiFilter2.Checked) flag = false;
                if (this.radioButtonAiFilter4.Checked) flag = false;
                if (this.radioButtonAiFilter5.Checked) flag = false;
            }

            this.textBoxAiPlcBase.Enabled = flag;
            this.textBoxAiPlcFull.Enabled = flag;
        }

        void EnableDisableMemorySubType(bool enable, params string[] titles)
        {
            this.comboBoxMemorySubType.Enabled = enable;

            for (int i = 0; i < 10; i++)
            {
                if (i < titles.Length)
                {
                    this.comboBoxMemorySubType.Items[i] = titles[i];
                }
                else
                {
                    this.comboBoxMemorySubType.Items[i] = i.ToString();
                }
            }
        }

        private void radioButtonAiMemoryType0_CheckedChanged(object sender, EventArgs e)
        {
            /* 
                Default
                1=S H+L 
                2=S L+H
                3=U H+L
                4=U L+H
                5=F H+L
                6=F L+H
                7
                8
                9
             */

            EnableDisableMemorySubType(true, "0=Default", "1=S H+L", "2=S L+H", "3=U H+L", "4=U L+H", "5=F H+L", "6=F L+H");
        }

        private void radioButtonAiMemoryType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType3_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType4_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType5_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType6_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType7_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType8_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType9_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType10_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType11_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(false);
        }

        private void radioButtonAiMemoryType12_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableMemorySubType(true, "0=Signed", "1=Unsigned");
        }

        private void checkBoxAiDataSave_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAiDataSave.Checked) checkBoxAiAccumulatedData.Enabled = true;

            else checkBoxAiAccumulatedData.Enabled = false;
        }

	}
}
