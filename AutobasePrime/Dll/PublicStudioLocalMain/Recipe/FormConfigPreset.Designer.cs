namespace PublicStudioLocalMain.Recipe
{
	partial class FormConfigPreset
	{
		private System.ComponentModel.IContainer components = null;

		// Controls — ToolStrip
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btnNew;
		private System.Windows.Forms.ToolStripButton btnDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSep1;
		private System.Windows.Forms.ToolStripButton btnSave;
		private System.Windows.Forms.ToolStripSeparator toolStripSep2;
		private System.Windows.Forms.ToolStripButton btnExportCsv;
		private System.Windows.Forms.ToolStripButton btnImportCsv;
		private System.Windows.Forms.ToolStripSeparator toolStripSep3;
		private System.Windows.Forms.ToolStripButton btnApply;
		private System.Windows.Forms.ToolStripButton btnCapture;

		// Controls — Layout
		private System.Windows.Forms.SplitContainer splitMain;

		// Left panel — search + preset list
		private System.Windows.Forms.Panel panelSearch;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.ListView listViewPresets;
		private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.ColumnHeader colVariantCount;
		private System.Windows.Forms.ColumnHeader colUpdated;

		// Right panel — info
		private System.Windows.Forms.Panel panelInfo;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textBoxName;

		// Right panel — Alias Map
		private System.Windows.Forms.Panel panelAliasHeader;
		private System.Windows.Forms.Label lblAliasCaption;
		private System.Windows.Forms.Button btnAddAlias;
		private System.Windows.Forms.Button btnRemoveAlias;
		private System.Windows.Forms.DataGridView dgvAliasMap;
		private System.Windows.Forms.DataGridViewTextBoxColumn AliasCol;
		private System.Windows.Forms.DataGridViewTextBoxColumn TagCol;
		private System.Windows.Forms.DataGridViewButtonColumn SelectTagCol;

		// Right panel — variant (TabControl)
		private System.Windows.Forms.Panel panelVariant;
		private System.Windows.Forms.Label lblVariantCaption;
		private System.Windows.Forms.TabControl tabVariants;
		private System.Windows.Forms.Button btnAddVariant;
		private System.Windows.Forms.Button btnDeleteVariant;

		// Right panel — items (Values grid)
		private System.Windows.Forms.DataGridView dgvItems;
		private System.Windows.Forms.DataGridViewTextBoxColumn AliasName;
		private System.Windows.Forms.DataGridViewTextBoxColumn SetValue;

		// Item controls
		private System.Windows.Forms.Panel panelItemButtons;
		private System.Windows.Forms.Button btnItemUp;
		private System.Windows.Forms.Button btnItemDown;

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.listViewPresets = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVariantCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUpdated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelSearch = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.AliasName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelItemButtons = new System.Windows.Forms.Panel();
            this.btnItemUp = new System.Windows.Forms.Button();
            this.btnItemDown = new System.Windows.Forms.Button();
            this.panelVariant = new System.Windows.Forms.Panel();
            this.tabVariants = new System.Windows.Forms.TabControl();
            this.lblVariantCaption = new System.Windows.Forms.Label();
            this.btnAddVariant = new System.Windows.Forms.Button();
            this.btnDeleteVariant = new System.Windows.Forms.Button();
            this.dgvAliasMap = new System.Windows.Forms.DataGridView();
            this.AliasCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TagCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectTagCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panelAliasHeader = new System.Windows.Forms.Panel();
            this.lblAliasCaption = new System.Windows.Forms.Label();
            this.btnAddAlias = new System.Windows.Forms.Button();
            this.btnRemoveAlias = new System.Windows.Forms.Button();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportCsv = new System.Windows.Forms.ToolStripButton();
            this.btnImportCsv = new System.Windows.Forms.ToolStripButton();
            this.toolStripSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnApply = new System.Windows.Forms.ToolStripButton();
            this.btnCapture = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.panelItemButtons.SuspendLayout();
            this.panelVariant.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAliasMap)).BeginInit();
            this.panelAliasHeader.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 25);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.listViewPresets);
            this.splitMain.Panel1.Controls.Add(this.panelSearch);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.dgvItems);
            this.splitMain.Panel2.Controls.Add(this.panelItemButtons);
            this.splitMain.Panel2.Controls.Add(this.panelVariant);
            this.splitMain.Panel2.Controls.Add(this.dgvAliasMap);
            this.splitMain.Panel2.Controls.Add(this.panelAliasHeader);
            this.splitMain.Panel2.Controls.Add(this.panelInfo);
            this.splitMain.Size = new System.Drawing.Size(684, 416);
            this.splitMain.SplitterDistance = 275;
            this.splitMain.TabIndex = 0;
            // 
            // listViewPresets
            // 
            this.listViewPresets.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewPresets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colVariantCount,
            this.colUpdated});
            this.listViewPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPresets.FullRowSelect = true;
            this.listViewPresets.HideSelection = false;
            this.listViewPresets.Location = new System.Drawing.Point(0, 28);
            this.listViewPresets.MultiSelect = false;
            this.listViewPresets.Name = "listViewPresets";
            this.listViewPresets.Size = new System.Drawing.Size(275, 388);
            this.listViewPresets.TabIndex = 0;
            this.listViewPresets.UseCompatibleStateImageBehavior = false;
            this.listViewPresets.View = System.Windows.Forms.View.Details;
            this.listViewPresets.SelectedIndexChanged += new System.EventHandler(this.listViewPresets_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 120;
            // 
            // colVariantCount
            // 
            this.colVariantCount.Text = "Variants";
            // 
            // colUpdated
            // 
            this.colUpdated.Text = "Updated";
            this.colUpdated.Width = 144;
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Padding = new System.Windows.Forms.Padding(4);
            this.panelSearch.Size = new System.Drawing.Size(275, 28);
            this.panelSearch.TabIndex = 1;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(4, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(267, 21);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // dgvItems
            // 
            this.dgvItems.AllowDrop = true;
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.BackgroundColor = System.Drawing.Color.White;
            this.dgvItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AliasName,
            this.SetValue});
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new System.Drawing.Point(0, 248);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(405, 138);
            this.dgvItems.TabIndex = 4;
            this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            this.dgvItems.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvItems_DragDrop);
            this.dgvItems.DragOver += new System.Windows.Forms.DragEventHandler(this.dgvItems_DragOver);
            this.dgvItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvItems_MouseDown);
            this.dgvItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvItems_MouseMove);
            // 
            // AliasName
            // 
            this.AliasName.FillWeight = 40F;
            this.AliasName.HeaderText = "Alias";
            this.AliasName.Name = "AliasName";
            this.AliasName.ReadOnly = true;
            // 
            // SetValue
            // 
            this.SetValue.FillWeight = 60F;
            this.SetValue.HeaderText = "Value";
            this.SetValue.Name = "SetValue";
            // 
            // panelItemButtons
            // 
            this.panelItemButtons.Controls.Add(this.btnItemUp);
            this.panelItemButtons.Controls.Add(this.btnItemDown);
            this.panelItemButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelItemButtons.Location = new System.Drawing.Point(0, 386);
            this.panelItemButtons.Name = "panelItemButtons";
            this.panelItemButtons.Size = new System.Drawing.Size(405, 30);
            this.panelItemButtons.TabIndex = 5;
            // 
            // btnItemUp
            // 
            this.btnItemUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnItemUp.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnItemUp.Location = new System.Drawing.Point(4, 2);
            this.btnItemUp.Name = "btnItemUp";
            this.btnItemUp.Size = new System.Drawing.Size(75, 25);
            this.btnItemUp.TabIndex = 0;
            this.btnItemUp.Text = "▲ Up";
            this.btnItemUp.Click += new System.EventHandler(this.btnItemUp_Click);
            // 
            // btnItemDown
            // 
            this.btnItemDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnItemDown.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnItemDown.Location = new System.Drawing.Point(83, 2);
            this.btnItemDown.Name = "btnItemDown";
            this.btnItemDown.Size = new System.Drawing.Size(75, 25);
            this.btnItemDown.TabIndex = 1;
            this.btnItemDown.Text = "▼ Down";
            this.btnItemDown.Click += new System.EventHandler(this.btnItemDown_Click);
            // 
            // panelVariant
            // 
            this.panelVariant.Controls.Add(this.tabVariants);
            this.panelVariant.Controls.Add(this.lblVariantCaption);
            this.panelVariant.Controls.Add(this.btnAddVariant);
            this.panelVariant.Controls.Add(this.btnDeleteVariant);
            this.panelVariant.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelVariant.Location = new System.Drawing.Point(0, 198);
            this.panelVariant.Name = "panelVariant";
            this.panelVariant.Size = new System.Drawing.Size(405, 50);
            this.panelVariant.TabIndex = 3;
            // 
            // tabVariants
            // 
            this.tabVariants.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabVariants.Location = new System.Drawing.Point(0, 24);
            this.tabVariants.Name = "tabVariants";
            this.tabVariants.SelectedIndex = 0;
            this.tabVariants.Size = new System.Drawing.Size(405, 24);
            this.tabVariants.TabIndex = 1;
            this.tabVariants.SelectedIndexChanged += new System.EventHandler(this.tabVariants_SelectedIndexChanged);
            // 
            // lblVariantCaption
            // 
            this.lblVariantCaption.AutoSize = true;
            this.lblVariantCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblVariantCaption.Location = new System.Drawing.Point(4, 4);
            this.lblVariantCaption.Name = "lblVariantCaption";
            this.lblVariantCaption.Size = new System.Drawing.Size(51, 15);
            this.lblVariantCaption.TabIndex = 0;
            this.lblVariantCaption.Text = "Variants";
            // 
            // btnAddVariant
            // 
            this.btnAddVariant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddVariant.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddVariant.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnAddVariant.Location = new System.Drawing.Point(306, 0);
            this.btnAddVariant.Name = "btnAddVariant";
            this.btnAddVariant.Size = new System.Drawing.Size(47, 23);
            this.btnAddVariant.TabIndex = 2;
            this.btnAddVariant.Text = "+";
            this.btnAddVariant.Click += new System.EventHandler(this.btnAddVariant_Click);
            // 
            // btnDeleteVariant
            // 
            this.btnDeleteVariant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteVariant.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteVariant.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnDeleteVariant.Location = new System.Drawing.Point(357, 0);
            this.btnDeleteVariant.Name = "btnDeleteVariant";
            this.btnDeleteVariant.Size = new System.Drawing.Size(44, 23);
            this.btnDeleteVariant.TabIndex = 3;
            this.btnDeleteVariant.Text = "-";
            this.btnDeleteVariant.Click += new System.EventHandler(this.btnDeleteVariant_Click);
            // 
            // dgvAliasMap
            // 
            this.dgvAliasMap.AllowUserToAddRows = false;
            this.dgvAliasMap.AllowUserToDeleteRows = false;
            this.dgvAliasMap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAliasMap.BackgroundColor = System.Drawing.Color.White;
            this.dgvAliasMap.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAliasMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvAliasMap.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AliasCol,
            this.TagCol,
            this.SelectTagCol});
            this.dgvAliasMap.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvAliasMap.Location = new System.Drawing.Point(0, 58);
            this.dgvAliasMap.Name = "dgvAliasMap";
            this.dgvAliasMap.RowHeadersVisible = false;
            this.dgvAliasMap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAliasMap.Size = new System.Drawing.Size(405, 140);
            this.dgvAliasMap.TabIndex = 2;
            this.dgvAliasMap.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAliasMap_CellClick);
            this.dgvAliasMap.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAliasMap_CellValueChanged);
            // 
            // AliasCol
            // 
            this.AliasCol.FillWeight = 40F;
            this.AliasCol.HeaderText = "Alias";
            this.AliasCol.Name = "AliasCol";
            // 
            // TagCol
            // 
            this.TagCol.FillWeight = 55F;
            this.TagCol.HeaderText = "Tag";
            this.TagCol.Name = "TagCol";
            // 
            // SelectTagCol
            // 
            this.SelectTagCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SelectTagCol.HeaderText = "";
            this.SelectTagCol.Name = "SelectTagCol";
            this.SelectTagCol.Text = "...";
            this.SelectTagCol.UseColumnTextForButtonValue = true;
            this.SelectTagCol.Width = 35;
            // 
            // panelAliasHeader
            // 
            this.panelAliasHeader.Controls.Add(this.lblAliasCaption);
            this.panelAliasHeader.Controls.Add(this.btnAddAlias);
            this.panelAliasHeader.Controls.Add(this.btnRemoveAlias);
            this.panelAliasHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAliasHeader.Location = new System.Drawing.Point(0, 32);
            this.panelAliasHeader.Name = "panelAliasHeader";
            this.panelAliasHeader.Size = new System.Drawing.Size(405, 26);
            this.panelAliasHeader.TabIndex = 1;
            // 
            // lblAliasCaption
            // 
            this.lblAliasCaption.AutoSize = true;
            this.lblAliasCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAliasCaption.Location = new System.Drawing.Point(4, 5);
            this.lblAliasCaption.Name = "lblAliasCaption";
            this.lblAliasCaption.Size = new System.Drawing.Size(59, 15);
            this.lblAliasCaption.TabIndex = 0;
            this.lblAliasCaption.Text = "Alias Map";
            // 
            // btnAddAlias
            // 
            this.btnAddAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAlias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddAlias.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnAddAlias.Location = new System.Drawing.Point(306, 1);
            this.btnAddAlias.Name = "btnAddAlias";
            this.btnAddAlias.Size = new System.Drawing.Size(47, 23);
            this.btnAddAlias.TabIndex = 1;
            this.btnAddAlias.Text = "+ Add";
            this.btnAddAlias.Click += new System.EventHandler(this.btnAddAlias_Click);
            // 
            // btnRemoveAlias
            // 
            this.btnRemoveAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveAlias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAlias.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnRemoveAlias.Location = new System.Drawing.Point(357, 1);
            this.btnRemoveAlias.Name = "btnRemoveAlias";
            this.btnRemoveAlias.Size = new System.Drawing.Size(44, 23);
            this.btnRemoveAlias.TabIndex = 2;
            this.btnRemoveAlias.Text = "- Del";
            this.btnRemoveAlias.Click += new System.EventHandler(this.btnRemoveAlias_Click);
            // 
            // panelInfo
            // 
            this.panelInfo.Controls.Add(this.labelName);
            this.panelInfo.Controls.Add(this.textBoxName);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelInfo.Location = new System.Drawing.Point(0, 0);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(405, 32);
            this.panelInfo.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelName.Location = new System.Drawing.Point(8, 8);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(43, 15);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxName.Location = new System.Drawing.Point(56, 6);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(341, 21);
            this.textBoxName.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnDelete,
            this.toolStripSep1,
            this.btnSave,
            this.toolStripSep2,
            this.btnExportCsv,
            this.btnImportCsv,
            this.toolStripSep3,
            this.btnApply,
            this.btnCapture});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(684, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // btnNew
            // 
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(49, 22);
            this.btnNew.Text = "✚ New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 22);
            this.btnDelete.Text = "✕ Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolStripSep1
            // 
            this.toolStripSep1.Name = "toolStripSep1";
            this.toolStripSep1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(52, 22);
            this.btnSave.Text = "■ Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSep2
            // 
            this.toolStripSep2.Name = "toolStripSep2";
            this.toolStripSep2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(60, 22);
            this.btnExportCsv.Text = "↑ Export";
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnImportCsv
            // 
            this.btnImportCsv.Name = "btnImportCsv";
            this.btnImportCsv.Size = new System.Drawing.Size(62, 22);
            this.btnImportCsv.Text = "↓ Import";
            this.btnImportCsv.Click += new System.EventHandler(this.btnImportCsv_Click);
            // 
            // toolStripSep3
            // 
            this.toolStripSep3.Name = "toolStripSep3";
            this.toolStripSep3.Size = new System.Drawing.Size(6, 25);
            this.toolStripSep3.Visible = false;
            // 
            // btnApply
            // 
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(58, 22);
            this.btnApply.Text = "▶ Apply";
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(69, 22);
            this.btnCapture.Text = "◎ Capture";
            this.btnCapture.Visible = false;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // FormConfigPreset
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(684, 441);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStrip1);
            this.MinimizeBox = false;
            this.Name = "FormConfigPreset";
            this.Text = "Preset Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigPreset_FormClosing);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.panelItemButtons.ResumeLayout(false);
            this.panelVariant.ResumeLayout(false);
            this.panelVariant.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAliasMap)).EndInit();
            this.panelAliasHeader.ResumeLayout(false);
            this.panelAliasHeader.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
	}
}
