using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using NetTools.OldDefine;
using System.Drawing.Drawing2D;
using AutoLibLocal;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormMainEdit.
	/// </summary>
	public class FormReportMainEdit : System.Windows.Forms.Form
	{
        private System.ComponentModel.IContainer components;
		//public string sFilename;
        private System.Windows.Forms.Panel panel1;
		public FormReportChild formChild;
        StatusBar statusBarMain;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItemPopupEditCut;
		private System.Windows.Forms.MenuItem menuItemPopupEditCopy;
		private System.Windows.Forms.MenuItem menuItemPopupEditPaste;
		private System.Windows.Forms.MenuItem menuItemPopupEditDelete;
		private System.Windows.Forms.MenuItem menuItem25;
		private System.Windows.Forms.MenuItem menuItemPopupCellTextColor;
		private System.Windows.Forms.MenuItem menuItemPopupCellBackColor;
		private System.Windows.Forms.MenuItem menuItemPopupCellLine;
		private System.Windows.Forms.MenuItem menuItemPopupCellFont;
		private System.Windows.Forms.MenuItem menuItemPopupCellSize;
		private System.Windows.Forms.MenuItem menuItemPopupCellAlign;
		private System.Windows.Forms.MenuItem menuItemPopupCellDisplayFormat;
		private System.Windows.Forms.MenuItem menuItem32;
		private System.Windows.Forms.MenuItem menuItemPopupCellGroup;
        private System.Windows.Forms.MenuItem menuItemPopupCellUngroup;
		private System.Windows.Forms.ToolBarButton toolBarButtonEditUndo;
		private System.Windows.Forms.ToolBarButton toolBarButtonEditCut;
		private System.Windows.Forms.ToolBarButton toolBarButtonEditCopy;
		private System.Windows.Forms.ToolBarButton toolBarButtonEditPaste;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButtonFilePrint;
		private System.Windows.Forms.ToolBarButton toolBarButtonPreview;
		private System.Windows.Forms.ToolBarButton toolBarButton10;
		private System.Windows.Forms.ToolBarButton toolBarButtonInsertTable;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellAlignLeft;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellAlignCenter;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellAlignRight;
		private System.Windows.Forms.ToolBarButton toolBarButton15;
		private System.Windows.Forms.ToolBarButton toolBarButtonZoomIn;
		private System.Windows.Forms.ToolBarButton toolBarButtonZoomOut;
		private System.Windows.Forms.ToolBarButton toolBarButtonConfigPrintTime;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellTextColor;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellBackColor;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellBorder;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellFont;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellSize;
		private System.Windows.Forms.ToolBarButton toolBarButton20;
		private System.Windows.Forms.ToolBarButton toolBarButtonTextAlignLeft;
		private System.Windows.Forms.ToolBarButton toolBarButtonTextAlignCenter;
		private System.Windows.Forms.ToolBarButton toolBarButtonTextAlignRight;
		private System.Windows.Forms.ToolBarButton toolBarButton24;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellChangeCommand;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellChangeTag;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellChangeTime;
		private System.Windows.Forms.ToolBarButton toolBarButtonCellChangeText;
		private System.Windows.Forms.ToolBarButton toolBarButton29;
		private System.Windows.Forms.ToolBarButton toolBarButtonInsertBasicData;
		private System.Windows.Forms.ToolBarButton toolBarButton31;
		private System.Windows.Forms.ToolBarButton toolBarButtonViewAsEditMode;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButtonViewAsRunMode;
		private System.Windows.Forms.ToolBarButton toolBarButtonViewAsCommandMode;
		private System.Windows.Forms.ToolBarButton toolBarButtonViewAsTagMode;
        private System.Windows.Forms.ToolBarButton toolBarButtonViewAsTimeMode;
        private System.Windows.Forms.ImageList imageList1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem saveResultToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem printToolStripMenuItem;
        private ToolStripMenuItem previewToolStripMenuItem;
        private ToolStripMenuItem paperSetupToolStripMenuItem;
        private ToolStripMenuItem printMultiDateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem fileInformationToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem pasteAsnewTableToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem editHeaderToolStripMenuItem;
        private ToolStripMenuItem editFooterToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem zoomInToolStripMenuItem;
        private ToolStripMenuItem zoomOutToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem viewAsEditModeToolStripMenuItem;
        private ToolStripMenuItem viewAsRunModeToolStripMenuItem;
        private ToolStripMenuItem viewAsCommandModeToolStripMenuItem;
        private ToolStripMenuItem viewAsTagNameModeToolStripMenuItem;
        private ToolStripMenuItem viewAsTimeModeToolStripMenuItem;
        private ToolStripMenuItem formToolStripMenuItem;
        private ToolStripMenuItem cellToolStripMenuItem;
        private ToolStripMenuItem textColorToolStripMenuItem;
        private ToolStripMenuItem backColorToolStripMenuItem;
        private ToolStripMenuItem borderToolStripMenuItem;
        private ToolStripMenuItem fontToolStripMenuItem;
        private ToolStripMenuItem sizeToolStripMenuItem;
        private ToolStripMenuItem alignToolStripMenuItem;
        private ToolStripMenuItem displayFormatToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem groupToolStripMenuItem;
        private ToolStripMenuItem ungroupToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem cellCommandToolStripMenuItem;
        private ToolStripMenuItem cellTagToolStripMenuItem;
        private ToolStripMenuItem cellTimeToolStripMenuItem;
        private ToolStripMenuItem cellTextToolStripMenuItem;
        private ToolStripMenuItem insertToolStripMenuItem;
        private ToolStripMenuItem basicDataToolStripMenuItem;
        private ToolStripMenuItem functionToolStripMenuItem;
        private ToolStripMenuItem tableToolStripMenuItem;
        private ToolStripMenuItem insertTableToolStripMenuItem;
        private ToolStripMenuItem deleteTableToolStripMenuItem;
        private ToolStripMenuItem insertRowToolStripMenuItem;
        private ToolStripMenuItem deleteRowToolStripMenuItem;
        private ToolStripMenuItem insertColumnToolStripMenuItem;
        private ToolStripMenuItem deleteColumnToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem alignToLeftToolStripMenuItem;
        private ToolStripMenuItem alignToCenterToolStripMenuItem;
        private ToolStripMenuItem alignToRightToolStripMenuItem;
        private ToolStripMenuItem configToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem paperColorToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem printTimeToolStripMenuItem;
        private ToolStripMenuItem minListTimeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem configStringVarsToolStripMenuItem;
        private ToolStripMenuItem reportEtcToolStripMenuItem;
        private ToolStripMenuItem windowToolStripMenuItem;
        private ToolStripMenuItem cascadeToolStripMenuItem;
        private ToolStripMenuItem tileHorizontallyToolStripMenuItem;
        private ToolStripMenuItem tileVerticallyToolStripMenuItem;
        private ToolStripMenuItem arraygeIconToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem closeAllToolStripMenuItem;
		StatusBarPanel statusBarPanelMain = new StatusBarPanel();

		public FormReportMainEdit(string filename, StatusBar status_bar, CallBackOnMdiActivated callbackmdi, CallBackOnPopupView callbackview)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                this.arraygeIconToolStripMenuItem.Visible = false;
            }

			formChild = new FormReportChild(this, filename, statusBarPanelMain, EnumViewMode.EDIT);
			formChild.TopLevel = false;
			formChild.Dock = DockStyle.Fill;
			this.panel1.Controls.Add(formChild);
			statusBarMain = status_bar;
			statusBarPanelMain.Width = 400;
			this.lpfnCallOnMdiActivated = callbackmdi;
			this.lpfnCallBackOnPopupView = callbackview;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportMainEdit));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paperSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printMultiDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.fileInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteAsnewTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.editHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFooterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.viewAsEditModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsRunModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsCommandModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsTagNameModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAsTimeModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ungroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.cellCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.alignToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.paperColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.printTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minListTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.configStringVarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportEtcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arraygeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemPopupEditCut = new System.Windows.Forms.MenuItem();
            this.menuItemPopupEditCopy = new System.Windows.Forms.MenuItem();
            this.menuItemPopupEditPaste = new System.Windows.Forms.MenuItem();
            this.menuItemPopupEditDelete = new System.Windows.Forms.MenuItem();
            this.menuItem25 = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellTextColor = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellBackColor = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellLine = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellFont = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellSize = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellAlign = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellDisplayFormat = new System.Windows.Forms.MenuItem();
            this.menuItem32 = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellGroup = new System.Windows.Forms.MenuItem();
            this.menuItemPopupCellUngroup = new System.Windows.Forms.MenuItem();
            this.toolBarButtonEditUndo = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonEditCut = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonEditCopy = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonEditPaste = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonFilePrint = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonPreview = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton10 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonInsertTable = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellAlignLeft = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellAlignCenter = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellAlignRight = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton15 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonZoomIn = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonZoomOut = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonConfigPrintTime = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellTextColor = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellBackColor = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellBorder = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellFont = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellSize = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton20 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonTextAlignLeft = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonTextAlignCenter = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonTextAlignRight = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton24 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellChangeCommand = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellChangeTag = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellChangeTime = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCellChangeText = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton29 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonInsertBasicData = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton31 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonViewAsEditMode = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonViewAsRunMode = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonViewAsCommandMode = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonViewAsTagMode = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonViewAsTimeMode = new System.Windows.Forms.ToolBarButton();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.formToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.tableToolStripMenuItem,
            this.configToolStripMenuItem,
            this.windowToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveResultToolStripMenuItem,
            this.toolStripSeparator2,
            this.printToolStripMenuItem,
            this.previewToolStripMenuItem,
            this.paperSetupToolStripMenuItem,
            this.printMultiDateToolStripMenuItem,
            this.toolStripSeparator3,
            this.fileInformationToolStripMenuItem});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.MergeIndex = 0;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator1.MergeIndex = 8;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 9;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileSave_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsToolStripMenuItem.MergeIndex = 10;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileSaveAs_Click);
            // 
            // saveResultToolStripMenuItem
            // 
            this.saveResultToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveResultToolStripMenuItem.MergeIndex = 11;
            this.saveResultToolStripMenuItem.Name = "saveResultToolStripMenuItem";
            resources.ApplyResources(this.saveResultToolStripMenuItem, "saveResultToolStripMenuItem");
            this.saveResultToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileSaveResult_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator2.MergeIndex = 12;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.printToolStripMenuItem.MergeIndex = 13;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            resources.ApplyResources(this.printToolStripMenuItem, "printToolStripMenuItem");
            this.printToolStripMenuItem.Click += new System.EventHandler(this.menuItemFilePrint_Click);
            // 
            // previewToolStripMenuItem
            // 
            this.previewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.previewToolStripMenuItem.MergeIndex = 14;
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            resources.ApplyResources(this.previewToolStripMenuItem, "previewToolStripMenuItem");
            this.previewToolStripMenuItem.Click += new System.EventHandler(this.menuItemFilePreview_Click);
            // 
            // paperSetupToolStripMenuItem
            // 
            this.paperSetupToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.paperSetupToolStripMenuItem.MergeIndex = 15;
            this.paperSetupToolStripMenuItem.Name = "paperSetupToolStripMenuItem";
            resources.ApplyResources(this.paperSetupToolStripMenuItem, "paperSetupToolStripMenuItem");
            this.paperSetupToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileConfigPaper_Click);
            // 
            // printMultiDateToolStripMenuItem
            // 
            this.printMultiDateToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.printMultiDateToolStripMenuItem.MergeIndex = 16;
            this.printMultiDateToolStripMenuItem.Name = "printMultiDateToolStripMenuItem";
            resources.ApplyResources(this.printMultiDateToolStripMenuItem, "printMultiDateToolStripMenuItem");
            this.printMultiDateToolStripMenuItem.Click += new System.EventHandler(this.menuItemFilePrintMultiDate_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator3.MergeIndex = 17;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // fileInformationToolStripMenuItem
            // 
            this.fileInformationToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.fileInformationToolStripMenuItem.MergeIndex = 18;
            this.fileInformationToolStripMenuItem.Name = "fileInformationToolStripMenuItem";
            resources.ApplyResources(this.fileInformationToolStripMenuItem, "fileInformationToolStripMenuItem");
            this.fileInformationToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileInformation_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.pasteAsnewTableToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator5,
            this.editHeaderToolStripMenuItem,
            this.editFooterToolStripMenuItem});
            this.editToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.editToolStripMenuItem.MergeIndex = 1;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditUndo_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditRedo_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditCut_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditCopy_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditPaste_Click);
            // 
            // pasteAsnewTableToolStripMenuItem
            // 
            this.pasteAsnewTableToolStripMenuItem.Name = "pasteAsnewTableToolStripMenuItem";
            resources.ApplyResources(this.pasteAsnewTableToolStripMenuItem, "pasteAsnewTableToolStripMenuItem");
            this.pasteAsnewTableToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditPasteAsNewTable_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditDelete_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // editHeaderToolStripMenuItem
            // 
            this.editHeaderToolStripMenuItem.Name = "editHeaderToolStripMenuItem";
            resources.ApplyResources(this.editHeaderToolStripMenuItem, "editHeaderToolStripMenuItem");
            this.editHeaderToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditHeader_Click);
            // 
            // editFooterToolStripMenuItem
            // 
            this.editFooterToolStripMenuItem.Name = "editFooterToolStripMenuItem";
            resources.ApplyResources(this.editFooterToolStripMenuItem, "editFooterToolStripMenuItem");
            this.editFooterToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditFooter_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator6,
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.toolStripSeparator7,
            this.viewAsEditModeToolStripMenuItem,
            this.viewAsRunModeToolStripMenuItem,
            this.viewAsCommandModeToolStripMenuItem,
            this.viewAsTagNameModeToolStripMenuItem,
            this.viewAsTimeModeToolStripMenuItem});
            this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.viewToolStripMenuItem.MergeIndex = 2;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemView_Popup);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            resources.ApplyResources(this.zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewZoomIn_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            resources.ApplyResources(this.zoomOutToolStripMenuItem, "zoomOutToolStripMenuItem");
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewZoomOut_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // viewAsEditModeToolStripMenuItem
            // 
            this.viewAsEditModeToolStripMenuItem.Name = "viewAsEditModeToolStripMenuItem";
            resources.ApplyResources(this.viewAsEditModeToolStripMenuItem, "viewAsEditModeToolStripMenuItem");
            this.viewAsEditModeToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewAsEditMode_Click);
            // 
            // viewAsRunModeToolStripMenuItem
            // 
            this.viewAsRunModeToolStripMenuItem.Name = "viewAsRunModeToolStripMenuItem";
            resources.ApplyResources(this.viewAsRunModeToolStripMenuItem, "viewAsRunModeToolStripMenuItem");
            this.viewAsRunModeToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewAsRunMode_Click);
            // 
            // viewAsCommandModeToolStripMenuItem
            // 
            this.viewAsCommandModeToolStripMenuItem.Name = "viewAsCommandModeToolStripMenuItem";
            resources.ApplyResources(this.viewAsCommandModeToolStripMenuItem, "viewAsCommandModeToolStripMenuItem");
            this.viewAsCommandModeToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewAsCommandMode_Click);
            // 
            // viewAsTagNameModeToolStripMenuItem
            // 
            this.viewAsTagNameModeToolStripMenuItem.Name = "viewAsTagNameModeToolStripMenuItem";
            resources.ApplyResources(this.viewAsTagNameModeToolStripMenuItem, "viewAsTagNameModeToolStripMenuItem");
            this.viewAsTagNameModeToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewAsTagMode_Click);
            // 
            // viewAsTimeModeToolStripMenuItem
            // 
            this.viewAsTimeModeToolStripMenuItem.Name = "viewAsTimeModeToolStripMenuItem";
            resources.ApplyResources(this.viewAsTimeModeToolStripMenuItem, "viewAsTimeModeToolStripMenuItem");
            this.viewAsTimeModeToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewAsTimeMode_Click);
            // 
            // formToolStripMenuItem
            // 
            this.formToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cellToolStripMenuItem});
            this.formToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.formToolStripMenuItem.MergeIndex = 3;
            this.formToolStripMenuItem.Name = "formToolStripMenuItem";
            resources.ApplyResources(this.formToolStripMenuItem, "formToolStripMenuItem");
            // 
            // cellToolStripMenuItem
            // 
            this.cellToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textColorToolStripMenuItem,
            this.backColorToolStripMenuItem,
            this.borderToolStripMenuItem,
            this.fontToolStripMenuItem,
            this.sizeToolStripMenuItem,
            this.alignToolStripMenuItem,
            this.displayFormatToolStripMenuItem,
            this.toolStripSeparator8,
            this.groupToolStripMenuItem,
            this.ungroupToolStripMenuItem,
            this.toolStripSeparator9,
            this.cellCommandToolStripMenuItem,
            this.cellTagToolStripMenuItem,
            this.cellTimeToolStripMenuItem,
            this.cellTextToolStripMenuItem});
            this.cellToolStripMenuItem.Name = "cellToolStripMenuItem";
            resources.ApplyResources(this.cellToolStripMenuItem, "cellToolStripMenuItem");
            this.cellToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItem1_Popup);
            // 
            // textColorToolStripMenuItem
            // 
            this.textColorToolStripMenuItem.Name = "textColorToolStripMenuItem";
            resources.ApplyResources(this.textColorToolStripMenuItem, "textColorToolStripMenuItem");
            this.textColorToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellTextColor_Click);
            // 
            // backColorToolStripMenuItem
            // 
            this.backColorToolStripMenuItem.Name = "backColorToolStripMenuItem";
            resources.ApplyResources(this.backColorToolStripMenuItem, "backColorToolStripMenuItem");
            this.backColorToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellBackColor_Click);
            // 
            // borderToolStripMenuItem
            // 
            this.borderToolStripMenuItem.Name = "borderToolStripMenuItem";
            resources.ApplyResources(this.borderToolStripMenuItem, "borderToolStripMenuItem");
            this.borderToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellBorder_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            resources.ApplyResources(this.fontToolStripMenuItem, "fontToolStripMenuItem");
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellFont_Click);
            // 
            // sizeToolStripMenuItem
            // 
            this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
            resources.ApplyResources(this.sizeToolStripMenuItem, "sizeToolStripMenuItem");
            this.sizeToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellSize_Click);
            // 
            // alignToolStripMenuItem
            // 
            this.alignToolStripMenuItem.Name = "alignToolStripMenuItem";
            resources.ApplyResources(this.alignToolStripMenuItem, "alignToolStripMenuItem");
            this.alignToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellAlign_Click);
            // 
            // displayFormatToolStripMenuItem
            // 
            this.displayFormatToolStripMenuItem.Name = "displayFormatToolStripMenuItem";
            resources.ApplyResources(this.displayFormatToolStripMenuItem, "displayFormatToolStripMenuItem");
            this.displayFormatToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellDisplayFormat_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // groupToolStripMenuItem
            // 
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            resources.ApplyResources(this.groupToolStripMenuItem, "groupToolStripMenuItem");
            this.groupToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellGroup_Click);
            // 
            // ungroupToolStripMenuItem
            // 
            this.ungroupToolStripMenuItem.Name = "ungroupToolStripMenuItem";
            resources.ApplyResources(this.ungroupToolStripMenuItem, "ungroupToolStripMenuItem");
            this.ungroupToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellUngroup_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // cellCommandToolStripMenuItem
            // 
            this.cellCommandToolStripMenuItem.Name = "cellCommandToolStripMenuItem";
            resources.ApplyResources(this.cellCommandToolStripMenuItem, "cellCommandToolStripMenuItem");
            this.cellCommandToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellChangeCommand_Click);
            // 
            // cellTagToolStripMenuItem
            // 
            this.cellTagToolStripMenuItem.Name = "cellTagToolStripMenuItem";
            resources.ApplyResources(this.cellTagToolStripMenuItem, "cellTagToolStripMenuItem");
            this.cellTagToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellChangeTag_Click);
            // 
            // cellTimeToolStripMenuItem
            // 
            this.cellTimeToolStripMenuItem.Name = "cellTimeToolStripMenuItem";
            resources.ApplyResources(this.cellTimeToolStripMenuItem, "cellTimeToolStripMenuItem");
            this.cellTimeToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellChangeTime_Click);
            // 
            // cellTextToolStripMenuItem
            // 
            this.cellTextToolStripMenuItem.Name = "cellTextToolStripMenuItem";
            resources.ApplyResources(this.cellTextToolStripMenuItem, "cellTextToolStripMenuItem");
            this.cellTextToolStripMenuItem.Click += new System.EventHandler(this.menuItemFormCellChangeText_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicDataToolStripMenuItem,
            this.functionToolStripMenuItem});
            this.insertToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.insertToolStripMenuItem.MergeIndex = 4;
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            resources.ApplyResources(this.insertToolStripMenuItem, "insertToolStripMenuItem");
            this.insertToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItem3_Popup);
            // 
            // basicDataToolStripMenuItem
            // 
            this.basicDataToolStripMenuItem.Name = "basicDataToolStripMenuItem";
            resources.ApplyResources(this.basicDataToolStripMenuItem, "basicDataToolStripMenuItem");
            this.basicDataToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertBasicData_Click);
            // 
            // functionToolStripMenuItem
            // 
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            resources.ApplyResources(this.functionToolStripMenuItem, "functionToolStripMenuItem");
            this.functionToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertFunction_Click);
            // 
            // tableToolStripMenuItem
            // 
            this.tableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertTableToolStripMenuItem,
            this.deleteTableToolStripMenuItem,
            this.insertRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.insertColumnToolStripMenuItem,
            this.deleteColumnToolStripMenuItem,
            this.toolStripSeparator10,
            this.alignToLeftToolStripMenuItem,
            this.alignToCenterToolStripMenuItem,
            this.alignToRightToolStripMenuItem});
            this.tableToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.tableToolStripMenuItem.MergeIndex = 5;
            this.tableToolStripMenuItem.Name = "tableToolStripMenuItem";
            resources.ApplyResources(this.tableToolStripMenuItem, "tableToolStripMenuItem");
            this.tableToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemTable_Popup);
            // 
            // insertTableToolStripMenuItem
            // 
            this.insertTableToolStripMenuItem.Name = "insertTableToolStripMenuItem";
            resources.ApplyResources(this.insertTableToolStripMenuItem, "insertTableToolStripMenuItem");
            this.insertTableToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertTable_Click);
            // 
            // deleteTableToolStripMenuItem
            // 
            this.deleteTableToolStripMenuItem.Name = "deleteTableToolStripMenuItem";
            resources.ApplyResources(this.deleteTableToolStripMenuItem, "deleteTableToolStripMenuItem");
            this.deleteTableToolStripMenuItem.Click += new System.EventHandler(this.menuItemDeleteTable_Click);
            // 
            // insertRowToolStripMenuItem
            // 
            this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
            resources.ApplyResources(this.insertRowToolStripMenuItem, "insertRowToolStripMenuItem");
            this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertRow_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            resources.ApplyResources(this.deleteRowToolStripMenuItem, "deleteRowToolStripMenuItem");
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.menuItemDeleteRow_Click);
            // 
            // insertColumnToolStripMenuItem
            // 
            this.insertColumnToolStripMenuItem.Name = "insertColumnToolStripMenuItem";
            resources.ApplyResources(this.insertColumnToolStripMenuItem, "insertColumnToolStripMenuItem");
            this.insertColumnToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertColumn_Click);
            // 
            // deleteColumnToolStripMenuItem
            // 
            this.deleteColumnToolStripMenuItem.Name = "deleteColumnToolStripMenuItem";
            resources.ApplyResources(this.deleteColumnToolStripMenuItem, "deleteColumnToolStripMenuItem");
            this.deleteColumnToolStripMenuItem.Click += new System.EventHandler(this.menuItemDeleteColumn_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // alignToLeftToolStripMenuItem
            // 
            this.alignToLeftToolStripMenuItem.Name = "alignToLeftToolStripMenuItem";
            resources.ApplyResources(this.alignToLeftToolStripMenuItem, "alignToLeftToolStripMenuItem");
            this.alignToLeftToolStripMenuItem.Click += new System.EventHandler(this.menuItemTableAlignToLeft_Click);
            // 
            // alignToCenterToolStripMenuItem
            // 
            this.alignToCenterToolStripMenuItem.Name = "alignToCenterToolStripMenuItem";
            resources.ApplyResources(this.alignToCenterToolStripMenuItem, "alignToCenterToolStripMenuItem");
            this.alignToCenterToolStripMenuItem.Click += new System.EventHandler(this.menuItemTableAlignToCenter_Click);
            // 
            // alignToRightToolStripMenuItem
            // 
            this.alignToRightToolStripMenuItem.Name = "alignToRightToolStripMenuItem";
            resources.ApplyResources(this.alignToRightToolStripMenuItem, "alignToRightToolStripMenuItem");
            this.alignToRightToolStripMenuItem.Click += new System.EventHandler(this.menuItemTableAlignToRight_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator11,
            this.paperColorToolStripMenuItem,
            this.toolStripSeparator12,
            this.printTimeToolStripMenuItem,
            this.minListTimeToolStripMenuItem,
            this.toolStripSeparator13,
            this.configStringVarsToolStripMenuItem,
            this.reportEtcToolStripMenuItem});
            this.configToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.configToolStripMenuItem.MergeIndex = 6;
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            this.configToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemConfig_Popup);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // paperColorToolStripMenuItem
            // 
            this.paperColorToolStripMenuItem.Name = "paperColorToolStripMenuItem";
            resources.ApplyResources(this.paperColorToolStripMenuItem, "paperColorToolStripMenuItem");
            this.paperColorToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigPaperColor_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // printTimeToolStripMenuItem
            // 
            this.printTimeToolStripMenuItem.Name = "printTimeToolStripMenuItem";
            resources.ApplyResources(this.printTimeToolStripMenuItem, "printTimeToolStripMenuItem");
            this.printTimeToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigPrintTime_Click);
            // 
            // minListTimeToolStripMenuItem
            // 
            this.minListTimeToolStripMenuItem.Name = "minListTimeToolStripMenuItem";
            resources.ApplyResources(this.minListTimeToolStripMenuItem, "minListTimeToolStripMenuItem");
            this.minListTimeToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigMinListTime_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // configStringVarsToolStripMenuItem
            // 
            this.configStringVarsToolStripMenuItem.Name = "configStringVarsToolStripMenuItem";
            resources.ApplyResources(this.configStringVarsToolStripMenuItem, "configStringVarsToolStripMenuItem");
            this.configStringVarsToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigStringVars_Click);
            // 
            // reportEtcToolStripMenuItem
            // 
            this.reportEtcToolStripMenuItem.Name = "reportEtcToolStripMenuItem";
            resources.ApplyResources(this.reportEtcToolStripMenuItem, "reportEtcToolStripMenuItem");
            this.reportEtcToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigEtc_Click);
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
            this.windowToolStripMenuItem.MergeIndex = 7;
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            resources.ApplyResources(this.windowToolStripMenuItem, "windowToolStripMenuItem");
            this.windowToolStripMenuItem.DropDownOpened += new System.EventHandler(this.windowToolStripMenuItem_DropDownOpened);
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            resources.ApplyResources(this.cascadeToolStripMenuItem, "cascadeToolStripMenuItem");
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.menuItemWindowCascade_Click);
            // 
            // tileHorizontallyToolStripMenuItem
            // 
            this.tileHorizontallyToolStripMenuItem.Name = "tileHorizontallyToolStripMenuItem";
            resources.ApplyResources(this.tileHorizontallyToolStripMenuItem, "tileHorizontallyToolStripMenuItem");
            this.tileHorizontallyToolStripMenuItem.Click += new System.EventHandler(this.menuItemWindowTileHorz_Click);
            // 
            // tileVerticallyToolStripMenuItem
            // 
            this.tileVerticallyToolStripMenuItem.Name = "tileVerticallyToolStripMenuItem";
            resources.ApplyResources(this.tileVerticallyToolStripMenuItem, "tileVerticallyToolStripMenuItem");
            this.tileVerticallyToolStripMenuItem.Click += new System.EventHandler(this.menuItemWindowTileVert_Click);
            // 
            // arraygeIconToolStripMenuItem
            // 
            this.arraygeIconToolStripMenuItem.Name = "arraygeIconToolStripMenuItem";
            resources.ApplyResources(this.arraygeIconToolStripMenuItem, "arraygeIconToolStripMenuItem");
            this.arraygeIconToolStripMenuItem.Click += new System.EventHandler(this.menuItemWindowArrangeIcon_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.menuItemWindowClose_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            resources.ApplyResources(this.closeAllToolStripMenuItem, "closeAllToolStripMenuItem");
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.menuItemWindowCloseAll_Click);
            // 
            // panel1
            // 
            this.panel1.ContextMenu = this.contextMenu1;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemPopupEditCut,
            this.menuItemPopupEditCopy,
            this.menuItemPopupEditPaste,
            this.menuItemPopupEditDelete,
            this.menuItem25,
            this.menuItemPopupCellTextColor,
            this.menuItemPopupCellBackColor,
            this.menuItemPopupCellLine,
            this.menuItemPopupCellFont,
            this.menuItemPopupCellSize,
            this.menuItemPopupCellAlign,
            this.menuItemPopupCellDisplayFormat,
            this.menuItem32,
            this.menuItemPopupCellGroup,
            this.menuItemPopupCellUngroup});
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // menuItemPopupEditCut
            // 
            this.menuItemPopupEditCut.Index = 0;
            resources.ApplyResources(this.menuItemPopupEditCut, "menuItemPopupEditCut");
            this.menuItemPopupEditCut.Click += new System.EventHandler(this.menuItemEditCut_Click);
            // 
            // menuItemPopupEditCopy
            // 
            this.menuItemPopupEditCopy.Index = 1;
            resources.ApplyResources(this.menuItemPopupEditCopy, "menuItemPopupEditCopy");
            this.menuItemPopupEditCopy.Click += new System.EventHandler(this.menuItemEditCopy_Click);
            // 
            // menuItemPopupEditPaste
            // 
            this.menuItemPopupEditPaste.Index = 2;
            resources.ApplyResources(this.menuItemPopupEditPaste, "menuItemPopupEditPaste");
            this.menuItemPopupEditPaste.Click += new System.EventHandler(this.menuItemEditPaste_Click);
            // 
            // menuItemPopupEditDelete
            // 
            this.menuItemPopupEditDelete.Index = 3;
            resources.ApplyResources(this.menuItemPopupEditDelete, "menuItemPopupEditDelete");
            this.menuItemPopupEditDelete.Click += new System.EventHandler(this.menuItemEditDelete_Click);
            // 
            // menuItem25
            // 
            this.menuItem25.Index = 4;
            resources.ApplyResources(this.menuItem25, "menuItem25");
            // 
            // menuItemPopupCellTextColor
            // 
            this.menuItemPopupCellTextColor.Index = 5;
            resources.ApplyResources(this.menuItemPopupCellTextColor, "menuItemPopupCellTextColor");
            this.menuItemPopupCellTextColor.Click += new System.EventHandler(this.menuItemFormCellTextColor_Click);
            // 
            // menuItemPopupCellBackColor
            // 
            this.menuItemPopupCellBackColor.Index = 6;
            resources.ApplyResources(this.menuItemPopupCellBackColor, "menuItemPopupCellBackColor");
            this.menuItemPopupCellBackColor.Click += new System.EventHandler(this.menuItemFormCellBackColor_Click);
            // 
            // menuItemPopupCellLine
            // 
            this.menuItemPopupCellLine.Index = 7;
            resources.ApplyResources(this.menuItemPopupCellLine, "menuItemPopupCellLine");
            this.menuItemPopupCellLine.Click += new System.EventHandler(this.menuItemFormCellBorder_Click);
            // 
            // menuItemPopupCellFont
            // 
            this.menuItemPopupCellFont.Index = 8;
            resources.ApplyResources(this.menuItemPopupCellFont, "menuItemPopupCellFont");
            this.menuItemPopupCellFont.Click += new System.EventHandler(this.menuItemFormCellFont_Click);
            // 
            // menuItemPopupCellSize
            // 
            this.menuItemPopupCellSize.Index = 9;
            resources.ApplyResources(this.menuItemPopupCellSize, "menuItemPopupCellSize");
            this.menuItemPopupCellSize.Click += new System.EventHandler(this.menuItemFormCellSize_Click);
            // 
            // menuItemPopupCellAlign
            // 
            this.menuItemPopupCellAlign.Index = 10;
            resources.ApplyResources(this.menuItemPopupCellAlign, "menuItemPopupCellAlign");
            this.menuItemPopupCellAlign.Click += new System.EventHandler(this.menuItemFormCellAlign_Click);
            // 
            // menuItemPopupCellDisplayFormat
            // 
            this.menuItemPopupCellDisplayFormat.Index = 11;
            resources.ApplyResources(this.menuItemPopupCellDisplayFormat, "menuItemPopupCellDisplayFormat");
            this.menuItemPopupCellDisplayFormat.Click += new System.EventHandler(this.menuItemFormCellDisplayFormat_Click);
            // 
            // menuItem32
            // 
            this.menuItem32.Index = 12;
            resources.ApplyResources(this.menuItem32, "menuItem32");
            // 
            // menuItemPopupCellGroup
            // 
            this.menuItemPopupCellGroup.Index = 13;
            resources.ApplyResources(this.menuItemPopupCellGroup, "menuItemPopupCellGroup");
            this.menuItemPopupCellGroup.Click += new System.EventHandler(this.menuItemFormCellGroup_Click);
            // 
            // menuItemPopupCellUngroup
            // 
            this.menuItemPopupCellUngroup.Index = 14;
            resources.ApplyResources(this.menuItemPopupCellUngroup, "menuItemPopupCellUngroup");
            this.menuItemPopupCellUngroup.Click += new System.EventHandler(this.menuItemFormCellUngroup_Click);
            // 
            // toolBarButtonEditUndo
            // 
            resources.ApplyResources(this.toolBarButtonEditUndo, "toolBarButtonEditUndo");
            this.toolBarButtonEditUndo.Name = "toolBarButtonEditUndo";
            this.toolBarButtonEditUndo.Tag = "EditUndo";
            // 
            // toolBarButtonEditCut
            // 
            resources.ApplyResources(this.toolBarButtonEditCut, "toolBarButtonEditCut");
            this.toolBarButtonEditCut.Name = "toolBarButtonEditCut";
            this.toolBarButtonEditCut.Tag = "EditCut";
            // 
            // toolBarButtonEditCopy
            // 
            resources.ApplyResources(this.toolBarButtonEditCopy, "toolBarButtonEditCopy");
            this.toolBarButtonEditCopy.Name = "toolBarButtonEditCopy";
            this.toolBarButtonEditCopy.Tag = "EditCopy";
            // 
            // toolBarButtonEditPaste
            // 
            resources.ApplyResources(this.toolBarButtonEditPaste, "toolBarButtonEditPaste");
            this.toolBarButtonEditPaste.Name = "toolBarButtonEditPaste";
            this.toolBarButtonEditPaste.Tag = "EditPaste";
            // 
            // toolBarButton6
            // 
            this.toolBarButton6.Name = "toolBarButton6";
            this.toolBarButton6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonFilePrint
            // 
            resources.ApplyResources(this.toolBarButtonFilePrint, "toolBarButtonFilePrint");
            this.toolBarButtonFilePrint.Name = "toolBarButtonFilePrint";
            this.toolBarButtonFilePrint.Tag = "FilePrint";
            // 
            // toolBarButtonPreview
            // 
            resources.ApplyResources(this.toolBarButtonPreview, "toolBarButtonPreview");
            this.toolBarButtonPreview.Name = "toolBarButtonPreview";
            this.toolBarButtonPreview.Tag = "Preview";
            // 
            // toolBarButton10
            // 
            this.toolBarButton10.Name = "toolBarButton10";
            this.toolBarButton10.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonInsertTable
            // 
            resources.ApplyResources(this.toolBarButtonInsertTable, "toolBarButtonInsertTable");
            this.toolBarButtonInsertTable.Name = "toolBarButtonInsertTable";
            this.toolBarButtonInsertTable.Tag = "InsertTable";
            // 
            // toolBarButtonCellAlignLeft
            // 
            resources.ApplyResources(this.toolBarButtonCellAlignLeft, "toolBarButtonCellAlignLeft");
            this.toolBarButtonCellAlignLeft.Name = "toolBarButtonCellAlignLeft";
            this.toolBarButtonCellAlignLeft.Tag = "TableAlignLeft";
            // 
            // toolBarButtonCellAlignCenter
            // 
            resources.ApplyResources(this.toolBarButtonCellAlignCenter, "toolBarButtonCellAlignCenter");
            this.toolBarButtonCellAlignCenter.Name = "toolBarButtonCellAlignCenter";
            this.toolBarButtonCellAlignCenter.Tag = "TableAlignCenter";
            // 
            // toolBarButtonCellAlignRight
            // 
            resources.ApplyResources(this.toolBarButtonCellAlignRight, "toolBarButtonCellAlignRight");
            this.toolBarButtonCellAlignRight.Name = "toolBarButtonCellAlignRight";
            this.toolBarButtonCellAlignRight.Tag = "TableAlignRight";
            // 
            // toolBarButton15
            // 
            this.toolBarButton15.Name = "toolBarButton15";
            this.toolBarButton15.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonZoomIn
            // 
            resources.ApplyResources(this.toolBarButtonZoomIn, "toolBarButtonZoomIn");
            this.toolBarButtonZoomIn.Name = "toolBarButtonZoomIn";
            this.toolBarButtonZoomIn.Tag = "ZoomIn";
            // 
            // toolBarButtonZoomOut
            // 
            resources.ApplyResources(this.toolBarButtonZoomOut, "toolBarButtonZoomOut");
            this.toolBarButtonZoomOut.Name = "toolBarButtonZoomOut";
            this.toolBarButtonZoomOut.Tag = "ZoomOut";
            // 
            // toolBarButtonConfigPrintTime
            // 
            resources.ApplyResources(this.toolBarButtonConfigPrintTime, "toolBarButtonConfigPrintTime");
            this.toolBarButtonConfigPrintTime.Name = "toolBarButtonConfigPrintTime";
            this.toolBarButtonConfigPrintTime.Tag = "ConfigPrintTime";
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonCellTextColor
            // 
            resources.ApplyResources(this.toolBarButtonCellTextColor, "toolBarButtonCellTextColor");
            this.toolBarButtonCellTextColor.Name = "toolBarButtonCellTextColor";
            this.toolBarButtonCellTextColor.Tag = "CellTextColor";
            // 
            // toolBarButtonCellBackColor
            // 
            resources.ApplyResources(this.toolBarButtonCellBackColor, "toolBarButtonCellBackColor");
            this.toolBarButtonCellBackColor.Name = "toolBarButtonCellBackColor";
            this.toolBarButtonCellBackColor.Tag = "CellBackColor";
            // 
            // toolBarButtonCellBorder
            // 
            resources.ApplyResources(this.toolBarButtonCellBorder, "toolBarButtonCellBorder");
            this.toolBarButtonCellBorder.Name = "toolBarButtonCellBorder";
            this.toolBarButtonCellBorder.Tag = "CellBorder";
            // 
            // toolBarButtonCellFont
            // 
            resources.ApplyResources(this.toolBarButtonCellFont, "toolBarButtonCellFont");
            this.toolBarButtonCellFont.Name = "toolBarButtonCellFont";
            this.toolBarButtonCellFont.Tag = "CellFont";
            // 
            // toolBarButtonCellSize
            // 
            resources.ApplyResources(this.toolBarButtonCellSize, "toolBarButtonCellSize");
            this.toolBarButtonCellSize.Name = "toolBarButtonCellSize";
            this.toolBarButtonCellSize.Tag = "CellSize";
            // 
            // toolBarButton20
            // 
            this.toolBarButton20.Name = "toolBarButton20";
            this.toolBarButton20.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonTextAlignLeft
            // 
            resources.ApplyResources(this.toolBarButtonTextAlignLeft, "toolBarButtonTextAlignLeft");
            this.toolBarButtonTextAlignLeft.Name = "toolBarButtonTextAlignLeft";
            this.toolBarButtonTextAlignLeft.Tag = "TextAlignLeft";
            // 
            // toolBarButtonTextAlignCenter
            // 
            resources.ApplyResources(this.toolBarButtonTextAlignCenter, "toolBarButtonTextAlignCenter");
            this.toolBarButtonTextAlignCenter.Name = "toolBarButtonTextAlignCenter";
            this.toolBarButtonTextAlignCenter.Tag = "TextAlignCenter";
            // 
            // toolBarButtonTextAlignRight
            // 
            resources.ApplyResources(this.toolBarButtonTextAlignRight, "toolBarButtonTextAlignRight");
            this.toolBarButtonTextAlignRight.Name = "toolBarButtonTextAlignRight";
            this.toolBarButtonTextAlignRight.Tag = "TextAlignRight";
            // 
            // toolBarButton24
            // 
            this.toolBarButton24.Name = "toolBarButton24";
            this.toolBarButton24.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonCellChangeCommand
            // 
            resources.ApplyResources(this.toolBarButtonCellChangeCommand, "toolBarButtonCellChangeCommand");
            this.toolBarButtonCellChangeCommand.Name = "toolBarButtonCellChangeCommand";
            this.toolBarButtonCellChangeCommand.Tag = "CellChangeCommand";
            // 
            // toolBarButtonCellChangeTag
            // 
            resources.ApplyResources(this.toolBarButtonCellChangeTag, "toolBarButtonCellChangeTag");
            this.toolBarButtonCellChangeTag.Name = "toolBarButtonCellChangeTag";
            this.toolBarButtonCellChangeTag.Tag = "CellChangeTag";
            // 
            // toolBarButtonCellChangeTime
            // 
            resources.ApplyResources(this.toolBarButtonCellChangeTime, "toolBarButtonCellChangeTime");
            this.toolBarButtonCellChangeTime.Name = "toolBarButtonCellChangeTime";
            this.toolBarButtonCellChangeTime.Tag = "CellChangeTime";
            // 
            // toolBarButtonCellChangeText
            // 
            resources.ApplyResources(this.toolBarButtonCellChangeText, "toolBarButtonCellChangeText");
            this.toolBarButtonCellChangeText.Name = "toolBarButtonCellChangeText";
            this.toolBarButtonCellChangeText.Tag = "CellChangeText";
            // 
            // toolBarButton29
            // 
            this.toolBarButton29.Name = "toolBarButton29";
            this.toolBarButton29.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonInsertBasicData
            // 
            resources.ApplyResources(this.toolBarButtonInsertBasicData, "toolBarButtonInsertBasicData");
            this.toolBarButtonInsertBasicData.Name = "toolBarButtonInsertBasicData";
            this.toolBarButtonInsertBasicData.Tag = "InsertBasicData";
            // 
            // toolBarButton31
            // 
            this.toolBarButton31.Name = "toolBarButton31";
            this.toolBarButton31.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButtonViewAsEditMode
            // 
            resources.ApplyResources(this.toolBarButtonViewAsEditMode, "toolBarButtonViewAsEditMode");
            this.toolBarButtonViewAsEditMode.Name = "toolBarButtonViewAsEditMode";
            this.toolBarButtonViewAsEditMode.Tag = "ViewAsEditMode";
            // 
            // toolBarButtonViewAsRunMode
            // 
            resources.ApplyResources(this.toolBarButtonViewAsRunMode, "toolBarButtonViewAsRunMode");
            this.toolBarButtonViewAsRunMode.Name = "toolBarButtonViewAsRunMode";
            this.toolBarButtonViewAsRunMode.Tag = "ViewAsRunMode";
            // 
            // toolBarButtonViewAsCommandMode
            // 
            resources.ApplyResources(this.toolBarButtonViewAsCommandMode, "toolBarButtonViewAsCommandMode");
            this.toolBarButtonViewAsCommandMode.Name = "toolBarButtonViewAsCommandMode";
            this.toolBarButtonViewAsCommandMode.Tag = "ViewAsCommandMode";
            // 
            // toolBarButtonViewAsTagMode
            // 
            resources.ApplyResources(this.toolBarButtonViewAsTagMode, "toolBarButtonViewAsTagMode");
            this.toolBarButtonViewAsTagMode.Name = "toolBarButtonViewAsTagMode";
            this.toolBarButtonViewAsTagMode.Tag = "ViewAsTagMode";
            // 
            // toolBarButtonViewAsTimeMode
            // 
            resources.ApplyResources(this.toolBarButtonViewAsTimeMode, "toolBarButtonViewAsTimeMode");
            this.toolBarButtonViewAsTimeMode.Name = "toolBarButtonViewAsTimeMode";
            this.toolBarButtonViewAsTimeMode.Tag = "ViewAsTimeMode";
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButtonEditUndo,
            this.toolBarButtonEditCut,
            this.toolBarButtonEditCopy,
            this.toolBarButtonEditPaste,
            this.toolBarButton6,
            this.toolBarButtonFilePrint,
            this.toolBarButtonPreview,
            this.toolBarButton10,
            this.toolBarButtonInsertTable,
            this.toolBarButtonCellAlignLeft,
            this.toolBarButtonCellAlignCenter,
            this.toolBarButtonCellAlignRight,
            this.toolBarButton15,
            this.toolBarButtonZoomIn,
            this.toolBarButtonZoomOut,
            this.toolBarButtonConfigPrintTime,
            this.toolBarButton2,
            this.toolBarButtonCellTextColor,
            this.toolBarButtonCellBackColor,
            this.toolBarButtonCellBorder,
            this.toolBarButtonCellFont,
            this.toolBarButtonCellSize,
            this.toolBarButton20,
            this.toolBarButtonTextAlignLeft,
            this.toolBarButtonTextAlignCenter,
            this.toolBarButtonTextAlignRight,
            this.toolBarButton24,
            this.toolBarButtonCellChangeCommand,
            this.toolBarButtonCellChangeTag,
            this.toolBarButtonCellChangeTime,
            this.toolBarButtonCellChangeText,
            this.toolBarButton29,
            this.toolBarButtonInsertBasicData,
            this.toolBarButton31,
            this.toolBarButtonViewAsEditMode,
            this.toolBarButtonViewAsRunMode,
            this.toolBarButtonViewAsCommandMode,
            this.toolBarButtonViewAsTagMode,
            this.toolBarButtonViewAsTimeMode});
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(233)))), ((int)(((byte)(216)))));
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            this.imageList1.Images.SetKeyName(27, "");
            this.imageList1.Images.SetKeyName(28, "");
            this.imageList1.Images.SetKeyName(29, "");
            this.imageList1.Images.SetKeyName(30, "");
            // 
            // FormReportMainEdit
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormReportMainEdit";
            this.Activated += new System.EventHandler(this.FormReportMainEdit_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormReportMainEdit_Closing);
            this.Load += new System.EventHandler(this.FormMainEdit_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		void ViewModeChanged()
		{
			PressViewModeButton();

			bool active = (formChild.workView.nViewMode != EnumViewMode.RUN);

			this.toolBarButtonCellAlignCenter.Enabled = active;
			this.toolBarButtonCellAlignLeft.Enabled = active;
			this.toolBarButtonCellAlignRight.Enabled = active;
			this.toolBarButtonCellBackColor.Enabled = active;
			this.toolBarButtonCellBorder.Enabled = active;
			this.toolBarButtonCellChangeCommand.Enabled = active;
			this.toolBarButtonCellChangeTag.Enabled = active;
			this.toolBarButtonCellChangeText.Enabled = active;
			this.toolBarButtonCellChangeTime.Enabled = active;
			this.toolBarButtonCellFont.Enabled = active;
			this.toolBarButtonCellSize.Enabled = active;
			this.toolBarButtonCellTextColor.Enabled = active;
			this.toolBarButtonEditCopy.Enabled = active;
			this.toolBarButtonEditCut.Enabled = active;
			this.toolBarButtonEditPaste.Enabled = active;
			this.toolBarButtonEditUndo.Enabled = active;
			this.toolBarButtonInsertBasicData.Enabled = active;
			this.toolBarButtonInsertTable.Enabled = active;
			this.toolBarButtonTextAlignCenter.Enabled = active;
			this.toolBarButtonTextAlignLeft.Enabled = active;
			this.toolBarButtonTextAlignRight.Enabled = active;
		}

		private void FormMainEdit_Load(object sender, System.EventArgs e)
		{
			formChild.Show();
			ViewModeChanged();
		}

		void MessageBoxCannotRunMode()
		{
			if(Tools.IsLangKorean()) 
			{
				MessageBox.Show("실행 모드에서는 사용할 수 없는 메뉴입니다.", "RUN Mode");
			}
			else if(Tools.IsLangChinese()) 
			{
				MessageBox.Show("在运行模式不可使用的菜单。", "运行模式");
			}
			else 
			{
				MessageBox.Show("Can't use This menu on RUN mode.", "RUN Mode");
			}
		}

		private void menuItemDeleteTable_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemDeleteTable_Click();
		}	

		private void menuItemInsertRow_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemInsertRow_Click();
		}

		private void menuItemDeleteRow_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemDeleteRow_Click();
		}

		private void menuItemInsertColumn_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemInsertColumn_Click();
		}

		private void menuItemDeleteColumn_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemDeleteColumn_Click();
		}

		private void menuItemAlignToLeft_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemAlignToLeft_Click();
		}

		private void menuItemAlignToCenter_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemAlignToCenter_Click();
		}

		private void menuItemAlignToRight_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemAlignToRight_Click();
		}

		private void menuItemInsertTable_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemInsertTable_Click();
		}

		private void menuItemInsertBasicData_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemInsertBasicData_Click();
		}

		private void menuItemInsertFunction_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemInsertFunction_Click();
		}

		private void menuItemConfigStringVars_Click(object sender, System.EventArgs e)
		{
			FormDialogConfigStringVar dialog = new FormDialogConfigStringVar();
            dialog.StartPosition = FormStartPosition.CenterParent;

			dialog.ShowDialog(this);
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch((string)e.Button.Tag)
			{
				case "EditUndo":
					ReportEditorUndo.menuItemEditUndo_Click(formChild);
					break;
				case "EditCut":
					formChild.menuItemEditCut_Click();
					break;
				case "EditCopy":
					formChild.menuItemEditCopy_Click();
					break;
				case "EditPaste":
					formChild.menuItemEditPaste_Click();
					break;
				case "FilePrint":
					formChild.menuItemFilePrint_Click();
					break;
				case "Preview":
					formChild.menuItemFilePreview_Click();
					break;

				case "InsertTable":
					formChild.menuItemInsertTable_Click();
					break;
				case "TableAlignLeft":
					formChild.menuItemTableAlignToLeft_Click();
					break;
				case "TableAlignCenter":
					formChild.menuItemTableAlignToCenter_Click();
					break;
				case "TableAlignRight":
					formChild.menuItemTableAlignToRight_Click();
					break;

				case "ZoomIn":
					formChild.menuItemViewZoomIn_Click();
					break;
				case "ZoomOut":
					formChild.menuItemViewZoomOut_Click();
					break;
				case "ConfigPrintTime":
					formChild.menuItemConfigPrintTime_Click();
					break;

				case "CellTextColor":
					formChild.menuItemFormCellTextColor_Click();
					break;
				case "CellBackColor":
					formChild.menuItemFormCellBackColor_Click();
					break;
				case "CellBorder":
					formChild.menuItemFormCellBorder_Click();
					break;
				case "CellFont":
					formChild.menuItemFormCellFont_Click();
					break;
				case "CellSize":
					formChild.menuItemFormCellSize_Click();
					break;

				case "TextAlignLeft":
					formChild.menuItemAlignToLeft_Click();
					break;
				case "TextAlignCenter":
					formChild.menuItemAlignToCenter_Click();
					break;
				case "TextAlignRight":
					formChild.menuItemAlignToRight_Click();
					break;

				case "CellChangeCommand":
					formChild.menuItemFormCellChangeCommand_Click();
					break;
				case "CellChangeTag":
					formChild.menuItemFormCellChangeTag_Click();
					break;
				case "CellChangeTime":
					formChild.menuItemFormCellChangeTime_Click();
					break;
				case "CellChangeText":
					formChild.menuItemFormCellChangeText_Click();
					break;

				case "InsertBasicData":
					formChild.menuItemInsertBasicData_Click();
					break;

				case "ViewAsEditMode": 
					formChild.menuItemViewAsEditMode_Click();
					ViewModeChanged();
					break;
				case "ViewAsRunMode":
					formChild.menuItemViewAsRunMode_Click();
					ViewModeChanged();
					break;
				case "ViewAsCommandMode":
					formChild.menuItemViewAsCommandMode_Click();
					ViewModeChanged();
					break;
				case "ViewAsTagMode":
					formChild.menuItemViewAsTagMode_Click();
					ViewModeChanged();
					break;
				case "ViewAsTimeMode":
					formChild.menuItemViewAsTimeMode_Click();
					ViewModeChanged();
					break;

			}
		}

		void PressViewModeButton()
		{
			this.toolBarButtonViewAsRunMode.Pushed = (formChild.workView.nViewMode == EnumViewMode.RUN);
			this.toolBarButtonViewAsEditMode.Pushed = (formChild.workView.nViewMode == EnumViewMode.EDIT);
			this.toolBarButtonViewAsCommandMode.Pushed = (formChild.workView.nViewMode == EnumViewMode.COMMAND);
			this.toolBarButtonViewAsTagMode.Pushed = (formChild.workView.nViewMode == EnumViewMode.TAG);
			this.toolBarButtonViewAsTimeMode.Pushed = (formChild.workView.nViewMode == EnumViewMode.TIME);
		}

		private void menuItemEditUndo_Click(object sender, System.EventArgs e)
		{
			ReportEditorUndo.menuItemEditUndo_Click(formChild);
		}

		private void menuItemEditCut_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditCut_Click();
		}

		private void menuItemEditCopy_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditCopy_Click();
		}

		private void menuItemEditPaste_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditPaste_Click();
		}

		private void menuItemEditPasteAsNewTable_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditPasteAsNewTable_Click();
		}

		private void menuItemEditDelete_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditDelete_Click();
		}

		private void menuItemEditHeader_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditHeader_Click();
		}

		private void menuItemEditFooter_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEditFooter_Click();
		}

		private void menuItemEdit_Popup(object sender, System.EventArgs e)
		{
			bool active = ReportCopyPaste.IsPasteEnable();

			if(formChild.workView.nViewMode == EnumViewMode.RUN)
				active = false;
			
			pasteToolStripMenuItem.Enabled = active;
			pasteAsnewTableToolStripMenuItem.Enabled = active;

			string title = "";

			active = ReportEditorUndo.IsPossibleEditUndo(this.formChild, ref title);
			if(formChild.workView.nViewMode == EnumViewMode.RUN)
				active = false;

			undoToolStripMenuItem.Enabled = active;
            undoToolStripMenuItem.Text = title;

			active = ReportEditorUndo.IsPossibleEditRedo(this.formChild, ref title);
			if(formChild.workView.nViewMode == EnumViewMode.RUN)
				active = false;

			redoToolStripMenuItem.Enabled = active;
            redoToolStripMenuItem.Text = title;

			active = (formChild.GetReportStruct().TableCount > 0);

			if(formChild.workView.nViewMode == EnumViewMode.RUN)
				active = false;

			copyToolStripMenuItem.Enabled = active;
			cutToolStripMenuItem.Enabled = active;
			deleteToolStripMenuItem.Enabled = active;

			active = (formChild.workView.nViewMode != EnumViewMode.RUN);
			editFooterToolStripMenuItem.Enabled = active;
			editHeaderToolStripMenuItem.Enabled = active;
		}

		private void menuItemFormCellTextColor_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellTextColor_Click();
		}

		private void menuItemFormCellBackColor_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellBackColor_Click();
		}

		private void menuItemFormCellBorder_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellBorder_Click();
		}

		private void menuItemFormCellFont_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellFont_Click();
		}

		private void menuItemFormCellSize_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellSize_Click();
		}

		private void menuItemFormCellAlign_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellAlign_Click();
		}

		private void menuItemFormCellDisplayFormat_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellDisplayFormat_Click();
		}

		private void menuItemFormCellGroup_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellGroup_Click();
		}

		private void menuItemFormCellUngroup_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellUngroup_Click();
		}

		private void menuItemFormCellChangeCommand_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellChangeCommand_Click();
		}

		private void menuItemFormCellChangeTag_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellChangeTag_Click();
		}

		private void menuItemFormCellChangeTime_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellChangeTime_Click();
		}

		private void menuItemFormCellChangeText_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFormCellChangeText_Click();
		}

		private void menuItemViewZoomIn_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewZoomIn_Click();
		}

		private void menuItemViewZoomOut_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewZoomOut_Click();
		}

		private void menuItemViewAsEditMode_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewAsEditMode_Click();
			ViewModeChanged();
		}

		private void menuItemViewAsRunMode_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewAsRunMode_Click();
			ViewModeChanged();
		}

		private void menuItemViewAsCommandMode_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewAsCommandMode_Click();
			ViewModeChanged();
		}

		private void menuItemViewAsTagMode_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewAsTagMode_Click();
			ViewModeChanged();
		}

		private void menuItemViewAsTimeMode_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewAsTimeMode_Click();
			ViewModeChanged();
		}

		private void menuItemConfigPaperColor_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemConfigPaperColor_Click();
		}

		private void menuItemConfigPrintTime_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemConfigPrintTime_Click();
		}

		private void menuItemConfigPrintCycle_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemConfigPrintCycle_Click();
		}

		private void menuItemConfigMinListTime_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemConfigMinListTime_Click();
		}

		private void menuItemWindowCascade_Click(object sender, System.EventArgs e)
		{
			this.ParentForm.LayoutMdi(MdiLayout.Cascade);
		}

		private void menuItemWindowTileHorz_Click(object sender, System.EventArgs e)
		{
			this.ParentForm.LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void menuItemWindowTileVert_Click(object sender, System.EventArgs e)
		{
			this.ParentForm.LayoutMdi(MdiLayout.TileVertical);
		}

		private void menuItemWindowArrangeIcon_Click(object sender, System.EventArgs e)
		{
			this.ParentForm.LayoutMdi(MdiLayout.ArrangeIcons);
		}

		private void menuItemWindowClose_Click(object sender, System.EventArgs e)
		{
			if(this.ParentForm.ActiveMdiChild != null)
				this.ParentForm.ActiveMdiChild.Close();
		}

		private void menuItemWindowCloseAll_Click(object sender, System.EventArgs e)
		{
			Form[] childForm = this.ParentForm.MdiChildren;
			//Make sure to ask for saving the doc before exiting the app 

			for(int i=0; i < childForm.Length ; i++) 
				childForm[i].Close(); 
		}

		public delegate void CallBackOnMdiActivated(Form form, string filename, object obj);
		public CallBackOnMdiActivated lpfnCallOnMdiActivated;

		private void FormReportMainEdit_Activated(object sender, System.EventArgs e)
		{
			statusBarMain.Panels.Clear();
			this.statusBarMain.Panels.Add(statusBarPanelMain);

			this.lpfnCallOnMdiActivated(this.formChild, this.formChild.sFilename, null);
		}

		private void menuItemFileSave_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFileSave_Click();
		}

		private void menuItemFileSaveAs_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFileSaveAs_Click();
		}

		private void menuItemHelpReportFileVersion_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuItemFileSaveResult_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFileSaveResult_Click();
		}

		private void menuItemFilePrint_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFilePrint_Click();
		}

		private void menuItemFilePreview_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFilePreview_Click();
		}

		private void menuItemFileConfigPaper_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFileConfigPaper_Click();
		}

		private void menuItemFilePrintMultiDate_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFilePrintMultiDate_Click();
		}

		private void menuItemFileInformation_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemFileInformation_Click();
		}

		public delegate void CallBackOnPopupView();
		CallBackOnPopupView lpfnCallBackOnPopupView;

		private void menuItemView_Popup(object sender, System.EventArgs e)
		{
			viewAsEditModeToolStripMenuItem.Checked = (formChild.workView.nViewMode == EnumViewMode.EDIT);
			viewAsRunModeToolStripMenuItem.Checked = (formChild.workView.nViewMode == EnumViewMode.RUN);
			viewAsCommandModeToolStripMenuItem.Checked = (formChild.workView.nViewMode == EnumViewMode.COMMAND);
			viewAsTagNameModeToolStripMenuItem.Checked = (formChild.workView.nViewMode == EnumViewMode.TAG);
			viewAsTimeModeToolStripMenuItem.Checked = (formChild.workView.nViewMode == EnumViewMode.TIME);

			lpfnCallBackOnPopupView();
		}

		private void menuItemTableAlignToLeft_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemTableAlignToLeft_Click();
		}

		private void menuItemTableAlignToCenter_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemTableAlignToCenter_Click();
		}

		private void menuItemTableAlignToRight_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemTableAlignToRight_Click();
		}

		private void FormReportMainEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(formChild.bChangedFlag) 
			{
				string msg;
				DialogResult result;
				if(Tools.IsLangKorean()) 
				{
					msg = String.Format("{0} 파일을 저장하지 않았습니다.\n파일을 저장할까요?", formChild.sFilename);
					result = MessageBox.Show(msg, "저장확인", MessageBoxButtons.YesNoCancel);
				}
				else if(Tools.IsLangChinese()) 
				{
					msg = String.Format("{0} 文件还没保存。\n想保存文件吗？", formChild.sFilename);
					result = MessageBox.Show(msg, "保存确认", MessageBoxButtons.YesNoCancel);
				}
				else 
				{
					msg = String.Format("{0} File not saved.\nDo you want to save changes to file?", formChild.sFilename);
					result = MessageBox.Show(msg, "File not saved", MessageBoxButtons.YesNoCancel);
				}

				if(result == DialogResult.Yes) 
				{
					if(!formChild.menuItemFileSave_Click())
						e.Cancel = true;
				}
				else if(result == DialogResult.Cancel) 
				{
					e.Cancel = true;	
				}
				else {}
			}
		}

		private void contextMenu1_Popup(object sender, System.EventArgs e)
		{
			bool active = (formChild.GetReportStruct().TableCount > 0);
			
			this.menuItemPopupCellAlign.Enabled = active;
			this.menuItemPopupCellBackColor.Enabled = active;
			this.menuItemPopupCellDisplayFormat.Enabled = active;
			this.menuItemPopupCellFont.Enabled = active;
			this.menuItemPopupCellGroup.Enabled = active;
			this.menuItemPopupCellLine.Enabled = active;
			this.menuItemPopupCellSize.Enabled = active;
			this.menuItemPopupCellTextColor.Enabled = active;
			this.menuItemPopupCellUngroup.Enabled = active;
			this.menuItemPopupEditCopy.Enabled = active;
			this.menuItemPopupEditCut.Enabled = active;
			this.menuItemPopupEditDelete.Enabled = active;
			this.menuItemPopupEditPaste.Enabled = active;
		}

		private void menuItem1_Popup(object sender, System.EventArgs e)
		{
			bool active = (formChild.GetReportStruct().TableCount > 0);

			if(formChild.workView.nViewMode == EnumViewMode.RUN)	// 실행모드에서는 셀속성을 바꾸지 못한다.
				active = false;

			alignToolStripMenuItem.Enabled = active;
			backColorToolStripMenuItem.Enabled = active;
			displayFormatToolStripMenuItem.Enabled = active;
			fontToolStripMenuItem.Enabled = active;
			groupToolStripMenuItem.Enabled = active;
			borderToolStripMenuItem.Enabled = active;
			sizeToolStripMenuItem.Enabled = active;
			textColorToolStripMenuItem.Enabled = active;
			ungroupToolStripMenuItem.Enabled = active;
			cellCommandToolStripMenuItem.Enabled = active;
			cellTagToolStripMenuItem.Enabled = active;
			cellTextToolStripMenuItem.Enabled = active;
			cellTimeToolStripMenuItem.Enabled = active;
		}

		private void menuItemConfigEtc_Click(object sender, System.EventArgs e)
		{
			FormDialogConfigEtc dialog = new FormDialogConfigEtc();
            dialog.StartPosition = FormStartPosition.CenterParent;

			dialog.ShowDialog(this);
		}

		private void menuItemEditRedo_Click(object sender, System.EventArgs e)
		{
			ReportEditorUndo.menuItemEditRedo_Click(formChild);
		}

		private void menuItem3_Popup(object sender, System.EventArgs e)
		{
			basicDataToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			functionToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
		}

		private void menuItemTable_Popup(object sender, System.EventArgs e)
		{
			insertTableToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			deleteTableToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			insertRowToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			deleteRowToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			insertColumnToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			deleteColumnToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			alignToLeftToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			alignToRightToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
			alignToCenterToolStripMenuItem.Enabled = (formChild.workView.nViewMode != EnumViewMode.RUN);
		}

		private void menuItemConfig_Popup(object sender, System.EventArgs e)
		{
			paperColorToolStripMenuItem.Enabled =  (formChild.workView.nViewMode != EnumViewMode.RUN);
		}

        private void windowToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                arraygeIconToolStripMenuItem.Visible = false;
            }
        }
	}
}
