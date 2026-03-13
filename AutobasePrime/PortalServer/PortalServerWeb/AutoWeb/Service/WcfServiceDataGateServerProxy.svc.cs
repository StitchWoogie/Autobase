using AutoLibLocal;
using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WcfServiceDataGateServerProxy"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WcfServiceDataGateServerProxy.svc나 WcfServiceDataGateServerProxy.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfServiceDataGateServerProxy : IWcfServiceDataGateServerProxy
    {
        private void Init()
        {
            // 기존 ASMX → WCF 공통 변수 초기화
            ServiceLib.SetCommonVars();

            ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
            ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
            ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
            ConfigVarTotal.nServicePort = 8732;
        }


        public string CheckServerEvent(int id, string hash)
        {
            try
            {
                Init();

                var service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();
                return service.CheckServerEvent(id, hash);
            }
            catch (Exception ex)

            {
                Debug.WriteLine("CheckServerEvent Exception: " + ex.Message);
            }
            return string.Empty;

        }

        public string GetAlarmEvents(int id, string hash)
        {
            try
            {
                Init();

                var service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();
                return service.GetAlarmEvents(id, hash);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetAlarmEvents Exception: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// ServiceLibSvcDataGate.GetServiceDataGate()를 통해 LocalMain으로 포워딩
        /// </summary>
        public bool ExecuteCommand(int id, string command, string argument, string hash, string guid)
        {
            try
            {
                if(!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    Debug.WriteLine("ExecuteCommand: Client not connected. Guid=" + guid);
                    return false;
                }

                Init();

                var service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();
                return service.ExecuteCommand(id, command, argument, hash);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteCommand Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        ///  LocalMain으로 포워딩하지 않고 로컬 ClassDataGateServer에서 직접 처리
        /// </summary>
        public int CommonMethod(List<byte[]> args, out List<byte[]> result)
        {
            Init();

            ClassDataGateServer server = new ClassDataGateServer();
            return server.CommonMethod(args, out result);
        }
    }
}
