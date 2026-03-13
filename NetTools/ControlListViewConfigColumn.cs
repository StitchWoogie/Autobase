using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace NetTools
{
    public partial class ControlListViewConfigColumn : Form
    {
        public ControlListViewConfigColumn()
        {
            InitializeComponent();
        }

        public void Set(ArrayList array)
        {
            ControlListViewHeader clvh;

            for (int i = 0; i < array.Count; i++)
            {
                clvh = (ControlListViewHeader)array[i];
                ListViewItem lvi = new ListViewItem(clvh.text);
                lvi.SubItems.Add(clvh.bVisible.ToString());

                this.listView1.Items.Add(lvi);
            }
        }

        public void Get(ArrayList array)
        {
            ControlListViewHeader clvh;

            for (int i = 0; i < array.Count; i++)
            {
                clvh = (ControlListViewHeader)array[i];
                ListViewItem lvi = listView1.Items[i];

                clvh.bVisible = ConvertTool.ToBoolean(lvi.SubItems[1].Text);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        bool bLoading = false;

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bLoading = true;
            if (this.listView1.SelectedItems.Count > 0)
            {
                this.checkBoxVisible.Checked = ConvertTool.ToBoolean(this.listView1.SelectedItems[0].SubItems[1].Text);
            }
            bLoading = false;
        }

        private void checkBoxVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (bLoading) return;

            if (this.listView1.SelectedItems.Count > 0)
            {
                this.listView1.SelectedItems[0].SubItems[1].Text = this.checkBoxVisible.Checked.ToString();
            }
        }
    }
}
