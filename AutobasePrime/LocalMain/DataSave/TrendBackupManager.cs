using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocalMain
{

    #region 백업 데이터 구조

    /// <summary>
    /// JSON 백업용 AI 트렌드 데이터
    /// </summary>
    public class BackupTrendAI
    {
        public string TagName { get; set; }
        public DateTime DataTime { get; set; }
        public float SumMin { get; set; }
        public float Average { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float CurrValue { get; set; }
    }

    /// <summary>
    /// JSON 백업용 DI 트렌드 데이터
    /// </summary>
    public class BackupTrendDI
    {
        public string TagName { get; set; }
        public DateTime DataTime { get; set; }
        public short CountOnOff { get; set; }
        public bool OnOffState { get; set; }
        public byte OnTime { get; set; }
    }

    #endregion

    #region 백업 파일 관리자
    /// <summary>
    /// JSON 백업 파일 관리자
    /// </summary>
    public class TrendBackupManager
    {
        private readonly string _backupPath;
        private readonly int _maxBackupFiles = 100; // 한번에 저장할 개수만큼 json 생성됨. 설정창 필요.
        private readonly object _fileLock = new object();

        /// <summary>
        /// Data 저장폴더\프로젝트명 폴더에 트랜드백업자료를 생성한다.
        /// </summary>
        /// <param name="backupPath"></param>
        public TrendBackupManager(string backupPath = null)
        {
            string lastFolderName = Path.GetFileName(TotalConfig.sDirWorkProject.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            _backupPath = backupPath ?? Path.Combine( ConfigData.sDirData, "Backup", $"{lastFolderName}\\TrendData");

            Directory.CreateDirectory(_backupPath);
        }

        /// <summary>
        /// AI 트렌드 데이터를 JSON 파일로 백업
        /// </summary>
        public bool BackupAIData(List<(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)> dataList)
        {
            if (dataList == null || dataList.Count == 0) return true;

            try
            {
                var backupData = dataList.Select(d => new BackupTrendAI
                {
                    TagName = d.tagName,
                    DataTime = d.dataTime,
                    SumMin = d.trend.fSumMin,
                    Average = d.trend.fAverage,
                    MinValue = d.trend.fMin,
                    MaxValue = d.trend.fMax,
                    CurrValue = d.trend.fCurr
                }).ToList();

                string filename = Path.Combine(_backupPath,
                    string.Format("AI_Backup_{0:yyyyMMdd_HHmmss_fff}.json", DateTime.Now));

                lock (_fileLock)
                {
                    string json = JsonSerializer.Serialize(backupData, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    File.WriteAllText(filename, json, Encoding.UTF8);
                }

                Debug.WriteLine(string.Format("AI 트렌드 백업 완료: {0}개, File: {1}",
                    dataList.Count, Path.GetFileName(filename)));

                CleanupOldBackups();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("AI 백업 실패: {0}", ex.Message));
                SmLog.LogError(LogCategory.DATA_SAVE, "AI 트렌드 백업 실패", ex);
                return false;
            }
        }

        /// <summary>
        /// DI 트렌드 데이터를 JSON 파일로 백업
        /// </summary>
        public bool BackupDIData(List<(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)> dataList)
        {
            if (dataList == null || dataList.Count == 0) return true;

            try
            {
                var backupData = dataList.Select(d => new BackupTrendDI
                {
                    TagName = d.tagName,
                    DataTime = d.dataTime,
                    CountOnOff = d.trend.nCountOnOff,
                    OnOffState = d.trend.bOnOff != 0,
                    OnTime = d.trend.cOnTime
                }).ToList();

                string filename = Path.Combine(_backupPath,
                    string.Format("DI_Backup_{0:yyyyMMdd_HHmmss_fff}.json", DateTime.Now));

                lock (_fileLock)
                {
                    string json = JsonSerializer.Serialize(backupData, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    File.WriteAllText(filename, json, Encoding.UTF8);
                }

                Debug.WriteLine(string.Format("DI 트렌드 백업 완료: {0}개, File: {1}",
                    dataList.Count, Path.GetFileName(filename)));

                CleanupOldBackups();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("DI 백업 실패: {0}", ex.Message));
                SmLog.LogError(LogCategory.DATA_SAVE, "DI 트렌드 백업 실패", ex);
                return false;
            }
        }

        /// <summary>
        /// 백업 파일에서 AI 데이터 복구
        /// </summary>
        public List<(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)> LoadAIBackup(string filename)
        {
            var result = new List<(string, DateTime, TREND_AI_STRUCT)>();

            try
            {
                lock (_fileLock)
                {
                    string json = File.ReadAllText(filename, Encoding.UTF8);
                    var backupData = JsonSerializer.Deserialize<List<BackupTrendAI>>(json);

                    foreach (var item in backupData)
                    {
                        TREND_AI_STRUCT trend = new TREND_AI_STRUCT
                        {
                            fSumMin = item.SumMin,
                            fAverage = item.Average,
                            fMin = item.MinValue,
                            fMax = item.MaxValue,
                            fCurr = item.CurrValue
                        };
                        result.Add((item.TagName, item.DataTime, trend));
                    }
                }

                Debug.WriteLine(string.Format("AI 백업 로드 완료: {0}개", result.Count));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("AI 백업 로드 실패: {0}", ex.Message));
                SmLog.LogError(LogCategory.DATA_SAVE, "AI 백업 로드 실패", ex);
            }

            return result;
        }

        /// <summary>
        /// 백업 파일에서 DI 데이터 복구
        /// </summary>
        public List<(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)> LoadDIBackup(string filename)
        {
            var result = new List<(string, DateTime, TREND_DI_STRUCT)>();

            try
            {
                lock (_fileLock)
                {
                    string json = File.ReadAllText(filename, Encoding.UTF8);
                    var backupData = JsonSerializer.Deserialize<List<BackupTrendDI>>(json);

                    foreach (var item in backupData)
                    {
                        TREND_DI_STRUCT trend = new TREND_DI_STRUCT
                        {
                            nCountOnOff = item.CountOnOff,
                            bOnOff = (byte)(item.OnOffState ? 1 : 0),
                            cOnTime = item.OnTime
                        };
                        result.Add((item.TagName, item.DataTime, trend));
                    }
                }

                Debug.WriteLine(string.Format("DI 백업 로드 완료: {0}개", result.Count));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("DI 백업 로드 실패: {0}", ex.Message));
                SmLog.LogError(LogCategory.DATA_SAVE, "DI 백업 로드 실패", ex);
            }

            return result;
        }

        /// <summary>
        /// 오래된 백업 파일 정리
        /// </summary>
        private void CleanupOldBackups()
        {
            try
            {
                var files = Directory.GetFiles(_backupPath, "*.json")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(_maxBackupFiles)
                    .ToList();

                foreach (var file in files)
                {
                    try
                    {
                        file.Delete();
                        Debug.WriteLine(string.Format("오래된 백업 삭제: {0}", file.Name));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("백업 파일 삭제 실패: {0}", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("백업 정리 실패: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 모든 백업 파일 목록 가져오기
        /// </summary>
        public List<FileInfo> GetBackupFiles(string pattern = "*.json")
        {
            try
            {
                return Directory.GetFiles(_backupPath, pattern)
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();
            }
            catch
            {
                return new List<FileInfo>();
            }
        }

        /// <summary>
        /// 백업 파일 자동 복구 시도
        /// </summary>
        public async Task<int> AutoRecoverFromBackupsAsync()
        {
            int recoveredCount = 0;

            try
            {
                var aiFiles = GetBackupFiles("AI_Backup_*.json");
                var diFiles = GetBackupFiles("DI_Backup_*.json");

                var db = DataPostgres.Instance;

                // AI 데이터 복구
                foreach (var file in aiFiles)
                {
                    try
                    {
                        var aiData = LoadAIBackup(file.FullName);
                        if (aiData.Count > 0)
                        {
                            int saved = await db.SaveMinDataAIBatch(aiData);
                            if (saved > 0)
                            {
                                recoveredCount += saved;
                                file.Delete(); // 복구 성공 시 백업 파일 삭제
                                Debug.WriteLine(string.Format("AI 백업 복구 완료: {0}개", saved));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("AI 백업 복구 실패: {0}", ex.Message));
                    }
                }

                // DI 데이터 복구
                foreach (var file in diFiles)
                {
                    try
                    {
                        var diData = LoadDIBackup(file.FullName);
                        if (diData.Count > 0)
                        {
                            int saved = await db.SaveMinDataDIBatch(diData);
                            if (saved > 0)
                            {
                                recoveredCount += saved;
                                file.Delete(); // 복구 성공 시 백업 파일 삭제
                                Debug.WriteLine(string.Format("DI 백업 복구 완료: {0}개", saved));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("DI 백업 복구 실패: {0}", ex.Message));
                    }
                }

                if (recoveredCount > 0)
                {
                    string msg = string.Format("백업 데이터 자동 복구 완료: {0}개", recoveredCount);
                    MessageDisplay.Show(msg);
                    SmLog.LogInfo(LogCategory.DATA_SAVE, msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("자동 복구 실패: {0}", ex.Message));
                SmLog.LogError(LogCategory.DATA_SAVE, "백업 자동 복구 실패", ex);
            }

            return recoveredCount;
        }
    }
    #endregion 백업 파일 관리자
}
