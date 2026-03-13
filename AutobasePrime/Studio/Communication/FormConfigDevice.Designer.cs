namespace Studio.Communication
{
    partial class FormConfigDevice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigDevice));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonDeviceType9 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType8 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType7 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType6 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDeviceType0 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.comboBoxComBaud = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxDataBit = new System.Windows.Forms.GroupBox();
            this.radioButtonComDataBit1 = new System.Windows.Forms.RadioButton();
            this.radioButtonComDataBit0 = new System.Windows.Forms.RadioButton();
            this.groupBoxTxFlow = new System.Windows.Forms.GroupBox();
            this.radioButtonComTx2 = new System.Windows.Forms.RadioButton();
            this.radioButtonComTx1 = new System.Windows.Forms.RadioButton();
            this.radioButtonComTx0 = new System.Windows.Forms.RadioButton();
            this.groupBoxRxFlow = new System.Windows.Forms.GroupBox();
            this.radioButtonComRx2 = new System.Windows.Forms.RadioButton();
            this.radioButtonComRx1 = new System.Windows.Forms.RadioButton();
            this.radioButtonComRx0 = new System.Windows.Forms.RadioButton();
            this.groupBoxStopBit = new System.Windows.Forms.GroupBox();
            this.radioButtonComStopBit1 = new System.Windows.Forms.RadioButton();
            this.radioButtonComStopBit0 = new System.Windows.Forms.RadioButton();
            this.groupBoxParityBit = new System.Windows.Forms.GroupBox();
            this.radioButtonComParityBit2 = new System.Windows.Forms.RadioButton();
            this.radioButtonComParityBit1 = new System.Windows.Forms.RadioButton();
            this.radioButtonComParityBit0 = new System.Windows.Forms.RadioButton();
            this.groupBoxEndDelay = new System.Windows.Forms.GroupBox();
            this.numericUpDownComEndDelayWrite = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownComEndDelayRead = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxStartDelay = new System.Windows.Forms.GroupBox();
            this.numericUpDownComStartDelayWrite = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownComStartDelayRead = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBoxTcpip = new System.Windows.Forms.GroupBox();
            this.textBoxTcpIP = new System.Windows.Forms.TextBox();
            this.numericUpDownTcpPort = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBoxSharedMemory = new System.Windows.Forms.GroupBox();
            this.textBoxSharedMemoryName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxTeleDevice = new System.Windows.Forms.GroupBox();
            this.buttonTeleDeviceOption = new System.Windows.Forms.Button();
            this.checkBoxRtsMethod = new System.Windows.Forms.CheckBox();
            this.numericUpDownTcpServerTimeout = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBoxDataBit.SuspendLayout();
            this.groupBoxTxFlow.SuspendLayout();
            this.groupBoxRxFlow.SuspendLayout();
            this.groupBoxStopBit.SuspendLayout();
            this.groupBoxParityBit.SuspendLayout();
            this.groupBoxEndDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComEndDelayWrite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComEndDelayRead)).BeginInit();
            this.groupBoxStartDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComStartDelayWrite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComStartDelayRead)).BeginInit();
            this.groupBoxTcpip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpPort)).BeginInit();
            this.groupBoxSharedMemory.SuspendLayout();
            this.groupBoxTeleDevice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpServerTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonDeviceType9);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType8);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType7);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType6);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType5);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType4);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType3);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType2);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType1);
            this.groupBox1.Controls.Add(this.radioButtonDeviceType0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonDeviceType9
            // 
            resources.ApplyResources(this.radioButtonDeviceType9, "radioButtonDeviceType9");
            this.radioButtonDeviceType9.Name = "radioButtonDeviceType9";
            this.radioButtonDeviceType9.TabStop = true;
            this.radioButtonDeviceType9.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType9.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType9_CheckedChanged);
            // 
            // radioButtonDeviceType8
            // 
            resources.ApplyResources(this.radioButtonDeviceType8, "radioButtonDeviceType8");
            this.radioButtonDeviceType8.Name = "radioButtonDeviceType8";
            this.radioButtonDeviceType8.TabStop = true;
            this.radioButtonDeviceType8.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType8.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType8_CheckedChanged);
            // 
            // radioButtonDeviceType7
            // 
            resources.ApplyResources(this.radioButtonDeviceType7, "radioButtonDeviceType7");
            this.radioButtonDeviceType7.Name = "radioButtonDeviceType7";
            this.radioButtonDeviceType7.TabStop = true;
            this.radioButtonDeviceType7.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType7.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType7_CheckedChanged);
            // 
            // radioButtonDeviceType6
            // 
            resources.ApplyResources(this.radioButtonDeviceType6, "radioButtonDeviceType6");
            this.radioButtonDeviceType6.Name = "radioButtonDeviceType6";
            this.radioButtonDeviceType6.TabStop = true;
            this.radioButtonDeviceType6.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType6.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType6_CheckedChanged);
            // 
            // radioButtonDeviceType5
            // 
            resources.ApplyResources(this.radioButtonDeviceType5, "radioButtonDeviceType5");
            this.radioButtonDeviceType5.Name = "radioButtonDeviceType5";
            this.radioButtonDeviceType5.TabStop = true;
            this.radioButtonDeviceType5.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType5.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType5_CheckedChanged);
            // 
            // radioButtonDeviceType4
            // 
            resources.ApplyResources(this.radioButtonDeviceType4, "radioButtonDeviceType4");
            this.radioButtonDeviceType4.Name = "radioButtonDeviceType4";
            this.radioButtonDeviceType4.TabStop = true;
            this.radioButtonDeviceType4.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType4.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType4_CheckedChanged);
            // 
            // radioButtonDeviceType3
            // 
            resources.ApplyResources(this.radioButtonDeviceType3, "radioButtonDeviceType3");
            this.radioButtonDeviceType3.Name = "radioButtonDeviceType3";
            this.radioButtonDeviceType3.TabStop = true;
            this.radioButtonDeviceType3.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType3.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType3_CheckedChanged);
            // 
            // radioButtonDeviceType2
            // 
            resources.ApplyResources(this.radioButtonDeviceType2, "radioButtonDeviceType2");
            this.radioButtonDeviceType2.Name = "radioButtonDeviceType2";
            this.radioButtonDeviceType2.TabStop = true;
            this.radioButtonDeviceType2.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType2.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType2_CheckedChanged);
            // 
            // radioButtonDeviceType1
            // 
            resources.ApplyResources(this.radioButtonDeviceType1, "radioButtonDeviceType1");
            this.radioButtonDeviceType1.Name = "radioButtonDeviceType1";
            this.radioButtonDeviceType1.TabStop = true;
            this.radioButtonDeviceType1.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType1.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType1_CheckedChanged);
            // 
            // radioButtonDeviceType0
            // 
            resources.ApplyResources(this.radioButtonDeviceType0, "radioButtonDeviceType0");
            this.radioButtonDeviceType0.Name = "radioButtonDeviceType0";
            this.radioButtonDeviceType0.TabStop = true;
            this.radioButtonDeviceType0.UseVisualStyleBackColor = true;
            this.radioButtonDeviceType0.CheckedChanged += new System.EventHandler(this.radioButtonDeviceType0_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxComPort, "comboBoxComPort");
            this.comboBoxComPort.Name = "comboBoxComPort";
            // 
            // comboBoxComBaud
            // 
            this.comboBoxComBaud.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxComBaud, "comboBoxComBaud");
            this.comboBoxComBaud.Name = "comboBoxComBaud";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBoxDataBit
            // 
            this.groupBoxDataBit.Controls.Add(this.radioButtonComDataBit1);
            this.groupBoxDataBit.Controls.Add(this.radioButtonComDataBit0);
            resources.ApplyResources(this.groupBoxDataBit, "groupBoxDataBit");
            this.groupBoxDataBit.Name = "groupBoxDataBit";
            this.groupBoxDataBit.TabStop = false;
            // 
            // radioButtonComDataBit1
            // 
            resources.ApplyResources(this.radioButtonComDataBit1, "radioButtonComDataBit1");
            this.radioButtonComDataBit1.Name = "radioButtonComDataBit1";
            this.radioButtonComDataBit1.TabStop = true;
            this.radioButtonComDataBit1.UseVisualStyleBackColor = true;
            // 
            // radioButtonComDataBit0
            // 
            resources.ApplyResources(this.radioButtonComDataBit0, "radioButtonComDataBit0");
            this.radioButtonComDataBit0.Name = "radioButtonComDataBit0";
            this.radioButtonComDataBit0.TabStop = true;
            this.radioButtonComDataBit0.UseVisualStyleBackColor = true;
            // 
            // groupBoxTxFlow
            // 
            this.groupBoxTxFlow.Controls.Add(this.radioButtonComTx2);
            this.groupBoxTxFlow.Controls.Add(this.radioButtonComTx1);
            this.groupBoxTxFlow.Controls.Add(this.radioButtonComTx0);
            resources.ApplyResources(this.groupBoxTxFlow, "groupBoxTxFlow");
            this.groupBoxTxFlow.Name = "groupBoxTxFlow";
            this.groupBoxTxFlow.TabStop = false;
            // 
            // radioButtonComTx2
            // 
            resources.ApplyResources(this.radioButtonComTx2, "radioButtonComTx2");
            this.radioButtonComTx2.Name = "radioButtonComTx2";
            this.radioButtonComTx2.TabStop = true;
            this.radioButtonComTx2.UseVisualStyleBackColor = true;
            // 
            // radioButtonComTx1
            // 
            resources.ApplyResources(this.radioButtonComTx1, "radioButtonComTx1");
            this.radioButtonComTx1.Name = "radioButtonComTx1";
            this.radioButtonComTx1.TabStop = true;
            this.radioButtonComTx1.UseVisualStyleBackColor = true;
            // 
            // radioButtonComTx0
            // 
            resources.ApplyResources(this.radioButtonComTx0, "radioButtonComTx0");
            this.radioButtonComTx0.Name = "radioButtonComTx0";
            this.radioButtonComTx0.TabStop = true;
            this.radioButtonComTx0.UseVisualStyleBackColor = true;
            // 
            // groupBoxRxFlow
            // 
            this.groupBoxRxFlow.Controls.Add(this.radioButtonComRx2);
            this.groupBoxRxFlow.Controls.Add(this.radioButtonComRx1);
            this.groupBoxRxFlow.Controls.Add(this.radioButtonComRx0);
            resources.ApplyResources(this.groupBoxRxFlow, "groupBoxRxFlow");
            this.groupBoxRxFlow.Name = "groupBoxRxFlow";
            this.groupBoxRxFlow.TabStop = false;
            // 
            // radioButtonComRx2
            // 
            resources.ApplyResources(this.radioButtonComRx2, "radioButtonComRx2");
            this.radioButtonComRx2.Name = "radioButtonComRx2";
            this.radioButtonComRx2.TabStop = true;
            this.radioButtonComRx2.UseVisualStyleBackColor = true;
            // 
            // radioButtonComRx1
            // 
            resources.ApplyResources(this.radioButtonComRx1, "radioButtonComRx1");
            this.radioButtonComRx1.Name = "radioButtonComRx1";
            this.radioButtonComRx1.TabStop = true;
            this.radioButtonComRx1.UseVisualStyleBackColor = true;
            // 
            // radioButtonComRx0
            // 
            resources.ApplyResources(this.radioButtonComRx0, "radioButtonComRx0");
            this.radioButtonComRx0.Name = "radioButtonComRx0";
            this.radioButtonComRx0.TabStop = true;
            this.radioButtonComRx0.UseVisualStyleBackColor = true;
            // 
            // groupBoxStopBit
            // 
            this.groupBoxStopBit.Controls.Add(this.radioButtonComStopBit1);
            this.groupBoxStopBit.Controls.Add(this.radioButtonComStopBit0);
            resources.ApplyResources(this.groupBoxStopBit, "groupBoxStopBit");
            this.groupBoxStopBit.Name = "groupBoxStopBit";
            this.groupBoxStopBit.TabStop = false;
            // 
            // radioButtonComStopBit1
            // 
            resources.ApplyResources(this.radioButtonComStopBit1, "radioButtonComStopBit1");
            this.radioButtonComStopBit1.Name = "radioButtonComStopBit1";
            this.radioButtonComStopBit1.TabStop = true;
            this.radioButtonComStopBit1.UseVisualStyleBackColor = true;
            // 
            // radioButtonComStopBit0
            // 
            resources.ApplyResources(this.radioButtonComStopBit0, "radioButtonComStopBit0");
            this.radioButtonComStopBit0.Name = "radioButtonComStopBit0";
            this.radioButtonComStopBit0.TabStop = true;
            this.radioButtonComStopBit0.UseVisualStyleBackColor = true;
            // 
            // groupBoxParityBit
            // 
            this.groupBoxParityBit.Controls.Add(this.radioButtonComParityBit2);
            this.groupBoxParityBit.Controls.Add(this.radioButtonComParityBit1);
            this.groupBoxParityBit.Controls.Add(this.radioButtonComParityBit0);
            resources.ApplyResources(this.groupBoxParityBit, "groupBoxParityBit");
            this.groupBoxParityBit.Name = "groupBoxParityBit";
            this.groupBoxParityBit.TabStop = false;
            // 
            // radioButtonComParityBit2
            // 
            resources.ApplyResources(this.radioButtonComParityBit2, "radioButtonComParityBit2");
            this.radioButtonComParityBit2.Name = "radioButtonComParityBit2";
            this.radioButtonComParityBit2.TabStop = true;
            this.radioButtonComParityBit2.UseVisualStyleBackColor = true;
            // 
            // radioButtonComParityBit1
            // 
            resources.ApplyResources(this.radioButtonComParityBit1, "radioButtonComParityBit1");
            this.radioButtonComParityBit1.Name = "radioButtonComParityBit1";
            this.radioButtonComParityBit1.TabStop = true;
            this.radioButtonComParityBit1.UseVisualStyleBackColor = true;
            // 
            // radioButtonComParityBit0
            // 
            resources.ApplyResources(this.radioButtonComParityBit0, "radioButtonComParityBit0");
            this.radioButtonComParityBit0.Name = "radioButtonComParityBit0";
            this.radioButtonComParityBit0.TabStop = true;
            this.radioButtonComParityBit0.UseVisualStyleBackColor = true;
            // 
            // groupBoxEndDelay
            // 
            this.groupBoxEndDelay.Controls.Add(this.numericUpDownComEndDelayWrite);
            this.groupBoxEndDelay.Controls.Add(this.numericUpDownComEndDelayRead);
            this.groupBoxEndDelay.Controls.Add(this.label4);
            this.groupBoxEndDelay.Controls.Add(this.label3);
            resources.ApplyResources(this.groupBoxEndDelay, "groupBoxEndDelay");
            this.groupBoxEndDelay.Name = "groupBoxEndDelay";
            this.groupBoxEndDelay.TabStop = false;
            // 
            // numericUpDownComEndDelayWrite
            // 
            resources.ApplyResources(this.numericUpDownComEndDelayWrite, "numericUpDownComEndDelayWrite");
            this.numericUpDownComEndDelayWrite.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownComEndDelayWrite.Name = "numericUpDownComEndDelayWrite";
            // 
            // numericUpDownComEndDelayRead
            // 
            resources.ApplyResources(this.numericUpDownComEndDelayRead, "numericUpDownComEndDelayRead");
            this.numericUpDownComEndDelayRead.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownComEndDelayRead.Name = "numericUpDownComEndDelayRead";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBoxStartDelay
            // 
            this.groupBoxStartDelay.Controls.Add(this.numericUpDownComStartDelayWrite);
            this.groupBoxStartDelay.Controls.Add(this.numericUpDownComStartDelayRead);
            this.groupBoxStartDelay.Controls.Add(this.label5);
            this.groupBoxStartDelay.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBoxStartDelay, "groupBoxStartDelay");
            this.groupBoxStartDelay.Name = "groupBoxStartDelay";
            this.groupBoxStartDelay.TabStop = false;
            // 
            // numericUpDownComStartDelayWrite
            // 
            resources.ApplyResources(this.numericUpDownComStartDelayWrite, "numericUpDownComStartDelayWrite");
            this.numericUpDownComStartDelayWrite.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownComStartDelayWrite.Name = "numericUpDownComStartDelayWrite";
            // 
            // numericUpDownComStartDelayRead
            // 
            resources.ApplyResources(this.numericUpDownComStartDelayRead, "numericUpDownComStartDelayRead");
            this.numericUpDownComStartDelayRead.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownComStartDelayRead.Name = "numericUpDownComStartDelayRead";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBoxTcpip
            // 
            this.groupBoxTcpip.Controls.Add(this.label11);
            this.groupBoxTcpip.Controls.Add(this.numericUpDownTcpServerTimeout);
            this.groupBoxTcpip.Controls.Add(this.label9);
            this.groupBoxTcpip.Controls.Add(this.textBoxTcpIP);
            this.groupBoxTcpip.Controls.Add(this.numericUpDownTcpPort);
            this.groupBoxTcpip.Controls.Add(this.label7);
            this.groupBoxTcpip.Controls.Add(this.label8);
            resources.ApplyResources(this.groupBoxTcpip, "groupBoxTcpip");
            this.groupBoxTcpip.Name = "groupBoxTcpip";
            this.groupBoxTcpip.TabStop = false;
            // 
            // textBoxTcpIP
            // 
            resources.ApplyResources(this.textBoxTcpIP, "textBoxTcpIP");
            this.textBoxTcpIP.Name = "textBoxTcpIP";
            // 
            // numericUpDownTcpPort
            // 
            resources.ApplyResources(this.numericUpDownTcpPort, "numericUpDownTcpPort");
            this.numericUpDownTcpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownTcpPort.Name = "numericUpDownTcpPort";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // groupBoxSharedMemory
            // 
            this.groupBoxSharedMemory.Controls.Add(this.textBoxSharedMemoryName);
            this.groupBoxSharedMemory.Controls.Add(this.label10);
            resources.ApplyResources(this.groupBoxSharedMemory, "groupBoxSharedMemory");
            this.groupBoxSharedMemory.Name = "groupBoxSharedMemory";
            this.groupBoxSharedMemory.TabStop = false;
            // 
            // textBoxSharedMemoryName
            // 
            resources.ApplyResources(this.textBoxSharedMemoryName, "textBoxSharedMemoryName");
            this.textBoxSharedMemoryName.Name = "textBoxSharedMemoryName";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // groupBoxTeleDevice
            // 
            this.groupBoxTeleDevice.Controls.Add(this.buttonTeleDeviceOption);
            resources.ApplyResources(this.groupBoxTeleDevice, "groupBoxTeleDevice");
            this.groupBoxTeleDevice.Name = "groupBoxTeleDevice";
            this.groupBoxTeleDevice.TabStop = false;
            // 
            // buttonTeleDeviceOption
            // 
            resources.ApplyResources(this.buttonTeleDeviceOption, "buttonTeleDeviceOption");
            this.buttonTeleDeviceOption.Name = "buttonTeleDeviceOption";
            this.buttonTeleDeviceOption.UseVisualStyleBackColor = true;
            this.buttonTeleDeviceOption.Click += new System.EventHandler(this.buttonTeleDeviceOption_Click);
            // 
            // checkBoxRtsMethod
            // 
            resources.ApplyResources(this.checkBoxRtsMethod, "checkBoxRtsMethod");
            this.checkBoxRtsMethod.Name = "checkBoxRtsMethod";
            this.checkBoxRtsMethod.UseVisualStyleBackColor = true;
            // 
            // numericUpDownTcpServerTimeout
            // 
            resources.ApplyResources(this.numericUpDownTcpServerTimeout, "numericUpDownTcpServerTimeout");
            this.numericUpDownTcpServerTimeout.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownTcpServerTimeout.Name = "numericUpDownTcpServerTimeout";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // FormConfigDevice
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxRtsMethod);
            this.Controls.Add(this.groupBoxTeleDevice);
            this.Controls.Add(this.groupBoxSharedMemory);
            this.Controls.Add(this.groupBoxTcpip);
            this.Controls.Add(this.groupBoxStartDelay);
            this.Controls.Add(this.groupBoxEndDelay);
            this.Controls.Add(this.groupBoxParityBit);
            this.Controls.Add(this.groupBoxStopBit);
            this.Controls.Add(this.groupBoxRxFlow);
            this.Controls.Add(this.groupBoxTxFlow);
            this.Controls.Add(this.groupBoxDataBit);
            this.Controls.Add(this.comboBoxComBaud);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigDevice";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigDevice_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxDataBit.ResumeLayout(false);
            this.groupBoxDataBit.PerformLayout();
            this.groupBoxTxFlow.ResumeLayout(false);
            this.groupBoxTxFlow.PerformLayout();
            this.groupBoxRxFlow.ResumeLayout(false);
            this.groupBoxRxFlow.PerformLayout();
            this.groupBoxStopBit.ResumeLayout(false);
            this.groupBoxStopBit.PerformLayout();
            this.groupBoxParityBit.ResumeLayout(false);
            this.groupBoxParityBit.PerformLayout();
            this.groupBoxEndDelay.ResumeLayout(false);
            this.groupBoxEndDelay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComEndDelayWrite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComEndDelayRead)).EndInit();
            this.groupBoxStartDelay.ResumeLayout(false);
            this.groupBoxStartDelay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComStartDelayWrite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComStartDelayRead)).EndInit();
            this.groupBoxTcpip.ResumeLayout(false);
            this.groupBoxTcpip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpPort)).EndInit();
            this.groupBoxSharedMemory.ResumeLayout(false);
            this.groupBoxSharedMemory.PerformLayout();
            this.groupBoxTeleDevice.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpServerTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonDeviceType9;
        private System.Windows.Forms.RadioButton radioButtonDeviceType8;
        private System.Windows.Forms.RadioButton radioButtonDeviceType7;
        private System.Windows.Forms.RadioButton radioButtonDeviceType6;
        private System.Windows.Forms.RadioButton radioButtonDeviceType5;
        private System.Windows.Forms.RadioButton radioButtonDeviceType4;
        private System.Windows.Forms.RadioButton radioButtonDeviceType3;
        private System.Windows.Forms.RadioButton radioButtonDeviceType2;
        private System.Windows.Forms.RadioButton radioButtonDeviceType1;
        private System.Windows.Forms.RadioButton radioButtonDeviceType0;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.ComboBox comboBoxComBaud;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxDataBit;
        private System.Windows.Forms.RadioButton radioButtonComDataBit1;
        private System.Windows.Forms.RadioButton radioButtonComDataBit0;
        private System.Windows.Forms.GroupBox groupBoxTxFlow;
        private System.Windows.Forms.RadioButton radioButtonComTx2;
        private System.Windows.Forms.RadioButton radioButtonComTx1;
        private System.Windows.Forms.RadioButton radioButtonComTx0;
        private System.Windows.Forms.GroupBox groupBoxRxFlow;
        private System.Windows.Forms.RadioButton radioButtonComRx2;
        private System.Windows.Forms.RadioButton radioButtonComRx1;
        private System.Windows.Forms.RadioButton radioButtonComRx0;
        private System.Windows.Forms.GroupBox groupBoxStopBit;
        private System.Windows.Forms.RadioButton radioButtonComStopBit1;
        private System.Windows.Forms.RadioButton radioButtonComStopBit0;
        private System.Windows.Forms.GroupBox groupBoxParityBit;
        private System.Windows.Forms.RadioButton radioButtonComParityBit2;
        private System.Windows.Forms.RadioButton radioButtonComParityBit1;
        private System.Windows.Forms.RadioButton radioButtonComParityBit0;
        private System.Windows.Forms.GroupBox groupBoxEndDelay;
        private System.Windows.Forms.NumericUpDown numericUpDownComEndDelayWrite;
        private System.Windows.Forms.NumericUpDown numericUpDownComEndDelayRead;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxStartDelay;
        private System.Windows.Forms.NumericUpDown numericUpDownComStartDelayWrite;
        private System.Windows.Forms.NumericUpDown numericUpDownComStartDelayRead;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBoxTcpip;
        private System.Windows.Forms.NumericUpDown numericUpDownTcpPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxTcpIP;
        private System.Windows.Forms.GroupBox groupBoxSharedMemory;
        private System.Windows.Forms.TextBox textBoxSharedMemoryName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBoxTeleDevice;
        private System.Windows.Forms.Button buttonTeleDeviceOption;
        private System.Windows.Forms.CheckBox checkBoxRtsMethod;
        private System.Windows.Forms.NumericUpDown numericUpDownTcpServerTimeout;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
    }
}