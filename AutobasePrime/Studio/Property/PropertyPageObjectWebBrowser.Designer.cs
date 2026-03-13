namespace Studio
{
    partial class PropertyPageObjectWebBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectWebBrowser));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxStyleScrollBar = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleContextMenu = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleAllowNavigation = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxUrl);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.AccessibleDescription = null;
            this.textBoxUrl.AccessibleName = null;
            resources.ApplyResources(this.textBoxUrl, "textBoxUrl");
            this.textBoxUrl.BackgroundImage = null;
            this.textBoxUrl.Font = null;
            this.textBoxUrl.Name = "textBoxUrl";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxStyleScrollBar);
            this.groupBox2.Controls.Add(this.checkBoxStyleContextMenu);
            this.groupBox2.Controls.Add(this.checkBoxStyleAllowNavigation);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxStyleScrollBar
            // 
            this.checkBoxStyleScrollBar.AccessibleDescription = null;
            this.checkBoxStyleScrollBar.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleScrollBar, "checkBoxStyleScrollBar");
            this.checkBoxStyleScrollBar.BackgroundImage = null;
            this.checkBoxStyleScrollBar.Font = null;
            this.checkBoxStyleScrollBar.Name = "checkBoxStyleScrollBar";
            // 
            // checkBoxStyleContextMenu
            // 
            this.checkBoxStyleContextMenu.AccessibleDescription = null;
            this.checkBoxStyleContextMenu.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleContextMenu, "checkBoxStyleContextMenu");
            this.checkBoxStyleContextMenu.BackgroundImage = null;
            this.checkBoxStyleContextMenu.Font = null;
            this.checkBoxStyleContextMenu.Name = "checkBoxStyleContextMenu";
            // 
            // checkBoxStyleAllowNavigation
            // 
            this.checkBoxStyleAllowNavigation.AccessibleDescription = null;
            this.checkBoxStyleAllowNavigation.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleAllowNavigation, "checkBoxStyleAllowNavigation");
            this.checkBoxStyleAllowNavigation.BackgroundImage = null;
            this.checkBoxStyleAllowNavigation.Font = null;
            this.checkBoxStyleAllowNavigation.Name = "checkBoxStyleAllowNavigation";
            // 
            // PropertyPageObjectWebBrowser
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectWebBrowser";
            this.Load += new System.EventHandler(this.PropertyPageObjectWebBrowser_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxStyleScrollBar;
        private System.Windows.Forms.CheckBox checkBoxStyleContextMenu;
        private System.Windows.Forms.CheckBox checkBoxStyleAllowNavigation;
    }
}