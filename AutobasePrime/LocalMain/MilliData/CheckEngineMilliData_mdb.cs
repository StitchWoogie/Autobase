using System;
using System.Collections;
using AutoLib;
using AutoLibLocal;
using NetTools.OldDefine;
using System.Data;
using System.IO;
using NetTools;
using GraphicModule;
using System.Threading;

namespace LocalMain
{
    /// <summary>
    /// Summary description for CheckEngineMilliData.
    /// </summary>
    public class CheckEngineMilliData_mdb
    {
        static CheckEngineMilliData_mdb()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public static void Init()
        {
            // 저장 속도를 올리기 위해서 쓰레드인 경우 쓰레드로 빼면 되지만 어차피 태그값이 갱신되지 않으면 의미가 없다. 당분간 쓰레드는 보류한다. 2019-10-17
            //for (int i = 0; i < MilliData.blockMilliData.Count; i++)
            //{
            //    MILLI_DATA_STRUCT item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[i];
            //    if (item.bThread)
            //    {
            //        Thread thread = new Thread(new ParameterizedThreadStart(ThreadProc));
            //        thread.Start(MilliData.blockMilliData[i]);
            //    }
            //}
        }

        public static void UnInit()
        {
            m_Done = true;
            TimeOutClass timeout = new TimeOutClass();
            while (nThreadCount > 0)
            {
                Thread.Sleep(1);
                if (timeout.IsTimeOut(3)) break;   // 너무 오랜시간이 지났다.
            }
            SaveRemainMilliData();
        }

        static bool m_Done = false;
        static int nThreadCount = 0;

        static void ThreadProc(object param)
        {
            nThreadCount++;
            MILLI_DATA_STRUCT item = (MILLI_DATA_STRUCT)param;

            while (!m_Done)
            {
                Thread.Sleep(10);
                CheckMilliDataOne(item);
            }
            nThreadCount--;
        }

        static int nCheckPos = 0;

        static void CheckMilliDataOne(MILLI_DATA_STRUCT item)
        {
            //if (item.bThread)    return;            
            if (item.bErrorFlag) return;			// 자료 저장에 문제가 있다.
            if (item.nCondition == 1)
            {				// DI ON동안만 수집
                CheckMilliDataOneByDI(item);
            }
            else if (item.nCondition == 2)
            {			// 계속 수집
                CheckMilliDataOneByContinue(item);
            }
        }

        public static void CheckMilliData()
        {
            //return;
            if (MilliData.blockMilliData.Count == 0) return;	// 미세 자료 감시가 없다.

            MILLI_DATA_STRUCT item;
            int l;
            TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

            for (l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                if (timeout.IsTimeOut(100))     // 감시 프로그램이 너무 부하가 걸리지 않게 한다. 2010-7-20 200->100으로 줄임
                {
                    break;
                }

                DebugSpeed speed = new DebugSpeed();
                speed.Start();
                nCheckPos %= MilliData.blockMilliData.Count;

                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[nCheckPos];
                CheckMilliDataOne(item);

                nCheckPos++;
                speed.Stop("MilliData");
            }
        }

        static void CheckMilliDataOneByDI(MILLI_DATA_STRUCT item)
        {
            if (item.bErrorFlag) return;

            TagDiClass di = TagLib.GetStructDI(item.tagCheckDI, ref item.tag_pos);
            SYSTEMTIME time = new SYSTEMTIME();

            if (di.curr == 1 && item.old_di_curr == 0)
            {	// 발생.
                if (item.bSavingFlag == 0)
                {
                    //if(di.curr == OFF)	return;
                    time.GetLocalTime();
                    MakeHeaderAndField(item, time);
                }
            }
            else if (di.curr == 0 && item.old_di_curr == 1)
            {	// 복귀
                if (item.bSavingFlag == 1)
                {
                    item.bSavingFlag = 0;
                    item.DbClose();
                }
            }
            else if (di.curr == 1)
            {	// 진행 중.
                if (item.bSavingFlag == 1)
                {
                    time.GetLocalTime();

                    CuttingCheck(item, time);   // 일정한 크기로 잘라준다. 2009.11.17
                }
            }
            else { }

            item.old_di_curr = di.curr;
        }

