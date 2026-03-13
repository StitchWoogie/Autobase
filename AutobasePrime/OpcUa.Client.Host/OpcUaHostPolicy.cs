using OPCUA.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Host.Config
{
    public class OpcUaHostPolicy
    {
        public List<OPCUAMember_server> Servers { get; } =
            new List<OPCUAMember_server>();

        public bool AutoReconnect { get; set; } = true;
        public int ReconnectIntervalSec { get; set; } = 30;
    }
}
