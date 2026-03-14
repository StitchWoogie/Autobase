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
using ICSharpCode.SharpZipLib.Zip;
using PicTools;
using Studio.Script;
 
namespace Studio
{
	/// <summary>
	/// Summary description for FormEditGraphic.
	/// </summary>
	public class FormEditGraphic : System.Windows.Forms.Form
    {
        private IContainer components;

		static public ArrayList arrayFormGraphFrame = new ArrayList();
		public WORK_MODULE_STRUCT workThis;
		public bool		bDisplayObjectNumber;

		ClassEditObjectLine editLine = new ClassEditObjectLine();
		public ClassEditObjectMove editMove = new ClassEditObjectMove();
		ClassEditObjectRect editRect = new ClassEditObjectRect();
		ClassEditObjectCircle editCircle = new ClassEditObjectCircle();
		ClassEditObjectRoundRect editRoundRect = new ClassEditObjectRoundRect();
		ClassEditObjectPoly editPoly = new ClassEditObjectPoly();
		ClassEditObjectCurve editCurve = new ClassEditObjectCurve();
		ClassEditObjectPointMove editPointMove = new ClassEditObjectPointMove();
		ClassEditObjectText editText = new ClassEditObjectText();
        ClassEditObjectSpuit editSpuit = new ClassEditObjectSpuit();
		ClassMainTool		editElse = new ClassMainTool();
		private System.Windows.Forms.Label label1;
		int nNewFlag = 0;   // open mode

		public ArrayList undoList = new ArrayList();
        public int nUndoPos = 0;
        public ContextMenuStrip contextMenuStripMove;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toCurveObjectToolStripMenuItem;
        private ToolStripMenuItem combineToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem groupToolStripMenuItem;
        private ToolStripMenuItem ungroupToolStripMenuItem;
        private ToolStripMenuItem mergeToBitmapToolStripMenuItem;
        private ToolStripMenuItem unmergeBitmapToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItemZoom10;
        private ToolStripMenuItem toolStripMenuItemZoom25;
        private ToolStripMenuItem toolStripMenuItemZoom50;
        private ToolStripMenuItem toolStripMenuItemZoom75;
        private ToolStripMenuItem toolStripMenuItemZoom100;
        private ToolStripMenuItem toolStripMenuItemZoom150;
        private ToolStripMenuItem toolStripMenuItemZoom200;
        private ToolStripMenuItem toolStripMenuItemZoom300;
        private ToolStripMenuItem toolStripMenuItemZoom400;
        private ToolStripMenuItem toolStripMenuItemZoom500;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem objectPropertiesToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemZoom600;
        private ToolStripMenuItem toolStripMenuItemZoom700;
        private ToolStripMenuItem toolStripMenuItemZoom800;
        private ToolStripMenuItem toolStripMenuItemZoom900;
        private ToolStripMenuItem toolStripMenuItemZoom1000;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem toAnimationEditorToolStripMenuItem;
        public ContextMenuStrip contextMenuStripPointMove;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem toLineToolStripMenuItem;
        private ToolStripMenuItem toCurveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem cuspToolStripMenuItem;
        private ToolStripMenuItem smoothToolStripMenuItem;
        private ToolStripMenuItem symmetricalToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem autoCloseToolStripMenuItem;
        private ToolStripMenuItem joinToolStripMenuItem;
        private ToolStripMenuItem breakApartToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem objectPropertiesToolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem alignToLeftToolStripMenuItem;
        private ToolStripMenuItem alignToHorizontalCenterToolStripMenuItem;
        private ToolStripMenuItem alignToRightToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem alignToTopToolStripMenuItem;
        private ToolStripMenuItem alignToVerticalCenterToolStripMenuItem;
        private ToolStripMenuItem alignToBottomToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem alignToModuleHorizontalCenterToolStripMenuItem;
        private ToolStripMenuItem alignToModuleVerticalCenterToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem spaceHorizontalToolStripMenuItem;
        private ToolStripMenuItem spaceVerticalToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemOrder;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem moveToFrontToolStripMenuItem;
        private ToolStripMenuItem moveToBackToolStripMenuItem;
        private ToolStripMenuItem moveToPrevFrontToolStripMenuItem;
        private ToolStripMenuItem moveToNextBackToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem flipHorizontalToolStripMenuItem;
        private ToolStripMenuItem flipVerticalToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem rotate90DegreeToRightToolStripMenuItem;
        private ToolStripMenuItem rotate90DegreeToLeftToolStripMenuItem;
        private ToolStripMenuItem breakObjectToolStripMenuItem;
		public FormEditGraphicFrame formFrame;
        // Panning Mode 추가 24-06-27 hsjeong
        public EnumViewMode cViewMode = EnumViewMode.CONTROL;
        Cursor cursorPanning = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Studio.Cursor.Panning.cur"));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="new_flag">0 = open, 1 = new, 2 = ObjectGroup</param>
        /// <param name="frame"></param>
		public FormEditGraphic(string filename, int new_flag, FormEditGraphicFrame frame, ObjectRoot root)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            // 마우스 휠 이벤트 등록
            this.MouseWheel += new MouseEventHandler(FormGraphic_MouseWheel);  //20250212 PSU 추가

			label1.ForeColor = Color.FromArgb(0,0,0,0);
			label1.BackColor = Color.FromArgb(0,0,0,0);

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			workThis = new WORK_MODULE_STRUCT();

            if (new_flag == 2)
            {
                workThis.obj = root;

                workThis.obj.objCommonProperty = new ObjectCommonProperty();    // 템플릿에서 가져온 경우는 환경을 새로 클리어하는 것이 낫다. 스트림에서 가져왔기 때문에 환경이 많이 다르다.
                workThis.obj.objCommonProperty.form = this;
                workThis.obj.SetCommonProperty();
            }

			string path;
			if(Path.IsPathRooted(filename))	path = filename;
			else							path = String.Format("{0}\\graphic\\{1}", TotalConfig.sDirWorkProject, filename);
			workThis.filename = path;
			workThis.obj.objCommonProperty.sModuleName = path;

			this.VScroll = false;
			nNewFlag = new_flag;

			formFrame = frame;

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                if (Tools.IsLangKorean())
                {
                    this.toolStripMenuItem3.Text = "위치교환/크기반전(&I)";
                    this.flipHorizontalToolStripMenuItem.Text = "좌우 위치교환(&H)";
                    this.flipVerticalToolStripMenuItem.Text = "상하 위치교환(&V)";
                    this.rotate90DegreeToLeftToolStripMenuItem.Text = "왼쪽으로 90도 크기반전(&L)";
                    this.rotate90DegreeToRightToolStripMenuItem.Text = "오른쪽으로 90도 크기반전(&R)";
                }

