using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetCommon;
using NetTools;

namespace NetServer
{
	/// <summary>
	/// Summary description for FormConfigServer.
	/// </summary>
	public class FormConfigServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxServerName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonServerType0;
		private System.Windows.Forms.RadioButton radioButtonServerType1;
		private System.Windows.Forms.RadioButton radioButtonServerType2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownPort;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBoxConditionCommunicationError;
		private System.Windows.Forms.CheckBox checkBoxConditionExitLocalMain;
		private System.Windows.Forms.CheckBox checkBoxConditionExitPlcScan;
		private System.Windows.Forms.CheckBox checkBoxConditionProtectChangeTime;
		private System.Windows.Forms.CheckBox checkBoxSupportPlcScan;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox checkBoxChangeLinePrinter;
		private System.Windows.Forms.CheckBox checkBoxChangeReportPrinter;
		private System.Windows.Forms.NumericUpDown numericUpDownTcpPort;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxConditionExitRunMain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigServer()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigServer));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxServerName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonServerType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonServerType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonServerType0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownTcpPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxConditionExitRunMain = new System.Windows.Forms.CheckBox();
            this.checkBoxConditionProtectChangeTime = new System.Windows.Forms.CheckBox();
            this.checkBoxConditionExitPlcScan = new System.Windows.Forms.CheckBox();
            this.checkBoxConditionExitLocalMain = new System.Windows.Forms.CheckBox();
            this.checkBoxConditionCommunicationError = new System.Windows.Forms.CheckBox();
            this.checkBoxSupportPlcScan = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxChangeReportPrinter = new System.Windows.Forms.CheckBox();
            this.checkBoxChangeLinePrinter = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            // textBoxServerName
            // 
            this.textBoxServerName.AccessibleDescription = null;
            this.textBoxServerName.AccessibleName = null;
            resources.ApplyResources(this.textBoxServerName, "textBoxServerName");
            this.textBoxServerName.BackgroundImage = null;
            this.textBoxServerName.Font = null;
            this.textBoxServerName.Name = "textBoxServerName";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonServerType2);
            this.groupBox1.Controls.Add(this.radioButtonServerType1);
            this.groupBox1.Controls.Add(this.radioButtonServerType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonServerType2
            // 
            this.radioButtonServerType2.AccessibleDescription = null;
            this.radioButtonServerType2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonServerType2, "radioButtonServerType2");
            this.radioButtonServerType2.BackgroundImage = null;
            this.radioButtonServerType2.Font = null;
            this.radioButtonServerType2.Name = "radioButtonServerType2";
            // 
            // radioButtonServerType1
            // 
            this.radioButtonServerType1.AccessibleDescription = null;
            this.radioButtonServerType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonServerType1, "radioButtonServerType1");
            this.radioButtonServerType1.BackgroundImage = null;
            this.radioButtonServerType1.Font = null;
            this.radioButtonServerType1.Name = "radioButtonServerType1";
            // 
            // radioButtonServerType0
            // 
            this.radioButtonServerType0.AccessibleDescription = null;
            this.radioButtonServerType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonServerType0, "radioButtonServerType0");
            this.radioButtonServerType0.BackgroundImage = null;
            this.radioButtonServerType0.Font = null;
            this.radioButtonServerType0.Name = "radioButtonServerType0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.numericUpDownTcpPort);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numericUpDownPort);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // numericUpDownTcpPort
            // 
            this.numericUpDownTcpPort.AccessibleDescription = null;
            this.numericUpDownTcpPort.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTcpPort, "numericUpDownTcpPort");
            this.numericUpDownTcpPort.Font = null;
            this.numericUpDownTcpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownTcpPort.Name = "numericUpDownTcpPort";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.AccessibleDescription = null;
            this.numericUpDownPort.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownPort, "numericUpDownPort");
            this.numericUpDownPort.Font = null;
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.checkBoxConditionExitRunMain);
            this.groupBox3.Controls.Add(this.checkBoxConditionProtectChangeTime);
            this.groupBox3.Controls.Add(this.checkBoxConditionExitPlcScan);
            this.groupBox3.Controls.Add(this.checkBoxConditionExitLocalMain);
            this.groupBox3.Controls.Add(this.checkBoxConditionCommunicationError);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // checkBoxConditionExitRunMain
            // 
            this.checkBoxConditionExitRunMain.AccessibleDescription = null;
            this.checkBoxConditionExitRunMain.AccessibleName = null;
            resources.ApplyResources(this.checkBoxConditionExitRunMain, "checkBoxConditionExitRunMain");
            this.checkBoxConditionExitRunMain.BackgroundImage = null;
            this.checkBoxConditionExitRunMain.Font = null;
            this.checkBoxConditionExitRunMain.Name = "checkBoxConditionExitRunMain";
            // 
            // checkBoxConditionProtectChangeTime
            // 
            this.checkBoxConditionProtectChangeTime.AccessibleDescription = null;
            this.checkBoxConditionProtectChangeTime.AccessibleName = null;
            resources.ApplyResources(this.checkBoxConditionProtectChangeTime, "checkBoxConditionProtectChangeTime");
            this.checkBoxConditionProtectChangeTime.BackgroundImage = null;
            this.checkBoxConditionProtectChangeTime.Font = null;
            this.checkBoxConditionProtectChangeTime.Name = "checkBoxConditionProtectChangeTime";
            // 
            // checkBoxConditionExitPlcScan
            // 
            this.checkBoxConditionExitPlcScan.AccessibleDescription = null;
            this.checkBoxConditionExitPlcScan.AccessibleName = null;
            resources.ApplyResources(this.checkBoxConditionExitPlcScan, "checkBoxConditionExitPlcScan");
            this.checkBoxConditionExitPlcScan.BackgroundImage = null;
            this.checkBoxConditionExitPlcScan.Font = null;
            this.checkBoxConditionExitPlcScan.Name = "checkBoxConditionExitPlcScan";
            // 
            // checkBoxConditionExitLocalMain
            // 
            this.checkBoxConditionExitLocalMain.AccessibleDescription = null;
            this.checkBoxConditionExitLocalMain.AccessibleName = null;
            resources.ApplyResources(this.checkBoxConditionExitLocalMain, "checkBoxConditionExitLocalMain");
            this.checkBoxConditionExitLocalMain.BackgroundImage = null;
            this.checkBoxConditionExitLocalMain.Font = null;
            this.checkBoxConditionExitLocalMain.Name = "checkBoxConditionExitLocalMain";
            // 
            // checkBoxConditionCommunicationError
            // 
            this.checkBoxConditionCommunicationError.AccessibleDescription = null;
            this.checkBoxConditionCommunicationError.AccessibleName = null;
            resources.ApplyResources(this.checkBoxConditionCommunicationError, "checkBoxConditionCommunicationError");
            this.checkBoxConditionCommunicationError.BackgroundImage = null;
            this.checkBoxConditionCommunicationError.Font = null;
            this.checkBoxConditionCommunicationError.Name = "checkBoxConditionCommunicationError";
            // 
            // checkBoxSupportPlcScan
            // 
            this.checkBoxSupportPlcScan.AccessibleDescription = null;
            this.checkBoxSupportPlcScan.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSupportPlcScan, "checkBoxSupportPlcScan");
            this.checkBoxSupportPlcScan.BackgroundImage = null;
            this.checkBoxSupportPlcScan.Font = null;
            this.checkBoxSupportPlcScan.Name = "checkBoxSupportPlcScan";
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.checkBoxChangeReportPrinter);
            this.groupBox4.Controls.Add(this.checkBoxChangeLinePrinter);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // checkBoxChangeReportPrinter
            // 
            this.checkBoxChangeReportPrinter.AccessibleDescription = null;
            this.checkBoxChangeReportPrinter.AccessibleName = null;
            resources.ApplyResources(this.checkBoxChangeReportPrinter, "checkBoxChangeReportPrinter");
            this.checkBoxChangeReportPrinter.BackgroundImage = null;
            this.checkBoxChangeReportPrinter.Font = null;
            this.checkBoxChangeReportPrinter.Name = "checkBoxChangeReportPrinter";
            // 
            // checkBoxChangeLinePrinter
            // 
            this.checkBoxChangeLinePrinter.AccessibleDescription = null;
            this.checkBoxChangeLinePrinter.AccessibleName = null;
            resources.ApplyResources(this.checkBoxChangeLinePrinter, "checkBoxChangeLinePrinter");
            this.checkBoxChangeLinePrinter.BackgroundImage = null;
            this.checkBoxChangeLinePrinter.Font = null;
            this.checkBoxChangeLinePrinter.Name = "checkBoxChangeLinePrinter";
            // 
            // FormConfigServer
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxSupportPlcScan);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxServerName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigServer";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigServer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FormConfigServer_Load(object sender, System.EventArgs e)
		{
			this.textBoxServerName.Text = configStruct.sNodeName;
			this.radioButtonServerType0.Checked = (configStruct.nNodeType == 0);
			this.radioButtonServerType1.Checked = (configStruct.nNodeType == 1);
			this.radioButtonServerType2.Checked = (configStruct.nNodeType == 2);
			this.numericUpDownPort.Value = ConfigNetCommon.nServerPort;
			this.numericUpDownTcpPort.Value = configStruct.nTcpPort;
			this.checkBoxConditionCommunicationError.Checked = (configStruct.bChangeConditionPlcScanTimeOut == 1);
			this.checkBoxConditionExitLocalMain.Checked = (configStruct.bChangeConditionProgramViewMain == 1);
			this.checkBoxConditionExitPlcScan.Checked = (configStruct.bChangeConditionProgramPlcScan == 1);
			this.checkBoxConditionExitRunMain.Checked = (configStruct.bChangeConditionProgramRunMain == 1);
			this.checkBoxConditionProtectChangeTime.Checked = (configStruct.bChangeConditionProtectChangeTime == 1);
			this.checkBoxChangeLinePrinter.Checked = configStruct.bLatchLinePrinter == 1;
			this.checkBoxChangeReportPrinter.Checked = configStruct.bLatchReportPrinter == 1;
			this.checkBoxSupportPlcScan.Checked = configStruct.bSupportPlcScanMemory == 1;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			configStruct.sNodeName = this.textBoxServerName.Text;
			
			if(this.radioButtonServerType0.Checked)	configStruct.nNodeType = 0;
			else if(this.radioButtonServerType1.Checked)	configStruct.nNodeType = 1;
			else if(this.radioButtonServerType2.Checked)	configStruct.nNodeType = 2;
			else											configStruct.nNodeType = 0;

			ConfigNetCommon.nServerPort = ConvertTool.ToInt32(this.numericUpDownPort.Value);
			configStruct.nTcpPort = ConvertTool.ToInt32(this.numericUpDownTcpPort.Value);
			configStruct.bChangeConditionPlcScanTimeOut = this.checkBoxConditionCommunicationError.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bChangeConditionProgramViewMain = this.checkBoxConditionExitLocalMain.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bChangeConditionProgramPlcScan = this.checkBoxConditionExitPlcScan.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bChangeConditionProgramRunMain = this.checkBoxConditionExitRunMain.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bChangeConditionProtectChangeTime = this.checkBoxConditionProtectChangeTime.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bLatchLinePrinter = this.checkBoxChangeLinePrinter.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bLatchReportPrinter = this.checkBoxChangeReportPrinter.Checked ? (sbyte)1 : (sbyte)0;
			configStruct.bSupportPlcScanMemory = this.checkBoxSupportPlcScan.Checked ? (sbyte)1 : (sbyte)0;

			configStruct.Save();

			FormViewServer.InvalidateServerStatusView();

			DialogResult = DialogResult.OK;
			Close();
		}


	}
}

