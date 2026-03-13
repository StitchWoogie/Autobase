using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;

namespace ScriptLibRun.Debugger
{
    // 서비스 Contract 선언
    [ServiceContract]
    public interface IDebuggerRun
    {
        [OperationContract]
        string SayHello();

        [OperationContract]
        void NextCommand(EnumDebugStep step);

        [OperationContract]
        void SetBreakPoint(string source_file, int y);
    }

    

    // 서비스 타입 구현
    public class DebuggerRunService : IDebuggerRun
    {
        public string SayHello()
        {
            return "Hello, i am Run";
        }

        static EnumDebugStep eDebugStep;

        public void NextCommand(EnumDebugStep step)
        {
            eDebugStep = step;
            bNextCommand = true;
        }

        static bool bNextCommand = false;

        public static EnumDebugStep WaitNextCommand()
        {
            while (true)
            {
                if (bNextCommand)
                {
                    bNextCommand = false;
                    return eDebugStep;
                }
                Thread.Sleep(1);
            }
        }

        public delegate void DelegateSetBreakPoint(string source_file, int y);
        public static DelegateSetBreakPoint procSetBreakPoint = null;

        public void SetBreakPoint(string source_file, int y)
        {
            if (procSetBreakPoint == null) return;

            procSetBreakPoint(source_file, y);
        }
    }
}
