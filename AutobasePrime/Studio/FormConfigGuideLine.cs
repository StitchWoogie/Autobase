using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for FormConfigGuideLine.
	/// </summary>
	public class FormConfigGuideLine : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonColor;
		private System.Windows.Forms.NumericUpDown numericUpDownGabY;
		private System.Windows.Forms.NumericUpDown numericUpDownGabX;
		private System.Windows.Forms.NumericUpDown numericUpDownDisplayY;
		private System.Windows.Forms.NumericUpDown numericUpDownDisplayX;
		private System.Windows.Forms.CheckBox checkBoxMatchUnit;
		private System.Windows.Forms.CheckBox checkBoxMatchDisplay;
		private System.Windows.Forms.CheckBox checkBoxFitToGuideLine;
		private System.Windows.Forms.RadioButton radioButtonType3;
		private System.Windows.Forms.RadioButton radioButtonType2;
		private System.Windows.Forms.RadioButton radioButtonType1;
		private System.Windows.Forms.RadioButton radioButtonType0;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigGuideLine()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigGuideLine));
            this.checkBoxFitToGuideLine = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonColor = new System.Windows.Forms.Button();
            this.radioButtonType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonType0 = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownGabY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownGabX = new System.Windows.Forms.NumericUpDown();
            this.checkBoxMatchUnit = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownDisplayY = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownDisplayX = new System.Windows.Forms.NumericUpDown();
            this.checkBoxMatchDisplay = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGabY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGabX)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDisplayY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDisplayX)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxFitToGuideLine
            // 
            resources.ApplyResources(this.checkBoxFitToGuideLine, "checkBoxFitToGuideLine");
            this.checkBoxFitToGuideLine.Name = "checkBoxFitToGuideLine";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonColor);
            this.groupBox1.Controls.Add(this.radioButtonType3);
            this.groupBox1.Controls.Add(this.radioButtonType2);
            this.groupBox1.Controls.Add(this.radioButtonType1);
            this.groupBox1.Controls.Add(this.radioButtonType0);
            this.groupBox1.Controls.Add(this.label5);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonColor
            // 
            resources.ApplyResources(this.buttonColor, "buttonColor");
            this.buttonColor.Name = "buttonColor";
            this.buttonColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // radioButtonType3
            // 
            resources.ApplyResources(this.radioButtonType3, "radioButtonType3");
            this.radioButtonType3.Name = "radioButtonType3";
            // 
            // radioButtonType2
            // 
            resources.ApplyResources(this.radioButtonType2, "radioButtonType2");
            this.radioButtonType2.Name = "radioButtonType2";
            // 
            // radioButtonType1
            // 
            resources.ApplyResources(this.radioButtonType1, "radioButtonType1");
            this.radioButtonType1.Name = "radioButtonType1";
            // 
            // radioButtonType0
            // 
            resources.ApplyResources(this.radioButtonType0, "radioButtonType0");
            this.radioButtonType0.Name = "radioButtonType0";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.numericUpDownGabY);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numericUpDownGabX);
            this.groupBox2.Controls.Add(this.checkBoxMatchUnit);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownGabY
            // 
            resources.ApplyResources(this.numericUpDownGabY, "numericUpDownGabY");
            this.numericUpDownGabY.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownGabY.Name = "numericUpDownGabY";
            this.numericUpDownGabY.Value = new decimal(new int[] {
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
            // numericUpDownGabX
            // 
            resources.ApplyResources(this.numericUpDownGabX, "numericUpDownGabX");
            this.numericUpDownGabX.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownGabX.Name = "numericUpDownGabX";
            this.numericUpDownGabX.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownGabX.TextChanged += new System.EventHandler(this.numericUpDownGabX_TextChanged);
            // 
            // checkBoxMatchUnit
            // 
            resources.ApplyResources(this.checkBoxMatchUnit, "checkBoxMatchUnit");
            this.checkBoxMatchUnit.Name = "checkBoxMatchUnit";
            this.checkBoxMatchUnit.CheckedChanged += new System.EventHandler(this.checkBoxMatchUnit_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownDisplayY);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.numericUpDownDisplayX);
            this.groupBox3.Controls.Add(this.checkBoxMatchDisplay);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownDisplayY
            // 
            resources.ApplyResources(this.numericUpDownDisplayY, "numericUpDownDisplayY");
            this.numericUpDownDisplayY.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDisplayY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDisplayY.Name = "numericUpDownDisplayY";
            this.numericUpDownDisplayY.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownDisplayX
            // 
            resources.ApplyResources(this.numericUpDownDisplayX, "numericUpDownDisplayX");
            this.numericUpDownDisplayX.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDisplayX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDisplayX.Name = "numericUpDownDisplayX";
            this.numericUpDownDisplayX.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownDisplayX.TextChanged += new System.EventHandler(this.numericUpDownDisplayX_TextChanged);
            // 
            // checkBoxMatchDisplay
            // 
            resources.ApplyResources(this.checkBoxMatchDisplay, "checkBoxMatchDisplay");
            this.checkBoxMatchDisplay.Name = "checkBoxMatchDisplay";
            this.checkBoxMatchDisplay.CheckedChanged += new System.EventHandler(this.checkBoxMatchDisplay_CheckedChanged);
            // 
            // FormConfigGuideLine
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkBoxFitToGuideLine);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigGuideLine";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigGuideLine_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGabY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGabX)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDisplayY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDisplayX)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void groupBox3_Enter(object sender, System.EventArgs e)
		{
		
		}

		private void FormConfigGuideLine_Load(object sender, System.EventArgs e)
		{
            this.numericUpDownDisplayX.Value = AutoLib.ConfigStudio.nGuideLineDisplayX;
            this.numericUpDownDisplayY.Value = AutoLib.ConfigStudio.nGuideLineDisplayY;
            this.numericUpDownGabX.Value = AutoLib.ConfigStudio.nGuideLineUnitX;
            this.numericUpDownGabY.Value = AutoLib.ConfigStudio.nGuideLineUnitY;
            this.radioButtonType0.Checked = (AutoLib.ConfigStudio.nGuideLineType == 0);
            this.radioButtonType1.Checked = (AutoLib.ConfigStudio.nGuideLineType == 1);
            this.radioButtonType2.Checked = (AutoLib.ConfigStudio.nGuideLineType == 2);
            this.radioButtonType3.Checked = (AutoLib.ConfigStudio.nGuideLineType == 3);
            this.buttonColor.BackColor = AutoLib.ConfigStudio.lGuideLineColor;
            this.checkBoxFitToGuideLine.Checked = AutoLib.ConfigStudio.bGuideLineFit;
            this.checkBoxMatchDisplay.Checked = AutoLib.ConfigStudio.bGuideLineMatchDisplay;
            this.checkBoxMatchUnit.Checked = AutoLib.ConfigStudio.bGuideLineMatchUnit;

			EnableDisableUnit();
			EnableDisableDisplay();
		}

		void EnableDisableUnit()
		{	
			this.numericUpDownGabY.Enabled = !checkBoxMatchUnit.Checked;
		}

		void EnableDisableDisplay()
		{
			this.numericUpDownDisplayY.Enabled = !checkBoxMatchDisplay.Checked;
		}

		private void checkBoxMatchUnit_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableUnit();
		}

		private void checkBoxMatchDisplay_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableDisplay();
		}

		private void numericUpDownGabX_TextChanged(object sender, System.EventArgs e)
		{
			if(this.checkBoxMatchUnit.Checked)	this.numericUpDownGabY.Value = this.numericUpDownGabX.Value;
		}

		private void numericUpDownDisplayX_TextChanged(object sender, System.EventArgs e)
		{
			if(this.checkBoxMatchDisplay.Checked)	this.numericUpDownDisplayY.Value = this.numericUpDownDisplayX.Value;		
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            AutoLib.ConfigStudio.nGuideLineDisplayX = ConvertTool.ToInt32(this.numericUpDownDisplayX.Value);
            AutoLib.ConfigStudio.nGuideLineDisplayY = ConvertTool.ToInt32(this.numericUpDownDisplayY.Value);
            AutoLib.ConfigStudio.nGuideLineUnitX = ConvertTool.ToInt32(this.numericUpDownGabX.Value);
            AutoLib.ConfigStudio.nGuideLineUnitY = ConvertTool.ToInt32(this.numericUpDownGabY.Value);
            if (this.radioButtonType0.Checked) AutoLib.ConfigStudio.nGuideLineType = 0;
            else if (this.radioButtonType1.Checked) AutoLib.ConfigStudio.nGuideLineType = 1;
            else if (this.radioButtonType2.Checked) AutoLib.ConfigStudio.nGuideLineType = 2;
            else if (this.radioButtonType3.Checked) AutoLib.ConfigStudio.nGuideLineType = 3;
			else {}
            AutoLib.ConfigStudio.lGuideLineColor = this.buttonColor.BackColor;
            AutoLib.ConfigStudio.bGuideLineFit = this.checkBoxFitToGuideLine.Checked;
            AutoLib.ConfigStudio.bGuideLineMatchDisplay = this.checkBoxMatchDisplay.Checked;
            AutoLib.ConfigStudio.bGuideLineMatchUnit = this.checkBoxMatchUnit.Checked;

			DialogResult = DialogResult.OK;
			Close();

            AutoLib.ConfigStudio.Save();
		}

        //private void buttonColor_Click(object sender, System.EventArgs e)
        //{
        //    ColorDialog dialog = new ColorDialog();

        //    dialog.Color = buttonColor.BackColor;

        //    if(dialog.ShowDialog(this) == DialogResult.OK) 
        //    {
        //        buttonColor.BackColor = dialog.Color;
        //    }

        //}

        //20241024 PSU 색상창 변경
        private void buttonColor_Click(object sender, System.EventArgs e)
		{
			FormColorDialog dialog = new FormColorDialog();

			dialog.SetSelectedColor(buttonColor.BackColor, true);

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				buttonColor.BackColor = dialog.GetSelectedColor();
			}
		}
	}
}
