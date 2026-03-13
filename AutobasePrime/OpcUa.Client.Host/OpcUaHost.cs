using System;
using System.Threading;
using System.Threading.Tasks;
using OPCUA.Client.Core;
using AutoLibLocal;
using OPCUA.Client.Host;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using System.IO;
using System.Collections.Generic;
using OpcUa.Client.Abstractions;
using System.Linq;
using BitFaster.Caching;
using OpcUa.Client.Ipc.Server;

namespace OpcUa.Client.Host
{
    /// <summary>
    /// OPC UA Host
    /// - Config Load / Save
    /// - Runtime lifecycle
    /// - Reconnect Timer policy
    /// </summary>
    public sealed class OpcUaHost : IDisposable
    {
        private readonly OpcUaRuntime _runtime;
        private static IpcServer _ipcServer;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private Timer _reconnectTimer;
        private bool _started;

        private bool _disposed;

        // 정책 값 (Config에서 읽음)
        private bool _reconnectEnabled = true;
        private TimeSpan _reconnectInterval = TimeSpan.FromSeconds(30);

        public bool IsReconnectEnabled => _reconnectEnabled;
        public TimeSpan ReconnectInterval => _reconnectInterval;

        private readonly ILogger _logger;

        public IReadOnlyList<OPCUAMember_server> Servers
         => _runtime.Servers;

        public ApplicationConfiguration UaConfig { get; private set; }

        private bool _autoTrustStore;

        public event Action ServersChanged;
        public event Action<OpcUaHostState, string> HostStateChanged;
        /// <summary>
        /// IPC 클라이언트(LocalMain)에서 Shutdown 명령을 수신했을 때 발생.
        /// UI에서 Application.Exit()를 호출해야 한다.
        /// </summary>
        public event Action ShutdownRequested;
        private void RaiseHostState(OpcUaHostState state, string message = null)
        {
            HostStateChanged?.Invoke(state, message);
        }
        private void RaiseServersChanged()
        {
            ServersChanged?.Invoke();
        }

        private OPCUAMember_server FindRuntimeServer(string accessName)
        {
            return _runtime.FindServer(accessName);
        }



        public enum OpcUaHostState
        {
            Stopped,
            Starting,
            Running,
            Stopping,
            Error
        }


        public OpcUaHost(ILogger logger = null)
        {
            _logger = logger;
            _runtime = new OpcUaRuntime(SynchronizationContext.Current);

            // 재연결 이벤트 처리
            _runtime.ReconnectFailed += (server, ex) =>
            {
                RaiseHostState(
                    OpcUaHostState.Error,
                    $"Reconnect failed [{server.accessName}]: {ex.Message}");
                _logger?.LogError(ex, "Reconnect failed for server: {ServerName}", server.accessName);
            };

            _runtime.ReconnectSucceeded += (server) =>
            {
                RaiseHostState(
                    OpcUaHostState.Running,
                    $"Server reconnected: {server.accessName}");
                _logger?.LogInformation("Server reconnected: {ServerName}", server.accessName);
            };

            // 서버 초기화 실패 이벤트 처리
            _runtime.ServerInitFailed += (server, ex) =>
            {
                RaiseHostState(
                    OpcUaHostState.Error,
                    $"Server init failed [{server.accessName}]: {ex.Message}");
                _logger?.LogError(ex, "Server initialization failed: {ServerName}", server.accessName);
            };

            // 서버 종료 실패 이벤트 처리
            _runtime.ServerUninitFailed += (server, ex) =>
            {
                RaiseHostState(
                    OpcUaHostState.Error,
                    $"Server uninit failed [{server.accessName}]: {ex.Message}");
                _logger?.LogError(ex, "Server uninitialization failed: {ServerName}", server.accessName);
            };

            // Write 실패 이벤트 처리
            _runtime.WriteValueFailed += (serverName, nodeId, ex) =>
            {
                RaiseHostState(
                    OpcUaHostState.Error,
                    $"Write failed [{serverName}] NodeId={nodeId}: {ex.Message}");
                _logger?.LogError(ex, "Write value failed. Server: {ServerName}, NodeId: {NodeId}", serverName, nodeId);
            };
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _ipcServer?.Stop();
            StopAsync().GetAwaiter().GetResult();
            
            _cts.Dispose();
        }

        // =========================
        // Start / Stop
        // =========================

