using NetTools.Cryptography;
namespace Studio.Communication
{
    partial class FormConfigPort
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigPort));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.checkBoxThread = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownReadCycle = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownWriteCycle = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDevice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownThreadCycle = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonDevice = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxProtocol = new System.Windows.Forms.ComboBox();
            this.textBoxProtocolOption = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonProtocolOption = new System.Windows.Forms.Button();
            this.numericUpDownSizeWORD = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownSizeFLOAT = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownSizeDWORD = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownSizeSTRING = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownSizeDOUBLE = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownSizeINT64 = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDownTimeoutRead = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDownTimeoutWrite = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxReadMethod = new System.Windows.Forms.TextBox();
            this.checkBoxUseStationInfomation = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxUseDeviceInformation = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBoxLineDuplexProtocol = new System.Windows.Forms.GroupBox();
            this.textBoxSecondaryProtocolOption = new System.Windows.Forms.TextBox();
            this.buttonSecondaryProtocolOption = new System.Windows.Forms.Button();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.checkBoxUseSecondaryProtocol = new System.Windows.Forms.CheckBox();
            this.comboBoxSecondaryProtocol = new System.Windows.Forms.ComboBox();
            this.groupBoxLineDuplexDevice = new System.Windows.Forms.GroupBox();
            this.buttonSecondaryDevice = new System.Windows.Forms.Button();
            this.textBoxSecondaryDevice = new System.Windows.Forms.TextBox();
            this.groupBoxLineDuplexCondition = new System.Windows.Forms.GroupBox();
            this.label33 = new System.Windows.Forms.Label();
            this.numericUpDownLineDuplexCodebad = new System.Windows.Forms.NumericUpDown();
            this.label34 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.numericUpDownLineDuplexTimeout = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.checkBoxUseLineDuplex = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxComputerDuplexThread = new System.Windows.Forms.CheckBox();
            this.groupBoxComputerDuplexAnotherComputer = new System.Windows.Forms.GroupBox();
            this.numericUpDownComputerDualPort = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.textBoxComputerDuplexIP = new System.Windows.Forms.TextBox();
            this.groupBoxComputerDuplexCondition = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.numericUpDownComputerDuplexTimeout = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.checkBoxUseComputerDuplex = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.numericUpDownTelConnectionTimeManual = new System.Windows.Forms.NumericUpDown();
            this.label28 = new System.Windows.Forms.Label();
            this.numericUpDownTelConnectionTimeAuto = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxTelAutoConnection = new System.Windows.Forms.CheckBox();
            this.label27 = new System.Windows.Forms.Label();
            this.numericUpDownTelConnectionCycle = new System.Windows.Forms.NumericUpDown();
            this.textBoxTelNumber = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.userControlConfigCryptography1 = new NetTools.Cryptography.UserControlConfigCryptography();
            this.label32 = new System.Windows.Forms.Label();
            this.textBoxPortNumber = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReadCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWriteCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreadCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeWORD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeFLOAT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeDWORD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeSTRING)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeDOUBLE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeINT64)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeoutRead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeoutWrite)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBoxLineDuplexProtocol.SuspendLayout();
            this.groupBoxLineDuplexDevice.SuspendLayout();
            this.groupBoxLineDuplexCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineDuplexCodebad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineDuplexTimeout)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBoxComputerDuplexAnotherComputer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComputerDualPort)).BeginInit();
            this.groupBoxComputerDuplexCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComputerDuplexTimeout)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTelConnectionTimeManual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTelConnectionTimeAuto)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTelConnectionCycle)).BeginInit();
            this.tabPage5.SuspendLayout();
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
            this.buttonOK.UseVisualStyleBackColor = true;
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
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.AccessibleDescription = null;
            this.checkBoxActive.AccessibleName = null;
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.BackgroundImage = null;
            this.checkBoxActive.Font = null;
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.UseVisualStyleBackColor = true;
            this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
            // 
            // checkBoxThread
            // 
            this.checkBoxThread.AccessibleDescription = null;
            this.checkBoxThread.AccessibleName = null;
            resources.ApplyResources(this.checkBoxThread, "checkBoxThread");
            this.checkBoxThread.BackgroundImage = null;
            this.checkBoxThread.Checked = true;
            this.checkBoxThread.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxThread.Font = null;
            this.checkBoxThread.Name = "checkBoxThread";
            this.checkBoxThread.UseVisualStyleBackColor = true;
            this.checkBoxThread.CheckedChanged += new System.EventHandler(this.checkBoxThread_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AccessibleDescription = null;
            this.textBoxDescription.AccessibleName = null;
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.BackgroundImage = null;
            this.textBoxDescription.Font = null;
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownReadCycle
            // 
            this.numericUpDownReadCycle.AccessibleDescription = null;
            this.numericUpDownReadCycle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownReadCycle, "numericUpDownReadCycle");
            this.numericUpDownReadCycle.Font = null;
            this.numericUpDownReadCycle.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.numericUpDownReadCycle.Name = "numericUpDownReadCycle";
            // 
            // numericUpDownWriteCycle
            // 
            this.numericUpDownWriteCycle.AccessibleDescription = null;
            this.numericUpDownWriteCycle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownWriteCycle, "numericUpDownWriteCycle");
            this.numericUpDownWriteCycle.Font = null;
            this.numericUpDownWriteCycle.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this.numericUpDownWriteCycle.Name = "numericUpDownWriteCycle";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBoxDevice
            // 
            this.textBoxDevice.AccessibleDescription = null;
            this.textBoxDevice.AccessibleName = null;
            resources.ApplyResources(this.textBoxDevice, "textBoxDevice");
            this.textBoxDevice.BackgroundImage = null;
            this.textBoxDevice.Font = null;
            this.textBoxDevice.Name = "textBoxDevice";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // numericUpDownThreadCycle
            // 
            this.numericUpDownThreadCycle.AccessibleDescription = null;
            this.numericUpDownThreadCycle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownThreadCycle, "numericUpDownThreadCycle");
            this.numericUpDownThreadCycle.Font = null;
            this.numericUpDownThreadCycle.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numericUpDownThreadCycle.Name = "numericUpDownThreadCycle";
            this.numericUpDownThreadCycle.ValueChanged += new System.EventHandler(this.numericUpDownThreadCycle_ValueChanged);
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // buttonDevice
            // 
            this.buttonDevice.AccessibleDescription = null;
            this.buttonDevice.AccessibleName = null;
            resources.ApplyResources(this.buttonDevice, "buttonDevice");
            this.buttonDevice.BackgroundImage = null;
            this.buttonDevice.Font = null;
            this.buttonDevice.Name = "buttonDevice";
            this.buttonDevice.UseVisualStyleBackColor = true;
            this.buttonDevice.Click += new System.EventHandler(this.buttonDevice_Click);
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // comboBoxProtocol
            // 
            this.comboBoxProtocol.AccessibleDescription = null;
            this.comboBoxProtocol.AccessibleName = null;
            resources.ApplyResources(this.comboBoxProtocol, "comboBoxProtocol");
            this.comboBoxProtocol.BackgroundImage = null;
            this.comboBoxProtocol.Font = null;
            this.comboBoxProtocol.FormattingEnabled = true;
            this.comboBoxProtocol.Name = "comboBoxProtocol";
            this.comboBoxProtocol.Sorted = true;
            // 
            // textBoxProtocolOption
            // 
            this.textBoxProtocolOption.AccessibleDescription = null;
            this.textBoxProtocolOption.AccessibleName = null;
            resources.ApplyResources(this.textBoxProtocolOption, "textBoxProtocolOption");
            this.textBoxProtocolOption.BackgroundImage = null;
            this.textBoxProtocolOption.Font = null;
            this.textBoxProtocolOption.Name = "textBoxProtocolOption";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // buttonProtocolOption
            // 
            this.buttonProtocolOption.AccessibleDescription = null;
            this.buttonProtocolOption.AccessibleName = null;
            resources.ApplyResources(this.buttonProtocolOption, "buttonProtocolOption");
            this.buttonProtocolOption.BackgroundImage = null;
            this.buttonProtocolOption.Font = null;
            this.buttonProtocolOption.Name = "buttonProtocolOption";
            this.buttonProtocolOption.UseVisualStyleBackColor = true;
            this.buttonProtocolOption.Click += new System.EventHandler(this.buttonProtocolOption_Click);
            // 
            // numericUpDownSizeWORD
            // 
            this.numericUpDownSizeWORD.AccessibleDescription = null;
            this.numericUpDownSizeWORD.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSizeWORD, "numericUpDownSizeWORD");
            this.numericUpDownSizeWORD.Font = null;
            this.numericUpDownSizeWORD.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownSizeWORD.Name = "numericUpDownSizeWORD";
            this.numericUpDownSizeWORD.Value = new decimal(new int[] {
            200,
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
            // numericUpDownSizeFLOAT
            // 
            this.numericUpDownSizeFLOAT.AccessibleDescription = null;
            this.numericUpDownSizeFLOAT.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSizeFLOAT, "numericUpDownSizeFLOAT");
            this.numericUpDownSizeFLOAT.Font = null;
            this.numericUpDownSizeFLOAT.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownSizeFLOAT.Name = "numericUpDownSizeFLOAT";
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // numericUpDownSizeDWORD
            // 
            this.numericUpDownSizeDWORD.AccessibleDescription = null;
            this.numericUpDownSizeDWORD.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSizeDWORD, "numericUpDownSizeDWORD");
            this.numericUpDownSizeDWORD.Font = null;
            this.numericUpDownSizeDWORD.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownSizeDWORD.Name = "numericUpDownSizeDWORD";
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Font = null;
            this.label10.Name = "label10";
            // 
            // numericUpDownSizeSTRING
            // 
            this.numericUpDownSizeSTRING.AccessibleDescription = null;
            this.numericUpDownSizeSTRING.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSizeSTRING, "numericUpDownSizeSTRING");
            this.numericUpDownSizeSTRING.Font = null;
            this.numericUpDownSizeSTRING.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownSizeSTRING.Name = "numericUpDownSizeSTRING";
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // numericUpDownSizeDOUBLE
            // 
            this.numericUpDownSizeDOUBLE.AccessibleDescription = null;
            this.numericUpDownSizeDOUBLE.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSizeDOUBLE, "numericUpDownSizeDOUBLE");
            this.numericUpDownSizeDOUBLE.Font = null;
            this.numericUpDownSizeDOUBLE.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownSizeDOUBLE.Name = "numericUpDownSizeDOUBLE";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // numericUpDownSizeINT64
            // 
            this.numericUpDownSizeINT64.AccessibleDescription = null;
            this.numericUpDownSizeINT64.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSizeINT64, "numericUpDownSizeINT64");
            this.numericUpDownSizeINT64.Font = null;
            this.numericUpDownSizeINT64.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownSizeINT64.Name = "numericUpDownSizeINT64";
            // 
            // label13
            // 
            this.label13.AccessibleDescription = null;
            this.label13.AccessibleName = null;
            resources.ApplyResources(this.label13, "label13");
            this.label13.Font = null;
            this.label13.Name = "label13";
            // 
            // numericUpDownTimeoutRead
            // 
            this.numericUpDownTimeoutRead.AccessibleDescription = null;
            this.numericUpDownTimeoutRead.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTimeoutRead, "numericUpDownTimeoutRead");
            this.numericUpDownTimeoutRead.Font = null;
            this.numericUpDownTimeoutRead.Maximum = new decimal(new int[] {
            7200000,
            0,
            0,
            0});
            this.numericUpDownTimeoutRead.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownTimeoutRead.Name = "numericUpDownTimeoutRead";
            this.numericUpDownTimeoutRead.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AccessibleDescription = null;
            this.label14.AccessibleName = null;
            resources.ApplyResources(this.label14, "label14");
            this.label14.Font = null;
            this.label14.Name = "label14";
            // 
            // numericUpDownTimeoutWrite
            // 
            this.numericUpDownTimeoutWrite.AccessibleDescription = null;
            this.numericUpDownTimeoutWrite.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTimeoutWrite, "numericUpDownTimeoutWrite");
            this.numericUpDownTimeoutWrite.Font = null;
            this.numericUpDownTimeoutWrite.Maximum = new decimal(new int[] {
            7200000,
            0,
            0,
            0});
            this.numericUpDownTimeoutWrite.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownTimeoutWrite.Name = "numericUpDownTimeoutWrite";
            this.numericUpDownTimeoutWrite.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // label15
            // 
            this.label15.AccessibleDescription = null;
            this.label15.AccessibleName = null;
            resources.ApplyResources(this.label15, "label15");
            this.label15.Font = null;
            this.label15.Name = "label15";
            // 
            // textBoxReadMethod
            // 
            this.textBoxReadMethod.AcceptsReturn = true;
            this.textBoxReadMethod.AccessibleDescription = null;
            this.textBoxReadMethod.AccessibleName = null;
            resources.ApplyResources(this.textBoxReadMethod, "textBoxReadMethod");
            this.textBoxReadMethod.BackgroundImage = null;
            this.textBoxReadMethod.Font = null;
            this.textBoxReadMethod.Name = "textBoxReadMethod";
            // 
            // checkBoxUseStationInfomation
            // 
            this.checkBoxUseStationInfomation.AccessibleDescription = null;
            this.checkBoxUseStationInfomation.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseStationInfomation, "checkBoxUseStationInfomation");
            this.checkBoxUseStationInfomation.BackgroundImage = null;
            this.checkBoxUseStationInfomation.Font = null;
            this.checkBoxUseStationInfomation.Name = "checkBoxUseStationInfomation";
            this.checkBoxUseStationInfomation.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
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
            this.tabPage1.Controls.Add(this.checkBoxUseDeviceInformation);
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.checkBoxUseStationInfomation);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.numericUpDownTimeoutWrite);
            this.tabPage1.Controls.Add(this.textBoxDescription);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.numericUpDownTimeoutRead);
            this.tabPage1.Controls.Add(this.numericUpDownReadCycle);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.numericUpDownSizeINT64);
            this.tabPage1.Controls.Add(this.numericUpDownWriteCycle);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.numericUpDownSizeDOUBLE);
            this.tabPage1.Controls.Add(this.textBoxDevice);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.numericUpDownSizeSTRING);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.buttonDevice);
            this.tabPage1.Controls.Add(this.numericUpDownSizeDWORD);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.comboBoxProtocol);
            this.tabPage1.Controls.Add(this.numericUpDownSizeFLOAT);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.textBoxProtocolOption);
            this.tabPage1.Controls.Add(this.numericUpDownSizeWORD);
            this.tabPage1.Controls.Add(this.buttonProtocolOption);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseDeviceInformation
            // 
            this.checkBoxUseDeviceInformation.AccessibleDescription = null;
            this.checkBoxUseDeviceInformation.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseDeviceInformation, "checkBoxUseDeviceInformation");
            this.checkBoxUseDeviceInformation.BackgroundImage = null;
            this.checkBoxUseDeviceInformation.Font = null;
            this.checkBoxUseDeviceInformation.Name = "checkBoxUseDeviceInformation";
            this.checkBoxUseDeviceInformation.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.AccessibleDescription = null;
            this.groupBox8.AccessibleName = null;
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.BackgroundImage = null;
            this.groupBox8.Controls.Add(this.checkBoxThread);
            this.groupBox8.Controls.Add(this.numericUpDownThreadCycle);
            this.groupBox8.Controls.Add(this.label5);
            this.groupBox8.Font = null;
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // label19
            // 
            this.label19.AccessibleDescription = null;
            this.label19.AccessibleName = null;
            resources.ApplyResources(this.label19, "label19");
            this.label19.Font = null;
            this.label19.Name = "label19";
            // 
            // label18
            // 
            this.label18.AccessibleDescription = null;
            this.label18.AccessibleName = null;
            resources.ApplyResources(this.label18, "label18");
            this.label18.Font = null;
            this.label18.Name = "label18";
            // 
            // label17
            // 
            this.label17.AccessibleDescription = null;
            this.label17.AccessibleName = null;
            resources.ApplyResources(this.label17, "label17");
            this.label17.Font = null;
            this.label17.Name = "label17";
            // 
            // label16
            // 
            this.label16.AccessibleDescription = null;
            this.label16.AccessibleName = null;
            resources.ApplyResources(this.label16, "label16");
            this.label16.Font = null;
            this.label16.Name = "label16";
            // 
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.groupBoxLineDuplexProtocol);
            this.tabPage2.Controls.Add(this.groupBoxLineDuplexDevice);
            this.tabPage2.Controls.Add(this.groupBoxLineDuplexCondition);
            this.tabPage2.Controls.Add(this.checkBoxUseLineDuplex);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBoxLineDuplexProtocol
            // 
            this.groupBoxLineDuplexProtocol.AccessibleDescription = null;
            this.groupBoxLineDuplexProtocol.AccessibleName = null;
            resources.ApplyResources(this.groupBoxLineDuplexProtocol, "groupBoxLineDuplexProtocol");
            this.groupBoxLineDuplexProtocol.BackgroundImage = null;
            this.groupBoxLineDuplexProtocol.Controls.Add(this.textBoxSecondaryProtocolOption);
            this.groupBoxLineDuplexProtocol.Controls.Add(this.buttonSecondaryProtocolOption);
            this.groupBoxLineDuplexProtocol.Controls.Add(this.label35);
            this.groupBoxLineDuplexProtocol.Controls.Add(this.label36);
            this.groupBoxLineDuplexProtocol.Controls.Add(this.checkBoxUseSecondaryProtocol);
            this.groupBoxLineDuplexProtocol.Controls.Add(this.comboBoxSecondaryProtocol);
            this.groupBoxLineDuplexProtocol.Font = null;
            this.groupBoxLineDuplexProtocol.Name = "groupBoxLineDuplexProtocol";
            this.groupBoxLineDuplexProtocol.TabStop = false;
            // 
            // textBoxSecondaryProtocolOption
            // 
            this.textBoxSecondaryProtocolOption.AccessibleDescription = null;
            this.textBoxSecondaryProtocolOption.AccessibleName = null;
            resources.ApplyResources(this.textBoxSecondaryProtocolOption, "textBoxSecondaryProtocolOption");
            this.textBoxSecondaryProtocolOption.BackgroundImage = null;
            this.textBoxSecondaryProtocolOption.Font = null;
            this.textBoxSecondaryProtocolOption.Name = "textBoxSecondaryProtocolOption";
            // 
            // buttonSecondaryProtocolOption
            // 
            this.buttonSecondaryProtocolOption.AccessibleDescription = null;
            this.buttonSecondaryProtocolOption.AccessibleName = null;
            resources.ApplyResources(this.buttonSecondaryProtocolOption, "buttonSecondaryProtocolOption");
            this.buttonSecondaryProtocolOption.BackgroundImage = null;
            this.buttonSecondaryProtocolOption.Font = null;
            this.buttonSecondaryProtocolOption.Name = "buttonSecondaryProtocolOption";
            this.buttonSecondaryProtocolOption.UseVisualStyleBackColor = true;
            this.buttonSecondaryProtocolOption.Click += new System.EventHandler(this.buttonSecondaryProtocolOption_Click);
            // 
            // label35
            // 
            this.label35.AccessibleDescription = null;
            this.label35.AccessibleName = null;
            resources.ApplyResources(this.label35, "label35");
            this.label35.Font = null;
            this.label35.Name = "label35";
            // 
            // label36
            // 
            this.label36.AccessibleDescription = null;
            this.label36.AccessibleName = null;
            resources.ApplyResources(this.label36, "label36");
            this.label36.Font = null;
            this.label36.Name = "label36";
            // 
            // checkBoxUseSecondaryProtocol
            // 
            this.checkBoxUseSecondaryProtocol.AccessibleDescription = null;
            this.checkBoxUseSecondaryProtocol.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseSecondaryProtocol, "checkBoxUseSecondaryProtocol");
            this.checkBoxUseSecondaryProtocol.BackgroundImage = null;
            this.checkBoxUseSecondaryProtocol.Font = null;
            this.checkBoxUseSecondaryProtocol.Name = "checkBoxUseSecondaryProtocol";
            this.checkBoxUseSecondaryProtocol.UseVisualStyleBackColor = true;
            this.checkBoxUseSecondaryProtocol.CheckedChanged += new System.EventHandler(this.checkBoxUseSecondaryProtocol_CheckedChanged);
            // 
            // comboBoxSecondaryProtocol
            // 
            this.comboBoxSecondaryProtocol.AccessibleDescription = null;
            this.comboBoxSecondaryProtocol.AccessibleName = null;
            resources.ApplyResources(this.comboBoxSecondaryProtocol, "comboBoxSecondaryProtocol");
            this.comboBoxSecondaryProtocol.BackgroundImage = null;
            this.comboBoxSecondaryProtocol.Font = null;
            this.comboBoxSecondaryProtocol.FormattingEnabled = true;
            this.comboBoxSecondaryProtocol.Name = "comboBoxSecondaryProtocol";
            this.comboBoxSecondaryProtocol.Sorted = true;
            // 
            // groupBoxLineDuplexDevice
            // 
            this.groupBoxLineDuplexDevice.AccessibleDescription = null;
            this.groupBoxLineDuplexDevice.AccessibleName = null;
            resources.ApplyResources(this.groupBoxLineDuplexDevice, "groupBoxLineDuplexDevice");
            this.groupBoxLineDuplexDevice.BackgroundImage = null;
            this.groupBoxLineDuplexDevice.Controls.Add(this.buttonSecondaryDevice);
            this.groupBoxLineDuplexDevice.Controls.Add(this.textBoxSecondaryDevice);
            this.groupBoxLineDuplexDevice.Font = null;
            this.groupBoxLineDuplexDevice.Name = "groupBoxLineDuplexDevice";
            this.groupBoxLineDuplexDevice.TabStop = false;
            // 
            // buttonSecondaryDevice
            // 
            this.buttonSecondaryDevice.AccessibleDescription = null;
            this.buttonSecondaryDevice.AccessibleName = null;
            resources.ApplyResources(this.buttonSecondaryDevice, "buttonSecondaryDevice");
            this.buttonSecondaryDevice.BackgroundImage = null;
            this.buttonSecondaryDevice.Font = null;
            this.buttonSecondaryDevice.Name = "buttonSecondaryDevice";
            this.buttonSecondaryDevice.UseVisualStyleBackColor = true;
            this.buttonSecondaryDevice.Click += new System.EventHandler(this.buttonSecondaryDevice_Click);
            // 
            // textBoxSecondaryDevice
            // 
            this.textBoxSecondaryDevice.AccessibleDescription = null;
            this.textBoxSecondaryDevice.AccessibleName = null;
            resources.ApplyResources(this.textBoxSecondaryDevice, "textBoxSecondaryDevice");
            this.textBoxSecondaryDevice.BackgroundImage = null;
            this.textBoxSecondaryDevice.Font = null;
            this.textBoxSecondaryDevice.Name = "textBoxSecondaryDevice";
            // 
            // groupBoxLineDuplexCondition
            // 
            this.groupBoxLineDuplexCondition.AccessibleDescription = null;
            this.groupBoxLineDuplexCondition.AccessibleName = null;
            resources.ApplyResources(this.groupBoxLineDuplexCondition, "groupBoxLineDuplexCondition");
            this.groupBoxLineDuplexCondition.BackgroundImage = null;
            this.groupBoxLineDuplexCondition.Controls.Add(this.label33);
            this.groupBoxLineDuplexCondition.Controls.Add(this.numericUpDownLineDuplexCodebad);
            this.groupBoxLineDuplexCondition.Controls.Add(this.label34);
            this.groupBoxLineDuplexCondition.Controls.Add(this.label21);
            this.groupBoxLineDuplexCondition.Controls.Add(this.numericUpDownLineDuplexTimeout);
            this.groupBoxLineDuplexCondition.Controls.Add(this.label20);
            this.groupBoxLineDuplexCondition.Font = null;
            this.groupBoxLineDuplexCondition.Name = "groupBoxLineDuplexCondition";
            this.groupBoxLineDuplexCondition.TabStop = false;
            // 
            // label33
            // 
            this.label33.AccessibleDescription = null;
            this.label33.AccessibleName = null;
            resources.ApplyResources(this.label33, "label33");
            this.label33.Font = null;
            this.label33.Name = "label33";
            // 
            // numericUpDownLineDuplexCodebad
            // 
            this.numericUpDownLineDuplexCodebad.AccessibleDescription = null;
            this.numericUpDownLineDuplexCodebad.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownLineDuplexCodebad, "numericUpDownLineDuplexCodebad");
            this.numericUpDownLineDuplexCodebad.Font = null;
            this.numericUpDownLineDuplexCodebad.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownLineDuplexCodebad.Name = "numericUpDownLineDuplexCodebad";
            // 
            // label34
            // 
            this.label34.AccessibleDescription = null;
            this.label34.AccessibleName = null;
            resources.ApplyResources(this.label34, "label34");
            this.label34.Font = null;
            this.label34.Name = "label34";
            // 
            // label21
            // 
            this.label21.AccessibleDescription = null;
            this.label21.AccessibleName = null;
            resources.ApplyResources(this.label21, "label21");
            this.label21.Font = null;
            this.label21.Name = "label21";
            // 
            // numericUpDownLineDuplexTimeout
            // 
            this.numericUpDownLineDuplexTimeout.AccessibleDescription = null;
            this.numericUpDownLineDuplexTimeout.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownLineDuplexTimeout, "numericUpDownLineDuplexTimeout");
            this.numericUpDownLineDuplexTimeout.Font = null;
            this.numericUpDownLineDuplexTimeout.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownLineDuplexTimeout.Name = "numericUpDownLineDuplexTimeout";
            // 
            // label20
            // 
            this.label20.AccessibleDescription = null;
            this.label20.AccessibleName = null;
            resources.ApplyResources(this.label20, "label20");
            this.label20.Font = null;
            this.label20.Name = "label20";
            // 
            // checkBoxUseLineDuplex
            // 
            this.checkBoxUseLineDuplex.AccessibleDescription = null;
            this.checkBoxUseLineDuplex.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseLineDuplex, "checkBoxUseLineDuplex");
            this.checkBoxUseLineDuplex.BackgroundImage = null;
            this.checkBoxUseLineDuplex.Font = null;
            this.checkBoxUseLineDuplex.Name = "checkBoxUseLineDuplex";
            this.checkBoxUseLineDuplex.UseVisualStyleBackColor = true;
            this.checkBoxUseLineDuplex.CheckedChanged += new System.EventHandler(this.checkBoxUseLineDuplex_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.AccessibleDescription = null;
            this.tabPage3.AccessibleName = null;
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.BackgroundImage = null;
            this.tabPage3.Controls.Add(this.checkBoxComputerDuplexThread);
            this.tabPage3.Controls.Add(this.groupBoxComputerDuplexAnotherComputer);
            this.tabPage3.Controls.Add(this.groupBoxComputerDuplexCondition);
            this.tabPage3.Controls.Add(this.checkBoxUseComputerDuplex);
            this.tabPage3.Font = null;
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxComputerDuplexThread
            // 
            this.checkBoxComputerDuplexThread.AccessibleDescription = null;
            this.checkBoxComputerDuplexThread.AccessibleName = null;
            resources.ApplyResources(this.checkBoxComputerDuplexThread, "checkBoxComputerDuplexThread");
            this.checkBoxComputerDuplexThread.BackgroundImage = null;
            this.checkBoxComputerDuplexThread.Font = null;
            this.checkBoxComputerDuplexThread.Name = "checkBoxComputerDuplexThread";
            this.checkBoxComputerDuplexThread.UseVisualStyleBackColor = true;
            // 
            // groupBoxComputerDuplexAnotherComputer
            // 
            this.groupBoxComputerDuplexAnotherComputer.AccessibleDescription = null;
            this.groupBoxComputerDuplexAnotherComputer.AccessibleName = null;
            resources.ApplyResources(this.groupBoxComputerDuplexAnotherComputer, "groupBoxComputerDuplexAnotherComputer");
            this.groupBoxComputerDuplexAnotherComputer.BackgroundImage = null;
            this.groupBoxComputerDuplexAnotherComputer.Controls.Add(this.numericUpDownComputerDualPort);
            this.groupBoxComputerDuplexAnotherComputer.Controls.Add(this.label25);
            this.groupBoxComputerDuplexAnotherComputer.Controls.Add(this.label24);
            this.groupBoxComputerDuplexAnotherComputer.Controls.Add(this.textBoxComputerDuplexIP);
            this.groupBoxComputerDuplexAnotherComputer.Font = null;
            this.groupBoxComputerDuplexAnotherComputer.Name = "groupBoxComputerDuplexAnotherComputer";
            this.groupBoxComputerDuplexAnotherComputer.TabStop = false;
            // 
            // numericUpDownComputerDualPort
            // 
            this.numericUpDownComputerDualPort.AccessibleDescription = null;
            this.numericUpDownComputerDualPort.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownComputerDualPort, "numericUpDownComputerDualPort");
            this.numericUpDownComputerDualPort.Font = null;
            this.numericUpDownComputerDualPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownComputerDualPort.Name = "numericUpDownComputerDualPort";
            // 
            // label25
            // 
            this.label25.AccessibleDescription = null;
            this.label25.AccessibleName = null;
            resources.ApplyResources(this.label25, "label25");
            this.label25.Font = null;
            this.label25.Name = "label25";
            // 
            // label24
            // 
            this.label24.AccessibleDescription = null;
            this.label24.AccessibleName = null;
            resources.ApplyResources(this.label24, "label24");
            this.label24.Font = null;
            this.label24.Name = "label24";
            // 
            // textBoxComputerDuplexIP
            // 
            this.textBoxComputerDuplexIP.AccessibleDescription = null;
            this.textBoxComputerDuplexIP.AccessibleName = null;
            resources.ApplyResources(this.textBoxComputerDuplexIP, "textBoxComputerDuplexIP");
            this.textBoxComputerDuplexIP.BackgroundImage = null;
            this.textBoxComputerDuplexIP.Font = null;
            this.textBoxComputerDuplexIP.Name = "textBoxComputerDuplexIP";
            // 
            // groupBoxComputerDuplexCondition
            // 
            this.groupBoxComputerDuplexCondition.AccessibleDescription = null;
            this.groupBoxComputerDuplexCondition.AccessibleName = null;
            resources.ApplyResources(this.groupBoxComputerDuplexCondition, "groupBoxComputerDuplexCondition");
            this.groupBoxComputerDuplexCondition.BackgroundImage = null;
            this.groupBoxComputerDuplexCondition.Controls.Add(this.label22);
            this.groupBoxComputerDuplexCondition.Controls.Add(this.numericUpDownComputerDuplexTimeout);
            this.groupBoxComputerDuplexCondition.Controls.Add(this.label23);
            this.groupBoxComputerDuplexCondition.Font = null;
            this.groupBoxComputerDuplexCondition.Name = "groupBoxComputerDuplexCondition";
            this.groupBoxComputerDuplexCondition.TabStop = false;
            // 
            // label22
            // 
            this.label22.AccessibleDescription = null;
            this.label22.AccessibleName = null;
            resources.ApplyResources(this.label22, "label22");
            this.label22.Font = null;
            this.label22.Name = "label22";
            // 
            // numericUpDownComputerDuplexTimeout
            // 
            this.numericUpDownComputerDuplexTimeout.AccessibleDescription = null;
            this.numericUpDownComputerDuplexTimeout.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownComputerDuplexTimeout, "numericUpDownComputerDuplexTimeout");
            this.numericUpDownComputerDuplexTimeout.Font = null;
            this.numericUpDownComputerDuplexTimeout.Name = "numericUpDownComputerDuplexTimeout";
            // 
            // label23
            // 
            this.label23.AccessibleDescription = null;
            this.label23.AccessibleName = null;
            resources.ApplyResources(this.label23, "label23");
            this.label23.Font = null;
            this.label23.Name = "label23";
            // 
            // checkBoxUseComputerDuplex
            // 
            this.checkBoxUseComputerDuplex.AccessibleDescription = null;
            this.checkBoxUseComputerDuplex.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseComputerDuplex, "checkBoxUseComputerDuplex");
            this.checkBoxUseComputerDuplex.BackgroundImage = null;
            this.checkBoxUseComputerDuplex.Font = null;
            this.checkBoxUseComputerDuplex.Name = "checkBoxUseComputerDuplex";
            this.checkBoxUseComputerDuplex.UseVisualStyleBackColor = true;
            this.checkBoxUseComputerDuplex.CheckedChanged += new System.EventHandler(this.checkBoxUseComputerDuplex_CheckedChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.AccessibleDescription = null;
            this.tabPage4.AccessibleName = null;
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.BackgroundImage = null;
            this.tabPage4.Controls.Add(this.groupBox7);
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.textBoxTelNumber);
            this.tabPage4.Controls.Add(this.label26);
            this.tabPage4.Font = null;
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.AccessibleDescription = null;
            this.groupBox7.AccessibleName = null;
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.BackgroundImage = null;
            this.groupBox7.Controls.Add(this.label31);
            this.groupBox7.Controls.Add(this.label30);
            this.groupBox7.Controls.Add(this.label29);
            this.groupBox7.Controls.Add(this.numericUpDownTelConnectionTimeManual);
            this.groupBox7.Controls.Add(this.label28);
            this.groupBox7.Controls.Add(this.numericUpDownTelConnectionTimeAuto);
            this.groupBox7.Font = null;
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label31
            // 
            this.label31.AccessibleDescription = null;
            this.label31.AccessibleName = null;
            resources.ApplyResources(this.label31, "label31");
            this.label31.Font = null;
            this.label31.Name = "label31";
            // 
            // label30
            // 
            this.label30.AccessibleDescription = null;
            this.label30.AccessibleName = null;
            resources.ApplyResources(this.label30, "label30");
            this.label30.Font = null;
            this.label30.Name = "label30";
            // 
            // label29
            // 
            this.label29.AccessibleDescription = null;
            this.label29.AccessibleName = null;
            resources.ApplyResources(this.label29, "label29");
            this.label29.Font = null;
            this.label29.Name = "label29";
            // 
            // numericUpDownTelConnectionTimeManual
            // 
            this.numericUpDownTelConnectionTimeManual.AccessibleDescription = null;
            this.numericUpDownTelConnectionTimeManual.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTelConnectionTimeManual, "numericUpDownTelConnectionTimeManual");
            this.numericUpDownTelConnectionTimeManual.Font = null;
            this.numericUpDownTelConnectionTimeManual.Maximum = new decimal(new int[] {
            36000,
            0,
            0,
            0});
            this.numericUpDownTelConnectionTimeManual.Name = "numericUpDownTelConnectionTimeManual";
            // 
            // label28
            // 
            this.label28.AccessibleDescription = null;
            this.label28.AccessibleName = null;
            resources.ApplyResources(this.label28, "label28");
            this.label28.Font = null;
            this.label28.Name = "label28";
            // 
            // numericUpDownTelConnectionTimeAuto
            // 
            this.numericUpDownTelConnectionTimeAuto.AccessibleDescription = null;
            this.numericUpDownTelConnectionTimeAuto.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTelConnectionTimeAuto, "numericUpDownTelConnectionTimeAuto");
            this.numericUpDownTelConnectionTimeAuto.Font = null;
            this.numericUpDownTelConnectionTimeAuto.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericUpDownTelConnectionTimeAuto.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownTelConnectionTimeAuto.Name = "numericUpDownTelConnectionTimeAuto";
            this.numericUpDownTelConnectionTimeAuto.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.checkBoxTelAutoConnection);
            this.groupBox4.Controls.Add(this.label27);
            this.groupBox4.Controls.Add(this.numericUpDownTelConnectionCycle);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // checkBoxTelAutoConnection
            // 
            this.checkBoxTelAutoConnection.AccessibleDescription = null;
            this.checkBoxTelAutoConnection.AccessibleName = null;
            resources.ApplyResources(this.checkBoxTelAutoConnection, "checkBoxTelAutoConnection");
            this.checkBoxTelAutoConnection.BackgroundImage = null;
            this.checkBoxTelAutoConnection.Font = null;
            this.checkBoxTelAutoConnection.Name = "checkBoxTelAutoConnection";
            this.checkBoxTelAutoConnection.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            this.label27.AccessibleDescription = null;
            this.label27.AccessibleName = null;
            resources.ApplyResources(this.label27, "label27");
            this.label27.Font = null;
            this.label27.Name = "label27";
            // 
            // numericUpDownTelConnectionCycle
            // 
            this.numericUpDownTelConnectionCycle.AccessibleDescription = null;
            this.numericUpDownTelConnectionCycle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTelConnectionCycle, "numericUpDownTelConnectionCycle");
            this.numericUpDownTelConnectionCycle.Font = null;
            this.numericUpDownTelConnectionCycle.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numericUpDownTelConnectionCycle.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownTelConnectionCycle.Name = "numericUpDownTelConnectionCycle";
            this.numericUpDownTelConnectionCycle.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // textBoxTelNumber
            // 
            this.textBoxTelNumber.AccessibleDescription = null;
            this.textBoxTelNumber.AccessibleName = null;
            resources.ApplyResources(this.textBoxTelNumber, "textBoxTelNumber");
            this.textBoxTelNumber.BackgroundImage = null;
            this.textBoxTelNumber.Font = null;
            this.textBoxTelNumber.Name = "textBoxTelNumber";
            // 
            // label26
            // 
            this.label26.AccessibleDescription = null;
            this.label26.AccessibleName = null;
            resources.ApplyResources(this.label26, "label26");
            this.label26.Font = null;
            this.label26.Name = "label26";
            // 
            // tabPage5
            // 
            this.tabPage5.AccessibleDescription = null;
            this.tabPage5.AccessibleName = null;
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.BackgroundImage = null;
            this.tabPage5.Controls.Add(this.userControlConfigCryptography1);
            this.tabPage5.Font = null;
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            this.tabPage5.Click += new System.EventHandler(this.tabPage5_Click);
            // 
            // userControlConfigCryptography1
            // 
            this.userControlConfigCryptography1.AccessibleDescription = null;
            this.userControlConfigCryptography1.AccessibleName = null;
            resources.ApplyResources(this.userControlConfigCryptography1, "userControlConfigCryptography1");
            this.userControlConfigCryptography1.BackgroundImage = null;
            this.userControlConfigCryptography1.Font = null;
            this.userControlConfigCryptography1.Name = "userControlConfigCryptography1";
            // 
            // label32
            // 
            this.label32.AccessibleDescription = null;
            this.label32.AccessibleName = null;
            resources.ApplyResources(this.label32, "label32");
            this.label32.Font = null;
            this.label32.Name = "label32";
            // 
            // textBoxPortNumber
            // 
            this.textBoxPortNumber.AccessibleDescription = null;
            this.textBoxPortNumber.AccessibleName = null;
            resources.ApplyResources(this.textBoxPortNumber, "textBoxPortNumber");
            this.textBoxPortNumber.BackgroundImage = null;
            this.textBoxPortNumber.Font = null;
            this.textBoxPortNumber.Name = "textBoxPortNumber";
            this.textBoxPortNumber.ReadOnly = true;
            // 
            // FormConfigPort
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.label32);
            this.Controls.Add(this.textBoxPortNumber);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBoxReadMethod);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxActive);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigPort";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigPort_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownReadCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWriteCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreadCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeWORD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeFLOAT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeDWORD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeSTRING)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeDOUBLE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeINT64)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeoutRead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeoutWrite)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBoxLineDuplexProtocol.ResumeLayout(false);
            this.groupBoxLineDuplexProtocol.PerformLayout();
            this.groupBoxLineDuplexDevice.ResumeLayout(false);
            this.groupBoxLineDuplexDevice.PerformLayout();
            this.groupBoxLineDuplexCondition.ResumeLayout(false);
            this.groupBoxLineDuplexCondition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineDuplexCodebad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineDuplexTimeout)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBoxComputerDuplexAnotherComputer.ResumeLayout(false);
            this.groupBoxComputerDuplexAnotherComputer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComputerDualPort)).EndInit();
            this.groupBoxComputerDuplexCondition.ResumeLayout(false);
            this.groupBoxComputerDuplexCondition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownComputerDuplexTimeout)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTelConnectionTimeManual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTelConnectionTimeAuto)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTelConnectionCycle)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxActive;
        private System.Windows.Forms.CheckBox checkBoxThread;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownReadCycle;
        private System.Windows.Forms.NumericUpDown numericUpDownWriteCycle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDevice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownThreadCycle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonDevice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxProtocol;
        private System.Windows.Forms.TextBox textBoxProtocolOption;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonProtocolOption;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeWORD;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeFLOAT;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeDWORD;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeSTRING;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeDOUBLE;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeINT64;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numericUpDownTimeoutRead;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericUpDownTimeoutWrite;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxReadMethod;
        private System.Windows.Forms.CheckBox checkBoxUseStationInfomation;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox groupBoxLineDuplexProtocol;
        private System.Windows.Forms.ComboBox comboBoxSecondaryProtocol;
        private System.Windows.Forms.GroupBox groupBoxLineDuplexDevice;
        private System.Windows.Forms.Button buttonSecondaryDevice;
        private System.Windows.Forms.TextBox textBoxSecondaryDevice;
        private System.Windows.Forms.GroupBox groupBoxLineDuplexCondition;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.NumericUpDown numericUpDownLineDuplexTimeout;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.CheckBox checkBoxUseLineDuplex;
        private System.Windows.Forms.CheckBox checkBoxUseSecondaryProtocol;
        private System.Windows.Forms.GroupBox groupBoxComputerDuplexAnotherComputer;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox textBoxComputerDuplexIP;
        private System.Windows.Forms.GroupBox groupBoxComputerDuplexCondition;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown numericUpDownComputerDuplexTimeout;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox checkBoxUseComputerDuplex;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxTelAutoConnection;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown numericUpDownTelConnectionCycle;
        private System.Windows.Forms.TextBox textBoxTelNumber;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.NumericUpDown numericUpDownTelConnectionTimeManual;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.NumericUpDown numericUpDownTelConnectionTimeAuto;
        private System.Windows.Forms.CheckBox checkBoxComputerDuplexThread;
        private System.Windows.Forms.NumericUpDown numericUpDownComputerDualPort;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox textBoxPortNumber;
        private System.Windows.Forms.CheckBox checkBoxUseDeviceInformation;
        private System.Windows.Forms.TabPage tabPage5;
        private UserControlConfigCryptography userControlConfigCryptography1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.NumericUpDown numericUpDownLineDuplexCodebad;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox textBoxSecondaryProtocolOption;
        private System.Windows.Forms.Button buttonSecondaryProtocolOption;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label36;
    }
}