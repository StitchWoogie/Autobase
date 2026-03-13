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

namespace AutoLibLocal
{
    public class MessageDisplay
    {
        /*
        public static FormMessageDisplay formMessageDisplay = null;
        public static int nScreenLifeTime = 5;

        public MessageDisplay()
        {

        }
        */
        static public void Show(string format, params object[] args)
        {
            /*
            string msg;

            if (args.Length == 0)	// 이 부분이 없으면 Tag Description에서 {} 문자를 사용했을 때 다운된다.	
                msg = format;
            else
                msg = String.Format(format, args);

            if (formMessageDisplay == null)
            {
                FormMessageDisplay form = new FormMessageDisplay();
                //form.TopMost = true;
                form.labelMsg.Text = msg;
                form.Owner = TotalConfig.formMain;

                if (NetTools.Tools.IsLangKorean())
                    form.Text = "메시지 발생:" + DateTime.Now.ToString();
                else if (NetTools.Tools.IsLangChinese())
                    form.Text = "消息发生:" + DateTime.Now.ToString();
                else
                    form.Text = "Message:" + DateTime.Now.ToString();

                form.Show();
            }
            else
            {
                formMessageDisplay.labelMsg.Text = msg;
                formMessageDisplay.Invalidate();
                formMessageDisplay.timeout.Reset();
                if (NetTools.Tools.IsLangKorean())
                    formMessageDisplay.Text = "메시지 발생:" + DateTime.Now.ToString();
                else if (NetTools.Tools.IsLangChinese())
                    formMessageDisplay.Text = "消息发生:" + DateTime.Now.ToString();
                else
                    formMessageDisplay.Text = "Message:" + DateTime.Now.ToString();
            }*/
            
        }


    }
}
