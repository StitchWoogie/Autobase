using System;
using AutoLibLocal;
using NetTools;
using AutoLib;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalMain.Alarm;

namespace LocalMain
{
    /// <summary>
    /// Summary description for AlarmDisplay.
    /// </summary>
    public class AlarmDisplay
    {
        public AlarmDisplay()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        class AlarmWaitClass
        {
            public ALARM_FILE_STRUCT afs;
            public TagPublicClass tp;
            public bool retn_or_event;
        }

        static List<AlarmWaitClass> arrayAlarm = new List<AlarmWaitClass>();

        const int MAX_WAIT_ALARM = 500;

        static void WaitAlarm(ALARM_FILE_STRUCT alarm, TagPublicClass tp, bool retn_or_event)
        {
            if (ConfigAlarm.bAlarmProtectAll) return;

            AlarmWaitClass awc = new AlarmWaitClass();
            awc.afs = alarm;
            awc.tp = tp;
            awc.retn_or_event = retn_or_event;

            // 경보를 저장해 놓고 시간날 때 경보를 디스프레이한다.
            lock (arrayAlarm)
            {
                if (arrayAlarm.Count >= MAX_WAIT_ALARM)
                {
                    arrayAlarm.RemoveAt(0); // 오래된 경보를 삭제하지 않으면 실제 경보 발생 시간이 너무 차이가 난다.
                }

                arrayAlarm.Add(awc);
            }
        }

        public static void WaitAlarmOnTimer()
        {
            if (arrayAlarm.Count == 0) return;

            /* 메시지가 경보 메시지 때문에 잘 보이지 않는다.
            if (arrayAlarm.Count >= MAX_WAIT_ALARM)
            {
                if (Tools.IsLangKorean())
                    MessageDisplay.Show("너무 많은 경보가 발생하고 있습니다.(Count={0})", arrayAlarm.Count);
                else
                    MessageDisplay.Show("Too many alarms are generated. (Count={0})", arrayAlarm.Count);
            }*/

            AlarmWaitClass awc;
            TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

            for(int i = 0; i < 100; i++) {
                // 빨리 꺼내고 닫는다.
                lock (arrayAlarm)
                {
                    if (arrayAlarm.Count == 0) return;
                    awc = arrayAlarm[0];
                    arrayAlarm.RemoveAt(0);
                }

                if (awc.tp.enumTagType == EnumTagType.AI)
                {
                    AlarmDisplayAIWithTime(awc.afs, (TagAiClass)awc.tp, awc.retn_or_event);
                }
                else if (awc.tp.enumTagType == EnumTagType.DI)
                {
                    AlarmDisplayDIWithTime(awc.afs, (TagDiClass)awc.tp, awc.retn_or_event);
                }

                if (timeout.IsTimeOut(100)) return;  // 메인이 너무 헤매지 않도록 한다.
            }
        }

        public static void AlarmDisplayAI(TagAiClass ai, string message, EnumAlarmType alarm_type, bool retn_or_event, string user, string ip, string computer)
        {
            ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();
            DateTime t;

            t = DateTime.Now;

            FillAlarmStruct(alarm, ai, t, message, alarm_type, user, ip, computer);

            if(TotalConfig.IsMainThread())
                AlarmDisplayAIWithTime(alarm, ai, retn_or_event);   // 같은 스레드일때는 혹시 알수 없으므로 원래대로 한다.
            else
                WaitAlarm(alarm, ai, retn_or_event);    // 쓰레드에서 사용할 수 있도록 경보를 버퍼에 저장해 놓고 Timer에서 처리한다. 2016-4-1
        }

