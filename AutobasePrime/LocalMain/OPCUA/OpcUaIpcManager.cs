using AutoLibLocal;
using NetTools;
using OpcUaClient.Ipc.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain.OPCUA
{
    /// <summary>
    /// Studio 도 동일하게 수정해야한다.
    /// </summary>
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
        private static bool _processStartAttempted;

        /// <summary>
        /// OPC UA Client 태그(cTagLinkType==5 &amp;&amp; flagOpcUAClient==1)가 존재하는 경우에만 시작한다.
        /// </summary>
        public static void Start()
        {
            if (_monitorTask != null)
                return;

            if (!HasOpcUaClientTag())
                return;

            _cts = new CancellationTokenSource();

            _monitorTask = Task.Run(() => MonitorLoop(_cts.Token));
        }

        /// <summary>
        /// 태그 목록에서 OPC UA Client 태그가 하나라도 있는지 확인한다.
        /// cTagLinkType == 5 (OPC) 이면서 flagOpcUAClient == 1 인 태그.
        /// </summary>
        private static bool HasOpcUaClientTag()
        {
            try
            {
                TagListStruct[] tagList = TagLib.tagListAll;
                if (tagList == null)
                    return false;

                for (int i = 0; i < tagList.Length; i++)
                {
                    TagPublicClass tp = tagList[i].ptr;
                    if (tp != null && tp.cTagLinkType == 5 && tp.flagOpcUAClient == 1)
                        return true;
                }
            }
            catch
            {
            }

            return false;
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

            // IPC 연결 실패 시, 최초 1회만 프로세스 실행을 시도한다.
            // 이후에는 워치독 등 외부 실행에 의존한다.
            if (!_processStartAttempted)
            {
                _processStartAttempted = true;
                TryStartProcessOnce();
                await Task.Delay(1500).ConfigureAwait(false); // 프로세스 초기화 대기
            }

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

        /// <summary>
        /// OPCUA Client 프로세스가 실행 중이 아니면 1회 실행한다.
        /// </summary>
        private static void TryStartProcessOnce()
        {
            try
            {
                string exePath = Path.Combine(Application.StartupPath, "Autobase_OPCUA_Client.exe");
                if (!File.Exists(exePath))
                {
                    Debug.WriteLine("[OpcUaIpcManager] Autobase_OPCUA_Client.exe not found.");
                    return;
                }

                Process[] procs = Process.GetProcessesByName("Autobase_OPCUA_Client");
                bool alreadyRunning = false;
                foreach (Process p in procs)
                {
                    try
                    {
                        if (p.MainModule.FileName.Equals(exePath, StringComparison.OrdinalIgnoreCase))
                        {
                            alreadyRunning = true;
                            break;
                        }
                    }
                    catch { }
                }

                if (!alreadyRunning)
                {
                    Process.Start(exePath);
                    Debug.WriteLine("[OpcUaIpcManager] OPCUA Client started.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[OpcUaIpcManager] Failed to start OPCUA Client: " + ex.Message);
            }
        }

        private static void SafeDisposeClient()
        {
            try { _client?.Dispose(); } catch { }
            _client = null;
        }




        private static void SafeDispose()
        {
            try { _client?.Dispose(); } catch { }
            _client = null;
            _connected = false;
        }


        /// <summary>
        /// OPCUA Client에 Shutdown 명령 전송 (트레이 상태에서도 종료).
        /// FormLocalMain_FormClosed에서 호출한다.
        /// Stop()과는 별개로 동작한다.
        /// </summary>
        public static void SendShutdown()
        {
            try
            {
                var client = Client;
                if (client != null)
                {
                    client.SendShutdownAsync()
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }
            catch { }
        }

        public static void Stop()
        {
            _cts?.Cancel();
            _monitorTask = null;
            _processStartAttempted = false;

            SafeDispose();
            OpcUaClientCache.Clear();
        }
    }
}
