using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using AutoLibLocal;
using NetTools;
using GraphicModule;
using System.Threading.Tasks;
using System.Threading;

namespace LocalMain
{
    class CheckEngineAutoDeleteUsedSize
    {
        public static void CheckAutoDelete()
        {
            if (ConfigData.bDataUseAutoDelete == false) return;	// 자동삭제 기능을 사용하지 않는다.

            if (ConfigData.nAutoDeleteMethod == 1)
            {
                double used_percent = FormConfigData.GetDiskUsedPercent(ConfigData.sDirData);

                if (used_percent >= ConfigData.nAutoDeleteUsedSizeAlarm)
                {
                    string msg;
                    string root = Path.GetPathRoot(ConfigData.sDirData);
                    
                    if(Tools.IsLangKorean())
                        msg = String.Format("디스크[{0}] 사용량이 현재 {1:F1}% 입니다.\n사용량이 {2}%가 되면 자동 삭제가 시작됩니다.", root, used_percent, ConfigData.nAutoDeleteUsedSizeStart);
                    else
                        msg = String.Format("Disk[{0}] used size is {1:F1}%.\nAuto Deleting will be started at {2} %", root, used_percent, ConfigData.nAutoDeleteUsedSizeStart);

                    MessageDisplay.Show(msg);
                }
                if (used_percent >= ConfigData.nAutoDeleteUsedSizeStart)
                {
                    CheckEngineAutoDeleteThread.bGoAutoDeleteUsedSize = true;
                    //DeleteOldDataByDiskUsed();
                }
            }
            else
            {

            }
        }

        static bool bFlagGetOldestDate = false;
        static DateTime tOldestDate = DateTime.Now;

        static void GetOldestDateFile_YYYYMMDD(ref DateTime tOldest, string directory, string search, CancellationToken cancellationToken)
        {
            int year, month, day;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fi in info.GetFiles(search))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(6, 2));

