using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using NetTools;
using PublicStudioLocalMain.Recipe;

namespace LocalMain
{
	/// <summary>
	/// ISA-88 Runtime Recipe Monitoring MDI Form (Phase 5 전면 재설계)
	/// - 배치 제어: Start / Hold / Restart / Abort
	/// - Step 트리: 색상 코딩 실행 상태
	/// - Step 상세: Entry/Exit 조건 + Action 태그값 실시간 표시
	/// - 실행 로그 + 배치 이력 탭
	/// </summary>
	public class FormRecipe : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components = null;
		public static CatWindowRing ringRecipe = new CatWindowRing();

		#region Controls

		// ToolStrip
		private ToolStrip toolStrip1;
		private ToolStripButton btnStart;
		private ToolStripButton btnHold;
		private ToolStripButton btnRestart;
		private ToolStripButton btnAbort;
		private ToolStripSeparator sep1;
		private ToolStripLabel labelUnit;
		private ToolStripComboBox comboUnit;
		private ToolStripSeparator sep2;
		private ToolStripButton btnUpload;
		private ToolStripButton btnImportJson;
		private ToolStripButton btnExportJson;
		private ToolStripSeparator sep3;
		private ToolStripButton btnRefresh;
		private ToolStripSeparator sep4;
		private ToolStripButton btnApprove;
		private ToolStripButton btnObsolete;
		private ToolStripSeparator sep5;
		private ToolStripButton btnSaveDraft;
		private ToolStripSeparator sep6;
		private ToolStripButton btnNewRevision;
		private ToolStripButton btnRevisionHistory;
		private ToolStripLabel labelStatus;

		// Main split: recipe list | detail area
		private SplitContainer splitMain;
		private ListView listViewRecipes;
		private ColumnHeader colName;
		private ColumnHeader colMode;
		private ColumnHeader colRecStatus;
		private ColumnHeader colVersion;
		private ColumnHeader colDesc;
		private ColumnHeader colUpdated;

		// Right side: batch info banner
		private Panel panelBatchInfo;
		private Label lblBatchCaption;
		private Label lblBatchId;
		private Label lblBatchState;
		private Label lblBatchOperator;
		private Label lblBatchTime;

		// Recipe Edit Panel
		private Panel panelRecipeEdit;
		private Label lblEditName;
		private TextBox txtRecipeName;
		private Label lblEditDesc;
		private TextBox txtDescription;

		// Right body: step area (top) | logs (bottom)
		private SplitContainer splitRightBody;

		// Step area: tree (left) | detail (right)
		private SplitContainer splitStepArea;
		private TreeView treeViewSteps;

		// Step detail panel (scrollable)
		private Panel panelStepDetail;
		private GroupBox grpEntryCondition;
		private Label lblEntryInfo;
		private Label lblEntryResult;
		private GroupBox grpActions;
		private DataGridView gridActions;
		private DataGridViewTextBoxColumn colActTag;
		private DataGridViewTextBoxColumn colActSetValue;
		private DataGridViewTextBoxColumn colActCurValue;
		private DataGridViewTextBoxColumn colActType;
		private GroupBox grpExitCondition;
		private Label lblExitInfo;
		private Label lblExitResult;

		// Transitions section (replaces ExitCondition when transitions exist)
		private GroupBox grpTransitions;
		private DataGridView gridTransitions;
		private DataGridViewTextBoxColumn colTrPriority;
		private DataGridViewTextBoxColumn colTrType;
		private DataGridViewTextBoxColumn colTrExpression;
		private DataGridViewTextBoxColumn colTrTarget;
		private DataGridViewTextBoxColumn colTrTimeout;
		private DataGridViewTextBoxColumn colTrStatus;

		// Enhanced batch status
		private Label lblCurrentStep;
		private Label lblWaitingCondition;
		private Label lblTimeoutCountdown;

		// Log tabs
		private TabControl tabLogs;
		private TabPage tabExecLog;
		private TabPage tabBatchHistory;
		private DataGridView gridExecLog;
		private DataGridViewTextBoxColumn colLogTime;
		private DataGridViewTextBoxColumn colLogAction;
		private DataGridViewTextBoxColumn colLogStatus;
		private DataGridViewTextBoxColumn colLogStep;
		private DataGridViewTextBoxColumn colLogMessage;
		private DataGridView gridBatchHistory;
		private DataGridViewTextBoxColumn colBhBatchId;
		private DataGridViewTextBoxColumn colBhRecipe;
		private DataGridViewTextBoxColumn colBhStatus;
		private DataGridViewTextBoxColumn colBhResult;
		private DataGridViewTextBoxColumn colBhStart;
		private DataGridViewTextBoxColumn colBhEnd;

		private Timer timerPoll;

		// 로그 일자 필터
		private Panel panelExecLogFilter;
		private DateTimePicker dtpExecLogDate;
		private Button btnExecLogSearch;
		private Panel panelBatchFilter;
		private DateTimePicker dtpBatchDate;
		private Button btnBatchSearch;

		#endregion

		// State
		private RecipeData _selectedRecipe;
		private int _selectedRecipeId;               // 리스트 선택 시 즉시 저장 (async 로딩 전에도 사용 가능)
		private ArrayList _flatSteps;                // 실행 순서대로 펼친 step 목록
		private bool _logNeedsRefresh = true;
		private bool _batchHistoryLoaded;
		private ArrayList _allRecipes = new ArrayList();    // 필터용 마스터 목록

