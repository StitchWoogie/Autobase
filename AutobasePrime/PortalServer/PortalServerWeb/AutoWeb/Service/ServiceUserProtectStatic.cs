using AutoLibLocal;
using AutoLibLocal.KeyLock;
using NetTools;
using Newtonsoft.Json;
using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Services;

namespace PortalServerWeb.AutoWeb.Service
{
    public class ServiceUserProtectStatic
    {
        // EventID 상수 정의
        private const int EVENT_ID_LOGIN_SUCCESS = 1001;
        private const int EVENT_ID_LOGIN_FAILED = 1002;
        private const int EVENT_ID_LOGOUT_NORMAL = 1003;
        private const int EVENT_ID_LOGOUT_TIMEOUT = 1004;
        private const int EVENT_ID_CONNECTION_EXPIRED = 1005;
        private const int EVENT_ID_HEARTBEAT_FAILED = 1006;
        private const int EVENT_ID_LICENSE_EXCEEDED = 1007;
        private const int EVENT_ID_TRIAL_EXCEEDED = 1008;

        static void EventLogWriteEntry(string source, string logname, string msg, EventLogEntryType type, int eventID)
        {

            try
            {
                if (!EventLog.SourceExists(source))	// 소스 Exist를 검사하는 도중 exception이 발생해서 catch를 잡았음.
                {
                    EventLog.CreateEventSource(source, logname);
                }
            }
            catch
            {
                return;	// fail
                //EventLog.CreateEventSource(source, logname);
            }

            EventLog myLog = new EventLog();
            myLog.Source = source;
            myLog.WriteEntry(msg, type, eventID);
        }

        private static HttpRequest SafeRequest(HttpContext context)
        {
            if (context == null)
                throw new InvalidOperationException("HttpContext is null.");

            if (context.Request == null)
                throw new InvalidOperationException("HttpContext.Request is null.");

            return context.Request;
        }

        public static bool LocalDefaultUserCheck(HttpContext context,
            HttpApplicationState app, out string username, out string err_msg, int need_version, string clientGuid, out bool bTrialMode )
        {
            err_msg = "";
            username = "";
            bTrialMode = false;

            // ClientGuid 유효성 검사
            if (string.IsNullOrEmpty(clientGuid))
            {
                err_msg = Tools.IsLangKorean() ? "클라이언트 ID가 필요합니다.클라이언트 버전을 확인하세요." : "A client ID is required. Check the client version";
                return false;
            }

            var request = SafeRequest(context);

            TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(request);  //DataLocal 뒤에 있어 앞으로 이동 251120 PSU 
            DataLocal local = new DataLocal();
            // TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);

            ClassDebug.Debug(TotalConfig.sDirWorkProject);

            bool retn = local.DefaultUserCheck(out err_msg, out username);

            if (retn)
            {
                if (!PlusUserCount(context, out err_msg, need_version, username, clientGuid, out bTrialMode)) return false;
            }

            return retn;
        }
        //250825 PSU 동시접속자 제어 추가 v10.3.7.5
        static bool PlusUserCount(HttpContext context, out string err_msg, int need_version, string username, string clientGuid, out bool bTrialMode)
        {
            err_msg = "";

            var request = SafeRequest(context);

            KeyLock keylock = new KeyLock();
            keylock.CheckKeyLockWeb();

            bool bLicenseMode = false;
            bTrialMode = false;

            if (!KeyLock.bExistWebKey)
            {
                if (!ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, "KeyLock not exists in the WebServer.")) return false;
                err_msg = Tools.IsLangKorean() ? "서버에 키락이 없습니다." : "There is no key lock on the server.";
                bTrialMode = true;
            }
            else
            {
                if (KeyLock.nKeyLockVersion < need_version)
                {
                    err_msg = Tools.IsLangKorean() ? "서버의 키락 버전이 " + need_version.ToString() + " 이상이어야 합니다. 현재=" + KeyLock.nKeyLockVersion.ToString()
                    : "The server's key lock version must be at least " + need_version.ToString() + ". Current = " + KeyLock.nKeyLockVersion.ToString();
                    if (!ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, err_msg)) return false;
                    return false;
                }
                else bLicenseMode = true;
            }

            // 이미 연결된 클라이언트 체크 (GUID 기반)
            if (GuidHeartbeatManager.IsClientConnected(clientGuid))
            {
                // 이미 활성 연결이 있으므로 접속자 수 증가 안함
                string msg2;
                ClassMain.SaveLog(out msg2, EnumLogType.LogIn,
                    string.Format("client already connected - ClientGuid: {0}", clientGuid));
                return true;
            }



            // 실시간 활성 사용자 수 확인 (GUID 기반)
            int currentLicenseUsers = GuidHeartbeatManager.GetActiveUserCount("License");
            int currentTrialUsers = GuidHeartbeatManager.GetActiveUserCount("Trial");

