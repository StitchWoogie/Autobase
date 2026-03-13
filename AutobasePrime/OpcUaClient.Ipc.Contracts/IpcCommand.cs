using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    public enum IpcCommand
    {
        Ping = 0,
        GetRuntimeTree,
        ReadItem,
        WriteItem,
        UpdateItem,

        // Browse (NEW)
        GetServers,
        GetGroups,
        GetItems,

        // Lifecycle
        Shutdown,

        // Events
        TagValueChanged,
        ServerStateChanged,
        RuntimeLog,
        RuntimeError
    }
}
