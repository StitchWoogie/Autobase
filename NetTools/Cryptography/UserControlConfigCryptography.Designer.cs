namespace NetTools.Cryptography
{
    partial class UserControlConfigCryptography
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlConfigCryptography));
            this.labelInfoIV = new System.Windows.Forms.Label();
            this.labelInfoKey = new System.Windows.Forms.Label();
            this.checkBoxUseEncryption = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonEngine1 = new System.Windows.Forms.RadioButton();
            this.radioButtonEngine0 = new System.Windows.Forms.RadioButton();
            this.comboBoxFrameMode = new System.Windows.Forms.ComboBox();
            this.label37 = new System.Windows.Forms.Label();
            this.comboBoxPaddingMode = new System.Windows.Forms.ComboBox();
            this.label36 = new System.Windows.Forms.Label();
            this.comboBoxCipherMode = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.textBoxIV = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelInfoIV
            // 
            resources.ApplyResources(this.labelInfoIV, "labelInfoIV");
            this.labelInfoIV.Name = "labelInfoIV";
            // 
            // labelInfoKey
            // 
            resources.ApplyResources(this.labelInfoKey, "labelInfoKey");
            this.labelInfoKey.Name = "labelInfoKey";
            // 
            // checkBoxUseEncryption
            // 
            resources.ApplyResources(this.checkBoxUseEncryption, "checkBoxUseEncryption");
            this.checkBoxUseEncryption.Name = "checkBoxUseEncryption";
            this.checkBoxUseEncryption.UseVisualStyleBackColor = true;
            this.checkBoxUseEncryption.CheckedChanged += new System.EventHandler(this.checkBoxUseEncryption_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonEngine1);
            this.groupBox1.Controls.Add(this.radioButtonEngine0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonEngine1
            // 
            resources.ApplyResources(this.radioButtonEngine1, "radioButtonEngine1");
            this.radioButtonEngine1.Name = "radioButtonEngine1";
            this.radioButtonEngine1.TabStop = true;
            this.radioButtonEngine1.UseVisualStyleBackColor = true;
            // 
            // radioButtonEngine0
            // 
            resources.ApplyResources(this.radioButtonEngine0, "radioButtonEngine0");
            this.radioButtonEngine0.Name = "radioButtonEngine0";
            this.radioButtonEngine0.TabStop = true;
            this.radioButtonEngine0.UseVisualStyleBackColor = true;
            // 
            // comboBoxFrameMode
            // 
            this.comboBoxFrameMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFrameMode.FormattingEnabled = true;
            this.comboBoxFrameMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxFrameMode.Items")});
            resources.ApplyResources(this.comboBoxFrameMode, "comboBoxFrameMode");
            this.comboBoxFrameMode.Name = "comboBoxFrameMode";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // comboBoxPaddingMode
            // 
            this.comboBoxPaddingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPaddingMode.FormattingEnabled = true;
            this.comboBoxPaddingMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxPaddingMode.Items"),
            resources.GetString("comboBoxPaddingMode.Items1"),
            resources.GetString("comboBoxPaddingMode.Items2"),
            resources.GetString("comboBoxPaddingMode.Items3"),
            resources.GetString("comboBoxPaddingMode.Items4")});
            resources.ApplyResources(this.comboBoxPaddingMode, "comboBoxPaddingMode");
            this.comboBoxPaddingMode.Name = "comboBoxPaddingMode";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // comboBoxCipherMode
            // 
            this.comboBoxCipherMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCipherMode.FormattingEnabled = true;
            this.comboBoxCipherMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxCipherMode.Items"),
            resources.GetString("comboBoxCipherMode.Items1"),
            resources.GetString("comboBoxCipherMode.Items2"),
            resources.GetString("comboBoxCipherMode.Items3"),
            resources.GetString("comboBoxCipherMode.Items4")});
            resources.ApplyResources(this.comboBoxCipherMode, "comboBoxCipherMode");
            this.comboBoxCipherMode.Name = "comboBoxCipherMode";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // textBoxIV
            // 
            resources.ApplyResources(this.textBoxIV, "textBoxIV");
            this.textBoxIV.Name = "textBoxIV";
            this.textBoxIV.TextChanged += new System.EventHandler(this.textBoxIV_TextChanged);
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // textBoxKey
            // 
            resources.ApplyResources(this.textBoxKey, "textBoxKey");
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.TextChanged += new System.EventHandler(this.textBoxKey_TextChanged);
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // UserControlConfigCryptography
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelInfoIV);
            this.Controls.Add(this.labelInfoKey);
            this.Controls.Add(this.checkBoxUseEncryption);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxFrameMode);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.comboBoxPaddingMode);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.comboBoxCipherMode);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.textBoxIV);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.textBoxKey);
            this.Controls.Add(this.label33);
            this.Name = "UserControlConfigCryptography";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInfoIV;
        private System.Windows.Forms.Label labelInfoKey;
        private System.Windows.Forms.CheckBox checkBoxUseEncryption;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonEngine1;
        private System.Windows.Forms.RadioButton radioButtonEngine0;
        private System.Windows.Forms.ComboBox comboBoxFrameMode;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.ComboBox comboBoxPaddingMode;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.ComboBox comboBoxCipherMode;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox textBoxIV;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Label label33;
    }
}
