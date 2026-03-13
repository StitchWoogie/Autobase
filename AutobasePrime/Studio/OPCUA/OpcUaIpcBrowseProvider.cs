using OpcUaClient.Ipc.Client;
using PublicStudioLocalMain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studio.OPCUA
{
    class OpcUaIpcBrowseProvider : IOpcBrowseProvider
    {
        private IpcClient Client => OpcUaIpcManager.Client;

        public async Task<IEnumerable<string>> GetServersAsync()
        {
            var client = Client;
            if (client == null || !client.IsConnected)
                return Enumerable.Empty<string>();

            var res = await client.GetServersAsync().ConfigureAwait(false);
            if (res?.Servers == null)
                return Enumerable.Empty<string>();

            return res.Servers
                      .Select(s => s.AccessName)
                      .ToList();
        }

        public async Task<IEnumerable<string>> GetGroupsAsync(string server)
        {
            if (string.IsNullOrEmpty(server))
                return Enumerable.Empty<string>();

            var client = Client;
            if (client == null || !client.IsConnected)
                return Enumerable.Empty<string>();

            var res = await client.GetGroupsAsync(server).ConfigureAwait(false);
            if (res?.Groups == null)
                return Enumerable.Empty<string>();

            return res.Groups
                      .Select(g => g.GroupName)
                      .ToList();
        }

        public async Task<IEnumerable<OpcBrowseItem>> GetItemsAsync(string server, string group)
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(group))
                return Enumerable.Empty<OpcBrowseItem>();

            var client = Client;
            if (client == null || !client.IsConnected)
                return Enumerable.Empty<OpcBrowseItem>();

            var res = await client.GetItemsAsync(server, group).ConfigureAwait(false);
            if (res?.Items == null)
                return Enumerable.Empty<OpcBrowseItem>();

            var list = new List<OpcBrowseItem>();

            foreach (var i in res.Items)
            {
                string value = null;

                if (!string.IsNullOrEmpty(i.FullName) &&
                    OpcUaClientCache.TryGet(i.FullName, out var cache))
                {
                    value = cache.ValueString;
                }

                list.Add(new OpcBrowseItem
                {
                    Name = i.ItemName,
                    Value = value
                });
            }

            return list;
        }

    }
}
