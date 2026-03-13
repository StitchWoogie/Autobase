using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IServiceUserProtect"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IServiceUserProtect
    {
        [OperationContract]
        bool CheckUserName(string username, string password, string clientGuid, out string err_msg, out bool bTrialMode);

        [OperationContract]
        bool CheckUserNameWithVersion(string username, string password, string clientGuid, out string err_msg, out bool bTrialMode);

        [OperationContract]
        UserCountInfo GetUserCount();

        [OperationContract]
        bool DefaultUserCheck(string clientGuid, out string username, out string err_msg, out bool bTrialMode);

        [OperationContract]
        bool DefaultUserCheckWithVersion(string clientGuid, out string username, out string err_msg, out bool bTrialMode);

        [OperationContract]
        bool LogOut(string clientGuid);

        [OperationContract]
        bool UpdateHeartbeat(string clientGuid, out string error);

        [OperationContract]
        string GetConnectionStatus();
    }

    [Serializable]
    public class UserCountInfo
    {
        public int KeylockCount { get; set; }
        public int LicenseCount { get; set; }
        public int TrialCount { get; set; }
        public int TotalCount => LicenseCount + TrialCount;
    }
}
