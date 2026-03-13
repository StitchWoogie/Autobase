using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpcClient.plcScanComm
{
	/// <summary>
	/// Summary description for plcScanCommSettingForm.
	/// </summary>
	public class plcScanCommSettingForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox_sharedMemoryName;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public plcScanCommSettingForm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(plcScanCommSettingForm));
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_sharedMemoryName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_sharedMemoryName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBox_sharedMemoryName
            // 
            resources.ApplyResources(this.textBox_sharedMemoryName, "textBox_sharedMemoryName");
            this.textBox_sharedMemoryName.Name = "textBox_sharedMemoryName";
            // 
            // plcScanCommSettingForm
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_Cancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "plcScanCommSettingForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.plcScanCommSettingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(this.textBox_sharedMemoryName.Text.Length <= 0) 
			{
				MessageBox.Show("PlcScan Shared Name Input Error.");
				return;
			}
			opcBasic.opcClientConfig.shareName = this.textBox_sharedMemoryName.Text;
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void plcScanCommSettingForm_Load(object sender, System.EventArgs e)
		{
			this.textBox_sharedMemoryName.Text = opcBasic.opcClientConfig.shareName;
		}
	}
}
