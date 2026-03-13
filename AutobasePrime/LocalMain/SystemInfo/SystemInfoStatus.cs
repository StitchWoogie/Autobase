using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetTools;
using AutoLibLocal;
using AutoLibLocal.KeyLock;

namespace LocalMain.SystemInfo
{
    class SystemInfoStatus
    {
        // VIN0 = DDR2
        public const int MAX_ITEM = 10;
        public static string[] itemName = new string[MAX_ITEM] {      "CPU Temp", "System Temp",  "CPU Fan Speed","System Fan Speed", "VCORE", "DDR2(V)", "12V",  "5V",   "3.3V", "Battery(V)" };
        public static string[] itemCommand = new string[MAX_ITEM] { "CPUTEMP", "SYSTEMP", "CPUFAN", "SYSFAN", "VCORE", "VIN0", "VIN2", "5VCC", "VIN1", "VBAT" };
        public static float[] itemMax = new float[MAX_ITEM] { 80, 75, 4000, 4000, 1.4f, 2, 13, 5.5f, 3.5f, 4 };
        public static float[] itemMin = new float[MAX_ITEM] { 0, 0, 2000, 2000, 0.5f, 1.5f, 11, 4.5f, 3.1f, 3 };
        public static float[] itemMaxDefault = new float[MAX_ITEM] { 80, 75, 4000, 4000, 1.4f, 2, 13, 5.5f, 3.5f, 4 };
        public static float[] itemMinDefault = new float[MAX_ITEM] { 0, 0, 2000, 2000, 0.5f, 1.5f, 11, 4.5f, 3.1f, 3 };
        public static double[] itemCurr = new double[MAX_ITEM] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        public static bool[] itemAlarm = new bool[MAX_ITEM] { true, true, false, true, true, true, true, true, true, true };

        static Thread threadMain = null;

        public static bool bUseBoardInfo = TotalConfig.LoadRegAutoBaseConfig("SysInfo", "Board", "bUseBoardInfo", true);
        public static bool bUseBoardAlarm = TotalConfig.LoadRegAutoBaseConfig("SysInfo", "Board", "bUseBoardAlarm", false);

        public static bool IsAble
        {
            get
            {
                return (threadMain != null);
            }
        }

        public static void SystemInfoInit()
        {
            if (KeyLock.sBaseBoardProductName != "945GSE") return;

            for (int i = 0; i < MAX_ITEM; i++)
            {
                itemAlarm[i] = TotalConfig.LoadRegAutoBaseConfig("SysInfo", "Board\\AlarmItem", itemName[i], itemAlarm[i]);
                itemMin[i] = ConvertTool.ToSingle(TotalConfig.LoadRegAutoBaseConfig("SysInfo", "Board\\AlarmLow", itemName[i], itemMinDefault[i].ToString()));
                itemMax[i] = ConvertTool.ToSingle(TotalConfig.LoadRegAutoBaseConfig("SysInfo", "Board\\AlarmHigh", itemName[i], itemMaxDefault[i].ToString()));
            }

            threadMain = new Thread(new ThreadStart(ThreadLoop));
            threadMain.Start();
        }

        static bool bEnd = false;

        static void ThreadLoop()
        {
            SystemInfoGate.GateInit();

            double val;
            TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();
            int pos = 0;

            while (!bEnd)
            {
                Thread.Sleep(1);

                if (!timeout.IsTimeOut(100)) continue;
                timeout.Reset();

                if (SystemInfoGate.GateRead(itemCommand[pos], out val))
                {
                    itemCurr[pos] = val;
                }

                pos++;
                pos %= MAX_ITEM;
            }

            SystemInfoGate.GateUnInit();
        }

        static int check_pos = 0;

        public static void SystemInfoCheck()
        {
            if(threadMain == null)  return;

            if (bUseBoardAlarm && itemAlarm[check_pos] && itemCurr[check_pos] != -1)
            {
                if (itemCurr[check_pos] <= itemMin[check_pos])
                {
                    string message;

                    if(Tools.IsLangKorean()) 
                        message = String.Format("시스템 보드 경보 : {0} LOW 상태. 현재값={1}", itemName[check_pos], itemCurr[check_pos]);
                    else
                        message = String.Format("Board Alarm : {0} is too LOW. Current={1}", itemName[check_pos], itemCurr[check_pos]);

                    MessageDisplay.Show(message);
                }
                if (itemCurr[check_pos] >= itemMax[check_pos])
                {
                    string message;

                    if(Tools.IsLangKorean())
                        message = String.Format("시스템 보드 경보 : {0} HIGH 상태. 현재값={1}", itemName[check_pos], itemCurr[check_pos]);
                    else
                        message = String.Format("Board Alarm : {0} is too HIGH. Current={1}", itemName[check_pos], itemCurr[check_pos]);

                    MessageDisplay.Show(message);
                }
            }

            check_pos++;
            check_pos %= MAX_ITEM;
        }

        public static void SystemInfoUninit()
        {
            if (threadMain == null) return;	// 

            bEnd = true;
            threadMain.Join(5000);
        }
    }
}
