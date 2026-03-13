namespace Studio.Script
{
    partial class FormNewScriptEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemBuildSolution = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.startDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.stepIntoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleBreakpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arraygeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolStripMenuItemBuild,
            this.windowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(804, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.MergeIndex = 0;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator3.MergeIndex = 8;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(137, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 9;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsToolStripMenuItem.MergeIndex = 10;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator5,
            this.toolStripMenuItemEditCut,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.editToolStripMenuItem.MergeIndex = 1;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.DropDownOpened += new System.EventHandler(this.editToolStripMenuItem_DropDownOpened);
            this.editToolStripMenuItem.DropDownClosed += new System.EventHandler(this.editToolStripMenuItem_DropDownClosed);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // toolStripMenuItemEditCut
            // 
            this.toolStripMenuItemEditCut.Name = "toolStripMenuItemEditCut";
            this.toolStripMenuItemEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.toolStripMenuItemEditCut.Size = new System.Drawing.Size(166, 22);
            this.toolStripMenuItemEditCut.Text = "Cu&t";
            this.toolStripMenuItemEditCut.Click += new System.EventHandler(this.toolStripMenuItemEditCut_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripMenuItemBuild
            // 
            this.toolStripMenuItemBuild.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemBuildSolution,
            this.toolStripSeparator1,
            this.startDebuggingToolStripMenuItem,
            this.stopDebuggingToolStripMenuItem,
            this.toolStripSeparator2,
            this.stepIntoToolStripMenuItem,
            this.stepOverToolStripMenuItem,
            this.toolStripSeparator4,
            this.toggleBreakpointToolStripMenuItem});
            this.toolStripMenuItemBuild.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripMenuItemBuild.MergeIndex = 3;
            this.toolStripMenuItemBuild.Name = "toolStripMenuItemBuild";
            this.toolStripMenuItemBuild.Size = new System.Drawing.Size(46, 20);
            this.toolStripMenuItemBuild.Text = "&Build";
            this.toolStripMenuItemBuild.DropDownOpened += new System.EventHandler(this.toolStripMenuItemBuild_DropDownOpened);
            // 
            // toolStripMenuItemBuildSolution
            // 
            this.toolStripMenuItemBuildSolution.Name = "toolStripMenuItemBuildSolution";
            this.toolStripMenuItemBuildSolution.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.toolStripMenuItemBuildSolution.Size = new System.Drawing.Size(237, 22);
            this.toolStripMenuItemBuildSolution.Text = "Build Solution";
            this.toolStripMenuItemBuildSolution.Click += new System.EventHandler(this.toolStripMenuItemBuildSolution_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(234, 6);
            // 
            // startDebuggingToolStripMenuItem
            // 
            this.startDebuggingToolStripMenuItem.Name = "startDebuggingToolStripMenuItem";
            this.startDebuggingToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startDebuggingToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.startDebuggingToolStripMenuItem.Text = "Start/Continue Debugging";
            this.startDebuggingToolStripMenuItem.Click += new System.EventHandler(this.startDebuggingToolStripMenuItem_Click);
            // 
            // stopDebuggingToolStripMenuItem
            // 
            this.stopDebuggingToolStripMenuItem.Name = "stopDebuggingToolStripMenuItem";
            this.stopDebuggingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.stopDebuggingToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.stopDebuggingToolStripMenuItem.Text = "Stop Debugging";
            this.stopDebuggingToolStripMenuItem.Click += new System.EventHandler(this.stopDebuggingToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(234, 6);
            // 
            // stepIntoToolStripMenuItem
            // 
            this.stepIntoToolStripMenuItem.Name = "stepIntoToolStripMenuItem";
            this.stepIntoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.stepIntoToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.stepIntoToolStripMenuItem.Text = "Step Into";
            this.stepIntoToolStripMenuItem.Click += new System.EventHandler(this.stepIntoToolStripMenuItem_Click);
            // 
            // stepOverToolStripMenuItem
            // 
            this.stepOverToolStripMenuItem.Name = "stepOverToolStripMenuItem";
            this.stepOverToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.stepOverToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.stepOverToolStripMenuItem.Text = "Step Over";
            this.stepOverToolStripMenuItem.Click += new System.EventHandler(this.stepOverToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(234, 6);
            // 
            // toggleBreakpointToolStripMenuItem
            // 
            this.toggleBreakpointToolStripMenuItem.Name = "toggleBreakpointToolStripMenuItem";
            this.toggleBreakpointToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.toggleBreakpointToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.toggleBreakpointToolStripMenuItem.Text = "Toggle Breakpoint";
            this.toggleBreakpointToolStripMenuItem.Click += new System.EventHandler(this.toggleBreakpointToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cascadeToolStripMenuItem,
            this.tileHorizontallyToolStripMenuItem,
            this.tileVerticallyToolStripMenuItem,
            this.arraygeIconToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem});
            this.windowToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.windowToolStripMenuItem.MergeIndex = 6;
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.cascadeToolStripMenuItem.Text = "&Cascade";
            // 
            // tileHorizontallyToolStripMenuItem
            // 
            this.tileHorizontallyToolStripMenuItem.Name = "tileHorizontallyToolStripMenuItem";
            this.tileHorizontallyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4)));
            this.tileHorizontallyToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.tileHorizontallyToolStripMenuItem.Text = "Tile &Horizontally";
            // 
            // tileVerticallyToolStripMenuItem
            // 
            this.tileVerticallyToolStripMenuItem.Name = "tileVerticallyToolStripMenuItem";
            this.tileVerticallyToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.tileVerticallyToolStripMenuItem.Text = "Tile &Vertically";
            // 
            // arraygeIconToolStripMenuItem
            // 
            this.arraygeIconToolStripMenuItem.Name = "arraygeIconToolStripMenuItem";
            this.arraygeIconToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.arraygeIconToolStripMenuItem.Text = "Arrange &Icon";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.closeToolStripMenuItem.Text = "C&lose";
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.closeAllToolStripMenuItem.Text = "Close &All";
            // 
            // FormNewScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 410);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormNewScriptEditor";
            this.Text = "FormNewScriptEditor";
            this.Load += new System.EventHandler(this.FormNewScriptEditor_Load);
            this.Activated += new System.EventHandler(this.FormNewScriptEditor_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNewScriptEditor_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileHorizontallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileVerticallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arraygeIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuild;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditCut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem stopDebuggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startDebuggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem stepIntoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepOverToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toggleBreakpointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemBuildSolution;
    }
}