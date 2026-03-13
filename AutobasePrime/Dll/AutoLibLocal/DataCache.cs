using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AutoLibLocal
{
    // 월별 파일 존재 여부 캐시 (성능 최적화)
    public class MonthlyFileCache
    {
        public Dictionary<string, bool> files;
        public DateTime lastUpdate;

        public MonthlyFileCache()
        {
            files = new Dictionary<string, bool>();
            lastUpdate = DateTime.MinValue;
        }
    }

    // 캐시 기반 클래스 (공통 기능)
    public abstract class BaseFileCache
    {
        public string FileName { get; set; }
        public DateTime CacheLoadTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime DataMonth { get; set; }
        public bool IsLoaded { get; set; }

        protected BaseFileCache()
        {
            IsLoaded = false;
            CacheLoadTime = DateTime.Now;
            LastAccessTime = DateTime.Now;
        }

        public void UpdateAccessTime()
        {
            LastAccessTime = DateTime.Now;
        }

        public bool IsValid(int cacheValidMinutes)
        {
            return IsLoaded && (DateTime.Now - CacheLoadTime).TotalMinutes <= cacheValidMinutes;
        }

        public bool IsMinDataRealTimeValid(DateTime requestTime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan diff = currentTime - requestTime;

            // 현재월과 요청월이 같은지 확인
            if (currentTime.Year == requestTime.Year && currentTime.Month == requestTime.Month)
            {
                // 현재월 데이터인 경우, 1분마다 갱신 필요
                DateTime cacheMinute = new DateTime(CacheLoadTime.Year, CacheLoadTime.Month,
                    CacheLoadTime.Day, CacheLoadTime.Hour, CacheLoadTime.Minute, 0);
                DateTime currentMinute = new DateTime(currentTime.Year, currentTime.Month,
                    currentTime.Day, currentTime.Hour, currentTime.Minute, 0);

                // 캐시 로드 시간과 현재 시간의 분이 다르면 무효
                return currentMinute == cacheMinute;
            }

            return IsValid(60); // 과거 데이터는 1시간 유효
        }

        public bool IsHourDataRealTimeValid(DateTime requestTime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan diff = currentTime - requestTime;

            // 현재월과 요청월이 같은지 확인
            if (currentTime.Year == requestTime.Year && currentTime.Month == requestTime.Month)
            {
                // 현재월 데이터인 경우, 시간이 바뀌면 갱신 필요
                DateTime cacheHour = new DateTime(CacheLoadTime.Year, CacheLoadTime.Month,
                    CacheLoadTime.Day, CacheLoadTime.Hour, 0, 0);
                DateTime currentHour = new DateTime(currentTime.Year, currentTime.Month,
                    currentTime.Day, currentTime.Hour, 0, 0);

                // 캐시 로드 시간과 현재 시간의 시가 다르면 무효
                return currentHour == cacheHour;
            }

            return IsValid(60); // 과거 데이터는 1시간 유효
        }

        public abstract long GetEstimatedMemoryUsage();
        public abstract void InitializeDataArray();
    }

    // AI 분별 데이터 캐시
    public class MinDataAIFileCache : BaseFileCache
    {
        private const int MINUTES_PER_DAY = 1440;
        private const int DAYS_PER_MONTH = 31;
        private const int TREND_AI_STRUCT_SIZE = 20; // float 5개

        public TREND_AI_STRUCT[] DataArray { get; set; }
        private bool[] _validFlags; // 유효 데이터 플래그 20251029 PSU 추가

        public MinDataAIFileCache()
        {
            InitializeDataArray();
        }

        public override void InitializeDataArray()
        {
           // DataArray = new TREND_AI_STRUCT[DAYS_PER_MONTH * MINUTES_PER_DAY];

            int totalMinutes = DAYS_PER_MONTH * MINUTES_PER_DAY; //20251029 PSU 수정
            DataArray = new TREND_AI_STRUCT[totalMinutes];
            _validFlags = new bool[totalMinutes]; // 기본값 false 20251029 PSU 추가

            for (int i = 0; i < DataArray.Length; i++)
            {
                DataArray[i] = new TREND_AI_STRUCT();
            }
        }

        public override long GetEstimatedMemoryUsage()
        {
            // return DAYS_PER_MONTH * MINUTES_PER_DAY * TREND_AI_STRUCT_SIZE;
            // TREND_AI_STRUCT 메모리 + bool 배열 메모리
            long structMemory = DAYS_PER_MONTH * MINUTES_PER_DAY * TREND_AI_STRUCT_SIZE;
            long flagMemory = DAYS_PER_MONTH * MINUTES_PER_DAY * sizeof(bool);
            return structMemory + flagMemory;
        }

        public bool GetData(int day, int hour, int min, TREND_AI_STRUCT data)
        {
            if (!IsDataValid(day, hour, min)) return false;

            int index = (day - 1) * MINUTES_PER_DAY + hour * 60 + min;
            if (index >= DataArray.Length) return false;

            // 유효 플래그 체크 (파일에서 실제로 로드된 데이터인지 확인)
            if (!_validFlags[index]) return false;

            UpdateAccessTime();

            TREND_AI_STRUCT cachedData = DataArray[index];

            // 빈 데이터 체크
            //if (IsEmptyData(cachedData)) return false; //20251029 PSU 삭제 v10.3.7.4 ~ v10.3.7.5에서 0 데이터 누락.

            // 데이터 복사
            CopyData(cachedData, data);
            return true;
        }

        public void SetData(int day, int hour, int min, TREND_AI_STRUCT data)
        {
            if (!IsDataValid(day, hour, min)) return;

            int index = (day - 1) * MINUTES_PER_DAY + hour * 60 + min;
            if (index < DataArray.Length)
            {
                DataArray[index] = data;
                _validFlags[index] = true; // 유효 플래그 설정 20251029 PSU 추가
            }
        }

        private bool IsDataValid(int day, int hour, int min)
        {
            return day >= 1 && day <= 31 && hour >= 0 && hour <= 23 && min >= 0 && min <= 59;
        }

        //20251029 PSU 삭제 
        //private bool IsEmptyData(TREND_AI_STRUCT data)
        //{
        //    return data.fAverage == 0 && data.fMin == 0 && data.fMax == 0 &&
        //           data.fCurr == 0 && data.fSumMin == 0;
        //}

        private void CopyData(TREND_AI_STRUCT source, TREND_AI_STRUCT target)
        {
            target.fSumMin = source.fSumMin;
            target.fAverage = source.fAverage;
            target.fMin = source.fMin;
            target.fMax = source.fMax;
            target.fCurr = source.fCurr;
        }
    }

    // DI 분별 데이터 캐시
    public class MinDataDIFileCache : BaseFileCache
    {
        private const int MINUTES_PER_DAY = 1440;
        private const int DAYS_PER_MONTH = 31;
        private const int TREND_DI_STRUCT_SIZE = 4;

        public TREND_DI_STRUCT[] DataArray { get; set; }
        private bool[] _validFlags; // 유효 데이터 플래그

        public MinDataDIFileCache()
        {
            InitializeDataArray();
        }

        public override void InitializeDataArray()
        {
            //DataArray = new TREND_DI_STRUCT[DAYS_PER_MONTH * MINUTES_PER_DAY];

            int totalMinutes = DAYS_PER_MONTH * MINUTES_PER_DAY; //20251029 PSU 수정
            DataArray = new TREND_DI_STRUCT[totalMinutes];
            _validFlags = new bool[totalMinutes]; // 기본값 false

            for (int i = 0; i < DataArray.Length; i++)
            {
                DataArray[i] = new TREND_DI_STRUCT();
            }
        }

        public override long GetEstimatedMemoryUsage()
        {
            // return DAYS_PER_MONTH * MINUTES_PER_DAY * TREND_DI_STRUCT_SIZE;
            // TREND_DI_STRUCT 메모리 + bool 배열 메모리
            long structMemory = DAYS_PER_MONTH * MINUTES_PER_DAY * TREND_DI_STRUCT_SIZE;
            long flagMemory = DAYS_PER_MONTH * MINUTES_PER_DAY * sizeof(bool);
            return structMemory + flagMemory;
        }

        public bool GetData(int day, int hour, int min, TREND_DI_STRUCT data)
        {
            if (!IsDataValid(day, hour, min)) return false;

            int index = (day - 1) * MINUTES_PER_DAY + hour * 60 + min;
            if (index >= DataArray.Length) return false;

            // 유효 플래그 체크 (파일에서 실제로 로드된 데이터인지 확인)
            if (!_validFlags[index]) return false;

            UpdateAccessTime();

            TREND_DI_STRUCT cachedData = DataArray[index];

            // 빈 데이터 체크
            //if (IsEmptyData(cachedData)) return false; //20251029 PSU 삭제 v10.3.7.4 ~ v10.3.7.5에서 0 데이터 누락.

            // 데이터 복사
            CopyData(cachedData, data);
            return true;
        }

        public void SetData(int day, int hour, int min, TREND_DI_STRUCT data)
        {
            if (!IsDataValid(day, hour, min)) return;

            int index = (day - 1) * MINUTES_PER_DAY + hour * 60 + min;
            if (index < DataArray.Length)
            {
                DataArray[index] = data;
                _validFlags[index] = true; // 유효 플래그 설정 20251029 PSU 추가
            }
        }

        private bool IsDataValid(int day, int hour, int min)
        {
            return day >= 1 && day <= 31 && hour >= 0 && hour <= 23 && min >= 0 && min <= 59;
        }

        //20251029 PSU 삭제
        //private bool IsEmptyData(TREND_DI_STRUCT data)
        //{
        //    return data.nCountOnOff == 0 && data.bOnOff == 0 && data.cOnTime == 0;
        //}

        private void CopyData(TREND_DI_STRUCT source, TREND_DI_STRUCT target)
        {
            target.nCountOnOff = source.nCountOnOff;
            target.bOnOff = source.bOnOff;
            target.cOnTime = source.cOnTime;
        }
    }

    // AI 시간별 데이터 캐시
    public class HourDataAIFileCache : BaseFileCache
    {
        private const int HOURS_PER_DAY = 24;
        private const int DAYS_PER_MONTH = 31;
        private const int HOUR_DATA_ANALOG_STRUCT_SIZE = 23;

        public HOUR_DATA_ANALOG_STRUCT[] DataArray { get; set; }

        public HourDataAIFileCache()
        {
            InitializeDataArray();
        }

        public override void InitializeDataArray()
        {
            DataArray = new HOUR_DATA_ANALOG_STRUCT[DAYS_PER_MONTH * HOURS_PER_DAY];
            for (int i = 0; i < DataArray.Length; i++)
            {
                DataArray[i] = new HOUR_DATA_ANALOG_STRUCT();
            }
        }

        public override long GetEstimatedMemoryUsage()
        {
            return DAYS_PER_MONTH * HOURS_PER_DAY * HOUR_DATA_ANALOG_STRUCT_SIZE;
        }

        public bool GetData(int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            if (!IsDataValid(day, hour)) return false;

            int index = (day - 1) * HOURS_PER_DAY + hour;
            if (index >= DataArray.Length) return false;

            UpdateAccessTime();

            HOUR_DATA_ANALOG_STRUCT cachedData = DataArray[index];

            // flag가 0이면 빈 데이터
            if (cachedData.flag == 0) return false;

            // 데이터 복사
            CopyData(cachedData, data);
            return true;
        }

        public void SetData(int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            if (!IsDataValid(day, hour)) return;

            int index = (day - 1) * HOURS_PER_DAY + hour;
            if (index < DataArray.Length)
            {
                DataArray[index] = data;
            }
        }

        private bool IsDataValid(int day, int hour)
        {
            return day >= 1 && day <= 31 && hour >= 0 && hour <= 23;
        }

        private void CopyData(HOUR_DATA_ANALOG_STRUCT source, HOUR_DATA_ANALOG_STRUCT target)
        {
            target.fSumHour = source.fSumHour;
            target.fAveHour = source.fAveHour;
            target.fMinHour = source.fMinHour;
            target.fMaxHour = source.fMaxHour;
            target.fCurrSumMeter = source.fCurrSumMeter;
            target.flag = source.flag;
            target.crc = source.crc;
        }
    }

    // DI 시간별 데이터 캐시
    public class HourDataDIFileCache : BaseFileCache
    {
        private const int HOURS_PER_DAY = 24;
        private const int DAYS_PER_MONTH = 31;
        private const int HOUR_DATA_DIGITAL_STRUCT_SIZE = 9;

        public HOUR_DATA_DIGITAL_STRUCT[] DataArray { get; set; }

        public HourDataDIFileCache()
        {
            InitializeDataArray();
        }

        public override void InitializeDataArray()
        {
            DataArray = new HOUR_DATA_DIGITAL_STRUCT[DAYS_PER_MONTH * HOURS_PER_DAY];
            for (int i = 0; i < DataArray.Length; i++)
            {
                DataArray[i] = new HOUR_DATA_DIGITAL_STRUCT();
            }
        }

        public override long GetEstimatedMemoryUsage()
        {
            return DAYS_PER_MONTH * HOURS_PER_DAY * HOUR_DATA_DIGITAL_STRUCT_SIZE;
        }

        public bool GetData(int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            if (!IsDataValid(day, hour)) return false;

            int index = (day - 1) * HOURS_PER_DAY + hour;
            if (index >= DataArray.Length) return false;

            UpdateAccessTime();

            HOUR_DATA_DIGITAL_STRUCT cachedData = DataArray[index];

            // flag가 0이면 빈 데이터
            if (cachedData.flag == 0) return false;

            // 데이터 복사
            CopyData(cachedData, data);
            return true;
        }

        public void SetData(int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            if (!IsDataValid(day, hour)) return;

            int index = (day - 1) * HOURS_PER_DAY + hour;
            if (index < DataArray.Length)
            {
                DataArray[index] = data;
            }
        }

        private bool IsDataValid(int day, int hour)
        {
            return day >= 1 && day <= 31 && hour >= 0 && hour <= 23;
        }

        private void CopyData(HOUR_DATA_DIGITAL_STRUCT source, HOUR_DATA_DIGITAL_STRUCT target)
        {
            target.wCountOnOff = source.wCountOnOff;
            target.dwOnTime = source.dwOnTime;
            target.flag = source.flag;
            target.crc = source.crc;
        }
    }

    // 캐시 매니저 클래스
    public class DataCacheManager
    {
        // 캐시 저장소
        private readonly Dictionary<string, MinDataAIFileCache> _minDataAICache;
        private readonly Dictionary<string, MinDataDIFileCache> _minDataDICache;
        private readonly Dictionary<string, HourDataAIFileCache> _hourDataAICache;
        private readonly Dictionary<string, HourDataDIFileCache> _hourDataDICache;

        // 파일 존재 여부 캐시 (File.Exists 호출 최소화)
        private readonly Dictionary<string, MonthlyFileCache> _monthlyFileCaches;
        private readonly TimeSpan _cacheExpireTime = TimeSpan.FromMinutes(10);

        // 단순한 락 사용 (복잡한 ReaderWriterLockSlim 대신)
        private readonly object _cacheLock = new object();
        private readonly object _monthlyFileLock = new object();

        // 캐시 설정
        private const int MAX_CACHE_SIZE = 20;
        private const int CACHE_VALID_MINUTES = 60;
        private const int HOUR_CACHE_VALID_MINUTES = 120;

        public DataCacheManager()
        {
            _minDataAICache = new Dictionary<string, MinDataAIFileCache>();
            _minDataDICache = new Dictionary<string, MinDataDIFileCache>();
            _hourDataAICache = new Dictionary<string, HourDataAIFileCache>();
            _hourDataDICache = new Dictionary<string, HourDataDIFileCache>();
            _monthlyFileCaches = new Dictionary<string, MonthlyFileCache>();
        }

        #region 파일 존재 여부 캐시 관리

        /// <summary>
        /// 월별 키 생성 (YYYY-MM 형태)
        /// </summary>
        private string GetMonthKey(int year, int month)
        {
            return String.Format("{0:D4}-{1:D2}", year, month);
        }


        /// <summary>
        /// 디렉토리를 스캔하여 파일 목록을 반환 (I/O 작업)
        /// </summary>
        private Dictionary<string, bool> ScanDirectoryFiles(string directoryPath)
        {
            Dictionary<string, bool> files = new Dictionary<string, bool>();

            if (!Directory.Exists(directoryPath))
                return files;

            try
            {
                // 모든 하위 디렉토리까지 재귀적으로 스캔
                string[] allFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

                foreach (string file in allFiles)
                {
                    files[file] = true;
                }

                System.Diagnostics.Debug.WriteLine(String.Format("디렉토리 스캔 완료 - {0}: {1}개 파일",
                    directoryPath, allFiles.Length));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("디렉토리 스캔 오류 - {0}: {1}",
                    directoryPath, ex.Message));
            }

            return files;
        }

        /// <summary>
        /// 특정 월의 파일 캐시 로드
        /// </summary>
        private void LoadMonthlyFileCache(int year, int month)
        {
            string monthKey = GetMonthKey(year, month);

            // 읽기 락으로 먼저 확인
            lock (_monthlyFileLock)
            {
                // 이미 캐시가 있고 만료되지 않았다면 건너뛰기
                if (_monthlyFileCaches.ContainsKey(monthKey))
                {
                    MonthlyFileCache existingCache = _monthlyFileCaches[monthKey];

                    // 현재 월인지 확인
                    DateTime now = DateTime.Now;
                    bool isCurrentMonth = (year == now.Year && month == now.Month);

                    if (!isCurrentMonth)
                    {
                        // 과거 월은 영구 유효 - 데이터가 변경되지 않음
                        return;
                    }

                    //현재 월인 경우
                    if (now.Minute > 5) //매시간 5분 이내는 계속 갱신.
                    {
                        // 현재 월만 만료 시간 체크
                        if (DateTime.Now - existingCache.lastUpdate < _cacheExpireTime)
                        {
                            return; // 캐시가 아직 유효함
                        }

                    }
                }
            }

            // 파일 I/O 작업을 락 밖에서 실행
            Dictionary<string, bool> trendFiles = new Dictionary<string, bool>();
            Dictionary<string, bool> sumFiles = new Dictionary<string, bool>();

            try
            {
                string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

                // 1. TREND 경로 스캔 (분별 데이터) - 락 밖에서 실행
                string trendPath = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}", data_dir, year, month);
                trendFiles = ScanDirectoryFiles(trendPath);

                // 2. SUM 경로 스캔 (시간별 데이터) - 락 밖에서 실행
                string sumPath = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}", data_dir, year, month);
                sumFiles = ScanDirectoryFiles(sumPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("파일 스캔 오류 - {0}: {1}", monthKey, ex.Message));
            }

            // 쓰기 락으로 캐시 업데이트
            lock (_monthlyFileLock)
            {
                // 더블 체킹 - 다른 스레드가 이미 로드했을 수 있음
                if (_monthlyFileCaches.ContainsKey(monthKey))
                {
                    MonthlyFileCache existingCache = _monthlyFileCaches[monthKey];
                    DateTime now = DateTime.Now;
                    bool isCurrentMonth = (year == now.Year && month == now.Month);

                    if (!isCurrentMonth ||
                        (now.Minute > 5 && DateTime.Now - existingCache.lastUpdate < _cacheExpireTime))
                    {
                        return; // 다른 스레드가 이미 로드함
                    }
                }

                // 새 캐시 생성
                MonthlyFileCache newCache = new MonthlyFileCache();

                // 스캔한 파일들을 캐시에 추가
                foreach (var kvp in trendFiles)
                {
                    newCache.files[kvp.Key] = kvp.Value;
                }
                foreach (var kvp in sumFiles)
                {
                    newCache.files[kvp.Key] = kvp.Value;
                }

                newCache.lastUpdate = DateTime.Now;
                _monthlyFileCaches[monthKey] = newCache;

                System.Diagnostics.Debug.WriteLine(String.Format("월별 캐시 로드 완료 - {0}: {1}개 파일", monthKey, newCache.files.Count));

                // 새 캐시 로드 후 메모리 체크
                CheckAndCleanupMemoryIfNeeded();
            }
        }
        

        /// <summary>
        /// 파일 존재 여부 확인 (월별 캐시 사용)
        /// </summary>
        public bool IsFileExistsCached(string filename, int year, int month)
        {
            // 해당 월의 캐시 로드
            LoadMonthlyFileCache(year, month);

            string monthKey = GetMonthKey(year, month);

            lock (_monthlyFileLock)
            {
                if (_monthlyFileCaches.ContainsKey(monthKey))
                {
                    MonthlyFileCache cache = _monthlyFileCaches[monthKey];

                    if (cache.files.ContainsKey(filename))
                        return cache.files[filename];
                }
            }

            // 캐시에 없으면 실제 파일 시스템 확인  (락 밖에서 실행)
            bool exists = File.Exists(filename);

            // 결과를 캐시에 저장
            lock (_monthlyFileLock)
            {
                // 결과를 캐시에 저장
                if (_monthlyFileCaches.ContainsKey(monthKey))
                {
                    _monthlyFileCaches[monthKey].files[filename] = exists;
                }
            }
            return exists;
        }

        /// <summary>
        /// 특정 월 캐시 제거 (메모리 절약)
        /// </summary>
        public void ClearMonthlyCache(int year, int month)
        {
            string monthKey = GetMonthKey(year, month);

            lock (_monthlyFileLock)
            {
                if (_monthlyFileCaches.ContainsKey(monthKey))
                {
                    _monthlyFileCaches.Remove(monthKey);
                    System.Diagnostics.Debug.WriteLine(String.Format("월별 캐시 제거: {0}", monthKey));
                }
            }
        }

        /// <summary>
        /// 오래된 월별 캐시 정리 (메모리 관리)
        /// </summary>
        public void CleanupOldMonthlyCaches()
        {
            DateTime cutoffTime = DateTime.Now.AddHours(-2); // 2시간 이전 캐시 제거
            List<string> keysToRemove = new List<string>();

            lock (_monthlyFileLock)
            {
                foreach (KeyValuePair<string, MonthlyFileCache> kvp in _monthlyFileCaches)
                {
                    if (kvp.Value.lastUpdate < cutoffTime)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
            }

            if (keysToRemove.Count > 0)
            {
                lock (_monthlyFileLock)
                {
                    foreach (string key in keysToRemove)
                    {
                        _monthlyFileCaches.Remove(key);
                        System.Diagnostics.Debug.WriteLine(String.Format("오래된 월별 캐시 정리: {0}", key));
                    }
                }
            }
        }

        private const long MAX_MONTHLY_CACHE_MEMORY_MB = 50; // 50MB 임계치
        private const int MAX_MONTHLY_CACHE_COUNT = 12;      // 최대 12개월
        private static DateTime _lastMemoryCheck = DateTime.MinValue;
        private const int MEMORY_CHECK_INTERVAL_MINUTES = 30; // 30분마다 체크

        /// <summary>
        /// 메모리 사용량 체크 및 필요시 정리 (락 제거)
        /// </summary>
        private void CheckAndCleanupMemoryIfNeeded()
        {
            // 너무 자주 체크하지 않도록 제한
            if (DateTime.Now - _lastMemoryCheck < TimeSpan.FromMinutes(MEMORY_CHECK_INTERVAL_MINUTES))
                return;

            _lastMemoryCheck = DateTime.Now;

            // 1. 캐시 개수 체크
            if (_monthlyFileCaches.Count > MAX_MONTHLY_CACHE_COUNT)
            {
                CleanupOldMonthlyCachesForMemory(MAX_MONTHLY_CACHE_COUNT);
                System.Diagnostics.Debug.WriteLine(String.Format("캐시 개수 초과로 정리: {0}개 → {1}개",
                    _monthlyFileCaches.Count, MAX_MONTHLY_CACHE_COUNT));
            }

            // 2. 메모리 사용량 체크
            long memoryUsageMB = GetMonthlyFileCacheMemoryUsage() / 1024 / 1024;
            if (memoryUsageMB > MAX_MONTHLY_CACHE_MEMORY_MB)
            {
                // 메모리 사용량이 임계치 초과시 더 적극적으로 정리
                int targetCacheCount = Math.Max(6, MAX_MONTHLY_CACHE_COUNT - 3); // 최소 6개월은 유지
                CleanupOldMonthlyCachesForMemory(targetCacheCount);

                System.Diagnostics.Debug.WriteLine(String.Format("메모리 사용량 초과로 정리: {0}MB → 목표 {1}개월",
                    memoryUsageMB, targetCacheCount));
            }
        }

        /// <summary>
        /// 월별 파일 캐시의 메모리 사용량 반환
        /// </summary>
        public long GetMonthlyFileCacheMemoryUsage()
        {
            long totalMemory = 0;

            foreach (MonthlyFileCache cache in _monthlyFileCaches.Values)
            {
                totalMemory += GetEstimatedMemoryUsage(cache);
            }

            return totalMemory;
        }

        /// <summary>
        /// 메모리 사용량 모니터링 메서드 (실제 런타임에서 사용)
        /// </summary>
        public static long GetEstimatedMemoryUsage(MonthlyFileCache cache)
        {
            if (cache == null || cache.files == null) return 0;

            int fileCount = cache.files.Count;
            long totalStringLength = 0;

            // 실제 경로들의 길이 합계 계산
            foreach (string path in cache.files.Keys)
            {
                totalStringLength += path.Length;
            }

            // 메모리 계산
            long dictionaryOverhead = 32;
            long entryOverhead = fileCount * 24;
            long stringMemory = totalStringLength * 2 + (fileCount * 28); // 문자열 오버헤드 포함
            long boolMemory = fileCount * 1;
            long classOverhead = 48;

            return dictionaryOverhead + entryOverhead + stringMemory + boolMemory + classOverhead;
        }

        public void CleanupOldMonthlyCachesForMemory()
        {
            CleanupOldMonthlyCachesForMemory(12);
        }


        /// <summary>
        /// 메모리 부족 시 과거 월 캐시를 LRU 방식으로 정리
        /// </summary>
        public void CleanupOldMonthlyCachesForMemory(int maxCacheCount)
        {
            DateTime now = DateTime.Now;
            List<KeyValuePair<string, DateTime>> pastMonthCaches = new List<KeyValuePair<string, DateTime>>();

            lock (_monthlyFileLock)
            {
                if (_monthlyFileCaches.Count <= maxCacheCount) return;

                // 과거 월 캐시만 수집
                foreach (KeyValuePair<string, MonthlyFileCache> kvp in _monthlyFileCaches)
                {
                    string monthKey = kvp.Key;
                    string[] parts = monthKey.Split('-');

                    if (parts.Length == 2)
                    {
                        int cacheYear = int.Parse(parts[0]);
                        int cacheMonth = int.Parse(parts[1]);

                        bool isCurrentMonth = (cacheYear == now.Year && cacheMonth == now.Month);

                        if (!isCurrentMonth)
                        {
                            pastMonthCaches.Add(new KeyValuePair<string, DateTime>(monthKey, kvp.Value.lastUpdate));
                        }
                    }
                }
            }

            if (pastMonthCaches.Count == 0) return;

            // 과거 월 캐시를 접근 시간 순으로 정렬 (오래된 것부터)
            pastMonthCaches.Sort((x, y) => x.Value.CompareTo(y.Value));

            // 오래된 것부터 제거하여 최대 개수 유지
            int removeCount = pastMonthCaches.Count - (maxCacheCount - 1); // 현재 월 1개 여유
            if (removeCount <= 0) return;

            lock (_monthlyFileLock)
            {
                for (int i = 0; i < removeCount && i < pastMonthCaches.Count; i++)
                {
                    string keyToRemove = pastMonthCaches[i].Key;
                    _monthlyFileCaches.Remove(keyToRemove);
                    System.Diagnostics.Debug.WriteLine(String.Format("메모리 부족으로 과거월 캐시 제거: {0}", keyToRemove));
                }
            }
        }


        #endregion

        #region AI 분별 데이터 캐시 관리

        public bool GetMinDataAI(string filename, int day, int hour, int min, TREND_AI_STRUCT data)
        {
            lock (_cacheLock)
            {
                if (!_minDataAICache.ContainsKey(filename) || !_minDataAICache[filename].IsLoaded)
                    return false;

                return _minDataAICache[filename].GetData(day, hour, min, data);
            }

        }


        /// <summary>
        ///  File.OpenRead(filename); === File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read) 다른 프로세스에서 쓰기 불가.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public bool LoadMinDataAIFile(string filename, int year, int month)
        {
            // 월별 파일 캐시를 사용하여 파일 존재 확인 (성능 최적화)
            if (!IsFileExistsCached(filename, year, month)) return false;

            // 파일 I/O 작업을 락 밖에서 실행
            MinDataAIFileCache newCache = null;
            bool loadSuccess = false;

            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        newCache = new MinDataAIFileCache();
                        newCache.FileName = filename;
                        newCache.DataMonth = new DateTime(year, month, 1);

                        LoadAIMinuteDataFromFile(br, fs, newCache);
                        loadSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("AI 캐시 분자료 로드 실패: {0} - {1}", filename, ex.Message));
                return false;
            }

            if (!loadSuccess) return false;

            Debug.WriteLine("AI분자료캐시 로딩");

            lock (_cacheLock)
            {
                // 기존 캐시 제거
                if (_minDataAICache.ContainsKey(filename))
                {
                    _minDataAICache.Remove(filename);
                    Debug.WriteLine(String.Format("AI분자료캐시 제거 {0}", filename));
                }

                // 캐시 크기 관리
                if (_minDataAICache.Count >= MAX_CACHE_SIZE)
                {
                    RemoveOldestCache(_minDataAICache);
                }

                newCache.IsLoaded = true;
                newCache.CacheLoadTime = DateTime.Now;
                newCache.LastAccessTime = DateTime.Now;

                _minDataAICache[filename] = newCache;
                Debug.WriteLine(String.Format("AI분자료캐시 할당 {0}", filename));
                return true;

            }
        }


        private void LoadAIMinuteDataFromFile(BinaryReader br, FileStream fs, MinDataAIFileCache cache)
        {
            long fileSize = fs.Length;
            long recordSize = 25;
            long totalRecords = fileSize / recordSize;

            for (long recordIndex = 0; recordIndex < totalRecords; recordIndex++)
            {
                long start = recordSize * recordIndex;
                if (start + recordSize > fileSize) break;

                fs.Seek(start, SeekOrigin.Begin);

                byte day = br.ReadByte();
                byte hour = br.ReadByte();
                byte min = br.ReadByte();

                if (day < 1 || day > 31 || hour > 23 || min > 59) continue;

                TREND_AI_STRUCT data = new TREND_AI_STRUCT();
                data.fSumMin = br.ReadSingle();
                data.fAverage = br.ReadSingle();
                data.fMin = br.ReadSingle();
                data.fMax = br.ReadSingle();
                data.fCurr = br.ReadSingle();
                br.ReadUInt16(); // crc

                cache.SetData(day, hour, min, data);
            }
        }


        #endregion

        #region DI 분별 데이터 캐시 관리

        public bool GetMinDataDI(string filename, int day, int hour, int min, TREND_DI_STRUCT data)
        {
            lock (_cacheLock)
            {
                if (!_minDataDICache.ContainsKey(filename) || !_minDataDICache[filename].IsLoaded)
                    return false;

                return _minDataDICache[filename].GetData(day, hour, min, data);
            }
        }

        public bool LoadMinDataDIFile(string filename, int year, int month)
        {
            // 월별 파일 캐시를 사용하여 파일 존재 확인 (성능 최적화)
            if (!IsFileExistsCached(filename, year, month)) return false;

            // 파일 I/O 작업을 락 밖에서 실행
            MinDataDIFileCache newCache = null;
            bool loadSuccess = false;

            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        newCache = new MinDataDIFileCache();
                        newCache.FileName = filename;
                        newCache.DataMonth = new DateTime(year, month, 1);

                        LoadDIMinuteDataFromFile(br, fs, newCache);
                        loadSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("DI 캐시 분자료 로드 실패: {0} - {1}", filename, ex.Message));
                return false;
            }

            if (!loadSuccess) return false;

            Debug.WriteLine("DI분자료캐시 로딩");

            // 캐시 업데이트는 락 안에서 실행
            lock (_cacheLock)
            {
                // 기존 캐시 제거
                if (_minDataDICache.ContainsKey(filename))
                {
                    _minDataDICache.Remove(filename);
                }

                // 캐시 크기 관리
                if (_minDataDICache.Count >= MAX_CACHE_SIZE)
                {
                    RemoveOldestCache(_minDataDICache);
                }

                newCache.IsLoaded = true;
                newCache.CacheLoadTime = DateTime.Now;
                newCache.LastAccessTime = DateTime.Now;

                _minDataDICache[filename] = newCache;
                return true;

            }
        }

        private void LoadDIMinuteDataFromFile(BinaryReader br, FileStream fs, MinDataDIFileCache cache)
        {
            long fileSize = fs.Length;
            long recordSize = 9;
            long totalRecords = fileSize / recordSize;

            for (long recordIndex = 0; recordIndex < totalRecords; recordIndex++)
            {
                long start = recordSize * recordIndex;
                if (start + recordSize > fileSize) break;

                fs.Seek(start, SeekOrigin.Begin);

                byte day = br.ReadByte();
                byte hour = br.ReadByte();
                byte min = br.ReadByte();

                if (day < 1 || day > 31 || hour > 23 || min > 59) continue;

                TREND_DI_STRUCT data = new TREND_DI_STRUCT();
                data.nCountOnOff = br.ReadInt16();
                data.bOnOff = br.ReadByte();
                data.cOnTime = br.ReadByte();
                br.ReadUInt16(); // crc

                cache.SetData(day, hour, min, data);
            }
        }

        #endregion

        #region 시간별 데이터 캐시 관리

        public bool GetHourDataAI(string filename, int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            lock (_cacheLock)
            {
                if (!_hourDataAICache.ContainsKey(filename) || !_hourDataAICache[filename].IsLoaded)
                    return false;

                return _hourDataAICache[filename].GetData(day, hour, data);
            }
        }

        public bool LoadHourDataAIFile(string filename, int year, int month)
        {
            // 월별 파일 캐시를 사용하여 파일 존재 확인 (성능 최적화)
            if (!IsFileExistsCached(filename, year, month)) return false;

            // 파일 I/O 작업을 락 밖에서 실행
            HourDataAIFileCache newCache = null;
            bool loadSuccess = false;

            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // 파일 크기 검증
                    long expectedSize = 18 + 23 * 24 * 31; // 헤더 + 데이터
                    if (fs.Length != expectedSize)
                    {
                        return false;
                    }

                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        newCache = new HourDataAIFileCache();
                        newCache.FileName = filename;
                        newCache.DataMonth = new DateTime(year, month, 1);

                        // 헤더 스킵
                        fs.Seek(18, SeekOrigin.Begin);

                        LoadAIHourDataFromFile(br, newCache);
                        loadSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("AI 캐시 시간자료 로드 실패: {0} - {1}", filename, ex.Message));
                return false;
            }

            if (!loadSuccess) return false;

            // 캐시 업데이트는 락 안에서 실행
            lock (_cacheLock)
            {
                // 기존 캐시 제거
                if (_hourDataAICache.ContainsKey(filename))
                {
                    _hourDataAICache.Remove(filename);
                }

                // 캐시 크기 관리
                if (_hourDataAICache.Count >= MAX_CACHE_SIZE)
                {
                    RemoveOldestCache(_hourDataAICache);
                }

                newCache.IsLoaded = true;
                newCache.CacheLoadTime = DateTime.Now;
                newCache.LastAccessTime = DateTime.Now;

                _hourDataAICache[filename] = newCache;
                return true;
            }
        }

        private void LoadAIHourDataFromFile(BinaryReader br, HourDataAIFileCache cache)
        {
            for (int day = 1; day <= 31; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    try
                    {
                        HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();
                        data.fSumHour = br.ReadSingle();
                        data.fAveHour = br.ReadSingle();
                        data.fMinHour = br.ReadSingle();
                        data.fMaxHour = br.ReadSingle();
                        data.fCurrSumMeter = br.ReadSingle();
                        data.flag = br.ReadByte();
                        data.crc = br.ReadUInt16();

                        cache.SetData(day, hour, data);
                    }
                    catch
                    {
                        // 읽기 오류 시 빈 데이터로 설정
                        HOUR_DATA_ANALOG_STRUCT emptyData = new HOUR_DATA_ANALOG_STRUCT();
                        emptyData.flag = 0;
                        cache.SetData(day, hour, emptyData);
                    }
                }
            }
        }

        #endregion

        #region 공통 캐시 관리 메서드

        private void RemoveOldestCache<T>(Dictionary<string, T> cache) where T : BaseFileCache
        {
            if (cache.Count == 0) return;

            string oldestKey = "";
            DateTime oldestTime = DateTime.MaxValue;

            foreach (var kvp in cache)
            {
                if (kvp.Value.LastAccessTime < oldestTime)
                {
                    oldestTime = kvp.Value.LastAccessTime;
                    oldestKey = kvp.Key;
                }
            }

            if (!string.IsNullOrEmpty(oldestKey))
            {
                cache.Remove(oldestKey);
            }
        }

        public void ClearAllCache()
        {
            lock (_cacheLock)
            {
                _minDataAICache.Clear();
                _minDataDICache.Clear();
                _hourDataAICache.Clear();
                _hourDataDICache.Clear();
            }
            lock (_monthlyFileLock)
            {
                _monthlyFileCaches.Clear(); // 월별 파일 캐시도 함께 정리
            }
        }

        public void InvalidateRealTimeCache()
        {
            DateTime currentTime = DateTime.Now;

            lock (_cacheLock)
            {
                InvalidateRealTimeCacheByType(_minDataAICache, currentTime);
                InvalidateRealTimeCacheByType(_minDataDICache, currentTime);
                InvalidateRealTimeCacheByType(_hourDataAICache, currentTime);
                InvalidateRealTimeCacheByType(_hourDataDICache, currentTime);
            }

            // 월별 파일 캐시도 주기적으로 정리
            CleanupOldMonthlyCaches();
        }

        private void InvalidateRealTimeCacheByType<T>(Dictionary<string, T> cache, DateTime currentTime) where T : BaseFileCache
        {
            List<string> keysToRemove = new List<string>();

            foreach (var kvp in cache)
            {
                if (kvp.Value.DataMonth.Year == currentTime.Year &&
                    kvp.Value.DataMonth.Month == currentTime.Month &&
                    (currentTime - kvp.Value.CacheLoadTime).TotalMinutes > 1)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (string key in keysToRemove)
            {
                cache.Remove(key);
            }
        }

        public string GetCacheStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== 캐시 상태 ===");
            sb.AppendLine(String.Format("분별 AI 캐시: {0}개", _minDataAICache.Count));
            sb.AppendLine(String.Format("분별 DI 캐시: {0}개", _minDataDICache.Count));
            sb.AppendLine(String.Format("시간별 AI 캐시: {0}개", _hourDataAICache.Count));
            sb.AppendLine(String.Format("시간별 DI 캐시: {0}개", _hourDataDICache.Count));

            // 메모리 사용량 계산
            long totalMemory = 0;
            foreach (var cache in _minDataAICache.Values)
                totalMemory += cache.GetEstimatedMemoryUsage();
            foreach (var cache in _minDataDICache.Values)
                totalMemory += cache.GetEstimatedMemoryUsage();
            foreach (var cache in _hourDataAICache.Values)
                totalMemory += cache.GetEstimatedMemoryUsage();
            foreach (var cache in _hourDataDICache.Values)
                totalMemory += cache.GetEstimatedMemoryUsage();

            sb.AppendLine(String.Format("추정 메모리 사용량: {0:F1} MB", totalMemory / 1024.0 / 1024.0));

            return sb.ToString();
        }

        public bool IsMinDataAICacheValid(string filename, DateTime requestTime)
        {
            lock (_cacheLock)
            {
                if (!_minDataAICache.ContainsKey(filename)) return false;
                return _minDataAICache[filename].IsMinDataRealTimeValid(requestTime);
            }
        }

        public bool IsMinDataDICacheValid(string filename, DateTime requestTime)
        {
            lock (_cacheLock)
            {
                if (!_minDataDICache.ContainsKey(filename)) return false;
                return _minDataDICache[filename].IsMinDataRealTimeValid(requestTime);
            }
        }

        public bool IsHourDataAICacheValid(string filename, DateTime requestTime)
        {
            lock (_cacheLock)
            {
                if (!_hourDataAICache.ContainsKey(filename)) return false;
                return _hourDataAICache[filename].IsHourDataRealTimeValid(requestTime);
            }
        }

        public bool IsHourDataDICacheValid(string filename, DateTime requestTime)
        {
            lock (_cacheLock)
            {
                if (!_hourDataDICache.ContainsKey(filename)) return false;
                return _hourDataDICache[filename].IsHourDataRealTimeValid(requestTime);
            }
        }

        public bool GetHourDataDI(string filename, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            lock (_cacheLock)
            {
                if (!_hourDataDICache.ContainsKey(filename) || !_hourDataDICache[filename].IsLoaded)
                    return false;

                return _hourDataDICache[filename].GetData(day, hour, data);
            }
        }

        public bool LoadHourDataDIFile(string filename, int year, int month)
        {
            // 월별 파일 캐시를 사용하여 파일 존재 확인 (성능 최적화)
            if (!IsFileExistsCached(filename, year, month)) return false;

            // 파일 I/O 작업을 락 밖에서 실행
            HourDataDIFileCache newCache = null;
            bool loadSuccess = false;

            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // 파일 크기 검증
                    long expectedSize = 18 + 9 * 24 * 31; // 헤더 + DI 데이터
                    if (fs.Length != expectedSize)
                    {
                        return false;
                    }

                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        newCache = new HourDataDIFileCache();
                        newCache.FileName = filename;
                        newCache.DataMonth = new DateTime(year, month, 1);

                        // 헤더 스킵
                        fs.Seek(18, SeekOrigin.Begin);

                        LoadDIHourDataFromFile(br, newCache);
                        loadSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("DI 캐시 시간자료 로드 실패: {0} - {1}", filename, ex.Message));
                return false;
            }

            if (!loadSuccess) return false;

            lock (_cacheLock)
            {
                // 기존 캐시 제거
                if (_hourDataDICache.ContainsKey(filename))
                {
                    _hourDataDICache.Remove(filename);
                }

                // 캐시 크기 관리
                if (_hourDataDICache.Count >= MAX_CACHE_SIZE)
                {
                    RemoveOldestCache(_hourDataDICache);
                }

                newCache.IsLoaded = true;
                newCache.CacheLoadTime = DateTime.Now;
                newCache.LastAccessTime = DateTime.Now;

                _hourDataDICache[filename] = newCache;
                return true;
            }
        }

        private void LoadDIHourDataFromFile(BinaryReader br, HourDataDIFileCache cache)
        {
            for (int day = 1; day <= 31; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    try
                    {
                        HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();
                        data.wCountOnOff = br.ReadUInt16();
                        data.dwOnTime = br.ReadUInt32();
                        data.flag = br.ReadByte();
                        data.crc = br.ReadUInt16();

                        cache.SetData(day, hour, data);
                    }
                    catch
                    {
                        // 읽기 오류 시 빈 데이터로 설정
                        HOUR_DATA_DIGITAL_STRUCT emptyData = new HOUR_DATA_DIGITAL_STRUCT();
                        emptyData.flag = 0;
                        cache.SetData(day, hour, emptyData);
                    }
                }
            }
        }

        /// <summary>
        /// 특정 파일의 캐시를 제거합니다.
        /// </summary>
        public void RemoveSpecificCache(string filename)
        {
            lock (_cacheLock)
            {
                _minDataAICache.Remove(filename);
                _minDataDICache.Remove(filename);
                _hourDataAICache.Remove(filename);
                _hourDataDICache.Remove(filename);
            }
        }

        /// <summary>
        /// 전체 캐시 개수를 반환합니다.
        /// </summary>
        public int GetTotalCacheCount()
        {
            lock (_cacheLock)
            {
                return _minDataAICache.Count + _minDataDICache.Count +
                       _hourDataAICache.Count + _hourDataDICache.Count;
            }
        }

        /// <summary>
        /// 추정 메모리 사용량을 MB 단위로 반환합니다.
        /// </summary>
        public double GetEstimatedMemoryUsage()
        {
            long totalMemory = 0;

            lock (_cacheLock)
            {
                foreach (var cache in _minDataAICache.Values)
                    totalMemory += cache.GetEstimatedMemoryUsage();
                foreach (var cache in _minDataDICache.Values)
                    totalMemory += cache.GetEstimatedMemoryUsage();
                foreach (var cache in _hourDataAICache.Values)
                    totalMemory += cache.GetEstimatedMemoryUsage();
                foreach (var cache in _hourDataDICache.Values)
                    totalMemory += cache.GetEstimatedMemoryUsage();
            }

            return totalMemory / 1024.0 / 1024.0; // MB 단위로 변환
        }

        /// <summary>
        /// 상세한 캐시 통계를 반환합니다.
        /// </summary>
        public Dictionary<string, object> GetCacheStatistics()
        {
            Dictionary<string, object> stats = new Dictionary<string, object>();

            lock (_cacheLock)
            {
                stats.Add("MinDataAI_Count", _minDataAICache.Count);
                stats.Add("MinDataDI_Count", _minDataDICache.Count);
                stats.Add("HourDataAI_Count", _hourDataAICache.Count);
                stats.Add("HourDataDI_Count", _hourDataDICache.Count);
                stats.Add("TotalCount", GetTotalCacheCount());
                stats.Add("EstimatedMemoryMB", GetEstimatedMemoryUsage());
                stats.Add("LastCleanupTime", DateTime.Now);
            }
            return stats;
        }

        /// <summary>
        /// 오래된 캐시들을 정리합니다. (1시간 이상 사용되지 않은 캐시)
        /// </summary>
        public int CleanupOldCaches(int maxAgeMinutes)
        {
            DateTime cutoffTime = DateTime.Now.AddMinutes(-maxAgeMinutes);
            int removedCount = 0;

            lock (_cacheLock)
            {
                removedCount += CleanupOldCachesByType(_minDataAICache, cutoffTime);
                removedCount += CleanupOldCachesByType(_minDataDICache, cutoffTime);
                removedCount += CleanupOldCachesByType(_hourDataAICache, cutoffTime);
                removedCount += CleanupOldCachesByType(_hourDataDICache, cutoffTime);
            }

            return removedCount;
        }

        public int CleanupOldCaches()
        {
            return CleanupOldCaches(60);
        }

        private int CleanupOldCachesByType<T>(Dictionary<string, T> cache, DateTime cutoffTime) where T : BaseFileCache
        {
            List<string> keysToRemove = new List<string>();

            foreach (var kvp in cache)
            {
                if (kvp.Value.LastAccessTime < cutoffTime)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (string key in keysToRemove)
            {
                cache.Remove(key);
            }

            return keysToRemove.Count;
        }

        #endregion
    }
}
