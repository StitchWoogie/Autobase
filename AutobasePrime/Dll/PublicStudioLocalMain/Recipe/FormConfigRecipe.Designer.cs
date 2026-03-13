using System.Drawing;
using System.Windows.Forms;

namespace PublicStudioLocalMain.Recipe
{
	partial class FormConfigRecipe
	{
		private System.ComponentModel.IContainer components = null;

		private SplitContainer splitContainer1;
		private ListView listViewRecipes;
		private ColumnHeader colName;
		private ColumnHeader colVersion;
		private ColumnHeader colDescription;
		private TreeView treeViewStructure;
		private ToolStrip toolStrip1;
		private ToolStripButton btnAddRecipe;
		private ToolStripButton btnDeleteRecipe;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripButton btnAddUnit;
		private ToolStripButton btnDeleteUnit;
		private ToolStripSeparator toolStripSeparatorUnit;
		private ToolStripButton btnAddStep;
		private ToolStripButton btnDeleteStep;
		private ToolStripButton btnEditStep;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripButton btnExportCsv;
		private ToolStripButton btnImportCsv;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripButton btnSave;
		private TextBox textBoxRecipeName;
		private TextBox textBoxDescription;
		private Label labelRecipeName;
		private Label labelDescription;
		private Label labelRecipeMode;
		private ComboBox comboBoxRecipeMode;
		private Label labelStatus;
		private Label labelStatusValue;
		private Panel panelRecipeInfo;

