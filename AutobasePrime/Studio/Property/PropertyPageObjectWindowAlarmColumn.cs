using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;

namespace Studio.Property
{
    public partial class PropertyPageObjectWindowAlarmColumn : Form
    {
        public List<AlarmEventColumn> arrayColumns;

        public PropertyPageObjectWindowAlarmColumn()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogToStruct();

            DialogResult = DialogResult.OK;
        }

        void FillList()
        {
            this.listViewColumn.Items.Clear();

            for (int i = 0; i < arrayColumns.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(arrayColumns[i].name);
                lvi.SubItems.Add(arrayColumns[i].visible.ToString());
                lvi.SubItems.Add(arrayColumns[i].title);

                this.listViewColumn.Items.Add(lvi);
            }
        }

        private void PropertyPageObjectWindowAlarmColumn_Load(object sender, EventArgs e)
        {
            if (arrayColumns == null)
                arrayColumns = AlarmEventColumn.Init();
            else
                arrayColumns = (List<AlarmEventColumn>)Tools.CopyObject(arrayColumns);

            FillList();
        }

        int old_position = -1;

        void DialogToStruct()
        {
            if (old_position != -1)
            {
                AlarmEventColumn aec = arrayColumns[old_position];
                aec.visible = this.checkBoxVisible.Checked;
                aec.title = this.textBoxColumnTitle.Text;

                ListViewItem lvi = listViewColumn.Items[old_position];

                lvi.SubItems[1].Text = aec.visible.ToString();
                lvi.SubItems[2].Text = aec.title;
            }
        }

        private void listViewColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewColumn.SelectedIndices.Count == 0) return;

            DialogToStruct();
            
            old_position = listViewColumn.SelectedIndices[0];

            AlarmEventColumn aec = arrayColumns[old_position];
            //ListViewItem lvi = listViewColumn.Items[old_position];

            this.checkBoxVisible.Checked = aec.visible;
            this.textBoxColumnTitle.Text = aec.title;
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            arrayColumns = AlarmEventColumn.Init();

            FillList();
        }
    }
}
