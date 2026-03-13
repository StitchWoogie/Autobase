using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ScriptLibRun.Debugger;
using System.ServiceModel;

namespace ScriptLibEdit.Debugger
{
    public class DebuggerHostEdit
    {
        static ServiceHost hostService;

        public static void Init()
        {
            hostService = new ServiceHost(typeof(DebuggerEditService),
                            new Uri("net.tcp://localhost/autobase/debug/ServiceDebugEdit"));

            NetTcpBinding ntb = new NetTcpBinding();

            //ntb.PortSharingEnabled = true;

            hostService.AddServiceEndpoint(
                    typeof(IDebuggerEdit),        // service contract
                    ntb,        // service binding
                    "");                // relative address

            hostService.Open();
        }

        public static void UnInit()
        {
            hostService.Close();
        }
    }
}
