namespace LocalMain.PythonAi
{
    partial class FormPythonAiDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PythonAiLogBridge.Detach(AddLog);
                if (timerFast != null) { timerFast.Stop(); timerFast.Dispose(); }
                if (timerSlow != null) { timerSlow.Stop(); timerSlow.Dispose(); }
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // === Create all controls ===
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.panelHeader = new System.Windows.Forms.Panel();
            this.chkEnableAi = new System.Windows.Forms.CheckBox();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblStateCaption = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lblUptimeCaption = new System.Windows.Forms.Label();
            this.lblUptime = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPing = new System.Windows.Forms.Button();
            this.lblPidCaption = new System.Windows.Forms.Label();
            this.lblProcessId = new System.Windows.Forms.Label();
            this.lblFailCaption = new System.Windows.Forms.Label();
            this.lblFailureCount = new System.Windows.Forms.Label();

            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConnection = new System.Windows.Forms.TabPage();
            this.tabMetrics = new System.Windows.Forms.TabPage();
            this.tabModels = new System.Windows.Forms.TabPage();
            this.tabTest = new System.Windows.Forms.TabPage();
            this.tabLog = new System.Windows.Forms.TabPage();

            // --- Tab 0: Connection ---
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.groupBoxPaths = new System.Windows.Forms.GroupBox();
            this.lblPythonExe = new System.Windows.Forms.Label();
            this.txtPythonExe = new System.Windows.Forms.TextBox();
            this.btnBrowsePython = new System.Windows.Forms.Button();
            this.lblEngineScript = new System.Windows.Forms.Label();
            this.txtEngineScript = new System.Windows.Forms.TextBox();
            this.btnBrowseScript = new System.Windows.Forms.Button();
            this.lblConfigFile = new System.Windows.Forms.Label();
            this.txtConfigFile = new System.Windows.Forms.TextBox();
            this.btnBrowseConfig = new System.Windows.Forms.Button();
            this.groupBoxTimeouts = new System.Windows.Forms.GroupBox();
            this.lblRequestTimeout = new System.Windows.Forms.Label();
            this.numRequestTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblPingInterval = new System.Windows.Forms.Label();
            this.numPingInterval = new System.Windows.Forms.NumericUpDown();
            this.lblBatchTimeout = new System.Windows.Forms.Label();
            this.numBatchTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblTrainingTimeout = new System.Windows.Forms.Label();
            this.numTrainingTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblScriptTimeout = new System.Windows.Forms.Label();
            this.numScriptTimeout = new System.Windows.Forms.NumericUpDown();
            this.groupBoxLogging = new System.Windows.Forms.GroupBox();
            this.lblLogLevel = new System.Windows.Forms.Label();
            this.cmbLogLevel = new System.Windows.Forms.ComboBox();
            this.lblLogFormat = new System.Windows.Forms.Label();
            this.cmbLogFormat = new System.Windows.Forms.ComboBox();
            this.btnApplySettings = new System.Windows.Forms.Button();
            this.btnResetDefaults = new System.Windows.Forms.Button();

            // --- Tab 1: Metrics ---
            this.groupBoxServiceMetrics = new System.Windows.Forms.GroupBox();
            this.lvMetrics = new System.Windows.Forms.ListView();
            this.colService = new System.Windows.Forms.ColumnHeader();
            this.colTotal = new System.Windows.Forms.ColumnHeader();
            this.colSuccess = new System.Windows.Forms.ColumnHeader();
            this.colErrors = new System.Windows.Forms.ColumnHeader();
            this.colErrorRate = new System.Windows.Forms.ColumnHeader();
            this.colAvgMs = new System.Windows.Forms.ColumnHeader();
            this.colMinMs = new System.Windows.Forms.ColumnHeader();
            this.colMaxMs = new System.Windows.Forms.ColumnHeader();
            this.groupBoxWorkerPool = new System.Windows.Forms.GroupBox();
            this.lblRealtimeCaption = new System.Windows.Forms.Label();
            this.lblRealtimeWorkers = new System.Windows.Forms.Label();
            this.lblBatchCaption = new System.Windows.Forms.Label();
            this.lblBatchWorkers = new System.Windows.Forms.Label();
            this.lblScriptCaption = new System.Windows.Forms.Label();
            this.lblScriptWorkers = new System.Windows.Forms.Label();
            this.groupBoxSystemHealth = new System.Windows.Forms.GroupBox();
            this.lblMemoryCaption = new System.Windows.Forms.Label();
            this.pbMemory = new System.Windows.Forms.ProgressBar();
            this.lblMemoryValue = new System.Windows.Forms.Label();
            this.lblPendingCaption = new System.Windows.Forms.Label();
            this.lblPendingRequests = new System.Windows.Forms.Label();

            // --- Tab 2: Models ---
            this.groupBoxModels = new System.Windows.Forms.GroupBox();
            this.lvModels = new System.Windows.Forms.ListView();
            this.colModelName = new System.Windows.Forms.ColumnHeader();
            this.colModelType = new System.Windows.Forms.ColumnHeader();
            this.colModelStatus = new System.Windows.Forms.ColumnHeader();
            this.colModelLoadTime = new System.Windows.Forms.ColumnHeader();
            this.colModelLastUsed = new System.Windows.Forms.ColumnHeader();
            this.colModelSize = new System.Windows.Forms.ColumnHeader();
            this.groupBoxCache = new System.Windows.Forms.GroupBox();
            this.lblCacheCountCaption = new System.Windows.Forms.Label();
            this.lblCacheCount = new System.Windows.Forms.Label();
            this.lblCacheMemCaption = new System.Windows.Forms.Label();
            this.lblCacheMemory = new System.Windows.Forms.Label();
            this.lblCacheHitCaption = new System.Windows.Forms.Label();
            this.pbCacheHit = new System.Windows.Forms.ProgressBar();
            this.lblCacheHitRate = new System.Windows.Forms.Label();
            this.btnRefreshModels = new System.Windows.Forms.Button();

            // --- Tab 3: Test ---
            this.groupBoxQuickTest = new System.Windows.Forms.GroupBox();
            this.btnTestPing = new System.Windows.Forms.Button();
            this.lblPingResult = new System.Windows.Forms.Label();
            this.groupBoxPredictPower = new System.Windows.Forms.GroupBox();
            this.lblTestKw = new System.Windows.Forms.Label();
            this.numTestKw = new System.Windows.Forms.NumericUpDown();
            this.btnTestPredict = new System.Windows.Forms.Button();
            this.lblTestHistory = new System.Windows.Forms.Label();
            this.txtTestHistory = new System.Windows.Forms.TextBox();
            this.lblTestModel = new System.Windows.Forms.Label();
            this.txtTestModel = new System.Windows.Forms.TextBox();
            this.groupBoxScript = new System.Windows.Forms.GroupBox();
            this.txtTestScript = new System.Windows.Forms.TextBox();
            this.btnTestScript = new System.Windows.Forms.Button();
            this.lblTestPayload = new System.Windows.Forms.Label();
            this.txtTestPayload = new System.Windows.Forms.TextBox();
            this.chkAllowTagWrite = new System.Windows.Forms.CheckBox();
            this.groupBoxTrend = new System.Windows.Forms.GroupBox();
            this.txtTestTrendValues = new System.Windows.Forms.TextBox();
            this.btnTestTrend = new System.Windows.Forms.Button();
            this.groupBoxTestResult = new System.Windows.Forms.GroupBox();
            this.rtbTestResult = new System.Windows.Forms.RichTextBox();

