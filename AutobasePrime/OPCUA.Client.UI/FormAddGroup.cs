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
    public partial class FormAddGroup : Form
    {
        public string GroupName { get; private set; }
        public int PublishingInterval { get; private set; }

        private bool _isModify = false;

        public FormAddGroup()
        {
            InitializeComponent();

           // textBoxGroupName.Text = "gr" + OPCUAClientMain.Servers.arrGroup.Count.ToString("000");
        }
        public void SetModify(string groupName, int publishingInterval)
        {
            _isModify = true;

            txtGroupName.Text = groupName;
            numInterval.Value = publishingInterval;

            this.Text = "Modify Group";
            btnOk.Text = "Apply";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("Group name is required.");
                return;
            }

            GroupName = txtGroupName.Text.Trim();
            PublishingInterval = (int)numInterval.Value;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numInterval_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtGroupName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
