namespace AniEditLib
{
    partial class FormNewAnimation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewAnimation));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownFrame = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownRpm = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxColor = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxFrameTime = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRpm)).BeginInit();
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
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.AccessibleDescription = null;
            this.numericUpDownWidth.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownWidth, "numericUpDownWidth");
            this.numericUpDownWidth.Font = null;
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.AccessibleDescription = null;
            this.numericUpDownHeight.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownHeight, "numericUpDownHeight");
            this.numericUpDownHeight.Font = null;
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Value = new decimal(new int[] {
            100,
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
            // numericUpDownFrame
            // 
            this.numericUpDownFrame.AccessibleDescription = null;
            this.numericUpDownFrame.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownFrame, "numericUpDownFrame");
            this.numericUpDownFrame.Font = null;
            this.numericUpDownFrame.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrame.Name = "numericUpDownFrame";
            this.numericUpDownFrame.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownFrame.ValueChanged += new System.EventHandler(this.numericUpDownFrame_ValueChanged);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownRpm
            // 
            this.numericUpDownRpm.AccessibleDescription = null;
            this.numericUpDownRpm.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownRpm, "numericUpDownRpm");
            this.numericUpDownRpm.Font = null;
            this.numericUpDownRpm.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownRpm.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRpm.Name = "numericUpDownRpm";
            this.numericUpDownRpm.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownRpm.ValueChanged += new System.EventHandler(this.numericUpDownRpm_ValueChanged);
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // comboBoxColor
            // 
            this.comboBoxColor.AccessibleDescription = null;
            this.comboBoxColor.AccessibleName = null;
            resources.ApplyResources(this.comboBoxColor, "comboBoxColor");
            this.comboBoxColor.BackgroundImage = null;
            this.comboBoxColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColor.Font = null;
            this.comboBoxColor.FormattingEnabled = true;
            this.comboBoxColor.Name = "comboBoxColor";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // textBoxFrameTime
            // 
            this.textBoxFrameTime.AccessibleDescription = null;
            this.textBoxFrameTime.AccessibleName = null;
            resources.ApplyResources(this.textBoxFrameTime, "textBoxFrameTime");
            this.textBoxFrameTime.BackgroundImage = null;
            this.textBoxFrameTime.Font = null;
            this.textBoxFrameTime.Name = "textBoxFrameTime";
            this.textBoxFrameTime.ReadOnly = true;
            // 
            // FormNewAnimation
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.textBoxFrameTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxColor);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownRpm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownFrame);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewAnimation";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormNewAnimation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRpm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownWidth;
        private System.Windows.Forms.NumericUpDown numericUpDownHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownFrame;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownRpm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxColor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxFrameTime;
    }
}