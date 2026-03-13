using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using DatabaseConnection;

namespace Studio
{
	/// <summary>
	/// Summary description for FormConfigDemandControlAdd.
	/// </summary>
	public class FormConfigDemandControlAdd : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.TextBox textBoxKWIN;
		private System.Windows.Forms.Button buttonKWIN;
		private System.Windows.Forms.Button buttonKwTarget;
		private System.Windows.Forms.TextBox textBoxKwTarget;
		private System.Windows.Forms.NumericUpDown numericUpDownControlTime;
		private System.Windows.Forms.TextBox textBoxPulseRatio;
		private System.Windows.Forms.Button buttonStartTarget;
		private System.Windows.Forms.TextBox textBoxStartTarget;
		private System.Windows.Forms.CheckBox checkBoxUseEoi;
		private System.Windows.Forms.Button buttonTagEoi;
		private System.Windows.Forms.TextBox textBoxTagEoi;
		private System.Windows.Forms.CheckBox checkBoxUseBreaker;
		private System.Windows.Forms.NumericUpDown numericUpDownProtectTime;
		private System.Windows.Forms.Button buttonTagEcho;
		private System.Windows.Forms.TextBox textBoxTagEcho;
		private System.Windows.Forms.RadioButton radioButtonOutputType0;
		private System.Windows.Forms.RadioButton radioButtonOutputType1;
		private System.Windows.Forms.Button buttonTagOut2;
		private System.Windows.Forms.TextBox textBoxTagOut2;
		private System.Windows.Forms.Button buttonTagOut;
		private System.Windows.Forms.TextBox textBoxTagOut;
		private System.Windows.Forms.CheckBox checkBoxAutoReset;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.NumericUpDown numericUpDownDisplayY;
		private System.Windows.Forms.Button buttonPredictionDisplayTag;
		private System.Windows.Forms.TextBox textBoxPredictionDisplayTag;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Button buttonPredictionInputTag;
		private System.Windows.Forms.TextBox textBoxPredictionInputTag;
        private CheckBox checkBoxSaveToDatabase;
        private Button buttonDsn;
        private Label label14;
        private ComboBox comboBoxDsn;
        private GroupBox groupBox8;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigDemandControlAdd()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigDemandControlAdd));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxKWIN = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonKWIN = new System.Windows.Forms.Button();
            this.buttonKwTarget = new System.Windows.Forms.Button();
            this.textBoxKwTarget = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownControlTime = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPulseRatio = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseEoi = new System.Windows.Forms.CheckBox();
            this.buttonTagEoi = new System.Windows.Forms.Button();
            this.textBoxTagEoi = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonTagOut2 = new System.Windows.Forms.Button();
            this.textBoxTagOut2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonTagOut = new System.Windows.Forms.Button();
            this.textBoxTagOut = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.radioButtonOutputType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonOutputType0 = new System.Windows.Forms.RadioButton();
            this.buttonTagEcho = new System.Windows.Forms.Button();
            this.textBoxTagEcho = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownProtectTime = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxUseBreaker = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonStartTarget = new System.Windows.Forms.Button();
            this.textBoxStartTarget = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonPredictionDisplayTag = new System.Windows.Forms.Button();
            this.textBoxPredictionDisplayTag = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDownDisplayY = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoReset = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.buttonPredictionInputTag = new System.Windows.Forms.Button();
            this.textBoxPredictionInputTag = new System.Windows.Forms.TextBox();
            this.checkBoxSaveToDatabase = new System.Windows.Forms.CheckBox();
            this.buttonDsn = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxDsn = new System.Windows.Forms.ComboBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownControlTime)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProtectTime)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDisplayY)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.AccessibleDescription = null;
            this.textBoxTitle.AccessibleName = null;
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackgroundImage = null;
            this.textBoxTitle.Font = null;
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // textBoxKWIN
            // 
            this.textBoxKWIN.AccessibleDescription = null;
            this.textBoxKWIN.AccessibleName = null;
            resources.ApplyResources(this.textBoxKWIN, "textBoxKWIN");
            this.textBoxKWIN.BackgroundImage = null;
            this.textBoxKWIN.Font = null;
            this.textBoxKWIN.Name = "textBoxKWIN";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // buttonKWIN
            // 
            this.buttonKWIN.AccessibleDescription = null;
            this.buttonKWIN.AccessibleName = null;
            resources.ApplyResources(this.buttonKWIN, "buttonKWIN");
            this.buttonKWIN.BackgroundImage = null;
            this.buttonKWIN.Font = null;
            this.buttonKWIN.Name = "buttonKWIN";
            this.buttonKWIN.Click += new System.EventHandler(this.buttonKWIN_Click);
            // 
            // buttonKwTarget
            // 
            this.buttonKwTarget.AccessibleDescription = null;
            this.buttonKwTarget.AccessibleName = null;
            resources.ApplyResources(this.buttonKwTarget, "buttonKwTarget");
            this.buttonKwTarget.BackgroundImage = null;
            this.buttonKwTarget.Font = null;
            this.buttonKwTarget.Name = "buttonKwTarget";
            this.buttonKwTarget.Click += new System.EventHandler(this.buttonKwTarget_Click);
            // 
            // textBoxKwTarget
            // 
            this.textBoxKwTarget.AccessibleDescription = null;
            this.textBoxKwTarget.AccessibleName = null;
            resources.ApplyResources(this.textBoxKwTarget, "textBoxKwTarget");
            this.textBoxKwTarget.BackgroundImage = null;
            this.textBoxKwTarget.Font = null;
            this.textBoxKwTarget.Name = "textBoxKwTarget";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // numericUpDownControlTime
            // 
            this.numericUpDownControlTime.AccessibleDescription = null;
            this.numericUpDownControlTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownControlTime, "numericUpDownControlTime");
            this.numericUpDownControlTime.Font = null;
            this.numericUpDownControlTime.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownControlTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownControlTime.Name = "numericUpDownControlTime";
            this.numericUpDownControlTime.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // textBoxPulseRatio
            // 
            this.textBoxPulseRatio.AccessibleDescription = null;
            this.textBoxPulseRatio.AccessibleName = null;
            resources.ApplyResources(this.textBoxPulseRatio, "textBoxPulseRatio");
            this.textBoxPulseRatio.BackgroundImage = null;
            this.textBoxPulseRatio.Font = null;
            this.textBoxPulseRatio.Name = "textBoxPulseRatio";
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
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.checkBoxUseEoi);
            this.groupBox1.Controls.Add(this.buttonTagEoi);
            this.groupBox1.Controls.Add(this.textBoxTagEoi);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxUseEoi
            // 
            this.checkBoxUseEoi.AccessibleDescription = null;
            this.checkBoxUseEoi.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseEoi, "checkBoxUseEoi");
            this.checkBoxUseEoi.BackgroundImage = null;
            this.checkBoxUseEoi.Font = null;
            this.checkBoxUseEoi.Name = "checkBoxUseEoi";
            this.checkBoxUseEoi.CheckedChanged += new System.EventHandler(this.checkBoxUseEoi_CheckedChanged);
            // 
            // buttonTagEoi
            // 
            this.buttonTagEoi.AccessibleDescription = null;
            this.buttonTagEoi.AccessibleName = null;
            resources.ApplyResources(this.buttonTagEoi, "buttonTagEoi");
            this.buttonTagEoi.BackgroundImage = null;
            this.buttonTagEoi.Font = null;
            this.buttonTagEoi.Name = "buttonTagEoi";
            this.buttonTagEoi.Click += new System.EventHandler(this.buttonTagEoi_Click);
            // 
            // textBoxTagEoi
            // 
            this.textBoxTagEoi.AccessibleDescription = null;
            this.textBoxTagEoi.AccessibleName = null;
            resources.ApplyResources(this.textBoxTagEoi, "textBoxTagEoi");
            this.textBoxTagEoi.BackgroundImage = null;
            this.textBoxTagEoi.Font = null;
            this.textBoxTagEoi.Name = "textBoxTagEoi";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.buttonTagEcho);
            this.groupBox2.Controls.Add(this.textBoxTagEcho);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDownProtectTime);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.checkBoxUseBreaker);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.buttonTagOut2);
            this.groupBox3.Controls.Add(this.textBoxTagOut2);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.buttonTagOut);
            this.groupBox3.Controls.Add(this.textBoxTagOut);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.radioButtonOutputType1);
            this.groupBox3.Controls.Add(this.radioButtonOutputType0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // buttonTagOut2
            // 
            this.buttonTagOut2.AccessibleDescription = null;
            this.buttonTagOut2.AccessibleName = null;
            resources.ApplyResources(this.buttonTagOut2, "buttonTagOut2");
            this.buttonTagOut2.BackgroundImage = null;
            this.buttonTagOut2.Font = null;
            this.buttonTagOut2.Name = "buttonTagOut2";
            this.buttonTagOut2.Click += new System.EventHandler(this.buttonTagOut2_Click);
            // 
            // textBoxTagOut2
            // 
            this.textBoxTagOut2.AccessibleDescription = null;
            this.textBoxTagOut2.AccessibleName = null;
            resources.ApplyResources(this.textBoxTagOut2, "textBoxTagOut2");
            this.textBoxTagOut2.BackgroundImage = null;
            this.textBoxTagOut2.Font = null;
            this.textBoxTagOut2.Name = "textBoxTagOut2";
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // buttonTagOut
            // 
            this.buttonTagOut.AccessibleDescription = null;
            this.buttonTagOut.AccessibleName = null;
            resources.ApplyResources(this.buttonTagOut, "buttonTagOut");
            this.buttonTagOut.BackgroundImage = null;
            this.buttonTagOut.Font = null;
            this.buttonTagOut.Name = "buttonTagOut";
            this.buttonTagOut.Click += new System.EventHandler(this.buttonTagOut_Click);
            // 
            // textBoxTagOut
            // 
            this.textBoxTagOut.AccessibleDescription = null;
            this.textBoxTagOut.AccessibleName = null;
            resources.ApplyResources(this.textBoxTagOut, "textBoxTagOut");
            this.textBoxTagOut.BackgroundImage = null;
            this.textBoxTagOut.Font = null;
            this.textBoxTagOut.Name = "textBoxTagOut";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // radioButtonOutputType1
            // 
            this.radioButtonOutputType1.AccessibleDescription = null;
            this.radioButtonOutputType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOutputType1, "radioButtonOutputType1");
            this.radioButtonOutputType1.BackgroundImage = null;
            this.radioButtonOutputType1.Font = null;
            this.radioButtonOutputType1.Name = "radioButtonOutputType1";
            this.radioButtonOutputType1.CheckedChanged += new System.EventHandler(this.radioButtonOutputType1_CheckedChanged);
            // 
            // radioButtonOutputType0
            // 
            this.radioButtonOutputType0.AccessibleDescription = null;
            this.radioButtonOutputType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOutputType0, "radioButtonOutputType0");
            this.radioButtonOutputType0.BackgroundImage = null;
            this.radioButtonOutputType0.Font = null;
            this.radioButtonOutputType0.Name = "radioButtonOutputType0";
            this.radioButtonOutputType0.CheckedChanged += new System.EventHandler(this.radioButtonOutputType0_CheckedChanged);
            // 
            // buttonTagEcho
            // 
            this.buttonTagEcho.AccessibleDescription = null;
            this.buttonTagEcho.AccessibleName = null;
            resources.ApplyResources(this.buttonTagEcho, "buttonTagEcho");
            this.buttonTagEcho.BackgroundImage = null;
            this.buttonTagEcho.Font = null;
            this.buttonTagEcho.Name = "buttonTagEcho";
            this.buttonTagEcho.Click += new System.EventHandler(this.buttonTagEcho_Click);
            // 
            // textBoxTagEcho
            // 
            this.textBoxTagEcho.AccessibleDescription = null;
            this.textBoxTagEcho.AccessibleName = null;
            resources.ApplyResources(this.textBoxTagEcho, "textBoxTagEcho");
            this.textBoxTagEcho.BackgroundImage = null;
            this.textBoxTagEcho.Font = null;
            this.textBoxTagEcho.Name = "textBoxTagEcho";
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // numericUpDownProtectTime
            // 
            this.numericUpDownProtectTime.AccessibleDescription = null;
            this.numericUpDownProtectTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownProtectTime, "numericUpDownProtectTime");
            this.numericUpDownProtectTime.Font = null;
            this.numericUpDownProtectTime.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDownProtectTime.Name = "numericUpDownProtectTime";
            this.numericUpDownProtectTime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // checkBoxUseBreaker
            // 
            this.checkBoxUseBreaker.AccessibleDescription = null;
            this.checkBoxUseBreaker.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseBreaker, "checkBoxUseBreaker");
            this.checkBoxUseBreaker.BackgroundImage = null;
            this.checkBoxUseBreaker.Font = null;
            this.checkBoxUseBreaker.Name = "checkBoxUseBreaker";
            this.checkBoxUseBreaker.CheckedChanged += new System.EventHandler(this.checkBoxUseBreaker_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.buttonStartTarget);
            this.groupBox4.Controls.Add(this.textBoxStartTarget);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // buttonStartTarget
            // 
            this.buttonStartTarget.AccessibleDescription = null;
            this.buttonStartTarget.AccessibleName = null;
            resources.ApplyResources(this.buttonStartTarget, "buttonStartTarget");
            this.buttonStartTarget.BackgroundImage = null;
            this.buttonStartTarget.Font = null;
            this.buttonStartTarget.Name = "buttonStartTarget";
            this.buttonStartTarget.Click += new System.EventHandler(this.buttonStartTarget_Click);
            // 
            // textBoxStartTarget
            // 
            this.textBoxStartTarget.AccessibleDescription = null;
            this.textBoxStartTarget.AccessibleName = null;
            resources.ApplyResources(this.textBoxStartTarget, "textBoxStartTarget");
            this.textBoxStartTarget.BackgroundImage = null;
            this.textBoxStartTarget.Font = null;
            this.textBoxStartTarget.Name = "textBoxStartTarget";
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = null;
            this.groupBox5.AccessibleName = null;
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.BackgroundImage = null;
            this.groupBox5.Controls.Add(this.buttonPredictionDisplayTag);
            this.groupBox5.Controls.Add(this.textBoxPredictionDisplayTag);
            this.groupBox5.Font = null;
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // buttonPredictionDisplayTag
            // 
            this.buttonPredictionDisplayTag.AccessibleDescription = null;
            this.buttonPredictionDisplayTag.AccessibleName = null;
            resources.ApplyResources(this.buttonPredictionDisplayTag, "buttonPredictionDisplayTag");
            this.buttonPredictionDisplayTag.BackgroundImage = null;
            this.buttonPredictionDisplayTag.Font = null;
            this.buttonPredictionDisplayTag.Name = "buttonPredictionDisplayTag";
            this.buttonPredictionDisplayTag.Click += new System.EventHandler(this.buttonPredictionTag_Click);
            // 
            // textBoxPredictionDisplayTag
            // 
            this.textBoxPredictionDisplayTag.AccessibleDescription = null;
            this.textBoxPredictionDisplayTag.AccessibleName = null;
            resources.ApplyResources(this.textBoxPredictionDisplayTag, "textBoxPredictionDisplayTag");
            this.textBoxPredictionDisplayTag.BackgroundImage = null;
            this.textBoxPredictionDisplayTag.Font = null;
            this.textBoxPredictionDisplayTag.Name = "textBoxPredictionDisplayTag";
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.numericUpDownDisplayY);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // label13
            // 
            this.label13.AccessibleDescription = null;
            this.label13.AccessibleName = null;
            resources.ApplyResources(this.label13, "label13");
            this.label13.Font = null;
            this.label13.Name = "label13";
            // 
            // numericUpDownDisplayY
            // 
            this.numericUpDownDisplayY.AccessibleDescription = null;
            this.numericUpDownDisplayY.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDisplayY, "numericUpDownDisplayY");
            this.numericUpDownDisplayY.Font = null;
            this.numericUpDownDisplayY.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownDisplayY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownDisplayY.Name = "numericUpDownDisplayY";
            this.numericUpDownDisplayY.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // checkBoxAutoReset
            // 
            this.checkBoxAutoReset.AccessibleDescription = null;
            this.checkBoxAutoReset.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAutoReset, "checkBoxAutoReset");
            this.checkBoxAutoReset.BackgroundImage = null;
            this.checkBoxAutoReset.Font = null;
            this.checkBoxAutoReset.Name = "checkBoxAutoReset";
            // 
            // groupBox7
            // 
            this.groupBox7.AccessibleDescription = null;
            this.groupBox7.AccessibleName = null;
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.BackgroundImage = null;
            this.groupBox7.Controls.Add(this.buttonPredictionInputTag);
            this.groupBox7.Controls.Add(this.textBoxPredictionInputTag);
            this.groupBox7.Font = null;
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // buttonPredictionInputTag
            // 
            this.buttonPredictionInputTag.AccessibleDescription = null;
            this.buttonPredictionInputTag.AccessibleName = null;
            resources.ApplyResources(this.buttonPredictionInputTag, "buttonPredictionInputTag");
            this.buttonPredictionInputTag.BackgroundImage = null;
            this.buttonPredictionInputTag.Font = null;
            this.buttonPredictionInputTag.Name = "buttonPredictionInputTag";
            this.buttonPredictionInputTag.Click += new System.EventHandler(this.buttonPredictionInputTag_Click);
            // 
            // textBoxPredictionInputTag
            // 
            this.textBoxPredictionInputTag.AccessibleDescription = null;
            this.textBoxPredictionInputTag.AccessibleName = null;
            resources.ApplyResources(this.textBoxPredictionInputTag, "textBoxPredictionInputTag");
            this.textBoxPredictionInputTag.BackgroundImage = null;
            this.textBoxPredictionInputTag.Font = null;
            this.textBoxPredictionInputTag.Name = "textBoxPredictionInputTag";
            // 
            // checkBoxSaveToDatabase
            // 
            this.checkBoxSaveToDatabase.AccessibleDescription = null;
            this.checkBoxSaveToDatabase.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveToDatabase, "checkBoxSaveToDatabase");
            this.checkBoxSaveToDatabase.BackgroundImage = null;
            this.checkBoxSaveToDatabase.Font = null;
            this.checkBoxSaveToDatabase.Name = "checkBoxSaveToDatabase";
            this.checkBoxSaveToDatabase.CheckedChanged += new System.EventHandler(this.checkBoxSaveToDatabase_CheckedChanged);
            // 
            // buttonDsn
            // 
            this.buttonDsn.AccessibleDescription = null;
            this.buttonDsn.AccessibleName = null;
            resources.ApplyResources(this.buttonDsn, "buttonDsn");
            this.buttonDsn.BackgroundImage = null;
            this.buttonDsn.Font = null;
            this.buttonDsn.Name = "buttonDsn";
            this.buttonDsn.Click += new System.EventHandler(this.buttonDsn_Click);
            // 
            // label14
            // 
            this.label14.AccessibleDescription = null;
            this.label14.AccessibleName = null;
            resources.ApplyResources(this.label14, "label14");
            this.label14.Font = null;
            this.label14.Name = "label14";
            // 
            // comboBoxDsn
            // 
            this.comboBoxDsn.AccessibleDescription = null;
            this.comboBoxDsn.AccessibleName = null;
            resources.ApplyResources(this.comboBoxDsn, "comboBoxDsn");
            this.comboBoxDsn.BackgroundImage = null;
            this.comboBoxDsn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDsn.Font = null;
            this.comboBoxDsn.Name = "comboBoxDsn";
            // 
            // groupBox8
            // 
            this.groupBox8.AccessibleDescription = null;
            this.groupBox8.AccessibleName = null;
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.BackgroundImage = null;
            this.groupBox8.Controls.Add(this.comboBoxDsn);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.buttonDsn);
            this.groupBox8.Controls.Add(this.checkBoxSaveToDatabase);
            this.groupBox8.Font = null;
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // FormConfigDemandControlAdd
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.checkBoxAutoReset);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxPulseRatio);
            this.Controls.Add(this.textBoxKwTarget);
            this.Controls.Add(this.textBoxKWIN);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownControlTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonKwTarget);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonKWIN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigDemandControlAdd";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigDemandControlAdd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownControlTime)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownProtectTime)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDisplayY)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void DialogToItem(FUNCTION_BLOCK_DEMAND_CONTROL item)
		{
			item.title = this.textBoxTitle.Text;
			item.tagCurr.tag = this.textBoxKWIN.Text;
			item.tagTarget.tag = this.textBoxKwTarget.Text;
			item.nControlTime = ConvertTool.ToInt32(this.numericUpDownControlTime.Value);
			item.nControlProtectTime = ConvertTool.ToInt32(this.numericUpDownProtectTime.Value);
			item.tagOut.tag = this.textBoxTagOut.Text;
			item.bUseIsolation = this.checkBoxUseBreaker.Checked ? (sbyte)1 : (sbyte)0;
			item.tagEcho.tag = this.textBoxTagEcho.Text;
			item.fPulseRatio = ConvertTool.ToSingle(this.textBoxPulseRatio.Text);
			item.bUseEOI = this.checkBoxUseEoi.Checked ?  (sbyte)1 : (sbyte)0;
			item.tagEOI.tag = this.textBoxTagEoi.Text;
			item.tagOut2.tag = this.textBoxTagOut2.Text;

			if(this.radioButtonOutputType0.Checked)		item.cOutputType = 0;
			else if(this.radioButtonOutputType1.Checked)item.cOutputType = 1;
			else										item.cOutputType = 0;

			item.tagStartTarget.tag = this.textBoxStartTarget.Text;
			item.bInputAutoReset = this.checkBoxAutoReset.Checked ?  (sbyte)1 : (sbyte)0;
			item.tagPredictionDisplay.tag = this.textBoxPredictionDisplayTag.Text;
			item.tagPredictionInput.tag = this.textBoxPredictionInputTag.Text;
			item.nPercentY = ConvertTool.ToInt32(this.numericUpDownDisplayY.Value);

            item.bDatabaseSave = this.checkBoxSaveToDatabase.Checked;
            item.sDatabaseDsn = this.comboBoxDsn.Text;
		}

		public void ItemToDialog(FUNCTION_BLOCK_DEMAND_CONTROL item)
		{
			this.textBoxTitle.Text = item.title;
			this.textBoxKWIN.Text = item.tagCurr.tag;
			this.textBoxKwTarget.Text = item.tagTarget.tag;
			this.numericUpDownControlTime.Value = item.nControlTime;
			this.numericUpDownProtectTime.Value = item.nControlProtectTime;
			this.textBoxTagOut.Text = item.tagOut.tag;
			this.checkBoxUseBreaker.Checked = item.bUseIsolation == 1;
			this.textBoxTagEcho.Text = item.tagEcho.tag;
			this.textBoxPulseRatio.Text = item.fPulseRatio.ToString();
			this.checkBoxUseEoi.Checked  = item.bUseEOI == 1;
			this.textBoxTagEoi.Text = item.tagEOI.tag;
			this.textBoxTagOut2.Text = item.tagOut2.tag;

			this.radioButtonOutputType0.Checked = item.cOutputType == 0;
			this.radioButtonOutputType1.Checked = item.cOutputType == 1;

			this.textBoxStartTarget.Text = item.tagStartTarget.tag;
			this.checkBoxAutoReset.Checked = item.bInputAutoReset == 1;
			this.textBoxPredictionDisplayTag.Text = item.tagPredictionDisplay.tag;
			this.textBoxPredictionInputTag.Text = item.tagPredictionInput.tag;
			this.numericUpDownDisplayY.Value = item.nPercentY;

            dsnList.ConnectionStringLoad();
            DbTool.FillComboBox(this.comboBoxDsn, dsnList);
            this.checkBoxSaveToDatabase.Checked = item.bDatabaseSave;
            this.comboBoxDsn.Text = item.sDatabaseDsn;
		}

		private void buttonOK_Click(object sender, System.EventArgs e) 
		{
			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("제목을 입력해야 합니다.", "입력오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入标题。", "输入错误");
				}
				else 
				{
					MessageBox.Show("You must input the title.", "Input error");
				}
				this.textBoxTitle.Select();
				return;
			}

			if(this.textBoxKWIN.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("전력입력 태그를 설정해야 합니다.", "입력오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入[输入电力标记]。", "输入错误");
				}
				else 
				{
					MessageBox.Show("You must select the pulse input tag.", "Input error");
				}

				this.textBoxKWIN.Select();
				return;
			}

			if(this.textBoxKwTarget.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("목표전력 태그를 설정해야 합니다.", "입력오류");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入[目标电力标记]。", "输入错误");
				}
				else 
				{
					MessageBox.Show("You must select the Target Power tag.", "Input error");
				}
				this.textBoxKwTarget.Select();
				return;
			}

            if (this.checkBoxSaveToDatabase.Checked)
            {
                if (this.comboBoxDsn.Text.Trim().Length == 0)
                {
                    if(Tools.IsLangKorean())
                        MessageBox.Show("DSN 항목을 입력해야 합니다.", "입력오류");
                    else
                        MessageBox.Show("DSN entry is required.", "Input Error");
                    return;
                }
            }

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonKWIN_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxKWIN.Text = tag;
			}
		}

		private void buttonKwTarget_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxKwTarget.Text = tag;
			}
		}

		private void buttonTagEoi_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxTagEoi.Text = tag;
			}		
		}

		private void buttonTagEcho_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxTagEcho.Text = tag;
			}
		}

		private void buttonTagOut_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDiDo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxTagOut.Text = tag;
			}
		}

		private void buttonTagOut2_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectDiDo(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxTagOut2.Text = tag;
			}
		}

		private void buttonStartTarget_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxStartTarget.Text = tag;
			}
		}

		private void buttonPredictionTag_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxPredictionDisplayTag.Text = tag;
			}
		}

        ConnectionStringList dsnList = new ConnectionStringList();

		private void FormConfigDemandControlAdd_Load(object sender, System.EventArgs e)
		{
			EnableDisableIsolation();
			EnableDisableEOI();
            EnableDatabase();
		}

        void EnableDatabase()
        {
            bool flag = this.checkBoxSaveToDatabase.Checked;

            this.comboBoxDsn.Enabled = flag;
            this.buttonDsn.Enabled = flag;
        }

		private void checkBoxUseBreaker_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableIsolation();
		}

		int GetRadioOutputType()
		{
			int type = 0;
			if(this.radioButtonOutputType0.Checked)		type = 0;
			else if(this.radioButtonOutputType1.Checked)type = 1;
			else										type = 0;

			return type;
		}

		void EnableDisableOutput2() 
		{
			bool flag_isolation = this.checkBoxUseBreaker.Checked;
			int m_cOutputType = GetRadioOutputType();

			bool flag_out2;
			if(m_cOutputType == 1 && flag_isolation)	flag_out2 = true;
			else										flag_out2 = false;

			this.textBoxTagOut2.Enabled = flag_out2;
			this.buttonTagOut2.Enabled = flag_out2;
		}

		void EnableDisableIsolation() 
		{
			bool flag = this.checkBoxUseBreaker.Checked;

			this.numericUpDownProtectTime.Enabled = flag;
			this.textBoxTagOut.Enabled = flag;
			this.buttonTagOut.Enabled = flag;
			this.textBoxTagEcho.Enabled = flag;
			this.buttonTagEcho.Enabled = flag;
			this.radioButtonOutputType0.Enabled = flag;
			this.radioButtonOutputType1.Enabled = flag;
	
			EnableDisableOutput2();
		}

		void EnableDisableEOI() 
		{
			bool flag = this.checkBoxUseEoi.Checked;

			this.textBoxTagEoi.Enabled = flag;
			this.buttonTagEoi.Enabled = flag;
		}

		private void checkBoxUseEoi_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableEOI();		
		}

		private void radioButtonOutputType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableOutput2();	
		}

		private void radioButtonOutputType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableOutput2();	
		}

		private void label3_Click(object sender, System.EventArgs e)
		{
		
		}

		private void label5_Click(object sender, System.EventArgs e)
		{
		
		}

		private void label4_Click(object sender, System.EventArgs e)
		{
		
		}

		private void buttonPredictionInputTag_Click(object sender, System.EventArgs e)
		{
			string tag;
			string des;
			if(SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK) 
			{
				this.textBoxPredictionInputTag.Text = tag;
			}		
		}

        private void buttonDsn_Click(object sender, EventArgs e)
        {
            FormDatabaseConnection form = new FormDatabaseConnection(dsnList);

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                DbTool.FillComboBox(this.comboBoxDsn, dsnList);
            }
        }

        private void checkBoxSaveToDatabase_CheckedChanged(object sender, EventArgs e)
        {
            EnableDatabase();
        }
	}
}

