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
using System.Threading;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class PublicInit
    {
        //Thread thread1;

        public void Init()
        {
            /*
            ConfigVarTotal.Init(MainStart.mainArgs.Length > 0 ? MainStart.mainArgs[0] : null);

            MakeFilePath.ResetAllFileFlags();

            TotalConfig.formMain = this;
            SharedViewMain.Prepare();

            // @MenuMessage에서 사용할 콜백함수를 등록한다.
            ScriptFunctionMenu.procCallBackMenuMessage = new ScriptFunctionMenu.CallBack(this.CallBackUserMenu);
            */

            TerminalClass.Init();

            //thread1 = new Thread(new ThreadStart(ThreadDataChange.ThreadMain));
            //thread1.Name = "ViewMainDataGather";
            //thread1.Start();

            /*
            if (ConfigVarTotal.bLocalFlag)
                this.Text = String.Format("AutoBase View", ConfigVarTotal.sSiteRootName);
            else
                this.Text = String.Format("AutoBase Web View (http://{0})", ConfigVarTotal.sSiteRootName);*/

            // toolBarModule = new ToolBarModule();

            
        }

        public void UnInit()
        {
            ThreadDataChange.bThreadContinue = false;

            TimeOutClass timeout = new TimeOutClass();
            while (!ThreadDataChange.bThreadEnded)
            {
                if (timeout.IsTimeOut(2)) break;
            }

            // timerMain.Enabled = false;

            //Form[] childForm = this.MdiChildren;
            //Make sure to ask for saving the doc before exiting the app 
            //for (int i = 0; i < childForm.Length; i++)
            //    childForm[i].Close();

            /*
            if (bLogInStatus) // 현재 로그인 중일 때만 로그 아웃.
            {
                DataGate gate = new DataGate();
                gate.LogOut();
            }*/
        }
    }
}
