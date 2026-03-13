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
    public class ScriptFunctionDbTrend
    {
        static int Function_DbTrendSetStartTime(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            double year;
            double mon;
            double day;
            double hour;
            double min;
            double sec;

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
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out sec)) return -1;

            if (year < 1) year = 1;
            if (year > 9999) year = 9999;
            if (mon < 1) mon = 1;
            if (mon > 12) mon = 12;
            if (day < 1) day = 1;
            if (day > 31) day = 31;
            if (hour < 0) hour = 0;
            if (hour > 23) hour = 23;
            if (min < 0) min = 0;
            if (min > 59) min = 59;
            if (sec < 0) sec = 0;
            if (sec > 59) sec = 59;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, (int)year, (int)mon, (int)day, (int)hour, (int)min, (int)sec);
            }

            return 1;
        }

        static int Function_DbTrendReLoad(ScriptClass scriptClass, string command, string argument, out object val)
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
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_DbTrendSetShowSize(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int size;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out size)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, size);
            }

            return 1;
        }

        static int Function_DbTrendGetShowSize(ScriptClass scriptClass, string command, string argument, out object val)
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
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_DbTrendSetDataType(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int type;
            int size;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out type)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out size)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, type, size);
            }

            return 1;
        }

        static int Function_DbTrendSetDsn(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string dsn;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out dsn)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, dsn);
            }

            return 1;
        }

        static int Function_DbTrendSetTable(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string table;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out table)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, table);
            }

            return 1;
        }

        static int Function_DbTrendShiftTime(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int shift;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out shift)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, shift);
            }

            return 1;
        }

        static int Function_DbTrendGetMemberFlags(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_DbTrendSetMemberFlags(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int flags;
            int pos;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out flags)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos, flags);
            }

            return 1;
        }

        static int Function_DbTrendSetMemberTable(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;
            string table;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out table)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos, table);
            }

            return 1;
        }

        static int Function_DbTrendClear(ScriptClass scriptClass, string command, string argument, out object val)
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
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_DbTrendAddMember(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            DB_TREND_MEMBER member = new DB_TREND_MEMBER();

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out member.tag)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out member.column)) return -1;

            arg.GetArgument(out buf);
            int color;

            if (!scriptClass.GetValueRecurse(buf, out color)) return -1;

            member.color = Tools.ConvertColor(color);   //Color.FromArgb(f);
            /*
            int a = (color >> 24 & 0xFF);
            int r = (color >> 16 & 0xFF);
            int g = (color >> 8 & 0xFF);
            int b = (color >> 0 & 0xFF);
            member.color = System.Drawing.Color.FromArgb(a, r, g, b);*/

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nValueType)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nPointType)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nLineThick)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nAxisPosition)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nLevelFrom)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nLevelTo)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nTagDisplaySize)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.bReverseY)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.wFlags)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out member.nGraphType)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out member.sTable)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out member.sWhereString)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out member.sDescription)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, member);
            }

            return 1;
        }

        static int Function_DbTrendRemoveAt(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos);
            }

            return 1;
        }


        static int Function_DbTrendSaveToCsv(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string filename;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, filename);
            }

            return 1;
        }

        static int Function_DbTrendGetRealPos(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int left_right;
            int pos;
            int level;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out left_right)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out level)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, left_right, pos, level);
            }

            return 1;
        }

        static int Function_DbTrendGetMax(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_DbTrendGetMin(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_DbTrendSetMax(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;
            double move;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out move)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos, move);
            }

            return 1;
        }

        static int Function_DbTrendSetMin(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            int pos;
            double move;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out pos)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out move)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabaseTrend.arrayClassList, class_name, command, pos, move);
            }

            return 1;
        }

        public static int Function_DbTrend(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "DbTrendSetStartTime")
            {
                return Function_DbTrendSetStartTime(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendReLoad")
            {
                return Function_DbTrendReLoad(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetShowSize")
            {
                return Function_DbTrendSetShowSize(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendGetShowSize")
            {
                return Function_DbTrendGetShowSize(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetDataType")
            {
                return Function_DbTrendSetDataType(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetDsn")
            {
                return Function_DbTrendSetDsn(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetTable")
            {
                return Function_DbTrendSetTable(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendShiftTime")
            {
                return Function_DbTrendShiftTime(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendGetMemberFlags")
            {
                return Function_DbTrendGetMemberFlags(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetMemberFlags")
            {
                return Function_DbTrendSetMemberFlags(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetMemberTable")
            {
                return Function_DbTrendSetMemberTable(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendClear")
            {
                return Function_DbTrendClear(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendAddMember")
            {
                return Function_DbTrendAddMember(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendRemoveAt")
            {
                return Function_DbTrendRemoveAt(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSaveToCsv")
            {
                return Function_DbTrendSaveToCsv(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendGetRealPos")
            {
                return Function_DbTrendGetRealPos(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendGetMax")
            {
                return Function_DbTrendGetMax(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendGetMin")
            {
                return Function_DbTrendGetMin(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetMax")
            {
                return Function_DbTrendSetMax(scriptClass, command, argument, out val);
            }
            else if (command == "DbTrendSetMin")
            {
                return Function_DbTrendSetMin(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 DbTrend 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 DbTrend 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined DbTrend function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
