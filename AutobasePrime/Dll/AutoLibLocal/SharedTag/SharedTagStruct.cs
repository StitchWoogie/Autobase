using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoLibLocal
{
    /// <summary>
    /// SCADA 태그 품질 상태 (OPC UA 표준 기반)
    /// </summary>
    public enum TagQuality : byte
    {
        Bad = 0,           // 잘못된 값
        Good = 1,          // 정상 값
        CommFail = 2,      // 통신 실패
        Timeout = 3,       // 타임아웃
        Manual = 4,        // 수동 입력
        Uncertain = 5,     // 불확실
        ConfigError = 6,   // 설정 오류
        NotConnected = 7,  // 연결 안됨
        DeviceError = 8    // 디바이스 에러
    }

    /// <summary>
    /// Memory-Mapped File Header
    /// 버전 관리 및 구조체 크기 검증
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SharedTagHeader
    {
        public int Version;              // 프로토콜 버전
        public int TagDataStructSize;    // SharedTagData 크기 검증용
        public int TagDirEntrySize;      // TagDirectoryEntry 크기 검증용
        public int TagCount;             // 현재 등록된 태그 수
        public int MaxTags;              // 최대 태그 수
        public long LastUpdateTicks;     // 마지막 업데이트 시간
        public uint CRC32;               // Header 체크섬
        public int Reserved1;

        public fixed byte Reserved[32];  // 향후 확장용

        // Total: 72 bytes
    }

    /// <summary>
    /// TagName → Index 매핑 디렉토리
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TagDirectoryEntry
    {
        public int Index;                // 태그 인덱스 (0-based)
        public fixed byte TagName[128];  // UTF-8 TagName (null-terminated)
        public long RegisteredTicks;     // 등록 시간
        public int Reserved;             // 정렬용

        // Total: 140 bytes
    }

    /// <summary>
    /// 실제 태그 데이터 (Atomic Update 지원)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct SharedTagData
    {
        // Atomic Update 제어
        public volatile byte UpdateSequence;  // 홀수=쓰기중, 짝수=완료
        public TagQuality Quality;
        public byte IsInitialized;
        public byte Reserved1;

        // 실제 데이터
        public double NumericValue;
        public long LastUpdateTicks;

        public fixed byte StringValue[256];  // UTF-8 문자열

        // 추가 메타데이터
        public int SourceDeviceId;      // 데이터 소스 식별
        public short AlarmLevel;        // 알람 레벨 (0=없음, 1=경고, 2=위험)
        public short Reserved2;

        // Total: 284 bytes
    }

    /// <summary>
    /// 태그 읽기 결과
    /// </summary>
    public struct TagReadResult
    {
        public double NumericValue;
        public string StringValue;
        public TagQuality Quality;
        public bool IsValid;
        public DateTime LastUpdate;
        public short AlarmLevel;
        public int SourceDeviceId;
    }
}
