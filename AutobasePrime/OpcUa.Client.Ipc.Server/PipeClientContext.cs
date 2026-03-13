using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpcUaClient.Ipc.Contracts;

namespace OpcUa.Client.Ipc.Server
{
    internal sealed class PipeClientContext
    {
        private readonly NamedPipeServerStream _pipe;
        private readonly IpcDispatcher _dispatcher;
        private readonly ClientRegistry _clients;
        private readonly JsonLengthPrefixSerializer _serializer =
            new JsonLengthPrefixSerializer();

        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private volatile bool _disposed;

        public PipeClientContext(
            NamedPipeServerStream pipe,
            IpcDispatcher dispatcher,
            ClientRegistry clients)
        {
            _pipe = pipe;
            _dispatcher = dispatcher;
            _clients = clients;
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(ReceiveLoop);
        }

        private async void ReceiveLoop(object state)
        {
            try
            {
                while (true)
                {
                    var msg = await _serializer.ReadAsync(_pipe).ConfigureAwait(false);

                    var response = await _dispatcher.DispatchAsync(msg, this).ConfigureAwait(false);
                    if (response != null)
                    {
                        await _writeLock.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            await _serializer.WriteAsync(_pipe, response).ConfigureAwait(false);
                        }
                        finally
                        {
                            _writeLock.Release();
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
                // Dispose 중 파이프 읽기 시 발생
            }
            finally
            {
                DisposeInternal();
            }
        }

        public async void SendEvent(IpcMessage evt)
        {
            if (_disposed)
                return;

            try
            {
                await _writeLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    await _serializer.WriteAsync(_pipe, evt).ConfigureAwait(false);
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch
            {
                // send 실패 → ReceiveLoop에서 정리됨
            }
        }

        private void DisposeInternal()
        {
            if (_disposed)
                return;

            _disposed = true;

            try { _clients.Remove(this); } catch { }
            try { _pipe.Dispose(); } catch { }
            try { _writeLock?.Dispose(); } catch { }
        }
    }
}
