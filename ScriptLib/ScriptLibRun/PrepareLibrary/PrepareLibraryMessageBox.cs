using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScriptLibRun
{
    class PrepareLibraryMessageBox
    {
        public static void AddClassMessageBox(ScriptLibNamespace sln)
        {
            ScriptLibClass slc = new ScriptLibClass(sln);
            slc.sNameClass = "MessageBox";

            PrepareLibrary.AddMethod(slc, "Show", new ScriptLibMemberMethod.DeleMethod(Show1), "in:string:text");
            PrepareLibrary.AddMethod(slc, "Show", new ScriptLibMemberMethod.DeleMethod(Show2), "in:string:text", "in:string:caption");

            sln.arrayMember.Add(slc);
        }

        static bool Show1(ClassDataStack cds, out object retn, object[] args)
        {
            retn = MessageBox.Show((string)args[0]);
            return true;
        }

        static bool Show2(ClassDataStack cds, out object retn, object[] args)
        {
            retn = MessageBox.Show((string)args[0], (string)args[1]);
            return true;
        }


    }
}
