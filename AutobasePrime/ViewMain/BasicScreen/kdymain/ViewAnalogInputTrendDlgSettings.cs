using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputTrendDialogSettings.
	/// </summary>
	public class ViewAnalogInputTrendDialogSettings  : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.CheckBox checkBox_alarm_line;
		public System.Windows.Forms.CheckBox checkBox_guide_line;
		public System.Windows.Forms.ComboBox comboBox_dot_type;
		public System.Windows.Forms.CheckBox checkBox_curr_line;
		public System.Windows.Forms.ComboBox comboBox_data_read_period;
		private System.Windows.Forms.GroupBox groupBox_data_read_period;
		private System.Windows.Forms.GroupBox groupBox_view;
		private System.Windows.Forms.Label label_view_full;
		private System.Windows.Forms.Label label1;
        public MyNumericUpDown numericUpDown_view_base;
        public MyNumericUpDown numericUpDown_view_full;
		private System.Windows.Forms.GroupBox groupBox_show_time;
		private System.Windows.Forms.Label label4;
        public MyNumericUpDown numericUpDown_hour;
		private System.Windows.Forms.Label label3;
        public MyNumericUpDown numericUpDown_day;
		private System.Windows.Forms.Label label2;
        public MyNumericUpDown numericUpDown_month;
		private System.Windows.Forms.Label label5;
        public MyNumericUpDown numericUpDown_year;
        public CheckBox checkBoxUseSamePeriod;		
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewAnalogInputTrendDialogSettings(Form parent)
            
		{

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.numericUpDown_view_full.Maximum = decimal.MaxValue;
			this.numericUpDown_view_full.Minimum = decimal.MinValue;
			this.numericUpDown_view_base.Maximum = decimal.MaxValue;
			this.numericUpDown_view_base.Minimum = decimal.MinValue;

			//numericUpDown_view_base.DecimalPlaces = 2;
			//numericUpDown_view_base.Increment = 1;//0.25M;

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

        public void Set(bool use_same_period)
        {
            this.checkBoxUseSamePeriod.Checked = use_same_period;
        }

        public void Get(out bool use_same_period)
        {
            use_same_period = this.checkBoxUseSamePeriod.Checked;
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputTrendDialogSettings));
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_curr_line = new System.Windows.Forms.CheckBox();
            this.checkBox_alarm_line = new System.Windows.Forms.CheckBox();
            this.checkBox_guide_line = new System.Windows.Forms.CheckBox();
            this.comboBox_dot_type = new System.Windows.Forms.ComboBox();
            this.groupBox_data_read_period = new System.Windows.Forms.GroupBox();
            this.checkBoxUseSamePeriod = new System.Windows.Forms.CheckBox();
            this.comboBox_data_read_period = new System.Windows.Forms.ComboBox();
            this.groupBox_view = new System.Windows.Forms.GroupBox();
            this.numericUpDown_view_full = new AutoLibLocal.MyNumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label_view_full = new System.Windows.Forms.Label();
            this.numericUpDown_view_base = new AutoLibLocal.MyNumericUpDown();
            this.groupBox_show_time = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_hour = new AutoLibLocal.MyNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_day = new AutoLibLocal.MyNumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_month = new AutoLibLocal.MyNumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_year = new AutoLibLocal.MyNumericUpDown();
            this.groupBox2.SuspendLayout();
            this.groupBox_data_read_period.SuspendLayout();
            this.groupBox_view.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_full)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_base)).BeginInit();
            this.groupBox_show_time.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).BeginInit();
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
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_curr_line);
            this.groupBox2.Controls.Add(this.checkBox_alarm_line);
            this.groupBox2.Controls.Add(this.checkBox_guide_line);
            this.groupBox2.Controls.Add(this.comboBox_dot_type);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBox_curr_line
            // 
            resources.ApplyResources(this.checkBox_curr_line, "checkBox_curr_line");
            this.checkBox_curr_line.Name = "checkBox_curr_line";
            // 
            // checkBox_alarm_line
            // 
            resources.ApplyResources(this.checkBox_alarm_line, "checkBox_alarm_line");
            this.checkBox_alarm_line.Name = "checkBox_alarm_line";
            // 
            // checkBox_guide_line
            // 
            resources.ApplyResources(this.checkBox_guide_line, "checkBox_guide_line");
            this.checkBox_guide_line.Name = "checkBox_guide_line";
            // 
            // comboBox_dot_type
            // 
            resources.ApplyResources(this.comboBox_dot_type, "comboBox_dot_type");
            this.comboBox_dot_type.Items.AddRange(new object[] {
            resources.GetString("comboBox_dot_type.Items"),
            resources.GetString("comboBox_dot_type.Items1"),
            resources.GetString("comboBox_dot_type.Items2"),
            resources.GetString("comboBox_dot_type.Items3"),
            resources.GetString("comboBox_dot_type.Items4"),
            resources.GetString("comboBox_dot_type.Items5"),
            resources.GetString("comboBox_dot_type.Items6")});
            this.comboBox_dot_type.Name = "comboBox_dot_type";
            // 
            // groupBox_data_read_period
            // 
            this.groupBox_data_read_period.Controls.Add(this.checkBoxUseSamePeriod);
            this.groupBox_data_read_period.Controls.Add(this.comboBox_data_read_period);
            resources.ApplyResources(this.groupBox_data_read_period, "groupBox_data_read_period");
            this.groupBox_data_read_period.Name = "groupBox_data_read_period";
            this.groupBox_data_read_period.TabStop = false;
            // 
            // checkBoxUseSamePeriod
            // 
            resources.ApplyResources(this.checkBoxUseSamePeriod, "checkBoxUseSamePeriod");
            this.checkBoxUseSamePeriod.Name = "checkBoxUseSamePeriod";
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
            // groupBox_view
            // 
            this.groupBox_view.Controls.Add(this.numericUpDown_view_full);
            this.groupBox_view.Controls.Add(this.label1);
            this.groupBox_view.Controls.Add(this.label_view_full);
            this.groupBox_view.Controls.Add(this.numericUpDown_view_base);
            resources.ApplyResources(this.groupBox_view, "groupBox_view");
            this.groupBox_view.Name = "groupBox_view";
            this.groupBox_view.TabStop = false;
            // 
            // numericUpDown_view_full
            // 
            this.numericUpDown_view_full.DecimalPlaces = 2;
            resources.ApplyResources(this.numericUpDown_view_full, "numericUpDown_view_full");
            this.numericUpDown_view_full.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.numericUpDown_view_full.Minimum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            -2147483648});
            this.numericUpDown_view_full.Name = "numericUpDown_view_full";
            this.numericUpDown_view_full.SampleProperty = 0;
            this.numericUpDown_view_full.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label_view_full
            // 
            resources.ApplyResources(this.label_view_full, "label_view_full");
            this.label_view_full.Name = "label_view_full";
            // 
            // numericUpDown_view_base
            // 
            this.numericUpDown_view_base.DecimalPlaces = 2;
            resources.ApplyResources(this.numericUpDown_view_base, "numericUpDown_view_base");
            this.numericUpDown_view_base.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numericUpDown_view_base.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
            this.numericUpDown_view_base.Name = "numericUpDown_view_base";
            this.numericUpDown_view_base.SampleProperty = 0;
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
            // ViewAnalogInputTrendDialogSettings
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_cancel;
            this.Controls.Add(this.groupBox_show_time);
            this.Controls.Add(this.groupBox_view);
            this.Controls.Add(this.groupBox_data_read_period);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewAnalogInputTrendDialogSettings";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewAnalogInputTrendDialogSettings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox_data_read_period.ResumeLayout(false);
            this.groupBox_view.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_full)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_base)).EndInit();
            this.groupBox_show_time.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
            int limit = NetTools.TimeUtil.getmonthlimit((int)this.numericUpDown_year.Value, (int)this.numericUpDown_month.Value);

            if (this.numericUpDown_day.Value > limit)
                this.numericUpDown_day.Value = limit;

		    Close();
		}

		private void ViewAnalogInputTrendDialogSettings_Load(object sender, System.EventArgs e)
		{		
	
		}
	}
}
