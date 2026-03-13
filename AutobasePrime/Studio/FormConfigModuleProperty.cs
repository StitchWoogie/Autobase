using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for FormConfigModuleProperty.
	/// </summary>
	public class FormConfigModuleProperty : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioButtonWindowPropertyMdi;
		private System.Windows.Forms.RadioButton radioButtonWindowPropertyPopup;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.NumericUpDown numericUpDownModuleSizeWidth;
		private System.Windows.Forms.NumericUpDown numericUpDownModuleSizeHeight;
		private System.Windows.Forms.RadioButton radioButtonLocation0;
		private System.Windows.Forms.RadioButton radioButtonLocation1;
		private System.Windows.Forms.RadioButton radioButtonLocation2;
		private System.Windows.Forms.RadioButton radioButtonLocation3;
		private System.Windows.Forms.RadioButton radioButtonLocation4;
		private System.Windows.Forms.NumericUpDown numericUpDownLocationX;
		private System.Windows.Forms.NumericUpDown numericUpDownLocationY;
		private System.Windows.Forms.RadioButton radioButtonModuleView0;
		private System.Windows.Forms.RadioButton radioButtonModuleView1;
		private System.Windows.Forms.RadioButton radioButtonModuleView2;
		private System.Windows.Forms.CheckBox checkBoxStyleCaption;
		private System.Windows.Forms.CheckBox checkBoxStyleSysmenu;
		private System.Windows.Forms.CheckBox checkBoxStyleBorder;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox checkBoxPopupDialog;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownOpacity;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBoxWindowStyle;
		private System.Windows.Forms.GroupBox groupBoxOpacity;
		private System.Windows.Forms.CheckBox checkBoxNotUsedMdiLimit;
        private CheckBox checkBoxUseSmoothing;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox5;
        private TextBox textBoxPassword;
        private Label label8;
        private TextBox textBoxPasswordConfirm;
        private Label label7;
        private CheckBox checkBoxPopupAlwaysOnTop;
        private Label label9;
        private NumericUpDown numericUpDownMonitorIndex;
        private Button buttonCheckMonitor;

		FormEditGraphic form;

		public FormConfigModuleProperty(FormEditGraphic f)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			form = f;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigModuleProperty));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonWindowPropertyPopup = new System.Windows.Forms.RadioButton();
            this.radioButtonWindowPropertyMdi = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownModuleSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownModuleSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownLocationY = new System.Windows.Forms.NumericUpDown();
            this.radioButtonLocation4 = new System.Windows.Forms.RadioButton();
            this.radioButtonLocation3 = new System.Windows.Forms.RadioButton();
            this.radioButtonLocation2 = new System.Windows.Forms.RadioButton();
            this.radioButtonLocation1 = new System.Windows.Forms.RadioButton();
            this.radioButtonLocation0 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownLocationX = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonModuleView2 = new System.Windows.Forms.RadioButton();
            this.radioButtonModuleView1 = new System.Windows.Forms.RadioButton();
            this.radioButtonModuleView0 = new System.Windows.Forms.RadioButton();
            this.groupBoxWindowStyle = new System.Windows.Forms.GroupBox();
            this.checkBoxPopupAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.checkBoxPopupDialog = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleBorder = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleSysmenu = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleCaption = new System.Windows.Forms.CheckBox();
            this.groupBoxOpacity = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownOpacity = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxNotUsedMdiLimit = new System.Windows.Forms.CheckBox();
            this.checkBoxUseSmoothing = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxPasswordConfirm = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.numericUpDownMonitorIndex = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonCheckMonitor = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleSizeHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleSizeWidth)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLocationY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLocationX)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBoxWindowStyle.SuspendLayout();
            this.groupBoxOpacity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOpacity)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMonitorIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonWindowPropertyPopup);
            this.groupBox1.Controls.Add(this.radioButtonWindowPropertyMdi);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonWindowPropertyPopup
            // 
            resources.ApplyResources(this.radioButtonWindowPropertyPopup, "radioButtonWindowPropertyPopup");
            this.radioButtonWindowPropertyPopup.Name = "radioButtonWindowPropertyPopup";
            this.radioButtonWindowPropertyPopup.CheckedChanged += new System.EventHandler(this.radioButtonWindowPropertyPopup_CheckedChanged);
            // 
            // radioButtonWindowPropertyMdi
            // 
            resources.ApplyResources(this.radioButtonWindowPropertyMdi, "radioButtonWindowPropertyMdi");
            this.radioButtonWindowPropertyMdi.Name = "radioButtonWindowPropertyMdi";
            this.radioButtonWindowPropertyMdi.CheckedChanged += new System.EventHandler(this.radioButtonWindowPropertyMdi_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDownModuleSizeHeight);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.numericUpDownModuleSizeWidth);
            this.groupBox2.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // numericUpDownModuleSizeHeight
            // 
            resources.ApplyResources(this.numericUpDownModuleSizeHeight, "numericUpDownModuleSizeHeight");
            this.numericUpDownModuleSizeHeight.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownModuleSizeHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownModuleSizeHeight.Name = "numericUpDownModuleSizeHeight";
            this.numericUpDownModuleSizeHeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownModuleSizeHeight.ValueChanged += new System.EventHandler(this.numericUpDownModuleSizeHeight_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownModuleSizeWidth
            // 
            resources.ApplyResources(this.numericUpDownModuleSizeWidth, "numericUpDownModuleSizeWidth");
            this.numericUpDownModuleSizeWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownModuleSizeWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownModuleSizeWidth.Name = "numericUpDownModuleSizeWidth";
            this.numericUpDownModuleSizeWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonCheckMonitor);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numericUpDownMonitorIndex);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.numericUpDownLocationY);
            this.groupBox3.Controls.Add(this.radioButtonLocation4);
            this.groupBox3.Controls.Add(this.radioButtonLocation3);
            this.groupBox3.Controls.Add(this.radioButtonLocation2);
            this.groupBox3.Controls.Add(this.radioButtonLocation1);
            this.groupBox3.Controls.Add(this.radioButtonLocation0);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownLocationX);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownLocationY
            // 
            resources.ApplyResources(this.numericUpDownLocationY, "numericUpDownLocationY");
            this.numericUpDownLocationY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownLocationY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDownLocationY.Name = "numericUpDownLocationY";
            // 
            // radioButtonLocation4
            // 
            resources.ApplyResources(this.radioButtonLocation4, "radioButtonLocation4");
            this.radioButtonLocation4.Name = "radioButtonLocation4";
            // 
            // radioButtonLocation3
            // 
            resources.ApplyResources(this.radioButtonLocation3, "radioButtonLocation3");
            this.radioButtonLocation3.Name = "radioButtonLocation3";
            // 
            // radioButtonLocation2
            // 
            resources.ApplyResources(this.radioButtonLocation2, "radioButtonLocation2");
            this.radioButtonLocation2.Name = "radioButtonLocation2";
            this.radioButtonLocation2.CheckedChanged += new System.EventHandler(this.radioButtonLocation2_CheckedChanged);
            // 
            // radioButtonLocation1
            // 
            resources.ApplyResources(this.radioButtonLocation1, "radioButtonLocation1");
            this.radioButtonLocation1.Name = "radioButtonLocation1";
            this.radioButtonLocation1.CheckedChanged += new System.EventHandler(this.radioButtonLocation1_CheckedChanged);
            // 
            // radioButtonLocation0
            // 
            resources.ApplyResources(this.radioButtonLocation0, "radioButtonLocation0");
            this.radioButtonLocation0.Name = "radioButtonLocation0";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownLocationX
            // 
            resources.ApplyResources(this.numericUpDownLocationX, "numericUpDownLocationX");
            this.numericUpDownLocationX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownLocationX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDownLocationX.Name = "numericUpDownLocationX";
            this.numericUpDownLocationX.ValueChanged += new System.EventHandler(this.numericUpDownLocationX_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonModuleView2);
            this.groupBox4.Controls.Add(this.radioButtonModuleView1);
            this.groupBox4.Controls.Add(this.radioButtonModuleView0);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // radioButtonModuleView2
            // 
            resources.ApplyResources(this.radioButtonModuleView2, "radioButtonModuleView2");
            this.radioButtonModuleView2.Name = "radioButtonModuleView2";
            this.radioButtonModuleView2.CheckedChanged += new System.EventHandler(this.radioButtonModuleView2_CheckedChanged);
            // 
            // radioButtonModuleView1
            // 
            resources.ApplyResources(this.radioButtonModuleView1, "radioButtonModuleView1");
            this.radioButtonModuleView1.Name = "radioButtonModuleView1";
            this.radioButtonModuleView1.CheckedChanged += new System.EventHandler(this.radioButtonModuleView1_CheckedChanged);
            // 
            // radioButtonModuleView0
            // 
            resources.ApplyResources(this.radioButtonModuleView0, "radioButtonModuleView0");
            this.radioButtonModuleView0.Name = "radioButtonModuleView0";
            this.radioButtonModuleView0.CheckedChanged += new System.EventHandler(this.radioButtonModuleView0_CheckedChanged);
            // 
            // groupBoxWindowStyle
            // 
            this.groupBoxWindowStyle.Controls.Add(this.checkBoxPopupAlwaysOnTop);
            this.groupBoxWindowStyle.Controls.Add(this.checkBoxPopupDialog);
            this.groupBoxWindowStyle.Controls.Add(this.checkBoxStyleBorder);
            this.groupBoxWindowStyle.Controls.Add(this.checkBoxStyleSysmenu);
            this.groupBoxWindowStyle.Controls.Add(this.checkBoxStyleCaption);
            resources.ApplyResources(this.groupBoxWindowStyle, "groupBoxWindowStyle");
            this.groupBoxWindowStyle.Name = "groupBoxWindowStyle";
            this.groupBoxWindowStyle.TabStop = false;
            // 
            // checkBoxPopupAlwaysOnTop
            // 
            resources.ApplyResources(this.checkBoxPopupAlwaysOnTop, "checkBoxPopupAlwaysOnTop");
            this.checkBoxPopupAlwaysOnTop.Name = "checkBoxPopupAlwaysOnTop";
            // 
            // checkBoxPopupDialog
            // 
            resources.ApplyResources(this.checkBoxPopupDialog, "checkBoxPopupDialog");
            this.checkBoxPopupDialog.Name = "checkBoxPopupDialog";
            // 
            // checkBoxStyleBorder
            // 
            resources.ApplyResources(this.checkBoxStyleBorder, "checkBoxStyleBorder");
            this.checkBoxStyleBorder.Name = "checkBoxStyleBorder";
            // 
            // checkBoxStyleSysmenu
            // 
            resources.ApplyResources(this.checkBoxStyleSysmenu, "checkBoxStyleSysmenu");
            this.checkBoxStyleSysmenu.Name = "checkBoxStyleSysmenu";
            // 
            // checkBoxStyleCaption
            // 
            resources.ApplyResources(this.checkBoxStyleCaption, "checkBoxStyleCaption");
            this.checkBoxStyleCaption.Name = "checkBoxStyleCaption";
            // 
            // groupBoxOpacity
            // 
            this.groupBoxOpacity.Controls.Add(this.label5);
            this.groupBoxOpacity.Controls.Add(this.numericUpDownOpacity);
            this.groupBoxOpacity.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBoxOpacity, "groupBoxOpacity");
            this.groupBoxOpacity.Name = "groupBoxOpacity";
            this.groupBoxOpacity.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownOpacity
            // 
            resources.ApplyResources(this.numericUpDownOpacity, "numericUpDownOpacity");
            this.numericUpDownOpacity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownOpacity.Name = "numericUpDownOpacity";
            this.numericUpDownOpacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // checkBoxNotUsedMdiLimit
            // 
            resources.ApplyResources(this.checkBoxNotUsedMdiLimit, "checkBoxNotUsedMdiLimit");
            this.checkBoxNotUsedMdiLimit.Name = "checkBoxNotUsedMdiLimit";
            // 
            // checkBoxUseSmoothing
            // 
            resources.ApplyResources(this.checkBoxUseSmoothing, "checkBoxUseSmoothing");
            this.checkBoxUseSmoothing.Name = "checkBoxUseSmoothing";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.checkBoxUseSmoothing);
            this.tabPage1.Controls.Add(this.groupBoxOpacity);
            this.tabPage1.Controls.Add(this.checkBoxNotUsedMdiLimit);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBoxWindowStyle);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox5);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.textBoxPasswordConfirm);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.textBoxPassword);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // textBoxPasswordConfirm
            // 
            resources.ApplyResources(this.textBoxPasswordConfirm, "textBoxPasswordConfirm");
            this.textBoxPasswordConfirm.Name = "textBoxPasswordConfirm";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // textBoxPassword
            // 
            resources.ApplyResources(this.textBoxPassword, "textBoxPassword");
            this.textBoxPassword.Name = "textBoxPassword";
            // 
            // numericUpDownMonitorIndex
            // 
            resources.ApplyResources(this.numericUpDownMonitorIndex, "numericUpDownMonitorIndex");
            this.numericUpDownMonitorIndex.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownMonitorIndex.Name = "numericUpDownMonitorIndex";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // buttonCheckMonitor
            // 
            resources.ApplyResources(this.buttonCheckMonitor, "buttonCheckMonitor");
            this.buttonCheckMonitor.Name = "buttonCheckMonitor";
            this.buttonCheckMonitor.UseVisualStyleBackColor = true;
            this.buttonCheckMonitor.Click += new System.EventHandler(this.buttonCheckMonitor_Click);
            // 
            // FormConfigModuleProperty
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
            this.Name = "FormConfigModuleProperty";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigModuleProperty_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleSizeHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleSizeWidth)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLocationY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLocationX)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBoxWindowStyle.ResumeLayout(false);
            this.groupBoxOpacity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOpacity)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMonitorIndex)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigModuleProperty_Load(object sender, System.EventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			int x, y;
			int location;
            int monitorIndex;

			work.obj.GetModuleSize(out x, out y);
			this.numericUpDownModuleSizeWidth.Value = x;
			this.numericUpDownModuleSizeHeight.Value = y;

			work.obj.GetDefaultLocation(out location, out x, out y);

            //monitorIndex 추가 20241010
            work.obj.GetDefaultMonitorIndex(out monitorIndex);
            Tools.SetNumericUpDownValue(this.numericUpDownMonitorIndex, monitorIndex);

			radioButtonLocation0.Checked = (location == 0);
			radioButtonLocation1.Checked = (location == 1);
			radioButtonLocation2.Checked = (location == 2);
			radioButtonLocation3.Checked = (location == 3);
			radioButtonLocation4.Checked = (location == 4);

            Tools.SetNumericUpDownValue(this.numericUpDownLocationX, x);
			Tools.SetNumericUpDownValue(this.numericUpDownLocationY, y);
	
			int val = work.obj.GetObjectOpticMethod();

			this.radioButtonModuleView0.Checked = (val == 0);
			this.radioButtonModuleView1.Checked = (val == 1);
			this.radioButtonModuleView2.Checked = (val == 2);

			val = work.obj.GetModuleWindowStyle();

			this.radioButtonWindowPropertyMdi.Checked = (val == 0);
			this.radioButtonWindowPropertyPopup.Checked = (val == 1);

			EnumWindowStyleFlags flags = (EnumWindowStyleFlags)work.obj.GetModuleWindowStyleFlag();		

			this.checkBoxStyleCaption.Checked = ((flags & EnumWindowStyleFlags.WS_CAPTION) == EnumWindowStyleFlags.WS_CAPTION);
			this.checkBoxStyleSysmenu.Checked = ((flags & EnumWindowStyleFlags.WS_SYSMENU) == EnumWindowStyleFlags.WS_SYSMENU);
			this.checkBoxStyleBorder.Checked = ((flags & EnumWindowStyleFlags.WS_BORDER) == EnumWindowStyleFlags.WS_BORDER);

			this.checkBoxPopupDialog.Checked = work.obj.ModulePopupDialog;
			this.numericUpDownOpacity.Value = work.obj.ModuleOpacity;

			this.checkBoxNotUsedMdiLimit.Checked = (work.obj.bNotUsedMdiLimit == 1);

            this.checkBoxUseSmoothing.Checked = (work.obj.cSmoothingMode == 1);

            this.textBoxPassword.Text = work.obj.sPassword;
            this.textBoxPasswordConfirm.Text = work.obj.sPassword;

            this.checkBoxPopupAlwaysOnTop.Checked = work.obj.bModulePopupAlwaysOnTop;

			EnableItemByRadio();
		}

		private void numericUpDownModuleSizeHeight_ValueChanged(object sender, System.EventArgs e)
		{
		
		}

		private void numericUpDownLocationX_ValueChanged(object sender, System.EventArgs e)
		{
		
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

            if (this.textBoxPassword.Text.Length > 0)
            {
                if (this.textBoxPassword.Text != this.textBoxPasswordConfirm.Text)
                {
                    MessageBox.Show("Password and Confirm Password mismatched.", "Password error");
                    return;
                }
            }

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                if (Tools.IsLangKorean())
                    ClassStudioEditUndo.UndoSave_All(form, "모듈 특성 설정");
                else
                    ClassStudioEditUndo.UndoSave_All(form, "Module Properties");
            }

            work.obj.sPassword = this.textBoxPassword.Text;

			int x;
			int y;

			x = ConvertTool.ToInt32(this.numericUpDownModuleSizeWidth.Value);
			y = ConvertTool.ToInt32(this.numericUpDownModuleSizeHeight.Value);

			work.obj.SetModuleSize(x, y);
			work.obj.groupRoot.UpdateGroupRealSize(x, y);

			int val = 0;

			if(this.radioButtonModuleView0.Checked)			val = 0;
			else if(this.radioButtonModuleView1.Checked)	val = 1;
			else if(this.radioButtonModuleView2.Checked)	val = 2;
			else val = 0;
						
			work.obj.SetObjectOpticMethod(val);

			work.obj.SetModuleWindowStyle(GetRadioPosWindowProperty());

			EnumWindowStyleFlags flags = 0;

			if(this.checkBoxStyleCaption.Checked)	flags |= EnumWindowStyleFlags.WS_CAPTION;
			if(this.checkBoxStyleSysmenu.Checked)	flags |= EnumWindowStyleFlags.WS_SYSMENU;
			if(this.checkBoxStyleBorder.Checked)	flags |= EnumWindowStyleFlags.WS_BORDER;

			work.obj.SetModuleWindowStyleFlag((uint)flags);

			int location = 0;
			if(radioButtonLocation0.Checked)	location = 0;
			else if(radioButtonLocation1.Checked)	location = 1;
			else if(radioButtonLocation2.Checked)	location = 2;
			else if(radioButtonLocation3.Checked)	location = 3;
			else if(radioButtonLocation4.Checked)	location = 4;
			else location = 0;

			x = ConvertTool.ToInt32(this.numericUpDownLocationX.Value);
			y = ConvertTool.ToInt32(this.numericUpDownLocationY.Value);
			
			work.obj.SetDefaultLocation(location, x, y);

            //20241010 monitorIndex 추가
            int monitorIndex;
            monitorIndex = ConvertTool.ToInt32(this.numericUpDownMonitorIndex.Value);
            work.obj.SetDefaultMonitorIndex(monitorIndex);

			work.obj.ModulePopupDialog = this.checkBoxPopupDialog.Checked;
			work.obj.ModuleOpacity = ConvertTool.ToInt32(this.numericUpDownOpacity.Value);
			work.obj.bNotUsedMdiLimit = this.checkBoxNotUsedMdiLimit.Checked ? (sbyte)1 : (sbyte)0;
			//work.obj.SetZoneAtPercent100();

            work.obj.cSmoothingMode = this.checkBoxUseSmoothing.Checked ? (sbyte)1 : (sbyte)0;

            work.obj.bModulePopupAlwaysOnTop = this.checkBoxPopupAlwaysOnTop.Checked;

			form.SetChangeFlag();
			form.ScrollUpdate();

			form.Invalidate();

            DialogResult = DialogResult.OK;

			Close();
		}

		int GetRadioPosWindowProperty()
		{
			int val;

			if(this.radioButtonWindowPropertyMdi.Checked)			val = 0;
			else if(this.radioButtonWindowPropertyPopup.Checked)	val = 1;
			else val = 0;

			return val;
		}

		void EnableItemByRadio()
		{
			int radio = GetRadioPosWindowProperty();

			bool flag_optic;
			bool flag_style;
			bool flag_location;
			bool flag_NotUsedMdiLimit;

			if(radio == 0) 
			{
				flag_optic = true;
				flag_style = false;
				flag_location = false;
				flag_NotUsedMdiLimit = true;
			}
			else // popup window
			{
				flag_optic = false;
				flag_style = true;
				flag_location = true;
				flag_NotUsedMdiLimit = false;
			}

			this.radioButtonModuleView0.Enabled = flag_optic;
			this.radioButtonModuleView1.Enabled = flag_optic;
			this.radioButtonModuleView2.Enabled = flag_optic;

			this.groupBoxWindowStyle.Enabled = flag_style;
			this.groupBoxOpacity.Enabled = flag_style;

			this.radioButtonLocation0.Enabled = flag_location;
			this.radioButtonLocation1.Enabled = flag_location;
			this.radioButtonLocation2.Enabled = flag_location;
			this.radioButtonLocation3.Enabled = flag_location;
			this.radioButtonLocation4.Enabled = flag_location;

			this.numericUpDownLocationX.Enabled = flag_location;
			this.numericUpDownLocationY.Enabled = flag_location;

			this.checkBoxNotUsedMdiLimit.Enabled = flag_NotUsedMdiLimit;

            //20241010 monitorIndex 추가
            this.numericUpDownMonitorIndex.Enabled = flag_location;
		}

		private void radioButtonWindowPropertyMdi_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableItemByRadio();
		}

		private void radioButtonWindowPropertyPopup_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableItemByRadio();
		}

		private void radioButtonModuleView0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableItemByRadio();
		}

		private void radioButtonModuleView1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableItemByRadio();
		}

		private void radioButtonModuleView2_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableItemByRadio();
		}

		private void radioButtonLocation2_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		private void radioButtonLocation1_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

        private void buttonCheckMonitor_Click(object sender, EventArgs e)
        {
            MonitorIndexDisplay.ShowMonitorIndexes();
        }
	}
}

