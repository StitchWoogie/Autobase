using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAClient.Abstractions
{
    public class OpcWriteResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }

        public OpcWriteResult(bool success, string error = null)
        {
            Success = success;
            ErrorMessage = error;
        }
    }
}
