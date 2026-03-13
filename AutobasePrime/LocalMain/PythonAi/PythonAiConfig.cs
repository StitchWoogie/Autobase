using System;
using System.IO;
using System.Windows.Forms;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 설정.
    /// Phase 1: 기본 연결 설정.
    /// Phase 2-4: 엔진 설정 파일 경로, 로그 포맷 추가.
    /// </summary>
    public static class PythonAiConfig
    {
        // ==================== 기본 설정 ====================

        /// <summary>기능 활성화 여부 (기본: false)</summary>
        public static bool Enabled = false;

        /// <summary>TCP 호스트</summary>
        public static string Host = "127.0.0.1";

        /// <summary>TCP 포트</summary>
        public static int Port = 5678;

        /// <summary>Python 실행 파일 경로</summary>
        public static string PythonExePath = "python";

        /// <summary>Python AI Engine main.py 경로 (자동 감지)</summary>
        public static string EngineScriptPath = "";

        /// <summary>기본 요청 타임아웃 (ms)</summary>
        public static int RequestTimeoutMs = 10000;

        /// <summary>Ping 주기 (ms)</summary>
        public static int PingIntervalMs = 5000;

        /// <summary>프로세스 시작 대기 시간 (ms)</summary>
        public static int ProcessStartTimeoutMs = 15000;

        /// <summary>
        /// Python 엔진과 협상된 메시지 버전.
        /// system/ping 응답에서 supportedMessageVersions를 확인하여 설정.
        /// 0이면 아직 협상 전 (기본 CURRENT_MESSAGE_VERSION 사용).
        /// </summary>
        public static int NegotiatedMessageVersion = 0;

        // ==================== Phase 2-4 확장 설정 ====================

        /// <summary>
        /// Python 엔진 설정 파일 경로 (engine_config.json).
        /// main.py에 --config 인자로 전달됨.
        /// </summary>
        public static string EngineConfigPath = "";

        /// <summary>Python 로그 포맷 ("text" 또는 "json")</summary>
        public static string LogFormat = "text";

        /// <summary>Python 로그 레벨 ("DEBUG", "INFO", "WARNING", "ERROR")</summary>
        public static string LogLevel = "INFO";

        /// <summary>배치 분석 기본 타임아웃 (ms)</summary>
        public static int BatchTimeoutMs = 60000;

        /// <summary>학습 작업 기본 타임아웃 (ms)</summary>
        public static int TrainingTimeoutMs = 300000;

        /// <summary>스크립트 실행 기본 타임아웃 (ms)</summary>
        public static int ScriptTimeoutMs = 30000;

        // ==================== 초기화 ====================

        /// <summary>설정 초기화 — Application.StartupPath 기준으로 경로 자동 설정</summary>
        public static void Initialize()
        {
            string basePath = Application.StartupPath;

            if (string.IsNullOrEmpty(EngineScriptPath))
            {
                EngineScriptPath = Path.Combine(basePath, "python_ai_engine", "main.py");
            }

            if (string.IsNullOrEmpty(EngineConfigPath))
            {
                string configCandidate = Path.Combine(basePath, "python_ai_engine", "engine_config.json");
                if (File.Exists(configCandidate))
                {
                    EngineConfigPath = configCandidate;
                }
            }
        }

        // ==================== 유틸리티 ====================

        /// <summary>상태 요약 문자열</summary>
        public static string GetStatusText()
        {
            if (!Enabled)
                return "Python AI Engine: Disabled";

            return String.Format("Python AI Engine: {0}:{1}", Host, Port);
        }

        /// <summary>상세 설정 문자열 (디버그용)</summary>
        public static string GetDetailText()
        {
            return String.Format(
                "PythonAi Config:\n" +
                "  Enabled={0}, Host={1}, Port={2}\n" +
                "  PythonExe={3}\n" +
                "  ScriptPath={4}\n" +
                "  ConfigPath={5}\n" +
                "  RequestTimeout={6}ms, PingInterval={7}ms\n" +
                "  LogFormat={8}, LogLevel={9}",
                Enabled, Host, Port,
                PythonExePath,
                EngineScriptPath,
                EngineConfigPath,
                RequestTimeoutMs, PingIntervalMs,
                LogFormat, LogLevel);
        }
    }
}
