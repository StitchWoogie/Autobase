using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageObjectAiMaxSum.
	/// </summary>
	public class PropertyPageObjectAiMaxSum : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonData0;
		private System.Windows.Forms.RadioButton radioButtonData1;
		private System.Windows.Forms.RadioButton radioButtonData2;
		private System.Windows.Forms.RadioButton radioButtonData3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectAiMaxSum()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAiMaxSum));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonData3 = new System.Windows.Forms.RadioButton();
            this.radioButtonData2 = new System.Windows.Forms.RadioButton();
            this.radioButtonData1 = new System.Windows.Forms.RadioButton();
            this.radioButtonData0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonData3);
            this.groupBox1.Controls.Add(this.radioButtonData2);
            this.groupBox1.Controls.Add(this.radioButtonData1);
            this.groupBox1.Controls.Add(this.radioButtonData0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonData3
            // 
            this.radioButtonData3.AccessibleDescription = null;
            this.radioButtonData3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonData3, "radioButtonData3");
            this.radioButtonData3.BackgroundImage = null;
            this.radioButtonData3.Font = null;
            this.radioButtonData3.Name = "radioButtonData3";
            // 
            // radioButtonData2
            // 
            this.radioButtonData2.AccessibleDescription = null;
            this.radioButtonData2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonData2, "radioButtonData2");
            this.radioButtonData2.BackgroundImage = null;
            this.radioButtonData2.Font = null;
            this.radioButtonData2.Name = "radioButtonData2";
            // 
            // radioButtonData1
            // 
            this.radioButtonData1.AccessibleDescription = null;
            this.radioButtonData1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonData1, "radioButtonData1");
            this.radioButtonData1.BackgroundImage = null;
            this.radioButtonData1.Font = null;
            this.radioButtonData1.Name = "radioButtonData1";
            // 
            // radioButtonData0
            // 
            this.radioButtonData0.AccessibleDescription = null;
            this.radioButtonData0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonData0, "radioButtonData0");
            this.radioButtonData0.BackgroundImage = null;
            this.radioButtonData0.Font = null;
            this.radioButtonData0.Name = "radioButtonData0";
            // 
            // PropertyPageObjectAiMaxSum
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectAiMaxSum";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetItem(OBJECT_AI_MAX_SUM obj)
		{
			this.radioButtonData0.Checked = (obj.nDataType == 0);
			this.radioButtonData1.Checked = (obj.nDataType == 1);
			this.radioButtonData2.Checked = (obj.nDataType == 2);
			this.radioButtonData3.Checked = (obj.nDataType == 3);
		}

		public void GetItem(OBJECT_AI_MAX_SUM obj)
		{
			if(this.radioButtonData0.Checked)		obj.nDataType = 0;
			else if(this.radioButtonData1.Checked)	obj.nDataType = 1;
			else if(this.radioButtonData2.Checked)	obj.nDataType = 2;
			else if(this.radioButtonData3.Checked)	obj.nDataType = 3;
			else									obj.nDataType = 0;
		}
	}
}
