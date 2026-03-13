using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ICSharpCode.SharpZipLib.Zip.Compression.DeflaterHuffman;

namespace AutoLibLocal
{
    //    [외부 프로세스] ──SetCurr()──→ [SharedTagClient.WriteTag()] ──NamedPipe──→ [SharedTagPipeServer]
    //                                                                                    │
    //                                                                         ┌──────────┴──────────┐
    //                                                                         │ MemoryManager.       │
    //                                                                         │ UpdateTag() → MMF 갱신│
    //                                                                         │ + 엔진 콜백           │
    //                                                                         └─────────────────────┘
    //                                                                                    │
    //[외부 프로세스] ──GetCurr()──→ [SharedTagClient.ReadTag()] ←───MMF 직접 읽기────────┘

    //MMF 읽기(Atomic Read)    UpdateSequence 기반 lock-free 읽기 — SpinWait + retry로 일관성 보장.고빈도에서도 우수
    //ReaderWriterLockSlim MMF 쓰기에 적절한 잠금 전략(다수 읽기/소수 쓰기)
    //MemoryBarrier 쓰기 전후 배리어로 CPU 재배치 방지
    //Directory 캐싱 ConcurrentDictionary<string,int> + 1초 쿨다운 재로딩
    //SemaphoreSlim(10)   Named Pipe 서버 동시 연결 수 제한

#pragma warning disable CS8500 //관리되는 유형('SharedTagHeader')의 주소를 사용하거나 크기를 가져오거나 포인터를 선언합니다.
    public unsafe class SharedTagMemoryManager : IDisposable
    {
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;

        private readonly int _maxTags;
        private readonly long _headerSize;
        private readonly long _directoryOffset;
        private readonly long _directorySize;
        private readonly long _dataOffset;
        private readonly long _dataSize;

        private readonly Dictionary<string, int> _tagNameToIndex;
        private readonly ReaderWriterLockSlim _lock;
        private int _currentTagCount;
        private bool _disposed;

        // 구조체 크기 (버전 검증용)
        private readonly int _tagDataSize;
        private readonly int _tagDirEntrySize;

        public const int CURRENT_VERSION = 1;
        // Local 네임스페이스 사용 (관리자 권한 불필요)
        public const string MMF_NAME = "Local\\SCADATagShare";


        public SharedTagMemoryManager(int maxTags = 10000)
        {
            _maxTags = maxTags;
            _tagDataSize = Marshal.SizeOf<SharedTagData>();
            _tagDirEntrySize = Marshal.SizeOf<TagDirectoryEntry>();
            _headerSize = Marshal.SizeOf<SharedTagHeader>();

            _directorySize = _tagDirEntrySize * maxTags;
            _dataSize = _tagDataSize * maxTags;

            _directoryOffset = _headerSize;
            _dataOffset = _headerSize + _directorySize;

            long totalSize = _headerSize + _directorySize + _dataSize;

            _tagNameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            System.Diagnostics.Debug.WriteLine($"[MMF] 초기화 시작 - Size: {totalSize:N0} bytes ({totalSize / 1024.0 / 1024.0:F2} MB)");

            try
            {
                System.Diagnostics.Debug.WriteLine($"[MMF] CreateOrOpen 시작: {MMF_NAME}");

                // Local 네임스페이스: 동일 세션 내에서만 공유, 관리자 권한 불필요
                _mmf = MemoryMappedFile.CreateOrOpen(
                    MMF_NAME,
                    totalSize,
                    MemoryMappedFileAccess.ReadWrite
                );

                System.Diagnostics.Debug.WriteLine("[MMF] CreateOrOpen 완료");

                _accessor = _mmf.CreateViewAccessor(0, totalSize);

                System.Diagnostics.Debug.WriteLine("[MMF] CreateViewAccessor 완료");

                InitializeOrLoadHeader();

                System.Diagnostics.Debug.WriteLine("[MMF] 초기화 완료");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MMF] 초기화 실패: {ex.Message}");
                throw new InvalidOperationException(
                    $"Memory-Mapped File 생성 실패: {ex.Message}", ex);
            }
        }