            // -------------------
            // 라이센스 모드 접속 제한 체크
            // -------------------
            if (bLicenseMode)
            {
                if (currentLicenseUsers >= KeyLock.nWebUserCount && KeyLock.nWebUserCount < 50) //50 이상은 무제한.
                {
                    // 라이센스 초과 → 체험판 전환
                    bLicenseMode = false;
                    bTrialMode = true;
                }
            }

            // -------------------
            //체험판 모드 접속 제한 체크
            // -------------------
            if (bTrialMode)
            {
                if (currentTrialUsers >= 2) // 체험판 동시접속 제한
                {
                    if (KeyLock.bExistWebKey)
                    {
                        err_msg = Tools.IsLangKorean()
                        ? String.Format("라이센스 동시접속자 수 ({0}) 및 체험판 동시 접속자(2)를 모두  초과하여 로그인이 제한됩니다.", KeyLock.nWebUserCount)
                        : String.Format("Login is restricted because the number of licensed concurrent users ({0}) and trial version concurrent users (2) has been exceeded.", KeyLock.nWebUserCount);
                    }
                    else
                    {
                        err_msg = Tools.IsLangKorean()
                        ? "체험판 동시 접속자가 2명을 초과하여 로그인이 제한됩니다."
                        : "Trial mode concurrent users exceeded 2, login restricted.";
                    }
                    return false;
                }
            }

            string loginMode = bTrialMode ? "Trial" : "License";

            // GUID 기반 연결 등록
            string userAgent = request.UserAgent ?? "";
            string ipAddress = request.UserHostAddress;

            if (!GuidHeartbeatManager.RegisterConnection(clientGuid, loginMode, username, ipAddress, userAgent))
            {
                err_msg = Tools.IsLangKorean() ? "연결 등록에 실패했습니다." : "Failed to register the connection.";
                return false;
            }


