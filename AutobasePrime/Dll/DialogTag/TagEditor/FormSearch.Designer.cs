namespace DialogTag.TagEditor
{
    partial class FormSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSearch));
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxSearchText = new System.Windows.Forms.ComboBox();
            this.comboBoxReplaceText = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSearchAtAll = new System.Windows.Forms.CheckBox();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.AccessibleDescription = null;
            this.buttonFindNext.AccessibleName = null;
            resources.ApplyResources(this.buttonFindNext, "buttonFindNext");
            this.buttonFindNext.BackgroundImage = null;
            this.buttonFindNext.Font = null;
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.AccessibleDescription = null;
            this.buttonReplace.AccessibleName = null;
            resources.ApplyResources(this.buttonReplace, "buttonReplace");
            this.buttonReplace.BackgroundImage = null;
            this.buttonReplace.Font = null;
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comboBoxSearchText
            // 
            this.comboBoxSearchText.AccessibleDescription = null;
            this.comboBoxSearchText.AccessibleName = null;
            resources.ApplyResources(this.comboBoxSearchText, "comboBoxSearchText");
            this.comboBoxSearchText.BackgroundImage = null;
            this.comboBoxSearchText.Font = null;
            this.comboBoxSearchText.FormattingEnabled = true;
            this.comboBoxSearchText.Name = "comboBoxSearchText";
            this.comboBoxSearchText.TextChanged += new System.EventHandler(this.comboBoxSearchText_TextChanged);
            // 
            // comboBoxReplaceText
            // 
            this.comboBoxReplaceText.AccessibleDescription = null;
            this.comboBoxReplaceText.AccessibleName = null;
            resources.ApplyResources(this.comboBoxReplaceText, "comboBoxReplaceText");
            this.comboBoxReplaceText.BackgroundImage = null;
            this.comboBoxReplaceText.Font = null;
            this.comboBoxReplaceText.FormattingEnabled = true;
            this.comboBoxReplaceText.Name = "comboBoxReplaceText";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.checkBoxSearchAtAll);
            this.groupBox1.Controls.Add(this.checkBoxMatchCase);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxSearchAtAll
            // 
            this.checkBoxSearchAtAll.AccessibleDescription = null;
            this.checkBoxSearchAtAll.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSearchAtAll, "checkBoxSearchAtAll");
            this.checkBoxSearchAtAll.BackgroundImage = null;
            this.checkBoxSearchAtAll.Font = null;
            this.checkBoxSearchAtAll.Name = "checkBoxSearchAtAll";
            this.checkBoxSearchAtAll.UseVisualStyleBackColor = true;
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.AccessibleDescription = null;
            this.checkBoxMatchCase.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMatchCase, "checkBoxMatchCase");
            this.checkBoxMatchCase.BackgroundImage = null;
            this.checkBoxMatchCase.Font = null;
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // FormSearch
            // 
            this.AcceptButton = this.buttonFindNext;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxReplaceText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxSearchText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.buttonFindNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSearch";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormSearch_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSearch_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFindNext;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxSearchText;
        private System.Windows.Forms.ComboBox comboBoxReplaceText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.CheckBox checkBoxSearchAtAll;

    }
}