		// Step Preview (B-2)
		private SplitContainer splitTreeAndPreview;
		private Panel panelPreview;
		private Label labelPreviewTitle;
		private Label labelPreviewWait;
		private Label labelPreviewEntry;
		private Label labelPreviewExit;
		private Label labelPreviewRunning;
		private ListView listViewPreviewItems;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewRecipes = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeViewStructure = new System.Windows.Forms.TreeView();
            this.panelRecipeInfo = new System.Windows.Forms.Panel();
            this.labelRecipeName = new System.Windows.Forms.Label();
            this.textBoxRecipeName = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelRecipeMode = new System.Windows.Forms.Label();
            this.comboBoxRecipeMode = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStatusValue = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddRecipe = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteRecipe = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddUnit = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteUnit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparatorUnit = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddStep = new System.Windows.Forms.ToolStripButton();
            this.btnEditStep = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteStep = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportCsv = new System.Windows.Forms.ToolStripButton();
            this.btnImportCsv = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelRecipeInfo.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewRecipes);
            //
            // splitContainer1.Panel2 — Controls added after creation below
            //
            this.splitContainer1.Size = new System.Drawing.Size(950, 555);
            this.splitContainer1.SplitterDistance = 363;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewRecipes
            // 
            this.listViewRecipes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colVersion,
            this.colDescription});
            this.listViewRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRecipes.FullRowSelect = true;
            this.listViewRecipes.GridLines = true;
            this.listViewRecipes.HideSelection = false;
            this.listViewRecipes.Location = new System.Drawing.Point(0, 0);
            this.listViewRecipes.Name = "listViewRecipes";
            this.listViewRecipes.Size = new System.Drawing.Size(363, 555);
            this.listViewRecipes.TabIndex = 0;
            this.listViewRecipes.UseCompatibleStateImageBehavior = false;
            this.listViewRecipes.View = System.Windows.Forms.View.Details;
            this.listViewRecipes.SelectedIndexChanged += new System.EventHandler(this.listViewRecipes_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Width = 150;
            // 
            // colVersion
            // 
            this.colVersion.Width = 50;
            // 
            // colDescription
            // 
            this.colDescription.Width = 140;
            //
            // splitTreeAndPreview (B-2: TreeView + Preview)
            //
            this.splitTreeAndPreview = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitTreeAndPreview)).BeginInit();
            this.splitTreeAndPreview.SuspendLayout();
            this.splitTreeAndPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTreeAndPreview.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitTreeAndPreview.Location = new System.Drawing.Point(0, 100);
            this.splitTreeAndPreview.Name = "splitTreeAndPreview";
            this.splitTreeAndPreview.Size = new System.Drawing.Size(583, 455);
            this.splitTreeAndPreview.SplitterDistance = 270;
            this.splitTreeAndPreview.TabIndex = 2;
            //
            // treeViewStructure
            //
            this.treeViewStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewStructure.HideSelection = false;
            this.treeViewStructure.Location = new System.Drawing.Point(0, 0);
            this.treeViewStructure.Name = "treeViewStructure";
            this.treeViewStructure.Size = new System.Drawing.Size(583, 270);
            this.treeViewStructure.TabIndex = 0;
            this.treeViewStructure.DoubleClick += new System.EventHandler(this.treeViewStructure_DoubleClick);
            this.treeViewStructure.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewStructure_AfterSelect);
            //
            // panelPreview (B-2: Step Preview Panel)
            //
            this.panelPreview = new System.Windows.Forms.Panel();
            this.labelPreviewTitle = new System.Windows.Forms.Label();
            this.labelPreviewWait = new System.Windows.Forms.Label();
            this.labelPreviewEntry = new System.Windows.Forms.Label();
            this.labelPreviewExit = new System.Windows.Forms.Label();
            this.labelPreviewRunning = new System.Windows.Forms.Label();
            this.listViewPreviewItems = new System.Windows.Forms.ListView();
            this.panelPreview.SuspendLayout();
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.AutoScroll = true;
            this.panelPreview.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // labelPreviewTitle
            this.labelPreviewTitle.AutoSize = true;
            this.labelPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelPreviewTitle.Location = new System.Drawing.Point(8, 6);
            // labelPreviewWait
            this.labelPreviewWait.AutoSize = true;
            this.labelPreviewWait.Location = new System.Drawing.Point(8, 28);
            // labelPreviewEntry
            this.labelPreviewEntry.AutoSize = true;
            this.labelPreviewEntry.Location = new System.Drawing.Point(8, 48);
            // labelPreviewExit
            this.labelPreviewExit.AutoSize = true;
            this.labelPreviewExit.Location = new System.Drawing.Point(8, 68);
            // labelPreviewRunning
            this.labelPreviewRunning.AutoSize = true;
            this.labelPreviewRunning.Location = new System.Drawing.Point(8, 88);
            // listViewPreviewItems
            this.listViewPreviewItems.Location = new System.Drawing.Point(8, 108);
            this.listViewPreviewItems.Size = new System.Drawing.Size(560, 70);
            this.listViewPreviewItems.Dock = System.Windows.Forms.DockStyle.None;
            this.listViewPreviewItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.listViewPreviewItems.View = System.Windows.Forms.View.Details;
            this.listViewPreviewItems.FullRowSelect = true;
            this.listViewPreviewItems.GridLines = true;
            this.listViewPreviewItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPreviewItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.listViewPreviewItems.Columns.Add("Tag", 200);
            this.listViewPreviewItems.Columns.Add("Value", 120);
            this.listViewPreviewItems.Columns.Add("Type", 80);
            this.panelPreview.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.labelPreviewTitle, this.labelPreviewWait,
                this.labelPreviewEntry, this.labelPreviewExit,
                this.labelPreviewRunning, this.listViewPreviewItems
            });
            // Wire SplitContainer
            this.splitTreeAndPreview.Panel1.Controls.Add(this.treeViewStructure);
            this.splitTreeAndPreview.Panel2.Controls.Add(this.panelPreview);
            //
            // splitContainer1.Panel2 — add children now that they exist
            //
            this.splitContainer1.Panel2.Controls.Add(this.splitTreeAndPreview);
            this.splitContainer1.Panel2.Controls.Add(this.panelRecipeInfo);
            //
            // panelRecipeInfo
            // 
            this.panelRecipeInfo.Controls.Add(this.labelRecipeName);
            this.panelRecipeInfo.Controls.Add(this.textBoxRecipeName);
            this.panelRecipeInfo.Controls.Add(this.labelDescription);
            this.panelRecipeInfo.Controls.Add(this.textBoxDescription);
            this.panelRecipeInfo.Controls.Add(this.labelRecipeMode);
            this.panelRecipeInfo.Controls.Add(this.comboBoxRecipeMode);
            this.panelRecipeInfo.Controls.Add(this.labelStatus);
            this.panelRecipeInfo.Controls.Add(this.labelStatusValue);
            this.panelRecipeInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRecipeInfo.Location = new System.Drawing.Point(0, 0);
            this.panelRecipeInfo.Name = "panelRecipeInfo";
            this.panelRecipeInfo.Size = new System.Drawing.Size(583, 100);
            this.panelRecipeInfo.TabIndex = 1;
            // 
            // labelRecipeName
            // 
            this.labelRecipeName.AutoSize = true;
            this.labelRecipeName.Location = new System.Drawing.Point(5, 8);
            this.labelRecipeName.Name = "labelRecipeName";
            this.labelRecipeName.Size = new System.Drawing.Size(0, 12);
            this.labelRecipeName.TabIndex = 0;
            // 
            // textBoxRecipeName
            // 
            this.textBoxRecipeName.Location = new System.Drawing.Point(50, 5);
            this.textBoxRecipeName.Name = "textBoxRecipeName";
            this.textBoxRecipeName.Size = new System.Drawing.Size(300, 21);
            this.textBoxRecipeName.TabIndex = 1;
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(5, 38);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(0, 12);
            this.labelDescription.TabIndex = 2;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(50, 35);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(400, 21);
            this.textBoxDescription.TabIndex = 3;
            // 
            // labelRecipeMode
            // 
            this.labelRecipeMode.AutoSize = true;
            this.labelRecipeMode.Location = new System.Drawing.Point(5, 68);
            this.labelRecipeMode.Name = "labelRecipeMode";
            this.labelRecipeMode.Size = new System.Drawing.Size(0, 12);
            this.labelRecipeMode.TabIndex = 4;
            // 
            // comboBoxRecipeMode
            // 
            this.comboBoxRecipeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecipeMode.Items.AddRange(new object[] {
            "Standard",
            "Quick"});
            this.comboBoxRecipeMode.Location = new System.Drawing.Point(50, 65);
            this.comboBoxRecipeMode.Name = "comboBoxRecipeMode";
            this.comboBoxRecipeMode.Size = new System.Drawing.Size(120, 20);
            this.comboBoxRecipeMode.TabIndex = 5;
            this.comboBoxRecipeMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxRecipeMode_SelectedIndexChanged);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(200, 68);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 12);
            this.labelStatus.TabIndex = 6;
            // 
            // labelStatusValue
            // 
            this.labelStatusValue.AutoSize = true;
            this.labelStatusValue.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatusValue.Location = new System.Drawing.Point(250, 68);
            this.labelStatusValue.Name = "labelStatusValue";
            this.labelStatusValue.Size = new System.Drawing.Size(0, 12);
            this.labelStatusValue.TabIndex = 7;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddRecipe,
            this.btnDeleteRecipe,
            this.toolStripSeparator1,
            this.btnAddUnit,
            this.btnDeleteUnit,
            this.toolStripSeparatorUnit,
            this.btnAddStep,
            this.btnEditStep,
            this.btnDeleteStep,
            this.toolStripSeparator2,
            this.btnExportCsv,
            this.btnImportCsv,
            this.toolStripSeparator3,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(950, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // btnAddRecipe
            // 
            this.btnAddRecipe.Name = "btnAddRecipe";
            this.btnAddRecipe.Size = new System.Drawing.Size(23, 22);
            this.btnAddRecipe.Click += new System.EventHandler(this.btnAddRecipe_Click);
            // 
            // btnDeleteRecipe
            // 
            this.btnDeleteRecipe.Name = "btnDeleteRecipe";
            this.btnDeleteRecipe.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteRecipe.Click += new System.EventHandler(this.btnDeleteRecipe_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddUnit
            // 
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(23, 22);
            this.btnAddUnit.Click += new System.EventHandler(this.btnAddUnit_Click);
            // 
            // btnDeleteUnit
            // 
            this.btnDeleteUnit.Name = "btnDeleteUnit";
            this.btnDeleteUnit.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteUnit.Click += new System.EventHandler(this.btnDeleteUnit_Click);
            // 
            // toolStripSeparatorUnit
            // 
            this.toolStripSeparatorUnit.Name = "toolStripSeparatorUnit";
            this.toolStripSeparatorUnit.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddStep
            // 
            this.btnAddStep.Name = "btnAddStep";
            this.btnAddStep.Size = new System.Drawing.Size(23, 22);
            this.btnAddStep.Click += new System.EventHandler(this.btnAddStep_Click);
            // 
            // btnEditStep
            // 
            this.btnEditStep.Name = "btnEditStep";
            this.btnEditStep.Size = new System.Drawing.Size(23, 22);
            this.btnEditStep.Click += new System.EventHandler(this.btnEditStep_Click);
            // 
            // btnDeleteStep
            // 
            this.btnDeleteStep.Name = "btnDeleteStep";
            this.btnDeleteStep.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteStep.Click += new System.EventHandler(this.btnDeleteStep_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(23, 22);
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnImportCsv
            // 
            this.btnImportCsv.Name = "btnImportCsv";
            this.btnImportCsv.Size = new System.Drawing.Size(23, 22);
            this.btnImportCsv.Click += new System.EventHandler(this.btnImportCsv_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FormConfigRecipe
            // 
            this.ClientSize = new System.Drawing.Size(950, 580);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormConfigRecipe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            this.splitTreeAndPreview.Panel1.ResumeLayout(false);
            this.splitTreeAndPreview.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTreeAndPreview)).EndInit();
            this.splitTreeAndPreview.ResumeLayout(false);
            this.panelRecipeInfo.ResumeLayout(false);
            this.panelRecipeInfo.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
	}
}
