using AutoLibLocal;
using NetTools;
using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WebService3"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WebService3.svc나 WebService3.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WebService3 : IWebService3
    {
        //const string sKey = "_+^&54361()-=";

        private readonly byte[] keyHash = new byte[32]
        {
            0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7, 0xB7, 0xf6, 0xbB, 0xcc,
            0xec, 0x12, 0xe4, 0x71, 0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7
        };


        private const int NEED_VERSION = 8;   // 2008년 이상 키만 사용가능

        private HttpContext Ctx
        {
            get { return HttpContext.Current; }
        }

        private HttpApplicationState App => HttpContext.Current.Application;

        // 공통 초기화 (필요하면 확장)
        private void InitProjectDir()
        {
            if (Ctx == null) return;

            string work_dir = ProjectLib.GetWorkDir(Ctx.Request);
            TotalConfig.sDirWorkProject = work_dir;
        }

        #region AES & Hash Helper

        // AES_256 암호화
        private string AESEncrypt256(string input, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = new byte[16];

            ICryptoTransform encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            return Convert.ToBase64String(xBuff);
        }

        // AES_256 복호화
        private string AESDecrypt256(string input, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = new byte[16];

            ICryptoTransform decrypt = aes.CreateDecryptor();
            byte[] xBuff;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            return Encoding.UTF8.GetString(xBuff);
        }

        private byte[] Hash(string source)
        {
            if (ConfigWeb.SecurityLevel() == 4) // 256 bit Hash
            {
                byte[] hash_s = Encoding.UTF8.GetBytes(source + "*&%6^0");
                // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
                // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
                SHA256 sha = new SHA256Managed();
                return sha.ComputeHash(hash_s);
            }

            // 160 bit Hash
            else {
                byte[] hash_s = Encoding.UTF8.GetBytes(source + "~!@#");
                SHA1 sha = new SHA1CryptoServiceProvider();
                return sha.ComputeHash(hash_s);
            }
        }

        private bool CheckKeyAndHash(string key_enc, string hash_source, byte[] hash, out string err_msg)
        {
            err_msg = "O.K";

            long ticks_s = DateTime.UtcNow.Ticks;

            byte[] hash2 = Hash(hash_source);

            if (!hash.SequenceEqual(hash2))
            {
                err_msg = "Hash mismatched";
                return false;
            }

            string decrypted = AESDecrypt256(key_enc, keyHash);
            long ticks_c = ConvertTool.ToInt64(decrypted);

            long ticks_gab = ticks_s - ticks_c;

            /* 세군데 PC에서는 되는데 한군데에서 안 되어서 일단 보류했다.
            if (Math.Abs(ticks_gab) > 50000000) // 50,000,000 ticks = 5초;
            {
                err_msg = "key 값이 적정 범위를 넘었습니다.";
                return false;
            }*/

            return true;
        }

        #endregion

        #region Conversation Key

        // SecurityLevel 3부터 지원
        public string GetConversationKey(string request,
            out string key2_enc, out string key3_enc,
            out string key4_enc, out string key5_enc)
        {
            // request는 아무거나 준다.
            string result = DateTime.UtcNow.Ticks.ToString();

            string encoded = AESEncrypt256(result, keyHash);

            key2_enc = AESEncrypt256("2222222222", keyHash);  // 의미없는 값이라도 준다.
            key3_enc = AESEncrypt256("3333333333", keyHash);  // 의미없는 값이라도 준다.
            key4_enc = AESEncrypt256("4444444444", keyHash);  // 의미없는 값이라도 준다.
            key5_enc = AESEncrypt256("5555555555", keyHash);  // 의미없는 값이라도 준다.

            return encoded;
        }

        #endregion

        #region 로그인 계열

        // SecurityLevel 3부터 지원
        public bool DefaultUserCheck(string key_enc,
            out string username_enc,
            out string err_msg,
            byte[] hash,
            string clientGuid,
            out bool bTrialMode)
        {
            bTrialMode = false;

            try
            {
                username_enc = "";

                if (!CheckKeyAndHash(key_enc,
                                     "DefaultUserCheck" + key_enc,
                                     hash,
                                     out err_msg))
                {
                    return false;
                }

                InitProjectDir();

                string username;
                // ⚠ 여기서 ServiceUserProtect.LocalDefaultUserCheck는
                // HttpContext 버전 오버로드가 있어야 함.
                bool retn = ServiceUserProtectStatic.LocalDefaultUserCheck(
                    Ctx, App,
                    out username,
                    out err_msg,
                    NEED_VERSION,
                    clientGuid,
                    out bTrialMode);

                username_enc = AESEncrypt256(username, keyHash);
                return retn;
            }
            catch (Exception ex)
            {
                username_enc = "";
                err_msg = "DefaultUserCheck exception: " + ex.Message;
                return false;
            }
        }

        // SecurityLevel 3부터 지원
        public bool CheckUserName(string key_enc,
            string username_enc,
            string passcode_enc,
            out string err_msg,
            byte[] hash,
            string clientGuid,
            out bool bTrailMode)
        {
            bTrailMode = false;
            try
            {
                if (!CheckKeyAndHash(key_enc,
                                     "CheckUserName" + key_enc + username_enc + passcode_enc,
                                     hash,
                                     out err_msg))
                {
                    return false;
                }

                InitProjectDir();

                string username = AESDecrypt256(username_enc, keyHash);
                string passcode256 = AESDecrypt256(passcode_enc, keyHash);


                // SecurityLevel 3부터는 암호가 Hash256만 사용한다.  10.3.1.6 이전에 설정된 사용자는 암호를 바꾸어주면 Hash256 값이 생성된다.

                return ServiceUserProtectStatic.LocalCheckUserName(
                    Ctx, App,
                    username,
                    null,
                    passcode256,
                    out err_msg,
                    NEED_VERSION,
                    clientGuid,
                    out bTrailMode);
            }
            catch (Exception ex)
            {
                err_msg = "CheckUserName exception: " + ex.Message;
                return false;
            }
        }

        #endregion

        #region WriteCurr

        // SecurityLevel 3부터 지원
        public bool WriteCurr(string key_enc,
            string username_enc,
            string computername_enc,
            string tag_enc,
            string val_enc,
            byte[] hash, string guid)
        {
            try
            {
                string err_msg;

                if(!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    return false;
                }

                if (!CheckKeyAndHash(key_enc,
                                     "WriteCurr" + key_enc + username_enc + computername_enc + tag_enc + val_enc,
                                     hash,
                                     out err_msg))
                {
                    // 검증 실패하면 아무 작업 안 함
                    return false;
                }

                InitProjectDir();

                string username = AESDecrypt256(username_enc, keyHash);
                string computername = AESDecrypt256(computername_enc, keyHash);
                string tag = AESDecrypt256(tag_enc, keyHash);
                string val = AESDecrypt256(val_enc, keyHash);

                string clientip = (Ctx != null && Ctx.Request != null)
                    ? Ctx.Request.UserHostAddress
                    : "";

                //string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);
                // ,가 포함된 값은 ,가 빠져서 2016-12-21 CommaTextMaker 로 변경함. 
                CommaTextMaker ctm = new CommaTextMaker();
                ctm.Write("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);
                string buf = ctm.GetResult();

                string recv_data;
                ServiceDataTagStatic.SendAndGetData(Ctx, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);

                string msg = string.Format("Tag={0},Value={1},IP={2}", tag, val, clientip);
                ClassMain.SaveLog(out err_msg, EnumLogType.Write, msg);
                return true;
            }
            catch(Exception ex)
            {
                string err_msg = "WriteCurr exception: " + ex.Message;
                Debug.WriteLine(err_msg);
                return false;
            }
        }

        #endregion

        #region DataSet 계열 (Mdb, Dsn, Command, DbType)

        public string GetDataSetFromMdb(string key_enc,
            string filename_enc,
            string command_enc,
            out string err_msg,
            byte[] hash)
        {
            try
            {
                if (!CheckKeyAndHash(key_enc,
                                     "GetDataSetFromMdb" + key_enc + filename_enc + command_enc,
                                     hash,
                                     out err_msg))
                {
                    return null;
                }

                InitProjectDir();

                string filename = AESDecrypt256(filename_enc, keyHash);
                string command = AESDecrypt256(command_enc, keyHash);

                string work_dir = TotalConfig.sDirWorkProject;

                DataLocal local = new DataLocal();
                System.Data.DataSet ds = local.GetDataSetFromMdb(filename, command, out err_msg);

                if (ds == null) return null;

                // XML을 암호화해서 반환
                return AESEncrypt256(ds.GetXml(), keyHash);
            }
            catch (Exception ex)
            {
                err_msg = "GetDataSetFromMdb exception: " + ex.Message;
                return null;
            }
        }

        public string GetDataSetFromDsn(string key_enc,
            string dsn_enc,
            string command_enc,
            out string err_msg,
            byte[] hash)
        {
            try
            {
                if (!CheckKeyAndHash(key_enc,
                                     "GetDataSetFromDsn" + key_enc + dsn_enc + command_enc,
                                     hash,
                                     out err_msg))
                {
                    return null;
                }

                InitProjectDir();

                string dsn = AESDecrypt256(dsn_enc, keyHash);
                string command = AESDecrypt256(command_enc, keyHash);

                string work_dir = TotalConfig.sDirWorkProject;

                DataLocal local = new DataLocal();
                System.Data.DataSet ds = local.GetDataSetFromDsn(dsn, command, out err_msg);

                if (ds == null) return null;

                return AESEncrypt256(ds.GetXml(), keyHash);
            }
            catch (Exception ex)
            {
                err_msg = "GetDataSetFromDsn exception: " + ex.Message;
                return null;
            }
        }

        public bool DataSetCommand(string key_enc,
            string dsn_enc,
            string command_enc,
            out string err_msg,
            byte[] hash, string guid)
        {
            try
            {
                if(!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    err_msg = "Client not connected";
                    return false;
                }

                if (!CheckKeyAndHash(key_enc,
                                     "DataSetCommand" + key_enc + dsn_enc + command_enc,
                                     hash,
                                     out err_msg))
                {
                    return false;
                }

                InitProjectDir();

                string dsn = AESDecrypt256(dsn_enc, keyHash);
                string command = AESDecrypt256(command_enc, keyHash);

                string work_dir = TotalConfig.sDirWorkProject;

                DataLocal local = new DataLocal();
                return local.DataSetCommand(dsn, command, out err_msg);
            }
            catch (Exception ex)
            {
                err_msg = "DataSetCommand exception: " + ex.Message;
                return false;
            }
        }

        public bool GetConnectionStringDbType(string key_enc,
            string dsn_enc,
            out int dbtype,
            byte[] hash)
        {
            try
            {
                string err_msg;
                dbtype = (int)EnumDbType.Normal;

                if (!CheckKeyAndHash(key_enc,
                                     "GetConnectionStringDbType" + key_enc + dsn_enc,
                                     hash,
                                     out err_msg))
                {
                    return false;
                }

                InitProjectDir();

                string dsn = AESDecrypt256(dsn_enc, keyHash);

                EnumDbType type;
                bool retn = DbTool.GetConnectionStringDbType(dsn, out type);
                dbtype = (int)type;
                return retn;
            }
            catch (Exception)
            {
                dbtype = (int)EnumDbType.Normal;
                return false;
            }
        }

        #endregion
    }
}
