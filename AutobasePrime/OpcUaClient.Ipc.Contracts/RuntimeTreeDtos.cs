using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    /// <summary>
    /// Runtime 트리 스냅샷 요청
    /// </summary>
    [Serializable]
    public sealed class GetRuntimeTreeRequest
    {
        // 현재는 옵션 없음. 추후 확장을 위해 DTO는 유지.
        public static readonly GetRuntimeTreeRequest Empty = new GetRuntimeTreeRequest();
    }

    /// <summary>
    /// Runtime 트리 스냅샷 응답
    /// </summary>
    [Serializable]
    public sealed class GetRuntimeTreeResponse
    {
        public DateTime SnapshotTime;
        public List<RuntimeServerNode> Servers;

        public GetRuntimeTreeResponse()
        {
            Servers = new List<RuntimeServerNode>();
            SnapshotTime = DateTime.UtcNow;
        }
    }

    [Serializable]
    public sealed class RuntimeServerNode
    {
        /// <summary>UI/Studio에서 서버 식별에 쓰는 키 (유니크)</summary>
        public string AccessName;

        /// <summary>표시용 이름(옵션)</summary>
        public string ServerName;

        /// <summary>Endpoint URL(옵션, 디버그/표시용)</summary>
        public string EndpointUrl;

        public RuntimeState State;

        public List<RuntimeGroupNode> Groups;

        public RuntimeServerNode()
        {
            Groups = new List<RuntimeGroupNode>();
        }
    }

    [Serializable]
    public sealed class RuntimeGroupNode
    {
        public string GroupName;
        public int PublishingInterval;

        public List<RuntimeItemNode> Items;

        public RuntimeGroupNode()
        {
            Items = new List<RuntimeItemNode>();
        }
    }

    [Serializable]
    public sealed class RuntimeItemNode
    {
        /// <summary>그룹 내 표시/식별 이름</summary>
        public string ItemName;

        /// <summary>OPC UA NodeId의 문자열 표현</summary>
        public string NodeId;

        /// <summary>표시용 데이터 타입(예: "Int32", "Double", "Boolean")</summary>
        public string DataType;

        /// <summary>
        /// UI/Studio 편의를 위한 FullName (권장)
        /// 예: "accessA.group1.itemX"
        /// </summary>
        public string FullName;

        /// <summary>현재 값 문자열 (선택적)</summary>
        public string Value;

        /// <summary>품질 코드</summary>
        public int Quality;

        /// <summary>타임스탬프</summary>
        public DateTime? Timestamp;
    }
}