        public void Start()
        {
            if (_started)
                throw new InvalidOperationException("OpcUaHost already started.");

            _started = true;
            RaiseHostState(OpcUaHostState.Starting, "Host starting");

            LoadConfig();

            // 여기서 Config 단 1회 생성
            UaConfig = OpcUaHostConfigBuilder.BuildUaConfigFromHostPolicy();


            BuildRuntimeServers();
            RaiseServersChanged();

            _runtime.Start(_cts.Token);

            // 최초 1회 즉시 연결
            _ = Task.Run(async () =>
            {
                await _runtime.ConnectAllAsync();
                RaiseServersChanged();          // 초기 연결 결과를 UI에 반영
            });

            // 재연결 Timer
            _reconnectTimer = new Timer(
                OnReconnectTick,
                null,
                _reconnectInterval,
                _reconnectInterval);

            _ipcServer = new IpcServer(_runtime, "Autobase.OPCUA.Client.Runtime");
            _ipcServer.ShutdownRequested += () => ShutdownRequested?.Invoke();
            _ipcServer.Start();

            Log("OPC UA Host started.");
            RaiseHostState(OpcUaHostState.Running, "Host running");
        }

        private async Task StopAsync()
        {
            if (!_started)
                return;

            RaiseHostState(OpcUaHostState.Stopping, "Host stopping");
            _started = false;

            Log("OPC UA Host stopping...");

            try { _reconnectTimer?.Dispose(); } catch { }
            try { _cts.Cancel(); } catch { }

            // 런타임 종료 전에 최종값을 Config에 반영
            try { SnapshotLastValues(); } catch { }

            try { await _runtime.StopAsync().ConfigureAwait(false); }
            catch { }

            try { OpcUaClientConfigManager.Save(); } catch { }

            RaiseHostState(OpcUaHostState.Stopped, "Host stopped");
            Log("OPC UA Host stopped.");
        }


        // =========================
        // Snapshot last runtime values
        // =========================

        /// <summary>
        /// 런타임 서버/그룹/아이템의 최종값을 ConfigManager의 definition에 반영.
        /// StopAsync() 전에 호출해야 한다 (_runtime.Servers가 비워지기 전).
        /// </summary>
        private void SnapshotLastValues()
        {
            var servers = _runtime.Servers;
            if (servers == null || servers.Count == 0)
                return;

            foreach (var srv in servers)
            {
                if (srv?.arrGroup == null)
                    continue;

                foreach (var grp in srv.arrGroup)
                {
                    if (grp?.arrItem == null)
                        continue;

                    foreach (var item in grp.arrItem)
                    {
                        if (item == null)
                            continue;

                        string val = item.sValue;
                        if (string.IsNullOrEmpty(val))
                            continue;

                        OpcUaClientConfigManager.UpdateItem(
                            srv.accessName,
                            grp.sName,
                            item.sName,
                            def => { def.LastValue = val; });
                    }
                }
            }
        }

        // =========================
        // Config & Runtime build
        // =========================
        private void LoadConfig()
        {
            string baseDir =
                AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData\\UaClient";

            OpcUaClientConfigManager.Initialize(baseDir);

            _reconnectEnabled =
                TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "OpcClientReconnect", true);

            int sec =
                TotalConfig.LoadRegAutoBaseConfig(
                    "Config", "Start", "OpcClientReconnectInterval", 30);

            _reconnectInterval = TimeSpan.FromSeconds(Math.Max(5, sec));

            _autoTrustStore =
             TotalConfig.LoadRegAutoBaseConfig(
                 "Config",
                 "Start",
                 "OpcUaClientAutoTrustStore",
                 true); // 개발: true / 운영: false
        }

        /// <summary>
        /// ConfigManager에서 서버/그룹/아이템 로딩 → Runtime 구성
        /// </summary>
        private void BuildRuntimeServers()
        {
            var defs = OpcUaClientConfigManager.LoadServerDefinitions();

            foreach (var def in defs)
            {
                var options = BuildServerOptions(def);

                var server = new OPCUAMember_server(
                    def.ServerName,
                    def.ServerDisplayName,
                    def.HostName,
                    def.AccessName,
                    def.EndpointIndex,
                    UaConfig,      //  Host가 가진 Config 사용
                    options,
                    _logger   // 없으면 null
                );

                // 사용자가 선택한 endpoint 보안 정보 설정
                server.endpointUrl = def.EndpointUrl;
                server.securityPolicyUri = def.SecurityPolicyUri;
                server.securityMode = def.SecurityMode;

                foreach (var g in def.Groups)
                {
                    var group = new OPCUAMember_group(
                        server,
                        g.GroupName,
                        g.PublishingInterval);

                    foreach (var i in g.Items)
                    {
                        var item = new OPCUAMember_item(i.Name, i.NodeId);
                        group.AddItem(item);
                    }

                    server.arrGroup.Add(group);
                }

                _runtime.AddServer(server);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(OpcUaHost));
        }

