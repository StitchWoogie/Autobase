using System;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionStringAnsi
    {
        public ScriptFunctionStringAnsi()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static string OldFormatToNewFormat(string old_format, char key)
        {
            if (old_format.Length == 0)
            {
                if (key == 'x' || key == 'X')
                {
                    return "{0:" + key + '}';
                }
                return "{0}";
            }

            string new_format = "{0}";

            string high = "";
            string low = "";
            bool flag_dot = false;

            for (int i = 0; i < old_format.Length; i++)
            {
                if (flag_dot)
                {
                    low += old_format[i];
                }
                else
                {
                    if (old_format[i] == '.')
                    {
                        flag_dot = true;
                    }
                    else
                    {
                        high += old_format[i];
                    }
                }
            }

            if (high.Length > 0)		// {n,m:f}의 형식이된다.
            {
                new_format = "{0," + ConvertTool.ToInt32(high).ToString() + ":";
            }
            else					// {n:f}의 형식이 된다.
            {
                new_format = "{0:";
            }

            if (key == 's')
                new_format += 'G';
            else if (key == 'u')		// u는 없음 
                new_format += 'd';
            else
                new_format += key;

            new_format += ConvertTool.ToInt32(low).ToString();	// 소수점이하가 있으면 표시
            new_format += "}";

            // {n[,m][:f]}  n - arg,number
            //				m = size 
            //						-10, +10
            //				f = format  
            //					F0 = 소수점 이하 없는 형식
            //					F10 = 소수점 이하 10자리

            return new_format;
        }

        static bool FillFormatString(ScriptClass scriptClass, out string target, int limit, string format, ScriptArgumentString arg)
        {
            target = "";

            bool flag_percent = false;

            int i;
            string imsi = "";
            double val;
            string buf;
            string real;
            string temp;
            string temp_format;

            target = "";		// "%s"만을 사용할 때 인자값이 NULL이면 그냥 리턴하므로 이전의 쓰레기가 남게 된다.

            for (i = 0; i < format.Length; i++)
            {
                if (flag_percent)
                {
                    if (format[i] == '%')
                    {
                        flag_percent = false;
                        target += '%';
                        continue;
                    }

                    if (format[i] == 'd' || format[i] == 'u' || format[i] == 'X' ||
                        format[i] == 'x')
                    {
                        arg.GetArgument(out buf);
                        if (!scriptClass.GetValueRecurse(buf, buf.Length, out val)) return false;

                        temp_format = OldFormatToNewFormat(imsi, format[i]);

                        temp = String.Format(temp_format, (long)val);

                        if (imsi.Length > 0 && imsi[0] == '0')
                            temp = temp.Replace(' ', '0');

                        target += temp;
                        flag_percent = false;
                    }
                    else if (format[i] == 'f' || format[i] == 'e')
                    {
                        arg.GetArgument(out buf);
                        if (!scriptClass.GetValueRecurse(buf, buf.Length, out val)) return false;

                        temp_format = OldFormatToNewFormat(imsi, format[i]);

                        temp = String.Format(temp_format, val);

                        if (imsi.Length > 0 && imsi[0] == '0')
                            temp = temp.Replace(' ', '0');

                        target += temp;
                        flag_percent = false;
                    }
                    else if (format[i] == 's')
                    {
                        arg.GetArgument(out buf);

                        if (buf.Length == 0)
                        {
                            if (Tools.IsLangKorean())
                                scriptClass.ErrorMessage("%s로 사용할 문자열이 지정되지 않았습니다.\n%s의 개수가 인자보다 많을 수 있음");
                            else
                                scriptClass.ErrorMessage("String argument not defined to use %s format.");

                            return false;
                        }
                        if (!scriptClass.GetArgumentString(buf, out real)) return false;

                        temp_format = OldFormatToNewFormat(imsi, format[i]);

                        temp = String.Format(temp_format, real);
                        target += temp;
                        flag_percent = false;
                    }
                    else if (format[i] == 'c' || format[i] == 'C')
                    {
                        arg.GetArgument(out buf);
                        if (!scriptClass.GetValueRecurse(buf, buf.Length, out val)) return false;

                        if (format[i] == 'c')
                        {
                            char ex = (char)val;
                            target += ex;
                        }
                        else
                        {
                            char ex = (char)val;
                            string s = ex.ToString();
                            target += s.ToUpper();
                        }

                        flag_percent = false;
                    }
                    else
                    {
                        imsi += format[i];
                    }
                    continue;
                }

                if (format[i] == '%')
                {
                    flag_percent = true;
                    imsi = "";
                }
                else
                {
                    target += format[i];
                }
            }

            return true;
        }

        static int Function_sprintf(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string var;
            string buf;
            string format;
            string maked;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out var);
            if (!scriptClass.IsStringVar(var, "@sprintf() arg1")) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out format)) return -1;

            if (!FillFormatString(scriptClass, out maked, 1000, format, arg)) return -1;

            if (!scriptClass.ChangeStringVar(var, maked)) return -1;

            return 1;
        }

        static int Function_atof(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string format;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out format)) return -1;

            if (format.Length == 0)
                val = 0;
            else
            {
                try
                {
                    val = ConvertTool.ToDouble(format);
                }
                catch
                {
                    val = 0;
                }
            }

            return 1;
        }

        static int Function_atoi(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string format;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out format)) return -1;

            if (format.Length == 0)
                val = 0;
            else
            {
                try
                {
                    val = ConvertTool.ToInt32(format);
                }
                catch
                {
                    val = 0;
                }
            }

            return 1;
        }

        static int Function_atox(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string format;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out format)) return -1;

            CommaBlockString comma = new CommaBlockString();
            uint dword = 0;
            comma.Set(format);
            comma.GetHexDWORD(ref dword);

            val = dword;

            return 1;
        }

        static int Function_strlen(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string data;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out data)) return -1;

            val = data.Length;

            return 1;
        }

        static int Function_strcmp(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string comp1;
            string comp2;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp2)) return -1;

            val = String.Compare(comp1, comp2);

            return 1;
        }

        static int Function_strncmp(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string comp1;
            string comp2;
            double count;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp2)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out count)) return -1;

            val = String.Compare(comp1, 0, comp2, 0, (int)count);

            return 1;
        }

        static int Function_stricmp(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string comp1;
            string comp2;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp2)) return -1;

            val = String.Compare(comp1, comp2, StringComparison.CurrentCultureIgnoreCase);

            return 1;
        }

        static int Function_strnicmp(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string comp1;
            string comp2;
            double count;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp2)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out count)) return -1;

            val = String.Compare(comp1, 0, comp2, 0, (int)count, StringComparison.CurrentCultureIgnoreCase);

            return 1;
        }

        static int Function_strcat(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string var;
            string comp2;
            string val1;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out var);
            if (!scriptClass.IsStringVar(var, "@strcat() arg1")) return -1;
            if (!scriptClass.GetArgumentString(var, out val1)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp2)) return -1;

            val1 += comp2;

            if (!scriptClass.ChangeStringVar(var, val1)) return -1;

            val = 1;

            return 1;
        }

        static int Function_strcpy(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string var;
            string comp2;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out var);
            if (!scriptClass.IsStringVar(var, "@strcat() arg1")) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out comp2)) return -1;

            if (!scriptClass.ChangeStringVar(var, comp2)) return -1;

            val = 1;

            return 1;
        }

        public static int Function_StringAnsi(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "sprintf")
            {
                return Function_sprintf(scriptClass, command, argument, out val);
            }
            else if (command == "atof")
            {
                return Function_atof(scriptClass, command, argument, out val);
            }
            else if (command == "atoi")
            {
                return Function_atoi(scriptClass, command, argument, out val);
            }
            else if (command == "atox")
            {
                return Function_atox(scriptClass, command, argument, out val);
            }
            else if (command == "strlen")
            {
                return Function_strlen(scriptClass, command, argument, out val);
            }
            else if (command == "strcmp")
            {
                return Function_strcmp(scriptClass, command, argument, out val);
            }
            else if (command == "strncmp")
            {
                return Function_strncmp(scriptClass, command, argument, out val);
            }
            else if (command == "stricmp")
            {
                return Function_stricmp(scriptClass, command, argument, out val);
            }
            else if (command == "strnicmp")
            {
                return Function_strnicmp(scriptClass, command, argument, out val);
            }
            else if (command == "strcat")
            {
                return Function_strcat(scriptClass, command, argument, out val);
            }
            else if (command == "strcpy")
            {
                return Function_strcpy(scriptClass, command, argument, out val);
            }
            else { }

            val = 0;

            return 0;
        }
    }
}