        private void InitializeOrLoadHeader()
        {
            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            try
            {
                SharedTagHeader* header = (SharedTagHeader*)ptr;

                bool needsInitialization = false;

                if (header->Version == 0)
                {
                    needsInitialization = true;
                }
                else if (header->Version != CURRENT_VERSION)
                {
                    throw new InvalidOperationException(
                        $"버전 불일치. 현재={CURRENT_VERSION}, MMF={header->Version}. " +
                        "서버를 재시작하거나 MMF를 삭제하세요.");
                }
                else if (header->TagDataStructSize != _tagDataSize ||
                         header->TagDirEntrySize != _tagDirEntrySize ||
                         header->MaxTags != _maxTags)
                {
                    throw new InvalidOperationException(
                        "구조체 크기 또는 MaxTags 불일치. MMF를 삭제하고 재시작하세요.");
                }

                if (needsInitialization)
                {
                    // 새로 초기화
                    header->Version = CURRENT_VERSION;
                    header->TagDataStructSize = _tagDataSize;
                    header->TagDirEntrySize = _tagDirEntrySize;
                    header->TagCount = 0;
                    header->MaxTags = _maxTags;
                    header->LastUpdateTicks = DateTime.Now.Ticks;
                    header->Reserved1 = 0;

                    // CRC 계산
                    header->CRC32 = CalculateHeaderCRC(header);

                    _currentTagCount = 0;
                }
                else
                {
                    // CRC 검증
                    uint storedCrc = header->CRC32;
                    uint calculatedCrc = CalculateHeaderCRC(header);

                    if (storedCrc != 0 && storedCrc != calculatedCrc)
                    {
                        throw new InvalidOperationException(
                            "Header CRC 불일치. MMF가 손상되었을 수 있습니다.");
                    }

                    // 기존 데이터 로드 (서버 재시작 시)
                    _currentTagCount = header->TagCount;
                    LoadExistingTags();
                }
            }
            finally
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        private void LoadExistingTags()
        {
            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            try
            {
                byte* directoryPtr = ptr + _directoryOffset;

                for (int i = 0; i < _currentTagCount; i++)
                {
                    TagDirectoryEntry* entry = (TagDirectoryEntry*)(directoryPtr + i * _tagDirEntrySize);

                    string tagName = GetStringFromFixed(entry->TagName, 128);
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        _tagNameToIndex[tagName] = entry->Index;
                    }
                }
            }
            finally
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        /// <summary>
        /// 태그 등록 (초기화 시 호출)
        /// </summary>
        public bool RegisterTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName) || tagName.Length > 120)
                return false;

            _lock.EnterUpgradeableReadLock();
            try
            {
                if (_tagNameToIndex.ContainsKey(tagName))
                    return true;

                if (_currentTagCount >= _maxTags)
                    return false;

                _lock.EnterWriteLock();
                try
                {
                    int index = _currentTagCount;
                    _tagNameToIndex[tagName] = index;

                    WriteTagDirectory(index, tagName);
                    InitializeTagData(index);

                    _currentTagCount++;
                    UpdateHeaderTagCount(_currentTagCount);

                    return true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        private void WriteTagDirectory(int index, string tagName)
        {
            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            try
            {
                byte* directoryPtr = ptr + _directoryOffset;
                TagDirectoryEntry* entry = (TagDirectoryEntry*)(directoryPtr + index * _tagDirEntrySize);

                entry->Index = index;
                entry->RegisteredTicks = DateTime.Now.Ticks;
                entry->Reserved = 0;

                byte[] nameBytes = Encoding.UTF8.GetBytes(tagName);
                int copyLen = Math.Min(nameBytes.Length, 127);

                fixed (byte* srcPtr = nameBytes)
                {
                    Buffer.MemoryCopy(srcPtr, entry->TagName, 128, copyLen);
                }
                entry->TagName[copyLen] = 0; // null terminator
            }
            finally
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        private void InitializeTagData(int index)
        {
            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            try
            {
                byte* dataPtr = ptr + _dataOffset;
                SharedTagData* tag = (SharedTagData*)(dataPtr + index * _tagDataSize);

                tag->UpdateSequence = 0;  // 짝수 = 완료
                tag->Quality = TagQuality.Bad;
                tag->IsInitialized = 0;
                tag->Reserved1 = 0;
                tag->NumericValue = 0;
                tag->LastUpdateTicks = DateTime.Now.Ticks;
                tag->SourceDeviceId = 0;
                tag->AlarmLevel = 0;
                tag->Reserved2 = 0;
                // StringValue는 fixed buffer로 이미 0 초기화됨
            }
            finally
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        /// <summary>
        /// UpdateSequence 헬퍼 - 쓰기 시작 (홀수)
        /// </summary>
        private static byte GetNextWriteSequence(byte current)
        {
            // 항상 짝수 → 홀수로 (쓰기 시작)
            // Overflow 안전: 255 → 0 → 1
            byte next = (byte)((current + 1) | 1);
            return next;
        }

        /// <summary>
        /// UpdateSequence 헬퍼 - 쓰기 완료 (짝수)
        /// </summary>
        private static byte GetCompleteSequence(byte writeSeq)
        {
            // 홀수 → 짝수로 (쓰기 완료)
            return (byte)(writeSeq + 1);
        }

        /// <summary>
        /// 문자열 → 숫자 변환 정책
        /// </summary>
        private static (double numValue, TagQuality quality) ParseValueWithQuality(
            string value, TagQuality requestedQuality)
        {
            if (double.TryParse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double numValue))
            {
                return (numValue, requestedQuality);
            }
            else
            {
                // 변환 실패 - Quality를 Uncertain으로 강등
                return (0.0, TagQuality.Uncertain);
            }
        }

        /// <summary>
        /// 태그 값 업데이트 (double) - Atomic 보장
        /// </summary>
        public bool UpdateTag(string tagName, double value, TagQuality quality = TagQuality.Good,
                             int sourceDeviceId = 0, short alarmLevel = 0)
        {
            _lock.EnterReadLock();
            try
            {
                if (!_tagNameToIndex.TryGetValue(tagName, out int index))
                {
                    _lock.ExitReadLock();
                    if (!RegisterTag(tagName))
                        return false;
                    _lock.EnterReadLock();
                    index = _tagNameToIndex[tagName];
                }

                byte* ptr = null;
                _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

                try
                {
                    byte* dataPtr = ptr + _dataOffset;
                    SharedTagData* tag = (SharedTagData*)(dataPtr + index * _tagDataSize);

                    // Atomic Update 시작 (명확한 홀수)
                    byte currentSeq = tag->UpdateSequence;
                    byte writeSeq = GetNextWriteSequence(currentSeq);
                    tag->UpdateSequence = writeSeq;

                    // Memory Barrier - 쓰기 순서 보장
                    Thread.MemoryBarrier();

                    // 실제 데이터 업데이트
                    tag->NumericValue = value;
                    tag->LastUpdateTicks = DateTime.Now.Ticks;
                    tag->Quality = quality;
                    tag->IsInitialized = 1;
                    tag->SourceDeviceId = sourceDeviceId;
                    tag->AlarmLevel = alarmLevel;

                    // 숫자를 문자열로 변환 (정밀도 보장)
                    string valueStr = value.ToString("G17",
                        System.Globalization.CultureInfo.InvariantCulture);
                    byte[] bytes = Encoding.UTF8.GetBytes(valueStr);
                    int copyLen = Math.Min(bytes.Length, 255);

                    fixed (byte* srcPtr = bytes)
                    {
                        Buffer.MemoryCopy(srcPtr, tag->StringValue, 256, copyLen);
                    }
                    tag->StringValue[copyLen] = 0;

                    // Memory Barrier
                    Thread.MemoryBarrier();

                    // Atomic Update 완료 (명확한 짝수)
                    tag->UpdateSequence = GetCompleteSequence(writeSeq);
                }
                finally
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }

                return true;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 태그 값 업데이트 (string) - Atomic 보장
        /// </summary>
        public bool UpdateTag(string tagName, string value, TagQuality quality = TagQuality.Good,
                             int sourceDeviceId = 0, short alarmLevel = 0)
        {
            _lock.EnterReadLock();
            try
            {
                if (!_tagNameToIndex.TryGetValue(tagName, out int index))
                {
                    _lock.ExitReadLock();
                    if (!RegisterTag(tagName))
                        return false;
                    _lock.EnterReadLock();
                    index = _tagNameToIndex[tagName];
                }

                byte* ptr = null;
                _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

                try
                {
                    byte* dataPtr = ptr + _dataOffset;
                    SharedTagData* tag = (SharedTagData*)(dataPtr + index * _tagDataSize);

                    // 문자열 → 숫자 변환 (정책 적용)
                    var (numValue, actualQuality) = ParseValueWithQuality(value, quality);

                    // Atomic Update 시작
                    byte currentSeq = tag->UpdateSequence;
                    byte writeSeq = GetNextWriteSequence(currentSeq);
                    tag->UpdateSequence = writeSeq;

                    Thread.MemoryBarrier();

                    tag->NumericValue = numValue;
                    tag->LastUpdateTicks = DateTime.Now.Ticks;
                    tag->Quality = actualQuality; // 변환 실패 시 자동으로 Uncertain
                    tag->IsInitialized = 1;
                    tag->SourceDeviceId = sourceDeviceId;
                    tag->AlarmLevel = alarmLevel;

                    // 문자열 저장 (원본 유지)
                    byte[] bytes = Encoding.UTF8.GetBytes(value);
                    int copyLen = Math.Min(bytes.Length, 255);

                    fixed (byte* srcPtr = bytes)
                    {
                        Buffer.MemoryCopy(srcPtr, tag->StringValue, 256, copyLen);
                    }
                    tag->StringValue[copyLen] = 0;

                    Thread.MemoryBarrier();

                    // Atomic Update 완료
                    tag->UpdateSequence = GetCompleteSequence(writeSeq);
                }
                finally
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }

                return true;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Quality만 업데이트 (통신 실패 등)
        /// </summary>
        public bool SetTagQuality(string tagName, TagQuality quality)
        {
            _lock.EnterReadLock();
            try
            {
                if (!_tagNameToIndex.TryGetValue(tagName, out int index))
                    return false;

                byte* ptr = null;
                _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

                try
                {
                    byte* dataPtr = ptr + _dataOffset;
                    SharedTagData* tag = (SharedTagData*)(dataPtr + index * _tagDataSize);

                    byte currentSeq = tag->UpdateSequence;
                    byte writeSeq = GetNextWriteSequence(currentSeq);
                    tag->UpdateSequence = writeSeq;

                    Thread.MemoryBarrier();

                    tag->Quality = quality;
                    tag->LastUpdateTicks = DateTime.Now.Ticks;

                    Thread.MemoryBarrier();

                    tag->UpdateSequence = GetCompleteSequence(writeSeq);
                }
                finally
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }

                return true;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private void UpdateHeaderTagCount(int count)
        {
            byte* ptr = null;
            _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);

            try
            {
                SharedTagHeader* header = (SharedTagHeader*)ptr;
                header->TagCount = count;
                header->LastUpdateTicks = DateTime.Now.Ticks;

                // CRC 재계산
                header->CRC32 = CalculateHeaderCRC(header);
            }
            finally
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

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

        /// <summary>
        /// Header CRC32 계산
        /// </summary>
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

            _lock?.Dispose();
            _accessor?.Dispose();
            _mmf?.Dispose();
            _disposed = true;
        }
    }
   #pragma warning restore CS8500 //관리되는 유형('SharedTagHeader')의 주소를 사용하거나 크기를 가져오거나 포인터를 선언합니다.
}
