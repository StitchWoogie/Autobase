using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExportCDTP
{
	/// <summary>
	/// Summary description for FormProtocolOption.
	/// </summary>
	public class FormProtocolOption : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button buttonCancel;		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormProtocolOption()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormProtocolOption));
			this.button1 = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.AccessibleDescription = resources.GetString("button1.AccessibleDescription");
			this.button1.AccessibleName = resources.GetString("button1.AccessibleName");
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("button1.Anchor")));
			this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
			this.button1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("button1.Dock")));
			this.button1.Enabled = ((bool)(resources.GetObject("button1.Enabled")));
			this.button1.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("button1.FlatStyle")));
			this.button1.Font = ((System.Drawing.Font)(resources.GetObject("button1.Font")));
			this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
			this.button1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button1.ImageAlign")));
			this.button1.ImageIndex = ((int)(resources.GetObject("button1.ImageIndex")));
			this.button1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("button1.ImeMode")));
			this.button1.Location = ((System.Drawing.Point)(resources.GetObject("button1.Location")));
			this.button1.Name = "button1";
			this.button1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("button1.RightToLeft")));
			this.button1.Size = ((System.Drawing.Size)(resources.GetObject("button1.Size")));
			this.button1.TabIndex = ((int)(resources.GetObject("button1.TabIndex")));
			this.button1.Text = resources.GetString("button1.Text");
			this.button1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("button1.TextAlign")));
			this.button1.Visible = ((bool)(resources.GetObject("button1.Visible")));
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.AccessibleDescription = resources.GetString("buttonCancel.AccessibleDescription");
			this.buttonCancel.AccessibleName = resources.GetString("buttonCancel.AccessibleName");
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonCancel.Anchor")));
			this.buttonCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonCancel.BackgroundImage")));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonCancel.Dock")));
			this.buttonCancel.Enabled = ((bool)(resources.GetObject("buttonCancel.Enabled")));
			this.buttonCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonCancel.FlatStyle")));
			this.buttonCancel.Font = ((System.Drawing.Font)(resources.GetObject("buttonCancel.Font")));
			this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
			this.buttonCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonCancel.ImageAlign")));
			this.buttonCancel.ImageIndex = ((int)(resources.GetObject("buttonCancel.ImageIndex")));
			this.buttonCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonCancel.ImeMode")));
			this.buttonCancel.Location = ((System.Drawing.Point)(resources.GetObject("buttonCancel.Location")));
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonCancel.RightToLeft")));
			this.buttonCancel.Size = ((System.Drawing.Size)(resources.GetObject("buttonCancel.Size")));
			this.buttonCancel.TabIndex = ((int)(resources.GetObject("buttonCancel.TabIndex")));
			this.buttonCancel.Text = resources.GetString("buttonCancel.Text");
			this.buttonCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonCancel.TextAlign")));
			this.buttonCancel.Visible = ((bool)(resources.GetObject("buttonCancel.Visible")));
			// 
			// FormProtocolOption
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.button1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "FormProtocolOption";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.ShowInTaskbar = false;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{			
			DialogResult = DialogResult.OK;			
			Close();
		}
	}
}
