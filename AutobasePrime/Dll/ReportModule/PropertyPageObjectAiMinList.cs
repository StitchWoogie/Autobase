using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageObjectAiMinList.
	/// </summary>
	public class PropertyPageObjectAiMinList : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonType0;
		private System.Windows.Forms.RadioButton radioButtonType1;
		private System.Windows.Forms.RadioButton radioButtonType2;
		private System.Windows.Forms.RadioButton radioButtonType3;
		private System.Windows.Forms.RadioButton radioButtonType4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonUnit0;
		private System.Windows.Forms.RadioButton radioButtonUnit1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textBoxDataGab;
		private System.Windows.Forms.Label label1;
        private RadioButton radioButtonType5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectAiMinList()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAiMinList));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonType0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonUnit1 = new System.Windows.Forms.RadioButton();
            this.radioButtonUnit0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxDataGab = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonType5);
            this.groupBox1.Controls.Add(this.radioButtonType4);
            this.groupBox1.Controls.Add(this.radioButtonType3);
            this.groupBox1.Controls.Add(this.radioButtonType2);
            this.groupBox1.Controls.Add(this.radioButtonType1);
            this.groupBox1.Controls.Add(this.radioButtonType0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonType5
            // 
            resources.ApplyResources(this.radioButtonType5, "radioButtonType5");
            this.radioButtonType5.Name = "radioButtonType5";
            // 
            // radioButtonType4
            // 
            resources.ApplyResources(this.radioButtonType4, "radioButtonType4");
            this.radioButtonType4.Name = "radioButtonType4";
            // 
            // radioButtonType3
            // 
            resources.ApplyResources(this.radioButtonType3, "radioButtonType3");
            this.radioButtonType3.Name = "radioButtonType3";
            // 
            // radioButtonType2
            // 
            resources.ApplyResources(this.radioButtonType2, "radioButtonType2");
            this.radioButtonType2.Name = "radioButtonType2";
            this.radioButtonType2.CheckedChanged += new System.EventHandler(this.radioButtonType2_CheckedChanged);
            // 
            // radioButtonType1
            // 
            resources.ApplyResources(this.radioButtonType1, "radioButtonType1");
            this.radioButtonType1.Name = "radioButtonType1";
            // 
            // radioButtonType0
            // 
            resources.ApplyResources(this.radioButtonType0, "radioButtonType0");
            this.radioButtonType0.Name = "radioButtonType0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonUnit1);
            this.groupBox2.Controls.Add(this.radioButtonUnit0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonUnit1
            // 
            resources.ApplyResources(this.radioButtonUnit1, "radioButtonUnit1");
            this.radioButtonUnit1.Name = "radioButtonUnit1";
            // 
            // radioButtonUnit0
            // 
            resources.ApplyResources(this.radioButtonUnit0, "radioButtonUnit0");
            this.radioButtonUnit0.Name = "radioButtonUnit0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxDataGab);
            this.groupBox3.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // textBoxDataGab
            // 
            resources.ApplyResources(this.textBoxDataGab, "textBoxDataGab");
            this.textBoxDataGab.Name = "textBoxDataGab";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PropertyPageObjectAiMinList
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "PropertyPageObjectAiMinList";
            this.Load += new System.EventHandler(this.PropertyPageObjectAiMinList_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetItem(OBJECT_AI_MIN_LIST obj)
		{
			this.radioButtonType0.Checked = (obj.data_type == 0);
			this.radioButtonType1.Checked = (obj.data_type == 1);
			this.radioButtonType2.Checked = (obj.data_type == 2);
			this.radioButtonType3.Checked = (obj.data_type == 3);
			this.radioButtonType4.Checked = (obj.data_type == 4);
            this.radioButtonType5.Checked = (obj.data_type == 5);

			this.radioButtonUnit0.Checked = (obj.cDataUnit == 0);
			this.radioButtonUnit1.Checked = (obj.cDataUnit == 1);

			this.textBoxDataGab.Text = obj.min_gab;
		}

		public void GetItem(OBJECT_AI_MIN_LIST obj)
		{
			if(this.radioButtonType0.Checked)		obj.data_type = 0;
			else if(this.radioButtonType1.Checked)	obj.data_type = 1;
			else if(this.radioButtonType2.Checked)	obj.data_type = 2;
			else if(this.radioButtonType3.Checked)	obj.data_type = 3;
			else if(this.radioButtonType4.Checked)	obj.data_type = 4;
            else if (this.radioButtonType5.Checked) obj.data_type = 5;
			else									obj.data_type = 0;

			if(this.radioButtonUnit0.Checked)		obj.cDataUnit = 0;
			else if(this.radioButtonUnit1.Checked)	obj.cDataUnit = 1;
			else									obj.cDataUnit = 0;

			obj.min_gab = this.textBoxDataGab.Text;
		}

		private void radioButtonType2_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

        private void PropertyPageObjectAiMinList_Load(object sender, EventArgs e)
        {

        }
	}
}
