using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcEtcConfigForm.
	/// </summary>
	public class opcEtcConfigForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_OK;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private GroupBox groupBox1;
        private Label label2;
        public NumericUpDown numericUpDown_displayPeriod;
        private TabPage tabPage2;
        private GroupBox groupBox5;
        private CheckBox checkBox_WriteAfterDeviceRead;
        private GroupBox groupBox4;
        private CheckBox checkBox_PeriodicDeviceReadMode;
        private Label label6;
        private CheckBox checkBox_PeriodicRead;
        public NumericUpDown numericUpDown_ReadInterval;
        private Label label7;
        private GroupBox groupBox3;
        public NumericUpDown numericUpDown_WriteRetryCount;
        private Label label3;
        public NumericUpDown numericUpDown_RetrySearchTime;
        private Label label4;
        private CheckBox checkBox_WriteRetry;
        private Label label5;
        private GroupBox groupBox2;
        private Label label1;
        public NumericUpDown numericUpDownWritingCycle;
        private GroupBox groupBox6;
        public NumericUpDown numericUpDown_CheckWritedItemNo;
        private Label label8;
        public NumericUpDown numericUpDown_CheckWritedItemTime;
        private Label label9;
        private CheckBox checkBox_WriteDelayWhenAsyncEvent;
        private Label label10;
        public NumericUpDown numericUpDown_WriteDelayTime;
        private Label label11;
        private Label label12;
        private CheckBox checkBoxManualControlToFirst;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public opcEtcConfigForm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(opcEtcConfigForm));
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox_WriteAfterDeviceRead = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBox_PeriodicDeviceReadMode = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox_PeriodicRead = new System.Windows.Forms.CheckBox();
            this.numericUpDown_ReadInterval = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDown_WriteRetryCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_RetrySearchTime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_WriteRetry = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxManualControlToFirst = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownWritingCycle = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_displayPeriod = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDown_WriteDelayTime = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown_CheckWritedItemNo = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_CheckWritedItemTime = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox_WriteDelayWhenAsyncEvent = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReadInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_WriteRetryCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RetrySearchTime)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWritingCycle)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_displayPeriod)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_WriteDelayTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CheckWritedItemNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CheckWritedItemTime)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleDescription = null;
            this.button_Cancel.AccessibleName = null;
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.BackgroundImage = null;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Font = null;
            this.button_Cancel.Name = "button_Cancel";
            // 
            // button_OK
            // 
            this.button_OK.AccessibleDescription = null;
            this.button_OK.AccessibleName = null;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.BackgroundImage = null;
            this.button_OK.Font = null;
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = null;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.AccessibleDescription = null;
            this.tabPage1.AccessibleName = null;
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.BackgroundImage = null;
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = null;
            this.groupBox5.AccessibleName = null;
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.BackgroundImage = null;
            this.groupBox5.Controls.Add(this.checkBox_WriteAfterDeviceRead);
            this.groupBox5.Font = null;
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBox_WriteAfterDeviceRead
            // 
            this.checkBox_WriteAfterDeviceRead.AccessibleDescription = null;
            this.checkBox_WriteAfterDeviceRead.AccessibleName = null;
            resources.ApplyResources(this.checkBox_WriteAfterDeviceRead, "checkBox_WriteAfterDeviceRead");
            this.checkBox_WriteAfterDeviceRead.BackgroundImage = null;
            this.checkBox_WriteAfterDeviceRead.Font = null;
            this.checkBox_WriteAfterDeviceRead.Name = "checkBox_WriteAfterDeviceRead";
            this.checkBox_WriteAfterDeviceRead.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.checkBox_PeriodicDeviceReadMode);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.checkBox_PeriodicRead);
            this.groupBox4.Controls.Add(this.numericUpDown_ReadInterval);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // checkBox_PeriodicDeviceReadMode
            // 
            this.checkBox_PeriodicDeviceReadMode.AccessibleDescription = null;
            this.checkBox_PeriodicDeviceReadMode.AccessibleName = null;
            resources.ApplyResources(this.checkBox_PeriodicDeviceReadMode, "checkBox_PeriodicDeviceReadMode");
            this.checkBox_PeriodicDeviceReadMode.BackgroundImage = null;
            this.checkBox_PeriodicDeviceReadMode.Font = null;
            this.checkBox_PeriodicDeviceReadMode.Name = "checkBox_PeriodicDeviceReadMode";
            this.checkBox_PeriodicDeviceReadMode.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // checkBox_PeriodicRead
            // 
            this.checkBox_PeriodicRead.AccessibleDescription = null;
            this.checkBox_PeriodicRead.AccessibleName = null;
            resources.ApplyResources(this.checkBox_PeriodicRead, "checkBox_PeriodicRead");
            this.checkBox_PeriodicRead.BackgroundImage = null;
            this.checkBox_PeriodicRead.Font = null;
            this.checkBox_PeriodicRead.Name = "checkBox_PeriodicRead";
            this.checkBox_PeriodicRead.UseVisualStyleBackColor = true;
            this.checkBox_PeriodicRead.CheckedChanged += new System.EventHandler(this.checkBox_PeriodicRead_CheckedChanged);
            // 
            // numericUpDown_ReadInterval
            // 
            this.numericUpDown_ReadInterval.AccessibleDescription = null;
            this.numericUpDown_ReadInterval.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_ReadInterval, "numericUpDown_ReadInterval");
            this.numericUpDown_ReadInterval.Font = null;
            this.numericUpDown_ReadInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDown_ReadInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown_ReadInterval.Name = "numericUpDown_ReadInterval";
            this.numericUpDown_ReadInterval.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.numericUpDown_WriteRetryCount);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDown_RetrySearchTime);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.checkBox_WriteRetry);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // numericUpDown_WriteRetryCount
            // 
            this.numericUpDown_WriteRetryCount.AccessibleDescription = null;
            this.numericUpDown_WriteRetryCount.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_WriteRetryCount, "numericUpDown_WriteRetryCount");
            this.numericUpDown_WriteRetryCount.Font = null;
            this.numericUpDown_WriteRetryCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_WriteRetryCount.Name = "numericUpDown_WriteRetryCount";
            this.numericUpDown_WriteRetryCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDown_RetrySearchTime
            // 
            this.numericUpDown_RetrySearchTime.AccessibleDescription = null;
            this.numericUpDown_RetrySearchTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_RetrySearchTime, "numericUpDown_RetrySearchTime");
            this.numericUpDown_RetrySearchTime.Font = null;
            this.numericUpDown_RetrySearchTime.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericUpDown_RetrySearchTime.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown_RetrySearchTime.Name = "numericUpDown_RetrySearchTime";
            this.numericUpDown_RetrySearchTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // checkBox_WriteRetry
            // 
            this.checkBox_WriteRetry.AccessibleDescription = null;
            this.checkBox_WriteRetry.AccessibleName = null;
            resources.ApplyResources(this.checkBox_WriteRetry, "checkBox_WriteRetry");
            this.checkBox_WriteRetry.BackgroundImage = null;
            this.checkBox_WriteRetry.Font = null;
            this.checkBox_WriteRetry.Name = "checkBox_WriteRetry";
            this.checkBox_WriteRetry.UseVisualStyleBackColor = true;
            this.checkBox_WriteRetry.CheckedChanged += new System.EventHandler(this.checkBox_WriteRetry_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxManualControlToFirst);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numericUpDownWritingCycle);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxManualControlToFirst
            // 
            this.checkBoxManualControlToFirst.AccessibleDescription = null;
            this.checkBoxManualControlToFirst.AccessibleName = null;
            resources.ApplyResources(this.checkBoxManualControlToFirst, "checkBoxManualControlToFirst");
            this.checkBoxManualControlToFirst.BackgroundImage = null;
            this.checkBoxManualControlToFirst.Font = null;
            this.checkBoxManualControlToFirst.Name = "checkBoxManualControlToFirst";
            this.checkBoxManualControlToFirst.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownWritingCycle
            // 
            this.numericUpDownWritingCycle.AccessibleDescription = null;
            this.numericUpDownWritingCycle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownWritingCycle, "numericUpDownWritingCycle");
            this.numericUpDownWritingCycle.Font = null;
            this.numericUpDownWritingCycle.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numericUpDownWritingCycle.Name = "numericUpDownWritingCycle";
            this.numericUpDownWritingCycle.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDown_displayPeriod);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDown_displayPeriod
            // 
            this.numericUpDown_displayPeriod.AccessibleDescription = null;
            this.numericUpDown_displayPeriod.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_displayPeriod, "numericUpDown_displayPeriod");
            this.numericUpDown_displayPeriod.Font = null;
            this.numericUpDown_displayPeriod.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numericUpDown_displayPeriod.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown_displayPeriod.Name = "numericUpDown_displayPeriod";
            this.numericUpDown_displayPeriod.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.numericUpDown_WriteDelayTime);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.numericUpDown_CheckWritedItemNo);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Controls.Add(this.numericUpDown_CheckWritedItemTime);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.checkBox_WriteDelayWhenAsyncEvent);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // numericUpDown_WriteDelayTime
            // 
            this.numericUpDown_WriteDelayTime.AccessibleDescription = null;
            this.numericUpDown_WriteDelayTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_WriteDelayTime, "numericUpDown_WriteDelayTime");
            this.numericUpDown_WriteDelayTime.Font = null;
            this.numericUpDown_WriteDelayTime.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDown_WriteDelayTime.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown_WriteDelayTime.Name = "numericUpDown_WriteDelayTime";
            this.numericUpDown_WriteDelayTime.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // numericUpDown_CheckWritedItemNo
            // 
            this.numericUpDown_CheckWritedItemNo.AccessibleDescription = null;
            this.numericUpDown_CheckWritedItemNo.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_CheckWritedItemNo, "numericUpDown_CheckWritedItemNo");
            this.numericUpDown_CheckWritedItemNo.Font = null;
            this.numericUpDown_CheckWritedItemNo.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_CheckWritedItemNo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_CheckWritedItemNo.Name = "numericUpDown_CheckWritedItemNo";
            this.numericUpDown_CheckWritedItemNo.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // numericUpDown_CheckWritedItemTime
            // 
            this.numericUpDown_CheckWritedItemTime.AccessibleDescription = null;
            this.numericUpDown_CheckWritedItemTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_CheckWritedItemTime, "numericUpDown_CheckWritedItemTime");
            this.numericUpDown_CheckWritedItemTime.Font = null;
            this.numericUpDown_CheckWritedItemTime.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_CheckWritedItemTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_CheckWritedItemTime.Name = "numericUpDown_CheckWritedItemTime";
            this.numericUpDown_CheckWritedItemTime.Value = new decimal(new int[] {
            20,
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
            // checkBox_WriteDelayWhenAsyncEvent
            // 
            this.checkBox_WriteDelayWhenAsyncEvent.AccessibleDescription = null;
            this.checkBox_WriteDelayWhenAsyncEvent.AccessibleName = null;
            resources.ApplyResources(this.checkBox_WriteDelayWhenAsyncEvent, "checkBox_WriteDelayWhenAsyncEvent");
            this.checkBox_WriteDelayWhenAsyncEvent.BackgroundImage = null;
            this.checkBox_WriteDelayWhenAsyncEvent.Font = null;
            this.checkBox_WriteDelayWhenAsyncEvent.Name = "checkBox_WriteDelayWhenAsyncEvent";
            this.checkBox_WriteDelayWhenAsyncEvent.UseVisualStyleBackColor = true;
            this.checkBox_WriteDelayWhenAsyncEvent.CheckedChanged += new System.EventHandler(this.checkBox_WriteDelayWhenAsyncEvent_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // opcEtcConfigForm
            // 
            this.AcceptButton = this.button_OK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.button_Cancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "opcEtcConfigForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.opcEtcConfigForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReadInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_WriteRetryCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RetrySearchTime)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWritingCycle)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_displayPeriod)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_WriteDelayTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CheckWritedItemNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CheckWritedItemTime)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        

        private void opcEtcConfigForm_Load(object sender, System.EventArgs e)
		{
			NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_displayPeriod, opcBasic.opcClientConfig.nItemDisplayPeriod);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownWritingCycle, opcBasic.opcClientConfig.nWritingCycle);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_WriteRetryCount, opcBasic.opcClientConfig.nReWriteCheckCount);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_RetrySearchTime, opcBasic.opcClientConfig.nReWriteCheckTime);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_ReadInterval, opcBasic.opcClientConfig.nPeriodicReadTime);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_CheckWritedItemNo, opcBasic.opcClientConfig.nWriteDelayCount);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_CheckWritedItemTime, opcBasic.opcClientConfig.nWriteDelayCheckTime);
            NetTools.Tools.SetNumericUpDownValue(this.numericUpDown_WriteDelayTime, opcBasic.opcClientConfig.nWriteDelayingTime);

            this.checkBoxManualControlToFirst.Checked = opcBasic.opcClientConfig.bManualControlToFirst;

            this.checkBox_WriteRetry.Checked = opcBasic.opcClientConfig.bUseReWriteCheck;
            this.checkBox_PeriodicRead.Checked = opcBasic.opcClientConfig.bUsePeriodicRead;
            this.checkBox_WriteAfterDeviceRead.Checked = opcBasic.opcClientConfig.bUseDeviceReadMode;
            this.checkBox_PeriodicDeviceReadMode.Checked = opcBasic.opcClientConfig.bUseDevicePeriodicReadMode;
            this.checkBox_WriteDelayWhenAsyncEvent.Checked = opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent;
            this.checkBox_PeriodicDeviceReadMode.Enabled = this.checkBox_PeriodicRead.Checked;
            
            this.numericUpDown_WriteRetryCount.Enabled = opcBasic.opcClientConfig.bUseReWriteCheck;
            this.numericUpDown_RetrySearchTime.Enabled = opcBasic.opcClientConfig.bUseReWriteCheck;
            this.numericUpDown_ReadInterval.Enabled = opcBasic.opcClientConfig.bUsePeriodicRead;

            this.numericUpDown_CheckWritedItemNo.Enabled = opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent;
            this.numericUpDown_CheckWritedItemTime.Enabled = opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent;
            this.numericUpDown_WriteDelayTime.Enabled = opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent;            
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			decimal		val = this.numericUpDown_displayPeriod.Value;

			val = ( val >= 50 && val <= 30000 ) ? val : 1000;
			opcBasic.opcClientConfig.nItemDisplayPeriod = val;

            opcBasic.opcClientConfig.nWritingCycle = (int)this.numericUpDownWritingCycle.Value;

            val = this.numericUpDown_WriteRetryCount.Value;
            val = (val >= 0 && val <= 10) ? val : 1;
            opcBasic.opcClientConfig.nReWriteCheckCount = (int)val;
            val = this.numericUpDown_RetrySearchTime.Value;
            val = (val >= 2 && val <= 3600) ? val : 5;
            opcBasic.opcClientConfig.nReWriteCheckTime = (int)val;
            val = this.numericUpDown_ReadInterval.Value;
            val = (val >= 100 && val <= 60000) ? val : 5000;
            opcBasic.opcClientConfig.nPeriodicReadTime = (int)val;
            val = this.numericUpDown_CheckWritedItemNo.Value;
            val = (val >= 1 && val <= 1000) ? val : 50;
            opcBasic.opcClientConfig.nWriteDelayCount = (int)val;
            val = this.numericUpDown_CheckWritedItemTime.Value;
            val = (val >= 1 && val <= 60) ? val : 20;
            opcBasic.opcClientConfig.nWriteDelayCheckTime = (int)val;
            val = this.numericUpDown_WriteDelayTime.Value;
            val = (val >= 100 && val <= 30000) ? val : 2000;
            opcBasic.opcClientConfig.nWriteDelayingTime = (int)val;

            opcBasic.opcClientConfig.bManualControlToFirst = this.checkBoxManualControlToFirst.Checked;

            opcBasic.opcClientConfig.bUseReWriteCheck = this.checkBox_WriteRetry.Checked;
            opcBasic.opcClientConfig.bUsePeriodicRead = this.checkBox_PeriodicRead.Checked;
            opcBasic.opcClientConfig.bUseDeviceReadMode = this.checkBox_WriteAfterDeviceRead.Checked;
            opcBasic.opcClientConfig.bUseDevicePeriodicReadMode = this.checkBox_PeriodicDeviceReadMode.Checked;
            opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent = this.checkBox_WriteDelayWhenAsyncEvent.Checked;
			this.DialogResult = DialogResult.OK;
			Close();
		}        
        
        
        private void checkBox_WriteDelayWhenAsyncEvent_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_CheckWritedItemNo.Enabled = this.checkBox_WriteDelayWhenAsyncEvent.Checked;
            this.numericUpDown_CheckWritedItemTime.Enabled = this.checkBox_WriteDelayWhenAsyncEvent.Checked;
            this.numericUpDown_WriteDelayTime.Enabled = this.checkBox_WriteDelayWhenAsyncEvent.Checked;
        }

        private void checkBox_WriteRetry_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_WriteRetryCount.Enabled = this.checkBox_WriteRetry.Checked;
            this.numericUpDown_RetrySearchTime.Enabled = this.checkBox_WriteRetry.Checked;
        }

        private void checkBox_PeriodicRead_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_ReadInterval.Enabled = this.checkBox_PeriodicRead.Checked;
            this.checkBox_PeriodicDeviceReadMode.Enabled = this.checkBox_PeriodicRead.Checked;
        }
        
	}
}