        public static void AlarmDisplayDI(TagDiClass di, string message, EnumAlarmType alarm_type, bool retn_or_event, string user, string ip, string computer)
        {
            ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();
            DateTime t;

            t = DateTime.Now;

            FillAlarmStruct(alarm, di, t, message, alarm_type, user, ip, computer);

            if (TotalConfig.IsMainThread())
                AlarmDisplayDIWithTime(alarm, di, retn_or_event);   // 같은 스레드일때는 혹시 알수 없으므로 원래대로 한다.
            else
                WaitAlarm(alarm, di, retn_or_event);    // 쓰레드에서 사용할 수 있도록 경보를 버퍼에 저장해 놓고 Timer에서 처리한다. 2016-4-1
        }

        static void FillAlarmStructPublic(ALARM_FILE_STRUCT alarm, string tag, string tagdes, ushort priority, ushort port, ushort station, uint address, EnumTagType tagtype, DateTime t, string str, EnumAlarmType msg_type, string user, string ip, string computer)
        {
            // fill alarm structure
            alarm.t = new NetTools.OldDefine.SYSTEMTIME();

            alarm.t.wYear = (ushort)t.Year;
            alarm.t.wMonth = (ushort)t.Month;
            alarm.t.wDay = (ushort)t.Day;
            alarm.t.wHour = (ushort)t.Hour;
            alarm.t.wMinute = (ushort)t.Minute;
            alarm.t.wSecond = (ushort)t.Second;
            alarm.t.wMilliseconds = (ushort)t.Millisecond;

            alarm.tag = tag;
            alarm.description = tagdes;
            alarm.msg = str;
            alarm.alarm_type = (ushort)msg_type;
            alarm.priority = priority;
            alarm.port = (ushort)port;
            alarm.station = (ushort)station;
            alarm.alarm_sub_type = 0;
            alarm.address = (uint)address;
            alarm.enumTagType = tagtype;
            alarm.user = user;
            alarm.ip = ip;
            alarm.computer = computer;
        }

        static void FillAlarmStruct(ALARM_FILE_STRUCT alarm, TagAiClass ai, DateTime t, string str, EnumAlarmType msg_type, string user, string ip, string computer)
        {
            FillAlarmStructPublic(alarm, ai.tag, ai.description, ai.wAlarmPriority, (ushort)ai.port, (ushort)ai.station, (uint)ai.address, ai.enumTagType, t, str, msg_type, user, ip, computer);
        }

        static void FillAlarmStruct(ALARM_FILE_STRUCT alarm, TagAoClass ao, DateTime t, string str, EnumAlarmType msg_type, string user, string ip, string computer)
        {
            FillAlarmStructPublic(alarm, ao.tag, ao.description, 0, (ushort)ao.port, (ushort)ao.station, (uint)ao.address, ao.enumTagType, t, str, msg_type, user, ip, computer);
        }

        static void FillAlarmStruct(ALARM_FILE_STRUCT alarm, TagDiClass di, DateTime t, string str, EnumAlarmType msg_type, string user, string ip, string computer)
        {
            FillAlarmStructPublic(alarm, di.tag, di.description, di.wAlarmPriority, (ushort)di.port, (ushort)di.station, (uint)(di.address_word * 16 + di.address_bit), di.enumTagType, t, str, msg_type, user, ip, computer);
        }

        static void FillAlarmStruct(ALARM_FILE_STRUCT alarm, TagDoClass dout, DateTime t, string str, EnumAlarmType msg_type, string user, string ip, string computer)
        {
            FillAlarmStructPublic(alarm, dout.tag, dout.description, 0, (ushort)dout.port, (ushort)dout.station, (uint)(dout.address), dout.enumTagType, t, str, msg_type, user, ip, computer);
        }

        static void AlarmDisplayAIWithTime(ALARM_FILE_STRUCT alarm, TagAiClass ai, bool retn_or_event)
        {
            if (ConfigAlarm.bAlarmProtectAll) return;

            AlarmDisplayProtect(alarm, ai.sAlarmWaveFile, ai.wProtectFlags, ai);

            if ((ai.wProtectFlags & EnumProtectFlag.ALARM_EVENT) == 0)
            {	// 경보 EVENT 금지.
                Alarm.AlarmConfirm.AlarmContinueRegister(ai, alarm, retn_or_event);
            }

            if (retn_or_event)
            {	// 알람이 발생 했을 때
                DisplayAlarmGraphicFile(ai.sGraphicFile);

                AlarmToDigitalOut.CheckAlarmToDigitalOut((EnumAlarmType)alarm.alarm_type);
            }
            else
            {	// 알람이 복귀 되었을 때.

            }

            ai.bNeedAlarmConfirm = retn_or_event;
        }

