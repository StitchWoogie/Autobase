using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using ScriptLibRun.Debugger;
using System.ServiceModel;
using NetTools;

namespace ScriptLibEdit.Debugger
{
    public class DebuggerEditMain
    {
        static IDebuggerRun GetProxy()
        {
            string url = String.Format("net.tcp://localhost:{0}/autobase/debug/ServiceDebugRun", (int)EnumReservedPort.AutoBaseProgramLocalMain);

            Uri uri = new Uri(url);

            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IDebuggerRun)),
                new NetTcpBinding(),
                new EndpointAddress(uri));

            ChannelFactory<IDebuggerRun> factory = new ChannelFactory<IDebuggerRun>(ep);
            IDebuggerRun proxy = factory.CreateChannel();

            return proxy;
        }

        public static void SendNextCommand(EnumDebugStep step)
        {
            IDebuggerRun proxy = GetProxy();

            proxy.NextCommand(step);

            (proxy as IDisposable).Dispose();
        }

        public static void SetBreakPoint(string source_file, int y)
        {
            IDebuggerRun proxy = GetProxy();

            proxy.SetBreakPoint(source_file, y);

            (proxy as IDisposable).Dispose();
        }

        /*
        public static void SendNextCommand(EnumDebugStep step)
        {
            string url = String.Format("net.tcp://localhost:{0}/autobase/debug/ServiceDebugRun", (int)EnumReservedPort.AutoBaseProgramLocalMain);

            Uri uri = new Uri(url);

            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IDebuggerRun)),
                new NetTcpBinding(),
                new EndpointAddress(uri));

            ChannelFactory<IDebuggerRun> factory = new ChannelFactory<IDebuggerRun>(ep);
            IDebuggerRun proxy = factory.CreateChannel();

            proxy.NextCommand(step);

            (proxy as IDisposable).Dispose();
        }

        public static void ToggleBreakPoint(string source_file, int y, bool toggle)
        {
            string url = String.Format("net.tcp://localhost:{0}/autobase/debug/ServiceDebugRun", (int)EnumReservedPort.AutoBaseProgramLocalMain);

            Uri uri = new Uri(url);

            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IDebuggerRun)),
                new NetTcpBinding(),
                new EndpointAddress(uri));

            ChannelFactory<IDebuggerRun> factory = new ChannelFactory<IDebuggerRun>(ep);
            IDebuggerRun proxy = factory.CreateChannel();

            proxy.ToggleBreakPoint(source_file, y, toggle);

            (proxy as IDisposable).Dispose();
        }*/
    }
}
