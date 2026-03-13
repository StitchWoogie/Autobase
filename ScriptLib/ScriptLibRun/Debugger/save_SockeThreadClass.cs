using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace ScriptLibRun.Debugger
{
    class SocketThreadClass
    {
        public Socket socket;
        public Thread thread;
        public bool bClose;
    }
}
