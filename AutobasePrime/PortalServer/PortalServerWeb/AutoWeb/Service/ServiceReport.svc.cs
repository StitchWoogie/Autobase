
using AutoLibLocal;
using Newtonsoft.Json;
using PortalServerWeb.Library;
using ReportBasicLib;
using ReportModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WcfServiceReport"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WcfServiceReport.svc나 WcfServiceReport.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfServiceReport : IServiceReport
    {
        HttpContext Ctx => HttpContext.Current;

        private void Init()
        {
            // 기존 ASMX: ServiceLib.SetCommonVars(this);
            ServiceLib.SetCommonVars();

            // TryLogIn / TagLib 초기화
            ServiceDataTagStatic.TryLogIn(TotalConfig.sDirWorkProject);
            TagLib.LocalTagLoad();
        }

        public string[] GetReportLists()
        {
            try
            {
                Init();
                return ReportLib.GetReportListsLocal();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("GetReportLists() Error" + ex.Message);
                return new string[0];
            }
        }

        public async Task< byte[]> GetReportStruct(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                Init();

                string path = $"{TotalConfig.sDirWorkProject}\\Report\\{Path.GetFileName(filename)}";

                ReportConfig.tHandReportTime = tHand;
                ReportConfig.tAutoReportTime = tAuto;
                ReportConfig.bRunByIIS = true;
                ReportConfig.SetMinListTimeFr(tMinListFr);
                ReportConfig.SetMinListTimeTo(tMinListTo);

                MakeRunReport make = new MakeRunReport();
                REPORT_STRUCT report = await make.MakeByFile(path, (EnumHandAuto)hand_auto);
                if (report == null) return null;

                // 이진 직렬화 그대로 유지
                using (MemoryStream m = new MemoryStream())
                {
                    BinaryFormatter fmt = new BinaryFormatter();
                    fmt.Serialize(m, report);
                    return m.ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetReportStruct() Error" + ex.Message);
                return null;
            }
        }

        public async Task<byte[]> GetReportStructWithDic(string[] keys, string[] values, string filename,
            DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo,
            int hand_auto)
        {
            try
            {
                ConfigVarTotal.varKeys = keys;
                ConfigVarTotal.varValues = values;

                return await GetReportStruct(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetReportStructWithDic() Error" + ex.Message);
                return null;
            }
        }

        public bool ReportSetVar(string name, string val)
        {
            try
            {
                Init();
                Ctx.Session[name] = val;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ReportSetVar() Error" + ex.Message);
                return false;
            }
        }

        public async Task<string> GetReportStructJson(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                Init();

                string path = $"{TotalConfig.sDirWorkProject}\\Report\\{Path.GetFileName(filename)}";

                ReportConfig.tHandReportTime = tHand;
                ReportConfig.tAutoReportTime = tAuto;
                ReportConfig.bRunByIIS = true;
                ReportConfig.SetMinListTimeFr(tMinListFr);
                ReportConfig.SetMinListTimeTo(tMinListTo);

                MakeRunReport make = new MakeRunReport();
                REPORT_STRUCT report = await make.MakeByFile(path, (EnumHandAuto)hand_auto);
                if (report == null) return string.Empty;

                return JsonConvert.SerializeObject(report, Formatting.None, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Include,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetReportStructJson() Error" + ex.Message);
                return string.Empty;
            }
        }

        public async Task<string> GetReportStructWithDicJson(string[] keys, string[] values, string filename,
            DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo,
            int hand_auto)
        {
            try
            {
                ConfigVarTotal.varKeys = keys;
                ConfigVarTotal.varValues = values;

                return await GetReportStructJson(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetReportStructWithDicJson() Error" + ex.Message);
                return string.Empty;
            }
        }

        public async Task<byte[][]> GetReportBitmapPngs(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                Init();

                ReportExecutionRequest request = CreateExecutionRequest(
                    filename,
                    tHand,
                    tAuto,
                    tMinListFr,
                    tMinListTo,
                    hand_auto,
                    ConfigVarTotal.varKeys,
                    ConfigVarTotal.varValues);

                ReportBitmapRenderFacade renderer = new ReportBitmapRenderFacade();
                List<byte[]> pages = await renderer.RenderPngBytesAsync(request).ConfigureAwait(false);
                if (pages == null || pages.Count == 0)
                {
                    return null;
                }

                return pages.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetReportBitmapPngs() Error" + ex.Message);
                return null;
            }
        }

        public async Task<byte[][]> GetReportBitmapPngsWithDic(string[] keys, string[] values, string filename,
            DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo,
            int hand_auto)
        {
            try
            {
                ConfigVarTotal.varKeys = keys;
                ConfigVarTotal.varValues = values;

                return await GetReportBitmapPngs(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetReportBitmapPngsWithDic() Error" + ex.Message);
                return null;
            }
        }

        private static ReportExecutionRequest CreateExecutionRequest(
            string filename,
            DateTime tHand,
            DateTime tAuto,
            DateTime tMinListFr,
            DateTime tMinListTo,
            int hand_auto,
            string[] keys,
            string[] values)
        {
            ReportExecutionRequest request = new ReportExecutionRequest
            {
                TemplateFile = Path.GetFileName(filename),
                HandAuto = (EnumHandAuto)hand_auto,
                HandTime = tHand,
                AutoTime = tAuto,
                MinListFrom = tMinListFr,
                MinListTo = tMinListTo
            };

            if (keys == null || values == null)
            {
                return request;
            }

            int max = Math.Min(keys.Length, values.Length);
            if (max <= 0)
            {
                return request;
            }

            request.StringVariables = new Dictionary<string, string>();
            for (int i = 0; i < max; i++)
            {
                string key = keys[i];
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                request.StringVariables[key] = values[i];
            }

            return request;
        }
    }
}
