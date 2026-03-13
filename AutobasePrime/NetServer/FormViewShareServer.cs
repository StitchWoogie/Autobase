using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NetServer
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
		private System.Windows.Forms.ColumnHeader columnHeader4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
            this.panel1.SuspendLayout();
            
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_list);
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
            // FormViewShareServer
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Name = "FormViewShareServer";
            this.panel1.ResumeLayout(false);
            
            this.ResumeLayout(false);

		}
		#endregion

		void ReLoadList()
		{
			m_list.Items.Clear();

			SocketThreadClass st;
			ListViewItem item;
			for(int i = 0; i < ShareServerMain.arraySocketThread.Count; i++) 
			{
				st = (SocketThreadClass)ShareServerMain.arraySocketThread[i];
				item = new ListViewItem(st.socket.RemoteEndPoint.ToString());
				item.SubItems.Add(st.tConnect.ToString());
				item.SubItems.Add(st.nRecvCount.ToString());
				item.SubItems.Add(st.nSendCount.ToString());
				m_list.Items.Add(item);
			}
		}

		private void timer1_Elapsed(object sender, System.EventArgs e)
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
				//if(item.SubItems[0].Text != st.socket.RemoteEndPoint.ToString())
				//	item.SubItems[0].Text = st.socket.RemoteEndPoint.ToString();
				//if(item.SubItems[1].Text != st.tConnect.ToString())
				//	item.SubItems[1].Text = st.tConnect.ToString();
				if(item.SubItems[2].Text != st.nRecvCount.ToString())
					item.SubItems[2].Text = st.nRecvCount.ToString();
				if(item.SubItems[3].Text != st.nSendCount.ToString())
					item.SubItems[3].Text = st.nRecvCount.ToString();
			}
			ShareServerMain.ResetWait();

			timer1.Enabled = true;
		}
	}
}
