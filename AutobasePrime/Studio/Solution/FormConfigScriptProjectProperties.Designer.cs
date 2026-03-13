namespace Studio.Solution
{
    partial class FormConfigScriptProjectProperties
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDefaultNamespace = new System.Windows.Forms.TextBox();
            this.textBoxProjectPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAddReference = new System.Windows.Forms.Button();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.listViewReference = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(349, 12);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(349, 41);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Default Namespace";
            // 
            // textBoxDefaultNamespace
            // 
            this.textBoxDefaultNamespace.Location = new System.Drawing.Point(16, 28);
            this.textBoxDefaultNamespace.Name = "textBoxDefaultNamespace";
            this.textBoxDefaultNamespace.Size = new System.Drawing.Size(213, 20);
            this.textBoxDefaultNamespace.TabIndex = 3;
            // 
            // textBoxProjectPath
            // 
            this.textBoxProjectPath.Location = new System.Drawing.Point(16, 77);
            this.textBoxProjectPath.Name = "textBoxProjectPath";
            this.textBoxProjectPath.ReadOnly = true;
            this.textBoxProjectPath.Size = new System.Drawing.Size(408, 20);
            this.textBoxProjectPath.TabIndex = 5;
            this.textBoxProjectPath.TextChanged += new System.EventHandler(this.textBoxProjectPath_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Project path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Reference";
            // 
            // buttonAddReference
            // 
            this.buttonAddReference.Location = new System.Drawing.Point(349, 138);
            this.buttonAddReference.Name = "buttonAddReference";
            this.buttonAddReference.Size = new System.Drawing.Size(75, 23);
            this.buttonAddReference.TabIndex = 8;
            this.buttonAddReference.Text = "Add";
            this.buttonAddReference.UseVisualStyleBackColor = true;
            this.buttonAddReference.Click += new System.EventHandler(this.buttonAddReference_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 250;
            // 
            // listViewReference
            // 
            this.listViewReference.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewReference.Location = new System.Drawing.Point(16, 138);
            this.listViewReference.Name = "listViewReference";
            this.listViewReference.Size = new System.Drawing.Size(322, 171);
            this.listViewReference.TabIndex = 6;
            this.listViewReference.UseCompatibleStateImageBehavior = false;
            this.listViewReference.View = System.Windows.Forms.View.Details;
            // 
            // FormConfigScriptProjectProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(436, 321);
            this.Controls.Add(this.buttonAddReference);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listViewReference);
            this.Controls.Add(this.textBoxProjectPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxDefaultNamespace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScriptProjectProperties";
            this.ShowInTaskbar = false;
            this.Text = "FormConfigScriptProjectProperties";
            this.Load += new System.EventHandler(this.FormConfigScriptProjectProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDefaultNamespace;
        private System.Windows.Forms.TextBox textBoxProjectPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonAddReference;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listViewReference;
    }
}