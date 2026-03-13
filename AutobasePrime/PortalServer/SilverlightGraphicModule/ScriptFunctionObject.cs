using System;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionObject
    {

        public ScriptFunctionObject()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_ObjectSetIntPublic(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int intval;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out intval)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = GraphicTool.ExecuteClassNameOnlyObject(class_name, command, intval);
            }

            return 1;
        }

        static int Function_ObjectSetStringPublic(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string sval;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out sval)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = GraphicTool.ExecuteClassNameOnlyObject(class_name, command, sval);
            }

            return 1;
        }

        public static int Function_Object(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "ObjectSetTextColor")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetBackColor")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetLineColor")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetFillColor")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetLineOption")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetFillOption")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetLineThick")
            {
                return Function_ObjectSetIntPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetToolTipText")
            {
                return Function_ObjectSetStringPublic(scriptClass, command, argument, out val);
            }
            else if (command == "ObjectSetText")
            {
                return Function_ObjectSetStringPublic(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Object 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Object 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Object function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }

        static int Run_ObjectSetRect(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            retn_value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                retn_value = GraphicTool.ExecuteClassNameOnlyObject((string)args[0], method_name, args);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Object";

            prepare.AddMethod(prename, "ObjectSetRect", "void", new ScriptExternalRun.DeleMethod(Run_ObjectSetRect), "in:string:classname", "in:int:x1", "in:int:y1", "in:int:y1", "in:int:x2");
        }
    }
}