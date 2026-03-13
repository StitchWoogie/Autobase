using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun.Debugger;

namespace ScriptLibRun
{
    class PrepareLibraryDebug
    {
        public static void AddClassDebug(ScriptLibNamespace sln)
        {
            ScriptLibClass slc = new ScriptLibClass(sln);
            slc.sNameClass = "Debug";

            PrepareLibrary.AddMethod(slc, "WriteLine", new ScriptLibMemberMethod.DeleMethod(WriteLine), "in:string:format", "params:object[]:args");

            sln.arrayMember.Add(slc);
        }

        static bool WriteLine(ClassDataStack cds, out object retn, object[] args)
        {
            object[] args2 = new object[args.Length - 1];

            // params는 args[1] 부터 시작한다.
            for (int i = 0; i < args.Length - 1; i++)
            {
                args2[i] = args[i + 1];
            }

            string msg = String.Format((string)args[0], args2);
            retn = msg;

            DebuggerMain.DebugWriteLine(msg);

            return true;
        }

    }
}
