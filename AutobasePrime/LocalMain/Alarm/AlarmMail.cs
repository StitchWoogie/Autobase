using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;
using System.Threading;
using GraphicModule;
using NetTools;

namespace LocalMain.Alarm
{
    class AlarmMail
    {
        static List<ALARM_FILE_STRUCT> arrayAlarm = new List<ALARM_FILE_STRUCT>();

        public static void SendAlarm(ALARM_FILE_STRUCT alarm)
        {
            if (!ConfigAlarm.bMailActive) return;

            if (arrayAlarm.Count >= 100) return;    // 대기중인 항목이 너무 많다.

            // 경보를 저장해 놓고 시간날 때 메일을 보내준다.
            lock (arrayAlarm)
            {
                arrayAlarm.Add(alarm);
            }

            if (threadMail == null)
            {
                threadMail = new Thread(new ThreadStart(ThreadLoop));
                threadMail.Start();
            }
        }

        static Thread threadMail = null;

        //static bool bDone = false;
        static bool bEnd = false;

        static void ThreadLoop()
        {
            bEnd = false;
            //bDone = false;

            string seed = System.Environment.MachineName + System.Environment.UserName;
            string password = WebTools.StringHash.Decode(ConfigAlarm.sMailPassword, seed);

            ALARM_FILE_STRUCT alarm;
            TimeOutClass timeout = new TimeOutClass();
            timeout.SetTime(ConfigAlarm.nMailSendingInverval);  // 시작시는 Interval이 없다.
            
            while (!bEnd)
            {
                Thread.Sleep(1);

                if (arrayAlarm.Count == 0) continue;

                if (ConfigAlarm.nMailSendingInverval > 0)
                {
                    if (!timeout.IsTimeOut(ConfigAlarm.nMailSendingInverval))
                        continue;
                    timeout.Reset();
                }

                lock (arrayAlarm)
                {
                    alarm = arrayAlarm[0];
                    arrayAlarm.RemoveAt(0);
                }

                string title = String.Format("Alarm {0} {1}", alarm.tag, alarm.msg);
                string text = String.Format("Time = {0}\nTag = {1}\nTag Description = {2}\nMessage = {3}\nPriority = {4}", 
                    alarm.t.ToDateTime(), alarm.tag, alarm.description, alarm.msg, alarm.priority);
                string err_msg;

                if (ScriptFunctionMail.SendMail(ConfigAlarm.sMailServer, ConfigAlarm.sMailUsername, password, ConfigAlarm.bMailSSL, ConfigAlarm.sMailTo, ConfigAlarm.sMailFrom, title, text, false, "", ConfigAlarm.nMailPort, out err_msg))
                {

                }
                else
                {
                    MessageDisplay.ShowInThread("Alarm Mail Error : {0}", err_msg);
                }

            }
            

            //bDone = true;
        }

        // 닫지 못한 메일용 스레드를 닫아준다.
        public static void UnInit()
        {
            if (threadMail != null)
            {
                bEnd = true;
                threadMail.Join(5000);
            }
        }
    }
}
