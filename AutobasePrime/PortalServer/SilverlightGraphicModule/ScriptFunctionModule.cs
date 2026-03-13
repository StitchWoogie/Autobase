using System;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionModule
    {
        public ScriptFunctionModule()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_LoadModule(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string filename;
            string buf;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                //string url = String.Format("GraphicModulePage.aspx?filename={0}", filename);
                //System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(url, UriKind.Relative));

                string url = MakeFilePath.SilverlightGraphicModule(filename);
                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(url, UriKind.Absolute));
            }

            return 1;
        }

        static int Function_CloseModule(ScriptClass scriptClass, string command, string argument, out object val)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                // 현재의 Active된 child에게 그래픽 모듈의 이름을 알아본다.
                Page form = (Page)scriptClass.formParent;

                /*
                while (true)
                {
                    if (form == null) break;
                    if (form.Name == "FormGraphicFrame")
                    {
                        //form.Close();
                        ((FormGraphicFrame)form).bCloseCommand = true;
                        break;
                    }
                    form = form.ParentForm;
                }*/
                form.Close();
            }
            return 1;
        }

        /*
        static int Function_LoadModulePosition(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string filename;
            string buf;
            double method;
            double x;
            double y;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out method)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out x)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out y)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                string path = MakeFilePath.Graphic(filename);
                GraphicTool.RestoreGraphicWindow(path, (int)method, (int)x, (int)y);
            }

            return 1;
        }

        static int Function_ModuleCloseName(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string filename;
            string buf;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int i;
                FormGraphicFrame form;
                for (i = 0; i < FormGraphicFrame.arrayFormGraphFrame.Count; i++)
                {
                    form = (FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                    if (String.Compare(filename, form.formChild.GetModuleSubDirAndName(), true) == 0)
                    {
                        //form.Close();
                        form.bCloseCommand = true;
                    }
                }
            }

            return 1;
        }

        static int Function_ModuleIsAlive(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string filename;
            string buf;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int i;
                FormGraphicFrame form;
                for (i = 0; i < FormGraphicFrame.arrayFormGraphFrame.Count; i++)
                {
                    form = (FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                    if (String.Compare(filename, form.formChild.GetModuleSubDirAndName(), true) == 0)
                    {
                        val = 1;
                        return 1;
                    }
                }

                val = 0;
            }

            return 1;
        }

        
        static ObjectRoot printModuleObject;

        private static void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            printModuleObject.SetObjectOpticMethod(1);
            printModuleObject.SetScreenSize((int)(ev.PageSettings.Bounds.Width * 0.9), (int)(ev.PageSettings.Bounds.Height * 0.9));
            Point point = new Point(0, 0);
            printModuleObject.DisplayPrint(ev.Graphics, ev.PageSettings.Bounds, ev.PageSettings.Bounds, point);

            ev.HasMorePages = false;
        }

        static int Function_PrintModule(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string path;
            string real;
            double orientation;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out real)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, buf.Length, out orientation)) return -1;


            path = MakeFilePath.Graphic(real);
            if (!File.Exists(path))
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("{0} 파일을 찾을 수 없습니다.", path));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Can't find {0} file.", path));
                }
                return -1;
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                ObjectRoot obj = new ObjectRoot();

                obj.Load(scriptClass.formParent, path);

                PrintDocument pd = new PrintDocument();
                string print_name = ConfigViewMain.PrinterOnScriptPrintModule;
                if (print_name.Length > 0)
                {
                    PrinterSettings ps = new PrinterSettings();
                    ps.PrinterName = print_name;
                    pd.PrinterSettings = ps;
                }

                PageSettings tempPage = new PageSettings();

                if (orientation == 1) tempPage.Landscape = false;
                else if (orientation == 2) tempPage.Landscape = true;
                else { }

                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                pd.DocumentName = Path.GetFileName(path);
                pd.DefaultPageSettings = tempPage;

                printModuleObject = obj;

                try
                {
                    pd.Print();
                }
                catch (Exception exception)
                {
                    string msg = String.Format("@PrintModule Error - {0}", exception.Message);
                    scriptClass.ErrorMessage(msg);
                    return -1;
                }

                return 1;
            }	// edit mode
            else
            {
                return 1;
            }
        }*/

        static int Function_ModuleInvalidate(ScriptClass scriptClass, string command, string argument, out object val)
        {
            // 사용하지 않아도 되는 함수는 return 1 한다.
            val = 0;

            return 1;

            /*
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string filename;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int i;
                FormGraphicFrame form;
                for (i = 0; i < FormGraphicFrame.arrayFormGraphFrame.Count; i++)
                {
                    form = (FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                    if (String.Compare(filename, form.formChild.GetModuleSubDirAndName(), true) == 0)
                    {
                        form.formChild.Invalidate();
                        val = 1;
                        break;
                    }
                }

                ToolBarModule.Invalidate(filename);

                val = 1;
            }

            return 1;*/
        }

        /*
        static int Function_ModuleUpdate(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string filename;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out filename)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int i;
                FormGraphicFrame form;
                for (i = 0; i < FormGraphicFrame.arrayFormGraphFrame.Count; i++)
                {
                    form = (FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                    if (String.Compare(filename, form.formChild.GetModuleSubDirAndName(), true) == 0)
                    {
                        form.formChild.Update();
                        val = 1;
                        break;
                    }
                }

                ToolBarModule.Update(filename);

                val = 1;
            }

            return 1;
        }*/

        public static int Function_Module(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (String.Compare(command, 0, "Module", 0, 6) == 0)
            {
                if (command == "ModuleCloseName")
                {
                    //return Function_ModuleCloseName(scriptClass, command, argument, out val);
                }
                else if (command == "ModuleIsAlive")
                {
                    //return Function_ModuleIsAlive(scriptClass, command, argument, out val);
                }
                else if (command == "ModuleInvalidate")
                {
                    return Function_ModuleInvalidate(scriptClass, command, argument, out val);
                }
                else if (command == "ModuleUpdate")
                {
                    //return Function_ModuleUpdate(scriptClass, command, argument, out val);
                }
                else
                {
                    if (Tools.IsLangKorean())
                    {
                        scriptClass.ErrorMessage(String.Format("지원되지 않는 Module 함수입니다.\n({0})", command));
                    }
                    else if (Tools.IsLangChinese())
                    {
                        scriptClass.ErrorMessage(String.Format("不支持的 Module 函数。({0})", command));
                    }
                    else
                    {
                        scriptClass.ErrorMessage(String.Format("Undefined Module function.\n({0})", command));
                    }
                    val = 0;
                    return -1;
                }
            }

            if (command == "LoadModule")
            {
                return Function_LoadModule(scriptClass, command, argument, out val);
            }
            else if (command == "CloseModule")
            {
                return Function_CloseModule(scriptClass, command, argument, out val);
            }
            else if (command == "LoadModulePosition")
            {
                //return Function_LoadModulePosition(scriptClass, command, argument, out val);
            }
            else if (command == "PrintModule")
            {
                //return Function_PrintModule(scriptClass, command, argument, out val);
            }
            else { }

            val = 0;

            return 0;
        }
    }
}