using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using System.Data;

namespace ViewMain
{
	/// <summary>
	/// Summary description for TestForm.
	/// </summary>
	public class TestForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dataGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Timers.Timer timer1;

		SharedData shareData;

		public TestForm(ref SharedData share_data)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			shareData = share_data;
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
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.timer1 = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGrid1
			// 
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.ReadOnly = true;
			this.dataGrid1.Size = new System.Drawing.Size(496, 302);
			this.dataGrid1.TabIndex = 0;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.SynchronizingObject = this;
			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
			// 
			// TestForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(496, 302);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.dataGrid1});
			this.Name = "TestForm";
			this.Text = "TestForm";
			this.Load += new System.EventHandler(this.TestForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.timer1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void TestForm_Load(object sender, System.EventArgs e)
		{
			DataSet dsTag = shareData.dsTag;

			dataGrid1.DataSource = dsTag;
			
			/*
			DataTable dt = new DataTable("BroadCast");
			DataColumn col;
			col = new DataColumn("ID",System.Type.GetType("System.Int32"));
			dt.Columns.Add(col);
			col = new DataColumn("Value");
			dt.Columns.Add(col);

			DataRow row;
			
			for(int i = 0; i < 100; i++) 
			{
				row = dt.NewRow();
				row["ID"] = i;
				row["Value"] = i;

				dt.Rows.Add(row);
			}

			//DataSet ds = new DataSet("BroadCast");

			//ds.Tables.Add(dt);
			//tagInfoAI.SetBroadCastTagAI(ds);
		*/
		}

		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			/*
			DataSet ds = tagInfoAI.GetBroadCastTagAI();

			DataRow row_s;
			DataRow row_t;
			DataTable table_t;
			DataTable table_s = ds.Tables[0];
			int id_s;
			int id_t;

			table_t = dsTagInfoAI.Tables[0];

			for(int i = 0; i < table_s.Rows.Count; i++) 
			{
				row_s = table_s.Rows[i];
				id_s = (int)row_s[0];

				for(int j = 0; j < table_t.Rows.Count; j++) 
				{
					row_t = table_t.Rows[j];

					id_t = (int)row_t[0];
					
					if(id_s == id_t) 
					{
						row_t.BeginEdit();
						row_t["Value"] = row_s["Value"];
						row_t.EndEdit();
						break;
					}
				}
			}
		*/
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
		
		}
	}
}
