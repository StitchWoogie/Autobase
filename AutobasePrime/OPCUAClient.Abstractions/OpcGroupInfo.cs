using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{
    public class OpcGroupInfo
    {
        public string Name { get; set; }
        public int Interval { get; set; }
        public IReadOnlyList<OpcItemInfo> Items { get; set; }
    }
}
