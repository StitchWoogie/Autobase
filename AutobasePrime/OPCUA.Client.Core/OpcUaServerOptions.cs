using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    public sealed class OpcUaServerOptions
    {
        public bool AutoAcceptUntrusted { get; set; } = false;
        public bool AutoTrustStore { get; set; } = true;
        public int DiscoverTimeoutMs { get; set; } = 15000;
        public uint SessionTimeoutMs { get; set; } = 60000;
        public string SessionName { get; set; } = "AutobaseOpcUaClient";

        /// <summary>
        /// User identity for session creation. null => Anonymous.
        /// </summary>
        public IUserIdentity UserIdentity { get; set; }
    }
}
