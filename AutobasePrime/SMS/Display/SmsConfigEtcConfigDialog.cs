using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SMS.SmsFunc;
using SMS.SmsComm;
using NetTools;

namespace SMS.Display
{
	/// <summary>
	/// Summary description for SmsConfigEtcConfigDialog.
	/// </summary>
	public class SmsConfigEtcConfigDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDown_DefaultRetry;
		private System.Windows.Forms.NumericUpDown numericUpDown_LineRetry;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown numericUpDown_StartDelay;
		private System.Windows.Forms.NumericUpDown numericUpDown_UserDelay;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.NumericUpDown numericUpDown_ConfirmTimeout;
		private System.Windows.Forms.NumericUpDown numericUpDown_ControlTimeout;
		private System.Windows.Forms.NumericUpDown numericUpDown_MaxChar;
		private System.Windows.Forms.ComboBox comboBox_MessageType;
		private System.Windows.Forms.TextBox textBox_SendTelNum;
		private System.Windows.Forms.CheckBox checkBox_CallEnable;
		private System.Windows.Forms.ComboBox comboBox_PcsModel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private Label label14;
        private NumericUpDown numericUpDown_ModiconID;
        private NumericUpDown numericUpDownMaxCountOfDay;
        private Label label15;
		public bool	bSmsCallStatus;

