using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA.Client.Core
{
    public sealed class RuntimeReadResult
    {
        public object Value;
        public int Quality;
        public DateTime Timestamp;
        public bool Success;
        public string Error;
    }
}