        static void AlarmDisplayDIWithTime(ALARM_FILE_STRUCT alarm, TagDiClass di, bool retn_or_event)
        {
            if (ConfigAlarm.bAlarmProtectAll) return;

            AlarmDisplayProtect(alarm, di.sAlarmWaveFile, di.wProtectFlags, di);

            if ((di.wProtectFlags & EnumProtectFlag.ALARM_EVENT) == 0)	// 경보 EVENT 금지.
                Alarm.AlarmConfirm.AlarmContinueRegister(di, alarm, retn_or_event);

            if (retn_or_event)
            {	// 알람이 발생 했을 때
                DisplayAlarmGraphicFile(di.sGraphicFile);
                
                AlarmToDigitalOut.CheckAlarmToDigitalOut((EnumAlarmType)alarm.alarm_type);
            }
            else
            {

            }

            di.bNeedAlarmConfirm = retn_or_event;
        }

        static void AlarmDisplayProtect(ALARM_FILE_STRUCT alarm, string alarm_wave, EnumProtectFlag protect_flags, TagPublicClass tp)
        {
            if (ConfigAlarm.bAlarmProtectAll) return;	// 전체 경보 금지가 되어 있다.

            if ((protect_flags & EnumProtectFlag.ALARM_EVENT) == 0)
            {
                if ((ConfigAlarm.dwAlarmFilterEvent & Tools.DWORD_MASK[alarm.alarm_type]) > 0)
                {
                    string imsi;
                    if (ConfigAlarm.bAlarmScreenFlag)   // 경보가 발생하면 화면을 띄울것이냐.
                    {	
                        // 2021-2-18 우선 순위별로 화면 경보가 있는데 이것이 적용이 안되었다. 0이면 띄우지 않도록 추가했다.
                        sbyte method = AlarmPriority.AlarmPriorityGetOptionScreen(alarm.priority);

                        if (method > 0) // 화면경보 없음이 아닐때만 띄어준다.
                        {
                            imsi = String.Format("{0}[{1}] {2}", alarm.tag, alarm.description, alarm.msg);
                            //MessageDisplay.Show(imsi);
                            MessageDisplay.ShowInThread(imsi);
                        }
                    }

                }

                if ((ConfigAlarm.dwAlarmFilterSound & Tools.DWORD_MASK[alarm.alarm_type]) > 0)
                {
                    AlarmSound.Play(alarm_wave, alarm.priority);
                }
            }

            if ((ConfigAlarm.dwAlarmFilterLinePrinter & Tools.DWORD_MASK[alarm.alarm_type]) > 0)
            {
                if (AlarmPriority.alarmPriority[alarm.priority].bLinePrint == 1)
                    LinePrinter.SendAlarm(alarm);
            }

            if ((ConfigAlarm.dwAlarmFilterSmsManager & Tools.DWORD_MASK[alarm.alarm_type]) > 0)
            {
                SmsManager.SendAlarm(alarm);
            }

            if ((ConfigAlarm.dwAlarmFilterMail & Tools.DWORD_MASK[alarm.alarm_type]) > 0)
            {
                LocalMain.Alarm.AlarmMail.SendAlarm(alarm);
            }

            if ((protect_flags & EnumProtectFlag.ALARM_DATA) == 0)
            {
                if ((ConfigAlarm.dwAlarmFilterFile & Tools.DWORD_MASK[alarm.alarm_type]) > 0)
                {
                   AlarmDataSave(alarm);
                }
            }

            SendEventToChild.SendEventAlarmToChild(alarm);	// 알람이 발생 했을 때 IDM_EVENT_ALARM_SAVE 을 챠일드 윈도우에 보낸다.

        }

