using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using AutoLibLocal;

namespace ViewMain
{
	/// <summary>
	/// Summary description for FormConfigEtc.
	/// </summary>
	public class FormConfigEtc : System.Windows.Forms.Form
	{
		private System.Windows.Forms.CheckBox checkBoxFitToWindow;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.CheckBox checkBoxDisplayToolTip;
		private System.Windows.Forms.CheckBox checkBoxResponseOnMouseLeft;
		private System.Windows.Forms.CheckBox checkBoxResponseOnMouseRight;
		private System.Windows.Forms.CheckBox checkBoxUseMenuButton;
		private System.Windows.Forms.GroupBox groupBox1;
        private TabPage tabPage1;
        private GroupBox groupBox2;
        private Label label2;
        private NumericUpDown numericUpDownTimeoutOfExcelReportRunDirect;
        private TabPage tabPage4;
        private GroupBox groupBox3;
        private Label label4;
        private NumericUpDown numericUpDownForLoopTimeout;
        private CheckBox checkBoxAllowManualOutputOnClient;
        private CheckBox checkBoxUseAlarmServer;
        private CheckBox checkBoxEnablePreviewValue;
        private CheckBox checkBoxShowUserManualControl;
        private Label label3;
        private NumericUpDown numericUpDownMaxReport;
        private NumericUpDown numericUpDownMaxBasicScreen;
        private Label label5;
        private Label label1;
        private CheckBox checkBoxAlwaysOpenNewReport;
        private CheckBox checkBoxSortingEventWindow;
        private TabPage tabPage5;
        private GroupBox groupBox4;
        private ListBox listBoxWebClient;
        private Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownMdiCount;

