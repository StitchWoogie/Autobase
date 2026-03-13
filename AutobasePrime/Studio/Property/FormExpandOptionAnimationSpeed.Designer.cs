namespace Studio
{
    partial class FormExpandOptionAnimationSpeed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExpandOptionAnimationSpeed));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.buttonTag = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBoxTag
            // 
            this.textBoxTag.AccessibleDescription = null;
            this.textBoxTag.AccessibleName = null;
            resources.ApplyResources(this.textBoxTag, "textBoxTag");
            this.textBoxTag.BackgroundImage = null;
            this.textBoxTag.Font = null;
            this.textBoxTag.Name = "textBoxTag";
            // 
            // buttonTag
            // 
            this.buttonTag.AccessibleDescription = null;
            this.buttonTag.AccessibleName = null;
            resources.ApplyResources(this.buttonTag, "buttonTag");
            this.buttonTag.BackgroundImage = null;
            this.buttonTag.Font = null;
            this.buttonTag.Name = "buttonTag";
            this.buttonTag.UseVisualStyleBackColor = true;
            this.buttonTag.Click += new System.EventHandler(this.buttonTag_Click);
            // 
            // FormExpandOptionAnimationSpeed
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.buttonTag);
            this.Controls.Add(this.textBoxTag);
            this.Controls.Add(this.label1);
            this.Font = null;
            this.Icon = null;
            this.Name = "FormExpandOptionAnimationSpeed";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTag;
        private System.Windows.Forms.Button buttonTag;
    }
}