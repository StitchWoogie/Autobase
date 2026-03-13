using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using PortalServerWeb.Library;
using ReportBasicLib;
using AutoLibLocal;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for ServiceReport
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceReport : System.Web.Services.WebService
    {

        [WebMethod]
        public string[] GetReportLists()
        {
            ServiceLib.SetCommonVars(this);

            return ReportLib.GetReportListsLocal();
        }

        [WebMethod(EnableSession = true)]
        public byte[] GetReportStruct(string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ServiceLib.SetCommonVars(this);

            ServiceDataTag.TryLogIn(TotalConfig.sDirWorkProject);   // 2006년 8월 21일 추가 리포터 데이터만 안되서 추가함

            TagLib.LocalTagLoad();	// 이 부분을 호출하지 않으면 리포터에서 태그 구조체를 요구하는 부분이 있으면 다운된다.

            string path = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, Path.GetFileName(filename));

            ReportConfig.tHandReportTime = tHand;
            ReportConfig.tAutoReportTime = tAuto;
            ReportConfig.bRunByIIS = true;
            ReportConfig.SetMinListTimeFr(tMinListFr);
            ReportConfig.SetMinListTimeTo(tMinListTo);

            MakeRunReport make = new MakeRunReport();

            REPORT_STRUCT report = make.MakeByFile(path, (EnumHandAuto)hand_auto).GetAwaiter().GetResult();

            if (report == null) return null;

            MemoryStream m = new MemoryStream();
            BinaryFormatter format = new BinaryFormatter();
            format.Serialize(m, report);
            byte[] b = new byte[m.Length];
            m.Seek(0, SeekOrigin.Begin);
            m.Read(b, 0, (int)m.Length);
            m.Close();
            return b;
        }

        [WebMethod]
        public byte[] GetReportStructWithDic(string[] keys, string[] values, string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ConfigVarTotal.varKeys = keys;
            ConfigVarTotal.varValues = values;

            ServiceLib.SetCommonVars(this);

            ServiceDataTag.TryLogIn(TotalConfig.sDirWorkProject);   // 2006년 8월 21일 추가 리포터 데이터만 안되서 추가함

            TagLib.LocalTagLoad();	// 이 부분을 호출하지 않으면 리포터에서 태그 구조체를 요구하는 부분이 있으면 다운된다.

            string path;

            //if (Path.GetPathRoot(filename) == null)
                path = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, filename);//Path.GetFileName(filename));
            //else
                //path = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, Path.GetFileName(filename));

            ReportConfig.tHandReportTime = tHand;
            ReportConfig.tAutoReportTime = tAuto;
            ReportConfig.bRunByIIS = true;
            ReportConfig.SetMinListTimeFr(tMinListFr);
            ReportConfig.SetMinListTimeTo(tMinListTo);

            MakeRunReport make = new MakeRunReport();

            REPORT_STRUCT report = make.MakeByFile(path, (EnumHandAuto)hand_auto).GetAwaiter().GetResult();

            if (report == null) return null;

            MemoryStream m = new MemoryStream();
            BinaryFormatter format = new BinaryFormatter();
            format.Serialize(m, report);
            byte[] b = new byte[m.Length];
            m.Seek(0, SeekOrigin.Begin);
            m.Read(b, 0, (int)m.Length);
            m.Close();
            return b;
        }

        [WebMethod(EnableSession = true)]
        public void ReportSetVar(string name, string val)
        {
            ServiceLib.SetCommonVars(this);

            Session[name] = val;
        }

        // ===== OpenSilver 호환을 위한 새로운 JSON 직렬화 메서드들 ===== 20250701 PSU 추가
        /// <summary>
        /// OpenSilver 호환을 위한 JSON 직렬화 버전
        /// </summary>
        [WebMethod(EnableSession = true)]
        public string GetReportStructJson(string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                ServiceLib.SetCommonVars(this);

                ServiceDataTag.TryLogIn(TotalConfig.sDirWorkProject);
                TagLib.LocalTagLoad();

                string path = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, Path.GetFileName(filename));

                ReportConfig.tHandReportTime = tHand;
                ReportConfig.tAutoReportTime = tAuto;
                ReportConfig.bRunByIIS = true;
                ReportConfig.SetMinListTimeFr(tMinListFr);
                ReportConfig.SetMinListTimeTo(tMinListTo);

                MakeRunReport make = new MakeRunReport();
                REPORT_STRUCT report = make.MakeByFile(path, (EnumHandAuto)hand_auto).GetAwaiter().GetResult();

                if (report == null) return string.Empty;

                // JSON 직렬화
                return JsonConvert.SerializeObject(report, Formatting.None, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Include,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                // 로그 기록 (필요시)
                // LogError(ex);
                throw new Exception(String.Format("Error during JSON serialization: {0}", ex.Message));
            }
        }
        /// <summary>
        /// 순수 JSON 문자열을 반환하는 버전 (Base64 인코딩 없음)
        /// </summary>
        [WebMethod]
        public string GetReportStructWithDicJson(string[] keys, string[] values, string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                ConfigVarTotal.varKeys = keys;
                ConfigVarTotal.varValues = values;

                ServiceLib.SetCommonVars(this);

                ServiceDataTag.TryLogIn(TotalConfig.sDirWorkProject);
                TagLib.LocalTagLoad();

                string path = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, filename);

                ReportConfig.tHandReportTime = tHand;
                ReportConfig.tAutoReportTime = tAuto;
                ReportConfig.bRunByIIS = true;
                ReportConfig.SetMinListTimeFr(tMinListFr);
                ReportConfig.SetMinListTimeTo(tMinListTo);

                MakeRunReport make = new MakeRunReport();
                REPORT_STRUCT report = make.MakeByFile(path, (EnumHandAuto)hand_auto).GetAwaiter().GetResult();

                if (report == null) return string.Empty;

                // JSON 직렬화하여 직접 반환
                return JsonConvert.SerializeObject(report, Formatting.None, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Include,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during JSON serialization: {0}", ex.Message));
            }
        }
    }
}
