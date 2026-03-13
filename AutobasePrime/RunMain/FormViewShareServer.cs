using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace RunMain
{
	/// <summary>
	/// Summary description for FormViewShareServer.
	/// </summary>
	public class FormViewShareServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Panel panel2;
        private Label label1;
        private Button buttonResetShareServer;
        private IContainer components;

		public FormViewShareServer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormViewShareServer));
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonResetShareServer = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_list);
            this.panel1.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.FullRowSelect = true;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Elapsed);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonResetShareServer);
            this.panel2.Controls.Add(this.label1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonResetShareServer
            // 
            resources.ApplyResources(this.buttonResetShareServer, "buttonResetShareServer");
            this.buttonResetShareServer.Name = "buttonResetShareServer";
            this.buttonResetShareServer.UseVisualStyleBackColor = true;
            this.buttonResetShareServer.Click += new System.EventHandler(this.buttonResetShareServer_Click);
            // 
            // FormViewShareServer
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Name = "FormViewShareServer";
            this.Load += new System.EventHandler(this.FormViewShareServer_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion


        void ReLoadList()
        {
            m_list.Items.Clear();

            CommunicationStatus st;
            ListViewItem item;
            for (int i = 0; i < ShareServerMain.arrayCommStatus.Count; i++)
            {
                st = (CommunicationStatus)ShareServerMain.arrayCommStatus[i];

                item = new ListViewItem(st.ip);
                item.SubItems.Add(st.nFrameRecv.ToString());
                item.SubItems.Add(st.nFrameSend.ToString());
                //item.SubItems.Add(st.bUdp ? "UDP" : "TCP");

                m_list.Items.Add(item);
            }
        }

        void UpdateTotalConnection()
        {
            string buf = String.Format("Threads:{0}", ShareServerMain.nThreadCount);

            if (buf != this.label1.Text)
                this.label1.Text = buf;
        }

        private void timer1_Elapsed(object sender, System.EventArgs e)
        {
            timer1.Enabled = false;
            //ShareServerMain.SetWait();
            lock (ShareServerMain.objLockArrayCommStatus)
            {
                if (m_list.Items.Count != ShareServerMain.arrayCommStatus.Count)
                    ReLoadList();
            }
            //ShareServerMain.ResetWait();

            CommunicationStatus st;
            ListViewItem item;

            //ShareServerMain.SetWait();
            lock (ShareServerMain.objLockArrayCommStatus)
            {
                for (int i = 0; i < m_list.Items.Count; i++)
                {
                    st = (CommunicationStatus)ShareServerMain.arrayCommStatus[i];
                    item = m_list.Items[i];

                    //if (item.SubItems[0].Text != st.socket.RemoteEndPoint.ToString())
                    //    item.SubItems[0].Text = st.socket.RemoteEndPoint.ToString();

                    if (item.SubItems[1].Text != st.nFrameRecv.ToString())
                        item.SubItems[1].Text = st.nFrameRecv.ToString();
                    if (item.SubItems[2].Text != st.nFrameSend.ToString())
                        item.SubItems[2].Text = st.nFrameSend.ToString();
                }
            }
            //ShareServerMain.ResetWait();

            UpdateTotalConnection();

            timer1.Enabled = true;
        }

        private void FormViewShareServer_Load(object sender, EventArgs e)
        {

        }

        private void buttonResetShareServer_Click(object sender, EventArgs e)
        {
            ShareServerMain.UnInit();
            ShareServerMain.Init();
        }

        /*

		void ReLoadList()
		{
			m_list.Items.Clear();

			SocketThreadClass st;
			ListViewItem item;
			for(int i = 0; i < ShareServerMain.arraySocketThread.Count; i++) 
			{
				st = (SocketThreadClass)ShareServerMain.arraySocketThread[i];

                item = new ListViewItem(st.socket.RemoteEndPoint == null ? "" : st.socket.RemoteEndPoint.ToString());
				item.SubItems.Add(st.nFrameRecv.ToString());
				item.SubItems.Add(st.nFrameSend.ToString());
                item.SubItems.Add(st.bUdp ? "UDP" : "TCP");

				m_list.Items.Add(item);
			}
		}

		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer1.Enabled = false;
			ShareServerMain.SetWait();
			if(m_list.Items.Count != ShareServerMain.arraySocketThread.Count) 
				ReLoadList();
			ShareServerMain.ResetWait();

			SocketThreadClass st;
			ListViewItem item;

			ShareServerMain.SetWait();
			for(int i = 0; i < m_list.Items.Count; i++) 
			{
				st = (SocketThreadClass)ShareServerMain.arraySocketThread[i];
				item = m_list.Items[i];

                if (st.socket.RemoteEndPoint == null)
                {
                    item.SubItems[0].Text = "";
                }
                else {
                    if (item.SubItems[0].Text != st.socket.RemoteEndPoint.ToString())
                        item.SubItems[0].Text = st.socket.RemoteEndPoint.ToString();
                }

				if(item.SubItems[1].Text != st.nFrameRecv.ToString())
					item.SubItems[1].Text = st.nFrameRecv.ToString();
				if(item.SubItems[2].Text != st.nFrameSend.ToString())
					item.SubItems[2].Text = st.nFrameSend.ToString();
			}
			ShareServerMain.ResetWait();

			timer1.Enabled = true;
		}*/
	}
}
