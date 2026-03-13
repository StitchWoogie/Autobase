using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcUa.Client.Ipc.Server
{
    internal sealed class PipeListener
    {
        private readonly string _pipeName;
        private readonly IpcDispatcher _dispatcher;
        private readonly ClientRegistry _clients;

        private volatile bool _running;
        private Thread _listenThread;

        public PipeListener(string pipeName,
                            IpcDispatcher dispatcher,
                            ClientRegistry clients)
        {
            _pipeName = pipeName;
            _dispatcher = dispatcher;
            _clients = clients;
        }

        public void Start()
        {
            if (_running)
                return;

            _running = true;

            _listenThread = new Thread(ListenLoop)
            {
                IsBackground = true,
                Name = "IPC Pipe Listener"
            };
            _listenThread.Start();
        }

        public void Stop()
        {
            _running = false;

            // WaitForConnection을 깨우기 위한 더미 연결
            try
            {
                using (var dummy = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
                {
                    dummy.Connect(50);
                }
            }
            catch
            {
                // ignore
            }

            // listener thread 종료 대기 (중요)
            try
            {
                if (_listenThread != null && _listenThread.IsAlive)
                    _listenThread.Join(1000);
            }
            catch { }
        }

        private void ListenLoop()
        {
            while (_running)
            {
                NamedPipeServerStream pipe = null;

                try
                {
                    pipe = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.WriteThrough);

                    pipe.WaitForConnection();

                    if (!_running)
                    {
                        pipe.Dispose();
                        break;
                    }

                    var ctx = new PipeClientContext(pipe, _dispatcher, _clients);
                    _clients.Add(ctx);

                    ctx.Start();
                }
                catch
                {
                    pipe?.Dispose();

                    if (!_running)
                        break;
                }
            }
        }
    }
}
