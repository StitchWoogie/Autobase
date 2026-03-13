using AutoLibLocal;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Client.Abstractions;
using OpcUa.Client.Host;
using OPCUA.Client.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpcUa.Client.Host.OpcUaHost;

namespace OPCUA.Client.UI
{
    public partial class FormOpcUaClient : Form
    {
        private readonly OpcUaHost _host;
        private SynchronizationContext _ui;
        private BindingList<OPCUAMember_item> _gridBinding;

        private ContextMenuStrip ctxServer;
        private ContextMenuStrip ctxGroup;
        private ContextMenuStrip ctxItem;

        private ImageList _treeImages;

        private bool _forceExit = false;

        private enum LogCol
        {
            Time = 0,
            Level,
            Server,
            Message
        }

        private readonly Dictionary<OPCUAMember_item, DataGridViewRow>
    _rowMap = new Dictionary<OPCUAMember_item, DataGridViewRow>();


        /// <summary>
        /// Designer 전용 기본 생성자.
        /// 런타임에서는 사용하지 않는다.
        /// </summary>
        public FormOpcUaClient()
        {
            InitializeComponent();
        }

        public FormOpcUaClient(OpcUaHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));

            InitializeComponent();

            InitTreeImages();
            InitLogView();
            InitTrayIcon();

            _ui = SynchronizationContext.Current;

            // ===== Server 메뉴 =====
            ctxServer = new ContextMenuStrip();
            ctxServer.Items.Add("Add Server", null, OnAddServer);
            ctxServer.Items.Add("Modify Server", null, OnModifyServer);       
            ctxServer.Items.Add("Remove Server", null, OnRemoveServer);
            ctxServer.Items.Add(new ToolStripSeparator());
            ctxServer.Items.Add("Add Group", null, OnAddGroup);

            var miConnect = new ToolStripMenuItem("Connect Server", null, OnConnectServer)
            {
                Name = "miConnectServer"
            };
            ctxServer.Items.Add(miConnect);

            // ===== Group 메뉴 =====
            ctxGroup = new ContextMenuStrip();
            //ctxGroup.Items.Add("Add Group", null, OnAddGroup);
            ctxGroup.Items.Add("Modify Group", null, OnModifyGroup);
            ctxGroup.Items.Add("Remove Group", null, OnRemoveGroup);
            ctxGroup.Items.Add(new ToolStripSeparator());
            ctxGroup.Items.Add("Add Item", null, OnAddItem);



            // ===== Item 메뉴 =====
            ctxItem = new ContextMenuStrip();
            //ctxItem.Items.Add("Add Item", null, OnAddItem);
            ctxItem.Items.Add("Modify Item", null, OnModifyItem);
            ctxItem.Items.Add("Remove Item", null, OnRemoveItem);
            ctxItem.Items.Add(new ToolStripSeparator());
            ctxItem.Items.Add("Write Item", null, OnWriteItem);

