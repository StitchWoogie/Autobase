namespace Studio
{
    partial class FormPropertyAndScript
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyAndScript));
            this.radioButtonPropertyType0 = new System.Windows.Forms.RadioButton();
            this.radioButtonPropertyType1 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxBasic = new System.Windows.Forms.GroupBox();
            this.groupBoxScript = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.insertRGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonPropertyType0
            // 
            resources.ApplyResources(this.radioButtonPropertyType0, "radioButtonPropertyType0");
            this.radioButtonPropertyType0.Name = "radioButtonPropertyType0";
            this.radioButtonPropertyType0.TabStop = true;
            this.radioButtonPropertyType0.UseVisualStyleBackColor = true;
            this.radioButtonPropertyType0.CheckedChanged += new System.EventHandler(this.radioButtonPropertyType0_CheckedChanged);
            // 
            // radioButtonPropertyType1
            // 
            resources.ApplyResources(this.radioButtonPropertyType1, "radioButtonPropertyType1");
            this.radioButtonPropertyType1.Name = "radioButtonPropertyType1";
            this.radioButtonPropertyType1.TabStop = true;
            this.radioButtonPropertyType1.UseVisualStyleBackColor = true;
            this.radioButtonPropertyType1.CheckedChanged += new System.EventHandler(this.radioButtonPropertyType1_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonPropertyType0);
            this.groupBox1.Controls.Add(this.radioButtonPropertyType1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
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
            // groupBoxBasic
            // 
            resources.ApplyResources(this.groupBoxBasic, "groupBoxBasic");
            this.groupBoxBasic.Name = "groupBoxBasic";
            this.groupBoxBasic.TabStop = false;
            // 
            // groupBoxScript
            // 
            resources.ApplyResources(this.groupBoxScript, "groupBoxScript");
            this.groupBoxScript.Name = "groupBoxScript";
            this.groupBoxScript.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            resources.ApplyResources(this.fontToolStripMenuItem, "fontToolStripMenuItem");
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scriptCopyToolStripMenuItem,
            this.scriptPasteToolStripMenuItem,
            this.toolStripSeparator1,
            this.insertRGBToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // scriptCopyToolStripMenuItem
            // 
            this.scriptCopyToolStripMenuItem.Name = "scriptCopyToolStripMenuItem";
            resources.ApplyResources(this.scriptCopyToolStripMenuItem, "scriptCopyToolStripMenuItem");
            this.scriptCopyToolStripMenuItem.Click += new System.EventHandler(this.scriptCopyToolStripMenuItem_Click);
            // 
            // scriptPasteToolStripMenuItem
            // 
            this.scriptPasteToolStripMenuItem.Name = "scriptPasteToolStripMenuItem";
            resources.ApplyResources(this.scriptPasteToolStripMenuItem, "scriptPasteToolStripMenuItem");
            this.scriptPasteToolStripMenuItem.Click += new System.EventHandler(this.scriptPasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // insertRGBToolStripMenuItem
            // 
            this.insertRGBToolStripMenuItem.Name = "insertRGBToolStripMenuItem";
            resources.ApplyResources(this.insertRGBToolStripMenuItem, "insertRGBToolStripMenuItem");
            this.insertRGBToolStripMenuItem.Click += new System.EventHandler(this.insertRGBToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // FormPropertyAndScript
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxScript);
            this.Controls.Add(this.groupBoxBasic);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "FormPropertyAndScript";
            this.Load += new System.EventHandler(this.FormPropertyAndScript_Load);
            this.SizeChanged += new System.EventHandler(this.FormPropertyAndScript_SizeChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonPropertyType0;
        private System.Windows.Forms.RadioButton radioButtonPropertyType1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxBasic;
        private System.Windows.Forms.GroupBox groupBoxScript;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptCopyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptPasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem insertRGBToolStripMenuItem;
    }
}