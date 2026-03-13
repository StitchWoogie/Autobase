using AutoLibLocal;
using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studio.OPCUA
{
    /// <summary>
    /// LocalMain OPC UA Client Cache
    /// - IPC(TagValueChangedEvent) 기반 Push 캐시
    /// - GetOpcData / GetOpcUaData에서 O(1) 조회
    /// - 대용량 태그 대응
    /// </summary>
    internal static class OpcUaClientCache
    {
        private static readonly Dictionary<string, OpcUaTagCache> _map =
            new Dictionary<string, OpcUaTagCache>(StringComparer.OrdinalIgnoreCase);

        private static readonly object _sync = new object();

        // =========================================================
        // IPC → Cache (Hot Path)
        // =========================================================
        public static void Update(TagValueChangedEvent e)
        {
            if (e == null || string.IsNullOrEmpty(e.FullName))
                return;

            OpcUaTagCache c;

            // ① 엔트리 확보 (짧은 lock)
            lock (_sync)
            {
                if (!_map.TryGetValue(e.FullName, out c))
                {
                    c = new OpcUaTagCache();
                    _map[e.FullName] = c;
                }
            }

            // 이전 상태 보관
            int prevQuality = c.Quality;

            // ② 값 갱신 (lock-free)
            c.Value = e.Value;
            c.Quality = e.Quality;
            c.Timestamp = e.Timestamp;

            // 문자열/품질 파생값은 1회 계산
            c.ValueString = e.Value != null ? e.Value.ToString() : "";
            ParseQuality(e.Quality, c);

            // 상태 변화 판정
            c.QualityChanged = (c.HasValue != 0 && prevQuality != e.Quality);

            c.HasValue = 1;
        }

        // =========================================================
        // Quality 파싱 (UA → 기존 Device 모델)
        // =========================================================
        private static void ParseQuality(int quality, OpcUaTagCache c)
        {
            int q1 = (quality >> 6) & 0x03; // DeviceQuality
            int q2 = (quality >> 2) & 0x0F; // SubStatus
            int q3 = (quality >> 0) & 0x03; // Limit

            c.DeviceQuality = (EnumDeviceQuality)q1;
            c.DeviceSubStatus = (byte)q2;
            c.DeviceLimit = (EnumDeviceLimit)q3;
        }

        // =========================================================
        // LocalMain → Cache 조회 (GetOpcData에서 사용)
        // =========================================================
        public static bool TryGet(string fullName, out OpcUaTagCache cache)
        {
            cache = null;

            if (string.IsNullOrEmpty(fullName))
                return false;

            lock (_sync)
            {
                if (!_map.TryGetValue(fullName, out cache))
                    return false;
            }

            // 값이 아직 한번도 안 들어온 경우
            return cache.HasValue != 0;
        }

        // =========================================================
        // 재연결 / 초기화
        // =========================================================
        public static void Clear()
        {
            lock (_sync)
            {
                _map.Clear();
            }
        }


        public static bool Exists(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return false;

            lock (_sync)
            {
                return _map.ContainsKey(fullName);
            }
        }
    }
}
