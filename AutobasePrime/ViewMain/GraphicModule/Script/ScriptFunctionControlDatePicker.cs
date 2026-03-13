using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using AutoLibLocal;
using System.Threading.Tasks;

namespace GraphicModule
{
    public class ScriptFunctionControlDatePicker
    {
        /*
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

            DateTime t = DateTimeServer.Now;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectControlDatePicker.arrayClassList, class_name, command);
                if (val != null)
                {
                    try
                    {
                        t = (DateTime)val;
                    }
                    catch
                    {
                        // 클래스를 찾을 수 없으면 0이 반납되어서 오류가 난다. 2012-5-24
                        t = new DateTime(1980, 1, 1);
                    }
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
        }*/

        static async Task<(int, object val)> Run_ObjectPublic(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object retn_value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                retn_value = await scriptClass.ExecuteClassName(ObjectControlDatePicker.arrayClassList, (string)args[0], method_name, args);
            }

            return (1, retn_value);
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "DatePicker";

            prepare.AddMethod(prename, "DatePickerGetDate", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "out:int:year", "out:int:month", "out:int:day");
            prepare.AddMethod(prename, "DatePickerSetDate", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:int:year", "in:int:month", "in:int:day");
        }
    }
}
