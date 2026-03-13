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
using AutoLib;
using System.IO;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionMenu
    {
        //public delegate System.EventHandler CallBack(ToolStripMenuItem mi, string id_buf);
        //public static CallBack procCallBackMenuMessage = null;

        static int Function_MenuMessage(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string real;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out real)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (String.Compare(real, 0, "MENU_SCRIPT_", 0, 12) == 0)
                {
                    int id = ConvertTool.ToInt32(real.Substring(12));
                    PlayMenuUserScript(scriptClass.formParent, id);
                }
                
                return 1;
            }
            else
            {
                return 1;
            }
        }

        static object PlayMenuUserScript(UserControl parent, int id)
        {
            ScriptFileDownLoadAndRun script = new ScriptFileDownLoadAndRun();

            string file = String.Format("Script{0:00}.CTLX", id);
            string fullpath = MakeFilePath.Project("Control/Menu", file);

            script.Run(parent, fullpath);

            return 1;
        }

        static int Function_MenuScript(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int id;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out id)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = PlayMenuUserScript(scriptClass.formParent, id);
                return 1;
            }
            else
            {
                return 1;
            }
        }

        public static int Function_Menu(ScriptClass scriptClass, string command, string argument, out object value)
        {
            if (String.Compare(command, "MenuMessage") == 0)
            {
                return Function_MenuMessage(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "MenuScript") == 0)
            {
                return Function_MenuScript(scriptClass, command, argument, out value);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Menu 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Menu 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Menu function ({0})", command));
                }
                value = 0;
                return -1;
            }
        }

        /*
        public delegate ToolStripMenuItem CallBackMenuItemGetFromTitle(string title);
        public static CallBackMenuItemGetFromTitle procCallBackMenuItemGetFromTitle = null;

        static int ProcMenuItemFindByTitle(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string title = (string)args[0];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procCallBackMenuItemGetFromTitle != null)
                {
                    val = procCallBackMenuItemGetFromTitle(title);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        static int ProcMenuItemSetTextColor(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            object menuitem = args[0];
            int color = (int)args[1];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (menuitem != null && menuitem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)menuitem;
                    tsmi.ForeColor = Color.FromArgb(color);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        static int ProcMenuItemSetBackColor(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            object menuitem = args[0];
            int color = (int)args[1];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (menuitem != null && menuitem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)menuitem;
                    tsmi.BackColor = Color.FromArgb(color);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        static int ProcMenuItemSetCheck(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            object menuitem = args[0];
            int check = (int)args[1];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (menuitem != null && menuitem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)menuitem;
                    tsmi.Checked = (check == 1);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Menu";

            prepare.AddMethod(prename, "MenuItemFindByTitle", "object", new ScriptExternalRun.DeleMethod(ProcMenuItemFindByTitle), "in:string:title");
            prepare.AddMethod(prename, "MenuItemSetBackColor", "void", new ScriptExternalRun.DeleMethod(ProcMenuItemSetBackColor), "in:object:menuitem", "in:int:color");
            prepare.AddMethod(prename, "MenuItemSetCheck", "void", new ScriptExternalRun.DeleMethod(ProcMenuItemSetCheck), "in:object:menuitem", "in:int:check");
            prepare.AddMethod(prename, "MenuItemSetTextColor", "void", new ScriptExternalRun.DeleMethod(ProcMenuItemSetTextColor), "in:object:menuitem", "in:int:color");
        }*/
    }

    /*
    public class ScriptFunctionMenu
    {
        public delegate System.EventHandler CallBack(ToolStripMenuItem mi, string id_buf);
        public static CallBack procCallBackMenuMessage = null;

        static int Function_MenuMessage(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string real;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out real)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procCallBackMenuMessage != null)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem();   // 사용자 메뉴의 MENU_SCRIPT_??? 의ID를 mi.tag에 할당하기 위해서 필요하다
                    System.EventHandler handler = procCallBackMenuMessage(mi, real);
                    if (handler != null)
                    {
                        handler(mi, null);
                    }
                }
                return 1;
            }
            else
            {
                return 1;
            }
        }

        public static object PlayMenuUserScript(int id)
        {
            ScriptClass control = new ScriptClass();

            string fullpath;

            if (ConfigVarTotal.bLocalFlag)
            {
                fullpath = String.Format("{0}\\control\\Menu\\Script{1:00}.CTLX", TotalConfig.sDirWorkProject, id);
                if (!File.Exists(fullpath))
                {
                    fullpath = String.Format("{0}\\control\\Menu\\Script{1:00}.CTL", TotalConfig.sDirWorkProject, id);
                }
            }

            else // 웹 버전에서는 CTLX 파일만 허용한다. 웹버전에서 MenuScript지원은 9.3.6 부터 지원되었기 때문에 CTL 파일은 지원하지 않는다.
            {
                string file = String.Format("Script{0:00}.CTLX", id);
                fullpath = MakeFilePath.Project("Control\\Menu", file);
            }

            if (!File.Exists(fullpath))
            {
                string message;
                message = String.Format("MENU_SCRIPT_{0:00}", id);

                if (Tools.IsLangKorean())
                    MessageBox.Show("메뉴 스크립트가 작성되지 않았습니다.\n스튜디오의 [파일|스크립트 편집|메뉴 스크립트] 를 사용하여 작성하세요.", message);
                else
                    MessageBox.Show("Menu Script is not found.\nEdit Menu Script at [File|Script|Menu Script] of Studio.", message);

                return 0;	// file not found
            }

            control.LoadFromFile(fullpath);
            control.SetHandOperation();
            control.Run(TotalConfig.formMain, null);

            if (control.IsError())
            {
                string message;
                message = control.GetError();
                MessageDisplay.Show(message);
            }

            return control.GetReturnValueObject();
        }

        static int Function_MenuScript(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int id;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out id)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = PlayMenuUserScript(id);
                return 1;
            }
            else
            {
                return 1;
            }
        }

        public static int Function_Menu(ScriptClass scriptClass, string command, string argument, out object value)
        {
            if (String.Compare(command, "MenuMessage") == 0)
            {
                return Function_MenuMessage(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "MenuScript") == 0)
            {
                return Function_MenuScript(scriptClass, command, argument, out value);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Menu 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Menu 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Menu function ({0})", command));
                }
                value = 0;
                return -1;
            }
        }

        public delegate ToolStripMenuItem CallBackMenuItemGetFromTitle(string title);
        public static CallBackMenuItemGetFromTitle procCallBackMenuItemGetFromTitle = null;

        static int ProcMenuItemFindByTitle(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string title = (string)args[0];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (procCallBackMenuItemGetFromTitle != null)
                {
                    val = procCallBackMenuItemGetFromTitle(title);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        static int ProcMenuItemSetTextColor(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            object menuitem = args[0];
            int color = (int)args[1];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (menuitem != null && menuitem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)menuitem;
                    tsmi.ForeColor = Color.FromArgb(color);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        static int ProcMenuItemSetBackColor(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            object menuitem = args[0];
            int color = (int)args[1];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (menuitem != null && menuitem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)menuitem;
                    tsmi.BackColor = Color.FromArgb(color);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        static int ProcMenuItemSetCheck(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            object menuitem = args[0];
            int check = (int)args[1];

            val = null;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (menuitem != null && menuitem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem tsmi = (ToolStripMenuItem)menuitem;
                    tsmi.Checked = (check == 1);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Menu";

            prepare.AddMethod(prename, "MenuItemFindByTitle", "object", new ScriptExternalRun.DeleMethod(ProcMenuItemFindByTitle), "in:string:title");
            prepare.AddMethod(prename, "MenuItemSetBackColor", "void", new ScriptExternalRun.DeleMethod(ProcMenuItemSetBackColor), "in:object:menuitem", "in:int:color");
            prepare.AddMethod(prename, "MenuItemSetCheck", "void", new ScriptExternalRun.DeleMethod(ProcMenuItemSetCheck), "in:object:menuitem", "in:int:check");
            prepare.AddMethod(prename, "MenuItemSetTextColor", "void", new ScriptExternalRun.DeleMethod(ProcMenuItemSetTextColor), "in:object:menuitem", "in:int:color");
        }
    }*/
}
