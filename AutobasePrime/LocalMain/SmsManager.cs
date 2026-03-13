using System;
using AutoLibLocal;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LocalMain
{
	/// <summary>
	/// Summary description for LinePrinter.
	/// </summary>
	public class SmsManager
	{
		public SmsManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		
		static byte[] ObjectSerialize(object source)
		{
			MemoryStream s = new MemoryStream();

			BinaryFormatter fomat = new BinaryFormatter();
			fomat.Serialize(s, source);
			return s.ToArray();
		}

        static RingSharedMemory smSmsManager = new RingSharedMemory();
        static RingSharedMemory smSmsMessage = new RingSharedMemory();

		public static void SendAlarm(ALARM_FILE_STRUCT alarm)
		{
            //---------------------------------------------------------------------------------
            // SMS메니저가 실행되어 있지 않으면 누적을 시키지 않는게 나을 듯    2007.2.12 추가
            /*
            bool createdNew = false;
            System.Threading.Mutex gM1 = new System.Threading.Mutex(true, "SmsMainFormMutex", out createdNew);
            gM1.Close();
            if (createdNew)
            {
                return;
            }*/
            //---------------------------------------------------------------------------------
            /*
            System.Threading.Mutex gM1;
            try
            {
                gM1 = System.Threading.Mutex.OpenExisting("SmsMainFormMutex");
            }
            catch
            {
                return;
            }
            if (gM1 == null) return;*/

            if (!LinePrinter.IsMutexExisting("SmsMainFormMutex")) return;

            smSmsManager.AddItem(alarm);

            

			/*
            using(ComAlarm.AlarmSmsManager com = new ComAlarm.AlarmSmsManager())
			{
				com.AddAlarm(ObjectSerialize(alarm));
			}*/
		}

		public static void Init()
		{
            smSmsManager.Create("NetSmsManagerSharedMemory", 10, 1000);
            smSmsMessage.Create("NetSmsMessageSendSharedMemory", 10, 1000);
		}

		public static void UnInit()
		{
            smSmsManager.Close();
            smSmsMessage.Close();
		}

		public static void SmsSend(string recv, string send, string msg, int sms_type)
		{
            ClassSmsMessage s = new ClassSmsMessage();
            s.recv = recv;
            s.send = send;
            s.msg = msg;
            s.sms_type = sms_type;

            smSmsMessage.AddItem(s);

			/*using(ComAlarm.SmsMessage com = new SmsMessage()) 
			{
				com.AddMessage(recv, send, msg, sms_type);
			}*/
		}
	}
}
