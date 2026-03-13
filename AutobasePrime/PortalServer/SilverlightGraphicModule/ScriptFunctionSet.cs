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
using NetTools;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionSet
    {
        public delegate void CallBackSetVipScan(string tag);
        public static CallBackSetVipScan procSetVipScan = null;

        static int Function_SetVipScan(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            int[] pos = new int[1];
            EnumTagType type = 0;

            TagLib.GetTagTypeAndPos(tag, ref type, ref pos);

            if (type == EnumTagType.AI)
            {

            }
            else if (type == EnumTagType.DI)
            {

            }
            else
            {
                scriptClass.ErrorMessage(String.Format("@SetVipScan()은 DI나 AI Tag를 사용해야 합니다.({0})", tag));
                return -1;
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (scriptClass.bHandOperation) // 수동으로 조작했을 때만 사용할 수 있다.
                {
                    if (procSetVipScan != null)
                        procSetVipScan(tag);
                }
            }

            return 1;
        }

        static int Function_SetTagValue(ScriptClass scriptClass, string command, string argument, out object val)
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
                string tag_name;
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

                arg.GetArgument(out buf);

                if (member_var == EnumTagMemberVar.MEMBER_VAR_string)
                {
                    string value_string;
                    if (!scriptClass.GetArgumentString(buf, out value_string)) return -1;
                    scriptClass.SetTagMemberString(tag_name, tag_type, ref tag_pos, tag_member, value_string);
                }
                else
                {
                    object value_double;
                    if (!scriptClass.GetValueRecurse(buf, out value_double)) return -1;
                    scriptClass.SetTagMemberValue(tag_name, tag_type, ref tag_pos, tag_member, scriptClass.GetValueDouble(value_double));
                }
            }

            return 1;
        }

        static int Function_SetVarValue(ScriptClass scriptClass, string command, string argument, out object val)
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

                arg.GetArgument(out buf);

                if (var.type == EnumVarType.VAR_TYPE_char && var.size > 1)
                {
                    string value_string;
                    if (!scriptClass.GetArgumentString(buf, out value_string)) return -1;
                    scriptClass.ChangeStringVar(tag, value_string);
                }
                else if (var.type == EnumVarType.VAR_TYPE_string)
                {
                    string value_string;
                    if (!scriptClass.GetArgumentString(buf, out value_string)) return -1;
                    scriptClass.ChangeStringVar(tag, value_string);
                }
                else
                {
                    object value_double;
                    if (!scriptClass.GetValueRecurse(buf, buf.Length, out value_double)) return -1;
                    scriptClass.SetVarValue(var_pos, value_double, 0);
                }
            }

            return 1;
        }

        //[NonSerialized]
        public static Color colorMultiTrendBack = Colors.Black;

        static int Function_SetBackColor(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int v;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out v)) return -1;
            colorMultiTrendBack = Tools.ConvertColor(v);

            return 1;
        }

        static int Function_SetActiveMain(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;

            /*
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                IntPtr handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                if (handle != null)
                    Win32Function.SetForegroundWindow(handle);
            }*/

            return 1;
        }

        public static int Function_Set(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "SetActiveMain")
            {
                return Function_SetActiveMain(scriptClass, command, argument, out val);
            }
            else if (command == "SetBackColor")
            {
                return Function_SetBackColor(scriptClass, command, argument, out val);
            }
            else if (command == "SetTagValue")
            {
                return Function_SetTagValue(scriptClass, command, argument, out val);
            }
            else if (command == "SetVarValue")
            {
                return Function_SetVarValue(scriptClass, command, argument, out val);
            }
            else if (command == "SetVipScan")
            {
                return Function_SetVipScan(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Set?? 함수입니다.\n{0}", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Set 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Set??? function.\n{0}", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
