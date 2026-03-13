using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    /// <summary>
    /// Core Runtime Engine (No UI / No Timer / No Registry)
    /// Host가 소유하며, Host가 정책(재연결/설정로드/저장)을 담당한다.
    /// </summary>
    public sealed class OpcUaRuntime : IDisposable
    {
        private readonly object _sync = new object();

        private readonly List<OPCUAMember_server> _servers = new List<OPCUAMember_server>();

        private CancellationToken _token;
        private bool _started;
        private bool _disposed;

        /// <summary>
        /// 필요하면 Host(UI)가 전달. Console Host는 null 전달해도 됨.
        /// Core는 UI 개념을 직접 사용하지 않지만, 기존 server 코드가 UI marshal이 필요하면 전달한다.
        /// </summary>
        private readonly SynchronizationContext _uiContext;

        public event Action<OPCUAMember_server, Exception> ReconnectFailed;
        public event Action<OPCUAMember_server> ReconnectSucceeded;

        // 추가 예외 이벤트
        public event Action<OPCUAMember_server, Exception> ServerInitFailed;
        public event Action<OPCUAMember_server, Exception> ServerUninitFailed;
        public event Action<string, string, Exception> WriteValueFailed;

        /// <summary>
        /// AddServer 호출 시 발생. RuntimeEventBridge 등에서 동적 등록에 사용.
        /// </summary>
        public event Action<OPCUAMember_server> ServerAdded;

        public OpcUaRuntime(SynchronizationContext uiContext = null)
        {
            _uiContext = uiContext;
        }

        public void Start(CancellationToken token)
        {
            ThrowIfDisposed();

            lock (_sync)
            {
                if (_started) throw new InvalidOperationException("OpcUaRuntime already started.");
                _token = token;
                _started = true;
            }
        }

        public async Task StopAsync()
        {
            ThrowIfDisposed();

            List<OPCUAMember_server> snapshot;

            lock (_sync)
            {
                if (!_started) return;
                _started = false;

                snapshot = new List<OPCUAMember_server>(_servers);
                _servers.Clear();
            }

            // best-effort cleanup
            for (int i = 0; i < snapshot.Count; i++)
            {
                try
                {
                    await snapshot[i].ServerUninitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ServerUninitFailed?.Invoke(snapshot[i], ex);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try { StopAsync().GetAwaiter().GetResult(); }
            catch { }
        }

        // =========================
        // Server registry (Host가 구성)
        // =========================

        public IReadOnlyList<OPCUAMember_server> GetServersSnapshot()
        {
            lock (_sync)
            {
                return _servers.ToArray();
            }
        }
        public IReadOnlyList<OPCUAMember_server> Servers
        {
            get
            {
                lock (_sync)
                    return _servers.ToList();
            }
        }
        public OPCUAMember_server FindServer(string accessName)
        {
            if (string.IsNullOrEmpty(accessName))
                return null;

            lock (_sync)
            {
                for (int i = 0; i < _servers.Count; i++)
                {
                    var s = _servers[i];
                    if (string.Equals(
                        s.accessName,
                        accessName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return s;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Host가 Config를 읽어서 서버 객체를 만들어 넣는다.
        /// (Core는 Config 저장/로드를 모른다)
        /// </summary>
        /// 
        public void AddServer(OPCUAMember_server server)
        {
            ThrowIfDisposed();

            if (server == null)
                throw new ArgumentNullException(nameof(server));

            lock (_sync)
            {
                // accessName 기준 중복 방지(기존 코드 정책 유지)
                for (int i = 0; i < _servers.Count; i++)
                {
                    if (string.Equals(_servers[i].accessName, server.accessName, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Server already exists: " + server.accessName);
                }

                server.AttachUiContext(_uiContext);
                _servers.Add(server);
            }

            try { ServerAdded?.Invoke(server); } catch { }
        }

        public bool RemoveServer(OPCUAMember_server server)
        {
            if (server == null)
                return false;

            ThrowIfDisposed();
            EnsureStarted();

            // OPCUAMember_server found = null;

            bool removed = false;

            lock (_sync)
            {
                removed = _servers.Remove(server);
            }

            return removed;
        }

        // =========================
        // Connection control (Host가 호출)
        // =========================

        /// <summary>
        /// 등록된 서버 전부 연결 시도 (Host가 최초 1회 호출)
        /// </summary>
        public async Task ConnectAllAsync()
        {
            ThrowIfDisposed();
            EnsureStarted();

            var list = GetServersSnapshot();
            for (int i = 0; i < list.Count; i++)
            {
                if (_token.IsCancellationRequested) break;

                var s = list[i];
                if (s == null) continue;

                try
                {
                    await s.ServerInitAsync(_token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // 개별 실패는 전체를 막지 않는다.
                    ServerInitFailed?.Invoke(s, ex);
                }
            }
        }

        /// <summary>
        /// 재연결 Tick: 죽은 서버만 재연결 시도.
        /// Timer는 Host가 가진다(예: 30초 주기).
        /// </summary>
        public async Task ReconnectDeadServersAsync()
        {
            ThrowIfDisposed();
            EnsureStarted();

            foreach (var s in GetServersSnapshot())
            {
                if (_token.IsCancellationRequested || s == null || s.bServerAlive)
                    continue;

                try
                {
                    await s.ServerInitAsync(_token).ConfigureAwait(false);
                    ReconnectSucceeded?.Invoke(s);
                }
                catch (Exception ex)
                {
                    ReconnectFailed?.Invoke(s, ex);
                }
            }
        }

        // =========================
        // Runtime query API (IPC가 호출)
        // =========================

        public OPCUAMember_item FindItem(string serverName, string groupName, string itemName)
        {
            ThrowIfDisposed();
            EnsureStarted();

            if (string.IsNullOrEmpty(serverName) ||
                string.IsNullOrEmpty(groupName) ||
                string.IsNullOrEmpty(itemName))
                return null;

            var list = GetServersSnapshot();

            for (int si = 0; si < list.Count; si++)
            {
                var srv = list[si];
                if (srv == null) continue;

                if (!string.Equals(srv.accessName, serverName, StringComparison.OrdinalIgnoreCase))
                    continue;

                var groups = srv.arrGroup;
                if (groups == null) return null;

                for (int gi = 0; gi < groups.Count; gi++)
                {
                    var grp = groups[gi];
                    if (grp == null) continue;

                    if (!string.Equals(grp.sName, groupName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var items = grp.arrItem;
                    if (items == null) return null;

                    for (int ii = 0; ii < items.Count; ii++)
                    {
                        var item = items[ii];
                        if (item == null) continue;

                        if (string.Equals(item.sName, itemName, StringComparison.OrdinalIgnoreCase))
                            return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 기존 TryGetUaData와 동일 의미.
        /// 레지스트리 접근 없이, subscription cache(DataValue)에서 읽는다.
        /// </summary>
        public bool TryGetUaData(string server, string group, string item, out DataValue dv)
        {
            dv = null;

            var it = FindItem(server, group, item);
            if (it == null) return false;

            dv = it.dvDataValue;
            return dv != null;
        }

        public async Task<StatusCode> WriteValueAsync(
          string serverAccessName,
          string nodeId,
          Variant value,
          CancellationToken ct)
        {
            if (string.IsNullOrEmpty(serverAccessName))
                return StatusCodes.BadInvalidArgument;
            if (string.IsNullOrEmpty(nodeId))
                return StatusCodes.BadInvalidArgument;

            var server = GetServer(serverAccessName);
            if (server == null)
                return StatusCodes.BadNotFound;

            try
            {
                return await server.WriteValueAsync(nodeId, value, ct)
                                  .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                WriteValueFailed?.Invoke(serverAccessName, nodeId, ex);
                return StatusCodes.Bad;
            }
        }


        /// <summary>
        /// AccessName 기준으로 Server 조회
        /// Host / WriteService에서 사용
        /// </summary>
        public OPCUAMember_server GetServer(string accessName)
        {
            if (string.IsNullOrEmpty(accessName))
                return null;

            lock (_sync)
            {
                for (int i = 0; i < _servers.Count; i++)
                {
                    var srv = _servers[i];
                    if (srv == null)
                        continue;

                    if (string.Equals(
                        srv.accessName,
                        accessName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return srv;
                    }
                }
            }

            return null;
        }

        // =========================
        // Helpers 
        // =========================


        //ThrowIfDisposed, EnsureStarted 의 경우만 throw를 허용.
        private void EnsureStarted()
        {
            if (!_started) throw new InvalidOperationException("OpcUaRuntime not started.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(OpcUaRuntime));
        }


        public RuntimeReadResult ReadItem(
            string accessName,
            string groupName,
            string itemName)
        {
            ThrowIfDisposed();
            EnsureStarted();

            var it = FindItem(accessName, groupName, itemName);
            if (it == null)
            {
                return new RuntimeReadResult
                {
                    Success = false,
                    Error = "Item not found"
                };
            }

            var dv = it.dvDataValue;
            if (dv == null)
            {
                return new RuntimeReadResult
                {
                    Success = false,
                    Error = "No cached value"
                };
            }

            return new RuntimeReadResult
            {
                Success = true,
                Value = dv.Value,
                Quality = (int)dv.StatusCode.Code,
                Timestamp = dv.SourceTimestamp
            };
        }

        public RuntimeReadResult ReadItemByFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return new RuntimeReadResult { Success = false, Error = "Empty name" };

            var parts = fullName.Split('.');
            if (parts.Length < 3)
                return new RuntimeReadResult { Success = false, Error = "Invalid full name" };

            return ReadItem(parts[0], parts[1], parts[2]);
        }

        public async Task<bool> WriteItemAsync(
            string accessName,
            string groupName,
            string itemName,
            object value,
            CancellationToken ct)
        {
            ThrowIfDisposed();
            EnsureStarted();

            var it = FindItem(accessName, groupName, itemName);
            if (it == null)
                return false;

            // NodeId는 Item이 이미 알고 있음
            var nodeId = it.sNode;
            if (string.IsNullOrEmpty(nodeId))
                return false;

            Variant v;
            try
            {
                v = new Variant(value);
            }
            catch
            {
                return false;
            }

            var status = await WriteValueAsync(
                accessName,
                nodeId,
                v,
                ct).ConfigureAwait(false);

            return StatusCode.IsGood(status);
        }

        public Task<bool> WriteItemByFullNameAsync(
            string fullName,
            object value,
            CancellationToken ct)
        {
            var p = fullName.Split('.');
            if (p.Length < 3)
                return Task.FromResult(false);

            return WriteItemAsync(p[0], p[1], p[2], value, ct);
        }

    }
}
