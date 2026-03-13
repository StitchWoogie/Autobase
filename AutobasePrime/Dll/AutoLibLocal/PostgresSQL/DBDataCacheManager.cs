using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLibLocal
{
    /// <summary>
    /// DB 조회 결과를 캐싱하는 간단한 캐시 매니저
    /// </summary>
    public class DBDataCacheManager
    {
        #region 캐시 데이터 구조
        /// <summary>
        /// 시간 범위별 AI 분별 데이터 캐시
        /// </summary>
        /// <summary>
        /// 월별 AI 분별 데이터 캐시 (파일시스템의 한 달 파일과 동일)
        /// </summary>
        private class MinuteAIMonthCache
        {
            public Dictionary<DateTime, TREND_AI_STRUCT> Data { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime LastAccessTime { get; set; }
            public DateTime CacheLoadTime { get; set; }
            public bool IsLoaded { get; set; }

            public MinuteAIMonthCache()
            {
                Data = new Dictionary<DateTime, TREND_AI_STRUCT>();
                LastAccessTime = DateTime.Now;
                CacheLoadTime = DateTime.Now;
                IsLoaded = false;
            }

            public void UpdateAccessTime()
            {
                LastAccessTime = DateTime.Now;
            }

            public bool IsCacheValid()
            {
                DateTime now = DateTime.Now;
                bool isCurrentMonth = (Year == now.Year && Month == now.Month);

              if (isCurrentMonth)
                {
                    // 현재 월은 1분마다 갱신 필요 - 분 단위로 비교
                    DateTime cacheMinute = new DateTime(CacheLoadTime.Year, CacheLoadTime.Month,
                        CacheLoadTime.Day, CacheLoadTime.Hour, CacheLoadTime.Minute, 0);
                    DateTime currentMinute = new DateTime(now.Year, now.Month,
                        now.Day, now.Hour, now.Minute, 0);

                    // 캐시 로드 시간과 현재 시간의 분이 같아야 유효
                    return currentMinute == cacheMinute;
                }
                else
                {
                    // 과거 월은 60분 유효
                    return (now - CacheLoadTime).TotalMinutes <= 60;
                }
            }
        }

        /// <summary>
        /// 월별 DI 분별 데이터 캐시
        /// </summary>
        private class MinuteDIMonthCache
        {
            public Dictionary<DateTime, TREND_DI_STRUCT> Data { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime LastAccessTime { get; set; }
            public DateTime CacheLoadTime { get; set; }
            public bool IsLoaded { get; set; }

            public MinuteDIMonthCache()
            {
                Data = new Dictionary<DateTime, TREND_DI_STRUCT>();
                LastAccessTime = DateTime.Now;
                CacheLoadTime = DateTime.Now;
                IsLoaded = false;
            }

            public void UpdateAccessTime()
            {
                LastAccessTime = DateTime.Now;
            }

            public bool IsCacheValid()
            {
                DateTime now = DateTime.Now;
                bool isCurrentMonth = (Year == now.Year && Month == now.Month);

                if (isCurrentMonth)
                {
                    // 현재 월은 1분마다 갱신 필요 - 분 단위로 비교
                    DateTime cacheMinute = new DateTime(CacheLoadTime.Year, CacheLoadTime.Month,
                        CacheLoadTime.Day, CacheLoadTime.Hour, CacheLoadTime.Minute, 0);
                    DateTime currentMinute = new DateTime(now.Year, now.Month,
                        now.Day, now.Hour, now.Minute, 0);

                    // 캐시 로드 시간과 현재 시간의 분이 같아야 유효
                    return currentMinute == cacheMinute;
                }
                else
                {
                    // 과거 월은 60분 유효
                    return (now - CacheLoadTime).TotalMinutes <= 60;
                }
            }
        }

        /// <summary>
        /// 월별 AI 시간 데이터 캐시
        /// </summary>
        private class HourAIMonthCache
        {
            public Dictionary<DateTime, HOUR_DATA_ANALOG_STRUCT> Data { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime LastAccessTime { get; set; }
            public DateTime CacheLoadTime { get; set; }
            public bool IsLoaded { get; set; }

            public HourAIMonthCache()
            {
                Data = new Dictionary<DateTime, HOUR_DATA_ANALOG_STRUCT>();
                LastAccessTime = DateTime.Now;
                CacheLoadTime = DateTime.Now;
                IsLoaded = false;
            }

            public void UpdateAccessTime()
            {
                LastAccessTime = DateTime.Now;
            }

            public bool IsCacheValid()
            {
                DateTime now = DateTime.Now;
                bool isCurrentMonth = (Year == now.Year && Month == now.Month);

                if (isCurrentMonth)
                {
                    // 현재 월은 시간 단위로 비교 (5분마다 갱신할 필요 없음)
                    DateTime cacheHour = new DateTime(CacheLoadTime.Year, CacheLoadTime.Month,
                        CacheLoadTime.Day, CacheLoadTime.Hour, 0, 0);
                    DateTime currentHour = new DateTime(now.Year, now.Month,
                        now.Day, now.Hour, 0, 0);

                    // 캐시 로드 시간과 현재 시간의 시가 같아야 유효
                    return currentHour == cacheHour;
                }
                else
                {
                    // 과거 월은 60분 유효
                    return (now - CacheLoadTime).TotalMinutes <= 60;
                }
            }
        }

        /// <summary>
        /// 월별 DI 시간 데이터 캐시
        /// </summary>
        private class HourDIMonthCache
        {
            public Dictionary<DateTime, HOUR_DATA_DIGITAL_STRUCT> Data { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime LastAccessTime { get; set; }
            public DateTime CacheLoadTime { get; set; }
            public bool IsLoaded { get; set; }

            public HourDIMonthCache()
            {
                Data = new Dictionary<DateTime, HOUR_DATA_DIGITAL_STRUCT>();
                LastAccessTime = DateTime.Now;
                CacheLoadTime = DateTime.Now;
                IsLoaded = false;
            }

            public void UpdateAccessTime()
            {
                LastAccessTime = DateTime.Now;
            }

            public bool IsCacheValid()
            {
                DateTime now = DateTime.Now;
                bool isCurrentMonth = (Year == now.Year && Month == now.Month);

                if (isCurrentMonth)
                {
                    // 현재 월은 시간 단위로 비교 (5분마다 갱신할 필요 없음)
                    DateTime cacheHour = new DateTime(CacheLoadTime.Year, CacheLoadTime.Month,
                        CacheLoadTime.Day, CacheLoadTime.Hour, 0, 0);
                    DateTime currentHour = new DateTime(now.Year, now.Month,
                        now.Day, now.Hour, 0, 0);

                    // 캐시 로드 시간과 현재 시간의 시가 같아야 유효
                    return currentHour == cacheHour;
                }
                else
                {
                    // 과거 월은 60분 유효
                    return (now - CacheLoadTime).TotalMinutes <= 60;
                }
            }
        }


        #endregion

        #region 캐시 저장소


        // 월별 캐시 저장소 - Key = "{tag}_{year}_{month}"
        private readonly Dictionary<string, MinuteAIMonthCache> _minuteAIMonthCache =
            new Dictionary<string, MinuteAIMonthCache>();

        private readonly Dictionary<string, MinuteDIMonthCache> _minuteDIMonthCache =
            new Dictionary<string, MinuteDIMonthCache>();

        private readonly Dictionary<string, HourAIMonthCache> _hourAIMonthCache =
            new Dictionary<string, HourAIMonthCache>();

        private readonly Dictionary<string, HourDIMonthCache> _hourDIMonthCache =
            new Dictionary<string, HourDIMonthCache>();

        private readonly object _lock = new object();

        // 캐시 설정
        private const int MAX_CACHE_SIZE = 20 * 12; // 태그 20개 * 최대 12개월분,   분자료일 경우  240개 × 44,640개 × 20바이트 = 약 208MB

        // 메모리 기반 제한 (선택적 사용)
        private const long MAX_MEMORY_MB = 300; // 300MB 제한

        #endregion

        #region AI 분별 데이터 캐싱 (시간 범위)

        public async Task<bool> GetMinDataAI(string tag, int year, int month, int day, int hour, int min, TREND_AI_STRUCT data)
        {
            DateTime requestTime = new DateTime(year, month, day, hour, min, 0);
            string monthKey = GetMonthKey(tag, year, month);

            lock (_lock)
            {
                if (_minuteAIMonthCache.TryGetValue(monthKey, out var cached))
                {
                    if (cached.IsLoaded && cached.IsCacheValid())
                    {
                        cached.UpdateAccessTime();

                        if (cached.Data.TryGetValue(requestTime, out var cachedData))
                        {
                            CopyTrendAIData(cachedData, data);
                            return true;
                        }

                        // 캐시에 있지만 해당 분 데이터가 없음
                        return false;
                    }

                    // 캐시 만료
                    _minuteAIMonthCache.Remove(monthKey);
                }
            }

            // DB에서 월별 전체 데이터 로드
            bool success = await LoadMonthMinuteDataAI(tag, year, month);
            if (!success) return false;

            // 재귀 호출
            return await GetMinDataAI(tag, year, month, day, hour, min, data).ConfigureAwait(false);
        }
        private async Task<bool> LoadMonthMinuteDataAI(string tag, int year, int month)
        {
            try
            {
                var db = DataPostgres.Instance;
                string monthKey = GetMonthKey(tag, year, month);

                // DB에서 월별 모든 분별 데이터를 한 번에 조회
                var monthlyData = await db.GetMonthMinuteDataAI(tag, year, month).ConfigureAwait(false);

                // 새 캐시 객체 생성 (lock 밖에서)
                var newCache = new MinuteAIMonthCache
                {
                    Year = year,
                    Month = month,
                    IsLoaded = true
                };

                // DataTable을 Dictionary로 변환
                foreach (System.Data.DataRow row in monthlyData.Rows)
                {
                    DateTime dbTime = (DateTime)row["data_time"];
                    DateTime localTime = dbTime.ToLocalTime();

                    var trend = new TREND_AI_STRUCT
                    {
                        fSumMin = Convert.ToSingle(row["sum_min"]),
                        fAverage = Convert.ToSingle(row["average"]),
                        fMin = Convert.ToSingle(row["min_value"]),
                        fMax = Convert.ToSingle(row["max_value"]),
                        fCurr = Convert.ToSingle(row["curr_value"])
                    };
                    newCache.Data[localTime] = trend;
                }

                // 캐시 업데이트 (lock 안에서)
                lock (_lock)
                {
                    // Double-checked locking
                    if (_minuteAIMonthCache.ContainsKey(monthKey))
                    {
                        return true;
                    }

                    // 캐시 크기 관리
                    if (_minuteAIMonthCache.Count >= MAX_CACHE_SIZE)
                    {
                        RemoveOldestMinuteAICache();
                    }

                    _minuteAIMonthCache[monthKey] = newCache;
                }

                Debug.WriteLine($"월별 AI 분별 데이터 캐시 로드: {tag} {year}-{month:D2} ({newCache.Data.Count}개)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadMonthMinuteDataAI 오류: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region DI 분별 데이터 캐싱 (월별 배치)

        public async Task<bool> GetMinDataDI(string tag, int year, int month, int day, int hour, int min, TREND_DI_STRUCT data)
        {
            DateTime requestTime = new DateTime(year, month, day, hour, min, 0);
            string monthKey = GetMonthKey(tag, year, month);

            lock (_lock)
            {
                if (_minuteDIMonthCache.TryGetValue(monthKey, out var cached))
                {
                    if (cached.IsLoaded && cached.IsCacheValid())
                    {
                        cached.UpdateAccessTime();

                        if (cached.Data.TryGetValue(requestTime, out var cachedData))
                        {
                            CopyTrendDIData(cachedData, data);
                            return true;
                        }

                        return false;
                    }

                    _minuteDIMonthCache.Remove(monthKey);
                }
            }

            bool success = await LoadMonthMinuteDataDI(tag, year, month);
            if (!success) return false;

            return await GetMinDataDI(tag, year, month, day, hour, min, data).ConfigureAwait(false);
        }

        private async Task<bool> LoadMonthMinuteDataDI(string tag, int year, int month)
        {
            try
            {
                var db = DataPostgres.Instance;
                string monthKey = GetMonthKey(tag, year, month);

                var monthlyData = await db.GetMonthMinuteDataDI(tag, year, month).ConfigureAwait(false);

                var newCache = new MinuteDIMonthCache
                {
                    Year = year,
                    Month = month,
                    IsLoaded = true
                };

                foreach (System.Data.DataRow row in monthlyData.Rows)
                {
                    DateTime dbTime = (DateTime)row["data_time"];
                    DateTime localTime = dbTime.ToLocalTime();

                    var trend = new TREND_DI_STRUCT
                    {
                        nCountOnOff = Convert.ToInt16(row["count_on_off"]),
                        bOnOff = Convert.ToBoolean(row["on_off_state"]) ? (byte)1 : (byte)0,
                        cOnTime = Convert.ToByte(row["on_time"])
                    };
                    newCache.Data[localTime] = trend;
                }

                lock (_lock)
                {
                    if (_minuteDIMonthCache.ContainsKey(monthKey))
                    {
                        return true;
                    }

                    if (_minuteDIMonthCache.Count >= MAX_CACHE_SIZE)
                    {
                        RemoveOldestMinuteDICache();
                    }

                    _minuteDIMonthCache[monthKey] = newCache;
                }

                Debug.WriteLine($"월별 DI 분별 데이터 캐시 로드: {tag} {year}-{month:D2} ({newCache.Data.Count}개)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadMonthMinuteDataDI 오류: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region AI 시간별 데이터 캐싱 (월별 배치)

        public async Task<bool> GetHourDataAI(string tag, int year, int month, int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            DateTime requestTime = new DateTime(year, month, day, hour, 0, 0);
            string monthKey = GetMonthKey(tag, year, month);

            lock (_lock)
            {
                if (_hourAIMonthCache.TryGetValue(monthKey, out var cached))
                {
                    if (cached.IsLoaded && cached.IsCacheValid())
                    {
                        cached.UpdateAccessTime();

                        if (cached.Data.TryGetValue(requestTime, out var cachedData))
                        {
                            CopyHourAIData(cachedData, data);
                            return cachedData.flag != 0;
                        }

                        return false;
                    }

                    _hourAIMonthCache.Remove(monthKey);
                }
            }

            bool success = await LoadMonthHourDataAI(tag, year, month);
            if (!success) return false;

            return await GetHourDataAI(tag, year, month, day, hour, data).ConfigureAwait(false);
        }

        private async Task<bool> LoadMonthHourDataAI(string tag, int year, int month)
        {
            try
            {
                var db = DataPostgres.Instance;
                string monthKey = GetMonthKey(tag, year, month);

                var monthlyData = await db.GetMonthHourDataAI(tag, year, month).ConfigureAwait(false);

                var newCache = new HourAIMonthCache
                {
                    Year = year,
                    Month = month,
                    IsLoaded = true
                };

                foreach (System.Data.DataRow row in monthlyData.Rows)
                {
                    DateTime dbTime = (DateTime)row["data_time"];
                    DateTime localTime = dbTime.ToLocalTime();

                    var hourData = new HOUR_DATA_ANALOG_STRUCT
                    {
                        fSumHour = Convert.ToSingle(row["sum_hour"]),
                        fAveHour = Convert.ToSingle(row["avg_hour"]),
                        fMinHour = Convert.ToSingle(row["min_hour"]),
                        fMaxHour = Convert.ToSingle(row["max_hour"]),
                        fCurrSumMeter = Convert.ToSingle(row["curr_sum_meter"]),
                        flag = Convert.ToByte(row["flag"])
                    };
                    newCache.Data[localTime] = hourData;
                }

                lock (_lock)
                {
                    if (_hourAIMonthCache.ContainsKey(monthKey))
                    {
                        return true;
                    }

                    if (_hourAIMonthCache.Count >= MAX_CACHE_SIZE)
                    {
                        RemoveOldestHourAICache();
                    }

                    _hourAIMonthCache[monthKey] = newCache;
                }

                Debug.WriteLine($"월별 AI 시간 데이터 캐시 로드: {tag} {year}-{month:D2} ({newCache.Data.Count}개)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadMonthHourDataAI 오류: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region DI 시간별 데이터 캐싱 (월별 배치)

        public async Task<bool> GetHourDataDI(string tag, int year, int month, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
                DateTime requestTime = new DateTime(year, month, day, hour, 0, 0);
                string monthKey = GetMonthKey(tag, year, month);
       
            lock (_lock)
            {
                if (_hourDIMonthCache.TryGetValue(monthKey, out var cached))
                {
                    if (cached.IsLoaded && cached.IsCacheValid())
                    {
                        cached.UpdateAccessTime();

                        if (cached.Data.TryGetValue(requestTime, out var cachedData))
                        {
                            CopyHourDIData(cachedData, data);
                            return cachedData.flag != 0;
                        }

                        return false;
                    }

                    _hourDIMonthCache.Remove(monthKey);
                }
            }

            bool success = await LoadMonthHourDataDI(tag, year, month);
            if (!success) return false;

            return await GetHourDataDI(tag, year, month, day, hour, data).ConfigureAwait(false);
        }

        private async Task<bool> LoadMonthHourDataDI(string tag, int year, int month)
        {
            try
            {
                var db = DataPostgres.Instance;
                string monthKey = GetMonthKey(tag, year, month);

                var monthlyData = await db.GetMonthHourDataDI(tag, year, month).ConfigureAwait(false);

                var newCache = new HourDIMonthCache
                {
                    Year = year,
                    Month = month,
                    IsLoaded = true
                };

                foreach (System.Data.DataRow row in monthlyData.Rows)
                {
                    DateTime dbTime = (DateTime)row["data_time"];
                    DateTime localTime = dbTime.ToLocalTime();

                    var hourData = new HOUR_DATA_DIGITAL_STRUCT
                    {
                        wCountOnOff = Convert.ToUInt16(row["count_on_off"]),
                        dwOnTime = Convert.ToUInt32(row["on_time"]),
                        flag = Convert.ToByte(row["flag"])
                    };
                    newCache.Data[localTime] = hourData;
                }

                lock (_lock)
                {
                    if (_hourDIMonthCache.ContainsKey(monthKey))
                    {
                        return true;
                    }

                    if (_hourDIMonthCache.Count >= MAX_CACHE_SIZE)
                    {
                        RemoveOldestHourDICache();
                    }

                    _hourDIMonthCache[monthKey] = newCache;
                }

                Debug.WriteLine($"월별 DI 시간 데이터 캐시 로드: {tag} {year}-{month:D2} ({newCache.Data.Count}개)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadMonthHourDataDI 오류: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 유틸리티 메서드

        private string GetMonthKey(string tag, int year, int month)
        {
            return $"{tag}_{year:0000}_{month:00}";
        }

        private void CopyTrendAIData(TREND_AI_STRUCT source, TREND_AI_STRUCT target)
        {
            target.fSumMin = source.fSumMin;
            target.fAverage = source.fAverage;
            target.fMin = source.fMin;
            target.fMax = source.fMax;
            target.fCurr = source.fCurr;
        }

        private void CopyTrendDIData(TREND_DI_STRUCT source, TREND_DI_STRUCT target)
        {
            target.nCountOnOff = source.nCountOnOff;
            target.bOnOff = source.bOnOff;
            target.cOnTime = source.cOnTime;
        }

        private void CopyHourAIData(HOUR_DATA_ANALOG_STRUCT source, HOUR_DATA_ANALOG_STRUCT target)
        {
            target.fSumHour = source.fSumHour;
            target.fAveHour = source.fAveHour;
            target.fMinHour = source.fMinHour;
            target.fMaxHour = source.fMaxHour;
            target.fCurrSumMeter = source.fCurrSumMeter;
            target.flag = source.flag;
        }

        private void CopyHourDIData(HOUR_DATA_DIGITAL_STRUCT source, HOUR_DATA_DIGITAL_STRUCT target)
        {
            target.wCountOnOff = source.wCountOnOff;
            target.dwOnTime = source.dwOnTime;
            target.flag = source.flag;
        }
        #endregion

        #region 동적 캐시 크기 관리

        private static DateTime _lastMemoryCheck = DateTime.MinValue;
        private const int MEMORY_CHECK_INTERVAL_MINUTES = 30; // 30분마다 체크

        /// <summary>
        /// 메모리 사용량 체크 및 필요시 캐시 정리
        /// </summary>
        private void CheckAndCleanupMemoryIfNeeded()
        {
            // 너무 자주 체크하지 않도록 제한
            if (DateTime.Now - _lastMemoryCheck < TimeSpan.FromMinutes(MEMORY_CHECK_INTERVAL_MINUTES))
                return;

            _lastMemoryCheck = DateTime.Now;

            double currentMemoryMB = GetEstimatedMemoryUsage();

            if (currentMemoryMB > MAX_MEMORY_MB)
            {
                Debug.WriteLine($"메모리 사용량 초과: {currentMemoryMB:F1}MB / {MAX_MEMORY_MB}MB");

                // 가장 오래된 캐시들을 제거하여 메모리 확보
                int removedCount = 0;

                // 1. 분별 데이터 중 가장 오래된 것부터 제거
                while (currentMemoryMB > MAX_MEMORY_MB * 0.8 && _minuteAIMonthCache.Count > 0)
                {
                    RemoveOldestMinuteAICache();
                    removedCount++;
                    currentMemoryMB = GetEstimatedMemoryUsage();
                }

                while (currentMemoryMB > MAX_MEMORY_MB * 0.8 && _minuteDIMonthCache.Count > 0)
                {
                    RemoveOldestMinuteDICache();
                    removedCount++;
                    currentMemoryMB = GetEstimatedMemoryUsage();
                }

                Debug.WriteLine($"메모리 정리 완료: {removedCount}개 캐시 제거, 현재 {currentMemoryMB:F1}MB");
            }
        }

        /// <summary>
        /// 캐시 추가 시 메모리 체크
        /// </summary>
        private void AddCacheWithMemoryCheck<T>(Dictionary<string, T> cache, string key, T value, int maxSize, Action removeOldest)
        {
            // 1. 개수 기반 제한 체크
            if (cache.Count >= maxSize)
            {
                removeOldest();
            }

            cache[key] = value;

            // 2. 메모리 기반 제한 체크
            CheckAndCleanupMemoryIfNeeded();
        }

        #endregion


        #region 캐시 정리 메서드

        private void RemoveOldestMinuteAICache()
        {
            if (_minuteAIMonthCache.Count == 0) return;

            var oldest = _minuteAIMonthCache.OrderBy(kvp => kvp.Value.LastAccessTime).First();
            _minuteAIMonthCache.Remove(oldest.Key);
            Debug.WriteLine($"오래된 AI 분별 캐시 제거: {oldest.Key}");
        }

        private void RemoveOldestMinuteDICache()
        {
            if (_minuteDIMonthCache.Count == 0) return;

            var oldest = _minuteDIMonthCache.OrderBy(kvp => kvp.Value.LastAccessTime).First();
            _minuteDIMonthCache.Remove(oldest.Key);
            Debug.WriteLine($"오래된 DI 분별 캐시 제거: {oldest.Key}");
        }

        private void RemoveOldestHourAICache()
        {
            if (_hourAIMonthCache.Count == 0) return;

            var oldest = _hourAIMonthCache.OrderBy(kvp => kvp.Value.LastAccessTime).First();
            _hourAIMonthCache.Remove(oldest.Key);
            Debug.WriteLine($"오래된 AI 시간 캐시 제거: {oldest.Key}");
        }

        private void RemoveOldestHourDICache()
        {
            if (_hourDIMonthCache.Count == 0) return;

            var oldest = _hourDIMonthCache.OrderBy(kvp => kvp.Value.LastAccessTime).First();
            _hourDIMonthCache.Remove(oldest.Key);
            Debug.WriteLine($"오래된 DI 시간 캐시 제거: {oldest.Key}");
        }

        public void ClearAllCache()
        {
            lock (_lock)
            {
                _minuteAIMonthCache.Clear();
                _minuteDIMonthCache.Clear();
                _hourAIMonthCache.Clear();
                _hourDIMonthCache.Clear();
            }
        }

        public void InvalidateCurrentMonthCache()
        {
            DateTime now = DateTime.Now;
            List<string> keysToRemove = new List<string>();

            lock (_lock)
            {
                // 현재 월 캐시만 무효화
                foreach (var kvp in _minuteAIMonthCache)
                {
                    if (kvp.Value.Year == now.Year && kvp.Value.Month == now.Month)
                        keysToRemove.Add(kvp.Key);
                }
                foreach (var key in keysToRemove)
                    _minuteAIMonthCache.Remove(key);

                keysToRemove.Clear();
                foreach (var kvp in _minuteDIMonthCache)
                {
                    if (kvp.Value.Year == now.Year && kvp.Value.Month == now.Month)
                        keysToRemove.Add(kvp.Key);
                }
                foreach (var key in keysToRemove)
                    _minuteDIMonthCache.Remove(key);

                keysToRemove.Clear();
                foreach (var kvp in _hourAIMonthCache)
                {
                    if (kvp.Value.Year == now.Year && kvp.Value.Month == now.Month)
                        keysToRemove.Add(kvp.Key);
                }
                foreach (var key in keysToRemove)
                    _hourAIMonthCache.Remove(key);

                keysToRemove.Clear();
                foreach (var kvp in _hourDIMonthCache)
                {
                    if (kvp.Value.Year == now.Year && kvp.Value.Month == now.Month)
                        keysToRemove.Add(kvp.Key);
                }
                foreach (var key in keysToRemove)
                    _hourDIMonthCache.Remove(key);
            }
        }

        public int GetTotalCacheCount()
        {
            lock (_lock)
            {
                return _minuteAIMonthCache.Count + _minuteDIMonthCache.Count +
                       _hourAIMonthCache.Count + _hourDIMonthCache.Count;
            }
        }

        public string GetCacheStatistics()
        {
            lock (_lock)
            {
                int totalMinuteData = _minuteAIMonthCache.Values.Sum(c => c.Data.Count) +
                                     _minuteDIMonthCache.Values.Sum(c => c.Data.Count);
                int totalHourData = _hourAIMonthCache.Values.Sum(c => c.Data.Count) +
                                   _hourDIMonthCache.Values.Sum(c => c.Data.Count);

                return $"월별 배치 캐시 통계:\n" +
                       $"분별 캐시: {_minuteAIMonthCache.Count + _minuteDIMonthCache.Count}개월, {totalMinuteData}개 데이터\n" +
                       $"시간별 캐시: {_hourAIMonthCache.Count + _hourDIMonthCache.Count}개월, {totalHourData}개 데이터\n" +
                       $"추정 메모리: {GetEstimatedMemoryUsage():F1} MB";
            }
        }

        public double GetEstimatedMemoryUsage()
        {
            lock (_lock)
            {
                long totalBytes = 0;

                foreach (var cache in _minuteAIMonthCache.Values)
                    totalBytes += cache.Data.Count * 20; // TREND_AI_STRUCT 크기

                foreach (var cache in _minuteDIMonthCache.Values)
                    totalBytes += cache.Data.Count * 4;  // TREND_DI_STRUCT 크기

                foreach (var cache in _hourAIMonthCache.Values)
                    totalBytes += cache.Data.Count * 23; // HOUR_DATA_ANALOG_STRUCT 크기

                foreach (var cache in _hourDIMonthCache.Values)
                    totalBytes += cache.Data.Count * 9;  // HOUR_DATA_DIGITAL_STRUCT 크기

                return totalBytes / (1024.0 * 1024.0);
            }
        }

        #endregion
    }
}
