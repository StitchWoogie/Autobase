using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ReportModule
{
	/// <summary>
	/// Summary description for DialogHeadToolLine.
	/// </summary>
	public class DialogHeadToolLine : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.VScrollBar vScrollBarThick;
		private System.Windows.Forms.Panel panelThick;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DialogHeadToolLine()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogHeadToolLine));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.vScrollBarThick = new System.Windows.Forms.VScrollBar();
            this.panelThick = new System.Windows.Forms.Panel();
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
            // vScrollBarThick
            // 
            this.vScrollBarThick.AccessibleDescription = null;
            this.vScrollBarThick.AccessibleName = null;
            resources.ApplyResources(this.vScrollBarThick, "vScrollBarThick");
            this.vScrollBarThick.BackgroundImage = null;
            this.vScrollBarThick.Font = null;
            this.vScrollBarThick.Maximum = 29;
            this.vScrollBarThick.Minimum = 1;
            this.vScrollBarThick.Name = "vScrollBarThick";
            this.vScrollBarThick.Value = 1;
            this.vScrollBarThick.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarThick_Scroll);
            // 
            // panelThick
            // 
            this.panelThick.AccessibleDescription = null;
            this.panelThick.AccessibleName = null;
            resources.ApplyResources(this.panelThick, "panelThick");
            this.panelThick.BackgroundImage = null;
            this.panelThick.Font = null;
            this.panelThick.Name = "panelThick";
            this.panelThick.Paint += new System.Windows.Forms.PaintEventHandler(this.panelThick_Paint);
            // 
            // DialogHeadToolLine
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.vScrollBarThick);
            this.Controls.Add(this.panelThick);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogHeadToolLine";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.DialogHeadToolLine_Load);
            this.ResumeLayout(false);

		}
		#endregion

		public int nLineThick = 1;

		private void panelThick_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			int pos = panelThick.ClientRectangle.Top+panelThick.ClientSize.Height/2 - nLineThick/2;
			Color color;
			color = Color.Black;
			DrawClass.gcls(e.Graphics, panelThick.ClientRectangle.Left+2, pos, panelThick.ClientRectangle.Right-2, pos+nLineThick-1, color); 
		}

		private void vScrollBarThick_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			nLineThick = 21-this.vScrollBarThick.Value;
			if(nLineThick < 1)	nLineThick = 1;
			if(nLineThick > 20) nLineThick = 20;
			panelThick.Invalidate();
		}

		private void DialogHeadToolLine_Load(object sender, System.EventArgs e)
		{
			this.vScrollBarThick.Value = 21-nLineThick;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}
	}
}