        //-----------------------------------------------------------------------------
        //	경보가 발생했을 때 해당되는 그래픽 파일을 보여준다.
        //-----------------------------------------------------------------------------

        static void DisplayAlarmGraphicFile(string modname)
        {
            if (ConfigAlarm.bDisplayGraphicFileOnAlarm == false) return;	// 경보가 발생해도 해당 그래픽 파일을 보여주지 않는다.

            if (modname.Length == 0) return;	// 설정된 그래픽 파일 없슴

            string filename;

            filename = String.Format("{0}\\graphic\\{1}", TotalConfig.sDirWorkProject, modname);
            if (!File.Exists(filename)) return;	// 해당 그래픽 파일 없슴

            GraphicModule.GraphicTool.RestoreGraphicWindow(filename, -1, 0, 0);
        }

        static void AlarmDataSave(ALARM_FILE_STRUCT alarm)
        {
            if ((ConfigAlarm.dwAlarmFilterFile & Tools.DWORD_MASK[alarm.alarm_type]) == 0) return;

            if (ConfigData.duplexDir.bUse == 1)
            {
                if (alarm.alarm_type == (ushort)EnumAlarmType.HAND_OPERATION)
                {

                }
                else
                {
                    return;
                }
            }

            string filename;
            string data_dir;


            if (ConfigData.duplexDir.bUse == 0)
            {
                data_dir = ConfigData.sDirData;
            }
            else
            {
                if (SystemStatusMemory.GetDI(SSMDI.DuplexActiveSecondary) == 1)
                {
                    data_dir = ConfigData.duplexDir.sSecondary;
                }
                else
                {
                    data_dir = ConfigData.duplexDir.sPrimary;
                }
            }

            filename = String.Format("{0}\\ALARM\\{1:0000}{2:00}{3:00}.{4}", data_dir, alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay, AlarmClass.ALARM_FILE_EXT);
            SaveToFile(alarm, filename, System.Text.Encoding.UTF8);

            // DB 저장 큐에 추가 
            AlarmProcessor.Instance.AddAlarm(alarm);

            if (ConfigAlarm.bAlarmAlsoSaveAsCsvFormat)
            {
                if (ConfigAlarm.bCsvSpecifyTargetFolder)
                {
                    filename = String.Format("{0}\\{1:0000}{2:00}{3:00}.CSV", ConfigAlarm.sCsvTargetFolder, alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
                }
                else
                {
                    filename = String.Format("{0}\\ALARM.CSV\\{1:0000}{2:00}{3:00}.CSV", data_dir, alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
                }
                SaveToFileByCsv(alarm, filename, System.Text.Encoding.Default);
            }

            SharedDatabase.WebServerAddAlarm(alarm);
        }

        static void SaveToFile(ALARM_FILE_STRUCT alarm, string filename, System.Text.Encoding encoding)
        {
            string dir = Path.GetDirectoryName(filename);

            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Can't create folder.\nFolder={0}\nError={1}", dir, exception.Message);
                return;
            }

            TextWriter writer;

            try
            {
                writer = new StreamWriter(filename, true, encoding);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Can't open file to write.\nfilename={0}\nError={1}", filename, exception.Message);
                return;
            }

            if (writer != null)
            {
                writer.Write("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000},", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay, alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond, alarm.t.wMilliseconds);
                writer.Write("{0},", alarm.tag);
                writer.Write("{0},", alarm.description);
                writer.Write("{0},", alarm.msg);
                writer.Write("{0},", alarm.alarm_type);
                writer.Write("{0},", alarm.priority);
                writer.Write("{0},", alarm.port);
                writer.Write("{0},", alarm.station);
                writer.Write("{0},", alarm.address);
                writer.Write("{0},", alarm.alarm_sub_type);
                writer.Write("{0},", alarm.user);
                writer.Write("{0},", alarm.ip);
                writer.Write("{0},", alarm.computer);
                writer.WriteLine();
                writer.Close();
            }
        }

        static void SaveToFileByCsv(ALARM_FILE_STRUCT alarm, string filename, System.Text.Encoding encoding)
        {
            string dir = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(dir);

            TextWriter writer;

            try
            {
                writer = new StreamWriter(filename, true, encoding);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Can't open file to write.\nfilename={0}\nError={1}", filename, exception.Message);
                return;
            }

            if (writer != null)
            {
                if (ConfigAlarm.bCsvSaveMillisecond)
                    writer.Write("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000},", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay, alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond, alarm.t.wMilliseconds);
                else
                    writer.Write("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00},", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay, alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond);

                if (ConfigAlarm.bCsvSaveTag)
                    writer.Write("{0},", alarm.tag);

                if (ConfigAlarm.bCsvSaveDescription)
                    writer.Write("{0},", alarm.description);

                if (ConfigAlarm.bCsvSaveMsg)
                    writer.Write("{0},", alarm.msg);

                if (ConfigAlarm.bCsvSaveAlarmType)
                    writer.Write("{0},", alarm.alarm_type);

                if (ConfigAlarm.bCsvSavePriority)
                    writer.Write("{0},", alarm.priority);

                if (ConfigAlarm.bCsvSavePort)
                    writer.Write("{0},", alarm.port);

                if (ConfigAlarm.bCsvSaveStation)
                    writer.Write("{0},", alarm.station);

                if (ConfigAlarm.bCsvSaveAddress)
                    writer.Write("{0},", alarm.address);

                if (ConfigAlarm.bCsvSaveAlarmSubType)
                    writer.Write("{0},", alarm.alarm_sub_type);

                writer.WriteLine();
                writer.Close();
            }
        }

        public static void AlarmDataSave(TagPublicClass tp, string msg, EnumAlarmType type, string user, string ip, string computer)
        {
            ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();

            DateTime t = DateTime.Now;

            if (tp.enumTagType == EnumTagType.AI)
                FillAlarmStruct(alarm, (TagAiClass)tp, t, msg, type, user, ip, computer);
            else if (tp.enumTagType == EnumTagType.AO)
                FillAlarmStruct(alarm, (TagAoClass)tp, t, msg, type, user, ip, computer);
            else if (tp.enumTagType == EnumTagType.DI)
                FillAlarmStruct(alarm, (TagDiClass)tp, t, msg, type, user, ip, computer);
            else if (tp.enumTagType == EnumTagType.DO)
                FillAlarmStruct(alarm, (TagDoClass)tp, t, msg, type, user, ip, computer);
            else
                return;

            AlarmDataSave(alarm);
        }

        public static void AlarmDataSaveGeneral(string tag, string description, string msg, EnumAlarmType type, EnumAlarmSubType sub_type)
        {
            ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();

            DateTime t = DateTime.Now;

            alarm.t = new NetTools.OldDefine.SYSTEMTIME();

            alarm.t.wYear = (ushort)t.Year;
            alarm.t.wMonth = (ushort)t.Month;
            alarm.t.wDay = (ushort)t.Day;
            alarm.t.wHour = (ushort)t.Hour;
            alarm.t.wMinute = (ushort)t.Minute;
            alarm.t.wSecond = (ushort)t.Second;
            alarm.t.wMilliseconds = (ushort)t.Millisecond;

            alarm.tag = tag;
            alarm.description = description;
            alarm.msg = msg;

            alarm.alarm_type = (ushort)type;
            alarm.priority = 0;
            alarm.port = 0;
            alarm.station = 0;
            alarm.alarm_sub_type = (ushort)sub_type;
            alarm.address = 0;
            alarm.enumTagType = 0;
            
            AlarmDataSave(alarm);
        }
    }
}


