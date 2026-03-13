using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using PortalServerWeb.Library;
using AutoLibLocal;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// 아래에 있는 함수는 ServiceDataGateServer.svc 를 사용하기 전에 ViewMain.exe 에서 실시간 경보를 가져가기 위해서 존재한다. WebClient에서 asmx 를 통해 호출하면
    /// 여기서는 LocalMain의 ServiceDataGateServer WCF로 CheckServerEvent, GetAlarmEvents, ExecuteCommand 3개를 호출하여 보내준다. 
    /// 이때는 id 가 모두 -2이었다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceDataGateServer : System.Web.Services.WebService
    {
        [WebMethod]
        public string CheckServerEvent(int id, string hash)
        {
            ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
            ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
            ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
            ConfigVarTotal.nServicePort = 8732;

            AutoLib.WcfReferenceDataGateServer.ServiceDataGateServerClient service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();

            return service.CheckServerEvent(id, hash);
        }

        [WebMethod]
        public string GetAlarmEvents(int id, string hash)
        {
            ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
            ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
            ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
            ConfigVarTotal.nServicePort = 8732;

            AutoLib.WcfReferenceDataGateServer.ServiceDataGateServerClient service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();

            return service.GetAlarmEvents(id, hash);
        }

        // 클라이언트의 각종 명령을 수행한다.
        [WebMethod]
        public bool ExecuteCommand(int id, string command, string argument, string hash)
        {
            ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
            ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
            ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
            ConfigVarTotal.nServicePort = 8732;

            AutoLib.WcfReferenceDataGateServer.ServiceDataGateServerClient service = AutoLib.ServiceLibSvcDataGate.GetServiceDataGate();

            return service.ExecuteCommand(id, command, argument, hash);
        }

        // #############################################################################################################################
        // 미세자료 트랜드에서 asmx를 통한 통신이 필요하면서 ServiceDataGateServer.svc와 WebServiceDataGateServer.asmx 에서 같이 
        // CommonMethod 하나만 가지고 통신을 하도록 추가 지원하였다. 2016-12-20
        // #############################################################################################################################

        [WebMethod]
        public int CommonMethod(List<byte[]> args, out List<byte[]> result)
        {
            // ServiceDataGateServer.svc 와 같은 클래스를 사용한다.
            ClassDataGateServer cdgs = new ClassDataGateServer();

            return cdgs.CommonMethod(args, out result);
        }
    }
}
