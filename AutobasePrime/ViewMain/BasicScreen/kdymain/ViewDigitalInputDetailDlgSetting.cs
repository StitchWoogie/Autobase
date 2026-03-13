using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputDetailDlgSetting.
	/// </summary>
	public class ViewDigitalInputDetailDlgSetting : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_data_read_period;
		public System.Windows.Forms.ComboBox comboBox_data_read_period;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		public MyNumericUpDown numericUpDown_total_time;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewDigitalInputDetailDlgSetting()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputDetailDlgSetting));
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox_data_read_period = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_data_read_period = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_total_time = new AutoLibLocal.MyNumericUpDown();
            this.groupBox_data_read_period.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_total_time)).BeginInit();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_cancel, "button_cancel");
            this.button_cancel.Name = "button_cancel";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            // 
            // groupBox_data_read_period
            // 
            this.groupBox_data_read_period.Controls.Add(this.label4);
            this.groupBox_data_read_period.Controls.Add(this.label3);
            this.groupBox_data_read_period.Controls.Add(this.comboBox_data_read_period);
            this.groupBox_data_read_period.Controls.Add(this.label2);
            this.groupBox_data_read_period.Controls.Add(this.numericUpDown_total_time);
            resources.ApplyResources(this.groupBox_data_read_period, "groupBox_data_read_period");
            this.groupBox_data_read_period.Name = "groupBox_data_read_period";
            this.groupBox_data_read_period.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBox_data_read_period
            // 
            this.comboBox_data_read_period.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_data_read_period.DropDownWidth = 72;
            resources.ApplyResources(this.comboBox_data_read_period, "comboBox_data_read_period");
            this.comboBox_data_read_period.Items.AddRange(new object[] {
            resources.GetString("comboBox_data_read_period.Items"),
            resources.GetString("comboBox_data_read_period.Items1"),
            resources.GetString("comboBox_data_read_period.Items2"),
            resources.GetString("comboBox_data_read_period.Items3"),
            resources.GetString("comboBox_data_read_period.Items4"),
            resources.GetString("comboBox_data_read_period.Items5"),
            resources.GetString("comboBox_data_read_period.Items6"),
            resources.GetString("comboBox_data_read_period.Items7"),
            resources.GetString("comboBox_data_read_period.Items8"),
            resources.GetString("comboBox_data_read_period.Items9")});
            this.comboBox_data_read_period.Name = "comboBox_data_read_period";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDown_total_time
            // 
            resources.ApplyResources(this.numericUpDown_total_time, "numericUpDown_total_time");
            this.numericUpDown_total_time.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_total_time.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_total_time.Name = "numericUpDown_total_time";
            this.numericUpDown_total_time.SampleProperty = 0;
            this.numericUpDown_total_time.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ViewDigitalInputDetailDlgSetting
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_cancel;
            this.Controls.Add(this.groupBox_data_read_period);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewDigitalInputDetailDlgSetting";
            this.ShowInTaskbar = false;
            this.groupBox_data_read_period.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_total_time)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
	}
}
