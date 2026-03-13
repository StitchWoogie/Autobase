using AutoLibLocal;
using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WcfServiceExcelReport"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WcfServiceExcelReport.svc나 WcfServiceExcelReport.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfServiceExcelReport : IServiceExcelReport
    {
        HttpContext Ctx => HttpContext.Current;

        private ExcelWorkerJobStore CreateJobStore()
        {
            return new ExcelWorkerJobStore(ProjectLib.GetWorkDir(Ctx.Request));
        }

        private void Init()
        {
            // 기존 ASMX: ServiceLib.SetCommonVars(this);
            ServiceLib.SetCommonVars();

            ServiceDataTagStatic.TryLogIn(TotalConfig.sDirWorkProject);
            TagLib.LocalTagLoad();
        }

        public string GetReportStruct(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                Init();
                return GetExcelResult(Ctx.Request, filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetReportStruct + {ex.Message} ");
                return null;
            }
        }

        public string GetReportStructWithDic(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                ConfigVarTotal.varKeys = keys;
                ConfigVarTotal.varValues = values;

                Init();
                return GetExcelResult(Ctx.Request, filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetReportStructWithDic + {ex.Message} ");
                return null;
            }
        }

        public ExcelWorkerJobInfo EnqueueExcelReportJob(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                Init();
                return CreateJobStore().Enqueue(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto, null, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EnqueueExcelReportJob + {ex.Message} ");
                return null;
            }
        }

        public ExcelWorkerJobInfo EnqueueExcelReportJobWithDic(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            try
            {
                Init();
                return CreateJobStore().Enqueue(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto, keys, values);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EnqueueExcelReportJobWithDic + {ex.Message} ");
                return null;
            }
        }

        public ExcelWorkerJobInfo GetExcelReportJob(string jobId)
        {
            try
            {
                Init();
                return CreateJobStore().Get(jobId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetExcelReportJob + {ex.Message} ");
                return null;
            }
        }

        public ExcelWorkerJobInfo ClaimNextExcelReportJob(string workerName)
        {
            try
            {
                Init();
                return CreateJobStore().ClaimNext(workerName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClaimNextExcelReportJob + {ex.Message} ");
                return null;
            }
        }

        public ExcelWorkerJobInfo CompleteExcelReportJob(string jobId, string workerName, string resultUrl, string resultFileName)
        {
            try
            {
                Init();
                return CreateJobStore().Complete(jobId, workerName, resultUrl, resultFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CompleteExcelReportJob + {ex.Message} ");
                return null;
            }
        }

        public ExcelWorkerJobInfo FailExcelReportJob(string jobId, string workerName, string errorMessage)
        {
            try
            {
                Init();
                return CreateJobStore().Fail(jobId, workerName, errorMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FailExcelReportJob + {ex.Message} ");
                return null;
            }
        }

        // --- 기존 ASMX static 함수 그대로 유지 ---
        public static string GetExcelResult(HttpRequest request, string filename,
            DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo,
            int hand_auto)
        {
            ServiceDataTagStatic.TryLogIn(TotalConfig.sDirWorkProject);
            TagLib.LocalTagLoad();

            string file = Path.GetFileName(filename);
            string source_file = TotalConfig.sDirWorkProject + "\\Report\\" + file;

            DateTime t = tHand;
            string target_name = String.Format("{6}_{0}{1:00}{2:00}-{3:00}{4:00}{5:00}{7}",
                t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second,
                Path.GetFileNameWithoutExtension(file), Path.GetExtension(file));

            string target_file = TotalConfig.sDirWorkProject + "\\Result\\" + target_name;

            Assembly assembly;

            if (request.Url.Host == "localhost")
                assembly = Assembly.LoadFrom("d:\\exe\\autobase.prime\\WebServer\\AutoWeb\\Runtime\\OfficeExcelLibrary.dll");
            else
                assembly = Assembly.LoadFrom(ProjectLib.GetRuntimeDir(request) + "\\OfficeExcelLibrary.dll");

            object[] para = new object[6] {
                source_file, target_file,
                tHand, tMinListFr, tMinListTo,
                true
            };

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
                url = $"{request.Url.Scheme}://{request.Url.Authority}/AutoWeb/Project/Result/{target_name}";
            }
            else
            {
                if (request.ApplicationPath.Length > 1)
                    url = $"{request.Url.Scheme}://{request.Url.Authority}{request.ApplicationPath}/AutoWeb/Project/Result/{target_name}";
                else
                    url = $"{request.Url.Scheme}://{request.Url.Authority}/AutoWeb/Project/Result/{target_name}";
            }

            return url;
        }
    }
}
