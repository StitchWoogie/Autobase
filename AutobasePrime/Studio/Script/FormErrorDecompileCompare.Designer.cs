namespace Studio.Script
{
    partial class FormErrorDecompileCompare
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.userControlScriptEditor1 = new ScriptLibEdit.Editor.UserControlScriptEditor();
            this.userControlScriptEditor2 = new ScriptLibEdit.Editor.UserControlScriptEditor();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.userControlScriptEditor1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.userControlScriptEditor2);
            this.splitContainer1.Size = new System.Drawing.Size(721, 465);
            this.splitContainer1.SplitterDistance = 377;
            this.splitContainer1.TabIndex = 0;
            // 
            // userControlScriptEditor1
            // 
            this.userControlScriptEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlScriptEditor1.Location = new System.Drawing.Point(0, 0);
            this.userControlScriptEditor1.Name = "userControlScriptEditor1";
            this.userControlScriptEditor1.Size = new System.Drawing.Size(377, 465);
            this.userControlScriptEditor1.TabIndex = 0;
            // 
            // userControlScriptEditor2
            // 
            this.userControlScriptEditor2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlScriptEditor2.Location = new System.Drawing.Point(0, 0);
            this.userControlScriptEditor2.Name = "userControlScriptEditor2";
            this.userControlScriptEditor2.Size = new System.Drawing.Size(340, 465);
            this.userControlScriptEditor2.TabIndex = 1;
            // 
            // FormErrorDecompileCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 465);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormErrorDecompileCompare";
            this.Text = "FormErrorDecompileCompare";
            this.Load += new System.EventHandler(this.FormErrorDecompileCompare_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ScriptLibEdit.Editor.UserControlScriptEditor userControlScriptEditor1;
        private ScriptLibEdit.Editor.UserControlScriptEditor userControlScriptEditor2;

    }
}