using AutoLibLocal;
using LocalMain.OPCUA;
using Microsoft.Extensions.Logging;
using NetTools;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Security.Certificates;
using Opc.Ua.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Opc.Ua.Server.UserDatabase.LinqUserDatabase;

/*
 * 
Telemetry : 프로그램이  "스스로의 상태와 동작을 외부로 관측 가능하게 만드는 체계"

Telemetry 구성요소
    Logs    	사건 기록	인증서 승인, Tag Write 실패
    Traces	흐름 추적	OPC Write → TagLib → Device
    Metrics	수치 계측	초당 Write 수, 연결 수

OPC UA SDK가 telemetry를 도입한 이유
 과거 문제
    Utils.Trace / Utils.LogXXX
    static logger
    전역 singleton
    멀티 서버 / 멀티 클라이언트 혼란

 지금 목표
    서버/클라이언트 동시 실행
    컨테이너 / 클라우드 / Edge
    OpenTelemetry 연동
    DI 가능 구조

Client Telemetry는 서버와 분리해야 한다.

Client에서 ISessionFactory 사용.



인증서 : 프로그램 단위

노드/태그 : 프로젝트 단위
사용자 인증 : 프로젝트 단위
서버목록 : 프로젝트 단위

 * 
 */


namespace LocalMain
{
    public static class OpcUaTelemetry
    {
        public static readonly ITelemetryContext Telemetry =
            DefaultTelemetry.Create(builder =>
            {
                builder.AddDebug();  //OPCUA SDK 내부 로그 확인용
                //builder.AddSerilog(); // Serilog 사용 시(파일 저장)
                //builder.AddConsole();
            });
    }
    /// <summary>
    /// TelemetryContext는 애플리케이션 루트에서 1회 생성
    /// ApplicationInstance 생성 시 반드시 주입
    ///서버/노드 매니저는 생성자 또는 Initialize()로 전달받아 사용
    ///ILogger / Activity / Meter는 반드시 Extension Method 사용
    /// </summary>
    public class OPCUAServerMain : StandardServer
    {
        static public bool bEnabled = true;
        static public bool bSecurity = false;
        static public bool bItemSecurity = false;
        static public bool bNone = true;

        static public bool bAutoTrustStore = true;
        // UserToken 정책 설정 추가
        static public bool bUserTokenAnonymous = true;
        static public bool bUserTokenUserName = false;
        static public bool bUserTokenCertificate = false;

        static public int nPort = 43344;

        public static ApplicationInstance _application;
        public MyNodeManager NodeManager { get; private set; }
        public static bool bLoad = false;
        public static bool bIpv4error = false;
        public static string buf_ip = "";
        public static OPCUAServerMain _server;
        public static ApplicationConfiguration config;

        private static ITelemetryContext _telemetry = null;

        private static System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();

        private static readonly object _userLock = new object();
        private static List<OpcUaUserRecord> _users = new List<OpcUaUserRecord>();

        private static OpcUaServerFileLogger _fileLogger;
        private static volatile bool _loggerAttached = false;

        // ── Auto-Restart Watchdog ──
        private static bool _autoRestart = false;
        private static int _autoRestartIntervalSec = 30;
        private static CancellationTokenSource _watchdogCts;
        private static Task _watchdogTask;

        public static bool AutoRestart => _autoRestart;
        public static int AutoRestartIntervalSec => _autoRestartIntervalSec;

        private static readonly string _userStorePath =
            Path.Combine(
                TotalConfig.sDirWorkProject,
                "OpcData", "Server", "users.json");

        private static readonly string _pkiBasePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "OPCUAServer", "pki");

        public OPCUAServerMain()
        {
            timer1.Interval = 1000;
            timer1.Tick += timer1_tick;
            timer1.Start();
        }

        private bool _isIpChecking = false;    // 260226 PSU, IP 체크 중 재진입 방지

        private async void timer1_tick(object sender, EventArgs e)
        {
            if (bLoad && _server != null && _application != null)
            {
                if (_isIpChecking) return;
                _isIpChecking = true;
                try
                {
                    string temp_ip = await Task.Run(() => GetLocalIPv4Address());
                    bIpv4error = (temp_ip != buf_ip);
                }
                catch { }
                finally
                {
                    _isIpChecking = false;
                }
            }
        }


