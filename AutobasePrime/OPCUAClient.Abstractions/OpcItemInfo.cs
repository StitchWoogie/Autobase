using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{

    public class OpcItemInfo
    {
        public string Name { get; set; }      // Alias
        public string NodeId { get; set; }    // ns=2;s=...
        public bool IsArray { get; set; }
        public bool IsMatrix { get; set; }
    }
}
