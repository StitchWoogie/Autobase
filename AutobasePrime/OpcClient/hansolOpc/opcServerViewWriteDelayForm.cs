using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace OpcClient
{
    public partial class opcServerViewWriteDelayForm : Form
    {
        public int nServerPos = 0;
        public int nCurrentWriteSaved = -1;
        opcServerReadWriteClass currentOpcServer = null;
        opcClientWriteDelayWhenAsyncEvnetClass writeDelayStart = null;

        public opcServerViewWriteDelayForm()
        {
            InitializeComponent();
        }

        private void getCurrentOpcServer()
        {
            if (nServerPos < 0 || nServerPos >= opcBasic.arrOpcServer.Count)
            {
                currentOpcServer = null;
                if (NetTools.Tools.IsLangKorean())
                {
                    this.label_WriteCount.Text = "메모리에 남아있는 출력된 갯수 : 0";
                    this.label_IsTimeDelay.Text = "현재 출력지연 여부 : No";
                }
                else
                {
                    this.label_WriteCount.Text = "Waitting Write Count : 0";
                    this.label_IsTimeDelay.Text = "Time Delay : No";
                }
                return;
            }
            currentOpcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServerPos];
            if (currentOpcServer.m_server == null)
            {
                currentOpcServer = null;
                if (NetTools.Tools.IsLangKorean())
                {
                    this.label_WriteCount.Text = "메모리에 남아있는 출력된 갯수 : 0";
                    this.label_IsTimeDelay.Text = "현재 출력지연 여부 : No";
                }
                else
                {
                    this.label_WriteCount.Text = "Waitting Write Count : 0";
                    this.label_IsTimeDelay.Text = "Time Delay : No";
                }
                return;
            }
        }

        private void listAddWriteWaitting()
        {
            try
            {
                if (currentOpcServer == null || currentOpcServer.arrWriteDelay == null) return;

                opcClientWriteDelayWhenAsyncEvnetClass writeDelay;
                if (currentOpcServer.arrWriteDelay.Count == this.nCurrentWriteSaved)
                {
                    if (currentOpcServer.arrWriteDelay.Count <= 0) return;
                    writeDelay = (opcClientWriteDelayWhenAsyncEvnetClass)currentOpcServer.arrWriteDelay[0];
                    if (this.writeDelayStart == writeDelay) return;
                }
                this.nCurrentWriteSaved = currentOpcServer.arrWriteDelay.Count;

                if (NetTools.Tools.IsLangKorean())
                {
                    this.label_WriteCount.Text = string.Format("메모리에 남아있는 출력된 갯수 : {0}", currentOpcServer.arrWriteDelay.Count);
                    if (currentOpcServer.bWriteDelayWait) this.label_IsTimeDelay.Text = "현재 출력지연 여부 : Yes";
                    else this.label_IsTimeDelay.Text = "현재 출력지연 여부 : No";
                }
                else
                {
                    this.label_WriteCount.Text = string.Format("Waitting Write Count : {0}", currentOpcServer.arrWriteDelay.Count);
                    if (currentOpcServer.bWriteDelayWait) this.label_IsTimeDelay.Text = "Time Delay : Yes";
                    else this.label_IsTimeDelay.Text = "Time Delay : No";
                }
                listView_WriteWaitting.Items.Clear();

                ListViewItem item;
                for (int i = 0; i < currentOpcServer.arrWriteDelay.Count; i++)
                {
                    item = new ListViewItem();
                    writeDelay = (opcClientWriteDelayWhenAsyncEvnetClass)currentOpcServer.arrWriteDelay[i];
                    item.Text = string.Format("{0}", i + 1);
                    item.SubItems.Add(writeDelay.groupName.ToString());
                    item.SubItems.Add(writeDelay.itemName.ToString());
                    item.SubItems.Add(string.Format("{0:d02}:{1:d02}", writeDelay.writeSec / 60, writeDelay.writeSec % 60));
                    listView_WriteWaitting.Items.Add(item);
                }
                if (currentOpcServer.arrWriteDelay.Count <= 0)
                {
                    writeDelayStart = null;
                }
                else
                {
                    writeDelayStart = (opcClientWriteDelayWhenAsyncEvnetClass)currentOpcServer.arrWriteDelay[0];
                }
            }
            catch { }
        }

        private void opcServerViewWriteDelayForm_Load(object sender, EventArgs e)
        {
            getCurrentOpcServer();
            listAddWriteWaitting();
        }

        private void timerViewWrite_Tick(object sender, EventArgs e)
        {
            listAddWriteWaitting();
        }
    }
}