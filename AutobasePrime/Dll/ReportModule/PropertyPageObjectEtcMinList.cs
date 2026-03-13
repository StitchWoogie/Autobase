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
	public class PropertyPageObjectEtcMinList : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonUnit0;
		private System.Windows.Forms.RadioButton radioButtonUnit1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textBoxDataGab;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxUseTo;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectEtcMinList()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectEtcMinList));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonUnit1 = new System.Windows.Forms.RadioButton();
            this.radioButtonUnit0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxDataGab = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxUseTo = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            // checkBoxUseTo
            // 
            resources.ApplyResources(this.checkBoxUseTo, "checkBoxUseTo");
            this.checkBoxUseTo.Name = "checkBoxUseTo";
            // 
            // PropertyPageObjectEtcMinList
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxUseTo);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "PropertyPageObjectEtcMinList";
            this.Load += new System.EventHandler(this.PropertyPageObjectEtcMinList_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetItem(OBJECT_ETC_MIN_LIST obj)
		{
			this.radioButtonUnit0.Checked = (obj.cDataUnit == 0);
			this.radioButtonUnit1.Checked = (obj.cDataUnit == 1);

			this.textBoxDataGab.Text = obj.min_gab;
			this.checkBoxUseTo.Checked = obj.bUseTo == 1;
		}

		public void GetItem(OBJECT_ETC_MIN_LIST obj)
		{
			if(this.radioButtonUnit0.Checked)		obj.cDataUnit = 0;
			else if(this.radioButtonUnit1.Checked)	obj.cDataUnit = 1;
			else									obj.cDataUnit = 0;

			obj.min_gab = this.textBoxDataGab.Text;
			obj.bUseTo = this.checkBoxUseTo.Checked ? (sbyte)1 : (sbyte)0;
		}

		private void PropertyPageObjectEtcMinList_Load(object sender, System.EventArgs e)
		{
		
		}
	}
}
