using Opc.Ua;
using Opc.Ua.Client;
using OPCUA.Client.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPCUA.Client.Host
{
    public sealed class OpcUaWriteService : IDisposable
    {
        private readonly OpcUaRuntime _runtime;
        private readonly ConcurrentQueue<WriteRequest> _queue = new ConcurrentQueue<WriteRequest>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _worker;

        public OpcUaWriteService(OpcUaRuntime runtime)
        {
            _runtime = runtime;
            _worker = Task.Run(WorkerAsync);
        }

        public void Enqueue(string server, string nodeId, Variant value)
        {
            _queue.Enqueue(new WriteRequest
            {
                Server = server,
                NodeId = nodeId,
                Value = value
            });
            _signal.Release();
        }

        private async Task WorkerAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                await _signal.WaitAsync(_cts.Token).ConfigureAwait(false);

                if (!_queue.TryDequeue(out var req))
                    continue;

                await ExecuteAsync(req).ConfigureAwait(false);
            }
        }

        private async Task ExecuteAsync(WriteRequest req)
        {
            var status = await _runtime.WriteValueAsync(
                req.Server, req.NodeId, req.Value, CancellationToken.None);

            if (StatusCode.IsBad(status))
            {
                // retry / log
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }

    internal class WriteRequest
    {
        public string Server;
        public string NodeId;
        public Variant Value;
    }
}
