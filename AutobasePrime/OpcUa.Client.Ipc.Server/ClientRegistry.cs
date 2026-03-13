using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Ipc.Server
{
    internal sealed class ClientRegistry
    {
        private readonly List<PipeClientContext> _clients =
            new List<PipeClientContext>();

        public void Add(PipeClientContext client)
        {
            lock (_clients)
            {
                _clients.Add(client);
            }
        }

        public void Remove(PipeClientContext client)
        {
            lock (_clients)
            {
                _clients.Remove(client);
            }
        }

        public PipeClientContext[] Snapshot()
        {
            lock (_clients)
            {
                return _clients.ToArray();
            }
        }
    }
}
