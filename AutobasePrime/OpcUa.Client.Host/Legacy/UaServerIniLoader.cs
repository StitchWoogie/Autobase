using NetTools;
using OpcUa.Client.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Host.Legacy
{
    public sealed class UaServerIniLoader
    {
        private readonly string _filePath;

        public UaServerIniLoader(string filePath)
        {
            _filePath = filePath;
        }

        public List<OpcUaServerDefinition> Load()
        {
            var servers = new List<OpcUaServerDefinition>();
            OpcUaServerDefinition currentServer = null;
            OpcUaGroupDefinition currentGroup = null;

            if (!File.Exists(_filePath))
                return servers;

            using (var reader = new StreamReader(_filePath, Encoding.UTF8))
            {
                var comma = new CommaBlockString();
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    comma.Set(line);

                    string head = string.Empty;
                    comma.GetString(ref head);

                    switch (head.ToLowerInvariant())
                    {
                        case "server":
                            currentServer = ParseServer(line);
                            servers.Add(currentServer);
                            currentGroup = null;
                            break;

                        case "group":
                            if (currentServer == null)
                                break;

                            currentGroup = ParseGroup(line);
                            currentServer.Groups.Add(currentGroup);
                            break;

                        case "item":
                            if (currentGroup == null)
                                break;

                            currentGroup.Items.Add(ParseItem(line));
                            break;
                    }
                }
            }

            return servers;
        }

        private static OpcUaServerDefinition ParseServer(string line)
        {
            var c = new CommaBlockString();
            string s = string.Empty;

            c.Set(line);
            c.GetString(ref s); // server
            c.GetString(ref s); // provider (unused)
            c.GetString(ref s); // tcp addr -> ServerDisplayName
            var serverAddr = s;

            c.GetString(ref s); // endpoint
            var endpoint = s;

            c.GetString(ref s); // index
            int index = Convert.ToInt32(s);

            c.GetString(ref s); // access name
            var access = s;

            return new OpcUaServerDefinition
            {
                ServerName = serverAddr,
                ServerDisplayName = serverAddr,
                HostName = endpoint,
                AccessName = access,
                EndpointIndex = index
            };
        }

        private static OpcUaGroupDefinition ParseGroup(string line)
        {
            var c = new CommaBlockString();
            string s = string.Empty;

            c.Set(line);
            c.GetString(ref s); // group
            c.GetString(ref s); // group name
            var name = s;

            c.GetString(ref s); // interval
            int interval = Convert.ToInt32(s);

            return new OpcUaGroupDefinition
            {
                GroupName = name,
                PublishingInterval = interval
            };
        }

        private static OpcUaItemDefinition ParseItem(string line)
        {
            var c = new CommaBlockString();
            string s = string.Empty;

            c.Set(line);
            c.GetString(ref s); // item
            c.GetString(ref s); // node
            var nodeId = s;

            c.GetString(ref s); // alias
            var alias = s;

            c.GetString(ref s); // reserved
            c.GetString(ref s); // reserved

            c.GetString(ref s); // initial value
            var value = s;

            return new OpcUaItemDefinition
            {
                NodeId = nodeId,
                Name = alias,
                InitialValue = value
            };
        }
    }
}
