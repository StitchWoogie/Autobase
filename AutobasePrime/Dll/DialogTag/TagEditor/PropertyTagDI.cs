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
	public class PropertyTagDI : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textBoxDiOffDes;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.TextBox textBoxDiOnDes;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.GroupBox groupBox30;
		private System.Windows.Forms.Button buttonDiSubOut2;
		private System.Windows.Forms.Button buttonDiSubOut1;
		private System.Windows.Forms.Button buttonDiSubOff;
		private System.Windows.Forms.TextBox textBoxDiSubOut2;
		private System.Windows.Forms.Label label50;
		private System.Windows.Forms.TextBox textBoxDiSubOut1;
		private System.Windows.Forms.Label label51;
		private System.Windows.Forms.TextBox textBoxDiSubOff;
		private System.Windows.Forms.Label label52;
		private System.Windows.Forms.TextBox textBoxDiSubOn;
		private System.Windows.Forms.Label label53;
		private System.Windows.Forms.Button buttonDiSubOn;
		private System.Windows.Forms.Label label54;
		private System.Windows.Forms.TextBox textBoxDiPort;
		private System.Windows.Forms.Label label55;
		private System.Windows.Forms.GroupBox groupBox36;
		private System.Windows.Forms.Button buttonDiAlarmWaveFile;
		private System.Windows.Forms.TextBox textBoxDiAlarmWaveFile;
		private System.Windows.Forms.GroupBox groupBox37;
		private System.Windows.Forms.Label label65;
		private MyNumericUpDown numericUpDownDiConfirmCount;
		private System.Windows.Forms.GroupBox groupBox38;
		private System.Windows.Forms.Label label66;
		private MyNumericUpDown numericUpDownDiScanTime;
		private System.Windows.Forms.GroupBox groupBox40;
		private System.Windows.Forms.Button buttonDiAlarmGraphicFile;
		private System.Windows.Forms.TextBox textBoxDiAlarmGraphicFile;
		private System.Windows.Forms.GroupBox groupBox41;
		private MyNumericUpDown numericUpDownDiAlarmPriority;
		private System.Windows.Forms.CheckBox checkBoxDiReverse;
		private System.Windows.Forms.RadioButton radioButtonDiOutMethod1;
		private System.Windows.Forms.RadioButton radioButtonDiOutMethod0;
		private System.Windows.Forms.GroupBox groupBox21;
		private System.Windows.Forms.RadioButton radioButtonDiAlarmCondition1;
		private System.Windows.Forms.RadioButton radioButtonDiAlarmCondition0;
		private System.Windows.Forms.RadioButton radioButtonDiAlarmCondition2;
		private System.Windows.Forms.RadioButton radioButtonDiAlarmCondition3;
		private System.Windows.Forms.RadioButton radioButtonDiAlarmCondition4;
		private System.Windows.Forms.RadioButton radioButtonDiAlarmCondition5;

		MultiSelectTextBox multiDiPort = new MultiSelectTextBox();
		MultiSelectTextBox multiDiAddressWord = new MultiSelectTextBox();
		MultiSelectTextBox multiDiAddressBit = new MultiSelectTextBox(true);
		MultiSelectTextBox multiDiOnDes = new MultiSelectTextBox();
		MultiSelectTextBox multiDiOffDes = new MultiSelectTextBox();
		MultiSelectTextBox multiDiSubOn = new MultiSelectTextBox();
		MultiSelectTextBox multiDiSubOff = new MultiSelectTextBox();
		MultiSelectTextBox multiDiSubOut1 = new MultiSelectTextBox();
		MultiSelectTextBox multiDiSubOut2 = new MultiSelectTextBox();
		MultiSelectTextBox multiDiAlarmWave = new MultiSelectTextBox();
		MultiSelectTextBox multiDiAlarmGraphic = new MultiSelectTextBox();
		MultiSelectCheckBox multiDiReverse = new MultiSelectCheckBox();
		MultiSelectRadioButton multiDiAlarmCondition = new MultiSelectRadioButton();
		MultiSelectRadioButton multiDiOutputMethod = new MultiSelectRadioButton();
		MultiSelectNumericUpDown multiDiScanTime = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiDiAlarmPriority = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiDiConfirmCount = new MultiSelectNumericUpDown();
		MultiSelectCheckBox multiDiUsingAlarm = new MultiSelectCheckBox();
		MultiSelectCheckBox multiDiDataSave = new MultiSelectCheckBox();

		private System.Windows.Forms.TextBox textBoxDiAddressWord;
		private System.Windows.Forms.TextBox textBoxDiAddressBit;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox checkBoxDiUsingAlarm;
		private System.Windows.Forms.GroupBox groupBoxDiSubOut;
		private System.Windows.Forms.GroupBox groupBoxPlcScan;
		private System.Windows.Forms.CheckBox checkBoxDiDataSave;

		public PropertyTagDI()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// 태그를 생성시킬때 0값이 항상들어가서 아래를 추가하였다.
			this.numericUpDownDiScanTime.Value = TagLib.DEFAULT_SCANTIME;

			// DI
			multiDiPort.Add(this.textBoxDiPort);
			multiDiAddressWord.Add(this.textBoxDiAddressWord);
			multiDiAddressBit.Add(this.textBoxDiAddressBit);
			multiDiOnDes.Add(this.textBoxDiOnDes);
			multiDiOffDes.Add(this.textBoxDiOffDes);
			multiDiSubOn.Add(this.textBoxDiSubOn);
			multiDiSubOff.Add(this.textBoxDiSubOff);
			multiDiSubOut1.Add(this.textBoxDiSubOut1);
			multiDiSubOut2.Add(this.textBoxDiSubOut2);
			multiDiAlarmWave.Add(this.textBoxDiAlarmWaveFile);
			multiDiAlarmGraphic.Add(this.textBoxDiAlarmGraphicFile);
			multiDiReverse.Add(this.checkBoxDiReverse);
			multiDiAlarmCondition.Add(this.radioButtonDiAlarmCondition0, radioButtonDiAlarmCondition1,
				radioButtonDiAlarmCondition2, radioButtonDiAlarmCondition3,
				radioButtonDiAlarmCondition4, radioButtonDiAlarmCondition5);
			multiDiOutputMethod.Add(this.radioButtonDiOutMethod0, radioButtonDiOutMethod1);
			multiDiScanTime.Add(this.numericUpDownDiScanTime);
			multiDiAlarmPriority.Add(this.numericUpDownDiAlarmPriority);
			multiDiConfirmCount.Add(this.numericUpDownDiConfirmCount);

			multiDiUsingAlarm.Add(this.checkBoxDiUsingAlarm);
			multiDiDataSave.Add(this.checkBoxDiDataSave);

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagDI));
            this.checkBoxDiDataSave = new System.Windows.Forms.CheckBox();
            this.checkBoxDiUsingAlarm = new System.Windows.Forms.CheckBox();
            this.checkBoxDiReverse = new System.Windows.Forms.CheckBox();
            this.textBoxDiOffDes = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.textBoxDiOnDes = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.groupBoxDiSubOut = new System.Windows.Forms.GroupBox();
            this.radioButtonDiOutMethod1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDiOutMethod0 = new System.Windows.Forms.RadioButton();
            this.groupBox30 = new System.Windows.Forms.GroupBox();
            this.buttonDiSubOut2 = new System.Windows.Forms.Button();
            this.buttonDiSubOut1 = new System.Windows.Forms.Button();
            this.buttonDiSubOff = new System.Windows.Forms.Button();
            this.textBoxDiSubOut2 = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.textBoxDiSubOut1 = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.textBoxDiSubOff = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.textBoxDiSubOn = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.buttonDiSubOn = new System.Windows.Forms.Button();
            this.groupBoxPlcScan = new System.Windows.Forms.GroupBox();
            this.textBoxDiAddressBit = new System.Windows.Forms.TextBox();
            this.textBoxDiAddressWord = new System.Windows.Forms.TextBox();
            this.label54 = new System.Windows.Forms.Label();
            this.textBoxDiPort = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox36 = new System.Windows.Forms.GroupBox();
            this.buttonDiAlarmWaveFile = new System.Windows.Forms.Button();
            this.textBoxDiAlarmWaveFile = new System.Windows.Forms.TextBox();
            this.groupBox37 = new System.Windows.Forms.GroupBox();
            this.label65 = new System.Windows.Forms.Label();
            this.numericUpDownDiConfirmCount = new MyNumericUpDown();
            this.groupBox38 = new System.Windows.Forms.GroupBox();
            this.label66 = new System.Windows.Forms.Label();
            this.numericUpDownDiScanTime = new MyNumericUpDown();
            this.groupBox40 = new System.Windows.Forms.GroupBox();
            this.buttonDiAlarmGraphicFile = new System.Windows.Forms.Button();
            this.textBoxDiAlarmGraphicFile = new System.Windows.Forms.TextBox();
            this.groupBox41 = new System.Windows.Forms.GroupBox();
            this.numericUpDownDiAlarmPriority = new MyNumericUpDown();
            this.groupBox21 = new System.Windows.Forms.GroupBox();
            this.radioButtonDiAlarmCondition5 = new System.Windows.Forms.RadioButton();
            this.radioButtonDiAlarmCondition4 = new System.Windows.Forms.RadioButton();
            this.radioButtonDiAlarmCondition3 = new System.Windows.Forms.RadioButton();
            this.radioButtonDiAlarmCondition2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDiAlarmCondition1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDiAlarmCondition0 = new System.Windows.Forms.RadioButton();
            this.groupBoxDiSubOut.SuspendLayout();
            this.groupBox30.SuspendLayout();
            this.groupBoxPlcScan.SuspendLayout();
            this.groupBox36.SuspendLayout();
            this.groupBox37.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiConfirmCount)).BeginInit();
            this.groupBox38.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiScanTime)).BeginInit();
            this.groupBox40.SuspendLayout();
            this.groupBox41.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiAlarmPriority)).BeginInit();
            this.groupBox21.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxDiDataSave
            // 
            this.checkBoxDiDataSave.AccessibleDescription = null;
            this.checkBoxDiDataSave.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDiDataSave, "checkBoxDiDataSave");
            this.checkBoxDiDataSave.BackgroundImage = null;
            this.checkBoxDiDataSave.Font = null;
            this.checkBoxDiDataSave.Name = "checkBoxDiDataSave";
            // 
            // checkBoxDiUsingAlarm
            // 
            this.checkBoxDiUsingAlarm.AccessibleDescription = null;
            this.checkBoxDiUsingAlarm.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDiUsingAlarm, "checkBoxDiUsingAlarm");
            this.checkBoxDiUsingAlarm.BackgroundImage = null;
            this.checkBoxDiUsingAlarm.Font = null;
            this.checkBoxDiUsingAlarm.Name = "checkBoxDiUsingAlarm";
            // 
            // checkBoxDiReverse
            // 
            this.checkBoxDiReverse.AccessibleDescription = null;
            this.checkBoxDiReverse.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDiReverse, "checkBoxDiReverse");
            this.checkBoxDiReverse.BackgroundImage = null;
            this.checkBoxDiReverse.Font = null;
            this.checkBoxDiReverse.Name = "checkBoxDiReverse";
            // 
            // textBoxDiOffDes
            // 
            this.textBoxDiOffDes.AccessibleDescription = null;
            this.textBoxDiOffDes.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiOffDes, "textBoxDiOffDes");
            this.textBoxDiOffDes.BackgroundImage = null;
            this.textBoxDiOffDes.Font = null;
            this.textBoxDiOffDes.Name = "textBoxDiOffDes";
            // 
            // label41
            // 
            this.label41.AccessibleDescription = null;
            this.label41.AccessibleName = null;
            resources.ApplyResources(this.label41, "label41");
            this.label41.Font = null;
            this.label41.Name = "label41";
            // 
            // textBoxDiOnDes
            // 
            this.textBoxDiOnDes.AccessibleDescription = null;
            this.textBoxDiOnDes.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiOnDes, "textBoxDiOnDes");
            this.textBoxDiOnDes.BackgroundImage = null;
            this.textBoxDiOnDes.Font = null;
            this.textBoxDiOnDes.Name = "textBoxDiOnDes";
            // 
            // label42
            // 
            this.label42.AccessibleDescription = null;
            this.label42.AccessibleName = null;
            resources.ApplyResources(this.label42, "label42");
            this.label42.Font = null;
            this.label42.Name = "label42";
            // 
            // groupBoxDiSubOut
            // 
            this.groupBoxDiSubOut.AccessibleDescription = null;
            this.groupBoxDiSubOut.AccessibleName = null;
            resources.ApplyResources(this.groupBoxDiSubOut, "groupBoxDiSubOut");
            this.groupBoxDiSubOut.BackgroundImage = null;
            this.groupBoxDiSubOut.Controls.Add(this.radioButtonDiOutMethod1);
            this.groupBoxDiSubOut.Controls.Add(this.radioButtonDiOutMethod0);
            this.groupBoxDiSubOut.Font = null;
            this.groupBoxDiSubOut.Name = "groupBoxDiSubOut";
            this.groupBoxDiSubOut.TabStop = false;
            // 
            // radioButtonDiOutMethod1
            // 
            this.radioButtonDiOutMethod1.AccessibleDescription = null;
            this.radioButtonDiOutMethod1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiOutMethod1, "radioButtonDiOutMethod1");
            this.radioButtonDiOutMethod1.BackgroundImage = null;
            this.radioButtonDiOutMethod1.Font = null;
            this.radioButtonDiOutMethod1.Name = "radioButtonDiOutMethod1";
            // 
            // radioButtonDiOutMethod0
            // 
            this.radioButtonDiOutMethod0.AccessibleDescription = null;
            this.radioButtonDiOutMethod0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiOutMethod0, "radioButtonDiOutMethod0");
            this.radioButtonDiOutMethod0.BackgroundImage = null;
            this.radioButtonDiOutMethod0.Font = null;
            this.radioButtonDiOutMethod0.Name = "radioButtonDiOutMethod0";
            // 
            // groupBox30
            // 
            this.groupBox30.AccessibleDescription = null;
            this.groupBox30.AccessibleName = null;
            resources.ApplyResources(this.groupBox30, "groupBox30");
            this.groupBox30.BackgroundImage = null;
            this.groupBox30.Controls.Add(this.buttonDiSubOut2);
            this.groupBox30.Controls.Add(this.buttonDiSubOut1);
            this.groupBox30.Controls.Add(this.buttonDiSubOff);
            this.groupBox30.Controls.Add(this.textBoxDiSubOut2);
            this.groupBox30.Controls.Add(this.label50);
            this.groupBox30.Controls.Add(this.textBoxDiSubOut1);
            this.groupBox30.Controls.Add(this.label51);
            this.groupBox30.Controls.Add(this.textBoxDiSubOff);
            this.groupBox30.Controls.Add(this.label52);
            this.groupBox30.Controls.Add(this.textBoxDiSubOn);
            this.groupBox30.Controls.Add(this.label53);
            this.groupBox30.Controls.Add(this.buttonDiSubOn);
            this.groupBox30.Font = null;
            this.groupBox30.Name = "groupBox30";
            this.groupBox30.TabStop = false;
            // 
            // buttonDiSubOut2
            // 
            this.buttonDiSubOut2.AccessibleDescription = null;
            this.buttonDiSubOut2.AccessibleName = null;
            resources.ApplyResources(this.buttonDiSubOut2, "buttonDiSubOut2");
            this.buttonDiSubOut2.BackgroundImage = null;
            this.buttonDiSubOut2.Font = null;
            this.buttonDiSubOut2.Name = "buttonDiSubOut2";
            this.buttonDiSubOut2.Click += new System.EventHandler(this.buttonDiSubOut2_Click);
            // 
            // buttonDiSubOut1
            // 
            this.buttonDiSubOut1.AccessibleDescription = null;
            this.buttonDiSubOut1.AccessibleName = null;
            resources.ApplyResources(this.buttonDiSubOut1, "buttonDiSubOut1");
            this.buttonDiSubOut1.BackgroundImage = null;
            this.buttonDiSubOut1.Font = null;
            this.buttonDiSubOut1.Name = "buttonDiSubOut1";
            this.buttonDiSubOut1.Click += new System.EventHandler(this.buttonDiSubOut1_Click);
            // 
            // buttonDiSubOff
            // 
            this.buttonDiSubOff.AccessibleDescription = null;
            this.buttonDiSubOff.AccessibleName = null;
            resources.ApplyResources(this.buttonDiSubOff, "buttonDiSubOff");
            this.buttonDiSubOff.BackgroundImage = null;
            this.buttonDiSubOff.Font = null;
            this.buttonDiSubOff.Name = "buttonDiSubOff";
            this.buttonDiSubOff.Click += new System.EventHandler(this.buttonDiSubOff_Click);
            // 
            // textBoxDiSubOut2
            // 
            this.textBoxDiSubOut2.AccessibleDescription = null;
            this.textBoxDiSubOut2.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiSubOut2, "textBoxDiSubOut2");
            this.textBoxDiSubOut2.BackgroundImage = null;
            this.textBoxDiSubOut2.Font = null;
            this.textBoxDiSubOut2.Name = "textBoxDiSubOut2";
            // 
            // label50
            // 
            this.label50.AccessibleDescription = null;
            this.label50.AccessibleName = null;
            resources.ApplyResources(this.label50, "label50");
            this.label50.Font = null;
            this.label50.Name = "label50";
            // 
            // textBoxDiSubOut1
            // 
            this.textBoxDiSubOut1.AccessibleDescription = null;
            this.textBoxDiSubOut1.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiSubOut1, "textBoxDiSubOut1");
            this.textBoxDiSubOut1.BackgroundImage = null;
            this.textBoxDiSubOut1.Font = null;
            this.textBoxDiSubOut1.Name = "textBoxDiSubOut1";
            // 
            // label51
            // 
            this.label51.AccessibleDescription = null;
            this.label51.AccessibleName = null;
            resources.ApplyResources(this.label51, "label51");
            this.label51.Font = null;
            this.label51.Name = "label51";
            // 
            // textBoxDiSubOff
            // 
            this.textBoxDiSubOff.AccessibleDescription = null;
            this.textBoxDiSubOff.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiSubOff, "textBoxDiSubOff");
            this.textBoxDiSubOff.BackgroundImage = null;
            this.textBoxDiSubOff.Font = null;
            this.textBoxDiSubOff.Name = "textBoxDiSubOff";
            // 
            // label52
            // 
            this.label52.AccessibleDescription = null;
            this.label52.AccessibleName = null;
            resources.ApplyResources(this.label52, "label52");
            this.label52.Font = null;
            this.label52.Name = "label52";
            // 
            // textBoxDiSubOn
            // 
            this.textBoxDiSubOn.AccessibleDescription = null;
            this.textBoxDiSubOn.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiSubOn, "textBoxDiSubOn");
            this.textBoxDiSubOn.BackgroundImage = null;
            this.textBoxDiSubOn.Font = null;
            this.textBoxDiSubOn.Name = "textBoxDiSubOn";
            // 
            // label53
            // 
            this.label53.AccessibleDescription = null;
            this.label53.AccessibleName = null;
            resources.ApplyResources(this.label53, "label53");
            this.label53.Font = null;
            this.label53.Name = "label53";
            // 
            // buttonDiSubOn
            // 
            this.buttonDiSubOn.AccessibleDescription = null;
            this.buttonDiSubOn.AccessibleName = null;
            resources.ApplyResources(this.buttonDiSubOn, "buttonDiSubOn");
            this.buttonDiSubOn.BackgroundImage = null;
            this.buttonDiSubOn.Font = null;
            this.buttonDiSubOn.Name = "buttonDiSubOn";
            this.buttonDiSubOn.Click += new System.EventHandler(this.buttonDiSubOn_Click);
            // 
            // groupBoxPlcScan
            // 
            this.groupBoxPlcScan.AccessibleDescription = null;
            this.groupBoxPlcScan.AccessibleName = null;
            resources.ApplyResources(this.groupBoxPlcScan, "groupBoxPlcScan");
            this.groupBoxPlcScan.BackgroundImage = null;
            this.groupBoxPlcScan.Controls.Add(this.textBoxDiAddressBit);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDiAddressWord);
            this.groupBoxPlcScan.Controls.Add(this.label54);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDiPort);
            this.groupBoxPlcScan.Controls.Add(this.label55);
            this.groupBoxPlcScan.Controls.Add(this.label19);
            this.groupBoxPlcScan.Controls.Add(this.checkBoxDiReverse);
            this.groupBoxPlcScan.Font = null;
            this.groupBoxPlcScan.Name = "groupBoxPlcScan";
            this.groupBoxPlcScan.TabStop = false;
            // 
            // textBoxDiAddressBit
            // 
            this.textBoxDiAddressBit.AccessibleDescription = null;
            this.textBoxDiAddressBit.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiAddressBit, "textBoxDiAddressBit");
            this.textBoxDiAddressBit.BackgroundImage = null;
            this.textBoxDiAddressBit.Font = null;
            this.textBoxDiAddressBit.Name = "textBoxDiAddressBit";
            // 
            // textBoxDiAddressWord
            // 
            this.textBoxDiAddressWord.AccessibleDescription = null;
            this.textBoxDiAddressWord.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiAddressWord, "textBoxDiAddressWord");
            this.textBoxDiAddressWord.BackgroundImage = null;
            this.textBoxDiAddressWord.Font = null;
            this.textBoxDiAddressWord.Name = "textBoxDiAddressWord";
            // 
            // label54
            // 
            this.label54.AccessibleDescription = null;
            this.label54.AccessibleName = null;
            resources.ApplyResources(this.label54, "label54");
            this.label54.Font = null;
            this.label54.Name = "label54";
            // 
            // textBoxDiPort
            // 
            this.textBoxDiPort.AccessibleDescription = null;
            this.textBoxDiPort.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiPort, "textBoxDiPort");
            this.textBoxDiPort.BackgroundImage = null;
            this.textBoxDiPort.Font = null;
            this.textBoxDiPort.Name = "textBoxDiPort";
            // 
            // label55
            // 
            this.label55.AccessibleDescription = null;
            this.label55.AccessibleName = null;
            resources.ApplyResources(this.label55, "label55");
            this.label55.Font = null;
            this.label55.Name = "label55";
            // 
            // label19
            // 
            this.label19.AccessibleDescription = null;
            this.label19.AccessibleName = null;
            resources.ApplyResources(this.label19, "label19");
            this.label19.Font = null;
            this.label19.Name = "label19";
            // 
            // groupBox36
            // 
            this.groupBox36.AccessibleDescription = null;
            this.groupBox36.AccessibleName = null;
            resources.ApplyResources(this.groupBox36, "groupBox36");
            this.groupBox36.BackgroundImage = null;
            this.groupBox36.Controls.Add(this.buttonDiAlarmWaveFile);
            this.groupBox36.Controls.Add(this.textBoxDiAlarmWaveFile);
            this.groupBox36.Font = null;
            this.groupBox36.Name = "groupBox36";
            this.groupBox36.TabStop = false;
            // 
            // buttonDiAlarmWaveFile
            // 
            this.buttonDiAlarmWaveFile.AccessibleDescription = null;
            this.buttonDiAlarmWaveFile.AccessibleName = null;
            resources.ApplyResources(this.buttonDiAlarmWaveFile, "buttonDiAlarmWaveFile");
            this.buttonDiAlarmWaveFile.BackgroundImage = null;
            this.buttonDiAlarmWaveFile.Font = null;
            this.buttonDiAlarmWaveFile.Name = "buttonDiAlarmWaveFile";
            this.buttonDiAlarmWaveFile.Click += new System.EventHandler(this.buttonDiAlarmWaveFile_Click);
            // 
            // textBoxDiAlarmWaveFile
            // 
            this.textBoxDiAlarmWaveFile.AccessibleDescription = null;
            this.textBoxDiAlarmWaveFile.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiAlarmWaveFile, "textBoxDiAlarmWaveFile");
            this.textBoxDiAlarmWaveFile.BackgroundImage = null;
            this.textBoxDiAlarmWaveFile.Font = null;
            this.textBoxDiAlarmWaveFile.Name = "textBoxDiAlarmWaveFile";
            // 
            // groupBox37
            // 
            this.groupBox37.AccessibleDescription = null;
            this.groupBox37.AccessibleName = null;
            resources.ApplyResources(this.groupBox37, "groupBox37");
            this.groupBox37.BackgroundImage = null;
            this.groupBox37.Controls.Add(this.label65);
            this.groupBox37.Controls.Add(this.numericUpDownDiConfirmCount);
            this.groupBox37.Font = null;
            this.groupBox37.Name = "groupBox37";
            this.groupBox37.TabStop = false;
            // 
            // label65
            // 
            this.label65.AccessibleDescription = null;
            this.label65.AccessibleName = null;
            resources.ApplyResources(this.label65, "label65");
            this.label65.Font = null;
            this.label65.Name = "label65";
            // 
            // numericUpDownDiConfirmCount
            // 
            this.numericUpDownDiConfirmCount.AccessibleDescription = null;
            this.numericUpDownDiConfirmCount.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDiConfirmCount, "numericUpDownDiConfirmCount");
            this.numericUpDownDiConfirmCount.Font = null;
            this.numericUpDownDiConfirmCount.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownDiConfirmCount.Name = "numericUpDownDiConfirmCount";
            // 
            // groupBox38
            // 
            this.groupBox38.AccessibleDescription = null;
            this.groupBox38.AccessibleName = null;
            resources.ApplyResources(this.groupBox38, "groupBox38");
            this.groupBox38.BackgroundImage = null;
            this.groupBox38.Controls.Add(this.label66);
            this.groupBox38.Controls.Add(this.numericUpDownDiScanTime);
            this.groupBox38.Font = null;
            this.groupBox38.Name = "groupBox38";
            this.groupBox38.TabStop = false;
            // 
            // label66
            // 
            this.label66.AccessibleDescription = null;
            this.label66.AccessibleName = null;
            resources.ApplyResources(this.label66, "label66");
            this.label66.Font = null;
            this.label66.Name = "label66";
            // 
            // numericUpDownDiScanTime
            // 
            this.numericUpDownDiScanTime.AccessibleDescription = null;
            this.numericUpDownDiScanTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDiScanTime, "numericUpDownDiScanTime");
            this.numericUpDownDiScanTime.Font = null;
            this.numericUpDownDiScanTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownDiScanTime.Name = "numericUpDownDiScanTime";
            this.numericUpDownDiScanTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox40
            // 
            this.groupBox40.AccessibleDescription = null;
            this.groupBox40.AccessibleName = null;
            resources.ApplyResources(this.groupBox40, "groupBox40");
            this.groupBox40.BackgroundImage = null;
            this.groupBox40.Controls.Add(this.buttonDiAlarmGraphicFile);
            this.groupBox40.Controls.Add(this.textBoxDiAlarmGraphicFile);
            this.groupBox40.Font = null;
            this.groupBox40.Name = "groupBox40";
            this.groupBox40.TabStop = false;
            // 
            // buttonDiAlarmGraphicFile
            // 
            this.buttonDiAlarmGraphicFile.AccessibleDescription = null;
            this.buttonDiAlarmGraphicFile.AccessibleName = null;
            resources.ApplyResources(this.buttonDiAlarmGraphicFile, "buttonDiAlarmGraphicFile");
            this.buttonDiAlarmGraphicFile.BackgroundImage = null;
            this.buttonDiAlarmGraphicFile.Font = null;
            this.buttonDiAlarmGraphicFile.Name = "buttonDiAlarmGraphicFile";
            this.buttonDiAlarmGraphicFile.Click += new System.EventHandler(this.buttonDiAlarmGraphicFile_Click);
            // 
            // textBoxDiAlarmGraphicFile
            // 
            this.textBoxDiAlarmGraphicFile.AccessibleDescription = null;
            this.textBoxDiAlarmGraphicFile.AccessibleName = null;
            resources.ApplyResources(this.textBoxDiAlarmGraphicFile, "textBoxDiAlarmGraphicFile");
            this.textBoxDiAlarmGraphicFile.BackgroundImage = null;
            this.textBoxDiAlarmGraphicFile.Font = null;
            this.textBoxDiAlarmGraphicFile.Name = "textBoxDiAlarmGraphicFile";
            // 
            // groupBox41
            // 
            this.groupBox41.AccessibleDescription = null;
            this.groupBox41.AccessibleName = null;
            resources.ApplyResources(this.groupBox41, "groupBox41");
            this.groupBox41.BackgroundImage = null;
            this.groupBox41.Controls.Add(this.numericUpDownDiAlarmPriority);
            this.groupBox41.Font = null;
            this.groupBox41.Name = "groupBox41";
            this.groupBox41.TabStop = false;
            // 
            // numericUpDownDiAlarmPriority
            // 
            this.numericUpDownDiAlarmPriority.AccessibleDescription = null;
            this.numericUpDownDiAlarmPriority.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDiAlarmPriority, "numericUpDownDiAlarmPriority");
            this.numericUpDownDiAlarmPriority.Font = null;
            this.numericUpDownDiAlarmPriority.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownDiAlarmPriority.Name = "numericUpDownDiAlarmPriority";
            // 
            // groupBox21
            // 
            this.groupBox21.AccessibleDescription = null;
            this.groupBox21.AccessibleName = null;
            resources.ApplyResources(this.groupBox21, "groupBox21");
            this.groupBox21.BackgroundImage = null;
            this.groupBox21.Controls.Add(this.radioButtonDiAlarmCondition5);
            this.groupBox21.Controls.Add(this.radioButtonDiAlarmCondition4);
            this.groupBox21.Controls.Add(this.radioButtonDiAlarmCondition3);
            this.groupBox21.Controls.Add(this.radioButtonDiAlarmCondition2);
            this.groupBox21.Controls.Add(this.radioButtonDiAlarmCondition1);
            this.groupBox21.Controls.Add(this.radioButtonDiAlarmCondition0);
            this.groupBox21.Font = null;
            this.groupBox21.Name = "groupBox21";
            this.groupBox21.TabStop = false;
            // 
            // radioButtonDiAlarmCondition5
            // 
            this.radioButtonDiAlarmCondition5.AccessibleDescription = null;
            this.radioButtonDiAlarmCondition5.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiAlarmCondition5, "radioButtonDiAlarmCondition5");
            this.radioButtonDiAlarmCondition5.BackgroundImage = null;
            this.radioButtonDiAlarmCondition5.Font = null;
            this.radioButtonDiAlarmCondition5.Name = "radioButtonDiAlarmCondition5";
            // 
            // radioButtonDiAlarmCondition4
            // 
            this.radioButtonDiAlarmCondition4.AccessibleDescription = null;
            this.radioButtonDiAlarmCondition4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiAlarmCondition4, "radioButtonDiAlarmCondition4");
            this.radioButtonDiAlarmCondition4.BackgroundImage = null;
            this.radioButtonDiAlarmCondition4.Font = null;
            this.radioButtonDiAlarmCondition4.Name = "radioButtonDiAlarmCondition4";
            // 
            // radioButtonDiAlarmCondition3
            // 
            this.radioButtonDiAlarmCondition3.AccessibleDescription = null;
            this.radioButtonDiAlarmCondition3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiAlarmCondition3, "radioButtonDiAlarmCondition3");
            this.radioButtonDiAlarmCondition3.BackgroundImage = null;
            this.radioButtonDiAlarmCondition3.Font = null;
            this.radioButtonDiAlarmCondition3.Name = "radioButtonDiAlarmCondition3";
            // 
            // radioButtonDiAlarmCondition2
            // 
            this.radioButtonDiAlarmCondition2.AccessibleDescription = null;
            this.radioButtonDiAlarmCondition2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiAlarmCondition2, "radioButtonDiAlarmCondition2");
            this.radioButtonDiAlarmCondition2.BackgroundImage = null;
            this.radioButtonDiAlarmCondition2.Font = null;
            this.radioButtonDiAlarmCondition2.Name = "radioButtonDiAlarmCondition2";
            // 
            // radioButtonDiAlarmCondition1
            // 
            this.radioButtonDiAlarmCondition1.AccessibleDescription = null;
            this.radioButtonDiAlarmCondition1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiAlarmCondition1, "radioButtonDiAlarmCondition1");
            this.radioButtonDiAlarmCondition1.BackgroundImage = null;
            this.radioButtonDiAlarmCondition1.Font = null;
            this.radioButtonDiAlarmCondition1.Name = "radioButtonDiAlarmCondition1";
            // 
            // radioButtonDiAlarmCondition0
            // 
            this.radioButtonDiAlarmCondition0.AccessibleDescription = null;
            this.radioButtonDiAlarmCondition0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDiAlarmCondition0, "radioButtonDiAlarmCondition0");
            this.radioButtonDiAlarmCondition0.BackgroundImage = null;
            this.radioButtonDiAlarmCondition0.Font = null;
            this.radioButtonDiAlarmCondition0.Name = "radioButtonDiAlarmCondition0";
            // 
            // PropertyTagDI
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBoxPlcScan);
            this.Controls.Add(this.textBoxDiOffDes);
            this.Controls.Add(this.textBoxDiOnDes);
            this.Controls.Add(this.groupBox40);
            this.Controls.Add(this.groupBox21);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.groupBox37);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.groupBoxDiSubOut);
            this.Controls.Add(this.groupBox41);
            this.Controls.Add(this.groupBox36);
            this.Controls.Add(this.groupBox38);
            this.Controls.Add(this.checkBoxDiDataSave);
            this.Controls.Add(this.groupBox30);
            this.Controls.Add(this.checkBoxDiUsingAlarm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagDI";
            this.ShowInTaskbar = false;
            this.groupBoxDiSubOut.ResumeLayout(false);
            this.groupBox30.ResumeLayout(false);
            this.groupBox30.PerformLayout();
            this.groupBoxPlcScan.ResumeLayout(false);
            this.groupBoxPlcScan.PerformLayout();
            this.groupBox36.ResumeLayout(false);
            this.groupBox36.PerformLayout();
            this.groupBox37.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiConfirmCount)).EndInit();
            this.groupBox38.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiScanTime)).EndInit();
            this.groupBox40.ResumeLayout(false);
            this.groupBox40.PerformLayout();
            this.groupBox41.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDiAlarmPriority)).EndInit();
            this.groupBox21.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void SetTagDI(TagDiClass di)
		{
			multiDiPort.Set(di.port);
			multiDiAddressWord.Set(di.address_word);
			multiDiAddressBit.Set(di.address_bit);
			multiDiOnDes.Set(di.desON);
			multiDiOffDes.Set(di.desOFF);
			multiDiSubOn.Set(di.sSubOutDigitalOnTag);
			multiDiSubOff.Set(di.sSubOutDigitalOffTag);
			multiDiSubOut1.Set(di.sSubOutDigital1);
			multiDiSubOut2.Set(di.sSubOutDigital2);
			multiDiAlarmWave.Set(di.sAlarmWaveFile);
			multiDiAlarmGraphic.Set(di.sGraphicFile);
			multiDiReverse.Set(di.bReverse);
			multiDiAlarmCondition.Set(di.cAlarmType);
			multiDiOutputMethod.Set(di.cOutLinkMethod);
			multiDiScanTime.Set(di.wScanTime);
			multiDiAlarmPriority.Set(di.wAlarmPriority);
			multiDiConfirmCount.Set(di.cConfirmCount);
			multiDiUsingAlarm.Set(di.alarm);
			multiDiDataSave.Set(di.bFileSave);

		}

		public void GetTagDI(TagDiClass di)
		{
			multiDiPort.Get(ref di.port);
			multiDiAddressWord.Get(ref di.address_word);
			multiDiAddressBit.Get(ref di.address_bit);
			multiDiOnDes.Get(ref di.desON);
			multiDiOffDes.Get(ref di.desOFF);
			multiDiSubOn.Get(ref di.sSubOutDigitalOnTag);
			multiDiSubOff.Get(ref di.sSubOutDigitalOffTag);
			multiDiSubOut1.Get(ref di.sSubOutDigital1);
			multiDiSubOut2.Get(ref di.sSubOutDigital2);
			multiDiAlarmWave.Get(ref di.sAlarmWaveFile);
			multiDiAlarmGraphic.Get(ref di.sGraphicFile);
			multiDiReverse.Get(ref di.bReverse);
			multiDiAlarmCondition.Get(ref di.cAlarmType);
			multiDiOutputMethod.Get(ref di.cOutLinkMethod);
			multiDiScanTime.Get(ref di.wScanTime);
			multiDiAlarmPriority.Get(ref di.wAlarmPriority);
			multiDiConfirmCount.Get(ref di.cConfirmCount);
			multiDiUsingAlarm.Get(ref di.alarm);
			multiDiDataSave.Get(ref di.bFileSave);

		}

		private void buttonDiSubOn_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDoGdo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxDiSubOn.Text = tag;
			}
		}

		private void buttonDiSubOff_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDoGdo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxDiSubOff.Text = tag;
			}
		}

		private void buttonDiSubOut1_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDoGdo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxDiSubOut1.Text = tag;
			}
		}

		private void buttonDiSubOut2_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDoGdo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxDiSubOut2.Text = tag;
			}
		}

		private void buttonDiAlarmWaveFile_Click(object sender, System.EventArgs e)
		{
			string filename;

			if(FormTagProperty.SelectAlarmWaveFile(out filename)) 
			{
				this.textBoxDiAlarmWaveFile.Text = filename;
			}		
		}

		private void buttonDiAlarmGraphicFile_Click(object sender, System.EventArgs e)
		{
			string filename;

			if(FormTagProperty.SelectAlarmModuleFile(out filename)) 
			{
				this.textBoxDiAlarmGraphicFile.Text = filename;
			}			
		}

		public void EnableDisableConnectionType(int type)
		{
			bool flag_out = true;
			bool flag_plcscan = true;

			if(type == 0) 
			{
				
			}
			else if(type == 1) 
			{
				flag_plcscan = false;
			}
			else if(type == 2) 
			{
				flag_out = false;
				flag_plcscan = false;
			}
			else if(type == 3) 
			{
				flag_plcscan = false;	
			}
			else if(type == 4) 
			{
				flag_plcscan = false;
			}
			else if(type == 5) 
			{
				flag_plcscan = false;
			}
			
			this.textBoxDiSubOut1.Enabled = flag_out;
			this.textBoxDiSubOut2.Enabled = flag_out;
			this.buttonDiSubOut1.Enabled = flag_out;
			this.buttonDiSubOut2.Enabled = flag_out;
			this.groupBoxDiSubOut.Enabled = flag_out;

			this.groupBoxPlcScan.Enabled = flag_plcscan;

		}

	}
}
