using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using PortalServerWeb.Library;
using AutoLibLocal;
using System.IO;
using System.Reflection;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for ServiceExcelReport
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceExcelReport : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public string GetReportStruct(string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ServiceLib.SetCommonVars(this);

            return GetExcelResult(Context.Request, filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto);
        }

        public static string GetExcelResult(HttpRequest request, string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ServiceDataTag.TryLogIn(TotalConfig.sDirWorkProject);   // 2006년 8월 21일 추가 리포터 데이터만 안되서 추가함

            TagLib.LocalTagLoad();	// 이 부분을 호출하지 않으면 리포터에서 태그 구조체를 요구하는 부분이 있으면 다운된다.

            string file = Path.GetFileName(filename);

            string source_file = TotalConfig.sDirWorkProject + "\\Report\\" + file;

            DateTime t = tHand;
            string target_name = String.Format("{6}_{0}{1:00}{2:00}-{3:00}{4:00}{5:00}{7}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, Path.GetFileNameWithoutExtension(file), Path.GetExtension(file));

            string target_file = TotalConfig.sDirWorkProject + "\\Result\\" + target_name;

            Assembly assembly;

            // LoadFile로 하면 ASP.NET에서는 잘 읽어오지 못한다. 어셈블리를 로드할 수 없다는 메시지가 나옴
            if (request.Url.Host == "localhost")
                assembly = Assembly.LoadFrom("d:\\exe\\autobase.prime\\WebServer\\AutoWeb\\Runtime\\OfficeExcelLibrary.dll");
            else
                assembly = Assembly.LoadFrom(ProjectLib.GetRuntimeDir(request) + "\\OfficeExcelLibrary.dll");

            object[] para = new object[6];

            para[0] = source_file;
            para[1] = target_file;
            para[2] = tHand;
            para[3] = tMinListFr;
            para[4] = tMinListTo;
            para[5] = true;     // target디렉토리를 지운다.

            foreach (Type type in assembly.GetTypes())
            {
                MethodInfo foo = type.GetMethod("MakeResult");
                if (foo != null)
                {
                    object newObj = assembly.CreateInstance(type.FullName);
                    foo.Invoke(newObj, para);
                    break;
                }
            }

            string url;

            if (request.Url.Host == "localhost")
            {
                //url = String.Format("{0}://{1}{2}/AutoWeb/Project/Result/{3}", request.Url.Scheme, request.Url.Authority, request.ApplicationPath, target_name);
                url = String.Format("{0}://{1}/AutoWeb/Project/Result/{2}", request.Url.Scheme, request.Url.Authority, target_name);
            }
            else
            {
                if (request.ApplicationPath.Length > 1)
                {    // "/"
                    url = String.Format("{0}://{1}{2}/AutoWeb/Project/Result/{3}", request.Url.Scheme, request.Url.Authority, request.ApplicationPath, target_name);
                }
                else
                {
                    url = String.Format("{0}://{1}/AutoWeb/Project/Result/{2}", request.Url.Scheme, request.Url.Authority, target_name);
                }
            }

            return url;
        }

        [WebMethod]
        public string GetReportStructWithDic(string[] keys, string[] values, string filename, DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ConfigVarTotal.varKeys = keys;
            ConfigVarTotal.varValues = values;

            ServiceLib.SetCommonVars(this);

            return GetExcelResult(Context.Request, filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto);            
        }
    }
}
