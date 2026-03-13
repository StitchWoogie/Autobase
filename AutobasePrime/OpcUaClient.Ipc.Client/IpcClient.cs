using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Client
{
    public sealed class IpcClient : IDisposable
    {
        private readonly PipeConnection _conn;
        private readonly RequestManager _reqMgr;
        private readonly EventDispatcher _evt;

        public event EventHandler<TagValueChangedEvent> TagValueChanged;
        public event EventHandler<ServerStateChangedEvent> ServerStateChanged;
        public event EventHandler<RuntimeLogEvent> RuntimeLogReceived;
        public event EventHandler Disconnected; // 상위로 전달용

        private volatile bool _connected;
        private volatile bool _disposed;

        // ── Auto-Reconnect ──
        private Timer _reconnectTimer;
        private int _reconnecting; // 0=idle, 1=busy
        private const int ReconnectIntervalMs = 5000;

        public IpcClient(string pipeName)
        {
            _conn = new PipeConnection(pipeName);
            _reqMgr = new RequestManager();
            _evt = new EventDispatcher();

            _conn.MessageReceived += OnMessage;
            _conn.Disconnected += (s, e) =>
            {
                _connected = false;

                // 대기중 요청 전부 실패 처리 (UI hang 방지)
                try { _reqMgr.CancelAll("Disconnected"); } catch { }

                try { Disconnected?.Invoke(this, EventArgs.Empty); } catch { }

                // 자동 재연결 시작
                StartReconnectTimer();
            };

            _evt.TagValueChanged += (s, e) =>
            {
                try { TagValueChanged?.Invoke(this, e); }
                catch { /* UI 예외 전파 금지 */ }
            };

            _evt.ServerStateChanged += (s, e) =>
            {
                try { ServerStateChanged?.Invoke(this, e); }
                catch { }
            };

            _evt.RuntimeLog += (s, e) =>
            {
                try { RuntimeLogReceived?.Invoke(this, e); }
                catch { }
            };
        }

        // =====================
        // Connection
        // =====================

        public bool IsConnected => _connected && _conn.IsConnected;

        /// <summary>
        /// UI에서 호출해도 되도록 비동기 래핑.
        /// (PipeConnection.Connect가 동기라면 Task.Run으로 감싼다)
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (_disposed) return false;
            if (IsConnected) return true;

            try
            {
                bool ok = await Task.Run(() => _conn.Connect()).ConfigureAwait(false);
                _connected = ok;
                return ok;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[IPC Client] Connect failed: " + ex.Message);
                _connected = false;
                return false;
            }
        }


        public void Dispose()
        {
            _disposed = true;

            StopReconnectTimer();
            try { _conn.Dispose(); } catch { }
            _connected = false;

            try { _reqMgr.CancelAll("Disposed"); } catch { }
        }

        // =====================
        // Auto-Reconnect
        // =====================

        private void StartReconnectTimer()
        {
            if (_disposed) return;

            if (_reconnectTimer == null)
            {
                _reconnectTimer = new Timer(
                    ReconnectCallback, null,
                    ReconnectIntervalMs, ReconnectIntervalMs);
            }
        }

        private void StopReconnectTimer()
        {
            try { _reconnectTimer?.Dispose(); } catch { }
            _reconnectTimer = null;
        }

        private async void ReconnectCallback(object state)
        {
            if (_disposed || _connected) return;

            // 중복 방지
            if (Interlocked.CompareExchange(ref _reconnecting, 1, 0) != 0)
                return;

            try
            {
                bool ok = await Task.Run(() => _conn.Connect()).ConfigureAwait(false);
                if (ok)
                {
                    _connected = true;
                    StopReconnectTimer();
                    Debug.WriteLine("[IPC Client] Reconnected.");
                }
            }
            catch
            {
                // 재연결 실패 → 다음 타이머에서 재시도
            }
            finally
            {
                Interlocked.Exchange(ref _reconnecting, 0);
            }
        }


        // =====================
        // Public API (SAFE)
        // =====================

        public Task<GetRuntimeTreeResponse> GetRuntimeTreeAsync()
             => CallAsync<GetRuntimeTreeResponse>(IpcCommand.GetRuntimeTree, null);

        /// <summary>
        /// 단일 아이템 읽기.
        /// GetItemsAsync 시 그룹에 대한 모든 아이템 현재값 등 다 포함하고 있음.
        /// </summary>
        /// <param name="accessName"></param>
        /// <param name="groupName"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Task<ReadItemResponse> ReadItemAsync(string accessName, string groupName, string itemName)
        {
            var req = new ReadItemRequest
            {
                AccessName = accessName,
                GroupName = groupName,
                ItemName = itemName
            };

            return CallAsync<ReadItemResponse>(IpcCommand.ReadItem, req);
        }

        public Task<object> WriteItemAsync(string accessName, string groupName, string itemName, object value)
        {
            var req = new WriteItemRequest
            {
                AccessName = accessName,
                GroupName = groupName,
                ItemName = itemName,
                Value = value
            };

            return CallAsync<object>(IpcCommand.WriteItem, req);
        }

        public Task<UpdateItemResponse> UpdateItemAsync(string accessName, string groupName, string itemName)
        {
            var req = new UpdateItemRequest
            {
                AccessName = accessName,
                GroupName = groupName,
                ItemName = itemName
            };

            return CallAsync<UpdateItemResponse>(IpcCommand.UpdateItem, req);
        }

        public async Task<GetServersResponse> GetServersAsync()
            => await CallAsync<GetServersResponse>(IpcCommand.GetServers, null);

        public async Task<GetGroupsResponse> GetGroupsAsync(string accessName)
        => await CallAsync<GetGroupsResponse>(
            IpcCommand.GetGroups,
            new GetGroupsRequest { AccessName = accessName });

        public async Task<GetItemsResponse> GetItemsAsync(string accessName, string groupName)
          => await CallAsync<GetItemsResponse>(
              IpcCommand.GetItems,
              new GetItemsRequest
              {
                  AccessName = accessName,
                  GroupName = groupName
              });

        /// <summary>
        /// OPCUA Client에 Shutdown 명령을 전송한다.
        /// LocalMain 종료 시 트레이 상태의 OPCUA Client도 정상 종료시킨다.
        /// </summary>
        public Task<string> SendShutdownAsync()
            => CallAsync<string>(IpcCommand.Shutdown, null);

        // =====================
        // Internal SAFE CALL
        // =====================

        private async Task<T> CallAsync<T>(IpcCommand cmd, object payload)
        {
            if (!IsConnected)
                return default(T);

            try
            {
                var msg = new IpcMessage
                {
                    Type = IpcMessageType.Request,
                    Command = cmd,
                    RequestId = Guid.NewGuid(),
                    Payload = payload
                };

                return await _reqMgr.SendAsync<T>(_conn, msg).ConfigureAwait(false);
            }
            catch (System.IO.IOException)
            {
                // 파이프 끊김 → 연결 상태 해제
                _connected = false;
                return default(T);
            }
            catch (TimeoutException)
            {
                // 개별 요청 타임아웃 → 연결은 유지
                return default(T);
            }
            catch (Exception)
            {
                // 기타 (직렬화 등) → 연결은 유지
                return default(T);
            }
        }

        private void OnMessage(object sender, IpcMessage msg)
        {
            try
            {
                if (msg.Type == IpcMessageType.Response)
                {
                    _reqMgr.OnResponse(msg);
                }
                else if (msg.Type == IpcMessageType.Event)
                {
                    _evt.Dispatch(msg);
                }
            }
            catch
            {
                // IPC 내부 오류 → 외부 전파 금지
            }
        }



    }
}
