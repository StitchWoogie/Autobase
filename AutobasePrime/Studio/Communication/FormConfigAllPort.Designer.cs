namespace Studio.Communication
{
    partial class FormConfigAllPort
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigAllPort));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.buttonModify = new System.Windows.Forms.Button();
            this.buttonPortRestart = new System.Windows.Forms.Button();
            this.groupBoxDllDeployment = new System.Windows.Forms.GroupBox();
            this.buttonDeploy = new System.Windows.Forms.Button();
            this.comboBoxProgramVersion = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxDllDeployment.SuspendLayout();
            this.SuspendLayout();
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
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // listView1
            // 
            this.listView1.AccessibleDescription = null;
            this.listView1.AccessibleName = null;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.BackgroundImage = null;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Font = null;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
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
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // buttonModify
            // 
            this.buttonModify.AccessibleDescription = null;
            this.buttonModify.AccessibleName = null;
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.BackgroundImage = null;
            this.buttonModify.Font = null;
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.UseVisualStyleBackColor = true;
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // buttonPortRestart
            // 
            this.buttonPortRestart.AccessibleDescription = null;
            this.buttonPortRestart.AccessibleName = null;
            resources.ApplyResources(this.buttonPortRestart, "buttonPortRestart");
            this.buttonPortRestart.BackgroundImage = null;
            this.buttonPortRestart.Font = null;
            this.buttonPortRestart.Name = "buttonPortRestart";
            this.buttonPortRestart.UseVisualStyleBackColor = true;
            this.buttonPortRestart.Click += new System.EventHandler(this.buttonPortRestart_Click);
            // 
            // groupBoxDllDeployment
            // 
            this.groupBoxDllDeployment.AccessibleDescription = null;
            this.groupBoxDllDeployment.AccessibleName = null;
            resources.ApplyResources(this.groupBoxDllDeployment, "groupBoxDllDeployment");
            this.groupBoxDllDeployment.BackgroundImage = null;
            this.groupBoxDllDeployment.Controls.Add(this.buttonDeploy);
            this.groupBoxDllDeployment.Controls.Add(this.comboBoxProgramVersion);
            this.groupBoxDllDeployment.Controls.Add(this.label1);
            this.groupBoxDllDeployment.Font = null;
            this.groupBoxDllDeployment.Name = "groupBoxDllDeployment";
            this.groupBoxDllDeployment.TabStop = false;
            // 
            // buttonDeploy
            // 
            this.buttonDeploy.AccessibleDescription = null;
            this.buttonDeploy.AccessibleName = null;
            resources.ApplyResources(this.buttonDeploy, "buttonDeploy");
            this.buttonDeploy.BackgroundImage = null;
            this.buttonDeploy.Font = null;
            this.buttonDeploy.Name = "buttonDeploy";
            this.buttonDeploy.UseVisualStyleBackColor = true;
            this.buttonDeploy.Click += new System.EventHandler(this.buttonDeploy_Click);
            // 
            // comboBoxProgramVersion
            // 
            this.comboBoxProgramVersion.AccessibleDescription = null;
            this.comboBoxProgramVersion.AccessibleName = null;
            resources.ApplyResources(this.comboBoxProgramVersion, "comboBoxProgramVersion");
            this.comboBoxProgramVersion.BackgroundImage = null;
            this.comboBoxProgramVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProgramVersion.Font = null;
            this.comboBoxProgramVersion.FormattingEnabled = true;
            this.comboBoxProgramVersion.Name = "comboBoxProgramVersion";
            this.comboBoxProgramVersion.Sorted = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // FormConfigAllPort
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxDllDeployment);
            this.Controls.Add(this.buttonPortRestart);
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigAllPort";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigAllPort_Load);
            this.groupBoxDllDeployment.ResumeLayout(false);
            this.groupBoxDllDeployment.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button buttonModify;
        private System.Windows.Forms.Button buttonPortRestart;
        private System.Windows.Forms.GroupBox groupBoxDllDeployment;
        private System.Windows.Forms.Button buttonDeploy;
        private System.Windows.Forms.ComboBox comboBoxProgramVersion;
        private System.Windows.Forms.Label label1;
    }
}