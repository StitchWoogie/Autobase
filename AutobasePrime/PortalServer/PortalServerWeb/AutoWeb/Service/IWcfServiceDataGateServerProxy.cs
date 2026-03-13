using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWcfServiceDataGateServerProxy"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IWcfServiceDataGateServerProxy
    {
        [OperationContract]
        string CheckServerEvent(int id, string hash);

        [OperationContract]
        string GetAlarmEvents(int id, string hash);

        [OperationContract]
        bool ExecuteCommand(int id, string command, string argument, string hash, string guid);

        [OperationContract]
        int CommonMethod(List<byte[]> args, out List<byte[]> result);
    }
}
