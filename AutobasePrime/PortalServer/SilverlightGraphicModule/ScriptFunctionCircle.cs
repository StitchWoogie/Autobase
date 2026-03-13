using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionCircle
    {
        static int Function_CircleSetType(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int type;

            val = "";	// 문자열을 반환해야 하므로 문자열로 초기화 한다.

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out type)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectCircle.arrayClassList, class_name, command, type);
            }

            return 1;
        }

        static int Function_CircleSetAngle(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            float startangle, sweepangle;

            val = "";	// 문자열을 반환해야 하므로 문자열로 초기화 한다.

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out startangle)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out sweepangle)) return -1;

            sweepangle %= 360;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectCircle.arrayClassList, class_name, command, startangle, sweepangle);
            }

            return 1;
        }

        public static int Function_Circle(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "CircleSetType")
            {
                return Function_CircleSetType(scriptClass, command, argument, out val);
            }
            else if (command == "CircleSetAngle")
            {
                return Function_CircleSetAngle(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Circle 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Circle 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Circle function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