                try
                {
                    DateTime t = new DateTime(year, month, day);
                    if (t < tOldest) tOldest = t;
                }
                catch
                {

                }
            }
        }

        static void GetOldestDateFile_YYYYMM(ref DateTime tOldest, string directory, string search, CancellationToken cancellationToken)
        {
            int year, month;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fi in info.GetFiles(search))
            {
               // if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));

                try
                {
                    DateTime t = new DateTime(year, month, 1);
                    if (t < tOldest) tOldest = t;
                }
                catch
                {

                }
            }
        }

        static void GetOldestDateDir_YYYY_MONMM(ref DateTime tOldest, string directory, CancellationToken cancellationToken)
        {
            int year, month;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (DirectoryInfo di in info.GetDirectories("*.*"))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(di.Name);

                if (year < 1900 || di.Name.Length != 4) continue;

                foreach (DirectoryInfo di2 in di.GetDirectories("*.*"))
                {
                    if (String.Compare(di2.Name, 0, "MON", 0, 3, true) == 0)
                    {
                        month = ConvertTool.ToInt32(di2.Name.Substring(3, 2));

                        try
                        {
                            DateTime t = new DateTime(year, month, 1);
                            if (t < tOldest) tOldest = t;
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        static void GetOldestDateFile_MiliData(ref DateTime tOldest, CancellationToken cancellationToken)
        {
            string directory;
            
            MILLI_DATA_STRUCT item;
            int i;

            for (i = 0; i < MilliData.blockMilliData.Count; i++)
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[i];

                directory = MilliData.GetDataFolder(item);

                GetOldestDateFile_YYYYMMDD(ref tOldestDate, directory, "*.*", cancellationToken);  // *.mdb, *.csv
            }
        }

        static void GetOldestDateFile_DATA_YYYYMMDD(ref DateTime tOldest, string directory, string search, CancellationToken cancellationToken)
        {
            int year, month, day;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fi in info.GetFiles(search))
            {
                // if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(5, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(9, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(11, 2));

                try
                {
                    DateTime t = new DateTime(year, month, day);
                    if (t < tOldest) tOldest = t;
                }
                catch
                {

                }
            }
        }

        static bool IsFolderDataDisk(string dir)
        {
            string root_data = Path.GetPathRoot(ConfigData.sDirData);
            string root_dir = Path.GetPathRoot(dir);

            if (String.Compare(root_data, root_dir, true) == 0)
            {
                return true;
            }

            return false;
        }

        // 제일 오래된 데이터의 날짜를 얻는다.
        static void GetDateOfOldest(CancellationToken cancellationToken)
        {
            if (bFlagGetOldestDate) return;

            string directory;

            if (IsFolderDataDisk(ConfigData.sDirDataLog))
            {
                directory = String.Format("{0}\\log", ConfigData.sDirDataLog);
                GetOldestDateFile_YYYYMMDD(ref tOldestDate, directory, "*.log?", cancellationToken);
            }

            directory = String.Format("{0}\\alarm", ConfigData.sDirData);
            GetOldestDateFile_YYYYMMDD(ref tOldestDate, directory, "*." + AlarmClass.ALARM_FILE_EXT, cancellationToken );

            directory = String.Format("{0}\\PlcScan", ConfigData.sDirData);
            GetOldestDateFile_YYYYMMDD(ref tOldestDate, directory, "*.log", cancellationToken);

            directory = String.Format("{0}\\database", ConfigData.sDirData);
            GetOldestDateFile_YYYYMM(ref tOldestDate, directory, "*.datx", cancellationToken);

            directory = String.Format("{0}\\TREND", ConfigData.sDirData);
            GetOldestDateDir_YYYY_MONMM(ref tOldestDate, directory, cancellationToken);

            directory = String.Format("{0}\\SUM", ConfigData.sDirData);
            GetOldestDateDir_YYYY_MONMM(ref tOldestDate, directory, cancellationToken);

            GetOldestDateFile_MiliData(ref tOldestDate, cancellationToken);

            if (IsFolderDataDisk(ConfigData.sBackupDirectory))
            {
                GetOldestDateFile_DATA_YYYYMMDD(ref tOldestDate, ConfigData.sBackupDirectory, "*.ZIP", cancellationToken);
            }

            bFlagGetOldestDate = true;
        }

        //------------------------------------------------------------------------------
        //	하루가 바뀌면 저장 기한이 지난 데이터를 모두 지운다.
        //------------------------------------------------------------------------------

        static int nOldSec;

        // Thread에서 불려진다. 2010-11-15
        public static async Task DeleteOldDataByDiskUsedAsync(CancellationToken cancellationToken)
        {
            if (ConfigData.bDataUseAutoDelete == false) return;	// 자동 삭제 기능을 사용하지 않는다.
            if (ConfigData.nAutoDeleteMethod != 1) return;

            GetDateOfOldest(cancellationToken);

            DateTime d = DateTime.Now;

            if (nOldSec == d.Second) return;    // 초가 바뀐 경우만 체크 너무 많은 삭제는 좋지 않다.
            nOldSec = d.Second;

            if (d.Year == tOldestDate.Year && d.Month == tOldestDate.Month) // 같은 달은 데이터를 지울 것이 없다.
            {
                return;
            }

            long today_days = TimeUtil.GetDayHap(tOldestDate);
            long today_months = TimeUtil.GetMonHap(tOldestDate);


            var _db = DataPostgres.Instance;

            await _db.PerformAutoDeleteOperationalTablesByDateTime(tOldestDate);

            for (int i = 0; i < MilliData.blockMilliData.Count; i++)
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                MILLI_DATA_STRUCT item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[i];

                await _db.PerformAutoDeleteMilliDataByUsedSize(item.tableName, tOldestDate);
            }
            

            BackupAndDelete backup = new BackupAndDelete();

            string directory;

            if (IsFolderDataDisk(ConfigData.sDirDataLog))
            {
                directory = String.Format("{0}\\log", ConfigData.sDirDataLog);
                DeleteOldDataFile_YYYYMMDD(backup, today_days, directory, "*.log?", cancellationToken);							// 저장 기한이 지난 로그 데이터를 지운다.
            }

            directory = String.Format("{0}\\alarm", ConfigData.sDirData);
            DeleteOldDataFile_YYYYMMDD(backup, today_days, directory, "*." + AlarmClass.ALARM_FILE_EXT, cancellationToken);	// 저장 기한이 지난 경보 데이터를 지운다.

            directory = String.Format("{0}\\PlcScan", ConfigData.sDirData);
            DeleteOldDataFile_YYYYMMDD(backup, today_days, directory, "*.log", cancellationToken);		                    // 저장 기한이 지난 PLCSCAN Log

            directory = String.Format("{0}\\database", ConfigData.sDirData);
            DeleteOldDataFile_YYYYMM(backup, today_days, directory, "*.datx", cancellationToken);		                    // 저장 기한이 지난 database 파일

            directory = String.Format("{0}\\TREND", ConfigData.sDirData);
            DeleteOldDataDir_YYYY_MONMM(backup, today_months, directory, cancellationToken);

            directory = String.Format("{0}\\SUM", ConfigData.sDirData);
            DeleteOldDataDir_YYYY_MONMM(backup, today_months, directory, cancellationToken);

            DeleteOldDataFile_MiliData(backup, today_days, cancellationToken);

            if (IsFolderDataDisk(ConfigData.sBackupDirectory))
            {
                DeleteOldDataFile_DATA_YYYYMMDD_NoneBackup(today_days, ConfigData.sBackupDirectory, "*.ZIP", cancellationToken);
            }


            backup.Close();

            tOldestDate = tOldestDate.AddDays(1);   // 하루씩 지운다.
        }

        //------------------------------------------------------------------------------
        // 기한이 지난 YYYYMMDD 파일을 지운다.
        //------------------------------------------------------------------------------
        static void DeleteOldDataFile_YYYYMMDD(BackupAndDelete backup, long today_days, string directory, string search, CancellationToken cancellationToken)
        {
            int count = 0;
            string filename;
            int year, month, day;
            long file_day;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fi in info.GetFiles(search))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다..
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(6, 2));

                file_day = TimeUtil.GetDayHap(year, month, day);

                if (file_day <= today_days)
                {
                    filename = fi.FullName;
                    try
                    {
                        backup.Delete(ConfigData.sDirData, filename);
                        count++;
                        if (Tools.IsLangKorean())
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE , "저장기한이 지난 {0} 파일을 삭제.", filename);
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

        //------------------------------------------------------------------------------
        // 기한이 지난 YYYYMMDD 파일을 지운다.
        //------------------------------------------------------------------------------
        static void DeleteOldDataFile_YYYYMM(BackupAndDelete backup, long today_days, string directory, string search, CancellationToken cancellationToken)
        {
            int count = 0;
            string filename;
            int year, month;
            long file_day;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fi in info.GetFiles(search))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));

                file_day = TimeUtil.GetDayHap(year, month, 1);

                if (file_day <= today_days)
                {
                    filename = fi.FullName;
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
        }

        static void DeleteOldDataDir_YYYY_MONMM(BackupAndDelete backup, long today_months, string directory, CancellationToken cancellationToken)
        {
            int year, month;
            long file_month;
            int count = 0;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (DirectoryInfo di in info.GetDirectories("*.*"))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(di.Name);

                if (year < 1900 || di.Name.Length != 4) continue;

                foreach (DirectoryInfo di2 in di.GetDirectories("*.*"))
                {
                    if (String.Compare(di2.Name, 0, "MON", 0, 3, true) == 0)
                    {
                        month = ConvertTool.ToInt32(di2.Name.Substring(3, 2));

                        file_month = TimeUtil.GetMonHap(year, month);

                        if (file_month <= today_months)
                        {
                            count += backup.DeleteDirAndFile(ConfigData.sDirData, di2.FullName, cancellationToken);

                            if (Tools.IsLangKorean())
                            {
                                SmLog.LogInfo(LogCategory.DATA_DELETE, "저장기한이 지난 {0} 폴더를 삭제.", di2.FullName);
                            }
                            else
                            {
                                SmLog.LogInfo(LogCategory.DATA_DELETE, "{0} Folder is deleted by AutoDelete Engine", di2.FullName);
                            }
                        }
                    }
                }
            }
        }

        static void DeleteOldDataFile_MiliData(BackupAndDelete backup, long today_days, CancellationToken cancellationToken)
        {
            string directory;

            MILLI_DATA_STRUCT item;
            int i;

            for (i = 0; i < MilliData.blockMilliData.Count; i++)
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[i];

                directory = MilliData.GetDataFolder(item);

                DeleteOldDataFile_YYYYMMDD(backup, today_days, directory, "*.*", cancellationToken);   // *.mdb, *.csv

            }
        }

        //------------------------------------------------------------------------------
        // 기한이 지난 DATA_YYYYMMDD 파일을 지운다.
        //------------------------------------------------------------------------------
        static void DeleteOldDataFile_DATA_YYYYMMDD_NoneBackup(long today_days, string directory, string search, CancellationToken cancellationToken)
        {
            int count = 0;
            string filename;
            int year, month, day;
            long file_day;

            if (!Directory.Exists(directory)) return;	// Alarm directory not exist

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fi in info.GetFiles(search))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return;

                year = ConvertTool.ToInt32(fi.Name.Substring(5, 4));
                month = ConvertTool.ToInt32(fi.Name.Substring(9, 2));
                day = ConvertTool.ToInt32(fi.Name.Substring(11, 2));

                file_day = TimeUtil.GetDayHap(year, month, day);

                if (file_day <= today_days)
                {
                    filename = fi.FullName;
                    try
                    {
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
        }

    }

    class BackupAndDelete
    {
        bool bOpenFlag = false;
        ZipOutputStream s = null;
        string sZipFilename;

        void OpenStream()
        {
            // 사용자가 삭제할 수 있으므로 새로 만든다.
            if (!Directory.Exists(ConfigData.sBackupDirectory)) Directory.CreateDirectory(ConfigData.sBackupDirectory);

            DateTime t = DateTime.Now;
            sZipFilename = String.Format("{0}\\DATA_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}.zip", ConfigData.sBackupDirectory, t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
            s = new ZipOutputStream(File.Create(sZipFilename));
            s.SetLevel(6);				// 0 - store only to 9 - means best compression
            bOpenFlag = true;
        }

        /// <summary>
        /// 파일을 ZIP 파일에 백업한 후, 실제 파일을 삭제하는 메서드
        /// 0바이트 파일은 백업되지 않음.
        /// ZIP 스트림(s)이 미리 열려 있어야 정상 동작함.
        /// root 경로가 잘못되면 Substring 부분에서 예외 발생할 수 있음.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filename"></param>
        public void Delete(string root, string filename)
        {
            if (!bOpenFlag)
            {
                OpenStream();
            }

            if (s != null)
            {
                Crc32 crc = new Crc32();
                FileInfo fi = new FileInfo(filename);

                byte[] buffer = File.ReadAllBytes(filename);

                string dir = fi.FullName.Substring(root.Length + 1);

                ZipEntry entry = new ZipEntry(dir);

                entry.DateTime = fi.LastWriteTime;
                entry.Size = fi.Length;
                entry.ExternalFileAttributes = (int)fi.Attributes;

                crc.Reset();
                crc.Update(buffer);

                entry.Crc = crc.Value;

                try
                {
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
                catch   // 0바이트일때는 저장할 수가 없다.
                {

                }
            }

            File.SetAttributes(filename, FileAttributes.Normal);
            File.Delete(filename);
        }

        public void Close()
        {
            if (bOpenFlag == false) return;
            if (s == null) return;

            try
            {
                s.Finish();
            }
            catch
            {
            }

            try
            {
                s.Close();
            }
            catch
            {

            }
        }

        public int DeleteDirAndFile(string root, string root_path, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(root_path)) return 0;

            DirectoryInfo info = new DirectoryInfo(root_path);

            int count = 0;

            foreach (DirectoryInfo di in info.GetDirectories("*.*"))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return count;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return count;

                count += DeleteDirAndFile(root, di.FullName, cancellationToken);
            }

            foreach (FileInfo fi in info.GetFiles("*.*"))
            {
                //if (CheckEngineAutoDeleteThread.bEnd) return count;    // 종료 명령어가 들어왔다.
                if (cancellationToken.IsCancellationRequested) return count;

                this.Delete(root, fi.FullName);
                count++;
            }

            // 디렉터리를 탐색기에서 열고 있으면 삭제가 되지 않고 Exception이 발생한다.
            try
            {
                Directory.Delete(root_path);
            }
            catch
            {

            }

            return count;
        }
    }
}
