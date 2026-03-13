using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "ServiceUserProtect"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 ServiceUserProtect.svc나 ServiceUserProtect.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ServiceUserProtect : IServiceUserProtect
    {
        private HttpContext Context => HttpContext.Current;
        private HttpApplicationState App => HttpContext.Current.Application;

        // 기존 PlusUserCount, LocalCheckUserName 등 모든 static 함수 그대로 재사용 가능

        public bool CheckUserName(string username, string password, string clientGuid, out string err_msg, out bool bTrialMode)
        {
            bTrialMode = false;
            try
            {
       
                if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
                    return false;

                return ServiceUserProtectStatic.LocalCheckUserName(
                     Context, App, username, password, null, out err_msg, 0, clientGuid, out bTrialMode);
            }
            catch (Exception ex)
            {
                err_msg = ".CheckUserName Exception: " + ex.Message;
                return false;
            }
        }

        public bool CheckUserNameWithVersion(string username, string password, string clientGuid, out string err_msg, out bool bTrialMode)
        {
            bTrialMode = false;

            try
            {
                if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
                    return false;

                return ServiceUserProtectStatic.LocalCheckUserName(
                      Context, App, username, password, null, out err_msg, 8, clientGuid, out bTrialMode);
            }
            catch(Exception ex)
            {
                err_msg = ".CheckUserNameWithVersion Exception: " + ex.Message;
                return false;
            }
        }

        public UserCountInfo GetUserCount()
        {
            try
            {
                return ServiceUserProtectStatic.GetUserCount();
            }
            catch
            {
                return null;
            }
        }


        public bool DefaultUserCheck(string clientGuid, out string username, out string err_msg, out bool trialMode)
        {
            trialMode = false;
            try
            {       
                if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
                {
                    username = "";
                    return false;
                }

                return ServiceUserProtectStatic.LocalDefaultUserCheck(
                    Context, App, out username, out err_msg, 0, clientGuid, out trialMode);
            }
            catch (Exception ex)
            {
                username = "";
                err_msg = "DefaultUserCheck Exception: " + ex.Message;               
                return false;
            }
        }

        public bool DefaultUserCheckWithVersion(string clientGuid, out string username, out string err_msg, out bool bTrialMode)
        {
            bTrialMode = false;

            try
            {
                if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
                {
                    username = "";
                    return false;
                }

                return ServiceUserProtectStatic.LocalDefaultUserCheck(
                    Context, App, out username, out err_msg, 8, clientGuid, out  bTrialMode);
            }
            catch (Exception ex)
            {
                username = "";
                err_msg = "DefaultUserCheckWithVersion Exception: " + ex.Message;
                return false;
            }
        }

        public bool LogOut(string clientGuid)
        {       
            return ServiceUserProtectStatic.LogOut(Context, clientGuid);
        }

        public bool UpdateHeartbeat(string clientGuid, out string error)
        {
            return ServiceUserProtectStatic.UpdateHeartbeat(Context, clientGuid, out error);
        }

        public string GetConnectionStatus()
        {
            return ServiceUserProtectStatic.GetConnectionStatus();
        }

        const int NEED_VERSION = 8;
    }
}
