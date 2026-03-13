using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{
    public class OpcServerInfo
    {
        public string Name { get; set; }          // AccessName
        public string Url { get; set; }           // opc.tcp://...
        public string Description { get; set; }   // Optional
        public IReadOnlyList<OpcGroupInfo> Groups { get; set; }
    }
}
