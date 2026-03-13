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
    public partial class FormWriteItem : Form
    {
        public string ServerName { get; private set; }
        public string GroupName { get; private set; }
        public string ItemName { get; private set; }
        public string NodeId { get; private set; }
        public string Type { get; private set; }

        public string WriteValue { get; private set; }
        public int ArrayIndex { get; private set; } = -1;

        private bool _isArray;

        public FormWriteItem()
        {
            InitializeComponent();           
        }

        public void SetItem(
            string serverName,
            string groupName,
            string itemName,
            string nodeId,
            string type,
            bool isArray)
        {
            ServerName = serverName;
            GroupName = groupName;
            ItemName = itemName;
            NodeId = nodeId;
            Type = type;
            _isArray = isArray;

            lbServer.Text = serverName;
            lbGroup.Text = groupName;
            lbItem.Text = itemName;

            // 배열 여부에 따라 UI 제어
            numArray.Enabled = isArray;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWrite.Text))
            {
                MessageBox.Show("Write value is required.");
                return;
            }

            WriteValue = txtWrite.Text.Trim();

            if (_isArray)
            {
                ArrayIndex = (int)numArray.Value;
            }
            else
            {
                ArrayIndex = -1;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

    }
}
