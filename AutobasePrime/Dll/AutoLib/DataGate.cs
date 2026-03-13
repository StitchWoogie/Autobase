using System;
using System.Data;
using System.Collections;
using AutoLibLocal;
using System.Net;
using NetTools;
using System.Security.Cryptography;
using NetTools.Hash;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using static AutoLibLocal.DataLocal;

namespace AutoLib
{
    /// <summary>
    /// Summary description for DataGate.
    /// </summary>
    /// 

    public class WebCommInfo
    {
        public static int nFailCount = 0;
        public static string sErrorMessage = "";

        public static void SetError(string format)//, params object[] args)
        {
            nFailCount++;

            sErrorMessage = format;//String.Format(format, args);
        }

        public static void Reset()
        {
            nFailCount = 0;
            sErrorMessage = "";
        }
    }

    public class DataGate
    {
        private static string _clientGuid;  //250825 PSU 
        public static bool WebDemo = false;

        public DataGate()
        {
            //
            // TODO: Add constructor logic here
            //

            // 클라이언트 GUID 생성 (한 번만) 250812 PSU
            if (string.IsNullOrEmpty(_clientGuid))
            {
                _clientGuid = GenerateClientGuid();
            }
        }

        /// <summary>
        /// 클라이언트 GUID 생성
        /// </summary>
        private string GenerateClientGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 현재 클라이언트 GUID 반환
        /// </summary>
        public static string GetClientGuid()
        {
            return _clientGuid;
        }

        public static async Task CheckServiceAlive()
        {
            ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

            try
            {
                int retn = await data.CheckServiceAliveAsync(500);
                ConfigVarTotal.bWebServiceAlive = true; //20250825 PSU 
                WebCommInfo.Reset();
            }
            catch
            {
                ConfigVarTotal.bWebServiceAlive = false; //20250825 PSU 
            }
        }

        public async Task< DataSet> GetTagValueListAsync(ArrayList array)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return null;
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    CommaTextMaker ctm = new CommaTextMaker();

                    for (int i = 0; i < array.Count; i++)
                    {
                        ctm.Write("{0},", (string)array[i]);
                    }

                    sldg.PrepareArg1(ctm.GetResult());
                    int retn = sldg.Command("V2_GetTagValues", ctm.GetResult());

