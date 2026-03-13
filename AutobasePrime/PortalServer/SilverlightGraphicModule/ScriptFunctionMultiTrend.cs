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
    public class ScriptFunctionMultiTrend
    {
        static int Function_MultiTrendSetStartTime(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int year;
            int mon;
            int day;
            int hour;
            int min;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out mon)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out hour)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out min)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (year < 1 || year > 9999) year = 2000;
                if (mon < 1 || mon > 12) mon = 1;
                if (day < 1 || day > TimeUtil.getmonthlimit(year, mon)) day = 1;
                if (hour < 0 || hour > 23) hour = 0;
                if (min < 0 || min > 59) min = 0;

                scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, "MultiTrendSetStartTime", (int)(double)year, (int)(double)mon, (int)(double)day, (int)(double)hour, (int)(double)min);
            }

            return 1;
        }

        static int Function_MultiTrendSetBackColor(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int color;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out color)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int r = ((int)color >> 16 & 0xFF);
                int g = ((int)color >> 8 & 0xFF);
                int b = ((int)color >> 0 & 0xFF);
                Color colort = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
                scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, "MultiTrendSetBackColor", colort);
            }

            return 1;
        }

        static int Function_MultiTrendOld(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;

            /*
            if (ScriptFunctionElse.arrayMultiRegister.Count == 0) return 1;

            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int time_size;
            int tag_disp_size;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out time_size)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out tag_disp_size)) return -1;


            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ViewAnalogInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputTrendMain((int)time_size, ScriptFunctionElse.arrayMultiRegister, ScriptFunctionSet.colorMultiTrendBack, (int)tag_disp_size), ConfigViewMain.nMdiCountOnBasicScreen);
            }

            ScriptFunctionElse.arrayMultiRegister.Clear();*/

            return 1;
        }

        static int Function_MultiTrendSetBasicLevel(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_MultiTrendGetCursorData(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_MultiTrendClear(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiTrendReLoad(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiTrendAddTag(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string s;
            int f;
            PUBLIC_GRAPH_MEMBER member = new PUBLIC_GRAPH_MEMBER();

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out s)) return -1;
            member.tag = s;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.color = Tools.ConvertColor(f);   //Color.FromArgb(f);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nValueType = (int)f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nPointType = (int)f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nLineThick = f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nAxisPosition = f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nLevelFrom = (int)f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nLevelTo = (int)f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.nTagDisplaySize = (int)f;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out f)) return -1;
            member.bReverseY = (sbyte)f;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, member);
            }

            return 1;
        }

        static int Function_MultiTrendDeleteTag(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string tag;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, tag);
            }

            return 1;
        }

        static int Function_MultiTrendSetDataSize(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int size;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out size)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, size);
            }

            return 1;
        }

        static int Function_MultiTrendGetDataSize(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiTrendGetStartTime(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string syear, smon, sday, shour, smin;
            int year = 0, mon = 0, day = 0, hour = 0, min = 0;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out syear);
            arg.GetArgument(out smon);
            arg.GetArgument(out sday);
            arg.GetArgument(out shour);
            arg.GetArgument(out smin);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DateTime t;
                t = ConvertTool.ToDateTime(scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command));
                year = t.Year;
                mon = t.Month;
                day = t.Day;
                hour = t.Hour;
                min = t.Minute;
            }

            if (!scriptClass.ChangeNumberVar(syear, year, "@MultiTrendGetStartTime() arg2")) return -1;
            if (!scriptClass.ChangeNumberVar(smon, mon, "@MultiTrendGetStartTime() arg3")) return -1;
            if (!scriptClass.ChangeNumberVar(sday, day, "@MultiTrendGetStartTime() arg4")) return -1;
            if (!scriptClass.ChangeNumberVar(shour, hour, "@MultiTrendGetStartTime() arg5")) return -1;
            if (!scriptClass.ChangeNumberVar(smin, min, "@MultiTrendGetStartTime() arg6")) return -1;

            return 1;
        }

        static int Function_MultiTrendSetStartTimeMode(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int mode;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out mode)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, mode);
            }

            return 1;
        }

        static int Function_MultiTrendGetStartTimeMode(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiTrendGetCursorTime(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string syear, smon, sday, shour, smin;
            int year = 0, mon = 0, day = 0, hour = 0, min = 0;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out syear);
            arg.GetArgument(out smon);
            arg.GetArgument(out sday);
            arg.GetArgument(out shour);
            arg.GetArgument(out smin);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DateTime t;
                t = ConvertTool.ToDateTime(scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command));
                year = t.Year;
                mon = t.Month;
                day = t.Day;
                hour = t.Hour;
                min = t.Minute;
            }

            if (!scriptClass.ChangeNumberVar(syear, year, "@MultiTrendGetCursorTime() arg2")) return -1;
            if (!scriptClass.ChangeNumberVar(smon, mon, "@MultiTrendGetCursorTime() arg3")) return -1;
            if (!scriptClass.ChangeNumberVar(sday, day, "@MultiTrendGetCursorTime() arg4")) return -1;
            if (!scriptClass.ChangeNumberVar(shour, hour, "@MultiTrendGetCursorTime() arg5")) return -1;
            if (!scriptClass.ChangeNumberVar(smin, min, "@MultiTrendGetCursorTime() arg6")) return -1;

            return 1;
        }

        static int Function_MultiTrendGetCursorSize(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiTrendGetTimeType(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiTrendSetTimeType(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int type;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out type)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = scriptClass.ExecuteClassName(ObjectMultiTrend.arrayClassList, class_name, command, type);
            }

            return 1;
        }

        public static int Function_MultiTrend(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "MultiTrendSetStartTime")
            {
                return Function_MultiTrendSetStartTime(scriptClass, command, argument, out val);
            }
            else if (command == "MultiTrendSetBackColor")
            {
                return Function_MultiTrendSetBackColor(scriptClass, command, argument, out val);
            }
            else if (command == "MultiTrend")
            {
                return Function_MultiTrendOld(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendSetBasicLevel") == 0)
            {
                return Function_MultiTrendSetBasicLevel(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetCursorData") == 0)
            {
                return Function_MultiTrendGetCursorData(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendClear") == 0)
            {
                return Function_MultiTrendClear(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendAddTag") == 0)
            {
                return Function_MultiTrendAddTag(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendDeleteTag") == 0)
            {
                return Function_MultiTrendDeleteTag(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendReLoad") == 0)
            {
                return Function_MultiTrendReLoad(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendSetDataSize") == 0)
            {
                return Function_MultiTrendSetDataSize(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetDataSize") == 0)
            {
                return Function_MultiTrendGetDataSize(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetStartTime") == 0)
            {
                return Function_MultiTrendGetStartTime(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendSetStartTimeMode") == 0)
            {
                return Function_MultiTrendSetStartTimeMode(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetStartTimeMode") == 0)
            {
                return Function_MultiTrendGetStartTimeMode(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetCursorTime") == 0)
            {
                return Function_MultiTrendGetCursorTime(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetCursorSize") == 0)
            {
                return Function_MultiTrendGetCursorSize(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendGetTimeType") == 0)
            {
                return Function_MultiTrendGetTimeType(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiTrendSetTimeType") == 0)
            {
                return Function_MultiTrendSetTimeType(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 MultiTrend 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 MultiTrend 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined MultiTrend function ({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
