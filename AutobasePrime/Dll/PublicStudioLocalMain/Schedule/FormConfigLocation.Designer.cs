namespace PublicStudioLocalMain.Schedule
{
    partial class FormConfigLocation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigLocation));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewLocations = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxNation = new System.Windows.Forms.ComboBox();
            this.buttonGoogleMap = new System.Windows.Forms.Button();
            this.buttonSetDefault = new System.Windows.Forms.Button();
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
            // listViewLocations
            // 
            this.listViewLocations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewLocations.FullRowSelect = true;
            this.listViewLocations.HideSelection = false;
            resources.ApplyResources(this.listViewLocations, "listViewLocations");
            this.listViewLocations.MultiSelect = false;
            this.listViewLocations.Name = "listViewLocations";
            this.listViewLocations.UseCompatibleStateImageBehavior = false;
            this.listViewLocations.View = System.Windows.Forms.View.Details;
            this.listViewLocations.DoubleClick += new System.EventHandler(this.listViewLocations_DoubleClick);
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
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonModify
            // 
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.UseVisualStyleBackColor = true;
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxNation
            // 
            this.comboBoxNation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNation.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxNation, "comboBoxNation");
            this.comboBoxNation.Name = "comboBoxNation";
            this.comboBoxNation.SelectedIndexChanged += new System.EventHandler(this.comboBoxNation_SelectedIndexChanged);
            // 
            // buttonGoogleMap
            // 
            resources.ApplyResources(this.buttonGoogleMap, "buttonGoogleMap");
            this.buttonGoogleMap.Name = "buttonGoogleMap";
            this.buttonGoogleMap.UseVisualStyleBackColor = true;
            this.buttonGoogleMap.Click += new System.EventHandler(this.buttonGoogleMap_Click);
            // 
            // buttonSetDefault
            // 
            resources.ApplyResources(this.buttonSetDefault, "buttonSetDefault");
            this.buttonSetDefault.Name = "buttonSetDefault";
            this.buttonSetDefault.UseVisualStyleBackColor = true;
            this.buttonSetDefault.Click += new System.EventHandler(this.buttonSetDefault_Click);
            // 
            // FormConfigLocation
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.buttonCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.buttonSetDefault);
            this.Controls.Add(this.buttonGoogleMap);
            this.Controls.Add(this.comboBoxNation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listViewLocations);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigLocation";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigLocation_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConfigLocation_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listViewLocations;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonModify;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxNation;
        private System.Windows.Forms.Button buttonGoogleMap;
        private System.Windows.Forms.Button buttonSetDefault;
    }
}