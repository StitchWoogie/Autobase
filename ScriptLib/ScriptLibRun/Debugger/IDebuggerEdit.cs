using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ScriptLibRun.Debugger
{
    // 서비스 Contract 선언
    [ServiceContract]
    public interface IDebuggerEdit
    {
        //[OperationContract]
        //string SayHello();

        [OperationContract]
        void GotoCursor(string filename, int col, int row, ClassDataStack class_stack, ClassDataStack data_stack);

        [OperationContract]
        void DebugWriteLine(string msg);
    }

    // 서비스 타입 구현
    public class DebuggerEditService : IDebuggerEdit
    {
        /*
        public string SayHello()
        {
            return "Hello, i am Edit";
        }*/

        public delegate void DelegateGotoCursor(string filename, int col, int row, ClassDataStack class_stack, ClassDataStack data_stack);
        public static DelegateGotoCursor procGotoCursor = null;

        public void GotoCursor(string filename, int col, int row, ClassDataStack class_stack, ClassDataStack data_stack)
        {
            if (procGotoCursor == null) return;

            procGotoCursor(filename, col, row, class_stack, data_stack);
        }

        public delegate void DelegateDebugWriteLine(string msg);
        public static DelegateDebugWriteLine procDebugWriteLine = null;

        public void DebugWriteLine(string msg)
        {
            if (procDebugWriteLine == null) return;

            procDebugWriteLine(msg);
        }
    }
}
