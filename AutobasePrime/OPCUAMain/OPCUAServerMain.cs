using AutoLibLocal;
using Microsoft.Extensions.Logging;
using NetTools;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Security.Certificates;
using Opc.Ua.Server;
using OPCUAMain;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Tls;
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
using System.Threading.Tasks;

/*
 * 
Telemetry : 프로그램이  “스스로의 상태와 동작을 외부로 관측 가능하게 만드는 체계”

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
 * 
 */


namespace OPCUAMain
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
    public class OPCUAServerMain :StandardServer
    {
        static public bool bEnabled = true;
        static public bool bSecurity = false;
        static public bool bItemSecurity = false;
        static public bool bNone = true;

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
            opcReadWriteFileAccessClass.loadOpcReadWriteConfigData2();

            nPort = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerPortNumber", nPort);
            bSecurity = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerSecurity", bSecurity);
            bNone = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerNone", bNone);

            buf_ip = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerSetIP", "");
        }

        public static async Task Init()
        {
            bEnabled = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerStart", true);

            if (!bEnabled) return;

            bItemSecurity = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcServerItemSecurity", false);
            Debug.WriteLine("OPCUAServer.Init()");

            await Config_init();
            if (_server is null || _application is null) return;

            try
            {
                await _server.StartAsync(config);
                Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION, String.Format("OPC UA Server Run"));
            }
            catch
            {
                bLoad = false;
            }
        }

        public static async Task Uninit()
        {
            if( bLoad && _server != null && _application != null )
            {
                await _server.StopAsync();
                _server.Dispose();
                _server = null;

                _application = null;

                bLoad = false;
                Log.Write(AutoLibLocal.LogLevel.INFO, LogCategory.COMMUNICATION, String.Format("OPC UA Server Stopped")  );
            }

            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerPortNumber", nPort);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerNone", bNone);
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerSecurity", bSecurity);

            timer1.Stop();       
        }

        private static async Task Config_init()
        {
            string buf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pki");
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
            else
            {
                sps = new ServerSecurityPolicyCollection();
                sps.Add(new ServerSecurityPolicy
                {
                    SecurityMode = MessageSecurityMode.None,
                    SecurityPolicyUri = SecurityPolicies.None
                });
            }

            config = new ApplicationConfiguration()
            {
                ApplicationName = "LocalMain",
                ApplicationType = ApplicationType.Server,
                ServerConfiguration = new ServerConfiguration
                {
                    BaseAddresses = { $"opc.tcp://127.0.0.1:{nPort}" },
                    SecurityPolicies = sps,
                },

                SecurityConfiguration = new SecurityConfiguration
                {                
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(buf, "own"),
                        SubjectName = "CN=LocalMain, O=Autobase, C=KR"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(buf, "issuers")
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(buf, "trusted")
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(buf, "rejected")
                    },
                    AutoAcceptUntrustedCertificates = true,

                },
                ApplicationUri = "urn:autobase:LocalMain:OPCUAServer",
                TransportQuotas = new TransportQuotas(),
                TraceConfiguration = new TraceConfiguration(),
                DisableHiResClock = false
            };

            if (bSecurity)
            {
                config.CertificateValidator.CertificateValidation += CertificateValidationHandler;
            }

            buf_ip = GetLocalIPv4Address();

            if (buf_ip.Length > 0) 
                config.ServerConfiguration.BaseAddresses.Add($"opc.tcp://{buf_ip}:{nPort}/");

            try
            {
                _application = new ApplicationInstance(OpcUaTelemetry.Telemetry)
                {
                    ApplicationName = "LocalMain",
                    ApplicationType = ApplicationType.Server,
                    ApplicationConfiguration = config
                };

                // pki 인증서 자동 생성              
                try
                {
                    var _subjectname = config.SecurityConfiguration.ApplicationCertificate.SubjectName;
                    var _SAN = config.ApplicationUri;
                    var domains = new List<string> { "autobase" };

                    var existingCert = await config.SecurityConfiguration.ApplicationCertificate.FindAsync();
                    if (existingCert == null)
                    {
                        var storePath = config.SecurityConfiguration.ApplicationCertificate.StorePath;
                        var storeType = config.SecurityConfiguration.ApplicationCertificate.StoreType;

                        var cert = CertificateBuilder
                        .Create(_subjectname)                            // string 으로 처리하면 서로 다르다고 인식되는 경우가 있어서 직접 가져옴.
                        .SetHashAlgorithm(HashAlgorithmName.SHA256)
                        .AddExtension(new X509SubjectAltNameExtension(_SAN, domains))  //URI URN 을 이용한 SAN이 없으면 오류 발생함
                        .SetLifeTime(TimeSpan.FromDays(365))
                        .SetRSAKeySize(2048)
                        .CreateForRSA();

                        var store = config.SecurityConfiguration.ApplicationCertificate.OpenStore(OpcUaTelemetry.Telemetry);
                        await store.AddAsync(cert);                  // automatically saves .der in certs and private key
                        store.Close();

                        config.SecurityConfiguration.ApplicationCertificate.Certificate = cert;
                    }
                    else
                    {
                        config.SecurityConfiguration.ApplicationCertificate.Certificate = existingCert;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Certificate creation error: " + ex.ToString());
                }

                await config.ValidateAsync(ApplicationType.Server);

                //_server = new Opc.Ua.Server.StandardServer();  // public override CreateMasterNodeManager 를 추가한걸 사용하려면 새 custom class로 생성              
                _server = new OPCUAServerMain();
            }
            catch (Exception ex)
            {
                MessageDisplay.Show(ex.ToString() );
            }
        }


        private static async void CertificateValidationHandler(CertificateValidator sender, CertificateValidationEventArgs e)
        {

            var telemetry = OpcUaTelemetry.Telemetry;
            var logger = telemetry.CreateLogger("OPCUA.Certificate");

            try
            {

                if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                {
                    logger.LogWarning(
                   "Untrusted certificate received: {Thumbprint}",
                   e.Certificate.Thumbprint);

                    var store =
                  config.SecurityConfiguration.TrustedPeerCertificates
                        .OpenStore(telemetry);

                    await store.AddAsync(e.Certificate);
                    store.Close();

                    e.Accept = true;

                    logger.LogInformation(
                        "Certificate trusted and stored: {Thumbprint}",
                        e.Certificate.Thumbprint);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Certificate validation handler failed");
                e.Accept = false;
            }
        }


        public static string GetLocalIPv4Address()
        {
            if (buf_ip.Length == 0)
            {
                var list = NetworkInterface.GetAllNetworkInterfaces();

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
                        }
                    }
                }

                return "";
            }
            else
            {
                return buf_ip ;
            }
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            NodeManager = new MyNodeManager(server, configuration);
            return new MasterNodeManager(server, configuration, null, NodeManager);
        }
    }


    public class MyNodeManager : CustomNodeManager2
    {
        private ITelemetryContext _telemetry;
        private ILogger _logger;

        private FolderState _root;
        public List<AutobaseNode> _listNode;

        public class AutobaseNode
        {
            public string sTagName = "";
            public int[] nTagPos = new int[1];
            public EnumTagType nTagType = EnumTagType.none;
            public BaseDataVariableState bdItemNode;   

            public AutobaseNode()
            { 
            }
        }

        public MyNodeManager(IServerInternal server, ApplicationConfiguration config)
            : base(server, config, "urn:LocalMain:NodeManager")
        {
            _telemetry = server.Telemetry;
            _logger = _telemetry.CreateLogger<MyNodeManager>();
        }

        // Called when the server starts to create the address space
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            return new NodeStateCollection();
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            _listNode = new List<AutobaseNode>();
            // Root folder for our custom nodes
            _root = CreateFolder(
                null,
                "Autobase",
                "Autobase",
                externalReferences);

            if (AutoLibLocal.TagLib.groupRoot.arrayTag != null && AutoLibLocal.TagLib.tagListAll.Length > 0)
            {
                
                RecurseGroup(_root, externalReferences, "");
            }

            AddPredefinedNode(SystemContext, _root);

            OPCUAServerMain.bLoad = true;
        }

        private void RecurseGroup(FolderState _fs, IDictionary<NodeId, IList<IReference>> externalReferences, string groupname )
        {
            TagGrClass group = TagLib.GetGroupArray(groupname);

            for (int i = 0; i < group.arrayTag.Count; i++)
            {

                AutobaseNode tempnode = new AutobaseNode();

                TagPublicClass tp = (TagPublicClass)(group.arrayTag[i]);

                tempnode.sTagName = tp.tag;
                
                if (tp.enumTagType == EnumTagType.GR)
                {               
                    FolderState _folder = CreateFolder(
                    _fs,
                    tp.tag,
                    tp.name,
                    tp.description,
                    externalReferences);

                    RecurseGroup(_folder, externalReferences, tempnode.sTagName) ;
                }
                else
                {
                    TagLib.GetTagTypeAndPos(tempnode.sTagName, ref tempnode.nTagType, ref tempnode.nTagPos);

                    if (tempnode.nTagType == EnumTagType.AI)
                    {
                        TagAiClass ai = TagLib.GetStructAI(tempnode.sTagName, ref tempnode.nTagPos);

                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Double,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = ai.curr;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue =   (ISystemContext context, NodeState node, ref object value) => 
                        {
                             AutoLib.TagWrite.WriteCurrAI(tp.tag, ai, (double)value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    Console.WriteLine($"[SERVER] WriteCurrAI error: {t.Exception}");
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);

                    }
                    else if (tempnode.nTagType == EnumTagType.AO)
                    {
                        TagAoClass ao = TagLib.GetStructAO(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Double,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = ao.curr;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {

                            AutoLib.TagWrite.WriteCurrAO(tp.tag, ao, (double)value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    Console.WriteLine($"[SERVER] WriteCurrAO error: {t.Exception}");
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.DI)
                    {
                        TagDiClass di = TagLib.GetStructDI(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Boolean,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = di.curr == 0 ? false : true;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            sbyte _value = (bool)value ? (sbyte)1 : (sbyte)0;

                            AutoLib.TagWrite.WriteCurrDI(tp.tag, di, _value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    Console.WriteLine($"[SERVER] WriteCurrDI error: {t.Exception}");
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.DO)
                    {
                        TagDoClass d_o = TagLib.GetStructDO(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Boolean,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = d_o.curr == 0 ? false : true;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            sbyte _value = (bool)value ? (sbyte)1 : (sbyte)0;

                            AutoLib.TagWrite.WriteCurrDO(tp.tag, d_o, _value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    Console.WriteLine($"[SERVER] WriteCurrDO error: {t.Exception}");
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.GDO)
                    {
                        TagDoGroupClass GDO = TagLib.GetStructDoGroup(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.Boolean,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = GDO.curr == 0 ? false : true; 
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                            sbyte _value = (bool)value ? (sbyte)1 : (sbyte)0;

                            AutoLib.TagWrite.WriteCurrDoGroup( GDO, _value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    Console.WriteLine($"[SERVER] WriteCurrGDO error: {t.Exception}");
                            });

                            return ServiceResult.Good;
                        };

                        _listNode.Add(tempnode);
                    }
                    else if (tempnode.nTagType == EnumTagType.ST)
                    {
                        TagStClass ST = TagLib.GetStructST(tempnode.sTagName, ref tempnode.nTagPos);


                        tempnode.bdItemNode = CreateVariable(
                         _fs,
                         tp.tag,
                         tp.name,
                         tp.description,
                         DataTypeIds.String,
                         ValueRanks.Scalar,
                         tp.flagsOpcServer);

                        tempnode.bdItemNode.Value = ST.curr;
                        tempnode.bdItemNode.StatusCode = StatusCodes.Good;
                        tempnode.bdItemNode.Timestamp = DateTime.UtcNow;

                        tempnode.bdItemNode.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object value) =>
                        {
                          
                            AutoLib.TagWrite.WriteCurrST( tp.tag, ST, (string)value, true).ContinueWith(t =>
                            {
                                if (t.Exception != null)
                                    Console.WriteLine($"[SERVER] WriteCurrST error: {t.Exception}");
                            });

                            return ServiceResult.Good;
                        };


                        _listNode.Add(tempnode);
                    }
                    else
                    {

                    }
                }
            }
        }

        // Helper to create folder
        private FolderState CreateFolder(NodeState parent, string name, string displayName, IDictionary<NodeId, IList<IReference>> externalRefs)
        {
            string buf = "";

            var folder = new FolderState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId( buf = (parent == null) ? name : string.Format("Autobase.{0}", name ), NamespaceIndexes[0]),
                BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                DisplayName = displayName,
                EventNotifier = EventNotifiers.None
            };

            if (parent == null)
            {
                IList<IReference> refs;
                if (!externalRefs.TryGetValue(ObjectIds.ObjectsFolder, out refs))
                {
                    refs = new List<IReference>();
                    externalRefs[ObjectIds.ObjectsFolder] = refs;
                }
                refs.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));
                folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            }
            else
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        private FolderState CreateFolder(NodeState parent, string name, string displayName, string description , IDictionary<NodeId, IList<IReference>> externalRefs)
        {
            string buf = "";

            var folder = new FolderState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(buf = (parent == null) ? name : string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                DisplayName = displayName,
                Description = description,
                EventNotifier = EventNotifiers.None
            };

            if (parent == null)
            {
                IList<IReference> refs;
                if (!externalRefs.TryGetValue(ObjectIds.ObjectsFolder, out refs))
                {
                    refs = new List<IReference>();
                    externalRefs[ObjectIds.ObjectsFolder] = refs;
                }
                refs.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));
                folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            }
            else
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        // Helper to create variable
        private BaseDataVariableState CreateVariable(NodeState parent, string name, string displayName, string description, NodeId dataType, int valueRank, uint opcflag)
        {
            byte flag = AccessLevels.CurrentReadOrWrite;
            bool bHide = false;

            if (OPCUAServerMain.bItemSecurity)
            {
                if ( (opcflag & 1) == 0)
                {
                    flag = AccessLevels.None;
                    bHide = true;
                }
                else
                {
                    if (opcflag == 7) flag = AccessLevels.CurrentReadOrWrite;
                    else
                    {
                        flag = AccessLevels.None;

                        if ( (opcflag & 2) != 0) flag = AccessLevels.CurrentRead;

                        if ( (opcflag & 4) != 0) flag = AccessLevels.CurrentWrite;
                    }
                }
            }

            if ( bHide )
            {
                var variable = new BaseDataVariableState(parent)
                {
                    SymbolicName = name,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                    NodeId = new NodeId(string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                    BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                    DisplayName = displayName,
                    Description = description,
                    DataType = dataType,
                    ValueRank = valueRank,
                    AccessLevel = flag,
                    UserAccessLevel = flag,
                    RolePermissions = new RolePermissionType[]
                    {
                    new RolePermissionType
                    {
                        RoleId = Objects.WellKnownRole_Anonymous,
                        Permissions = 0
                    }
                    },
                    Historizing = false
                };

                parent?.AddChild(variable);
                return variable;
            }
            else
            {
                var variable = new BaseDataVariableState(parent)
                {
                    SymbolicName = name,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                    NodeId = new NodeId(string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                    BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                    DisplayName = displayName,
                    Description = description,
                    DataType = dataType,
                    ValueRank = valueRank,
                    AccessLevel = flag,
                    UserAccessLevel = flag,
                    Historizing = false
                };

                parent?.AddChild(variable);
                return variable;
            }              
        }

        private BaseDataVariableState CreateVariable(NodeState parent, string name, string displayName,  NodeId dataType, int valueRank, uint opcflag)
        {

            byte flag = AccessLevels.CurrentReadOrWrite;
            if (OPCUAServerMain.bItemSecurity)
            {
                if (opcflag % 1 == 0)
                {
                    flag = AccessLevels.None;
                }
                else
                {
                    if (opcflag == 7) flag = AccessLevels.CurrentReadOrWrite;
                    else
                    {
                        if (opcflag % 2 != 0) flag = AccessLevels.CurrentRead;

                        if (opcflag % 4 != 0) flag = AccessLevels.CurrentWrite;
                    }
                }
            }

            var variable = new BaseDataVariableState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(string.Format("Autobase.{0}", name), NamespaceIndexes[0]),
                BrowseName = new QualifiedName(name, NamespaceIndexes[0]),
                DisplayName = displayName,
                DataType = dataType,
                ValueRank = valueRank,
                AccessLevel = flag,
                UserAccessLevel = flag,
                Historizing = false
            };

            parent?.AddChild(variable);
            return variable;
        }

        // Example: method to update the variable
        public void UpdateItem(int index, double newValue, string tag)
        {
            if (!OPCUAServerMain.bEnabled) return;
            try
            {
                AutobaseNode abn = this._listNode[index];

                if (abn.sTagName != tag) return; //wrong update

                BaseDataVariableState itemnode = abn.bdItemNode;
                if ((double) itemnode.Value == newValue) return;

                itemnode.Value = newValue;
                itemnode.Timestamp = DateTime.UtcNow;
                itemnode.ClearChangeMasks(SystemContext, false);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void UpdateItem(int index, sbyte newValue, string tag)
        {
            if (!OPCUAServerMain.bEnabled) return;
            try
            {
                bool _value = newValue == 0 ? false : true;
                AutobaseNode abn = this._listNode[index];

                if (abn.sTagName != tag) return; //wrong update

                BaseDataVariableState itemnode = abn.bdItemNode;
                if ((bool)itemnode.Value == _value) return;

                itemnode.Value = _value;
                itemnode.Timestamp = DateTime.UtcNow;
                itemnode.ClearChangeMasks(SystemContext, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void UpdateItem(int index, string newValue, string tag)
        {
            if (!OPCUAServerMain.bEnabled) return;
            try
            {
               
                AutobaseNode abn = this._listNode[index];

                if (abn.sTagName != tag) return; //wrong update

                BaseDataVariableState itemnode = abn.bdItemNode;
                if ((string)itemnode.Value == newValue) return;

                itemnode.Value = newValue;
                itemnode.Timestamp = DateTime.UtcNow;
                itemnode.ClearChangeMasks(SystemContext, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }





    }


    


}
