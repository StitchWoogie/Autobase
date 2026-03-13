using AutoLibLocal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain
{
    #region 트렌드 데이터 저장 매니저
    public class TrendSaveManager
    {
        private static TrendSaveManager _instance;
        private static readonly object _instanceLock = new object();

        public static TrendSaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                            _instance = new TrendSaveManager();
                    }
                }
                return _instance;
            }
        }

        // 큐 (메인 타이머에서 데이터를 큐에만 추가)
        private readonly ConcurrentQueue<(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)> _aiQueue
            = new ConcurrentQueue<(string, DateTime, TREND_AI_STRUCT)>();
        private readonly ConcurrentQueue<(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)> _diQueue
            = new ConcurrentQueue<(string, DateTime, TREND_DI_STRUCT)>();

        // 우선순위 분 데이터 큐 (트렌드 화면에서 요청한 태그)
        private readonly ConcurrentQueue<(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)> _aiPriorityQueue
            = new ConcurrentQueue<(string, DateTime, TREND_AI_STRUCT)>();
        private readonly ConcurrentQueue<(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)> _diPriorityQueue
            = new ConcurrentQueue<(string, DateTime, TREND_DI_STRUCT)>();

        // 시간 데이터 처리 큐 (59분 데이터 저장 완료 후 처리)
        private readonly ConcurrentQueue<(TagPublicClass tp, DateTime hourTime)> _hourProcessQueue
            = new ConcurrentQueue<(TagPublicClass, DateTime)>();

        // 백업 관리자
        private readonly TrendBackupManager _backupManager;

        // DB 상태 모니터
        private readonly DatabaseHealthMonitor _healthMonitor;

        // 처리 스레드
        private Thread _saveThread;
        private bool _isRunning = false;
        private readonly AutoResetEvent _saveSignal = new AutoResetEvent(false);

        // 통계
        private long _totalQueued = 0;
        private long _totalSaved = 0;
        private long _totalFailed = 0;
        private long _totalBackedUp = 0;
        private long _totalPriorityProcessed = 0;


        // 시간 데이터 저장을 위한 59분 추적
        private readonly HashSet<string> _processedHourKeys = new HashSet<string>();
        private readonly object _hourKeysLock = new object();
        private DateTime _lastHourKeysCleanup = DateTime.Now;

        private TrendSaveManager()
        {
            _backupManager = new TrendBackupManager();

            // DatabaseHealthMonitor 싱글톤 사용
            if (!DatabaseHealthMonitor.IsInitialized)
            {
                DatabaseHealthMonitor.Initialize(ConfigDataDB.sConnectionString);
            }

            // 초기화 후 로컬 참조 저장 및 이벤트 구독
            if (DatabaseHealthMonitor.IsInitialized)
            {
                _healthMonitor = DatabaseHealthMonitor.Instance;
                // DB 상태 변경 이벤트 구독
                _healthMonitor.StateChanged += OnDbStateChanged;
            }
        }

        /// <summary>
        /// 저장 스레드 시작
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _saveThread = new Thread(SaveThreadLoop)
            {
                Name = "TrendSaveThread",
                IsBackground = true
            };
            _saveThread.Start();

            Debug.WriteLine("트렌드 저장 스레드 시작됨");
        }

        /// <summary>
        /// 저장 스레드 중지
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _saveSignal.Set();

            if (_saveThread != null && _saveThread.IsAlive)
            {
                _saveThread.Join(5000);
            }

            Debug.WriteLine("트렌드 저장 스레드 중지됨");
        }

        /// <summary>
        /// AI 데이터 큐에 추가 (메인 타이머에서 호출)
        /// </summary>
        public void EnqueueAI(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)
        {
            _aiQueue.Enqueue((tagName, dataTime, trend));
            Interlocked.Increment(ref _totalQueued);


            // 큐가 일정 크기 이상이면 저장 신호
            if (_aiQueue.Count >= ConfigData.nSaveCountAtOnce)
            {
                _saveSignal.Set();
            }
        }

        /// <summary>
        /// DI 데이터 큐에 추가 (메인 타이머에서 호출)
        /// </summary>
        public void EnqueueDI(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)
        {
            _diQueue.Enqueue((tagName, dataTime, trend));
            Interlocked.Increment(ref _totalQueued);

            // 큐가 일정 크기 이상이면 저장 신호
            if (_diQueue.Count >= ConfigData.nSaveCountAtOnce)
            {
                _saveSignal.Set();
            }
        }

        /// <summary>
        /// AI 분 데이터 큐에 우선순위로 추가 (트렌드 화면에서 호출)
        /// </summary>
        public void EnqueueAIPriority(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)
        {
            _aiPriorityQueue.Enqueue((tagName, dataTime, trend));
            Interlocked.Increment(ref _totalQueued);
            Interlocked.Increment(ref _totalPriorityProcessed);

            // 우선순위 큐는 즉시 처리 신호
            _saveSignal.Set();

            Debug.WriteLine(string.Format("우선순위 AI 큐 추가: {0}, {1:yyyy-MM-dd HH:mm}",
                tagName, dataTime));
        }

        /// <summary>
        /// DI 분 데이터 큐에 우선순위로 추가 (트렌드 화면에서 호출)
        /// </summary>
        public void EnqueueDIPriority(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)
        {
            _diPriorityQueue.Enqueue((tagName, dataTime, trend));
            Interlocked.Increment(ref _totalQueued);
            Interlocked.Increment(ref _totalPriorityProcessed);

            // 우선순위 큐는 즉시 처리 신호
            _saveSignal.Set();

            Debug.WriteLine(string.Format("우선순위 DI 큐 추가: {0}, {1:yyyy-MM-dd HH:mm}",
                tagName, dataTime));
        }


        /// <summary>
        /// 시간 데이터 처리 큐에 추가 (StatusRemainTrendSaveAsync에서 호출)
        /// </summary>
        public void EnqueueHourProcess(TagPublicClass tp, DateTime hourTime)
        {
            _hourProcessQueue.Enqueue((tp, hourTime));
            Interlocked.Increment(ref _totalQueued);
        }

        /// <summary>
        /// 저장 스레드 루프
        /// </summary>
        private void SaveThreadLoop()
        {
            while (_isRunning)
            {
                try
                {
                    // 신호 대기 (최대 1초)
                    _saveSignal.WaitOne(1000); 

                    if (!_isRunning) break;

                    // 배치 처리
                    //ProcessBatchAsync().Wait();
                    // ConfigureAwait(false)를 사용하여 안전하게 대기
                    ProcessBatchAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("저장 스레드 오류: {0}", ex.Message));
                    SmLog.LogError(LogCategory.DATA_SAVE, "저장 스레드 오류", ex);
                    Thread.Sleep(1000); // 오류 발생 시 잠시 대기
                }
            }

            Debug.WriteLine("저장 스레드 종료");
        }

        /// <summary>
        /// 배치 처리 (별도 스레드에서 실행)
        /// </summary>
        private async Task ProcessBatchAsync()
        {
            bool aiDataSaved = false;
            bool diDataSaved = false;

            // 1. 우선순위 AI 데이터 먼저 처리
            if (_aiPriorityQueue.Count > 0)
            {
                var aiBatch = new List<(string, DateTime, TREND_AI_STRUCT)>();
                int batchSize = Math.Min(_aiPriorityQueue.Count, ConfigData.nSaveCountAtOnce);

                for (int i = 0; i < batchSize; i++)
                {
                    if (_aiPriorityQueue.TryDequeue(out var item))
                    {
                        aiBatch.Add(item);
                        Interlocked.Add(ref _totalQueued, -1);
                    }
                }

                if (aiBatch.Count > 0)
                {
                    Debug.WriteLine(string.Format("우선순위 AI 배치 처리: {0}건", aiBatch.Count));
                    await SaveAIBatchAsync(aiBatch);
                    aiDataSaved = true;
                    CheckAndPrepareHourData(aiBatch, EnumTagType.AI);
                }
            }



            // 3. 우선순위 DI 데이터 먼저 처리
            if (_diPriorityQueue.Count > 0)
            {
                var diBatch = new List<(string, DateTime, TREND_DI_STRUCT)>();
                int batchSize = Math.Min(_diPriorityQueue.Count, ConfigData.nSaveCountAtOnce);

                for (int i = 0; i < batchSize; i++)
                {
                    if (_diPriorityQueue.TryDequeue(out var item))
                    {
                        diBatch.Add(item);
                        Interlocked.Add(ref _totalQueued, -1);
                    }
                }

                if (diBatch.Count > 0)
                {
                    Debug.WriteLine(string.Format("우선순위 DI 배치 처리: {0}건", diBatch.Count));
                    await SaveDIBatchAsync(diBatch);
                    diDataSaved = true;
                    CheckAndPrepareHourData(diBatch, EnumTagType.DI);
                }
            }

            // AI 데이터 처리
            if (_aiQueue.Count > 0)
            {
                var aiBatch = new List<(string, DateTime, TREND_AI_STRUCT)>();
                int batchSize = Math.Min(_aiQueue.Count, ConfigData.nSaveCountAtOnce );

                for (int i = 0; i < batchSize; i++)
                {
                    if (_aiQueue.TryDequeue(out var item))
                    {
                        aiBatch.Add(item);
                        Interlocked.Add(ref _totalQueued, -1);
                    }
                }

                if (aiBatch.Count > 0)
                {
                    await SaveAIBatchAsync(aiBatch);
                    aiDataSaved = true;

                    // 59분 데이터가 있는지 확인하고 시간 처리 준비
                    CheckAndPrepareHourData(aiBatch, EnumTagType.AI);
                }
            }

            // DI 데이터 처리
            if (_diQueue.Count > 0)
            {
                var diBatch = new List<(string, DateTime, TREND_DI_STRUCT)>();
                int batchSize = Math.Min(_diQueue.Count, ConfigData.nSaveCountAtOnce );

                for (int i = 0; i < batchSize; i++)
                {
                    if (_diQueue.TryDequeue(out var item))
                    {
                        diBatch.Add(item);
                        Interlocked.Add(ref _totalQueued, -1);
                    }
                }

                if (diBatch.Count > 0)
                {
                    await SaveDIBatchAsync(diBatch);
                    diDataSaved = true;

                    // 59분 데이터가 있는지 확인하고 시간 처리 준비
                    CheckAndPrepareHourData(diBatch, EnumTagType.DI);
                }
            }

            // 분 데이터 저장이 완료되었으면 시간 데이터 처리
            if (aiDataSaved || diDataSaved)
            {
                await ProcessHourQueueAsync();
            }
        }

        /// <summary>
        /// 59분 데이터 확인 및 시간 처리 준비
        /// </summary>
        private void CheckAndPrepareHourData<T>(List<(string tagName, DateTime dataTime, T trend)> batch, EnumTagType tagType)
        {
            foreach (var item in batch)
            {
                if (item.dataTime.Minute == 59)
                {
                    // 시간별로 한 번만 처리하도록 키 생성
                    string hourKey = string.Format("{0}_{1:yyyyMMddHH}", item.tagName, item.dataTime);

                    lock (_hourKeysLock)
                    {
                        if (!_processedHourKeys.Contains(hourKey))
                        {
                            _processedHourKeys.Add(hourKey);

                            // 태그 찾기
                            TagPublicClass tp = FindTagByName(item.tagName);
                            if (tp != null)
                            {
                                _hourProcessQueue.Enqueue((tp, item.dataTime));
                            }
                        }

                        // 메모리 누수 방지: 2시간 이상 지난 키 정리 (1시간마다 실행)
                        if ((DateTime.Now - _lastHourKeysCleanup).TotalHours >= 1.0 && _processedHourKeys.Count > 0)
                        {
                            _lastHourKeysCleanup = DateTime.Now;
                            string cutoffPrefix = DateTime.Now.AddHours(-2).ToString("yyyyMMddHH");
                            _processedHourKeys.RemoveWhere(key =>
                            {
                                // 키 형식: "tagName_yyyyMMddHH" - 마지막 10자리가 시간 정보
                                int underscoreIdx = key.LastIndexOf('_');
                                if (underscoreIdx < 0 || underscoreIdx + 1 + 10 > key.Length) return false;
                                string timePart = key.Substring(underscoreIdx + 1);
                                return string.CompareOrdinal(timePart, cutoffPrefix) < 0;
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 태그 이름으로 태그 찾기
        /// </summary>
        private TagPublicClass FindTagByName(string tagName)
        {
            for (int i = 0; i < TagLib.tagListAll.Length; i++)
            {
                TagPublicClass tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);

                if (tp.enumTagType == EnumTagType.AI)
                {
                    TagAiClass ai = (TagAiClass)tp;
                    if (ai.tag == tagName)
                        return tp;
                }
                else if (tp.enumTagType == EnumTagType.DI)
                {
                    TagDiClass di = (TagDiClass)tp;
                    if (di.tag == tagName)
                        return tp;
                }
            }
            return null;
        }

        /// <summary>
        /// AI 배치 저장 (장애 시 백업)
        /// </summary>
        private async Task SaveAIBatchAsync(List<(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)> batch)
        {
            try
            {
                // DB 연결 상태 확인
                if (_healthMonitor.CurrentState != DbConnectionState.Connected)
                {
                    // DB 연결 불가 시 백업
                    _backupManager.BackupAIData(batch);
                    Interlocked.Add(ref _totalBackedUp, batch.Count);
                    return;
                }

                var db = DataPostgres.Instance;

                // 타임아웃 설정을 위한 Task.WhenAny 사용
                var saveTask = db.SaveMinDataAIBatch(batch);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

                var completedTask = await Task.WhenAny(saveTask, timeoutTask);

                if (completedTask == saveTask)
                {
                    // 정상 저장 완료 (saved는 중복 제거 후 실제 INSERT/UPDATE된 건수)
                    int saved = await saveTask;
                    Interlocked.Add(ref _totalSaved, saved);

                    if (saved == 0 && batch.Count > 0)
                    {
                        Debug.WriteLine(string.Format("AI 저장 실패: 0/{0}건 - 백업 수행",
                            batch.Count));
                        _backupManager.BackupAIData(batch);
                        Interlocked.Add(ref _totalBackedUp, batch.Count);
                        Interlocked.Add(ref _totalFailed, batch.Count);
                    }
                }
                else
                {
                    // 타임아웃 발생 - 백업
                    Debug.WriteLine("AI 저장 타임아웃 - 백업 수행");
                    _backupManager.BackupAIData(batch);
                    Interlocked.Add(ref _totalBackedUp, batch.Count);
                    Interlocked.Add(ref _totalFailed, batch.Count);
                }
            }
            catch (Exception ex)
            {
                // 예외 발생 - 백업
                Debug.WriteLine(string.Format("AI 저장 실패 - 백업 수행: {0}", ex.Message));
                _backupManager.BackupAIData(batch);
                Interlocked.Add(ref _totalBackedUp, batch.Count);
                Interlocked.Add(ref _totalFailed, batch.Count);
            }
        }

        /// <summary>
        /// 큐 크기 가져오기
        /// </summary>
        public int GetQueueSize()
        {
            return _aiQueue.Count + _diQueue.Count + _aiPriorityQueue.Count + _diPriorityQueue.Count + _hourProcessQueue.Count;
        }

        /// <summary>
        /// DI 배치 저장 (장애 시 백업)
        /// </summary>
        private async Task SaveDIBatchAsync(List<(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)> batch)
        {
            try
            {
                // DB 연결 상태 확인
                if (_healthMonitor.CurrentState != DbConnectionState.Connected)
                {
                    // DB 연결 불가 시 백업
                    _backupManager.BackupDIData(batch);
                    Interlocked.Add(ref _totalBackedUp, batch.Count);
                    return;
                }

                var db = DataPostgres.Instance;

                // 타임아웃 설정
                var saveTask = db.SaveMinDataDIBatch(batch);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

                var completedTask = await Task.WhenAny(saveTask, timeoutTask);

                if (completedTask == saveTask)
                {
                    // 정상 저장 완료 (saved는 중복 제거 후 실제 INSERT/UPDATE된 건수)
                    int saved = await saveTask;
                    Interlocked.Add(ref _totalSaved, saved);

                    if (saved == 0 && batch.Count > 0)
                    {
                        Debug.WriteLine(string.Format("DI 저장 실패: 0/{0}건 - 백업 수행",
                            batch.Count));
                        _backupManager.BackupDIData(batch);
                        Interlocked.Add(ref _totalBackedUp, batch.Count);
                        Interlocked.Add(ref _totalFailed, batch.Count);
                    }
                }
                else
                {
                    // 타임아웃 발생 - 백업
                    Debug.WriteLine("DI 저장 타임아웃 - 백업 수행");
                    _backupManager.BackupDIData(batch);
                    Interlocked.Add(ref _totalBackedUp, batch.Count);
                    Interlocked.Add(ref _totalFailed, batch.Count);
                }
            }
            catch (Exception ex)
            {
                // 예외 발생 - 백업
                Debug.WriteLine(string.Format("DI 저장 실패 - 백업 수행: {0}", ex.Message));
                _backupManager.BackupDIData(batch);
                Interlocked.Add(ref _totalBackedUp, batch.Count);
                Interlocked.Add(ref _totalFailed, batch.Count);
            }
        }

        /// <summary>
        /// 시간 데이터 처리 큐 처리 (분 데이터 저장 완료 후 실행)
        /// </summary>
        private async Task ProcessHourQueueAsync()
        {
            while (_hourProcessQueue.TryDequeue(out var item))
            {
                try
                {
                    var (tp, hourTime) = item;

                    if (tp.enumTagType == EnumTagType.AI)
                    {
                        TagAiClass ai = (TagAiClass)tp;
                        if (ai != null && ai.act != 0 && ai.bFileSave != 0)
                        {
                            await CheckEngineMinuteChanged.SaveHourTrendAIAsync(ai, hourTime);
                        }
                    }
                    else if (tp.enumTagType == EnumTagType.DI)
                    {
                        TagDiClass di = (TagDiClass)tp;
                        if (di != null && di.act != 0 && di.bFileSave != 0)
                        {
                            await CheckEngineMinuteChanged.SaveHourTrendDIAsync(di, hourTime);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("시간 데이터 처리 오류: {0}", ex.Message));
                    SmLog.LogError(LogCategory.DATA_SAVE, "시간 데이터 처리 오류", ex);
                }
            }
        }

        /// <summary>
        /// DB 상태 변경 이벤트 핸들러
        /// </summary>
        private async void OnDbStateChanged(object sender, DbStateChangedEventArgs e)
        {
            string msg = string.Format("DB 상태 변경: {0} → {1} ({2})",
                e.PreviousState, e.CurrentState, e.Message);

            Debug.WriteLine(msg);
            MessageDisplay.Show(msg);

            // DB 연결 복구 시 백업 데이터 자동 복구 시도
            if (e.CurrentState == DbConnectionState.Connected &&
                e.PreviousState != DbConnectionState.Connected)
            {
                Debug.WriteLine("DB 연결 복구 - 백업 데이터 복구 시도");
                await _backupManager.AutoRecoverFromBackupsAsync();
            }
        }

        /// <summary>
        /// 통계 정보 가져오기
        /// </summary>
        public string GetStatistics()
        {
            return string.Format(
                "큐: AI={0}, DI={1} | 저장={2} | 실패={3} | 백업={4} | DB상태={5}",
                _aiQueue.Count, _diQueue.Count,
                _totalSaved, _totalFailed, _totalBackedUp,
                _healthMonitor.CurrentState);
        }

        /// <summary>
        /// 종료 처리
        /// </summary>
        public async Task ShutdownAsync()
        {
            Debug.WriteLine("트렌드 저장 매니저 종료 시작...");

            // 스레드 중지
            Stop();

            // 남은 큐 데이터 모두 처리
            if (_aiQueue.Count > 0 || _diQueue.Count > 0)
            {
                Debug.WriteLine(string.Format("남은 데이터 저장 중... AI={0}, DI={1}",
                    _aiQueue.Count, _diQueue.Count));

                await ProcessBatchAsync();

                // 한 번 더 확인
                if (_aiQueue.Count > 0 || _diQueue.Count > 0)
                {
                    Debug.WriteLine("일부 데이터가 남아있음 - 백업 수행");

                    var remainAI = new List<(string, DateTime, TREND_AI_STRUCT)>();
                    while (_aiQueue.TryDequeue(out var item))
                    {
                        remainAI.Add(item);
                    }
                    if (remainAI.Count > 0)
                    {
                        _backupManager.BackupAIData(remainAI);
                    }

                    var remainDI = new List<(string, DateTime, TREND_DI_STRUCT)>();
                    while (_diQueue.TryDequeue(out var item))
                    {
                        remainDI.Add(item);
                    }
                    if (remainDI.Count > 0)
                    {
                        _backupManager.BackupDIData(remainDI);
                    }
                }
            }

            Debug.WriteLine(string.Format("트렌드 저장 매니저 종료 완료. 통계: {0}", GetStatistics()));
        }
    }

#endregion  트렌드 데이터 저장 매니저
}
