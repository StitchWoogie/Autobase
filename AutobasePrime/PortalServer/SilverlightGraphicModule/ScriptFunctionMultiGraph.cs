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
    public class ScriptFunctionMultiGraph
    {
        static int Function_MultiGraphSetBackColor(ScriptClass scriptClass, string command, string argument, out object val)
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
                scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, "MultiGraphSetBackColor", colort);
            }

            return 1;
        }

        static int Function_MultiGraphSetBasicLevel(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_MultiGraphGetCursorData(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command, pos);
            }

            return 1;
        }

        static int Function_MultiGraphClear(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command);
            }

            return 1;
        }

        static int Function_MultiGraphAddTag(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command, member);
            }

            return 1;
        }

        static int Function_MultiGraphDeleteTag(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command, tag);
            }

            return 1;
        }

        static int Function_MultiGraphSetDataSize(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command, size);
            }

            return 1;
        }

        static int Function_MultiGraphGetDataSize(ScriptClass scriptClass, string command, string argument, out object value)
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
                value = scriptClass.ExecuteClassName(ObjectMultiGraph.arrayClassList, class_name, command);
            }

            return 1;
        }

        public static int Function_MultiGraph(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "MultiGraphSetBackColor")
            {
                return Function_MultiGraphSetBackColor(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphSetBasicLevel") == 0)
            {
                return Function_MultiGraphSetBasicLevel(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphGetCursorData") == 0)
            {
                return Function_MultiGraphGetCursorData(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphClear") == 0)
            {
                return Function_MultiGraphClear(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphAddTag") == 0)
            {
                return Function_MultiGraphAddTag(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphDeleteTag") == 0)
            {
                return Function_MultiGraphDeleteTag(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphSetDataSize") == 0)
            {
                return Function_MultiGraphSetDataSize(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "MultiGraphGetDataSize") == 0)
            {
                return Function_MultiGraphGetDataSize(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 MultiGraph 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 MultiGraph 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined MultiGraph function ({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