                    if (retn == 1)
                    {
                        CommaTextReader comma = new CommaTextReader();
                        DataSet ds = new DataSet();
                        DataTable dt = new DataTable();

                        comma.Set(sldg.GetResultString(0));

                        dt.Columns.Add(new DataColumn("Tag", typeof(string)));
                        dt.Columns.Add(new DataColumn("curr", typeof(string)));

                        DataRow row;
                        for (int i = 0; i < array.Count; i++)
                        {
                            row = dt.NewRow();
                            row["tag"] = (string)array[i];
                            row["curr"] = comma.GetString(); ;
                            dt.Rows.Add(row);
                        }

                        ds.Tables.Add(dt);

                        return ds;
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                        return null;
                    }
                }
                else
                {

                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();
                    DataSet ds;

                    try
                    {
                        ds = await data.GetTagValueListAsync(array.ToArray());
                    }
                    catch (Exception ex)
                    {

                        ds = null;
                        WebCommInfo.SetError("DataGate.cs GetTagValueList()\n" + ex.Message);
                    }

                    return ds;
                }
            }
        }

        byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];

            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2 + 0] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }

            return b;
        }

        async Task WriteCurrSecurity(string tag, string value)
        {
            ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

            string key_enc = ServiceLib.GetKeyEnc();
            string username_enc = ServiceLib.Encrypt(SharedData.userInfo.sUsername);
            string computername_enc = ServiceLib.Encrypt(TotalConfig.sCurrentComputer);
            string tag_enc = ServiceLib.Encrypt(tag);
            string val_enc = ServiceLib.Encrypt(value);
            byte[] hash = ServiceLib.Hash("WriteCurr" + key_enc + username_enc + computername_enc + tag_enc + val_enc);

            try
            {
                await service.WriteCurrAsync(key_enc, username_enc, computername_enc, tag_enc, val_enc, hash, _clientGuid);
            }
            catch (Exception ex)
            {
                WebCommInfo.SetError("DataGate.cs WriteCurr()\n" + ex.Message);
            }
        }

        // 수동 출력일 때만 원격으로 보내야 한다.
        public async Task WriteCurrDouble(string tag, TagPublicClass tp, double val, bool bHandOperation)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                SharedTag.SetCurr(tag, val);
            }
            else
            {
                if (tp.bLocalTag == 1) return;  // local tag이므로 네트워크를 통해서 출력되지 않는다.
                if (ConfigViewMain.bAllowManualOutputOnClient && !bHandOperation) return;	// 수동출력만 원격 출력 허용

                TagPublicClass tpreal = TagLib.GetDirectTag(tp);    // 9.5.3 부터 추가 클라이언트의 구역을 벗어날때는 간접태그일 경우 실 태그를 연결한다.

                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_WriteCurr", SharedData.userInfo.sUsername, TotalConfig.sCurrentComputer, tpreal.tag, val.ToString());

                    if (retn == -1)
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                }
                else
                {
                    if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                    {
                        await WriteCurrSecurity(tag, val.ToString());
                    }
                    else
                    {
                        if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 0, 4))
                        {
                            ServiceReferenceDataTag2.WebServiceDataTag2Client data = ServiceLib.GetServiceDataTag2();

                            try
                            {
                                byte[] hash_s = StringToBytes("WriteCurr" + SharedData.userInfo.sUsername + TotalConfig.sCurrentComputer + tpreal.tag + val.ToString());

                                SHA1 sha = new SHA1CryptoServiceProvider();
                                byte[] result = sha.ComputeHash(hash_s);

                               await data.WriteCurrAsync(SharedData.userInfo.sUsername, TotalConfig.sCurrentComputer, tpreal.tag, val.ToString(), result, _clientGuid);
                            }
                            catch (Exception ex)
                            {
                                WebCommInfo.SetError("DataGate.cs WriteCurrAI()\n" + ex.Message);
                            }
                        }
                        else
                        {
                            ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                            try
                            {
                                data.WriteCurrAI(tpreal.tag, val, _clientGuid);
                            }
                            catch (Exception ex)
                            {
                                WebCommInfo.SetError("DataGate.cs WriteCurrAI()\n" + ex.Message);
                            }
                        }
                    }
                }
            }
        }

        // 수동 출력일 때만 원격으로 보내야 한다.
        public async Task WriteCurrString(string tag, TagPublicClass tp, string val, bool bHandOperation)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                //DataLocal data = new DataLocal();
                //data.WriteCurrST(tag, val);
                SharedTag.SetCurr(tag, val);
            }
            else
            {
                if (tp.bLocalTag == 1) return;  // local tag이므로 네트워크를 통해서 출력되지 않는다.
                if (ConfigViewMain.bAllowManualOutputOnClient && !bHandOperation) return;	// 수동출력만 원격 출력 허용

                TagPublicClass tpreal = TagLib.GetDirectTag(tp);    // 9.5.3 부터 추가 클라이언트의 구역을 벗어날때는 간접태그일 경우 실 태그를 연결한다.

                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_WriteCurr", SharedData.userInfo.sUsername, TotalConfig.sCurrentComputer, tpreal.tag, val.ToString());

                    if (retn == -1)
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                }
                else
                {
                    if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                    {
                        await WriteCurrSecurity(tag, val.ToString());
                    }
                    else
                    {
                        if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 0, 4))
                        {
                            ServiceReferenceDataTag2.WebServiceDataTag2Client data = ServiceLib.GetServiceDataTag2();

                            try
                            {
                                byte[] hash_s = StringToBytes("WriteCurr" + SharedData.userInfo.sUsername + TotalConfig.sCurrentComputer + tpreal.tag + val);

                                SHA1 sha = new SHA1CryptoServiceProvider();
                                byte[] result = sha.ComputeHash(hash_s);

                                await data.WriteCurrAsync(SharedData.userInfo.sUsername, TotalConfig.sCurrentComputer, tpreal.tag, val, result, _clientGuid);
                            }
                            catch (Exception ex)
                            {
                                WebCommInfo.SetError("DataGate.cs WriteCurrString()\n" + ex.Message);
                            }
                        }
                        else
                        {

                            ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                            try
                            {
                                data.WriteCurrST(tpreal.tag, val, _clientGuid);
                            }
                            catch (Exception ex)
                            {
                                WebCommInfo.SetError("DataGate.cs WriteCurrString()\n" + ex.Message);
                            }
                        }
                    }
                }
            }
        }

        // 수동 출력일 때만 원격으로 보내야 한다. 숫자 문자열에 상관없이 출력하고 지연시간도 있다.
        public async Task WriteCurr(TagPublicClass tp, object val, bool bHandOperation, int delay_sec)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                SharedTag.SetCurrDelaySec(tp.tag, val, delay_sec);
            }
            else
            {
                if (tp.bLocalTag == 1) return;  // local tag이므로 네트워크를 통해서 출력되지 않는다.
                if (!bHandOperation) return;	// 수동출력만 원격 출력 허용

                TagPublicClass tpreal = TagLib.GetDirectTag(tp);    // 9.5.3 부터 추가 클라이언트의 구역을 벗어날때는 간접태그일 경우 실 태그를 연결한다.

                if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                {
                    await WriteCurrSecurity(tpreal.tag, val.ToString());
                }
                else
                {
                    if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 0, 4))
                    {
                        ServiceReferenceDataTag2.WebServiceDataTag2Client data = ServiceLib.GetServiceDataTag2();

                        try
                        {
                            byte[] hash_s = StringToBytes("WriteCurr" + SharedData.userInfo.sUsername + TotalConfig.sCurrentComputer + tpreal.tag + val.ToString());

                            SHA1 sha = new SHA1CryptoServiceProvider();
                            byte[] result = sha.ComputeHash(hash_s);

                            await data.WriteCurrAsync(SharedData.userInfo.sUsername, TotalConfig.sCurrentComputer, tpreal.tag, val.ToString(), result, _clientGuid);
                        }
                        catch (Exception ex)
                        {
                            WebCommInfo.SetError("DataGate.cs WriteCurrAI()\n" + ex.Message);
                        }
                    }
                    else
                    {
                        ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                        try
                        {
                            if (val.GetType() == typeof(string))
                                data.WriteCurrST(tpreal.tag, (string)val, _clientGuid);
                            else
                                data.WriteCurrAI(tpreal.tag, ObjectValue.ToDouble(val), _clientGuid);
                        }
                        catch (Exception ex)
                        {
                            WebCommInfo.SetError("DataGate.cs WriteCurr()\n" + ex.Message);
                        }
                    }
                }
            }
        }

        public async Task<DataSet> GetDataAi(string tag, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetDataAi(tag, data_type, data_time, year, mon, day, hour, min, data_count, data_gab);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_GetDataAi", tag, (int)data_type, (int)data_time, year, mon, day, hour, min, data_count, data_gab);

                    if (retn == -1)
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                    else if (retn == 1)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(sldg.GetResultString(0))));
                        return ds;
                    }

                    return null;
                }
                else
                {
                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                    DataSet ds;
                    try
                    {
                        ds = await data.GetDataAiAsync(tag, (int)data_type, (int)data_time, year, mon, day, hour, min, data_count, data_gab);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                    }
                    return ds;
                }
            }
        }

        public async Task<DataSet> GetDataDi(string tag, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetDataDi(tag, data_type, data_time, year, mon, day, hour, min, data_count, data_gab);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_GetDataDi", tag, (int)data_type, (int)data_time, year, mon, day, hour, min, data_count, data_gab);

                    if (retn == -1)
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                    else if (retn == 1)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(sldg.GetResultString(0))));
                        return ds;
                    }

                    return null;
                }
                else
                {

                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();
                    DataSet ds;

                    try
                    {
                        ds = await  data.GetDataDiAsync(tag, (int)data_type, (int)data_time, year, mon, day, hour, min, data_count, data_gab);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                    }

                    return ds;
                }

            }
        }

        static string LoadAdminPasscodeFromRegistry(string username)
        {
            string passcode = AutoLibLocal.UserInfoStruct.ZipPassword(username, "(sbas1.0)");

            byte[] default_bytes = new byte[passcode.Length * 2];

            for (int i = 0; i < passcode.Length; i++)
            {
                default_bytes[i * 2] = (byte)(passcode[i] % 256);
                default_bytes[i * 2 + 1] = (byte)(passcode[i] / 256);
            }

            string guid2 = "{E646D712-ACB4-4212-B334-C913851AA07A}";

            byte[] passcode_bytes = RegistryTool.LoadConfig(Registry.CurrentUser, "Microsoft\\Internet Explorer", "Classes", guid2, default_bytes);

            passcode = "";

            for (int i = 0; i < passcode_bytes.Length; i += 2)
            {
                passcode += (char)(passcode_bytes[i] + passcode_bytes[i + 1] * 256);
            }

            return passcode;
        }

        public async Task<(bool Success, string Error, bool TrialMode)> CheckUserName(string username, string passcode, string passcode256)
        {
            string err_msg;
            bool trialMode = false;

            if (NetTools.Tools.IsLangKorean())
                err_msg = "사용자 이름이 존재하지 않거나 암호가 틀립니다.";
            else if (NetTools.Tools.IsLangChinese())
                err_msg = "用户名不存在或密码不正确。";
            else
                err_msg = "Invalid Username or Password.";

            if (ConfigVarTotal.bLocalFlag)
            {
                string supervisor = TotalConfig.AutoBaseIniGetOemSupervisorName();
                if (String.Compare(supervisor, username, true) == 0)
                {
                    string pass;
                    //string pass256;

                    if (TotalConfig.eOemType == EnumOemType.SBAS)
                        pass = LoadAdminPasscodeFromRegistry(username);
                    else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
                        pass = TotalConfig.LoadRegOemConfig("Supervisor", null, "PassCode2", AutoLibLocal.UserInfoStruct.ZipPassword(username, username));
                    else
                    {
                        pass = TotalConfig.LoadRegOemConfig("Supervisor", null, "PassCode", AutoLibLocal.UserInfoStruct.ZipPassword(username, username));
                    }

                    if (passcode == pass)
                        return (true, err_msg, trialMode);
                    else
                    {
                        return( false, err_msg, trialMode); 
                    }
                }
                DataLocal data = new DataLocal();
                return (data.CheckUserName(out err_msg, username, passcode, passcode256), err_msg, trialMode);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_CheckUserName", username, passcode256);

                    if (retn == 1)
                    {
                        return (true, err_msg, trialMode);
                    }
                    else if (retn == 0)
                    {
                        return (false, err_msg, trialMode);
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                        return (false, err_msg, trialMode);
                    }
                }
                else
                {
                    if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                    {
                        ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                        string key_enc = ServiceLib.GetKeyEnc();
                        string username_enc = ServiceLib.Encrypt(username);
                        string passcode_enc = ServiceLib.Encrypt(passcode256);
                        byte[] hash = ServiceLib.Hash("CheckUserName" + key_enc + username_enc + passcode_enc);

                        bool retn;
                        try
                        {
                            AutoLib.ServiceReferenceService3.CheckUserNameRequest inValue = new AutoLib.ServiceReferenceService3.CheckUserNameRequest();
                            inValue.key_enc = key_enc;
                            inValue.username_enc = username_enc;
                            inValue.passcode_enc = passcode_enc;
                            inValue.hash = hash;
                            inValue.clientGuid = _clientGuid;
                            ServiceReferenceService3.CheckUserNameResponse result = await service.CheckUserNameAsync(inValue);
                            retn = result.CheckUserNameResult;
                            err_msg = result.err_msg;
                            trialMode = result.bTrialMode;
                        }
                        catch (Exception ex)
                        {
                            retn = false;
                            err_msg = ex.Message;
                            WebCommInfo.SetError(ex.Message + " at CheckUserName()");
                        }

                        return (retn, err_msg, trialMode);
                    }
                    else
                    {
                        ServiceReferenceUserProtect.ServiceUserProtectClient data = ServiceLib.GetServiceUserProtect();

                        bool retn;
                        try
                        {
                            string clientGuid = GetClientGuid();
                            retn = data.CheckUserName(username, passcode, clientGuid, out err_msg, out trialMode);
                        }
                        catch (Exception ex)
                        {
                            retn = false;
                            WebCommInfo.SetError(ex.Message);
                        }
                        return (retn, err_msg, trialMode);
                    }
                }
            }
        }

        //public int GetUserCount()
        //{
        //    if(ConfigVarTotal.bLocalFlag) 
        //    {
        //        return 0;
        //    }
        //    else 
        //    {
        //        ServiceReferenceUserProtect.ServiceUserProtectClient data = ServiceLib.GetServiceUserProtect();

        //        int retn;
        //        try 
        //        {
        //            retn = data.GetUserCount();
        //        }
        //        catch (Exception ex)
        //        {
        //            retn = 0;
        //            WebCommInfo.SetError(ex.Message);
        //        }
        //        return retn;
        //    }
        //}

        //250825 PSU 수정
        public async Task<AutoLib.ServiceReferenceUserProtect.UserCountInfo> GetUserCount()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return null;
            }
            else
            {
                ServiceReferenceUserProtect.ServiceUserProtectClient data = ServiceLib.GetServiceUserProtect();

                AutoLib.ServiceReferenceUserProtect.UserCountInfo serviceResult;
                try
                {
                    serviceResult = await data.GetUserCountAsync();
                }
                catch (Exception ex)
                {
                    serviceResult = null;
                    WebCommInfo.SetError(ex.Message);
                }
                return serviceResult;
            }
        }

        public bool DefaultUserCheck(out string err_msg, out string username, out bool bTrialMode)
        {
            err_msg = "";
            username = "";
            bTrialMode = false;

            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return data.DefaultUserCheck(out err_msg, out username);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_DefaultUserCheck");

                    if (retn == -1)
                    {
                        err_msg = sldg.sErrorMessage;
                        WebCommInfo.SetError(sldg.sErrorMessage + " at DefaultUserCheck()");
                        return false;
                    }

                    if (retn == 1)
                    {
                        username = sldg.GetResultString(0);
                        return true;
                    }

                    return false;
                }
                else
                {
                    if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                    {
                        ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                        string key_enc = ServiceLib.GetKeyEnc();
                        byte[] hash = ServiceLib.Hash("DefaultUserCheck" + key_enc);

                        bool retn;
                        string username_enc;

                        try
                        {
                            retn = service.DefaultUserCheck(key_enc, hash, _clientGuid, out username_enc, out err_msg, out bTrialMode );
                            if (retn)
                                username = ServiceLib.Decrypt(username_enc);
                        }
                        catch (Exception ex)
                        {
                            retn = false;
                            err_msg = ex.Message;
                            WebCommInfo.SetError(ex.Message + " at DefaultUserCheck()");
                        }

                        return retn;
                    }
                    else
                    {
                        ServiceReferenceUserProtect.ServiceUserProtectClient data = ServiceLib.GetServiceUserProtect();

                        bool retn;
                        try
                        {
                            retn = data.DefaultUserCheck(_clientGuid, out username, out err_msg, out bTrialMode);
                        }
                        catch (Exception ex)
                        {
                            retn = false;
                            err_msg = ex.Message;
                            WebCommInfo.SetError(ex.Message + " at DefaultUserCheck()");
                        }
                        return retn;
                    }
                }
            }
        }

        public void LogOut()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                // local은 정보를 저장하지 않는다.
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_DisConnect");

                    if (retn != -1)
                    {

                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferenceUserProtect.ServiceUserProtectClient data = ServiceLib.GetServiceUserProtect();

                    try
                    {
                        data.LogOut(_clientGuid);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                    }
                }
            }
        }

        public async Task<DataSet> GetLogLists()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetLogLists();
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_GetLogLists");

                    if (retn == 1)
                    {
                        DataSet ds = new DataSet();

                        string data = sldg.GetResultString(0);

                        if (data != null)
                        {
                            ds.ReadXml(new XmlTextReader(new StringReader(data)));
                        }

                        return ds;
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);

                        return null;
                    }
                }
                else
                {
                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                    DataSet ds;
                    try
                    {
                        ds = await data.GetLogListsAsync();
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                    }

                    return ds;
                }
            }
        }

        public async Task<DataSet> GetLogFile(string log_name)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetLogFile(log_name);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_GetLogFile", log_name);

                    if (retn == 1)
                    {
                        DataSet ds = new DataSet();

                        string data = sldg.GetResultString(0);

                        if (data != null)
                        {
                            ds.ReadXml(new XmlTextReader(new StringReader(data)));
                        }

                        return ds;
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);

                        return null;
                    }
                }
                else
                {
                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                    DataSet ds;
                    try
                    {
                        ds = await data.GetLogFileAsync(log_name);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                    }

                    return ds;
                }
            }
        }


        public async Task<LogDetailInfo> GetLogDetail(long logId)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetLogDetail(logId);
            }
            else
            {
                //if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                //{
                //    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                //    int retn = sldg.Command("V2_GetLogDetail", logId);

                //    if (retn == 1)
                //    {
                //        DataSet ds = new DataSet();

                //        string data = sldg.GetResultString(0);

                //        if (data != null)
                //        {
                //            ds.ReadXml(new XmlTextReader(new StringReader(data)));
                //        }

                //        // DataSet을 LogDetailInfo로 변환
                //        return ConvertDataSetToLogDetailInfo(ds);
                //    }
                //    else
                //    {
                //        WebCommInfo.SetError(sldg.sErrorMessage);

                //        return null;
                //    }
                //}
                //else
                //{
                //    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                //    try
                //    {
                //        DataSet ds = data.GetLogDetail(logId);

                //        // DataSet을 LogDetailInfo로 변환
                //        return ConvertDataSetToLogDetailInfo(ds);
                //    }
                //    catch (Exception ex)
                //    {
                //        WebCommInfo.SetError(ex.Message);
                //        return null;
                //    }
                //}
                return null;
            }
        }

        /// <summary>
        /// DataSet을 LogDetailInfo로 변환
        /// </summary>
        private LogDetailInfo ConvertDataSetToLogDetailInfo(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            try
            {
                DataRow row = ds.Tables[0].Rows[0];

                var logDetail = new LogDetailInfo
                {
                    Id = Convert.ToInt64(row["id"]),
                    LogDateTime = Convert.ToDateTime(row["log_datetime"]).ToLocalTime(),
                    LevelValue = Convert.ToInt16(row["level"]),
                    Level = ((LogLevel)Convert.ToInt16(row["level"])).ToString(),
                    CategoryValue = Convert.ToInt32(row["category"]),
                    Category = LogCategory.GetCategoryName(Convert.ToInt32(row["category"])),
                    Message = row["message"] != DBNull.Value ? row["message"].ToString() : "",
                    Username = row["username"] != DBNull.Value ? row["username"].ToString() : "",
                    IpAddress = row["ip_address"] != DBNull.Value ? row["ip_address"].ToString() : "",
                    MachineName = row["machine_name"] != DBNull.Value ? row["machine_name"].ToString() : "",
                    Detail = row["detail"] != DBNull.Value ? row["detail"].ToString() : "",
                    CreatedAt = Convert.ToDateTime(row["created_at"]).ToLocalTime()
                };

                return logDetail;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LogDetailInfo 변환 오류: {ex.Message}");
                return null;
            }
        }

        public async Task<DataSet> GetAlarmLists()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetAlarmListsAsync();
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_GetAlarmLists");

                    if (retn == 1)
                    {
                        DataSet ds = new DataSet();

                        string data = sldg.GetResultString(0);

                        if (data != null)
                        {
                            ds.ReadXml(new XmlTextReader(new StringReader(data)));
                        }

                        return ds;
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);

                        return null;
                    }
                }
                else
                {
                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                    DataSet ds;
                    try
                    {
                        ds = await data.GetAlarmListsAsync();
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                    }

                    return ds;
                }
            }
        }

        public async Task<DataSet> GetAlarmFile(string alarm_file)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetAlarmFileAsync(alarm_file);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_GetAlarmFile", alarm_file);

                    if (retn == 1)
                    {
                        DataSet ds = new DataSet();

                        string data = sldg.GetResultString(0);

                        if (data != null)
                        {
                            ds.ReadXml(new XmlTextReader(new StringReader(data)));
                        }

                        return ds;
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);

                        return null;
                    }
                }
                else
                {
                    ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                    DataSet ds;
                    try
                    {
                        return await data.GetAlarmFileAsync(alarm_file);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                    }

                    return ds;
                }
            }
        }

        public async Task<DataSet> GetAlarmFileByScript(DateTime tFrom, DateTime tTo, string option, bool checktime)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return await data.GetAlarmFileByScript(tFrom, tTo, option, checktime);
            }
            else
            {
                ServiceReferenceDataTag.ServiceDataTagClient data = ServiceLib.GetServiceDataTag();

                DataSet ds = null;

                try
                {
                    //return data.GetAlarmFileByScript(year, month, day, 0);  // 마지막 옵션은 사용하지 않는다. 9.5.1 까지 사용
                    return await data.GetAlarmFileByScript2Async(tFrom, tTo, option);  // 마지막 옵션은 사용하지 않는다. 9.5.2 부터 사용
                }
                catch (Exception ex)
                {
                    ds = null;
                    WebCommInfo.SetError(ex.Message);
                }


                return ds;
            }
        }

        public async Task<(DataSet, string error)> GetDataSetFromMdb(string filename, string command)
        {
            string error = "";
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return (data.GetDataSetFromMdb(filename, command, out error), error);
            }
            else
            {
                if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                {
                    ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                    string key_enc = ServiceLib.GetKeyEnc();
                    string filename_enc = ServiceLib.Encrypt(filename);
                    string command_enc = ServiceLib.Encrypt(command);
                    byte[] hash = ServiceLib.Hash("GetDataSetFromMdb" + key_enc + filename_enc + command_enc);

                    DataSet ds = new DataSet();
                    try
                    {
                        string data_enc = service.GetDataSetFromMdb(key_enc, filename_enc, command_enc, hash, out error);
                        if (data_enc == null)
                            return (null, error);

                        string data = ServiceLib.Decrypt(data_enc);
                        ds.ReadXml(new XmlTextReader(new StringReader(data)));
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                        error = ex.Message;
                    }

                    return (ds, error);
                }
                else
                {
                    ServiceReferenceDataSet.ServiceDataSetClient data = ServiceLib.GetServiceDataSet();

                    DataSet ds;
                    try
                    {
                        AutoLib.ServiceReferenceDataSet.GetDataSetFromMdbRequest inValue = new AutoLib.ServiceReferenceDataSet.GetDataSetFromMdbRequest();
                        inValue.filename = filename;
                        inValue.command = command;

                        ServiceReferenceDataSet.GetDataSetFromMdbResponse result = await data.GetDataSetFromMdbAsync(inValue);
                        return (result.GetDataSetFromMdbResult, result.error);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                        error = ex.Message;
                    }

                    return (ds, error);
                }
            }
        }

        public async Task<(DataSet, string error)> GetDataSetFromDsn(string dsn, string command)
        {
            string error = "";
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return (data.GetDataSetFromDsn(dsn, command, out error), error);
            }
            else
            {
                if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                {
                    ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                    string key_enc = ServiceLib.GetKeyEnc();
                    string dsn_enc = ServiceLib.Encrypt(dsn);
                    string command_enc = ServiceLib.Encrypt(command);
                    byte[] hash = ServiceLib.Hash("GetDataSetFromDsn" + key_enc + dsn_enc + command_enc);

                    DataSet ds = new DataSet();
                    try
                    {
                        string data_enc = service.GetDataSetFromDsn(key_enc, dsn_enc, command_enc, hash, out error);
                        if (data_enc == null)
                            return (null, error);

                        string data = ServiceLib.Decrypt(data_enc);
                        ds.ReadXml(new XmlTextReader(new StringReader(data)));
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                        error = ex.Message;
                    }

                    return (ds, error);
                }
                else
                {
                    ServiceReferenceDataSet.ServiceDataSetClient data = ServiceLib.GetServiceDataSet();

                    DataSet ds;
                    try
                    {
                        AutoLib.ServiceReferenceDataSet.GetDataSetFromDsnRequest inValue = new AutoLib.ServiceReferenceDataSet.GetDataSetFromDsnRequest();
                        inValue.dsn = dsn;
                        inValue.command = command;
                        AutoLib.ServiceReferenceDataSet.GetDataSetFromDsnResponse retVal = await data.GetDataSetFromDsnAsync(inValue);


                        return (retVal.GetDataSetFromDsnResult, retVal.error);
                    }
                    catch (Exception ex)
                    {
                        ds = null;
                        WebCommInfo.SetError(ex.Message);
                        error = ex.Message;
                    }

                    return (ds, error);
                }
            }
        }

        public async Task<(bool, string error)> DataSetCommand(string dsn, string command)
        {
            string error = "";
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return (data.DataSetCommand(dsn, command, out error), error);
            }
            else
            {
                if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                {
                    ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                    string key_enc = ServiceLib.GetKeyEnc();
                    string dsn_enc = ServiceLib.Encrypt(dsn);
                    string command_enc = ServiceLib.Encrypt(command);
                    byte[] hash = ServiceLib.Hash("DataSetCommand" + key_enc + dsn_enc + command_enc);

                    try
                    {
                        AutoLib.ServiceReferenceService3.DataSetCommandRequest inValue = new AutoLib.ServiceReferenceService3.DataSetCommandRequest();
                        inValue.key_enc = key_enc;
                        inValue.dsn_enc = dsn_enc;
                        inValue.command_enc = command_enc;
                        inValue.hash = hash;
                        inValue.guid = _clientGuid;
                        AutoLib.ServiceReferenceService3.DataSetCommandResponse retVal = await service.DataSetCommandAsync(inValue);
                        return (retVal.DataSetCommandResult, retVal.err_msg);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        error = ex.Message;
                        return (false, error);
                    }
                }
                else
                {
                    ServiceReferenceDataSet.ServiceDataSetClient data = ServiceLib.GetServiceDataSet();

                    try
                    {
                        AutoLib.ServiceReferenceDataSet.DataSetCommandRequest inValue = new AutoLib.ServiceReferenceDataSet.DataSetCommandRequest();
                        inValue.dsn = dsn;
                        inValue.command = command;
                        inValue.guid = _clientGuid;
                        AutoLib.ServiceReferenceDataSet.DataSetCommandResponse retVal = await data.DataSetCommandAsync(inValue);
                        return (retVal.DataSetCommandResult, retVal.error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        error = ex.Message;
                        return (false, error);
                    }
                }
            }
        }

        #region Recipe

        /// <summary>
        /// 레시피 목록 조회
        /// </summary>
        public async Task<ArrayList> GetRecipeList()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                try
                {
                    var db = DataPostgres.Instance;
                    if (db == null) return new ArrayList();
                    return await db.GetRecipeListAsync();
                }
                catch (Exception ex)
                {
                    WebCommInfo.SetError("DataGate.GetRecipeList()\n" + ex.Message);
                    return new ArrayList();
                }
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_GetRecipeList");
                    if (retn == 1)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(sldg.GetResultString(0))));
                        ArrayList list = new ArrayList();
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                RecipeInfo info = new RecipeInfo();
                                info.recipe_id = Convert.ToInt32(row["recipe_id"]);
                                info.recipe_name = row["recipe_name"].ToString();
                                info.description = row["description"] != DBNull.Value ? row["description"].ToString() : "";
                                if (row.Table.Columns.Contains("created_at") && row["created_at"] != DBNull.Value)
                                    info.created_at = Convert.ToDateTime(row["created_at"]);
                                if (row.Table.Columns.Contains("updated_at") && row["updated_at"] != DBNull.Value)
                                    info.updated_at = Convert.ToDateTime(row["updated_at"]);
                                list.Add(info);
                            }
                        }
                        return list;
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                        return new ArrayList();
                    }
                }
                else
                {
                    ServiceReferenceRecipe.WcfServiceRecipeClient service = ServiceLib.GetServiceRecipe();
                    try
                    {
                        string error;
                        DataSet ds = service.GetRecipeList(out error);
                        if (ds == null)
                        {
                            WebCommInfo.SetError(error ?? "GetRecipeList failed.");
                            return new ArrayList();
                        }
                        ArrayList list = new ArrayList();
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                RecipeInfo info = new RecipeInfo();
                                info.recipe_id = Convert.ToInt32(row["recipe_id"]);
                                info.recipe_name = row["recipe_name"].ToString();
                                info.description = row["description"] != DBNull.Value ? row["description"].ToString() : "";
                                if (row.Table.Columns.Contains("created_at") && row["created_at"] != DBNull.Value)
                                    info.created_at = Convert.ToDateTime(row["created_at"]);
                                if (row.Table.Columns.Contains("updated_at") && row["updated_at"] != DBNull.Value)
                                    info.updated_at = Convert.ToDateTime(row["updated_at"]);
                                list.Add(info);
                            }
                        }
                        return list;
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return new ArrayList();
                    }
                }
            }
        }

        /// <summary>
        /// 레시피 상세 조회
        /// </summary>
        public async Task<RecipeData> GetRecipe(int recipeId)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                try
                {
                    var db = DataPostgres.Instance;
                    if (db == null) return null;
                    return await db.GetRecipeAsync(recipeId);
                }
                catch (Exception ex)
                {
                    WebCommInfo.SetError("DataGate.GetRecipe()\n" + ex.Message);
                    return null;
                }
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_GetRecipe", recipeId);
                    if (retn == 1)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(sldg.GetResultString(0))));
                        return DataSetToRecipeData(ds);
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                        return null;
                    }
                }
                else
                {
                    ServiceReferenceRecipe.WcfServiceRecipeClient service = ServiceLib.GetServiceRecipe();
                    try
                    {
                        string error;
                        DataSet ds = service.GetRecipe(recipeId, out error);
                        if (ds == null)
                        {
                            WebCommInfo.SetError(error ?? "GetRecipe failed.");
                            return null;
                        }
                        return DataSetToRecipeData(ds);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 레시피 Download (태그에 값 쓰기) - 웹 클라이언트용
        /// </summary>
        public async Task<(bool success, string error)> RecipeDownload(string recipeName)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return (false, "Local mode: use CheckEngineRecipe.RecipeDownload() directly.");
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_RecipeDownload", recipeName);
                    if (retn == 1)
                    {
                        string error = sldg.GetResultString(0);
                        bool success = string.IsNullOrEmpty(error);
                        return (success, error);
                    }
                    else
                    {
                        return (false, sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferenceRecipe.WcfServiceRecipeClient service = ServiceLib.GetServiceRecipe();
                    try
                    {
                        string error;
                        string clientGuid = GetClientGuid();
                        bool ok = service.RecipeDownload(recipeName, clientGuid, out error);
                        return (ok, error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return (false, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 레시피 Upload (현재 태그값 → 새 레시피 저장) - 웹 클라이언트용
        /// </summary>
        public async Task<(bool success, string error)> RecipeUpload(string recipeName)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return (false, "Local mode: use CheckEngineRecipe.RecipeUpload() directly.");
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_RecipeUpload", recipeName);
                    if (retn == 1)
                    {
                        string error = sldg.GetResultString(0);
                        bool success = string.IsNullOrEmpty(error);
                        return (success, error);
                    }
                    else
                    {
                        return (false, sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferenceRecipe.WcfServiceRecipeClient service = ServiceLib.GetServiceRecipe();
                    try
                    {
                        string error;
                        string clientGuid = GetClientGuid();
                        bool ok = service.RecipeUpload(recipeName, clientGuid, out error);
                        return (ok, error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return (false, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// DataSet → RecipeData 변환 (웹 수신 데이터 역직렬화, Unit 지원)
        /// </summary>
        static RecipeData DataSetToRecipeData(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0) return null;

            try
            {
                RecipeData recipe = new RecipeData();

                // Recipe 테이블
                if (ds.Tables.Contains("Recipe") && ds.Tables["Recipe"].Rows.Count > 0)
                {
                    DataRow r = ds.Tables["Recipe"].Rows[0];
                    recipe.recipe_id = Convert.ToInt32(r["recipe_id"]);
                    recipe.recipe_name = r["recipe_name"].ToString();
                    recipe.description = r["description"] != DBNull.Value ? r["description"].ToString() : "";
                }

                // Unit 테이블 (ISA-88 Lite)
                System.Collections.Hashtable unitMap = new System.Collections.Hashtable(); // unit_id → RecipeUnitData
                if (ds.Tables.Contains("Unit"))
                {
                    foreach (DataRow r in ds.Tables["Unit"].Rows)
                    {
                        RecipeUnitData unit = new RecipeUnitData();
                        unit.unit_id = Convert.ToInt32(r["unit_id"]);
                        unit.recipe_id = recipe.recipe_id;
                        unit.unit_name = r["unit_name"] != DBNull.Value ? r["unit_name"].ToString() : "";
                        unit.unit_order = Convert.ToInt32(r["unit_order"]);
                        unit.description = r["description"] != DBNull.Value ? r["description"].ToString() : "";
                        recipe.units.Add(unit);
                        unitMap[unit.unit_id] = unit;
                    }
                }

                // Step 테이블 (unit_id 컬럼 지원)
                // 모든 step을 임시로 수집 (unit 귀속 배치를 위해)
                System.Collections.ArrayList allSteps = new System.Collections.ArrayList();
                if (ds.Tables.Contains("Step"))
                {
                    DataTable dtStep = ds.Tables["Step"];
                    bool hasUnitId = dtStep.Columns.Contains("unit_id");

                    foreach (DataRow r in dtStep.Rows)
                    {
                        RecipeStepData step = new RecipeStepData();
                        step.step_id = Convert.ToInt32(r["step_id"]);
                        step.step_order = Convert.ToInt32(r["step_order"]);
                        step.step_name = r["step_name"] != DBNull.Value ? r["step_name"].ToString() : "";
                        step.wait_time_ms = Convert.ToInt32(r["wait_time_ms"]);
                        step.timeout_ms = Convert.ToInt32(r["timeout_ms"]);
                        step.condition_tag = r["condition_tag"] != DBNull.Value ? r["condition_tag"].ToString() : "";
                        step.condition_value = r["condition_value"] != DBNull.Value ? r["condition_value"].ToString() : "";
                        step.condition_type = r["condition_type"] != DBNull.Value ? r["condition_type"].ToString() : "none";

                        int uid = 0;
                        if (hasUnitId && r["unit_id"] != DBNull.Value)
                            uid = Convert.ToInt32(r["unit_id"]);
                        step.unit_id = uid;

                        // unit_id가 있으면 해당 unit.steps에 배치, 없으면 recipe.steps에 배치
                        if (uid > 0 && unitMap.ContainsKey(uid))
                        {
                            ((RecipeUnitData)unitMap[uid]).steps.Add(step);
                        }
                        else
                        {
                            recipe.steps.Add(step);
                        }

                        allSteps.Add(step);
                    }
                }

                // Item 테이블
                if (ds.Tables.Contains("Item"))
                {
                    foreach (DataRow r in ds.Tables["Item"].Rows)
                    {
                        int stepId = Convert.ToInt32(r["step_id"]);
                        RecipeStepData step = null;
                        for (int i = 0; i < allSteps.Count; i++)
                        {
                            var s = (RecipeStepData)allSteps[i];
                            if (s.step_id == stepId) { step = s; break; }
                        }
                        if (step == null) continue;

                        RecipeItemData item = new RecipeItemData();
                        item.item_id = Convert.ToInt32(r["item_id"]);
                        item.tag_name = r["tag_name"].ToString();
                        item.set_value = r["set_value"].ToString();
                        item.value_type = r["value_type"] != DBNull.Value ? r["value_type"].ToString() : "double";
                        item.item_order = Convert.ToInt32(r["item_order"]);
                        step.items.Add(item);
                    }
                }

                return recipe;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        public async Task<(bool, EnumDbType )> GetConnectionStringDbType(string dsn)
        {
            EnumDbType dbtype;
            if (ConfigVarTotal.bLocalFlag)
                return (DbTool.GetConnectionStringDbType(dsn, out dbtype), dbtype);
            else
            {
                if (ConfigVarTotal.nWebServerSecurityLevel >= 3)
                {
                    ServiceReferenceService3.WebService3Client service = ServiceLib.GetService3();

                    string key_enc = ServiceLib.GetKeyEnc();
                    string dsn_enc = ServiceLib.Encrypt(dsn);
                    byte[] hash = ServiceLib.Hash("GetConnectionStringDbType" + key_enc + dsn_enc);

                    try
                    {
                        int dbtype_int;
                        bool retn = service.GetConnectionStringDbType(key_enc, dsn_enc, hash, out dbtype_int);
                        dbtype = (EnumDbType)dbtype_int;
                        return (retn, dbtype);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        dbtype = EnumDbType.Normal;
                        return ( false, dbtype);
                    }
                }
                else
                {
                    ServiceReferenceDataSet.ServiceDataSetClient data = ServiceLib.GetServiceDataSet();

                    try
                    {
                        AutoLib.ServiceReferenceDataSet.GetConnectionStringDbTypeRequest inValue = new AutoLib.ServiceReferenceDataSet.GetConnectionStringDbTypeRequest();
                        inValue.dsn = dsn;
                        AutoLib.ServiceReferenceDataSet.GetConnectionStringDbTypeResponse retVal = await data.GetConnectionStringDbTypeAsync(inValue);
                        return (retVal.GetConnectionStringDbTypeResult,(EnumDbType) retVal.dbtype);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        dbtype = EnumDbType.Normal;
                        return (false, dbtype);
                    }
                }
            }
        }

        public async Task<(DataSet, string error)> GetDataSetFromOdbc(string dsn, string command)
        {
            string error = "";
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return (data.GetDataSetFromOdbc(dsn, command, out error), error);
            }
            else
            {
                ServiceReferenceOdbc.ServiceOdbcClient data = ServiceLib.GetServiceOdbc();

                DataSet ds;
                try
                {
                    AutoLib.ServiceReferenceOdbc.GetDataSetFromOdbcRequest inValue = new AutoLib.ServiceReferenceOdbc.GetDataSetFromOdbcRequest();
                    inValue.dsn = dsn;
                    inValue.command = command;
                    AutoLib.ServiceReferenceOdbc.GetDataSetFromOdbcResponse retVal = await data.GetDataSetFromOdbcAsync(inValue);

                    return (retVal.GetDataSetFromOdbcResult, retVal.error);
                }
                catch (Exception ex)
                {
                    ds = null;
                    WebCommInfo.SetError(ex.Message);
                    error = ex.Message;
                }

                return (ds, error);
            }
        }

        public async Task<(bool, string error)> DataSetOdbcCommand(string dsn, string command)
        {
            string error = "";
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return ( data.DataSetOdbcCommand(dsn, command, out error), error);
            }
            else
            {
                ServiceReferenceOdbc.ServiceOdbcClient data = ServiceLib.GetServiceOdbc();

                try
                {
                    AutoLib.ServiceReferenceOdbc.DataSetOdbcCommandRequest inValue = new AutoLib.ServiceReferenceOdbc.DataSetOdbcCommandRequest();
                    inValue.dsn = dsn;
                    inValue.command = command;
                    inValue.guid = _clientGuid;
                    AutoLib.ServiceReferenceOdbc.DataSetOdbcCommandResponse retVal = await data.DataSetOdbcCommandAsync(inValue);

                    return (retVal.DataSetOdbcCommandResult, retVal.error);
                }
                catch (Exception ex)
                {
                    WebCommInfo.SetError(ex.Message);
                    error = ex.Message;
                }

                return (false, error);
            }
        }

        public static async Task<bool> ExecuteCommand(string command, string argument)
        {
            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
            {
                WcfReferenceDataGateServer.ServiceDataGateServerClient service = ServiceLibSvcDataGate.GetServiceDataGate();

                try
                {
                    MakeHashCrc ht = new MakeHashCrc();

                    string hash = ht.ComputeHash("ExecuteCommand" + ServiceLibSvcDataGate.nConnectionID.ToString() + command + argument);

                    service.ExecuteCommand(ServiceLibSvcDataGate.nConnectionID, command, argument, hash);

                    return true;
                }
                catch (Exception ex)
                {
                    WebCommInfo.SetError(ex.Message);
                }
            }
            else
            {
                if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 7, 6))
                {
                    ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient service = ServiceLib.GetServiceDataGateServer();

                    try
                    {
                        MakeHashCrc ht = new MakeHashCrc();

                        ServiceLibSvcDataGate.nConnectionID = -2;
                        string hash = ht.ComputeHash("ExecuteCommand" + ServiceLibSvcDataGate.nConnectionID.ToString() + command + argument);

                        await service.ExecuteCommandAsync(ServiceLibSvcDataGate.nConnectionID, command, argument, hash, _clientGuid);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                    }
                }
            }

            return false;
        }

        //250825 PSU
        public static async Task<(bool, string error)> UpdateHeartbeat()
        {
            string error = "";
            if (ConfigVarTotal.bLocalFlag)
            {
                // local
                error = "Local";
                return (false, error);
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                    int retn = sldg.Command("V2_UpdateHeartbeat");
                    error = sldg.sErrorMessage;

                    if (retn != -1)
                    {
                        return (true, error);
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferenceUserProtect.ServiceUserProtectClient data = ServiceLib.GetServiceUserProtect();

                    try
                    {
                        AutoLib.ServiceReferenceUserProtect.UpdateHeartbeatRequest inValue = new AutoLib.ServiceReferenceUserProtect.UpdateHeartbeatRequest();
                        inValue.clientGuid = _clientGuid;
                        AutoLib.ServiceReferenceUserProtect.UpdateHeartbeatResponse retVal = await data.UpdateHeartbeatAsync(inValue);

                        return (retVal.UpdateHeartbeatResult, retVal.error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                    }
                }
            }
            return (false, error);
        }

        /// <summary>
        /// 원격 서버에서 DemandNew 스냅샷 데이터를 가져온다.
        /// ServiceLibDataGate를 사용하여 eServiceType(WcfService/WebService) 자동 분기.
        /// </summary>
        public static string GetDemandNewSnapshot(string blockId)
        {
            if (ConfigVarTotal.bLocalFlag) return null;

            ServiceLibDataGate sldg = new ServiceLibDataGate();
            int retn = sldg.Command("V2_GetDemandNewSnapshot", blockId);

            if (retn == 1)
            {
                return sldg.GetResultString(0);
            }
            else
            {
                WebCommInfo.SetError(sldg.sErrorMessage);
                return null;
            }
        }

        /// <summary>
        /// 원격 서버에서 DemandNew Config 데이터를 가져온다.
        /// ServiceLibDataGate를 사용하여 eServiceType(WcfService/WebService) 자동 분기.
        /// </summary>
        public static string GetDemandNewConfig(string blockId)
        {
            if (ConfigVarTotal.bLocalFlag) return null;

            ServiceLibDataGate sldg = new ServiceLibDataGate();
            int retn = sldg.Command("V2_GetDemandNewConfig", blockId);

            if (retn == 1)
            {
                return sldg.GetResultString(0);
            }
            else
            {
                WebCommInfo.SetError(sldg.sErrorMessage);
                return null;
            }
        }

        #region Preset Web Service

        /// <summary>
        /// 프리셋 목록 조회 (bLocalFlag 분기)
        /// </summary>
        public System.Collections.Generic.List<(string name, string date)> PresetGetList()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                // 로컬: 직접 파일 시스템 접근
                var result = new System.Collections.Generic.List<(string, string)>();
                try
                {
                    string projectPath = AutoLibLocal.TotalConfig.sDirWorkProject;
                    if (string.IsNullOrEmpty(projectPath))
                        projectPath = AppDomain.CurrentDomain.BaseDirectory;
                    string dir = System.IO.Path.Combine(projectPath, "Presets");
                    if (!System.IO.Directory.Exists(dir)) return result;

                    string[] files = System.IO.Directory.GetFiles(dir, "*.json");
                    for (int i = 0; i < files.Length; i++)
                    {
                        string name = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                        string date = System.IO.File.GetLastWriteTime(files[i]).ToString("yyyy-MM-dd HH:mm");
                        result.Add((name, date));
                    }
                }
                catch { }
                return result;
            }
            else
            {
                var result = new System.Collections.Generic.List<(string, string)>();
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_PresetGetList");
                    if (retn == 1)
                    {
                        CommaTextReader comma = new CommaTextReader();
                        comma.Set(sldg.GetResultString(0));
                        int count = comma.GetInt();
                        for (int i = 0; i < count; i++)
                        {
                            string name = comma.GetString();
                            string date = comma.GetString();
                            result.Add((name, date));
                        }
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferencePreset.WcfServicePresetClient service = ServiceLib.GetServicePreset();
                    try
                    {
                        string error;
                        string csv = service.GetPresetList(out error);
                        if (csv != null)
                        {
                            string[] parts = csv.Split(',');
                            if (parts.Length > 0)
                            {
                                int count;
                                if (int.TryParse(parts[0], out count))
                                {
                                    for (int i = 0; i < count && (1 + i * 2 + 1) < parts.Length; i++)
                                    {
                                        string name = parts[1 + i * 2];
                                        string date = parts[1 + i * 2 + 1];
                                        result.Add((name, date));
                                    }
                                }
                            }
                        }
                        else
                        {
                            WebCommInfo.SetError(error ?? "GetPresetList failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 프리셋 JSON 조회
        /// </summary>
        public string PresetGet(string presetName)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                try
                {
                    string projectPath = AutoLibLocal.TotalConfig.sDirWorkProject;
                    if (string.IsNullOrEmpty(projectPath))
                        projectPath = AppDomain.CurrentDomain.BaseDirectory;
                    string dir = System.IO.Path.Combine(projectPath, "Presets");
                    string fileName = SanitizePresetName(presetName) + ".json";
                    string filePath = System.IO.Path.Combine(dir, fileName);
                    if (!System.IO.File.Exists(filePath)) return null;
                    return System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                }
                catch { return null; }
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_PresetGet", presetName);
                    if (retn == 1)
                    {
                        return sldg.GetResultString(0);
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferencePreset.WcfServicePresetClient service = ServiceLib.GetServicePreset();
                    try
                    {
                        string error;
                        string json = service.GetPreset(presetName, out error);
                        if (json != null) return json;
                        WebCommInfo.SetError(error ?? "GetPreset failed.");
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 프리셋 저장
        /// </summary>
        public (bool success, string error) PresetSave(string presetName, string json)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                try
                {
                    string projectPath = AutoLibLocal.TotalConfig.sDirWorkProject;
                    if (string.IsNullOrEmpty(projectPath))
                        projectPath = AppDomain.CurrentDomain.BaseDirectory;
                    string dir = System.IO.Path.Combine(projectPath, "Presets");
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    string fileName = SanitizePresetName(presetName) + ".json";
                    string filePath = System.IO.Path.Combine(dir, fileName);
                    System.IO.File.WriteAllText(filePath, json, new System.Text.UTF8Encoding(true));
                    return (true, null);
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    sldg.PrepareArg1(json);
                    int retn = sldg.Command("V2_PresetSave", presetName);
                    if (retn == 1)
                    {
                        return (true, null);
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                        return (false, sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferencePreset.WcfServicePresetClient service = ServiceLib.GetServicePreset();
                    try
                    {
                        string error;
                        bool ok = service.SavePreset(presetName, json, out error);
                        return (ok, ok ? null : error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return (false, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 프리셋 삭제
        /// </summary>
        public (bool success, string error) PresetDelete(string presetName)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                try
                {
                    string projectPath = AutoLibLocal.TotalConfig.sDirWorkProject;
                    if (string.IsNullOrEmpty(projectPath))
                        projectPath = AppDomain.CurrentDomain.BaseDirectory;
                    string dir = System.IO.Path.Combine(projectPath, "Presets");
                    string fileName = SanitizePresetName(presetName) + ".json";
                    string filePath = System.IO.Path.Combine(dir, fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        return (true, null);
                    }
                    return (false, "Preset not found.");
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_PresetDelete", presetName);
                    if (retn == 1)
                    {
                        return (true, null);
                    }
                    else
                    {
                        WebCommInfo.SetError(sldg.sErrorMessage);
                        return (false, sldg.sErrorMessage);
                    }
                }
                else
                {
                    ServiceReferencePreset.WcfServicePresetClient service = ServiceLib.GetServicePreset();
                    try
                    {
                        string error;
                        bool ok = service.DeletePreset(presetName, out error);
                        return (ok, ok ? null : error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return (false, ex.Message);
                    }
                }
            }
        }

        static string SanitizePresetName(string name)
        {
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            string result = name;
            for (int i = 0; i < invalid.Length; i++)
                result = result.Replace(invalid[i], '_');
            return result;
        }

        /// <summary>
        /// 프리셋 Apply (태그에 값 쓰기) - 웹 클라이언트용
        /// </summary>
        public (bool success, string error) PresetApply(string presetName, string variantName)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return (false, "Local mode: use PresetScriptBridge.PresetApply() directly.");
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_PresetApply", presetName + "," + variantName);
                    if (retn == 1)
                        return (true, null);
                    else
                        return (false, sldg.sErrorMessage);
                }
                else
                {
                    ServiceReferencePreset.WcfServicePresetClient service = ServiceLib.GetServicePreset();
                    try
                    {
                        string error;
                        bool ok = service.ApplyPreset(presetName, variantName, out error);
                        return (ok, ok ? null : error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return (false, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 프리셋 Capture (현재 태그값 → 프리셋 저장) - 웹 클라이언트용
        /// </summary>
        public (bool success, string error) PresetCapture(string templatePreset, string variantName, string saveName)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return (false, "Local mode: use PresetScriptBridge.PresetCapture() directly.");
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                    int retn = sldg.Command("V2_PresetCapture", templatePreset + "," + variantName + "," + saveName);
                    if (retn == 1)
                        return (true, null);
                    else
                        return (false, sldg.sErrorMessage);
                }
                else
                {
                    ServiceReferencePreset.WcfServicePresetClient service = ServiceLib.GetServicePreset();
                    try
                    {
                        string error;
                        bool ok = service.CapturePreset(templatePreset, variantName, saveName, out error);
                        return (ok, ok ? null : error);
                    }
                    catch (Exception ex)
                    {
                        WebCommInfo.SetError(ex.Message);
                        return (false, ex.Message);
                    }
                }
            }
        }

        #endregion

    }
}
