using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain.OPCUA
{
    internal sealed class OpcUaTagCache
    {
        // ===== 원본 데이터 =====
        public object Value;        // IPC에서 받은 원본 값
        public int Quality;         // OPC UA StatusCode.Code
        public DateTime Timestamp;

        // ===== 파생 데이터 (LocalMain 최적화) =====
        public string ValueString;  // ToString() 캐시
        public EnumDeviceQuality DeviceQuality;
        public EnumDeviceLimit DeviceLimit;
        public byte DeviceSubStatus;

        // ===== 상태 =====
        public volatile int HasValue; // 0/1

        public int PrevQuality;
        public bool QualityChanged;   // 이번 Update에서 바뀌었는지

        public bool TryGetValueString(int index, out string value)
        {
            value = "";

            if (Value == null)
                return false;

            try
            {
                // Array 처리
                if (Value is Array arr)
                {
                    if (index < 0 || index >= arr.Length)
                        return false;

                    object v = arr.GetValue(index);
                    value = v != null ? v.ToString() : "";
                    return true;
                }

                // Scalar
                value = Value.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
