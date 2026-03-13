using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using NetTools;

namespace AutoLibLocal
{
    public partial class FormConfigWebTagList : Form
    {
        public FormConfigWebTagList()
        {
            InitializeComponent();
        }

        List<WebTagList> tempList = new List<WebTagList>();

        private void FormConfigWebTagList_Load(object sender, EventArgs e)
        {
            tempList = WebGroupList.Load();

            WebTagList list;

            for (int i = 0; i < tempList.Count; i++)
            {
                list = (WebTagList)tempList[i];

                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("");

                ChangeItem(lvi, list);

                this.listView1.Items.Add(lvi);
            }            
        }

        void ChangeItem(ListViewItem lvi, WebTagList list)
        {
            lvi.SubItems[0].Text = list.name;
            lvi.SubItems[1].Text = list.description;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormConfigWebTagListAdd dialog = new FormConfigWebTagListAdd();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                WebTagList list = dialog.Get();

                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("");

                ChangeItem(lvi, list);
                
                this.listView1.Items.Add(lvi);

                lvi.Selected = true;
                lvi.EnsureVisible();

                tempList.Add(list);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item to delete.", "Delete error");
                return;
            }

            int index = listView1.SelectedItems[0].Index;
            listView1.Items.RemoveAt(index);

            this.tempList.RemoveAt(index);
        }

        void Modify()
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item to modify.", "Modify error");
                return;
            }

            ListViewItem lvi = listView1.SelectedItems[0];

            FormConfigWebTagListAdd dialog = new FormConfigWebTagListAdd();


            dialog.Set((WebTagList)tempList[lvi.Index]);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                WebTagList list = dialog.Get();

                ChangeItem(lvi, list);
                tempList[lvi.Index] = list;
            }
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Modify();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            WebGroupList.Save(tempList);

            DialogResult = DialogResult.OK;
            Close();
        }
    }

    
}