        private void EnsureStarted()
        {
            if (!_started)
                throw new InvalidOperationException(
                    "OpcUaHost is not started.");
        }

        private void EnsureUsable()
        {
            ThrowIfDisposed();
            EnsureStarted();
        }

        public async Task AddServerAsync(OpcUaServerDefinition def)
        {
            if (FindRuntimeServer(def.AccessName) != null)
                throw new InvalidOperationException(
                    $"Server '{def.AccessName}' already exists.");

            if (def == null)
                throw new ArgumentNullException(nameof(def));

            EnsureUsable();

            // 1️ Config 정의 추가
            OpcUaClientConfigManager.AddServer(def);
            OpcUaClientConfigManager.Save();

            // 2️ Runtime 서버 생성
            var server = new OPCUAMember_server(
                def.ServerName,
                def.ServerDisplayName,
                def.HostName,
                def.AccessName,
                def.EndpointIndex,
                UaConfig,
                BuildServerOptions(def),
                _logger
            );

            // 사용자가 선택한 endpoint 보안 정보 설정
            server.endpointUrl = def.EndpointUrl;
            server.securityPolicyUri = def.SecurityPolicyUri;
            server.securityMode = def.SecurityMode;

            // Group / Item 구성
            foreach (var g in def.Groups)
            {
                var group = new OPCUAMember_group(
                    server,
                    g.GroupName,
                    g.PublishingInterval);

                foreach (var i in g.Items)
                {
                    group.AddItem(new OPCUAMember_item(i.Name, i.NodeId));
                }

                server.arrGroup.Add(group);
            }

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            // 3️ Runtime 등록
            _runtime.AddServer(server);

            // 4️ 즉시 연결 (정책)
            await server.ServerInitAsync(_cts.Token);
        }


        public async Task<bool> ConnectServerAsync(OPCUAMember_server server)
        {
            if (server == null)
                return false;

            EnsureUsable();

            if (server.bServerAlive)
                return true;

            // bConnecting이면 초기 연결 등이 진행 중 → 완료를 기다림
            // (ServerInitAsync 내부의 _connectLock으로 직렬화됨)

            try
            {
                RaiseServersChanged();

                await server.ServerInitAsync(_cts.Token).ConfigureAwait(false);

                RaiseHostState(
                    OpcUaHostState.Running,
                    $"Server connected: {server.accessName}");

                return true;
            }
            catch (Exception ex)
            {
                RaiseHostState(
                    OpcUaHostState.Error,
                    $"Connect failed [{server.accessName}]: {ex.Message}");

                _logger?.LogError(ex, "OPC UA connect failed");

                return false;
            }
            finally
            {
                RaiseServersChanged();
            }
        }
        public async Task<bool> DisconnectServerAsync(OPCUAMember_server server)
        {
            if (server == null)
                return false;

            EnsureUsable();

            // 이미 끊겨 있고 연결 중도 아니면 성공으로 간주
            if (!server.bServerAlive && !server.bConnecting)
                return true;

            try
            {
                server.bConnecting = true;
                RaiseServersChanged();   // UI: 🟡 Disconnecting 표시

                await server.ServerUninitAsync().ConfigureAwait(false);

                RaiseHostState(
                    OpcUaHostState.Running,
                    $"Server disconnected: {server.accessName}");

                return true;
            }
            catch (Exception ex)
            {
                //  예외는 Host 내부에서만 처리
                RaiseHostState(
                    OpcUaHostState.Error,
                    $"Disconnect failed [{server.accessName}]: {ex.Message}");

                _logger?.LogError(ex, "OPC UA disconnect failed");

                return false;
            }
            finally
            {
                server.bConnecting = false;     // Disconnect는 ServerUninitAsync에서 관리 안 함
                RaiseServersChanged();
            }
        }

