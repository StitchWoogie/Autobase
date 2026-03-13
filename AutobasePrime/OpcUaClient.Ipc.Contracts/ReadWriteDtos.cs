using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    [Serializable]
    public sealed class ReadItemRequest
    {
        public string AccessName;   // tp.sOpcServer
        public string GroupName;    // tp.sOpcGroup
        public string ItemName;     // tp.sOpcItem
    }

    [Serializable]
    public sealed class ReadItemResponse
    {
        public IpcResultCode ResultCode;
        public string Value;
        public int Quality;
        public DateTime Timestamp;
    }

    [Serializable]
    public sealed class WriteItemRequest
    {
        public string AccessName;
        public string GroupName;    // tp.sOpcGroup
        public string ItemName;

        public object Value;
    }

    [Serializable]
    public sealed class UpdateItemRequest
    {
        public string AccessName;
        public string GroupName;
        public string ItemName;
    }

    [Serializable]
    public sealed class UpdateItemResponse
    {
        public string Value;
        public int Quality;
        public DateTime Timestamp;
    }


    public sealed class GetServersResponse
    {
        public List<ServerInfo> Servers { get; set; } = new List<ServerInfo>();
    }

    public sealed class ServerInfo
    {
        public string AccessName { get; set; }
        public string ServerName { get; set; }
    }

    public sealed class GetGroupsRequest
    {
        public string AccessName { get; set; }
    }

    public sealed class GetGroupsResponse
    {
        public List<GroupInfo> Groups { get; set; } = new List<GroupInfo>();
    }

    public sealed class GroupInfo
    {
        public string GroupName { get; set; }
        public int PublishingInterval { get; set; }
    }

    public sealed class GetItemsRequest
    {
        public string AccessName { get; set; }
        public string GroupName { get; set; }
    }

    public sealed class GetItemsResponse
    {
        public List<ItemInfo> Items { get; set; } = new List<ItemInfo>();
    }

    public sealed class ItemInfo
    {
        public string ItemName { get; set; }
        public string FullName { get; set; }
        public string NodeId { get; set; }
        public int DataType { get; set; }

        // 현재 값 정보 (선택적)
        public string Value { get; set; }
        public int Quality { get; set; }
        public DateTime? Timestamp { get; set; }
    }

}
