using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using ScriptLibEdit.Debugger;
using ScriptLibRun;
using ScriptLibRun.Debugger;

namespace ScriptLibEdit.Editor
{
    /// <summary>
    /// WebView2 + Monaco Editor 기반 스크립트 편집기 컨트롤.
    /// TextArea를 대체하며 IScriptEditorPanel 인터페이스를 구현한다.
    /// </summary>
    public class MonacoEditorBridge : Panel, IScriptEditorPanel
    {
        // --- WebView2 Control ---
        private WebView2 webView;
        private bool isEditorReady = false;

        // --- State (JS에서 이벤트로 동기화) ---
        private string _sFilename = "";
        private bool _bChangeFlag = false;
        private bool _bInsertMode = true;
        private bool _bDisplayBreakPointZone = false;
        private int _cursorLine = 0;
        private int _cursorColumn = 0;
        private bool _canUndo = false;
        private bool _canRedo = false;
        private bool _hasSelection = false;

        // --- Breakpoints ---
        private List<int> arrayBreakPoints = new List<int>();

        // --- Debug ---
        private static bool bDebugStarted = false;
        private bool bBreaking = false;

        // --- Async Request Queue (진짜 TaskCompletionSource<T> 사용) ---
        private Dictionary<string, TaskCompletionSource<string>> pendingContentRequests = new Dictionary<string, TaskCompletionSource<string>>();
        private Dictionary<string, TaskCompletionSource<string>> pendingSelectedTextRequests = new Dictionary<string, TaskCompletionSource<string>>();
        private Dictionary<string, TaskCompletionSource<int>> pendingPositionRequests = new Dictionary<string, TaskCompletionSource<int>>();

        // --- Script 실행 직렬화 큐 (순서 보장, async void 제거) ---
        private Task _scriptQueue = Task.CompletedTask;
        private readonly object _scriptLock = new object();

        // --- 동기 응답 대기 중 재진입 방지 ---
        private bool _isWaitingForSyncResponse = false;

        // --- Pool에서 가져온 WebView2 여부 (OnResize 수동 처리용) ---
        private bool _isPooledTaken = false;

        // --- Parent reference ---
        private UserControlScriptEditor parentEditor;

        // --- Pending content (set before editor is ready) ---
        private string pendingContent = null;

        // --- Pending navigation (set before editor is ready) ---
        private int? pendingGotoX = null;
        private int? pendingGotoY = null;
        private int? pendingGotoPosition = null;

        // --- Completion data (tags/methods) ---
        private string pendingTagsJson = null;
        private string pendingMethodsJson = null;

        // --- WebView2 Environment cache (shared across instances) ---
        private static CoreWebView2Environment sharedEnvironment = null;
        private static string sharedUserDataFolder = null;
        private static bool isPrewarming = false;

        // --- Pre-warmed WebView2 pool (앱 시작 시 미리 생성하여 즉시 사용 가능) ---
        private static WebView2 pooledWebView = null;
        private static bool pooledEditorReady = false;
        private static Form pooledHostForm = null;   // persistent - 1회 생성 후 재사용
        private static string cachedMonacoFolder = null;

        #region IScriptEditorPanel Properties

        public string sFilename
        {
            get { return _sFilename; }
            set { _sFilename = value; }
        }

        public bool bChangeFlag
        {
            get { return _bChangeFlag; }
            set { _bChangeFlag = value; }
        }

        public bool bInsertMode
        {
            get { return _bInsertMode; }
        }

        public bool bDisplayBreakPointZone
        {
            get { return _bDisplayBreakPointZone; }
            set
            {
                _bDisplayBreakPointZone = value;
                EnqueueScript(string.Format("window.bridge.setGlyphMargin({0})", value ? "true" : "false"));
            }
        }

        public int ViewCursorX { get { return _cursorColumn; } }
        public int ViewCursorY { get { return _cursorLine; } }

        // arrayString: 호환성을 위해 GetSourceString 호출 시 구축
        private List<StringBuilder> _arrayString = new List<StringBuilder>();
        public List<StringBuilder> arrayString { get { return _arrayString; } }

        // TextArea 호환: nPageX, nPageY (Monaco가 자체 처리하므로 무시)
        public int nPageX { get; set; }
        public int nPageY { get; set; }

        #endregion

        #region Constructor / Initialization

        // --- Loading overlay ---
        private Label loadingLabel;
        private bool initStarted = false;

        public MonacoEditorBridge(UserControlScriptEditor parent)
        {
            parentEditor = parent;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // 로딩 표시 레이블 (pre-warmed 사용 시 즉시 제거됨)
            loadingLabel = new Label();
            loadingLabel.Text = "Loading Script Editor...";
            loadingLabel.Dock = DockStyle.Fill;
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            loadingLabel.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
            loadingLabel.ForeColor = Color.Gray;
            loadingLabel.BackColor = Color.White;
            this.Controls.Add(loadingLabel);

            // HandleCreated에서 pooled 확인 또는 신규 초기화
            this.HandleCreated += MonacoEditorBridge_HandleCreated;
        }

        // --- 부모 Form 참조 (FormClosing에서 pool 반환) ---
        private Form parentForm = null;

        private void MonacoEditorBridge_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= MonacoEditorBridge_HandleCreated;
            if (initStarted) return;
            initStarted = true;

            System.Diagnostics.Debug.WriteLine(string.Format(
                "[MonacoEditor] HandleCreated: pooledWebView={0}, pooledEditorReady={1}, pooledDisposed={2}",
                pooledWebView != null ? "exists" : "null",
                pooledEditorReady,
                pooledWebView != null && pooledWebView.IsDisposed ? "YES!" : "no"));

            // 부모 Form의 FormClosing 이벤트 구독 (Dispose가 호출되지 않을 수 있으므로)
            HookParentFormClosing();

            // ── 방법1: Pre-warmed WebView2가 있으면 즉시 사용 (0초 로드) ──
            if (pooledWebView != null && pooledEditorReady && !pooledWebView.IsDisposed)
            {
                TakePooledWebView();
                OnEditorReady(); // pending content/tags/methods 처리
                return;
            }

            // ── 방법2: 폴백 - 신규 WebView2 생성 ──
            System.Diagnostics.Debug.WriteLine("[MonacoEditor] HandleCreated: FALLBACK - creating new WebView2");
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            webView.Visible = false;
            this.Controls.Add(webView);
            InitializeWebView2Async();
        }

