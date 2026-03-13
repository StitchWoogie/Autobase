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
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using System.IO;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ObjectButtonProgram : ObjectButtonPublic
    {
        public ScriptClass scriptButton = null;
        public int nScriptType = 1;
        public string sFileName;

        public ObjectButtonProgram(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general, ObjectArgsButtonPublic args, int script_type, string filename, ScriptClass script)
            : base(ocp, parent_canvas, rect, eid, lf, general, args)
        {
            //
            // TODO: Add constructor logic here
            //

            enumObjectType = EnumObjectType.ButtonProgramm;

            nScriptType = script_type;
            sFileName = filename;
            scriptButton = script;
        }

        protected override void OnClicked()
        {
            if (nScriptType == 0)
            {
                string path = MakeFilePath.Project("Control/button", sFileName);
                path = MakeFilePath.MakePublishTextPath(path);

                System.Net.WebClient client = new System.Net.WebClient();
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                client.OpenReadAsync(new Uri(path, UriKind.Absolute));
            }
            else
            {
                if (scriptButton != null)
                {
                    scriptButton.SetHandOperation();	// 수동으로 출력한다.
                    scriptButton.Run(objCommonProperty.rootPage); 

                    if (scriptButton.IsError())
                    {
                        string message;
                        message = scriptButton.GetError();

                        if (Tools.IsLangKorean())
                            MessageBox.Show("스크립트 오류\n\n" + message, "스크립트 실행 버튼", MessageBoxButton.OK);
                        else if (Tools.IsLangChinese())
                            MessageBox.Show("脚本错误\n\n" + message, "脚本运行按钮", MessageBoxButton.OK);
                        else
                            MessageBox.Show("Script Error\n\n" + message, "Program Button", MessageBoxButton.OK);
                    }
                }
            }
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null) return;

            System.Windows.Resources.StreamResourceInfo resinfo = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

            TextReader reader = new StreamReader(resinfo.Stream);

            if (reader == null) return;

            ScriptClass script = new ScriptClass();

            script.LoadFromMODX(reader, "LoadFromModXFile");

            script.SetHandOperation();	// 수동으로 출력한다.
            script.Run(objCommonProperty.rootPage);

            if (script.IsError())
            {
                string message;
                message = script.GetError();

                if (Tools.IsLangKorean())
                    MessageBox.Show("스크립트 오류\n\n" + message, "스크립트 실행 버튼", MessageBoxButton.OK);
                else if (Tools.IsLangChinese())
                    MessageBox.Show("脚本错误\n\n" + message, "脚本运行按钮", MessageBoxButton.OK);
                else
                    MessageBox.Show("Script Error\n\n" + message, "Program Button", MessageBoxButton.OK);
            }
        }

        /*
        public override bool WmLeftButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            bool retn = base.WmLeftButtonUp(form, e);

            if (retn == true)
            {
                if (nScriptType == 0)
                {
                    if (sFileName != null)
                    {
                        ScriptClass script = new ScriptClass();

                        string path = MakeFilePath.Project("Control\\button", sFileName);

                        script.LoadFromFile(path);

                        script.SetHandOperation();	// 수동으로 출력한다.
                        script.Run(form);

                        if (script.IsError())
                        {
                            string message;
                            message = script.GetError();

                            if (Tools.IsLangKorean())
                                MessageBox.Show("스크립트 오류\n\n" + message, "스크립트 실행 버튼");
                            else if (Tools.IsLangChinese())
                                MessageBox.Show("脚本错误\n\n" + message, "脚本运行按钮");
                            else
                                MessageBox.Show("Script Error\n\n" + message, "Program Button");
                        }
                    }
                }
                else
                {
                    if (scriptButton != null)
                    {
                        scriptButton.SetHandOperation();	// 수동으로 출력한다.
                        scriptButton.Run(form);

                        if (scriptButton.IsError())
                        {
                            string message;
                            message = scriptButton.GetError();

                            if (Tools.IsLangKorean())
                                MessageBox.Show("스크립트 오류\n\n" + message, "스크립트 실행 버튼");
                            else if (Tools.IsLangChinese())
                                MessageBox.Show("脚本错误\n\n" + message, "脚本运行按钮");
                            else
                                MessageBox.Show("Script Error\n\n" + message, "Program Button");
                        }
                    }
                }
            }

            return retn;
        }


        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.FileName(writer, sFileName);
            if (scriptButton != null)
            {
                writer.WriteLine("\t\tLocalScript,BEGIN");
                scriptButton.SaveFile(writer);
                writer.WriteLine("\t\tLocalScript,END");

            }
            writer.WriteLine("\tStringOption,{0},", nScriptType);
            SaveObjectItem.String(writer, this.Text);
            ObjectSaveFont(writer);
        }*/

        public override void GetMultiSelectTagList(System.Collections.Generic.List<object> block)
        {
            base.GetMultiSelectTagList(block);

            if (scriptButton != null)
            {
                scriptButton.GetMultiSelectTagList(block, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, "Script");
            }
        }

        public override void SetMultiSelectTagList(System.Collections.Generic.List<object> block)
        {
            base.SetMultiSelectTagList(block);

            if (scriptButton != null)
            {
                scriptButton.SetMultiSelectTagList(block);
            }
        }

    }
}
