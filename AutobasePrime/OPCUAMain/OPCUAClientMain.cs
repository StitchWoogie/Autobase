using AutoLibLocal;
using Microsoft.Extensions.Logging;
using NetTools;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Opc.Ua.Security.Certificates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace OPCUAMain
{
    public static class OpcUaClientTelemetry
    {
        public static readonly ITelemetryContext Telemetry =
            DefaultTelemetry.Create(builder =>
            {
                builder.AddDebug(); 
              // builder.AddConsole(); // 콘솔 앱일 때만
            });
    }
    public class OPCUAClientMain
    {
        public static readonly List<OPCUAMember_server> Servers = new List<OPCUAMember_server>();
        public static bool bLoad = false;

        static private System.Windows.Forms.Timer _timerReconnect ;
        private static readonly SemaphoreSlim _tickLock = new SemaphoreSlim(1, 1);
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static readonly ITelemetryContext _telemetry =
            OpcUaClientTelemetry.Telemetry;

        private static readonly ILogger _logger =
            _telemetry.CreateLogger<OPCUAClientMain>();

        // UI thread marshal (Init 시점이 UI thread라면 캡처)
        private static SynchronizationContext _uiContext;

        public static Type ToSystemType(BuiltInType btype)
        {
            switch (btype)
            {
                case BuiltInType.Boolean: return typeof(bool);
                case BuiltInType.SByte: return typeof(sbyte);
                case BuiltInType.Byte: return typeof(byte);
                case BuiltInType.Int16: return typeof(short);
                case BuiltInType.UInt16: return typeof(ushort);
                case BuiltInType.Int32: return typeof(int);
                case BuiltInType.UInt32: return typeof(uint);
                case BuiltInType.Int64: return typeof(long);
                case BuiltInType.UInt64: return typeof(ulong);
                case BuiltInType.Float: return typeof(float);
                case BuiltInType.Double: return typeof(double);
                case BuiltInType.String: return typeof(string);
                case BuiltInType.DateTime: return typeof(DateTime);
                case BuiltInType.Guid: return typeof(Guid);
                case BuiltInType.ByteString: return typeof(byte[]);
                case BuiltInType.XmlElement: return typeof(System.Xml.XmlElement);
                case BuiltInType.NodeId: return typeof(NodeId);
                case BuiltInType.ExpandedNodeId: return typeof(ExpandedNodeId);
                case BuiltInType.StatusCode: return typeof(StatusCode);
                case BuiltInType.QualifiedName: return typeof(QualifiedName);
                case BuiltInType.LocalizedText: return typeof(LocalizedText);
                case BuiltInType.ExtensionObject: return typeof(ExtensionObject);
                case BuiltInType.DataValue: return typeof(DataValue);
                case BuiltInType.Variant: return typeof(Variant);
                case BuiltInType.DiagnosticInfo: return typeof(DiagnosticInfo);
                default: return typeof(object);
            }
        }

        public static void Init()
        {
            // WinForms UI thread에서 호출된다고 가정
            _uiContext = SynchronizationContext.Current;

            _logger.LogInformation("OPC UA Client initializing");

            opcReadWriteFileAccessClass.loadOpcReadWriteConfigData();

            try
            {
                _timerReconnect = new System.Windows.Forms.Timer();
                _timerReconnect.Tick += timer_reconnect_tick;
                _timerReconnect.Interval = 30000; //default : 30 sec
                _timerReconnect.Start();
            }
            catch
            {
                bLoad = false;
            }
        }

        public static void Uninit()
        {
            Debug.WriteLine("OPCUAClient.UnInit()");
            opcReadWriteFileAccessClass.saveOpcReadWriteConfigData();

            try { _cts.Cancel(); } catch { }
            bLoad = false;

            if (_timerReconnect != null)
            {
                try
                {
                    _timerReconnect.Stop();
                    _timerReconnect.Tick -= timer_reconnect_tick;
                }
                catch { }
                _timerReconnect = null;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await CleanupPreviousSessionAsync().ConfigureAwait(false);
                }
                catch { }
            });
        }

        static private async void timer_reconnect_tick(object sender, EventArgs e)
        {

            // Timer는 UI thread에서 실행 → 블로킹/중첩 방지
            if (!await _tickLock.WaitAsync(0).ConfigureAwait(false))
                return;

            try
            {
                if (!bLoad)
                {
                    _ = Task.Run(() => ConnectAllAsync(_cts.Token));
                    bLoad = true;
                    return;
                }

                bool enabled = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcClientReconnect", false);
                if (!enabled) return;

                if (Servers.Count == 0) return;

                foreach (var s in Servers)
                {
                    if (s == null) continue;
                    if (s.bServerAlive) continue;

                    Log.Write(AutoLibLocal.LogLevel.ERROR, LogCategory.COMMUNICATION,
                        string.Format("Try to reconnect OPC UA Client : {0}", s.accessName));

                    _ = Task.Run(async () =>
                    {
                        try { await s.ServerInitAsync(_cts.Token, _uiContext).ConfigureAwait(false); }
                        catch { }
                    });
                }
            }
            finally
            {
                _tickLock.Release();
            }
      
        }

        private static async Task ConnectAllAsync(CancellationToken ct)
        {
            if (Servers.Count == 0) return;

            foreach (var s in Servers)
            {
                if (ct.IsCancellationRequested) break;
                if (s == null) continue;

                try
                {
                    await s.ServerInitAsync(ct, _uiContext).ConfigureAwait(false);
                }
                catch
                {
                    // 개별 서버 실패는 전체를 막지 않음
                }
            }
        }

        private static async Task CleanupPreviousSessionAsync()
        {
            foreach (var s in Servers)
            {
                if (s == null) continue;
                try { await s.ServerUninitAsync().ConfigureAwait(false); }
                catch { }
            }
        }
    }


    public class OPCUAMember_server  
    {
        public string serverName;
        public string hostName;
        public string accessName;
        public int endpointindex;

        // 최신 ISession 기반 권장. 기존 Session 타입이 필요하면 캐스팅 가능.
        public ISession seServer;
        private ISessionFactory _sessionFactory;
        public List<OPCUAMember_group> arrGroup;
        private readonly ITelemetryContext _telemetry;
        private readonly ILogger _logger;

        private bool _bServerAlive = false;
        public bool bServerAlive 
        {
            get => _bServerAlive;
            set 
            {
                if (_bServerAlive == value) return;
                _bServerAlive = value;

                    if (_bServerAlive)
                        Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION, String.Format("OPC UA Client Started : {0}", this.accessName));
                    else
                    {
                        Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION, String.Format("OPC UA Client Ended : {0}", this.accessName));
                    }
                
            }
        }

        private readonly SemaphoreSlim _connectLock = new SemaphoreSlim(1, 1);


        public OPCUAMember_server( string _server, string _host, string _access, int _index)
        {
            this.serverName = _server;
            this.hostName = _host;
            this.accessName = _access;
            this.endpointindex = _index;
            this.arrGroup = new List<OPCUAMember_group>();

            _telemetry = OpcUaClientTelemetry.Telemetry;
            _logger = _telemetry.CreateLogger<OPCUAMember_server>();
        }

        public async Task ServerInitAsync(CancellationToken ct, SynchronizationContext uiContext)
        {
            if (bServerAlive) return;

            using (var activity = _telemetry.StartActivity("OPCUA.Client.Reconnect"))
            {
                _logger.LogWarning("Reconnect failed. Server={Server}", accessName);
            }

            _sessionFactory = new DefaultSessionFactory(_telemetry);

            await _connectLock.WaitAsync(ct).ConfigureAwait(false);

            try {
                if (bServerAlive) return; // 락 획득 후 재확인


                // 기존 세션 정리
                await SafeDisposeSessionAsync().ConfigureAwait(false);

                // config 생성
                var config = BuildClientConfiguration();
                await config.ValidateAsync(ApplicationType.Client).ConfigureAwait(false);


                // 인증서 자동 수락 보강(권장)
                config.CertificateValidator.CertificateValidation += (s, e) =>
                {
                    //if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                    //    e.Accept = true;

                    if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                    {
                        _logger.LogWarning(
                            "Untrusted server certificate accepted. Thumbprint={Thumbprint}",
                            e.Certificate.Thumbprint);

                        e.Accept = true;
                    }
                };

                OPCUAMember_server temp_server = this;

                // Endpoint 선택 (최신 SelectEndpointAsync - discoveryUrl 기반)
                EndpointDescription selectedEndpoint = null;

                try
                {
                    selectedEndpoint = await CoreClientUtils.SelectEndpointAsync(
                        application: config,
                        discoveryUrl: hostName,
                        useSecurity: false,
                        discoverTimeout: 15000,
                        telemetry: _telemetry,
                        ct: ct
                    ).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION,
                       $"SelectEndpointAsync failed for {hostName}: {ex.Message}");

                    _logger.LogError(ex,
                  "SelectEndpointAsync failed. Host={Host}",
                  hostName);

                }

                var endpointConfig = EndpointConfiguration.Create(config);
                var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfig);

                if (selectedEndpoint == null)
                    throw new Exception("No suitable OPC UA endpoint found.");

                // ISessionFactory를 사용한 세션 생성
                seServer = await _sessionFactory.CreateAsync(
                    configuration: config,
                    endpoint: endpoint,
                    updateBeforeConnect: false,
                    checkDomain: false,
                    sessionName: config.ApplicationName,
                    sessionTimeout: 60000,
                    identity: new UserIdentity(new AnonymousIdentityToken()),
                    preferredLocales: null,
                    ct: CancellationToken.None
                );


                // KeepAlive는 상태표시만, 재접속은 Timer가 담당
                seServer.KeepAlive += Check_Alive;


                // 구독 생성
                await CreateSubscriptionsAsync(seServer, ct, uiContext).ConfigureAwait(false);

                bServerAlive = true;
            }
            catch (Exception ex)
            {
                bServerAlive = false;

                // 예외는 외부로 던져도 되고, 여기서 UI 메시지로 처리해도 됨
                // SCADA 운영 특성상 메시지 박스가 시스템을 멈추는 경우가 있어
                // 최소한의 출력만 권장
                try
                {
                    MessageDisplay.Show("OPC UA Client Error", ex.Message);
                }
                catch { }

                throw;
            }
            finally
            {
                _connectLock.Release();
            }
        }

        private async Task CreateSubscriptionsAsync(ISession session, CancellationToken ct, SynchronizationContext uiContext)
        {
            if (arrGroup == null || arrGroup.Count == 0) return;

            for (int j = 0; j < arrGroup.Count; j++)
            {
                ct.ThrowIfCancellationRequested();

                var group = arrGroup[j];
                if (group == null) continue;

                group.sbSubscription = new Subscription(session.DefaultSubscription)
                {
                    PublishingInterval = group.nInterval
                };

                if (group.arrItem != null && group.arrItem.Count > 0)
                {
                    for (int k = 0; k < group.arrItem.Count; k++)
                    {
                        var item = group.arrItem[k];
                        if (item == null) continue;

                        item.nGroupindex = j;
                        item.nItemindex = k;

                        var monitoredItem = new MonitoredItem(group.sbSubscription.DefaultItem)
                        {
                            StartNodeId = new NodeId(item.sNode),
                            AttributeId = Attributes.Value,
                            DisplayName = item.sName,
                        };

                        // Notification은 UA 스레드에서 옴 → UI thread marshal 필요
                        monitoredItem.Notification += (m, a) =>
                        {
                            try
                            {
                                var n = a.NotificationValue as MonitoredItemNotification;
                                if (n == null) return;

                                var dv = n.Value;

                                // 값 문자열화
                                string display = ToDisplayString(item, dv);

                                // UI 적용 (PropertyChanged / DataGrid 바인딩 등)
                                void Apply()
                                {
                                    if (dv.Value != null)
                                    {
                                        if (string.IsNullOrEmpty(item.DataType) ||
                                            item.DataType == "Bad" ||
                                            item.DataType == "Not Found")
                                        {
                                            item.DataType = dv.WrappedValue.TypeInfo.BuiltInType.ToString();

                                            // 배열/매트릭스 차원 정보 덧붙이기
                                            var suffix = GetArraySuffix(item);
                                            if (suffix.Length > 0)
                                                item.DataType += suffix;
                                        }

                                        if (item.tType == null && dv.Value != null)
                                            item.tType = dv.Value.GetType();

                                        item.sValue = display;
                                    }
                                    else
                                    {
                                        item.DataType = "Bad";
                                        item.sValue = "???";
                                    }
                                }

                                if (uiContext != null)
                                    uiContext.Post(_ => Apply(), null);
                                else
                                    Apply();
                            }
                            catch
                            {
                            }
                        };

                        item.moItem = monitoredItem;
                        group.sbSubscription.AddItem(monitoredItem);
                    }
                }

                session.AddSubscription(group.sbSubscription);
                await group.sbSubscription.CreateAsync().ConfigureAwait(false);
            }
        }
        private static string ToDisplayString(OPCUAMember_item item, DataValue dv)
        {
            if (dv == null) return "null";

            var v = dv.Value;
            if (v == null) return "null";

            // Matrix
            var mx = v as Matrix;
            if (mx != null)
            {
                item.bIsMatrix = true;
                item.bIsArray = false;
                item.dvDataValue = dv;

                var elements = mx.Elements;
                if (elements == null || elements.Length == 0) return string.Empty;

                // .NET 4.8 / C#7.3: string.Join에 object[] 직접 사용 가능
                var parts = new string[elements.Length];
                for (int i = 0; i < elements.Length; i++)
                    parts[i] = elements.GetValue(i)?.ToString() ?? string.Empty;

                return string.Join(",", parts);
            }

            // Array
            var arr = v as Array;
            if (arr != null)
            {
                item.bIsArray = true;
                item.bIsMatrix = false;
                item.dvDataValue = dv;

                var parts = new string[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                    parts[i] = arr.GetValue(i)?.ToString() ?? string.Empty;

                return string.Join(",", parts);
            }

            // scalar
            item.bIsArray = false;
            item.bIsMatrix = false;
            item.dvDataValue = dv;
            return v.ToString();
        }

        private static string GetArraySuffix(OPCUAMember_item item)
        {
            if (item == null) return string.Empty;
            if (item.dvDataValue == null) return string.Empty;
            if (item.dvDataValue.Value == null) return string.Empty;

            if (item.bIsMatrix)
            {
                var mx = item.dvDataValue.Value as Matrix;
                if (mx != null && mx.Dimensions != null)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append(" Array");
                    for (int i = 0; i < mx.Dimensions.Length; i++)
                        sb.AppendFormat("[{0}]", mx.Dimensions[i]);
                    return sb.ToString();
                }
            }

            if (item.bIsArray)
            {
                var arr = item.dvDataValue.Value as Array;
                if (arr != null)
                    return " Array[" + arr.Length + "]";
            }

            return string.Empty;
        }

        private ApplicationConfiguration BuildClientConfiguration()
        {
            // 디렉터리 보장(상대경로는 실행경로 기준)
            Directory.CreateDirectory("pki/own");
            Directory.CreateDirectory("pki/trusted");
            Directory.CreateDirectory("pki/rejected");
            Directory.CreateDirectory("pki/issuers");

            return new ApplicationConfiguration
            {
                ApplicationName = "LocalMain",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = "pki/own",
                        SubjectName = "CN=LocalMainOpcUaClient"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "pki/issuers"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "pki/trusted"
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "pki/rejected"
                    },
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            };
        }


        public async Task ServerUninitAsync()
        {
            await _connectLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _logger.LogInformation(
                "Disconnecting OPC UA Client. Server={Server}",
                accessName);

                await SafeDisposeSessionAsync().ConfigureAwait(false);
                bServerAlive = false;
            }
            finally
            {
                _connectLock.Release();
            }
        }

        private async Task SafeDisposeSessionAsync()
        {
            var s = seServer;
            seServer = null;

            if (s == null) return;

            // 구독 정리 (베스트 에포트)
            if (arrGroup != null && arrGroup.Count > 0)
            {
                foreach (var g in arrGroup)
                {
                    if (g == null) continue;
                    var sub = g.sbSubscription;
                    g.sbSubscription = null;

                    if (sub != null)
                    {
                        try { await sub.DeleteItemsAsync().ConfigureAwait(false); } catch { }
                        try { await sub.DeleteAsync(true).ConfigureAwait(false); } catch { }
                        try { sub.Dispose(); } catch { }
                    }
                }
            }

            try { s.KeepAlive -= Check_Alive; } catch { }

            try
            {
                // CloseAsync는 ISession에 존재
                await s.CloseAsync().ConfigureAwait(false);
            }
            catch { }

            try { s.Dispose(); } catch { }
        }
        private void Check_Alive(ISession session, KeepAliveEventArgs e)
        {
            if (ServiceResult.IsBad(e.Status))
            {
                _logger.LogWarning(
            "OPC UA KeepAlive BAD. Status={Status}",
            e.Status);

                bServerAlive = false;
            }
            else
            {
                bServerAlive = true;
            }
        }

    }

    public class OPCUAMember_group
    {
        public string sName;
        public int nInterval;
        public Subscription sbSubscription;
        public List<OPCUAMember_item> arrItem;

        //public ObservableCollection<OPCUAMember_item> _opcItems;

        public OPCUAMember_group(string _name, int _interval)
        {
            this.sName = _name;
            this.nInterval = _interval;
            this.arrItem = new List<OPCUAMember_item>();
        }
    }

    public class OPCUAMember_item : INotifyPropertyChanged
    {
        public bool bIsArray = false;
        public bool bIsMatrix = false;
        public Opc.Ua.DataValue dvDataValue;
        public Type tType;
        public int nGroupindex = -1;
        public int nItemindex = -1;
        


        private string _name;
        public string sName
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(sName)); }
        }

        private string _node;

        public string sNode
        {
            get => _node;
            set { _node = value; OnPropertyChanged(nameof(sNode)); }
        }

        private string _dataType;
        public string DataType
        {
            get => _dataType;
            set { _dataType = value; OnPropertyChanged(nameof(DataType)); }
        }

        private string _value;

        public string sValue
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(sValue)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public MonitoredItem moItem;

        public OPCUAMember_item(string _name, string _node, string _value)
        {
            this.sName = _name;
            this.sNode = _node;

            this.DataType = "Not Found";
            this.sValue = _value.Length >0 ? _value : "???" ;
        }
    }
}
