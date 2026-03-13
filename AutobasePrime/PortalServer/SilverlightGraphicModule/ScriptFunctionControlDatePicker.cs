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
    public class ScriptFunctionControlDatePicker
    {
        static int Function_DatePickerGetDate(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string syear, smon, sday;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out syear);
            arg.GetArgument(out smon);
            arg.GetArgument(out sday);

            DateTime t = DateTime.Now;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectControlDatePicker.arrayClassList, class_name, command);
                if (val != null)
                {
                    t = (DateTime)val;
                }
            }

            if (!scriptClass.ChangeNumberVar(syear, t.Year, "@DatePickerGetDate() arg2")) return -1;
            if (!scriptClass.ChangeNumberVar(smon, t.Month, "@DatePickerGetDate() arg3")) return -1;
            if (!scriptClass.ChangeNumberVar(sday, t.Day, "@DatePickerGetDate() arg4")) return -1;

            return 1;
        }

        static int Function_DatePickerSetDate(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int year, mon, day;

            val = "";	// 문자열을 반환해야 하므로 문자열로 초기화 한다.

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out mon)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectControlDatePicker.arrayClassList, class_name, command, year, mon, day);
            }

            return 1;
        }

        public static int Function_DatePicker(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "DatePickerGetDate")
            {
                return Function_DatePickerGetDate(scriptClass, command, argument, out val);
            }
            else if (command == "DatePickerSetDate")
            {
                return Function_DatePickerSetDate(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 DatePicker 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 DatePicker 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined DatePicker function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
