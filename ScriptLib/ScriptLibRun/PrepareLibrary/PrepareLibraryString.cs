using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NetTools;

namespace ScriptLibRun
{
    // String 은 C#문법에서 struct가 아닌 class 속성이다.
    class PrepareLibraryString
    {
        public static void AddClassString(ScriptLibNamespace sln)
        {
            ScriptLibClass slc = new ScriptLibClass(sln);
            slc.sNameClass = "String";

            PrepareLibrary.AddMethod(slc, "Format", new ScriptLibMemberMethod.DeleMethod(StringFormat), "in:string:format", "params:object[]:args");

            PrepareLibrary.AddMethodNonStatic(slc, "ToLower", new ScriptLibMemberMethod.DeleMethod(ToLower));

            sln.arrayMember.Add(slc);
        }

        static bool StringFormat(ClassDataStack cds, out object retn, object[] args)
        {
            object[] args2 = new object[args.Length - 1];

            // params는 args[1] 부터 시작한다.
            for (int i = 0; i < args.Length - 1; i++)
            {
                args2[i] = args[i + 1];
            }

            retn = String.Format((string)args[0], args2);
            return true;
        }

        static bool ToLower(ClassDataStack cds, out object retn, object[] args)
        {
            string source = ObjectValue.ToString(cds.selfValue);

            retn = source.ToLower();
            return true;
        }
    }
}
