using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewDigitalInputTrendDlgSetting.
	/// </summary>
	public class ViewDigitalInputTrendDlgSetting : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox_show_time;
		private System.Windows.Forms.Label label4;
		public MyNumericUpDown numericUpDown_hour;
		private System.Windows.Forms.Label label3;
		public MyNumericUpDown numericUpDown_day;
		private System.Windows.Forms.Label label2;
		public MyNumericUpDown numericUpDown_month;
		private System.Windows.Forms.Label label5;
		public MyNumericUpDown numericUpDown_year;
		private System.Windows.Forms.GroupBox groupBox_data_read_period;
		public System.Windows.Forms.ComboBox comboBox_data_read_period;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.CheckBox checkBox_curr_line;
		public System.Windows.Forms.CheckBox checkBox_guide_line;
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_OK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewDigitalInputTrendDlgSetting()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputTrendDlgSetting));
            this.groupBox_show_time = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_hour = new AutoLibLocal.MyNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_day = new AutoLibLocal.MyNumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_month = new AutoLibLocal.MyNumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_year = new AutoLibLocal.MyNumericUpDown();
            this.groupBox_data_read_period = new System.Windows.Forms.GroupBox();
            this.comboBox_data_read_period = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_curr_line = new System.Windows.Forms.CheckBox();
            this.checkBox_guide_line = new System.Windows.Forms.CheckBox();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox_show_time.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).BeginInit();
            this.groupBox_data_read_period.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_show_time
            // 
            this.groupBox_show_time.Controls.Add(this.label4);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_hour);
            this.groupBox_show_time.Controls.Add(this.label3);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_day);
            this.groupBox_show_time.Controls.Add(this.label2);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_month);
            this.groupBox_show_time.Controls.Add(this.label5);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_year);
            resources.ApplyResources(this.groupBox_show_time, "groupBox_show_time");
            this.groupBox_show_time.Name = "groupBox_show_time";
            this.groupBox_show_time.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDown_hour
            // 
            resources.ApplyResources(this.numericUpDown_hour, "numericUpDown_hour");
            this.numericUpDown_hour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDown_hour.Name = "numericUpDown_hour";
            this.numericUpDown_hour.SampleProperty = 0;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDown_day
            // 
            resources.ApplyResources(this.numericUpDown_day, "numericUpDown_day");
            this.numericUpDown_day.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown_day.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_day.Name = "numericUpDown_day";
            this.numericUpDown_day.SampleProperty = 0;
            this.numericUpDown_day.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDown_month
            // 
            resources.ApplyResources(this.numericUpDown_month, "numericUpDown_month");
            this.numericUpDown_month.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown_month.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_month.Name = "numericUpDown_month";
            this.numericUpDown_month.SampleProperty = 0;
            this.numericUpDown_month.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDown_year
            // 
            resources.ApplyResources(this.numericUpDown_year, "numericUpDown_year");
            this.numericUpDown_year.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_year.Name = "numericUpDown_year";
            this.numericUpDown_year.SampleProperty = 0;
            this.numericUpDown_year.Value = new decimal(new int[] {
            2002,
            0,
            0,
            0});
            // 
            // groupBox_data_read_period
            // 
            this.groupBox_data_read_period.Controls.Add(this.comboBox_data_read_period);
            resources.ApplyResources(this.groupBox_data_read_period, "groupBox_data_read_period");
            this.groupBox_data_read_period.Name = "groupBox_data_read_period";
            this.groupBox_data_read_period.TabStop = false;
            // 
            // comboBox_data_read_period
            // 
            this.comboBox_data_read_period.DropDownWidth = 104;
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
            resources.GetString("comboBox_data_read_period.Items9"),
            resources.GetString("comboBox_data_read_period.Items10"),
            resources.GetString("comboBox_data_read_period.Items11")});
            this.comboBox_data_read_period.Name = "comboBox_data_read_period";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_curr_line);
            this.groupBox2.Controls.Add(this.checkBox_guide_line);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBox_curr_line
            // 
            resources.ApplyResources(this.checkBox_curr_line, "checkBox_curr_line");
            this.checkBox_curr_line.Name = "checkBox_curr_line";
            // 
            // checkBox_guide_line
            // 
            resources.ApplyResources(this.checkBox_guide_line, "checkBox_guide_line");
            this.checkBox_guide_line.Name = "checkBox_guide_line";
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
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // ViewDigitalInputTrendDlgSetting
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox_show_time);
            this.Controls.Add(this.groupBox_data_read_period);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewDigitalInputTrendDlgSetting";
            this.ShowInTaskbar = false;
            this.groupBox_show_time.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).EndInit();
            this.groupBox_data_read_period.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        private void button_OK_Click(object sender, EventArgs e)
        {
            int limit = NetTools.TimeUtil.getmonthlimit((int)this.numericUpDown_year.Value, (int)this.numericUpDown_month.Value);

            if (this.numericUpDown_day.Value > limit)
                this.numericUpDown_day.Value = limit;
        }
	}
}
