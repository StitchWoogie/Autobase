using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewListAlarmDetailDlgPriority.
	/// </summary>
	public class ViewListAlarmDetailDlgPriority : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox listBox_AlarmPriority;
		private System.Windows.Forms.Button button_UnSelectAll;
		private System.Windows.Forms.Button button_SelectAll;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public bool[] bPriority = new bool[1000];

		public ViewListAlarmDetailDlgPriority(bool[] priority)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			if(priority != null) 
			{
				int		count = (priority.Length > 1000 ) ? 1000 : priority.Length;
				for(int i = 0; i < count; i++) bPriority[i] = priority[i];
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewListAlarmDetailDlgPriority));
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBox_AlarmPriority = new System.Windows.Forms.ListBox();
            this.button_UnSelectAll = new System.Windows.Forms.Button();
            this.button_SelectAll = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBox_AlarmPriority);
            this.groupBox3.Controls.Add(this.button_UnSelectAll);
            this.groupBox3.Controls.Add(this.button_SelectAll);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // listBox_AlarmPriority
            // 
            resources.ApplyResources(this.listBox_AlarmPriority, "listBox_AlarmPriority");
            this.listBox_AlarmPriority.Name = "listBox_AlarmPriority";
            this.listBox_AlarmPriority.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // button_UnSelectAll
            // 
            resources.ApplyResources(this.button_UnSelectAll, "button_UnSelectAll");
            this.button_UnSelectAll.Name = "button_UnSelectAll";
            this.button_UnSelectAll.Click += new System.EventHandler(this.button_UnSelectAll_Click);
            // 
            // button_SelectAll
            // 
            resources.ApplyResources(this.button_SelectAll, "button_SelectAll");
            this.button_SelectAll.Name = "button_SelectAll";
            this.button_SelectAll.Click += new System.EventHandler(this.button_SelectAll_Click);
            // 
            // ViewListAlarmDetailDlgPriority
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewListAlarmDetailDlgPriority";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewListAlarmDetailDlgPriority_Load);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void ViewListAlarmDetailDlgPriority_Load(object sender, System.EventArgs e)
		{
			int			i;
			for(i = 0; i < 1000; i++) 
			{
				this.listBox_AlarmPriority.Items.Add(String.Format("{0,3:d03}", i));
			}
			for(i = 0; i < 1000; i++)
			{
				this.listBox_AlarmPriority.SetSelected(i, bPriority[i]);
			}
		}
		

		void displayAllAlarmPriority(bool status)
		{
			for(int i = 0; i < 1000; i++) 
			{
				this.listBox_AlarmPriority.SetSelected(i, status);
			}
		}

		private void button_SelectAll_Click(object sender, System.EventArgs e)
		{
			displayAllAlarmPriority(true);
		}

		private void button_UnSelectAll_Click(object sender, System.EventArgs e)
		{
			displayAllAlarmPriority(false);
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			for(int i = 0; i < 1000; i++) 
			{
				bPriority[i] = this.listBox_AlarmPriority.GetSelected(i);
			}
		}


	}
}
