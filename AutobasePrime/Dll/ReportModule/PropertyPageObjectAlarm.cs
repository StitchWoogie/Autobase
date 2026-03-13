using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageObjectAlarm.
	/// </summary>
	public class PropertyPageObjectAlarm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonColumn0;
		private System.Windows.Forms.RadioButton radioButtonColumn1;
		private System.Windows.Forms.RadioButton radioButtonColumn2;
		private System.Windows.Forms.RadioButton radioButtonColumn3;
		private System.Windows.Forms.RadioButton radioButtonColumn4;
		private System.Windows.Forms.RadioButton radioButtonColumn5;
		private System.Windows.Forms.RadioButton radioButtonColumn6;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectAlarm()
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
		protected override void Dispose(bool disposing)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAlarm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonColumn6 = new System.Windows.Forms.RadioButton();
            this.radioButtonColumn5 = new System.Windows.Forms.RadioButton();
            this.radioButtonColumn4 = new System.Windows.Forms.RadioButton();
            this.radioButtonColumn3 = new System.Windows.Forms.RadioButton();
            this.radioButtonColumn2 = new System.Windows.Forms.RadioButton();
            this.radioButtonColumn1 = new System.Windows.Forms.RadioButton();
            this.radioButtonColumn0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonColumn6);
            this.groupBox1.Controls.Add(this.radioButtonColumn5);
            this.groupBox1.Controls.Add(this.radioButtonColumn4);
            this.groupBox1.Controls.Add(this.radioButtonColumn3);
            this.groupBox1.Controls.Add(this.radioButtonColumn2);
            this.groupBox1.Controls.Add(this.radioButtonColumn1);
            this.groupBox1.Controls.Add(this.radioButtonColumn0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonColumn6
            // 
            this.radioButtonColumn6.AccessibleDescription = null;
            this.radioButtonColumn6.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn6, "radioButtonColumn6");
            this.radioButtonColumn6.BackgroundImage = null;
            this.radioButtonColumn6.Font = null;
            this.radioButtonColumn6.Name = "radioButtonColumn6";
            // 
            // radioButtonColumn5
            // 
            this.radioButtonColumn5.AccessibleDescription = null;
            this.radioButtonColumn5.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn5, "radioButtonColumn5");
            this.radioButtonColumn5.BackgroundImage = null;
            this.radioButtonColumn5.Font = null;
            this.radioButtonColumn5.Name = "radioButtonColumn5";
            // 
            // radioButtonColumn4
            // 
            this.radioButtonColumn4.AccessibleDescription = null;
            this.radioButtonColumn4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn4, "radioButtonColumn4");
            this.radioButtonColumn4.BackgroundImage = null;
            this.radioButtonColumn4.Font = null;
            this.radioButtonColumn4.Name = "radioButtonColumn4";
            // 
            // radioButtonColumn3
            // 
            this.radioButtonColumn3.AccessibleDescription = null;
            this.radioButtonColumn3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn3, "radioButtonColumn3");
            this.radioButtonColumn3.BackgroundImage = null;
            this.radioButtonColumn3.Font = null;
            this.radioButtonColumn3.Name = "radioButtonColumn3";
            // 
            // radioButtonColumn2
            // 
            this.radioButtonColumn2.AccessibleDescription = null;
            this.radioButtonColumn2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn2, "radioButtonColumn2");
            this.radioButtonColumn2.BackgroundImage = null;
            this.radioButtonColumn2.Font = null;
            this.radioButtonColumn2.Name = "radioButtonColumn2";
            // 
            // radioButtonColumn1
            // 
            this.radioButtonColumn1.AccessibleDescription = null;
            this.radioButtonColumn1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn1, "radioButtonColumn1");
            this.radioButtonColumn1.BackgroundImage = null;
            this.radioButtonColumn1.Font = null;
            this.radioButtonColumn1.Name = "radioButtonColumn1";
            // 
            // radioButtonColumn0
            // 
            this.radioButtonColumn0.AccessibleDescription = null;
            this.radioButtonColumn0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonColumn0, "radioButtonColumn0");
            this.radioButtonColumn0.BackgroundImage = null;
            this.radioButtonColumn0.Font = null;
            this.radioButtonColumn0.Name = "radioButtonColumn0";
            // 
            // PropertyPageObjectAlarm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectAlarm";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetItem(OBJECT_ALARM obj)
		{
			this.radioButtonColumn0.Checked = (obj.view == 0);
			this.radioButtonColumn1.Checked = (obj.view == 1);
			this.radioButtonColumn2.Checked = (obj.view == 2);
			this.radioButtonColumn3.Checked = (obj.view == 3);
			this.radioButtonColumn4.Checked = (obj.view == 4);
			this.radioButtonColumn5.Checked = (obj.view == 5);
			this.radioButtonColumn6.Checked = (obj.view == 6);
		}

		public void GetItem(OBJECT_ALARM obj)
		{
			if(this.radioButtonColumn0.Checked)			obj.view = 0;
			else if(this.radioButtonColumn1.Checked)	obj.view = 1;
			else if(this.radioButtonColumn2.Checked)	obj.view = 2;
			else if(this.radioButtonColumn3.Checked)	obj.view = 3;
			else if(this.radioButtonColumn4.Checked)	obj.view = 4;
			else if(this.radioButtonColumn5.Checked)	obj.view = 5;
			else if(this.radioButtonColumn6.Checked)	obj.view = 6;
			else										obj.view = 0;
		}
	}
}
