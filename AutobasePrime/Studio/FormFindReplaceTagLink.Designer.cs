namespace Studio
{
    partial class FormFindReplaceTagLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFindReplaceTagLink));
            this.listViewTag = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonTag = new System.Windows.Forms.Button();
            this.checkBoxReplaceOptionWholeWord = new System.Windows.Forms.CheckBox();
            this.checkBoxFindOptionMatchCase = new System.Windows.Forms.CheckBox();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.textBoxReplace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLookin = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.listViewUsed = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTag
            // 
            this.listViewTag.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewTag.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.listViewTag, "listViewTag");
            this.listViewTag.FullRowSelect = true;
            this.listViewTag.HideSelection = false;
            this.listViewTag.Name = "listViewTag";
            this.listViewTag.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewTag.UseCompatibleStateImageBehavior = false;
            this.listViewTag.View = System.Windows.Forms.View.Details;
            this.listViewTag.SelectedIndexChanged += new System.EventHandler(this.listViewTag_SelectedIndexChanged);
            this.listViewTag.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewTag_MouseDoubleClick);
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
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.copyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonTag);
            this.groupBox2.Controls.Add(this.checkBoxReplaceOptionWholeWord);
            this.groupBox2.Controls.Add(this.checkBoxFindOptionMatchCase);
            this.groupBox2.Controls.Add(this.buttonReplace);
            this.groupBox2.Controls.Add(this.buttonFindNext);
            this.groupBox2.Controls.Add(this.textBoxReplace);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBoxFind);
            this.groupBox2.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonTag
            // 
            resources.ApplyResources(this.buttonTag, "buttonTag");
            this.buttonTag.Name = "buttonTag";
            this.buttonTag.UseVisualStyleBackColor = true;
            this.buttonTag.Click += new System.EventHandler(this.buttonTag_Click);
            // 
            // checkBoxReplaceOptionWholeWord
            // 
            resources.ApplyResources(this.checkBoxReplaceOptionWholeWord, "checkBoxReplaceOptionWholeWord");
            this.checkBoxReplaceOptionWholeWord.Name = "checkBoxReplaceOptionWholeWord";
            this.checkBoxReplaceOptionWholeWord.UseVisualStyleBackColor = true;
            // 
            // checkBoxFindOptionMatchCase
            // 
            resources.ApplyResources(this.checkBoxFindOptionMatchCase, "checkBoxFindOptionMatchCase");
            this.checkBoxFindOptionMatchCase.Name = "checkBoxFindOptionMatchCase";
            this.checkBoxFindOptionMatchCase.UseVisualStyleBackColor = true;
            // 
            // buttonReplace
            // 
            resources.ApplyResources(this.buttonReplace, "buttonReplace");
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonFindNext
            // 
            resources.ApplyResources(this.buttonFindNext, "buttonFindNext");
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // textBoxReplace
            // 
            resources.ApplyResources(this.textBoxReplace, "textBoxReplace");
            this.textBoxReplace.Name = "textBoxReplace";
            this.textBoxReplace.TextChanged += new System.EventHandler(this.textBoxReplace_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxFind
            // 
            resources.ApplyResources(this.textBoxFind, "textBoxFind");
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxLookin
            // 
            this.comboBoxLookin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLookin.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxLookin, "comboBoxLookin");
            this.comboBoxLookin.Name = "comboBoxLookin";
            this.comboBoxLookin.SelectedIndexChanged += new System.EventHandler(this.comboBoxLookin_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // listViewUsed
            // 
            this.listViewUsed.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader7});
            resources.ApplyResources(this.listViewUsed, "listViewUsed");
            this.listViewUsed.FullRowSelect = true;
            this.listViewUsed.HideSelection = false;
            this.listViewUsed.Name = "listViewUsed";
            this.listViewUsed.UseCompatibleStateImageBehavior = false;
            this.listViewUsed.View = System.Windows.Forms.View.Details;
            this.listViewUsed.SelectedIndexChanged += new System.EventHandler(this.listViewUsed_SelectedIndexChanged);
            this.listViewUsed.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewUsed_MouseDoubleClick);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.progressBar1);
            this.panel2.Controls.Add(this.cancelButton);
            this.panel2.Controls.Add(this.buttonRefresh);
            this.panel2.Controls.Add(this.comboBoxLookin);
            this.panel2.Controls.Add(this.label3);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // buttonRefresh
            // 
            resources.ApplyResources(this.buttonRefresh, "buttonRefresh");
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.listViewUsed);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.listViewTag);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // panel7
            // 
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // FormFindReplaceTagLink
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormFindReplaceTagLink";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFindReplaceTagLink_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormFindReplaceTagLink_FormClosed);
            this.Load += new System.EventHandler(this.FormFindReplaceTagLink_Load);
            this.SizeChanged += new System.EventHandler(this.FormFindReplaceTagLink_SizeChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewTag;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonFindNext;
        private System.Windows.Forms.TextBox textBoxReplace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFind;
        private System.Windows.Forms.CheckBox checkBoxFindOptionMatchCase;
        private System.Windows.Forms.CheckBox checkBoxReplaceOptionWholeWord;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxLookin;
        private System.Windows.Forms.Button buttonTag;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListView listViewUsed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}