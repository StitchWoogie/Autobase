using Microsoft.Extensions.Logging;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    public static class OpcUaClientTelemetry
    {
        public static readonly ITelemetryContext Telemetry =
            DefaultTelemetry.Create(builder =>
            {
                builder.AddDebug();
                // builder.AddConsole(); // 콘솔 앱일 때만
            });
    }
}