		public SmsConfigEtcConfigDialog()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmsConfigEtcConfigDialog));
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_LineRetry = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_DefaultRetry = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_StartDelay = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_UserDelay = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown_ConfirmTimeout = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDown_ControlTimeout = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numericUpDown_MaxChar = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.comboBox_MessageType = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBox_SendTelNum = new System.Windows.Forms.TextBox();
            this.checkBox_CallEnable = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDown_ModiconID = new System.Windows.Forms.NumericUpDown();
            this.comboBox_PcsModel = new System.Windows.Forms.ComboBox();
            this.numericUpDownMaxCountOfDay = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LineRetry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DefaultRetry)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_StartDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_UserDelay)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ConfirmTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ControlTimeout)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MaxChar)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ModiconID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxCountOfDay)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDown_LineRetry);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDown_DefaultRetry);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDown_LineRetry
            // 
            resources.ApplyResources(this.numericUpDown_LineRetry, "numericUpDown_LineRetry");
            this.numericUpDown_LineRetry.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_LineRetry.Name = "numericUpDown_LineRetry";
            this.numericUpDown_LineRetry.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDown_DefaultRetry
            // 
            resources.ApplyResources(this.numericUpDown_DefaultRetry, "numericUpDown_DefaultRetry");
            this.numericUpDown_DefaultRetry.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_DefaultRetry.Name = "numericUpDown_DefaultRetry";
            this.numericUpDown_DefaultRetry.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDown_StartDelay);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numericUpDown_UserDelay);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label7);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // numericUpDown_StartDelay
            // 
            resources.ApplyResources(this.numericUpDown_StartDelay, "numericUpDown_StartDelay");
            this.numericUpDown_StartDelay.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDown_StartDelay.Name = "numericUpDown_StartDelay";
            this.numericUpDown_StartDelay.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDown_UserDelay
            // 
            resources.ApplyResources(this.numericUpDown_UserDelay, "numericUpDown_UserDelay");
            this.numericUpDown_UserDelay.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDown_UserDelay.Name = "numericUpDown_UserDelay";
            this.numericUpDown_UserDelay.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numericUpDown_ConfirmTimeout);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.numericUpDown_ControlTimeout);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // numericUpDown_ConfirmTimeout
            // 
            resources.ApplyResources(this.numericUpDown_ConfirmTimeout, "numericUpDown_ConfirmTimeout");
            this.numericUpDown_ConfirmTimeout.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDown_ConfirmTimeout.Name = "numericUpDown_ConfirmTimeout";
            this.numericUpDown_ConfirmTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // numericUpDown_ControlTimeout
            // 
            resources.ApplyResources(this.numericUpDown_ControlTimeout, "numericUpDown_ControlTimeout");
            this.numericUpDown_ControlTimeout.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDown_ControlTimeout.Name = "numericUpDown_ControlTimeout";
            this.numericUpDown_ControlTimeout.Value = new decimal(new int[] {
            5,
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numericUpDown_MaxChar);
            this.groupBox4.Controls.Add(this.label13);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // numericUpDown_MaxChar
            // 
            resources.ApplyResources(this.numericUpDown_MaxChar, "numericUpDown_MaxChar");
            this.numericUpDown_MaxChar.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_MaxChar.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_MaxChar.Name = "numericUpDown_MaxChar";
            this.numericUpDown_MaxChar.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.comboBox_MessageType);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // comboBox_MessageType
            // 
            this.comboBox_MessageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox_MessageType, "comboBox_MessageType");
            this.comboBox_MessageType.Name = "comboBox_MessageType";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBox_SendTelNum);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // textBox_SendTelNum
            // 
            resources.ApplyResources(this.textBox_SendTelNum, "textBox_SendTelNum");
            this.textBox_SendTelNum.Name = "textBox_SendTelNum";
            // 
            // checkBox_CallEnable
            // 
            resources.ApplyResources(this.checkBox_CallEnable, "checkBox_CallEnable");
            this.checkBox_CallEnable.Name = "checkBox_CallEnable";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label14);
            this.groupBox7.Controls.Add(this.numericUpDown_ModiconID);
            this.groupBox7.Controls.Add(this.comboBox_PcsModel);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // numericUpDown_ModiconID
            // 
            resources.ApplyResources(this.numericUpDown_ModiconID, "numericUpDown_ModiconID");
            this.numericUpDown_ModiconID.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_ModiconID.Name = "numericUpDown_ModiconID";
            this.numericUpDown_ModiconID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // comboBox_PcsModel
            // 
            this.comboBox_PcsModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox_PcsModel, "comboBox_PcsModel");
            this.comboBox_PcsModel.Name = "comboBox_PcsModel";
            this.comboBox_PcsModel.SelectedIndexChanged += new System.EventHandler(this.comboBox_PcsModel_SelectedIndexChanged);
            // 
            // numericUpDownMaxCountOfDay
            // 
            resources.ApplyResources(this.numericUpDownMaxCountOfDay, "numericUpDownMaxCountOfDay");
            this.numericUpDownMaxCountOfDay.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownMaxCountOfDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxCountOfDay.Name = "numericUpDownMaxCountOfDay";
            this.numericUpDownMaxCountOfDay.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // SmsConfigEtcConfigDialog
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_Cancel;
            this.Controls.Add(this.numericUpDownMaxCountOfDay);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.checkBox_CallEnable);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SmsConfigEtcConfigDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.SmsConfigEtcConfigDialog_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_LineRetry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DefaultRetry)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_StartDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_UserDelay)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ConfirmTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ControlTimeout)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_MaxChar)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ModiconID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxCountOfDay)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void SmsConfigEtcConfigDialog_Load(object sender, System.EventArgs e)
		{
			this.bSmsCallStatus = SmsBasic.smsConfig.bStopSmsCall;
			try 
			{
				this.numericUpDown_DefaultRetry.Value = SmsBasic.smsConfig.nRetryCount;
				this.numericUpDown_LineRetry.Value = SmsBasic.smsConfig.nRetryErrorCount;
				this.numericUpDown_UserDelay.Value = SmsBasic.smsConfig.nDelaySiteCall;
				this.numericUpDown_StartDelay.Value = SmsBasic.smsConfig.nDelayDataCall;
				this.numericUpDown_ControlTimeout.Value = SmsBasic.smsConfig.nTimeOutBasic;
				this.numericUpDown_ConfirmTimeout.Value = SmsBasic.smsConfig.nTimeOutMessage;
				this.numericUpDown_MaxChar.Value = SmsBasic.smsConfig.nMaxSendChar;
                this.numericUpDownMaxCountOfDay.Value = SmsBasic.smsConfig.nMaxCountOfDay;
			}
			catch {}

			this.checkBox_CallEnable.Checked = !SmsBasic.smsConfig.bStopSmsCall;
			this.textBox_SendTelNum.Text = SmsBasic.smsConfig.sSendNumber;
			if(Tools.IsLangKorean()) 
			{
				this.comboBox_MessageType.Items.Add("일반");
				this.comboBox_MessageType.Items.Add("빠름");
				this.comboBox_MessageType.Items.Add("긴급");

				this.comboBox_PcsModel.Items.Add("LG Mobile Messanger");
				this.comboBox_PcsModel.Items.Add("삼성 애니콜");
				this.comboBox_PcsModel.Items.Add("삼성 애니콜 CDMA");
				this.comboBox_PcsModel.Items.Add("삼성 애니콜 CDMA 2000");
				this.comboBox_PcsModel.Items.Add("삼성 애니콜 CDMA2000(SPH-X4019 외)");
				this.comboBox_PcsModel.Items.Add("Air NCL");
                this.comboBox_PcsModel.Items.Add("Telit BSM860S");     // 2007-10-22
                this.comboBox_PcsModel.Items.Add("KTF Mobicon");     // 2010-11-18
                this.comboBox_PcsModel.Items.Add("GSM Modem");       // add 2012-04-09 for vietnam
                this.comboBox_PcsModel.Items.Add("EMV-1800K CDMA");  // add 2012-12-18
                this.comboBox_PcsModel.Items.Add("M2M WM-215");     // add 2016-1-8
			}
			else 
			{
				this.comboBox_MessageType.Items.Add("Normal");
				this.comboBox_MessageType.Items.Add("Fast");
				this.comboBox_MessageType.Items.Add("Urgency");

				this.comboBox_PcsModel.Items.Add("LG Mobile Messanger");
				this.comboBox_PcsModel.Items.Add("Samsung Any Call");
				this.comboBox_PcsModel.Items.Add("Samsung Any Call CDMA");
				this.comboBox_PcsModel.Items.Add("Samsung Any Call CDMA 2000"); 
				this.comboBox_PcsModel.Items.Add("Samsung Any Call CDMA2000(SPH-X4019 Etc)");
				this.comboBox_PcsModel.Items.Add("Air NCL");
                this.comboBox_PcsModel.Items.Add("Telit BSM860S");    // 2007-10-22
                this.comboBox_PcsModel.Items.Add("KTF Mobicon");     // 2010-11-18
                this.comboBox_PcsModel.Items.Add("GSM Modem");       // add 2012-04-09 for vietnam
                this.comboBox_PcsModel.Items.Add("EMV-1800K CDMA");  // add 2012-12-18
                this.comboBox_PcsModel.Items.Add("M2M WM-215");     // add 2016-1-8
			}
			this.comboBox_MessageType.SelectedIndex = ((int)SmsBasic.smsConfig.eMessageType-1) % 3;
			this.comboBox_PcsModel.SelectedIndex = (int)SmsBasic.smsConfig.eCdma % 11;
            this.numericUpDown_ModiconID.Value = (byte)SmsBasic.smsConfig.cMobiconID;// add 2010-11-18
            numericUpDown_ModiconEnabelDisable();// add 2010-11-18
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			this.Focus();

			SmsBasic.smsConfig.nRetryCount = (int)this.numericUpDown_DefaultRetry.Value;
			SmsBasic.smsConfig.nRetryErrorCount = (int)this.numericUpDown_LineRetry.Value;
			SmsBasic.smsConfig.nDelaySiteCall = (int)this.numericUpDown_UserDelay.Value;
			SmsBasic.smsConfig.nDelayDataCall = (int)this.numericUpDown_StartDelay.Value;
			SmsBasic.smsConfig.nTimeOutBasic = (int)this.numericUpDown_ControlTimeout.Value;
			SmsBasic.smsConfig.nTimeOutMessage = (int)this.numericUpDown_ConfirmTimeout.Value;
			SmsBasic.smsConfig.bStopSmsCall = !checkBox_CallEnable.Checked;
			SmsBasic.smsConfig.sSendNumber = this.textBox_SendTelNum.Text;
			SmsBasic.smsConfig.nMaxSendChar = (int)this.numericUpDown_MaxChar.Value;
			SmsBasic.smsConfig.eMessageType = (SendSMSData.eMsgType)((this.comboBox_MessageType.SelectedIndex % 3) + 1);
			SmsBasic.smsConfig.eCdma = (SendSMSData.eCdmaType)this.comboBox_PcsModel.SelectedIndex;
			SmsBasic.smsConfig.bChange = true;
            SmsBasic.smsConfig.cMobiconID = (byte)this.numericUpDown_ModiconID.Value;// add 2010-11-18

            SmsBasic.smsConfig.nMaxCountOfDay = ConvertTool.ToInt32(this.numericUpDownMaxCountOfDay.Value);// add 2011-11-23

            SmsFileDataLoadSave.SaveSmsConfigData();    // 스튜디오에서 바로 CE로 배포할 수 있도록 환경을 바로 저장해 준다.

			this.DialogResult = DialogResult.OK;
			Close();
		}

        private void comboBox_PcsModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown_ModiconEnabelDisable();// add 2010-11-18
        }

        private void numericUpDown_ModiconEnabelDisable()// add 2010-11-18
        {
            if ((SendSMSData.eCdmaType)this.comboBox_PcsModel.SelectedIndex == SendSMSData.eCdmaType.KTF_MOBICON) numericUpDown_ModiconID.Enabled = true;
            else numericUpDown_ModiconID.Enabled = false;
        }
	}
}