        public async Task RemoveServerAsync(string accessName)
        {
            EnsureUsable();

            if (string.IsNullOrEmpty(accessName))
                return;

            var server = FindRuntimeServer(accessName);

            // 1️ Runtime 정리
            if (server != null)
            {
                try
                {
                    await server.ServerUninitAsync();
                }
                catch { }

                _runtime.RemoveServer(server);
            }

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            // 2️ Config 제거
            OpcUaClientConfigManager.RemoveServer(accessName);
            OpcUaClientConfigManager.Save();
        }

        public async Task ModifyServerAsync(
            string accessName,
            Action<OpcUaServerDefinition> updater)
        {
            EnsureUsable();

            if (string.IsNullOrEmpty(accessName))
                throw new ArgumentNullException(nameof(accessName));


            var server = FindRuntimeServer(accessName);
            if (server == null)
                throw new InvalidOperationException("Runtime server not found.");

            bool wasConnected = server.bServerAlive;

            // 1️ 안전하게 Disconnect
            if (wasConnected)
                await server.ServerUninitAsync();

            // 2️ Config 수정
            bool updated = OpcUaClientConfigManager.UpdateServer(
                accessName,
                updater);

            if (!updated)
                throw new InvalidOperationException("Config server not found.");

            // 3️ Config → Runtime 반영
            var def = OpcUaClientConfigManager
                .LoadServerDefinitions()
                .First(s => s.AccessName == accessName);

            server.serverName = def.ServerName;
            server.ServerDisplayName = def.ServerDisplayName;
            server.hostName = def.HostName;
            server.endpointindex = def.EndpointIndex;
            server.endpointUrl = def.EndpointUrl;
            server.securityPolicyUri = def.SecurityPolicyUri;
            server.securityMode = def.SecurityMode;

            // AccessName 변경 금지 >> RenameServer
            if (!string.Equals(accessName, def.AccessName,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Changing AccessName is not supported.");
            }

            // Update options with new identity
            server.UpdateOptions(BuildServerOptions(def));

            OpcUaClientConfigManager.Save();

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            // 4️ 재연결
            if (wasConnected)
                await server.ServerInitAsync(_cts.Token);
        }

        private OpcUaServerOptions BuildServerOptions(OpcUaServerDefinition def)
        {
            var options = new OpcUaServerOptions
            {
                AutoTrustStore = _autoTrustStore
            };

            try
            {
                options.UserIdentity = OpcUaIdentityBuilder.Build(def, UaConfig);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "Failed to build user identity for server {AccessName}. Falling back to Anonymous.",
                    def?.AccessName);
            }

            return options;
        }


        // =========================
        // Reconnect policy
        // =========================

        private async void OnReconnectTick(object state)
        {
            if (_disposed || !_started || !_reconnectEnabled)
                return;

            try
            {
                await _runtime.ReconnectDeadServersAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
               // RaiseHostState(OpcUaHostState.Error, ex.Message);
                _logger?.LogCritical(ex, "Reconnect loop crashed");
            }
        }

        public void UpdateReconnectPolicy(bool enabled, TimeSpan interval)
        {
            ThrowIfDisposed();

            _reconnectEnabled = enabled;
            _reconnectInterval = interval;

            RestartReconnectTimer();
        }

        private void RestartReconnectTimer()
        {
            _reconnectTimer?.Dispose();
            _reconnectTimer = null;

            if (!_reconnectEnabled)
                return;

            _reconnectTimer = new Timer(
                OnReconnectTick,
                null,
                _reconnectInterval,
                _reconnectInterval);


            TotalConfig.SaveRegAutoBaseConfig(
                "Config", "Start", "OpcClientReconnect", _reconnectEnabled);

            TotalConfig.SaveRegAutoBaseConfig(
                "Config", "Start", "OpcClientReconnectInterval", (int)_reconnectInterval.TotalSeconds);
        }

        // =========================
        // Logging
        // =========================

        private void Log(string msg)
        {
            Console.WriteLine($"[HOST] {DateTime.Now:HH:mm:ss} {msg}");
        }

