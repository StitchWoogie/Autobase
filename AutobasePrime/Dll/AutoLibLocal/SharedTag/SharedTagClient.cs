using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace AutoLibLocal
{
#pragma warning disable CS8500 //관리되는 유형('SharedTagHeader')의 주소를 사용하거나 크기를 가져오거나 포인터를 선언합니다.
    public unsafe class SharedTagClient : IDisposable
    {
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;

        private ConcurrentDictionary<string, int> _tagNameToIndex;

        private readonly long _headerSize;
        private long _directoryOffset;
        private long _dataOffset;
        private int _expectedVersion;
        private int _tagDataSize;
        private int _tagDirEntrySize;

        // Directory 재로딩 제어
        private readonly object _reloadLock = new object();
        private DateTime _lastDirectoryReload;
        private const int RELOAD_COOLDOWN_MS = 1000; // 1초 쿨다운

        // WriteTag 시 레지스트리 매번 조회 방지 — 프로세스 수명 동안 변경되지 않는 값
        private static uint _cachedSeed1;
        private static bool _seed1Cached;

        // 영속 파이프 연결 — 매 WriteTag마다 새 파이프 생성 방지
        private NamedPipeClientStream _writePipe;
        private readonly object _writeLock = new object();
        private const string WRITE_PIPE_NAME = "SCADATagWritePipe";

        private bool _disposed;

        public SharedTagClient()
        {
            _headerSize = Marshal.SizeOf<SharedTagHeader>();
            _tagNameToIndex = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _lastDirectoryReload = DateTime.MinValue;

            try
            {
                _mmf = MemoryMappedFile.OpenExisting(
                    SharedTagMemoryManager.MMF_NAME,
                    MemoryMappedFileRights.Read
                );

                _accessor = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

                ValidateVersionAndLoadDirectory();
            }
            catch (FileNotFoundException)
            {
                Cleanup();
                throw new InvalidOperationException("SCADA 서버가 실행되지 않았습니다.");
            }
            catch (Exception ex)
            {
                Cleanup();
                throw new InvalidOperationException($"MMF 연결 실패: {ex.Message}", ex);
            }
        }

        private void Cleanup()
        {
            try
            {
                DisconnectWritePipe();
                _accessor?.Dispose();
                _mmf?.Dispose();
            }
            catch { }

            _accessor = null;
            _mmf = null;
        }

        private void ValidateVersionAndLoadDirectory()
        {
            lock (_reloadLock)
            {
                byte* ptr = null;
                _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

                try
                {
#pragma warning disable CS8500 // 주소를 가져오거나, 크기를 가져오거나, 관리되는 형식에 대한 포인터를 선언합니다.
                    SharedTagHeader* header = (SharedTagHeader*)ptr;
#pragma warning restore CS8500 // 주소를 가져오거나, 크기를 가져오거나, 관리되는 형식에 대한 포인터를 선언합니다.

                    // 버전 검증
                    if (header->Version != SharedTagMemoryManager.CURRENT_VERSION)
                    {
                        throw new InvalidOperationException(
                            $"버전 불일치. 클라이언트={SharedTagMemoryManager.CURRENT_VERSION}, " +
                            $"서버={header->Version}");
                    }

                    _expectedVersion = header->Version;
                    _tagDataSize = header->TagDataStructSize;
                    _tagDirEntrySize = header->TagDirEntrySize;

                    // 구조체 크기 검증
                    int expectedTagDataSize = Marshal.SizeOf<SharedTagData>();
                    int expectedTagDirSize = Marshal.SizeOf<TagDirectoryEntry>();

                    if (_tagDataSize != expectedTagDataSize || _tagDirEntrySize != expectedTagDirSize)
                    {
                        throw new InvalidOperationException(
                            $"구조체 크기 불일치. 클라이언트를 재빌드하세요.");
                    }

                    // CRC 검증 (옵션)
                    if (header->CRC32 != 0)
                    {
                        uint calculatedCrc = CalculateHeaderCRC(header);
                        if (calculatedCrc != header->CRC32)
                        {
                            throw new InvalidOperationException(
                                "Header CRC 불일치. MMF가 손상되었을 수 있습니다.");
                        }
                    }

                    int tagCount = header->TagCount;
                    int maxTags = header->MaxTags;

                    _directoryOffset = _headerSize;
                    _dataOffset = _headerSize + _tagDirEntrySize * maxTags;

                    // Directory 로드 (Thread-Safe)
                    var newDict = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    byte* directoryPtr = ptr + _directoryOffset;

                    for (int i = 0; i < tagCount; i++)
                    {
                        TagDirectoryEntry* entry = (TagDirectoryEntry*)(directoryPtr + i * _tagDirEntrySize);

                        string tagName = GetStringFromFixed(entry->TagName, 128);
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            newDict[tagName] = entry->Index;
                        }
                    }

                    // Atomic swap
                    _tagNameToIndex = newDict;
                    _lastDirectoryReload = DateTime.Now;
                }
                finally
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
        }

        /// <summary>
        /// Directory 재로딩 (쿨다운 적용)
        /// </summary>
        private void ReloadDirectoryIfNeeded()
        {
            // 너무 자주 재로딩하지 않도록 쿨다운
            if ((DateTime.Now - _lastDirectoryReload).TotalMilliseconds < RELOAD_COOLDOWN_MS)
                return;

            // Lock을 얻지 못하면 그냥 패스 (다른 스레드가 이미 로딩 중)
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_reloadLock, 0, ref lockTaken);
                if (lockTaken)
                {
                    // 이중 체크
                    if ((DateTime.Now - _lastDirectoryReload).TotalMilliseconds < RELOAD_COOLDOWN_MS)
                        return;

                    ValidateVersionAndLoadDirectory();
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_reloadLock);
            }
        }

        /// <summary>
        /// 태그 읽기 (TagName으로) - Atomic Read 보장
        /// </summary>
        public bool ReadTag(string tagName, out double numericValue, out string stringValue,
                           out TagQuality quality, out bool isValid, out DateTime lastUpdate,
                           int maxRetries = 3)
        {
            numericValue = 0;
            stringValue = "";
            quality = TagQuality.Bad;
            isValid = false;
            lastUpdate = DateTime.MinValue;

            // 연결 상태 확인
            if (_accessor == null || _mmf == null)
            {
                throw new InvalidOperationException("MMF 연결이 끊어졌습니다.");
            }

            if (!_tagNameToIndex.TryGetValue(tagName, out int index))
            {
                // Directory 갱신 후 재시도 (Thread-Safe)
                ReloadDirectoryIfNeeded();

                if (!_tagNameToIndex.TryGetValue(tagName, out index))
                    return false;
            }

            return ReadTagByIndex(index, out numericValue, out stringValue,
                                 out quality, out isValid, out lastUpdate, maxRetries);
        }

        /// <summary>
        /// 태그 읽기 (Index로) - 고성능 배치 처리용
        /// </summary>
        public bool ReadTagByIndex(int tagIndex, out double numericValue, out string stringValue,
                                  out TagQuality quality, out bool isValid, out DateTime lastUpdate,
                                  int maxRetries = 3)
        {
            numericValue = 0;
            stringValue = "";
            quality = TagQuality.Bad;
            isValid = false;
            lastUpdate = DateTime.MinValue;

            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            try
            {
                byte* dataPtr = ptr + _dataOffset;
                SharedTagData* tag = (SharedTagData*)(dataPtr + tagIndex * _tagDataSize);

                // Atomic Read with retry
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    // 1. UpdateSequence 읽기 (시작)
                    byte seq1 = tag->UpdateSequence;

                    // 홀수면 쓰기 중 - 잠시 대기
                    if ((seq1 & 1) == 1)
                    {
                        Thread.SpinWait(10);
                        continue;
                    }

                    Thread.MemoryBarrier();

                    // 2. 데이터 읽기
                    byte isInit = tag->IsInitialized;
                    if (isInit == 0)
                        return false;

                    double numVal = tag->NumericValue;
                    long ticks = tag->LastUpdateTicks;
                    TagQuality qual = tag->Quality;
                    short alarmLvl = tag->AlarmLevel;
                    int deviceId = tag->SourceDeviceId;

                    // 문자열 복사
                    byte[] strBuffer = new byte[256];
                    Marshal.Copy((IntPtr)tag->StringValue, strBuffer, 0, 256);

                    Thread.MemoryBarrier();

                    // 3. UpdateSequence 재확인 (끝)
                    byte seq2 = tag->UpdateSequence;

                    // 시퀀스가 같고 짝수면 일관된 데이터
                    if (seq1 == seq2 && (seq2 & 1) == 0)
                    {
                        numericValue = numVal;
                        quality = qual;
                        isValid = (qual == TagQuality.Good);

                        // DateTime 변환 (안전)
                        if (ticks >= DateTime.MinValue.Ticks && ticks <= DateTime.MaxValue.Ticks)
                        {
                            try
                            {
                                lastUpdate = new DateTime(ticks);
                            }
                            catch
                            {
                                lastUpdate = DateTime.MinValue;
                            }
                        }

                        // 문자열 변환
                        int strLen = Array.IndexOf(strBuffer, (byte)0);
                        if (strLen < 0) strLen = 256;
                        stringValue = Encoding.UTF8.GetString(strBuffer, 0, strLen);

                        return true;
                    }

                    // 불일치 - 재시도
                    Thread.SpinWait(10);
                }

                // 최대 재시도 초과
                return false;
            }
            finally
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        /// <summary>
        /// 간단한 읽기 (Quality 무시)
        /// </summary>
        public bool ReadTagSimple(string tagName, out double value)
        {
            bool result = ReadTag(tagName, out value, out _, out _, out _, out _);
            return result;
        }

        /// <summary>
        /// 배치 읽기 (고성능)
        /// </summary>
        public ConcurrentDictionary<string, TagReadResult> ReadMultipleTags(string[] tagNames)
        {
            var results = new ConcurrentDictionary<string, TagReadResult>(StringComparer.OrdinalIgnoreCase);

            foreach (var tagName in tagNames)
            {
                if (ReadTag(tagName, out double numVal, out string strVal,
                           out TagQuality qual, out bool valid, out DateTime lastUpd))
                {
                    results[tagName] = new TagReadResult
                    {
                        NumericValue = numVal,
                        StringValue = strVal,
                        Quality = qual,
                        IsValid = valid,
                        LastUpdate = lastUpd
                    };
                }
            }

            return results;
        }

        #region 영속 파이프 연결 관리

        /// <summary>
        /// 영속 파이프 연결 확보 (없거나 끊어진 경우 새로 연결)
        /// </summary>
        private void EnsureWritePipeConnected()
        {
            if (_writePipe != null && _writePipe.IsConnected)
                return;

            DisconnectWritePipe();

            _writePipe = new NamedPipeClientStream(".", WRITE_PIPE_NAME, PipeDirection.InOut, PipeOptions.None);
            _writePipe.Connect(5000);
        }

        /// <summary>
        /// 영속 파이프 연결 해제
        /// </summary>
        private void DisconnectWritePipe()
        {
            try { _writePipe?.Dispose(); } catch { }
            _writePipe = null;
        }

        /// <summary>
        /// 파이프로 요청 전송 + 응답 수신 (length-prefix 프로토콜)
        /// </summary>
        private string SendPipeRequest(byte[] jsonBytes)
        {
            // 1. 길이 전송 (4바이트)
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            _writePipe.Write(lengthBytes, 0, 4);

            // 2. 데이터 전송
            _writePipe.Write(jsonBytes, 0, jsonBytes.Length);
            _writePipe.Flush();

            // 3. 응답 길이 읽기
            byte[] responseLengthBuffer = new byte[4];
            int bytesRead = ReadExact(_writePipe, responseLengthBuffer, 0, 4);
            if (bytesRead != 4) throw new IOException("응답 길이 읽기 실패");

            int responseLength = BitConverter.ToInt32(responseLengthBuffer, 0);
            if (responseLength <= 0 || responseLength > 1024 * 1024)
                throw new IOException("비정상 응답 길이");

            // 4. 응답 데이터 읽기
            byte[] responseBuffer = new byte[responseLength];
            bytesRead = ReadExact(_writePipe, responseBuffer, 0, responseLength);
            if (bytesRead != responseLength) throw new IOException("응답 읽기 실패");

            return Encoding.UTF8.GetString(responseBuffer);
        }

        /// <summary>
        /// 인증 코드 생성에 필요한 seed1 캐시 확보
        /// </summary>
        private static void EnsureSeed1Cached()
        {
            if (!_seed1Cached)
            {
                _cachedSeed1 = (uint)TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "Code", 0);
                _seed1Cached = true;
            }
        }

        // 정확한 바이트 수 읽기
        private int ReadExact(Stream stream, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int bytesRead = stream.Read(buffer, offset + totalRead, count - totalRead);
                if (bytesRead == 0)
                    break;
                totalRead += bytesRead;
            }
            return totalRead;
        }

        #endregion

        #region 태그 쓰기 (단일/배치)

        /// <summary>
        /// 태그 쓰기 (string) — 영속 파이프 사용, 실패 시 자동 재연결 1회 시도
        /// </summary>
        public bool WriteTag(string tagName, string value, string user = "",
                            string ip = "", string computer = "")
        {
            lock (_writeLock)
            {
                EnsureSeed1Cached();
                long seed2 = DateTime.Now.Ticks;
                uint code = SharedTag.MakeCode(_cachedSeed1, (uint)seed2);

                var cmd = new
                {
                    Command = "SET",
                    TagName = tagName,
                    Value = value,
                    Seed2 = seed2,
                    Code = code,
                    Quality = (byte)TagQuality.Manual,
                    User = user,
                    IP = ip,
                    Computer = computer
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(cmd);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                // 최대 2회 시도 (영속 연결 사용 → 실패 시 재연결 1회)
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    try
                    {
                        EnsureWritePipeConnected();
                        string response = SendPipeRequest(jsonBytes);
                        return response == "OK";
                    }
                    catch
                    {
                        DisconnectWritePipe();
                        if (attempt > 0) return false;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 태그 쓰기 (double)
        /// </summary>
        public bool WriteTag(string tagName, double value, string user = "",
                         string ip = "", string computer = "")
        {
            return WriteTag(tagName, value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                          user, ip, computer);
        }

        /// <summary>
        /// 배치 태그 쓰기 — N개 태그를 1회 IPC 왕복으로 처리
        /// 반환값: 성공한 태그 수
        /// </summary>
        public int WriteTagBatch(List<KeyValuePair<string, string>> tags,
                                 string user = "", string ip = "", string computer = "")
        {
            if (tags == null || tags.Count == 0) return 0;

            lock (_writeLock)
            {
                EnsureSeed1Cached();
                long seed2 = DateTime.Now.Ticks;
                uint code = SharedTag.MakeCode(_cachedSeed1, (uint)seed2);

                var tagsArray = new Newtonsoft.Json.Linq.JArray();
                foreach (var tag in tags)
                {
                    tagsArray.Add(new Newtonsoft.Json.Linq.JObject
                    {
                        ["TagName"] = tag.Key,
                        ["Value"] = tag.Value
                    });
                }

                var cmd = new Newtonsoft.Json.Linq.JObject
                {
                    ["Command"] = "BATCH_SET",
                    ["Seed2"] = seed2,
                    ["Code"] = code,
                    ["Quality"] = (byte)TagQuality.Manual,
                    ["User"] = user,
                    ["IP"] = ip,
                    ["Computer"] = computer,
                    ["Tags"] = tagsArray
                };

                string json = cmd.ToString(Newtonsoft.Json.Formatting.None);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                // 최대 2회 시도 (영속 연결 사용 → 실패 시 재연결 1회)
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    try
                    {
                        EnsureWritePipeConnected();
                        string response = SendPipeRequest(jsonBytes);

                        // "OK:N" 형식 파싱 (N = 성공 수)
                        if (response.StartsWith("OK:"))
                        {
                            int.TryParse(response.Substring(3), out int count);
                            return count;
                        }
                        if (response == "OK") return tags.Count;
                        return 0;
                    }
                    catch
                    {
                        DisconnectWritePipe();
                        if (attempt > 0) return 0;
                    }
                }
                return 0;
            }
        }

        #endregion

        private static string GetStringFromFixed(byte* buffer, int maxLength)
        {
            int length = 0;
            while (length < maxLength && buffer[length] != 0)
                length++;

            if (length == 0)
                return string.Empty;

            byte[] temp = new byte[length];
            Marshal.Copy((IntPtr)buffer, temp, 0, length);
            return Encoding.UTF8.GetString(temp);
        }

        private static uint CalculateHeaderCRC(SharedTagHeader* header)
        {
            uint crc = 0xFFFFFFFF;

            crc = Crc32Update(crc, header->Version);
            crc = Crc32Update(crc, header->TagDataStructSize);
            crc = Crc32Update(crc, header->TagDirEntrySize);
            crc = Crc32Update(crc, header->TagCount);
            crc = Crc32Update(crc, header->MaxTags);

            return ~crc;
        }

        private static uint Crc32Update(uint crc, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            foreach (byte b in bytes)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
            }
            return crc;
        }

        public void Dispose()
        {
            if (_disposed) return;


            Cleanup();
            _disposed = true;
        }
    }
#pragma warning restore CS8500 //관리되는 유형('SharedTagHeader')의 주소를 사용하거나 크기를 가져오거나 포인터를 선언합니다.
}
