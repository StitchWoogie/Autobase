using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for show_errorMessageDlg.
	/// </summary>
	public class show_errorMessageDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelText;
		private System.Windows.Forms.Button button_OK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public	string	sText = "";


		public show_errorMessageDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(show_errorMessageDlg));
            this.labelText = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelText
            // 
            resources.ApplyResources(this.labelText, "labelText");
            this.labelText.Name = "labelText";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            // 
            // show_errorMessageDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "show_errorMessageDlg";
            this.Load += new System.EventHandler(this.show_errorMessageDlg_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void show_errorMessageDlg_Load(object sender, System.EventArgs e)
		{
			this.labelText.Text = sText;			
		}
	}
}
