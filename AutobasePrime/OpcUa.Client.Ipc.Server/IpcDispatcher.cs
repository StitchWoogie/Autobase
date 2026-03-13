using OPCUA.Client.Core;
using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcUa.Client.Ipc.Server
{
    internal sealed class IpcDispatcher
    {
        private readonly OpcUaRuntime _runtime;

        /// <summary>
        /// IPC Shutdown 명령 수신 시 호출되는 콜백.
        /// IpcServer에서 설정한다.
        /// </summary>
        public Action OnShutdownRequested;

        public IpcDispatcher(OpcUaRuntime runtime)
        {
            _runtime = runtime;
        }

        public async Task<IpcMessage> DispatchAsync(IpcMessage req, PipeClientContext client)
        {
            if (req.Type != IpcMessageType.Request)
                return null;

            try
            {
                switch (req.Command)
                {
                    case IpcCommand.Ping:
                        return Ok(req, "OK");

                    case IpcCommand.Shutdown:
                        OnShutdownRequested?.Invoke();
                        return Ok(req, "OK");

                    case IpcCommand.GetRuntimeTree:
                        return Ok(req, RuntimeSnapshot.Build(_runtime));

                    case IpcCommand.ReadItem:
                        return HandleReadItem(req);

                    case IpcCommand.WriteItem:
                        return await HandleWriteAsync(req).ConfigureAwait(false);

                    case IpcCommand.UpdateItem:
                        return HandleUpdateItem(req);

                    case IpcCommand.GetServers:
                        {
                            var res = HandleGetServers(_runtime);
                            return Ok(req, res);
                        }

                    case IpcCommand.GetGroups:
                        {
                            var payload = (GetGroupsRequest)req.Payload;
                            var res = HandleGetGroups(_runtime, payload);
                            return Ok(req, res);
                        }

                    case IpcCommand.GetItems:
                        {
                            var payload = (GetItemsRequest)req.Payload;
                            var res = HandleGetItems(_runtime, payload);
                            return Ok(req, res);
                        }
                }

                return Error(req, IpcResultCode.NotFound, "Unknown command");
            }
            catch (Exception ex)
            {
                return Error(req, IpcResultCode.InternalError, ex.Message);
            }
        }

        // =========================
        // Read
        // =========================

        private IpcMessage HandleReadItem(IpcMessage req)
        {
            var r = (ReadItemRequest)req.Payload;

            if (string.IsNullOrEmpty(r.AccessName) ||
                string.IsNullOrEmpty(r.GroupName) ||
                string.IsNullOrEmpty(r.ItemName))
            {
                return Error(req, IpcResultCode.InvalidArgument, "Invalid ReadItemRequest");
            }

            var server = _runtime.GetServer(r.AccessName);
            if (server == null)
                return Error(req, IpcResultCode.NotFound, "Server not found");

            var group = server.FindGroup(r.GroupName);
            if (group == null)
                return Error(req, IpcResultCode.NotFound, "Group not found");

            var item = group.FindItem(r.ItemName);
            if (item == null)
                return Error(req, IpcResultCode.NotFound, "Item not found");

            var dv = item.dvDataValue;
            if (dv == null)
                return Error(req, IpcResultCode.NotReady, "Value not available");

            return Ok(req, new ReadItemResponse
            {
                Value = item.sValue,
                Quality = (int)dv.StatusCode.Code,
                Timestamp = dv.SourceTimestamp
            });
        }

        // =========================
        // Write
        // =========================

        private async Task<IpcMessage> HandleWriteAsync(IpcMessage req)
        {
            var w = (WriteItemRequest)req.Payload;

            var ok = await _runtime
                .WriteItemByFullNameAsync(
                    w.ItemName,
                    w.Value,
                    CancellationToken.None)
                .ConfigureAwait(false);

            if (!ok)
            {
                return Error(req, IpcResultCode.InvalidState, "Write failed");
            }

            return Ok(req, null);
        }

        // =========================
        // Update (이벤트 재발생)
        // =========================

        private IpcMessage HandleUpdateItem(IpcMessage req)
        {
            var r = (UpdateItemRequest)req.Payload;

            if (string.IsNullOrEmpty(r.AccessName) ||
                string.IsNullOrEmpty(r.GroupName) ||
                string.IsNullOrEmpty(r.ItemName))
            {
                return Error(req, IpcResultCode.InvalidArgument, "Invalid UpdateItemRequest");
            }

            var server = _runtime.GetServer(r.AccessName);
            if (server == null)
                return Error(req, IpcResultCode.NotFound, "Server not found");

            var group = server.FindGroup(r.GroupName);
            if (group == null)
                return Error(req, IpcResultCode.NotFound, "Group not found");

            var item = group.FindItem(r.ItemName);
            if (item == null)
                return Error(req, IpcResultCode.NotFound, "Item not found");

            var dv = item.dvDataValue;
            if (dv == null)
                return Error(req, IpcResultCode.NotReady, "Value not available");

            // Updated 이벤트 재발생 (구독으로 이미 받은 값)
            try
            {
                item.UpdateFromNotification(dv);
            }
            catch (Exception ex)
            {
                return Error(req, IpcResultCode.InternalError, $"Update notification failed: {ex.Message}");
            }

            // 현재 값 반환
            return Ok(req, new UpdateItemResponse
            {
                Value = item.sValue,
                Quality = (int)dv.StatusCode.Code,
                Timestamp = dv.SourceTimestamp
            });
        }

        // =========================
        // Helpers
        // =========================

        private static IpcMessage Ok(IpcMessage req, object payload)
        {
            return new IpcMessage
            {
                Type = IpcMessageType.Response,
                Command = req.Command,
                RequestId = req.RequestId,
                ResultCode = IpcResultCode.Ok,
                Payload = payload
            };
        }

        private static IpcMessage Error(
            IpcMessage req, IpcResultCode code, string msg)
        {
            return new IpcMessage
            {
                Type = IpcMessageType.Response,
                Command = req.Command,
                RequestId = req.RequestId,
                ResultCode = code,
                Error = msg
            };
        }



        private GetServersResponse HandleGetServers(OpcUaRuntime runtime)
        {
            var res = new GetServersResponse();

            foreach (var s in runtime.Servers)
            {
                if (s == null) continue;

                res.Servers.Add(new ServerInfo
                {
                    AccessName = s.accessName,
                    ServerName = s.serverName
                });
            }

            return res;
        }

        private GetGroupsResponse HandleGetGroups(
    OpcUaRuntime runtime,
    GetGroupsRequest req)
        {
            var res = new GetGroupsResponse();

            var s = runtime.Servers
                .FirstOrDefault(x => x.accessName == req.AccessName);

            if (s == null)
                return res;

            foreach (var g in s.arrGroup)
            {
                if (g == null) continue;

                res.Groups.Add(new GroupInfo
                {
                    GroupName = g.sName,
                    PublishingInterval = g.nInterval
                });
            }

            return res;
        }

        private GetItemsResponse HandleGetItems(
    OpcUaRuntime runtime,
    GetItemsRequest req)
        {
            var res = new GetItemsResponse();

            var s = runtime.Servers
                .FirstOrDefault(x => x.accessName == req.AccessName);
            if (s == null) return res;

            var g = s.arrGroup
                .FirstOrDefault(x => x.sName == req.GroupName);
            if (g == null) return res;

            foreach (var i in g.arrItem)
            {
                if (i == null) continue;

                var dv = i.dvDataValue;

                var itemInfo = new ItemInfo
                {
                    ItemName = i.sName,
                    NodeId = i.sNode,
                    DataType = i.DataTypeCode,
                    FullName =
                        s.accessName + "." +
                        g.sName + "." +
                        i.sName
                };

                // 현재 값 포함 (있는 경우)
                if (dv != null)
                {
                    itemInfo.Value = i.sValue;
                    itemInfo.Quality = (int)dv.StatusCode.Code;
                    itemInfo.Timestamp = dv.SourceTimestamp;
                }

                res.Items.Add(itemInfo);
            }

            return res;
        }

    }
}