        /// <summary>
        /// 부모 Form의 FormClosing 이벤트를 구독하여 WebView2를 pool에 반환한다.
        /// Dispose가 호출되지 않는 경우에도 pool 반환을 보장한다.
        /// </summary>
        private void HookParentFormClosing()
        {
            // 컨트롤 트리를 올라가며 실제 닫히는 Form을 찾는다.
            // TopLevel=false인 임베디드 Form(예: FormScriptEditorSimpleNew)은
            // 부모가 닫혀도 FormClosing이 발생하지 않으므로 건너뛴다.
            // MDI child(IsMdiChild=true)는 사용자가 직접 닫으므로 훅 대상이다.
            Control ctrl = this.Parent;
            while (ctrl != null)
            {
                if (ctrl is Form form && (form.TopLevel || form.IsMdiChild))
                {
                    parentForm = form;
                    form.FormClosing += OnParentFormClosing;
                    System.Diagnostics.Debug.WriteLine(string.Format(
                        "[MonacoEditor] Hooked FormClosing on: {0} (TopLevel={1}, IsMdiChild={2})",
                        form.GetType().Name, form.TopLevel, form.IsMdiChild));
                    return;
                }
                ctrl = ctrl.Parent;
            }
            System.Diagnostics.Debug.WriteLine("[MonacoEditor] WARNING: Top-level parent Form not found for FormClosing hook");
        }

