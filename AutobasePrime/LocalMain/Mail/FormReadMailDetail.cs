using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using AutoLibLocal;
using AutoLib;

namespace LocalMain.Mail
{
	/// <summary>
	/// Summary description for FormReadMailDetail.
	/// </summary>
	public class FormReadMailDetail : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBoxBody;
		private System.Windows.Forms.Button buttonOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public string sReadMailDetailFile;

		public FormReadMailDetail()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReadMailDetail));
            this.textBoxBody = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxBody
            // 
            resources.ApplyResources(this.textBoxBody, "textBoxBody");
            this.textBoxBody.Name = "textBoxBody";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            // 
            // FormReadMailDetail
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxBody);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReadMailDetail";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormReadMailDetail_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FormReadMailDetail_Load(object sender, System.EventArgs e)
		{
			string filename;
			filename = String.Format("{0}\\user\\{1}\\{2}", TotalConfig.AutoBaseIniGetConfigDirectory(), SharedData.userInfo.sUsername, sReadMailDetailFile);
			TextReader reader = new StreamReader(filename);
			if(reader == null)	return;
			this.textBoxBody.Text = reader.ReadToEnd();
			reader.Close();

			this.buttonOK.Select();
		}
	}
}

