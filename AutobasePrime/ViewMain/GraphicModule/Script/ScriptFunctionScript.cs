using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.Collections;
using AutoLibLocal;
using ScriptLibRun;
using System.Windows.Forms;

namespace GraphicModule
{
    public class ScriptFunctionScript
    {
        /*
        static ScriptLibRun.ScriptLibMain scriptLibMain = null;

        static void ReadyNewScript()
        {
            if (scriptLibMain != null) return;

            scriptLibMain = new ScriptLibRun.ScriptLibMain();

            string filename = String.Format("{0}\\Bin", TotalConfig.sDirWorkProject);
            scriptLibMain.LoadAllLibrary(filename);
        }*/

        /*
        static int Function_ScriptCall(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;

            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string callname;
            object param;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out callname)) return -1;

            ArrayList args = new ArrayList();

            while (true)
            {
                arg.GetArgument(out buf);
                if (buf.Length == 0) break;

                if (!scriptClass.GetValueRecurse(buf, out param)) return -1;
                args.Add(param);
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string name_namespace;
                string name_class;
                string name_method;

                ScriptLibTools.SplitNamespaceClassMethod(callname, out name_namespace, out name_class, out name_method);

                if (name_class == null || name_namespace == null)
                {
                    scriptClass.ErrorMessage(String.Format("MyScriptCall 의 arg1({0}) 에서 namespace.class.method 형식으로 지정되어야 합니다.", callname));
                    return -1;
                }

                ScriptClass.ReadyNewScript();

                object[] p = null;
                if (args.Count > 0)
                {
                    p = new object[args.Count];
                    for (int i = 0; i < args.Count; i++)
                    {
                        p[i] = args[i];
                    }
                }

                ClassDataStack class_stack = new ClassDataStack(null);

                if (!ScriptClass.slmLocal.RunWithError(name_namespace, name_class, name_method, class_stack, out val, p))
                {
                    scriptClass.ErrorMessage(ScriptClass.slmLocal.arrayError[0].MakeErrorString());
                    return -1;
                }
            }

            return 1;
        }

        public static int Function_Script(ScriptClass scriptClass, string command, string argument, out object val)
        {
            // 이 함수는 함수 리스트에 올리는 것을 보류한다. ScriptCall은 이전스크립트에서 신 스크립트를 호출할 때 사용한다. 스크립트가 통합되면 이 함수는 필요 없을 듯 하다.
            if (command == "ScriptCall")
            {
                return Function_ScriptCall(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 MyScript?? 함수입니다.\n{0}", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 MyScript 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined MyScript??? function.\n{0}", command));
                }
                val = 0;
                return -1;
            }
        }*/

        public delegate object DelegateScriptLocalCallBack(string method_name, object[] args);
        public static GraphicModule.ScriptFunctionScript.DelegateScriptLocalCallBack procCommonCallBack = null;

        static int Run_ScriptCommon(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            if (procCommonCallBack != null)
            {
                val = procCommonCallBack(method_name, args);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Script";

            prepare.AddMethod(prename, "ScriptGetActiveAll", "int", new ScriptExternalRun.DeleMethod(Run_ScriptCommon));
            prepare.AddMethod(prename, "ScriptGetActiveFile", "int", new ScriptExternalRun.DeleMethod(Run_ScriptCommon), "in:string:scriptname");
            prepare.AddMethod(prename, "ScriptSetActiveAll", "void", new ScriptExternalRun.DeleMethod(Run_ScriptCommon), "in:int:flag");
            prepare.AddMethod(prename, "ScriptSetActiveFile", "void", new ScriptExternalRun.DeleMethod(Run_ScriptCommon), "in:string:scriptname", "in:int:flag");
        }
    }
}