        public async Task AddGroupAsync(
            string serverAccessName,
            string groupName,
            int publishingInterval)
        {
            EnsureUsable();

            var server = FindRuntimeServer(serverAccessName)
                ?? throw new InvalidOperationException("Server not found.");

            bool ok = OpcUaClientConfigManager.AddGroup(
               serverAccessName,
               new OpcUaGroupDefinition
               {
                   GroupName = groupName,
                   PublishingInterval = publishingInterval
               });

            if (!ok)
                throw new InvalidOperationException("Config server not found.");

            OpcUaClientConfigManager.Save();

            // 2️⃣ Runtime
            var group = new OPCUAMember_group(
                server,
                groupName,
                publishingInterval);

            server.arrGroup.Add(group);

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            // 3️ 재구독
            if (server.bServerAlive)
            {
                await group.InitializeAsync(
                    server.seServer,
                    _cts.Token);
            }
        }

        public void ModifyGroup(
            string serverAccessName,
            string groupName,
            Action<OpcUaGroupDefinition> updater)
        {
            EnsureUsable();

            var server = FindRuntimeServer(serverAccessName)
                ?? throw new InvalidOperationException("Server not found.");

            var group = server.arrGroup
                .FirstOrDefault(g => g.sName == groupName)
                ?? throw new InvalidOperationException("Group not found.");

            //bool wasConnected = server.bServerAlive;

            //if (wasConnected)
            //    await server.ServerUninitAsync();

            // 1️⃣ Config
            bool updated = OpcUaClientConfigManager.UpdateServer(
                serverAccessName,
                s =>
                {
                    var g = s.Groups.First(x => x.GroupName == groupName);
                    updater(g);
                });

            if (!updated)
                throw new InvalidOperationException("Config update failed.");

            // 2️⃣ Runtime 반영
            var def = OpcUaClientConfigManager.LoadServerDefinitions()
                .First(s => s.AccessName == serverAccessName)
                .Groups.First(g => g.GroupName == groupName);

            group.Update(def.GroupName, def.PublishingInterval);

            OpcUaClientConfigManager.Save();

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();


        }

        public async Task RemoveGroupAsync(
    string serverAccessName,
    string groupName)
        {
            EnsureUsable();

            var server = FindRuntimeServer(serverAccessName)
                ?? throw new InvalidOperationException("Server not found.");

            var group = server.arrGroup
                .FirstOrDefault(g => g.sName == groupName);

            if (group == null)
                return;

            if (server.bServerAlive)
            {
                await group.ClearSubscriptionAsync();
            }

            // 1️⃣ Config
            OpcUaClientConfigManager.UpdateServer(
                serverAccessName,
                s =>
                {
                    s.Groups.RemoveAll(g => g.GroupName == groupName);
                });

            OpcUaClientConfigManager.Save();

            // 2️⃣ Runtime
            server.arrGroup.Remove(group);

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            await group.DeleteSubscriptionAsync();

        }


        public async Task AddItemAsync(
            string serverAccessName,
            string groupName,
            OpcUaItemDefinition itemDef)
        {
            EnsureUsable();

            var server = FindRuntimeServer(serverAccessName);
            var group = server.arrGroup.First(g => g.sName == groupName);

            // 1️⃣ Config
            OpcUaClientConfigManager.UpdateServer(
                serverAccessName,
                s =>
                {
                    var g = s.Groups.First(x => x.GroupName == groupName);
                    g.Items.Add(itemDef);
                });


            OpcUaClientConfigManager.Save();

            // 2️⃣ Runtime
            var item = new OPCUAMember_item(itemDef.Name, itemDef.NodeId);

            //  Runtime 구조 반영 (항상)
            group.AddItem(item);

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            if (server.bServerAlive)
            {
                await group.AddItemAsync(
                    item,
                    server.seServer,
                    _cts.Token);
            }

        }

        public async Task RemoveItemAsync(
    string accessName,
    string groupName,
    string itemName)
        {
            EnsureUsable();

            if (string.IsNullOrEmpty(accessName))
                throw new ArgumentNullException(nameof(accessName));
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException(nameof(itemName));

            // 1️⃣ Runtime Server 찾기
            var server = FindRuntimeServer(accessName);
            if (server == null)
                throw new InvalidOperationException("Runtime server not found.");

            // 2️⃣ Runtime Group 찾기
            var group = server.arrGroup
                .FirstOrDefault(g => g.sName == groupName);

            if (group == null)
                throw new InvalidOperationException("Runtime group not found.");

            // 3️⃣ Runtime Item 찾기
            var item = group.arrItem
                .FirstOrDefault(i => i.sName == itemName);

            if (item == null)
                return; // 이미 없음 → 무시 정책

            bool needResubscribe = server.bServerAlive;

            // 4️⃣ Runtime 구조 제거
            group.RemoveItem(item);

            // 5️⃣ Config 제거
            OpcUaClientConfigManager.RemoveItem(
                accessName,
                groupName,
                itemName);

            OpcUaClientConfigManager.Save();

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            if (server.bServerAlive)
            {
                await group.RemoveItemAsync(item, _cts.Token);
            }
        }

