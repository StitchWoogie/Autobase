namespace AutobaseRESTAPIMonitor
{
    partial class FormRequestHeader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRequestHeader));
            this.dgvHeaders = new System.Windows.Forms.DataGridView();
            this.cboHeaderName = new System.Windows.Forms.ComboBox();
            this.txtHeaderValue = new System.Windows.Forms.TextBox();
            this.btnAddOrUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvHeaders
            // 
            resources.ApplyResources(this.dgvHeaders, "dgvHeaders");
            this.dgvHeaders.AllowUserToAddRows = false;
            this.dgvHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHeaders.Name = "dgvHeaders";
            this.dgvHeaders.ReadOnly = true;
            this.dgvHeaders.RowTemplate.Height = 34;
            this.dgvHeaders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHeaders.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHeaders_CellClick);
            // 
            // cboHeaderName
            // 
            resources.ApplyResources(this.cboHeaderName, "cboHeaderName");
            this.cboHeaderName.FormattingEnabled = true;
            this.cboHeaderName.Name = "cboHeaderName";
            // 
            // txtHeaderValue
            // 
            resources.ApplyResources(this.txtHeaderValue, "txtHeaderValue");
            this.txtHeaderValue.Name = "txtHeaderValue";
            // 
            // btnAddOrUpdate
            // 
            resources.ApplyResources(this.btnAddOrUpdate, "btnAddOrUpdate");
            this.btnAddOrUpdate.Name = "btnAddOrUpdate";
            this.btnAddOrUpdate.UseVisualStyleBackColor = true;
            this.btnAddOrUpdate.Click += new System.EventHandler(this.btnAddOrUpdate_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkEncrypt
            // 
            resources.ApplyResources(this.chkEncrypt, "chkEncrypt");
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.dgvHeaders);
            this.panel3.Name = "panel3";
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
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.btnDelete);
            this.panel4.Controls.Add(this.chkEncrypt);
            this.panel4.Controls.Add(this.btnAddOrUpdate);
            this.panel4.Controls.Add(this.txtHeaderValue);
            this.panel4.Controls.Add(this.cboHeaderName);
            this.panel4.Name = "panel4";
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Controls.Add(this.btnCancel);
            this.panel5.Controls.Add(this.btnOK);
            this.panel5.Name = "panel5";
            // 
            // FormRequestHeader
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormRequestHeader";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHeaders;
        private System.Windows.Forms.ComboBox cboHeaderName;
        private System.Windows.Forms.TextBox txtHeaderValue;
        private System.Windows.Forms.Button btnAddOrUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
    }
}