using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using AutoLib;
using AutoLibLocal;
using NetTools.OldDefine;
using NetTools;
using System.Drawing.Drawing2D;
using System.IO;
 
namespace Studio
{
	/// <summary>
	/// Summary description for FormEditGraphic.
	/// </summary>
	public class FormEditGraphicFrame : System.Windows.Forms.Form 
	{
		private System.ComponentModel.IContainer components = null;

        static public ArrayList arrayFormGraphFrame = new ArrayList();

		StatusBar statusBarMain;
		public StatusBarPanel statusBarPanelPosition = new StatusBarPanel();
        public StatusBarPanel statusBarPanelSize = new StatusBarPanel();
        private System.Windows.Forms.Panel panel1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonMove;
        private ToolStripButton toolStripButtonLine;
        private ToolStripButton toolStripButtonRectangle;
        private ToolStripButton toolStripButtonFillRectangle;
        private ToolStripButton toolStripButtonCircle;
        private ToolStripButton toolStripButtonFillCircle;
        private ToolStripButton toolStripButtonPoly;
        private ToolStripButton toolStripButtonFillPoly;
        private ToolStripButton toolStripButtonText;
        private ToolStripButton toolStripButtonRoundRectangle;
        private ToolStripButton toolStripButtonCurve;
        private ToolStripButton toolStripButtonPointMove;
        private ToolStripButton toolStripButtonSpoid;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonAlignToLeft;
        private ToolStripButton toolStripButtonAlignHorizontalCenter;
        private ToolStripButton toolStripButtonAlignToRight;
        private ToolStripButton toolStripButtonInsertFromLibrary;
        private ToolStripButton toolStripButtonInsertFromLibrarySearch;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonSpaceHorizontal;
        private ToolStripButton toolStripButtonSpaceVertical;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem saveAsBitmapToolStripMenuItem;
        private ToolStripMenuItem saveAsTempleteLibraryToolStripMenuItem;
        private ToolStripMenuItem saveAsTempleteFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem findAndReplaceToolStripMenuItem;
        private ToolStripMenuItem linkedTagToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem orderToolStripMenuItem;
        private ToolStripMenuItem moveToFrontToolStripMenuItem;
        private ToolStripMenuItem moveToBackToolStripMenuItem;
        private ToolStripMenuItem moveToPrevFrontToolStripMenuItem;
        private ToolStripMenuItem moveToNextBackToolStripMenuItem;
        private ToolStripMenuItem alignToolStripMenuItem;
        private ToolStripMenuItem alignToLeftToolStripMenuItem;
        private ToolStripMenuItem alignToHorizontalCenterToolStripMenuItem;
        private ToolStripMenuItem alignToRightToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem alignToTopToolStripMenuItem;
        private ToolStripMenuItem alignToVerticalCenterToolStripMenuItem;
        private ToolStripMenuItem alignToBottomToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem alignToModuleHorizontalCenterToolStripMenuItem;
        private ToolStripMenuItem alignToModuleVerticalCenterToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem spaceHorizontalToolStripMenuItem;
        private ToolStripMenuItem spaceVerticalToolStripMenuItem;
        private ToolStripMenuItem flipRotateToolStripMenuItem;
        private ToolStripMenuItem flipHorizontalToolStripMenuItem;
        private ToolStripMenuItem flipVerticalToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem rotate90DegreeToRightToolStripMenuItem;
        private ToolStripMenuItem rotate90DegreeToLeftToolStripMenuItem;
        private ToolStripMenuItem makeSameSizeToolStripMenuItem;
        private ToolStripMenuItem widthToolStripMenuItem;
        private ToolStripMenuItem heightToolStripMenuItem;
        private ToolStripMenuItem allToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem groupToolStripMenuItem;
        private ToolStripMenuItem ungroupToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem lockToolStripMenuItem;
        private ToolStripMenuItem unlockAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem registerToLibraryToolStripMenuItem;
        private ToolStripMenuItem saveToLibraryFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem objectPropertiesToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripMenuItem zoomToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemPercent10;
        private ToolStripMenuItem toolStripMenuItemPercent25;
        private ToolStripMenuItem toolStripMenuItemPercent50;
        private ToolStripMenuItem toolStripMenuItemPercent75;
        private ToolStripMenuItem toolStripMenuItemPercent100;
        private ToolStripMenuItem toolStripMenuItemPercent150;
        private ToolStripMenuItem toolStripMenuItemPercent200;
        private ToolStripMenuItem toolStripMenuItemPercent300;
        private ToolStripMenuItem toolStripMenuItemPercent400;
        private ToolStripMenuItem toolStripMenuItemPercent500;
        private ToolStripMenuItem toolStripMenuItemPercent600;
        private ToolStripMenuItem toolStripMenuItemPercent700;
        private ToolStripMenuItem toolStripMenuItemPercent800;
        private ToolStripMenuItem toolStripMenuItemPercent900;
        private ToolStripMenuItem toolStripMenuItemPercent1000;
        private ToolStripMenuItem objectNumberToolStripMenuItem;
        private ToolStripMenuItem objectToolStripMenuItem;
        private ToolStripMenuItem buttonToolStripMenuItem;
        private ToolStripMenuItem moduleSelectionButtonToolStripMenuItem;
        private ToolStripMenuItem moduleHideButtonToolStripMenuItem;
        private ToolStripMenuItem scriptButtonToolStripMenuItem;
        private ToolStripMenuItem digitalOutputButtonToolStripMenuItem;
        private ToolStripMenuItem digitalToolStripMenuItem;
        private ToolStripMenuItem digitalAnimationToolStripMenuItem;
        private ToolStripMenuItem digitalCircleToolStripMenuItem;
        private ToolStripMenuItem digitalRectangleToolStripMenuItem;
        private ToolStripMenuItem digitalStringToolStripMenuItem;
        private ToolStripMenuItem analogToolStripMenuItem;
        private ToolStripMenuItem analogRectangleToolStripMenuItem;
        private ToolStripMenuItem analogStringToolStripMenuItem;
        private ToolStripMenuItem analogMeterToolStripMenuItem;
        private ToolStripMenuItem analogStatusToolStripMenuItem;
        private ToolStripMenuItem analogRotateToolStripMenuItem;
        private ToolStripMenuItem stringTagToolStripMenuItem;
        private ToolStripMenuItem tagAnimationToolStripMenuItem;
        private ToolStripMenuItem displayChangesToolStripMenuItem;
        private ToolStripMenuItem graphicModuleToolStripMenuItem;
        private ToolStripMenuItem alarmWindowToolStripMenuItem;
        private ToolStripMenuItem windowControlToolStripMenuItem;
        private ToolStripMenuItem listBoxToolStripMenuItem;
        private ToolStripMenuItem comboBoxToolStripMenuItem;
        private ToolStripMenuItem editBoxToolStripMenuItem;
        private ToolStripMenuItem radioButtonToolStripMenuItem;
        private ToolStripMenuItem checkBoxToolStripMenuItem;
        private ToolStripMenuItem datePickerToolStripMenuItem;
        private ToolStripMenuItem graphTrendToolStripMenuItem;
        private ToolStripMenuItem multigraphToolStripMenuItem;
        private ToolStripMenuItem multitrendToolStripMenuItem;
        private ToolStripSeparator toolStripSeparatorChart;
        private ToolStripMenuItem milliDataWindowToolStripMenuItem;
        private ToolStripMenuItem milliDataTrendToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripMenuItem demandWindowToolStripMenuItem;
        private ToolStripMenuItem realtimeTestGraphToolStripMenuItem;
        private ToolStripMenuItem xYGraphToolStripMenuItem;
        private ToolStripMenuItem databaseTrendToolStripMenuItem;
        private ToolStripMenuItem databaseToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem bitmapToolStripMenuItem;
        private ToolStripMenuItem animationToolStripMenuItem;
        private ToolStripMenuItem singleLineTextToolStripMenuItem;
        private ToolStripMenuItem rectangleToolStripMenuItem;
        private ToolStripMenuItem circleToolStripMenuItem;
        private ToolStripMenuItem lineToolStripMenuItem;
        private ToolStripMenuItem roundedRectangleToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem textToolStripMenuItem;
        private ToolStripMenuItem clockToolStripMenuItem;
        private ToolStripMenuItem dateToolStripMenuItem;
        private ToolStripMenuItem webBrowserToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripMenuItem objectLibraryToolStripMenuItem;
        private ToolStripMenuItem buyObjectLibraryFromWebToolStripMenuItem;
        private ToolStripMenuItem libraryFileToolStripMenuItem;
        private ToolStripMenuItem etcToolStripMenuItem;
        private ToolStripMenuItem backgroundBitmapToolStripMenuItem;
        private ToolStripMenuItem backgroundColorToolStripMenuItem;
        private ToolStripMenuItem modulePropertiesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripMenuItem scriptOnModuleOpenedToolStripMenuItem;
        private ToolStripMenuItem scriptOnModuleRunningToolStripMenuItem;
        private ToolStripMenuItem scriptOnModuleClosedToolStripMenuItem;
        private ToolStripMenuItem scriptOnModuleActivatedToolStripMenuItem;
        private ToolStripMenuItem scriptOnModuleDeactivatedToolStripMenuItem;
        private ToolStripMenuItem configToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem guideLineToolStripMenuItem;
        private ToolStripMenuItem windowToolStripMenuItem;
        private ToolStripMenuItem cascadeToolStripMenuItem;
        private ToolStripMenuItem tileHorizontallyToolStripMenuItem;
        private ToolStripMenuItem tileVerticallyToolStripMenuItem;
        private ToolStripMenuItem arraygeIconToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem closeAllToolStripMenuItem;
        private ToolStripMenuItem dataGridViewToolStripMenuItem;
        private ToolStripMenuItem tabControlToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripMenuItem zoomInToolStripMenuItem;
        private ToolStripMenuItem zoomOutToolStripMenuItem1;
        private ToolStripMenuItem treeViewToolStripMenuItem;
        private ToolStripMenuItem webViewToolStripMenuItem;
        private ToolStripMenuItem VLCAxToolStripMenuItem;
        private ToolStripMenuItem autopasteToolStripMenuItem;
        private ToolStripMenuItem svgToolStripMenuItem;
        private ToolStripButton toolStripButtonAlignToTop;
        private ToolStripButton toolStripButtonAlignToBottom;
        private ToolStripButton toolStripButtonAlignVerticalCenter;
        private ToolStripButton toolStripButtonMakeSameSizeWidth;
        private ToolStripButton toolStripButtonMakeSameSizeHeight;
        private ToolStripButton toolStripButtonMoveToPrevFront;
        private ToolStripButton toolStripButtonMoveToNextBack;
        private ToolStripButton toolStripButtonRotateLeft;
        private ToolStripButton toolStripButtonRotateRight;
        private ToolStripSeparator toolStripSeparator25;
        private ToolStripMenuItem analogGaugeToolStripMenuItem;
        private ToolStripMenuItem DonutChartToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemClipboardPaste;
        private ToolStripMenuItem chartToolStripMenuItem;
        private ToolStripMenuItem demandChartToolStripMenuItem;
        private ToolStripMenuItem barcodeDisplayToolStripMenuItem;
        private ToolStripMenuItem barcodeScannerToolStripMenuItem;
        public FormEditGraphic formChild;

		public FormEditGraphicFrame(string filename, int new_flag, StatusBar status_bar, CallBackOnMdiActivated callbackmdi, ObjectRoot root)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("Z_IoT_Studio.ico", 16, 16);
                if (icon != null)
                    this.Icon = icon;
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                this.arraygeIconToolStripMenuItem.Visible = false;

                if (Tools.IsLangKorean())
                {
                    this.flipRotateToolStripMenuItem.Text = "위치교환/크기반전(&I)";
                    this.flipHorizontalToolStripMenuItem.Text = "좌우 위치교환(&H)";
                    this.flipVerticalToolStripMenuItem.Text = "상하 위치교환(&V)";
                    this.rotate90DegreeToLeftToolStripMenuItem.Text = "왼쪽으로 90도 크기반전(&L)";
                    this.rotate90DegreeToRightToolStripMenuItem.Text = "오른쪽으로 90도 크기반전(&R)";
                }

                this.toolStripSeparator10.Visible = false;
                this.spaceHorizontalToolStripMenuItem.Visible = false;
                this.spaceVerticalToolStripMenuItem.Visible = false;

                this.toolStripButtonSpaceHorizontal.Visible = false;
                this.toolStripButtonSpaceVertical.Visible = false;
            }
            
            
			statusBarMain = status_bar;

			lpfnCallOnMdiActivated = callbackmdi;

