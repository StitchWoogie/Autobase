namespace Studio
{
    partial class FormDialogAutoPaste
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogAutoPaste));
            this.labelTotalHeight = new System.Windows.Forms.Label();
            this.labelTotalWidth = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelModuleHeight = new System.Windows.Forms.Label();
            this.labelModuleWidth = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownColumnSpace = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRowSpace = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownColumn = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownRow = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumnSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRowSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumn)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRow)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTotalHeight
            // 
            resources.ApplyResources(this.labelTotalHeight, "labelTotalHeight");
            this.labelTotalHeight.Name = "labelTotalHeight";
            // 
            // labelTotalWidth
            // 
            resources.ApplyResources(this.labelTotalWidth, "labelTotalWidth");
            this.labelTotalWidth.Name = "labelTotalWidth";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // labelModuleHeight
            // 
            resources.ApplyResources(this.labelModuleHeight, "labelModuleHeight");
            this.labelModuleHeight.Name = "labelModuleHeight";
            // 
            // labelModuleWidth
            // 
            resources.ApplyResources(this.labelModuleWidth, "labelModuleWidth");
            this.labelModuleWidth.Name = "labelModuleWidth";
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
            // numericUpDownColumnSpace
            // 
            resources.ApplyResources(this.numericUpDownColumnSpace, "numericUpDownColumnSpace");
            this.numericUpDownColumnSpace.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownColumnSpace.Name = "numericUpDownColumnSpace";
            this.numericUpDownColumnSpace.ValueChanged += new System.EventHandler(this.numericUpDownColumnSpace_ValueChanged);
            this.numericUpDownColumnSpace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownColumnSpace_KeyDown);
            // 
            // numericUpDownRowSpace
            // 
            resources.ApplyResources(this.numericUpDownRowSpace, "numericUpDownRowSpace");
            this.numericUpDownRowSpace.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownRowSpace.Name = "numericUpDownRowSpace";
            this.numericUpDownRowSpace.ValueChanged += new System.EventHandler(this.numericUpDownRowSpace_ValueChanged);
            this.numericUpDownRowSpace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownRowSpace_KeyDown);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click_1);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // numericUpDownColumn
            // 
            resources.ApplyResources(this.numericUpDownColumn, "numericUpDownColumn");
            this.numericUpDownColumn.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownColumn.Name = "numericUpDownColumn";
            this.numericUpDownColumn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownColumn.ValueChanged += new System.EventHandler(this.numericUpDownColumn_ValueChanged);
            this.numericUpDownColumn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownColumn_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownRow);
            this.groupBox1.Controls.Add(this.numericUpDownColumn);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownRow
            // 
            resources.ApplyResources(this.numericUpDownRow, "numericUpDownRow");
            this.numericUpDownRow.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDownRow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRow.Name = "numericUpDownRow";
            this.numericUpDownRow.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRow.ValueChanged += new System.EventHandler(this.numericUpDownRow_ValueChanged);
            this.numericUpDownRow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownRow_KeyDown);
            // 
            // FormDialogAutoPaste
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.labelTotalHeight);
            this.Controls.Add(this.labelTotalWidth);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.labelModuleHeight);
            this.Controls.Add(this.labelModuleWidth);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownColumnSpace);
            this.Controls.Add(this.numericUpDownRowSpace);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormDialogAutoPaste";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormDialogAutoPaste_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumnSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRowSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownColumn)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTotalHeight;
        private System.Windows.Forms.Label labelTotalWidth;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelModuleHeight;
        private System.Windows.Forms.Label labelModuleWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownColumnSpace;
        private System.Windows.Forms.NumericUpDown numericUpDownRowSpace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownColumn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownRow;
    }
}