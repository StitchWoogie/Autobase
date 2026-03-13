using System;
using AutoLibLocal;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LocalMain
{
	/// <summary>
	/// Summary description for LinePrinter.
	/// </summary>
	public class LinePrinter
	{
		public LinePrinter()
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

        public static bool IsMutexExisting(string name)
        {
            bool createdNew = false;
            System.Threading.Mutex gM1 = new System.Threading.Mutex(true, name, out createdNew);
            gM1.Close();
            if (createdNew)
            {
                return false;
            }

            return true;
        }

		public static void SendAlarm(ALARM_FILE_STRUCT alarm)
		{
            /*
            //---------------------------------------------------------------------------------
            // LinePrinter가 실행되어 있지 않으면 누적을 시키지 않는게 나을 듯    2007.2.20 추가
            bool createdNew = false;
            System.Threading.Mutex gM1 = new System.Threading.Mutex(true, "AutoBaseLinePrinter", out createdNew);
            gM1.Close();
            if (createdNew)
            {
                return;
            }
            //---------------------------------------------------------------------------------
             */
            /*System.Threading.Mutex gM1;
            try  // Open은 없을 때 try catch가 발생한다.
            {
                gM1 = System.Threading.Mutex.OpenExisting("AutoBaseLinePrinter");
            }
            catch
            {
                return;
            }
            if (gM1 == null) return;*/
            if (!IsMutexExisting("AutoBaseLinePrinter")) return;
            
            smLinePrinter.AddItem(alarm);
            /*
			using(ComAlarm.AlarmLinePrinter com = new AlarmLinePrinter())
			{
				com.AddAlarm(ObjectSerialize(alarm));
			}*/
		}

        static RingSharedMemory smLinePrinter = new RingSharedMemory();

		public static void Init()
		{
            smLinePrinter.Create("NetLinePrinterSharedMemory", 10, 1000);
		}

		public static void UnInit()
		{
            smLinePrinter.Close();
		}
	}
}
