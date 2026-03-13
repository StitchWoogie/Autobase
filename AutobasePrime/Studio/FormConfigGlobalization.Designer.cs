using System.Drawing;
using System.Windows.Forms;

namespace Studio
{
    partial class FormConfigGlobalization
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvLanguageTable;
        private ToolStrip toolStrip;
        private ToolStripButton btnAdd;
        private ToolStripButton btnDelete;
        private ToolStripButton btnAddColumn;
        private ToolStripButton btnDeleteColumn;
        private ToolStripSeparator separator1;
        private ToolStripSeparator separator2;
        private ToolStripButton btnSave;
        private ToolStripButton btnRefresh;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigGlobalization));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddColumn = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteColumn = new System.Windows.Forms.ToolStripButton();
            this.separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.dgvLanguageTable = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanguageTable)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.separator2,
            this.btnAdd,
            this.btnDelete,
            this.separator1,
            this.btnAddColumn,
            this.btnDeleteColumn,
            this.separator3,
            this.btnSave,
            this.btnRefresh});
            this.toolStrip.Name = "toolStrip";
            // 
            // toolStripDropDownButton1
            // 
            resources.ApplyResources(this.toolStripDropDownButton1, "toolStripDropDownButton1");
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importCSVToolStripMenuItem,
            this.exportCSVToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            // 
            // importCSVToolStripMenuItem
            // 
            resources.ApplyResources(this.importCSVToolStripMenuItem, "importCSVToolStripMenuItem");
            this.importCSVToolStripMenuItem.Name = "importCSVToolStripMenuItem";
            this.importCSVToolStripMenuItem.Click += new System.EventHandler(this.importCSVToolStripMenuItem_Click);
            // 
            // exportCSVToolStripMenuItem
            // 
            resources.ApplyResources(this.exportCSVToolStripMenuItem, "exportCSVToolStripMenuItem");
            this.exportCSVToolStripMenuItem.Name = "exportCSVToolStripMenuItem";
            this.exportCSVToolStripMenuItem.Click += new System.EventHandler(this.exportCSVToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // separator2
            // 
            resources.ApplyResources(this.separator2, "separator2");
            this.separator2.Name = "separator2";
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // separator1
            // 
            resources.ApplyResources(this.separator1, "separator1");
            this.separator1.Name = "separator1";
            // 
            // btnAddColumn
            // 
            resources.ApplyResources(this.btnAddColumn, "btnAddColumn");
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Click += new System.EventHandler(this.BtnAddColumn_Click);
            // 
            // btnDeleteColumn
            // 
            resources.ApplyResources(this.btnDeleteColumn, "btnDeleteColumn");
            this.btnDeleteColumn.Name = "btnDeleteColumn";
            this.btnDeleteColumn.Click += new System.EventHandler(this.BtnDeleteColumn_Click);
            // 
            // separator3
            // 
            resources.ApplyResources(this.separator3, "separator3");
            this.separator3.Name = "separator3";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // dgvLanguageTable
            // 
            resources.ApplyResources(this.dgvLanguageTable, "dgvLanguageTable");
            this.dgvLanguageTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLanguageTable.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvLanguageTable.Name = "dgvLanguageTable";
            this.dgvLanguageTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLanguageTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLanguageTable_CellValueChanged);
            this.dgvLanguageTable.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvLanguageTable_RowsAdded);
            this.dgvLanguageTable.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvLanguageTable_UserDeletedRow);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.dgvLanguageTable);
            this.panel1.Name = "panel1";
            // 
            // FormConfigGlobalization
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip);
            this.Name = "FormConfigGlobalization";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigGlobalization_FormClosing);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanguageTable)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStripSeparator separator3;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem importCSVToolStripMenuItem;
        private ToolStripMenuItem exportCSVToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private Panel panel1;
        private ToolStripMenuItem exitToolStripMenuItem;
    }
}