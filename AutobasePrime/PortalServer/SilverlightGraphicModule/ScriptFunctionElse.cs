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
using SilverlightDialogControl;
using NetTools;
using AutoLib;
using System.IO;
using System.Windows.Controls.Primitives;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionElse
    {
        public ScriptFunctionElse()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_ControlBox(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            int[] pos = new int[1];

            TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

            if (tp.enumTagType == EnumTagType.AI)
            {

            }
            else if (tp.enumTagType == EnumTagType.DI)
            {

            }
            else
            {
                scriptClass.ErrorMessage(String.Format("@ControlBox()는 DI나 AI Tag를 사용해야 합니다.({0})", tag));
                return -1;
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (scriptClass.bHandOperation)
                {
                    if (tp.enumTagType == EnumTagType.DI)
                    {
                        ControlBoxDigitalInputGo dialog = new ControlBoxDigitalInputGo();

                        dialog.Go(scriptClass.formParent, tag);
                    }
                    else if (tp.enumTagType == EnumTagType.AI)
                    {
                        ControlBoxAnalogInputGo dialog = new ControlBoxAnalogInputGo();

                        dialog.Go(scriptClass.formParent, tag);
                    }
                    else { }
                }
            }

            return 1;
        }

        /*
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
        }*/

        static int Function_MessageBox(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string msg1;
            string msg2;
            int msg_type;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out msg1)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out msg2)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out msg_type)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = (double)MessageBox.Show(msg1, msg2,  (MessageBoxButton)msg_type);
            }

            return 1;
        }

        static int Function_RGB(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int value_r, value_g, value_b;
            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_r)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_g)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_b)) return -1;

            val = 0xFF000000 + value_r * 0x10000 + value_g * 0x100 + value_b;

            return 1;
        }

        static int Function_ARGB(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int value_a, value_r, value_g, value_b;
            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_a)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_r)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_g)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out value_b)) return -1;

            val = value_a * 0x1000000 + value_r * 0x10000 + value_g * 0x100 + value_b;

            return 1;
        }

        //[NonSerialized]
        static Random random = new Random();

        static int Function_rand(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = random.Next(0x7FFF);	// C++에서 RAND_MAX함수가 7FFF이다. (오토베이스 8 버전과 호환성을 유지하기위해서)

            return 1;
        }

        static int Function_srand(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int seed;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out seed)) return -1;

            random = new Random(seed);

            return 1;
        }

        /*
        static int Function_GetPercent(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double curr, value_base, value_full, fBase, fFull;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out curr)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out value_base)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out value_full)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out fBase)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out fFull)) return -1;

            if ((value_full - value_base) == 0)
            {
                val = fBase;
            }
            else
            {
                val = (((curr - value_base) * (fFull - fBase)) / (value_full - value_base)) + fBase;
            }

            return 1;
        }*/

        static bool GetDigitalInputPos(ScriptClass scriptClass, string tag, ref int[] pos)
        {
            EnumTagType type = 0;

            if (!TagLib.GetTagTypeAndPos(tag, ref type, ref pos))
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("없는 DI 태그({0})", tag));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("DI({0}) tag not found", tag));
                }
                return false;
            }

            if (type != EnumTagType.DI)
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("DI 태그가 아닙니다.({0})", tag));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("not DI tag.({0})", tag));
                }
                return false;
            }

            return true;
        }

        static bool GetDigitalOutputPos(ScriptClass scriptClass, string tag, ref int[] pos)
        {
            EnumTagType type = 0;

            if (!TagLib.GetTagTypeAndPos(tag, ref type, ref pos))
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("없는 DO 태그({0})", tag));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("DO({0}) tag not found", tag));
                }

                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                {
                    MessageDisplay.Show(scriptClass.GetError());
                }
                return false;
            }

            if (type != EnumTagType.DO)
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("DO 태그가 아닙니다.({0})", tag));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Not DO tag.({0})", tag));
                }

                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                {
                    MessageDisplay.Show(scriptClass.GetError());
                }
                return false;
            }

            return true;
        }

        static bool GetDigital(ScriptClass scriptClass, string tag, out double flag)
        {
            int[] pos = new int[1];
            flag = 0;

            if (!GetDigitalInputPos(scriptClass, tag, ref pos)) return false;
            TagDiClass di = TagLib.GetStructDI(tag, ref pos);
            di.NeedDataCurr = true;
            flag = di.curr;

            return true;
        }

        static int Function_DiCurr(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            double curr;
            if (!GetDigital(scriptClass, tag, out curr)) return -1;
            val = curr;

            return 1;
        }

        static Popup popupMessage = null;
        static TimeOutClass timeoutPopup = new TimeOutClass();

        public static void PopupMessageTimer()
        {
            if(popupMessage == null)    return;

            if(!timeoutPopup.IsTimeOut(5))   return;

            popupMessage.IsOpen = false;
            popupMessage = null;
        }

        public static void PopupMessage(string msg)
        {
            if (popupMessage != null)
            {
                popupMessage.IsOpen = false;
                popupMessage = null;
            }

            popupMessage = new Popup();
            Border border = new Border();
            TextBlock tb = new TextBlock();

            DateTime t = DateTime.Now;

            tb.Text = String.Format("{0:00}:{1:00}:{2:00} - {3}", t.Hour, t.Minute, t.Second, msg);
            tb.FontSize = 15;
            tb.TextAlignment = TextAlignment.Center;
            

            border.Background = new SolidColorBrush(Colors.LightGray);
            border.Child = tb;
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            popupMessage.Child = border;

            //popupMessage.VerticalAlignment = VerticalAlignment.Center;
            //popupMessage.HorizontalAlignment = HorizontalAlignment.Center;

            double positioningX = (Application.Current.Host.Content.ActualWidth / 2) - (tb.ActualWidth / 2);
            double positioningY = (Application.Current.Host.Content.ActualHeight / 2) - (tb.ActualHeight / 2);
            if (positioningX > 0)
                popupMessage.HorizontalOffset = positioningX;
            if (positioningY > 0)
                popupMessage.VerticalOffset = positioningY;

            popupMessage.IsOpen = true;

            timeoutPopup.Reset();
        }
                
        static int Function_Message(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string msg;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out msg)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                PopupMessage(msg);
            }

            return 1;
        }
        
        static int Function_PlaySound(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string filename;
            string path;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                path = MakeFilePath.Sound(filename);

                /*
                if (!File.Exists(path))
                {
                    if (Tools.IsLangKorean())
                    {
                        scriptClass.ErrorMessage(String.Format("PlaySound에서 [{0}] 파일을 찾을 수 없습니다.", path));
                    }
                    else
                    {
                        scriptClass.ErrorMessage(String.Format("Can't found [{0}] file at PlaySound function", path));
                    }
                    return -1;
                }
                else
                {
                    Win32Function.PlaySound(null, 0, EnumPlaySound.SND_ASYNC);	// 1 = SND_ASYNC
                    Win32Function.PlaySound(path, 0, EnumPlaySound.SND_ASYNC);	// 1 = SND_ASYNC
                }*/
            }

            return 1;
        }
        /*
        static int Function_WinExec(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string filename;
            string args = "";

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                uint retn = Win32Function.WinExec(filename, (uint)EnumShowWindow.SW_SHOW);

                if (retn < 32)
                {
                    if (Tools.IsLangKorean())
                        scriptClass.ErrorMessage(String.Format("WinExec 오류\nfilename={0}", filename, args));
                    else
                        scriptClass.ErrorMessage(String.Format("WinExec Error\nfilename={0}", filename, args));

                    return -1;
                }
            }

            return 1;
        }
        */
        static int Function_ON_OFF(ScriptClass scriptClass, string command, string argument, out object val, byte flag)
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
                int[] pos = new int[1];

                if (GetDigitalOutputPos(scriptClass, tag, ref pos))
                {
                    TagDoClass dout = TagLib.GetStructDO(tag, ref pos);

                    if (scriptClass.bHandOperation && !SharedData.userInfo.HaveRightsHandOperationAndMsg(dout))
                    {

                    }
                    else
                    {
                        TagWrite.WriteCurrDO(tag, dout, (sbyte)flag, scriptClass.bHandOperation);
                    }
                }
            }
            return 1;
        }
        
        static int Function_DiSet(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;
            double flag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out flag)) return -1;

            int[] pos = new int[1];

            if (!GetDigitalInputPos(scriptClass, tag, ref pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                TagDiClass di = TagLib.GetStructDI(tag, ref pos);
                if (scriptClass.bHandOperation && !SharedData.userInfo.HaveRightsHandOperationAndMsg(di))
                {

                }
                else
                {
                    TagWrite.WriteCurrDI(tag, di, (sbyte)flag, scriptClass.bHandOperation);
                }
            }
            return 1;
        }

        static int Function_DoSetGroup(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;
            double flag;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out tag)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out flag)) return -1;

            int[] pos = new int[1];

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

                if (tp.enumTagType != EnumTagType.GDO)
                {
                    scriptClass.ErrorMessage(String.Format("{0} tag is not Do Group tag.", tag));
                    return -1;
                }

                TagWrite.WriteCurrDoGroup((TagDoGroupClass)tp, (sbyte)flag, scriptClass.bHandOperation);
            }

            return 1;
        }

        /*
        //[NonSerialized]
        public static ArrayList arrayMultiRegister = new ArrayList();

        static int Function_MultiRegister(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf = "";

            int v;
            multiTrendTagStruct block = new multiTrendTagStruct();
            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out block.tag)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out v)) return -1;
            block.color = Tools.ConvertToColor(v);
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out v)) return -1;
            block.edge = (eTrendEdgeType)v;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int[] pos = new int[1];
                if (!scriptClass.GetAnalogInputPos(block.tag, ref pos))
                {
                    return -1;
                }
            }

            if (arrayMultiRegister.Count < 256)
            {
                arrayMultiRegister.Add(block);
            }

            return 1;
        }

        static int Function_MultiClear(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;
            arrayMultiRegister.Clear();

            return 1;
        }

        static int Function_MultiData(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;
            if (arrayMultiRegister.Count == 0) return 1;

            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            double v;
            eDataTime time_size;
            int graph_type;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out v)) return -1;
            time_size = (eDataTime)v;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out v)) return -1;
            graph_type = (int)v;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ViewAnalogInputDataMain.ringViewAnalogInputDataMain.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDataMain(time_size, arrayMultiRegister, ScriptFunctionSet.colorMultiTrendBack, graph_type), ConfigViewMain.nMdiCountOnBasicScreen);
            }

            arrayMultiRegister.Clear();

            return 1;
        }

        public static void PrintScreen(Form form)
        {
            form.Update();

            Bitmap bitmap = ScreenCapture.Capture();

            DialogClipBoardPrint.FormClipBoardPrint dialog = new DialogClipBoardPrint.FormClipBoardPrint();

            dialog.prepareBitmap = bitmap;

            dialog.ShowDialog();
        }

        static int Function_PrintScreen(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                PrintScreen(TotalConfig.formMain);
            }

            return 1;
        }

        static int Function_ViewAlarmList(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;

            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int year, month, day;
            int msg_type;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out month)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out msg_type)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                BasicScreen.kdymain.ViewListAlarmMain.ViewAlarmList(year, month, day, msg_type);
            }

            return 1;
        }*/

        public static int Function_Else(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "ControlBox")
            {
                return Function_ControlBox(scriptClass, command, argument, out val);
            }
            /*    
            else if (command == "GetTagValue")
            {
                return Function_GetTagValue(scriptClass, command, argument, out val);
            }

            else if (command == "GetVarValue")
            {
                return Function_GetVarValue(scriptClass, command, argument, out val);
            }*/
            else if (command == "MessageBox")
            {
                return Function_MessageBox(scriptClass, command, argument, out val);
            }
            else if (command == "RGB")
            {
                return Function_RGB(scriptClass, command, argument, out val);
            }
            else if (command == "ARGB")
            {
                return Function_ARGB(scriptClass, command, argument, out val);
            }
            else if (command == "rand")
            {
                return Function_rand(scriptClass, command, argument, out val);
            }
            else if (command == "srand")
            {
                return Function_srand(scriptClass, command, argument, out val);
            }
                /*
            else if (command == "GetPercent")
            {
                return Function_GetPercent(scriptClass, command, argument, out val);
            }*/
            else if (command == "DiCurr")
            {
                return Function_DiCurr(scriptClass, command, argument, out val);
            }
            else if (command == "Message")
            {
                return Function_Message(scriptClass, command, argument, out val);
            }
            else if (command == "PlaySound")
            {
                return Function_PlaySound(scriptClass, command, argument, out val);
            }
            else if (command == "WinExec")
            {
                //return Function_WinExec(scriptClass, command, argument, out val);
            }
            else if (command == "ON")
            {
                return Function_ON_OFF(scriptClass, command, argument, out val, 1);
            }
            else if (command == "OFF")
            {
                return Function_ON_OFF(scriptClass, command, argument, out val, 0);
            }
            else if (command == "DiSet")
            {
                return Function_DiSet(scriptClass, command, argument, out val);
            }
            else if (command == "DoSetGroup")
            {
                return Function_DoSetGroup(scriptClass, command, argument, out val);
            }
            else if (command == "MultiRegister")
            {
                //return Function_MultiRegister(scriptClass, command, argument, out val);
            }
            else if (command == "MultiClear")
            {
                //return Function_MultiClear(scriptClass, command, argument, out val);
            }
            else if (command == "MultiData")
            {
                //return Function_MultiData(scriptClass, command, argument, out val);
            }

            else if (command == "PrintScreen")
            {
                //return Function_PrintScreen(scriptClass, command, argument, out val);
            }
            else if (command == "ViewAlarmList")
            {
                //return Function_ViewAlarmList(scriptClass, command, argument, out val);
            }

            else { }

            val = 0;

            return 0;
        }
    }
}
