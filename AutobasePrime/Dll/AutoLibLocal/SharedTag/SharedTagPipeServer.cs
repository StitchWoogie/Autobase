using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AutoLibLocal
{
    /// <summary>
    /// 외부 프로그램의 태그 쓰기 요청을 처리하는 Named Pipe 서버
    /// </summary>
    public class SharedTagPipeServer : IDisposable
    {
        private readonly SharedTagMemoryManager _memoryManager;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _acceptTask;
        private readonly ConcurrentDictionary<int, Task> _clientTasks;
        private int _taskIdCounter;
        private bool _disposed;
        private uint _seed1;

        private const int MAX_CONCURRENT_CLIENTS = 10;
        private const string PIPE_NAME = "SCADATagWritePipe";
        private readonly SemaphoreSlim _connectionSemaphore;

        // 엔진 태그 쓰기 콜백 
        public delegate void TagWriteCallback(string tagName, string value, string user, string ip, string computer);
        private TagWriteCallback _onTagWrite;

        public SharedTagPipeServer(SharedTagMemoryManager memoryManager)
        {
            _memoryManager = memoryManager ?? throw new ArgumentNullException(nameof(memoryManager));
            _clientTasks = new ConcurrentDictionary<int, Task>();
            _connectionSemaphore = new SemaphoreSlim(MAX_CONCURRENT_CLIENTS);
            _seed1 = (uint)TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "Code", 0);
        }

        /// <summary>
        /// 태그 쓰기 콜백 설정 (엔진 태그값 업데이트용)
        /// </summary>
        public void SetTagWriteCallback(TagWriteCallback callback)
        {
            _onTagWrite = callback;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _acceptTask = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));
            System.Diagnostics.Debug.WriteLine("[Pipe Server] 시작됨");
        }

        private volatile bool _stopping;

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && !_stopping)
            {
                NamedPipeServerStream pipe = null;

                try
                {
                    // 동시 연결 제한
                    await _connectionSemaphore.WaitAsync(token);

                    pipe = new NamedPipeServerStream(
                        PIPE_NAME,
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte, // Message 대신 Byte 모드
                        PipeOptions.Asynchronous);

                    // 비동기로 연결 대기
                    await pipe.WaitForConnectionAsync(token);

                    // 각 클라이언트를 별도 Task로 처리 (완료 시 자동 제거)
                    int taskId = Interlocked.Increment(ref _taskIdCounter);
                    var clientTask = HandleClientAsync(pipe);
                    _clientTasks[taskId] = clientTask;
                    // 완료 시 딕셔너리에서 자동 제거 → 메모리 누수 방지
                    clientTask.ContinueWith(_ => _clientTasks.TryRemove(taskId, out _));
                }
                catch (OperationCanceledException)
                {
                    pipe?.Dispose(); // 즉시 깨짐
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Pipe Server] Accept 에러: {ex.Message}");
                    pipe?.Dispose();
                    _connectionSemaphore.Release();
                }
            }
        }

        /// <summary>
        /// 영속 연결 루프 모드: 클라이언트가 연결을 끊거나 유휴 타임아웃까지 반복 처리
        /// 기존 단발 클라이언트도 호환 (1회 요청 후 연결 종료 → 루프 자동 종료)
        /// </summary>
        private async Task HandleClientAsync(NamedPipeServerStream pipeServer)
        {
            try
            {
                using (pipeServer)
                {
                    while (pipeServer.IsConnected && !_stopping)
                    {
                        // 유휴 타임아웃: 30초간 메시지 없으면 연결 종료
                        using (var idleCts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                        {
                            try
                            {
                                // 1. 길이 읽기 (4바이트)
                                byte[] lengthBuffer = new byte[4];
                                int bytesRead = await ReadExactAsync(pipeServer, lengthBuffer, 0, 4, idleCts.Token);

                                if (bytesRead != 4)
                                    break; // 클라이언트 연결 종료

                                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                                if (messageLength <= 0 || messageLength > 1024 * 1024)
                                {
                                    await SendResponseAsync(pipeServer, "ERROR:Invalid message length", idleCts.Token);
                                    break;
                                }

                                // 2. 실제 데이터 읽기
                                byte[] messageBuffer = new byte[messageLength];
                                bytesRead = await ReadExactAsync(pipeServer, messageBuffer, 0, messageLength, idleCts.Token);

                                if (bytesRead != messageLength)
                                    break;

                                string request = Encoding.UTF8.GetString(messageBuffer);

                                // 3. 명령 처리
                                string response = ProcessCommand(request);

                                // 4. 응답 전송
                                await SendResponseAsync(pipeServer, response, idleCts.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                break; // 유휴 타임아웃
                            }
                            catch (IOException)
                            {
                                break; // 파이프 연결 끊김
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PipeServer] 클라이언트 처리 에러: {ex.Message}");
            }
            finally
            {
                _connectionSemaphore.Release();
            }
        }

        // 정확한 바이트 수만큼 읽기
        private async Task<int> ReadExactAsync(Stream stream, byte[] buffer, int offset, int count, CancellationToken ct)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int bytesRead = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead, ct);
                if (bytesRead == 0)
                    break; // 연결 종료
                totalRead += bytesRead;
            }
            return totalRead;
        }

        // 응답 전송 (Length-Prefix)
        private async Task SendResponseAsync(Stream stream, string message, CancellationToken ct)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

            // 길이 전송
            await stream.WriteAsync(lengthBytes, 0, 4, ct);
            // 메시지 전송
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length, ct);
            await stream.FlushAsync(ct);
        }


        private string ProcessCommand(string request)
        {
            try
            {
                var json = JObject.Parse(request);
                string command = json["Command"]?.Value<string>();

                switch (command)
                {
                    case "SET":
                        return ProcessSingleSet(json);
                    case "BATCH_SET":
                        return ProcessBatchSet(json);
                    default:
                        return "ERROR:Invalid command";
                }
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                return $"ERROR:JSON parse error:{ex.Message}";
            }
            catch (Exception ex)
            {
                return $"ERROR:Exception:{ex.Message}";
            }
        }

        /// <summary>
        /// 인증 검증 (SET/BATCH_SET 공통)
        /// </summary>
        private bool ValidateAuth(long seed2, uint clientCode, out string error)
        {
            error = null;

            if (seed2 < DateTime.MinValue.Ticks || seed2 > DateTime.MaxValue.Ticks)
            {
                error = "ERROR:Invalid timestamp";
                return false;
            }

            uint serverCode = SharedTag.MakeCode(_seed1, (uint)seed2);
            if (clientCode != serverCode)
            {
                error = "ERROR:Code mismatch";
                return false;
            }

            DateTime requestTime;
            try
            {
                requestTime = new DateTime(seed2);
            }
            catch (ArgumentOutOfRangeException)
            {
                error = "ERROR:Timestamp out of range";
                return false;
            }

            double minutesDiff = Math.Abs((DateTime.Now - requestTime).TotalMinutes);
            if (minutesDiff > 30)
            {
                error = $"ERROR:Request expired ({minutesDiff:F1} minutes old)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 단일 태그 쓰기 처리
        /// </summary>
        private string ProcessSingleSet(JObject json)
        {
            long seed2 = json["Seed2"]?.Value<long>() ?? 0;
            uint clientCode = json["Code"]?.Value<uint>() ?? 0;

            if (!ValidateAuth(seed2, clientCode, out string error))
                return error;

            string tagName = json["TagName"]?.Value<string>();
            string value = json["Value"]?.Value<string>();
            byte qualityByte = json["Quality"]?.Value<byte>() ?? 4;
            TagQuality quality = (TagQuality)qualityByte;

            bool success = _memoryManager.UpdateTag(tagName, value, quality);

            if (success && _onTagWrite != null)
            {
                string user = json["User"]?.Value<string>() ?? "";
                string ip = json["IP"]?.Value<string>() ?? "";
                string computer = json["Computer"]?.Value<string>() ?? "";
                _onTagWrite(tagName, value, user, ip, computer);
            }

            return success ? "OK" : "ERROR:Update failed";
        }

        /// <summary>
        /// 배치 태그 쓰기 처리 — N개 태그를 1회 IPC로 처리
        /// </summary>
        private string ProcessBatchSet(JObject json)
        {
            long seed2 = json["Seed2"]?.Value<long>() ?? 0;
            uint clientCode = json["Code"]?.Value<uint>() ?? 0;

            if (!ValidateAuth(seed2, clientCode, out string error))
                return error;

            string user = json["User"]?.Value<string>() ?? "";
            string ip = json["IP"]?.Value<string>() ?? "";
            string computer = json["Computer"]?.Value<string>() ?? "";
            byte qualityByte = json["Quality"]?.Value<byte>() ?? 4;
            TagQuality quality = (TagQuality)qualityByte;

            var tags = json["Tags"] as JArray;
            if (tags == null || tags.Count == 0)
                return "ERROR:No tags";

            int successCount = 0;
            foreach (var tagObj in tags)
            {
                string tagName = tagObj["TagName"]?.Value<string>();
                string tagValue = tagObj["Value"]?.Value<string>();

                if (string.IsNullOrEmpty(tagName)) continue;

                if (_memoryManager.UpdateTag(tagName, tagValue ?? "", quality))
                {
                    successCount++;
                    _onTagWrite?.Invoke(tagName, tagValue ?? "", user, ip, computer);
                }
            }

            return $"OK:{successCount}";
        }

        public void Stop()
        {
            System.Diagnostics.Debug.WriteLine("[Pipe Server] 종료 중...");

            _stopping = true;
            _cancellationTokenSource?.Cancel();

            // Accept task는 짧게만 기다림
            _acceptTask?.Wait(TimeSpan.FromSeconds(2));

            // 클라이언트는 오래 기다리지 않음
            var activeTasks = _clientTasks.Values.ToArray();
            if (activeTasks.Length > 0)
                Task.WaitAll(activeTasks, TimeSpan.FromSeconds(3));

            System.Diagnostics.Debug.WriteLine("[Pipe Server] 종료 완료");
        }


        public void Dispose()
        {
            if (_disposed) return;

            Stop();
            _cancellationTokenSource?.Dispose();
            _connectionSemaphore?.Dispose();
            _disposed = true;
        }
    }
}