            // 로그 작성
            string mode = bTrialMode ? "Trial" : "License";
            string msg = string.Format("Mode={0}, IP={1}, Username={2}, ClientGuid={3}",
                mode, request.UserHostAddress, username, clientGuid);
            if (!ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, msg)) return false;

            // 실시간 활성 사용자 수 확인 (GUID 기반)
            currentLicenseUsers = GuidHeartbeatManager.GetActiveUserCount("License");
            currentTrialUsers = GuidHeartbeatManager.GetActiveUserCount("Trial");

            EventLogWriteEntry("LogIn", "AutoBaseWebServer", msg, EventLogEntryType.Information,
                            EVENT_ID_LOGIN_SUCCESS);

            err_msg = String.Format("mode={0}, UserCountTrial={1},UserCountLicense={2}"
                             , mode, currentTrialUsers, currentLicenseUsers);
            return true;
        }

        public static bool LocalCheckUserName(HttpContext context,
        HttpApplicationState app, string username, string password, string passcode256, out string err_msg, int need_version, string clientGuid, out bool bTrialMode)
        {
            err_msg = "";
            bTrialMode = false;

            // ClientGuid 유효성 검사
            if (string.IsNullOrEmpty(clientGuid))
            {
                err_msg = Tools.IsLangKorean() ? "클라이언트 ID가 필요합니다.\n웹클라이언트 버전을 확인하세요." : "A client ID is required.\nPlease check the WebClient version";
                return false;
            }

            var request = SafeRequest(context);

            string item_count = "FailCount_" + username;
            string item_time = "FailTime_" + username;

            int fail_count = ConfigApplication.GetInt32(app, item_count);

            if (ConfigVarTotal.bLocalFlag && fail_count >= 5)
            {
                DateTime tLast = ConfigApplication.GetDateTime(app, item_time);
                DateTime t = DateTime.Now;
                TimeSpan ts = t - tLast;
                int MAX_WAIT = 60;

                if (ts.TotalSeconds < MAX_WAIT)
                {
                    //250808 PSU 한영 적용
                    if (Tools.IsLangKorean())
                        err_msg = String.Format("로그인을 연속으로 실패({0}회)해서 {1}초 후에 다시 시도할 수 있습니다.", fail_count, (int)(MAX_WAIT - ts.TotalSeconds));
                    else
                        err_msg = String.Format("You can try again after {1} seconds as you have failed to log in consecutively ({0} times).", fail_count, (int)(MAX_WAIT - ts.TotalSeconds));

                    string msg2;
                    ClassMain.SaveLog(out msg2, EnumLogType.LogIn, err_msg);
                    return false;
                }
            }

            TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(request);  //DataLocal 뒤에 있어 앞으로 이동 251120 PSU 
            DataLocal local = new DataLocal();
            //TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);

            // AutoLibLocal 라이브러리를 초기화한다.
            AutoLibLocal._LibraryInit.SetGuid(HashTool.MakeHash(AutoLibLocal._LibraryInit.GetGuid() + "AutoLibLocal.dll"));

            bool retn = local.CheckUserName(out err_msg, username, password, passcode256);
            if (retn == true)
            {
                if (!PlusUserCount(context, out err_msg, need_version, username, clientGuid, out bTrialMode)) return false;

                string msg2;
                ClassMain.SaveLog(out msg2, EnumLogType.LogIn,
              string.Format("Login success - Username: {0}, ClientGuid: {1}", username, clientGuid));
                ConfigApplication.SetValue(app, item_count, 0);
            }
            else
            {
                fail_count++;
                ConfigApplication.SetValue(app, item_count, fail_count);
                ConfigApplication.SetValue(app, item_time, DateTime.Now);

                string msg = String.Format("Username:{0} {1} FailCount={2} ClientGuid={3}",
                                  username, err_msg, fail_count, clientGuid);
                ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, msg);
            }
            return retn;
        }

        const int NEED_VERSION = 8; // 2008년 이상 키만 사용가능

        public static UserCountInfo GetUserCount()
        {
            UserCountInfo info = new UserCountInfo();

            // 실시간 활성 사용자 수 반환 (GUID 기반)
            info.KeylockCount = KeyLock.nWebUserCount;
            info.LicenseCount = GuidHeartbeatManager.GetActiveUserCount("License");
            info.TrialCount = GuidHeartbeatManager.GetActiveUserCount("Trial");

            return info;
        }

        #region 로그아웃 / 하트비트 / 상태조회

        public static bool LogOut(HttpContext context, string clientGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(clientGuid))
                {
                    string errMsg = Tools.IsLangKorean()
                        ? "LogOut: ClientGuid가 null이거나 빈 문자열입니다."
                        : "LogOut: ClientGuid is null or an empty string.";
                    string tmp;
                    ClassMain.SaveLog(out tmp, EnumLogType.Error, errMsg);
                    return false;
                }

                var request = SafeRequest(context);

                if (GuidHeartbeatManager.IsClientConnected(clientGuid))
                {
                    GuidHeartbeatManager.DisconnectClient(clientGuid);

                    string msg = Tools.IsLangKorean()
                        ? string.Format("정상 로그아웃 - ClientGuid: {0}, IP: {1}", clientGuid, request.UserHostAddress)
                        : string.Format("Normal Logout - ClientGuid: {0}, IP: {1}", clientGuid, request.UserHostAddress);

                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Normal, msg);

                    EventLogWriteEntry(
                        "LogOut",
                        "AutoBaseWebServer",
                        msg,
                        EventLogEntryType.Information,
                        EVENT_ID_LOGOUT_NORMAL);
                    return true;
                }
                else
                {
                    string msg = Tools.IsLangKorean()
                        ? string.Format("이미 해제된 연결 - ClientGuid: {0}", clientGuid)
                        : string.Format("The connection has already been released. - ClientGuid: {0}", clientGuid);

                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Normal, msg);
                    return true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("LogOut error: {0}, ClientGuid: {1}", ex.Message, clientGuid);
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error, errorMsg);
                return false;
            }
        }

        public static bool UpdateHeartbeat(HttpContext context, string clientGuid, out string error)
        {
            error = "";
            try
            {
                if (string.IsNullOrEmpty(clientGuid))
                {
                    error = "ClientGuid required";
                    return false;
                }

                var request = SafeRequest(context);
                string ipAddress = request.UserHostAddress;

                bool result = GuidHeartbeatManager.UpdateHeartbeat(clientGuid, ipAddress);

                if (!result)
                {
                    error = "Connection not found - re-login required";
                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                        string.Format("Heartbeat failed: No connection - ClientGuid: {0}, IP: {1}", clientGuid, ipAddress));

                    EventLogWriteEntry(
                        "Heartbeat",
                        "AutoBaseWebServer",
                        "Heartbeat failed for " + clientGuid,
                        EventLogEntryType.Warning,
                        EVENT_ID_HEARTBEAT_FAILED);
                }

                return result;
            }
            catch (Exception ex)
            {
                string errMsg;
                error = ex.Message;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                    string.Format("Heartbeat error: {0}, ClientGuid: {1}", ex.Message, clientGuid));
                return false;
            }
        }

        public static string GetConnectionStatus()
        {
            try
            {
                var connections = GuidHeartbeatManager.GetAllActiveConnections();
                var result = new
                {
                    TotalConnections = connections.Count,
                    KeylockUsers = KeyLock.nWebUserCount,
                    LicenseUsers = connections.Count(c => c.LoginMode == "License"),
                    TrialUsers = connections.Count(c => c.LoginMode == "Trial"),
                    Connections = connections.Select(c => new
                    {
                        ClientGuid = c.ClientGuid,
                        Username = c.Username,
                        LoginMode = c.LoginMode,
                        IPAddress = c.IPAddress,
                        LoginTime = c.LoginTime,
                        LastHeartbeat = c.LastHeartbeat,
                        Duration = DateTime.Now - c.LoginTime
                    }).ToArray()
                };

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        #endregion

    }
}