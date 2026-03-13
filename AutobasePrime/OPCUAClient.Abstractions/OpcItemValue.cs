using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{
    public class OpcItemValue
    {
        public object Value { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsGood { get; set; }
    }
}
