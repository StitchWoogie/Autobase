using System;
using System.Collections;
using AutoLib;
using AutoLibLocal;
using NetTools.OldDefine;
using System.Data;
using System.IO;
using NetTools;
using GraphicModule;
using System.Collections.Generic;

namespace LocalMain
{
    /// <summary>
    /// Summary description for CheckRealTimeTestData.
    /// </summary>
    public class CheckRealTimeTestData
    {
        static CheckRealTimeTestData()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static RealTimeTestTag SeekTagMember(string tag, List<RealTimeTestTag> block)
        {
            for (int i = 0; i < block.Count; i++)
            {
                if (block[i].tag == tag) return block[i];
            }

            return null;
        }

        public static void Init()
        {
            RealTimeTestData.LoadRealTimeTestData(RealTimeTestData.blockRealTestData);

            TextReader reader;
            string dir;
            string filename;
            RealTimeTestStruct item;
            CommaBlockString comma = new CommaBlockString();
            string one_line;
            string buf="";
            int flag=0;

            for (int l = 0; l < RealTimeTestData.blockRealTestData.Count; l++)
            {
                item = (RealTimeTestStruct)RealTimeTestData.blockRealTestData[l];

                dir = RealTimeTestData.GetDataFolder(item);
                filename = String.Format("{0}\\status_{1}.txt", dir, item.title);

                if (!File.Exists(filename)) continue;

                reader = new StreamReader(filename);
                if (reader == null) continue;

                while (true)
                {
                    one_line = reader.ReadLine();
                    if (one_line == null) break;

                    comma.Set(one_line);
                    comma.GetString(ref buf);
                    if (buf == "Status")
                    {
                        comma.GetInt(ref flag);
                        if (flag == 0) break;

                        item.bStartFlag = true;

                        comma.GetChar(ref item.bOldTagValue);
                        comma.GetInt(ref item.nOldMilliSec);
                        comma.GetInt(ref item.nRemainMilliSec);
                    }
                    else if (buf == "StartTime")
                    {
                        int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, milisecond = 0;

                        comma.GetInt(ref year);
                        comma.GetInt(ref month);
                        comma.GetInt(ref day);
                        comma.GetInt(ref hour);
                        comma.GetInt(ref minute);
                        comma.GetInt(ref second);
                        comma.GetInt(ref milisecond);

                        item.tStart = new DateTime(year, month, day, hour, minute, second, milisecond);
                    }
                    else if (buf == "DataCount")
                    {
                        comma.GetInt(ref item.nBufPos);
                    }
                    else if (buf == "Datas")
                    {
                        string tag = "";
                        comma.GetString(ref tag);

                        RealTimeTestTag realtag = SeekTagMember(tag, item.blockTag);

                        if (realtag != null)
                        {
                            for (int j = 0; j < item.nBufPos; j++)
                            {
                                comma.GetDouble(ref realtag.data[j]);
                            }
                        }
                    }
                    else if (buf == "Flags")
                    {
                        string tag = "";
                        comma.GetString(ref tag);

                        RealTimeTestTag realtag = SeekTagMember(tag, item.blockTag);

                        if (realtag != null)
                        {
                            for (int j = 0; j < item.nBufPos; j++)
                            {
                                comma.GetBYTE(ref realtag.flags[j]);
                            }
                        }
                    }
                }

                reader.Close();
            }
        }

        static void SaveStatusData(RealTimeTestStruct item)
        {
            string dir = RealTimeTestData.GetDataFolder(item);

            Directory.CreateDirectory(dir);

            string filename = String.Format("{0}\\status_{1}.txt", dir, item.title);

            TextWriter writer = new StreamWriter(filename);
            if (writer == null) return;

            writer.WriteLine("Status,{0},{1},{2},{3},", item.bStartFlag ? 1 : 0, item.bOldTagValue, item.nOldMilliSec, item.nRemainMilliSec);
            writer.WriteLine("StartTime,{0},{1},{2},{3},{4},{5},{6},", item.tStart.Year, item.tStart.Month, item.tStart.Day, item.tStart.Hour, item.tStart.Minute, item.tStart.Second, item.tStart.Millisecond);
            writer.WriteLine("DataCount,{0},", item.nBufPos);

            for (int j = 0; j < item.blockTag.Count; j++)
            {
                writer.Write("Datas,{0},", item.blockTag[j].tag);
                for (int k = 0; k < item.nBufPos; k++)
                {
                    writer.Write("{0},", item.blockTag[j].data[k]);
                }
                writer.WriteLine();

                writer.Write("Flags,{0},", item.blockTag[j].tag);
                for (int k = 0; k < item.nBufPos; k++)
                {
                    writer.Write("{0},", item.blockTag[j].flags[k]);
                }
                writer.WriteLine();
            }

            writer.Close();
        }

