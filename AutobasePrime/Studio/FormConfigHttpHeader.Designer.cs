namespace Studio
{
    partial class FormConfigHttpHeader
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
            this.listView = new System.Windows.Forms.ListView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.lbName = new System.Windows.Forms.Label();
            this.lbValue = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtApiName = new System.Windows.Forms.TextBox();
            this.lbApiName = new System.Windows.Forms.Label();
            this.cmbHeaderName = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnWarnigs = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(570, 265);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(274, 63);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 42);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(11, 84);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(249, 21);
            this.txtValue.TabIndex = 3;
            // 
            // chkEncrypt
            // 
            this.chkEncrypt.AutoSize = true;
            this.chkEncrypt.Location = new System.Drawing.Point(274, 33);
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.Size = new System.Drawing.Size(110, 16);
            this.chkEncrypt.TabIndex = 4;
            this.chkEncrypt.Text = "Use Encryption";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(9, 6);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(83, 12);
            this.lbName.TabIndex = 5;
            this.lbName.Text = "Header Name";
            // 
            // lbValue
            // 
            this.lbValue.AutoSize = true;
            this.lbValue.Location = new System.Drawing.Point(9, 63);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(81, 12);
            this.lbValue.TabIndex = 6;
            this.lbValue.Text = "Header Value";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(504, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 36);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(431, 11);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(148, 40);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Delete Selection";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(423, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 36);
            this.btnLoad.TabIndex = 10;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtApiName
            // 
            this.txtApiName.Location = new System.Drawing.Point(11, 27);
            this.txtApiName.Name = "txtApiName";
            this.txtApiName.Size = new System.Drawing.Size(278, 21);
            this.txtApiName.TabIndex = 11;
            // 
            // lbApiName
            // 
            this.lbApiName.AutoSize = true;
            this.lbApiName.Location = new System.Drawing.Point(12, 9);
            this.lbApiName.Name = "lbApiName";
            this.lbApiName.Size = new System.Drawing.Size(141, 12);
            this.lbApiName.TabIndex = 12;
            this.lbApiName.Text = "Header Config Filename";
            // 
            // cmbHeaderName
            // 
            this.cmbHeaderName.FormattingEnabled = true;
            this.cmbHeaderName.Location = new System.Drawing.Point(11, 31);
            this.cmbHeaderName.Name = "cmbHeaderName";
            this.cmbHeaderName.Size = new System.Drawing.Size(249, 20);
            this.cmbHeaderName.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbApiName);
            this.panel1.Controls.Add(this.txtApiName);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(591, 59);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnWarnigs);
            this.panel2.Controls.Add(this.cmbHeaderName);
            this.panel2.Controls.Add(this.lbValue);
            this.panel2.Controls.Add(this.lbName);
            this.panel2.Controls.Add(this.chkEncrypt);
            this.panel2.Controls.Add(this.txtValue);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.btnAdd);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 324);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(591, 117);
            this.panel2.TabIndex = 15;
            // 
            // btnWarnigs
            // 
            this.btnWarnigs.Location = new System.Drawing.Point(431, 63);
            this.btnWarnigs.Name = "btnWarnigs";
            this.btnWarnigs.Size = new System.Drawing.Size(148, 40);
            this.btnWarnigs.TabIndex = 13;
            this.btnWarnigs.Text = "View Warnings";
            this.btnWarnigs.UseVisualStyleBackColor = true;
            this.btnWarnigs.Click += new System.EventHandler(this.btnWarnigs_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 59);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(11, 265);
            this.panel3.TabIndex = 16;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(581, 59);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(10, 265);
            this.panel4.TabIndex = 17;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.listView);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(11, 59);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(570, 265);
            this.panel5.TabIndex = 18;
            // 
            // FormConfigHttpHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 441);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormConfigHttpHeader";
            this.Text = "HTTP Header Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigHttpHeader_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbValue;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtApiName;
        private System.Windows.Forms.Label lbApiName;
        private System.Windows.Forms.ComboBox cmbHeaderName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnWarnigs;
        private System.Windows.Forms.Panel panel5;
    }
}