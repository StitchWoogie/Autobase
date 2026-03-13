using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;
using NetTools;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormDialogInsertTable.
	/// </summary>
	public class FormDialogInsertTable : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonWhere0;
		private System.Windows.Forms.RadioButton radioButtonWhere1;
		private System.Windows.Forms.RadioButton radioButtonWhere2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.NumericUpDown numericUpDownCountX;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.NumericUpDown numericUpDownCountY;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDownHeight;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown numericUpDownWidth;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public int m_where = 0;

		public FormDialogInsertTable()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogInsertTable));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonWhere2 = new System.Windows.Forms.RadioButton();
            this.radioButtonWhere1 = new System.Windows.Forms.RadioButton();
            this.radioButtonWhere0 = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownCountY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownCountX = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountX)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonWhere2);
            this.groupBox1.Controls.Add(this.radioButtonWhere1);
            this.groupBox1.Controls.Add(this.radioButtonWhere0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonWhere2
            // 
            this.radioButtonWhere2.AccessibleDescription = null;
            this.radioButtonWhere2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonWhere2, "radioButtonWhere2");
            this.radioButtonWhere2.BackgroundImage = null;
            this.radioButtonWhere2.Font = null;
            this.radioButtonWhere2.Name = "radioButtonWhere2";
            // 
            // radioButtonWhere1
            // 
            this.radioButtonWhere1.AccessibleDescription = null;
            this.radioButtonWhere1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonWhere1, "radioButtonWhere1");
            this.radioButtonWhere1.BackgroundImage = null;
            this.radioButtonWhere1.Font = null;
            this.radioButtonWhere1.Name = "radioButtonWhere1";
            // 
            // radioButtonWhere0
            // 
            this.radioButtonWhere0.AccessibleDescription = null;
            this.radioButtonWhere0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonWhere0, "radioButtonWhere0");
            this.radioButtonWhere0.BackgroundImage = null;
            this.radioButtonWhere0.Font = null;
            this.radioButtonWhere0.Name = "radioButtonWhere0";
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
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numericUpDownCountY);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numericUpDownCountX);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // numericUpDownCountY
            // 
            this.numericUpDownCountY.AccessibleDescription = null;
            this.numericUpDownCountY.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownCountY, "numericUpDownCountY");
            this.numericUpDownCountY.Font = null;
            this.numericUpDownCountY.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownCountY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCountY.Name = "numericUpDownCountY";
            this.numericUpDownCountY.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownCountX
            // 
            this.numericUpDownCountX.AccessibleDescription = null;
            this.numericUpDownCountX.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownCountX, "numericUpDownCountX");
            this.numericUpDownCountX.Font = null;
            this.numericUpDownCountX.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownCountX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCountX.Name = "numericUpDownCountX";
            this.numericUpDownCountX.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.numericUpDownHeight);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.numericUpDownWidth);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.AccessibleDescription = null;
            this.numericUpDownHeight.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownHeight, "numericUpDownHeight");
            this.numericUpDownHeight.Font = null;
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownHeight.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Value = new decimal(new int[] {
            25,
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
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.AccessibleDescription = null;
            this.numericUpDownWidth.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownWidth, "numericUpDownWidth");
            this.numericUpDownWidth.Font = null;
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownWidth.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // FormDialogInsertTable
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDialogInsertTable";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDialogInsertTable_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountX)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		static int nPos = 0;

		private void FormDialogInsertTable_Load(object sender, System.EventArgs e)
		{
			this.numericUpDownWidth.Value  = ReportConfig.InitCellWidth;
			this.numericUpDownHeight.Value = ReportConfig.InitCellHeight;

			m_where = nPos;
			this.radioButtonWhere0.Checked = (m_where == 0);
			this.radioButtonWhere1.Checked = (m_where == 1);
			this.radioButtonWhere2.Checked = (m_where == 2);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ReportConfig.InitCellWidth  = ConvertTool.ToInt32(this.numericUpDownWidth.Value);
			ReportConfig.InitCellHeight = ConvertTool.ToInt32(this.numericUpDownHeight.Value);

			if(this.radioButtonWhere0.Checked)		m_where = 0;
			else if(this.radioButtonWhere1.Checked)	m_where = 1;
			else if(this.radioButtonWhere2.Checked)	m_where = 2;
			else									m_where = 0;

			nPos = m_where;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