        public static void UnInit()
        {
            RealTimeTestStruct item;
            int l;

            for (l = 0; l < RealTimeTestData.blockRealTestData.Count; l++)
            {
                item = (RealTimeTestStruct)RealTimeTestData.blockRealTestData[l];

                SaveStatusData(item);
            }
        }

        public static void CheckRealTimeTest()
        {
            if (RealTimeTestData.blockRealTestData.Count == 0) return;	// 미세 자료 감시가 없다.

            RealTimeTestStruct item;
            int l;

            for (l = 0; l < RealTimeTestData.blockRealTestData.Count; l++)
            {
                item = (RealTimeTestStruct)RealTimeTestData.blockRealTestData[l];

                if (item.bErrorFlag) continue;			// 자료 저장에 문제가 있다.
                CheckMilliDataOneByDI(item);
            }
        }

        static void CheckMilliDataOneByDI(RealTimeTestStruct item)
        {
            if (item.bErrorFlag) return;

            TagDiClass di;

            di = TagLib.GetStructDI(item.tagRunDI, ref item.tag_pos_run);

            bool run_flag = true;

            if (item.tag_pos_run[0] == TagLib.TAG_NOT_FOUND)
            {

            }
            else
            {
                if (di.curr == 0)
                    run_flag = false;
            }
            
            di = TagLib.GetStructDI(item.tagCheckDI, ref item.tag_pos);

            DateTime st = DateTime.Now;

            // 분이 바뀌면 저장할 데이타가 있으면 저장한다.
            if (st.Minute != item.nBackupStatusOldMinute)
            {
                item.nBackupStatusOldMinute = st.Minute;
                if (item.nBackupStatusGatherRemain > 0)
                {
                    item.nBackupStatusGatherRemain = 0;
                    SaveStatusData(item);
                }
            }

            if (item.bStartFlag == false)
            {
                if (di.curr == 1)
                {
                    item.bStartFlag = true;
                    item.tStart = new DateTime(st.Ticks);
                    item.bOldTagValue = 1;
                    item.nOldMilliSec = st.Second * 1000 + st.Millisecond;
                    item.nRemainMilliSec = 0;
                    //nBufPos = nBufPosByScript;
                    //nBufPosByScript = 0;    // 1회만 적용한다.
                    //ReadFlagClear();
                    FillData(item, run_flag);
                    //InvalidateObject(form);
                    return;
                }
                return;
            }
            else
            {
                if (di.curr == 1)
                {
                    if (item.bOldTagValue == 0)
                    {	// 다시 시작 되었다.
                        item.bStartFlag = true;
                        item.bOldTagValue = 1;
                        item.nOldMilliSec = st.Second * 1000 + st.Millisecond;
                        item.nRemainMilliSec = 0;
                        item.nBufPos = 0;
                        //ReadFlagClear();
                        FillData(item, run_flag);
                        //InvalidateObject(form);
                        return;
                    }
                }
                else
                {
                    item.bOldTagValue = di.curr;
                }
            }

            if (item.nBufPos >= item.nSize)
            {	// 이미 범위를 벗어났다.
                return;
            }

            int curMilliSec = st.Second * 1000 + st.Millisecond;

            if (curMilliSec < item.nOldMilliSec)
            {	// 분이 바뀌었다.
                item.nRemainMilliSec += (curMilliSec + 60000) - item.nOldMilliSec;
            }
            else
            {
                item.nRemainMilliSec += curMilliSec - item.nOldMilliSec;
            }

            item.nOldMilliSec = curMilliSec;

            while (true)
            {
                if (item.nBufPos >= item.nSize)
                {	// 이미 범위를 벗어났다.
                    break;
                }

                if (item.nRemainMilliSec >= item.nGab)
                {
                    FillData(item, run_flag);
                    item.nRemainMilliSec -= item.nGab;
                    item.nBackupStatusGatherRemain++;
                }
                else
                {
                    break;
                }
            }

            /*
            if (data_flag)
            {
                // InvalidateObject(form);
            }

            
            if (item.bErrorFlag) return;

            TagDiClass di = TagLib.GetStructDI(item.tagCheckDI, ref item.tag_pos);
            SYSTEMTIME time = new SYSTEMTIME();

            
            if (di.curr == 1 && item.old_di_curr == 0)  // 시작됨
            {	
                if (item.bSavingFlag == 0)
                {
                    //if(di.curr == OFF)	return;
                    time.GetLocalTime();
                    //MakeHeaderAndField(item, time);
                }
            }
            else if (di.curr == 0 && item.old_di_curr == 1) // 종료됨
            {	
                if (item.bSavingFlag == 1)
                {
                    item.bSavingFlag = 0;
                    if (item.db != null && item.db.State == ConnectionState.Open) item.db.Close();
                }
            }
            else if (di.curr == 1)
            {	// 진행 중.
                if (item.bSavingFlag == 1)
                {
                    time.GetLocalTime();
                    RemainCheck(item, time);

                    long msec_curr = (long)item.dwRecordCount * item.nGab;
                    long msec_target;

                    if (item.nCutMethod == 0)
                    {	// sec
                        msec_target = (long)item.nSizeCut * 1000;
                    }
                    else if (item.nCutMethod == 1)
                    {	// minute
                        msec_target = (long)item.nSizeCut * ((long)1000 * 60);
                    }
                    else if (item.nCutMethod == 2)
                    {	// hour
                        msec_target = (long)item.nSizeCut * ((long)1000 * 60 * 60);
                    }
                    else if (item.nCutMethod == 3)
                    {	// day
                        msec_target = (long)item.nSizeCut * ((long)1000 * 60 * 60 * 24);
                    }
                    else if (item.nCutMethod == 4)
                    {	// week
                        msec_target = (long)item.nSizeCut * ((long)1000 * 60 * 60 * 24 * 7);
                    }
                    else if (item.nCutMethod == 5)
                    {	// month
                        msec_target = (long)item.nSizeCut * ((long)1000 * 60 * 60 * 24 * 31);
                    }
                    else if (item.nCutMethod == 6)
                    {	// year
                        msec_target = (long)item.nSizeCut * ((long)1000 * 60 * 60 * 24 * 365);
                    }
                    else
                    {	// year
                        msec_target = (long)item.nSizeCut * ((long)1000);
                    }

                    if (msec_curr >= msec_target)
                    {
                        item.bSavingFlag = 0;
                        if (item.db != null && item.db.State == ConnectionState.Open) item.db.Close();
                    }
                }
            }
            else { }
             */

            item.bOldTagValue = di.curr;
        }

