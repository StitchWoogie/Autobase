using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    public enum RuntimeLogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    [Serializable]
    public sealed class RuntimeLogEvent
    {
        public DateTime Time;
        public RuntimeLogLevel Level;

        /// <summary>예: "Server", "Group", "Item", "IPC", "Core"</summary>
        public string Source;

        /// <summary>예: AccessName/GroupName/ItemName 등 컨텍스트</summary>
        public string Context;

        public string Message;

        public RuntimeLogEvent()
        {
            Time = DateTime.UtcNow;
        }
    }
}
