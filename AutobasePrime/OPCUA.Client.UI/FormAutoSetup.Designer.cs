namespace OPCUA.Client.UI
{
    partial class FormAutoSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoSetup));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numRestartInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTimerRestart = new System.Windows.Forms.Label();
            this.checkBoxRestart = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRestartInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numRestartInterval);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.labelTimerRestart);
            this.groupBox2.Controls.Add(this.checkBoxRestart);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Restart";
            // 
            // numRestartInterval
            // 
            this.numRestartInterval.Location = new System.Drawing.Point(62, 49);
            this.numRestartInterval.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numRestartInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numRestartInterval.Name = "numRestartInterval";
            this.numRestartInterval.Size = new System.Drawing.Size(82, 21);
            this.numRestartInterval.TabIndex = 2;
            this.numRestartInterval.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sec";
            // 
            // labelTimerRestart
            // 
            this.labelTimerRestart.AutoSize = true;
            this.labelTimerRestart.Location = new System.Drawing.Point(6, 53);
            this.labelTimerRestart.Name = "labelTimerRestart";
            this.labelTimerRestart.Size = new System.Drawing.Size(50, 12);
            this.labelTimerRestart.TabIndex = 1;
            this.labelTimerRestart.Text = "Timer : ";
            // 
            // checkBoxRestart
            // 
            this.checkBoxRestart.AutoSize = true;
            this.checkBoxRestart.Location = new System.Drawing.Point(62, 20);
            this.checkBoxRestart.Name = "checkBoxRestart";
            this.checkBoxRestart.Size = new System.Drawing.Size(63, 16);
            this.checkBoxRestart.TabIndex = 0;
            this.checkBoxRestart.Text = "Enable";
            this.checkBoxRestart.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(305, 25);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(305, 59);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormAutoSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 121);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAutoSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AutoSetup";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRestartInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.NumericUpDown numRestartInterval;
        private System.Windows.Forms.Label labelTimerRestart;
        private System.Windows.Forms.CheckBox checkBoxRestart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}