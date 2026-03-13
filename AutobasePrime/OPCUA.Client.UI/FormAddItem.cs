using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCUA.Client.UI
{

    public partial class FormAddItem : Form
    {
        public class AddItemEntry
        {
            public string NodeId { get; set; }
            public string Alias { get; set; }
        }
        public string ItemName { get; private set; }
        public string NodeId { get; private set; }
        public string DefaultValue { get; private set; }

        private bool _isModify = false;
        private string _originNodeId;

        private class OpcBrowseNode
        {
            public NodeId NodeId;
            public NodeClass NodeClass;
            public string DisplayName;
        }

        private ISession _session;

        public IReadOnlyList<AddItemEntry> ResultItems => _pendingItems;

        private readonly List<AddItemEntry> _pendingItems = new List<AddItemEntry>();
        private readonly HashSet<string> _existingNodeIds;
        private readonly HashSet<string> _existingAliases;

        public FormAddItem()
        {
            InitializeComponent();
        }

        public FormAddItem(
            IEnumerable<string> existNodeIds,
            IEnumerable<string> existAliases)
        {
            InitializeComponent();

            _existingNodeIds = new HashSet<string>(existNodeIds);
            _existingAliases = new HashSet<string>(existAliases);

            treeViewOpc.NodeMouseClick += TreeViewOpc_NodeMouseClick;

            if (this.checkBoxManual.Checked)
            {
                txtManualId.Enabled = true;
                txtNodeId.Enabled = false;
            }
            else
            {
                txtManualId.Enabled = false;
                txtNodeId.Enabled = true;
            }
        }

        private bool IsExistingNode(NodeId nodeId)
        {
            if (nodeId == null) return false;
            string s = nodeId.ToString();
            return _existingNodeIds.Contains(s);
        }

        public void SetSession(ISession session)
        {
            _session = session;
            InitBrowseTree();
        }

        public void SetModify(string nodeId, string alias)
        {
            _isModify = true;

            _originNodeId = nodeId;

            txtNodeId.Text = nodeId;
            txtAlias.Text = alias;

            this.Text = "Modify Item";
            buttonAdd.Enabled = false;
            buttonOK.Text = "Apply";
        }

        private void InitBrowseTree()
        {
            if (_session == null)
                return;

            treeViewOpc.BeginUpdate();
            treeViewOpc.Nodes.Clear();

            var root = new TreeNode("Objects")
            {
                Tag = new OpcBrowseNode
                {
                    NodeId = ObjectIds.ObjectsFolder,
                    NodeClass = NodeClass.Object
                }
            };

            // Lazy loading placeholder
            root.Nodes.Add(new TreeNode("Loading..."));

            treeViewOpc.Nodes.Add(root);

            treeViewOpc.BeforeExpand -= TreeViewOpc_BeforeExpand;
            treeViewOpc.BeforeExpand += TreeViewOpc_BeforeExpand;
            treeViewOpc.NodeMouseDoubleClick += treeViewOpc_NodeMouseDoubleClick;

            treeViewOpc.EndUpdate();
        }

        private void TreeViewOpc_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // 이미 로드된 경우 skip
            if (e.Node.Nodes.Count != 1 || e.Node.Nodes[0].Text != "Loading...")
                return;

            e.Node.Nodes.Clear();
            BrowseAndAddChildren(e.Node);
        }

        private void BrowseAndAddChildren(TreeNode parentNode)
        {
            if (_session == null)
                return;

            var tag = parentNode.Tag as OpcBrowseNode;
            if (tag == null)
                return;

            ReferenceDescriptionCollection refs;
            byte[] continuationPoint;

            _session.Browse(
                null,
                null,
                tag.NodeId,
                0,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)(NodeClass.Object | NodeClass.Variable),
                out continuationPoint,
                out refs
            );

            foreach (var rd in refs)
            {
                var childNodeId =
                    ExpandedNodeId.ToNodeId(rd.NodeId, _session.NamespaceUris);

                if (childNodeId == null)
                    continue;

                bool isDuplicated = IsExistingNode(childNodeId);

                var displayName = rd.DisplayName?.Text ?? childNodeId.ToString();

                var child = new TreeNode(rd.DisplayName.Text)
                {
                    Tag = new OpcBrowseNode
                    {
                        NodeId = childNodeId,
                        NodeClass = rd.NodeClass,
                        DisplayName = displayName
                    }
                };

                if (rd.NodeClass == NodeClass.Variable)
                {
                    if (isDuplicated)
                    {
                        child.ForeColor = Color.DarkGray;
                        child.NodeFont = new Font(treeViewOpc.Font, FontStyle.Strikeout);
                        child.ToolTipText = "Already added item";
                    }
                    else
                    {
                        child.ForeColor = Color.Black;
                    }
                }

                // Object / Folder 는 다시 Browse 가능
                if (rd.NodeClass == NodeClass.Object)
                {
                    child.Nodes.Add(new TreeNode("Loading..."));
                }

                parentNode.Nodes.Add(child);
            }
        }

        private void TreeViewOpc_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tag = e.Node?.Tag as OpcBrowseNode;
            if (tag == null || tag.NodeClass != NodeClass.Variable)
                return;

            string nodeId = tag.NodeId.ToString();

            //  NodeId 텍스트박스에 반드시 세팅
            txtNodeId.Text = nodeId;

            //string alias = nodeId.Replace("/", ".").Replace(" ", "_");
            //int lastDot = alias.LastIndexOf('.');
            //if (lastDot >= 0)
            //    alias = alias.Substring(lastDot + 1);

            //  DisplayName 기반 alias
            string alias = tag.DisplayName;
            alias = alias.Replace(" ", "_");   // 필요하면 정규화

            txtAlias.Text = alias;

            bool duplicated = _existingNodeIds.Contains(nodeId);

            buttonAdd.Enabled = !duplicated;

            if (duplicated)
            {
                statusLabel.Text = "Already registered item";
                statusLabel.ForeColor = Color.Red;
            }
            else
            {
                statusLabel.Text = "";
            }
        }

        private void treeViewOpc_NodeMouseDoubleClick(
    object sender,
    TreeNodeMouseClickEventArgs e)
        {
            var tag = e.Node?.Tag as OpcBrowseNode;
            if (tag == null || tag.NodeClass != NodeClass.Variable)
                return;

            string nodeId = tag.NodeId.ToString();

            //string alias = nodeId.Replace("/", ".").Replace(" ", "_");
            //int lastDot = alias.LastIndexOf('.');
            //if (lastDot >= 0)
            //    alias = alias.Substring(lastDot + 1);

            string alias = tag.DisplayName.Replace(" ", "_");

            AddItem(nodeId, alias);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_isModify)
            {
                string nodeId = txtNodeId.Text.Trim();
                string alias = txtAlias.Text.Trim();

                if (string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(alias))
                {
                    MessageBox.Show("NodeId and Alias are required.");
                    return;
                }

                if (alias.Contains("/"))
                {
                    MessageBox.Show("You can't use '/' for the access name.");
                    return;
                }

                _pendingItems.Clear();
                _pendingItems.Add(new AddItemEntry
                {
                    NodeId = nodeId,
                    Alias = alias
                });

                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            if (_pendingItems.Count == 0)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string nodeId = checkBoxManual.Checked
        ? txtManualId.Text.Trim()
        : txtNodeId.Text.Trim();

            string alias = txtAlias.Text.Trim();

            if (string.IsNullOrEmpty(nodeId))
            {
                MessageBox.Show(this, "Please choose or input NodeId");
                return;
            }

            if (string.IsNullOrEmpty(alias))
            {
                MessageBox.Show(this, "Please input Access name");
                return;
            }

            AddItem(nodeId, alias);

            txtNodeId.Clear();
            txtAlias.Clear();
        }

        private void checkBoxManual_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxManual.Checked)
            {
                txtManualId.Enabled = true;
                txtNodeId.Enabled = false;
            }
            else
            {
                txtManualId.Enabled = false;
                txtNodeId.Enabled = true;
            }
        }

        private void FormAddItem_Load(object sender, EventArgs e)
        {
            InitListView();
        }

        private void InitListView()
        {
            listViewItems.Clear();

            listViewItems.View = View.Details;
            listViewItems.FullRowSelect = true;
            listViewItems.GridLines = true;
            listViewItems.HideSelection = false;

            listViewItems.Columns.Add("NodeId", 350);
            listViewItems.Columns.Add("Alias", 150);
        }

        private void AddItem(string nodeId, string alias)
        {
            if (_existingNodeIds.Contains(nodeId) ||
                _pendingItems.Any(i => i.NodeId == nodeId))
            {
                MessageBox.Show(this, "You have already registered the same item");
                return;
            }

            if (_existingAliases.Contains(alias) ||
                _pendingItems.Any(i => i.Alias == alias))
            {
                MessageBox.Show(this, "You have already registered the same access name");
                return;
            }

            var entry = new AddItemEntry
            {
                NodeId = nodeId,
                Alias = alias
            };

            _pendingItems.Add(entry);

            var lvi = new ListViewItem(nodeId);
            lvi.SubItems.Add(alias);
            lvi.Tag = entry;

            listViewItems.Items.Add(lvi);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewItems.SelectedItems.Count == 0)
                return;

            foreach (ListViewItem item in listViewItems.SelectedItems)
            {
                var entry = item.Tag as AddItemEntry;
                if (entry != null)
                {
                    _pendingItems.Remove(entry);
                }

                listViewItems.Items.Remove(item);
            }
        }
    }
}
