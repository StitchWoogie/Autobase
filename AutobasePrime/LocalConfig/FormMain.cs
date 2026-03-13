using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;

namespace LocalConfig
{
	/// <summary>
	/// Summary description for FormMain.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.CheckBox checkBoxKeyLockAllCheck;
		private System.Windows.Forms.NumericUpDown numericUpDownKeyLockWait;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TextBox textBoxOemProgramName;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textBoxOemCompanyName;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox textBoxDdeServiceName;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxAlarmMsgRET;
		private System.Windows.Forms.TextBox textBoxAlarmMsgCNF;
		private System.Windows.Forms.TextBox textBoxAlarmMsgNEW;
        private TabPage tabPage5;
        private GroupBox groupBox6;
        private NumericUpDown numericUpDownPlcScanMaxPorts;
        private GroupBox groupBox7;
        private NumericUpDown numericUpDownPlcScanMaxScanWriteItems;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormMain()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownKeyLockWait = new System.Windows.Forms.NumericUpDown();
            this.checkBoxKeyLockAllCheck = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxOemProgramName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxOemCompanyName = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxDdeServiceName = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxAlarmMsgRET = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAlarmMsgCNF = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAlarmMsgNEW = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.numericUpDownPlcScanMaxScanWriteItems = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericUpDownPlcScanMaxPorts = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKeyLockWait)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlcScanMaxScanWriteItems)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlcScanMaxPorts)).BeginInit();
            this.SuspendLayout();
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
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.checkBoxKeyLockAllCheck);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownKeyLockWait);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownKeyLockWait
            // 
            this.numericUpDownKeyLockWait.AccessibleDescription = null;
            this.numericUpDownKeyLockWait.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownKeyLockWait, "numericUpDownKeyLockWait");
            this.numericUpDownKeyLockWait.Font = null;
            this.numericUpDownKeyLockWait.Name = "numericUpDownKeyLockWait";
            // 
            // checkBoxKeyLockAllCheck
            // 
            this.checkBoxKeyLockAllCheck.AccessibleDescription = null;
            this.checkBoxKeyLockAllCheck.AccessibleName = null;
            resources.ApplyResources(this.checkBoxKeyLockAllCheck, "checkBoxKeyLockAllCheck");
            this.checkBoxKeyLockAllCheck.BackgroundImage = null;
            this.checkBoxKeyLockAllCheck.Font = null;
            this.checkBoxKeyLockAllCheck.Name = "checkBoxKeyLockAllCheck";
            // 
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.textBoxOemProgramName);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxOemProgramName
            // 
            this.textBoxOemProgramName.AccessibleDescription = null;
            this.textBoxOemProgramName.AccessibleName = null;
            resources.ApplyResources(this.textBoxOemProgramName, "textBoxOemProgramName");
            this.textBoxOemProgramName.BackgroundImage = null;
            this.textBoxOemProgramName.Font = null;
            this.textBoxOemProgramName.Name = "textBoxOemProgramName";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.textBoxOemCompanyName);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // textBoxOemCompanyName
            // 
            this.textBoxOemCompanyName.AccessibleDescription = null;
            this.textBoxOemCompanyName.AccessibleName = null;
            resources.ApplyResources(this.textBoxOemCompanyName, "textBoxOemCompanyName");
            this.textBoxOemCompanyName.BackgroundImage = null;
            this.textBoxOemCompanyName.Font = null;
            this.textBoxOemCompanyName.Name = "textBoxOemCompanyName";
            // 
            // tabPage3
            // 
            this.tabPage3.AccessibleDescription = null;
            this.tabPage3.AccessibleName = null;
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.BackgroundImage = null;
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Font = null;
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.textBoxDdeServiceName);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // textBoxDdeServiceName
            // 
            this.textBoxDdeServiceName.AccessibleDescription = null;
            this.textBoxDdeServiceName.AccessibleName = null;
            resources.ApplyResources(this.textBoxDdeServiceName, "textBoxDdeServiceName");
            this.textBoxDdeServiceName.BackgroundImage = null;
            this.textBoxDdeServiceName.Font = null;
            this.textBoxDdeServiceName.Name = "textBoxDdeServiceName";
            // 
            // tabPage4
            // 
            this.tabPage4.AccessibleDescription = null;
            this.tabPage4.AccessibleName = null;
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.BackgroundImage = null;
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Font = null;
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = null;
            this.groupBox5.AccessibleName = null;
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.BackgroundImage = null;
            this.groupBox5.Controls.Add(this.textBoxAlarmMsgRET);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.textBoxAlarmMsgCNF);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.textBoxAlarmMsgNEW);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Font = null;
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // textBoxAlarmMsgRET
            // 
            this.textBoxAlarmMsgRET.AccessibleDescription = null;
            this.textBoxAlarmMsgRET.AccessibleName = null;
            resources.ApplyResources(this.textBoxAlarmMsgRET, "textBoxAlarmMsgRET");
            this.textBoxAlarmMsgRET.BackgroundImage = null;
            this.textBoxAlarmMsgRET.Font = null;
            this.textBoxAlarmMsgRET.Name = "textBoxAlarmMsgRET";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // textBoxAlarmMsgCNF
            // 
            this.textBoxAlarmMsgCNF.AccessibleDescription = null;
            this.textBoxAlarmMsgCNF.AccessibleName = null;
            resources.ApplyResources(this.textBoxAlarmMsgCNF, "textBoxAlarmMsgCNF");
            this.textBoxAlarmMsgCNF.BackgroundImage = null;
            this.textBoxAlarmMsgCNF.Font = null;
            this.textBoxAlarmMsgCNF.Name = "textBoxAlarmMsgCNF";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBoxAlarmMsgNEW
            // 
            this.textBoxAlarmMsgNEW.AccessibleDescription = null;
            this.textBoxAlarmMsgNEW.AccessibleName = null;
            resources.ApplyResources(this.textBoxAlarmMsgNEW, "textBoxAlarmMsgNEW");
            this.textBoxAlarmMsgNEW.BackgroundImage = null;
            this.textBoxAlarmMsgNEW.Font = null;
            this.textBoxAlarmMsgNEW.Name = "textBoxAlarmMsgNEW";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // tabPage5
            // 
            this.tabPage5.AccessibleDescription = null;
            this.tabPage5.AccessibleName = null;
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.BackgroundImage = null;
            this.tabPage5.Controls.Add(this.groupBox7);
            this.tabPage5.Controls.Add(this.groupBox6);
            this.tabPage5.Font = null;
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.AccessibleDescription = null;
            this.groupBox7.AccessibleName = null;
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.BackgroundImage = null;
            this.groupBox7.Controls.Add(this.numericUpDownPlcScanMaxScanWriteItems);
            this.groupBox7.Font = null;
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // numericUpDownPlcScanMaxScanWriteItems
            // 
            this.numericUpDownPlcScanMaxScanWriteItems.AccessibleDescription = null;
            this.numericUpDownPlcScanMaxScanWriteItems.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownPlcScanMaxScanWriteItems, "numericUpDownPlcScanMaxScanWriteItems");
            this.numericUpDownPlcScanMaxScanWriteItems.Font = null;
            this.numericUpDownPlcScanMaxScanWriteItems.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numericUpDownPlcScanMaxScanWriteItems.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownPlcScanMaxScanWriteItems.Name = "numericUpDownPlcScanMaxScanWriteItems";
            this.numericUpDownPlcScanMaxScanWriteItems.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.numericUpDownPlcScanMaxPorts);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // numericUpDownPlcScanMaxPorts
            // 
            this.numericUpDownPlcScanMaxPorts.AccessibleDescription = null;
            this.numericUpDownPlcScanMaxPorts.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownPlcScanMaxPorts, "numericUpDownPlcScanMaxPorts");
            this.numericUpDownPlcScanMaxPorts.Font = null;
            this.numericUpDownPlcScanMaxPorts.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownPlcScanMaxPorts.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPlcScanMaxPorts.Name = "numericUpDownPlcScanMaxPorts";
            this.numericUpDownPlcScanMaxPorts.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
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
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormMain
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownKeyLockWait)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlcScanMaxScanWriteItems)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPlcScanMaxPorts)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormMain_Load(object sender, System.EventArgs e)
		{
			this.checkBoxKeyLockAllCheck.Checked = TotalConfig.LoadRegAutoBaseConfig("KeyLock", null, "AllCheck", false);
			this.numericUpDownKeyLockWait.Value = TotalConfig.LoadRegAutoBaseConfig("KeyLock", null, "Wait", 0);

			this.textBoxOemProgramName.Text = TotalConfig.AutoBaseIniGetOemProgramName();
			this.textBoxOemCompanyName.Text = TotalConfig.AutoBaseIniGetOemCompanyName();

			this.textBoxDdeServiceName.Text = TotalConfig.LoadRegAutoBaseConfig("DDE", null, "Service", "AUTOBASE");

			this.textBoxAlarmMsgNEW.Text = TotalConfig.LoadRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgNEW", "NEW");
			this.textBoxAlarmMsgCNF.Text = TotalConfig.LoadRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgCNF", "CNF");
			this.textBoxAlarmMsgRET.Text = TotalConfig.LoadRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgRET", "RET");

            this.numericUpDownPlcScanMaxPorts.Value = TotalConfigProject.LoadConfig("PlcScan", "Config", "MaxPorts", 256);
            this.numericUpDownPlcScanMaxScanWriteItems.Value = TotalConfigProject.LoadConfig("PlcScan", "Config", "MaxScanWriteItem", 1000);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			TotalConfig.SaveRegAutoBaseConfig("KeyLock", null, "AllCheck", this.checkBoxKeyLockAllCheck.Checked);
			TotalConfig.SaveRegAutoBaseConfig("KeyLock", null, "Wait", this.numericUpDownKeyLockWait.Value);

            Profile.WritePrivateProfileStringW("OEM", "Program name", this.textBoxOemProgramName.Text, TotalConfig.GetProgramConfigFilename());
            Profile.WritePrivateProfileStringW("OEM", "Company name", this.textBoxOemCompanyName.Text, TotalConfig.GetProgramConfigFilename());
			//TotalConfig.SaveRegAutoBaseConfig("OEM", null, "program name", this.textBoxOemProgramName.Text);
			//TotalConfig.SaveRegAutoBaseConfig("OEM", null, "Company name", this.textBoxOemCompanyName.Text);

			TotalConfig.SaveRegAutoBaseConfig("DDE", null, "Service", this.textBoxDdeServiceName.Text);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgNEW", this.textBoxAlarmMsgNEW.Text);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgCNF", this.textBoxAlarmMsgCNF.Text);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "AlarmMsgDefine", "sAlarmMsgRET", this.textBoxAlarmMsgRET.Text);

            TotalConfigProject.SaveConfig("PlcScan", "Config", "MaxPorts", ConvertTool.ToInt32(numericUpDownPlcScanMaxPorts.Value));
            TotalConfigProject.SaveConfig("PlcScan", "Config", "MaxScanWriteItem", ConvertTool.ToInt32(numericUpDownPlcScanMaxScanWriteItems.Value));

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
