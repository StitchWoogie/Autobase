using OPCUA.Client.Core;
using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Ipc.Server
{
    internal static class RuntimeSnapshot
    {
        public static GetRuntimeTreeResponse Build(OpcUaRuntime runtime)
        {
            var res = new GetRuntimeTreeResponse();

            foreach (var s in runtime.Servers)
            {
                if (s == null)
                    continue;

                var sn = new RuntimeServerNode
                {
                    AccessName = s.accessName,
                    ServerName = s.serverName,

                    // EndpointUrl은 Core에 없으므로 null
                    EndpointUrl = null,

                    // bool → RuntimeState 변환
                    State = s.bServerAlive
                        ? RuntimeState.Connected
                        : RuntimeState.Disconnected
                };

                var groups = s.arrGroup;
                if (groups == null)
                {
                    res.Servers.Add(sn);
                    continue;
                }

                for (int gi = 0; gi < groups.Count; gi++)
                {
                    var g = groups[gi];
                    if (g == null)
                        continue;

                    var gn = new RuntimeGroupNode
                    {
                        GroupName = g.sName,
                        PublishingInterval = g.nInterval
                    };

                    var items = g.arrItem;
                    if (items == null)
                    {
                        sn.Groups.Add(gn);
                        continue;
                    }

                    for (int ii = 0; ii < items.Count; ii++)
                    {
                        var i = items[ii];
                        if (i == null)
                            continue;

                        var dv = i.dvDataValue;

                        var itemNode = new RuntimeItemNode
                        {
                            ItemName = i.sName,
                            NodeId = i.sNode,

                            // DataType은 cache 기반으로 계산
                            DataType = i.DataType,

                            FullName =
                                s.accessName + "." +
                                g.sName + "." +
                                i.sName
                        };

                        // 현재 값 포함 (있는 경우)
                        if (dv != null)
                        {
                            itemNode.Value = i.sValue;
                            itemNode.Quality = (int)dv.StatusCode.Code;
                            itemNode.Timestamp = dv.SourceTimestamp;
                        }

                        gn.Items.Add(itemNode);
                    }

                    sn.Groups.Add(gn);
                }

                res.Servers.Add(sn);
            }

            return res;
        }
    }
}
