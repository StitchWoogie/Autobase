namespace OPCUA.Client.UI
{
    partial class FormAddItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddItem));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxManual = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtManualId = new System.Windows.Forms.TextBox();
            this.txtNodeId = new System.Windows.Forms.TextBox();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxShortForm = new System.Windows.Forms.CheckBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.treeViewOpc = new System.Windows.Forms.TreeView();
            this.listViewItems = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item Node ID manually :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Variable Item Node ID :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Item Access Name :";
            // 
            // checkBoxManual
            // 
            this.checkBoxManual.AutoSize = true;
            this.checkBoxManual.Location = new System.Drawing.Point(178, 7);
            this.checkBoxManual.Name = "checkBoxManual";
            this.checkBoxManual.Size = new System.Drawing.Size(139, 16);
            this.checkBoxManual.TabIndex = 1;
            this.checkBoxManual.Text = "Insert Item Manually";
            this.checkBoxManual.UseVisualStyleBackColor = true;
            this.checkBoxManual.CheckedChanged += new System.EventHandler(this.checkBoxManual_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Item List to be added";
            // 
            // txtManualId
            // 
            this.txtManualId.Location = new System.Drawing.Point(178, 41);
            this.txtManualId.Name = "txtManualId";
            this.txtManualId.Size = new System.Drawing.Size(257, 21);
            this.txtManualId.TabIndex = 2;
            // 
            // txtNodeId
            // 
            this.txtNodeId.Location = new System.Drawing.Point(178, 75);
            this.txtNodeId.Name = "txtNodeId";
            this.txtNodeId.Size = new System.Drawing.Size(257, 21);
            this.txtNodeId.TabIndex = 2;
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(178, 109);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(257, 21);
            this.txtAlias.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Item Browser for the Server";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(450, 41);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(450, 72);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxShortForm
            // 
            this.checkBoxShortForm.AutoSize = true;
            this.checkBoxShortForm.Location = new System.Drawing.Point(450, 114);
            this.checkBoxShortForm.Name = "checkBoxShortForm";
            this.checkBoxShortForm.Size = new System.Drawing.Size(86, 16);
            this.checkBoxShortForm.TabIndex = 1;
            this.checkBoxShortForm.Text = "Short From";
            this.checkBoxShortForm.UseVisualStyleBackColor = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(445, 10);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // treeViewOpc
            // 
            this.treeViewOpc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewOpc.Location = new System.Drawing.Point(5, 46);
            this.treeViewOpc.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewOpc.Name = "treeViewOpc";
            this.treeViewOpc.Size = new System.Drawing.Size(552, 257);
            this.treeViewOpc.TabIndex = 5;
            // 
            // listViewItems
            // 
            this.listViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewItems.HideSelection = false;
            this.listViewItems.Location = new System.Drawing.Point(5, 46);
            this.listViewItems.Margin = new System.Windows.Forms.Padding(0);
            this.listViewItems.Name = "listViewItems";
            this.listViewItems.Size = new System.Drawing.Size(552, 90);
            this.listViewItems.TabIndex = 6;
            this.listViewItems.UseCompatibleStateImageBehavior = false;
            this.listViewItems.View = System.Windows.Forms.View.Details;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.txtAlias);
            this.panel1.Controls.Add(this.txtNodeId);
            this.panel1.Controls.Add(this.txtManualId);
            this.panel1.Controls.Add(this.checkBoxShortForm);
            this.panel1.Controls.Add(this.checkBoxManual);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(564, 146);
            this.panel1.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(10, 164);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.listViewItems);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.treeViewOpc);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainer1.Size = new System.Drawing.Size(564, 455);
            this.splitContainer1.SplitterDistance = 143;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.buttonDelete);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(552, 41);
            this.panel2.TabIndex = 7;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(445, 9);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.statusLabel);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.buttonAdd);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(5, 5);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(552, 41);
            this.panel3.TabIndex = 6;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(220, 15);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 12);
            this.statusLabel.TabIndex = 5;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(10, 156);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(564, 8);
            this.panel4.TabIndex = 9;
            // 
            // FormAddItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(584, 629);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 480);
            this.Name = "FormAddItem";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Item";
            this.Load += new System.EventHandler(this.FormAddItem_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxManual;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtManualId;
        private System.Windows.Forms.TextBox txtNodeId;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxShortForm;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.TreeView treeViewOpc;
        private System.Windows.Forms.ListView listViewItems;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Label statusLabel;
    }
}