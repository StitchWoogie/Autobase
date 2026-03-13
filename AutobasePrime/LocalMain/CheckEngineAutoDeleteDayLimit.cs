using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using AutoLibLocal;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain
{
    class CheckEngineAutoDeleteDayLimit
    {
        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 데이터를 모두 지운다.
        //------------------------------------------------------------------------------

        public static async Task DeleteOldDataByMonthLimitAsync(CancellationToken cancellationToken)
        {
            if (ConfigData.bDataUseAutoDelete == false) return;	// 자동 삭제 기능을 사용하지 않는다.
            if (ConfigData.nAutoDeleteMethod != 0) return;

            DateTime d = DateTime.Now;

            long today_day = TimeUtil.GetDayHap(d.Year, d.Month, d.Day);
            long today_mon = TimeUtil.GetMonHap(d.Year, d.Month);
            int limit_day = ConfigData.nMonthDataSave * 31;

            var _db = DataPostgres.Instance;
            await _db.PerformAutoDeleteAlarm(ConfigData.nMonthDataSave);
            await _db.PerformAutoDeleteHourData(ConfigData.nMonthDataSave);
            await _db.PerformAutoDeleteLog(ConfigData.nMonthDataSave);
            await  _db.PerformAutoDeleteMinData(ConfigData.nMonthDataSaveOfMin);

            BackupAndDelete backup = new BackupAndDelete();

            DeleteOldDataLog(backup, today_day, limit_day, cancellationToken);								// 저장 기한이 지난 로그 데이터를 지운다.
            DeleteOldDataAlarm(backup, today_day, limit_day, cancellationToken);							// 저장 기한이 지난 경보 데이터를 지운다.
            DeleteOldHourData(backup, d, today_mon, ConfigData.nMonthDataSave, cancellationToken);			// 저장 기한이 지난 데이터를 지운다.
            DeleteOldMinData(backup, d, today_mon, ConfigData.nMonthDataSaveOfMin, cancellationToken);	// 저장 기한이 지난 데이터를 지운다.

            backup.Close();
        }

        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 로그 데이터를 지운다.
        //------------------------------------------------------------------------------
        static void DeleteOldDataLog(BackupAndDelete backup, long today_day, int limit_day, CancellationToken token)
        {
            int count = 0;
            string seekfile;
            string filename;
            int year, month, day;
            long file_day;

            seekfile = String.Format("{0}\\log", ConfigData.sDirData);

            if (!Directory.Exists(seekfile)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(seekfile);

            foreach (FileInfo fi in info.GetFiles("*.log?"))
            {
                // if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (token.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(6, 2));

                file_day = TimeUtil.GetDayHap(year, month, day);

                if (today_day - file_day > limit_day)
                {
                    filename = String.Format("{0}\\log\\{1}", ConfigData.sDirData, fi.Name);
                    try
                    {
                        backup.Delete(ConfigData.sDirData, filename);
                        count++;
                        if (Tools.IsLangKorean())
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, "저장기한이 지난 {0} 파일을 삭제.", filename);
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

            if (Tools.IsLangKorean())
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "저장기한이 지난 Log 파일 총:{0}개를 삭제.", count);
            }
            else
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "Total:{0} log files deleted.", count);
            }
        }

        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 경보 데이터를 지운다.
        //------------------------------------------------------------------------------

        static void DeleteOldDataAlarm(BackupAndDelete backup, long today_day, int limit_day, CancellationToken token)
        {
            int count = 0;
            string seekfile;
            string filename;
            int year, month, day;
            long file_day;

            seekfile = String.Format("{0}\\alarm", ConfigData.sDirData);

            if (!Directory.Exists(seekfile)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(seekfile);

            foreach (FileInfo fi in info.GetFiles("*." + AlarmClass.ALARM_FILE_EXT))
            {
                // if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (token.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(6, 2));

                file_day = TimeUtil.GetDayHap(year, month, day);

                if (today_day - file_day > limit_day)
                {
                    filename = String.Format("{0}\\alarm\\{1}", ConfigData.sDirData, fi.Name);
                    try
                    {
                        backup.Delete(ConfigData.sDirData, filename);
                        count++;
                        if (Tools.IsLangKorean())
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, "저장기한이 지난 {0} 파일을 삭제.", filename);
                        }
                        else
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, "{0} file is deleted because save date expired.", filename);
                        }
                    }
                    catch
                    {
                        SmLog.LogError(LogCategory.DATA_DELETE, "Error:Can't delete {0} file.", filename);
                    }

                }
            }
            if (Tools.IsLangKorean())
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "저장 기한이 지난 경보 파일 총:{0}개를 삭제.", count);
            }
            else
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "Total:{0} Alarm files deleted.", count);
            }
        }

        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 저장 데이터를 지운다.
        //------------------------------------------------------------------------------

        static void DeleteOldHourData(BackupAndDelete backup, DateTime d, long today_mon, int limit_mon, CancellationToken token)
        {
            int count = 0;
            string seekfile;
            int year, month;
            long file_mon;
            int i;

            // 10년 전의 데이터부터 검사.
            for (i = -10; i <= 0; i++)
            {
                // if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (token.IsCancellationRequested) return;

                year = d.Year + i;
                seekfile = String.Format("{0}\\sum\\{1:0000}", ConfigData.sDirData, year);

                if (!Directory.Exists(seekfile)) continue;	// year directory not exist

                for (month = 1; month <= 12; month++)
                {
                    //  if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                    if (token.IsCancellationRequested) return;

                    file_mon = TimeUtil.GetMonHap(year, month);
                    if (today_mon - file_mon > limit_mon)
                    {

                        // 여기는 3.04 version 이후의 데이터가 저장되는 곳이다. 달로 구분된다.
                        seekfile = String.Format("{0}\\sum\\{1:0000}\\MON{2:00}", ConfigData.sDirData, year, month);
                        count += backup.DeleteDirAndFile(ConfigData.sDirData, seekfile, token);
                    }
                }
            }

            if (Tools.IsLangKorean())
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "저장 기한이 지난 시간 자료 데이터 파일 {0}개를 삭제.", count);
            }
            else
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "Total:{0} Hour Data files deleted.", count);
            }
        }

        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 저장 데이터를 지운다.
        //------------------------------------------------------------------------------

        static void DeleteOldMinData(BackupAndDelete backup, DateTime d, long today_mon, int limit_mon, CancellationToken token)
        {
            int count = 0;
            string seekfile;
            int year, month;
            long file_mon;
            int i;
            bool delete_start = false;

            // 10년 전의 데이터부터 검사.
            for (i = -10; i <= 0; i++)
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (token.IsCancellationRequested) return;

                year = d.Year + i;
                seekfile = String.Format("{0}\\trend\\{1:0000}", ConfigData.sDirData, year);

                if (!Directory.Exists(seekfile)) continue;	// year directory not exist

                for (month = 1; month <= 12; month++)
                {
                    // if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                    if (token.IsCancellationRequested) return;

                    file_mon = TimeUtil.GetMonHap(year, month);
                    if (today_mon - file_mon > limit_mon)
                    {
                        if (!delete_start)
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, "TREAND Data Backup/Delete Engine is started.", count);
                            delete_start = true;
                        }
                        // 여기는 3.04 version 이후의 데이터가 저장되는 곳이다. 달로 구분된다.
                        seekfile = String.Format("{0}\\trend\\{1:0000}\\MON{2:00}", ConfigData.sDirData, year, month);
                        count += backup.DeleteDirAndFile(ConfigData.sDirData, seekfile, token);
                    }
                }
            }

            if (Tools.IsLangKorean())
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "저장 기한이 지난 분 자료 데이터 파일 {0}개를 삭제.", count);
            }
            else
            {
                if (count > 0) SmLog.LogInfo(LogCategory.DATA_DELETE, "Total:{0} MIN Data files deleted.", count);
            }
        }
    }
}
