using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using AutoLibLocal;
using NetTools;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormDialogConfigEtc.
	/// </summary>
	public class FormDialogConfigEtc : System.Windows.Forms.Form
	{
		private System.Windows.Forms.CheckBox checkBoxNonData;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxNonData;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBoxPrinter;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox3;
        private CheckBox checkBoxMaxSub;
        private NumericUpDown numericUpDownStartHour;
        private Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDialogConfigEtc()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogConfigEtc));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxNonData = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxNonData = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxPrinter = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxMaxSub = new System.Windows.Forms.CheckBox();
            this.numericUpDownStartHour = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartHour)).BeginInit();
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
            // checkBoxNonData
            // 
            this.checkBoxNonData.AccessibleDescription = null;
            this.checkBoxNonData.AccessibleName = null;
            resources.ApplyResources(this.checkBoxNonData, "checkBoxNonData");
            this.checkBoxNonData.BackgroundImage = null;
            this.checkBoxNonData.Font = null;
            this.checkBoxNonData.Name = "checkBoxNonData";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxNonData);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxNonData
            // 
            this.textBoxNonData.AccessibleDescription = null;
            this.textBoxNonData.AccessibleName = null;
            resources.ApplyResources(this.textBoxNonData, "textBoxNonData");
            this.textBoxNonData.BackgroundImage = null;
            this.textBoxNonData.Font = null;
            this.textBoxNonData.Name = "textBoxNonData";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.comboBoxPrinter);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboBoxPrinter
            // 
            this.comboBoxPrinter.AccessibleDescription = null;
            this.comboBoxPrinter.AccessibleName = null;
            resources.ApplyResources(this.comboBoxPrinter, "comboBoxPrinter");
            this.comboBoxPrinter.BackgroundImage = null;
            this.comboBoxPrinter.Font = null;
            this.comboBoxPrinter.Name = "comboBoxPrinter";
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
            this.tabPage1.Controls.Add(this.checkBoxNonData);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Font = null;
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.AccessibleDescription = null;
            this.tabPage2.AccessibleName = null;
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackgroundImage = null;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.numericUpDownStartHour);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Font = null;
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.checkBoxMaxSub);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // checkBoxMaxSub
            // 
            this.checkBoxMaxSub.AccessibleDescription = null;
            this.checkBoxMaxSub.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMaxSub, "checkBoxMaxSub");
            this.checkBoxMaxSub.BackgroundImage = null;
            this.checkBoxMaxSub.Font = null;
            this.checkBoxMaxSub.Name = "checkBoxMaxSub";
            this.checkBoxMaxSub.UseVisualStyleBackColor = true;
            // 
            // numericUpDownStartHour
            // 
            this.numericUpDownStartHour.AccessibleDescription = null;
            this.numericUpDownStartHour.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownStartHour, "numericUpDownStartHour");
            this.numericUpDownStartHour.Font = null;
            this.numericUpDownStartHour.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.numericUpDownStartHour.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            -2147483648});
            this.numericUpDownStartHour.Name = "numericUpDownStartHour";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // FormDialogConfigEtc
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDialogConfigEtc";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDialogConfigEtc_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartHour)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormDialogConfigEtc_Load(object sender, System.EventArgs e)
		{
			this.checkBoxNonData.Checked = ReportBasicLib.ReportConfig.CalcNoData;
			this.textBoxNonData.Text = ReportBasicLib.ReportConfig.NoData;

			foreach(String printer in PrinterSettings.InstalledPrinters) 
			{
				comboBoxPrinter.Items.Add(printer);

				if(ReportBasicLib.ReportConfig.ReportPrintFunctionPrinter == printer) 
				{
					comboBoxPrinter.SelectedIndex = comboBoxPrinter.Items.Count-1;	
				}
			}

			if(comboBoxPrinter.Items.Count > 0) 
			{
				if(comboBoxPrinter.SelectedIndex == -1)
					comboBoxPrinter.SelectedIndex = 0;
			}

            this.numericUpDownStartHour.Value = ConfigViewMain.nReportStartHourOfDay;
            this.checkBoxMaxSub.Checked = ConfigViewMain.bReportStartHourOfDayMaxSub;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ReportBasicLib.ReportConfig.CalcNoData = this.checkBoxNonData.Checked;
			ReportBasicLib.ReportConfig.NoData = this.textBoxNonData.Text;
			ReportBasicLib.ReportConfig.bCalcNoData = this.checkBoxNonData.Checked;
			ReportBasicLib.ReportConfig.sNoData = this.textBoxNonData.Text;
			ReportBasicLib.ReportConfig.ReportPrintFunctionPrinter = comboBoxPrinter.Text;

            ConfigViewMain.nReportStartHourOfDay = ConvertTool.ToInt32(this.numericUpDownStartHour.Value);
            ConfigViewMain.bReportStartHourOfDayMaxSub = this.checkBoxMaxSub.Checked;

            ConfigViewMain.Save();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
