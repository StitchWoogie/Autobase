using System;
using System.Collections.Generic;
using System.Text;
using AutoLib;
using AutoLibLocal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Python AI Engine WCF 클라이언트.
    /// DataGate 패턴: bLocalFlag에 따라 로컬/원격 분기.
    /// LocalMain의 V2_PythonAi* 커맨드를 CommonMethod를 통해 호출.
    /// </summary>
    public class ClassPythonAi
    {
        /// <summary>Python AI 엔진 연결 상태 확인</summary>
        public static bool IsConnected()
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return CommandLocal("V2_PythonAiIsConnected") == "1";
            }
            else
            {
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                int retn = sldg.Command("V2_PythonAiIsConnected");
                if (retn == 1)
                    return sldg.GetResultString(0) == "1";
                return false;
            }
        }

        /// <summary>
        /// 범용 Python AI 서비스 호출.
        /// service: "predict.power", "analysis.trend", "system.ping" 등
        /// payloadJson: JSON payload (nullable)
        /// timeoutMs: 타임아웃 (0=기본값)
        /// </summary>
        public static string Call(string service, string payloadJson, int timeoutMs)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                return CommandLocalWithPayload("V2_PythonAiCall", payloadJson,
                    service, timeoutMs);
            }
            else
            {
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                if (!string.IsNullOrEmpty(payloadJson))
                    sldg.PrepareArg1(payloadJson);
                int retn = sldg.Command("V2_PythonAiCall", service, timeoutMs);
                if (retn == 1)
                    return sldg.GetResultString(0);

                WebCommInfo.SetError(sldg.sErrorMessage);
                return null;
            }
        }

        /// <summary>
        /// Python 스크립트 실행 (태그 프리로드 포함).
        /// code: Python 스크립트 코드
        /// payloadJson: 스크립트에 전달할 payload JSON (nullable)
        /// </summary>
        public static string ExecuteScript(string code, string payloadJson)
        {
            // code + payload를 하나의 JSON으로 묶어 arg1로 전송
            JObject scriptData = new JObject();
            scriptData["code"] = code;
            if (!string.IsNullOrEmpty(payloadJson))
            {
                try { scriptData["payload"] = JToken.Parse(payloadJson); }
                catch { scriptData["payload"] = payloadJson; }
            }
            string scriptDataJson = scriptData.ToString(Formatting.None);

            if (ConfigVarTotal.bLocalFlag)
            {
                return CommandLocalWithPayload("V2_PythonAiScript", scriptDataJson);
            }
            else
            {
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
                sldg.PrepareArg1(scriptDataJson);
                int retn = sldg.Command("V2_PythonAiScript");
                if (retn == 1)
                    return sldg.GetResultString(0);

                WebCommInfo.SetError(sldg.sErrorMessage);
                return null;
            }
        }

        // ==================== Local (bLocalFlag) ====================
        // LocalMain 프로세스 내에서 직접 CommonMethod 호출 (WCF 경유 없이)

        static string CommandLocal(string command, params object[] param)
        {
            try
            {
                var server = new ClassDataGateServer();
                var args = BuildArgs(command, null, param);

                List<byte[]> result;
                int retn = server.CommonMethod(args, out result);
                if (retn == 1 && result != null && result.Count > 0)
                    return DecryptString(result[0]);
                return null;
            }
            catch
            {
                return null;
            }
        }

        static string CommandLocalWithPayload(string command, string payloadData, params object[] param)
        {
            try
            {
                var server = new ClassDataGateServer();
                var args = BuildArgs(command, payloadData, param);

                List<byte[]> result;
                int retn = server.CommonMethod(args, out result);
                if (retn == 1 && result != null && result.Count > 0)
                    return DecryptString(result[0]);
                return null;
            }
            catch
            {
                return null;
            }
        }

        // ==================== Encryption Helpers ====================

        static byte[] KEY = { 0xee, 0xc3, 0x1d, 0x2d, 0x1c, 0xcb, 0x18, 0x53,
                              0x13, 0x29, 0x19, 0x2c, 0x11, 0x23, 0x36, 0x4b };
        static byte[] IV  = { 0xf5, 0x69, 0x73, 0x83, 0x1c, 0xa3, 0x1b, 0x2a,
                              0xc2, 0x2e, 0x42, 0x20, 0x1f, 0x58, 0x1a, 0x27 };

        static byte[] EncryptString(string data)
        {
            byte[] baData = Encoding.UTF8.GetBytes(data);
            return NetTools.Cryptography.CryptoAES.Encrypt(
                baData, KEY, IV,
                System.Security.Cryptography.CipherMode.CBC,
                System.Security.Cryptography.PaddingMode.PKCS7);
        }

        static string DecryptString(byte[] data)
        {
            byte[] baData = NetTools.Cryptography.CryptoAES.Decrypt(
                data, KEY, IV,
                System.Security.Cryptography.CipherMode.CBC,
                System.Security.Cryptography.PaddingMode.PKCS7);
            return Encoding.UTF8.GetString(baData);
        }

        static byte[] MakeHash(byte[] org)
        {
            using (var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return sha.ComputeHash(org);
            }
        }

        static void AddCrc(List<byte[]> args)
        {
            int totalLen = 4; // salt length
            for (int i = 0; i < args.Count; i++)
                totalLen += args[i].Length;

            byte[] total = new byte[totalLen];
            int pos = 0;
            for (int i = 0; i < args.Count; i++)
            {
                Array.Copy(args[i], 0, total, pos, args[i].Length);
                pos += args[i].Length;
            }
            total[pos++] = (byte)'~';
            total[pos++] = (byte)'$';
            total[pos++] = (byte)'7';
            total[pos++] = (byte)'l';

            args.Add(MakeHash(total));
        }

        /// <summary>CommonMethod 호출용 args 구성</summary>
        static List<byte[]> BuildArgs(string command, string payloadData, object[] param)
        {
            var args = new List<byte[]>();

            // arg0: id,command,param1,param2,...
            var ctm = new NetTools.CommaTextMaker();
            ctm.Write("{0},{1}", ServiceLibSvcDataGate.nConnectionID, command);
            for (int i = 0; i < param.Length; i++)
                ctm.Write(",{0}", param[i]);
            args.Add(EncryptString(ctm.GetResult()));

            // arg1: payload (optional)
            if (!string.IsNullOrEmpty(payloadData))
                args.Add(EncryptString(payloadData));

            // CRC hash
            AddCrc(args);

            return args;
        }
    }
}
