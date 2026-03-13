using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PortalServerWeb.AutoWeb.Service;

namespace RunMain.DataGateServer
{
    public partial class FormDataGateServer : Form
    {
        public FormDataGateServer()
        {
            InitializeComponent();
        }

        void ReLoadList()
        {
            m_list.Items.Clear();

            DataGateClient st;
            ListViewItem item;
            for (int i = 0; i < DataGateClient.arrayClient.Count; i++)
            {
                st = DataGateClient.arrayClient[i];

                item = new ListViewItem(st.id.ToString());
                item.SubItems.Add(st.ip);//st.nFrameRecv.ToString());
                item.SubItems.Add("");//st.nFrameSend.ToString());
                item.SubItems.Add("");//st.nFrameSend.ToString());

                m_list.Items.Add(item);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //ShareServerMain.SetWait();
            if (m_list.Items.Count != DataGateClient.arrayClient.Count)
                ReLoadList();
            //ShareServerMain.ResetWait();

            DataGateClient st;
            ListViewItem item;

            //ShareServerMain.SetWait();
            for (int i = 0; i < m_list.Items.Count; i++)
            {
                st = DataGateClient.arrayClient[i];
                item = m_list.Items[i];

                if (item.SubItems[0].Text != st.id.ToString())
                    item.SubItems[0].Text = st.id.ToString();

                //if (item.SubItems[1].Text != st.ip)
                //    item.SubItems[1].Text = st.ip;
                if (item.SubItems[2].Text != st.nFrameRecv.ToString())
                    item.SubItems[2].Text = st.nFrameRecv.ToString();
                if (item.SubItems[3].Text != st.nFrameSend.ToString())
                    item.SubItems[3].Text = st.nFrameSend.ToString();
            }
            //ShareServerMain.ResetWait();

            timer1.Enabled = true;
        }

        private void FormDataGateServer_Load(object sender, EventArgs e)
        {

        }
    }
}
