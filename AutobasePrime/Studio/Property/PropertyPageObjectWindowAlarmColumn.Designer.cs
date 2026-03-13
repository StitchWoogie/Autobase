namespace Studio.Property
{
    partial class PropertyPageObjectWindowAlarmColumn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectWindowAlarmColumn));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewColumn = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.checkBoxVisible = new System.Windows.Forms.CheckBox();
            this.textBoxColumnTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            // listViewColumn
            // 
            this.listViewColumn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewColumn.FullRowSelect = true;
            this.listViewColumn.HideSelection = false;
            resources.ApplyResources(this.listViewColumn, "listViewColumn");
            this.listViewColumn.Name = "listViewColumn";
            this.listViewColumn.UseCompatibleStateImageBehavior = false;
            this.listViewColumn.View = System.Windows.Forms.View.Details;
            this.listViewColumn.SelectedIndexChanged += new System.EventHandler(this.listViewColumn_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // checkBoxVisible
            // 
            resources.ApplyResources(this.checkBoxVisible, "checkBoxVisible");
            this.checkBoxVisible.Name = "checkBoxVisible";
            this.checkBoxVisible.UseVisualStyleBackColor = true;
            // 
            // textBoxColumnTitle
            // 
            resources.ApplyResources(this.textBoxColumnTitle, "textBoxColumnTitle");
            this.textBoxColumnTitle.Name = "textBoxColumnTitle";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonDefault
            // 
            resources.ApplyResources(this.buttonDefault, "buttonDefault");
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // PropertyPageObjectWindowAlarmColumn
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxColumnTitle);
            this.Controls.Add(this.checkBoxVisible);
            this.Controls.Add(this.listViewColumn);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyPageObjectWindowAlarmColumn";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PropertyPageObjectWindowAlarmColumn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listViewColumn;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox checkBoxVisible;
        private System.Windows.Forms.TextBox textBoxColumnTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDefault;
    }
}