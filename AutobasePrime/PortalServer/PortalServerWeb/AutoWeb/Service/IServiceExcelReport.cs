using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    [ServiceContract]
    public interface IServiceExcelReport
    {
        [OperationContract]
        string GetReportStruct(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        string GetReportStructWithDic(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        ExcelWorkerJobInfo EnqueueExcelReportJob(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        ExcelWorkerJobInfo EnqueueExcelReportJobWithDic(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        ExcelWorkerJobInfo GetExcelReportJob(string jobId);

        [OperationContract]
        ExcelWorkerJobInfo ClaimNextExcelReportJob(string workerName);

        [OperationContract]
        ExcelWorkerJobInfo CompleteExcelReportJob(string jobId, string workerName, string resultUrl, string resultFileName);

        [OperationContract]
        ExcelWorkerJobInfo FailExcelReportJob(string jobId, string workerName, string errorMessage);
    }
}
