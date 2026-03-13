using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutobaseRESTAPIMonitor
{
    /// <summary>
    /// API 클라이언트 설정 정보
    /// </summary>
    public class ApiClientInfo
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int IntervalSeconds { get; set; }
        public List<ParseRule> ParseRules { get; set; } = new List<ParseRule>();
        public bool IsRunning { get; set; } = true;
        public string NotFoundText { get; set; } = "Not Found";
        public bool UseNotFoundText { get; set; } = false;
        public List<(string Name, string Value, bool Encrypt)> Headers { get; set; } = new List<(string Name, string Value, bool Encrypt)>();
    }

    /// <summary>
    /// JSON 파싱 규칙
    /// </summary>
    public class ParseRule
    {
        public string JsonPath { get; set; }
        public string Tag { get; set; }

        public ParseRule Clone()
        {
            return new ParseRule
            {
                JsonPath = this.JsonPath,
                Tag = this.Tag,
            };
        }
    }

    /// <summary>
    /// UI 표시용 API 정보
    /// </summary>
    public class ApiInfo
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int IntervalSeconds { get; set; }
        public string Status { get; set; }
        public string LastUpdate { get; set; }
    }

    /// <summary>
    /// 다국어 설정
    /// </summary>
    public static class GlobalSettings
    {
        public static bool IsKorean { get; set; }

        public static void Initialize()
        {
            string currentCulture = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
            IsKorean = currentCulture.StartsWith("ko") || currentCulture.Equals("ko-kr");
        }
    }

    /// <summary>
    /// REST API Monitor 서비스.
    /// Config 파일 I/O는 직접 구현 (Studio/LocalMain 공통).
    /// 실행 작업은 Delegate 기반 (LocalMain에서만 등록).
    /// </summary>
    public static class RestApiMonitorService
    {
        // ── Config 경로 ──
        private static string _configPath;
        private static string _configFile;
        private static bool _configInitialized;

        /// <summary>
        /// Config 경로 초기화 (Studio/LocalMain 공통)
        /// </summary>
        public static void InitConfig()
        {
            if (_configInitialized) return;

            if (string.IsNullOrWhiteSpace(TotalConfig.sDirWorkProject)) return;

            _configPath = Path.Combine(TotalConfig.sDirWorkProject, "RESTAPIClient");
            _configFile = Path.Combine(_configPath, "APIClient.lst");

            if (!Directory.Exists(_configPath))
                Directory.CreateDirectory(_configPath);

            _configInitialized = true;
        }

        public static string ConfigFile
        {
            get
            {
                if (!_configInitialized) InitConfig();
                return _configFile;
            }
        }

        public static string ConfigPath
        {
            get
            {
                if (!_configInitialized) InitConfig();
                return _configPath;
            }
        }

        // ── Config 작업 (항상 사용 가능 - 직접 구현) ──

        public static List<ApiClientInfo> LoadAllConfigs()
        {
            if (!_configInitialized) InitConfig();

            if (string.IsNullOrEmpty(_configFile) || !File.Exists(_configFile))
                return new List<ApiClientInfo>();

            var configs = new List<ApiClientInfo>();
            ApiClientInfo currentClient = null;
            bool inParseRules = false;
            bool inHeaders = false;

            string[] lines = File.ReadAllLines(_configFile);
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                string[] parts = trimmedLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                switch (parts[0].Trim())
                {
                    case "Client":
                        if (parts[1].Trim() == "Begin")
                            currentClient = new ApiClientInfo();
                        else if (parts[1].Trim() == "End" && currentClient != null)
                        {
                            configs.Add(currentClient);
                            currentClient = null;
                        }
                        break;

                    case "Headers":
                        if (parts[1].Trim() == "Begin") inHeaders = true;
                        else if (parts[1].Trim() == "End") inHeaders = false;
                        break;

                    case "Header" when inHeaders && currentClient != null:
                        currentClient.Headers.Add((parts[1].Trim(), parts[2].Trim(), bool.Parse(parts[3].Trim())));
                        break;

                    case "ParseRules":
                        if (parts[1].Trim() == "Begin") inParseRules = true;
                        else if (parts[1].Trim() == "End") inParseRules = false;
                        break;

                    case "Rule" when inParseRules && currentClient != null && parts.Length >= 3:
                        currentClient.ParseRules.Add(new ParseRule
                        {
                            JsonPath = parts[1].Trim(),
                            Tag = parts[2].Trim()
                        });
                        break;

                    case "Title" when currentClient != null:
                        currentClient.Title = parts[1].Trim();
                        break;

                    case "Url" when currentClient != null:
                        currentClient.Url = parts[1].Trim();
                        break;

                    case "IntervalSeconds" when currentClient != null:
                        if (int.TryParse(parts[1].Trim(), out int interval))
                            currentClient.IntervalSeconds = interval;
                        break;

                    case "IsRunning" when currentClient != null:
                        if (bool.TryParse(parts[1].Trim(), out bool isRunning))
                            currentClient.IsRunning = isRunning;
                        break;

                    case "NotFoundText" when currentClient != null:
                        currentClient.NotFoundText = parts[1].Trim();
                        break;

                    case "UseNotFoundText" when currentClient != null:
                        if (bool.TryParse(parts[1].Trim(), out bool useNotFound))
                            currentClient.UseNotFoundText = useNotFound;
                        break;
                }
            }

            return configs;
        }

        public static void SaveApiClient(ApiClientInfo clientInfo)
        {
            if (!_configInitialized) InitConfig();

            var configs = LoadAllConfigs();
            configs.RemoveAll(x => x.Title == clientInfo.Title);

            if (!string.IsNullOrWhiteSpace(clientInfo.Title))
                configs.Add(clientInfo);

            configs.RemoveAll(x => string.IsNullOrWhiteSpace(x.Title) || string.IsNullOrWhiteSpace(x.Url));

            SaveAllConfigs(configs);
        }

        public static void SaveAllConfigs(List<ApiClientInfo> configs)
        {
            if (!_configInitialized) InitConfig();

            if (string.IsNullOrEmpty(_configPath) || string.IsNullOrEmpty(_configFile)) return;

            if (!Directory.Exists(_configPath))
                Directory.CreateDirectory(_configPath);

            configs.RemoveAll(x => string.IsNullOrWhiteSpace(x.Title) || string.IsNullOrWhiteSpace(x.Url));

            using (StreamWriter writer = new StreamWriter(_configFile))
            {
                foreach (var config in configs)
                {
                    writer.WriteLine("Client,Begin,");
                    writer.WriteLine($"\tTitle,{config.Title},");
                    writer.WriteLine($"\tUrl,{config.Url},");
                    writer.WriteLine($"\tIntervalSeconds,{config.IntervalSeconds},");
                    writer.WriteLine($"\tIsRunning,{config.IsRunning},");
                    writer.WriteLine($"\tNotFoundText,{config.NotFoundText},");
                    writer.WriteLine($"\tUseNotFoundText,{config.UseNotFoundText},");

                    writer.WriteLine("Headers,Begin,");
                    foreach (var header in config.Headers)
                    {
                        writer.WriteLine($"\tHeader,{header.Name},{header.Value},{header.Encrypt},");
                    }
                    writer.WriteLine("Headers,End,");

                    if (config.ParseRules != null && config.ParseRules.Count > 0)
                    {
                        writer.WriteLine("ParseRules,Begin,");
                        foreach (var rule in config.ParseRules)
                        {
                            writer.WriteLine($"\tRule,{rule.JsonPath},{rule.Tag},");
                        }
                        writer.WriteLine("ParseRules,End,");
                    }

                    writer.WriteLine("Client,End,");
                }
            }
        }

        // ── 실행 작업 (LocalMain에서만 사용 가능 - Delegate) ──
        public static Action InitEngine;
        public static Action<ApiClientInfo> StartClientTask;
        public static Action<string> StopClient;
        public static Action StartAllClients;
        public static Action StopAllClients;
        public static Func<string, bool> IsClientRunning;

        // ── 이벤트 ──
        private static Action<string, string, string> _clientStatusUpdate;

        public static event Action<string, string, string> ClientStatusUpdate
        {
            add { _clientStatusUpdate += value; }
            remove { _clientStatusUpdate -= value; }
        }

        /// <summary>
        /// 실행 모드 여부 (LocalMain에서 delegate가 등록되었는지)
        /// </summary>
        public static bool IsExecutionMode => StartClientTask != null;

        /// <summary>
        /// 상태 업데이트 이벤트 발생 (ClassClient에서 호출)
        /// </summary>
        public static void RaiseClientStatusUpdate(string title, string lastUpdate, string status)
        {
            _clientStatusUpdate?.Invoke(title, lastUpdate, status);
        }
    }

    /// <summary>
    /// Control.InvokeAsync 확장 메서드
    /// </summary>
    public static class ControlExtensions
    {
        public static Task InvokeAsync(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                return Task.Factory.FromAsync(
                    control.BeginInvoke(action),
                    control.EndInvoke);
            }
            action();
            return Task.CompletedTask;
        }
    }
}
