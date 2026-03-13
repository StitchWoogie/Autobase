using AutoLibLocal;
using System;
using System.Diagnostics;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 초기화/종료 진입점.
    /// C_init.ViewProgrammStart/End에서 호출된다.
    /// </summary>
    public static class CheckEnginePythonAi
    {
        /// <summary>초기화</summary>
        public static void Initialize()
        {
            try
            {
                // 레지스트리에서 설정 로드
                LoadConfigFromRegistry();

                if (!PythonAiConfig.Enabled)
                {
                    Debug.WriteLine("PythonAi: Disabled");
                    return;
                }

                PythonAiManager.Start();
            }
            catch (Exception ex)
            {
                SmLog.Message(LogLevel.ERROR, LogCategory.SYSTEM,
                    String.Format("Python AI Engine 초기화 실패: {0}", ex.Message));
            }
        }

        /// <summary>종료</summary>
        public static void Shutdown()
        {
            try
            {
                PythonAiManager.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("PythonAi: Shutdown error: {0}", ex.Message));
            }
        }

        /// <summary>레지스트리에서 PythonAiConfig 로드</summary>
        private static void LoadConfigFromRegistry()
        {
            try
            {
                PythonAiConfig.Enabled = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiEnabled", false);
                PythonAiConfig.Host = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiHost", "127.0.0.1");
                PythonAiConfig.Port = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiPort", 5678);
                PythonAiConfig.PythonExePath = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiExePath", "python");
                PythonAiConfig.EngineScriptPath = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiScriptPath", "");
                PythonAiConfig.EngineConfigPath = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiConfigPath", "");
                PythonAiConfig.RequestTimeoutMs = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiRequestTimeout", 10000);
                PythonAiConfig.PingIntervalMs = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiPingInterval", 5000);
                PythonAiConfig.BatchTimeoutMs = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiBatchTimeout", 60000);
                PythonAiConfig.TrainingTimeoutMs = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiTrainingTimeout", 300000);
                PythonAiConfig.ScriptTimeoutMs = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiScriptTimeout", 30000);
                PythonAiConfig.LogLevel = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiLogLevel", "INFO");
                PythonAiConfig.LogFormat = TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "PythonAiLogFormat", "text");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("PythonAi: Registry load error: {0}", ex.Message));
            }
        }
    }
}
