using AutoLibLocal;
namespace BasicScreen.kdymain
{
    partial class ViewListAlarmDetailConfigSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewListAlarmDetailConfigSearch));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownToDay = new AutoLibLocal.MyNumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownToMonth = new AutoLibLocal.MyNumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownToYear = new AutoLibLocal.MyNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownFrDay = new AutoLibLocal.MyNumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownFrMonth = new AutoLibLocal.MyNumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownFrYear = new AutoLibLocal.MyNumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterPort = new System.Windows.Forms.TextBox();
            this.checkBoxFilterPort = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listBoxFilterAlarmType = new System.Windows.Forms.ListBox();
            this.checkBoxFilterAlarmType = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxFilterTag = new System.Windows.Forms.TextBox();
            this.checkBoxFilterTag = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrYear)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numericUpDownToDay);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericUpDownToMonth);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numericUpDownToYear);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownFrDay);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownFrMonth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownFrYear);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // numericUpDownToDay
            // 
            resources.ApplyResources(this.numericUpDownToDay, "numericUpDownToDay");
            this.numericUpDownToDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownToDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToDay.Name = "numericUpDownToDay";
            this.numericUpDownToDay.SampleProperty = 0;
            this.numericUpDownToDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericUpDownToMonth
            // 
            resources.ApplyResources(this.numericUpDownToMonth, "numericUpDownToMonth");
            this.numericUpDownToMonth.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownToMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToMonth.Name = "numericUpDownToMonth";
            this.numericUpDownToMonth.SampleProperty = 0;
            this.numericUpDownToMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // numericUpDownToYear
            // 
            resources.ApplyResources(this.numericUpDownToYear, "numericUpDownToYear");
            this.numericUpDownToYear.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownToYear.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToYear.Name = "numericUpDownToYear";
            this.numericUpDownToYear.SampleProperty = 0;
            this.numericUpDownToYear.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownFrDay
            // 
            resources.ApplyResources(this.numericUpDownFrDay, "numericUpDownFrDay");
            this.numericUpDownFrDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownFrDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrDay.Name = "numericUpDownFrDay";
            this.numericUpDownFrDay.SampleProperty = 0;
            this.numericUpDownFrDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownFrMonth
            // 
            resources.ApplyResources(this.numericUpDownFrMonth, "numericUpDownFrMonth");
            this.numericUpDownFrMonth.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownFrMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrMonth.Name = "numericUpDownFrMonth";
            this.numericUpDownFrMonth.SampleProperty = 0;
            this.numericUpDownFrMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDownFrYear
            // 
            resources.ApplyResources(this.numericUpDownFrYear, "numericUpDownFrYear");
            this.numericUpDownFrYear.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownFrYear.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrYear.Name = "numericUpDownFrYear";
            this.numericUpDownFrYear.SampleProperty = 0;
            this.numericUpDownFrYear.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxFilterPort);
            this.groupBox3.Controls.Add(this.checkBoxFilterPort);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // textBoxFilterPort
            // 
            resources.ApplyResources(this.textBoxFilterPort, "textBoxFilterPort");
            this.textBoxFilterPort.Name = "textBoxFilterPort";
            // 
            // checkBoxFilterPort
            // 
            resources.ApplyResources(this.checkBoxFilterPort, "checkBoxFilterPort");
            this.checkBoxFilterPort.Name = "checkBoxFilterPort";
            this.checkBoxFilterPort.UseVisualStyleBackColor = true;
            this.checkBoxFilterPort.CheckedChanged += new System.EventHandler(this.checkBoxFilterPort_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxFilterAlarmType);
            this.groupBox4.Controls.Add(this.checkBoxFilterAlarmType);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // listBoxFilterAlarmType
            // 
            this.listBoxFilterAlarmType.FormattingEnabled = true;
            resources.ApplyResources(this.listBoxFilterAlarmType, "listBoxFilterAlarmType");
            this.listBoxFilterAlarmType.Name = "listBoxFilterAlarmType";
            this.listBoxFilterAlarmType.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // checkBoxFilterAlarmType
            // 
            resources.ApplyResources(this.checkBoxFilterAlarmType, "checkBoxFilterAlarmType");
            this.checkBoxFilterAlarmType.Name = "checkBoxFilterAlarmType";
            this.checkBoxFilterAlarmType.UseVisualStyleBackColor = true;
            this.checkBoxFilterAlarmType.CheckedChanged += new System.EventHandler(this.checkBoxFilterAlarmType_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxFilterTag);
            this.groupBox5.Controls.Add(this.checkBoxFilterTag);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // textBoxFilterTag
            // 
            resources.ApplyResources(this.textBoxFilterTag, "textBoxFilterTag");
            this.textBoxFilterTag.Name = "textBoxFilterTag";
            // 
            // checkBoxFilterTag
            // 
            resources.ApplyResources(this.checkBoxFilterTag, "checkBoxFilterTag");
            this.checkBoxFilterTag.Name = "checkBoxFilterTag";
            this.checkBoxFilterTag.UseVisualStyleBackColor = true;
            this.checkBoxFilterTag.CheckedChanged += new System.EventHandler(this.checkBoxFilterTag_CheckedChanged);
            // 
            // ViewListAlarmDetailConfigSearch
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewListAlarmDetailConfigSearch";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewListAlarmDetailConfigSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrYear)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private MyNumericUpDown numericUpDownFrDay;
        private System.Windows.Forms.Label label2;
        private MyNumericUpDown numericUpDownFrMonth;
        private System.Windows.Forms.Label label1;
        private MyNumericUpDown numericUpDownFrYear;
        private System.Windows.Forms.Label label6;
        private MyNumericUpDown numericUpDownToDay;
        private System.Windows.Forms.Label label7;
        private MyNumericUpDown numericUpDownToMonth;
        private System.Windows.Forms.Label label8;
        private MyNumericUpDown numericUpDownToYear;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxFilterPort;
        private System.Windows.Forms.CheckBox checkBoxFilterPort;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxFilterAlarmType;
        private System.Windows.Forms.ListBox listBoxFilterAlarmType;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxFilterTag;
        private System.Windows.Forms.CheckBox checkBoxFilterTag;
    }
}