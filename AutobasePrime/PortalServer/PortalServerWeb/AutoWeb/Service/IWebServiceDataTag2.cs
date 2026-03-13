using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IServiceDataTag2"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IWebServiceDataTag2
    {
        [OperationContract]
        int CheckServiceAlive(int val);

        [OperationContract]
        string GetTagValueLists(List<string> array);

        [OperationContract]
        bool WriteCurrAI(string tag, double val, string guid);

        [OperationContract]
        bool WriteCurrST(string tag, string val, string guid);

        [OperationContract]
        Task<string> GetDataAi(string tag, int value_type, int data_time,
                        int year, int mon, int day, int hour, int min,
                        int data_count, int data_gab);

        [OperationContract]
        Task<string> GetDataDi(string tag, int value_type, int data_time,
                        int year, int mon, int day, int hour, int min,
                        int data_count, int data_gab);

        [OperationContract]
        Task<string> GetLogLists();

        [OperationContract]
        Task<string> GetLogFile(string name);

        [OperationContract]
        Task<string> GetAlarmLists();

        [OperationContract]
        Task<string> GetAlarmFile(string name);

        [OperationContract]
        Task<string> GetAlarmFileByScript2(DateTime tFrom, DateTime tTo, string option);

        [OperationContract]
        string GetServerVersion();

        [OperationContract]
        string GetServerInformations();

        [OperationContract]
        bool WriteCurr(string username, string computername, string tag, string val, byte[] hash, string guid);

    }
}
