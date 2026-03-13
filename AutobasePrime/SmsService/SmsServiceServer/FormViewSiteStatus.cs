using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SmsServiceServer
{
	/// <summary>
	/// Summary description for FormViewSiteStatus.
	/// </summary>
	public class FormViewSiteStatus : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		public FormViewSiteStatus()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormViewSiteStatus));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
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
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.Font = null;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
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
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormViewSiteStatus
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.listView1);
            this.Icon = null;
            this.Name = "FormViewSiteStatus";
            this.Load += new System.EventHandler(this.FormViewSiteStatus_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			ListViewItem item;
			for(int i = 0; i < 2; i++) 
			{
				item = this.listView1.Items[i];

				item.SubItems[1].Text = ShareServerMain.commStatus[i].nCountTotal.ToString();
				item.SubItems[2].Text = ShareServerMain.commStatus[i].nCountFail.ToString();
				item.SubItems[3].Text = ShareServerMain.commStatus[i].nContinueFail.ToString();
			}
		}

		private void FormViewSiteStatus_Load(object sender, System.EventArgs e)
		{
			ListViewItem item;
			for(int i = 0; i < 2; i++) 
			{
				item = new ListViewItem();
				if(i == 0)	item = new ListViewItem(ShareServerMain.sSiteNamePrimary);
				else		item = new ListViewItem(ShareServerMain.sSiteNameSecondary);

				item.SubItems.Add(ShareServerMain.commStatus[i].nCountTotal.ToString());
				item.SubItems.Add(ShareServerMain.commStatus[i].nCountFail.ToString());
				item.SubItems.Add(ShareServerMain.commStatus[i].nContinueFail.ToString());

				this.listView1.Items.Add(item);
			}
		}
	}
}
