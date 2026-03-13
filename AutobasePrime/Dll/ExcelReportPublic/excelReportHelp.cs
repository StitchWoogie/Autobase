using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using System.Reflection;
using System.Threading;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for excelReportHelp.
	/// </summary>
	public class excelReportHelp : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Label labelVersionTotal;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public excelReportHelp()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(excelReportHelp));
            this.button_OK = new System.Windows.Forms.Button();
            this.labelVersionTotal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            // 
            // labelVersionTotal
            // 
            resources.ApplyResources(this.labelVersionTotal, "labelVersionTotal");
            this.labelVersionTotal.Name = "labelVersionTotal";
            // 
            // excelReportHelp
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.labelVersionTotal);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "excelReportHelp";
            this.Load += new System.EventHandler(this.excelReportHelp_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void excelReportHelp_Load(object sender, System.EventArgs e)
		{
            if (TotalConfig.bVersionCertificationSmallBusinessProducts)
            {
                labelVersionTotal.Text = String.Format("ExcelReportData.dll Version = {0}", TotalConfig.AutoBaseIntGetVersionString());
            }
            else
            {
                Version version = GetProgramVersion();
                labelVersionTotal.Text = String.Format("ExcelReportData.dll Version = {0}", version.ToString());
            }
		}

		public static Version GetProgramVersion()
		{
			Assembly[] myAssemblies = Thread.GetDomain().GetAssemblies();

			Assembly myAssembly = null;
			for(int i = 0; i < myAssemblies.Length; i++)
			{
				if(String.Compare(myAssemblies[i].GetName().Name, "ExcelReportData") == 0)
					myAssembly = myAssemblies[i];
			}

			Version version = new Version(1,0,0,0);

			if(myAssembly != null)
			{
				version = myAssembly.GetName().Version;
			}

			return version;
		}
	}
}
