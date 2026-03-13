using AutoLibLocal;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using OPCUA.Client.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Host
{
    public sealed class OpcUaHostConfigBuilder
    {
        public string ApplicationName { get; set; } = "AutobaseOpcUaClient";
        public string ApplicationUri { get; set; } = "urn:autobase.opcua:client";
        // PKI base dir (프로젝트/제품 정책)
        public string PkiBasePath { get; set; }

        // 보안 정책 (Host가 결정)
        public bool AutoAcceptUntrusted { get; set; } = true;

        // 타임아웃/쿼터
        public int OperationTimeoutMs { get; set; } = 15000;
        public int DefaultSessionTimeoutMs { get; set; } = 60000;

        //  개인키 암호화 지원
        public string CertificatePassword { get; set; } = null;

        public ApplicationConfiguration Build()
        {
            if (string.IsNullOrWhiteSpace(PkiBasePath))
                throw new InvalidOperationException("PkiBasePath must be set.");

            EnsurePkiDirectories(PkiBasePath);

            var appUri = ApplicationUri ??
                         Utils.Format(@"urn:{0}:{1}",
                             System.Net.Dns.GetHostName(),
                             ApplicationName);

            var config = new ApplicationConfiguration
            {
                ApplicationName = ApplicationName,
                ApplicationUri = appUri,
                ApplicationType = ApplicationType.Client,

                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(PkiBasePath, "own"),
                        SubjectName = $"CN={ApplicationName}, DC={System.Net.Dns.GetHostName()}"
                    },

                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(PkiBasePath, "trusted")
                    },

                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(PkiBasePath, "issuers")
                    },

                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = Path.Combine(PkiBasePath, "rejected")
                    },
                    // AutoAcceptUntrusted=true → SDK가 보안채널 수립 시 인증서 자동 수락
                    // AutoAcceptUntrusted=false → CertificateValidation 이벤트로 수동 처리
                    AutoAcceptUntrustedCertificates = AutoAcceptUntrusted
                },

                TransportConfigurations = new TransportConfigurationCollection(),

                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = OperationTimeoutMs
                },

                ClientConfiguration = new ClientConfiguration
                {
                    DefaultSessionTimeout = DefaultSessionTimeoutMs
                }
            };

            return config;
        }

        private static void EnsurePkiDirectories(string basePath)
        {
            Directory.CreateDirectory(Path.Combine(basePath, "own"));
            Directory.CreateDirectory(Path.Combine(basePath, "trusted"));
            Directory.CreateDirectory(Path.Combine(basePath, "issuers"));
            Directory.CreateDirectory(Path.Combine(basePath, "rejected"));
        }

        public static ApplicationConfiguration BuildUaConfigFromHostPolicy()
        {
            var builder = new OpcUaHostConfigBuilder
            {
                ApplicationName = "AutobaseOpcUaClient",
                //PkiBasePath = Path.Combine(
                //    TotalConfig.sDirWorkProject,
                //    "OpcData",
                //    "UaClient",
                //    "pki"),

                PkiBasePath = Path.Combine(
                   AppDomain.CurrentDomain.BaseDirectory, "OPCUAClient", "pki"),

                //AutoAcceptUntrusted =
                //    TotalConfig.LoadRegAutoBaseConfig(
                //        "Config", "Start",
                //        "OpcUaClientAutoAcceptUntrusted", true),

                OperationTimeoutMs = 15000,
                DefaultSessionTimeoutMs = 60000
            };

            var config = builder.Build();


            // pki 인증서 자동 생성              
            try
            {
                var _subjectname = config.SecurityConfiguration.ApplicationCertificate.SubjectName;
                var _SAN = config.ApplicationUri;
                var domains = GetAllLocalAddresses();

                var existingCert = config.SecurityConfiguration.ApplicationCertificate.FindAsync().GetAwaiter().GetResult();

                if (existingCert == null) //인증서가 없는 경우 생성
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

                    var store = config.SecurityConfiguration.ApplicationCertificate.OpenStore(OpcUaClientTelemetry.Telemetry);
                    store.AddAsync(cert).GetAwaiter().GetResult();                 // automatically saves .der in certs and private key
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
                Debug.WriteLine("Auto Certificate creation error: " + ex.ToString());
            }

            // ✔ Host 초기화 단계에서는 동기 검증이 맞다
            config.ValidateAsync(ApplicationType.Client).GetAwaiter().GetResult();

            return config;
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

    }
}
