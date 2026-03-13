using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace OPCUA.Client.Core
{
    /// <summary>
    /// Core Item runtime (no UI binding)
    /// - Identity: Name, NodeId string
    /// - Cache: last DataValue + status + timestamp
    /// </summary>
    public sealed class OPCUAMember_item
    {
        // ===== Identity (Config) =====
        public string sName { get; private set; }
        public string sNode { get; private set; }   // "ns=2;s=..."

        // Back-reference (Core internal)
        public OPCUAMember_group Group { get; private set; }
        public OPCUAMember_server Server => Group?.Server;

        // UA subscription object (Core internal)
        internal MonitoredItem moItem;

        // ===== Runtime cache =====
        private DataValue _lastValue; // reference swap only
        private StatusCode _lastStatusCode = StatusCodes.Bad;
        private DateTime _lastTimestamp = DateTime.MinValue;
        private int _hasEverRead; // 0/1

        // Optional flags derived from value type (Core can keep, UI can ignore)
        public bool bIsArray { get; private set; }
        public bool bIsMatrix { get; private set; }

        // indexes (optional; if you still need them)
        public int nGroupIndex = -1;
        public int nItemIndex = -1;

        public OPCUAMember_item(string name, string nodeId)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(nodeId)) throw new ArgumentNullException(nameof(nodeId));

            sName = name;
            sNode = nodeId;
        }

        internal void SetGroup(OPCUAMember_group group) => Group = group;
        internal void ClearGroup() => Group = null;

        // ===== Cache getters (lock-free) =====
        public DataValue dvDataValue => _lastValue;
        public StatusCode LastStatusCode => _lastStatusCode;
        public DateTime LastTimestamp => _lastTimestamp;

        public DateTime? LastTimestampLocal
        {
            get
            {
                if (_lastTimestamp == DateTime.MinValue)
                    return null;

                var utc = _lastTimestamp.Kind == DateTimeKind.Utc
                    ? _lastTimestamp
                    : DateTime.SpecifyKind(_lastTimestamp, DateTimeKind.Utc);

                return utc.ToLocalTime();
            }
        }

        public bool HasEverRead => Volatile.Read(ref _hasEverRead) != 0;

        /// <summary>
        /// OPC UA 값이 갱신되었을 때 발생
        /// </summary>
        public event EventHandler Updated;

        // ===== UI-friendly display properties =====

        public string sValue
        {
            get
            {
                var dv = _lastValue;
                if (dv?.Value == null)
                    return string.Empty;

                return OpcUaValueFormatter.FormatSafe(
                    dv.Value,
                    dv.StatusCode,
                    dv.WrappedValue.TypeInfo
                );
            }
        }


        public string DataType
        {
            get
            {
                var dv = _lastValue;
                return dv?.Value?.GetType().Name ?? "Unknown type";
            }
        }

        public int DataTypeCode
        {
            get
            {
                var dv = _lastValue;               // DataValue
                if (dv == null)
                    return (int)BuiltInType.Null;

                // WrappedValue.TypeInfo가 있으면 그게 제일 정확함
                var ti = dv.WrappedValue.TypeInfo;
                if (ti != null)
                    return (int)ti.BuiltInType;

                // fallback: Value의 CLR Type으로 유추
                var v = dv.Value;
                if (v == null)
                    return (int)BuiltInType.Null;

                // return (int)TypeInfo.GetBuiltInType(v.GetType());
                // fallback
                return (int)BuiltInType.Null;
            }
        }

        public string QualityString
        {
            get
            {
                if (StatusCode.IsBad(_lastStatusCode))
                    return "Bad";
                if (StatusCode.IsUncertain(_lastStatusCode))
                    return "Uncertain";
                return HasEverRead ? "Good" : "";
            }
        }

        public string FullItemId
        {
            get
            {
                var srv = Server;
                var grp = Group;
                if (srv == null || grp == null)
                    return sName;
                return $"{srv.accessName}.{grp.sName}.{sName}";
            }
        }



        public OpcQuality Quality
        {
            get
            {
                if (StatusCode.IsBad(LastStatusCode))
                    return OpcQuality.Bad;
                if (StatusCode.IsUncertain(LastStatusCode))
                    return OpcQuality.Uncertain;
                return OpcQuality.Good;
            }
        }

        /// <summary>
        /// Called by subscription notification thread.
        /// Must be fast; no UI code.
        /// </summary>
        public void UpdateFromNotification(DataValue dv)
        {
            if (dv == null) return;

            // derive flags cheaply
            var v = dv.Value;
            bIsMatrix = v is Matrix;
            bIsArray = (!bIsMatrix && v is Array);

            _lastValue = dv;                     // atomic reference swap
            _lastStatusCode = dv.StatusCode;
            _lastTimestamp = dv.SourceTimestamp;

            Volatile.Write(ref _hasEverRead, 1);

            Updated?.Invoke(this, EventArgs.Empty); 
        }

        public bool Update(string newNodeId, string newName)
        {
            bool nodeChanged = sNode != newNodeId;

            sNode = newNodeId;
            sName = newName;

            return nodeChanged;
        }
    }
    public enum OpcQuality
    {
        Good,
        Uncertain,
        Bad
    }
}