        public async Task ModifyItemAsync(
    string accessName,
    string groupName,
    string itemName,
    Action<OpcUaItemDefinition> updater)
        {
            EnsureUsable();

            if (string.IsNullOrEmpty(accessName))
                throw new ArgumentNullException(nameof(accessName));
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException(nameof(itemName));
            if (updater == null)
                throw new ArgumentNullException(nameof(updater));

            // 1️⃣ Runtime Server 찾기
            var server = FindRuntimeServer(accessName);
            if (server == null)
                throw new InvalidOperationException("Runtime server not found.");

            // 2️⃣ Runtime Group 찾기
            var group = server.arrGroup
                .FirstOrDefault(g => g.sName == groupName);

            if (group == null)
                throw new InvalidOperationException("Runtime group not found.");

            // 3️⃣ Runtime Item 찾기
            var item = group.arrItem
                .FirstOrDefault(i => i.sName == itemName);

            if (item == null)
                throw new InvalidOperationException("Runtime item not found.");

            bool wasConnected = server.bServerAlive;
            bool nodeChanged;

            // 4️⃣ Config 수정
            nodeChanged = OpcUaClientConfigManager.UpdateItem(
                accessName,
                groupName,
                itemName,
                updater);

            // Config 파일에 항상 저장
            OpcUaClientConfigManager.Save();

            if (!nodeChanged && !wasConnected)
            {
                // 이름만 바뀌었고 서버도 꺼져있으면
                ApplyItemChangeFromConfig(group, itemName);
                RaiseServersChanged();
                return;
            }

            // 5️⃣ Config → Runtime 반영
            ApplyItemChangeFromConfig(group, itemName);

            //통신 반영 전에 상태 변경 이벤트 발생
            RaiseServersChanged();

            // 6️⃣ NodeId 변경 + 연결 중이면 재구독
            if (wasConnected && nodeChanged)
            {
                await group.RecreateSubscriptionAsync(
              server.seServer,
              _cts.Token);
            }
        }

        private void ApplyItemChangeFromConfig(
    OPCUAMember_group group,
    string itemName)
        {
            var def = OpcUaClientConfigManager
                .LoadServerDefinitions()
                .SelectMany(s => s.Groups)
                .Where(g => g.GroupName == group.sName)
                .SelectMany(g => g.Items)
                .First(i => i.Name == itemName);

            var item = group.arrItem
                .First(i => i.sName == itemName);

            item.Update(
                def.NodeId,
                def.Name);
        }


        public async Task DisconnectAllAsync(string reason = null)
        {
            EnsureUsable();

            var servers = GetServersSnapshot(); // thread-safe snapshot
            if (servers == null || servers.Count == 0)
                return;

            RaiseHostState(
                OpcUaHostState.Running,
                reason ?? "Disconnecting all servers");

            for (int i = 0; i < servers.Count; i++)
            {
                if (_cts.IsCancellationRequested)
                    break;

                var s = servers[i];
                if (s == null)
                    continue;

                // 이미 끊겨 있으면 스킵 (DisconnectServerAsync 내부에서도 가드 있음)
                await DisconnectServerAsync(s).ConfigureAwait(false);
            }
        }

        // 인증서 Reject 발생 시
        public void OnServerCertificateRejected()
        {
           // DisconnectAll("Server certificate rejected");
           // StopAutoReconnect();
        }
        public IReadOnlyList<OPCUAMember_server> GetServersSnapshot()
        {
            return _runtime.Servers.ToList();
        }

        // Client 인증서 Regenerate 시
        public async Task OnClientCertificateRegenerated()
        {
            await DisconnectAllAsync("Client certificate changed");
            await Task.Delay(500);

            foreach (var server in GetServersSnapshot())
            {
                await ConnectServerAsync(server);
            }
        }


    }
}
