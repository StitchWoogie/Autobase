using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using System.Data;

namespace ViewMain
{
	/// <summary>
	/// Summary description for FormTestLogList.
	/// </summary>
	public class FormTestLogList : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dataGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		SharedData sharedData;

		public FormTestLogList(SharedData shared_data)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			sharedData = shared_data;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTestLogList));
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.dataGrid1, "dataGrid1");
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dataGrid1_Navigate);
            // 
            // FormTestLogList
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.dataGrid1);
            this.Name = "FormTestLogList";
            this.Load += new System.EventHandler(this.FormTestLogList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void dataGrid1_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
		{
		
		}

		private async void FormTestLogList_Load(object sender, System.EventArgs e)
		{
			DataGate gate = new DataGate();
			DataSet ds = await gate.GetDataAi("AI_0000", EnumDataType.AveMinMaxSum, EnumDataTime.Week, 2002, 1, 31, 0, 0, 60, 1);

			dataGrid1.DataSource = ds;
		}
	}
}
