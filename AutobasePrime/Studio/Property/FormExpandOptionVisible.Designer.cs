namespace Studio
{
    partial class FormExpandOptionVisible
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExpandOptionVisible));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.buttonTag = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxCondition = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxTag
            // 
            resources.ApplyResources(this.textBoxTag, "textBoxTag");
            this.textBoxTag.Name = "textBoxTag";
            // 
            // buttonTag
            // 
            resources.ApplyResources(this.buttonTag, "buttonTag");
            this.buttonTag.Name = "buttonTag";
            this.buttonTag.UseVisualStyleBackColor = true;
            this.buttonTag.Click += new System.EventHandler(this.buttonTag_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxCondition);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxValue);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxCondition
            // 
            this.comboBoxCondition.DropDownHeight = 200;
            this.comboBoxCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCondition.DropDownWidth = 125;
            this.comboBoxCondition.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCondition, "comboBoxCondition");
            this.comboBoxCondition.Name = "comboBoxCondition";
            this.comboBoxCondition.SelectedIndexChanged += new System.EventHandler(this.comboBoxCondition_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textBoxValue
            // 
            resources.ApplyResources(this.textBoxValue, "textBoxValue");
            this.textBoxValue.Name = "textBoxValue";
            // 
            // FormExpandOptionVisible
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonTag);
            this.Controls.Add(this.textBoxTag);
            this.Controls.Add(this.label1);
            this.Name = "FormExpandOptionVisible";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTag;
        private System.Windows.Forms.Button buttonTag;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxCondition;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxValue;
    }
}