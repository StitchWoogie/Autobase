using AutoLibLocal;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    public sealed class OPCUAMember_server : IDisposable
    {
        public string serverName;
        public string hostName;
        public string accessName;
        public int endpointindex;
        public string ServerDisplayName { get; set; }

        // 사용자가 선택한 endpoint 보안 정보
        public string endpointUrl;
        public string securityPolicyUri;
        public int securityMode;  // MessageSecurityMode: 1=None, 2=Sign, 3=SignAndEncrypt

        public ISession seServer;
        private ISessionFactory _sessionFactory;

        public List<OPCUAMember_group> arrGroup;

        private readonly ILogger _logger;
        private readonly ApplicationConfiguration _config;
        private OpcUaServerOptions _options;

        private bool _certValidationHooked = false;
        private bool _disposed;

        private bool _bServerAlive = false;
        private bool _bConnecting = false;
        public bool bConnecting
        {
            get { return _bConnecting; }
            set
            {
                if (_bConnecting == value) return;
                _bConnecting = value;
                try { StateChanged?.Invoke(); } catch { }
            }
        }

        public event Action StateChanged;

        internal SynchronizationContext UiContext { get; private set; }

        public event EventHandler ServerStateChanged;
        public event EventHandler<string> RuntimeLog;

        private void SetServerAlive(bool alive)
        {
            if (bServerAlive == alive)
                return;

            bServerAlive = alive;
            ServerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Log(string message)
        {
            RuntimeLog?.Invoke(this, message);
        }

        internal void AttachUiContext(SynchronizationContext ctx)
        {
            UiContext = ctx;
        }

        public bool bServerAlive
        {
            get { return _bServerAlive; }
            private set
            {
                if (_bServerAlive == value) return;
                _bServerAlive = value;
                try { StateChanged?.Invoke(); } catch { }
            }
        }

        private readonly SemaphoreSlim _connectLock = new SemaphoreSlim(1, 1);

        public OPCUAMember_server(
            string _server,
            string _serverName,
            string _host,
            string _access,
            int _index,
            ApplicationConfiguration config,
            OpcUaServerOptions options,
            ILogger logger = null,
            ISessionFactory sessionFactory = null)
        {
            serverName = _server;
            ServerDisplayName = _serverName;
            hostName = _host;
            accessName = _access;
            endpointindex = _index;

            arrGroup = new List<OPCUAMember_group>();

            _config = config ?? throw new ArgumentNullException(nameof(config));
            _options = options ?? new OpcUaServerOptions();
            _logger = logger;

            _sessionFactory = sessionFactory; // null이면 Init에서 DefaultSessionFactory 생성
        }

        public void UpdateOptions(OpcUaServerOptions newOptions)
        {
            if (bServerAlive)
                throw new InvalidOperationException("Cannot update options while connected.");
            _options = newOptions ?? throw new ArgumentNullException(nameof(newOptions));
        }

        public async Task ServerInitAsync(CancellationToken ct)
        {
            ThrowIfDisposed();
            if (bServerAlive) return;

            await _connectLock.WaitAsync(ct).ConfigureAwait(false);
            try
            
            {
                if (bServerAlive) return;

                bConnecting = true;

                // 기존 세션 정리
                await SafeDisposeSessionAsync().ConfigureAwait(false);

                if (!_certValidationHooked)
                    HookCertificateValidationOnce(_config, _options);

                if (_sessionFactory == null)
                    _sessionFactory = new DefaultSessionFactory(OpcUaClientTelemetry.Telemetry); 

                // Endpoint 선택
                _logger?.LogInformation(
                    "ServerInitAsync: Selecting endpoint. Host={Host}", hostName);
                EndpointDescription selectedEndpoint = await SelectEndpointAsync(ct).ConfigureAwait(false);
                if (selectedEndpoint == null)
                    throw new Exception($"No suitable OPC UA endpoint found. Host={hostName}");

                var endpointConfig = EndpointConfiguration.Create(_config);
                var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfig);

                // Session 생성
                // updateBeforeConnect=false: SelectEndpointAsync에서 이미
                // FindServers+GetEndpoints 완료했으므로 중복 Discovery 불필요
                seServer = await _sessionFactory.CreateAsync(
                    configuration: _config,
                    endpoint: endpoint,
                    updateBeforeConnect: false,
                    checkDomain: false,
                    sessionName: _options.SessionName ?? _config.ApplicationName,
                    sessionTimeout: _options.SessionTimeoutMs,
                    identity: _options.UserIdentity ?? new UserIdentity(new AnonymousIdentityToken()),
                    preferredLocales: null,
                    ct: ct
                ).ConfigureAwait(false);

                seServer.KeepAlive += Check_Alive;

                // 구독 생성 (Core는 캐시만 갱신)
                await CreateSubscriptionsAsync(seServer, ct).ConfigureAwait(false);

                bServerAlive = true;
            }
            finally
            {
                _connectLock.Release();
                bConnecting = false;
            }
        }

        public async Task ServerUninitAsync()
        {
            ThrowIfDisposed();

            await _connectLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await SafeDisposeSessionAsync().ConfigureAwait(false);
                bServerAlive = false;
            }
            finally
            {
                _connectLock.Release();
            }
        }
        internal void OnGroupIntervalChanged(OPCUAMember_group group)
        {
            if (seServer == null || !seServer.Connected)
                return;

            _ = Task.Run(async () =>
            {
                try
                {
                    await group.CreateSubscriptionAsync(
                        seServer,
                        CancellationToken.None
                    ).ConfigureAwait(false);

                    group.ClearDirty();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(
                        ex,
                        "Recreate subscription failed. Group={Group}",
                        group.sName
                    );
                }
            });
        }


        private async Task<EndpointDescription> SelectEndpointAsync(CancellationToken ct)
        {
            try
            {
                var userUri = new Uri(hostName);

                _logger?.LogInformation(
                    "SelectEndpoint: Host={Host}, SecurityPolicy={Policy}, SecurityMode={Mode}",
                    hostName, securityPolicyUri ?? "(auto)", securityMode);

                // ── Step 1: FindServers (btnFindServers_Click 와 동일) ──
                // 서버의 실제 DiscoveryUrl을 얻기 위해 FindServers를 먼저 호출
                string serverDiscoveryUrl = null;
                using (var dc = await DiscoveryClient.CreateAsync(
                    _config, userUri, DiagnosticsMasks.None, ct)
                    .ConfigureAwait(false))
                {
                    var servers = await dc.FindServersAsync(null, ct)
                        .ConfigureAwait(false);

                    if (servers != null && servers.Count > 0)
                    {
                        // 첫 번째 서버의 DiscoveryUrl 사용
                        foreach (var s in servers)
                        {
                            if (s.DiscoveryUrls != null && s.DiscoveryUrls.Count > 0)
                            {
                                serverDiscoveryUrl = s.DiscoveryUrls[0];
                                _logger?.LogInformation(
                                    "FindServers: found server '{Name}', DiscoveryUrl={Url}",
                                    s.ApplicationName?.Text, serverDiscoveryUrl);
                                break;
                            }
                        }
                    }
                }

                // FindServers 결과가 없으면 사용자 입력 URL 그대로 사용
                if (string.IsNullOrEmpty(serverDiscoveryUrl))
                {
                    serverDiscoveryUrl = hostName;
                    _logger?.LogWarning(
                        "FindServers returned no servers. Using user URL: {Url}", hostName);
                }

                // ── Step 2: GetEndpoints (btnGetEndpoints_Click 와 동일) ──
                // 서버가 알려준 DiscoveryUrl로 endpoint 목록을 가져옴
                //이 과정에서 인증서 교환/검증이 발생 → 서버 인증서가 trusted store에 저장됨
                var serverUri = new Uri(serverDiscoveryUrl);
                EndpointDescriptionCollection endpoints;
                using (var dc = await DiscoveryClient.CreateAsync(
                    _config, serverUri, DiagnosticsMasks.None, ct)
                    .ConfigureAwait(false))
                {
                    endpoints = await dc.GetEndpointsAsync(null, ct)
                        .ConfigureAwait(false);
                }

                if (endpoints == null || endpoints.Count == 0)
                {
                    _logger?.LogWarning("No endpoints returned from discovery. Host={Host}", hostName);
                    return null;
                }

                _logger?.LogInformation("Discovery returned {Count} endpoints.", endpoints.Count);

                // endpoint URL 호스트를 사용자가 입력한 호스트로 치환
                // (서버가 반환한 호스트명이 다를 수 있음: e.g. "PSU-DEV" vs "localhost")
                foreach (var ep in endpoints)
                {
                    try
                    {
                        var epUri = new Uri(ep.EndpointUrl);
                        if (!string.Equals(epUri.Host, userUri.Host,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            var builder = new UriBuilder(epUri) { Host = userUri.Host };
                            ep.EndpointUrl = builder.Uri.ToString();
                        }
                    }
                    catch { }
                }

                // ── Step 3: Endpoint 선택 ──
                var requiredTokenType = GetRequiredTokenType();
                EndpointDescription selected = null;

                // 사용자가 지정한 SecurityPolicy/SecurityMode가 있으면 정확히 매칭
                if (!string.IsNullOrEmpty(securityPolicyUri) && securityMode > 0)
                {
                    var targetMode = (MessageSecurityMode)securityMode;

                    selected = endpoints
                        .Where(ep =>
                            ep.SecurityMode == targetMode &&
                            string.Equals(ep.SecurityPolicyUri, securityPolicyUri,
                                StringComparison.OrdinalIgnoreCase) &&
                            SupportsTokenType(ep, requiredTokenType))
                        .OrderByDescending(ep => ep.SecurityLevel)
                        .FirstOrDefault();

                    // 토큰 타입 무시하고 SecurityPolicy/Mode만 매칭 시도
                    if (selected == null)
                    {
                        selected = endpoints
                            .Where(ep =>
                                ep.SecurityMode == targetMode &&
                                string.Equals(ep.SecurityPolicyUri, securityPolicyUri,
                                    StringComparison.OrdinalIgnoreCase))
                            .OrderByDescending(ep => ep.SecurityLevel)
                            .FirstOrDefault();

                        if (selected != null)
                        {
                            _logger?.LogWarning(
                                "Matched endpoint by security only (token type mismatch). {Url} [{Mode}/{Policy}], Tokens=[{Tokens}]",
                                selected.EndpointUrl, selected.SecurityMode, selected.SecurityPolicyUri,
                                string.Join(",", selected.UserIdentityTokens?.Select(t => t.TokenType.ToString()) ?? new string[0]));
                        }
                    }

                    if (selected != null && selected.UserIdentityTokens != null)
                    {
                        _logger?.LogInformation(
                            "Matched saved endpoint: {Url} [{Mode}/{Policy}], Tokens=[{Tokens}]",
                            selected.EndpointUrl, selected.SecurityMode, selected.SecurityPolicyUri,
                            string.Join(",", selected.UserIdentityTokens.Select(t => t.TokenType.ToString())));
                    }
                    else if (selected == null)
                    {
                        _logger?.LogWarning(
                            "No endpoint matched saved security. Mode={Mode}, Policy={Policy}, Token={Token}",
                            targetMode, securityPolicyUri, requiredTokenType);
                    }
                }

                // 매칭 실패 시 UserIdentity 호환 endpoint 중 SecurityLevel 기준 선택
                if (selected == null)
                {
                    selected = endpoints
                        .Where(ep => SupportsTokenType(ep, requiredTokenType))
                        .OrderByDescending(ep => ep.SecurityLevel)
                        .ThenBy(ep => ep.SecurityMode.ToString())
                        .FirstOrDefault();

                    if (selected != null)
                    {
                        _logger?.LogInformation(
                            "Auto-selected endpoint: {Url} [{Mode}/{Policy}] Level={Level}",
                            selected.EndpointUrl, selected.SecurityMode,
                            selected.SecurityPolicyUri, selected.SecurityLevel);
                    }
                    else
                    {
                        // 토큰 필터 없이 최종 fallback
                        selected = endpoints
                            .OrderByDescending(ep => ep.SecurityLevel)
                            .FirstOrDefault();

                        _logger?.LogWarning(
                            "No endpoint supports token {Token}. Fallback: {Url} [{Mode}/{Policy}]",
                            requiredTokenType, selected?.EndpointUrl,
                            selected?.SecurityMode, selected?.SecurityPolicyUri);
                    }
                }

                return selected;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "SelectEndpointAsync failed. Host={Host}", hostName);
                throw;
            }
        }

        /// <summary>현재 UserIdentity에 필요한 UserTokenType 반환</summary>
        private UserTokenType GetRequiredTokenType()
        {
            var identity = _options?.UserIdentity;
            if (identity == null)
                return UserTokenType.Anonymous;

            return identity.TokenType;
        }

        /// <summary>endpoint가 지정된 UserTokenType을 지원하는지 확인</summary>
        private static bool SupportsTokenType(EndpointDescription ep, UserTokenType tokenType)
        {
            if (ep.UserIdentityTokens == null || ep.UserIdentityTokens.Count == 0)
                return tokenType == UserTokenType.Anonymous;

            return ep.UserIdentityTokens.Any(t => t.TokenType == tokenType);
        }

        private void HookCertificateValidationOnce(ApplicationConfiguration config, OpcUaServerOptions options)
        {
            if (_certValidationHooked) return;
            if (config == null) return;

            config.CertificateValidator.CertificateValidation += (s, e) =>
            {
                try
                {
                    // 정상 인증서는 그대로 통과
                    if (e.Error.StatusCode == StatusCodes.Good)
                    {
                        return;
                    }

                    string thumb = e.Certificate?.Thumbprint ?? "N/A";
                    string subject = e.Certificate?.Subject ?? "N/A";

                    _logger?.LogWarning(
                        "Certificate validation error. Status={StatusCode}, Subject={Subject}, Thumbprint={Thumbprint}",
                        e.Error.StatusCode, subject, thumb);

                    // AutoTrustStore=true: 모든 인증서 오류를 자동 수락
                    if (options.AutoTrustStore)
                    {
                        _logger?.LogInformation(
                            "Auto-accepting certificate (AutoTrustStore=true). Status={StatusCode}, Thumbprint={Thumbprint}",
                            e.Error.StatusCode, thumb);

                        e.Accept = true;

                        // Untrusted 인증서는 Trusted 저장소에 저장
                        if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted
                            && e.Certificate != null)
                        {
                            _ = SaveToTrustedStoreAsync(e.Certificate, _logger);
                        }
                        return;
                    }

                    // AutoTrustStore=false: 모든 인증서 오류를 거부
                    _logger?.LogWarning(
                        "Certificate rejected (AutoTrustStore=false). Status={StatusCode}, Thumbprint={Thumbprint}",
                        e.Error.StatusCode, thumb);

                    e.Accept = false;

                    if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted
                        && e.Certificate != null)
                    {
                        _ = SaveToRejectedStoreAsync(e.Certificate, _logger);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "CertificateValidation handler failed.");
                    e.Accept = false;
                }
            };

            _certValidationHooked = true;
        }

        private async Task SaveToRejectedStoreAsync(X509Certificate2 cert, ILogger logger)
        {
            if (cert == null) return;
            try
            {
                var store = _config
                    .SecurityConfiguration
                    .RejectedCertificateStore
                    .OpenStore(OpcUaClientTelemetry.Telemetry);

                // 이미 있으면 중복 저장 방지
                var existing = await store.FindByThumbprintAsync(cert.Thumbprint);
                if (existing == null || existing.Count == 0)
                {
                    await store.AddAsync(cert).ConfigureAwait(false);
                    logger.LogInformation(
                        "Certificate saved to rejected store. Thumbprint={Thumbprint}",
                        cert.Thumbprint);
                }
                store.Close();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save certificate to rejected store.");
            }
        }

        private async Task SaveToTrustedStoreAsync(X509Certificate2 cert, ILogger logger)
        {
            if (cert == null) return;
            try
            {
                var store = _config
                    .SecurityConfiguration
                    .TrustedPeerCertificates
                    .OpenStore(OpcUaClientTelemetry.Telemetry);

                await store.AddAsync(cert).ConfigureAwait(false);
                store.Close();
                logger.LogInformation(
                    "Certificate auto-trusted and stored. Thumbprint={Thumbprint}",
                    cert.Thumbprint);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to store certificate to trusted store.");
                // 저장 실패해도 Accept는 유지 (이미 설정됨)
            }
        }

        private async Task CreateSubscriptionsAsync(ISession session, CancellationToken ct)
        {
            if (arrGroup == null || arrGroup.Count == 0)
                return;

            foreach (var group in arrGroup)
            {
                ct.ThrowIfCancellationRequested();
                if (group == null) continue;

                try
                {
                    await group.InitializeAsync(session, ct);
                }
                catch (Exception ex)
                {
                    // 한 그룹 실패가 서버 전체를 죽이지 않게
                    _logger?.LogError(
                        ex,
                        "Failed to create subscription. Group={Group}",
                        group.sName);
                }
            }
        }

        private async Task SafeDisposeSessionAsync()
        {
            var s = seServer;
            seServer = null;

            if (s == null) return;

            // 구독 정리 (best-effort)
            if (arrGroup != null)
            {
                foreach (var g in arrGroup)
                {
                    if (g == null) continue;

                    try
                    {
                        await g.ClearSubscriptionAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        // best-effort
                    }
                }
            }

            try { s.KeepAlive -= Check_Alive; } catch { }
            try { await s.CloseAsync().ConfigureAwait(false); } catch { }
            try { s.Dispose(); } catch { }
        }

        private void Check_Alive(ISession session, KeepAliveEventArgs e)
        {
            try
            {
                if (e.Status != null && ServiceResult.IsBad(e.Status))
                {
                    if (!bServerAlive || bConnecting) return;

                    _logger?.LogWarning("OPC UA KeepAlive BAD. Status={Status}, CurrentState={State}", e.Status, e.CurrentState);

                    // ✅ Core는 상태만 반영
                    bServerAlive = false;
                    bConnecting = true;
                    return;
                }

                bConnecting = false;
                bServerAlive = true;
            }
            catch
            {
            }
        }


        public string DisplayText
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(accessName);

                if (!string.IsNullOrEmpty(ServerDisplayName))
                    sb.Append(" - ").Append(ServerDisplayName);

                if (!string.IsNullOrEmpty(hostName))
                    sb.Append(" [").Append(hostName).Append("]");

                return sb.ToString();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { ServerUninitAsync().GetAwaiter().GetResult(); } catch { }
            try { _connectLock.Dispose(); } catch { }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(OPCUAMember_server));
        }

        public async Task<StatusCode> WriteValueAsync(
           string nodeId,
           Variant value,
           CancellationToken ct)
        {
            var session = seServer;
            if (session == null || !session.Connected)
                return StatusCodes.BadServerNotConnected;

            try
            {
                var wv = new WriteValue
                {
                    NodeId = new NodeId(nodeId),
                    AttributeId = Attributes.Value,
                    Value = new DataValue(value)
                };

                var col = new WriteValueCollection { wv };

                var res = await session.WriteAsync(
                    null, col, ct).ConfigureAwait(false);

                return res.Results[0];
            }
            catch
            {
                throw;
            }
        }

        private readonly SemaphoreSlim _subscriptionLock
            = new SemaphoreSlim(1, 1);


        public OPCUAMember_group FindGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return null;

            var groups = arrGroup;
            if (groups == null)
                return null;

            for (int i = 0; i < groups.Count; i++)
            {
                var g = groups[i];
                if (g != null &&
                    string.Equals(g.sName, groupName, StringComparison.OrdinalIgnoreCase))
                {
                    return g;
                }
            }

            return null;
        }

    }


}
