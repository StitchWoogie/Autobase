using OPCUA.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{
    public interface IOpcUaClientEngine
    {
        bool IsConnected(OPCUAMember_server server);

        Task ConnectAsync(OPCUAMember_server server);
        Task DisconnectAsync(OPCUAMember_server server);

        Task RecreateGroupAsync(OPCUAMember_group group);

        Task<OpcWriteResult> WriteAsync(
              OPCUAMember_server server,
              OpcWriteRequest request);
    }
}
