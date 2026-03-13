using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    class PrepareLibraryMath
    {
        public static void AddClassMath(ScriptLibNamespace sln)
        {
            ScriptLibClass slc = new ScriptLibClass(sln);
            slc.sNameClass = "Math";

            PrepareLibrary.AddMethod(slc, "Abs", new ScriptLibMemberMethod.DeleMethod(MathAbs), "in:double:value");

            sln.arrayMember.Add(slc);
        }

        static bool MathAbs(ClassDataStack cds, out object retn, object[] args)
        {
            retn = Math.Abs((double)args[0]);
            return true;
        }


    }
}