/*

#define	MAX_NODE_TYPE	4

BOOL CDialogConfigNode::OnInitDialog() 
{
CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
SetRadioPosition(m_hWnd, IDC_ConfigNode_RADIO_TYPE0, 4, m_nodeType);
m_spin_serverPort.SetRange(0, 9999);

return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void ConfigNode()
{ 
CDialogConfigNode dialog;
	
dialog.m_nodeName = configStruct.sNodeName;
dialog.m_nodeType = configStruct.nNodeType;
dialog.m_edit_serverPort = configStruct.nServerPort;
dialog.m_bChangeConditionPlcScanTimeOut = configStruct.bChangeConditionPlcScanTimeOut;
dialog.m_bChangeConditionProgramViewMain = configStruct.bChangeConditionProgramViewMain;
dialog.m_bChangeConditionProgramPlcScan = configStruct.bChangeConditionProgramPlcScan;
dialog.m_bChangeConditionProgramRunMain = configStruct.bChangeConditionProgramRunMain;
dialog.m_bChangeConditionProtectChangeTime = configStruct.bChangeConditionProtectChangeTime;
dialog.m_latchLinePrinter = configStruct.bLatchLinePrinter;
dialog.m_latchReportPrinter = configStruct.bLatchReportPrinter;
dialog.m_bSupportPlcScanMemory = configStruct.bSupportPlcScanMemory;
 	
if(dialog.DoModal() == IDOK) 
{
UninitDevice();

strcpy(configStruct.sNodeName, dialog.m_nodeName);
configStruct.nNodeType = dialog.m_nodeType;
configStruct.nServerPort = dialog.m_edit_serverPort;

configStruct.bChangeConditionPlcScanTimeOut = dialog.m_bChangeConditionPlcScanTimeOut;
configStruct.bChangeConditionProgramViewMain = dialog.m_bChangeConditionProgramViewMain;
configStruct.bChangeConditionProgramPlcScan = dialog.m_bChangeConditionProgramPlcScan;
configStruct.bChangeConditionProgramRunMain = dialog.m_bChangeConditionProgramRunMain;
configStruct.bChangeConditionProtectChangeTime = dialog.m_bChangeConditionProtectChangeTime;

configStruct.bLatchLinePrinter = dialog.m_latchLinePrinter;
configStruct.bLatchReportPrinter = dialog.m_latchReportPrinter;

configStruct.bSupportPlcScanMemory = dialog.m_bSupportPlcScanMemory;

InvalidateServerStatusView();

InitDevice(AfxGetMainWnd()->m_hWnd);

nUdpPort = configStruct.nServerPort;
}
}

void CDialogConfigNode::OnOK() 
{
	// TODO: Add extra validation here
m_nodeType = GetRadioPosition(m_hWnd, IDC_ConfigNode_RADIO_TYPE0, 4);
	
CDialog::OnOK();
}




*/ 
