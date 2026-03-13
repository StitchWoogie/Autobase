namespace Studio
{
    partial class FormConfigJsonTemplate
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
            this.btnTag = new System.Windows.Forms.Button();
            this.rtbEditor = new System.Windows.Forms.RichTextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnFormat = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lbTemplateID = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtTemplateId = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTag
            // 
            this.btnTag.Location = new System.Drawing.Point(489, 37);
            this.btnTag.Name = "btnTag";
            this.btnTag.Size = new System.Drawing.Size(82, 23);
            this.btnTag.TabIndex = 7;
            this.btnTag.Text = "Tag";
            this.btnTag.UseVisualStyleBackColor = true;
            this.btnTag.Click += new System.EventHandler(this.ButtonTag_Click);
            // 
            // rtbEditor
            // 
            this.rtbEditor.AcceptsTab = true;
            this.rtbEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEditor.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbEditor.Location = new System.Drawing.Point(10, 70);
            this.rtbEditor.Name = "rtbEditor";
            this.rtbEditor.Size = new System.Drawing.Size(564, 384);
            this.rtbEditor.TabIndex = 6;
            this.rtbEditor.Text = "";
            this.rtbEditor.WordWrap = false;
            this.rtbEditor.SelectionChanged += new System.EventHandler(this.rtbEditor_SelectionChanged);
            this.rtbEditor.TextChanged += new System.EventHandler(this.RtbEditor_TextChanged);
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(574, 70);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(10, 384);
            this.panel4.TabIndex = 9;
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(395, 37);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(82, 23);
            this.btnFormat.TabIndex = 5;
            this.btnFormat.Text = "Sort ";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.BtnFormat_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Location = new System.Drawing.Point(299, 37);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(82, 23);
            this.btnValidate.TabIndex = 4;
            this.btnValidate.Text = "Validation";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.BtnValidate_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 70);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(10, 384);
            this.panel3.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(395, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(299, 8);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(82, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // lbTemplateID
            // 
            this.lbTemplateID.AutoSize = true;
            this.lbTemplateID.Location = new System.Drawing.Point(9, 14);
            this.lbTemplateID.Name = "lbTemplateID";
            this.lbTemplateID.Size = new System.Drawing.Size(73, 12);
            this.lbTemplateID.TabIndex = 0;
            this.lbTemplateID.Text = "Template ID";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 454);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(584, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusBar";
            // 
            // statusBar
            // 
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(0, 17);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnTag);
            this.panel2.Controls.Add(this.btnFormat);
            this.panel2.Controls.Add(this.btnValidate);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnLoad);
            this.panel2.Controls.Add(this.txtTemplateId);
            this.panel2.Controls.Add(this.lbTemplateID);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(584, 70);
            this.panel2.TabIndex = 8;
            // 
            // txtTemplateId
            // 
            this.txtTemplateId.Location = new System.Drawing.Point(88, 10);
            this.txtTemplateId.Name = "txtTemplateId";
            this.txtTemplateId.Size = new System.Drawing.Size(185, 21);
            this.txtTemplateId.TabIndex = 1;
            // 
            // FormConfigJsonTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 476);
            this.Controls.Add(this.rtbEditor);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel2);
            this.Name = "FormConfigJsonTemplate";
            this.Text = "JSON Template Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigJson_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTag;
        private System.Windows.Forms.RichTextBox rtbEditor;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lbTemplateID;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBar;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtTemplateId;
    }
}