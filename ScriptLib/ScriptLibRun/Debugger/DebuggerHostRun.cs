using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NetTools;
using System.Threading;

namespace ScriptLibRun.Debugger
{
    public class DebuggerHostRun
    {
        static ServiceHost hostService;

        public static void Init()
        {
            Thread thread = new Thread(WcfThread);
            thread.Start();
        }

        static bool bDone = false;

        static void WcfThread()
        {
            string url = String.Format("net.tcp://localhost:{0}/autobase/debug/ServiceDebugRun", (int)EnumReservedPort.AutoBaseProgramLocalMain);

            hostService = new ServiceHost(typeof(DebuggerRunService), new Uri(url));

            NetTcpBinding ntb = new NetTcpBinding();
            //ntb.PortSharingEnabled = true;

            hostService.AddServiceEndpoint(
                    typeof(IDebuggerRun),        // service contract
                    ntb,        // service binding
                    "");                // relative address

            hostService.Open();

            while (!bDone)
            {
                Thread.Sleep(1);
            }

            hostService.Close();
        }

        public static void UnInit()
        {
            bDone = true;
        }
    }
}
