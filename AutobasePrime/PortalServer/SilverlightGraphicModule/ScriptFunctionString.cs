using System;
using AutoLib;
using AutoLibLocal;
using NetTools;

namespace SilverlightGraphicModule
{
    /// <summary>
    /// Summary description for ScriptFunctionComboBox.
    /// </summary>
    public class ScriptFunctionString
    {
        public ScriptFunctionString()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_Trim(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            val = source.Trim();

            return 1;
        }

        static int Function_TrimEnd(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            val = source.TrimEnd();

            return 1;
        }

        static int Function_TrimStart(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            val = source.TrimStart();

            return 1;
        }

        static int Function_IndexOf(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;
            string search;
            int index;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out search)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out index)) return -1;

            val = source.IndexOf(search, index);

            return 1;
        }

        static int Function_Substring(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;
            int index;
            int length;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out index)) return -1;

            arg.GetArgument(out buf);
            if (buf.Length == 0)
            {
                val = source.Substring(index);
            }
            else
            {
                if (!scriptClass.GetValueRecurse(buf, out length)) return -1;
                val = source.Substring(index, length);
            }

            return 1;
        }

        public static int Function_String(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "StringTrim")
            {
                return Function_Trim(scriptClass, command, argument, out val);
            }
            else if (command == "StringTrimEnd")
            {
                return Function_TrimEnd(scriptClass, command, argument, out val);
            }
            else if (command == "StringTrimStart")
            {
                return Function_TrimStart(scriptClass, command, argument, out val);
            }
            else if (command == "StringIndexOf")
            {
                return Function_IndexOf(scriptClass, command, argument, out val);
            }
            else if (command == "StringSubstring")
            {
                return Function_Substring(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 String 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 String 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined String function ({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
