using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.Windows.Forms;

namespace ScriptLibRun
{
    public class PrepareLibrary
    {
        // C의 기본함수나 기본클래스 중에서도 static인 것만 선언해서 사용하도록 한다.
        // 기본Namespace는 WPF엔진을 기본으로 한다. MessageBox 는 Form에서는 System.Windows.Forms에 있지만 WPF에서는 System.Windows에 있다.
        public static void Prepare(ScriptLibMain main)
        {
            ScriptLibLibrary sll = new ScriptLibLibrary();
            sll.sNameLibrary = "_InternalLibrary_";
            sll.bInternalLibrary = true;

            main.arrayLibrary.Add(sll);

            ScriptLibNamespace sln = new ScriptLibNamespace(sll);
            sln.sNameNamespace = "System";

            PrepareLibraryMath.AddClassMath(sln);
            PrepareLibraryNative.AddClassNative(sln);
            PrepareLibraryString.AddClassString(sln);
            PrepareLibraryConsole.AddClassConsole(sln);

            sll.arrayNamespace.Add(sln);

            sln = new ScriptLibNamespace(sll);
            sln.sNameNamespace = "System.Windows";

            PrepareLibraryMessageBox.AddClassMessageBox(sln);

            sll.arrayNamespace.Add(sln);

            sln = new ScriptLibNamespace(sll);
            sln.sNameNamespace = "System.Diagnostics";

            PrepareLibraryDebug.AddClassDebug(sln);

            sll.arrayNamespace.Add(sln);
        }

        static void AddMethodLocal(ScriptLibClass slc, string name, bool bStatic, ScriptLibMemberMethod.DeleMethod proc, params string[] args)
        {
            ScriptLibMemberMethod slm = new ScriptLibMemberMethod(slc);
            slm.sNameMethod = name;
            slm.eAccessLevel = EnumAccessLevel.access_public;
            slm.bStatic = bStatic;

            MethodArgument ma;
            CommaBlockString comma = new CommaBlockString();
            comma.SetBlockCode(':');
            string buf;

            for (int i = 0; i < args.Length; i++)
            {
                ma = new MethodArgument();
                comma.Set(args[i]);
                buf = comma.GetString();

                if (buf == "ref")
                    ma.eInOut = EnumInOut.Ref;
                else if (buf == "out")
                    ma.eInOut = EnumInOut.Out;
                else if (buf == "params")
                    ma.eInOut = EnumInOut.Params;
                else
                    ma.eInOut = EnumInOut.In;

                buf = comma.GetString();
                ma.pVar.sVarType = buf;

                if (comma.IsEOS())
                {
                    string msg = String.Format("{0} 가 inout:data:name 형식으로 구성되어야 합니다. MethodName={1}", args[i], name);
                    MessageBox.Show(msg, "PrepareLibraryError");
                    return;
                }

                buf = comma.GetString();
                ma.pVar.sVarName = buf;

                slm.arrayParams.Add(ma);
            }

            slm.procMethodByProgram = proc;
            slc.arrayMethod.Add(slm);
        }

        // 일단은 기본 라이브러리는 public static 을 모두 가지고 있다.
        public static void AddMethod(ScriptLibClass slc, string name, ScriptLibMemberMethod.DeleMethod proc, params string[] args)
        {
            AddMethodLocal(slc, name, true, proc, args);
        }

        // 기본 라이브러리 중에서 String.ToLower() 같은 Method는 static이 아니다.
        public static void AddMethodNonStatic(ScriptLibClass slc, string name, ScriptLibMemberMethod.DeleMethod proc, params string[] args)
        {
            AddMethodLocal(slc, name, false, proc, args);
        }

        /*
        static void AddVariable(ScriptLibClass slc, string name, bool bStatic, ScriptLibMemberMethod.DeleMethod proc, params string[] args)
        {
            ScriptLibMemberVariable slv = new ScriptLibMemberVariable();
            
            slm.sNameMethod = name;
            slm.eAccessLevel = EnumAccessLevel.access_public;
            slm.bStatic = bStatic;

            MethodArgument ma;
            CommaBlockString comma = new CommaBlockString();
            comma.SetBlockCode(':');
            string buf;

            for (int i = 0; i < args.Length; i++)
            {
                ma = new MethodArgument();
                comma.Set(args[i]);
                buf = comma.GetString();

                if (buf == "ref")
                    ma.eInOut = EnumInOut.Ref;
                else if (buf == "out")
                    ma.eInOut = EnumInOut.Out;
                else if (buf == "params")
                    ma.eInOut = EnumInOut.Params;
                else
                    ma.eInOut = EnumInOut.In;

                buf = comma.GetString();
                ma.pVar.sVarType = buf;

                if (comma.IsEOS())
                {
                    string msg = String.Format("{0} 가 inout:data:name 형식으로 구성되어야 합니다. MethodName={1}", args[i], name);
                    MessageBox.Show(msg, "PrepareLibraryError");
                    return;
                }

                buf = comma.GetString();
                ma.pVar.sVarName = buf;

                slm.arrayParams.Add(ma);
            }

            slm.procMethodByProgram = proc;
            slc.arrayMethod.Add(slm);
        }
        */
    }
}