			formChild = new FormEditGraphic(filename, new_flag, this, root);
			formChild.TopLevel = false;
			formChild.FormBorderStyle = FormBorderStyle.None;
			formChild.Dock = DockStyle.Fill;
			this.panel1.Controls.Add(formChild);
			formChild.Show();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditGraphicFrame));
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsTempleteLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsTempleteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autopasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClipboardPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.findAndReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkedTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.orderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToPrevFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToNextBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToHorizontalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.alignToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToVerticalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.alignToModuleHorizontalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToModuleVerticalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.spaceHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spaceVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipRotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.rotate90DegreeToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90DegreeToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeSameSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.widthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ungroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.lockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unlockAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.registerToLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToLibraryFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.objectPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent25 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent50 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent75 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent100 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent150 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent200 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent300 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent400 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent500 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent600 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent700 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent800 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent900 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPercent1000 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.objectNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moduleSelectionButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moduleHideButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalOutputButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalCircleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalRectangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.digitalStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogRectangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogMeterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogRotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogGaugeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stringTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alarmWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radioButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.datePickerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphTrendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multigraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multitrendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorChart = new System.Windows.Forms.ToolStripSeparator();
            this.milliDataWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.milliDataTrendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.demandWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.demandChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realtimeTestGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xYGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseTrendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DonutChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.bitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.svgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleLineTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.circleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundedRectangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VLCAxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barcodeDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barcodeScannerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.objectLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buyObjectLibraryFromWebToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.etcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modulePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.scriptOnModuleOpenedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptOnModuleRunningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptOnModuleClosedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptOnModuleActivatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptOnModuleDeactivatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.guideLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arraygeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLine = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRectangle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFillRectangle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCircle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFillCircle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPoly = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFillPoly = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonText = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRoundRectangle = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCurve = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPointMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSpoid = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonInsertFromLibrary = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInsertFromLibrarySearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAlignToLeft = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignHorizontalCenter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignToRight = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSpaceHorizontal = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSpaceVertical = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignToTop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignVerticalCenter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignToBottom = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonMakeSameSizeWidth = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMakeSameSizeHeight = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveToPrevFront = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveToNextBack = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotateRight = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotateLeft = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.objectToolStripMenuItem,
            this.etcToolStripMenuItem,
            this.configToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveAsBitmapToolStripMenuItem,
            this.saveAsTempleteLibraryToolStripMenuItem,
            this.saveAsTempleteFileToolStripMenuItem,
            this.toolStripSeparator4,
            this.importToolStripMenuItem});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.MergeIndex = 0;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator3.MergeIndex = 8;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
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
            // saveAsBitmapToolStripMenuItem
            // 
            this.saveAsBitmapToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsBitmapToolStripMenuItem.MergeIndex = 11;
            this.saveAsBitmapToolStripMenuItem.Name = "saveAsBitmapToolStripMenuItem";
            resources.ApplyResources(this.saveAsBitmapToolStripMenuItem, "saveAsBitmapToolStripMenuItem");
            this.saveAsBitmapToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileSaveAsBitmap_Click);
            // 
            // saveAsTempleteLibraryToolStripMenuItem
            // 
            this.saveAsTempleteLibraryToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsTempleteLibraryToolStripMenuItem.MergeIndex = 12;
            this.saveAsTempleteLibraryToolStripMenuItem.Name = "saveAsTempleteLibraryToolStripMenuItem";
            resources.ApplyResources(this.saveAsTempleteLibraryToolStripMenuItem, "saveAsTempleteLibraryToolStripMenuItem");
            this.saveAsTempleteLibraryToolStripMenuItem.Click += new System.EventHandler(this.menuItemSaveAsTemplete_Click);
            // 
            // saveAsTempleteFileToolStripMenuItem
            // 
            this.saveAsTempleteFileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsTempleteFileToolStripMenuItem.MergeIndex = 13;
            this.saveAsTempleteFileToolStripMenuItem.Name = "saveAsTempleteFileToolStripMenuItem";
            resources.ApplyResources(this.saveAsTempleteFileToolStripMenuItem, "saveAsTempleteFileToolStripMenuItem");
            this.saveAsTempleteFileToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileSaveAsTempleteFile_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator4.MergeIndex = 14;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.importToolStripMenuItem.MergeIndex = 15;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            resources.ApplyResources(this.importToolStripMenuItem, "importToolStripMenuItem");
            this.importToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileImport_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator5,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.autopasteToolStripMenuItem,
            this.toolStripMenuItemClipboardPaste,
            this.deleteToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator6,
            this.findAndReplaceToolStripMenuItem,
            this.toolStripSeparator7,
            this.orderToolStripMenuItem,
            this.alignToolStripMenuItem,
            this.flipRotateToolStripMenuItem,
            this.makeSameSizeToolStripMenuItem,
            this.toolStripSeparator12,
            this.groupToolStripMenuItem,
            this.ungroupToolStripMenuItem,
            this.toolStripSeparator13,
            this.lockToolStripMenuItem,
            this.unlockAllToolStripMenuItem,
            this.toolStripSeparator14,
            this.registerToLibraryToolStripMenuItem,
            this.saveToLibraryFileToolStripMenuItem,
            this.toolStripSeparator15,
            this.objectPropertiesToolStripMenuItem});
            this.editToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.editToolStripMenuItem.MergeIndex = 1;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.DropDownClosed += new System.EventHandler(this.editToolStripMenuItem_DropDownClosed);
            this.editToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemEdit_Popup);
            this.editToolStripMenuItem.Click += new System.EventHandler(this.menuItemEdit_Click);
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
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
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
            // autopasteToolStripMenuItem
            // 
            this.autopasteToolStripMenuItem.Name = "autopasteToolStripMenuItem";
            resources.ApplyResources(this.autopasteToolStripMenuItem, "autopasteToolStripMenuItem");
            this.autopasteToolStripMenuItem.Click += new System.EventHandler(this.autopasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItemClipboardPaste
            // 
            this.toolStripMenuItemClipboardPaste.Name = "toolStripMenuItemClipboardPaste";
            resources.ApplyResources(this.toolStripMenuItemClipboardPaste, "toolStripMenuItemClipboardPaste");
            this.toolStripMenuItemClipboardPaste.Click += new System.EventHandler(this.toolStripMenuItemClipboardPaste_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditDelete_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditSelectAll_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // findAndReplaceToolStripMenuItem
            // 
            this.findAndReplaceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linkedTagToolStripMenuItem});
            this.findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            resources.ApplyResources(this.findAndReplaceToolStripMenuItem, "findAndReplaceToolStripMenuItem");
            // 
            // linkedTagToolStripMenuItem
            // 
            this.linkedTagToolStripMenuItem.Name = "linkedTagToolStripMenuItem";
            resources.ApplyResources(this.linkedTagToolStripMenuItem, "linkedTagToolStripMenuItem");
            this.linkedTagToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditFindReplaceTagLink_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // orderToolStripMenuItem
            // 
            this.orderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToFrontToolStripMenuItem,
            this.moveToBackToolStripMenuItem,
            this.moveToPrevFrontToolStripMenuItem,
            this.moveToNextBackToolStripMenuItem});
            this.orderToolStripMenuItem.Name = "orderToolStripMenuItem";
            resources.ApplyResources(this.orderToolStripMenuItem, "orderToolStripMenuItem");
            // 
            // moveToFrontToolStripMenuItem
            // 
            this.moveToFrontToolStripMenuItem.Name = "moveToFrontToolStripMenuItem";
            resources.ApplyResources(this.moveToFrontToolStripMenuItem, "moveToFrontToolStripMenuItem");
            this.moveToFrontToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMoveToFront_Click);
            // 
            // moveToBackToolStripMenuItem
            // 
            this.moveToBackToolStripMenuItem.Name = "moveToBackToolStripMenuItem";
            resources.ApplyResources(this.moveToBackToolStripMenuItem, "moveToBackToolStripMenuItem");
            this.moveToBackToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMoveToBack_Click);
            // 
            // moveToPrevFrontToolStripMenuItem
            // 
            this.moveToPrevFrontToolStripMenuItem.Name = "moveToPrevFrontToolStripMenuItem";
            resources.ApplyResources(this.moveToPrevFrontToolStripMenuItem, "moveToPrevFrontToolStripMenuItem");
            this.moveToPrevFrontToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMoveToPrevFront_Click);
            // 
            // moveToNextBackToolStripMenuItem
            // 
            this.moveToNextBackToolStripMenuItem.Name = "moveToNextBackToolStripMenuItem";
            resources.ApplyResources(this.moveToNextBackToolStripMenuItem, "moveToNextBackToolStripMenuItem");
            this.moveToNextBackToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMoveToNextBack_Click);
            // 
            // alignToolStripMenuItem
            // 
            this.alignToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignToLeftToolStripMenuItem,
            this.alignToHorizontalCenterToolStripMenuItem,
            this.alignToRightToolStripMenuItem,
            this.toolStripSeparator8,
            this.alignToTopToolStripMenuItem,
            this.alignToVerticalCenterToolStripMenuItem,
            this.alignToBottomToolStripMenuItem,
            this.toolStripSeparator9,
            this.alignToModuleHorizontalCenterToolStripMenuItem,
            this.alignToModuleVerticalCenterToolStripMenuItem,
            this.toolStripSeparator10,
            this.spaceHorizontalToolStripMenuItem,
            this.spaceVerticalToolStripMenuItem});
            this.alignToolStripMenuItem.Name = "alignToolStripMenuItem";
            resources.ApplyResources(this.alignToolStripMenuItem, "alignToolStripMenuItem");
            this.alignToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemEditAlignPopup_Popup);
            // 
            // alignToLeftToolStripMenuItem
            // 
            this.alignToLeftToolStripMenuItem.Name = "alignToLeftToolStripMenuItem";
            resources.ApplyResources(this.alignToLeftToolStripMenuItem, "alignToLeftToolStripMenuItem");
            this.alignToLeftToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToLeft_Click);
            // 
            // alignToHorizontalCenterToolStripMenuItem
            // 
            this.alignToHorizontalCenterToolStripMenuItem.Name = "alignToHorizontalCenterToolStripMenuItem";
            resources.ApplyResources(this.alignToHorizontalCenterToolStripMenuItem, "alignToHorizontalCenterToolStripMenuItem");
            this.alignToHorizontalCenterToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToHorzCenter_Click);
            // 
            // alignToRightToolStripMenuItem
            // 
            this.alignToRightToolStripMenuItem.Name = "alignToRightToolStripMenuItem";
            resources.ApplyResources(this.alignToRightToolStripMenuItem, "alignToRightToolStripMenuItem");
            this.alignToRightToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToRight_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // alignToTopToolStripMenuItem
            // 
            this.alignToTopToolStripMenuItem.Name = "alignToTopToolStripMenuItem";
            resources.ApplyResources(this.alignToTopToolStripMenuItem, "alignToTopToolStripMenuItem");
            this.alignToTopToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToTop_Click);
            // 
            // alignToVerticalCenterToolStripMenuItem
            // 
            this.alignToVerticalCenterToolStripMenuItem.Name = "alignToVerticalCenterToolStripMenuItem";
            resources.ApplyResources(this.alignToVerticalCenterToolStripMenuItem, "alignToVerticalCenterToolStripMenuItem");
            this.alignToVerticalCenterToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToVertCenter_Click);
            // 
            // alignToBottomToolStripMenuItem
            // 
            this.alignToBottomToolStripMenuItem.Name = "alignToBottomToolStripMenuItem";
            resources.ApplyResources(this.alignToBottomToolStripMenuItem, "alignToBottomToolStripMenuItem");
            this.alignToBottomToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToBottom_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // alignToModuleHorizontalCenterToolStripMenuItem
            // 
            this.alignToModuleHorizontalCenterToolStripMenuItem.Name = "alignToModuleHorizontalCenterToolStripMenuItem";
            resources.ApplyResources(this.alignToModuleHorizontalCenterToolStripMenuItem, "alignToModuleHorizontalCenterToolStripMenuItem");
            this.alignToModuleHorizontalCenterToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToModuleHorzCenter_Click);
            // 
            // alignToModuleVerticalCenterToolStripMenuItem
            // 
            this.alignToModuleVerticalCenterToolStripMenuItem.Name = "alignToModuleVerticalCenterToolStripMenuItem";
            resources.ApplyResources(this.alignToModuleVerticalCenterToolStripMenuItem, "alignToModuleVerticalCenterToolStripMenuItem");
            this.alignToModuleVerticalCenterToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditAlignToModuleVertCenter_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // spaceHorizontalToolStripMenuItem
            // 
            this.spaceHorizontalToolStripMenuItem.Name = "spaceHorizontalToolStripMenuItem";
            resources.ApplyResources(this.spaceHorizontalToolStripMenuItem, "spaceHorizontalToolStripMenuItem");
            this.spaceHorizontalToolStripMenuItem.Click += new System.EventHandler(this.menuItemSpaceHorizontal_Click);
            // 
            // spaceVerticalToolStripMenuItem
            // 
            this.spaceVerticalToolStripMenuItem.Name = "spaceVerticalToolStripMenuItem";
            resources.ApplyResources(this.spaceVerticalToolStripMenuItem, "spaceVerticalToolStripMenuItem");
            this.spaceVerticalToolStripMenuItem.Click += new System.EventHandler(this.menuItemSpaceVertical_Click);
            // 
            // flipRotateToolStripMenuItem
            // 
            this.flipRotateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flipHorizontalToolStripMenuItem,
            this.flipVerticalToolStripMenuItem,
            this.toolStripSeparator11,
            this.rotate90DegreeToRightToolStripMenuItem,
            this.rotate90DegreeToLeftToolStripMenuItem});
            this.flipRotateToolStripMenuItem.Name = "flipRotateToolStripMenuItem";
            resources.ApplyResources(this.flipRotateToolStripMenuItem, "flipRotateToolStripMenuItem");
            this.flipRotateToolStripMenuItem.DropDownOpened += new System.EventHandler(this.flipRotateToolStripMenuItem_DropDownOpened);
            this.flipRotateToolStripMenuItem.Click += new System.EventHandler(this.flipRotateToolStripMenuItem_Click);
            // 
            // flipHorizontalToolStripMenuItem
            // 
            this.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem";
            resources.ApplyResources(this.flipHorizontalToolStripMenuItem, "flipHorizontalToolStripMenuItem");
            this.flipHorizontalToolStripMenuItem.Click += new System.EventHandler(this.menuItemFlipHorizontal_Click);
            // 
            // flipVerticalToolStripMenuItem
            // 
            this.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem";
            resources.ApplyResources(this.flipVerticalToolStripMenuItem, "flipVerticalToolStripMenuItem");
            this.flipVerticalToolStripMenuItem.Click += new System.EventHandler(this.menuItemFlipVertical_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // rotate90DegreeToRightToolStripMenuItem
            // 
            this.rotate90DegreeToRightToolStripMenuItem.Name = "rotate90DegreeToRightToolStripMenuItem";
            resources.ApplyResources(this.rotate90DegreeToRightToolStripMenuItem, "rotate90DegreeToRightToolStripMenuItem");
            this.rotate90DegreeToRightToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditRotateRight_Click);
            // 
            // rotate90DegreeToLeftToolStripMenuItem
            // 
            this.rotate90DegreeToLeftToolStripMenuItem.Name = "rotate90DegreeToLeftToolStripMenuItem";
            resources.ApplyResources(this.rotate90DegreeToLeftToolStripMenuItem, "rotate90DegreeToLeftToolStripMenuItem");
            this.rotate90DegreeToLeftToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditRotateLeft_Click);
            // 
            // makeSameSizeToolStripMenuItem
            // 
            this.makeSameSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.widthToolStripMenuItem,
            this.heightToolStripMenuItem,
            this.allToolStripMenuItem});
            this.makeSameSizeToolStripMenuItem.Name = "makeSameSizeToolStripMenuItem";
            resources.ApplyResources(this.makeSameSizeToolStripMenuItem, "makeSameSizeToolStripMenuItem");
            // 
            // widthToolStripMenuItem
            // 
            this.widthToolStripMenuItem.Name = "widthToolStripMenuItem";
            resources.ApplyResources(this.widthToolStripMenuItem, "widthToolStripMenuItem");
            this.widthToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMakeSameSizeWidth_Click);
            // 
            // heightToolStripMenuItem
            // 
            this.heightToolStripMenuItem.Name = "heightToolStripMenuItem";
            resources.ApplyResources(this.heightToolStripMenuItem, "heightToolStripMenuItem");
            this.heightToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMakeSameSizeHeight_Click);
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            resources.ApplyResources(this.allToolStripMenuItem, "allToolStripMenuItem");
            this.allToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditMakeSameSizeAll_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // groupToolStripMenuItem
            // 
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            resources.ApplyResources(this.groupToolStripMenuItem, "groupToolStripMenuItem");
            this.groupToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditGroup_Click);
            // 
            // ungroupToolStripMenuItem
            // 
            this.ungroupToolStripMenuItem.Name = "ungroupToolStripMenuItem";
            resources.ApplyResources(this.ungroupToolStripMenuItem, "ungroupToolStripMenuItem");
            this.ungroupToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditUngroup_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // lockToolStripMenuItem
            // 
            this.lockToolStripMenuItem.Name = "lockToolStripMenuItem";
            resources.ApplyResources(this.lockToolStripMenuItem, "lockToolStripMenuItem");
            this.lockToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditLock_Click);
            // 
            // unlockAllToolStripMenuItem
            // 
            this.unlockAllToolStripMenuItem.Name = "unlockAllToolStripMenuItem";
            resources.ApplyResources(this.unlockAllToolStripMenuItem, "unlockAllToolStripMenuItem");
            this.unlockAllToolStripMenuItem.Click += new System.EventHandler(this.menuItemUnlockAll_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // registerToLibraryToolStripMenuItem
            // 
            this.registerToLibraryToolStripMenuItem.Name = "registerToLibraryToolStripMenuItem";
            resources.ApplyResources(this.registerToLibraryToolStripMenuItem, "registerToLibraryToolStripMenuItem");
            this.registerToLibraryToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditRegisterToLibrary_Click);
            // 
            // saveToLibraryFileToolStripMenuItem
            // 
            this.saveToLibraryFileToolStripMenuItem.Name = "saveToLibraryFileToolStripMenuItem";
            resources.ApplyResources(this.saveToLibraryFileToolStripMenuItem, "saveToLibraryFileToolStripMenuItem");
            this.saveToLibraryFileToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditRegisterToLibraryFile_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // objectPropertiesToolStripMenuItem
            // 
            this.objectPropertiesToolStripMenuItem.Name = "objectPropertiesToolStripMenuItem";
            resources.ApplyResources(this.objectPropertiesToolStripMenuItem, "objectPropertiesToolStripMenuItem");
            this.objectPropertiesToolStripMenuItem.Click += new System.EventHandler(this.menuItemEditObjectProperties_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator16,
            this.zoomToolStripMenuItem,
            this.objectNumberToolStripMenuItem});
            this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.viewToolStripMenuItem.MergeIndex = 200;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPercent10,
            this.toolStripMenuItemPercent25,
            this.toolStripMenuItemPercent50,
            this.toolStripMenuItemPercent75,
            this.toolStripMenuItemPercent100,
            this.toolStripMenuItemPercent150,
            this.toolStripMenuItemPercent200,
            this.toolStripMenuItemPercent300,
            this.toolStripMenuItemPercent400,
            this.toolStripMenuItemPercent500,
            this.toolStripMenuItemPercent600,
            this.toolStripMenuItemPercent700,
            this.toolStripMenuItemPercent800,
            this.toolStripMenuItemPercent900,
            this.toolStripMenuItemPercent1000,
            this.toolStripSeparator24,
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem1});
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            resources.ApplyResources(this.zoomToolStripMenuItem, "zoomToolStripMenuItem");
            this.zoomToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemViewOptic_Popup);
            // 
            // toolStripMenuItemPercent10
            // 
            this.toolStripMenuItemPercent10.Name = "toolStripMenuItemPercent10";
            resources.ApplyResources(this.toolStripMenuItemPercent10, "toolStripMenuItemPercent10");
            this.toolStripMenuItemPercent10.Click += new System.EventHandler(this.menuItemViewPercent10_Click);
            // 
            // toolStripMenuItemPercent25
            // 
            this.toolStripMenuItemPercent25.Name = "toolStripMenuItemPercent25";
            resources.ApplyResources(this.toolStripMenuItemPercent25, "toolStripMenuItemPercent25");
            this.toolStripMenuItemPercent25.Click += new System.EventHandler(this.menuItemViewPercent25_Click);
            // 
            // toolStripMenuItemPercent50
            // 
            this.toolStripMenuItemPercent50.Name = "toolStripMenuItemPercent50";
            resources.ApplyResources(this.toolStripMenuItemPercent50, "toolStripMenuItemPercent50");
            this.toolStripMenuItemPercent50.Click += new System.EventHandler(this.menuItemViewPercent50_Click);
            // 
            // toolStripMenuItemPercent75
            // 
            this.toolStripMenuItemPercent75.Name = "toolStripMenuItemPercent75";
            resources.ApplyResources(this.toolStripMenuItemPercent75, "toolStripMenuItemPercent75");
            this.toolStripMenuItemPercent75.Click += new System.EventHandler(this.menuItemViewPercent75_Click);
            // 
            // toolStripMenuItemPercent100
            // 
            this.toolStripMenuItemPercent100.Name = "toolStripMenuItemPercent100";
            resources.ApplyResources(this.toolStripMenuItemPercent100, "toolStripMenuItemPercent100");
            this.toolStripMenuItemPercent100.Click += new System.EventHandler(this.menuItemViewPercent100_Click);
            // 
            // toolStripMenuItemPercent150
            // 
            this.toolStripMenuItemPercent150.Name = "toolStripMenuItemPercent150";
            resources.ApplyResources(this.toolStripMenuItemPercent150, "toolStripMenuItemPercent150");
            this.toolStripMenuItemPercent150.Click += new System.EventHandler(this.menuItemViewPercent150_Click);
            // 
            // toolStripMenuItemPercent200
            // 
            this.toolStripMenuItemPercent200.Name = "toolStripMenuItemPercent200";
            resources.ApplyResources(this.toolStripMenuItemPercent200, "toolStripMenuItemPercent200");
            this.toolStripMenuItemPercent200.Click += new System.EventHandler(this.menuItemViewPercent200_Click);
            // 
            // toolStripMenuItemPercent300
            // 
            this.toolStripMenuItemPercent300.Name = "toolStripMenuItemPercent300";
            resources.ApplyResources(this.toolStripMenuItemPercent300, "toolStripMenuItemPercent300");
            this.toolStripMenuItemPercent300.Click += new System.EventHandler(this.menuItemViewPercent300_Click);
            // 
            // toolStripMenuItemPercent400
            // 
            this.toolStripMenuItemPercent400.Name = "toolStripMenuItemPercent400";
            resources.ApplyResources(this.toolStripMenuItemPercent400, "toolStripMenuItemPercent400");
            this.toolStripMenuItemPercent400.Click += new System.EventHandler(this.menuItemViewPercent400_Click);
            // 
            // toolStripMenuItemPercent500
            // 
            this.toolStripMenuItemPercent500.Name = "toolStripMenuItemPercent500";
            resources.ApplyResources(this.toolStripMenuItemPercent500, "toolStripMenuItemPercent500");
            this.toolStripMenuItemPercent500.Click += new System.EventHandler(this.menuItemViewPercent500_Click);
            // 
            // toolStripMenuItemPercent600
            // 
            this.toolStripMenuItemPercent600.Name = "toolStripMenuItemPercent600";
            resources.ApplyResources(this.toolStripMenuItemPercent600, "toolStripMenuItemPercent600");
            this.toolStripMenuItemPercent600.Click += new System.EventHandler(this.menuItemViewPercent600_Click);
            // 
            // toolStripMenuItemPercent700
            // 
            this.toolStripMenuItemPercent700.Name = "toolStripMenuItemPercent700";
            resources.ApplyResources(this.toolStripMenuItemPercent700, "toolStripMenuItemPercent700");
            this.toolStripMenuItemPercent700.Click += new System.EventHandler(this.menuItemViewPercent700_Click);
            // 
            // toolStripMenuItemPercent800
            // 
            this.toolStripMenuItemPercent800.Name = "toolStripMenuItemPercent800";
            resources.ApplyResources(this.toolStripMenuItemPercent800, "toolStripMenuItemPercent800");
            this.toolStripMenuItemPercent800.Click += new System.EventHandler(this.menuItemViewPercent800_Click);
            // 
            // toolStripMenuItemPercent900
            // 
            this.toolStripMenuItemPercent900.Name = "toolStripMenuItemPercent900";
            resources.ApplyResources(this.toolStripMenuItemPercent900, "toolStripMenuItemPercent900");
            this.toolStripMenuItemPercent900.Click += new System.EventHandler(this.menuItemViewPercent900_Click);
            // 
            // toolStripMenuItemPercent1000
            // 
            this.toolStripMenuItemPercent1000.Name = "toolStripMenuItemPercent1000";
            resources.ApplyResources(this.toolStripMenuItemPercent1000, "toolStripMenuItemPercent1000");
            this.toolStripMenuItemPercent1000.Click += new System.EventHandler(this.menuItemViewPercent1000_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            resources.ApplyResources(this.zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem1
            // 
            this.zoomOutToolStripMenuItem1.Name = "zoomOutToolStripMenuItem1";
            resources.ApplyResources(this.zoomOutToolStripMenuItem1, "zoomOutToolStripMenuItem1");
            this.zoomOutToolStripMenuItem1.Click += new System.EventHandler(this.zoomOutToolStripMenuItem1_Click);
            // 
            // objectNumberToolStripMenuItem
            // 
            this.objectNumberToolStripMenuItem.Name = "objectNumberToolStripMenuItem";
            resources.ApplyResources(this.objectNumberToolStripMenuItem, "objectNumberToolStripMenuItem");
            this.objectNumberToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewObjectNumber_Click);
            // 
            // objectToolStripMenuItem
            // 
            this.objectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonToolStripMenuItem,
            this.digitalToolStripMenuItem,
            this.analogToolStripMenuItem,
            this.stringTagToolStripMenuItem,
            this.tagAnimationToolStripMenuItem,
            this.displayChangesToolStripMenuItem,
            this.graphicModuleToolStripMenuItem,
            this.alarmWindowToolStripMenuItem,
            this.windowControlToolStripMenuItem,
            this.graphTrendToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.dataGridViewToolStripMenuItem,
            this.DonutChartToolStripMenuItem,
            this.chartToolStripMenuItem,
            this.toolStripSeparator19,
            this.bitmapToolStripMenuItem,
            this.animationToolStripMenuItem,
            this.svgToolStripMenuItem,
            this.singleLineTextToolStripMenuItem,
            this.rectangleToolStripMenuItem,
            this.circleToolStripMenuItem,
            this.lineToolStripMenuItem,
            this.roundedRectangleToolStripMenuItem,
            this.toolStripSeparator20,
            this.textToolStripMenuItem,
            this.clockToolStripMenuItem,
            this.dateToolStripMenuItem,
            this.webBrowserToolStripMenuItem,
            this.webViewToolStripMenuItem,
            this.VLCAxToolStripMenuItem,
            this.barcodeDisplayToolStripMenuItem,
            this.barcodeScannerToolStripMenuItem,
            this.toolStripSeparator21,
            this.objectLibraryToolStripMenuItem,
            this.buyObjectLibraryFromWebToolStripMenuItem,
            this.libraryFileToolStripMenuItem});
            this.objectToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.objectToolStripMenuItem.MergeIndex = 3;
            this.objectToolStripMenuItem.Name = "objectToolStripMenuItem";
            resources.ApplyResources(this.objectToolStripMenuItem, "objectToolStripMenuItem");
            this.objectToolStripMenuItem.DropDownOpened += new System.EventHandler(this.objectToolStripMenuItem_DropDownOpened);
            // 
            // buttonToolStripMenuItem
            // 
            this.buttonToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moduleSelectionButtonToolStripMenuItem,
            this.moduleHideButtonToolStripMenuItem,
            this.scriptButtonToolStripMenuItem,
            this.digitalOutputButtonToolStripMenuItem});
            this.buttonToolStripMenuItem.Name = "buttonToolStripMenuItem";
            resources.ApplyResources(this.buttonToolStripMenuItem, "buttonToolStripMenuItem");
            // 
            // moduleSelectionButtonToolStripMenuItem
            // 
            this.moduleSelectionButtonToolStripMenuItem.Name = "moduleSelectionButtonToolStripMenuItem";
            resources.ApplyResources(this.moduleSelectionButtonToolStripMenuItem, "moduleSelectionButtonToolStripMenuItem");
            this.moduleSelectionButtonToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectButtonModule3D_Click);
            // 
            // moduleHideButtonToolStripMenuItem
            // 
            this.moduleHideButtonToolStripMenuItem.Name = "moduleHideButtonToolStripMenuItem";
            resources.ApplyResources(this.moduleHideButtonToolStripMenuItem, "moduleHideButtonToolStripMenuItem");
            this.moduleHideButtonToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectButtonModuleHide_Click);
            // 
            // scriptButtonToolStripMenuItem
            // 
            this.scriptButtonToolStripMenuItem.Name = "scriptButtonToolStripMenuItem";
            resources.ApplyResources(this.scriptButtonToolStripMenuItem, "scriptButtonToolStripMenuItem");
            this.scriptButtonToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectButtonProgram_Click);
            // 
            // digitalOutputButtonToolStripMenuItem
            // 
            this.digitalOutputButtonToolStripMenuItem.Name = "digitalOutputButtonToolStripMenuItem";
            resources.ApplyResources(this.digitalOutputButtonToolStripMenuItem, "digitalOutputButtonToolStripMenuItem");
            this.digitalOutputButtonToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectButtonDigitalOut_Click);
            // 
            // digitalToolStripMenuItem
            // 
            this.digitalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.digitalAnimationToolStripMenuItem,
            this.digitalCircleToolStripMenuItem,
            this.digitalRectangleToolStripMenuItem,
            this.digitalStringToolStripMenuItem});
            this.digitalToolStripMenuItem.Name = "digitalToolStripMenuItem";
            resources.ApplyResources(this.digitalToolStripMenuItem, "digitalToolStripMenuItem");
            // 
            // digitalAnimationToolStripMenuItem
            // 
            this.digitalAnimationToolStripMenuItem.Name = "digitalAnimationToolStripMenuItem";
            resources.ApplyResources(this.digitalAnimationToolStripMenuItem, "digitalAnimationToolStripMenuItem");
            this.digitalAnimationToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDigitalAnimation_Click);
            // 
            // digitalCircleToolStripMenuItem
            // 
            this.digitalCircleToolStripMenuItem.Name = "digitalCircleToolStripMenuItem";
            resources.ApplyResources(this.digitalCircleToolStripMenuItem, "digitalCircleToolStripMenuItem");
            this.digitalCircleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDigitalCircle_Click);
            // 
            // digitalRectangleToolStripMenuItem
            // 
            this.digitalRectangleToolStripMenuItem.Name = "digitalRectangleToolStripMenuItem";
            resources.ApplyResources(this.digitalRectangleToolStripMenuItem, "digitalRectangleToolStripMenuItem");
            this.digitalRectangleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDigitalRectangle_Click);
            // 
            // digitalStringToolStripMenuItem
            // 
            this.digitalStringToolStripMenuItem.Name = "digitalStringToolStripMenuItem";
            resources.ApplyResources(this.digitalStringToolStripMenuItem, "digitalStringToolStripMenuItem");
            this.digitalStringToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDigitalString_Click);
            // 
            // analogToolStripMenuItem
            // 
            this.analogToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analogRectangleToolStripMenuItem,
            this.analogStringToolStripMenuItem,
            this.analogMeterToolStripMenuItem,
            this.analogStatusToolStripMenuItem,
            this.analogRotateToolStripMenuItem,
            this.analogGaugeToolStripMenuItem});
            this.analogToolStripMenuItem.Name = "analogToolStripMenuItem";
            resources.ApplyResources(this.analogToolStripMenuItem, "analogToolStripMenuItem");
            // 
            // analogRectangleToolStripMenuItem
            // 
            this.analogRectangleToolStripMenuItem.Name = "analogRectangleToolStripMenuItem";
            resources.ApplyResources(this.analogRectangleToolStripMenuItem, "analogRectangleToolStripMenuItem");
            this.analogRectangleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnalogRectangle_Click);
            // 
            // analogStringToolStripMenuItem
            // 
            this.analogStringToolStripMenuItem.Name = "analogStringToolStripMenuItem";
            resources.ApplyResources(this.analogStringToolStripMenuItem, "analogStringToolStripMenuItem");
            this.analogStringToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnalogString_Click);
            // 
            // analogMeterToolStripMenuItem
            // 
            this.analogMeterToolStripMenuItem.Name = "analogMeterToolStripMenuItem";
            resources.ApplyResources(this.analogMeterToolStripMenuItem, "analogMeterToolStripMenuItem");
            this.analogMeterToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnalogMeter_Click);
            // 
            // analogStatusToolStripMenuItem
            // 
            this.analogStatusToolStripMenuItem.Name = "analogStatusToolStripMenuItem";
            resources.ApplyResources(this.analogStatusToolStripMenuItem, "analogStatusToolStripMenuItem");
            this.analogStatusToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnalogStatus_Click);
            // 
            // analogRotateToolStripMenuItem
            // 
            this.analogRotateToolStripMenuItem.Name = "analogRotateToolStripMenuItem";
            resources.ApplyResources(this.analogRotateToolStripMenuItem, "analogRotateToolStripMenuItem");
            this.analogRotateToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnalogRotate_Click);
            // 
            // analogGaugeToolStripMenuItem
            // 
            this.analogGaugeToolStripMenuItem.Name = "analogGaugeToolStripMenuItem";
            resources.ApplyResources(this.analogGaugeToolStripMenuItem, "analogGaugeToolStripMenuItem");
            this.analogGaugeToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnalogGauge_Click);
            // 
            // stringTagToolStripMenuItem
            // 
            this.stringTagToolStripMenuItem.Name = "stringTagToolStripMenuItem";
            resources.ApplyResources(this.stringTagToolStripMenuItem, "stringTagToolStripMenuItem");
            this.stringTagToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectStringTag_Click);
            // 
            // tagAnimationToolStripMenuItem
            // 
            this.tagAnimationToolStripMenuItem.Name = "tagAnimationToolStripMenuItem";
            resources.ApplyResources(this.tagAnimationToolStripMenuItem, "tagAnimationToolStripMenuItem");
            this.tagAnimationToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertTagAnimation_Click);
            // 
            // displayChangesToolStripMenuItem
            // 
            this.displayChangesToolStripMenuItem.Name = "displayChangesToolStripMenuItem";
            resources.ApplyResources(this.displayChangesToolStripMenuItem, "displayChangesToolStripMenuItem");
            this.displayChangesToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectChangeValueDisplay_Click);
            // 
            // graphicModuleToolStripMenuItem
            // 
            this.graphicModuleToolStripMenuItem.Name = "graphicModuleToolStripMenuItem";
            resources.ApplyResources(this.graphicModuleToolStripMenuItem, "graphicModuleToolStripMenuItem");
            this.graphicModuleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectGraphicModule_Click);
            // 
            // alarmWindowToolStripMenuItem
            // 
            this.alarmWindowToolStripMenuItem.Name = "alarmWindowToolStripMenuItem";
            resources.ApplyResources(this.alarmWindowToolStripMenuItem, "alarmWindowToolStripMenuItem");
            this.alarmWindowToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAlarmWindow_Click);
            // 
            // windowControlToolStripMenuItem
            // 
            this.windowControlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listBoxToolStripMenuItem,
            this.comboBoxToolStripMenuItem,
            this.editBoxToolStripMenuItem,
            this.radioButtonToolStripMenuItem,
            this.checkBoxToolStripMenuItem,
            this.datePickerToolStripMenuItem,
            this.tabControlToolStripMenuItem,
            this.treeViewToolStripMenuItem});
            this.windowControlToolStripMenuItem.Name = "windowControlToolStripMenuItem";
            resources.ApplyResources(this.windowControlToolStripMenuItem, "windowControlToolStripMenuItem");
            // 
            // listBoxToolStripMenuItem
            // 
            this.listBoxToolStripMenuItem.Name = "listBoxToolStripMenuItem";
            resources.ApplyResources(this.listBoxToolStripMenuItem, "listBoxToolStripMenuItem");
            this.listBoxToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectControlListBox_Click);
            // 
            // comboBoxToolStripMenuItem
            // 
            this.comboBoxToolStripMenuItem.Name = "comboBoxToolStripMenuItem";
            resources.ApplyResources(this.comboBoxToolStripMenuItem, "comboBoxToolStripMenuItem");
            this.comboBoxToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectControlComboBox_Click);
            // 
            // editBoxToolStripMenuItem
            // 
            this.editBoxToolStripMenuItem.Name = "editBoxToolStripMenuItem";
            resources.ApplyResources(this.editBoxToolStripMenuItem, "editBoxToolStripMenuItem");
            this.editBoxToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectControlEditBox_Click);
            // 
            // radioButtonToolStripMenuItem
            // 
            this.radioButtonToolStripMenuItem.Name = "radioButtonToolStripMenuItem";
            resources.ApplyResources(this.radioButtonToolStripMenuItem, "radioButtonToolStripMenuItem");
            this.radioButtonToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectControlRadioButton_Click);
            // 
            // checkBoxToolStripMenuItem
            // 
            this.checkBoxToolStripMenuItem.Name = "checkBoxToolStripMenuItem";
            resources.ApplyResources(this.checkBoxToolStripMenuItem, "checkBoxToolStripMenuItem");
            this.checkBoxToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectControlCheckBox_Click);
            // 
            // datePickerToolStripMenuItem
            // 
            this.datePickerToolStripMenuItem.Name = "datePickerToolStripMenuItem";
            resources.ApplyResources(this.datePickerToolStripMenuItem, "datePickerToolStripMenuItem");
            this.datePickerToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectControlDatePicker_Click);
            // 
            // tabControlToolStripMenuItem
            // 
            this.tabControlToolStripMenuItem.Name = "tabControlToolStripMenuItem";
            resources.ApplyResources(this.tabControlToolStripMenuItem, "tabControlToolStripMenuItem");
            this.tabControlToolStripMenuItem.Click += new System.EventHandler(this.tabControlToolStripMenuItem_Click);
            // 
            // treeViewToolStripMenuItem
            // 
            this.treeViewToolStripMenuItem.Name = "treeViewToolStripMenuItem";
            resources.ApplyResources(this.treeViewToolStripMenuItem, "treeViewToolStripMenuItem");
            this.treeViewToolStripMenuItem.Click += new System.EventHandler(this.treeViewToolStripMenuItem_Click);
            // 
            // graphTrendToolStripMenuItem
            // 
            this.graphTrendToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.multigraphToolStripMenuItem,
            this.multitrendToolStripMenuItem,
            this.toolStripSeparatorChart,
            this.milliDataWindowToolStripMenuItem,
            this.milliDataTrendToolStripMenuItem,
            this.toolStripSeparator18,
            this.demandWindowToolStripMenuItem,
            this.demandChartToolStripMenuItem,
            this.realtimeTestGraphToolStripMenuItem,
            this.xYGraphToolStripMenuItem,
            this.databaseTrendToolStripMenuItem});
            this.graphTrendToolStripMenuItem.Name = "graphTrendToolStripMenuItem";
            resources.ApplyResources(this.graphTrendToolStripMenuItem, "graphTrendToolStripMenuItem");
            // 
            // multigraphToolStripMenuItem
            // 
            this.multigraphToolStripMenuItem.Name = "multigraphToolStripMenuItem";
            resources.ApplyResources(this.multigraphToolStripMenuItem, "multigraphToolStripMenuItem");
            this.multigraphToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectMultiGraph_Click);
            // 
            // multitrendToolStripMenuItem
            // 
            this.multitrendToolStripMenuItem.Name = "multitrendToolStripMenuItem";
            resources.ApplyResources(this.multitrendToolStripMenuItem, "multitrendToolStripMenuItem");
            this.multitrendToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectMultiTrend_Click);
            // 
            // toolStripSeparatorChart
            // 
            this.toolStripSeparatorChart.Name = "toolStripSeparatorChart";
            resources.ApplyResources(this.toolStripSeparatorChart, "toolStripSeparatorChart");
            // 
            // milliDataWindowToolStripMenuItem
            // 
            this.milliDataWindowToolStripMenuItem.Name = "milliDataWindowToolStripMenuItem";
            resources.ApplyResources(this.milliDataWindowToolStripMenuItem, "milliDataWindowToolStripMenuItem");
            this.milliDataWindowToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectMilliDataWindow_Click);
            // 
            // milliDataTrendToolStripMenuItem
            // 
            this.milliDataTrendToolStripMenuItem.Name = "milliDataTrendToolStripMenuItem";
            resources.ApplyResources(this.milliDataTrendToolStripMenuItem, "milliDataTrendToolStripMenuItem");
            this.milliDataTrendToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectMilliDataTrend_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // demandWindowToolStripMenuItem
            // 
            this.demandWindowToolStripMenuItem.Name = "demandWindowToolStripMenuItem";
            resources.ApplyResources(this.demandWindowToolStripMenuItem, "demandWindowToolStripMenuItem");
            this.demandWindowToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDemandWindow_Click);
            // 
            // demandChartToolStripMenuItem
            // 
            this.demandChartToolStripMenuItem.Name = "demandChartToolStripMenuItem";
            resources.ApplyResources(this.demandChartToolStripMenuItem, "demandChartToolStripMenuItem");
            this.demandChartToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDemandChart_Click);
            // 
            // realtimeTestGraphToolStripMenuItem
            // 
            this.realtimeTestGraphToolStripMenuItem.Name = "realtimeTestGraphToolStripMenuItem";
            resources.ApplyResources(this.realtimeTestGraphToolStripMenuItem, "realtimeTestGraphToolStripMenuItem");
            this.realtimeTestGraphToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectRealTimeTestGraph_Click);
            // 
            // xYGraphToolStripMenuItem
            // 
            this.xYGraphToolStripMenuItem.Name = "xYGraphToolStripMenuItem";
            resources.ApplyResources(this.xYGraphToolStripMenuItem, "xYGraphToolStripMenuItem");
            this.xYGraphToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectXyGraph_Click);
            // 
            // databaseTrendToolStripMenuItem
            // 
            this.databaseTrendToolStripMenuItem.Name = "databaseTrendToolStripMenuItem";
            resources.ApplyResources(this.databaseTrendToolStripMenuItem, "databaseTrendToolStripMenuItem");
            this.databaseTrendToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDatabaseTrend_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            resources.ApplyResources(this.databaseToolStripMenuItem, "databaseToolStripMenuItem");
            this.databaseToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDatabase_Click);
            // 
            // dataGridViewToolStripMenuItem
            // 
            this.dataGridViewToolStripMenuItem.Name = "dataGridViewToolStripMenuItem";
            resources.ApplyResources(this.dataGridViewToolStripMenuItem, "dataGridViewToolStripMenuItem");
            this.dataGridViewToolStripMenuItem.Click += new System.EventHandler(this.dataGridViewToolStripMenuItem_Click);
            // 
            // DonutChartToolStripMenuItem
            // 
            this.DonutChartToolStripMenuItem.Name = "DonutChartToolStripMenuItem";
            resources.ApplyResources(this.DonutChartToolStripMenuItem, "DonutChartToolStripMenuItem");
            this.DonutChartToolStripMenuItem.Click += new System.EventHandler(this.DonutChartToolStripMenuItem_Click);
            // 
            // chartToolStripMenuItem
            // 
            this.chartToolStripMenuItem.Name = "chartToolStripMenuItem";
            resources.ApplyResources(this.chartToolStripMenuItem, "chartToolStripMenuItem");
            this.chartToolStripMenuItem.Click += new System.EventHandler(this.chartToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // bitmapToolStripMenuItem
            // 
            this.bitmapToolStripMenuItem.Name = "bitmapToolStripMenuItem";
            resources.ApplyResources(this.bitmapToolStripMenuItem, "bitmapToolStripMenuItem");
            this.bitmapToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectBitmap_Click);
            // 
            // animationToolStripMenuItem
            // 
            this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            resources.ApplyResources(this.animationToolStripMenuItem, "animationToolStripMenuItem");
            this.animationToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectAnimation_Click);
            // 
            // svgToolStripMenuItem
            // 
            this.svgToolStripMenuItem.Name = "svgToolStripMenuItem";
            resources.ApplyResources(this.svgToolStripMenuItem, "svgToolStripMenuItem");
            this.svgToolStripMenuItem.Click += new System.EventHandler(this.SVGToolStripMenuItem_Click);
            // 
            // singleLineTextToolStripMenuItem
            // 
            this.singleLineTextToolStripMenuItem.Name = "singleLineTextToolStripMenuItem";
            resources.ApplyResources(this.singleLineTextToolStripMenuItem, "singleLineTextToolStripMenuItem");
            this.singleLineTextToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectSingleText_Click);
            // 
            // rectangleToolStripMenuItem
            // 
            this.rectangleToolStripMenuItem.Name = "rectangleToolStripMenuItem";
            resources.ApplyResources(this.rectangleToolStripMenuItem, "rectangleToolStripMenuItem");
            this.rectangleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectRectangle_Click);
            // 
            // circleToolStripMenuItem
            // 
            this.circleToolStripMenuItem.Name = "circleToolStripMenuItem";
            resources.ApplyResources(this.circleToolStripMenuItem, "circleToolStripMenuItem");
            this.circleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectCircle_Click);
            // 
            // lineToolStripMenuItem
            // 
            this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
            resources.ApplyResources(this.lineToolStripMenuItem, "lineToolStripMenuItem");
            this.lineToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectLine_Click);
            // 
            // roundedRectangleToolStripMenuItem
            // 
            this.roundedRectangleToolStripMenuItem.Name = "roundedRectangleToolStripMenuItem";
            resources.ApplyResources(this.roundedRectangleToolStripMenuItem, "roundedRectangleToolStripMenuItem");
            this.roundedRectangleToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectRoundRectangle_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            resources.ApplyResources(this.textToolStripMenuItem, "textToolStripMenuItem");
            this.textToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectText_Click);
            // 
            // clockToolStripMenuItem
            // 
            this.clockToolStripMenuItem.Name = "clockToolStripMenuItem";
            resources.ApplyResources(this.clockToolStripMenuItem, "clockToolStripMenuItem");
            this.clockToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectWatch_Click);
            // 
            // dateToolStripMenuItem
            // 
            this.dateToolStripMenuItem.Name = "dateToolStripMenuItem";
            resources.ApplyResources(this.dateToolStripMenuItem, "dateToolStripMenuItem");
            this.dateToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectDate_Click);
            // 
            // webBrowserToolStripMenuItem
            // 
            this.webBrowserToolStripMenuItem.Name = "webBrowserToolStripMenuItem";
            resources.ApplyResources(this.webBrowserToolStripMenuItem, "webBrowserToolStripMenuItem");
            this.webBrowserToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertObjectWebBrowser_Click);
            // 
            // webViewToolStripMenuItem
            // 
            this.webViewToolStripMenuItem.Name = "webViewToolStripMenuItem";
            resources.ApplyResources(this.webViewToolStripMenuItem, "webViewToolStripMenuItem");
            this.webViewToolStripMenuItem.Click += new System.EventHandler(this.webViewToolStripMenuItem_Click);
            // 
            // VLCAxToolStripMenuItem
            // 
            this.VLCAxToolStripMenuItem.Name = "VLCAxToolStripMenuItem";
            resources.ApplyResources(this.VLCAxToolStripMenuItem, "VLCAxToolStripMenuItem");
            this.VLCAxToolStripMenuItem.Click += new System.EventHandler(this.VLCAxToolStripMenuItem_Click);
            // 
            // barcodeDisplayToolStripMenuItem
            // 
            this.barcodeDisplayToolStripMenuItem.Name = "barcodeDisplayToolStripMenuItem";
            resources.ApplyResources(this.barcodeDisplayToolStripMenuItem, "barcodeDisplayToolStripMenuItem");
            this.barcodeDisplayToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertBarcodeDisplay_Click);
            // 
            // barcodeScannerToolStripMenuItem
            // 
            this.barcodeScannerToolStripMenuItem.Name = "barcodeScannerToolStripMenuItem";
            resources.ApplyResources(this.barcodeScannerToolStripMenuItem, "barcodeScannerToolStripMenuItem");
            this.barcodeScannerToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertBarcodeScanner_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            // 
            // objectLibraryToolStripMenuItem
            // 
            this.objectLibraryToolStripMenuItem.Name = "objectLibraryToolStripMenuItem";
            resources.ApplyResources(this.objectLibraryToolStripMenuItem, "objectLibraryToolStripMenuItem");
            this.objectLibraryToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertFromLibrary_Click);
            // 
            // buyObjectLibraryFromWebToolStripMenuItem
            // 
            this.buyObjectLibraryFromWebToolStripMenuItem.Name = "buyObjectLibraryFromWebToolStripMenuItem";
            resources.ApplyResources(this.buyObjectLibraryFromWebToolStripMenuItem, "buyObjectLibraryFromWebToolStripMenuItem");
            this.buyObjectLibraryFromWebToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertFromLibrarySearch_Click);
            // 
            // libraryFileToolStripMenuItem
            // 
            this.libraryFileToolStripMenuItem.Name = "libraryFileToolStripMenuItem";
            resources.ApplyResources(this.libraryFileToolStripMenuItem, "libraryFileToolStripMenuItem");
            this.libraryFileToolStripMenuItem.Click += new System.EventHandler(this.menuItemInsertFromLibraryFile_Click);
            // 
            // etcToolStripMenuItem
            // 
            this.etcToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundBitmapToolStripMenuItem,
            this.backgroundColorToolStripMenuItem,
            this.modulePropertiesToolStripMenuItem,
            this.toolStripSeparator22,
            this.scriptOnModuleOpenedToolStripMenuItem,
            this.scriptOnModuleRunningToolStripMenuItem,
            this.scriptOnModuleClosedToolStripMenuItem,
            this.scriptOnModuleActivatedToolStripMenuItem,
            this.scriptOnModuleDeactivatedToolStripMenuItem});
            this.etcToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.etcToolStripMenuItem.MergeIndex = 4;
            this.etcToolStripMenuItem.Name = "etcToolStripMenuItem";
            resources.ApplyResources(this.etcToolStripMenuItem, "etcToolStripMenuItem");
            // 
            // backgroundBitmapToolStripMenuItem
            // 
            this.backgroundBitmapToolStripMenuItem.Name = "backgroundBitmapToolStripMenuItem";
            resources.ApplyResources(this.backgroundBitmapToolStripMenuItem, "backgroundBitmapToolStripMenuItem");
            this.backgroundBitmapToolStripMenuItem.Click += new System.EventHandler(this.menuItemEtcBackgroundBitmap_Click);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            resources.ApplyResources(this.backgroundColorToolStripMenuItem, "backgroundColorToolStripMenuItem");
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.menuItemEtcBackgroundColor_Click);
            // 
            // modulePropertiesToolStripMenuItem
            // 
            this.modulePropertiesToolStripMenuItem.Name = "modulePropertiesToolStripMenuItem";
            resources.ApplyResources(this.modulePropertiesToolStripMenuItem, "modulePropertiesToolStripMenuItem");
            this.modulePropertiesToolStripMenuItem.Click += new System.EventHandler(this.menuItemEtcModuleProperty_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            // 
            // scriptOnModuleOpenedToolStripMenuItem
            // 
            this.scriptOnModuleOpenedToolStripMenuItem.Name = "scriptOnModuleOpenedToolStripMenuItem";
            resources.ApplyResources(this.scriptOnModuleOpenedToolStripMenuItem, "scriptOnModuleOpenedToolStripMenuItem");
            this.scriptOnModuleOpenedToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptOnModuleOpened_Click);
            // 
            // scriptOnModuleRunningToolStripMenuItem
            // 
            this.scriptOnModuleRunningToolStripMenuItem.Name = "scriptOnModuleRunningToolStripMenuItem";
            resources.ApplyResources(this.scriptOnModuleRunningToolStripMenuItem, "scriptOnModuleRunningToolStripMenuItem");
            this.scriptOnModuleRunningToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptOnModuleRunning_Click);
            // 
            // scriptOnModuleClosedToolStripMenuItem
            // 
            this.scriptOnModuleClosedToolStripMenuItem.Name = "scriptOnModuleClosedToolStripMenuItem";
            resources.ApplyResources(this.scriptOnModuleClosedToolStripMenuItem, "scriptOnModuleClosedToolStripMenuItem");
            this.scriptOnModuleClosedToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptOnModuleClosed_Click);
            // 
            // scriptOnModuleActivatedToolStripMenuItem
            // 
            this.scriptOnModuleActivatedToolStripMenuItem.Name = "scriptOnModuleActivatedToolStripMenuItem";
            resources.ApplyResources(this.scriptOnModuleActivatedToolStripMenuItem, "scriptOnModuleActivatedToolStripMenuItem");
            this.scriptOnModuleActivatedToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptOnModuleActivated_Click);
            // 
            // scriptOnModuleDeactivatedToolStripMenuItem
            // 
            this.scriptOnModuleDeactivatedToolStripMenuItem.Name = "scriptOnModuleDeactivatedToolStripMenuItem";
            resources.ApplyResources(this.scriptOnModuleDeactivatedToolStripMenuItem, "scriptOnModuleDeactivatedToolStripMenuItem");
            this.scriptOnModuleDeactivatedToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptOnModuleDeactivated_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator23,
            this.guideLineToolStripMenuItem});
            this.configToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.configToolStripMenuItem.MergeIndex = 800;
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            // 
            // guideLineToolStripMenuItem
            // 
            this.guideLineToolStripMenuItem.Name = "guideLineToolStripMenuItem";
            resources.ApplyResources(this.guideLineToolStripMenuItem, "guideLineToolStripMenuItem");
            this.guideLineToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigGuideLine_Click);
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
            resources.ApplyResources(this.windowToolStripMenuItem, "windowToolStripMenuItem");
            this.windowToolStripMenuItem.DropDownOpened += new System.EventHandler(this.windowToolStripMenuItem_DropDownOpened);
            this.windowToolStripMenuItem.Click += new System.EventHandler(this.windowToolStripMenuItem_Click);
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
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonMove,
            this.toolStripButtonLine,
            this.toolStripButtonRectangle,
            this.toolStripButtonFillRectangle,
            this.toolStripButtonCircle,
            this.toolStripButtonFillCircle,
            this.toolStripButtonPoly,
            this.toolStripButtonFillPoly,
            this.toolStripButtonText,
            this.toolStripButtonRoundRectangle,
            this.toolStripButtonCurve,
            this.toolStripButtonPointMove,
            this.toolStripButtonSpoid,
            this.toolStripSeparator1,
            this.toolStripButtonInsertFromLibrary,
            this.toolStripButtonInsertFromLibrarySearch,
            this.toolStripSeparator2,
            this.toolStripButtonAlignToLeft,
            this.toolStripButtonAlignHorizontalCenter,
            this.toolStripButtonAlignToRight,
            this.toolStripButtonSpaceHorizontal,
            this.toolStripButtonSpaceVertical,
            this.toolStripButtonAlignToTop,
            this.toolStripButtonAlignVerticalCenter,
            this.toolStripButtonAlignToBottom,
            this.toolStripSeparator25,
            this.toolStripButtonMakeSameSizeWidth,
            this.toolStripButtonMakeSameSizeHeight,
            this.toolStripButtonMoveToPrevFront,
            this.toolStripButtonMoveToNextBack,
            this.toolStripButtonRotateRight,
            this.toolStripButtonRotateLeft});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // toolStripButtonMove
            // 
            this.toolStripButtonMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonMove, "toolStripButtonMove");
            this.toolStripButtonMove.Name = "toolStripButtonMove";
            this.toolStripButtonMove.Click += new System.EventHandler(this.toolStripButtonMove_Click);
            // 
            // toolStripButtonLine
            // 
            this.toolStripButtonLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonLine, "toolStripButtonLine");
            this.toolStripButtonLine.Name = "toolStripButtonLine";
            this.toolStripButtonLine.Click += new System.EventHandler(this.toolStripButtonLine_Click);
            // 
            // toolStripButtonRectangle
            // 
            this.toolStripButtonRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRectangle, "toolStripButtonRectangle");
            this.toolStripButtonRectangle.Name = "toolStripButtonRectangle";
            this.toolStripButtonRectangle.Click += new System.EventHandler(this.toolStripButtonRectangle_Click);
            // 
            // toolStripButtonFillRectangle
            // 
            this.toolStripButtonFillRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonFillRectangle, "toolStripButtonFillRectangle");
            this.toolStripButtonFillRectangle.Name = "toolStripButtonFillRectangle";
            this.toolStripButtonFillRectangle.Click += new System.EventHandler(this.toolStripButtonFillRectangle_Click);
            // 
            // toolStripButtonCircle
            // 
            this.toolStripButtonCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonCircle, "toolStripButtonCircle");
            this.toolStripButtonCircle.Name = "toolStripButtonCircle";
            this.toolStripButtonCircle.Click += new System.EventHandler(this.toolStripButtonCircle_Click);
            // 
            // toolStripButtonFillCircle
            // 
            this.toolStripButtonFillCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonFillCircle, "toolStripButtonFillCircle");
            this.toolStripButtonFillCircle.Name = "toolStripButtonFillCircle";
            this.toolStripButtonFillCircle.Click += new System.EventHandler(this.toolStripButtonFillCircle_Click);
            // 
            // toolStripButtonPoly
            // 
            this.toolStripButtonPoly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonPoly, "toolStripButtonPoly");
            this.toolStripButtonPoly.Name = "toolStripButtonPoly";
            this.toolStripButtonPoly.Click += new System.EventHandler(this.toolStripButtonPoly_Click);
            // 
            // toolStripButtonFillPoly
            // 
            this.toolStripButtonFillPoly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonFillPoly, "toolStripButtonFillPoly");
            this.toolStripButtonFillPoly.Name = "toolStripButtonFillPoly";
            this.toolStripButtonFillPoly.Click += new System.EventHandler(this.toolStripButtonFillPoly_Click);
            // 
            // toolStripButtonText
            // 
            this.toolStripButtonText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonText, "toolStripButtonText");
            this.toolStripButtonText.Name = "toolStripButtonText";
            this.toolStripButtonText.Click += new System.EventHandler(this.toolStripButtonText_Click);
            // 
            // toolStripButtonRoundRectangle
            // 
            this.toolStripButtonRoundRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRoundRectangle, "toolStripButtonRoundRectangle");
            this.toolStripButtonRoundRectangle.Name = "toolStripButtonRoundRectangle";
            this.toolStripButtonRoundRectangle.Click += new System.EventHandler(this.toolStripButtonRoundRectangle_Click);
            // 
            // toolStripButtonCurve
            // 
            this.toolStripButtonCurve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonCurve, "toolStripButtonCurve");
            this.toolStripButtonCurve.Name = "toolStripButtonCurve";
            this.toolStripButtonCurve.Click += new System.EventHandler(this.toolStripButtonCurve_Click);
            // 
            // toolStripButtonPointMove
            // 
            this.toolStripButtonPointMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonPointMove, "toolStripButtonPointMove");
            this.toolStripButtonPointMove.Name = "toolStripButtonPointMove";
            this.toolStripButtonPointMove.Click += new System.EventHandler(this.toolStripButtonPointMove_Click);
            // 
            // toolStripButtonSpoid
            // 
            this.toolStripButtonSpoid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSpoid, "toolStripButtonSpoid");
            this.toolStripButtonSpoid.Name = "toolStripButtonSpoid";
            this.toolStripButtonSpoid.Click += new System.EventHandler(this.toolStripButtonSpoid_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonInsertFromLibrary
            // 
            this.toolStripButtonInsertFromLibrary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonInsertFromLibrary, "toolStripButtonInsertFromLibrary");
            this.toolStripButtonInsertFromLibrary.Name = "toolStripButtonInsertFromLibrary";
            this.toolStripButtonInsertFromLibrary.Click += new System.EventHandler(this.toolStripButtonInsertFromLibrary_Click);
            // 
            // toolStripButtonInsertFromLibrarySearch
            // 
            this.toolStripButtonInsertFromLibrarySearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonInsertFromLibrarySearch, "toolStripButtonInsertFromLibrarySearch");
            this.toolStripButtonInsertFromLibrarySearch.Name = "toolStripButtonInsertFromLibrarySearch";
            this.toolStripButtonInsertFromLibrarySearch.Click += new System.EventHandler(this.toolStripButtonInsertFromLibrarySearch_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonAlignToLeft
            // 
            this.toolStripButtonAlignToLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAlignToLeft, "toolStripButtonAlignToLeft");
            this.toolStripButtonAlignToLeft.Name = "toolStripButtonAlignToLeft";
            this.toolStripButtonAlignToLeft.Click += new System.EventHandler(this.toolStripButtonAlignToLeft_Click);
            // 
            // toolStripButtonAlignHorizontalCenter
            // 
            this.toolStripButtonAlignHorizontalCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAlignHorizontalCenter, "toolStripButtonAlignHorizontalCenter");
            this.toolStripButtonAlignHorizontalCenter.Name = "toolStripButtonAlignHorizontalCenter";
            this.toolStripButtonAlignHorizontalCenter.Click += new System.EventHandler(this.toolStripButtonAlignHorizontalCenter_Click);
            // 
            // toolStripButtonAlignToRight
            // 
            this.toolStripButtonAlignToRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAlignToRight, "toolStripButtonAlignToRight");
            this.toolStripButtonAlignToRight.Name = "toolStripButtonAlignToRight";
            this.toolStripButtonAlignToRight.Click += new System.EventHandler(this.toolStripButtonAlignToRight_Click);
            // 
            // toolStripButtonSpaceHorizontal
            // 
            this.toolStripButtonSpaceHorizontal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSpaceHorizontal, "toolStripButtonSpaceHorizontal");
            this.toolStripButtonSpaceHorizontal.Name = "toolStripButtonSpaceHorizontal";
            this.toolStripButtonSpaceHorizontal.Click += new System.EventHandler(this.toolStripButtonSpaceHorizontal_Click);
            // 
            // toolStripButtonSpaceVertical
            // 
            this.toolStripButtonSpaceVertical.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSpaceVertical, "toolStripButtonSpaceVertical");
            this.toolStripButtonSpaceVertical.Name = "toolStripButtonSpaceVertical";
            this.toolStripButtonSpaceVertical.Click += new System.EventHandler(this.toolStripButtonSpaceVertical_Click);
            // 
            // toolStripButtonAlignToTop
            // 
            this.toolStripButtonAlignToTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAlignToTop, "toolStripButtonAlignToTop");
            this.toolStripButtonAlignToTop.Name = "toolStripButtonAlignToTop";
            this.toolStripButtonAlignToTop.Click += new System.EventHandler(this.toolStripButtonAlignToTop_Click);
            // 
            // toolStripButtonAlignVerticalCenter
            // 
            this.toolStripButtonAlignVerticalCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAlignVerticalCenter, "toolStripButtonAlignVerticalCenter");
            this.toolStripButtonAlignVerticalCenter.Name = "toolStripButtonAlignVerticalCenter";
            this.toolStripButtonAlignVerticalCenter.Click += new System.EventHandler(this.toolStripButtonAlignVerticalCenter_Click);
            // 
            // toolStripButtonAlignToBottom
            // 
            this.toolStripButtonAlignToBottom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonAlignToBottom, "toolStripButtonAlignToBottom");
            this.toolStripButtonAlignToBottom.Name = "toolStripButtonAlignToBottom";
            this.toolStripButtonAlignToBottom.Click += new System.EventHandler(this.toolStripButtonAlignToBottom_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // toolStripButtonMakeSameSizeWidth
            // 
            this.toolStripButtonMakeSameSizeWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonMakeSameSizeWidth, "toolStripButtonMakeSameSizeWidth");
            this.toolStripButtonMakeSameSizeWidth.Name = "toolStripButtonMakeSameSizeWidth";
            this.toolStripButtonMakeSameSizeWidth.Click += new System.EventHandler(this.toolStripButtonMakeSameSizeWidth_Click);
            // 
            // toolStripButtonMakeSameSizeHeight
            // 
            this.toolStripButtonMakeSameSizeHeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonMakeSameSizeHeight, "toolStripButtonMakeSameSizeHeight");
            this.toolStripButtonMakeSameSizeHeight.Name = "toolStripButtonMakeSameSizeHeight";
            this.toolStripButtonMakeSameSizeHeight.Click += new System.EventHandler(this.toolStripButtonMakeSameSizeHeight_Click);
            // 
            // toolStripButtonMoveToPrevFront
            // 
            this.toolStripButtonMoveToPrevFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonMoveToPrevFront, "toolStripButtonMoveToPrevFront");
            this.toolStripButtonMoveToPrevFront.Name = "toolStripButtonMoveToPrevFront";
            this.toolStripButtonMoveToPrevFront.Click += new System.EventHandler(this.toolStripButtonMoveToFront_Click);
            // 
            // toolStripButtonMoveToNextBack
            // 
            this.toolStripButtonMoveToNextBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonMoveToNextBack, "toolStripButtonMoveToNextBack");
            this.toolStripButtonMoveToNextBack.Name = "toolStripButtonMoveToNextBack";
            this.toolStripButtonMoveToNextBack.Click += new System.EventHandler(this.toolStripButtonMoveToBack_Click);
            // 
            // toolStripButtonRotateRight
            // 
            this.toolStripButtonRotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRotateRight, "toolStripButtonRotateRight");
            this.toolStripButtonRotateRight.Name = "toolStripButtonRotateRight";
            this.toolStripButtonRotateRight.Click += new System.EventHandler(this.toolStripButtonRotateRight_Click);
            // 
            // toolStripButtonRotateLeft
            // 
            this.toolStripButtonRotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRotateLeft, "toolStripButtonRotateLeft");
            this.toolStripButtonRotateLeft.Name = "toolStripButtonRotateLeft";
            this.toolStripButtonRotateLeft.Click += new System.EventHandler(this.toolStripButtonRotateLeft_Click);
            // 
            // FormEditGraphicFrame
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormEditGraphicFrame";
            this.Activated += new System.EventHandler(this.FormEditGraphic_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormEditGraphic_Closing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormEditGraphicFrame_FormClosed);
            this.Load += new System.EventHandler(this.FormEditGraphicFrame_Load);
            this.VisibleChanged += new System.EventHandler(this.FormEditGraphicFrame_VisibleChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormEditGraphic_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		

		private void menuItemViewPercent10_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(10);
		}

		private void menuItemViewPercent25_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(25);
		}

		private void menuItemViewPercent50_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(50);
		}

		private void menuItemViewPercent100_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(100);
		}

		private void menuItemViewPercent200_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(200);
		}

		private void menuItemViewPercent300_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(300);	
		}

		private void menuItemViewPercent1000_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(1000);
		}

		private void menuItemConfigGuideLine_Click(object sender, System.EventArgs e)
		{
			FormConfigGuideLine form = new FormConfigGuideLine();
            form.StartPosition = FormStartPosition.CenterParent;

			if(form.ShowDialog(this) == DialogResult.OK)
			{
				this.Invalidate(true);
			}
		}

		private void menuItemEditCopy_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditCopy(this.formChild);
		}

		private void menuItemEditPaste_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditPaste(this.formChild);
		}

		private void menuItemEditDelete_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditDelete(this.formChild);
		}

		private void menuItemEditSelectAll_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditSelectAll(this.formChild);
		}

		private void menuItemEditMoveToFront_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMovePost(this.formChild);
		}

		private void menuItemEditMoveToBack_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMoveBack(this.formChild);
		}

		private void menuItemEditMoveToPrevFront_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMovePrevFront(this.formChild);
		}

		private void menuItemEditMoveToNextBack_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMoveNextBack(this.formChild);
		}

		private void menuItemEditAlignToLeft_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideLeft(this.formChild);
		}

		private void menuItemEditAlignToRight_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideRight(this.formChild);
		}

		private void menuItemEditAlignToTop_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideTop(this.formChild);
		}

		private void menuItemEditAlignToBottom_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideBottom(this.formChild);
		}

		private void menuItemSpaceHorizontal_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditSpaceHorz(this.formChild);
		}

		private void menuItemSpaceVertical_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditSpaceVert(this.formChild);
		}

		private void menuItemEditGroup_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditGroup(this.formChild);
		}

		private void menuItemEditUngroup_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditUnGroup(this.formChild);
		}

		private void menuItemEdit_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuItemView_Popup(object sender, System.EventArgs e)
		{
			SharedStudio.formMain.menuItemView_Popup(sender, e);
			this.objectNumberToolStripMenuItem.Checked = formChild.bDisplayObjectNumber;
		}

		private void menuItemViewOptic_Popup(object sender, System.EventArgs e)
		{
			toolStripMenuItemPercent10.Checked = (formChild.workThis.obj.nOpticRate == 10);
            toolStripMenuItemPercent25.Checked = (formChild.workThis.obj.nOpticRate == 25);
            toolStripMenuItemPercent50.Checked = (formChild.workThis.obj.nOpticRate == 50);
            toolStripMenuItemPercent75.Checked = (formChild.workThis.obj.nOpticRate == 75);
            toolStripMenuItemPercent100.Checked = (formChild.workThis.obj.nOpticRate == 100);
            toolStripMenuItemPercent150.Checked = (formChild.workThis.obj.nOpticRate == 150);
            toolStripMenuItemPercent200.Checked = (formChild.workThis.obj.nOpticRate == 200);
            toolStripMenuItemPercent300.Checked = (formChild.workThis.obj.nOpticRate == 300);
            toolStripMenuItemPercent400.Checked = (formChild.workThis.obj.nOpticRate == 400);
            toolStripMenuItemPercent500.Checked = (formChild.workThis.obj.nOpticRate == 500);
            toolStripMenuItemPercent600.Checked = (formChild.workThis.obj.nOpticRate == 600);
            toolStripMenuItemPercent700.Checked = (formChild.workThis.obj.nOpticRate == 700);
            toolStripMenuItemPercent800.Checked = (formChild.workThis.obj.nOpticRate == 800);
            toolStripMenuItemPercent900.Checked = (formChild.workThis.obj.nOpticRate == 900);
            toolStripMenuItemPercent1000.Checked = (formChild.workThis.obj.nOpticRate == 1000);
		}

		private void menuItemViewPercent75_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(75);		
		}

		private void menuItemViewPercent150_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(150);
		}

		private void menuItemViewPercent400_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(400);
		}

		private void menuItemViewPercent500_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(500);
		}

		private void menuItemViewPercent600_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(600);
		}

		private void menuItemViewPercent700_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(700);
		}

		private void menuItemViewPercent800_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(800);
		}

		private void menuItemViewPercent900_Click(object sender, System.EventArgs e)
		{
			formChild.OpticGo(900);
		}

		private void menuItemContextMoveToCurveObject_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditToCurveObject(this.formChild);
		}

		private void menuItemContextMoveCombine_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditCombine(this.formChild);
		}

		private void menuItemContextMoveGroup_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditGroup(this.formChild);
		}

		private void menuItemContextMoveUngroup_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditUnGroup(this.formChild);
		}

		private void menuItemContextMoveObjectProperty_Click(object sender, System.EventArgs e)
		{
			ClassEditProperty.OnUserPropertyClick(this.formChild);
		}

		private void menuItemEditUndo_Click(object sender, System.EventArgs e)
		{
			ClassStudioEditUndo.menuItemEditUndo_Click(this.formChild);
		}

		private void menuItemEditRedo_Click(object sender, System.EventArgs e)
		{
			ClassStudioEditUndo.menuItemEditRedo_Click(this.formChild);
		}

		private void menuItemEdit_Popup(object sender, System.EventArgs e)
		{
			string title = "";
			undoToolStripMenuItem.Enabled = ClassStudioEditUndo.IsPossibleEditUndo(this.formChild, ref title);
            undoToolStripMenuItem.Text = title;
			redoToolStripMenuItem.Enabled = ClassStudioEditUndo.IsPossibleEditRedo(this.formChild, ref title);
            redoToolStripMenuItem.Text = title;

			copyToolStripMenuItem.Enabled   = ClassStudioEdit.IsPossibleEditCopy(this.formChild);
			pasteToolStripMenuItem.Enabled  = ClassStudioEdit.IsPossibleEditPaste(this.formChild);
            autopasteToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditAutoPaste(this.formChild); // 24-06-27 autopaste 추가 hsjeong
			deleteToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditDelete(this.formChild);

			groupToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditGroup(this.formChild);
			ungroupToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditUnGroup(this.formChild);

			moveToBackToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMoveBack(this.formChild);
            moveToFrontToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMovePost(this.formChild);
            moveToPrevFrontToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMovePrevFront(this.formChild);
            moveToNextBackToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMoveNextBack(this.formChild);

            flipHorizontalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(this.formChild);
            flipVerticalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(this.formChild);
            rotate90DegreeToRightToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditRotate(this.formChild);
            rotate90DegreeToLeftToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditRotate(this.formChild);
            
			lockToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditLock(this.formChild);
			unlockAllToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditUnLock(this.formChild);

            makeSameSizeToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);

            registerToLibraryToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 0);
            saveToLibraryFileToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 0);
		}

		private void menuItemEtcBackgroundBitmap_Click(object sender, System.EventArgs e)
		{
			FormConfigBackgroundBitmap dialog = new FormConfigBackgroundBitmap(this.formChild);
            dialog.StartPosition = FormStartPosition.CenterParent;

			dialog.ShowDialog(this);
		}

		private void menuItemEtcBackgroundColor_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemEtcBackgroundColor_Click();
		}

		private void menuItemEtcModuleProperty_Click(object sender, System.EventArgs e) 
		{
			FormConfigModuleProperty dialog = new FormConfigModuleProperty(this.formChild);
            dialog.StartPosition = FormStartPosition.CenterParent;

			dialog.ShowDialog(this);
		}

		private void menuItemInsertObjectDatabase_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDatabase(this.formChild);
		}

		private void menuItemInsertObjectButtonModule3D_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonModule3D(this.formChild);
		}

		private void menuItemInsertObjectButtonModuleHide_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonModuleHide(this.formChild);
		}

		private void menuItemInsertObjectButtonProgram_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonProgram(this.formChild);
		}

		private void menuItemInsertObjectButtonDigitalOut_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonDigitalOut(this.formChild);
		}

		private void menuItemInsertObjectDigitalAnimation_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalAnimation(this.formChild);
		}

		private void menuItemInsertObjectDigitalCircle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalCircle(this.formChild);
		}

		private void menuItemInsertObjectDigitalRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalRectangle(this.formChild);
		}

		private void menuItemInsertObjectDigitalString_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalString(this.formChild);
		}

		private void menuItemInsertObjectAnalogRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogRectangle(this.formChild);
		}

		private void menuItemInsertObjectAnalogString_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogString(this.formChild);
		}

		private void menuItemInsertObjectAnalogMeter_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogMeter(this.formChild);
		}

		private void menuItemInsertObjectAnalogStatus_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogStatus(this.formChild);
		}

		private void menuItemInsertObjectAnalogRotate_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogRotate(this.formChild);
		}

        private void menuItemInsertObjectAnalogGauge_Click(object sender, System.EventArgs e) //hsjeong 2025-01-22
        {
            ClassEditInsert.EditInsertAnalogGauge(this.formChild);
        }

		private void menuItemInsertObjectStringTag_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertStringString(this.formChild);
		}

		private void menuItemInsertObjectGraphicModule_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertModule(this.formChild);
		}

		private void menuItemInsertObjectAlarmWindow_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertWindowAlarm(this.formChild);
		}

		private void menuItemInsertObjectMultiGraph_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertMultiGraph(this.formChild);
		}

		private void menuItemInsertObjectMultiTrend_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertMultiTrend(this.formChild);
		}

        //25-02-24 Chart 컨트롤 기반 오브젝트 메뉴 핸들러
        private void menuItemInsertCustomChart_Click(object sender, System.EventArgs e)
        {
            ClassEditInsert.EditInsertCustomChart(this.formChild);
        }

		private void menuItemInsertObjectDemandWindow_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDemandWindow(this.formChild);
		}

		private void menuItemInsertObjectMilliDataWindow_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertMilliData(this.formChild);
		}

		private void menuItemInsertObjectXyGraph_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertXYGraph(this.formChild);
		}

		private void menuItemInsertObjectControlListBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlListBox(this.formChild);
		}

		private void menuItemInsertObjectControlComboBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlComboBox(this.formChild);
		}

		private void menuItemInsertObjectControlEditBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlEditBox(this.formChild);
		}

		private void menuItemInsertObjectControlRadioButton_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlRadioButton(this.formChild);
		}

		private void menuItemInsertObjectControlCheckBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlCheckBox(this.formChild);
		}

        private void menuItemInsertObjectControlDatePicker_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertControlDatePicker(this.formChild);
        }

        private void tabControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertControlTabControl(this.formChild);
        }

        private void treeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertControlTreeView(this.formChild);
        }

		private void menuItemInsertObjectRealTimeTestGraph_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertRealTimeTestGraph(this.formChild);
		}

		private void menuItemInsertObjectDatabaseTrend_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDatabaseTrend(this.formChild);
		}

		private void menuItemInsertObjectBitmap_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertBitmap(this.formChild);
		}

		private void menuItemInsertObjectAnimation_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnimation(this.formChild);
		}

		private void menuItemInsertObjectSingleText_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertSingleText(this.formChild);
		}

		private void menuItemInsertObjectRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertRectangle(this.formChild);
		}

		private void menuItemInsertObjectCircle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertCircle(this.formChild);
		}

		private void menuItemInsertObjectLine_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertLine(this.formChild);
		}

		private void menuItemInsertObjectText_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertText(this.formChild);
		}

		private void menuItemInsertObjectWatch_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertClock(this.formChild);
		}

		private void menuItemInsertObjectDate_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDate(this.formChild);
		}

		private void menuItemInsertFromLibrary_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertFromLibrary(this.formChild);
		}

		private void menuItemFileSave_Click(object sender, System.EventArgs e)
		{
			formChild.FileSave();
		}

		private void menuItemFileSaveAs_Click(object sender, System.EventArgs e)
		{
			string filename = formChild.workThis.filename;

			string ext = Path.GetExtension(filename);
			if(String.Compare(ext, ".mod", true) == 0) 
			{
				filename = Path.GetDirectoryName(filename)+"\\"+Path.GetFileNameWithoutExtension(filename)+".modx";
			}
			formChild.SaveAs(filename);
		}

		private void menuItemScriptOnModuleOpened_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemScriptOnModuleOpened_Click();
		}

		private void menuItemScriptOnModuleRunning_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemScriptOnModuleRunning_Click();
		}

		private void menuItemScriptOnModuleClosed_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemScriptOnModuleClosed_Click();
		}

		private void menuItemScriptOnModuleActivated_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemScriptOnModuleActivated_Click();
		}

		private void menuItemScriptOnModuleDeactivated_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemScriptOnModuleDeactivated_Click();
		}

		private void menuItemFlipHorizontal_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditFlipHorizontal(this.formChild);
		}

		private void menuItemFlipVertical_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditFlipVertical(this.formChild);
		}

		private void FormEditGraphic_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			formChild.FormEditGraphic_Closing(e);
		}

		private void menuItemViewObjectNumber_Click(object sender, System.EventArgs e)
		{
			formChild.menuItemViewObjectNumber_Click();
		}

		private void menuItemInsertObjectRoundRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertRoundRectangle(this.formChild);
		}

		private void menuItemEditRegisterToLibrary_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditRegisterToLibrary(this.formChild);
		}

		public void InsertLibrary_FromInsertClick(ObjectGroup groupCopy)
		{
			ClassEditInsert.EditInsertFrom_LibraryInsertClick(this.formChild, groupCopy);
		}

		private void menuItemWindowCascade_Click(object sender, System.EventArgs e)
		{
			SharedStudio.formMain.LayoutMdi(MdiLayout.Cascade);
		}

		private void menuItemWindowTileHorz_Click(object sender, System.EventArgs e)
		{
			SharedStudio.formMain.LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void menuItemWindowTileVert_Click(object sender, System.EventArgs e)
		{
			SharedStudio.formMain.LayoutMdi(MdiLayout.TileVertical);
		}

		private void menuItemWindowArrangeIcon_Click(object sender, System.EventArgs e)
		{
			SharedStudio.formMain.LayoutMdi(MdiLayout.ArrangeIcons);
		}

		private void menuItemWindowClose_Click(object sender, System.EventArgs e)
		{
			if(SharedStudio.formMain.ActiveMdiChild != null)
				SharedStudio.formMain.ActiveMdiChild.Close();
		}

		private void menuItemWindowCloseAll_Click(object sender, System.EventArgs e)
		{
			Form[] childForm = SharedStudio.formMain.MdiChildren; 
			//Make sure to ask for saving the doc before exiting the app 

			for(int i=0; i < childForm.Length; i++) 
				childForm[i].Close(); 
		}

		public delegate void CallBackOnMdiActivated(Form form, string filename, object obj);
		public CallBackOnMdiActivated lpfnCallOnMdiActivated;

		private void FormEditGraphic_Activated(object sender, System.EventArgs e)
		{
			statusBarMain.Panels.Clear();
			this.statusBarMain.Panels.Add(statusBarPanelPosition);
			this.statusBarMain.Panels.Add(statusBarPanelSize);

			statusBarPanelPosition.Width = 120;
			statusBarPanelSize.Width = 200;

			this.lpfnCallOnMdiActivated(formChild, formChild.workThis.filename, formChild.workThis.obj.groupRoot);
		}

		private void menuItemEditObjectProperties_Click(object sender, System.EventArgs e)
		{
			ClassEditProperty.OnUserPropertyClick(this.formChild);
		}

		private void menuItemEditObjectProperty_Click(object sender, System.EventArgs e)
		{
			ClassEditProperty.OnUserPropertyClick(this.formChild);
		}

		private void FormEditGraphic_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			
		}

		private void menuItemFileSaveAsBitmap_Click(object sender, System.EventArgs e)
		{
			FormFileSaveAsBitmap dialog = new FormFileSaveAsBitmap(this.formChild, this.formChild.workThis.obj);

			dialog.ShowDialog(this);
		}

		

		public EnumMainTool enumMainTool = EnumMainTool.ARROW;

        /*
		private void toolBarMainTool_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			EnumMainTool tool = (EnumMainTool)ConvertTool.ToInt16(e.Button.Tag.ToString());
			if(tool == enumMainTool)	return;

			formChild.OnBeforeMainToolChanged();

			enumMainTool = tool;
			SetMainIcon();

			formChild.Invalidate();
		}*/

		void SetMainIcon()
		{
            /*
			toolBarButton1.Pushed = (enumMainTool == (EnumMainTool)0);
			toolBarButton2.Pushed = (enumMainTool == (EnumMainTool)1);
			toolBarButton3.Pushed = (enumMainTool == (EnumMainTool)2);
			toolBarButton4.Pushed = (enumMainTool == (EnumMainTool)3);
			toolBarButton5.Pushed = (enumMainTool == (EnumMainTool)4);
			toolBarButton6.Pushed = (enumMainTool == (EnumMainTool)5);
			toolBarButton7.Pushed = (enumMainTool == (EnumMainTool)6);
			toolBarButton8.Pushed = (enumMainTool == (EnumMainTool)7);
			toolBarButton9.Pushed = (enumMainTool == (EnumMainTool)8);
			toolBarButton10.Pushed = (enumMainTool == (EnumMainTool)9);
			toolBarButton11.Pushed = (enumMainTool == (EnumMainTool)10);
			toolBarButton12.Pushed = (enumMainTool == (EnumMainTool)11);
            toolBarButton13.Pushed = (enumMainTool == (EnumMainTool)12);*/

            toolStripButtonMove.Checked = (enumMainTool == (EnumMainTool)0);
            toolStripButtonLine.Checked = (enumMainTool == (EnumMainTool)1);
            toolStripButtonRectangle.Checked = (enumMainTool == (EnumMainTool)2);
            toolStripButtonFillRectangle.Checked = (enumMainTool == (EnumMainTool)3);
            toolStripButtonCircle.Checked = (enumMainTool == (EnumMainTool)4);
            toolStripButtonFillCircle.Checked = (enumMainTool == (EnumMainTool)5);
            toolStripButtonPoly.Checked = (enumMainTool == (EnumMainTool)6);
            toolStripButtonFillPoly.Checked = (enumMainTool == (EnumMainTool)7);
            toolStripButtonText.Checked = (enumMainTool == (EnumMainTool)8);
            toolStripButtonRoundRectangle.Checked = (enumMainTool == (EnumMainTool)9);
            toolStripButtonCurve.Checked = (enumMainTool == (EnumMainTool)10);
            toolStripButtonPointMove.Checked = (enumMainTool == (EnumMainTool)11);
            toolStripButtonSpoid.Checked = (enumMainTool == (EnumMainTool)12);
		}

        void SetGraphicFrameToPlatformInvironment()
        {
            bool flag_win = false;//, flag_ce = false;

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.Win)
                flag_win = true;

            // PC 버전에서만 사용하는 메뉴
            tabControlToolStripMenuItem.Visible = flag_win;
            milliDataTrendToolStripMenuItem.Visible = flag_win;
            milliDataWindowToolStripMenuItem.Visible = flag_win;
            toolStripSeparator18.Visible = flag_win;            // 줄이 두개 생겨서 하나를 지워준다.

            demandWindowToolStripMenuItem.Visible = flag_win;
            demandChartToolStripMenuItem.Visible = flag_win;
            realtimeTestGraphToolStripMenuItem.Visible = flag_win;
            databaseTrendToolStripMenuItem.Visible = flag_win;
            databaseToolStripMenuItem.Visible = flag_win;
            dataGridViewToolStripMenuItem.Visible = flag_win;

            scriptOnModuleActivatedToolStripMenuItem.Visible = flag_win;
            scriptOnModuleDeactivatedToolStripMenuItem.Visible = flag_win;

            svgToolStripMenuItem.Visible = flag_win;
            VLCAxToolStripMenuItem.Visible = flag_win;
            webViewToolStripMenuItem.Visible = flag_win;

            analogGaugeToolStripMenuItem.Visible = flag_win;
            DonutChartToolStripMenuItem.Visible = flag_win;

            

            // CE 버전에서만 사용하는 메뉴
        }

		private void FormEditGraphicFrame_Load(object sender, System.EventArgs e)
		{
			SetMainIcon();
            SetGraphicFrameToPlatformInvironment();

            if (!NextVersion.bAutobase11)
            {
                // this.newObjectToolStripMenuItem.Visible = false;
            }

            if (TotalConfig.eOemType == EnumOemType.KobasAI)
            {
                this.saveAsBitmapToolStripMenuItem.Visible = false;
                this.saveAsTempleteLibraryToolStripMenuItem.Visible = false;
                this.saveAsTempleteFileToolStripMenuItem.Visible = false;
                this.toolStripSeparator4.Visible = false;
                this.importToolStripMenuItem.Visible = false;

                this.registerToLibraryToolStripMenuItem.Visible = false;
                this.buyObjectLibraryFromWebToolStripMenuItem.Visible = false;
            }

            
		}

		private void menuItemInsertObjectChangeValueDisplay_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertChangeValueDisplay(this.formChild);		
		}

		private void menuItemFileImport_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditImport(this.formChild);
		}

		private void menuItemEditLock_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditLock(this.formChild);
		}

		private void menuItemUnlockAll_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditUnLock(this.formChild);
		}

        private void menuItemInsertObjectWebBrowser_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertWebBrowser(this.formChild);
        }

        private void menuItemInsertTagAnimation_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertTagAnimation(this.formChild);
        }

        private void menuItemEditAlignToHorzCenter_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToHorzCenter(this.formChild);
        }

        private void menuItemEditAlignToVertCenter_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToVertCenter(this.formChild);
        }

        private void menuItemEditAlignToModuleHorzCenter_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToModuleHorzCenter(this.formChild);
        }

        private void menuItemEditAlignToModuleVertCenter_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToModuleVertCenter(this.formChild);
        }

        private void menuItemEditMakeSameSizeWidth_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMakeSameSize(this.formChild, true, false);
        }

        private void menuItemEditMakeSameSizeHeight_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMakeSameSize(this.formChild, false, true);
        }

        private void menuItemEditMakeSameSizeAll_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMakeSameSize(this.formChild, true, true);
        }

        private void menuItemEditAlignPopup_Popup(object sender, EventArgs e)
        {
            alignToLeftToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);
            alignToHorizontalCenterToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);
            alignToRightToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);
            alignToTopToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);
            alignToVerticalCenterToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);
            alignToBottomToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 1);

            alignToModuleHorizontalCenterToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 0);
            alignToModuleVerticalCenterToolStripMenuItem.Enabled = (formChild.workThis.nSelectCount > 0);

            spaceHorizontalToolStripMenuItem.Enabled = (formChild.workThis.nSelectCountOnlyChild > 2);
            spaceVerticalToolStripMenuItem.Enabled = (formChild.workThis.nSelectCountOnlyChild > 2);
        }

        private void menuItemInsertObjectMilliDataTrend_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertMilliDataTrend(this.formChild);
        }

        WebLibraryModCut weblibrarySearchModCut = new WebLibraryModCut();   // 이전의 상태를 기억하기 위해서 외부에 선언

        void CallLibrarySearch()
        {
            if (!WebLibraryGate.LogIn()) return;

            FormInsertFromLibrarySearch dialog = new FormInsertFromLibrarySearch(weblibrarySearchModCut);

            if (Tools.IsLangKorean())
                dialog.Text = "웹에서 요소 라이브러리 구매";

            dialog.Owner = SharedStudio.formMain;
            dialog.Show(); //250828 PSU SharedStudio.formMain 제거
        }

        private void menuItemInsertFromLibrarySearch_Click(object sender, EventArgs e)
        {
            CallLibrarySearch();
        }

        WebLibraryModFull weblibraryModFull = new WebLibraryModFull();

        private void menuItemSaveAsTemplete_Click(object sender, EventArgs e)
        {
            weblibraryModFull.nIconSize = 2;
            weblibraryModFull.groupTemp = this.formChild.workThis.obj;

            FormRegisterToLibrary dialog = new FormRegisterToLibrary(weblibraryModFull);

            if (Tools.IsLangKorean())
                dialog.Text = "템플릿 라이브러리로 등록";

            dialog.sSourceDirectory = Path.GetDirectoryName(this.formChild.workThis.obj.objCommonProperty.sModuleName);
            dialog.ShowDialog(this);
        }

        public void Tool_Click(EnumMainTool tool)
        {
            if (tool == enumMainTool) return;

            formChild.OnBeforeMainToolChanged();

            enumMainTool = tool;
            SetMainIcon();

            formChild.Invalidate();
        }

        private void toolStripButtonMove_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.ARROW);
        }

        private void toolStripButtonLine_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.LINE);
        }

        private void toolStripButtonRectangle_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.RECT);
        }

        private void toolStripButtonFillRectangle_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.RECT_FILL);
        }

        private void toolStripButtonCircle_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.CIRCLE);
        }

        private void toolStripButtonFillCircle_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.CIRCLE_FILL);
        }

        private void toolStripButtonPoly_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.POLY);
        }

        private void toolStripButtonFillPoly_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.POLY_FILL);
        }

        private void toolStripButtonText_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.TEXT);
        }

        private void toolStripButtonRoundRectangle_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.ROUND_RECTANGLE);
        }

        private void toolStripButtonCurve_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.CURVE);
        }

        private void toolStripButtonPointMove_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.POINT);
        }

        private void toolStripButtonSpoid_Click(object sender, EventArgs e)
        {
            Tool_Click(EnumMainTool.SPUIT);
        }

        private void toolStripButtonAlignToLeft_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideLeft(this.formChild);
        }

        private void toolStripButtonAlignHorizontalCenter_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToHorzCenter(this.formChild);
        }

        private void toolStripButtonAlignToRight_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideRight(this.formChild);
        }

        private void toolStripButtonInsertFromLibrary_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertFromLibrary(this.formChild);
        }

        private void toolStripButtonInsertFromLibrarySearch_Click(object sender, EventArgs e)
        {
            CallLibrarySearch();
        }

        private void menuItemFileSaveAsTempleteFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            if (Tools.IsLangKorean())
                dialog.Title = "템플릿 파일로 저장";

            WebLibraryModFull weblibrary = weblibraryModFull;

            dialog.Filter = String.Format("Templete files (*.{0})|*.{0}", weblibrary.sLibExt);
            dialog.OverwritePrompt = true;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                weblibrary.groupTemp = this.formChild.workThis.obj;
                string source_directory = Path.GetDirectoryName(this.formChild.workThis.obj.objCommonProperty.sModuleName);

                MemoryStream stream = weblibrary.ObjectToStream(source_directory, "");

                byte[] buffer = stream.ToArray();

                WebLibraryUtil.HashBuffer(buffer);
                File.WriteAllBytes(dialog.FileName, buffer);
            }
        }

        private void menuItemEditRegisterToLibraryFile_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditRegisterToLibraryFile(this.formChild);
        }

        private void menuItemInsertFromLibraryFile_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertFromLibraryFile(this);
        }

        private void toolStripButtonSpaceHorizontal_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditSpaceHorz(this.formChild);
        }

        private void toolStripButtonSpaceVertical_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditSpaceVert(this.formChild);
        }

        private void FormEditGraphicFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.lpfnCallOnMdiActivated(null, formChild.workThis.filename, null);

            formChild.workThis.obj.Dispose();                          // 2011-10-11 추가. 메모리가 누적되는 것 같다.
            ClassStudioEditUndo.UndoClose(formChild);   // 2011-10-11 추가. 메모리가 누적되는 것 같다.
        }

        private void menuItemEditFindReplaceTagLink_Click(object sender, EventArgs e)
        {
            FormFindReplaceTagLink.Run();
        }

        private void menuItemEditRotateRight_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateRight(this.formChild);            
        }

        private void menuItemEditRotateLeft_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateLeft(this.formChild);            
        }

        private void FormEditGraphicFrame_VisibleChanged(object sender, EventArgs e)
        {
            // Loading 에서는 Close가 안된다. 

            if (formChild.bFailLoading) this.Close();
        }

        void RecurseEnableAllSubMenuItems(ToolStripMenuItem item)
        {
            ToolStripItem tsi;
            for (int i = 0; i < item.DropDownItems.Count; i++)
            {
                tsi = item.DropDownItems[i];
                if (tsi.GetType() == typeof(ToolStripMenuItem))
                {
                    tsi.Enabled = true;
                    RecurseEnableAllSubMenuItems((ToolStripMenuItem)tsi);
                }
            }
        }

        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            // 상황에 맞게 메뉴가 활성/비활성화 되고 난 후 그래픽 감시에서 편집하면 비활성화 된 메뉴의 shortkey가 동작하지 않으므로 항상 메뉴 닫을 때 비활성화 된 메뉴를 살리도록 한다. 2011-12-14
            RecurseEnableAllSubMenuItems(this.editToolStripMenuItem);
        }

        private void dataGridViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertDataGridView(this.formChild);
        }

        private void DonutChartToolStripMenuItem_Click(object sender, EventArgs e) //도넛차트용 추가 hsjeong 25-02-04
        {
            ClassEditInsert.EditInsertDonutChart(this.formChild);
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rate = formChild.workThis.obj.nOpticRate;

            if (rate >= 1000) return;
            rate += 10;

            formChild.OpticGo(rate);
        }

        private void zoomOutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int rate = formChild.workThis.obj.nOpticRate;

            if (rate <= 10) return;
            rate -= 10;

            formChild.OpticGo(rate);
        }

        private void objectToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                this.dataGridViewToolStripMenuItem.Visible = false;
                this.tabControlToolStripMenuItem.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.SCADA_LITE || TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite)
            {
                milliDataWindowToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.MilliData); // 요소삽입-그래프/트랜드-미세자료 윈도우
                milliDataTrendToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.MilliData); // 요소삽입-그래프/트랜드-미세자료 트랜드
                toolStripSeparator18.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.MilliData); // 요소삽입-그래프/트랜드-미세자료 트랜드 아래의 Serapater
                databaseTrendToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database); // 요소삽입-그래프/트랜드-데이터베이스 트랜드

                databaseToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database); // 요소삽입-데이터베이스
                dataGridViewToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database); // 요소삽입-데이터그리드
            }
        }

        private void windowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void windowToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                arraygeIconToolStripMenuItem.Visible = false;
            }
        }

        private void flipRotateToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            this.flipHorizontalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(formChild);
            this.flipVerticalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(formChild);
        }

        private void flipRotateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void webViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertWebView(this.formChild);
        }

        private void VLCAxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertVLCAx(this.formChild);
        }

        private void autopasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAutoPaste(this.formChild);
        }

        private void SVGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertSVG(this.formChild);
        }

        private void toolStripButtonAlignToTop_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideTop(this.formChild);
        }

        private void toolStripButtonAlignToBottom_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideBottom(this.formChild);
        }

        private void toolStripButtonAlignVerticalCenter_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToVertCenter(this.formChild);
        }

        private void toolStripButtonMakeSameSizeWidth_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMakeSameSize(this.formChild, true, false);
        }

        private void toolStripButtonMakeSameSizeHeight_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMakeSameSize(this.formChild, false, true);
        }

        private void toolStripButtonMoveToFront_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMovePrevFront(this.formChild);
        }

        private void toolStripButtonMoveToBack_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMoveNextBack(this.formChild);
        }

        private void toolStripButtonRotateLeft_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateLeft(this.formChild);  
        }

        private void toolStripButtonRotateRight_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateRight(this.formChild);  
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItemClipboardPaste_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditClipboardPaste(this.formChild);
        }

        private void chartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertCustomChart(this.formChild);
        }

        private void menuItemInsertObjectDemandChart_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertDemandChart(this.formChild);
        }

        private void menuItemInsertBarcodeDisplay_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertBarcodeDisplay(this.formChild);
        }

        private void menuItemInsertBarcodeScanner_Click(object sender, EventArgs e)
        {
            ClassEditInsert.EditInsertBarcodeScanner(this.formChild);
        }
    }
}

