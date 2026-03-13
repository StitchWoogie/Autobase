namespace Studio.Property
{
    partial class PropertyPageObjectControlTreeView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectControlTreeView));
            this.buttonEventDoubleClick = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxStyleBorder = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonEventDoubleClick
            // 
            resources.ApplyResources(this.buttonEventDoubleClick, "buttonEventDoubleClick");
            this.buttonEventDoubleClick.Name = "buttonEventDoubleClick";
            this.buttonEventDoubleClick.UseVisualStyleBackColor = true;
            this.buttonEventDoubleClick.Click += new System.EventHandler(this.buttonEventDoubleClick_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxStyleBorder);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxStyleBorder
            // 
            resources.ApplyResources(this.checkBoxStyleBorder, "checkBoxStyleBorder");
            this.checkBoxStyleBorder.Name = "checkBoxStyleBorder";
            // 
            // PropertyPageObjectControlTreeView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonEventDoubleClick);
            this.Name = "PropertyPageObjectControlTreeView";
            this.Load += new System.EventHandler(this.PropertyPageObjectControlTreeView_Load);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonEventDoubleClick;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxStyleBorder;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}