using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExportInfoVisionSI
{
	/// <summary>
	/// Summary description for FormProtocolOption.
	/// </summary>
	public class FormProtocolOption : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(120, 240);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "Close";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 128);
			this.label1.TabIndex = 1;
			this.label1.Text = "Data Read, Data Write Supported";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormProtocolOption
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProtocolOption";
			this.ShowInTaskbar = false;
			this.Text = "FormProtocolOption";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