		// 레시피 필터
		private Panel panelRecipeSearch;
        private Label lblExecDate;
        private Label lblBatchDate;
        private TextBox txtRecipeSearch;
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);
		private const int EM_SETCUEBANNER = 0x1501;

		/// <summary>
		/// TreeNode.Tag에 저장하는 Step 매핑 정보
		/// </summary>
		class StepNodeTag
		{
			public RecipeStepData Step;
			public int FlatIndex;  // _flatSteps 내 인덱스
		}

		public FormRecipe()
		{
			InitializeComponent();
			PostInitializeComponent();
			ApplyLocalization();
			ApplyPermissions();
			LoadRecipeList();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ringRecipe.push(this);

			// VIEW 권한 체크
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
				!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_VIEW))
			{
				bool isKor = Tools.IsLangKorean();
				MessageBox.Show(
					isKor ? "레시피 열람 권한이 없습니다." : "No recipe view permission.",
					isKor ? "권한 오류" : "Permission Denied",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				this.Close();
				return;
			}

			UpdateButtonStates(BatchState.Idle);
			timerPoll.Start();
		}

		protected override void OnClosed(EventArgs e)
		{
			timerPoll.Stop();
			ringRecipe.pop(this);
			base.OnClosed(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (timerPoll != null) timerPoll.Dispose();
				if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnStart = new System.Windows.Forms.ToolStripButton();
            this.btnHold = new System.Windows.Forms.ToolStripButton();
            this.btnRestart = new System.Windows.Forms.ToolStripButton();
            this.btnAbort = new System.Windows.Forms.ToolStripButton();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.labelUnit = new System.Windows.Forms.ToolStripLabel();
            this.comboUnit = new System.Windows.Forms.ToolStripComboBox();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUpload = new System.Windows.Forms.ToolStripButton();
            this.btnImportJson = new System.Windows.Forms.ToolStripButton();
            this.btnExportJson = new System.Windows.Forms.ToolStripButton();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.sep4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnApprove = new System.Windows.Forms.ToolStripButton();
            this.btnObsolete = new System.Windows.Forms.ToolStripButton();
            this.sep5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSaveDraft = new System.Windows.Forms.ToolStripButton();
            this.sep6 = new System.Windows.Forms.ToolStripSeparator();
            this.btnNewRevision = new System.Windows.Forms.ToolStripButton();
            this.btnRevisionHistory = new System.Windows.Forms.ToolStripButton();
            this.labelStatus = new System.Windows.Forms.ToolStripLabel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.listViewRecipes = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRecStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUpdated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelRecipeSearch = new System.Windows.Forms.Panel();
            this.txtRecipeSearch = new System.Windows.Forms.TextBox();
            this.splitRightBody = new System.Windows.Forms.SplitContainer();
            this.splitStepArea = new System.Windows.Forms.SplitContainer();
            this.treeViewSteps = new System.Windows.Forms.TreeView();
            this.panelStepDetail = new System.Windows.Forms.Panel();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.gridActions = new System.Windows.Forms.DataGridView();
            this.colActTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActSetValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActCurValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpExitCondition = new System.Windows.Forms.GroupBox();
            this.lblExitInfo = new System.Windows.Forms.Label();
            this.lblExitResult = new System.Windows.Forms.Label();
            this.grpEntryCondition = new System.Windows.Forms.GroupBox();
            this.lblEntryInfo = new System.Windows.Forms.Label();
            this.lblEntryResult = new System.Windows.Forms.Label();
            this.tabLogs = new System.Windows.Forms.TabControl();
            this.tabExecLog = new System.Windows.Forms.TabPage();
            this.gridExecLog = new System.Windows.Forms.DataGridView();
            this.colLogTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelExecLogFilter = new System.Windows.Forms.Panel();
            this.lblExecDate = new System.Windows.Forms.Label();
            this.dtpExecLogDate = new System.Windows.Forms.DateTimePicker();
            this.btnExecLogSearch = new System.Windows.Forms.Button();
            this.tabBatchHistory = new System.Windows.Forms.TabPage();
            this.gridBatchHistory = new System.Windows.Forms.DataGridView();
            this.colBhBatchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBhRecipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBhStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBhResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBhStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBhEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelBatchFilter = new System.Windows.Forms.Panel();
            this.lblBatchDate = new System.Windows.Forms.Label();
            this.dtpBatchDate = new System.Windows.Forms.DateTimePicker();
            this.btnBatchSearch = new System.Windows.Forms.Button();
            this.panelBatchInfo = new System.Windows.Forms.Panel();
            this.lblBatchCaption = new System.Windows.Forms.Label();
            this.lblBatchId = new System.Windows.Forms.Label();
            this.lblBatchState = new System.Windows.Forms.Label();
            this.lblBatchOperator = new System.Windows.Forms.Label();
            this.lblBatchTime = new System.Windows.Forms.Label();
            this.panelRecipeEdit = new System.Windows.Forms.Panel();
            this.lblEditName = new System.Windows.Forms.Label();
            this.txtRecipeName = new System.Windows.Forms.TextBox();
            this.lblEditDesc = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.timerPoll = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelRecipeSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRightBody)).BeginInit();
            this.splitRightBody.Panel1.SuspendLayout();
            this.splitRightBody.Panel2.SuspendLayout();
            this.splitRightBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitStepArea)).BeginInit();
            this.splitStepArea.Panel1.SuspendLayout();
            this.splitStepArea.Panel2.SuspendLayout();
            this.splitStepArea.SuspendLayout();
            this.panelStepDetail.SuspendLayout();
            this.grpActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridActions)).BeginInit();
            this.grpExitCondition.SuspendLayout();
            this.grpEntryCondition.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.tabExecLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridExecLog)).BeginInit();
            this.panelExecLogFilter.SuspendLayout();
            this.tabBatchHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridBatchHistory)).BeginInit();
            this.panelBatchFilter.SuspendLayout();
            this.panelBatchInfo.SuspendLayout();
            this.panelRecipeEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.btnHold,
            this.btnRestart,
            this.btnAbort,
            this.sep1,
            this.labelUnit,
            this.comboUnit,
            this.sep2,
            this.btnUpload,
            this.btnImportJson,
            this.btnExportJson,
            this.sep3,
            this.btnRefresh,
            this.sep4,
            this.btnApprove,
            this.btnObsolete,
            this.sep5,
            this.btnSaveDraft,
            this.sep6,
            this.btnNewRevision,
            this.btnRevisionHistory,
            this.labelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(980, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // btnStart
            // 
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(36, 22);
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnHold
            // 
            this.btnHold.Name = "btnHold";
            this.btnHold.Size = new System.Drawing.Size(37, 22);
            this.btnHold.Text = "Hold";
            this.btnHold.Click += new System.EventHandler(this.btnHold_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(47, 22);
            this.btnRestart.Text = "Restart";
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(41, 22);
            this.btnAbort.Text = "Abort";
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Size = new System.Drawing.Size(6, 25);
            // 
            // labelUnit
            // 
            this.labelUnit.Name = "labelUnit";
            this.labelUnit.Size = new System.Drawing.Size(32, 22);
            this.labelUnit.Text = "Unit:";
            // 
            // comboUnit
            // 
            this.comboUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboUnit.Name = "comboUnit";
            this.comboUnit.Size = new System.Drawing.Size(121, 25);
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUpload
            // 
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(49, 22);
            this.btnUpload.Text = "Upload";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnImportJson
            // 
            this.btnImportJson.Name = "btnImportJson";
            this.btnImportJson.Size = new System.Drawing.Size(80, 22);
            this.btnImportJson.Text = "JSON Import";
            this.btnImportJson.Click += new System.EventHandler(this.btnImportJson_Click);
            // 
            // btnExportJson
            // 
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(78, 22);
            this.btnExportJson.Text = "JSON Export";
            this.btnExportJson.Click += new System.EventHandler(this.btnExportJson_Click);
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // sep4
            // 
            this.sep4.Name = "sep4";
            this.sep4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnApprove
            // 
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(56, 22);
            this.btnApprove.Text = "Approve";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // btnObsolete
            // 
            this.btnObsolete.Name = "btnObsolete";
            this.btnObsolete.Size = new System.Drawing.Size(58, 22);
            this.btnObsolete.Text = "Obsolete";
            this.btnObsolete.Click += new System.EventHandler(this.btnObsolete_Click);
            // 
            // sep5
            // 
            this.sep5.Name = "sep5";
            this.sep5.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSaveDraft
            // 
            this.btnSaveDraft.Name = "btnSaveDraft";
            this.btnSaveDraft.Size = new System.Drawing.Size(67, 22);
            this.btnSaveDraft.Text = "Save Draft";
            this.btnSaveDraft.Click += new System.EventHandler(this.btnSaveDraft_Click);
            // 
            // sep6
            // 
            this.sep6.Name = "sep6";
            this.sep6.Size = new System.Drawing.Size(6, 25);
            // 
            // btnNewRevision
            // 
            this.btnNewRevision.Name = "btnNewRevision";
            this.btnNewRevision.Size = new System.Drawing.Size(83, 22);
            this.btnNewRevision.Text = "New Revision";
            this.btnNewRevision.Click += new System.EventHandler(this.btnNewRevision_Click);
            // 
            // btnRevisionHistory
            // 
            this.btnRevisionHistory.Name = "btnRevisionHistory";
            this.btnRevisionHistory.Size = new System.Drawing.Size(49, 22);
            this.btnRevisionHistory.Text = "History";
            this.btnRevisionHistory.Click += new System.EventHandler(this.btnRevisionHistory_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 22);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 25);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.listViewRecipes);
            this.splitMain.Panel1.Controls.Add(this.panelRecipeSearch);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.splitRightBody);
            this.splitMain.Panel2.Controls.Add(this.panelBatchInfo);
            this.splitMain.Panel2.Controls.Add(this.panelRecipeEdit);
            this.splitMain.Size = new System.Drawing.Size(980, 595);
            this.splitMain.SplitterDistance = 394;
            this.splitMain.TabIndex = 0;
            // 
            // listViewRecipes
            // 
            this.listViewRecipes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colVersion,
            this.colMode,
            this.colRecStatus,
            this.colDesc,
            this.colUpdated});
            this.listViewRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRecipes.FullRowSelect = true;
            this.listViewRecipes.GridLines = true;
            this.listViewRecipes.HideSelection = false;
            this.listViewRecipes.Location = new System.Drawing.Point(0, 28);
            this.listViewRecipes.Name = "listViewRecipes";
            this.listViewRecipes.Size = new System.Drawing.Size(394, 567);
            this.listViewRecipes.TabIndex = 0;
            this.listViewRecipes.UseCompatibleStateImageBehavior = false;
            this.listViewRecipes.View = System.Windows.Forms.View.Details;
            this.listViewRecipes.SelectedIndexChanged += new System.EventHandler(this.listViewRecipes_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Width = 110;
            // 
            // colVersion
            // 
            this.colVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colVersion.Width = 35;
            // 
            // colMode
            // 
            this.colMode.Width = 55;
            // 
            // colDesc
            // 
            this.colDesc.Width = 90;
            // 
            // colUpdated
            // 
            this.colUpdated.Width = 120;
            // 
            // panelRecipeSearch
            // 
            this.panelRecipeSearch.Controls.Add(this.txtRecipeSearch);
            this.panelRecipeSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRecipeSearch.Location = new System.Drawing.Point(0, 0);
            this.panelRecipeSearch.Name = "panelRecipeSearch";
            this.panelRecipeSearch.Padding = new System.Windows.Forms.Padding(4, 4, 4, 2);
            this.panelRecipeSearch.Size = new System.Drawing.Size(394, 28);
            this.panelRecipeSearch.TabIndex = 1;
            // 
            // txtRecipeSearch
            // 
            this.txtRecipeSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRecipeSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecipeSearch.Location = new System.Drawing.Point(4, 4);
            this.txtRecipeSearch.Name = "txtRecipeSearch";
            this.txtRecipeSearch.Size = new System.Drawing.Size(386, 21);
            this.txtRecipeSearch.TabIndex = 0;
            this.txtRecipeSearch.TextChanged += new System.EventHandler(this.txtRecipeSearch_TextChanged);
            // 
            // splitRightBody
            // 
            this.splitRightBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRightBody.Location = new System.Drawing.Point(0, 115);
            this.splitRightBody.Name = "splitRightBody";
            this.splitRightBody.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitRightBody.Panel1
            // 
            this.splitRightBody.Panel1.Controls.Add(this.splitStepArea);
            // 
            // splitRightBody.Panel2
            // 
            this.splitRightBody.Panel2.Controls.Add(this.tabLogs);
            this.splitRightBody.Size = new System.Drawing.Size(582, 480);
            this.splitRightBody.SplitterDistance = 340;
            this.splitRightBody.TabIndex = 0;
            // 
            // splitStepArea
            // 
            this.splitStepArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitStepArea.Location = new System.Drawing.Point(0, 0);
            this.splitStepArea.Name = "splitStepArea";
            // 
            // splitStepArea.Panel1
            // 
            this.splitStepArea.Panel1.Controls.Add(this.treeViewSteps);
            // 
            // splitStepArea.Panel2
            // 
            this.splitStepArea.Panel2.Controls.Add(this.panelStepDetail);
            this.splitStepArea.Size = new System.Drawing.Size(582, 340);
            this.splitStepArea.SplitterDistance = 179;
            this.splitStepArea.TabIndex = 0;
            // 
            // treeViewSteps
            // 
            this.treeViewSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSteps.HideSelection = false;
            this.treeViewSteps.Location = new System.Drawing.Point(0, 0);
            this.treeViewSteps.Name = "treeViewSteps";
            this.treeViewSteps.Size = new System.Drawing.Size(179, 340);
            this.treeViewSteps.TabIndex = 0;
            this.treeViewSteps.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSteps_AfterSelect);
            // 
            // panelStepDetail
            // 
            this.panelStepDetail.AutoScroll = true;
            this.panelStepDetail.Controls.Add(this.grpActions);
            this.panelStepDetail.Controls.Add(this.grpExitCondition);
            this.panelStepDetail.Controls.Add(this.grpEntryCondition);
            this.panelStepDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStepDetail.Location = new System.Drawing.Point(0, 0);
            this.panelStepDetail.Name = "panelStepDetail";
            this.panelStepDetail.Size = new System.Drawing.Size(399, 340);
            this.panelStepDetail.TabIndex = 0;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.gridActions);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(0, 70);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(399, 200);
            this.grpActions.TabIndex = 0;
            this.grpActions.TabStop = false;
            // 
            // gridActions
            // 
            this.gridActions.AllowUserToAddRows = false;
            this.gridActions.AllowUserToDeleteRows = false;
            this.gridActions.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridActions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colActTag,
            this.colActSetValue,
            this.colActCurValue,
            this.colActType});
            this.gridActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridActions.Location = new System.Drawing.Point(3, 17);
            this.gridActions.Name = "gridActions";
            this.gridActions.RowHeadersVisible = false;
            this.gridActions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridActions.Size = new System.Drawing.Size(393, 180);
            this.gridActions.TabIndex = 0;
            // 
            // colActTag
            // 
            this.colActTag.Name = "colActTag";
            this.colActTag.ReadOnly = true;
            this.colActTag.Width = 140;
            // 
            // colActSetValue
            // 
            this.colActSetValue.Name = "colActSetValue";
            this.colActSetValue.Width = 90;
            // 
            // colActCurValue
            // 
            this.colActCurValue.Name = "colActCurValue";
            this.colActCurValue.ReadOnly = true;
            this.colActCurValue.Width = 90;
            // 
            // colActType
            // 
            this.colActType.Name = "colActType";
            this.colActType.ReadOnly = true;
            this.colActType.Width = 65;
            // 
            // grpExitCondition
            // 
            this.grpExitCondition.Controls.Add(this.lblExitInfo);
            this.grpExitCondition.Controls.Add(this.lblExitResult);
            this.grpExitCondition.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpExitCondition.Location = new System.Drawing.Point(0, 270);
            this.grpExitCondition.Name = "grpExitCondition";
            this.grpExitCondition.Size = new System.Drawing.Size(399, 70);
            this.grpExitCondition.TabIndex = 1;
            this.grpExitCondition.TabStop = false;
            // 
            // lblExitInfo
            // 
            this.lblExitInfo.AutoSize = true;
            this.lblExitInfo.Location = new System.Drawing.Point(8, 18);
            this.lblExitInfo.Name = "lblExitInfo";
            this.lblExitInfo.Size = new System.Drawing.Size(0, 12);
            this.lblExitInfo.TabIndex = 0;
            // 
            // lblExitResult
            // 
            this.lblExitResult.AutoSize = true;
            this.lblExitResult.Font = new System.Drawing.Font("굴림", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblExitResult.Location = new System.Drawing.Point(8, 38);
            this.lblExitResult.Name = "lblExitResult";
            this.lblExitResult.Size = new System.Drawing.Size(0, 12);
            this.lblExitResult.TabIndex = 1;
            // 
            // grpEntryCondition
            // 
            this.grpEntryCondition.Controls.Add(this.lblEntryInfo);
            this.grpEntryCondition.Controls.Add(this.lblEntryResult);
            this.grpEntryCondition.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEntryCondition.Location = new System.Drawing.Point(0, 0);
            this.grpEntryCondition.Name = "grpEntryCondition";
            this.grpEntryCondition.Size = new System.Drawing.Size(399, 70);
            this.grpEntryCondition.TabIndex = 2;
            this.grpEntryCondition.TabStop = false;
            // 
            // lblEntryInfo
            // 
            this.lblEntryInfo.AutoSize = true;
            this.lblEntryInfo.Location = new System.Drawing.Point(8, 18);
            this.lblEntryInfo.Name = "lblEntryInfo";
            this.lblEntryInfo.Size = new System.Drawing.Size(0, 12);
            this.lblEntryInfo.TabIndex = 0;
            // 
            // lblEntryResult
            // 
            this.lblEntryResult.AutoSize = true;
            this.lblEntryResult.Font = new System.Drawing.Font("굴림", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblEntryResult.Location = new System.Drawing.Point(8, 38);
            this.lblEntryResult.Name = "lblEntryResult";
            this.lblEntryResult.Size = new System.Drawing.Size(0, 12);
            this.lblEntryResult.TabIndex = 1;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.tabExecLog);
            this.tabLogs.Controls.Add(this.tabBatchHistory);
            this.tabLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabLogs.Location = new System.Drawing.Point(0, 0);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.SelectedIndex = 0;
            this.tabLogs.Size = new System.Drawing.Size(582, 136);
            this.tabLogs.TabIndex = 0;
            // 
            // tabExecLog
            // 
            this.tabExecLog.Controls.Add(this.gridExecLog);
            this.tabExecLog.Controls.Add(this.panelExecLogFilter);
            this.tabExecLog.Location = new System.Drawing.Point(4, 22);
            this.tabExecLog.Name = "tabExecLog";
            this.tabExecLog.Size = new System.Drawing.Size(574, 110);
            this.tabExecLog.TabIndex = 0;
            // 
            // gridExecLog
            // 
            this.gridExecLog.AllowUserToAddRows = false;
            this.gridExecLog.AllowUserToDeleteRows = false;
            this.gridExecLog.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridExecLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLogTime,
            this.colLogAction,
            this.colLogStatus,
            this.colLogStep,
            this.colLogMessage});
            this.gridExecLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridExecLog.Location = new System.Drawing.Point(0, 30);
            this.gridExecLog.Name = "gridExecLog";
            this.gridExecLog.ReadOnly = true;
            this.gridExecLog.RowHeadersVisible = false;
            this.gridExecLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridExecLog.Size = new System.Drawing.Size(574, 80);
            this.gridExecLog.TabIndex = 0;
            // 
            // colLogTime
            // 
            this.colLogTime.Name = "colLogTime";
            this.colLogTime.ReadOnly = true;
            this.colLogTime.Width = 130;
            // 
            // colLogAction
            // 
            this.colLogAction.Name = "colLogAction";
            this.colLogAction.ReadOnly = true;
            this.colLogAction.Width = 130;
            // 
            // colLogStatus
            // 
            this.colLogStatus.Name = "colLogStatus";
            this.colLogStatus.ReadOnly = true;
            this.colLogStatus.Width = 130;
            // 
            // colLogStep
            // 
            this.colLogStep.Name = "colLogStep";
            this.colLogStep.ReadOnly = true;
            this.colLogStep.Width = 50;
            // 
            // colLogMessage
            // 
            this.colLogMessage.Name = "colLogMessage";
            this.colLogMessage.ReadOnly = true;
            this.colLogMessage.Width = 300;
            // 
            // panelExecLogFilter
            // 
            this.panelExecLogFilter.Controls.Add(this.lblExecDate);
            this.panelExecLogFilter.Controls.Add(this.dtpExecLogDate);
            this.panelExecLogFilter.Controls.Add(this.btnExecLogSearch);
            this.panelExecLogFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelExecLogFilter.Location = new System.Drawing.Point(0, 0);
            this.panelExecLogFilter.Name = "panelExecLogFilter";
            this.panelExecLogFilter.Size = new System.Drawing.Size(574, 30);
            this.panelExecLogFilter.TabIndex = 1;
            // 
            // lblExecDate
            // 
            this.lblExecDate.AutoSize = true;
            this.lblExecDate.Location = new System.Drawing.Point(8, 6);
            this.lblExecDate.Name = "lblExecDate";
            this.lblExecDate.Size = new System.Drawing.Size(34, 12);
            this.lblExecDate.TabIndex = 0;
            this.lblExecDate.Text = "Date:";
            // 
            // dtpExecLogDate
            // 
            this.dtpExecLogDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExecLogDate.Location = new System.Drawing.Point(50, 3);
            this.dtpExecLogDate.Name = "dtpExecLogDate";
            this.dtpExecLogDate.Size = new System.Drawing.Size(120, 21);
            this.dtpExecLogDate.TabIndex = 1;
            this.dtpExecLogDate.Value = new System.DateTime(2026, 3, 8, 0, 0, 0, 0);
            // 
            // btnExecLogSearch
            // 
            this.btnExecLogSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecLogSearch.Location = new System.Drawing.Point(178, 2);
            this.btnExecLogSearch.Name = "btnExecLogSearch";
            this.btnExecLogSearch.Size = new System.Drawing.Size(60, 25);
            this.btnExecLogSearch.TabIndex = 2;
            this.btnExecLogSearch.Text = "Search";
            this.btnExecLogSearch.Click += new System.EventHandler(this.btnExecLogSearch_Click);
            // 
            // tabBatchHistory
            // 
            this.tabBatchHistory.Controls.Add(this.gridBatchHistory);
            this.tabBatchHistory.Controls.Add(this.panelBatchFilter);
            this.tabBatchHistory.Location = new System.Drawing.Point(4, 22);
            this.tabBatchHistory.Name = "tabBatchHistory";
            this.tabBatchHistory.Size = new System.Drawing.Size(574, 110);
            this.tabBatchHistory.TabIndex = 1;
            // 
            // gridBatchHistory
            // 
            this.gridBatchHistory.AllowUserToAddRows = false;
            this.gridBatchHistory.AllowUserToDeleteRows = false;
            this.gridBatchHistory.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridBatchHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBhBatchId,
            this.colBhRecipe,
            this.colBhStatus,
            this.colBhResult,
            this.colBhStart,
            this.colBhEnd});
            this.gridBatchHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridBatchHistory.Location = new System.Drawing.Point(0, 30);
            this.gridBatchHistory.Name = "gridBatchHistory";
            this.gridBatchHistory.ReadOnly = true;
            this.gridBatchHistory.RowHeadersVisible = false;
            this.gridBatchHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridBatchHistory.Size = new System.Drawing.Size(574, 80);
            this.gridBatchHistory.TabIndex = 0;
            // 
            // colBhBatchId
            // 
            this.colBhBatchId.Name = "colBhBatchId";
            this.colBhBatchId.ReadOnly = true;
            this.colBhBatchId.Width = 150;
            // 
            // colBhRecipe
            // 
            this.colBhRecipe.Name = "colBhRecipe";
            this.colBhRecipe.ReadOnly = true;
            this.colBhRecipe.Width = 120;
            // 
            // colBhStatus
            // 
            this.colBhStatus.Name = "colBhStatus";
            this.colBhStatus.ReadOnly = true;
            this.colBhStatus.Width = 70;
            // 
            // colBhResult
            // 
            this.colBhResult.Name = "colBhResult";
            this.colBhResult.ReadOnly = true;
            this.colBhResult.Width = 70;
            // 
            // colBhStart
            // 
            this.colBhStart.Name = "colBhStart";
            this.colBhStart.ReadOnly = true;
            this.colBhStart.Width = 130;
            // 
            // colBhEnd
            // 
            this.colBhEnd.Name = "colBhEnd";
            this.colBhEnd.ReadOnly = true;
            this.colBhEnd.Width = 130;
            // 
            // panelBatchFilter
            // 
            this.panelBatchFilter.Controls.Add(this.lblBatchDate);
            this.panelBatchFilter.Controls.Add(this.dtpBatchDate);
            this.panelBatchFilter.Controls.Add(this.btnBatchSearch);
            this.panelBatchFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBatchFilter.Location = new System.Drawing.Point(0, 0);
            this.panelBatchFilter.Name = "panelBatchFilter";
            this.panelBatchFilter.Size = new System.Drawing.Size(574, 30);
            this.panelBatchFilter.TabIndex = 1;
            // 
            // lblBatchDate
            // 
            this.lblBatchDate.AutoSize = true;
            this.lblBatchDate.Location = new System.Drawing.Point(8, 6);
            this.lblBatchDate.Name = "lblBatchDate";
            this.lblBatchDate.Size = new System.Drawing.Size(34, 12);
            this.lblBatchDate.TabIndex = 0;
            this.lblBatchDate.Text = "Date:";
            // 
            // dtpBatchDate
            // 
            this.dtpBatchDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBatchDate.Location = new System.Drawing.Point(50, 3);
            this.dtpBatchDate.Name = "dtpBatchDate";
            this.dtpBatchDate.Size = new System.Drawing.Size(120, 21);
            this.dtpBatchDate.TabIndex = 1;
            this.dtpBatchDate.Value = new System.DateTime(2026, 3, 8, 0, 0, 0, 0);
            // 
            // btnBatchSearch
            // 
            this.btnBatchSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBatchSearch.Location = new System.Drawing.Point(178, 2);
            this.btnBatchSearch.Name = "btnBatchSearch";
            this.btnBatchSearch.Size = new System.Drawing.Size(60, 25);
            this.btnBatchSearch.TabIndex = 2;
            this.btnBatchSearch.Text = "Search";
            this.btnBatchSearch.Click += new System.EventHandler(this.btnBatchSearch_Click);
            // 
            // panelBatchInfo
            // 
            this.panelBatchInfo.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelBatchInfo.Controls.Add(this.lblBatchCaption);
            this.panelBatchInfo.Controls.Add(this.lblBatchId);
            this.panelBatchInfo.Controls.Add(this.lblBatchState);
            this.panelBatchInfo.Controls.Add(this.lblBatchOperator);
            this.panelBatchInfo.Controls.Add(this.lblBatchTime);
            this.panelBatchInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBatchInfo.Location = new System.Drawing.Point(0, 55);
            this.panelBatchInfo.Name = "panelBatchInfo";
            this.panelBatchInfo.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.panelBatchInfo.Size = new System.Drawing.Size(582, 60);
            this.panelBatchInfo.TabIndex = 1;
            // 
            // lblBatchCaption
            // 
            this.lblBatchCaption.AutoSize = true;
            this.lblBatchCaption.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.lblBatchCaption.Location = new System.Drawing.Point(6, 4);
            this.lblBatchCaption.Name = "lblBatchCaption";
            this.lblBatchCaption.Size = new System.Drawing.Size(108, 12);
            this.lblBatchCaption.TabIndex = 0;
            this.lblBatchCaption.Text = "No active batch";
            // 
            // lblBatchId
            // 
            this.lblBatchId.AutoSize = true;
            this.lblBatchId.Location = new System.Drawing.Point(6, 22);
            this.lblBatchId.Name = "lblBatchId";
            this.lblBatchId.Size = new System.Drawing.Size(0, 12);
            this.lblBatchId.TabIndex = 1;
            // 
            // lblBatchState
            // 
            this.lblBatchState.AutoSize = true;
            this.lblBatchState.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.lblBatchState.Location = new System.Drawing.Point(280, 4);
            this.lblBatchState.Name = "lblBatchState";
            this.lblBatchState.Size = new System.Drawing.Size(0, 12);
            this.lblBatchState.TabIndex = 2;
            // 
            // lblBatchOperator
            // 
            this.lblBatchOperator.AutoSize = true;
            this.lblBatchOperator.Location = new System.Drawing.Point(280, 22);
            this.lblBatchOperator.Name = "lblBatchOperator";
            this.lblBatchOperator.Size = new System.Drawing.Size(0, 12);
            this.lblBatchOperator.TabIndex = 3;
            // 
            // lblBatchTime
            // 
            this.lblBatchTime.AutoSize = true;
            this.lblBatchTime.Location = new System.Drawing.Point(6, 40);
            this.lblBatchTime.Name = "lblBatchTime";
            this.lblBatchTime.Size = new System.Drawing.Size(0, 12);
            this.lblBatchTime.TabIndex = 4;
            // 
            // panelRecipeEdit
            // 
            this.panelRecipeEdit.Controls.Add(this.lblEditName);
            this.panelRecipeEdit.Controls.Add(this.txtRecipeName);
            this.panelRecipeEdit.Controls.Add(this.lblEditDesc);
            this.panelRecipeEdit.Controls.Add(this.txtDescription);
            this.panelRecipeEdit.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRecipeEdit.Location = new System.Drawing.Point(0, 0);
            this.panelRecipeEdit.Name = "panelRecipeEdit";
            this.panelRecipeEdit.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.panelRecipeEdit.Size = new System.Drawing.Size(582, 55);
            this.panelRecipeEdit.TabIndex = 2;
            // 
            // lblEditName
            // 
            this.lblEditName.AutoSize = true;
            this.lblEditName.Location = new System.Drawing.Point(6, 7);
            this.lblEditName.Name = "lblEditName";
            this.lblEditName.Size = new System.Drawing.Size(43, 12);
            this.lblEditName.TabIndex = 0;
            this.lblEditName.Text = "Name:";
            // 
            // txtRecipeName
            // 
            this.txtRecipeName.Location = new System.Drawing.Point(50, 4);
            this.txtRecipeName.Name = "txtRecipeName";
            this.txtRecipeName.ReadOnly = true;
            this.txtRecipeName.Size = new System.Drawing.Size(200, 21);
            this.txtRecipeName.TabIndex = 1;
            // 
            // lblEditDesc
            // 
            this.lblEditDesc.AutoSize = true;
            this.lblEditDesc.Location = new System.Drawing.Point(6, 31);
            this.lblEditDesc.Name = "lblEditDesc";
            this.lblEditDesc.Size = new System.Drawing.Size(38, 12);
            this.lblEditDesc.TabIndex = 2;
            this.lblEditDesc.Text = "Desc:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(50, 28);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(520, 21);
            this.txtDescription.TabIndex = 3;
            // 
            // timerPoll
            // 
            this.timerPoll.Interval = 1000;
            this.timerPoll.Tick += new System.EventHandler(this.timerPoll_Tick);
            // 
            // FormRecipe
            // 
            this.ClientSize = new System.Drawing.Size(980, 620);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormRecipe";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelRecipeSearch.ResumeLayout(false);
            this.panelRecipeSearch.PerformLayout();
            this.splitRightBody.Panel1.ResumeLayout(false);
            this.splitRightBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRightBody)).EndInit();
            this.splitRightBody.ResumeLayout(false);
            this.splitStepArea.Panel1.ResumeLayout(false);
            this.splitStepArea.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitStepArea)).EndInit();
            this.splitStepArea.ResumeLayout(false);
            this.panelStepDetail.ResumeLayout(false);
            this.grpActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridActions)).EndInit();
            this.grpExitCondition.ResumeLayout(false);
            this.grpExitCondition.PerformLayout();
            this.grpEntryCondition.ResumeLayout(false);
            this.grpEntryCondition.PerformLayout();
            this.tabLogs.ResumeLayout(false);
            this.tabExecLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridExecLog)).EndInit();
            this.panelExecLogFilter.ResumeLayout(false);
            this.panelExecLogFilter.PerformLayout();
            this.tabBatchHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridBatchHistory)).EndInit();
            this.panelBatchFilter.ResumeLayout(false);
            this.panelBatchFilter.PerformLayout();
            this.panelBatchInfo.ResumeLayout(false);
            this.panelBatchInfo.PerformLayout();
            this.panelRecipeEdit.ResumeLayout(false);
            this.panelRecipeEdit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		/// <summary>
		/// InitializeComponent 이후 추가 컨트롤 생성 (Designer가 파싱하지 못하는 코드)
		/// </summary>
		void PostInitializeComponent()
		{
			// === Transitions Grid (Step Detail 내부, grpExitCondition 대체 가능) ===
			grpTransitions = new GroupBox();
			grpTransitions.Text = "Transitions";
			grpTransitions.Dock = DockStyle.Bottom;
			grpTransitions.Height = 140;
			grpTransitions.Visible = false;

			gridTransitions = new DataGridView();
			gridTransitions.Dock = DockStyle.Fill;
			gridTransitions.AllowUserToAddRows = false;
			gridTransitions.AllowUserToDeleteRows = false;
			gridTransitions.ReadOnly = true;
			gridTransitions.RowHeadersVisible = false;
			gridTransitions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridTransitions.BackgroundColor = System.Drawing.SystemColors.Window;
			gridTransitions.BorderStyle = BorderStyle.None;
			gridTransitions.Font = new System.Drawing.Font("Segoe UI", 8.5F);
			gridTransitions.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

			colTrPriority = new DataGridViewTextBoxColumn { HeaderText = "Priority", Name = "TrPriority", Width = 80 };
			colTrType = new DataGridViewTextBoxColumn { HeaderText = "Type", Name = "TrType", Width = 65 };
			colTrExpression = new DataGridViewTextBoxColumn { HeaderText = "Expression", Name = "TrExpression", Width = 140 };
			colTrTarget = new DataGridViewTextBoxColumn { HeaderText = "Target", Name = "TrTarget", Width = 50 };
			colTrTimeout = new DataGridViewTextBoxColumn { HeaderText = "Timeout", Name = "TrTimeout", Width = 65 };
			colTrStatus = new DataGridViewTextBoxColumn { HeaderText = "Status", Name = "TrStatus", Width = 55 };

			gridTransitions.Columns.AddRange(new DataGridViewColumn[] {
				colTrPriority, colTrType, colTrExpression, colTrTarget, colTrTimeout, colTrStatus
			});

			grpTransitions.Controls.Add(gridTransitions);
			panelStepDetail.Controls.Add(grpTransitions);

			// 종료 조건(Exit Condition) → 전이 조건(Transitions)으로 완전 전환
			grpTransitions.Visible = true;
			grpExitCondition.Visible = false;

			// === Enhanced Batch Status Labels ===
			lblCurrentStep = new Label();
			lblCurrentStep.AutoSize = true;
			lblCurrentStep.Location = new System.Drawing.Point(6, 22);
			lblCurrentStep.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			lblCurrentStep.Text = "";

			lblWaitingCondition = new Label();
			lblWaitingCondition.AutoSize = true;
			lblWaitingCondition.Location = new System.Drawing.Point(6, 40);
			lblWaitingCondition.Font = new System.Drawing.Font("Segoe UI", 8.5F);
			lblWaitingCondition.ForeColor = System.Drawing.Color.DarkSlateGray;
			lblWaitingCondition.Text = "";

			lblTimeoutCountdown = new Label();
			lblTimeoutCountdown.AutoSize = true;
			lblTimeoutCountdown.Location = new System.Drawing.Point(400, 22);
			lblTimeoutCountdown.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			lblTimeoutCountdown.Text = "";

			panelBatchInfo.Controls.Add(lblCurrentStep);
			panelBatchInfo.Controls.Add(lblWaitingCondition);
			panelBatchInfo.Controls.Add(lblTimeoutCountdown);
		}

		#region Localization

		void ApplyLocalization()
		{
			bool isKor = Tools.IsLangKorean();
			this.Text = isKor ? "배치 레시피 편집기" : "Batch Recipe Editor";

			// 레시피 검색 placeholder
			if (txtRecipeSearch != null && txtRecipeSearch.IsHandleCreated)
				SendMessage(txtRecipeSearch.Handle, EM_SETCUEBANNER, IntPtr.Zero,
					isKor ? "레시피 검색..." : "Search recipes...");

			// ToolStrip buttons
			this.btnStart.Text = isKor ? "▶ 시작" : "▶ Start";
			this.btnStart.ToolTipText = isKor ? "배치 시작" : "Start batch";
			this.btnHold.Text = isKor ? "⏸ 일시정지" : "⏸ Hold";
			this.btnHold.ToolTipText = isKor ? "실행 중인 배치를 일시정지" : "Hold running batch";
			this.btnRestart.Text = isKor ? "↻ 재개" : "↻ Restart";
			this.btnRestart.ToolTipText = isKor ? "일시정지된 배치를 재개" : "Restart held batch";
			this.btnAbort.Text = isKor ? "✖ 중단" : "✖ Abort";
			this.btnAbort.ToolTipText = isKor ? "실행 중인 배치를 중단" : "Abort running batch";
			this.labelUnit.Text = "Unit:";
			this.btnUpload.Text = "Upload";
			this.btnUpload.ToolTipText = isKor ? "현재 태그값을 읽어 새 레시피 저장" : "Read current tag values and save as recipe";
			this.btnImportJson.Text = "JSON Import";
			this.btnImportJson.ToolTipText = isKor ? "JSON 파일에서 레시피 Import" : "Import recipes from JSON file";
			this.btnExportJson.Text = "JSON Export";
			this.btnExportJson.ToolTipText = isKor ? "선택한 레시피를 JSON 파일로 Export" : "Export selected recipe to JSON file";
			this.btnRefresh.Text = isKor ? "새로고침" : "Refresh";
			this.btnApprove.Text = isKor ? "✔ 승인" : "✔ Approve";
			this.btnApprove.ToolTipText = isKor ? "레시피를 승인 상태로 변경 (전자서명 필요)" : "Approve recipe (electronic signature required)";
			this.btnObsolete.Text = isKor ? "✘ 폐기" : "✘ Obsolete";
			this.btnObsolete.ToolTipText = isKor ? "레시피를 폐기 상태로 변경 (전자서명 필요)" : "Obsolete recipe (electronic signature required)";
			this.btnSaveDraft.Text = isKor ? "💾 저장" : "💾 Save";
			this.btnSaveDraft.ToolTipText = isKor ? "수정 내용을 DB에 저장" : "Save changes to DB";
			this.btnNewRevision.Text = isKor ? "📋 새 Revision" : "📋 New Revision";
			this.btnNewRevision.ToolTipText = isKor ? "승인된 레시피의 새 Revision 생성 (version+1, draft)" : "Create new revision from approved recipe (version+1, draft)";
			this.btnRevisionHistory.Text = isKor ? "📜 이력" : "📜 History";
			this.btnRevisionHistory.ToolTipText = isKor ? "레시피 변경 이력 보기" : "View revision history";

			// Recipe edit panel
			this.lblEditName.Text = isKor ? "제목:" : "Name:";
			this.lblEditDesc.Text = isKor ? "설명:" : "Desc:";

			// ListView columns
			this.colName.Text = isKor ? "레시피" : "Recipe";
			this.colVersion.Text = "Ver";
			this.colMode.Text = isKor ? "모드" : "Mode";
			this.colRecStatus.Text = isKor ? "상태" : "Status";
			this.colDesc.Text = isKor ? "설명" : "Desc";
			this.colUpdated.Text = isKor ? "수정일" : "Updated";

			// Batch info
			this.lblBatchCaption.Text = isKor ? "배치 없음" : "No active batch";

			// Step detail
			this.grpEntryCondition.Text = isKor ? "시작 조건 (Entry Condition)" : "Entry Condition";
			this.grpActions.Text = isKor ? "동작 (Actions)" : "Actions";
			this.grpTransitions.Text = isKor ? "전이 조건 (Transitions)" : "Transitions";

			// Transition grid columns
			this.colTrPriority.HeaderText = isKor ? "우선순위" : "Priority";
			this.colTrType.HeaderText = isKor ? "유형" : "Type";
			this.colTrExpression.HeaderText = isKor ? "조건식" : "Expression";
			this.colTrTarget.HeaderText = isKor ? "대상" : "Target";
			this.colTrTimeout.HeaderText = isKor ? "타임아웃" : "Timeout";
			this.colTrStatus.HeaderText = isKor ? "상태" : "Status";

			// Action grid columns
			this.colActTag.HeaderText = isKor ? "태그" : "Tag";
			this.colActSetValue.HeaderText = isKor ? "설정값" : "Set Value";
			this.colActCurValue.HeaderText = isKor ? "현재값" : "Current";
			this.colActType.HeaderText = isKor ? "타입" : "Type";

			// Execution log columns
			this.colLogTime.HeaderText = isKor ? "시간" : "Time";
			this.colLogAction.HeaderText = isKor ? "동작" : "Action";
			this.colLogStatus.HeaderText = isKor ? "상태" : "Status";
			this.colLogStep.HeaderText = "Step";
			this.colLogMessage.HeaderText = isKor ? "메시지" : "Message";

			// Batch history columns
			this.colBhBatchId.HeaderText = "Batch ID";
			this.colBhRecipe.HeaderText = isKor ? "레시피" : "Recipe";
			this.colBhStatus.HeaderText = isKor ? "상태" : "Status";
			this.colBhResult.HeaderText = isKor ? "결과" : "Result";
			this.colBhStart.HeaderText = isKor ? "시작" : "Start";
			this.colBhEnd.HeaderText = isKor ? "종료" : "End";

			// Log tabs
			this.tabExecLog.Text = isKor ? "실행 로그" : "Execution Log";
			this.tabBatchHistory.Text = isKor ? "배치 이력" : "Batch History";

			// Log date filter labels
			if (panelExecLogFilter != null && panelExecLogFilter.Controls.Count > 0)
			{
				var lblExec = panelExecLogFilter.Controls[0] as Label;
				if (lblExec != null) lblExec.Text = isKor ? "날짜:" : "Date:";
			}
			if (btnExecLogSearch != null)
				btnExecLogSearch.Text = isKor ? "조회" : "Search";
			if (panelBatchFilter != null && panelBatchFilter.Controls.Count > 0)
			{
				var lblBatch = panelBatchFilter.Controls[0] as Label;
				if (lblBatch != null) lblBatch.Text = isKor ? "날짜:" : "Date:";
			}
			if (btnBatchSearch != null)
				btnBatchSearch.Text = isKor ? "조회" : "Search";
		}

		#endregion

		#region Permissions

		void ApplyPermissions()
		{
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return;

			bool canExecute = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_EXECUTE);
			bool canModify = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_MODIFY);
			bool canBatchCtrl = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_BATCH_CONTROL);

			if (!canExecute)
			{
				btnStart.Enabled = false;
				btnUpload.Enabled = false;
			}

			// Hold/Restart/Abort는 BATCH_CONTROL 권한으로 제어
			if (!canBatchCtrl)
			{
				btnHold.Enabled = false;
				btnRestart.Enabled = false;
				btnAbort.Enabled = false;
			}

			if (!canModify)
			{
				btnImportJson.Enabled = false;
				btnExportJson.Enabled = false;
				btnApprove.Enabled = false;
				btnObsolete.Enabled = false;
				btnSaveDraft.Enabled = false;
				btnNewRevision.Enabled = false;
			}
		}

		#endregion

		#region Editability Rules

		/// <summary>
		/// 상태별 편집 가능 여부 적용
		/// draft: 제목/설명 수정 가능, Save Draft 활성, New Revision 비활성
		/// approved: 제목/설명 수정 금지, Save Draft 비활성, New Revision 활성
		/// obsolete: 모두 수정 금지
		/// </summary>
		void ApplyEditability()
		{
			if (_selectedRecipe == null)
			{
				txtRecipeName.ReadOnly = true;
				txtDescription.ReadOnly = true;
				btnSaveDraft.Enabled = false;
				btnNewRevision.Enabled = false;
				return;
			}

			bool canModify = true;
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
				canModify = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_MODIFY);

			string status = _selectedRecipe.status ?? "draft";

			if (status == "draft" && canModify)
			{
				// Standard mode: 제목 변경 불가 (ISA-88 — Recipe Identity 보존)
				txtRecipeName.ReadOnly = true;
				txtRecipeName.BackColor = SystemColors.Control;
				txtDescription.ReadOnly = false;
				txtDescription.BackColor = SystemColors.Window;
				btnSaveDraft.Enabled = true;
				btnNewRevision.Enabled = false;
				// gridActions Set Value 편집 가능
				colActSetValue.ReadOnly = false;
			}
			else if (status == "approved" && canModify)
			{
				txtRecipeName.ReadOnly = true;
				txtRecipeName.BackColor = SystemColors.Control;
				txtDescription.ReadOnly = true;
				txtDescription.BackColor = SystemColors.Control;
				btnSaveDraft.Enabled = false;
				btnNewRevision.Enabled = true;
				colActSetValue.ReadOnly = true;
			}
			else
			{
				// obsolete 또는 권한 없음
				txtRecipeName.ReadOnly = true;
				txtRecipeName.BackColor = SystemColors.Control;
				txtDescription.ReadOnly = true;
				txtDescription.BackColor = SystemColors.Control;
				btnSaveDraft.Enabled = false;
				btnNewRevision.Enabled = false;
				colActSetValue.ReadOnly = true;
			}
		}

		#endregion

		#region Recipe List

		async void LoadRecipeList()
		{
			bool isKor = Tools.IsLangKorean();
			listViewRecipes.Items.Clear();
			_allRecipes.Clear();
			_selectedRecipe = null;
			_selectedRecipeId = 0;
			_flatSteps = null;
			treeViewSteps.Nodes.Clear();
			comboUnit.Items.Clear();
			ClearStepDetail();
			ClearBatchInfo();

			try
			{
				ArrayList list = await RecipeManager.LoadRecipeListAsync();

				for (int i = 0; i < list.Count; i++)
				{
					var info = (RecipeInfo)list[i];
					// Preset(Quick)은 FormPresetEditor에서 관리 — Batch Recipe만 표시
					if (info.recipe_mode == "quick") continue;
					_allRecipes.Add(info);
				}

				ApplyRecipeFilter();
			}
			catch (Exception ex)
			{
				MessageDisplay.Show(
					(isKor ? "레시피 목록 로드 실패: " : "Failed to load recipe list: ") + ex.Message);
			}

			// placeholder 설정 (Handle 생성 후)
			if (txtRecipeSearch != null && txtRecipeSearch.IsHandleCreated)
				SendMessage(txtRecipeSearch.Handle, EM_SETCUEBANNER, IntPtr.Zero,
					isKor ? "레시피 검색..." : "Search recipes...");
		}

		void ApplyRecipeFilter()
		{
			string filter = txtRecipeSearch != null ? txtRecipeSearch.Text.Trim().ToLowerInvariant() : "";
			listViewRecipes.BeginUpdate();
			listViewRecipes.Items.Clear();

			for (int i = 0; i < _allRecipes.Count; i++)
			{
				var info = (RecipeInfo)_allRecipes[i];

				// 필터 적용: recipe_name, status, description 대상
				if (!string.IsNullOrEmpty(filter))
				{
					bool match = false;
					if (info.recipe_name != null && info.recipe_name.ToLowerInvariant().IndexOf(filter) >= 0)
						match = true;
					if (!match && info.status != null && info.status.ToLowerInvariant().IndexOf(filter) >= 0)
						match = true;
					if (!match && info.description != null && info.description.ToLowerInvariant().IndexOf(filter) >= 0)
						match = true;
					if (!match) continue;
				}

				ListViewItem lvi = new ListViewItem(info.recipe_name);
				lvi.SubItems.Add("v" + info.version.ToString());
				lvi.SubItems.Add("Standard");
				lvi.SubItems.Add(info.status);
				lvi.SubItems.Add(info.description);
				lvi.SubItems.Add(info.updated_at.ToString("yyyy-MM-dd HH:mm"));
				lvi.Tag = info;

				// 상태별 색상
				if (info.status == "draft")
					lvi.ForeColor = Color.Gray;
				else if (info.status == "obsolete")
					lvi.ForeColor = Color.IndianRed;

				listViewRecipes.Items.Add(lvi);
			}

			listViewRecipes.EndUpdate();
		}

		void txtRecipeSearch_TextChanged(object sender, EventArgs e)
		{
			ApplyRecipeFilter();
		}

		void listViewRecipes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewRecipes.SelectedItems.Count == 0) return;

			var info = (RecipeInfo)listViewRecipes.SelectedItems[0].Tag;
			_selectedRecipeId = info.recipe_id;
			_batchHistoryLoaded = false;
			_logNeedsRefresh = true;
			LoadRecipeDetail(info.recipe_id);
		}

		#endregion

		#region Recipe Detail & Tree

		async void LoadRecipeDetail(int recipeId)
		{
			bool isKor = Tools.IsLangKorean();
			treeViewSteps.Nodes.Clear();
			comboUnit.Items.Clear();
			ClearStepDetail();

			try
			{
				_selectedRecipe = await RecipeManager.LoadRecipeAsync(recipeId);
				if (_selectedRecipe == null) return;

				// 제목/설명 편집 필드 설정
				txtRecipeName.Text = _selectedRecipe.recipe_name;
				txtDescription.Text = _selectedRecipe.description ?? "";
				ApplyEditability();

				// Unit ComboBox
				comboUnit.Items.Add(isKor ? "(전체)" : "(All)");
				for (int u = 0; u < _selectedRecipe.units.Count; u++)
				{
					var unit = (RecipeUnitData)_selectedRecipe.units[u];
					comboUnit.Items.Add(unit.unit_name);
				}
				comboUnit.SelectedIndex = 0;

				BuildTree();
				UpdateBatchInfoFromContext();
			}
			catch (Exception ex)
			{
				MessageDisplay.Show(
					(isKor ? "레시피 상세 로드 실패: " : "Failed to load recipe detail: ") + ex.Message);
			}
		}

		/// <summary>
		/// Step 트리 빌드 + flatSteps 매핑 생성
		/// </summary>
		void BuildTree()
		{
			treeViewSteps.Nodes.Clear();
			_flatSteps = new ArrayList();
			if (_selectedRecipe == null) return;

			bool isKor = Tools.IsLangKorean();
			int flatIdx = 0;

			// Recipe 직속 Step
			if (_selectedRecipe.steps.Count > 0)
			{
				string directLabel = isKor ? "(직속 Steps)" : "(Direct Steps)";
				TreeNode directNode = new TreeNode(directLabel);
				directNode.Tag = "__direct__";   // Unit 필터링용
				directNode.ForeColor = Color.DarkSlateGray;

				for (int i = 0; i < _selectedRecipe.steps.Count; i++)
				{
					var step = (RecipeStepData)_selectedRecipe.steps[i];
					TreeNode stepNode = BuildStepNode(step, ref flatIdx);
					directNode.Nodes.Add(stepNode);
				}
				treeViewSteps.Nodes.Add(directNode);
			}

			// Unit 노드
			for (int u = 0; u < _selectedRecipe.units.Count; u++)
			{
				var unit = (RecipeUnitData)_selectedRecipe.units[u];
				string unitLabel = String.Format("Unit {0}: {1}", unit.unit_order, unit.unit_name);
				TreeNode unitNode = new TreeNode(unitLabel);
				unitNode.Tag = unit.unit_name;   // Unit 필터링용
				unitNode.ForeColor = Color.DarkSlateGray;

				for (int s = 0; s < unit.steps.Count; s++)
				{
					var step = (RecipeStepData)unit.steps[s];
					TreeNode stepNode = BuildStepNode(step, ref flatIdx);
					unitNode.Nodes.Add(stepNode);
				}
				treeViewSteps.Nodes.Add(unitNode);
			}

			treeViewSteps.ExpandAll();
		}

		/// <summary>
		/// Step TreeNode 생성 + Transition을 자식 노드로 추가
		/// </summary>
		TreeNode BuildStepNode(RecipeStepData step, ref int flatIdx)
		{
			string label = String.Format("[{0}] {1}", step.step_order, step.step_name);
			TreeNode stepNode = new TreeNode(label);
			stepNode.Tag = new StepNodeTag { Step = step, FlatIndex = flatIdx };
			_flatSteps.Add(step);
			flatIdx++;

			// Transition 자식 노드 추가
			var transitions = ParseDisplayTransitions(step);
			for (int t = 0; t < transitions.Count; t++)
			{
				var tr = transitions[t];
				string trLabel = FormatTransitionLabel(tr, step.step_order);
				TreeNode trNode = new TreeNode(trLabel);
				trNode.ForeColor = Color.DarkGray;
				trNode.Tag = tr;  // StepTransition 저장
				stepNode.Nodes.Add(trNode);
			}

			return stepNode;
		}

		/// <summary>
		/// transitions_json에서 StepTransition 목록 파싱 (UI 표시용)
		/// </summary>
		List<StepTransition> ParseDisplayTransitions(RecipeStepData step)
		{
			if (!string.IsNullOrEmpty(step.transitions_json))
			{
				try
				{
					var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StepTransition>>(step.transitions_json);
					if (list != null) return list;
				}
				catch { }
			}
			return new List<StepTransition>();
		}

		/// <summary>
		/// Transition 트리 노드 레이블 포맷
		/// </summary>
		string FormatTransitionLabel(StepTransition tr, int currentStepOrder)
		{
			string typeStr = tr.type.ToString();
			string exprPart = !string.IsNullOrEmpty(tr.expression) ? tr.expression : "(auto)";
			if (exprPart.Length > 40) exprPart = exprPart.Substring(0, 37) + "...";

			string targetPart = "";
			if (tr.type == TransitionType.Loop)
			{
				int target = tr.target_step_order >= 0 ? tr.target_step_order : currentStepOrder;
				targetPart = " \u2192 S" + target;
				if (tr.max_loop_count > 0) targetPart += " (max " + tr.max_loop_count + ")";
			}

			string timeoutPart = "";
			if (tr.timeout_ms > 0)
				timeoutPart = String.Format(" ({0}s timeout)", tr.timeout_ms / 1000);

			return String.Format("[{0}] {1}: {2}{3}{4}",
				tr.priority, typeStr, exprPart, targetPart, timeoutPart);
		}

		#endregion

		#region Step Detail Panel

		void treeViewSteps_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null || e.Node.Tag == null) return;
			var tag = e.Node.Tag as StepNodeTag;
			if (tag == null) return;

			ShowStepDetail(tag.Step);
		}

		void ShowStepDetail(RecipeStepData step)
		{
			if (step == null) { ClearStepDetail(); return; }
			bool isKor = Tools.IsLangKorean();

			// ---- Entry Condition ----
			if (step.entry_condition_type != "none" && !string.IsNullOrEmpty(step.entry_condition_tag))
			{
				string op = GetComparisonSymbol(step.entry_condition_type);
				lblEntryInfo.Text = String.Format("{0} {1} {2}", step.entry_condition_tag, op, step.entry_condition_value);
				string tagVal = ReadTagValue(step.entry_condition_tag);
				bool met = EvaluateCondition(tagVal, step.entry_condition_type, step.entry_condition_value);
				lblEntryResult.Text = met
					? (isKor ? "→ 조건 충족" : "→ Condition MET")
					: (isKor ? "→ 대기 중" : "→ Waiting");
				lblEntryResult.ForeColor = met ? Color.Green : Color.DarkOrange;

				if (step.entry_timeout_ms > 0)
					lblEntryInfo.Text += String.Format("  (Timeout: {0}s)", step.entry_timeout_ms / 1000);
			}
			else
			{
				lblEntryInfo.Text = isKor ? "(없음)" : "(None)";
				lblEntryResult.Text = "";
			}

			// ---- Actions ----
			gridActions.Rows.Clear();
			for (int i = 0; i < step.items.Count; i++)
			{
				var item = (RecipeItemData)step.items[i];
				int rowIdx = gridActions.Rows.Add();
				var row = gridActions.Rows[rowIdx];
				row.Cells[0].Value = item.tag_name;
				row.Cells[1].Value = item.set_value;
				row.Cells[2].Value = ReadTagValue(item.tag_name);
				row.Cells[3].Value = item.value_type;
			}

			// ---- Transitions (전이 조건) ----
			gridTransitions.Rows.Clear();
			var transitions = ParseDisplayTransitions(step);

			for (int t = 0; t < transitions.Count; t++)
			{
				var tr = transitions[t];
				int rowIdx = gridTransitions.Rows.Add();
				var row = gridTransitions.Rows[rowIdx];
				row.Cells["TrPriority"].Value = tr.priority;
				row.Cells["TrType"].Value = tr.type.ToString();
				row.Cells["TrExpression"].Value = tr.expression;

				if (tr.type == TransitionType.Loop)
				{
					int target = tr.target_step_order >= 0 ? tr.target_step_order : step.step_order;
					row.Cells["TrTarget"].Value = "S" + target;
				}

				row.Cells["TrTimeout"].Value = tr.timeout_ms > 0
					? (tr.timeout_ms / 1000) + "s" : "-";

				// 실시간 expression 평가
				if (!string.IsNullOrEmpty(tr.expression))
				{
					try
					{
						bool result = RecipeExpressionEvaluator.Evaluate(tr.expression);
						row.Cells["TrStatus"].Value = result
							? (isKor ? "충족" : "MET") : (isKor ? "대기" : "Wait");
						row.Cells["TrStatus"].Style.ForeColor = result ? Color.Green : Color.Gray;
					}
					catch
					{
						row.Cells["TrStatus"].Value = "Err";
						row.Cells["TrStatus"].Style.ForeColor = Color.Red;
					}
				}
			}
		}

		void ClearStepDetail()
		{
			lblEntryInfo.Text = "";
			lblEntryResult.Text = "";
			gridActions.Rows.Clear();
			if (gridTransitions != null) gridTransitions.Rows.Clear();
		}

		/// <summary>
		/// 선택된 Step의 Action 태그 현재값을 갱신
		/// </summary>
		void RefreshStepDetailTagValues()
		{
			if (treeViewSteps.SelectedNode == null) return;
			var tag = treeViewSteps.SelectedNode.Tag as StepNodeTag;
			if (tag == null || tag.Step == null) return;

			// Action grid 현재값 갱신
			for (int i = 0; i < gridActions.Rows.Count && i < tag.Step.items.Count; i++)
			{
				var item = (RecipeItemData)tag.Step.items[i];
				gridActions.Rows[i].Cells[2].Value = ReadTagValue(item.tag_name);
			}

			// Entry/Exit 조건 상태 갱신
			ShowStepDetail(tag.Step);
		}

		#endregion

		#region Batch Control Buttons

		string GetSelectedUnitName()
		{
			if (comboUnit.SelectedIndex <= 0) return null;
			return comboUnit.SelectedItem?.ToString();
		}

		void btnStart_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string recipeName = _selectedRecipe.recipe_name;
			string unitName = GetSelectedUnitName();

			// 이미 실행 중 확인
			if (CheckEngineRecipe.IsExecuting(recipeName, unitName))
			{
				MessageBox.Show(
					isKor ? "해당 레시피가 이미 실행 중입니다." : "This recipe is already executing.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// ISA-88 배치 시작 — 전자서명 필수 (Unit 선택과 무관)
			string objName = recipeName;
			if (!string.IsNullOrEmpty(unitName)) objName += " [" + unitName + "]";
			var (sigOk, signature, reason) = FormElectronicSignature.ShowSignature(
				this, "batch_start", null, "RECIPE", objName, _selectedRecipe.version);
			if (!sigOk) return;

			StartBatch(recipeName, signature, reason, unitName);
		}

		async void StartBatch(string recipeName, string signature = null, string reason = null, string unitName = null)
		{
			bool isKor = Tools.IsLangKorean();
			labelStatus.Text = isKor ? "배치 시작 중..." : "Starting batch...";
			_logNeedsRefresh = true;
			try
			{
				string username = null;
				try { username = SharedData.userInfo?.sUsername; } catch { }

				var result = await CheckEngineRecipe.BatchStart(recipeName, null, username, unitName);
				if (result.success)
				{
					labelStatus.Text = String.Format(isKor ? "배치 완료: {0}" : "Batch complete: {0}", result.batchId);
				}
				else
				{
					labelStatus.Text = String.Format(isKor ? "배치 종료: {0}" : "Batch ended: {0}", result.error);
				}

				// 배치 시작 감사 로그 (전자서명 포함)
				if (!string.IsNullOrEmpty(signature))
				{
					try
					{
						var db = DataPostgres.Instance;
						if (db != null)
						{
							_ = db.InsertRecipeAuditLogAsync(
								0, recipeName, "BATCH_START",
								"BATCH", result.batchId ?? "",
								"", result.success ? "completed" : "failed",
								username ?? "", Environment.MachineName,
								signature, reason);

							// operational.audit_log
							int ver = _selectedRecipe != null ? _selectedRecipe.version : 0;
							_ = db.InsertOperationalAuditLogAsync(
								username ?? "", "BATCH_START",
								"RECIPE", recipeName, ver,
								reason, result.success ? "success" : "fail",
								signature, Environment.MachineName);
						}
					}
					catch { }
				}

				_logNeedsRefresh = true;
				_batchHistoryLoaded = false;
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Batch Error: " + ex.Message;
			}
		}

		void btnHold_Click(object sender, EventArgs e)
		{
			if (_selectedRecipe == null) return;
			bool isKor = Tools.IsLangKorean();

			bool ok = CheckEngineRecipe.HoldBatch(_selectedRecipe.recipe_name);
			labelStatus.Text = ok
				? (isKor ? "Hold 요청됨" : "Hold requested")
				: (isKor ? "Hold 불가" : "Hold not available");
		}

		void btnRestart_Click(object sender, EventArgs e)
		{
			if (_selectedRecipe == null) return;
			bool isKor = Tools.IsLangKorean();

			bool ok = CheckEngineRecipe.RestartBatch(_selectedRecipe.recipe_name);
			labelStatus.Text = ok
				? (isKor ? "Restart 요청됨" : "Restart requested")
				: (isKor ? "Restart 불가" : "Restart not available");
		}

		void btnAbort_Click(object sender, EventArgs e)
		{
			if (_selectedRecipe == null) return;
			bool isKor = Tools.IsLangKorean();

			// 전자서명 필요
			var (sigOk, signature, reason) = FormElectronicSignature.ShowSignature(
				this, "batch_abort", null,
				"RECIPE", _selectedRecipe.recipe_name, _selectedRecipe.version);
			if (!sigOk) return;

			bool ok = CheckEngineRecipe.AbortBatch(_selectedRecipe.recipe_name);
			labelStatus.Text = ok
				? (isKor ? "Abort 요청됨" : "Abort requested")
				: (isKor ? "Abort 불가" : "Abort not available");

			// 감사 로그 (전자서명 포함)
			if (ok && !string.IsNullOrEmpty(signature))
			{
				try
				{
					string username = null;
					try { username = SharedData.userInfo?.sUsername; } catch { }

					var db = DataPostgres.Instance;
					if (db != null)
					{
						_ = db.InsertRecipeAuditLogAsync(
							0, _selectedRecipe.recipe_name, "BATCH_ABORT",
							"BATCH", _selectedRecipe.recipe_name,
							"running", "aborted",
							username ?? "", Environment.MachineName,
							signature, reason);

						// operational.audit_log
						_ = db.InsertOperationalAuditLogAsync(
							username ?? "", "BATCH_ABORT",
							"RECIPE", _selectedRecipe.recipe_name, _selectedRecipe.version,
							reason, "success",
							signature, Environment.MachineName);
					}
				}
				catch { }
			}

			_logNeedsRefresh = true;
		}

		async void btnApprove_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (_selectedRecipe.status == "approved")
			{
				MessageBox.Show(
					isKor ? "이미 승인된 레시피입니다." : "Recipe is already approved.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// 전자서명 필요
			var (sigOk, signature, reason) = FormElectronicSignature.ShowSignature(
				this, "approve", null,
				"RECIPE", _selectedRecipe.recipe_name, _selectedRecipe.version);
			if (!sigOk) return;

			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return;

				string username = null;
				try { username = SharedData.userInfo?.sUsername; } catch { }

				bool ok = await db.UpdateRecipeStatusAsync(_selectedRecipe.recipe_id, "approved", username);
				if (ok)
				{
					labelStatus.Text = isKor ? "승인 완료" : "Approved";
					_selectedRecipe.status = "approved";

					// 감사 로그
					_ = db.InsertRecipeAuditLogAsync(
						_selectedRecipe.recipe_id, _selectedRecipe.recipe_name, "APPROVE",
						"RECIPE", _selectedRecipe.recipe_name,
						"draft", "approved",
						username ?? "", Environment.MachineName,
						signature, reason);

					// operational.audit_log
					_ = db.InsertOperationalAuditLogAsync(
						username ?? "", "APPROVE",
						"RECIPE", _selectedRecipe.recipe_name, _selectedRecipe.version,
						reason, "success",
						signature, Environment.MachineName);

					LoadRecipeList();
				}
				else
				{
					labelStatus.Text = isKor ? "승인 실패" : "Approve failed";
				}
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Approve Error: " + ex.Message;
			}
		}

		async void btnObsolete_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (_selectedRecipe.status == "obsolete")
			{
				MessageBox.Show(
					isKor ? "이미 폐기된 레시피입니다." : "Recipe is already obsolete.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// 전자서명 필요
			var (sigOk, signature, reason) = FormElectronicSignature.ShowSignature(
				this, "obsolete", null,
				"RECIPE", _selectedRecipe.recipe_name, _selectedRecipe.version);
			if (!sigOk) return;

			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return;

				string username = null;
				try { username = SharedData.userInfo?.sUsername; } catch { }

				string prevStatus = _selectedRecipe.status;
				bool ok = await db.UpdateRecipeStatusAsync(_selectedRecipe.recipe_id, "obsolete", username);
				if (ok)
				{
					labelStatus.Text = isKor ? "폐기 완료" : "Obsoleted";
					_selectedRecipe.status = "obsolete";

					// 감사 로그
					_ = db.InsertRecipeAuditLogAsync(
						_selectedRecipe.recipe_id, _selectedRecipe.recipe_name, "OBSOLETE",
						"RECIPE", _selectedRecipe.recipe_name,
						prevStatus, "obsolete",
						username ?? "", Environment.MachineName,
						signature, reason);

					// operational.audit_log
					_ = db.InsertOperationalAuditLogAsync(
						username ?? "", "OBSOLETE",
						"RECIPE", _selectedRecipe.recipe_name, _selectedRecipe.version,
						reason, "success",
						signature, Environment.MachineName);

					LoadRecipeList();
				}
				else
				{
					labelStatus.Text = isKor ? "폐기 실패" : "Obsolete failed";
				}
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Obsolete Error: " + ex.Message;
			}
		}

		async void btnSaveDraft_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// approved 상태에서는 직접 수정 불가 → New Revision 사용
			if (_selectedRecipe.status == "approved")
			{
				MessageBox.Show(
					isKor ? "승인된 레시피는 직접 수정할 수 없습니다.\n'새 Revision' 버튼을 사용하세요."
						  : "Cannot modify approved recipe.\nUse 'New Revision' button.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// 실행 중 저장 불가
			if (CheckEngineRecipe.IsExecuting(_selectedRecipe.recipe_name, null))
			{
				MessageBox.Show(
					isKor ? "실행 중인 레시피는 수정할 수 없습니다." : "Cannot modify a running recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// 설명 반영 (Standard mode: 제목 변경 불가)
			_selectedRecipe.description = txtDescription.Text.Trim();

			// gridActions에서 수정된 set_value를 _selectedRecipe에 반영
			if (treeViewSteps.SelectedNode != null)
			{
				var tag = treeViewSteps.SelectedNode.Tag as StepNodeTag;
				if (tag != null && tag.Step != null)
				{
					for (int i = 0; i < gridActions.Rows.Count && i < tag.Step.items.Count; i++)
					{
						var item = (RecipeItemData)tag.Step.items[i];
						object cellVal = gridActions.Rows[i].Cells[1].Value;
						if (cellVal != null)
							item.set_value = cellVal.ToString();
					}
				}
			}

			// Draft 상태 유지 — version 자동 증가 X
			_selectedRecipe.status = "draft";

			try
			{
				var result = await RecipeManager.SaveRecipeAsync(_selectedRecipe);
				if (result.error == null)
				{
					labelStatus.Text = String.Format(
						isKor ? "저장 완료 (v{0})" : "Saved (v{0})",
						_selectedRecipe.version);
					LoadRecipeList();
				}
				else
				{
					labelStatus.Text = (isKor ? "저장 실패: " : "Save failed: ") + result.error;
				}
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Save Error: " + ex.Message;
			}
		}

		async void btnNewRevision_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (_selectedRecipe.status != "approved")
			{
				MessageBox.Show(
					isKor ? "승인된 레시피만 새 Revision을 생성할 수 있습니다." : "Only approved recipes can create new revisions.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			int oldVersion = _selectedRecipe.version;
			int newVersion = oldVersion + 1;

			if (MessageBox.Show(
				String.Format(
					isKor ? "'{0}' v{1} 기반으로 새 Revision(v{2}, draft)을 생성하시겠습니까?\n기존 v{1}(approved)은 그대로 보존됩니다."
						  : "Create new Revision (v{2}, draft) based on '{0}' v{1}?\nExisting v{1} (approved) will be preserved.",
					_selectedRecipe.recipe_name, oldVersion, newVersion),
				isKor ? "새 Revision" : "New Revision",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			// ISA-88: 기존 approved 보존, 깊은 복사로 새 레코드 INSERT
			RecipeData newRevision = _selectedRecipe.Clone();
			newRevision.recipe_id = 0;       // 0 → SaveRecipeAsync가 INSERT 실행
			newRevision.version = newVersion;
			newRevision.status = "draft";
			newRevision.approved_by = "";
			newRevision.approved_at = null;
			// recipe_guid는 동일하게 유지 (같은 Recipe 계보)

			// 하위 unit/step/item의 DB ID 초기화 (새 레코드로 INSERT되도록)
			for (int u = 0; u < newRevision.units.Count; u++)
			{
				var unit = (RecipeUnitData)newRevision.units[u];
				unit.unit_id = 0;
				for (int s = 0; s < unit.steps.Count; s++)
				{
					var step = (RecipeStepData)unit.steps[s];
					step.step_id = 0;
					for (int t = 0; t < step.items.Count; t++)
						((RecipeItemData)step.items[t]).item_id = 0;
				}
			}
			for (int s = 0; s < newRevision.steps.Count; s++)
			{
				var step = (RecipeStepData)newRevision.steps[s];
				step.step_id = 0;
				for (int t = 0; t < step.items.Count; t++)
					((RecipeItemData)step.items[t]).item_id = 0;
			}

			try
			{
				var result = await RecipeManager.SaveRecipeAsync(newRevision);
				if (result.error == null)
				{
					labelStatus.Text = String.Format(
						isKor ? "새 Revision 생성 (v{0}, draft) — 기존 v{1} 보존됨"
							  : "New revision created (v{0}, draft) — v{1} preserved",
						newVersion, oldVersion);

					// 감사 로그
					try
					{
						string username = null;
						try { username = SharedData.userInfo?.sUsername; } catch { }
						var db = DataPostgres.Instance;
						if (db != null)
						{
							_ = db.InsertRecipeAuditLogAsync(
								result.recipeId, newRevision.recipe_name, "NEW_REVISION",
								"RECIPE", newRevision.recipe_name,
								"v" + oldVersion + " (approved)", "v" + newVersion + " (draft)",
								username ?? "", Environment.MachineName);
						}
					}
					catch { }

					LoadRecipeList();
				}
				else
				{
					labelStatus.Text = (isKor ? "Revision 생성 실패: " : "Revision failed: ") + result.error;
				}
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Revision Error: " + ex.Message;
			}
		}

		async void btnRevisionHistory_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return;

				var logs = await db.GetRecipeAuditLogsAsync(_selectedRecipe.recipe_id, 200);
				ShowRevisionHistoryDialog(logs);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					(isKor ? "이력 조회 오류: " : "History error: ") + ex.Message,
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Revision History 다이얼로그 표시
		/// </summary>
		void ShowRevisionHistoryDialog(ArrayList logs)
		{
			bool isKor = Tools.IsLangKorean();

			Form dlg = new Form();
			dlg.Text = isKor ? "Revision 이력 — " + _selectedRecipe.recipe_name : "Revision History — " + _selectedRecipe.recipe_name;
			dlg.Size = new Size(750, 420);
			dlg.StartPosition = FormStartPosition.CenterParent;
			dlg.MinimizeBox = false;
			dlg.MaximizeBox = false;
			dlg.FormBorderStyle = FormBorderStyle.Sizable;

			DataGridView grid = new DataGridView();
			grid.Dock = DockStyle.Fill;
			grid.ReadOnly = true;
			grid.AllowUserToAddRows = false;
			grid.AllowUserToDeleteRows = false;
			grid.RowHeadersVisible = false;
			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			grid.BackgroundColor = SystemColors.Window;

			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = isKor ? "일시" : "Time",
				Name = "Time", Width = 140
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = isKor ? "작업" : "Action",
				Name = "Action", Width = 100
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = isKor ? "이전" : "Before",
				Name = "Before", Width = 100
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = isKor ? "이후" : "After",
				Name = "After", Width = 100
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = isKor ? "사용자" : "User",
				Name = "User", Width = 80
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				HeaderText = isKor ? "사유" : "Reason",
				Name = "Reason", Width = 180
			});

			for (int i = 0; i < logs.Count; i++)
			{
				var entry = (RecipeAuditLogEntry)logs[i];
				int rowIdx = grid.Rows.Add();
				var row = grid.Rows[rowIdx];
				row.Cells["Time"].Value = entry.created_at.ToString("yyyy-MM-dd HH:mm:ss");
				row.Cells["Action"].Value = entry.action;
				row.Cells["Before"].Value = entry.old_value;
				row.Cells["After"].Value = entry.new_value;
				row.Cells["User"].Value = entry.username;
				row.Cells["Reason"].Value = entry.reason;

				// 색상
				if (entry.action == "APPROVE")
					row.DefaultCellStyle.ForeColor = Color.Green;
				else if (entry.action == "OBSOLETE")
					row.DefaultCellStyle.ForeColor = Color.IndianRed;
				else if (entry.action.Contains("ABORT") || entry.action.Contains("FAIL"))
					row.DefaultCellStyle.ForeColor = Color.Red;
			}

			dlg.Controls.Add(grid);
			dlg.ShowDialog(this);
		}

		void btnUpload_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "레시피를 선택하세요." : "Please select a recipe.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string recipeName = _selectedRecipe.recipe_name;
			string unitName = GetSelectedUnitName();

			if (CheckEngineRecipe.IsExecuting(recipeName, unitName))
			{
				MessageBox.Show(
					isKor ? "해당 레시피가 실행 중입니다." : "This recipe is executing.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string target = recipeName;
			if (unitName != null) target += " [" + unitName + "]";

			if (MessageBox.Show(
				String.Format(isKor ? "'{0}'의 현재 태그값으로 새 레시피를 저장하시겠습니까?" : "Save current tag values of '{0}' as new recipe?", target),
				isKor ? "확인" : "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			StartUpload(recipeName, unitName);
		}

		async void StartUpload(string recipeName, string unitName)
		{
			bool isKor = Tools.IsLangKorean();
			labelStatus.Text = isKor ? "Upload 실행 중..." : "Upload running...";
			try
			{
				var result = await CheckEngineRecipe.RecipeUpload(recipeName, unitName);
				if (result.success)
				{
					labelStatus.Text = "Upload " + (isKor ? "완료" : "Complete");
					LoadRecipeList();
				}
				else
				{
					labelStatus.Text = "Upload " + (isKor ? "실패: " : "Failed: ") + result.error;
				}
			}
			catch (Exception ex)
			{
				labelStatus.Text = "Upload Error: " + ex.Message;
			}
		}

		void btnImportJson_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
				!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_MODIFY))
			{
				MessageBox.Show(
					isKor ? "레시피 수정 권한이 없습니다." : "No recipe modify permission.",
					isKor ? "권한 오류" : "Permission Denied",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			FormRecipeImport importForm = new FormRecipeImport();
			importForm.LoadAndCompare();

			if (importForm.DialogResult == DialogResult.OK)
			{
				LoadRecipeList();
			}
		}

		async void btnExportJson_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();

			// Standard 모드: MODIFY 권한 필요
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
				!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_MODIFY))
			{
				MessageBox.Show(
					isKor ? "레시피 내보내기 권한이 없습니다." : "No recipe export permission.",
					isKor ? "권한 오류" : "Permission Denied",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (_selectedRecipe == null)
			{
				MessageBox.Show(
					isKor ? "내보낼 레시피를 선택하세요." : "Please select a recipe to export.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			using (var sfd = new SaveFileDialog())
			{
				sfd.Filter = "JSON Files (*.json)|*.json";
				sfd.FileName = _selectedRecipe.recipe_name + "_v" + _selectedRecipe.version + ".json";
				if (sfd.ShowDialog() != DialogResult.OK) return;

				try
				{
					// 전체 레시피 데이터 로드
					RecipeData fullRecipe = await RecipeManager.LoadRecipeAsync(_selectedRecipe.recipe_id);
					if (fullRecipe == null)
					{
						labelStatus.Text = isKor ? "레시피 로드 실패" : "Failed to load recipe";
						return;
					}

					var list = new System.Collections.Generic.List<RecipeData> { fullRecipe };
					string exportError;
					bool ok = RecipeJsonHelper.SaveToJsonFile(list, sfd.FileName, out exportError);
					if (!ok)
					{
						labelStatus.Text = (isKor ? "JSON Export 실패: " : "JSON Export failed: ") + exportError;
						return;
					}

					labelStatus.Text = String.Format(
						isKor ? "JSON Export 완료: {0}" : "JSON Export done: {0}",
						System.IO.Path.GetFileName(sfd.FileName));
				}
				catch (Exception ex)
				{
					MessageBox.Show(
						(isKor ? "JSON Export 실패: " : "JSON Export failed: ") + ex.Message,
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		void btnRefresh_Click(object sender, EventArgs e)
		{
			LoadRecipeList();
			labelStatus.Text = "";
		}

		#endregion

		#region Timer Polling & State Updates

		void timerPoll_Tick(object sender, EventArgs e)
		{
			try
			{
				if (_selectedRecipe == null) return;

				string recipeName = _selectedRecipe.recipe_name;

				// 1. 현재 batch 상태 가져오기
				string stateStr = CheckEngineRecipe.GetBatchState(recipeName);
				BatchState state;
				try { state = (BatchState)Enum.Parse(typeof(BatchState), stateStr, true); }
				catch { state = BatchState.Idle; }

				// 2. 버튼 상태 업데이트
				UpdateButtonStates(state);

				// 3. Batch 정보 업데이트
				UpdateBatchInfoFromContext();

				// 4. Tree 색상 업데이트
				UpdateTreeColors(recipeName);

				// 5. Step detail 태그값 갱신 (실행 중일 때만)
				if (state == BatchState.Running || state == BatchState.Holding || state == BatchState.Held)
				{
					RefreshStepDetailTagValues();
				}

				// 6. 실행 로그 갱신 (변경 시에만)
				if (_logNeedsRefresh && tabLogs.SelectedTab == tabExecLog)
				{
					LoadExecutionLogs(recipeName);
					_logNeedsRefresh = false;
				}

				// 7. 배치 이력 로드 (최초 1회)
				if (!_batchHistoryLoaded && tabLogs.SelectedTab == tabBatchHistory)
				{
					LoadBatchHistory();
					_batchHistoryLoaded = true;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("timerPoll error: " + ex.Message);
			}
		}

		void UpdateBatchInfoFromContext()
		{
			bool isKor = Tools.IsLangKorean();
			if (_selectedRecipe == null) { ClearBatchInfo(); return; }

			var execs = CheckEngineRecipe.GetAllExecutions();
			CheckEngineRecipe.RecipeExecutionContext activeCtx = null;

			if (execs != null)
			{
				for (int i = 0; i < execs.Length; i++)
				{
					if (execs[i].RecipeName == _selectedRecipe.recipe_name)
					{
						activeCtx = execs[i];
						break;
					}
				}
			}

			if (activeCtx == null)
			{
				ClearBatchInfo();
				return;
			}

			lblBatchCaption.Text = String.Format("{0}  Ver.{1}",
				activeCtx.RecipeName, _selectedRecipe.version);

			if (!string.IsNullOrEmpty(activeCtx.BatchId))
				lblBatchId.Text = "Batch: " + activeCtx.BatchId;
			else
				lblBatchId.Text = isKor ? "(단순 실행)" : "(Simple execution)";

			// 상태 표시 with 색상
			string stateStr = "IDLE";
			Color stateColor = Color.Gray;
			if (activeCtx.StateMachine != null)
			{
				stateStr = activeCtx.StateMachine.CurrentState.ToString().ToUpper();
				stateColor = GetStateColor(activeCtx.StateMachine.CurrentState);
			}
			else if (activeCtx.IsExecuting)
			{
				stateStr = "RUNNING";
				stateColor = Color.Blue;
			}
			lblBatchState.Text = stateStr;
			lblBatchState.ForeColor = stateColor;

			lblBatchOperator.Text = !string.IsNullOrEmpty(activeCtx.Username)
				? activeCtx.Username : "";
			lblBatchTime.Text = String.Format("{0}: {1}  |  Step: {2}/{3}",
				isKor ? "시작" : "Start",
				activeCtx.StartTime.ToString("HH:mm:ss"),
				activeCtx.CurrentStepIndex + 1,
				activeCtx.TotalSteps);

			// === Enhanced batch status labels ===

			// Current Step
			if (_flatSteps != null && activeCtx.CurrentStepIndex >= 0
				&& activeCtx.CurrentStepIndex < _flatSteps.Count)
			{
				var curStep = (RecipeStepData)_flatSteps[activeCtx.CurrentStepIndex];
				lblCurrentStep.Text = String.Format("{0}: [{1}] {2}",
					isKor ? "현재 Step" : "Current Step",
					curStep.step_order, curStep.step_name);

				// Waiting condition — first Complete transition's expression
				var transitions = ParseDisplayTransitions(curStep);
				string waitExpr = "";
				for (int t = 0; t < transitions.Count; t++)
				{
					if (transitions[t].type == TransitionType.Complete
						&& !string.IsNullOrEmpty(transitions[t].expression))
					{
						waitExpr = transitions[t].expression;
						break;
					}
				}
				if (string.IsNullOrEmpty(waitExpr) && !string.IsNullOrEmpty(curStep.exit_expression))
					waitExpr = curStep.exit_expression;

				lblWaitingCondition.Text = !string.IsNullOrEmpty(waitExpr)
					? String.Format("{0}: {1}", isKor ? "대기" : "Waiting", waitExpr)
					: "";

				// Timeout countdown
				int stepTimeout = curStep.timeout_ms;
				if (stepTimeout > 0 && activeCtx.CurrentStepStartTime != DateTime.MinValue)
				{
					double elapsedMs = (DateTime.Now - activeCtx.CurrentStepStartTime).TotalMilliseconds;
					int remainMs = stepTimeout - (int)elapsedMs;
					if (remainMs < 0) remainMs = 0;
					int remainSec = remainMs / 1000;
					lblTimeoutCountdown.Text = String.Format("Timeout: {0}s", remainSec);
					lblTimeoutCountdown.ForeColor = remainSec < 10 ? Color.Red : Color.Black;
				}
				else
				{
					lblTimeoutCountdown.Text = "";
				}
			}
			else
			{
				lblCurrentStep.Text = "";
				lblWaitingCondition.Text = "";
				lblTimeoutCountdown.Text = "";
			}
		}

		void ClearBatchInfo()
		{
			bool isKor = Tools.IsLangKorean();
			lblBatchCaption.Text = isKor ? "배치 없음" : "No active batch";
			lblBatchId.Text = "";
			lblBatchState.Text = "";
			lblBatchOperator.Text = "";
			lblBatchTime.Text = "";
			txtRecipeName.Text = "";
			txtDescription.Text = "";
			lblCurrentStep.Text = "";
			lblWaitingCondition.Text = "";
			lblTimeoutCountdown.Text = "";
		}

		/// <summary>
		/// 트리 노드 색상 업데이트 (실행 상태 기반)
		/// </summary>
		void UpdateTreeColors(string recipeName)
		{
			var execs = CheckEngineRecipe.GetAllExecutions();
			CheckEngineRecipe.RecipeExecutionContext ctx = null;

			if (execs != null)
			{
				for (int i = 0; i < execs.Length; i++)
				{
					if (execs[i].RecipeName == recipeName)
					{
						ctx = execs[i];
						break;
					}
				}
			}

			BatchState batchState = BatchState.Idle;
			int curStep = -1;
			if (ctx != null)
			{
				curStep = ctx.CurrentStepIndex;
				if (ctx.StateMachine != null)
					batchState = ctx.StateMachine.CurrentState;
				else if (ctx.IsExecuting)
					batchState = BatchState.Running;
			}

			bool isRunning = (batchState != BatchState.Idle && batchState != BatchState.Complete
				&& batchState != BatchState.Aborted);

			// Step-level 상태 사용 가능 여부
			bool hasStepStates = ctx?.StepStates != null && ctx.StepStates.Count > 0;

			// Unit 전용 실행 여부
			string execUnitName = ctx?.UnitName;

			// 모든 step node 순회
			foreach (TreeNode parentNode in treeViewSteps.Nodes)
			{
				// Unit 전용 실행 시: 해당 Unit이 아닌 노드는 색상 변경 안 함
				bool isTargetGroup = true;
				if (!string.IsNullOrEmpty(execUnitName))
				{
					string parentTag = parentNode.Tag as string;
					if (parentTag == "__direct__")
						isTargetGroup = false;  // 직속 Step 건너뛰기
					else if (parentTag != null && !string.Equals(parentTag, execUnitName, StringComparison.OrdinalIgnoreCase))
						isTargetGroup = false;  // 다른 Unit 건너뛰기
				}

				foreach (TreeNode stepNode in parentNode.Nodes)
				{
					var tag = stepNode.Tag as StepNodeTag;
					if (tag == null) continue;

					StepState ss = StepState.Pending;
					Color stepColor = Color.Black;

					// Unit 전용 실행 시: 대상 아닌 그룹은 기본 색상 유지
					if (!isTargetGroup)
					{
						string skipLabel = String.Format("[{0}] {1}", tag.Step.step_order, tag.Step.step_name);
						stepNode.Text = "\u25CB " + skipLabel;  // ○
						stepNode.ForeColor = Color.Black;
						continue;
					}

					// Step-level 상태 머신 사용 (C-6)
					if (hasStepStates)
					{
						if (ctx.StepStates.TryGetValue(tag.Step.step_order, out ss))
							stepColor = GetStepStateColor(ss, batchState);
					}
					else if (!isRunning && ctx == null)
					{
						stepColor = Color.Black;
					}
					else if (!isRunning && ctx != null)
					{
						if (tag.FlatIndex < curStep)
						{ stepColor = Color.Green; ss = StepState.Completed; }
						else if (tag.FlatIndex == curStep)
						{
							stepColor = batchState == BatchState.Aborted ? Color.Red : Color.Green;
							ss = batchState == BatchState.Aborted ? StepState.Aborted : StepState.Completed;
						}
						else
						{ stepColor = Color.Gray; }
					}
					else
					{
						if (tag.FlatIndex < curStep)
						{ stepColor = Color.Green; ss = StepState.Completed; }
						else if (tag.FlatIndex == curStep)
						{
							if (batchState == BatchState.Held || batchState == BatchState.Holding)
							{ stepColor = Color.Goldenrod; ss = StepState.Running; }
							else
							{ stepColor = Color.Blue; ss = StepState.Running; }
						}
						else
						{ stepColor = Color.Gray; }
					}

					// 상태 아이콘
					string icon;
					switch (ss)
					{
						case StepState.Completed: icon = "\u2714"; break;         // ✔
						case StepState.Running:
						case StepState.WaitingExit: icon = "\u25B6"; break;       // ▶
						case StepState.WaitingEntry: icon = "\u25CB"; break;      // ○
						case StepState.Exception:
						case StepState.Aborted: icon = "\u2716"; break;           // ✖
						default: icon = "\u25CB"; break;                           // ○
					}

					string baseLabel = String.Format("[{0}] {1}", tag.Step.step_order, tag.Step.step_name);
					stepNode.Text = icon + " " + baseLabel;
					stepNode.ForeColor = stepColor;
				}
			}
		}

		/// <summary>
		/// StepState에 따른 TreeNode 색상 반환 (ISA-88 Step 상태 시각화)
		/// </summary>
		Color GetStepStateColor(StepState state, BatchState batchState)
		{
			switch (state)
			{
				case StepState.WaitingEntry:
					return Color.Orange;
				case StepState.Running:
					if (batchState == BatchState.Held || batchState == BatchState.Holding)
						return Color.Goldenrod;
					return Color.LimeGreen;
				case StepState.WaitingExit:
					return Color.DodgerBlue;
				case StepState.Completed:
					return Color.Gray;
				case StepState.Aborted:
					return Color.Red;
				case StepState.Exception:
					return Color.DarkRed;
				default: // Pending
					return Color.Black;
			}
		}

		#endregion

		#region Button State Management

		/// <summary>
		/// ISA-88 상태에 따른 버튼 Enable/Disable
		/// </summary>
		void UpdateButtonStates(BatchState state)
		{
			// 권한 체크
			bool canExecute = true;
			bool canBatchCtrl = true;
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				canExecute = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_EXECUTE);
				canBatchCtrl = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_BATCH_CONTROL);
			}

			if (!canExecute)
			{
				btnStart.Enabled = false;
				btnHold.Enabled = false;
				btnRestart.Enabled = false;
				btnAbort.Enabled = false;
				return;
			}

			switch (state)
			{
				case BatchState.Idle:
					btnStart.Enabled = true;
					btnHold.Enabled = false;
					btnRestart.Enabled = false;
					btnAbort.Enabled = false;
					break;
				case BatchState.Running:
					btnStart.Enabled = false;
					btnHold.Enabled = canBatchCtrl;
					btnRestart.Enabled = false;
					btnAbort.Enabled = canBatchCtrl;
					break;
				case BatchState.Holding:
					btnStart.Enabled = false;
					btnHold.Enabled = false;
					btnRestart.Enabled = false;
					btnAbort.Enabled = canBatchCtrl;
					break;
				case BatchState.Held:
					btnStart.Enabled = false;
					btnHold.Enabled = false;
					btnRestart.Enabled = canBatchCtrl;
					btnAbort.Enabled = canBatchCtrl;
					break;
				case BatchState.Restarting:
					btnStart.Enabled = false;
					btnHold.Enabled = false;
					btnRestart.Enabled = false;
					btnAbort.Enabled = canBatchCtrl;
					break;
				case BatchState.Aborting:
					btnStart.Enabled = false;
					btnHold.Enabled = false;
					btnRestart.Enabled = false;
					btnAbort.Enabled = false;
					break;
				case BatchState.Aborted:
				case BatchState.Complete:
					btnStart.Enabled = true;
					btnHold.Enabled = false;
					btnRestart.Enabled = false;
					btnAbort.Enabled = false;
					break;
			}
		}

		#endregion

		#region Execution Log

		async void LoadExecutionLogs(string recipeName, DateTime? date = null)
		{
			try
			{
				var db = DataPostgres.Instance;
				if (db == null) return;

				ArrayList logs;
				if (date.HasValue)
				{
					DateTime from = date.Value.Date;
					DateTime to = from.AddDays(1);
					logs = await db.GetRecentExecutionLogsAsync(recipeName, from, to, 500);
				}
				else
				{
					DateTime from = DateTime.Today;
					DateTime to = from.AddDays(1);
					logs = await db.GetRecentExecutionLogsAsync(recipeName, from, to, 500);
				}

				gridExecLog.Rows.Clear();

				for (int i = 0; i < logs.Count; i++)
				{
					var entry = (RecipeExecutionLogEntry)logs[i];
					int rowIdx = gridExecLog.Rows.Add();
					var row = gridExecLog.Rows[rowIdx];
					row.Cells[0].Value = entry.created_at.ToString("yyyy-MM-dd HH:mm:ss");
					row.Cells[1].Value = entry.action;
					row.Cells[2].Value = entry.status;
					row.Cells[3].Value = entry.total_steps > 0
						? String.Format("{0}/{1}", entry.step_index, entry.total_steps) : "";
					row.Cells[4].Value = entry.error_message;

					// 상태별 행 색상
					if (entry.status == "FAILED")
						row.DefaultCellStyle.ForeColor = Color.Red;
					else if (entry.status == "COMPLETED")
						row.DefaultCellStyle.ForeColor = Color.Green;
					else if (entry.status == "ABORTED")
						row.DefaultCellStyle.ForeColor = Color.OrangeRed;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("LoadExecutionLogs error: " + ex.Message);
			}
		}

		async void LoadBatchHistory(DateTime? date = null)
		{
			try
			{
				if (_selectedRecipeId <= 0) return;

				DateTime d = date ?? DateTime.Today;
				DateTime from = d.Date;
				DateTime to = from.AddDays(1);
				var batches = await RecipeManager.GetRecentBatchesAsync(_selectedRecipeId, from, to, 500);
				gridBatchHistory.Rows.Clear();

				for (int i = 0; i < batches.Count; i++)
				{
					var rec = batches[i];
					int rowIdx = gridBatchHistory.Rows.Add();
					var row = gridBatchHistory.Rows[rowIdx];
					row.Cells[0].Value = rec.batch_id;
					row.Cells[1].Value = rec.master_recipe_name;
					row.Cells[2].Value = rec.status;
					row.Cells[3].Value = rec.result;
					row.Cells[4].Value = rec.start_time?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
					row.Cells[5].Value = rec.end_time?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

					if (rec.result == "failed" || rec.result == "aborted")
						row.DefaultCellStyle.ForeColor = Color.Red;
					else if (rec.result == "completed")
						row.DefaultCellStyle.ForeColor = Color.Green;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("LoadBatchHistory error: " + ex.Message);
			}
		}

		void btnExecLogSearch_Click(object sender, EventArgs e)
		{
			if (_selectedRecipe == null) return;
			LoadExecutionLogs(_selectedRecipe.recipe_name, dtpExecLogDate.Value);
		}

		void btnBatchSearch_Click(object sender, EventArgs e)
		{
			LoadBatchHistory(dtpBatchDate.Value);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// 태그 현재값 읽기 (TagLib 사용)
		/// </summary>
		string ReadTagValue(string tagName)
		{
			if (string.IsNullOrEmpty(tagName)) return "";
			try
			{
				if (TagLib.IsTagExist(tagName))
				{
					int[] tagPos = new int[1];
					TagPublicClass tp = TagLib.GetStructPublic(tagName, ref tagPos);
					object val = tp.GetCurr();
					if (val != null) return val.ToString();
				}
			}
			catch { }
			return "N/A";
		}

		/// <summary>
		/// 비교 연산자 기호 변환
		/// </summary>
		string GetComparisonSymbol(string condType)
		{
			switch (condType)
			{
				case "equal": return "==";
				case "greater": return ">";
				case "less": return "<";
				case "not_equal": return "!=";
				case "greater_equal": return ">=";
				case "less_equal": return "<=";
				default: return "?";
			}
		}

		/// <summary>
		/// 조건 평가 (문자열 비교 기반)
		/// </summary>
		bool EvaluateCondition(string currentValue, string condType, string targetValue)
		{
			if (currentValue == "N/A" || string.IsNullOrEmpty(currentValue)) return false;

			double cur, tgt;
			bool curNum = double.TryParse(currentValue, out cur);
			bool tgtNum = double.TryParse(targetValue, out tgt);

			if (curNum && tgtNum)
			{
				switch (condType)
				{
					case "equal": return Math.Abs(cur - tgt) < 0.001;
					case "greater": return cur > tgt;
					case "less": return cur < tgt;
					case "not_equal": return Math.Abs(cur - tgt) >= 0.001;
					case "greater_equal": return cur >= tgt;
					case "less_equal": return cur <= tgt;
				}
			}
			else
			{
				// 문자열 비교
				if (condType == "equal") return currentValue == targetValue;
				if (condType == "not_equal") return currentValue != targetValue;
			}
			return false;
		}

		/// <summary>
		/// BatchState에 대한 색상 반환
		/// </summary>
		Color GetStateColor(BatchState state)
		{
			switch (state)
			{
				case BatchState.Idle: return Color.Gray;
				case BatchState.Running: return Color.Blue;
				case BatchState.Holding:
				case BatchState.Held: return Color.Goldenrod;
				case BatchState.Restarting: return Color.DodgerBlue;
				case BatchState.Aborting: return Color.OrangeRed;
				case BatchState.Aborted: return Color.Red;
				case BatchState.Complete: return Color.Green;
				default: return Color.Black;
			}
		}

		#endregion
	}
}
