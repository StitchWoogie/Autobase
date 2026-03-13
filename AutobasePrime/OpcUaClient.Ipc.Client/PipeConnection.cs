using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Client
{
    internal sealed class PipeConnection : IDisposable
    {
        private readonly string _pipeName;
        private NamedPipeClientStream _pipe;
        private readonly JsonLengthPrefixSerializer _serializer =
            new JsonLengthPrefixSerializer();

        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private volatile bool _disposed;

        public event EventHandler<IpcMessage> MessageReceived;
        public event EventHandler Disconnected;

        public PipeConnection(string pipeName)
        {
            _pipeName = pipeName;
        }

        public bool IsConnected => _pipe != null && _pipe.IsConnected;

        public bool Connect()
        {
            DisposePipeOnly();

            try
            {
                _pipe = new NamedPipeClientStream(
                    ".", _pipeName,
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);

                _pipe.Connect(3000);

                ThreadPool.QueueUserWorkItem(ReceiveLoop);
                return true;
            }
            catch (TimeoutException)
            {
                return false; // 서버 아직 없음 (정상)
            }
            catch (IOException)
            {
                return false; // 파이프 없음 (정상)
            }
        }

        private async void ReceiveLoop(object state)
        {
            try
            {
                while (!_disposed)
                {
                    var msg = await _serializer.ReadAsync(_pipe).ConfigureAwait(false);
                    MessageReceived?.Invoke(this, msg);
                }
            }
            catch (EndOfStreamException)
            {
                // 정상 종료(상대 종료/재시작)
            }
            catch (IOException)
            {
                // 파이프 끊김
            }
            catch (ObjectDisposedException)
            {
                // Dispose 중
            }
            catch
            {
                // 기타: 로그만
            }
            finally
            {
                DisposePipeOnly();
                if (!_disposed)
                    Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task SendAsync(IpcMessage msg)
        {
            if (_disposed || _pipe == null || !_pipe.IsConnected)
                throw new IOException("Pipe not connected");

            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await _serializer.WriteAsync(_pipe, msg).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private void DisposePipeOnly()
        {
            try { _pipe?.Dispose(); } catch { }
            _pipe = null;
        }

        public void Dispose()
        {
            _disposed = true;
            DisposePipeOnly();
            _writeLock?.Dispose();
        }
    }
}