        static void CuttingCheck(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            // 시간이 꺼꾸로 흘렀다. 시간 동기화 때문에 발생할 수 있다. 남아 있는 데이터는 저장하지 않는것이 좋을 듯... 2013.1.17
            if (SYSTEMTIME.CompareTime(time, item.old_time) < 0)
            {
                item.bSavingFlag = 0;
                item.DbClose();
                return;
            }

            if (item.nCutMethod == 0)
            {	// 초
                if ((time.wSecond % item.nSizeCut) == 0)
                {	// Time to Cut
                    if (time.wSecond != item.stStart.wSecond || time.wMinute != item.stStart.wMinute)
                    {
                        time.wMilliseconds = 0;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            else if (item.nCutMethod == 1)
            {	// 분
                if ((time.wMinute % item.nSizeCut) == 0)
                {	// Time to Cut
                    if (time.wMinute != item.stStart.wMinute || time.wHour != item.stStart.wHour)
                    {
                        time.wMilliseconds = 0;
                        time.wSecond = 0;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            else if (item.nCutMethod == 2)
            {	// hour
                if ((time.wHour % item.nSizeCut) == 0)
                {	// Time to Cut
                    if (time.wHour != item.stStart.wHour || time.wDay != item.stStart.wDay)
                    {
                        time.wMilliseconds = 0;
                        time.wSecond = 0;
                        time.wMinute = 0;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            else if (item.nCutMethod == 3)
            {	// day
                if (((time.wDay - 1) % item.nSizeCut) == 0)
                {	// Time to Cut
                    if (time.wDay != item.stStart.wDay || time.wMonth != item.stStart.wMonth)
                    {
                        time.wMilliseconds = 0;
                        time.wSecond = 0;
                        time.wMinute = 0;
                        time.wHour = 0;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            else if (item.nCutMethod == 4)
            {	// week로 선택하면 무조건 1주씩만 저장된다.
                if (time.wDayOfWeek == 0)
                {	// Time to Cut. start is sunday
                    if (time.wDay != item.stStart.wDay || time.wDayOfWeek != item.stStart.wDayOfWeek)
                    {
                        time.wMilliseconds = 0;
                        time.wSecond = 0;
                        time.wMinute = 0;
                        time.wHour = 0;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            else if (item.nCutMethod == 5)
            {	// month
                if (((time.wMonth - 1) % item.nSizeCut) == 0)
                {	// Time to Cut
                    if (time.wMonth != item.stStart.wMonth || time.wYear != item.stStart.wYear)
                    {
                        time.wMilliseconds = 0;
                        time.wSecond = 0;
                        time.wMinute = 0;
                        time.wHour = 0;
                        time.wDay = 1;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            else if (item.nCutMethod == 6)
            {	// year
                if (((time.wYear - 1) % item.nSizeCut) == 0)
                {	// Time to Cut
                    if (time.wYear != item.stStart.wYear)
                    {
                        time.wMilliseconds = 0;
                        time.wSecond = 0;
                        time.wMinute = 0;
                        time.wHour = 0;
                        time.wDay = 1;
                        time.wMonth = 1;
                        ChangeNewDatabase(item, time);
                        return;
                    }
                }
            }
            RemainCheck(item, time);
        }

        static void CheckMilliDataOneByContinue(MILLI_DATA_STRUCT item)
        {
            if (item.bErrorFlag) return;

            SYSTEMTIME time = new SYSTEMTIME();

            if (item.bSavingFlag == 0)
            {	// first routine
                time.GetLocalTime();
                MakeHeaderAndField(item, time);
            }
            else
            {
                time.GetLocalTime();
                CuttingCheck(item, time);
            }
        }

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

            // 2023-7-27 추가
            if (item.bUseFilenameAddition)  
            {
                TagStClass st = TagLib.GetStructST(item.sUseFilenameAdditionTag, ref item.tag_posUseFilenameAddition);

                if (item.tag_posUseFilenameAddition[0] != TagLib.TAG_NOT_FOUND)
                {
                    string curr = st.curr.Trim();
                    if (curr.Length > 0)
                        filename = filename + curr;
                }
            }

            return filename;
        }

        //static bool bOpenEverySaving = true;        // 저장할때마다 열어서 사용한다. 2017-3-15 추가. 테스트 결과 사용불가. 파일이 많은 경우 열고 닫는데 몇초씩 걸림

        static void MakeHeaderAndFieldMdb(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            item.bDbOpenError = false;

            try
            {
                item.filename_mdb = String.Format("{0}\\{1}.mdb", MilliData.GetDataFolder(item), MakeFileName(item));

                bool existed = File.Exists(item.filename_mdb);

                MdbLib.MdbTool.MdbCreate(item.filename_mdb);

                string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", item.filename_mdb);
                item.db = new CommonDbConnection(EnumDbConnectionType.OleDb, dsn, false);

                item.DbOpen();

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
                    else if (member.tag_type == EnumTagType.ST)	// ST
                        check.AddColumn(MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(member.tag), EnumDbDataType.String, 255);    // MDB는 문자열을 255개만 허용한다.
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

                        if (member.tag_type == EnumTagType.AI)
                        {
                            TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                            command.CommandText = String.Format("INSERT INTO Member (Tag,[Full],Base,TagType) VALUES ('{0}','{1}','{2}','{3}')",
                                MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(ai.tag), ai.fFull, ai.fBase, 0);
                        }
                        else if (member.tag_type == EnumTagType.DI)
                        {	
                            TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                            command.CommandText = String.Format("INSERT INTO Member (Tag,[Full],Base,TagType) VALUES ('{0}','{1}','{2}','{3}')",
                                MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(di.tag), 100, 0, 2);
                        }
                        else if (member.tag_type == EnumTagType.ST)
                        {
                            TagStClass st = TagLib.GetStructST(member.tag, ref member.tag_pos);
                            command.CommandText = String.Format("INSERT INTO Member (Tag,[Full],Base,TagType) VALUES ('{0}','{1}','{2}','{3}')",
                                MILLI_DATA_STRUCT.ChangeToFitMdbColumnName(st.tag), 100, 0, 9);
                        }
                        command.ExecuteNonQuery();
                    }
                }

                /*
                if (bOpenEverySaving)
                {
                    item.DbClose();
                }*/
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("MilliData Error:Title={0}\nError Message={1}", item.title, exception.Message);
                item.bDbOpenError = true;
            }
        }

        static void MakeHeaderAndFieldCsv(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            try //250730 PSU try 밖으로 빼기.
            {
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
            catch (Exception exception)
            {
                MessageDisplay.Show("MilliData Make Header Error:Title={0}\nError Message={1}", item.title, exception.Message);
                //item.bDbOpenError = true;
            }
        }

        // 시간을 사이클에 맞는 시간으로 맞춘다...
        // 10.1.1 부터 지원
        static void FitTimeWithCycle(int cycle, SYSTEMTIME st)
        {
            int day_millisec = st.wHour * 60 * 60 * 1000 + st.wMinute * 60 * 1000 + st.wSecond * 1000 + st.wMilliseconds;

            // millisec check
            if (cycle != 0)
            {
                day_millisec = (day_millisec / cycle) * cycle;
                st.wHour = (ushort)(day_millisec / (60 * 60 * 1000));
                day_millisec %= (60 * 60 * 1000);
                st.wMinute = (ushort)(day_millisec / (60 * 1000));
                day_millisec %= (60 * 1000);
                st.wSecond = (ushort)(day_millisec / (1000));
                day_millisec %= 1000;
                st.wMilliseconds = (ushort)day_millisec;
            }
        }

        static void MakeHeaderAndField(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            string dir;

            item.bSavingFlag = 1;
            item.dwRecordCount = 0;
            item.nRemainedTime = 0;

            SYSTEMTIME.memcpy(item.old_time, time);

            FitTimeWithCycle(item.nGab, item.old_time); // 시간을 사이클에 맞는 시간으로 맞춘다... 10.1.1 부터 지원

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
            if (item.bDbOpenError)
            {
                return; // MDB Column과 헤더만들때 오류가 났다.
            }

            if (item.db == null) return;    // 앞에서 무슨 이유로 해서 초기화를 하지 못했다.

            no = GetNumber(no, item);

            item.DbOpen();  // 저장할 때마다 여는 경우가 있으므로 열려있지 않으면 연다.

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
                else if (member.tag_type == EnumTagType.ST)
                {
                    TagStClass st = TagLib.GetStructST(member.tag, ref member.tag_pos);
                    values += String.Format(",'{0}'", DbTool.ConvertStringValue(EnumDbType.MDB, st.curr));
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

                /*
                // 저장할때마다 열어서 사용하는 경우는 닫아준다.
                if (bOpenEverySaving)
                    item.DbClose();*/
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
                MessageDisplay.Show("MilliData Add Row Error:Title={0}\nError Message={1}", item.title, exception.Message);
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
                else if (member.tag_type == EnumTagType.ST)
                {
                    // 문자열 태그는 저자하는 부분이 없어서 2012-6-20 추가했다.
                    TagStClass st = TagLib.GetStructST(member.tag, ref member.tag_pos);

                    string curr = st.curr;

                    if (CommaTextWriter.IsExistBlockCode(curr))
                        curr = CommaTextWriter.MakeString(curr);

                    writer.Write("{0},", curr);
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
            long curr = time.GetMilliSecHap() - item.old_time.GetMilliSecHap();

            // 1분이 넘었다. 너무 많은 시간 (1시간) 이 흐르면 데이터를 저장하지 않는다. 2013.1.17
            if(curr < 0 || curr > 3600000) 
            {
                item.bSavingFlag = 0;
                item.DbClose();
                return;
            }
     
            /*
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
            }*/

            SYSTEMTIME.memcpy(item.old_time, time);

            item.nRemainedTime += (int)curr;
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
            item.DbClose();
            MakeHeaderAndField(item, time);
        }

        static void SaveRemainMilliData()
        {
            if (MilliData.blockMilliData.Count == 0) return;	// 미세 자료 감시가 없다.

            MILLI_DATA_STRUCT item;
            int l;

            for (l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[l];
                if (item.bErrorFlag) continue;   // 자료 저장에 문제가 있다.
                if (item.bSavingFlag == 0) continue;   // 자료 저장중이 아니다.
                item.DbClose();
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
                            SmLog.LogInfo(LogCategory.DATA_DELETE, "저장 기한이 지난 {0} 파일을 삭제.", filename);
                        }
                        else
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, "{0} file is deleted because save date expired.", filename);
                        }
                    }
                    catch
                    {
                        SmLog.LogError(LogCategory.DATA_DELETE, "Error:Can't delete {0}.", filename);
                    }
                }
            }
        }


    }
}

