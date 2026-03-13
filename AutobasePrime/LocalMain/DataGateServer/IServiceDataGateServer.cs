using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    // CommonMethod의 반환 타입
    [DataContract]
    public class CommonMethodResult
    {
        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public List<byte[]> Recv { get; set; }
    }

    // 서비스 Contract 선언
    [ServiceContract]
    public interface IServiceDataGateServer
    {
        // 아래의 함수는 10.3.2.4 이전에 웹서버에서 asmx 서비스에서 ID=-2 로 호출했기 때문에 호환성을 위해서 그냥 둔다.
        [OperationContract]
        string CheckServerEvent(int id, string hash);
        [OperationContract]
        string GetAlarmEvents(int id, string hash);
        [OperationContract]
        bool ExecuteCommand(int id, string command, string argument, string hash);

        // 10.3.2.5 부터 WCF 를 LocalMain과 웹서버에 동일하게 두어 UWP에서는 항상 ServiceDataGateServer.svc 하나만으로 호출할 수 있게 하였다.
        // 즉 Net.tcp 와 http  프로토콜을 동시에 사용할 수 있도록 지원하였다.

        [OperationContract]
        Task<CommonMethodResult> CommonMethod(List<byte[]> send);
        //  Task<(int, List<byte[]> recv)> CommonMethod(List<byte[]> send );

        // 변경하기전 함수
        /*
        [OperationContract]
        string GetServerVersion();

        [OperationContract]
        int Connect();

        [OperationContract]
        void DisConnect(int id, string hash);

        [OperationContract]
        bool DefaultUserCheck(int id, string hash, out string username, out string err_msg);

        [OperationContract]
        bool CheckUserNameWithVersion(int id, string username, string password, string hash, out string err_msg);

        [OperationContract]
        string GetProjectFilesInfo(int id, string hash);

        [OperationContract]
        byte[] DownLoadFile(int id, string filename, string hash);

        [OperationContract]
        byte[] DownLoadFileWithCompare(int id, string filename, string hash, ref DateTime file_time, ref long file_size, out int result);

        [OperationContract]
        string[] GetTagValues(int id, string[] tags, string hash);

        [OperationContract]
        void WriteCurr(int id, string username, string computer, string tag, string val, string hash);

        [OperationContract]
        string GetAlarmLists(int id, string hash);

        [OperationContract]
        string GetAlarmFile(int id, string filename, string hash);

        [OperationContract]
        string GetLogLists(int id, string hash);

        [OperationContract]
        string GetLogFile(int id, string filename, string hash);

        [OperationContract]
        string GetReportLists(int id, string hash);

        [OperationContract]
        List<byte[]> GetReportFileByBitmap(int id, string filename, int year, int month, int day, int hour, int minute, int second, string hash);

        [OperationContract]
        string GetDataAi(int id, string tag, int value_type, int data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab, string hash);

        [OperationContract]
        string GetDataDi(int id, string tag, int value_type, int data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab, string hash);

        [OperationContract]
        string GetDataSetFromDsn(int id, string dsn, string command, out string err_msg, string hash);

        [OperationContract]
        bool DataSetCommand(int id, string dsn, string command, out string err_msg, string hash);*/
    }
}
