using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OPCUA.Client.Core;
using OpcUaClient.Ipc.Contracts;
using Newtonsoft.Json;

namespace OpcUa.Client.Ipc.Server
{
    public sealed class IpcServer
    {
        private readonly PipeListener _listener;
        private readonly ClientRegistry _clients;

        /// <summary>
        /// IPC 클라이언트(LocalMain 등)에서 Shutdown 명령을 수신했을 때 발생.
        /// </summary>
        public event Action ShutdownRequested;

        public IpcServer(OpcUaRuntime runtime, string pipeName)
        {
            _clients = new ClientRegistry();

            var dispatcher = new IpcDispatcher(runtime);
            dispatcher.OnShutdownRequested = () => ShutdownRequested?.Invoke();

            var bridge = new RuntimeEventBridge(runtime, _clients);

            _listener = new PipeListener(pipeName, dispatcher, _clients);
        }

        public void Start() => _listener.Start();

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
