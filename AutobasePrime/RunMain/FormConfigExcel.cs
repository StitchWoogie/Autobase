using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace RunMain
{
	/// <summary>
	/// Summary description for FormConfigExcel.
	/// </summary>
	public class FormConfigExcel : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxPath;
		private System.Windows.Forms.Button buttonFind;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigExcel()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigExcel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonFind);
            this.groupBox1.Controls.Add(this.textBoxPath);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonFind
            // 
            resources.ApplyResources(this.buttonFind, "buttonFind");
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // textBoxPath
            // 
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
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
            // FormConfigExcel
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigExcel";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigExcel_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigExcel_Load(object sender, System.EventArgs e)
		{
			textBoxPath.Text = TotalConfig.LoadRegAutoBaseConfig("ExcelReport", null, "ExcelPath", "C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE"); 
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			TotalConfig.SaveRegAutoBaseConfig("ExcelReport", null, "ExcelPath", textBoxPath.Text); 
			Close();
		}

		private void buttonFind_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "EXE files (*.exe) |*.exe";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				textBoxPath.Text = dialog.FileName;
			}
		}
	}
}
