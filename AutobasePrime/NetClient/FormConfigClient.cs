using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetCommon;
using NetTools;

namespace NetClient
{
	/// <summary>
	/// Summary description for FormConfigClient.
	/// </summary>
	public class FormConfigClient : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownUdpPort;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownTcpPort;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigClient()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigClient));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownTcpPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownUdpPort = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUdpPort)).BeginInit();
            this.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownTcpPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownUdpPort);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownTcpPort
            // 
            this.numericUpDownTcpPort.AccessibleDescription = null;
            this.numericUpDownTcpPort.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTcpPort, "numericUpDownTcpPort");
            this.numericUpDownTcpPort.Font = null;
            this.numericUpDownTcpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownTcpPort.Name = "numericUpDownTcpPort";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownUdpPort
            // 
            this.numericUpDownUdpPort.AccessibleDescription = null;
            this.numericUpDownUdpPort.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownUdpPort, "numericUpDownUdpPort");
            this.numericUpDownUdpPort.Font = null;
            this.numericUpDownUdpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownUdpPort.Name = "numericUpDownUdpPort";
            this.numericUpDownUdpPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FormConfigClient
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigClient";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigClient_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTcpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUdpPort)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigClient_Load(object sender, System.EventArgs e)
		{
			this.numericUpDownUdpPort.Value = ConfigNetCommon.nServerPort;
			this.numericUpDownTcpPort.Value = configStruct.nTcpPort;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ConfigNetCommon.nServerPort = ConvertTool.ToInt32(this.numericUpDownUdpPort.Value);
			configStruct.nTcpPort = ConvertTool.ToInt32(this.numericUpDownTcpPort.Value);

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
