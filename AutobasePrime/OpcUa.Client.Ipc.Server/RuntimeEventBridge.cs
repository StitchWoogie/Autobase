using OPCUA.Client.Core;
using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Ipc.Server
{
    internal sealed class RuntimeEventBridge
    {
        private readonly ClientRegistry _clients;

        public RuntimeEventBridge(OpcUaRuntime runtime,
                                  ClientRegistry clients)
        {
            _clients = clients;

            // 기존 서버들 attach
            var servers = runtime.GetServersSnapshot();
            for (int i = 0; i < servers.Count; i++)
            {
                AttachServer(servers[i]);
            }

            // 이후 AddServer 될 경우 동적으로 이벤트 등록
            runtime.ServerAdded += AttachServer;
        }

        private void AttachServer(OPCUAMember_server server)
        {
            if (server == null)
                return;

            server.ServerStateChanged += OnServerStateChanged;
            server.RuntimeLog += OnRuntimeLog;

            var groups = server.arrGroup;
            if (groups == null)
                return;

            for (int gi = 0; gi < groups.Count; gi++)
            {
                var g = groups[gi];
                if (g == null) continue;

                var items = g.arrItem;
                if (items == null) continue;

                for (int ii = 0; ii < items.Count; ii++)
                {
                    var it = items[ii];
                    if (it == null) continue;

                    it.Updated += OnItemUpdated;
                }
            }
        }

        // =========================
        // 실제 이벤트 핸들러
        // =========================

        private void OnServerStateChanged(object sender, EventArgs e)
        {
            var server = sender as OPCUAMember_server;
            if (server == null)
                return;

            Broadcast(new IpcMessage
            {
                Type = IpcMessageType.Event,
                Command = IpcCommand.ServerStateChanged,
                Payload = new ServerStateChangedEvent
                {
                    AccessName = server.accessName,
                    NewState = server.bServerAlive
                        ? RuntimeState.Connected
                        : RuntimeState.Disconnected
                }
            });
        }

        private void OnRuntimeLog(object sender, string message)
        {
            var server = sender as OPCUAMember_server;
            if (server == null)
                return;

            Broadcast(new IpcMessage
            {
                Type = IpcMessageType.Event,
                Command = IpcCommand.RuntimeLog,
                Payload = new RuntimeLogEvent
                {
                    Level = RuntimeLogLevel.Info,
                    Source = "Server",
                    Context = server.accessName,
                    Message = message
                }
            });
        }

        private void OnItemUpdated(object sender, EventArgs e)
        {
            var it = sender as OPCUAMember_item;
            if (it == null)
                return;

            var dv = it.dvDataValue;

            Broadcast(new IpcMessage
            {
                Type = IpcMessageType.Event,
                Command = IpcCommand.TagValueChanged,
                Payload = new TagValueChangedEvent
                {
                    AccessName = it.Server?.accessName,
                    GroupName = it.Group?.sName,
                    ItemName = it.sName,

                    //  AccessName.GroupName.ItemName 형식으로 조립
                    FullName = $"{it.Server?.accessName}.{it.Group?.sName}.{it.sName}",

                    //  문자열 값 사용
                    Value = it.sValue,

                    //  OPC UA StatusCode 그대로
                    Quality = dv != null
                        ? (int)dv.StatusCode.Code
                        : 0,

                    Timestamp = dv?.SourceTimestamp ?? DateTime.UtcNow
                }
            });
        }

        private void Broadcast(IpcMessage evt)
        {
            foreach (var c in _clients.Snapshot())
            {
                try
                {
                    c.SendEvent(evt);
                }
                catch
                {
                }
            }
        }
    }
}
