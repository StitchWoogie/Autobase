using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine TCP 연결 관리.
    /// OpcUaClient.Ipc.Client.PipeConnection 패턴을 TCP 소켓으로 포팅.
    /// </summary>
    internal sealed class PythonAiTcpConnection : IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient _tcp;
        private NetworkStream _stream;
        private readonly PythonAiSerializer _serializer = new PythonAiSerializer();
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private volatile bool _disposed;

        /// <summary>메시지 수신 이벤트</summary>
        public event EventHandler<PythonAiMessage> MessageReceived;

        /// <summary>연결 끊김 이벤트</summary>
        public event EventHandler Disconnected;

        /// <summary>연결 상태</summary>
        public bool IsConnected
        {
            get
            {
                try { return _tcp != null && _tcp.Connected && !_disposed; }
                catch { return false; }
            }
        }

        public PythonAiTcpConnection(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>TCP 연결 (3초 타임아웃)</summary>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                _tcp = new TcpClient();
                _tcp.NoDelay = true;
                _tcp.ReceiveBufferSize = 65536;
                _tcp.SendBufferSize = 65536;

                var connectTask = _tcp.ConnectAsync(_host, _port);
                if (await Task.WhenAny(connectTask, Task.Delay(3000)).ConfigureAwait(false) != connectTask)
                {
                    DisposeTcpOnly();
                    return false;
                }

                if (connectTask.IsFaulted)
                {
                    DisposeTcpOnly();
                    return false;
                }

                _stream = _tcp.GetStream();

                // 수신 루프 시작
                ThreadPool.QueueUserWorkItem(_ => ReceiveLoop());

                return true;
            }
            catch
            {
                DisposeTcpOnly();
                return false;
            }
        }

        /// <summary>메시지 전송</summary>
        public async Task SendAsync(PythonAiMessage msg)
        {
            if (_disposed || _stream == null)
                throw new InvalidOperationException("Not connected");

            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await _serializer.WriteAsync(_stream, msg).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private async void ReceiveLoop()
        {
            try
            {
                while (!_disposed && _stream != null)
                {
                    var msg = await _serializer.ReadAsync(_stream).ConfigureAwait(false);
                    if (msg != null)
                    {
                        try { MessageReceived?.Invoke(this, msg); }
                        catch { }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                Debug.WriteLine("PythonAi: Connection closed (EOF)");
            }
            catch (IOException)
            {
                Debug.WriteLine("PythonAi: Connection closed (IO)");
            }
            catch (ObjectDisposedException)
            {
                // 정상 종료
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("PythonAi: ReceiveLoop error: {0}", ex.Message));
            }
            finally
            {
                if (!_disposed)
                {
                    DisposeTcpOnly();
                    try { Disconnected?.Invoke(this, EventArgs.Empty); }
                    catch { }
                }
            }
        }

        private void DisposeTcpOnly()
        {
            try { _stream?.Close(); } catch { }
            try { _tcp?.Close(); } catch { }
            _stream = null;
            _tcp = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            DisposeTcpOnly();
            _writeLock.Dispose();
        }
    }
}
