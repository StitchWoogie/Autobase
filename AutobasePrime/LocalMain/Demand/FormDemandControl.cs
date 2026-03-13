using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormDemandControl.
	/// </summary>
	public class FormDemandControl : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDemandControl()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDemandControl));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.AccessibleDescription = null;
            this.listBox1.AccessibleName = null;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.BackgroundImage = null;
            this.listBox1.Font = null;
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // splitter1
            // 
            this.splitter1.AccessibleDescription = null;
            this.splitter1.AccessibleName = null;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.BackgroundImage = null;
            this.splitter1.Font = null;
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // FormDemandControl
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.listBox1);
            this.Icon = null;
            this.Name = "FormDemandControl";
            this.Load += new System.EventHandler(this.FormDemandControl_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormDemandControl_FormClosed);
            this.ResumeLayout(false);

		}
		#endregion

		static int nTempNo = 0;

		GraphicModule.FormDemandControlChild childDemand;

		private void FormDemandControl_Load(object sender, System.EventArgs e)
		{
			childDemand = new GraphicModule.FormDemandControlChild();
			childDemand.TopLevel = false;
			childDemand.Dock = DockStyle.Fill;
			childDemand.SetControlNo(nTempNo);
            childDemand.SetBasicColor(); // 20250227 PSU Ãß°¡
			this.panel1.Controls.Add(childDemand);
			childDemand.Show();
						
			if(DemandControl.blockDemandControl.Count < 2) 
			{
				this.listBox1.Visible = false;
				this.splitter1.Visible = false;

				EnableSelectedDemand(nTempNo);
			}
			else 
			{
				FUNCTION_BLOCK_DEMAND_CONTROL item;

				for(int i = 0; i < DemandControl.blockDemandControl.Count; i++) 
				{
					item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[i];
					this.listBox1.Items.Add(item.title);
				}

				this.listBox1.SelectedIndex = nTempNo;
			}
		}

		void EnableSelectedDemand(int no)
		{
			childDemand.SetControlNo(no);
			childDemand.Invalidate();
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			nTempNo = this.listBox1.SelectedIndex;
			EnableSelectedDemand(nTempNo);
		}

        private void FormDemandControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            childDemand.Close();
        }
	}
}
