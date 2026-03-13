using AutoLibLocal;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetTools;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 관리 대시보드.
    /// FormOpcUaServer 패턴: Timer 기반 UI 갱신, Thread-safe 로그, 레지스트리 영속화.
    /// </summary>
    public partial class FormPythonAiDashboard : Form
    {
        private Timer timerFast;
        private Timer timerSlow;
        private volatile bool _isRefreshing;
        private DateTime _connectedTime = DateTime.MinValue;
        private const int MaxLogLines = 1000;
        private string _currentLogFilter = "All";

        public FormPythonAiDashboard()
        {
            InitializeComponent();
            SetupUI();
        }

        // ==================== Form Lifecycle ====================

        private void FormPythonAiDashboard_Load(object sender, EventArgs e)
        {
            ApplyLocalization();
            LoadSettingsToUI();

            PythonAiLogBridge.Attach(AddLog);
            PythonAiManager.StateChanged += OnStateChanged;

            timerFast = new Timer();
            timerFast.Interval = 1000;
            timerFast.Tick += timerFast_Tick;
            timerFast.Start();

            timerSlow = new Timer();
            timerSlow.Interval = 5000;
            timerSlow.Tick += timerSlow_Tick;
            timerSlow.Start();

            UpdateStateDisplay(PythonAiManager.State);
        }

        private void FormPythonAiDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettingsFromUI();
        }

        private void FormPythonAiDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            PythonAiLogBridge.Detach(AddLog);
            PythonAiManager.StateChanged -= OnStateChanged;

            if (timerFast != null) { timerFast.Stop(); }
            if (timerSlow != null) { timerSlow.Stop(); }
        }

        // ==================== Localization ====================

        private void ApplyLocalization()
        {
            bool isKor = Tools.IsLangKorean();

            this.Text = isKor ? "Python AI 엔진 대시보드" : "Python AI Engine Dashboard";

            // Header
            chkEnableAi.Text = isKor ? "Python AI 엔진 사용" : "Use Python AI Engine";
            btnStart.Text = isKor ? "시작" : "Start";
            btnStop.Text = isKor ? "정지" : "Stop";
            lblStateCaption.Text = isKor ? "상태:" : "Status:";
            lblUptimeCaption.Text = isKor ? "가동:" : "Uptime:";
            lblFailCaption.Text = isKor ? "실패:" : "Failures:";

            // Tabs
            tabConnection.Text = isKor ? "연결 설정" : "Connection";
            tabMetrics.Text = isKor ? "메트릭" : "Metrics";
            tabModels.Text = isKor ? "모델 관리" : "Models";
            tabTest.Text = isKor ? "테스트" : "Test";
            tabLog.Text = isKor ? "로그" : "Log";

            // Connection tab
            groupBoxConnection.Text = isKor ? "연결" : "Connection";
            groupBoxPaths.Text = isKor ? "경로" : "Paths";
            groupBoxTimeouts.Text = isKor ? "타임아웃 (ms)" : "Timeouts (ms)";
            groupBoxLogging.Text = isKor ? "로깅" : "Logging";
            btnApplySettings.Text = isKor ? "적용" : "Apply";
            btnResetDefaults.Text = isKor ? "기본값" : "Reset";

            // Metrics tab
            groupBoxServiceMetrics.Text = isKor ? "서비스 메트릭" : "Service Metrics";
            groupBoxWorkerPool.Text = isKor ? "워커 풀" : "Worker Pool";
            groupBoxSystemHealth.Text = isKor ? "시스템 상태" : "System Health";
            lblMemoryCaption.Text = isKor ? "메모리:" : "Memory:";
            lblPendingCaption.Text = isKor ? "대기:" : "Pending:";

            // Models tab
            groupBoxModels.Text = isKor ? "로드된 모델" : "Loaded Models";
            groupBoxCache.Text = isKor ? "모델 캐시" : "Model Cache";
            btnRefreshModels.Text = isKor ? "새로고침" : "Refresh";

            // Test tab
            groupBoxQuickTest.Text = isKor ? "빠른 테스트" : "Quick Test";
            groupBoxPredictPower.Text = isKor ? "전력 예측 (3-Tier)" : "Predict Power (3-Tier)";
            groupBoxScript.Text = isKor ? "스크립트 실행 (샌드박스)" : "Execute Script (Sandbox)";
            groupBoxTrend.Text = isKor ? "트렌드 분석" : "Analyze Trend";
            groupBoxTestResult.Text = isKor ? "테스트 결과" : "Test Result";
            btnTestPredict.Text = isKor ? "예측" : "Predict";
            btnTestScript.Text = isKor ? "실행" : "Execute";
            btnTestTrend.Text = isKor ? "분석" : "Analyze";
            lblTestHistory.Text = isKor ? "히스토리:" : "History:";
            lblTestModel.Text = isKor ? "모델:" : "Model:";
            lblTestPayload.Text = isKor ? "페이로드:" : "Payload:";
            chkAllowTagWrite.Text = isKor ? "태그쓰기 허용" : "Allow tag_write";

            // Log tab
            btnClearLog.Text = isKor ? "로그 지우기" : "Clear Log";
            chkAutoScroll.Text = isKor ? "자동 스크롤" : "Auto Scroll";
            lblFilterCaption.Text = isKor ? "필터:" : "Filter:";

            // Menu
            fileToolStripMenuItem.Text = isKor ? "파일" : "File";
            closeToolStripMenuItem.Text = isKor ? "닫기" : "Close";
            toolsToolStripMenuItem.Text = isKor ? "도구" : "Tools";
            exportLogToolStripMenuItem.Text = isKor ? "로그 내보내기..." : "Export Log...";
        }

        // ==================== Timer Handlers ====================

        private void timerFast_Tick(object sender, EventArgs e)
        {
            UpdateStateDisplay(PythonAiManager.State);

            // Uptime
            if (PythonAiManager.IsConnected)
            {
                if (_connectedTime == DateTime.MinValue)
                    _connectedTime = DateTime.Now;

                TimeSpan uptime = DateTime.Now - _connectedTime;
                lblUptime.Text = String.Format("{0:D2}:{1:D2}:{2:D2}",
                    (int)uptime.TotalHours, uptime.Minutes, uptime.Seconds);
            }
            else
            {
                _connectedTime = DateTime.MinValue;
                lblUptime.Text = "--";
            }

            // Pending requests
            lblPendingRequests.Text = "0";

            // Failure count — reflection 없이 State에서 유추
            var state = PythonAiManager.State;
            if (state == PythonAiState.Connected)
                lblFailureCount.Text = "0";
        }

        private async void timerSlow_Tick(object sender, EventArgs e)
        {
            if (_isRefreshing) return;
            if (!PythonAiManager.IsConnected) return;

            _isRefreshing = true;
            try
            {
                await RefreshMetricsAsync();
                await RefreshHealthAsync();
                await RefreshModelsAsync();
            }
            catch { }
            finally
            {
                _isRefreshing = false;
            }
        }

        // ==================== State Management ====================

        private void SetupUI()
        {
            bool isConnected = PythonAiManager.IsConnected;
            btnStart.Enabled = !isConnected;
            btnStop.Enabled = isConnected;
            chkEnableAi.Checked = PythonAiConfig.Enabled;
        }

        private void UpdateStateDisplay(PythonAiState state)
        {
            string text;
            Color color;

            switch (state)
            {
                case PythonAiState.Connected:
                    text = "Connected";
                    color = Color.Green;
                    break;
                case PythonAiState.Connecting:
                    text = "Connecting...";
                    color = Color.Blue;
                    break;
                case PythonAiState.Degraded:
                    text = "Degraded";
                    color = Color.DarkOrange;
                    break;
                case PythonAiState.Error:
                    text = "Error (Circuit Open)";
                    color = Color.Red;
                    break;
                default:
                    text = "Disconnected";
                    color = Color.Gray;
                    break;
            }

            if (Tools.IsLangKorean())
            {
                switch (state)
                {
                    case PythonAiState.Connected: text = "연결됨"; break;
                    case PythonAiState.Connecting: text = "연결 중..."; break;
                    case PythonAiState.Degraded: text = "성능 저하"; break;
                    case PythonAiState.Error: text = "오류 (회로 차단)"; break;
                    default: text = "연결 끊김"; break;
                }
            }

            lblState.Text = text;
            lblState.ForeColor = color;

            bool isConn = (state == PythonAiState.Connected || state == PythonAiState.Degraded);
            btnStart.Enabled = !isConn && !PythonAiConfig.Enabled;
            btnStop.Enabled = isConn || PythonAiConfig.Enabled;
            btnPing.Enabled = isConn;
        }

        private void OnStateChanged(object sender, PythonAiStateChangedEventArgs e)
        {
            SafeUpdateUI(() => UpdateStateDisplay(e.CurrentState));
        }

        // ==================== Header Button Handlers ====================

        private void chkEnableAi_CheckedChanged(object sender, EventArgs e)
        {
            PythonAiConfig.Enabled = chkEnableAi.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            PythonAiConfig.Enabled = true;
            chkEnableAi.Checked = true;
            PythonAiManager.Start();
            PythonAiLogBridge.Info("Engine", "Dashboard: Engine started by user");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            PythonAiManager.Stop();
            PythonAiConfig.Enabled = false;
            chkEnableAi.Checked = false;
            PythonAiLogBridge.Info("Engine", "Dashboard: Engine stopped by user");
            UpdateStateDisplay(PythonAiState.Disconnected);
        }

        private async void btnPing_Click(object sender, EventArgs e)
        {
            btnPing.Enabled = false;
            try
            {
                var sw = Stopwatch.StartNew();
                bool ok = await PythonAiManager.PingAsync();
                sw.Stop();

                string result = String.Format("Ping: {0} ({1} ms)",
                    ok ? "OK" : "FAIL", sw.ElapsedMilliseconds);
                PythonAiLogBridge.Info("Connection", result);
            }
            catch (Exception ex)
            {
                PythonAiLogBridge.Error("Connection", "Ping error: " + ex.Message);
            }
            finally
            {
                btnPing.Enabled = PythonAiManager.IsConnected;
            }
        }

        // ==================== Connection Tab ====================

        private void btnBrowsePython_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Python Executable";
                dlg.Filter = "Executable (*.exe)|*.exe|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtPythonExe.Text = dlg.FileName;
            }
        }

        private void btnBrowseScript_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Engine Script (main.py)";
                dlg.Filter = "Python (*.py)|*.py|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtEngineScript.Text = dlg.FileName;
            }
        }

        private void btnBrowseConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Engine Config (JSON)";
                dlg.Filter = "JSON (*.json)|*.json|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtConfigFile.Text = dlg.FileName;
            }
        }

        private void btnApplySettings_Click(object sender, EventArgs e)
        {
            SaveSettingsFromUI();
            string msg = Tools.IsLangKorean() ? "설정이 적용되었습니다." : "Settings applied.";
            PythonAiLogBridge.Info("Config", msg);
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            txtHost.Text = "127.0.0.1";
            numPort.Value = 5678;
            txtPythonExe.Text = "python";
            txtEngineScript.Text = "";
            txtConfigFile.Text = "";
            numRequestTimeout.Value = 10000;
            numPingInterval.Value = 5000;
            numBatchTimeout.Value = 60000;
            numTrainingTimeout.Value = 300000;
            numScriptTimeout.Value = 30000;
            cmbLogLevel.SelectedIndex = 1; // INFO
            cmbLogFormat.SelectedIndex = 0; // text
        }

        // ==================== Metrics Tab ====================

        private async Task RefreshMetricsAsync()
        {
            try
            {
                var response = await PythonAiManager.GetMetricsAsync();
                if (response != null && response.IsSuccess)
                {
                    SafeUpdateUI(() => PopulateMetrics(response));
                }
            }
            catch { }
        }

        private void PopulateMetrics(PythonAiMessage response)
        {
            var result = response.Result as JObject;
            if (result == null)
            {
                if (response.Result is JToken jt)
                    result = jt as JObject;
            }
            if (result == null) return;

            var services = result["services"] as JObject;
            if (services == null) return;

            lvMetrics.BeginUpdate();
            lvMetrics.Items.Clear();
            foreach (var prop in services.Properties())
            {
                var svc = prop.Value as JObject;
                if (svc == null) continue;

                var item = new ListViewItem(prop.Name);
                item.SubItems.Add(GetJsonStr(svc, "total", "0"));
                item.SubItems.Add(GetJsonStr(svc, "success", "0"));
                item.SubItems.Add(GetJsonStr(svc, "errors", "0"));
                item.SubItems.Add(GetJsonStr(svc, "error_rate", "0.0"));
                item.SubItems.Add(GetJsonStr(svc, "avg_ms", "0.0"));
                item.SubItems.Add(GetJsonStr(svc, "min_ms", "0.0"));
                item.SubItems.Add(GetJsonStr(svc, "max_ms", "0.0"));
                lvMetrics.Items.Add(item);
            }
            lvMetrics.EndUpdate();
        }

        // ==================== Health ====================

        private async Task RefreshHealthAsync()
        {
            try
            {
                var response = await PythonAiManager.GetHealthAsync();
                if (response != null && response.IsSuccess)
                {
                    SafeUpdateUI(() => PopulateHealth(response));
                }
            }
            catch { }
        }

        private void PopulateHealth(PythonAiMessage response)
        {
            var result = response.Result as JObject;
            if (result == null)
            {
                if (response.Result is JToken jt)
                    result = jt as JObject;
            }
            if (result == null) return;

            // Workers
            var workers = result["workers"] as JObject;
            if (workers != null)
            {
                lblRealtimeWorkers.Text = GetJsonStr(workers, "realtime_workers", "--");
                lblBatchWorkers.Text = GetJsonStr(workers, "batch_workers", "--");
                lblScriptWorkers.Text = GetJsonStr(workers, "script_workers", "--");
            }

            // Memory
            var memory = result["memory"] as JObject;
            if (memory != null)
            {
                string rss = GetJsonStr(memory, "rss_mb", "0");
                lblMemoryValue.Text = rss + " MB";

                double rssMb = 0;
                double.TryParse(rss, NumberStyles.Any, CultureInfo.InvariantCulture, out rssMb);
                int pct = Math.Min(100, (int)(rssMb / 20.48)); // 2048MB = 100%
                pbMemory.Value = pct;
            }

            // Process ID
            string pid = GetJsonStr(result, "pid", null);
            if (pid != null)
                lblProcessId.Text = pid;
        }

        // ==================== Models Tab ====================

        private async Task RefreshModelsAsync()
        {
            try
            {
                var response = await PythonAiManager.GetModelsAsync();
                if (response != null && response.IsSuccess)
                {
                    SafeUpdateUI(() => PopulateModels(response));
                }
            }
            catch { }
        }

        private void PopulateModels(PythonAiMessage response)
        {
            var result = response.Result as JObject;
            if (result == null)
            {
                if (response.Result is JToken jt)
                    result = jt as JObject;
            }
            if (result == null) return;

            // Models
            var registry = result["registry"] as JArray;
            lvModels.BeginUpdate();
            lvModels.Items.Clear();
            if (registry != null)
            {
                foreach (var model in registry)
                {
                    var m = model as JObject;
                    if (m == null) continue;
                    var item = new ListViewItem(GetJsonStr(m, "name", "?"));
                    item.SubItems.Add(GetJsonStr(m, "framework", "--"));
                    item.SubItems.Add(GetJsonStr(m, "enabled", "true"));
                    item.SubItems.Add(GetJsonStr(m, "created", "--"));
                    item.SubItems.Add(GetJsonStr(m, "last_used", "--"));
                    item.SubItems.Add(GetJsonStr(m, "size_mb", "--"));
                    lvModels.Items.Add(item);
                }
            }
            lvModels.EndUpdate();

            // Cache
            var cache = result["cache"] as JObject;
            if (cache != null)
            {
                lblCacheCount.Text = GetJsonStr(cache, "count", "0");
                lblCacheMemory.Text = GetJsonStr(cache, "total_memory_mb", "0") + " MB";

                string hitStr = GetJsonStr(cache, "hit_rate", "0");
                double hitRate = 0;
                double.TryParse(hitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out hitRate);
                int hitPct = Math.Min(100, (int)(hitRate * 100));
                pbCacheHit.Value = hitPct;
                lblCacheHitRate.Text = String.Format("{0}%", hitPct);
            }
        }

        private async void btnRefreshModels_Click(object sender, EventArgs e)
        {
            btnRefreshModels.Enabled = false;
            try
            {
                await RefreshModelsAsync();
            }
            finally
            {
                btnRefreshModels.Enabled = true;
            }
        }

        // ==================== Test Tab ====================

        private async void btnTestPing_Click(object sender, EventArgs e)
        {
            btnTestPing.Enabled = false;
            AppendTestResult("--- Ping Test ---");
            try
            {
                var sw = Stopwatch.StartNew();
                bool ok = await PythonAiManager.PingAsync();
                sw.Stop();
                AppendTestResult(
                    String.Format("Ping: {0} ({1} ms)", ok ? "OK" : "FAIL", sw.ElapsedMilliseconds),
                    ok ? Color.Green : Color.Red);
            }
            catch (Exception ex)
            {
                AppendTestResult("Ping Error: " + ex.Message, Color.Red);
            }
            finally
            {
                btnTestPing.Enabled = true;
            }
        }

        private async void btnTestPredict_Click(object sender, EventArgs e)
        {
            btnTestPredict.Enabled = false;
            double kw = (double)numTestKw.Value;
            string modelName = txtTestModel.Text.Trim();
            AppendTestResult(String.Format("--- Predict Power (kW={0}, model={1}) ---", kw, modelName));
            try
            {
                // Build payload with history + model_name for 3-Tier prediction
                var payload = new JObject();
                payload["current_kw"] = kw;
                if (!string.IsNullOrEmpty(modelName))
                    payload["model_name"] = modelName;

                // Parse history CSV → JSON array
                string historyText = txtTestHistory.Text.Trim();
                if (!string.IsNullOrEmpty(historyText))
                {
                    var histArr = new JArray();
                    string[] parts = historyText.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        double val;
                        if (double.TryParse(parts[i].Trim(), NumberStyles.Any,
                            CultureInfo.InvariantCulture, out val))
                            histArr.Add(val);
                    }
                    if (histArr.Count > 0)
                        payload["history"] = histArr;
                }

                var response = await PythonAiManager.CallAsync(
                    "predict/power", payload, PythonCallOptions.RealtimePredict);

                if (response.IsSuccess)
                {
                    var result = response.Result as JObject;
                    if (result == null && response.Result is JToken jt)
                        result = jt as JObject;

                    if (result != null)
                    {
                        string method = GetJsonStr(result, "method", "?");
                        string predicted = GetJsonStr(result, "predicted_kw", "?");
                        string confidence = GetJsonStr(result, "confidence", "?");
                        string inferMs = GetJsonStr(result, "inference_ms", "?");
                        string mdl = GetJsonStr(result, "model", "?");

                        // Color-code by tier: green=model, blue=statistics, orange=fallback
                        Color methodColor = Color.Green;
                        if (method == "fallback") methodColor = Color.DarkOrange;
                        else if (method == "statistics") methodColor = Color.Blue;

                        AppendTestResult(
                            String.Format("[{0}] predicted={1} kW, confidence={2}, model={3}, inference={4}ms",
                                method, predicted, confidence, mdl, inferMs),
                            methodColor);
                    }
                    else
                    {
                        AppendTestResult(String.Format("Result: {0}", response.Result), Color.Green);
                    }
                }
                else
                {
                    AppendTestResult("Error: " + response.Error, Color.Red);
                }
            }
            catch (Exception ex)
            {
                AppendTestResult("Exception: " + ex.Message, Color.Red);
            }
            finally
            {
                btnTestPredict.Enabled = true;
            }
        }

        private async void btnTestScript_Click(object sender, EventArgs e)
        {
            btnTestScript.Enabled = false;
            string code = txtTestScript.Text;
            bool allowWrite = chkAllowTagWrite.Checked;
            AppendTestResult(String.Format("--- Execute Script (tag_write={0}) ---", allowWrite));
            try
            {
                // Build payload JSON
                JObject payloadObj = null;
                string payloadText = txtTestPayload.Text.Trim();
                if (!string.IsNullOrEmpty(payloadText))
                {
                    try { payloadObj = JObject.Parse(payloadText); }
                    catch (Exception parseEx)
                    {
                        AppendTestResult("Payload JSON parse error: " + parseEx.Message, Color.Red);
                        btnTestScript.Enabled = true;
                        return;
                    }
                }
                else
                {
                    payloadObj = new JObject();
                }

                // _allow_tag_write 플래그 추가 (ScriptBridge에서 권한으로 변환됨)
                if (allowWrite)
                    payloadObj["_allow_tag_write"] = true;

                // ScriptBridge 경유: _read_tags→_tag_snapshot, 권한 설정, _pending_writes 적용
                var response = await PythonAiScriptBridge.ExecuteWithPreloadAsync(code, payloadObj);

                if (response.IsSuccess)
                {
                    var result = response.Result as JObject;
                    if (result == null && response.Result is JToken jt)
                        result = jt as JObject;

                    if (result != null)
                    {
                        string status = GetJsonStr(result, "status", "?");
                        string durationMs = GetJsonStr(result, "duration_ms", "?");
                        Color statusColor = status == "ok" ? Color.Green : Color.Red;

                        // Script result
                        JToken scriptResult = result["result"];
                        if (scriptResult != null)
                        {
                            AppendTestResult(
                                String.Format("[{0}] result={1} ({2}ms)", status, scriptResult, durationMs),
                                statusColor);
                        }
                        else
                        {
                            string error = GetJsonStr(result, "error", "");
                            AppendTestResult(
                                String.Format("[{0}] {1} ({2}ms)", status,
                                    !string.IsNullOrEmpty(error) ? error : "(no result)", durationMs),
                                statusColor);
                        }

                        // Show logs if any
                        var logs = result["logs"] as JArray;
                        if (logs != null && logs.Count > 0)
                        {
                            for (int i = 0; i < logs.Count && i < 5; i++)
                            {
                                var logEntry = logs[i] as JObject;
                                if (logEntry != null)
                                {
                                    AppendTestResult(
                                        String.Format("  [{0}] {1}",
                                            GetJsonStr(logEntry, "level", "?"),
                                            GetJsonStr(logEntry, "message", "")),
                                        Color.Gray);
                                }
                            }
                        }

                        // Show pending writes count (already applied by ScriptBridge)
                        var pending = result["_pending_writes"] as JArray;
                        if (pending != null && pending.Count > 0)
                        {
                            AppendTestResult(
                                String.Format("  tag_writes: {0} applied", pending.Count),
                                Color.DarkCyan);
                        }
                    }
                    else
                    {
                        AppendTestResult(String.Format("Result: {0}", response.Result), Color.Green);
                    }
                }
                else
                {
                    AppendTestResult("Error: " + response.Error, Color.Red);
                }
            }
            catch (Exception ex)
            {
                AppendTestResult("Exception: " + ex.Message, Color.Red);
            }
            finally
            {
                btnTestScript.Enabled = true;
            }
        }

        private async void btnTestTrend_Click(object sender, EventArgs e)
        {
            btnTestTrend.Enabled = false;
            AppendTestResult("--- Analyze Trend ---");
            try
            {
                string[] parts = txtTestTrendValues.Text.Split(',');
                double[] values = new double[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    double.TryParse(parts[i].Trim(), NumberStyles.Any,
                        CultureInfo.InvariantCulture, out values[i]);
                }

                var response = await PythonAiManager.AnalyzeTrendAsync(values);
                if (response.IsSuccess)
                {
                    AppendTestResult(
                        String.Format("Result: {0}", response.Result),
                        Color.Green);
                }
                else
                {
                    AppendTestResult("Error: " + response.Error, Color.Red);
                }
            }
            catch (Exception ex)
            {
                AppendTestResult("Exception: " + ex.Message, Color.Red);
            }
            finally
            {
                btnTestTrend.Enabled = true;
            }
        }

        private void AppendTestResult(string text, Color? color = null)
        {
            if (rtbTestResult.IsDisposed) return;

            Color c = color ?? Color.Black;
            string line = String.Format("[{0:HH:mm:ss}] {1}\n", DateTime.Now, text);
            rtbTestResult.SelectionStart = rtbTestResult.TextLength;
            rtbTestResult.SelectionLength = 0;
            rtbTestResult.SelectionColor = c;
            rtbTestResult.AppendText(line);
            rtbTestResult.ScrollToCaret();
        }

        // ==================== Log Tab ====================

        private void AddLog(PythonAiLogEntry entry)
        {
            if (IsDisposed || !IsHandleCreated) return;

            if (InvokeRequired)
            {
                try { BeginInvoke(new Action<PythonAiLogEntry>(AddLog), entry); }
                catch { }
                return;
            }

            // 필터 적용
            if (_currentLogFilter != "All" && entry.Level != _currentLogFilter)
                return;

            // 라인 트림
            if (richTextBoxLog.Lines.Length > MaxLogLines)
            {
                int removeUpTo = richTextBoxLog.GetFirstCharIndexFromLine(
                    richTextBoxLog.Lines.Length - MaxLogLines);
                if (removeUpTo > 0)
                {
                    richTextBoxLog.Select(0, removeUpTo);
                    richTextBoxLog.SelectedText = "";
                }
            }

            // 색상 결정
            Color c;
            switch (entry.Level)
            {
                case "ERROR": c = Color.Red; break;
                case "WARN": c = Color.DarkOrange; break;
                default: c = Color.Black; break;
            }

            string line = String.Format("[{0:HH:mm:ss}] [{1}] [{2}] {3}\n",
                entry.Time, entry.Level, entry.Category, entry.Message);

            richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
            richTextBoxLog.SelectionLength = 0;
            richTextBoxLog.SelectionColor = c;
            richTextBoxLog.AppendText(line);

            if (chkAutoScroll.Checked)
                richTextBoxLog.ScrollToCaret();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
        }

        // ==================== Settings Persistence ====================

        private void LoadSettingsToUI()
        {
            chkEnableAi.Checked = PythonAiConfig.Enabled;
            txtHost.Text = PythonAiConfig.Host;
            numPort.Value = PythonAiConfig.Port;
            txtPythonExe.Text = PythonAiConfig.PythonExePath;
            txtEngineScript.Text = PythonAiConfig.EngineScriptPath;
            txtConfigFile.Text = PythonAiConfig.EngineConfigPath;
            numRequestTimeout.Value = PythonAiConfig.RequestTimeoutMs;
            numPingInterval.Value = PythonAiConfig.PingIntervalMs;
            numBatchTimeout.Value = PythonAiConfig.BatchTimeoutMs;
            numTrainingTimeout.Value = PythonAiConfig.TrainingTimeoutMs;
            numScriptTimeout.Value = PythonAiConfig.ScriptTimeoutMs;

            // Log Level
            int lvlIdx = cmbLogLevel.Items.IndexOf(PythonAiConfig.LogLevel);
            cmbLogLevel.SelectedIndex = lvlIdx >= 0 ? lvlIdx : 1;

            // Log Format
            int fmtIdx = cmbLogFormat.Items.IndexOf(PythonAiConfig.LogFormat);
            cmbLogFormat.SelectedIndex = fmtIdx >= 0 ? fmtIdx : 0;
        }

        private void SaveSettingsFromUI()
        {
            PythonAiConfig.Enabled = chkEnableAi.Checked;
            PythonAiConfig.Host = txtHost.Text.Trim();
            PythonAiConfig.Port = (int)numPort.Value;
            PythonAiConfig.PythonExePath = txtPythonExe.Text.Trim();
            PythonAiConfig.EngineScriptPath = txtEngineScript.Text.Trim();
            PythonAiConfig.EngineConfigPath = txtConfigFile.Text.Trim();
            PythonAiConfig.RequestTimeoutMs = (int)numRequestTimeout.Value;
            PythonAiConfig.PingIntervalMs = (int)numPingInterval.Value;
            PythonAiConfig.BatchTimeoutMs = (int)numBatchTimeout.Value;
            PythonAiConfig.TrainingTimeoutMs = (int)numTrainingTimeout.Value;
            PythonAiConfig.ScriptTimeoutMs = (int)numScriptTimeout.Value;

            if (cmbLogLevel.SelectedItem != null)
                PythonAiConfig.LogLevel = cmbLogLevel.SelectedItem.ToString();
            if (cmbLogFormat.SelectedItem != null)
                PythonAiConfig.LogFormat = cmbLogFormat.SelectedItem.ToString();

            SaveSettingsToRegistry();
        }

        private void SaveSettingsToRegistry()
        {
            try
            {
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiEnabled", PythonAiConfig.Enabled);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiHost", PythonAiConfig.Host);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiPort", PythonAiConfig.Port);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiExePath", PythonAiConfig.PythonExePath);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiScriptPath", PythonAiConfig.EngineScriptPath);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiConfigPath", PythonAiConfig.EngineConfigPath);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiRequestTimeout", PythonAiConfig.RequestTimeoutMs);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiPingInterval", PythonAiConfig.PingIntervalMs);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiBatchTimeout", PythonAiConfig.BatchTimeoutMs);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiTrainingTimeout", PythonAiConfig.TrainingTimeoutMs);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiScriptTimeout", PythonAiConfig.ScriptTimeoutMs);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiLogLevel", PythonAiConfig.LogLevel);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "PythonAiLogFormat", PythonAiConfig.LogFormat);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PythonAiDashboard: Registry save error: " + ex.Message);
            }
        }

        // ==================== Menu Handlers ====================

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "Export Log";
                dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FileName = String.Format("python_ai_log_{0:yyyyMMdd_HHmmss}.txt", DateTime.Now);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dlg.FileName, richTextBoxLog.Text,
                            System.Text.Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Export failed: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ==================== Helpers ====================

        private void SafeUpdateUI(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;

            if (InvokeRequired)
            {
                try { BeginInvoke(action); }
                catch { }
                return;
            }

            action();
        }

        private static string GetJsonStr(JObject obj, string key, string defaultVal)
        {
            if (obj == null) return defaultVal;
            JToken val;
            if (obj.TryGetValue(key, out val))
                return val.ToString();
            return defaultVal;
        }
    }
}
