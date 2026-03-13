using NetTools;
using OpcUa.Client.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Host
{

    //// Init / Load / Save
    //Initialize(string baseDir);
    //LoadServerDefinitions();
    //Save();

    //// Server CRUD
    //AddServer(OpcUaServerDefinition def);
    //UpdateServer(string accessName, Action<OpcUaServerDefinition> updater);
    //RenameServer(string oldAccessName, string newAccessName); // optional
    //RemoveServer(string accessName);

    /// <summary>
    /// OPC UA Client 설정 관리자
    /// - 서버/그룹/아이템 "정의(definition)"만 관리
    /// - Runtime / OPC UA 객체 참조 금지
    /// </summary>
    public static class OpcUaClientConfigManager
    {
        private static string _baseDir;
        private static string _serverIniPath;

        private static List<OpcUaServerDefinition> _definitions =
            new List<OpcUaServerDefinition>();

        // =========================
        // Init
        // =========================

        public static void Initialize(string baseDir)
        {
            if (string.IsNullOrEmpty(baseDir))
                throw new ArgumentNullException("baseDir");

            _baseDir = baseDir;
            Directory.CreateDirectory(_baseDir);

            _serverIniPath = Path.Combine(_baseDir, "UAServer.ini");

            _definitions = LoadServerDefinitionsInternal();
        }

        // =========================
        // Public API
        // =========================

        public static List<OpcUaServerDefinition> LoadServerDefinitions()
        {
            // 외부에서 수정 못 하게 복사본 반환
            return CloneDefinitions(_definitions);
        }

        public static void SetServerDefinitions(
            List<OpcUaServerDefinition> defs)
        {
            if (defs == null)
                throw new ArgumentNullException("defs");

            _definitions = CloneDefinitions(defs);
        }

        public static void Save()
        {
            SaveServerDefinitionsInternal(_definitions);
        }

        // =========================
        // Load (INI → Definition)
        // =========================

        private static List<OpcUaServerDefinition> LoadServerDefinitionsInternal()
        {
            var result = new List<OpcUaServerDefinition>();
            var accessNameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(_serverIniPath))
                return result;

            using (var reader = new StreamReader(_serverIniPath, Encoding.UTF8))
            {
                var comma = new CommaBlockString();
                string line;

                OpcUaServerDefinition currentServer = null;
                OpcUaGroupDefinition currentGroup = null;

                while ((line = reader.ReadLine()) != null)
                {
                    comma.Set(line);

                    string head = "";
                    comma.GetString(ref head);
                    head = head.ToLowerInvariant();

                    if (head == "server")
                    {
                        var parsed = ParseServer(line);

                        if (accessNameSet.Contains(parsed.AccessName))
                        {
                            // ⚠ 중복 서버 무시 (또는 로그)
                            continue;
                        }

                        accessNameSet.Add(parsed.AccessName);
                        currentServer = parsed;
                        result.Add(currentServer);
                        currentGroup = null;
                    }
                    else if (head == "endpoint_security")
                    {
                        if (currentServer == null)
                            continue;

                        ParseEndpointSecurity(line, currentServer);
                    }
                    else if (head == "auth")
                    {
                        if (currentServer == null)
                            continue;

                        ParseAuth(line, currentServer);
                    }
                    else if (head == "group")
                    {
                        if (currentServer == null)
                            continue;

                        currentGroup = ParseGroup(line);
                        currentServer.Groups.Add(currentGroup);
                    }
                    else if (head == "item")
                    {
                        if (currentGroup == null)
                            continue;

                        currentGroup.Items.Add(ParseItem(line));
                    }
                }
            }

            return result;
        }

        private static OpcUaServerDefinition ParseServer(string line)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // server
            c.GetString(ref s); // provider (opcua)
            c.GetString(ref s); // server name (표시용)
            string serverName = s;

            c.GetString(ref s); // tcp addr -> server display name, ApplicationDescription
            string serverDisplayName = s;

            c.GetString(ref s); // endpoint
            string endpoint = s;

            c.GetString(ref s); // index
            int index = 0;
            int.TryParse(s, out index);

            c.GetString(ref s); // access name
            string access = s;

            return new OpcUaServerDefinition
            {
                ServerName = serverName,
                ServerDisplayName = serverDisplayName,
                HostName = endpoint,
                AccessName = access,
                EndpointIndex = index,
            };
        }

        private static void ParseAuth(string line, OpcUaServerDefinition server)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // "auth"

            c.GetString(ref s); // authMode
            int authMode = 0;
            int.TryParse(s, out authMode);
            server.AuthMode = (OpcUaAuthMode)authMode;

            c.GetString(ref s); // userName
            server.AuthUserName = s;

            // password (DPAPI Base64) - use GetStringTotalRemain to avoid comma issues
            if (authMode == (int)OpcUaAuthMode.UserName)
            {
                c.GetString(ref s);
                server.AuthPasswordProtected = s;

                c.GetString(ref s); // certThumbprint (empty for username mode)
                server.AuthCertificateThumbprint = s;
            }
            else
            {
                c.GetString(ref s); // password (empty for non-username mode)
                server.AuthPasswordProtected = s;

                c.GetString(ref s); // certThumbprint
                server.AuthCertificateThumbprint = s;
            }

            // Extended cert fields (v2)
            c.GetString(ref s); // certFilePath
            server.AuthCertificateFilePath = s;

            c.GetString(ref s); // certPasswordProtected
            server.AuthCertPasswordProtected = s;

            // SaveCredentials flag (v3)
            c.GetString(ref s);
            server.SaveCredentials = (s == "1");
        }

        private static void ParseEndpointSecurity(string line, OpcUaServerDefinition server)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // "endpoint_security"

            c.GetString(ref s); // endpointUrl
            server.EndpointUrl = s;

            c.GetString(ref s); // securityPolicyUri
            server.SecurityPolicyUri = s;

            c.GetString(ref s); // securityMode
            int mode = 0;
            int.TryParse(s, out mode);
            server.SecurityMode = mode;
        }

        private static OpcUaGroupDefinition ParseGroup(string line)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // group
            c.GetString(ref s); // group name
            string name = s;

            c.GetString(ref s); // interval
            int interval = 1000;
            int.TryParse(s, out interval);

            return new OpcUaGroupDefinition
            {
                GroupName = name,
                PublishingInterval = interval
            };
        }

        private static OpcUaItemDefinition ParseItem(string line)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // item
            c.GetString(ref s); // node
            string node = s;

            c.GetString(ref s); // alias
            string alias = s;

            c.GetString(ref s); // reserved
            c.GetString(ref s); // reserved
            c.GetString(ref s); // initialValue
            string value = s;

            c.GetString(ref s); // lastValue (7번째 필드, 없으면 빈 문자열)
            string lastValue = s;

            return new OpcUaItemDefinition
            {
                NodeId = node,
                Name = alias,
                InitialValue = value,
                LastValue = lastValue
            };
        }

        // =========================
        // Save (Definition → INI)
        // =========================

        private static void SaveServerDefinitionsInternal(
            List<OpcUaServerDefinition> defs)
        {
            if (defs == null)
                return;

            Directory.CreateDirectory(_baseDir);

            using (var writer = new StreamWriter(
                _serverIniPath, false, Encoding.UTF8))
            {
                foreach (var srv in defs)
                {
                    writer.WriteLine(
                         "server,{0},{1},{2},{3},{4},{5}",
                         "opcua",                   // provider (dummy)
                         srv.ServerName ?? "",      // server name (표시용)
                         srv.ServerDisplayName ?? "",      // tcp addr -> ServerDisplayName, ApplicationDescription
                         srv.HostName ?? "",        // endpoint
                         srv.EndpointIndex,
                         srv.AccessName ?? "");

                    // endpoint security line
                    writer.WriteLine(
                        "endpoint_security,{0},{1},{2}",
                        srv.EndpointUrl ?? "",
                        srv.SecurityPolicyUri ?? "",
                        srv.SecurityMode);

                    // auth line (always written for forward compatibility)
                    writer.WriteLine(
                        "auth,{0},{1},{2},{3},{4},{5},{6}",
                        (int)srv.AuthMode,
                        srv.AuthUserName ?? "",
                        srv.AuthPasswordProtected ?? "",
                        srv.AuthCertificateThumbprint ?? "",
                        srv.AuthCertificateFilePath ?? "",
                        srv.AuthCertPasswordProtected ?? "",
                        srv.SaveCredentials ? "1" : "0");

                    foreach (var grp in srv.Groups)
                    {
                        writer.WriteLine(
                            "group,{0},{1},0,0,0,0",
                            grp.GroupName ?? "",
                            grp.PublishingInterval);

                        foreach (var item in grp.Items)
                        {
                            writer.WriteLine(
                                "item,{0},{1},0,0,{2},{3}",
                                item.NodeId ?? "",
                                item.Name ?? "",
                                item.InitialValue ?? "",
                                item.LastValue ?? "");
                        }
                    }
                }
            }
        }

        // =========================
        // Utils
        // =========================

        private static List<OpcUaServerDefinition> CloneDefinitions(
            List<OpcUaServerDefinition> src)
        {
            var list = new List<OpcUaServerDefinition>();

            foreach (var s in src)
            {
                var ns = new OpcUaServerDefinition
                {
                    ServerName = s.ServerName,
                    ServerDisplayName = s.ServerDisplayName,
                    HostName = s.HostName,
                    AccessName = s.AccessName,
                    EndpointIndex = s.EndpointIndex,
                    EndpointUrl = s.EndpointUrl,
                    SecurityPolicyUri = s.SecurityPolicyUri,
                    SecurityMode = s.SecurityMode,
                    AuthMode = s.AuthMode,
                    AuthUserName = s.AuthUserName,
                    AuthPasswordProtected = s.AuthPasswordProtected,
                    AuthCertificateThumbprint = s.AuthCertificateThumbprint,
                    AuthCertificateFilePath = s.AuthCertificateFilePath,
                    AuthCertPasswordProtected = s.AuthCertPasswordProtected,
                    SaveCredentials = s.SaveCredentials,
                };

                foreach (var g in s.Groups)
                {
                    var ng = new OpcUaGroupDefinition
                    {
                        GroupName = g.GroupName,
                        PublishingInterval = g.PublishingInterval
                    };

                    foreach (var i in g.Items)
                    {
                        ng.Items.Add(new OpcUaItemDefinition
                        {
                            Name = i.Name,
                            NodeId = i.NodeId,
                            InitialValue = i.InitialValue,
                            LastValue = i.LastValue
                        });
                    }

                    ns.Groups.Add(ng);
                }

                list.Add(ns);
            }

            return list;
        }

        private static OpcUaServerDefinition CloneServer(
            OpcUaServerDefinition src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            var dst = new OpcUaServerDefinition
            {
                ServerName = src.ServerName,
                ServerDisplayName = src.ServerDisplayName,
                HostName = src.HostName,
                AccessName = src.AccessName,
                EndpointIndex = src.EndpointIndex,
                EndpointUrl = src.EndpointUrl,
                SecurityPolicyUri = src.SecurityPolicyUri,
                SecurityMode = src.SecurityMode,
                AuthMode = src.AuthMode,
                AuthUserName = src.AuthUserName,
                AuthPasswordProtected = src.AuthPasswordProtected,
                AuthCertificateThumbprint = src.AuthCertificateThumbprint,
                AuthCertificateFilePath = src.AuthCertificateFilePath,
                AuthCertPasswordProtected = src.AuthCertPasswordProtected,
                SaveCredentials = src.SaveCredentials,
            };

            foreach (var g in src.Groups)
            {
                var ng = new OpcUaGroupDefinition
                {
                    GroupName = g.GroupName,
                    PublishingInterval = g.PublishingInterval
                };

                foreach (var i in g.Items)
                {
                    ng.Items.Add(new OpcUaItemDefinition
                    {
                        Name = i.Name,
                        NodeId = i.NodeId,
                        InitialValue = i.InitialValue
                    });
                }

                dst.Groups.Add(ng);
            }

            return dst;
        }

        public static void AddServer(OpcUaServerDefinition def)
        {
            if (def == null)
                throw new ArgumentNullException(nameof(def));

            if (string.IsNullOrEmpty(def.AccessName))
                throw new ArgumentException("AccessName is required.");

            // 중복 검사
            foreach (var s in _definitions)
            {
                if (string.Equals(
                    s.AccessName,
                    def.AccessName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Server '{def.AccessName}' already exists.");
                }
            }

            _definitions.Add(CloneServer(def));
        }

        public static bool UpdateServer(
            string accessName,
            Action<OpcUaServerDefinition> updater)
        {
            if (string.IsNullOrEmpty(accessName))
                return false;

            var src = _definitions
                  .FirstOrDefault(s =>
                      string.Equals(
                          s.AccessName,
                          accessName,
                          StringComparison.OrdinalIgnoreCase));

            if (src == null)
                return false;

            var clone = CloneServer(src);

            var originalAccessName = src.AccessName;

            updater?.Invoke(clone);

            if (!string.Equals(
                clone.AccessName,
                originalAccessName,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Changing AccessName is not allowed.");
            }

            // 원본 교체 (atomic)
            int index = _definitions.IndexOf(src);
            _definitions[index] = clone;

            return true;
        }

        public static bool RemoveServer(string accessName)
        {
            if (string.IsNullOrEmpty(accessName))
                return false;

            if (_definitions == null)
                return false;

            for (int i = 0; i < _definitions.Count; i++)
            {
                if (string.Equals(
                    _definitions[i].AccessName,
                    accessName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    _definitions.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public static bool RenameServer(
        string oldAccessName,
        string newAccessName)
        {
            if (string.IsNullOrEmpty(oldAccessName) ||
                string.IsNullOrEmpty(newAccessName))
                return false;

            // 중복 방지
            foreach (var s in _definitions)
            {
                if (string.Equals(s.AccessName, newAccessName,
                    StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("AccessName already exists.");
            }

            foreach (var s in _definitions)
            {
                if (string.Equals(s.AccessName, oldAccessName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    s.AccessName = newAccessName;
                    return true;
                }
            }

            return false;
        }

        public static bool AddGroup(
    string serverAccessName,
    OpcUaGroupDefinition group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return UpdateServer(
                serverAccessName,
                s =>
                {
                    if (s.Groups.Any(g =>
                        string.Equals(g.GroupName, group.GroupName,
                            StringComparison.OrdinalIgnoreCase)))
                        throw new InvalidOperationException("Group already exists.");

                    s.Groups.Add(group);
                });
        }


        public static bool RemoveItem(
    string accessName,
    string groupName,
    string itemName)
        {
            if (string.IsNullOrEmpty(accessName) ||
                string.IsNullOrEmpty(groupName) ||
                string.IsNullOrEmpty(itemName))
                return false;

            var server = _definitions
                .FirstOrDefault(s =>
                    string.Equals(s.AccessName, accessName,
                        StringComparison.OrdinalIgnoreCase));

            if (server == null)
                return false;

            var group = server.Groups
                .FirstOrDefault(g =>
                    string.Equals(g.GroupName, groupName,
                        StringComparison.OrdinalIgnoreCase));

            if (group == null)
                return false;

            for (int i = 0; i < group.Items.Count; i++)
            {
                if (string.Equals(
                    group.Items[i].Name,
                    itemName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    group.Items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public static bool UpdateItem(
    string accessName,
    string groupName,
    string itemName,
    Action<OpcUaItemDefinition> updater)
        {
            if (string.IsNullOrEmpty(accessName) ||
                string.IsNullOrEmpty(groupName) ||
                string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException();

            if (updater == null)
                throw new ArgumentNullException(nameof(updater));

            var server = _definitions
                .FirstOrDefault(s =>
                    string.Equals(s.AccessName, accessName,
                        StringComparison.OrdinalIgnoreCase));

            if (server == null)
                return false;

            var group = server.Groups
                .FirstOrDefault(g =>
                    string.Equals(g.GroupName, groupName,
                        StringComparison.OrdinalIgnoreCase));

            if (group == null)
                return false;

            var item = group.Items
                .FirstOrDefault(i =>
                    string.Equals(i.Name, itemName,
                        StringComparison.OrdinalIgnoreCase));

            if (item == null)
                return false;

            // 🔹 변경 전 NodeId 백업
            string oldNodeId = item.NodeId;

            // 🔹 수정 적용
            updater(item);

            // 🔹 NodeId 변경 여부 반환
            return !string.Equals(
                oldNodeId,
                item.NodeId,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
