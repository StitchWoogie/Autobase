using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace RunMain
{
	/// <summary>
	/// Summary description for FormShareServer.
	/// </summary>
	public class FormShareServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxTcpPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxUdpPort;
        private GroupBox groupBox2;
        private CheckBox checkBoxShowRunTagOFFStatus;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormShareServer()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShareServer));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxUdpPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTcpPort = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxShowRunTagOFFStatus = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxUdpPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxTcpPort);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxUdpPort
            // 
            resources.ApplyResources(this.textBoxUdpPort, "textBoxUdpPort");
            this.textBoxUdpPort.Name = "textBoxUdpPort";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxTcpPort
            // 
            resources.ApplyResources(this.textBoxTcpPort, "textBoxTcpPort");
            this.textBoxTcpPort.Name = "textBoxTcpPort";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxShowRunTagOFFStatus);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxShowRunTagOFFStatus
            // 
            resources.ApplyResources(this.checkBoxShowRunTagOFFStatus, "checkBoxShowRunTagOFFStatus");
            this.checkBoxShowRunTagOFFStatus.Name = "checkBoxShowRunTagOFFStatus";
            this.checkBoxShowRunTagOFFStatus.UseVisualStyleBackColor = true;
            // 
            // FormShareServer
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormShareServer";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormShareServer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormShareServer_Load(object sender, System.EventArgs e)
		{
			this.textBoxTcpPort.Text = ShareServerMain.nPortTcp.ToString();
			//this.textBoxUdpPort.Text = ShareServerMain.nPortUdp.ToString();

            this.checkBoxShowRunTagOFFStatus.Checked = ShareServerMain.bShowRunTagOffStatus;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			int tcp = ConvertTool.ToInt32(this.textBoxTcpPort.Text);
			int udp = ConvertTool.ToInt32(this.textBoxUdpPort.Text);

			ShareServerMain.UnInit();
			ShareServerMain.nPortTcp = tcp;
            ShareServerMain.bShowRunTagOffStatus = this.checkBoxShowRunTagOFFStatus.Checked;
			//ShareServerMain.nPortUdp = udp;
            ShareServerMain.SaveConfg();
			ShareServerMain.Init();

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
