using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{
    public class OpcWriteRequest
    {
        public string NodeId { get; }
        public object Value { get; }
        public int? ArrayIndex { get; }

        public OpcWriteRequest(string nodeId, object value, int? arrayIndex = null)
        {
            NodeId = nodeId;
            Value = value;
            ArrayIndex = arrayIndex;
        }
    }
}
