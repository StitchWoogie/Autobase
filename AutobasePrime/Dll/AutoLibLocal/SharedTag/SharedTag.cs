using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace AutoLibLocal
{
    /// <summary>
    /// Summary description for SharedTag.
    /// </summary>
    public class SharedTag
    {
        private static SharedTagClient _client;
        private static readonly object _clientLock = new object();
        private static bool _initialized = false;
        private static DateTime _lastConnectionAttempt = DateTime.MinValue;
        private static int _connectionFailureCount = 0;
        private const int MAX_RETRY_INTERVAL_MS = 30000; // 최대 30초

        // 사용자 정보 캐시 (매 SetCurr 호출마다 DNS 조회 방지)
        private static string _cachedUserName;
        private static string _cachedComputerName;
        private static string _cachedIPAddress;
        private static bool _userInfoCached = false;
        private static readonly object _userInfoLock = new object();

        public SharedTag()
        {
            Initialize();
        }

        /// <summary>
        /// 클라이언트 초기화 (생성자에서 자동 호출)
        /// </summary>
        private static void Initialize()
        {
            if (_initialized && _client != null)
                return;

            lock (_clientLock)
            {
                if (!_initialized || _client == null)
                {
                    try
                    {
                        _client = new SharedTagClient();
                        _initialized = true;
                        _connectionFailureCount = 0;
                        System.Diagnostics.Debug.WriteLine("[SharedTag] 클라이언트 연결 성공");
                    }
                    catch (InvalidOperationException ex)
                    {
                        // 서버 미실행 예외
                        _client = null;
                        _initialized = false;
                        System.Diagnostics.Debug.WriteLine($"[SharedTag] 서버 연결 실패: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 클라이언트 종료
        /// </summary>
        public static void Shutdown()
        {
            lock (_clientLock)
            {
                _client?.Dispose();
                _client = null;
                _initialized = false;
                _connectionFailureCount = 0;
            }
        }

        /// <summary>
        /// 클라이언트 확인 및 자동 재연결
        /// </summary>
        private static bool EnsureClient()
        {
            // 이미 연결된 경우
            if (_client != null)
                return true;

            // 재연결 시도 간격 제어 (지수 백오프). 5회 이상 부터는 30초.
            TimeSpan timeSinceLastAttempt = DateTime.Now - _lastConnectionAttempt;
            int cappedFailureCount = Math.Min(_connectionFailureCount, 5);
            int retryInterval = Math.Min(1000 << cappedFailureCount, MAX_RETRY_INTERVAL_MS);

            if (timeSinceLastAttempt.TotalMilliseconds < retryInterval)
            {
                // 아직 재시도 시간이 안됨
                return false;
            }

            lock (_clientLock)
            {
                // lock 진입 후 다시 확인 (double-checked locking)
                if (_client != null)
                    return true;

                _lastConnectionAttempt = DateTime.Now;

                try
                {
                    System.Diagnostics.Debug.WriteLine($"[SharedTag] 서버 재연결 시도 ({_connectionFailureCount + 1}번째)");

                    _client = new SharedTagClient();
                    _initialized = true;
                    _connectionFailureCount = 0;

                    System.Diagnostics.Debug.WriteLine("[SharedTag] 서버 재연결 성공");
                    return true;
                }
                catch (InvalidOperationException)
                {
                    _client = null;
                    _connectionFailureCount++;

                    System.Diagnostics.Debug.WriteLine(
                        $"[SharedTag] 서버 재연결 실패 (다음 시도: {retryInterval / 1000.0:F1}초 후)");

                    return false;
                }
            }
        }

        /// <summary>
        /// 연결 끊김 처리
        /// </summary>
        private static void HandleDisconnection()
        {
            lock (_clientLock)
            {
                if (_client != null)
                {
                    try
                    {
                        _client.Dispose();
                    }
                    catch { }

                    _client = null;
                    _initialized = false;

                    System.Diagnostics.Debug.WriteLine("[SharedTag] 서버 연결 끊김 감지");
                }
            }
        }


        public static byte GetSum8(string s)
        {
            byte c = 0;

            for (int i = 0; i < s.Length; i++)
            {
                c += (byte)(s[i] % 256);
                c += (byte)(s[i] / 256);
            }

            return c;
        }

        public static bool GetCurr(string tag, ref string curr)
        {
            if (!EnsureClient())
            {
                curr = "";
                return false;
            }

            try
            {

                if (_client.ReadTag(tag, out double numValue, out string strValue,
                              out TagQuality quality, out bool isValid, out DateTime lastUpdate))
                {
                    // 문자열 값이 있으면 문자열 반환, 없으면 숫자 반환
                    curr = !string.IsNullOrEmpty(strValue) ? strValue : numValue.ToString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 읽기 예외: {ex.Message}");
                HandleDisconnection();
            }

            curr = "";
            return false;
        }


        /// <summary>
        /// 태그 현재값 가져오기 (숫자)
        /// </summary>
        public static bool GetCurr(string tag, out double value)
        {
            value = 0;

            if (!EnsureClient())
                return false;

            try
            {
                return _client.ReadTagSimple(tag, out value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 읽기 예외: {ex.Message}");
                HandleDisconnection();
                return false;
            }
        }


        /// <summary>
        /// writing FLAG가 off되었을때만 값을 읽어온다 (호환성 유지, MMF에서는 항상 최신값 반환)
        /// </summary>
        public static bool GetCurrAfterWrite(string tag, ref string curr)
        {
            // MMF에서는 항상 최신값을 반환
            return GetCurr(tag, ref curr);
        }


        public static uint MakeCode(uint seed1, uint seed2)
        {
            uint code = seed1;

            code += 0x7F43;
            code ^= seed2;
            code ^= 0x5678;
            code += seed2;

            return code;
        }
        /// <summary>
        /// 태그 값 설정 (double)
        /// </summary>
        public static bool SetCurr(string tag, double val)
        {
            if (!EnsureClient())
                return false;

            try
            {
                EnsureUserInfoCached();
                return _client.WriteTag(tag, val, _cachedUserName, _cachedIPAddress, _cachedComputerName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 쓰기 실패: {ex.Message}");
                HandleDisconnection();
                return false;
            }
        }

        /// <summary>
        /// 태그 값 설정 (string, 사용자 정보 포함)
        /// </summary>
        public static bool SetCurr(string tag, string val, string user, string ip, string computer)
        {
            if (!EnsureClient())
                return false;

            try
            {
                return _client.WriteTag(tag, val, user, ip, computer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 쓰기 예외: {ex.Message}");
                HandleDisconnection();
                return false;
            }
        }

        /// <summary>
        /// 태그 값 설정 (string)
        /// </summary>
        public static bool SetCurr(string tag, string val)
        {
            if (!EnsureClient())
                return false;

            try
            {
                EnsureUserInfoCached();
                return _client.WriteTag(tag, val, _cachedUserName, _cachedIPAddress, _cachedComputerName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 쓰기 실패: {ex.Message}");
                HandleDisconnection();
                return false;
            }
        }


        /// <summary>
        /// 배치 태그 쓰기 — N개 태그를 1회 IPC 왕복으로 처리
        /// 반환값: 성공한 태그 수
        /// </summary>
        public static int SetCurrBatch(List<KeyValuePair<string, string>> tags)
        {
            if (tags == null || tags.Count == 0) return 0;

            if (!EnsureClient())
                return 0;

            try
            {
                EnsureUserInfoCached();
                return _client.WriteTagBatch(tags, _cachedUserName, _cachedIPAddress, _cachedComputerName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 배치 쓰기 실패: {ex.Message}");
                HandleDisconnection();
                return 0;
            }
        }

        /// <summary>
        /// 지연 설정 (호환성 유지)
        /// </summary>
        public static bool SetCurrDelaySec(string tag, object val, int delay_sec)
        {
            if (val.GetType() == typeof(string))
            {
                return SetCurr(tag, val.ToString());
            }
            else
            {
                if (double.TryParse(val.ToString(), out double dval))
                {
                    return SetCurr(tag, dval);
                }
                return false;
            }
        }

        /// <summary>
        /// 사용자 정보 캐시 초기화 (최초 1회만 DNS 조회)
        /// </summary>
        private static void EnsureUserInfoCached()
        {
            if (_userInfoCached) return;

            lock (_userInfoLock)
            {
                if (_userInfoCached) return;

                _cachedUserName = Environment.UserName;
                _cachedComputerName = Environment.MachineName;
                _cachedIPAddress = "";

                try
                {
                    var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            _cachedIPAddress = ip.ToString();
                            break;
                        }
                    }
                }
                catch
                {
                    // IP 가져오기 실패 시 빈 문자열 유지
                }

                _userInfoCached = true;
            }
        }

        /// <summary>
        /// 로컬 IP 주소 가져오기 (캐시 사용)
        /// </summary>
        private static string GetLocalIPAddress()
        {
            EnsureUserInfoCached();
            return _cachedIPAddress;
        }

        /// <summary>
        /// 배치 태그 읽기 (고성능)
        /// </summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<string, TagReadResult>
            ReadMultipleTags(string[] tagNames)
        {
            if (!EnsureClient())
            {
                return new System.Collections.Concurrent.ConcurrentDictionary<string, TagReadResult>();
            }

            try
            {
                return _client.ReadMultipleTags(tagNames);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 배치 읽기 예외: {ex.Message}");
                HandleDisconnection();
                return new System.Collections.Concurrent.ConcurrentDictionary<string, TagReadResult>();
            }
        }

        /// <summary>
        /// Quality 포함 상세 읽기
        /// </summary>
        public static bool ReadTagWithQuality(string tag, out double numericValue, out string stringValue,
                                             out TagQuality quality, out bool isValid, out DateTime lastUpdate)
        {
            numericValue = 0;
            stringValue = "";
            quality = TagQuality.Bad;
            isValid = false;
            lastUpdate = DateTime.MinValue;

            if (!EnsureClient())
                return false;
            try
            {
                return _client.ReadTag(tag, out numericValue, out stringValue,
                                     out quality, out isValid, out lastUpdate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SharedTag] 상세 읽기 예외: {ex.Message}");
                HandleDisconnection();
                return false;
            }
        }

        /// <summary>
        /// 서버 연결 상태 확인
        /// </summary>
        public static bool IsServerConnected()
        {
            return _client != null;
        }

        /// <summary>
        /// 연결 실패 횟수 확인
        /// </summary>
        public static int GetConnectionFailureCount()
        {
            return _connectionFailureCount;
        }

        /// <summary>
        /// 수동 재연결 시도
        /// </summary>
        public static bool TryReconnect()
        {
            lock (_clientLock)
            {
                _lastConnectionAttempt = DateTime.MinValue; // 재시도 간격 초기화
                return EnsureClient();
            }
        }

        /// <summary>
        /// Quality 포함 상세 읽기 (알람 레벨, 디바이스 ID 포함)
        /// </summary>
        public static bool ReadTagFull(string tag, out TagReadResult result)
        {
            result = new TagReadResult();
            EnsureClient();

            if (_client.ReadTag(tag, out double numVal, out string strVal,
                              out TagQuality qual, out bool valid, out DateTime lastUpd))
            {
                result.NumericValue = numVal;
                result.StringValue = strVal;
                result.Quality = qual;
                result.IsValid = valid;
                result.LastUpdate = lastUpd;
                return true;
            }

            return false;
        }
    }
}
