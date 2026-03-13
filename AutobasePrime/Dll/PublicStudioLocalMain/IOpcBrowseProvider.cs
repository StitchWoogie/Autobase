using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicStudioLocalMain
{
    public interface IOpcBrowseProvider
    {
        Task<IEnumerable<string>> GetServersAsync();
        Task<IEnumerable<string>> GetGroupsAsync(string server);
        Task<IEnumerable<OpcBrowseItem>> GetItemsAsync(string server, string group);
    }

    public class OpcBrowseItem
    {
        public string Name;
        public string Value;
    }
}
