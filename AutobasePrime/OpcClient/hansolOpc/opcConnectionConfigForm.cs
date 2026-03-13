using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcConnectionConfigForm.
	/// </summary>
	public class opcConnectionConfigForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		public  System.Windows.Forms.NumericUpDown numericUpDown_retryTime;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		public  System.Windows.Forms.CheckBox checkBox_useRetry;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public opcConnectionConfigForm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(opcConnectionConfigForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_useRetry = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_retryTime = new System.Windows.Forms.NumericUpDown();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_retryTime)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.checkBox_useRetry);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDown_retryTime);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBox_useRetry
            // 
            this.checkBox_useRetry.AccessibleDescription = null;
            this.checkBox_useRetry.AccessibleName = null;
            resources.ApplyResources(this.checkBox_useRetry, "checkBox_useRetry");
            this.checkBox_useRetry.BackgroundImage = null;
            this.checkBox_useRetry.Font = null;
            this.checkBox_useRetry.Name = "checkBox_useRetry";
            this.checkBox_useRetry.CheckedChanged += new System.EventHandler(this.checkBox_useRetry_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDown_retryTime
            // 
            this.numericUpDown_retryTime.AccessibleDescription = null;
            this.numericUpDown_retryTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDown_retryTime, "numericUpDown_retryTime");
            this.numericUpDown_retryTime.Font = null;
            this.numericUpDown_retryTime.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown_retryTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_retryTime.Name = "numericUpDown_retryTime";
            this.numericUpDown_retryTime.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleDescription = null;
            this.button_Cancel.AccessibleName = null;
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.BackgroundImage = null;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Font = null;
            this.button_Cancel.Name = "button_Cancel";
            // 
            // button_OK
            // 
            this.button_OK.AccessibleDescription = null;
            this.button_OK.AccessibleName = null;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.BackgroundImage = null;
            this.button_OK.Font = null;
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // opcConnectionConfigForm
            // 
            this.AcceptButton = this.button_OK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.button_Cancel;
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "opcConnectionConfigForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.opcConnectionConfigForm_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_retryTime)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		void enabelDisable()
		{
			this.numericUpDown_retryTime.Enabled = this.checkBox_useRetry.Checked;
		}

		private void checkBox_useRetry_CheckedChanged(object sender, System.EventArgs e)
		{
			enabelDisable();
		}

		private void opcConnectionConfigForm_Load(object sender, System.EventArgs e)
		{
			try 
			{
				checkBox_useRetry.Checked = opcBasic.opcClientConfig.bRetryConnect;
				this.numericUpDown_retryTime.Value = opcBasic.opcClientConfig.nRetryMin;
			}
			catch{}
			enabelDisable();
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			opcBasic.opcClientConfig.bRetryConnect = checkBox_useRetry.Checked;
			opcBasic.opcClientConfig.nRetryMin = this.numericUpDown_retryTime.Value;
			this.DialogResult = DialogResult.OK;
			Close();
		}
	}
}

