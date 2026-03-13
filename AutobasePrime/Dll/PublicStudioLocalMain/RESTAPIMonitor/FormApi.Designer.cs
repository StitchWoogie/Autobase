namespace AutobaseRESTAPIMonitor
{
    partial class FormApi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApi));
            this.dataGridView1 = new AutobaseRESTAPIMonitor.SafeDataGridView();
            this.btnTestParse = new System.Windows.Forms.Button();
            this.btnAddRule = new System.Windows.Forms.Button();
            this.btnDeleteRule = new System.Windows.Forms.Button();
            this.btnJsonPathHelp = new System.Windows.Forms.Button();
            this.txtJsonPath = new System.Windows.Forms.TextBox();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.labelJsonPath = new System.Windows.Forms.Label();
            this.labelTag = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.btnSelectTag = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.numericInterval = new System.Windows.Forms.NumericUpDown();
            this.labelInterval = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUrlCopy = new System.Windows.Forms.Button();
            this.btnSetHeaders = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUrlPreview = new System.Windows.Forms.TextBox();
            this.checkBoxUse = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chkUseNotFound = new System.Windows.Forms.CheckBox();
            this.txtNotFoundText = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericInterval)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.DarkGray;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 34;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.toolTip1.SetToolTip(this.dataGridView1, resources.GetString("dataGridView1.ToolTip"));
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            // 
            // btnTestParse
            // 
            resources.ApplyResources(this.btnTestParse, "btnTestParse");
            this.btnTestParse.Name = "btnTestParse";
            this.toolTip1.SetToolTip(this.btnTestParse, resources.GetString("btnTestParse.ToolTip"));
            this.btnTestParse.UseVisualStyleBackColor = true;
            this.btnTestParse.Click += new System.EventHandler(this.btnTestParse_Click);
            // 
            // btnAddRule
            // 
            resources.ApplyResources(this.btnAddRule, "btnAddRule");
            this.btnAddRule.Name = "btnAddRule";
            this.toolTip1.SetToolTip(this.btnAddRule, resources.GetString("btnAddRule.ToolTip"));
            this.btnAddRule.UseVisualStyleBackColor = true;
            this.btnAddRule.Click += new System.EventHandler(this.btnAddRule_Click);
            // 
            // btnDeleteRule
            // 
            resources.ApplyResources(this.btnDeleteRule, "btnDeleteRule");
            this.btnDeleteRule.Name = "btnDeleteRule";
            this.toolTip1.SetToolTip(this.btnDeleteRule, resources.GetString("btnDeleteRule.ToolTip"));
            this.btnDeleteRule.UseVisualStyleBackColor = true;
            this.btnDeleteRule.Click += new System.EventHandler(this.btnDeleteRule_Click);
            // 
            // btnJsonPathHelp
            // 
            resources.ApplyResources(this.btnJsonPathHelp, "btnJsonPathHelp");
            this.btnJsonPathHelp.Name = "btnJsonPathHelp";
            this.toolTip1.SetToolTip(this.btnJsonPathHelp, resources.GetString("btnJsonPathHelp.ToolTip"));
            this.btnJsonPathHelp.UseVisualStyleBackColor = true;
            this.btnJsonPathHelp.Click += new System.EventHandler(this.btnJsonPathHelp_Click);
            // 
            // txtJsonPath
            // 
            resources.ApplyResources(this.txtJsonPath, "txtJsonPath");
            this.txtJsonPath.Name = "txtJsonPath";
            this.toolTip1.SetToolTip(this.txtJsonPath, resources.GetString("txtJsonPath.ToolTip"));
            // 
            // txtTag
            // 
            resources.ApplyResources(this.txtTag, "txtTag");
            this.txtTag.Name = "txtTag";
            this.toolTip1.SetToolTip(this.txtTag, resources.GetString("txtTag.ToolTip"));
            // 
            // labelJsonPath
            // 
            resources.ApplyResources(this.labelJsonPath, "labelJsonPath");
            this.labelJsonPath.Name = "labelJsonPath";
            this.toolTip1.SetToolTip(this.labelJsonPath, resources.GetString("labelJsonPath.ToolTip"));
            // 
            // labelTag
            // 
            resources.ApplyResources(this.labelTag, "labelTag");
            this.labelTag.Name = "labelTag";
            this.toolTip1.SetToolTip(this.labelTag, resources.GetString("labelTag.ToolTip"));
            // 
            // txtUrl
            // 
            resources.ApplyResources(this.txtUrl, "txtUrl");
            this.txtUrl.Name = "txtUrl";
            this.toolTip1.SetToolTip(this.txtUrl, resources.GetString("txtUrl.ToolTip"));
            this.txtUrl.TextChanged += new System.EventHandler(this.txtUrl_TextChanged);
            // 
            // labelUrl
            // 
            resources.ApplyResources(this.labelUrl, "labelUrl");
            this.labelUrl.Name = "labelUrl";
            this.toolTip1.SetToolTip(this.labelUrl, resources.GetString("labelUrl.ToolTip"));
            // 
            // btnSelectTag
            // 
            resources.ApplyResources(this.btnSelectTag, "btnSelectTag");
            this.btnSelectTag.Name = "btnSelectTag";
            this.toolTip1.SetToolTip(this.btnSelectTag, resources.GetString("btnSelectTag.ToolTip"));
            this.btnSelectTag.UseVisualStyleBackColor = true;
            this.btnSelectTag.Click += new System.EventHandler(this.btnSelectTag_Click);
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.Name = "labelTitle";
            this.toolTip1.SetToolTip(this.labelTitle, resources.GetString("labelTitle.ToolTip"));
            // 
            // txtTitle
            // 
            resources.ApplyResources(this.txtTitle, "txtTitle");
            this.txtTitle.Name = "txtTitle";
            this.toolTip1.SetToolTip(this.txtTitle, resources.GetString("txtTitle.ToolTip"));
            this.txtTitle.MouseHover += new System.EventHandler(this.txtTitle_MouseHover);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.toolTip1.SetToolTip(this.btnOK, resources.GetString("btnOK.ToolTip"));
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // numericInterval
            // 
            resources.ApplyResources(this.numericInterval, "numericInterval");
            this.numericInterval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericInterval.Name = "numericInterval";
            this.toolTip1.SetToolTip(this.numericInterval, resources.GetString("numericInterval.ToolTip"));
            this.numericInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelInterval
            // 
            resources.ApplyResources(this.labelInterval, "labelInterval");
            this.labelInterval.Name = "labelInterval";
            this.toolTip1.SetToolTip(this.labelInterval, resources.GetString("labelInterval.ToolTip"));
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnUrlCopy);
            this.panel1.Controls.Add(this.btnSetHeaders);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtUrlPreview);
            this.panel1.Controls.Add(this.checkBoxUse);
            this.panel1.Controls.Add(this.labelInterval);
            this.panel1.Controls.Add(this.numericInterval);
            this.panel1.Controls.Add(this.txtTitle);
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Controls.Add(this.labelUrl);
            this.panel1.Controls.Add(this.txtUrl);
            this.panel1.Controls.Add(this.btnTestParse);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // btnUrlCopy
            // 
            resources.ApplyResources(this.btnUrlCopy, "btnUrlCopy");
            this.btnUrlCopy.Name = "btnUrlCopy";
            this.toolTip1.SetToolTip(this.btnUrlCopy, resources.GetString("btnUrlCopy.ToolTip"));
            this.btnUrlCopy.UseVisualStyleBackColor = true;
            this.btnUrlCopy.Click += new System.EventHandler(this.btnUrlCopy_Click);
            // 
            // btnSetHeaders
            // 
            resources.ApplyResources(this.btnSetHeaders, "btnSetHeaders");
            this.btnSetHeaders.Name = "btnSetHeaders";
            this.toolTip1.SetToolTip(this.btnSetHeaders, resources.GetString("btnSetHeaders.ToolTip"));
            this.btnSetHeaders.UseVisualStyleBackColor = true;
            this.btnSetHeaders.Click += new System.EventHandler(this.btnSetHeaders_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // txtUrlPreview
            // 
            resources.ApplyResources(this.txtUrlPreview, "txtUrlPreview");
            this.txtUrlPreview.Name = "txtUrlPreview";
            this.txtUrlPreview.ReadOnly = true;
            this.toolTip1.SetToolTip(this.txtUrlPreview, resources.GetString("txtUrlPreview.ToolTip"));
            // 
            // checkBoxUse
            // 
            resources.ApplyResources(this.checkBoxUse, "checkBoxUse");
            this.checkBoxUse.Name = "checkBoxUse";
            this.toolTip1.SetToolTip(this.checkBoxUse, resources.GetString("checkBoxUse.ToolTip"));
            this.checkBoxUse.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Name = "panel2";
            this.toolTip1.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            this.toolTip1.SetToolTip(this.panel5, resources.GetString("panel5.ToolTip"));
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            this.toolTip1.SetToolTip(this.panel4, resources.GetString("panel4.ToolTip"));
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.chkUseNotFound);
            this.panel3.Controls.Add(this.txtNotFoundText);
            this.panel3.Controls.Add(this.txtTag);
            this.panel3.Controls.Add(this.txtJsonPath);
            this.panel3.Controls.Add(this.btnDeleteRule);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnAddRule);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Controls.Add(this.labelJsonPath);
            this.panel3.Controls.Add(this.btnJsonPathHelp);
            this.panel3.Controls.Add(this.btnSelectTag);
            this.panel3.Controls.Add(this.labelTag);
            this.panel3.Name = "panel3";
            this.toolTip1.SetToolTip(this.panel3, resources.GetString("panel3.ToolTip"));
            // 
            // chkUseNotFound
            // 
            resources.ApplyResources(this.chkUseNotFound, "chkUseNotFound");
            this.chkUseNotFound.Name = "chkUseNotFound";
            this.toolTip1.SetToolTip(this.chkUseNotFound, resources.GetString("chkUseNotFound.ToolTip"));
            this.chkUseNotFound.UseVisualStyleBackColor = true;
            this.chkUseNotFound.CheckedChanged += new System.EventHandler(this.chkUseNotFound_CheckedChanged);
            // 
            // txtNotFoundText
            // 
            resources.ApplyResources(this.txtNotFoundText, "txtNotFoundText");
            this.txtNotFoundText.Name = "txtNotFoundText";
            this.toolTip1.SetToolTip(this.txtNotFoundText, resources.GetString("txtNotFoundText.ToolTip"));
            // 
            // FormApi
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "FormApi";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormApi_FormClosing);
            this.Click += new System.EventHandler(this.FormApi_Click);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericInterval)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AutobaseRESTAPIMonitor.SafeDataGridView dataGridView1;
        private System.Windows.Forms.Button btnTestParse;
        private System.Windows.Forms.Button btnAddRule;
        private System.Windows.Forms.Button btnDeleteRule;
        private System.Windows.Forms.Button btnJsonPathHelp;
        private System.Windows.Forms.TextBox txtJsonPath;
        private System.Windows.Forms.TextBox txtTag;
        private System.Windows.Forms.Label labelJsonPath;
        private System.Windows.Forms.Label labelTag;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Button btnSelectTag;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown numericInterval;
        private System.Windows.Forms.Label labelInterval;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBoxUse;
        private System.Windows.Forms.TextBox txtNotFoundText;
        private System.Windows.Forms.CheckBox chkUseNotFound;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUrlPreview;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnSetHeaders;
        private System.Windows.Forms.Button btnUrlCopy;
    }
}