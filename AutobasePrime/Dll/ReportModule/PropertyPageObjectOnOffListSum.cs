using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageObjectOnOffList.
	/// </summary>
	public class PropertyPageObjectOnOffListSum : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonTime0;
		private System.Windows.Forms.RadioButton radioButtonTime1;
		private System.Windows.Forms.CheckBox checkBoxUseMinListTime;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectOnOffListSum()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectOnOffListSum));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTime1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTime0 = new System.Windows.Forms.RadioButton();
            this.checkBoxUseMinListTime = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonTime1);
            this.groupBox1.Controls.Add(this.radioButtonTime0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonTime1
            // 
            this.radioButtonTime1.AccessibleDescription = null;
            this.radioButtonTime1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonTime1, "radioButtonTime1");
            this.radioButtonTime1.BackgroundImage = null;
            this.radioButtonTime1.Font = null;
            this.radioButtonTime1.Name = "radioButtonTime1";
            // 
            // radioButtonTime0
            // 
            this.radioButtonTime0.AccessibleDescription = null;
            this.radioButtonTime0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonTime0, "radioButtonTime0");
            this.radioButtonTime0.BackgroundImage = null;
            this.radioButtonTime0.Font = null;
            this.radioButtonTime0.Name = "radioButtonTime0";
            // 
            // checkBoxUseMinListTime
            // 
            this.checkBoxUseMinListTime.AccessibleDescription = null;
            this.checkBoxUseMinListTime.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseMinListTime, "checkBoxUseMinListTime");
            this.checkBoxUseMinListTime.BackgroundImage = null;
            this.checkBoxUseMinListTime.Font = null;
            this.checkBoxUseMinListTime.Name = "checkBoxUseMinListTime";
            // 
            // PropertyPageObjectOnOffListSum
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.checkBoxUseMinListTime);
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectOnOffListSum";
            this.Load += new System.EventHandler(this.PropertyPageObjectOnOffList_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageObjectOnOffList_Load(object sender, System.EventArgs e)
		{
		
		}

		public void SetItem(OBJECT_DI_ONOFF_LIST_SUM obj)
		{
			this.radioButtonTime0.Checked = (obj.field_time == 0);
			this.radioButtonTime1.Checked = (obj.field_time == 1);

			//this.radioButtonSort0.Checked = (obj.nSort == 0);
			//this.radioButtonSort1.Checked = (obj.nSort == 1);
			//this.radioButtonSort2.Checked = (obj.nSort == 2);

			//this.numericUpDownViewColumn.Value = obj.field_view;
			this.checkBoxUseMinListTime.Checked = (obj.bUseFromToTime == 1);
		}

		public void GetItem(OBJECT_DI_ONOFF_LIST_SUM obj)
		{
			if(this.radioButtonTime0.Checked)		obj.field_time = 0;
			else if(this.radioButtonTime1.Checked)	obj.field_time = 1;
			else									obj.field_time = 0;

			//if(this.radioButtonSort0.Checked)		obj.nSort = 0;
			//else if(this.radioButtonSort1.Checked)	obj.nSort = 1;
			//else if(this.radioButtonSort2.Checked)	obj.nSort = 2;
			//else									obj.nSort = 0;

			//obj.field_view = ConvertTool.ToInt32(this.numericUpDownViewColumn.Value);
			obj.bUseFromToTime = this.checkBoxUseMinListTime.Checked ? (sbyte)1 : (sbyte)0;
		}
	}
}
