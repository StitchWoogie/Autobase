using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opc;


namespace OpcClient
{
	/// <summary>
	/// Summary description for FormSelectServer.
	/// </summary>
	public class FormSelectServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        public string hostName = "";

		public FormSelectServer()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectServer));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AccessibleDescription = null;
            this.listView1.AccessibleName = null;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.BackgroundImage = null;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.Font = null;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // FormSelectServer
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectServer";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormSelectServer_Load);
            this.ResumeLayout(false);

		}
		#endregion


		bool checkEqualServerExist(string serverName)
		{
			if(listView1.Items.Count <= 0) return false;

			ListViewItem	item;
			for(int i = 0; i < listView1.Items.Count; i++) 
			{
				item = listView1.Items[i];
				if(item.Text == serverName) return true;
			}
			return false;
		}

		void FillServer(IDiscovery discovery, Opc.Specification spec)
		{
			// get default login information.
			// ConnectData connectData = FindConnectData(node);

			// find the servers.
			Opc.Server[] servers = discovery.GetAvailableServers(spec, (hostName == null || hostName.Length <= 0) ? null : hostName, null);

			// add children.
			if (servers != null)
			{
				foreach (Opc.Server server in servers)
				{
					//TreeNode child   = new TreeNode(server.Name);
					//child.ImageIndex = child.SelectedImageIndex = Opc.Resources.IMAGE_LOCAL_SERVER;	
					//child.Tag        = server;

					//node.Nodes.Add(child);
					if(checkEqualServerExist(server.Name)) continue;
					ListViewItem item = new ListViewItem(server.Name);
					item.SubItems.Add(spec.Description);			// version 정보표시
					
					this.listView1.Items.Add(item);
				}
			}
		}

		//private IDiscovery discovery = null;

		private void FormSelectServer_Load(object sender, System.EventArgs e)
		{
            try // 2009-02-27 add, 에러가 발생하는 경우가 있음, Windows Vista 이면서, 리모트 연결일 때 다운되는 현상이 있기 때문에
            {
                OpcCom.ServerEnumerator discovery = new OpcCom.ServerEnumerator();

                FillServer(discovery, Opc.Specification.COM_DA_30);
                FillServer(discovery, Opc.Specification.COM_DA_20);
                FillServer(discovery, Opc.Specification.COM_DA_10);
            }
            catch { }
		}

		public string m_sServer;

		void OK()
		{
			if(this.listView1.SelectedItems.Count == 0) 
			{
				MessageBox.Show("Select Server");
				return;
			}
			m_sServer = this.listView1.SelectedItems[0].SubItems[0].Text;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			OK();
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			OK();
		}


	}
}
