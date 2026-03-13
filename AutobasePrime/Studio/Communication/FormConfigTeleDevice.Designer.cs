namespace Studio.Communication
{
    partial class FormConfigTeleDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigTeleDevice));
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxTeleConnectCommand = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxTeleInitCommand = new System.Windows.Forms.TextBox();
            this.numericUpDownTeleTimeout = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTeleTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox11
            // 
            this.groupBox11.AccessibleDescription = null;
            this.groupBox11.AccessibleName = null;
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.BackgroundImage = null;
            this.groupBox11.Controls.Add(this.label13);
            this.groupBox11.Controls.Add(this.textBoxTeleConnectCommand);
            this.groupBox11.Controls.Add(this.label12);
            this.groupBox11.Controls.Add(this.textBoxTeleInitCommand);
            this.groupBox11.Controls.Add(this.numericUpDownTeleTimeout);
            this.groupBox11.Controls.Add(this.label9);
            this.groupBox11.Controls.Add(this.label11);
            this.groupBox11.Font = null;
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // label13
            // 
            this.label13.AccessibleDescription = null;
            this.label13.AccessibleName = null;
            resources.ApplyResources(this.label13, "label13");
            this.label13.Font = null;
            this.label13.Name = "label13";
            // 
            // textBoxTeleConnectCommand
            // 
            this.textBoxTeleConnectCommand.AccessibleDescription = null;
            this.textBoxTeleConnectCommand.AccessibleName = null;
            resources.ApplyResources(this.textBoxTeleConnectCommand, "textBoxTeleConnectCommand");
            this.textBoxTeleConnectCommand.BackgroundImage = null;
            this.textBoxTeleConnectCommand.Font = null;
            this.textBoxTeleConnectCommand.Name = "textBoxTeleConnectCommand";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // textBoxTeleInitCommand
            // 
            this.textBoxTeleInitCommand.AccessibleDescription = null;
            this.textBoxTeleInitCommand.AccessibleName = null;
            resources.ApplyResources(this.textBoxTeleInitCommand, "textBoxTeleInitCommand");
            this.textBoxTeleInitCommand.BackgroundImage = null;
            this.textBoxTeleInitCommand.Font = null;
            this.textBoxTeleInitCommand.Name = "textBoxTeleInitCommand";
            // 
            // numericUpDownTeleTimeout
            // 
            this.numericUpDownTeleTimeout.AccessibleDescription = null;
            this.numericUpDownTeleTimeout.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTeleTimeout, "numericUpDownTeleTimeout");
            this.numericUpDownTeleTimeout.Font = null;
            this.numericUpDownTeleTimeout.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownTeleTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownTeleTimeout.Name = "numericUpDownTeleTimeout";
            this.numericUpDownTeleTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.label9.Font = null;
            this.label9.Name = "label9";
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
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
            // FormConfigTeleDevice
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox11);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigTeleDevice";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigTeleDevice_Load);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTeleTimeout)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxTeleConnectCommand;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxTeleInitCommand;
        private System.Windows.Forms.NumericUpDown numericUpDownTeleTimeout;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}