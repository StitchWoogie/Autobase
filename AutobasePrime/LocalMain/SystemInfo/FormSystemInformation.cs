using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;

namespace LocalMain.SystemInfo
{
    public partial class FormSystemInformation : Form
    {
        public FormSystemInformation()
        {
            InitializeComponent();
        }

        void EnableDisable()
        {
            bool flag_alarm = false;
            bool flag_status = false;
            bool flag_item = false;

            if (SystemInfoStatus.IsAble)
            {
                flag_status = true;
                flag_alarm = true;
                flag_item = true;
            }

            if (this.checkBoxUseBoardStatus.Checked)
            {
                flag_alarm = true;
                flag_item = true;
            }
            else
            {
                flag_alarm = false;
                flag_item = false;
            }

            this.checkBoxUseBoardStatus.Enabled = flag_status;
            this.checkBoxUseStatusAlarm.Enabled = flag_alarm;
            this.listView1.Enabled = flag_item;
        }

        private void FormSystemInformation_Load(object sender, EventArgs e)
        {
            this.checkBoxUseBoardStatus.Checked = SystemInfoStatus.bUseBoardInfo;
            this.checkBoxUseStatusAlarm.Checked = SystemInfoStatus.bUseBoardAlarm;

            FillListBox();

            EnableDisable();

            TotalConfig.AutoBaseListCtrlConfigLoad(this.listView1, "LocalMain", "FormSystemInformation");
        }

        void FillListBox()
        {
            for (int i = 0; i < SystemInfoStatus.MAX_ITEM; i++)
            {
                ListViewItem lvi = new ListViewItem(SystemInfoStatus.itemName[i]);
                lvi.Checked = SystemInfoStatus.itemAlarm[i];
                lvi.SubItems.Add("?");
                lvi.SubItems.Add(SystemInfoStatus.itemMin[i].ToString());
                lvi.SubItems.Add(SystemInfoStatus.itemMax[i].ToString());

                listView1.Items.Add(lvi);
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ListViewItem lvi;
            string buf;

            for (int i = 0; i < SystemInfoStatus.MAX_ITEM; i++)
            {
                lvi = this.listView1.Items[i];

                buf = SystemInfoStatus.itemCurr[i].ToString("F2");

                if (buf != lvi.SubItems[1].Text)
                {
                    lvi.SubItems[1].Text = buf;
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            TotalConfig.AutoBaseListCtrlConfigSave(this.listView1, "LocalMain", "FormSystemInformation");
            
            SystemInfoStatus.bUseBoardInfo = this.checkBoxUseBoardStatus.Checked;
            SystemInfoStatus.bUseBoardAlarm = this.checkBoxUseStatusAlarm.Checked;

            TotalConfig.SaveRegAutoBaseConfig("SysInfo", "Board", "bUseBoardInfo", SystemInfoStatus.bUseBoardInfo);
            TotalConfig.SaveRegAutoBaseConfig("SysInfo", "Board", "bUseBoardAlarm", SystemInfoStatus.bUseBoardAlarm);

            ListViewItem lvi;

            for (int i = 0; i < SystemInfoStatus.MAX_ITEM; i++)
            {
                lvi = listView1.Items[i];
                SystemInfoStatus.itemAlarm[i] = lvi.Checked;
                SystemInfoStatus.itemMin[i] = ConvertTool.ToSingle(lvi.SubItems[2].Text);
                SystemInfoStatus.itemMax[i] = ConvertTool.ToSingle(lvi.SubItems[3].Text);

                TotalConfig.SaveRegAutoBaseConfig("SysInfo", "Board\\AlarmItem", SystemInfoStatus.itemName[i], SystemInfoStatus.itemAlarm[i]);
                TotalConfig.SaveRegAutoBaseConfig("SysInfo", "Board\\AlarmLow", SystemInfoStatus.itemName[i], SystemInfoStatus.itemMin[i].ToString());
                TotalConfig.SaveRegAutoBaseConfig("SysInfo", "Board\\AlarmHigh", SystemInfoStatus.itemName[i], SystemInfoStatus.itemMax[i].ToString());
            }

            SystemInfoStatus.SystemInfoUninit();
            SystemInfoStatus.SystemInfoInit();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void checkBoxUseBoardStatus_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select the item to Modify.");
                return;
            }

            ListViewItem lvi = this.listView1.SelectedItems[0];

            FormModify dialog = new FormModify();

            string min = lvi.SubItems[2].Text;
            string max = lvi.SubItems[3].Text;

            dialog.Set(min, max);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(out min, out max);
                lvi.SubItems[2].Text = min;
                lvi.SubItems[3].Text = max;
            }
        }

        private void buttonSetToDefault_Click(object sender, EventArgs e)
        {
            ListViewItem lvi;

            for (int i = 0; i < SystemInfoStatus.MAX_ITEM; i++)
            {
                lvi = listView1.Items[i];

                lvi.Checked = SystemInfoStatus.itemAlarm[i];
                lvi.SubItems[2].Text = SystemInfoStatus.itemMinDefault[i].ToString();
                lvi.SubItems[3].Text = SystemInfoStatus.itemMaxDefault[i].ToString();
            }
        }
    }
}
