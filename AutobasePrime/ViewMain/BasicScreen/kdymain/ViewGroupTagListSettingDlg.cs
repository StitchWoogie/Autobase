using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using System.Data;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewGroupTagListSettingDlg.
	/// </summary>
	public class ViewGroupTagListSettingDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		public  System.Windows.Forms.CheckBox checkBox_AllTagDisplay;
		private System.Windows.Forms.Label label_TagCount;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewGroupTagListSettingDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewGroupTagListSettingDlg));
            this.checkBox_AllTagDisplay = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_TagCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox_AllTagDisplay
            // 
            resources.ApplyResources(this.checkBox_AllTagDisplay, "checkBox_AllTagDisplay");
            this.checkBox_AllTagDisplay.Name = "checkBox_AllTagDisplay";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_TagCount);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label_TagCount
            // 
            resources.ApplyResources(this.label_TagCount, "label_TagCount");
            this.label_TagCount.Name = "label_TagCount";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // 
            // ViewGroupTagListSettingDlg
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox_AllTagDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewGroupTagListSettingDlg";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewGroupTagListSettingDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void ViewGroupTagListSettingDlg_Load(object sender, System.EventArgs e)
		{
			this.label_TagCount.Text = TagLib.tagListAll.Length.ToString();
		}
	}
}
