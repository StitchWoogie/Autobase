using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWcfServiceReport"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IServiceReport
    {
        [OperationContract]
        string[] GetReportLists();

        [OperationContract]
        Task<byte[]> GetReportStruct(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        Task<byte[]> GetReportStructWithDic(string[] keys, string[] values, string filename,
            DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        bool ReportSetVar(string name, string val);

        [OperationContract]
        Task<string> GetReportStructJson(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        Task<string> GetReportStructWithDicJson(string[] keys, string[] values,
            string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        Task<byte[][]> GetReportBitmapPngs(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);

        [OperationContract]
        Task<byte[][]> GetReportBitmapPngsWithDic(string[] keys, string[] values, string filename,
            DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto);
    }
}
