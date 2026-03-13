using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcClient
{
    public sealed class BrowseNodeResult
    {
        public string NodeId { get; set; }
        public string DisplayName { get; set; }
        public string DataType { get; set; }
        public bool IsFolder { get; set; }
    }
}
