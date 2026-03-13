using NetTools;
using PublicStudioLocalMain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studio.OPCUA
{
    /// <summary>
    /// OPC UA Client가 꺼져있을 때 UAServer.ini 파일에서
    /// 서버/그룹/아이템 트리 정보를 읽어오는 오프라인 Provider.
    /// 현재값은 표시하지 않고 태그 이름만 제공한다.
    /// </summary>
    class OpcUaOfflineBrowseProvider : IOpcBrowseProvider
    {
        private readonly string _iniPath;

        public OpcUaOfflineBrowseProvider(string iniPath)
        {
            _iniPath = iniPath;
        }

        public Task<IEnumerable<string>> GetServersAsync()
        {
            var defs = LoadDefinitions();
            IEnumerable<string> result = defs
                .Select(s => s.AccessName)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<string>> GetGroupsAsync(string server)
        {
            if (string.IsNullOrEmpty(server))
                return Task.FromResult(Enumerable.Empty<string>());

            var defs = LoadDefinitions();
            var srv = defs.FirstOrDefault(s =>
                string.Equals(s.AccessName, server,
                    StringComparison.OrdinalIgnoreCase));

            if (srv == null)
                return Task.FromResult(Enumerable.Empty<string>());

            IEnumerable<string> result = srv.Groups
                .Select(g => g.GroupName)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<OpcBrowseItem>> GetItemsAsync(
            string server, string group)
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(group))
                return Task.FromResult(Enumerable.Empty<OpcBrowseItem>());

            var defs = LoadDefinitions();
            var srv = defs.FirstOrDefault(s =>
                string.Equals(s.AccessName, server,
                    StringComparison.OrdinalIgnoreCase));

            if (srv == null)
                return Task.FromResult(Enumerable.Empty<OpcBrowseItem>());

            var grp = srv.Groups.FirstOrDefault(g =>
                string.Equals(g.GroupName, group,
                    StringComparison.OrdinalIgnoreCase));

            if (grp == null)
                return Task.FromResult(Enumerable.Empty<OpcBrowseItem>());

            IEnumerable<OpcBrowseItem> result = grp.Items
                .Select(i => new OpcBrowseItem
                {
                    Name = i.Name,
                    Value = i.LastValue  // INI에 저장된 최종값 표시
                })
                .ToList();
            return Task.FromResult(result);
        }

        // ─── INI parsing (OpcUaClientConfigManager 와 동일한 형식) ───

        private List<ServerDef> LoadDefinitions()
        {
            var result = new List<ServerDef>();

            if (!File.Exists(_iniPath))
                return result;

            try
            {
                using (var reader = new StreamReader(_iniPath, Encoding.UTF8))
                {
                    var comma = new CommaBlockString();
                    string line;

                    ServerDef currentServer = null;
                    GroupDef currentGroup = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        comma.Set(line);
                        string head = "";
                        comma.GetString(ref head);
                        head = head.ToLowerInvariant();

                        if (head == "server")
                        {
                            currentServer = ParseServer(line);
                            result.Add(currentServer);
                            currentGroup = null;
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
                        // endpoint_security, auth 등은 트리 표시에 불필요하므로 무시
                    }
                }
            }
            catch
            {
                // 파일 읽기 실패 시 빈 목록 반환
            }

            return result;
        }

        private static ServerDef ParseServer(string line)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // "server"
            c.GetString(ref s); // provider
            c.GetString(ref s); // server name
            c.GetString(ref s); // server display name
            c.GetString(ref s); // host/endpoint
            c.GetString(ref s); // index
            c.GetString(ref s); // access name
            string access = s;

            return new ServerDef { AccessName = access };
        }

        private static GroupDef ParseGroup(string line)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // "group"
            c.GetString(ref s); // group name

            return new GroupDef { GroupName = s };
        }

        private static ItemDef ParseItem(string line)
        {
            var c = new CommaBlockString();
            string s = "";

            c.Set(line);
            c.GetString(ref s); // "item"
            c.GetString(ref s); // nodeId
            c.GetString(ref s); // name (alias)
            string name = s;

            c.GetString(ref s); // reserved
            c.GetString(ref s); // reserved
            c.GetString(ref s); // initialValue
            c.GetString(ref s); // lastValue (7번째 필드)
            string lastValue = s;

            return new ItemDef { Name = name, LastValue = lastValue };
        }

        // ─── 경량 내부 DTO (Abstractions 참조 불필요) ───

        private class ServerDef
        {
            public string AccessName;
            public List<GroupDef> Groups = new List<GroupDef>();
        }

        private class GroupDef
        {
            public string GroupName;
            public List<ItemDef> Items = new List<ItemDef>();
        }

        private class ItemDef
        {
            public string Name;
            public string LastValue;
        }
    }
}
