using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using PortalServerWeb.Library;
using System.Diagnostics;
using System.Web.Services;
using NetTools;
using System.Configuration;

namespace PortalServerWeb.AutoWeb.Service
{
    public class GuidHeartbeatManager
    {
        private static readonly Dictionary<string, ClientConnectionInfo> _activeConnections = new Dictionary<string, ClientConnectionInfo>();
        private static readonly object _lock = new object();
        private static Timer _cleanupTimer;

        // 설정에서 타임아웃 값을 읽어오는 프로퍼티
        private static int UserTimeOutMinutes
        {
            get
            {
                try
                {
                    int timeout;
                    string timeoutValue = ConfigurationManager.AppSettings["UserTimeOut"];
                    if (!string.IsNullOrEmpty(timeoutValue) && int.TryParse(timeoutValue, out timeout))
                    {
                        return timeout > 0 ? timeout : 2; // 0 이하면 기본값 2분
                    }
                }
                catch (Exception ex)
                {
                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                        string.Format("UserTimeOut config read error: {0}, using default 2 minutes", ex.Message));
                }
                return 2; // 기본값 2분
            }
        }

        static GuidHeartbeatManager()
        {
            // 30초마다 만료된 연결 정리
            _cleanupTimer = new Timer(new TimerCallback(CleanupExpiredConnections), null,
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            // 초기 설정값 로깅
            try
            {
                string errMsg;
                string initMsg = Tools.IsLangKorean()
                    ? string.Format("GuidHeartbeatManager 초기화 - 사용자 타임아웃: {0}분", UserTimeOutMinutes)
                    : string.Format("GuidHeartbeatManager initialized - User timeout: {0} minutes", UserTimeOutMinutes);

                ClassMain.SaveLog(out errMsg, EnumLogType.Normal, initMsg);
            }
            catch { }
        }

        public class ClientConnectionInfo
        {
            public string ClientGuid { get; set; }
            public string LoginMode { get; set; }
            public string Username { get; set; }
            public string IPAddress { get; set; }
            public DateTime LastHeartbeat { get; set; }
            public DateTime LoginTime { get; set; }
            public string UserAgent { get; set; }
        }

        /// <summary>
        /// 클라이언트 GUID로 하트비트 업데이트
        /// </summary>
        public static bool UpdateHeartbeat(string clientGuid, string ipAddress)
        {
            if (string.IsNullOrEmpty(clientGuid))
                return false;

            try
            {
                lock (_lock)
                {
                    bool connectionExists = _activeConnections.ContainsKey(clientGuid);

                    // 디버깅 정보 수집
                    string debugMsg = string.Format(
                        "UpdateHeartbeat Debug - ClientGuid: {0}, IP: {1}, Connection: {2}, Count: {3}",
                        clientGuid,
                        ipAddress,
                        connectionExists,
                        _activeConnections.Count
                    );

                    string errMsg;
                    //ClassMain.SaveLog(out errMsg, EnumLogType.Normal, debugMsg);

                    // 정상적인 하트비트 업데이트
                    if (connectionExists)
                    {
                        var connectionInfo = _activeConnections[clientGuid];
                        connectionInfo.LastHeartbeat = DateTime.Now;

                        // IP 주소가 변경된 경우 업데이트 (모바일 환경 등에서 발생 가능)
                        if (connectionInfo.IPAddress != ipAddress)
                        {
                            string logMsg = Tools.IsLangKorean()
                                ? string.Format("IP 주소 변경 감지 - ClientGuid: {0}, 기존IP: {1}, 새IP: {2}",
                                    clientGuid, connectionInfo.IPAddress, ipAddress)
                                : string.Format("IP address change detected - ClientGuid: {0}, OldIP: {1}, NewIP: {2}",
                                    clientGuid, connectionInfo.IPAddress, ipAddress);

                            ClassMain.SaveLog(out errMsg, EnumLogType.Normal, logMsg);
                            connectionInfo.IPAddress = ipAddress;
                        }

                        return true;
                    }

                    // 연결이 존재하지 않음
                    string notFoundMsg = Tools.IsLangKorean()
                        ? string.Format("UpdateHeartbeat 실패 - 연결이 존재하지 않음. ClientGuid: {0}, IP: {1}",
                            clientGuid, ipAddress)
                        : string.Format("UpdateHeartbeat failed - Connection not found. ClientGuid: {0}, IP: {1}",
                            clientGuid, ipAddress);

                    ClassMain.SaveLog(out errMsg, EnumLogType.Normal, notFoundMsg);

                    return false; // 연결이 존재하지 않음 (재로그인 필요)
                }
            }
            catch (Exception ex)
            {
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                    string.Format("UpdateHeartbeat error - ClientGuid: {0}, Error: {1}",
                    clientGuid, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 새로운 클라이언트 연결 등록
        /// </summary>
        public static bool RegisterConnection(string clientGuid, string loginMode, string username, string ipAddress, string userAgent)
        {
            if (string.IsNullOrEmpty(clientGuid))
            {
                string errMsg;
                string nullGuidMsg = Tools.IsLangKorean()
                    ? "RegisterConnection: ClientGuid가 null이거나 빈 문자열입니다."
                    : "RegisterConnection: ClientGuid is null or empty string.";

                ClassMain.SaveLog(out errMsg, EnumLogType.Normal, nullGuidMsg);
                return false;
            }

            try
            {
                lock (_lock)
                {
                    // 이미 등록된 GUID인지 확인
                    if (_activeConnections.ContainsKey(clientGuid))
                    {
                        // 기존 연결 정보 업데이트
                        var existingConnection = _activeConnections[clientGuid];
                        existingConnection.LastHeartbeat = DateTime.Now;
                        existingConnection.IPAddress = ipAddress;

                        string errMsg;
                        string existingConnMsg = Tools.IsLangKorean()
                            ? string.Format("기존 연결 갱신 - ClientGuid: {0}, LoginMode: {1}, Username: {2}",
                                clientGuid, loginMode, username)
                            : string.Format("Existing connection updated - ClientGuid: {0}, LoginMode: {1}, Username: {2}",
                                clientGuid, loginMode, username);

                        ClassMain.SaveLog(out errMsg, EnumLogType.Normal, existingConnMsg);
                        return true;
                    }

                    var connectionInfo = new ClientConnectionInfo
                    {
                        ClientGuid = clientGuid,
                        LoginMode = loginMode,
                        Username = username,
                        IPAddress = ipAddress,
                        UserAgent = userAgent,
                        LastHeartbeat = DateTime.Now,
                        LoginTime = DateTime.Now
                    };

                    _activeConnections[clientGuid] = connectionInfo;

                    // 등록 확인 로그
                    string confirmMsg = Tools.IsLangKorean()
                      ? string.Format("RegisterConnection 완료 - ClientGuid: {0}, LoginMode: {1}, Username: {2}, IP: {3}, 총연결수: {4}",
                          clientGuid, loginMode, username, ipAddress, _activeConnections.Count)
                      : string.Format("RegisterConnection completed - ClientGuid: {0}, LoginMode: {1}, Username: {2}, IP: {3}, TotalConnections: {4}",
                          clientGuid, loginMode, username, ipAddress, _activeConnections.Count);

                    string errMsg2;
                    ClassMain.SaveLog(out errMsg2, EnumLogType.Normal, confirmMsg);

                    return true;
                }
            }
            catch (Exception ex)
            {
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                    string.Format("RegisterConnection error - ClientGuid: {0}, Error: {1}",
                    clientGuid, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 클라이언트 연결 해제
        /// </summary>
        public static void DisconnectClient(string clientGuid)
        {
            try
            {
                lock (_lock)
                {
                    if (_activeConnections.ContainsKey(clientGuid))
                    {
                        var connectionInfo = _activeConnections[clientGuid];
                        _activeConnections.Remove(clientGuid);

                        // 로그 기록
                        LogConnectionEnd(connectionInfo, Tools.IsLangKorean() ? "정상 해제" : "Normal disconnect");
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg;
                string exceptionMsg = Tools.IsLangKorean()
                    ? string.Format("DisconnectClient 예외 발생 - ClientGuid: {0}, Error: {1}",
                        clientGuid, ex.Message)
                    : string.Format("DisconnectClient exception occurred - ClientGuid: {0}, Error: {1}",
                        clientGuid, ex.Message);

                ClassMain.SaveLog(out errMsg, EnumLogType.Error, exceptionMsg);
            }
        }



        /// <summary>
        /// 만료된 연결 정리
        /// </summary>
        private static void CleanupExpiredConnections(object state)
        {
            try
            {
                lock (_lock)
                {
                    var expiredConnections = new List<string>();
                    // 설정값을 사용하여 타임아웃 시간 계산
                    var cutoffTime = DateTime.Now.AddMinutes(-UserTimeOutMinutes);

                    foreach (var kvp in _activeConnections.ToArray()) // ToArray()로 복사본 생성
                    {
                        if (kvp.Value.LastHeartbeat < cutoffTime)
                        {
                            expiredConnections.Add(kvp.Key);
                        }
                    }

                    // 배치 정리로 성능 개선
                    foreach (string clientGuid in expiredConnections)
                    {
                        if (_activeConnections.ContainsKey(clientGuid)) // 재확인
                        {
                            var connectionInfo = _activeConnections[clientGuid];
                            _activeConnections.Remove(clientGuid);

                            // 로그 기록
                            LogConnectionEnd(connectionInfo, Tools.IsLangKorean() ? "타임아웃" : "Timeout");
                        }
                    }

                    if (expiredConnections.Count > 0)
                    {
                        string msg = Tools.IsLangKorean()
                            ? string.Format("만료된 연결 {0}개 정리 완료", expiredConnections.Count)
                            : string.Format("Cleaned up {0} expired connections", expiredConnections.Count);

                        string errMsg;
                        ClassMain.SaveLog(out errMsg, EnumLogType.Normal, msg);
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                    string.Format("CleanupExpiredConnections error: {0}", ex.Message));

                // 예외를 다시 던져서 상위의 SafeCleanupExpiredConnections에서 처리하도록 함
                throw;
            }
        }

        /// <summary>
        /// 특정 로그인 모드의 활성 사용자 수 반환
        /// </summary>
        public static int GetActiveUserCount(string loginMode)
        {
            try
            {

                lock (_lock)
                {
                    int count = 0;
                    foreach (ClientConnectionInfo connection in _activeConnections.Values)
                    {
                        if (connection.LoginMode == loginMode)
                            count++;
                    }
                    return count;
                }
            }
            catch (Exception ex)
            {
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                    string.Format("GetActiveUserCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// 전체 활성 연결 수 반환
        /// </summary>
        public static int GetTotalActiveCount()
        {
            lock (_lock)
            {
                return _activeConnections.Count;
            }
        }

        /// <summary>
        /// 특정 클라이언트가 이미 연결되어 있는지 확인
        /// </summary>
        public static bool IsClientConnected(string clientGuid)
        {
            if (string.IsNullOrEmpty(clientGuid))
                return false;

            lock (_lock)
            {
                return _activeConnections.ContainsKey(clientGuid);
            }
        }


        /// <summary>
        /// 연결 종료 로그 기록
        /// </summary>
        private static void LogConnectionEnd(ClientConnectionInfo connectionInfo, string reason)
        {
            try
            {
                TimeSpan duration = DateTime.Now - connectionInfo.LoginTime;
                string durationStr = string.Format("{0:00}:{1:00}:{2:00}",
                    (int)duration.TotalHours, duration.Minutes, duration.Seconds);

                string msg = Tools.IsLangKorean()
                    ? string.Format("연결 종료 - Reason={0}, Mode={1}, IP={2}, Username={3}, Duration={4}, ClientGuid={5}",
                        reason, connectionInfo.LoginMode, connectionInfo.IPAddress, connectionInfo.Username,
                        durationStr, connectionInfo.ClientGuid)
                    : string.Format("Connection ended - Reason={0}, Mode={1}, IP={2}, Username={3}, Duration={4}, ClientGuid={5}",
                        reason, connectionInfo.LoginMode, connectionInfo.IPAddress, connectionInfo.Username,
                        durationStr, connectionInfo.ClientGuid);

                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Normal, msg);

                EventLogEntryType logType = reason == "타임아웃" ? EventLogEntryType.Warning : EventLogEntryType.Information;
                EventLogWriteEntry(reason, "AutoBaseWebServer", msg, logType,
                    GetActiveUserCount(connectionInfo.LoginMode));
            }
            catch (Exception ex)
            {
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                    string.Format("LogConnectionEnd error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 연결된 클라이언트 정보 조회 (디버깅용)
        /// </summary>
        public static List<ClientConnectionInfo> GetAllActiveConnections()
        {
            lock (_lock)
            {
                return _activeConnections.Values.ToList();
            }
        }

        /// <summary>
        /// EventLog 작성
        /// </summary>
        private static void EventLogWriteEntry(string action, string source, string message,
           EventLogEntryType type, int userCount)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = source;
                    eventLog.WriteEntry(message, type, userCount);
                }
            }
            catch
            {
                // EventLog 작성 실패 시 무시
            }
        }
    }
}
