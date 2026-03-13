using AutoLibLocal;
using Microsoft.Extensions.Logging;
using NetTools;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Opc.Ua.Security.Certificates;
using OPCUA.Client.Core.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace OPCUA.Client.Core
{
    public class OPCUAClientMain
    {
        public static readonly List<OPCUAMember_server> Servers = new List<OPCUAMember_server>();
        public static bool bLoad = false;

        static private System.Windows.Forms.Timer _timerReconnect ;
        private static readonly SemaphoreSlim _tickLock = new SemaphoreSlim(1, 1);
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        private static readonly ITelemetryContext _telemetry =
            OpcUaClientTelemetry.Telemetry;

        private static readonly ILogger _logger =
            _telemetry.CreateLogger<OPCUAClientMain>();

        // UI thread marshal (Init 시점이 UI thread라면 캡처)
        private static SynchronizationContext _uiContext;

        public static Type ToSystemType(BuiltInType btype)
        {
            switch (btype)
            {
                case BuiltInType.Boolean: return typeof(bool);
                case BuiltInType.SByte: return typeof(sbyte);
                case BuiltInType.Byte: return typeof(byte);
                case BuiltInType.Int16: return typeof(short);
                case BuiltInType.UInt16: return typeof(ushort);
                case BuiltInType.Int32: return typeof(int);
                case BuiltInType.UInt32: return typeof(uint);
                case BuiltInType.Int64: return typeof(long);
                case BuiltInType.UInt64: return typeof(ulong);
                case BuiltInType.Float: return typeof(float);
                case BuiltInType.Double: return typeof(double);
                case BuiltInType.String: return typeof(string);
                case BuiltInType.DateTime: return typeof(DateTime);
                case BuiltInType.Guid: return typeof(Guid);
                case BuiltInType.ByteString: return typeof(byte[]);
                case BuiltInType.XmlElement: return typeof(System.Xml.XmlElement);
                case BuiltInType.NodeId: return typeof(NodeId);
                case BuiltInType.ExpandedNodeId: return typeof(ExpandedNodeId);
                case BuiltInType.StatusCode: return typeof(StatusCode);
                case BuiltInType.QualifiedName: return typeof(QualifiedName);
                case BuiltInType.LocalizedText: return typeof(LocalizedText);
                case BuiltInType.ExtensionObject: return typeof(ExtensionObject);
                case BuiltInType.DataValue: return typeof(DataValue);
                case BuiltInType.Variant: return typeof(Variant);
                case BuiltInType.DiagnosticInfo: return typeof(DiagnosticInfo);
                default: return typeof(object);
            }
        }

        public static void Init()
        {
            // WinForms UI thread에서 호출된다고 가정
            _uiContext = SynchronizationContext.Current;

            OpcUaClientConfigManager.Initialize(
                  AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData\\UaClient"
              );

            _logger.LogInformation("OPC UA Client initializing");

            try
            {
                _timerReconnect = new System.Windows.Forms.Timer();
                _timerReconnect.Tick += timer_reconnect_tick;
                _timerReconnect.Interval = 30000; //default : 30 sec
                _timerReconnect.Start();

                // 초기 즉시 1회 실행
                timer_reconnect_tick(_timerReconnect, EventArgs.Empty);

                OpcUaWriteService.Start();

            }
            catch
            {
                bLoad = false;
            }
        }

        public static void Uninit()
        {
            Debug.WriteLine("OPCUAClient.UnInit()");
            OpcUaClientConfigManager.Save();

            try { _cts.Cancel(); } catch { }
            bLoad = false;

            if (_timerReconnect != null)
            {
                try
                {
                    _timerReconnect.Stop();
                    _timerReconnect.Tick -= timer_reconnect_tick;
                }
                catch { }
                _timerReconnect = null;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await CleanupPreviousSessionAsync().ConfigureAwait(false);
                }
                catch { }
            });

            OpcUaWriteService.Stop();
        }

        static private async void timer_reconnect_tick(object sender, EventArgs e)
        {

            // Timer는 UI thread에서 실행 → 블로킹/중첩 방지
            if (!await _tickLock.WaitAsync(0).ConfigureAwait(false))
                return;

            try
            {
                if (!bLoad)
                {
                    _ = Task.Run(() => ConnectAllAsync(_cts.Token));
                    bLoad = true;
                    return;
                }

                bool enabled = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcClientReconnect", false);
                if (!enabled) return;

                if (Servers.Count == 0) return;

                foreach (var s in Servers)
                {
                    if (s == null) continue;
                    if (s.bServerAlive) continue;

                    Log.Write(AutoLibLocal.LogLevel.ERROR, LogCategory.COMMUNICATION,
                        string.Format("Try to reconnect OPC UA Client : {0}", s.accessName));

                    _ = Task.Run(async () =>
                    {
                        try { await s.ServerInitAsync(_cts.Token, _uiContext).ConfigureAwait(false); }
                        catch { }
                    });
                }
            }
            finally
            {
                _tickLock.Release();
            }
      
        }

        private static async Task ConnectAllAsync(CancellationToken ct)
        {
            if (Servers.Count == 0) return;

            foreach (var s in Servers)
            {
                if (ct.IsCancellationRequested) break;
                if (s == null) continue;

                try
                {
                    await s.ServerInitAsync(ct, _uiContext).ConfigureAwait(false);
                }
                catch
                {
                    // 개별 서버 실패는 전체를 막지 않음
                }
            }
        }

        private static async Task CleanupPreviousSessionAsync()
        {
            foreach (var s in Servers)
            {
                if (s == null) continue;
                try { await s.ServerUninitAsync().ConfigureAwait(false); }
                catch { }
            }
        }

        public static OPCUAMember_item FindItem(
            string serverName,
            string groupName,
            string itemName)
        {
            if (Servers == null || Servers.Count == 0)
                return null;

            foreach (var srv in Servers)
            {
                if (srv == null)
                    continue;

                if (!string.Equals(
                        srv.accessName,
                        serverName,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                if (srv.arrGroup == null)
                    return null;

                foreach (var grp in srv.arrGroup)
                {
                    if (grp == null)
                        continue;

                    if (!string.Equals(
                            grp.sName,
                            groupName,
                            StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (grp.arrItem == null)
                        return null;

                    foreach (var item in grp.arrItem)
                    {
                        if (item == null)
                            continue;

                        if (string.Equals(
                                item.sName,
                                itemName,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return item;
                        }
                    }
                }
            }

            return null;
        }

        public static void SaveConfig()
        {
            OpcUaClientConfigManager.Save();
        }

        public static bool TryGetUaData(
            string server,
            string group,
            string item,
            out DataValue dv)
        {
            dv = null;

            foreach (var s in Servers)
            {
                if (s == null || s.accessName != server) continue;

                foreach (var g in s.arrGroup)
                {
                    if (g == null || g.sName != group) continue;

                    foreach (var i in g.arrItem)
                    {
                        if (i == null || i.sName != item) continue;

                        dv = i.dvDataValue;
                        return dv != null;
                    }
                }
            }
            return false;
        }
    }


}
