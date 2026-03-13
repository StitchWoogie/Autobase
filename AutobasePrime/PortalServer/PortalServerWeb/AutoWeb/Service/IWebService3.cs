using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWebService3"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IWebService3
    {
        [OperationContract]
        string GetConversationKey(string request,
           out string key2_enc, out string key3_enc,
           out string key4_enc, out string key5_enc);

        [OperationContract]
        bool DefaultUserCheck(string key_enc,
            out string username_enc,
            out string err_msg,
            byte[] hash,
            string clientGuid, out bool bTrialMode);

        [OperationContract]
        bool CheckUserName(string key_enc,
            string username_enc,
            string passcode_enc,
            out string err_msg,
            byte[] hash,
            string clientGuid, out bool bTrialMode);

        [OperationContract]
        bool WriteCurr(string key_enc,
            string username_enc,
            string computername_enc,
            string tag_enc,
            string val_enc,
            byte[] hash, string guid);

        [OperationContract]
        string GetDataSetFromMdb(string key_enc,
            string filename_enc,
            string command_enc,
            out string err_msg,
            byte[] hash);

        [OperationContract]
        string GetDataSetFromDsn(string key_enc,
            string dsn_enc,
            string command_enc,
            out string err_msg,
            byte[] hash);

        [OperationContract]
        bool DataSetCommand(string key_enc,
            string dsn_enc,
            string command_enc,
            out string err_msg,
            byte[] hash, string guid);

        [OperationContract]
        bool GetConnectionStringDbType(string key_enc,
            string dsn_enc,
            out int dbtype,
            byte[] hash);
    }
}
