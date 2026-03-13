using System;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace GraphicModule
{
    /// <summary>
    /// Summary description for ScriptFunctionTag.
    /// </summary>
    public class ScriptFunctionToolBar
    {
        /*
        static int Function_ToolBarAdd(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string filename;
            int position;
            int size;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out position)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out size)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ToolBarModule.AddOneToolBar(filename, position, size);
            }

            return 1;
        }

        static int Function_ToolBarDelete(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int pos;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ToolBarModule.DeleteOneToolBar(pos);
            }

            return 1;
        }

        static int Function_ToolBarChange(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string filename;
            int position;
            int size;
            int index;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out index)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out position)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out size)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ToolBarModule.ChangeOneToolBar(index, filename, position, size);
            }

            return 1;
        }

        public static int Function_ToolBar(ScriptClass scriptClass, string command, string argument, out object value)
        {
            if (String.Compare(command, "ToolBarAdd") == 0)
            {
                return Function_ToolBarAdd(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "ToolBarDelete") == 0)
            {
                return Function_ToolBarDelete(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "ToolBarChange") == 0)
            {
                return Function_ToolBarChange(scriptClass, command, argument, out value);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 ToolBar?? 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 ToolBar 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined ToolBar function ({0})", command));
                }
                value = 0;
                return -1;
            }
        }*/

        static int Run_ToolBarAdd(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            string filename = (string)args[0];
            int position = (int)args[1];
            int size = (int)args[2];

            value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ToolBarModule.AddOneToolBar(filename, position, size);
            }

            return 1;
        }

        static int Run_ToolBarDelete(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int pos = (int)args[0];

            value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ToolBarModule.DeleteOneToolBar(pos);
            }

            return 1;
        }

        static int Run_ToolBarChange(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            int index = (int)args[0];
            string filename = (string)args[1];
            int position = (int)args[2];
            int size = (int)args[3];

            value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ToolBarModule.ChangeOneToolBar(index, filename, position, size);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "ToolBar";

            prepare.AddMethod(prename, "ToolBarAdd", "void", new ScriptExternalRun.DeleMethod(Run_ToolBarAdd), "in:string:filename", "in:int:position", "in:int:size");
            prepare.AddMethod(prename, "ToolBarDelete", "void", new ScriptExternalRun.DeleMethod(Run_ToolBarDelete), "in:int:index");
            prepare.AddMethod(prename, "ToolBarChange", "void", new ScriptExternalRun.DeleMethod(Run_ToolBarChange), "in:int:index", "in:string:filename", "in:int:position", "in:int:size");
        }
    }
}