            // --- Tab 4: Log ---
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.panelLogBottom = new System.Windows.Forms.Panel();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.chkAutoScroll = new System.Windows.Forms.CheckBox();
            this.lblFilterCaption = new System.Windows.Forms.Label();
            this.cmbLogFilter = new System.Windows.Forms.ComboBox();

            // === SuspendLayout ===
            this.menuStrip1.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabConnection.SuspendLayout();
            this.tabMetrics.SuspendLayout();
            this.tabModels.SuspendLayout();
            this.tabTest.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.groupBoxConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.groupBoxPaths.SuspendLayout();
            this.groupBoxTimeouts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRequestTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPingInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrainingTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScriptTimeout)).BeginInit();
            this.groupBoxLogging.SuspendLayout();
            this.groupBoxServiceMetrics.SuspendLayout();
            this.groupBoxWorkerPool.SuspendLayout();
            this.groupBoxSystemHealth.SuspendLayout();
            this.groupBoxModels.SuspendLayout();
            this.groupBoxCache.SuspendLayout();
            this.groupBoxQuickTest.SuspendLayout();
            this.groupBoxPredictPower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTestKw)).BeginInit();
            this.groupBoxScript.SuspendLayout();
            this.groupBoxTrend.SuspendLayout();
            this.groupBoxTestResult.SuspendLayout();
            this.panelLogBottom.SuspendLayout();
            this.SuspendLayout();

            // =======================================
            // MenuStrip
            // =======================================
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileToolStripMenuItem, this.toolsToolStripMenuItem });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 0;
            //
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.closeToolStripMenuItem });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Text = "File";
            //
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            //
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.exportLogToolStripMenuItem });
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Text = "Tools";
            //
            this.exportLogToolStripMenuItem.Name = "exportLogToolStripMenuItem";
            this.exportLogToolStripMenuItem.Text = "Export Log...";
            this.exportLogToolStripMenuItem.Click += new System.EventHandler(this.exportLogToolStripMenuItem_Click);

            // =======================================
            // panelHeader (상단 고정 패널)
            // =======================================
            this.panelHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHeader.Controls.Add(this.chkEnableAi);
            this.panelHeader.Controls.Add(this.lblHeaderTitle);
            this.panelHeader.Controls.Add(this.lblStateCaption);
            this.panelHeader.Controls.Add(this.lblState);
            this.panelHeader.Controls.Add(this.lblUptimeCaption);
            this.panelHeader.Controls.Add(this.lblUptime);
            this.panelHeader.Controls.Add(this.btnStart);
            this.panelHeader.Controls.Add(this.btnStop);
            this.panelHeader.Controls.Add(this.btnPing);
            this.panelHeader.Controls.Add(this.lblPidCaption);
            this.panelHeader.Controls.Add(this.lblProcessId);
            this.panelHeader.Controls.Add(this.lblFailCaption);
            this.panelHeader.Controls.Add(this.lblFailureCount);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 24);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(884, 60);
            this.panelHeader.TabIndex = 1;
            //
            // chkEnableAi
            //
            this.chkEnableAi.AutoSize = true;
            this.chkEnableAi.Location = new System.Drawing.Point(12, 8);
            this.chkEnableAi.Name = "chkEnableAi";
            this.chkEnableAi.Size = new System.Drawing.Size(160, 16);
            this.chkEnableAi.TabIndex = 0;
            this.chkEnableAi.Text = "Use Python AI Engine";
            this.chkEnableAi.UseVisualStyleBackColor = true;
            this.chkEnableAi.CheckedChanged += new System.EventHandler(this.chkEnableAi_CheckedChanged);
            //
            // lblHeaderTitle
            //
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.Location = new System.Drawing.Point(12, 32);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(130, 15);
            this.lblHeaderTitle.TabIndex = 1;
            this.lblHeaderTitle.Text = "Python AI Engine";
            //
            // lblStateCaption
            //
            this.lblStateCaption.AutoSize = true;
            this.lblStateCaption.Location = new System.Drawing.Point(240, 8);
            this.lblStateCaption.Name = "lblStateCaption";
            this.lblStateCaption.Size = new System.Drawing.Size(44, 12);
            this.lblStateCaption.TabIndex = 2;
            this.lblStateCaption.Text = "Status:";
            //
            // lblState
            //
            this.lblState.AutoSize = true;
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblState.ForeColor = System.Drawing.Color.Gray;
            this.lblState.Location = new System.Drawing.Point(290, 6);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(90, 15);
            this.lblState.TabIndex = 3;
            this.lblState.Text = "Disconnected";
            //
            // lblUptimeCaption
            //
            this.lblUptimeCaption.AutoSize = true;
            this.lblUptimeCaption.Location = new System.Drawing.Point(240, 32);
            this.lblUptimeCaption.Name = "lblUptimeCaption";
            this.lblUptimeCaption.Size = new System.Drawing.Size(50, 12);
            this.lblUptimeCaption.TabIndex = 4;
            this.lblUptimeCaption.Text = "Uptime:";
            //
            // lblUptime
            //
            this.lblUptime.AutoSize = true;
            this.lblUptime.Location = new System.Drawing.Point(296, 32);
            this.lblUptime.Name = "lblUptime";
            this.lblUptime.Size = new System.Drawing.Size(16, 12);
            this.lblUptime.TabIndex = 5;
            this.lblUptime.Text = "--";
            //
            // btnStart
            //
            this.btnStart.Location = new System.Drawing.Point(440, 5);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(70, 25);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Location = new System.Drawing.Point(515, 5);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(70, 25);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnPing
            //
            this.btnPing.Location = new System.Drawing.Point(590, 5);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(60, 25);
            this.btnPing.TabIndex = 8;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            //
            // lblPidCaption
            //
            this.lblPidCaption.AutoSize = true;
            this.lblPidCaption.Location = new System.Drawing.Point(680, 8);
            this.lblPidCaption.Name = "lblPidCaption";
            this.lblPidCaption.Size = new System.Drawing.Size(28, 12);
            this.lblPidCaption.TabIndex = 9;
            this.lblPidCaption.Text = "PID:";
            //
            // lblProcessId
            //
            this.lblProcessId.AutoSize = true;
            this.lblProcessId.Location = new System.Drawing.Point(712, 8);
            this.lblProcessId.Name = "lblProcessId";
            this.lblProcessId.Size = new System.Drawing.Size(16, 12);
            this.lblProcessId.TabIndex = 10;
            this.lblProcessId.Text = "--";
            //
            // lblFailCaption
            //
            this.lblFailCaption.AutoSize = true;
            this.lblFailCaption.Location = new System.Drawing.Point(680, 32);
            this.lblFailCaption.Name = "lblFailCaption";
            this.lblFailCaption.Size = new System.Drawing.Size(52, 12);
            this.lblFailCaption.TabIndex = 11;
            this.lblFailCaption.Text = "Failures:";
            //
            // lblFailureCount
            //
            this.lblFailureCount.AutoSize = true;
            this.lblFailureCount.Location = new System.Drawing.Point(736, 32);
            this.lblFailureCount.Name = "lblFailureCount";
            this.lblFailureCount.Size = new System.Drawing.Size(11, 12);
            this.lblFailureCount.TabIndex = 12;
            this.lblFailureCount.Text = "0";

            // =======================================
            // TabControl
            // =======================================
            this.tabControl1.Controls.Add(this.tabConnection);
            this.tabControl1.Controls.Add(this.tabMetrics);
            this.tabControl1.Controls.Add(this.tabModels);
            this.tabControl1.Controls.Add(this.tabTest);
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 84);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(884, 527);
            this.tabControl1.TabIndex = 2;

            // =======================================
            // Tab 0: Connection
            // =======================================
            this.tabConnection.Controls.Add(this.groupBoxConnection);
            this.tabConnection.Controls.Add(this.groupBoxPaths);
            this.tabConnection.Controls.Add(this.groupBoxTimeouts);
            this.tabConnection.Controls.Add(this.groupBoxLogging);
            this.tabConnection.Controls.Add(this.btnApplySettings);
            this.tabConnection.Controls.Add(this.btnResetDefaults);
            this.tabConnection.Location = new System.Drawing.Point(4, 22);
            this.tabConnection.Name = "tabConnection";
            this.tabConnection.Padding = new System.Windows.Forms.Padding(8);
            this.tabConnection.Size = new System.Drawing.Size(876, 501);
            this.tabConnection.TabIndex = 0;
            this.tabConnection.Text = "Connection";
            this.tabConnection.UseVisualStyleBackColor = true;
            //
            // groupBoxConnection
            //
            this.groupBoxConnection.Controls.Add(this.lblHost);
            this.groupBoxConnection.Controls.Add(this.txtHost);
            this.groupBoxConnection.Controls.Add(this.lblPort);
            this.groupBoxConnection.Controls.Add(this.numPort);
            this.groupBoxConnection.Location = new System.Drawing.Point(11, 11);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(420, 55);
            this.groupBoxConnection.TabIndex = 0;
            this.groupBoxConnection.Text = "Connection";
            //
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(10, 25);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(34, 12);
            this.lblHost.Text = "Host:";
            this.txtHost.Location = new System.Drawing.Point(50, 22);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(150, 21);
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(220, 25);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(30, 12);
            this.lblPort.Text = "Port:";
            this.numPort.Location = new System.Drawing.Point(260, 22);
            this.numPort.Name = "numPort";
            this.numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numPort.Value = new decimal(new int[] { 5678, 0, 0, 0 });
            this.numPort.Size = new System.Drawing.Size(80, 21);
            //
            // groupBoxPaths
            //
            this.groupBoxPaths.Controls.Add(this.lblPythonExe);
            this.groupBoxPaths.Controls.Add(this.txtPythonExe);
            this.groupBoxPaths.Controls.Add(this.btnBrowsePython);
            this.groupBoxPaths.Controls.Add(this.lblEngineScript);
            this.groupBoxPaths.Controls.Add(this.txtEngineScript);
            this.groupBoxPaths.Controls.Add(this.btnBrowseScript);
            this.groupBoxPaths.Controls.Add(this.lblConfigFile);
            this.groupBoxPaths.Controls.Add(this.txtConfigFile);
            this.groupBoxPaths.Controls.Add(this.btnBrowseConfig);
            this.groupBoxPaths.Location = new System.Drawing.Point(11, 72);
            this.groupBoxPaths.Name = "groupBoxPaths";
            this.groupBoxPaths.Size = new System.Drawing.Size(850, 110);
            this.groupBoxPaths.TabIndex = 1;
            this.groupBoxPaths.Text = "Paths";
            //
            this.lblPythonExe.AutoSize = true;
            this.lblPythonExe.Location = new System.Drawing.Point(10, 22);
            this.lblPythonExe.Name = "lblPythonExe";
            this.lblPythonExe.Size = new System.Drawing.Size(72, 12);
            this.lblPythonExe.Text = "Python Exe:";
            this.txtPythonExe.Location = new System.Drawing.Point(110, 19);
            this.txtPythonExe.Name = "txtPythonExe";
            this.txtPythonExe.Size = new System.Drawing.Size(680, 21);
            this.btnBrowsePython.Location = new System.Drawing.Point(796, 18);
            this.btnBrowsePython.Name = "btnBrowsePython";
            this.btnBrowsePython.Size = new System.Drawing.Size(40, 23);
            this.btnBrowsePython.Text = "...";
            this.btnBrowsePython.Click += new System.EventHandler(this.btnBrowsePython_Click);
            //
            this.lblEngineScript.AutoSize = true;
            this.lblEngineScript.Location = new System.Drawing.Point(10, 50);
            this.lblEngineScript.Name = "lblEngineScript";
            this.lblEngineScript.Size = new System.Drawing.Size(88, 12);
            this.lblEngineScript.Text = "Engine Script:";
            this.txtEngineScript.Location = new System.Drawing.Point(110, 47);
            this.txtEngineScript.Name = "txtEngineScript";
            this.txtEngineScript.Size = new System.Drawing.Size(680, 21);
            this.btnBrowseScript.Location = new System.Drawing.Point(796, 46);
            this.btnBrowseScript.Name = "btnBrowseScript";
            this.btnBrowseScript.Size = new System.Drawing.Size(40, 23);
            this.btnBrowseScript.Text = "...";
            this.btnBrowseScript.Click += new System.EventHandler(this.btnBrowseScript_Click);
            //
            this.lblConfigFile.AutoSize = true;
            this.lblConfigFile.Location = new System.Drawing.Point(10, 78);
            this.lblConfigFile.Name = "lblConfigFile";
            this.lblConfigFile.Size = new System.Drawing.Size(72, 12);
            this.lblConfigFile.Text = "Config File:";
            this.txtConfigFile.Location = new System.Drawing.Point(110, 75);
            this.txtConfigFile.Name = "txtConfigFile";
            this.txtConfigFile.Size = new System.Drawing.Size(680, 21);
            this.btnBrowseConfig.Location = new System.Drawing.Point(796, 74);
            this.btnBrowseConfig.Name = "btnBrowseConfig";
            this.btnBrowseConfig.Size = new System.Drawing.Size(40, 23);
            this.btnBrowseConfig.Text = "...";
            this.btnBrowseConfig.Click += new System.EventHandler(this.btnBrowseConfig_Click);
            //
            // groupBoxTimeouts
            //
            this.groupBoxTimeouts.Controls.Add(this.lblRequestTimeout);
            this.groupBoxTimeouts.Controls.Add(this.numRequestTimeout);
            this.groupBoxTimeouts.Controls.Add(this.lblPingInterval);
            this.groupBoxTimeouts.Controls.Add(this.numPingInterval);
            this.groupBoxTimeouts.Controls.Add(this.lblBatchTimeout);
            this.groupBoxTimeouts.Controls.Add(this.numBatchTimeout);
            this.groupBoxTimeouts.Controls.Add(this.lblTrainingTimeout);
            this.groupBoxTimeouts.Controls.Add(this.numTrainingTimeout);
            this.groupBoxTimeouts.Controls.Add(this.lblScriptTimeout);
            this.groupBoxTimeouts.Controls.Add(this.numScriptTimeout);
            this.groupBoxTimeouts.Location = new System.Drawing.Point(11, 188);
            this.groupBoxTimeouts.Name = "groupBoxTimeouts";
            this.groupBoxTimeouts.Size = new System.Drawing.Size(850, 80);
            this.groupBoxTimeouts.TabIndex = 2;
            this.groupBoxTimeouts.Text = "Timeouts (ms)";
            //
            this.lblRequestTimeout.AutoSize = true;
            this.lblRequestTimeout.Location = new System.Drawing.Point(10, 25);
            this.lblRequestTimeout.Text = "Request:";
            this.numRequestTimeout.Location = new System.Drawing.Point(75, 22);
            this.numRequestTimeout.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            this.numRequestTimeout.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numRequestTimeout.Value = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numRequestTimeout.Size = new System.Drawing.Size(80, 21);
            //
            this.lblPingInterval.AutoSize = true;
            this.lblPingInterval.Location = new System.Drawing.Point(175, 25);
            this.lblPingInterval.Text = "Ping:";
            this.numPingInterval.Location = new System.Drawing.Point(215, 22);
            this.numPingInterval.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            this.numPingInterval.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numPingInterval.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            this.numPingInterval.Size = new System.Drawing.Size(80, 21);
            //
            this.lblBatchTimeout.AutoSize = true;
            this.lblBatchTimeout.Location = new System.Drawing.Point(10, 52);
            this.lblBatchTimeout.Text = "Batch:";
            this.numBatchTimeout.Location = new System.Drawing.Point(75, 49);
            this.numBatchTimeout.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            this.numBatchTimeout.Minimum = new decimal(new int[] { 5000, 0, 0, 0 });
            this.numBatchTimeout.Value = new decimal(new int[] { 60000, 0, 0, 0 });
            this.numBatchTimeout.Size = new System.Drawing.Size(80, 21);
            //
            this.lblTrainingTimeout.AutoSize = true;
            this.lblTrainingTimeout.Location = new System.Drawing.Point(175, 52);
            this.lblTrainingTimeout.Text = "Training:";
            this.numTrainingTimeout.Location = new System.Drawing.Point(240, 49);
            this.numTrainingTimeout.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            this.numTrainingTimeout.Minimum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numTrainingTimeout.Value = new decimal(new int[] { 300000, 0, 0, 0 });
            this.numTrainingTimeout.Size = new System.Drawing.Size(80, 21);
            //
            this.lblScriptTimeout.AutoSize = true;
            this.lblScriptTimeout.Location = new System.Drawing.Point(340, 52);
            this.lblScriptTimeout.Text = "Script:";
            this.numScriptTimeout.Location = new System.Drawing.Point(390, 49);
            this.numScriptTimeout.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            this.numScriptTimeout.Minimum = new decimal(new int[] { 5000, 0, 0, 0 });
            this.numScriptTimeout.Value = new decimal(new int[] { 30000, 0, 0, 0 });
            this.numScriptTimeout.Size = new System.Drawing.Size(80, 21);
            //
            // groupBoxLogging
            //
            this.groupBoxLogging.Controls.Add(this.lblLogLevel);
            this.groupBoxLogging.Controls.Add(this.cmbLogLevel);
            this.groupBoxLogging.Controls.Add(this.lblLogFormat);
            this.groupBoxLogging.Controls.Add(this.cmbLogFormat);
            this.groupBoxLogging.Location = new System.Drawing.Point(11, 274);
            this.groupBoxLogging.Name = "groupBoxLogging";
            this.groupBoxLogging.Size = new System.Drawing.Size(420, 55);
            this.groupBoxLogging.TabIndex = 3;
            this.groupBoxLogging.Text = "Logging";
            //
            this.lblLogLevel.AutoSize = true;
            this.lblLogLevel.Location = new System.Drawing.Point(10, 25);
            this.lblLogLevel.Text = "Log Level:";
            this.cmbLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogLevel.Items.AddRange(new object[] { "DEBUG", "INFO", "WARNING", "ERROR" });
            this.cmbLogLevel.Location = new System.Drawing.Point(80, 22);
            this.cmbLogLevel.Size = new System.Drawing.Size(100, 20);
            //
            this.lblLogFormat.AutoSize = true;
            this.lblLogFormat.Location = new System.Drawing.Point(200, 25);
            this.lblLogFormat.Text = "Log Format:";
            this.cmbLogFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogFormat.Items.AddRange(new object[] { "text", "json" });
            this.cmbLogFormat.Location = new System.Drawing.Point(280, 22);
            this.cmbLogFormat.Size = new System.Drawing.Size(80, 20);
            //
            // Apply / Reset
            //
            this.btnApplySettings.Location = new System.Drawing.Point(11, 340);
            this.btnApplySettings.Name = "btnApplySettings";
            this.btnApplySettings.Size = new System.Drawing.Size(80, 28);
            this.btnApplySettings.TabIndex = 4;
            this.btnApplySettings.Text = "Apply";
            this.btnApplySettings.UseVisualStyleBackColor = true;
            this.btnApplySettings.Click += new System.EventHandler(this.btnApplySettings_Click);
            //
            this.btnResetDefaults.Location = new System.Drawing.Point(100, 340);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new System.Drawing.Size(80, 28);
            this.btnResetDefaults.TabIndex = 5;
            this.btnResetDefaults.Text = "Reset";
            this.btnResetDefaults.UseVisualStyleBackColor = true;
            this.btnResetDefaults.Click += new System.EventHandler(this.btnResetDefaults_Click);

            // =======================================
            // Tab 1: Metrics
            // =======================================
            this.tabMetrics.Controls.Add(this.groupBoxServiceMetrics);
            this.tabMetrics.Controls.Add(this.groupBoxWorkerPool);
            this.tabMetrics.Controls.Add(this.groupBoxSystemHealth);
            this.tabMetrics.Location = new System.Drawing.Point(4, 22);
            this.tabMetrics.Name = "tabMetrics";
            this.tabMetrics.Padding = new System.Windows.Forms.Padding(8);
            this.tabMetrics.Size = new System.Drawing.Size(876, 501);
            this.tabMetrics.TabIndex = 1;
            this.tabMetrics.Text = "Metrics";
            this.tabMetrics.UseVisualStyleBackColor = true;
            //
            // groupBoxServiceMetrics
            //
            this.groupBoxServiceMetrics.Controls.Add(this.lvMetrics);
            this.groupBoxServiceMetrics.Location = new System.Drawing.Point(11, 11);
            this.groupBoxServiceMetrics.Name = "groupBoxServiceMetrics";
            this.groupBoxServiceMetrics.Size = new System.Drawing.Size(850, 260);
            this.groupBoxServiceMetrics.TabIndex = 0;
            this.groupBoxServiceMetrics.Text = "Service Metrics";
            //
            this.lvMetrics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colService, this.colTotal, this.colSuccess, this.colErrors,
                this.colErrorRate, this.colAvgMs, this.colMinMs, this.colMaxMs });
            this.lvMetrics.FullRowSelect = true;
            this.lvMetrics.GridLines = true;
            this.lvMetrics.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvMetrics.Location = new System.Drawing.Point(10, 20);
            this.lvMetrics.Name = "lvMetrics";
            this.lvMetrics.Size = new System.Drawing.Size(830, 230);
            this.lvMetrics.View = System.Windows.Forms.View.Details;
            //
            this.colService.Text = "Service";
            this.colService.Width = 150;
            this.colTotal.Text = "Total";
            this.colTotal.Width = 70;
            this.colSuccess.Text = "Success";
            this.colSuccess.Width = 70;
            this.colErrors.Text = "Errors";
            this.colErrors.Width = 70;
            this.colErrorRate.Text = "Error Rate";
            this.colErrorRate.Width = 80;
            this.colAvgMs.Text = "Avg(ms)";
            this.colAvgMs.Width = 80;
            this.colMinMs.Text = "Min(ms)";
            this.colMinMs.Width = 80;
            this.colMaxMs.Text = "Max(ms)";
            this.colMaxMs.Width = 80;
            //
            // groupBoxWorkerPool
            //
            this.groupBoxWorkerPool.Controls.Add(this.lblRealtimeCaption);
            this.groupBoxWorkerPool.Controls.Add(this.lblRealtimeWorkers);
            this.groupBoxWorkerPool.Controls.Add(this.lblBatchCaption);
            this.groupBoxWorkerPool.Controls.Add(this.lblBatchWorkers);
            this.groupBoxWorkerPool.Controls.Add(this.lblScriptCaption);
            this.groupBoxWorkerPool.Controls.Add(this.lblScriptWorkers);
            this.groupBoxWorkerPool.Location = new System.Drawing.Point(11, 280);
            this.groupBoxWorkerPool.Name = "groupBoxWorkerPool";
            this.groupBoxWorkerPool.Size = new System.Drawing.Size(420, 55);
            this.groupBoxWorkerPool.TabIndex = 1;
            this.groupBoxWorkerPool.Text = "Worker Pool";
            //
            this.lblRealtimeCaption.AutoSize = true;
            this.lblRealtimeCaption.Location = new System.Drawing.Point(10, 25);
            this.lblRealtimeCaption.Text = "Realtime:";
            this.lblRealtimeWorkers.AutoSize = true;
            this.lblRealtimeWorkers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblRealtimeWorkers.Location = new System.Drawing.Point(72, 25);
            this.lblRealtimeWorkers.Text = "--";
            //
            this.lblBatchCaption.AutoSize = true;
            this.lblBatchCaption.Location = new System.Drawing.Point(140, 25);
            this.lblBatchCaption.Text = "Batch:";
            this.lblBatchWorkers.AutoSize = true;
            this.lblBatchWorkers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBatchWorkers.Location = new System.Drawing.Point(185, 25);
            this.lblBatchWorkers.Text = "--";
            //
            this.lblScriptCaption.AutoSize = true;
            this.lblScriptCaption.Location = new System.Drawing.Point(260, 25);
            this.lblScriptCaption.Text = "Script:";
            this.lblScriptWorkers.AutoSize = true;
            this.lblScriptWorkers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblScriptWorkers.Location = new System.Drawing.Point(305, 25);
            this.lblScriptWorkers.Text = "--";
            //
            // groupBoxSystemHealth
            //
            this.groupBoxSystemHealth.Controls.Add(this.lblMemoryCaption);
            this.groupBoxSystemHealth.Controls.Add(this.pbMemory);
            this.groupBoxSystemHealth.Controls.Add(this.lblMemoryValue);
            this.groupBoxSystemHealth.Controls.Add(this.lblPendingCaption);
            this.groupBoxSystemHealth.Controls.Add(this.lblPendingRequests);
            this.groupBoxSystemHealth.Location = new System.Drawing.Point(440, 280);
            this.groupBoxSystemHealth.Name = "groupBoxSystemHealth";
            this.groupBoxSystemHealth.Size = new System.Drawing.Size(421, 55);
            this.groupBoxSystemHealth.TabIndex = 2;
            this.groupBoxSystemHealth.Text = "System Health";
            //
            this.lblMemoryCaption.AutoSize = true;
            this.lblMemoryCaption.Location = new System.Drawing.Point(10, 25);
            this.lblMemoryCaption.Text = "Memory:";
            this.pbMemory.Location = new System.Drawing.Point(72, 22);
            this.pbMemory.Size = new System.Drawing.Size(120, 18);
            this.lblMemoryValue.AutoSize = true;
            this.lblMemoryValue.Location = new System.Drawing.Point(200, 25);
            this.lblMemoryValue.Text = "-- MB";
            //
            this.lblPendingCaption.AutoSize = true;
            this.lblPendingCaption.Location = new System.Drawing.Point(280, 25);
            this.lblPendingCaption.Text = "Pending:";
            this.lblPendingRequests.AutoSize = true;
            this.lblPendingRequests.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPendingRequests.Location = new System.Drawing.Point(340, 25);
            this.lblPendingRequests.Text = "0";

            // =======================================
            // Tab 2: Models
            // =======================================
            this.tabModels.Controls.Add(this.groupBoxModels);
            this.tabModels.Controls.Add(this.groupBoxCache);
            this.tabModels.Controls.Add(this.btnRefreshModels);
            this.tabModels.Location = new System.Drawing.Point(4, 22);
            this.tabModels.Name = "tabModels";
            this.tabModels.Padding = new System.Windows.Forms.Padding(8);
            this.tabModels.Size = new System.Drawing.Size(876, 501);
            this.tabModels.TabIndex = 2;
            this.tabModels.Text = "Models";
            this.tabModels.UseVisualStyleBackColor = true;
            //
            this.groupBoxModels.Controls.Add(this.lvModels);
            this.groupBoxModels.Location = new System.Drawing.Point(11, 11);
            this.groupBoxModels.Name = "groupBoxModels";
            this.groupBoxModels.Size = new System.Drawing.Size(850, 280);
            this.groupBoxModels.TabIndex = 0;
            this.groupBoxModels.Text = "Loaded Models";
            //
            this.lvModels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colModelName, this.colModelType, this.colModelStatus,
                this.colModelLoadTime, this.colModelLastUsed, this.colModelSize });
            this.lvModels.FullRowSelect = true;
            this.lvModels.GridLines = true;
            this.lvModels.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvModels.Location = new System.Drawing.Point(10, 20);
            this.lvModels.Name = "lvModels";
            this.lvModels.Size = new System.Drawing.Size(830, 250);
            this.lvModels.View = System.Windows.Forms.View.Details;
            //
            this.colModelName.Text = "Name";
            this.colModelName.Width = 160;
            this.colModelType.Text = "Type";
            this.colModelType.Width = 100;
            this.colModelStatus.Text = "Status";
            this.colModelStatus.Width = 100;
            this.colModelLoadTime.Text = "Load Time";
            this.colModelLoadTime.Width = 140;
            this.colModelLastUsed.Text = "Last Used";
            this.colModelLastUsed.Width = 140;
            this.colModelSize.Text = "Size (MB)";
            this.colModelSize.Width = 80;
            //
            // groupBoxCache
            //
            this.groupBoxCache.Controls.Add(this.lblCacheCountCaption);
            this.groupBoxCache.Controls.Add(this.lblCacheCount);
            this.groupBoxCache.Controls.Add(this.lblCacheMemCaption);
            this.groupBoxCache.Controls.Add(this.lblCacheMemory);
            this.groupBoxCache.Controls.Add(this.lblCacheHitCaption);
            this.groupBoxCache.Controls.Add(this.pbCacheHit);
            this.groupBoxCache.Controls.Add(this.lblCacheHitRate);
            this.groupBoxCache.Location = new System.Drawing.Point(11, 300);
            this.groupBoxCache.Name = "groupBoxCache";
            this.groupBoxCache.Size = new System.Drawing.Size(540, 55);
            this.groupBoxCache.TabIndex = 1;
            this.groupBoxCache.Text = "Model Cache";
            //
            this.lblCacheCountCaption.AutoSize = true;
            this.lblCacheCountCaption.Location = new System.Drawing.Point(10, 25);
            this.lblCacheCountCaption.Text = "Cached:";
            this.lblCacheCount.AutoSize = true;
            this.lblCacheCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCacheCount.Location = new System.Drawing.Point(62, 25);
            this.lblCacheCount.Text = "0";
            //
            this.lblCacheMemCaption.AutoSize = true;
            this.lblCacheMemCaption.Location = new System.Drawing.Point(110, 25);
            this.lblCacheMemCaption.Text = "Memory:";
            this.lblCacheMemory.AutoSize = true;
            this.lblCacheMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCacheMemory.Location = new System.Drawing.Point(165, 25);
            this.lblCacheMemory.Text = "-- MB";
            //
            this.lblCacheHitCaption.AutoSize = true;
            this.lblCacheHitCaption.Location = new System.Drawing.Point(250, 25);
            this.lblCacheHitCaption.Text = "Hit Rate:";
            this.pbCacheHit.Location = new System.Drawing.Point(310, 22);
            this.pbCacheHit.Size = new System.Drawing.Size(120, 18);
            this.lblCacheHitRate.AutoSize = true;
            this.lblCacheHitRate.Location = new System.Drawing.Point(440, 25);
            this.lblCacheHitRate.Text = "--%";
            //
            this.btnRefreshModels.Location = new System.Drawing.Point(11, 365);
            this.btnRefreshModels.Name = "btnRefreshModels";
            this.btnRefreshModels.Size = new System.Drawing.Size(80, 28);
            this.btnRefreshModels.Text = "Refresh";
            this.btnRefreshModels.Click += new System.EventHandler(this.btnRefreshModels_Click);

            // =======================================
            // Tab 3: Test
            // =======================================
            this.tabTest.Controls.Add(this.groupBoxQuickTest);
            this.tabTest.Controls.Add(this.groupBoxPredictPower);
            this.tabTest.Controls.Add(this.groupBoxScript);
            this.tabTest.Controls.Add(this.groupBoxTrend);
            this.tabTest.Controls.Add(this.groupBoxTestResult);
            this.tabTest.Location = new System.Drawing.Point(4, 22);
            this.tabTest.Name = "tabTest";
            this.tabTest.Padding = new System.Windows.Forms.Padding(8);
            this.tabTest.Size = new System.Drawing.Size(876, 501);
            this.tabTest.TabIndex = 3;
            this.tabTest.Text = "Test";
            this.tabTest.UseVisualStyleBackColor = true;
            //
            // groupBoxQuickTest
            //
            this.groupBoxQuickTest.Controls.Add(this.btnTestPing);
            this.groupBoxQuickTest.Controls.Add(this.lblPingResult);
            this.groupBoxQuickTest.Location = new System.Drawing.Point(11, 11);
            this.groupBoxQuickTest.Size = new System.Drawing.Size(420, 55);
            this.groupBoxQuickTest.Text = "Quick Test";
            this.btnTestPing.Location = new System.Drawing.Point(10, 20);
            this.btnTestPing.Size = new System.Drawing.Size(70, 25);
            this.btnTestPing.Text = "Ping";
            this.btnTestPing.Click += new System.EventHandler(this.btnTestPing_Click);
            this.lblPingResult.AutoSize = true;
            this.lblPingResult.Location = new System.Drawing.Point(90, 25);
            this.lblPingResult.Text = "";
            //
            // groupBoxPredictPower (Phase 5: 3-Tier prediction with history + model)
            //
            this.groupBoxPredictPower.Controls.Add(this.lblTestKw);
            this.groupBoxPredictPower.Controls.Add(this.numTestKw);
            this.groupBoxPredictPower.Controls.Add(this.btnTestPredict);
            this.groupBoxPredictPower.Controls.Add(this.lblTestHistory);
            this.groupBoxPredictPower.Controls.Add(this.txtTestHistory);
            this.groupBoxPredictPower.Controls.Add(this.lblTestModel);
            this.groupBoxPredictPower.Controls.Add(this.txtTestModel);
            this.groupBoxPredictPower.Location = new System.Drawing.Point(11, 72);
            this.groupBoxPredictPower.Size = new System.Drawing.Size(850, 85);
            this.groupBoxPredictPower.Text = "Predict Power (3-Tier)";
            // Row 1: kW + model + button
            this.lblTestKw.AutoSize = true;
            this.lblTestKw.Location = new System.Drawing.Point(10, 22);
            this.lblTestKw.Text = "Current kW:";
            this.numTestKw.DecimalPlaces = 1;
            this.numTestKw.Location = new System.Drawing.Point(90, 19);
            this.numTestKw.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            this.numTestKw.Value = new decimal(new int[] { 150, 0, 0, 0 });
            this.numTestKw.Size = new System.Drawing.Size(100, 21);
            this.lblTestModel.AutoSize = true;
            this.lblTestModel.Location = new System.Drawing.Point(210, 22);
            this.lblTestModel.Text = "Model:";
            this.txtTestModel.Location = new System.Drawing.Point(260, 19);
            this.txtTestModel.Size = new System.Drawing.Size(140, 21);
            this.txtTestModel.Text = "power_predict";
            this.btnTestPredict.Location = new System.Drawing.Point(760, 17);
            this.btnTestPredict.Size = new System.Drawing.Size(80, 25);
            this.btnTestPredict.Text = "Predict";
            this.btnTestPredict.Click += new System.EventHandler(this.btnTestPredict_Click);
            // Row 2: history
            this.lblTestHistory.AutoSize = true;
            this.lblTestHistory.Location = new System.Drawing.Point(10, 52);
            this.lblTestHistory.Text = "History:";
            this.txtTestHistory.Location = new System.Drawing.Point(90, 49);
            this.txtTestHistory.Size = new System.Drawing.Size(750, 21);
            this.txtTestHistory.Text = "140.0, 142.5, 145.0, 148.0, 150.5";
            //
            // groupBoxScript (Phase 5: payload + tag_write permission)
            //
            this.groupBoxScript.Controls.Add(this.txtTestScript);
            this.groupBoxScript.Controls.Add(this.btnTestScript);
            this.groupBoxScript.Controls.Add(this.lblTestPayload);
            this.groupBoxScript.Controls.Add(this.txtTestPayload);
            this.groupBoxScript.Controls.Add(this.chkAllowTagWrite);
            this.groupBoxScript.Location = new System.Drawing.Point(11, 163);
            this.groupBoxScript.Size = new System.Drawing.Size(850, 130);
            this.groupBoxScript.Text = "Execute Script (Sandbox)";
            // Row 1: Script code
            this.txtTestScript.Location = new System.Drawing.Point(10, 20);
            this.txtTestScript.Multiline = true;
            this.txtTestScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestScript.Size = new System.Drawing.Size(740, 55);
            this.txtTestScript.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtTestScript.Text = "temp = tag_read('Station1.Temp')\nlog_info('Temperature: ' + str(temp))\nemit_result({'temp': temp})";
            this.btnTestScript.Location = new System.Drawing.Point(760, 20);
            this.btnTestScript.Size = new System.Drawing.Size(80, 25);
            this.btnTestScript.Text = "Execute";
            this.btnTestScript.Click += new System.EventHandler(this.btnTestScript_Click);
            // Row 2: Payload JSON + tag_write checkbox
            this.lblTestPayload.AutoSize = true;
            this.lblTestPayload.Location = new System.Drawing.Point(10, 85);
            this.lblTestPayload.Text = "Payload:";
            this.txtTestPayload.Location = new System.Drawing.Point(70, 82);
            this.txtTestPayload.Size = new System.Drawing.Size(580, 21);
            this.txtTestPayload.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtTestPayload.Text = "{\"_read_tags\": [\"Station1.Temp\"]}";
            this.chkAllowTagWrite.AutoSize = true;
            this.chkAllowTagWrite.Location = new System.Drawing.Point(660, 84);
            this.chkAllowTagWrite.Size = new System.Drawing.Size(90, 16);
            this.chkAllowTagWrite.Text = "Allow tag_write";
            this.chkAllowTagWrite.UseVisualStyleBackColor = true;
            //
            // groupBoxTrend
            //
            this.groupBoxTrend.Controls.Add(this.txtTestTrendValues);
            this.groupBoxTrend.Controls.Add(this.btnTestTrend);
            this.groupBoxTrend.Location = new System.Drawing.Point(11, 299);
            this.groupBoxTrend.Size = new System.Drawing.Size(850, 55);
            this.groupBoxTrend.Text = "Analyze Trend";
            this.txtTestTrendValues.Location = new System.Drawing.Point(10, 22);
            this.txtTestTrendValues.Size = new System.Drawing.Size(740, 21);
            this.txtTestTrendValues.Text = "1.0, 2.1, 2.9, 4.2, 5.0, 5.8, 7.1";
            this.btnTestTrend.Location = new System.Drawing.Point(760, 20);
            this.btnTestTrend.Size = new System.Drawing.Size(80, 25);
            this.btnTestTrend.Text = "Analyze";
            this.btnTestTrend.Click += new System.EventHandler(this.btnTestTrend_Click);
            //
            // groupBoxTestResult
            //
            this.groupBoxTestResult.Controls.Add(this.rtbTestResult);
            this.groupBoxTestResult.Location = new System.Drawing.Point(11, 360);
            this.groupBoxTestResult.Size = new System.Drawing.Size(850, 130);
            this.groupBoxTestResult.Text = "Test Result";
            this.rtbTestResult.Location = new System.Drawing.Point(10, 20);
            this.rtbTestResult.ReadOnly = true;
            this.rtbTestResult.Size = new System.Drawing.Size(830, 100);
            this.rtbTestResult.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbTestResult.BackColor = System.Drawing.Color.White;
            this.rtbTestResult.Text = "";

            // =======================================
            // Tab 4: Log
            // =======================================
            this.tabLog.Controls.Add(this.richTextBoxLog);
            this.tabLog.Controls.Add(this.panelLogBottom);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(8);
            this.tabLog.Size = new System.Drawing.Size(876, 501);
            this.tabLog.TabIndex = 4;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            //
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBoxLog.Location = new System.Drawing.Point(8, 8);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.BackColor = System.Drawing.Color.White;
            this.richTextBoxLog.Size = new System.Drawing.Size(860, 450);
            this.richTextBoxLog.Text = "";
            //
            this.panelLogBottom.Controls.Add(this.btnClearLog);
            this.panelLogBottom.Controls.Add(this.chkAutoScroll);
            this.panelLogBottom.Controls.Add(this.lblFilterCaption);
            this.panelLogBottom.Controls.Add(this.cmbLogFilter);
            this.panelLogBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLogBottom.Location = new System.Drawing.Point(8, 460);
            this.panelLogBottom.Name = "panelLogBottom";
            this.panelLogBottom.Size = new System.Drawing.Size(860, 33);
            //
            this.btnClearLog.Location = new System.Drawing.Point(0, 5);
            this.btnClearLog.Size = new System.Drawing.Size(80, 25);
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            this.chkAutoScroll.AutoSize = true;
            this.chkAutoScroll.Checked = true;
            this.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoScroll.Location = new System.Drawing.Point(100, 8);
            this.chkAutoScroll.Text = "Auto Scroll";
            //
            this.lblFilterCaption.AutoSize = true;
            this.lblFilterCaption.Location = new System.Drawing.Point(220, 10);
            this.lblFilterCaption.Text = "Filter:";
            //
            this.cmbLogFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogFilter.Items.AddRange(new object[] { "All", "INFO", "WARN", "ERROR" });
            this.cmbLogFilter.Location = new System.Drawing.Point(260, 6);
            this.cmbLogFilter.SelectedIndex = 0;
            this.cmbLogFilter.Size = new System.Drawing.Size(80, 20);

            // =======================================
            // FormPythonAiDashboard
            // =======================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 611);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormPythonAiDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Python AI Engine Dashboard";
            this.Load += new System.EventHandler(this.FormPythonAiDashboard_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPythonAiDashboard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormPythonAiDashboard_FormClosed);

            // === ResumeLayout ===
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.groupBoxPaths.ResumeLayout(false);
            this.groupBoxPaths.PerformLayout();
            this.groupBoxTimeouts.ResumeLayout(false);
            this.groupBoxTimeouts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRequestTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPingInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrainingTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScriptTimeout)).EndInit();
            this.groupBoxLogging.ResumeLayout(false);
            this.groupBoxLogging.PerformLayout();
            this.tabConnection.ResumeLayout(false);
            this.groupBoxServiceMetrics.ResumeLayout(false);
            this.groupBoxWorkerPool.ResumeLayout(false);
            this.groupBoxWorkerPool.PerformLayout();
            this.groupBoxSystemHealth.ResumeLayout(false);
            this.groupBoxSystemHealth.PerformLayout();
            this.tabMetrics.ResumeLayout(false);
            this.groupBoxModels.ResumeLayout(false);
            this.groupBoxCache.ResumeLayout(false);
            this.groupBoxCache.PerformLayout();
            this.tabModels.ResumeLayout(false);
            this.groupBoxQuickTest.ResumeLayout(false);
            this.groupBoxQuickTest.PerformLayout();
            this.groupBoxPredictPower.ResumeLayout(false);
            this.groupBoxPredictPower.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTestKw)).EndInit();
            this.groupBoxScript.ResumeLayout(false);
            this.groupBoxScript.PerformLayout();
            this.groupBoxTrend.ResumeLayout(false);
            this.groupBoxTrend.PerformLayout();
            this.groupBoxTestResult.ResumeLayout(false);
            this.tabTest.ResumeLayout(false);
            this.panelLogBottom.ResumeLayout(false);
            this.panelLogBottom.PerformLayout();
            this.tabLog.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // MenuStrip
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportLogToolStripMenuItem;

        // Header
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.CheckBox chkEnableAi;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblStateCaption;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblUptimeCaption;
        private System.Windows.Forms.Label lblUptime;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Label lblPidCaption;
        private System.Windows.Forms.Label lblProcessId;
        private System.Windows.Forms.Label lblFailCaption;
        private System.Windows.Forms.Label lblFailureCount;

        // TabControl
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabConnection;
        private System.Windows.Forms.TabPage tabMetrics;
        private System.Windows.Forms.TabPage tabModels;
        private System.Windows.Forms.TabPage tabTest;
        private System.Windows.Forms.TabPage tabLog;

        // Tab 0: Connection
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.GroupBox groupBoxPaths;
        private System.Windows.Forms.Label lblPythonExe;
        private System.Windows.Forms.TextBox txtPythonExe;
        private System.Windows.Forms.Button btnBrowsePython;
        private System.Windows.Forms.Label lblEngineScript;
        private System.Windows.Forms.TextBox txtEngineScript;
        private System.Windows.Forms.Button btnBrowseScript;
        private System.Windows.Forms.Label lblConfigFile;
        private System.Windows.Forms.TextBox txtConfigFile;
        private System.Windows.Forms.Button btnBrowseConfig;
        private System.Windows.Forms.GroupBox groupBoxTimeouts;
        private System.Windows.Forms.Label lblRequestTimeout;
        private System.Windows.Forms.NumericUpDown numRequestTimeout;
        private System.Windows.Forms.Label lblPingInterval;
        private System.Windows.Forms.NumericUpDown numPingInterval;
        private System.Windows.Forms.Label lblBatchTimeout;
        private System.Windows.Forms.NumericUpDown numBatchTimeout;
        private System.Windows.Forms.Label lblTrainingTimeout;
        private System.Windows.Forms.NumericUpDown numTrainingTimeout;
        private System.Windows.Forms.Label lblScriptTimeout;
        private System.Windows.Forms.NumericUpDown numScriptTimeout;
        private System.Windows.Forms.GroupBox groupBoxLogging;
        private System.Windows.Forms.Label lblLogLevel;
        private System.Windows.Forms.ComboBox cmbLogLevel;
        private System.Windows.Forms.Label lblLogFormat;
        private System.Windows.Forms.ComboBox cmbLogFormat;
        private System.Windows.Forms.Button btnApplySettings;
        private System.Windows.Forms.Button btnResetDefaults;

        // Tab 1: Metrics
        private System.Windows.Forms.GroupBox groupBoxServiceMetrics;
        private System.Windows.Forms.ListView lvMetrics;
        private System.Windows.Forms.ColumnHeader colService;
        private System.Windows.Forms.ColumnHeader colTotal;
        private System.Windows.Forms.ColumnHeader colSuccess;
        private System.Windows.Forms.ColumnHeader colErrors;
        private System.Windows.Forms.ColumnHeader colErrorRate;
        private System.Windows.Forms.ColumnHeader colAvgMs;
        private System.Windows.Forms.ColumnHeader colMinMs;
        private System.Windows.Forms.ColumnHeader colMaxMs;
        private System.Windows.Forms.GroupBox groupBoxWorkerPool;
        private System.Windows.Forms.Label lblRealtimeCaption;
        private System.Windows.Forms.Label lblRealtimeWorkers;
        private System.Windows.Forms.Label lblBatchCaption;
        private System.Windows.Forms.Label lblBatchWorkers;
        private System.Windows.Forms.Label lblScriptCaption;
        private System.Windows.Forms.Label lblScriptWorkers;
        private System.Windows.Forms.GroupBox groupBoxSystemHealth;
        private System.Windows.Forms.Label lblMemoryCaption;
        private System.Windows.Forms.ProgressBar pbMemory;
        private System.Windows.Forms.Label lblMemoryValue;
        private System.Windows.Forms.Label lblPendingCaption;
        private System.Windows.Forms.Label lblPendingRequests;

        // Tab 2: Models
        private System.Windows.Forms.GroupBox groupBoxModels;
        private System.Windows.Forms.ListView lvModels;
        private System.Windows.Forms.ColumnHeader colModelName;
        private System.Windows.Forms.ColumnHeader colModelType;
        private System.Windows.Forms.ColumnHeader colModelStatus;
        private System.Windows.Forms.ColumnHeader colModelLoadTime;
        private System.Windows.Forms.ColumnHeader colModelLastUsed;
        private System.Windows.Forms.ColumnHeader colModelSize;
        private System.Windows.Forms.GroupBox groupBoxCache;
        private System.Windows.Forms.Label lblCacheCountCaption;
        private System.Windows.Forms.Label lblCacheCount;
        private System.Windows.Forms.Label lblCacheMemCaption;
        private System.Windows.Forms.Label lblCacheMemory;
        private System.Windows.Forms.Label lblCacheHitCaption;
        private System.Windows.Forms.ProgressBar pbCacheHit;
        private System.Windows.Forms.Label lblCacheHitRate;
        private System.Windows.Forms.Button btnRefreshModels;

        // Tab 3: Test
        private System.Windows.Forms.GroupBox groupBoxQuickTest;
        private System.Windows.Forms.Button btnTestPing;
        private System.Windows.Forms.Label lblPingResult;
        private System.Windows.Forms.GroupBox groupBoxPredictPower;
        private System.Windows.Forms.Label lblTestKw;
        private System.Windows.Forms.NumericUpDown numTestKw;
        private System.Windows.Forms.Button btnTestPredict;
        private System.Windows.Forms.Label lblTestHistory;
        private System.Windows.Forms.TextBox txtTestHistory;
        private System.Windows.Forms.Label lblTestModel;
        private System.Windows.Forms.TextBox txtTestModel;
        private System.Windows.Forms.GroupBox groupBoxScript;
        private System.Windows.Forms.TextBox txtTestScript;
        private System.Windows.Forms.Button btnTestScript;
        private System.Windows.Forms.Label lblTestPayload;
        private System.Windows.Forms.TextBox txtTestPayload;
        private System.Windows.Forms.CheckBox chkAllowTagWrite;
        private System.Windows.Forms.GroupBox groupBoxTrend;
        private System.Windows.Forms.TextBox txtTestTrendValues;
        private System.Windows.Forms.Button btnTestTrend;
        private System.Windows.Forms.GroupBox groupBoxTestResult;
        private System.Windows.Forms.RichTextBox rtbTestResult;

        // Tab 4: Log
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Panel panelLogBottom;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkAutoScroll;
        private System.Windows.Forms.Label lblFilterCaption;
        private System.Windows.Forms.ComboBox cmbLogFilter;
    }
}
