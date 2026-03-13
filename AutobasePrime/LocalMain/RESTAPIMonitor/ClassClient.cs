using AutoLibLocal;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace AutobaseRESTAPIMonitor
{
    public class ClassClient
    {
        // 각 클라이언트별 CancellationTokenSource 관리
        private static readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new Dictionary<string, CancellationTokenSource>();
        private static readonly Dictionary<string, CancellationTokenSource> _parsingTokens = new Dictionary<string, CancellationTokenSource>();
        private static readonly Dictionary<string, Task> _runningTasks = new Dictionary<string, Task>();
        private static readonly Dictionary<string, int> _dailyCallCounts = new Dictionary<string, int>();
        private static readonly Dictionary<string, DateTime> _lastCallDates = new Dictionary<string, DateTime>();
        private static readonly object _lockObject = new object();

        private static readonly ConcurrentQueue<(string Response, string ClientTitle)> _responseQueue =
            new ConcurrentQueue<(string, string)>();
        private static readonly Dictionary<string, Task> _parsingTasks = new Dictionary<string, Task>();
        private static readonly Dictionary<string, ConcurrentQueue<(string response, int CallCount)>> _clientQueues =
    new Dictionary<string, ConcurrentQueue<(string, int)>>();

        private static readonly Dictionary<string, bool> TagExistenceCache = new Dictionary<string, bool>();

        private static bool _initialized;

        public static void Init()
        {
            try
            {
                if (_initialized) return;

                // Config 경로 초기화 (RestApiMonitorService에서 관리)
                RestApiMonitorService.InitConfig();

                // RestApiMonitorService에 실행 delegate 등록
                RestApiMonitorService.InitEngine = Init;
                RestApiMonitorService.StartClientTask = StartClientTask;
                RestApiMonitorService.StopClient = StopClient;
                RestApiMonitorService.StartAllClients = StartAllClients;
                RestApiMonitorService.StopAllClients = StopAllClients;
                RestApiMonitorService.IsClientRunning = IsClientRunning;

                _initialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    GlobalSettings.IsKorean ?
                    $"초기화 오류: {ex.Message}\nConfigPath: {RestApiMonitorService.ConfigPath}" :
                    $"Initialization Error: {ex.Message}\nConfigPath: {RestApiMonitorService.ConfigPath}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void InitializeTagCache(IEnumerable<string> tags)
        {
            TagExistenceCache.Clear(); // 기존 캐시 초기화

            foreach (var tag in tags)
            {
                if (!TagExistenceCache.ContainsKey(tag))
                {
                    TagExistenceCache[tag] = TagLib.IsTagExist(tag);
                }
            }
        }

        private static void UpdateTagCache(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                // 태그가 캐시에 없거나 갱신이 필요하면 다시 체크
                if (!TagExistenceCache.ContainsKey(tag))
                {
                    TagExistenceCache[tag] = TagLib.IsTagExist(tag);
                }
            }
        }

        public static bool IsTagExistWithCache(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return false;

            if (TagExistenceCache.TryGetValue(tag, out bool exists))
            {
                return exists;
            }

            // 캐시에 없으면 외부 메서드 호출 후 추가
            exists = TagLib.IsTagExist(tag);
            TagExistenceCache[tag] = exists;

            return exists;
        }


        private static async Task RunClientTask(ApiClientInfo clientInfo, CancellationToken ct)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30); // 기본 100초 → 30초로 단축

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        ct.ThrowIfCancellationRequested(); // 명시적으로 취소 확인
                        Debug.WriteLine($"[RunClientTask] {clientInfo.Url}: 새로운 요청 시작");

                        // 호출 횟수 관리
                        var today = DateTime.Today;
                        lock (_lockObject)
                        {
                            if (!_lastCallDates.ContainsKey(clientInfo.Title) || _lastCallDates[clientInfo.Title].Date != today)
                            {
                                _dailyCallCounts[clientInfo.Title] = 0;  // 새로운 날로 초기화
                                _lastCallDates[clientInfo.Title] = today;
                            }
                            _dailyCallCounts[clientInfo.Title]++;
                        }

                        string invalidFormat;
                        if (!UrlFormatter.ValidateUrlFormat(clientInfo.Url, out invalidFormat))
                        {
                            if (_isShuttingDown)
                                return;

                            RestApiMonitorService.RaiseClientStatusUpdate(
                                clientInfo.Title,
                                null, //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                GlobalSettings.IsKorean ?
                                    $"오류: 잘못된 URL 포맷 - {invalidFormat}" :
                                    $"Error: Invalid URL format - {invalidFormat}"
                            );
                            return;
                        }

                        // URL에 포맷 적용
                        string formattedUrl = UrlFormatter.FormatUrl(clientInfo.Url);

                        // 기존 헤더 초기화
                        client.DefaultRequestHeaders.Clear();

                        foreach (var header in clientInfo.Headers)
                        {
                            string headerValue = header.Encrypt ? EncryptionHelper.Decrypt(header.Value) : header.Value;
                            client.DefaultRequestHeaders.Add(header.Name, headerValue);
                        }

                        // CancellationToken 사용: 취소 시 즉시 HTTP 요청 중단
                        var httpResponse = await client.GetAsync(formattedUrl, ct);
                        string response = await httpResponse.Content.ReadAsStringAsync();

                        // 클라이언트 전용 큐에 응답 데이터 추가, 호출횟수 포함
                        lock (_lockObject)
                        {
                            if (!_clientQueues.ContainsKey(clientInfo.Title))
                            {
                                _clientQueues[clientInfo.Title] = new ConcurrentQueue<(string Response, int CallCount)>();
                            }
                            else
                            {
                                // 기존 큐를 비웁니다.
                                while (_clientQueues[clientInfo.Title].TryDequeue(out _)) { }
                            }

                            _clientQueues[clientInfo.Title].Enqueue((response, _dailyCallCounts[clientInfo.Title]));
                        }

                        // _clientQueues[clientInfo.Title].Enqueue(response);
                        if (_isShuttingDown)
                            return;

                        RestApiMonitorService.RaiseClientStatusUpdate(
                            clientInfo.Title,
                            null,
                            GlobalSettings.IsKorean ? $"정상(호출횟수: {_dailyCallCounts[clientInfo.Title]})" :
                            $"Normal (Calls: {_dailyCallCounts[clientInfo.Title]})"
                        );
                    }
                    catch (Exception ex)
                    {
                        if (_isShuttingDown)
                            return;

                        RestApiMonitorService.RaiseClientStatusUpdate(
                            clientInfo.Title,
                            null, //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                             GlobalSettings.IsKorean ? $"오류: {ex.Message}": $"Error: {ex.Message}"
                        );
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(clientInfo.IntervalSeconds), ct);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        public static void StartResponseProcessor(ApiClientInfo clientInfo)
        {
            lock (_lockObject)
            {
                if (_parsingTasks.ContainsKey(clientInfo.Title)) return;

                // 클라이언트별 파싱 작업 시작
                var cts = new CancellationTokenSource();
                _parsingTokens[clientInfo.Title] = cts;

                // Task.Run으로 감싸서 반드시 ThreadPool에서 시작 (UI 스레드 블로킹 방지)
                var task = Task.Run(() => ProcessClientQueue(clientInfo, cts.Token));
                _parsingTasks[clientInfo.Title] = task;
            }
        }
        /// <summary>
        ///  JSON 파싱을 처리하는 별도 태스크
        ///  값 파싱 시 AutoLibLocal.SharedTag 를 이용하기 위해 간단하게 문자열 값 처리.
        ///  값이 여러 개 일 경우, 콤마 구분자로 구분하여 @StringSplit() 함수로 쉽게 분리하도록 처리.
        ///  숫자형, 불리언 등 단순 값은 그대로 사용.
        ///  배열이나 여러 토큰의 결과는 JSON 배열 형식([])으로 처리.
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        private static async Task ProcessClientQueue(ApiClientInfo clientInfo, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    (string Response, int CallCount)? queuedItem = null;

                    lock (_lockObject)
                    {
                        if (_clientQueues.ContainsKey(clientInfo.Title) &&
                            _clientQueues[clientInfo.Title].TryDequeue(out var dequeuedItem))
                        {
                            queuedItem = dequeuedItem;
                        }
                    }

                    if (queuedItem != null)
                    {
                        try
                        {
                            var (response, callCount) = queuedItem.Value;
                            // JSON 시작 위치 찾기 (배열 또는 객체)
                            int jsonStartIndex = response.IndexOfAny(new char[] { '[', '{' });
                            if (jsonStartIndex == -1)
                            {
                                if (_isShuttingDown)
                                    return;

                                RestApiMonitorService.RaiseClientStatusUpdate(
                                     clientInfo.Title,
                                     null, //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                     GlobalSettings.IsKorean ?
                                         "오류: 유효한 JSON 데이터를 찾을 수 없습니다" :
                                         "Error: No valid JSON data found"
                                 );
                                return;
                            }

                            // 시작 문자에 따른 종료 문자 결정
                            char endChar = response[jsonStartIndex] == '[' ? ']' : '}';

                            // JSON 끝 위치 찾기
                            int jsonEndIndex = response.LastIndexOf(endChar);
                            if (jsonEndIndex == -1)
                            {
                                if (_isShuttingDown)
                                    return;

                                RestApiMonitorService.RaiseClientStatusUpdate(
                                   clientInfo.Title,
                                   null, //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                   GlobalSettings.IsKorean ?
                                       "오류: 유효한 JSON 데이터를 찾을 수 없습니다" :
                                       "Error: No valid JSON data found"
                               );
                                return;
                            }
                            // 주석을 제외한 순수 JSON 문자열 추출
                            string jsonStr = response.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1);

                            // JSON 파싱
                            var jsonToken = JToken.Parse(jsonStr);  // JObject 대신 JToken 사용 (배열과 객체 모두 처리 가능)
                            var values = new Dictionary<string, JToken>();

                            // 배치 쓰기용: 태그 값을 모아서 1회 IPC로 전송
                            var batchTags = new List<KeyValuePair<string, string>>();

                            foreach (var rule in clientInfo.ParseRules)
                            {
                                var tokens = jsonToken.SelectTokens(rule.JsonPath);
                                JToken resultValue;  // 결과를 저장할 JToken
                                string tagValue;     // 태그에 저장할 문자열 값

                                bool bExistRuleTag = IsTagExistWithCache(rule.Tag);

                                if (tokens.Any())
                                {
                                    if (tokens.Count() == 1)
                                    {
                                        var token = tokens.First();
                                        switch (token.Type)
                                        {
                                            case JTokenType.Integer:
                                            case JTokenType.Float:
                                            case JTokenType.Boolean:
                                                resultValue = token;
                                                tagValue = token.ToString();
                                                break;
                                            case JTokenType.String:
                                                string processedString = ProcessStringValue(token.Value<string>());
                                                resultValue = JToken.FromObject(processedString);
                                                tagValue = processedString;
                                                break;
                                            case JTokenType.Array:
                                            case JTokenType.Object:
                                                resultValue = ProcessTokenValue(token);
                                                tagValue = resultValue.ToString(Formatting.None);
                                                break;
                                            default:
                                                string processedValue = ProcessStringValue(token.ToString());
                                                resultValue = JToken.FromObject(processedValue);
                                                tagValue = processedValue;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        var tokenArray = new JArray();
                                        foreach (var token in tokens)
                                        {
                                            switch (token.Type)
                                            {
                                                case JTokenType.Integer:
                                                case JTokenType.Float:
                                                case JTokenType.Boolean:
                                                    tokenArray.Add(token);
                                                    break;
                                                case JTokenType.String:
                                                    tokenArray.Add(JToken.FromObject(ProcessStringValue(token.Value<string>())));
                                                    break;
                                                case JTokenType.Array:
                                                case JTokenType.Object:
                                                    tokenArray.Add(ProcessTokenValue(token));
                                                    break;
                                                default:
                                                    tokenArray.Add(JToken.FromObject(ProcessStringValue(token.ToString())));
                                                    break;
                                            }
                                        }
                                        resultValue = tokenArray;
                                        tagValue = tokenArray.ToString(Formatting.None);
                                    }

                                    // 태그에 저장할 값 제한
                                    tagValue = LimitTextString(tagValue);
                                    if (bExistRuleTag) batchTags.Add(new KeyValuePair<string, string>(rule.Tag, tagValue));
                                }
                                else
                                {
                                    string notFoundValue = clientInfo.UseNotFoundText ? clientInfo.NotFoundText : "";
                                    resultValue = JToken.FromObject(notFoundValue);
                                    if (clientInfo.UseNotFoundText)
                                    {
                                        batchTags.Add(new KeyValuePair<string, string>(rule.Tag, notFoundValue));
                                    }
                                }

                                // Dictionary에 JToken 저장
                                values[rule.Tag] = resultValue;
                            }

                            // 배치 쓰기: N개 태그를 1회 IPC 왕복으로 처리
                            if (batchTags.Count > 0)
                                AutoLibLocal.SharedTag.SetCurrBatch(batchTags);

                            // 호출 횟수 및 파싱 결과를 로그에 저장
                            SaveParsedData(clientInfo.Title, values, callCount);

                            if (_isShuttingDown)
                                return;

                            RestApiMonitorService.RaiseClientStatusUpdate(
                                clientInfo.Title,
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                null
                            );

                        }
                        catch (Exception ex)
                        {
                            // RestApiMonitorService.RaiseClientStatusUpdate(clientInfo.Title, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), $"Parsing Error: {ex.Message}");

                            if (_isShuttingDown)
                                return;

                            RestApiMonitorService.RaiseClientStatusUpdate(clientInfo.Title, null, $"Parsing Error: {ex.Message}");
                        }
                    }
                    else
                    {
                        await Task.Delay(100); // 큐가 비어있을 때 대기
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("ProcessClientQueue canceled");
            }
        }

        private static string ProcessStringValue(string value) //html 태그 제거
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // HTML 태그 제거
            string processed = System.Text.RegularExpressions.Regex.Replace(value, "<.*?>", string.Empty);
            // HTML 엔티티 디코딩
            processed = System.Web.HttpUtility.HtmlDecode(processed);
            // 연속된 공백 제거 //20250212 제거
            //processed = System.Text.RegularExpressions.Regex.Replace(processed, @"\s+", " ");

            return processed.Trim();
        }

        // JToken 처리를 위한 헬퍼 메서드 추가
        private static JToken ProcessTokenValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var newObj = new JObject();
                    foreach (var prop in token.Children<JProperty>())
                    {
                        newObj[prop.Name] = ProcessTokenValue(prop.Value);
                    }
                    return newObj;

                case JTokenType.Array:
                    var newArr = new JArray();
                    foreach (var item in token.Children())
                    {
                        newArr.Add(ProcessTokenValue(item));
                    }
                    return newArr;

                case JTokenType.String:
                    return JToken.FromObject(ProcessStringValue(token.Value<string>()));

                default:
                    return token;
            }
        }

        private static void SaveParsedData(string title, Dictionary<string, string> values, int callCount)
        {
            var logData = new
            {
                Title = title,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CallCount = callCount,
                Values = values
            };

            // JSON 형식으로 저장
            string safeTitle = FileNameHelper.GetSafeFileName(title);   
            string logPath = Path.Combine(RestApiMonitorService.ConfigPath, $"{safeTitle}_log.txt");
            //string jsonLog = JsonConvert.SerializeObject(logData, Formatting.Indented);
            string jsonLog = JsonConvert.SerializeObject(logData,
       Formatting.Indented,
       new JsonSerializerSettings
       {
           StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
           Formatting = Formatting.Indented
       });
            File.WriteAllText(logPath, jsonLog, Encoding.UTF8);
        }

        private static void SaveParsedData(string title, Dictionary<string, JToken> values, int callCount)
        {
            var valuesObj = new JObject();
            foreach (var kv in values)
            {
                valuesObj[kv.Key] = kv.Value;
            }

            var logData = new JObject
            {
                ["Title"] = title,
                ["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["CallCount"] = callCount,
                ["Values"] = valuesObj
            };

            string safeTitle = FileNameHelper.GetSafeFileName(title);
            string logPath = Path.Combine(RestApiMonitorService.ConfigPath, $"{safeTitle}_log.txt");

            string jsonLog = logData.ToString(Formatting.Indented);
            File.WriteAllText(logPath, jsonLog, Encoding.UTF8);
        }

        public static void StartClientTask(ApiClientInfo clientInfo)
        {
            lock (_lockObject)
            {
                if (_runningTasks.ContainsKey(clientInfo.Title))
                    return; // 이미 실행 중이면 무시

                // 기존 작업이 있으면 취소
                StopClient(clientInfo.Title);

                // IsRunning이 true인 경우에만 작업 시작
                if (clientInfo.IsRunning)
                {
                    var cts = new CancellationTokenSource();
                    _cancellationTokens[clientInfo.Title] = cts;

                    // Task.Run으로 감싸서 반드시 ThreadPool에서 시작 (UI 스레드 블로킹 방지)
                    var task = Task.Run(() => RunClientTask(clientInfo, cts.Token));
                    _runningTasks[clientInfo.Title] = task;

                    // 큐 프로세서 시작
                    StartResponseProcessor(clientInfo);

                    if (_isShuttingDown)
                        return;

                    RestApiMonitorService.RaiseClientStatusUpdate(
                        clientInfo.Title,
                        null,//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        GlobalSettings.IsKorean ? "실행" : "Running"
                    );
                }  //GlobalSettings.IsKorean? "실행중": "Running" : GlobalSettings.IsKorean ? "중지됨" : "Stopped"
                else
                {
                    if (_isShuttingDown)
                        return;

                    RestApiMonitorService.RaiseClientStatusUpdate(
                        clientInfo.Title,
                        null,//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                       GlobalSettings.IsKorean ? "중지" : "Stopped"
                    );
                }
            }
        }
        public static void StopClient(string title)
        {
            Debug.WriteLine($"[StopClient] 시작: {title}");

            CancellationTokenSource ctsToDispose = null;
            CancellationTokenSource parsingCtsToDispose = null;

            lock (_lockObject)
            {
                if (_cancellationTokens.ContainsKey(title))
                {
                    ctsToDispose = _cancellationTokens[title];
                    ctsToDispose.Cancel(); // 취소 신호 발생

                    // 참조만 제거 (Wait 하지 않음 - UI 스레드 블로킹/데드락 방지)
                    _cancellationTokens.Remove(title);
                    _runningTasks.Remove(title);
                    Debug.WriteLine($"[StopClient] {title}: 취소 신호 발생, 참조 제거 완료");
                }

                // 파싱 작업 정리
                if (_parsingTokens.ContainsKey(title))
                {
                    parsingCtsToDispose = _parsingTokens[title];
                    parsingCtsToDispose.Cancel();
                    _parsingTokens.Remove(title);
                }

                if (_parsingTasks.ContainsKey(title))
                {
                    if (_clientQueues.ContainsKey(title))
                    {
                        while (_clientQueues[title].TryDequeue(out _)) { }
                        _clientQueues.Remove(title);
                    }
                    _parsingTasks.Remove(title);
                    Debug.WriteLine($"[StopClient] {title}: 파싱 작업 정리 완료");
                }
            }

            // CTS Dispose는 lock 밖에서, 백그라운드로 지연 처리
            // (백그라운드 태스크가 CancellationToken을 아직 사용 중일 수 있음)
            if (ctsToDispose != null || parsingCtsToDispose != null)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(3000); // 태스크가 취소를 인식할 시간 확보
                    try { ctsToDispose?.Dispose(); } catch { }
                    try { parsingCtsToDispose?.Dispose(); } catch { }
                });
            }

            Debug.WriteLine($"[StopClient] 완료: {title}");
        }

        public static void StartAllClients()
        {
            var configs = RestApiMonitorService.LoadAllConfigs();
            foreach (var config in configs)
            {
                StartClientTask(config);
            }
        }

        private static volatile bool _isShuttingDown;

        public static void StopAllClients()
        {
            _isShuttingDown = true;

            lock (_lockObject)
            {
                foreach (var cts in _cancellationTokens.Values)
                {
                    try
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }
                    catch { }
                }
                _cancellationTokens.Clear();
                _runningTasks.Clear();

                foreach (var cts in _parsingTokens.Values)
                {
                    try
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }
                    catch { }
                }

                _parsingTokens.Clear();
                _parsingTasks.Clear();
                _clientQueues.Clear();
            }
        }

        // 클라이언트 실행 상태 확인 메서드 추가
        public static bool IsClientRunning(string title)
        {
            lock (_lockObject)
            {
                return _runningTasks.ContainsKey(title);
            }
        }

        private static string LimitStringLength(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static string LimitNumberString(string value)
        {
            return LimitStringLength(value, 300);
        }

        private static string LimitTextString(string value)
        {
            return LimitStringLength(value, 2048);
        }





    }


}
