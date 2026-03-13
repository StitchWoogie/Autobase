using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Diagnostics;
using AutoLibLocal.KeyLock;
using AutoLibLocal;
using NetTools;
using PortalServerWeb.Library;
using System.Web.Script.Services;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for ServiceUserProtect
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceUserProtect : System.Web.Services.WebService
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

        #region PlusUserCount Old
        //static bool PlusUserCount(WebService ws, out string err_msg, int need_version, string username)
        //{
        //    err_msg = "";

        //    KeyLock keylock = new KeyLock();
        //    keylock.CheckKeyLockWeb();

        //    if (!KeyLock.bExistWebKey)
        //    {
        //        if (!ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, "KeyLock not exists in the WebServer.")) return false;

        //        err_msg = "서버에 키락이 없습니다.";
        //        return false;
        //    }

        //    if (KeyLock.nKeyLockVersion < need_version)
        //    {
        //        err_msg = "서버의 키락 버전이 " + need_version.ToString() + " 이상이어야 합니다. 현재=" + KeyLock.nKeyLockVersion.ToString();

        //        if (!ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, err_msg)) return false;

        //        return false;
        //    }

        //    int count = 0;

        //    if (ws.Application["UserCount"] == null)       // 2005에서는 없으면 "" 이었는데 2008은 null이다.
        //        count = 0;
        //    else
        //        count = NetTools.ConvertTool.ToInt32(ws.Application["UserCount"].ToString());

        //    count++;
        //    ws.Application["UserCount"] = count.ToString();

        //    string msg = "UserHostAddress:" + ws.Context.Request.UserHostAddress + "\n";
        //    msg += "UserHostName:" + ws.Context.Request.UserHostName + "\n";
        //    msg += "Root:" + ws.Context.Request.PhysicalApplicationPath + "\n";

        //    EventLogWriteEntry("LogIn", "AutoBaseWebServer", msg, EventLogEntryType.Information, count);

        //    msg = String.Format("IP:{0}, Username:{1}", ws.Context.Request.UserHostAddress, username);

        //    if (!ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, msg)) return false;

        //    return true;
        //}

        //public static bool LocalCheckUserName(WebService ws, string username, string password, string passcode256, out string err_msg, int need_version)
        //{
        //    err_msg = "";

        //    string item_count = "FailCount_" + username;
        //    string item_time = "FailTime_" + username;

        //    int fail_count = ConfigApplication.GetInt32(ws.Application, item_count);

        //    if (ConfigVarTotal.bLocalFlag && fail_count >= 5)
        //    {
        //        DateTime tLast = ConfigApplication.GetDateTime(ws.Application, item_time);
        //        DateTime t = DateTime.Now;

        //        TimeSpan ts = t - tLast;

        //        int MAX_WAIT = 60;

        //        if (ts.TotalSeconds < MAX_WAIT)
        //        {
        //            err_msg = String.Format("로그인을 연속으로 실패({0}회)해서 {1}초 후에 다시 시도할 수 있습니다.", fail_count, (int)(MAX_WAIT - ts.TotalSeconds));
        //            return false;
        //        }
        //    }

        //    DataLocal local = new DataLocal();
        //    TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);
        //    // AutoLibLocal 라이브러리를 초기화한다.
        //    AutoLibLocal._LibraryInit.SetGuid(HashTool.MakeHash(AutoLibLocal._LibraryInit.GetGuid() + "AutoLibLocal.dll"));

        //    bool retn = local.CheckUserName(out err_msg, username, password, passcode256);
        //    if (retn == true)
        //    {
        //        if (!PlusUserCount(ws, out err_msg, need_version, username)) return false;
        //        ConfigApplication.SetValue(ws.Application, item_count, 0);    // Fail Count를 클리어한다.
        //    }
        //    else
        //    {
        //        fail_count++;

        //        ConfigApplication.SetValue(ws.Application, item_count, fail_count);
        //        ConfigApplication.SetValue(ws.Application, item_time, DateTime.Now);

        //        string msg = String.Format("Username:{0} {1} FailCount={2}", username, err_msg, fail_count);

        //        ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, msg);


        //    }
        //    return retn;
        //}

        //const int NEED_VERSION = 8; // 2008년 이상 키만 사용가능

        //[WebMethod]
        //public bool CheckUserName(string username, string password, out string err_msg)
        //{
        //    if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
        //    {
        //        return false;
        //    }

        //    return LocalCheckUserName(this, username, password, null, out err_msg, 0);
        //}

        //[WebMethod]
        //public bool CheckUserNameWithVersion(string username, string password, out string err_msg)
        //{
        //    if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
        //    {
        //        return false;
        //    }

        //    return LocalCheckUserName(this, username, password, null, out err_msg, NEED_VERSION);
        //}

        ///* SecurityLevel 3을 지원하면서 사라짐
        //// 512 Hash를 미리 지원해서 웹 클라이언트에서 이 함수를 사용할 수 있도록 한다. 2015-11-2
        //[WebMethod]
        //public bool CheckUserNameWithVersionPass512(string username, string password, out string err_msg)
        //{
        //    return LocalCheckUserName(this, username, password, null, out err_msg, NEED_VERSION);
        //}*/

        //[WebMethod]
        //public int GetUserCount()
        //{
        //    int count = 0;
        //    count = Convert.ToInt32(Application["UserCount"].ToString());
        //    return count;
        //}


        //public static bool LocalDefaultUserCheck(WebService ws, out string username, out string err_msg, int need_version)
        //{
        //    err_msg = "";



        //    DataLocal local = new DataLocal();
        //    TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);

        //    ClassDebug.Debug(TotalConfig.sDirWorkProject);

        //    bool retn = local.DefaultUserCheck(out err_msg, out username);

        //    if (retn)
        //    {
        //        if (!PlusUserCount(ws, out err_msg, need_version, username)) return false;
        //    }

        //    return retn;
        //}

        //[WebMethod]
        //public bool DefaultUserCheck(out string username, out string err_msg)
        //{
        //    if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
        //    {
        //        username = "";
        //        return false;
        //    }

        //    return LocalDefaultUserCheck(this, out username, out err_msg, 0);
        //}

        //[WebMethod]
        //public bool DefaultUserCheckWithVersion(out string username, out string err_msg)
        //{
        //    if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
        //    {
        //        username = "";
        //        return false;
        //    }

        //    return LocalDefaultUserCheck(this, out username, out err_msg, NEED_VERSION);
        //}

        //[WebMethod]
        //public void LogOut()
        //{
        //    int count = 0;
        //    count = Convert.ToInt32(Application["UserCount"].ToString());

        //    count--;
        //    Application["UserCount"] = count.ToString();

        //    string msg = "UserHostAddress:" + Context.Request.UserHostAddress + "\n";
        //    msg += "UserHostName:" + Context.Request.UserHostName + "\n";
        //    msg += "Root:" + Context.Request.PhysicalApplicationPath + "\n";

        //    EventLogWriteEntry("LogOut", "AutoBaseWebServer", msg, EventLogEntryType.Information, count);
        //}
        #endregion

        //250825 PSU 동시접속자 제어 추가 v10.3.7.5
        static bool PlusUserCount(WebService ws, out string err_msg, int need_version, string username, string clientGuid)
        {
            err_msg = "";

            KeyLock keylock = new KeyLock();
            keylock.CheckKeyLockWeb();

            bool bLicenseMode = false;
            bool bTrialMode = false;

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
            string userAgent = ws.Context.Request.UserAgent ?? "";
            string ipAddress = ws.Context.Request.UserHostAddress;

            if (!GuidHeartbeatManager.RegisterConnection(clientGuid, loginMode, username, ipAddress, userAgent))
            {
                err_msg = Tools.IsLangKorean() ? "연결 등록에 실패했습니다." : "Failed to register the connection.";
                return false;
            }


            // 로그 작성
            string mode = bTrialMode ? "Trial" : "License";
            string msg = string.Format("Mode={0}, IP={1}, Username={2}, ClientGuid={3}",
                mode, ws.Context.Request.UserHostAddress, username, clientGuid);
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

        public static bool LocalCheckUserName(WebService ws, string username, string password, string passcode256, out string err_msg, int need_version, string clientGuid)
        {
            err_msg = "";

            // ClientGuid 유효성 검사
            if (string.IsNullOrEmpty(clientGuid))
            {
                err_msg = Tools.IsLangKorean() ? "클라이언트 ID가 필요합니다.\n웹클라이언트 버전을 확인하세요." : "A client ID is required.\nPlease check the WebClient version";
                return false;
            }

            string item_count = "FailCount_" + username;
            string item_time = "FailTime_" + username;

            int fail_count = ConfigApplication.GetInt32(ws.Application, item_count);

            if (ConfigVarTotal.bLocalFlag && fail_count >= 5)
            {
                DateTime tLast = ConfigApplication.GetDateTime(ws.Application, item_time);
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

            TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);  //DataLocal 뒤에 있어 앞으로 이동 251120 PSU 
            DataLocal local = new DataLocal();
            //TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);

            // AutoLibLocal 라이브러리를 초기화한다.
            AutoLibLocal._LibraryInit.SetGuid(HashTool.MakeHash(AutoLibLocal._LibraryInit.GetGuid() + "AutoLibLocal.dll"));

            bool retn = local.CheckUserName(out err_msg, username, password, passcode256);
            if (retn == true)
            {
                if (!PlusUserCount(ws, out err_msg, need_version, username, clientGuid)) return false;

                string msg2;
                ClassMain.SaveLog(out msg2, EnumLogType.LogIn,
              string.Format("Login success - Username: {0}, ClientGuid: {1}", username, clientGuid));
                ConfigApplication.SetValue(ws.Application, item_count, 0);
            }
            else
            {
                fail_count++;
                ConfigApplication.SetValue(ws.Application, item_count, fail_count);
                ConfigApplication.SetValue(ws.Application, item_time, DateTime.Now);

                string msg = String.Format("Username:{0} {1} FailCount={2} ClientGuid={3}",
                                  username, err_msg, fail_count, clientGuid);
                ClassMain.SaveLog(out err_msg, EnumLogType.LogIn, msg);
            }
            return retn;
        }

        const int NEED_VERSION = 8; // 2008년 이상 키만 사용가능

        [WebMethod]
        public bool CheckUserName(string username, string password, string clientGuid, out string err_msg)
        {
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                return false;
            }

            return LocalCheckUserName(this, username, password, null, out err_msg, 0, clientGuid);
        }

        [WebMethod]
        public bool CheckUserNameWithVersion(string username, string password, string clientGuid, out string err_msg)
        {
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                return false;
            }

            return LocalCheckUserName(this, username, password, null, out err_msg, NEED_VERSION, clientGuid);
        }

        /* SecurityLevel 3을 지원하면서 사라짐
        // 512 Hash를 미리 지원해서 웹 클라이언트에서 이 함수를 사용할 수 있도록 한다. 2015-11-2
        [WebMethod]
        public bool CheckUserNameWithVersionPass512(string username, string password, out string err_msg)
        {
            return LocalCheckUserName(this, username, password, null, out err_msg, NEED_VERSION);
        }*/

        public class UserCountInfo
        {
            private int _keylockCount;
            private int _licenseCount;
            private int _trialCount;

            public int KeylockCount
            {
                get { return _keylockCount; }
                set { _keylockCount = value; }
            }

            public int LicenseCount
            {
                get { return _licenseCount; }
                set { _licenseCount = value; }
            }

            public int TrialCount
            {
                get { return _trialCount; }
                set { _trialCount = value; }
            }

            public int TotalCount
            {
                get { return _licenseCount + _trialCount; }
            }
        }

        [WebMethod]
        public UserCountInfo GetUserCount()
        {
            UserCountInfo info = new UserCountInfo();

            // 실시간 활성 사용자 수 반환 (GUID 기반)
            info.KeylockCount = KeyLock.nWebUserCount;
            info.LicenseCount = GuidHeartbeatManager.GetActiveUserCount("License");
            info.TrialCount = GuidHeartbeatManager.GetActiveUserCount("Trial");

            return info;
        }


        public static bool LocalDefaultUserCheck(WebService ws, out string username, out string err_msg, int need_version, string clientGuid)
        {
            err_msg = "";
            username = "";

            // ClientGuid 유효성 검사
            if (string.IsNullOrEmpty(clientGuid))
            {
                err_msg = Tools.IsLangKorean() ? "클라이언트 ID가 필요합니다.클라이언트 버전을 확인하세요." : "A client ID is required. Check the client version";
                return false;
            }

            TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);  //DataLocal 뒤에 있어 앞으로 이동 251120 PSU 
            DataLocal local = new DataLocal();
           // TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ws.Context.Request);

            ClassDebug.Debug(TotalConfig.sDirWorkProject);

            bool retn = local.DefaultUserCheck(out err_msg, out username);

            if (retn)
            {
                if (!PlusUserCount(ws, out err_msg, need_version, username, clientGuid)) return false;
            }

            return retn;
        }

        [WebMethod]
        public bool DefaultUserCheck(string clientGuid, out string username, out string err_msg)
        {
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                username = "";
                return false;
            }

            return LocalDefaultUserCheck(this, out username, out err_msg, 0, clientGuid);
        }

        [WebMethod]
        public bool DefaultUserCheckWithVersion(string clientGuid, out string username, out string err_msg)
        {
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                username = "";
                return false;
            }

            return LocalDefaultUserCheck(this, out username, out err_msg, NEED_VERSION, clientGuid);
        }

        [WebMethod]
        public void LogOut(string clientGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(clientGuid))
                {
                    string errMsg = Tools.IsLangKorean() ? "LogOut: ClientGuid가 null이거나 빈 문자열입니다."
                        : "LogOut: ClientGuid is null or an empty string.";
                    ClassMain.SaveLog(out errMsg, EnumLogType.Error, errMsg);
                    return;
                }

                if (GuidHeartbeatManager.IsClientConnected(clientGuid))
                {
                    // GUID 기반 연결 해제
                    GuidHeartbeatManager.DisconnectClient(clientGuid);

                    // 로그 기록
                    string msg = Tools.IsLangKorean() ? string.Format("정상 로그아웃 - ClientGuid: {0}, IP: {1}",
                        clientGuid, Context.Request.UserHostAddress)
                        : string.Format("Normal Logout - ClientGuid: {0}, IP: {1}",
                        clientGuid, Context.Request.UserHostAddress);

                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Normal, msg);

                    EventLogWriteEntry("LogOut", "AutoBaseWebServer", msg, EventLogEntryType.Information, EVENT_ID_LOGOUT_NORMAL
                        );
                }
                else
                {
                    string msg = Tools.IsLangKorean() ? string.Format("이미 해제된 연결 - ClientGuid: {0}", clientGuid)
                        : string.Format("The connection has already been released. - ClientGuid: {0}", clientGuid);
                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Normal, msg);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("LogOut error: {0}, ClientGuid: {1}", ex.Message, clientGuid);
                string errMsg;
                ClassMain.SaveLog(out errMsg, EnumLogType.Error, errorMsg);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public bool UpdateHeartbeat(string clientGuid, out string error)
        {
            error = "";
            try
            {
                if (string.IsNullOrEmpty(clientGuid))
                {
                    error = "ClientGuid required";
                    return false;
                }

                string ipAddress = Context.Request.UserHostAddress;
                bool result = GuidHeartbeatManager.UpdateHeartbeat(clientGuid, ipAddress);

                if (!result)
                {
                    error = "Connection not found - re-login required";
                    string errMsg;
                    ClassMain.SaveLog(out errMsg, EnumLogType.Error,
                        string.Format("Heartbeat failed: No connection - ClientGuid: {0}, IP: {1}", clientGuid, ipAddress));
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

        // 디버깅용 메서드
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public string GetConnectionStatus()
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

                return Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }


    }
}