        static void FillData(RealTimeTestStruct item, bool run_flag)
        {
            if (item.nBufPos >= item.nSize)
            {	// 이미 범위를 벗어났다.
                return;
            }

            int l;
            RealTimeTestTag member;
            int buf_pos;

            for (l = 0; l < item.blockTag.Count; l++)
            {
                member = item.blockTag[l];

                buf_pos = item.nBufPos;

                if (buf_pos < 0) continue;
                if (buf_pos >= item.nSize) continue;

                if (member.data != null)
                {
                    if (member.tag_type == 0)
                    { 	// AI TAG
                        TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                        member.data[buf_pos] = ai.curr;
                        member.flags[buf_pos] = run_flag ? (byte)1 : (byte)0;
                    }
                    else
                    {
                        TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                        member.data[buf_pos] = di.curr;
                        member.flags[buf_pos] = run_flag ? (byte)1 : (byte)0;
                    }
                }
            }

            item.nBufPos++;
        }

        /*
        // 날짜/시간 별로 정리 기능을 사용한다.
        static string MakeFileName(MILLI_DATA_STRUCT item)
        {
            int year = item.stStart.wYear;
            int month = item.stStart.wMonth;
            int day = item.stStart.wDay;
            int hour = item.stStart.wHour;
            int minute = item.stStart.wMinute;
            int second = item.stStart.wSecond;

            DateTime t = DateTime.Now;

            if (item.bDateTimeMatch)
            {
                if (item.nCutMethod == 0)
                {	// 초
                    second = (t.Second / item.nSizeCut) * item.nSizeCut;
                }
                else if (item.nCutMethod == 1)
                {	// 분
                    second = 0;
                    minute = (t.Minute / item.nSizeCut) * item.nSizeCut;
                }
                else if (item.nCutMethod == 2)
                {	// hour
                    second = 0;
                    minute = 0;
                    hour = (t.Hour / item.nSizeCut) * item.nSizeCut;
                }
                else if (item.nCutMethod == 3)
                {	// day
                    second = 0;
                    minute = 0;
                    hour = 0;
                    day = ((t.Day - 1) / item.nSizeCut) * item.nSizeCut + 1;
                }
                else if (item.nCutMethod == 4)
                {	// week로 선택하면 무조건 1주씩만 저장된다.
                    second = 0;
                    minute = 0;
                    hour = 0;
                    day = 1;
                    month = ((t.Month - 1) / item.nSizeCut) * item.nSizeCut + 1;
                }
                else if (item.nCutMethod == 5)
                {	// month
                    second = 0;
                    minute = 0;
                    hour = 0;
                    day = 1;
                    month = ((t.Month - 1) / item.nSizeCut) * item.nSizeCut + 1;
                }
                else if (item.nCutMethod == 6)
                {	// year
                    second = 0;
                    minute = 0;
                    hour = 0;
                    day = 1;
                    month = 1;
                    year = ((t.Year - 1) / item.nSizeCut) * item.nSizeCut + 1;
                }
            }

            string filename = String.Format("{0:0000}{1:00}{2:00}_{3:00}{4:00}{5:00}", year, month, day, hour, minute, second);

            return filename;
        }

        static void MakeHeaderAndFieldMdb(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            item.bMdbOpenError = false;

            try
            {
                //item.filename_mdb = String.Format("{0}\\MiliData\\{1}\\{2}.mdb", ConfigData.sDirData, item.title, MakeFileName(item) );
                item.filename_mdb = String.Format("{0}\\{1}.mdb", MilliData.GetDataFolder(item), MakeFileName(item));

                bool existed = File.Exists(item.filename_mdb);

                MdbLib.MdbTool.MdbCreate(item.filename_mdb);

                string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", item.filename_mdb);
                item.db = new CommonDbConnection(EnumDbConnectionType.OleDb, dsn);

                item.db.Open();

                CheckTable check = new CheckTable();

                check.AddColumn("Data Count", EnumDbDataType.Integer, 0);
                check.AddColumn("Data Time", EnumDbDataType.DateTime, 0);
                check.AddColumn("MilliSec", EnumDbDataType.Integer, 0);

                MILLI_DATA_TAG member;
                int l;

                for (l = 0; l < item.blockTag.Count; l++)
                {
                    member = (MILLI_DATA_TAG)item.blockTag[l];
                    if (member.tag_type == EnumTagType.DI)	// DI
                        check.AddColumn(MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(member.tag), EnumDbDataType.Integer, 0);
                    else
                        check.AddColumn(MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(member.tag), EnumDbDataType.Float, 0);
                }

                if (existed)		// 파일이 존재하면 무조건 테이블을 만들면 오류가 발생한다.
                    check.Check(item.db, EnumDbType.MDB, "Data");
                else
                    check.Create(item.db, EnumDbType.MDB, "Data");

                // header를 쓴다.
                check = new CheckTable();
                check.AddColumn("TimeInterval", EnumDbDataType.Integer, 0);
                check.AddColumn("StartTime", EnumDbDataType.DateTime, 0);
                if (existed)
                    check.Check(item.db, EnumDbType.MDB, "Header");
                else
                    check.Create(item.db, EnumDbType.MDB, "Header");

                check = new CheckTable();
                check.AddColumn("Tag", EnumDbDataType.String, 80);
                check.AddColumn("Full", EnumDbDataType.Float, 0);
                check.AddColumn("Base", EnumDbDataType.Float, 0);
                check.AddColumn("TagType", EnumDbDataType.Integer, 0);

                if (existed)
                    check.Check(item.db, EnumDbType.MDB, "Member");
                else
                    check.Create(item.db, EnumDbType.MDB, "Member");

                if (!existed)
                {
                    CommonDbCommand command = new CommonDbCommand(item.db.connType);
                    command.Connection = item.db;
                    command.CommandText = String.Format("INSERT INTO Header (TimeInterval,StartTime) VALUES ('{0}',{1})",
                        item.nGab, DbTool.MakeDateTimeString(EnumDbType.MDB, 0, item.stStart));
                    command.ExecuteNonQuery();

                    for (l = 0; l < item.blockTag.Count; l++)
                    {
                        member = (MILLI_DATA_TAG)item.blockTag[l];

                        if (member.tag_type == EnumTagType.DI)
                        {	// DI
                            TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                            command.CommandText = String.Format("INSERT INTO Member (Tag,[Full],Base,TagType) VALUES ('{0}','{1}','{2}','{3}')",
                                MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(di.tag), 100, 0, 2);
                        }
                        else
                        {
                            TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                            command.CommandText = String.Format("INSERT INTO Member (Tag,[Full],Base,TagType) VALUES ('{0}','{1}','{2}','{3}')",
                                MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(ai.tag), ai.fFull, ai.fBase, 0);
                        }
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception exception)
            {
                MessageDisplay.Show("MilliData Error:Title={0}\nError Message={1}", item.title, exception.Message);
                item.bMdbOpenError = true;
            }
        }

        static void MakeHeaderAndFieldCsv(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            //item.filename_csv = String.Format("{0}\\MiliData\\{1}\\{2}.csv", ConfigData.sDirData, item.title, MakeFileName(item));
            item.filename_csv = String.Format("{0}\\{1}.csv", MilliData.GetDataFolderCsv(item, time), MakeFileName(item));

            bool existed = File.Exists(item.filename_csv);

            // 헤더를 만든다.
            if (!existed)
            {
                TextWriter writer = new StreamWriter(item.filename_csv, true, System.Text.Encoding.Default);

                MILLI_DATA_TAG member;
                int l;

                writer.Write("No,DataTime,MilliSec,");

                for (l = 0; l < item.blockTag.Count; l++)
                {
                    member = (MILLI_DATA_TAG)item.blockTag[l];
                    writer.Write("{0},", member.tag);
                }

                writer.WriteLine();

                writer.Close();
            }
        }

        static void MakeHeaderAndField(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            string dir;

            item.bSavingFlag = 1;
            item.dwRecordCount = 0;
            item.nRemainedTime = 0;

            SYSTEMTIME.memcpy(item.old_time, time);

            item.old_time.wMilliseconds = 0;	// 모든 데이터가 0mmsec부터 시작하는것이 좋다.

            SYSTEMTIME.memcpy(item.stStart, item.old_time);
            SYSTEMTIME.memcpy(item.stRecord, item.old_time);

            dir = MilliData.GetDataFolder(item);

            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Can't create folder.\nFolder={0}\nError={1}", dir, exception.Message);
                return;
            }

            if (item.nSaveFileType == 1)
            {
                MakeHeaderAndFieldMdb(item, time);
                MakeHeaderAndFieldCsv(item, time);
            }
            else if (item.nSaveFileType == 2)
            {
                MakeHeaderAndFieldCsv(item, time);
            }
            else
            {
                MakeHeaderAndFieldMdb(item, time);
            }

        }

        static int GetNumber(int no, MILLI_DATA_STRUCT item)
        {
            if (item.bDateTimeMatch)
            {
                if (item.nCutMethod == 0)
                {	// 초
                    no = item.stRecord.wSecond % item.nSizeCut;
                }
                else if (item.nCutMethod == 1)
                {	// 분
                    no = item.stRecord.wSecond + (item.stRecord.wMinute % item.nSizeCut) * 60;
                }
                else if (item.nCutMethod == 2)
                {	// hour
                    no = item.stRecord.wSecond + item.stRecord.wMinute * 60 + (item.stRecord.wHour % item.nSizeCut) * 3600;
                }
                else if (item.nCutMethod == 3)
                {	// day
                    no = item.stRecord.wSecond + item.stRecord.wMinute * 60 + item.stRecord.wHour * 3600 + ((item.stRecord.wDay - 1) % item.nSizeCut) * 3600 * 24;
                }
                else if (item.nCutMethod == 4)
                {	// week로 선택하면 무조건 1주씩만 저장된다.
                    no = item.stRecord.wSecond + item.stRecord.wMinute * 60 + item.stRecord.wHour * 3600 + (item.stRecord.wDay - 1) * 3600 * 24 + ((item.stRecord.wMonth - 1) % item.nSizeCut) * 3600 * 24 * 31;
                }
                else if (item.nCutMethod == 5)
                {	// month
                    no = item.stRecord.wSecond + item.stRecord.wMinute * 60 + item.stRecord.wHour * 3600 + (item.stRecord.wDay - 1) * 3600 * 24 + ((item.stRecord.wMonth - 1) % item.nSizeCut) * 3600 * 24 * 31;
                }
                else if (item.nCutMethod == 6)
                {	// year
                    no = item.stRecord.wSecond + item.stRecord.wMinute * 60 + item.stRecord.wHour * 3600 + (item.stRecord.wDay - 1) * 3600 * 24 + (item.stRecord.wMonth - 1) * 3600 * 24 * 31 + ((item.stRecord.wYear - 1) % item.nSizeCut) * 3600 * 24 * 365;
                }

                no = (int)(no * 1000L / item.nGab);
            }

            return no;
        }

        static void AddOneRowMdb(int no, MILLI_DATA_STRUCT item)
        {
            if (item.bMdbOpenError)
            {
                return; // MDB Column과 헤더만들때 오류가 났다.
            }

            if (item.db == null) return;    // 앞에서 무슨 이유로 해서 초기화를 하지 못했다.

            no = GetNumber(no, item);

            CommonDbCommand command = new CommonDbCommand(item.db.connType);
            command.Connection = item.db;

            MILLI_DATA_TAG member;
            string columns = "";
            string values = "";

            columns += String.Format("[Data Count]");
            values += String.Format("{0}", no);

            columns += String.Format(",[Data Time]");
            values += String.Format(",{0}", DbTool.MakeDateTimeString(EnumDbType.MDB, 0, item.stRecord));

            columns += String.Format(",MilliSec");
            values += String.Format(",{0}", item.stRecord.wMilliseconds);

            for (int l = 0; l < item.blockTag.Count; l++)
            {
                member = (MILLI_DATA_TAG)item.blockTag[l];

                columns += String.Format(",{0}", DbTool.Field(EnumDbType.MDB, MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(member.tag)));

                if (member.tag_type == 0)
                {
                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                    values += String.Format(",{0}", ai.curr);
                }
                else if (member.tag_type == EnumTagType.DI)
                {
                    TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                    values += String.Format(",{0}", di.curr);
                }
                else
                {
                    values += String.Format(",{0}", 0);
                }
            }

            command.CommandText = String.Format("INSERT INTO Data ({0}) VALUES({1})", columns, values);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("MilliData Error:Title={0}\nError Message={1}", item.title, exception.Message);
            }
        }

        static void AddOneRowCsv(int no, MILLI_DATA_STRUCT item)
        {
            no = GetNumber(no, item);

            MILLI_DATA_TAG member;

            TextWriter writer;

            try
            {
                writer = new StreamWriter(item.filename_csv, true, System.Text.Encoding.Default);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("MilliData Error:Title={0}\nError Message={1}", item.title, exception.Message);
                return;
            }

            writer.Write("{0},", no);
            writer.Write("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00},", item.stRecord.wYear, item.stRecord.wMonth, item.stRecord.wDay, item.stRecord.wHour, item.stRecord.wMinute, item.stRecord.wSecond);
            writer.Write("{0},", (item.nGab % 1000) == 0 ? 0 : (int)(item.stRecord.wMilliseconds));

            for (int l = 0; l < item.blockTag.Count; l++)
            {
                member = (MILLI_DATA_TAG)item.blockTag[l];

                if (member.tag_type == 0)
                {
                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                    writer.Write("{0},", ai.curr);
                }
                else if (member.tag_type == EnumTagType.DI)
                {
                    TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                    writer.Write("{0},", di.curr);
                }
                else
                {
                    writer.Write("{0},", 0);
                }
            }
            writer.WriteLine();
            writer.Close();
        }

        static void RemainCheck(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            int curr;

            if (time.wSecond < item.old_time.wSecond)
            {	// 분이 바뀌었다.
                curr = ((time.wSecond + 60) * 1000 + time.wMilliseconds)
                    - (item.old_time.wSecond * 1000 + item.old_time.wMilliseconds);
            }
            else
            {
                curr = (time.wSecond * 1000 + time.wMilliseconds)
                    - (item.old_time.wSecond * 1000 + item.old_time.wMilliseconds);
            }

            SYSTEMTIME.memcpy(item.old_time, time);

            item.nRemainedTime += curr;
            if (item.nRemainedTime >= item.nGab)
            {
                int no;

                while (item.nRemainedTime >= item.nGab)
                {
                    no = item.dwRecordCount;
                    item.dwRecordCount++;

                    if (item.nSaveFileType == 1)
                    {
                        AddOneRowMdb(no, item);
                        AddOneRowCsv(no, item);
                    }
                    else if (item.nSaveFileType == 2)
                    {
                        AddOneRowCsv(no, item);
                    }
                    else
                    {
                        AddOneRowMdb(no, item);
                    }

                    item.nRemainedTime -= item.nGab;
                    TimeUtil.AddMilliSecond(item.stRecord, item.nGab);
                }
            }
        }

        static void ChangeNewDatabase(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            RemainCheck(item, time);
            if (item.db != null && item.db.State == ConnectionState.Open) item.db.Close();
            MakeHeaderAndField(item, time);
        }

        public static void SaveRemainMilliData()
        {
            if (MilliData.blockMilliData.Count == 0) return;	// 미세 자료 감시가 없다.

            MILLI_DATA_STRUCT item;
            int l;

            for (l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[l];
                if (item.bErrorFlag) continue;   // 자료 저장에 문제가 있다.
                if (item.bSavingFlag == 0) continue;   // 자료 저장중이 아니다.
                if (item.db != null && item.db.State == ConnectionState.Open)
                    item.db.Close();
            }
        }

        public static void CheckAutoDelete()
        {
            MILLI_DATA_STRUCT item;
            int l;
            DateTime t = DateTime.Now;

            long today_day = TimeUtil.GetDayHap(t.Year, t.Month, t.Day);

            for (l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[l];
                if (item.bAutoDelete == false) continue;

                DeleteOldData(item, today_day, item.nDaysOfAutoDelete, "*.mdb");
                DeleteOldData(item, today_day, item.nDaysOfAutoDelete, "*.csv");
            }
        }

        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 로그 데이터를 지운다.
        //------------------------------------------------------------------------------

        static void DeleteOldData(MILLI_DATA_STRUCT item, long today_day, int limit_day, string ext)
        {
            int count = 0;
            string path;
            string filename;
            int year, month, day;
            long file_day;

            //path = String.Format("{0}\\MiliData\\{1}", ConfigData.sDirData, item.title);
            path = MilliData.GetDataFolder(item);

            if (!Directory.Exists(path)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(path);

            foreach (FileInfo fi in info.GetFiles(ext))
            {
                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(6, 2));

                file_day = TimeUtil.GetDayHap(year, month, day);

                if (today_day - file_day > limit_day)
                {
                    filename = fi.FullName;
                    try
                    {
                        File.Delete(filename);
                        count++;
                        if (Tools.IsLangKorean())
                        {
                            SmLog.Message("저장 기한이 지난 {0} 파일을 삭제.", filename);
                        }
                        else
                        {
                            SmLog.Message("{0} file is deleted because save date expired.", filename);
                        }
                    }
                    catch
                    {
                        SmLog.Message("Error:Can't delete {0}.", filename);
                    }
                }
            }
        }
        */

    }
}

