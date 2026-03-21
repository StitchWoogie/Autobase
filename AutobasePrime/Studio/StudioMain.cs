//#define USE_EXCEPTION_REPORT

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using AutoLibLocal;
using NetTools;
using DialogConfigUser;
using AutoLib;
using System.IO;
using System.Diagnostics;
using DatabaseConnection;
using ReportModule;
using GraphicModule;
using AniEditLib;
using System.Threading;
using FutureVersion;
using PublicStudioLocalMain.Schedule;
using System.Collections.Generic;
using HelpLib;
using Studio.Script;
using AutoLibLocal.KeyLock;
using AutoLibLocal.PostgresSQL;
using System.Globalization;
using static AutoLibLocal.LanguageManager;
using Studio.OPCUA;

namespace Studio
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
    public class StudioMain : System.Windows.Forms.Form
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.StatusBar statusBarMain;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Splitter splitterRight;
        private SplitContainer splitContainer1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonNewModule;
        private ToolStripButton toolStripButtonOpenModule;
        private ToolStripButton toolStripButtonNewReport;
        private ToolStripButton toolStripButtonOpenReport;
        private ToolStripButton toolStripButtonSave;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonScriptEditor;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonTagEditor;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolStripButtonTileHorizontal;
        private ToolStripButton toolStripButtonTileVertical;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton toolStripButtonRunLocalMain;
        private ToolStripButton toolStripButtonHelp;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton toolStripButtonKeyScript;
        private ToolStripButton toolStripButtonMenuScript;
        private ToolStripButton toolStripButtonConfigWebServer;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem projectToolStripMenuItem;
        private ToolStripMenuItem newFileToolStripMenuItem;
        private ToolStripMenuItem graphicModuleToolStripMenuItem;
        private ToolStripMenuItem reportToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem graphicFromMyTempletesToolStripMenuItem;
        private ToolStripMenuItem buyGraphicFromWebTempletesToolStripMenuItem;
        private ToolStripMenuItem graphicFromTempleteFilesToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem openReportToolStripMenuItem;
        private ToolStripMenuItem newSheetReportToolStripMenuItem;
        private ToolStripMenuItem openSheetReportToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem tagEditorToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem scriptToolStripMenuItem;
        private ToolStripMenuItem atProgramStartToolStripMenuItem;
        private ToolStripMenuItem atProgramRunningToolStripMenuItem;
        private ToolStripMenuItem atProgramEndToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem keyScriptToolStripMenuItem;
        private ToolStripMenuItem menuScriptToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem logInScriptToolStripMenuItem;
        private ToolStripMenuItem beforeLogOutScriptToolStripMenuItem;
        private ToolStripMenuItem afterLogOutScriptToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem runLocalMainToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem maintoolToolStripMenuItem;
        private ToolStripMenuItem explorerToolStripMenuItem;
        private ToolStripMenuItem statusBarToolStripMenuItem;
        private ToolStripMenuItem animationEditorToolStripMenuItem;
        private ToolStripMenuItem configToolStripMenuItem;
        private ToolStripMenuItem configUserToolStripMenuItem;
        private ToolStripMenuItem externalEditorToolStripMenuItem;
        private ToolStripMenuItem libraryToolStripMenuItem;
        private ToolStripMenuItem databaseConnectionToolStripMenuItem;
        private ToolStripMenuItem toolBarSetupToolStripMenuItem;
        private ToolStripMenuItem webServerToolStripMenuItem;
        private ToolStripMenuItem webServerSetupToolStripMenuItem;
        private ToolStripMenuItem webTagGroupToolStripMenuItem;
        private ToolStripMenuItem alarmPriorityToolStripMenuItem;
        private ToolStripMenuItem demandControlToolStripMenuItem;
        private ToolStripMenuItem demandNewToolStripMenuItem;
        private ToolStripMenuItem milliDataToolStripMenuItem;
        private ToolStripMenuItem realTimeTestDataToolStripMenuItem;
        private ToolStripMenuItem recipeToolStripMenuItem;
        private ToolStripMenuItem presetToolStripMenuItem;
        private ToolStripMenuItem sQLBindListToolStripMenuItem;
        private ToolStripMenuItem onOffListToolStripMenuItem;
        private ToolStripMenuItem findWaveFileToolStripMenuItem;
        private ToolStripMenuItem arrangeBitmapFilesToolStripMenuItem;
        private ToolStripMenuItem configEtcToolStripMenuItem;
        private ToolStripMenuItem communicationToolStripMenuItem;
        private ToolStripMenuItem portToolStripMenuItem;
        private ToolStripMenuItem networkMemoryServerToolStripMenuItem;
        private ToolStripMenuItem schedulesToolStripMenuItem;
        private ToolStripMenuItem yearlyFixedSchedulesToolStripMenuItem;
        private ToolStripMenuItem yealyAdditionalSchedulesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem modelsToolStripMenuItem;
        private ToolStripMenuItem weeklySchedulesToolStripMenuItem;
        private ToolStripMenuItem deploymentToolStripMenuItem;
        private ToolStripMenuItem remoteProjectEditorToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem1;
        private ToolStripMenuItem remoteASToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem toolStripHelpCheckUpdateHelp;
        private Panel panelMessage;
        private Splitter splitterErrorList;
        private ToolStripMenuItem newScriptToolStripMenuItem;
        private ToolStripMenuItem ApiToolStripMenu;
        private ToolStripMenuItem JsonTemplateToolStripMenuItem;
        private ToolStripMenuItem httpHeaderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripMenuItem scheduleDesignerToolStripMenuItem;
        private ToolStripMenuItem dBToolStripMenuItem;
        private ToolStripMenuItem globalizationToolStripMenuItem;
        private ToolStripMenuItem GlobalSetupToolStripMenuItem;
        private ToolStripMenuItem englishToolStripMenuItem;
        private ToolStripMenuItem koreanToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem chineseToolStripMenuItem;
        private ToolStripMenuItem japaneseToolStripMenuItem;
        private ToolStripMenuItem russianToolStripMenuItem;
        private ToolStripMenuItem vietnameseToolStripMenuItem;
        private ToolStripMenuItem rESTAPIMonitorToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;

        public StudioMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                this.ShowIcon = false;
                /*
                Icon icon = TotalConfig.LoadIconFromConfigFolder("Z_IoT_Studio.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;*/
            }
            else if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                Icon icon = TotalConfig.LoadIconFromConfigFolder("FT-Studio.ico", 0, 0);
                if (icon != null)
                    this.Icon = icon;
            }

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            TotalConfig.defineMode = EnumDefineMode.MODE_EDIT;
            TerminalClass.Init();

            SharedStudio.formMain = this;

            //config.Load();

            this.CreateParams.ClassName = this.CreateParams.ClassName;

            // 태그 편집기에서 사용할 스크립트 편집기를 등록한다.
            DialogTag.TagEditor.FormTagProperty.procEditScript = new DialogTag.TagEditor.FormTagProperty.DeleEditScript(PropertyPageMouseResponse.EditFileScript);

            AutoLibLocal._LibraryInit.SetGuid(HashTool.MakeHash(AutoLibLocal._LibraryInit.GetGuid() + "AutoLibLocal.dll"));

            // OpcUaIpcManager.Start() 제거: 태그 선택 시 필요할 때만 연결 (OpcTool.CreateUaBrowseProvider)
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudioMain));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusBarMain = new System.Windows.Forms.StatusBar();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.panelRight = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNewModule = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenModule = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNewReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenReport = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonScriptEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonKeyScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMenuScript = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonTagEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonConfigWebServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonTileHorizontal = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTileVertical = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRunLocalMain = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.graphicFromMyTempletesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buyGraphicFromWebTempletesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicFromTempleteFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSheetReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSheetReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tagEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.atProgramStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.atProgramRunningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.atProgramEndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.keyScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.logInScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beforeLogOutScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.afterLogOutScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.runLocalMainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maintoolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.explorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.externalEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webServerSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webTagGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alarmPriorityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.demandControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.demandNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.milliDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realTimeTestDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ApiToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.JsonTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.httpHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rESTAPIMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLBindListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onOffListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findWaveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeBitmapFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configEtcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.communicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkMemoryServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.presetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recipeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schedulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yearlyFixedSchedulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yealyAdditionalSchedulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.modelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.weeklySchedulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.scheduleDesignerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deploymentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteProjectEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GlobalSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.koreanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chineseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.japaneseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.russianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vietnameseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripHelpCheckUpdateHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteASToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMessage = new System.Windows.Forms.Panel();
            this.splitterErrorList = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // statusBarMain
            // 
            resources.ApplyResources(this.statusBarMain, "statusBarMain");
            this.statusBarMain.Name = "statusBarMain";
            this.statusBarMain.ShowPanels = true;
            // 
            // splitterRight
            // 
            resources.ApplyResources(this.splitterRight, "splitterRight");
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.TabStop = false;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.Name = "panelRight";
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewModule,
            this.toolStripButtonOpenModule,
            this.toolStripButtonNewReport,
            this.toolStripButtonOpenReport,
            this.toolStripButtonSave,
            this.toolStripSeparator1,
            this.toolStripButtonScriptEditor,
            this.toolStripButtonKeyScript,
            this.toolStripButtonMenuScript,
            this.toolStripSeparator2,
            this.toolStripButtonTagEditor,
            this.toolStripSeparator3,
            this.toolStripButtonConfigWebServer,
            this.toolStripSeparator6,
            this.toolStripButtonTileHorizontal,
            this.toolStripButtonTileVertical,
            this.toolStripSeparator4,
            this.toolStripButtonRunLocalMain,
            this.toolStripSeparator5,
            this.toolStripButtonHelp});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // toolStripButtonNewModule
            // 
            this.toolStripButtonNewModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonNewModule, "toolStripButtonNewModule");
            this.toolStripButtonNewModule.Name = "toolStripButtonNewModule";
            this.toolStripButtonNewModule.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonNewModule.Click += new System.EventHandler(this.toolStripButtonNewModule_Click);
            // 
            // toolStripButtonOpenModule
            // 
            this.toolStripButtonOpenModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonOpenModule, "toolStripButtonOpenModule");
            this.toolStripButtonOpenModule.Name = "toolStripButtonOpenModule";
            this.toolStripButtonOpenModule.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonOpenModule.Click += new System.EventHandler(this.toolStripButtonOpenModule_Click);
            // 
            // toolStripButtonNewReport
            // 
            this.toolStripButtonNewReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonNewReport, "toolStripButtonNewReport");
            this.toolStripButtonNewReport.Name = "toolStripButtonNewReport";
            this.toolStripButtonNewReport.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonNewReport.Click += new System.EventHandler(this.toolStripButtonNewReport_Click);
            // 
            // toolStripButtonOpenReport
            // 
            this.toolStripButtonOpenReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonOpenReport, "toolStripButtonOpenReport");
            this.toolStripButtonOpenReport.Name = "toolStripButtonOpenReport";
            this.toolStripButtonOpenReport.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonOpenReport.Click += new System.EventHandler(this.toolStripButtonOpenReport_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonSave, "toolStripButtonSave");
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonScriptEditor
            // 
            this.toolStripButtonScriptEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonScriptEditor, "toolStripButtonScriptEditor");
            this.toolStripButtonScriptEditor.Name = "toolStripButtonScriptEditor";
            this.toolStripButtonScriptEditor.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonScriptEditor.Click += new System.EventHandler(this.toolStripButtonScriptEditor_Click);
            // 
            // toolStripButtonKeyScript
            // 
            this.toolStripButtonKeyScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonKeyScript, "toolStripButtonKeyScript");
            this.toolStripButtonKeyScript.Name = "toolStripButtonKeyScript";
            this.toolStripButtonKeyScript.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonKeyScript.Click += new System.EventHandler(this.toolStripButtonKeyScript_Click);
            // 
            // toolStripButtonMenuScript
            // 
            this.toolStripButtonMenuScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonMenuScript, "toolStripButtonMenuScript");
            this.toolStripButtonMenuScript.Name = "toolStripButtonMenuScript";
            this.toolStripButtonMenuScript.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonMenuScript.Click += new System.EventHandler(this.toolStripButtonMenuScript_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonTagEditor
            // 
            this.toolStripButtonTagEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonTagEditor, "toolStripButtonTagEditor");
            this.toolStripButtonTagEditor.Name = "toolStripButtonTagEditor";
            this.toolStripButtonTagEditor.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonTagEditor.Click += new System.EventHandler(this.toolStripButtonTagEditor_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButtonConfigWebServer
            // 
            this.toolStripButtonConfigWebServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonConfigWebServer, "toolStripButtonConfigWebServer");
            this.toolStripButtonConfigWebServer.Name = "toolStripButtonConfigWebServer";
            this.toolStripButtonConfigWebServer.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonConfigWebServer.Click += new System.EventHandler(this.toolStripButtonConfigWebServer_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // toolStripButtonTileHorizontal
            // 
            this.toolStripButtonTileHorizontal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonTileHorizontal, "toolStripButtonTileHorizontal");
            this.toolStripButtonTileHorizontal.Name = "toolStripButtonTileHorizontal";
            this.toolStripButtonTileHorizontal.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonTileHorizontal.Click += new System.EventHandler(this.toolStripButtonTileHorizontal_Click);
            // 
            // toolStripButtonTileVertical
            // 
            this.toolStripButtonTileVertical.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonTileVertical, "toolStripButtonTileVertical");
            this.toolStripButtonTileVertical.Name = "toolStripButtonTileVertical";
            this.toolStripButtonTileVertical.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonTileVertical.Click += new System.EventHandler(this.toolStripButtonTileVertical_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripButtonRunLocalMain
            // 
            this.toolStripButtonRunLocalMain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonRunLocalMain, "toolStripButtonRunLocalMain");
            this.toolStripButtonRunLocalMain.Name = "toolStripButtonRunLocalMain";
            this.toolStripButtonRunLocalMain.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonRunLocalMain.Click += new System.EventHandler(this.toolStripButtonRunLocalMain_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButtonHelp, "toolStripButtonHelp");
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStripButtonHelp.Click += new System.EventHandler(this.toolStripButtonHelp_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.configToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.newFileToolStripMenuItem,
            this.openToolStripMenuItem,
            this.openReportToolStripMenuItem,
            this.newSheetReportToolStripMenuItem,
            this.openSheetReportToolStripMenuItem,
            this.toolStripSeparator8,
            this.tagEditorToolStripMenuItem,
            this.toolStripSeparator9,
            this.scriptToolStripMenuItem,
            this.toolStripSeparator12,
            this.runLocalMainToolStripMenuItem,
            this.toolStripSeparator13,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            resources.ApplyResources(this.projectToolStripMenuItem, "projectToolStripMenuItem");
            this.projectToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileProject_Click);
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphicModuleToolStripMenuItem,
            this.reportToolStripMenuItem,
            this.toolStripSeparator7,
            this.graphicFromMyTempletesToolStripMenuItem,
            this.buyGraphicFromWebTempletesToolStripMenuItem,
            this.graphicFromTempleteFilesToolStripMenuItem});
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            resources.ApplyResources(this.newFileToolStripMenuItem, "newFileToolStripMenuItem");
            // 
            // graphicModuleToolStripMenuItem
            // 
            this.graphicModuleToolStripMenuItem.Name = "graphicModuleToolStripMenuItem";
            resources.ApplyResources(this.graphicModuleToolStripMenuItem, "graphicModuleToolStripMenuItem");
            this.graphicModuleToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileNewGraphic_Click);
            // 
            // reportToolStripMenuItem
            // 
            this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            resources.ApplyResources(this.reportToolStripMenuItem, "reportToolStripMenuItem");
            this.reportToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileNewReport_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // graphicFromMyTempletesToolStripMenuItem
            // 
            this.graphicFromMyTempletesToolStripMenuItem.Name = "graphicFromMyTempletesToolStripMenuItem";
            resources.ApplyResources(this.graphicFromMyTempletesToolStripMenuItem, "graphicFromMyTempletesToolStripMenuItem");
            this.graphicFromMyTempletesToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileNewGraphicFromMyTemplete_Click);
            // 
            // buyGraphicFromWebTempletesToolStripMenuItem
            // 
            this.buyGraphicFromWebTempletesToolStripMenuItem.Name = "buyGraphicFromWebTempletesToolStripMenuItem";
            resources.ApplyResources(this.buyGraphicFromWebTempletesToolStripMenuItem, "buyGraphicFromWebTempletesToolStripMenuItem");
            this.buyGraphicFromWebTempletesToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileNewGraphicFromSearchTemplete_Click);
            // 
            // graphicFromTempleteFilesToolStripMenuItem
            // 
            this.graphicFromTempleteFilesToolStripMenuItem.Name = "graphicFromTempleteFilesToolStripMenuItem";
            resources.ApplyResources(this.graphicFromTempleteFilesToolStripMenuItem, "graphicFromTempleteFilesToolStripMenuItem");
            this.graphicFromTempleteFilesToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileNewGraphicFromMyTempleteFiles_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileOpen_Click);
            // 
            // openReportToolStripMenuItem
            // 
            this.openReportToolStripMenuItem.Name = "openReportToolStripMenuItem";
            resources.ApplyResources(this.openReportToolStripMenuItem, "openReportToolStripMenuItem");
            this.openReportToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileOpenReport_Click);
            //
            // newSheetReportToolStripMenuItem
            //
            this.newSheetReportToolStripMenuItem.Name = "newSheetReportToolStripMenuItem";
            this.newSheetReportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newSheetReportToolStripMenuItem.Text = "New Sheet Report";
            this.newSheetReportToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileNewSheetReport_Click);
            //
            // openSheetReportToolStripMenuItem
            //
            this.openSheetReportToolStripMenuItem.Name = "openSheetReportToolStripMenuItem";
            this.openSheetReportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openSheetReportToolStripMenuItem.Text = "Open Sheet Report...";
            this.openSheetReportToolStripMenuItem.Click += new System.EventHandler(this.menuItemFileOpenSheetReport_Click);
            //
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // tagEditorToolStripMenuItem
            // 
            this.tagEditorToolStripMenuItem.Name = "tagEditorToolStripMenuItem";
            resources.ApplyResources(this.tagEditorToolStripMenuItem, "tagEditorToolStripMenuItem");
            this.tagEditorToolStripMenuItem.Click += new System.EventHandler(this.menuItemTagEditor_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.atProgramStartToolStripMenuItem,
            this.atProgramRunningToolStripMenuItem,
            this.atProgramEndToolStripMenuItem,
            this.toolStripSeparator10,
            this.keyScriptToolStripMenuItem,
            this.menuScriptToolStripMenuItem,
            this.toolStripSeparator11,
            this.logInScriptToolStripMenuItem,
            this.beforeLogOutScriptToolStripMenuItem,
            this.afterLogOutScriptToolStripMenuItem,
            this.newScriptToolStripMenuItem});
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            resources.ApplyResources(this.scriptToolStripMenuItem, "scriptToolStripMenuItem");
            // 
            // atProgramStartToolStripMenuItem
            // 
            this.atProgramStartToolStripMenuItem.Name = "atProgramStartToolStripMenuItem";
            resources.ApplyResources(this.atProgramStartToolStripMenuItem, "atProgramStartToolStripMenuItem");
            this.atProgramStartToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptProgramStart_Click);
            // 
            // atProgramRunningToolStripMenuItem
            // 
            this.atProgramRunningToolStripMenuItem.Name = "atProgramRunningToolStripMenuItem";
            resources.ApplyResources(this.atProgramRunningToolStripMenuItem, "atProgramRunningToolStripMenuItem");
            this.atProgramRunningToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptProgramRunning_Click);
            // 
            // atProgramEndToolStripMenuItem
            // 
            this.atProgramEndToolStripMenuItem.Name = "atProgramEndToolStripMenuItem";
            resources.ApplyResources(this.atProgramEndToolStripMenuItem, "atProgramEndToolStripMenuItem");
            this.atProgramEndToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptProgramEnd_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // keyScriptToolStripMenuItem
            // 
            this.keyScriptToolStripMenuItem.Name = "keyScriptToolStripMenuItem";
            resources.ApplyResources(this.keyScriptToolStripMenuItem, "keyScriptToolStripMenuItem");
            this.keyScriptToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptKey_Click);
            // 
            // menuScriptToolStripMenuItem
            // 
            this.menuScriptToolStripMenuItem.Name = "menuScriptToolStripMenuItem";
            resources.ApplyResources(this.menuScriptToolStripMenuItem, "menuScriptToolStripMenuItem");
            this.menuScriptToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptMenu_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // logInScriptToolStripMenuItem
            // 
            this.logInScriptToolStripMenuItem.Name = "logInScriptToolStripMenuItem";
            resources.ApplyResources(this.logInScriptToolStripMenuItem, "logInScriptToolStripMenuItem");
            this.logInScriptToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptLogIn_Click);
            // 
            // beforeLogOutScriptToolStripMenuItem
            // 
            this.beforeLogOutScriptToolStripMenuItem.Name = "beforeLogOutScriptToolStripMenuItem";
            resources.ApplyResources(this.beforeLogOutScriptToolStripMenuItem, "beforeLogOutScriptToolStripMenuItem");
            this.beforeLogOutScriptToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptLogOut_Click);
            // 
            // afterLogOutScriptToolStripMenuItem
            // 
            this.afterLogOutScriptToolStripMenuItem.Name = "afterLogOutScriptToolStripMenuItem";
            resources.ApplyResources(this.afterLogOutScriptToolStripMenuItem, "afterLogOutScriptToolStripMenuItem");
            this.afterLogOutScriptToolStripMenuItem.Click += new System.EventHandler(this.menuItemScriptLogOutAfter_Click);
            // 
            // newScriptToolStripMenuItem
            // 
            this.newScriptToolStripMenuItem.Name = "newScriptToolStripMenuItem";
            resources.ApplyResources(this.newScriptToolStripMenuItem, "newScriptToolStripMenuItem");
            this.newScriptToolStripMenuItem.Click += new System.EventHandler(this.newScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.MergeIndex = 100;
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // runLocalMainToolStripMenuItem
            // 
            this.runLocalMainToolStripMenuItem.MergeIndex = 100;
            this.runLocalMainToolStripMenuItem.Name = "runLocalMainToolStripMenuItem";
            resources.ApplyResources(this.runLocalMainToolStripMenuItem, "runLocalMainToolStripMenuItem");
            this.runLocalMainToolStripMenuItem.Click += new System.EventHandler(this.menuItemRunViewMain_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.MergeIndex = 100;
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.MergeIndex = 100;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maintoolToolStripMenuItem,
            this.explorerToolStripMenuItem,
            this.statusBarToolStripMenuItem,
            this.animationEditorToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownOpened += new System.EventHandler(this.menuItemView_Popup);
            // 
            // maintoolToolStripMenuItem
            // 
            this.maintoolToolStripMenuItem.Name = "maintoolToolStripMenuItem";
            resources.ApplyResources(this.maintoolToolStripMenuItem, "maintoolToolStripMenuItem");
            this.maintoolToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewMainTool_Click);
            // 
            // explorerToolStripMenuItem
            // 
            this.explorerToolStripMenuItem.Name = "explorerToolStripMenuItem";
            resources.ApplyResources(this.explorerToolStripMenuItem, "explorerToolStripMenuItem");
            this.explorerToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewSolution_Click);
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            resources.ApplyResources(this.statusBarToolStripMenuItem, "statusBarToolStripMenuItem");
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewStatusBar_Click);
            // 
            // animationEditorToolStripMenuItem
            // 
            this.animationEditorToolStripMenuItem.Name = "animationEditorToolStripMenuItem";
            resources.ApplyResources(this.animationEditorToolStripMenuItem, "animationEditorToolStripMenuItem");
            this.animationEditorToolStripMenuItem.Click += new System.EventHandler(this.menuItemViewAnimationEditor_Click);
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dBToolStripMenuItem,
            this.configUserToolStripMenuItem,
            this.externalEditorToolStripMenuItem,
            this.libraryToolStripMenuItem,
            this.databaseConnectionToolStripMenuItem,
            this.toolBarSetupToolStripMenuItem,
            this.webServerToolStripMenuItem,
            this.alarmPriorityToolStripMenuItem,
            this.demandControlToolStripMenuItem,
            this.demandNewToolStripMenuItem,
            this.milliDataToolStripMenuItem,
            this.realTimeTestDataToolStripMenuItem,
            this.ApiToolStripMenu,
            this.sQLBindListToolStripMenuItem,
            this.onOffListToolStripMenuItem,
            this.findWaveFileToolStripMenuItem,
            this.arrangeBitmapFilesToolStripMenuItem,
            this.configEtcToolStripMenuItem,
            this.communicationToolStripMenuItem,
            this.presetToolStripMenuItem,
            this.recipeToolStripMenuItem,
            this.schedulesToolStripMenuItem,
            this.deploymentToolStripMenuItem,
            this.remoteProjectEditorToolStripMenuItem,
            this.globalizationToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            resources.ApplyResources(this.configToolStripMenuItem, "configToolStripMenuItem");
            // 
            // dBToolStripMenuItem
            // 
            this.dBToolStripMenuItem.Name = "dBToolStripMenuItem";
            resources.ApplyResources(this.dBToolStripMenuItem, "dBToolStripMenuItem");
            this.dBToolStripMenuItem.Click += new System.EventHandler(this.dBToolStripMenuItem_Click);
            // 
            // configUserToolStripMenuItem
            // 
            this.configUserToolStripMenuItem.Name = "configUserToolStripMenuItem";
            resources.ApplyResources(this.configUserToolStripMenuItem, "configUserToolStripMenuItem");
            this.configUserToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigUser_Click);
            // 
            // externalEditorToolStripMenuItem
            // 
            this.externalEditorToolStripMenuItem.Name = "externalEditorToolStripMenuItem";
            resources.ApplyResources(this.externalEditorToolStripMenuItem, "externalEditorToolStripMenuItem");
            this.externalEditorToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigBitmapEditor_Click);
            // 
            // libraryToolStripMenuItem
            // 
            this.libraryToolStripMenuItem.Name = "libraryToolStripMenuItem";
            resources.ApplyResources(this.libraryToolStripMenuItem, "libraryToolStripMenuItem");
            this.libraryToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigLibrary_Click);
            // 
            // databaseConnectionToolStripMenuItem
            // 
            this.databaseConnectionToolStripMenuItem.Name = "databaseConnectionToolStripMenuItem";
            resources.ApplyResources(this.databaseConnectionToolStripMenuItem, "databaseConnectionToolStripMenuItem");
            this.databaseConnectionToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigDsn_Click);
            // 
            // toolBarSetupToolStripMenuItem
            // 
            this.toolBarSetupToolStripMenuItem.Name = "toolBarSetupToolStripMenuItem";
            resources.ApplyResources(this.toolBarSetupToolStripMenuItem, "toolBarSetupToolStripMenuItem");
            this.toolBarSetupToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigToolBarWindow_Click);
            // 
            // webServerToolStripMenuItem
            // 
            this.webServerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.webServerSetupToolStripMenuItem,
            this.webTagGroupToolStripMenuItem});
            this.webServerToolStripMenuItem.Name = "webServerToolStripMenuItem";
            resources.ApplyResources(this.webServerToolStripMenuItem, "webServerToolStripMenuItem");
            // 
            // webServerSetupToolStripMenuItem
            // 
            this.webServerSetupToolStripMenuItem.Name = "webServerSetupToolStripMenuItem";
            resources.ApplyResources(this.webServerSetupToolStripMenuItem, "webServerSetupToolStripMenuItem");
            this.webServerSetupToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigWebServer_Click);
            // 
            // webTagGroupToolStripMenuItem
            // 
            this.webTagGroupToolStripMenuItem.Name = "webTagGroupToolStripMenuItem";
            resources.ApplyResources(this.webTagGroupToolStripMenuItem, "webTagGroupToolStripMenuItem");
            this.webTagGroupToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigWebTagGroup_Click);
            // 
            // alarmPriorityToolStripMenuItem
            // 
            this.alarmPriorityToolStripMenuItem.Name = "alarmPriorityToolStripMenuItem";
            resources.ApplyResources(this.alarmPriorityToolStripMenuItem, "alarmPriorityToolStripMenuItem");
            this.alarmPriorityToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigAlarmPriority_Click);
            // 
            // demandControlToolStripMenuItem
            // 
            this.demandControlToolStripMenuItem.Name = "demandControlToolStripMenuItem";
            resources.ApplyResources(this.demandControlToolStripMenuItem, "demandControlToolStripMenuItem");
            this.demandControlToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigDemandControl_Click);
            // 
            // demandNewToolStripMenuItem
            // 
            this.demandNewToolStripMenuItem.Name = "demandNewToolStripMenuItem";
            resources.ApplyResources(this.demandNewToolStripMenuItem, "demandNewToolStripMenuItem");
            this.demandNewToolStripMenuItem.Tag = "";
            this.demandNewToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigDemandNew_Click);
            // 
            // milliDataToolStripMenuItem
            // 
            this.milliDataToolStripMenuItem.Name = "milliDataToolStripMenuItem";
            resources.ApplyResources(this.milliDataToolStripMenuItem, "milliDataToolStripMenuItem");
            this.milliDataToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigMilliData_Click);
            // 
            // realTimeTestDataToolStripMenuItem
            // 
            this.realTimeTestDataToolStripMenuItem.Name = "realTimeTestDataToolStripMenuItem";
            resources.ApplyResources(this.realTimeTestDataToolStripMenuItem, "realTimeTestDataToolStripMenuItem");
            this.realTimeTestDataToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigRealTimeTestGraph_Click);
            // 
            // ApiToolStripMenu
            // 
            this.ApiToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JsonTemplateToolStripMenuItem,
            this.httpHeaderToolStripMenuItem,
            this.rESTAPIMonitorToolStripMenuItem});
            this.ApiToolStripMenu.Name = "ApiToolStripMenu";
            resources.ApplyResources(this.ApiToolStripMenu, "ApiToolStripMenu");
            // 
            // JsonTemplateToolStripMenuItem
            // 
            this.JsonTemplateToolStripMenuItem.Name = "JsonTemplateToolStripMenuItem";
            resources.ApplyResources(this.JsonTemplateToolStripMenuItem, "JsonTemplateToolStripMenuItem");
            this.JsonTemplateToolStripMenuItem.Click += new System.EventHandler(this.JsonTemplateToolStripMenuItem_Click);
            // 
            // httpHeaderToolStripMenuItem
            // 
            this.httpHeaderToolStripMenuItem.Name = "httpHeaderToolStripMenuItem";
            resources.ApplyResources(this.httpHeaderToolStripMenuItem, "httpHeaderToolStripMenuItem");
            this.httpHeaderToolStripMenuItem.Click += new System.EventHandler(this.httpHeaderToolStripMenuItem_Click);
            // 
            // rESTAPIMonitorToolStripMenuItem
            // 
            this.rESTAPIMonitorToolStripMenuItem.Name = "rESTAPIMonitorToolStripMenuItem";
            resources.ApplyResources(this.rESTAPIMonitorToolStripMenuItem, "rESTAPIMonitorToolStripMenuItem");
            this.rESTAPIMonitorToolStripMenuItem.Click += new System.EventHandler(this.rESTAPIMonitorToolStripMenuItem_Click);
            // 
            // sQLBindListToolStripMenuItem
            // 
            this.sQLBindListToolStripMenuItem.Name = "sQLBindListToolStripMenuItem";
            resources.ApplyResources(this.sQLBindListToolStripMenuItem, "sQLBindListToolStripMenuItem");
            this.sQLBindListToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigSqlBindList_Click);
            // 
            // onOffListToolStripMenuItem
            // 
            this.onOffListToolStripMenuItem.Name = "onOffListToolStripMenuItem";
            resources.ApplyResources(this.onOffListToolStripMenuItem, "onOffListToolStripMenuItem");
            this.onOffListToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigOnOffList_Click);
            // 
            // findWaveFileToolStripMenuItem
            // 
            this.findWaveFileToolStripMenuItem.Name = "findWaveFileToolStripMenuItem";
            resources.ApplyResources(this.findWaveFileToolStripMenuItem, "findWaveFileToolStripMenuItem");
            this.findWaveFileToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigFindWaveFile_Click);
            // 
            // arrangeBitmapFilesToolStripMenuItem
            // 
            this.arrangeBitmapFilesToolStripMenuItem.Name = "arrangeBitmapFilesToolStripMenuItem";
            resources.ApplyResources(this.arrangeBitmapFilesToolStripMenuItem, "arrangeBitmapFilesToolStripMenuItem");
            this.arrangeBitmapFilesToolStripMenuItem.Click += new System.EventHandler(this.menuItemArrayBitmapFile_Click);
            // 
            // configEtcToolStripMenuItem
            // 
            this.configEtcToolStripMenuItem.Name = "configEtcToolStripMenuItem";
            resources.ApplyResources(this.configEtcToolStripMenuItem, "configEtcToolStripMenuItem");
            this.configEtcToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigEtc_Click);
            // 
            // communicationToolStripMenuItem
            // 
            this.communicationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.portToolStripMenuItem,
            this.networkMemoryServerToolStripMenuItem});
            this.communicationToolStripMenuItem.Name = "communicationToolStripMenuItem";
            resources.ApplyResources(this.communicationToolStripMenuItem, "communicationToolStripMenuItem");
            // 
            // portToolStripMenuItem
            // 
            this.portToolStripMenuItem.Name = "portToolStripMenuItem";
            resources.ApplyResources(this.portToolStripMenuItem, "portToolStripMenuItem");
            this.portToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigCommunicationPort_Click);
            // 
            // networkMemoryServerToolStripMenuItem
            // 
            this.networkMemoryServerToolStripMenuItem.Name = "networkMemoryServerToolStripMenuItem";
            resources.ApplyResources(this.networkMemoryServerToolStripMenuItem, "networkMemoryServerToolStripMenuItem");
            this.networkMemoryServerToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigCommunicationScanServer_Click);
            // 
            // presetToolStripMenuItem
            // 
            this.presetToolStripMenuItem.Name = "presetToolStripMenuItem";
            resources.ApplyResources(this.presetToolStripMenuItem, "presetToolStripMenuItem");
            this.presetToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigPreset_Click);
            // 
            // recipeToolStripMenuItem
            // 
            this.recipeToolStripMenuItem.Name = "recipeToolStripMenuItem";
            resources.ApplyResources(this.recipeToolStripMenuItem, "recipeToolStripMenuItem");
            this.recipeToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigRecipe_Click);
            // 
            // schedulesToolStripMenuItem
            // 
            this.schedulesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yearlyFixedSchedulesToolStripMenuItem,
            this.yealyAdditionalSchedulesToolStripMenuItem,
            this.toolStripSeparator14,
            this.modelsToolStripMenuItem,
            this.weeklySchedulesToolStripMenuItem,
            this.toolStripSeparator16,
            this.scheduleDesignerToolStripMenuItem});
            this.schedulesToolStripMenuItem.Name = "schedulesToolStripMenuItem";
            resources.ApplyResources(this.schedulesToolStripMenuItem, "schedulesToolStripMenuItem");
            // 
            // yearlyFixedSchedulesToolStripMenuItem
            // 
            this.yearlyFixedSchedulesToolStripMenuItem.Name = "yearlyFixedSchedulesToolStripMenuItem";
            resources.ApplyResources(this.yearlyFixedSchedulesToolStripMenuItem, "yearlyFixedSchedulesToolStripMenuItem");
            this.yearlyFixedSchedulesToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigFixedSchedules_Click);
            // 
            // yealyAdditionalSchedulesToolStripMenuItem
            // 
            this.yealyAdditionalSchedulesToolStripMenuItem.Name = "yealyAdditionalSchedulesToolStripMenuItem";
            resources.ApplyResources(this.yealyAdditionalSchedulesToolStripMenuItem, "yealyAdditionalSchedulesToolStripMenuItem");
            this.yealyAdditionalSchedulesToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigAddtionalSchedules_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // modelsToolStripMenuItem
            // 
            this.modelsToolStripMenuItem.Name = "modelsToolStripMenuItem";
            resources.ApplyResources(this.modelsToolStripMenuItem, "modelsToolStripMenuItem");
            this.modelsToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigScheduleModels_Click);
            // 
            // weeklySchedulesToolStripMenuItem
            // 
            this.weeklySchedulesToolStripMenuItem.Name = "weeklySchedulesToolStripMenuItem";
            resources.ApplyResources(this.weeklySchedulesToolStripMenuItem, "weeklySchedulesToolStripMenuItem");
            this.weeklySchedulesToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigWeeklySchedules_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // scheduleDesignerToolStripMenuItem
            // 
            this.scheduleDesignerToolStripMenuItem.Name = "scheduleDesignerToolStripMenuItem";
            resources.ApplyResources(this.scheduleDesignerToolStripMenuItem, "scheduleDesignerToolStripMenuItem");
            this.scheduleDesignerToolStripMenuItem.Click += new System.EventHandler(this.scheduleDesignerToolStripMenuItem_Click);
            // 
            // deploymentToolStripMenuItem
            // 
            this.deploymentToolStripMenuItem.Name = "deploymentToolStripMenuItem";
            resources.ApplyResources(this.deploymentToolStripMenuItem, "deploymentToolStripMenuItem");
            this.deploymentToolStripMenuItem.Click += new System.EventHandler(this.menuItemConfigDeployment_Click);
            //
            // remoteProjectEditorToolStripMenuItem
            //
            this.remoteProjectEditorToolStripMenuItem.Name = "remoteProjectEditorToolStripMenuItem";
            this.remoteProjectEditorToolStripMenuItem.Text = "Remote Project Editor";
            this.remoteProjectEditorToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.remoteProjectEditorToolStripMenuItem.Click += new System.EventHandler(this.menuItemRemoteProjectEditor_Click);
            //
            // globalizationToolStripMenuItem
            // 
            this.globalizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GlobalSetupToolStripMenuItem,
            this.toolStripSeparator17,
            this.englishToolStripMenuItem,
            this.koreanToolStripMenuItem,
            this.chineseToolStripMenuItem,
            this.japaneseToolStripMenuItem,
            this.russianToolStripMenuItem,
            this.vietnameseToolStripMenuItem});
            this.globalizationToolStripMenuItem.Name = "globalizationToolStripMenuItem";
            resources.ApplyResources(this.globalizationToolStripMenuItem, "globalizationToolStripMenuItem");
            this.globalizationToolStripMenuItem.DropDownOpened += new System.EventHandler(this.globalizationToolStripMenuItem_DropDownOpened);
            // 
            // GlobalSetupToolStripMenuItem
            // 
            this.GlobalSetupToolStripMenuItem.Name = "GlobalSetupToolStripMenuItem";
            resources.ApplyResources(this.GlobalSetupToolStripMenuItem, "GlobalSetupToolStripMenuItem");
            this.GlobalSetupToolStripMenuItem.Click += new System.EventHandler(this.globalConfigToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // koreanToolStripMenuItem
            // 
            this.koreanToolStripMenuItem.Name = "koreanToolStripMenuItem";
            resources.ApplyResources(this.koreanToolStripMenuItem, "koreanToolStripMenuItem");
            this.koreanToolStripMenuItem.Click += new System.EventHandler(this.koreanToolStripMenuItem_Click);
            // 
            // chineseToolStripMenuItem
            // 
            this.chineseToolStripMenuItem.Name = "chineseToolStripMenuItem";
            resources.ApplyResources(this.chineseToolStripMenuItem, "chineseToolStripMenuItem");
            this.chineseToolStripMenuItem.Click += new System.EventHandler(this.chineseToolStripMenuItem_Click);
            // 
            // japaneseToolStripMenuItem
            // 
            this.japaneseToolStripMenuItem.Name = "japaneseToolStripMenuItem";
            resources.ApplyResources(this.japaneseToolStripMenuItem, "japaneseToolStripMenuItem");
            this.japaneseToolStripMenuItem.Click += new System.EventHandler(this.japaneseToolStripMenuItem_Click);
            // 
            // russianToolStripMenuItem
            // 
            this.russianToolStripMenuItem.Name = "russianToolStripMenuItem";
            resources.ApplyResources(this.russianToolStripMenuItem, "russianToolStripMenuItem");
            this.russianToolStripMenuItem.Click += new System.EventHandler(this.russianToolStripMenuItem_Click);
            // 
            // vietnameseToolStripMenuItem
            // 
            this.vietnameseToolStripMenuItem.Name = "vietnameseToolStripMenuItem";
            resources.ApplyResources(this.vietnameseToolStripMenuItem, "vietnameseToolStripMenuItem");
            this.vietnameseToolStripMenuItem.Click += new System.EventHandler(this.vietnameseToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.toolStripHelpCheckUpdateHelp,
            this.remoteASToolStripMenuItem,
            this.toolStripSeparator15,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            resources.ApplyResources(this.helpToolStripMenuItem1, "helpToolStripMenuItem1");
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.menuItemHelpStudio_Click);
            // 
            // toolStripHelpCheckUpdateHelp
            // 
            this.toolStripHelpCheckUpdateHelp.Name = "toolStripHelpCheckUpdateHelp";
            resources.ApplyResources(this.toolStripHelpCheckUpdateHelp, "toolStripHelpCheckUpdateHelp");
            this.toolStripHelpCheckUpdateHelp.Click += new System.EventHandler(this.toolStripHelpCheckUpdateHelp_Click);
            // 
            // remoteASToolStripMenuItem
            // 
            this.remoteASToolStripMenuItem.Name = "remoteASToolStripMenuItem";
            resources.ApplyResources(this.remoteASToolStripMenuItem, "remoteASToolStripMenuItem");
            this.remoteASToolStripMenuItem.Click += new System.EventHandler(this.menuItemHelpRemoteAS_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.menuItemHelpAbout_Click);
            // 
            // panelMessage
            // 
            resources.ApplyResources(this.panelMessage, "panelMessage");
            this.panelMessage.Name = "panelMessage";
            // 
            // splitterErrorList
            // 
            resources.ApplyResources(this.splitterErrorList, "splitterErrorList");
            this.splitterErrorList.Name = "splitterErrorList";
            this.splitterErrorList.TabStop = false;
            // 
            // StudioMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitterErrorList);
            this.Controls.Add(this.panelMessage);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.statusBarMain);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "StudioMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.StudioMain_FormClosed);
            this.Load += new System.EventHandler(this.StudioMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        static string[] mainArgs;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
//        [STAThread]
//        static void Main(string[] args)
//        {
//#if USE_EXCEPTION_REPORT
//            try 
//            {
//#endif
//            LanguageTool.ChangeUICulture();
//            mainArgs = args;

//            Process[] pro = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

//            if (pro.Length >= 2)
//            {
//                for (int i = 0; i < pro.Length; i++)
//                {
//                    if (pro[i].Id != Process.GetCurrentProcess().Id)
//                    {
//                        Win32Function.BringWindowToTop(pro[i].MainWindowHandle);
//                        Win32Function.SetForegroundWindow(pro[i].MainWindowHandle);
//                        Win32Function.ShowWindow(pro[i].MainWindowHandle, (int)EnumShowWindow.SW_SHOW);
//                    }
//                }
//            }
//            else
//            {
//                if (CheckStudioLogIn())
//                {
//                    Application.AddMessageFilter(new MessageFilter());
//                    Application.Run(new StudioMain());
//                }
//            }

//#if USE_EXCEPTION_REPORT
//            }
//            catch (Exception exception)
//            {
//                ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);
//                dialog.ShowDialog();
//            }
//#endif
//        }


        //20250214 PSU mutex 이용. 좀비프로세스 종료 로직 추가.
        [STAThread]
        static void Main(string[] args)
        {
#if USE_EXCEPTION_REPORT
            try
            {
#endif
                LanguageTool.ChangeUICulture();
                mainArgs = args;

                // 좀비 프로세스 체크 및 정상 프로세스 활성화
                Process currentProcess = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
                bool hasNormalProcess = false;

                foreach (Process process in processes)
                {
                    if (process.Id != currentProcess.Id)
                    {
                        try
                        {
                            if (process.MainWindowHandle != IntPtr.Zero)
                            {
                                // 실행 파일의 전체 경로 확인
                                string processPath = process.MainModule.FileName;
                                if (processPath.Equals(Application.StartupPath + "\\Studio.exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    // 정상 프로세스 창 활성화
                                    Win32Function.BringWindowToTop(process.MainWindowHandle);
                                    Win32Function.SetForegroundWindow(process.MainWindowHandle);
                                    Win32Function.ShowWindow(process.MainWindowHandle, (int)EnumShowWindow.SW_SHOW);
                                    hasNormalProcess = true;
                                }
                                else
                                {
                                    // MainWindowHandle이 없는 프로세스는 좀비로 간주하고 종료
                                    process.Kill();
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                if (hasNormalProcess)
                {
                    return;
                }

                // Mutex를 이용한 중복 실행 방지
                bool createdNew = false;
                Mutex gM1 = new Mutex(true, "AutoBaseStudioMutex", out createdNew);
                if (createdNew)
                {
                    if (CheckStudioLogIn())
                    {
                        Application.AddMessageFilter(new MessageFilter());
                        Application.Run(new StudioMain());
                    }
                }
                else
                {
                    Process p = Tools.GetPreviousProcess();
                    if (p != null && p.MainWindowHandle != IntPtr.Zero)
                    {
                        Win32Function.BringWindowToTop(p.MainWindowHandle);
                        Win32Function.SetForegroundWindow(p.MainWindowHandle);
                        Win32Function.ShowWindow(p.MainWindowHandle, (int)EnumShowWindow.SW_SHOW);
                    }
                    else
                    {
                        // 정상적인 프로세스를 찾을 수 없는 경우 Mutex를 해제하고 새로 실행
                        gM1.Close();
                        if (CheckStudioLogIn())
                        {
                            Application.AddMessageFilter(new MessageFilter());
                            Application.Run(new StudioMain());
                        }
                    }
                }

#if USE_EXCEPTION_REPORT
            }
            catch (Exception exception)
            {
                ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);
                dialog.ShowDialog();
            }
#endif
        }

        static bool CheckStudioLogIn()
        {
            /*
            bool flag = FormConfigUser.GetDisplayLogInBoxWhenStartStudio();

            if (!flag) return true;

            FormLogIn dialog = new FormLogIn();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SharedData.userInfo.sUsername = dialog.textBoxUsername.Text;    // 스튜디오에서는 admin을 위해서 강제로 이름을 지정해야 한다.
                if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RUN_STUDIO))
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("스튜디오를 실행할 권한이 없습니다.", "권한 없음");
                    else
                        MessageBox.Show("You have not permission to run Studio.", "permission error");

                    return false;
                }

                return true;
            }

            return false;*/

            return true;
        }

        private void menuItemExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void menuItemConfigUser_Click(object sender, System.EventArgs e)
        {
            FormConfigUser dialog = new FormConfigUser();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        void SetMainTitle()
        {
            string version = TotalConfig.AutoBaseIntGetVersionString();
            string oem = TotalConfig.AutoBaseIniGetOemProgramName();

            string platform = "";

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
                platform = " CE";

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                this.Text = oem;
            }
            else
            {
                if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite && oem.IndexOf("Lite") == -1)
                {
                    oem += " Lite";
                }

                if (Tools.IsLangKorean())
                    this.Text = String.Format("{0} 스튜디오 {1} (프로젝트:{2}){3}", oem, version, TotalConfig.sDirWorkProject, platform);
                else if (Tools.IsLangChinese())
                    this.Text = String.Format("{0} Studio {1} (项目:{2}){3}", oem, version, TotalConfig.sDirWorkProject, platform);
                else
                    this.Text = String.Format("{0} Studio {1} (Project:{2}){3}", oem, version, TotalConfig.sDirWorkProject, platform);
            }
        }

        public void OpenCalledDocument(string filename)
        {
            string ext = Path.GetExtension(filename);

            if (String.Compare(ext, ".mod", true) == 0 ||
                String.Compare(ext, ".modx", true) == 0)
            {
                OpenModule(filename);
            }
            else if (String.Compare(ext, ".rpt", true) == 0 ||
                String.Compare(ext, ".rptx", true) == 0)
            {
                FormReportMainEdit form = new FormReportMainEdit(filename,
                    this.statusBarMain,
                    new FormReportMainEdit.CallBackOnMdiActivated(this.MdiActivate),
                    new FormReportMainEdit.CallBackOnPopupView(OnPopupView));
                form.MdiParent = this;
                form.WindowState = FormWindowState.Maximized;
                form.Show();
            }
            else if (String.Compare(ext, ".tagx", true) == 0)
            {
                SharedStudio.formMain.Activate();   // 스튜디오가 뜬 상태에서는 태그화면만 보이므로 활성화를 시키는 것이 좋다.
                RunTagEditor();
            }
            else if (String.Compare(ext, ".ctlx", true) == 0)
            {
                SharedStudio.formMain.Activate();   // 스튜디오가 뜬 상태에서는 태그화면만 보이므로 활성화를 시키는 것이 좋다.
                RunScriptAlways();
            }
            else if (String.Compare(ext, ".cs", true) == 0)
            {
                // 자동으로 호출할 때는 LocalScript\Class1.cs 처럼 사용하든지  
                // D:\NET2008\ScriptLib\ScriptLibCode\FullTest\DebugMain.cs 처럼 모든 경로명을 사용한다.
                OpenScript(filename);
            }
            else if (String.Compare(filename, "FutureVersion", true) == 0)
            {
                FutureVersionGo();
            }
            else if (String.Compare(filename, "Communication", true) == 0)
            {
                Communication.FormConfigAllPort dialog = new Studio.Communication.FormConfigAllPort();

                dialog.ShowDialog(this);
            }
            else
            {

            }
        }

        void ExecuteArgument()
        {
            if (mainArgs.Length > 0)
            {
                OpenCalledDocument(mainArgs[0]);
            }
        }

        static Form formTagEditor = null;

        void RunTagEditor()
        {
            if (formTagEditor != null) return;

            DialogTag.TagEditor.FormTagEditor dialog = new DialogTag.TagEditor.FormTagEditor();
            formTagEditor = dialog;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.bRunLocalMain)
                {
                    this.StartLocalMain("local.tagx", 0);
                }
            }

            formTagEditor = null;
        }

        private void StudioMain_Load(object sender, System.EventArgs e)
        {
            SetFormLocation(); //20241010 PSU 시작모니터 지정.
            SetStudioToPlatformInvironment();

            // 24-12-13 추가. SetMainTitle 전에 키락형태를 체크한다. 단 소프트락만.
            KeyLock keylock = new KeyLock();
            keylock.CheckKeyLockOnlySoftLock(); //Autolock 폴더에 .key 파일이 있는 경우, 1~3초 딜레이가 생긴다.

            SetMainTitle();

            FormSolution solution = new FormSolution();
            SharedStudio.formSolution = solution;
            ReportModule.FormReportChild.lpfnFormSolutionReLoad = new ReportModule.FormReportChild.DelegateFormSolutionReLoad(solution.ReLoad);

            solution.TopLevel = false;
            this.splitContainer1.Panel1.Controls.Add(solution);
            solution.Dock = DockStyle.Fill;
            solution.Show();

            Layer.FormLayer layer = new Studio.Layer.FormLayer();
            layer.TopLevel = false;
            this.splitContainer1.Panel2.Controls.Add(layer);
            layer.Dock = DockStyle.Fill;
            layer.Show();

            string tag_file = String.Format("{0}\\Tag\\local.tagx", TotalConfig.sDirWorkProject);
            DialogTag.TagEditor.Editor.checkTagFileChanged.Register(tag_file);

            PageLoadFutureVersion();


            DialogTag.TagEditor.FormTagProperty.procOpcList = new DialogTag.TagEditor.FormTagProperty.DelegateOpcList(OpcTool.SelectItem);
            DialogTag.TagEditor.FormTagProperty.procOpcListUA = new DialogTag.TagEditor.FormTagProperty.DelegateOpcList(OpcTool.SelectItemUA); // OPC UA 24-09-02 추가 hsjeong

            

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                //this.maintoolToolStripMenuItem.Visible = false;
                this.helpToolStripMenuItem.Visible = false;
                this.toolStrip1.Visible = false;

                this.splitContainer1.Panel2Collapsed = true;

                this.toolStripButtonHelp.Visible = false;

                Icon icon = TotalConfig.LoadIconFromConfigFolder("Z_IoT_Main.ico", 16, 16);
                this.toolStripButtonRunLocalMain.Image = icon.ToBitmap();
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
            {
                //KeyLock keylock = new KeyLock();

                if (keylock.CheckKeyLockLocal())
                {
                    ObjectRoot.eOemTypeKeyLockOnStudio = TotalConfig.eOemKeylockType;
                }

                //this.helpToolStripMenuItem1.Visible = false;
                this.toolStripHelpCheckUpdateHelp.Visible = false;
                this.remoteASToolStripMenuItem.Visible = false;
                //this.toolStripSeparator15.Visible = false;
                this.toolStripSeparator5.Visible = false;
                this.toolStripButtonHelp.Visible = false;
                this.animationEditorToolStripMenuItem.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                this.realTimeTestDataToolStripMenuItem.Visible = false;

                Icon icon = TotalConfig.LoadIconFromConfigFolder("FT-Studio.ico", 16, 16);
                this.toolStripButtonRunLocalMain.Image = icon.ToBitmap();
            }
            else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                //this..realTimeTestDataToolStripMenuItem.Visible = false;
                this.toolStripHelpCheckUpdateHelp.Visible = false;

                // 인증이 끝난후 업체에서 추가 2021-2-1
                this.helpToolStripMenuItem1.Visible = false;
                this.remoteASToolStripMenuItem.Visible = false;
                this.toolStripSeparator15.Visible = false;
            }
            else if (TotalConfig.eOemType == EnumOemType.SCADA_LITE || TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite)
            {
                reportToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Report); // New File-리포터
                openReportToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Report); // New File-리포터

                databaseConnectionToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database); // 환경설정-데이터베이스 연결문자열
                webServerToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.WebServer); // 환경설정-웹서버 설정
                milliDataToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.MilliData); // 환경설정-미세자료 수집
                sQLBindListToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database); // 환경설정-SQL 연결 설정
                onOffListToolStripMenuItem.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.OnOffList); // 환경설정-ON/OFF 리스트 설정

                toolStripButtonNewReport.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Report); // 툴 아이콘-Report New
                toolStripButtonOpenReport.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Report); // 툴 아이콘-Report Open
                toolStripButtonConfigWebServer.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.WebServer); // 툴 아이콘-웹서버 설정
                toolStripSeparator6.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.WebServer); // 툴 아이콘-웹서버 설정 다음 구분 라인.

                ApiToolStripMenu.Visible = TotalConfig.GetOemTypeRight(EnumOemTypeRight.Http); //환경설정-API Setup  20250226 PSU 추가.
            }

            if (TotalConfig.eOemType == EnumOemType.KobasAI)
            {
                this.webServerToolStripMenuItem.Visible = false;
                this.remoteASToolStripMenuItem.Visible = false;
            }

            // solution form보다 뒤에 실행되어야 한다.
            ExecuteArgument();

            // WebView2 환경을 백그라운드에서 사전 생성 (Monaco 에디터 열기 속도 향상)
            ScriptLibEdit.Editor.MonacoEditorBridge.PrewarmWebView2Environment();
        }

        private void SetFormLocation()
        {
            // **전체화면 모드(Maximized)에서는 운영체제가 창의 크기와 위치를 직접 제어하여, Location 속성 설정이 무시된다.
            // 현재 폼의 상태를 저장
            FormWindowState originalState = this.WindowState;
            FormBorderStyle originalStyle = this.FormBorderStyle;

            // 전체 화면 설정 해제
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // 원하는 모니터의 Screen 객체를 가져옵니다.
            //int monitorIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StudioMonitorIndex", 0);
            //Screen targetScreen = Screen.AllScreens[monitorIndex]; // 두 번째 모니터 (인덱스는 0부터 시작)

            // 원하는 모니터의 Screen 객체를 가져옵니다.  20250401 PSU 스튜디오 모니터인덱스 확인 추가.
            int monitorIndex = TotalConfig.LoadRegAutoBaseConfig("LocalMain", "Form", "StudioMonitorIndex", 0);
            Screen targetScreen;

            // monitorIndex가 유효한지 확인 
            if (monitorIndex >= 0 && monitorIndex < Screen.AllScreens.Length)
            {
                targetScreen = Screen.AllScreens[monitorIndex];
            }
            else
            {
                // 유효하지 않은 경우 주 모니터로 설정
                targetScreen = Screen.PrimaryScreen;
            }

            // Form의 위치를 설정합니다.
            this.Location = targetScreen.Bounds.Location;

            // 전체 화면으로 다시 설정
            this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = FormBorderStyle.None;   //None 으로 하면, 창 잘림.

            // 원래 상태로 복원 (필요한 경우)
            //this.WindowState = originalState;
            this.FormBorderStyle = originalStyle;
        }

        void PageLoadFutureVersion()
        {
            if (NextVersion.bScript11)
            {
                FormMessagePanel child = new FormMessagePanel();
                child.TopLevel = false;
                child.Dock = DockStyle.Fill;
                panelMessage.Controls.Add(child);

                child.Show();

                Script11.Init();
                //ScriptLibEdit.Debugger.DebuggerEditMain.SendNextCommand();
            }
            else
            {
                ShowMessagePanel(false);
                this.newScriptToolStripMenuItem.Visible = false;
            }
        }

        public void ShowMessagePanel(bool visible)
        {
            this.splitterErrorList.Visible = visible;
            this.panelMessage.Visible = visible;
        }

        public void OpenModule(string path)
        {
            string filename = path;
            FormEditGraphicFrame form;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i].Name == "FormEditGraphicFrame")
                {
                    form = (FormEditGraphicFrame)this.MdiChildren[i];
                    if (String.Compare(filename, form.formChild.workThis.filename, true) == 0)
                    {
                        //form.Activate();	// ?? form.Focus() 도 확실하지 않음
                        form.Select();

                        if (form.WindowState == FormWindowState.Minimized)
                            form.WindowState = FormWindowState.Normal;
                        return;
                    }
                }
            }

            bool first_flag = (this.MdiChildren.Length == 0);
            form = new FormEditGraphicFrame(filename, 0, this.statusBarMain, new FormEditGraphicFrame.CallBackOnMdiActivated(this.MdiActivate), null);
            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        public void OpenReport(string path)
        {
            //string filename = Path.GetFileName(path);
            string filename = path;
            FormReportMainEdit form;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i].Name == "FormReportMainEdit")
                {
                    form = (FormReportMainEdit)this.MdiChildren[i];
                    if (String.Compare(filename, form.formChild.sFilename, true) == 0)
                    {
                        //form.Activate();	// ?? form.Focus() 도 확실하지 않음
                        form.Select();

                        if (form.WindowState == FormWindowState.Minimized)
                            form.WindowState = FormWindowState.Normal;
                        return;
                    }
                }
            }

            bool first_flag = (this.MdiChildren.Length == 0);
            form = new FormReportMainEdit(filename, this.statusBarMain, new FormReportMainEdit.CallBackOnMdiActivated(this.MdiActivate), new FormReportMainEdit.CallBackOnPopupView(OnPopupView));
            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        public FormNewScriptEditor OpenScript(string path)
        {
            string filename = path;
            FormNewScriptEditor form;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i].GetType() == typeof(FormNewScriptEditor))
                {
                    form = (FormNewScriptEditor)this.MdiChildren[i];
                    if (String.Compare(filename, form.formChild.panelEditor.sFilename, true) == 0)
                    {
                        //form.Activate();	// ?? form.Focus() 도 확실하지 않음
                        form.Select();

                        if (form.WindowState == FormWindowState.Minimized)
                            form.WindowState = FormWindowState.Normal;
                        return form;
                    }
                }
            }

            bool first_flag = (this.MdiChildren.Length == 0);
            form = new FormNewScriptEditor(filename, 0, this.statusBarMain, new FormNewScriptEditor.CallBackOnMdiActivated(this.MdiActivate));
            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();

            return form;
        }

        static string sLastDirGraphicOpen = null;

        void MenuItemFileOpen()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Module files (*.modx) unicode|*.modx|Module files (*.mod) ascii|*.mod";

            if (sLastDirGraphicOpen != null)
                dialog.InitialDirectory = sLastDirGraphicOpen;
            else
                dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";

            if (Tools.IsLangKorean())
                dialog.Title = "그래픽 모듈 열기";
            else if (Tools.IsLangJapanese())
                dialog.Title = "グラフィック モジュール 開く";
            else if (Tools.IsLangChinese())
                dialog.Title = "打开图形模块";
            else
                dialog.Title = "Open Graphic Module";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                sLastDirGraphicOpen = Path.GetDirectoryName(dialog.FileName);

                OpenModule(dialog.FileName);
            }
        }

        private void menuItemFileOpen_Click(object sender, System.EventArgs e)
        {
            MenuItemFileOpen();
        }

        private void notifyIcon1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //NotifyIcon
            //StatusBar
        }

        private void OnPopupView()
        {
            this.statusBarToolStripMenuItem.Checked = statusBarMain.Visible;
            this.explorerToolStripMenuItem.Checked = panelRight.Visible;
            this.maintoolToolStripMenuItem.Checked = toolStrip1.Visible;
            this.animationEditorToolStripMenuItem.Checked = (AniEditLib.AniEditOnStudio.formAniEditor != null);
        }

        public void menuItemView_Popup(object sender, System.EventArgs e)
        {
            OnPopupView();
        }

        private void menuItemViewStatusBar_Click(object sender, System.EventArgs e)
        {
            statusBarMain.Visible = !statusBarMain.Visible;
        }

        private void menuItemViewSolution_Click(object sender, System.EventArgs e)
        {
            bool flag = !panelRight.Visible;
            panelRight.Visible = flag;
            splitterRight.Visible = flag;
        }

        private void menuItemViewMainTool_Popup(object sender, System.EventArgs e)
        {

        }

        private void menuItemConfigBitmapEditor_Click(object sender, System.EventArgs e)
        {
            FormConfigBitmapEditor dialog = new FormConfigBitmapEditor();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigLibrary_Click(object sender, System.EventArgs e)
        {
            FormConfigLibrary dialog = new FormConfigLibrary();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        /*
        private void menuItemConfigBindList_Click(object sender, System.EventArgs e)
        {

        }

        
        private void menuItemFileScriptOnProgramStart_Click(object sender, System.EventArgs e)
        {
            FormScriptEditorSimpleOld form = new FormScriptEditorSimpleOld();
            form.MdiParent = this;
            form.Show();
        }

        private void menuItemFileScriptOnProgramEnd_Click(object sender, System.EventArgs e)
        {

        }*/

        private void menuItemHelpAbout_Click(object sender, System.EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();

            dialog.ProgramIcon = this.Icon;
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
        }

        private void menuItemConfigDsn_Click(object sender, System.EventArgs e)
        {
            ConnectionStringList list = new ConnectionStringList();
            list.ConnectionStringLoad();

            FormDatabaseConnection dialog = new FormDatabaseConnection(list);

            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {

            }
        }

        private void menuItemConfigToolBarWindow_Click(object sender, System.EventArgs e)
        {
            FormConfigToolBarWindow dialog = new FormConfigToolBarWindow();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemHelpStudio_Click(object sender, System.EventArgs e)
        {
            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                string filename = String.Format("{0}\\Help\\Studio.chm", Application.StartupPath);
                Help.ShowHelp(this, filename, "");   // 도움말이 한번 로딩되면 그 도움말이 캐시에 남아 있는것 같음 언어를 바꾸어도 되지 않음
            }
            else
            {
                ClassHelp.ShowHelp(this, "Studio.chm", "", true);
            }
        }

        /*
        private void menuItemHelpScript_Click(object sender, System.EventArgs e)
        {
            ClassHelp.ShowHelp(this, "Script.chm", "ScriptStart.htm", false);
        }

        private void menuItemHelpReport_Click(object sender, System.EventArgs e)
        {
            ClassHelp.ShowHelp(this, "Report.chm", "ReportMain.htm", false);
        }*/

        void ConfigWebServer()
        {
            if (!SaveAllDocuments()) return;

            FormConfigWebServer dialog = new FormConfigWebServer();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigWebServer_Click(object sender, System.EventArgs e)
        {
            ConfigWebServer();
        }

        static string sLastDirReportOpen = null;

        void MenuItemFileOpenReport()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Report files (*.rptx;*.rpt) unicode|*.rptx;*.rpt|Report files (*.rptx) unicode|*.rptx|Report files (*.rpt) ascii|*.rpt";

            if (sLastDirReportOpen != null)
                dialog.InitialDirectory = sLastDirReportOpen;
            else
                dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\report";

            if (Tools.IsLangKorean())
                dialog.Title = "리포터 열기";
            else if (Tools.IsLangJapanese())
                dialog.Title = "レポート開く";
            else if (Tools.IsLangChinese())
                dialog.Title = "打开报表";
            else
                dialog.Title = "Open Report";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                sLastDirReportOpen = Path.GetDirectoryName(dialog.FileName);

                this.OpenReport(dialog.FileName);
            }
        }

        private void menuItemFileOpenReport_Click(object sender, System.EventArgs e)
        {
            MenuItemFileOpenReport();
        }

        #region 새 시트 리포트 (Reporting.Designer)

        private void menuItemFileNewSheetReport_Click(object sender, EventArgs e)
        {
            OpenNewSheetReport(null);
        }

        private void menuItemFileOpenSheetReport_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                dlg.Title = "Open Sheet Report";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    OpenNewSheetReport(dlg.FileName);
                }
            }
        }

        /// <summary>
        /// 새 시트 리포트 편집기를 MDI 자식으로 열기
        /// </summary>
        public void OpenNewSheetReport(string filePath)
        {
            // 이미 열려있는 파일인지 확인
            if (!string.IsNullOrEmpty(filePath))
            {
                for (int i = 0; i < this.MdiChildren.Length; i++)
                {
                    if (this.MdiChildren[i] is Reporting.Designer.FormReportDesigner existing)
                    {
                        if (string.Equals(existing.Text, System.IO.Path.GetFileName(filePath), StringComparison.OrdinalIgnoreCase))
                        {
                            existing.Select();
                            if (existing.WindowState == FormWindowState.Minimized)
                                existing.WindowState = FormWindowState.Normal;
                            return;
                        }
                    }
                }
            }

            bool firstChild = (this.MdiChildren.Length == 0);
            var form = new Reporting.Designer.FormReportDesigner(
                filePath, this.statusBarMain,
                new Reporting.Designer.FormReportDesigner.CallBackOnMdiActivated(this.MdiActivate),
                new Reporting.Designer.FormReportDesigner.CallBackOnPopupView(OnPopupView));
            form.MdiParent = this;
            if (firstChild) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        #endregion

        static int nNewModule = 0;

        public void OnFileNewGraphic()
        {
            string filename;

            while (true)
            {
                filename = String.Format("{0}\\graphic\\noname{1:00}.modx", TotalConfig.sDirWorkProject, nNewModule++);

                if (!File.Exists(filename)) break;
            }

            bool first_flag = (this.MdiChildren.Length == 0);

            FormEditGraphicFrame form;

            form = new FormEditGraphicFrame(filename, 1, this.statusBarMain, new FormEditGraphicFrame.CallBackOnMdiActivated(this.MdiActivate), null);

            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        public void FileNewGraphicByTemplete(ObjectRoot root, string templete_name)
        {
            string filename;

            int no = 1;

            filename = String.Format("{0}\\graphic\\{1}.modx", TotalConfig.sDirWorkProject, templete_name);

            while (true)
            {
                if (!File.Exists(filename)) break;

                filename = String.Format("{0}\\graphic\\{1}_{2:00}.modx", TotalConfig.sDirWorkProject, templete_name, no++);
            }

            bool first_flag = (this.MdiChildren.Length == 0);

            FormEditGraphicFrame form;

            form = new FormEditGraphicFrame(filename, 2, this.statusBarMain, new FormEditGraphicFrame.CallBackOnMdiActivated(this.MdiActivate), root);

            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }


        private void menuItemFileNewGraphic_Click(object sender, System.EventArgs e)
        {
            OnFileNewGraphic();
            AutoSaveExcute();
        }

        static int nNewReportNo = 0;

        void MenuItemFileNewReport()
        {
            string filename;

            while (true)
            {
                filename = String.Format("{0}\\report\\noname{1:00}.rptx", TotalConfig.sDirWorkProject, nNewReportNo++);

                if (!File.Exists(filename)) break;
            }

            bool first_flag = (this.MdiChildren.Length == 0);
            FormReportMainEdit form = new FormReportMainEdit(filename, this.statusBarMain, new FormReportMainEdit.CallBackOnMdiActivated(this.MdiActivate), new FormReportMainEdit.CallBackOnPopupView(OnPopupView));
            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void menuItemFileNewReport_Click(object sender, System.EventArgs e)
        {
            MenuItemFileNewReport();
            AutoSaveExcute();
        }

        private void menuItemConfigAlarmPriority_Click(object sender, System.EventArgs e)
        {
            FormConfigAlarmPriority dialog = new FormConfigAlarmPriority();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigDemandControl_Click(object sender, System.EventArgs e)
        {
            FormConfigDemandControl.FunctionBlockDemandControl(this);
        }
        private void menuItemConfigDemandNew_Click(object sender, System.EventArgs e)
        {
            FormConfigDemandNew.FunctionBlockDemandNew(this);
        }

        private void menuItemConfigMilliData_Click(object sender, System.EventArgs e)
        {
            FormConfigMilliData.ConfigMilliData();
        }

        private void menuItemTagEditor_Click(object sender, System.EventArgs e)
        {
            //DialogTag.TagEditor.FormTagEditor.OpenTagEditor(this);
            RunTagEditor();
        }

        private void menuItemScriptProgramStart_Click(object sender, System.EventArgs e)
        {
            string path = String.Format("{0}\\Control\\StartEnd\\START.ctlx", TotalConfig.sDirWorkProject);
            path = FormEditGraphic.CheckAndCopyControl(path);

            if (FormScriptAlways.IsExternalScriptEditorKey())
            {
                if (!File.Exists(path))
                {
                    GraphicModule.ScriptClass sample = new GraphicModule.ScriptClass();
                    sample.SaveFileToMODX(path);
                }
                FormScriptAlways.RunExternalScriptEditor(path);
                return;
            }

            FormScriptEditor dialog = new FormScriptEditor();

            dialog.EnableScanTimeUse(false);
            dialog.SetScript(path);

            if (Tools.IsLangKorean())
                dialog.SetDescription("감시 프로그램 시작 시 한번 실행할 스크립트");
            else if (Tools.IsLangChinese())
                dialog.SetDescription("打开监控程序时，要运行一次的脚本");
            else
                dialog.SetDescription("Script when Local main program started.");

            dialog.ShowDialog();
        }

        void RunScriptAlways()
        {
            FormScriptAlways dialog = new FormScriptAlways();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(SharedStudio.formMain);
            if (dialog.bEndButtonRunLocalMain)
            {
                this.StartLocalMain("test.ctlx", 0);
            }
        }

        private void menuItemScriptProgramRunning_Click(object sender, System.EventArgs e)
        {
            RunScriptAlways();
        }

        private void menuItemScriptProgramEnd_Click(object sender, System.EventArgs e)
        {
            string path = String.Format("{0}\\Control\\StartEnd\\END.ctlx", TotalConfig.sDirWorkProject);
            path = FormEditGraphic.CheckAndCopyControl(path);

            if (FormScriptAlways.IsExternalScriptEditorKey())
            {
                if (!File.Exists(path))
                {
                    GraphicModule.ScriptClass sample = new GraphicModule.ScriptClass();
                    sample.SaveFileToMODX(path);
                }
                FormScriptAlways.RunExternalScriptEditor(path);
                return;
            }

            FormScriptEditor dialog = new FormScriptEditor();

            dialog.EnableScanTimeUse(false);
            dialog.SetScript(path);

            if (Tools.IsLangKorean())
                dialog.SetDescription("감시 프로그램 종료 시 한번 실행할 스크립트");
            else if (Tools.IsLangChinese())
                dialog.SetDescription("在退出监控程序时，要运行一次的脚本");
            else
                dialog.SetDescription("Script when Local main program ended.");

            dialog.ShowDialog();
        }

        private void menuItemScriptKey_Click(object sender, System.EventArgs e)
        {
            FormScriptKey dialog = new FormScriptKey();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemScriptMenu_Click(object sender, System.EventArgs e)
        {
            FormScriptMenu dialog = new FormScriptMenu();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemScriptLogIn_Click(object sender, System.EventArgs e)
        {
            string path = TotalConfig.FileOldNew("Control\\LogInOut", "LogIn.ctl", "LogIn.ctlx");
            path = FormEditGraphic.CheckAndCopyControl(path);

            if (FormScriptAlways.IsExternalScriptEditorKey())
            {
                if (!File.Exists(path))
                {
                    GraphicModule.ScriptClass sample = new GraphicModule.ScriptClass();
                    sample.SaveFileToMODX(path);
                }
                FormScriptAlways.RunExternalScriptEditor(path);
                return;
            }


            FormScriptEditor dialog = new FormScriptEditor();

            dialog.EnableScanTimeUse(false);
            dialog.SetScript(path);

            if (Tools.IsLangKorean())
                dialog.SetDescription("사용자 로그인 시 실행할 스크립트");
            else if (Tools.IsLangChinese())
                dialog.SetDescription("用户登记时，要运行的脚本");
            else
                dialog.SetDescription("Script when uer login.");

            dialog.ShowDialog();
        }

        private void menuItemScriptLogOut_Click(object sender, System.EventArgs e)
        {
            string path = TotalConfig.FileOldNew("Control\\LogInOut", "LogOut.ctl", "LogOut.ctlx");
            path = FormEditGraphic.CheckAndCopyControl(path);

            if (FormScriptAlways.IsExternalScriptEditorKey())
            {
                if (!File.Exists(path))
                {
                    GraphicModule.ScriptClass sample = new GraphicModule.ScriptClass();
                    sample.SaveFileToMODX(path);
                }
                FormScriptAlways.RunExternalScriptEditor(path);
                return;
            }

            FormScriptEditor dialog = new FormScriptEditor();

            dialog.EnableScanTimeUse(false);
            dialog.SetScript(path);

            if (Tools.IsLangKorean())
                dialog.SetDescription("사용자 로그아웃 전에 실행할 스크립트");
            else if (Tools.IsLangChinese())
                dialog.SetDescription("用户退出前，要运行的脚本");
            else
                dialog.SetDescription("Script before logout.");

            dialog.ShowDialog();
        }

        private void menuItemScriptLogOutAfter_Click(object sender, System.EventArgs e)
        {
            string path = TotalConfig.FileOldNew("Control\\LogInOut", "AfterLogOut.ctl", "AfterLogOut.ctlx");
            path = FormEditGraphic.CheckAndCopyControl(path);

            if (FormScriptAlways.IsExternalScriptEditorKey())
            {
                if (!File.Exists(path))
                {
                    GraphicModule.ScriptClass sample = new GraphicModule.ScriptClass();
                    sample.SaveFileToMODX(path);
                }
                FormScriptAlways.RunExternalScriptEditor(path);
                return;
            }

            FormScriptEditor dialog = new FormScriptEditor();

            dialog.EnableScanTimeUse(false);
            dialog.SetScript(path);

            if (Tools.IsLangKorean())
                dialog.SetDescription("사용자 로그아웃 후에 실행할 스크립트");
            else if (Tools.IsLangChinese())
                dialog.SetDescription("用户退出后，要运行的脚本");
            else
                dialog.SetDescription("Script after logout.");

            dialog.ShowDialog();
        }

        private void menuItemConfigSqlBindList_Click(object sender, System.EventArgs e)
        {
            FormConfigSqlBindList.ConfigSqlBindList();
        }

        private void menuItemConfigOnOffList_Click(object sender, System.EventArgs e)
        {
            FormConfigOnOffList dialog = new FormConfigOnOffList();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigFindWaveFile_Click(object sender, System.EventArgs e)
        {
            FormConfigSearchSoundFile dialog = new FormConfigSearchSoundFile();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemArrayBitmapFile_Click(object sender, System.EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("열려있는 창을 모두 닫은 후에 사용할 수 있습니다.");
                }
                else if (Tools.IsLangChinese())
                {
                    MessageBox.Show("把现在打开的所有窗户关闭了以后，才能使用。");
                }
                else
                {
                    MessageBox.Show("Use this menu after close all mdi window.");
                }
                return;
            }

            FormConfigArrangeBitmapFiles dialog = new FormConfigArrangeBitmapFiles();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="debug_mode">0=none debug mode, 1=debug run, 2 = debug (stepinto/stopover)</param>
        /// <returns></returns>
        public bool StartLocalMain(string argument, int debug_mode)
        {
            Process p = SeekProcess("LocalMain");

            if (p != null)
            {
                if (Tools.IsLangKorean())
                {
                    if (MessageBox.Show("감시 프로그램이 현재 실행 중입니다.\n감시 프로그램을 재시작 할까요?", "실행 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return false;
                }
                else
                {
                    if (MessageBox.Show("LocalMain program is running.\nRestart LocalMain program?", "Restart", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return false;
                }
            }

            if (!CloseAnotherProgram("LocalMain")) return false;

            string path = Application.StartupPath + "\\LocalMain.exe";

            if (!File.Exists(path)) 
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show(path, "실행파일이 존재하지 않습니다.");
                else
                    MessageBox.Show(path, "The exe file does not exist.");

                return false;
            }

            if (argument.Length != 0)
            {
                argument = '"' + argument + '"';
            }

            if (debug_mode > 0)    
            {
                if (argument.Length > 0)
                    argument += " ";        // 한칸 띄고

                argument += String.Format("\"Debug={0}\"", debug_mode);
            }
                        
            if (argument.Length == 0)
                Process.Start(path);
            else
            {
                Process.Start(path, argument);
            }

            return true;
        }

        public static Process SeekProcess(string processname)
        {
            Process[] p;

            p = Process.GetProcessesByName(processname);

            if (p.Length > 0)
            {
                return p[0];
            }

            p = Process.GetProcessesByName(processname+".vshost");
            if (p.Length > 0)
            {
                return p[0];
            }

            return null;
        }

        public static void ShutDownProcess(string processname)
        {
            Process p = SeekProcess(processname);
            if (p == null) return;
            p.Kill();
        }

        bool CloseAnotherProgram(string processname)
        {
            Process p = SeekProcess(processname);

            if (p != null)
            {
                Win32Function.PostMessage(p.MainWindowHandle, 0x111, (IntPtr)(int)EnumIdmPublic.IDM_PUBLIC_DESTROY_WINDOW, (IntPtr)12345678);

                TimeOutClass timeout = new TimeOutClass();
                while (true)
                {
                    if (timeout.IsTimeOut(10))
                    {
                        string msg;
                        
                        if(Tools.IsLangKorean())
                            msg = String.Format("{0} 프로그램을 종료할 수 없습니다.\n강제로 종료할까요?", processname);
                        else
                            msg = "Can't close the " + processname + " Program.\nShut down the program?";

                        if (MessageBox.Show(msg, "error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ShutDownProcess(processname);
                            return true;
                        }
                        return false;
                    }

                    Thread.Sleep(10);
                    p = SeekProcess(processname);
                    if (p == null) break;
                }
            }

            return true;
        }

        public static void StartCompactMain(string argument)
        {
            RemoteApi remote = new RemoteApi();

            if (!remote.Init(true)) return;

            if(Path.IsPathRooted(argument)) {
                //argument = Path.GetFileName(argument);
                string ext = Path.GetExtension(argument);
                if (String.Compare(ext, ".modx", true) == 0)
                {
                    argument = argument.Substring(TotalConfig.sDirWorkProject.Length+9); // Work+ \\Graphic\\
                }
                else
                {
                    argument = Path.GetFileName(argument);
                }
            }

            string cmdline = "\"" + argument + "\" Restart";

            string target_root = FormConfigCompactVersion.GetDeployTargetFolder();

            string exe_name = String.Format("{0}\\Runtime\\CompactMain.exe", target_root);
            remote.CreateProcess(exe_name, cmdline);

            remote.Uninit();
        }

        public string GetFilenameOfActiveMdiChild()
        {
            Form child = ActiveMdiChild;
            string argument = "";

            if (child != null)
            {
                if (child.Name == "FormEditGraphicFrame")
                {
                    FormEditGraphicFrame form = (FormEditGraphicFrame)child;

                    argument = form.formChild.workThis.filename;
                }
                else if (child.Name == "FormReportMainEdit")
                {
                    FormReportMainEdit form = (FormReportMainEdit)child;

                    argument = form.formChild.sFilename;
                }
                else { }
            }

            return argument;
        }

        void MenuItemRunViewMain()
        {
            if (!SaveAllDocuments()) return;

            string argument = GetFilenameOfActiveMdiChild();

            /*
            Form child = ActiveMdiChild;
            string argument = "";

            if (child != null)
            {
                if (child.Name == "FormEditGraphicFrame")
                {
                    FormEditGraphicFrame form = (FormEditGraphicFrame)child;

                    argument = form.formChild.workThis.filename;
                }
                else if (child.Name == "FormReportMainEdit")
                {
                    FormReportMainEdit form = (FormReportMainEdit)child;

                    argument = form.formChild.sFilename;
                }
                else if (child.Name == "FormEditRealModuleFrame")
                {
                    FormEditRealModuleFrame form = (FormEditRealModuleFrame)child;

                    argument = form.formChild.sFileName;
                }
                else { }
            }*/

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                // StartCompactMain(argument);
                FutureVersion.FormConfigCompactVersion dialog = new FutureVersion.FormConfigCompactVersion();

                dialog.ShowDialog(this);
            }
            else
            {
                StartLocalMain(argument, 0);
            }

            /*
            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                StartCompactMain(argument);
            }
            else
            {
                StartLocalMain(argument, 0);
            }*/
        }

        public bool SaveAllDocuments()
        {
            Form[] childForms = this.MdiChildren;
            Form child;
            //Make sure to ask for saving the doc before exiting the app 

            for (int i = 0; i < childForms.Length; i++)
            {
                child = childForms[i];

                if (child.GetType() == typeof(FormEditGraphicFrame))
                {
                    if(!((FormEditGraphicFrame)child).formChild.FileSave()) return false;
                }
                else if (child.GetType() == typeof(FormReportMainEdit))
                {
                    if(!((FormReportMainEdit)child).formChild.menuItemFileSave_Click()) return false;
                }
                else if (child.GetType() == typeof(FormNewScriptEditor))
                {
                    if (!((FormNewScriptEditor)child).FileSave()) return false;
                }
            }

            return true;
        }

        private void menuItemRunViewMain_Click(object sender, System.EventArgs e)
        {
            MenuItemRunViewMain();
        }

        private void menuItemViewMainTool_Click(object sender, System.EventArgs e)
        {
            toolStrip1.Visible = !toolStrip1.Visible;
        }
        
        private void menuItemFileScript_Click(object sender, System.EventArgs e)
        {

        }

        private void menuItemConfigEtc_Click(object sender, EventArgs e)
        {
            FormConfigEtc dialog = new FormConfigEtc();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        void MdiActivate(Form form, string filepath, object obj)
        {
            SharedStudio.formSolution.MdiActivate(filepath);
            Layer.FormLayer.MdiActivate(form, obj);
        }

        void FutureVersionGo()
        {
            string retn = FormFutureVersion.FutureVersionGo();

            if (retn == "New Graphic")
            {
                //OnFileNewRealGraphic();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 0x100)
            {
                if (keyData == (Keys.F12 | Keys.Control | Keys.Shift | Keys.Alt))
                {
                    FutureVersionGo();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void menuItemHelpRemoteAS_Click(object sender, EventArgs e)
        {
            PublicStudioLocalMain.ClassPublic.CallRemoteAS("", "", "");
        }

        private void menuItemConfigWebTagGroup_Click(object sender, EventArgs e)
        {
            AutoLibLocal.FormConfigWebTagList list = new FormConfigWebTagList();

            list.StartPosition = FormStartPosition.CenterParent;
            list.ShowDialog(this);
        }

        WebLibraryModFull weblibraryModFull = new WebLibraryModFull();  // 환경을 저장해서 두번이상 열었을 때 기존의 위치로 가기위해서 외부에 선언

        private void menuItemFileNewGraphicFromMyTemplete_Click(object sender, EventArgs e)
        {
            FormInsertFromLibrary dialog = new FormInsertFromLibrary(weblibraryModFull);

            dialog.Text =  this.graphicFromMyTempletesToolStripMenuItem.Text;

            dialog.FormBorderStyle = FormBorderStyle.Sizable;
            dialog.MaximizeBox = true;
            dialog.MinimizeBox = true;
            dialog.WindowState = FormWindowState.Maximized;
            dialog.Owner = SharedStudio.formMain;

            dialog.ShowDialog(this);
        }

        private void menuItemFileNewGraphicFromSearchTemplete_Click(object sender, EventArgs e)
        {
            if (!WebLibraryGate.LogIn()) return;

            FormInsertFromLibrarySearch dialog = new FormInsertFromLibrarySearch(weblibraryModFull);

            dialog.Text = this.buyGraphicFromWebTempletesToolStripMenuItem.Text;

            dialog.FormBorderStyle = FormBorderStyle.Sizable;
            dialog.MaximizeBox = true;
            dialog.MinimizeBox = true;
            dialog.WindowState = FormWindowState.Maximized;
            dialog.Owner = SharedStudio.formMain;

            dialog.ShowDialog(this);
        }



        private void toolStripButtonNewModule_Click(object sender, EventArgs e)
        {
            OnFileNewGraphic();
            AutoSaveExcute();
        }

        private void toolStripButtonOpenModule_Click(object sender, EventArgs e)
        {
            MenuItemFileOpen();
        }

        private void toolStripButtonNewReport_Click(object sender, EventArgs e)
        {
            MenuItemFileNewReport();
            AutoSaveExcute();
        }

        private void toolStripButtonOpenReport_Click(object sender, EventArgs e)
        {
            MenuItemFileOpenReport();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            AutoSaveExcute();
        }

        private void AutoSaveExcute()
        {
            Form child = this.ActiveMdiChild;

            if (child != null)
            {
                if (child.GetType() == typeof(FormEditGraphicFrame))
                {
                    ((FormEditGraphicFrame)child).formChild.FileSave();
                }
                else if (child.GetType() == typeof(FormReportMainEdit))
                {
                    ((FormReportMainEdit)child).formChild.menuItemFileSave_Click();
                }
                else if (child.GetType() == typeof(FormNewScriptEditor))
                {
                    ((FormNewScriptEditor)child).FileSave();
                }
            }
        }

        private void toolStripButtonScriptEditor_Click(object sender, EventArgs e)
        {
            RunScriptAlways();
        }

        private void toolStripButtonTagEditor_Click(object sender, EventArgs e)
        {
            RunTagEditor();
        }

        private void toolStripButtonTileHorizontal_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void toolStripButtonTileVertical_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void toolStripButtonRunLocalMain_Click(object sender, EventArgs e)
        {
            MenuItemRunViewMain();
        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            //ClassHelp.ShowHelp(this, "Studio.chm", "StudioMain.htm", false);
            ClassHelp.ShowHelp(this, "Studio.chm", "", true);
        }

        private void toolStripButtonKeyScript_Click(object sender, EventArgs e)
        {
            FormScriptKey dialog = new FormScriptKey();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void toolStripButtonMenuScript_Click(object sender, EventArgs e)
        {
            FormScriptMenu dialog = new FormScriptMenu();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void toolStripButtonConfigWebServer_Click(object sender, EventArgs e)
        {
            ConfigWebServer();
        }

        private void menuItemFileNewGraphicFromMyTempleteFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            WebLibraryModFull weblibrary = weblibraryModFull;

            dialog.Filter = String.Format("Templete files (*.{0})|*.{0}", weblibrary.sLibExt);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                PreviewItemPublic item = new PreviewItemPublic();

                byte[] buffer = File.ReadAllBytes(dialog.FileName);
                WebLibraryUtil.HashBuffer(buffer);

                object obj = weblibrary.InsertSelection(item, buffer);

                SharedStudio.formMain.FileNewGraphicByTemplete((ObjectRoot)obj, Path.GetFileNameWithoutExtension(dialog.FileName));
            }
            
            FormInsertFromLibrary d = new FormInsertFromLibrary(weblibraryModFull);
        }

        private void menuItemViewAnimationEditor_Click(object sender, EventArgs e)
        {
            if (AniEditLib.AniEditOnStudio.formAniEditor == null)
            {
                AniEditLib.AniEditOnStudio.Run(this);
            }
            else
            {
                AniEditLib.AniEditOnStudio.Stop();
            }
        }

        private void menuItemConfigRealTimeTestGraph_Click(object sender, EventArgs e)
        {
            FormConfigRealTimeGraph.ConfigRealTimeTestGraph();
        }

        void SetStudioToPlatformInvironment()
        {
            TotalConfigProject.ReReadProjectPlatform();  

            bool flag_win = false, flag_ce = false;

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                flag_win = true;
            }
            else
            {
                if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
                    flag_ce = true;
                else
                    flag_win = true;
            }

            // PC 버전에서만 사용하는 메뉴
            reportToolStripMenuItem.Visible = flag_win;
            openReportToolStripMenuItem.Visible = flag_win;

            keyScriptToolStripMenuItem.Visible = flag_win;

            databaseConnectionToolStripMenuItem.Visible = flag_win;
            webServerToolStripMenuItem.Visible = flag_win;
            demandControlToolStripMenuItem.Visible = flag_win;
            milliDataToolStripMenuItem.Visible = flag_win;
            realTimeTestDataToolStripMenuItem.Visible = flag_win;
            sQLBindListToolStripMenuItem.Visible = flag_win;
            onOffListToolStripMenuItem.Visible = flag_win;

            toolStripButtonNewReport.Visible = flag_win;
            toolStripButtonOpenReport.Visible = flag_win;
            toolStripButtonKeyScript.Visible = flag_win;
            toolStripButtonConfigWebServer.Visible = flag_win;
            toolStripSeparator6.Visible = flag_win; // toolStripButtonConfigWebServer 아이콘이 사라지면 라인이 두개가 되어서 하나를 삭제한다.

            // CE 버전에서만 사용하는 메뉴
            deploymentToolStripMenuItem.Visible = flag_ce;

            ApiToolStripMenu.Visible = flag_win; //20250327 PSU 추가
            dBToolStripMenuItem.Visible = flag_win; //20251119 PSU 추가
            globalizationToolStripMenuItem.Visible = flag_win; //20251119 PSU 추가
        }

        private void menuItemFileProject_Click(object sender, EventArgs e)
        {
            ProjectSelect.FormSelectProject dialog = new ProjectSelect.FormSelectProject();

            dialog.bCallByStudio = true;
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (String.Compare(TotalConfig.sDirWorkProject, dialog.sSelectedProject, true) == 0)
                {
                    // 프로젝트는 바꾸지 않았어도 Platform만 바뀔수가 있다.
                    SetStudioToPlatformInvironment();
                    SetMainTitle(); // 20250327 PSU 추가.
                    return;    // same project
                }

                if (Tools.IsLangKorean())
                {
                    if (MessageBox.Show("현재 실행중인 모든 프로젝트 파일과 실행파일을 닫습니다.", "종료 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }
                else
                {
                    if (MessageBox.Show("All Project file and program will be closed.", "Project Close", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }

                if (!CloseAnotherProgram("WatchDog")) return;
                if (!CloseAnotherProgram("RunMain")) return;
                if (!CloseAnotherProgram("SMS")) return;
                if (!CloseAnotherProgram("OpcClient")) return;
                if (!CloseAnotherProgram("ExportServer")) return;
                if (!CloseAnotherProgram("LinePrinter")) return;
                if (!CloseAnotherProgram("NetServer")) return;
                if (!CloseAnotherProgram("NetClient")) return;
                if (!CloseAnotherProgram("SmsServiceServer")) return;

                if(!CloseAnotherProgram("LocalMain"))   return;
                if(!CloseAnotherProgram("PLC_SCAN"))    return;
                
                Form[] childForm = SharedStudio.formMain.MdiChildren;
                //Make sure to ask for saving the doc before exiting the app 

                for (int i = 0; i < childForm.Length; i++)
                    childForm[i].Close();

                if (SharedStudio.formMain.MdiChildren.Length != 0)
                {
                    return;
                }

                TotalConfig.SaveRegAutoBaseConfig("Work Project", null, "Directory", dialog.sSelectedProject);
                
                TotalConfig.sDirWorkProject = dialog.sSelectedProject;
                SetStudioToPlatformInvironment();

                TerminalClass.Init();
                SetMainTitle();
                SharedStudio.formSolution.ReLoad();
            }
        }

        public void ToTop()
        {
            this.Activate();
            //if (this.WindowState == FormWindowState.Minimized)
            //{
            this.WindowState = FormWindowState.Maximized;   // 항상 크게 하는 것이 좋겠다.
            //}
        }

        private void menuItemConfigDeployment_Click(object sender, EventArgs e)
        {
            if (!SaveAllDocuments()) return;

            FutureVersion.FormConfigCompactVersion dialog = new FutureVersion.FormConfigCompactVersion();
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
        }

        private void menuItemRemoteProjectEditor_Click(object sender, EventArgs e)
        {
            if (!SaveAllDocuments()) return;

            var dialog = new Studio.RemoteProjectEditor.FormRemoteProjectEditor();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigFixedSchedules_Click(object sender, EventArgs e)
        {
            FormConfigSchedule dialog = new FormConfigSchedule();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigAddtionalSchedules_Click(object sender, EventArgs e)
        {
            FormConfigScheduleAdditional dialog = new FormConfigScheduleAdditional();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigRecipe_Click(object sender, EventArgs e)
        {
            PublicStudioLocalMain.Recipe.FormConfigRecipe.ConfigRecipe();
        }

        private void menuItemConfigPreset_Click(object sender, EventArgs e)
        {
            PublicStudioLocalMain.Recipe.FormConfigPreset.ConfigPreset();
        }

        private void menuItemConfigScheduleModels_Click(object sender, EventArgs e)
        {
            FormConfigModel dialog = new FormConfigModel();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigWeeklySchedules_Click(object sender, EventArgs e)
        {
            FormConfigScheduleWeek dialog = new FormConfigScheduleWeek();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigCommunicationPort_Click(object sender, EventArgs e)
        {
            Communication.FormConfigAllPort dialog = new Studio.Communication.FormConfigAllPort();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemConfigCommunicationScanServer_Click(object sender, EventArgs e)
        {
            Communication.FormConfigScanServer dialog = new Studio.Communication.FormConfigScanServer();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        CheckFileChanged checkFileChanged = new CheckFileChanged();

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripHelpCheckUpdateHelp_Click(object sender, EventArgs e)
        {
            HelpLib.ClassHelp.DialogConfigHelp(this);
        }

        /*
        void MenuItemFileNewScript(string filename)
        {
            bool first_flag = (this.MdiChildren.Length == 0);
            Studio.Script.FormNewScriptEditor form = new Studio.Script.FormNewScriptEditor(filename, 1, this.statusBarMain, new Studio.Script.FormNewScriptEditor.CallBackOnMdiActivated(this.MdiActivate));
            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }*/

        private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            FormSelectNewClass dialog = new FormSelectNewClass();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string filename = dialog.sFileName;
            MenuItemFileNewScript(filename);*/
        }

        private void StudioMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (NextVersion.bScript11)
            {
                Script11.UnInit();
            }

            OpcUaIpcManager.Stop();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //20250204 PSU 추가
        private void JsonTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigJsonTemplate dialog = new FormConfigJsonTemplate();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }
        //20250204 PSU 추가
        private void httpHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigHttpHeader dialog = new FormConfigHttpHeader();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        //20250311 hsjeong 스케쥴디자이너 추가
        private void scheduleDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigScheduleDesigner dialog = new FormConfigScheduleDesigner();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void dBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigDataDB.ShowConfigurationUI();
        }


        private void globalConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigGlobalization dialog = new FormConfigGlobalization();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("en-US");
            this.Refresh();
        }

        private void koreanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ko-KR");
            this.Refresh();
        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("zh-CN");
            this.Refresh();
        }

        private void japaneseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ja-JP");
            this.Refresh();
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("ru-RU");
            this.Refresh();
        }

        private void vietnameseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageManager.Instance.SetLang("vi-VN");
            this.Refresh();
        }

        private void globalizationToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            string lang = CultureInfo.DefaultThreadCurrentUICulture.Name.ToLower();

            koreanToolStripMenuItem.Checked = lang.StartsWith("ko");
            chineseToolStripMenuItem.Checked = lang.StartsWith("zh");
            japaneseToolStripMenuItem.Checked = lang.StartsWith("ja");
            englishToolStripMenuItem.Checked = lang.StartsWith("en");
            russianToolStripMenuItem.Checked = lang.StartsWith("ru");
            vietnameseToolStripMenuItem.Checked = lang.StartsWith("vi");
        }

        private AutobaseRESTAPIMonitor.FormRestApiMonitor _formRestApiMonitor;
        private void ViewRestApiClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formRestApiMonitor == null || _formRestApiMonitor.IsDisposed)
            {
                _formRestApiMonitor = new AutobaseRESTAPIMonitor.FormRestApiMonitor();
                _formRestApiMonitor.Show(this);
            }
            else
            {
                _formRestApiMonitor.BringToFront();
                _formRestApiMonitor.Activate();
            }
        }

        private void rESTAPIMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ViewRestApiClientToolStripMenuItem_Click(sender, e);
        }
    }

    public class MessageFilter : System.Windows.Forms.IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x111: // WM_COMMAND
                    if (m.WParam == new IntPtr((int)EnumIdmPublic.IDM_PUBLIC_ACTIVE_DOCUMENT))
                    {
                        string argument = TotalConfig.LoadRegAutoBaseConfig("Execute", null, "ActiveDocument", "");
                        SharedStudio.formMain.OpenCalledDocument(argument);
                        SharedStudio.formMain.ToTop();
                    }
                    break;
            }

            return false;

        }

    }
}