            _host.HostStateChanged += OnHostStateChanged;
            _host.ServersChanged += OnServersChanged;
            _host.ShutdownRequested += OnRemoteShutdown;
        }

        private void FormOpcUaClient_Load(object sender, EventArgs e)
        {
            // Designer 모드에서는 초기화 건너뛰기
            if (DesignMode || _host == null) return;

            WatchDogInfo.Init();

            _ui = SynchronizationContext.Current;

            _gridBinding = new BindingList<OPCUAMember_item>();
            gridRealtime.DataSource = _gridBinding;

            OpcUaUiLogBridge.Attach(AddLog);

            // 서버 리스트 UI treeSources 초기화
            InitGrid();

            foreach (var srv in _host.Servers)
                AttachServerEvents(srv);

            BuildTree(expandAll: false);

            // Start Minimized to Tray 설정 로드
            int startMinimized = TotalConfig.LoadRegAutoBaseConfig(
                "Config", "Start", "OpcUaClientStartMinimizedToTray", 1);
            startMinimizedToTrayToolStripMenuItem.Checked = (startMinimized == 1);

            if (startMinimized == 1)
            {
                // 트레이로 숨김
                BeginInvoke(new Action(() =>
                {
                    Hide();
                    notifyIcon1.ShowBalloonTip(
                        1000,
                        "Autobase OPCUA Client",
                        "Started minimized to tray",
                        ToolTipIcon.Info);
                }));
            }
        }

        #region Initialize listview, treeviewImgage
        private void InitTreeImages()
        {
            _treeImages = new ImageList();
            _treeImages.ImageSize = new Size(16, 16);

            _treeImages.Images.Add("server_disconnected", Properties.Resources.RedCircle);
            _treeImages.Images.Add("server_connected", Properties.Resources.GreenCircle);
            _treeImages.Images.Add("server_connecting", Properties.Resources.YellowCircle);

            _treeImages.Images.Add("group", Properties.Resources.Folder);
            _treeImages.Images.Add("item", Properties.Resources.Tag);

            treeSources.ImageList = _treeImages;
        }

        private void InitLogView()
        {
            listLog.View = View.Details;
            listLog.FullRowSelect = true;
            listLog.GridLines = true;
            listLog.HideSelection = false;

            listLog.Columns.Clear();

            listLog.Columns.Add("Time", 110);
            listLog.Columns.Add("Level", 60);
            listLog.Columns.Add("Server", 120);
            listLog.Columns.Add("Message", 500);
        }

        private void InitTrayIcon()
        {
            var ctxTray = new ContextMenuStrip();
            ctxTray.Items.Add("Open", null, OnTrayOpen);
            ctxTray.Items.Add(new ToolStripSeparator());
            ctxTray.Items.Add("Exit", null, OnTrayExit);

            notifyIcon1.ContextMenuStrip = ctxTray;
            notifyIcon1.DoubleClick += OnTrayDoubleClick;
        }
        #endregion 

        private void BuildTree(bool expandAll = true)
        {
            // 현재 선택된 노드의 Tag를 기억 (트리 재생성 후 다시 선택하기 위해)
            object selectedTag = treeSources.SelectedNode?.Tag;

            treeSources.BeginUpdate();
            treeSources.Nodes.Clear();

            TreeNode nodeToSelect = null;

            foreach (var srv in _host.Servers)
            {

                string serverImageKey;

                if (srv.bConnecting)
                    serverImageKey = "server_connecting";
                else if (srv.bServerAlive)
                    serverImageKey = "server_connected";
                else
                    serverImageKey = "server_disconnected";

                var nServer = new TreeNode(srv.DisplayText)//= new TreeNode(srv.accessName)
                {
                    Tag = srv,
                    ImageKey = serverImageKey,
                    SelectedImageKey = serverImageKey
                };

                if (srv == selectedTag)
                    nodeToSelect = nServer;

                foreach (var grp in srv.arrGroup)
                {
                    var nGroup = new TreeNode(grp.sName)
                    {
                        Tag = grp,
                        ImageKey = "group",
                        SelectedImageKey = "group"
                    };

                    if (grp == selectedTag)
                        nodeToSelect = nGroup;

                    foreach (var item in grp.arrItem)
                    {
                        var nItem = new TreeNode(item.sName)
                        {
                            Tag = item,
                            ImageKey = "item",
                            SelectedImageKey = "item"
                        };

                        if (item == selectedTag)
                            nodeToSelect = nItem;

                        nGroup.Nodes.Add(nItem);
                    }

                    nServer.Nodes.Add(nGroup);
                }

                treeSources.Nodes.Add(nServer);
            }

            if (expandAll)
            {
                treeSources.ExpandAll();
            }
            else
            {
                // 서버 노드만 펼침 (그룹/아이템은 접힌 상태)
                foreach (TreeNode serverNode in treeSources.Nodes)
                    serverNode.Expand();
            }

            // 이전에 선택된 노드를 다시 선택 (선택이 없으면 첫 번째 노드)
            if (nodeToSelect != null)
                treeSources.SelectedNode = nodeToSelect;
            else if (treeSources.Nodes.Count > 0)
                treeSources.SelectedNode = treeSources.Nodes[0];

            treeSources.EndUpdate();
        }

        /// <summary>
        /// 현재 트리 선택 기준으로 gridRealtime을 갱신한다.
        /// BuildTree 후 호출하여 Grid 동기화를 보장한다.
        /// </summary>
        private void RefreshGridFromSelection()
        {
            var node = treeSources.SelectedNode;
            if (node == null)
            {
                // 선택 없으면 Grid 비움
                _suppressTreeSelection = true;
                try
                {
                    _gridBinding?.Clear();
                    _rowMap?.Clear();
                }
                finally { _suppressTreeSelection = false; }
                return;
            }

            if (node.Tag is OPCUAMember_server srv)
                ShowServerItems(srv);
            else if (node.Tag is OPCUAMember_group grp)
                ShowGroupItems(grp);
            else if (node.Tag is OPCUAMember_item item)
            {
                var parentGrp = node.Parent?.Tag as OPCUAMember_group;
                if (parentGrp != null)
                    ShowGroupItems(parentGrp);
            }
        }

        private bool _suppressGridSelection = false;
        private bool _suppressTreeSelection = false;

        private void treeSources_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is OPCUAMember_server srv)
            {
                ShowServerItems(srv);
            }
            else if (e.Node?.Tag is OPCUAMember_group grp)
            {
                ShowGroupItems(grp);
            }
            else if (e.Node?.Tag is OPCUAMember_item item)
            {
                var itemGrp = e.Node.Parent?.Tag as OPCUAMember_group;
                if (itemGrp != null)
                    ShowGroupItems(itemGrp);

                // Grid에서 해당 아이템 선택
                SelectGridRowByItem(item);
            }
        }

        private void SelectGridRowByItem(OPCUAMember_item item)
        {
            if (item == null || _suppressGridSelection)
                return;

            _suppressGridSelection = true;
            try
            {
                if (_rowMap.TryGetValue(item, out var row))
                {
                    gridRealtime.ClearSelection();
                    row.Selected = true;

                    // 스크롤 인덱스 유효성 검사
                    if (row.Index >= 0 && row.Index < gridRealtime.Rows.Count && row.Displayed == false)
                    {
                        try
                        {
                            gridRealtime.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                        catch
                        {
                            // 스크롤 실패 시 무시
                        }
                    }
                }
            }
            finally
            {
                _suppressGridSelection = false;
            }
        }

        private void ShowServerItems(OPCUAMember_server srv)
        {
            if (srv == null)
                return;

            var list = new List<OPCUAMember_item>();

            if (srv.arrGroup != null)
            {
                foreach (var g in srv.arrGroup)
                {
                    if (g?.arrItem != null)
                        list.AddRange(g.arrItem);
                }
            }

            ReplaceItems(list);
        }


        private void ShowGroupItems(OPCUAMember_group grp)
        {
            //_gridBinding = new BindingList<OPCUAMember_item>(grp.arrItem);
            //gridRealtime.DataSource = _gridBinding;
            ReplaceItems(grp.arrItem);
        }

        private void ReplaceItems(IEnumerable<OPCUAMember_item> items)
        {
            // SelectionChanged 이벤트 차단
            _suppressTreeSelection = true;
            try
            {
                gridRealtime.SuspendLayout();

                _gridBinding.Clear();
                _rowMap.Clear();

                foreach (var item in items)
                {
                    HookItem(item);
                    _gridBinding.Add(item);

                    // Add 직후 Row가 생성된다
                    var rowIndex = _gridBinding.Count - 1;
                    var row = gridRealtime.Rows[rowIndex];

                    _rowMap[item] = row;
                }

                gridRealtime.ResumeLayout();
            }
            finally
            {
                _suppressTreeSelection = false;
            }
        }

        private void HookItem(OPCUAMember_item item)
        {
            if (item == null) return;

            item.Updated -= OnItemUpdated;
            item.Updated += OnItemUpdated;
        }

        private void OnItemUpdated(object sender, EventArgs e)
        {
            var item = sender as OPCUAMember_item;
            if (item == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(OnItemUpdated), sender, e);
                return;
            }

            if (!_rowMap.TryGetValue(item, out var row))
                return;

            // 해당 Row만 다시 그리기
            gridRealtime.InvalidateRow(row.Index);
        }

        private void InitGrid()
        {
            gridRealtime.AutoGenerateColumns = false;
            gridRealtime.ReadOnly = true;
            gridRealtime.AllowUserToAddRows = false;
            gridRealtime.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridRealtime.Columns.Clear();

            gridRealtime.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Item ID",
                DataPropertyName = "sNode",
                Width = 100
            });

            gridRealtime.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Item Name",
                DataPropertyName = "sName",
                Width = 100
            });

            gridRealtime.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Value",
                DataPropertyName = "sValue",
                Width = 200
            });

            gridRealtime.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Type",
                DataPropertyName = "DataType",
                Width = 120
            });

            gridRealtime.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Quality",
                DataPropertyName = "QualityString",
                Width = 90
            });

            gridRealtime.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Timestamp",
                DataPropertyName = "LastTimestampLocal",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "yyyy-MM-dd HH:mm:ss"
                }
            });

            gridRealtime.SelectionChanged += GridRealtime_SelectionChanged;
        }


        private void gridRealtime_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var item = gridRealtime.Rows[e.RowIndex].DataBoundItem as OPCUAMember_item;
            if (item == null) return;

            var style = gridRealtime.Rows[e.RowIndex].DefaultCellStyle;

            switch (item.Quality)
            {
                case OpcQuality.Bad:
                    style.BackColor = Color.FromArgb(255, 220, 220);
                    style.ForeColor = Color.DarkRed;
                    break;
                case OpcQuality.Uncertain:
                    style.BackColor = Color.FromArgb(255, 240, 200);
                    style.ForeColor = Color.DarkOrange;
                    break;
                default:
                    style.BackColor = Color.Black;
                    style.ForeColor = Color.Lime;
                    break;
            }

            //if (StatusCode.IsBad(item.LastStatusCode))
            //{
            //    style.BackColor = Color.FromArgb(255, 220, 220);
            //    style.ForeColor = Color.DarkRed;
            //}
            //else if (StatusCode.IsUncertain(item.LastStatusCode))
            //{
            //    style.BackColor = Color.FromArgb(255, 240, 200);
            //    style.ForeColor = Color.DarkOrange;
            //}
            //else
            //{
            //    style.BackColor = Color.Black;
            //    style.ForeColor = Color.Lime;
            //}
        }


        public static OPCUAMember_group AddGroup(
            OPCUAMember_server server,
            string groupName,
            int publishingIntervalMs)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            var group = new OPCUAMember_group(server, groupName, publishingIntervalMs);
            server.arrGroup.Add(group);

            return group;
        }

        //var grp = AddGroup(
        //        srv,
        //    groupName: "gr000",
        //    publishingIntervalMs: 1000
        //);


        private OPCUAMember_group _currentGroup;

        /// <summary>비동기 버튼 중복 클릭 방지 플래그</summary>
        private bool _asyncBusy;

        /// <summary>
        /// 비동기 작업 시작 시 호출. 중복 클릭을 방지한다.
        /// 이미 작업 중이면 false를 반환한다.
        /// </summary>
        private bool BeginAsyncOp()
        {
            if (_asyncBusy) return false;
            _asyncBusy = true;
            return true;
        }

        /// <summary>비동기 작업 종료 시 호출.</summary>
        private void EndAsyncOp()
        {
            _asyncBusy = false;
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnAddServer(sender, e);
        }


        private void treeSources_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            treeSources.SelectedNode = e.Node;

            if (e.Node.Tag is OPCUAMember_server srv)
            {
                var mi = ctxServer.Items["miConnectServer"] as ToolStripMenuItem;

                if (mi != null)
                {
                    mi.Text = srv.bServerAlive ? "Disconnect Server" : "Connect Server";
                }

                ctxServer.Show(treeSources, e.Location);
            }
            else if (e.Node.Tag is OPCUAMember_group)
            {
                ctxGroup.Show(treeSources, e.Location);
            }
            else if (e.Node.Tag is OPCUAMember_item)
            {
                ctxItem.Show(treeSources, e.Location);
            }
        }

        #region menu action
        private async void OnAddServer(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                using (var dlg = new FormAddServer(_host.UaConfig))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;

                    var def = new OpcUaServerDefinition
                    {
                        ServerName = dlg.ServerName,
                        ServerDisplayName = dlg.ServerDisplayName,
                        HostName = dlg.DiscoveryUrl,       // 사용자가 입력한 원본 URL (예: opc.tcp://127.0.0.1:43344/)
                        AccessName = dlg.AccessName,
                        EndpointIndex = dlg.EndpointIndex,
                        EndpointUrl = dlg.EndpointUrl,
                        SecurityPolicyUri = dlg.SecurityPolicyUri,
                        SecurityMode = dlg.SecurityModeValue,
                        AuthMode = dlg.AuthMode,
                        AuthUserName = dlg.AuthUserName,
                        AuthPasswordProtected = dlg.AuthPasswordProtected,
                        AuthCertificateThumbprint = dlg.AuthCertificateThumbprint,
                        AuthCertificateFilePath = dlg.AuthCertificateFilePath,
                        AuthCertPasswordProtected = dlg.AuthCertPasswordProtected,
                        SaveCredentials = dlg.SaveCredentials,
                    };

                    try
                    {
                        await _host.AddServerAsync(def);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Add Server Failed");
                    }
                }
            }
            finally { EndAsyncOp(); }
        }

        private async void OnModifyServer(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var srv = treeSources.SelectedNode?.Tag as OPCUAMember_server;
                if (srv == null)
                    return;

                // 현재 서버의 인증 정보를 ConfigManager에서 가져옴
                var currentDefs = OpcUaClientConfigManager.LoadServerDefinitions();
                var currentDef = currentDefs.FirstOrDefault(
                    d => string.Equals(d.AccessName, srv.accessName, StringComparison.OrdinalIgnoreCase));

                using (var dlg = new FormAddServer(_host.UaConfig))
                {
                    dlg.SetModify(
                        srv.serverName,
                        srv.hostName,
                        srv.accessName,
                        srv.endpointindex,
                        currentDef?.AuthMode ?? OpcUaAuthMode.Anonymous,
                        currentDef?.AuthUserName,
                        currentDef?.AuthCertificateThumbprint,
                        currentDef?.AuthCertificateFilePath,
                        (currentDef?.SaveCredentials == true) ? currentDef?.AuthPasswordProtected : null
                    );

                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;

                    try
                    {
                        await _host.ModifyServerAsync(
                            srv.accessName,
                            def =>
                            {
                                def.ServerName = dlg.ServerName;
                                def.ServerDisplayName = dlg.ServerDisplayName;
                                def.HostName = dlg.DiscoveryUrl;       // 사용자가 입력한 원본 URL
                                // def.AccessName = dlg.AccessName;      // 가능하면 변경 금지 정책이면 여기 막아도 됨
                                def.EndpointIndex = dlg.EndpointIndex;
                                def.EndpointUrl = dlg.EndpointUrl;
                                def.SecurityPolicyUri = dlg.SecurityPolicyUri;
                                def.SecurityMode = dlg.SecurityModeValue;
                                def.AuthMode = dlg.AuthMode;
                                def.AuthUserName = dlg.AuthUserName;
                                def.AuthPasswordProtected = dlg.AuthPasswordProtected;
                                def.AuthCertificateThumbprint = dlg.AuthCertificateThumbprint;
                                def.AuthCertificateFilePath = dlg.AuthCertificateFilePath;
                                def.AuthCertPasswordProtected = dlg.AuthCertPasswordProtected;
                                def.SaveCredentials = dlg.SaveCredentials;
                            });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Modify Server Failed");
                    }

                }
            }
            finally { EndAsyncOp(); }
        }

        private async void OnConnectServer(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var srv = treeSources.SelectedNode?.Tag as OPCUAMember_server;
                if (srv == null) return;

                bool ok;

                if (srv.bServerAlive)
                    ok = await _host.DisconnectServerAsync(srv);
                else
                    ok = await _host.ConnectServerAsync(srv);

                if (!ok)
                {
                    MessageBox.Show(
                        "Failed to connect to the server.",
                        "OPC UA",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally { EndAsyncOp(); }
        }


        private void AttachServerEvents(OPCUAMember_server srv)
        {
            if (srv == null)
                return;

            // 중복 등록 방지
            srv.StateChanged -= OnServerStateChanged;
            srv.StateChanged += OnServerStateChanged;
        }

        private void OnServerStateChanged()
        {
            // OPC UA 스레드 → UI 스레드 마샬링
            if (_ui != null)
                _ui.Post(_ => BuildTree(), null);
            else
                BuildTree();
        }

        private async void OnRemoveServer(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var srv = treeSources.SelectedNode.Tag as OPCUAMember_server;
                if (srv == null)
                    return;

                if (MessageBox.Show(
                    $"Remove server '{srv.accessName}'?",
                    "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                await _host.RemoveServerAsync(srv.accessName);
            }
            finally { EndAsyncOp(); }
        }


        //===================Group =============================
        private async void OnAddGroup(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var srv = treeSources.SelectedNode.Tag as OPCUAMember_server;
                if (srv == null)
                    return;

                using (var dlg = new FormAddGroup())
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;

                    try
                    {
                        await _host.AddGroupAsync(
                            srv.accessName,
                            dlg.GroupName,
                            dlg.PublishingInterval);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Add Group Failed");
                    }

                }
            }
            finally { EndAsyncOp(); }
        }
        private void OnModifyGroup(object sender, EventArgs e)
        {
            var grp = treeSources.SelectedNode?.Tag as OPCUAMember_group;
            if (grp == null)
                return;

            var srv = grp.Server;
            if (srv == null) return;

            using (var dlg = new FormAddGroup())
            {
                dlg.SetModify(grp.sName, grp.nInterval);

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    _host.ModifyGroup(
                       srv.accessName,
                       grp.sName,
                       g =>
                       {
                           g.GroupName = dlg.GroupName;
                           g.PublishingInterval = dlg.PublishingInterval;
                       });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Modify Group Failed");
                }
            }
        }


        private async void OnRemoveGroup(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var grp = treeSources.SelectedNode?.Tag as OPCUAMember_group;
                if (grp == null) return;

                var srv = grp.Server;
                if (srv == null) return;

                if (MessageBox.Show(
                       "Remove group '" + grp.sName + "'?",
                       "Confirm",
                       MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                try
                {
                    await _host.RemoveGroupAsync(srv.accessName, grp.sName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Remove Group Failed");
                }
            }
            finally { EndAsyncOp(); }
        }

        //=====================Item ==========================


        private async void OnAddItem(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var grp = treeSources.SelectedNode?.Tag as OPCUAMember_group;
                if (grp == null) return;

                var srv = grp.Server;
                if (srv == null) return;

                if (srv.seServer == null || !srv.seServer.Connected)
                {
                    MessageBox.Show("Server is not connected.");
                    return;
                }

                using (var dlg = new FormAddItem(
                    grp.arrItem.Select(i => i.sNode),
                    grp.arrItem.Select(i => i.sName)))
                {
                    dlg.SetSession(grp.Server.seServer);

                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;


                    try
                    {
                        foreach (var it in dlg.ResultItems)
                        {
                            await _host.AddItemAsync(
                                srv.accessName,
                                grp.sName,
                                new OpcUaItemDefinition
                                {
                                    Name = it.Alias,
                                    NodeId = it.NodeId,
                                    InitialValue = ""
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Add Item Failed");
                    }
                }
            }
            finally { EndAsyncOp(); }
        }

        private async void OnModifyItem(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var item = treeSources.SelectedNode?.Tag as OPCUAMember_item;
                if (item == null)
                    return;

                var grp = treeSources.SelectedNode.Parent.Tag as OPCUAMember_group;
                if (grp == null)
                    return;

                var srv = grp.Server;
                if (srv == null) return;

                //Modify 대상은 중복 검사에서 제외
                using (var dlg = new FormAddItem(
                       grp.arrItem.Where(i => i != item).Select(i => i.sNode),
                       grp.arrItem.Where(i => i != item).Select(i => i.sName)))
                {

                    if (srv.seServer == null || !srv.seServer.Connected)
                    {
                        MessageBox.Show("Server is not connected.");
                        return;
                    }

                    dlg.SetSession(grp.Server.seServer);
                    dlg.SetModify(item.sNode, item.sName);

                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;

                    var r = dlg.ResultItems.First();

                    try
                    {
                        await _host.ModifyItemAsync(
                            srv.accessName,
                            grp.sName,
                            item.sName, // 기존 이름으로 찾는다 (정책)
                            i =>
                            {
                                i.NodeId = r.NodeId;
                                i.Name = r.Alias;
                            });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Modify Item Failed");
                    }

                }
            }
            finally { EndAsyncOp(); }
        }

        private async void OnRemoveItem(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
                var item = treeSources.SelectedNode?.Tag as OPCUAMember_item;
                if (item == null) return;

                var grp = treeSources.SelectedNode?.Parent?.Tag as OPCUAMember_group;
                if (grp == null) return;

                var srv = grp.Server;
                if (srv == null) return;

                if (MessageBox.Show(
                    "Remove item '" + item.sName + "'?",
                    "Confirm",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                try
                {
                    await _host.RemoveItemAsync(
                        srv.accessName,
                        grp.sName,
                        item.sName);

                    //  Grid에서도 제거
                    RemoveItemFromGrid(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Remove Item Failed");
                }
            }
            finally { EndAsyncOp(); }
        }

        private void RemoveItemFromGrid(OPCUAMember_item item)
        {
            if (item == null)
                return;

            if (_rowMap.TryGetValue(item, out var row))
            {
                _rowMap.Remove(item);
            }

            item.Updated -= OnItemUpdated;

            if (_gridBinding.Contains(item))
                _gridBinding.Remove(item);
        }


        //OPC UA는 부분 Write 불가
        //Array / Matrix는 Read → Modify → Write
        private async void OnWriteItem(object sender, EventArgs e)
        {
            if (!BeginAsyncOp()) return;
            try
            {
            var item = treeSources.SelectedNode?.Tag as OPCUAMember_item;
            if (item == null)
                return;

            var grp = treeSources.SelectedNode.Parent?.Tag as OPCUAMember_group;
            var srv = grp?.Server;

            if (srv == null)
                return;

            var session = srv.seServer;
            if (session == null || !session.Connected)
            {
                MessageBox.Show("Server not connected.");
                return;
            }

            using (var dlg = new FormWriteItem())
            {
                dlg.SetItem(
                    srv.accessName,
                    grp.sName,
                    item.sName,
                    item.sNode,
                    item.DataType,
                    item.bIsArray || item.bIsMatrix
                );

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                WriteValue writeValue;

                // ===== Array / Matrix 처리 =====
                if (dlg.ArrayIndex >= 0)
                {
                    // 1️⃣ 기존 값 Read
                    DataValue dv;
                    try
                    {
                        dv = await srv.seServer.ReadValueAsync(
                            new NodeId(item.sNode));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Read failed:\n" + ex.Message);
                        return;
                    }

                    var arr = dv.Value as Array;
                    if (arr == null)
                    {
                        MessageBox.Show("Target node is not an array.");
                        return;
                    }

                    if (dlg.ArrayIndex < 0 || dlg.ArrayIndex >= arr.Length)
                    {
                        MessageBox.Show("Array index out of range.");
                        return;
                    }

                    try
                    {
                        var elementType = arr.GetType().GetElementType();
                        var converted =
                            Convert.ChangeType(dlg.WriteValue, elementType);

                        arr.SetValue(converted, dlg.ArrayIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Type conversion failed:\n" + ex.Message);
                        return;
                    }

                    writeValue = new WriteValue
                    {
                        NodeId = new NodeId(item.sNode),
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(arr))
                    };
                }
                else
                {
                    // ===== Scalar 처리 =====
                    object scalarValue = dlg.WriteValue;

                    // 가능하면 기존 타입 기준으로 변환 // 마지막 수신 값 기준으로 타입 추론
                    try
                    {
                        var dv = item.dvDataValue;
                        if (dv != null && dv.Value != null)
                        {
                            var targetType = dv.Value.GetType();
                            scalarValue = Convert.ChangeType(dlg.WriteValue, targetType);
                        }
                    }
                    catch
                    {
                        // 변환 실패 시 문자열 그대로 시도
                        scalarValue = dlg.WriteValue;
                    }

                    writeValue = new WriteValue
                    {
                        NodeId = new NodeId(item.sNode),
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(scalarValue))
                    };
                }

                var nodesToWrite = new WriteValueCollection { writeValue };

                // ===== WriteAsync (Task 기반) =====
                WriteResponse response;
                try
                {
                    response = await srv.seServer.WriteAsync(
                        null,
                        nodesToWrite,
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Write failed:\n" + ex.Message);
                    return;
                }

                // ===== 결과 확인 =====
                var status = response.Results[0];
                if (StatusCode.IsBad(status))
                {
                    MessageBox.Show("Write failed: " + status);
                }

            }
            }
            finally { EndAsyncOp(); }
        }

        #endregion menu action

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnConnectServer(sender, e);
        }


        public void AddLog(LogEntry entry)
        {
            if (listLog.InvokeRequired)
            {
                listLog.BeginInvoke(new Action<LogEntry>(AddLog), entry);
                return;
            }

            var item = new System.Windows.Forms.ListViewItem(entry.Time.ToString("HH:mm:ss.fff"));
            item.SubItems.Add(entry.Level);
            item.SubItems.Add(entry.Server);
            item.SubItems.Add(entry.Message);

            switch (entry.Level)
            {
                case "ERROR":
                    item.ForeColor = Color.Red;
                    break;
                case "WARN":
                    item.ForeColor = Color.DarkOrange;
                    break;
                case "INFO":
                    item.ForeColor = Color.Black;
                    break;
            }

            listLog.Items.Add(item);
            item.EnsureVisible();
        }

        private void FormOpcUaClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_forceExit && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                notifyIcon1.ShowBalloonTip(
                    1000,
                    "Autobase OPCUA Client",
                    "Application minimized to tray",
                    ToolTipIcon.Info);
            }
        }

        const int WM_APP_EXIT = 0x8001;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_APP_EXIT)
            {
                _forceExit = true;
                Close();
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var handle = this.Handle; // 핸들 강제 생성
        }

        private void FormOpcUaClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            WatchDogInfo.UnInit();

            if (_host == null) return;

            _host.HostStateChanged -= OnHostStateChanged;
            _host.ServersChanged -= OnServersChanged;
            _host.ShutdownRequested -= OnRemoteShutdown;
        }

        private void OnHostStateChanged(
            OpcUaHostState state,
            string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                    OnHostStateChanged(state, message)));
                return;
            }

            if (!string.IsNullOrEmpty(message))
            {
                string level;
                switch (state)
                {
                    case OpcUaHostState.Error:
                        level = "ERROR";
                        break;
                    case OpcUaHostState.Stopping:
                    case OpcUaHostState.Starting:
                        level = "WARN";
                        break;
                    default:
                        level = "INFO";
                        break;
                }

                AddLog(new LogEntry
                {
                    Level = level,
                    Server = "Host",
                    Message = message
                });
            }
        }

        private void OnServersChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(OnServersChanged));
                return;
            }

            //  새로 들어온 서버들에도 이벤트 연결
            foreach (var srv in _host.Servers)
                AttachServerEvents(srv);

            BuildTree();
            RefreshGridFromSelection();
        }

        #region Tray Icon Handlers
        private void OnTrayOpen(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void OnTrayDoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void OnTrayExit(object sender, EventArgs e)
        {
            _forceExit = true;
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        /// <summary>
        /// IPC Shutdown 명령 수신 시 호출.
        /// LocalMain 종료 시 트레이 상태에서도 정상 종료.
        /// </summary>
        private void OnRemoteShutdown()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(OnRemoteShutdown));
                return;
            }

            _forceExit = true;
            notifyIcon1.Visible = false;
            Application.Exit();
        }
        #endregion

        #region Grid and Tree Synchronization
        private void GridRealtime_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressTreeSelection || gridRealtime.SelectedRows.Count == 0)
                return;

            var item = gridRealtime.SelectedRows[0].DataBoundItem as OPCUAMember_item;
            if (item != null)
            {
                SelectTreeNodeByItem(item);
            }
        }

        private void SelectTreeNodeByItem(OPCUAMember_item item)
        {
            if (item == null || _suppressTreeSelection)
                return;

            _suppressTreeSelection = true;
            try
            {
                var node = FindTreeNodeByItem(treeSources.Nodes, item);
                if (node != null)
                {
                    treeSources.SelectedNode = node;
                    node.EnsureVisible();
                }
            }
            finally
            {
                _suppressTreeSelection = false;
            }
        }

        private TreeNode FindTreeNodeByItem(TreeNodeCollection nodes, OPCUAMember_item item)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == item)
                    return node;

                var found = FindTreeNodeByItem(node.Nodes, item);
                if (found != null)
                    return found;
            }
            return null;
        }
        #endregion

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BuildTree();
        }


        private void gridRealtime_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (e.RowIndex < 0)
                return;

            //  이 시점에는 RowIndex가 확정됨
            gridRealtime.ClearSelection();
            gridRealtime.Rows[e.RowIndex].Selected = true;
            gridRealtime.CurrentCell =
                gridRealtime.Rows[e.RowIndex].Cells[e.ColumnIndex];

            var item =
                gridRealtime.Rows[e.RowIndex].DataBoundItem as OPCUAMember_item;
            if (item == null)
                return;

            // Tree 동기화
            //SelectTreeNodeByItem(item);

            // Item 컨텍스트 메뉴
            ctxItem.Show(
                gridRealtime,
                gridRealtime.PointToClient(Cursor.Position));
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            HideToTray();
        }

        private void HideToTray()
        {
            Hide();
            notifyIcon1.ShowBalloonTip(
                1000,
                "Autobase OPCUA Client",
                "Application minimized to tray",
                ToolTipIcon.Info);
        }

        private void connectionOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormAutoSetup())
            {
                // 현재 Host 정책 반영 (선택)
                dlg.UseReconnect = _host.IsReconnectEnabled;
                dlg.ReconnectIntervalSec = (int)_host.ReconnectInterval.TotalSeconds;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _host.UpdateReconnectPolicy(
                        dlg.UseReconnect,
                        TimeSpan.FromSeconds(dlg.ReconnectIntervalSec)
                    );
                }
            }
        }

        private void menuCertificateSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new FormCertificateSettings(_host.UaConfig, _host))
                {
                    dlg.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Certificate Settings Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void menuCreateCert_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormCreateCert())
            {
                dlg.ShowDialog(this);
            }

        }

        private void startMinimizedToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.startMinimizedToTrayToolStripMenuItem.Checked)
            {
                this.startMinimizedToTrayToolStripMenuItem.Checked = false;
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcUaClientStartMinimizedToTray", 0);
                //flag = TotalConfig.LoadRegAutoBaseConfig("Config", "Start", "OpcUaClientStartMinimizedToTray", 1);
            }
            else
            {
                this.startMinimizedToTrayToolStripMenuItem.Checked = true;
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcUaClientStartMinimizedToTray", 1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            WatchDogInfo.SetTimer(EnumWatchDogInfo.WDI_OpcUAClient, 0);
        }
    }


    public class LogEntry
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public string Level { get; set; } = "INFO";
        public string Server { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public static class OpcUaUiLogBridge
    {
        public static Action<LogEntry> OnLog;

        public static void Write(LogEntry entry)
        {
            OnLog?.Invoke(entry);
        }

        public static void Write(string msg)
        {
            Write(new LogEntry { Message = msg });
        }

        public static void Attach(Action<LogEntry> handler)
        {
            OnLog = handler;
        }
    }

    /// <summary>
    /// ILogger 구현 — Host/Core 로그를 OpcUaUiLogBridge를 통해 UI 로그 리스트에 표시
    /// </summary>
    public sealed class OpcUaUiLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _category;

        public OpcUaUiLogger(string category)
        {
            _category = category ?? "";
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            => logLevel >= Microsoft.Extensions.Logging.LogLevel.Information;

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            string level;
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Error:
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    level = "ERROR";
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    level = "WARN";
                    break;
                default:
                    level = "INFO";
                    break;
            }

            string msg = formatter(state, exception);
            if (exception != null)
                msg += " | " + exception.Message;

            OpcUaUiLogBridge.Write(new LogEntry
            {
                Level = level,
                Server = _category,
                Message = msg
            });
        }
    }
}
