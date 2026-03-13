using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using NetTools;
using System.Threading;
using AutoLibLocal;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for WebService3
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService3 : System.Web.Services.WebService
    {
        //const string sKey = "_+^&54361()-=";

        readonly byte[] keyHash = new byte[32] {
            0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7, 0xB7, 0xf6, 0xbB, 0xcc,
            0xec, 0x12, 0xe4, 0x71, 0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7};

        //AES_256 암호화
        String AESEncrypt256(String Input, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);
            return Output;
        }

        //AES_256 복호화
        String AESDecrypt256(String Input, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Encoding.UTF8.GetString(xBuff);
            return Output;
        }

        byte[] Hash(string source)
        {
            if (ConfigWeb.SecurityLevel() == 4) // 256 bit Hash
            {
                byte[] hash_s = Encoding.UTF8.GetBytes(source + "*&%6^0");
                // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
                // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
                SHA256 sha = new SHA256Managed();
                byte[] result = sha.ComputeHash(hash_s);

                return result;
            }
            else // 160 bit Hash
            {
                byte[] hash_s = Encoding.UTF8.GetBytes(source + "~!@#");
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] result = sha.ComputeHash(hash_s);

                return result;
            }
        }

        bool CheckKeyAndHash(string key_enc, string hash_source, byte[] hash, out string err_msg)
        {
            err_msg = "O.K";

            long ticks_s = DateTime.UtcNow.Ticks;   // 시간을 먼저 얻어둔다.

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

        // SecurityLevel 3부터 지원
        [WebMethod]
        public string GetConversationKey(string request, out string key2_enc, out string key3_enc, out string key4_enc, out string key5_enc)
        {
            // request는 아무거나 준다.
            string result = DateTime.UtcNow.Ticks.ToString();

            string encoded = AESEncrypt256(result, keyHash);

            key2_enc = AESEncrypt256("2222222222", keyHash);    // 의미없는 값이라도 준다.
            key3_enc = AESEncrypt256("3333333333", keyHash);    // 의미없는 값이라도 준다.
            key4_enc = AESEncrypt256("4444444444", keyHash);    // 의미없는 값이라도 준다.
            key5_enc = AESEncrypt256("5555555555", keyHash);    // 의미없는 값이라도 준다.

            return encoded;
        }

        const int NEED_VERSION = 8; // 2008년 이상 키만 사용가능

        // SecurityLevel 3부터 지원
        [WebMethod]
        public bool DefaultUserCheck(string key_enc, out string username_enc, out string err_msg, byte[] hash, string clientGuid)
        {
            username_enc = "";

            if (!CheckKeyAndHash(key_enc, "DefaultUserCheck" + key_enc, hash, out err_msg))
            {
                return false;
            }

            string username = "";

            bool retn = ServiceUserProtect.LocalDefaultUserCheck(this, out username, out err_msg, NEED_VERSION, clientGuid);

            username_enc = AESEncrypt256(username, keyHash);

            return retn;
        }

        // SecurityLevel 3부터 지원
        [WebMethod]
        public bool CheckUserName(string key_enc, string username_enc, string passcode_enc, out string err_msg, byte[] hash, string clientGuid)
        {
            if (!CheckKeyAndHash(key_enc, "CheckUserName" + key_enc + username_enc + passcode_enc, hash, out err_msg))
            {
                return false;
            }

            string username = AESDecrypt256(username_enc, keyHash);
            string passcode256 = AESDecrypt256(passcode_enc, keyHash);

            // SecurityLevel 3부터는 암호가 Hash256만 사용한다.  10.3.1.6 이전에 설정된 사용자는 암호를 바꾸어주면 Hash256 값이 생성된다.
            return ServiceUserProtect.LocalCheckUserName(this, username, null, passcode256, out err_msg, NEED_VERSION, clientGuid);
        }

        /*
        [WebMethod(Description = "CommonMethod Session", EnableSession = true)]
        public string CommonMethod(string key_enc, string command_enc, string args_enc, byte[] hash)
        {
            string err_msg;
            CommaTextMaker writer = new CommaTextMaker();

            if (!CheckKeyAndHash(key_enc, "CommonMethod" + key_enc + command_enc + args_enc, hash, out err_msg))
            {
                writer.Write("Result={0},", false);
                writer.Write("ErrorMsg={0},", err_msg);
                return writer.GetResult();
            }

            string command = AESDecrypt256(command_enc, keyHash);
            string args = AESDecrypt256(args_enc, keyHash);

            CommaTextReader reader = new CommaTextReader();
            reader.Set(args);

            if (command == "WriteCurr")
            {
                CommandWriteCurr(reader, writer);
            }
            
            return "";
        }

        public bool CommandWriteCurr(CommaTextReader reader, CommaTextMaker writer)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TotalConfig.sDirWorkProject = work_dir;
            string clientip = Context.Request.UserHostAddress;

            string tag="", val="", username="", computername="";
            string item;

            while (!reader.IsEOS())
            {
                item = reader.GetString();    
            }
            
            string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);

            string recv_data;

            ServiceDataTag.SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
        }*/

        // SecurityLevel 3부터 지원
        [WebMethod(Description = "WriteCurr Session", EnableSession = true)]
        public void WriteCurr(string key_enc, string username_enc, string computername_enc, string tag_enc, string val_enc, byte[] hash)
        {
            string err_msg;

            if (!CheckKeyAndHash(key_enc, "WriteCurr" + key_enc + username_enc + computername_enc + tag_enc + val_enc, hash, out err_msg))
            {
                //return false;
            }

            string username = AESDecrypt256(username_enc, keyHash);
            string computername = AESDecrypt256(computername_enc, keyHash);
            string tag = AESDecrypt256(tag_enc, keyHash);
            string val = AESDecrypt256(val_enc, keyHash);

            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TotalConfig.sDirWorkProject = work_dir;
            string clientip = Context.Request.UserHostAddress;

            //string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);
            // ,가 포함된 값은 ,가 빠져서 2016-12-21 CommaTextMaker 로 변경함. 
            CommaTextMaker ctm = new CommaTextMaker();
            ctm.Write("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);
            string buf = ctm.GetResult();

            string recv_data;

            ServiceDataTag.SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);

            string msg = String.Format("Tag={0},Value={1},IP={2}", tag, val, clientip);

            ClassMain.SaveLog(out err_msg, EnumLogType.Write, msg);
        }

        [WebMethod]
        public string GetDataSetFromMdb(string key_enc, string filename_enc, string command_enc, out string err_msg, byte[] hash)
        {
            if (!CheckKeyAndHash(key_enc, "GetDataSetFromMdb" + key_enc + filename_enc + command_enc, hash, out err_msg))
            {
                return null;
            }

            string filename = AESDecrypt256(filename_enc, keyHash);
            string command = AESDecrypt256(command_enc, keyHash);

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;

            System.Data.DataSet ds = local.GetDataSetFromMdb(filename, command, out err_msg);

            if (ds == null) return null;
            return AESEncrypt256(ds.GetXml(), keyHash);
        }

        [WebMethod]
        public string GetDataSetFromDsn(string key_enc, string dsn_enc, string command_enc, out string err_msg, byte[] hash)
        {
            if (!CheckKeyAndHash(key_enc, "GetDataSetFromDsn" + key_enc + dsn_enc + command_enc, hash, out err_msg))
            {
                return null;
            }

            string dsn = AESDecrypt256(dsn_enc, keyHash);
            string command = AESDecrypt256(command_enc, keyHash);

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;

            System.Data.DataSet ds = local.GetDataSetFromDsn(dsn, command, out err_msg);

            if (ds == null) return null;
            return AESEncrypt256(ds.GetXml(), keyHash);
        }

        [WebMethod]
        public bool DataSetCommand(string key_enc, string dsn_enc, string command_enc, out string err_msg, byte[] hash)
        {
            if (!CheckKeyAndHash(key_enc, "DataSetCommand" + key_enc + dsn_enc + command_enc, hash, out err_msg))
            {
                return false;
            }

            string dsn = AESDecrypt256(dsn_enc, keyHash);
            string command = AESDecrypt256(command_enc, keyHash);

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.DataSetCommand(dsn, command, out err_msg);
        }

        [WebMethod]
        public bool GetConnectionStringDbType(string key_enc, string dsn_enc, out int dbtype, byte[] hash)
        {
            string err_msg;
            dbtype = (int)EnumDbType.Normal;

            if (!CheckKeyAndHash(key_enc, "GetConnectionStringDbType" + key_enc + dsn_enc, hash, out err_msg))
            {
                return false;
            }

            string dsn = AESDecrypt256(dsn_enc, keyHash);

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;
            EnumDbType type;
            bool retn = DbTool.GetConnectionStringDbType(dsn, out type);
            dbtype = (int)type;
            return retn;
        }
    }
}
