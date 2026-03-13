using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ats.Comm;

namespace ModbusServer
{
    public partial class FormViewList : Form
    {
        DeviceTcpListener myTcpListner;

        public FormViewList(DeviceTcpListener listner)
        {
            InitializeComponent();

            myTcpListner = listner;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DeviceCommon device;
            ListViewItem lvi;
            string buf;

            lock (myTcpListner.arrayDevice)
            {
                for (int i = 0; i < myTcpListner.arrayDevice.Count; i++)
                {
                    device = myTcpListner.arrayDevice[i];

                    if (i >= listView1.Items.Count)
                    {
                        lvi = new ListViewItem("");
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("");
                        listView1.Items.Add(lvi);
                    }
                    else
                    {
                        lvi = listView1.Items[i];
                    }

                    buf = device.GetInfoString();
                    if (lvi.SubItems[0].Text != buf)
                        lvi.SubItems[0].Text = buf;
                    buf = device.nRecvBytes.ToString();
                    if (lvi.SubItems[1].Text != buf)
                        lvi.SubItems[1].Text = buf;
                    buf = device.nSendBytes.ToString();
                    if (lvi.SubItems[2].Text != buf)
                        lvi.SubItems[2].Text = buf;

                    if (device.Connected)
                        buf = "Connected";
                    else
                        buf = "Disconnected";
                    if (lvi.SubItems[3].Text != buf)
                        lvi.SubItems[3].Text = buf;
                }

                if (myTcpListner.arrayDevice.Count < listView1.Items.Count)
                {
                    listView1.Items.RemoveAt(myTcpListner.arrayDevice.Count);
                }
            }
        }

        private void FormViewList_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
        }

        private void FormViewList_Closed(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
        }
    }
}