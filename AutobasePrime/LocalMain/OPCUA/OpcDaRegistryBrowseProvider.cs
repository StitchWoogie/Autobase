using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicStudioLocalMain
{
    class OpcDaRegistryBrowseProvider : IOpcBrowseProvider
    {
        public async Task<IEnumerable<string>> GetServersAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<string>();

                RegistryKey root = AutoLibLocal.TotalConfig.GetRootRegistryKey();
                using (var reg = root.OpenSubKey(@"Software\AutoBase\OpcData"))
                {
                    if (reg == null) return Enumerable.Empty<string>();
                    list.AddRange(reg.GetSubKeyNames());
                }

                return list.AsEnumerable();
            });
        }

        public async Task<IEnumerable<string>> GetGroupsAsync(string server)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(server))
                    return Enumerable.Empty<string>();

                var list = new List<string>();

                RegistryKey root = AutoLibLocal.TotalConfig.GetRootRegistryKey();
                using (var reg = root.OpenSubKey(
                    @"Software\AutoBase\OpcData\" + server))
                {
                    if (reg == null) return Enumerable.Empty<string>();
                    list.AddRange(reg.GetSubKeyNames());
                }

                return list.AsEnumerable();
            });
        }

        public async Task<IEnumerable<OpcBrowseItem>> GetItemsAsync(string server, string group)
        {
            return await Task.Run(() =>
            {
                var items = new List<OpcBrowseItem>();

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(group))
                    return Enumerable.Empty<OpcBrowseItem>();

                RegistryKey root = AutoLibLocal.TotalConfig.GetRootRegistryKey();
                using (var reg = root.OpenSubKey(
                    $@"Software\AutoBase\OpcData\{server}\{group}"))
                {
                    if (reg == null) return Enumerable.Empty<OpcBrowseItem>();

                    foreach (var sub in reg.GetSubKeyNames())
                    {
                        using (var r = reg.OpenSubKey(sub))
                        {
                            if (r == null) continue;

                            foreach (var name in r.GetValueNames())
                            {
                                items.Add(new OpcBrowseItem
                                {
                                    Name = name,
                                    Value = Convert.ToString(r.GetValue(name))
                                });
                            }
                        }
                    }
                }

                return items.AsEnumerable();
            });
        }
    }
}
