using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoLibLocal;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for SmLog.
	/// </summary>
	public class SmLog
	{


        private static readonly object lockObject = new object();  //20241024 메시지lock 추가 PSU

        private static readonly BlockingCollection<LogEntry> _logQueue =
            new BlockingCollection<LogEntry>(new ConcurrentQueue<LogEntry>());
        private static Task _logProcessorTask;
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static DataPostgres _dataPostgres;
        private static DatabaseHealthMonitor _healthMonitor;

        // 로그 배치 설정
        private const int BATCH_SIZE = 100;       // 로그 배치 개수.
        private const int BATCH_TIMEOUT_MS = 1000; // 1초 대기

        // 큐 관리 설정
        private const int MAX_QUEUE_SIZE = 1000;  // 최대 큐 크기 (DB 연결 불량 시)
        private const int QUEUE_CLEANUP_SIZE = 200;  // 한 번에 제거할 로그 개수

        public SmLog()
		{
            //
            // TODO: Add constructor logic here
            //
		}


        // 애플리케이션 시작 시 한 번만 호출
        public static void InitializeLogProcessor()
        {
            if (_dataPostgres == null)
                _dataPostgres = DataPostgres.Instance;

            // DatabaseHealthMonitor 초기화 (싱글톤)
            if (!DatabaseHealthMonitor.IsInitialized &&
                !string.IsNullOrEmpty(ConfigDataDB.sConnectionString))
            {
                DatabaseHealthMonitor.Initialize(ConfigDataDB.sConnectionString);
            }

            // 초기화 후 이벤트 구독
            if (DatabaseHealthMonitor.IsInitialized)
            {
                _healthMonitor = DatabaseHealthMonitor.Instance;
                //_healthMonitor.StateChanged += OnDbStateChanged;
            }

                _logProcessorTask = Task.Run(async () =>
            {
                var batch = new List<LogEntry>();

                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        // 먼저 DB 상태와 큐 크기 확인
                        bool isDbConnected = _healthMonitor?.CurrentState == DbConnectionState.Connected;
                        int currentQueueSize = _logQueue.Count;

                        // 큐가 최대 크기를 초과한 경우 정리
                        if (currentQueueSize > MAX_QUEUE_SIZE)
                        {
                            int removedCount = CleanupOldLogs();
                            string warningMsg = $"큐 크기 초과 ({currentQueueSize}개). 오래된 로그 {removedCount}개 제거됨";
                            Debug.WriteLine(warningMsg);
                            WriteErrorToFile(warningMsg);
                        }

                        // DB가 연결되지 않은 경우 대기
                        if (!isDbConnected)
                        {
                            // 현재 배치가 있으면 저장 시도는 하지 않고 보관
                            if (batch.Count > 0)
                            {
                                Debug.WriteLine($"DB 연결 불량. 현재 배치 {batch.Count}개 보관 중. 큐 대기: {_logQueue.Count}개");
                            }

                            // 일정 시간 대기 후 다시 확인
                            await Task.Delay(BATCH_TIMEOUT_MS, _cts.Token);
                            continue;
                        }

                        // DB 연결 정상 - 큐에서 로그 가져오기
                        if (_logQueue.TryTake(out var item, BATCH_TIMEOUT_MS, _cts.Token))
                        {
                            batch.Add(item);

                            // 배치 크기 도달 시 즉시 저장
                            if (batch.Count >= BATCH_SIZE)
                            {
                                // 배치 복사 후 즉시 비우기
                                var batchToSave = new List<LogEntry>(batch);
                                batch.Clear();

                                // 복사본 저장
                                await SaveLogBatch(batchToSave);
                            }
                        }
                        else
                        {
                            // 타임아웃: 대기 중인 데이터 저장
                            if (batch.Count > 0)
                            {
                                var batchToSave = new List<LogEntry>(batch);
                                batch.Clear();
                                await SaveLogBatch(batchToSave);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("로그 프로세서 종료");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"로그 프로세서 오류: {ex.Message}");
                }
                finally
                {
                    // 종료 시나 오류 시 진입. 종료 시에는 await 이후 로직 실행 안됨. 
                    // 종료 시 남은 배치와 큐의 모든 항목 저장
                    Debug.WriteLine($"로그 프로세서 정리 시작. 배치: {batch.Count}개, 큐: {_logQueue.Count}개");

                    // 현재 배치에 큐의 모든 항목 추가
                    while (_logQueue.TryTake(out var remainingItem, 0))
                    {
                        batch.Add(remainingItem);
                    }

                    // 남은 배치 저장
                    if (batch.Count > 0)
                    {
                        try
                        {
                            await SaveLogBatch(batch);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"종료 시 로그 배치 저장 실패: {ex.Message}");
                        }
                    }
                }
            }, _cts.Token);
        }

        /// <summary>
        /// 로그 배치 저장
        /// </summary>
        private static async Task SaveLogBatch(List<LogEntry> batch)
        {
            if (batch.Count == 0) return;

            try
            {
                int savedCount = await _dataPostgres.SaveLogsBatch(batch);

                if (savedCount > 0)
                {
                    Debug.WriteLine($"로그 배치 저장 완료: {savedCount}개 (남은 큐: {_logQueue.Count}개)");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 배치 저장 실패: {ex.Message}");
                WriteErrorToFile($"로그 배치 저장 실패 ({batch.Count}개): {ex.Message}");
            }
        }

        /// <summary>
        /// 오래된 로그 제거 (큐가 가득 찼을 때)
        /// </summary>
        private static int CleanupOldLogs()
        {
            int removedCount = 0;
            var removedLogs = new List<LogEntry>();

            // QUEUE_CLEANUP_SIZE 만큼 오래된 로그 제거
            for (int i = 0; i < QUEUE_CLEANUP_SIZE; i++)
            {
                if (_logQueue.TryTake(out var oldEntry, 0))  // 즉시 가져오기 (대기 안 함)
                {
                    removedLogs.Add(oldEntry);
                    removedCount++;
                }
                else
                {
                    break;
                }
            }

            return removedCount;
        }

        /// <summary>
        /// 중요 오류를 파일에 기록, DB 저장 실패 시. Log 텍스트파일 병행 저장 시 필요없지만 일단 추가.
        /// </summary>
        private static void WriteErrorToFile(string errorMessage)
        {
            try
            {
                string errorDir = String.Format("{0}\\LOG_DbError", ConfigData.sDirDataLog);
                if (!Directory.Exists(errorDir))
                {
                    Directory.CreateDirectory(errorDir);
                }

                DateTime now = DateTime.Now;
                string filename = String.Format("{0}\\{1:0000}{2:00}{3:00}.log",
                    errorDir, now.Year, now.Month, now.Day);

                string logLine = String.Format("{0:yyyy-MM-dd HH:mm:ss}\t{1}",
                    now, errorMessage);

                File.AppendAllText(filename, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"오류 파일 기록 실패: {ex.Message}");
            }
        }


        public static void Message(string format, params object[] args)
		{
			Message(EnumEventID.None, format, args);
		}

        static string EncodeLogOneLine(string source)
        {
            NetTools.Hash.HashString hash = new NetTools.Hash.HashString();

            hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^5);

            string target = hash.Encode(source, "LOG");

            return target;
        }

        public static void MessageSbas(string format, params object[] args)
        {
            string imsi;
            string filename;
            DateTime t = DateTime.Now;
            BinaryWriter writer;

            imsi = String.Format(format, args);

            filename = String.Format("{0}\\LOG", ConfigData.sDirDataLog);
            if (!Directory.Exists(filename))
            {
                try
                {
                    Directory.CreateDirectory(filename);
                }
                catch
                {
                    MessageDisplay.Show("Can't create folder\nFolder={0}", filename);
                    return;
                }
            }

            filename = String.Format("{0}\\LOG\\{1:0000}{2:00}{3:00}.logx", ConfigData.sDirDataLog, t.Year, t.Month, t.Day);
            FileStream fs;
            try
            {
                fs = File.Open(filename, FileMode.Create | FileMode.Append, FileAccess.Write, FileShare.Read);
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("Error:{0}\nMessage={1}", filename, exception.Message);
                fs = null;
                return;
            }

            writer = new BinaryWriter(fs);

            if (writer == null) return;

            string one_line = String.Format("{0:00}:{1:00}\t{2}", t.Hour, t.Minute, imsi);

            one_line = EncodeLogOneLine(one_line);

            if (one_line.Length > 0)
            {
                byte[] data = new byte[one_line.Length+1];
                for (int i = 0; i < one_line.Length; i++)
                {
                    data[i] = (byte)(one_line[i] - 46);
                }
                data[one_line.Length] = 1;
                writer.Write(data);
            }

            writer.Close();
        }


        public static void Message(EnumEventID eid, string format, params object[] args)
        {
            lock (lockObject)
            {
                if (TotalConfig.eOemType == EnumOemType.SBAS)
                {
                    MessageSbas(format, args);
                    return;
                }

                string imsi;
                string filename;
                DateTime t = DateTime.Now;
                TextWriter writer;

                imsi = String.Format(format, args);

                filename = String.Format("{0}\\LOG", ConfigData.sDirDataLog);
                if (!Directory.Exists(filename))
                {
                    try
                    {
                        Directory.CreateDirectory(filename);
                    }
                    catch
                    {
                        MessageDisplay.Show("Can't create folder\nFolder={0}", filename);
                        return;
                    }
                }

                filename = String.Format("{0}\\LOG\\{1:0000}{2:00}{3:00}.logx", ConfigData.sDirDataLog, t.Year, t.Month, t.Day);
                try
                {
                    writer = new StreamWriter(filename, true);
                }
                catch (Exception exception)
                {
                    MessageDisplay.Show("Error:{0}\nMessage={1}", filename, exception.Message);
                    writer = null;
                }
                if (writer == null) return;

                string one_line = String.Format("{0:00}:{1:00}\t{2}", t.Hour, t.Minute, imsi);

                if (TotalConfig.eOemType == EnumOemType.SBAS)
                {
                    //one_line = EncodeLogOneLine(one_line);
                }

                writer.WriteLine("{0}", one_line);
                writer.Close();

                AutoLibLocal.EventSave.Log(imsi, eid);

                // 2. DB 저장 큐에 추가 (즉시 반환)
                try
                {
                    var (level, category) = ConvertEventIdToLogInfo(eid);

                    _logQueue.Add(new LogEntry
                    {
                        LogDateTime = t,
                        Level = level,
                        Category = category,
                        Message = imsi,
                        Username = GetCurrentUsername(),
                        IpAddress = GetClientIpAddress(),
                        MachineName = Environment.MachineName,
                        Detail = null
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"로그 큐 추가 실패: {ex.Message}");
                }
            }
            // if (_dataPostgres == null) _dataPostgres = DataPostgres.Instance;
            //_dataPostgres.SaveLog(DateTime.Now, String.Format(format, args), (int)eid));


        }

        /// <summary>
        /// 로그 메시지 (새로운 방식 - Level과 Category 직접 지정)
        /// </summary>
        public static void Message(LogLevel level, int category, string format, params object[] args)
        {
            lock (lockObject)
            {
                string imsi;
                string filename;
                DateTime t = DateTime.Now;
                TextWriter writer;

                imsi = String.Format(format, args);

                // 1. 파일 저장
                filename = String.Format("{0}\\LOG", ConfigData.sDirDataLog);
                if (!Directory.Exists(filename))
                {
                    try
                    {
                        Directory.CreateDirectory(filename);
                    }
                    catch
                    {
                        MessageDisplay.Show("Can't create folder\nFolder={0}", filename);
                        return;
                    }
                }

                filename = String.Format("{0}\\LOG\\{1:0000}{2:00}{3:00}.logx",
                    ConfigData.sDirDataLog, t.Year, t.Month, t.Day);

                try
                {
                    writer = new StreamWriter(filename, true);
                }
                catch (Exception exception)
                {
                    MessageDisplay.Show("Error:{0}\nMessage={1}", filename, exception.Message);
                    writer = null;
                }

                if (writer == null) return;

                string one_line = String.Format("{0:00}:{1:00}\t[{2}][{3}]\t{4}",
                    t.Hour, t.Minute, level, LogCategory.GetCategoryName(category), imsi);

                writer.WriteLine("{0}", one_line);
                writer.Close();

                // 2. DB 저장 큐에 추가
                try
                {
                    _logQueue.Add(new LogEntry
                    {
                        LogDateTime = t,
                        Level = level,
                        Category = category,
                        Message = imsi,
                        Username = GetCurrentUsername(),
                        IpAddress = GetClientIpAddress(),
                        MachineName = Environment.MachineName,
                        Detail = null
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"로그 큐 추가 실패: {ex.Message}");
                }
            }
        }



        // 프로그램 종료 시 호출
        public static void ShutdownLogProcessor(int timeoutMs = 5000)
        {
            try
            {
                // 1. 새 항목 추가 차단
                _logQueue.CompleteAdding();
                Debug.WriteLine($"대기 중인 로그: {_logQueue.Count}개");

                // 2. 취소 신호 전송
                _cts.Cancel();

                // Task 완료 대기 (타임아웃 포함)
                if (_logProcessorTask != null)
                {
                    if (_logProcessorTask.Wait(timeoutMs))
                    {
                        Debug.WriteLine("모든 로그 저장 완료");
                    }
                    else
                    {
                        Debug.WriteLine($"로그 저장 타임아웃 (남은 로그: {_logQueue.Count}개)");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 프로세서 종료 오류: {ex.Message}");
            }
        }



        /// <summary>
        /// 예외 정보와 함께 로그 메시지
        /// </summary>
        public static void MessageWithException(LogLevel level, int category, Exception exception, string format, params object[] args)
        {
            string detail = null;
            if (exception != null)
            {
                // JSON 형식으로 예외 정보 저장
                detail = $"{{\"error\":\"{EscapeJson(exception.Message)}\"," +
                         $"\"type\":\"{exception.GetType().Name}\"," +
                         $"\"stackTrace\":\"{EscapeJson(exception.StackTrace)}\"}}";
            }

            lock (lockObject)
            {
                string imsi = String.Format(format, args);
                DateTime t = DateTime.Now;

                // 1. 파일 저장 (예외 정보 포함)
                string filename = String.Format("{0}\\LOG_Error", ConfigData.sDirDataLog);
                if (!Directory.Exists(filename))
                {
                    try
                    {
                        Directory.CreateDirectory(filename);
                    }
                    catch
                    {
                        MessageDisplay.Show("Can't create folder\nFolder={0}", filename);
                        return;
                    }
                }

                filename = String.Format("{0}\\LOG_Error\\{1:0000}{2:00}{3:00}.logx",
                    ConfigData.sDirDataLog, t.Year, t.Month, t.Day);

                try
                {
                    using (TextWriter writer = new StreamWriter(filename, true))
                    {
                        string one_line = String.Format("{0:00}:{1:00}\t[{2}][{3}]\t{4}",
                            t.Hour, t.Minute, level, LogCategory.GetCategoryName(category), imsi);
                        writer.WriteLine("{0}", one_line);

                        if (exception != null)
                        {
                            writer.WriteLine($"\tException: {exception.Message}");
                            writer.WriteLine($"\tType: {exception.GetType().Name}");
                            if (!string.IsNullOrEmpty(exception.StackTrace))
                            {
                                writer.WriteLine($"\tStackTrace: {exception.StackTrace}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"로그 파일 저장 실패: {ex.Message}");
                }

                // 2. DB 저장 큐에 추가
                try
                {
                    _logQueue.Add(new LogEntry
                    {
                        LogDateTime = t,
                        Level = level,
                        Category = category,
                        Message = imsi,
                        Username = GetCurrentUsername(),
                        IpAddress = GetClientIpAddress(),
                        MachineName = Environment.MachineName,
                        Detail = detail
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"로그 큐 추가 실패: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 간편 로그 메서드들
        /// </summary>
        public static void LogInfo(int category, string format, params object[] args)
        {
            Message(LogLevel.INFO, category, format, args);
        }

        public static void LogWarning(int category, string format, params object[] args)
        {
            Message(LogLevel.WARNING, category, format, args);
        }

        public static void LogError(int category, string format, params object[] args)
        {
            Message(LogLevel.ERROR, category, format, args);
        }

        public static void LogError(int category, Exception ex, string format, params object[] args)
        {
            MessageWithException(LogLevel.ERROR, category, ex, format, args);
        }

        public static void LogCritical(int category, string format, params object[] args)
        {
            Message(LogLevel.CRITICAL, category, format, args);
        }

        public static void LogCritical(int category, Exception ex, string format, params object[] args)
        {
            MessageWithException(LogLevel.CRITICAL, category, ex, format, args);
        }

        /// <summary>
        /// 현재 사용자명 가져오기
        /// </summary>
        private static string GetCurrentUsername()
        {
            // 실제 구현은 프로젝트의 사용자 관리 시스템에 맞게 수정
            try
            {
                // 예: 로그인한 사용자 정보가 있는 경우
                // return CurrentUser.Username;
                return ConfigRunMain.sUserName;
            }
            catch
            {
                return "SYSTEM";
            }
        }

        /// <summary>
        /// 클라이언트 IP 주소 가져오기
        /// </summary>
        private static string GetClientIpAddress()
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

        /// <summary>
        /// JSON 문자열 이스케이프
        /// </summary>
        private static string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("\\", "\\\\")
                       .Replace("\"", "\\\"")
                       .Replace("\r", "\\r")
                       .Replace("\n", "\\n")
                       .Replace("\t", "\\t");
        }

        /// <summary>
        /// EventID를 LogLevel과 Category로 변환
        /// </summary>
        private static (LogLevel level, int category) ConvertEventIdToLogInfo(EnumEventID eid)
        {
            // EnumEventID에 따라 적절한 Level과 Category 매핑
            switch (eid)
            {
                // 시스템 시작/종료
                case EnumEventID.ProgramStart:
                    return (LogLevel.INFO, LogCategory.PROGRAM_START);
                case EnumEventID.ProgramEnd:
                    return (LogLevel.INFO, LogCategory.PROGRAM_END);
                case EnumEventID.SystemStart:
                    return (LogLevel.INFO, LogCategory.SYSTEM_START);
                case EnumEventID.SystemStop:
                    return (LogLevel.INFO, LogCategory.SYSTEM_STOP);

                // 사용자 활동
                case EnumEventID.UserLogin:
                    return (LogLevel.INFO, LogCategory.SECURITY_LOGIN);
                case EnumEventID.UserLogout:
                    return (LogLevel.INFO, LogCategory.SECURITY_LOGOUT);

                // 데이터 관련
                case EnumEventID.DataSave:
                    return (LogLevel.INFO, LogCategory.DATA_SAVE);
                case EnumEventID.DataDelete:
                    return (LogLevel.WARNING, LogCategory.DATA_DELETE);
                case EnumEventID.DataExport:
                    return (LogLevel.INFO, LogCategory.DATA_EXPORT);
                case EnumEventID.DataImport:
                    return (LogLevel.INFO, LogCategory.DATA_IMPORT);

                // 통신 관련
                case EnumEventID.CommError:
                    return (LogLevel.ERROR, LogCategory.COMM_ERROR);
                case EnumEventID.CommRecover:
                    return (LogLevel.INFO, LogCategory.COMM_RECOVER);
                case EnumEventID.DeviceError:
                    return (LogLevel.ERROR, LogCategory.DEVICE_ERROR);
                case EnumEventID.DeviceRecover:
                    return (LogLevel.INFO, LogCategory.DEVICE_RECOVER);


                // 경고/오류
                case EnumEventID.Warning:
                    return (LogLevel.WARNING, LogCategory.SYSTEM);
                case EnumEventID.Error:
                    return (LogLevel.ERROR, LogCategory.SYSTEM);
                case EnumEventID.Critical:
                    return (LogLevel.CRITICAL, LogCategory.SYSTEM);

                // 기본값
                default:
                    return (LogLevel.INFO, LogCategory.SYSTEM);
            }
        }


    }





}

