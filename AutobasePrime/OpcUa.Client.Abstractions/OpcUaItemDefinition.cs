using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Abstractions
{
    public sealed class OpcUaItemDefinition
    {
        public string Name;
        public string NodeId;
        public string InitialValue;
        /// <summary>
        /// OPC UA Client 종료 시 저장된 최종 런타임 값.
        /// Studio 오프라인 모드에서 표시용으로 사용.
        /// </summary>
        public string LastValue;
    }
}
