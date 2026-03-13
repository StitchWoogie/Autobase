using AutoLibLocal;
using AutoLibLocal.KeyLock;
using GraphicModule;
using NetTools;
using NetTools.Hash;
using Newtonsoft.Json.Linq;
using ReportBasicLib;
using ReportModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace PortalServerWeb.AutoWeb.Service
{
    enum EnumDownLoadFileResult
    {
        failed = 0,
        matched = 1,
        download = 2,
        notfound = 3,
        //HashMismatched = 4,
    }

    // 서비스 타입 구현
    //[ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall,ConcurrencyMode=ConcurrencyMode.Multiple)]
    class ServiceDataGateServer : IServiceDataGateServer
    {
        // 아래의 함수는 10.3.2.4 이전에 웹서버에서 asmx 서비스에서 ID=-2 로 호출했기 때문에 호환성을 위해서 그냥 둔다.
        public string CheckServerEvent(int id, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return null;

            string retn = String.Format("{0},{1}",
                FormAlarmEvent.nEventAlarmTrans,
                FormAlarmEvent.blockAlarmConfirmNot.Count);

            return retn;
        }

        // 아래의 함수는 10.3.2.4 이전에 웹서버에서 asmx 서비스에서 ID=-2 로 호출했기 때문에 호환성을 위해서 그냥 둔다.
        public string GetAlarmEvents(int id, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return null;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.Add("alarm_datetime", typeof(DateTime));
            dt.Columns.Add("Tag");
            dt.Columns.Add("Description");
            dt.Columns.Add("Message");
            dt.Columns.Add("alarm_type");
            dt.Columns.Add("bAlarm", typeof(Int32));
            dt.Columns.Add("bConfirmMethod", typeof(Int32));
            dt.Columns.Add("tReturn", typeof(DateTime));
            dt.Columns.Add("ID", typeof(Int32));

            DataRow row;
            ALARM_CONFIRMATION_STRUCT list;

            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {

                for (int i = 0; i < FormAlarmEvent.blockAlarmConfirmNot.Count; i++)
                {
                    list = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[i];

                    row = dt.NewRow();
                    row[0] = list.t.ToDateTime();
                    row[1] = list.tag;
                    row[2] = list.description;
                    row[3] = list.message;
                    row[4] = list.msg_type;
                    row[5] = list.bAlarm;
                    row[6] = list.bConfirmMethod;
                    row[7] = list.tReturn.ToDateTime();
                    row[8] = list.id;
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);

            return ds.GetXml();
        }

        // 아래의 함수는 10.3.2.4 이전에 웹서버에서 asmx 서비스에서 ID=-2 로 호출했기 때문에 호환성을 위해서 그냥 둔다.
        // 클라이언트의 각종 명령을 수행한다.
        public bool ExecuteCommand(int id, string command, string argument, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, command + argument, hash, out err_msg)) return false;

            CommaBlockString comma = new CommaBlockString();
            comma.Set(argument);

            if (command == "EventAlarmConfirm")
            {
                int alarm_id = comma.GetInt();
                FormAlarmEvent.ConfirmOneID(alarm_id);
            }
            else if (command == "EventAlarmDelete")
            {
                int alarm_id = comma.GetInt();
                FormAlarmEvent.DeleteOneID(alarm_id);
            }

            return true;
        }

        bool CheckHashAndID(MethodBase mb, int id, string source, string hash, out string err_msg)
        {
            MakeHashCrc mhc = new MakeHashCrc();
            string h = mhc.ComputeHash(mb.Name + id.ToString() + source);

            if (h != hash)
            {
                err_msg = String.Format("Hash mismatched at {0} method", mb.Name);
                return false;
            }

            if (!DataGateClient.CheckID(id))
            {
                err_msg = String.Format("Connection id {0} is not exists.", id);
                return false;
            }

            err_msg = "";
            return true;
        }
        //###################################################################################################################################


        //###################################################################################################################################
        // 10.3.2.5 부터 WCF 를 LocalMain과 웹서버에 동일하게 두어 UWP에서는 항상 ServiceDataGateServer.svc 하나만으로 호출할 수 있게 하였다.
        // 즉 Net.tcp 와 http  프로토콜을 동시에 사용할 수 있도록 지원하였다.

        
        byte[] MakeHash(byte[] org)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(org);

            return result;
        }

        static byte[] KEY = { 0xee, 0xc3, 0x1d, 0x2d, 0x1c, 0xcb, 0x18, 0x53, 0x13, 0x29, 0x19, 0x2c, 0x11, 0x23, 0x36, 0x4b };
        static byte[] IV = { 0xf5, 0x69, 0x73, 0x83, 0x1c, 0xa3, 0x1b, 0x2a, 0xc2, 0x2e, 0x42, 0x20, 0x1f, 0x58, 0x1a, 0x27 };

        bool CheckHash(List<byte[]> args)
        {
            if (args.Count == 0) return false;

            byte[] hash = args[args.Count-1];

            int nTotalCount = 0;

            for (int i = 0; i < args.Count-1; i++)
                nTotalCount += args[i].Length;
            nTotalCount += 4;

            byte[] total = new byte[nTotalCount];

            int pos = 0;
            int length;

            for (int i = 0; i < args.Count-1; i++)
            {
                length = args[i].Length;
                Array.Copy(args[i], 0, total, pos, length);
                pos += length;
            }

            total[pos++] = (byte)'~';
            total[pos++] = (byte)'$';
            total[pos++] = (byte)'7';
            total[pos++] = (byte)'l';

            byte[] total_hash = MakeHash(total);

            if (!total_hash.SequenceEqual(hash))
            {
                return false;
            }

            return true;
        }

        void AddCRC(List<byte[]> args)
        {
            if (args.Count == 0) return;    // 하나도 없으면 CRC를 넣을 필요는 없다. 쓸데없이 시간만 걸림

            int nTotalCount = 0;

            for (int i = 0; i < args.Count; i++)
                nTotalCount += args[i].Length;
            nTotalCount += 4;

            byte[] total = new byte[nTotalCount];

            int pos = 0;
            int length;

            for (int i = 0; i < args.Count; i++)
            {
                length = args[i].Length;
                Array.Copy(args[i], 0, total, pos, length);
                pos += length;
            }

            total[pos++] = (byte)'~';
            total[pos++] = (byte)'$';
            total[pos++] = (byte)'7';
            total[pos++] = (byte)'l';

            byte[] total_hash = MakeHash(total);

            args.Add(total_hash);
        }

        void AddString(List<byte[]> recv, string data)
        {
            byte[] baData = Encoding.UTF8.GetBytes(data);
            byte[] baData_Hash = NetTools.Cryptography.CryptoAES.Encrypt(baData, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            recv.Add(baData_Hash);
        }

        void AddBytes(List<byte[]> recv, byte[] data)
        {
            byte[] baData_Hash = NetTools.Cryptography.CryptoAES.Encrypt(data, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            recv.Add(baData_Hash);
        }

        // 오류메시지는 맨처음에 오류메시지를 넣고 -1을 반환한다.
        void AddErrorMessage(List<byte[]> recv, string format, params object[] args)
        {
            string msg = String.Format(format, args);
            byte[] baMessage = Encoding.UTF8.GetBytes(msg);
            byte[] baMessage_Hash = NetTools.Cryptography.CryptoAES.Encrypt(baMessage, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            recv.Add(baMessage_Hash);

            AddCRC(recv);
        }

        // 공통으로 사용할 수 있는 함수 
        public async Task<CommonMethodResult> CommonMethod(List<byte[]> args)
        {
            List<byte[]> result = new List<byte[]>();
            int code;
            if (args.Count < 2)
            {
                AddErrorMessage(result, "Hash Error args.Count < 2");
                return new CommonMethodResult
                {
                    Code = -1,
                    Recv = result
                };
            }

            if (!CheckHash(args))
            {
                AddErrorMessage(result, "Hash mismatched.");
                return new CommonMethodResult
                {
                    Code = -1,
                    Recv = result
                };
            }

            args.RemoveAt(args.Count - 1);  // Hash는 검사했으므로 제거한다.

            // arg0 = id,comand 
            byte[] baArg0 = NetTools.Cryptography.CryptoAES.Decrypt(args[0], KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

            string sArg0 = Encoding.UTF8.GetString(baArg0);
            CommaTextReader comma = new CommaTextReader();
            comma.Set(sArg0);
            int id = comma.GetInt();
            string sCommand = comma.GetString();

            if (sCommand == "V2_Connect")
            {
                code = V2_Connect(comma, result);

                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }

            if (!DataGateClient.CheckID(id))
            {
                AddErrorMessage(result, "Connection id {0} is not exists.", id);
                return new CommonMethodResult
                {
                    Code = -1,
                    Recv = result
                };
            }

            if (sCommand == "V2_GetServerVersion")
            {
                code = V2_GetServerVersion(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_DisConnect")
            {
                code = V2_DisConnect(id);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_DefaultUserCheck")
            {
                code = V2_DefaultUserCheck(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_CheckUserName")
            {
                code = V2_CheckUserName(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetWebKeyInfo")
            {
                code = V2_GetWebKeyInfo(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetProjectFilesInfo")
            {
                code = V2_GetProjectFilesInfo(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_DownLoadFile")
            {
                code = V2_DownLoadFile(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_DownLoadFileWithCompare") // ViewMain에서 사용
            {
                code = V2_DownLoadFileWithCompare(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetTagValues")
            {
                code = V2_GetTagValues(comma, args, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_WriteCurr")
            {
                code = V2_WriteCurr(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetAlarmLists")
            {
                code = await V2_GetAlarmLists(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetAlarmFile")
            {
                code = await V2_GetAlarmFile(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetLogLists")
            {
                code = await V2_GetLogLists(comma, result).ConfigureAwait(false);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetLogFile")
            {
                code = await V2_GetLogFile(comma, result).ConfigureAwait(false);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetReportLists")
            {
                code = V2_GetReportLists(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetReportFileByBitmap")
            {
                code = await V2_GetReportFileByBitmap(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetDataAi")
            {
                code = await V2_GetDataAi(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetDataDi")
            {
                code = await V2_GetDataDi(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetDataSetFromDsn")
            {
                code = V2_GetDataSetFromDsn(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_DataSetCommand")
            {
                code = V2_DataSetCommand(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_CheckServerEvent")
            {
                code = V2_CheckServerEvent(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetAlarmEvents")
            {
                code = V2_GetAlarmEvents(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_ExecuteCommand")
            {
                code = V2_ExecuteCommand(comma , result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "MilliDataTrend")
            {
                code = MilliDataTrend(comma, args, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "MilliDataGetGroupLists")
            {
                code = MilliDataGetGroupLists(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "MilliDataGetFileLists")
            {
                code = MilliDataGetFileLists(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "MilliDataGetOneFile")
            {
                code = MilliDataGetOneFile(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetDemandNewSnapshot")
            {
                code = V2_GetDemandNewSnapshot(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_GetDemandNewConfig")
            {
                code = V2_GetDemandNewConfig(comma, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            // Preset V2
            else if (sCommand == "V2_PresetGetList")
            {
                code = V2_PresetGetList(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            else if (sCommand == "V2_PresetGet")
            {
                code = V2_PresetGet(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            else if (sCommand == "V2_PresetSave")
            {
                code = V2_PresetSave(comma, args, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            else if (sCommand == "V2_PresetDelete")
            {
                code = V2_PresetDelete(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            else if (sCommand == "V2_PresetApply")
            {
                code = V2_PresetApply(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            else if (sCommand == "V2_PresetCapture")
            {
                code = V2_PresetCapture(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            // Recipe V2
            else if (sCommand == "V2_RecipeDownload")
            {
                code = V2_RecipeDownload(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            else if (sCommand == "V2_RecipeUpload")
            {
                code = V2_RecipeUpload(comma, result);
                return new CommonMethodResult { Code = code, Recv = result };
            }
            // ==================== Python AI Engine ====================
            else if (sCommand == "V2_PythonAiIsConnected")
            {
                code = V2_PythonAiIsConnected(result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_PythonAiCall")
            {
                code = await V2_PythonAiCall(comma, args, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else if (sCommand == "V2_PythonAiScript")
            {
                code = await V2_PythonAiScript(comma, args, result);
                return new CommonMethodResult
                {
                    Code = code,
                    Recv = result
                };
            }
            else
            {
                AddErrorMessage(result, "{0} Command Unknown.", sCommand);
                 return new CommonMethodResult
                {
                    Code = -1,
                    Recv = result
                };
            }
        }

        int V2_Connect(CommaTextReader comma, List<byte[]> result)
        {
            AddString(result, DataGateClient.Connect().ToString());
            AddCRC(result);
            return 1;
        }

        int V2_GetServerVersion(CommaTextReader comma, List<byte[]> result)
        {
            AddString(result, Application.ProductVersion);
            AddCRC(result);
            return 1;
        }

        int V2_DisConnect(int id)
        {
            DataGateClient.DisConnect(id);
            return 1;
        }

        int V2_DefaultUserCheck(CommaTextReader comma, List<byte[]> result)
        {
            string username;
            string err_msg;

            int retn = LocalDefaultUserCheck(out username, out err_msg, 0) ? 1 : 0;

            AddString(result, username);
            AddCRC(result);

            return retn;
        }

        bool LocalDefaultUserCheck(out string username, out string err_msg, int need_version)
        {
            err_msg = "";

            DataLocal local = new DataLocal();

            bool retn = local.DefaultUserCheck(out err_msg, out username);

            if (retn)
            {
                //if (!PlusUserCount(out err_msg, need_version)) return false;
            }

            return retn;
        }

        bool LocalCheckUserName(string username, string passcode, string passcode256, out string err_msg, int need_version)
        {
            err_msg = "";

            DataLocal local = new DataLocal();

            bool retn = local.CheckUserName(out err_msg, username, passcode, passcode256);
            if (retn == true)
            {
                //if (!PlusUserCount(out err_msg, need_version)) return false;
            }
            return retn;
        }

        int V2_CheckUserName(CommaTextReader comma, List<byte[]> result)
        {
            int retn;
            string err_msg;

            const int NEED_VERSION = 8; // 2008년 이상 키만 사용가능

            string username = comma.GetString();
            string passcode256 = comma.GetString();

            retn = LocalCheckUserName(username, "", passcode256, out err_msg, NEED_VERSION) ? 1 : 0;

            return retn;
        }

        /// <summary>
        /// Web KeyLock 정보 반환 (WebViewer용)
        /// 응답: "bExistWebKey,nWebUserCount,nKeyLockVersion,sSerialNumber,"
        /// </summary>
        int V2_GetWebKeyInfo(CommaTextReader comma, List<byte[]> result)
        {
            string info = String.Format("{0},{1},{2},{3},",
                KeyLock.bExistWebKey ? "1" : "0",
                KeyLock.nWebUserCount,
                KeyLock.nKeyLockVersion,
                KeyLock.sSerialNumber ?? "");

            AddString(result, info);
            AddCRC(result);
            return 1;
        }

        void RecurseProjectFilesInfo(string root_dir, string dir, DataTable dt)
        {
            if (!Directory.Exists(dir)) return; 

            DirectoryInfo infodir = new DirectoryInfo(dir);

            foreach (DirectoryInfo di in infodir.GetDirectories())
            {
                RecurseProjectFilesInfo(root_dir, di.FullName, dt);
            }

            foreach (FileInfo fi in infodir.GetFiles("*.*"))
            {
                string ext = fi.Extension;
                //if (String.Compare(ext, ".ANI", true) == 0) continue;
                if (String.Compare(ext, ".PCX", true) == 0) continue;
                //if (String.Compare(ext, ".GIF", true) == 0) continue;
                //if (String.Compare(ext, ".BMP", true) == 0) continue;
                //if (String.Compare(ext, ".TIF", true) == 0) continue;
                //if (String.Compare(ext, ".JPG", true) == 0) continue;
                //if (String.Compare(ext, ".WMF", true) == 0) continue;
                //if (String.Compare(ext, ".EMF", true) == 0) continue;

                DataRow row = dt.NewRow();

                //ProjectFilesInfo2 file = new ProjectFilesInfo2();

                row["filename"] = fi.FullName.Substring(root_dir.Length + 1);
                row["filesize"] = fi.Length;
                row["filetime"] = fi.LastWriteTimeUtc;
                dt.Rows.Add(row);
            }
        }

        string GetWebProjectFolder()
        {
            string project_dir = TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebCopyTarget", "C:\\WebRoot");

            project_dir = String.Format("{0}\\AutoWeb\\Project", project_dir);

            return project_dir;
        }

        int V2_GetProjectFilesInfo(CommaTextReader comma, List<byte[]> result)
        {
            string project_dir = GetWebProjectFolder();

            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            ds.Tables.Add(dt);

            DataColumn dc;

            dc = new DataColumn("filename", typeof(string));
            dt.Columns.Add(dc);
            dc = new DataColumn("filesize", typeof(int));
            dt.Columns.Add(dc);
            dc = new DataColumn("filetime", typeof(DateTime));
            dt.Columns.Add(dc);

            RecurseProjectFilesInfo(project_dir, project_dir, dt);

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }
        
        int V2_DownLoadFile(CommaTextReader comma, List<byte[]> result)
        {
            string filename = comma.GetString();

            string path;

            string project_dir = GetWebProjectFolder();

            path = String.Format("{0}\\{1}", project_dir, filename);
            if (!File.Exists(path))
            {
                return 0;
            }

            FileInfo info = new FileInfo(path);

            if (info.Length == 0)	// file size 0
            {
                return 0;
            }

            byte[] data = File.ReadAllBytes(path);

            AddBytes(result, data);
            AddCRC(result);

            return 1;
        }

        int V2_DownLoadFileWithCompare(CommaTextReader comma, List<byte[]> result)
        {
            int retn = (int)EnumDownLoadFileResult.failed;

            string dir = comma.GetString();
            string filename = comma.GetString();
            DateTime file_time = comma.GetDateTime();
            long file_size = comma.GetLong();

            string path;

            string project_dir = GetWebProjectFolder();

            path = String.Format("{0}\\{1}\\{2}", project_dir, dir, filename);
            if (!File.Exists(path))
            {
                retn = (int)EnumDownLoadFileResult.notfound;
                return retn;
            }

            DateTime dt;

            dt = File.GetLastWriteTime(path);
            FileInfo info = new FileInfo(path);

                if (file_time == dt && file_size == info.Length)
                {
                    retn = (int)EnumDownLoadFileResult.matched;
                    return retn;
                }

            file_time = dt;
            file_size = info.Length;

            if (info.Length == 0)	// file size 0
            {
                AddBytes(result, new byte[0]);
                AddString(result, ConvertTool.ToDateTimeString(file_time));
                AddString(result, file_size.ToString());
                AddCRC(result);
                retn = (int)EnumDownLoadFileResult.download;
                return retn;
            }

            byte[] data = File.ReadAllBytes(path);

            AddBytes(result, data);
            AddString(result, ConvertTool.ToDateTimeString(file_time));
            AddString(result, file_size.ToString());
            AddCRC(result);

            retn = (int)EnumDownLoadFileResult.download;

            return retn;
        }

        int V2_GetTagValues(CommaTextReader comma, List<byte[]> args, List<byte[]> result)
        {
            if (args.Count <= 1)
            {
                AddErrorMessage(result, "V2_GetTagValues arg1 not found.");
                return -1;
            }
            byte[] baArg1 = NetTools.Cryptography.CryptoAES.Decrypt(args[1], KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            string sArg1 = Encoding.UTF8.GetString(baArg1);

            CommaTextReader comma2 = new CommaTextReader();
            comma2.Set(sArg1);

            string tag;
            string curr = "";
            CommaTextMaker ctm = new CommaTextMaker();

            while (!comma2.IsEOS())
            {
                tag = comma2.GetString();
                SharedTag.GetCurr(tag, ref curr);
                ctm.Write("{0},", curr);
            }

            AddString(result, ctm.GetResult());
            AddCRC(result);
            
            return 1;
        }

        int V2_WriteCurr(CommaTextReader comma, List<byte[]> result)
        {
            string username = comma.GetString();
            string computer = comma.GetString();
            string tag = comma.GetString();
            string val = comma.GetString();

            string ip = "?.?.?.?";
            SharedTag.SetCurr(tag, val, username, ip, computer);

            return 1;
        }

        async Task< int> V2_GetAlarmLists(CommaTextReader comma, List<byte[]> result)
        {
            DataLocal local = new DataLocal();
            DataSet dataSet = await local.GetAlarmListsAsync();
            AddString(result, dataSet.GetXml());
            AddCRC(result);

            return 1;
        }

        async Task<int> V2_GetAlarmFile(CommaTextReader comma, List<byte[]> result)
        {
            string filename = comma.GetString();

            DataLocal local = new DataLocal();
            //AddString(result, local.GetAlarmFile(filename).GetXml());

            DataSet dataSet = await local.GetAlarmFileAsync(filename);
            AddString(result, dataSet.GetXml());
            AddCRC(result);

            return 1;
        }
        
        async Task<int> V2_GetLogLists(CommaTextReader comma, List<byte[]> result)
        {
            DataLocal local = new DataLocal();
            DataSet ds = await local.GetLogLists().ConfigureAwait(false);
            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

        async Task< int> V2_GetLogFile(CommaTextReader comma, List<byte[]> result)
        {
            string filename = comma.GetString();

            DataLocal local = new DataLocal();
            DataSet ds = await local.GetLogFile(filename).ConfigureAwait(false);
            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

        int V2_GetReportLists(CommaTextReader comma, List<byte[]> result)
        {
            string[] lists = ReportLib.GetReportListsLocal();

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.Add("Filename");
            dt.Columns.Add("Description");

            DataRow row;

            if (lists != null)
            {
                for (int i = 0; i < lists.Length; i += 2)
                {
                    row = dt.NewRow();
                    row[0] = lists[i + 0];
                    row[1] = lists[i + 1];
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }
        
        async Task<int> V2_GetReportFileByBitmap(CommaTextReader comma, List<byte[]> result)
        {
            string filename = comma.GetString();
            int year = comma.GetInt();
            int month = comma.GetInt();
            int day = comma.GetInt();
            int hour = comma.GetInt();
            int minute = comma.GetInt();
            int second = comma.GetInt();

            ReportPrint report = new ReportPrint();

            ReportConfig.tHandReportTime = TimeUtil.MakeDateTime(year, month, day, hour, minute, 0);
            
            List<byte[]> data = await report.MakeBitmapReportResults("", filename, EnumHandAuto.HAND_MODE);

            for (int i = 0; i < data.Count; i++)
            {
                AddBytes(result, data[i]);
            }
            AddCRC(result);

            return 1;
        }

        async Task< int> V2_GetDataAi(CommaTextReader comma, List<byte[]> result)
        {
            string tag = comma.GetString();
            int value_type = comma.GetInt();
            int data_time = comma.GetInt();
            int year = comma.GetInt();
            int month = comma.GetInt();
            int day = comma.GetInt();
            int hour = comma.GetInt();
            int minute = comma.GetInt();
            int data_count = comma.GetInt();
            int data_gab = comma.GetInt();

            DataLocal local = new DataLocal();
            DataSet ds = await local.GetDataAi(tag, (EnumDataType)value_type, (EnumDataTime)data_time, year, month, day, hour, minute, data_count, data_gab);

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

       async Task< int> V2_GetDataDi(CommaTextReader comma, List<byte[]> result)
        {
            string tag = comma.GetString();
            int value_type = comma.GetInt();
            int data_time = comma.GetInt();
            int year = comma.GetInt();
            int month = comma.GetInt();
            int day = comma.GetInt();
            int hour = comma.GetInt();
            int minute = comma.GetInt();
            int data_count = comma.GetInt();
            int data_gab = comma.GetInt();

            DataLocal local = new DataLocal();
            DataSet ds = await local.GetDataDi(tag, (EnumDataType)value_type, (EnumDataTime)data_time, year, month, day, hour, minute, data_count, data_gab);

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

        int V2_GetDataSetFromDsn(CommaTextReader comma, List<byte[]> result)
        {
            string dsn = comma.GetString();
            string command = comma.GetString();

            DataLocal local = new DataLocal();

            string err_msg;
            System.Data.DataSet ds = local.GetDataSetFromDsn(dsn, command, out err_msg);

            if (ds == null) return 0;

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

        int V2_DataSetCommand(CommaTextReader comma, List<byte[]> result)
        {
            string dsn = comma.GetString();
            string command = comma.GetString();

            DataLocal local = new DataLocal();

            string err_msg;

            return local.DataSetCommand(dsn, command, out err_msg) ? 1 : 0;
        }

        int V2_CheckServerEvent(CommaTextReader comma, List<byte[]> result)
        {
            string retn = String.Format("{0},{1}",
                FormAlarmEvent.nEventAlarmTrans,
                FormAlarmEvent.blockAlarmConfirmNot.Count);

            AddString(result, retn);
            AddCRC(result);

            return 1;
        }

        int V2_GetAlarmEvents(CommaTextReader comma, List<byte[]> result)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.Add("alarm_datetime", typeof(DateTime));
            dt.Columns.Add("Tag");
            dt.Columns.Add("Description");
            dt.Columns.Add("Message");
            dt.Columns.Add("alarm_type");
            dt.Columns.Add("bAlarm", typeof(Int32));
            dt.Columns.Add("bConfirmMethod", typeof(Int32));
            dt.Columns.Add("tReturn", typeof(DateTime));
            dt.Columns.Add("ID", typeof(Int32));

            DataRow row;
            ALARM_CONFIRMATION_STRUCT list;

            lock (FormAlarmEvent.blockAlarmConfirmNot)
            {

                for (int i = 0; i < FormAlarmEvent.blockAlarmConfirmNot.Count; i++)
                {
                    list = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[i];

                    row = dt.NewRow();
                    row[0] = list.t.ToDateTime();
                    row[1] = list.tag;
                    row[2] = list.description;
                    row[3] = list.message;
                    row[4] = list.msg_type;
                    row[5] = list.bAlarm;
                    row[6] = list.bConfirmMethod;
                    row[7] = list.tReturn.ToDateTime();
                    row[8] = list.id;
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

        int V2_ExecuteCommand(CommaTextReader comma, List<byte[]> result)
        {
            string command = comma.GetString();
            
            if (command == "EventAlarmConfirm")
            {
                int alarm_id = comma.GetInt();
                FormAlarmEvent.ConfirmOneID(alarm_id);
            }
            else if (command == "EventAlarmDelete")
            {
                int alarm_id = comma.GetInt();
                FormAlarmEvent.DeleteOneID(alarm_id);
            }

            return 1;
        }

        int MilliDataTrend(CommaTextReader comma, List<byte[]> args, List<byte[]> result)
        {
            if (args.Count <= 1)
            {
                AddErrorMessage(result, "MilliDataTrend arg1 not found.");
                return -1;
            }
            byte[] baArg1 = NetTools.Cryptography.CryptoAES.Decrypt(args[1], KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            string sArg1 = Encoding.UTF8.GetString(baArg1);

            //ServiceDataTag.TryLogIn("");

            MilliDataTrendReader mdtr = new MilliDataTrendReader();
            string dsn = comma.GetString();
            DateTime start_time = comma.GetDateTime();
            int show_unit = comma.GetInt();
            int data_cycle = comma.GetInt();
            int time_select_option = comma.GetInt();

            MilliData.LoadMilliData(MilliData.blockMilliData);  // 

            DataSet ds = mdtr.ReadData(sArg1, dsn, start_time, show_unit, data_cycle, time_select_option);

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }

        string ListStringToCommaString(List<string> array)
        {
            CommaTextMaker maker = new CommaTextMaker();

            for (int i = 0; i < array.Count; i++)
            {
                maker.Write("{0},", array[i]);
            }

            return maker.GetResult();
        }

        int MilliDataGetGroupLists(CommaTextReader comma, List<byte[]> result)
        {
            //ServiceDataTag.TryLogIn("");

            MilliDataBasicReader reader = new MilliDataBasicReader();

            string lists = ListStringToCommaString(reader.LoadGroupList());

            AddString(result, lists);
            AddCRC(result);

            return 1;
        }

        int MilliDataGetFileLists(CommaTextReader comma, List<byte[]> result)
        {
            string group_name = comma.GetString();

            //ServiceDataTag.TryLogIn("");

            MilliDataBasicReader reader = new MilliDataBasicReader();

            string lists = ListStringToCommaString(reader.LoadMemberList(group_name));

            AddString(result, lists);
            AddCRC(result);

            return 1;
        }

        int MilliDataGetOneFile(CommaTextReader comma, List<byte[]> result)
        {
            string group_name = comma.GetString();
            string file_name = comma.GetString();

            //ServiceDataTag.TryLogIn("");

            MilliDataBasicReader reader = new MilliDataBasicReader();

            string err_msg;

            DataSet ds = reader.GetFile(group_name, file_name, out err_msg);

            if (ds == null)
            {
                AddErrorMessage(result, err_msg);
                return -1;
            }

            AddString(result, ds.GetXml());
            AddCRC(result);

            return 1;
        }


        int V2_GetDemandNewSnapshot(CommaTextReader comma, List<byte[]> result)
        {
            string blockId = comma.GetString();

            var manager = LocalMain.DemandNew.CheckEngineDemandNew.Manager;
            if (manager == null)
            {
                AddErrorMessage(result, "DemandNew not initialized.");
                return -1;
            }

            var bus = manager.GetBus(blockId);
            if (bus == null)
            {
                AddErrorMessage(result, "DemandNew block '{0}' not found.", blockId);
                return -1;
            }

            var snapshot = bus.LatestSnapshot;
            if (snapshot == null)
            {
                AddErrorMessage(result, "No snapshot available for block '{0}'.", blockId);
                return -1;
            }

            string data = AutoLibLocal.DemandNew.DemandSnapshotSerializer.Serialize(snapshot);
            AddString(result, data);
            AddCRC(result);
            return 1;
        }

        int V2_GetDemandNewConfig(CommaTextReader comma, List<byte[]> result)
        {
            string blockId = comma.GetString();

            var manager = LocalMain.DemandNew.CheckEngineDemandNew.Manager;
            if (manager == null)
            {
                AddErrorMessage(result, "DemandNew not initialized.");
                return -1;
            }

            var engine = manager.GetEngine(blockId);
            if (engine == null)
            {
                AddErrorMessage(result, "DemandNew block '{0}' not found.", blockId);
                return -1;
            }

            string data = AutoLibLocal.DemandNew.DemandNewConfigLoader.FormatConfigLine(engine.Config);
            AddString(result, data);
            AddCRC(result);
            return 1;
        }

        #region Preset V2 Handlers (LocalMain 실행)

        int V2_PresetGetList(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string projectPath = TotalConfig.sDirWorkProject;
                if (string.IsNullOrEmpty(projectPath))
                    projectPath = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.Combine(projectPath, "Presets");
                if (!Directory.Exists(dir))
                {
                    AddString(result, "0");
                    AddCRC(result);
                    return 1;
                }

                string[] files = Directory.GetFiles(dir, "*.json");
                CommaTextMaker maker = new CommaTextMaker();
                maker.Write("{0},", files.Length);
                for (int i = 0; i < files.Length; i++)
                {
                    string name = Path.GetFileNameWithoutExtension(files[i]);
                    string date = File.GetLastWriteTime(files[i]).ToString("yyyy-MM-dd HH:mm");
                    maker.Write("{0},{1},", name, date);
                }
                AddString(result, maker.GetResult());
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetGetList: {0}", ex.Message); return -1; }
        }

        int V2_PresetGet(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                string projectPath = TotalConfig.sDirWorkProject;
                if (string.IsNullOrEmpty(projectPath))
                    projectPath = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.Combine(projectPath, "Presets");
                string filePath = Path.Combine(dir, SanitizePresetFileName(presetName) + ".json");
                if (!File.Exists(filePath))
                { AddErrorMessage(result, "Preset '{0}' not found.", presetName); return -1; }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                AddString(result, json);
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetGet: {0}", ex.Message); return -1; }
        }

        int V2_PresetSave(CommaTextReader comma, List<byte[]> args, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                if (args.Count <= 1)
                { AddErrorMessage(result, "V2_PresetSave: JSON data not provided."); return -1; }

                byte[] baArg1 = NetTools.Cryptography.CryptoAES.Decrypt(args[1], KEY, IV,
                    System.Security.Cryptography.CipherMode.CBC,
                    System.Security.Cryptography.PaddingMode.PKCS7);
                string json = Encoding.UTF8.GetString(baArg1);

                string projectPath = TotalConfig.sDirWorkProject;
                if (string.IsNullOrEmpty(projectPath))
                    projectPath = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.Combine(projectPath, "Presets");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string filePath = Path.Combine(dir, SanitizePresetFileName(presetName) + ".json");
                File.WriteAllText(filePath, json, new UTF8Encoding(true));

                AddString(result, "OK");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetSave: {0}", ex.Message); return -1; }
        }

        int V2_PresetDelete(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                string projectPath = TotalConfig.sDirWorkProject;
                if (string.IsNullOrEmpty(projectPath))
                    projectPath = AppDomain.CurrentDomain.BaseDirectory;
                string dir = Path.Combine(projectPath, "Presets");
                string filePath = Path.Combine(dir, SanitizePresetFileName(presetName) + ".json");
                if (!File.Exists(filePath))
                { AddErrorMessage(result, "Preset '{0}' not found.", presetName); return -1; }

                File.Delete(filePath);
                AddString(result, "OK");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetDelete: {0}", ex.Message); return -1; }
        }

        int V2_PresetApply(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                string variantName = comma.GetString();

                var task = LocalMain.PresetScriptBridge.PresetApply(presetName, variantName);
                task.Wait();
                var (success, error) = task.Result;
                if (success)
                {
                    AddString(result, "");
                    AddCRC(result);
                    return 1;
                }
                else
                {
                    AddErrorMessage(result, error ?? "PresetApply failed.");
                    return -1;
                }
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetApply: {0}", ex.Message); return -1; }
        }

        int V2_PresetCapture(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string templatePreset = comma.GetString();
                string variantName = comma.GetString();
                string saveName = comma.GetString();

                var task = LocalMain.PresetScriptBridge.PresetCapture(templatePreset, variantName, saveName);
                task.Wait();
                var (success, error) = task.Result;
                if (success)
                {
                    AddString(result, "");
                    AddCRC(result);
                    return 1;
                }
                else
                {
                    AddErrorMessage(result, error ?? "PresetCapture failed.");
                    return -1;
                }
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetCapture: {0}", ex.Message); return -1; }
        }

        static string SanitizePresetFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string r = name;
            for (int i = 0; i < invalid.Length; i++)
                r = r.Replace(invalid[i], '_');
            return r;
        }

        #endregion

        #region Recipe V2 Handlers (LocalMain 실행)

        int V2_RecipeDownload(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string recipeName = comma.GetString();
                var task = LocalMain.CheckEngineRecipe.RecipeDownload(recipeName, null);
                task.Wait();
                var (success, error) = task.Result;
                if (success)
                {
                    AddString(result, "");
                    AddCRC(result);
                    return 1;
                }
                else
                {
                    AddErrorMessage(result, error ?? "RecipeDownload failed.");
                    return -1;
                }
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_RecipeDownload: {0}", ex.Message); return -1; }
        }

        int V2_RecipeUpload(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string recipeName = comma.GetString();
                var task = LocalMain.CheckEngineRecipe.RecipeUpload(recipeName, null);
                task.Wait();
                var (success, error) = task.Result;
                if (success)
                {
                    AddString(result, "");
                    AddCRC(result);
                    return 1;
                }
                else
                {
                    AddErrorMessage(result, error ?? "RecipeUpload failed.");
                    return -1;
                }
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_RecipeUpload: {0}", ex.Message); return -1; }
        }

        #endregion

        // ==================== Python AI Engine ====================

        /// <summary>V2_PythonAiIsConnected → "1" or "0"</summary>
        int V2_PythonAiIsConnected(List<byte[]> result)
        {
            bool connected = LocalMain.PythonAi.PythonAiManager.IsConnected;
            AddString(result, connected ? "1" : "0");
            AddCRC(result);
            return 1;
        }

        /// <summary>
        /// V2_PythonAiCall(service, timeoutMs) + arg1=payloadJson
        /// 범용 Python AI 서비스 호출.
        /// </summary>
        async Task<int> V2_PythonAiCall(CommaTextReader comma, List<byte[]> args, List<byte[]> result)
        {
            try
            {
                string service = comma.GetString();
                int timeoutMs = comma.GetInt();

                string payloadJson = "";
                if (args.Count > 1)
                {
                    byte[] baPayload = NetTools.Cryptography.CryptoAES.Decrypt(
                        args[1], KEY, IV,
                        System.Security.Cryptography.CipherMode.CBC,
                        System.Security.Cryptography.PaddingMode.PKCS7);
                    payloadJson = Encoding.UTF8.GetString(baPayload);
                }

                string resultJson = await LocalMain.PythonAi.PythonAiScriptBridge
                    .CallServiceAsync(service, payloadJson, timeoutMs)
                    .ConfigureAwait(false);

                AddString(result, resultJson ?? "");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            {
                AddErrorMessage(result, "V2_PythonAiCall: {0}", ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// V2_PythonAiScript + arg1=scriptData (JSON: {"code":"...", "payload":{...}})
        /// Python 스크립트 실행 with 태그 프리로드.
        /// </summary>
        async Task<int> V2_PythonAiScript(CommaTextReader comma, List<byte[]> args, List<byte[]> result)
        {
            try
            {
                if (args.Count < 2)
                {
                    AddErrorMessage(result, "V2_PythonAiScript: missing script data (arg1)");
                    return -1;
                }

                byte[] baData = NetTools.Cryptography.CryptoAES.Decrypt(
                    args[1], KEY, IV,
                    System.Security.Cryptography.CipherMode.CBC,
                    System.Security.Cryptography.PaddingMode.PKCS7);
                string scriptDataJson = Encoding.UTF8.GetString(baData);
                JObject scriptData = JObject.Parse(scriptDataJson);

                string code = scriptData.Value<string>("code") ?? "";
                JObject payloadObj = scriptData["payload"] as JObject;

                var response = await LocalMain.PythonAi.PythonAiScriptBridge
                    .ExecuteWithPreloadAsync(code, payloadObj)
                    .ConfigureAwait(false);

                // Serialize PythonAiMessage → JSON
                var obj = new JObject();
                obj["ok"] = response != null && response.IsSuccess;
                if (response != null)
                {
                    if (response.Result != null)
                    {
                        if (response.Result is JToken jt)
                            obj["result"] = jt;
                        else
                            obj["result"] = JToken.FromObject(response.Result);
                    }
                    if (response.Error != null)
                        obj["error"] = response.Error;
                    if (response.DurationMs.HasValue)
                        obj["elapsed_ms"] = response.DurationMs.Value;
                }

                AddString(result, obj.ToString(Newtonsoft.Json.Formatting.None));
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            {
                AddErrorMessage(result, "V2_PythonAiScript: {0}", ex.Message);
                return -1;
            }
        }
    }
}

public class DataGateClient
{
    public static List<DataGateClient> arrayClient = new List<DataGateClient>();
    public static Random rand = new Random();

    public int id;
    public string ip;
    public int nFrameSend;
    public int nFrameRecv;
    public object syncLock = new object();  // 다른 thread에서 사용하면 충돌이 나기 때문에 변경시 lock을 해서 사용한다.

    public static int Connect()
    {
        while (true)
        {
        seek_next:
            int id = rand.Next(1, int.MaxValue);
            for (int i = 0; i < arrayClient.Count; i++)
            {
                if (arrayClient[i].id == id)
                {
                    goto seek_next;
                }
            }

            DataGateClient client = new DataGateClient();
            client.id = id;
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            client.ip = endpoint.Address;

            arrayClient.Add(client);

            return id;
        }
    }

    public static void DisConnect(int id)
    {
        for (int i = 0; i < arrayClient.Count; i++)
        {
            if (arrayClient[i].id == id)
            {
                //using (arrayClient)
                //{
                //arrayClient.RemoveAt(i);
                //}
                arrayClient[i].id = -1;
                return;
            }
        }
    }

    public static bool CheckID(int id)
    {
        if (id == -2) return true;  // Connection 없이 바로 연결할 경우. 웹 서버를 거쳐서 호출하는 경우 접속없이 연결할 때 사용

        for (int i = 0; i < arrayClient.Count; i++)
        {
            if (arrayClient[i].id == id)
            {
                arrayClient[i].nFrameRecv++;
                return true;
            }
        }

        return false;
    }

}



