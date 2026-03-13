using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    [Serializable]
    public sealed class ServerStateChangedEvent
    {
        public string AccessName;
        public RuntimeState NewState;
    }

    [Serializable]
    public sealed class TagValueChangedEvent
    {
        public string FullName;
        public string AccessName;
        public string GroupName;
        public string ItemName;

        public string Value;
        public int Quality;
        public DateTime Timestamp;
    }

    [Serializable]
    public sealed class RuntimeErrorEvent
    {
        public string Source;
        public string Message;
        public int ErrorCode;
    }
}
