using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWcfServiceDataTag"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IServiceDataTag
    {
        [OperationContract]
        int CheckServiceAlive(int val);

        [OperationContract]
        DataSet GetTagValueList(ArrayList array);

        [OperationContract]
        bool WriteCurrAI(string tag, double val, string guid);

        [OperationContract]
        bool WriteCurrST(string tag, string val, string guid);

        [OperationContract]
        Task<DataSet> GetDataAi(string tag, int value_type, int data_time,
            int year, int mon, int day, int hour, int min, int data_count, int data_gab);

        [OperationContract]
        Task<DataSet> GetDataDi(string tag, int value_type, int data_time,
            int year, int mon, int day, int hour, int min, int data_count, int data_gab);

        [OperationContract]
        Task<DataSet> GetLogLists();

        [OperationContract]
        Task<DataSet> GetLogFile(string name);

        [OperationContract]
        Task<DataSet> GetAlarmLists();

        [OperationContract]
        Task<DataSet> GetAlarmFile(string name);

        [OperationContract]
        Task<DataSet> GetAlarmFileByScript(int year, int month, int day, int option);

        [OperationContract]
        Task<DataSet> GetAlarmFileByScript2(DateTime tFrom, DateTime tTo, string option);
    }
}