		public FormConfigEtc()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigEtc));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxFitToWindow = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxShowUserManualControl = new System.Windows.Forms.CheckBox();
            this.checkBoxEnablePreviewValue = new System.Windows.Forms.CheckBox();
            this.checkBoxSortingEventWindow = new System.Windows.Forms.CheckBox();
            this.checkBoxUseAlarmServer = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxAlwaysOpenNewReport = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownMaxReport = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMaxBasicScreen = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownMdiCount = new System.Windows.Forms.NumericUpDown();
            this.checkBoxUseMenuButton = new System.Windows.Forms.CheckBox();
            this.checkBoxResponseOnMouseRight = new System.Windows.Forms.CheckBox();
            this.checkBoxResponseOnMouseLeft = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayToolTip = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.checkBoxAllowManualOutputOnClient = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownForLoopTimeout = new System.Windows.Forms.NumericUpDown();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownTimeoutOfExcelReportRunDirect = new System.Windows.Forms.NumericUpDown();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listBoxWebClient = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxBasicScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMdiCount)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownForLoopTimeout)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeoutOfExcelReportRunDirect)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // checkBoxFitToWindow
            // 
            resources.ApplyResources(this.checkBoxFitToWindow, "checkBoxFitToWindow");
            this.checkBoxFitToWindow.Name = "checkBoxFitToWindow";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBoxShowUserManualControl);
            this.tabPage3.Controls.Add(this.checkBoxEnablePreviewValue);
            this.tabPage3.Controls.Add(this.checkBoxSortingEventWindow);
            this.tabPage3.Controls.Add(this.checkBoxUseAlarmServer);
            this.tabPage3.Controls.Add(this.checkBoxFitToWindow);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowUserManualControl
            // 
            resources.ApplyResources(this.checkBoxShowUserManualControl, "checkBoxShowUserManualControl");
            this.checkBoxShowUserManualControl.Name = "checkBoxShowUserManualControl";
            this.checkBoxShowUserManualControl.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnablePreviewValue
            // 
            resources.ApplyResources(this.checkBoxEnablePreviewValue, "checkBoxEnablePreviewValue");
            this.checkBoxEnablePreviewValue.Name = "checkBoxEnablePreviewValue";
            // 
            // checkBoxSortingEventWindow
            // 
            resources.ApplyResources(this.checkBoxSortingEventWindow, "checkBoxSortingEventWindow");
            this.checkBoxSortingEventWindow.Name = "checkBoxSortingEventWindow";
            // 
            // checkBoxUseAlarmServer
            // 
            resources.ApplyResources(this.checkBoxUseAlarmServer, "checkBoxUseAlarmServer");
            this.checkBoxUseAlarmServer.Name = "checkBoxUseAlarmServer";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBoxAlwaysOpenNewReport);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.checkBoxUseMenuButton);
            this.tabPage2.Controls.Add(this.checkBoxResponseOnMouseRight);
            this.tabPage2.Controls.Add(this.checkBoxResponseOnMouseLeft);
            this.tabPage2.Controls.Add(this.checkBoxDisplayToolTip);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // checkBoxAlwaysOpenNewReport
            // 
            resources.ApplyResources(this.checkBoxAlwaysOpenNewReport, "checkBoxAlwaysOpenNewReport");
            this.checkBoxAlwaysOpenNewReport.Name = "checkBoxAlwaysOpenNewReport";
            this.checkBoxAlwaysOpenNewReport.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownMaxReport);
            this.groupBox1.Controls.Add(this.numericUpDownMaxBasicScreen);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownMdiCount);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownMaxReport
            // 
            resources.ApplyResources(this.numericUpDownMaxReport, "numericUpDownMaxReport");
            this.numericUpDownMaxReport.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMaxReport.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxReport.Name = "numericUpDownMaxReport";
            this.numericUpDownMaxReport.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericUpDownMaxBasicScreen
            // 
            resources.ApplyResources(this.numericUpDownMaxBasicScreen, "numericUpDownMaxBasicScreen");
            this.numericUpDownMaxBasicScreen.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownMaxBasicScreen.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxBasicScreen.Name = "numericUpDownMaxBasicScreen";
            this.numericUpDownMaxBasicScreen.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownMdiCount
            // 
            resources.ApplyResources(this.numericUpDownMdiCount, "numericUpDownMdiCount");
            this.numericUpDownMdiCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMdiCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMdiCount.Name = "numericUpDownMdiCount";
            this.numericUpDownMdiCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // checkBoxUseMenuButton
            // 
            resources.ApplyResources(this.checkBoxUseMenuButton, "checkBoxUseMenuButton");
            this.checkBoxUseMenuButton.Name = "checkBoxUseMenuButton";
            // 
            // checkBoxResponseOnMouseRight
            // 
            resources.ApplyResources(this.checkBoxResponseOnMouseRight, "checkBoxResponseOnMouseRight");
            this.checkBoxResponseOnMouseRight.Name = "checkBoxResponseOnMouseRight";
            // 
            // checkBoxResponseOnMouseLeft
            // 
            resources.ApplyResources(this.checkBoxResponseOnMouseLeft, "checkBoxResponseOnMouseLeft");
            this.checkBoxResponseOnMouseLeft.Name = "checkBoxResponseOnMouseLeft";
            // 
            // checkBoxDisplayToolTip
            // 
            resources.ApplyResources(this.checkBoxDisplayToolTip, "checkBoxDisplayToolTip");
            this.checkBoxDisplayToolTip.Name = "checkBoxDisplayToolTip";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.checkBoxAllowManualOutputOnClient);
            this.tabPage4.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // checkBoxAllowManualOutputOnClient
            // 
            resources.ApplyResources(this.checkBoxAllowManualOutputOnClient, "checkBoxAllowManualOutputOnClient");
            this.checkBoxAllowManualOutputOnClient.Name = "checkBoxAllowManualOutputOnClient";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.numericUpDownForLoopTimeout);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownForLoopTimeout
            // 
            resources.ApplyResources(this.numericUpDownForLoopTimeout, "numericUpDownForLoopTimeout");
            this.numericUpDownForLoopTimeout.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numericUpDownForLoopTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownForLoopTimeout.Name = "numericUpDownForLoopTimeout";
            this.numericUpDownForLoopTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.numericUpDownTimeoutOfExcelReportRunDirect);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownTimeoutOfExcelReportRunDirect
            // 
            resources.ApplyResources(this.numericUpDownTimeoutOfExcelReportRunDirect, "numericUpDownTimeoutOfExcelReportRunDirect");
            this.numericUpDownTimeoutOfExcelReportRunDirect.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTimeoutOfExcelReportRunDirect.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTimeoutOfExcelReportRunDirect.Name = "numericUpDownTimeoutOfExcelReportRunDirect";
            this.numericUpDownTimeoutOfExcelReportRunDirect.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label6);
            this.tabPage5.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxWebClient);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // listBoxWebClient
            // 
            resources.ApplyResources(this.listBoxWebClient, "listBoxWebClient");
            this.listBoxWebClient.Name = "listBoxWebClient";
            // 
            // FormConfigEtc
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigEtc";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigEtc_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxBasicScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMdiCount)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownForLoopTimeout)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeoutOfExcelReportRunDirect)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		static int nTempTabPos = 0;

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			// 화면에 그림을 맞춤을 설정하면 폰트가 바뀌었다는 메세지를  보내준다.
			bool bChangedFitToWindow = (ConfigViewMain.bFitToWindow != checkBoxFitToWindow.Checked);

			ConfigViewMain.bFitToWindow = checkBoxFitToWindow.Checked;
			ConfigViewMain.bDisplayToolTip = this.checkBoxDisplayToolTip.Checked;
			ConfigViewMain.bResponseMouseLeftOnGraphic = this.checkBoxResponseOnMouseLeft.Checked;
			ConfigViewMain.bResponseMouseRightOnGraphic = this.checkBoxResponseOnMouseRight.Checked;
			ConfigViewMain.bUseMenuButtonOnGraphic = this.checkBoxUseMenuButton.Checked;
			ConfigViewMain.nMdiCountOnGraphic = ConvertTool.ToInt32(this.numericUpDownMdiCount.Value);
            ConfigViewMain.nMdiCountOnBasicScreen = ConvertTool.ToInt32(this.numericUpDownMaxBasicScreen.Value);
            ConfigViewMain.nMdiCountOnReport = ConvertTool.ToInt32(this.numericUpDownMaxReport.Value);

            ConfigViewMain.nTimeoutOfExcelReportRunDirect = ConvertTool.ToInt32(this.numericUpDownTimeoutOfExcelReportRunDirect.Value);

            ConfigViewMain.nForLoopTimeout = ConvertTool.ToInt32(this.numericUpDownForLoopTimeout.Value);
            ConfigViewMain.bAllowManualOutputOnClient = this.checkBoxAllowManualOutputOnClient.Checked;

            ConfigViewMain.bUseAlarmServer = this.checkBoxUseAlarmServer.Checked;

            ConfigViewMain.bEnableSettingValuePreview = this.checkBoxEnablePreviewValue.Checked;
            ConfigViewMain.bShowUserManualControl = this.checkBoxShowUserManualControl.Checked;
            ConfigViewMain.bAlwaysOpenNewReport = this.checkBoxAlwaysOpenNewReport.Checked; //리포트 모듈을 새로 열기 24-07-01 hsjeong

            ConfigAlarm.bAlarmConfirmSorting = this.checkBoxSortingEventWindow.Checked; //경보이벤트창 정렬 20241111 PSU

			ConfigViewMain.Save();

			nTempTabPos = this.tabControl1.SelectedIndex;

			if(bChangedFitToWindow)	// 화면에 그림을 맞춤을 설정하면 폰트가 바뀌었다는 메세지를  보내준다.
				SharedViewMain.EventGoMainFontChanged();

            LanguageTool.SetLanguageWebClient((string)listBoxWebClient.SelectedItem); //LangTool 기능 20251119 PSU 

            DialogResult = DialogResult.OK;
			Close();
		}

		private void FormConfigEtc_Load(object sender, System.EventArgs e)
		{
			checkBoxFitToWindow.Checked = ConfigViewMain.bFitToWindow;
			this.checkBoxDisplayToolTip.Checked = ConfigViewMain.bDisplayToolTip;
			this.checkBoxResponseOnMouseLeft.Checked = ConfigViewMain.bResponseMouseLeftOnGraphic;
			this.checkBoxResponseOnMouseRight.Checked = ConfigViewMain.bResponseMouseRightOnGraphic;
			this.checkBoxUseMenuButton.Checked = ConfigViewMain.bUseMenuButtonOnGraphic;
			this.numericUpDownMdiCount.Value = ConfigViewMain.nMdiCountOnGraphic;
            this.numericUpDownMaxBasicScreen.Value = ConfigViewMain.nMdiCountOnBasicScreen;
            this.numericUpDownMaxReport.Value = ConfigViewMain.nMdiCountOnReport;

            this.numericUpDownTimeoutOfExcelReportRunDirect.Value = ConfigViewMain.nTimeoutOfExcelReportRunDirect;

            this.numericUpDownForLoopTimeout.Value = ConfigViewMain.nForLoopTimeout;
            this.checkBoxAllowManualOutputOnClient.Checked = ConfigViewMain.bAllowManualOutputOnClient;
            this.checkBoxAlwaysOpenNewReport.Checked = ConfigViewMain.bAlwaysOpenNewReport; //리포트 모듈을 새로 열기 24-07-01 hsjeong

			this.tabControl1.SelectedIndex = nTempTabPos;

            this.checkBoxUseAlarmServer.Checked = ConfigViewMain.bUseAlarmServer;

            this.checkBoxEnablePreviewValue.Checked = ConfigViewMain.bEnableSettingValuePreview;
            this.checkBoxShowUserManualControl.Checked = ConfigViewMain.bShowUserManualControl;

            this.checkBoxSortingEventWindow.Checked = ConfigAlarm.bAlarmConfirmSorting;  //경보이벤트창 정렬 20241111 PSU

            string[] types = Enum.GetNames(typeof(EnumLanguage));

            // 웹클라이언트용 언어 목록 설정 20251119 PSU
            for (int i = 0; i < types.Length; i++)
            {
                this.listBoxWebClient.Items.Add(types[i]);
            }

            EnumLanguage elang = LanguageTool.GetLanguage();
            elang = LanguageTool.GetLanguageWebClient();
            this.listBoxWebClient.SelectedItem = elang.ToString();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
	}
}
