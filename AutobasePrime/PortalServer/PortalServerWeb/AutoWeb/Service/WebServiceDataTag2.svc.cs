using AutoLibLocal;
using NetTools;
using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "ServiceDataTag3"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 ServiceDataTag3.svc나 ServiceDataTag3.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WebServiceDataTag2 : IWebServiceDataTag2
    {
        private HttpContext Context => HttpContext.Current;
        public int CheckServiceAlive(int val)
        {
            return (~val);
        }

        // 기존 TCP 통신 + XDocument 변환
        private static XDocument GetTagValueListFromTcpXDocument(HttpContext context, List<string> array)
        {
            string buf = "";
            for (int i = 0; i < array.Count; i++)
                buf += array[i] + ",";

            string recv_data;

            // ASMX는 this(WebService) 전달 → WCF는 HttpContext로 대체
            if (!ServiceDataTagStatic.SendAndGetData(context, EnumMultiBlockCommand.TagValueList, buf, out recv_data))
                return null;

            var doc = new XDocument();
            var root = new XElement("Root");
            doc.Add(root);

            CommaBlockString comma = new CommaBlockString();
            comma.Set(recv_data);

            string tag = "";
            string val = "";

            for (int i = 0; i < array.Count; i++)
            {
                tag = array[i];
                comma.GetString(ref val);

                var container = new XElement("Values");
                container.Add(new XElement("Tag", tag));
                container.Add(new XElement("Curr", val));
                root.Add(container);
            }

            return doc;
        }

        public string GetTagValueLists(List<string> array)
        {
            try
            {
                ServiceLib.SetCommonVars();

                var doc = GetTagValueListFromTcpXDocument(Context, array);
                return doc?.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetTagValueLists exception: {ex.Message}");
                return "";
            }
            }

        public bool WriteCurrAI(string tag, double val, string guid)
        {
            try
            {
                string err;

                if (!GuidHeartbeatManager.IsClientConnected(guid)) return false;

                if (!ConfigWeb.CheckBelowSecurityLevel3(out err))
                    return false;

                string work_dir = ProjectLib.GetWorkDir(Context.Request);
                TotalConfig.sDirWorkProject = work_dir;

                string clientip = Context.Request.UserHostAddress;
                string buf = $"{tag},{val},WebService,{clientip},WebServer";

                string recv;
                ServiceDataTagStatic.SendAndGetData(Context, EnumMultiBlockCommand.SetTagValue, buf, out recv);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WriteCurrAI exception: {ex.Message}");
                return false;
            }
        }

        public bool WriteCurrST(string tag, string val, string guid)
        {
            try
            {
                string err;

                if (!GuidHeartbeatManager.IsClientConnected(guid)) return false;

                if (!ConfigWeb.CheckBelowSecurityLevel3(out err))
                    return false;

                string work_dir = ProjectLib.GetWorkDir(Context.Request);
                TotalConfig.sDirWorkProject = work_dir;

                string clientip = Context.Request.UserHostAddress;
                string buf = $"{tag},{val},WebService,{clientip},WebServer";

                string recv;
                ServiceDataTagStatic.SendAndGetData(Context, EnumMultiBlockCommand.SetTagValue, buf, out recv);

                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"WriteCurrST exception: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetDataAi(string tag, int value_type, int data_time,
                               int year, int mon, int day, int hour, int min,
                               int data_count, int data_gab)
        {
            try
            {
                WcfServiceDataTag service = new WcfServiceDataTag();
                var ds = await service.GetDataAi(tag, value_type, data_time,
                                           year, mon, day, hour, min,
                                           data_count, data_gab);

                return ds.GetXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetDataAi exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetDataDi(string tag, int value_type, int data_time,
                               int year, int mon, int day, int hour, int min,
                               int data_count, int data_gab)
        {
            try
            {
                WcfServiceDataTag service = new WcfServiceDataTag();
                var ds = await service.GetDataDi(tag, value_type, data_time,
                                           year, mon, day, hour, min,
                                           data_count, data_gab);

                return ds.GetXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetDataDi exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetLogLists()
        {
            try
            {
                var service = new WcfServiceDataTag();
                DataSet dataset = await service.GetLogLists();
                return dataset.GetXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLogLists exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetLogFile(string name)
        {
            try
            {
                var service = new WcfServiceDataTag();
                DataSet dataset = await service.GetLogFile(name);
                return dataset.GetXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLogFile exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetAlarmLists()
        {
            try
            {
                var service = new WcfServiceDataTag();
                DataSet dataset = await service.GetAlarmLists();
                return dataset.GetXml();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAlarmLists exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetAlarmFile(string name)
        {
            try
            {
                var service = new WcfServiceDataTag();
                DataSet dataset = await service.GetAlarmFile(name);
                return dataset.GetXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAlarmFile exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetAlarmFileByScript2(DateTime tFrom, DateTime tTo, string option)
        {
            try
            {
                var service = new WcfServiceDataTag();
                DataSet dataset = await service.GetAlarmFileByScript2(tFrom, tTo, option);
                return dataset.GetXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAlarmFileByScript2 exception: {ex.Message}");
                return "";
            }
        }

        public string GetServerVersion()
        {
            return ServerVersion().ToString();
        }

        public static Version ServerVersion()
        {
            return new Version(10, 5, 0, 0);
        }

        public string GetServerInformations()
        {
            StringBuilder sb = new StringBuilder();

            TimeZoneInfo tzi = TimeZoneInfo.Local;
            sb.Append($"TimeZoneSeconds={tzi.BaseUtcOffset.TotalSeconds},");
            sb.Append($"TimeZoneID={tzi.Id},");
            sb.Append($"SecurityLevel={ConfigWeb.SecurityLevel()},");

            return sb.ToString();
        }

        public bool WriteCurr(string username, string computername, string tag, string val, byte[] hash, string guid)
        {
            try
            {
                string err;
                if (!GuidHeartbeatManager.IsClientConnected(guid))
                    return false;

                if (!ConfigWeb.CheckBelowSecurityLevel3(out err))
                    return false;

                byte[] hash_s = WcfWebServiceAndroid.StringToBytes("WriteCurr" + username + computername + tag + val);
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] result = sha.ComputeHash(hash_s);

                if (!result.SequenceEqual(hash))
                    return false;

                string work_dir = ProjectLib.GetWorkDir(Context.Request);
                TotalConfig.sDirWorkProject = work_dir;
                string clientip = Context.Request.UserHostAddress;

                CommaTextMaker ctm = new CommaTextMaker();
                ctm.Write("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);

                string recv_data;
                ServiceDataTagStatic.SendAndGetData(Context, EnumMultiBlockCommand.SetTagValue, ctm.GetResult(), out recv_data);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WriteCurr exception: {ex.Message}");
                return false;
            }
        }
    }
}
