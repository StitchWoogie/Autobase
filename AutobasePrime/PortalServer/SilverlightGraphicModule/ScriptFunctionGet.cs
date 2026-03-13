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
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionGet
    {
        public ScriptFunctionGet()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_GetTagValue(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag_full;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag_full)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                EnumTagType tag_type;
                int[] tag_pos = new int[1];
                EnumTagMember tag_member;
                EnumTagMemberVar member_var;
                string var;
                string tag_name;	// 
                TagPublicClass tp;

                if (!TagLib.GetTagTypePosMember(tag_full, out tag_name, out tag_type, ref tag_pos, out tag_member, out member_var, out tp))
                {
                    if (Tools.IsLangKorean())
                    {
                        scriptClass.ErrorMessage(String.Format("존재하지 않는 태그(${0})", tag_full));
                    }
                    else
                    {
                        scriptClass.ErrorMessage(String.Format("(${0}) tag not found", tag_full));
                    }
                    return -1;		// 대입문이 아니다.
                }

                if (member_var == EnumTagMemberVar.MEMBER_VAR_string)
                {
                    arg.GetArgument(out var);

                    // 9.5.2 부터 추가
                    if (var.Length == 0)    // 인자가 없다.
                    {
                        val = scriptClass.GetTagMemberValue(tag_name, tag_type, tp, tag_member);
                        return 1;
                    }

                    if (!scriptClass.IsStringVar(var, "GetTagValue() arg1")) return -1;

                    string value_string = "";

                    value_string = scriptClass.GetValueString(scriptClass.GetTagMemberValue(tag_name, tag_type, tp, tag_member));
                    if (!scriptClass.ChangeStringVar(var, value_string)) return -1;
                }
                else
                {
                    val = scriptClass.GetTagMemberValue(tag_name, tag_type, tp, tag_member);
                }
            }

            return 1;
        }

        static int Function_GetVarValue(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                VAR_STRUCT var;
                int var_pos;

                for (var_pos = 0; var_pos < scriptClass.arrayVar.Count; var_pos++)
                {
                    var = (VAR_STRUCT)scriptClass.arrayVar[var_pos];
                    if (var.name == tag)
                    {	// 선언된 변수중에 있다.
                        goto ok_i_seek;
                    }
                }
                scriptClass.ErrorMessage(String.Format("{0} var is not found.", tag));
                return -1;

            ok_i_seek:

                if (var.type == EnumVarType.VAR_TYPE_char && var.size > 1)
                {
                    string string_var;
                    arg.GetArgument(out string_var);
                    if (!scriptClass.IsStringVar(string_var, "GetVarValue() arg2")) return -1;

                    string val_val = "";

                    char[] vp = (char[])var.val;

                    for (int i = 0; i < vp.Length; i++)
                    {
                        if (vp[i] == 0) break;
                        val_val += vp[i];
                    }

                    if (!scriptClass.ChangeStringVar(string_var, val_val)) return -1;
                }
                else if (var.type == EnumVarType.VAR_TYPE_string)
                {
                    string string_var;
                    arg.GetArgument(out string_var);
                    if (!scriptClass.IsStringVar(string_var, "GetVarValue() arg2")) return -1;

                    string val_val = ((string[])var.val)[0];

                    if (!scriptClass.ChangeStringVar(string_var, val_val)) return -1;
                }
                else
                {
                    if (!scriptClass.GetVarValue(tag, tag.Length, out val, 0, false)) return -1;
                }
            }

            return 1;
        }

        static int Function_GetDayCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int year;
            int month;
            int day;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;

            val = TimeUtil.GetDayHap(year, month, day);

            return 1;
        }

        static int Function_GetMinCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year;
            double month;
            double day;
            double hour;
            double min;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out day)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out hour)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out min)) return -1;

            val = TimeUtil.GetMinHap((int)year, (int)month, (int)day, (int)hour, (int)min);

            return 1;
        }

        static int Function_GetHourCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year;
            double month;
            double day;
            double hour;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out day)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out hour)) return -1;

            val = TimeUtil.GetHourHap((int)year, (int)month, (int)day, (int)hour);

            return 1;
        }

        static int Function_GetLastDay(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year;
            double month;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;

            val = TimeUtil.getmonthlimit((int)year, (int)month);

            return 1;
        }

        static int Function_GetTimeHour(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Hour;

            return 1;
        }

        static int Function_GetTimeMin(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Minute;

            return 1;
        }

        static int Function_GetTimeSec(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Second;

            return 1;
        }

        static int Function_GetDateYear(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Year;

            return 1;
        }

        static int Function_GetDateMon(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Month;

            return 1;
        }

        static int Function_GetDateDay(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Day;

            return 1;
        }

        static int Function_GetWeekDay(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year, month, day;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out day)) return -1;

            val = TimeUtil.GetWeekDay((int)year, (int)month, (int)day);

            return 1;
        }

        static int Function_GetUserRight(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int value_right;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_right)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                //value = SharedData.userInfo.IsHaveRight((EnumUserRights)value_right);	// 각종 권한을 가지고 있는가 알아본다.
            }

            return 1;
        }

        static int Function_GetUserRightControl(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            value = SharedData.userInfo.IsHaveTagRight(tag) ? 1 : 0;

            return 1;
        }

        static int Function_GetUserName(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string var;
            string username = "";

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out var);
            if (!scriptClass.IsStringVar(var, "@GetUserName() arg1")) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                username = SharedData.userInfo.sUsername;
            }

            if (!scriptClass.ChangeStringVar(var, username)) return -1;

            return 1;
        }

        public static int Function_Get(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "GetTagValue")
            {
                return Function_GetTagValue(scriptClass, command, argument, out val);
            }
            else if (command == "GetVarValue")
            {
                return Function_GetVarValue(scriptClass, command, argument, out val);
            }
            else if (command == "GetDayCount")
            {
                return Function_GetDayCount(scriptClass, command, argument, out val);
            }
            else if (command == "GetMinCount")
            {
                return Function_GetMinCount(scriptClass, command, argument, out val);
            }
            else if (command == "GetHourCount")
            {
                return Function_GetHourCount(scriptClass, command, argument, out val);
            }
            else if (command == "GetLastDay")
            {
                return Function_GetLastDay(scriptClass, command, argument, out val);
            }
            else if (command == "GetTimeHour")
            {
                return Function_GetTimeHour(scriptClass, command, argument, out val);
            }
            else if (command == "GetTimeMin")
            {
                return Function_GetTimeMin(scriptClass, command, argument, out val);
            }
            else if (command == "GetTimeSec")
            {
                return Function_GetTimeSec(scriptClass, command, argument, out val);
            }
            else if (command == "GetDateYear")
            {
                return Function_GetDateYear(scriptClass, command, argument, out val);
            }
            else if (command == "GetDateMon")
            {
                return Function_GetDateMon(scriptClass, command, argument, out val);
            }
            else if (command == "GetDateDay")
            {
                return Function_GetDateDay(scriptClass, command, argument, out val);
            }
            else if (command == "GetWeekDay")
            {
                return Function_GetWeekDay(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "GetUserRight") == 0)
            {
                return Function_GetUserRight(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "GetUserRightControl") == 0)
            {
                return Function_GetUserRightControl(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "GetUserName") == 0)
            {
                return Function_GetUserName(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Get?? 함수입니다.\n{0}", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Get 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Get??? function.\n{0}", command));
                }
                val = 0;
                return -1;
            }
        }

        static int Run_GetLastError(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            retn_value = 0;
            //retn_value = scriptClass.GetLastError();

            return 1;
        }

        static int Run_GetPercent(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            double curr = (double)args[0],
                    value_base = (double)args[1],
                    value_full = (double)args[2],
                    fBase = (double)args[3],
                    fFull = (double)args[4];

            retn_value = 0;

            if ((value_full - value_base) == 0)
            {
                retn_value = fBase;
            }
            else
            {
                retn_value = (((curr - value_base) * (fFull - fBase)) / (value_full - value_base)) + fBase;
            }

            return 1;
        }

        static long GetDiskFreeSpace(string directory)
        {
            /*
            long size = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string root = String.Format("win32_logicaldisk.deviceid=\"{0}:\"", directory[0]);

                try
                {
                    System.Management.ManagementObject disk = new System.Management.ManagementObject(root);
                    disk.Get();
                    size = ConvertTool.ToInt64(disk["FreeSpace"].ToString());
                }
                catch
                {
                    size = 0;
                }

                size = size / 1000000;	// Mega Byte로 변환한다.
            }

            return size;*/

            return 0;
        }

        static int Run_GetDataFreeSpace(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            retn_value = 0;

            /*
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string directory;

                directory = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

                retn_value = GetDiskFreeSpace(directory);
            }*/

            return 1;
        }

        static int Run_GetDiskFreeSpace(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            string directory = (string)args[0];

            retn_value = GetDiskFreeSpace(directory);

            return 1;
        }

        static int Run_GetSecCount(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            /*
            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int minute = (int)args[4];
            int second = (int)args[5];

            val = TimeUtil.GetSecHap(year, month, day, hour, minute, second);*/

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Get";

            prepare.AddMethod(prename, "GetDataFreeSpace", "long", new ScriptExternalRun.DeleMethod(Run_GetDataFreeSpace));
            prepare.AddMethod(prename, "GetDiskFreeSpace", "long", new ScriptExternalRun.DeleMethod(Run_GetDiskFreeSpace), "in:string:directory");

            prepare.AddMethod(prename, "GetLastError", "string", new ScriptExternalRun.DeleMethod(Run_GetLastError));
            prepare.AddMethod(prename, "GetPercent", "double", new ScriptExternalRun.DeleMethod(Run_GetPercent), "in:double:value", "in:double:value_base", "in:double:value_full", "in:double:Base", "in:double:Full");

            prepare.AddMethod(prename, "GetSecCount", "", new ScriptExternalRun.DeleMethod(Run_GetSecCount), "in:int:year", "in:int:month", "in:int:day", "in:int:hour", "in:int:minute", "in:int:second");
        }
    }
}

