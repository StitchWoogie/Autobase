namespace Studio.Script
{
    partial class FormConfigScriptNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScriptNew));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxAutomaticallyFormat = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayLineEndingGlyph = new System.Windows.Forms.CheckBox();
            this.checkBoxDrawingByMemory = new System.Windows.Forms.CheckBox();
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
            // checkBoxAutomaticallyFormat
            // 
            this.checkBoxAutomaticallyFormat.AccessibleDescription = null;
            this.checkBoxAutomaticallyFormat.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAutomaticallyFormat, "checkBoxAutomaticallyFormat");
            this.checkBoxAutomaticallyFormat.BackgroundImage = null;
            this.checkBoxAutomaticallyFormat.Font = null;
            this.checkBoxAutomaticallyFormat.Name = "checkBoxAutomaticallyFormat";
            this.checkBoxAutomaticallyFormat.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisplayLineEndingGlyph
            // 
            this.checkBoxDisplayLineEndingGlyph.AccessibleDescription = null;
            this.checkBoxDisplayLineEndingGlyph.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDisplayLineEndingGlyph, "checkBoxDisplayLineEndingGlyph");
            this.checkBoxDisplayLineEndingGlyph.BackgroundImage = null;
            this.checkBoxDisplayLineEndingGlyph.Font = null;
            this.checkBoxDisplayLineEndingGlyph.Name = "checkBoxDisplayLineEndingGlyph";
            this.checkBoxDisplayLineEndingGlyph.UseVisualStyleBackColor = true;
            // 
            // checkBoxDrawingByMemory
            // 
            this.checkBoxDrawingByMemory.AccessibleDescription = null;
            this.checkBoxDrawingByMemory.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDrawingByMemory, "checkBoxDrawingByMemory");
            this.checkBoxDrawingByMemory.BackgroundImage = null;
            this.checkBoxDrawingByMemory.Font = null;
            this.checkBoxDrawingByMemory.Name = "checkBoxDrawingByMemory";
            this.checkBoxDrawingByMemory.UseVisualStyleBackColor = true;
            // 
            // FormConfigScriptNew
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxDrawingByMemory);
            this.Controls.Add(this.checkBoxDisplayLineEndingGlyph);
            this.Controls.Add(this.checkBoxAutomaticallyFormat);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScriptNew";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigScriptNew_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxAutomaticallyFormat;
        private System.Windows.Forms.CheckBox checkBoxDisplayLineEndingGlyph;
        private System.Windows.Forms.CheckBox checkBoxDrawingByMemory;
    }
}