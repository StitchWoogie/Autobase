namespace Studio.Library
{
    partial class FormConfigItemProperty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigItemProperty));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPrice = new System.Windows.Forms.NumericUpDown();
            this.checkBoxShareOnWeb = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.groupBoxWebLibrary = new System.Windows.Forms.GroupBox();
            this.textBoxKeywords = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrice)).BeginInit();
            this.groupBoxWebLibrary.SuspendLayout();
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
            // numericUpDownPrice
            // 
            this.numericUpDownPrice.AccessibleDescription = null;
            this.numericUpDownPrice.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownPrice, "numericUpDownPrice");
            this.numericUpDownPrice.Font = null;
            this.numericUpDownPrice.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPrice.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownPrice.Name = "numericUpDownPrice";
            // 
            // checkBoxShareOnWeb
            // 
            this.checkBoxShareOnWeb.AccessibleDescription = null;
            this.checkBoxShareOnWeb.AccessibleName = null;
            resources.ApplyResources(this.checkBoxShareOnWeb, "checkBoxShareOnWeb");
            this.checkBoxShareOnWeb.BackgroundImage = null;
            this.checkBoxShareOnWeb.Font = null;
            this.checkBoxShareOnWeb.Name = "checkBoxShareOnWeb";
            this.checkBoxShareOnWeb.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // textBoxComment
            // 
            this.textBoxComment.AccessibleDescription = null;
            this.textBoxComment.AccessibleName = null;
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.BackgroundImage = null;
            this.textBoxComment.Font = null;
            this.textBoxComment.Name = "textBoxComment";
            // 
            // groupBoxWebLibrary
            // 
            this.groupBoxWebLibrary.AccessibleDescription = null;
            this.groupBoxWebLibrary.AccessibleName = null;
            resources.ApplyResources(this.groupBoxWebLibrary, "groupBoxWebLibrary");
            this.groupBoxWebLibrary.BackgroundImage = null;
            this.groupBoxWebLibrary.Controls.Add(this.checkBoxShareOnWeb);
            this.groupBoxWebLibrary.Controls.Add(this.textBoxComment);
            this.groupBoxWebLibrary.Controls.Add(this.label1);
            this.groupBoxWebLibrary.Controls.Add(this.label2);
            this.groupBoxWebLibrary.Controls.Add(this.numericUpDownPrice);
            this.groupBoxWebLibrary.Font = null;
            this.groupBoxWebLibrary.Name = "groupBoxWebLibrary";
            this.groupBoxWebLibrary.TabStop = false;
            // 
            // textBoxKeywords
            // 
            this.textBoxKeywords.AccessibleDescription = null;
            this.textBoxKeywords.AccessibleName = null;
            resources.ApplyResources(this.textBoxKeywords, "textBoxKeywords");
            this.textBoxKeywords.BackgroundImage = null;
            this.textBoxKeywords.Font = null;
            this.textBoxKeywords.Name = "textBoxKeywords";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // FormConfigItemProperty
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.textBoxKeywords);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBoxWebLibrary);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigItemProperty";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrice)).EndInit();
            this.groupBoxWebLibrary.ResumeLayout(false);
            this.groupBoxWebLibrary.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownPrice;
        private System.Windows.Forms.CheckBox checkBoxShareOnWeb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.GroupBox groupBoxWebLibrary;
        private System.Windows.Forms.TextBox textBoxKeywords;
        private System.Windows.Forms.Label label3;
    }
}