        /// <summary>
        /// 부모 Form이 닫힐 때 WebView2를 pool에 반환한다.
        /// WebView2는 pooledHostForm.Controls에 유지되므로 별도 안전장치 불필요.
        /// </summary>
        private void OnParentFormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender is Form form)
                form.FormClosing -= OnParentFormClosing;
            parentForm = null;

            // WebView2를 pool에 반환
            if (webView != null && isEditorReady && webView.CoreWebView2 != null && pooledWebView == null)
            {
                System.Diagnostics.Debug.WriteLine("[MonacoEditor] FormClosing: returning webView to pool");
                ReturnWebViewToPool();
                webView = null; // 이 인스턴스에서 참조 해제 (Dispose 시 중복 반환 방지)
            }
        }

        /// <summary>
        /// Pre-warmed WebView2를 pool에서 가져와 즉시 사용한다.
        /// WebView2는 pooledHostForm.Controls에 유지된 채로 HWND만 이 패널로 이동한다.
        /// ControlCollection을 직접 조작하지 않아 WinForms 내부 상태 불일치 위험이 없다.
        /// </summary>
        private void TakePooledWebView()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            webView = pooledWebView;
            pooledWebView = null;
            pooledEditorReady = false;

            // pool용 메시지 핸들러 → 인스턴스 메시지 핸들러 교체
            webView.CoreWebView2.WebMessageReceived -= OnPooledWebMessageReceived;
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // WebView2는 pooledHostForm.Controls에 남겨둔 채로 HWND만 이동.
            // ControlCollection 리플렉션 없이 SetParent + SyncController만으로 동작.
            if (webView.IsHandleCreated && this.IsHandleCreated)
            {
                IntPtr hwnd = webView.Handle;
                int w = this.ClientSize.Width;
                int h = this.ClientSize.Height;

                SetParent(hwnd, this.Handle);
                MoveWindow(hwnd, 0, 0, w, h, true);
                ShowWindow(hwnd, SW_SHOW);

                // CoreWebView2Controller의 ParentWindow + Bounds를 새 부모로 업데이트
                // ParentWindow: 마우스/키보드 입력 라우팅
                // Bounds: 실제 렌더링 영역 (이것이 없으면 이전 크기로 렌더링됨)
                var controller = GetCoreWebView2Controller(webView);
                if (controller != null)
                {
                    controller.ParentWindow = this.Handle;
                    controller.Bounds = new Rectangle(0, 0, w, h);
                    controller.NotifyParentWindowPositionChanged();
                }
            }

            isEditorReady = true;
            _isPooledTaken = true;

            System.Diagnostics.Debug.WriteLine(string.Format(
                "[MonacoEditor] TakePooled via SetParent: {0}ms", sw.ElapsedMilliseconds));
        }

        #endregion

        #region Static Prewarm (앱 시작 시 WebView2 + Monaco 사전 로드)

        /// <summary>
        /// WebView2 데이터 폴더 경로를 반환한다.
        /// AppData\Local\Autobase 하위에 생성하여 Program Files 등 쓰기 불가 경로를 피한다.
        /// </summary>
        private static string GetWebView2DataFolder()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string folder = Path.Combine(appData, "Autobase", "ScriptEditor.WebView2");
                Directory.CreateDirectory(folder);
                return folder;
            }
            catch
            {
                string folder = Path.Combine(Path.GetTempPath(), "Autobase.ScriptEditor.WebView2");
                Directory.CreateDirectory(folder);
                return folder;
            }
        }

        /// <summary>
        /// WebView2 + Monaco Editor를 완전히 사전 로드한다.
        /// 앱 시작 시(StudioMain_Load) 호출하면 에디터를 열 때 즉시 표시된다.
        /// Environment 생성 → WebView2 초기화 → editor.html 네비게이션 → editorReady 대기 까지 완료한다.
        /// </summary>
        public static async void PrewarmWebView2Environment()
        {
            try
            {
                if (isPrewarming) return;
                isPrewarming = true;

                // 1. Environment 생성 (1회만)
                if (sharedEnvironment == null)
                {
                    sharedUserDataFolder = GetWebView2DataFolder();
                    sharedEnvironment = await CoreWebView2Environment.CreateAsync(null, sharedUserDataFolder, null);
                }

                // 2. WebView2 컨트롤까지 완전히 초기화
                await PrewarmWebView2Control();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[MonacoEditor] Prewarm failed: " + ex.Message);
            }
            finally
            {
                isPrewarming = false;
            }
        }

        /// <summary>
        /// WebView2 컨트롤을 숨겨진 Form에서 생성하고 Monaco를 완전히 로드한다.
        /// editorReady 메시지를 받으면 pooledEditorReady = true가 되어 즉시 사용 가능.
        /// </summary>
        private static async System.Threading.Tasks.Task PrewarmWebView2Control()
        {
            if (pooledWebView != null) return;

            // Monaco 폴더 경로 캐시
            if (cachedMonacoFolder == null)
                cachedMonacoFolder = GetMonacoFolderPathStatic();

            if (cachedMonacoFolder == null)
            {
                System.Diagnostics.Debug.WriteLine("[MonacoEditor] Prewarm: Monaco folder not found");
                return;
            }

            // 숨겨진 host Form 확보 (WebView2에 HWND 필요)
            EnsurePoolHostForm();

            var wv = new WebView2();
            wv.Dock = DockStyle.Fill;
            pooledHostForm.Controls.Add(wv);

            await wv.EnsureCoreWebView2Async(sharedEnvironment);

            wv.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "monaco.local", cachedMonacoFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            wv.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            wv.CoreWebView2.Settings.IsWebMessageEnabled = true;
            wv.CoreWebView2.Settings.AreDevToolsEnabled = true;
            wv.CoreWebView2.Settings.IsStatusBarEnabled = false;
            wv.CoreWebView2.Settings.IsZoomControlEnabled = false;

            // editorReady 메시지 대기용 핸들러
            pooledEditorReady = false;
            wv.CoreWebView2.WebMessageReceived += OnPooledWebMessageReceived;

            wv.CoreWebView2.Navigate("https://monaco.local/editor.html");
            pooledWebView = wv;

            // editorReady 대기 (최대 15초)
            int timeout = 0;
            while (!pooledEditorReady && timeout < 150)
            {
                await System.Threading.Tasks.Task.Delay(100);
                timeout++;
            }

            System.Diagnostics.Debug.WriteLine(pooledEditorReady
                ? "[MonacoEditor] Prewarm complete - WebView2 ready"
                : "[MonacoEditor] Prewarm timeout - editorReady not received");
        }

        /// <summary>
        /// Pool용 WebMessageReceived 핸들러. editorReady만 처리.
        /// </summary>
        private static void OnPooledWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string json = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(json)) return;

            var msg = MonacoMessageProtocol.ParseMessage(json);
            if (msg.type == "editorReady")
            {
                pooledEditorReady = true;
            }
        }

        #endregion

        #region Fallback Initialization (pre-warmed 없을 때)

        /// <summary>
        /// Pre-warmed WebView2가 없을 때 사용하는 폴백 초기화.
        /// HandleCreated 이벤트에서 호출된다.
        /// </summary>
        private async void InitializeWebView2Async()
        {
            try
            {
                // Prewarm이 진행 중이면 완료를 대기한다 (최대 5초)
                int waitCount = 0;
                while (isPrewarming && waitCount < 50)
                {
                    await System.Threading.Tasks.Task.Delay(100);
                    waitCount++;
                }

                // 대기 후 pooled가 준비되었으면 그것을 사용
                if (pooledWebView != null && pooledEditorReady)
                {
                    this.Controls.Remove(webView);
                    webView.Dispose();

                    TakePooledWebView();
                    OnEditorReady();
                    return;
                }

                // 공유 Environment 사용 (이미 생성된 경우 재사용)
                CoreWebView2Environment env = sharedEnvironment;
                if (env == null)
                {
                    sharedUserDataFolder = GetWebView2DataFolder();
                    env = await CoreWebView2Environment.CreateAsync(null, sharedUserDataFolder, null);
                    sharedEnvironment = env;
                }

                await webView.EnsureCoreWebView2Async(env);

                string monacoFolder = cachedMonacoFolder ?? GetMonacoFolderPathStatic();
                if (cachedMonacoFolder == null) cachedMonacoFolder = monacoFolder;

                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "monaco.local", monacoFolder,
                    CoreWebView2HostResourceAccessKind.Allow);

                webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = false;

                webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
                webView.CoreWebView2.Navigate("https://monaco.local/editor.html");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[MonacoEditor] Init failed: " + ex.ToString());

                if (loadingLabel != null)
                {
                    loadingLabel.Text = string.Format("Script Editor 로드 실패\n\n{0}\n\nWebView2 Runtime이 설치되어 있는지 확인하세요.", ex.Message);
                    loadingLabel.ForeColor = Color.Red;
                    loadingLabel.Font = new Font("Segoe UI", 9f);
                }
            }
        }

        #endregion

        #region Monaco Folder Path (static)

        private static string GetMonacoFolderPathStatic()
        {
            string exeDir = Application.StartupPath;
            string dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string path1 = Path.Combine(exeDir, "Monaco");
            if (IsValidMonacoFolder(path1)) return path1;

            string path2 = Path.Combine(dllDir, "Monaco");
            if (IsValidMonacoFolder(path2)) return path2;

            string found = SearchUpForMonaco(exeDir);
            if (found != null) return found;

            if (!string.Equals(dllDir, exeDir, StringComparison.OrdinalIgnoreCase))
            {
                found = SearchUpForMonaco(dllDir);
                if (found != null) return found;
            }

            try
            {
                string driveRoot = Path.GetPathRoot(exeDir);
                if (driveRoot != null)
                {
                    foreach (string dir in Directory.GetDirectories(driveRoot, "Autobase*"))
                    {
                        string candidate = Path.Combine(dir, @"ScriptLib\ScriptLibEdit\Monaco");
                        if (IsValidMonacoFolder(candidate))
                            return Path.GetFullPath(candidate);
                    }
                }
            }
            catch { }

            return path1;
        }

        private static bool IsValidMonacoFolder(string path)
        {
            return Directory.Exists(path)
                && File.Exists(Path.Combine(path, "editor.html"))
                && File.Exists(Path.Combine(path, @"monaco-editor\vs\loader.js"));
        }

        private static string SearchUpForMonaco(string startDir)
        {
            string dir = startDir;
            for (int i = 0; i < 6; i++)
            {
                string parent = Path.GetDirectoryName(dir);
                if (parent == null || parent == dir) break;
                dir = parent;

                string candidate = Path.Combine(dir, @"ScriptLib\ScriptLibEdit\Monaco");
                if (IsValidMonacoFolder(candidate))
                    return Path.GetFullPath(candidate);
            }
            return null;
        }

        #endregion

        #region WebView2 Message Handler

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string json = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(json)) return;

            var msg = MonacoMessageProtocol.ParseMessage(json);

            switch (msg.type)
            {
                case "editorReady":
                    isEditorReady = true;
                    OnEditorReady();
                    break;

                case "cursorChanged":
                    _cursorLine = msg.line;
                    _cursorColumn = msg.column;
                    if (parentEditor != null)
                        parentEditor.SendEvent(EnumEditorEventType.CursorPositionChanged);
                    break;

                case "contentChanged":
                    if (msg.isDirty && !_bChangeFlag)
                    {
                        _bChangeFlag = true;
                        if (parentEditor != null)
                            parentEditor.SendEvent(EnumEditorEventType.SourceModified);
                    }
                    _canUndo = msg.canUndo;
                    _canRedo = msg.canRedo;
                    _hasSelection = msg.hasSelection;
                    break;

                case "selectionChanged":
                    _hasSelection = msg.hasSelection;
                    break;

                case "insertModeChanged":
                    _bInsertMode = msg.isInsert;
                    if (parentEditor != null)
                        parentEditor.SendEvent(EnumEditorEventType.InsertModeChanged);
                    break;

                case "breakpointToggled":
                    ToggleBreakPoint(msg.line);
                    break;

                case "contentResponse":
                    if (msg.requestId != null && pendingContentRequests.ContainsKey(msg.requestId))
                    {
                        var tcs = pendingContentRequests[msg.requestId];
                        pendingContentRequests.Remove(msg.requestId);
                        tcs.TrySetResult(msg.content ?? "");
                    }
                    break;

                case "selectedTextResponse":
                    if (msg.requestId != null && pendingSelectedTextRequests.ContainsKey(msg.requestId))
                    {
                        var tcs = pendingSelectedTextRequests[msg.requestId];
                        pendingSelectedTextRequests.Remove(msg.requestId);
                        tcs.TrySetResult(msg.text ?? "");
                    }
                    break;

                case "cursorPositionResponse":
                    if (msg.requestId != null && pendingPositionRequests.ContainsKey(msg.requestId))
                    {
                        var tcs = pendingPositionRequests[msg.requestId];
                        pendingPositionRequests.Remove(msg.requestId);
                        tcs.TrySetResult(msg.position);
                    }
                    break;

                case "aiAssistRequest":
                    HandleAiAssistRequest(msg);
                    break;

                case "aiValidateRequest":
                    HandleAiValidateRequest(msg);
                    break;
            }
        }

        /// <summary>AI 코드 생성 요청 처리</summary>
        private async void HandleAiAssistRequest(MonacoMessage msg)
        {
            try
            {
                string prompt = msg.text ?? "";
                string requestId = msg.requestId ?? "";
                string content = msg.content ?? "";

                // 태그 목록 수집
                var tagNames = new List<string>();
                if (window.autobaseCompletionData?.tags != null)
                {
                    // pendingTagsJson에서 태그 이름 추출 (이미 로드된 데이터 활용)
                }

                var context = new
                {
                    existing_code = content,
                    cursor_line = _cursorLine
                };
                var payload = new { prompt = prompt, context = context };

                // Python AI Engine 호출 시도
                string resultCode = "";
                string explanation = "";
                try
                {
                    // PythonAiManager가 사용 가능한 경우 호출
                    // 런타임에서만 사용 가능하므로, Studio에서는 로컬 폴백 사용
                    resultCode = GenerateCodeLocal(prompt, content);
                    explanation = "로컬 템플릿 기반 생성";
                }
                catch
                {
                    resultCode = "// " + prompt + "\n// TODO: 구현 필요";
                    explanation = "코드 생성 실패";
                }

                string escapedCode = MonacoMessageProtocol.EscapeForJs(resultCode);
                string escapedExplanation = MonacoMessageProtocol.EscapeForJs(explanation);
                EnqueueScript(string.Format(
                    "window.autobaseAiAssist && window.autobaseAiAssist.onGenerateResult(\"{0}\", \"{1}\", \"{2}\")",
                    requestId, escapedCode, escapedExplanation));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AI Assist error: " + ex.Message);
            }
        }

        /// <summary>AI 코드 검증 요청 처리</summary>
        private async void HandleAiValidateRequest(MonacoMessage msg)
        {
            try
            {
                string code = msg.content ?? "";
                string requestId = msg.requestId ?? "";

                var diagnostics = ValidateCodeLocal(code);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(diagnostics);
                string escaped = MonacoMessageProtocol.EscapeForJs(json);
                EnqueueScript(string.Format(
                    "window.autobaseAiAssist && window.autobaseAiAssist.onValidateResult(\"{0}\", \"{1}\")",
                    requestId, escaped));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AI Validate error: " + ex.Message);
            }
        }

        /// <summary>로컬 코드 생성 (Python AI Engine 미사용 시 폴백)</summary>
        private string GenerateCodeLocal(string prompt, string existingCode)
        {
            string lower = prompt.ToLower();

            if (lower.Contains("알람") || lower.Contains("alarm"))
            {
                return "// 알람 체크\ndouble val = $Tag.value;\nif (val > $Tag.hihi)\n{\n\t@SetAlarm(\"TAG_HI\");\n}\nelse if (val < $Tag.lolo)\n{\n\t@SetAlarm(\"TAG_LO\");\n}";
            }
            if (lower.Contains("로그") || lower.Contains("log") || lower.Contains("기록"))
            {
                return "// 데이터 기록\ndouble val = $Tag.value;\n@LogToDatabase(\"Tag\", val, DateTime.Now);";
            }
            if (lower.Contains("토글") || lower.Contains("toggle"))
            {
                return "// 토글 제어\nif ($Tag.value == 0)\n\t$Tag.value = 1;\nelse\n\t$Tag.value = 0;";
            }
            if (lower.Contains("for") || lower.Contains("반복") || lower.Contains("loop"))
            {
                return "for (int i = 0; i < 10; i++)\n{\n\t// TODO: 반복 처리\n}";
            }

            return "// " + prompt + "\n// TODO: 구현 필요";
        }

        /// <summary>로컬 코드 검증</summary>
        private List<object> ValidateCodeLocal(string code)
        {
            var diagnostics = new List<object>();
            if (string.IsNullOrEmpty(code)) return diagnostics;

            string[] lines = code.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd('\r');
                string trimmed = line.Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//") || trimmed.StartsWith("/*"))
                    continue;

                // Check for statements without semicolons
                if (!trimmed.EndsWith(";") && !trimmed.EndsWith("{") && !trimmed.EndsWith("}")
                    && !trimmed.EndsWith(",") && !trimmed.EndsWith("(")
                    && !trimmed.StartsWith("if") && !trimmed.StartsWith("else")
                    && !trimmed.StartsWith("for") && !trimmed.StartsWith("while")
                    && !trimmed.StartsWith("//") && !trimmed.StartsWith("#")
                    && trimmed.Contains("="))
                {
                    diagnostics.Add(new { line = i + 1, column = 1, severity = 2, message = "세미콜론(;)이 누락되었을 수 있습니다" });
                }

                // Check for assignment in condition
                if ((trimmed.StartsWith("if") || trimmed.StartsWith("while"))
                    && trimmed.Contains("(") && trimmed.Contains("=")
                    && !trimmed.Contains("==") && !trimmed.Contains("!=")
                    && !trimmed.Contains(">=") && !trimmed.Contains("<="))
                {
                    diagnostics.Add(new { line = i + 1, column = 1, severity = 1, message = "'=='(비교)를 의도하셨나요? ('='는 대입)" });
                }
            }

            // Check bracket balance
            int openBraces = 0, closeBraces = 0;
            foreach (char c in code)
            {
                if (c == '{') openBraces++;
                else if (c == '}') closeBraces++;
            }
            if (openBraces != closeBraces)
            {
                diagnostics.Add(new { line = lines.Length, column = 1, severity = 0,
                    message = string.Format("중괄호 불균형: 열기 {0}개, 닫기 {1}개", openBraces, closeBraces) });
            }

            return diagnostics;
        }

        private void OnEditorReady()
        {
            // 로딩 표시 제거, WebView2 표시
            if (loadingLabel != null)
            {
                loadingLabel.Visible = false;
                this.Controls.Remove(loadingLabel);
                loadingLabel.Dispose();
                loadingLabel = null;
            }
            if (webView != null)
            {
                webView.Visible = true;
                webView.BringToFront();
            }

            // Pending content가 있으면 로드
            if (pendingContent != null)
            {
                SetContentToEditor(pendingContent);
                pendingContent = null;
            }

            // Breakpoint zone 설정
            EnqueueScript(string.Format("window.bridge.setGlyphMargin({0})", _bDisplayBreakPointZone ? "true" : "false"));

            // Breakpoints 복원
            if (arrayBreakPoints.Count > 0)
            {
                UpdateBreakpointsInEditor();
            }

            // Pending completion data 전송
            if (pendingTagsJson != null)
            {
                EnqueueScript(string.Format("window.autobaseCompletionData.tags = {0}", pendingTagsJson));
                pendingTagsJson = null;
            }
            if (pendingMethodsJson != null)
            {
                EnqueueScript(string.Format("window.autobaseCompletionData.methods = {0}", pendingMethodsJson));
                pendingMethodsJson = null;
            }

            // Pending navigation 실행
            if (pendingGotoX.HasValue && pendingGotoY.HasValue)
            {
                EnqueueScript(string.Format("window.bridge.gotoPosition({0}, {1})", pendingGotoY.Value, pendingGotoX.Value));
                pendingGotoX = null;
                pendingGotoY = null;
            }
            else if (pendingGotoPosition.HasValue)
            {
                EnqueueScript(string.Format("window.bridge.gotoOffset({0})", pendingGotoPosition.Value));
                pendingGotoPosition = null;
            }
        }

        #endregion

        #region File Operations

        public void LoadFromFile(string filename)
        {
            _sFilename = filename;

            string source = "";
            try
            {
                source = File.ReadAllText(filename, Encoding.Default);
            }
            catch { }

            LoadBreakPoints(filename);

            if (isEditorReady)
            {
                SetContentToEditor(source);
                _bChangeFlag = false;
                UpdateBreakpointsInEditor();
            }
            else
            {
                pendingContent = source;
            }
        }

        public void LoadFromString(string source)
        {
            if (source == null) source = "";

            if (isEditorReady)
            {
                SetContentToEditor(source);
                _bChangeFlag = false;
            }
            else
            {
                pendingContent = source;
            }
        }

        public bool Save(string filename)
        {
            try
            {
                string content = GetContentFromEditor();
                File.WriteAllText(filename, content, Encoding.Default);

                // Breakpoint 저장
                SaveBreakPoint(filename);

                _bChangeFlag = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Content Get/Set (Sync Bridge)

        private void SetContentToEditor(string text)
        {
            string escaped = MonacoMessageProtocol.EscapeForJs(text);
            EnqueueScript(string.Format("window.bridge.setContent(\"{0}\")", escaped));
        }

        /// <summary>
        /// Monaco에서 전체 내용을 동기적으로 가져온다.
        /// Application.DoEvents()로 메시지 펌프를 유지하면서 대기한다.
        /// 재진입 방지 플래그로 DoEvents 중 중첩 호출을 차단한다.
        /// </summary>
        private string GetContentFromEditor()
        {
            if (!isEditorReady) return pendingContent ?? "";
            if (_isWaitingForSyncResponse) return ""; // 재진입 방지

            _isWaitingForSyncResponse = true;
            try
            {
                string requestId = Guid.NewGuid().ToString("N");
                var tcs = new TaskCompletionSource<string>();
                pendingContentRequests[requestId] = tcs;

                EnqueueScript(string.Format("window.bridge.getContent(\"{0}\")", requestId));

                // 동기 대기 (최대 5초) - UI 메시지 펌프 유지 필요 (WebMessageReceived 수신용)
                DateTime timeout = DateTime.Now.AddSeconds(5);
                while (!tcs.Task.IsCompleted && DateTime.Now < timeout)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }

                if (tcs.Task.IsCompleted)
                    return tcs.Task.Result ?? "";

                // 타임아웃 시 정리
                pendingContentRequests.Remove(requestId);
                return "";
            }
            finally
            {
                _isWaitingForSyncResponse = false;
            }
        }

        /// <summary>
        /// arrayString을 현재 내용으로 구축한다 (호환성).
        /// </summary>
        private void BuildArrayString(string content)
        {
            _arrayString.Clear();
            if (string.IsNullOrEmpty(content))
            {
                _arrayString.Add(new StringBuilder());
                return;
            }

            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.EndsWith("\r"))
                    line = line.Substring(0, line.Length - 1);
                _arrayString.Add(new StringBuilder(line));
            }
        }

        #endregion

        #region Clipboard

        public void Copy()
        {
            EnqueueScript("window.bridge.copy()");
        }

        public void Paste()
        {
            // WebView2에서 프로그래밍 방식 클립보드 붙여넣기가 작동하지 않으므로
            // .NET에서 클립보드를 읽어 직접 텍스트를 삽입한다.
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(text))
                    {
                        string escaped = MonacoMessageProtocol.EscapeForJs(text);
                        EnqueueScript(string.Format("window.bridge.paste(\"{0}\")", escaped));
                        return;
                    }
                }
            }
            catch { }

            // 폴백: 브라우저 기본 붙여넣기 시도
            EnqueueScript("window.bridge.paste()");
        }

        /// <summary>
        /// 텍스트를 현재 커서 위치에 직접 삽입한다.
        /// 클립보드를 사용하지 않는다.
        /// </summary>
        public void InsertText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (!isEditorReady) return;

            string escaped = MonacoMessageProtocol.EscapeForJs(text);
            EnqueueScript(string.Format("window.bridge.insertText(\"{0}\")", escaped));
        }

        public void EditCut()
        {
            EnqueueScript("window.bridge.cut()");
        }

        public void SelectAll()
        {
            EnqueueScript("window.bridge.selectAll()");
        }

        #endregion

        #region Undo/Redo

        public void Undo()
        {
            EnqueueScript("window.bridge.undo()");
        }

        public void Redo()
        {
            EnqueueScript("window.bridge.redo()");
        }

        public bool IsPosibleUndo()
        {
            return _canUndo;
        }

        public bool IsPosibleRedo()
        {
            return _canRedo;
        }

        public bool IsPosibleCopy()
        {
            return _hasSelection;
        }

        public bool IsPosiblePaste()
        {
            return true; // 클립보드에 텍스트가 있으면 항상 가능
        }

        #endregion

        #region Edit

        public void KeyDownDelete()
        {
            EnqueueScript("window.bridge.deleteSelection()");
        }

        #endregion

        #region Font/Config

        public void SetFont(Font font)
        {
            if (font == null) return;
            string family = font.FontFamily.Name;
            // WinForms Font.SizeInPoints (pt) → Monaco fontSize (CSS px) 변환
            // CSS px = pt × 4/3 (96dpi 기준). WebView2가 DPI 스케일링을 자체 처리하므로
            // 96dpi 기준으로 변환하면 된다.
            int size = (int)Math.Round(font.SizeInPoints * 4.0 / 3.0);
            EnqueueScript(string.Format("window.bridge.setFont(\"{0}\", {1})",
                MonacoMessageProtocol.EscapeForJs(family), size));
        }

        public void OnConfigurationChanged()
        {
            // 설정 변경 시 필요한 Monaco 옵션 업데이트
        }

        #endregion

        #region Navigation

        public void GotoViewCursor(int x, int y)
        {
            if (!isEditorReady)
            {
                pendingGotoX = x;
                pendingGotoY = y;
                pendingGotoPosition = null;
                return;
            }
            EnqueueScript(string.Format("window.bridge.gotoPosition({0}, {1})", y, x));
        }

        public void GotoBreakPoint(int x, int y)
        {
            GotoViewCursor(x, y);
            bBreaking = true;
            if (isEditorReady)
                EnqueueScript(string.Format("window.bridge.setDebugLine({0})", y));
        }

        public void GotoPosition(int position)
        {
            if (!isEditorReady)
            {
                pendingGotoPosition = position;
                pendingGotoX = null;
                pendingGotoY = null;
                return;
            }
            EnqueueScript(string.Format("window.bridge.gotoOffset({0})", position));
        }

        public void ScrollToCursor()
        {
            EnqueueScript("window.bridge.scrollToCursor()");
        }

        #endregion

        #region Selection

        public string GetSelectedText()
        {
            if (!isEditorReady) return "";
            if (_isWaitingForSyncResponse) return ""; // 재진입 방지

            _isWaitingForSyncResponse = true;
            try
            {
                string requestId = Guid.NewGuid().ToString("N");
                var tcs = new TaskCompletionSource<string>();
                pendingSelectedTextRequests[requestId] = tcs;

                EnqueueScript(string.Format("window.bridge.getSelectedText(\"{0}\")", requestId));

                DateTime timeout = DateTime.Now.AddSeconds(3);
                while (!tcs.Task.IsCompleted && DateTime.Now < timeout)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }

                if (tcs.Task.IsCompleted)
                    return tcs.Task.Result ?? "";

                pendingSelectedTextRequests.Remove(requestId);
                return "";
            }
            finally
            {
                _isWaitingForSyncResponse = false;
            }
        }

        public int GetCursorPosition()
        {
            if (!isEditorReady) return 0;
            if (_isWaitingForSyncResponse) return 0; // 재진입 방지

            _isWaitingForSyncResponse = true;
            try
            {
                string requestId = Guid.NewGuid().ToString("N");
                var tcs = new TaskCompletionSource<int>();
                pendingPositionRequests[requestId] = tcs;

                EnqueueScript(string.Format("window.bridge.getCursorPosition(\"{0}\")", requestId));

                DateTime timeout = DateTime.Now.AddSeconds(3);
                while (!tcs.Task.IsCompleted && DateTime.Now < timeout)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }

                if (tcs.Task.IsCompleted)
                    return tcs.Task.Result;

                pendingPositionRequests.Remove(requestId);
                return 0;
            }
            finally
            {
                _isWaitingForSyncResponse = false;
            }
        }

        public void SetSelection(int start, int length)
        {
            EnqueueScript(string.Format("window.bridge.setSelection({0}, {1})", start, length));
        }

        #endregion

        #region Breakpoints

        private void ToggleBreakPoint(int line0based)
        {
            bool found = false;
            for (int i = 0; i < arrayBreakPoints.Count; i++)
            {
                if (arrayBreakPoints[i] == line0based)
                {
                    arrayBreakPoints.RemoveAt(i);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                arrayBreakPoints.Add(line0based);
            }

            // 디버그 모드에서 BreakPoint 변경을 서버에 알린다
            if (bDebugStarted)
            {
                DebuggerEditMain.SetBreakPoint(_sFilename, line0based);
            }
        }

        private void UpdateBreakpointsInEditor()
        {
            if (!isEditorReady) return;

            var sb = new StringBuilder("[");
            for (int i = 0; i < arrayBreakPoints.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(arrayBreakPoints[i]);
            }
            sb.Append("]");

            EnqueueScript(string.Format("window.bridge.setBreakpoints({0})", sb.ToString()));
        }

        private void SaveBreakPoint(string sourcefile)
        {
            string filename = Path.ChangeExtension(sourcefile, "BreakPoints");

            if (arrayBreakPoints.Count <= 0)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
            else
            {
                using (TextWriter writer = new StreamWriter(filename))
                {
                    for (int i = 0; i < arrayBreakPoints.Count; i++)
                    {
                        writer.WriteLine("{0}", arrayBreakPoints[i]);
                    }
                }
            }
        }

        private void LoadBreakPoints(string sourcefile)
        {
            arrayBreakPoints.Clear();

            string filename = Path.ChangeExtension(sourcefile, "BreakPoints");

            arrayBreakPoints = ScriptLibMain.LoadBreakPoints(filename);
        }

        public void SetDebugBreakPointByCursor()
        {
            ToggleBreakPoint(_cursorLine);
            UpdateBreakpointsInEditor();
        }

        #endregion

        #region Debug

        public static void DebugStart()
        {
            bDebugStarted = true;
        }

        public bool IsDebugStarted()
        {
            return bDebugStarted;
        }

        public void DebugStop()
        {
            bDebugStarted = false;

            if (bBreaking)
            {
                bBreaking = false;
                EnqueueScript("window.bridge.setDebugLine(-1)");
            }
        }

        public void DebugStepInto()
        {
            if (bBreaking)
            {
                bBreaking = false;
                EnqueueScript("window.bridge.setDebugLine(-1)");
                DebuggerEditMain.SendNextCommand(EnumDebugStep.StepInto);
            }
        }

        public void DebugStepOver()
        {
            if (bBreaking)
            {
                bBreaking = false;
                EnqueueScript("window.bridge.setDebugLine(-1)");
                DebuggerEditMain.SendNextCommand(EnumDebugStep.StepOver);
            }
        }

        public void DebugStepContinue()
        {
            if (bBreaking)
            {
                bBreaking = false;
                EnqueueScript("window.bridge.setDebugLine(-1)");
                DebuggerEditMain.SendNextCommand(EnumDebugStep.StepContinue);
            }
        }

        #endregion

        #region Layout

        public void OnSize()
        {
            // WebView2 HWND 및 CoreWebView2Controller.Bounds를 패널 크기에 맞춰 수동 조정.
            // Pool에서 가져온 WebView2는 pooledHostForm.Controls에 남아있어:
            //   1) WinForms Dock=Fill이 작동하지 않음 (MoveWindow 필요)
            //   2) WinForms 자동 Controller.Bounds 업데이트가 안 됨 (직접 설정 필요)
            // Controller.Bounds가 업데이트되지 않으면 MoveWindow로 HWND를 리사이즈해도
            // WebView2 렌더링 영역은 이전 크기에 머문다.
            if (webView != null && webView.IsHandleCreated && this.IsHandleCreated)
            {
                int w = this.ClientSize.Width;
                int h = this.ClientSize.Height;
                MoveWindow(webView.Handle, 0, 0, w, h, true);

                // CoreWebView2Controller의 Bounds를 명시적으로 설정
                var controller = GetCoreWebView2Controller(webView);
                if (controller != null)
                {
                    controller.Bounds = new Rectangle(0, 0, w, h);
                    controller.NotifyParentWindowPositionChanged();
                }
            }

            // Monaco editor 내부 레이아웃 재계산
            if (isEditorReady)
            {
                EnqueueScript("window.bridge.layout()");
            }
        }

        public void OnTimer()
        {
            // Monaco는 자체 커서 깜빡임을 관리하므로 불필요
        }

        #endregion

        #region Completion Data (Tags / Methods)

        /// <summary>
        /// 태그 자동완성 데이터를 설정한다.
        /// JSON 배열 형식: [{"name":"TagName","type":"AI","description":"desc"}, ...]
        /// </summary>
        public void SetCompletionTags(string json)
        {
            if (!isEditorReady)
            {
                pendingTagsJson = json;
                return;
            }
            EnqueueScript(string.Format("window.autobaseCompletionData.tags = {0}", json));
        }

        /// <summary>
        /// 메서드 자동완성 데이터를 설정한다.
        /// JSON 배열 형식: [{"name":"Screen.GetValue","returnType":"int","signature":"Screen.GetValue(string name)","description":""}, ...]
        /// </summary>
        public void SetCompletionMethods(string json)
        {
            if (!isEditorReady)
            {
                pendingMethodsJson = json;
                return;
            }
            EnqueueScript(string.Format("window.autobaseCompletionData.methods = {0}", json));
        }

        #endregion

        #region GetSourceString (UserControlScriptEditor 호환)

        /// <summary>
        /// 전체 소스 코드를 문자열로 반환한다.
        /// UserControlScriptEditor.GetSourceString()에서 호출됨.
        /// </summary>
        public string GetSourceString()
        {
            string content = GetContentFromEditor();
            BuildArrayString(content);
            return content;
        }

        #endregion

        #region Helper

        // --- Win32 P/Invoke: HWND 파괴 없이 부모 윈도우를 변경한다 ---
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        // --- CoreWebView2Controller 리플렉션 캐시 ---
        private static FieldInfo cachedControllerField = null;
        private static bool controllerFieldSearched = false;

        /// <summary>
        /// Script 실행을 직렬화된 큐에 추가한다.
        /// ContinueWith 체인으로 실행 순서를 보장하며, async void의 레이스 문제를 해결한다.
        /// OnEditorReady()에서 setContent → setGlyphMargin → completion data 순서가 보장된다.
        /// </summary>
        private void EnqueueScript(string script)
        {
            if (webView == null || webView.CoreWebView2 == null) return;

            lock (_scriptLock)
            {
                _scriptQueue = _scriptQueue.ContinueWith(async _ =>
                {
                    try
                    {
                        if (webView != null && webView.CoreWebView2 != null)
                            await webView.CoreWebView2.ExecuteScriptAsync(script);
                    }
                    catch { }
                }, TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();
            }
        }

        /// <summary>
        /// Pool에서 가져온 WebView2의 크기를 수동으로 조정한다.
        /// WebView2는 pooledHostForm.Controls에 남아있어 Dock=Fill이 작동하지 않으므로
        /// OnResize에서 MoveWindow로 크기를 동기화한다.
        /// Fallback(new WebView2) 경로에서는 this.Controls에 추가되어 Dock=Fill이 작동한다.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_isPooledTaken && webView != null && webView.IsHandleCreated && this.IsHandleCreated)
            {
                int w = this.ClientSize.Width;
                int h = this.ClientSize.Height;
                MoveWindow(webView.Handle, 0, 0, w, h, true);

                // CoreWebView2Controller.Bounds도 업데이트 (렌더링 영역 동기화)
                var controller = GetCoreWebView2Controller(webView);
                if (controller != null)
                {
                    controller.Bounds = new Rectangle(0, 0, w, h);
                    controller.NotifyParentWindowPositionChanged();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(
                "[MonacoEditor] Dispose called: disposing={0}, webView={1}, pooledWebView={2}",
                disposing,
                webView != null ? "exists" : "null",
                pooledWebView != null ? "exists" : "null"));

            if (disposing)
            {
                // FormClosing 구독 해제
                if (parentForm != null)
                {
                    parentForm.FormClosing -= OnParentFormClosing;
                    parentForm = null;
                }

                if (loadingLabel != null)
                {
                    loadingLabel.Dispose();
                    loadingLabel = null;
                }
                if (webView != null)
                {
                    if (isEditorReady && webView.CoreWebView2 != null && pooledWebView == null)
                    {
                        // Pool에 반환 (다음 에디터에서 즉시 재사용)
                        // WebView2는 pooledHostForm.Controls에 유지되므로 base.Dispose에서 파괴되지 않는다
                        System.Diagnostics.Debug.WriteLine("[MonacoEditor] Dispose: returning webView to pool");
                        ReturnWebViewToPool();
                    }
                    else if (!_isPooledTaken)
                    {
                        // Fallback으로 생성된 WebView2 → 직접 파괴
                        if (webView.CoreWebView2 != null)
                            webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
                        webView.Dispose();
                    }
                    // _isPooledTaken이면서 pool에 반환 못한 경우: 이미 pool에 있으므로 무시
                    webView = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// WebView2를 파괴하지 않고 pool에 반환한다.
        /// WebView2는 pooledHostForm.Controls에 유지된 채로 HWND만 이동한다.
        /// ControlCollection 조작 없이 SetParent + SyncController만으로 동작.
        /// </summary>
        private void ReturnWebViewToPool()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Monaco 에디터 내용 클리어 (다음 사용 시 이전 내용이 보이지 않도록)
            try { webView.CoreWebView2.ExecuteScriptAsync("window.bridge.setContent('')"); } catch { }

            // 인스턴스 핸들러 → pool 핸들러 교체
            webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
            webView.CoreWebView2.WebMessageReceived += OnPooledWebMessageReceived;

            EnsurePoolHostForm();

            if (webView.IsHandleCreated && pooledHostForm.IsHandleCreated)
            {
                IntPtr hwnd = webView.Handle;
                SetParent(hwnd, pooledHostForm.Handle);
                ShowWindow(hwnd, SW_HIDE);
                SyncControllerParentWindow(webView, pooledHostForm.Handle);
            }

            pooledWebView = webView;
            pooledEditorReady = true;

            System.Diagnostics.Debug.WriteLine(string.Format(
                "[MonacoEditor] ReturnToPool via SetParent: {0}ms", sw.ElapsedMilliseconds));
        }

        /// <summary>
        /// Pool host Form이 유효한지 확인하고 없으면 생성한다.
        /// 한 번 생성되면 앱 종료까지 유지된다.
        /// </summary>
        private static void EnsurePoolHostForm()
        {
            if (pooledHostForm != null && !pooledHostForm.IsDisposed) return;

            pooledHostForm = new Form();
            pooledHostForm.FormBorderStyle = FormBorderStyle.None;
            pooledHostForm.ShowInTaskbar = false;
            pooledHostForm.Size = new Size(800, 600);
            pooledHostForm.StartPosition = FormStartPosition.Manual;
            pooledHostForm.Location = new Point(-32000, -32000);
            pooledHostForm.Show();
        }

        /// <summary>
        /// WebView2 내부의 CoreWebView2Controller를 리플렉션으로 가져온다.
        /// Controller의 ParentWindow를 업데이트해야 입력(마우스/키보드)이 정상 라우팅된다.
        /// </summary>
        private static CoreWebView2Controller GetCoreWebView2Controller(WebView2 wv)
        {
            if (!controllerFieldSearched)
            {
                controllerFieldSearched = true;
                try
                {
                    // WebView2 내부에서 CoreWebView2Controller 타입의 필드를 찾는다
                    foreach (var field in typeof(WebView2).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        if (field.FieldType == typeof(CoreWebView2Controller))
                        {
                            cachedControllerField = field;
                            break;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(string.Format(
                        "[MonacoEditor] Controller field: {0}",
                        cachedControllerField != null ? cachedControllerField.Name : "NOT FOUND"));
                }
                catch { }
            }
            if (cachedControllerField == null) return null;
            try { return cachedControllerField.GetValue(wv) as CoreWebView2Controller; }
            catch { return null; }
        }

        /// <summary>
        /// CoreWebView2Controller의 ParentWindow를 새 부모 HWND로 업데이트한다.
        /// SetParent로 HWND를 이동한 후 반드시 호출해야 입력이 정상 동작한다.
        /// </summary>
        private static void SyncControllerParentWindow(WebView2 wv, IntPtr newParentHwnd)
        {
            try
            {
                var controller = GetCoreWebView2Controller(wv);
                if (controller != null)
                {
                    controller.ParentWindow = newParentHwnd;
                    controller.NotifyParentWindowPositionChanged();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[MonacoEditor] SyncControllerParentWindow failed: " + ex.Message);
            }
        }

        #endregion
    }
}
