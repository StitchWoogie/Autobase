using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using System.Collections;

namespace Studio.Script
{
    public partial class FormSelectMethod : Form
    {
        public FormSelectMethod()
        {
            InitializeComponent();
        }

        class PointsComparer : IComparer
        {
            private const int pointsColumnIndex = 1;

            public int Compare(object x, object y)
            {
                ListViewItem lvi1 = (ListViewItem)x;
                ListViewItem lvi2 = (ListViewItem)y;

                return lvi1.SubItems[1].Text.CompareTo(lvi2.SubItems[1].Text);
            }
        }

        private void FormSelectMethod_Load(object sender, EventArgs e)
        {
            listView1.ListViewItemSorter = new PointsComparer();
            //listView1.Sort();

            ScriptExternalRun.scriptExternal.FillMethodsToListView(this.listView1);
        }

        public string sSelectedMethod;

        void OK()
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item", "Error");
                return;
            }

            ListViewItem lvi = listView1.SelectedItems[0];

            sSelectedMethod = lvi.SubItems[1].Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            OK();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    
}
