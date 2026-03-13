using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel.Description;
using System.ServiceModel;

namespace ScriptLibRun.Debugger
{
    class DebuggerMain
    {
        public static EnumDebugStep Goto(string filename, int col, int row, ClassDataStack class_stack, ClassDataStack data_stack)
        {
            class_stack.ConvertToStudioValue();
            data_stack.ConvertToStudioValue();

            Uri uri = new Uri("net.tcp://localhost/autobase/debug/ServiceDebugEdit");
            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IDebuggerEdit)),
                new NetTcpBinding(),
                new EndpointAddress(uri));

            ChannelFactory<IDebuggerEdit> factory = new ChannelFactory<IDebuggerEdit>(ep);
            IDebuggerEdit proxy = factory.CreateChannel();

            try
            {
                proxy.GotoCursor(filename, col, row, class_stack, data_stack);

                (proxy as IDisposable).Dispose();
            }
            catch
            {
                // data_stack의 value값이 object[] 일때는 Serialize가ㅣ 되지 않고 오류가 난다.

                ScriptLibMain.bDebugMode = false;   // 오류가 나면 Debug 모드를 풀어준다.
                return EnumDebugStep.StopNone;
            }

            return DebuggerRunService.WaitNextCommand();
        }

        public static void DebugWriteLine(string msg)
        {
            if(!ScriptLibRun.ScriptLibMain.bDebugMode)  return;

            Uri uri = new Uri("net.tcp://localhost/autobase/debug/ServiceDebugEdit");
            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IDebuggerEdit)),
                new NetTcpBinding(),
                new EndpointAddress(uri));

            ChannelFactory<IDebuggerEdit> factory = new ChannelFactory<IDebuggerEdit>(ep);
            IDebuggerEdit proxy = factory.CreateChannel();

            try
            {
                proxy.DebugWriteLine(msg);
                (proxy as IDisposable).Dispose();
            }
            catch
            {
                ScriptLibMain.bDebugMode = false;   // 오류가 나면 Debug 모드를 풀어준다.
            }
            finally
            {
                
            }
        }
    }
}
