using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NetTools;
using GraphicModule;
using System.Data;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.Service
{
    public class ClassDataGateServer
    {
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

            byte[] hash = args[args.Count - 1];

            int nTotalCount = 0;

            for (int i = 0; i < args.Count - 1; i++)
                nTotalCount += args[i].Length;
            nTotalCount += 4;

            byte[] total = new byte[nTotalCount];

            int pos = 0;
            int length;

            for (int i = 0; i < args.Count - 1; i++)
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
        public void AddErrorMessage(List<byte[]> recv, string format, params object[] args)
        {
            string msg = String.Format(format, args);
            byte[] baMessage = Encoding.UTF8.GetBytes(msg);
            byte[] baMessage_Hash = NetTools.Cryptography.CryptoAES.Encrypt(baMessage, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            recv.Add(baMessage_Hash);

            AddCRC(recv);
        }

        // 공통으로 사용할 수 있는 함수 
        public int  CommonMethod(List<byte[]> args, out List<byte[]> result)
        {
            try
            {
                result = new List<byte[]>();

                if (args.Count < 2)
                {
                    AddErrorMessage(result, "Hash Error args.Count < 2");
                    return -1;
                }

                if (!CheckHash(args))
                {
                    AddErrorMessage(result, "Hash mismatched.");
                    return -1;
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
                    AddErrorMessage(result, "V2_Connect 아직 지원중");
                    return -1;
                    //return V2_Connect(comma, result);
                }

                /*
                if (!DataGateClient.CheckID(id))
                {
                    AddErrorMessage(result, "Connection id {0} is not exists.", id);
                    return -1;
                }*/

                if (sCommand == "")
                {
                    AddErrorMessage(result, "sCommand 아직 지원중");
                    return -1;
                }
                /*
                if (sCommand == "V2_GetServerVersion")
                {
                    return V2_GetServerVersion(comma, result);
                }
                else if (sCommand == "V2_DisConnect")
                {
                    return V2_DisConnect(id);
                }
                else if (sCommand == "V2_DefaultUserCheck")
                {
                    return V2_DefaultUserCheck(comma, result);
                }
                else if (sCommand == "V2_CheckUserName")
                {
                    return V2_CheckUserName(comma, result);
                }
                else if (sCommand == "V2_GetProjectFilesInfo")
                {
                    return V2_GetProjectFilesInfo(comma, result);
                }
                else if (sCommand == "V2_DownLoadFile")
                {
                    return V2_DownLoadFile(comma, result);
                }
                else if (sCommand == "V2_DownLoadFileWithCompare") // ViewMain에서 사용
                {
                    return V2_DownLoadFileWithCompare(comma, result);
                }
                else if (sCommand == "V2_GetTagValues")
                {
                    return V2_GetTagValues(comma, args, result);
                }
                else if (sCommand == "V2_WriteCurr")
                {
                    return V2_WriteCurr(comma, result);
                }
                else if (sCommand == "V2_GetAlarmLists")
                {
                    return V2_GetAlarmLists(comma, result);
                }
                else if (sCommand == "V2_GetAlarmFile")
                {
                    return V2_GetAlarmFile(comma, result);
                }
                else if (sCommand == "V2_GetLogLists")
                {
                    return V2_GetLogLists(comma, result);
                }
                else if (sCommand == "V2_GetLogFile")
                {
                    return V2_GetLogFile(comma, result);
                }
                else if (sCommand == "V2_GetReportLists")
                {
                    return V2_GetReportLists(comma, result);
                }
                else if (sCommand == "V2_GetReportFileByBitmap")
                {
                    return V2_GetReportFileByBitmap(comma, result);
                }
                else if (sCommand == "V2_GetDataAi")
                {
                    return V2_GetDataAi(comma, result);
                }
                else if (sCommand == "V2_GetDataDi")
                {
                    return V2_GetDataDi(comma, result);
                }
                else if (sCommand == "V2_GetDataSetFromDsn")
                {
                    return V2_GetDataSetFromDsn(comma, result);
                }
                else if (sCommand == "V2_DataSetCommand")
                {
                    return V2_DataSetCommand(comma, result);
                }
                else if (sCommand == "V2_CheckServerEvent")
                {
                    return V2_CheckServerEvent(comma, result);
                }
                else if (sCommand == "V2_GetAlarmEvents")
                {
                    return V2_GetAlarmEvents(comma, result);
                }
                else if (sCommand == "V2_ExecuteCommand")
                {
                    return V2_ExecuteCommand(comma, result);
                }*/
                // ==================== Python AI Engine (→ LocalMain 전달) ====================
                else if (sCommand == "V2_PythonAiIsConnected"
                    || sCommand == "V2_PythonAiCall"
                    || sCommand == "V2_PythonAiScript")
                {
                    return ForwardToLocalMain(args, result);
                }
                else if (sCommand == "MilliDataTrend")
                {
                    return MilliDataTrend(comma, args, result);
                }
                else if (sCommand == "MilliDataGetGroupLists")
                {
                    return MilliDataGetGroupLists(comma, result);
                }
                else if (sCommand == "MilliDataGetFileLists")
                {
                    return MilliDataGetFileLists(comma, result);
                }
                else if (sCommand == "MilliDataGetOneFile")
                {
                    return MilliDataGetOneFile(comma, result);
                }
                else if (sCommand == "V2_PresetGetList")
                {
                    return V2_PresetGetList(comma, result);
                }
                else if (sCommand == "V2_PresetGet")
                {
                    return V2_PresetGet(comma, result);
                }
                else if (sCommand == "V2_PresetSave")
                {
                    return V2_PresetSave(comma, args, result);
                }
                else if (sCommand == "V2_PresetDelete")
                {
                    return V2_PresetDelete(comma, result);
                }
                else if (sCommand == "V2_PresetApply")
                {
                    return V2_PresetApply(comma, result);
                }
                else if (sCommand == "V2_PresetCapture")
                {
                    return V2_PresetCapture(comma, result);
                }
                else if (sCommand == "V2_RecipeDownload")
                {
                    return V2_RecipeDownload(comma, result);
                }
                else if (sCommand == "V2_RecipeUpload")
                {
                    return V2_RecipeUpload(comma, result);
                }
                else
                {
                    AddErrorMessage(result, "{0} Command Unknown.", sCommand);
                    return -1;
                }
            }
            catch(Exception ex)
            {
                result = new List<byte[]>();
                AddErrorMessage(result, "Exception : {0}", ex.Message);
                return -1;
            }
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

            ServiceDataTagStatic.TryLogIn("");

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
            ServiceDataTagStatic.TryLogIn("");

            MilliDataBasicReader reader = new MilliDataBasicReader();

            string lists = ListStringToCommaString(reader.LoadGroupList());

            AddString(result, lists);
            AddCRC(result);

            return 1;
        }

        int MilliDataGetFileLists(CommaTextReader comma, List<byte[]> result)
        {
            string group_name = comma.GetString();

            ServiceDataTagStatic.TryLogIn("");

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

            ServiceDataTagStatic.TryLogIn("");

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

        #region Preset Handlers (단일 경유점: LocalMain의 ClassDataGateServer를 경유)

        void InitPresetProxy()
        {
            ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
            ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
            ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
            ConfigVarTotal.nServicePort = 8732;
        }

        int V2_PresetGetList(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_PresetGetList");
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, sldg.GetResultString(0));
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetGetList error: {0}", ex.Message); return -1; }
        }

        int V2_PresetGet(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_PresetGet", presetName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, sldg.GetResultString(0));
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetGet error: {0}", ex.Message); return -1; }
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

                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                sldg.PrepareArg1(json);
                int retn = sldg.Command("V2_PresetSave", presetName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, "OK");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetSave error: {0}", ex.Message); return -1; }
        }

        int V2_PresetDelete(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_PresetDelete", presetName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, "OK");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetDelete error: {0}", ex.Message); return -1; }
        }

        int V2_PresetApply(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string presetName = comma.GetString();
                string variantName = comma.GetString();
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_PresetApply", presetName + "," + variantName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, "OK");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetApply error: {0}", ex.Message); return -1; }
        }

        int V2_PresetCapture(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string templatePreset = comma.GetString();
                string variantName = comma.GetString();
                string saveName = comma.GetString();
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_PresetCapture", templatePreset + "," + variantName + "," + saveName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, "OK");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_PresetCapture error: {0}", ex.Message); return -1; }
        }

        #endregion

        #region Recipe Handlers (단일 경유점: LocalMain 경유)

        int V2_RecipeDownload(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string recipeName = comma.GetString();
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_RecipeDownload", recipeName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, "");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_RecipeDownload error: {0}", ex.Message); return -1; }
        }

        int V2_RecipeUpload(CommaTextReader comma, List<byte[]> result)
        {
            try
            {
                string recipeName = comma.GetString();
                InitPresetProxy();
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_RecipeUpload", recipeName);
                if (retn != 1)
                { AddErrorMessage(result, sldg.sErrorMessage ?? "LocalMain 통신 실패"); return -1; }

                AddString(result, "");
                AddCRC(result);
                return 1;
            }
            catch (Exception ex)
            { AddErrorMessage(result, "V2_RecipeUpload error: {0}", ex.Message); return -1; }
        }


        #endregion


        // ==================== LocalMain Forwarding ====================

        /// <summary>
        /// V2_PythonAi* 커맨드를 LocalMain WCF로 전달.
        /// args는 이미 hash가 제거된 상태이므로 CRC를 재생성하여 전송.
        /// </summary>
        int ForwardToLocalMain(List<byte[]> args, List<byte[]> result)
        {
            try
            {
                // CRC 재생성 (CommonMethod 진입 시 hash가 제거되었으므로)
                AddCRC(args);

                // LocalMain WCF 호출
                var service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();
                byte[][] sendArray = new byte[args.Count][];
                for (int i = 0; i < args.Count; i++)
                    sendArray[i] = args[i];

                var cmr = service.CommonMethod(sendArray);

                // 응답 전달 (이미 암호화+CRC 포함)
                if (cmr.Recv != null)
                {
                    for (int i = 0; i < cmr.Recv.Length; i++)
                        result.Add(cmr.Recv[i]);
                }

                return cmr.Code;
            }
            catch (Exception ex)
            {
                AddErrorMessage(result, "ForwardToLocalMain: {0}", ex.Message);
                return -1;
            }
        }

    }
}
