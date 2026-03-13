using System;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionControlEditBox
    {
        public ScriptFunctionControlEditBox()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static int Function_EditBoxSetText(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string str;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out str)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectControlEditBox.arrayClassList, class_name, command, str);
            }

            return 1;
        }

        public static int Function_EditBoxGetText(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string data = "";
            string var;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out var);
            if (!scriptClass.IsStringVar(var, "EditBoxGetText() arg2")) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                data = scriptClass.ExecuteClassNameStringReturn(ObjectControlEditBox.arrayClassList, class_name, command);
            }

            if (!scriptClass.ChangeStringVar(var, data)) return -1;

            return 1;
        }

        public static int Function_EditBoxSelectAll(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectControlEditBox.arrayClassList, class_name, command);
            }

            return 1;
        }

        public static int Function_EditBoxSetFocus(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectControlEditBox.arrayClassList, class_name, command);
            }

            return 1;
        }

        public static int Function_EditBox(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "EditBoxSetText")
            {
                return Function_EditBoxSetText(scriptClass, command, argument, out val);
            }
            else if (command == "EditBoxGetText")
            {
                return Function_EditBoxGetText(scriptClass, command, argument, out val);
            }
            else if (command == "EditBoxSelectAll")
            {
                return Function_EditBoxSelectAll(scriptClass, command, argument, out val);
            }
            else if (command == "EditBoxSetFocus")
            {
                return Function_EditBoxSetFocus(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 EditBox 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 EditBox 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined EditBox function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}