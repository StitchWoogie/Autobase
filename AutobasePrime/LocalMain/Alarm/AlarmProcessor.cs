using AutoLibLocal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain.Alarm
{
    public class AlarmProcessor
    {
        private static AlarmProcessor _instance;
        private static readonly object _lockObject = new object();

        private static readonly BlockingCollection<AlarmItem> _alarmQueue =
            new BlockingCollection<AlarmItem>(new ConcurrentQueue<AlarmItem>());

        private static Task _alarmProcessorTask;
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static DataPostgres _dataPostgres;
        private static DatabaseHealthMonitor _healthMonitor;

        // 동적 배치 크기 조정
        private const int BATCH_SIZE_MIN = 10;      // 최소 배치 크기
        private const int BATCH_SIZE_MAX = 100;     // 최대 배치 크기
        private const int BATCH_TIMEOUT_MS = 500;   // 500ms 대기

        // 큐 관리 설정
        private const int MAX_QUEUE_SIZE = 2000;      // 경보는 로그보다 중요하므로 더 많이 보관
        private const int QUEUE_CLEANUP_SIZE = 400;   // 한 번에 제거할 경보 개수

        private static int _currentBatchSize = 50;  // 현재 배치 크기 (동적 조정)

        public static AlarmProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AlarmProcessor();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 경보 프로세서 초기화 (애플리케이션 시작 시 호출)
        /// </summary>
        public static void Initialize()
        {
            if (_dataPostgres == null)
                _dataPostgres = DataPostgres.Instance;

            // DatabaseHealthMonitor 초기화 (싱글톤)
            if (!DatabaseHealthMonitor.IsInitialized &&
                !string.IsNullOrEmpty(ConfigDataDB.sConnectionString))
            {
                DatabaseHealthMonitor.Initialize(ConfigDataDB.sConnectionString);
            }

            // 초기화 후 로컬 참조 저장 및 이벤트 구독
            if (DatabaseHealthMonitor.IsInitialized)
            {
                _healthMonitor = DatabaseHealthMonitor.Instance;
            }

            _alarmProcessorTask = Task.Run(async () =>
            {
                var batch = new List<AlarmItem>();
                var lastSaveTime = DateTime.UtcNow;

                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // 먼저 DB 상태와 큐 크기 확인
                            bool isDbConnected = _healthMonitor?.CurrentState == DbConnectionState.Connected;
                            int currentQueueSize = _alarmQueue.Count;

                            // 큐가 최대 크기를 초과한 경우 정리
                            if (currentQueueSize > MAX_QUEUE_SIZE)
                            {
                                int removedCount = CleanupOldAlarms();
                                string warningMsg = $"경보 큐 크기 초과 ({currentQueueSize}개). 오래된 경보 {removedCount}개 제거됨";
                                Debug.WriteLine(warningMsg);
                                WriteErrorToFile(warningMsg);
                            }

                            // DB가 연결되지 않은 경우 대기
                            if (!isDbConnected)
                            {
                                if (batch.Count > 0)
                                {
                                    Debug.WriteLine($"DB 연결 불량. 현재 경보 배치 {batch.Count}개 보관 중. 큐 대기: {_alarmQueue.Count}개");
                                }

                                // 일정 시간 대기 후 다시 확인
                                await Task.Delay(BATCH_TIMEOUT_MS, _cts.Token);
                                continue;
                            }

                            if (_alarmQueue.TryTake(out var item, BATCH_TIMEOUT_MS, _cts.Token))
                            {
                                batch.Add(item);

                                // 동적 배치 크기 도달 시 저장
                                if (batch.Count >= _currentBatchSize)
                                {
                                    var saveStartTime = DateTime.UtcNow;

                                    // 배치 복사 후 즉시 비우기
                                    var batchToSave = new List<AlarmItem>(batch);
                                    batch.Clear();

                                    // 복사본 저장
                                    await SaveAlarmBatch(batchToSave);

                                    // 저장 시간 측정 및 배치 크기 동적 조정
                                    var saveTime = (DateTime.UtcNow - saveStartTime).TotalMilliseconds;
                                    AdjustBatchSize(saveTime, _alarmQueue.Count);

                                    lastSaveTime = DateTime.UtcNow;
                                }
                            }
                            else
                            {
                                // 타임아웃: 대기 중인 데이터 저장
                                if (batch.Count > 0)
                                {
                                    var batchToSave = new List<AlarmItem>(batch);
                                    batch.Clear();
                                    await SaveAlarmBatch(batchToSave);
                                    lastSaveTime = DateTime.UtcNow;
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            throw; // 상위 catch로 전파하여 정상 종료 처리
                        }
                        catch (Exception ex)
                        {
                            // 개별 반복 예외 → 루프 계속 (경보 프로세서 영구 정지 방지)
                            Debug.WriteLine($"[AlarmProcessor] 루프 반복 오류: {ex.Message}");
                            WriteErrorToFile($"루프 반복 오류: {ex.Message}");
                            await Task.Delay(1000); // 빠른 실패 루프 방지
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("경보 프로세서 종료");
                }
                catch (Exception ex)
                {
                    // while 루프 자체가 실패한 치명적 상황
                    Debug.WriteLine($"[AlarmProcessor] 치명적 오류로 프로세서 종료: {ex}");
                    WriteErrorToFile($"치명적 오류로 프로세서 종료: {ex.Message}");
                }
                finally
                {
                    // 종료 시나 오류 시 진입. 종료 시에는 await 이후 로직 실행 안됨. 
                    // 종료 시 남은 배치와 큐의 모든 항목 저장
                    Debug.WriteLine($"경보 프로세서 정리 시작. 배치: {batch.Count}개, 큐: {_alarmQueue.Count}개");

                    // 현재 배치에 큐의 모든 항목 추가
                    while (_alarmQueue.TryTake(out var remainingItem, 0))
                    {
                        batch.Add(remainingItem);
                    }

                    if (batch.Count > 0)
                    {
                        try
                        {
                            await SaveAlarmBatch(batch);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"종료 시 경보 배치 저장 실패: {ex.Message}");
                        }
                    }
                }
            }, _cts.Token);
        }


        ///// <summary>
        ///// DB 상태 변경 이벤트 핸들러
        ///// </summary>
        //private static void OnDbStateChanged(object sender, DbStateChangedEventArgs e)
        //{
        //    Debug.WriteLine($"[AlarmProcessor] DB 상태 변경: {e.PreviousState} → {e.CurrentState}, 메시지: {e.Message}");

        //    // DB 연결 복구 시
        //    if (e.CurrentState == DbConnectionState.Connected)
        //    {
        //        int queueCount = _alarmQueue.Count;
        //        if (queueCount > 0)
        //        {
        //            Debug.WriteLine($"[AlarmProcessor] DB 연결 복구됨. 대기 중인 경보 {queueCount}개 저장 시작");
        //        }
        //    }
        //    // DB 연결 끊김 시
        //    else if (e.PreviousState == DbConnectionState.Connected)
        //    {
        //        Debug.WriteLine($"[AlarmProcessor] DB 연결 끊김. 현재 큐: {_alarmQueue.Count}개");
        //    }
        //}

        /// <summary>
        /// 배치 크기 동적 조정 (선택적 최적화)
        /// </summary>
        private static void AdjustBatchSize(double saveTimeMs, int queueSize)
        {
            // 큐에 대기 중인 경보가 많으면 배치 크기 증가
            if (queueSize > 100 && _currentBatchSize < BATCH_SIZE_MAX)
            {
                _currentBatchSize = Math.Min(_currentBatchSize + 10, BATCH_SIZE_MAX);
                Debug.WriteLine($"배치 크기 증가: {_currentBatchSize} (큐: {queueSize}개)");
            }
            // 큐가 비어있고 저장이 빠르면 배치 크기 감소
            else if (queueSize < 10 && saveTimeMs < 100 && _currentBatchSize > BATCH_SIZE_MIN)
            {
                _currentBatchSize = Math.Max(_currentBatchSize - 10, BATCH_SIZE_MIN);
                Debug.WriteLine($"배치 크기 감소: {_currentBatchSize}");
            }
        }

        /// <summary>
        /// 오래된 경보 제거 (큐가 가득 찼을 때)
        /// </summary>
        private static int CleanupOldAlarms()
        {
            int removedCount = 0;
            var removedAlarms = new List<AlarmItem>();

            // QUEUE_CLEANUP_SIZE 만큼 오래된 경보 제거
            for (int i = 0; i < QUEUE_CLEANUP_SIZE; i++)
            {
                if (_alarmQueue.TryTake(out var oldEntry, 0))
                {
                    removedAlarms.Add(oldEntry);
                    removedCount++;
                }
                else
                {
                    break;
                }
            }

            //// 제거된 경보 정보를 파일에 기록
            //if (removedCount > 0)
            //{
            //    try
            //    {
            //        string errorDir = String.Format("{0}\\ALARM_DbError", ConfigData.sDirDataLog);
            //        if (!System.IO.Directory.Exists(errorDir))
            //        {
            //            System.IO.Directory.CreateDirectory(errorDir);
            //        }

            //        DateTime now = DateTime.Now;
            //        string filename = String.Format("{0}\\{1:0000}{2:00}{3:00}_cleanup.log",
            //            errorDir, now.Year, now.Month, now.Day);

            //        using (var writer = new System.IO.StreamWriter(filename, true))
            //        {
            //            writer.WriteLine($"{now:yyyy-MM-dd HH:mm:ss}\t경보 큐 정리: {removedCount}개 제거");
            //            writer.WriteLine($"  - 시작: {removedAlarms.First().AlarmDateTime:yyyy-MM-dd HH:mm:ss}");
            //            writer.WriteLine($"  - 종료: {removedAlarms.Last().AlarmDateTime:yyyy-MM-dd HH:mm:ss}");
            //            writer.WriteLine($"  - 남은 큐: {_alarmQueue.Count}개");

            //            // 경보는 중요하므로 제거된 항목 요약 기록
            //            writer.WriteLine("  - 제거된 경보 요약:");
            //            var priorityGroups = removedAlarms.GroupBy(a => a.Priority)
            //                .OrderBy(g => g.Key);
            //            foreach (var group in priorityGroups)
            //            {
            //                writer.WriteLine($"    Priority {group.Key}: {group.Count()}개");
            //            }
            //            writer.WriteLine();
            //        }
                //}
                //catch (Exception ex)
                //{
                //    Debug.WriteLine($"경보 큐 정리 로그 기록 실패: {ex.Message}");
                //}
            //}

            return removedCount;
        }

        /// <summary>
        /// 중요 오류를 파일에 기록
        /// </summary>
        private static void WriteErrorToFile(string errorMessage)
        {
            try
            {
                string errorDir = String.Format("{0}\\ALARM_DbError", ConfigData.sDirDataLog);
                if (!System.IO.Directory.Exists(errorDir))
                {
                    System.IO.Directory.CreateDirectory(errorDir);
                }

                DateTime now = DateTime.Now;
                string filename = String.Format("{0}\\{1:0000}{2:00}{3:00}.log",
                    errorDir, now.Year, now.Month, now.Day);

                string logLine = String.Format("{0:yyyy-MM-dd HH:mm:ss}\t{1}",
                    now, errorMessage);

                System.IO.File.AppendAllText(filename, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"오류 파일 기록 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 경보 배치 저장
        /// </summary>
        private static async Task SaveAlarmBatch(List<AlarmItem> batch)
        {
            if (batch.Count == 0) return;

            try
            {
                var alarmDataList = batch.Select(item => new DataPostgres.AlarmData
                {
                    AlarmDateTime = item.AlarmDateTime,
                    TagName = item.TagName,
                    Description = item.Description,
                    Message = item.Message,
                    AlarmType = item.AlarmType,
                    Priority = item.Priority,
                    Port = item.Port,
                    Station = item.Station,
                    Address = item.Address,
                    SubType = item.SubType,
                    Username = item.Username,
                    IpAddress = item.IpAddress,
                    ComputerName = item.ComputerName
                }).ToList();

                int savedCount = await _dataPostgres.SaveAlarmsBatch(alarmDataList);

                if (savedCount > 0)
                {
                    Debug.WriteLine($"경보 배치 저장: {savedCount}개");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"경보 배치 저장 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 경보 추가 (ALARM_FILE_STRUCT 사용)
        /// </summary>
        public void AddAlarm(ALARM_FILE_STRUCT alarm)
        {
            try
            {
                var localDateTime = new DateTime(
                 alarm.t.wYear,
                 alarm.t.wMonth,
                 alarm.t.wDay,
                 alarm.t.wHour,
                 alarm.t.wMinute,
                 alarm.t.wSecond,
                 alarm.t.wMilliseconds,
                 DateTimeKind.Local
             );

                //var utcDateTime = localDateTime.ToUniversalTime();

                Debug.WriteLine($"경보 추가: {alarm.tag}, 시간: {localDateTime:yyyy-MM-dd HH:mm:ss.fff}");


                _alarmQueue.Add(new AlarmItem
                {
                    AlarmDateTime = localDateTime.ToUniversalTime(),
                    TagName = alarm.tag ?? "",
                    Description = alarm.description ?? "",
                    Message = alarm.msg ?? "",
                    AlarmType = alarm.alarm_type,
                    Priority = alarm.priority,
                    Port = alarm.port,
                    Station = alarm.station,
                    Address = alarm.address,
                    SubType = alarm.alarm_sub_type,
                    Username = alarm.user ?? "",
                    IpAddress = alarm.ip ?? "",
                    ComputerName = alarm.computer ?? ""
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"경보 큐 추가 실패: {ex.Message}");
            }
        }


        /// <summary>
        /// 프로그램 종료 시 호출
        /// </summary>
        public static void Shutdown(int timeoutMs = 10000)
        {
            try
            {
                _alarmQueue.CompleteAdding();
                Debug.WriteLine($"대기 중인 경보: {_alarmQueue.Count}개");

                _cts.Cancel();

                if (_alarmProcessorTask != null)
                {
                    if (_alarmProcessorTask.Wait(timeoutMs))
                    {
                        Debug.WriteLine("모든 경보 저장 완료");
                    }
                    else
                    {
                        Debug.WriteLine($"경보 저장 타임아웃 (남은 경보: {_alarmQueue.Count}개)");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"경보 프로세서 종료 오류: {ex.Message}");
            }
        }

        private static string GetLocalIpAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }
            return "127.0.0.1";
        }
    }

    #region Helper Class

    /// <summary>
    /// 경보 아이템
    /// </summary>
    public class AlarmItem
    {
        public DateTime AlarmDateTime { get; set; }
        public string TagName { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public ushort AlarmType { get; set; }      // ushort
        public ushort Priority { get; set; }       // ushort
        public ushort Port { get; set; }           // ushort
        public ushort Station { get; set; }        // ushort
        public uint Address { get; set; }          // uint
        public ushort SubType { get; set; }        // ushort
        public string Username { get; set; }
        public string IpAddress { get; set; }
        public string ComputerName { get; set; }
    }

    #endregion
}
