using NetTools;
using OpcUaClient.Ipc.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studio.OPCUA
{
    internal static class OpcUaIpcManager
    {
        private static IpcClient _client;

        private static Task _monitorTask;
        private static CancellationTokenSource _cts;
        private static readonly object _lock = new object();

        public static IpcClient Client
        {
            get
            {
                if (_client == null || !_client.IsConnected)
                    return null;
                return _client;
            }
        }

        private static bool _connected;

        public static void Start()
        {
            if (_monitorTask != null)
                return;

            _cts = new CancellationTokenSource();

            _monitorTask = Task.Run(() => MonitorLoop(_cts.Token));
        }

        private static async Task MonitorLoop(CancellationToken ct)
        {
            int delayMs = 500;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (Client == null)
                    {
                        bool connected = await EnsureConnectedOnceAsync();
                        // 연결 성공/실패에 따라 backoff 조절
                        delayMs = (Client != null) ? 500 : Math.Min(delayMs * 2, 5000);
                    }
                    else
                    {
                        delayMs = 500;
                    }
                }
                catch
                {
                    delayMs = Math.Min(delayMs * 2, 5000);
                }

                try { await Task.Delay(delayMs, ct); }
                catch { }
            }
        }

        private static async Task<bool> EnsureConnectedOnceAsync()
        {

            if (Client != null)
                return true;


                TryStartUiProcess(); // 프로세스가 없으면 실행 (트레이여도 OK)

            IpcClient newClient = null;

            try
            {
                newClient = new IpcClient("Autobase.OPCUA.Client.Runtime");

                newClient.TagValueChanged += (s, e) =>
                {
                    try { OpcUaClientCache.Update(e); } catch { }
                };

                newClient.Disconnected += (s, e) =>
                {
                    lock (_lock)
                    {
                        if (ReferenceEquals(_client, s))
                            SafeDisposeClient();
                    }
                };

                bool ok = await newClient.ConnectAsync().ConfigureAwait(false);
                if (!ok)
                {
                    newClient.Dispose();
                    return false;
                }

                lock (_lock)
                {
                    if (_client != null)
                    {
                        newClient.Dispose();
                        return true;
                    }

                    _client = newClient;
                    return true;
                }
            }
            catch
            {
                try { newClient?.Dispose(); } catch { }
                return false;
            }

        }

        private static void SafeDisposeClient()
        {
            try { _client?.Dispose(); } catch { }
            _client = null;
        }


        private static void TryStartUiProcess()
        {
            try
            {
                string clientPath = Application.StartupPath + "\\Autobase_OPCUA_Client.exe";
                if (!File.Exists(clientPath))
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show(clientPath, "실행 파일이 존재하지 않습니다.");
                    else
                        MessageBox.Show(clientPath, "The exe file does not exist.");
                    return;
                }

                bool alreadyRunning = false;

                foreach (var p in Process.GetProcessesByName("Autobase_OPCUA_Client"))
                {
                    try
                    {
                        string processPath = p.MainModule.FileName;
                        if (processPath.Equals(clientPath, StringComparison.OrdinalIgnoreCase))
                        {
                            //  트레이 / 숨김 / 윈도우 없음 모두 정상
                            alreadyRunning = true;
                            break;
                        }
                    }
                    catch
                    {
                        // 접근 불가 프로세스 → 무시
                    }
                }

                if (!alreadyRunning)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = clientPath,
                        UseShellExecute = true
                    });
                }
            }
            catch
            {
                // 로그만
            }
        }


        private static void SafeDispose()
        {
            try { _client?.Dispose(); } catch { }
            _client = null;
            _connected = false;
        }


        /// <summary>
        /// 프로세스를 실행하지 않고, IPC 연결만 한 번 시도한다.
        /// 이미 실행 중인 OPC UA Client에 연결할 때 사용.
        /// </summary>
        public static async Task<bool> TryConnectOnceAsync(int timeoutMs = 3000)
        {
            if (Client != null)
                return true;

            IpcClient newClient = null;

            try
            {
                newClient = new IpcClient("Autobase.OPCUA.Client.Runtime");

                newClient.TagValueChanged += (s, e) =>
                {
                    try { OpcUaClientCache.Update(e); } catch { }
                };

                newClient.Disconnected += (s, e) =>
                {
                    lock (_lock)
                    {
                        if (ReferenceEquals(_client, s))
                            SafeDisposeClient();
                    }
                };

                var connectTask = newClient.ConnectAsync();
                var completed = await Task.WhenAny(
                    connectTask,
                    Task.Delay(timeoutMs)).ConfigureAwait(false);

                if (completed != connectTask || !connectTask.Result)
                {
                    newClient.Dispose();
                    return false;
                }

                lock (_lock)
                {
                    if (_client != null)
                    {
                        newClient.Dispose();
                        return true;
                    }

                    _client = newClient;
                    return true;
                }
            }
            catch
            {
                try { newClient?.Dispose(); } catch { }
                return false;
            }
        }

        public static void Stop()
        {
            _cts?.Cancel();
            _monitorTask = null;

            SafeDispose();
            OpcUaClientCache.Clear();
        }
    }
}