/*
namespace SilverlightGraphicModule
{
    public class ScriptFunctionGet
    {
        public ScriptFunctionGet()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_GetTagValue(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag_full;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag_full)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                EnumTagType tag_type;
                int[] tag_pos = new int[1];
                EnumTagMember tag_member;
                EnumTagMemberVar member_var;
                string var;
                string tag_name;	// 
                TagPublicClass tp;

                if (!TagLib.GetTagTypePosMember(tag_full, out tag_name, out tag_type, ref tag_pos, out tag_member, out member_var, out tp))
                {
                    if (Tools.IsLangKorean())
                    {
                        scriptClass.ErrorMessage(String.Format("존재하지 않는 태그(${0})", tag_full));
                    }
                    else
                    {
                        scriptClass.ErrorMessage(String.Format("(${0}) tag not found", tag_full));
                    }
                    return -1;		// 대입문이 아니다.
                }

                if (member_var == EnumTagMemberVar.MEMBER_VAR_string)
                {
                    arg.GetArgument(out var);

                    // 9.5.2 부터 추가
                    if (var.Length == 0)    // 인자가 없다.
                    {
                        val = scriptClass.GetTagMemberValue(tag_name, tag_type, tp, tag_member);
                        return 1;
                    }

                    if (!scriptClass.IsStringVar(var, "GetTagValue() arg1")) return -1;

                    string value_string = "";

                    value_string = scriptClass.GetValueString(scriptClass.GetTagMemberValue(tag_name, tag_type, tp, tag_member));
                    if (!scriptClass.ChangeStringVar(var, value_string)) return -1;
                }
                else
                {
                    val = scriptClass.GetTagMemberValue(tag_name, tag_type, tp, tag_member);
                }
            }

            return 1;
        }

        static int Function_GetVarValue(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                VAR_STRUCT var;
                int var_pos;

                for (var_pos = 0; var_pos < scriptClass.arrayVar.Count; var_pos++)
                {
                    var = (VAR_STRUCT)scriptClass.arrayVar[var_pos];
                    if (var.name == tag)
                    {	// 선언된 변수중에 있다.
                        goto ok_i_seek;
                    }
                }
                scriptClass.ErrorMessage(String.Format("{0} var is not found.", tag));
                return -1;

            ok_i_seek:

                if (var.type == EnumVarType.VAR_TYPE_char && var.size > 1)
                {
                    string string_var;
                    arg.GetArgument(out string_var);
                    if (!scriptClass.IsStringVar(string_var, "GetVarValue() arg2")) return -1;

                    string val_val = "";

                    char[] vp = (char[])var.val;

                    for (int i = 0; i < vp.Length; i++)
                    {
                        if (vp[i] == 0) break;
                        val_val += vp[i];
                    }

                    if (!scriptClass.ChangeStringVar(string_var, val_val)) return -1;
                }
                else if (var.type == EnumVarType.VAR_TYPE_string)
                {
                    string string_var;
                    arg.GetArgument(out string_var);
                    if (!scriptClass.IsStringVar(string_var, "GetVarValue() arg2")) return -1;

                    string val_val = ((string[])var.val)[0];

                    if (!scriptClass.ChangeStringVar(string_var, val_val)) return -1;
                }
                else
                {
                    if (!scriptClass.GetVarValue(tag, tag.Length, out val, 0, false)) return -1;
                }
            }

            return 1;
        }

        static int Function_GetDayCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int year;
            int month;
            int day;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;

            val = TimeUtil.GetDayHap(year, month, day);

            return 1;
        }

        static int Function_GetMinCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year;
            double month;
            double day;
            double hour;
            double min;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out day)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out hour)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out min)) return -1;

            val = TimeUtil.GetMinHap((int)year, (int)month, (int)day, (int)hour, (int)min);

            return 1;
        }

        static int Function_GetHourCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year;
            double month;
            double day;
            double hour;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out day)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out hour)) return -1;

            val = TimeUtil.GetHourHap((int)year, (int)month, (int)day, (int)hour);

            return 1;
        }

        static int Function_GetLastDay(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year;
            double month;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;

            val = TimeUtil.getmonthlimit((int)year, (int)month);

            return 1;
        }

        static int Function_GetTimeHour(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Hour;

            return 1;
        }

        static int Function_GetTimeMin(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Minute;

            return 1;
        }

        static int Function_GetTimeSec(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Second;

            return 1;
        }

        static int Function_GetDateYear(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Year;

            return 1;
        }

        static int Function_GetDateMon(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Month;

            return 1;
        }

        static int Function_GetDateDay(ScriptClass scriptClass, string command, string argument, out object val)
        {
            DateTime dt = DateTime.Now;
            val = dt.Day;

            return 1;
        }

        static int Function_GetWeekDay(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double year, month, day;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out day)) return -1;

            val = TimeUtil.GetWeekDay((int)year, (int)month, (int)day);

            return 1;
        }

        static int Function_GetUserRight(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int value_right;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_right)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                value = SharedData.userInfo.IsHaveRight((EnumUserRights)value_right);	// 각종 권한을 가지고 있는가 알아본다.
            }

            return 1;
        }

        static int Function_GetUserRightControl(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            value = SharedData.userInfo.IsHaveTagRight(tag) ? 1 : 0;

            return 1;
        }

        static int Function_GetUserName(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string var;
            string username = "";

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out var);
            if (!scriptClass.IsStringVar(var, "@GetUserName() arg1")) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                username = SharedData.userInfo.sUsername;
            }

            if (!scriptClass.ChangeStringVar(var, username)) return -1;

            return 1;
        }

        public static int Function_Get(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "GetTagValue")
            {
                return Function_GetTagValue(scriptClass, command, argument, out val);
            }
            else if (command == "GetVarValue")
            {
                return Function_GetVarValue(scriptClass, command, argument, out val);
            }
            else if (command == "GetDayCount")
            {
                return Function_GetDayCount(scriptClass, command, argument, out val);
            }
            else if (command == "GetMinCount")
            {
                return Function_GetMinCount(scriptClass, command, argument, out val);
            }
            else if (command == "GetHourCount")
            {
                return Function_GetHourCount(scriptClass, command, argument, out val);
            }
            else if (command == "GetLastDay")
            {
                return Function_GetLastDay(scriptClass, command, argument, out val);
            }
            else if (command == "GetTimeHour")
            {
                return Function_GetTimeHour(scriptClass, command, argument, out val);
            }
            else if (command == "GetTimeMin")
            {
                return Function_GetTimeMin(scriptClass, command, argument, out val);
            }
            else if (command == "GetTimeSec")
            {
                return Function_GetTimeSec(scriptClass, command, argument, out val);
            }
            else if (command == "GetDateYear")
            {
                return Function_GetDateYear(scriptClass, command, argument, out val);
            }
            else if (command == "GetDateMon")
            {
                return Function_GetDateMon(scriptClass, command, argument, out val);
            }
            else if (command == "GetDateDay")
            {
                return Function_GetDateDay(scriptClass, command, argument, out val);
            }
            else if (command == "GetWeekDay")
            {
                return Function_GetWeekDay(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "GetUserRight") == 0)
            {
                return Function_GetUserRight(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "GetUserRightControl") == 0)
            {
                return Function_GetUserRightControl(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "GetUserName") == 0)
            {
                return Function_GetUserName(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Get?? 함수입니다.\n{0}", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Get 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Get??? function.\n{0}", command));
                }
                val = 0;
                return -1;
            }
        }

        static int Run_GetLastError(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            retn_value = scriptClass.GetLastError();

            return 1;
        }

        static int Run_GetPercent(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            double curr = (double)args[0],
                    value_base = (double)args[1],
                    value_full = (double)args[2],
                    fBase = (double)args[3],
                    fFull = (double)args[4];

            retn_value = 0;

            if ((value_full - value_base) == 0)
            {
                retn_value = fBase;
            }
            else
            {
                retn_value = (((curr - value_base) * (fFull - fBase)) / (value_full - value_base)) + fBase;
            }

            return 1;
        }

        static long GetDiskFreeSpace(string directory)
        {
            long size = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string root = String.Format("win32_logicaldisk.deviceid=\"{0}:\"", directory[0]);

                try
                {
                    System.Management.ManagementObject disk = new System.Management.ManagementObject(root);
                    disk.Get();
                    size = ConvertTool.ToInt64(disk["FreeSpace"].ToString());
                }
                catch
                {
                    size = 0;
                }

                size = size / 1000000;	// Mega Byte로 변환한다.
            }

            return size;
        }

        static int Run_GetDataFreeSpace(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            retn_value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string directory;

                directory = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

                retn_value = GetDiskFreeSpace(directory);
            }

            return 1;
        }

        static int Run_GetDiskFreeSpace(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            string directory = (string)args[0];

            retn_value = GetDiskFreeSpace(directory);

            return 1;
        }

        static int Run_GetSecCount(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int minute = (int)args[4];
            int second = (int)args[5];

            val = TimeUtil.GetSecHap(year, month, day, hour, minute, second);

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Get";

            prepare.AddMethod(prename, "GetDataFreeSpace", "long", new ScriptExternalRun.DeleMethod(Run_GetDataFreeSpace));
            prepare.AddMethod(prename, "GetDiskFreeSpace", "long", new ScriptExternalRun.DeleMethod(Run_GetDiskFreeSpace), "in:string:directory");

            prepare.AddMethod(prename, "GetLastError", "string", new ScriptExternalRun.DeleMethod(Run_GetLastError));
            prepare.AddMethod(prename, "GetPercent", "double", new ScriptExternalRun.DeleMethod(Run_GetPercent), "in:double:value", "in:double:value_base", "in:double:value_full", "in:double:Base", "in:double:Full");

            prepare.AddMethod(prename, "GetSecCount", "", new ScriptExternalRun.DeleMethod(Run_GetSecCount), "in:int:year", "in:int:month", "in:int:day", "in:int:hour", "in:int:minute", "in:int:second");
        }
    }
}
*/