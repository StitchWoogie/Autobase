using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Abstractions
{
    public sealed class OpcUaServerDefinition
    {
        public string ServerName;
        public string ServerDisplayName; //ApplicationDescription
        public string HostName;
        public string AccessName;
        public int EndpointIndex;

        // Endpoint Security (사용자가 선택한 endpoint의 보안 설정)
        public string EndpointUrl;                 // 실제 endpoint URL
        public string SecurityPolicyUri;           // e.g. http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256
        public int SecurityMode;                   // MessageSecurityMode: 1=None, 2=Sign, 3=SignAndEncrypt

        public List<OpcUaGroupDefinition> Groups = new List<OpcUaGroupDefinition>();

        // Authentication
        public OpcUaAuthMode AuthMode;
        public string AuthUserName;
        public string AuthPasswordProtected;       // DPAPI-encrypted Base64
        public string AuthCertificateThumbprint;
        public string AuthCertificateFilePath;     // .pfx/.p12 file path
        public string AuthCertPasswordProtected;   // DPAPI-encrypted pfx password
        public bool SaveCredentials;                // true이면 수정 시 username/password 표시
    }
}
