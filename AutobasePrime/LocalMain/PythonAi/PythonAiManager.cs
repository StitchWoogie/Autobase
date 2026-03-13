using AutoLibLocal;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 핵심 매니저.
    /// Phase 1: 프로세스 관리 + 모니터 루프 + 지수 백오프.
    /// Phase 2-4: IPythonManager 구현, CallOptions 지원, 편의 API.
    /// </summary>
    internal static class PythonAiManager
    {
        private static PythonAiTcpConnection _connection;
        private static PythonAiRequestManager _reqMgr;
        private static PythonAiHealthMonitor _health;
        private static Process _pythonProcess;
        private static ChildProcessJob _job;

        private static Task _monitorTask;
        private static CancellationTokenSource _cts;
        private static readonly object _lock = new object();

        // Phase 2-4: IPythonManager 래퍼 인스턴스
        private static PythonAiManagerWrapper _wrapper;

        /// <summary>현재 엔진 상태</summary>
        public static PythonAiState State
        {
            get { return _health != null ? _health.CurrentState : PythonAiState.Disconnected; }
        }

        /// <summary>연결 여부</summary>
        public static bool IsConnected
        {
            get
            {
                var conn = _connection;
                return conn != null && conn.IsConnected;
            }
        }

        /// <summary>상태 변경 이벤트</summary>
        public static event EventHandler<PythonAiStateChangedEventArgs> StateChanged;

        /// <summary>IPythonManager 인터페이스 인스턴스 반환</summary>
        public static IPythonManager GetInterface()
        {
            if (_wrapper == null)
                _wrapper = new PythonAiManagerWrapper();
            return _wrapper;
        }

        // ==================== Lifecycle ====================

        /// <summary>엔진 시작</summary>
        public static void Start()
        {
            if (_monitorTask != null)
                return;

            if (!PythonAiConfig.Enabled)
            {
                Debug.WriteLine("PythonAi: Disabled in config");
                return;
            }

            PythonAiConfig.Initialize();

            _health = new PythonAiHealthMonitor();
            _health.StateChanged += (s, e) =>
            {
                try { StateChanged?.Invoke(null, e); } catch { }
            };

            _reqMgr = new PythonAiRequestManager();
            _cts = new CancellationTokenSource();

            _monitorTask = Task.Run(() => MonitorLoop(_cts.Token));

            SmLog.Message(LogLevel.INFO, LogCategory.SYSTEM, "Python AI Engine 시작");
            PythonAiLogBridge.Info("Engine", "Python AI Engine started");
        }

        /// <summary>엔진 정지</summary>
        public static void Stop()
        {
            _cts?.Cancel();
            _monitorTask = null;

            SafeDisposeConnection();
            KillPythonProcess();
            try { _job?.Dispose(); } catch { }
            _job = null;
            _reqMgr?.CancelAll("Engine stopping");

            _health?.Reset();
            _health = null;
            _reqMgr = null;

            SmLog.Message(LogLevel.INFO, LogCategory.SYSTEM, "Python AI Engine 정지");
            PythonAiLogBridge.Info("Engine", "Python AI Engine stopped");
        }

        // ==================== Public API ====================

        /// <summary>
        /// Python AI 서비스 호출 (기본 옵션).
        /// </summary>
        /// <param name="service">서비스명 (예: "predict/power")</param>
        /// <param name="payload">요청 데이터</param>
        /// <param name="timeoutMs">타임아웃 (0이면 기본값 사용)</param>
        /// <returns>응답 메시지 (실패 시 에러 메시지 포함)</returns>
        public static async Task<PythonAiMessage> CallAsync(string service, object payload, int timeoutMs = 0)
        {
            if (timeoutMs <= 0)
                timeoutMs = PythonAiConfig.RequestTimeoutMs;

            var conn = _connection;
            var reqMgr = _reqMgr;
            var health = _health;

            if (conn == null || !conn.IsConnected || reqMgr == null)
            {
                return new PythonAiMessage
                {
                    Ok = false,
                    Error = "ENGINE_UNAVAILABLE: Python AI Engine이 연결되지 않았습니다."
                };
            }

            try
            {
                var request = PythonAiMessage.CreateRequest(service, payload);
                var response = await reqMgr.SendAsync(conn, request, timeoutMs).ConfigureAwait(false);

                if (health != null)
                {
                    if (response.IsSuccess)
                        health.OnSuccess();
                    else
                        health.OnFailure(response.Error);
                }

                return response;
            }
            catch (TimeoutException ex)
            {
                health?.OnFailure("Timeout: " + service);
                return new PythonAiMessage { Ok = false, Error = "TIMEOUT: " + ex.Message };
            }
            catch (Exception ex)
            {
                health?.OnFailure(ex.Message);
                return new PythonAiMessage { Ok = false, Error = "WORKER_ERROR: " + ex.Message };
            }
        }

        /// <summary>
        /// Python AI 서비스 호출 (확장 옵션).
        /// Phase 2-4: 우선순위, 컨텍스트, 재시도, Fire-and-Forget 지원.
        /// </summary>
        public static async Task<PythonAiMessage> CallAsync(string service, object payload, PythonCallOptions options)
        {
            if (options == null)
                options = PythonCallOptions.Default;

            int timeoutMs = options.TimeoutMs > 0 ? options.TimeoutMs : PythonAiConfig.RequestTimeoutMs;

            var conn = _connection;
            var reqMgr = _reqMgr;
            var health = _health;

            if (conn == null || !conn.IsConnected || reqMgr == null)
            {
                return new PythonAiMessage
                {
                    Ok = false,
                    Error = "ENGINE_UNAVAILABLE: Python AI Engine이 연결되지 않았습니다."
                };
            }

            // Fire-and-Forget 모드
            if (options.FireAndForget)
            {
                try
                {
                    var request = PythonAiMessage.CreateRequest(service, payload, options);
                    await conn.SendAsync(request).ConfigureAwait(false);
                    return new PythonAiMessage { Id = request.Id, Ok = true, Result = "SENT" };
                }
                catch (Exception ex)
                {
                    return new PythonAiMessage { Ok = false, Error = "SEND_ERROR: " + ex.Message };
                }
            }

            // 재시도 루프
            int attempts = 0;
            int maxAttempts = Math.Max(1, options.RetryCount + 1);
            PythonAiMessage lastResponse = null;

            while (attempts < maxAttempts)
            {
                attempts++;
                try
                {
                    var request = PythonAiMessage.CreateRequest(service, payload, options);
                    var response = await reqMgr.SendAsync(conn, request, timeoutMs).ConfigureAwait(false);

                    if (health != null)
                    {
                        if (response.IsSuccess)
                            health.OnSuccess();
                        else
                            health.OnFailure(response.Error);
                    }

                    // 성공이면 바로 반환
                    if (response.IsSuccess)
                        return response;

                    lastResponse = response;

                    // 재시도 가능한 에러인지 확인
                    if (attempts < maxAttempts && IsRetryableError(response.Error))
                    {
                        await Task.Delay(options.RetryDelayMs).ConfigureAwait(false);
                        continue;
                    }

                    return response;
                }
                catch (TimeoutException ex)
                {
                    health?.OnFailure("Timeout: " + service);
                    lastResponse = new PythonAiMessage { Ok = false, Error = "TIMEOUT: " + ex.Message };

                    if (attempts < maxAttempts)
                    {
                        await Task.Delay(options.RetryDelayMs).ConfigureAwait(false);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    health?.OnFailure(ex.Message);
                    lastResponse = new PythonAiMessage { Ok = false, Error = "WORKER_ERROR: " + ex.Message };

                    if (attempts < maxAttempts)
                    {
                        await Task.Delay(options.RetryDelayMs).ConfigureAwait(false);
                        continue;
                    }
                }
            }

            return lastResponse ?? new PythonAiMessage { Ok = false, Error = "UNKNOWN_ERROR" };
        }

        /// <summary>Ping 호출 + 메시지 버전 협상</summary>
        public static async Task<bool> PingAsync()
        {
            try
            {
                var response = await CallAsync("system/ping", null, 3000).ConfigureAwait(false);
                if (response == null || !response.IsSuccess)
                    return false;

                // 메시지 버전 협상 (최초 1회)
                if (PythonAiConfig.NegotiatedMessageVersion == 0
                    && response.SupportedMessageVersions != null
                    && response.SupportedMessageVersions.Length > 0)
                {
                    int best = 0;
                    for (int i = 0; i < response.SupportedMessageVersions.Length; i++)
                    {
                        int v = response.SupportedMessageVersions[i];
                        if (v <= PythonAiMessage.CURRENT_MESSAGE_VERSION && v > best)
                            best = v;
                    }
                    PythonAiConfig.NegotiatedMessageVersion =
                        best > 0 ? best : PythonAiMessage.CURRENT_MESSAGE_VERSION;

                    PythonAiLogBridge.Info("Protocol",
                        String.Format("Message version negotiated: {0}", PythonAiConfig.NegotiatedMessageVersion));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ==================== Phase 2-4 편의 API ====================

        /// <summary>시스템 상태 조회</summary>
        public static Task<PythonAiMessage> GetStatusAsync()
        {
            return CallAsync("system/status", null, 5000);
        }

        /// <summary>시스템 메트릭 조회</summary>
        public static Task<PythonAiMessage> GetMetricsAsync()
        {
            return CallAsync("system/metrics", null, 5000);
        }

        /// <summary>모델 목록 조회</summary>
        public static Task<PythonAiMessage> GetModelsAsync()
        {
            return CallAsync("system/models", null, 5000);
        }

        /// <summary>건강 상태 조회</summary>
        public static Task<PythonAiMessage> GetHealthAsync()
        {
            return CallAsync("system/health", null, 5000);
        }

        /// <summary>전력 예측 (Phase 1 MVP → Phase 5: permissions 포함)</summary>
        public static Task<PythonAiMessage> PredictPowerAsync(double currentKw, PythonCallOptions options = null)
        {
            var payload = new { current_kw = currentKw };
            return CallAsync("predict/power", payload, options ?? PythonCallOptions.RealtimePredict);
        }

        /// <summary>트렌드 분석 (Phase 2+)</summary>
        public static Task<PythonAiMessage> AnalyzeTrendAsync(double[] values, PythonCallOptions options = null)
        {
            var payload = new { values = values };
            return CallAsync("analysis/trend", payload, options ?? PythonCallOptions.BatchAnalysis);
        }

        /// <summary>상관 분석 (Phase 2+)</summary>
        public static Task<PythonAiMessage> AnalyzeCorrelationAsync(
            double[] valuesX, double[] valuesY, PythonCallOptions options = null)
        {
            var payload = new { values_x = valuesX, values_y = valuesY };
            return CallAsync("analysis/correlation", payload, options ?? PythonCallOptions.BatchAnalysis);
        }

        /// <summary>스크립트 실행 (Phase 3+)</summary>
        public static Task<PythonAiMessage> ExecuteScriptAsync(
            string code, object scriptPayload = null, PythonCallOptions options = null)
        {
            var payload = new { code = code, payload = scriptPayload };
            return CallAsync("script/execute", payload, options ?? PythonCallOptions.ScriptExecution);
        }

        /// <summary>학습 시작 (Phase 4)</summary>
        public static Task<PythonAiMessage> StartTrainingAsync(
            string modelName, object config = null, PythonCallOptions options = null)
        {
            var payload = new { model = modelName, config = config };
            return CallAsync("train/start", payload, options ?? PythonCallOptions.Training);
        }

        /// <summary>학습 상태 조회 (Phase 4)</summary>
        public static Task<PythonAiMessage> GetTrainingStatusAsync(string jobId = null)
        {
            var payload = jobId != null ? new { jobId = jobId } : null;
            return CallAsync("train/status", (object)payload, 5000);
        }

        /// <summary>학습 취소 (Phase 4)</summary>
        public static Task<PythonAiMessage> CancelTrainingAsync(string jobId)
        {
            var payload = new { jobId = jobId };
            return CallAsync("train/cancel", payload, 5000);
        }

        // ==================== Monitor Loop ====================

        private static async Task MonitorLoop(CancellationToken ct)
        {
            int delayMs = 1000;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (_connection == null || !_connection.IsConnected)
                    {
                        if (_health != null && _health.CanAttempt())
                        {
                            _health.OnConnecting();
                            EnsureProcessRunning();
                            bool connected = await TryConnectAsync().ConfigureAwait(false);
                            delayMs = connected ? PythonAiConfig.PingIntervalMs : Math.Min(delayMs * 2, 10000);
                        }
                        else
                        {
                            delayMs = 5000;
                        }
                    }
                    else
                    {
                        // 주기적 ping
                        bool ok = await PingAsync().ConfigureAwait(false);
                        if (ok)
                        {
                            delayMs = PythonAiConfig.PingIntervalMs;
                        }
                        else
                        {
                            delayMs = 1000;
                        }
                    }
                }
                catch
                {
                    delayMs = Math.Min(delayMs * 2, 10000);
                }

                try { await Task.Delay(delayMs, ct).ConfigureAwait(false); }
                catch { }
            }
        }

        // ==================== Connection ====================

        private static async Task<bool> TryConnectAsync()
        {
            if (_connection != null && _connection.IsConnected)
                return true;

            SafeDisposeConnection();

            var conn = new PythonAiTcpConnection(PythonAiConfig.Host, PythonAiConfig.Port);

            conn.MessageReceived += (s, msg) =>
            {
                try { _reqMgr?.OnResponse(msg); } catch { }
            };

            conn.Disconnected += (s, e) =>
            {
                lock (_lock)
                {
                    if (ReferenceEquals(_connection, s))
                    {
                        SafeDisposeConnection();
                        _health?.OnDisconnected();
                        _reqMgr?.CancelAll("Connection lost");
                    }
                }
            };

            bool ok = await conn.ConnectAsync().ConfigureAwait(false);
            if (!ok)
            {
                conn.Dispose();
                return false;
            }

            lock (_lock)
            {
                if (_connection != null)
                {
                    conn.Dispose();
                    return true;
                }
                _connection = conn;
            }

            _health?.OnSuccess();
            Debug.WriteLine("PythonAi: Connected");
            PythonAiLogBridge.Info("Connection", "Connected to Python AI Engine");
            return true;
        }

        // ==================== Process Management ====================

        private static void EnsureProcessRunning()
        {
            try
            {
                // 이미 실행 중이면 스킵
                if (_pythonProcess != null && !_pythonProcess.HasExited)
                    return;

                _pythonProcess = null;

                string scriptPath = PythonAiConfig.EngineScriptPath;
                if (!File.Exists(scriptPath))
                {
                    Debug.WriteLine(String.Format("PythonAi: Script not found: {0}", scriptPath));
                    return;
                }

                // 설정 파일 경로 (있으면 전달)
                string configArg = "";
                string configPath = PythonAiConfig.EngineConfigPath;
                if (!string.IsNullOrEmpty(configPath) && File.Exists(configPath))
                {
                    configArg = String.Format(" --config \"{0}\"", configPath);
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = PythonAiConfig.PythonExePath,
                    Arguments = String.Format("\"{0}\" --host {1} --port {2}{3}",
                        scriptPath, PythonAiConfig.Host, PythonAiConfig.Port, configArg),
                    UseShellExecute = false, // ← 핸들 상속 활성화 , 프로그램 종료 시 같이 종료해야함. 종료안된 경우 재실행 시 재실행 해야함.
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(scriptPath)
                };

                _pythonProcess = Process.Start(startInfo);

                if (_pythonProcess != null)
                {
                    // Job Object에 등록 → 부모(LocalMain) 종료 시 자식(python)도 자동 종료
                    // 디버거 중지, Task Manager Kill 등 비정상 종료에서도 동작
                    if (_job == null)
                        _job = new ChildProcessJob();
                    _job.AssignProcess(_pythonProcess);

                    // stdout/stderr 비동기 읽기 (블로킹 방지)
                    _pythonProcess.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            Debug.WriteLine("PythonAi [OUT]: " + e.Data);
                    };
                    _pythonProcess.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            Debug.WriteLine("PythonAi [ERR]: " + e.Data);
                    };
                    _pythonProcess.BeginOutputReadLine();
                    _pythonProcess.BeginErrorReadLine();

                    SmLog.Message(LogLevel.INFO, LogCategory.SYSTEM,
                        String.Format("Python AI Engine 프로세스 시작 (PID: {0})", _pythonProcess.Id));
                    PythonAiLogBridge.Info("Engine",
                        String.Format("Process started (PID: {0})", _pythonProcess.Id));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("PythonAi: Failed to start process: {0}", ex.Message));
                SmLog.Message(LogLevel.ERROR, LogCategory.SYSTEM,
                    String.Format("Python AI Engine 프로세스 시작 실패: {0}", ex.Message));
                PythonAiLogBridge.Error("Engine",
                    String.Format("Process start failed: {0}", ex.Message));
            }
        }

        private static void KillPythonProcess()
        {
            try
            {
                if (_pythonProcess != null && !_pythonProcess.HasExited)
                {
                    // 먼저 graceful shutdown 시도
                    try
                    {
                        var conn = _connection;
                        if (conn != null && conn.IsConnected)
                        {
                            var shutdownMsg = PythonAiMessage.CreateRequest("system/shutdown", null);
                            conn.SendAsync(shutdownMsg).Wait(2000);
                            _pythonProcess.WaitForExit(3000);
                        }
                    }
                    catch { }

                    // 아직 살아있으면 강제 종료
                    if (!_pythonProcess.HasExited)
                    {
                        _pythonProcess.Kill();
                        _pythonProcess.WaitForExit(3000);
                    }

                    Debug.WriteLine("PythonAi: Process killed");
                }
            }
            catch { }
            finally
            {
                try { _pythonProcess?.Dispose(); } catch { }
                _pythonProcess = null;
            }
        }

        // ==================== Helpers ====================

        private static bool IsRetryableError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return false;

            // 재시도 가능한 에러 패턴
            return error.Contains("TIMEOUT")
                || error.Contains("ENGINE_UNAVAILABLE")
                || error.Contains("BACKPRESSURE");
        }

        private static void SafeDisposeConnection()
        {
            lock (_lock)
            {
                try { _connection?.Dispose(); } catch { }
                _connection = null;
            }
        }
    }

    // ==================== IPythonManager 래퍼 ====================

    /// <summary>
    /// PythonAiManager static 클래스를 IPythonManager 인터페이스로 래핑.
    /// DI 및 테스트 용도.
    /// </summary>
    internal sealed class PythonAiManagerWrapper : IPythonManager
    {
        public PythonAiState State
        {
            get { return PythonAiManager.State; }
        }

        public bool IsConnected
        {
            get { return PythonAiManager.IsConnected; }
        }

        public Task<PythonAiMessage> CallAsync(string service, object payload, int timeoutMs = 0)
        {
            return PythonAiManager.CallAsync(service, payload, timeoutMs);
        }

        public Task<PythonAiMessage> CallAsync(string service, object payload, PythonCallOptions options)
        {
            return PythonAiManager.CallAsync(service, payload, options);
        }

        public Task<bool> PingAsync()
        {
            return PythonAiManager.PingAsync();
        }

        public Task<PythonAiMessage> GetStatusAsync()
        {
            return PythonAiManager.GetStatusAsync();
        }

        public Task<PythonAiMessage> GetMetricsAsync()
        {
            return PythonAiManager.GetMetricsAsync();
        }

        public Task<PythonAiMessage> GetModelsAsync()
        {
            return PythonAiManager.GetModelsAsync();
        }

        public Task<PythonAiMessage> GetHealthAsync()
        {
            return PythonAiManager.GetHealthAsync();
        }
    }
}
