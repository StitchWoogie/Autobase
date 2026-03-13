using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    /// <summary>
    /// IPC 메시지 유형
    /// </summary>
    public enum IpcMessageType
    {
        /// <summary>Client → Server 요청</summary>
        Request = 0,

        /// <summary>Server → Client 응답</summary>
        Response = 1,

        /// <summary>Server → Client 이벤트(Push)</summary>
        Event = 2
    }
}
