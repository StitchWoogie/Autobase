using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ReportModule
{
	/// <summary>
	/// Summary description for DialogHeadToolText.
	/// </summary>
	public class DialogHeadToolText : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonPage;
		private System.Windows.Forms.Button buttonDate;
		private System.Windows.Forms.Button buttonFilename;
		private System.Windows.Forms.Button buttonTotalPage;
		private System.Windows.Forms.Button buttonTime;
		public System.Windows.Forms.TextBox textBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DialogHeadToolText()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogHeadToolText));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonPage = new System.Windows.Forms.Button();
            this.buttonDate = new System.Windows.Forms.Button();
            this.buttonFilename = new System.Windows.Forms.Button();
            this.buttonTotalPage = new System.Windows.Forms.Button();
            this.buttonTime = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
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
            // buttonPage
            // 
            this.buttonPage.AccessibleDescription = null;
            this.buttonPage.AccessibleName = null;
            resources.ApplyResources(this.buttonPage, "buttonPage");
            this.buttonPage.BackgroundImage = null;
            this.buttonPage.Font = null;
            this.buttonPage.Name = "buttonPage";
            this.buttonPage.Click += new System.EventHandler(this.buttonPage_Click);
            // 
            // buttonDate
            // 
            this.buttonDate.AccessibleDescription = null;
            this.buttonDate.AccessibleName = null;
            resources.ApplyResources(this.buttonDate, "buttonDate");
            this.buttonDate.BackgroundImage = null;
            this.buttonDate.Font = null;
            this.buttonDate.Name = "buttonDate";
            this.buttonDate.Click += new System.EventHandler(this.buttonDate_Click);
            // 
            // buttonFilename
            // 
            this.buttonFilename.AccessibleDescription = null;
            this.buttonFilename.AccessibleName = null;
            resources.ApplyResources(this.buttonFilename, "buttonFilename");
            this.buttonFilename.BackgroundImage = null;
            this.buttonFilename.Font = null;
            this.buttonFilename.Name = "buttonFilename";
            this.buttonFilename.Click += new System.EventHandler(this.buttonFilename_Click);
            // 
            // buttonTotalPage
            // 
            this.buttonTotalPage.AccessibleDescription = null;
            this.buttonTotalPage.AccessibleName = null;
            resources.ApplyResources(this.buttonTotalPage, "buttonTotalPage");
            this.buttonTotalPage.BackgroundImage = null;
            this.buttonTotalPage.Font = null;
            this.buttonTotalPage.Name = "buttonTotalPage";
            this.buttonTotalPage.Click += new System.EventHandler(this.buttonTotalPage_Click);
            // 
            // buttonTime
            // 
            this.buttonTime.AccessibleDescription = null;
            this.buttonTime.AccessibleName = null;
            resources.ApplyResources(this.buttonTime, "buttonTime");
            this.buttonTime.BackgroundImage = null;
            this.buttonTime.Font = null;
            this.buttonTime.Name = "buttonTime";
            this.buttonTime.Click += new System.EventHandler(this.buttonTime_Click);
            // 
            // textBox
            // 
            this.textBox.AccessibleDescription = null;
            this.textBox.AccessibleName = null;
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.BackgroundImage = null;
            this.textBox.Font = null;
            this.textBox.Name = "textBox";
            // 
            // DialogHeadToolText
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.buttonTime);
            this.Controls.Add(this.buttonTotalPage);
            this.Controls.Add(this.buttonFilename);
            this.Controls.Add(this.buttonDate);
            this.Controls.Add(this.buttonPage);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogHeadToolText";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonPage_Click(object sender, System.EventArgs e)
		{
			textBox.Text += "&[Page]";
		}

		private void buttonDate_Click(object sender, System.EventArgs e)
		{
			textBox.Text += "&[Date]";
		}

		private void buttonFilename_Click(object sender, System.EventArgs e)
		{
			textBox.Text += "&[FileName]";
		}

		private void buttonTotalPage_Click(object sender, System.EventArgs e)
		{
			textBox.Text += "&[TotalPage]";
		}

		private void buttonTime_Click(object sender, System.EventArgs e)
		{
			textBox.Text += "&[Time]";
		}
	}
}
