namespace Studio
{
    partial class PropertyPageControlBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageControlBox));
            this.checkBoxUseModule = new System.Windows.Forms.CheckBox();
            this.textBoxModuleName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonSearchModule = new System.Windows.Forms.Button();
            this.buttonSearchLibrary = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxUseModule
            // 
            resources.ApplyResources(this.checkBoxUseModule, "checkBoxUseModule");
            this.checkBoxUseModule.Name = "checkBoxUseModule";
            this.checkBoxUseModule.UseVisualStyleBackColor = true;
            this.checkBoxUseModule.CheckedChanged += new System.EventHandler(this.checkBoxUseModule_CheckedChanged);
            // 
            // textBoxModuleName
            // 
            resources.ApplyResources(this.textBoxModuleName, "textBoxModuleName");
            this.textBoxModuleName.Name = "textBoxModuleName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxTitle
            // 
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // buttonSearchModule
            // 
            resources.ApplyResources(this.buttonSearchModule, "buttonSearchModule");
            this.buttonSearchModule.Name = "buttonSearchModule";
            this.buttonSearchModule.UseVisualStyleBackColor = true;
            this.buttonSearchModule.Click += new System.EventHandler(this.buttonSearchModule_Click);
            // 
            // buttonSearchLibrary
            // 
            resources.ApplyResources(this.buttonSearchLibrary, "buttonSearchLibrary");
            this.buttonSearchLibrary.Name = "buttonSearchLibrary";
            this.buttonSearchLibrary.UseVisualStyleBackColor = true;
            this.buttonSearchLibrary.Click += new System.EventHandler(this.buttonSearchLibrary_Click);
            // 
            // PropertyPageControlBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonSearchLibrary);
            this.Controls.Add(this.buttonSearchModule);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxModuleName);
            this.Controls.Add(this.checkBoxUseModule);
            this.Name = "PropertyPageControlBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxUseModule;
        private System.Windows.Forms.TextBox textBoxModuleName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonSearchModule;
        private System.Windows.Forms.Button buttonSearchLibrary;
    }
}