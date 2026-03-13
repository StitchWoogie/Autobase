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
using AutoLib;
using NetTools;
using System.Threading;
using System.IO;

namespace SilverlightGraphicModule
{
    /*
    public class ScriptFileDownLoadAndRun
    {
        bool bReaded = false;
        string sErrorMessage = "";
        UserControl parentControl = null;

        public bool Run(UserControl parent, string filename)
        {
            parentControl = parent;
            bReaded = false;
            sErrorMessage = "";

            string path = MakeFilePath.MakePublishTextPath(filename);

            System.Net.WebClient client = new System.Net.WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri(path, UriKind.Absolute));

            TimeOutClass timeout = new TimeOutClass();

            while (!bReaded)
            {
                Thread.Sleep(1);
                if (timeout.IsTimeOut(10))
                {
                    MessageBox.Show("Cannot download the script file.", System.IO.Path.GetFileName(filename), MessageBoxButton.OK);
                    return false;
                }
            }

            if (sErrorMessage.Length > 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("스크립트 오류\n\n" + sErrorMessage, "스크립트", MessageBoxButton.OK);
                else if (Tools.IsLangChinese())
                    MessageBox.Show("脚本错误\n\n" + sErrorMessage, "Script", MessageBoxButton.OK);
                else
                    MessageBox.Show("Script Error\n\n" + sErrorMessage, "Script", MessageBoxButton.OK);

                return false;
            }

            return true;
        }

        void Message(string msg)
        {
            if (Tools.IsLangKorean())
                MessageBox.Show("스크립트 오류\n\n" + msg, "스크립트", MessageBoxButton.OK);
            else if (Tools.IsLangChinese())
                MessageBox.Show("脚本错误\n\n" + msg, "Script", MessageBoxButton.OK);
            else
                MessageBox.Show("Script Error\n\n" + msg, "Script", MessageBoxButton.OK);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                sErrorMessage = e.Error.Message;
                Message(sErrorMessage);
                bReaded = true;
                return;
            }

            if (e.Cancelled == true)
            {
                sErrorMessage = "Canceled";
                Message(sErrorMessage);
                bReaded = true;
                return;
            }

            if (e.Result == null)
            {
                sErrorMessage = "Result = null";
                Message(sErrorMessage);
                bReaded = true;
                return;
            }

            System.Windows.Resources.StreamResourceInfo resinfo = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

            TextReader reader = new StreamReader(resinfo.Stream);

            if (reader == null)
            {
                sErrorMessage = "Can't read the stream.";
                Message(sErrorMessage);
                bReaded = true;
                return;
            }

            ScriptClass script = new ScriptClass();

            script.LoadFromMODX(reader, "LoadFromModXFile");

            script.SetHandOperation();	// 수동으로 출력한다.
            script.Run(parentControl);

            if (script.IsError())
            {
                sErrorMessage = script.GetError();
                Message(sErrorMessage);
            }

            bReaded = true;
            return;
        }
    }*/

    public class ScriptFileDownLoadAndRun
    {
        UserControl parentControl = null;
        string sFileName;

        public bool Run(UserControl parent, string filename)
        {
            parentControl = parent;
            sFileName = filename;

            string path = MakeFilePath.MakePublishTextPath(filename);

            System.Net.WebClient client = new System.Net.WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri(path, UriKind.Absolute));

            return true;
        }

        void Message(string msg)
        {
            if (Tools.IsLangKorean())
                MessageBox.Show("스크립트 오류\n\n" + msg, System.IO.Path.GetFileName(sFileName), MessageBoxButton.OK);
            else if (Tools.IsLangChinese())
                MessageBox.Show("脚本错误\n\n" + msg, System.IO.Path.GetFileName(sFileName), MessageBoxButton.OK);
            else
                MessageBox.Show("Script Error\n\n" + msg, System.IO.Path.GetFileName(sFileName), MessageBoxButton.OK);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            string msg;

            if (e.Error != null)
            {
                msg = e.Error.Message;
                Message(msg);
                return;
            }

            if (e.Cancelled == true)
            {
                msg = "Canceled";
                Message(msg);
                return;
            }

            if (e.Result == null)
            {
                msg = "Result = null";
                Message(msg);
                return;
            }

            System.Windows.Resources.StreamResourceInfo resinfo = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

            TextReader reader = new StreamReader(resinfo.Stream);

            if (reader == null)
            {
                msg = "Can't read the stream.";
                Message(msg);
                return;
            }

            ScriptClass script = new ScriptClass();

            script.LoadFromMODX(reader, "LoadFromModXFile");

            script.SetHandOperation();	// 수동으로 출력한다.
            script.Run(parentControl);

            if (script.IsError())
            {
                msg = script.GetError();
                Message(msg);
            }

            return;
        }
    }
}
