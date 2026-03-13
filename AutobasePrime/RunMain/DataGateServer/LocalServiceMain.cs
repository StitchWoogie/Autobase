using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RunMain
{
    class LocalServiceMain
    {
        static ServiceHost hostService;

        public static void Init()
        {
            /*
            hostService = new ServiceHost(typeof(ServiceDataGate), new Uri("http://192.168.1.100:8731/RunMain/ServiceDataGate"));

            BasicHttpBinding ntb = new BasicHttpBinding();

            //ntb.Security.Mode = SecurityMode.None;

            ntb.MaxReceivedMessageSize = 2147483647;
            ntb.MaxBufferSize = 2147483647;
            ntb.ReaderQuotas.MaxArrayLength = 2147483647;

            hostService.AddServiceEndpoint(
                    typeof(IServiceDataGate),        // service contract
                    ntb,        // service binding
                    "");                // relative address

            hostService.Open();*/

            //hostService = new ServiceHost(typeof(ServiceDataGate));
            //hostService.Open();

            
            //hostService = new ServiceHost(typeof(PortalServerWeb.AutoWeb.Service.ServiceDataGate));
            //hostService.Open();
            
            
            hostService = new ServiceHost(typeof(PortalServerWeb.AutoWeb.Service.ServiceDataGate), new Uri("net.tcp://localhost:8732/AutoWeb/Service/ServiceDataGate.svc"));

            // mexBinding을 추가하려면 Behavior가 있어야 한다.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            hostService.Description.Behaviors.Add(smb);
            
            NetTcpBinding ntb = new NetTcpBinding();

            ntb.Security.Mode = SecurityMode.None;

            ntb.MaxReceivedMessageSize = 2147483647;
            ntb.MaxBufferSize = 2147483647;
            ntb.ReaderQuotas.MaxArrayLength = 2147483647;

            hostService.AddServiceEndpoint(
                    typeof(PortalServerWeb.AutoWeb.Service.IServiceDataGate),        // service contract
                    ntb,        // service binding
                    "");                // relative address

            // Service Reference를 하려면 mexbinding이 필요하다.
            hostService.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            hostService.Open();
        }

        public static void UnInit()
        {
            hostService.Close();
        }
    }
}