        public static void LoadConfig()
        {
            OpcUaServerConfigStore.Load();

            nPort = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerPortNumber", nPort);
            bSecurity = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerSecurity", bSecurity);
            bNone = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerNone", bNone);

            buf_ip = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerSetIP", "");

            bEnabled = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "EnableOpcUAServer", false);

            // UserToken 정책 로드
            bUserTokenAnonymous = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenAnonymous", true);
            bUserTokenUserName = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenUserName", false);
            bUserTokenCertificate = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenCertificate", false);

            bAutoTrustStore = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerAutoTrustStore", true);

            // Auto-Restart 설정 로드
            _autoRestart = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcUAServerAutoRestart", false);
            _autoRestartIntervalSec = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcUAServerAutoRestartInterval", 30);
        }

        /// <summary>
        /// Auto-Restart 설정 변경 (FormAutoSetup 에서 호출)
        /// </summary>
        public static void SetAutoRestart(bool enable, int intervalSec)
        {
            _autoRestart = enable;
            _autoRestartIntervalSec = Math.Max(10, intervalSec);

            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcUAServerAutoRestart", _autoRestart);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcUAServerAutoRestartInterval", _autoRestartIntervalSec);

            ApplyRestartTimer();
        }

        /// <summary>
        /// Supervisor watchdog 시작/재시작
        /// </summary>
        private static void ApplyRestartTimer()
        {
            StopRestartTimer();

            if (!_autoRestart || !bEnabled)
                return;

            _watchdogCts = new CancellationTokenSource();
            _watchdogTask = RunWatchdogAsync(_watchdogCts.Token);
        }

        private static void StopRestartTimer()
        {
            if (_watchdogCts != null)
            {
                _watchdogCts.Cancel();
                _watchdogCts.Dispose();
                _watchdogCts = null;
            }
            _watchdogTask = null;
        }

        /// <summary>
        /// Supervisor watchdog loop (async Task, not async void).
        /// 서버가 다운되면 자동으로 재시작 시도.
        /// </summary>
        private static async Task RunWatchdogAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_autoRestartIntervalSec * 1000, token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (!_autoRestart || !bEnabled) continue;
                if (bLoad) continue;

                try
                {
                    OpcUaServerLogBridge.Warn("AutoRestart", "Server stopped. Restarting...");
                    await Init();

                    if (bLoad)
                        OpcUaServerLogBridge.Info("AutoRestart", "Server restarted successfully.");
                    else
                        OpcUaServerLogBridge.Error("AutoRestart", "Restart completed but server failed to start.");
                }
                catch (Exception ex)
                {
                    OpcUaServerLogBridge.Error("AutoRestart",
                        string.Format("Restart failed: {0}", ex.Message));
                }
            }
        }

        private static void LoadUsers()
        {
            lock (_userLock)
            {
                var store = new OpcUaUserStore(_userStorePath);
                _users = store.Load();
            }
        }

        public static async Task Init()
        {

            if (!bEnabled) return;

            // Initialize file logger ONCE — 재시작 시 중복 Attach 방지
            if (!_loggerAttached)
            {
                string logDir = Path.Combine(TotalConfig.GetProjectDataDirectory(), "OpcUaServer");
                _fileLogger = new OpcUaServerFileLogger(logDir);
                OpcUaServerLogBridge.Attach(_fileLogger.WriteLog);
                _loggerAttached = true;
            }

            bItemSecurity = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerItemSecurity", false);
            Debug.WriteLine("OPCUAServer.Init()");

            await Config_init();
            LoadUsers();
            if (_server is null || _application is null) return;

            try
            {
                await _server.StartAsync(config);
                Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION, String.Format("OPC UA Server Run"));
                OpcUaServerLogBridge.Info("Server", string.Format("OPC UA Server started on port {0}", nPort));
            }
            catch (Exception ex)
            {
                bLoad = false;
                OpcUaServerLogBridge.Error("Server", string.Format("OPC UA Server failed to start: {0}", ex.Message));
            }

            // Auto-Restart 타이머 적용
            ApplyRestartTimer();
        }

        public static async Task Uninit(bool releaseLogger = false)
        {
            // 먼저 watchdog 중지 — race condition 방지
            StopRestartTimer();

            if (bLoad && _server != null && _application != null)
            {
                await _server.StopAsync();
                _server.Dispose();
                _server = null;

                _application = null;

                bLoad = false;
                Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION, String.Format("OPC UA Server Stopped"));
                OpcUaServerLogBridge.Info("Server", "OPC UA Server stopped");
            }

            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerPortNumber", nPort);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerNone", bNone);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerSecurity", bSecurity);

            // UserToken 정책 저장
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenAnonymous", bUserTokenAnonymous);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenUserName", bUserTokenUserName);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenCertificate", bUserTokenCertificate);

            timer1.Stop();

            // 프로그램 종료 시에만 logger 완전 해제
            if (releaseLogger && _fileLogger != null)
            {
                OpcUaServerLogBridge.DetachAll();
                _fileLogger.Dispose();
                _fileLogger = null;
                _loggerAttached = false;
            }
        }

        private static async Task Config_init()
        {
            ServerSecurityPolicyCollection sps;

            if (bSecurity && !bNone)
            {
                sps = new ServerSecurityPolicyCollection();

                sps.Add(new ServerSecurityPolicy
                {
                    SecurityMode = MessageSecurityMode.SignAndEncrypt,
                    SecurityPolicyUri = SecurityPolicies.Basic256Sha256
                });
            }
            else if (bSecurity && bNone)
            {
                sps = new ServerSecurityPolicyCollection();

                sps.Add(new ServerSecurityPolicy
                {
                    SecurityMode = MessageSecurityMode.SignAndEncrypt,
                    SecurityPolicyUri = SecurityPolicies.Basic256Sha256
                });

                sps.Add(new ServerSecurityPolicy
                {
                    SecurityMode = MessageSecurityMode.None,
                    SecurityPolicyUri = SecurityPolicies.None
                });
            }
            else //only none
            {
                sps = new ServerSecurityPolicyCollection();
                sps.Add(new ServerSecurityPolicy
                {
                    SecurityMode = MessageSecurityMode.None,
                    SecurityPolicyUri = SecurityPolicies.None
                });
            }

            // 초기화 시 한번에 설정
            var baseAddresses = new StringCollection
            {
                $"opc.tcp://localhost:{nPort}",
                $"opc.tcp://127.0.0.1:{nPort}"
            };

            string localIp = GetLocalIPv4Address();
            if (!string.IsNullOrEmpty(localIp))
            {
                OPCUAServerMain.buf_ip = localIp;
                baseAddresses.Add($"opc.tcp://{localIp}:{nPort}");
            }

            // ★ UserTokenPolicies는 GetEndpoints()에서 Endpoint별로 자동 설정하므로
            //   여기서는 placeholder만 설정 (SDK가 최소 1개를 요구)
            var userTokenPolicies = new UserTokenPolicyCollection
            {
                new UserTokenPolicy(UserTokenType.Anonymous)
            };

            config = new ApplicationConfiguration()
            {
                ApplicationName = "AutobaseOPCUAServer",
                ApplicationType = ApplicationType.Server,
                ServerConfiguration = new ServerConfiguration
                {
                    BaseAddresses = baseAddresses,
                    SecurityPolicies = sps,
                    UserTokenPolicies = userTokenPolicies,  // placeholder, GetEndpoints()에서 덮어씀
                    MinSessionTimeout = 10000,
                    MaxSessionTimeout = 3600000,
                    MaxSessionCount = 100,
                    MaxSubscriptionCount = 500,
                    // LDS(Local Discovery Server, port 4840) 등록 비활성화
                    // 0 = 등록 안 함. LDS가 없는 환경에서 불필요한 SocketException 방지
                    MaxRegistrationInterval = 0,
                },

                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(_pkiBasePath, "own"),
                        SubjectName = $"CN=AutobaseOPCUAServer, DC={System.Net.Dns.GetHostName()}"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(_pkiBasePath, "issuers")
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(_pkiBasePath, "trusted")
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(_pkiBasePath, "rejected")
                    },
                    AutoAcceptUntrustedCertificates = false,
                },
                ApplicationUri = "urn:autobase.opcua:server",
                TransportQuotas = new TransportQuotas(),
                TraceConfiguration = new TraceConfiguration(),
                DisableHiResClock = false
            };

            if (bSecurity)
            {
                config.CertificateValidator.CertificateValidation += CertificateValidationHandler;
            }

            try
            {
                _application = new ApplicationInstance(OpcUaTelemetry.Telemetry)
                {
                    ApplicationName = "AutobaseOPCUAServer",
                    ApplicationType = ApplicationType.Server,
                    ApplicationConfiguration = config
                };

                // pki 인증서 자동 생성
                // ★ None-only + UserName 조합에서도 서버 인증서가 필요 (토큰 암호화용)
                //   따라서 Security 여부와 무관하게 항상 인증서를 생성
                try
                {
                    var _subjectname = config.SecurityConfiguration.ApplicationCertificate.SubjectName;
                    var _SAN = config.ApplicationUri;
                    var domains = GetAllLocalAddresses();
                    var existingCert = await config.SecurityConfiguration.ApplicationCertificate.FindAsync();

                    if (existingCert == null) //인증서가 없는 경우 생성
                    {
                        var storePath = config.SecurityConfiguration.ApplicationCertificate.StorePath;
                        var storeType = config.SecurityConfiguration.ApplicationCertificate.StoreType;

                        var cert = CertificateBuilder
                        .Create(_subjectname)
                        .SetHashAlgorithm(HashAlgorithmName.SHA256)
                        .AddExtension(new X509SubjectAltNameExtension(_SAN, domains))
                        .SetLifeTime(TimeSpan.FromDays(365))
                        .SetRSAKeySize(2048)
                        .CreateForRSA();

                        var store = config.SecurityConfiguration.ApplicationCertificate.OpenStore(OpcUaTelemetry.Telemetry);
                        await store.AddAsync(cert);
                        store.Close();

                        config.SecurityConfiguration.ApplicationCertificate.Certificate = cert;
                        OpcUaServerLogBridge.Info("Certificate", "Server certificate auto-created");
                    }
                    else
                    {
                        config.SecurityConfiguration.ApplicationCertificate.Certificate = existingCert;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Auto Certificate creation error: " + ex.ToString());
                    OpcUaServerLogBridge.Error("Certificate", string.Format("Auto certificate creation error: {0}", ex.Message));
                }

                await config.ValidateAsync(ApplicationType.Server);

                _server = new OPCUAServerMain();
            }
            catch (Exception ex)
            {
                MessageDisplay.Show(ex.ToString());
                OpcUaServerLogBridge.Error("Server", string.Format("Server configuration failed: {0}", ex.Message));
            }
        }

        /// <summary>
        ///  보안 채널이 설정된 후에 호출
        ///  BadSecurityChecksFailed는 보안 채널 설정 단계에서 발생하므로 CertificateValidation보다 먼저 실패
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CertificateValidationHandler(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            var telemetry = OpcUaTelemetry.Telemetry;
            var logger = telemetry.CreateLogger("OPCUA.Certificate");
            try
            {
                // 정상 인증서는 그대로 통과
                if (e.Error.StatusCode == StatusCodes.Good)
                {
                    return;
                }

                // Untrusted 인증서 처리
                if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                {
                    string thumb = (e.Certificate != null) ? e.Certificate.Thumbprint : "";
                    string subject = (e.Certificate != null) ? e.Certificate.Subject : "";

                    logger.LogWarning(
                        "Untrusted client certificate. Thumbprint={Thumbprint}, Subject={Subject}",
                        thumb, subject);
                    OpcUaServerLogBridge.Warn("Certificate", string.Format("Untrusted client certificate. Subject={0}", subject));

                    if (!bAutoTrustStore)
                    {
                        logger.LogWarning(
                            "Certificate rejected (AutoTrustStore=false). Thumbprint={Thumbprint}",
                            thumb);
                        OpcUaServerLogBridge.Warn("Certificate", string.Format("Certificate rejected (AutoTrustStore=false). Thumbprint={0}", thumb));

                        // ★ 동기 호출로 변경 - CertificateValidation은 동기 이벤트
                        SaveToRejectedStoreSync(e.Certificate, logger);

                        e.Accept = false;
                        return;
                    }

                    // AutoTrustStore: Trusted 저장소에 저장
                    e.Accept = true;
                    SaveToTrustedStoreSync(e.Certificate, logger);
                    OpcUaServerLogBridge.Info("Certificate", string.Format("Certificate auto-trusted. Subject={0}", subject));
                    return;
                }

                if (e.Error.StatusCode == StatusCodes.BadCertificateUseNotAllowed)
                {
                    logger.LogWarning(
                        "CertificateUseNotAllowed. Subject={Subject}, Thumbprint={Thumbprint}",
                        e.Certificate?.Subject ?? "N/A",
                        e.Certificate?.Thumbprint ?? "N/A");

                    // EKU 확인
                    foreach (var ext in e.Certificate.Extensions)
                    {
                        if (ext is X509EnhancedKeyUsageExtension eku)
                            foreach (var oid in eku.EnhancedKeyUsages)
                                logger.LogWarning("  EKU: {Name} ({Oid})", oid.FriendlyName, oid.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Certificate validation handler failed");
                OpcUaServerLogBridge.Error("Certificate", string.Format("Certificate validation failed: {0}", ex.Message));
                e.Accept = false;
            }
        }

        /// <summary>
        /// 동기 방식으로 Rejected 저장소에 인증서 저장
        /// </summary>
        private static void SaveToRejectedStoreSync(X509Certificate2 cert, ILogger logger)
        {
            if (cert == null) return;
            try
            {
                var store = config
                    .SecurityConfiguration
                    .RejectedCertificateStore
                    .OpenStore(OpcUaTelemetry.Telemetry);

                var existing = store.FindByThumbprintAsync(cert.Thumbprint)
                    .GetAwaiter().GetResult();

                if (existing == null || existing.Count == 0)
                {
                    store.AddAsync(cert).GetAwaiter().GetResult();
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

        /// <summary>
        /// 동기 방식으로 Trusted 저장소에 인증서 저장
        /// </summary>
        private static void SaveToTrustedStoreSync(X509Certificate2 cert, ILogger logger)
        {
            if (cert == null) return;
            try
            {
                var store = config
                    .SecurityConfiguration
                    .TrustedPeerCertificates
                    .OpenStore(OpcUaTelemetry.Telemetry);

                store.AddAsync(cert).GetAwaiter().GetResult();
                store.Close();

                logger.LogInformation(
                    "Certificate auto-trusted and stored. Thumbprint={Thumbprint}",
                    cert.Thumbprint);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to store certificate to trusted store.");
            }
        }


        //인증서 SAN 용.
        public static List<string> GetAllLocalAddresses()
        {
            var addresses = new HashSet<string>
            {
                "localhost",
                "127.0.0.1",
                System.Net.Dns.GetHostName() //컴퓨터이름
            };

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                foreach (var addr in ni.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        addresses.Add(addr.Address.ToString());
                    }
                }
            }

            return addresses.ToList();
        }

        public static string GetLocalIPv4Address()
        {
            var list = NetworkInterface.GetAllNetworkInterfaces();
            string buf = "";

            if (list.Length > 0)
            {
                foreach (var ni in list)
                {
                    if (ni.OperationalStatus != OperationalStatus.Up)
                        continue;

                    var ipProps = ni.GetIPProperties();
                    foreach (var addr in ipProps.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // Exclude loopback addresses
                            if (!IPAddress.IsLoopback(addr.Address))
                                return addr.Address.ToString();
                        }
                        else
                        {
                            if (!IPAddress.IsLoopback(addr.Address))
                            {
                                if (buf_ip == addr.Address.ToString())
                                {
                                    return buf_ip;
                                }
                                else
                                {
                                    buf = addr.Address.ToString();
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            return buf;
        }


        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            NodeManager = new MyNodeManager(server, configuration);
            return new MasterNodeManager(server, configuration, null, NodeManager);
        }

        /// <summary>
        /// ★ Endpoint별 UserTokenPolicy를 채널 보안 수준에 따라 자동 설정
        /// OPC UA 서버의 기본 구현은 서버 설정 파일의 호스트명을 고정적으로 사용하기 때문에, 
        /// 다양한 네트워크 주소(IP, localhost, 호스트명)로 접속하는 클라이언트를 지원하려면 서버 측에서 GetEndpoints를 오버라이드하여 
        /// 요청 URL의 호스트를 EndpointUrl에 반영해야 합니다.
        /// 
        /// 규칙:
        ///   채널이 SignAndEncrypt → 토큰 SecurityPolicyUri = None (채널이 이미 보호)
        ///   채널이 None          → 토큰 SecurityPolicyUri = Basic256Sha256 (토큰 자체 보호)
        ///   Anonymous            → 항상 SecurityPolicyUri 없음 (보호할 데이터 없음)
        /// </summary>
        public override EndpointDescriptionCollection GetEndpoints()
        {
            var endpoints = base.GetEndpoints();

            foreach (var ep in endpoints)
            {
                bool isChannelSecure =
                    ep.SecurityMode == MessageSecurityMode.SignAndEncrypt ||
                    ep.SecurityMode == MessageSecurityMode.Sign;

                var policies = new UserTokenPolicyCollection();

                if (bUserTokenAnonymous)
                {
                    policies.Add(new UserTokenPolicy(UserTokenType.Anonymous)
                    {
                        PolicyId = "anon"
                    });
                }

                if (bUserTokenUserName)
                {
                    policies.Add(new UserTokenPolicy(UserTokenType.UserName)
                    {
                        PolicyId = isChannelSecure ? "user_secure" : "user_none",
                        SecurityPolicyUri = isChannelSecure
                            ? SecurityPolicies.None
                            : SecurityPolicies.Basic256Sha256
                    });
                }

                if (bUserTokenCertificate)
                {
                    policies.Add(new UserTokenPolicy(UserTokenType.Certificate)
                    {
                        PolicyId = isChannelSecure ? "cert_secure" : "cert_none",
                        SecurityPolicyUri = isChannelSecure
                            ? SecurityPolicies.None
                            : SecurityPolicies.Basic256Sha256
                    });
                }

                if (policies.Count == 0)
                {
                    policies.Add(new UserTokenPolicy(UserTokenType.Anonymous)
                    {
                        PolicyId = "anon_fallback"
                    });
                }

                ep.UserIdentityTokens = policies;
            }

            return endpoints;
        }

        public static async Task ImportServerCertificateAsync(
            X509Certificate2 cert)
        {
            var appCert = config.SecurityConfiguration.ApplicationCertificate;

            var old = await appCert.FindAsync();
            if (old != null)
            {
                using (var store = appCert.OpenStore(OpcUaTelemetry.Telemetry))
                    await store.DeleteAsync(old.Thumbprint);
            }

            using (var store = appCert.OpenStore(OpcUaTelemetry.Telemetry))
                await store.AddAsync(cert);

            appCert.Certificate = cert;
        }

        public static void ReloadUsers(List<OpcUaUserRecord> users)
        {
            lock (_userLock)
            {
                _users = users != null
                    ? users.ToList()
                    : new List<OpcUaUserRecord>();
            }
        }

        // ★ ActivateSessionAsync 오버라이드 제거
        //   - 토큰 타입 검증은 OnImpersonateUser에서 처리 (복호화 후 단계)
        //   - 중복 검증 불필요, base 구현으로 충분

        /// <summary>
        /// 커스텀 SessionManager 생성
        /// </summary>
        protected override ISessionManager CreateSessionManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            return new CustomSessionManager(server, configuration);
        }

        /// <summary>
        /// 커스텀 SessionManager 클래스
        /// </summary>
        public class CustomSessionManager : SessionManager
        {
            public CustomSessionManager(IServerInternal server, ApplicationConfiguration configuration)
                : base(server, configuration)
            {
                // ImpersonateUser 이벤트 등록 (실제 사용자 검증)
                this.ImpersonateUser += new ImpersonateEventHandler(OnImpersonateUser);
            }

            /// <summary>
            /// throw 는 클라이언트에게 OPC UA 에러 응답으로 전달.
            /// </summary>
            private void OnImpersonateUser(ISession session, ImpersonateEventArgs args)
            {
                var telemetry = OpcUaTelemetry.Telemetry;
                var logger = telemetry.CreateLogger("OPCUA.Authentication");

                var userToken = args.NewIdentity;

                if (userToken == null)
                {
                    logger.LogWarning("User identity token is null");
                    throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                        "Invalid user identity");
                }

                // Anonymous 접근 처리
                if (userToken is AnonymousIdentityToken)
                {
                    if (!OPCUAServerMain.bUserTokenAnonymous)
                    {
                        logger.LogWarning("Anonymous login denied (policy disabled)");
                        OpcUaServerLogBridge.Warn("Auth", "Anonymous login denied (policy disabled)");
                        throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                            "Anonymous access is not allowed");
                    }

                    logger.LogInformation("Anonymous login completed for session: {SessionId}", session.Id);
                    OpcUaServerLogBridge.Info("Auth", string.Format("Anonymous login. SessionId={0}", session.Id));
                    return;
                }

                // Username/Password 인증
                if (userToken is UserNameIdentityToken usernameToken)
                {
                    if (!OPCUAServerMain.bUserTokenUserName)
                    {
                        logger.LogWarning("Username/Password login denied (policy disabled)");
                        OpcUaServerLogBridge.Warn("Auth", "Username/Password login denied (policy disabled)");
                        throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                            "Username/Password authentication is not allowed");
                    }

                    string username = usernameToken.UserName;

                    string password = string.Empty;
                    if (usernameToken.DecryptedPassword != null && usernameToken.DecryptedPassword.Length > 0)
                    {
                        password = Encoding.UTF8.GetString(usernameToken.DecryptedPassword);
                    }

                    var user = OPCUAServerMain.ValidateUser(username, password, logger);

                    // Permission → Role 매핑
                    Role mappedRole = MapPermissionsToRole(user.Permissions);

                    logger.LogInformation("User authenticated successfully: {UserName}, Role: {Role}, Permissions: {Permissions}",
                        username, mappedRole.Name, user.Permissions);
                    OpcUaServerLogBridge.Info("Auth", string.Format("User authenticated: {0}, Role: {1}", username, mappedRole.Name));

                    args.Identity = new RoleBasedIdentity(
                        new UserIdentity(new UserNameIdentityToken
                        {
                            UserName = username,
                            PolicyId = usernameToken.PolicyId
                        }),
                        new List<Role> { mappedRole });

                    return;
                }

                // 인증서 기반 인증
                if (userToken is X509IdentityToken certToken)
                {
                    if (!OPCUAServerMain.bUserTokenCertificate)
                    {
                        logger.LogWarning("Certificate-based login denied (policy disabled)");
                        OpcUaServerLogBridge.Warn("Auth", "Certificate login denied (policy disabled)");
                        throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                            "Certificate authentication is not allowed");
                    }

                    if (certToken.CertificateData == null || certToken.CertificateData.Length == 0)
                    {
                        logger.LogWarning("Certificate data is empty");
                        OpcUaServerLogBridge.Warn("Auth", "Certificate login failed: empty certificate data");
                        throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                            "Invalid certificate");
                    }

                    X509Certificate2 userCert = null;
                    try
                    {
                        userCert = new X509Certificate2(certToken.CertificateData);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to parse certificate");
                        throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                            "Invalid certificate format");
                    }

                    // Thumbprint 기반 사용자 검증 + 사용자 레코드 획득
                    var user = OPCUAServerMain.ValidateCertificateUser(userCert, logger, out string description);

                    // Permission → Role 매핑
                    Role mappedRole = MapPermissionsToRole(user.Permissions);

                    logger.LogInformation(
                        "Certificate-based authentication successful: {UserName}, Subject={Subject}, Desc={Desc}, Role={Role}",
                        user.UserName, userCert.Subject, description, mappedRole.Name);
                    OpcUaServerLogBridge.Info("Auth", string.Format("Certificate auth: {0}, Subject={1}, Role={2}", user.UserName, userCert.Subject, mappedRole.Name));

                    // RoleBasedIdentity 구성
                    args.Identity = new RoleBasedIdentity(
                        new UserIdentity(certToken),
                        new List<Role> { mappedRole });

                    return;
                }

                logger.LogWarning("Unsupported identity token type: {Type}", userToken.GetType().Name);
                OpcUaServerLogBridge.Warn("Auth", string.Format("Unsupported identity token type: {0}", userToken.GetType().Name));
                throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                    "Unsupported authentication method");
            }

            private static string ExtractUsernameFromCertificate(X509Certificate2 cert)
            {
                string subject = cert.Subject;
                var parts = subject.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    var trimmed = part.Trim();
                    if (trimmed.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                    {
                        return trimmed.Substring(3).Trim();
                    }
                }

                return string.Empty;
            }

            private static Role MapPermissionsToRole(OpcUaUserPermissions p)
            {
                if (p.HasFlag(OpcUaUserPermissions.Write))
                {
                    return Role.Operator;
                }

                if (p.HasFlag(OpcUaUserPermissions.Read))
                {
                    return Role.Observer;
                }

                if (p.HasFlag(OpcUaUserPermissions.Browse))
                {
                    return Role.AuthenticatedUser;
                }

                return Role.Anonymous;
            }
        }

        /// <summary>
        /// 사용자 검증 (Username/Password)
        /// </summary>
        public static OpcUaUserRecord ValidateUser(string username, string password, ILogger logger)
        {
            lock (_userLock)
            {
                var user = _users.FirstOrDefault(u =>
                    string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    logger.LogWarning("User not found: {UserName}", username);
                    OpcUaServerLogBridge.Warn("Auth", string.Format("Login failed: User not found: {0}", username));
                    throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                        "User not found");
                }

                if (!user.IsEnabled)
                {
                    logger.LogWarning("User disabled: {UserName}", username);
                    OpcUaServerLogBridge.Warn("Auth", string.Format("Login failed: User disabled: {0}", username));
                    throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                        "User account is disabled");
                }

                if (!OpcUaUserStore.VerifyPassword(user, password))
                {
                    logger.LogWarning("Invalid password for user: {UserName}", username);
                    OpcUaServerLogBridge.Warn("Auth", string.Format("Login failed: Invalid password for user: {0}", username));
                    throw new ServiceResultException(StatusCodes.BadUserAccessDenied,
                        "Invalid password");
                }

                logger.LogInformation("User authenticated: {UserName}, Permissions: {Permissions}",
                    username, user.Permissions);

                return user;
            }
        }

        /// <summary>
        /// 사용자 검증 (Certificate)
        /// </summary>
        public static OpcUaUserRecord ValidateCertificateUser(
            X509Certificate2 cert,
            ILogger logger,
            out string description)
        {
            if (cert == null)
                throw new ServiceResultException(StatusCodes.BadUserAccessDenied, "Invalid certificate");

            OpcUaUserRecord user;

            lock (_userLock)
            {
                // 1. Thumbprint로 사용자 매칭 (정답 루트)
                user = OpcUaUserRecord.FindByCertificateThumbprint(
                      _users,
                      cert.Thumbprint);

                // 2) Fallback: CN → UserName 매칭 (선택)
                if (user == null)
                {
                    string cn = ExtractUsernameFromCertificate(cert);
                    if (!string.IsNullOrWhiteSpace(cn))
                    {
                        user = _users.FirstOrDefault(u =>
                            string.Equals(u.UserName, cn, StringComparison.OrdinalIgnoreCase));
                    }
                }

                if (user == null || !user.IsEnabled)
                    throw new ServiceResultException(StatusCodes.BadUserAccessDenied);

                description = user.Description ?? string.Empty;
                return user;
            }
        }

        private static string ExtractUsernameFromCertificate(X509Certificate2 cert)
        {
            if (cert == null)
                return null;

            // 1) Subject Alternative Name - UPN (권장)
            string upn = GetUpnFromCertificate(cert);
            if (!string.IsNullOrWhiteSpace(upn))
                return upn;

            // 2) Subject Alternative Name - DNS (선택)
            string dns = GetDnsFromCertificate(cert);
            if (!string.IsNullOrWhiteSpace(dns))
                return dns;

            // 3) Subject CN (Fallback)
            string cn = GetCommonName(cert.Subject);
            if (!string.IsNullOrWhiteSpace(cn))
                return cn;

            return null;
        }

        private static string GetUpnFromCertificate(X509Certificate2 cert)
        {
            foreach (var ext in cert.Extensions)
            {
                if (ext.Oid == null || ext.Oid.Value != "2.5.29.17") // SubjectAltName
                    continue;

                var asnData = new AsnEncodedData(ext.Oid, ext.RawData);
                string formatted = asnData.Format(true);

                // 예: "Other Name: Principal Name=operator1"
                var lines = formatted.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var idx = line.IndexOf("Principal Name=", StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        return line.Substring(idx + "Principal Name=".Length).Trim();
                    }
                }
            }
            return null;
        }

        private static string GetDnsFromCertificate(X509Certificate2 cert)
        {
            foreach (var ext in cert.Extensions)
            {
                if (ext.Oid == null || ext.Oid.Value != "2.5.29.17")
                    continue;

                var asnData = new AsnEncodedData(ext.Oid, ext.RawData);
                string formatted = asnData.Format(true);

                // 예: "DNS Name=operator1"
                var lines = formatted.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var idx = line.IndexOf("DNS Name=", StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        return line.Substring(idx + "DNS Name=".Length).Trim();
                    }
                }
            }
            return null;
        }

        private static string GetCommonName(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
                return null;

            var parts = subject.Split(',');
            foreach (var part in parts)
            {
                var p = part.Trim();
                if (p.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                    return p.Substring(3).Trim();
            }
            return null;
        }
    }
}