                this.toolStripSeparator13.Visible = false;
                this.spaceHorizontalToolStripMenuItem.Visible = false;
                this.spaceVerticalToolStripMenuItem.Visible = false;
            }
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditGraphic));
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStripMove = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toCurveObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemOrder = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToPrevFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToNextBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToHorizontalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.alignToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToVerticalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.alignToModuleHorizontalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToModuleVerticalCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.spaceHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spaceVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.flipHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.rotate90DegreeToRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90DegreeToLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ungroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeToBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unmergeBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom25 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom75 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom150 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom200 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom300 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom400 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom500 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom600 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom700 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom800 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom900 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemZoom1000 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.objectPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toAnimationEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripPointMove = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toCurveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.cuspToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.symmetricalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.autoCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakApartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.objectPropertiesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMove.SuspendLayout();
            this.contextMenuStripPointMove.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // contextMenuStripMove
            // 
            this.contextMenuStripMove.AccessibleDescription = null;
            this.contextMenuStripMove.AccessibleName = null;
            resources.ApplyResources(this.contextMenuStripMove, "contextMenuStripMove");
            this.contextMenuStripMove.BackgroundImage = null;
            this.contextMenuStripMove.Font = null;
            this.contextMenuStripMove.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator1,
            this.toCurveObjectToolStripMenuItem,
            this.combineToolStripMenuItem,
            this.breakObjectToolStripMenuItem,
            this.toolStripSeparator2,
            this.toolStripMenuItemOrder,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripSeparator10,
            this.groupToolStripMenuItem,
            this.ungroupToolStripMenuItem,
            this.mergeToBitmapToolStripMenuItem,
            this.unmergeBitmapToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolStripMenuItem1,
            this.toolStripSeparator4,
            this.objectPropertiesToolStripMenuItem,
            this.toolStripSeparator5,
            this.toAnimationEditorToolStripMenuItem});
            this.contextMenuStripMove.Name = "contextMenuStripMove";
            this.contextMenuStripMove.Opened += new System.EventHandler(this.contextMenuStripMove_Opened);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.AccessibleDescription = null;
            this.undoToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.BackgroundImage = null;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.AccessibleDescription = null;
            this.redoToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
            this.redoToolStripMenuItem.BackgroundImage = null;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toCurveObjectToolStripMenuItem
            // 
            this.toCurveObjectToolStripMenuItem.AccessibleDescription = null;
            this.toCurveObjectToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.toCurveObjectToolStripMenuItem, "toCurveObjectToolStripMenuItem");
            this.toCurveObjectToolStripMenuItem.BackgroundImage = null;
            this.toCurveObjectToolStripMenuItem.Name = "toCurveObjectToolStripMenuItem";
            this.toCurveObjectToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.toCurveObjectToolStripMenuItem.Click += new System.EventHandler(this.toCurveObjectToolStripMenuItem_Click);
            // 
            // combineToolStripMenuItem
            // 
            this.combineToolStripMenuItem.AccessibleDescription = null;
            this.combineToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.combineToolStripMenuItem, "combineToolStripMenuItem");
            this.combineToolStripMenuItem.BackgroundImage = null;
            this.combineToolStripMenuItem.Name = "combineToolStripMenuItem";
            this.combineToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.combineToolStripMenuItem.Click += new System.EventHandler(this.combineToolStripMenuItem_Click);
            // 
            // breakObjectToolStripMenuItem
            // 
            this.breakObjectToolStripMenuItem.AccessibleDescription = null;
            this.breakObjectToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.breakObjectToolStripMenuItem, "breakObjectToolStripMenuItem");
            this.breakObjectToolStripMenuItem.BackgroundImage = null;
            this.breakObjectToolStripMenuItem.Name = "breakObjectToolStripMenuItem";
            this.breakObjectToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.breakObjectToolStripMenuItem.Click += new System.EventHandler(this.breakObjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AccessibleDescription = null;
            this.toolStripSeparator2.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripMenuItemOrder
            // 
            this.toolStripMenuItemOrder.AccessibleDescription = null;
            this.toolStripMenuItemOrder.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemOrder, "toolStripMenuItemOrder");
            this.toolStripMenuItemOrder.BackgroundImage = null;
            this.toolStripMenuItemOrder.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToFrontToolStripMenuItem,
            this.moveToBackToolStripMenuItem,
            this.moveToPrevFrontToolStripMenuItem,
            this.moveToNextBackToolStripMenuItem});
            this.toolStripMenuItemOrder.Name = "toolStripMenuItemOrder";
            this.toolStripMenuItemOrder.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemOrder.DropDownOpened += new System.EventHandler(this.toolStripMenuItemOrder_DropDownOpened);
            // 
            // moveToFrontToolStripMenuItem
            // 
            this.moveToFrontToolStripMenuItem.AccessibleDescription = null;
            this.moveToFrontToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.moveToFrontToolStripMenuItem, "moveToFrontToolStripMenuItem");
            this.moveToFrontToolStripMenuItem.BackgroundImage = null;
            this.moveToFrontToolStripMenuItem.Name = "moveToFrontToolStripMenuItem";
            this.moveToFrontToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.moveToFrontToolStripMenuItem.Click += new System.EventHandler(this.moveToFrontToolStripMenuItem_Click);
            // 
            // moveToBackToolStripMenuItem
            // 
            this.moveToBackToolStripMenuItem.AccessibleDescription = null;
            this.moveToBackToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.moveToBackToolStripMenuItem, "moveToBackToolStripMenuItem");
            this.moveToBackToolStripMenuItem.BackgroundImage = null;
            this.moveToBackToolStripMenuItem.Name = "moveToBackToolStripMenuItem";
            this.moveToBackToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.moveToBackToolStripMenuItem.Click += new System.EventHandler(this.moveToBackToolStripMenuItem_Click);
            // 
            // moveToPrevFrontToolStripMenuItem
            // 
            this.moveToPrevFrontToolStripMenuItem.AccessibleDescription = null;
            this.moveToPrevFrontToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.moveToPrevFrontToolStripMenuItem, "moveToPrevFrontToolStripMenuItem");
            this.moveToPrevFrontToolStripMenuItem.BackgroundImage = null;
            this.moveToPrevFrontToolStripMenuItem.Name = "moveToPrevFrontToolStripMenuItem";
            this.moveToPrevFrontToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.moveToPrevFrontToolStripMenuItem.Click += new System.EventHandler(this.moveToPrevFrontToolStripMenuItem_Click);
            // 
            // moveToNextBackToolStripMenuItem
            // 
            this.moveToNextBackToolStripMenuItem.AccessibleDescription = null;
            this.moveToNextBackToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.moveToNextBackToolStripMenuItem, "moveToNextBackToolStripMenuItem");
            this.moveToNextBackToolStripMenuItem.BackgroundImage = null;
            this.moveToNextBackToolStripMenuItem.Name = "moveToNextBackToolStripMenuItem";
            this.moveToNextBackToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.moveToNextBackToolStripMenuItem.Click += new System.EventHandler(this.moveToNextBackToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.AccessibleDescription = null;
            this.toolStripMenuItem2.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.BackgroundImage = null;
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignToLeftToolStripMenuItem,
            this.alignToHorizontalCenterToolStripMenuItem,
            this.alignToRightToolStripMenuItem,
            this.toolStripSeparator11,
            this.alignToTopToolStripMenuItem,
            this.alignToVerticalCenterToolStripMenuItem,
            this.alignToBottomToolStripMenuItem,
            this.toolStripSeparator12,
            this.alignToModuleHorizontalCenterToolStripMenuItem,
            this.alignToModuleVerticalCenterToolStripMenuItem,
            this.toolStripSeparator13,
            this.spaceHorizontalToolStripMenuItem,
            this.spaceVerticalToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeyDisplayString = null;
            this.toolStripMenuItem2.DropDownOpened += new System.EventHandler(this.toolStripMenuItem2_DropDownOpened);
            // 
            // alignToLeftToolStripMenuItem
            // 
            this.alignToLeftToolStripMenuItem.AccessibleDescription = null;
            this.alignToLeftToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToLeftToolStripMenuItem, "alignToLeftToolStripMenuItem");
            this.alignToLeftToolStripMenuItem.BackgroundImage = null;
            this.alignToLeftToolStripMenuItem.Name = "alignToLeftToolStripMenuItem";
            this.alignToLeftToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToLeftToolStripMenuItem.Click += new System.EventHandler(this.alignToLeftToolStripMenuItem_Click);
            // 
            // alignToHorizontalCenterToolStripMenuItem
            // 
            this.alignToHorizontalCenterToolStripMenuItem.AccessibleDescription = null;
            this.alignToHorizontalCenterToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToHorizontalCenterToolStripMenuItem, "alignToHorizontalCenterToolStripMenuItem");
            this.alignToHorizontalCenterToolStripMenuItem.BackgroundImage = null;
            this.alignToHorizontalCenterToolStripMenuItem.Name = "alignToHorizontalCenterToolStripMenuItem";
            this.alignToHorizontalCenterToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToHorizontalCenterToolStripMenuItem.Click += new System.EventHandler(this.alignToHorizontalCenterToolStripMenuItem_Click);
            // 
            // alignToRightToolStripMenuItem
            // 
            this.alignToRightToolStripMenuItem.AccessibleDescription = null;
            this.alignToRightToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToRightToolStripMenuItem, "alignToRightToolStripMenuItem");
            this.alignToRightToolStripMenuItem.BackgroundImage = null;
            this.alignToRightToolStripMenuItem.Name = "alignToRightToolStripMenuItem";
            this.alignToRightToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToRightToolStripMenuItem.Click += new System.EventHandler(this.alignToRightToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.AccessibleDescription = null;
            this.toolStripSeparator11.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            // 
            // alignToTopToolStripMenuItem
            // 
            this.alignToTopToolStripMenuItem.AccessibleDescription = null;
            this.alignToTopToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToTopToolStripMenuItem, "alignToTopToolStripMenuItem");
            this.alignToTopToolStripMenuItem.BackgroundImage = null;
            this.alignToTopToolStripMenuItem.Name = "alignToTopToolStripMenuItem";
            this.alignToTopToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToTopToolStripMenuItem.Click += new System.EventHandler(this.alignToTopToolStripMenuItem_Click);
            // 
            // alignToVerticalCenterToolStripMenuItem
            // 
            this.alignToVerticalCenterToolStripMenuItem.AccessibleDescription = null;
            this.alignToVerticalCenterToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToVerticalCenterToolStripMenuItem, "alignToVerticalCenterToolStripMenuItem");
            this.alignToVerticalCenterToolStripMenuItem.BackgroundImage = null;
            this.alignToVerticalCenterToolStripMenuItem.Name = "alignToVerticalCenterToolStripMenuItem";
            this.alignToVerticalCenterToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToVerticalCenterToolStripMenuItem.Click += new System.EventHandler(this.alignToVerticalCenterToolStripMenuItem_Click);
            // 
            // alignToBottomToolStripMenuItem
            // 
            this.alignToBottomToolStripMenuItem.AccessibleDescription = null;
            this.alignToBottomToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToBottomToolStripMenuItem, "alignToBottomToolStripMenuItem");
            this.alignToBottomToolStripMenuItem.BackgroundImage = null;
            this.alignToBottomToolStripMenuItem.Name = "alignToBottomToolStripMenuItem";
            this.alignToBottomToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToBottomToolStripMenuItem.Click += new System.EventHandler(this.alignToBottomToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.AccessibleDescription = null;
            this.toolStripSeparator12.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            // 
            // alignToModuleHorizontalCenterToolStripMenuItem
            // 
            this.alignToModuleHorizontalCenterToolStripMenuItem.AccessibleDescription = null;
            this.alignToModuleHorizontalCenterToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToModuleHorizontalCenterToolStripMenuItem, "alignToModuleHorizontalCenterToolStripMenuItem");
            this.alignToModuleHorizontalCenterToolStripMenuItem.BackgroundImage = null;
            this.alignToModuleHorizontalCenterToolStripMenuItem.Name = "alignToModuleHorizontalCenterToolStripMenuItem";
            this.alignToModuleHorizontalCenterToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToModuleHorizontalCenterToolStripMenuItem.Click += new System.EventHandler(this.alignToModuleHorizontalCenterToolStripMenuItem_Click);
            // 
            // alignToModuleVerticalCenterToolStripMenuItem
            // 
            this.alignToModuleVerticalCenterToolStripMenuItem.AccessibleDescription = null;
            this.alignToModuleVerticalCenterToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.alignToModuleVerticalCenterToolStripMenuItem, "alignToModuleVerticalCenterToolStripMenuItem");
            this.alignToModuleVerticalCenterToolStripMenuItem.BackgroundImage = null;
            this.alignToModuleVerticalCenterToolStripMenuItem.Name = "alignToModuleVerticalCenterToolStripMenuItem";
            this.alignToModuleVerticalCenterToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.alignToModuleVerticalCenterToolStripMenuItem.Click += new System.EventHandler(this.alignToModuleVerticalCenterToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.AccessibleDescription = null;
            this.toolStripSeparator13.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            // 
            // spaceHorizontalToolStripMenuItem
            // 
            this.spaceHorizontalToolStripMenuItem.AccessibleDescription = null;
            this.spaceHorizontalToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.spaceHorizontalToolStripMenuItem, "spaceHorizontalToolStripMenuItem");
            this.spaceHorizontalToolStripMenuItem.BackgroundImage = null;
            this.spaceHorizontalToolStripMenuItem.Name = "spaceHorizontalToolStripMenuItem";
            this.spaceHorizontalToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.spaceHorizontalToolStripMenuItem.Click += new System.EventHandler(this.spaceHorizontalToolStripMenuItem_Click);
            // 
            // spaceVerticalToolStripMenuItem
            // 
            this.spaceVerticalToolStripMenuItem.AccessibleDescription = null;
            this.spaceVerticalToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.spaceVerticalToolStripMenuItem, "spaceVerticalToolStripMenuItem");
            this.spaceVerticalToolStripMenuItem.BackgroundImage = null;
            this.spaceVerticalToolStripMenuItem.Name = "spaceVerticalToolStripMenuItem";
            this.spaceVerticalToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.spaceVerticalToolStripMenuItem.Click += new System.EventHandler(this.spaceVerticalToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.AccessibleDescription = null;
            this.toolStripMenuItem3.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.BackgroundImage = null;
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flipHorizontalToolStripMenuItem,
            this.flipVerticalToolStripMenuItem,
            this.toolStripSeparator14,
            this.rotate90DegreeToRightToolStripMenuItem,
            this.rotate90DegreeToLeftToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.ShortcutKeyDisplayString = null;
            this.toolStripMenuItem3.DropDownOpened += new System.EventHandler(this.toolStripMenuItem3_DropDownOpened);
            // 
            // flipHorizontalToolStripMenuItem
            // 
            this.flipHorizontalToolStripMenuItem.AccessibleDescription = null;
            this.flipHorizontalToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.flipHorizontalToolStripMenuItem, "flipHorizontalToolStripMenuItem");
            this.flipHorizontalToolStripMenuItem.BackgroundImage = null;
            this.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem";
            this.flipHorizontalToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.flipHorizontalToolStripMenuItem.Click += new System.EventHandler(this.flipHorizontalToolStripMenuItem_Click);
            // 
            // flipVerticalToolStripMenuItem
            // 
            this.flipVerticalToolStripMenuItem.AccessibleDescription = null;
            this.flipVerticalToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.flipVerticalToolStripMenuItem, "flipVerticalToolStripMenuItem");
            this.flipVerticalToolStripMenuItem.BackgroundImage = null;
            this.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem";
            this.flipVerticalToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.flipVerticalToolStripMenuItem.Click += new System.EventHandler(this.flipVerticalToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.AccessibleDescription = null;
            this.toolStripSeparator14.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            // 
            // rotate90DegreeToRightToolStripMenuItem
            // 
            this.rotate90DegreeToRightToolStripMenuItem.AccessibleDescription = null;
            this.rotate90DegreeToRightToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.rotate90DegreeToRightToolStripMenuItem, "rotate90DegreeToRightToolStripMenuItem");
            this.rotate90DegreeToRightToolStripMenuItem.BackgroundImage = null;
            this.rotate90DegreeToRightToolStripMenuItem.Name = "rotate90DegreeToRightToolStripMenuItem";
            this.rotate90DegreeToRightToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.rotate90DegreeToRightToolStripMenuItem.Click += new System.EventHandler(this.rotate90DegreeToRightToolStripMenuItem_Click);
            // 
            // rotate90DegreeToLeftToolStripMenuItem
            // 
            this.rotate90DegreeToLeftToolStripMenuItem.AccessibleDescription = null;
            this.rotate90DegreeToLeftToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.rotate90DegreeToLeftToolStripMenuItem, "rotate90DegreeToLeftToolStripMenuItem");
            this.rotate90DegreeToLeftToolStripMenuItem.BackgroundImage = null;
            this.rotate90DegreeToLeftToolStripMenuItem.Name = "rotate90DegreeToLeftToolStripMenuItem";
            this.rotate90DegreeToLeftToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.rotate90DegreeToLeftToolStripMenuItem.Click += new System.EventHandler(this.rotate90DegreeToLeftToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.AccessibleDescription = null;
            this.toolStripSeparator10.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            // 
            // groupToolStripMenuItem
            // 
            this.groupToolStripMenuItem.AccessibleDescription = null;
            this.groupToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.groupToolStripMenuItem, "groupToolStripMenuItem");
            this.groupToolStripMenuItem.BackgroundImage = null;
            this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
            this.groupToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.groupToolStripMenuItem.Click += new System.EventHandler(this.groupToolStripMenuItem_Click);
            // 
            // ungroupToolStripMenuItem
            // 
            this.ungroupToolStripMenuItem.AccessibleDescription = null;
            this.ungroupToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.ungroupToolStripMenuItem, "ungroupToolStripMenuItem");
            this.ungroupToolStripMenuItem.BackgroundImage = null;
            this.ungroupToolStripMenuItem.Name = "ungroupToolStripMenuItem";
            this.ungroupToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.ungroupToolStripMenuItem.Click += new System.EventHandler(this.ungroupToolStripMenuItem_Click);
            // 
            // mergeToBitmapToolStripMenuItem
            // 
            this.mergeToBitmapToolStripMenuItem.Name = "mergeToBitmapToolStripMenuItem";
            this.mergeToBitmapToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.mergeToBitmapToolStripMenuItem.Text = "Merge to Bitmap";
            this.mergeToBitmapToolStripMenuItem.Click += new System.EventHandler(this.mergeToBitmapToolStripMenuItem_Click);
            // 
            // unmergeBitmapToolStripMenuItem
            // 
            this.unmergeBitmapToolStripMenuItem.Name = "unmergeBitmapToolStripMenuItem";
            this.unmergeBitmapToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.unmergeBitmapToolStripMenuItem.Text = "Unmerge Bitmap";
            this.unmergeBitmapToolStripMenuItem.Click += new System.EventHandler(this.unmergeBitmapToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AccessibleDescription = null;
            this.toolStripSeparator3.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.AccessibleDescription = null;
            this.toolStripMenuItem1.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.BackgroundImage = null;
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemZoom10,
            this.toolStripMenuItemZoom25,
            this.toolStripMenuItemZoom50,
            this.toolStripMenuItemZoom75,
            this.toolStripMenuItemZoom100,
            this.toolStripMenuItemZoom150,
            this.toolStripMenuItemZoom200,
            this.toolStripMenuItemZoom300,
            this.toolStripMenuItemZoom400,
            this.toolStripMenuItemZoom500,
            this.toolStripMenuItemZoom600,
            this.toolStripMenuItemZoom700,
            this.toolStripMenuItemZoom800,
            this.toolStripMenuItemZoom900,
            this.toolStripMenuItemZoom1000});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeyDisplayString = null;
            this.toolStripMenuItem1.DropDownOpened += new System.EventHandler(this.toolStripMenuItem1_DropDownOpened);
            // 
            // toolStripMenuItemZoom10
            // 
            this.toolStripMenuItemZoom10.AccessibleDescription = null;
            this.toolStripMenuItemZoom10.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom10, "toolStripMenuItemZoom10");
            this.toolStripMenuItemZoom10.BackgroundImage = null;
            this.toolStripMenuItemZoom10.Name = "toolStripMenuItemZoom10";
            this.toolStripMenuItemZoom10.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom10.Click += new System.EventHandler(this.toolStripMenuItemZoom10_Click);
            // 
            // toolStripMenuItemZoom25
            // 
            this.toolStripMenuItemZoom25.AccessibleDescription = null;
            this.toolStripMenuItemZoom25.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom25, "toolStripMenuItemZoom25");
            this.toolStripMenuItemZoom25.BackgroundImage = null;
            this.toolStripMenuItemZoom25.Name = "toolStripMenuItemZoom25";
            this.toolStripMenuItemZoom25.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom25.Click += new System.EventHandler(this.toolStripMenuItemZoom25_Click);
            // 
            // toolStripMenuItemZoom50
            // 
            this.toolStripMenuItemZoom50.AccessibleDescription = null;
            this.toolStripMenuItemZoom50.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom50, "toolStripMenuItemZoom50");
            this.toolStripMenuItemZoom50.BackgroundImage = null;
            this.toolStripMenuItemZoom50.Name = "toolStripMenuItemZoom50";
            this.toolStripMenuItemZoom50.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom50.Click += new System.EventHandler(this.toolStripMenuItemZoom50_Click);
            // 
            // toolStripMenuItemZoom75
            // 
            this.toolStripMenuItemZoom75.AccessibleDescription = null;
            this.toolStripMenuItemZoom75.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom75, "toolStripMenuItemZoom75");
            this.toolStripMenuItemZoom75.BackgroundImage = null;
            this.toolStripMenuItemZoom75.Name = "toolStripMenuItemZoom75";
            this.toolStripMenuItemZoom75.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom75.Click += new System.EventHandler(this.toolStripMenuItemZoom75_Click);
            // 
            // toolStripMenuItemZoom100
            // 
            this.toolStripMenuItemZoom100.AccessibleDescription = null;
            this.toolStripMenuItemZoom100.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom100, "toolStripMenuItemZoom100");
            this.toolStripMenuItemZoom100.BackgroundImage = null;
            this.toolStripMenuItemZoom100.Name = "toolStripMenuItemZoom100";
            this.toolStripMenuItemZoom100.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom100.Click += new System.EventHandler(this.toolStripMenuItemZoom100_Click);
            // 
            // toolStripMenuItemZoom150
            // 
            this.toolStripMenuItemZoom150.AccessibleDescription = null;
            this.toolStripMenuItemZoom150.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom150, "toolStripMenuItemZoom150");
            this.toolStripMenuItemZoom150.BackgroundImage = null;
            this.toolStripMenuItemZoom150.Name = "toolStripMenuItemZoom150";
            this.toolStripMenuItemZoom150.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom150.Click += new System.EventHandler(this.toolStripMenuItemZoom150_Click);
            // 
            // toolStripMenuItemZoom200
            // 
            this.toolStripMenuItemZoom200.AccessibleDescription = null;
            this.toolStripMenuItemZoom200.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom200, "toolStripMenuItemZoom200");
            this.toolStripMenuItemZoom200.BackgroundImage = null;
            this.toolStripMenuItemZoom200.Name = "toolStripMenuItemZoom200";
            this.toolStripMenuItemZoom200.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom200.Click += new System.EventHandler(this.toolStripMenuItemZoom200_Click);
            // 
            // toolStripMenuItemZoom300
            // 
            this.toolStripMenuItemZoom300.AccessibleDescription = null;
            this.toolStripMenuItemZoom300.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom300, "toolStripMenuItemZoom300");
            this.toolStripMenuItemZoom300.BackgroundImage = null;
            this.toolStripMenuItemZoom300.Name = "toolStripMenuItemZoom300";
            this.toolStripMenuItemZoom300.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom300.Click += new System.EventHandler(this.toolStripMenuItemZoom300_Click);
            // 
            // toolStripMenuItemZoom400
            // 
            this.toolStripMenuItemZoom400.AccessibleDescription = null;
            this.toolStripMenuItemZoom400.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom400, "toolStripMenuItemZoom400");
            this.toolStripMenuItemZoom400.BackgroundImage = null;
            this.toolStripMenuItemZoom400.Name = "toolStripMenuItemZoom400";
            this.toolStripMenuItemZoom400.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom400.Click += new System.EventHandler(this.toolStripMenuItemZoom400_Click);
            // 
            // toolStripMenuItemZoom500
            // 
            this.toolStripMenuItemZoom500.AccessibleDescription = null;
            this.toolStripMenuItemZoom500.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom500, "toolStripMenuItemZoom500");
            this.toolStripMenuItemZoom500.BackgroundImage = null;
            this.toolStripMenuItemZoom500.Name = "toolStripMenuItemZoom500";
            this.toolStripMenuItemZoom500.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom500.Click += new System.EventHandler(this.toolStripMenuItemZoom500_Click);
            // 
            // toolStripMenuItemZoom600
            // 
            this.toolStripMenuItemZoom600.AccessibleDescription = null;
            this.toolStripMenuItemZoom600.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom600, "toolStripMenuItemZoom600");
            this.toolStripMenuItemZoom600.BackgroundImage = null;
            this.toolStripMenuItemZoom600.Name = "toolStripMenuItemZoom600";
            this.toolStripMenuItemZoom600.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom600.Click += new System.EventHandler(this.toolStripMenuItemZoom600_Click);
            // 
            // toolStripMenuItemZoom700
            // 
            this.toolStripMenuItemZoom700.AccessibleDescription = null;
            this.toolStripMenuItemZoom700.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom700, "toolStripMenuItemZoom700");
            this.toolStripMenuItemZoom700.BackgroundImage = null;
            this.toolStripMenuItemZoom700.Name = "toolStripMenuItemZoom700";
            this.toolStripMenuItemZoom700.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom700.Click += new System.EventHandler(this.toolStripMenuItemZoom700_Click);
            // 
            // toolStripMenuItemZoom800
            // 
            this.toolStripMenuItemZoom800.AccessibleDescription = null;
            this.toolStripMenuItemZoom800.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom800, "toolStripMenuItemZoom800");
            this.toolStripMenuItemZoom800.BackgroundImage = null;
            this.toolStripMenuItemZoom800.Name = "toolStripMenuItemZoom800";
            this.toolStripMenuItemZoom800.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom800.Click += new System.EventHandler(this.toolStripMenuItemZoom800_Click);
            // 
            // toolStripMenuItemZoom900
            // 
            this.toolStripMenuItemZoom900.AccessibleDescription = null;
            this.toolStripMenuItemZoom900.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom900, "toolStripMenuItemZoom900");
            this.toolStripMenuItemZoom900.BackgroundImage = null;
            this.toolStripMenuItemZoom900.Name = "toolStripMenuItemZoom900";
            this.toolStripMenuItemZoom900.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom900.Click += new System.EventHandler(this.toolStripMenuItemZoom900_Click);
            // 
            // toolStripMenuItemZoom1000
            // 
            this.toolStripMenuItemZoom1000.AccessibleDescription = null;
            this.toolStripMenuItemZoom1000.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItemZoom1000, "toolStripMenuItemZoom1000");
            this.toolStripMenuItemZoom1000.BackgroundImage = null;
            this.toolStripMenuItemZoom1000.Name = "toolStripMenuItemZoom1000";
            this.toolStripMenuItemZoom1000.ShortcutKeyDisplayString = null;
            this.toolStripMenuItemZoom1000.Click += new System.EventHandler(this.toolStripMenuItemZoom1000_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AccessibleDescription = null;
            this.toolStripSeparator4.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // objectPropertiesToolStripMenuItem
            // 
            this.objectPropertiesToolStripMenuItem.AccessibleDescription = null;
            this.objectPropertiesToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.objectPropertiesToolStripMenuItem, "objectPropertiesToolStripMenuItem");
            this.objectPropertiesToolStripMenuItem.BackgroundImage = null;
            this.objectPropertiesToolStripMenuItem.Name = "objectPropertiesToolStripMenuItem";
            this.objectPropertiesToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.objectPropertiesToolStripMenuItem.Click += new System.EventHandler(this.objectPropertiesToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.AccessibleDescription = null;
            this.toolStripSeparator5.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // toAnimationEditorToolStripMenuItem
            // 
            this.toAnimationEditorToolStripMenuItem.AccessibleDescription = null;
            this.toAnimationEditorToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.toAnimationEditorToolStripMenuItem, "toAnimationEditorToolStripMenuItem");
            this.toAnimationEditorToolStripMenuItem.BackgroundImage = null;
            this.toAnimationEditorToolStripMenuItem.Name = "toAnimationEditorToolStripMenuItem";
            this.toAnimationEditorToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.toAnimationEditorToolStripMenuItem.Click += new System.EventHandler(this.toAnimationEditorToolStripMenuItem_Click);
            // 
            // contextMenuStripPointMove
            // 
            this.contextMenuStripPointMove.AccessibleDescription = null;
            this.contextMenuStripPointMove.AccessibleName = null;
            resources.ApplyResources(this.contextMenuStripPointMove, "contextMenuStripPointMove");
            this.contextMenuStripPointMove.BackgroundImage = null;
            this.contextMenuStripPointMove.Font = null;
            this.contextMenuStripPointMove.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator6,
            this.toLineToolStripMenuItem,
            this.toCurveToolStripMenuItem,
            this.toolStripSeparator7,
            this.cuspToolStripMenuItem,
            this.smoothToolStripMenuItem,
            this.symmetricalToolStripMenuItem,
            this.toolStripSeparator8,
            this.autoCloseToolStripMenuItem,
            this.joinToolStripMenuItem,
            this.breakApartToolStripMenuItem,
            this.toolStripSeparator9,
            this.objectPropertiesToolStripMenuItem1});
            this.contextMenuStripPointMove.Name = "contextMenuStripPointMove";
            this.contextMenuStripPointMove.Opened += new System.EventHandler(this.contextMenuStripPointMove_Opened);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.AccessibleDescription = null;
            this.addToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.BackgroundImage = null;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.AccessibleDescription = null;
            this.deleteToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.BackgroundImage = null;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.AccessibleDescription = null;
            this.toolStripSeparator6.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // toLineToolStripMenuItem
            // 
            this.toLineToolStripMenuItem.AccessibleDescription = null;
            this.toLineToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.toLineToolStripMenuItem, "toLineToolStripMenuItem");
            this.toLineToolStripMenuItem.BackgroundImage = null;
            this.toLineToolStripMenuItem.Name = "toLineToolStripMenuItem";
            this.toLineToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.toLineToolStripMenuItem.Click += new System.EventHandler(this.toLineToolStripMenuItem_Click);
            // 
            // toCurveToolStripMenuItem
            // 
            this.toCurveToolStripMenuItem.AccessibleDescription = null;
            this.toCurveToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.toCurveToolStripMenuItem, "toCurveToolStripMenuItem");
            this.toCurveToolStripMenuItem.BackgroundImage = null;
            this.toCurveToolStripMenuItem.Name = "toCurveToolStripMenuItem";
            this.toCurveToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.toCurveToolStripMenuItem.Click += new System.EventHandler(this.toCurveToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.AccessibleDescription = null;
            this.toolStripSeparator7.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // cuspToolStripMenuItem
            // 
            this.cuspToolStripMenuItem.AccessibleDescription = null;
            this.cuspToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.cuspToolStripMenuItem, "cuspToolStripMenuItem");
            this.cuspToolStripMenuItem.BackgroundImage = null;
            this.cuspToolStripMenuItem.Name = "cuspToolStripMenuItem";
            this.cuspToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.cuspToolStripMenuItem.Click += new System.EventHandler(this.cuspToolStripMenuItem_Click);
            // 
            // smoothToolStripMenuItem
            // 
            this.smoothToolStripMenuItem.AccessibleDescription = null;
            this.smoothToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.smoothToolStripMenuItem, "smoothToolStripMenuItem");
            this.smoothToolStripMenuItem.BackgroundImage = null;
            this.smoothToolStripMenuItem.Name = "smoothToolStripMenuItem";
            this.smoothToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.smoothToolStripMenuItem.Click += new System.EventHandler(this.smoothToolStripMenuItem_Click);
            // 
            // symmetricalToolStripMenuItem
            // 
            this.symmetricalToolStripMenuItem.AccessibleDescription = null;
            this.symmetricalToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.symmetricalToolStripMenuItem, "symmetricalToolStripMenuItem");
            this.symmetricalToolStripMenuItem.BackgroundImage = null;
            this.symmetricalToolStripMenuItem.Name = "symmetricalToolStripMenuItem";
            this.symmetricalToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.symmetricalToolStripMenuItem.Click += new System.EventHandler(this.symmetricalToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.AccessibleDescription = null;
            this.toolStripSeparator8.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // autoCloseToolStripMenuItem
            // 
            this.autoCloseToolStripMenuItem.AccessibleDescription = null;
            this.autoCloseToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.autoCloseToolStripMenuItem, "autoCloseToolStripMenuItem");
            this.autoCloseToolStripMenuItem.BackgroundImage = null;
            this.autoCloseToolStripMenuItem.Name = "autoCloseToolStripMenuItem";
            this.autoCloseToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.autoCloseToolStripMenuItem.Click += new System.EventHandler(this.autoCloseToolStripMenuItem_Click);
            // 
            // joinToolStripMenuItem
            // 
            this.joinToolStripMenuItem.AccessibleDescription = null;
            this.joinToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.joinToolStripMenuItem, "joinToolStripMenuItem");
            this.joinToolStripMenuItem.BackgroundImage = null;
            this.joinToolStripMenuItem.Name = "joinToolStripMenuItem";
            this.joinToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.joinToolStripMenuItem.Click += new System.EventHandler(this.joinToolStripMenuItem_Click);
            // 
            // breakApartToolStripMenuItem
            // 
            this.breakApartToolStripMenuItem.AccessibleDescription = null;
            this.breakApartToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.breakApartToolStripMenuItem, "breakApartToolStripMenuItem");
            this.breakApartToolStripMenuItem.BackgroundImage = null;
            this.breakApartToolStripMenuItem.Name = "breakApartToolStripMenuItem";
            this.breakApartToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.breakApartToolStripMenuItem.Click += new System.EventHandler(this.breakApartToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.AccessibleDescription = null;
            this.toolStripSeparator9.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            // 
            // objectPropertiesToolStripMenuItem1
            // 
            this.objectPropertiesToolStripMenuItem1.AccessibleDescription = null;
            this.objectPropertiesToolStripMenuItem1.AccessibleName = null;
            resources.ApplyResources(this.objectPropertiesToolStripMenuItem1, "objectPropertiesToolStripMenuItem1");
            this.objectPropertiesToolStripMenuItem1.BackgroundImage = null;
            this.objectPropertiesToolStripMenuItem1.Name = "objectPropertiesToolStripMenuItem1";
            this.objectPropertiesToolStripMenuItem1.ShortcutKeyDisplayString = null;
            this.objectPropertiesToolStripMenuItem1.Click += new System.EventHandler(this.objectPropertiesToolStripMenuItem1_Click);
            // 
            // FormEditGraphic
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.label1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "FormEditGraphic";
            this.Load += new System.EventHandler(this.FormEditGraphic_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormEditGraphic_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormEditGraphic_Paint);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FormEditGraphic_Scroll);
            this.SizeChanged += new System.EventHandler(this.FormEditGraphic_SizeChanged);
            this.DoubleClick += new System.EventHandler(this.FormEditGraphic_DoubleClick);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormEditGraphic_DragDrop);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormEditGraphic_MouseDown);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormEditGraphic_DragEnter);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormEditGraphic_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormEditGraphic_KeyUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormEditGraphic_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormEditGraphic_KeyDown);
            this.contextMenuStripMove.ResumeLayout(false);
            this.contextMenuStripPointMove.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        public bool bFailLoading = false;

		private void FormEditGraphic_Load(object sender, System.EventArgs e)
		{
            if(nNewFlag == 0)   // Open mode
			{
                if (!workThis.obj.Load(this, workThis.filename))
                {
                    bFailLoading = true;
                    return;
                }
			}
            else if (nNewFlag == 2) // create by templete
            {
                this.FileSave();
                SharedStudio.formSolution.ReLoad(); // 새로 만들어진 파일이므로 솔루션에 보이도록 한다.
            }

			workThis.obj.SetScreenSize(ClientSize.Width, ClientSize.Height);

			label1.Left = -label1.Width;

			ScrollUpdate();

			SetTitle();

            // 2007.6.14 OnPaintBitmap 대신 사용할 수 있다. 이 함수로 화면 떨림을 예방할 수 있다. OnPaintBitmap(Memory dc)을 사용하면 화면위에 투명한 윈도우가 오면 화면 떨림이 발생한다.
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                this.toolStripSeparator5.Visible = false;
                this.toAnimationEditorToolStripMenuItem.Visible = false;
            }
		}

		private void FormEditGraphic_SizeChanged(object sender, System.EventArgs e)
		{
			if(this.IsDisposed)	return;

			workThis.obj.SetScreenSize(ClientSize.Width, ClientSize.Height);
			ScrollUpdate();
			Invalidate();
		}

		public void SetTitle()
		{
			string title;

			string dir = TotalConfig.sDirWorkProject+"\\graphic";
			// 작업폴더 밑에 있는 파일이다.
			if(String.Compare(workThis.filename, 0, dir, 0, dir.Length, true) == 0)
				title = workThis.filename.Substring(dir.Length+1);
			else
				title = workThis.filename;

			if(workThis.bChangeFlag) 
			{
				title += " *";
			}

			title += String.Format(", {0}%", workThis.obj.nOpticRate);

			this.formFrame.Text = title;
		}

		public void SetChangeFlag()
		{
			if(!workThis.bChangeFlag) 
			{
				workThis.bChangeFlag = true;
				SetTitle();
			}
            Layer.FormLayer.UpdateObjectCountIfVisible();
        }

		ClassMainTool GetToolClass()
		{
			switch(formFrame.enumMainTool)
			{
				case EnumMainTool.ARROW:
					return editMove;
				case EnumMainTool.LINE:
					return editLine;
				case EnumMainTool.RECT:
					editRect.bFillFlag = false;
					return editRect;
				case EnumMainTool.RECT_FILL:
					editRect.bFillFlag = true;
					return editRect;
				case EnumMainTool.CIRCLE:
					editCircle.bFillFlag = false;
					return editCircle;
				case EnumMainTool.CIRCLE_FILL:
					editCircle.bFillFlag = true;
					return editCircle;
				case EnumMainTool.ROUND_RECTANGLE:
					return editRoundRect;
				case EnumMainTool.POLY:
					editPoly.bFillFlag = false;
					return editPoly;
				case EnumMainTool.POLY_FILL:
					editPoly.bFillFlag = true;
					return editPoly;
				case EnumMainTool.CURVE:
					return editCurve;
				case EnumMainTool.POINT:
					return editPointMove;
				case EnumMainTool.TEXT:
					return editText;
                case EnumMainTool.SPUIT:
                    return editSpuit;

				default:
					return editElse;
			}
		}
		
		private void FormEditGraphic_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(this.WindowState == FormWindowState.Minimized)	                return;
			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)	    return;
			workThis.obj.Display(e.Graphics, this.ClientRectangle, e.ClipRectangle, this.AutoScrollPosition);

			workThis.nScrollHorPos = -AutoScrollPosition.X;
			workThis.nScrollVerPos = -AutoScrollPosition.Y;

			DrawGuideLine(e, workThis);
			ObjectNotAll(e.Graphics, workThis);

			GetToolClass().Paint(this, e.Graphics);

			int drawx = workThis.obj.nModuleSizeX*workThis.obj.nOpticRate/100+this.AutoScrollPosition.X;
			int drawy = workThis.obj.nModuleSizeY*workThis.obj.nOpticRate/100+this.AutoScrollPosition.Y;
			e.Graphics.DrawLine(Pens.Black, drawx, 0, drawx, drawy);
			e.Graphics.DrawLine(Pens.Black, 0, drawy, drawx, drawy);

			if(bDisplayObjectNumber) 
			{
				int x1=0, y1=0, x2=0, y2=0;
				Font font;
				
				if(Tools.IsLangKorean())
					font = new Font("Gulim", 9);
				else
					font = new Font("Tahoma", 9);

				for(int i = 0; i < workThis.obj.groupRoot.GetObjectHap(); i++) 
				{
					workThis.obj.groupRoot.GetZone(i, ref x1, ref y1, ref x2, ref y2);
					x1 = GetDisplayPosX(x1);
					y1 = GetDisplayPosY(y1);
					SafeException.SafeDrawString(e.Graphics, i.ToString(), font, Brushes.Black, x1, y1); 
				}
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 

		} 

		public void ScrollUpdate()
		{
			Rectangle r = workThis.obj.groupRoot.GetObjectExistZone();

			int width, height;
			
			if(r.Right > workThis.obj.nModuleSizeX)
				width = r.Right*workThis.obj.nOpticRate/100;
			else
				width = workThis.obj.nModuleSizeX*workThis.obj.nOpticRate/100;

			if(r.Bottom > workThis.obj.nModuleSizeY)
				height = r.Bottom*workThis.obj.nOpticRate/100;
			else
				height = workThis.obj.nModuleSizeY*workThis.obj.nOpticRate/100;

			Size size = new Size(width, height);

			if(this.ClientRectangle.Width >= width && this.ClientRectangle.Height >= height) 
			{
				this.AutoScrollMargin = new Size(0, 0);
				//this.AutoScroll = false;
			}
			else 
			{
				this.AutoScrollMargin = size;// 아래의 this.autoscroll 이 먼저오면 안된다. (사이즈를 결정하고 enable 할것.
                //this.AutoScroll = true;
			}
		}



        // 250731 PSU
        public void ScrollToSelectedObject()
        {
            if (workThis.nSelectCount == 0) return;

            // 선택된 오브젝트들의 영역 계산
            Rectangle selectedBounds = GetSelectedObjectsBounds();
            if (selectedBounds.IsEmpty) return;

            ScrollToRectangle(selectedBounds);
        }

        // 선택된 오브젝트들의 전체 영역을 구하는 메서드
        private Rectangle GetSelectedObjectsBounds()
        {
            Rectangle bounds = Rectangle.Empty;
            bool first = true;

            RecurseGetSelectedBounds(workThis.obj.groupRoot, ref bounds, ref first);

            return bounds;
        }

        // 재귀적으로 선택된 오브젝트들의 영역을 구함
        private void RecurseGetSelectedBounds(ObjectPublicGroupLayer group, ref Rectangle bounds, ref bool first)
        {
            ObjectExpand type;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectExpand)group.GetPoint(i);

                if (type.bOnStudioSelected)
                {
                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    type.GetZone(ref x1, ref y1, ref x2, ref y2);

                    // 화면 좌표로 변환
                    x1 = type.GetViewPosXOn100Percent(x1);
                    y1 = type.GetViewPosYOn100Percent(y1);
                    x2 = type.GetViewPosXOn100Percent(x2);
                    y2 = type.GetViewPosYOn100Percent(y2);

                    Rectangle objRect = new Rectangle(
                        Math.Min(x1, x2),
                        Math.Min(y1, y2),
                        Math.Abs(x2 - x1),
                        Math.Abs(y2 - y1)
                    );

                    if (first)
                    {
                        bounds = objRect;
                        first = false;
                    }
                    else
                    {
                        bounds = Rectangle.Union(bounds, objRect);
                    }
                }

                // 하위 그룹/레이어도 검사
                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    RecurseGetSelectedBounds((ObjectPublicGroupLayer)type, ref bounds, ref first);
                }
            }
        }


        // 특정 오브젝트를 찾아서 스크롤하는 메서드 (검색 기능용) 250731 PSU
        public void ScrollToObject(ObjectExpand targetObject)
        {
            if (targetObject == null) return;

            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            targetObject.GetZone(ref x1, ref y1, ref x2, ref y2);

            x1 = targetObject.GetViewPosXOn100Percent(x1);
            y1 = targetObject.GetViewPosYOn100Percent(y1);
            x2 = targetObject.GetViewPosXOn100Percent(x2);
            y2 = targetObject.GetViewPosYOn100Percent(y2);

            Rectangle objRect = new Rectangle(
                Math.Min(x1, x2),
                Math.Min(y1, y2),
                Math.Abs(x2 - x1),
                Math.Abs(y2 - y1)
            );

            ScrollToRectangle(objRect);
        }

        // 특정 영역이 보이도록 스크롤 이동 250731 PSU
        private void ScrollToRectangle(Rectangle targetRect)
        {
            // 현재 보이는 영역
            Rectangle viewRect = new Rectangle(
                this.AutoScrollPosition.X * -1,
                this.AutoScrollPosition.Y * -1,
                this.ClientRectangle.Width,
                this.ClientRectangle.Height
            );

            // 타겟이 이미 완전히 보이는지 확인
            if (viewRect.Contains(targetRect))
            {
                return; // 이미 보이므로 스크롤 불필요
            }

            // 새로운 스크롤 위치 계산
            Point newScrollPos = CalculateOptimalScrollPosition(targetRect, viewRect);

            // 스크롤 적용
            this.AutoScrollPosition = newScrollPos;
            this.Invalidate();
        }

        // 최적의 스크롤 위치 계산 250731 PSU
        private Point CalculateOptimalScrollPosition(Rectangle targetRect, Rectangle viewRect)
        {
            int newX = viewRect.X;
            int newY = viewRect.Y;

            // 타겟이 뷰포트보다 큰 경우: 타겟의 중심을 뷰포트 중심에 맞춤
            if (targetRect.Width > viewRect.Width || targetRect.Height > viewRect.Height)
            {
                newX = targetRect.X + targetRect.Width / 2 - viewRect.Width / 2;
                newY = targetRect.Y + targetRect.Height / 2 - viewRect.Height / 2;
            }
            else
            {
                // X축 스크롤 계산
                if (targetRect.Left < viewRect.Left)
                {
                    // 타겟이 왼쪽으로 벗어남
                    newX = targetRect.Left - 20; // 20픽셀 여백
                }
                else if (targetRect.Right > viewRect.Right)
                {
                    // 타겟이 오른쪽으로 벗어남
                    newX = targetRect.Right - viewRect.Width + 20;
                }

                // Y축 스크롤 계산
                if (targetRect.Top < viewRect.Top)
                {
                    // 타겟이 위쪽으로 벗어남
                    newY = targetRect.Top - 20; // 20픽셀 여백
                }
                else if (targetRect.Bottom > viewRect.Bottom)
                {
                    // 타겟이 아래쪽으로 벗어남
                    newY = targetRect.Bottom - viewRect.Height + 20;
                }
            }

            // 스크롤 범위 제한
            newX = Math.Max(0, newX);
            newY = Math.Max(0, newY);

            return new Point(newX, newY);
        }


        private void FormEditGraphic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// this.label1.Select() focus나 Select를 실행하면 스크롤이 된상태에서 다시 처음으로 돌아온다.

			this.Select();	// 이 부분이 없으면 MDI창이 하나일 때 솔루션을 선택한 후 키보드 포커스를 MDI로 위치할 수가 없다.

            //if(e.Button == MouseButtons.Left)
            //    GetToolClass().MouseDownLeft(this, e);
            //else if(e.Button == MouseButtons.Right) 
            //    GetToolClass().MouseDownRight(this, e);
            //else {}  // Panning Mode 추가 위해 비활성화 24-06-27 hsjeong

            if (e.Button == MouseButtons.Left) // Panning Mode 기능 추가 24-06-27 hsjeong
            {
                if (cViewMode == EnumViewMode.PANNING)
                {
                    this.Capture = true;
                    bMouseCaptureFlag = true;
                    nCaptureX = e.X;
                    nCaptureY = e.Y;

                    Point point = this.AutoScrollPosition;
                    nCaptureScrollX = point.X;
                    nCaptureScrollY = point.Y;
                    this.Cursor = cursorPanning;
                    return;
                }
                else GetToolClass().MouseDownLeft(this, e);
            }

            else if (e.Button == MouseButtons.Right)
                GetToolClass().MouseDownRight(this, e);
            else { }

		}

		void DrawMousePosition(int x, int y)
		{
			string buf;
			
			x = GetPicturePosX(x);
			y = GetPicturePosY(y);

			buf = String.Format("mx:{0}, my:{1}", x, y);
			formFrame.statusBarPanelPosition.Text = buf;
		}
        
         bool bMouseCaptureFlag = false; //panning mode 기능 위해 추가 24-06-27 hsejong

        int nCaptureX, nCaptureY;				// mouse capture 할 당시의 커서 위치값.
        int nCaptureScrollX, nCaptureScrollY;	// mouse capture 할 당시의 커서 위치값.

		private void FormEditGraphic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            //DrawMousePosition(e.X, e.Y);
            //GetToolClass().MouseMove(this, e);

            //Panning 모드 추가 24-06-27 hsejong
            if (cViewMode == EnumViewMode.PANNING)
            {


                if (bMouseCaptureFlag)
                {	// mouse capture 중이다.
                    int movex = nCaptureX - e.X;
                    int movey = nCaptureY - e.Y;

                    Point point = this.AutoScrollPosition;

                    point.X = movex - nCaptureScrollX;
                    point.Y = movey - nCaptureScrollY;

                    this.AutoScrollPosition = point;



                    Invalidate();
                }
            }
            else
            {

                DrawMousePosition(e.X, e.Y);
                GetToolClass().MouseMove(this, e);
            }

		}

		private void FormEditGraphic_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            //if(e.Button == MouseButtons.Left)
            //    GetToolClass().MouseUpLeft(this, e);
            //else if(e.Button == MouseButtons.Right)
            //    GetToolClass().MouseUpRight(this, e);
            //else {}
            
            // panning mode 추가 24-06-27 hsejong
            if (e.Button == MouseButtons.Left)
            {
                if (cViewMode == EnumViewMode.PANNING)
                {
                    this.Capture = false;
                    bMouseCaptureFlag = false;
                    //this.Cursor = Cursors.Arrow;  //20250212 PSU 커서 깜빡임 제거.
                    return;
                }
                else GetToolClass().MouseUpLeft(this, e);
            }



            else if (e.Button == MouseButtons.Right)
                GetToolClass().MouseUpRight(this, e);
            else { }


		}

		private void FormEditGraphic_DoubleClick(object sender, System.EventArgs e)
		{
            //GetToolClass().DoubleClick(this); //panning mode 추가 24-06-27 hsjeong

            if (cViewMode == EnumViewMode.PANNING)
            {
                this.Capture = false;
                bMouseCaptureFlag = false;
                this.Cursor = Cursors.Arrow;
                return;
            }
            else GetToolClass().DoubleClick(this);

		}

		public void OnBeforeMainToolChanged()
		{
			GetToolClass().OnBeforeMainToolChanged();
		}

		void NotFillRectangle(Graphics g, int x, int y, bool first_object)
		{
            Brush brush;

            if (first_object)
                brush = new SolidBrush(Color.FromArgb(128, 0, 255, 255));
            else
                brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

			g.FillRectangle(brush, x-2, y-2, 5, 5);
			Pen pen = new Pen(Color.Black, 1);
			g.DrawRectangle(pen, x-3, y-3, 6, 6);
		}

        void NotFillRectangleThick(Graphics g, int x, int y, bool first_object)
		{
			Brush brush;
            
            if(first_object) 
                brush = new SolidBrush(Color.FromArgb(128, 0, 255, 255));
            else
                brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

			g.FillRectangle(brush, x-4, y-4, 9, 9);
			Pen pen = new Pen(Color.Black, 1);
			g.DrawRectangle(pen, x-5, y-5, 10, 10);
		}

		void NotRectangle(Graphics g, int x1, int y1, int x2, int y2, ObjectType obj, bool first_object)
		{
			EnumNotType not_type;

			if(obj == null) 
			{
				not_type = EnumNotType.NOT_TYPE_RECT;
			}
			else 
			{
				not_type = obj.GetNotType();
			}
	
			if(not_type == EnumNotType.NOT_TYPE_LINE) 
			{
				if(editMove.bMouseLeftFlag)
					g.DrawLine(Pens.Black, x1, y1, x2, y2);
				NotFillRectangle(g, x1, y1, first_object);
                NotFillRectangle(g, x2, y2, first_object);
			}
			else if(not_type == EnumNotType.NOT_TYPE_POINT8) 
			{
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

                NotFillRectangle(g, x1, y1, first_object);
                NotFillRectangle(g, x1, y2, first_object);
                NotFillRectangle(g, x2, y1, first_object);
                NotFillRectangle(g, x2, y2, first_object);

                NotFillRectangle(g, x1, y1 + (y2 - y1) / 2, first_object);
                NotFillRectangle(g, x2, y1 + (y2 - y1) / 2, first_object);
                NotFillRectangle(g, x1 + (x2 - x1) / 2, y1, first_object);
                NotFillRectangle(g, x1 + (x2 - x1) / 2, y2, first_object);
			}
			else if(not_type == EnumNotType.NOT_TYPE_GROUP) 
			{
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

                NotFillRectangleThick(g, x1, y1, first_object);
                NotFillRectangleThick(g, x1, y2, first_object);
                NotFillRectangleThick(g, x2, y1, first_object);
                NotFillRectangleThick(g, x2, y2, first_object);

                NotFillRectangleThick(g, x1, y1 + (y2 - y1) / 2, first_object);
                NotFillRectangleThick(g, x2, y1 + (y2 - y1) / 2, first_object);
                NotFillRectangleThick(g, x1 + (x2 - x1) / 2, y1, first_object);
                NotFillRectangleThick(g, x1 + (x2 - x1) / 2, y2, first_object);
			}
			else 
			{
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

                NotFillRectangle(g, x1, y1, first_object);
                NotFillRectangle(g, x1, y2, first_object);
                NotFillRectangle(g, x2, y1, first_object);
                NotFillRectangle(g, x2, y2, first_object);

                NotFillRectangle(g, x1, y1 + (y2 - y1) / 2, first_object);
                NotFillRectangle(g, x2, y1 + (y2 - y1) / 2, first_object);
                NotFillRectangle(g, x1 + (x2 - x1) / 2, y1, first_object);
                NotFillRectangle(g, x1 + (x2 - x1) / 2, y2, first_object);
			}
		}

		void NotRectangle(Graphics g, int x1, int y1, int x2, int y2, bool first_object)
		{
			NotRectangle(g, x1, y1, x2, y2, null, first_object);
		}

		void DrawObjectSize(int x, int y, int width, int height)
		{
			if(width == -1 && height == -1)	 
			{
				formFrame.statusBarPanelSize.Text = "";
				return;
			}

			string buf;
			buf = String.Format("x:{0}, y:{1}, w:{2}, h:{3}", x, y, width, height);
			formFrame.statusBarPanelSize.Text = buf;
		}

		public void DrawMakingObjectSize(int sx, int sy, int mx, int my)
		{
			int x1 = GetPicturePosX(sx);
			int y1 = GetPicturePosX(sy);
			int x2 = GetPicturePosX(mx);
			int y2 = GetPicturePosX(my);

			int width = Math.Abs(x2-x1)+1;
			int height = Math.Abs(y2-y1)+1;

			DrawObjectSize(x1, y1, width, height); 
		}

		public void DrawSelectedObjectSize(WORK_MODULE_STRUCT work)
		{
			if(work.nSelectCount == 0) 
			{
				DrawObjectSize(-1, -1, -1, -1);
				return;
			}

			int x1, y1, x2, y2;
			int i;

			x1 = work.selectList[0].x1;
			y1 = work.selectList[0].y1;
			x2 = work.selectList[0].x2;
			y2 = work.selectList[0].y2;

			for(i = 0; i < work.nSelectCount; i++) 
			{
				if(work.selectList[i].x1 < x1)	x1 = work.selectList[i].x1;
				if(work.selectList[i].x2 < x1)	x1 = work.selectList[i].x2;

				if(work.selectList[i].y1 < y1)	y1 = work.selectList[i].y1;
				if(work.selectList[i].y2 < y1)	y1 = work.selectList[i].y2;

				if(work.selectList[i].x1 > x2)	x2 = work.selectList[i].x1;
				if(work.selectList[i].x2 > x2)	x2 = work.selectList[i].x2;

				if(work.selectList[i].y1 > y2)	y2 = work.selectList[i].y1;
				if(work.selectList[i].y2 > y2)	y2 = work.selectList[i].y2;

			}

			int width = Math.Abs(x2-x1)+1;
			int height = Math.Abs(y2-y1)+1;
		
			DrawObjectSize(x1, y1, width, height);
		}

        // 선택 리스트가 변경되고 해야 할일
        public void DisplayAfterSelectedChanged()
        {
            ClassEditProperty.SelectChanged(this);
            Layer.FormLayer.SelectedChanged();
			DrawSelectedObjectSize(this.workThis);
        }

        // 선택 리스트가 변경되고 해야 할일 레이어에서 부르므로 레이어 관련 함수를 Call하지 말것
        public void DisplayAfterSelectedChangedCallByLayer()
        {
            ClassEditProperty.SelectChanged(this);
            DrawSelectedObjectSize(this.workThis);
        }

        public void SelectListAdd(WORK_MODULE_STRUCT work, object obj)
        {
            SELECT_LIST[] list;
            int i;

            list = new SELECT_LIST[work.nSelectCount + 1];

            if (list == null) return;		// 메모리를 할당할 수 없다.
            for (i = 0; i < work.nSelectCount; i++)
            {
                list[i] = work.selectList[i];
            }

            ((ObjectExpand)obj).GetZone(ref list[i].x1, ref list[i].y1, ref list[i].x2, ref list[i].y2);

            ((ObjectExpand)obj).bOnStudioSelected = true;

            list[i].obj = obj;
            ClassEditInsert.RecurseMakePos(work.obj.groupRoot, obj, ref list[i].opos);

            work.selectList = list;
            work.selectListOnlyParent = SelectListMakeOnlyParent();
            work.selectListOnlyChild = SelectListMakeOnlyChild();
        }
        
		public void SelectListUpdateFirstItem()
		{
			WORK_MODULE_STRUCT work = workThis;

			if(work.nSelectCount != 1)	return;

			int i = 0;

            ((ObjectExpand)work.selectList[i].obj).GetZone(ref work.selectList[i].x1, ref work.selectList[i].y1, ref work.selectList[i].x2, ref work.selectList[i].y2);
		}

        /// <summary>
        /// 영역의 크기만을 Update한다.
        /// </summary>
		public void SelectListUpdate()
		{
			WORK_MODULE_STRUCT work = workThis;

			int i = 0; 

			for(i = 0; i < work.nSelectCount; i++) 
			{
                ((ObjectExpand)work.selectList[i].obj).GetZone(ref work.selectList[i].x1, ref work.selectList[i].y1, ref work.selectList[i].x2, ref work.selectList[i].y2);
                
			}
		}

        /// <summary>
        /// 위치 정보를 가지고 object와 Zone을 Update한다. 오브젝트가 바뀐경우에 사용
        /// </summary>
        public void SelectListUpdateByPos()
        {
            WORK_MODULE_STRUCT work = workThis;

            int i = 0;

            for (i = 0; i < work.nSelectCount; i++)
            {
                work.selectList[i].obj = work.obj.groupRoot.RecurseSeekObject(work.selectList[i].opos, 0);
                ((ObjectExpand)work.selectList[i].obj).GetZone(ref work.selectList[i].x1, ref work.selectList[i].y1, ref work.selectList[i].x2, ref work.selectList[i].y2);
            }
        }

		public void SelectListDel(WORK_MODULE_STRUCT work, object obj)
		{
            ((ObjectType)obj).bOnStudioSelected = false;

			SELECT_LIST[] list;
			int i, j;

			if(work.nSelectCount == 1) 
			{
                ((ObjectType)work.selectList[0].obj).bOnStudioSelected = false;
				work.selectList = null;
				return;
			}

			for(i = 0; i < work.nSelectCount; i++) 
			{
                if (work.selectList[i].obj == obj) goto go;
			}
			return;	// can't seek match pos

			go:
				list = new SELECT_LIST[work.nSelectCount-1];

			if(list == null)	return;		// 메모리를 할당할 수 없다.
	
			for(i = 0; i < work.nSelectCount; i++) 
			{
                if (work.selectList[i].obj == obj) 
				{
					for(j = i; j < work.nSelectCount-1; j++) 
					{
						list[j] = work.selectList[j+1];
					}
					break;
				}
				list[i] = work.selectList[i];
			}

			work.selectList = list;
            work.selectListOnlyParent = SelectListMakeOnlyParent();
            work.selectListOnlyChild = SelectListMakeOnlyChild();
		}

        public void RecurseSelectAll(ObjectPublicGroupLayer gl)
        {
            ObjectExpand obj;

            for (int i = 0; i < gl.GetObjectHap(); i++)
            {
                obj = (ObjectExpand)gl.GetPoint(i);

                if (!obj.objGeneral.bOnStudioLocked && obj.objGeneral.bOnStudioVisible)
                {
                    obj.bOnStudioSelected = true;

                    if (obj.enumObjectType == EnumObjectType.Layer)
                    {
                        RecurseSelectAll((ObjectPublicGroupLayer)obj);
                    }
                }
            }
        }

        void RecurseUnSelectAll(ObjectPublicGroupLayer gl)
        {
            ObjectType obj;

            for (int i = 0; i < gl.GetObjectHap(); i++)
            {
                obj = (ObjectType)gl.GetPoint(i);

                obj.bOnStudioSelected = false;

                if (obj.enumObjectType == EnumObjectType.Layer || obj.enumObjectType == EnumObjectType.Group)
                {
                    RecurseUnSelectAll((ObjectPublicGroupLayer)obj);
                }
            }
        }

        public void SelectListClear(WORK_MODULE_STRUCT work)
        {
            RecurseUnSelectAll(work.obj.groupRoot);

            work.selectList = null;
            work.selectListOnlyParent = null;
            work.selectListOnlyChild = null;
        }

        int RecurseUpdateSelectListGetCount(ObjectPublicGroupLayer group, bool only_parent, bool only_child)
        {
            ObjectType type;
            int count = 0;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectType)group.GetPoint(i);

                if (type.enumObjectType == EnumObjectType.Layer)
                {
                    if (type.bOnStudioSelected && !only_child)
                    {
                        count++;
                        if (only_parent) continue; // only_parent 일 경우 자식 object는 계산하지 않는다.
                    }

                    count += RecurseUpdateSelectListGetCount((ObjectPublicGroupLayer)type, only_parent, only_child);
                }
                else if (type.enumObjectType == EnumObjectType.Group)
                {
                    if (type.bOnStudioSelected) count++;
                    else
                    {
                        count += RecurseUpdateSelectListGetCount((ObjectPublicGroupLayer)type, only_parent, only_child);
                    }
                }
                else
                {
                    if (type.bOnStudioSelected) count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 오브젝트를 검사해서 선택되어 있는 오브젝트를 SELECT_LIST로 만든다.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="pos"></param>
        /// <param name="list"></param>
        /// <param name="opos"></param>
        void RecurseUpdateSelectList(ObjectPublicGroupLayer group, ref int pos, ref SELECT_LIST[] list, int[] opos, bool only_parent, bool only_child)
        {
            ObjectType type;
            int depth;

            if (opos == null)   depth = 0;
            else                depth = opos.Length;

            int[] opos2 = new int[depth+1];
            
            for (int i = 0; i < depth; i++)
            {
                opos2[i] = opos[i];
            }

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                opos2[depth] = i;

                type = (ObjectType)group.GetPoint(i);

                if (type.enumObjectType == EnumObjectType.Layer)
                {
                    if (type.bOnStudioSelected && !only_child)
                    {
                        ((ObjectLayer)type).GetLayerZone(ref list[pos].x1, ref list[pos].y1, ref list[pos].x2, ref list[pos].y2);

                        list[pos].opos = (int[])opos2.Clone();
                        list[pos].obj = type;

                        pos++;

                        if (only_parent) continue; // only_parent 일 경우 자식 object는 계산하지 않는다.
                    }

                    RecurseUpdateSelectList((ObjectPublicGroupLayer)type, ref pos, ref list, opos2, only_parent, only_child);
                }
                else if (type.enumObjectType == EnumObjectType.Group)
                {
                    if (type.bOnStudioSelected)
                    {
                        group.GetZone(i, ref list[pos].x1, ref list[pos].y1, ref list[pos].x2, ref list[pos].y2);

                        list[pos].opos = (int[])opos2.Clone();
                        list[pos].obj = type;

                        pos++;
                    }
                    else
                    {
                        RecurseUpdateSelectList((ObjectPublicGroupLayer)type, ref pos, ref list, opos2, only_parent, only_child);
                    }
                }
                else
                {
                    if (type.bOnStudioSelected)
                    {
                        group.GetZone(i, ref list[pos].x1, ref list[pos].y1, ref list[pos].x2, ref list[pos].y2);

                        //list[pos].parent = group;
                        list[pos].opos = (int[])opos2.Clone();
                        list[pos].obj = type;

                        pos++;
                    }
                }
            }
        }

        /// <summary>
        /// 선택 리스트를 다시 만든다. (선택된 모든 오브젝트를 리스트화 한다.)
        /// </summary>
        public void SelectListReMake()
        {
            int count = this.RecurseUpdateSelectListGetCount(workThis.obj.groupRoot, false, false);

            SELECT_LIST[] list = null;

            if (count > 0)
                list = new SELECT_LIST[count];

            int pos = 0;

            RecurseUpdateSelectList(workThis.obj.groupRoot, ref pos, ref list, null, false, false);

            workThis.selectList = list;
            workThis.selectListOnlyParent = SelectListMakeOnlyParent();
            workThis.selectListOnlyChild = SelectListMakeOnlyChild();
        }

        /// <summary>
        /// 선택 리스트를 만든다. (Layer 가 선택된 경우 child 는 포함하지 않는다.)
        /// </summary>
        /// 
        public SELECT_LIST[] SelectListMakeOnlyParent()
        {
            int count = this.RecurseUpdateSelectListGetCount(workThis.obj.groupRoot, true, false);

            SELECT_LIST[] list = null;

            if (count > 0)
                list = new SELECT_LIST[count];

            int pos = 0;

            RecurseUpdateSelectList(workThis.obj.groupRoot, ref pos, ref list, null, true, false);

            // 첫번째 선택한 오브젝트는 정렬의 기준이 되는 경우가 있으므로 첫번째는 같은 오브젝트로 만들어 준다.
            if (workThis.selectList != null)
            {
                object obj = workThis.selectList[0].obj;

                for (int i = 0; i < count; i++)
                {
                    if (list[i].obj == obj)
                    {
                        if (i == 0) break;

                        SELECT_LIST temp = list[i];
                        list[i] = list[0];
                        list[0] = temp;

                        break;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 선택 리스트를 만든다. (Layer인 경우 child 만 포함한다.)
        /// </summary>
        /// 
        public SELECT_LIST[] SelectListMakeOnlyChild()
        {
            int count = this.RecurseUpdateSelectListGetCount(workThis.obj.groupRoot, false, true);

            SELECT_LIST[] list = null;

            if (count > 0)
                list = new SELECT_LIST[count];

            int pos = 0;

            RecurseUpdateSelectList(workThis.obj.groupRoot, ref pos, ref list, null, false, true);

            // 첫번째 선택한 오브젝트는 정렬의 기준이 되는 경우가 있으므로 첫번째는 같은 오브젝트로 만들어 준다.
            if (workThis.selectList != null)
            {
                object obj = workThis.selectList[0].obj;

                for (int i = 0; i < count; i++)
                {
                    if (list[i].obj == obj)
                    {
                        if (i == 0) break;

                        SELECT_LIST temp = list[i];
                        list[i] = list[0];
                        list[0] = temp;

                        break;
                    }
                }
            }

            return list;
        }

        bool ObjectNotAllRecurse(Graphics g, ObjectPublicGroupLayer pgroup, SELECT_LIST list, bool first_object)
        {
            ObjectExpand obj;

            for (int i = 0; i < pgroup.GetObjectHap(); i++)
            {
                obj = (ObjectExpand)pgroup.GetPoint(i);

                if (obj == list.obj)
                {
                    if (obj.enumObjectType == EnumObjectType.Layer)
                    {
                        // 레이어 일 경우는 그리지 않는다.
                        int x1 = -1, y1 = -1, x2 = -1, y2 = -1;

                        ((ObjectLayer)obj).GetLayerZone(ref x1, ref y1, ref x2, ref y2);

                        if (x1 == -1 && y1 == -1 && x2 == -1 && y2 == -1)   // 레이어 안에 아무것도 없다. // 그리는 것보다 안그리는 것이 낫다.
                        {
                            x1 = 100;
                            y1 = 100;
                            x2 = 400;
                            y2 = 400;
                        }
                        else
                        {
                            x1 = obj.GetViewPosX(x1);
                            y1 = obj.GetViewPosY(y1);
                            x2 = obj.GetViewPosX(x2);
                            y2 = obj.GetViewPosY(y2);

                            NotRectangle(g, x1, y1, x2, y2, (ObjectType)list.obj, first_object);
                        }
                    }
                    else
                    {
                        int x1 = obj.GetViewPosX(list.x1);
                        int y1 = obj.GetViewPosY(list.y1);
                        int x2 = obj.GetViewPosX(list.x2);
                        int y2 = obj.GetViewPosY(list.y2);

                        if (obj.RotationAngle != 0) { // 회전이 적용 되었다.  편집기상에서 선택된 사각형도 회전이 되면 보기가 좋다.
                            int cx, cy;

                            int w = Math.Abs(x2 - x1) + 1;
                            int h = Math.Abs(y2 - y1) + 1;

                            cx = (x1 > x2) ? x2 + w / 2 : x1 + w / 2;
                            cy = (y1 > y2) ? y2 + h / 2 : y1 + h / 2;

                            //obj.CalcRotationCenter(out cx, out cy);

                            g.TranslateTransform(cx, cy);
                            g.RotateTransform(obj.RotationAngle);

                            NotRectangle(g, x1-cx, y1-cy, x2-cx, y2-cy, (ObjectType)list.obj, first_object);

                            g.ResetTransform();
                        }
                        else {
                            NotRectangle(g, x1, y1, x2, y2, (ObjectType)list.obj, first_object);
                        }
                    }

                    return true;
                }

                if (obj.enumObjectType == EnumObjectType.Group || obj.enumObjectType == EnumObjectType.Layer)
                {
                    if (ObjectNotAllRecurse(g, (ObjectPublicGroupLayer)obj, list, first_object)) return true;
                }
            }

            return false;
        }

		public void ObjectNotAll(Graphics g, WORK_MODULE_STRUCT work)
		{
			if(formFrame.enumMainTool == EnumMainTool.POINT)	return;

            int i;

            for (i = 0; i < work.nSelectCount; i++)
            {
                ObjectNotAllRecurse(g, work.obj.groupRoot, work.selectList[i], (i==0));
            }
		}

		public int GetDisplayPosX(int x)
		{
			x = x*workThis.obj.nOpticRate/100;
			return x-workThis.nScrollHorPos;
		}

		public int GetDisplayPosY(int y)
		{
			y = y*workThis.obj.nOpticRate/100;
			return y-workThis.nScrollVerPos;
		}

		public int GetPictureSize(int xy)
		{
			return 100*xy/workThis.obj.nOpticRate;
		}

		public int GetPicturePosX(int x)
		{
			x += workThis.nScrollHorPos;
			return 100*x/workThis.obj.nOpticRate;
		}

		public int GetPicturePosY(int y)
		{
			y += workThis.nScrollVerPos;
			return 100*y/workThis.obj.nOpticRate;
		}

		public void ObjectNotOne(Graphics g, SELECT_LIST list, bool first_object)
		{
			int x1 = GetDisplayPosX(list.x1);
			int y1 = GetDisplayPosY(list.y1);
			int x2 = GetDisplayPosX(list.x2);
			int y2 = GetDisplayPosY(list.y2);

			NotRectangle(g, x1, y1, x2, y2, (ObjectType)list.obj, first_object);
		}

		// main point 는 곡선은 x[2], y[2] 이외는 x[0],y[0]의 값이다.
		void GetCurveMainPoint(CURVE_STRUCT curve, Point point)
		{
			if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
			{
				point.X = curve.x[2];
				point.Y = curve.y[2];
			}
			else 
			{
				point.X = curve.x[0];
				point.Y = curve.y[0];
			}
		}

		void DrawGuideLine(System.Windows.Forms.PaintEventArgs e, WORK_MODULE_STRUCT work)
		{
            if (AutoLib.ConfigStudio.nGuideLineType == 0) return;

			Graphics g = e.Graphics;

			int x, y;
			int posx, posy;

            if (AutoLib.ConfigStudio.nGuideLineType == 1) 
			{
                Pen pen = new Pen(AutoLib.ConfigStudio.lGuideLineColor, 1);

				pen.DashStyle = DashStyle.Dot;

                for (x = 0; ; x += AutoLib.ConfigStudio.nGuideLineUnitX * AutoLib.ConfigStudio.nGuideLineDisplayX) 
				{
					posx = GetDisplayPosX(x);
					if(posx < e.ClipRectangle.Left)		continue;
					if(posx >= e.ClipRectangle.Right)	break;

					g.DrawLine(pen, posx, 0, posx, ClientRectangle.Bottom);
				}

                for (y = 0; ; y += AutoLib.ConfigStudio.nGuideLineUnitY * AutoLib.ConfigStudio.nGuideLineDisplayY) 
				{
					posy = GetDisplayPosY(y);
					if(posy < e.ClipRectangle.Top)		continue;
					if(posy >= e.ClipRectangle.Bottom)	break;

					g.DrawLine(pen, 0, posy, ClientRectangle.Right, posy);
				}
			}
            else if (AutoLib.ConfigStudio.nGuideLineType == 2) 
			{
                Pen pen = new Pen(AutoLib.ConfigStudio.lGuideLineColor, 1);

				pen.DashStyle = DashStyle.Solid;

                for (x = 0; ; x += AutoLib.ConfigStudio.nGuideLineUnitX * AutoLib.ConfigStudio.nGuideLineDisplayX) 
				{
					posx = GetDisplayPosX(x);
					if(posx < e.ClipRectangle.Left)		continue;
					if(posx >= e.ClipRectangle.Right)	break;

					g.DrawLine(pen, posx, 0, posx, ClientRectangle.Bottom);
				}

                for (y = 0; ; y += AutoLib.ConfigStudio.nGuideLineUnitY * AutoLib.ConfigStudio.nGuideLineDisplayY) 
				{
					posy = GetDisplayPosY(y);
					if(posy < e.ClipRectangle.Top)		continue;
					if(posy >= e.ClipRectangle.Bottom)	break;

					g.DrawLine(pen, 0, posy, ClientRectangle.Right, posy);
				}
			}
            else if (AutoLib.ConfigStudio.nGuideLineType == 3)
			{
                Pen pen = new Pen(AutoLib.ConfigStudio.lGuideLineColor, 1);
                Brush brush = new SolidBrush(AutoLib.ConfigStudio.lGuideLineColor);

				pen.DashStyle = DashStyle.Solid;

                int xgab = AutoLib.ConfigStudio.nGuideLineUnitX * AutoLib.ConfigStudio.nGuideLineDisplayX;
                int ygab = AutoLib.ConfigStudio.nGuideLineUnitY * AutoLib.ConfigStudio.nGuideLineDisplayY;

				for(x = 0; ; x+=xgab)
				{
					posx = GetDisplayPosX(x);
					if(posx < e.ClipRectangle.Left)		    continue;
					if(posx >= e.ClipRectangle.Right)	    break;

					for(y = 0; ; y+=ygab) 
					{
						posy = GetDisplayPosY(y);
						if(posy < e.ClipRectangle.Top)		continue;
						if(posy >= e.ClipRectangle.Bottom)	break;
                        g.FillRectangle(brush, posx, posy, 1, 1);
						//g.DrawLine(pen, posx, posy, posx, posy+1); 이것은 FillRectangle보다 느리고 2점을 찍음
					}
				}
			}
			else {}
		}

		public void ConvertMousePointByGuideLine(WORK_MODULE_STRUCT work, ref Point p, int mx, int my)
		{
			p.X = mx;
			p.Y = my;

			int x = GetPicturePosX(mx);
			int y = GetPicturePosY(my);

			p.X = GetDisplayPosX(x);
			p.Y = GetDisplayPosY(y);

			/*
			if(config.bGuideLineFit == OFF) 
			{
				p.x = mx;
				p.y = my;
				return;
			}

			int x = (int)(work->nScrollHorPos+mx+config.nGuideLineUnitX/2.0);
			x = (x/config.nGuideLineUnitX)*config.nGuideLineUnitX;
			p.x = x-work->nScrollHorPos;

			int y = (int)(work->nScrollVerPos+my+config.nGuideLineUnitY/2.0);
			y = (y/config.nGuideLineUnitY)*config.nGuideLineUnitY;
			p.y = y-work->nScrollVerPos;
			*/
		}


		void SetPicturePosition(int x, int y)
		{
			Rectangle rect;
			int rate = workThis.obj.GetOpticRate();

			rect = this.ClientRectangle;

			x = x*rate/100-(rect.Right/2);
			y = y*rate/100-(rect.Bottom/2);

			if(x < 0)	x = 0;
			if(y < 0)	y = 0;

			Point point = new Point(x, y);
			this.AutoScrollPosition = point;
		}

		// 선택된 것이 있으면 선택된 오브젝트를 중앙에 놓고 그렇지 않으면 화면중앙을 중앙으로 놓는다.
		public void OpticGo(int rate)
		{
			int pic_x, pic_y;
			// 선택이 되어 있다.
			if(this.workThis.nSelectCount > 0) 
			{
				ObjectGroup group = ClassStudioEdit.SelectedObjectToGroup(this, false);
				int x1=0, y1=0, x2=0, y2=0;
				group.GetZone(ref x1, ref y1, ref x2, ref y2);
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);
				pic_x = x1+(x2-x1)/2;
				pic_y = y1+(y2-y1)/2;;
			}
			else 
			{
				pic_x = this.GetPicturePosX(this.ClientRectangle.Width/2);
				pic_y = this.GetPicturePosY(this.ClientRectangle.Height/2);
			}

			workThis.obj.SetOpticRate(rate);
			workThis.obj.nOpticRate = rate;

			SetTitle();
			ScrollUpdate();
			Invalidate();
			
			SetPicturePosition(pic_x, pic_y);
		}

        private void FormGraphic_MouseWheel(object sender, MouseEventArgs e)   //20250212 PSU 추가
        {
            //  Control + 휠: 줌 인/아웃
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                int rate = workThis.obj.nOpticRate;

                if (e.Delta > 0)
                {
                    if (rate >= 1000) return;
                    rate += 10;
                    OpticGo(rate);
                }
                else
                {
                    if (rate <= 10) return;
                    rate -= 10;
                    OpticGo(rate);
                }
                return;
            }

            this.Invalidate(); //휠로 수직스크롤을 위로 이동할 때 모듈크기박스가 남아있어서 추가
        }

		private void contextMenuPointMove_Popup(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveAdd_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveDelete_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveToLine_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveToCurve_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveCusp_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveSmooth_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveSymmetrical_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveAutoClose_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveJoin_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemPointMoveBreakApart_Click(object sender, System.EventArgs e)
		{
			
		}

		public void SelectListSortSmallToBig(ref SELECT_LIST[] table, int hap)
		{
			int i, j;
			int small;
			SELECT_LIST temp;

			for(i = 0; i < hap; i++) 
			{
                small = i;
				for(j = i+1; j < hap; j++) 
				{
                    if (ClassStudioEditUndo.ComparePos(table[j].opos, table[small].opos) < 0) small = j;
				}

                if (i != small) 
				{
					temp = table[i];
                    table[i] = table[small];
                    table[small] = temp;
				}
			}
		}

		private void menuItemConfigGuideLine_Click(object sender, System.EventArgs e)
		{
			FormConfigGuideLine form = new FormConfigGuideLine();
            form.StartPosition = FormStartPosition.CenterParent;

			if(form.ShowDialog(this) == DialogResult.OK)	this.Invalidate();
		}

		public void ConvertX(WORK_MODULE_STRUCT work, int ox, ref int gabx)
		{
            if (AutoLib.ConfigStudio.bGuideLineFit == false) 
			{
				gabx = GetPictureSize(gabx);
				return;
			}

			int psize = GetPictureSize(gabx);

            int x = (int)(ox + psize + AutoLib.ConfigStudio.nGuideLineUnitX / 2.0);

            x = (x / AutoLib.ConfigStudio.nGuideLineUnitX) * AutoLib.ConfigStudio.nGuideLineUnitX;

			gabx = x-ox;
		}

		public void ConvertY(WORK_MODULE_STRUCT work, int oy, ref int gaby)
		{
            if (AutoLib.ConfigStudio.bGuideLineFit == false)	
			{
				gaby = GetPictureSize(gaby);
				return;
			}

			int psize = GetPictureSize(gaby);

            int y = (int)(oy + psize + AutoLib.ConfigStudio.nGuideLineUnitY / 2.0);

            y = (y / AutoLib.ConfigStudio.nGuideLineUnitY) * AutoLib.ConfigStudio.nGuideLineUnitY;

			gaby = y-oy;
		}

		private void menuItemEditCopy_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditCopy(this);
		}

		private void menuItemEditPaste_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditPaste(this);
		}

		private void menuItemEditDelete_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditDelete(this);
		}

		private void menuItemEditSelectAll_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditSelectAll(this);
		}

		private void menuItemEditMoveToFront_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMovePost(this);
		}

		private void menuItemEditMoveToBack_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMoveBack(this);
		}

		private void menuItemEditMoveToPrevFront_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMovePrevFront(this);
		}

		private void menuItemEditMoveToNextBack_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditMoveNextBack(this);
		}

		private void menuItemEditAlignToLeft_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideLeft(this);
		}

		private void menuItemEditAlignToRight_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideRight(this);
		}

		private void menuItemEditAlignToTop_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideTop(this);
		}

		private void menuItemEditAlignToBottom_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditArrangeSideBottom(this);
		}

		private void menuItemSpaceHorizontal_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditSpaceHorz(this);
		}

		private void menuItemSpaceVertical_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditSpaceVert(this);
		}

		private void menuItemEditGroup_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditGroup(this);
		}

		private void menuItemEditUngroup_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditUnGroup(this);
		}

		private void menuItemEdit_Click(object sender, System.EventArgs e)
		{
		
		}

        /*
		private void menuItemContextMoveToCurveObject_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemContextMoveCombine_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemContextMoveGroup_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemContextMoveUngroup_Click(object sender, System.EventArgs e)
		{
			
		}

		private void menuItemContextMoveObjectProperty_Click(object sender, System.EventArgs e)
		{
			
		}

		private void contextMenuMove_Popup(object sender, System.EventArgs e)
		{
			
		}*/

		private void menuItemEditUndo_Click(object sender, System.EventArgs e)
		{
			ClassStudioEditUndo.menuItemEditUndo_Click(this);
		}

		private void menuItemEditRedo_Click(object sender, System.EventArgs e)
		{
			ClassStudioEditUndo.menuItemEditRedo_Click(this);
		}

		private void menuItemEtcBackgroundBitmap_Click(object sender, System.EventArgs e)
		{
			FormConfigBackgroundBitmap dialog = new FormConfigBackgroundBitmap(this);

			dialog.ShowDialog(this);
		}

		public void menuItemEtcBackgroundColor_Click()
		{
            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                if (Tools.IsLangKorean())
                    ClassStudioEditUndo.UndoSave_Root(this, "배경 색상");
                else
                    ClassStudioEditUndo.UndoSave_Root(this, "Background Color");
            }

			WORK_MODULE_STRUCT work = workThis;
			FormColorDialog dialog = new FormColorDialog();

			dialog.SetSelectedColor(work.obj.GetBackGroundColor(), true);
            // dialog.bUseAlpha = false;

			if(Tools.IsLangKorean())	dialog.Text = "배경색상";
			else if(Tools.IsLangJapanese())	dialog.Text = "背景色";
			else if(Tools.IsLangChinese())	dialog.Text = "背景颜色";
            else if (Tools.IsLangVietnamese()) dialog.Text = "Màu nền";
			else						dialog.Text = "Background Color";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				work.obj.SetBackGroundColor(dialog.GetSelectedColor());
				SetChangeFlag();
				Invalidate();
			}
		}

		private void menuItemEtcModuleProperty_Click(object sender, System.EventArgs e)
		{
			FormConfigModuleProperty dialog = new FormConfigModuleProperty(this);
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
		}

		private void menuItemInsertObjectDatabase_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDatabase(this);
		}

		private void menuItemInsertObjectButtonModule3D_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonModule3D(this);
		}

		private void menuItemInsertObjectButtonModuleHide_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonModuleHide(this);
		}

		private void menuItemInsertObjectButtonProgram_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonProgram(this);
		}

		private void menuItemInsertObjectButtonDigitalOut_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertButtonDigitalOut(this);
		}

		private void menuItemInsertObjectDigitalAnimation_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalAnimation(this);
		}

		private void menuItemInsertObjectDigitalCircle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalCircle(this);
		}

		private void menuItemInsertObjectDigitalRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalRectangle(this);
		}

		private void menuItemInsertObjectDigitalString_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDigitalString(this);
		}

		private void menuItemInsertObjectAnalogRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogRectangle(this);
		}

		private void menuItemInsertObjectAnalogString_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogString(this);
		}

		private void menuItemInsertObjectAnalogMeter_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogMeter(this);
		}

		private void menuItemInsertObjectAnalogStatus_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogStatus(this);
		}

		private void menuItemInsertObjectAnalogRotate_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnalogRotate(this);
		}

		private void menuItemInsertObjectStringTag_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertStringString(this);
		}

		private void menuItemInsertObjectGraphicModule_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertModule(this);
		}

		private void menuItemInsertObjectAlarmWindow_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertWindowAlarm(this);
		}

		private void menuItemInsertObjectMultiGraph_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertMultiGraph(this);
		}

		private void menuItemInsertObjectMultiTrend_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertMultiTrend(this);
		}

        //25-02-24 Chart 컨트롤 기반 오브젝트 메뉴 핸들러
        private void menuItemInsertCustomChart_Click(object sender, System.EventArgs e)
        {
            ClassEditInsert.EditInsertCustomChart(this);
        }

        private void menuItemInsertBarcodeDisplay_Click(object sender, System.EventArgs e)
        {
            ClassEditInsert.EditInsertBarcodeDisplay(this);
        }

        private void menuItemInsertBarcodeScanner_Click(object sender, System.EventArgs e)
        {
            ClassEditInsert.EditInsertBarcodeScanner(this);
        }

		private void menuItemInsertObjectDemandWindow_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDemandWindow(this);
		}

        private void menuItemInsertObjectMilliDataWindow_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertMilliData(this);
		}

		private void menuItemInsertObjectXyGraph_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertXYGraph(this);
		}

		private void menuItemInsertObjectControlListBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlListBox(this);
		}

		private void menuItemInsertObjectControlComboBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlComboBox(this);
		}

		private void menuItemInsertObjectControlEditBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlEditBox(this);
		}

		private void menuItemInsertObjectControlRadioButton_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlRadioButton(this);
		}

		private void menuItemInsertObjectControlCheckBox_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertControlCheckBox(this);
		}

		private void menuItemInsertObjectRealTimeTestGraph_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertRealTimeTestGraph(this);
		}

		private void menuItemInsertObjectDatabaseTrend_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDatabaseTrend(this);
		}

		private void menuItemInsertObjectBitmap_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertBitmap(this);
		}

		private void menuItemInsertObjectAnimation_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertAnimation(this);
		}

		private void menuItemInsertObjectSingleText_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertSingleText(this);
		}

		private void menuItemInsertObjectRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertRectangle(this);
		}

		private void menuItemInsertObjectCircle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertCircle(this);
		}

		private void menuItemInsertObjectLine_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertLine(this);
		}

		private void menuItemInsertObjectText_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertText(this);
		}

		private void menuItemInsertObjectWatch_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertClock(this);
		}

		private void menuItemInsertObjectDate_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertDate(this);
		}

		private void menuItemInsertFromLibrary_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertFromLibrary(this);
		}

		/*
		string GetOnlyGraphicPath(string path) 
		{
			string dir = TotalConfig.sDirWorkProject+"\\graphic";
			string filename = path.Substring(dir.Length+1);
			return filename;
		}
		*/

		void CheckSameDirectoryAndCopy(string source, string target)
		{
			string dir_src = Path.GetDirectoryName(source);
			string dir_tar = Path.GetDirectoryName(target);

			if(String.Compare(dir_src, dir_tar, true) == 0) 
			{
				return;
			}

			ArrayList family_file = new ArrayList();
			workThis.obj.GetFamilyFile(family_file);

			FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

			for(int l = 0; l < family_file.Count; l++) 
			{
				family = (FAMILY_FILE_STRUCT)family_file[l];
				ClassStudioEditCopyFile.CopyFileToGraphicDirectoryFamily(dir_tar, dir_src, family);
			}

			workThis.obj.ChangeFamilyFile(family_file);
		}

        /* WCF를 이용한 로컬 웹서버도 웹 복사를 이용해서 하는 것이 좋을 듯 해서 이부분은 당분간 보류한다.
        // pcx나 이전의 이미지 파일을 png로 바꾸어준다.
        public void CheckConvertForAutobase11_OldImageFormat()
        {
            if (!NextVersion.IsAutobase11) return;

            ArrayList array = new ArrayList();
            workThis.obj.GetFamilyFile(array);

            FAMILY_FILE_STRUCT family;
            string ext;
            int count = 0;
            string ext_name = "";

            for (int i = 0; i < array.Count; i++)
            {
                family = (FAMILY_FILE_STRUCT)array[i];
                ext = Path.GetExtension(family.filename);

                if (String.Compare(ext, ".pcx", true) == 0)
                {
                    if (count == 0)
                    {
                        ext_name = ext;
                    }
                    count++;
                }
            }

            if (count == 0) return;

            string msg;

            msg = String.Format("This module file contains *{0} image files.(count={1})\nConvert these image files to PNG format?", ext_name, count);
            if (MessageBox.Show(msg, "Image File Convert", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            FAMILY_FILE_STRUCT temp = new FAMILY_FILE_STRUCT();
            string target_dir = Path.GetDirectoryName(this.workThis.obj.objCommonProperty.sModuleName);

            for (int i = 0; i < array.Count; i++)
            {
                family = (FAMILY_FILE_STRUCT)array[i];
                ext = Path.GetExtension(family.filename);

                if (String.Compare(ext, ".pcx", true) == 0)
                {
                    string source_path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(this.workThis.obj.objCommonProperty, family.filename);

                    PictureToBitmap load = new PictureToBitmap();

                    Bitmap bitmap = load.Load(source_path);
                    MemoryStream stream = new MemoryStream();
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    temp.filename = Path.ChangeExtension(family.filename, ".png");
                    ClassStudioEditCopyFile.CopyFileToGraphicDirectoryFamilyFromStream(target_dir, stream, temp);
                    family.change = temp.change;
                }
            }

            workThis.obj.ChangeFamilyFile(array);
        }

        void CheckConvertForAutobase11_AnimationFile()
        {
            workThis.obj.groupRoot.SaveAnimationFileToVersion3();
        }

        void CheckConvertForAutobase11()
        {
            CheckConvertForAutobase11_OldImageFormat();
            CheckConvertForAutobase11_AnimationFile();
        }*/

        public bool FileSave(string filename)
		{
			// 먼저 폴더가 존재하는지 검사한다.
			string dir = Path.GetDirectoryName(filename);
			if(!Directory.Exists(dir)) 
			{
				Directory.CreateDirectory(dir);
			}

			BackUp.BackUpFile(filename);

			CheckSameDirectoryAndCopy(workThis.filename, filename);

            /*
            if(NextVersion.IsAutobase11)
                CheckConvertForAutobase11();*/

			if(!workThis.obj.ObjectSave(filename)) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("파일을 저장할 수 없습니다.", filename);
				else
					MessageBox.Show("Can't save the file.", filename);

				return false;
			}
			else 
			{
				workThis.bChangeFlag = false;
				workThis.filename = filename;
				workThis.obj.objCommonProperty.sModuleName = filename;
				SetTitle(); // title을 다시 그린다.
			}

            //SaveRuntimeFile(filename);

			return true;
		}

		public bool FileSave()
		{
			string ext = Path.GetExtension(workThis.filename);
			if(String.Compare(ext, ".mod", true) == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("파일을 MODX 형태로 저장해야 합니다.", "파일변환");
				else
					MessageBox.Show("This file will be save to MODX format.", "File conversion");

				return SaveAs(Path.ChangeExtension(workThis.filename, ".modx"));
			}

			string name = Path.GetFileNameWithoutExtension(workThis.filename);
			if(String.Compare(name, 0, "noname", 0, 6, true) == 0) 
			{
				return SaveAs(Path.ChangeExtension(workThis.filename, ".modx"));
			}
						
			string path = workThis.filename;

			return FileSave(path);
		}

		private void menuItemFileSave_Click(object sender, System.EventArgs e)
		{
			FileSave();
		}

		public bool SaveAs(string init_file)
		{
			SaveFileDialog dialog = new SaveFileDialog();

			dialog.Filter = "Module files (*.modx)|*.modx";
			string init = TotalConfig.sDirWorkProject+"\\Graphic";

			if(!Directory.Exists(init)) 
			{
                try
                {
                    Directory.CreateDirectory(init);
                }
                catch
                {
                    return false;
                }
			}
			dialog.InitialDirectory = init;

			if(init_file != null) {
                if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
                {
                    string only_file = Path.GetFileName(init_file);
                    dialog.FileName = only_file;
                }
                else
                {
                    dialog.FileName = init_file;
                }
			}

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				bool retn = FileSave(dialog.FileName);
				if(retn) 
				{
					SharedStudio.formSolution.ReLoad();
				}
				return retn;
			}

			return false;
		}

        
		/// <summary>
		/// CTLX 파일이 있으면 그냥 사용하고 그렇지 않으면 같은 이름의 CTL파일을 사용한다.
		/// </summary>
		/// <param name="path"></param>
		public static string CheckAndCopyControl(string path)
		{
			if(File.Exists(path))	return path;	 // 이미 파일이 존재하므로 변경할 필요가 없다.
			
			//string ext = Path.GetExtension(path);
			string pre_file = String.Format("{0}\\{1}.ctl", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

			if(File.Exists(pre_file)) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("이전 버전의 CTL 파일이 존재합니다.\n편집 후 확인을 누르면 자동으로 CTLX파일로 저장됩니다.", "이전버전의 파일 존재");
				else
					MessageBox.Show("Old version's CTL file is already exist.\nIf press OK after editing then save to CTLX format.", "Old version already exist");

				return pre_file;
			}

			return path;
		}

        /*
		string GetControlName(string control_sub, string modname)
		{
			string graphic_dir = TotalConfig.sDirWorkProject+"\\graphic";
			string mod_path = modname.Substring(graphic_dir.Length+1);

			string ext = Path.GetExtension(modname);
			string name = mod_path.Substring(0, mod_path.Length-ext.Length);

			string path = String.Format("{0}\\Control\\{1}\\{2}.ctlx", TotalConfig.sDirWorkProject, control_sub, name);
			path = CheckAndCopyControl(path);

			return path;
		}*/

        bool EditModuleScript(ref ScriptClass script, string default_description)
        {
            FormScriptEditor dialog = new FormScriptEditor();

            dialog.EnableScanTimeUse(false);
            if (script != null)
            {
                dialog.SetScript(script);
            }
            else
            {
                dialog.SetDescription(default_description);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                script = dialog.GetScript();
                return true;
            }

            return false;
        }

        public void menuItemScriptOnModuleOpened_Click()
        {
            string default_description;

            if (Tools.IsLangKorean())
                default_description = "모듈 열릴 때 한번 실행할 스크립트";
            else if (Tools.IsLangChinese())
                default_description = "脚本:在打开模块时";
            else
                default_description = "Script when module file opened";

            if (EditModuleScript(ref workThis.obj.scriptModuleStart, default_description))
                SetChangeFlag();
        }

		public void menuItemScriptOnModuleRunning_Click()
		{
            string default_description;

			if(Tools.IsLangKorean()) 
				default_description = "모듈 열려있을 때 계속 실행할 스크립트";
			else if(Tools.IsLangChinese()) 
				default_description = "脚本:在运行模块时";
			else
				default_description = "Script when module file alive";

            if (EditModuleScript(ref workThis.obj.scriptModuleAlways, default_description))
                SetChangeFlag();
		}

		public void menuItemScriptOnModuleClosed_Click()
        {
            string default_description;

			if(Tools.IsLangKorean()) 
				default_description = "모듈 닫힐 때 한번 실행할 스크립트";
			else if(Tools.IsLangChinese()) 
				default_description = "脚本:在结束模块时";
			else
				default_description = "Script when module file closed";

            if (EditModuleScript(ref workThis.obj.scriptModuleEnd, default_description))
                SetChangeFlag();
		}

		public void menuItemScriptOnModuleActivated_Click()
		{
            string default_description;

			if(Tools.IsLangKorean()) 
				default_description = "모듈 활성화 시 한번 실행할 스크립트";
			else if(Tools.IsLangChinese()) 
				default_description = "脚本:模块在活动时";
			else
				default_description = "Script when module actived";

            if (EditModuleScript(ref workThis.obj.scriptModuleActive, default_description))
                SetChangeFlag();
		}

		public void menuItemScriptOnModuleDeactivated_Click()
		{
            string default_description;

			if(Tools.IsLangKorean()) 
				default_description = "모듈 비활성화 시 한번 실행할 스크립트";
			else if(Tools.IsLangChinese()) 
				default_description = "脚本:模块在非活动时";
			else
				default_description = "Script when module file deactivated";

            if (EditModuleScript(ref workThis.obj.scriptModuleDeactive, default_description))
                SetChangeFlag();
		}

        /*
		private void menuItemFlipHorizontal_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditFlipHorizontal(this);
		}

		private void menuItemFlipVertical_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.OnEditFlipVertical(this);
		}

        private void menuItemEditRotateRight_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateRight(this);
        }

        private void menuItemEditRotateLeft_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateLeft(this);
        }*/

		public void FormEditGraphic_Closing(System.ComponentModel.CancelEventArgs e)
		{
			if(this.workThis.bChangeFlag) 
			{
				DialogResult result;

				if(Tools.IsLangKorean()) 
					result = MessageBox.Show(Path.GetFileName(this.workThis.filename)+" 파일을 저장하지 않았습니다.\n파일을 저장할까요?", this.workThis.filename, MessageBoxButtons.YesNoCancel);
				else if(Tools.IsLangChinese()) 
					result = MessageBox.Show(Path.GetFileName(this.workThis.filename)+" 文件还没保存。\n想保存文件吗？", this.workThis.filename, MessageBoxButtons.YesNoCancel);
				else 
					result = MessageBox.Show(Path.GetFileName(this.workThis.filename)+" File not saved.\nSave to file?", this.workThis.filename, MessageBoxButtons.YesNoCancel);

				if(result == DialogResult.No)		return;
				if(result == DialogResult.Cancel) 
				{
					e.Cancel = true;
					return;
				}
				if(!FileSave())	
				{
					e.Cancel = true;
					return;
				}

			}
		}

		void MoveObject(int gabx, int gaby)
		{
			WORK_MODULE_STRUCT work = workThis;

			if(workThis.nSelectCount == 0)	return;	// 선택된 object가 없다.

			ClassStudioEditUndo.UndoSave_Selected(this, "Move object by arrow key");
			ClassEditObjectMove.SelectListGetZone(workThis);

			for(int i = 0; i < work.nSelectCount; i++) 
			{
				work.selectList[i].x1 += gabx;
				work.selectList[i].y1 += gaby;
				work.selectList[i].x2 += gabx;
				work.selectList[i].y2 += gaby;
			}
			
			ClassEditObjectMove.UpdateSelectedObject(this, workThis);
			this.DrawSelectedObjectSize(workThis);
            ClassEditProperty.ObjectPosSizeChanged(this);
			
			Invalidate();
		}

		void ProcessArrowKey(Keys key)
		{
			if(key == Keys.Left)	
			{
				if(Control.ModifierKeys == Keys.Control) 
					ClassStudioEdit.EditArrangeSideLeft(this);
				else
					MoveObject(-1, 0);
			}
			else if(key == Keys.Right)
			{
				if(Control.ModifierKeys == Keys.Control) 
					ClassStudioEdit.EditArrangeSideRight(this);
				else
					MoveObject(+1, 0);
			}
			else if(key == Keys.Up) 
			{
				if(Control.ModifierKeys == Keys.Control) 
					ClassStudioEdit.EditArrangeSideTop(this);
				else
					MoveObject(0, -1);
			}
			else if(key == Keys.Down) 
			{
				if(Control.ModifierKeys == Keys.Control) 
					ClassStudioEdit.EditArrangeSideBottom(this);
				else
					MoveObject(0, +1);
			}
			else 
			{

			}
		}
		
		/// <summary>
		/// 실제 방향키는 이벤트가 들어오지는 않고 Ctrl을 누른 상태에서만 들어온다.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 
		private void FormEditGraphic_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
            if (e.KeyCode == Keys.NumPad0)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertButtonModule3D(this);
                }
                else if (e.Alt)
                {
                    OpticGo(50);
                }
            }
            else if (e.KeyCode == Keys.NumPad1)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertButtonModuleHide(this);
                }
                else if (e.Alt)
                {
                    OpticGo(100);
                }
            }
            else if (e.KeyCode == Keys.NumPad2)
            {
                if (e.Control)
                {
                    ClassStudioEdit.EditLock(this);
                }
                else if (e.Alt)
                {
                    OpticGo(200);
                }
            }
            else if (e.KeyCode == Keys.NumPad3)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertButtonProgram(this);
                }
                else if (e.Alt)
                {
                    OpticGo(300);
                }
            }
            else if (e.KeyCode == Keys.NumPad4)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertButtonDigitalOut(this);
                }
                else if (e.Alt)
                {
                    OpticGo(400);
                }
            }
            else if (e.KeyCode == Keys.NumPad5)
            {
                if (e.Control)
                {
                    ClassStudioEdit.EditUnLock(this);
                }
                else if (e.Alt)
                {
                    OpticGo(500);
                }
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertDigitalAnimation(this);
                }
                else if (e.Alt)
                {
                    OpticGo(600);
                }
            }
            else if (e.KeyCode == Keys.NumPad7)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertDigitalCircle(this);
                }
                else if (e.Alt)
                {
                    OpticGo(700);
                }
            }
            else if (e.KeyCode == Keys.NumPad8)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertDigitalRectangle(this);
                }
                else if (e.Alt)
                {
                    OpticGo(800);
                }
            }
            else if (e.KeyCode == Keys.NumPad9)
            {
                if (e.Control)
                {
                    ClassEditInsert.EditInsertDigitalString(this);
                }
                else if (e.Alt)
                {
                    OpticGo(900);
                }
            }

            if (e.Alt || e.Control || e.Shift) return;  // 제어키가 눌러져 있으면 실행하지 않는다. 2011-12-9  어떤 경우 Control+C키를 누르면 Curve Tool이 선택되는 경우가 있다.

            if (e.KeyCode == Keys.Escape)
            {
                if (ClassEditProperty.propertySheet != null)
                {
                    ClassEditProperty.propertySheet.Close();
                }
            }
            else if (e.KeyCode == Keys.V)
            {
                ChangeToolByHotKey(EnumMainTool.ARROW);
            }
            else if (e.KeyCode == Keys.L)
            {
                ChangeToolByHotKey(EnumMainTool.LINE);
            }
            else if (e.KeyCode == Keys.R)
            {
                ChangeToolByHotKey(EnumMainTool.RECT);
            }
            else if (e.KeyCode == Keys.M)
            {
                ChangeToolByHotKey(EnumMainTool.RECT_FILL);
            }
            else if (e.KeyCode == Keys.S)
            {
                ChangeToolByHotKey(EnumMainTool.CIRCLE);
            }
            else if (e.KeyCode == Keys.E)
            {
                ChangeToolByHotKey(EnumMainTool.CIRCLE_FILL);
            }
            else if (e.KeyCode == Keys.O)
            {
                ChangeToolByHotKey(EnumMainTool.POLY);
            }
            else if (e.KeyCode == Keys.Y)
            {
                ChangeToolByHotKey(EnumMainTool.POLY_FILL);
            }
            else if (e.KeyCode == Keys.T)
            {
                ChangeToolByHotKey(EnumMainTool.TEXT);
            }
            else if (e.KeyCode == Keys.D)
            {
                ChangeToolByHotKey(EnumMainTool.ROUND_RECTANGLE);
            }
            else if (e.KeyCode == Keys.C)
            {
                ChangeToolByHotKey(EnumMainTool.CURVE);
            }
            else if (e.KeyCode == Keys.N)
            {
                ChangeToolByHotKey(EnumMainTool.POINT);
            }
            else if (e.KeyCode == Keys.I)
            {
                ChangeToolByHotKey(EnumMainTool.SPUIT);
            }

                else if (e.KeyCode == Keys.Space) //panning mode 동작용 space 추가 24-06-27 hsejong
            {
                if (formFrame.enumMainTool == EnumMainTool.ARROW)
                {
                    cViewMode = EnumViewMode.PANNING;
                    this.Cursor = cursorPanning; //20250212 PSU 추가, 바로 커서 변경.
                }
                //ChangeToolByHotKey(EnumMainTool.SPUIT);
                // 이동모드 일 때만 동작하도록
                
            }

            

		}

        void ChangeToolByHotKey(EnumMainTool tool)
        {
            if (formFrame.enumMainTool == EnumMainTool.TEXT && ((ClassEditObjectText)GetToolClass()).textBox != null)
            {

            }
            else
            {
                formFrame.Tool_Click(tool);
            }
        }

		public void menuItemViewObjectNumber_Click()
		{
			bDisplayObjectNumber = !bDisplayObjectNumber;
			this.Invalidate();
		}

		private void menuItemInsertObjectRoundRectangle_Click(object sender, System.EventArgs e)
		{
			ClassEditInsert.EditInsertRoundRectangle(this);
		}

		private void menuItemEditRegisterToLibrary_Click(object sender, System.EventArgs e)
		{
			ClassStudioEdit.EditRegisterToLibrary(this);
		}

        //private void FormEditGraphic_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        //{
        //	string[] format = e.Data.GetFormats();

        //	if(format.Length != 1)	return;

        //          if (format[0] == "GraphicModule.ObjectGroup") 
        //		e.Effect = DragDropEffects.Copy;
        //	else
        //		e.Effect = DragDropEffects.None;
        //}

        //private void FormEditGraphic_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        //{
        //	string[] format = e.Data.GetFormats();

        //	if(format.Length != 1)	return;

        //	if (format[0] != "GraphicModule.ObjectGroup")	return;

        //          if (!FormLibraryGroupPreview.thisDragAndDrop.CheckCash()) return;

        //          object obj = FormLibraryGroupPreview.thisDragAndDrop.InsertSelection();

        //          Point p = new Point(e.X, e.Y);
        //          p = this.PointToClient(p);		// 화면좌표를 폼좌표로 바꾼다.

        //          ClassEditInsert.EditInsertFrom_LibraryDragAndDrop(this, (ObjectGroup)obj, p.X, p.Y);
        //}


        private void FormEditGraphic_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                string[] format = e.Data.GetFormats();

                // 기존 GraphicModule.ObjectGroup 처리
                if (format.Length == 1 && format[0] == "GraphicModule.ObjectGroup")
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }

                // 파일 드래그 앤 드롭 처리 추가
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    // 이미지, SVG, Animation 파일인지 확인
                    if (files.Length > 0)
                    {
                        string firstFile = files[0];
                        if (IsValidBitmapFile(firstFile) || IsValidSVGFile(firstFile) || IsValidAnimationFile(firstFile))
                        {
                            e.Effect = DragDropEffects.Copy;
                            return;
                        }
                    }
                }

                e.Effect = DragDropEffects.None;
            }
            catch (System.Exception ex)
            {
                e.Effect = DragDropEffects.None;
                string errorMsg = Tools.IsLangKorean() ?
                                 "드래그 진입 처리 중 오류가 발생했습니다: " + ex.Message :
                                 "An error occurred during drag enter processing: " + ex.Message;
                string title = Tools.IsLangKorean() ? "오류" : "Error";

                System.Windows.Forms.MessageBox.Show(errorMsg, title,
                           System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void FormEditGraphic_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                // 기존 GraphicModule.ObjectGroup 처리
                string[] format = e.Data.GetFormats();
                if (format.Length == 1 && format[0] == "GraphicModule.ObjectGroup")
                {
                    if (!FormLibraryGroupPreview.thisDragAndDrop.CheckCash()) return;
                    object obj = FormLibraryGroupPreview.thisDragAndDrop.InsertSelection();
                    Point p = new Point(e.X, e.Y);
                    p = this.PointToClient(p);		// 화면좌표를 폼좌표로 바꾼다.
                    ClassEditInsert.EditInsertFrom_LibraryDragAndDrop(this, (ObjectGroup)obj, p.X, p.Y);
                    return;
                }

                // 파일 드래그 앤 드롭 처리 추가
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    Point dropPoint = this.PointToClient(new Point(e.X, e.Y));

                    // 여러 파일이 드롭된 경우 처리
                    int offset = 0;
                    foreach (string file in files)
                    {
                        try
                        {
                            Point currentPos = new Point(dropPoint.X + offset, dropPoint.Y + offset);

                            if (IsValidBitmapFile(file))
                            {
                                ClassEditInsert.EditInsertBitmapByDrag(this, file, currentPos);
                                offset += 20; // 겹치지 않게 약간씩 이동
                            }
                            else if (IsValidSVGFile(file))
                            {
                                ClassEditInsert.EditInsertSVGByDrag(this, file, currentPos);
                                offset += 20;
                            }
                            else if (IsValidAnimationFile(file))
                            {
                                ClassEditInsert.EditInsertAnimationByDrag(this, file, currentPos);
                                offset += 20;
                            }
                        }
                        catch (System.Exception fileEx)
                        {
                            string errorMsg = Tools.IsLangKorean() ?
                                             "파일 '" + System.IO.Path.GetFileName(file) + "' 처리 중 오류가 발생했습니다: " + fileEx.Message :
                                             "An error occurred while processing file '" + System.IO.Path.GetFileName(file) + "': " + fileEx.Message;
                            string title = Tools.IsLangKorean() ? "파일 처리 오류" : "File Processing Error";

                            System.Windows.Forms.MessageBox.Show(errorMsg, title,
                                       System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            // 다른 파일 처리는 계속 진행
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string errorMsg = Tools.IsLangKorean() ?
                                 "드래그 드롭 처리 중 오류가 발생했습니다: " + ex.Message :
                                 "An error occurred during drag and drop processing: " + ex.Message;
                string title = Tools.IsLangKorean() ? "오류" : "Error";

                System.Windows.Forms.MessageBox.Show(errorMsg, title,
                           System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }


        // 이미지 파일 유효성 검사 메서드 추가
        private bool IsValidBitmapFile(string filename)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(filename).ToLower();

                // PropertyPageObjectBitmap.sFilterBitmap과 동일한 확장자 체크
                string[] validExtensions = new string[]
                {
                    ".png", ".bmp", ".pcx", ".gif",
                    ".tif", ".tiff", ".jpg", ".jpeg", ".jpe",
                    ".wmf", ".emf"
                };

                foreach (string ext in validExtensions)
                {
                    if (extension == ext)
                        return true;
                }
                return false;
            }
            catch (System.Exception)
            {
                return false; // 파일명 처리에서 오류가 발생하면 유효하지 않은 파일로 처리
            }
        }


        // SVG 파일 유효성 검사 메서드 추가
        private bool IsValidSVGFile(string filename)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(filename).ToLower();
                return extension == ".svg";
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        // Animation 파일 유효성 검사 메서드 추가
        private bool IsValidAnimationFile(string filename)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(filename).ToLower();
                return extension == ".ani";
            }
            catch (System.Exception)
            {
                return false;
            }
        }





        public void InsertLibrary_FromInsertClick(ObjectGroup groupCopy)
		{
			ClassEditInsert.EditInsertFrom_LibraryInsertClick(this, groupCopy);
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

		private void menuItemEditObjectProperties_Click(object sender, System.EventArgs e)
		{
			ClassEditProperty.OnUserPropertyClick(this);
		}

		private void menuItemEditObjectProperty_Click(object sender, System.EventArgs e)
		{
			
		}

		private void FormEditGraphic_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			
		}

		private void menuItemFileSaveAsBitmap_Click(object sender, System.EventArgs e)
		{
			FormFileSaveAsBitmap dialog = new FormFileSaveAsBitmap(this, this.workThis.obj);

			dialog.ShowDialog(this);
		}

		public void SelectNodeClear()
		{
			this.editPointMove.SelectNodeClear();
		}

        public void SelectNodePrepare()
        {
            this.editPointMove.SelectedNodePrepare(this);
        }

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(msg.Msg == 0x100)	// key Down, 방향키는 ProcessCmdKey 에서만 들어온다
			{
				if(msg.WParam.ToInt32() == 0x25) ProcessArrowKey(Keys.Left);
				else if(msg.WParam.ToInt32() == 0x26) ProcessArrowKey(Keys.Up);
				else if(msg.WParam.ToInt32() == 0x27) ProcessArrowKey(Keys.Right);
				else if(msg.WParam.ToInt32() == 0x28) ProcessArrowKey(Keys.Down);

                else if (keyData == Keys.NumPad0)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0)
                    {
                        ClassEditInsert.EditInsertButtonModule3D(this);
                    }
                }
                else if (keyData == Keys.NumPad1)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertButtonModuleHide(this);
                    }
                }
                else if (keyData == Keys.NumPad2)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {

                    }
                }
                else if (keyData == Keys.NumPad3)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertButtonProgram(this);
                    }
                }
                else if (keyData == Keys.NumPad4)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertButtonDigitalOut(this);
                    }
                }
                else if (keyData == Keys.NumPad5)
                {
                    
                }
                else if (keyData == Keys.NumPad6)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertDigitalAnimation(this);
                    }
                }
                else if (keyData == Keys.NumPad7)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertDigitalCircle(this);
                    }
                }
                else if (keyData == Keys.NumPad8)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertDigitalRectangle(this);
                    }
                }
                else if (keyData == Keys.NumPad9)
                {
                    if ((Control.ModifierKeys & Keys.Control) > 0 && (Control.ModifierKeys & Keys.Shift) > 0)
                    {
                        ClassEditInsert.EditInsertDigitalString(this);
                    }
                }
			}
			
			return base.ProcessCmdKey (ref msg, keyData);
		}

		private void menuItemContextEditUndo_Click(object sender, System.EventArgs e)
		{
			
		}

		private void ContextEditRedo_Click(object sender, System.EventArgs e)
		{
			
		}

		private void ContextViewZoom_Popup(object sender, System.EventArgs e)
		{
			
		}

        private void menuItemToAnimationEditor_Click(object sender, EventArgs e)
        {
            
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEditUndo.menuItemEditUndo_Click(this);		
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEditUndo.menuItemEditRedo_Click(this);
        }

        private void toCurveObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditToCurveObject(this);
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditCombine(this);
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditGroup(this);
        }

        private void ungroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditUnGroup(this);
        }

        private void mergeToBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMergeToBitmap(this);
        }

        private void unmergeBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditUnmergeBitmap(this);
        }

        private void toolStripMenuItemZoom10_Click(object sender, EventArgs e)
        {
            OpticGo(10);
        }

        private void toolStripMenuItemZoom25_Click(object sender, EventArgs e)
        {
            OpticGo(25);
        }

        private void toolStripMenuItemZoom50_Click(object sender, EventArgs e)
        {
            OpticGo(50);
        }

        private void toolStripMenuItemZoom75_Click(object sender, EventArgs e)
        {
            OpticGo(75);
        }

        private void toolStripMenuItemZoom100_Click(object sender, EventArgs e)
        {
            OpticGo(100);
        }

        private void toolStripMenuItemZoom150_Click(object sender, EventArgs e)
        {
            OpticGo(150);
        }

        private void toolStripMenuItemZoom200_Click(object sender, EventArgs e)
        {
            OpticGo(200);
        }

        private void toolStripMenuItemZoom300_Click(object sender, EventArgs e)
        {
            OpticGo(300);
        }

        private void toolStripMenuItemZoom400_Click(object sender, EventArgs e)
        {
            OpticGo(400);
        }

        private void toolStripMenuItemZoom500_Click(object sender, EventArgs e)
        {
            OpticGo(500);
        }

        private void toolStripMenuItemZoom600_Click(object sender, EventArgs e)
        {
            OpticGo(600);
        }

        private void toolStripMenuItemZoom700_Click(object sender, EventArgs e)
        {
            OpticGo(700);
        }

        private void toolStripMenuItemZoom800_Click(object sender, EventArgs e)
        {
            OpticGo(800);
        }

        private void toolStripMenuItemZoom900_Click(object sender, EventArgs e)
        {
            OpticGo(900);
        }

        private void toolStripMenuItemZoom1000_Click(object sender, EventArgs e)
        {
            OpticGo(1000);
        }

        private void objectPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassEditProperty.OnUserPropertyClick(this);
        }

        private void contextMenuStripMove_Opened(object sender, EventArgs e)
        {
            this.toCurveObjectToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleToCurveObject(this);
            this.combineToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditCombine(this);
            this.breakObjectToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditBreakObject(this);
            this.groupToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditGroup(this);
            this.ungroupToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditUnGroup(this);
            this.mergeToBitmapToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditMergeToBitmap(this);
            this.unmergeBitmapToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditUnmergeBitmap(this);

            string title = "";
            undoToolStripMenuItem.Enabled = ClassStudioEditUndo.IsPossibleEditUndo(this, ref title);
            undoToolStripMenuItem.Text = title;
            redoToolStripMenuItem.Enabled = ClassStudioEditUndo.IsPossibleEditRedo(this, ref title);
            redoToolStripMenuItem.Text = title;

            toolStripMenuItemZoom10.Checked = (workThis.obj.nOpticRate == 10);
            toolStripMenuItemZoom25.Checked = (workThis.obj.nOpticRate == 25);
            toolStripMenuItemZoom50.Checked = (workThis.obj.nOpticRate == 50);
            toolStripMenuItemZoom75.Checked = (workThis.obj.nOpticRate == 75);
            toolStripMenuItemZoom100.Checked = (workThis.obj.nOpticRate == 100);
            toolStripMenuItemZoom150.Checked = (workThis.obj.nOpticRate == 150);
            toolStripMenuItemZoom200.Checked = (workThis.obj.nOpticRate == 200);
            toolStripMenuItemZoom300.Checked = (workThis.obj.nOpticRate == 300);
            toolStripMenuItemZoom400.Checked = (workThis.obj.nOpticRate == 400);
            toolStripMenuItemZoom500.Checked = (workThis.obj.nOpticRate == 500);
            toolStripMenuItemZoom600.Checked = (workThis.obj.nOpticRate == 600);
            toolStripMenuItemZoom700.Checked = (workThis.obj.nOpticRate == 700);
            toolStripMenuItemZoom800.Checked = (workThis.obj.nOpticRate == 800);
            toolStripMenuItemZoom900.Checked = (workThis.obj.nOpticRate == 900);
            toolStripMenuItemZoom1000.Checked = (workThis.obj.nOpticRate == 1000);

            this.toAnimationEditorToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditCopy(this);

            // 아래에 있다.
            //this.flipHorizontalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(this);
            //this.flipVerticalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(this);

            //this.rotate90DegreeToLeftToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditRotate(this);
            //this.rotate90DegreeToRightToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditRotate(this);
        }

        private void toolStripMenuItem1_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void toAnimationEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSelectedObjectToBitmap dialog = new FormSelectedObjectToBitmap(this, this.workThis.obj);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                AniEditLib.AniEditOnStudio.Run(this);   // 화면을 일단 띄운다.
                AniEditLib.AniEditOnStudio.Insert(dialog.bitmapResult);
            }
        }

        private void menuItemContextViewZoom_Click(object sender, EventArgs e)
        {

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointAdd(this);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointDelete(this);
        }

        private void toLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointToLine(this);
        }

        private void toCurveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointToCurve(this);
        }

        private void cuspToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointCusp(this);
        }

        private void smoothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointSmooth(this);
        }

        private void symmetricalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointSymmetrical(this);
        }

        private void autoCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointAutoClose(this);
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointJoin(this);
        }

        private void breakApartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPointMove.IdmCurvePointBreakApart(this);
        }

        private void objectPropertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClassEditProperty.OnUserPropertyClick(this);
        }

        private void contextMenuStripPointMove_Opened(object sender, EventArgs e)
        {
            this.addToolStripMenuItem.Enabled = editPointMove.IsPossibleAdd(this);
            this.deleteToolStripMenuItem.Enabled = editPointMove.IsPossibleDelete(this);
            this.toLineToolStripMenuItem.Enabled = editPointMove.IsPossibleToLine(this);
            this.toCurveObjectToolStripMenuItem.Enabled = editPointMove.IsPossibleToCurve(this);

            this.cuspToolStripMenuItem.Enabled = editPointMove.IsPossibleCusp(this);
            this.smoothToolStripMenuItem.Enabled = editPointMove.IsPossibleSmooth(this);
            this.symmetricalToolStripMenuItem.Enabled = editPointMove.IsPossibleSymmetrical(this);

            int mode = editPointMove.GetCurveModeBySelectedNodes();
            if (cuspToolStripMenuItem.Enabled)
                cuspToolStripMenuItem.Checked = (mode == 1);
            if (smoothToolStripMenuItem.Enabled)
                smoothToolStripMenuItem.Checked = (mode == 2);
            if (symmetricalToolStripMenuItem.Enabled)
                symmetricalToolStripMenuItem.Checked = (mode == 3);


            this.autoCloseToolStripMenuItem.Enabled = editPointMove.IsPossibleAutoClose(this);
            this.breakApartToolStripMenuItem.Enabled = editPointMove.IsPossibleBreakApart(this);
            this.joinToolStripMenuItem.Enabled = editPointMove.IsPossibleJoin(this);
        }

        private void moveToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMovePost(this);
        }

        private void moveToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMoveBack(this);
        }

        private void moveToPrevFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMovePrevFront(this);
        }

        private void moveToNextBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditMoveNextBack(this);
        }

        private void alignToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideLeft(this);
        }

        private void alignToHorizontalCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToHorzCenter(this);
        }

        private void alignToRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideRight(this);
        }

        private void alignToTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideTop(this);
        }

        private void alignToVerticalCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToVertCenter(this);
        }

        private void alignToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditArrangeSideBottom(this);
        }

        private void alignToModuleHorizontalCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToModuleHorzCenter(this);
        }

        private void alignToModuleVerticalCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditAlignToModuleVertCenter(this);
        }

        private void spaceHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditSpaceHorz(this);
        }

        private void spaceVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditSpaceVert(this);
        }

        private void toolStripMenuItem2_DropDownOpened(object sender, EventArgs e)
        {
            this.alignToLeftToolStripMenuItem.Enabled = (workThis.nSelectCount > 1);
            this.alignToHorizontalCenterToolStripMenuItem.Enabled = (workThis.nSelectCount > 1);
            this.alignToRightToolStripMenuItem.Enabled = (workThis.nSelectCount > 1);
            this.alignToTopToolStripMenuItem.Enabled = (workThis.nSelectCount > 1);
            this.alignToVerticalCenterToolStripMenuItem.Enabled = (workThis.nSelectCount > 1);
            this.alignToBottomToolStripMenuItem.Enabled = (workThis.nSelectCount > 1);

            this.alignToModuleHorizontalCenterToolStripMenuItem.Enabled = (workThis.nSelectCount > 0);
            this.alignToModuleVerticalCenterToolStripMenuItem.Enabled = (workThis.nSelectCount > 0);

            this.spaceHorizontalToolStripMenuItem.Enabled = (workThis.nSelectCountOnlyChild > 2);
            this.spaceVerticalToolStripMenuItem.Enabled = (workThis.nSelectCountOnlyChild > 2);
        }

        private void toolStripMenuItemOrder_DropDownOpened(object sender, EventArgs e)
        {
            this.moveToBackToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMoveBack(this);
            this.moveToFrontToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMovePost(this);
            this.moveToPrevFrontToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMovePrevFront(this);
            this.moveToNextBackToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleMoveNextBack(this);
        }

        private void flipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditFlipHorizontal(this);
        }

        private void flipVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditFlipVertical(this);
        }

        private void rotate90DegreeToRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateRight(this);
        }

        private void rotate90DegreeToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditRotateLeft(this);
        }

        private void toolStripMenuItem3_DropDownOpened(object sender, EventArgs e)
        {
            this.flipHorizontalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(this);
            this.flipVerticalToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditFlip(this);
            this.rotate90DegreeToLeftToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditRotate(this);
            this.rotate90DegreeToRightToolStripMenuItem.Enabled = ClassStudioEdit.IsPossibleEditRotate(this);
        }

        private void breakObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.OnEditBreakObject(this);
        }

        private void FormEditGraphic_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
        }

        private void FormEditGraphic_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                //ChangeToolByHotKey(EnumMainTool.SPUIT); 단축키로 기능 고정 원할시 변경할 것 
                if (cViewMode == EnumViewMode.PANNING)
                {

                    cViewMode = EnumViewMode.CONTROL;
                    this.Cursor = Cursors.Arrow;
                    this.Capture = false;
                    bMouseCaptureFlag = false;
                    this.Cursor = Cursors.Arrow;
                }
            }

        } //panning 모드 해제 추가 24-06-27 hsjeong

        }

        
		
	}


