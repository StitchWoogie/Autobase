using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    class PrepareLibraryNative
    {
        public static void AddClassNative(ScriptLibNamespace sln)
        {
            ScriptLibClass slc = new ScriptLibClass(sln);
            slc.sNameClass = "Native";

            AddMethodGetTime(slc);

            sln.arrayMember.Add(slc);
        }

        static void AddMethodGetTime(ScriptLibClass slc)
        {
            PrepareLibrary.AddMethod(slc, "GetTime", new ScriptLibMemberMethod.DeleMethod(ProcGetTime), "out:int:year", "out:int:month", "out:int:day", "out:int:hour", "out:int:minute", "out:int:second", "out:int:milisecond");
        }

        static bool ProcGetTime(ClassDataStack cds, out object retn, object[] args)
        {
            DateTime t = DateTime.Now;
            args[0] = t.Year;
            args[1] = t.Month;
            args[2] = t.Day;
            args[3] = t.Hour;
            args[4] = t.Minute;
            args[5] = t.Second;
            args[6] = t.Millisecond;

            retn = 0;
            return true;
        }


    }
}
